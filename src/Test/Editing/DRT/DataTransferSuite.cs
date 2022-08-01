// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Interop;
using System.Text;
using MS.Win32;
using System.Runtime.InteropServices;

namespace DRT
{
    internal class DataTransferSuite : DrtTestSuite
    {
        // Constructor.
        internal DataTransferSuite() : base("DataTransfer")
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            CreateWindow();

            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest( DataObjectTest ),
                new DrtTest( DelayRenderTest ),
                new DrtTest( TextDragDrop ),
                new DrtTest( ButtonDragDrop ),
                new DrtTest( ElementDragDrop ),
                new DrtTest( CheckDragDropResult ),
                new DrtTest( CloseWindow ),
                new DrtTest( TestClipboard),
                new DrtTest( CreateCopyPasteWindow),
                new DrtTest( ClickCopySource),
                new DrtTest( CopyText),
                new DrtTest( ClickPasteTarget),
                new DrtTest( PasteText),
                new DrtTest( VerifyTextCopyPaste),
                new DrtTest( CloseWindow),
            };
        }

        private void CreateWindow()
        {
            PresentationSource source;

            // Create widnow.
            _win = new Window();
            _win.Width = 800;
            _win.Height = 600;
            _win.Title = _windowText;

            // Create the root canvas.
            _rootCanvas = new Canvas();
            _rootCanvas.Width = _win.Width;
            _rootCanvas.Height = _win.Height;
            _rootCanvas.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            _rootCanvas.Name = "rootCanvas";
            _rootCanvas.Focusable = true;
            _win.Content = _rootCanvas;

            // Create the first child canvas.
            _childCanvas1 = new Canvas();
            _childCanvas1.Width = _win.Width / 2;
            _childCanvas1.Height = _win.Height / 3;
            _childCanvas1.Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            _childCanvas1.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childCanvas1);
            _childCanvas1.Name = "_childCanvas1";
            Canvas.SetLeft(_childCanvas1, 0);
            Canvas.SetTop(_childCanvas1, 0);

            // Create the second child canvas.
            _childCanvas2 = new Canvas();
            _childCanvas2.Width = _win.Width;
            _childCanvas2.Height = _win.Height / 3;
            _childCanvas2.Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xff, 0xf0));
            _childCanvas2.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childCanvas2);
            _childCanvas2.Name = "_childCanvas2";
            Canvas.SetLeft(_childCanvas2, _win.Width / 2);
            Canvas.SetTop(_childCanvas2, 0);

            // Create the child stackpanel.
            _childStackPanel = new StackPanel();
            _childStackPanel.Orientation = Orientation.Horizontal;
            _childStackPanel.Width = _win.Width;
            _childStackPanel.Height = _win.Height / 3;
            _childStackPanel.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xf0, 0xf0));
            _childStackPanel.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childStackPanel);
            Canvas.SetLeft(_childStackPanel, 0);
            Canvas.SetTop(_childStackPanel, _win.Height / 3);

            // Create the first child dockpanel.
            _childDockPanel = new DockPanel();
            _childDockPanel.Width = _win.Width;
            _childDockPanel.Height = _win.Height / 3;
            _childDockPanel.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xf0));
            _childDockPanel.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childDockPanel);
            Canvas.SetLeft(_childDockPanel, 0);
            Canvas.SetTop(_childDockPanel, (_win.Height / 3) * 2);

            // Create the drag source textbox.
            _textboxSource = new TextBox();
            _textboxSource.Width = _win.Width / 2 - 20;
            _textboxSource.Height = _win.Height / 6;
            ((IAddChild)_childCanvas1).AddChild(_textboxSource);
            Canvas.SetLeft(_textboxSource, 10);
            Canvas.SetTop(_textboxSource, 10);

            // Create the drop target textbox.
            _textboxTarget = new TextBox();
            _textboxTarget.Width = _win.Width / 2 - 20;
            _textboxTarget.Height = _win.Height / 6;
            ((IAddChild)_childCanvas2).AddChild(_textboxTarget);
            Canvas.SetLeft(_textboxTarget, 10);
            Canvas.SetTop(_textboxTarget, 10);

            // Create the buttons for the first child canvas.
            _button1 = new System.Windows.Controls.Button();
            _button1.Content = "Button1";
            ((IAddChild)_childCanvas1).AddChild(_button1);
            Canvas.SetLeft(_button1, 20);
            Canvas.SetTop(_button1, 150);
            _button2 = new System.Windows.Controls.Button();
            _button2.Content = "Button2";
            ((IAddChild)_childCanvas1).AddChild(_button2);
            Canvas.SetLeft(_button2, 120);
            Canvas.SetTop(_button2, 150);
            _button3 = new System.Windows.Controls.Button();
            _button3.Content = "Button3";
            ((IAddChild)_childCanvas1).AddChild(_button3);
            Canvas.SetLeft(_button3, 220);
            Canvas.SetTop(_button3, 150);
            _button4 = new System.Windows.Controls.Button();
            _button4.Content = "Button4";
            ((IAddChild)_childCanvas1).AddChild(_button4);
            Canvas.SetLeft(_button4, 320);
            Canvas.SetTop(_button4, 150);

            // Create the dragable canvas.
            Canvas dragCanvas = new Canvas();

            dragCanvas.Width = 100;
            dragCanvas.Height = 25;
            dragCanvas.Background = new SolidColorBrush(Color.FromRgb(0x00, 0xff, 0xf0));
            dragCanvas.Focusable = true;
            ((IAddChild)_childCanvas1).AddChild(dragCanvas);
            Canvas.SetLeft(dragCanvas, 20);
            Canvas.SetTop(dragCanvas, 120);

            // Create the boxElement for the second child canvas.
            _el4[0] = new BoxElement("c2B1", 50, 150, 50, 30);
            _el4[1] = new BoxElement("c2B2", 120, 150, 50, 30);
            _el4[2] = new BoxElement("c2B3", 200, 150, 50, 30);
            ((IAddChild)_childCanvas2).AddChild(_el4[0]);
            ((IAddChild)_childCanvas2).AddChild(_el4[1]);
            ((IAddChild)_childCanvas2).AddChild(_el4[2]);

            // Create the boxElement for the child stackpanel.
            _el2[0] = new BoxElement("F1B1", 50, 30);
            _el2[1] = new BoxElement("F1B2", 50, 30);
            _el2[2] = new BoxElement("F1B3", 50, 30);
            _el2[3] = new BoxElement("F1B4", 50, 30);
            ((IAddChild)_childStackPanel).AddChild(_el2[0]);
            ((IAddChild)_childStackPanel).AddChild(_el2[1]);
            ((IAddChild)_childStackPanel).AddChild(_el2[2]);
            ((IAddChild)_childStackPanel).AddChild(_el2[3]);

            // Create the boxElement for the child dockpanel.
            _el3[0] = new BoxElement("D1B1", 30);
            _el3[1] = new BoxElement("D1B2", 30);
            _el3[2] = new BoxElement("D1B3", 30);
            _el3[3] = new BoxElement("D1B4", 30);
            ((IAddChild)_childDockPanel).AddChild(_el3[0]);
            ((IAddChild)_childDockPanel).AddChild(_el3[1]);
            ((IAddChild)_childDockPanel).AddChild(_el3[2]);
            ((IAddChild)_childDockPanel).AddChild(_el3[3]);
            DockPanel.SetDock(_el3[0], Dock.Top);
            DockPanel.SetDock(_el3[1], Dock.Top);
            DockPanel.SetDock(_el3[2], Dock.Top);
            DockPanel.SetDock(_el3[3], Dock.Top);

            // Set the AllowDrop property.
            _childCanvas1.AllowDrop = true;
            _textboxSource.AllowDrop = false;
            _childCanvas2.AllowDrop = true;
            _childStackPanel.AllowDrop = true;
            _childDockPanel.AllowDrop = true;
            _button1.AllowDrop = false;
            _button2.AllowDrop = false;
            _button3.AllowDrop = false;
            _button4.AllowDrop = false;

            _win.Closed += new EventHandler(OnClosed);
            _win.Show();

            // Checking the drop target elements.
            if (!_textboxTarget.AllowDrop)
            {
                Console.WriteLine("FAILED! TextTarget doesn't allow the drop.");
                throw (new ApplicationException());
            }

            if (!_childCanvas2.AllowDrop)
            {
                Console.WriteLine("FAILED! The child canvas doesn't allow the drop.");
                throw (new ApplicationException());
            }

            // Register the drop target window.
            source = PresentationSource.FromVisual(_rootCanvas);

            _win32Window = source as IWin32Window;
            if (_win32Window == null || _win32Window.Handle == IntPtr.Zero)
            {
                Console.WriteLine("FAILED! Application window handle is null.");
                throw (new ApplicationException());
            }

            // Create the selection array list.
            _selectionList = new ArrayList();

            // Add mouse event handler.
            _rootCanvas.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseDown), false);
            _rootCanvas.AddHandler(Mouse.MouseMoveEvent, new MouseEventHandler(OnMouseMove), false);
            _rootCanvas.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseUp), false);

            // Add DragSource and DropTarget event handlers.
            _rootCanvas.AddHandler(DragDrop.QueryContinueDragEvent, new QueryContinueDragEventHandler(MyOnQueryContinueDrag1));
            _rootCanvas.AddHandler(DragDrop.GiveFeedbackEvent, new GiveFeedbackEventHandler(MyOnGiveFeedback1));
            _rootCanvas.AddHandler(DragDrop.DragEnterEvent, new DragEventHandler(MyOnDragEnter));
            _rootCanvas.AddHandler(DragDrop.DragOverEvent, new DragEventHandler(MyOnDragOver));
            _rootCanvas.AddHandler(DragDrop.DragLeaveEvent, new DragEventHandler(MyOnDragLeave));
            _rootCanvas.AddHandler(DragDrop.DropEvent, new DragEventHandler(MyOnDrop));

            _textboxSource.AddHandler(DragDrop.PreviewQueryContinueDragEvent, new QueryContinueDragEventHandler(OnTextSourcePreviewQueryContinueDrag));

            _textboxTarget.AddHandler(DragDrop.PreviewDropEvent, new DragEventHandler(OnTextTargetPreviewDrop));

            // Initialize rcScreen.
            _rcScreen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        }

        private void DataObjectTest()
        {
            // 1. Test empty data object.
            DataObject dataObj = new DataObject();
            String[] formats = dataObj.GetFormats();

            DRT.Assert((formats != null && formats.Length == 0), "Empty data object should not have any available formats.");

            // 2. Test text data object.
            // Creates a new data object using a string and the text format.
            string myString = "My new text string";
            string dataString;
            DataObject myDataObject = new DataObject(DataFormats.Text, myString);

            // Test the GetData() with/without autoconvert
            DRT.Assert(myDataObject.GetData("System.String", false) == null, "GetData System.String is failed without autoconvert");
            DRT.Assert(myDataObject.GetData("System.String", true) != null, "GetData System.String is failed with autoconvert");

            dataString = myDataObject.GetData("System.String", true).ToString();

            //Verify the what we get back is what we put up there
            DRT.Assert(dataString == myString, "Returned String does not match");

            // Creates a new data object using a string and the text format.
            myDataObject = new DataObject(DataFormats.Text, "Another string");

            // Test the GetDataPresent with the autoconvert == false.
            DRT.Assert(myDataObject.GetDataPresent("System.String", false) == false, "GetDataPresent System.String is failed without autoconvert");

            // Test the GetDataPresent with the autoconvert == true
            DRT.Assert(myDataObject.GetDataPresent("System.String", true), "GetDataPresent System.String is failed with autoconvert");

            // 3. Test the bitmap data object.
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(@"DrtFiles\Editing\cut.png");
            object data;

            // Test bitmap data.
            dataObj.SetData(DataFormats.Bitmap, bitmap);
            formats = dataObj.GetFormats();

            if (DRT.Verbose)
                Console.WriteLine("Available formats:");

            foreach(string format in formats)
            {
                data = dataObj.GetData(format);

                if (DRT.Verbose)
                    Console.WriteLine("GetData("+format+") returned "+((data == null)?"null":data));
            }

            DRT.Assert(formats != null, "Formats is null for setting of Bitmap data object");
        }

        private void DelayRenderTest()
        {
            DataObject dataObj;
            object data = null;

            // Test SampleData (autoconvert)
            dataObj = new DataObject();
            Foo fooData = new Foo();
            dataObj.SetData("fooData", fooData);

            if (DRT.Verbose)
                Console.WriteLine("Available formats:");

            string[] formats = dataObj.GetFormats();
            foreach(string format in formats)
            {
                data = dataObj.GetData(format);

                if (DRT.Verbose)
                    Console.WriteLine("GetData("+format+") returned "+((data == null)?"null":data));
            }

            DRT.Assert(data != null, "GetData is failed for getting the delay render data object");

            if (data == fooData && DRT.Verbose)
            {
                Console.WriteLine("GetData(fooData) returned " + ((Foo) dataObj.GetData("fooData")).foo);
            }
        }


        [TypeConverter(typeof(ServiceConverter))]
        internal interface IService
        {
        }

        internal interface IFooService : IService
        {
        }

        internal class ServiceConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }

                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    string fullName = ((string)value).Trim();
                    return new Foo();
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == null)
                    throw new ArgumentNullException("destinationType");

                if (destinationType == typeof(string))
                {
                    return value.GetType().Name;
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        [Serializable()]
            internal class Foo : ISerializable, IFooService
        {
            public string foo = "foovalue";
            public Foo()
            {
                foo = "bar";
            }

            //Deserialization constructor.
            public Foo(SerializationInfo info, StreamingContext context)
            {
                foo = (String)info.GetValue("FooSerialized", typeof(string));
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("FooSerialized", foo);
            }
        }

        private void TextDragDrop()
        {
            Point mousePoint;

            _textboxSource.Focus();
            _textboxSource.SelectedText = _dragString;

            // Down the mouse on the drag source selection text and move/up to the drop target text box.
            mousePoint = new Point(40, 25);

            // Move the mouse point to the selected text and then down the mouse.
            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);

            // Move the mouse for starting DragDrop operation and release the mouse to complete DragDrop.
            mousePoint = new Point(_win.Width / 2 + 40, 25);
            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
        }

        private void ButtonDragDrop()
        {
            Point mousePoint;

            // Move the mouse to _button1.
            mousePoint = new Point(40, 160);

            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);

            // Dragging the mouse to the _childCanvas2 area.
            mousePoint = new Point(_win.Width / 2 + 20, 120);
            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
        }

        private void ElementDragDrop()
        {
            Point mousePoint;

            // Move the mouse to _childCanvasl.
            mousePoint = new Point(10, 190);

            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);

            // Dragging the mouse to the _childStackPanel area.
            mousePoint = new Point(_win.Width / 2, _win.Height / 3 + 50);

            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX + 10, _normalizedY + 10, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
        }

        private void CheckDragDropResult()
        {
            Canvas parent;
            StackPanel parent2;

//            // Removed text dragdrop result checking by the mouse operation sending issue.
//            // The issue is that Avalon input send mouse left button down, up and move instead of left button
//            // down, move and up.
//            DRT.Assert(textSelection != null && textSelection.Text == _dragString,
//                "Text DragDrop is failed! drag source string and drop text string doesn't match." +
//                "\r\nSource: " + _textboxSource.Text +
//                "\r\nTarget: " + _textboxTarget.Text +
//                "\r\nTarget selection: " + textSelection.Text +
//                "\r\nPreviewQueryContinueDrag: " + _textPreviewQueryContinueDrag +
//                "\r\nPreviewDrop: " + _textPreviewDrop);

            if (DRT.Verbose)
            {
                Console.WriteLine("Text DragDrop Result." +
                "\r\nSource: " + _textboxSource.Text +
                "\r\nTarget: " + _textboxTarget.Text +
                "\r\nTarget selection: " + _textboxTarget.SelectedText +
                "\r\nPreviewQueryContinueDrag: " + _textPreviewQueryContinueDrag +
                "\r\nPreviewDrop: " + _textPreviewDrop);
            }

            // Get _button1 element's parent and compare it with the target parent(_childCanvas2).
            parent = VisualTreeHelper.GetParent(_button1) as Canvas;

            DRT.Assert(parent == null || parent != _childCanvas2,
                "_button1 DragDrop is failed!" +
                "\r\nParent: " + parent +
                "\r\nQueryContinueDrag: " + _queryContinueDrag1 +
                "\r\nGiveFeedback: " + _giveFeedback1 +
                "\r\nDragEnter: " + _dragEnter +
                "\r\nDrop: " + _drop);

            DRT.Assert(_queryContinueDrag1 && _giveFeedback1 && _dragEnter && _drop,
                "_button1 DragDrop is failed to get DragSource/DropTarget events!" +
                "\r\nQueryContinueDrag: " + _queryContinueDrag1 +
                "\r\nGiveFeedback: " + _giveFeedback1 +
                "\r\nDragEnter: " + _dragEnter +
                "\r\nDrop: " + _drop);

            // Get _childCanvas element's parent and compare it with the target parent(_childStackPanel).
            parent2 = VisualTreeHelper.GetParent(_childCanvas1) as StackPanel;

            DRT.Assert(parent2 == null || parent2 != _childStackPanel, "_childCanvas2 DragDrop is failed!" + "\r\nParent: " + parent2);
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Window win;

            win = sender as Window;

            if (win == null)
            {
                throw new ApplicationException("sender in Window.Closed Handler is not a Window");
            }
        }

        private void CloseWindow()
        {
            Thread.Sleep(1000);
            _win.Close();
        }

        private void TestClipboard()
        {
            string textString = "This is DRT Clipboard text string.";

            DataObject dataObjectOrg = new DataObject(textString);
            Clipboard.SetDataObject(dataObjectOrg);

            DRT.Assert(Clipboard.IsCurrent(dataObjectOrg), "dataObject that is placed by SetDataObject() doesn't exist on the clipboard.");

            IDataObject dataObject = Clipboard.GetDataObject();

            DRT.Assert(dataObject != null, "GetDataObject is failed or dataObject doesn't exist.");
            DRT.Assert(dataObject.GetDataPresent(DataFormats.Text, true), "GetDataPresent is failed");

            string dataString = dataObject.GetData("System.String", true).ToString();

            DRT.Assert(textString == dataString, "Get the incorrected data string");

            Clipboard.Clear();
            DRT.Assert(!Clipboard.IsCurrent(dataObjectOrg), "Clipboard isn't empty, dataObject exists on the clipboard.");

            if (DRT.Verbose)
            {
                Console.WriteLine("Got all correct data from clipboard. (Passed)");
            }
        }


        private void ClickCopySource()
        {
            // Down the mouse on the source to make sure the focus is on the source.
            Point mousePoint = new Point(20, 10);
            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
        }

        private void CopyText()
        {
            // Set the focus on the source text box.
            _textboxCopyPasteSource.Focus();
            _textboxCopyPasteSource.Focus();

            // Select all text and objects of the source text box.
            _textboxCopyPasteSource.SelectAll();

            // Copy all text and embedded objects from the source text box.
            KeyboardType("^C");
        }

        private void ClickPasteTarget()
        {
            // Down the mouse on the paste target to make sure the focus is on the target.
            Point mousePoint = new Point(20, _win.Height / 3 + 10);
            GetNormalizedPoint(mousePoint);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_MOVE | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, IntPtr.Zero);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
            UnsafeNativeMethods.Mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP | NativeMethods.MOUSEEVENTF_ABSOLUTE, _normalizedX, _normalizedY, 0, (IntPtr)0);
        }

        private void PasteText()
        {
            // Set the focus on the source text box.
            _textboxCopyPasteTarget.Focus();

            // Select all content as we intend to replace it all
            _textboxCopyPasteTarget.SelectAll();

            // Paste all text and embedded objects from the source text box to the target text box.
            KeyboardType("^V");
        }

        private void VerifyTextCopyPaste()
        {
            DRT.Assert(_textboxCopyPasteSource.Text == _textboxCopyPasteTarget.Text,
                "CopyPaste failed: Source: \"" + _textboxCopyPasteSource.Text + " != Target: \"" + _textboxCopyPasteTarget.Text + "\"");
        }


        private void GetNormalizedPoint(Point mousePoint)
        {
            NativeMethods.POINT point;
            int x, y;
            double normalizedX1;
            double normalizedY1;
            double normalizedX2;
            double normalizedY2;

            point = new NativeMethods.POINT(0, 0);

            UnsafeNativeMethods.ClientToScreen(new HandleRef(null, _win32Window.Handle), point);

            x = (int)point.x;
            y = (int)point.y;

            normalizedX1 = (65535.0 * (mousePoint.X + point.x)) / ((double)_rcScreen.Width);
            normalizedY1 = (65535.0 * (mousePoint.Y + point.y)) / ((double)_rcScreen.Height);
            normalizedX2 = (65535.0 * (mousePoint.X + point.x + 1)) / ((double)_rcScreen.Width);
            normalizedY2 = (65535.0 * (mousePoint.Y + point.y + 1)) / ((double)_rcScreen.Height);

            _normalizedX = (int)(normalizedX1 + ((normalizedX2 - normalizedX1) / 2.0));
            _normalizedY = (int)(normalizedY1 + ((normalizedY2 - normalizedY1) / 2.0));
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement target;
            ModifierKeys modKeys;
            bool fShift;

            // Get the visual real target.
            target = e.Source as FrameworkElement;

            if (target == null || target is TextBox || (target is TextBlock && target.Focusable))
            {
                return;
            }

            // Mouse input doesn't indicate the right target element. This is a workaround code.
            if (!target.Focusable)
            {
                FrameworkElement newTarget;

                newTarget = VisualTreeHelper.GetParent(target) as FrameworkElement;

                while (newTarget != null && !newTarget.Focusable)
                {
                    newTarget = VisualTreeHelper.GetParent(newTarget) as FrameworkElement;
                }

                if (newTarget != null)
                {
                    target = newTarget;
                }
            }

            modKeys = InputManager.Current.PrimaryKeyboardDevice.Modifiers;
            fShift = ((modKeys & ModifierKeys.Shift) != 0);

            // Reset the current selected components.
            if (!fShift)
            {
                _selectionList.Clear();
            }

            // Add the component into the selection list and adorner layer.
            if (target is BoxElement || target is Button || target is Panel)
            {
                if (!_selectionList.Contains(target))
                {
                    _selectionList.Add(target);
                }
            }

            _dragSource = target;

            _mouseDownPoint = e.GetPosition((IInputElement)target);

            e.Handled = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragSource != null)
            {
                DoDragDropComponents(_dragSource as FrameworkElement);

                _dragSource = null;
                e.Handled = true;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragSource != null)
            {
                _dragSource = null;
            }
        }

        private void DoDragDropComponents(FrameworkElement dragSource)
        {
            System.Windows.IDataObject dataObject;
            DragDropEffects allowedDropEffect;
            DragDropEffects dropEffects;

            // Get the data object for drag and drop opertaion.
            dataObject = GetDataObject(dragSource);

            // Set the default drop effects with Copy and Move.
            allowedDropEffect = DragDropEffects.Move | DragDropEffects.Copy;

            dropEffects = DragDrop.DoDragDrop(dragSource, dataObject, allowedDropEffect);

            DragEnd(dropEffects);
        }

        private void MyOnQueryContinueDrag1(object sender, QueryContinueDragEventArgs e)
        {
            bool mouseUp;

            mouseUp = false;

            e.Action = DragAction.Continue;
            mouseUp = (((int)e.KeyStates & (int)DragDropKeyStates.LeftMouseButton) == 0);
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
            }
            else if (mouseUp)
            {
                e.Action = DragAction.Drop;
            }

            e.Handled = true;

            _queryContinueDrag1 = true;
        }

        private void MyOnGiveFeedback1(object sender, GiveFeedbackEventArgs e)
        {
            // Show the default DragDrop cursor.
            e.UseDefaultCursors = true;

            e.Handled = true;

            _giveFeedback1 = true;
        }

        private void MyOnDragEnter(object sender, DragEventArgs e)
        {
            bool ctrlKeyDown;

            // If there's no supported data available, don't allow the drag-and-drop.
            if (e.Data == null)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            // Ok, there's data to move or copy here.
            if ((e.AllowedEffects & DragDropEffects.Move) != 0)
                e.Effects = DragDropEffects.Move;

            ctrlKeyDown = ((int)(e.KeyStates & DragDropKeyStates.ControlKey) != 0);
            if (ctrlKeyDown)
            {
                e.Effects |= DragDropEffects.Copy;
            }

            e.Handled = true;

            _dragEnter = true;
        }

        private void MyOnDragOver(object sender, DragEventArgs e)
        {
            bool ctrlKeyDown;

            // If there's no supported data available, don't allow the drag-and-drop.
            if (e.Data == null)
            {
                return;
            }

            // Ok, there's data to move or copy here.
            if ((e.AllowedEffects & DragDropEffects.Move) != 0)
            {
                e.Effects = DragDropEffects.Move;
            }

            ctrlKeyDown = ((int)(e.KeyStates & DragDropKeyStates.ControlKey) != 0);
            if (ctrlKeyDown)
            {
                e.Effects |= DragDropEffects.Copy;
            }

            e.Handled = true;
        }

        private void MyOnDragLeave(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void MyOnDrop(object sender, DragEventArgs e)
        {
            DependencyObject target;
            string xml;
            FrameworkElement element = null;
            string targetXML;

            target =(DependencyObject) e.Source;

            if (target == null || e.Data == null || e.AllowedEffects == DragDropEffects.None)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            if ((int)(e.KeyStates & DragDropKeyStates.ControlKey) != 0)
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Effects != DragDropEffects.Copy)
            {
                e.Effects = DragDropEffects.Move;
            }

            // Get the XML data from the data object.
            xml = e.Data.GetData(DataFormats.Xaml) as string;
  
            if (xml != null && xml != "")
            {
                element = System.Windows.Markup.XamlReader.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(xml))) as FrameworkElement;
            }

            targetXML = System.Windows.Markup.XamlWriter.Save(target);


            string currentXML = System.Windows.Markup.XamlWriter.Save(element);

            // The element shouldn't be the target element.
            if (targetXML != currentXML)
            {
                if (target is Panel)
                {
                    Panel targetPanel;

                    targetPanel = target as Panel;
                    ((IAddChild)targetPanel).AddChild(element);
                }

                _dropTarget = target;
            }

            e.Handled = true;

            _drop = true;
        }


        private void OnTextSourcePreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            _textPreviewQueryContinueDrag = true;
        }

        private void OnTextTargetPreviewDrop(object sender, DragEventArgs e)
        {
            _textPreviewDrop = true;
        }

        private System.Windows.IDataObject GetDataObject(FrameworkElement dragSource)
        {
            System.Windows.IDataObject dataObject;
            ArrayList selectedComponentsArrayList;

            // Create the data object for drag and drop.
            dataObject = null;

            // Initialize the drag data components.
            _dragDataComponents = null;

            // Adding the current drag source object if there isn't the selected component.
            selectedComponentsArrayList = (ArrayList) _selectionList.Clone();

            if (selectedComponentsArrayList != null && selectedComponentsArrayList.Count > 0)
            {
                FrameworkElement[] selectedComponents;
                StringBuilder xmlData;
                string xml;

                // We need to cache the selected elements so that we can delete them if the
                // result of the drag drop operation was a move.
                selectedComponents = (FrameworkElement[])selectedComponentsArrayList.ToArray(typeof(FrameworkElement));

                // Serialize the selected elements and add it to the data object.
                xmlData = new StringBuilder();

                for (int i = 0; i < selectedComponents.Length; i++)
                {
                    xml = System.Windows.Markup.XamlWriter.Save(selectedComponents[i]);

                    xmlData.Append(xml);
                }

                // Set the data object as XML format.
                dataObject = new System.Windows.DataObject();
                dataObject.SetData(DataFormats.Xaml, xmlData.ToString());

                // Set the original drag selected components.
                _dragDataComponents = selectedComponents;
            }

            return dataObject;
        }

        private void DragEnd(DragDropEffects dropEffects)
        {
            if (dropEffects == DragDropEffects.Move)
            {
                // Remove the drag source object after drop.
                if (_dragDataComponents is FrameworkElement[])
                {
                    FrameworkElement[] elements;

                    elements = (FrameworkElement[])_dragDataComponents;

                    foreach (FrameworkElement elem in elements)
                    {
                        if (elem != null && elem != (FrameworkElement)_dropTarget)
                        {
                            FrameworkElement parent;

                            parent = elem.Parent as FrameworkElement;

                            if (parent != null)
                            {
                                if (parent is Panel)
                                {
                                    Panel parentSource;

                                    parentSource = parent as Panel;

                                    parentSource.Children.Remove(elem);
                                    parentSource.InvalidateMeasure();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateCopyPasteWindow()
        {
            // Create widnow.
            _win = new Window();
            _win.Width = 800;
            _win.Height = 600;
            _win.Title = _windowText;

            // Create the root canvas.
            _rootCanvas = new Canvas();
            _rootCanvas.Width = _win.Width;
            _rootCanvas.Height = _win.Height;
            _rootCanvas.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
            _rootCanvas.Name = "rootCanvas";
            _rootCanvas.Focusable = true;
            _win.Content = _rootCanvas;

            // Create the first child canvas.
            _childCanvas1 = new Canvas();
            _childCanvas1.Width = _win.Width;
            _childCanvas1.Height = _win.Height / 3;
            _childCanvas1.Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0));
            _childCanvas1.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childCanvas1);
            _childCanvas1.Name = "_childCanvas1";
            Canvas.SetLeft(_childCanvas1, 0);
            Canvas.SetTop(_childCanvas1, 0);

            // Create the second child canvas.
            _childCanvas2 = new Canvas();
            _childCanvas2.Width = _win.Width;
            _childCanvas2.Height = _win.Height / 3;
            _childCanvas2.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xf0, 0xf0));
            _childCanvas2.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childCanvas2);
            _childCanvas2.Name = "_childCanvas2";
            Canvas.SetLeft(_childCanvas2, 0);
            Canvas.SetTop(_childCanvas2, _win.Height / 3);

            // Create the child stackpanel.
            _childStackPanel = new StackPanel();
            _childStackPanel.Orientation = Orientation.Horizontal;
            _childStackPanel.Width = _win.Width;
            _childStackPanel.Height = _win.Height / 3;
            _childStackPanel.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xf0));
            _childStackPanel.Focusable = true;
            ((IAddChild)_rootCanvas).AddChild(_childStackPanel);
            Canvas.SetLeft(_childStackPanel, 0);
            Canvas.SetTop(_childStackPanel, (_win.Height / 3) * 2);

            // Create the copy/paste source textbox.
            _textboxCopyPasteSource = new MyTextBox();
            _textboxCopyPasteSource.Width = _win.Width - 30;
            _textboxCopyPasteSource.Height = _win.Height / 6;
            ((IAddChild)_childCanvas1).AddChild(_textboxCopyPasteSource);
            Canvas.SetLeft(_textboxCopyPasteSource, 10);
            Canvas.SetTop(_textboxCopyPasteSource, 10);

            // Create the copy/paste target textbox.
            _textboxCopyPasteTarget = new MyTextBox();
            _textboxCopyPasteTarget.Width = _win.Width - 30;
            _textboxCopyPasteTarget.Height = _win.Height / 6;
            ((IAddChild)_childCanvas2).AddChild(_textboxCopyPasteTarget);
            Canvas.SetLeft(_textboxCopyPasteTarget, 10);
            Canvas.SetTop(_textboxCopyPasteTarget, 10);

            TextRange range = new TextRange(_textboxCopyPasteTarget.Document.ContentStart, _textboxCopyPasteTarget.Document.ContentEnd);
            string t = range.Text;

            // Create the boxElement for the child dockpanel.
            _win.Closed += new EventHandler(OnClosed);
            _win.Show();

            // Get the window.
            PresentationSource source = PresentationSource.FromVisual(_rootCanvas);
            _win32Window = source as IWin32Window;

            DRT.Assert(_win32Window != null && _win32Window.Handle != IntPtr.Zero);

            // Initialize rcScreen.
            _rcScreen = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            // Add the text string and embedded objects into the source.
            AddTextAndEmbeddedObject();
        }

        private void AddTextAndEmbeddedObject()
        {
            // Append the text on the source textbox.
            _textboxCopyPasteSource.AppendText("Source TextBox...");

            // Create FlowDocumentScrollViewer and add into the source TextBox.
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            fdsv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            fdsv.Document = new FlowDocument(new Paragraph(new Run("TextFlow...")));
            fdsv.Document.TextAlignment = TextAlignment.Left;

            fdsv.Width = 250;
            fdsv.Background = new SolidColorBrush(Color.FromRgb(0xff, 0xf0, 0xf0));
            _textboxCopyPasteSource.AppendObject(fdsv);

            // Create Inline and Add into the FlowDocumentScrollViewer.
            Run _inlineTextPanel = new Run(" InlineOnTextPanel ");

            _inlineTextPanel.FontWeight = FontWeights.Bold;
            _inlineTextPanel.FontStyle = FontStyles.Italic;
            ((Paragraph)fdsv.Document.Blocks.LastBlock).Inlines.Add(_inlineTextPanel);

            // Create Button and Add into the FlowDocumentScrollViewer.
            Button buttonTextPanel = new Button();
            buttonTextPanel.Content = "Button";
            ((Paragraph)fdsv.Document.Blocks.LastBlock).Inlines.Add(new InlineUIContainer(buttonTextPanel));

            // Add Inline on the source textbox.
            _inline = new Run(" Inline ");
            _inline.FontWeight = FontWeights.Bold;
            _textboxCopyPasteSource.AppendInline(_inline);

            // Add CustomInlineElement on the source textbox.
            CustomInlineElement customInlineElement = new CustomInlineElement(" CustomInlineElement ");
            _textboxCopyPasteSource.AppendInline(customInlineElement);

            // Add the embedded objects(StackPanel with Button) on the source textbox.
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Background = new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xff));
            stackPanel.Width = 100;
            stackPanel.Height = 50;
            _textboxCopyPasteSource.AppendObject(stackPanel);

            Button button = new Button();
            button.Content = "Button Test";
            ((IAddChild)stackPanel).AddChild(button);

            // Add normal Inline on the source textbox.
            _inline = new Run(" InlineText2 ");
            _textboxCopyPasteSource.AppendInline(_inline);

            // Add image on the source so that image copy/paste will be operated
            Image image = new Image();
            image.Source = (ImageSource)BitmapFrame.Create(new Uri(@".\DrtFiles\Editing\cut.png", UriKind.RelativeOrAbsolute)); ;
            image.Width = 100;
            image.Height = 100;
            InlineUIContainer inlineUIContainer = new InlineUIContainer(image);
            _textboxCopyPasteSource.AppendInline(inlineUIContainer);
        }

        private void KeyboardType(string text)
        {
            DrtInput.KeyboardType(text);
        }

        private Window _win;
        private string _windowText = "Drt Window";
        private Canvas _rootCanvas;
        private Canvas _childCanvas1;
        private Canvas _childCanvas2;
        private StackPanel _childStackPanel;
        private DockPanel _childDockPanel;
        private TextBox _textboxSource;
        private TextBox _textboxTarget;
        private BoxElement[] _el1 = new BoxElement[4];
        private BoxElement[] _el2 = new BoxElement[4];
        private BoxElement[] _el3 = new BoxElement[4];
        private BoxElement[] _el4 = new BoxElement[4];
        private System.Windows.Controls.Button _button1, _button2, _button3, _button4;

        private ArrayList _selectionList;
        private System.Drawing.Rectangle _rcScreen;
        private int _normalizedX;
        private int _normalizedY;
        private IWin32Window _win32Window;
        private bool _queryContinueDrag1;
        private bool _giveFeedback1;
        private bool _dragEnter;
        private bool _drop;
        private bool _textPreviewQueryContinueDrag;
        private bool _textPreviewDrop;

        private DependencyObject _dragSource;
        private DependencyObject _dropTarget;
        private FrameworkElement[] _dragDataComponents;
        private Point _mouseDownPoint;
        private string _dragString = "This is Avalon DragDrop testing...";

        private MyTextBox _textboxCopyPasteSource;
        private MyTextBox _textboxCopyPasteTarget;

        private Run _inline;

    }

    public class BoxElement : Button
    {
        public BoxElement()
        {
        }

        public BoxElement(string name, int left, int top, int width, int height)
        {
            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            this.Width = width;
            this.Height = height;
            Content = name;
            _name = name;
        }

        public BoxElement(string name, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            Content = name;
            _name = name;
        }

        public BoxElement(string name, int height)
        {
            this.Height = height;
            Content = name;
            _name = name;
        }

        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
        private string _name;

    }

    public class MyTextBox : RichTextBox
    {
        public MyTextBox()
        {
        }

        // Adds an embedded object to the end of text content of a RichTextBox
        public void AppendObject(UIElement embeddedObject)
        {
            ((Paragraph)this.Document.Blocks.LastBlock).Inlines.Add(new InlineUIContainer(embeddedObject));
        }

        public void AppendInline(Inline inline)
        {
            ((Paragraph)this.Document.Blocks.LastBlock).Inlines.Add(inline);
        }

        public string Text
        {
            get
            {
                return new TextRange(this.Document.ContentStart, this.Document.ContentStart).Text;
            }
        }
    }

    public class CustomInlineElement : Span
    {
        public CustomInlineElement()
            : base()
        {
            this.FontStyle = FontStyles.Italic;

            _nameString = "CustomInlineElement";
        }

        public CustomInlineElement(string text)
            : base(new Run(text))
        {
        }

        public String TestName
        {
            get { return _nameString; }
        }

        String _nameString;
    }
}