// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;

using System.Text;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Runtime.InteropServices;
using System.Windows.Automation;


namespace DRT
{

    public class DrtPopupSuite : DrtTestSuite
    {
        public DrtPopupSuite() : base("Popup")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Border border = new Border();
            border.Background = Brushes.White;

            DRT.Show(border);

            Canvas canvas = new Canvas();
            border.Child = canvas;
            canvas.Width = 400;
            canvas.Height = 300;

            // Test elements

            border = new Border();
            border.SetValue(Canvas.LeftProperty, 150.0);
            border.SetValue(Canvas.TopProperty, 100.0);
            border.Background = Brushes.Yellow;
            border.BorderThickness = new Thickness(1.0);
            border.BorderBrush = Brushes.Orange;
            canvas.Children.Add(border);

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            stackPanel.Width = 150.0;
            stackPanel.Height = 100.0;
            border.Child = stackPanel;
            _placementTarget = stackPanel;

            TextBlock text = new TextBlock();
            text.Text = "Placement Target";
            stackPanel.Children.Add(text);

            _popup = new Popup();
            _popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(CustomPopupPlacement);
            stackPanel.Children.Add(_popup);

            border = new Border();
            border.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
            border.BorderThickness = new Thickness(1.0);
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(190, 0, 0, 0));
            border.Width = 50.0;
            border.Height = 75.0;
            _popup.Child = border;
            DRT.Assert(border.Parent == _popup, "child's logical parent is not the popup");

            bool found = false;
            int numChildren = 0;
            foreach (object currentChild in LogicalTreeHelper.GetChildren(_popup))
            {
                numChildren++;
                if (currentChild == border)
                {
                    found = true;
                }
            }
            DRT.Assert(found, "Popup's logical children did not include the added child");
            DRT.Assert(numChildren == 1, String.Format("Popup has {0} logical children instead of 1", numChildren));

            // UI Elements

            DockPanel dockPanel = new DockPanel();
            dockPanel.SetValue(Canvas.LeftProperty, 10.0);
            dockPanel.SetValue(Canvas.TopProperty, 10.0);
            canvas.Children.Add(dockPanel);

            text = new TextBlock();
            text.SetValue(DockPanel.DockProperty, Dock.Top);
            text.Text = "Placement Mode:";
            dockPanel.Children.Add(text);

            ListBox listBox = new ListBox();
            listBox.SetValue(DockPanel.DockProperty, Dock.Top);
            foreach (PlacementMode mode in Enum.GetValues(typeof(PlacementMode)))
            {
                listBox.Items.Add(mode);
            }
            dockPanel.Children.Add(listBox);

            Binding binding = new Binding("Placement");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = _popup;
            binding.Converter = new PlacementModeConverter();
            listBox.SetBinding(ListBox.SelectedIndexProperty, binding);

            CheckBox checkBox = new CheckBox();
            checkBox.SetValue(DockPanel.DockProperty, Dock.Top);
            checkBox.Content = "Popup.IsOpen";
            dockPanel.Children.Add(checkBox);

            binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = _popup;
            checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);

            _popup.IsOpen = true;

            // Tests

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(SetPlacement),
                    new DrtTest(SetPlacement2),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }


        private void Start()
        {
        }

        public void Cleanup()
        {
            if ((_popup != null) && _popup.IsOpen)
            {
                _popup.IsOpen = false;
            }
        }

        private FrameworkElement _placementTarget;
        private Popup _popup;
        private static double s_EPSILON = 1e-10;

        private CustomPopupPlacement[] CustomPopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[]
                {
                    new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) / 2.0, targetSize.Height), PopupPrimaryAxis.Horizontal),
                    new CustomPopupPlacement(new Point((targetSize.Width - popupSize.Width) / 2.0, -popupSize.Height), PopupPrimaryAxis.Horizontal)
                };
        }

        private void SetPlacement()
        {
            PlacementTest test = s_tests[s_curTest];
            if (test.MousePosition.X != 0 && test.MousePosition.Y != 0)
            {
                Mouse.Capture((IInputElement)DRT.RootElement);
                DRT.MoveMouse(test.MousePosition);
                Mouse.Capture(null);
            }
            _popup.Placement = test.Placement;

            DRT.ResumeAt(new DrtTest(PostSetPlacement));
        }

        private void PostSetPlacement()
        {
            _popup.IsOpen = true;

            DRT.ResumeAt(new DrtTest(Check));
        }

        private Rect GetPopupBounds()
        {
            HwndSource source = PresentationSource.FromVisual(_popup.Child) as HwndSource;
            if (source == null)
            {
                DRT.Assert(false, "Popup's HwndSource was null");
                return Rect.Empty;
            }

            CompositionTarget vt = source.CompositionTarget;
            if (vt == null)
            {
                DRT.Assert(false, "Popup Visual Target was null");
                return Rect.Empty;
            }
            POINT upperLeft = new POINT(0, 0);
            ClientToScreen(source.Handle, upperLeft);

            RECT windowRect = new RECT(0, 0, 0, 0);
            GetClientRect(new HandleRef(null, source.Handle), ref windowRect);

            return new Rect(upperLeft.x, upperLeft.y, windowRect.right, windowRect.bottom);
        }

        private Rect GetFrameworkElementBounds(FrameworkElement fe)
        {
            HwndSource source = PresentationSource.FromVisual(fe) as HwndSource;
            if (source == null)
            {
                DRT.Assert(false, "ISource was null");
                return Rect.Empty;
            }

            CompositionTarget vt = source.CompositionTarget;
            if (vt == null)
            {
                DRT.Assert(false, "Composition Target was null");
                return Rect.Empty;
            }

            GeneralTransform transform = fe.TransformToAncestor(source.RootVisual);
            Point rootOffset;
            transform.TryTransform(new Point(0.0, 0.0), out rootOffset);

            Point targetSize = new Point(fe.RenderSize.Width, fe.RenderSize.Height);

            Point convertedRoot = vt.TransformToDevice.Transform(rootOffset);
            Point convertedSize = vt.TransformToDevice.Transform(targetSize);

            POINT tempUL = new POINT((int)convertedRoot.X, (int)convertedRoot.Y);
            ClientToScreen(source.Handle, tempUL);
            POINT tempSize = new POINT((int)convertedSize.X, (int)convertedSize.Y);

            return new Rect(tempUL.x, tempUL.y, tempSize.x, tempSize.y);
        }

        private void Check()
        {
            // we can't verify as well as robovision, but we can at least test that the placement
            // was mostly correct

            Rect popupBounds = GetPopupBounds();
            Rect contentBounds = GetFrameworkElementBounds(_placementTarget);
            Rect intersection;
            System.Console.WriteLine("PlacementMode: " + _popup.Placement);

            if (_popup.Placement == PlacementMode.Bottom || _popup.Placement == PlacementMode.Right
                || _popup.Placement == PlacementMode.Center || _popup.Placement == PlacementMode.Relative)
            {
                System.Console.WriteLine("Popup: " + popupBounds);
                System.Console.WriteLine("Content: " + contentBounds);
                intersection = Rect.Intersect(popupBounds, contentBounds);
                System.Console.WriteLine("Intersection: " + intersection);
                if (intersection == Rect.Empty)
                {
                    DRT.Assert(false, "Popup was not over the element");
                }
                if (_popup.Placement == PlacementMode.Bottom)
                {
                    if (intersection.Height > s_EPSILON)
                    {
                        DRT.Assert(false, "Bottom: intersection.Height > 0");
                    }
                    if (intersection.Width < s_EPSILON)
                    {
                        DRT.Assert(false, "Bottom: intersection.Width == 0");
                    }
                }
                else if (_popup.Placement == PlacementMode.Right)
                {
                    if (intersection.Height < s_EPSILON)
                    {
                        DRT.Assert(false, "Right: intersection.Height == 0");
                    }

                    if (intersection.Width > s_EPSILON)
                    {
                        DRT.Assert(false, String.Format("Right: intersection.Width (= {0}) > 0", intersection.Width));
                    }
                }
                else if (_popup.Placement == PlacementMode.Center)
                {
                    // center of this element should be the center of the popup
                    double popupXCenter = (popupBounds.Left + popupBounds.Right) / 2.0;
                    double popupYCenter = (popupBounds.Top + popupBounds.Bottom) / 2.0;
                    double contentXCenter = (contentBounds.Left + contentBounds.Right) / 2.0;
                    double contentYCenter = (contentBounds.Top + contentBounds.Bottom) / 2.0;
                    // we will allow off by 1/2 b/c we can only place the popup on integral coordinates
                    if (Math.Abs(popupXCenter - contentXCenter) > s_EPSILON + 0.5)
                    {
                        DRT.Assert(false, "Center: Popup was not horizontally centered (popup: " + popupXCenter + ", content: " + contentXCenter + ")");
                    }
                    if (Math.Abs(popupYCenter - contentYCenter) > s_EPSILON + 0.5)
                    {
                        DRT.Assert(false, "Center: Popup was not vertically centered (popup: " + popupYCenter + ", content: " + contentYCenter + ")");
                    }
                }
                else if (_popup.Placement == PlacementMode.Relative)
                {
                    // here the popup should be completely over the element
                    if (intersection != popupBounds)
                    {
                        DRT.Assert(false, "Relative: Popup was not completely over the element");
                    }
                }
            }
            if (_popup.Placement == PlacementMode.Mouse)
            {
                POINT mousePt = GetCursorPos();
                if (popupBounds.Left != mousePt.x)
                {
                    DRT.Assert(false, String.Format("Mouse: popup.Left ({0}) != mouse.X ({1})", popupBounds.Left, mousePt.x));
                }
            }
            else if (_popup.Placement == PlacementMode.MousePoint)
            {
                POINT mousePt = GetCursorPos();
                if (popupBounds.Left != mousePt.x || popupBounds.Top != mousePt.y)
                {
                    DRT.Assert(false, "MousePoint: popup is not at cursor position");
                }
            }

            _popup.IsOpen = false;
            s_curTest++;

            System.Console.WriteLine();

            if (s_curTest < s_tests.Length)
            {
                DRT.ResumeAt(new DrtTest(SetPlacement));
            }
            else
            {
                s_curTest = 0;
                Rect placementRect = GetFrameworkElementBounds(_placementTarget);
                _popup.PlacementRectangle = new Rect(0,0,placementRect.Width,placementRect.Height);
            }
        }


        // PlacementRect tests
        private void SetPlacement2()
        {
            PlacementTest test = s_tests[s_curTest];
            _popup.Placement = test.Placement;
            _popup.IsOpen = true;

            DRT.ResumeAt(new DrtTest(Check2));
        }


        private void Check2()
        {
            // we can't verify as well as robovision, but we can at least test that the placement
            // was mostly correct

            Rect popupBounds = GetPopupBounds();
            Rect contentBounds = GetFrameworkElementBounds(_placementTarget);
            Rect intersection;
            System.Console.WriteLine("PlacementMode(PlacementRectangle): " + _popup.Placement);

            if (_popup.Placement == PlacementMode.Bottom || _popup.Placement == PlacementMode.Right
                || _popup.Placement == PlacementMode.Center || _popup.Placement == PlacementMode.Relative)
            {
                System.Console.WriteLine("Popup: " + popupBounds);
                System.Console.WriteLine("Content: " + contentBounds);
                intersection = Rect.Intersect(popupBounds, contentBounds);
                System.Console.WriteLine("Intersection: " + intersection);
                if (intersection == Rect.Empty)
                {
                    DRT.Assert(false, "Popup was not over the element");
                }
                if (_popup.Placement == PlacementMode.Bottom)
                {
                    if (intersection.Height > s_EPSILON)
                    {
                        DRT.Assert(false, "Bottom: intersection.Height > 0");
                    }
                    if (intersection.Width < s_EPSILON)
                    {
                        DRT.Assert(false, "Bottom: intersection.Width == 0");
                    }
                }
                else if (_popup.Placement == PlacementMode.Right)
                {
                    if (intersection.Height < s_EPSILON)
                    {
                        DRT.Assert(false, "Right: intersection.Height == 0");
                    }

                    if (intersection.Width > s_EPSILON)
                    {
                        DRT.Assert(false, String.Format("Right: intersection.Width (= {0}) > 0", intersection.Width));
                    }
                }
                else if (_popup.Placement == PlacementMode.Center)
                {
                    // center of this element should be the center of the popup
                    double popupXCenter = (popupBounds.Left + popupBounds.Right) / 2.0;
                    double popupYCenter = (popupBounds.Top + popupBounds.Bottom) / 2.0;
                    double contentXCenter = (contentBounds.Left + contentBounds.Right) / 2.0;
                    double contentYCenter = (contentBounds.Top + contentBounds.Bottom) / 2.0;
                    // we will allow off by 1/2 b/c we can only place the popup on integral coordinates
                    if (Math.Abs(popupXCenter - contentXCenter) > s_EPSILON + 0.5)
                    {
                        DRT.Assert(false, "Center: Popup was not horizontally centered (popup: " + popupXCenter + ", content: " + contentXCenter + ")");
                    }
                    if (Math.Abs(popupYCenter - contentYCenter) > s_EPSILON + 0.5)
                    {
                        DRT.Assert(false, "Center: Popup was not vertically centered (popup: " + popupYCenter + ", content: " + contentYCenter + ")");
                    }
                }
                else if (_popup.Placement == PlacementMode.Relative)
                {
                    // here the popup should be completely over the element
                    if (intersection != popupBounds)
                    {
                        DRT.Assert(false, "Relative: Popup was not completely over the element");
                    }
                }
            }
            if (_popup.Placement == PlacementMode.Mouse)
            {
                POINT mousePt = GetCursorPos();
                if (popupBounds.Left != mousePt.x)
                {
                    DRT.Assert(false, String.Format("Mouse: popup.Left ({0}) != mouse.X ({1})", popupBounds.Left, mousePt.x));
                }
            }
            else if (_popup.Placement == PlacementMode.MousePoint)
            {
                POINT mousePt = GetCursorPos();
                if (popupBounds.Left != mousePt.x || popupBounds.Top != mousePt.y)
                {
                    DRT.Assert(false, "MousePoint: popup is not at cursor position");
                }
            }
            else if (_popup.Placement == PlacementMode.Custom)
            {
                double popupXCenter = (popupBounds.Left + popupBounds.Right) / 2.0;
                double contentXCenter = (contentBounds.Left + contentBounds.Right) / 2.0;
                // we will allow off by 1/2 b/c we can only place the popup on integral coordinates
                if (Math.Abs(popupXCenter - contentXCenter) > s_EPSILON + 0.5)
                {
                    DRT.Assert(false, "Center: Popup was not horizontally centered (popup: " + popupXCenter + ", content: " + contentXCenter + ")");
                }
                intersection = Rect.Intersect(popupBounds, contentBounds);
                if (intersection == Rect.Empty)
                {
                    DRT.Assert(false, "Popup was not over the element");
                }
                if (intersection.Height > s_EPSILON)
                {
                    DRT.Assert(false, "Bottom: intersection.Height > 0");
                }
            }

            _popup.IsOpen = false;
            s_curTest++;

            System.Console.WriteLine();

            if (s_curTest < s_tests.Length)
            {
                DRT.ResumeAt(new DrtTest(SetPlacement2));
            }
        }

        private class PlacementTest
        {
            public PlacementTest(PlacementMode placement)
            {
                Placement = placement;
                MousePosition = new Point(0,0);
            }
            public PlacementTest(PlacementMode placement, Point mousePosition)
            {
                Placement = placement;
                MousePosition = mousePosition;
            }

            public PlacementMode Placement;
            public Point MousePosition;
        }

        private POINT GetCursorPos()
        {
            IInputElement directlyOver = Mouse.DirectlyOver;
            if (directlyOver != null)
            {
                PresentationSource source = PresentationSource.FromVisual((Visual)directlyOver);
                HwndSource hwnd = (HwndSource)source;
                if ((hwnd != null) && !hwnd.IsDisposed)
                {
                    Visual rootVisual = hwnd.RootVisual;
                    CompositionTarget ct = hwnd.CompositionTarget;
                    if ((rootVisual != null) && (ct != null))
                    {
                        Point pt = Mouse.GetPosition((IInputElement)rootVisual);
                        Matrix transform = GetVisualTransform(rootVisual) * ct.TransformToDevice;
                        pt = transform.Transform(pt);

                        POINT devicePt = new POINT((int)pt.X, (int)pt.Y);
                        ClientToScreen(hwnd.Handle, devicePt);

                        POINT verifyPt = new POINT(0, 0);
                        GetCursorPos(verifyPt);

                        if (devicePt.x != verifyPt.x || devicePt.y != verifyPt.y)
                        {
                            DRT.Assert(false, String.Format("Avalon mouse position ({0}, {1}) does not match Win32 device ({2}, {3})", devicePt.x, devicePt.y, verifyPt.x, verifyPt.y));
                        }

                        return devicePt;
                    }
                }
            }

            POINT outsidePt = new POINT(0, 0);
            GetCursorPos(outsidePt);
            return outsidePt;
        }

        /// <summary>
        ///     Gets the matrix that will convert a point 
        ///     from "above" the coordinate space of a visual
        ///     into the the coordinate space "below" the visual.
        /// </summary>
        private static Matrix GetVisualTransform(Visual v)
        {
            if (v != null)
            {
                Matrix m = Matrix.Identity;

                Transform transform = VisualTreeHelper.GetTransform(v);
                if (transform != null)
                {
                    Matrix cm = transform.Value;
                    m = Matrix.Multiply(m, cm);
                }

                Vector offset = VisualTreeHelper.GetOffset(v);
                m.Translate(offset.X, offset.Y);

                return m;
            }

            return Matrix.Identity;
        }

        private static int s_curTest = 0;
        private static PlacementTest[] s_tests = new PlacementTest[]
            {
                new PlacementTest(PlacementMode.Right, new Point(200, 200)),
                new PlacementTest(PlacementMode.Bottom),
                new PlacementTest(PlacementMode.Center),
                new PlacementTest(PlacementMode.Relative),
                // 200, 200 should be over the main window
                new PlacementTest(PlacementMode.Mouse, new Point(200, 200)),
                new PlacementTest(PlacementMode.MousePoint, new Point(200, 200)),
                // Do another test to make sure avalon has picked up the new position of the mouse
                new PlacementTest(PlacementMode.Right, new Point(700, 500)),
                // 600,100 should be outside the main window
                new PlacementTest(PlacementMode.Mouse, new Point(700, 500)),
                new PlacementTest(PlacementMode.MousePoint, new Point(700, 500)),
                new PlacementTest(PlacementMode.Custom),
            };

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos([In, Out] POINT pt);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int ClientToScreen(IntPtr hWnd, [In, Out] POINT pt);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool GetClientRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public class POINT {
            public int x = 0;
            public int y = 0;

            public POINT() {
            }

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height) {
                return new RECT(x, y, x + width, y + height);
            }
        }


    }

    public class PlacementModeConverter : IValueConverter
    {
        public object Convert(object o, Type type, object parameter, CultureInfo culture)
        {
            return System.Convert.ChangeType(o, typeof(int));
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            return Enum.ToObject(typeof(PlacementMode), (int)o);
        }
    }

}

