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
    
    // ??? Removal API
    public bool Remove(TViewModel item);
    public int RemoveRange(IEnumerable<TViewModel> items);
    public void Clear();
    
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

### 2. TransformTo-Integration (Automatische Synchronisation)

**ModelStore ? ViewModelStore:**
```csharp
// Internal: Wird im Constructor erstellt
_modelStore.TransformTo<TModel, TViewModel>(
    _viewModelStore,
    factoryFunc: model => _viewModelFactory.Create(model),
    comparerFunc: (m, vm) => _modelComparer.Equals(m, vm.Model));

// ? ViewModels werden automatisch erstellt und synchronisiert!
```

**Automatischer Lifecycle:**
```csharp
// Model hinzufügen
collectionViewModel.ModelStore.Add(customer);
// ? TransformTo erstellt automatisch CustomerViewModel
// ? ViewModel wird zu Items hinzugefügt
// ? UI wird automatisch aktualisiert

// Model entfernen
collectionViewModel.ModelStore.Remove(customer);
// ? TransformTo entfernt automatisch CustomerViewModel
// ? ViewModel wird disposed
// ? UI wird automatisch aktualisiert
```

### 3. ToReadOnlyObservableCollection (Items-Sync)

**ViewModelStore ? Items (ReadOnlyObservableCollection):**
```csharp
// Internal: Wird im Constructor erstellt
_itemsSync = _viewModelStore.ToReadOnlyObservableCollection(
    comparer: services.ComparerService.GetComparer<TViewModel>());

Items = _itemsSync.Collection;

// ? Items ist automatisch synchronisiert mit ViewModelStore!
```

**UI-Binding:**
```xml
<ListBox ItemsSource="{Binding Customers}"/>
<!-- Bindet an ReadOnlyObservableCollection<CustomerViewModel> -->
<!-- Alle Änderungen am ModelStore werden automatisch angezeigt! -->
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
```

## Verwendung

### Grundlegende Verwendung

```csharp
using CustomWPFControls.ViewModels;
using CustomWPFControls.Services;

public class MainViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public MainViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> viewModelFactory)
    {
        // ? Erstellt CollectionViewModel mit lokalem Store
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services,
            viewModelFactory);
    }
    
    // UI bindet an Items
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    // UI bindet an SelectedItem
    public CustomerViewModel? SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    // Daten-Operationen via ModelStore
    public void AddCustomer(string name, string email)
    {
        var customer = new Customer 
        { 
            Name = name, 
            Email = email 
        };
        
        // ? Fügt zum lokalen ModelStore hinzu
        _customers.ModelStore.Add(customer);
        // ? TransformTo erstellt automatisch CustomerViewModel
        // ? UI wird automatisch aktualisiert
    }
    
    public void DeleteSelectedCustomer()
    {
        if (_customers.SelectedItem != null)
        {
            // ? Remove-API der CollectionViewModel
            _customers.Remove(_customers.SelectedItem);
            // ? Entfernt aus ModelStore
            // ? ViewModel wird disposed
            // ? SelectedItem = null
        }
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

### Mit DI-Registrierung

```csharp
using CustomWPFControls.Services;
using Microsoft.Extensions.DependencyInjection;
using DataStores.Bootstrap;
using Common.Bootstrap;

public class MyAppServiceModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // 1. DataStores Core-Services
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        // 2. CustomWPFControls Services (Facade + ViewModelPackage)
        services.AddCustomWPFControls<Customer, CustomerViewModel>();
        
        // 3. ViewModel (erstellt CollectionViewModel im Constructor)
        services.AddTransient<MainViewModel>();
    }
}
```

### XAML-Binding

```xml
<Window>
    <Grid>
        <!-- ListBox bindet an Items -->
        <ListBox ItemsSource="{Binding Customers}"
                 SelectedItem="{Binding SelectedCustomer}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Name}" FontWeight="Bold"/>
                        <TextBlock Text="{Binding Email}" Foreground="Gray"/>
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        
        <!-- Details für SelectedCustomer -->
        <StackPanel DataContext="{Binding SelectedCustomer}">
            <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"/>
            <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"/>
        </StackPanel>
        
        <!-- Buttons für Daten-Operationen -->
        <StackPanel Orientation="Horizontal">
            <Button Content="Hinzufügen" Command="{Binding AddCommand}"/>
            <Button Content="Löschen" Command="{Binding DeleteCommand}"/>
        </StackPanel>
    </Grid>
