using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CustomWPFControls.Factories;
using CustomWPFControls.Services;
using DataStores.Abstractions;
using DataStores.Extensions;

namespace CustomWPFControls.ViewModels
{
    /// <summary>
    /// Collection-ViewModel mit DataStore-Integration und automatischer Synchronisation via TransformTo.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    /// <remarks>
    /// <para>
    /// <b>TransformTo-Integration:</b> Diese Klasse nutzt die TransformTo-Extension für automatische
    /// Synchronisation zwischen Model-Store und ViewModel-Store. ViewModels werden automatisch erstellt,
    /// synchronisiert und disposed.
    /// </para>
    /// <para>
    /// <b>ObservableCollection-Sync:</b> Nutzt ToReadOnlyObservableCollection() für automatische
    /// Synchronisation zwischen DataStore und WPF-bindbare ObservableCollection.
    /// </para>
    /// <para>
    /// <b>High-Level API:</b> Bietet Remove/RemoveRange/Clear/LoadModels Methoden für Commands.
    /// SelectedItem/SelectedItems werden automatisch invalidiert wenn Items via diese Methoden entfernt werden.
    /// </para>
    /// </remarks>
    public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly IViewModelFactory<TModel, TViewModel> _viewModelFactory;
        private readonly IEqualityComparer<TModel> _modelComparer;
        private readonly Func<TModel, TViewModel> _factoryFunc;
        private readonly Func<TModel, TViewModel, bool> _comparerFunc;
        
        private readonly IDataStore<TModel> _modelStore;
        private readonly IDataStore<TViewModel> _viewModelStore;
        private readonly ReadOnlyObservableCollectionSynchronization<TViewModel> _itemsSync;
        private readonly ObservableCollection<TViewModel> _selectedItems;
        private IDisposable _unidirectionalSync;
        private bool _disposed;
        private TViewModel? _selectedItem;

