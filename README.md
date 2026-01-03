# CustomWPFControls

Ein umfassendes MVVM-Framework für WPF-Anwendungen mit automatischer DataStore-Synchronisation, wiederverwendbaren Controls und Dependency Injection.

## Überblick

**CustomWPFControls** ist eine moderne Library für professionelle WPF-Entwicklung mit .NET 8, die folgende Kernfeatures bietet:

- **MVVM ViewModels** mit automatischem PropertyChanged-Support via Fody
- **Bidirektionale Synchronisation** zwischen DataStores und ViewModels
- **Wiederverwendbare Editor-Controls** für Listen und DropDowns
- **Command-Integration** für CRUD-Operationen
- **Dependency Injection** durchgängig unterstützt
- **Dialog-Services** für modale Fenster und MessageBoxes

## Projekte in dieser Solution

### [CustomWPFControls](CustomWPFControls/README.md)
Haupt-Library mit ViewModels, Controls, Commands und Services.

**Kernkomponenten:**
- `ViewModelBase<TModel>` - Basisklasse mit PropertyChanged
- `CollectionViewModel<TModel, TViewModel>` - Automatische Collection-Synchronisation
- `EditableCollectionViewModel<TModel, TViewModel>` - Mit CRUD-Commands
- `ListEditorView` - ListView mit integrierten Buttons
- `DropDownEditorView` - ComboBox mit integrierten Buttons
- `DialogService` - Modale Dialoge
- `MessageBoxService` - MessageBox-Wrapper

### [CustomWPFControls.TestHelpers](CustomWPFControls.TestHelpers/README.md)
Test-Utilities und Fixtures für Unit- und Integrationstests.

**Inhalte:**
- Bootstrap-Fixtures für DataStores
- Test-Models und ViewModels
- Helper-Methoden für Tests

### CustomWPFControls.Tests
Umfassende Test-Suite mit 70+ Tests.

**Test-Kategorien:**
- Unit-Tests für ViewModels und Commands
- Integration-Tests für DataStore-Synchronisation
- Behavior-Tests für CRUD-Operationen

## Schnellstart

### 1. Installation

```bash
dotnet add package PropertyChanged.Fody
dotnet add package Fody
dotnet add package Microsoft.Extensions.DependencyInjection
```

Referenzieren Sie die benötigten Projekte:
```xml
<ItemGroup>
  <ProjectReference Include="..\CustomWPFControls\CustomWPFControls.csproj" />
  <ProjectReference Include="..\DataStores\DataStores.csproj" />
  <ProjectReference Include="..\Common.Bootstrap\Common.Bootstrap.csproj" />
</ItemGroup>
```

### 2. Model und ViewModel erstellen

```csharp
// Model
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

// ViewModel
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    public string Name => Model.Name;
    public string Email => Model.Email;
    public bool IsSelected { get; set; }
}
```

### 3. DI-Setup

```csharp
public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        services.AddViewModelFactory<Customer, CustomerViewModel>();
        services.AddSingleton<IEqualityComparer<Customer>>(
            new FallbackEqualsComparer<Customer>());
    }
}
```

### 4. In XAML verwenden

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    SelectedItem="{Binding SelectedCustomer}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"
    AddButtonText="Neu"
    EditButtonText="Bearbeiten"
    DeleteButtonText="Löschen"/>
```

## Dokumentation

### Haupt-Dokumentation
- [Getting Started](CustomWPFControls/Docs/Getting-Started.md) - Ausführlicher Einstieg
- [Architecture](CustomWPFControls/Docs/Architecture.md) - Architektur-Übersicht
- [API Reference](CustomWPFControls/Docs/API-Reference.md) - Vollständige API-Dokumentation
- [Best Practices](CustomWPFControls/Docs/Best-Practices.md) - Empfehlungen und Patterns

### Komponenten-Spezifisch
- [ViewModelBase](CustomWPFControls/Docs/ViewModelBase.md) - Basisklasse für ViewModels
- [CollectionViewModel](CustomWPFControls/Docs/CollectionViewModel.md) - Collection-Synchronisation
- [EditableCollectionViewModel](CustomWPFControls/Docs/EditableCollectionViewModel.md) - Mit Commands
- [ViewModelFactory](CustomWPFControls/Docs/ViewModelFactory.md) - DI-Factory-Pattern
- [Controls-Guide](CustomWPFControls/Docs/Controls-Guide.md) - ListEditorView & DropDownEditorView

## Features im Detail

### ViewModels
```csharp
// Automatisches PropertyChanged via Fody
public class PersonViewModel : ViewModelBase<Person>
{
    public PersonViewModel(Person model) : base(model) { }
    
