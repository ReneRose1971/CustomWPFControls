# Best Practices - CustomWPFControls

Bewährte Praktiken, Tipps und Tricks für die Verwendung von CustomWPFControls.

## Inhaltsverzeichnis

- [ViewModel-Design](#viewmodel-design)
- [DataStore-Integration](#datastore-integration)
- [Commands](#commands)
- [Controls](#controls)
- [Performance](#performance)
- [Testing](#testing)

---

## ViewModel-Design

### Properties von Model delegieren

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    // Read-only, delegiert an Model
    public string Name => Model.Name;
    public string Email => Model.Email;
    
    // UI-spezifische Properties
    public bool IsSelected { get; set; }
}
```

### Computed Properties für UI-Logik

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public string Name => Model.Name;
    public string Email => Model.Email;
    
    // Computed Property für Display
    public string DisplayName => $"{Name} ({Email})";
    public string ShortName => Name.Length > 20 
        ? Name.Substring(0, 20) + "..." 
        : Name;
}
```

### ViewModelBase als Basis verwenden

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
}
```

**Warum?**
- Automatisches PropertyChanged via Fody
- Model-Property bereits vorhanden
- Equals/GetHashCode implementiert

---

## DataStore-Integration

### IEqualityComparer implementieren

```csharp
public class CustomerComparer : IEqualityComparer<Customer>
{
    public bool Equals(Customer? x, Customer? y)
    {
        if (x == null || y == null) return false
;
        return x.Id == y.Id; // Stabile Property (Id)
    }

    public int GetHashCode(Customer obj)
    {
        return obj.Id.GetHashCode();
    }
}

// DI-Registrierung
services.AddSingleton<IEqualityComparer<Customer>>(new CustomerComparer());
```

**Warum Id verwenden?**
- Id ist unveränderlich nach DB-Insert
- HashCode bleibt stabil
- Dictionary-Lookups funktionieren zuverlässig

### DataStore via DI injizieren

```csharp
public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // Singleton für shared state
        services.AddSingleton<IEqualityComparer<Customer>>(
            new FallbackEqualsComparer<Customer>());
    }
}
```

---

## Commands

### CreateModel für neue Objekte

```csharp
var viewModel = new EditableCollectionViewModel<Customer, CustomerViewModel>(
    dataStores, factory, comparerService);

viewModel.CreateModel = () => new Customer
{
    Name = "Neuer Kunde",
    Email = "neu@example.com",
    CreatedAt = DateTime.Now
};
```

### EditModel für Bearbeitung

```csharp
viewModel.EditModel = customer =>
{
    var dialog = new CustomerEditDialog
    {
        DataContext = new CustomerEditViewModel(customer)
    };
    
    if (dialog.ShowDialog() == true)
    {
        // Model wurde bearbeitet
        // DataStore aktualisiert automatisch
    }
};
```

### Commands via Binding verwenden

```xml
<Button Content="Hinzufügen" 
        Command="{Binding AddCommand}"
        ToolTip="Neuen Kunden hinzufügen"/>

<Button Content="Löschen" 
        Command="{Binding DeleteCommand}"
        ToolTip="Ausgewählten Kunden löschen"/>
```

---

## Controls

### ListEditorView verwenden

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"
    AddButtonText="Neu"
    EditButtonText="Bearbeiten"
    DeleteButtonText="Löschen"/>
```

### DropDownEditorView mit ButtonPlacement

```xml
<!-- Right: Kompakt für Formulare -->
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    ButtonPlacement="Right"
    AddCommand="{Binding AddCommand}"/>

<!-- Bottom: Ähnlich wie ListView -->
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    ButtonPlacement="Bottom"
    AddCommand="{Binding AddCommand}"/>

<!-- Top: Professionell mit ToolBar -->
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    ButtonPlacement="Top"
    AddCommand="{Binding AddCommand}"/>
```

### Button-Texte lokalisieren

```xml
<controls:ListEditorView 
    AddButtonText="{x:Static resx:Resources.ButtonAdd}"
    EditButtonText="{x:Static resx:Resources.ButtonEdit}"
    DeleteButtonText="{x:Static resx:Resources.ButtonDelete}"/>
```

### Selektive Button-Sichtbarkeit

```xml
<controls:DropDownEditorView 
    IsEditVisible="False"
    IsClearVisible="False"/>
```

---

## Performance

### Virtualization für große Listen

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    VirtualizingPanel.IsVirtualizing="True"
    VirtualizingPanel.VirtualizationMode="Recycling"
    VirtualizingPanel.CacheLength="20"/>
```

**Vorteile:**
- Nur sichtbare Items werden gerendert
- Recycling reduziert Memory-Footprint
- Performance bleibt konstant bei 10.000+ Items

### ICollectionView für Filtering

```csharp
var view = CollectionViewSource.GetDefaultView(viewModel.Items);
view.Filter = item =>
{
    var vm = (CustomerViewModel)item;
    return vm.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
};

// Filter aktualisieren
searchTextBox.TextChanged += (s, e) => view.Refresh();
```

### Lazy Loading bei Bedarf

```csharp
public class LazyCustomerViewModel : ViewModelBase<Customer>
{
    private ObservableCollection<OrderViewModel>? _orders;
    
    public ObservableCollection<OrderViewModel> Orders
    {
        get
        {
            // Lazy Load: Nur wenn benötigt
            if (_orders == null)
            {
                _orders = new ObservableCollection<OrderViewModel>(
                    orderRepository.GetByCustomerId(Model.Id)
                        .Select(o => new OrderViewModel(o)));
            }
            return _orders;
        }
    }
}
```

---

## Testing

### Unit-Tests für ViewModels

```csharp
[Fact]
public void CustomerViewModel_Name_ReturnsModelName()
{
    // Arrange
    var customer = new Customer { Name = "Alice" };
    var viewModel = new CustomerViewModel(customer);

    // Act & Assert
    Assert.Equal("Alice", viewModel.Name);
}
```

### Integration-Tests für Synchronisation

```csharp
[Fact]
public void CollectionViewModel_DataStoreAdd_CreatesViewModel()
{
    // Arrange
    var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
        dataStores, factory, comparerService);
    
    // Act
    dataStores.GetGlobal<Customer>().Add(new Customer { Name = "Bob" });
    
    // Assert
    Assert.Equal(1, viewModel.Count);
    Assert.Equal("Bob", viewModel.Items[0].Name);
}
```

### Behavior-Tests für Commands

```csharp
[Fact]
public void AddCommand_WithCreateModel_AddsToDataStore()
{
    // Arrange
    var viewModel = new EditableCollectionViewModel<Customer, CustomerViewModel>(
        dataStores, factory, comparerService);
    viewModel.CreateModel = () => new Customer { Name = "New" };
    
    // Act
    viewModel.AddCommand.Execute(null);
    
    // Assert
    Assert.Equal(1, viewModel.Count);
}
```

---

## Quick Reference

### ViewModel-Checklist

- Von `ViewModelBase<TModel>` ableiten
- Constructor mit `TModel` Parameter
- Domain-Properties read-only (delegiert an Model)
- UI-Properties mit Auto-Property
- Computed Properties für UI-Logik

### DI-Registrierung-Checklist

- IEqualityComparer registriert
- ViewModelFactory registriert (via AddViewModelFactory)
- CollectionViewModel registriert

### Control-Checklist

- SelectedItem mit Mode=TwoWay binden
- ButtonPlacement je nach Kontext wählen
- Button-Texte für Lokalisierung konfigurieren
- Nicht benötigte Buttons ausblenden

### Performance-Checklist

- Virtualization aktiviert (bei > 100 Items)
- ICollectionView für Filtering
- Lazy Loading für Child-Collections

---

## Siehe auch

- [Getting Started](Getting-Started.md)
- [Controls-Guide](Controls-Guide.md)
- [Architecture](Architecture.md)
- [API Reference](API-Reference.md)
