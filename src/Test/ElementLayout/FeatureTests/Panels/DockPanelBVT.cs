// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all DockPanel code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - ChildNaturalSizePlusMargins
    /// - FillStretch
    /// - LeftRightStretch
    /// - TopBottomStretch
    /// - MaxHeightWidth
    /// - MinHeightWidth
    /// - MixedDocking
    /// - Resize1
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.DockPanel", "DockPanelFillStretch", Variables="Area=ElementLayout")]
    public class DockPanelFillStretch : CodeTest
    {


        public DockPanelFillStretch()
        {
        }

        public override void WindowSetup()
        {

            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _mainDock;
        DockPanel _fillDock;

        public override FrameworkElement TestContent()
        {
            Border root = new Border();
            root.Background = new SolidColorBrush(Colors.White);
            Border fillBorder = new Border();
            fillBorder.Background = new SolidColorBrush(Colors.DarkRed);

            _mainDock = new DockPanel();
            _mainDock.LastChildFill = true;
            _fillDock = new DockPanel();

            //adding border elements to dockpanels
            fillBorder.Child = _fillDock;

            //main parent dockpanel
            _mainDock.Height = 500;
            _mainDock.Width = 700;

            root.Child = _mainDock;
            _mainDock.Children.Add(fillBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_fillDock.RenderSize.Height != _mainDock.RenderSize.Height || _fillDock.RenderSize.Width != _mainDock.RenderSize.Width)
            {
                this.Result = false;
                Helpers.Log("FILL DOCKPANEL HEIGHT OR WIDTH WAS INCORRECT...");
            }
            else
            {
                this.Result = true;
                Helpers.Log("FILL DOCKPANEL: stretching is correct, height=" + _fillDock.RenderSize.Height + " width=" + _fillDock.RenderSize.Width);
            }
        }
    }

    [Test(0, "Panels.DockPanel", "DockPanelLeftRightStretch", Variables="Area=ElementLayout")]
    public class DockPanelLeftRightStretch : CodeTest
    {


        public DockPanelLeftRightStretch()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _mainDock;
        DockPanel _leftDock;
        DockPanel _rightDock;
        public override FrameworkElement TestContent()
        {
            Border mainBorder = new Border(); mainBorder.Background = new SolidColorBrush(Colors.White);
            Border leftBorder = new Border(); leftBorder.Background = new SolidColorBrush(Colors.DarkRed);
            Border rightBorder = new Border(); rightBorder.Background = new SolidColorBrush(Colors.MediumTurquoise);

            _mainDock = new DockPanel();
            _leftDock = new DockPanel();
            _rightDock = new DockPanel();

            rightBorder.Child = _rightDock;
            leftBorder.Child = _leftDock;

            //main parent dockpanel
            _mainDock.Height = 500;
            _mainDock.Width = 700;

            //top dockpanel
            _leftDock.Width = 200;
            DockPanel.SetDock(leftBorder, Dock.Left);


            //bottom dockpanel
            _rightDock.Width = 200;
            DockPanel.SetDock(rightBorder, Dock.Right);

            mainBorder.Child = _mainDock;
            _mainDock.Children.Add(leftBorder);
            _mainDock.Children.Add(rightBorder);

            return mainBorder;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_leftDock.RenderSize.Height != _mainDock.RenderSize.Height || _rightDock.RenderSize.Height != _mainDock.RenderSize.Height)
            {
                Helpers.Log("Left DockPanel (" + _leftDock.RenderSize.Height + ") or Right DockPanel (" + _rightDock.RenderSize.Height + ") had incorrect stretched height...");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Left DockPanel and Right DockPanel had correct heights after stretching.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.DockPanel", "DockPanelTopBottomStretch", Variables="Area=ElementLayout")]
    public class DockPanelTopBottomStretch : CodeTest
    {


        public DockPanelTopBottomStretch()
        {
        }

        public override void WindowSetup()
        {

            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _mainDock;
        DockPanel _topDock;
        DockPanel _bottomDock;

        public override FrameworkElement TestContent()
        {
            Border mainBorder = new Border();
            mainBorder.Background = new SolidColorBrush(Colors.White);
            Border topBorder = new Border();
            topBorder.Background = new SolidColorBrush(Colors.Gainsboro);
            Border bottomBorder = new Border();
            bottomBorder.Background = new SolidColorBrush(Colors.Purple);

            _mainDock = new DockPanel();
            _topDock = new DockPanel();
            _bottomDock = new DockPanel();

            bottomBorder.Child = _bottomDock;
            topBorder.Child = _topDock;

            _mainDock.Height = 500;
            _mainDock.Width = 700;

            _topDock.Height = 100;
            DockPanel.SetDock(topBorder, Dock.Top);

            _bottomDock.Height = 100;
            DockPanel.SetDock(bottomBorder, Dock.Bottom);

            mainBorder.Child = _mainDock;
            _mainDock.Children.Add(topBorder);
            _mainDock.Children.Add(bottomBorder);

            return mainBorder;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_topDock.RenderSize.Width != _mainDock.RenderSize.Width || _bottomDock.RenderSize.Width != _mainDock.RenderSize.Width)
            {
                Helpers.Log("Top DockPanel (" + _topDock.RenderSize.Height + ") or Bottom DockPanel (" + _bottomDock.RenderSize.Height + ") had incorrect stretched width...");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Top DockPanel and BottomDockPanel had correct widths after stretching.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.DockPanel", "DockPanelMaxHeightWidth", Variables="Area=ElementLayout")]
    public class DockPanelMaxHeightWidth : CodeTest
    {


        public DockPanelMaxHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 200;
            this.window.Width = 200;

            this.window.Content = this.TestContent();
        }


        DockPanel _dockpanelchild;
        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _dockpanel = new DockPanel();
            _dockpanelchild = new DockPanel();
            _dockpanelchild.Background = new SolidColorBrush(Colors.DarkOrange);
            _dockpanelchild.MaxHeight = 200;
            _dockpanelchild.MaxWidth = 200;
            _dockpanel.Children.Add(_dockpanelchild);
            return _dockpanel;
        }

        public override void TestActions()
        {
            this.window.Height = 800;
            this.window.Width = 800;
        }

        public override void TestVerify()
        {
            if (_dockpanelchild.ActualHeight != 200 || _dockpanelchild.ActualWidth != 200)
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

    [Test(0, "Panels.DockPanel", "DockPanelMinHeightWidth", Variables="Area=ElementLayout")]
    public class DockPanelMinHeightWidth : CodeTest
    {
        public DockPanelMinHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _dockpanelchild;
        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _dockpanel = new DockPanel();
            _dockpanel.Background = Brushes.Gray;

            _dockpanelchild = new DockPanel();
            _dockpanelchild.Background = new SolidColorBrush(Colors.DarkOrange);
            _dockpanelchild.MinHeight = 200;
            _dockpanelchild.MinWidth = 200;
            _dockpanel.Children.Add(_dockpanelchild);
            return _dockpanel;
        }

        public override void TestActions()
        {
            this.window.Height = 150;
            this.window.Width = 150;
        }

        public override void TestVerify()
        {
            if (_dockpanelchild.ActualHeight != 200 || _dockpanelchild.ActualWidth != 200)
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

    [Test(0, "Panels.DockPanel", "DockPanelMixedDocking", Variables="Area=ElementLayout")]
    public class DockPanelMixedDocking : CodeTest
    {


        public DockPanelMixedDocking()
        {
        }

        public override void WindowSetup()
        {

            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _mainDock;
        DockPanel _topDock;
        DockPanel _leftDock;
        DockPanel _rightDock;
        DockPanel _bottomDock;
        DockPanel _fillDock;

        public override FrameworkElement TestContent()
        {
            Border mainBorder = new Border();
            mainBorder.Background = new SolidColorBrush(Colors.White);

            Border topBorder = new Border();
            topBorder.Background = new SolidColorBrush(Colors.Gainsboro);

            Border leftBorder = new Border();
            leftBorder.Background = new SolidColorBrush(Colors.Orange);

            Border rightBorder = new Border();
            rightBorder.Background = new SolidColorBrush(Colors.Blue);

            Border bottomBorder = new Border();
            bottomBorder.Background = new SolidColorBrush(Colors.Purple);

            Border fillBorder = new Border();
            fillBorder.Background = new SolidColorBrush(Colors.Red);

            _mainDock = new DockPanel();
            _mainDock.LastChildFill = true;

            _topDock = new DockPanel();
            _leftDock = new DockPanel();
            _rightDock = new DockPanel();
            _bottomDock = new DockPanel();
            _fillDock = new DockPanel();

            //adding border elements to dockpanels
            fillBorder.Child = _fillDock;
            bottomBorder.Child = _bottomDock;
            rightBorder.Child = _rightDock;
            leftBorder.Child = _leftDock;
            topBorder.Child = _topDock;

            //main parent dockpanel
            _mainDock.Height = 600;
            _mainDock.Width = 800;

            //top dockpanel
            _topDock.Height = 60;
            DockPanel.SetDock(topBorder, Dock.Top);

            //bottom dockpanel
            _bottomDock.Height = 25;
            DockPanel.SetDock(bottomBorder, Dock.Bottom);

            //left dockpanel
            leftBorder.Width = 100;
            DockPanel.SetDock(leftBorder, Dock.Left);

            //right dockpanel
            _rightDock.Width = 200;
            DockPanel.SetDock(rightBorder, Dock.Right);

            //fill dockpanel
            mainBorder.Child = _mainDock;
            _mainDock.Children.Add(topBorder);
            _mainDock.Children.Add(bottomBorder);
            _mainDock.Children.Add(leftBorder);
            _mainDock.Children.Add(rightBorder);
            _mainDock.Children.Add(fillBorder);

            return mainBorder;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_mainDock.RenderSize.Height != 600 || _mainDock.RenderSize.Width != 800)
            {
                Helpers.Log("MAIN DOCKPANEL SIZE WAS INCORRECT: Height : " + _mainDock.RenderSize.Height + ", Width : " + _mainDock.RenderSize.Width);
                this.Result = false;
            }
            else if (_topDock.RenderSize.Height != 60 || _topDock.RenderSize.Width != 800)
            {
                Helpers.Log("TOP DOCKPANEL SIZE WAS INCORRECT: Height : " + _topDock.RenderSize.Height + ", Width : " + _topDock.RenderSize.Width);
                this.Result = false;
            }
            else if (_bottomDock.RenderSize.Height != 25 || _bottomDock.RenderSize.Width != 800)
            {
                Helpers.Log("BOTTOM DOCKPANEL SIZE WAS INCORRECT: Height : " + _bottomDock.RenderSize.Height + ", Width : " + _bottomDock.RenderSize.Width);
                this.Result = false;
            }
            else if (_leftDock.RenderSize.Height != 515 || _leftDock.RenderSize.Width != 100)
            {
                Helpers.Log("LEFT DOCKPANEL SIZE WAS INCORRECT: Height : " + _leftDock.RenderSize.Height + ", Width : " + _leftDock.RenderSize.Width);
                this.Result = false;
            }
            else if (_rightDock.RenderSize.Height != 515 || _rightDock.RenderSize.Width != 200)
            {
                Helpers.Log("RIGHT DOCKPANEL SIZE WAS INCORRECT: Height : " + _rightDock.RenderSize.Height + ", Width : " + _rightDock.RenderSize.Width);
                this.Result = false;
            }
            else if (_fillDock.RenderSize.Height != 515 || _fillDock.RenderSize.Width != 500)
            {
                Helpers.Log("FILL DOCKPANEL SIZE WAS INCORRECT: Height : " + _fillDock.RenderSize.Height + ", Width : " + _fillDock.RenderSize.Width);
                this.Result = false;
            }
            else
            {
                this.Result = true;
                Helpers.Log("All DockPanel's had correct size, Test Passed.");
            }
        }
    }

    [Test(0, "Panels.DockPanel", "DockPanelResize1", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class DockPanelResize1 : CodeTest
    {
        public DockPanelResize1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            DockPanel dock = new DockPanel();
            dock.LastChildFill = true;

            DockPanel one = new DockPanel();
            one.Background = Brushes.Crimson;
            one.Width = 100;
            DockPanel.SetDock(one, Dock.Left);

            DockPanel two = new DockPanel();
            two.Background = Brushes.CornflowerBlue;
            two.Height = 75;
            DockPanel.SetDock(two, Dock.Bottom);

            DockPanel three = new DockPanel();
            three.Background = Brushes.Orange;
            three.Height = 50;
            DockPanel.SetDock(three, Dock.Top);

            DockPanel four = new DockPanel();
            four.Background = Brushes.Salmon;
            four.Width = 50;
            DockPanel.SetDock(four, Dock.Right);

            DockPanel five = new DockPanel();
            five.Background = Brushes.Silver;

            dock.Children.Add(one);
            dock.Children.Add(two);
            dock.Children.Add(three);
            dock.Children.Add(four);
            dock.Children.Add(five);
            return dock;
        }

        int _count = 0;
        public override void TestActions()
        {
            switch (this._count)
            {
                case 0:
                    this.window.Height = this.window.RenderSize.Height / 2;
                    this.window.Width = this.window.RenderSize.Width / 2;
                    this._count++;
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    break;

                case 1:
                    this.window.Height = this.window.RenderSize.Height * 3;
                    this.window.Width = this.window.RenderSize.Width * 3;
                    this._count++;
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    break;

                case 2:
                    this.window.Height = 200;
                    this.window.Width = 1000;
                    this._count++;
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    break;

                case 3:
                    this.window.Height = 1000;
                    this.window.Width = 200;
                    this._count++;
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    break;

                case 4:
                    this.window.Height = 500;
                    this.window.Width = 500;
                    this._count++;
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    break;

                default:
                    break;
            }
        }

        public override void TestVerify()
        {
            VScanCommon vscan = new VScanCommon(this);
            
            if (!vscan.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }
}
