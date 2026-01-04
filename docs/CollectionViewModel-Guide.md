# CollectionViewModel & EditableCollectionViewModel - Developer Guide

## Übersicht

Das **CollectionViewModel-System** bietet eine vollautomatische DataStore-Integration für WPF-Collections mit Model-ViewModel-Synchronisation, Selection-Management und Command-Pattern-Support.

### Kern-Konzepte

- **Model-ViewModel-Trennung**: Domain-Logik (Model) getrennt von UI-Logik (ViewModel)
- **DataStore-Integration**: Zentrale Datenhaltung mit automatischer Synchronisation
- **TransformTo-Pattern**: Automatisches Mapping Model ? ViewModel via Factory
- **Selection-Management**: Single- und Multi-Selection mit automatischer Invalidierung
- **Command-Pattern**: CRUD-Operationen via ICommand (Add, Edit, Delete, Clear)

---

## Architecture

```
???????????????????????????????????????????????????????????
?                   UI Layer (XAML)                        ?
?  ListBox/ComboBox ? Binding to CollectionViewModel      ?
???????????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????????
?         CollectionViewModel<TModel, TViewModel>         ?
?  ???????????????????????????????????????????????????   ?
?  ? Items (ReadOnlyObservableCollection)            ?   ?
?  ? SelectedItem / SelectedItems                    ?   ?
?  ? Remove() / RemoveRange() / Clear()              ?   ?
?  ???????????????????????????????????????????????????   ?
???????????????????????????????????????????????????????????
             ?                   ?
    ????????????????????  ??????????????????????????
    ? Model-DataStore  ?  ? ViewModel-DataStore    ?
    ?   (Global)       ?  ?     (Local)            ?
    ????????????????????  ??????????????????????????
             ?                   ?
             ????????TransformTo??
                 (Factory-based Sync)
```

---

## Core Components

### 1. CollectionViewModel<TModel, TViewModel>

**Verantwortlichkeiten:**
- Model-ViewModel-Synchronisation via TransformTo
- ObservableCollection-Bereitstellung für WPF-Binding
- Selection-Management (Single + Multi)
- Automatische Invalidierung bei Item-Removal

**Konstruktor:**

```csharp
public CollectionViewModel(
    ICustomWPFServices services,
    IViewModelFactory<TModel, TViewModel> viewModelFactory)
```

**Properties:**

| Property | Typ | Beschreibung |
|----------|-----|--------------|
| `Items` | `ReadOnlyObservableCollection<TViewModel>` | Schreibgeschützte Collection für View-Binding |
| `SelectedItem` | `TViewModel?` | Single-Selection, wird automatisch bei Removal invalidiert |
| `SelectedItems` | `ObservableCollection<TViewModel>` | Multi-Selection für ListBox (via MultiSelectBehavior) |
| `Count` | `int` | Anzahl der Items, löst PropertyChanged aus |

**Methoden:**

| Methode | Beschreibung |
|---------|--------------|
| `Remove(TViewModel item)` | Entfernt Item, invalidiert Selection |
| `RemoveRange(IEnumerable<TViewModel> items)` | Bulk-Remove mit Selection-Invalidierung |
| `Clear()` | Leert Collection, setzt Selection zurück |
| `Dispose()` | Cleanup: Unsubscribe, Dispose Syncs & ViewModels |

---

### 2. EditableCollectionViewModel<TModel, TViewModel>

**Erweitert CollectionViewModel** um CRUD-Commands.

**Zusätzliche Properties:**

| Property | Typ | Beschreibung |
|----------|-----|--------------|
| `CreateModel` | `Func<TModel>?` | Factory für neue Models (required für AddCommand) |
| `EditModel` | `Action<TModel>?` | Callback für Edit-Dialogs |

**Commands:**

