using CustomWPFControls.Services;
using DataStores.Bootstrap;
using DataStores.Registration;
using System.Text.Json;

namespace CustomWPFControls.Bootstrap;

/// <summary>
/// Registrar für den WindowLayoutData DataStore.
/// Registriert einen globalen JSON-persistenten DataStore für WindowLayoutData.
/// </summary>
/// <remarks>
/// Dieser Registrar wird automatisch durch <see cref="DataStoresBootstrapDecorator"/> 
/// gefunden und ausgeführt, wenn er über RegisterServices gescannt wird.
/// Benötigt einen parameterlosen Konstruktor für das automatische Scanning.
/// </remarks>
public sealed class WindowLayoutDataStoreRegistrar : DataStoreRegistrarBase
{
    /// <summary>
    /// Erstellt einen neuen WindowLayoutDataStoreRegistrar.
    /// </summary>
    /// <remarks>
    /// Parameterloser Konstruktor ist erforderlich für das automatische Assembly-Scanning
    /// durch den DataStoresBootstrapDecorator.
    /// Der DataStore wird über den IDataStorePathProvider konfiguriert und in einer
    /// windowlayouts.json Datei persistiert.
    /// </remarks>
    public WindowLayoutDataStoreRegistrar()
    {
    }

    /// <summary>
    /// Konfiguriert den globalen DataStore für WindowLayoutData.
    /// </summary>
    /// <param name="serviceProvider">Der Service Provider für Dependency Resolution.</param>
    /// <param name="pathProvider">Provider für standardisierte Dateipfade.</param>
    protected override void ConfigureStores(
        IServiceProvider serviceProvider, 
        IDataStorePathProvider pathProvider)
    {
        // JSON-Pfad über PathProvider generieren
        var jsonPath = pathProvider.FormatJsonFileName("windowlayouts");

        // JSON-Serialisierungsoptionen
        var jsonOptions = new JsonSerializerOptions 
        { 
            WriteIndented = true 
        };

        // Store mit Builder registrieren
        AddStore(new JsonDataStoreBuilder<WindowLayoutData>(
            filePath: jsonPath,
             autoLoad: true,
            autoSave: true));
    }
}
