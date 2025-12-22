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
    /// Unterstützt zwei Modi:
    /// 1. DataStore-Modus: Models (TModel) werden in ViewModels (TViewModel) gewrappt
    /// 2. Direct-Modus: Items direkt ohne Wrapping (für einfache Szenarien)
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly IDataStore<TModel>? _dataStore;
        private readonly Factories.IViewModelFactory<TModel, TViewModel>? _viewModelFactory;
        private readonly IEqualityComparer<TModel>? _modelComparer;
        
        private readonly ObservableCollection<TViewModel> _viewModels;
        private readonly Dictionary<TModel, TViewModel>? _modelToViewModelMap;
        
        private TViewModel? _selectedItem;
        private bool _disposed;

        /// <summary>
        /// Erstellt ein CollectionViewModel mit DataStore-Integration.
        /// </summary>
        /// <param name="dataStore">DataStore für Models.</param>
        /// <param name="viewModelFactory">Factory zur Erstellung von ViewModels.</param>
        /// <param name="modelComparer">Comparer zum Vergleich von Models.</param>
        public CollectionViewModel(
            IDataStore<TModel> dataStore,
            Factories.IViewModelFactory<TModel, TViewModel> viewModelFactory,
            IEqualityComparer<TModel> modelComparer)
        {
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
            _modelComparer = modelComparer ?? throw new ArgumentNullException(nameof(modelComparer));
            
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

        /// <summary>
        /// Fügt ein neues Model hinzu (erstellt automatisch ViewModel).
        /// </summary>
        public bool AddModel(TModel model)
        {
            if (_dataStore == null || _modelToViewModelMap == null)
                throw new InvalidOperationException("DataStore nicht initialisiert.");

            if (model == null)
                return false;

            if (_modelToViewModelMap.ContainsKey(model))
                return false;

            _dataStore.Add(model);
            return true;
        }

        /// <summary>
        /// Entfernt ein Model (disposed automatisch ViewModel).
        /// </summary>
        public bool RemoveModel(TModel model)
        {
            if (_dataStore == null)
                throw new InvalidOperationException("DataStore nicht initialisiert.");

            return _dataStore.Remove(model);
        }

        /// <summary>
        /// Entfernt ein ViewModel (entfernt automatisch Model aus DataStore).
        /// </summary>
        public bool RemoveViewModel(TViewModel viewModel)
        {
            if (_dataStore == null)
                throw new InvalidOperationException("DataStore nicht initialisiert.");

            var model = viewModel.Model;
            return _dataStore.Remove(model);
        }

        /// <summary>
        /// Leert die Collection (disposed alle ViewModels).
        /// </summary>
        public void Clear()
        {
            if (_dataStore == null)
                throw new InvalidOperationException("DataStore nicht initialisiert.");

            _dataStore.Clear();
        }

        #region Private: Synchronisation DataStore ? ViewModels

        private void OnDataStoreChanged(object? sender, DataStoreChangedEventArgs<TModel> e)
        {
            if (_modelToViewModelMap == null || _viewModelFactory == null)
                return;

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
                    
                    foreach (var model in _dataStore!.Items)
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
            if (_viewModelFactory == null || _modelToViewModelMap == null)
                throw new InvalidOperationException("Factory nicht initialisiert.");

            var viewModel = _viewModelFactory.Create(model);
            _modelToViewModelMap[model] = viewModel;
            return viewModel;
        }

        private void RemoveAndDisposeViewModel(TModel model, TViewModel viewModel)
        {
            _modelToViewModelMap?.Remove(model);
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

            if (_dataStore != null)
            {
                _dataStore.Changed -= OnDataStoreChanged;
            }

            if (_modelToViewModelMap != null)
            {
                foreach (var viewModel in _modelToViewModelMap.Values)
                {
                    DisposeViewModel(viewModel);
                }
                _modelToViewModelMap.Clear();
            }
            
            _viewModels.Clear();
        }

        #endregion
    }
}