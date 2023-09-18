// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Media;

namespace XamlPadEdit
{
    /// <summary>
    /// ListTreeView is a control that can show hierarchical data with tree and columns.
    /// Its instances can be found in Visual Studio. VS uses ListTreeView to display properties
    /// of an object in debug mode.
    /// 
    /// ListTreeView derives from TreeView and uses components of ListView to construct columns.
    /// The components from ListView include GridViewColumn, GridViewHeaderRowPresenter, and
    /// GridViewRowPresenter.
    /// 
    /// The main difficulty of ListTreeView is how to align column cells. Here are the main steps.
    /// 1. Use GridViewRowPresenter instead of ContentPresenter to display item data. The result
    ///    is listed below. Because there is an indent before GridViewRowPresenter, its cells do
    ///    not align with its columns.
    ///    +------------+-----------+
    ///    |1st column  |2nd column |
    ///    +------------+-----------+
    ///    +-+ +------------+-----------+
    ///    |+| |1st cell    |2nd cell   |
    ///    +-+ +------------+-----------+
    /// 
    /// 2. Derive a class called <code>IndentedRowPresetner</code> from <code>
    ///    GridViewRowPresenter</code>. And make the first column cell indented by setting
    ///    its Margin in overrided MeasureOverride(). The exact length of indent is the
    ///    length between the start point of ListTreeView and the start point of ListTreeViewItem.
    ///    +------------+-----------+
    ///    |1st column  |2nd column |
    ///    +------------+-----------+
    ///    +-+     +--------+-----------+
    ///    |+|     |1st cell|2nd cell   |
    ///    +-+     +--------+-----------+
    /// 
    /// 3. Set the margin of the <code>IndentedRowPresenter</code> to be negative value to
    ///    make it moved forward an amount of length. The length is the same of the indent.
    ///    so finally the cells are aligned with their columns. NOTE: there is a special
    ///    case here when the indent is larger than the column's actual width. In this case,
    ///    the forward length should be the actual width because the second cell and cells
    ///    following should not be indented. Or the width of the ListTreeViewItem will be
    ///    incorrect. It is less than expected. What value the margin should be is calculated
    ///    in <code>MarginConverter</code>.
    ///    +------------+-----------+
    ///    |1st column  |2nd column |
    ///    +------------+-----------+
    ///    +-+ +--------+-----------+
    ///    |+| |1st cell|2nd cell   |
    ///    +-+ +--------+-----------+
    /// 
    /// 4. There is a still a problem when the first cell is indented so much that its width
    ///    is nagative (see the first graph for details). In this scenario, the first cell is
    ///    invisible. But the second cell is still there, and it is not indented (and it
    ///    should not be indented). This will cause the second cell and other cells following
    ///    not aligned with their columns. The solution is still using margin: setting the
    ///    margin of rows's outside border to be nagative. So the border will be moved forwad
    ///    an amount of length. As the result, the cells can align with their columns. The
    ///    margin of the border is calculated in <code>ForwardConverter</code>. See the second
    ///    graph for the result.
    ///    +------------+-----------+-----------+
    ///    |1st column  |2nd column |3rd column |
    ///    +------------+-----------+-----------+
    ///                +-+ +-----------+-----------+
    ///                |+| |2nd cell   |3rd cell   |
    ///                +-+ +-----------+-----------+
    ///    
    ///    +------------+-----------+-----------+
    ///    |1st column  |2nd column |3rd column |
    ///    +------------+-----------+-----------+
    ///                ++-----------+-----------+
    ///                ||2nd cell   |3rd cell   |
    ///                ++-----------+-----------+
    /// 
    /// </summary>
    public class ListTreeView : TreeView
    {
        #region Constructor