| Command | CanExecute | Execute |
|---------|------------|---------|
| `AddCommand` | `CreateModel != null` | Erstellt Model, fügt zu Store hinzu |
| `DeleteCommand` | `SelectedItem != null` | Löscht selektiertes Item |
| `DeleteSelectedCommand` | `SelectedItems.Count > 0` | Löscht alle selektierten Items |
| `ClearCommand` | `Count > 0` | Leert gesamte Collection |
| `EditCommand` | `SelectedItem != null && EditModel != null` | Öffnet Edit-Dialog |

---

## Interfaces & Contracts

### IViewModelWrapper<TModel>

```csharp
public interface IViewModelWrapper<out TModel> where TModel : class
{
    TModel Model { get; }
}
```

**Zweck:** Ermöglicht CollectionViewModel den Zugriff auf das gewrappte Model für DataStore-Operationen.

**Implementation-Beispiel:**

```csharp
public class CustomerViewModel : IViewModelWrapper<Customer>, INotifyPropertyChanged
{
    private readonly Customer _model;
    
    public Customer Model => _model;
    
    public string Name
    {
        get => _model.Name;
        set
        {
            if (_model.Name != value)
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }
    }
    
    public CustomerViewModel(Customer model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }
}
```

### IViewModelFactory<TModel, TViewModel>

```csharp
public interface IViewModelFactory<in TModel, out TViewModel>
    where TModel : class
    where TViewModel : class
{
    TViewModel Create(TModel model);
}
```

**Zweck:** Factory-Pattern für ViewModel-Erstellung mit DI-Support.

**Implementation-Beispiel:**

```csharp
public class CustomerViewModelFactory : IViewModelFactory<Customer, CustomerViewModel>
{
    public CustomerViewModel Create(Customer model)
    {
        return new CustomerViewModel(model);
    }
}
```

**DI-Registrierung:**

```csharp
services.AddSingleton<IViewModelFactory<Customer, CustomerViewModel>, CustomerViewModelFactory>();
```

---

## Usage Examples

### Basic Setup (Read-Only Collection)

**ViewModel:**

```csharp
public class CustomersPageViewModel
{
    public CollectionViewModel<Customer, CustomerViewModel> Customers { get; }
    
    public CustomersPageViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        Customers = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
    }
}
```

**XAML (ListView):**

```xml
<ListView ItemsSource="{Binding Customers.Items}"
          SelectedItem="{Binding Customers.SelectedItem}">
    <ListView.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}"/>
        </DataTemplate>
    </ListView.ItemTemplate>
</ListView>
```

---

### Editable Collection with Commands

**ViewModel:**

```csharp
public class CustomersManagementViewModel
{
    public EditableCollectionViewModel<Customer, CustomerViewModel> Customers { get; }
    
    public CustomersManagementViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IDialogService dialogService)
    {
        Customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(services, factory);
        
        // Configure CRUD operations
        Customers.CreateModel = () => new Customer { Name = "New Customer" };
        
        Customers.EditModel = (customer) =>
        {
            var editVm = new CustomerEditViewModel(customer);
            dialogService.ShowDialog(editVm);
        };
    }
}
```

**XAML mit Command-Buttons:**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    
    <!-- Toolbar -->
    <StackPanel Orientation="Horizontal" Grid.Row="0">
        <Button Content="Add" Command="{Binding Customers.AddCommand}"/>
        <Button Content="Edit" Command="{Binding Customers.EditCommand}"/>
        <Button Content="Delete" Command="{Binding Customers.DeleteCommand}"/>
        <Button Content="Clear All" Command="{Binding Customers.ClearCommand}"/>
    </StackPanel>
    
    <!-- ListView -->
    <ListView Grid.Row="1"
              ItemsSource="{Binding Customers.Items}"
              SelectedItem="{Binding Customers.SelectedItem}">
        <ListView.ItemTemplate>
            <DataTemplate>
                <TextBlock Text="{Binding Name}"/>
            </DataTemplate>
        </ListView.ItemTemplate>
    </ListView>
