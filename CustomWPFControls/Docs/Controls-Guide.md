# Controls Guide - CustomWPFControls

Dokumentation der Editor-Controls für CRUD-Operationen.

## Übersicht

CustomWPFControls bietet zwei Editor-Controls:

- **ListEditorView**: ListView mit CRUD-Buttons
- **DropDownEditorView**: ComboBox mit CRUD-Buttons und konfigurierbarem Layout

Beide Controls funktionieren mit `EditableCollectionViewModel`.

## ListEditorView

ListView mit integrierten Action-Buttons unterhalb der Liste.

### Verwendung

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    SelectedItem="{Binding SelectedCustomer}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"
    ClearCommand="{Binding ClearCommand}"/>
```

### Properties

- **Commands**: `AddCommand`, `EditCommand`, `DeleteCommand`, `ClearCommand`
- **Visibility**: `IsAddVisible`, `IsEditVisible`, `IsDeleteVisible`, `IsClearVisible` (Default: true)
- **Text**: `AddButtonText`, `EditButtonText`, `DeleteButtonText`, `ClearButtonText`
- **Count**: Anzahl der Items (read-only)

## DropDownEditorView

ComboBox mit integrierten Action-Buttons und konfigurierbarem Button-Layout.

### Verwendung

```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    SelectedItem="{Binding SelectedCategory}"
    ButtonPlacement="Right"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

### Properties

Zusätzlich zu ListEditorView-Properties:

- **ButtonPlacement**: `Right`, `Bottom`, `Top` (Default: Right)

### ButtonPlacement

```csharp
public enum ButtonPlacement
{
    Right,   // Buttons rechts neben ComboBox
    Bottom,  // Buttons unter ComboBox
    Top      // Buttons in ToolBar über ComboBox
}
```

## Integration mit ViewModels

```csharp
public class CustomerListViewModel
{
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        ICustomWPFServices services,
        IViewModelFactory<Customer, CustomerViewModel> factory)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            services, factory);
        
        _customers.CreateModel = () => new Customer();
        _customers.EditModel = customer => OpenEditDialog(customer);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;
    public ICommand DeleteCommand => _customers.DeleteCommand;
}
```

### XAML-Binding

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Customers}"
    SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"
    AddCommand="{Binding AddCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

## Contract

- Commands werden automatisch disabled wenn nicht verfügbar (CanExecute)
- SelectedItem-Binding sollte `Mode=TwoWay` verwenden
- ButtonPlacement bestimmt Layout bei DropDownEditorView
- Count-Property ist read-only und automatisch aktualisiert

## Siehe auch

- [EditableCollectionViewModel](EditableCollectionViewModel.md)
- [API Reference](API-Reference.md)
