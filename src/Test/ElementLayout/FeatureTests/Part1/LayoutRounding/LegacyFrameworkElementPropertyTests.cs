// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

using Microsoft.Test.Logging;
using ElementLayout.FeatureTests;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>    
    /// Runs legacy FrameworkElement property tests with UseLayoutRounding == true  
    /// </summary>
    [Test(1, "Part1.LayoutRounding", "Legacy Legacy Border FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Border, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.HeightWidthActual", TestParameters = "target=Border, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Border, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.MinHeightWidth", TestParameters = "target=Border, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Border, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.Visibility", TestParameters = "target=Border, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Border, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.VerticalAlignment", TestParameters = "target=Border, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.FlowDirection", TestParameters = "target=Border, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy Border FrameworkElementProps.Margin", TestParameters = "target=Border, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Canvas, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.HeightWidthActual", TestParameters = "target=Canvas, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Canvas, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.MinHeightWidth", TestParameters = "target=Canvas, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Canvas, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.Visibility", TestParameters = "target=Canvas, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Canvas, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.VerticalAlignment", TestParameters = "target=Canvas, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.FlowDirection", TestParameters = "target=Canvas, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy Canvas FrameworkElementProps.Margin", TestParameters = "target=Canvas, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Decorator, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.HeightWidthActual", TestParameters = "target=Decorator, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Decorator, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.MinHeightWidth", TestParameters = "target=Decorator, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Decorator, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.Visibility", TestParameters = "target=Decorator, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Decorator, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.VerticalAlignment", TestParameters = "target=Decorator, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.FlowDirection", TestParameters = "target=Decorator, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy Decorator FrameworkElementProps.Margin", TestParameters = "target=Decorator, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.HeightWidthDefault", TestParameters = "target=DockPanel, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.HeightWidthActual", TestParameters = "target=DockPanel, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.ChildHeightWidth", TestParameters = "target=DockPanel, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.MinHeightWidth", TestParameters = "target=DockPanel, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.MaxHeightWidth", TestParameters = "target=DockPanel, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.Visibility", TestParameters = "target=DockPanel, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.HorizontalAlignment", TestParameters = "target=DockPanel, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.VerticalAlignment", TestParameters = "target=DockPanel, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.FlowDirection", TestParameters = "target=DockPanel, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy DockPanel FrameworkElementProps.Margin", TestParameters = "target=DockPanel, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Grid, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.HeightWidthActual", TestParameters = "target=Grid, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Grid, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.MinHeightWidth", TestParameters = "target=Grid, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Grid, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.Visibility", TestParameters = "target=Grid, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Grid, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.VerticalAlignment", TestParameters = "target=Grid, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.FlowDirection", TestParameters = "target=Grid, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy Grid FrameworkElementProps.Margin", TestParameters = "target=Grid, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Panel, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.HeightWidthActual", TestParameters = "target=Panel, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Panel, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.MinHeightWidth", TestParameters = "target=Panel, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Panel, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.Visibility", TestParameters = "target=Panel, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Panel, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.VerticalAlignment", TestParameters = "target=Panel, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.FlowDirection", TestParameters = "target=Panel, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy Panel FrameworkElementProps.Margin", TestParameters = "target=Panel, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.HeightWidthDefault", TestParameters = "target=ScrollViewer, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.HeightWidthActual", TestParameters = "target=ScrollViewer, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.ChildHeightWidth", TestParameters = "target=ScrollViewer, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.MinHeightWidth", TestParameters = "target=ScrollViewer, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.MaxHeightWidth", TestParameters = "target=ScrollViewer, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.Visibility", TestParameters = "target=ScrollViewer, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.VerticalAlignment", TestParameters = "target=ScrollViewer, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.FlowDirection", TestParameters = "target=ScrollViewer, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy ScrollViewer FrameworkElementProps.Margin", TestParameters = "target=ScrollViewer, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.HeightWidthDefault", TestParameters = "target=StackPanel, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.HeightWidthActual", TestParameters = "target=StackPanel, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.ChildHeightWidth", TestParameters = "target=StackPanel, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.Visibility", TestParameters = "target=StackPanel, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.HorizontalAlignment", TestParameters = "target=StackPanel, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.VerticalAlignment", TestParameters = "target=StackPanel, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.FlowDirection", TestParameters = "target=StackPanel, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy StackPanel FrameworkElementProps.Margin", TestParameters = "target=StackPanel, test=Margin")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.HeightWidthDefault", TestParameters = "target=WrapPanel, test=HeightWidthDefault")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.HeightWidthActual", TestParameters = "target=WrapPanel, test=HeightWidthActual")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.ChildHeightWidth", TestParameters = "target=WrapPanel, test=ChildHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.MinHeightWidth", TestParameters = "target=WrapPanel, test=MinHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.MaxHeightWidth", TestParameters = "target=WrapPanel, test=MaxHeightWidth")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.Visibility", TestParameters = "target=WrapPanel, test=Visibility")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.HorizontalAlignment", TestParameters = "target=WrapPanel, test=HorizontalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.VerticalAlignment", TestParameters = "target=WrapPanel, test=VerticalAlignment")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.FlowDirection", TestParameters = "target=WrapPanel, test=FlowDirection")]
    [Test(1, "Part1.LayoutRounding", "Legacy WrapPanel FrameworkElementProps.Margin", TestParameters = "target=WrapPanel, test=Margin")]
    public class LayoutRoundingLegacyFrameworkElementTest : FrameworkElementPropertiesTest
    {
        public LayoutRoundingLegacyFrameworkElementTest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.UseLayoutRounding = true;

            FrameworkElement testContent = this.TestContent();
            testContent.Width = 720.5;
            testContent.Height = 719.7;
            this.window.Content = testContent;
        }

        public override void TestActions()
        {
            if (Microsoft.Test.Display.Monitor.Dpi.x != 96 || Microsoft.Test.Display.Monitor.Dpi.y != 96)
            {
                // Since special measurements are applied for non standard dpi (see LayoutRounding spec) we bypass these tests on dpi other than 96
                // Otherwise validation will fail.
                return;
            }
            else
            {
                base.TestActions();
            }
        }
    }
}
