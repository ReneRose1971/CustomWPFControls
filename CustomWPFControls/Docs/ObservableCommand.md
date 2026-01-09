# ObservableCommand

ICommand-Implementierung mit automatischen CanExecuteChanged-Benachrichtigungen bei PropertyChanged-Events.

## Übersicht

`ObservableCommand` feuert CanExecuteChanged automatisch wenn sich überwachte Properties ändern.

### Definition

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

## Contract

- Überwacht INotifyPropertyChanged-Objekte
- Feuert CanExecuteChanged automatisch bei PropertyChanged von überwachten Properties
- Selektive Überwachung via observedProperties oder vollständige Überwachung (null/leer)
- Funktioniert ohne CommandManager (Unit-Test-kompatibel)
- Manuelle Benachrichtigung via RaiseCanExecuteChanged() möglich

## Verwendung

### Spezifisches Property überwachen

```csharp
var deleteCommand = new ObservableCommand(
    execute: _ => DeleteItem(),
    canExecute: _ => SelectedItem != null,
    observedObject: this,
    observedProperties: nameof(SelectedItem));

// CanExecuteChanged wird automatisch gefeuert wenn SelectedItem sich ändert
```

### Alle Properties überwachen

```csharp
var saveCommand = new ObservableCommand(
    execute: _ => SaveChanges(),
    canExecute: _ => HasChanges && IsValid,
    observedObject: this);

// CanExecuteChanged bei JEDEM PropertyChanged
```

### Manuelle Benachrichtigung

```csharp
var command = new ObservableCommand(
    execute: _ => ProcessData(),
    canExecute: _ => CanProcess());

// Manuell auslösen
command.RaiseCanExecuteChanged();
```

## Vergleich mit RelayCommand

| Feature | RelayCommand | ObservableCommand |
|---------|--------------|-------------------|
| CanExecuteChanged | CommandManager.RequerySuggested | PropertyChanged-Events |
| PropertyChanged-Support | Nein (nur UI-Aktionen) | Ja (explizit) |
| Unit-Test-freundlich | Bedingt (CommandManager) | Ja (kein CommandManager) |
| Verwendung | Standard WPF Commands | Property-abhängige Commands |

## Siehe auch

- [EditableCollectionViewModel](EditableCollectionViewModel.md)
- [API Reference](API-Reference.md)
