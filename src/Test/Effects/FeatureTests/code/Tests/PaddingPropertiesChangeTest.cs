// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test changing ShaderEffect.Padding* properties. 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test changing Padding properties in between. 
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Arrowhead\\Padding", "ChangPaddingProperties",
    //     SupportFiles = @"FeatureTests\Effects\Xamls\PaddingPropertyChangeTest.xaml,
    //     FeatureTests\Effects\Masters\StartPaddingPropertyChangeTest.xaml.png,
    //     FeatureTests\Effects\Masters\EndPaddingPropertyChangeTest.xaml.png,
    //     FeatureTests\Effects\Model\testprofile.xml,
    //     common\InvariantTheme.xaml")]
    public class PaddingPropertyChangeTest : XamlBasedChangingTest 
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("PaddingPropertyChangeTest.xaml", "StartPaddingPropertyChangeTest.xaml.png", "EndPaddingPropertyChangeTest.xaml.png")]
        public PaddingPropertyChangeTest(string xamlFileName, string startMasterImageName, string endMasterImageName)
            : base(xamlFileName, startMasterImageName, endMasterImageName)
        {
            Interval = 500; //500 ms should be enough for padding change. 
        }

        protected override void Change(FrameworkElement content)
        {
            ShaderEffectWithPadding effect = content.FindName("effect") as ShaderEffectWithPadding;
            effect.PaddingLeft = effect.PaddingRight = effect.PaddingTop = effect.PaddingBottom = 20;
        }

        #endregion

    }
}