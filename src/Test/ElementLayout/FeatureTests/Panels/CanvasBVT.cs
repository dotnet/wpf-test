// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Windows.Media.Imaging;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{

    //////////////////////////////////////////////////////////////////
    /// This will load and run all Canvas code BVT's.
    /// 
    /// Possible Tests:
    /// 
    //////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////
    //
    //  This test case is to mimic a Zindex type functionality
    //	in the Cavnas..   
    //
    //	4 images are tiled, and the last image is on top..  
    //	the test will reverse the order, so the first image
    //	is on top.
    //
    //////////////////////////////////////////////////////////

    [Test(0, "Panels.Canvas", "CanvasZorderRTL", Variables="Area=ElementLayout")]
    public class CanvasZorderRTL : CodeTest
    {
        public CanvasZorderRTL()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 650;
            this.window.Height = 650;
            this.window.Content = this.TestContent();
        }
        
        private Canvas _canvas;
        private Image _i1;
        private Image _i2;
        private Image _i3;
        private Image _i4;

        public override FrameworkElement TestContent()
        {
            _canvas = new Canvas();
            _canvas.FlowDirection = FlowDirection.RightToLeft;
            _canvas.Background = Brushes.Pink;
            _canvas.Height = 600;
            _canvas.Width = 600;

            BitmapImage img1 = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img2 = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img3 = new BitmapImage(new Uri("light.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img4 = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));

            _i1 = new Image();
            _i1.Source = img1;
            _i1.Height = 100;
            _i1.Width = 100;
            _i1.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i1, 25);
            Canvas.SetTop(_i1, 25);

            _i2 = new Image();
            _i2.Source = img2;
            _i2.Height = 100;
            _i2.Width = 100;
            _i2.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i2, 50);
            Canvas.SetTop(_i2, 50);

            _i3 = new Image();
            _i3.Source = img3;
            _i3.Height = 100;
            _i3.Width = 100;
            _i3.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i3, 75);
            Canvas.SetTop(_i3, 75);

            _i4 = new Image();
            _i4.Source = img4;
            _i4.Height = 100;
            _i4.Width = 100;
            _i4.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i4, 100);
            Canvas.SetTop(_i4, 100);

            _canvas.Children.Add(_i1);
            _canvas.Children.Add(_i2);
            _canvas.Children.Add(_i3);
            _canvas.Children.Add(_i4);

            return _canvas;
        }
        public override void TestActions()
        {
            _canvas.Children.Clear();
            _canvas.Children.Add(_i4);
            _canvas.Children.Add(_i3);
            _canvas.Children.Add(_i2);
            _canvas.Children.Add(_i1);
        }
        public override void TestVerify()
        {
            this.Result = true;
            string[] expected = { "cloud.bmp", "computer.bmp", "light.bmp", "note.bmp" };

            for (int r = 0; r < 4; r++)
            {
                Point pt = new Point(75 + (r * 25), 110 + (r * 25));
                IInputElement myInputElement;

                myInputElement = _canvas.InputHitTest(pt);

                Image img = new Image();
                img = (Image)myInputElement;

                if (img.Source.ToString() != expected[r])
                {
                    this.Result = false;
                    Helpers.Log(string.Format("EXPECTED [ {0} ] ", expected[r]));// img.Source.ToString() + " has been hit. " + expected[r] + " should be hit.");
                    Helpers.Log(string.Format("ACTUAL   [ {0} ] ", img.Source.ToString()));
                }
                else 
                {
                    Helpers.Log(string.Format("EXPECTED [ {0} ] HIT", expected[r]));
                }
            }
        }

    }
    
    //////////////////////////////////////////////////////////
    //
    //  This test case is to mimic a Zindex type functionality
    //	in the Cavnas..   
    //
    //	4 images are tiled, and the last image is on top..  
    //	the test will reverse the order, so the first image
    //	is on top.
    //
    //////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasZorder", Variables="Area=ElementLayout")]
    public class CanvasZorder : CodeTest
    {
        public CanvasZorder()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 650;
            this.window.Height = 650;
            this.window.Content = this.TestContent();
        }
       
        private Canvas _canvas;
        private Image _i1;
        private Image _i2;
        private Image _i3;
        private Image _i4;

        public override FrameworkElement TestContent()
        {
            _canvas = new Canvas();
            _canvas.Background = Brushes.Pink;
            _canvas.Height = 600;
            _canvas.Width = 600;

            BitmapImage img1 = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img2 = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img3 = new BitmapImage(new Uri("light.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img4 = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));

            _i1 = new Image();
            _i1.Source = img1;
            _i1.Height = 100;
            _i1.Width = 100;
            _i1.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i1, 25);
            Canvas.SetTop(_i1, 25);

            _i2 = new Image();
            _i2.Source = img2;
            _i2.Height = 100;
            _i2.Width = 100;
            _i2.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i2, 50);
            Canvas.SetTop(_i2, 50);

            _i3 = new Image();
            _i3.Source = img3;
            _i3.Height = 100;
            _i3.Width = 100;
            _i3.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i3, 75);
            Canvas.SetTop(_i3, 75);

            _i4 = new Image();
            _i4.Source = img4;
            _i4.Height = 100;
            _i4.Width = 100;
            _i4.Stretch = Stretch.Fill;
            Canvas.SetLeft(_i4, 100);
            Canvas.SetTop(_i4, 100);

            _canvas.Children.Add(_i1);
            _canvas.Children.Add(_i2);
            _canvas.Children.Add(_i3);
            _canvas.Children.Add(_i4);

            return _canvas;
        }
        public override void TestActions()
        {
            _canvas.Children.Clear();
            _canvas.Children.Add(_i4);
            _canvas.Children.Add(_i3);
            _canvas.Children.Add(_i2);
            _canvas.Children.Add(_i1);
        }
        public override void TestVerify()
        {
            this.Result = true;
            string[] expected = { "cloud.bmp", "computer.bmp", "light.bmp", "note.bmp" };

            for (int r = 0; r < 4; r++)
            {
                Point pt = new Point(75 + (r * 25), 110 + (r * 25));
                
                IInputElement myInputElement;
                myInputElement = _canvas.InputHitTest(pt);

                Image img = new Image();
                img = (Image)myInputElement;
                
                if (img.Source.ToString() != expected[r])
                {
                    this.Result = false;
                    Helpers.Log(string.Format("EXPECTED [ {0} ] ", expected[r]));// img.Source.ToString() + " has been hit. " + expected[r] + " should be hit.");
                    Helpers.Log(string.Format("ACTUAL   [ {0} ] ", img.Source.ToString()));
                }
                else
                {
                    Helpers.Log(string.Format("EXPECTED [ {0} ] HIT", expected[r]));
                }
            }
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing how the Canvas sizes it self when child
    //  elements have margin properties set..
    //
    //  Globalized : FlowDirection.RightToLeft used.
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasSizeToContentChildMarginsRTL", Variables="Area=ElementLayout")]
    public class CanvasSizeToContentChildMarginsRTL : CodeTest
    {

        public CanvasSizeToContentChildMarginsRTL()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle[] rect;

        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            canvas = new Canvas();
            canvas.FlowDirection = FlowDirection.RightToLeft;
            eRoot.Height = 325;
            eRoot.Width = 325;
            canvas.Height = 325;
            canvas.Width = 325;
            rect = new Rectangle[4];
            for (int b = 0; b < 4; b++)
            {
                rect[b] = new Rectangle();
                rect[b].Width = 100;
                rect[b].Height = 100;
                canvas.Children.Add(rect[b]);
            }

            rect[0].Fill = new SolidColorBrush(Colors.Red);
            rect[1].Fill = new SolidColorBrush(Colors.Orange);
            rect[2].Fill = new SolidColorBrush(Colors.Green);
            rect[3].Fill = new SolidColorBrush(Colors.Blue);
            Canvas.SetLeft(rect[0], 0);
            Canvas.SetTop(rect[0], 0);
            Canvas.SetRight(rect[1], 0);
            Canvas.SetTop(rect[1], 0);
            Canvas.SetRight(rect[2], 0);
            Canvas.SetBottom(rect[2], 0);
            Canvas.SetLeft(rect[3], 0);
            Canvas.SetBottom(rect[3], 0);
            rect[0].Margin = new Thickness(100, 0, 0, 0);
            rect[1].Margin = new Thickness(0, 100, 0, 0);
            rect[2].Margin = new Thickness(0, 0, 100, 0);
            rect[3].Margin = new Thickness(0, 0, 0, 100);
            eRoot.Children.Add(canvas);
            return eRoot;
        }
        public override void TestVerify()
        {
            double[] expectedXvalue = { 225, 100, 200, 325 };
            double[] expectedYvalue = { 0, 100, 225, 125 };
            double[] actualXvalue = new double[4];
            double[] actualYvalue = new double[4];

            this.Result = true;
            for (int r = 0; r < 4; r++)
            {
                Matrix pt;
                System.Windows.Media.GeneralTransform gt = rect[r].TransformToAncestor(eRoot);
                System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
                if (t == null)
                {
                    throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
                }
                pt = t.Value;
                actualXvalue[r] = pt.OffsetX;
                actualYvalue[r] = pt.OffsetY;
            }

            for (int v = 0; v < 4; v++)
            {
                if (expectedXvalue[v] != actualXvalue[v])
                {
                    this.Result = false;
                    Helpers.Log("Expected X " + v.ToString() + ": " + expectedXvalue[v]);
                    Helpers.Log("  Actual X " + v.ToString() + ": " + actualXvalue[v]);
                }

                if (expectedYvalue[v] != actualYvalue[v])
                {
                    this.Result = false;
                    Helpers.Log("Expected Y " + v.ToString() + ": " + expectedYvalue[v]);
                    Helpers.Log("  Actual Y " + v.ToString() + ": " + actualYvalue[v]);
                }
            }
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing how the Canvas sizes it self when child
    //  elements have margin properties set..
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasSizeToContentChildMargins", Variables="Area=ElementLayout")]
    public class CanvasSizeToContentChildMargins : CodeTest
    {

        public CanvasSizeToContentChildMargins()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle[] rect;

        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            canvas = new Canvas();
            eRoot.Height = 325;
            eRoot.Width = 325;
            canvas.Height = 325;
            canvas.Width = 325;
            rect = new Rectangle[4];
            for (int b = 0; b < 4; b++)
            {
                rect[b] = new Rectangle();
                rect[b].Width = 100;
                rect[b].Height = 100;
                canvas.Children.Add(rect[b]);
            }

            rect[0].Fill = new SolidColorBrush(Colors.Red);
            rect[1].Fill = new SolidColorBrush(Colors.Orange);
            rect[2].Fill = new SolidColorBrush(Colors.Green);
            rect[3].Fill = new SolidColorBrush(Colors.Blue);
            Canvas.SetLeft(rect[0], 0);
            Canvas.SetTop(rect[0], 0);
            Canvas.SetRight(rect[1], 0);
            Canvas.SetTop(rect[1], 0);
            Canvas.SetRight(rect[2], 0);
            Canvas.SetBottom(rect[2], 0);
            Canvas.SetLeft(rect[3], 0);
            Canvas.SetBottom(rect[3], 0);
            rect[0].Margin = new Thickness(100, 0, 0, 0);
            rect[1].Margin = new Thickness(0, 100, 0, 0);
            rect[2].Margin = new Thickness(0, 0, 100, 0);
            rect[3].Margin = new Thickness(0, 0, 0, 100);
            eRoot.Children.Add(canvas);
            return eRoot;
        }
        public override void TestVerify()
        {
            double[] expectedXvalue = { 100, 225, 125, 0 }; //for LTR
            double[] expectedYvalue = { 0, 100, 225, 125 }; //for LTR
            double[] actualXvalue = new double[4];
            double[] actualYvalue = new double[4];

            this.Result = true;
            for (int r = 0; r < 4; r++)
            {
                Matrix pt;
                System.Windows.Media.GeneralTransform gt = rect[r].TransformToAncestor(eRoot);
                System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
                if (t == null)
                {
                    throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
                }
                pt = t.Value;
                actualXvalue[r] = pt.OffsetX;
                actualYvalue[r] = pt.OffsetY;
            }

            for (int v = 0; v < 4; v++)
            {
                if (expectedXvalue[v] != actualXvalue[v])
                {
                    this.Result = false;
                    Helpers.Log("Expected X " + v.ToString() + ": " + expectedXvalue[v]);
                    Helpers.Log("  Actual X " + v.ToString() + ": " + actualXvalue[v]);
                }

                if (expectedYvalue[v] != actualYvalue[v])
                {
                    this.Result = false;
                    Helpers.Log("Expected Y " + v.ToString() + ": " + expectedYvalue[v]);
                    Helpers.Log("  Actual Y " + v.ToString() + ": " + actualYvalue[v]);
                }
            }
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas and how the positioning props
    //  are affect by a window resize..
    //
    //  This case will verify the position of the image after each
    //  window resize..  if image position is not correct the 
    //  case will fail..
    //  
    //  if either of these return a fail, the case will fail..
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasResizeAndPositionRTL", Variables="Area=ElementLayout")]
    public class CanvasResizeAndPositionRTL : CodeTest
    {


        public CanvasResizeAndPositionRTL()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();


        }

        public DockPanel eRoot;
        public Canvas canvas;
        public Image image;
        public int up = 0;
        public int down = 0;
        public bool makeBigResult;
        public bool makeSmallResult;
        public override FrameworkElement TestContent()
        {
            canvas = new Canvas();
            canvas.FlowDirection = FlowDirection.RightToLeft;
            BitmapImage img1 = new BitmapImage(new Uri("light.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img2 = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));
            Image imageNoMove = new Image();

            imageNoMove.Source = img2;
            imageNoMove.Height = 100;
            imageNoMove.Width = 100;
            Canvas.SetLeft(imageNoMove, 0);
            Canvas.SetTop(imageNoMove, 0);
            image = new Image();
            image.Source = img1;
            image.Height = 125;
            image.Width = 137.5;
            Canvas.SetRight(image, 0);
            Canvas.SetBottom(image, 0);

            canvas.Children.Add(imageNoMove);
            canvas.Children.Add(image);
            return canvas;
        }
        public override void TestActions()
        {
            makeWinBig();
        }
        public override void TestVerify()
        {
            // set this.Result..
            Helpers.Log("******************************************");
            Helpers.Log("makeWinBig result: " + makeBigResult.ToString());
            Helpers.Log("makeWinSmall result: " + makeSmallResult.ToString());
            Helpers.Log("******************************************");
            this.Result = (makeBigResult != true || makeSmallResult != true) ? false : true;
        }
        public void makeWinBig()
        {
            up++;
            this.window.Height = this.window.Height + 100;
            this.window.Width = this.window.Width + 100;
            if (up <= 3)
            {
                CommonFunctionality.FlushDispatcher();
                makeWinBig();
            }
            else
            {
                makeBigResult = true;
                CommonFunctionality.FlushDispatcher();
                makeWinSmall();
            }

            if (this.positionVerify() != true)
            {
                makeBigResult = false;
            }
        }

        public void makeWinSmall()
        {
            down++;
            this.window.Height = this.window.Height - 100;
            this.window.Width = this.window.Width - 100;
            if (down <= 3)
            {
                makeWinSmall();
            }
            else
            {
                makeSmallResult = true;
            }

            if (positionVerify() != true)
            {
                makeSmallResult = false;
            }
        }
        public bool positionVerify()
        {
            Helpers.Log("******************************************");
            Helpers.Log("window size is " + this.window.RenderSize);
            Helpers.Log("image is positioned at");
            Helpers.Log("right: " + Canvas.GetRight(image));
            Helpers.Log("bottom: " + Canvas.GetBottom(image));
            Helpers.Log("******************************************");
            if ((Canvas.GetRight(image)) != 0 || (Canvas.GetBottom(image)) != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas and how the positioning props
    //  are affect by a window resize..
    //
    //  This case will verify the position of the image after each
    //  window resize..  if image position is not correct the 
    //  case will fail..
    //  
    //  if either of these return a fail, the case will fail..
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasResizeAndPosition", Variables="Area=ElementLayout")]
    public class CanvasResizeAndPosition : CodeTest
    {
        public CanvasResizeAndPosition()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();


        }

        public DockPanel eRoot;
        public Canvas canvas;
        public Image image;
        public int up = 0;
        public int down = 0;
        public bool makeBigResult;
        public bool makeSmallResult;
        public override FrameworkElement TestContent()
        {
            canvas = new Canvas();
            BitmapImage img1 = new BitmapImage(new Uri("light.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage img2 = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));
            Image imageNoMove = new Image();

            imageNoMove.Source = img2;
            imageNoMove.Height = 100;
            imageNoMove.Width = 100;
            Canvas.SetLeft(imageNoMove, 0);
            Canvas.SetTop(imageNoMove, 0);
            image = new Image();
            image.Source = img1;
            image.Height = 125;
            image.Width = 137.5;
            Canvas.SetRight(image, 0);
            Canvas.SetBottom(image, 0);

            canvas.Children.Add(imageNoMove);
            canvas.Children.Add(image);
            return canvas;
        }
        public override void TestActions()
        {
            makeWinBig();
        }
        public override void TestVerify()
        {
            // set this.Result..
            Helpers.Log("******************************************");
            Helpers.Log("makeWinBig result: " + makeBigResult.ToString());
            Helpers.Log("makeWinSmall result: " + makeSmallResult.ToString());
            Helpers.Log("******************************************");
            this.Result = (makeBigResult != true || makeSmallResult != true) ? false : true;
        }
        public void makeWinBig()
        {
            up++;
            this.window.Height = this.window.Height + 100;
            this.window.Width = this.window.Width + 100;
            if (up <= 3)
            {
                CommonFunctionality.FlushDispatcher();
                makeWinBig();
            }
            else
            {
                makeBigResult = true;
                CommonFunctionality.FlushDispatcher();
                makeWinSmall();
            }

            if (this.positionVerify() != true)
            {
                makeBigResult = false;
            }
        }

        public void makeWinSmall()
        {
            down++;
            this.window.Height = this.window.Height - 100;
            this.window.Width = this.window.Width - 100;
            if (down <= 3)
            {
                makeWinSmall();
            }
            else
            {
                makeSmallResult = true;
            }

            if (positionVerify() != true)
            {
                makeSmallResult = false;
            }
        }
        public bool positionVerify()
        {
            Helpers.Log("******************************************");
            Helpers.Log("window size is " + this.window.RenderSize);
            Helpers.Log("image is positioned at");
            Helpers.Log("right: " + Canvas.GetRight(image));
            Helpers.Log("bottom: " + Canvas.GetBottom(image));
            Helpers.Log("******************************************");
            if ((Canvas.GetRight(image)) != 0 || (Canvas.GetBottom(image)) != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [Test(0, "Panels.Canvas", "CanvasPositionPropsRTL", Variables="Area=ElementLayout")]
    public class CanvasPositionPropsRTL : CodeTest
    {

        public CanvasPositionPropsRTL()
        {  }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle[] rect;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Background = new SolidColorBrush(Colors.CornflowerBlue);
            eRoot.Width = 400;
            eRoot.Height = 400;
            canvas = new Canvas();
            canvas.FlowDirection = FlowDirection.RightToLeft;
            canvas.Height = 400;
            canvas.Width = 400;

            rect = new Rectangle[4];

            for (int r = 0; r < 4; r++)
            {
                rect[r] = new Rectangle();
                rect[r].Width = 170;
                rect[r].Height = 170;
                canvas.Children.Add(rect[r]);
            }

            rect[0].Fill = new SolidColorBrush(Colors.Red);
            Canvas.SetLeft(rect[0], 25);
            Canvas.SetTop(rect[0], 25);
            rect[1].Fill = new SolidColorBrush(Colors.Blue);
            Canvas.SetRight(rect[1], 25);
            Canvas.SetTop(rect[1], 25);
            rect[2].Fill = new SolidColorBrush(Colors.Green);
            Canvas.SetRight(rect[2], 25);
            Canvas.SetBottom(rect[2], 25);
            rect[3].Fill = new SolidColorBrush(Colors.Orange);
            Canvas.SetLeft(rect[3], 25);
            Canvas.SetBottom(rect[3], 25);
            eRoot.Children.Add(canvas);
            return eRoot;
        }
        public override void TestVerify()
        {
            double[] expectedXvalue = { 375, 195, 195, 375 };
            double[] expectedYvalue = { 25, 25, 205, 205 };
            double[] actualXvalue = new double[4];
            double[] actualYvalue = new double[4];

            this.Result = true;

            for (int i = 0; i < 4; i++)
            {
                Matrix pt;
                System.Windows.Media.GeneralTransform gt = rect[i].TransformToAncestor(eRoot);
                System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
                if (t == null)
                {
                    throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
                }
                pt = t.Value;
                actualXvalue[i] = pt.OffsetX;
                actualYvalue[i] = pt.OffsetY;
            }

            for (int j = 0; j < 4; j++)
            {
                if (expectedXvalue[j] != actualXvalue[j])
                {
                    this.Result = false;
                    Helpers.Log("Expected X " + j.ToString() + ": " + expectedXvalue[j]);
                    Helpers.Log("  Actual X " + j.ToString() + ": " + actualXvalue[j]);
                }

                if (expectedYvalue[j] != actualYvalue[j])
                {
                    this.Result = false;
                    Helpers.Log("Expected Y " + j.ToString() + ": " + expectedYvalue[j]);
                    Helpers.Log("  Actual Y " + j.ToString() + ": " + actualYvalue[j]);
                }
            }
        }
    }

    [Test(0, "Panels.Canvas", "CanvasPositionProps", Variables="Area=ElementLayout")]
    public class CanvasPositionProps : CodeTest
    {

        public CanvasPositionProps()
        {}

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Canvas eRoot;
        public override FrameworkElement TestContent()
        {
            Border bRoot = new Border();

            bRoot.Background = new SolidColorBrush(Colors.CornflowerBlue);
            bRoot.Height = 400;
            bRoot.Width = 400;
            eRoot = new Canvas();
            eRoot.Height = 400;
            eRoot.Width = 400;

            BitmapImage image = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));
            Image imgA = new Image();

            imgA.Source = image;
            imgA.Height = 170;
            imgA.Width = 170;
            Canvas.SetLeft(imgA, 25);
            Canvas.SetTop(imgA, 25);

            Image imgB = new Image();

            imgB.Source = image;
            imgB.Height = 170;
            imgB.Width = 170;
            Canvas.SetRight(imgB, 25);
            Canvas.SetTop(imgB, 25);

            Image imgC = new Image();

            imgC.Source = image;
            imgC.Height = 170;
            imgC.Width = 170;
            Canvas.SetRight(imgC, 25);
            Canvas.SetBottom(imgC, 25);

            Image imgD = new Image();

            imgD.Source = image;
            imgD.Height = 170;
            imgD.Width = 170;
            Canvas.SetLeft(imgD, 25);
            Canvas.SetBottom(imgD, 25);
            eRoot.Children.Add(imgA);
            eRoot.Children.Add(imgB);
            eRoot.Children.Add(imgC);
            eRoot.Children.Add(imgD);
            bRoot.Child = eRoot;
            return bRoot;
        }
        public override void TestVerify()
        {
            int count = eRoot.Children.Count;

            this.Result = true;

            double[] actualValueX = new double[count];
            double[] actualValueY = new double[count];
            double[] expectedValuesX = new double[] { 25, 205, 205, 25 };
            double[] expectedValuesY = new double[] { 25, 25, 205, 205 };

            for (int i = 0; i < count; i++)
            {
                PointHitTestResult result = (PointHitTestResult)VisualTreeHelper.HitTest(eRoot.Children[i], new Point(0, 0));
                Matrix pt;
                System.Windows.Media.GeneralTransform gt = (eRoot.Children[i]).TransformToAncestor(eRoot);
                System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
                if (t == null)
                {
                    throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
                }
                pt = t.Value;

                actualValueX[i] = pt.OffsetX;
                actualValueY[i] = pt.OffsetY;
                Helpers.Log("x: " + actualValueX[i] + "\ny: " + actualValueY[i]);
            }

            for (int x = 0; x < count; x++)
            {
                if (actualValueX[x] != expectedValuesX[x])
                {
                    this.Result = false;
                    Helpers.Log("Element " + x.ToString() + "\nActual x Value: " + actualValueX[x] + "\nExpected x Value: " + expectedValuesX[x]);
                }
            }

            for (int y = 0; y < count; y++)
            {
                if (actualValueY[y] != expectedValuesY[y])
                {
                    this.Result = false;
                    Helpers.Log("Element " + y.ToString() + "\nActual y Value: " + actualValueY[y] + "\nExpected y Value: " + expectedValuesY[y]);
                }
            }
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas the order of precedence when 
    //  declaring positioning properties..
    //
    //  Canvas.SetTop should have precedence over Canvas.SetBottom
    //
    //  This case is verified by using OffsetY value.
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasPositionPrecedenceTB", Variables="Area=ElementLayout")]
    public class CanvasPositionPrecedenceTB : CodeTest
    {
        public CanvasPositionPrecedenceTB()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        private Border _border;
        private Canvas _canvas;
        public override FrameworkElement TestContent()
        {
            _border = new Border();
            _border.Background = new SolidColorBrush(Colors.LightSteelBlue);
            _border.Height = 400;
            _border.Width = 400;
            _canvas = new Canvas();
            _canvas.Height = 400;
            _canvas.Width = 400;

            BitmapImage img = new BitmapImage(new Uri("note.bmp", UriKind.RelativeOrAbsolute));
            Image iTB = new Image();

            iTB.Source = img;
            iTB.Height = 200;
            iTB.Width = 200;
            iTB.Stretch = Stretch.Fill;
            Canvas.SetTop(iTB, 20);
            Canvas.SetBottom(iTB, 20);
            _canvas.Children.Add(iTB);
            _border.Child = _canvas;
            return _border;
        }
        public override void TestVerify()
        {
            Matrix pt;
            System.Windows.Media.GeneralTransform gt = (_canvas.Children[0]).TransformToAncestor(_canvas);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            pt = t.Value;
            if (pt.OffsetY == 20)
                this.Result = true;
            else
                this.Result = false;
            Helpers.Log("Image didn't get Offset from Top correctly. -- OffsetY: " + pt.OffsetY.ToString() + " should be 20");
        }

    }

    [Test(0, "Panels.Canvas", "CanvasPositionPrecedenceRTL", Variables="Area=ElementLayout")]
    public class CanvasPositionPrecedenceRTL : CodeTest
    {

        public CanvasPositionPrecedenceRTL()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        private DockPanel _eRoot;
        private Canvas _canvas;
        private Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _eRoot = new DockPanel();

            _eRoot.Background = new SolidColorBrush(Colors.LightSteelBlue);
            _eRoot.Height = 400;
            _eRoot.Width = 400;

            _canvas = new Canvas();

            _canvas.FlowDirection = FlowDirection.RightToLeft;
            _canvas.Height = 400;
            _canvas.Width = 400;

            _rect = new Rectangle();
            _rect.Height = 300;
            _rect.Width = 200;
            _rect.Fill = new SolidColorBrush(Colors.Red);

            Canvas.SetLeft(_rect, 20);
            Canvas.SetRight(_rect, 20);
            _canvas.Children.Add(_rect);
            _eRoot.Children.Add(_canvas);
            return _eRoot;
        }
        public override void TestVerify()
        {
            Matrix pt;
            System.Windows.Media.GeneralTransform gt = _rect.TransformToAncestor(_eRoot);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            pt = t.Value;
            if (pt.OffsetX == 380)
                this.Result = true;
            else
            {
                this.Result = false;
                Helpers.Log("Check the position of the red rectangle.");
            }
        }

    }

    [Test(0, "Panels.Canvas", "CanvasPositionPrecedenceLR", Variables="Area=ElementLayout")]
    public class CanvasPositionPrecedenceLR : CodeTest
    {

        public CanvasPositionPrecedenceLR()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Border border;
        public Canvas canvas;
        public override FrameworkElement TestContent()
        {
            border = new Border();
            border.Background = new SolidColorBrush(Colors.LightSteelBlue);
            border.Height = 400;
            border.Width = 400;
            canvas = new Canvas();
            canvas.Height = 400;
            canvas.Width = 400;

            BitmapImage img = new BitmapImage(new Uri("light.bmp", UriKind.RelativeOrAbsolute));
            Image iLR = new Image();

            iLR.Source = img;
            iLR.Height = 300;
            iLR.Width = 200;
            iLR.Stretch = Stretch.Fill;
            Canvas.SetLeft(iLR, 20);
            Canvas.SetRight(iLR, 20);
            canvas.Children.Add(iLR);
            border.Child = canvas;
            return border;
        }
        public override void TestVerify()
        {
            Matrix pt;
            System.Windows.Media.GeneralTransform gt = canvas.Children[0].TransformToAncestor(canvas);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            pt = t.Value;
            if (pt.OffsetX == 20)
                this.Result = true;
            else
            {
                this.Result = false;
                Helpers.Log("Image didn't get offset from left. -- OffsetX: " + pt.OffsetX.ToString());
            }
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas MinHeight and MinWidth
    //      
    //  This case will resize the app window then verify the size
    //  of the child element inside the Canvas..
    //  
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasMinHeightWidth", Variables="Area=ElementLayout")]
    public class CanvasMinHeightWidth : CodeTest
    {
        public CanvasMinHeightWidth()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 808;
            this.window.Height = 834;
            this.window.Content = this.TestContent();


        }

        public Canvas canvas;
        public Border b;
        public override FrameworkElement TestContent()
        {
            canvas = new Canvas();

            b = new Border();
            b.Background = new SolidColorBrush(Colors.DarkOrange);
            b.MinHeight = 200;
            b.MinWidth = 200;

            canvas.Children.Add(b);
            return canvas;
        }
        public override void TestActions()
        {
            Helpers.Log("resizing window, then verifying minheight and min width..");
            this.window.Height = 134;
            this.window.Width = 108;
        }
        public override void TestVerify()
        {
            if (b.ActualHeight != 200 || b.ActualWidth != 200)
            {
                this.Result = false;
                Helpers.Log("test case failed because MinHeight or MinWidth was not respected..");
            }
            else
            {
                this.Result = true;
            }

        }

    }

    /////////////////////////////////////////////////////////////////////////
    //
    //	This test case is for the MAXHEIGHT and MAXWIDTH on a Canvas
    //
    //	For verification the Canvas size will be verified after resizing 
    //	the test window..  if the Canvas size is greater than 200x200
    //	after the resize the test case will fail..
    //
    /////////////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasMaxHeightWidth", Variables="Area=ElementLayout")]
    public class CanvasMaxHeightWidth : CodeTest
    {


        public CanvasMaxHeightWidth()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 200;
            this.window.Height = 200;
            this.window.Content = this.TestContent();


        }

        public Canvas canvas;
        public Border border;
        public override FrameworkElement TestContent()
        {
            canvas = new Canvas();
            canvas.Height = 200;
            canvas.Width = 200;
            border = new Border();
            border.Background = new SolidColorBrush(Colors.DarkOrange);
            border.Height = 300;
            border.Width = 300;
            border.MaxHeight = 200;
            border.MaxWidth = 200;
            canvas.Children.Add(border);
            return canvas;
        }
        public override void TestActions()
        {
            this.window.Height = 800;
            this.window.Width = 800;
        }
        public override void TestVerify()
        {
            Helpers.Log("starting verification...");
            this.Result = (border.RenderSize.Height != 200 || border.RenderSize.Width != 200) ? false : true;
            if (!this.Result)
            {
                Helpers.Log("CodeTest failed..  max height or maxwidth not enforced...");
                Helpers.Log("border size should be 200x200 after resize..  actual size is height: " + border.RenderSize.Height + " width: " + border.RenderSize.Width);
            }
        }
    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas Clip properties
    //      
    //      ClipToBounds (Image cloud.bmp) = true;
    //      ClipToBounds (Image computer.bmp)= false;
    //
    //	Globalized: FlowDirection.RightToLeft used.
    //
    //	Verification:
    //		point (30, 210) should NOT hit image.
    //		Point (190,210) should hit image.
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasClipToBoundsRTL", Variables="Area=ElementLayout")]
    public class CanvasClipToBoundsRTL : CodeTest
    {
        public CanvasClipToBoundsRTL()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        public StackPanel eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = new StackPanel();
            eRoot.Orientation = Orientation.Horizontal;
            eRoot.HorizontalAlignment = HorizontalAlignment.Left;
            eRoot.VerticalAlignment = VerticalAlignment.Top;
            Canvas c1 = new Canvas();
            c1.FlowDirection = FlowDirection.RightToLeft;
            c1.Height = 200;
            c1.Width = 200;
            c1.Background = new SolidColorBrush(Colors.CornflowerBlue);
            c1.ClipToBounds = true;

            BitmapImage image1 = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage image2 = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));

            Image img1 = new Image();
            img1.Source = image1;
            img1.Height = 200;
            img1.Width = 200;
            Canvas.SetLeft(img1, 75);
            Canvas.SetTop(img1, 75);
            Canvas c2 = new Canvas();
            c2.FlowDirection = FlowDirection.RightToLeft;
            c2.Background = new SolidColorBrush(Colors.RoyalBlue);
            c2.Height = 200;
            c2.Width = 200;
            c2.ClipToBounds = false;
            Image img2 = new Image();
            img2.Source = image2;
            img2.Height = 200;
            img2.Width = 200;
            Canvas.SetLeft(img2, 75);
            Canvas.SetTop(img2, 75);
            c1.Children.Add(img1);
            c2.Children.Add(img2);
            eRoot.Children.Add(c1);
            eRoot.Children.Add(c2);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = false;

            Point pt1 = new Point(30, 210);
            Point pt2 = new Point(190, 210);
            IInputElement myInputElement1;

            myInputElement1 = eRoot.InputHitTest(pt1);

            IInputElement myInputElement2;

            myInputElement2 = eRoot.InputHitTest(pt2);
            if (myInputElement1 == null)
            {
                if (myInputElement2.GetType().Name == "Image")
                    this.Result = true;
                else
                    Helpers.Log("Secont point hits " + myInputElement2.GetType().ToString());
            }
            else
                Helpers.Log("First point hits " + myInputElement1.GetType().ToString());
        }

    }

    //////////////////////////////////////////////////////////////////
    //  This case is testing the Canvas Clip properties
    //      
    //      ClipToBounds (Image cloud.bmp) = true;
    //      ClipToBounds (Image computer.bmp)= false;
    //
    //
    //	Verification:
    //		Point(100, 210) should NOT hit image.
    //		Point(300, 210) should hit image.
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Canvas", "CanvasClipToBounds", Variables="Area=ElementLayout")]
    public class CanvasClipToBounds : CodeTest
    {
        public CanvasClipToBounds()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        public DockPanel eRoot;

        public override FrameworkElement TestContent()
        {
            eRoot = new DockPanel();
            eRoot.LastChildFill = false;
            Canvas c1;
            Canvas c2;
            Image img1;
            Image img2;

            c1 = new Canvas();
            c1.VerticalAlignment = VerticalAlignment.Top;
            c1.Height = 200;
            c1.Width = 200;
            c1.Background = new SolidColorBrush(Colors.CornflowerBlue);
            c1.ClipToBounds = true;

            BitmapImage image1 = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            BitmapImage image2 = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));

            img1 = new Image();
            img1.Source = image1;
            img1.Height = 200;
            img1.Width = 200;
            Canvas.SetLeft(img1, 75);
            Canvas.SetTop(img1, 75);

            c2 = new Canvas();
            c2.VerticalAlignment = VerticalAlignment.Top;
            c2.Background = new SolidColorBrush(Colors.RoyalBlue);
            c2.Height = 200;
            c2.Width = 200;
            c2.ClipToBounds = false;

            img2 = new Image();
            img2.Source = image2;
            img2.Height = 200;
            img2.Width = 200;
            Canvas.SetLeft(img2, 75);
            Canvas.SetTop(img2, 75);
            DockPanel.SetDock(c1, Dock.Left);
            DockPanel.SetDock(c2, Dock.Left);

            c1.Children.Add(img1);
            c2.Children.Add(img2);
            eRoot.Children.Add(c1);
            eRoot.Children.Add(c2);
            return eRoot;
        }
        public override void TestVerify()
        {
            // set this.Result..
            this.Result = false;

            Point pt1 = new Point(100, 210);
            Point pt2 = new Point(300, 210);
            IInputElement myInputElement1;

            myInputElement1 = eRoot.InputHitTest(pt1);

            IInputElement myInputElement2;

            myInputElement2 = eRoot.InputHitTest(pt2);
            if (myInputElement1 == null)
            {
                if (myInputElement2.GetType().Name == "Image")
                    this.Result = true;
                else
                    Helpers.Log("Second point hits " + myInputElement2.GetType().ToString());
            }
            else
                Helpers.Log("First point hits " + myInputElement1.GetType().ToString());
        }

    }

    [Test(0, "Panels.Canvas", "CanvasClip", Variables="Area=ElementLayout")]
    public class CanvasClip : CodeTest
    {

        public CanvasClip()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        public Canvas canvas;

        public override FrameworkElement TestContent()
        {
            eRoot = new DockPanel();
            eRoot.LastChildFill = false;
            eRoot.Background = new SolidColorBrush(Colors.Violet);
            canvas = new Canvas();
            canvas.VerticalAlignment = VerticalAlignment.Top;
            canvas.Background = new SolidColorBrush(Colors.Yellow);

            canvas.Height = 500;
            canvas.Width = 500;
            canvas.Clip = new EllipseGeometry(new Point(250, 250), 250, 250);
            DockPanel.SetDock(canvas, Dock.Left);
            eRoot.Children.Add(canvas);

            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = true;
            double[,] point = { { 73, 73 }, { 427, 73 }, { 73, 427 }, { 427, 427 } };
            for (int r = 0; r < 4; r++)
            {
                Point pt = new Point(point[r, 0], point[r, 1]);
                IInputElement hitElement = eRoot.InputHitTest(pt);
                Helpers.Log("Point (" + point[r, 0].ToString() + "," + point[r, 1].ToString() + ") hits " + hitElement.GetType().ToString());
                if (hitElement.GetType().Name != "DockPanel") this.Result = false;
            }
        }

    }
}