        static ListTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListTreeView), new FrameworkPropertyMetadata(typeof(ListTreeView)));
        }

        #endregion

        #region Columns

        /// <summary>
        /// Column list
        /// </summary>
        public GridViewColumnCollection Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new GridViewColumnCollection();
                }
                return _columns;
            }
        }

        private GridViewColumnCollection _columns;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create or identify the UI element used to display items.
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListTreeViewItem();
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemContainer
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListTreeViewItem;
        }

        #endregion
    }

    /// <summary>
    /// IndentedRowPresenter is responsible for ListTreeViewItems's cells' layout
    /// It will indent the first column to make ListTreeViewItems align with assosiated column.
    /// </summary>
    public class IndentedRowPresenter : GridViewRowPresenter
    {
        #region Constructor

        static IndentedRowPresenter()
        {
            ColumnsProperty.OverrideMetadata(typeof(IndentedRowPresenter),
                new FrameworkPropertyMetadata(OnColumnsChanged));
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IndentedRowPresenter irp = d as IndentedRowPresenter;
            if (irp != null)
            {
                irp._needsUpdate = true;
                GridViewColumnCollection oldColumn = e.OldValue as GridViewColumnCollection;
                GridViewColumnCollection newColumn = e.NewValue as GridViewColumnCollection;
                if (oldColumn != null)
                {
                    oldColumn.CollectionChanged -= irp.OnColumnCollectionChanged;
                }
                if (newColumn != null)
                {
                    newColumn.CollectionChanged += irp.OnColumnCollectionChanged;
                }
            }
        }

        private void OnColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _needsUpdate = true;
        }

        private bool _needsUpdate = false;

        #endregion

        #region Public properties

        /// <summary>
        /// the indent lenght per level
        /// </summary>
        public double IndentPerLevel
        {
            get { return (double)GetValue(IndentPerLevelProperty); }
            set { SetValue(IndentPerLevelProperty, value); }
        }

        public static readonly DependencyProperty IndentPerLevelProperty =
            DependencyProperty.Register(
                "IndentPerLevel",
                typeof(double),
                typeof(IndentedRowPresenter),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure)
            );

        /// <summary>
        /// the level that it is located at
        /// </summary>
        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register(
                "Level",
                typeof(int),
                typeof(IndentedRowPresenter),
                new FrameworkPropertyMetadata(
                    0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure)
            );

        #endregion

        #region Protected methods

        /// <summary>
        /// Override MeasureOverride to indent the first column cell
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (_needsUpdate)
            {
                _needsUpdate = false;
                if (Columns != null && Columns.Count > 0)
                {
                    // Indent the 1st cell
                    double actualIndent = IndentPerLevel * (Level + 1);
                    FrameworkElement fe = GetVisualChild(0) as FrameworkElement;
                    if (fe != null)
                    {
                        Thickness orig = fe.Margin;
                        fe.Margin = new Thickness(actualIndent, orig.Top, orig.Right, orig.Bottom);
                    }

                    // Set TextTrimming of its children to be CharacterEllipsis if they are TextBlocks
                    for (int i = 0, count = Columns.Count; i < count; ++i)
                    {
                        TextBlock tb = GetVisualChild(i) as TextBlock;
                        if (tb != null)
                        {
                            tb.TextTrimming = TextTrimming.CharacterEllipsis;
                        }
                    }
                }
            }
            return base.MeasureOverride(constraint);
        }

        #endregion
    }

    /// <summary>
    /// Control that implements a selectable item inside a ListTreeView.
    /// </summary>
    public class ListTreeViewItem : TreeViewItem
    {
        #region Constructor

        static ListTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListTreeViewItem), new FrameworkPropertyMetadata(typeof(ListTreeViewItem)));
        }

        #endregion

        #region Level

        /// <summary>
        /// The level that the item is located.
        /// The level of the topmost item inside ListTreeView is 0;
        /// the second topmost is 1, and so on down.
        /// </summary>
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    ListTreeViewItem parent = ItemsControl.ItemsControlFromItemContainer(this) as ListTreeViewItem;
                    _level = (parent != null) ? parent.Level + 1 : 0;
                }
                return _level;
            }
        }

        private int _level = -1;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create or identify the UI element used to display items.
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            ListTreeViewItem container = new ListTreeViewItem();
            return container;
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemContainer
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ListTreeViewItem;
        }

        #endregion
    }

    /// <summary>
    /// ListTreeViewItemRow displays the row inside ListTreeViewItem
    /// It makes it convenient to customize the visual of ListTreeView
    /// </summary>
    public class ListTreeViewItemRow : Control
    {
        #region Constructor

        static ListTreeViewItemRow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListTreeViewItemRow),
                new FrameworkPropertyMetadata(typeof(ListTreeViewItemRow)));
            FocusableProperty.OverrideMetadata(typeof(ListTreeViewItemRow),
                new FrameworkPropertyMetadata(false));
        }

        #endregion

        #region Content

        /// <summary>
        /// The content of the row control
        /// </summary>
        public static readonly DependencyProperty ContentProperty = ContentControl.ContentProperty.AddOwner(typeof(ListTreeViewItemRow));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion
    }

    /// <summary>
    /// ForwardConverter calculates the forward length of the outer border based on the actual indent
    /// The forward length will keep beeing 0.0 until the actual indent is greater than the first
    /// column's actual width. In that case its value will be the first column's width minus actual indent.
    /// </summary>
    public class ForwardConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values.Length == 2 && values[0] is int && values[1] is double))
                return 0.0;

            int level = (int)values[0];
            double actualWidth = (double)values[1];
            double left = Math.Min(0.0, actualWidth - (level + 1) * IndentPerLevel);
            return new Thickness(left, 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IndentPerLevel

        /// <summary>
        /// the indent lenght per level
        /// </summary>
        public double IndentPerLevel
        {
            get { return _indentPerLevel; }
            set { _indentPerLevel = value; }
        }

        private double _indentPerLevel = 0.0;

        #endregion
    }

    /// <summary>
    /// MarginConverter calculates the margin of IndentedRowPresenter based on the actual indent.
    /// The forward length will be the actual indent of the outer ListTreeViewItem untill it is
    /// greater than the actual width of the first column. In that case its value is the actual width
    /// of the first column.
    /// </summary>
    public class MarginConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values.Length == 2 && values[0] is int && values[1] is double))
                return 0.0;

            int level = (int)values[0];
            double actualWidth = (double)values[1];
            double actualIndent = (level + 1) * IndentPerLevel;
            double left = Math.Min(actualIndent, actualWidth);
            return new Thickness(-left, 0, 0, 0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IndentPerLevel

        /// <summary>
        /// the indent lenght per level
        /// </summary>
        public double IndentPerLevel
        {
            get { return _indentPerLevel; }
            set { _indentPerLevel = value; }
        }

        private double _indentPerLevel = 0.0;

        #endregion
    }
}
