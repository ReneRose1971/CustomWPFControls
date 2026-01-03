using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomWPFControls.Controls
{
    /// <summary>
    /// ComboBox mit integrierten CRUD-Commands und konfigurierbarer Button-Platzierung.
    /// </summary>
    /// <remarks>
    /// <para>
    /// DropDownEditorView erweitert BaseComboBoxView um Command-Properties und 
    /// konfigurierbare Button-Platzierung via ButtonPlacement-Property.
    /// </para>
    /// <para>
    /// <b>Button-Layouts:</b>
    /// - Right: Buttons rechts neben der ComboBox (kompakt, inline)
    /// - Bottom: Buttons unter der ComboBox (ähnlich wie ListEditorView)
    /// - Top: Buttons in ToolBar über der ComboBox (professionell)
    /// </para>
    /// <para>
    /// <b>Integration mit EditableCollectionViewModel:</b>
    /// Bindet direkt an AddCommand, EditCommand, DeleteCommand, ClearCommand.
    /// </para>
    /// </remarks>
    public class DropDownEditorView : BaseComboBoxView
    {
        static DropDownEditorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownEditorView), 
                new FrameworkPropertyMetadata("DropDownEditorViewStyle"));
        }

        // ????????????????????????????????????????????????????????????
        // Layout Configuration
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty ButtonPlacementProperty =
            DependencyProperty.Register("ButtonPlacement", typeof(ButtonPlacement), 
                typeof(DropDownEditorView), new PropertyMetadata(ButtonPlacement.Right));

        /// <summary>
        /// Definiert die Position der Action-Buttons.
        /// </summary>
        /// <remarks>
        /// - Right: Buttons rechts neben der ComboBox (Standard)
        /// - Bottom: Buttons unter der ComboBox
        /// - Top: Buttons in ToolBar über der ComboBox
        /// </remarks>
        public ButtonPlacement ButtonPlacement
        {
            get => (ButtonPlacement)GetValue(ButtonPlacementProperty);
            set => SetValue(ButtonPlacementProperty, value);
        }

        // ????????????????????????????????????????????????????????????
        // Command Properties
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), 
                typeof(DropDownEditorView), new PropertyMetadata(null));

        public ICommand? AddCommand
        {
            get => (ICommand?)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), 
                typeof(DropDownEditorView), new PropertyMetadata(null));

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), 
                typeof(DropDownEditorView), new PropertyMetadata(null));

        public ICommand? EditCommand
        {
            get => (ICommand?)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), 
                typeof(DropDownEditorView), new PropertyMetadata(null));

        public ICommand? ClearCommand
        {
            get => (ICommand?)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        // ????????????????????????????????????????????????????????????
        // Visibility Properties
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty IsAddVisibleProperty =
            DependencyProperty.Register("IsAddVisible", typeof(bool), 
                typeof(DropDownEditorView), new PropertyMetadata(true));

        public bool IsAddVisible
        {
            get => (bool)GetValue(IsAddVisibleProperty);
            set => SetValue(IsAddVisibleProperty, value);
        }

        public static readonly DependencyProperty IsDeleteVisibleProperty =
            DependencyProperty.Register("IsDeleteVisible", typeof(bool), 
                typeof(DropDownEditorView), new PropertyMetadata(true));

        public bool IsDeleteVisible
        {
            get => (bool)GetValue(IsDeleteVisibleProperty);
            set => SetValue(IsDeleteVisibleProperty, value);
        }

        public static readonly DependencyProperty IsEditVisibleProperty =
            DependencyProperty.Register("IsEditVisible", typeof(bool), 
                typeof(DropDownEditorView), new PropertyMetadata(true));

        public bool IsEditVisible
        {
            get => (bool)GetValue(IsEditVisibleProperty);
            set => SetValue(IsEditVisibleProperty, value);
        }

        public static readonly DependencyProperty IsClearVisibleProperty =
            DependencyProperty.Register("IsClearVisible", typeof(bool), 
                typeof(DropDownEditorView), new PropertyMetadata(false));

        /// <summary>
        /// Sichtbarkeit des Clear-Buttons (Standard: false, da für ComboBox unüblich).
        /// </summary>
        public bool IsClearVisible
        {
            get => (bool)GetValue(IsClearVisibleProperty);
            set => SetValue(IsClearVisibleProperty, value);
        }

        // ????????????????????????????????????????????????????????????
        // Button Text Properties (für Lokalisierung)
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty AddButtonTextProperty =
            DependencyProperty.Register("AddButtonText", typeof(string), 
                typeof(DropDownEditorView), new PropertyMetadata("Hinzufügen"));

        public string AddButtonText
        {
            get => (string)GetValue(AddButtonTextProperty);
            set => SetValue(AddButtonTextProperty, value);
        }

        public static readonly DependencyProperty EditButtonTextProperty =
            DependencyProperty.Register("EditButtonText", typeof(string), 
                typeof(DropDownEditorView), new PropertyMetadata("Bearbeiten"));

        public string EditButtonText
        {
            get => (string)GetValue(EditButtonTextProperty);
            set => SetValue(EditButtonTextProperty, value);
        }

        public static readonly DependencyProperty DeleteButtonTextProperty =
            DependencyProperty.Register("DeleteButtonText", typeof(string), 
                typeof(DropDownEditorView), new PropertyMetadata("Löschen"));

        public string DeleteButtonText
        {
            get => (string)GetValue(DeleteButtonTextProperty);
            set => SetValue(DeleteButtonTextProperty, value);
        }

        public static readonly DependencyProperty ClearButtonTextProperty =
            DependencyProperty.Register("ClearButtonText", typeof(string), 
                typeof(DropDownEditorView), new PropertyMetadata("Alle löschen"));

        public string ClearButtonText
        {
            get => (string)GetValue(ClearButtonTextProperty);
            set => SetValue(ClearButtonTextProperty, value);
        }
    }
}
