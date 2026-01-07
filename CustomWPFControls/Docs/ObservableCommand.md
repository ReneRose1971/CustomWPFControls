# ObservableCommand

ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

## ?? Inhaltsverzeichnis

- [Überblick](#überblick)
- [Problem & Lösung](#problem--lösung)
- [Constructor](#constructor)
- [Features](#features)
- [Verwendung](#verwendung)
- [Vergleich mit RelayCommand](#vergleich-mit-relaycommand)
- [Best Practices](#best-practices)
- [Beispiele](#beispiele)

---

## Überblick

`ObservableCommand` ist eine ICommand-Implementierung, die **automatisch `CanExecuteChanged` feuert**, wenn sich überwachte Properties ändern.

### Definition

```csharp
namespace CustomWPFControls.Commands;

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

---

## Problem & Lösung

### ? Problem mit RelayCommand

**RelayCommand** verwendet `CommandManager.RequerySuggested`, das nur bei UI-Aktionen neu evaluiert wird:

```csharp
public ICommand DeleteCommand { get; }

// Setup mit RelayCommand
DeleteCommand = new RelayCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null);

// Problem:
SelectedItem = someItem;
// ? CanExecuteChanged wird NICHT automatisch gefeuert!
// ? Button bleibt disabled bis Focus-Wechsel oder Mausklick
// ? In Unit-Tests: CommandManager.InvalidateRequerySuggested() manuell aufrufen
```

### ? Lösung mit ObservableCommand

**ObservableCommand** überwacht explizit PropertyChanged-Events:

```csharp
// Setup mit ObservableCommand
DeleteCommand = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,                    // ViewModel
    observedProperties: nameof(SelectedItem)); // Überwacht SelectedItem

// Lösung:
SelectedItem = someItem;
// ? CanExecuteChanged wird AUTOMATISCH gefeuert!
// ? Button wird sofort enabled
// ? Funktioniert in Unit-Tests ohne CommandManager
```

---

## Constructor

### Parameter

```csharp
public ObservableCommand(
    Action<object?> execute,                    // Required
    Func<object?, bool>? canExecute = null,     // Optional
    INotifyPropertyChanged? observedObject = null,   // Optional
    params string[]? observedProperties)        // Optional
```

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| **execute** | `Action<object?>` | ? Ja | Die auszuführende Aktion |
| **canExecute** | `Func<object?, bool>?` | ? Nein | Funktion zur CanExecute-Prüfung (default: `true`) |
| **observedObject** | `INotifyPropertyChanged?` | ? Nein | Zu überwachendes Objekt (z.B. ViewModel) |
| **observedProperties** | `params string[]?` | ? Nein | Zu überwachende Property-Namen. Null/leer = alle Properties |

### Verhalten

**Mit observedObject und observedProperties:**
```csharp
var command = new ObservableCommand(
    execute: _ => Save(),
    canExecute: _ => IsDirty,
    observedObject: this,
    observedProperties: nameof(IsDirty));

// IsDirty ändert sich ? CanExecuteChanged wird gefeuert
// Andere Properties ändern sich ? kein Event
```

**Mit observedObject ohne observedProperties:**
```csharp
var command = new ObservableCommand(
    execute: _ => Save(),
    canExecute: _ => IsDirty && IsValid,
    observedObject: this);  // Überwacht ALLE Properties

// JEDES PropertyChanged ? CanExecuteChanged wird gefeuert
```

**Ohne observedObject:**
```csharp
var command = new ObservableCommand(
    execute: _ => Save(),
    canExecute: _ => CanSave());

// Kein automatisches CanExecuteChanged
// Manuelle Benachrichtigung via RaiseCanExecuteChanged() möglich
```

---

## Features

### ? PropertyChanged-Monitoring

Überwacht `INotifyPropertyChanged.PropertyChanged`-Events:

```csharp
public class ViewModel : INotifyPropertyChanged
{
    private bool _isValid;
    
    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
            }
        }
    }
    
    public ICommand SaveCommand { get; }
    
    public ViewModel()
    {
        SaveCommand = new ObservableCommand(
            execute: _ => Save(),
            canExecute: _ => IsValid,
            observedObject: this,
            observedProperties: nameof(IsValid));
    }
    
    private void ValidateData()
    {
        IsValid = CheckValidation();
        // ? SaveCommand.CanExecuteChanged wird AUTOMATISCH gefeuert!
    }
}
```

### ? Selektive Überwachung

Überwacht nur spezifische Properties für optimale Performance:

```csharp
// Nur SelectedItem überwachen
var deleteCommand = new ObservableCommand(
    execute: _ => Delete(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,
    observedProperties: nameof(SelectedItem));  // Nur SelectedItem!

// Andere Property-Änderungen werden ignoriert
Count = 42;  // ? Kein CanExecuteChanged
IsLoading = true;  // ? Kein CanExecuteChanged
SelectedItem = item;  // ? CanExecuteChanged gefeuert!
```

### ? Mehrere Properties überwachen

```csharp
var command = new ObservableCommand(
    execute: _ => ProcessData(),
    canExecute: _ => IsValid && !IsLoading && HasData,
    observedObject: this,
    observedProperties: new[] { nameof(IsValid), nameof(IsLoading), nameof(HasData) });

// Änderung an EINEM der drei Properties ? CanExecuteChanged
```

### ? Testfreundlich

Funktioniert **ohne WPF CommandManager**:

```csharp
[Fact]
public void CanExecuteChanged_Fires_WhenPropertyChanges()
{
    // Arrange
    var viewModel = new TestViewModel();
    bool eventFired = false;
    viewModel.DeleteCommand.CanExecuteChanged += (s, e) => eventFired = true;
    
    // Act
    viewModel.SelectedItem = new Item();
    
    // Assert
    Assert.True(eventFired);  // ? Funktioniert ohne CommandManager!
}
```

### ? Manuelle Benachrichtigung

```csharp
var command = new ObservableCommand(_ => Save(), _ => CanSave());

// Manuell CanExecuteChanged auslösen
command.RaiseCanExecuteChanged();
```

---

## Verwendung

### Beispiel 1: DeleteCommand in ViewModel

```csharp
public class CustomerListViewModel : INotifyPropertyChanged
{
    private CustomerViewModel? _selectedCustomer;
    
    public CustomerViewModel? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (_selectedCustomer != value)
            {
                _selectedCustomer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCustomer)));
            }
        }
    }
    
    public ICommand DeleteCommand { get; }
    
    public CustomerListViewModel()
    {
        DeleteCommand = new ObservableCommand(
            execute: _ => DeleteSelectedCustomer(),
            canExecute: _ => SelectedCustomer != null,
            observedObject: this,
            observedProperties: nameof(SelectedCustomer));
    }
    
    private void DeleteSelectedCustomer()
    {
        if (SelectedCustomer != null)
        {
            // Löschen-Logik
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

**UI-Verhalten:**
```csharp
// User wählt Item aus
SelectedCustomer = customers[0];
// ? DeleteCommand.CanExecuteChanged wird gefeuert
// ? Delete-Button wird enabled

// User deselektiert Item
SelectedCustomer = null;
// ? DeleteCommand.CanExecuteChanged wird gefeuert
// ? Delete-Button wird disabled
```

### Beispiel 2: Mehrere Commands mit gleicher Property

```csharp
public class OrderViewModel : INotifyPropertyChanged
{
    private Order? _selectedOrder;
    
    public Order? SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            if (_selectedOrder != value)
            {
                _selectedOrder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedOrder)));
            }
        }
    }
    
    public ICommand EditCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand DuplicateCommand { get; }
    
    public OrderViewModel()
    {
        // Alle drei Commands überwachen SelectedOrder
        EditCommand = new ObservableCommand(
            execute: _ => EditOrder(),
            canExecute: _ => SelectedOrder != null,
            observedObject: this,
            observedProperties: nameof(SelectedOrder));
        
        DeleteCommand = new ObservableCommand(
            execute: _ => DeleteOrder(),
            canExecute: _ => SelectedOrder != null,
            observedObject: this,
            observedProperties: nameof(SelectedOrder));
        
        DuplicateCommand = new ObservableCommand(
            execute: _ => DuplicateOrder(),
            canExecute: _ => SelectedOrder != null,
            observedObject: this,
            observedProperties: nameof(SelectedOrder));
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}

// SelectedOrder ändert sich
// ? ALLE drei Commands feuern CanExecuteChanged!
```

### Beispiel 3: Komplexe CanExecute-Logik

```csharp
public class DataViewModel : INotifyPropertyChanged
{
    private bool _isValid;
    private bool _isLoading;
    private bool _hasChanges;
    
    public bool IsValid
    {
        get => _isValid;
        set { _isValid = value; OnPropertyChanged(); }
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }
    
    public bool HasChanges
    {
        get => _hasChanges;
        set { _hasChanges = value; OnPropertyChanged(); }
    }
    
    public ICommand SaveCommand { get; }
    
    public DataViewModel()
    {
        SaveCommand = new ObservableCommand(
            execute: _ => SaveData(),
            canExecute: _ => IsValid && !IsLoading && HasChanges,
            observedObject: this,
            observedProperties: new[] 
            { 
                nameof(IsValid), 
                nameof(IsLoading), 
                nameof(HasChanges) 
            });
    }
    
    // CanExecute wird neu evaluiert wenn EINES der drei Properties sich ändert
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

---

## Vergleich mit RelayCommand

| Feature | RelayCommand | ObservableCommand |
|---------|--------------|-------------------|
| **CanExecuteChanged Trigger** | `CommandManager.RequerySuggested` | `INotifyPropertyChanged.PropertyChanged` |
| **Automatisch bei Property-Änderung** | ? Nein | ? Ja (wenn überwacht) |
| **UI-Events (Focus, Mausklick)** | ? Ja | ? Nein (nur PropertyChanged) |
| **Unit-Test-freundlich** | ?? Bedingt (benötigt CommandManager) | ? Ja (kein CommandManager) |
| **Performance** | Gut | Sehr gut (selective monitoring) |
| **Explizite Abhängigkeiten** | ? Nein (implizit via CommandManager) | ? Ja (observedProperties) |
| **Manuelle Benachrichtigung** | ? Nein | ? Ja (`RaiseCanExecuteChanged()`) |

### Wann welches Command verwenden?

**RelayCommand:**
- ? Standard WPF-Commands ohne Property-Abhängigkeiten
- ? Commands die nur auf UI-Events reagieren sollen
- ? Einfache Szenarien ohne komplexe CanExecute-Logik

**ObservableCommand:**
- ? Commands die auf Property-Änderungen reagieren sollen
- ? Unit-Tests ohne WPF CommandManager
- ? Explizite Property ? Command Abhängigkeiten
- ? EditableCollectionViewModel Commands (DeleteCommand, EditCommand, ClearCommand)

---

## Best Practices

### ? Do's

**1. Überwache nur relevante Properties:**
```csharp
// ? Gut: Nur SelectedItem überwachen
var command = new ObservableCommand(
    _ => Delete(),
    _ => SelectedItem != null,
    this,
    nameof(SelectedItem));  // Spezifisch!

// ? Schlecht: Alle Properties überwachen (Performance!)
var command = new ObservableCommand(
    _ => Delete(),
    _ => SelectedItem != null,
    this);  // Kein observedProperties ? ALLE Properties!
```

**2. Verwende nameof() für Property-Namen:**
```csharp
// ? Gut: Compile-time-sicher
observedProperties: nameof(SelectedItem)

// ? Schlecht: Fehleranfällig bei Refactoring
observedProperties: "SelectedItem"
```

**3. Gruppiere verwandte Commands:**
```csharp
// ? Gut: Mehrere Commands mit gleicher Abhängigkeit
EditCommand = new ObservableCommand(
    _ => Edit(), _ => SelectedItem != null, this, nameof(SelectedItem));
DeleteCommand = new ObservableCommand(
    _ => Delete(), _ => SelectedItem != null, this, nameof(SelectedItem));
```

**4. Verwende für komplexe CanExecute:**
```csharp
// ? Gut: Mehrere Properties überwachen
var command = new ObservableCommand(
    _ => Save(),
    _ => IsValid && !IsLoading && HasChanges,
    this,
    new[] { nameof(IsValid), nameof(IsLoading), nameof(HasChanges) });
```

### ? Don'ts

**1. Nicht alle Properties überwachen wenn spezifisch möglich:**
```csharp
// ? Schlecht: Overhead
var command = new ObservableCommand(_ => Delete(), _ => SelectedItem != null, this);

// ? Gut: Spezifisch
var command = new ObservableCommand(
    _ => Delete(), _ => SelectedItem != null, this, nameof(SelectedItem));
```

**2. Nicht observedObject vergessen wenn Property überwacht werden soll:**
```csharp
// ? Schlecht: observedProperties ohne observedObject
var command = new ObservableCommand(
    _ => Delete(), 
    _ => SelectedItem != null,
    null,  // observedObject ist null!
    nameof(SelectedItem));  // ? Wird ignoriert!

// ? Gut: observedObject gesetzt
var command = new ObservableCommand(
    _ => Delete(), 
    _ => SelectedItem != null,
    this,  // observedObject!
    nameof(SelectedItem));
```

**3. Nicht für Commands ohne Property-Abhängigkeit verwenden:**
```csharp
// ? Schlecht: ObservableCommand ohne Nutzen
var command = new ObservableCommand(_ => ShowHelp());

// ? Gut: RelayCommand ausreichend
var command = new RelayCommand(_ => ShowHelp());
```

---

## Beispiele

### Beispiel 1: EditableCollectionViewModel

**Wie es in EditableCollectionViewModel verwendet wird:**

```csharp
public class EditableCollectionViewModel<TModel, TViewModel> 
    : CollectionViewModel<TModel, TViewModel>
{
    private ICommand? _deleteCommand;
    
    public ICommand DeleteCommand => _deleteCommand ??= new ObservableCommand(
        execute: _ =>
        {
            if (SelectedItem != null)
            {
                Remove(SelectedItem);
            }
        },
        canExecute: _ => SelectedItem != null,
        observedObject: this,
        observedProperties: nameof(SelectedItem));
    
    // SelectedItem ändert sich ? CanExecuteChanged automatisch!
}
```

### Beispiel 2: Validierungs-Command

```csharp
public class FormViewModel : INotifyPropertyChanged
{
    private string _name = "";
    private string _email = "";
    private bool _isNameValid;
    private bool _isEmailValid;
    
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            IsNameValid = !string.IsNullOrWhiteSpace(value);
            OnPropertyChanged();
        }
    }
    
    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            IsEmailValid = IsValidEmail(value);
            OnPropertyChanged();
        }
    }
    
    public bool IsNameValid
    {
        get => _isNameValid;
        private set
        {
            _isNameValid = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsEmailValid
    {
        get => _isEmailValid;
        private set
        {
            _isEmailValid = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand SubmitCommand { get; }
    
    public FormViewModel()
    {
        SubmitCommand = new ObservableCommand(
            execute: _ => SubmitForm(),
            canExecute: _ => IsNameValid && IsEmailValid,
            observedObject: this,
            observedProperties: new[] { nameof(IsNameValid), nameof(IsEmailValid) });
    }
    
    private void SubmitForm()
    {
        // Submit-Logik
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

**XAML:**
```xml
<StackPanel>
    <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"/>
    <TextBox Text="{Binding Email, UpdateSourceTrigger=PropertyChanged}"/>
    
    <!-- Button wird automatisch enabled/disabled basierend auf Validierung -->
    <Button Content="Submit" Command="{Binding SubmitCommand}"/>
</StackPanel>
```

### Beispiel 3: Async-Operationen

```csharp
public class DataLoadViewModel : INotifyPropertyChanged
{
    private bool _isLoading;
    
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    
    public ICommand LoadCommand { get; }
    public ICommand CancelCommand { get; }
    
    public DataLoadViewModel()
    {
        // LoadCommand ist enabled wenn NICHT loading
        LoadCommand = new ObservableCommand(
            execute: _ => LoadData(),
            canExecute: _ => !IsLoading,
            observedObject: this,
            observedProperties: nameof(IsLoading));
        
        // CancelCommand ist enabled wenn loading
        CancelCommand = new ObservableCommand(
            execute: _ => CancelLoad(),
            canExecute: _ => IsLoading,
            observedObject: this,
            observedProperties: nameof(IsLoading));
    }
    
    private async void LoadData()
    {
        IsLoading = true;
        // ? LoadCommand disabled, CancelCommand enabled (automatisch!)
        
        try
        {
            await Task.Delay(5000);  // Simulate loading
        }
        finally
        {
            IsLoading = false;
            // ? LoadCommand enabled, CancelCommand disabled (automatisch!)
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

---

## Siehe auch

- ?? [RelayCommand](API-Reference.md#relaycommand) - Standard ICommand-Implementierung
- ?? [EditableCollectionViewModel](EditableCollectionViewModel.md) - Verwendet ObservableCommand
- ?? [API Reference](API-Reference.md) - Vollständige API-Dokumentation
- ?? [Best Practices](Best-Practices.md) - Allgemeine Tipps
