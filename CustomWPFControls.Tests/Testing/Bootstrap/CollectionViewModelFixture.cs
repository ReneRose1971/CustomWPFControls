using CustomWPFControls.Factories;
using CustomWPFControls.Services;
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
/// - ICustomWPFServices Facade (mit DataStores, ComparerService, DialogService, MessageBoxService)
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
    /// CustomWPFServices Facade mit allen Core-Services.
    /// </summary>
    /// <remarks>
    /// Kapselt: DataStores, ComparerService, DialogService, MessageBoxService
    /// </remarks>
    public ICustomWPFServices Services { get; protected set; } = null!;

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
    /// Löst TestDtoStore, ViewModelFactory und Services aus dem ServiceProvider auf.
    /// ViewModelFactory und Services wurden automatisch via CustomWPFControlsTestServiceModule registriert.
    /// </remarks>
    protected override void InitializeServices()
    {
        // TestDto DataStore auflösen (wurde via CustomWPFControlsTestDataStoreRegistrar registriert)
        TestDtoStore = DataStores.GetGlobal<TestDto>();

        // ViewModelFactory auflösen (wurde via CustomWPFControlsTestServiceModule registriert)
        ViewModelFactory = ServiceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();

        // CustomWPFServices Facade auflösen (wurde via CustomWPFControlsServiceModule registriert)
        Services = ServiceProvider.GetRequiredService<ICustomWPFServices>();
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
