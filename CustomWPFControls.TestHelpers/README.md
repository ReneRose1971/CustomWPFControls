# CustomWPFControls.TestHelpers

Test helpers and mock implementations for unit testing applications that use the **CustomWPFControls** library.

## ?? Installation

```sh
# In your test project:
dotnet add package CustomWPFControls.TestHelpers
```

## ?? Purpose

This package provides mock implementations of **CustomWPFControls** services, enabling you to:

- ? Test ViewModels that use `IDialogService` or `IMessageBoxService`
- ? Verify dialog calls without actually showing UI
- ? Configure dialog results for different test scenarios
- ? Track all service interactions for assertions

## ?? Included Mocks

### MockDialogService

Mock implementation of `IDialogService` for testing dialog interactions.

**Features:**
- Tracks all `ShowDialog()`, `ShowWindow()`, and `ShowMessage()` calls
- Configurable results via `NextDialogResult`, `NextConfirmationResult`
- Verification helpers: `VerifyDialogShown<TViewModel>()`, `VerifyMessageShown()`

**Example:**

```csharp
using CustomWPFControls.TestHelpers.Mocks;
using Xunit;

public class MainViewModelTests
{
    [Fact]
    public void SaveCommand_ShowsConfirmationDialog()
    {
        // Arrange
        var mockDialog = new MockDialogService();
        mockDialog.NextConfirmationResult = true;  // Simulate "Yes" click
        
        var viewModel = new MainViewModel(mockDialog);
        
        // Act
        viewModel.SaveCommand.Execute(null);
        
        // Assert
        mockDialog.VerifyDialogShown<CustomerEditViewModel>();
        mockDialog.Calls.Should().HaveCount(1);
    }
}
```

---

### MockMessageBoxService

Mock implementation of `IMessageBoxService` for testing message box interactions.

**Features:**
- Tracks all `ShowMessage()`, `ShowWarning()`, `ShowError()`, `ShowConfirmation()` calls
- Configurable results via `NextYesNoResult`, `NextYesNoCancelResult`, `NextOkCancelResult`
- Verification helpers: `VerifyMessageShown()`, `VerifyWarningShown()`, `VerifyErrorShown()`

**Example:**

```csharp
using CustomWPFControls.TestHelpers.Mocks;
using Xunit;

public class MainViewModelTests
{
    [Fact]
    public void SaveCommand_ShowsSuccessMessage()
    {
        // Arrange
        var mockMessageBox = new MockMessageBoxService();
        var viewModel = new MainViewModel(mockMessageBox);
        
        // Act
        viewModel.SaveCommand.Execute(null);
        
        // Assert
        mockMessageBox.VerifyMessageShown("Erfolgreich gespeichert");
    }
    
    [Fact]
    public void DeleteCommand_AsksForConfirmation()
    {
        // Arrange
        var mockMessageBox = new MockMessageBoxService();
        mockMessageBox.NextYesNoResult = false;  // Simulate "No" click
        
        var viewModel = new MainViewModel(mockMessageBox);
        
        // Act
        viewModel.DeleteCommand.Execute(null);
        
        // Assert
        mockMessageBox.VerifyConfirmationShown("Wirklich löschen?");
        mockMessageBox.Calls.Should().HaveCount(1);
    }
}
```

---

## ?? API Reference

### MockDialogService

#### Configuration Properties
- `NextDialogResult` (bool?) - Result for next `ShowDialog()` call (default: true)
- `NextConfirmationResult` (bool) - Result for next `ShowConfirmation()` call (default: true)
- `NextMessageBoxResult` (MessageBoxResult) - Result for next `ShowMessageBox()` call (default: OK)

#### Tracking
- `Calls` (IReadOnlyList<DialogCall>) - All tracked service calls

#### Verification Methods
- `VerifyDialogShown<TViewModel>()` - Asserts that a dialog for the specified ViewModel was shown
- `VerifyMessageShown(string message)` - Asserts that a message was shown
- `VerifyAnyDialogShown()` - Asserts that at least one dialog was shown
- `Reset()` - Clears all calls and resets configuration to defaults

---

### MockMessageBoxService

#### Configuration Properties
- `NextResult` (MessageBoxResult) - Result for next `ShowMessageBox()` call (default: OK)
- `NextYesNoResult` (bool) - Result for next `AskYesNo()` / `ShowConfirmation()` call (default: true)
- `NextYesNoCancelResult` (bool?) - Result for next `AskYesNoCancel()` call (default: true)
- `NextOkCancelResult` (bool) - Result for next `AskOkCancel()` call (default: true)

#### Tracking
- `Calls` (IReadOnlyList<MessageBoxCall>) - All tracked MessageBox calls

#### Verification Methods
- `VerifyMessageShown(string message)` - Asserts that an information message was shown
- `VerifyWarningShown(string message)` - Asserts that a warning was shown
- `VerifyErrorShown(string message)` - Asserts that an error was shown
- `VerifyConfirmationShown(string message)` - Asserts that a confirmation dialog was shown
- `VerifyCallCount(int count)` - Asserts that exactly N MessageBoxes were shown
- `Reset()` - Clears all calls and resets configuration to defaults

---

## ?? Best Practices

### 1. Reset Mocks Between Tests

```csharp
private readonly MockDialogService _mockDialog = new();

[Fact]
public void Test1()
{
    _mockDialog.Reset();  // Clean state
    // ... test code
}

[Fact]
public void Test2()
{
    _mockDialog.Reset();  // Clean state
    // ... test code
}
```

### 2. Configure Results Before Actions

```csharp
[Fact]
public void Test_UserCancels()
{
    var mock = new MockDialogService();
    mock.NextDialogResult = false;  // Configure BEFORE action
    
    viewModel.Command.Execute(null);
    
    // Assert behavior when user cancels
}
```

### 3. Use Fluent Assertions

```csharp
mockDialog.Calls.Should().HaveCount(2);
mockDialog.Calls[0].ViewModelType.Should().Be<CustomerEditViewModel>();
mockDialog.Calls[1].Type.Should().Be(DialogType.Confirmation);
```

### 4. Verify Specific Messages

```csharp
// Partial match (recommended)
mockMessageBox.VerifyMessageShown("gespeichert");

// Exact match
mockMessageBox.Calls.Should().Contain(c => c.Message == "Erfolgreich gespeichert!");
```

---

## ?? Related Documentation

- **CustomWPFControls Documentation:** See main library documentation for `IDialogService` and `IMessageBoxService` usage
- **Testing Guide:** See `CustomWPFControls.md` for complete testing examples

---

## ?? License

MIT License - Same as CustomWPFControls main library

---

## ?? Links

- **NuGet Package:** `CustomWPFControls.TestHelpers`
- **Main Library:** `CustomWPFControls`
- **Repository:** [GitHub](https://github.com/yourrepo/CustomWPFControls)

---

**Version:** 2.0.0  
**Target Framework:** .NET 8 (Windows)
