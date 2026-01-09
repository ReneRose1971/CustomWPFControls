# ViewModelBase<TModel>

Basisklasse für ViewModels mit PropertyChanged-Support und Model-Wrapping.

## Übersicht

`ViewModelBase<TModel>` ist die abstrakte Basisklasse für alle ViewModels. Sie wraps ein Domain-Model und bietet:

- Automatisches PropertyChanged via Fody.PropertyChanged
- Referenz-basierte Gleichheit (ViewModels mit gleichem Model sind gleich)
- Model-Wrapping für Trennung zwischen Domain und View

### Definition

```csharp
[AddINotifyPropertyChangedInterface]
public abstract class ViewModelBase<TModel> : IViewModelWrapper<TModel>, INotifyPropertyChanged
    where TModel : class
{
    public TModel Model { get; }
    protected ViewModelBase(TModel model);
    
    public override int GetHashCode();
    public override bool Equals(object? obj);
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

## Contract

### Model-Wrapping

ViewModels wrappen Domain-Models ohne sie zu modifizieren.

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    // Domain-Properties delegieren an Model
    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name != value)
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
    }
    
    // UI-Properties nur im ViewModel
    public bool IsSelected { get; set; }
}
```

### PropertyChanged via Fody

`[AddINotifyPropertyChangedInterface]` implementiert INotifyPropertyChanged automatisch.

```csharp
// PropertyChanged wird automatisch gefeuert
public bool IsSelected { get; set; }
public string SearchFilter { get; set; } = "";
```

**Kein PropertyChanged:**
```csharp
[DoNotNotify]
public string InternalCache { get; set; }
```

**Abhängige Properties:**
```csharp
public string FirstName { get; set; } = "";
public string LastName { get; set; } = "";

[DependsOn(nameof(FirstName), nameof(LastName))]
public string FullName => $"{FirstName} {LastName}";
```

### Gleichheit

ViewModels sind gleich, wenn sie die gleiche Model-Instanz wrappen (Referenz-basiert).

```csharp
var customer = new Customer { Id = 1, Name = "Alice" };
var vm1 = new CustomerViewModel(customer);
var vm2 = new CustomerViewModel(customer);

Assert.True(vm1.Equals(vm2));  // Gleiche Model-Referenz
Assert.Equal(vm1.GetHashCode(), vm2.GetHashCode());
```

## Verwendung

### Minimal-Beispiel

```csharp
public class ProductViewModel : ViewModelBase<Product>
{
    public ProductViewModel(Product model) : base(model) { }
    
    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name != value)
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
    }
    
    public bool IsOnSale { get; set; }
}
```

### Mit DI-Services

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;
    
    public CustomerViewModel(
        Customer model,
        IMessageService messageService) : base(model)
    {
        _messageService = messageService;
    }
    
    public void SendEmail()
    {
        _messageService.Send(Model.Email, "Hello!");
    }
}
```

### Mit Commands

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly ICustomerService _service;
    
    public CustomerViewModel(Customer model, ICustomerService service) 
        : base(model)
    {
        _service = service;
    }
    
    public string Name
    {
        get => Model.Name;
        set
        {
            if (Model.Name != value)
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }
    }
    
    private ICommand? _saveCommand;
    public ICommand SaveCommand => _saveCommand ??= new RelayCommand(
        _ => _service.Save(Model),
        _ => !string.IsNullOrWhiteSpace(Name));
}
```

## Voraussetzungen

- Fody.PropertyChanged und PropertyChanged.Fody NuGet-Pakete
- FodyWeavers.xml im Projekt-Root
- Constructor muss Model als ersten Parameter haben (für ViewModelFactory)

## Siehe auch

- [CollectionViewModel](CollectionViewModel.md)
- [EditableCollectionViewModel](EditableCollectionViewModel.md)
- [ViewModelFactory](ViewModelFactory.md)
- [API Reference](API-Reference.md)
