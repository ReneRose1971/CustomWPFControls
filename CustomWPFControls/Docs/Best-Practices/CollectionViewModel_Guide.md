# CollectionViewModel - Best Practices Guide

Umfassender Leitfaden für die Verwendung von `CollectionViewModel<TModel, TViewModel>` in WPF-Anwendungen.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [Grundlegende Verwendung](#grundlegende-verwendung)
- [DI-Registrierung mit AddViewModelPackage](#di-registrierung-mit-addviewmodelpackage)
- [ModelStore-Konzept](#modelstore-konzept)
- [LoadData Extension](#loaddata-extension)
- [TransformTo-Integration](#transformto-integration)
- [Selection-Management](#selection-management)
- [Praktische Beispiele](#praktische-beispiele)

---

## Überblick

`CollectionViewModel<TModel, TViewModel>` ist die zentrale Klasse für Collection-Verwaltung in CustomWPFControls. Sie bietet:

- **Lokaler ModelStore**: Jede Instanz hat ihren eigenen isolierten DataStore
- **Automatische Synchronisation**: Via TransformTo und ToReadOnlyObservableCollection
- **ViewModel-Lifecycle**: Automatische Erstellung und Dispose von ViewModels
- **Selection-Tracking**: SelectedItem und SelectedItems für UI-Binding
- **Remove-API**: Remove(), RemoveRange(), Clear() mit automatischer Invalidierung

---

## Grundlegende Verwendung

### ViewModel-Struktur

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    // Domain-Properties (delegiert an Model)
    public string Name => Model.Name;
    public string Email => Model.Email;
    
    // UI-Properties (mit PropertyChanged via Fody)
    public bool IsSelected { get; set; }
}
```

### Container-ViewModel

```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> viewModelFactory)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services,
            viewModelFactory);
    }
    
    // UI-Binding Properties
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public CustomerViewModel? SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

### XAML-Binding

```xml
<Window DataContext="{Binding CustomerListViewModel}">
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
</Window>
```

---

## DI-Registrierung mit AddViewModelPackage

### Was ist AddViewModelPackage?

`AddViewModelPackage` ist eine Extension-Methode, die alle benötigten Services für ein Model-ViewModel-Paar in einem Aufruf registriert.

### Registrierte Services

Die Extension registriert folgende Services:

1. **IViewModelFactory<TModel, TViewModel>** als Singleton
2. **CollectionViewModel<TModel, TViewModel>** als Transient
3. **EditableCollectionViewModel<TModel, TViewModel>** als Transient

### Verwendung

```csharp
using CustomWPFControls.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // 1. DataStores Core Services
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        // 2. CustomWPFControls Core Services
        services.AddSingleton<ICustomWPFServices, CustomWPFServices>();
        
        // 3. ViewModel Package für Customer
        services.AddViewModelPackage<Customer, CustomerViewModel>();
        
        // 4. EqualityComparer
        services.AddSingleton<IEqualityComparer<Customer>>(
            new FallbackEqualsComparer<Customer>());
        
        // 5. Container-ViewModel
        services.AddTransient<CustomerListViewModel>();
    }
}
```

### Service-Auflösung

```csharp
// ViewModelFactory auflösen
var factory = serviceProvider.GetRequiredService<
    IViewModelFactory<Customer, CustomerViewModel>>();

// CollectionViewModel auflösen (neue Instanz bei jedem Aufruf)
var collectionVM = serviceProvider.GetRequiredService<
    CollectionViewModel<Customer, CustomerViewModel>>();

// EditableCollectionViewModel auflösen
var editableVM = serviceProvider.GetRequiredService<
    EditableCollectionViewModel<Customer, CustomerViewModel>>();
```

### Warum Transient für ViewModels?

CollectionViewModel und EditableCollectionViewModel werden als **Transient** registriert, weil:

- Jede View-Instanz benötigt ihre eigene ViewModel-Instanz
- Jede CollectionViewModel hat ihren eigenen lokalen ModelStore
- Mehrere Views können unabhängig voneinander dieselben Typen verwenden
- Vermeidung von Shared State zwischen Views

---

## ModelStore-Konzept

### Lokaler vs. Globaler Store

Jede `CollectionViewModel`-Instanz erstellt intern einen **lokalen ModelStore**:

```csharp
public class CollectionViewModel<TModel, TViewModel>
{
    public IDataStore<TModel> ModelStore { get; }
    
    public CollectionViewModel(
        ICustomWPFServices services,
        IViewModelFactory<TModel, TViewModel> viewModelFactory)
    {
        // Lokaler Store wird intern erstellt
        _modelStore = services.DataStores.CreateLocal<TModel>();
        
        // Zugriff via readonly Property
        ModelStore = _modelStore;
    }
}
```

### Daten-Operationen

Alle Daten-Operationen erfolgen über den ModelStore:

```csharp
// Einzelnes Item hinzufügen
collectionViewModel.ModelStore.Add(customer);

// Mehrere Items hinzufügen
collectionViewModel.ModelStore.AddRange(customers);

// Item entfernen
collectionViewModel.ModelStore.Remove(customer);

// Alle Items entfernen
collectionViewModel.ModelStore.Clear();
```

### Isolation zwischen Instanzen

```csharp
var activeCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
    services, factory);
    
var inactiveCustomers = new CollectionViewModel<Customer, CustomerViewModel>(
    services, factory);

// Beide Instanzen sind komplett isoliert
activeCustomers.ModelStore.Add(customer1);
inactiveCustomers.ModelStore.Add(customer2);

// activeCustomers.Items.Count = 1
// inactiveCustomers.Items.Count = 1
```

---

## LoadData Extension

### Zweck

Die `LoadData` Extension-Methode vereinfacht das Laden von Daten in den ModelStore und bietet optionale automatische Selektion.

### Signatur

```csharp
public static void LoadData<TModel, TViewModel>(
    this CollectionViewModel<TModel, TViewModel> collectionViewModel,
    IEnumerable<TModel>? data,
    bool selectFirst = true)
```

### Funktionalität

Die Extension:

1. Leert den aktuellen ModelStore (Clear)
2. Fügt die neuen Daten hinzu (AddRange)
3. Selektiert optional das erste Item (wenn `selectFirst = true`)

### Verwendung

```csharp
// Standard: Daten laden und erstes Item selektieren
collectionViewModel.LoadData(customers);

// Daten laden ohne automatische Selektion
collectionViewModel.LoadData(customers, selectFirst: false);

// Leere Daten sind valide (z.B. nach Filter ohne Treffer)
collectionViewModel.LoadData(Enumerable.Empty<Customer>());

// Null wird als leere Collection behandelt
collectionViewModel.LoadData(null);
```

### Praktisches Beispiel

```csharp
public class CustomerListViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly ICustomerRepository _repository;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        ICustomerRepository repository)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        _repository = repository;
    }
    
    public async Task LoadAllCustomersAsync()
    {
        var customers = await _repository.GetAllAsync();
        _customers.LoadData(customers);
    }
    
    public async Task LoadActiveCustomersAsync()
    {
        var activeCustomers = await _repository.GetActiveAsync();
        _customers.LoadData(activeCustomers, selectFirst: false);
    }
    
    public void ClearCustomers()
    {
        _customers.LoadData(null);
    }
}
```

### Selektionsverhalten

Bei `selectFirst = true`:

```csharp
// Szenario 1: Items vorhanden
collectionViewModel.LoadData(customers); // customers.Count > 0
// ? SelectedItem = Items[0]

// Szenario 2: Keine Items
collectionViewModel.LoadData(Enumerable.Empty<Customer>());
// ? SelectedItem = null

// Szenario 3: Null-Daten
collectionViewModel.LoadData(null);
// ? SelectedItem = null
```

---

## TransformTo-Integration

### Architektur-Überblick

```
ModelStore (local)
       ?
  TransformTo (automatische Sync)
       ?
ViewModelStore (internal)
       ?
ToReadOnlyObservableCollection
       ?
Items (ReadOnlyObservableCollection)
       ?
  UI-Binding (ListView)
```

### Automatische ViewModel-Erstellung

```csharp
// Im Constructor von CollectionViewModel:
_unidirectionalSync = _modelStore.TransformTo<TModel, TViewModel>(
    _viewModelStore,
    factoryFunc: model => _viewModelFactory.Create(model),
    comparerFunc: (m, vm) => _modelComparer.Equals(m, vm.Model));
```

### Lifecycle-Management

```csharp
// Model hinzufügen
collectionViewModel.ModelStore.Add(customer);
// ? TransformTo erstellt automatisch CustomerViewModel
// ? ViewModel wird zu ViewModelStore hinzugefügt
// ? ToReadOnlyObservableCollection synchronisiert Items
// ? UI wird automatisch aktualisiert

// Model entfernen
collectionViewModel.ModelStore.Remove(customer);
// ? TransformTo findet zugehöriges ViewModel
// ? ViewModel wird aus ViewModelStore entfernt
// ? ViewModel.Dispose() wird aufgerufen (falls IDisposable)
// ? Items wird automatisch aktualisiert
// ? UI wird automatisch aktualisiert
```

---

## Selection-Management

### SelectedItem

```csharp
// Single-Selection
public CustomerViewModel? SelectedCustomer
{
    get => _customers.SelectedItem;
    set => _customers.SelectedItem = value;
}
```

```xml
<ListBox ItemsSource="{Binding Customers}"
         SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"/>
```

### SelectedItems mit MultiSelectBehavior

Für Multi-Selection benötigen Sie das `MultiSelectBehavior`:

```csharp
public ObservableCollection<CustomerViewModel> SelectedCustomers
    => _customers.SelectedItems;
```

```xml
<ListBox SelectionMode="Multiple"
         ItemsSource="{Binding Customers}"
         behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedCustomers}"/>
```

**Hinweis**: Das MultiSelectBehavior ist erforderlich, weil `ListBox.SelectedItems` in WPF readonly ist und nicht direkt gebunden werden kann.

### Automatische Invalidierung

```csharp
// SelectedItem wird automatisch invalidiert bei Remove
collectionViewModel.SelectedItem = viewModel1;
collectionViewModel.Remove(viewModel1);
// ? SelectedItem = null

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

---

## Praktische Beispiele

### Beispiel 1: Einfache Kundenliste

```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly ICustomerRepository _repository;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> viewModelFactory,
        ICustomerRepository repository)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, viewModelFactory);
        _repository = repository;
        
        LoadCustomersAsync().ConfigureAwait(false);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public CustomerViewModel? SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    private async Task LoadCustomersAsync()
    {
        var customers = await _repository.GetAllAsync();
        _customers.LoadData(customers);
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

### Beispiel 2: Master-Detail mit zwei Listen

```csharp
public class MasterDetailViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly CollectionViewModel<Order, OrderViewModel> _orders;
    private readonly IOrderRepository _orderRepository;
    
    public MasterDetailViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> customerFactory,
        IViewModelFactory<Order, OrderViewModel> orderFactory,
        IOrderRepository orderRepository)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, customerFactory);
            
        _orders = new CollectionViewModel<Order, OrderViewModel>(
            services, orderFactory);
            
        _orderRepository = orderRepository;
        
        // Bei Kundenwechsel Orders laden
        _customers.PropertyChanged += OnCustomerSelectionChanged;
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public CustomerViewModel? SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    public ReadOnlyObservableCollection<OrderViewModel> Orders 
        => _orders.Items;
    
    private async void OnCustomerSelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_customers.SelectedItem) && _customers.SelectedItem != null)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(
                _customers.SelectedItem.Id);
            _orders.LoadData(orders);
        }
    }
    
    public void Dispose()
    {
        _customers.PropertyChanged -= OnCustomerSelectionChanged;
        _customers.Dispose();
        _orders.Dispose();
    }
}
```

### Beispiel 3: Filter-Funktionalität

```csharp
public class FilterableCustomerListViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly ICustomerRepository _repository;
    private ICollectionView? _customersView;
    
    public FilterableCustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> viewModelFactory,
        ICustomerRepository repository)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, viewModelFactory);
        _repository = repository;
        
        LoadCustomersAsync().ConfigureAwait(false);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public string SearchText { get; set; } = "";
    
    private async Task LoadCustomersAsync()
    {
        var customers = await _repository.GetAllAsync();
        _customers.LoadData(customers);
        
        // ICollectionView für Filtering erstellen
        _customersView = CollectionViewSource.GetDefaultView(_customers.Items);
        _customersView.Filter = FilterCustomer;
    }
    
    private bool FilterCustomer(object obj)
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return true;
            
        var customer = (CustomerViewModel)obj;
        return customer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
               customer.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
    }
    
    public void ApplyFilter()
    {
        _customersView?.Refresh();
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

### Beispiel 4: Lazy Loading mit LoadData

```csharp
public class LazyLoadingViewModel : IDisposable
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;
    private readonly ICustomerRepository _repository;
    private int _currentPage = 0;
    private const int PageSize = 50;
    
    public LazyLoadingViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> viewModelFactory,
        ICustomerRepository repository)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            services, viewModelFactory);
        _repository = repository;
        
        LoadNextPageCommand = new RelayCommand(
            _ => LoadNextPageAsync().ConfigureAwait(false),
            _ => CanLoadMore);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public ICommand LoadNextPageCommand { get; }
    public bool CanLoadMore { get; private set; } = true;
    
    private async Task LoadNextPageAsync()
    {
        var customers = await _repository.GetPageAsync(_currentPage, PageSize);
        
        if (customers.Count < PageSize)
            CanLoadMore = false;
        
        // Erste Seite: LoadData verwenden
        if (_currentPage == 0)
        {
            _customers.LoadData(customers);
        }
        // Weitere Seiten: Direkt an ModelStore anhängen
        else
        {
            _customers.ModelStore.AddRange(customers);
        }
        
        _currentPage++;
    }
    
    public void Dispose()
    {
        _customers.Dispose();
    }
}
```

---

## Zusammenfassung

### Wichtigste Punkte

1. **AddViewModelPackage**: Registriert alle benötigten Services in einem Aufruf
2. **Lokaler ModelStore**: Jede CollectionViewModel-Instanz ist isoliert
3. **LoadData Extension**: Vereinfacht das Laden und Ersetzen von Daten
4. **TransformTo**: Automatische ViewModel-Erstellung und Synchronisation
5. **Selection-Management**: Automatische Invalidierung bei Remove-Operationen
6. **MultiSelectBehavior**: Erforderlich für Multi-Selection in ListBox

### Weiterführende Dokumentation

- [EditableCollectionViewModel Guide](EditableCollectionViewModel_Guide.md) - Commands und CRUD-Operationen
- [Custom Controls Guide](CustomControls_Guide.md) - ListEditorView und DropDownEditorView
- [API Reference](../API-Reference.md) - Vollständige API-Dokumentation
- [Architecture](../Architecture.md) - Architektur-Überblick
