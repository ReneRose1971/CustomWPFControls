# CustomWPFControls.TestHelpers

Test-Utilities und Fixtures für CustomWPFControls-Tests mit DataStore-Integration.

## Überblick

Dieses Projekt stellt wiederverwendbare Test-Infrastruktur für Unit-, Integration- und Behavior-Tests bereit:

- **Bootstrap-Fixtures** - DataStores-Bootstrap für Tests
- **Test-Models** - Beispiel-Models und ViewModels
- **Helper-Methoden** - Utilities für Testdaten-Erstellung

## Hauptkomponenten

### DataStoresFixtureBase

Abstrakte Basisklasse für Test-Fixtures mit vollständigem DataStores-Bootstrap.

```csharp
public abstract class DataStoresFixtureBase : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public IDataStores DataStores { get; }
    public IEqualityComparerService ComparerService { get; }
    public IDataStorePathProvider PathProvider { get; }
    
    protected abstract void InitializeServices();
    protected abstract void InitializeData();
}
```

**Features:**
- Vollständiger 6-Schritte-Bootstrap-Prozess
- Automatische Assembly-Scanning
- Template-Methoden für Service-Auflösung
- Cleanup bei Dispose

**Verwendung:**
```csharp
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    public IDataStore<TestDto> TestDtoStore { get; private set; }
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; private set; }
    
    protected override void InitializeServices()
    {
        TestDtoStore = DataStores.GetGlobal<TestDto>();
        ViewModelFactory = ServiceProvider.GetRequiredService<
            IViewModelFactory<TestDto, TestViewModel>>();
    }
    
    protected override void InitializeData()
    {
        // Optional: Testdaten hinzufügen
    }
}
```

### CollectionViewModelFixture

Spezialisierte Fixture für CollectionViewModel-Tests.

```csharp
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    public IDataStore<TestDto> TestDtoStore { get; }
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; }
    
    public void ClearTestData();
}
```

**Bereitgestellte Services:**
- TestDto DataStore (via CustomWPFControlsTestDataStoreRegistrar)
- ViewModelFactory für TestDto/TestViewModel
- IDataStores Facade
- IEqualityComparerService

**Verwendung in Tests:**
```csharp
public class MyTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Test_WithDataStore()
    {
        // Arrange
        var viewModel = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
        
        // Act
        _fixture.TestDtoStore.Add(new TestDto { Id = 1, Name = "Test" });
        
        // Assert
        Assert.Equal(1, viewModel.Count);
    }
}
```

## Test-Models

### TestDto

```csharp
public class TestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
```

### TestViewModel

```csharp
public class TestViewModel : ViewModelBase<TestDto>
{
    public TestViewModel(TestDto model) : base(model) { }
    
    public int Id => Model.Id;
    public string Name => Model.Name;
    public bool IsSelected { get; set; }
}
```

### TestDtoComparer

```csharp
public class TestDtoComparer : IEqualityComparer<TestDto>
{
    public bool Equals(TestDto? x, TestDto? y) => x?.Id == y?.Id;
    public int GetHashCode(TestDto obj) => obj.Id.GetHashCode();
}
```

## Service-Module

### CustomWPFControlsTestServiceModule

Registriert Test-spezifische Services:
- ViewModelFactory für TestDto/TestViewModel
- IEqualityComparer für TestDto

```csharp
public class CustomWPFControlsTestServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        services.AddViewModelFactory<TestDto, TestViewModel>();
        services.AddSingleton<IEqualityComparer<TestDto>>(
            new TestDtoComparer());
    }
}
```

### CustomWPFControlsTestDataStoreRegistrar

Registriert globalen TestDto-DataStore:

```csharp
public class CustomWPFControlsTestDataStoreRegistrar : IDataStoreRegistrar
{
    public void RegisterDataStores(IServiceProvider serviceProvider, IDataStores dataStores)
    {
        var comparer = serviceProvider.GetRequiredService<IEqualityComparer<TestDto>>();
        dataStores.RegisterGlobal<TestDto>(comparer);
    }
}
```

## PathProvider

### TestDataStorePathProvider

Stellt isolierte Test-Pfade bereit und bereinigt nach Tests.

```csharp
public class TestDataStorePathProvider : IDataStorePathProvider
{
    public TestDataStorePathProvider(string testSuiteName);
    
    public string GetPath(string filename);
    public void Cleanup();
}
```

**Features:**
- Eindeutige Pfade pro Test-Suite
- Automatische Cleanup bei Dispose
- Vermeidet Konflikte zwischen Tests

## Verwendung in Tests

### Unit-Test

```csharp
public class ViewModelTest
{
    [Fact]
    public void Constructor_WithModel_SetsProperty()
    {
        // Arrange
        var dto = new TestDto { Id = 1, Name = "Test" };
        
        // Act
        var viewModel = new TestViewModel(dto);
        
        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Test", viewModel.Name);
    }
}
```

### Integration-Test mit Fixture

```csharp
public class CollectionViewModelIntegrationTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public CollectionViewModelIntegrationTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData(); // Clean slate
    }
    
    [Fact]
    public void DataStore_Add_SyncsToViewModel()
    {
        // Arrange
        var viewModel = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
        
        // Act
        _fixture.TestDtoStore.Add(new TestDto { Id = 1, Name = "Alice" });
        
        // Assert
        Assert.Equal(1, viewModel.Items.Count);
        Assert.Equal("Alice", viewModel.Items[0].Name);
    }
}
```

### Behavior-Test

```csharp
public class EditableCollectionViewModelBehaviorTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public EditableCollectionViewModelBehaviorTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void AddCommand_CreatesAndAddsModel()
    {
        // Arrange
        var viewModel = new EditableCollectionViewModel<TestDto, TestViewModel>(
            _fixture.DataStores,
            _fixture.ViewModelFactory,
            _fixture.ComparerService);
        
        viewModel.CreateModel = () => new TestDto { Id = 1, Name = "New" };
        
        // Act
        viewModel.AddCommand.Execute(null);
        
        // Assert
        Assert.Equal(1, viewModel.Count);
        Assert.Equal("New", viewModel.Items[0].Name);
    }
}
```

## Best Practices

### Fixture-Wiederverwendung

```csharp
// Fixture einmal pro Test-Klasse (IClassFixture)
public class MyTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Test1() { /* ... */ }
    
    [Fact]
    public void Test2() { /* ... */ }
}
```

### Test-Isolation

```csharp
public MyTest(CollectionViewModelFixture fixture)
{
    _fixture = fixture;
    _fixture.ClearTestData(); // Sauberer Zustand
}
```

### Custom Fixtures

```csharp
public class MyCustomFixture : DataStoresFixtureBase
{
    public IDataStore<MyModel> MyModelStore { get; private set; }
    
    protected override void InitializeServices()
    {
        MyModelStore = DataStores.GetGlobal<MyModel>();
    }
    
    protected override void InitializeData()
    {
        // Initial-Daten
        MyModelStore.Add(new MyModel { Id = 1 });
    }
}
```

## Siehe auch

- [CustomWPFControls README](../CustomWPFControls/README.md)
- [Test Strategy](../CustomWPFControls.Tests/TestStrategy.md)
- [Getting Started](../CustomWPFControls/Docs/Getting-Started.md)
