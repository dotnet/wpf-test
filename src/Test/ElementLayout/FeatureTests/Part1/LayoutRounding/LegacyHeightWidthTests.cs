// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

using Microsoft.Test.Logging;
using ElementLayout.FeatureTests.Property;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>    
    /// Runs legacy HeightWidth property tests with UseLayoutRounding == true  
    /// </summary>
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Panel", TestParameters = "target=Panel")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Canvas", TestParameters = "target=Canvas")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest StackPanel", TestParameters = "target=StackPanel")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Grid", TestParameters = "target=Grid")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest DockPanel", TestParameters = "target=DockPanel")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Decorator", TestParameters = "target=Decorator")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Border", TestParameters = "target=Border")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest Viewbox", TestParameters = "target=Viewbox")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest ScrollViewer", TestParameters = "target=ScrollViewer")]
    [Test(2, "Part1.LayoutRounding", "Legacy HeightWidthTest WrapPanel", TestParameters = "target=WrapPanel")]    
    public class LayoutRoundingLegacyHeightWidthTest : HeightWidthTest
    {
        public LayoutRoundingLegacyHeightWidthTest()
            : base(true)
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.UseLayoutRounding = true;
            this.window.Content = this.TestContent();
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