</Grid>
```

---

### Multi-Selection mit MultiSelectBehavior

**XAML:**

```xml
<Window xmlns:behaviors="clr-namespace:CustomWPFControls.Behaviors;assembly=CustomWPFControls">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        
        <!-- Toolbar -->
        <StackPanel Orientation="Horizontal" Grid.Row="0">
            <Button Content="Delete Selected" 
                    Command="{Binding Customers.DeleteSelectedCommand}"/>
            <TextBlock Text="{Binding Customers.SelectedItems.Count, 
                              StringFormat='Selected: {0}'}"/>
        </StackPanel>
        
        <!-- ListBox mit Multi-Selection -->
        <ListBox Grid.Row="1"
                 SelectionMode="Multiple"
                 ItemsSource="{Binding Customers.Items}"
                 behaviors:MultiSelectBehavior.SelectedItems="{Binding Customers.SelectedItems}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Name}"/>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </Grid>
</Window>
```

**Hinweis:** `MultiSelectBehavior` ermöglicht bidirektionale Synchronisation zwischen `ListBox.SelectedItems` (ReadOnly) und ViewModel-Collection.

---

### ComboBox / Dropdown-Szenarien

**ViewModel:**

```csharp
public class OrderViewModel
{
    public CollectionViewModel<Customer, CustomerViewModel> AvailableCustomers { get; }
    
    private CustomerViewModel? _selectedCustomer;
    public CustomerViewModel? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
        }
    }
    
    public OrderViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        AvailableCustomers = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
    }
}
```

**XAML:**

```xml
<ComboBox ItemsSource="{Binding AvailableCustomers.Items}"
          SelectedItem="{Binding SelectedCustomer}"
          DisplayMemberPath="Name"/>
```

---

## DataStore Integration

### Model-Store (Global)

Models werden im **globalen DataStore** gespeichert:

```csharp
var customerStore = services.DataStores.GetGlobal<Customer>();
customerStore.Add(new Customer { Name = "John Doe" });
```

**TransformTo** synchronisiert automatisch:
```
Global Model-Store ? Local ViewModel-Store ? ObservableCollection (Items)
```

### ViewModel-Store (Local)

Wird intern von CollectionViewModel erstellt:

```csharp
_viewModelStore = services.DataStores.CreateLocal<TViewModel>();
```

**Lifecycle:**
- Automatisch erstellt beim Konstruktor
- Disposed beim CollectionViewModel.Dispose()
- ViewModels werden automatisch disposed (via TransformTo)

---

## Selection Management

### Automatische Invalidierung

**Bei Remove():**
```csharp
public bool Remove(TViewModel item)
{
    var removed = _modelStore.Remove(item.Model);
    
    if (removed)
    {
        // Automatische Invalidierung
        if (SelectedItem == item)
            SelectedItem = null;
            
        _selectedItems.Remove(item);
    }
    
    return removed;
}
```

**Bei Clear():**
```csharp
public void Clear()
{
    _modelStore.Clear();
    
    SelectedItem = null;
    _selectedItems.Clear();
}
```

### Best Practices

? **DO:**
- Verwende `SelectedItem` für Single-Selection-Szenarien
- Verwende `SelectedItems` + `MultiSelectBehavior` für Multi-Selection
- Binde Commands an ViewModel-Level, nicht Code-Behind

? **DON'T:**
- Manipuliere `Items` direkt (ist ReadOnly)
- Vergiss nicht `Dispose()` aufzurufen (Memory Leaks!)
- Verwende manuelle Event-Handler für Selection-Sync (nutze Behavior)

---

## Testing

### Test-Fixture Setup

```csharp
public class CollectionViewModelFixture : DataStoresFixtureBase
{
    public IDataStore<TestDto> TestDtoStore { get; protected set; } = null!;
    public IViewModelFactory<TestDto, TestViewModel> ViewModelFactory { get; protected set; } = null!;
    public ICustomWPFServices Services { get; protected set; } = null!;
    
