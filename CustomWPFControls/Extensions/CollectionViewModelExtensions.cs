using System;
using System.Collections.Generic;
using System.Linq;
using CustomWPFControls.ViewModels;

namespace CustomWPFControls.Extensions;

/// <summary>
/// Extension-Methoden für CollectionViewModel zur Vereinfachung häufiger Operationen.
/// </summary>
public static class CollectionViewModelExtensions
{
    /// <summary>
    /// Lädt Daten aus einer Quelle in den lokalen ModelStore und selektiert optional das erste Item.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    /// <param name="collectionViewModel">Die CollectionViewModel-Instanz.</param>
    /// <param name="data">Die zu ladenden Model-Daten. Null wird als leere Collection behandelt.</param>
    /// <param name="selectFirst">
    /// Wenn true, wird das erste Item automatisch selektiert (unabhängig vom aktuellen SelectedItem-Status).
    /// Wenn false, bleibt die Selektion unverändert.
    /// </param>
    /// <exception cref="ArgumentNullException">Wenn <paramref name="collectionViewModel"/> null ist.</exception>
    /// <remarks>
    /// <para>
    /// Diese Methode ersetzt den gesamten Inhalt des ModelStore durch die übergebenen Daten.
    /// Der vorherige Inhalt wird vollständig entfernt (Clear + AddRange).
    /// </para>
    /// <para>
    /// <b>Verhalten bei leeren/null Daten:</b> Die Methode wirft keine Exception bei leerer oder null Collection.
    /// Das gebundene UI-Control bleibt einfach leer. Die Entscheidung über UI-Feedback (z.B. Hinweismeldung)
    /// liegt beim Konsumenten der Library.
    /// </para>
    /// <para>
    /// <b>Selektionsverhalten:</b> Wenn selectFirst = true und Daten vorhanden sind, wird das erste Item
    /// IMMER selektiert, auch wenn bereits ein anderes Item selektiert war. Dies stellt sicheres,
    /// deterministisches Verhalten bei Daten-Reload sicher.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Standard-Nutzung: Daten laden und erstes Item selektieren
    /// collectionViewModel.LoadData(customers);
    /// 
    /// // Daten laden ohne automatische Selektion
    /// collectionViewModel.LoadData(customers, selectFirst: false);
    /// 
    /// // Leere Daten sind valide (z.B. nach Filter ohne Treffer)
    /// collectionViewModel.LoadData(Enumerable.Empty&lt;Customer&gt;());
    /// 
    /// // Null wird als leere Collection behandelt
    /// collectionViewModel.LoadData(null);
    /// </code>
    /// </example>
    public static void LoadData<TModel, TViewModel>(
        this CollectionViewModel<TModel, TViewModel> collectionViewModel,
        IEnumerable<TModel>? data,
        bool selectFirst = true)
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        if (collectionViewModel == null)
            throw new ArgumentNullException(nameof(collectionViewModel));

        data ??= Enumerable.Empty<TModel>();

        collectionViewModel.ModelStore.Clear();
        collectionViewModel.ModelStore.AddRange(data);

        if (selectFirst && collectionViewModel.Items.Count > 0)
        {
            collectionViewModel.SelectedItem = collectionViewModel.Items[0];
        }
    }
}
