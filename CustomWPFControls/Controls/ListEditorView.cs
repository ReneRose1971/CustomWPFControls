using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomWPFControls.Controls
{
    public class ListEditorView : BaseListView
    {
        static ListEditorView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListEditorView), new FrameworkPropertyMetadata("ListEditorViewStyle"));
        }

        // ????????????????????????????????????????????????????????????
        // Command Properties
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(ListEditorView), new PropertyMetadata(null));

        public ICommand? AddCommand
        {
            get => (ICommand?)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(ListEditorView), new PropertyMetadata(null));

        public ICommand? DeleteCommand
        {
            get => (ICommand?)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public static readonly DependencyProperty ClearCommandProperty =
            DependencyProperty.Register("ClearCommand", typeof(ICommand), typeof(ListEditorView), new PropertyMetadata(null));

        public ICommand? ClearCommand
        {
            get => (ICommand?)GetValue(ClearCommandProperty);
            set => SetValue(ClearCommandProperty, value);
        }

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(ListEditorView), new PropertyMetadata(null));

        public ICommand? EditCommand
        {
            get => (ICommand?)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        // ????????????????????????????????????????????????????????????
        // Visibility Properties
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty IsAddVisibleProperty =
            DependencyProperty.Register("IsAddVisible", typeof(bool), typeof(ListEditorView), new PropertyMetadata(true));

        public bool IsAddVisible
        {
            get => (bool)GetValue(IsAddVisibleProperty);
            set => SetValue(IsAddVisibleProperty, value);
        }

        public static readonly DependencyProperty IsDeleteVisibleProperty =
            DependencyProperty.Register("IsDeleteVisible", typeof(bool), typeof(ListEditorView), new PropertyMetadata(true));

        public bool IsDeleteVisible
        {
            get => (bool)GetValue(IsDeleteVisibleProperty);
            set => SetValue(IsDeleteVisibleProperty, value);
        }

        public static readonly DependencyProperty IsClearVisibleProperty =
            DependencyProperty.Register("IsClearVisible", typeof(bool), typeof(ListEditorView), new PropertyMetadata(true));

        public bool IsClearVisible
        {
            get => (bool)GetValue(IsClearVisibleProperty);
            set => SetValue(IsClearVisibleProperty, value);
        }

        public static readonly DependencyProperty IsEditVisibleProperty =
            DependencyProperty.Register("IsEditVisible", typeof(bool), typeof(ListEditorView), new PropertyMetadata(true));

        public bool IsEditVisible
        {
            get => (bool)GetValue(IsEditVisibleProperty);
            set => SetValue(IsEditVisibleProperty, value);
        }

        // ????????????????????????????????????????????????????????????
        // Button Text Properties (für Lokalisierung)
        // ????????????????????????????????????????????????????????????

        public static readonly DependencyProperty AddButtonTextProperty =
            DependencyProperty.Register("AddButtonText", typeof(string), typeof(ListEditorView), new PropertyMetadata("Hinzufügen"));

        public string AddButtonText
        {
            get => (string)GetValue(AddButtonTextProperty);
            set => SetValue(AddButtonTextProperty, value);
        }

        public static readonly DependencyProperty EditButtonTextProperty =
            DependencyProperty.Register("EditButtonText", typeof(string), typeof(ListEditorView), new PropertyMetadata("Bearbeiten"));

        public string EditButtonText
        {
            get => (string)GetValue(EditButtonTextProperty);
            set => SetValue(EditButtonTextProperty, value);
        }

        public static readonly DependencyProperty DeleteButtonTextProperty =
            DependencyProperty.Register("DeleteButtonText", typeof(string), typeof(ListEditorView), new PropertyMetadata("Löschen"));

        public string DeleteButtonText
        {
            get => (string)GetValue(DeleteButtonTextProperty);
            set => SetValue(DeleteButtonTextProperty, value);
        }

        public static readonly DependencyProperty ClearButtonTextProperty =
            DependencyProperty.Register("ClearButtonText", typeof(string), typeof(ListEditorView), new PropertyMetadata("Alle löschen"));

        public string ClearButtonText
        {
            get => (string)GetValue(ClearButtonTextProperty);
            set => SetValue(ClearButtonTextProperty, value);
        }
    }
}