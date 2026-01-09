# EditableCollectionViewModel<TModel, TViewModel>

Erweitert CollectionViewModel um Commands für CRUD-Operationen mit automatischen CanExecuteChanged-Benachrichtigungen.

## Übersicht

`EditableCollectionViewModel<TModel, TViewModel>` erweitert CollectionViewModel um ICommand-Properties.

### Definition

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

## Contract

- **AddCommand**: Fügt Model via CreateModel() zu lokalem ModelStore hinzu
- **DeleteCommand**: Entfernt SelectedItem, reagiert automatisch auf SelectedItem-Änderungen
- **ClearCommand**: Entfernt alle Items, reagiert automatisch auf Count-Änderungen
- **EditCommand**: Ruft EditModel() auf, reagiert automatisch auf SelectedItem-Änderungen
- CreateModel und EditModel müssen gesetzt sein für Commands

## Verwendung

### Minimal-Beispiel

```csharp
var viewModel = new EditableCollectionViewModel<Customer, CustomerViewModel>(
    services, factory);

viewModel.CreateModel = () => new Customer { Name = "Neu" };
viewModel.EditModel = customer => OpenEditDialog(customer);

// Commands verwenden
viewModel.AddCommand.Execute(null);
Assert.Equal(1, viewModel.Count);
```

### XAML-Binding

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    SelectedItem="{Binding SelectedItem, Mode=TwoWay}"
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

### Mit ViewModel-Wrapper

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
        _customers.EditModel = c => OpenDialog(c);
    }
    
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand DeleteCommand => _customers.DeleteCommand;
}
```

## Automatische CanExecuteChanged-Events

Commands verwenden ObservableCommand für automatische CanExecuteChanged-Benachrichtigungen:

- **DeleteCommand/EditCommand**: Reagieren automatisch auf SelectedItem-Änderungen
- **ClearCommand**: Reagiert automatisch auf Count-Änderungen
- Keine manuelle RaiseCanExecuteChanged() notwendig
- Funktioniert in Unit-Tests ohne CommandManager

```csharp
// SelectedItem setzen -> CanExecuteChanged automatisch gefeuert
viewModel.SelectedItem = customer;
// DeleteCommand und EditCommand werden automatisch aktualisiert
```

## Siehe auch

- [CollectionViewModel](CollectionViewModel.md)
- [ObservableCommand](ObservableCommand.md)
- [API Reference](API-Reference.md)
