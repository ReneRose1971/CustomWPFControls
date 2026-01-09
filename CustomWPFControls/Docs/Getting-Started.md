# Getting Started - CustomWPFControls

Setup-Anleitung für CustomWPFControls.

## Installation

### NuGet-Pakete

```bash
dotnet add package PropertyChanged.Fody
dotnet add package Fody
dotnet add package Microsoft.Extensions.DependencyInjection
```

### FodyWeavers.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<Weavers xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <PropertyChanged FilterType="Explicit" InjectOnPropertyNameChanged="false" />
</Weavers>
```

## Minimal-Setup

### Model

```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}
```

### ViewModel

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    public string Name => Model.Name;
    public bool IsSelected { get; set; }
}
```

### DI-Registrierung

```csharp
public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        services.AddViewModelFactory<Customer, CustomerViewModel>();
        services.AddSingleton<IEqualityComparer<Customer>>(
            new FallbackEqualsComparer<Customer>());
    }
}
```

### App.xaml.cs Bootstrap

```csharp
protected override async void OnStartup(StartupEventArgs e)
{
    _host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddModulesFromAssemblies(typeof(App).Assembly);
            services.AddSingleton<MainWindow>();
        })
        .Build();

    await DataStoreBootstrap.RunAsync(_host.Services);
    
    var mainWindow = _host.Services.GetRequiredService<MainWindow>();
    mainWindow.Show();
}
```

## Voraussetzungen

- `DataStoreBootstrap.RunAsync()` muss nach `Build()` aufgerufen werden
- ViewModel-Constructor muss TModel als ersten Parameter haben
- IEqualityComparer für TModel muss registriert sein

## Siehe auch

- [Architecture](Architecture.md)
- [API Reference](API-Reference.md)
- [ViewModelBase](ViewModelBase.md)
