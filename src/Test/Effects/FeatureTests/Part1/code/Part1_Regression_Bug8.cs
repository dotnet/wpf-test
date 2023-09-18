// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug8.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Regression test for Part1 Regression_Bug8. 
    /// </summary>`
    [Test(2, "Regression", "Part1Regression_Bug8",
        Area="2D",
        Description = @"Regression test for Part1 Regression_Bug8 : 
               Changing FillRule of a Polygon does not update in real-time",
        SupportFiles = @"FeatureTests\Effects\Xamls\Part1Regression_Bug8.xaml,
        FeatureTests\Effects\EffectsTests.dll,
        Common\Effects\ShaderEffects.dll,
        FeatureTests\Effects\Masters\StartPart1Regression_Bug8.xaml.png,
        FeatureTests\Effects\Masters\EndPart1Regression_Bug8.xaml.png,
        FeatureTests\Effects\Model\testprofilebad.xml,
        common\InvariantTheme.xaml")]
    public class Part1_Regression_Bug8 : XamlBasedChangingTest
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("Part1Regression_Bug8.xaml", "StartPart1Regression_Bug8.xaml.png", "EndPart1Regression_Bug8.xaml.png")]
        public Part1_Regression_Bug8(string xamlFileName, string startMasterImageName, string endMasterImageName)
            : base(xamlFileName, startMasterImageName, endMasterImageName)
        {
            Interval = 500;
            ToleranceFilePath = "testprofilebad.xml";
        }

        /// <summary>
        /// Update the content of ContentControls 
        /// </summary>
        /// <param name="content"></param>
        protected override void Change(FrameworkElement content)
        {
            Polygon polygon = content.FindName("polygon") as Polygon;
            polygon.FillRule = FillRule.EvenOdd;
        }

        #endregion
    }
}
