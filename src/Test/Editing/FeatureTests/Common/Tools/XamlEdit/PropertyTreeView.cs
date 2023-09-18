// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace XamlPadEdit
{
    /// <summary>
    /// a helper class that is used to create property source item
    /// </summary>
    internal static class PropertyHelper
    {
        private class PropertyInfoComparer : IComparer<PropertyInfo>
        {
            public int Compare(PropertyInfo left, PropertyInfo right)
            {
                if (left == right)
                    return 0;
                if (left == null)
                    return -1;
                if (right == null)
                    return 1;
                return string.Compare(left.Name, right.Name);
            }
        }

        /// <summary>
        /// Add normal properties of an object defined in a specific type into target list
        /// </summary>
        public static void AddNormalProperties(IList target, Type type, object value)
        {
            // Only publicly declared non-static properties will be shown
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            Array.Sort(properties, new PropertyInfoComparer());

            foreach (PropertyInfo property in properties)
            {
                ParameterInfo[] indexers = property.GetIndexParameters();

                // Only properties without indexers are supported now
                if (indexers.Length == 0)
                {
                    PropertyPath propertyPath = null;

                    // if the property is a dependency property
                    if (value is DependencyObject)
                    {
                        FieldInfo fi = value.GetType().GetField(property.Name + "Property",
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                        if (fi != null && fi.FieldType == typeof(DependencyProperty))
                        {
                            propertyPath = new PropertyPath("(0)", fi.GetValue(null));
                        }
                    }

                    // if the property is not a dependency property
                    if (propertyPath == null)
                    {
                        propertyPath = new PropertyPath("(0)", property);
                    }

                    Binding binding = new Binding();
                    binding.Source = value;
                    binding.Mode = BindingMode.OneWay;
                    binding.Path = propertyPath;

                    object o1 = ReflectionUtils.GetProperty(value, property.Name);

                    NormalPropertySourceItem item = new NormalPropertySourceItem(property.PropertyType, property.Name, value, o1);

                    BindingOperations.SetBinding(item, NormalPropertySourceItem.ValueProperty, binding);
                    target.Add(item);
                }
            }
        }

        /// <summary>
        /// Add the base property source item of an object into target list
        /// </summary>
        public static void AddBaseProperty(IList target, Type type, object value)
        {
            if (type != null && type.BaseType != null)
            {
                BasePropertySourceItem baseItem = new BasePropertySourceItem(type.BaseType, value);
                target.Add(baseItem);
            }
        }

        /// <summary>
        /// Add the collection property source item of an object into target list
        /// </summary>
        public static void AddCollectionProperty(IList target, object obj)
        {
            if (obj is IEnumerable)
            {
                PropertyCollectionSourceItem item = new PropertyCollectionSourceItem(typeof(object));

                Binding binding = new Binding();
                binding.Source = obj;
                binding.Mode = BindingMode.OneWay;

                BindingOperations.SetBinding(item, PropertyCollectionSourceItem.ValueProperty, binding);

                target.Add(item);
            }
        }

        /// <summary>
        /// Add all property source items related to a object into target list
        /// </summary>
        public static void AddAllProperties(IList target, object obj)
        {
            if (obj == null)
                return;

            Type type = obj.GetType();
            PropertyHelper.AddBaseProperty(target, type, obj);
            PropertyHelper.AddNormalProperties(target, type, obj);
            PropertyHelper.AddCollectionProperty(target, obj);
        }

        /// <summary>
        /// Get all children property source items from an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void AddChildren(IList target, object obj)
        {
            IEnumerable col = obj as IEnumerable;
            if (col == null)
                return;

            // If it is a dictionary, it is displayed as "[key]".
            IDictionary dic = col as IDictionary;
            if (dic != null)
            {
                foreach (object key in dic.Keys)
                {
                    target.Add(GetCollectionItem("[" + key + "]", dic[key]));
                }
                return;
            }

            // If it is a list, it is displayed as "[index]".
            IList c = obj as IList;
            if (c != null)
            {
                for (int i = 0; i < c.Count; ++i)
                {
                    target.Add(GetCollectionItem("[" + i + "]", c[i]));
                }
                return;
            }

            // If it is an IEnumerable, it is displayed as "[]"
            foreach (object child in col)
            {
                target.Add(GetCollectionItem("[]", child));
            }
            return;
        }

        private static NormalPropertySourceItem GetCollectionItem(string propertyName, object source)
        {
            NormalPropertySourceItem item = new NormalPropertySourceItem(typeof(object), propertyName, source, null);

            Binding binding = new Binding();
            binding.Source = source;
            binding.Mode = BindingMode.OneWay;

            BindingOperations.SetBinding(item, NormalPropertySourceItem.ValueProperty, binding);

            return item;
        }

        /// <summary>
        /// Get the root property item
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static object GetRootProperty(object root)
        {
            NormalPropertySourceItem rootItem = new NormalPropertySourceItem(typeof(object), "Root", root, null);
            rootItem.Value = root;
            return rootItem;
        }
    }

    /// <summary>
    /// The source item for base
    /// </summary>
    public sealed class BasePropertySourceItem : DependencyObject
    {
        #region Name

        public string Name
        {
            get { return "base"; }
        }

        #endregion

        #region Constructor

        public BasePropertySourceItem(Type type, object value)
        {
            _type = type;
            _value = value;
        }

        #endregion

        #region Type

        /// <summary>
        /// the current type
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        private readonly Type _type;

        #endregion

        #region Items

        /// <summary>
        /// the items of the base property source item
        /// </summary>
        public ObservableCollection<object> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<object>();
                    // a base source item has only base property and normal properties.
                    // it does not have collection properties.
                    PropertyHelper.AddBaseProperty(_items, Type, Value);
                    PropertyHelper.AddNormalProperties(_items, Type, Value);
                }
                return _items;
            }
        }

        private ObservableCollection<object> _items = null;

        #endregion

        #region Value

        /// <summary>
        /// the value of the base property source item
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        private readonly object _value;

        #endregion
    }

    /// <summary>
    /// normal property source item
    /// </summary>
    public sealed class NormalPropertySourceItem : DependencyObject
    {
        #region Constructor

        public NormalPropertySourceItem(Type propertyType, string name, object parent, object currentVal)
        {
            _propertyType = propertyType;
            _name = name;
            _parent = parent;
            _currentVal = currentVal;
            SetValue(s_typePropertyKey, PropertyType);
        }

        #endregion

        #region Parent

        public object Parent
        {
            get { return _parent; }
        }

        private object _parent;

        #endregion

        #region Parent

        public object currentVal
        {
            get { return _currentVal; }
        }

        private object _currentVal;

        #endregion

        #region Name

        public string Name
        {
            get { return _name; }
        }

        private string _name;

        #endregion

        #region Value

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(NormalPropertySourceItem),
            new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NormalPropertySourceItem item = d as NormalPropertySourceItem;
            if (item == null)
                return;

            item.OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (_items != null)
            {
                _items.Clear();
                ResetItems();
            }

            Type type = PropertyType;
            if (Value != null)
                type = Value.GetType();
            SetValue(s_typePropertyKey, type);
        }

        #endregion

        #region Type

        /// <summary>
        /// The actual type of Value
        /// If Value is null, Type will be PropertyType
        /// </summary>
        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
        }

        private static readonly DependencyPropertyKey s_typePropertyKey =
            DependencyProperty.RegisterReadOnly("Type",
            typeof(Type), typeof(NormalPropertySourceItem),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TypeProperty = s_typePropertyKey.DependencyProperty;

        #endregion

        #region PropertyType

        /// <summary>
        /// The type of property
        /// The difference between PropertyType and Type can be illustrated by the following example:
        /// View property of ListView whose View is a GridView:
        ///     ViewBase is its PropertyType, while GridView is its Type.
        /// If ListView.View is null:
        ///     ViewBase is its PropertyType and also its Type.
        /// </summary>
        public Type PropertyType
        {
            get { return _propertyType; }
        }

        private readonly Type _propertyType;

        #endregion

        #region Items

        public ObservableCollection<object> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<object>();
                    ResetItems();
                }
                return _items;
            }
        }

        private void ResetItems()
        {
            PropertyHelper.AddAllProperties(_items, Value);
        }

        private ObservableCollection<object> _items = null;

        #endregion
    }

    /// <summary>
    /// collection source item
    /// </summary>
    public sealed class PropertyCollectionSourceItem : DependencyObject
    {
        #region Name

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
        }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(PropertyCollectionSourceItem),
            new PropertyMetadata("Item[0]"));

        #endregion

        #region Value

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(PropertyCollectionSourceItem),
            new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyCollectionSourceItem item = d as PropertyCollectionSourceItem;
            if (item == null)
                return;

            item.OnValueChanged();
        }

        void OnValueChanged()
        {
            if (_items != null)
            {
                _items.Clear();
                ResetItems();
            }

            Type type = Value == null ? PropertyType : Value.GetType();
            SetValue(s_typePropertyKey, type);
        }

        #endregion

        #region Type

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
        }

        private static readonly DependencyPropertyKey s_typePropertyKey =
            DependencyProperty.RegisterReadOnly("Type",
            typeof(Type), typeof(PropertyCollectionSourceItem),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty TypeProperty = s_typePropertyKey.DependencyProperty;

        #endregion

        #region PropertyType

        public Type PropertyType
        {
            get { return _propertyType; }
        }

        private readonly Type _propertyType;

        #endregion

        #region NameConverter

        private class NameConverter : IValueConverter
        {
            #region IValueConverter Members

            object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return "Item[" + (int)value + "]";
            }

            object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Constructor

        public PropertyCollectionSourceItem(Type propertyType)
        {
            Binding nameBinding = new Binding();
            nameBinding.Source = this;
            nameBinding.Path = new PropertyPath("Items.Count");
            nameBinding.Converter = new NameConverter();

            BindingOperations.SetBinding(this, NameProperty, nameBinding);

            _propertyType = propertyType;

            SetValue(s_typePropertyKey, propertyType);
        }

        #endregion

        #region Items

        public ObservableCollection<object> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<object>();
                    ResetItems();
                }
                return _items;
            }
        }

        private void ResetItems()
        {
            PropertyHelper.AddChildren(_items, Value);

            INotifyCollectionChanged ncc = Value as INotifyCollectionChanged;
            if (ncc == null)
                return;

            ncc.CollectionChanged += OnCollectionChanged;
        }

        private ObservableCollection<object> _items = null;

        #endregion

        #region CollectionChanged

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshItems();
        }

        private void RefreshItems()
        {
            if (_items != null)
            {
                _items.Clear();
                PropertyHelper.AddChildren(_items, Value);
            }
        }

        #endregion
    }

    /// <summary>
    /// Property tree view displays an object's property tree
    /// </summary>
    public class PropertyTreeView : Control
    {
        private static PropertyTreeView s_treeViewControl;
        #region Constructor

        static PropertyTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyTreeView),
                new FrameworkPropertyMetadata(typeof(PropertyTreeView)));

            //register refresh command
            InputGestureCollection gestures = new InputGestureCollection();
            gestures.Add(new KeyGesture(Key.F5));
            RefreshCommand = new RoutedUICommand("Refresh", "Refresh", typeof(PropertyTreeView), gestures);
            CommandManager.RegisterClassCommandBinding(typeof(PropertyTreeView),
                new CommandBinding(RefreshCommand, OnRefreshCommand));
        }

        #endregion

        #region ApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TreeView tv = GetTemplateChild("PART_Tree") as TreeView;
            if (tv != null)
            {
                tv.SelectedItemChanged += OnSelectedItemChanged;
            }
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            NormalPropertySourceItem item = null;
            if (e.NewValue is NormalPropertySourceItem)
            {
                item = e.NewValue as NormalPropertySourceItem;
            }
            //VisualSourceItem item = e.NewValue as VisualSourceItem;

            if (item == null)
            {
                //SetValue(CurrentPropertyKey, null);
                _currentSelected1 = null;
            }
            else
            {
                //SetValue(CurrentPropertyKey, item.Header);
                _currentSelected1 = item as object;
            }
        }

        object _currentSelected1 = null;
        #endregion

        #region Target

        /// <summary>
        /// the object whose property tree will be displayed
        /// </summary>
        public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(PropertyTreeView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTargetChanged)));

        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyTreeView view = d as PropertyTreeView;
            s_treeViewControl = view;
            if (view != null)
                view.Init();
        }

        private void Init()
        {
            ViewTree.Clear();

            if (Target == null)
                return;

            if (s_treeViewControl.ContextMenu.Items.Count == 1)
            {
                MenuItem addToInterpreterItem = new MenuItem();
                addToInterpreterItem.Header = "Send to Command Interpreter";
                addToInterpreterItem.Tag = s_treeViewControl;
                addToInterpreterItem.Click += new RoutedEventHandler(addToInterpreterItem_Click);
                s_treeViewControl.ContextMenu.Items.Add(addToInterpreterItem);
                s_treeViewControl.ContextMenuOpening += new ContextMenuEventHandler(treeViewControl_ContextMenuOpening);
            }

            if (ShowsRoot)
            {
                ViewTree.Add(PropertyHelper.GetRootProperty(Target));
            }
            else
            {
                PropertyHelper.AddAllProperties(ViewTree, Target);
            }
        }

        void treeViewControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (_currentSelected1 == null)
            {
                ((MenuItem)(s_treeViewControl.ContextMenu.Items[1])).IsEnabled = false;
            }
            else
            {
                ((MenuItem)(s_treeViewControl.ContextMenu.Items[1])).IsEnabled = true;
            }
        }


        void addToInterpreterItem_Click(object sender, RoutedEventArgs e)
        {
            bool selectedParent = false;
            object o = null;

            if (_currentSelected1 == null)
            {
                //NoSelectionDialog dialog = new NoSelectionDialog();
                //dialog.SetText("No Item Is Selected");
                //dialog.ShowDialog();
                return;
            }
            if ((((NormalPropertySourceItem)_currentSelected1).PropertyType.IsPrimitive == true) &&
            (((NormalPropertySourceItem)_currentSelected1).Parent.GetType().IsPrimitive))
            {
                _currentSelected1 = null;
                return;
            }
            else
                if ((((NormalPropertySourceItem)_currentSelected1).currentVal != null) &&
               ((((NormalPropertySourceItem)_currentSelected1).currentVal is FrameworkElement) ||
               (((NormalPropertySourceItem)_currentSelected1).currentVal is ContentElement) ||
               (((NormalPropertySourceItem)_currentSelected1).currentVal.ToString().Contains("System.Windows.Documents"))))
                {
                    _currentSelected1 = ((NormalPropertySourceItem)_currentSelected1).currentVal;
                }
                else
                    if ((((NormalPropertySourceItem)_currentSelected1).Parent is FrameworkElement) ||
                    (((NormalPropertySourceItem)_currentSelected1).Parent is ContentElement) ||
                    (((NormalPropertySourceItem)_currentSelected1).Parent.ToString().Contains("System.Windows.Documents")))
                    {
                        selectedParent = true;
                        o = _currentSelected1;
                        _currentSelected1 = ((NormalPropertySourceItem)_currentSelected1).Parent;

                    }
                    else
                    {
                        _currentSelected1 = null;
                        return;
                    }

           
            Window w = Application.Current.MainWindow;
            XamlPadPage mainPage = LogicalTreeHelper.FindLogicalNode(w, "MainPage") as XamlPadPage;
            if (mainPage != null)
            {

                Window interpreterWindow = mainPage.CommandInterpreter;
                if (interpreterWindow == null)
                {
                    object[] o1 = new object[0];
                    ReflectionUtils.InvokeInstanceMethod(mainPage, "VisualTreeViewItemSelected", o1);
                    interpreterWindow = mainPage.CommandInterpreter;
                }

                CommandParser parser = mainPage.Parser;
                TextBox interpreterResultsBox = LogicalTreeHelper.FindLogicalNode(interpreterWindow, "InterpreterResults") as TextBox;
                if (parser == null)
                {
                    parser = new CommandParser(mainPage.RootElement, interpreterResultsBox);
                    mainPage.Parser = parser;
                }

                string objectName = (_currentSelected1 is DependencyObject) ? ((DependencyObject)_currentSelected1).GetValue(FrameworkElement.NameProperty) as string : "";
                string name = "";

                if (objectName != "")
                {
                    name = parser.CreateObjectForTreeItem(_currentSelected1, objectName);
                    //  name = parser.CreateObjectForTreeItem(currentSelected);
                }
                else
                {
                    name = parser.CreateObjectForTreeItem(_currentSelected1);
                }

                interpreterResultsBox.Text += "Use Object Name:[" + name + "] for item [" + _currentSelected1.ToString().Split('.')[_currentSelected1.ToString().Split('.').Length - 1] + "] \r\n";
                interpreterResultsBox.Text += "---------------------------------------------\r\n";
                interpreterWindow.Focus();
            }
            if (selectedParent == true)
            {
                _currentSelected1 = o;
                selectedParent = false;
            }
        }

        #endregion

        #region ShowsRoot

        /// <summary>
        /// Whether or not the root element is shown.
        /// If ShowsRoot is false, its children will be the topmost elements to show.
        /// </summary>
        public bool ShowsRoot
        {
            get { return (bool)GetValue(ShowsRootProperty); }
            set { SetValue(ShowsRootProperty, value); }
        }

        public static readonly DependencyProperty ShowsRootProperty =
            DependencyProperty.Register("ShowsRoot", typeof(bool), typeof(PropertyTreeView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowsRootChanged)));

        private static void OnShowsRootChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyTreeView view = d as PropertyTreeView;
            if (view != null)
                view.Init();
        }

        #endregion

        #region ViewTree

        private static DependencyPropertyKey s_viewTreePropertyKey = DependencyProperty.RegisterReadOnly("ViewTree",
            typeof(ObservableCollection<object>), typeof(PropertyTreeView),
            new PropertyMetadata((ObservableCollection<object>)null));

        public static DependencyProperty ViewTreeProperty = s_viewTreePropertyKey.DependencyProperty;

        public ObservableCollection<object> ViewTree
        {
            get
            {
                ObservableCollection<object> ts = (ObservableCollection<object>)GetValue(ViewTreeProperty);
                if (ts == null)
                {
                    ts = new ObservableCollection<object>();
                    SetValue(s_viewTreePropertyKey, ts);
                }
                return ts;
            }
        }

        #endregion

        #region RefreshCommand

        /// <summary>
        /// refresh command is used to refresh the whole tree
        /// </summary>
        public static readonly RoutedUICommand RefreshCommand;

        private static void OnRefreshCommand(object sender, ExecutedRoutedEventArgs e)
        {
            PropertyTreeView view = sender as PropertyTreeView;

            if (view != null)
                view.Init();
        }

        #endregion
    }
}
