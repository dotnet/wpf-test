// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Layout.TestTypes;
using System.Windows.Controls;
using System.Windows;
using ElementLayout.TestLibrary;
using Microsoft.Test.Layout;
using System.Windows.Media;

namespace ElementLayout.FeatureTests
{
    public class FrameworkElementPropertiesTest : CodeTest
    {
        public FrameworkElementPropertiesTest(string testpanel, string testtype) 
        {
            this.Result = true;

            if (testpanel != null)
            {
                _paneltype = testpanel;
            }
            if (testtype != null || testtype == string.Empty)
            {
                _test = testtype;
            }
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.DarkGray;

            _panel = CreateTestPanel(_paneltype);

            _child = new Border();
            _child.Background = Brushes.LawnGreen;
            _child.Margin = new Thickness(10);

            if (_panel is Panel)
            {
                ((Panel)_panel).Background = Brushes.Crimson;
                _panel.Margin = new Thickness(10);
                ((Panel)_panel).Children.Add(_child);
            }
            else if (_panel is Decorator)
            {
                _panel.Margin = new Thickness(10);
                ((Decorator)_panel).Child = _child;
            }
            else if (_panel is ContentControl)
            { 
                ((ContentControl)_panel).Content = _child;
            }

            _root.Children.Add(_panel);

            return _root;
        }

