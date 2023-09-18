// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Controls.Primitives;

namespace XamlPadEdit
{
    /// <summary>
    /// source item for visual
    /// </summary>
    public class VisualSourceItem
    {
        #region Constructor

        public VisualSourceItem(DependencyObject header)
        {
            _header = header;
        }

        #endregion

        #region Header

        public DependencyObject Header
        {
            get { return _header; }
        }

        private readonly DependencyObject _header;

        #endregion

        #region Type

        public Type Type
        {
            get { return Header.GetType(); }
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
                    FrameworkElement fe = Header as FrameworkElement;
                    if (fe != null && !fe.IsLoaded)
                    {
                        fe.Loaded += OnLoaded;
                    }
                    else
                    {
                        AddVisualChildren();
                    }
                }
                return _items;
            }
        }

        private ObservableCollection<object> _items = null;

        private void AddVisualChildren()
        {
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(Header); i < count; ++i)
            {
                VisualSourceItem tempItem = new VisualSourceItem(VisualTreeHelper.GetChild(Header, i));
                _items.Add(new VisualSourceItem(VisualTreeHelper.GetChild(Header, i)));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            if (fe != null)
            {
                fe.Loaded -= OnLoaded;
                AddVisualChildren();
            }
        }

        #endregion
    }

    /// <summary>
    /// VisualTreeView displays the visual tree of a given visual
    /// </summary>
    public class VisualTreeView : Control
    {
        #region Constructor
        private static VisualTreeView s_treeViewControl;
        private  DependencyObject _currentSelected = null;

        static VisualTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VisualTreeView), 
                new FrameworkPropertyMetadata(typeof(VisualTreeView)));

            //register refresh command
            InputGestureCollection gestures = new InputGestureCollection();
            gestures.Add(new KeyGesture(Key.F5));
            RefreshCommand = new RoutedUICommand("Refresh", "Refresh", typeof(VisualTreeView), gestures);
            CommandManager.RegisterClassCommandBinding(typeof(VisualTreeView),
                new CommandBinding(RefreshCommand, OnRefreshCommand));
        }

        #endregion

        #region Target

        public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Visual), typeof(VisualTreeView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTargetChanged)));

        public Visual Target
        {
            get
            { return (Visual)GetValue(TargetProperty); }
            set
            { SetValue(TargetProperty, value); }
        }

        private  static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VisualTreeView view = d as VisualTreeView;
            s_treeViewControl= view;
            if (view != null)
                view.Init();
        }

        private void addToInterpreterItem_Click(object sender, RoutedEventArgs e)
        {
            VisualTreeView treeView = ((MenuItem)sender).Tag as VisualTreeView;
            if (_currentSelected == null)
            {
                NoSelectionDialog dialog = new NoSelectionDialog();
                dialog.SetText("No Item Is Selected");
                dialog.ShowDialog();
                return;
            }
            Window w = Application.Current.MainWindow;
            XamlPadPage mainPage = LogicalTreeHelper.FindLogicalNode(w, "MainPage") as XamlPadPage;
            if (mainPage != null)
            {

                Window interpreterWindow = mainPage.CommandInterpreter;
                if (interpreterWindow == null)
                {
                    object[] o = new object[0];
                    ReflectionUtils.InvokeInstanceMethod(mainPage, "VisualTreeViewItemSelected", o);
                    interpreterWindow = mainPage.CommandInterpreter;
                }

                CommandParser parser = mainPage.Parser;
                TextBox interpreterResultsBox = LogicalTreeHelper.FindLogicalNode(interpreterWindow, "InterpreterResults") as TextBox;
                if (parser == null)
                {
                    parser = new CommandParser(mainPage.RootElement, interpreterResultsBox);
                    mainPage.Parser = parser;
                }

                string objectName = _currentSelected.GetValue(FrameworkElement.NameProperty) as string;
                string name = "";
                if (objectName != "")
                {
                    name = parser.CreateObjectForTreeItem(_currentSelected, objectName);
                }
                else
                {
                     name= parser.CreateObjectForTreeItem(_currentSelected);
                }
                interpreterResultsBox.Text += "Use Object Name:[" + name + "] for selected item in VisualTree\r\n";
                interpreterResultsBox.Text += "---------------------------------------------\r\n";
                interpreterWindow.Focus();
            }
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
            }

            VisualSourceItem root = new VisualSourceItem(Target);
            ViewTree.Add(root);
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
            VisualSourceItem item = e.NewValue as VisualSourceItem;

            if (item == null)
            {
                SetValue(CurrentPropertyKey, null);
                _currentSelected = null;
            }
            else
            {
                SetValue(CurrentPropertyKey, item.Header);
                _currentSelected = item.Header;
            }
        }

        #endregion

        #region ViewTree

        private static DependencyPropertyKey s_viewTreePropertyKey = DependencyProperty.RegisterReadOnly("ViewTree",
            typeof(ObservableCollection<object>), typeof(VisualTreeView),
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

        #region Current

        public object Current
        {
            get { return GetValue(CurrentProperty); }
        }

        protected static readonly DependencyPropertyKey CurrentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Current",
                typeof(object),
                typeof(VisualTreeView),
                new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CurrentProperty = CurrentPropertyKey.DependencyProperty;

        #endregion

        #region RefreshCommand

        /// <summary>
        /// refresh command is used to refresh the whole tree
        /// </summary>
        public static readonly RoutedUICommand RefreshCommand;

        private static void OnRefreshCommand(object sender, ExecutedRoutedEventArgs e)
        {
            VisualTreeView view = sender as VisualTreeView;
            if (view != null)
                view.Init();
        }

        #endregion
    }
}