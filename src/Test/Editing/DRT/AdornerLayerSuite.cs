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
using System.Windows.Documents;

namespace DRT
{
    internal class AdornerLayerSuite : DrtTestSuite
    {
        // Constructor.
        internal AdornerLayerSuite() : base("AdornerLayer")
        {
        }

        internal class BoxElement : FrameworkElement
        {
            public BoxElement(int left, int top, int width, int height)
            {
                Canvas.SetLeft(this, left);
                Canvas.SetTop(this, top);
                this.Width = width;
                this.Height = height;
            }

            protected override void OnRender(DrawingContext ctx)
            {
                ctx.DrawRectangle(null, new Pen(new SolidColorBrush(Color), 2.0), new Rect(0, 0, Width, Height));
            }

            public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
        }

        internal class DRTAdornerLayerException : Exception
        {
            public DRTAdornerLayerException() : base()
            {
            }

            public DRTAdornerLayerException(string message) : base(message)
            {
            }
        }

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {            
            CreateWindow();

            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest( CreateAdorners ),
                new DrtTest( MoveAdorners ),
                new DrtTest( ResizeAdorners ),
                new DrtTest( CloseWindow ),
            };
        }

        private UIElement CreateWindow()
        {
            _win = new Window();
            _win.Width = 800;
            _win.Height = 600;
            _win.Title = _windowText;
            _el[0] = new BoxElement(0, 0, 100, 100);
            _el[1] = new BoxElement(0, 120, 100, 100);
            _el[2] = new BoxElement(120, 0, 100, 100);
            _el[3] = new BoxElement(120, 120, 100, 100);
            _firstChild = new Canvas();
            _firstChild.Children.Add(_el[0] as BoxElement);
            _firstChild.Children.Add(_el[1] as BoxElement);
            _firstChild.Children.Add(_el[2] as BoxElement);
            _firstChild.Children.Add(_el[3] as BoxElement);

            _win.Content = _firstChild;
            _win.Closed += new EventHandler(OnClosed);
            _win.Show();

            return _firstChild;
        }

        private void CreateAdorners()
        {
            // Create adorners
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00)), 1.0f);
            Brush brush = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xff));
            Size size = new Size(9, 9);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_firstChild);
            GrabHandleAdorner adorner = new GrabHandleAdorner(_el[0], GrabHandleAnchor.Centered, GrabHandles.All, size, pen, brush);

            layer.Add(adorner);
            adorner = new GrabHandleAdorner(_el[1], GrabHandleAnchor.Outside, GrabHandles.All, size, pen, brush);
            layer.Add(adorner);
            adorner = new GrabHandleAdorner(_el[2], GrabHandleAnchor.Inside, GrabHandles.All, size, pen, brush);
            layer.Add(adorner);
            adorner = new GrabHandleAdorner(_el[3], GrabHandleAnchor.Centered, GrabHandles.Corners, size, pen, brush);
            layer.Add(adorner);
        }

        private void MoveAdorners()
        {
            TestAdorners(4);
            Canvas.SetLeft(_el[2], 250);
            Canvas.SetTop(_el[2], 250);
        }

        private void ResizeAdorners()
        {
            TestAdorners(4);
            if (HitTest(new Point(0, 0), _el[0]) == false || HitTest(new Point(120, 120), _el[3]) == false)
            {
                throw (new DRTAdornerLayerException("Hit test failed."));
            }

            Thread.Sleep(1000);
            _el[2].Width = 50;
            _el[2].Height = 50;

            // And while we're at it, let's remove adorners from element 1 and remove element 0
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_firstChild);
            Adorner[] adorners = layer.GetAdorners(_el[1]);

            foreach (Adorner adorner in adorners)
                layer.Remove(adorner);

            _firstChild.Children.Remove(_el[0]);
            layer.InvalidateMeasure();
        }

        private void TestAdorners(int numExpected)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_firstChild);
            int count = VisualTreeHelper.GetChildrenCount(layer);

            if (count != numExpected)
            {
                throw (new DRTAdornerLayerException("Incorrect number of adorners."));
            }
        }

        private bool HitTest(Point pt, UIElement element)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_firstChild);
            AdornerHitTestResult result = layer.AdornerHitTest(pt);

            if (result == null || result.Adorner.AdornedElement != element)
                return false;
            else
                return true;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Window win = sender as Window;

            if (win == null)
            {
                throw new DRTAdornerLayerException("sender in Window.Closed Handler is not a Window");
            }
        }

        private void CloseWindow()
        {
            TestAdorners(2);
            Thread.Sleep(1000);
            _win.Close();
        }

        private Canvas _firstChild;
        private BoxElement[] _el = new BoxElement[4];
        private Window _win;
        private string _windowText = "Drt Window";
    }
}