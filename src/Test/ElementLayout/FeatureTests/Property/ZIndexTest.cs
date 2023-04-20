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
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using ElementLayout.TestLibrary;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Test;
using Microsoft.Test.Layout.TestTypes;
using System.IO;

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This will load and run all ZIndex code BVT's.
    ///
    /// Possible Tests
    /// - ZIndexCanvas
    /// - ZIndexDockPanel
    /// - ZIndexDockPanelLcfFalse
    /// - ZIndexGrid
    /// - ZIndexStackPanelHoriz
    /// - ZIndexStackPanelVert
    /// - ZIndexWrapPanelHoriz
    /// 
    /// - ZIndexWrapPanelVert
    /// 
    /// - ZIndexBasic
    /// - ZIndexExceptions
    /// - ZIndexOnPanels
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(2, "Property.ZIndex", "ZIndexCanvas", Variables="Area=ElementLayout")]
    public class ZIndexCanvas : CodeTest
    {
        public ZIndexCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            string xamlfile = "ZIndexCanvas.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexDockPanel", Variables="Area=ElementLayout")]
    public class ZIndexDockPanel : CodeTest
    {
        public ZIndexDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            string xamlfile = "ZIndexDockPanel.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexDockPanelLcfFalse", Variables="Area=ElementLayout")]
    public class ZIndexDockPanelLcfFalse : CodeTest
    {
        public ZIndexDockPanelLcfFalse()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ZIndexDockPanel_lcffalse.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexGrid", Variables="Area=ElementLayout")]
    public class ZIndexGrid : CodeTest
    {
        public ZIndexGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ZIndexGrid.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexStackPanelHoriz", Variables="Area=ElementLayout")]
    public class ZIndexStackPanelHoriz : CodeTest
    {
        public ZIndexStackPanelHoriz()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ZIndexStackPanel_horiz.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexStackPanelVert", Variables="Area=ElementLayout")]
    public class ZIndexStackPanelVert : CodeTest
    {
        public ZIndexStackPanelVert()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ZIndexStackPanel_vert.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexWrapPanelHoriz", Variables="Area=ElementLayout")]
    public class ZIndexWrapPanelHoriz : CodeTest
    {
        public ZIndexWrapPanelHoriz()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            string xamlfile = "ZIndexWrapPanel_horiz.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(2, "Property.ZIndex", "ZIndexWrapPanelVert", Variables="Area=ElementLayout")]
    public class ZIndexWrapPanelVert : CodeTest
    {
        public ZIndexWrapPanelVert()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "ZIndexWrapPanel_vert.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        Panel _root_target;
        Button _one;
        Button _two;
        Button _three;
        Button _four;
        Button _five;

        public override void TestActions()
        {
            if (this.window.Content is Panel)
            {
                _root_target = this.window.Content as Panel;
            }
            else
            {
                _tempresult = false;
                Helpers.Log("Test failed to find root element of type Panel...");
                //exit test
            }

            _one = LogicalTreeHelper.FindLogicalNode(_root_target, "one_btn") as Button;
            _two = LogicalTreeHelper.FindLogicalNode(_root_target, "two_btn") as Button;
            _three = LogicalTreeHelper.FindLogicalNode(_root_target, "three_btn") as Button;
            _four = LogicalTreeHelper.FindLogicalNode(_root_target, "four_btn") as Button;
            _five = LogicalTreeHelper.FindLogicalNode(_root_target, "five_btn") as Button;

            if (_one == null || _two == null || _three == null || _four == null || _five == null)
            {
                Helpers.Log("test buttons are null..");
                _tempresult = false;
                //end test
            }
            else
            {
                _one.Click += new RoutedEventHandler(btn_Click);
                _two.Click += new RoutedEventHandler(btn_Click);
                _three.Click += new RoutedEventHandler(btn_Click);
                _four.Click += new RoutedEventHandler(btn_Click);
                _five.Click += new RoutedEventHandler(btn_Click);

                Point test_point = new Point((_root_target.ActualWidth / 2), (_root_target.ActualHeight / 2));

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }

                CommonFunctionality.FlushDispatcher();

                if (_tempresult)
                {
                    UserInput.MouseLeftDown(_root_target, (int)test_point.X, (int)test_point.Y);
                    UserInput.MouseLeftUp(_root_target, (int)test_point.X, (int)test_point.Y);
                }
            }
        }

        int _clickcount = 5;

        void btn_Click(object sender, RoutedEventArgs e)
        {
            if (_clickcount == 5)
            {
                if (((Button)sender).Content.ToString() == "five")
                {
                    Helpers.Log("Action 1 : correct button on top..");
                    Panel.SetZIndex(_five, -5);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 1 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 4)
            {
                if (((Button)sender).Content.ToString() == "four")
                {
                    Helpers.Log("Action 2 : correct button on top..");
                    Panel.SetZIndex(_four, -4);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 2 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 3)
            {
                if (((Button)sender).Content.ToString() == "three")
                {
                    Helpers.Log("Action 3 : correct button on top..");
                    Panel.SetZIndex(_three, -3);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 3 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 2)
            {
                if (((Button)sender).Content.ToString() == "two")
                {
                    Helpers.Log("Action 4 : correct button on top..");
                    Panel.SetZIndex(_two, -2);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 4 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 1)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Action 5 : correct button on top..");
                    Panel.SetZIndex(_four, -1);
                    CommonFunctionality.FlushDispatcher();
                    _clickcount--;
                }
                else
                {
                    Helpers.Log("Action 5 : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else if (_clickcount == 0)
            {
                if (((Button)sender).Content.ToString() == "one")
                {
                    Helpers.Log("Final Action : correct button on top..");
                }
                else
                {
                    Helpers.Log("Final Action : wrong button on top [Zindex test faild]..");
                    _tempresult = false;
                }
            }
            else
            {
                Helpers.Log("click count incorrect [Zindex test faild]...");
                _tempresult = false;
            }



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
    }

    [Test(0, "Property.ZIndex", "ZIndexBasic", Variables="Area=ElementLayout")]
    public class ZIndexBasic : CodeTest
    {
        public ZIndexBasic()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root_target = new Canvas();

            return _root_target;
        }

        Panel _root_target;

        public override void TestActions()
        {
            AddChildren(_root_target, 50, false, false);
            CommonFunctionality.FlushDispatcher();

            //validate default zindex value is 0
            foreach (UIElement child in _root_target.Children)
            {
                if (Panel.GetZIndex(child) != 0)
                {
                    Helpers.Log("Default ZIndex at child " + _root_target.Children.IndexOf(child) + " is incorrect..");
                    _tempresult = false;
                }
                else
                {
                    ((Button)child).Background = Brushes.Orange;
                    CommonFunctionality.FlushDispatcher();
                }
            }

            if (_tempresult)
                Helpers.Log("default zindex values test passed...");

            _root_target.Children.Clear();
            AddChildren(_root_target, 50, true, false);
            CommonFunctionality.FlushDispatcher();

            //verify Increasing zindex on children..
            foreach (UIElement child in _root_target.Children)
            {
                if (Panel.GetZIndex(child) != _root_target.Children.IndexOf(child))
                {
                    Helpers.Log("Increasing ZIndex at child " + _root_target.Children.IndexOf(child) + " is incorrect..");
                    _tempresult = false;
                }
                else
                {
                    ((Button)child).Background = Brushes.Orange;
                    CommonFunctionality.FlushDispatcher();
                }
            }
            if (_tempresult)
                Helpers.Log("increasing zindex values test passed...");

            _root_target.Children.Clear();
            AddChildren(_root_target, 50, true, true);
            CommonFunctionality.FlushDispatcher();

            //verify Decreasing zindex on children..
            foreach (UIElement child in _root_target.Children)
            {
                if (Panel.GetZIndex(child) != ((_root_target.Children.Count - 1) - _root_target.Children.IndexOf(child)))
                {
                    Helpers.Log("Decreasing ZIndex at child " + _root_target.Children.IndexOf(child) + " is incorrect..");
                    _tempresult = false;
                }
                else
                {
                    ((Button)child).Background = Brushes.Orange;
                    CommonFunctionality.FlushDispatcher();
                }
            }

            if (_tempresult)
                Helpers.Log("decreasing zindex values test passed...");
        }

        void AddChildren(Panel parent, int count, bool addZindex, bool isdecrease)
        {
            double left = 10;
            double top = 10;

            int z = count - 1;

            for (int i = 0; i < count; i++)
            {
                Button b = new Button();
                b.Content = i.ToString();
                b.Height = 100;
                b.Width = 100;
                Canvas.SetLeft(b, left);
                Canvas.SetTop(b, top);
                parent.Children.Add(b);
                left += 10;
                top += 10;
                if (addZindex)
                {
                    if (isdecrease)
                    {
                        Panel.SetZIndex(b, z);
                        z -= 1;
                    }
                    else
                    {
                        Panel.SetZIndex(b, i);
                    }

                }
            }
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
    }

    [Test(3, "Property.ZIndex", "ZIndexExceptions", Variables="Area=ElementLayout")]
    public class ZIndexExceptions : CodeTest
    {
        public ZIndexExceptions()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();

        }

        public override FrameworkElement TestContent()
        {
            _root_target = new Canvas();

            return _root_target;
        }

        Panel _root_target;

        public override void TestActions()
        {
            //exception tests..

            _root_target.Children.Clear();
            Button btn = null;
            CommonFunctionality.FlushDispatcher();

            //set with null element
            try
            {
                Panel.SetZIndex(btn, 10);
                _tempresult = false;
                Helpers.Log("Exception should be thrown when setting ZIndex on null element...");
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("ArgunmentNullException caught setting ZIndex...");
            }

            //get with null element
            try
            {
                Panel.GetZIndex(btn);
                _tempresult = false;
                Helpers.Log("Exception should be thrown when getting ZIndex on null element...");
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("ArgunmentNullException caught getting ZIndex...");
            }

            btn = new Button();
            _root_target.Children.Add(btn);

            //set with non int value
            try
            {
                btn.SetValue(Panel.ZIndexProperty, 10.5);
                _tempresult = false;
                Helpers.Log("Exception should be thrown when setting ZIndex to non-int...");
            }
            catch (Exception)
            {
                Helpers.Log("Exception caught setting ZIndex to non-int...");
            }

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
    }

    [Test(1, "Property.ZIndex", "ZIndexOnPanels", Variables="Area=ElementLayout")]
    public class ZIndexOnPanels : CodeTest
    {
        public ZIndexOnPanels()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();

        }

        public override FrameworkElement TestContent()
        {
            _root_target = new Grid();

            return _root_target;
        }

        Panel _root_target;

        public override void TestActions()
        {
            #region DockPanel ZIndex Test

            DockPanel d = new DockPanel();
            Button d_child = new Button();
            d_child.Content = "DockPanel Test.";
            d.Children.Add(d_child);
            _root_target.Children.Add(d);

            try
            {
                DockPanel.SetZIndex(d_child, 10);
                Helpers.Log("PASS : DockPanel.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : DockPanel.SetZIndex");
            }

            if (DockPanel.GetZIndex(d_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : DockPanel.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : DockPanel.GetZIndex");
            }

            #endregion

            _root_target.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            #region WrapPanel ZIndex Test

            WrapPanel wp = new WrapPanel();
            Button wp_child = new Button();
            wp_child.Content = "WrapPanel Test.";
            wp.Children.Add(wp_child);
            _root_target.Children.Add(wp);

            try
            {
                WrapPanel.SetZIndex(wp_child, 10);
                Helpers.Log("PASS : WrapPanel.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : WrapPanel.SetZIndex");
            }

            if (WrapPanel.GetZIndex(wp_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : WrapPanel.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : WrapPanel.GetZIndex");
            }

            #endregion

            _root_target.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            #region StackPanel ZIndex Test

            StackPanel sp = new StackPanel();
            Button sp_child = new Button();
            sp_child.Content = "StackPanel Test.";
            sp.Children.Add(sp_child);
            _root_target.Children.Add(sp);

            try
            {
                StackPanel.SetZIndex(sp_child, 10);
                Helpers.Log("PASS : StackPanel.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : StackPanel.SetZIndex");
            }

            if (StackPanel.GetZIndex(sp_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : StackPanel.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : StackPanel.GetZIndex");
            }

            #endregion

            _root_target.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            #region Grid ZIndex Test

            Grid g = new Grid();
            Button g_child = new Button();
            g_child.Content = "Grid Test.";
            g.Children.Add(g_child);
            _root_target.Children.Add(g);

            try
            {
                Grid.SetZIndex(g_child, 10);
                Helpers.Log("PASS : Grid.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : Grid.SetZIndex");
            }

            if (Grid.GetZIndex(g_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : Grid.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : Grid.GetZIndex");
            }

            #endregion

            _root_target.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            #region Canvas ZIndex Test

            Canvas c = new Canvas();
            Button c_child = new Button();
            c_child.Content = "Canvas Test.";
            c.Children.Add(c_child);
            _root_target.Children.Add(c);

            try
            {
                Canvas.SetZIndex(c_child, 10);
                Helpers.Log("PASS : Canvas.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : Canvas.SetZIndex");
            }

            if (Canvas.GetZIndex(c_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : Canvas.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : Canvas.GetZIndex");
            }

            #endregion

            _root_target.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            #region CustomPanel ZIndex Test

            CustomPanel cp = new CustomPanel();
            Button cp_child = new Button();
            cp_child.Content = "Panel Test.";
            cp.Children.Add(cp_child);
            _root_target.Children.Add(cp);

            try
            {
                CustomPanel.SetZIndex(cp_child, 10);
                Helpers.Log("PASS : CustomPanel.SetZIndex");
            }
            catch (Exception)
            {
                _tempresult = false;
                Helpers.Log("FAIL : CustomPanel.SetZIndex");
            }

            if (CustomPanel.GetZIndex(cp_child) == 0)
            {
                _tempresult = false;
                Helpers.Log("FAIL : CustomPanel.GetZIndex return wrong value.");
            }
            else
            {
                Helpers.Log("PASS : CustomPanel.GetZIndex");
            }

            #endregion
        }

        public class CustomPanel : Panel
        {
            public CustomPanel()
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
    }
}
