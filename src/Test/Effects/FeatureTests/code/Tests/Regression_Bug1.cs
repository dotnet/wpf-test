// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug1.
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Regression test for Regression_Bug1.
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Arrowhead\\Regression", "Release effect with image as input twice",
    //     SupportFiles = @"FeatureTests\Effects\Xamls\Regression_Bug1.xaml,
    //     FeatureTests\Effects\Images\Red.jpg,
    //     Common\Effects\ShaderEffects.dll,
    //     FeatureTests\Effects\Masters\StartRegression_Bug1.xaml.png,
    //     FeatureTests\Effects\Masters\EndRegression_Bug1.xaml.png,
    //     FeatureTests\Effects\Model\testprofilebad.xml,
    //     common\InvariantTheme.xaml")]
    public class Regression_Bug1 : XamlBasedChangingTest 
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("Regression_Bug1.xaml", "StartRegression_Bug1.xaml.png", "EndRegression_Bug1.xaml.png")]
        public Regression_Bug1(string xamlFileName, string startMasterImageName, string endMasterImageName)
            : base(xamlFileName, startMasterImageName, endMasterImageName)
        {
            Interval = 500; 
            ToleranceFilePath = "testprofilebad.xml";
        }

        /// <summary>
        /// The changes, assign a new ImageBrush to effect and remove the effect twice,
        /// caused a crash before fix of Regression_Bug1
        /// </summary>
        /// <param name="content"></param>
        protected override void Change(FrameworkElement content)
        {
            BitmapImage bi = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Red.jpg"));
            ImageBrush ib = new ImageBrush(bi);
            Rectangle rect1 = content.FindName("rect1") as Rectangle;
            Rectangle rect2 = content.FindName("rect2") as Rectangle;
            MultiInputEffect effect = rect1.Effect as MultiInputEffect;
            effect.Input1 = ib;
            rect1.Effect = null;
            rect2.Effect = null;
        }

        #endregion

    }
}