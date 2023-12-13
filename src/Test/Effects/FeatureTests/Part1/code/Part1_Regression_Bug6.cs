// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug6.
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
    /// Regression test for Part1 Regression_Bug6: Max texture size limits do not scale content correctly. 
    /// Verify that a Rectangle at the far left side got passed in to shader correctly. 
    /// </summary>`
    ///Test disabled since Regression_Bug6 won't be fixed in the near future. 
    [Test(2, "Regression", "Part1_Regression_Bug6",
        SupportFiles = @"FeatureTests\Effects\Xamls\Part1Regression_Bug6.xaml,
        FeatureTests\Effects\EffectsTests.dll,
        Common\Effects\ShaderEffects.dll,
        FeatureTests\Effects\Masters\StartPart1Regression_Bug6.xaml.png,
        FeatureTests\Effects\Masters\EndPart1Regression_Bug6.xaml.png,
        FeatureTests\Effects\Model\testprofilebad.xml,
        common\InvariantTheme.xaml", Disabled=true)]
    public class Part1_Regression_Bug6 : XamlBasedChangingTest
    {
        #region Methods

        /// <summary>
        /// Constructor, pass in xaml file name, master file name, and shader file name. 
        /// </summary>
        /// <param name="xamlFileName"></param>
        /// <param name="masterImageName"></param>
        /// <param name="shaderfileName"></param>
        [Variation("Part1Regression_Bug6.xaml", "StartPart1Regression_Bug6.xaml.png", "EndPart1Regression_Bug6.xaml.png")]
        public Part1_Regression_Bug6(string xamlFileName, string startMasterImageName, string endMasterImageName)
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
            ChangeConttrolContent(content, "controlWithCustomEffect");
            ChangeConttrolContent(content, "controlWithBlurEffect");
            ChangeConttrolContent(content, "controlWithDropShadowEffect");
        }

        /// <summary>
        /// Find each Content Controls in the page, and update the content with 
        /// Rectangle at far left side, not visible if before the Regression_Bug6 is fixed. 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="controlName"></param>
        private void ChangeConttrolContent(FrameworkElement root, string controlName)
        {
            ContentControl bigControl = root.FindName(controlName) as ContentControl;

            if (bigControl == null)
            {
                Log.LogEvidence(string.Format("Control {0} not found.", controlName));
            }

            Canvas innerCanvas = bigControl.Template.FindName("innerCanvas", bigControl) as Canvas;
            if (innerCanvas == null)
            {
                Log.LogEvidence("innerCanvas not found");
            }

            Rectangle invisibleRectangle = bigControl.Template.FindName("invisibleRectangle", bigControl) as Rectangle;

            if (invisibleRectangle == null)
            {
                Log.LogEvidence("invisibleRectangle not found");
            }

            VisualBrush vbrush = null;
            vbrush = new VisualBrush(invisibleRectangle);

            innerCanvas.Children.Clear();
            Rectangle r = new Rectangle();
            r.Width = 80;
            r.Height = 80;
            r.Fill = vbrush;

            innerCanvas.Children.Add(r);
        }

        #endregion
    }
}
