// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Layout;
using System.Collections;
using Microsoft.Test.Logging;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using System.Xml;
using System.IO;
using Microsoft.Test.Serialization;
using Microsoft.Test.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Data;
using Microsoft.Test.Globalization;
using System.Windows.Markup;
using Microsoft.Test.Layout.TestTypes;
using System.Resources;
using Microsoft.Test.Threading;
using System.Collections.ObjectModel;

namespace ElementLayout.TestLibrary
{
    #region Test Panels
    /// Common Code used in Element Layout Cases

    /// <summary>
    /// Dummy Test Panel.
    /// </summary>
    public class TestPanel : Panel
    {
        public TestPanel()
            : base()
        {
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size childConstraint = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(childConstraint);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }
            return arrangeSize;
        }
    }

    /// <summary>
    /// Infinite Test Panel.  Measure's and Arrange's to infinite size.
    /// </summary>
    public class InfinityArrangePanel : Panel
    {
        public bool result = false;
        public InfinityArrangePanel()
            : base()
        { }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    try
                    {
                        child.Arrange(new Rect(0, 0, double.PositiveInfinity, double.PositiveInfinity));
                    }
                    catch (Exception e)
                    {
                        // Test Exception
                        // UIElement_Layout_InfinityArrange=Cannot call Arrange on a UIElement with infinite size or NaN. Parent of type '{0}' invokes the UIElement. Arrange called on element of type '{1}'.
                        this.result = Exceptions.CompareMessage(e.Message, "UIElement_Layout_InfinityArrange", WpfBinaries.PresentationCore);
                    }
                }
            }
            return arrangeSize;
        }
    }

    /// <summary>
    /// NaN Test Panel.  Arranges to NaN size.
    /// </summary>
    public class NaNArrangePanel : Panel
    {
        public bool result = false;
        public NaNArrangePanel()
            : base()
        { }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in Children)
            {
                if (child != null)
                {
                    try
                    {
                        child.Arrange(new Rect(0, 0, double.NaN, double.NaN));
                    }
                    catch (Exception e)
                    {
                        // Test Exception
                        // UIElement_Layout_InfinityArrange=Cannot call Arrange on a UIElement with infinite size or NaN. Parent of type '{0}' invokes the UIElement. Arrange called on element of type '{1}'.
                        this.result = Exceptions.CompareMessage(e.Message, "UIElement_Layout_InfinityArrange", WpfBinaries.PresentationCore);
                    }
                }
            }
            return arrangeSize;
        }
    }

    /// <summary>
    /// NaN Test Panel.  Meaures to NaN size.
    /// </summary>
    public class NaNMeasurePanel : Panel
    {
        public bool result = false;
        public NaNMeasurePanel()
            : base()
        { }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement child in Children)
            {
                Size childConstraint = new Size(double.NaN, double.NaN);
                try
                {
                    child.Measure(childConstraint);
                }
                catch(Exception e)
                {
                    // Exception being tested.
                    // UIElement_Layout_NaNMeasure=UIElement.Measure(availableSize) cannot be called with NaN size.
                    this.result = Exceptions.CompareMessage(e.Message, "UIElement_Layout_NaNMeasure", WpfBinaries.PresentationCore);
                }
            }
            return (new Size(500, 500));
        }
    }

    /// <summary>
    /// NaN Test Panel.  Meaures returns NaN size.
    /// </summary>
    public class MeasureReturnsNaNPanel : Panel
    {
        public MeasureReturnsNaNPanel()
            : base()
        { }

        protected override Size MeasureOverride(Size constraint)
        {
            return (new Size(double.NaN, double.NaN));
        }
    }

    /// <summary>
    /// Create instance of Virtualizing Stack Panel
    /// </summary>
    public class VSP : ItemsControl
    {
        public VSP() :
            this(double.NaN, double.NaN, 0, Orientation.Vertical) { }

        public VSP(int children) :
            this(double.NaN, double.NaN, children, Orientation.Vertical) { }

        public VSP(Orientation orientation) :
            this(double.NaN, double.NaN, 0, orientation) { }

        public VSP(int children, Orientation orientation) :
            this(double.NaN, double.NaN, children, orientation) { }

        public VSP(double height, double width) :
            this(height, width, 0, Orientation.Vertical) { }

        public VSP(double height, double width, int children) :
            this(height, width, children, Orientation.Vertical) { }

        public VSP(double height, double width, int children, Orientation orientation)
        {
            FrameworkElementFactory virtualizingStackPanel = new FrameworkElementFactory(typeof(VirtualizingStackPanel), "VirtualizingStackPanel");
            virtualizingStackPanel.SetValue(VirtualizingStackPanel.BackgroundProperty, Brushes.CornflowerBlue);
            virtualizingStackPanel.SetValue(VirtualizingStackPanel.IsItemsHostProperty, true);
            virtualizingStackPanel.SetValue(VirtualizingStackPanel.OrientationProperty, orientation);

            Style style = new Style(typeof(ItemsControl));

            ControlTemplate template = new ControlTemplate(typeof(ItemsControl));
            template.VisualTree = virtualizingStackPanel;

            style.Setters.Add(new Setter(ItemsControl.TemplateProperty, template));

            Style = style;
            Height = height;
            Width = width;

            Binding binding = new Binding();
            binding.Source = new ChildrenCollection(children);
            SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }
    }

    /// <summary>
    /// Children Collection used by VirtualizingStackPanel
    /// </summary>
    public class ChildrenCollection : ObservableCollection<UIElement>
    {
        public ChildrenCollection(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Border border = new Border();
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.White;
                border.Height = 46;
                border.Width = 46;
                border.Margin = new Thickness(2);

                TextBlock text = new TextBlock();
                text.Text = i.ToString();
                text.Foreground = Brushes.White;
                text.FontSize = 25;
                text.FontFamily = new FontFamily("Arial");
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;

                border.Child = text;

                Add(border);
            }
        }
    }

    #endregion

    #region Test Helpers

    /// <summary>
    /// Struct for collection of elements used in tests.
    /// </summary>
    public static class Helpers
    {
        #region Layout Test Logger

        /// <summary>
        /// Layout Test Logger.
        /// </summary>
        /// <param name="s"></param>
        public static void Log(string s)
        {
            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus(s);
            }
            else
            {
                GlobalLog.LogStatus(s);
            }
        }

        public static void Log(double d)
        {
            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus(d.ToString());
            }
            else
            {
                GlobalLog.LogStatus(d.ToString());
            }
        }

        public static void Log(int i)
        {
            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus(i.ToString());
            }
            else
            {
                GlobalLog.LogStatus(i.ToString());
            }
        }

        #endregion

        /// <summary>
        /// Collection of elements.
        /// </summary>
        public static string[] ElementList = {
            "System.Windows.Controls.Canvas",
            "System.Windows.Controls.Control",
            "System.Windows.Controls.ContentControl",
            "System.Windows.Controls.ItemsControl",
            "System.Windows.Controls.HeaderedItemsControl",
            "System.Windows.Controls.MenuItem",
            "System.Windows.Controls.FlowDocumentScrollViewer",
            "System.Windows.Controls.FlowDocumentPageViewer",
            "System.Windows.Controls.AccessText",
            "System.Windows.Controls.Border",
            "System.Windows.Controls.Button",
            "System.Windows.Controls.CheckBox",
            "System.Windows.Controls.ComboBox",
            "System.Windows.Controls.ListBoxItem",
            "System.Windows.Controls.ComboBoxItem",
            "System.Windows.Controls.ContentPresenter",
            "System.Windows.Controls.DockPanel",
            "System.Windows.Controls.DocumentViewer",
            "System.Windows.Controls.HeaderedContentControl",
            "System.Windows.Controls.Expander",
            "System.Windows.Controls.FlowDocumentReader",
            "System.Windows.Controls.Frame",
            "System.Windows.Controls.Grid",
            "System.Windows.Controls.GridSplitter",
            "System.Windows.Controls.GroupBox",
            // Disabling this item from tests
            // "System.Windows.Controls.GroupItem",
            "System.Windows.Controls.Image",
            "System.Windows.Controls.InkCanvas",
            "System.Windows.Controls.InkPresenter",
            "System.Windows.Controls.ItemsPresenter",
            "System.Windows.Controls.Label",
            "System.Windows.Controls.ListBox",
            "System.Windows.Controls.ListView",
            "System.Windows.Controls.MediaElement",
            "System.Windows.Controls.Menu",
            "System.Windows.Controls.PasswordBox",
            "System.Windows.Controls.ScrollContentPresenter",
            "System.Windows.Controls.StackPanel",
            "System.Windows.Controls.ProgressBar",
            "System.Windows.Controls.RadioButton",
            "System.Windows.Controls.RichTextBox",
            "System.Windows.Controls.ScrollViewer",
            "System.Windows.Controls.Separator",
            "System.Windows.Controls.Slider",
            "System.Windows.Controls.TabControl",
            "System.Windows.Controls.TabItem",
            "System.Windows.Controls.TextBlock",
            "System.Windows.Controls.TextBox",
            "System.Windows.Controls.ToolBar",
            "System.Windows.Controls.ToolBarTray",
            "System.Windows.Controls.TreeView",
            "System.Windows.Controls.TreeViewItem",
            "System.Windows.Controls.UserControl",
            "System.Windows.Controls.Viewbox",
            "System.Windows.Controls.Viewport3D",
            "System.Windows.Controls.VirtualizingStackPanel",
            "System.Windows.Controls.WrapPanel"
        };

        /// <summary>
        /// Load generic, theme neutral styles for cases that use visual verification
        /// </summary>
        /// <param name="filename">Name of file to load from resources.</param>
        /// <returns>A resource dictonary</returns>
        public static ResourceDictionary LoadStyle(string filename)
        {
            ResourceDictionary generic = null;

            Stream resourceStream = null;
            Assembly targetAssembly = Assembly.GetCallingAssembly();

            ResourceManager resourceManager = new ResourceManager("ElementLayoutLibrary.g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject(filename);

            if (resourceStream != null)
            {
                generic = XamlReader.Load(resourceStream) as ResourceDictionary;
            }

            return generic;
        }

        /// <summary>
        /// Load content file from resource
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static FrameworkElement LoadContent(string filename)
        {
            FrameworkElement content = null;

            Stream resourceStream = null;
            Assembly targetAssembly = Assembly.GetCallingAssembly();

            ResourceManager resourceManager = new ResourceManager("ElementLayoutLibrary.g", targetAssembly);
            resourceManager.IgnoreCase = true;
            resourceStream = (Stream)resourceManager.GetObject(filename);

            if (resourceStream != null)
            {
                content = XamlReader.Load(resourceStream) as FrameworkElement;
            }

            return content;
        }


        /// <summary>
        /// Adjust content size of window to the size the content would be in an aero window.
        /// </summary>
        /// <param name="w">Target Window</param>
        public static void AdjustWindowContentSize(Window w)
        {
            if (w.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.

                Size aeroChromeSize = new Size(16, 36);

                ((FrameworkElement)w.Content).Width = w.ActualWidth - aeroChromeSize.Width;
                ((FrameworkElement)w.Content).Height = w.ActualHeight - aeroChromeSize.Height;
            }
        }

        /// <summary>
        /// Toggle window state, thus giving window focus.
        /// work around for issues where test need window to have
        /// focus, and fails if it does not.
        /// </summary>
        public static void ToggleWindowState(Window w)
        {
            w.WindowState = WindowState.Minimized;
            DispatcherHelper.DoEvents();
            w.WindowState = WindowState.Normal;
            DispatcherHelper.DoEvents();
        }

    }

    /// <summary>
    /// Helper for Clipping Tests.
    /// </summary>
    public class ElementClipHelper
    {
        /// <summary>
        /// Adds test content to a panel.
        /// </summary>
        /// <param name="testitem"></param>
        /// <returns></returns>
        public static FrameworkElement AddTestContent(string testitem)
        {
            FrameworkElement theparent = null;

            switch (testitem)
            {
                case "Border":
                    Border bod = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(5), Brushes.Orange, 100, 100);
                    theparent = bod;
                    break;

                case "Dock":
                    DockPanel dock = CommonFunctionality.CreateDockPanel(100, 100, Brushes.Orange);
                    theparent = dock;
                    break;

                case "Canvas":
                    Canvas canvas = CommonFunctionality.CreateCanvas(100, 100, Brushes.LightGreen);
                    theparent = canvas;
                    break;

                case "Grid":
                    GridLength[] gridlength = { new GridLength(100) };
                    Grid grid = GridCommon.CreateGrid(1, 1, gridlength, gridlength);
                    grid.Background = Brushes.Gray;
                    grid.Children.Add(staticContent());
                    theparent = grid;
                    break;

                case "Text":
                    TextBlock text = CommonFunctionality.CreateText("This is some text we are using to test clip to bounds. I will set a background, width and height.");
                    text.Width = 100;
                    text.Height = 100;
                    theparent = text;
                    break;

                case "FlowDoc":
                    FlowDocumentScrollViewer textp = CommonFunctionality.CreateFlowDoc(100, 100, Brushes.Gray);

                    textp.Document.ContentEnd.InsertTextInRun("The BMW 3 - Series claimed the top spot once again on our list of vehicles most popular with shoppers on MSN Autos last month. The cyber-shoppers' top ten is based on monthly visits to the MSN Autos vehicle research pages. This latest list, which includes vehicles ranging from basic sedans to exotic sports cars, shows the varied interests of visitors to MSN Autos. In addition to its 3-Series in the top spot, BMW also placed its MINI Cooper, a modern version of the British cult-classic from the 1960s, in the seventh position. The only domestic branded model found on this top-ten list is the ...");
                    theparent = textp;
                    break;

                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Height = 600;
                    panel.Width = 600;
                    panel.Background = Brushes.LightSteelBlue;
                    theparent = panel;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Height = 600;
                    stack.Width = 600;
                    stack.Background = Brushes.LightSalmon;
                    stack.Orientation = Orientation.Horizontal;

                    Rectangle s1 = CommonFunctionality.CreateRectangle(200, 300, Brushes.LightGray);
                    stack.Children.Add(s1);

                    Rectangle s2 = CommonFunctionality.CreateRectangle(200, 100, Brushes.LightGreen);
                    stack.Children.Add(s2);

                    Rectangle s3 = CommonFunctionality.CreateRectangle(200, 400, Brushes.LavenderBlush);
                    stack.Children.Add(s3);

                    theparent = stack;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 600;
                    decorator.Width = 600;

                    Border b = new Border();
                    b.Margin = new Thickness(25);
                    b.Background = Brushes.Crimson;

                    decorator.Child = b;

                    theparent = decorator;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 600;
                    viewbox.Width = 600;
                    viewbox.Stretch = Stretch.Uniform;

                    TextBlock tb = new TextBlock();
                    tb.Text = "viewbox";

                    viewbox.Child = tb;

                    theparent = viewbox;
                    break;

                case "Transform":
                    Border transformBorder = new Border();
                    transformBorder.Background = Brushes.Blue;

                    RotateTransform rt = new RotateTransform(30, 300, 300);

                    Decorator td = new Decorator();
                    td.Height = 600;
                    td.Width = 600;
                    td.Child = transformBorder;
                    td.LayoutTransform = rt;

                    theparent = td;
                    break;

                case "WrapPanel":
                    WrapPanel wrap = new WrapPanel();
                    wrap.Height = 600;
                    wrap.Width = 600;
                    wrap.ItemHeight = 200;
                    wrap.ItemWidth = 200;
                    for (int i = 0; i < 9; i++)
                    {
                        Border wc = new Border();
                        wc.Background = Brushes.LightGray;
                        TextBlock txt = new TextBlock();
                        txt.FontSize = 100;
                        txt.Foreground = Brushes.White;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                        txt.VerticalAlignment = VerticalAlignment.Center;
                        txt.Text = i.ToString();
                        wc.Child = txt;
                        wrap.Children.Add(wc);
                    }

                    theparent = wrap;
                    break;
            }
            return theparent;
        }

        /// <summary>
        /// Static content for tests.
        /// </summary>
        /// <returns></returns>
        public static FrameworkElement staticContent()
        {
            FrameworkElement TheChild = null;
            Image image1 = CommonFunctionality.CreateImage("note.bmp");

            image1.Width = 250;
            image1.Height = 250;
            TheChild = image1;
            return TheChild;
        }
    }

    /// <summary>
    /// Canvas Common Code.
    /// </summary>
    public class CanvasCommon
    {
        /// <summary>Return the Canvas Size</summary>
        /// <param name="canvas"></param>
        /// <returns></returns>
        public static Point GetCanvasSize(Canvas canvas)
        {
            return LayoutUtility.GetElementSize((FrameworkElement)canvas);
        }

        /// <summary>Return the position of Canvas child</summary>
        /// <param name="canvas"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Point GetChildPosition(Canvas canvas, UIElement child)
        {
            return LayoutUtility.GetElementPosition(child, canvas);
        }

        /// <summary>
        /// Gets expected position of element
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Point GetExpectedPosition(Canvas canvas, UIElement child)
        {
            Point canvasSize = LayoutUtility.GetElementSize((FrameworkElement)canvas);
            Point childSize = LayoutUtility.GetElementSize((FrameworkElement)child);
            FrameworkElement fwChild = (FrameworkElement)child;

            Point expectedPosition = new Point();
            //get position X
            if (double.IsNaN(Canvas.GetLeft(child)))
            {
                if (double.IsNaN(Canvas.GetRight(child)))
                    expectedPosition.X = fwChild.Margin.Left;
                else
                    expectedPosition.X = canvasSize.X - fwChild.ActualWidth - fwChild.Margin.Right - Canvas.GetRight(child);
            }
            else
                expectedPosition.X = Canvas.GetLeft(child) + fwChild.Margin.Left;

            //get position Y
            if (double.IsNaN(Canvas.GetTop(child)))
            {
                if (double.IsNaN(Canvas.GetBottom(child)))
                    expectedPosition.Y = fwChild.Margin.Top;
                else
                    expectedPosition.Y = canvasSize.Y - fwChild.ActualHeight - fwChild.Margin.Bottom - Canvas.GetBottom(child);
            }
            else
                expectedPosition.Y = Canvas.GetTop(child) + fwChild.Margin.Top;

            return expectedPosition;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FrameworkElement CanvasContent()
        {
            Grid g;
            Canvas c;
            Rectangle r;

            g = new Grid();
            g.Background = Brushes.Yellow;
            c = CommonFunctionality.CreateCanvas(double.NaN, double.NaN, new SolidColorBrush(Colors.Blue));
            r = CommonFunctionality.CreateRectangle(200, 150, new SolidColorBrush(Colors.Red));
            c.Children.Add(r);
            g.Children.Add(c);
            return g;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="log"></param>
        /// <param name="relayoutOccurred"></param>
        /// <param name="expVal"></param>
        /// <returns></returns>
        public static bool CanvasRelayoutOnPropChangeTestVerifier(Window w, TestLog log, bool relayoutOccurred, Point expVal)
        {
            string[] resultSet = new string[] { null, null };
            Grid root = w.Content as Grid;
            Point actVal = LayoutUtility.GetElementPosition(LayoutUtility.GetChildFromVisualTree(root, typeof(Rectangle)) as UIElement, LayoutUtility.GetChildFromVisualTree(root, typeof(Canvas)) as UIElement);

            log.LogStatus("Expected:X=" + expVal.X.ToString() + ", Y=" + expVal.Y.ToString());
            log.LogStatus("  Acutal:X=" + actVal.X.ToString() + ", Y=" + actVal.Y.ToString());

            //canvas(Canvas) = 400x600
            //rect(Rectangle) = 150x200
            if (relayoutOccurred)
            {
                return (expVal == actVal);
            }
            else
            {
                log.LogStatus("LayoutUpdated event is not fired.");
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static FrameworkElement CanvasBasicContent()
        {
            Grid g;
            g = GridCommon.CreateGrid(1, 1);
            g.Width = 150;
            g.Height = 150;
            g.Background = Brushes.Gray;

            Canvas c;
            c = new Canvas();
            c.Background = Brushes.Yellow;
            g.Children.Add(c);
            return g;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool CanvasSizeVerifier(Window w, TestLog log)
        {
            Grid root = w.Content as Grid;
            Canvas c = LayoutUtility.GetChildFromVisualTree(root, typeof(Canvas)) as Canvas;
            LayoutUtility.ActualLayoutAlignments actualAlignment = new LayoutUtility.ActualLayoutAlignments(root, c);
            Point actualSize = LayoutUtility.GetElementSize((FrameworkElement)c);
            Point expectedSize = new Point();

            if (actualAlignment.ActualHorizontalAlignment == HorizontalAlignment.Stretch)
                expectedSize.X = root.ActualWidth;
            else if ((actualAlignment.ActualHorizontalAlignment != HorizontalAlignment.Stretch) && Double.IsNaN(c.Width))
                expectedSize.X = 0;
            else
                expectedSize.X = c.Width;

            if (actualAlignment.ActualVerticalAlignment == VerticalAlignment.Stretch)
                expectedSize.Y = root.ActualHeight;
            else if ((actualAlignment.ActualVerticalAlignment != VerticalAlignment.Stretch) && Double.IsNaN(c.Height))
                expectedSize.Y = 0;
            else
                expectedSize.Y = c.Height;

            log.LogStatus("Expected: " + expectedSize.X + "x" + expectedSize.Y);
            log.LogStatus("  Actual: " + actualSize.X + "x" + actualSize.Y);

            return (actualSize == expectedSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool CanvasChildPositionVerifier(Window w, TestLog log)
        {
            Grid root = w.Content as Grid;
            Canvas c = LayoutUtility.GetChildFromVisualTree(root, typeof(Canvas)) as Canvas;
            Rectangle rect = LayoutUtility.GetChildFromVisualTree(c, typeof(Rectangle)) as Rectangle;

            Point actualPosition = GetChildPosition(c, rect);
            Point expectedPosition = GetExpectedPosition(c, rect);

            log.LogStatus("Expected: " + expectedPosition.X + "," + expectedPosition.Y);
            log.LogStatus("  Actual: " + actualPosition.X + "," + actualPosition.Y);

            return (actualPosition == expectedPosition);
        }

    }

    /// <summary>
    /// Grid Common Code.
    /// </summary>
    public class GridCommon
    {
        #region CreateGrid
        /// <summary>Creating Grid with default number of Column and Row = 1.</summary>
        /// <returns></returns>
        public static Grid CreateGrid()
        {
            Grid grid = CreateGrid(1, 1);
            return grid;
        }

        ///<summary>Creating Grid with specified number of Column and Row.</summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static Grid CreateGrid(int column, int row)
        {
            Grid grid = Init();
            ColumnDefinition[] cd = new ColumnDefinition[column];
            RowDefinition[] rd = new RowDefinition[row];

            for (int c = 0; c < column; c++)
            {
                cd[c] = new ColumnDefinition();
                grid.ColumnDefinitions.Add(cd[c]);
            }

            for (int r = 0; r < row; r++)
            {
                rd[r] = new RowDefinition();
                grid.RowDefinitions.Add(rd[r]);
            }

            return grid;
        }

        ///<summary>Creating Grid</summary>
        ///<remarks>Creating Grid with specified number of Column and Row specified GridLenth for Column Width and Row Height</remarks>
        ///<param name="column"></param>
        ///<param name="row"></param>
        ///<param name="columnWidth"></param>
        ///<param name="rowHeight"></param>
        ///<returns></returns>
        public static Grid CreateGrid(int column, int row, GridLength[] columnWidth, GridLength[] rowHeight)
        {
            Grid grid = CreateGrid(column, row);
            SettingColumnWidth(grid, columnWidth);
            SettingRowHeight(grid, rowHeight);
            grid.ShowGridLines = true;
            return grid;
        }

        //-----------------------------------------------------------
        //	Init()
        //-----------------------------------------------------------
        private static Grid Init()
        {
            Grid g = new Grid();
            return g;
        }
        #endregion

        #region SettingSize on Column/Row
        /// <summary>Setting Column Width</summary>
        /// <param name="g"></param>
        /// <param name="width"></param>
        public static void SettingColumnWidth(Grid g, GridLength[] width)
        {
            for (int i = 0; i < width.Length; i++)
            {
                g.ColumnDefinitions[i].Width = width[i];
            }
        }

        /// <summary>Setting Row Height</summary>
        /// <param name="g"></param>
        /// <param name="height"></param>
        public static void SettingRowHeight(Grid g, GridLength[] height)
        {
            for (int i = 0; i < height.Length; i++)
            {
                g.RowDefinitions[i].Height = height[i];
            }
        }

        /// <summary>Setting Column MinWidth</summary>
        /// <param name="g"></param>
        /// <param name="minWidth"></param>
        public static void SettingColumnMinWidth(Grid g, double[] minWidth)
        {
            for (int i = 0; i < minWidth.Length; i++)
            {
                g.ColumnDefinitions[i].MinWidth = minWidth[i];
            }
        }

        /// <summary>Setting Row MinHeight</summary>
        /// <param name="g"></param>
        /// <param name="minHeight"></param>
        public static void SettingRowMinHeight(Grid g, double[] minHeight)
        {
            for (int i = 0; i < minHeight.Length; i++)
            {
                g.RowDefinitions[i].MinHeight = minHeight[i];
            }
        }

        /// <summary>Setting Column MaxWidth</summary>
        /// <param name="g"></param>
        /// <param name="maxWidth"></param>
        public static void SettingColumnMaxWidth(Grid g, double[] maxWidth)
        {
            for (int i = 0; i < maxWidth.Length; i++)
            {
                g.ColumnDefinitions[i].MaxWidth = maxWidth[i];
            }
        }

        /// <summary>Setting Row MaxHeight</summary>
        /// <param name="g"></param>
        /// <param name="maxHeight"></param>
        public static void SettingRowMaxHeight(Grid g, double[] maxHeight)
        {
            for (int i = 0; i < maxHeight.Length; i++)
            {
                g.RowDefinitions[i].MaxHeight = maxHeight[i];
            }
        }
        #endregion

        #region Setting Properties Span, Column/Row, Left/Top/Right/Bottom
        /// <summary>Setting Span</summary>
        /// <param name="element"></param>
        /// <param name="columnSpan"></param>
        /// <param name="rowSpan"></param>
        public static void SettingSpan(UIElement element, int columnSpan, int rowSpan)
        {
            if (columnSpan != 0)
                Grid.SetColumnSpan(element, columnSpan);
            if (rowSpan != 0)
                Grid.SetRowSpan(element, rowSpan);
        }

        ///<summary>Placing Child in Grid</summary>
        ///<param name="g"></param>
        ///<param name="element"></param>
        ///<param name="column"></param>
        ///<param name="row"></param>
        public static void PlacingChild(Grid g, UIElement element, int column, int row)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        ///<summary>
        /// Placing Child in Grid 
        ///</summary>
        ///<param name="g"></param>
        ///<param name="element"></param>
        ///<param name="column"></param>
        ///<param name="row"></param>
        ///<param name="addAsChildToGrid"></param>
        public static void PlacingChild(Grid g, UIElement element, int column, int row, bool addAsChildToGrid)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            if (addAsChildToGrid)
                g.Children.Add(element);
        }

        ///<summary>Placing Child in Grid</summary>
        ///<param name="g"></param>
        ///<param name="element"></param>
        ///<param name="column"></param>
        ///<param name="row"></param>
        ///<param name="top"></param>
        ///<param name="left"></param>
        public static void PlacingChild(Grid g, UIElement element, int column, int row, double top, double left)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            SettingOffset(element, left, top);
        }

        ///<summary>Placing Child in Grid</summary>
        ///<param name="g"></param>
        ///<param name="element"></param>
        ///<param name="column"></param>
        ///<param name="row"></param>
        ///<param name="top"></param>
        ///<param name="left"></param>
        ///<param name="right"></param>
        ///<param name="bottom"></param>
        public static void PlacingChild(Grid g, UIElement element, int column, int row, double top, double left, double right, double bottom)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            SettingOffset(element, left, top, right, bottom);
        }


        /// <summary>Setting Offset on Child of Grid.</summary>
        ///<param name="element"></param>
        ///<param name="left"></param>
        ///<param name="top"></param>
        public static void SettingOffset(UIElement element, double left, double top)
        {
            element.SetValue(FrameworkElement.MarginProperty, new Thickness(left, top, 0, 0));
        }

        /// <summary>Setting Offset on Child of Grid.</summary>
        ///<param name="element"></param>
        ///<param name="left"></param>
        ///<param name="top"></param>
        ///<param name="right"></param>
        ///<param name="bottom"></param>
        public static void SettingOffset(UIElement element, double left, double top, double right, double bottom)
        {
            element.SetValue(FrameworkElement.MarginProperty, new Thickness(left, top, right, bottom));
        }
        /// <summary>Setting HorizontalAlignment on Child of Grid.</summary>
        ///<param name="element"></param>
        ///<param name="h"></param>
        public static void SettingAlignment(UIElement element, HorizontalAlignment h)
        {
            element.SetValue(FrameworkElement.HorizontalAlignmentProperty, h);
        }

        /// <summary>Setting VerticalAlignment on Child of Grid.</summary>
        ///<param name="element"></param>
        ///<param name="v"></param>
        public static void SettingAlignment(UIElement element, VerticalAlignment v)
        {
            element.SetValue(FrameworkElement.VerticalAlignmentProperty, v);
        }

        /// <summary>Setting HorizontalAlignment and VerticalAlignment on Child of Grid.</summary>
        ///<param name="element"></param>
        ///<param name="h"></param>
        /// ///<param name="v"></param>
        public static void SettingAlignment(UIElement element, HorizontalAlignment h, VerticalAlignment v)
        {
            element.SetValue(FrameworkElement.HorizontalAlignmentProperty, h);
            element.SetValue(FrameworkElement.VerticalAlignmentProperty, v);
        }
        #endregion

        #region AddingRectangle
        ///<summary>adding rectangles with specified size of rectangle and background.</summary>
        ///<remarks>adding rectangles with specified size of rectangle and background. if column = true, rectangle will be added to all columns  if row = true, rectangle will be added to all rows </remarks>
        ///<param name="g"></param>
        ///<param name="size"></param>
        ///<param name="background"></param>
        ///<param name="column"></param>
        ///<param name="row"></param>
        public static void AddingRectangle(Grid g, double[] size, SolidColorBrush[] background, Boolean column, Boolean row)
        {
            int numRect = 1;
            int cNum = 1;
            int rNum = 1;

            if (column)
                if (row)
                {
                    cNum = g.ColumnDefinitions.Count;
                    rNum = g.RowDefinitions.Count;
                    numRect = cNum * rNum;
                }
                else
                {
                    cNum = g.ColumnDefinitions.Count;
                    numRect = cNum;
                }
            else if (row)
            {
                rNum = g.RowDefinitions.Count;
                numRect = rNum;
            }

            Rectangle[] rect = new Rectangle[numRect];
            for (int r = 0; r < rNum; r++)
            {
                for (int c = 0; c < cNum; c++)
                {
                    rect[c] = CommonFunctionality.CreateRectangle(size[c + (cNum * r)], size[c + (cNum * r)], background[c + (cNum * r)]);
                    PlacingChild(g, rect[c], c, r, true);
                }
            }
        }
        /// <summary>adding rectangles with specified size in all columns and rows. </summary>
        ///<param name="g"></param>
        ///<param name="size"></param>
        public static void AddingCommonRectangle(Grid g, double size)
        {
            byte color = 0;
            for (int c = 0; c < g.ColumnDefinitions.Count; c++)
            {
                for (int r = 0; r < g.RowDefinitions.Count; r++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(size, size, new SolidColorBrush(Color.FromRgb(color, color, 0)));
                    PlacingChild(g, rect, c, r, true);
                    color += 15;
                }
            }
        }
        #endregion

        #region Verification

        /// <summary>This returns array of computed width for each column.</summary>
        /// <param g="g">g as Grid</param>
        /// <returns>Array of double for column width </returns>
        public static double[] GettingActualColumnWidth(Grid g)
        {
            int columnNum = g.ColumnDefinitions.Count;
            double[] columnWidth = new double[columnNum];
            for (int i = 0; i < columnNum; i++)
            {
                columnWidth[i] = g.ColumnDefinitions[i].ActualWidth;
            }
            return columnWidth;
        }

        /// <summary>This returns array of computed height for each row.</summary>
        /// <param g="g">g as Grid</param>
        /// <returns>Array of double for row height</returns>
        public static double[] GettingActualRowHeight(Grid g)
        {
            int rowNum = g.RowDefinitions.Count;
            double[] rowHeight = new double[rowNum];
            for (int i = 0; i < rowNum; i++)
            {
                rowHeight[i] = g.RowDefinitions[i].ActualHeight;
            }
            return rowHeight;
        }

        /// <remarks>This sets the expected position of each child in Grid by using GetLeft, GetRight, etc. If Left is set to Absolute value, then it will be respected always.If left is set to auto and right is set to Absolute value, then Right will be respected. If both Left and Right is set to Auto, then the element sould be placed in the middle. Same logic applied for Top and Bottom. </remarks>
        /// <param g="g">g as Grid</param>
        /// <returns>Array of point for each child's position.</returns>
        public static Point[] SettingExpectedPosition(Grid g)
        {
            int num = g.Children.Count; // number of children
            double x, y;
            Point[] expPosition = new Point[num];
            for (int i = 0; i < num; i++)
            {
                FrameworkElement fe = g.Children[i] as FrameworkElement;
                //Setting expected X values.
                if (fe.HorizontalAlignment == HorizontalAlignment.Left)
                    x = fe.Margin.Left;
                else if (fe.HorizontalAlignment == HorizontalAlignment.Right)
                    x = g.ColumnDefinitions[i].ActualWidth - fe.ActualWidth - fe.Margin.Right;
                else
                    x = (g.ColumnDefinitions[i].ActualWidth - fe.ActualWidth - fe.Margin.Left - fe.Margin.Right) / 2;

                expPosition[i].X = TotalColumnWidth(g, Grid.GetColumn(g.Children[i])) + x;

                //Setting expected Y values.
                if (fe.VerticalAlignment == VerticalAlignment.Top)
                    y = fe.Margin.Top;
                else if (fe.VerticalAlignment == VerticalAlignment.Bottom)
                    y = g.RowDefinitions[i].ActualHeight - fe.ActualHeight - fe.Margin.Bottom;
                else
                    y = (g.RowDefinitions[i].ActualHeight - fe.ActualHeight - fe.Margin.Top - fe.Margin.Bottom) / 2;

                expPosition[i].Y = TotalRowHeight(g, Grid.GetRow(g.Children[i])) + y;
            }
            return expPosition;
        }

        /// <summary>This gets the actual position of each child in Grid by using LayoutUtility.GetElementPosition().</summary>
        /// <param g="g">g as Grid</param>
        /// <returns>Array of point for each child's position.</returns>
        public static Point[] GettingActualPosition(Grid g)
        {
            int num = g.Children.Count; // number of children
            Point[] actPosition = new Point[num];
            for (int i = 0; i < num; i++)
            {
                actPosition[i] = LayoutUtility.GetElementPosition(g.Children[i], g);
            }

            return actPosition;

        }

        /// <summary></summary>
        /// <param g="g">g as Grid</param>
        /// <returns>total width of all columns</returns>
        public static double TotalColumnWidth(Grid g)
        {
            return TotalColumnWidth(g, g.ColumnDefinitions.Count);
        }

        /// <summary>This returs total width of columns from first to specified column.</summary>
        /// <param g="g">g as Grid</param>
        /// <param col="col">col as integer</param>
        /// <returns>total width as a double</returns>
        public static double TotalColumnWidth(Grid g, int col)
        {
            double columnWidth = 0;
            for (int c = 0; c < col; c++)
            {
                columnWidth += g.ColumnDefinitions[c].ActualWidth;
            }
            return columnWidth;

        }
        /// <summary></summary>
        /// <param g="g">g as Grid</param>
        /// <param col="col">row as integer</param>
        /// <returns>total height of all rows</returns>
        public static double TotalRowHeight(Grid g)
        {
            return TotalRowHeight(g, g.RowDefinitions.Count);
        }

        /// <summary>This returs total height of rows from first to specified row.</summary>
        /// <param g="g">g as Grid</param>
        /// <param col="col">row as integer</param>
        /// <returns>total height as a double</returns>
        public static double TotalRowHeight(Grid g, int row)
        {
            double rowHeight = 0;
            for (int c = 0; c < row; c++)
            {
                rowHeight += g.RowDefinitions[c].ActualHeight;
            }
            return rowHeight;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public class GridMinMaxSizeVerification
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="log"></param>
            /// <returns></returns>
            public static bool GridMinMaxSizeVerifier(Grid g, TestLog log)
            {
                double defSpecifiedSize, maxSize, minSize, contentSize;
                double actualValue = 0;
                double expectedValue = 0;

                // Get Content size
                Thickness childMargin = (Thickness)g.Children[0].GetValue(FrameworkElement.MarginProperty);
                contentSize = g.Children[0].RenderSize.Width + childMargin.Left + childMargin.Right;

                int col = Grid.GetColumn(g.Children[0]);
                //Get Column Specified Size
                GridLength gl = (GridLength)g.ColumnDefinitions[col].GetValue(ColumnDefinition.WidthProperty);

                defSpecifiedSize = gl.IsAbsolute ? gl.Value : gl.IsStar ? g.ActualWidth / g.ColumnDefinitions.Count : contentSize;

                //Get Column Max & Min Size
                maxSize = (double)g.ColumnDefinitions[col].GetValue(ColumnDefinition.MaxWidthProperty);
                minSize = (double)g.ColumnDefinitions[col].GetValue(ColumnDefinition.MinWidthProperty);

                if (minSize != 0) expectedValue = defSpecifiedSize < minSize ? minSize : defSpecifiedSize;
                if (maxSize != 0) expectedValue = defSpecifiedSize > maxSize ? maxSize : defSpecifiedSize;
                //if (expectedValue < contentSize) expectedValue = contentSize;

                actualValue = g.ColumnDefinitions[col].ActualWidth;

                // Get Result and result message if result is not true
                PrintMsg(log, actualValue, expectedValue);
                return Equal(actualValue, expectedValue);
            }

            static private bool Equal(double actual, double expected)
            {
                return actual == expected;
            }
            static private void PrintMsg(TestLog log, double actual, double expected)
            {
                log.LogStatus("** Expected Value:\t" + expected.ToString());
                log.LogStatus("** Actual Value:\t" + actual.ToString());
            }
        }

        /// <summary>
        /// </summary>
        public class GridColumnRowSpanTestCommon
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="numSpan"></param>
            /// <param name="cell"></param>
            /// <returns></returns>
            public static FrameworkElement CommonContent(int numSpan, int cell)
            {
                GridLength[] gridLength = new GridLength[] {
				new GridLength(1, GridUnitType.Star),
				new GridLength(100, GridUnitType.Pixel),
				new GridLength(1, GridUnitType.Auto),
				new GridLength(3, GridUnitType.Star),
				new GridLength(2, GridUnitType.Star),
				new GridLength(1, GridUnitType.Auto)};
                Grid grid = GridCommon.CreateGrid(6, 6, gridLength, gridLength);
                grid.ShowGridLines = true;

                //Children (Rectangle) with various UnitType for Width and Height
                Rectangle rect = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Orange));

                //Setting ColumnSpan & RowSpan
                GridCommon.SettingSpan(rect, numSpan, numSpan);
                //Placing Child
                GridCommon.PlacingChild(grid, rect, cell, cell, true);

                //adding child to occupy the space for Auto
                Rectangle srect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Green));
                GridCommon.PlacingChild(grid, srect, 2, 2, true);
                //adding child to occupy the space for Auto
                Rectangle srect2 = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Green));
                GridCommon.PlacingChild(grid, srect2, 5, 5, true);
                return grid;

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="_numSpan"></param>
            /// <param name="_cell"></param>
            /// <param name="log"></param>
            /// <returns></returns>
            public static bool GridColumnRowSpanTestVerifier(Grid g, int _numSpan, int _cell, TestLog log)
            {
                log.LogStatus("Setting Column/RowSpan to " + _numSpan.ToString() + " on (" + _cell.ToString() + "," + _cell.ToString() + ")");
                // Expected Value = sum of spanned Width/Height
                // Actual Value = computed Width/Height of Rectangle.
                double[] actualColumnWidth = GridCommon.GettingActualColumnWidth(g);
                double[] actualRowHeight = GridCommon.GettingActualRowHeight(g);

                double expectedWidth = 0;
                double expectedHeight = 0;
                //Get Expected value
                for (int i = _cell; i < _cell + _numSpan; i++)
                {
                    expectedWidth += actualColumnWidth[i];
                    expectedHeight += actualRowHeight[i];
                }
                double actualWidth = ((Rectangle)g.Children[0]).ActualWidth;
                double actualHeight = ((Rectangle)g.Children[0]).ActualHeight;

                log.LogStatus("\n\tExpected Width:\t" + expectedWidth + "\n\tActual Width:\t" + actualWidth + "\n\tExpectedHeight:\t" + expectedHeight + "\n\tActual Height:\t" + actualHeight);
                return (Equal(expectedWidth, actualWidth) && Equal(expectedHeight, actualHeight));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="exp"></param>
            /// <param name="act"></param>
            /// <returns></returns>
            public static bool Equal(double exp, double act)
            {
                return (Math.Abs(exp - act) < 0.01);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridRTLSupportTest
        {
            /// <summary>
            /// Basic Grid with Columns & Rows. 
            /// </summary>
            /// <param name="g"></param>
            /// <returns></returns>
            static public Grid ColumnRowTest(Grid g)
            {
                GridLength[] gl = { new GridLength(100), new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
                g = GridCommon.CreateGrid(4, 4, gl, gl);

                byte color = 0;

                for (int i = 0; i < 4; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Color.FromRgb(color, color, 0)));
                    GridCommon.PlacingChild(g, rect, i, i);
                    g.Children.Add(rect);
                    color += 100;
                }
                g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <returns></returns>
            static public Grid LeftSpacingTest(Grid g)
            {
                GridLength[] gl = {new GridLength(100), new GridLength(2, GridUnitType.Star),
				new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star)};
                g = GridCommon.CreateGrid(4, 4, gl, gl);

                byte color = 0;

                for (int i = 0; i < 4; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Color.FromRgb(color, color, 0)));
                    GridCommon.PlacingChild(g, rect, i, i);
                    g.Children.Add(rect);
                    rect.Margin = new Thickness((10 * (i + 1)), 0, 0, 0);
                    GridCommon.SettingAlignment(rect, HorizontalAlignment.Left, VerticalAlignment.Top);
                    color += 100;
                }
                g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <returns></returns>
            static public Grid RightSpacingTest(Grid g)
            {
                GridLength[] gl = { new GridLength(100), new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
                g = GridCommon.CreateGrid(4, 4, gl, gl);
                byte color = 0;
                for (int i = 0; i < 4; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Color.FromRgb(color, color, 0)));
                    GridCommon.PlacingChild(g, rect, i, i);
                    g.Children.Add(rect);
                    rect.Margin = new Thickness(0, 0, (10 * (i + 1)), 0);
                    GridCommon.SettingAlignment(rect, HorizontalAlignment.Right, VerticalAlignment.Top);
                    color += 100;
                }
                g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <returns></returns>
            static public Grid SpanningTest(Grid g)
            {
                GridLength[] gl = { new GridLength(50), new GridLength(0.5, GridUnitType.Star), 
				new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star),
			new GridLength(50), new GridLength(3, GridUnitType.Star), 
			new GridLength(50), new GridLength(2, GridUnitType.Star), 
			new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star)};
                g = GridCommon.CreateGrid(10, 10, gl, gl);
                byte color = 0;
                for (int i = 0; i < 10; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(double.NaN, 50, new SolidColorBrush(Color.FromRgb(color, color, 0)));

                    GridCommon.PlacingChild(g, rect, i, i);
                    g.Children.Add(rect);
                    Grid.SetColumnSpan(rect, 10 - i);
                    color += 70;
                }
                g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithTextPanel(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument();
                tp.Background = Brushes.Bisque;
                tp.Document.ContentStart.InsertTextInRun("People don't live to be 110 because they don't age; it's a fortuitous, genetic roll of the dice, so there is not an intervention available to people who are still alive, says L. Stephen Coles, MD, PhD, a gerontologist with the Los Angeles Gerontology Research Group at UCLA. No one is likely to beat Jeanne Calment's record in our lifetime because she was so exceptional.");
                g.Children.Add(tp);
                GridCommon.PlacingChild(g, tp, 1, 1, 20, 20);
                if (rtlOnChild) tp.FlowDirection = FlowDirection.RightToLeft;
                if (rtlOnGrid) g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithFlowPanel(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithCanvas(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                Canvas c = new Canvas();
                Rectangle rect1 = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Salmon));
                Rectangle rect2 = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.SandyBrown));
                c.Children.Add(rect1);
                c.Children.Add(rect2);
                Canvas.SetLeft(rect1, 50);
                Canvas.SetTop(rect2, 100);
                Canvas.SetRight(rect2, 50);
                g.Children.Add(c);
                if (rtlOnChild) c.FlowDirection = FlowDirection.RightToLeft;
                if (rtlOnGrid) g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithDockPanel(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                DockPanel dp = new DockPanel();
                dp.LastChildFill = true;
                Rectangle[] rect = new Rectangle[5];
                byte color = 0;
                for (int i = 0; i < 5; i++)
                {
                    rect[i] = new Rectangle();
                    rect[i].Fill = new SolidColorBrush(Color.FromRgb(color, color, 0));
                    color += 100;
                    dp.Children.Add(rect[i]);
                }
                rect[0].Width = 50;
                rect[1].Height = 50;
                rect[2].Width = 50;
                rect[3].Height = 50;

                DockPanel.SetDock(rect[0], Dock.Left);
                DockPanel.SetDock(rect[1], Dock.Top);
                DockPanel.SetDock(rect[2], Dock.Right);
                DockPanel.SetDock(rect[3], Dock.Bottom);
                //DockPanel.SetDock(rect[4], Dock.Fill);

                g.Children.Add(dp);
                if (rtlOnChild) dp.FlowDirection = FlowDirection.RightToLeft;
                if (rtlOnGrid) g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithButton(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                Button btn = new Button();
                btn.Content = "This is the Button inside of Grid";
                g.Children.Add(btn);
                GridCommon.PlacingChild(g, btn, 0, 0, 20, 20);
                if (rtlOnChild) btn.FlowDirection = FlowDirection.RightToLeft;
                if (rtlOnGrid) g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            /// <param name="rtlOnGrid"></param>
            /// <param name="rtlOnChild"></param>
            /// <returns></returns>
            static public Grid TestWithTextBox(Grid g, bool rtlOnGrid, bool rtlOnChild)
            {
                g = GridCommon.CreateGrid(2, 2);
                TextBox tb = new TextBox();
                tb.Text = "Text Box";
                g.Children.Add(tb);
                GridCommon.PlacingChild(g, tb, 1, 1, 20, 20);
                if (rtlOnChild) tb.FlowDirection = FlowDirection.RightToLeft;
                if (rtlOnGrid) g.FlowDirection = FlowDirection.RightToLeft;
                return g;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool GridRowDefinitionExceptionThrownTestVerifier(Type e, TestLog log)
        {
            bool result = (e != null) ? true : false;
            if (result) log.LogStatus(e.ToString() + "occurred.");
            else log.LogStatus("No Exception occurred~");

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        public class GridResizingVariousObjectTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                GridLength[] gridLength = new GridLength[]{
                new GridLength(100),
                new GridLength(150),
                new GridLength(1,GridUnitType.Auto),
                new GridLength(1,GridUnitType.Star)
            };

                Grid grid = GridCommon.CreateGrid(4, 4, gridLength, gridLength);

                // Resetting static members for when tests run in a common app domain.
                resizeCount = 1;
                verified = 0;
                result = true;
                return grid;
            }

            static int resizeCount = 1;
            static int verified = 0;
            public static bool result = true;
            public static void ResizeWindow(Window _w, TestLog log)
            {
                int size;
                if (resizeCount < 3)
                    size = 50;
                else
                    size = -50;
                _w.Height = _w.Height + size;
                _w.Width = _w.Width + size;
                log.LogStatus("-" + resizeCount.ToString() + "- Window resized to " + _w.RenderSize.Width.ToString() + "x" + _w.RenderSize.Height.ToString());
                resizeCount++;
                //Verifying after resize the window for each time.
                GridResizingVariousObjectTestVerifier((Grid)_w.Content, log);
            }

            public static void GridResizingVariousObjectTestVerifier(Grid g, TestLog log)
            {
                int num = g.Children.Count;
                Size[] expected = new Size[num];
                Size[] actual = new Size[num];

                //Getting actual values
                log.LogStatus("--Actual Values--");
                for (int a = 0; a < num; a++)
                {
                    actual[a].Width = g.ColumnDefinitions[a].ActualWidth;
                    actual[a].Height = g.RowDefinitions[a].ActualHeight;
                    log.LogStatus("width: " + actual[a].Width + "\theight: " + actual[a].Height);
                }

                //Setting expected values
                log.LogStatus("--Expected Values--");
                for (int e = 0; e < num; e++)
                {
                    if ((g.Children[e].RenderSize.Width - g.ColumnDefinitions[e].ActualWidth) > 0.001)
                        expected[e].Width = g.ActualWidth - GridCommon.TotalColumnWidth(g, e);
                    else
                        expected[e].Width = g.Children[e].RenderSize.Width;

                    if ((g.Children[e].RenderSize.Height - g.RowDefinitions[e].ActualHeight) > 0.001)
                        expected[e].Height = g.ActualHeight - GridCommon.TotalRowHeight(g, e);
                    else
                        expected[e].Height = g.Children[e].RenderSize.Height;

                    log.LogStatus("width: " + expected[e].Width + "\theight: " + expected[e].Height);
                }
                //Compare the values
                if (!ComareValues(expected, actual))
                {
                    result = false;
                    log.LogStatus("======Failed!!!======");
                }
                else
                    log.LogStatus("======Passed!!!======");
                if (verified < 5) verified++;
            }

            public static bool ComareValues(Size[] exp, Size[] act)
            {
                for (int v = 0; v < exp.Length; v++)
                {
                    if (Math.Abs(exp[v].Width - act[v].Width) > 0.01 || Math.Abs(exp[v].Height - act[v].Height) > 0.01)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridResizingWidthHeightTestCommon
        {
            public static void ResizeWindow(Window _w, TestLog log)
            {
                int size;

                if (resizeCount < 3)
                    size = 50;
                else
                    size = -50;

                _w.Height = _w.Height + size;
                _w.Width = _w.Width + size;
                log.LogStatus("-" + resizeCount.ToString() + "- Window resized to " + ((Grid)_w.Content).RenderSize.Width.ToString() + "x" + ((Grid)_w.Content).RenderSize.Height.ToString());
                resizeCount++;
                //Verifying after resize the window for each time.
                GridResizingWidthHeightTestVerifier((Grid)_w.Content, log);
            }
            public static int failCount;
            public static int resizeCount;
            public static double[] expectedValuesC;
            public static double[] expectedValuesR;
            public static void GridResizingWidthHeightTestVerifier(Grid g, TestLog log)
            {
                expectedValuesC = new double[4];
                expectedValuesR = new double[4];

                for (int c = 0; c < 4; c++)
                {
                    GridLength columnWidth = (GridLength)g.ColumnDefinitions[c].GetValue(ColumnDefinition.WidthProperty);
                    if (columnWidth.IsAbsolute) //Absolute
                        expectedValuesC[c] = columnWidth.Value;
                    else if (Double.IsNaN(columnWidth.Value)) //Auto
                        expectedValuesC[c] = g.Children[c].RenderSize.Width;                   
                    else //Star
                        expectedValuesC[c] = g.ColumnDefinitions[c].ActualWidth;
                }

                for (int r = 0; r < 4; r++)
                {
                    GridLength rowHeight = (GridLength)g.RowDefinitions[r].GetValue(RowDefinition.HeightProperty);
                    if (rowHeight.IsAbsolute) //Absolute
                        expectedValuesR[r] = rowHeight.Value;
                    else if (Double.IsNaN(rowHeight.Value)) //Auto
                        expectedValuesR[r] = g.Children[r].RenderSize.Height;                   
                    else //Star
                        expectedValuesR[r] = g.RowDefinitions[r].ActualHeight;
                }

                //Verify Width
                for (int c = 0; c < g.ColumnDefinitions.Count; c++)
                {
                    if (Math.Abs(expectedValuesC[c] - g.ColumnDefinitions[c].ActualWidth) > 0.01)
                    {
                        log.LogStatus("\n\tFailed: Column " + c.ToString() + ": expected = " + expectedValuesC[c] + " (actual = " + g.ColumnDefinitions[c].ActualWidth.ToString() + ")");
                        failCount++;
                    }
                }

                //Verify Height
                for (int r = 0; r < g.ColumnDefinitions.Count; r++)
                {
                    if (Math.Abs(expectedValuesR[r] - g.RowDefinitions[r].ActualHeight) > 0.01)
                    {
                        log.LogStatus("\n\tFailed: Row " + r.ToString() + ": expected = " + expectedValuesR[r] + " (actual = " + g.RowDefinitions[r].ActualHeight.ToString() + ")");
                        failCount++;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridResizingSpanningTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                GridLength[] gridLength = new GridLength[]{
                new GridLength(100),
                new GridLength(80),
                new GridLength(1,GridUnitType.Auto),
                new GridLength(1,GridUnitType.Star)
            };

                Grid grid = GridCommon.CreateGrid(4, 4, gridLength, gridLength);

                // Adding Rectangles in (0,0), (1,1), (2,2), and (3,3)
                Rectangle[] rect = new Rectangle[grid.ColumnDefinitions.Count];
                for (int r = 0; r < grid.ColumnDefinitions.Count; r++)
                {
                    rect[r] = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Orange));
                    GridCommon.PlacingChild(grid, rect[r], r, r, true);
                }

                // Resetting static members for when tests run in a common app domain.
                resizeCount = 1;
                verified = 0;
                result = true;
                return grid;

            }

            static int resizeCount = 1;
            static int verified = 0;
            public static bool result = true;
            public static void ResizeWindow(Window _w, TestLog log)
            {
                int size;

                if (resizeCount < 3)
                    size = 50;
                else
                    size = -50;

                _w.Height = _w.Height + size;
                _w.Width = _w.Width + size;
                log.LogStatus("-" + resizeCount.ToString() + "- Window resized to " + ((Grid)_w.Content).RenderSize.Width.ToString() + "x" + ((Grid)_w.Content).RenderSize.Height.ToString());
                resizeCount++;
                //Verifying after resize the window for each time.
                GridResizingSpanningTestVerifier((Grid)_w.Content, log);
            }

            public static void GridResizingSpanningTestVerifier(Grid g, TestLog log)
            {
                int num = g.ColumnDefinitions.Count;
                Size[] actual = new Size[num];
                Size[] expected = new Size[num];
                //Get Actual Values = Size of Children
                log.LogStatus("====Actual Values====");
                for (int a = 0; a < num; a++)
                {
                    actual[a].Width = g.Children[a].RenderSize.Width;
                    actual[a].Height = g.Children[a].RenderSize.Height;
                    log.LogStatus("width: " + actual[a].Width + "\theight: " + actual[a].Height);
                }
                //Set Expected Values = Sum of ColumnWidth/RowHeight 
                //according to the location of children and ColumnSpan/RowSpan
                log.LogStatus("====Expected Values====");
                for (int e = 0; e < num; e++)
                {
                    for (int c = Grid.GetColumn(g.Children[e]); c < Grid.GetColumn(g.Children[e]) + Grid.GetColumnSpan(g.Children[e]); c++)
                        expected[e].Width += g.ColumnDefinitions[c].ActualWidth;
                    for (int r = Grid.GetRow(g.Children[e]); r < Grid.GetRow(g.Children[e]) + Grid.GetRowSpan(g.Children[e]); r++)
                        expected[e].Height += g.RowDefinitions[r].ActualHeight;
                    log.LogStatus("width: " + expected[e].Width + "\theight: " + expected[e].Height);
                }
                //Compare the values
                if (!ComareValues(expected, actual))
                {
                    result = false;
                    log.LogStatus("======Failed!!!======");
                }
                else log.LogStatus("======Passed!!!======");
                if (verified < 7) verified++;
            }

            public static bool ComareValues(Size[] exp, Size[] act)
            {
                for (int v = 0; v < exp.Length; v++)
                {
                    if (exp[v] != act[v])
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridResizingOffsetTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                GridLength[] gridLength = new GridLength[] { new GridLength(100), new GridLength(80), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
                Grid grid = GridCommon.CreateGrid(4, 4, gridLength, gridLength);

                // Adding Rectangles in (0,0), (1,1), (2,2), and (3,3)
                Rectangle[] rect = new Rectangle[4];
                for (int r = 0; r < 4; r++)
                {
                    rect[r] = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Orange));
                    GridCommon.PlacingChild(grid, rect[r], r, r, true);
                    rect[r].HorizontalAlignment = HorizontalAlignment.Left;
                    rect[r].VerticalAlignment = VerticalAlignment.Top;
                }

                // Resetting static members for when tests run in a common app domain.
                resizeCount = 0;           
                result = true;

                return grid;
            }
            static int resizeCount = 0;           
            public static bool result = true;
            public static void ResizeWindow(Window _w, TestLog log)
            {
                int size;
                if (resizeCount < 3) size = 50;
                else size = -50;
                _w.Height = _w.Height + size;
                _w.Width = _w.Width + size;
                log.LogStatus("-" + resizeCount.ToString() + "- Window resized to " + ((Grid)_w.Content).RenderSize.Width.ToString() + "x" + ((Grid)_w.Content).RenderSize.Height.ToString());
                resizeCount++;
                GridResizingOffsetTestVerifier((Grid)_w.Content, log);
            }

            public static void GridResizingOffsetTestVerifier(Grid g, TestLog log)
            {                
                Point[] actual = GridCommon.GettingActualPosition(g);
                Point[] expected = GridCommon.SettingExpectedPosition(g);

                //Log status for Actual and Expected Value
                log.LogStatus("===Acutal Value===");
                for (int f = 0; f < actual.Length; f++)
                {
                    log.LogStatus("x: " + actual[f].X + "\ty: " + actual[f].Y);
                }
                log.LogStatus("===Expected Value===");
                for (int f = 0; f < expected.Length; f++)
                {
                    log.LogStatus("x: " + expected[f].X + "\ty: " + expected[f].Y);
                }
                // Compare the values 
                if (!CompareValues(expected, actual))
                {
                    result = false;
                    log.LogStatus("===Failed!!!===");
                }                
            }

            public static bool CompareValues(Point[] exp, Point[] act)
            {
                for (int c = 0; c < exp.Length; c++)
                {
                    if (exp[c] != act[c])
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridRelayoutOnPropertyChangeTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                GridLength[] gridLengths = new GridLength[] {
				new GridLength(100),
				new GridLength(80),
				new GridLength(1, GridUnitType.Auto),
				new GridLength(1,GridUnitType.Star)};

                Grid eRoot = GridCommon.CreateGrid(4, 4, gridLengths, gridLengths);

                //Adding Children (Rectangle)
                GridCommon.AddingCommonRectangle(eRoot, 50);

                //Setting Offset.
                for (int o = 0; o < eRoot.ColumnDefinitions.Count; o++)
                {
                    GridCommon.SettingOffset(eRoot.Children[0], 20, 20, 0, 0);
                }

                // Resetting static members for when tests run in a common app domain.
                relayoutOccurred = false;

                return eRoot;
            }
            public static bool relayoutOccurred = false;
            public static void OnLayoutUpdatedOccured(object sender, EventArgs e)
            {
                relayoutOccurred = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridRelayoutOnContentChangeTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                GridLength[] gridLength = new GridLength[] {
                new GridLength(100),
                new GridLength(2, GridUnitType.Star),
                new GridLength(1, GridUnitType.Auto),
                new GridLength(1, GridUnitType.Star)};
                Grid grid = GridCommon.CreateGrid(4, 4, gridLength, gridLength);

                // Resetting static members for when tests run in a common app domain.
                relayoutOccurred = false;
                result = true;

                return grid;
            }
            public static bool relayoutOccurred = false;
            public static void OnLayoutUpdatedOccured(object sender, EventArgs e)
            {
                relayoutOccurred = true;
            }
            public static bool result = true;
            public static bool GridRelayoutOnContentChangeTestVerifier(Grid _grid, TestLog log)
            {
                double expectedValue;
                double actualValue;

                if (relayoutOccurred)
                {
                    log.LogStatus("Layout updated~");
                    if (_grid.Children.Count > 0)
                    {
                        expectedValue = _grid.Children[0].RenderSize.Width;
                        actualValue = _grid.ColumnDefinitions[2].ActualWidth;
                    }
                    else
                    {
                        expectedValue = 5;
                        if (_grid.ColumnDefinitions.Count > 4)
                            actualValue = _grid.ColumnDefinitions.Count;
                        else
                            actualValue = _grid.RowDefinitions.Count;
                    }

                    log.LogStatus("Expected Value: " + expectedValue.ToString());
                    log.LogStatus("Actual Value: " + actualValue.ToString());

                    if (Math.Abs(expectedValue - actualValue) < 0.00001)
                        log.LogStatus("Expected and Actual values are matched~");
                    else
                    {
                        log.LogStatus(" Layout updated, but the actual value does not match with expected value!!!");
                        result = false;
                    }
                }
                else
                {
                    log.LogStatus("Layout did not updated~!!!");
                    result = false;
                }
                return result;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridDefinitionCollectionTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                DockPanel dockpanel = new DockPanel();
                //create 5x5 Grid with default Width & Height.
                Grid grid = GridCommon.CreateGrid(5, 5);
                for (int g = 0; g < 5; g++)
                {
                    grid.ColumnDefinitions[g].Name = "cd" + g.ToString();
                    grid.RowDefinitions[g].Name = "rd" + g.ToString();
                }
                grid.ShowGridLines = true;
                for (int c = 0; c < 5; c++)
                {
                    for (int r = 0; r < 5; r++)
                    {
                        TextBlock tb = new TextBlock();
                        tb.Text = "(" + c.ToString() + "," + r.ToString() + ")";
                        GridCommon.PlacingChild(grid, tb, c, r, true);
                    }
                }
                dockpanel.Children.Add(grid);
                return dockpanel;
            }

            public static ArrayList GetActualIDsForColumn(Grid g)
            {
                ArrayList actualIDs = new ArrayList();
                for (int c = 0; c < g.ColumnDefinitions.Count; c++)
                {
                    ColumnDefinition cd = g.ColumnDefinitions[c];
                    actualIDs.Add(cd.Name);
                }
                return actualIDs;
            }
            public static ArrayList GetExpectedIDsForColumn(Grid g)
            {
                ArrayList expectedIDs = new ArrayList();
                for (int i = 0; i < 5; i++)
                    expectedIDs.Add("cd" + i.ToString());
                return expectedIDs;
            }

            public static ArrayList GetActualIDsForRow(Grid g)
            {
                ArrayList actualIDs = new ArrayList();
                for (int r = 0; r < g.RowDefinitions.Count; r++)
                {
                    RowDefinition rd = g.RowDefinitions[r];
                    actualIDs.Add(rd.Name);
                }
                return actualIDs;
            }
            public static ArrayList GetExpectedIDsForRow(Grid g)
            {
                ArrayList expectedIDs = new ArrayList();
                for (int i = 0; i < 5; i++)
                    expectedIDs.Add("rd" + i.ToString());
                return expectedIDs;
            }

            public static string GetList(ArrayList list)
            {
                string listStr = null;
                foreach (string item in list)
                {
                    listStr += item + " ";
                }
                return listStr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class GridChildrenCollectionTestCommon
        {
            public static FrameworkElement CommonContent()
            {
                DockPanel dockpanel = new DockPanel();
                Grid grid = GridCommon.CreateGrid(5, 5);
                for (int g = 0; g < 5; g++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Orange));
                    rect.Name = "Rect" + g.ToString();
                    GridCommon.PlacingChild(grid, rect, g, g, true);
                }
                grid.ShowGridLines = true;

                dockpanel.Children.Add(grid);
                return dockpanel;
            }

            public static ArrayList GetActualIDsForChildren(Grid g)
            {
                ArrayList actualIDs = new ArrayList();
                for (int c = 0; c < g.Children.Count; c++)
                {
                    Rectangle element = g.Children[c] as Rectangle;
                    actualIDs.Add(element.Name);
                }
                return actualIDs;
            }
            public static ArrayList GetExpectedIDsForChildren(Grid g)
            {
                ArrayList expectedIDs = new ArrayList();
                for (int i = 0; i < 5; i++)
                    expectedIDs.Add("Rect" + i.ToString());
                return expectedIDs;
            }

            public static string GetList(ArrayList list)
            {
                string listStr = null;
                foreach (string item in list)
                {
                    listStr += item + " ";
                }
                return listStr;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static bool GridColumnDefinitionExceptionThrownTestVerifier(Type e, TestLog log)
        {
            bool result = (e != null) ? true : false;
            if (result) log.LogStatus(e.ToString() + "occurred.");
            else log.LogStatus("No Exception occurred~");

            return result;
        }
    }

    /// <summary>
    /// StackPanel common.
    /// </summary>
    public class StackPanelCommon
    {
        public static bool VerifyHorizontalStacking(string testPanel, StackPanel root, TestLog log)
        {
            log.LogStatus("Verify stacking of " + testPanel + " in horizontal direction.");

            FrameworkElement currentChild = null;
            FrameworkElement previousChild = null;

            Point currentChildLoc;
            Point previousChildLoc;

            Transform currentChildTransform;
            Transform previousChildTransform;

            int ChildCount = 0;

            IEnumerator children = LogicalTreeHelper.GetChildren(root).GetEnumerator();

            if (children != null)
            {
                while (children.MoveNext())
                {
                    if (testPanel != "Transform")
                    {
                        ChildCount++;
                        currentChild = children.Current as FrameworkElement;
                        if (currentChild == root.Children[0])
                        {
                            currentChildLoc = LayoutUtility.GetElementPosition(currentChild, root);
                            if (currentChildLoc.X != (0 + currentChild.Margin.Left))
                            {
                                log.LogStatus("child " + ChildCount + " location is not ok");
                                return false;
                            }
                            else
                            {
                                //log.LogStatus("child " + ChildCount + " location is ok");
                                previousChild = currentChild;
                            }
                        }
                        else
                        {
                            previousChildLoc = LayoutUtility.GetElementPosition(previousChild, root);
                            currentChildLoc = LayoutUtility.GetElementPosition(currentChild, root);

                            if (previousChild != null)
                            {
                                if (currentChildLoc.X != (previousChildLoc.X + previousChild.ActualWidth + previousChild.Margin.Right + currentChild.Margin.Left))
                                {
                                    log.LogStatus("child " + ChildCount + " location is not ok");
                                    return false;
                                }
                                else
                                {
                                    //log.LogStatus("child " + ChildCount + " location is ok");
                                    previousChild = currentChild;
                                }
                            }
                            else
                            {
                                log.LogStatus("previous child is null, test cannot continue.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        ChildCount++;
                        currentChild = children.Current as FrameworkElement;
                        currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
                        double getSquareValue = ((currentChild.ActualHeight * currentChild.ActualHeight) + (currentChild.ActualWidth * currentChild.ActualWidth));

                        double rotatedWidth = Math.Sqrt(getSquareValue);
                        if (currentChild == root.Children[0])
                        {
                            if (currentChildTransform.Value.OffsetX != (rotatedWidth / 2))
                            {
                                log.LogStatus("child " + ChildCount + " location is not ok");
                                return false;
                            }
                            else
                            {
                                //log.LogStatus("child " + ChildCount + " location is ok");
                                previousChild = currentChild;
                            }
                        }
                        else
                        {

                            currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
                            previousChildTransform = VisualTreeHelper.GetTransform(previousChild);

                            getSquareValue = ((previousChild.ActualHeight * previousChild.ActualHeight) + (previousChild.ActualWidth * previousChild.ActualWidth));

                            rotatedWidth = Math.Sqrt(getSquareValue);

                            double offset = rotatedWidth / 2;

                            if (previousChild != null)
                            {
                                if (VisualTreeHelper.GetOffset(currentChild).X != (rotatedWidth * (ChildCount - 1)))
                                {
                                    log.LogStatus("child " + ChildCount + " location is not ok");
                                    //Console.WriteStackPanelLine(currentChildTransform.Value.OffsetX);
                                    // previousChild = currentChild;
                                    return false;
                                }
                                else
                                {
                                    //log.LogStatus("child " + ChildCount + " location is ok");
                                    previousChild = currentChild;
                                }
                            }
                            else
                            {
                                log.LogStatus("previous child is null, test cannot continue.");
                                return false;
                            }
                        }
                    }
                }

            }
            else
            {
                log.LogStatus("Child enumerator is null! Test cannot continue.");
                return false;
            }
            return true;
        }

        public static bool VerifyVerticalStacking(string testPanel, StackPanel root, TestLog log)
        {
            log.LogStatus("Verify stacking of " + testPanel + " in Vertical direction.");

            FrameworkElement currentChild = null;
            FrameworkElement previousChild = null;

            Point currentChildLoc;
            Point previousChildLoc;

            Transform currentChildTransform;
            Transform previousChildTransform;

            int ChildCount = 0;

            IEnumerator children = LogicalTreeHelper.GetChildren(root).GetEnumerator();

            if (children != null)
            {
                while (children.MoveNext())
                {
                    if (testPanel != "Transform")
                    {
                        ChildCount++;
                        currentChild = children.Current as FrameworkElement;
                        if (currentChild == root.Children[0])
                        {
                            currentChildLoc = LayoutUtility.GetElementPosition(currentChild, root);
                            if (currentChildLoc.Y != (0 + currentChild.Margin.Top))
                            {
                                log.LogStatus("child " + ChildCount + " location is not ok");
                                return false;
                            }
                            else
                            {
                                //log.LogStatus("child " + ChildCount + " location is ok");
                                previousChild = currentChild;
                            }
                        }
                        else
                        {
                            previousChildLoc = LayoutUtility.GetElementPosition(previousChild, root);
                            currentChildLoc = LayoutUtility.GetElementPosition(currentChild, root);

                            if (previousChild != null)
                            {
                                if (currentChildLoc.Y != (previousChildLoc.Y + previousChild.ActualHeight + previousChild.Margin.Bottom + currentChild.Margin.Top))
                                {
                                    log.LogStatus("child " + ChildCount + " location is not ok");
                                    return false;
                                }
                                else
                                {
                                    //log.LogStatus("child " + ChildCount + " location is ok");
                                    previousChild = currentChild;
                                }
                            }
                            else
                            {
                                log.LogStatus("previous child is null, test cannot continue.");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        ChildCount++;
                        currentChild = children.Current as FrameworkElement;
                        currentChildTransform = VisualTreeHelper.GetTransform(currentChild);

                        if (currentChild == root.Children[0])
                        {
                            if (currentChildTransform.Value.OffsetY != 0)
                            {
                                log.LogStatus("child " + ChildCount + " location is not ok");
                                return false;
                            }
                            else
                            {
                                //log.LogStatus("child " + ChildCount + " location is ok");
                                previousChild = currentChild;
                            }
                        }
                        else
                        {
                            currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
                            previousChildTransform = VisualTreeHelper.GetTransform(previousChild);

                            double getSquareValue = ((previousChild.ActualHeight * previousChild.ActualHeight) + (previousChild.ActualWidth * previousChild.ActualWidth));

                            double rotatedHeight = Math.Sqrt(getSquareValue);

                            if (previousChild != null)
                            {
                                if (VisualTreeHelper.GetOffset(currentChild).Y != (rotatedHeight * (ChildCount - 1)))
                                {
                                    log.LogStatus("child " + ChildCount + " location is not ok");
                                    return false;
                                }
                                else
                                {
                                    //log.LogStatus("child " + ChildCount + " location is ok");
                                    previousChild = currentChild;
                                }
                            }
                            else
                            {
                                log.LogStatus("previous child is null, test cannot continue.");
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                log.LogStatus("Child enumerator is null! Test cannot continue.");
                return false;
            }
            return true;
        }

        public static void AddChildren(int ChildCount, string ChildType, StackPanel root, TestLog log)
        {
            int i;
            switch (ChildType)
            {
                case "Panel":
                    {
                        log.LogStatus("Testing Stacking of Panel");
                        for (i = 0; i < ChildCount; i++)
                        {
                            TestPanel panel = new TestPanel();
                            panel.Margin = new Thickness(10);
                            panel.Background = Brushes.YellowGreen;
                            panel.Height = 220;
                            panel.Width = 220;
                            root.Children.Add(panel);
                        }
                        break;
                    }

                case "Canvas":
                    {
                        log.LogStatus("Testing Stacking of Canvas");
                        for (i = 0; i < ChildCount; i++)
                        {
                            Canvas canvas = CommonFunctionality.CreateCanvas(175, 175, Brushes.YellowGreen);
                            Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                            Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                            canvas.Children.Add(r1);
                            canvas.Children.Add(r2);
                            Canvas.SetLeft(r1, 10);
                            Canvas.SetTop(r1, 10);
                            Canvas.SetBottom(r2, 10);
                            Canvas.SetRight(r2, 10);
                            root.Children.Add(canvas);
                        }
                        break;
                    }

                case "StackPanel":
                    {
                        log.LogStatus("Testing Stacking of StackPanel");
                        for (i = 0; i < ChildCount; i++)
                        {
                            StackPanel stack = new StackPanel();
                            stack.Height = 300;
                            stack.Width = 300;
                            stack.Background = Brushes.GhostWhite;
                            Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                            Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                            Rectangle r3 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                            stack.Children.Add(r1);
                            stack.Children.Add(r2);
                            stack.Children.Add(r3);
                            root.Children.Add(stack);
                        }
                        break;
                    }

                case "Grid":
                    {
                        log.LogStatus("Testing Stacking of Grid");
                        for (i = 0; i < ChildCount; i++)
                        {
                            ColumnDefinition col1, col2;
                            RowDefinition row1, row2;
                            Grid grid = new Grid();
                            grid.Height = 200;
                            grid.Width = 200;
                            grid.Background = Brushes.Crimson;
                            grid.ColumnDefinitions.Add(col1 = new ColumnDefinition());
                            grid.ColumnDefinitions.Add(col2 = new ColumnDefinition());
                            grid.RowDefinitions.Add(row1 = new RowDefinition());
                            grid.RowDefinitions.Add(row2 = new RowDefinition());

                            Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Gray);
                            Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.HotPink);

                            Grid.SetRow(r1, 1);
                            Grid.SetColumn(r1, 1);
                            Grid.SetRow(r2, 0);
                            Grid.SetColumn(r2, 0);

                            grid.Children.Add(r1);
                            grid.Children.Add(r2);

                            root.Children.Add(grid);
                        }
                        break;
                    }

                case "DockPanel":
                    {
                        log.LogStatus("Testing Stacking of DockPanel");
                        for (i = 0; i < ChildCount; i++)
                        {
                            DockPanel dock = new DockPanel();
                            dock.Height = 250;
                            dock.Width = 250;
                            dock.Background = Brushes.Firebrick;

                            Rectangle r1 = CommonFunctionality.CreateRectangle(250, 100, Brushes.Gray);
                            Rectangle r2 = CommonFunctionality.CreateRectangle(100, 150, Brushes.HotPink);

                            dock.Children.Add(r1);
                            dock.Children.Add(r2);

                            DockPanel.SetDock(r1, Dock.Top);
                            DockPanel.SetDock(r2, Dock.Right);
                            root.Children.Add(dock);
                        }
                        break;
                    }

                case "Decorator":
                    {
                        log.LogStatus("Testing Stacking of Decorator");
                        for (i = 0; i < ChildCount; i++)
                        {
                            Decorator dec = new Decorator();
                            dec.Height = 230;
                            dec.Width = 230;
                            dec.Margin = new Thickness(25);

                            Rectangle r = new Rectangle();
                            r.Fill = Brushes.Crimson;
                            r.Margin = new Thickness(10);
                            dec.Child = r;
                            root.Children.Add(dec);
                        }
                        break;
                    }

                case "Border":
                    {
                        log.LogStatus("Testing Stacking of Border");
                        for (i = 0; i < ChildCount; i++)
                        {
                            Border bor = new Border();
                            bor.Height = 200;
                            bor.Width = 200;
                            bor.CornerRadius = new CornerRadius(50);
                            bor.Background = Brushes.Gray;
                            bor.Padding = new Thickness(50);

                            Rectangle r = CommonFunctionality.CreateRectangle(100, 100, Brushes.Orange);

                            bor.Child = r;

                            root.Children.Add(bor);
                        }
                        break;
                    }

                case "Viewbox":
                    {
                        log.LogStatus("Testing Stacking of ViewBox");
                        for (i = 0; i < ChildCount; i++)
                        {
                            Viewbox vb = new Viewbox();
                            vb.Height = 225;
                            vb.Width = 225;
                            vb.Stretch = Stretch.Fill;

                            TextBlock txt = new TextBlock();
                            txt.Text = "foo";

                            vb.Child = txt;

                            root.Children.Add(vb);
                        }
                        break;
                    }

                case "Transform":
                    {
                        log.LogStatus("Testing Stacking of Transform");
                        for (i = 0; i < ChildCount; i++)
                        {
                            RotateTransform rt = new RotateTransform(45);

                            //TransformDecorator td = new TransformDecorator();
                            Decorator td = new Decorator();
                            td.LayoutTransform = rt;
                            Rectangle r = new Rectangle();
                            r.Height = 200;
                            r.Width = 200;

                            r.Fill = Brushes.AntiqueWhite;

                            td.Child = r;
                            root.Children.Add(td);
                        }
                        break;
                    }

                case "ScrollViewer":
                    {
                        log.LogStatus("Testing Stacking of ScrollViewer");
                        for (i = 0; i < ChildCount; i++)
                        {
                            ScrollViewer sv = new ScrollViewer();
                            sv.Height = 350;
                            sv.Width = 350;
                            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                            TextBlock txt = new TextBlock();
                            txt.Text = "some text";
                            txt.FontSize = 100;

                            sv.Content = txt;

                            root.Children.Add(sv);
                        }
                        break;
                    }

            }
        }

        public static void AddSizedChildren(int ChildCount, string SizeType, StackPanel stack, TestLog log)
        {
            double IncreasingValue = 10;
            double DecreasingValue = 1000;

            switch (SizeType)
            {
                case "Small":
                    log.LogStatus("Testing : Small Child Size");
                    for (int i = 0; i < ChildCount; i++)
                    {
                        Border child = new Border();
                        child.Height = 10;
                        child.Width = 10;
                        child.Background = Brushes.Gray;
                        stack.Children.Add(child);
                    }
                    break;
                case "Medium":
                    log.LogStatus("Testing : Medium Child Size");
                    for (int i = 0; i < ChildCount; i++)
                    {
                        Border child = new Border();
                        child.Height = 100;
                        child.Width = 100;
                        child.Background = Brushes.Gray;
                        stack.Children.Add(child);
                    }
                    break;
                case "Large":
                    log.LogStatus("Testing : Large Child Size");
                    for (int i = 0; i < ChildCount; i++)
                    {
                        Border child = new Border();
                        child.Height = 1000;
                        child.Width = 1000;
                        child.Background = Brushes.Gray;
                        stack.Children.Add(child);
                    }
                    break;
                case "Increasing":
                    log.LogStatus("Testing : Increasing Child Size");
                    for (int i = 0; i < ChildCount; i++)
                    {
                        Border child = new Border();
                        child.Height = IncreasingValue;
                        child.Width = IncreasingValue;
                        child.Background = Brushes.Gray;
                        stack.Children.Add(child);
                        IncreasingValue = IncreasingValue + 1;
                    }
                    break;
                case "Decreasing":
                    log.LogStatus("Testing : Decreasing Child Size");
                    for (int i = 0; i < ChildCount; i++)
                    {
                        Border child = new Border();
                        child.Height = DecreasingValue;
                        child.Width = DecreasingValue;
                        child.Background = Brushes.Gray;
                        stack.Children.Add(child);
                        DecreasingValue = DecreasingValue - 1;
                    }
                    break;
            }

        }
    }

    /// <summary>
    /// ScrollViewer Test Common.
    /// </summary>
    public class ScrollTestCommon
    {
        #region ScrollerCommon
        /// <summary>This returns VerticalScrollBar's Height. If there is no VerticalScrollBar, this will return 0.</summary>
        /// <param name="sv">ScrollViewer</param>
        /// <returns>VerticalScrollBar's Height in double</returns>
        public static double VerticalScrollBarSize(ScrollViewer sv)
        {
            ScrollBar vsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 0) as ScrollBar;
            if (vsb == null)
                return 0;
            else
                return vsb.ActualHeight;
        }

        /// <summary>This returns HorizontalScrollBar's Width. If there is no HorizontalScrollBar, this will return 0.</summary>
        /// <param name="sv">ScrollViewer</param>
        ///	<returns>HorizontalScrollBar's Width in double</returns>
        public static double HorizontalScrollBarSize(ScrollViewer sv)
        {
            ScrollBar hsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 1) as ScrollBar;
            if (hsb == null)
                return 0;
            else
                return hsb.ActualWidth;
        }
        #endregion ScrollerCommon

        #region ScrollerVisibilities
        ///<summary>ScrollerVisibilities used when the case needs to be verified with ScrollBarVisibilityX and ScrollerVieibilityY.</summary>
        public struct ScrollerVisibilities
        {
            /// <summary></summary>
            public ScrollBarVisibility horizontal;

            /// <summary></summary>
            public ScrollBarVisibility vertical;

            /// <summary></summary>
            /// <param name="sv"></param>
            public ScrollerVisibilities(ScrollViewer sv)
            {
                GlobalLog.LogEvidence("IN SV COMMON");
                ScrollBar hsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 1) as ScrollBar;
                ScrollBar vsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 0) as ScrollBar;
                if (hsb.Visibility == Visibility.Collapsed)
                {
                    if (sv.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto)
                        horizontal = ScrollBarVisibility.Hidden;
                    else
                        horizontal = ScrollBarVisibility.Disabled;
                }
                else
                    horizontal = ScrollBarVisibility.Visible;
                vertical = (vsb.Visibility == Visibility.Collapsed) ? ScrollBarVisibility.Hidden : ScrollBarVisibility.Visible;
            }

            /// <summary></summary>
            /// <param name="svX"></param>
            /// <param name="svY"></param>
            public ScrollerVisibilities(ScrollBarVisibility svX, ScrollBarVisibility svY)
            {
                this.horizontal = svX;
                this.vertical = svY;
            }
        }

        /// <summary>GetExptectedScrollerVisibilities: return the ScrollerVisibilities decided by comparing Computed Width of ScrollViewer content and ViewportSize.</summary>
        /// <param name="sv">ScrollViewer</param>
        /// <returns>ScrollBarVisibilityX and Y value of ScrollVeiwer in ScrollerVisibilities </returns>
        static public ScrollerVisibilities GetExpectedScrollerVisibilities(ScrollViewer sv)
        {
            ScrollerVisibilities expected = new ScrollerVisibilities();
            UIElement scrollViewerContent = (UIElement)sv.Content;

            if ((ScrollBarVisibility)sv.GetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty) != ScrollBarVisibility.Disabled)
                expected.horizontal = (scrollViewerContent.RenderSize.Width > sv.ViewportWidth) ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            else
                expected.horizontal = ScrollBarVisibility.Disabled;
            expected.vertical = ScrollBarVisibility.Visible; // since default value of VerticalScrollBarVisibility = Visible
            return expected;
        }
        #endregion ScrollerVisibilities

        #region ThumbSize
        /// <summary>ThumbSize used with the case needs to be verified with size of Thumb. This holds two value for Horizontal Scrollbar's Thumb size and Vertical Scrollbar's Thumb size.</summary>
        public struct ThumbSize
        {
            /// <summary></summary>
            public double thumSizeX;

            /// <summary></summary>
            public double thumSizeY;

            /// <summary></summary>
            /// <param name="sv"></param>
            public ThumbSize(ScrollViewer sv)
            {
                ScrollBar hsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 1) as ScrollBar;
                Track htrack = LayoutUtility.GetChildFromVisualTree(hsb, typeof(Track), 0) as Track;
                Thumb hThumb = htrack.Thumb;
                ScrollBar vsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 0) as ScrollBar;
                Track vtrack = LayoutUtility.GetChildFromVisualTree(vsb, typeof(Track), 0) as Track;
                Thumb vThumb = vtrack.Thumb;
                thumSizeX = (hThumb == null) ? 0 : hThumb.ActualWidth;
                thumSizeY = (vThumb == null) ? 0 : vThumb.ActualHeight;
            }

            /// <summary></summary>
            /// <param name="tsX"></param>
            /// <param name="tsY"></param>
            public ThumbSize(double tsX, double tsY)
            {
                this.thumSizeX = tsX;
                this.thumSizeY = tsY;
            }
        }

        /// <summary></summary>
        public const double minThumbSize = 8;

        /// <summary></summary>
        public const double lineButtonDefaultSize = 16; //for Theme: Windows Classic

        /// <summary>If Extent is less than or equal to ViewportSize, then Thumb is not shown (Thumb size will be zero). If Extent is greater than ViewportSize, then ThumbSize = (ViewportSize / Extent) * TrackLength If trackLength &lt; minThumbSize, Thumb will be removed. (Thumb size will be zero).</summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        static public ThumbSize GetExpectedThumSize(ScrollViewer sv)
        {
            ThumbSize expected = new ThumbSize();

            if (sv.ExtentWidth > sv.ViewportWidth)
            {
                ScrollBar hsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 1) as ScrollBar;
                double trackLength = new TrackLength(sv).TrackLengthX;
                if (trackLength < minThumbSize)
                    expected.thumSizeX = 0;
                else
                {
                    double computedThumbSize = sv.ViewportWidth / sv.ExtentWidth * trackLength;
                    expected.thumSizeX = (computedThumbSize > minThumbSize) ? computedThumbSize : minThumbSize;
                }
            }
            else
                expected.thumSizeX = 0;

            if (sv.ExtentHeight > sv.ViewportHeight)
            {
                ScrollBar vsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 0) as ScrollBar;
                double trackLength = new TrackLength(sv).TrackLengthY;
                if (trackLength < minThumbSize)
                    expected.thumSizeY = 0;
                else
                {
                    double computedThumbSize = sv.ViewportHeight / sv.ExtentHeight * trackLength;
                    expected.thumSizeY = (computedThumbSize > minThumbSize) ? computedThumbSize : minThumbSize;
                }
            }
            else
                expected.thumSizeY = 0;

            return expected;
        }
        #endregion ThumbSize

        #region ThumbPosition
        /// <summary></summary>
        public struct ThumbPosition
        {
            /// <summary></summary>
            public double ThumbPositionX;
            /// <summary></summary>
            public double ThumbPositionY;

            /// <summary></summary>
            /// <param name="sv"></param>
            public ThumbPosition(ScrollViewer sv)
            {
                ScrollBar hsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 1) as ScrollBar;
                Track htrack = LayoutUtility.GetChildFromVisualTree(hsb, typeof(Track)) as Track;
                Thumb hThumb = htrack.Thumb;
                ScrollBar vsb = LayoutUtility.GetChildFromVisualTree(sv, typeof(ScrollBar), 0) as ScrollBar;
                Track vtrack = LayoutUtility.GetChildFromVisualTree(vsb, typeof(Track)) as Track;
                Thumb vThumb = vtrack.Thumb;
                ThumbPositionX = (LayoutUtility.GetElementPosition(hThumb, sv)).X;
                ThumbPositionY = LayoutUtility.GetElementPosition(vThumb, sv).Y;
            }

            /// <summary></summary>
            /// <param name="tpX"></param>
            /// <param name="tpY"></param>
            public ThumbPosition(double tpX, double tpY)
            {
                this.ThumbPositionX = tpX;
                this.ThumbPositionY = tpY;
            }
        }
        #endregion ThumbPosition

        #region TrackLength
        /// <summary>TrackLength used when Thumb size needs to be calculated. TrackLength = ArrangeSize - (ButtonSize * number of buttons)</summary>		
        public struct TrackLength
        {
            /// <summary></summary>
            public double TrackLengthX;

            /// <summary></summary>
            public double TrackLengthY;

            /// <summary></summary>
            /// <param name="sv"></param>
            public TrackLength(ScrollViewer sv)
            {
                ScrollerVisibilities scrollerVisibilities = new ScrollerVisibilities(sv);
                this.TrackLengthX = (scrollerVisibilities.vertical == ScrollBarVisibility.Visible) ? (sv.ActualWidth - (lineButtonDefaultSize * 3)) : (sv.ActualWidth - (lineButtonDefaultSize * 2));
                this.TrackLengthY = (scrollerVisibilities.horizontal == ScrollBarVisibility.Visible) ? (sv.ActualHeight - (lineButtonDefaultSize * 3)) : (sv.ActualHeight - (lineButtonDefaultSize * 2));
            }
        }
        #endregion TrackLength

        #region
        /// <summary></summary>
        /// <param name="sv"></param>
        public static Point SVContentPosition(ScrollViewer sv)
        {
            UIElement content = sv.Content as UIElement;
            return LayoutUtility.GetElementPosition(content, sv);
        }
        #endregion


        #region Equal
        /// <summary>Compare the Values of ScrollerVisibilities.</summary>
        /// <param name="actual">the actual value of ScrollerVisibilities to compare.</param>
        /// <param name="expected">the expected value of ScrollerVisibilities to compare</param>
        /// <returns>return true if actual and expected values are the same.</returns>
        public static bool Equal(ScrollerVisibilities actual, ScrollerVisibilities expected)
        {
            return (actual.horizontal == expected.horizontal) && (actual.vertical == expected.vertical);
        }

        /// <summary>Compare the Values of ScrollerVisibilities.</summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns>return true if actual and expected values are the same.</returns>
        public static bool Equal(ThumbSize actual, ThumbSize expected)
        {
            return (Math.Abs(actual.thumSizeX - expected.thumSizeX) < 0.001) && (Math.Abs(actual.thumSizeY - expected.thumSizeY) < 0.001);
        }

        public static bool Equal(Point actual, Point expected)
        {
            return ((Math.Abs(actual.X - expected.X) < 0.001) && (Math.Abs(actual.Y - expected.Y) < 0.001));
        }
        /// <summary>Compare the Values of ScrollerVisibilities.</summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns>return true if actual and expected values are the same.</returns>
        public static bool Equal(double actual, double expected)
        {
            return (Math.Abs(actual - expected) < 0.001);
        }

        #endregion Equal

        #region GetFailureMsg
        /// <summary>Message for the result when it's failed.</summary>
        /// <param name="actual">the actual value of ScrollerVisibilities/ThumbSize</param>
        /// <param name="expected">the expected value of ScrollerVisibilities/ThumbSize</param>
        /// <returns>failure message in string</returns>
        public static string GetFailureMsg(ScrollerVisibilities actual, ScrollerVisibilities expected)
        {
            string msg = null;

            msg += "\n\tExpected Value:\n\t\tHorizontalScrollBar:\t" + expected.horizontal + "\n\t\tVerticalScrollBar:\t" + expected.vertical;
            msg += "\n\tActual Value:\n\t\tHorizontalScrollBar:\t" + actual.horizontal + "\n\t\tVerticalScrollBar:\t" + actual.vertical;
            return msg;
        }

        /// <summary>Message for the result when it's failed.</summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static string GetFailureMsg(ThumbSize actual, ThumbSize expected)
        {
            string msg = null;

            msg += "\n\tExpected Value:\n\t\tThumbSizeX:\t" + expected.thumSizeX + "\n\t\tThumbSizeY:\t" + expected.thumSizeY;
            msg += "\n\tActual Value:\n\t\tThumbSizeX:\t" + actual.thumSizeX + "\n\t\tThumbSizeY:\t" + actual.thumSizeY;
            return msg;
        }

        /// <summary>Message for the result when it's failed.</summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static string GetFailureMsg(Point actual, Point expected)
        {
            string msg = null;

            msg += "\n\tExpected Value X:" + expected.X;
            msg += "\n\tExpected Value Y:" + expected.Y;
            msg += "\n\tActual Value X:" + actual.X;
            msg += "\n\tActual Value Y:" + actual.Y;
            return msg;
        }


        /// <summary>Message for the result when it's failed.</summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static string GetFailureMsg(double actual, double expected)
        {
            string msg = null;

            msg += "\n\tExpected Value:" + expected;
            msg += "\n\tActual Value:" + actual;
            return msg;
        }

        #endregion GetFailureMsg

        /// <summary>
        /// Random Scroll Amount
        /// </summary>
        /// <param name="scroll"></param>
        /// <returns></returns>
        public static int RandomScrollAmount(ScrollViewer scroll)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int RandomScrollAmount = random.Next(0, (((int)scroll.ExtentHeight / 2) / 16));
            //GlobalLog.LogStatus(RandomScrollAmount);
            return RandomScrollAmount;
        }

        /// <summary>
        /// Scroll Left Command
        /// </summary>
        /// <param name="_ScrollAmount"></param>
        /// <param name="scroll"></param>
        public static void ScrollLeft(int _ScrollAmount, ScrollViewer scroll)
        {
            for (int i = 0; i < _ScrollAmount; i++)
            {
                scroll.LineLeft();
                CommonFunctionality.FlushDispatcher();
            }
        }

        /// <summary>
        /// Scroll Right Command
        /// </summary>
        /// <param name="_ScrollAmount"></param>
        /// <param name="scroll"></param>
        public static void ScrollRight(int _ScrollAmount, ScrollViewer scroll)
        {
            for (int i = 0; i < _ScrollAmount; i++)
            {
                scroll.LineRight();
                CommonFunctionality.FlushDispatcher();
            }
        }

        /// <summary>
        /// Scroll Up Command.
        /// </summary>
        /// <param name="_ScrollAmount"></param>
        /// <param name="scroll"></param>
        public static void ScrollUp(int _ScrollAmount, ScrollViewer scroll)
        {
            for (int i = 0; i < _ScrollAmount; i++)
            {
                scroll.LineUp();
                CommonFunctionality.FlushDispatcher();
            }
        }

        /// <summary>
        /// Scroll Down Command.
        /// </summary>
        /// <param name="_ScrollAmount"></param>
        /// <param name="scroll"></param>
        public static void ScrollDown(int _ScrollAmount, ScrollViewer scroll)
        {
            for (int i = 0; i < _ScrollAmount; i++)
            {
                scroll.LineDown();
                CommonFunctionality.FlushDispatcher();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        static public LayoutTestResult Verification(ScrollViewer sv)
        {
            LayoutTestResult tr = new LayoutTestResult();

            ScrollerVisibilities actual = new ScrollerVisibilities(sv);
            ScrollerVisibilities expected = GetExpectedScrollerVisibilities(sv);
            // Get Result

            bool result = Equal(actual, expected);
            string resultMsg = GetFailureMsg(actual, expected);
            // Log Result.
            tr.message = resultMsg;
            tr.result = result;
            return tr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sv"></param>
        /// <returns></returns>
        static public LayoutTestResult VerifyWithThumb(ScrollViewer sv)
        {
            LayoutTestResult tr = new LayoutTestResult();

            ThumbSize actual = new ThumbSize(sv);
            ThumbSize expected = GetExpectedThumSize(sv);

            bool result = Equal(actual, expected);

            if (!result)
            {
                TestLog.Current.LogStatus("Expected sizes were different, scroll line up/down buttons differ by one pixel on classic and aero.");

                if (actual.thumSizeX - expected.thumSizeX <= 1 && actual.thumSizeY - expected.thumSizeY <= 1)
                {
                    TestLog.Current.LogStatus("With less than 1 pixel buffer, test pass.");
                    result = true;
                }
            }

            string resultMsg = GetFailureMsg(actual, expected);

            tr.result = result;
            tr.message = resultMsg;

            return tr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static public LayoutTestResult VerifyWithScrollBarSize(ScrollViewer sv, Type type)
        {
            double actual, expected;
            actual = (type == typeof(ScrollBar)) ? VerticalScrollBarSize(sv) : HorizontalScrollBarSize(sv);

            UIElement content = sv.Content as UIElement;
            expected = (type == typeof(ScrollBar)) ? content.RenderSize.Height : content.RenderSize.Width;

            bool result = Equal(actual, expected);
            string resultMsg = GetFailureMsg(actual, expected);

            LayoutTestResult tr = new LayoutTestResult();
            tr.message = resultMsg;
            tr.result = result;

            return tr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        static public void AddMoreContent(WrapPanel w)
        {
            for (int i = 0; i < 50; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Red);
                w.Children.Add(r);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        static public void RemoveContent(WrapPanel w)
        {
            //Paragraph p = LayoutUtility.GetChildFromVisualTree(tp, typeof(Paragraph),1) as Paragraph;
            //tp.Document.Blocks.Clear();
            //int pnum = LayoutUtility.GetChildCountFromLogicalTree(tp, typeof(Paragraph));
            //for (int i = 0; i < pnum; i++)
            //{
            //    Paragraph p = LayoutUtility.GetChildFromLogicalTree(tp, typeof(Paragraph), i) as Paragraph;
            //    p.Text = "";
            //}
            w.Children.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="rect"></param>
        static public void AddRectangle(ScrollViewer sv, Rectangle rect)
        {
            sv.Content = null;
            sv.Content = rect;
        }

        public static void AddScrollViewerContent(string _TestElement, ScrollViewer scroll)
        {
            scroll.Content = AddScrollViewerContent(_TestElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_TestElement"></param>
        /// <returns></returns>
        public static FrameworkElement AddScrollViewerContent(string _TestElement)
        {
            FrameworkElement testelement = null;

            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Height = 600;
                    panel.Width = 600;
                    panel.Background = Brushes.LightSteelBlue;
                    testelement = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Height = 600;
                    canvas.Width = 600;
                    canvas.Background = Brushes.LightSteelBlue;

                    Rectangle c1 = CommonFunctionality.CreateRectangle(400, 400, Brushes.LightGray);
                    canvas.Children.Add(c1);
                    Canvas.SetLeft(c1, 25);
                    Canvas.SetBottom(c1, 25);

                    Rectangle c2 = CommonFunctionality.CreateRectangle(400, 400, Brushes.LightGreen);
                    canvas.Children.Add(c2);
                    Canvas.SetRight(c2, 25);
                    Canvas.SetTop(c2, 25);

                    testelement = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Height = 600;
                    stack.Width = 600;
                    stack.Background = Brushes.LightSalmon;
                    stack.Orientation = Orientation.Horizontal;

                    Rectangle s1 = CommonFunctionality.CreateRectangle(200, 300, Brushes.LightGray);
                    stack.Children.Add(s1);

                    Rectangle s2 = CommonFunctionality.CreateRectangle(200, 100, Brushes.LightGreen);
                    stack.Children.Add(s2);

                    Rectangle s3 = CommonFunctionality.CreateRectangle(200, 400, Brushes.LavenderBlush);
                    stack.Children.Add(s3);

                    testelement = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Height = 600;
                    grid.Width = 600;
                    grid.Background = Brushes.Orange;

                    ColumnDefinition col1 = new ColumnDefinition();
                    ColumnDefinition col2 = new ColumnDefinition();
                    ColumnDefinition col3 = new ColumnDefinition();

                    RowDefinition row1 = new RowDefinition();
                    RowDefinition row2 = new RowDefinition();

                    grid.RowDefinitions.Add(row1);
                    grid.RowDefinitions.Add(row2);
                    grid.ColumnDefinitions.Add(col1);
                    grid.ColumnDefinitions.Add(col2);
                    grid.ColumnDefinitions.Add(col3);

                    Rectangle g1 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, Brushes.LightGray);
                    grid.Children.Add(g1);
                    Grid.SetRow(g1, 1);
                    Grid.SetColumn(g1, 1);

                    Rectangle g2 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, Brushes.LightGreen);
                    grid.Children.Add(g2);
                    Grid.SetRow(g2, 0);
                    Grid.SetColumn(g2, 0);

                    Rectangle g3 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, Brushes.LavenderBlush);
                    grid.Children.Add(g3);
                    Grid.SetRow(g3, 0);
                    Grid.SetColumn(g3, 2);

                    testelement = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Height = 600;
                    dock.Width = 600;
                    dock.Background = Brushes.Blue;

                    Rectangle d1 = CommonFunctionality.CreateRectangle(double.NaN, 200, Brushes.LightGray);
                    dock.Children.Add(d1);
                    DockPanel.SetDock(d1, Dock.Bottom);

                    Rectangle d2 = CommonFunctionality.CreateRectangle(200, double.NaN, Brushes.LightGreen);
                    dock.Children.Add(d2);
                    DockPanel.SetDock(d2, Dock.Left);

                    Rectangle d3 = CommonFunctionality.CreateRectangle(100, 100, Brushes.LavenderBlush);
                    dock.Children.Add(d3);

                    testelement = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 600;
                    decorator.Width = 600;

                    Border b = new Border();
                    b.Margin = new Thickness(25);
                    b.Background = Brushes.Crimson;

                    decorator.Child = b;

                    testelement = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 600;
                    border.Width = 600;
                    border.CornerRadius = new CornerRadius(300);
                    border.BorderBrush = Brushes.Black;
                    border.BorderThickness = new Thickness(10);
                    testelement = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 600;
                    viewbox.Width = 600;
                    viewbox.Stretch = Stretch.Uniform;

                    TextBlock tb = new TextBlock();
                    tb.FontSize = 12;
                    tb.FontFamily = new FontFamily("Tahoma");
                    tb.Foreground = Brushes.Black;
                    tb.Text = "viewbox";

                    viewbox.Child = tb;

                    testelement = viewbox;
                    break;

                case "Transform":
                    Border transformBorder = new Border();
                    transformBorder.Background = Brushes.Blue;

                    RotateTransform rt = new RotateTransform(30, 300, 300);

                    Decorator td = new Decorator();
                    td.Height = 600;
                    td.Width = 600;
                    td.Child = transformBorder;
                    td.LayoutTransform = rt;

                    testelement = td;
                    break;

                case "WrapPanel":
                    WrapPanel wrap = new WrapPanel();
                    wrap.Height = 600;
                    wrap.Width = 600;
                    wrap.ItemHeight = 200;
                    wrap.ItemWidth = 200;
                    for (int i = 0; i < 9; i++)
                    {
                        Border wc = new Border();
                        wc.Background = Brushes.LightGray;
                        TextBlock txt = new TextBlock();
                        txt.FontFamily = new FontFamily("Tahoma");
                        txt.FontSize = 100;
                        txt.Foreground = Brushes.White;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                        txt.VerticalAlignment = VerticalAlignment.Center;
                        txt.Text = i.ToString();
                        wc.Child = txt;
                        wrap.Children.Add(wc);
                    }
                    testelement = wrap;
                    break;

                default:
                    break;
            }
            return testelement;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public static void ChangeToLeftyScroll(Window w)
        {
            string xamlfile = "leftyscroll.xaml";
            FileStream f = new FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            w.Content = (FrameworkElement)XamlReader.Load(f);
            f.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eRoot"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static LayoutTestResult ScrollBarVerification(FrameworkElement eRoot, ScrollTestCommon.ScrollerVisibilities expected)
        {
            ScrollViewer sv = LayoutUtility.GetChildFromVisualTree(((UIElement)eRoot), typeof(ScrollViewer)) as ScrollViewer;
            ScrollerVisibilities actualVisibilities = new ScrollerVisibilities(sv);
            // Get Result and result message
            bool result = Equal(actualVisibilities, expected);
            string resultMsg = GetFailureMsg(actualVisibilities, expected);
            // Log Result.
            //ScrollingBascFunc.frm.LogTest(result, resultMsg);
            // Close Window
            //ScrollingBascFunc.testWin.Close();

            LayoutTestResult tr = new LayoutTestResult();
            tr.message = resultMsg;
            tr.result = result;
            return tr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eRoot"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        public static LayoutTestResult ContentPositionVerification(FrameworkElement eRoot, ScrollTestCommon.ScrollerVisibilities expected)
        {
            ScrollViewer sv = LayoutUtility.GetChildFromVisualTree(((UIElement)eRoot), typeof(ScrollViewer)) as ScrollViewer;
            Point contentPosition = SVContentPosition(sv);
            Point expectedPosition = new Point((sv.HorizontalOffset) * (-1), (sv.VerticalOffset) * (-1));
            // Get Result and result message
            bool result = Equal(contentPosition, expectedPosition);
            string resultMsg = GetFailureMsg(contentPosition, expectedPosition);
            // Log Result.
            LayoutTestResult tr = new LayoutTestResult();
            tr.message = resultMsg;
            tr.result = result;
            return tr;
            //ScrollingBascFunc.frm.LogTest(result, resultMsg);
            // Close Window
            //ScrollingBascFunc.testWin.Close();

        }

        /// <summary>
        /// Test Result Info.
        /// </summary>
        public struct LayoutTestResult
        {
            /// <summary>
            /// Test Result.
            /// </summary>
            public bool result;

            /// <summary>
            /// Commments for tests.
            /// </summary>
            public string message;
        }
    }

    /// <summary>
    /// Helpers for layout transform cases.
    /// </summary>
    public class LayoutTransformHelpers
    {
        public static FrameworkElement AddingChild(string childElement)
        {
            FrameworkElement feChild;
            feChild = new FrameworkElement();
            switch (childElement)
            {
                case "Border":
                    Border border = new Border();
                    border.Width = 150;
                    border.Height = 100;
                    border.Background = Brushes.Orange;
                    feChild = (FrameworkElement)border;
                    break;
                case "Button":
                    Button btn = new Button();
                    btn.Content = "Button";
                    feChild = (FrameworkElement)btn;
                    break;
                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Width = 150;
                    canvas.Height = 100;
                    canvas.Background = Brushes.Orange;
                    Rectangle rect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Green));
                    canvas.Children.Add(rect);
                    Canvas.SetTop(rect, 20);
                    Canvas.SetLeft(rect, 20);
                    feChild = (FrameworkElement)canvas;
                    break;
                case "CheckBox":
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = "Check Box";
                    checkBox.IsChecked = true;
                    feChild = (FrameworkElement)checkBox;
                    break;
                case "ComboBox":
                    ComboBox cBox = new ComboBox();
                    for (int i = 1; i <= 5; i++)
                    {
                        ComboBoxItem cBoxItem = new ComboBoxItem();
                        cBoxItem.Content = "ComboBoxItem " + i.ToString();
                        cBox.Items.Add(cBoxItem);
                        if (i == 1)
                            cBoxItem.IsSelected = true;
                    }
                    feChild = (FrameworkElement)cBox;
                    break;
                /*
                case "ContextMenu":
                    ContextMenu cMenu = new ContextMenu();
                    break;
                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Width = 150;
                    decorator.Height = 100;
                    feChild = (FrameworkElement)decorator;
                    break;
                */
                case "DockPanel":
                    DockPanel dockPanel = new DockPanel();
                    dockPanel.Background = Brushes.LightBlue;
                    Rectangle rect1 = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Orange));
                    DockPanel.SetDock(rect1, Dock.Left);
                    dockPanel.Children.Add(rect1);
                    Rectangle rect2 = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Green));
                    DockPanel.SetDock(rect2, Dock.Top);
                    dockPanel.Children.Add(rect2);
                    Rectangle rect3 = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Yellow));
                    dockPanel.Children.Add(rect3);
                    feChild = (FrameworkElement)dockPanel;
                    break;
                case "DocumentViewer":
                    DocumentViewer docViewer = new DocumentViewer();
                    feChild = (FrameworkElement)docViewer;
                    break;
                case "Frame":
                    Frame frame = new Frame();
                    frame.Width = 150;
                    frame.Height = 100;
                    frame.Background = Brushes.Orange;
                    feChild = (FrameworkElement)frame;
                    break;
                case "Grid":
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.RowDefinitions.Add(new RowDefinition());
                    grid.ShowGridLines = true;
                    feChild = (FrameworkElement)grid;
                    break;
                case "HyperLink":
                    Paragraph p = new Paragraph();
                    FlowDocumentScrollViewer tf = new FlowDocumentScrollViewer();
                    tf.Document = new FlowDocument(p);
                    Hyperlink hyperlink = new Hyperlink();
                    hyperlink.Inlines.Clear();
                    hyperlink.Inlines.Add(new Run("HyperLink Test"));
                    p.Inlines.Add(hyperlink);
                    feChild = (FrameworkElement)tf;
                    break;
                case "Image":
                    Image image = new Image();
                    BitmapImage bimage = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
                    image.Source = bimage;
                    feChild = (FrameworkElement)image;
                    break;
                case "Label":
                    Label label = new Label();
                    label.Content = "Label Test";
                    feChild = (FrameworkElement)label;
                    break;
                case "ListBox":
                    ListBox listBox = new ListBox();
                    for (int i = 0; i < 10; i++)
                    {
                        ListBoxItem item = new ListBoxItem();
                        item.Content = "Item " + i.ToString();
                        listBox.Items.Add(item);
                    }
                    feChild = (FrameworkElement)listBox;
                    break;
                case "Menu":
                    Menu menu = new Menu();
                    menu = new Menu();
                    menu.Background = Brushes.LightBlue;
                    MenuItem mi = new MenuItem();
                    mi.Header = "File";
                    menu.Items.Add(mi);
                    MenuItem mia = new MenuItem();
                    mia.Header = "Cut";
                    mia.InputGestureText = "Ctrl+X";
                    mi.Items.Add(mia);
                    MenuItem mib = new MenuItem();
                    mib.Command = System.Windows.Input.ApplicationCommands.Copy;
                    mi.Items.Add(mib);
                    MenuItem mic = new MenuItem();
                    mic.Command = System.Windows.Input.ApplicationCommands.Paste;
                    mi.Items.Add(mic);
                    feChild = (FrameworkElement)menu;
                    break;
                case "Panel":
                    Panel panel = new TestPanel();
                    panel.Background = Brushes.Yellow;
                    Rectangle rectPanel = new Rectangle();
                    rectPanel.Width = 200;
                    rectPanel.Height = 200;
                    rectPanel.Fill = Brushes.Blue;
                    panel.Children.Add(rectPanel);
                    feChild = (FrameworkElement)panel;
                    break;
                case "PasswordBox":
                    PasswordBox passwordBox = new PasswordBox();
                    feChild = (FrameworkElement)passwordBox;
                    break;
                case "RadioButton":
                    ListBox radioBtnList = new ListBox();
                    radioBtnList.Resources.Add(typeof(ListBoxItem), CreateRadioButtonListStyle());
                    radioBtnList.BorderBrush = Brushes.Transparent;
                    KeyboardNavigation.SetDirectionalNavigation(radioBtnList, KeyboardNavigationMode.Cycle);

                    for (int i = 0; i < 5; i++)
                    {
                        ListBoxItem radioBtn = new ListBoxItem();
                        radioBtn.Content = "RadioButton " + i.ToString();
                        radioBtnList.Items.Add(radioBtn);
                        if (i == 2) radioBtn.IsSelected = true;
                    }
                    feChild = (FrameworkElement)radioBtnList;
                    break;
                case "RichTextBox":
                    RichTextBox richTextBox = new RichTextBox();
                    richTextBox.AppendText("This is the RichTextBox...");
                    richTextBox.FontSize = 20;
                    richTextBox.Width = 150;
                    feChild = (FrameworkElement)richTextBox;
                    break;
                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.Background = Brushes.LightSeaGreen;
                    sv.Width = 200;
                    Paragraph para = new Paragraph();
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    textflow.Document = new FlowDocument(para);
                    para.Inlines.Clear();
                    para.Inlines.Add(new Run("Lorem ipsum dolor sit amet, consectetuer adipiscing elit, sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat. Ut wisi enim ad minim veniam, quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat. Duis autem vel eum iriure."));
                    sv.Content = textflow;
                    feChild = (FrameworkElement)sv;
                    break;
                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    for (int i = 0; i < 5; i++)
                    {
                        Button stackBtn = new Button();
                        stackBtn.Content = "Button " + i.ToString();
                        stack.Children.Add(stackBtn);
                    }
                    feChild = (FrameworkElement)stack;
                    break;
                case "TabControl":
                    TabControl tabControl = new TabControl();
                    for (int i = 0; i < 5; i++)
                    {
                        TabItem tabItem = new TabItem();
                        tabItem.Header = "Tab Item " + i.ToString();
                        StackPanel stackpanel = new StackPanel();
                        Button button = new Button();
                        button.Content = "Button in StackPanel for TabItem " + i.ToString();
                        stackpanel.Children.Add(button);
                        tabItem.Content = stackpanel;
                        tabControl.Items.Add(tabItem);
                    }
                    feChild = (FrameworkElement)tabControl;
                    break;
                case "TextBlock":
                    TextBlock textblock = new TextBlock();
                    //textblock.ContentEnd.InsertTextInRun("This is the text in the TextBlock");
                    textblock.Text = "This is the text in the TextBlock";
                    textblock.Background = Brushes.LightSkyBlue;
                    feChild = (FrameworkElement)textblock;
                    break;
                case "TextBox":
                    TextBox textbox = new TextBox();
                    textbox.Text = "This is the text box...";
                    feChild = (FrameworkElement)textbox;
                    break;
                case "ToolBar":
                    ToolBar toolbar = new ToolBar();
                    for (int i = 0; i < 3; i++)
                    {
                        Button toolbutton = new Button();
                        toolbutton.Content = "Button " + i.ToString();
                        toolbar.Items.Add(toolbutton);
                    }
                    feChild = (FrameworkElement)toolbar;
                    break;
                case "ViewBox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Width = 200;
                    viewbox.Height = 200;
                    TextBlock viewboxText = new TextBlock();
                    viewboxText.Text = "This is the text in the TextBlock";
                    viewboxText.Background = Brushes.LightSkyBlue;
                    viewbox.Child = viewboxText;
                    feChild = (FrameworkElement)viewbox;
                    break;
                default:
                    break;
            }
            return feChild;
        }

        /// <summary>
        /// Return Style for ListBoxItem that will have similar
        /// functionality as RadioButton. RadioButtonList is gone
        /// and ListBox with styling is used as a replacement
        /// </summary>
        /// <returns></returns>
        public static Style CreateRadioButtonListStyle()
        {

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(RadioButton), "RadioButton");
            radioButton.SetValue(FrameworkElement.FocusableProperty, false);
            radioButton.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));

            Binding isChecked = new Binding("IsSelected");
            isChecked.Mode = BindingMode.TwoWay;
            isChecked.RelativeSource = RelativeSource.TemplatedParent;
            radioButton.SetBinding(RadioButton.IsCheckedProperty, isChecked);

            Style radioButtonListStyle = new Style(typeof(ListBoxItem));
            ControlTemplate template = new ControlTemplate(typeof(ListBoxItem));
            template.VisualTree = radioButton;
            radioButtonListStyle.Setters.Add(new Setter(Control.TemplateProperty, template));

            return radioButtonListStyle;
        }

        public class PlotPanel : Panel
        {
            public PlotPanel()
                : base()
            {
            }
            protected override Size MeasureOverride(Size availableSize)
            {
                Size childSize = availableSize;
                foreach (UIElement child in InternalChildren)
                {
                    if (child == null) { continue; }
                    child.Measure(childSize);
                    if (double.IsInfinity(childSize.Width))
                        childSize.Width = child.DesiredSize.Width;
                    if (double.IsInfinity(childSize.Height))
                        childSize.Height = child.DesiredSize.Height;

                }
                return childSize;
            }
            protected override Size ArrangeOverride(Size finalSize)
            {
                foreach (UIElement child in InternalChildren)
                {
                    if (child == null) { continue; }
                    double x = 50;
                    double y = 50;
                    child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
                }
                return finalSize;
            }
        }

        public static FrameworkElement AlignmentTestContent(string testStr)
        {
            //Auto, 200px, *, 2*, Auto
            GridLength[] gl = { new GridLength(1, GridUnitType.Auto), 
                new GridLength(250, GridUnitType.Pixel), new GridLength(1, GridUnitType.Star), 
                new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Auto) };
            Grid grid = GridCommon.CreateGrid(5, 5, gl, gl);
            Border border = new Border();
            border.Background = Brushes.Orange;
            border.Width = 100;
            border.Height = 100;
            switch (testStr)
            {
                /* ==== 
                 * Specify cell in Grid or define the size of Border
                 * Set LayoutTransform 
                 * Set other property set (Alignment, Margin, Padding)
                 * ===
                 
                    case "~~~":
                        //Auto, 200px, *, 2*, Auto    
                        Grid.SetColumn(border, 1);
                        Grid.SetRow(border, 1);
                    break;
                */
                //====================================================================
                //  Alignment + Rotate
                //====================================================================
                case "HARotate1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new RotateTransform(45);
                    break;
                case "HARotate2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new RotateTransform(45);
                    break;
                case "HARotate3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new RotateTransform(50);
                    break;
                case "HARotate4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(-45);
                    break;
                case "VARotate1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new RotateTransform(350);
                    break;
                case "VARotate2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new RotateTransform(35);
                    break;
                case "VARotate3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new RotateTransform(750);
                    break;
                case "VARotate4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(80);
                    break;
                case "VARotate5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(60);
                    break;
                //====================================================================
                //  Alignment + Scale
                //====================================================================
                case "HAScale1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "HAScale2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "HAScale3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "HAScale4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VAScale1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "VAScale2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VAScale3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "VAScale4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VAScale5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                //====================================================================
                //  Alignment + Skew(AngleX, AngleY)
                //====================================================================
                case "HASkew1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;
                case "HASkew2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new SkewTransform(-20, -20);
                    break;
                case "HASkew3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new SkewTransform(10, 10);
                    break;
                case "HASkew4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(30, 30);
                    break;
                case "VASkew1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new SkewTransform(-10, -10);
                    break;
                case "VASkew2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new SkewTransform(10, 10);
                    break;
                case "VASkew3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;
                case "VASkew4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(-15, -15);
                    break;
                case "VASkew5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;
                //====================================================================
                //  Alignment with Big Element
                //====================================================================

                case "HABRotate1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new RotateTransform(45);
                    break;
                case "HABRotate2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new RotateTransform(45);
                    break;
                case "HABRotate3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new RotateTransform(50);
                    break;
                case "HABRotate4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(-45);
                    break;
                case "VABRotate1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new RotateTransform(350);
                    break;
                case "VABRotate2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new RotateTransform(35);
                    break;
                case "VABRotate3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new RotateTransform(750);
                    break;
                case "VABRotate4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(80);
                    break;
                case "VABRotate5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new RotateTransform(60);
                    break;
                //====================================================================
                //  Alignment + Scale
                //====================================================================
                case "HABScale1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "HABScale2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "HABScale3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "HABScale4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VABScale1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "VABScale2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VABScale3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                case "VABScale4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(1.5, 1.5);
                    break;
                case "VABScale5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new ScaleTransform(2, 2);
                    break;
                //====================================================================
                //  Alignment + Skew(AngleX, AngleY)
                //====================================================================
                case "HABSkew1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Left;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;
                case "HABSkew2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Right;
                    border.LayoutTransform = new SkewTransform(-20, -20);
                    break;
                case "HABSkew3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.LayoutTransform = new SkewTransform(10, 10);
                    break;
                case "HABSkew4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(30, 30);
                    break;
                case "VABSkew1":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Top;
                    border.LayoutTransform = new SkewTransform(-10, -10);
                    break;
                case "VABSkew2":
                    GridCommon.PlacingChild(grid, border, 2, 2);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.LayoutTransform = new SkewTransform(10, 10);
                    break;
                case "VABSkew3":
                    GridCommon.PlacingChild(grid, border, 3, 3);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Bottom;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;
                case "VABSkew4":
                    GridCommon.PlacingChild(grid, border, 4, 4);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(-15, -15);
                    break;
                case "VABSkew5":
                    GridCommon.PlacingChild(grid, border, 1, 1);
                    border.Width = 300;
                    border.Height = 300;
                    border.VerticalAlignment = VerticalAlignment.Stretch;
                    border.LayoutTransform = new SkewTransform(20, 20);
                    break;

                default:
                    break;
            }

            grid.Children.Add(border);

            return grid;
        }
    }

    /// <summary>
    /// Property Test Helpers.
    /// </summary>
    public class PropertyTestHelpers
    {
        public static FrameworkElement AddTestContent(string _TestElement)
        {
            FrameworkElement child = null;
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    child = scrollviewer;
                    break;
            }
            return child;
        }

        public static void doublePropertyTest(FrameworkElement child, bool TestResult, string failing_test, DependencyProperty dp, string testType, double dpValue)
        {
            switch (dp.Name)
            {
                case "Margin":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(FrameworkElement.MarginProperty, new Thickness(dpValue));
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;

                        case "Infinity":
                            child.SetValue(FrameworkElement.MarginProperty, new Thickness(dpValue));
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "LargeValue":
                            child.SetValue(FrameworkElement.MarginProperty, new Thickness(dpValue));
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(FrameworkElement.MarginProperty, new Thickness(dpValue));
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(FrameworkElement.MarginProperty, new Thickness(dpValue));
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "Height":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "Width":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "MinHeight":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "MinWidth":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "MaxHeight":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;

                case "MaxWidth":
                    switch (testType)
                    {
                        case "Negative":
                            try
                            {
                                child.SetValue(dp, dpValue);
                            }
                            catch (ArgumentException ex)
                            {
                                PropertyTestHelpers.ValidateDoubleExcetpion(dp, dpValue, ex.Message, testType, failing_test, TestResult);
                            }
                            break;
                        case "Infinity":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                        case "LargeValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "SmallValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;

                        case "MidValue":
                            child.SetValue(dp, dpValue);
                            CommonFunctionality.FlushDispatcher();
                            PropertyTestHelpers.ValidateDoubleValue(child, dp, dpValue, testType, failing_test, TestResult);
                            break;
                    }
                    break;
            }
        }

        public static void propertyTest(FrameworkElement child, string failingtest, bool TestResult, DependencyProperty dp, string testType, object dpValue)
        {
            //            switch (dp.Name)
            //            {
            //                case "FlowDirection":
            switch (testType)
            {
                case "InValid":
                    try
                    {
                        child.SetValue(dp, dpValue);
                        ValidateValue(dp, dpValue, testType, child, failingtest, TestResult);
                    }
                    catch (ArgumentException ex)
                    {
                        ValidateExcetpion(dp, dpValue, ex.Message, testType, failingtest, TestResult);
                    }
                    break;
                case "Valid":
                    child.SetValue(dp, dpValue);
                    CommonFunctionality.FlushDispatcher();
                    ValidateValue(dp, dpValue, testType, child, failingtest, TestResult);
                    break;
            }
            //                    break;
            //            }
        }

        // These static variables are safe to run in multiple app domains b/c they are meant to never change.
        static double _MaxHeight = double.PositiveInfinity;
        static double _MinHeight = 0;
        static double _MaxWidth = double.PositiveInfinity;
        static double _MinWidth = 0;
        static double _Width = 100;
        static double _Height = 100;
        static double _Thickness = 0;
        static double _CornerRadius = 0;
        static FlowDirection _FlowDirection = FlowDirection.LeftToRight;
        static Visibility _Visibility = Visibility.Visible;
        static HorizontalAlignment _HorizontalAlignment = HorizontalAlignment.Stretch;
        static VerticalAlignment _VerticalAlignment = VerticalAlignment.Stretch;
        static bool _ClipToBounds = false;
        static Transform _Transform = null;
        static Geometry _Geometry = null;
        static SolidColorBrush _BorderBrush = Brushes.Transparent;
        static bool _LastChildFill = true;
        static Orientation _Orientation = Orientation.Vertical;
        static StretchDirection _StretchDirection = StretchDirection.Both;
        static Stretch _Stretch = Stretch.Uniform;

        public static void restoreValue(FrameworkElement child, DependencyProperty dp)
        {
            switch (dp.Name)
            {
                case "Margin":
                    child.SetValue(dp, new Thickness(_Thickness));
                    break;

                case "Height":
                    child.SetValue(dp, _Height);
                    break;

                case "Width":
                    child.SetValue(dp, _Width);
                    break;

                case "MinHeight":
                    child.SetValue(dp, _MinHeight);
                    break;

                case "MinWidth":
                    child.SetValue(dp, _MinWidth);
                    break;

                case "MaxHeight":
                    child.SetValue(dp, _MaxHeight);
                    break;

                case "MaxWidth":
                    child.SetValue(dp, _MaxWidth);
                    break;

                case "FlowDirection":
                    child.SetValue(dp, _FlowDirection);
                    break;

                case "Visibility":
                    child.SetValue(dp, _Visibility);
                    break;

                case "HorizontalAlignment":
                    child.SetValue(dp, _HorizontalAlignment);
                    break;

                case "VerticalAlignment":
                    child.SetValue(dp, _VerticalAlignment);
                    break;

                case "ClipToBounds":
                    child.SetValue(dp, _ClipToBounds);
                    break;

                case "LayoutTransform":
                    child.SetValue(dp, _Transform);
                    break;

                case "RenderTransform":
                    child.SetValue(dp, _Transform);
                    break;

                case "Clip":
                    child.SetValue(dp, _Geometry);
                    break;

                case "BorderThickness":
                    child.SetValue(dp, new Thickness(_Thickness));
                    break;

                case "CornerRadius":
                    child.SetValue(dp, new CornerRadius(_CornerRadius));
                    break;

                case "BorderBrush":
                    child.SetValue(dp, _BorderBrush);
                    break;
                case "Padding":
                    child.SetValue(dp, new Thickness(_Thickness));
                    break;

                case "LastChildFill":
                    child.SetValue(dp, _LastChildFill);
                    break;

                case "Orientation":
                    child.SetValue(dp, _Orientation);
                    break;

                case "Stretch":
                    child.SetValue(dp, _Stretch);
                    break;

                case "StretchDirection":
                    child.SetValue(dp, _StretchDirection);
                    break;
            }
        }

        public static void ValidateValue(DependencyProperty dp, object dpValue, string testType, FrameworkElement child, string failingTest, bool TestResult)
        {
            object value = child.GetValue(dp);

            if (!Object.Equals(value, dpValue))
            {
                GlobalLog.LogStatus("Wrong value for " + testType + ".");
                GlobalLog.LogStatus("Expected : " + dpValue);
                GlobalLog.LogStatus("Actual : " + value);
                GlobalLog.LogStatus("Test Failed.");
                failingTest += " " + dp + " failed with " + dpValue + " as value.\n";
                TestResult = false;
                GlobalLog.LogStatus("------------------------------");
            }
            //else
            //{
            //    GlobalLog.LogStatus("Verified correct value for " + testType + " " + dp + " test.");
            //    GlobalLog.LogStatus("Expected : " + dpValue);
            //    GlobalLog.LogStatus("Actual : " + value);
            //    GlobalLog.LogStatus("Test Passed.");
            //    GlobalLog.LogStatus("------------------------------");
            //}
        }

        public static void ValidateExcetpion(DependencyProperty dp, object dpValue, string exceptionMessage, string testType, string failingTest, bool testResult)
        {
            if (!Exceptions.CompareMessage(exceptionMessage, "InvalidPropertyValue", WpfBinaries.PresentationFramework))
            {
                testResult = false;
            }
        }

        public static void ValidateDoubleValue(FrameworkElement child, DependencyProperty dp, double dpValue, string testType, string failingTest, bool TestResult)
        {
            double value;
            if (dp.Name != "Margin")
            {
                value = (double)child.GetValue(dp);
            }
            else
            {
                Thickness values = (Thickness)child.GetValue(FrameworkElement.MarginProperty);
                value = values.Left;
            }

            if (value != dpValue)
            {
                GlobalLog.LogStatus("Wrong value for " + testType + ".");
                GlobalLog.LogStatus("Expected : " + dpValue);
                GlobalLog.LogStatus("Actual : " + value);
                GlobalLog.LogStatus("Test Failed.");
                failingTest += " " + dp + " failed with " + dpValue + " as value.\n";
                TestResult = false;
                GlobalLog.LogStatus("------------------------------");
            }
        }

        public static void ValidateDoubleExcetpion(DependencyProperty dp, double dpValue, string exceptionMessage, string testType, string failingTest, bool testResult)
        {
            if (!Exceptions.CompareMessage(exceptionMessage, "InvalidPropertyValue", WpfBinaries.WindowsBase))
            {
                testResult = false;
            }
        }

        public static void ScenarioTests(string failingTest, bool TestResult, int childCount, FrameworkElement root, FrameworkElement child)
        {
            Scenarios(failingTest, TestResult, childCount, root, child, "AllConsumingMargins");
            CommonFunctionality.FlushDispatcher();
            Scenarios(failingTest, TestResult, childCount, root, child, "ChildAddRemove");
            CommonFunctionality.FlushDispatcher();
        }

        public static void Scenarios(string failingTest, bool TestResult, int childCount, FrameworkElement _Parent, FrameworkElement _Child, string _TestType)
        {
            switch (_TestType)
            {
                case "AllConsumingMargins":
                    double heightValue = _Parent.ActualHeight;
                    double widthValue = _Parent.ActualWidth;
                    _Child.Height = heightValue;
                    _Child.Width = widthValue;
                    _Child.Margin = new Thickness((widthValue / 2), (heightValue / 2), (widthValue / 2), (heightValue / 2));
                    CommonFunctionality.FlushDispatcher();
                    ValidateScenarios(failingTest, TestResult, _Parent, _Child, _TestType, 0);
                    break;

                case "ChildAddRemove":
                    if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.Decorator" || _Child.GetType().ToString() == "System.Windows.Controls.Decorator")
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            ((Decorator)_Child).Child = TestChildToAdd();
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildAdd", 0);
                            ((Decorator)_Child).Child = null;
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildRemove", 0);
                        }
                    }
                    else if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.ContentControl")
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            ((ContentControl)_Child).Content = TestChildToAdd();
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildAdd", 0);
                            ((ContentControl)_Child).Content = null;
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildRemove", 0);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < childCount; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                ((Panel)_Child).Children.Add(TestChildToAdd());
                            }
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildAdd", i);

                            ((Panel)_Child).Children.Clear();
                            ValidateScenarios(failingTest, TestResult, _Parent, _Child, "ChildRemove", 0);
                        }
                    }
                    break;
            }
        }

        public static void ValidateScenarios(string failingTest, bool TestResult, FrameworkElement _Parent, FrameworkElement _Child, string _TestType, int _ChildCount)
        {
            switch (_TestType)
            {
                case "AllConsumingMargins":
                    GlobalLog.LogStatus("Boundary Scenario : " + _TestType);
                    //margins are == parent width and height, leaving no room for panel.

                    if (_Child.GetType().Name != "Canvas")
                    {

                        Size zeroSize = new Size(0, 0);

                        Rect childBounds;

                        if (_Child.GetType().Name != "Decorator")
                        {
                            childBounds = VisualTreeHelper.GetClip(_Child).Bounds;
                        }
                        else if (_Child.GetType().Name != "Veiwbox")
                        {
                            childBounds = VisualTreeHelper.GetClip(_Child).Bounds;
                        }
                        else
                        {
                            childBounds = Rect.Intersect(VisualTreeHelper.GetContentBounds(_Child), VisualTreeHelper.GetClip(_Child).Bounds);
                        }


                        if (childBounds.Height != zeroSize.Height || childBounds.Width != zeroSize.Width)
                        {
                            GlobalLog.LogStatus(_TestType + ": Zero Size Check was not sucessfull.");
                            GlobalLog.LogStatus("Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("No Size Check Passed.");
                            if (_Child.Margin.Left + _Child.Margin.Right != _Parent.ActualWidth || _Child.Margin.Top + _Child.Margin.Bottom != _Parent.ActualHeight)
                            {
                                GlobalLog.LogStatus(_TestType + ": Margin Size Check was not sucessfull.");
                                GlobalLog.LogStatus("Test Failed.");
                                failingTest += " " + _TestType + " failed.";
                                TestResult = false;
                                GlobalLog.LogStatus("------------------------------");
                            }
                            else
                            {
                                GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                                GlobalLog.LogStatus("Test Passed.");
                                GlobalLog.LogStatus("------------------------------");
                            }
                        }
                    }
                    else
                    {
                        GlobalLog.LogStatus("Child is either Canvas or Decorator. \nCanvas will not be contrained by Margins, \nand Decorator does not expose a ContentRect. \nTest is not valid.");
                    }
                    break;

                case "ChildAdd":
                    if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.Decorator" || _Child.GetType().ToString() == "System.Windows.Controls.Decorator")
                    {
                        if (((Decorator)_Child).Child == null)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    else if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.ContentControl")
                    {
                        if (((ContentControl)_Child).Content == null)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    else
                    {
                        if (((Panel)_Child).Children.Count != _ChildCount)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.(" + _ChildCount + " Children)");
                            failingTest += " " + _TestType + " failed. (" + _ChildCount + " Children)";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.(" + _ChildCount + " Children)");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    break;

                case "ChildRemove":
                    if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.Decorator" || _Child.GetType().ToString() == "System.Windows.Controls.Decorator")
                    {
                        if (((Decorator)_Child).Child != null)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    else if (_Child.GetType().BaseType.ToString() == "System.Windows.Controls.ContentControl")
                    {
                        if (((ContentControl)_Child).Content != null)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    else
                    {
                        if (((Panel)_Child).Children.Count != 0)
                        {
                            GlobalLog.LogStatus(_TestType + "Test Failed.");
                            failingTest += " " + _TestType + " failed.";
                            TestResult = false;
                            GlobalLog.LogStatus("------------------------------");
                        }
                        else
                        {
                            GlobalLog.LogStatus("Verified correct value for " + _TestType + " test.");
                            GlobalLog.LogStatus("Test Passed.");
                            GlobalLog.LogStatus("------------------------------");
                        }
                    }
                    break;

            }
        }

        public static void PanelSpecificCases(FrameworkElement child, string failingtest, bool TestResult)
        {
            switch (child.GetType().Name)
            {
                case "Border":
                    propertyTest(child, failingtest, TestResult, Border.BorderThicknessProperty, "InValid", new Thickness(-1));
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.BorderThicknessProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.BorderThicknessProperty, "Valid", new Thickness(25));
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.BorderThicknessProperty);
                    CommonFunctionality.FlushDispatcher();

                    try
                    {
                        child.SetValue(Border.CornerRadiusProperty, new CornerRadius(-1));
                        ValidateValue(Border.CornerRadiusProperty, new CornerRadius(-1), "InValid", child, failingtest, TestResult);
                    }
                    catch (ArgumentException ex)
                    {
                        ValidateExcetpion(Border.CornerRadiusProperty, new CornerRadius(-1), ex.Message, "InValid", failingtest, TestResult);
                    }
                    PropertyTestHelpers.restoreValue(child, Border.CornerRadiusProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.CornerRadiusProperty, "Valid", new CornerRadius(25));
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.CornerRadiusProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.BorderBrushProperty, "InValid", FlowDirection.RightToLeft);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.BorderBrushProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.BorderBrushProperty, "Valid", Brushes.SpringGreen);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.BorderBrushProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.PaddingProperty, "InValid", new Thickness(-1));
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.PaddingProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Border.PaddingProperty, "Valid", new Thickness(25));
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Border.PaddingProperty);
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "DockPanel":
                    propertyTest(child, failingtest, TestResult, DockPanel.LastChildFillProperty, "InValid", Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, DockPanel.LastChildFillProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, DockPanel.LastChildFillProperty, "Valid", false);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, DockPanel.LastChildFillProperty);
                    CommonFunctionality.FlushDispatcher();

                    Rectangle dockChild = TestChildToAdd();
                    ((DockPanel)child).Children.Add(dockChild);

                    propertyOnChildTest(failingtest, TestResult, DockPanel.DockProperty, "Valid", Dock.Left, dockChild);
                    CommonFunctionality.FlushDispatcher();
                    propertyOnChildTest(failingtest, TestResult, DockPanel.DockProperty, "InValid", Visibility.Collapsed, dockChild);
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Grid":
                    propertyTest(child, failingtest, TestResult, Grid.ShowGridLinesProperty, "InValid", Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Grid.ShowGridLinesProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Grid.ShowGridLinesProperty, "Valid", true);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Grid.ShowGridLinesProperty);
                    CommonFunctionality.FlushDispatcher();

                    int rowCount = 12569;
                    int colCount = 12569;

                    for (int i = 0; i < rowCount; i++)
                    {
                        RowDefinition row;
                        ((Grid)child).RowDefinitions.Add(row = new RowDefinition());
                    }

                    CommonFunctionality.FlushDispatcher();

                    if (((Grid)child).RowDefinitions.Count != rowCount)
                    {
                        GlobalLog.LogStatus("Row Definition Count Was Inncorrect");
                        GlobalLog.LogStatus("Test Failed.");
                        failingtest += " RowDefinition Test Failed adding " + rowCount + " Rows \n";
                        TestResult = false;
                        GlobalLog.LogStatus("------------------------------");
                    }
                    else
                    {
                        GlobalLog.LogStatus("Row Definition Count Was Correct.");
                        GlobalLog.LogStatus("Test Passed.");
                        GlobalLog.LogStatus("------------------------------");
                    }

                    CommonFunctionality.FlushDispatcher();

                    ((Grid)child).RowDefinitions.Clear();

                    CommonFunctionality.FlushDispatcher();

                    if (((Grid)child).RowDefinitions.Count != 0)
                    {
                        GlobalLog.LogStatus("Row Definition Count Was Inncorrect");
                        GlobalLog.LogStatus("Test Failed.");
                        failingtest += " RowDefinition Test Failed Clearing " + rowCount + " Rows \n";
                        TestResult = false;
                        GlobalLog.LogStatus("------------------------------");
                    }
                    else
                    {
                        GlobalLog.LogStatus("Row Definition Count Was Correct after Clearing.");
                        GlobalLog.LogStatus("Test Passed.");
                        GlobalLog.LogStatus("------------------------------");
                    }

                    CommonFunctionality.FlushDispatcher();

                    for (int i = 0; i < colCount; i++)
                    {
                        ColumnDefinition col;
                        ((Grid)child).ColumnDefinitions.Add(col = new ColumnDefinition());
                    }

                    CommonFunctionality.FlushDispatcher();

                    if (((Grid)child).ColumnDefinitions.Count != colCount)
                    {
                        GlobalLog.LogStatus("Column Definition Count Was Inncorrect");
                        GlobalLog.LogStatus("Test Failed.");
                        failingtest += " ColumnDefinition Test Failed adding " + colCount + " Rows \n";
                        TestResult = false;
                        GlobalLog.LogStatus("------------------------------");
                    }
                    else
                    {
                        GlobalLog.LogStatus("Column Definition Count Was Correct.");
                        GlobalLog.LogStatus("Test Passed.");
                        GlobalLog.LogStatus("------------------------------");
                    }

                    CommonFunctionality.FlushDispatcher();

                    ((Grid)child).ColumnDefinitions.Clear();

                    CommonFunctionality.FlushDispatcher();

                    if (((Grid)child).ColumnDefinitions.Count != 0)
                    {
                        GlobalLog.LogStatus("Column Definition Count Was Inncorrect");
                        GlobalLog.LogStatus("Test Failed.");
                        failingtest += " ColumnDefinition Test Failed Clearing " + rowCount + " Rows \n";
                        TestResult = false;
                        GlobalLog.LogStatus("------------------------------");
                    }
                    else
                    {
                        GlobalLog.LogStatus("Column Definition Count Was Correct after Clearing.");
                        GlobalLog.LogStatus("Test Passed.");
                        GlobalLog.LogStatus("------------------------------");
                    }

                    break;

                case "Canvas":
                    Rectangle canvasChild = TestChildToAdd();
                    ((Canvas)child).Children.Add(canvasChild);

                    propertyOnChildTest(failingtest, TestResult, Canvas.BottomProperty, "Valid", 4949d, canvasChild);
                    CommonFunctionality.FlushDispatcher();
                    propertyOnChildTest(failingtest, TestResult, Canvas.BottomProperty, "InValid", Dock.Bottom, canvasChild);
                    CommonFunctionality.FlushDispatcher();

                    propertyOnChildTest(failingtest, TestResult, Canvas.TopProperty, "Valid", 256d, canvasChild);
                    CommonFunctionality.FlushDispatcher();
                    propertyOnChildTest(failingtest, TestResult, Canvas.TopProperty, "InValid", Brushes.CadetBlue, canvasChild);
                    CommonFunctionality.FlushDispatcher();

                    propertyOnChildTest(failingtest, TestResult, Canvas.LeftProperty, "Valid", 1082d, canvasChild);
                    CommonFunctionality.FlushDispatcher();
                    propertyOnChildTest(failingtest, TestResult, Canvas.LeftProperty, "InValid", FlowDirection.RightToLeft, canvasChild);
                    CommonFunctionality.FlushDispatcher();

                    propertyOnChildTest(failingtest, TestResult, Canvas.RightProperty, "Valid", 9999d, canvasChild);
                    CommonFunctionality.FlushDispatcher();
                    propertyOnChildTest(failingtest, TestResult, Canvas.RightProperty, "InValid", Visibility.Visible, canvasChild);
                    CommonFunctionality.FlushDispatcher();

                    break;

                case "StackPanel":
                    propertyTest(child, failingtest, TestResult, StackPanel.OrientationProperty, "InValid", Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, StackPanel.OrientationProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, StackPanel.OrientationProperty, "Valid", Orientation.Horizontal);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, StackPanel.OrientationProperty);
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "Viewbox":
                    propertyTest(child, failingtest, TestResult, Viewbox.StretchDirectionProperty, "InValid", Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Viewbox.StretchDirectionProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Viewbox.StretchDirectionProperty, "Valid", StretchDirection.DownOnly);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Viewbox.StretchDirectionProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Viewbox.StretchProperty, "InValid", Visibility.Hidden);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Viewbox.StretchProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, Viewbox.StretchProperty, "Valid", Stretch.Uniform);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, Viewbox.StretchProperty);
                    CommonFunctionality.FlushDispatcher();
                    break;

                case "ScrollViewer":
                    propertyTest(child, failingtest, TestResult, ScrollViewer.HorizontalScrollBarVisibilityProperty, "InValid", Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, ScrollViewer.HorizontalScrollBarVisibilityProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, ScrollViewer.HorizontalScrollBarVisibilityProperty, "Valid", ScrollBarVisibility.Auto);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, ScrollViewer.HorizontalScrollBarVisibilityProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, ScrollViewer.VerticalScrollBarVisibilityProperty, "InValid", StretchDirection.DownOnly);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, ScrollViewer.VerticalScrollBarVisibilityProperty);
                    CommonFunctionality.FlushDispatcher();

                    propertyTest(child, failingtest, TestResult, ScrollViewer.VerticalScrollBarVisibilityProperty, "Valid", ScrollBarVisibility.Disabled);
                    CommonFunctionality.FlushDispatcher();
                    PropertyTestHelpers.restoreValue(child, ScrollViewer.VerticalScrollBarVisibilityProperty);
                    CommonFunctionality.FlushDispatcher();

                    break;
            }
        }

        public static void propertyOnChildTest(string failingtest, bool TestResult, DependencyProperty dp, string testType, object dpValue, FrameworkElement _TestChild)
        {
            switch (testType)
            {
                case "InValid":
                    try
                    {
                        _TestChild.SetValue(dp, dpValue);
                        ValidateValueOnChild(failingtest, TestResult, dp, dpValue, testType, _TestChild);
                    }
                    catch (ArgumentException ex)
                    {
                        ValidateExcetpion(dp, dpValue, ex.Message, testType, failingtest, TestResult);
                    }
                    break;
                case "Valid":
                    _TestChild.SetValue(dp, dpValue);
                    CommonFunctionality.FlushDispatcher();
                    ValidateValueOnChild(failingtest, TestResult, dp, dpValue, testType, _TestChild);
                    break;
            }
        }

        public static void ValidateValueOnChild(string failingTest, bool TestResult, DependencyProperty dp, object dpValue, string testType, FrameworkElement _TestChild)
        {
            object value = _TestChild.GetValue(dp);

            if (!Object.Equals(value, dpValue))
            {
                GlobalLog.LogStatus("Wrong value for " + testType + ".");
                GlobalLog.LogStatus("Expected : " + dpValue);
                GlobalLog.LogStatus("Actual : " + value);
                GlobalLog.LogStatus("Test Failed.");
                failingTest += " " + dp + " failed with " + dpValue + " as value.\n";
                TestResult = false;
                GlobalLog.LogStatus("------------------------------");
            }
            //else
            //{
            //    GlobalLog.LogStatus("Verified correct value for " + testType + " " + dp + " test.");
            //    GlobalLog.LogStatus("Expected : " + dpValue);
            //    GlobalLog.LogStatus("Actual : " + value);
            //    GlobalLog.LogStatus("Test Passed.");
            //    GlobalLog.LogStatus("------------------------------");
            //}
        }

        public static Transform TestTransform()
        {
            RotateTransform rt = new RotateTransform();
            rt.Angle = 45;
            return rt;
        }

        public static Geometry TestClipGeometry()
        {
            EllipseGeometry eg = new EllipseGeometry();
            eg.RadiusX = 50;
            eg.RadiusY = 75;
            return eg;
        }

        public static Rectangle TestChildToAdd()
        {
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, Brushes.Gray);
            return rect;
        }

    }

    #endregion
}