    protected override void InitializeServices()
    {
        TestDtoStore = DataStores.GetGlobal<TestDto>();
        ViewModelFactory = ServiceProvider.GetRequiredService<IViewModelFactory<TestDto, TestViewModel>>();
        Services = ServiceProvider.GetRequiredService<ICustomWPFServices>();
    }
    
    protected override void InitializeData()
    {
        // Store bleibt leer - Tests fügen eigene Daten hinzu
    }
    
    public void ClearTestData()
    {
        TestDtoStore.Clear();
    }
}
```

### Test-Beispiel

```csharp
public sealed class SelectedItem_CanBeSet : IClassFixture<CollectionViewModelFixture>
{
    private readonly CollectionViewModelFixture _fixture;
    
    public SelectedItem_CanBeSet(CollectionViewModelFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public void Test_SelectedItem_CanBeSet()
    {
        // Arrange
        _fixture.TestDtoStore.Add(new TestDto { Name = "Test" });
        
        var sut = new CollectionViewModel<TestDto, TestViewModel>(
            _fixture.Services,
            _fixture.ViewModelFactory);
        
        // Act
        sut.SelectedItem = sut.Items.First();
        
        // Assert
        sut.SelectedItem.Should().NotBeNull();
        
        // Cleanup
        _fixture.ClearTestData();
        sut.Dispose();
    }
}
```

---

## Advanced Patterns

### Custom Filtering

```csharp
public class CustomersPageViewModel
{
    private CollectionViewModel<Customer, CustomerViewModel> _allCustomers;
    
    public ICollectionView FilteredCustomers { get; }
    
    public CustomersPageViewModel(ICustomWPFServices services, ...)
    {
        _allCustomers = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
        
        FilteredCustomers = CollectionViewSource.GetDefaultView(_allCustomers.Items);
        FilteredCustomers.Filter = obj =>
        {
            if (obj is CustomerViewModel vm)
                return vm.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase);
            return false;
        };
    }
}
```

### Hierarchical Data

```csharp
public class CategoryViewModel : IViewModelWrapper<Category>
{
    public Category Model { get; }
    
    public CollectionViewModel<Product, ProductViewModel> Products { get; }
    
    public CategoryViewModel(
        Category model,
        ICustomWPFServices services,
        IViewModelFactory<Product, ProductViewModel> productFactory)
    {
        Model = model;
        
        // Filter products by category
        var categoryProducts = services.DataStores.GetGlobal<Product>()
            .Items.Where(p => p.CategoryId == model.Id);
        
        Products = new CollectionViewModel<Product, ProductViewModel>(services, productFactory);
    }
}
```

---

## Common Pitfalls

### 1. Memory Leaks durch fehlende Dispose

? **Falsch:**
```csharp
public class MyViewModel
{
    public CollectionViewModel<Customer, CustomerViewModel> Customers { get; }
    
    // KEIN Dispose() implementiert!
}
```

? **Richtig:**
```csharp
public class MyViewModel : IDisposable
{
    public CollectionViewModel<Customer, CustomerViewModel> Customers { get; }
    
    public void Dispose()
    {
        Customers?.Dispose();
    }
}
```

### 2. CreateModel vergessen

? **Falsch:**
```csharp
var editableVm = new EditableCollectionViewModel<Customer, CustomerViewModel>(services, factory);
// CreateModel nicht gesetzt!
// AddCommand.CanExecute() gibt false zurück
```

? **Richtig:**
```csharp
var editableVm = new EditableCollectionViewModel<Customer, CustomerViewModel>(services, factory);
editableVm.CreateModel = () => new Customer();
```

### 3. SelectedItems vs. SelectedItem

? **Falsch:**
```csharp
// ListBox mit SelectionMode="Multiple" aber nur SelectedItem gebunden
<ListBox SelectionMode="Multiple"
         SelectedItem="{Binding Customers.SelectedItem}"/>