    // Domain-Properties (read-only)
    public string FullName => $"{Model.FirstName} {Model.LastName}";
    
    // UI-Properties (automatisches PropertyChanged)
    public bool IsExpanded { get; set; }
}
```

### Collection-Synchronisation
```csharp
var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
    dataStores,
    viewModelFactory,
    comparerService);

// Automatische Synchronisation
dataStores.GetGlobal<Customer>().Add(new Customer());
// ViewModel wird automatisch erstellt und in Items angezeigt
```

### Editor-Controls
```xml
<!-- ListView mit konfigurierbaren Buttons -->
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    AddCommand="{Binding AddCommand}"
    IsEditVisible="True"
    IsClearVisible="False"/>

<!-- ComboBox mit konfigurierbarem Button-Layout -->
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    ButtonPlacement="Right"
    AddCommand="{Binding AddCommand}"/>
```

## Architektur

```
???????????????????????????????????????
?         WPF Application             ?
?  (Views, Windows, UserControls)     ?
???????????????????????????????????????
               ? Bindings
               ?
???????????????????????????????????????
?      CustomWPFControls              ?
?  ?????????????????????????????????  ?
?  ? Controls                      ?  ?
?  ?  - ListEditorView             ?  ?
?  ?  - DropDownEditorView         ?  ?
?  ?????????????????????????????????  ?
?  ?????????????????????????????????  ?
?  ? ViewModels                    ?  ?
?  ?  - ViewModelBase              ?  ?
?  ?  - CollectionViewModel        ?  ?
?  ?  - EditableCollectionViewModel?  ?
?  ?????????????????????????????????  ?
?  ?????????????????????????????????  ?
?  ? Services                      ?  ?
?  ?  - DialogService              ?  ?
?  ?  - MessageBoxService          ?  ?
?  ?????????????????????????????????  ?
???????????????????????????????????????
               ? Data Access
               ?
???????????????????????????????????????
?         DataStores                  ?
?  (Persistierung & Synchronisation)  ?
???????????????????????????????????????
```

## Abhängigkeiten

- **.NET 8.0** - Zielframework
- **PropertyChanged.Fody** - Automatisches PropertyChanged
- **Microsoft.Extensions.DependencyInjection** - DI-Container
- **DataStores** - Persistierung und Synchronisation
- **Common.Bootstrap** - Service-Registrierung

## Tests

Die Library verfügt über eine umfassende Test-Suite:

```
Unit-Tests:          19 Tests
Integration-Tests:   25 Tests  
Behavior-Tests:      26 Tests
?????????????????????????????
GESAMT:              70 Tests
Coverage:            ~87%
```

## Best Practices

### ViewModel-Erstellung
```csharp
public class ProductViewModel : ViewModelBase<Product>
{
    public ProductViewModel(Product model, IDialogService dialogService) 
        : base(model)
    {
        // Model als erster Parameter für Factory-Kompatibilität
        // Weitere Dependencies via DI
    }
    
    // Domain-Properties delegieren an Model
    public string Name => Model.Name;
    public decimal Price => Model.Price;
    
    // UI-Properties mit Auto-Property
    public bool IsSelected { get; set; }
}
```

### DI-Registrierung
```csharp
services.AddViewModelFactory<Product, ProductViewModel>();
services.AddSingleton<IEqualityComparer<Product>>(
    new FallbackEqualsComparer<Product>());
```

### Control-Verwendung
```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    SelectedItem="{Binding SelectedCategory}"
    ButtonPlacement="Right"
    AddButtonText="Neu"
    EditButtonText="Ändern"
    DeleteButtonText="Entfernen"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

## Lizenz

Siehe [LICENSE](LICENSE) Datei.

## Support & Beiträge

- GitHub Issues: [CustomWPFControls/Issues](https://github.com/ReneRose1971/CustomWPFControls/issues)
- Dokumentation: [Docs-Verzeichnis](CustomWPFControls/Docs/)
- Pull Requests sind willkommen

## Changelog

### Version 1.0.0
- Initiale Version mit ViewModels
- CollectionViewModel und EditableCollectionViewModel
- ListEditorView und DropDownEditorView Controls
- Dialog- und MessageBox-Services
- Umfassende Dokumentation und Tests
