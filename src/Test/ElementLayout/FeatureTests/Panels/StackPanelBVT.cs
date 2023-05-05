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

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Threading;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all StackPanel code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - DefaultOrientation
    /// - HorizontalOrientation
    /// - VerticalOrientation
    /// - ChildAlignments
    /// - MaxHeightWidth
    /// - MinHeightWidth
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.StackPanel", "StackPanelDefaultOrientation", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class StackPanelDefaultOrientation : CodeTest
    {
        public StackPanelDefaultOrientation() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Rectangle _cOne,_cTwo,_cThree,_cFour;
        public override FrameworkElement TestContent()
        {
            StackPanel root = new StackPanel();
            root.Background = Brushes.CornflowerBlue;

            _cOne = CommonFunctionality.CreateRectangle(100, 100, Brushes.Yellow);
            root.Children.Add(_cOne);

            _cTwo = CommonFunctionality.CreateRectangle(100, 100, Brushes.Red);
            root.Children.Add(_cTwo);

            _cThree = CommonFunctionality.CreateRectangle(100, 100, Brushes.Green);
            root.Children.Add(_cThree);

            _cFour = CommonFunctionality.CreateRectangle(100, 100, Brushes.Gray);
            root.Children.Add(_cFour);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            DispatcherHelper.DoEvents(1000);
            VScanCommon vscan = new VScanCommon(this);
            this.Result = vscan.CompareImage();
        }
    }

    [Test(0, "Panels.StackPanel", "StackPanelHorizontalOrientation", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class StackPanelHorizontalOrientation : CodeTest
    {
        public StackPanelHorizontalOrientation() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Rectangle _cOne,_cTwo,_cThree,_cFour;
        public override FrameworkElement TestContent()
        {
            StackPanel root = new StackPanel();
            root.Background = Brushes.CornflowerBlue;
            root.Orientation = Orientation.Horizontal;

            _cOne = CommonFunctionality.CreateRectangle(100, 100, Brushes.Yellow);
            root.Children.Add(_cOne);

            _cTwo = CommonFunctionality.CreateRectangle(100, 100, Brushes.Red);
            root.Children.Add(_cTwo);

            _cThree = CommonFunctionality.CreateRectangle(100, 100, Brushes.Green);
            root.Children.Add(_cThree);

            _cFour = CommonFunctionality.CreateRectangle(100, 100, Brushes.Gray);
            root.Children.Add(_cFour);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            DispatcherHelper.DoEvents(1000);
            VScanCommon vscan = new VScanCommon(this);
            this.Result = vscan.CompareImage();
        }
    }

    [Test(0, "Panels.StackPanel", "StackPanelVerticalOrientation", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class StackPanelVerticalOrientation : CodeTest
    {
        public StackPanelVerticalOrientation() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Rectangle _cOne,_cTwo,_cThree,_cFour;
        public override FrameworkElement TestContent()
        {
            StackPanel root = new StackPanel();
            root.Background = Brushes.CornflowerBlue;
            root.Orientation = Orientation.Vertical;

            _cOne = CommonFunctionality.CreateRectangle(100, 100, Brushes.Yellow);
            root.Children.Add(_cOne);

            _cTwo = CommonFunctionality.CreateRectangle(100, 100, Brushes.Red);
            root.Children.Add(_cTwo);

            _cThree = CommonFunctionality.CreateRectangle(100, 100, Brushes.Green);
            root.Children.Add(_cThree);

            _cFour = CommonFunctionality.CreateRectangle(100, 100, Brushes.Gray);
            root.Children.Add(_cFour);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            DispatcherHelper.DoEvents(1000);
            VScanCommon vscan = new VScanCommon(this);
            this.Result = vscan.CompareImage();
        }
    }

    [Test(0, "Panels.StackPanel", "StackPanelChildAlignments", Variables = "Area=ElementLayout")]
    public class StackPanelChildAlignments : CodeTest
    {
        public StackPanelChildAlignments() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Rectangle _rect;
        StackPanel _root;
        public override FrameworkElement TestContent()
        {
            _root = new StackPanel();
            _root.Background = Brushes.CornflowerBlue;
            _root.Orientation = Orientation.Vertical;

            _rect = new Rectangle();
            _rect.Fill = Brushes.Gray;
            _root.Children.Add(_rect);

            return _root;
        }

        public override void TestActions()
        {
            _rect.Height = 100;
            _rect.Width = 100;
            _rect.HorizontalAlignment = HorizontalAlignment.Right;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.HorizontalAlignment = HorizontalAlignment.Center;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.HorizontalAlignment = HorizontalAlignment.Left;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.Width = double.NaN;
            _rect.HorizontalAlignment = HorizontalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _root.Orientation = Orientation.Horizontal;
            _rect.Height = 100;
            _rect.Width = 100;
            _rect.VerticalAlignment = VerticalAlignment.Top;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.VerticalAlignment = VerticalAlignment.Center;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.VerticalAlignment = VerticalAlignment.Bottom;
            CommonFunctionality.FlushDispatcher();
            Validate();

            _rect.Height = double.NaN;
            _rect.VerticalAlignment = VerticalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();
            Validate();
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

        void Validate()
        {
            Point rectLoc = LayoutUtility.GetElementPosition(_rect, _root);
            Size rootSize = new Size(_root.ActualWidth, _root.ActualHeight);
            Size rectSize = new Size(_rect.ActualWidth, _rect.ActualHeight);


            if (_root.Orientation == Orientation.Vertical)
            {
                switch (_rect.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (rectLoc.X != 0)
                        {
                            Helpers.Log("Left Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.HorizontalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Left Alignment is correct.");
                        }
                        break;
                    case HorizontalAlignment.Center:
                        if (rectLoc.X != ((rootSize.Width / 2) - (rectSize.Width / 2)))
                        {
                            Helpers.Log("Center Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.HorizontalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Center Alignment is correct.");
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (rectLoc.X != (rootSize.Width - rectSize.Width))
                        {
                            Helpers.Log("Right Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.HorizontalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Right Alignment is correct.");
                        }
                        break;
                    case HorizontalAlignment.Stretch:
                        if (rectSize.Width != rootSize.Width)
                        {
                            Helpers.Log("Stretch Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.HorizontalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Stretch Alignment is correct.");
                        }
                        break;
                }
            }
            if (_root.Orientation == Orientation.Horizontal)
            {
                switch (_rect.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        if (rectLoc.Y != 0)
                        {
                            Helpers.Log("Top Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.VerticalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Top Alignment is correct.");
                        }
                        break;
                    case VerticalAlignment.Center:
                        if (rectLoc.Y != ((rootSize.Height / 2) - (rectSize.Height / 2)))
                        {
                            Helpers.Log("Center Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.VerticalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Center Alignment is correct.");
                        }
                        break;
                    case VerticalAlignment.Bottom:
                        if (rectLoc.Y != (rootSize.Height - rectSize.Height))
                        {
                            Helpers.Log("Bottom Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.VerticalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Bottom Alignment is correct.");
                        }
                        break;
                    case VerticalAlignment.Stretch:
                        if (rectSize.Height != rootSize.Height)
                        {
                            Helpers.Log("Stretch Alignment not correct.");
                            Helpers.Log("Test Failed : " + _rect.VerticalAlignment + ", " + _root.Orientation);
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Stretch Alignment is correct.");
                        }
                        break;
                }
            }
        }
    }

    [Test(0, "Panels.StackPanel", "StackPanelMaxHeightWidth", Variables = "Area=ElementLayout")]
    public class StackPanelMaxHeightWidth : CodeTest
    {
        public StackPanelMaxHeightWidth() { }

        public override void WindowSetup()
        {
            this.window.Height = 200;
            this.window.Width = 200;
            this.window.Content = this.TestContent();
        }

        StackPanel _stackpanel;
        Grid _root;
        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stackpanel = new StackPanel();

            _stackpanel.Background = new SolidColorBrush(Colors.DarkOrange);
            _stackpanel.MaxHeight = 200;
            _stackpanel.MaxWidth = 200;

            _root.Children.Add(_stackpanel);
            return _root;
        }

        public override void TestActions()
        {
            this.window.Height = 800;
            this.window.Width = 800;
        }

        public override void TestVerify()
        {
            if (_stackpanel.ActualHeight != 200 || _stackpanel.ActualWidth != 200)
            {
                Helpers.Log("Test Failed..  Max Height or Max Width not enforced...");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Test Passed..");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.StackPanel", "StackPanelMinHeightWidth", Variables = "Area=ElementLayout")]
    public class StackPanelMinHeightWidth : CodeTest
    {
        public StackPanelMinHeightWidth() { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        StackPanel _stackpanel;
        Grid _root;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stackpanel = new StackPanel();
            _stackpanel.Background = new SolidColorBrush(Colors.DarkOrange);
            _stackpanel.MinHeight = 200;
            _stackpanel.MinWidth = 200;

            _root.Children.Add(_stackpanel);
            return _root;
        }

        public override void TestActions()
        {
            this.window.Height = 150;
            this.window.Width = 150;
        }

        public override void TestVerify()
        {
            if (_stackpanel.ActualHeight != 200 || _stackpanel.ActualWidth != 200)
            {
                Helpers.Log("Test Failed..  Min Height or Min Width not enforced...");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Test Passed..");
                this.Result = true;
            }
        }
    }
}
