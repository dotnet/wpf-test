// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

#endregion

namespace ElementLayout.FeatureTests.LayoutSystem
{
    //////////////////////////////////////////////////////////////////
    /// This will contains all code for Layout System BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - LSTestSortingDirtyQueues
    /// - LSOnMeasureVerification
    /// - LSOnArrangeVerification
    /// - LSNonLayoutAffectingProperty
    /// - LSAffectRender
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "LayoutSystem", "LSTestSortingDirtyQueues", Variables = "Area=ElementLayout")]
    public class LSTestSortingDirtyQueues : CodeTest
    {
        public LSTestSortingDirtyQueues()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot1,_myFlowPanel1;
       public static bool measureflag = false;
        public static bool arrangeflag = false;
        Image _myImage1;
       LSTestSortingDirtyQueues.FileBlockFlowerPanel _eRoot2;

        public override FrameworkElement TestContent()
        {
            DockPanel myDockPanel = new DockPanel();
            _eRoot1 = new Grid();
            _eRoot2 = new FileBlockFlowerPanel();
            _eRoot1.SetValue(DockPanel.DockProperty, Dock.Left);

            _eRoot2.SetValue(DockPanel.DockProperty, Dock.Right);

            myDockPanel.Children.Add(_eRoot1);
            myDockPanel.Children.Add(_eRoot2);
            Image myImage = new Image();
            BitmapImage myimagedata = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            myImage.Source = myimagedata;
            _eRoot1.Children.Add(myImage);
            _myFlowPanel1 = new Grid();
            _eRoot2.Children.Add(_myFlowPanel1);
            _myImage1 = new Image();

            BitmapImage myimagedata1 = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            _myImage1.Source = myimagedata1;
            _myFlowPanel1.Children.Add(_myImage1);

            return myDockPanel;
        }

        public override void TestActions()
        {
            _myImage1.Width = 200;
            _myImage1.Height = 200;
            _myFlowPanel1.Width = 200;
            _myFlowPanel1.Height = 200;
            _eRoot2.Height = 200;
            _eRoot2.Width = 200;
            _eRoot1.Height = 200;
            _eRoot1.Width = 200;
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            if (arrangeflag == false && measureflag == false)
            {
                Helpers.Log("Measure and Arrange flags are false.");
                this.Result = true;
            }
            else
            {
                this.Result = false;
            }
        }

        public class FileBlockFlowPanel : FrameworkElement
        {
            private BitmapImage _image;           // The actual image used to measure/arrange/render.

            public BitmapImage BitmapImage
            {
                get { return _image; }
                set { _image = value; }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                return new Size(200, 55);
            }


            /// <summary>
            /// Render control's content.
            /// </summary>
            /// <param name="ctx">Drawing context.</param>
            protected override void OnRender(DrawingContext ctx)
            {
                int imagestartpointleft = 5;
                int imagestartpointtop = 7;
                int iconimageheight = 42;
                int iconimagewidth = 42;
                int fontheight = (iconimageheight - 4) / 3;//use system font.....
                int secondtextstarttop = imagestartpointtop + fontheight + 2;// 2 ia the scale which can be made vaiable later
                int thirdtextstarttop = imagestartpointtop + 2 * (fontheight + 2);// 4 ia the scale which can be made vaiable later

                if (_image != null)
                {
                    ctx.DrawImage(_image, new Rect(imagestartpointleft, imagestartpointtop, imagestartpointleft + iconimagewidth, imagestartpointtop + iconimagewidth), null);
                }
            }

            /// <summary>
            /// Property invalidation callback.
            /// </summary>
            /// <param name="id">DependencyID (native or attached) that represents the property.</param>
        }

        public class FileBlockFlowerPanel : Panel
        {
            public FileBlockFlowerPanel()
                : base()
            {
            }

            protected override Size MeasureOverride(Size constraint)
            {
                Size szChildMax = new Size(0, 0);
                double childleft = 0;
                double childtop = 0;
                double heightcount = 0;
                double widthcount = 0;
                double myWidth = 0;
                double myHeight = 0;
                int i = -1;
                if (LSTestSortingDirtyQueues.measureflag == false)
                {
                    foreach (UIElement child in Children)
                    {
                        i++;

                        Vector childTotalSize = new Vector();   
                        Size childConstraint = constraint;
                        Size childSize;
                        child.Measure(childConstraint);
                        childSize = child.DesiredSize;

                        childTotalSize += (Vector)childSize;
                        if (child != null)
                        {
                            Size szChildCalc;

                            szChildCalc = childSize;
                            if ((childleft + szChildCalc.Width > constraint.Width) && (i != 0))
                            {
                                heightcount++;
                                childleft = 0;
                                childtop = childtop + szChildCalc.Height;
                                childleft = childleft + szChildCalc.Width;
                                widthcount = 1;
                                myWidth = ((widthcount * szChildCalc.Width) > (myWidth)) ? (widthcount * szChildCalc.Width) : (myWidth);
                            }
                            else
                            {
                                childleft = childleft + szChildCalc.Width;
                                widthcount++;
                                heightcount++;
                                myWidth = ((widthcount * szChildCalc.Width) > (myWidth)) ? (widthcount * szChildCalc.Width) : (myWidth);
                            }

                            myHeight = myHeight + szChildCalc.Height;
                        }
                    }
                    LSTestSortingDirtyQueues.measureflag = true;
                    return (new Size(myWidth, myHeight));
                }
                else
                {
                    LSTestSortingDirtyQueues.measureflag = false;
                    return (this.DesiredSize);
                }
            }

            /// <summary>
            /// Content arrangement.
            /// </summary>
            protected override Size ArrangeOverride(Size arrangeSize)
            {
                Size szChildMax = new Size(0, 0);
                double childleft = 0;
                double childtop = 0;
                int i = -1;

                if (LSTestSortingDirtyQueues.arrangeflag == false)
                {
                    foreach (UIElement child in Children)
                    {
                        i++;
                        if (child != null)
                        {
                            Size szChildCalc;

                            szChildCalc = child.RenderSize;
                            if ((childleft + szChildCalc.Width > arrangeSize.Width) && (i != 0))
                            {
                                childleft = 0;
                                childtop = childtop + szChildCalc.Height;
                                child.Arrange(new Rect(childleft, childtop, szChildCalc.Width, szChildCalc.Height));
                                childleft = childleft + szChildCalc.Width;
                            }
                            else
                            {
                                child.Arrange(new Rect(childleft, childtop, szChildCalc.Width, szChildCalc.Height));
                                childleft = childleft + szChildCalc.Width;
                            }
                        }
                    }
                    LSTestSortingDirtyQueues.arrangeflag = true;
                }
                else
                {
                    LSTestSortingDirtyQueues.arrangeflag = false;

                }
                return arrangeSize;
            }

            /// <summary>
            /// Render control's content.
            /// </summary>
            /// <param name="ctx">Drawing context.</param>
            protected override void OnRender(DrawingContext ctx)
            {
            }

            /// <summary>
            /// Property invalidation callback.
            /// </summary>
            /// <param name="id">DependencyID (native or attached) that represents the property.</param>
        }
    }

    [Test(0, "LayoutSystem", "LSOnMeasureVerification", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class LSOnMeasureVerification : CodeTest
    {
        public LSOnMeasureVerification()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Image _myFlowPanel;
        MyBorder _myBorder;

        static bool s_measure = false,s_arrange = false,s_render = false;
        static int s_counter = 0;

        public override FrameworkElement TestContent()
        {
            Canvas eRoot = new Canvas();

            eRoot.Height = 500;
            eRoot.Width = 700;
            _myBorder = new MyBorder();
            _myBorder.BorderThickness = new Thickness(5);
            _myBorder.Background = new SolidColorBrush(Colors.Green);
            eRoot.Children.Add(_myBorder);

            BitmapImage image = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));

            _myFlowPanel = new Image();
            _myFlowPanel.Source = image;
            _myBorder.Child = _myFlowPanel;
            return eRoot;
        }

        public override void TestActions()
        {
            s_measure = false;
            s_arrange = false;
            s_render = false;
            _myBorder.Background = new SolidColorBrush(Colors.Red);
            _myBorder.BorderThickness = new Thickness(10);
            s_counter = 0;
        }

        public override void TestVerify()
        {
            if ((s_measure == true) && (s_arrange == true) && (s_render == true) && (s_counter == 3))
                this.Result = true;
            else
                this.Result = false;
        }

        public class MyBorder : Border
        {
            protected override void OnRender(DrawingContext dc)
            {
                LSOnMeasureVerification.s_counter++;
                LSOnMeasureVerification.s_render = true;
                base.OnRender(dc);
            }

            protected override Size MeasureOverride(Size constraint)
            {
                LSOnMeasureVerification.s_counter++;
                LSOnMeasureVerification.s_measure = true;
                return (base.MeasureOverride(constraint));
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                LSOnMeasureVerification.s_counter++;
                LSOnMeasureVerification.s_arrange = true;
                return base.ArrangeOverride(arrangeSize);
            }
        }
    }

    [Test(0, "LayoutSystem", "LSOnArrangeVerification", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class LSOnArrangeVerification : CodeTest
    {
        public LSOnArrangeVerification()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Image _myFlowPanel;
        private MyCanvas _eRoot;
        private Border _myBorder;
        static bool s_measure = false,s_arrange = false,s_render = false;

        public override FrameworkElement TestContent()
        {
            _eRoot = new MyCanvas();

            _eRoot.Height = 500;
            _eRoot.Width = 700;
            _myBorder = new Border();
            _myBorder.SetValue(Canvas.TopProperty, 100d);
            _myBorder.SetValue(Canvas.LeftProperty, 100d);

            _eRoot.Children.Add(_myBorder);

            BitmapImage image = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));

            _myFlowPanel = new Image();
            _myFlowPanel.Source = image;
            _myBorder.Child = _myFlowPanel;
            return _eRoot;
        }

        public override void TestActions()
        {
            s_measure = false;
            s_arrange = false;
            s_render = false;
            _myBorder.SetValue(Canvas.TopProperty, 200d);
            _myBorder.SetValue(Canvas.LeftProperty, 200d);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            if ((s_measure == false) && (s_arrange == true) && (s_render == false))
                this.Result = true;
            else
                this.Result = false;
        }

        public class MyCanvas : Canvas
        {
            protected override void OnRender(DrawingContext dc)
            {
                LSOnArrangeVerification.s_render = true;
                base.OnRender(dc);
            }

            protected override Size MeasureOverride(Size constraint)
            {
                LSOnArrangeVerification.s_measure = true;
                return (base.MeasureOverride(constraint));
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                LSOnArrangeVerification.s_arrange = true;
                return base.ArrangeOverride(arrangeSize);
            }
        }
    }

    [Test(0, "LayoutSystem", "LSNonLayoutAffectingProperty", Variables="Area=ElementLayout")]
    public class LSNonLayoutAffectingProperty : CodeTest
    {
        public LSNonLayoutAffectingProperty()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Image _myFlowPanel;
        private MyBorder _myBorder;
        static bool s_measure = false,s_arrange = false,s_render = false;
        static int s_counter = 0;

        public override FrameworkElement TestContent()
        {
            Canvas eRoot = new Canvas();

            eRoot.Height = 500;
            eRoot.Width = 700;
            _myBorder = new MyBorder();
            _myBorder.BorderThickness = new Thickness(5);
            _myBorder.Background = new SolidColorBrush(Colors.Green);
            eRoot.Children.Add(_myBorder);

            BitmapImage image = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));

            _myFlowPanel = new Image();
            _myFlowPanel.Source = image;
            _myBorder.Child = _myFlowPanel;
            return eRoot;
        }

        public override void TestActions()
        {
            s_measure = false;
            s_arrange = false;
            s_render = false;
            _myFlowPanel.Focus();
            s_counter = 0;
        }

        public override void TestVerify()
        {
            if ((s_measure == false) && (s_arrange == false) && (s_render == false) && (s_counter == 0))
                this.Result = true;
            else
                this.Result = false;
        }

        public class MyBorder : Border
        {
            protected override void OnRender (DrawingContext dc)
            {
                LSNonLayoutAffectingProperty.s_counter++;
                LSNonLayoutAffectingProperty.s_render = true;
                base.OnRender (dc);
            }

            protected override Size MeasureOverride (Size constraint)
            {
                LSNonLayoutAffectingProperty.s_counter++;
                LSNonLayoutAffectingProperty.s_measure = true;
                return (base.MeasureOverride (constraint));
            }

            protected override Size ArrangeOverride (Size arrangeSize)
            {
                LSNonLayoutAffectingProperty.s_counter++;
                LSNonLayoutAffectingProperty.s_arrange = true;
                return base.ArrangeOverride (arrangeSize);
            }
        }
    }

    [Test(0, "LayoutSystem", "LSAffectRender", Variables="Area=ElementLayout")]
    public class LSAffectRender : CodeTest
    {
        public LSAffectRender()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Image _myFlowPanel;
        private MyBorder _myBorder;
        static bool s_measure = false,s_arrange = false,s_render = false;

        public override FrameworkElement TestContent()
        {
            Canvas eRoot = new Canvas ();
            eRoot.Height = 500;
            eRoot.Width = 700;
            _myBorder = new MyBorder ();
            _myBorder.Background = new SolidColorBrush (Colors.Green);
            eRoot.Children.Add (_myBorder);
            BitmapImage image = new BitmapImage(new Uri("cloud.bmp", UriKind.RelativeOrAbsolute));
            _myFlowPanel = new Image ();
            _myFlowPanel.Source = image;
            _myBorder.Child = _myFlowPanel;
            return eRoot;
        }

        public override void TestActions()
        {
            s_measure = false;
            s_arrange = false;
            s_render = false;
            _myBorder.Background = new SolidColorBrush(Colors.Red);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            if ((s_measure == false) && (s_arrange == true) && (s_render == true))
                this.Result = true;
            else
                this.Result = false;
        }

        public class MyBorder : Border
        {
            protected override void OnRender(DrawingContext dc)
            {
                LSAffectRender.s_render = true;
                base.OnRender(dc);

            }
            protected override Size MeasureOverride(Size constraint)
            {
                LSAffectRender.s_measure = true;
                return (base.MeasureOverride(constraint));
            }

            protected override Size ArrangeOverride(Size arrangeSize)
            {
                LSAffectRender.s_arrange = true;
                return base.ArrangeOverride(arrangeSize);
            }

        }
    }
}
