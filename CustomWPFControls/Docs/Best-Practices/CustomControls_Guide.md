# Custom Controls - Best Practices Guide

Umfassender Leitfaden für die Verwendung der wiederverwendbaren Editor-Controls `ListEditorView` und `DropDownEditorView`.

## Inhaltsverzeichnis

- [Überblick](#überblick)
- [ListEditorView](#listeditorview)
- [DropDownEditorView](#dropdowneditorview)
- [MultiSelectBehavior](#multiselectbehavior)
- [Integration mit EditableCollectionViewModel](#integration-mit-editablecollectionviewmodel)
- [Styling und Anpassung](#styling-und-anpassung)
- [Praktische Beispiele](#praktische-beispiele)

---

## Überblick

CustomWPFControls bietet zwei Haupt-Editor-Controls für CRUD-Operationen:

| Control | Basis | Zweck | Button-Layout |
|---------|-------|-------|---------------|
| **ListEditorView** | ListView | Master-Detail-Ansichten, Listen | Bottom (fest) |
| **DropDownEditorView** | ComboBox | Auswahlfelder, kompakte Editoren | Right/Bottom/Top (konfigurierbar) |

Beide Controls:
- Arbeiten nahtlos mit `EditableCollectionViewModel`
- Unterstützen vollständige CRUD-Operationen
- Bieten konfigurierbare Button-Sichtbarkeit und -Texte
- Zeigen automatisch die Anzahl der Items an (Count-Property)

---

## ListEditorView

### Beschreibung

`ListEditorView` ist ein erweitertes `ListView`-Control mit integrierten Action-Buttons unterhalb der Liste.

### Grundlegende Verwendung

```xml
<Window xmlns:controls="clr-namespace:CustomWPFControls.Controls;assembly=CustomWPFControls">
    <controls:ListEditorView 
        ItemsSource="{Binding Customers}"
        SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"
        
        AddCommand="{Binding AddCommand}"
        EditCommand="{Binding EditCommand}"
        DeleteCommand="{Binding DeleteCommand}"
        ClearCommand="{Binding ClearCommand}"/>
</Window>
```

### Alle Properties

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Products}"
    SelectedItem="{Binding SelectedProduct, Mode=TwoWay}"
    
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
            <GridViewColumn Header="Preis" DisplayMemberBinding="{Binding Price}" Width="100"/>
        </GridView>
    </controls:ListEditorView.View>
</controls:ListEditorView>
```

### Layout-Struktur

```
???????????????????????????????????
?        ListView Content         ?
?  ?????????????????????????????  ?
?  ? Item 1                    ?  ?
?  ? Item 2                    ?  ?
?  ? Item 3                    ?  ?
?  ?????????????????????????????  ?
???????????????????????????????????
? Count: 3     [Add][Edit][Del][Clr]?
???????????????????????????????????
```

### Property-Referenz

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

### Verwendungsszenarien

#### Minimalistisch (nur Add)

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    AddCommand="{Binding AddCommand}"
    IsEditVisible="False"
    IsDeleteVisible="False"
    IsClearVisible="False"/>
```

#### Read-Only Liste (keine Buttons)

```xml
<controls:ListEditorView 
    ItemsSource="{Binding Items}"
    IsAddVisible="False"
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

---

## DropDownEditorView

### Beschreibung

`DropDownEditorView` ist eine erweiterte `ComboBox` mit integrierten Action-Buttons und konfigurierbarem Button-Layout.

### Grundlegende Verwendung

```xml
<controls:DropDownEditorView 
    ItemsSource="{Binding Categories}"
    SelectedItem="{Binding SelectedCategory, Mode=TwoWay}"
    DisplayMemberPath="Name"
    
    ButtonPlacement="Right"
    
    AddCommand="{Binding AddCommand}"
    EditCommand="{Binding EditCommand}"
    DeleteCommand="{Binding DeleteCommand}"/>
```

### ButtonPlacement

```csharp
public enum ButtonPlacement
{
    Right,   // Buttons rechts neben ComboBox (Standard)
    Bottom,  // Buttons unter ComboBox
    Top      // Buttons in ToolBar über ComboBox
}
```

### Layout-Varianten

#### Right (Standard)

```
[ComboBox ?] [Add][Edit][Delete]
```

Ideal für: Kompakte Formulare, Inline-Editing

```xml
<controls:DropDownEditorView 
    ButtonPlacement="Right"
    ItemsSource="{Binding Categories}"/>
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
    ItemsSource="{Binding Categories}"/>
```

#### Top

```
[? Hinzufügen | Bearbeiten | ??? Löschen]
[ComboBox ?]
```

Ideal für: Professionelle Anwendungen, Master-Detail-Views

```xml
<controls:DropDownEditorView 
    ButtonPlacement="Top"
    ItemsSource="{Binding Categories}"/>
```

### Property-Referenz

Zusätzlich zu den gemeinsamen Properties:

| Property | Type | Default | Beschreibung |
|----------|------|---------|---------------|
| `ButtonPlacement` | ButtonPlacement | Right | Position der Buttons |
| `IsClearVisible` | bool | false | Clear für ComboBox unüblich |

### Verwendungsszenarien

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

## MultiSelectBehavior

### Zweck

Das `MultiSelectBehavior` ermöglicht bidirektionale Synchronisation zwischen `ListBox.SelectedItems` und einer ViewModel-Collection.

**Grund**: WPF's `ListBox.SelectedItems` ist readonly und kann nicht direkt gebunden werden.

### Verwendung

```xml
<Window xmlns:behaviors="clr-namespace:CustomWPFControls.Behaviors;assembly=CustomWPFControls">
    <ListBox SelectionMode="Multiple"
             ItemsSource="{Binding Products}"
             behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}"/>
</Window>
```

### ViewModel-Seite

```csharp
public class ProductListViewModel
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductListViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products)
    {
        _products = products;
    }
    
    // SelectedItems aus EditableCollectionViewModel
    public ObservableCollection<ProductViewModel> SelectedProducts 
        => _products.SelectedItems;
}
```

### Funktionalität

Das Behavior synchronisiert bidirektional:

- **ListBox ? ViewModel**: Wenn Benutzer Items in der ListBox auswählt/abwählt
- **ViewModel ? ListBox**: Wenn Code Items zur SelectedItems-Collection hinzufügt/entfernt

### Praktisches Beispiel

```xml
<Window>
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <!-- Multi-Selection ListBox -->
        <ListBox Grid.Row="0"
                 SelectionMode="Multiple"
                 ItemsSource="{Binding Products}"
                 behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel>
                        <TextBlock Text="{Binding Name}" FontWeight="Bold"/>
                        <TextBlock Text="{Binding Price, StringFormat=C}"/>
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
        
        <!-- Batch-Operations -->
        <StackPanel Grid.Row="1" Orientation="Horizontal">
            <TextBlock Text="{Binding SelectedProducts.Count, StringFormat='Ausgewählt: {0}'}" 
                       Margin="5"/>
            <Button Content="Ausgewählte löschen" 
                    Command="{Binding DeleteSelectedCommand}"/>
            <Button Content="Preise aktualisieren" 
                    Command="{Binding UpdatePricesCommand}"/>
        </StackPanel>
    </Grid>
</Window>
```

### Mit ListEditorView

```xml
<controls:ListEditorView 
    SelectionMode="Multiple"
    ItemsSource="{Binding Products}"
    behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}"
    DeleteCommand="{Binding DeleteSelectedCommand}"/>
```

---

## Integration mit EditableCollectionViewModel

### Vollständige Integration

```csharp
public class ProductManagementViewModel : IDisposable
{
    private readonly EditableCollectionViewModel<Product, ProductViewModel> _products;
    
    public ProductManagementViewModel(
        EditableCollectionViewModel<Product, ProductViewModel> products,
        IDialogService dialogService)
    {
        _products = products;
        
        // Commands konfigurieren
        _products.CreateModel = () => new Product { Name = "Neues Produkt" };
        _products.EditModel = product =>
        {
            var dialog = new ProductEditDialog(product);
            dialog.ShowDialog();
        };
    }
    
    // Properties für View-Binding
    public ReadOnlyObservableCollection<ProductViewModel> Products 
        => _products.Items;
    
    public ProductViewModel? SelectedProduct
    {
        get => _products.SelectedItem;
        set => _products.SelectedItem = value;
    }
    
    public ObservableCollection<ProductViewModel> SelectedProducts 
        => _products.SelectedItems;
    
    // Commands für Controls
    public ICommand AddCommand => _products.AddCommand;
    public ICommand EditCommand => _products.EditCommand;
    public ICommand DeleteCommand => _products.DeleteCommand;
    public ICommand ClearCommand => _products.ClearCommand;
    public ICommand DeleteSelectedCommand => _products.DeleteSelectedCommand;
    
    public void Dispose()
    {
        _products.Dispose();
    }
}
```

### XAML-Binding

```xml
<Window DataContext="{Binding ProductManagementViewModel}">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        
        <!-- ListEditorView mit Multi-Selection -->
        <controls:ListEditorView 
            Grid.Row="0"
            SelectionMode="Multiple"
            ItemsSource="{Binding Products}"
            SelectedItem="{Binding SelectedProduct}"
            behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}"
            
            AddCommand="{Binding AddCommand}"
            EditCommand="{Binding EditCommand}"
            DeleteCommand="{Binding DeleteCommand}"
            ClearCommand="{Binding ClearCommand}">
            
            <controls:ListEditorView.View>
                <GridView>
                    <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}"/>
                    <GridViewColumn Header="Preis" DisplayMemberBinding="{Binding Price}"/>
                </GridView>
            </controls:ListEditorView.View>
        </controls:ListEditorView>
        
        <!-- Zusätzliche Buttons für Batch-Operations -->
        <StackPanel Grid.Row="1" Orientation="Horizontal">
            <TextBlock Text="{Binding SelectedProducts.Count, StringFormat='Ausgewählt: {0}'}" 
                       Margin="5" 
                       VerticalAlignment="Center"/>
            <Button Content="Ausgewählte löschen" 
                    Command="{Binding DeleteSelectedCommand}"
                    Margin="5"/>
        </StackPanel>
    </Grid>
