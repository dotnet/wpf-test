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
using System.Windows.Markup;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.Panel", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Panel, test=HeightWidthDefault")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=Panel, test=HeightWidthActual")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Panel, test=ChildHeightWidth")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=Panel, test=MinHeightWidth")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Panel, test=MaxHeightWidth")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.Visibility", TestParameters = "target=Panel, test=Visibility")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Panel, test=HorizontalAlignment")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=Panel, test=VerticalAlignment")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.FlowDirection", TestParameters = "target=Panel, test=FlowDirection")]
    [Test(1, "Panels.Panel", "FrameworkElementProps.Margin", TestParameters = "target=Panel, test=Margin")]
    public class PanelFETest : FrameworkElementPropertiesTest
    {
        public PanelFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.Panel", "PanelIAddChildTest", Variables="Area=ElementLayout")]
    public class PanelIAddChildTest : CodeTest
    {   
        public PanelIAddChildTest()
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

        CustomPanel _panel;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            _panel = new CustomPanel();
            _panel.Height = 300;
            _panel.Width = 300;

            root.Children.Add(_panel);

            return root;
        }

        public override void TestActions()
        {

            //"TextWhiteSpace":
            try
            {
                ((IAddChild)_panel).AddText("                      ");
                Helpers.Log("Test Passed.  Only WhiteSpace Text as Child with No Exception Thrown.");
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
                Helpers.Log("Exception was thrown when WhiteSpace only Text was added as Decorator Child.\n");
                this.Result = false;
            }

            //case "NullChild":
            try
            {
                Border b = null;
                ((IAddChild)_panel).AddChild(b);
                Helpers.Log("Null Child was added, should have thrown an exception.");
                this.Result = false;
            }
            catch (Exception ex)
            {
                Helpers.Log("Test Passed.  Exception was thrown when Null Child was added as Panel Child.");
                Helpers.Log(ex.Message);
            }


            //case "IsItemsHost":

            try
            {
                _panel.IsItemsHost = true;
                Border bor = new Border();
                ((IAddChild)_panel).AddChild(bor);
                Helpers.Log("Child Added when Panel IsItemsHost, should have thrown an exception. Test Failed.");
                this.Result = false;
            }
            catch (Exception ex)
            {
                Helpers.Log("Test Passed.  Exception was thrown when adding a Child to Panel when IsItemsHost.");
                Helpers.Log(ex.Message);
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
    }

    [Test(1, "Panels.Panel", "PanelSetBackgroundTest", Variables="Area=ElementLayout")]
    public class PanelSetBackgroundTest : CodeTest
    {       
        public PanelSetBackgroundTest()
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

        CustomPanel _panel;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            _panel = new CustomPanel();
            _panel.Height = 300;
            _panel.Width = 300;

            _panel.Background = Brushes.Crimson;

            root.Children.Add(_panel);

            return root;
        }

        public override void TestActions()
        {
            Brush panelBackground = _panel.Background;
            if (panelBackground != null)
            {
                Helpers.Log("Background set on Panel!");
            }
            else
            {
                Helpers.Log("Could not set background on Panel.");
                this.Result = false;
            }

            CommonFunctionality.FlushDispatcher();
            Brush firstColor = _panel.GetValue(Panel.BackgroundProperty) as Brush;

            _panel.SetValue(Panel.BackgroundProperty, Brushes.RoyalBlue);

            CommonFunctionality.FlushDispatcher();

            Brush secondColor = _panel.GetValue(Panel.BackgroundProperty) as Brush;

            if (secondColor == firstColor)
            {
                Helpers.Log("Second Background set did not work.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Setting background with SetValue worked.");
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
    }
}
