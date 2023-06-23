using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics;

namespace WpfControlToolkit
{
    /// <summary>
    /// ========================================
    /// .NET Framework 3.0 Custom Control
    /// ========================================
    ///
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BreadcrumbBar"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BreadcrumbBar;assembly=BreadcrumbBar"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file. Note that Intellisense in the
    /// XML editor does not currently work on custom controls and its child elements.
    ///
    ///     <MyNamespace:BreadcrumbBar/>
    ///
    /// </summary>
    public class BreadcrumbBar : ItemsControl
    {
        #region Constructors

        static BreadcrumbBar()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbBar), new FrameworkPropertyMetadata(typeof(BreadcrumbBar)));

            CommandManager.RegisterClassCommandBinding(typeof(BreadcrumbBar), new CommandBinding(SelectItemCommand, new ExecutedRoutedEventHandler(OnSelectItem), new CanExecuteRoutedEventHandler(OnCanSelectItem)));
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The hierarchy of objects that are selected. The list is ordered with parents first, followed by children.
        /// </summary>
        public ObservableCollection<object> SelectedHierarchy
        {
            get
            {
                if (_selectedHierarchy == null)
                {
                    _selectedHierarchy = new ObservableCollection<object>();
                    _selectedHierarchy.CollectionChanged += new NotifyCollectionChangedEventHandler(OnSelectedHierarchyChanged);
                }

                return _selectedHierarchy;
            }
        }

        /// <summary>
        ///     The currently selected item. Also, the last item in the SelectedHierarchy list.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
        }

        private static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItem", typeof(object), typeof(BreadcrumbBar), new UIPropertyMetadata(null, null, new CoerceValueCallback(OnCoerceSelectedItem)));

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        private static object OnCoerceSelectedItem(DependencyObject d, object baseValue)
        {
            BreadcrumbBar bar = d as BreadcrumbBar;
            if ((bar != null) && (bar._selectedHierarchy != null))
            {
                int count = bar._selectedHierarchy.Count;
                if (count > 0)
                {
                    return bar._selectedHierarchy[count - 1];
                }
            }

            return null;
        }

        private void OnSelectedHierarchyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CoerceValue(SelectedItemProperty);
            RaiseSelectionChangedEvent();
        }

        /// <summary>
        ///     The command that is invoked to indicate that an item should be selected.
        ///     Set the CommandParameter to the container of the item to be selected.
        ///     The control will travel up the element tree to the BreadcrumbBar where it
        ///     will then select the object hierarchy leading to the desired item.
        /// </summary>
        public static readonly RoutedCommand SelectItemCommand = new RoutedCommand("SelectItem", typeof(BreadcrumbBar));

        /// <summary>
        ///     Template to be used to generate icons.
        ///     The data object will be the same source used for the header.
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(BreadcrumbBar), new UIPropertyMetadata(null));

        /// <summary>
        ///     Template selector to determine the template to use to generate icons.
        /// </summary>
        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(BreadcrumbBar), new UIPropertyMetadata(null));

        #endregion

        #region Events

        /// <summary>
        ///     An event when there are changes in the SelectedHierarchy.
        /// </summary>
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        ///     An event when there are changes in the SelectedHierarchy.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbBar));

        /// <summary>
        ///     Raises the SelectionChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnSelectionChanged(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        private void RaiseSelectionChangedEvent()
        {
            // Don't fire the event if we are in the middle of making changes
            if (!_changingSelection)
            {
                OnSelectionChanged(new RoutedEventArgs(SelectionChangedEvent));
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        ///     Return true if the item is (or should be) its own item container
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is HeaderedItemsControl);
        }

        /// <summary> Create or identify the element used to display the given item. </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeaderedItemsControl();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if ((e.Property == ItemsControl.HasItemsProperty) && ((bool)e.NewValue))
            {
                if ((_selectedHierarchy == null) || (_selectedHierarchy.Count == 0))
                {
                    //  Selects the root item initially
                    SelectItem(Items[0]); // Selection change event will occur automatically
                }
            }
        }

        private static void OnSelectItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            BreadcrumbBar bar = (BreadcrumbBar)sender;
            DependencyObject container = e.Parameter as DependencyObject;

            if (container != null)
            {
                ItemsControl parentContainer = ItemsControl.ItemsControlFromItemContainer(container);
                if (parentContainer != null)
                {
                    object item = parentContainer.ItemContainerGenerator.ItemFromContainer(container);
                    if (item != null)
                    {
                        bar.SelectItem(item, container, parentContainer);
                    }
                }
            }
        }

        private static void OnCanSelectItem(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.ContinueRouting = false;
            e.Handled = true;
        }

        private void SelectItem(object item)
        {
            SelectedHierarchy.Add(item);

            // Do not fire the selection changed event since it will be fired by the caller
        }

        private void SelectItem(object newItem, DependencyObject itemContainer, ItemsControl parentContainer)
        {
            List<object> newItems = new List<object>();
            while ((parentContainer != null) && (parentContainer != this) && (parentContainer.TemplatedParent != this))
            {
                newItems.Add(parentContainer.ItemContainerGenerator.ItemFromContainer(itemContainer));

                itemContainer = parentContainer;
                parentContainer = ItemsControl.ItemsControlFromItemContainer(itemContainer);
            }

            if ((parentContainer != null) && (itemContainer != null))
            {
                object item = parentContainer.ItemContainerGenerator.ItemFromContainer(itemContainer);
                if (item != null)
                {
                    Debug.Assert(_selectedHierarchy != null, "_selectedHierarchy should be non-null");

                    _changingSelection = true;

                    try
                    {

                        int index = _selectedHierarchy.IndexOf(item);
                        if (index >= 0)
                        {
                            // Remove items that are no longer selected
                            int count = _selectedHierarchy.Count;
                            for (int i = count - 1; i > index; i--)
                            {
                                _selectedHierarchy.RemoveAt(i);
                            }

                            if (item != newItem)
                            {
                                // If the item is a child, then select it.
                                // Otherwise, it was a parent, and doesn't need to be re-selected.
                                SelectItems(newItems);
                            }
                        }
                        else
                        {
                            // This is a new selection
                            _selectedHierarchy.Clear();
                            SelectItems(newItems);
                        }

                        // This is to ensure that menus are closed
                        if (IsKeyboardFocusWithin)
                        {
                            Keyboard.Focus(this);
                        }
                    }
                    finally
                    {
                        _changingSelection = false;
                    }

                    RaiseSelectionChangedEvent();
                }
            }
        }

        private void SelectItems(List<object> newItems)
        {
            int count = newItems.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                SelectItem(newItems[i]);
            }

            // Do not fire the selection changed event since it will be fired by the caller
        }

        #endregion

        #region Private Data

        private ObservableCollection<object> _selectedHierarchy;
        private bool _changingSelection;

        #endregion
    }
}
