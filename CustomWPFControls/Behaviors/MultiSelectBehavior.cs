using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace CustomWPFControls.Behaviors
{
    /// <summary>
    /// Attached Behavior für bidirektionale Synchronisation von ListBox.SelectedItems mit ViewModel-Collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// WPF's ListBox.SelectedItems ist ReadOnly und kann nicht direkt gebunden werden.
    /// Dieses Behavior ermöglicht Two-Way Synchronisation zwischen ListBox und ViewModel.
    /// </para>
    /// <para>
    /// <b>Verwendung:</b>
    /// <code>
    /// &lt;ListBox SelectionMode="Multiple"
    ///          ItemsSource="{Binding Items}"
    ///          behaviors:MultiSelectBehavior.SelectedItems="{Binding SelectedItems}" /&gt;
    /// </code>
    /// </para>
    /// </remarks>
    public static class MultiSelectBehavior
    {
        /// <summary>
        /// Attached Property für SelectedItems-Binding.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(MultiSelectBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        /// <summary>
        /// Setzt die SelectedItems-Collection für einen ListBox.
        /// </summary>
        public static void SetSelectedItems(DependencyObject element, IList? value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        /// <summary>
        /// Gibt die SelectedItems-Collection eines ListBox zurück.
        /// </summary>
        public static IList? GetSelectedItems(DependencyObject element)
        {
            return (IList?)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox)
                return;

            // Unsubscribe old collection
            if (e.OldValue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= (s, args) => OnViewModelCollectionChanged(listBox, args);
            }

            // Subscribe new collection
            if (e.NewValue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += (s, args) => OnViewModelCollectionChanged(listBox, args);
                
                // Initial sync: ViewModel ? ListBox
                listBox.SelectedItems.Clear();
                if (e.NewValue is IList newList)
                {
                    foreach (var item in newList)
                    {
                        if (!listBox.SelectedItems.Contains(item))
                        {
                            listBox.SelectedItems.Add(item);
                        }
                    }
                }
            }

            // Subscribe to ListBox SelectionChanged
            listBox.SelectionChanged -= OnListBoxSelectionChanged;
            listBox.SelectionChanged += OnListBoxSelectionChanged;
        }

        private static void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox listBox)
                return;

            var targetList = GetSelectedItems(listBox);
            if (targetList == null)
                return;

            // Sync ListBox ? ViewModel
            foreach (var item in e.RemovedItems)
            {
                if (targetList.Contains(item))
                {
                    targetList.Remove(item);
                }
            }

            foreach (var item in e.AddedItems)
            {
                if (!targetList.Contains(item))
                {
                    targetList.Add(item);
                }
            }
        }

        private static void OnViewModelCollectionChanged(ListBox listBox, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                listBox.SelectedItems.Clear();
                return;
            }

            // Sync ViewModel ? ListBox
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (listBox.SelectedItems.Contains(item))
                    {
                        listBox.SelectedItems.Remove(item);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (!listBox.SelectedItems.Contains(item))
                    {
                        listBox.SelectedItems.Add(item);
                    }
                }
            }
        }
    }
}
