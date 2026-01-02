using System;
using System.Windows.Input;
using CustomWPFControls.Commands;
using DataStores.Abstractions;

namespace CustomWPFControls.ViewModels
{
    /// <summary>
    /// Erweitert CollectionViewModel um Bearbeitungs-Commands (Add, Delete, Clear, Edit).
    /// Unterstützt DataStore-Integration mit bidirektionaler Synchronisation via TransformTo.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    /// <remarks>
    /// <para>
    /// <b>Model-Verwaltung:</b> Models werden direkt über den Model-Store verwaltet:
    /// <c>dataStores.GetGlobal&lt;TModel&gt;().Add(model)</c>
    /// </para>
    /// <para>
    /// <b>Commands:</b> AddCommand und ClearCommand arbeiten mit dem Model-Store.
    /// DeleteCommand arbeitet mit dem aktuell ausgewählten ViewModel.
    /// </para>
    /// </remarks>
    public class EditableCollectionViewModel<TModel, TViewModel> : CollectionViewModel<TModel, TViewModel>
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly IDataStores _dataStores;

        /// <summary>
        /// Factory-Funktion zum Erstellen neuer Models.
        /// </summary>
        public Func<TModel>? CreateModel { get; set; }

        /// <summary>
        /// Action zum Bearbeiten eines Models (z.B. Dialog öffnen).
        /// </summary>
        public Action<TModel>? EditModel { get; set; }

        /// <summary>
        /// Erstellt ein EditableCollectionViewModel mit DataStore-Integration.
        /// </summary>
        public EditableCollectionViewModel(
            IDataStores dataStores,
            Factories.IViewModelFactory<TModel, TViewModel> viewModelFactory,
            IEqualityComparerService comparerService)
            : base(dataStores, viewModelFactory, comparerService)
        {
            _dataStores = dataStores ?? throw new ArgumentNullException(nameof(dataStores));
        }

        #region Commands

        private ICommand? _addCommand;
        /// <summary>
        /// Command zum Hinzufügen eines neuen Elements.
        /// </summary>
        public ICommand AddCommand => _addCommand ??= new RelayCommand(_ =>
        {
            if (CreateModel == null)
                throw new InvalidOperationException("CreateModel muss gesetzt sein.");

            var model = CreateModel();
            if (model != null)
            {
                var modelStore = _dataStores.GetGlobal<TModel>();
                modelStore.Add(model);
            }
        }, _ => CreateModel != null);

        private ICommand? _deleteCommand;
        /// <summary>
        /// Command zum Löschen des ausgewählten Elements.
        /// </summary>
        public ICommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ =>
        {
            if (SelectedItem != null)
            {
                var modelStore = _dataStores.GetGlobal<TModel>();
                modelStore.Remove(SelectedItem.Model);
            }
        }, _ => SelectedItem != null);

        private ICommand? _clearCommand;
        /// <summary>
        /// Command zum Löschen aller Elemente.
        /// </summary>
        public ICommand ClearCommand => _clearCommand ??= new RelayCommand(_ =>
        {
            var modelStore = _dataStores.GetGlobal<TModel>();
            modelStore.Clear();
        }, _ => Count > 0);

        private ICommand? _editCommand;
        /// <summary>
        /// Command zum Bearbeiten des ausgewählten Elements.
        /// </summary>
        public ICommand EditCommand => _editCommand ??= new RelayCommand(_ =>
        {
            if (SelectedItem != null && EditModel != null)
            {
                EditModel(SelectedItem.Model);
            }
        }, _ => SelectedItem != null && EditModel != null);

        #endregion
    }
}