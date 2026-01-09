# TestHelper.CustomWPFControls

Test-Utilities und Mock-Implementierungen für CustomWPFControls-Tests.

## Überblick

Wiederverwendbare Test-Infrastruktur für Unit- und Integration-Tests:

- **CollectionViewModelExtensions** - Deterministische Synchronisation für Tests
- **MockDialogService** - Mock-Implementation von IDialogService
- **MockMessageBoxService** - Mock-Implementation von IMessageBoxService

## Hauptkomponenten

### CollectionViewModelExtensions

Event-basierte Synchronisation für CollectionViewModel-Tests.

#### LoadModelsAndWait

Lädt Models und wartet auf TransformTo-Synchronisation.

```csharp
// Garantiert synchronisierte Items nach LoadModels
viewModel.LoadModelsAndWait(models);
Assert.Equal(3, viewModel.Items.Count);
```

**Zweck:** `LoadModels()` ist synchron, aber TransformTo läuft asynchron. `LoadModelsAndWait` garantiert synchronisierte `viewModel.Items`.

**Varianten:**
- `LoadModelsAndWait(models)`
- `LoadModelsAndWait(models, expectedCount)`
- `LoadModelsAndWait(models, timeout)`
- `LoadModelsAndWaitAsync(models)`

### MockDialogService

Mock-Implementation von IDialogService für Unit-Tests.

```csharp
var mockDialog = new MockDialogService();
mockDialog.NextDialogResult = true;

viewModel.AddCommand.Execute(null);

mockDialog.VerifyDialogShown<CustomerEditViewModel>();
```

**Contract:**
- Aufzeichnung aller Dialog-Aufrufe
- Konfigurierbare Rückgabewerte (NextDialogResult)
- Verify-Methoden für Assertions
- Reset() für Test-Isolation

### MockMessageBoxService

Mock-Implementation von IMessageBoxService für Unit-Tests.

```csharp
var mockMessageBox = new MockMessageBoxService();
mockMessageBox.NextYesNoResult = false;

viewModel.DeleteCommand.Execute(null);

mockMessageBox.VerifyConfirmationShown("Delete?");
```

**Contract:**
- Tracking aller MessageBox-Aufrufe
- Konfigurierbare Ergebnisse (NextResult, NextYesNoResult)
- Verify-Methoden für verschiedene MessageBox-Typen
- Reset() für Test-Isolation

## DI-Integration

```csharp
services.AddSingleton<IDialogService, MockDialogService>();
services.AddSingleton<IMessageBoxService, MockMessageBoxService>();
```

## Siehe auch

- [CustomWPFControls README](../CustomWPFControls/README.md)
- [Getting Started](../CustomWPFControls/Docs/Getting-Started.md)