</Window>
```

## Lokaler ModelStore

### Warum lokaler Store?

**? Vorteile:**
- **Isolation:** Jede CollectionViewModel-Instanz hat ihre eigenen Daten
- **Testbarkeit:** Tests können isoliert laufen ohne globalen State
- **Flexibilität:** Mehrere Listen desselben Typs möglich
- **Klarheit:** Datenquelle ist explizit (ModelStore Property)

**? NICHT wie früher (globaler Store):**
```csharp
// ? Alt: Globaler Store, Shared State
var dataStore = dataStores.GetGlobal<Customer>();
var collectionVM = new CollectionViewModel<Customer, CustomerViewModel>(
    dataStore, factory, comparer);
// Problem: Alle Instanzen teilen denselben Store!
```

**? NEU: Lokaler Store, Isolation:**
```csharp
// ? Neu: Lokaler Store wird intern erstellt
var collectionVM = new CollectionViewModel<Customer, CustomerViewModel>(
    services, viewModelFactory);
    
// ? Zugriff via ModelStore Property
collectionVM.ModelStore.Add(customer);
```

### ModelStore Property

**Public Readonly Access:**
```csharp
public IDataStore<TModel> ModelStore { get; }
// ? Readonly Property - kann nicht ersetzt werden
// ? Store wird im Constructor erstellt via services.DataStores.CreateLocal<TModel>()
// ? Alle DataStore-Operations verfügbar
```

**Usage:**
```csharp
// Hinzufügen
collectionVM.ModelStore.Add(customer);
collectionVM.ModelStore.AddRange(customers);

// Entfernen (direkt via Store)
collectionVM.ModelStore.Remove(customer);
collectionVM.ModelStore.Clear();

// Oder via CollectionViewModel Remove-API
collectionVM.Remove(viewModel);  // Entfernt Model UND disposed ViewModel
collectionVM.Clear();  // Leert Store UND invalidiert Selection
```

## TransformTo-Integration

### Architektur-Überblick

```
???????????????????????????????????????????????
?  ModelStore (local)                         ?
?  Created via services.DataStores.Create..() ?
?  IDataStore<TModel>                         ?
???????????????????????????????????????????????
                  ?
           TransformTo
         (automatische Sync)
                  ?
                  ?
???????????????????????????????????????????????
?  ViewModelStore (internal)                  ?
?  IDataStore<TViewModel>                     ?
???????????????????????????????????????????????
                  ?
  ToReadOnlyObservableCollection
      (automatische Sync)
                  ?
                  ?
???????????????????????????????????????????????
?  Items (public)                             ?
?  ReadOnlyObservableCollection<TViewModel>   ?
???????????????????????????????????????????????
                  ?
             UI Binding
                  ?
                  ?
          ????????????????
          ?   ListView   ?
          ????????????????
```

### TransformTo Details

**Constructor-Setup:**
```csharp
public CollectionViewModel(
    ICustomWPFServices services,
    IViewModelFactory<TModel, TViewModel> viewModelFactory)
{
    // 1. Lokalen ModelStore erstellen
    _modelStore = services.DataStores.CreateLocal<TModel>();
    
    // 2. Internen ViewModelStore erstellen
    _viewModelStore = services.DataStores.CreateLocal<TViewModel>();
    
    // 3. TransformTo einrichten (bidirektionale Sync)
    _unidirectionalSync = _modelStore.TransformTo<TModel, TViewModel>(
        _viewModelStore,
        factoryFunc: model => _viewModelFactory.Create(model),
        comparerFunc: (m, vm) => _modelComparer.Equals(m, vm.Model));
    
    // 4. Items-Collection einrichten (UI-Binding)
    _itemsSync = _viewModelStore.ToReadOnlyObservableCollection(
        comparer: services.ComparerService.GetComparer<TViewModel>());
    
    Items = _itemsSync.Collection;
}
```

## ViewModel-Lifecycle

### Lifecycle-Phasen

```
????????????????????????????????????????????????????????
? 1. CREATION                                          ?
?    ModelStore.Add(model)                             ?
?    ?                                                 ?
?    TransformTo: ViewModelFactory.Create(model)       ?
?    ?                                                 ?
?    ViewModel wird zu ViewModelStore hinzugefügt      ?
?    ?                                                 ?
?    ToReadOnlyObservableCollection: Items aktualisiert?
????????????????????????????????????????????????????????
                         ?
