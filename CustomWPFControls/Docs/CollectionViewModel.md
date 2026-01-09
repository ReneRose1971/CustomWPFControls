# CollectionViewModel<TModel, TViewModel>

Collection-ViewModel mit lokalem DataStore und automatischer Synchronisation via TransformTo.

## Übersicht

`CollectionViewModel<TModel, TViewModel>` synchronisiert lokalen ModelStore und ViewModels automatisch.

### Definition

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

## Contract

- Jede Instanz hat eigenen lokalen ModelStore
- Items sind automatisch mit ModelStore synchronisiert via TransformTo
- Remove/Clear invalidieren Selection automatisch
- LoadModels ersetzt alle Models (Clear + AddRange)
- ViewModels werden automatisch erstellt und disposed

## Verwendung

### Minimal-Beispiel

```csharp
var viewModel = new CollectionViewModel<Customer, CustomerViewModel>(
    services, factory);

// Models zum lokalen Store hinzufügen
viewModel.ModelStore.Add(new Customer { Name = "Alice" });

// ViewModel wird automatisch erstellt
Assert.Equal(1, viewModel.Count);
Assert.Equal("Alice", viewModel.Items[0].Name);
```

### LoadModels

```csharp
var customers = new[] 
{
    new Customer { Name = "Alice" },
    new Customer { Name = "Bob" }
};

viewModel.LoadModels(customers);

// Ersetzt alle Items
Assert.Equal(2, viewModel.Count);
```

### Remove

```csharp
var item = viewModel.Items[0];
bool removed = viewModel.Remove(item);

// Model und ViewModel entfernt, Selection invalidiert
Assert.True(removed);
Assert.Equal(0, viewModel.Count);
```

## Lokaler ModelStore

Jede CollectionViewModel-Instanz hat eigenen isolierten ModelStore.

```csharp
var vm1 = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);
var vm2 = new CollectionViewModel<Customer, CustomerViewModel>(services, factory);

vm1.ModelStore.Add(new Customer { Name = "Alice" });

// vm2 ist unabhängig
Assert.Equal(1, vm1.Count);
Assert.Equal(0, vm2.Count);
```

## TransformTo-Integration

ViewModels werden automatisch via TransformTo synchronisiert.

```csharp
// Add
viewModel.ModelStore.Add(model);
// -> ViewModel automatisch erstellt und zu Items hinzugefügt

// Remove
viewModel.ModelStore.Remove(model);
// -> ViewModel automatisch aus Items entfernt und disposed
```

## Siehe auch

- [EditableCollectionViewModel](EditableCollectionViewModel.md)
- [ViewModelBase](ViewModelBase.md)
- [API Reference](API-Reference.md)