        public override void TestActions()
        {
            switch (_test)
            {
                case "HeightWidthDefault":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Height / Width Test : ");
                    Helpers.Log("Panel Height and Width have not been set.");
                    Helpers.Log("Properties should be NaN.");
                    Helpers.Log("---------------------------------------------");

                    if (Double.IsNaN(_panel.Width) && Double.IsNaN(_panel.Height))
                    {
                        Helpers.Log("Panel Height and Width ARE NaN (value if properties are unset).");

                        if (_panel.ActualWidth != (_root.ActualWidth - _panel.Margin.Left - _panel.Margin.Right))
                        {
                            Helpers.Log("Panel width IS NOT equal to root width - panel margins");
                            this.Result = false;
                        }
                        else
                        {
                            Helpers.Log("Panel width IS equal to root width - panel margins");
                        }

                        if (_panel.ActualHeight != (_root.ActualHeight - _panel.Margin.Top - _panel.Margin.Bottom))
                        {
                            Helpers.Log("panel height IS NOT equal to root height - panel margins");
                            this.Result = false;
                        }
                        else
                        {
                            Helpers.Log("panel height IS equal to root height - panel margins");
                        }
                    }
                    else
                    {
                        Helpers.Log("Panel Height and Width ARE NOT NaN (value if properties are unset).");
                        this.Result = false;
                    }
                    break;

                case "HeightWidthActual":

                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Height/Width Test : ");
                    Helpers.Log("Setting Height and Width on panel.");
                    Helpers.Log("Properties should not be NaN.");
                    Helpers.Log("---------------------------------------------");
                    _panel.Height = _staticHeight;
                    _panel.Width = _staticWidth;
                    CommonFunctionality.FlushDispatcher();

                    if (!Double.IsNaN(_panel.ActualWidth) && !Double.IsNaN(_panel.ActualHeight))
                    {
                        Helpers.Log("Panel Height and Width ARE NOT NaN (value if properties are set).");

                    }
                    else
                    {
                        Helpers.Log("Panel Height and Width ARE NaN (value if properties are unset).");
                        this.Result = false;
                    }

                    break;

                case "ChildHeightWidth":

                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Child Height/Width Test : ");
                    Helpers.Log("Setting Height and Width on child.");
                    Helpers.Log("Properties should not be NaN.");
                    Helpers.Log("---------------------------------------------");
                    _child.Height = _staticHeight - 100;
                    _child.Width = _staticWidth - 100;
                    CommonFunctionality.FlushDispatcher();

                    if (!Double.IsNaN(_child.ActualHeight) && !Double.IsNaN(_child.ActualWidth) && (_child.ActualHeight > 0) && (_child.ActualWidth > 0))
                    {
                        Helpers.Log("Panel Child Height and Width are greater than 0 (they were set).");

                    }
                    else
                    {
                        Helpers.Log("Panel Child Height and Width are still 0.");
                        this.Result = false;
                    }

                    break;

                case "MinHeightWidth":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Min Test : ");
                    Helpers.Log("Setting Min Height and Width.");
                    Helpers.Log("---------------------------------------------");

                    _panel.Height = double.NaN;
                    _panel.Width = double.NaN;
                    _panel.MinHeight = _staticHeight * 2;
                    _panel.MinWidth = _staticWidth * 2;
                    _root.Height = 100;
                    _root.Width = 100;
                    CommonFunctionality.FlushDispatcher();

                    if (
                        (_panel.ActualHeight != _staticHeight * 2) ||
                        (_panel.ActualWidth != _staticWidth * 2) ||
                        !(_panel.ActualHeight > _root.ActualHeight) ||
                        !(_panel.ActualWidth > _root.ActualWidth))
                    {
                        Helpers.Log("Panel Min Height / Width failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Min Height / Width passed.");
                    }
                    break;

                case "MaxHeightWidth":

                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Max Test : ");
                    Helpers.Log("Setting Max Height and Width.");
                    Helpers.Log("---------------------------------------------");

                    _panel.MinHeight = 0;
                    _panel.MinWidth = 0;
                    _root.Height = double.NaN;
                    _root.Width = double.NaN;
                    _panel.MaxHeight = _staticHeight / 2;
                    _panel.MaxWidth = _staticWidth / 2;
                    _child.Height = 1000;
                    _child.Width = 1000;
                    CommonFunctionality.FlushDispatcher();

                    if (
                        (_panel.ActualHeight != _staticHeight / 2) ||
                        (_panel.ActualWidth != _staticWidth / 2) ||
                        !(_child.ActualHeight > _panel.ActualHeight) ||
                        !(_child.ActualWidth > _panel.ActualWidth))
                    {
                        Helpers.Log("Panel Max Height / Width failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Max Height / Width passed.");
                    }
                    break;

                case "Visibility":

                    _panel.MaxHeight = double.PositiveInfinity;
                    _panel.MaxWidth = double.PositiveInfinity;
                    _panel.Height = _staticHeight;
                    _panel.Width = _staticWidth;
                    _child.Height = double.NaN;
                    _child.Width = double.NaN;
                    CommonFunctionality.FlushDispatcher();

                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Visiblity Test : ");
                    Helpers.Log("Setting Visibility (Visible, Hidden, Collapsed)");
                    Helpers.Log("---------------------------------------------");


                    _panel.Visibility = Visibility.Visible;
                    CommonFunctionality.FlushDispatcher();

                    if (_panel.ActualHeight != _staticHeight || _panel.ActualWidth != _staticWidth)
                    {
                        Helpers.Log("Panel Visilbity.Visilbe failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Visilbity.Visilbe passed.");
                    }

                    _panel.Visibility = Visibility.Hidden;
                    CommonFunctionality.FlushDispatcher();

                    Point HitTestPoint = new Point((_root.ActualWidth / 2), (_root.ActualHeight / 2));

                    IInputElement panelHitTest;
                    panelHitTest = _root.InputHitTest(HitTestPoint);

                    if ((_panel.ActualHeight != _staticHeight || _panel.ActualWidth != _staticWidth) && (panelHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
                    {
                        Helpers.Log("Panel Visilbity.Hidden failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Visilbity.Hidden passed.");
                    }

                    _panel.Visibility = Visibility.Collapsed;
                    CommonFunctionality.FlushDispatcher();

                    panelHitTest = _root.InputHitTest(HitTestPoint);

                    if ((_panel.ActualHeight != 0 || _panel.ActualWidth != 0) && (panelHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
                    {
                        Helpers.Log("Panel Visilbity.Collapsed failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Visilbity.Collapsed passed.");
                    }
                    break;

                case "HorizontalAlignment":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("HorizontalAlignment Test : ");
                    Helpers.Log("Setting to Stretch, Center, Left, Right)");
                    Helpers.Log("---------------------------------------------");

                    _panel.Visibility = Visibility.Visible;
                    _panel.HorizontalAlignment = HorizontalAlignment.Left;
                    CommonFunctionality.FlushDispatcher();

                    Point panelLoc;
                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.X != (0 + _panel.Margin.Left))
                    {
                        Helpers.Log("Panel HorizontalAlignment.Left failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel HorizontalAlignment.Left passed.");
                    }

                    _panel.HorizontalAlignment = HorizontalAlignment.Right;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.X != (_root.ActualWidth - (_panel.ActualWidth + _panel.Margin.Right)))
                    {
                        Helpers.Log("Panel HorizontalAlignment.Right failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel HorizontalAlignment.Right passed.");
                    }

                    _panel.HorizontalAlignment = HorizontalAlignment.Center;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.X != ((_root.ActualWidth / 2) - (_panel.ActualWidth / 2)))
                    {
                        Helpers.Log("Panel HorizontalAlignment.Center failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel HorizontalAlignment.Center passed.");
                    }

                    _panel.Height = double.NaN;
                    _panel.Width = double.NaN;
                    _panel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if ((panelLoc.X != (0 + _panel.Margin.Left)) && (_panel.ActualWidth != (_root.ActualWidth - _panel.Margin.Left - _panel.Margin.Right)))
                    {
                        Helpers.Log("Panel HorizontalAlignment.Stretch failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel HorizontalAlignment.Stretch passed.");
                    }
                    break;

                case "VerticalAlignment":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("VerticalAlignment Test : ");
                    Helpers.Log("Setting to Stretch, Center, Top, Bottom");
                    Helpers.Log("---------------------------------------------");

                    _panel.Height = _staticHeight;
                    _panel.Width = _staticWidth;
                    _panel.VerticalAlignment = VerticalAlignment.Top;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.Y != (0 + _panel.Margin.Top))
                    {
                        Helpers.Log("Panel VerticalAlignment.Top failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel VerticalAlignment.Top passed.");
                    }

                    _panel.VerticalAlignment = VerticalAlignment.Bottom;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.Y != (_root.ActualHeight - (_panel.ActualHeight + _panel.Margin.Bottom)))
                    {
                        Helpers.Log("Panel VerticalAlignment.Bottom failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel VerticalAlignment.Bottom passed.");
                    }

                    _panel.VerticalAlignment = VerticalAlignment.Center;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (panelLoc.Y != ((_root.ActualHeight / 2) - (_panel.ActualHeight / 2)))
                    {
                        Helpers.Log("Panel VerticalAlignment.Center failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel VerticalAlignment.Center passed.");
                    }

                    _panel.Height = double.NaN;
                    _panel.Width = double.NaN;
                    _panel.VerticalAlignment = VerticalAlignment.Stretch;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if ((panelLoc.Y != (0 + _panel.Margin.Top)) && (_panel.ActualHeight != (_root.ActualHeight - _panel.Margin.Top - _panel.Margin.Bottom)))
                    {
                        Helpers.Log("Panel VerticalAlignment.Stretch failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel VerticalAlignment.Stretch passed.");
                    }
                    break;

                case "FlowDirection":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("FlowDirection Test : ");
                    Helpers.Log("Setting to Left - Right, Right - Left");
                    Helpers.Log("---------------------------------------------");

                    _panel.FlowDirection = FlowDirection.RightToLeft;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (!(panelLoc.X >= _panel.ActualWidth))
                    {
                        Helpers.Log("Panel FlowDirection.RightToLeft failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel FlowDirection.RightToLeft passed.");
                    }

                    _panel.FlowDirection = FlowDirection.LeftToRight;
                    CommonFunctionality.FlushDispatcher();

                    panelLoc = LayoutUtility.GetElementPosition(_panel, _root);
                    if (!(panelLoc.X == (0 + _panel.Margin.Left)))
                    {
                        Helpers.Log("Panel FlowDirection.LeftToRight failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel FlowDirection.LeftToRight passed.");
                    }
                    break;

                case "Margin":
                    Helpers.Log("---------------------------------------------");
                    Helpers.Log("Margin Test : ");
                    Helpers.Log("Testing Margin on panel.");
                    Helpers.Log("---------------------------------------------");

                    _panel.Margin = new Thickness(30, 75, 9, 102);
                    CommonFunctionality.FlushDispatcher();

                    Point LeftMarginHitTestPoint = new Point(RandomDoubleWithinMargin(0, _panel.Margin.Left), (_root.ActualWidth / 2));

                    IInputElement LeftMarginHitTest;
                    LeftMarginHitTest = _root.InputHitTest(LeftMarginHitTestPoint);

                    if (LeftMarginHitTest.GetType().ToString() != _panel.Parent.GetType().ToString())
                    {
                        Helpers.Log("Panel Margin.Left failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Margin.Left passed.");
                    }

                    Point RightMarginHitTestPoint = new Point((_panel.ActualWidth + _panel.Margin.Left + RandomDoubleWithinMargin(0, _panel.Margin.Right)), (_root.ActualWidth / 2));

                    IInputElement RightMarginHitTest;
                    RightMarginHitTest = _root.InputHitTest(RightMarginHitTestPoint);

                    if (RightMarginHitTest.GetType().ToString() != _panel.Parent.GetType().ToString())
                    {
                        Helpers.Log("Panel Margin.Right failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Margin.Right passed.");
                    }

                    Point TopMarginHitTestPoint = new Point((_root.ActualHeight / 2), RandomDoubleWithinMargin(0, _panel.Margin.Top));

                    IInputElement TopMarginHitTest;
                    TopMarginHitTest = _root.InputHitTest(TopMarginHitTestPoint);

                    if (TopMarginHitTest.GetType().ToString() != _panel.Parent.GetType().ToString())
                    {
                        Helpers.Log("Panel Margin.Top failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Margin.Top passed.");
                    }

                    Point BottomMarginHitTestPoint = new Point((_root.ActualHeight / 2), (_panel.ActualHeight + _panel.Margin.Top + RandomDoubleWithinMargin(0, _panel.Margin.Bottom)));

                    IInputElement BottomMarginHitTest;
                    BottomMarginHitTest = _root.InputHitTest(BottomMarginHitTestPoint);

                    if (BottomMarginHitTest.GetType().ToString() != _panel.Parent.GetType().ToString())
                    {
                        Helpers.Log("Panel Margin.Bottom failed.");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("Panel Margin.Bottom passed.");
                    }
                    break;

                default:
                    Helpers.Log("Invalid test arguments.");
                    this.Result = false;
                    break;
            }
        }

