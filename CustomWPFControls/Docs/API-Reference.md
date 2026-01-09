# API Reference - CustomWPFControls

API-Dokumentation aller öffentlichen Typen.

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

---

### `ViewModelBase<TModel>`

Basisklasse für ViewModels mit PropertyChanged-Support via Fody.

```csharp
[AddINotifyPropertyChangedInterface]
public abstract class ViewModelBase<TModel> : IViewModelWrapper<TModel>, INotifyPropertyChanged
    where TModel : class
{
    public TModel Model { get; }
    protected ViewModelBase(TModel model);
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null);
}
```

[Detaillierte Dokumentation](ViewModelBase.md)

---

### `CollectionViewModel<TModel, TViewModel>`

Collection-ViewModel mit lokalem ModelStore und automatischer Synchronisation via TransformTo.

```csharp
public class CollectionViewModel<TModel, TViewModel> : INotifyPropertyChanged, IDisposable
    where TModel : class
    where TViewModel : class, IViewModelWrapper<TModel>
{
    public IDataStore<TModel> ModelStore { get; }
    public ReadOnlyObservableCollection<TViewModel> Items { get; }
    public TViewModel? SelectedItem { get; set; }
    public ObservableCollection<TViewModel> SelectedItems { get; }
    public int Count { get; }
    
    public bool Remove(TViewModel item);
    public int RemoveRange(IEnumerable<TViewModel> items);
    public void Clear();
    public void LoadModels(IEnumerable<TModel> models);
    
    public CollectionViewModel(
        ICustomWPFServices services,
        IViewModelFactory<TModel, TViewModel> viewModelFactory);
}
```

**Contract:**
- Jede Instanz hat eigenen lokalen ModelStore
- Items sind automatisch mit ModelStore synchronisiert
- Remove/Clear invalidieren Selection automatisch
- LoadModels ersetzt alle Models (Clear + AddRange)

[Detaillierte Dokumentation](CollectionViewModel.md)

---

### `EditableCollectionViewModel<TModel, TViewModel>`

Erweitert CollectionViewModel um Commands mit automatischen CanExecuteChanged-Benachrichtigungen.

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

**Contract:**
- AddCommand fügt zu lokalem ModelStore hinzu
- DeleteCommand/EditCommand reagieren automatisch auf SelectedItem-Änderungen
- ClearCommand reagiert automatisch auf Count-Änderungen
- CreateModel und EditModel müssen für Commands gesetzt sein

[Detaillierte Dokumentation](EditableCollectionViewModel.md)

---

## CustomWPFControls.Factories

### `IViewModelFactory<TModel, TViewModel>`

Factory-Interface für DI-basierte ViewModel-Erstellung.

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

[Detaillierte Dokumentation](ViewModelFactory.md)

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

ListView mit Count-Property.

```csharp
public class BaseListView : ListView
{
    public int Count { get; set; }
}
```

---

### `ListEditorView`

ListView mit integrierten CRUD-Buttons.

```csharp
public class ListEditorView : BaseListView
{
    public ICommand? AddCommand { get; set; }
    public ICommand? EditCommand { get; set; }
    public ICommand? DeleteCommand { get; set; }
    public ICommand? ClearCommand { get; set; }
    
    public bool IsAddVisible { get; set; }      // Default: true
    public bool IsEditVisible { get; set; }     // Default: true
    public bool IsDeleteVisible { get; set; }   // Default: true
    public bool IsClearVisible { get; set; }    // Default: true
    
    public string AddButtonText { get; set; }      // Default: "Hinzufügen"
    public string EditButtonText { get; set; }     // Default: "Bearbeiten"
    public string DeleteButtonText { get; set; }   // Default: "Löschen"
    public string ClearButtonText { get; set; }    // Default: "Alle löschen"
}
```

[Detaillierte Dokumentation](Controls-Guide.md#listeditorview)

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

ComboBox mit Count-Property.

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
    public ButtonPlacement ButtonPlacement { get; set; }  // Default: Right
    
    public ICommand? AddCommand { get; set; }
    public ICommand? EditCommand { get; set; }
    public ICommand? DeleteCommand { get; set; }
    public ICommand? ClearCommand { get; set; }
    
    public bool IsAddVisible { get; set; }      // Default: true
    public bool IsEditVisible { get; set; }     // Default: true
    public bool IsDeleteVisible { get; set; }   // Default: true
    public bool IsClearVisible { get; set; }    // Default: false
    
    public string AddButtonText { get; set; }
    public string EditButtonText { get; set; }
    public string DeleteButtonText { get; set; }
    public string ClearButtonText { get; set; }
}
```

[Detaillierte Dokumentation](Controls-Guide.md#dropdowneditorview)

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

**Contract:**
- CanExecuteChanged via CommandManager.RequerySuggested
- Automatische Re-Evaluation bei UI-Aktionen
- Nicht automatisch bei PropertyChanged-Events

---

### `ObservableCommand`

ICommand mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

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

**Contract:**
- Überwacht INotifyPropertyChanged-Objekte
- Selektive oder vollständige Property-Überwachung
- Funktioniert ohne CommandManager (Unit-Test-kompatibel)
- Manuelle Benachrichtigung via RaiseCanExecuteChanged()

**Parameter:**
- `observedObject`: Zu überwachendes Objekt (optional)
- `observedProperties`: Property-Namen. Null/leer = alle Properties

[Detaillierte Dokumentation](ObservableCommand.md)

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

## Siehe auch

- [Getting Started](Getting-Started.md)
- [Architecture](Architecture.md)
- [Controls Guide](Controls-Guide.md)
- [Best Practices](Best-Practices.md)
