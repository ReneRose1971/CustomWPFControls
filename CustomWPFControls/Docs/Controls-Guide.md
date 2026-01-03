# Controls-Guide - CustomWPFControls

Umfassende Dokumentation der wiederverwendbaren Editor-Controls mit integrierten CRUD-Funktionen.

## Inhaltsverzeichnis

- [Übersicht](#übersicht)
- [Control-Hierarchie](#control-hierarchie)
- [ListEditorView](#listeditorview)
- [DropDownEditorView](#dropdowneditorview)
- [ButtonPlacement](#buttonplacement)
- [Integration mit ViewModels](#integration-mit-viewmodels)
- [Styling und Anpassung](#styling-und-anpassung)
- [Best Practices](#best-practices)

---

## Übersicht

CustomWPFControls bietet zwei Haupt-Editor-Controls für CRUD-Operationen:

| Control | Basis | Zweck | Button-Layout |
|---------|-------|-------|---------------|
| **ListEditorView** | ListView | Master-Detail-Ansichten, Listen | Bottom (fest) |
| **DropDownEditorView** | ComboBox | Auswahlfelder, kompakte Editoren | Right/Bottom/Top (konfigurierbar) |

Beide Controls teilen die gleiche API-Struktur und funktionieren nahtlos mit `EditableCollectionViewModel`.

---

## Control-Hierarchie

```
System.Windows.Controls.ItemsControl
    ?? ListView
    ?   ?? BaseListView (+ Count Property)
    ?       ?? ListEditorView (+ Commands, Buttons)
    ?
    ?? ComboBox
        ?? BaseComboBoxView (+ Count Property)
            ?? DropDownEditorView (+ Commands, Buttons, ButtonPlacement)
```

### Gemeinsame Features

Alle Editor-Controls bieten:
- **Command-Properties**: `AddCommand`, `EditCommand`, `DeleteCommand`, `ClearCommand`
- **Visibility-Properties**: `IsAddVisible`, `IsEditVisible`, `IsDeleteVisible`, `IsClearVisible`
- **Text-Properties**: `AddButtonText`, `EditButtonText`, `DeleteButtonText`, `ClearButtonText`
- **Count-Property**: Anzahl der Items (von Basis-Controls)

---

## ListEditorView

### Beschreibung

`ListEditorView` ist ein erweitertes `ListView`-Control mit integrierten Action-Buttons unterhalb der Liste.

### Verwendung

```xml
<Window xmlns:controls="clr-namespace:CustomWPFControls.Controls;assembly=CustomWPFControls">
    <controls:ListEditorView 
        ItemsSource="{Binding Customers}"
        SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"
        
        AddCommand="{Binding AddCommand}"
        EditCommand="{Binding EditCommand}"
        DeleteCommand="{Binding DeleteCommand}"
        ClearCommand="{Binding ClearCommand}"
        
        AddButtonText="Hinzufügen"
        EditButtonText="Bearbeiten"
        DeleteButtonText="Löschen"
        ClearButtonText="Alle löschen"
        
        IsAddVisible="True"
        IsEditVisible="True"
        IsDeleteVisible="True"
        IsClearVisible="True">
        
        <controls:ListEditorView.View>
            <GridView>
                <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" Width="200"/>
                <GridViewColumn Header="Email" DisplayMemberBinding="{Binding Email}" Width="250"/>
            </GridView>
        </controls:ListEditorView.View>
    </controls:ListEditorView>
</Window>
```

### Properties

| Property | Type | Default | Beschreibung |
|----------|------|---------|--------------|
| `AddCommand` | ICommand | null | Command zum Hinzufügen |
| `EditCommand` | ICommand | null | Command zum Bearbeiten |
| `DeleteCommand` | ICommand | null | Command zum Löschen |
| `ClearCommand` | ICommand | null | Command zum Leeren |
| `IsAddVisible` | bool | true | Sichtbarkeit Add-Button |
| `IsEditVisible` | bool | true | Sichtbarkeit Edit-Button |
| `IsDeleteVisible` | bool | true | Sichtbarkeit Delete-Button |
| `IsClearVisible` | bool | true | Sichtbarkeit Clear-Button |
| `AddButtonText` | string | "Hinzufügen" | Text Add-Button |
| `EditButtonText` | string | "Bearbeiten" | Text Edit-Button |
| `DeleteButtonText` | string | "Löschen" | Text Delete-Button |
| `ClearButtonText` | string | "Alle löschen" | Text Clear-Button |
| `Count` | int | 0 | Anzahl der Items (read-only) |

### Layout

```
??????????????????????????????????????
?        ListView Content            ?
?  ????????????????????????????????  ?
?  ? Item 1                       ?  ?
?  ? Item 2                       ?  ?
?  ? Item 3                       ?  ?
?  ????????????????????????????????  ?
??????????????????????????????????????
? Count: 3     [Add][Edit][Del][Clr]?
??????????????????????????????????????
```

### Beispiele

#### Minimalistisch (nur Add)
```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    AddCommand="{Binding AddCommand}"
    IsEditVisible="False"
    IsDeleteVisible="False"
    IsClearVisible="False"/>
```

#### Englische Beschriftungen
```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    AddButtonText="Add"
    EditButtonText="Edit"
    DeleteButtonText="Delete"
    ClearButtonText="Clear All"/>
```

#### Mit Icons
```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    AddButtonText="? New"
    EditButtonText="?? Edit"
    DeleteButtonText="??? Remove"/>
```

---

## DropDownEditorView

### Beschreibung

`DropDownEditorView` ist eine erweiterte `ComboBox` mit integrierten Action-Buttons und konfigurierbarem Button-Layout.

### Verwendung

```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    SelectedItem="{Binding SelectedCategory, Mode=TwoWay}"
    DisplayMemberPath="Name"
    
    ButtonPlacement="Right"
    
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"
    
    AddButtonText="Neu"
    EditButtonText="Ändern"
    DeleteButtonText="Entfernen"
    
    IsAddVisible="True"
    IsEditVisible="True"
    IsDeleteVisible="True"
    IsClearVisible="False"/>
```

### Properties

Zusätzlich zu den gemeinsamen Properties:

| Property | Type | Default | Beschreibung |
|----------|------|---------|--------------|
| `ButtonPlacement` | ButtonPlacement | Right | Position der Buttons |

### ButtonPlacement

```csharp
public enum ButtonPlacement
{
    Right,   // Buttons rechts neben ComboBox (Standard)
    Bottom,  // Buttons unter ComboBox
    Top      // Buttons in ToolBar über ComboBox
}
```

### Layouts

#### Right (Standard)
```
[ComboBox ?] [Add][Edit][Delete]
```

Ideal für: Kompakte Formulare, Inline-Editing

```xml
<controls:DropDownEditorView 
    ButtonPlacement="Right"
    ItemsSource="{Binding Items}"/>
```

#### Bottom
```
[ComboBox ?]
[Hinzufügen] [Bearbeiten] [Löschen]
```

Ideal für: Breite Layouts, ähnlich ListView

```xml
<controls:DropDownEditorView 
    ButtonPlacement="Bottom"
    ItemsSource="{Binding Items}"/>
```

#### Top
```
[?? Hinzufügen | Bearbeiten | ??? Löschen]
[ComboBox ?]
```

Ideal für: Professionelle Anwendungen, Master-Detail-Views

```xml
<controls:DropDownEditorView 
    ButtonPlacement="Top"
    ItemsSource="{Binding Items}"/>
```

### Beispiele

#### Kompakte Kategorie-Auswahl
```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    DisplayMemberPath="Name"
    ButtonPlacement="Right"
    AddCommand="{Binding AddCategoryCommand}"
    EditCommand="{Binding EditCategoryCommand}"
    DeleteCommand="{Binding DeleteCategoryCommand}"
    AddButtonText="+"
    EditButtonText="??"
    DeleteButtonText="-"/>
```

#### Status-Auswahl mit ToolBar
```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding StatusList}"
    DisplayMemberPath="DisplayName"
    ButtonPlacement="Top"
    AddButtonText="Neuer Status"
    EditButtonText="Status bearbeiten"
    DeleteButtonText="Status löschen"
    IsClearVisible="False"/>
```

---

## Integration mit ViewModels

### EditableCollectionViewModel

Beide Controls sind für nahtlose Integration mit `EditableCollectionViewModel` konzipiert:

```csharp
public class CustomerListViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Customer, CustomerViewModel> _customers;
    
    public CustomerListViewModel(
        IDataStores dataStores,
        IViewModelFactory<Customer, CustomerViewModel> factory,
        IEqualityComparerService comparerService)
    {
        _customers = new EditableCollectionViewModel<Customer, CustomerViewModel>(
            dataStores,
            factory,
            comparerService);
        
        // CreateModel-Callback
        _customers.CreateModel = () => new Customer 
        { 
            Name = "Neuer Kunde",
            Email = "neu@example.com"
        };
        
        // EditModel-Callback
        _customers.EditModel = customer =>
        {
            var dialog = new CustomerEditDialog(customer);
            dialog.ShowDialog();
        };
    }
    
    // Properties für View-Binding
    public ReadOnlyObservableCollection<CustomerViewModel> Customers => _customers.Items;
    public CustomerViewModel SelectedCustomer
    {
        get => _customers.SelectedItem;
        set => _customers.SelectedItem = value;
    }
    
    // Commands für Controls
    public ICommand AddCommand => _customers.AddCommand;
    public ICommand EditCommand => _customers.EditCommand;
    public ICommand DeleteCommand => _customers.DeleteCommand;
    public ICommand ClearCommand => _customers.ClearCommand;
    
    public void Dispose() => _customers.Dispose();
}
```

### XAML-Binding

```xml
<Window DataContext="{Binding CustomerListViewModel}">
    <Grid>
        <!-- ListEditorView -->
        <controls:ListEditorView 
            ItemsSource="{Binding Customers}"
            SelectedItem="{Binding SelectedCustomer}"
            AddCommand="{Binding AddCommand}"
            EditCommand="{Binding EditCommand}"
            DeleteCommand="{Binding DeleteCommand}"/>
        
        <!-- DropDownEditorView -->
        <controls:DropDownEditorView 
            ItemsSource="{Binding Customers}"
            SelectedItem="{Binding SelectedCustomer}"
            DisplayMemberPath="Name"
            AddCommand="{Binding AddCommand}"
            EditCommand="{Binding EditCommand}"
            DeleteCommand="{Binding DeleteCommand}"/>
    </Grid>
</Window>
```

---

## Styling und Anpassung

### Control-Templates überschreiben

Sie können das gesamte Template eines Controls überschreiben:

```xml
<controls:ListEditorView ItemsSource="{Binding Items}">
    <controls:ListEditorView.Template>
        <ControlTemplate TargetType="{x:Type controls:ListEditorView}">
            <DockPanel>
                <!-- Custom Layout -->
                <ToolBar DockPanel.Dock="Top">
                    <Button Content="Neu" Command="{TemplateBinding AddCommand}"/>
                    <Button Content="Bearbeiten" Command="{TemplateBinding EditCommand}"/>
                </ToolBar>
                <ItemsPresenter/>
            </DockPanel>
        </ControlTemplate>
    </controls:ListEditorView.Template>
</controls:ListEditorView>
```

### Styles anpassen

```xml
<Window.Resources>
    <Style TargetType="{x:Type controls:ListEditorView}">
        <Setter Property="AddButtonText" Value="? Hinzufügen"/>
        <Setter Property="EditButtonText" Value="?? Bearbeiten"/>
        <Setter Property="DeleteButtonText" Value="??? Löschen"/>
        <Setter Property="BorderBrush" Value="LightGray"/>
        <Setter Property="BorderThickness" Value="1"/>
    </Style>
</Window.Resources>
```

### Lokalisierung

Mit Resource-Dateien:

```csharp
// Resources.resx
AddButton = "Add"
EditButton = "Edit"
DeleteButton = "Delete"
```

```xml
<controls:ListEditorView 
    AddButtonText="{x:Static resx:Resources.AddButton}"
    EditButtonText="{x:Static resx:Resources.EditButton}"
    DeleteButtonText="{x:Static resx:Resources.DeleteButton}"/>
```

---

## Best Practices

### Command-Verfügbarkeit

Commands werden automatisch disabled, wenn sie nicht verfügbar sind:

```csharp
// EditCommand ist disabled wenn SelectedItem == null
// AddCommand ist disabled wenn CreateModel == null
// ClearCommand ist disabled wenn Count == 0
```

### SelectedItem Binding

Verwenden Sie immer `Mode=TwoWay` für SelectedItem:

```xml
<controls:ListEditorView 
    SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"/>
```

### Button-Sichtbarkeit

Verstecken Sie nicht benötigte Buttons:

```xml
<controls:DropDownEditorView 
    IsEditVisible="False"
    IsClearVisible="False"/>
```

### ButtonPlacement-Wahl

| Kontext | Empfohlenes Layout | Begründung |
|---------|-------------------|------------|
| Formular-Felder | Right | Kompakt, platzsparend |
| Master-Detail | Bottom | Konsistent mit ListView |
| Tool-Fenster | Top | Professionell, ToolBar-Look |

### Performance bei großen Listen

Aktivieren Sie Virtualisierung für ListEditorView:

```xml
<controls:ListEditorView 
    VirtualizingPanel.IsVirtualizing="True"
    VirtualizingPanel.VirtualizationMode="Recycling"/>
```

---

## Siehe auch

- [EditableCollectionViewModel](EditableCollectionViewModel.md)
- [CollectionViewModel](CollectionViewModel.md)
- [ViewModelBase](ViewModelBase.md)
- [API Reference](API-Reference.md)
