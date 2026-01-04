using CustomWPFControls.Services.Dialogs;
using CustomWPFControls.Services.MessageBoxes;
using DataStores.Abstractions;

namespace CustomWPFControls.Services
{
    /// <summary>
    /// Service-Facade für alle CustomWPFControls Core-Services.
    /// Kapselt Dialog-, MessageBox- und DataStore-Services.
    /// </summary>
    /// <remarks>
    /// Diese Facade vereinfacht den Zugriff auf häufig benötigte Services
    /// und reduziert Constructor-Parameter in ViewModels.
    /// </remarks>
    public interface ICustomWPFServices
    {
        /// <summary>
        /// Service für modale Dialoge.
        /// </summary>
        IDialogService DialogService { get; }
        
        /// <summary>
        /// Service für MessageBoxes.
        /// </summary>
        IMessageBoxService MessageBoxService { get; }
        
        /// <summary>
        /// DataStores Facade für Model-Store-Zugriff (readonly).
        /// </summary>
        /// <remarks>
        /// Dieser Service stammt aus der DataStores-Library und wird
        /// als readonly-Referenz bereitgestellt.
        /// </remarks>
        IDataStores DataStores { get; }
        
        /// <summary>
        /// Service zur Auflösung von EqualityComparern (readonly).
        /// </summary>
        /// <remarks>
        /// Dieser Service stammt aus der DataStores-Library und wird
        /// zur Erstellung von Comparern für TransformTo benötigt.
        /// </remarks>
        IEqualityComparerService ComparerService { get; }
    }
}