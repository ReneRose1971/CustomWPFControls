# Migration Guide: Globaler Store ? Lokaler ModelStore

Dieser Guide erklärt die Migration von der alten CollectionViewModel-API (globaler DataStore) zur neuen API (lokaler ModelStore).

## ?? Inhaltsverzeichnis

- [Übersicht der Änderungen](#übersicht-der-änderungen)
- [Breaking Changes](#breaking-changes)
- [Migration Schritt-für-Schritt](#migration-schritt-für-schritt)
- [Vor/Nachher-Beispiele](#vornachher-beispiele)
- [FAQs](#faqs)

---

## Übersicht der Änderungen

### Hauptänderungen

| Aspekt | Alt (? Deprecated) | Neu (? Aktuell) |
|--------|---------------------|------------------|
| **Store-Typ** | Globaler Shared Store | Lokaler isolierter Store |
| **Constructor** | `new CollectionViewModel(dataStore, factory, comparer)` | `new CollectionViewModel(services, viewModelFactory)` |
| **Daten hinzufügen** | `globalStore.Add(model)` | `viewModel.ModelStore.Add(model)` |
| **Store-Zugriff** | Via DI: `dataStores.GetGlobal<T>()` | Via Property: `viewModel.ModelStore` |
| **Isolation** | Alle Instanzen teilen Daten | Jede Instanz hat eigene Daten |

### Vorteile der neuen API

? **Isolation:** Jede CollectionViewModel-Instanz hat ihre eigenen Daten  
? **Testbarkeit:** Tests laufen isoliert ohne globalen State  
? **Flexibilität:** Mehrere Listen desselben Typs möglich  
? **Klarheit:** Datenquelle ist explizit (ModelStore Property)  
? **Konsistenz:** EditableCollectionViewModel.AddCommand verwendet lokalen Store  

---

## Breaking Changes

### 1. Constructor-Signatur geändert

**Alt:**
```csharp
? public CollectionViewModel(
    IDataStore<TModel> dataStore,
    IViewModelFactory<TModel, TViewModel> viewModelFactory,
    IEqualityComparer<TModel> modelComparer)
```

**Neu:**
```csharp
? public CollectionViewModel(
    ICustomWPFServices services,
    IViewModelFactory<TModel, TViewModel> viewModelFactory)
```

**Warum?**
- Store wird nun intern erstellt (lokaler Store)
- Comparer wird über `services.ComparerService` aufgelöst
- Einfachere API, weniger Parameter

### 2. AddModel/RemoveModel Methoden entfernt

**Alt:**
```csharp
? collectionViewModel.AddModel(customer);
? collectionViewModel.RemoveModel(customer);
```

**Neu:**
```csharp
? collectionViewModel.ModelStore.Add(customer);
? collectionViewModel.Remove(viewModel);  // Preferred: Entfernt und disposed
```

**Warum?**
- `ModelStore` Property bietet vollen DataStore-Zugriff
- `Remove(viewModel)` API ist besser für ViewModel-Lifecycle-Management

### 3. ModelStore Property hinzugefügt

**Neu:**
```csharp
? public IDataStore<TModel> ModelStore { get; }
```

**Verwendung:**
```csharp
// Alle DataStore-Operations verfügbar
collectionViewModel.ModelStore.Add(customer);
collectionViewModel.ModelStore.AddRange(customers);
collectionViewModel.ModelStore.Remove(customer);
collectionViewModel.ModelStore.Clear();
```

### 4. EditableCollectionViewModel.AddCommand geändert

**Alt:**
```csharp
? // AddCommand fügte zu globalem Store hinzu
var modelStore = _services.DataStores.GetGlobal<TModel>();
modelStore.Add(model);
```

**Neu:**
```csharp
? // AddCommand fügt zu lokalem ModelStore hinzu
ModelStore.Add(model);
```

**Warum?**
- Konsistenz: Alle Commands arbeiten mit lokalem Store
- Isolation: Jede Instanz verwaltet ihre eigenen Daten

---

## Migration Schritt-für-Schritt

### Schritt 1: Constructor-Aufruf aktualisieren

**Alt:**
```csharp
? public MainViewModel(
    IDataStores dataStores,
    IViewModelFactory<Customer, CustomerViewModel> factory,
    IEqualityComparerService comparerService)
{
    var dataStore = dataStores.GetGlobal<Customer>();
    var comparer = comparerService.GetComparer<Customer>();
    
    _customers = new CollectionViewModel<Customer, CustomerViewModel>(
        dataStore,
        factory,
        comparer);
}
```

**Neu:**
```csharp
? public MainViewModel(
    ICustomWPFServices services,
    IViewModelFactory<Customer, CustomerViewModel> factory)
{
    _customers = new CollectionViewModel<Customer, CustomerViewModel>(
        services,
        factory);
}
```

### Schritt 2: Daten-Operationen migrieren

**Alt:**
```csharp
? // GlobalStore via DI
private readonly IDataStores _dataStores;

public void AddCustomer(Customer customer)
{
    var globalStore = _dataStores.GetGlobal<Customer>();
    globalStore.Add(customer);
}
```

**Neu:**
```csharp
? // ModelStore Property
public void AddCustomer(Customer customer)
{
    _customers.ModelStore.Add(customer);
}
```

### Schritt 3: EditableCollectionViewModel migrieren

**Alt:**
```csharp
? public class CustomerListViewModel
{
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        var dataStore = dataStores.GetGlobal<Customer>();
        var comparer = comparerService.GetComparer<Customer>();
        
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            dataStore,
            factory,
            comparer);
    }
}
```

**Neu:**
```csharp
? public class CustomerListViewModel
{
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services,
            factory);
    }
}
```

### Schritt 4: DI-Registrierung vereinfachen

**Alt:**
```csharp
? public class MyServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // DataStores
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        // ViewModelFactory
        services.AddViewModelFactory<Customer, CustomerViewModel>();
        
        // EqualityComparer
        services.AddSingleton<IEqualityComparer<Customer>>(
            new FallbackEqualsComparer<Customer>());
        
        // ViewModel
        services.AddSingleton<CustomerListViewModel>();
    }
}
```

**Neu:**
```csharp
? public class MyServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // DataStores Core
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        // CustomWPFControls (registriert alles!)
        services.AddCustomWPFControls<Customer, CustomerViewModel>();
        
        // ViewModel
        services.AddSingleton<CustomerListViewModel>();
    }
}
```

---

## Vor/Nachher-Beispiele

### Beispiel 1: Einfache Customer-Liste

**? Alt:**
```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly IDataStores _dataStores;
    
    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        _dataStores = dataStores;
        
        var dataStore = dataStores.GetGlobal<Customer>();
        var comparer = comparerService.GetComparer<Customer>();
        
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            dataStore,
            factory,
            comparer);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public void AddCustomer(string name, string email)
    {
        var customer = new Customer { Name = name, Email = email };
        
        // Zu globalem Store hinzufügen
        var globalStore = _dataStores.GetGlobal<Customer>();
        globalStore.Add(customer);
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

**? Neu:**
```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        // Lokaler Store wird automatisch erstellt
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services,
            factory);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public void AddCustomer(string name, string email)
    {
        var customer = new Customer { Name = name, Email = email };
        
        // Zu lokalem ModelStore hinzufügen
        _customers.ModelStore.Add(customer);
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

### Beispiel 2: EditableCollectionViewModel

**? Alt:**
```csharp
public class ProductListViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Product, ProductViewModel> factory,
        IEqualityComparerService comparerService)
    {
        var dataStore = dataStores.GetGlobal<Product>();
        var comparer = comparerService.GetComparer<Product>();
        
        _products = new EditableCollectionViewModel<Product, ProductViewModel>(
            dataStore,
            factory,
            comparer);
        
        _products.CreateModel = () => new Product { Id = 0 };
    }
    
    public ReadOnlyObservableCollection<ProductViewModel> Products => _products.Items;
    public ICommand AddCommand => _products.AddCommand;
    
    public void Dispose() => _products.Dispose();
}
```

**? Neu:**
```csharp
public class ProductListViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Product, ProductViewModel> factory)
    {
        // Lokaler Store wird automatisch erstellt
        _products = new EditableCollectionViewModel<Product, ProductViewModel>(
            services,
            factory);
        
        _products.CreateModel = () => new Product { Id = 0 };
        // AddCommand fügt jetzt zu lokalem ModelStore hinzu!
    }
    
    public ReadOnlyObservableCollection<ProductViewModel> Products => _products.Items;
    public ICommand AddCommand => _products.AddCommand;
    
    public void Dispose() => _products.Dispose();
}
```

### Beispiel 3: Mehrere Listen desselben Typs

**? Alt (PROBLEM: Shared State!):**
```csharp
public class MultiListViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _activeCustomers;
    private readonly CollectionViewModel<Customer, CustomerViewModel> _inactiveCustomers;
    
    public MultiListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        var dataStore = dataStores.GetGlobal<Customer>();  // ? Problem!
        var comparer = comparerService.GetComparer<Customer>();
        
        // ? BEIDE Instanzen teilen denselben globalen Store!
        _activeCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            dataStore, factory, comparer);
        
        _inactiveCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            dataStore, factory, comparer);
        
        // ? Beide zeigen die gleichen Daten!
    }
}
```

**? Neu (LÖSUNG: Isolation!):**
```csharp
public class MultiListViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _activeCustomers;
    private readonly CollectionViewModel<Customer, CustomerViewModel> _inactiveCustomers;
    
    public MultiListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        // ? Jede Instanz hat ihren eigenen lokalen Store!
        _activeCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _inactiveCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        // ? Komplett isoliert, verschiedene Daten!
    }
    
    public void MoveToInactive(CustomerViewModel customer)
    {
        // Aus aktiven entfernen, zu inaktiven hinzufügen
        _activeCustomers.Remove(customer);
        _inactiveCustomers.ModelStore.Add(customer.Model);
    }
}
```

---

## FAQs

### F: Warum wurde die API geändert?

**A:** Hauptgründe:
1. **Isolation:** Globaler Store führte zu unerwarteten Side-Effects
2. **Testbarkeit:** Tests interferieren nicht mehr über globalen State
3. **Flexibilität:** Mehrere Listen desselben Typs sind jetzt möglich
4. **Klarheit:** Datenquelle ist explizit (ModelStore Property)

### F: Kann ich noch globalen Store verwenden?

**A:** Nein, CollectionViewModel erstellt immer einen lokalen Store. Wenn Sie Daten zwischen Instanzen teilen müssen:

```csharp
// Option 1: Manuell synchronisieren
_customers1.ModelStore.Add(customer);
_customers2.ModelStore.Add(customer);  // Explizite Synchronisation

// Option 2: Shared Service-Layer
public class CustomerService
{
    private readonly IDataStore<Customer> _globalStore;
    
    public void AddCustomer(Customer customer)
    {
        _globalStore.Add(customer);
        // Notify alle Subscribers
    }
}
```

### F: Wie migriere ich bestehende Tests?

**A:** Hauptänderungen:
```csharp
? Alt:
var dataStore = new LocalDataStore<Customer>();
var collectionVM = new CollectionViewModel<Customer, CustomerViewModel>(
    dataStore, factory, comparer);

? Neu:
// Test-Fixture stellt services bereit
var collectionVM = new CollectionViewModel<Customer, CustomerViewModel>(
    services, factory);

// Daten via ModelStore hinzufügen
collectionVM.ModelStore.Add(customer);
```

### F: Was ist mit IDataStores.GetGlobal()?

**A:** GetGlobal() existiert noch in DataStores-Framework, wird aber nicht mehr von CollectionViewModel verwendet:

```csharp
// ? GetGlobal() existiert weiterhin für andere Use-Cases
var globalStore = dataStores.GetGlobal<Customer>();

// ? Aber CollectionViewModel verwendet es nicht mehr
// ? Stattdessen: Lokaler Store via services.DataStores.CreateLocal<T>()
```

### F: Performance-Auswirkungen?

**A:** Keine negativen Auswirkungen:
- Lokale Stores sind genauso performant wie globale
- TransformTo-Mechanismus ist effizienter als manuelle Synchronisation
- Memory-Overhead ist minimal (nur für zusätzliche Store-Instanz)

### F: Wie synchronisiere ich mehrere CollectionViewModel-Instanzen?

**A:** Wenn Sie wirklich Shared State brauchen:

```csharp
// Option 1: Master-Detail Pattern
public class MasterDetailViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly CollectionViewModel<Order, OrderViewModel> _orders;
    
    public MasterDetailViewModel(/* DI */)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, customerFactory);
        
        _orders = new CollectionViewModel<Order, OrderViewModel>(
            services, orderFactory);
        
        // SelectedItem-Change synchronisieren
        _customers.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_customers.SelectedItem))
            {
                LoadOrdersForCustomer(_customers.SelectedItem);
            }
        };
    }
    
    private void LoadOrdersForCustomer(CustomerViewModel? customer)
    {
        _orders.ModelStore.Clear();
        
        if (customer != null)
        {
            var orders = _orderService.GetOrdersForCustomer(customer.Model.Id);
            _orders.ModelStore.AddRange(orders);
        }
    }
}

// Option 2: Event-Aggregator Pattern
public class EventBasedSyncViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly IEventAggregator _eventAggregator;
    
    public EventBasedSyncViewModel(/* DI */)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        // Subscribe to global events
        _eventAggregator.GetEvent<CustomerAddedEvent>()
            .Subscribe(customer =>
            {
                _customers.ModelStore.Add(customer);
            });
    }
}
```

---

## Siehe auch

- ?? [CollectionViewModel](CollectionViewModel.md) - Vollständige Dokumentation
- ?? [EditableCollectionViewModel](EditableCollectionViewModel.md) - Commands
- ?? [Getting Started](Getting-Started.md) - Schnellstart-Guide
- ?? [CustomWPFServices](CustomWPFControls.md) - Service Facade
