// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;

using Microsoft.Test.Layout.PropertyDump;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.ElementLayout.FeatureTests.Part1
{
    /// <summary>   
    /// Part1 layout comparison tests that use Property Dump for verification.
    /// </summary>  
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Offsets", TestParameters = "content=LR_Offsets.xaml, bypassForDpi=true")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Alignment", TestParameters = "content=LR_Alignment.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_GridExplicitLengths", TestParameters = "content=LR_GridExplicitLength.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Shapes", TestParameters = "content=LR_Shapes.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_TextBlock_Inlines", TestParameters = "content=LR_TextBlock_Inlines.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_TextBlock_TextWrapping", TestParameters = "content=LR_TextBlock_TextWrapping.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Margin", TestParameters = "content=LR_Margin.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Border_BorderThickness", TestParameters = "content=LR_Border_BorderThickness.xaml")]
    [Test(1, "Part1.LayoutRounding", "LayoutRounding_Border_Padding", TestParameters = "content=LR_Border_Padding.xaml")]
    public class Part1PropertyDumpTests : CodeTest
    {
        public Part1PropertyDumpTests()
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
            // Determine if test should use property dump verification under the current test condition.
            bool verifyTest = true;

            // Layout rounding tests under non standard dpi should be skipped.
            // Special measurements are applied for non standard dpi (see LayoutRounding spec) that create different dumps.           
            if (DriverState.TestName.Contains("LayoutRounding"))
            {                
                if (Microsoft.Test.Display.Monitor.Dpi.x != 96 || Microsoft.Test.Display.Monitor.Dpi.y != 96)
                {                       
                    verifyTest = false;
                }               
            }            

            if (verifyTest)
            {
                VerifyPropertyDumpTest();
            }
            else
            {
                Microsoft.Test.Layout.CommonFunctionality.FlushDispatcher();
                this.Result = true;
            }
        }

        private void VerifyPropertyDumpTest()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }        
    }
}
