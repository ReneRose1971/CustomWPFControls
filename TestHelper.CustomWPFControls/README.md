# TestHelper.CustomWPFControls

Test-Utilities und Mock-Implementierungen für CustomWPFControls-Tests.

## Überblick

Dieses Projekt stellt wiederverwendbare Test-Infrastruktur für Unit- und Integration-Tests bereit:

- **Mock-Services** - Mock-Implementierungen von IDialogService und IMessageBoxService
- **Test-Helpers** - Utilities für Testdaten-Erstellung

## Hauptkomponenten

### MockDialogService

Mock-Implementation des IDialogService für Unit-Tests ohne WPF-UI.

```csharp
using TestHelper.CustomWPFControls.Mocks;

public class ViewModelTest
{
    [Fact]
    public void AddCustomer_ShowsEditDialog()
    {
        // Arrange
        var mockDialog = new MockDialogService();
        mockDialog.NextDialogResult = true;  // Simuliert OK-Klick
        
        var viewModel = new CustomerListViewModel(services, factory, mockDialog);
        
        // Act
        viewModel.AddCustomerCommand.Execute(null);
        
        // Assert
        mockDialog.VerifyDialogShown<CustomerEditViewModel>();
        Assert.Single(mockDialog.Calls);
    }
}
```

**Features:**
- Aufzeichnung aller Dialog-Aufrufe
- Konfigurierbare Rückgabewerte (NextDialogResult, NextConfirmationResult)
- Verify-Methoden für Assertions
- Keine WPF-UI erforderlich

### MockMessageBoxService

Mock-Implementierung des IMessageBoxService für Unit-Tests.

```csharp
using TestHelper.CustomWPFControls.Mocks;

public class ViewModelTest
{
    [Fact]
    public void DeleteCustomer_ShowsConfirmation()
    {
        // Arrange
        var mockMessageBox = new MockMessageBoxService();
        mockMessageBox.NextYesNoResult = true;  // Simuliert Ja-Klick
        
        var viewModel = new CustomerListViewModel(services, factory, mockMessageBox);
        
        // Act
        viewModel.DeleteCustomerCommand.Execute(null);
        
        // Assert
        mockMessageBox.VerifyConfirmationShown("Wirklich löschen");
        Assert.Single(mockMessageBox.Calls);
    }
}
```

**Features:**
- Tracking aller MessageBox-Aufrufe
- Konfigurierbare Ergebnisse (NextResult, NextYesNoResult, etc.)
- Verify-Methoden für verschiedene MessageBox-Typen
- Keine WPF-UI erforderlich

## Verwendung in Tests

### MockDialogService Usage

```csharp
[Fact]
public void Test_DialogInteraction()
{
    // Arrange
    var mockDialog = new MockDialogService();
    mockDialog.NextDialogResult = true;  // User clicks OK
    
    var viewModel = new MyViewModel(mockDialog);
    
    // Act
    viewModel.ShowDialogCommand.Execute(null);
    
    // Assert
    mockDialog.VerifyDialogShown<MyDialogViewModel>();
    Assert.Equal(1, mockDialog.Calls.Count);
    
    var call = mockDialog.GetCallFor<MyDialogViewModel>();
    Assert.NotNull(call);
    Assert.Equal("Edit Customer", call.Title);
}
```

### MockMessageBoxService Usage

```csharp
[Fact]
public void Test_MessageBoxInteraction()
{
    // Arrange
    var mockMessageBox = new MockMessageBoxService();
    mockMessageBox.NextYesNoResult = false;  // User clicks No
    
    var viewModel = new MyViewModel(mockMessageBox);
    
    // Act
    viewModel.DeleteCommand.Execute(null);
    
    // Assert
    mockMessageBox.VerifyConfirmationShown("Delete item?");
    mockMessageBox.VerifyCallCount(1);
}
```

## Mock-Reset

Beide Mock-Services unterstützen Reset für Test-Isolation:

```csharp
public class MyTests
{
    private readonly MockDialogService _mockDialog = new();
    
    [Fact]
    public void Test1()
    {
        _mockDialog.NextDialogResult = true;
        // ... test ...
    }
    
    [Fact]
    public void Test2()
    {
        _mockDialog.Reset();  // Clean slate
        _mockDialog.NextDialogResult = false;
        // ... test ...
    }
}
```

## Integration mit DI

```csharp
services.AddSingleton<IDialogService, MockDialogService>();
services.AddSingleton<IMessageBoxService, MockMessageBoxService>();
```

## Siehe auch

- [CustomWPFControls README](../CustomWPFControls/README.md)
- [CustomWPFControls.Tests](../CustomWPFControls.Tests/README.md)
- [Getting Started](../CustomWPFControls/Docs/Getting-Started.md)
