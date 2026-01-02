# CustomWPFControls - Entwickler-Dokumentation

**Version:** 2.0.0  
**Target:** .NET 8 / WPF  
**Lizenz:** MIT  

---

## ?? Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Architektur-Hierarchie](#architektur-hierarchie)
3. [Kern-Komponenten](#kern-komponenten)
4. [IDialogService - Convention over Configuration](#idialogservice---convention-over-configuration)
5. [IMessageBoxService - Testbare MessageBoxen](#imessageboxservice---testbare-messageboxen)
6. [Testing mit CustomWPFControls.TestHelpers](#testing-mit-customwpfcontrolstesthelpers)
7. [ViewModels erstellen](#viewmodels-erstellen)
8. [Verwendung](#verwendung)
9. [DialogService mit WindowLayoutService](#dialogservice-mit-windowlayoutservice)
10. [Vollständiges Beispiel](#vollständiges-beispiel)
11. [Best Practices](#best-practices)
12. [Troubleshooting](#troubleshooting)

---

## Übersicht

**CustomWPFControls** ist eine WPF-Library für **automatische ViewModel-Verwaltung** mit Integration in **DataStores**.

### Kernfunktionen

- ? **Automatische ViewModel-Erstellung** via `IViewModelFactory`
- ? **Bidirektionale Synchronisation** zwischen DataStore und UI
- ? **MVVM-Pattern** mit `ViewModelBase<TModel>`
- ? **Commands** via `EditableCollectionViewModel`
- ? **DI-Integration** mit optionalem `IServiceProvider`
- ? **Dialog-Management** via `IDialogService` ? **NEU**
- ? **MessageBox-Management** via `IMessageBoxService` ? **NEU**
- ? **Convention-based Dialogs** via Marker-Interfaces ? **NEU**
- ? **Assembly-Scanning** für automatische Registrierung ? **NEU**
- ? **Test-Helpers** für testbare UI-Interaktionen ? **NEU**
- ? **Automatische Dialog-Persistierung** via `WindowLayoutService` ? **NEU**

---

## Architektur-Hierarchie

```
???????????????????????????????????????????????????????????????
?                    IViewModelWrapper<TModel>                 ?
?  Interface: Model-Property für alle ViewModels               ?
???????????????????????????????????????????????????????????????
                              ?
                              ? implements
                              ?
???????????????????????????????????????????????????????????????
?              ViewModelBase<TModel> (abstract)                ?
?  - Model: TModel (read-only)                                ?
?  - ServiceProvider?: IServiceProvider (optional, protected)  ?
?  - INotifyPropertyChanged (via Fody)                         ?
?  - Equals/GetHashCode (referenz-basiert)                     ?
???????????????????????????????????????????????????????????????
                              ?
                              ? erben Consumer-ViewModels
                              ?
        ?????????????????????????????????????????????
        ?                                           ?
?????????????????????????              ?????????????????????????
?  CustomerViewModel    ?              ?  OrderViewModel       ?
?  : ViewModelBase<>    ?              ?  : ViewModelBase<>    ?
?????????????????????????              ?????????????????????????
        ?                                           ?
        ? erstellt via                              ?
        ?                                           ?
        ?                                           ?
???????????????????????????????????????????????????????????????
?          IViewModelFactory<TModel, TViewModel>               ?
?  Create(TModel model) ? TViewModel                           ?
???????????????????????????????????????????????????????????????
                              ?
                              ? verwendet von
                              ?
???????????????????????????????????????????????????????????????
?         CollectionViewModel<TModel, TViewModel>              ?
?  - Items: ReadOnlyObservableCollection<TViewModel>          ?
?  - SelectedItem: TViewModel?                                ?
?  - Synchronisiert DataStore ? ViewModels                    ?
???????????????????????????????????????????????????????????????
                              ?
                              ? erweitert
                              ?
???????????????????????????????????????????????????????????????
?      EditableCollectionViewModel<TModel, TViewModel>        ?
?  + AddCommand, DeleteCommand, ClearCommand, EditCommand      ?
?  + CreateModel: Func<TModel>?                               ?
?  + EditModel: Action<TModel>?                               ?
???????????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????????
?                      IDialogService ? NEU                   ?
?  - ShowDialog<TViewModel>()  (Convention-based)              ?
?  - ShowDialog(viewModel)     (Instance-based)                ?
?  - ShowWindow<TViewModel>()                                  ?
?  - ShowMessage(), ShowWarning(), ShowError()                 ?
?  - Integration mit WindowLayoutService                       ?
?  - Automatische Assembly-Scanning Registrierung              ?
???????????????????????????????????????????????????????????????
```

---

## DialogService mit WindowLayoutService

### Automatische Dialog-Persistierung

**IDialogService** nutzt automatisch **WindowLayoutService** um Position und Größe aller Dialoge zu persistieren.

#### Funktionsweise:

```
Dialog wird geöffnet
    ?
DialogService.ShowDialog<CustomerEditViewModel>()
    ?
1. View aus DI-Container holen (via IDialogView<CustomerEditViewModel>)
2. WindowLayoutService.Attach(window, "Dialog_CustomerEditViewModel")
    ?
3. Layout wiederherstellen (falls vorhanden)
    ?
4. Dialog anzeigen
    ?
Benutzer ändert Position/Größe
    ?
5. WindowLayoutService persistiert automatisch zu JSON
    ?
Dialog schließen
    ?
Nächstes Öffnen ? Position wiederhergestellt ?
```

#### Layout-Key-Generierung:

```csharp
// Automatische Key-Generierung:
"Dialog_" + ViewModel-Typ-Name

// Beispiele:
CustomerEditViewModel    ? "Dialog_CustomerEditViewModel"
OrderViewModel          ? "Dialog_OrderViewModel"
SettingsViewModel       ? "Dialog_SettingsViewModel"
```

#### Persistierte Daten:

```json
{
  "WindowKey": "Dialog_CustomerEditViewModel",
  "Left": 450.0,
  "Top": 200.0,
  "Width": 640.0,
  "Height": 480.0,
  "WindowState": 0
}
```

---

### Verwendungsbeispiel mit EditableCollectionViewModel

```csharp
public class CustomerListViewModel
{
    private readonly IDialogService _dialogService;
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService,
        IDialogService dialogService)  // ? IDialogService injizieren
    {
        _dialogService = dialogService;
        
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            dataStores, factory, comparerService);
        
        // CreateModel: Dialog für neuen Kunden
        _customers.CreateModel = CreateNewCustomer;
        
        // EditModel: Dialog für Bearbeitung
        _customers.EditModel = EditCustomer;
    }
    
    private Customer? CreateNewCustomer()
    {
        var vm = new CustomerEditViewModel();
        
        // Dialog zeigen - Position wird automatisch wiederhergestellt
        var result = _dialogService.ShowDialog(vm, "Neuer Kunde");
        
        if (result == true)
        {
            return new Customer
            {
                Name = vm.Name,
                Email = vm.Email
            };
        }
        
        return null;  // Abbruch
    }
    
    private void EditCustomer(Customer customer)
    {
        var vm = new CustomerEditViewModel(customer);
        
        // Dialog zeigen - Position wird automatisch wiederhergestellt
        var result = _dialogService.ShowDialog(vm, "Kunde bearbeiten");
        
        if (result == true)
        {
            customer.Name = vm.Name;
            customer.Email = vm.Email;
            // PropertyChanged ? DataStore ? Auto-Persistenz
        }
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers 
        => _customers.Items;
    
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand DeleteCommand => _customers.DeleteCommand;
    public ICommand EditCommand => _customers.EditCommand;
}
```

---

### DI-Registrierung mit WindowLayoutService

```csharp
using Common.Bootstrap;
using CustomWPFControls.Bootstrap;
using DataStores.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// ????????????????????????????????????????????????????????????
// Bootstrap-Decorator-Chain (Decorator-Pattern)
// ????????????????????????????????????????????????????????????

var bootstrap = new CustomWPFControlsBootstrapDecorator(
    new DataStoresBootstrapDecorator(
        new DefaultBootstrapWrapper()));

// ????????????????????????????????????????????????????????????
// Assembly-Scanning - Automatische Registrierung
// ????????????????????????????????????????????????????????????

bootstrap.RegisterServices(
    builder.Services,
    typeof(Program).Assembly);  // ? Scannt ALLE IDialogView<> und IDialogViewModelMarker

// Was wird automatisch registriert:
// ? IServiceModule-Implementierungen (via DefaultBootstrapWrapper)
// ? IEqualityComparer<T>-Implementierungen (via DefaultBootstrapWrapper)
// ? IDataStoreRegistrar-Implementierungen (via DataStoresBootstrapDecorator)
// ? IDialogView<TViewModel>-Implementierungen (via CustomWPFControlsBootstrapDecorator)
// ? IDialogViewModelMarker-Implementierungen (via CustomWPFControlsBootstrapDecorator)
// ? IDialogService, IMessageBoxService, WindowLayoutService (via CustomWPFControlsServiceModule)

// ????????????????????????????????????????????????????????????
// Manuelle Service-Registrierungen (optional)
// ????????????????????????????????????????????????????????????

builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddViewModelFactory<Customer, CustomerViewModel>();
builder.Services.AddTransient<MainViewModel>();

// ????????????????????????????????????????????????????????????
// Build & Run
// ????????????????????????????????????????????????????????????

var app = builder.Build();  // ? NUR EIN BUILD!

// DataStore-Bootstrap ausführen
await DataStoreBootstrap.RunAsync(app.Services);

await app.RunAsync();
```

**Was passiert automatisch:**

1. ? `WindowLayoutService` wird als Singleton registriert
2. ? `IDialogService` wird mit `WindowLayoutService` erstellt
3. ? `IMessageBoxService` wird als Singleton registriert
4. ? Alle `IDialogView<TViewModel>`-Implementierungen werden gefunden und registriert
5. ? Alle `IDialogViewModelMarker`-Implementierungen werden gefunden und registriert
6. ? Position/Größe wird in `WindowLayoutData`-DataStore persistiert
7. ? JSON-Datei wird automatisch geschrieben (via `WindowLayoutDataStoreRegistrar`)

**KEINE manuelle View-Registrierung mehr nötig!** ?

---

### Features

#### 1. Automatische Position-Wiederherstellung

```csharp
// Erstes Öffnen: Dialog zentriert
dialogService.ShowDialog(vm, "Kunde bearbeiten");
// Benutzer verschiebt Dialog nach (500, 300)
// Dialog schließen

// Zweites Öffnen: Dialog öffnet bei (500, 300) ?
dialogService.ShowDialog(vm, "Kunde bearbeiten");
```

#### 2. Automatische Größen-Wiederherstellung

```csharp
// Erstes Öffnen: Dialog mit Standardgröße
dialogService.ShowDialog(vm);
// Benutzer vergrößert auf 800x600
// Dialog schließen

// Zweites Öffnen: Dialog öffnet mit 800x600 ?
dialogService.ShowDialog(vm);
```

#### 3. WindowState-Persistierung

```csharp
// Dialog maximieren
// Dialog schließen

// Nächstes Öffnen: Dialog öffnet maximiert ?
```

#### 4. Fehlertoleranz

```csharp
// Falls WindowLayoutService nicht verfügbar ist:
// ? Dialog funktioniert trotzdem (nur ohne Persistierung)

// Falls Layout-Key bereits verwendet wird:
// ? Dialog funktioniert trotzdem (nur ohne Persistierung)
```

---

## Vollständiges Beispiel

### 1. Domain-Model

```csharp
namespace MyApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public override bool Equals(object? obj)
        {
            return obj is Customer other && Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
```

---

### 2. EqualityComparer (automatisch gefunden via Assembly-Scanning)

```csharp
namespace MyApp.Comparers
{
    public sealed class CustomerComparer : IEqualityComparer<Customer>
    {
        public bool Equals(Customer? x, Customer? y)
        {
            if (x == null || y == null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(Customer obj) => obj.Id.GetHashCode();
    }
}
```

---

### 3. ViewModel

```csharp
namespace MyApp.ViewModels
{
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

        // Domain-Properties
        public int Id => Model.Id;
        public string Name => Model.Name;
        public string Email => Model.Email;
        public string Phone => Model.Phone;

        // UI-Properties (PropertyChanged via Fody)
        public bool IsSelected { get; set; }

        // Actions
        public void SendEmail()
        {
            _messageService.Send($"Email an {Email}");
            
            var logger = ServiceProvider?.GetService<ILogger>();
            logger?.LogInformation($"Email sent to {Email}");
        }
    }
}
```

---

### 4. Edit-Dialog ViewModel mit Marker-Interface

```csharp
namespace MyApp.ViewModels
{
    using CustomWPFControls.Services.Dialogs;
    
    [AddINotifyPropertyChangedInterface]
    public class CustomerEditViewModel : IDialogViewModelMarker  // ? Marker für Assembly-Scanning!
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        
        public CustomerEditViewModel() { }
        
        public CustomerEditViewModel(Customer customer)
        {
            Name = customer.Name;
            Email = customer.Email;
        }
        
        public Customer ToModel()
        {
            return new Customer
            {
                Name = Name,
                Email = Email
            };
        }
        
        public void UpdateModel(Customer customer)
        {
            customer.Name = Name;
            customer.Email = Email;
        }
        
        public bool Validate()
        {
            return !string.IsNullOrEmpty(Name) && 
                   !string.IsNullOrEmpty(Email);
        }
    }
}
```

**Wichtig:** `IDialogViewModelMarker` ermöglicht automatische Registrierung via Assembly-Scanning!

---

### 5. Edit-Dialog View mit Marker-Interface

```xml
<Window x:Class="MyApp.Views.CustomerEditDialog"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Kunde bearbeiten" 
        Width="400" Height="250"
        WindowStartupLocation="CenterOwner">
    
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <TextBlock Text="Name:" Grid.Row="0"/>
        <TextBox Text="{Binding Name}" Grid.Row="0" Margin="80,0,0,0"/>
        
        <TextBlock Text="E-Mail:" Grid.Row="1" Margin="0,10,0,0"/>
        <TextBox Text="{Binding Email}" Grid.Row="1" Margin="80,10,0,0"/>
        
        <StackPanel Grid.Row="3" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="OK" Width="80" IsDefault="True" Click="OnOkClick"/>
            <Button Content="Abbrechen" Width="80" Margin="10,0,0,0" IsCancel="True"/>
        </StackPanel>
    </Grid>
</Window>
```

```csharp
namespace MyApp.Views
{
    using System.Windows;
    using CustomWPFControls.Services.Dialogs;
    using MyApp.ViewModels;
    
    public partial class CustomerEditDialog : Window, 
        IDialogView<CustomerEditViewModel>  // ? Marker + Zuordnung!
    {
        public CustomerEditDialog()
        {
            InitializeComponent();
        }
        
        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as CustomerEditViewModel;
            if (vm?.Validate() == true)
            {
                DialogResult = true;
            }
        }
    }
}
```

**Wichtig:** `IDialogView<CustomerEditViewModel>` definiert die ViewModel-Zuordnung via Generic-Typ!

## Best Practices

### ? DO's

1. **Marker-Interfaces konsequent verwenden**
   ```csharp
   // ViewModel
   public class MyViewModel : IDialogViewModelMarker { }
   
   // View
   public class MyDialog : Window, IDialogView<MyViewModel> { }
   ```

2. **Assembly-Scanning für automatische Registrierung nutzen**
   ```csharp
   var bootstrap = new CustomWPFControlsBootstrapDecorator(...);
   bootstrap.RegisterServices(builder.Services, typeof(Program).Assembly);
   ```

3. **Convention-based für einfache Dialoge**
   ```csharp
   // ViewModel wird automatisch erstellt + Dependencies aufgelöst
   _dialogService.ShowDialog<SettingsViewModel>("Einstellungen");
   ```

4. **Instance-based für CreateModel/EditModel**
   ```csharp
   _customers.CreateModel = () =>
   {
       var vm = new CustomerEditViewModel();
       return _dialogService.ShowDialog(vm) == true ? vm.ToModel() : null;
   };
   ```

5. **Dependencies über Constructor injiziert**
   ```csharp
   public class MyViewModel : IDialogViewModelMarker
   {
       public MyViewModel(IService service) { }  // ? Wird automatisch aufgelöst
   }
   ```

6. **IMessageBoxService für MessageBoxen verwenden**
   ```csharp
   // ? RICHTIG
   _messageBoxService.ShowMessage("Gespeichert!");
   
   // ? FALSCH
   MessageBox.Show("Gespeichert!");
   ```

7. **MockDialogService und MockMessageBoxService in Unit-Tests**
   ```csharp
   using CustomWPFControls.TestHelpers.Mocks;
   
   var mockDialog = new MockDialogService();
   var mockMessageBox = new MockMessageBoxService();
   
   mockDialog.NextDialogResult = true;
   mockMessageBox.NextYesNoResult = true;
   
   var vm = new MainViewModel(mockDialog, mockMessageBox);
   vm.AddCommand.Execute(null);
   
   mockDialog.VerifyDialogShown<CustomerEditViewModel>();
   mockMessageBox.VerifyMessageShown("Erfolgreich");
   ```

---

### ? DON'Ts

1. **Keine manuellen View-Registrierungen**
   ```csharp
   // ? FALSCH - NICHT mehr nötig!
   registry.Register<CustomerEditViewModel, CustomerEditDialog>();
   
   // ? RICHTIG - Automatisch via Assembly-Scanning
   public class CustomerEditDialog : Window, IDialogView<CustomerEditViewModel> { }
   ```

2. **Keine manuellen ServiceProvider-Builds**
   ```csharp
   // ? FALSCH
   var sp = builder.Services.BuildServiceProvider();
   var registry = sp.GetRequiredService<IDialogRegistry>();
   
   // ? RICHTIG
   var bootstrap = new CustomWPFControlsBootstrapDecorator(...);
   bootstrap.RegisterServices(builder.Services, typeof(Program).Assembly);
   ```

3. **Keine Views mit Constructor-Dependencies**
   ```csharp
   // ? FALSCH - Views brauchen parameterlosen Constructor
   public class MyDialog : Window
   {
       public MyDialog(IService service) { }
   }
   
   // ? RICHTIG - Dependencies ins ViewModel
   public class MyViewModel : IDialogViewModelMarker
   {
       public MyViewModel(IService service) { }
   }
   ```

4. **Keine statischen Dialog-Aufrufe**
   ```csharp
   // ? FALSCH - MessageBox
   MessageBox.Show("Message");
   
   // ? RICHTIG - IMessageBoxService
   _messageBoxService.ShowMessage("Message");
   
   // ? FALSCH - Direktes Dialog-Erstellen
   var dialog = new CustomerEditDialog();
   dialog.ShowDialog();
   
   // ? RICHTIG - IDialogService
   _dialogService.ShowDialog<CustomerEditViewModel>();
   ```

5. **Keine manuelle ViewModel-Erstellung wenn nicht nötig**
   ```csharp
   // ? FALSCH (wenn keine Initialisierung nötig)
   var vm = new CustomerEditViewModel();
   _dialogService.ShowDialog(vm);
   
   // ? RICHTIG - Convention-based
   _dialogService.ShowDialog<CustomerEditViewModel>();
   ```

6. **Keine Test-Dependencies in Production-Code**
   ```csharp
   // ? FALSCH - MockDialogService in Production
   public class MainViewModel
   {
       public MainViewModel(MockDialogService dialog) { }  // ? FALSCH!
   }
   
   // ? RICHTIG - Interface in Production
   public class MainViewModel
   {
       public MainViewModel(IDialogService dialog) { }  // ? RICHTIG!
   }
   ```

---

### 7. DI-Setup mit Assembly-Scanning (Program.cs)

```csharp
using Common.Bootstrap;
using CustomWPFControls.Bootstrap;
using CustomWPFControls.Factories;
using DataStores.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// ????????????????????????????????????????????????????????????
// Bootstrap-Decorator-Chain (Decorator-Pattern)
// ????????????????????????????????????????????????????????????

var bootstrap = new CustomWPFControlsBootstrapDecorator(
    new DataStoresBootstrapDecorator(
        new DefaultBootstrapWrapper()));

// ????????????????????????????????????????????????????????????
// Assembly-Scanning - Automatische Registrierung
// ????????????????????????????????????????????????????????????

bootstrap.RegisterServices(
    builder.Services,
    typeof(Program).Assembly);  // ? Scannt ALLE IDialogView<> und IDialogViewModelMarker

// Was wird automatisch registriert:
// ? IServiceModule-Implementierungen (via DefaultBootstrapWrapper)
// ? IEqualityComparer<T>-Implementierungen (via DefaultBootstrapWrapper)
// ? IDataStoreRegistrar-Implementierungen (via DataStoresBootstrapDecorator)
// ? IDialogView<TViewModel>-Implementierungen (via CustomWPFControlsBootstrapDecorator)
// ? IDialogViewModelMarker-Implementierungen (via CustomWPFControlsBootstrapDecorator)
// ? IDialogService, IMessageBoxService, WindowLayoutService (via CustomWPFControlsServiceModule)

// ????????????????????????????????????????????????????????????
// Manuelle Service-Registrierungen
// ????????????????????????????????????????????????????????????

builder.Services.AddSingleton<IMessageService, MessageService>();
builder.Services.AddViewModelFactory<Customer, CustomerViewModel>();
builder.Services.AddTransient<MainViewModel>();

// ????????????????????????????????????????????????????????????
// Build & Run
// ????????????????????????????????????????????????????????????

var app = builder.Build();  // ? NUR EIN BUILD!

// DataStore-Bootstrap ausführen
await DataStoreBootstrap.RunAsync(app.Services);

await app.RunAsync();
```

**Wichtig:** KEINE manuelle View-Registrierung mehr! Alles automatisch via Assembly-Scanning! ?

---

### 8. MainViewModel mit Services

```csharp
namespace MyApp.ViewModels
{
    public class MainViewModel : IDisposable
    {
        private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
        private readonly IDialogService _dialogService;
        private readonly IMessageBoxService _messageBoxService;

        public MainViewModel(
            IDataStores dataStores,
            IViewModelFactory<Customer, CustomerViewModel> factory,
            IEqualityComparerService comparerService,
            IDialogService dialogService,
            IMessageBoxService messageBoxService)  // ? IMessageBoxService injiziert
        {
            _dialogService = dialogService;
            _messageBoxService = messageBoxService;
            
            _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
                dataStores,
                factory,
                comparerService);

            // Instance-based (für CreateModel/EditModel empfohlen)
            _customers.CreateModel = () =>
            {
                var vm = new CustomerEditViewModel();
                var result = _dialogService.ShowDialog(vm, "Neuer Kunde");
                
                if (result == true)
                {
                    _messageBoxService.ShowMessage("Kunde erfolgreich hinzugefügt!");
                    return vm.ToModel();
                }
                
                return null;
            };
            
            _customers.EditModel = (customer) =>
            {
                var vm = new CustomerEditViewModel(customer);
                var result = _dialogService.ShowDialog(vm, "Kunde bearbeiten");
                
                if (result == true)
                {
                    vm.UpdateModel(customer);
                    _messageBoxService.ShowMessage("Kunde erfolgreich aktualisiert!");
                }
            };
        }
        
        // Delete mit Bestätigung
        private ICommand? _deleteCustomerCommand;
        public ICommand DeleteCustomerCommand => _deleteCustomerCommand ??= 
            new RelayCommand(DeleteCustomer, CanDeleteCustomer);
        
        private bool CanDeleteCustomer() => _customers.SelectedItem != null;
        
        private void DeleteCustomer()
        {
            if (_customers.SelectedItem == null) return;
            
            var customerName = _customers.SelectedItem.Name;
            
            // Bestätigung abfragen
            if (_messageBoxService.AskYesNo(
                $"Möchten Sie den Kunden '{customerName}' wirklich löschen?",
                "Kunde löschen"))
            {
                _customers.DeleteCommand.Execute(null);
                _messageBoxService.ShowMessage($"Kunde '{customerName}' wurde gelöscht.");
            }
        }

        public ReadOnlyObservableCollection<CustomerViewModel> Customers 
            => _customers.Items;

        public ICommand AddCommand => _customers.AddCommand;
        public ICommand EditCommand => _customers.EditCommand;

        public void Dispose() => _customers.Dispose();
    }
}
```

---

### Wichtigste Regeln

1. **ViewModels erben von `ViewModelBase<TModel>`**
2. **Model als erster Constructor-Parameter**
3. **Erforderliche Services via Constructor, optionale via ServiceProvider**
4. **Factory via `services.AddViewModelFactory<TModel, TViewModel>()`**
5. **Dialog-ViewModels mit `IDialogViewModelMarker` dekorieren** ? **NEU**
6. **Dialog-Views mit `IDialogView<TViewModel>` dekorieren** ? **NEU**
7. **Assembly-Scanning via `CustomWPFControlsBootstrapDecorator`** ? **NEU**
8. **Convention-based für einfache Dialoge, Instance-based für komplexe** ? **NEU**
9. **IMessageBoxService statt MessageBox.Show()** ? **NEU**
10. **CustomWPFControls.TestHelpers für Unit-Tests** ? **NEU**
11. **WindowLayoutService persistiert automatisch Dialog-Position/Größe**
