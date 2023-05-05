// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
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
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all Panel code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - ChildAddRemoveClear
    /// - ChildCollection
    /// - ChildSize
    /// 
    /// - MaxHeightWidth
    /// - MinHeightWidth
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.Panel", "PanelChildAddRemoveClear", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class PanelChildAddRemoveClear : CodeTest
    {


        public PanelChildAddRemoveClear()
        {
        }

        public override void WindowSetup()
        {

            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        TestPanel _panel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _panel = new TestPanel();
            _panel.Height = 400;
            _panel.Width = 400;
            _panel.Background = Brushes.CornflowerBlue;

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            //verify no child at start of the test
            if (_panel.Children.Count != 0)
            {
                Helpers.Log("Custom Panel should not have child at start of the test.");
                this.Result = false;
            }

            //add child
            _panel.Children.Add(panelChild());
            CommonFunctionality.FlushDispatcher();

            if (_panel.Children.Count != 1)
            {
                {
                    Helpers.Log("Custom Panel did not add Child.");
                    this.Result = false;
                }
            }

            //remove child

            _panel.Children.Add(panelChild());
            _panel.Children.Add(panelChild());
            CommonFunctionality.FlushDispatcher();

            FrameworkElement removeMe = _panel.Children[1] as FrameworkElement;
            if (removeMe == null)
            {
                Helpers.Log("Child To Remove is null");
                this.Result = false;
            }
            else
            {
                _panel.Children.Remove(removeMe);
            }

            CommonFunctionality.FlushDispatcher();

            if (_panel.Children.Count != 2)
            {
                Helpers.Log("Custom Panel did not remove Child.");
                this.Result = false;
            }

            //clear all
            _panel.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            if (_panel.Children.Count != 0)
            {
                Helpers.Log("Custom Panel did not clear all Children.");
                this.Result = false;
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

        FrameworkElement panelChild()
        {
            Border b = new Border();
            b.Height = 500;
            b.Width = 500;
            b.Background = Brushes.Yellow;
            return b;
        }
    }

    [Test(0, "Panels.Panel", "PanelChildCollection", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class PanelChildCollection : CodeTest
    {


        public PanelChildCollection()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        TestPanel _panel;
        int _panelChildCount = 156;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _panel = new TestPanel();
            _panel.Height = 400;
            _panel.Width = 400;
            _panel.Background = Brushes.CornflowerBlue;

            addChildren(_panelChildCount);

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            int _panelChildCount = 0;

            IEnumerator enumerator = LogicalTreeHelper.GetChildren(_panel).GetEnumerator();

            if (enumerator != null)
            {
                while (enumerator.MoveNext())
                {
                    this._panelChildCount++;
                }
            }
            else
            {
                Helpers.Log("enumerator is null, could not find custom panel children");
                this.Result = false;
            }

            if (this._panelChildCount != _panelChildCount)
            {
                Helpers.Log("Child Collection had incorrect child count");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Child Collection had correct child count");
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

        void addChildren(int count)
        {
            int i;
            for (i = 0; i < count; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Height = 100;
                rect.Width = 100;
                rect.Fill = Brushes.LightGray;
                _panel.Children.Add(rect);
            }
        }

    }

    [Test(0, "Panels.Panel", "PanelChildSize", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class PanelChildSize : CodeTest
    {


        public PanelChildSize()
        {
        }

        public override void WindowSetup()
        {

            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        TestPanel _panel;
        Border _panelChild;
        Size _panelChildSize = new Size(500, 500);

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _panel = new TestPanel();
            _panel.Height = 400;
            _panel.Width = 400;
            _panel.Background = Brushes.CornflowerBlue;

            _panelChild = new Border();
            _panelChild.Height = _panelChildSize.Height;
            _panelChild.Width = _panelChildSize.Width;
            _panelChild.Background = Brushes.Crimson;

            _panel.Children.Add(_panelChild);

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            if (_panel.Children.Count != 0)
            {
                Helpers.Log("Panel has a child.");
                if (_panelChild.ActualHeight != _panelChildSize.Height || _panelChild.ActualWidth != _panelChildSize.Width)
                {
                    Helpers.Log("Custom Panel Child Size should be " + _panelChildSize + " : Actual Size : " + _panelChild.RenderSize);
                    this.Result = false;
                }
            }
            else
            {
                Helpers.Log("Custom Panel does not have a child.");
                this.Result = false;
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

    [Test(0, "Panels.Panel", "PanelMaxHeightWidth", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class PanelMaxHeightWidth : CodeTest
    {
        public PanelMaxHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        TestPanel _panel;
        Grid _root;
        Border _panelcontent;

        double _maxValue = 350;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _panel = new TestPanel();
            _panel.Height = 100;
            _panel.Width = 100;

            _panelcontent = new Border();
            _panelcontent.Background = Brushes.YellowGreen;
            _panelcontent.Height = 100;
            _panelcontent.Width = 100;

            _panel.Children.Add(_panelcontent);

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            _panel.Height = 1000;
            _panel.Width = 1000;
            _panel.MaxHeight = _maxValue;
            _panel.MaxWidth = _maxValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size PanelSize = _panel.RenderSize;

            if (PanelSize.Height != _maxValue || PanelSize.Width != _maxValue)
            {
                Helpers.Log("Panel Max Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Panel Max Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Panel", "PanelMinHeightWidth", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite,MicroSuite")]
    public class PanelMinHeightWidth : CodeTest
    {


        public PanelMinHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        TestPanel _panel;
        Grid _root;
        Border _panelcontent;

        double _minValue = 200;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _panel = new TestPanel();
            _panel.Height = 100;
            _panel.Width = 100;

            _panelcontent = new Border();
            _panelcontent.Background = Brushes.YellowGreen;
            _panelcontent.Height = 100;
            _panelcontent.Width = 100;

            _panel.Children.Add(_panelcontent);

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            _panel.Height = 10;
            _panel.Width = 10;
            _panel.MinHeight = _minValue;
            _panel.MinWidth = _minValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size PanelSize = _panel.RenderSize;

            if (PanelSize.Height != _minValue || PanelSize.Width != _minValue)
            {
                Helpers.Log("Panel Min Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Panel Min Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }
}