????????????????????????????????????????????????????????
? 2. ACTIVE                                            ?
?    ViewModel ist in Items                            ?
?    UI kann binden                                    ?
????????????????????????????????????????????????????????
                         ?
????????????????????????????????????????????????????????
? 3. REMOVAL                                           ?
?    ModelStore.Remove(model) ODER Remove(viewModel)   ?
?    ?                                                 ?
?    TransformTo: ViewModel wird entfernt              ?
?    ?                                                 ?
?    viewModel.Dispose() [falls IDisposable]           ?
????????????????????????????????????????????????????????
```

### Dispose-Pattern

```csharp
public void Dispose()
{
    if (_disposed) return;
    _disposed = true;

    // 1. Store-Events abmelden
    _viewModelStore.Changed -= OnViewModelStoreChanged;

    // 2. ModelStore leeren (disposed ViewModels via TransformTo)
    _modelStore.Clear();

    // 3. TransformTo-Sync disposed
    _unidirectionalSync?.Dispose();

    // 4. ObservableCollection-Sync disposed
    _itemsSync?.Dispose();

    // 5. Stores disposed
    if (_viewModelStore is IDisposable disposable)
    {
        disposable.Dispose();
    }
    
    _selectedItems.Clear();
}
```

## Best Practices

### ? Do's

**1. ModelStore für Daten-Operationen:**
```csharp
// ? Gut: Via ModelStore
collectionVM.ModelStore.Add(customer);
collectionVM.ModelStore.AddRange(customers);

// ? Gut: Via Remove-API (disposed ViewModel)
collectionVM.Remove(viewModel);
```

**2. CustomWPFServices Facade verwenden:**
```csharp
// ? Gut: Services Facade
services.AddCustomWPFControls<Customer, CustomerViewModel>();
var collectionVM = serviceProvider.GetRequiredService<
    CollectionViewModel<Customer, CustomerViewModel>>();
```

**3. Dispose aufrufen:**
```csharp
public class MainViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public void Dispose()
    {
        _customers.Dispose();  // ? Wichtig!
    }
}
```

### ? Don'ts

**1. Keine direkte Items-Mutation:**
```csharp
// ? Schlecht: ReadOnlyObservableCollection ist read-only!
collectionVM.Items.Add(viewModel);  // Compile-Error!

// ? Gut: Via ModelStore
collectionVM.ModelStore.Add(model);
```

**2. Kein globaler Store mehr:**
```csharp
// ? Alt: Globaler Store (veraltet)
var globalStore = dataStores.GetGlobal<Customer>();

// ? Neu: Lokaler Store via ModelStore Property
collectionVM.ModelStore.Add(customer);
```

## Beispiele

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
```

### Beispiel 2: Mehrere isolierte Listen

```csharp
public class MultiListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _activeCustomers;
    private readonly CollectionViewModel<Customer, CustomerViewModel> _inactiveCustomers;
    
    public MultiListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        // ? Beide Instanzen sind komplett isoliert!
        _activeCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _inactiveCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> ActiveCustomers 
        => _activeCustomers.Items;
    
    public ReadOnlyObservableCollection<CustomerViewModel> InactiveCustomers 
        => _inactiveCustomers.Items;
    
    public void MoveToInactive(CustomerViewModel customer)
    {
        // ? Entfernen aus aktiven, hinzufügen zu inaktiven
        _activeCustomers.Remove(customer);
        _inactiveCustomers.ModelStore.Add(customer.Model);
    }
    
    public void Dispose()
    {
        _activeCustomers.Dispose();
        _inactiveCustomers.Dispose();
    }
}
