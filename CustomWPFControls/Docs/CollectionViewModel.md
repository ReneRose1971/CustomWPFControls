# CollectionViewModel<TModel, TViewModel>

Collection-ViewModel mit **lokalem DataStore** und automatischer Synchronisation via TransformTo.

## ?? Inhaltsverzeichnis

- [Überblick](#überblick)
- [Features](#features)
- [Verwendung](#verwendung)
- [Lokaler ModelStore](#lokaler-modelstore)
- [TransformTo-Integration](#transformto-integration)
- [ViewModel-Lifecycle](#viewmodel-lifecycle)
- [Best Practices](#best-practices)
- [Beispiele](#beispiele)

## Überblick

`CollectionViewModel<TModel, TViewModel>` ist die **zentrale Klasse** für Collection-Verwaltung in CustomWPFControls. Sie:

- ?? **Synchronisiert** lokalen ModelStore ? ViewModels automatisch (via TransformTo)
- ??? **Erstellt** ViewModels automatisch via Factory
- ??? **Disposed** ViewModels automatisch bei Entfernung
- ?? **Isoliert** Daten in eigenem lokalen Store
- ?? **Tracked** SelectedItem/SelectedItems für UI-Binding
- ??? **Bietet** Remove/RemoveRange/Clear API

### Definition

```csharp
namespace CustomWPFControls.ViewModels;

public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
    where TModel : class
    where TViewModel : class, IViewModelWrapper<TModel>
{
    // ?? Readonly Access - Lokaler Store für diese Instanz
    public IDataStore<TModel> ModelStore { get; }
    
    // ?? UI-Binding Collections
    public ReadOnlyObservableCollection<TViewModel> Items { get; }
    public TViewModel? SelectedItem { get; set; }
    public ObservableCollection<TViewModel> SelectedItems { get; }
    public int Count { get; }
    
    // ??? Collection Manipulation API
    public bool Remove(TViewModel item);
    public int RemoveRange(IEnumerable<TViewModel> items);
    public void Clear();
    public void LoadModels(IEnumerable<TModel> models);
    
    // ??? Constructor mit CustomWPFServices Facade
    public CollectionViewModel(
        ICustomWPFServices services,
        IViewModelFactory<TModel, TViewModel> viewModelFactory);
}
```

## Features

### 1. Lokaler ModelStore (Isolierte Daten)

**Jede CollectionViewModel-Instanz hat ihren eigenen lokalen Store:**
```csharp
var customerList1 = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
var customerList2 = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);

// ? Beide Instanzen sind vollständig isoliert!
customerList1.ModelStore.Add(customer1);  // Nur in customerList1 sichtbar
customerList2.ModelStore.Add(customer2);  // Nur in customerList2 sichtbar

// ? Kein globaler Shared State!
```

**ModelStore Property:**
```csharp
// Lokaler Store ist readonly accessible
collectionViewModel.ModelStore.Add(newCustomer);
collectionViewModel.ModelStore.Remove(customer);
collectionViewModel.ModelStore.Clear();

// ? Alle Änderungen werden automatisch via TransformTo synchronisiert!
```

### 2. TransformTo-Integration (Automatische ViewModel-Erstellung)

**Nahtlose Integration mit TransformTo:**
```csharp
var customer = new Customer();
var customerViewModel = transformToService.TransformTo<Customer, CustomerViewModel>(customer);

// ? Automatisches Mapping von Customer zu CustomerViewModel
// ? Alle ViewModels werden automatisch der Collection hinzugefügt
```

### 3. ViewModel-Lifecycle

**Automatisches Lifecycle-Management:**
```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    // ? Automatisches Subscription-Management für Collection-Änderungen
    // ? Automatisches IDisposable für ViewModels bei Entfernung
}
```

### 4. SelectedItem/SelectedItems-Management

**Automatisches Selection-Tracking:**
```csharp
// SelectedItem wird automatisch invalidiert bei Remove
collectionViewModel.SelectedItem = viewModel1;
collectionViewModel.Remove(viewModel1);
// ? SelectedItem wird automatisch auf null gesetzt

// SelectedItems werden automatisch bereinigt
collectionViewModel.SelectedItems.Add(viewModel1);
collectionViewModel.SelectedItems.Add(viewModel2);
collectionViewModel.Remove(viewModel1);
// ? viewModel1 wird automatisch aus SelectedItems entfernt

// Clear invalidiert alle Selections
collectionViewModel.Clear();
// ? SelectedItem = null
// ? SelectedItems.Clear()

// LoadModels invalidiert alle Selections (via Clear)
collectionViewModel.LoadModels(newModels);
// ? SelectedItem = null
// ? SelectedItems.Clear()
```

### 5. LoadModels API

**Ersetzt alle Models in der Collection (atomare Operation):**
```csharp
// Signatur
public void LoadModels(IEnumerable<TModel> models)

// Funktionalität:
// 1. Clear() aufrufen (entfernt alle Items, invalidiert Selection)
// 2. ModelStore.AddRange(models) (fügt neue Items hinzu)
// 3. TransformTo erstellt automatisch neue ViewModels
```

**Verwendung:**
```csharp
// Standard: Alle Items ersetzen
var newCustomers = await repository.GetAllAsync();
collectionViewModel.LoadModels(newCustomers);

// Leere Liste ist valide (leert die Collection)
collectionViewModel.LoadModels(Array.Empty<Customer>());

// ? Null wirft ArgumentNullException
collectionViewModel.LoadModels(null);  // Exception!
```

**Vorteile gegenüber manueller Clear + AddRange:**
```csharp
// ? Manuell (umständlich, vergisst Selection-Invalidierung)
collectionViewModel.ModelStore.Clear();
collectionViewModel.ModelStore.AddRange(newModels);
collectionViewModel.SelectedItem = null;
collectionViewModel.SelectedItems.Clear();

// ? LoadModels (kompakt, garantiert Selection-Invalidierung)
collectionViewModel.LoadModels(newModels);

```

## Verwendung

### Beispiel 1: Einfache Customer-Liste

```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        // ? Lokaler Store wird automatisch erstellt
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services,
            factory);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public CustomerViewModel? SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    public void AddCustomer(string name, string email)
    {
        var customer = new Customer { Name = name, Email = email };
        
        // ? Fügt zum lokalen ModelStore hinzu
        _customers.ModelStore.Add(customer);
        // ? TransformTo erstellt automatisch ViewModel
        // ? UI wird automatisch aktualisiert
    }
    
    public void LoadCustomers(IEnumerable<Customer> customers)
    {
        // ? Ersetzt alle Kunden in einem Aufruf
        _customers.LoadModels(customers);
        // ? Selection wird automatisch invalidiert
    }
    
    public void DeleteSelectedCustomer()
    {
        if (SelectedCustomer != null)
        {
            // ? Remove-API entfernt und disposed
            _customers.Remove(SelectedCustomer);
        }
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
