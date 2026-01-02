# IViewModelFactory - Vollständige Dokumentation

## ?? Inhaltsverzeichnis

- [Übersicht](#übersicht)
- [Konzept & Architektur](#konzept--architektur)
- [Registrierung](#registrierung)
- [Initialisierung](#initialisierung)
- [Verwendung](#verwendung)
- [ViewModelBase mit IServiceProvider](#viewmodelbase-mit-iserviceprovider)
- [Erweiterte Szenarien](#erweiterte-szenarien)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

---

## Übersicht

**`IViewModelFactory<TModel, TViewModel>`** ist die zentrale Factory-Schnittstelle für die **DI-basierte Erstellung von ViewModels** in CustomWPFControls.

### Kernfunktionen

1. ? **DI-Integration** – ViewModels werden via Dependency Injection erstellt
2. ? **Automatische Parameter-Auflösung** – Constructor-Dependencies werden automatisch injiziert
3. ? **IServiceProvider-Support** – Optional verfügbar in ViewModelBase
4. ? **Typsicherheit** – Generische Typen garantieren korrekte Model-ViewModel-Zuordnung
5. ? **Singleton-Pattern** – Eine Factory pro Model-ViewModel-Paar

---

## Konzept & Architektur

### Interface-Definition

```csharp
namespace CustomWPFControls.Factories
{
    public interface IViewModelFactory<in TModel, out TViewModel>
        where TModel : class
        where TViewModel : class
    {
        TViewModel Create(TModel model);
    }
}
```

### Implementierung: ViewModelFactory

```csharp
public sealed class ViewModelFactory<TModel, TViewModel> : IViewModelFactory<TModel, TViewModel>
    where TModel : class
    where TViewModel : class
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public TViewModel Create(TModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        try
        {
            // ActivatorUtilities erstellt das ViewModel:
            // - Übergibt 'model' als expliziten Parameter
            // - Löst alle anderen Constructor-Parameter via DI auf (inkl. IServiceProvider)
            return ActivatorUtilities.CreateInstance<TViewModel>(_serviceProvider, model);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Fehler beim Erstellen von {typeof(TViewModel).Name}. " +
                $"Stellen Sie sicher, dass der ViewModel-Constructor eine der unterstützten Signaturen hat.",
                ex);
        }
    }
}
```

### Unterstützte Constructor-Signaturen

**ViewModelFactory unterstützt automatisch:**

```csharp
// 1. Nur Model (minimal)
public CustomerViewModel(Customer model)
    : base(model) { }

// 2. Model + IServiceProvider
public CustomerViewModel(Customer model, IServiceProvider serviceProvider)
    : base(model, serviceProvider) { }

// 3. Model + Services (empfohlen)
public CustomerViewModel(Customer model, IMessageService messageService)
    : base(model) { }

// 4. Model + IServiceProvider + Services
public CustomerViewModel(
    Customer model, 
    IServiceProvider serviceProvider,
    IMessageService messageService)
    : base(model, serviceProvider) { }
```

**ActivatorUtilities wählt automatisch den besten Constructor!**

### Wie `ActivatorUtilities` funktioniert

**`ActivatorUtilities.CreateInstance<T>(IServiceProvider, params object[])`** macht folgendes:

1. **Konstruktor-Scanning** – Findet alle öffentlichen Constructors
2. **Bester Match** – Wählt Constructor mit den meisten auflösbaren Parametern
3. **Parameter-Auflösung:**
   - Explizite Parameter verwenden (z.B. `model`)
   - Fehlende Parameter via DI auflösen (inkl. `IServiceProvider`)
4. **Instanz erstellen**

#### Beispiel:

```csharp
// ViewModel-Constructor:
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;

    public CustomerViewModel(
        Customer model,                      // ? Parameter 1: Explizit von Factory
        IServiceProvider serviceProvider,    // ? Parameter 2: Automatisch aufgelöst
        IMessageService messageService)      // ? Parameter 3: Aus DI
        : base(model, serviceProvider)
    {
        _messageService = messageService;
    }
}

// Factory.Create():
factory.Create(customer);

// Intern passiert:
// 1. ActivatorUtilities.CreateInstance<CustomerViewModel>(_serviceProvider, customer)
// 2. Findet Constructor mit 3 Parametern
// 3. Parameter 1 (Customer): customer (explizit übergeben)
// 4. Parameter 2 (IServiceProvider): _serviceProvider (automatisch)
// 5. Parameter 3 (IMessageService): _serviceProvider.GetService<IMessageService>()
// 6. Erstellt: new CustomerViewModel(customer, serviceProvider, messageService)
```

---

## Registrierung

### 1. Extension-Method: `AddViewModelFactory`

Die **empfohlene Methode** zur Registrierung:

```csharp
using CustomWPFControls.Factories;
using Microsoft.Extensions.DependencyInjection;

services.AddViewModelFactory<Customer, CustomerViewModel>();
```

**Eigenschaften:**
- ? Verwendet `TryAddSingleton` ? idempotent (keine Duplikate)
- ? Registriert als **Singleton** ? eine Factory pro Model-ViewModel-Paar
- ? Generisch typsicher

### 2. Im ServiceModule-Pattern

```csharp
using Common.Bootstrap;
using CustomWPFControls.Factories;

public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // Factories registrieren
        services.AddViewModelFactory<Customer, CustomerViewModel>();
        services.AddViewModelFactory<Order, OrderViewModel>();
        services.AddViewModelFactory<Product, ProductViewModel>();
        
        // ViewModels
        services.AddTransient<CustomerListViewModel>();
    }
}
```

### 3. Vollständiges Beispiel: Startup-Konfiguration

```csharp
using Common.Bootstrap;
using CustomWPFControls.Factories;
using DataStores.Bootstrap;

var builder = Host.CreateApplicationBuilder(args);

// 1. DataStores Core Services
new DataStoresServiceModule().Register(builder.Services);

// 2. Application-Services
builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddSingleton<IValidationService, ValidationService>();

// 3. ViewModelFactories
builder.Services.AddViewModelFactory<Customer, CustomerViewModel>();

// 4. ViewModels
builder.Services.AddTransient<MainViewModel>();

var app = builder.Build();
await app.RunAsync();
```

---

## Initialisierung

Die Factory wird **automatisch** beim ersten Abruf initialisiert:

```csharp
public class CustomerListViewModel
{
    private readonly IViewModelFactory<Customer, CustomerViewModel> _factory;

    public CustomerListViewModel(
        IViewModelFactory<Customer, CustomerViewModel> factory)  // ? DI injiziert
    {
        _factory = factory;
    }
}
```

**Lifecycle:**
1. **Registrierung** (Startup): `services.AddViewModelFactory<>()`
2. **Build**: `serviceProvider = services.BuildServiceProvider()`
3. **Auflösung** (Runtime): DI erstellt Factory beim ersten Zugriff
4. **Wiederverwendung**: Singleton ? eine Instanz für alle

---

## Verwendung

### 1. Einfaches Beispiel

```csharp
public class CustomerService
{
    private readonly IViewModelFactory<Customer, CustomerViewModel> _factory;

    public CustomerService(IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _factory = factory;
    }

    public CustomerViewModel GetViewModel(int customerId)
    {
        var customer = LoadCustomerById(customerId);
        return _factory.Create(customer);  // ? Services automatisch injiziert
    }
}
```

### 2. CollectionViewModel-Integration

```csharp
public class CustomerListViewModel
{
    private readonly CollectionViewModel<Customer, CustomerViewModel> _customers;

    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            dataStores, factory, comparerService);
    }

    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
}
```

---

## ViewModelBase mit IServiceProvider

**Neu in v2.0:** `ViewModelBase<TModel>` bietet optionalen Zugriff auf `IServiceProvider`.

### Constructor-Varianten

```csharp
// Constructor 1: Nur Model (Abwärtskompatibel)
protected ViewModelBase(TModel model)

// Constructor 2: Model + ServiceProvider (NEU)
protected ViewModelBase(TModel model, IServiceProvider serviceProvider)
```

### Protected Property

```csharp
/// <summary>
/// ServiceProvider für abgeleitete ViewModels (optional).
/// Nur gesetzt, wenn Constructor mit IServiceProvider verwendet wird.
/// </summary>
protected IServiceProvider? ServiceProvider { get; }
```

### Verwendungsbeispiele

#### Variante 1: Einfaches ViewModel (ohne ServiceProvider)

```csharp
public class SimpleCustomerViewModel : ViewModelBase<Customer>
{
    public SimpleCustomerViewModel(Customer model) 
        : base(model) 
    {
    }

    public string Name => Model.Name;
    public string Email => Model.Email;
}
```

#### Variante 2: Mit ServiceProvider

```csharp
public class AdvancedCustomerViewModel : ViewModelBase<Customer>
{
    public AdvancedCustomerViewModel(
        Customer model, 
        IServiceProvider serviceProvider)
        : base(model, serviceProvider)
    {
    }

    public void LogAction()
    {
        // Zugriff auf optionale Services via ServiceProvider
        var logger = ServiceProvider?.GetService<ILogger>();
        logger?.LogInformation($"Action on {Model.Name}");
    }
}
```

#### Variante 3: Mit ServiceProvider + Services ? **Empfohlen**

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;

    public CustomerViewModel(
        Customer model, 
        IServiceProvider serviceProvider,
        IMessageService messageService)
        : base(model, serviceProvider)
    {
        _messageService = messageService;
    }

    public string Name => Model.Name;
    public bool IsSelected { get; set; }

    public void SendEmail()
    {
        // Erforderliche Services via Constructor-Injection
        _messageService.Send($"Email to {Model.Email}");
        
        // Optionale Services via ServiceProvider
        var logger = ServiceProvider?.GetService<ILogger>();
        logger?.LogInformation($"Email sent to {Model.Email}");
    }
}
```

#### Variante 4: Nur mit Services (ohne ServiceProvider)

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;

    public CustomerViewModel(
        Customer model,
        IMessageService messageService)
        : base(model)  // ? Verwendet Constructor ohne ServiceProvider
    {
        _messageService = messageService;
    }

    public void SendEmail()
    {
        _messageService.Send($"Email to {Model.Email}");
    }
}
```

### Wann welche Variante?

| Szenario | Empfohlene Variante | Begründung |
|----------|-------------------|------------|
| Keine Services benötigt | Variante 1 | Minimal, einfach |
| Nur optionale Services | Variante 2 | ServiceProvider für GetService<T>() |
| Erforderliche + optionale Services | Variante 3 ? | Best of Both Worlds |
| Nur erforderliche Services | Variante 4 | Explizite Dependencies |

---

## Erweiterte Szenarien

### 1. ViewModel mit zusätzlichen Dependencies

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;
    private readonly IValidationService _validationService;

    public CustomerViewModel(
        Customer model,
        IServiceProvider serviceProvider,
        IMessageService messageService,
        IValidationService validationService)
        : base(model, serviceProvider)
    {
        _messageService = messageService;
        _validationService = validationService;
    }

    public async Task SaveAsync()
    {
        if (!_validationService.Validate(Model))
        {
            await _messageService.ShowErrorAsync("Validation failed");
            return;
        }

        await _messageService.ShowSuccessAsync("Customer saved");
        
        // Optionale Services via ServiceProvider
        var analytics = ServiceProvider?.GetService<IAnalyticsService>();
        analytics?.Track("CustomerSaved");
    }
}
```

### 2. Lazy Service Resolution

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private ILogger? _logger;

    public CustomerViewModel(Customer model, IServiceProvider serviceProvider)
        : base(model, serviceProvider)
    {
    }

    private ILogger Logger => _logger ??= ServiceProvider?.GetService<ILogger>() 
                                          ?? NullLogger.Instance;

    public void DoSomething()
    {
        Logger.LogInformation("Action performed");
    }
}
```

### 3. Mehrere Factories in einem ViewModel

```csharp
public class MainViewModel
{
    public MainViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> customerFactory,
        IViewModelFactory<Order, OrderViewModel> orderFactory,
        IEqualityComparerService comparerService)
    {
        _customers = new CollectionViewModel<Customer, CustomerViewModel>(
            dataStores, customerFactory, comparerService);

        _orders = new CollectionViewModel<Order, OrderViewModel>(
            dataStores, orderFactory, comparerService);
    }
}
```

---

## Best Practices

### ? DO's

#### 1. Model als ersten Constructor-Parameter

```csharp
// ? RICHTIG
public CustomerViewModel(Customer model, IServiceProvider serviceProvider)

// ? FALSCH
public CustomerViewModel(IServiceProvider serviceProvider, Customer model)
```

#### 2. ServiceProvider für optionale Services

```csharp
// ? RICHTIG: Optionale Services via ServiceProvider
var analytics = ServiceProvider?.GetService<IAnalyticsService>();
analytics?.Track("Event");

// ? FALSCH: Erforderliche Services via ServiceProvider
var required = ServiceProvider.GetRequiredService<IMessageService>(); // Sollte Constructor-Parameter sein!
```

#### 3. Erforderliche Services via Constructor-Injection

```csharp
// ? RICHTIG
public CustomerViewModel(
    Customer model,
    IServiceProvider serviceProvider,
    IMessageService messageService)  // ? Erforderlich ? Constructor
{
    _messageService = messageService;
}

// ? FALSCH
public CustomerViewModel(Customer model, IServiceProvider serviceProvider)
{
    _messageService = serviceProvider.GetRequiredService<IMessageService>(); // Service Locator!
}
```

#### 4. Null-Check für ServiceProvider

```csharp
// ? RICHTIG
var logger = ServiceProvider?.GetService<ILogger>();
logger?.LogInformation("Message");

// ? FALSCH (wenn ServiceProvider optional ist)
var logger = ServiceProvider.GetService<ILogger>(); // NullReferenceException!
```

### ? DON'Ts

#### 1. Keine statischen Factories

```csharp
// ? FALSCH
public static class ViewModelFactory
{
    public static CustomerViewModel Create(Customer model)
    {
        return new CustomerViewModel(model, new MessageService());
    }
}
```

#### 2. Kein Service Locator Pattern

```csharp
// ? FALSCH
public CustomerViewModel(Customer model, IServiceProvider serviceProvider)
    : base(model, serviceProvider)
{
    _messageService = serviceProvider.GetRequiredService<IMessageService>();
    _validation = serviceProvider.GetRequiredService<IValidationService>();
    _logger = serviceProvider.GetRequiredService<ILogger>();
}

// ? RICHTIG
public CustomerViewModel(
    Customer model,
    IServiceProvider serviceProvider,
    IMessageService messageService,
    IValidationService validation,
    ILogger logger)
    : base(model, serviceProvider)
{
    _messageService = messageService;
    _validation = validation;
    _logger = logger;
}
```

---

## Troubleshooting

### Problem 1: `InvalidOperationException` beim Create

**Fehler:**
```
Fehler beim Erstellen von CustomerViewModel für Model Customer.
```

**Lösungen:**

1. **Model nicht als erster Parameter**
   ```csharp
   // ? FALSCH
   public CustomerViewModel(IServiceProvider sp, Customer model)
   
   // ? RICHTIG
   public CustomerViewModel(Customer model, IServiceProvider sp)
   ```

2. **Service nicht registriert**
   ```csharp
   services.AddSingleton<IMessageService, MessageService>();
   ```

3. **ServiceProvider wird erwartet, aber nicht übergeben**
   ```csharp
   // ViewModel erwartet IServiceProvider, aber base(model) statt base(model, serviceProvider)
   public CustomerViewModel(Customer model, IServiceProvider serviceProvider)
       : base(model)  // ? Fehler! Sollte base(model, serviceProvider) sein
   ```

### Problem 2: NullReferenceException bei ServiceProvider

**Fehler:**
```
NullReferenceException: Object reference not set to an instance of an object.
```

**Lösung:**

```csharp
// ? FALSCH (wenn Constructor ohne ServiceProvider verwendet wurde)
var logger = ServiceProvider.GetService<ILogger>();

// ? RICHTIG
var logger = ServiceProvider?.GetService<ILogger>();
logger?.LogInformation("Message");
```

### Problem 3: Factory wird nicht aufgelöst

**Fehler:**
```
Unable to resolve service for type 'IViewModelFactory<Customer, CustomerViewModel>'
```

**Lösung:**

```csharp
services.AddViewModelFactory<Customer, CustomerViewModel>();
```

---

## Zusammenfassung

### Wichtigste Punkte

1. **Factory erstellt ViewModels automatisch** via `ActivatorUtilities`
2. **Unterstützte Constructor-Signaturen:**
   - `(TModel model)`
   - `(TModel model, IServiceProvider serviceProvider)`
   - `(TModel model, IService1, IService2, ...)`
   - Alle Kombinationen
3. **ViewModelBase bietet optional `ServiceProvider`**
4. **ServiceProvider für optionale Services** – Erforderliche via Constructor
5. **Factory bleibt unverändert** – `ActivatorUtilities` löst automatisch auf

### Registrierung

```csharp
services.AddViewModelFactory<Customer, CustomerViewModel>();
```

### Verwendung

```csharp
var viewModel = factory.Create(customer);
```

### Siehe auch

- [CollectionViewModel](CollectionViewModel.md)
- [ViewModelBase](ViewModelBase.md)
- [Getting Started](Getting-Started.md)

---

**© 2024 CustomWPFControls** | [Zurück zur Übersicht](../README.md)