</Window>
```

---

## Styling und Anpassung

### Control-Templates überschreiben

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

## Praktische Beispiele

### Beispiel 1: Kundenverwaltung mit ListEditorView

```xml
<Window>
    <controls:ListEditorView 
        ItemsSource="{Binding Customers}"
        SelectedItem="{Binding SelectedCustomer, Mode=TwoWay}"
        
        AddCommand="{Binding AddCommand}"
        EditCommand="{Binding EditCommand}"
        DeleteCommand="{Binding DeleteCommand}"
        
        AddButtonText="Neuer Kunde"
        EditButtonText="Kunde bearbeiten"
        DeleteButtonText="Kunde löschen">
        
        <controls:ListEditorView.View>
            <GridView>
                <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" Width="200"/>
                <GridViewColumn Header="Email" DisplayMemberBinding="{Binding Email}" Width="250"/>
                <GridViewColumn Header="Telefon" DisplayMemberBinding="{Binding Phone}" Width="150"/>
            </GridView>
        </controls:ListEditorView.View>
        
        <controls:ListEditorView.InputBindings>
            <MouseBinding Gesture="LeftDoubleClick" Command="{Binding EditCommand}"/>
        </controls:ListEditorView.InputBindings>
    </controls:ListEditorView>
</Window>
```

### Beispiel 2: Kompakte Kategorie-Auswahl

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <TextBlock Grid.Column="0" 
               Text="Kategorie:" 
               VerticalAlignment="Center" 
               Margin="0,0,10,0"/>
    
    <controls:DropDownEditorView 
        Grid.Column="1"
        ItemsSource="{Binding Categories}"
        SelectedItem="{Binding SelectedCategory}"
        DisplayMemberPath="Name"
        ButtonPlacement="Right"
        
        AddCommand="{Binding AddCategoryCommand}"
        EditCommand="{Binding EditCategoryCommand}"
        DeleteCommand="{Binding DeleteCategoryCommand}"
        
        AddButtonText="+"
        EditButtonText="??"
        DeleteButtonText="-"
        IsClearVisible="False"/>
</Grid>
```

