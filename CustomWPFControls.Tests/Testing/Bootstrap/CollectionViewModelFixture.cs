using CustomWPFControls.Factories;
using DataStores.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.Models;

namespace CustomWPFControls.Tests.Testing;

/// <summary>
/// Test-Fixture für CollectionViewModel-Tests mit DataStores-Bootstrap.
/// </summary>
/// <remarks>
/// Diese Fixture erbt von <see cref="DataStoresFixtureBase"/> und stellt
/// Services für CollectionViewModel-Tests bereit:
/// - TestDto DataStore (via CustomWPFControlsTestDataStoreRegistrar)
/// - ViewModelFactory für TestDto/TestViewModel (via CustomWPFControlsTestServiceModule)
/// - IDataStores Facade
/// - IEqualityComparerService
/// </remarks>
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    /// <summary>
    /// Der globale TestDto DataStore.
    /// </summary>
    public IDataStore<TestDto> TestDtoStore { get; protected set; } = null!;

    /// <summary>
    /// ViewModelFactory für TestDto → TestViewModel.
    /// </summary>
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; protected set; } = null!;

    /// <summary>
    /// Erstellt die Fixture und führt den kompletten Bootstrap-Prozess aus.
    /// </summary>
    public CollectionViewModelFixture()
    {
        // Basisklasse führt Bootstrap-Prozess aus und ruft dann
        // InitializeServices() und InitializeData() auf
    }

    /// <summary>
    /// Initialisiert die Services nach dem Bootstrap.
    /// </summary>
    /// <remarks>
    /// Löst TestDtoStore und ViewModelFactory aus dem ServiceProvider auf.
    /// ViewModelFactory wurde automatisch via CustomWPFControlsTestServiceModule registriert.
    /// </remarks>
    protected override void InitializeServices()
    {
        // TestDto DataStore auflösen (wurde via CustomWPFControlsTestDataStoreRegistrar registriert)
        TestDtoStore = DataStores.GetGlobal<TestDto>();

        // ViewModelFactory auflösen (wurde via CustomWPFControlsTestServiceModule registriert)
        ViewModelFactory = ServiceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();
    }

    /// <summary>
    /// Initialisiert die Testdaten nach der Service-Initialisierung.
    /// </summary>
    /// <remarks>
    /// Für CollectionViewModel-Tests bleibt der Store initial leer.
    /// Tests fügen ihre eigenen Daten hinzu.
    /// Kann in abgeleiteten Klassen überschrieben werden.
    /// </remarks>
    protected override void InitializeData()
    {
        // Store bleibt leer - Tests fügen ihre eigenen Daten hinzu
    }

    /// <summary>
    /// Räumt den TestDtoStore auf (entfernt alle Items).
    /// </summary>
    /// <remarks>
    /// Sollte nach jedem Test aufgerufen werden, um saubere Test-Isolation zu gewährleisten.
    /// </remarks>
    public void ClearTestData()
    {
        TestDtoStore.Clear();
    }
}
