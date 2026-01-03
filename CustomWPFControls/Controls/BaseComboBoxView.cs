using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace CustomWPFControls.Controls
{
    /// <summary>
    /// Basisklasse für ComboBox-basierte Controls mit Count-Property.
    /// </summary>
    /// <remarks>
    /// Analog zu BaseListView, bietet diese Klasse eine bindbare Count-Property
    /// für ComboBox-Controls. Die Count-Property wird automatisch aktualisiert,
    /// wenn sich die Items-Collection ändert.
    /// </remarks>
    public class BaseComboBoxView : ComboBox
    {
        static BaseComboBoxView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseComboBoxView), 
                new FrameworkPropertyMetadata(typeof(BaseComboBoxView)));
        }

        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(BaseComboBoxView), 
                new PropertyMetadata(0));

        /// <summary>
        /// Anzahl der Items in der ComboBox.
        /// </summary>
        /// <remarks>
        /// Diese Property wird automatisch aktualisiert, wenn sich die Items-Collection ändert.
        /// Kann in XAML gebunden werden, z.B. für Anzeige "Anzahl: {Binding Count}".
        /// </remarks>
        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        /// <summary>
        /// Aktualisiert die Count-Property bei Änderungen der Items-Collection.
        /// </summary>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            SetValue(CountProperty, Items?.Count ?? 0);
        }
    }
}
