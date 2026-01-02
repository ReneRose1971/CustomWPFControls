using System;
using Common.Bootstrap;
using DataStores.Abstractions;
using DataStores.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using TestHelper.DataStores.PathProviders;

namespace CustomWPFControls.Tests.Testing;

/// <summary>
/// Abstrakte Basisklasse für Test-Fixtures mit DataStores-Bootstrap.
/// </summary>
/// <remarks>
/// Diese Basisklasse implementiert den kompletten 6-Schritte-Bootstrap-Prozess
/// und ruft nach dem Bootstrap zwei Template-Methoden auf:
/// 1. <see cref="InitializeServices"/> - Für Service-Auflösung (Properties setzen)
/// 2. <see cref="InitializeData"/> - Für Test-Daten-Initialisierung
/// 
/// Abgeleitete Klassen müssen diese Methoden implementieren, um ihre spezifischen
/// Services und Testdaten bereitzustellen.
/// </remarks>
public abstract class DataStoresFixtureBase : IDisposable
{
    private bool _disposed;
    private readonly IDataStorePathProvider _pathProvider;

    /// <summary>
    /// Der ServiceProvider mit allen registrierten Dependencies.
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    /// Die IDataStores Facade für Zugriff auf globale/lokale Stores.
    /// </summary>
    public IDataStores DataStores { get; private set; } = null!;

    /// <summary>
    /// Der IEqualityComparerService für Comparer-Auflösung.
    /// </summary>
    public IEqualityComparerService ComparerService { get; private set; } = null!;

    /// <summary>
    /// Der IDataStorePathProvider für isolierte Test-Pfade.
    /// </summary>
    public IDataStorePathProvider PathProvider => _pathProvider;

    /// <summary>
    /// Erstellt die Fixture und führt den kompletten Bootstrap-Prozess aus.
    /// </summary>
    /// <remarks>
    /// Führt folgende Schritte aus:
    /// 1. ServiceCollection erstellen
    /// 2. PathProvider erstellen und registrieren
    /// 3. ConfigureServices() aufrufen (Template-Methode - registriert zusätzliche Services)
    /// 4-5. Bootstrap-Decorator erstellen
    /// 6. RegisterServices() aufrufen (scannt Assemblies)
    /// 7. ServiceProvider bauen + DataStoreBootstrap.RunAsync()
    /// 8. InitializeServices() aufrufen (Template-Methode)
    /// 9. InitializeData() aufrufen (Template-Methode)
    /// </remarks>
    protected DataStoresFixtureBase()
    {
        var services = new ServiceCollection();

        // ✅ Schritt 2: PathProvider erstellen und registrieren (VOR RegisterServices!)
        _pathProvider = new TestDataStorePathProvider("CustomWPFControlsTests");
        services.AddSingleton<IDataStorePathProvider>(_pathProvider);

        // ✅ Schritt 3-4: Bootstrap-Decorator erstellen
        var defaultWrapper = new DefaultBootstrapWrapper();
        var bootstrap = new DataStoresBootstrapDecorator(defaultWrapper);

        // ✅ Schritt 5: RegisterServices aufrufen
        // Scannt nach IServiceModule, IEqualityComparer<T> und IDataStoreRegistrar
        bootstrap.RegisterServices(services, GetAssembliesToScan());

        // ✅ Schritt 6: ServiceProvider bauen
        ServiceProvider = services.BuildServiceProvider();

        // ✅ Schritt 6: DataStores Bootstrap ausführen
        DataStoreBootstrap.RunAsync(ServiceProvider).GetAwaiter().GetResult();

        // Basis-Services auflösen
        DataStores = ServiceProvider.GetRequiredService<IDataStores>();
        ComparerService = ServiceProvider.GetRequiredService<IEqualityComparerService>();

        // ✅ Template-Methode: Abgeleitete Klasse löst ihre Services auf
        InitializeServices();

        // ✅ Template-Methode: Abgeleitete Klasse initialisiert Testdaten
        InitializeData();
    }


    /// <summary>
    /// Gibt die Assemblies zurück, die für Assembly-Scanning verwendet werden sollen.
    /// </summary>
    /// <remarks>
    /// Standard: Common.Bootstrap, DataStores, CustomWPFControls, CustomWPFControls.Tests
    /// Kann überschrieben werden für zusätzliche Assemblies.
    /// </remarks>
    protected virtual System.Reflection.Assembly[] GetAssembliesToScan()
    {
        return new[]
        {
            typeof(DefaultBootstrapWrapper).Assembly,                                   // Common.Bootstrap
            typeof(DataStoresBootstrapDecorator).Assembly,                              // DataStores
            typeof(CustomWPFControls.Bootstrap.CustomWPFControlsServiceModule).Assembly, // CustomWPFControls
            typeof(DataStoresFixtureBase).Assembly                                       // CustomWPFControls.Tests
        };
    }

    /// <summary>
    /// Initialisiert die Services nach dem Bootstrap.
    /// </summary>
    /// <remarks>
    /// Diese Methode wird NACH dem Bootstrap-Prozess aufgerufen.
    /// Abgeleitete Klassen MÜSSEN diese Methode implementieren, um ihre
    /// spezifischen Services aus dem ServiceProvider aufzulösen
    /// (z.B. DataStores, ViewModelFactories, etc.).
    /// 
    /// Beispiel:
    /// <code>
    /// protected override void InitializeServices()
    /// {
    ///     TestModelStore = DataStores.GetGlobal&lt;TestModel&gt;();
    ///     ViewModelFactory = ServiceProvider.GetRequiredService&lt;IViewModelFactory&lt;TestModel, TestViewModel&gt;&gt;();
    /// }
    /// </code>
    /// </remarks>
    protected abstract void InitializeServices();

    /// <summary>
    /// Initialisiert die Testdaten nach der Service-Initialisierung.
    /// </summary>
    /// <remarks>
    /// Diese Methode wird NACH <see cref="InitializeServices"/> aufgerufen.
    /// Abgeleitete Klassen MÜSSEN diese Methode implementieren, um ihre
    /// Testdaten zu initialisieren (z.B. DataStore mit Beispieldaten füllen).
    /// 
    /// Beispiel:
    /// <code>
    /// protected override void InitializeData()
    /// {
    ///     // Optional: Testdaten hinzufügen
    ///     TestModelStore.AddRange(CreateTestModels(5));
    /// }
    /// </code>
    /// 
    /// Hinweis: Kann leer bleiben, wenn keine initiale Befüllung nötig ist.
    /// </remarks>
    protected abstract void InitializeData();

    public virtual void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        // TestDataStorePathProvider cleanup (entfernt Test-Verzeichnisse)
        if (_pathProvider is TestDataStorePathProvider testPathProvider)
        {
            testPathProvider.Cleanup();
        }

        // ServiceProvider disposed (alle Singletons werden disposed)
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