### Beispiel 3: Master-Detail mit zwei Controls

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <!-- Master: Kunden -->
    <controls:ListEditorView 
        Grid.Column="0"
        Margin="5"
        ItemsSource="{Binding Customers}"
        SelectedItem="{Binding SelectedCustomer}"
        AddCommand="{Binding AddCustomerCommand}"
        EditCommand="{Binding EditCustomerCommand}"
        DeleteCommand="{Binding DeleteCustomerCommand}">
        
        <controls:ListEditorView.View>
            <GridView>
                <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}"/>
            </GridView>
        </controls:ListEditorView.View>
    </controls:ListEditorView>
    
    <!-- Detail: Bestellungen des Kunden -->
    <controls:ListEditorView 
        Grid.Column="1"
        Margin="5"
        ItemsSource="{Binding Orders}"
        SelectedItem="{Binding SelectedOrder}"
        AddCommand="{Binding AddOrderCommand}"
        EditCommand="{Binding EditOrderCommand}"
        DeleteCommand="{Binding DeleteOrderCommand}"
        IsEnabled="{Binding SelectedCustomer, Converter={StaticResource NotNullToBoolConverter}}">
        
        <controls:ListEditorView.View>
            <GridView>
                <GridViewColumn Header="Bestellnr." DisplayMemberBinding="{Binding OrderNumber}"/>
                <GridViewColumn Header="Datum" DisplayMemberBinding="{Binding OrderDate}"/>
            </GridView>
        </controls:ListEditorView.View>
    </controls:ListEditorView>
