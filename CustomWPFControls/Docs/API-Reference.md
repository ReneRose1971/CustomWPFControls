# API Reference - CustomWPFControls

Vollständige API-Dokumentation aller öffentlichen Typen, Methoden und Properties.

## Namespaces

- [CustomWPFControls.ViewModels](#customwpfcontrolsviewmodels)
- [CustomWPFControls.Factories](#customwpfcontrolsfactories)
- [CustomWPFControls.Controls](#customwpfcontrolscontrols)
- [CustomWPFControls.Commands](#customwpfcontrolscommands)
- [CustomWPFControls.Services](#customwpfcontrolsservices)

---

## CustomWPFControls.ViewModels

### `IViewModelWrapper<TModel>`

Interface für ViewModels, die ein Domain-Model wrappen.

```csharp
public interface IViewModelWrapper<out TModel> where TModel : class
{
    TModel Model { get; }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| **Model** | `TModel` | Das gewrappte Domain-Model (read-only) |

---

### `ViewModelBase<TModel>`

Basisklasse für alle ViewModels mit automatischem PropertyChanged.

```csharp
[AddINotifyPropertyChangedInterface]
public abstract class ViewModelBase<TModel> : IViewModelWrapper<TModel>, INotifyPropertyChanged
    where TModel : class
{
    public TModel Model { get; }
    protected ViewModelBase(TModel model);
    
    public override int GetHashCode();
    public override bool Equals(object? obj);
    public override string ToString();
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null);
}
```

[Vollständige Dokumentation](ViewModelBase.md)

---

### `CollectionViewModel<TModel, TViewModel>`

Collection-ViewModel mit **lokalem ModelStore** und automatischer Synchronisation via TransformTo.

```csharp
public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
    where TModel : class
    where TViewModel : class, IViewModelWrapper<TModel>
{
    // ?? Lokaler Store (readonly Property)
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

**Wichtige Änderungen:**
- ? **ModelStore Property** - Lokaler DataStore für diese Instanz
- ? **Constructor** - Verwendet CustomWPFServices Facade (kein dataStore Parameter mehr)
- ? **Isolation** - Jede Instanz hat eigene Daten (kein globaler Shared State)
- ? **Removed** - `AddModel()`/`RemoveModel()` Methoden (verwende `ModelStore.Add()`)

[Vollständige Dokumentation](CollectionViewModel.md)

---

### `EditableCollectionViewModel<TModel, TViewModel>`

Erweitert CollectionViewModel um Commands mit **lokalem ModelStore** und **automatischen CanExecuteChanged-Benachrichtigungen**.

```csharp
public class EditableCollectionViewModel<TModel, TViewModel> : CollectionViewModel<TModel, TViewModel>
    where TModel : class
    where TViewModel : class, IViewModelWrapper<TModel>
{
    public Func<TModel>? CreateModel { get; set; }
    public Action<TModel>? EditModel { get; set; }
    
    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand DeleteSelectedCommand { get; }
    
    public EditableCollectionViewModel(
        ICustomWPFServices services,
        IViewModelFactory<TModel, TViewModel> viewModelFactory);
}
```

**Wichtige Features:**
- ? **AddCommand** - Fügt zu lokalem ModelStore hinzu
- ? **DeleteCommand** - Verwendet `ObservableCommand`, reagiert automatisch auf `SelectedItem`-Änderungen
- ? **ClearCommand** - Verwendet `ObservableCommand`, reagiert automatisch auf `Count`-Änderungen
- ? **EditCommand** - Verwendet `ObservableCommand`, reagiert automatisch auf `SelectedItem`-Änderungen
- ? **Automatische CanExecuteChanged-Events** - Keine manuelle Benachrichtigung nötig

**Command-Verhalten:**
- **DeleteCommand** & **EditCommand**: Feuern `CanExecuteChanged` automatisch wenn `SelectedItem` sich ändert
- **ClearCommand**: Feuert `CanExecuteChanged` automatisch wenn `Count` sich ändert
- **Testbar**: Funktioniert in Unit-Tests ohne WPF CommandManager

[Vollständige Dokumentation](EditableCollectionViewModel.md)

---

## CustomWPFControls.Factories

### `IViewModelFactory<TModel, TViewModel>`

Factory-Interface für ViewModel-Erstellung.

```csharp
public interface IViewModelFactory<in TModel, out TViewModel>
    where TModel : class
    where TViewModel : class
{
    TViewModel Create(TModel model);
}
```

---

### `ViewModelFactory<TModel, TViewModel>`

DI-basierte Factory-Implementierung.

```csharp
public sealed class ViewModelFactory<TModel, TViewModel> : IViewModelFactory<TModel, TViewModel>
    where TModel : class
    where TViewModel : class
{
    public ViewModelFactory(IServiceProvider serviceProvider);
    public TViewModel Create(TModel model);
}
```

[Vollständige Dokumentation](ViewModelFactory.md)

---

### `ViewModelFactoryExtensions`

DI-Extensions für Factory-Registrierung.

```csharp
public static class ViewModelFactoryExtensions
{
    public static IServiceCollection AddViewModelFactory<TModel, TViewModel>(
        this IServiceCollection services)
        where TModel : class
        where TViewModel : class;
}
```

---

## CustomWPFControls.Controls

### `BaseListView`

Basisklasse für ListView mit Count-Property.

```csharp
public class BaseListView : ListView
{
    public int Count { get; set; }
}
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| **Count** | `int` | Anzahl der Items (auto-updated) |

---

### `ListEditorView`

ListView mit integrierten CRUD-Buttons.

```csharp
public class ListEditorView : BaseListView
{
    // Command Properties
    public ICommand? AddCommand { get; set; }
    public ICommand? EditCommand { get; set; }
    public ICommand? DeleteCommand { get; set; }
    public ICommand? ClearCommand { get; set; }
    
    // Visibility Properties
    public bool IsAddVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
    public bool IsClearVisible { get; set; }
    
    // Button Text Properties
    public string AddButtonText { get; set; }
    public string EditButtonText { get; set; }
    public string DeleteButtonText { get; set; }
    public string ClearButtonText { get; set; }
}
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **AddCommand** | `ICommand` | null | Command zum Hinzufügen |
| **EditCommand** | `ICommand` | null | Command zum Bearbeiten |
| **DeleteCommand** | `ICommand` | null | Command zum Löschen |
| **ClearCommand** | `ICommand` | null | Command zum Leeren |
| **IsAddVisible** | `bool` | true | Sichtbarkeit Add-Button |
| **IsEditVisible** | `bool` | true | Sichtbarkeit Edit-Button |
| **IsDeleteVisible** | `bool` | true | Sichtbarkeit Delete-Button |
| **IsClearVisible** | `bool` | true | Sichtbarkeit Clear-Button |
| **AddButtonText** | `string` | "Hinzufügen" | Text Add-Button |
| **EditButtonText** | `string` | "Bearbeiten" | Text Edit-Button |
| **DeleteButtonText** | `string` | "Löschen" | Text Delete-Button |
| **ClearButtonText** | `string` | "Alle löschen" | Text Clear-Button |

[Vollständige Dokumentation](Controls-Guide.md#listeditorview)

---

### `ButtonPlacement`

Enum für Button-Positionierung in DropDownEditorView.

```csharp
public enum ButtonPlacement
{
    Right,   // Buttons rechts neben Control
    Bottom,  // Buttons unter Control
    Top      // Buttons in ToolBar über Control
}
```

---

### `BaseComboBoxView`

Basisklasse für ComboBox mit Count-Property.

```csharp
public class BaseComboBoxView : ComboBox
{
    public int Count { get; set; }
}
```

---

### `DropDownEditorView`

ComboBox mit integrierten CRUD-Buttons und konfigurierbarem Layout.

```csharp
public class DropDownEditorView : BaseComboBoxView
{
    // Layout
    public ButtonPlacement ButtonPlacement { get; set; }
    
    // Command Properties
    public ICommand? AddCommand { get; set; }
    public ICommand? EditCommand { get; set; }
    public ICommand? DeleteCommand { get; set; }
    public ICommand? ClearCommand { get; set; }
    
    // Visibility Properties
    public bool IsAddVisible { get; set; }
    public bool IsEditVisible { get; set; }
    public bool IsDeleteVisible { get; set; }
    public bool IsClearVisible { get; set; }
    
    // Button Text Properties
    public string AddButtonText { get; set; }
    public string EditButtonText { get; set; }
    public string DeleteButtonText { get; set; }
    public string ClearButtonText { get; set; }
}
```

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| **ButtonPlacement** | `ButtonPlacement` | Right | Position der Buttons |
| **IsClearVisible** | `bool` | false | Clear für ComboBox unüblich |

[Vollständige Dokumentation](Controls-Guide.md#dropdowneditorview)

---

## CustomWPFControls.Commands

### `RelayCommand`

Standard ICommand-Implementierung mit CommandManager.RequerySuggested.

```csharp
public class RelayCommand : ICommand
{
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null);
    
    public bool CanExecute(object? parameter);
    public void Execute(object? parameter);
    public event EventHandler? CanExecuteChanged;
}
```

**CanExecuteChanged-Verhalten:**
- Verwendet `CommandManager.RequerySuggested`
- Automatische Re-Evaluation bei UI-Aktionen (Focus, Mausklicks)
- **Nicht automatisch** bei PropertyChanged-Events

#### Example

```csharp
var command = new RelayCommand(
    execute: _ => DoSomething(),
    canExecute: _ => CanDoSomething());
```

---

### `ObservableCommand`

ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

```csharp
public class ObservableCommand : ICommand
{
    public ObservableCommand(
        Action<object?> execute, 
        Func<object?, bool>? canExecute = null,
        INotifyPropertyChanged? observedObject = null,
        params string[]? observedProperties);
    
    public bool CanExecute(object? parameter);
    public void Execute(object? parameter);
    public void RaiseCanExecuteChanged();
    public event EventHandler? CanExecuteChanged;
}
```

**Features:**
- ? **PropertyChanged-Monitoring**: Überwacht INotifyPropertyChanged-Objekte
- ? **Selektive Überwachung**: Nur spezifische Properties überwachen
- ? **Testfreundlich**: Funktioniert ohne CommandManager (Unit-Test-kompatibel)
- ? **Explizite Benachrichtigung**: `RaiseCanExecuteChanged()` für manuelle Steuerung

#### Constructor Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| **execute** | `Action<object?>` | Die auszuführende Aktion (required) |
| **canExecute** | `Func<object?, bool>?` | Funktion zur CanExecute-Prüfung (optional) |
| **observedObject** | `INotifyPropertyChanged?` | Zu überwachendes Objekt (optional) |
| **observedProperties** | `params string[]?` | Zu überwachende Property-Namen. Null/leer = alle Properties |

#### Example - Spezifisches Property überwachen

```csharp
var command = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,  // ViewModel
    observedProperties: nameof(SelectedItem));  // Nur SelectedItem überwachen

// CanExecuteChanged wird automatisch gefeuert wenn SelectedItem sich ändert
```

#### Example - Alle Properties überwachen

```csharp
var command = new ObservableCommand(
    execute: _ => SaveChanges(),
    canExecute: _ => HasChanges && IsValid,
    observedObject: this);  // Überwacht alle PropertyChanged-Events

// CanExecuteChanged wird bei JEDEM PropertyChanged gefeuert
```

#### Example - Manuelle Benachrichtigung

```csharp
var command = new ObservableCommand(
    execute: _ => ProcessData(),
    canExecute: _ => CanProcess());

// Manuell CanExecuteChanged auslösen
command.RaiseCanExecuteChanged();
```

#### Vergleich: RelayCommand vs ObservableCommand

| Feature | RelayCommand | ObservableCommand |
|---------|--------------|-------------------|
| CanExecuteChanged | CommandManager.RequerySuggested | PropertyChanged-Events |
| PropertyChanged-Support | ? Nein (nur UI-Aktionen) | ? Ja (explizit) |
| Unit-Test-freundlich | ?? Bedingt (benötigt CommandManager) | ? Ja (kein CommandManager) |
| Performance | Gut (lazy evaluation) | Sehr gut (selective monitoring) |
| Verwendung | Standard WPF Commands | Property-abhängige Commands |

**Wann welches Command verwenden:**
- **RelayCommand**: Standard-Szenarien, keine Property-Abhängigkeiten
- **ObservableCommand**: Commands die auf Property-Änderungen reagieren sollen (z.B. DeleteCommand bei SelectedItem-Änderung)

---

### `AsyncRelayCommand`

Asynchrone ICommand-Implementierung.

```csharp
public class AsyncRelayCommand : ICommand
{
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null);
    
    public bool CanExecute(object? parameter);
    public async void Execute(object? parameter);
    public event EventHandler? CanExecuteChanged;
}
```

#### Example

```csharp
var command = new AsyncRelayCommand(
    execute: async () => await LoadDataAsync(),
    canExecute: () => !IsLoading);
```

---

## CustomWPFControls.Services

### `IDialogService`

Service für modale Dialoge.

```csharp
public interface IDialogService
{
    bool? ShowDialog(IDialogViewModel viewModel);
    void ShowWindow(IDialogViewModel viewModel);
}
```

---

### `IMessageBoxService`

Service für MessageBoxes.

```csharp
public interface IMessageBoxService
{
    MessageBoxResult Show(string message, string caption = "", 
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None);
}
```

---

### `IWindowLayoutService`

Service für Window-Layout-Persistierung.

```csharp
public interface IWindowLayoutService
{
    void SaveLayout(Window window, string key);
    void RestoreLayout(Window window, string key);
}
```

---

## Type Constraints

### TModel Constraints

```csharp
where TModel : class
```

- Muss Referenztyp sein
- Für IDataStore kompatibel

### TViewModel Constraints

```csharp
where TViewModel : class, IViewModelWrapper<TModel>
```

- Muss Referenztyp sein
- Muss IViewModelWrapper implementieren
- Typischerweise von ViewModelBase abgeleitet

---

## Code-Beispiele

### ViewModel erstellen

```csharp
public class ProductViewModel : ViewModelBase<Product>
{
    public ProductViewModel(Product model, IDialogService dialogService) 
        : base(model)
    {
        // Model als erster Parameter
        // Weitere Dependencies via DI
    }
    
    public string Name => Model.Name;
    public decimal Price => Model.Price;
    public bool IsSelected { get; set; }
}
```

### Factory registrieren

```csharp
services.AddViewModelFactory<Product, ProductViewModel>();
```

### CollectionViewModel verwenden

```csharp
var viewModel = new CollectionViewModel<Product, ProductViewModel>(
    dataStores,
    viewModelFactory,
    comparerService);
```

### Control in XAML

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Products}"
    SelectedItem="{Binding SelectedProduct}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

---

## Siehe auch

- [Getting Started](Getting-Started.md)
- [Architecture](Architecture.md)
- [Controls-Guide](Controls-Guide.md)
- [Best Practices](Best-Practices.md)
