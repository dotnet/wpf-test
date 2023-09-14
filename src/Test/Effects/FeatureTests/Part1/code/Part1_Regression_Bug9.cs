// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug9.
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
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Regression test for Part1 Regression_Bug9
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Regression", "Part1_Regression_Bug9",
    //     SupportFiles = @"FeatureTests\Effects\Masters\Part1_Regression_Bug9*.png, 
    //     FeatureTests\Effects\Model\testprofilebad.xml,
    //     FeatureTests\Effects\Images\CheckerBoard.jpg,
    //     common\InvariantTheme.xaml")]
    public class Part1_Regression_Bug9 : EffectsWindowTest
    {
        /// <summary>
        /// Constructor, setup steps.
        /// </summary>
        [Variation(BitmapScalingMode.NearestNeighbor, SamplingMode.NearestNeighbor, "Part1_Regression_Bug9_NearestNeighbor_NearestNeighbor.png")]
        [Variation(BitmapScalingMode.NearestNeighbor, SamplingMode.Bilinear, "Part1_Regression_Bug9_NearestNeighbor_Bilinear.png")]
        [Variation(BitmapScalingMode.NearestNeighbor, SamplingMode.Auto, "Part1_Regression_Bug9_NearestNeighbor_Auto.png")]
        [Variation(BitmapScalingMode.Linear, SamplingMode.NearestNeighbor, "Part1_Regression_Bug9_Linear_NearestNeighbor.png")]
        [Variation(BitmapScalingMode.Linear, SamplingMode.Bilinear, "Part1_Regression_Bug9_Linear_Bilinear.png")]
        [Variation(BitmapScalingMode.Linear, SamplingMode.Auto, "Part1_Regression_Bug9_Linearr_Auto.png")]
        [Variation(BitmapScalingMode.Fant, SamplingMode.NearestNeighbor, "Part1_Regression_Bug9_Fant_NearestNeighbor.png")]
        [Variation(BitmapScalingMode.Fant, SamplingMode.Bilinear, "Part1_Regression_Bug9_Fant_Bilinear.png")]
        [Variation(BitmapScalingMode.Fant, SamplingMode.Auto, "Part1_Regression_Bug9_Fant_Auto.png")]
        [Variation(BitmapScalingMode.Unspecified, SamplingMode.NearestNeighbor, "Part1_Regression_Bug9_Unspecified_NearestNeighbor.png")]
        [Variation(BitmapScalingMode.LowQuality, SamplingMode.Bilinear, "Part1_Regression_Bug9_LowQuality_Bilinear.png")]
        [Variation(BitmapScalingMode.HighQuality, SamplingMode.Auto, "Part1_Regression_Bug9_HighQuality_Auto.png")]
        public Part1_Regression_Bug9(BitmapScalingMode elementScalingMode, SamplingMode inputSamplingMode, string masterImagePath)
            : base(300, 300)
        {
            this._elementScalingMode = elementScalingMode;
            this._inputSamplingMode = inputSamplingMode;
            this._masterImagePath =  masterImagePath;
            RunSteps += new TestStep(VerifyScalingMode);
        }

        /// <summary>
        /// Verify that Effect.ImplicitInput used as a brush, and as Input for effect, won't cause crash. 
        /// </summary>
        /// <returns></returns>
        TestResult VerifyScalingMode()
        {
            Log.LogStatus("Verify that using Effect.ImplicitInput won't cause a crash.");

            Rectangle rectangle = new Rectangle();
            rectangle.Width = 283;
            rectangle.Height = 261;
            rectangle.Fill = Brushes.White;

            rectangle.SetValue(RenderOptions.BitmapScalingModeProperty, _elementScalingMode);

            ImageBrush imageBrush = new ImageBrush();
            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://siteoforigin:,,,/CheckerBoard.jpg"));
            imageBrush.ImageSource = bitmapImage;

            DoubleInputEffect effect = ConstructDoubleInputEffect();
            
            effect.Input0 = imageBrush;

            rectangle.Effect = effect;

            ContentRoot.Content = rectangle;

            Window.Show();
            WaitFor((int)EffectsTestHelper.IntervalForRender.TotalMilliseconds);

            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, _masterImagePath, "testprofilebad.xml"))
            {
                Log.LogEvidence("Test failed in the visual validation.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private DoubleInputEffect ConstructDoubleInputEffect()
        {
            DoubleInputEffect effect;

            switch (_inputSamplingMode)
            {
                case SamplingMode.NearestNeighbor:
                    effect = new NearestNeighborSamplingModeInputEffect();
                    break;

                case SamplingMode.Bilinear:
                    effect = new BilinearSamplingModeInputEffect();
                    break;

                default:
                    effect = new DoubleInputEffect();
                    break;
            }

            return effect;
        }

        private BitmapScalingMode _elementScalingMode;
        private SamplingMode _inputSamplingMode;
        private string _masterImagePath;
    }
}