```

? **Richtig:**
```csharp
<ListBox SelectionMode="Multiple"
         behaviors:MultiSelectBehavior.SelectedItems="{Binding Customers.SelectedItems}"/>
```

---

## Performance Considerations

### TransformTo-Overhead

- ViewModels werden bei jedem Model-Add erstellt
- Bei großen Datasets (>1000 Items): Virtualisierung nutzen

```xml
<ListView ItemsSource="{Binding Items}"
          VirtualizingPanel.IsVirtualizing="True"
          VirtualizingPanel.VirtualizationMode="Recycling"/>
```

### Bulk-Operations

? **Prefer:**
```csharp
var itemsToRemove = Customers.SelectedItems.ToList();
Customers.RemoveRange(itemsToRemove); // Eine Operation
```

? **Avoid:**
```csharp
foreach (var item in Customers.SelectedItems.ToList())
{
    Customers.Remove(item); // N Operationen
}
```

---

## Migration Guide (von alten Patterns)

### Alt: ObservableCollection<T> im ViewModel

```csharp
// Alt
public class OldViewModel
{
    public ObservableCollection<Customer> Customers { get; } = new();
    
    public void LoadCustomers()
    {
        var customers = _repository.GetAll();
        Customers.Clear();
        foreach (var c in customers)
            Customers.Add(c);
    }
}
```

### Neu: CollectionViewModel

```csharp
// Neu
public class NewViewModel
{
    public CollectionViewModel<Customer, CustomerViewModel> Customers { get; }
    
    public NewViewModel(ICustomWPFServices services, IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        Customers = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
        
        // Daten in DataStore laden
        var customerStore = services.DataStores.GetGlobal<Customer>();
        customerStore.AddRange(_repository.GetAll());
        // Automatische Synchronisation via TransformTo!
    }
}
```

**XAML-Anpassung:**
```xml
<!-- Alt -->
<ListView ItemsSource="{Binding Customers}"/>

<!-- Neu -->
<ListView ItemsSource="{Binding Customers.Items}"/>
```

---

## Cheat Sheet

| Szenario | Klasse | Properties | XAML |
|----------|--------|------------|------|
| Read-Only List | `CollectionViewModel` | `Items`, `SelectedItem` | `ItemsSource="{Binding Items}"` |
| Editable List | `EditableCollectionViewModel` | + `CreateModel`, `EditModel` | + Command-Buttons |
| Multi-Selection | `CollectionViewModel` | `SelectedItems` | `MultiSelectBehavior.SelectedItems` |
| Dropdown | `CollectionViewModel` | `Items` | `ComboBox ItemsSource` |

---

## Dependencies

| Service | Purpose |
|---------|---------|
| `ICustomWPFServices` | Facade für DataStores, ComparerService, DialogService |
| `IViewModelFactory<TModel, TViewModel>` | ViewModel-Erstellung mit DI |
| `IDataStore<T>` | Zentrale Datenhaltung |
| `IEqualityComparerService` | Model-Vergleich für TransformTo |

---

## Summary

**CollectionViewModel** bietet:
- ? Automatische Model-ViewModel-Synchronisation
- ? DataStore-Integration out-of-the-box
- ? Selection-Management mit Invalidierung
- ? Command-Pattern für CRUD (via EditableCollectionViewModel)
- ? WPF-bindbare ObservableCollections
- ? Testbarkeit durch DI-Architektur

**Verwende es für:**
- Listen-Ansichten (ListView, DataGrid)
- Dropdowns (ComboBox)
- Master-Detail-Szenarien
- Alle CRUD-Operationen mit Collections

**Nächste Schritte:**
1. Implementiere `IViewModelWrapper<TModel>` für deine ViewModels
2. Registriere `IViewModelFactory<TModel, TViewModel>` in DI
3. Erstelle `CollectionViewModel` oder `EditableCollectionViewModel`
4. Binde `Items` an XAML-Controls
5. Nutze Commands für CRUD-Operationen
