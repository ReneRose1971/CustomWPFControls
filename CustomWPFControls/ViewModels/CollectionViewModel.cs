using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// <b>High-Level API:</b> Bietet Remove/RemoveRange/Clear Methoden für Commands.
    /// SelectedItem/SelectedItems sind reine UI-Binding Properties - WPF cleared diese automatisch.
    /// </para>
    /// </remarks>
    public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly IDataStore<TModel> _modelStore;
        private readonly IDataStore<TViewModel> _viewModelStore;
        private readonly ObservableCollection<TViewModel> _items;
        private readonly ObservableCollection<TViewModel> _selectedItems;
        private readonly IDisposable _unidirectionalSync;
        private bool _disposed;

        /// <summary>
        /// Erstellt ein CollectionViewModel mit DataStore-Integration via TransformTo.
        /// </summary>
        /// <param name="dataStores">IDataStores Facade für Zugriff auf Stores.</param>
        /// <param name="viewModelFactory">Factory zur Erstellung von ViewModels.</param>
        /// <param name="comparerService">Service zur Auflösung von EqualityComparern.</param>
        /// <exception cref="ArgumentNullException">Wenn einer der Parameter null ist.</exception>
        public CollectionViewModel(
            IDataStores dataStores,
            Factories.IViewModelFactory<TModel, TViewModel> viewModelFactory,
            IEqualityComparerService comparerService)
        {
            if (dataStores == null) throw new ArgumentNullException(nameof(dataStores));
            if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));
            if (comparerService == null) throw new ArgumentNullException(nameof(comparerService));

            _modelStore = dataStores.GetGlobal<TModel>();
            var modelComparer = comparerService.GetComparer<TModel>();
            _viewModelStore = dataStores.CreateLocal<TViewModel>();

            // ════════════════════════════════════════════════════════════
            // TransformTo: Model-Store → ViewModel-Store
            // Automatische Synchronisation, ViewModel-Erstellung und Dispose!
            // ════════════════════════════════════════════════════════════
            Func<TModel, TViewModel> factoryFunc = model => viewModelFactory.Create(model);
            Func<TModel, TViewModel, bool> comparerFunc = (m, vm) => modelComparer.Equals(m, vm.Model);

            _unidirectionalSync = _modelStore.TransformTo<TModel, TViewModel>(
                _viewModelStore, factoryFunc, comparerFunc);
            
            _selectedItems = new ObservableCollection<TViewModel>();
            _items = new ObservableCollection<TViewModel>(_viewModelStore.Items);
            
            Items = new ReadOnlyObservableCollection<TViewModel>(_items);
            
            // Synchronisation: _viewModelStore → _items
            _viewModelStore.Changed += OnViewModelStoreChanged;
        }

        private void OnViewModelStoreChanged(object? sender, DataStoreChangedEventArgs<TViewModel> e)
        {
            switch (e.ChangeType)
            {
                case DataStoreChangeType.Add:
                    foreach (var vm in e.AffectedItems)
                    {
                        if (!_items.Contains(vm))
                            _items.Add(vm);
                    }
                    break;

                case DataStoreChangeType.Remove:
                    foreach (var vm in e.AffectedItems)
                    {
                        _items.Remove(vm);
                    }
                    break;

                case DataStoreChangeType.Clear:
                    _items.Clear();
                    break;

                case DataStoreChangeType.Reset:
                    _items.Clear();
                    foreach (var vm in _viewModelStore.Items)
                    {
                        _items.Add(vm);
                    }
                    break;

                case DataStoreChangeType.BulkAdd:
                    foreach (var vm in e.AffectedItems)
                    {
                        if (!_items.Contains(vm))
                            _items.Add(vm);
                    }
                    break;
            }
            
            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        /// Schreibgeschützte Sicht auf die ViewModels (für View-Binding).
        /// </summary>
        public ReadOnlyObservableCollection<TViewModel> Items { get; }

        /// <summary>
        /// Ausgewähltes ViewModel (nur für UI-Binding, Single-Selection).
        /// Wird automatisch von WPF gecleared, wenn Item aus Items entfernt wird.
        /// </summary>
        public TViewModel? SelectedItem { get; set; }

        /// <summary>
        /// Ausgewählte ViewModels (nur für UI-Binding, Multi-Selection).
        /// Verwenden Sie MultiSelectBehavior für automatische Synchronisation mit ListBox.
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
        public int Count => _items.Count;

        #region Public API: Collection Manipulation

        /// <summary>
        /// Entfernt ein ViewModel und dessen zugehöriges Model aus dem DataStore.
        /// TransformTo disposed das ViewModel automatisch.
        /// SelectedItem wird automatisch von WPF gecleared.
        /// </summary>
        /// <param name="item">Das zu entfernende ViewModel.</param>
        /// <returns>True, wenn das Item entfernt wurde; andernfalls false.</returns>
        /// <exception cref="ArgumentNullException">Wenn <paramref name="item"/> null ist.</exception>
        public bool Remove(TViewModel item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            return _modelStore.Remove(item.Model);
        }

        /// <summary>
        /// Entfernt mehrere ViewModels und deren zugehörige Models aus dem DataStore.
        /// TransformTo disposed alle ViewModels automatisch.
        /// SelectedItem/SelectedItems werden automatisch von WPF gecleared.
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
                    removedCount++;
            }
            
            return removedCount;
        }

        /// <summary>
        /// Entfernt alle ViewModels aus der Collection.
        /// TransformTo disposed alle ViewModels automatisch.
        /// SelectedItem/SelectedItems werden automatisch von WPF gecleared.
        /// </summary>
        public void Clear()
        {
            _modelStore.Clear();
        }

        /// <summary>
        /// Prüft, ob ein ViewModel in der Collection enthalten ist.
        /// </summary>
        /// <param name="item">Das zu prüfende ViewModel.</param>
        /// <returns>True, wenn das Item enthalten ist; andernfalls false.</returns>
        public bool Contains(TViewModel item)
        {
            return item != null && _items.Contains(item);
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

            // TransformTo-Sync disposed
            _unidirectionalSync?.Dispose();

            // Unsubscribe from store events
            _viewModelStore.Changed -= OnViewModelStoreChanged;

            // ViewModel-Store disposed (TransformTo disposed automatisch alle ViewModels!)
            if (_viewModelStore is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            _items.Clear();
            _selectedItems.Clear();
        }

        #endregion
    }
}