using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CustomWPFControls.ViewModels;

namespace TestHelper.CustomWPFControls.Extensions;

/// <summary>
/// Test-Extensions für CollectionViewModel - vereinfacht Test-Setup mit garantierter Synchronisation.
/// </summary>
public static class CollectionViewModelExtensions
{
    /// <summary>
    /// Lädt Models UND wartet auf Items-Synchronisation (für Tests).
    /// Verwendet CollectionChanged-Events für deterministische Synchronisation.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ</typeparam>
    /// <param name="viewModel">Das CollectionViewModel</param>
    /// <param name="models">Die zu ladenden Models (null wird als leere Liste behandelt)</param>
    /// <param name="expectedCount">Erwartete Anzahl (default: models.Count)</param>
    /// <param name="timeout">Timeout für Synchronisation (default: 500ms)</param>
    /// <exception cref="ArgumentNullException">Wenn viewModel null ist</exception>
    /// <exception cref="TimeoutException">Wenn Synchronisation länger als timeout dauert</exception>
    /// <remarks>
    /// <para>
    /// Diese Extension garantiert, dass nach dem Aufruf die asynchrone TransformTo-Synchronisation
    /// abgeschlossen ist und viewModel.Items die erwartete Anzahl an Einträgen enthält.
    /// </para>
    /// <para>
    /// <b>Verwendung in Tests:</b>
    /// </para>
    /// <code>
    /// // Standard: Lädt Models und wartet auf Synchronisation
    /// var models = new[] { model1, model2, model3 };
    /// viewModel.LoadModelsAndWait(models);
    /// Assert.Equal(3, viewModel.Items.Count); // Garantiert synchronisiert!
    /// 
    /// // Mit expliziter Anzahl (z.B. wenn Duplikate ignoriert werden)
    /// viewModel.LoadModelsAndWait(modelsWithDuplicates, expectedCount: 3);
    /// 
    /// // Mit custom Timeout
    /// viewModel.LoadModelsAndWait(largeModelSet, timeout: TimeSpan.FromSeconds(2));
    /// </code>
    /// </remarks>
    public static void LoadModelsAndWait<TModel, TViewModel>(
        this CollectionViewModel<TModel, TViewModel> viewModel,
        IEnumerable<TModel>? models,
        int? expectedCount = null,
        TimeSpan? timeout = null)
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        timeout ??= TimeSpan.FromMilliseconds(500);
        
        var modelList = models?.ToList() ?? new List<TModel>();
        expectedCount ??= modelList.Count;
        
        // TaskCompletionSource für Event-basierte Synchronisation
        var tcs = new TaskCompletionSource<bool>();
        var items = (INotifyCollectionChanged)viewModel.Items;
        
        NotifyCollectionChangedEventHandler? handler = null;
        handler = (s, e) =>
        {
            if (viewModel.Items.Count >= expectedCount.Value)
            {
                items.CollectionChanged -= handler;
                tcs.TrySetResult(true);
            }
        };
        
        // ?? Event ZUERST registrieren (Race Condition vermeiden)
        items.CollectionChanged += handler;
        
        try
        {
            // Dann erst LoadModels aufrufen
            viewModel.LoadModels(modelList);
            
            // Warten mit Timeout
            if (!tcs.Task.Wait(timeout.Value))
            {
                throw new TimeoutException(
                    $"Items-Synchronisation timeout nach {timeout.Value.TotalMilliseconds}ms. " +
                    $"Erwartet: {expectedCount.Value}, Aktuell: {viewModel.Items.Count}");
            }
        }
        finally
        {
            // ?? Cleanup: Event immer abmelden (auch bei Exceptions)
            items.CollectionChanged -= handler;
        }
    }
    
    /// <summary>
    /// Lädt Models asynchron UND wartet auf Items-Synchronisation (für async Tests).
    /// </summary>
    /// <inheritdoc cref="LoadModelsAndWait{TModel,TViewModel}(CollectionViewModel{TModel,TViewModel},IEnumerable{TModel},int?,TimeSpan?)"/>
    /// <remarks>
    /// Async-Variante für moderne async/await Test-Patterns.
    /// </remarks>
    /// <example>
    /// <code>
    /// [Fact]
    /// public async Task Test_LoadModelsAsync()
    /// {
    ///     var models = await repository.GetAllAsync();
    ///     await viewModel.LoadModelsAndWaitAsync(models);
    ///     Assert.Equal(3, viewModel.Items.Count);
    /// }
    /// </code>
    /// </example>
    public static async Task LoadModelsAndWaitAsync<TModel, TViewModel>(
        this CollectionViewModel<TModel, TViewModel> viewModel,
        IEnumerable<TModel>? models,
        int? expectedCount = null,
        TimeSpan? timeout = null)
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        timeout ??= TimeSpan.FromMilliseconds(500);
        
        var modelList = models?.ToList() ?? new List<TModel>();
        expectedCount ??= modelList.Count;
        
        var tcs = new TaskCompletionSource<bool>();
        var items = (INotifyCollectionChanged)viewModel.Items;
        
        NotifyCollectionChangedEventHandler? handler = null;
        handler = (s, e) =>
        {
            if (viewModel.Items.Count >= expectedCount.Value)
            {
                items.CollectionChanged -= handler;
                tcs.TrySetResult(true);
            }
        };
        
        items.CollectionChanged += handler;
        
        try
        {
            viewModel.LoadModels(modelList);
            
            // Async-Variante mit CancellationToken
            using var cts = new System.Threading.CancellationTokenSource(timeout.Value);
            await tcs.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException(
                $"Items-Synchronisation timeout nach {timeout.Value.TotalMilliseconds}ms. " +
                $"Erwartet: {expectedCount.Value}, Aktuell: {viewModel.Items.Count}");
        }
        finally
        {
            items.CollectionChanged -= handler;
        }
    }
}
