# CustomWPFControls

MVVM-Framework für WPF mit DataStore-Integration, automatischer Synchronisation und PropertyChanged-Support via Fody.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [Features](#features)
- [Installation](#installation)
- [Schnellstart](#schnellstart)
- [Controls](#controls)
- [ViewModels](#viewmodels)
- [Commands](#commands)
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
- **ObservableCommand** für automatische CanExecuteChanged-Benachrichtigungen bereitstellt
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
Collection-ViewModel mit **lokalem DataStore** und automatischer Synchronisation via TransformTo.

**Wichtig:** Jede `CollectionViewModel`-Instanz hat ihren **eigenen lokalen ModelStore** (keine globale Shared State)!

```csharp
var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
    services,  // ICustomWPFServices Facade
    viewModelFactory);

// ? Lokaler ModelStore für diese Instanz
viewModel.ModelStore.Add(new Customer { Name = "Alice" });
// ? ViewModel wird automatisch erstellt und zu Items hinzugefügt

// ? Items sind automatisch synchronisiert (via TransformTo)
<ListBox ItemsSource="{Binding Items}"/>
```

**Features:**
- ?? **Lokaler ModelStore** - Jede Instanz isoliert
- ?? **TransformTo-Integration** - Automatische Model?ViewModel Synchronisation
- ??? **Remove API** - Remove()/RemoveRange()/Clear() mit automatischem Dispose
- ?? **Selection-Tracking** - SelectedItem/SelectedItems Management

### 3. EditableCollectionViewModel<TModel, TViewModel>
Erweitert CollectionViewModel um Commands (Add, Delete, Edit, Clear) mit **automatischen CanExecuteChanged-Events**.

```csharp
var viewModel = new EditableCollectionViewModel<Customer, CustomerViewModel>(
    services,  // ICustomWPFServices Facade  
    viewModelFactory);

viewModel.CreateModel = () => new Customer();
viewModel.EditModel = customer => OpenEditDialog(customer);

// ? Commands mit automatischer CanExecute-Benachrichtigung
viewModel.AddCommand.Execute(null);
viewModel.DeleteCommand.Execute(null);

// ? CanExecuteChanged wird automatisch gefeuert!
viewModel.SelectedItem = someCustomer;  
// ? DeleteCommand & EditCommand CanExecuteChanged automatisch
```

**Neue Features:**
- ? **Automatische CanExecuteChanged-Events** - Keine manuelle Benachrichtigung nötig
- ? **ObservableCommand-Integration** - DeleteCommand, EditCommand, ClearCommand reagieren automatisch
- ? **Testfreundlich** - Funktioniert in Unit-Tests ohne CommandManager
- ? **Explizite Abhängigkeiten** - Klare Property ? Command Beziehungen

**Wichtig:** `AddCommand` fügt Models zum **lokalen ModelStore** hinzu (nicht zu globalem Store)!

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
        ICustomWPFServices services,  // ? Facade statt einzelne Services
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _customers.CreateModel = () => new Customer { Name = "Neu" };
        _customers.EditModel = customer => { /* Dialog öffnen */ };
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    
    // ? Commands mit automatischer CanExecuteChanged-Benachrichtigung
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;  // Reagiert auf SelectedItem
    public ICommand DeleteCommand => _customers.DeleteCommand;  // Reagiert auf SelectedItem
    
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

## Commands

### RelayCommand
Standard ICommand-Implementierung mit CommandManager.RequerySuggested.

```csharp
var command = new RelayCommand(
    execute: _ => DoSomething(),
    canExecute: _ => CanDoSomething());
```

**CanExecuteChanged-Verhalten:**
- Verwendet `CommandManager.RequerySuggested`
- Automatische Re-Evaluation bei UI-Aktionen (Focus, Mausklicks)
- **Nicht automatisch** bei PropertyChanged-Events

### ObservableCommand (? NEU)
ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

```csharp
var deleteCommand = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,  // ViewModel
    observedProperties: nameof(SelectedItem));  // Überwacht SelectedItem

// ? CanExecuteChanged wird automatisch gefeuert wenn SelectedItem sich ändert!
```

**Features:**
- ? **PropertyChanged-Monitoring** - Überwacht INotifyPropertyChanged-Objekte
- ? **Selektive Überwachung** - Nur spezifische Properties überwachen
- ? **Testfreundlich** - Funktioniert ohne CommandManager
- ? **Explizite Benachrichtigung** - RaiseCanExecuteChanged() für manuelle Steuerung

**Verwendung in EditableCollectionViewModel:**
```csharp
// DeleteCommand überwacht SelectedItem
viewModel.SelectedItem = customer;
// ? DeleteCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!

// ClearCommand überwacht Count
viewModel.ModelStore.Add(newCustomer);
// ? ClearCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!
```

**Wann welches Command verwenden:**
- **RelayCommand**: Standard-Szenarien, keine Property-Abhängigkeiten
- **ObservableCommand**: Commands die auf Property-Änderungen reagieren sollen

### AsyncRelayCommand
Asynchrone ICommand-Implementierung für async/await-Operationen.

```csharp
var loadCommand = new AsyncRelayCommand(
    execute: async () => await LoadDataAsync(),
    canExecute: () => !IsLoading);
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
        ICustomWPFServices services,  // ? Facade statt einzelne Services
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _customers.CreateModel = () => new Customer { Name = "Neu" };
        _customers.EditModel = customer => { /* Dialog öffnen */ };
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    
    // ? Commands mit automatischer CanExecuteChanged-Benachrichtigung
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;  // Reagiert auf SelectedItem
    public ICommand DeleteCommand => _customers.DeleteCommand;  // Reagiert auf SelectedItem
    
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

## Commands

### RelayCommand
Standard ICommand-Implementierung mit CommandManager.RequerySuggested.

```csharp
var command = new RelayCommand(
    execute: _ => DoSomething(),
    canExecute: _ => CanDoSomething());
```

**CanExecuteChanged-Verhalten:**
- Verwendet `CommandManager.RequerySuggested`
- Automatische Re-Evaluation bei UI-Aktionen (Focus, Mausklicks)
- **Nicht automatisch** bei PropertyChanged-Events

### ObservableCommand (? NEU)
ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

```csharp
var deleteCommand = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,  // ViewModel
    observedProperties: nameof(SelectedItem));  // Überwacht SelectedItem

// ? CanExecuteChanged wird automatisch gefeuert wenn SelectedItem sich ändert!
```

**Features:**
- ? **PropertyChanged-Monitoring** - Überwacht INotifyPropertyChanged-Objekte
- ? **Selektive Überwachung** - Nur spezifische Properties überwachen
- ? **Testfreundlich** - Funktioniert ohne CommandManager
- ? **Explizite Benachrichtigung** - RaiseCanExecuteChanged() für manuelle Steuerung

**Verwendung in EditableCollectionViewModel:**
```csharp
// DeleteCommand überwacht SelectedItem
viewModel.SelectedItem = customer;
// ? DeleteCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!

// ClearCommand überwacht Count
viewModel.ModelStore.Add(newCustomer);
// ? ClearCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!
```

**Wann welches Command verwenden:**
- **RelayCommand**: Standard-Szenarien, keine Property-Abhängigkeiten
- **ObservableCommand**: Commands die auf Property-Änderungen reagieren sollen

### AsyncRelayCommand
Asynchrone ICommand-Implementierung für async/await-Operationen.

```csharp
var loadCommand = new AsyncRelayCommand(
    execute: async () => await LoadDataAsync(),
    canExecute: () => !IsLoading);
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
        ICustomWPFServices services,  // ? Facade statt einzelne Services
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _customers.CreateModel = () => new Customer { Name = "Neu" };
        _customers.EditModel = customer => { /* Dialog öffnen */ };
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    
    // ? Commands mit automatischer CanExecuteChanged-Benachrichtigung
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;  // Reagiert auf SelectedItem
    public ICommand DeleteCommand => _customers.DeleteCommand;  // Reagiert auf SelectedItem
    
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

## Commands

### RelayCommand
Standard ICommand-Implementierung mit CommandManager.RequerySuggested.

```csharp
var command = new RelayCommand(
    execute: _ => DoSomething(),
    canExecute: _ => CanDoSomething());
```

**CanExecuteChanged-Verhalten:**
- Verwendet `CommandManager.RequerySuggested`
- Automatische Re-Evaluation bei UI-Aktionen (Focus, Mausklicks)
- **Nicht automatisch** bei PropertyChanged-Events

### ObservableCommand (? NEU)
ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

```csharp
var deleteCommand = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,  // ViewModel
    observedProperties: nameof(SelectedItem));  // Überwacht SelectedItem

// ? CanExecuteChanged wird automatisch gefeuert wenn SelectedItem sich ändert!
```

**Features:**
- ? **PropertyChanged-Monitoring** - Überwacht INotifyPropertyChanged-Objekte
- ? **Selektive Überwachung** - Nur spezifische Properties überwachen
- ? **Testfreundlich** - Funktioniert ohne CommandManager
- ? **Explizite Benachrichtigung** - RaiseCanExecuteChanged() für manuelle Steuerung

**Verwendung in EditableCollectionViewModel:**
```csharp
// DeleteCommand überwacht SelectedItem
viewModel.SelectedItem = customer;
// ? DeleteCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!

// ClearCommand überwacht Count
viewModel.ModelStore.Add(newCustomer);
// ? ClearCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!
```

**Wann welches Command verwenden:**
- **RelayCommand**: Standard-Szenarien, keine Property-Abhängigkeiten
- **ObservableCommand**: Commands die auf Property-Änderungen reagieren sollen

### AsyncRelayCommand
Asynchrone ICommand-Implementierung für async/await-Operationen.

```csharp
var loadCommand = new AsyncRelayCommand(
    execute: async () => await LoadDataAsync(),
    canExecute: () => !IsLoading);
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
        ICustomWPFServices services,  // ? Facade statt einzelne Services
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _customers.CreateModel = () => new Customer { Name = "Neu" };
        _customers.EditModel = customer => { /* Dialog öffnen */ };
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    
    // ? Commands mit automatischer CanExecuteChanged-Benachrichtigung
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;  // Reagiert auf SelectedItem
    public ICommand DeleteCommand => _customers.DeleteCommand;  // Reagiert auf SelectedItem
    
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

## Tests

```
Unit-Tests:          344 Tests (21 neue für ObservableCommand & Commands)
Integration-Tests:   Vollständige DataStore-Integration
Behavior-Tests:      CRUD-Operationen End-to-End
??????????????????????????????????????????????
GESAMT:              344 Tests
Coverage:            ~87%
```

**Neue Tests:**
- **ObservableCommand** (12 Tests): Constructor, CanExecute, Execute, CanExecuteChanged
- **EditableCollectionViewModel Commands** (9 Tests): Automatische CanExecuteChanged-Benachrichtigungen

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
