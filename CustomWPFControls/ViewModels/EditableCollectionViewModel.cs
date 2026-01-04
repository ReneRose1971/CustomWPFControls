using System;
using System.Linq;
using System.Windows.Input;
using CustomWPFControls.Commands;
using CustomWPFControls.Services;

namespace CustomWPFControls.ViewModels
{
    /// <summary>
    /// Erweitert CollectionViewModel um Bearbeitungs-Commands (Add, Delete, Clear, Edit).
    /// Unterstützt DataStore-Integration mit automatischer Synchronisation via TransformTo.
    /// </summary>
    /// <typeparam name="TModel">Model-Typ (Domain-Objekt).</typeparam>
    /// <typeparam name="TViewModel">ViewModel-Typ (muss IViewModelWrapper&lt;TModel&gt; implementieren).</typeparam>
    /// <remarks>
    /// <para>
    /// <b>Model-Verwaltung:</b> Models werden über den lokalen ModelStore verwaltet.
    /// Alle Commands (Add, Delete, Clear) arbeiten mit dem lokalen ModelStore.
    /// </para>
    /// <para>
    /// <b>Commands:</b> AddCommand fügt Models zum lokalen ModelStore hinzu.
    /// DeleteCommand nutzt die Remove()-Methode des CollectionViewModel.
    /// </para>
    /// </remarks>
    public class EditableCollectionViewModel<TModel, TViewModel> : CollectionViewModel<TModel, TViewModel>
        where TModel : class
        where TViewModel : class, IViewModelWrapper<TModel>
    {
        private readonly ICustomWPFServices _services;

        /// <summary>
        /// Factory-Funktion zum Erstellen neuer Models.
        /// </summary>
        public Func<TModel>? CreateModel { get; set; }

        /// <summary>
        /// Action zum Bearbeiten eines Models (z.B. Dialog öffnen).
        /// </summary>
        public Action<TModel>? EditModel { get; set; }

        /// <summary>
        /// Erstellt ein EditableCollectionViewModel mit CustomWPFServices Facade.
        /// </summary>
        /// <param name="services">CustomWPFControls Service-Facade.</param>
        /// <param name="viewModelFactory">Factory zur Erstellung von ViewModels.</param>
        /// <exception cref="ArgumentNullException">Wenn einer der Parameter null ist.</exception>
        public EditableCollectionViewModel(
            ICustomWPFServices services,
            Factories.IViewModelFactory<TModel, TViewModel> viewModelFactory)
            : base(services, viewModelFactory)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        #region Commands

        private ICommand? _addCommand;
        /// <summary>
        /// Command zum Hinzufügen eines neuen Elements.
        /// Fügt das Model zum lokalen ModelStore hinzu.
        /// </summary>
        public ICommand AddCommand => _addCommand ??= new RelayCommand(_ =>
        {
            if (CreateModel == null)
                throw new InvalidOperationException("CreateModel muss gesetzt sein.");

            var model = CreateModel();
            if (model != null)
            {
                ModelStore.Add(model);
            }
        }, _ => CreateModel != null);

        private ICommand? _deleteCommand;
        /// <summary>
        /// Command zum Löschen des ausgewählten Elements.
        /// Nutzt die Remove()-Methode des CollectionViewModel.
        /// </summary>
        public ICommand DeleteCommand => _deleteCommand ??= new RelayCommand(_ =>
        {
            if (SelectedItem != null)
            {
                Remove(SelectedItem);
            }
        }, _ => SelectedItem != null);

        private ICommand? _clearCommand;
        /// <summary>
        /// Command zum Löschen aller Elemente.
        /// Nutzt die Clear()-Methode des CollectionViewModel.
        /// </summary>
        public ICommand ClearCommand => _clearCommand ??= new RelayCommand(_ =>
        {
            Clear();
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

        private ICommand? _deleteSelectedCommand;
        /// <summary>
        /// Command zum Löschen aller ausgewählten Elemente (Multi-Selection).
        /// Nutzt die RemoveRange()-Methode des CollectionViewModel.
        /// </summary>
        public ICommand DeleteSelectedCommand => _deleteSelectedCommand ??= new RelayCommand(_ =>
        {
            if (SelectedItems != null && SelectedItems.Count > 0)
            {
                RemoveRange(SelectedItems.ToList());
            }
        }, _ => SelectedItems != null && SelectedItems.Count > 0);

        #endregion
    }
}