        /// <summary>
        /// Erstellt ein CollectionViewModel mit CustomWPFServices Facade.
        /// </summary>
        /// <param name="services">CustomWPFControls Service-Facade.</param>
        /// <param name="viewModelFactory">Factory zur Erstellung von ViewModels.</param>
        /// <exception cref="ArgumentNullException">Wenn einer der Parameter null ist.</exception>
        public CollectionViewModel(
            ICustomWPFServices services,
            IViewModelFactory<TModel, TViewModel> viewModelFactory)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));

            _viewModelFactory = viewModelFactory;
            _modelComparer = services.ComparerService.GetComparer<TModel>();
            
            _factoryFunc = model => _viewModelFactory.Create(model);
            _comparerFunc = (m, vm) => _modelComparer.Equals(m, vm.Model);

            _modelStore = services.DataStores.CreateLocal<TModel>();
            _viewModelStore = services.DataStores.CreateLocal<TViewModel>();

            // ════════════════════════════════════════════════════════════
            // TransformTo: Model-Store → ViewModel-Store
            // Automatische Synchronisation, ViewModel-Erstellung und Dispose!
            // ════════════════════════════════════════════════════════════
            _unidirectionalSync = _modelStore.TransformTo<TModel, TViewModel>(
                _viewModelStore, _factoryFunc, _comparerFunc);
            
            _selectedItems = new ObservableCollection<TViewModel>();

            // ════════════════════════════════════════════════════════════
            // ToReadOnlyObservableCollection: ViewModel-Store → WPF ObservableCollection
            // Automatische Synchronisation ohne manuelle Event-Handler!
            // ════════════════════════════════════════════════════════════
            _itemsSync = _viewModelStore.ToReadOnlyObservableCollection(
                comparer: services.ComparerService.GetComparer<TViewModel>());
            
            Items = _itemsSync.Collection;
            
            // PropertyChanged für Count bei Store-Änderungen
            _viewModelStore.Changed += OnViewModelStoreChanged;
        }

        private void OnViewModelStoreChanged(object? sender, DataStoreChangedEventArgs<TViewModel> e)
        {
            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        /// Schreibgeschützte Sicht auf die ViewModels (für View-Binding).
        /// </summary>
        /// <remarks>
        /// Wird automatisch via ToReadOnlyObservableCollection() synchronisiert.
        /// Alle Änderungen am _viewModelStore werden automatisch in Items reflektiert.
        /// </remarks>
        public ReadOnlyObservableCollection<TViewModel> Items { get; }

        /// <summary>
        /// Model-DataStore für diese Collection (readonly).
        /// Verwenden Sie die Add/Remove/Clear-Methoden des Stores, um Daten zu manipulieren.
        /// </summary>
        /// <remarks>
        /// Der ModelStore ist readonly und wird im Konstruktor als lokaler Store erstellt.
        /// Alle Änderungen am ModelStore werden automatisch via TransformTo in die ViewModel-Collection übertragen.
        /// </remarks>
        public IDataStore<TModel> ModelStore => _modelStore;

        /// <summary>
        /// Ausgewähltes ViewModel (Single-Selection).
        /// Wird automatisch auf null gesetzt wenn das Item via Remove/RemoveRange/Clear entfernt wird.
        /// Feuert PropertyChanged Event bei Änderungen.
        /// </summary>
        public TViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Ausgewählte ViewModels (Multi-Selection).
        /// Verwenden Sie MultiSelectBehavior für automatische Synchronisation mit ListBox.
        /// Entfernte Items werden automatisch aus dieser Collection entfernt (via Remove/RemoveRange/Clear).
        /// </summary>
        /// <example>
        /// <code>
        /// &lt;ListBox SelectionMode="Multiple"
        ///          behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedItems}" /&gt;
        /// </code>
        /// </example>
        public ObservableCollection<TViewModel> SelectedItems => _selectedItems;

        /// <summary>
        /// Anzahl der ViewModels.
        /// </summary>
        public int Count => _viewModelStore.Items.Count;

        #region Public API: Collection Manipulation

        /// <summary>
        /// Entfernt ein ViewModel und dessen zugehöriges Model aus dem DataStore.
        /// TransformTo synchronisiert automatisch: ViewModel wird aus _viewModelStore entfernt und disposed.
        /// ToReadOnlyObservableCollection synchronisiert automatisch: ViewModel wird aus Items entfernt.
        /// Invalidiert automatisch SelectedItem und SelectedItems wenn das Item selektiert war.
        /// </summary>
        /// <param name="item">Das zu entfernende ViewModel.</param>
        /// <returns>True, wenn das Item entfernt wurde; andernfalls false.</returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="item"/> null ist.</exception>
        public bool Remove(TViewModel item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            
            var removed = _modelStore.Remove(item.Model);
            
            if (removed)
            {
                // Invalidierung: SelectedItem/SelectedItems aktualisieren
                if (SelectedItem == item)
                    SelectedItem = null;
                    
                _selectedItems.Remove(item);
            }
            
            return removed;
        }

        /// <summary>
        /// Entfernt mehrere ViewModels und deren zugehörige Models aus dem DataStore.
        /// TransformTo synchronisiert automatisch: ViewViews werden aus _viewModelStore entfernt und disposed.
        /// ToReadOnlyObservableCollection synchronisiert automatisch: ViewModels werden aus Items entfernt.
        /// Invalidiert automatisch SelectedItem und SelectedItems für alle entfernten Items.
        /// </summary>
        /// <param name="items">Die zu entfernenden ViewModels.</param>
        /// <returns>Anzahl der erfolgreich entfernten Items.</returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="items"/> null ist.</exception>
        public int RemoveRange(IEnumerable<TViewModel> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var itemList = items.ToList();
            if (itemList.Count == 0) return 0;

            int removedCount = 0;
            foreach (var item in itemList)
            {
                if (_modelStore.Remove(item.Model))
                {
                    removedCount++;
                    
                    // Invalidierung: SelectedItem/SelectedItems aktualisieren
                    if (SelectedItem == item)
                        SelectedItem = null;
                        
                    _selectedItems.Remove(item);
                }
            }
            
            return removedCount;
        }

        /// <summary>
        /// Entfernt alle ViewModels aus der Collection.
        /// TransformTo synchronisiert automatisch: Alle ViewModels werden aus _viewModelStore entfernt und disposed.
        /// ToReadOnlyObservableCollection synchronisiert automatisch: Items wird geleert.
        /// Invalidiert automatisch SelectedItem und SelectedItems.
        /// </summary>
        public void Clear()
        {
            _modelStore.Clear();
            
            // Invalidierung: Selektion komplett zurücksetzen
            SelectedItem = null;
            _selectedItems.Clear();
        }

        /// <summary>
        /// Ersetzt alle Models in der Collection (Clear + AddRange).
        /// TransformTo synchronisiert automatisch: Alle alten ViewModels werden entfernt/disposed, neue werden erstellt.
        /// ToReadOnlyObservableCollection synchronisiert automatisch: Items wird aktualisiert.
        /// Invalidiert automatisch SelectedItem und SelectedItems.
        /// </summary>
        /// <param name="models">Die neuen Models, die geladen werden sollen.</param>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="models"/> null ist.</exception>
        /// <remarks>
        /// Diese Methode ist eine atomare Operation, die erst alle vorhandenen Models entfernt (via <see cref="Clear"/>)
        /// und dann die neuen Models hinzufügt. Die Selection wird dabei automatisch zurückgesetzt.
        /// ViewModels werden automatisch erstellt und synchronisiert.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Ersetze alle Items in der Collection
        /// var newModels = new[] { model1, model2, model3 };
        /// viewModel.LoadModels(newModels);
        /// // Items.Count == 3, SelectedItem == null
        /// </code>
        /// </example>
        public void LoadModels(IEnumerable<TModel> models)
        {
            if (models == null) throw new ArgumentNullException(nameof(models));
            
            Clear();
            _modelStore.AddRange(models);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Unsubscribe from ViewModel store events
            _viewModelStore.Changed -= OnViewModelStoreChanged;

            // Clear stores before disposing sync mechanisms
            // This ensures Items collection is emptied via automatic synchronization
            _modelStore.Clear();
            
            // TransformTo-Sync disposed
            _unidirectionalSync?.Dispose();

            // ObservableCollection-Sync disposed
            _itemsSync?.Dispose();

            // ViewModel-Store disposed (TransformTo disposed automatisch alle ViewModels!)
            if (_viewModelStore is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            _selectedItems.Clear();
        }

        #endregion
    }
}