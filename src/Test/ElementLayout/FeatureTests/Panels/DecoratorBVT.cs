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
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all Decorator code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - ChildAddRemove
    /// - ChildSize
    /// - MaxHeightWidth
    /// - MinHeightWidth
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.Decorator", "DecoratorChildAddRemove", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class DecoratorChildAddRemove : CodeTest
    {
        public DecoratorChildAddRemove()
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
        Decorator _decorator;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _decorator = new Decorator();
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            _decorator.Child = addChild();
            CommonFunctionality.FlushDispatcher();
            if (_decorator.Child == null)
            {
                Helpers.Log("Could not Add Child to Decorator.");
                _tempresult = false;
            }

            _decorator.Child = null;
            if (_decorator.Child != null)
            {
                Helpers.Log("Could not Remove Child to Decorator.");
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

        FrameworkElement addChild()
        {
            Rectangle child = new Rectangle();
            child.Margin = new Thickness(60, 40, 25, 25);
            child.Height = 400;
            child.Width = 350;
            child.Fill = Brushes.Orange;

            return child;
        }
    }

    [Test(0, "Panels.Decorator", "DecoratorChildSize", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class DecoratorChildSize : CodeTest
    {


        public DecoratorChildSize()
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
        Decorator _decorator;
        Rectangle _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _decorator = new Decorator();
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            _child = new Rectangle();
            _child.Margin = new Thickness(60, 40, 25, 25);
            _child.Height = 400;
            _child.Width = 350;
            _child.Fill = Brushes.Orange;

            _decorator.Child = _child;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom) || _decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
            {
                Helpers.Log("Decorator Size is not equal to Child Size + Margins");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Size is equal to Child Size + Margins");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Decorator", "DecoratorMaxHeightWidth", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class DecoratorMaxHeightWidth : CodeTest
    {


        public DecoratorMaxHeightWidth()
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

        Decorator _decorator;
        Grid _root;
        Border _decoratorcontent;

        double _maxValue = 350;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _decorator = new Decorator();
            _decorator.Height = 100;
            _decorator.Width = 100;

            _decoratorcontent = new Border();
            _decoratorcontent.Background = Brushes.YellowGreen;
            _decoratorcontent.Height = 100;
            _decoratorcontent.Width = 100;

            _decorator.Child = _decoratorcontent;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            _decorator.Height = 1000;
            _decorator.Width = 1000;
            _decorator.MaxHeight = _maxValue;
            _decorator.MaxWidth = _maxValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size DecoratorSize = _decorator.RenderSize;

            if (DecoratorSize.Height != _maxValue || DecoratorSize.Width != _maxValue)
            {
                Helpers.Log("Decorator Max Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Max Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Decorator", "DecoratorMinHeightWidth", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite")]
    public class DecoratorMinHeightWidth : CodeTest
    {


        public DecoratorMinHeightWidth()
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

        Decorator _decorator;
        Grid _root;
        Border _decoratorcontent;

        double _minValue = 200;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _decorator = new Decorator();
            _decorator.Height = 100;
            _decorator.Width = 100;

            _decoratorcontent = new Border();
            _decoratorcontent.Background = Brushes.YellowGreen;
            _decoratorcontent.Height = 100;
            _decoratorcontent.Width = 100;

            _decorator.Child = _decoratorcontent;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            _decorator.Height = 10;
            _decorator.Width = 10;
            _decorator.MinHeight = _minValue;
            _decorator.MinWidth = _minValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size DecoratorSize = _decorator.RenderSize;

            if (DecoratorSize.Height != _minValue || DecoratorSize.Width != _minValue)
            {
                Helpers.Log("Decorator Min Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Min Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }
}
