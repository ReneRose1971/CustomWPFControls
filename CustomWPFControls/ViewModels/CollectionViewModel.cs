using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DataStores.Abstractions;

namespace CustomWPFControls.ViewModels
{
    /// <summary>
    /// Collection-ViewModel mit DataStore-Integration und bidirektionaler Synchronisation.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    /// <remarks>
    /// <para>
    /// <b>DataStores Facade:</b> Diese Klasse nutzt IDataStores für den Zugriff auf den Model-Store.
    /// </para>
    /// <para>
    /// <b>Comparer-Auflösung:</b> Der EqualityComparer für TModel wird über IEqualityComparerService
    /// aufgelöst und fällt automatisch auf EqualityComparer&lt;T&gt;.Default zurück.
    /// </para>
    /// <para>
    /// <b>Bidirektionale Synchronisation:</b> Änderungen im DataStore werden automatisch
    /// in die ViewModel-Collection übernommen und umgekehrt.
    /// </para>
    /// </remarks>
    public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly IDataStore<TModel> _dataStore;
        private readonly Factories.IViewModelFactory<TModel, TViewModel> _viewModelFactory;
        private readonly IEqualityComparer<TModel> _modelComparer;
        
        private readonly ObservableCollection<TViewModel> _viewModels;
        private readonly Dictionary<TModel, TViewModel> _modelToViewModelMap;
        
        private TViewModel? _selectedItem;
        private bool _disposed;

        /// <summary>
        /// Erstellt ein CollectionViewModel mit DataStore-Integration.
        /// </summary>
        /// <param name="dataStores">IDataStores Facade für Zugriff auf Stores.</param>
        /// <param name="viewModelFactory">Factory zur Erstellung von ViewModels.</param>
        /// <param name="comparerService">Service zur Auflösung von EqualityComparern.</param>
        /// <exception cref="ArgumentNullException">
        /// Wenn einer der Parameter null ist.
        /// </exception>
        public CollectionViewModel(
            IDataStores dataStores,
            Factories.IViewModelFactory<TModel, TViewModel> viewModelFactory,
            IEqualityComparerService comparerService)
        {
            if (dataStores == null) throw new ArgumentNullException(nameof(dataStores));
            if (viewModelFactory == null) throw new ArgumentNullException(nameof(viewModelFactory));
            if (comparerService == null) throw new ArgumentNullException(nameof(comparerService));

            // Model-Store aus Facade abrufen
            _dataStore = dataStores.GetGlobal<TModel>();
            _viewModelFactory = viewModelFactory;
            
            // Model-Comparer über ComparerService auflösen (automatischer Fallback auf Default)
            _modelComparer = comparerService.GetComparer<TModel>();
            
            _modelToViewModelMap = new Dictionary<TModel, TViewModel>(_modelComparer);
            _viewModels = new ObservableCollection<TViewModel>();
            
            // Initiale ViewModels erstellen
            foreach (var model in _dataStore.Items)
            {
                var viewModel = CreateAndMapViewModel(model);
                _viewModels.Add(viewModel);
            }
            
            // Synchronisation: DataStore ? ViewModels
            _dataStore.Changed += OnDataStoreChanged;
            
            Items = new ReadOnlyObservableCollection<TViewModel>(_viewModels);
        }

        /// <summary>
        /// Schreibgeschützte Sicht auf die ViewModels (für View-Binding).
        /// </summary>
        public ReadOnlyObservableCollection<TViewModel> Items { get; }

        /// <summary>
        /// Ausgewähltes ViewModel.
        /// </summary>
        public TViewModel? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Anzahl der ViewModels.
        /// </summary>
        public int Count => _viewModels.Count;

        #region Private: Synchronisation DataStore ? ViewModels

        private void OnDataStoreChanged(object? sender, DataStoreChangedEventArgs<TModel> e)
        {
            switch (e.ChangeType)
            {
                case DataStoreChangeType.Add:
                    foreach (TModel model in e.AffectedItems)
                    {
                        if (!_modelToViewModelMap.ContainsKey(model))
                        {
                            var viewModel = CreateAndMapViewModel(model);
                            _viewModels.Add(viewModel);
                        }
                    }
                    OnPropertyChanged(nameof(Count));
                    break;

                case DataStoreChangeType.Remove:
                    foreach (TModel model in e.AffectedItems)
                    {
                        if (_modelToViewModelMap.TryGetValue(model, out var viewModel))
                        {
                            if (ReferenceEquals(_selectedItem, viewModel))
                            {
                                SelectedItem = null;
                            }
                            
                            _viewModels.Remove(viewModel);
                            RemoveAndDisposeViewModel(model, viewModel);
                        }
                    }
                    OnPropertyChanged(nameof(Count));
                    break;

                case DataStoreChangeType.Clear:
                    if (_selectedItem != null)
                    {
                        SelectedItem = null;
                    }
                    
                    foreach (var kvp in _modelToViewModelMap.ToList())
                    {
                        _viewModels.Remove(kvp.Value);
                        DisposeViewModel(kvp.Value);
                    }
                    _modelToViewModelMap.Clear();
                    OnPropertyChanged(nameof(Count));
                    break;

                case DataStoreChangeType.Reset:
                    if (_selectedItem != null)
                    {
                        SelectedItem = null;
                    }
                    
                    foreach (var kvp in _modelToViewModelMap.ToList())
                    {
                        _viewModels.Remove(kvp.Value);
                        DisposeViewModel(kvp.Value);
                    }
                    _modelToViewModelMap.Clear();
                    
                    foreach (var model in _dataStore.Items)
                    {
                        var viewModel = CreateAndMapViewModel(model);
                        _viewModels.Add(viewModel);
                    }
                    OnPropertyChanged(nameof(Count));
                    break;

                case DataStoreChangeType.BulkAdd:
                    foreach (TModel model in e.AffectedItems)
                    {
                        if (!_modelToViewModelMap.ContainsKey(model))
                        {
                            var viewModel = CreateAndMapViewModel(model);
                            _viewModels.Add(viewModel);
                        }
                    }
                    OnPropertyChanged(nameof(Count));
                    break;
            }
        }

        #endregion

        #region Private: ViewModel Lifecycle

        private TViewModel CreateAndMapViewModel(TModel model)
        {
            var viewModel = _viewModelFactory.Create(model);
            _modelToViewModelMap[model] = viewModel;
            return viewModel;
        }

        private void RemoveAndDisposeViewModel(TModel model, TViewModel viewModel)
        {
            _modelToViewModelMap.Remove(model);
            OnViewModelRemoving(viewModel);
            DisposeViewModel(viewModel);
        }

        private void DisposeViewModel(TViewModel viewModel)
        {
            if (viewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        /// <summary>
        /// Hook: Wird aufgerufen, bevor ein ViewModel disposed wird.
        /// </summary>
        protected virtual void OnViewModelRemoving(TViewModel viewModel)
        {
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

            _dataStore.Changed -= OnDataStoreChanged;

            foreach (var viewModel in _modelToViewModelMap.Values)
            {
                DisposeViewModel(viewModel);
            }
            _modelToViewModelMap.Clear();
            
            _viewModels.Clear();
        }

        #endregion
    }
}