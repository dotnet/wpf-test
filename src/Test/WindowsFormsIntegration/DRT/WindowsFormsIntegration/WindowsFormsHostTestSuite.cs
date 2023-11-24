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

    SWC.DockPanel dockPanel;
    SWC.Button avButton;
    SWF.Button wfButton;
    Window window;
    WindowsFormsHost windowsFormsHost;

    public override DrtTest[] PrepareTests()
    {
        //Avalon DockPanel added to Window
        dockPanel = new SWC.DockPanel();
        window = new Window();
        window.Width = 300d;
        window.Height = 200d;
        window.Content = dockPanel;

        //Avalon Button added to dock to the BOTTOM of the panel
        avButton = new SWC.Button();
        avButton.Content = "Avalon Button";
        SWC.DockPanel.SetDock(avButton, SWC.Dock.Bottom);
        dockPanel.Children.Add(avButton);

        //WinFormsHost added to dock to the LEFT of the panel
        windowsFormsHost = new WindowsFormsHost();
        SWC.DockPanel.SetDock(windowsFormsHost, SWC.Dock.Left);
        dockPanel.Children.Add(windowsFormsHost);

        //WinForm Button added to Host
        wfButton = new SWF.Button();
        wfButton.Text = "&Windows Forms Button";
        wfButton.Dock = SWF.DockStyle.Fill;

        windowsFormsHost.Child = wfButton;
        window.Show();

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
        dockPanel.FlowDirection = SW.FlowDirection.RightToLeft;
        DRT.AssertEqual(SWF.RightToLeft.Yes, windowsFormsHost.Child.RightToLeft, "Property mapping for RightToLeft didn't work");
        dockPanel.FlowDirection = SW.FlowDirection.LeftToRight;
        DRT.AssertEqual(SWF.RightToLeft.No, windowsFormsHost.Child.RightToLeft, "Property mapping for RightToLeft didn't work");
    }

    private void TestVisibility()
    {
        windowsFormsHost.Visibility = SW.Visibility.Hidden;
        DRT.Assert(!windowsFormsHost.Child.Visible, "Property mapping for Visible (false) didn't work");
        windowsFormsHost.Visibility = SW.Visibility.Visible;
        DRT.Assert(windowsFormsHost.Child.Visible, "Property mapping for Visible (true) didn't work");
    }

    private void TestForeground()
    {
        SWM.Color color = SWM.Color.FromRgb(255, 0, 0);
        SWM.Color colorEnd = SWM.Color.FromRgb(128, 255, 0);
        SWM.LinearGradientBrush fancyBrush = new SWM.LinearGradientBrush(color, colorEnd, 0);
        windowsFormsHost.Foreground = fancyBrush;
        //Ensure this doesn't throw
    }

    private void TestFont()
    {
        windowsFormsHost.FontWeight = SW.FontWeights.Bold;
        DRT.AssertEqual(SD.FontStyle.Bold, windowsFormsHost.Child.Font.Style, "Property mapping for FontStyle didn't work");
    }

    private void TestResize()
    {
        int originalWidth = windowsFormsHost.Child.Width;
        window.Width += 100;
        DRT.Assert(windowsFormsHost.Child.Width > originalWidth, "WindowsFormsHost layout issue: Width didn't grow as parent's Width grew");
        windowsFormsHost.Width = 250;
        SWF.Application.DoEvents();
        originalWidth = windowsFormsHost.Child.Width;
        window.Width += 100;
        SWF.Application.DoEvents();
        DRT.AssertEqual(originalWidth, windowsFormsHost.Child.Width, "WindowsFormsHost layout issue: Child Width grew when host's Width was explicitly set");
    }
}
