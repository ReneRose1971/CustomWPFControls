using System;
using CustomWPFControls.Services.Dialogs;
using CustomWPFControls.Services.MessageBoxes;
using DataStores.Abstractions;

namespace CustomWPFControls.Services
{
    /// <summary>
    /// Implementierung der CustomWPFControls Service-Facade.
    /// </summary>
    public class CustomWPFServices : ICustomWPFServices
    {
        /// <summary>
        /// Erstellt eine neue Instanz der CustomWPFServices Facade.
        /// </summary>
        /// <param name="dialogService">Dialog-Service für modale Fenster.</param>
        /// <param name="messageBoxService">MessageBox-Service.</param>
        /// <param name="dataStores">DataStores Facade (aus DataStores-Library).</param>
        /// <param name="comparerService">EqualityComparer-Service (aus DataStores-Library).</param>
        /// <exception cref="ArgumentNullException">Wenn einer der Parameter null ist.</exception>
        public CustomWPFServices(
            IDialogService dialogService,
            IMessageBoxService messageBoxService,
            IDataStores dataStores,
            IEqualityComparerService comparerService)
        {
            DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            MessageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            DataStores = dataStores ?? throw new ArgumentNullException(nameof(dataStores));
            ComparerService = comparerService ?? throw new ArgumentNullException(nameof(comparerService));
        }
        
        /// <inheritdoc/>
        public IDialogService DialogService { get; }
        
        /// <inheritdoc/>
        public IMessageBoxService MessageBoxService { get; }
        
        /// <inheritdoc/>
        public IDataStores DataStores { get; }
        
        /// <inheritdoc/>
        public IEqualityComparerService ComparerService { get; }
    }
}