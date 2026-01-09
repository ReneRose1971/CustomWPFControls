# CustomWPFControls

MVVM-Framework für WPF mit DataStore-Integration und automatischer PropertyChanged-Support via Fody.

## Überblick

**CustomWPFControls** bietet:
- Bidirektionale Synchronisation zwischen DataStore und ViewModels
- ViewModelFactory für DI-basierte ViewModel-Erstellung
- Collection-ViewModels mit automatischer Synchronisation
- Editor-Controls (ListEditorView, DropDownEditorView)
- Command-Implementierungen (RelayCommand, ObservableCommand, AsyncRelayCommand)
- Dialog- und MessageBox-Services

## Installation

### Voraussetzungen
- .NET 8.0
- WPF-Projekt

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

## Verwendung

### ViewModel-Erstellung
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
services.AddViewModelFactory<Customer, CustomerViewModel>();
services.AddSingleton<IEqualityComparer<Customer>>(
    new FallbackEqualsComparer<Customer>());
```

### XAML-Binding
```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    AddCommand="{Binding AddCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

## Dokumentation

Detaillierte Dokumentation im [Docs](Docs/) Verzeichnis:

- [Getting Started](Docs/Getting-Started.md)
- [Architecture](Docs/Architecture.md)
- [API Reference](Docs/API-Reference.md)
- [Controls Guide](Docs/Controls-Guide.md)
- [Best Practices](Docs/Best-Practices.md)

### Komponentenspezifisch
- [ViewModelBase](Docs/ViewModelBase.md)
- [CollectionViewModel](Docs/CollectionViewModel.md)
- [EditableCollectionViewModel](Docs/EditableCollectionViewModel.md)
- [ViewModelFactory](Docs/ViewModelFactory.md)
- [ObservableCommand](Docs/ObservableCommand.md)

## Lizenz

Siehe LICENSE-Datei im Repository.
