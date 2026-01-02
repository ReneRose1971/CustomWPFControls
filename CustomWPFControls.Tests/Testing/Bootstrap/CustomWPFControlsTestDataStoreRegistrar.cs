using DataStores.Bootstrap;
using DataStores.Registration;
using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing;

/// <summary>
/// Registrar für Test-DataStores im CustomWPFControls.Tests Projekt.
/// Registriert einen globalen InMemory-DataStore für TestDto.
/// </summary>
/// <remarks>
/// Dieser Registrar wird automatisch durch <see cref="DataStoresBootstrapDecorator"/> 
/// gefunden und ausgeführt, wenn er über RegisterServices gescannt wird.
/// Benötigt einen parameterlosen Konstruktor für das automatische Scanning.
/// </remarks>
public sealed class CustomWPFControlsTestDataStoreRegistrar : DataStoreRegistrarBase
{
    /// <summary>
    /// Erstellt einen neuen CustomWPFControlsTestDataStoreRegistrar.
    /// </summary>
    /// <remarks>
    /// Parameterloser Konstruktor ist erforderlich für das automatische Assembly-Scanning
    /// durch den DataStoresBootstrapDecorator.
    /// Der InMemory-DataStore verwendet den Default-Comparer von TestDto
    /// (basierend auf Equals/GetHashCode Override).
    /// </remarks>
    public CustomWPFControlsTestDataStoreRegistrar()
    {
    }

    /// <summary>
    /// Konfiguriert den globalen InMemory-DataStore für TestDto.
    /// </summary>
    /// <param name="serviceProvider">Der Service Provider für Dependency Resolution.</param>
    /// <param name="pathProvider">Provider für standardisierte Dateipfade (wird für InMemory nicht benötigt).</param>
    protected override void ConfigureStores(
        IServiceProvider serviceProvider,
        IDataStorePathProvider pathProvider)
    {
        // InMemory-Store für TestDto registrieren
        // Verwendet Default-Comparer (TestDto.Equals/GetHashCode)
        AddStore(new InMemoryDataStoreBuilder<TestDto>());
    }
}
