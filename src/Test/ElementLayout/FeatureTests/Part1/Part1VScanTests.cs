// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Markup;

using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>
    /// Part1 layout comparison tests that use VScan for verification.
    /// </summary>
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BlurryImage", TestParameters = "content=LR_BlurryImage.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    //To fix test bug, split case LayoutRounding_BadAlphaBorder to 4.0 and 4.5 version with different masters
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BadAlphaBorder_40", TestParameters = "content=LR_BadAlpha_Border.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN",Versions = "4.0,4.0Client,4.0GDR,4.0GDRClient")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BadAlphaBorder_45", TestParameters = "content=LR_BadAlpha_Border.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN",Versions = "4.5+,4.5Client+" )]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BadAlphaRectangle", TestParameters = "content=LR_BadAlpha_Rectangle.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BadAlphaRectangle_SnapsToDevicePixels", TestParameters = "content=LR_BadAlpha_Rectangle_SnapsToDevicePixels.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_BackgroundLines", TestParameters = "content=LR_BackgroundLines.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Transforms_Layout", TestParameters = "content=LR_Transforms_Layout_ParentVerification.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Transforms_Render", TestParameters = "content=LR_Transforms_Render_ParentVerification.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_TextBlock_Inlines_Pixel_Boundaries", TestParameters = "content=LR_TextBlock_Inline_PixelBoundaries.xaml", Variables = "VscanMasterPath=FeatureTests\\ElementLayout\\Masters\\VSCAN")]
    public class Part1VScanTests : CodeTest
    {
        public Part1VScanTests()
        {}

        public override void WindowSetup()
        {
            FileStream fileStream = new FileStream(DriverState.DriverParameters["content"], FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)XamlReader.Load(fileStream);
            fileStream.Close();

            // Setting Window.Content size to ensure same size of root element over all themes.
            // Different themes have diffent sized window chrome which will cause property dump
            // failures even though the rest of the content is the same.
            // 784x564 is the content size of a 800x600 window in Aero them.
            ((FrameworkElement)this.window.Content).Height = 564;
            ((FrameworkElement)this.window.Content).Width = 784;
        }

        public override void TestVerify()
        {
            // Determine if test should use vscan verification under the current test condition.
            bool verifyTest = true;

            // Layout rounding tests under non standard dpi should be skipped.
            // Special measurements are applied for non standard dpi (see LayoutRounding spec) that can potentially create slightly different rendering.
            if (DriverState.TestName.Contains("LayoutRounding"))
            {
                if (Microsoft.Test.Display.Monitor.Dpi.x != 96 || Microsoft.Test.Display.Monitor.Dpi.y != 96)
                {
                    verifyTest = false;
                }
            }

            if (verifyTest)
            {
                VerifyVScanTest();
            }
            else
            {
                Microsoft.Test.Layout.CommonFunctionality.FlushDispatcher();
                this.Result = true;
            }
        }

        private void VerifyVScanTest()
        {
            VScanCommon vscanCommon = new VScanCommon(this);
            this.Result = vscanCommon.CompareImage();
        }
    }
}
