// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms.Integration;
using System.Windows;
using System.Windows.Threading;

using SD = System.Drawing;
using SW = System.Windows;
using SWC = System.Windows.Controls;
using SWM = System.Windows.Media;
using SWF = System.Windows.Forms;
using SWS = System.Windows.Markup;
using SWA = System.Windows.Automation;
using SWI = System.Windows.Input;

using DRT;


public sealed class WindowsFormsHostTestSuite : DrtTestSuite
{
    public WindowsFormsHostTestSuite() : base("WindowsFormsHostTestSuite") { }

    SWC.DockPanel _dockPanel;
    SWC.Button _avButton;
    SWF.Button _wfButton;
    Window _window;
    WindowsFormsHost _windowsFormsHost;

    public override DrtTest[] PrepareTests()
    {
        //Avalon DockPanel added to Window
        _dockPanel = new SWC.DockPanel();
        _window = new Window();
        _window.Width = 300d;
        _window.Height = 200d;
        _window.Content = _dockPanel;

        //Avalon Button added to dock to the BOTTOM of the panel
        _avButton = new SWC.Button();
        _avButton.Content = "Avalon Button";
        SWC.DockPanel.SetDock(_avButton, SWC.Dock.Bottom);
        _dockPanel.Children.Add(_avButton);

        //WinFormsHost added to dock to the LEFT of the panel
        _windowsFormsHost = new WindowsFormsHost();
        SWC.DockPanel.SetDock(_windowsFormsHost, SWC.Dock.Left);
        _dockPanel.Children.Add(_windowsFormsHost);

        //WinForm Button added to Host
        _wfButton = new SWF.Button();
        _wfButton.Text = "&Windows Forms Button";
        _wfButton.Dock = SWF.DockStyle.Fill;

        _windowsFormsHost.Child = _wfButton;
        _window.Show();

        return new DrtTest[] 
            {
                new DrtTest(TestRightToLeft),
                new DrtTest(TestVisibility),
                new DrtTest(TestForeground),
                new DrtTest(TestFont),
                new DrtTest(TestResize),
            };
    }

    private void TestRightToLeft()
    {
        _dockPanel.FlowDirection = SW.FlowDirection.RightToLeft;
        DRT.AssertEqual(SWF.RightToLeft.Yes, _windowsFormsHost.Child.RightToLeft, "Property mapping for RightToLeft didn't work");
        _dockPanel.FlowDirection = SW.FlowDirection.LeftToRight;
        DRT.AssertEqual(SWF.RightToLeft.No, _windowsFormsHost.Child.RightToLeft, "Property mapping for RightToLeft didn't work");
    }

    private void TestVisibility()
    {
        _windowsFormsHost.Visibility = SW.Visibility.Hidden;
        DRT.Assert(!_windowsFormsHost.Child.Visible, "Property mapping for Visible (false) didn't work");
        _windowsFormsHost.Visibility = SW.Visibility.Visible;
        DRT.Assert(_windowsFormsHost.Child.Visible, "Property mapping for Visible (true) didn't work");
    }

    private void TestForeground()
    {
        SWM.Color color = SWM.Color.FromRgb(255, 0, 0);
        SWM.Color colorEnd = SWM.Color.FromRgb(128, 255, 0);
        SWM.LinearGradientBrush fancyBrush = new SWM.LinearGradientBrush(color, colorEnd, 0);
        _windowsFormsHost.Foreground = fancyBrush;
        //Ensure this doesn't throw
    }

    private void TestFont()
    {
        _windowsFormsHost.FontWeight = SW.FontWeights.Bold;
        DRT.AssertEqual(SD.FontStyle.Bold, _windowsFormsHost.Child.Font.Style, "Property mapping for FontStyle didn't work");
    }

    private void TestResize()
    {
        int originalWidth = _windowsFormsHost.Child.Width;
        _window.Width += 100;
        DRT.Assert(_windowsFormsHost.Child.Width > originalWidth, "WindowsFormsHost layout issue: Width didn't grow as parent's Width grew");
        _windowsFormsHost.Width = 250;
        SWF.Application.DoEvents();
        originalWidth = _windowsFormsHost.Child.Width;
        _window.Width += 100;
        SWF.Application.DoEvents();
        DRT.AssertEqual(originalWidth, _windowsFormsHost.Child.Width, "WindowsFormsHost layout issue: Child Width grew when host's Width was explicitly set");
    }
}
