# Architecture - CustomWPFControls

Architektur-Übersicht des MVVM-Frameworks.

## Schichtenarchitektur

```
WPF Application
    |
    v
CustomWPFControls
    |-- ViewModels (ViewModelBase, CollectionViewModel, EditableCollectionViewModel)
    |-- Factories (IViewModelFactory, ViewModelFactory)
    |-- Controls (ListEditorView, DropDownEditorView)
    |-- Commands (RelayCommand, ObservableCommand, AsyncRelayCommand)
    |-- Services (DialogService, MessageBoxService)
    |
    v
DataStores
    |-- IDataStore<T>
    |-- InMemoryDataStore<T>
    |-- PersistentDataStore<T>
    |
    v
Storage Layer (JSON, LiteDB)
```

## Komponenten

### ViewModelBase<TModel>

Basisklasse für ViewModels mit PropertyChanged-Support via Fody.

**Verantwortlichkeiten:**
- Model-Wrapping
- PropertyChanged-Events
- Equals/GetHashCode-Delegation

### CollectionViewModel<TModel, TViewModel>

Bidirektionale Synchronisation zwischen DataStore und ViewModels.

**Verantwortlichkeiten:**
- ViewModel-Erstellung via Factory
- Synchronisation DataStore <-> ViewModels
- SelectedItem-Verwaltung
- ViewModel-Lifecycle

**Abhängigkeiten:**
- IDataStore<TModel>
- IViewModelFactory<TModel, TViewModel>
- IEqualityComparer<TModel>

### EditableCollectionViewModel<TModel, TViewModel>

Erweitert CollectionViewModel um Commands.

**Verantwortlichkeiten:**
- AddCommand, DeleteCommand, EditCommand, ClearCommand
- CanExecute-Logik
- CreateModel/EditModel-Callbacks

### ViewModelFactory<TModel, TViewModel>

DI-basierte Factory für ViewModel-Erstellung.

**Verantwortlichkeiten:**
- ViewModel-Instanziierung via ActivatorUtilities
- DI-Dependency-Auflösung

## Datenfluss

### Item hinzufügen

```
AddCommand.Execute()
  -> CreateModel()
  -> ModelStore.Add(model)
  -> DataStore CollectionChanged
  -> Factory.Create(model)
  -> ViewModel zu Items hinzugefügt
  -> View-Update
```

### Item löschen

```
DeleteCommand.Execute()
  -> ModelStore.Remove(model)
  -> DataStore CollectionChanged
  -> ViewModel aus Items entfernt
  -> ViewModel.Dispose()
  -> View-Update
```

### Externe DataStore-Änderung

```
DataStore.Add(model) (extern)
  -> DataStore CollectionChanged
  -> Factory.Create(model)
  -> ViewModel zu Items hinzugefügt
  -> View-Update (automatisch via Binding)
```

## Design-Patterns

- **MVVM**: Model-View-ViewModel Trennung
- **Factory**: IViewModelFactory für ViewModel-Erstellung
- **Observer**: CollectionChanged-Events für Synchronisation
- **Dependency Injection**: Constructor Injection für alle Dependencies

## Thread-Safety

CollectionViewModel respektiert WPF UI-Thread:
- DataStore mit SynchronizationContext
- Alle Collection-Änderungen auf UI-Thread marshaled
- ObservableCollection ist nicht thread-safe (Dispatcher verwenden)

## Siehe auch

- [ViewModelBase](ViewModelBase.md)
- [CollectionViewModel](CollectionViewModel.md)
- [API Reference](API-Reference.md)
