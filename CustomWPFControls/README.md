# CustomWPFControls

MVVM-Framework für WPF mit DataStore-Integration, automatischer Synchronisation und PropertyChanged-Support via Fody.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [Features](#features)
- [Installation](#installation)
- [Schnellstart](#schnellstart)
- [Controls](#controls)
- [ViewModels](#viewmodels)
- [Services](#services)
- [Dokumentation](#dokumentation)
- [Tests](#tests)

---

## Überblick

**CustomWPFControls** ist ein leistungsstarkes MVVM-Framework für WPF-Anwendungen, das:
- Bidirektionale Synchronisation zwischen DataStore und ViewModels bietet
- Automatische PropertyChanged-Events via Fody.PropertyChanged implementiert
- ViewModelFactory für DI-basierte ViewModel-Erstellung bereitstellt
- CollectionViewModel für einfache Collection-Verwaltung anbietet
- EditableCollectionViewModel mit Commands (Add, Delete, Edit, Clear) erweitert
- Wiederverwendbare Editor-Controls (ListEditorView, DropDownEditorView) bereitstellt
- Dialog- und MessageBox-Services integriert

---

## Features

### 1. ViewModelBase<TModel>
Basisklasse für alle ViewModels mit automatischem PropertyChanged-Support.

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    // Domain-Properties (delegiert an Model)
    public string Name => Model.Name;
    
    // UI-Properties (mit PropertyChanged)
    public bool IsSelected { get; set; }
}
```

### 2. CollectionViewModel<TModel, TViewModel>
Bidirektionale Synchronisation zwischen DataStore und ViewModels.

```csharp
var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
    dataStores,
    viewModelFactory,
    comparerService);

// Items sind automatisch synchronisiert
dataStores.GetGlobal<Customer>().Add(new Customer { Name = "Alice" });
// ViewModel wird automatisch erstellt und in Items angezeigt
```

### 3. EditableCollectionViewModel<TModel, TViewModel>
Erweitert CollectionViewModel um Commands (Add, Delete, Edit, Clear).

```csharp
var viewModel = new EditableCollectionViewModel<Customer, CustomerViewModel>(
    dataStores, factory, comparerService);

viewModel.CreateModel = () => new Customer();
viewModel.EditModel = customer => OpenEditDialog(customer);

// Commands sind bereit
viewModel.AddCommand.Execute(null);
viewModel.DeleteCommand.Execute(null);
```

### 4. ListEditorView
ListView mit integrierten CRUD-Buttons.

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    SelectedItem="{Binding SelectedCustomer}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"
    AddButtonText="Hinzufügen"
    EditButtonText="Bearbeiten"
    DeleteButtonText="Löschen"/>
```

### 5. DropDownEditorView
ComboBox mit konfigurierbarem Button-Layout (Right/Bottom/Top).

```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    SelectedItem="{Binding SelectedCategory}"
    ButtonPlacement="Right"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

### 6. ViewModelFactory<TModel, TViewModel>
DI-basierte Factory für ViewModel-Erstellung.

```csharp
services.AddViewModelFactory<Customer, CustomerViewModel>();

var factory = serviceProvider.GetRequiredService<IViewModelFactory<Customer, CustomerViewModel>>();
var viewModel = factory.Create(customer);
```

---

## Installation

### Voraussetzungen
- .NET 8.0 oder höher
- WPF-Projekt

### NuGet-Pakete
```bash
dotnet add package PropertyChanged.Fody
dotnet add package Fody
dotnet add package Microsoft.Extensions.DependencyInjection
```

### Projekt-Referenzen
```xml
<ProjectReference Include="..\DataStores\DataStores.csproj" />
<ProjectReference Include="..\Common.Bootstrap\Common.Bootstrap.csproj" />
```

### FodyWeavers.xml erstellen
```xml
<?xml version="1.0" encoding="utf-8"?>
<Weavers xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <PropertyChanged FilterType="Explicit" InjectOnPropertyNameChanged="false" />
</Weavers>
```

---

## Schnellstart

[Siehe vollständigen Schnellstart-Guide](Docs/Getting-Started.md)

### 1. Model definieren

```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
```

### 2. ViewModel erstellen

```csharp
public class CustomerViewModel : ViewModelBase<Customer>
{
    public CustomerViewModel(Customer model) : base(model) { }
    
    public int Id => Model.Id;
    public string Name => Model.Name;
    public string Email => Model.Email;
    public bool IsSelected { get; set; }
}
```

### 3. DI registrieren

```csharp
public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // DataStores Core Services
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

### 4. ViewModel verwenden

```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            dataStores, factory, comparerService);
        
        _customers.CreateModel = () => new Customer { Name = "Neu" };
        _customers.EditModel = customer => { /* Dialog öffnen */ };
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;
    public ICommand DeleteCommand => _customers.DeleteCommand;
    
    public void Dispose() => _customers.Dispose();
}
```

### 5. In XAML binden

```xml
<Window xmlns:controls="clr-namespace:CustomWPFControls.Controls;assembly=CustomWPFControls">
    <controls:ListEditorView 
        ItemsSource="{Binding Customers}"
        SelectedItem="{Binding SelectedCustomer}"
        AddCommand="{Binding AddCommand}"
        EditCommand="{Binding EditCommand}"
        DeleteCommand="{Binding DeleteCommand}"/>
</Window>
```

---

## Controls

### BaseListView
Erweitert ListView um Count-Property.

```csharp
public class BaseListView : ListView
{
    public int Count { get; }
}
```

### ListEditorView
ListView mit CRUD-Buttons unterhalb der Liste.

**Features:**
- Add, Edit, Delete, Clear Commands
- Konfigurierbare Button-Texte
- Selektive Button-Sichtbarkeit
- Count-Anzeige

### BaseComboBoxView
Erweitert ComboBox um Count-Property.

```csharp
public class BaseComboBoxView : ComboBox
{
    public int Count { get; }
}
```

### DropDownEditorView
ComboBox mit CRUD-Buttons und konfigurierbarem Layout.

**Features:**
- Add, Edit, Delete, Clear Commands
- ButtonPlacement: Right, Bottom, Top
- Konfigurierbare Button-Texte
- Selektive Button-Sichtbarkeit

[Siehe Controls-Guide](Docs/Controls-Guide.md) für Details

---

## ViewModels

### ViewModelBase<TModel>
Basisklasse mit PropertyChanged-Support.

**Features:**
- Model-Wrapping
- Automatisches PropertyChanged (Fody)
- Equals/GetHashCode basierend auf Model

### CollectionViewModel<TModel, TViewModel>
Automatische Synchronisation zwischen DataStore und ViewModels.

**Features:**
- TransformTo-Integration
- ReadOnlyObservableCollection für View-Binding
- SelectedItem/SelectedItems Management
- Remove/RemoveRange/Clear API

### EditableCollectionViewModel<TModel, TViewModel>
Erweitert CollectionViewModel um Commands.

**Features:**
- AddCommand, EditCommand, DeleteCommand, ClearCommand
- CreateModel und EditModel Delegates
- Automatische CanExecute-Logik

[Siehe API Reference](Docs/API-Reference.md) für Details

---

## Services

### DialogService
Service für modale Dialoge.

```csharp
public interface IDialogService
{
    bool? ShowDialog(IDialogViewModel viewModel);
}
```

### MessageBoxService
Wrapper für MessageBox mit MVVM-Support.

```csharp
public interface IMessageBoxService
{
    MessageBoxResult Show(string message, string caption = "", 
        MessageBoxButton buttons = MessageBoxButton.OK);
}
```

### WindowLayoutService
Speichert und stellt Fenster-Layouts wieder her.

```csharp
public interface IWindowLayoutService
{
    void SaveLayout(Window window, string key);
    void RestoreLayout(Window window, string key);
}
```

---

## Dokumentation

### Vollständige Dokumentation

- [Getting Started](Docs/Getting-Started.md) - Detaillierter Einstieg
- [Architecture](Docs/Architecture.md) - Architektur-Übersicht
- [Controls-Guide](Docs/Controls-Guide.md) - ListEditorView & DropDownEditorView
- [ViewModelBase](Docs/ViewModelBase.md) - Basisklasse
- [CollectionViewModel](Docs/CollectionViewModel.md) - Collection-Sync
- [EditableCollectionViewModel](Docs/EditableCollectionViewModel.md) - Commands
- [ViewModelFactory](Docs/ViewModelFactory.md) - Factory-Pattern
- [Best Practices](Docs/Best-Practices.md) - Tipps & Tricks
- [API Reference](Docs/API-Reference.md) - Vollständige API

---

## Tests

```
Unit-Tests:          19 Tests
Integration-Tests:   25 Tests
Behavior-Tests:      26 Tests
?????????????????????????????
GESAMT:              70 Tests
Coverage:            ~87%
```

Test-Kategorien:
- **Unit-Tests**: ViewModels, Commands, Services isoliert
- **Integration-Tests**: DataStore-Synchronisation
- **Behavior-Tests**: CRUD-Operationen End-to-End

---

## Lizenz

Siehe LICENSE-Datei im Repository.

---

## Support

- Issues erstellen: [GitHub Issues](https://github.com/ReneRose1971/CustomWPFControls/issues)
- Dokumentation: [Docs-Verzeichnis](Docs/)
- Pull Requests: Willkommen