        private int RandomDoubleWithinMargin(double Min, double Max)
        {
            Random random = new Random();
            int WithInMargin = random.Next((int)(Min + 1), (int)Max);
            return WithInMargin;
        }

        private string _test = string.Empty;
        private string _paneltype = string.Empty;
        private FrameworkElement _panel = null;
        private Border _child = null;
        private Grid _root = null;

        private double _staticWidth = 300;
        private double _staticHeight = 250;

        private FrameworkElement CreateTestPanel(string panel)
        {
            FrameworkElement testpanel = null;
            switch(panel)
            {
                case "Border":
                    testpanel = new Border();
                    break;

                case "Canvas":
                    testpanel = new Canvas();
                    break;

                case "Decorator":
                    testpanel = new Decorator();
                    break;

                case "DockPanel":
                    testpanel = new DockPanel();
                    break;

                case "Grid":
                    testpanel = new Grid();
                    break;

                case "Panel":
                    testpanel = new TestPanel();
                    break;

                case "ScrollViewer":
                    testpanel = new ScrollViewer();
                    break;

                case "StackPanel":
                    testpanel = new StackPanel();
                    break;

                case "Viewbox":
                    testpanel = new Viewbox();
                    break;

                case "WrapPanel":
                    testpanel = new WrapPanel();
                    break;

                default:
                    testpanel = new TestPanel();
                    Helpers.Log("Invalid test argument, using default TestPanel.");
                    break;
            }
            return testpanel;
        }
    }
}
