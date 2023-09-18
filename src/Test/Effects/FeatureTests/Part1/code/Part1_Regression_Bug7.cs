// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug7.
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
    /// Regression test for Part1 Regression_Bug7
    /// </summary>`
   //commenting ignored cases
    // [Test(2, "Regression", "Part1_Regression_Bug7",
    //     SupportFiles = @"FeatureTests\Effects\Masters\Part1_Regression_Bug7.png,        
    //     FeatureTests\Effects\Model\testprofilebad.xml,
    //     common\InvariantTheme.xaml")]
    public class Part1_Regression_Bug7 : EffectsWindowTest
    {
        /// <summary>
        /// Constructor, setup steps.
        /// </summary>
        [Variation()]
        public Part1_Regression_Bug7()
            : base(150, 150)
        {
            RunSteps += new TestStep(LoadEffectImplicitInput);
        }

        /// <summary>
        /// Verify that Effect.ImplicitInput used as a brush, and as Input for effect, won't cause crash. 
        /// </summary>
        /// <returns></returns>
        TestResult LoadEffectImplicitInput()
        {
            Log.LogStatus("Verify that using Effect.ImplicitInput won't cause a crash.");

            Rectangle rectangle = new Rectangle();
            rectangle.Width = 100;
            rectangle.Height = 100;

            rectangle.Fill = Effect.ImplicitInput;

            MultiInputEffect effect = new MultiInputEffect();
            effect.Input3 = Effect.ImplicitInput;

            rectangle.Effect = effect;

            ContentRoot.Content = rectangle;

            Window.Show();
            WaitFor(200);

            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "Part1_Regression_Bug7.png", "testprofilebad.xml"))
            {
                Log.LogEvidence("Test failed in the visual validation.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}
