// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug2.
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
    /// Regression test for Part1 Regression_Bug2. 
    /// </summary>`
    [Test(2, "Regression", "Part1Regression_Bug2",
        Area = "2D",
        Description = @"Regression test for Part1 Regression_Bug2 : 
               Changing Path.Stretch from Fill to None or vice versa does not trigger a redraw",
        SupportFiles = @"FeatureTests\Effects\Xamls\Part1Regression_Bug2_NoneToFill.xaml,
        FeatureTests\Effects\Xamls\Part1Regression_Bug2_FillToNone.xaml,
        FeatureTests\Effects\Masters\Part1Regression_Bug2_*.xaml.png,
        FeatureTests\Effects\EffectsTests.dll,
        Common\Effects\ShaderEffects.dll,
        FeatureTests\Effects\Model\testprofilebad.xml,
        common\InvariantTheme.xaml")]
    public class Part1_Regression_Bug2 : XamlBasedChangingTest
    {
        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("Part1Regression_Bug2_FillToNone.xaml", "Part1Regression_Bug2_FillToNone_Start.xaml.png", "Part1Regression_Bug2_FillToNone_End.xaml.png")]
        [Variation("Part1Regression_Bug2_NoneToFill.xaml", "Part1Regression_Bug2_NoneToFill_Start.xaml.png", "Part1Regression_Bug2_NoneToFill_End.xaml.png")]
        public Part1_Regression_Bug2(string xamlFileName, string startMasterImageName, string endMasterImageName)
            : base(xamlFileName, startMasterImageName, endMasterImageName)
        {
            Interval = 500; //in milliseconds. 
            ToleranceFilePath = "testprofilebad.xml";
        }

        /// <summary>
        /// Switch Path.Stretch between None and Fill
        /// </summary>
        /// <param name="content"></param>
        protected override void Change(FrameworkElement content)
        {
            Path path = content.FindName("path") as Path;
            if (path == null)
            {
                throw new TestValidationException("Path path not found.");
            }

            path.Stretch = ToggleStretchMode(path.Stretch);
        }

        /// <summary>
        /// Change Toggle Stretch between None and Fill, throw for others possible values. 
        /// </summary>
        /// <param name="stretch">Stretch to toggle</param>
        /// <returns></returns>
        private Stretch ToggleStretchMode(Stretch stretch)
        {
            if (stretch == Stretch.None)
            {
                return Stretch.Fill;
            }
            else if (stretch == Stretch.Fill)
            {
                return Stretch.None;
            }
            else
            {
                throw new NotImplementedException(string.Format("Strech: {0} not implemented in method ToggleStretchMode.", stretch.ToString()));
            }
        }
    }
}
