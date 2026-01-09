# ViewModelFactory<TModel, TViewModel>

DI-basierte Factory für ViewModel-Erstellung mit automatischer Service-Injektion.

## Übersicht

`ViewModelFactory<TModel, TViewModel>` erstellt ViewModels via ActivatorUtilities und injiziert Services automatisch.

### Definition

```csharp
public sealed class ViewModelFactory<TModel, TViewModel> : IViewModelFactory<TModel, TViewModel>
    where TModel : class
    where TViewModel : class
{
    public ViewModelFactory(IServiceProvider serviceProvider);
    public TViewModel Create(TModel model);
}
```

## Contract

- Erstellt ViewModels via ActivatorUtilities.CreateInstance
- Injiziert Services automatisch aus DI-Container
- TModel wird als erster Constructor-Parameter übergeben
- Weitere Dependencies werden aus ServiceProvider aufgelöst

## Verwendung

### DI-Registrierung

```csharp
public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        services.AddViewModelFactory<Customer, CustomerViewModel>();
        
        // Registriert:
        // - IViewModelFactory<Customer, CustomerViewModel>
        // - ViewModelFactory<Customer, CustomerViewModel>
    }
}
```

### ViewModel-Constructor

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    private readonly IMessageService _messageService;
    
    // Model als erster Parameter, weitere Dependencies aus DI
    public CustomerViewModel(
        Customer model,
        IMessageService messageService) : base(model)
    {
        _messageService = messageService;
    }
}
```

### Factory verwenden

```csharp
var factory = serviceProvider.GetRequiredService<IViewModelFactory<Customer, CustomerViewModel>>();

var customer = new Customer { Name = "Alice" };
var viewModel = factory.Create(customer);

// IMessageService wurde automatisch injiziert
```

## ActivatorUtilities

Factory verwendet ActivatorUtilities.CreateInstance für ViewModel-Erstellung:

```csharp
public TViewModel Create(TModel model)
{
    return ActivatorUtilities.CreateInstance<TViewModel>(
        _serviceProvider, 
        model);
}
```

**Auflösung:**
1. TModel wird direkt als Parameter übergeben
2. Weitere Constructor-Parameter werden aus ServiceProvider aufgelöst
3. Nicht registrierte Services führen zu Exception

## Voraussetzungen

- ViewModel-Constructor muss TModel als ersten Parameter haben
- Alle anderen Dependencies müssen im DI-Container registriert sein
- ViewModel-Typ muss public Constructor haben

## Siehe auch

- [ViewModelBase](ViewModelBase.md)
- [CollectionViewModel](CollectionViewModel.md)
- [API Reference](API-Reference.md)
