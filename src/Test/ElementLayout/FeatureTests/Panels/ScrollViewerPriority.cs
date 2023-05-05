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
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using ElementLayout.TestLibrary;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Layout.TestTypes;
using System.IO;
using System.Windows.Markup;

#endregion

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=ScrollViewer, test=HeightWidthDefault")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=ScrollViewer, test=HeightWidthActual")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=ScrollViewer, test=ChildHeightWidth")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=ScrollViewer, test=MinHeightWidth")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=ScrollViewer, test=MaxHeightWidth")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.Visibility", TestParameters = "target=ScrollViewer, test=Visibility")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=ScrollViewer, test=HorizontalAlignment")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=ScrollViewer, test=VerticalAlignment")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.FlowDirection", TestParameters = "target=ScrollViewer, test=FlowDirection")]
    [Test(1, "Panels.ScrollViewer", "FrameworkElementProps.Margin", TestParameters = "target=ScrollViewer, test=Margin")]
    public class ScrollViewerFETest : FrameworkElementPropertiesTest
    {
        public ScrollViewerFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.ScrollViewer", "ScrollViewerGetSet", Variables="Area=ElementLayout")]
    public class ScrollViewerGetSet : CodeTest
    {
        public ScrollViewerGetSet()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        ScrollViewer _sv;
        Grid _root;
        StackPanel _stack;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("Test: ScrollViewer.SetCanContentScroll on null element.");
            try
            {
                ScrollViewer.SetCanContentScroll(_sv, true);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.SetHorizontalScrollBarVisibility on null element.");
            try
            {
                ScrollViewer.SetHorizontalScrollBarVisibility(_sv, ScrollBarVisibility.Visible);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.SetVerticalScrollBarVisibility on null element.");
            try
            {
                ScrollViewer.SetVerticalScrollBarVisibility(_sv, ScrollBarVisibility.Visible);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.GetCanContentScroll on null element.");
            try
            {
                ScrollViewer.GetCanContentScroll(_sv);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.GetHorizontalScrollBarVisibility on null element.");
            try
            {
                ScrollViewer.GetHorizontalScrollBarVisibility(_sv);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.GetVerticalScrollBarVisibility on null element.");
            try
            {
                ScrollViewer.GetVerticalScrollBarVisibility(_sv);
                _tempresult = false;
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
            }

            _sv = new ScrollViewer();
            _stack = new StackPanel();
            _sv.Content = _stack;
            _root.Children.Add(_sv);
            CommonFunctionality.FlushDispatcher();

            Helpers.Log("Test: ScrollViewer.SetCanContentScroll on element.");
            try
            {
                ScrollViewer.SetCanContentScroll(_sv, true);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.SetHorizontalScrollBarVisibility on element.");
            try
            {
                ScrollViewer.SetHorizontalScrollBarVisibility(_sv, ScrollBarVisibility.Visible);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.SetVerticalScrollBarVisibility on element.");
            try
            {
                ScrollViewer.SetVerticalScrollBarVisibility(_sv, ScrollBarVisibility.Visible);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.GetCanContentScroll on element.");
            try
            {
                ScrollViewer.GetCanContentScroll(_sv);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.GetHorizontalScrollBarVisibility on element.");
            try
            {
                ScrollViewer.GetHorizontalScrollBarVisibility(_sv);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.GetVerticalScrollBarVisibility on element.");
            try
            {
                ScrollViewer.GetVerticalScrollBarVisibility(_sv);
            }
            catch (ArgumentException)
            {
                Helpers.Log("Argument exception caught.");
                _tempresult = false;
            }

            Helpers.Log("Test: ScrollViewer.ScrollToHorizontalOffset to double.NaN.");
            try
            {
                _sv.ScrollToHorizontalOffset(double.NaN);
                _tempresult = false;
            }
            catch (Exception)
            {
                Helpers.Log("Exception caught.");
            }

            Helpers.Log("Test: ScrollViewer.ScrollToVerticalOffset to double.NaN.");
            try
            {
                _sv.ScrollToVerticalOffset(double.NaN);
                _tempresult = false;
            }
            catch (Exception)
            {
                Helpers.Log("Exception caught.");
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Panels.ScrollViewer", "ScrollingThumbTest", Variables="Area=ElementLayout")]
    public class ScrollingThumbTest : CodeTest
    {
        public ScrollingThumbTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "scrolling_deadloop.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)XamlReader.Load(f);

            f.Close();

        }

        bool _scroll_result = true;
        bool _thumb_result = true;

        ScrollViewer _scrollviewer;

        public override void TestActions()
        {
            ListBox lbx = null;

            while (lbx == null)
            {
                Helpers.Log("lbx is still null, checking again.");
                lbx = LogicalTreeHelper.FindLogicalNode(window, "lbx") as ListBox;
            }

            try
            {
                object scroll = LayoutUtility.GetChildFromVisualTree(lbx, typeof(ScrollViewer));

                if (scroll != null)
                {
                    if (scroll is ScrollViewer)
                    {
                        _scrollviewer = scroll as ScrollViewer;

                        for (int i = 0; i < 100; i++)
                        {
                            ((ScrollViewer)scroll).LineDown();
                            CommonFunctionality.FlushDispatcher();
                        }
                        for (int i = 0; i < 100; i++)
                        {
                            ((ScrollViewer)scroll).LineRight();
                            CommonFunctionality.FlushDispatcher();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence(ex);
                _scroll_result = false;
            }

            object vscroll = LayoutUtility.GetChildFromVisualTree(_scrollviewer, typeof(ScrollBar));

            if (vscroll != null)
            {
                object thumb = LayoutUtility.GetChildFromVisualTree(vscroll as ScrollBar, typeof(Thumb));

                if (thumb != null)
                {
                    Point p = LayoutUtility.GetElementPosition(thumb as Thumb, vscroll as ScrollBar);

                    if (p.Y <= 0)
                    {
                        Helpers.Log("thumb did not get updated.");
                        _thumb_result = false;
                    }
                }
                else
                {
                    Helpers.Log("Could not find thumb in vertical scroll.");
                    _thumb_result = false;
                }
            }
            else
            {
                Helpers.Log("Could not find vertical scroll.");
                _thumb_result = false;
            }
        }


        public override void TestVerify()
        {
            if (!_scroll_result)
            {
                Helpers.Log("Scrolling test failed.");
                this.Result = false;
            }
            else if (!_thumb_result)
            {
                Helpers.Log("Thumb update test failed.");
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

    }

    #region ScrollView ReLayout on Content Changed

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged1", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged1 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 1;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }
  
        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged2", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged2 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 2;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged3", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged3 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 3;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged4", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged4 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 4;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged5", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged5 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 5;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged6", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged6 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 6;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged7", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged7 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 7;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged8", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged8 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged8()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 8;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged9", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged9 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged9()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 9;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }

    [Test(3, "Panels.ScrollViewer", "ScrollViewerReLayoutOnContentChanged10", Variables="Area=ElementLayout")]
    public class ScrollViewerReLayoutOnContentChanged10 : CodeTest
    {
        public ScrollViewerReLayoutOnContentChanged10()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 300;
            this.window.Width = 400;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _scrollviewer = new ScrollViewer();
            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.Content = svContent();
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        int _testID = 10;
        bool _tempresult = true;
        bool _scrollable = true;
        string _resultMsg = null;

        public override void TestActions()
        {
            switch (_testID)
            {
                case 1:
                    _content.Width = 500;
                    _content.Height = 500;
                    break;
                case 2:
                    ((Grid)_content).ColumnDefinitions.Add(new ColumnDefinition());
                    ((Grid)_content).RowDefinitions.Add(new RowDefinition());
                    GridCommon.PlacingChild((Grid)_content, CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.Red)), 3, 3, true);
                    break;
                case 3:
                    ((Grid)_content).ColumnDefinitions.RemoveRange(1, 2);
                    ((Grid)_content).RowDefinitions.RemoveRange(1, 2);
                    _scrollable = false;
                    break;
                case 4:
                    ((DockPanel)_content).Children.RemoveRange(2, 2);
                    _scrollable = false;
                    break;
                case 5:
                    Rectangle d2rect3 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Yellow));
                    DockPanel.SetDock(d2rect3, Dock.Left);
                    Rectangle d2rect4 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.Orchid));
                    ((DockPanel)_content).Children.Add(d2rect3);
                    ((DockPanel)_content).Children.Add(d2rect4);
                    break;
                case 6:
                    ((TextBlock)_content).Width = _scrollviewer.ViewportWidth;
                    ((TextBlock)_content).Text += " Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. ";
                    ((TextBlock)_content).Text += " The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. ";
                    break;
                case 7:
                    Paragraph p2 = new Paragraph();
                    p2.Background = Brushes.LightPink;
                    p2.Inlines.Clear();
                    p2.Inlines.Add(new Run(" Engineers had encountered problems with the same sensor system during a launch-pad test in April, and NASA was not able track down the precise cause at that time. A second tanking test in May was successful, but a different tank was installed on Discovery last month, and NASA didn't run the test for a third time. The shuttle crew, led by commander Eileen Collins, was already strapped into the shuttle when the launch was postponed at about 1:30 p.m. ET. The seven astronauts climbed back out and were driven back to their quarters while the launch-pad team secured the shuttle. The space shuttle Discovery had been scheduled to lift off at 3:51 p.m. ET on a mission to resupply the international space station and test safety procedures that had been developed in the wake of the shuttle Columbia's catastrophic breakup in February 2003. All seven of Columbia's astronauts were killed in the tragedy, which led NASA to ground its shuttle fleet. Until the sensor problem was discovered, the main worry Wednesday had to do with Florida's changeable weather, not technical problems. "));
                    Paragraph p3 = new Paragraph();
                    p3.Background = Brushes.LightGreen;
                    p3.Inlines.Clear();
                    p3.Inlines.Add(new Run(" The sensor system works a bit like a car's gas gauge, signaling when the fuel tank's level sinks to 2 percent of capacity. When the gauge starts to read 'E,' the shuttle's control system readjusts pressure levels in preparation for shutting down the main engines. A faulty sensor could cause the shutdown to happen too early or late. There are eight of these low-level sensors, also known as engine-cutoff sensors or ECO sensors. Four measure the liquid oxygen in the fuel tank; four measure the liquid hydrogen. Mission managers said one of the sensors for the hydrogen levels was apparently sending out suspicious readings during a test — basically, continuing to signal that it was covered with liquid hydrogen even when the system was set to read 'E.' The criteria for launching the shuttle require all four sensors to be working, even though the system could handle two failures during launch, deputy shuttle program manager Wayne Hale said. Mission managers said the source of the problem might be with the circuitry outside the fuel tank rather than with the sensors themselves, but a full check of the system would require removing the fuel from the tank. If the sensor inside the tank is found to be at fault, the shuttle might have to be brought back to the Vehicle Assembly Building, sources told NBC News. In that scenario, launch would likely be delayed until September. However, U.S. Rep. Sherwood Boehlert, a New York Republican who heads the House Science Committee, was more optimistic: 'I'm confident that they will solve the problem and we'll have a successful launch, probably next week.' Earlier, another launch-day snag was handled more easily: During early-morning preparations for fueling up the shuttle's external fuel tank, engineers encountered a problem with a launch-pad heater. A replacement heater was sent to the pad for a quick change, and after an hour's delay, the fueling process and other preparations went ahead smoothly. "));
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p2);
                    ((FlowDocumentScrollViewer)_content).Document.Blocks.Add(p3);
                    break;
                case 8:
                    ((Rectangle)_content).Margin = new Thickness(100);
                    break;
                case 9:
                    _scrollviewer.Padding = new Thickness(100);
                    break;
                case 10:
                    ((Rectangle)_content).HorizontalAlignment = HorizontalAlignment.Right;
                    ((Rectangle)_content).VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    break;
            }
            _scrollviewer.UpdateLayout();
            _scrollviewer.ScrollToBottom();
            _scrollviewer.ScrollToRightEnd();
        }

        FrameworkElement _content;
        FrameworkElement svContent()
        {
            _content = new FrameworkElement();
            switch (_testID)
            {
                case 1:
                    Canvas c = new Canvas();
                    c.Background = Brushes.Black;
                    c.HorizontalAlignment = HorizontalAlignment.Center;
                    Rectangle cRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle cRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle cRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    c.Children.Add(cRect0);
                    c.Children.Add(cRect1);
                    c.Children.Add(cRect2);
                    Canvas.SetTop(cRect0, 100);
                    Canvas.SetLeft(cRect0, 100);
                    Canvas.SetRight(cRect1, 100);
                    Canvas.SetBottom(cRect2, 100);
                    _content = c;
                    break;
                case 2:
                case 3:
                    Grid g = GridCommon.CreateGrid(3, 3);
                    Rectangle gRect0 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Orange));
                    Rectangle gRect1 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Blue));
                    Rectangle gRect2 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.Green));
                    GridCommon.PlacingChild(g, gRect0, 0, 0, true);
                    GridCommon.PlacingChild(g, gRect1, 1, 1, true);
                    GridCommon.PlacingChild(g, gRect2, 2, 2, true);
                    g.ShowGridLines = true;
                    _content = g;
                    break;
                case 4:
                    DockPanel d = new DockPanel();
                    Rectangle dRect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle dRect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    Rectangle dRect2 = CommonFunctionality.CreateRectangle(200, 300, new SolidColorBrush(Colors.Green));
                    Rectangle dRect3 = CommonFunctionality.CreateRectangle(200, 400, new SolidColorBrush(Colors.Yellow));
                    d.Children.Add(dRect0);
                    d.Children.Add(dRect1);
                    d.Children.Add(dRect2);
                    d.Children.Add(dRect3);
                    DockPanel.SetDock(dRect0, Dock.Top);
                    DockPanel.SetDock(dRect1, Dock.Left);
                    DockPanel.SetDock(dRect2, Dock.Left);
                    _content = d;
                    break;
                case 5:
                    DockPanel d2 = new DockPanel();
                    Rectangle d2Rect0 = CommonFunctionality.CreateRectangle(200, 50, new SolidColorBrush(Colors.Orange));
                    Rectangle d2Rect1 = CommonFunctionality.CreateRectangle(50, 100, new SolidColorBrush(Colors.Blue));
                    d2.Children.Add(d2Rect0);
                    d2.Children.Add(d2Rect1);
                    DockPanel.SetDock(d2Rect0, Dock.Top);
                    _content = d2;
                    break;
                case 6:
                    TextBlock textblock = new TextBlock();
                    textblock.Background = Brushes.Yellow;
                    textblock.TextWrapping = TextWrapping.Wrap;
                    textblock.Text = "CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. ";
                    _content = textblock;
                    break;
                case 7:
                    FlowDocumentScrollViewer textflow = new FlowDocumentScrollViewer();
                    Paragraph p1 = new Paragraph(new Run("CAPE CANAVERAL, Fla. - The first space shuttle mission in more than two years was postponed less than three hours before its scheduled launch Wednesday when NASA encountered a problem with one of the external fuel tank's low-level sensors. No new launch date was set, but deputy shuttle program manager Wayne Hale told a news conference that the 'absolute best-case' scenario for launch would be no earlier than Saturday. Earlier, NASA Administrator Mike Griffin had told visiting congressional members that the next launch attempt could not take place until Monday at the earliest. Workers at Kennedy Space Center were draining the external fuel tank, Hale said, and the team would likely have more data about the problem late Wednesday night. He said a decision about what to do next would not come before Thursday. The current launch opportunity extends until the end of July, after which NASA would have to wait until September. For now, Discovery's crew was staying in Cape Canaveral, officials said. "));
                    p1.Background = Brushes.LightBlue;
                    textflow.Document = new FlowDocument(p1);//.Blocks.Add(p1);
                    _content = textflow;
                    break;
                case 8:
                case 9:
                    Rectangle rect8 = CommonFunctionality.CreateRectangle(200, 200, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect8;
                    break;
                case 10:
                    Rectangle rect10 = CommonFunctionality.CreateRectangle(400, 400, new SolidColorBrush(Colors.BlanchedAlmond));
                    _content = rect10;
                    break;
                default:
                    _tempresult = false;
                    _resultMsg += "TestID not set correctly. ";
                    break;

            }
            return _content;
        }

        public override void TestVerify()
        {
            FrameworkElement content = _scrollviewer.Content as FrameworkElement;
            if (_tempresult)
            {
                if (_scrollable)
                {
                    _tempresult = (Math.Abs(content.DesiredSize.Width - _scrollviewer.ExtentWidth) > 0.1
                        || Math.Abs(content.DesiredSize.Height - _scrollviewer.ExtentHeight) > 0.1) ? false : true;
                    _resultMsg += "\nContentWidth/Height = " + content.DesiredSize.Width + "/" + content.DesiredSize.Height;
                    _resultMsg += "\nExtentWidth/Height = " + _scrollviewer.ExtentWidth + "/" + _scrollviewer.ExtentHeight;
                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - _scrollviewer.VerticalOffset) > 0.1
                            || Math.Abs(_scrollviewer.ScrollableWidth - _scrollviewer.HorizontalOffset) > 0.1) ? false : true;
                        _resultMsg += "\nScrollable Width/Height = " + _scrollviewer.ScrollableWidth + "/" + _scrollviewer.ScrollableHeight;
                        _resultMsg += "\nHorizontal/Vertical Offset = " + _scrollviewer.HorizontalOffset + "/" + _scrollviewer.VerticalOffset;
                    }

                    if (_tempresult)
                    {
                        _tempresult = (Math.Abs(_scrollviewer.ScrollableHeight - (_scrollviewer.ExtentHeight - _scrollviewer.ViewportHeight)) > 0.1
                           || Math.Abs(_scrollviewer.ScrollableWidth - (_scrollviewer.ExtentWidth - _scrollviewer.ViewportWidth)) > 0.1) ? false : true;
                        _resultMsg += "\nExtentWidth/Height = " + content.ActualWidth + "/" + content.ActualHeight;
                        _resultMsg += "\nViewportWidth/Height = " + _scrollviewer.ViewportWidth + "/" + _scrollviewer.ViewportHeight;
                    }

                }
                else
                {
                    _resultMsg += "\nComputedHorizontalScrollBarVisibility = " + _scrollviewer.ComputedHorizontalScrollBarVisibility.ToString();
                    _resultMsg += "\nComputedVerticalScrollBarVisibility = " + _scrollviewer.ComputedVerticalScrollBarVisibility.ToString();
                    //verify with HorizontalScrollBarVisibility
                    _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeX != 0) ? false : true;
                    //verify with VerticalScrollBarVisibility
                    if (_tempresult)
                        _tempresult = (ScrollTestCommon.GetExpectedThumSize(_scrollviewer).thumSizeY != 0) ? false : true;
                }
            }

            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
    }
    
    #endregion

    //#region Scrolling Basic Function

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction1", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction1 : CodeTest
    //{
    //    public ScrollingBasicFunction1()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 1;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction2", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction2 : CodeTest
    //{
    //    public ScrollingBasicFunction2()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 2;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction3", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction3 : CodeTest
    //{
    //    public ScrollingBasicFunction3()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 3;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction4", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction4 : CodeTest
    //{
    //    public ScrollingBasicFunction4()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 4;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction5", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction5 : CodeTest
    //{
    //    public ScrollingBasicFunction5()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 5;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction6", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction6 : CodeTest
    //{
    //    public ScrollingBasicFunction6()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 6;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        GlobalLog.LogEvidence("IN TEST CONTENT");
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);


    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        GlobalLog.LogEvidence("IN TEST CONTENT 2");
    //        sv.Content = g;
    //        //----- End -----

    //        //Border b6 = new Border();
    //        //ComboBox cb6 = new ComboBox();
    //        //cb6.Height = 25;
    //        //cb6.SelectedIndex = 0;
    //        //for (int i = 0; i < 50; i++)
    //        //{
    //        //    ComboBoxItem cbi6 = new ComboBoxItem();
    //        //    cbi6.Content = "ComboBoxItem " + i.ToString();
    //        //    cb6.Items.Add(cbi6);
    //        //}
    //        //b6.Child = cb6;
    //        //eRoot = (FrameworkElement)b6;
           
    //        GlobalLog.LogEvidence("IN TEST CONTENT 3");
         
    //        return sv;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction7", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction7 : CodeTest
    //{
    //    public ScrollingBasicFunction7()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 1;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction8", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction8 : CodeTest
    //{
    //    public ScrollingBasicFunction8()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 8;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //[Test(2, "Panels.ScrollViewer", "ScrollingBasicFunction9", Variables="Area=ElementLayout")]
    //public class ScrollingBasicFunction9 : CodeTest
    //{
    //    public ScrollingBasicFunction9()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 300;
    //        this.window.Width = 400;
    //        this.window.Top = 0;
    //        this.window.Left = 0;
    //        this.window.Content = this.TestContent();
    //    }

    //    public static int testID = 9;
    //    public static bool doPostAction = false;
    //    public static bool scrollBarVerify = true;

    //    public static FrameworkElement eRoot;
    //    public static ScrollTestCommon.ScrollerVisibilities expected;

    //    public override FrameworkElement TestContent()
    //    {
    //        expected = new ScrollTestCommon.ScrollerVisibilities(ScrollBarVisibility.Disabled, ScrollBarVisibility.Visible);

    //        //----- ScrollViewer With Grid ** -----
    //        ScrollViewer sv = new ScrollViewer();
    //        Grid g = GridCommon.CreateGrid(5, 5);
    //        g.ShowGridLines = true;
    //        byte color = 20;
    //        for (int i = 0; i < 5; i++)
    //        {
    //            for (int j = 0; j < 5; j++)
    //            {
    //                string tStr = " (Column" + i.ToString() + ", " + "Row" + j.ToString() + ")";
    //                TextBlock t = CommonFunctionality.CreateText(tStr);
    //                t.Foreground = Brushes.Black;
    //                GridCommon.PlacingChild(g, t, i, j);
    //                color += 25;
    //                //b.Child = t;
    //                g.Children.Add(t);
    //            }
    //        }
    //        sv.Content = g;
    //        //----- End -----

    //        switch (testID)
    //        {
    //            case 1:
    //                Border b1 = new Border();
    //                TextBox tb1 = new TextBox();
    //                tb1.TextWrapping = TextWrapping.Wrap;
    //                tb1.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb1.Text = "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //                tb1.Width = 150;
    //                tb1.Height = 150;
    //                b1.Child = tb1;
    //                eRoot = (FrameworkElement)b1;
    //                break;
    //            case 2:
    //                g.Width = 500;
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Hidden;

    //                break;
    //            case 3:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 500;
    //                g.Height = 400;
    //                eRoot = (FrameworkElement)sv;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Visible;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;
    //            case 4:
    //                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                g.Width = 800;
    //                g.Height = 600;
    //                sv.ScrollToHorizontalOffset(160);
    //                sv.ScrollToVerticalOffset(120);
    //                eRoot = (FrameworkElement)sv;
    //                scrollBarVerify = false;
    //                break;
    //            case 5:
    //                Border b5 = new Border();
    //                ListBox lb5 = new ListBox();
    //                lb5.Height = 100;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li5 = new ListBoxItem();
    //                    li5.Content = "ListItem " + i.ToString();
    //                    lb5.Items.Add(li5);
    //                }
    //                b5.Child = lb5;
    //                eRoot = (FrameworkElement)b5;
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                break;
    //            case 6:
    //                Border b6 = new Border();
    //                ComboBox cb6 = new ComboBox();
    //                cb6.Height = 25;
    //                cb6.SelectedIndex = 0;
    //                for (int i = 0; i < 50; i++)
    //                {
    //                    ComboBoxItem cbi6 = new ComboBoxItem();
    //                    cbi6.Content = "ComboBoxItem " + i.ToString();
    //                    cb6.Items.Add(cbi6);
    //                }
    //                b6.Child = cb6;
    //                eRoot = (FrameworkElement)b6;
    //                break;
    //            case 7:
    //                DockPanel dp7 = new DockPanel();

    //                TextBox tb7 = new TextBox();
    //                tb7.TextWrapping = TextWrapping.Wrap;
    //                tb7.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb7.Text = "Adding more text in this text box...";
    //                tb7.Width = 300;

    //                Button btn7 = new Button();
    //                btn7.Content = "Add more text~";
    //                btn7.Click += new RoutedEventHandler(AddingText);
    //                btn7.Height = 70;

    //                dp7.Children.Add(tb7);
    //                dp7.Children.Add(btn7);
    //                eRoot = (FrameworkElement)dp7;
    //                doPostAction = true;
    //                break;
    //            case 8:
    //                DockPanel dp8 = new DockPanel();

    //                TextBox tb8 = new TextBox();
    //                tb8.Width = 250;
    //                tb8.TextWrapping = TextWrapping.Wrap;
    //                tb8.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
    //                tb8.Text = "Adding more text in this text box...";

    //                Button btn8 = new Button();
    //                btn8.Content = "Add more text~";
    //                btn8.Click += new RoutedEventHandler(AddingText);

    //                dp8.Children.Add(tb8);
    //                dp8.Children.Add(btn8);
    //                eRoot = (FrameworkElement)dp8;
    //                doPostAction = true;
    //                break;

    //            case 9:
    //                DockPanel dp9 = new DockPanel();
    //                ListBox lb9 = new ListBox();
    //                //lb9.Width = 100;
    //                lb9.Height = 150;
    //                for (int i = 0; i < 15; i++)
    //                {
    //                    ListBoxItem li9 = new ListBoxItem();
    //                    li9.Content = "List Item " + i.ToString();
    //                    lb9.Items.Add(li9);
    //                }
    //                Button btn9 = new Button();
    //                btn9.Content = "Add more Items~";
    //                btn9.Click += new RoutedEventHandler(AddingItems);
    //                dp9.Children.Add(lb9);
    //                dp9.Children.Add(btn9);
    //                eRoot = (FrameworkElement)dp9;
    //                doPostAction = true;
    //                //reset expected value (diff from default expected value)
    //                expected.horizontal = ScrollBarVisibility.Hidden;
    //                expected.vertical = ScrollBarVisibility.Visible;
    //                break;

    //            default: break;
    //        }
    //        return eRoot;
    //    }

    //    ScrollTestCommon.LayoutTestResult testresult = new ScrollTestCommon.LayoutTestResult();

    //    public override void TestActions()
    //    {
    //        if (doPostAction)
    //        {
    //            FrameworkElement clickElement = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Button)) as FrameworkElement;
    //            UserInput.MouseLeftClickCenter(clickElement);
    //            CommonFunctionality.FlushDispatcher();
    //            ScrollTestCommon.LayoutTestResult tr1 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr1.message;
    //            testresult.result = tr1.result;
    //        }

    //        if (scrollBarVerify)
    //        {
    //            ScrollTestCommon.LayoutTestResult tr2 = ScrollTestCommon.ScrollBarVerification(eRoot, expected);
    //            testresult.message += tr2.message;
    //            testresult.result = tr2.result;
    //        }
    //        else
    //        {
    //            ScrollTestCommon.LayoutTestResult tr3 = ScrollTestCommon.ContentPositionVerification(eRoot, expected);
    //            testresult.message += tr3.message;
    //            testresult.result = tr3.result;
    //        }
    //    }


    //    public override void TestVerify()
    //    {
    //        Helpers.Log(testresult.message);
    //        this.Result = testresult.result;
    //    }

    //    static void AddingText(object sender, RoutedEventArgs e)
    //    {
    //        TextBox textbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(TextBox)) as TextBox;
    //        textbox.Text += "Window glass is a big source of external heat in a vehicle. It's easy to see why. Some of that glass [on a vehicle] can act as a magnifier, said Kevin Wood, an officer in the Arizona Public Department of Safety. That's why window tinting has become increasingly popular. Depending on the type of tint used, nearly all of the ultraviolet rays coming through a car window and up to 65 percent of solar energy or heat can be eliminated, said Alan Lawson, sales manager for SunTekWindow Films of Martinsville, Va.Typically, the darker the windows, the more heat and light are reduced.The materials used in the window film are also a factor.For example, SunTek's standard film is a dyed polyester that's rather neutral in color, Lawson said. The up-level film is a high - performance charcoal color that is made of dyed polyester with reflective metal in it, he said. By the way, some drivers have more than one reason for gettingwindows tinted.Young drivers think the dark tint looks cool on their vehicles, Lawson said.But remember to check with law enforcement in your area. Some states restrict how dark a car's window film can be, and which car windows it can be placed on. Typically, police officers want to be able to see into a vehicle through the driver and front-passenger windows. Some motorcyclists also strive to make eye contact with drivers to confirm they and their bikes are in sight and being monitored by the driver, which is another reason to keep those front-door windows clear of dark tints. Prices for window tinting commonly range from $100 to more than $300.";
    //    }

    //    static void AddingItems(object sender, RoutedEventArgs e)
    //    {
    //        ListBox listbox = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(ListBox)) as ListBox;
    //        for (int j = 0; j < 5; j++)
    //        {
    //            ListBoxItem listitem = new ListBoxItem();
    //            listitem.Content = "Additional List Item " + j.ToString();
    //            listbox.Items.Add(listitem);
    //        }
    //    }

    //}

    //#endregion

}
