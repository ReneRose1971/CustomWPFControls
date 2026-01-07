# CustomWPFControls - Testing Guide

Umfassender Leitfaden für das Testen von CustomWPFControls-basierten Anwendungen und ViewModels.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [SynchronizationContext für WPF-Tests](#synchronizationcontext-für-wpf-tests)
- [Test-Fixtures mit DataStoresFixtureBase](#test-fixtures-mit-datastoresffixturebase)
- [Unit-Tests für ViewModels](#unit-tests-für-viewmodels)
- [Integration-Tests für DataStore-Synchronisation](#integration-tests-für-datastore-synchronisation)
- [Behavior-Tests für Commands](#behavior-tests-für-commands)
- [Test-Isolation und Cleanup](#test-isolation-und-cleanup)
- [Praktische Beispiele](#praktische-beispiele)

---

## Überblick

Das Testen von CustomWPFControls-basierten Anwendungen erfordert besondere Konfigurationen:

1. **SynchronizationContext**: Für ObservableCollection-Updates in Tests
2. **DataStores-Bootstrap**: Vollständiger DI-Container für Integration-Tests
3. **Test-Fixtures**: Wiederverwendbare Test-Infrastruktur
4. **Test-Isolation**: Saubere Trennung zwischen Tests

---

## SynchronizationContext für WPF-Tests

### Warum wird ein SynchronizationContext benötigt?

WPF-Controls wie `ObservableCollection` erwarten, dass Property-Updates auf dem UI-Thread erfolgen. In Unit-Tests gibt es standardmäßig keinen UI-Thread, was zu Fehlern führen kann:

```
System.NotSupportedException: This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread.
```

### Lösung: SynchronizationContext einrichten

Für Tests, die mit ObservableCollections arbeiten, muss ein `SynchronizationContext` konfiguriert werden.

#### Option 1: Pro Test-Methode

```csharp
using System.Threading;
using Xunit;

public class CollectionViewModelTest
{
    [Fact]
    public void Test_WithSynchronizationContext()
    {
        // Arrange: SynchronizationContext einrichten
        SynchronizationContext.SetSynchronizationContext(
            new SynchronizationContext());
        
        var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        // Act
        viewModel.ModelStore.Add(new Customer { Name = "Test" });
        
        // Assert
        Assert.Equal(1, viewModel.Items.Count);
    }
}
```

#### Option 2: Test-Fixture (empfohlen)

```csharp
using System.Threading;
using Xunit;

public class CollectionViewModelFixture : IDisposable
{
    public CollectionViewModelFixture()
    {
        // SynchronizationContext für alle Tests in dieser Fixture
        SynchronizationContext.SetSynchronizationContext(
            new SynchronizationContext());
        
        // Weitere Initialisierung...
    }
    
    public void Dispose()
    {
        // Cleanup
    }
}

public class MyTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Test_UsesFixtureSynchronizationContext()
    {
        // SynchronizationContext ist bereits konfiguriert
        var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
            _fixture.Services, _fixture.ViewModelFactory);
        
        viewModel.ModelStore.Add(new Customer());
        Assert.Equal(1, viewModel.Items.Count);
    }
}
```

### Wann wird SynchronizationContext benötigt?

| Test-Typ | SynchronizationContext erforderlich? |
|----------|--------------------------------------|
| Unit-Tests für ViewModelBase | ? Nein |
| Unit-Tests für CollectionViewModel (ohne Items-Zugriff) | ? Nein |
| Unit-Tests mit Items-Collection | ? Ja |
| Integration-Tests mit DataStore-Sync | ? Ja |
| Behavior-Tests mit Commands | ? Ja (wenn Items geprüft werden) |

---

## Test-Fixtures mit DataStoresFixtureBase

### Was ist DataStoresFixtureBase?

`DataStoresFixtureBase` ist eine abstrakte Basisklasse für Test-Fixtures, die den kompletten DataStores-Bootstrap-Prozess ausführt.

### Features

- ? Vollständiger 6-Schritte-Bootstrap
- ? Automatisches Assembly-Scanning
- ? Isolierte Test-Pfade (TestDataStorePathProvider)
- ? Template-Methoden für Service-Auflösung
- ? Automatische Cleanup bei Dispose

### Verwendung

```csharp
using CustomWPFControls.Tests.Testing;
using DataStores.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class MyTestFixture : DataStoresFixtureBase
{
    public IDataStore<Customer> CustomerStore { get; private set; } = null!;
    public IViewModelFactory<Customer, CustomerViewModel> ViewModelFactory { get; private set; } = null!;
    public ICustomWPFServices Services { get; private set; } = null!;
    
    protected override void InitializeServices()
    {
        // Services aus ServiceProvider auflösen
        CustomerStore = DataStores.GetGlobal<Customer>();
        ViewModelFactory = ServiceProvider.GetRequiredService<
            IViewModelFactory<Customer, CustomerViewModel>>();
        Services = ServiceProvider.GetRequiredService<ICustomWPFServices>();
    }
    
    protected override void InitializeData()
    {
        // Optional: Initiale Testdaten
        CustomerStore.Add(new Customer { Id = 1, Name = "Test" });
    }
}
```

### Vorgefertigte Fixture: CollectionViewModelFixture

Das CustomWPFControls-Projekt stellt eine vorgefertigte Fixture bereit:

```csharp
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; }
    public ICustomWPFServices Services { get; }
    public CollectionViewModel<TestDto, TestViewModel> Sut { get; }
    public IDataStore<TestDto> TestDtoStore => Sut.ModelStore;
    
    // Helper-Methoden
    public TestDto[] AddTestData(params string[] names);
    public TestDto AddSingleItem(string name = "TestItem");
    public void ClearTestData();
    public void ResetSut();
}
```

### Verwendung in Tests

```csharp
public class MyTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData(); // Sauberer Zustand
    }
    
    [Fact]
    public void Test_WithFixture()
    {
        // Arrange: Fixture stellt alles bereit
        var dto = _fixture.AddSingleItem("Test");
        
        // Act & Assert
        Assert.Equal(1, _fixture.Sut.Items.Count);
        Assert.Equal("Test", _fixture.Sut.Items[0].Name);
    }
}
```

---

## Unit-Tests für ViewModels

### ViewModelBase-Tests

```csharp
public class CustomerViewModelTests
{
    [Fact]
    public void Constructor_WithModel_SetsProperties()
    {
        // Arrange
        var customer = new Customer 
        { 
            Id = 1, 
            Name = "Alice", 
            Email = "alice@example.com" 
        };
        
        // Act
        var viewModel = new CustomerViewModel(customer);
        
        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("Alice", viewModel.Name);
        Assert.Equal("alice@example.com", viewModel.Email);
    }
    
    [Fact]
    public void PropertyChanged_IsFiredForUIProperties()
    {
        // Arrange
        var customer = new Customer { Id = 1 };
        var viewModel = new CustomerViewModel(customer);
        
        bool propertyChangedFired = false;
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.IsSelected))
                propertyChangedFired = true;
        };
        
        // Act
        viewModel.IsSelected = true;
        
        // Assert
        Assert.True(propertyChangedFired);
    }
}
```

### CollectionViewModel-Tests

```csharp
public class CollectionViewModelTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public CollectionViewModelTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void Constructor_CreatesEmptyCollection()
    {
        // Arrange & Act
        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Assert
        Assert.NotNull(sut.Items);
        Assert.Empty(sut.Items);
        Assert.Equal(0, sut.Count);
        
        // Cleanup
        sut.Dispose();
    }
    
    [Fact]
    public void ModelStore_Add_IncreasesCount()
    {
        // Arrange
        var sut = _fixture.Sut;
        
        // Act
        sut.ModelStore.Add(new TestDto { Name = "Test" });
        
        // Assert
        Assert.Equal(1, sut.Count);
        
        // Cleanup
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void Remove_RemovesItemAndInvalidatesSelection()
    {
        // Arrange
        var dto = _fixture.AddSingleItem("Test");
        var viewModel = _fixture.Sut.Items[0];
        _fixture.Sut.SelectedItem = viewModel;
        
        // Act
        _fixture.Sut.Remove(viewModel);
        
        // Assert
        Assert.Equal(0, _fixture.Sut.Count);
        Assert.Null(_fixture.Sut.SelectedItem);
        
        // Cleanup
        _fixture.ClearTestData();
    }
}
```

---

## Integration-Tests für DataStore-Synchronisation

### TransformTo-Synchronisation

```csharp
public class DataStoreSyncTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public DataStoreSyncTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void ModelStore_Add_CreatesViewModel()
    {
        // Arrange
        var sut = _fixture.Sut;
        
        // Act
        sut.ModelStore.Add(new TestDto { Id = 1, Name = "Alice" });
        
        // Assert
        Assert.Equal(1, sut.Items.Count);
        Assert.Equal("Alice", sut.Items[0].Name);
        
        // Cleanup
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void ModelStore_Remove_RemovesViewModel()
    {
        // Arrange
        var dto = new TestDto { Id = 1, Name = "Bob" };
        _fixture.Sut.ModelStore.Add(dto);
        
        // Act
        _fixture.Sut.ModelStore.Remove(dto);
        
        // Assert
        Assert.Empty(_fixture.Sut.Items);
        
        // Cleanup
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void ModelStore_AddRange_CreatesMultipleViewModels()
    {
        // Arrange
        var dtos = new[]
        {
            new TestDto { Id = 1, Name = "Alice" },
            new TestDto { Id = 2, Name = "Bob" },
            new TestDto { Id = 3, Name = "Charlie" }
        };
        
        // Act
        _fixture.Sut.ModelStore.AddRange(dtos);
        
        // Assert
        Assert.Equal(3, _fixture.Sut.Items.Count);
        Assert.Equal("Alice", _fixture.Sut.Items[0].Name);
        Assert.Equal("Bob", _fixture.Sut.Items[1].Name);
        Assert.Equal("Charlie", _fixture.Sut.Items[2].Name);
        
        // Cleanup
        _fixture.ClearTestData();
    }
}
```

### LoadData Extension-Tests

```csharp
public class LoadDataTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public LoadDataTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void LoadData_WithValidData_LoadsAndSelectsFirst()
    {
        // Arrange
        var dtos = new[]
        {
            new TestDto { Id = 1, Name = "First" },
            new TestDto { Id = 2, Name = "Second" }
        };
        
        // Act
        _fixture.Sut.LoadData(dtos);
        
        // Assert
        Assert.Equal(2, _fixture.Sut.Count);
        Assert.NotNull(_fixture.Sut.SelectedItem);
        Assert.Equal("First", _fixture.Sut.SelectedItem.Name);
        
        // Cleanup
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void LoadData_WithSelectFirstFalse_DoesNotSelectItem()
    {
        // Arrange
        var dtos = new[] { new TestDto { Id = 1, Name = "Test" } };
        
        // Act
        _fixture.Sut.LoadData(dtos, selectFirst: false);
        
        // Assert
        Assert.Equal(1, _fixture.Sut.Count);
        Assert.Null(_fixture.Sut.SelectedItem);
        
        // Cleanup
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void LoadData_WithExistingData_ReplacesCompletelyWithNewData()
    {
        // Arrange
        _fixture.AddTestData("Old1", "Old2");
        var newDtos = new[]
        {
            new TestDto { Id = 3, Name = "New1" },
            new TestDto { Id = 4, Name = "New2" }
        };
        
        // Act
        _fixture.Sut.LoadData(newDtos);
        
        // Assert
        Assert.Equal(2, _fixture.Sut.Count);
        Assert.Equal("New1", _fixture.Sut.Items[0].Name);
        Assert.Equal("New2", _fixture.Sut.Items[1].Name);
        
        // Cleanup
        _fixture.ClearTestData();
    }
}
```

---

## Behavior-Tests für Commands

### EditableCollectionViewModel-Commands

```csharp
public class AddCommandTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    
    public AddCommandTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = _fixture.CreateEditableCollectionViewModel();
        
        // CreateModel konfigurieren
        _sut.CreateModel = () => new TestDto { Name = "NewItem" };
    }
    
    [Fact]
    public void AddCommand_CanExecute_ReturnsTrueWhenCreateModelIsSet()
    {
        // Act & Assert
        Assert.True(_sut.AddCommand.CanExecute(null));
    }
    
    [Fact]
    public void AddCommand_Execute_AddsModelToStore()
    {
        // Act
        _sut.AddCommand.Execute(null);
        
        // Assert
        Assert.Equal(1, _sut.Count);
        Assert.Equal("NewItem", _sut.Items[0].Name);
    }
    
    public void Dispose()
    {
        _sut?.Dispose();
    }
}

public class DeleteCommandTests : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly EditableCollectionViewModel<TestDto, TestViewModel> _sut;
    
    public DeleteCommandTests(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _sut = _fixture.CreateEditableCollectionViewModel();
    }
    
    [Fact]
    public void DeleteCommand_CanExecute_ReturnsFalseWhenNoSelection()
    {
        // Act & Assert
        Assert.False(_sut.DeleteCommand.CanExecute(null));
    }
    
    [Fact]
    public void DeleteCommand_Execute_RemovesSelectedItem()
    {
        // Arrange
        _sut.ModelStore.Add(new TestDto { Name = "Test" });
        _sut.SelectedItem = _sut.Items[0];
        
        // Act
        _sut.DeleteCommand.Execute(null);
        
        // Assert
        Assert.Equal(0, _sut.Count);
        Assert.Null(_sut.SelectedItem);
    }
    
    public void Dispose()
    {
        _sut?.Dispose();
    }
}
```

---

## Test-Isolation und Cleanup

### Per-Test Cleanup

```csharp
public class MyTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        // ? Cleanup im Constructor für sauberen Zustand
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void Test1()
    {
        // Test-Logik
        _fixture.AddTestData("Item1");
        Assert.Equal(1, _fixture.Sut.Count);
        
        // ? Optional: Cleanup nach Test
        _fixture.ClearTestData();
    }
}
```

### IDisposable-Pattern für SUT

```csharp
public class MyTest : IClassFixture<CollectionViewModelFixture>, IDisposable
{
    private readonly CollectionViewModelFixture _fixture;
    private readonly CollectionViewModel<TestDto, TestViewModel> _sut;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        // Eigene SUT-Instanz erstellen
        _sut = _fixture.CreateCollectionViewModel();
    }
    
    [Fact]
    public void Test_WithOwnSut()
    {
        _sut.ModelStore.Add(new TestDto { Name = "Test" });
        Assert.Equal(1, _sut.Count);
    }
    
    public void Dispose()
    {
        // ? SUT disposed
        _sut?.Dispose();
    }
}
```

### Fixture-Reset

```csharp
public class MyTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public MyTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Test_RequiresFreshSut()
    {
        // ? Komplett neue SUT-Instanz
        _fixture.ResetSut();
        
        Assert.Equal(0, _fixture.Sut.Count);
    }
}
```

---

## Praktische Beispiele

### Beispiel 1: Vollständiger Unit-Test

```csharp
public class CustomerViewModelFullTest
{
    [Fact]
    public void FullScenario_CreateEditDelete()
    {
        // Arrange: SynchronizationContext
        SynchronizationContext.SetSynchronizationContext(
            new SynchronizationContext());
        
        var services = CreateTestServices();
        var factory = CreateTestFactory();
        
        var sut = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        sut.CreateModel = () => new Customer { Name = "New" };
        
        // Act & Assert: Add
        sut.AddCommand.Execute(null);
        Assert.Equal(1, sut.Count);
        
        // Act & Assert: Edit
        var customer = sut.Items[0].Model;
        customer.Name = "Updated";
        Assert.Equal("Updated", sut.Items[0].Name);
        
        // Act & Assert: Delete
        sut.SelectedItem = sut.Items[0];
        sut.DeleteCommand.Execute(null);
        Assert.Equal(0, sut.Count);
        
        // Cleanup
        sut.Dispose();
    }
}
```

### Beispiel 2: Integration-Test mit Fixture

```csharp
public class CustomerIntegrationTest : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public CustomerIntegrationTest(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
        _fixture.ClearTestData();
    }
    
    [Fact]
    public void Integration_AddMultipleCustomers_SyncsCorrectly()
    {
        // Arrange
        var customers = new[]
        {
            new TestDto { Id = 1, Name = "Alice" },
            new TestDto { Id = 2, Name = "Bob" },
            new TestDto { Id = 3, Name = "Charlie" }
        };
        
        // Act
        _fixture.Sut.LoadData(customers);
        
        // Assert: Count
        Assert.Equal(3, _fixture.Sut.Count);
        
        // Assert: Selection
        Assert.NotNull(_fixture.Sut.SelectedItem);
        Assert.Equal("Alice", _fixture.Sut.SelectedItem.Name);
        
        // Assert: Items
        Assert.Equal("Alice", _fixture.Sut.Items[0].Name);
        Assert.Equal("Bob", _fixture.Sut.Items[1].Name);
        Assert.Equal("Charlie", _fixture.Sut.Items[2].Name);
        
        // Cleanup
        _fixture.ClearTestData();
    }
}
```

### Beispiel 3: Behavior-Test mit Mock

```csharp
public class CommandBehaviorTest
{
    [Fact]
    public void AddCommand_WithValidation_RejectsInvalidData()
    {
        // Arrange
        SynchronizationContext.SetSynchronizationContext(
            new SynchronizationContext());
        
        var services = CreateTestServices();
        var factory = CreateTestFactory();
        var validator = new Mock<IValidator>();
        
        var sut = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        sut.CreateModel = () =>
        {
            var customer = new Customer { Name = "" }; // Invalid
            
            var errors = validator.Object.Validate(customer);
            if (errors.Any())
                return null; // Rejection
            
            return customer;
        };
        
        // Mock: Validation fails
        validator.Setup(v => v.Validate(It.IsAny<Customer>()))
            .Returns(new[] { "Name is required" });
        
        // Act
        sut.AddCommand.Execute(null);
        
        // Assert: Nothing added
        Assert.Equal(0, sut.Count);
        
        // Cleanup
        sut.Dispose();
    }
}
```

---

## Zusammenfassung

### Checkliste für Tests

#### Unit-Tests
- ? SynchronizationContext setzen (wenn Items verwendet werden)
- ? ViewModelBase-Properties testen
- ? PropertyChanged-Events prüfen
- ? Dispose aufrufen

#### Integration-Tests
- ? DataStoresFixtureBase verwenden
- ? SynchronizationContext in Fixture
- ? ClearTestData() im Constructor
- ? TransformTo-Synchronisation testen
- ? LoadData Extension testen

#### Behavior-Tests
- ? EditableCollectionViewModel Commands testen
- ? CanExecute-Logik prüfen
- ? CreateModel/EditModel Delegates konfigurieren
- ? Multi-Selection mit SelectedItems testen

### Wichtigste Punkte

1. **SynchronizationContext**: Erforderlich für ObservableCollection-Tests
2. **DataStoresFixtureBase**: Vollständiger Bootstrap für Integration-Tests
3. **CollectionViewModelFixture**: Vorgefertigte Fixture mit Helper-Methoden
4. **Test-Isolation**: ClearTestData() oder ResetSut() verwenden
5. **Dispose-Pattern**: SUT und Fixture immer disposed

### Weiterführende Dokumentation

- [CollectionViewModel Guide](CollectionViewModel_Guide.md) - Basis-Funktionalität
- [EditableCollectionViewModel Guide](EditableCollectionViewModel_Guide.md) - Commands
- [Custom Controls Guide](CustomControls_Guide.md) - ListEditorView und DropDownEditorView
- [TestHelpers README](../../CustomWPFControls.TestHelpers/README.md) - Test-Infrastruktur