</Grid>
```

### Beispiel 4: Multi-Selection mit Batch-Operations

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    
    <!-- ListEditorView mit Multi-Selection -->
    <controls:ListEditorView 
        Grid.Row="0"
        SelectionMode="Multiple"
        ItemsSource="{Binding Products}"
        SelectedItem="{Binding SelectedProduct}"
        behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedProducts}"
        
        AddCommand="{Binding AddCommand}"
        EditCommand="{Binding EditCommand}"
        DeleteCommand="{Binding DeleteCommand}">
        
        <controls:ListEditorView.View>
            <GridView>
                <GridViewColumn Header="Produkt" DisplayMemberBinding="{Binding Name}"/>
                <GridViewColumn Header="Preis" DisplayMemberBinding="{Binding Price}"/>
            </GridView>
        </controls:ListEditorView.View>
    </controls:ListEditorView>
    
    <!-- Batch-Operations Panel -->
    <GroupBox Grid.Row="1" Header="Batch-Operationen" Margin="5">
        <StackPanel Orientation="Horizontal">
            <TextBlock Text="{Binding SelectedProducts.Count, StringFormat='Ausgewählt: {0}'}" 
                       Margin="5" 
                       VerticalAlignment="Center"/>
            <Button Content="Ausgewählte löschen" 
                    Command="{Binding DeleteSelectedCommand}"
                    Margin="5"/>
            <Button Content="Preise +10%" 
                    Command="{Binding IncreasePricesCommand}"
                    Margin="5"/>
            <Button Content="Auf Lager setzen" 
                    Command="{Binding MarkAsInStockCommand}"
                    Margin="5"/>
        </StackPanel>
    </GroupBox>
</Grid>
```

### Beispiel 5: DropDownEditorView in Formular

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <!-- Name -->
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Name:" Margin="5"/>
    <TextBox Grid.Row="0" Grid.Column="1" 
             Text="{Binding Name}" 
             Margin="5"/>
    
    <!-- Kategorie mit DropDownEditorView -->
    <TextBlock Grid.Row="1" Grid.Column="0" Text="Kategorie:" Margin="5"/>
    <controls:DropDownEditorView 
        Grid.Row="1" Grid.Column="1"
        ItemsSource="{Binding Categories}"
        SelectedItem="{Binding SelectedCategory}"
        DisplayMemberPath="Name"
        ButtonPlacement="Right"
        AddCommand="{Binding AddCategoryCommand}"
        EditCommand="{Binding EditCategoryCommand}"
        DeleteCommand="{Binding DeleteCategoryCommand}"
        Margin="5"/>
    
    <!-- Status mit DropDownEditorView -->
    <TextBlock Grid.Row="2" Grid.Column="0" Text="Status:" Margin="5"/>
    <controls:DropDownEditorView 
        Grid.Row="2" Grid.Column="1"
        ItemsSource="{Binding StatusList}"
        SelectedItem="{Binding SelectedStatus}"
        DisplayMemberPath="DisplayName"
        ButtonPlacement="Right"
        AddCommand="{Binding AddStatusCommand}"
        IsEditVisible="False"
        IsDeleteVisible="False"
        Margin="5"/>
</Grid>
```

---

## Zusammenfassung

### Wichtigste Punkte

1. **ListEditorView**: ListView mit CRUD-Buttons unterhalb, ideal für Master-Detail
2. **DropDownEditorView**: ComboBox mit konfigurierbarem Button-Layout (Right/Bottom/Top)
3. **MultiSelectBehavior**: Erforderlich für bidirektionale SelectedItems-Bindung
4. **ButtonPlacement**: Right (kompakt), Bottom (ähnlich ListView), Top (professionell)
5. **Integration**: Nahtlose Integration mit EditableCollectionViewModel
6. **Anpassung**: Vollständig anpassbar via Properties, Styles und Templates

### Empfehlungen

| Szenario | Control | ButtonPlacement |
|----------|---------|-----------------|
| Master-Detail Listen | ListEditorView | - |
| Formular-Felder | DropDownEditorView | Right |
| Kompakte Auswahl | DropDownEditorView | Right |
| Professionelle Tools | DropDownEditorView | Top |
| Breite Layouts | DropDownEditorView | Bottom |
| Multi-Selection | ListEditorView + MultiSelectBehavior | - |

### Weiterführende Dokumentation

- [EditableCollectionViewModel Guide](EditableCollectionViewModel_Guide.md) - Commands und CRUD
- [CollectionViewModel Guide](CollectionViewModel_Guide.md) - Basis-Funktionalität
- [Test Guide](CustomWPFControls_TestGuide.md) - Testing von Controls
- [API Reference](../API-Reference.md) - Vollständige API-Dokumentation
