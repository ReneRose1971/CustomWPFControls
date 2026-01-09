# Best Practices - CustomWPFControls

Hinweise zur Verwendung von CustomWPFControls.

## ViewModel-Design

- ViewModelBase<TModel> als Basis verwenden
- Domain-Properties an Model delegieren
- UI-Properties nur im ViewModel
- Constructor muss TModel als ersten Parameter haben

## DataStore-Integration

- IEqualityComparer für TModel registrieren
- Stable HashCode verwenden (z.B. Id-Property)

## Commands

- CreateModel-Callback für AddCommand setzen
- EditModel-Callback für EditCommand setzen
- CanExecute wird automatisch geprüft

## Controls

- SelectedItem mit Mode=TwoWay binden
- ButtonPlacement nach Kontext wählen (Right/Bottom/Top)
- Nicht benötigte Buttons via IsVisible ausblenden

## Performance

- VirtualizingPanel.IsVirtualizing="True" bei Listen >100 Items
- ICollectionView für Filtering verwenden

## Siehe auch

- [Getting Started](Getting-Started.md)
- [Architecture](Architecture.md)
- [API Reference](API-Reference.md)
