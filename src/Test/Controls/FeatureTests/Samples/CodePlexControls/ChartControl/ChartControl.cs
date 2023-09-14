using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace WpfControlToolkit
{
    [ContentPropertyAttribute("Items")]
    public abstract class ChartControl : FrameworkElement
    {
        public ChartControl()
        {
            _items = new ObservableCollection<object>();
            _items.CollectionChanged += CollectionChanged;
            _collectionChangedHandler = ItemsSourceItemsChanged;
        }

        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        public static DependencyProperty ValuePathProperty
            = DependencyProperty.Register("ValuePath", typeof(string), typeof(ChartControl));

        /// <summary>
        /// Name of property to bind the labels of each bar/pie slice to
        /// </summary>
        public string NamePath
        {
            get { return (string)GetValue(NamePathProperty); }
            set { SetValue(NamePathProperty, value); }
        }

        public static DependencyProperty NamePathProperty
            = DependencyProperty.Register("NamePath", typeof(string), typeof(ChartControl));

        /// <summary>
        /// Allows the user to specify children in XAML instead of binding
        /// ItemsSource to a list
        /// </summary>
        public ObservableCollection<object> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// The list representing the Effective ItemsSource, whether it be Items or ItemsSource
        /// </summary>
        protected IList EffectiveItemsSource
        {
            get
            {
                if (ItemsSource != null && Items.Count == 0)
                {
                    return ItemsSource;
                }
                else
                {
                    return Items;
                }
            }
        }

        /// <summary>
        /// Allows user to get/set the List of objects to be represented
        /// in the graph
        /// </summary>
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty
            = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(ChartControl),
                                          new FrameworkPropertyMetadata((IList)null,
                                                                        new PropertyChangedCallback(ItemsSourceChanged)));

        /// <summary>
        /// Throw an exception if a user tries to add children to the collection while
        /// the collection is in "ItemsSource" mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource != null)
            {
                throw new InvalidOperationException("In ItemsSource Mode, cannot add children to Items.");
            }

            // If NamePath has not been set, check if it is a ChartItem object, and set NamePath to "Name"
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                ChartItem ci = e.NewItems[0] as ChartItem;
                if (ci != null)
                {
                    NamePath = "Name";
                    ValuePath = "Value";
                }
            }
        }

        /// <summary>
        /// This handler handles the case where an item is added or removed from the ItemsSource
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemsSourceItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            VerifyAccess();

            // Check to make sure that Items is empty before setting ItemsSource
            if (Items.Count > 0)
            {
                throw new InvalidOperationException("There are items in Items collection, cannot add/remove items from ItemsSource.");
            }

            OnCollectionInvalidated();
        }

        /// <summary>
        /// This handles the case where the ItemsSource is completely replaced
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartControl cc = (ChartControl)d;
            cc.ItemsSourceChanged((IList)e.NewValue, (IList)e.OldValue);
        }

        private void ItemsSourceChanged(IList newValue, IList oldValue)
        {
            // Check to make sure that Items is empty before setting ItemsSource
            if (Items.Count > 0)
            {
                throw new InvalidOperationException("There are items in Items collection, cannot set ItemsSource.");
            }

            if (oldValue != null)
            {
                INotifyCollectionChanged oldCollection = oldValue as INotifyCollectionChanged;
                if (oldCollection != null)
                {
                    oldCollection.CollectionChanged -= _collectionChangedHandler;
                }
            }
            if (newValue != null)
            {
                INotifyCollectionChanged newCollection = newValue as INotifyCollectionChanged;
                if (newCollection != null)
                {
                    newCollection.CollectionChanged += _collectionChangedHandler;
                }
            }

            OnCollectionInvalidated();
        }


        /// <summary>
        /// When ItemsSource is changed, on default, allow the user to override
        /// this method if additional or different functionality is required
        /// </summary>
        protected virtual void OnCollectionInvalidated()
        {   
        }

        private ObservableCollection<object> _items;
        private readonly NotifyCollectionChangedEventHandler _collectionChangedHandler;
    }
}
