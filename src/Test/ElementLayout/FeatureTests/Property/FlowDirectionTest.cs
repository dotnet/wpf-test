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
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Property
{
    [Test(1, "Property.FlowDirection", "FlowDirectionTest", Variables = "Area=ElementLayout", Keywords = "Setup_SanitySuite, Localization_Suite")]
    public class FlowDirectionTest : CodeTest
    {
        public FlowDirectionTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _zero;
        Grid _one;
        DockPanel _two;
        WrapPanel _three;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.DarkOrange;

            _zero = new Border();
            _zero.FlowDirection = FlowDirection.RightToLeft;
            _zero.Height = 400;
            _zero.Width = 400;
            _root.Children.Add(_zero);

            _one = new Grid();
            _one.Height = 400;
            _one.Width = 400;
            _zero.Child = _one;

            _two = new DockPanel();
            _two.Height = 400;
            _two.Width = 400;
            _one.Children.Add(_two);

            _three = new WrapPanel();
            _three.Height = 400;
            _three.Width = 400;
            _two.Children.Add(_three);

            return _root;
        }

        public override void TestActions()
        {
            if (_zero.FlowDirection != _one.FlowDirection ||
                 _one.FlowDirection != _two.FlowDirection ||
                 _two.FlowDirection != _three.FlowDirection)
            {
                Helpers.Log("Test failed when flow directions were the same [inherited].");
                _tempresult = false;
            }

            _zero.FlowDirection = FlowDirection.RightToLeft;
            _one.FlowDirection = FlowDirection.LeftToRight;
            _two.FlowDirection = FlowDirection.RightToLeft;
            _three.FlowDirection = FlowDirection.LeftToRight;

            CommonFunctionality.FlushDispatcher();

            if (_zero.FlowDirection == _one.FlowDirection ||
                 _one.FlowDirection == _two.FlowDirection ||
                 _two.FlowDirection == _three.FlowDirection)
            {
                Helpers.Log("Test failed when flow directions were different.");
                _tempresult = false;
            }
        }

        bool _tempresult = true;

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }
}
