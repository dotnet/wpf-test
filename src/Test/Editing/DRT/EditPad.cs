// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Reflection; // for resources
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Windows.Markup;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class EditPad
    {
        #region Public Methods

        public Window Window
        {
            get
            {
                return s_window;
            }
        }

        public RichTextBox RichTextBox
        {
            get
            {
                return _richTextBox;
            }
        }

        #endregion Public Properties

        #region Application Window

        public void StartUp()
        {
            Stream stream;  // Streamed resource to load frame XAML.

            // create window
            s_window = new Window();
            s_window.Title = "EditPad";
            s_window.Width = 800;
            s_window.Height = 600;

            // Load EditPadFrame
            stream = Assembly.GetEntryAssembly().GetManifestResourceStream(s_editPadFrameXaml);
            if(stream == null)
            {
                throw new ApplicationException("Unable to find resources for [" + s_editPadFrameXaml + "]");
            }

            s_window.Content = (UIElement)System.Windows.Markup.XamlReader.Load(stream);

            // Find RichTextBox and forward ui commands to it
            _richTextBox = LogicalTreeHelper.FindLogicalNode((DependencyObject)s_window.Content, "RichTextBox") as RichTextBox;
            if(_richTextBox == null)
            {
                throw new ApplicationException("RichTextBox not found in EditPadFrame.xaml");
            }


            // Find XamlViewer and link it to its source - RichTextBox
            _xamlViewer = LogicalTreeHelper.FindLogicalNode((DependencyObject)s_window.Content, "XamlViewer") as XamlViewer;
            if (_xamlViewer != null)
            {
                _xamlViewer.Source = _richTextBox;
            }

            // Set command handlers for debugging buttons
            SetCommandHandler("ShowXaml", new RoutedEventHandler(ShowXamlClick));
            SetCommandHandler("Debug", new RoutedEventHandler(DebugClick));

            // Set big nice font for demo purposes
            // Set some fancy text properties - for demo purposes
            _richTextBox.SetValue(Typography.DiscretionaryLigaturesProperty, true);
            _richTextBox.SetValue(FlowDocument.FontFamilyProperty, new FontFamily("Palatino Linotype"));
            _richTextBox.SetValue(FlowDocument.FontSizeProperty, 10.0);

            _richTextBox.Selection.End.InsertTextInRun("Quickly! Welcome to EditPad - a test application for rich text editing");
            _richTextBox.Selection.Select(_richTextBox.Selection.Start, _richTextBox.Selection.Start);

            s_window.Show();

            // Allow drag the text and objects on the content.
            _richTextBox.AllowDrop = true;
        }

        #endregion Application Window

        #region Custom Commands Helper Methods

        /// <summary>
        /// Sets the handler for a command - on button or on menu item.
        /// </summary>
        /// <param name="id">
        ///    String identifying a button or menu item element in the EditPad frame.
        /// </param>
        /// <param name="handler">
        ///    Command handler for this command.
        /// </param>
        private void SetCommandHandler(string id, RoutedEventHandler handler)
        {
            FrameworkElement commandControl = (FrameworkElement)LogicalTreeHelper.FindLogicalNode((DependencyObject)s_window.Content, id);

            if (commandControl is MenuItem)
            {
                ((MenuItem)commandControl).Click += handler;
            }
            else if (commandControl is Button)
            {
                ((Button)commandControl).Click += handler;
            }
        }

        #endregion Custom Commands Helper Methods

        #region Private Methods

        #region Built-in Commands

        private void ShowXamlClick(object sender, RoutedEventArgs args)
        {
            if (_xamlViewer != null)
            {
                _xamlViewer.RefreshText();
            }
        }

        private void DebugClick(object sender, RoutedEventArgs args)
        {
            #if false
            string xaml = @"<TextRange xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
<!--StartFragment--><Paragraph>text<LineBreak/>text</Paragraph>
<!--EndFragment-->
</TextRange>";

            _richTextBox.Selection.AppendXaml(xaml);
            #endif

            _richTextBox.Selection.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)));
        }

        #endregion Build-it Commands

        #endregion Private Methods

        #region Private Fields

        private const string ApplicationName = "EditPad";
        private const string ApplicationVersion = "2.1";

        private static Window s_window;
        private static string s_editPadFrameXaml = "EditPadFrame.xaml";
        private RichTextBox _richTextBox = null;
        private XamlViewer _xamlViewer = null;

        #endregion Private Fields
    }

    //  XAML Viewer
    public class XamlViewer: TextBox
    {
        //  Constructor
        public XamlViewer(): base()
        {
            InitializeDefaults();
        }


        //  Public Properties
        /// <summary>
        /// A link to some framework element whose content should be viewed
        /// by this XamlViewer.
        /// </summary>
        /// <value></value>
        public RichTextBox Source
        {
            get
            {
                return _source;
            }

            set
            {
                // Set connection to our source
                _source = value;
                RefreshText();
            }
        }

        public void RefreshText()
        {
            // Clean the current content
            Text = "";

            // Build the new viwew
            if (_source != null)
            {
                String xaml;
                String namespaceDefinition;
                int start;
                int end;

                if (_source.Selection.IsEmpty)
                {
                    xaml = DrtEditing.GetTextRangeXml(new TextRange(_source.Document.ContentStart, _source.Document.ContentEnd));
                }
                else
                {
                    xaml = DrtEditing.GetTextRangeXml(_source.Selection);
                }

                // Remove namespace definition
                namespaceDefinition = " xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\"";
                start = xaml.IndexOf(namespaceDefinition);
                xaml = xaml.Substring(0, start) + xaml.Substring(start + namespaceDefinition.Length);

                // Put the text into a viewer
                this.Text = xaml;

                // Highlight selected part
                start = xaml.IndexOf("<!--StartFragment-->");
                end = xaml.IndexOf("<!--EndFragment-->");
                if (start > 0 && end > 0)
                {
                    start += "<!--StartFragment-->".Length;
                }
                else
                {
                    start = 0;
                    end = this.Text.Length;
                }

                this.SelectionStart = start;
                this.SelectionLength = end - start;
                PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
                if (TextSelectionInfo == null)
                {
                    throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
                }
                TextSelection selection = TextSelectionInfo.GetValue(this, null) as TextSelection;
                selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);

                this.SelectionLength = 0;
            }
        }

        //  Private Methods
        private void InitializeDefaults()
        {
            this.Background = Brushes.Tan;
            this.FontFamily = new FontFamily("Courier New");
            this.FontSize = 10.0;
            this.IsReadOnly = true;
            this.TextWrapping = TextWrapping.Wrap;
        }

        //  Private Fields
        private RichTextBox _source;
    }

    //  ParameterizedExecuteEventArgs

    /// <summary>
    /// Command Arguments
    /// </summary>
    public class ParameterizedExecuteEventArgs
    {
        /// <summary>
        /// Argument
        /// </summary>
        public static object Parameter
        {
            get
            {
                return s_parameter;
            }
            set
            {
                s_parameter = value;
            }
        }

        private static object s_parameter;
    }

    //  ComboBoxCommand
    public class ComboBoxCommand: ComboBox
    {
        /// <summary>
        ///     The DependencyProperty for Command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(ComboBoxCommand));

        /// <summary>
        /// Get or set Command property
        /// </summary>
        public RoutedCommand Command
        {
            get { return (RoutedCommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// A virtual function that is called when the selection is changed. Default behavior
        /// is to raise a SelectionChangedEvent
        /// </summary>
        /// <param name="args">The inputs for this event. Can be raised (default behavior) or processed
        ///   in some other way.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            base.OnSelectionChanged(args);

            // Make sure that we invoke an associated command, if specified.
            if (this.Command != null && args.AddedItems.Count == 1)
            {
                ComboBoxCommandItem selectedItem = args.AddedItems[0] as ComboBoxCommandItem;
                if (selectedItem != null)
                {
                    ParameterizedExecuteEventArgs.Parameter = selectedItem.Parameter;
                    this.Command.Execute(null, null);
               }
            }
        }
    }

    public class ComboBoxCommandItem: ComboBoxItem
    {
        public object Parameter
        {
            get
            {
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }

        private object _parameter;
    }

    public class VerticalSplitPanel : Grid, IAddChild
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public VerticalSplitPanel() : base()
        {
            RowDefinition rowDefinition;

            // Row for content
            rowDefinition = new RowDefinition();
            this.RowDefinitions.Add(rowDefinition);

            // Row for divider
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(DividerSize);
            this.RowDefinitions.Add(rowDefinition);

            // Row for ui
            rowDefinition = new RowDefinition();
            rowDefinition.Height = new GridLength(0);
            this.RowDefinitions.Add(rowDefinition);
        }

        #endregion Constructors

        #region Public Methods

        #region IAddChild Interface

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            UIElement uie;
            ScrollViewer scrollViewer;

            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            uie = o as UIElement;

            if (uie == null)
            {
                throw new ArgumentException("o must be UIElement");
            }

            switch (Children.Count)
            {
                case 0:
                    // First child - by defailt it goes into cell [0,0] with streatchable heighg
                    Children.Add(uie);
                    Grid.SetRow(uie, 0);
                    break;
                case 1:
                    // Add divider
                    _divider = new Rectangle();
                    _divider.SetValue(Shape.StrokeProperty, Brushes.Blue);
                    _divider.SetValue(Shape.FillProperty, Brushes.Gray);
                    _divider.SetValue(Rectangle.RadiusXProperty, 4.0);
                    _divider.SetValue(Rectangle.RadiusYProperty, 4.0);
                    _divider.SetValue(CursorProperty, System.Windows.Input.Cursors.SizeNS);
                    _divider.SetValue(StyleProperty, null);

                    // Set mouse event handlers
                    _divider.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                    _divider.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
                    _divider.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);

                    Grid.SetRow(_divider, 1);
                    Children.Add(_divider);

                    // Add bottom subpanel
                    scrollViewer = new ScrollViewer();
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scrollViewer.Content = uie;
                    Grid.SetRow(scrollViewer, 2);
                    Children.Add(scrollViewer);
                    break;
                default:
                    throw new Exception("VerticalSplitPanel can have maximum two children");
            }
        }

        ///<summary>
        /// Called when text appears under the tag in markup.
        ///</summary>
        ///<param name="textData">
        /// Text to Add to the Object
        ///</param>
        void IAddChild.AddText(string textData)
        {
            // Ignore text runs
        }

        #endregion IAddChild Interface

        #endregion Public Methods

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _mouseDownPoint = e.GetPosition(this);
            _initialSize = this.RowDefinitions[2].Height.Value;
            _divider.CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseMovePoint;
            Double delta;
            Double newSize;
            Double maxSize;

            if (!_divider.IsMouseCaptured)
            {
                return;
            }

            e.Handled = true;

            mouseMovePoint = e.GetPosition(this);
            delta = mouseMovePoint.Y - _mouseDownPoint.Y;

            maxSize = this.RenderSize.Height - DividerSize;
            if (maxSize < 0)
            {
                maxSize = 0.0;
            }
            newSize = _initialSize - delta;
            if (newSize > maxSize)
            {
                newSize = maxSize;
            }
            else if (newSize < 0.0)
            {
                newSize = 0.0;
            }
            this.RowDefinitions[2].Height = new GridLength(newSize);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            _divider.ReleaseMouseCapture();
        }

        private Point _mouseDownPoint;
        private Double _initialSize;
        private Rectangle _divider;

        public static readonly double DividerSize = 10.0;
    }

    public class HorizontalSplitPanel : Grid, IAddChild
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public HorizontalSplitPanel() : base()
        {
            ColumnDefinition columnDefinition;

            // Column for content
            columnDefinition = new ColumnDefinition();
            this.ColumnDefinitions.Add(columnDefinition);

            // Column for divider
            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(DividerSize);
            this.ColumnDefinitions.Add(columnDefinition);

            // Column for ui
            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(0);
            this.ColumnDefinitions.Add(columnDefinition);
        }

        #endregion Constructors

        #region Public Methods

        #region IAddChild Interface

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            UIElement uie;
            ScrollViewer scrollViewer;

            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            uie = o as UIElement;

            if (uie == null)
            {
                throw new ArgumentException("o must be UIElement");
            }

            switch (Children.Count)
            {
                case 0:
                    // First child - by defailt it goes into cell [0,0] with streatchable heighg
                    Children.Add(uie);
                    Grid.SetColumn(uie, 0);
                    break;
                case 1:
                    // Add divider
                    _divider = new Rectangle();
                    _divider.SetValue(Shape.StrokeProperty, Brushes.Blue);
                    _divider.SetValue(Shape.FillProperty, Brushes.Gray);
                    _divider.SetValue(Rectangle.RadiusXProperty, 4.0);
                    _divider.SetValue(Rectangle.RadiusYProperty, 4.0);
                    _divider.SetValue(CursorProperty, System.Windows.Input.Cursors.SizeWE);
                    _divider.SetValue(StyleProperty, null);

                    // Set mouse event handlers
                    _divider.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
                    _divider.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove));
                    _divider.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);

                    Grid.SetColumn(_divider, 1);
                    Children.Add(_divider);

                    // Add bottom subpanel
                    scrollViewer = new ScrollViewer();
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    scrollViewer.Content = uie;
                    Grid.SetColumn(scrollViewer, 2);
                    Children.Add(scrollViewer);
                    break;
                default:
                    throw new Exception("HorizontalSplitPanel can have maximum two children");
            }
        }

        ///<summary>
        /// Called when text appears under the tag in markup.
        ///</summary>
        ///<param name="textData">
        /// Text to Add to the Object
        ///</param>
        void IAddChild.AddText(string textData)
        {
            // Ignore text runs
        }

        #endregion IAddChild Interface

        #endregion Public Methods

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            _mouseDownPoint = e.GetPosition(this);
            _initialSize = this.ColumnDefinitions[2].Width.Value;
            _divider.CaptureMouse();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point mouseMovePoint;
            Double delta;
            Double newSize;
            Double maxSize;

            if (!_divider.IsMouseCaptured)
            {
                return;
            }

            e.Handled = true;

            mouseMovePoint = e.GetPosition(this);
            delta = mouseMovePoint.X - _mouseDownPoint.X;

            maxSize = this.RenderSize.Width - DividerSize;
            if (maxSize < 0)
            {
                maxSize = 0.0;
            }
            newSize = _initialSize - delta;
            if (newSize > maxSize)
            {
                newSize = maxSize;
            }
            else if (newSize < 0.0)
            {
                newSize = 0.0;
            }
            this.ColumnDefinitions[2].Width = new GridLength(newSize);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            _divider.ReleaseMouseCapture();
        }

        private Point _mouseDownPoint;
        private Double _initialSize;
        private Rectangle _divider;

        public static readonly double DividerSize = 10.0;
    }

    /// <summary>
    /// Decorator allowing to zoom its content by rolling mouse wheel.
    /// </summary>
    public class ZoomDecorator : Decorator
    {
        #region Constructors

        public ZoomDecorator()
        {
            // Add handler wor mouse wheel - for zooming
            this.AddHandler(Mouse.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnMouseWheel));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Zoom property specifies
        /// </summary>
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.RegisterAttached( //
            "Zoom", typeof(Double), typeof(ZoomDecorator), //
            new FrameworkPropertyMetadata(1.0));

        public Double Zoom
        {
            get
            {
                return (Double)this.GetValue(ZoomProperty);
            }

            set
            {
                this.SetValue(ZoomProperty, value);
            }
        }

        #endregion Public Properties

        #region Private Methods

        // MouseWheelEvent handler - zooming
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                e.Handled = true;

                if (e.Delta > 0)
                {
                    if (Zoom < 20)
                    {
                        Zoom += Zoom * 0.20;
                        this.LayoutTransform = new ScaleTransform(Zoom, Zoom);
                    }
                }
                else
                {
                    if (Zoom > 0.25)
                    {
                        Zoom -= Zoom * 0.20;
                        this.LayoutTransform = new ScaleTransform(Zoom, Zoom);
                    }
                }
            }
        }

        #endregion Private Methods
    }

    /// <summary>
    /// A container for controls providing an update mechanism for contained controls.
    /// </summary>
    public class ToolBar : DockPanel
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ToolBar() : base()
        {
            InitializeToolBar();
        }

        private void InitializeToolBar()
        {
        }

        private void ScheduleNextUpdate()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ToolBarUpdate),
                null);
        }

        private object ToolBarUpdate(object obj)
        {
            ScheduleNextUpdate();
            return null;
        }

        private void OnTick(object state)
        {
            try
            {
                throw new Exception("Stop timer");
            }
            catch (Exception)
            {
                _timer.Change(-1, -1);
                _timer = null;
            }
        }

        #endregion Constructors

        private Timer _timer;
    }
}