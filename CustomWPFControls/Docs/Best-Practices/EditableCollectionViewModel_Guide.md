# EditableCollectionViewModel - Best Practices Guide

Umfassender Leitfaden für die Verwendung von `EditableCollectionViewModel<TModel, TViewModel>` mit Commands für CRUD-Operationen.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [DI-Registrierung mit AddViewModelPackage](#di-registrierung-mit-addviewmodelpackage)
- [Commands im Detail](#commands-im-detail)
- [CreateModel und EditModel Delegates](#createmodel-und-editmodel-delegates)
- [Erweiterte Command-Szenarien](#erweiterte-command-szenarien)
- [Integration mit Custom Commands](#integration-mit-custom-commands)
- [Praktische Beispiele](#praktische-beispiele)

---

## Überblick

`EditableCollectionViewModel<TModel, TViewModel>` erweitert `CollectionViewModel` um vollständige CRUD-Funktionalität via Commands:

- **AddCommand**: Fügt neue Elemente über CreateModel-Delegate hinzu
- **DeleteCommand**: Löscht das ausgewählte Element
- **ClearCommand**: Löscht alle Elemente
- **EditCommand**: Bearbeitet das ausgewählte Element über EditModel-Delegate
- **DeleteSelectedCommand**: Löscht alle ausgewählten Elemente (Multi-Selection)

Alle Commands arbeiten mit dem **lokalen ModelStore** der CollectionViewModel-Instanz.

---

## DI-Registrierung mit AddViewModelPackage

### Was wird registriert?

Die `AddViewModelPackage` Extension registriert:

1. **IViewModelFactory<TModel, TViewModel>** als Singleton
2. **CollectionViewModel<TModel, TViewModel>** als Transient
3. **EditableCollectionViewModel<TModel, TViewModel>** als Transient

### Registrierung

```csharp
using CustomWPFControls.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

public class ViewModelModule : IServiceModule
{
    public void Register(IServiceCollection services)
    {
        // 1. DataStores Core Services
        var dataStoresModule = new DataStoresServiceModule();
        dataStoresModule.Register(services);
        
        // 2. CustomWPFControls Core Services
        services.AddSingleton<ICustomWPFServices, CustomWPFServices>();
        
        // 3. ViewModel Package (inkl. EditableCollectionViewModel)
        services.AddViewModelPackage<Product, ProductViewModel>();
        
        // 4. EqualityComparer
        services.AddSingleton<IEqualityComparer<Product>>(
            new FallbackEqualsComparer<Product>());
        
        // 5. Container-ViewModel
        services.AddTransient<ProductListViewModel>();
    }
}
```

### Service-Auflösung

```csharp
// EditableCollectionViewModel via DI auflösen
public class ProductListViewModel
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductListViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products)
    {
        _products = products;
        
        // Commands konfigurieren
        SetupCommands();
    }
    
    private void SetupCommands()
    {
        _products.CreateModel = () => new Product { Name = "Neues Produkt" };
        _products.EditModel = product => OpenEditDialog(product);
    }
}
```

---

## Commands im Detail

### AddCommand

Fügt ein neues Element zum lokalen ModelStore hinzu.

#### CanExecute-Logik

```csharp
// Command ist enabled wenn:
CreateModel != null
```

#### Execute-Logik

```csharp
// 1. CreateModel() aufrufen
var model = CreateModel();

// 2. Null-Check (Abbruch erlaubt)
if (model != null)
{
    // 3. Zum lokalen ModelStore hinzufügen
    ModelStore.Add(model);
    
    // 4. TransformTo erstellt automatisch ViewModel
    // 5. UI wird automatisch aktualisiert
}
```

#### Verwendung

```csharp
editableVM.CreateModel = () => new Customer 
{ 
    Name = "Neuer Kunde",
    Email = "neu@example.com",
    CreatedAt = DateTime.Now
};
```

```xml
<Button Content="Hinzufügen" Command="{Binding AddCommand}"/>
```

### DeleteCommand

Löscht das ausgewählte Element.

#### CanExecute-Logik

```csharp
// Command ist enabled wenn:
SelectedItem != null
```

#### Execute-Logik

```csharp
// 1. SelectedItem prüfen
if (SelectedItem != null)
{
    // 2. Remove-Methode der CollectionViewModel aufrufen
    Remove(SelectedItem);
    
    // 3. Model wird aus ModelStore entfernt
    // 4. ViewModel wird disposed
    // 5. SelectedItem wird auf null gesetzt
    // 6. UI wird automatisch aktualisiert
}
```

#### Verwendung

```xml
<Button Content="Löschen" Command="{Binding DeleteCommand}"/>
```

### ClearCommand

Löscht alle Elemente.

#### CanExecute-Logik

```csharp
// Command ist enabled wenn:
Count > 0
```

#### Execute-Logik

```csharp
// 1. Clear-Methode der CollectionViewModel aufrufen
Clear();

// 2. ModelStore wird geleert
// 3. Alle ViewModels werden disposed
// 4. SelectedItem = null
// 5. SelectedItems.Clear()
// 6. UI wird automatisch aktualisiert
```

#### Verwendung

```xml
<Button Content="Alle löschen" Command="{Binding ClearCommand}"/>
```

### EditCommand

Bearbeitet das ausgewählte Element.

#### CanExecute-Logik

```csharp
// Command ist enabled wenn:
SelectedItem != null && EditModel != null
```

#### Execute-Logik

```csharp
// 1. SelectedItem und EditModel prüfen
if (SelectedItem != null && EditModel != null)
{
    // 2. EditModel-Delegate mit Model aufrufen
    EditModel(SelectedItem.Model);
    
    // 3. Model-Änderungen werden durch PropertyChanged-Tracking persistiert
    // 4. ViewModel reflektiert automatisch die Änderungen
    // 5. UI wird automatisch aktualisiert
}
```

#### Verwendung

```csharp
editableVM.EditModel = customer =>
{
    var dialog = new EditCustomerDialog(customer);
    dialog.ShowDialog();
};
```

```xml
<Button Content="Bearbeiten" Command="{Binding EditCommand}"/>
```

### DeleteSelectedCommand

Löscht alle ausgewählten Elemente (Multi-Selection).

#### CanExecute-Logik

```csharp
// Command ist enabled wenn:
SelectedItems != null && SelectedItems.Count > 0
```

#### Execute-Logik

```csharp
// 1. SelectedItems prüfen
if (SelectedItems != null && SelectedItems.Count > 0)
{
    // 2. RemoveRange-Methode aufrufen
    RemoveRange(SelectedItems.ToList());
    
    // 3. Alle Models werden aus ModelStore entfernt
    // 4. Alle ViewModels werden disposed
    // 5. SelectedItems wird geleert
    // 6. UI wird automatisch aktualisiert
}
```

#### Verwendung

```xml
<ListBox SelectionMode="Multiple"
         behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}"/>
<Button Content="Ausgewählte löschen" Command="{Binding DeleteSelectedCommand}"/>
```

---

## CreateModel und EditModel Delegates

### CreateModel-Delegate

#### Einfache Objekt-Erstellung

```csharp
editableVM.CreateModel = () => new Product
{
    Id = 0,
    Name = "Neues Produkt",
    Price = 0.0m,
    CreatedAt = DateTime.Now
};
```

#### Mit Dialog

```csharp
editableVM.CreateModel = () =>
{
    var dialog = new NewProductDialog();
    var result = dialog.ShowDialog();
    
    // Null zurückgeben bei Abbruch
    return result == true ? dialog.Product : null;
};
```

#### Mit DialogService

```csharp
private readonly IDialogService _dialogService;

editableVM.CreateModel = () =>
{
    var viewModel = new ProductEditViewModel();
    var result = _dialogService.ShowDialog(viewModel);
    
    return result == true ? viewModel.ToModel() : null;
};
```

#### Mit Validierung

```csharp
private readonly IValidationService _validationService;

editableVM.CreateModel = () =>
{
    var product = new Product { Id = 0, Name = "Neu" };
    
    var errors = _validationService.Validate(product);
    if (errors.Any())
    {
        ShowValidationErrors(errors);
        return null; // Abbruch
    }
    
    return product;
};
```

### EditModel-Delegate

#### Mit Dialog

```csharp
editableVM.EditModel = product =>
{
    var dialog = new EditProductDialog(product);
    dialog.ShowDialog();
    
    // product wurde bereits im Dialog geändert
    // PropertyChanged-Tracking persistiert automatisch
};
```

#### Mit DialogService und Rollback

```csharp
editableVM.EditModel = product =>
{
    // Backup für Rollback
    var originalState = product.Clone();
    
    var viewModel = new ProductEditViewModel(product);
    var result = _dialogService.ShowDialog(viewModel);
    
    if (result == true)
    {
        // Validierung
        var errors = _validationService.Validate(product);
        if (errors.Any())
        {
            ShowValidationErrors(errors);
            product.RestoreFrom(originalState); // Rollback
        }
        else
        {
            viewModel.UpdateModel(product);
        }
    }
    else
    {
        // Abbruch: Rollback
        product.RestoreFrom(originalState);
    }
};
```

#### Navigation statt Dialog

```csharp
editableVM.EditModel = product =>
{
    // Navigation zu Edit-Page
    _navigationService.NavigateTo("ProductEdit", product);
};
```

---

## Erweiterte Command-Szenarien

### Bestätigungsdialoge

```csharp
// DeleteCommand mit Bestätigung überschreiben
var customDeleteCommand = new RelayCommand(
    _ =>
    {
        var result = _messageBoxService.Show(
            "Wirklich löschen?",
            "Bestätigung",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
        
        if (result == MessageBoxResult.Yes && editableVM.SelectedItem != null)
        {
            editableVM.Remove(editableVM.SelectedItem);
        }
    },
    _ => editableVM.SelectedItem != null);

// Custom Command verwenden statt Standard DeleteCommand
public ICommand DeleteWithConfirmationCommand => customDeleteCommand;
```

### Asynchrone Operationen

```csharp
// Async AddCommand
var asyncAddCommand = new AsyncRelayCommand(
    async _ =>
    {
        IsLoading = true;
        try
        {
            var product = await CreateProductAsync();
            if (product != null)
            {
                editableVM.ModelStore.Add(product);
            }
        }
        finally
        {
            IsLoading = false;
        }
    },
    _ => !IsLoading);

public ICommand AsyncAddCommand => asyncAddCommand;
```

### Command-Verkettung

```csharp
// AddCommand mit automatischer Selektion und Edit
var addAndEditCommand = new RelayCommand(_ =>
{
    if (editableVM.CreateModel == null) return;
    
    var model = editableVM.CreateModel();
    if (model != null)
    {
        editableVM.ModelStore.Add(model);
        
        // Neu erstelltes ViewModel finden und selektieren
        var viewModel = editableVM.Items.FirstOrDefault(
            vm => vm.Model == model);
        
        if (viewModel != null)
        {
            editableVM.SelectedItem = viewModel;
            
            // Direkt EditCommand ausführen
            if (editableVM.EditCommand.CanExecute(null))
            {
                editableVM.EditCommand.Execute(null);
            }
        }
    }
});
```

---

## Integration mit Custom Commands

### Zusätzliche Commands im ViewModel

```csharp
public class ProductListViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductListViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products)
    {
        _products = products;
        
        // Standard Commands konfigurieren
        _products.CreateModel = () => new Product();
        _products.EditModel = p => OpenEditDialog(p);
        
        // Custom Commands erstellen
        DuplicateCommand = new RelayCommand(
            _ => DuplicateSelectedProduct(),
            _ => _products.SelectedItem != null);
        
        ExportCommand = new RelayCommand(
            _ => ExportProducts(),
            _ => _products.Count > 0);
    }
    
    // Standard Commands durchreichen
    public ICommand AddCommand => _products.AddCommand;
    public ICommand EditCommand => _products.EditCommand;
    public ICommand DeleteCommand => _products.DeleteCommand;
    
    // Custom Commands
    public ICommand DuplicateCommand { get; }
    public ICommand ExportCommand { get; }
    
    private void DuplicateSelectedProduct()
    {
        if (_products.SelectedItem == null) return;
        
        var original = _products.SelectedItem.Model;
        var duplicate = new Product
        {
            Id = 0, // Neue ID
            Name = $"{original.Name} (Kopie)",
            Price = original.Price,
            Description = original.Description
        };
        
        _products.ModelStore.Add(duplicate);
    }
    
    private void ExportProducts()
    {
        // Export-Logik
    }
    
    public void Dispose()
    {
        _products.Dispose();
    }
}
```

### Command-Interaktion mit eigenem Code

```csharp
public class ProductListViewModel
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    private readonly IProductService _productService;
    
    public ProductListViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products,
        IProductService productService)
    {
        _products = products;
        _productService = productService;
        
        // CreateModel mit Service-Logik
        _products.CreateModel = () =>
        {
            // Service erstellt Product mit Default-Werten
            var product = _productService.CreateDefault();
            
            // Eigene Logik vor Dialog
            product.CreatedBy = CurrentUser.Name;
            
            var dialog = new ProductEditDialog(product);
            var result = dialog.ShowDialog();
            
            if (result == true)
            {
                // Eigene Logik nach Dialog
                _productService.ApplyBusinessRules(product);
                return product;
            }
            
            return null;
        };
        
        // EditModel mit Pre/Post-Processing
        _products.EditModel = product =>
        {
            // Pre-Processing
            var originalPrice = product.Price;
            
            var dialog = new ProductEditDialog(product);
            var result = dialog.ShowDialog();
            
            if (result == true)
            {
                // Post-Processing
                if (product.Price != originalPrice)
                {
                    // Preis-Änderungs-Logik
                    _productService.NotifyPriceChange(product, originalPrice);
                }
            }
        };
    }
}
```

### Commands mit Event-Notification

```csharp
public class ProductListViewModel
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public event EventHandler<ProductEventArgs>? ProductAdded;
    public event EventHandler<ProductEventArgs>? ProductDeleted;
    
    public ProductListViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products)
    {
        _products = products;
        
        // Wrap AddCommand mit Event
        AddWithNotificationCommand = new RelayCommand(
            _ =>
            {
                if (_products.CreateModel == null) return;
                
                var model = _products.CreateModel();
                if (model != null)
                {
                    _products.ModelStore.Add(model);
                    ProductAdded?.Invoke(this, new ProductEventArgs(model));
                }
            },
            _ => _products.CreateModel != null);
        
        // Wrap DeleteCommand mit Event
        DeleteWithNotificationCommand = new RelayCommand(
            _ =>
            {
                if (_products.SelectedItem == null) return;
                
                var model = _products.SelectedItem.Model;
                _products.Remove(_products.SelectedItem);
                ProductDeleted?.Invoke(this, new ProductEventArgs(model));
            },
            _ => _products.SelectedItem != null);
    }
    
    public ICommand AddWithNotificationCommand { get; }
    public ICommand DeleteWithNotificationCommand { get; }
}
```

---

## Praktische Beispiele

### Beispiel 1: Vollständige Produktverwaltung

```csharp
public class ProductManagementViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    private readonly IDialogService _dialogService;
    private readonly IMessageBoxService _messageBoxService;
    
    public ProductManagementViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products,
        IDialogService dialogService,
        IMessageBoxService messageBoxService)
    {
        _products = products;
        _dialogService = dialogService;
        _messageBoxService = messageBoxService;
        
        SetupCommands();
    }
    
    private void SetupCommands()
    {
        _products.CreateModel = CreateNewProduct;
        _products.EditModel = EditExistingProduct;
    }
    
    private Product? CreateNewProduct()
    {
        var viewModel = new ProductEditViewModel();
        var result = _dialogService.ShowDialog(viewModel);
        
        if (result == true)
        {
            return new Product
            {
                Id = 0,
                Name = viewModel.Name,
                Price = viewModel.Price,
                Description = viewModel.Description,
                CreatedAt = DateTime.Now
            };
        }
        
        return null;
    }
    
    private void EditExistingProduct(Product product)
    {
        var viewModel = new ProductEditViewModel(product);
        var result = _dialogService.ShowDialog(viewModel);
        
        if (result == true)
        {
            product.Name = viewModel.Name;
            product.Price = viewModel.Price;
            product.Description = viewModel.Description;
            product.ModifiedAt = DateTime.Now;
        }
    }
    
    public ReadOnlyObservableCollection<ProductViewModel> Products 
        => _products.Items;
    
    public ProductViewModel? SelectedProduct
    {
        get => _products.SelectedItem;
        set => _products.SelectedItem = value;
    }
    
    public ICommand AddCommand => _products.AddCommand;
    public ICommand EditCommand => _products.EditCommand;
    public ICommand DeleteCommand => _products.DeleteCommand;
    public ICommand ClearCommand => _products.ClearCommand;
    
    public void Dispose()
    {
        _products.Dispose();
    }
}
```

### Beispiel 2: Mit Validierung und Rollback

```csharp
public class ValidatedProductViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    private readonly IValidationService _validationService;
    private readonly IMessageBoxService _messageBoxService;
    
    public ValidatedProductViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products,
        IValidationService validationService,
        IMessageBoxService messageBoxService)
    {
        _products = products;
        _validationService = validationService;
        _messageBoxService = messageBoxService;
        
        SetupCommands();
    }
    
    private void SetupCommands()
    {
        _products.CreateModel = () =>
        {
            var product = new Product { Id = 0 };
            
            var errors = _validationService.Validate(product);
            if (errors.Any())
            {
                ShowValidationErrors(errors);
                return null;
            }
            
            return product;
        };
        
        _products.EditModel = product =>
        {
            var originalState = product.Clone();
            
            var dialog = new EditProductDialog(product);
            if (dialog.ShowDialog() == true)
            {
                var errors = _validationService.Validate(product);
                if (errors.Any())
                {
                    ShowValidationErrors(errors);
                    product.RestoreFrom(originalState);
                }
            }
        };
    }
    
    private void ShowValidationErrors(IEnumerable<string> errors)
    {
        var message = string.Join(Environment.NewLine, errors);
        _messageBoxService.Show(
            message,
            "Validierungsfehler",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
    
    public ReadOnlyObservableCollection<ProductViewModel> Products 
        => _products.Items;
    
    public ICommand AddCommand => _products.AddCommand;
    public ICommand EditCommand => _products.EditCommand;
    public ICommand DeleteCommand => _products.DeleteCommand;
    
    public void Dispose()
    {
        _products.Dispose();
    }
}
```

### Beispiel 3: Batch-Operationen

```csharp
public class BatchOperationsViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public BatchOperationsViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products)
    {
        _products = products;
        
        _products.CreateModel = () => new Product();
        
        // Custom Commands für Batch-Operationen
        DeleteSelectedCommand = new RelayCommand(
            _ => DeleteSelectedProducts(),
            _ => _products.SelectedItems.Count > 0);
        
        UpdatePricesCommand = new RelayCommand(
            _ => UpdateSelectedPrices(),
            _ => _products.SelectedItems.Count > 0);
    }
    
    public ReadOnlyObservableCollection<ProductViewModel> Products 
        => _products.Items;
    
    public ObservableCollection<ProductViewModel> SelectedProducts 
        => _products.SelectedItems;
    
    public ICommand DeleteSelectedCommand { get; }
    public ICommand UpdatePricesCommand { get; }
    
    private void DeleteSelectedProducts()
    {
        var itemsToRemove = _products.SelectedItems.ToList();
        _products.RemoveRange(itemsToRemove);
    }
    
    private void UpdateSelectedPrices()
    {
        foreach (var product in _products.SelectedItems)
        {
            // Preis-Update Logik
            var model = product.Model;
            model.Price *= 1.1m; // 10% Erhöhung
        }
    }
    
    public void Dispose()
    {
        _products.Dispose();
    }
}
```

---

## Zusammenfassung

### Wichtigste Punkte

1. **AddViewModelPackage**: Registriert EditableCollectionViewModel automatisch als Transient
2. **Commands**: Alle Commands arbeiten mit dem lokalen ModelStore
3. **CreateModel**: Delegate für neue Objekt-Erstellung, null-Rückgabe erlaubt
4. **EditModel**: Delegate für Bearbeitung, PropertyChanged-Tracking persistiert automatisch
5. **DeleteSelectedCommand**: Für Multi-Selection mit MultiSelectBehavior
6. **Custom Commands**: Können zusätzlich zu Standard-Commands verwendet werden
7. **CanExecute**: Automatische Validierung basierend auf SelectedItem, Count und Delegates

### Weiterführende Dokumentation

- [CollectionViewModel Guide](CollectionViewModel_Guide.md) - Basis-Funktionalität ohne Commands
- [Custom Controls Guide](CustomControls_Guide.md) - ListEditorView und DropDownEditorView
- [Test Guide](CustomWPFControls_TestGuide.md) - Testing von ViewModels
- [API Reference](../API-Reference.md) - Vollständige API-Dokumentation
