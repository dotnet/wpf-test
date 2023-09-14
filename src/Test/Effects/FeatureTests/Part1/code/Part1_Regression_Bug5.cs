// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Part1 Regression_Bug5.
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
    /// Regression test for Part1 Regression_Bug5: in ShaderEffect _sentFirstTime has not been set to be true after first set.
    /// </summary>`
    /// Test disabled since Regression_Bug5 won't be fixed in the near future. 
    [Test(2, "Regression", "Part1_Regression_Bug5",
        SupportFiles = @"FeatureTests\Effects\Masters\Part1_Regression_Bug5.png,        
        Common\Effects\Shaders\VerifyDdxUvDdyUv.ps,
        FeatureTests\Effects\Model\testprofile.xml,
        common\InvariantTheme.xaml", Disabled=true)]
    public class Part1_Regression_Bug5 : EffectsWindowTest
    {
        #region Methods

        /// <summary>
        /// Constructor, setup steps.
        /// </summary>
        [Variation()]
        public Part1_Regression_Bug5()
            : base(350, 350)
        {
            RunSteps += new TestStep(TestSettingDdxUvDdyUvRegisterIndexInConstructorOnly);
            RunSteps += new TestStep(TestSwitchingDdxUvDdyUvRegister);
            RunSteps += new TestStep(TestSettingDdxUvDdyUvRegisterIndexThroughNonContructorMethod);
        }

        /// <summary>
        /// Verify that the DdxUvDdyUv property has been passed through register 1 successfully through visual validation.
        /// </summary>
        /// <returns></returns>
        TestResult TestSettingDdxUvDdyUvRegisterIndexInConstructorOnly()
        {
            Log.LogStatus("Verify that DdxUvDdyUvRegisterIndex set in constructor works.");
            ShaderEffectDdxUvDdyUvRegister1 effect = new ShaderEffectDdxUvDdyUvRegister1();

            Rectangle rectangle = new Rectangle();
            rectangle.Width = 200;
            rectangle.Height = 100;
            rectangle.Fill = Brushes.Red;

            rectangle.Effect = effect;

            ContentRoot.Content = rectangle;

            Window.Show();
            WaitFor(200);

            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "Part1_Regression_Bug5.png", "testprofile.xml"))
            {
                Log.LogEvidence("Test failed in the visual validation.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that switching DdxUvDdyUvRegister property caused an exception. 
        /// </summary>
        /// <returns></returns>
        TestResult TestSwitchingDdxUvDdyUvRegister()
        {
            Log.LogStatus("Verify the exception switching DdxUvDdyUvRegisterIndex property.");
            Rectangle rectangle = ContentRoot.Content as Rectangle;
            ShaderEffectDdxUvDdyUvRegister1 effect = rectangle.Effect as ShaderEffectDdxUvDdyUvRegister1;

            ExceptionHelper.ExpectException<InvalidOperationException>(
              delegate()
              {
                  effect.UpdateDdxUvDdyUvRegisterIndex(2);
              },
             new InvalidOperationException(),
             "Effect_ShaderDdxUvDdyUvRegisterIndex",
             Microsoft.Test.Globalization.WpfBinaries.PresentationCore);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that setting DdxUvDdyUvRegister property through method other than constructor, 
        /// on a effect, caused an exception. 
        /// </summary>
        /// <returns></returns>
        TestResult TestSettingDdxUvDdyUvRegisterIndexThroughNonContructorMethod()
        {
            Log.LogStatus("Verify the exception changing DdxUvDdyUvRegisterIndex in method other than constructor.");
            ShaderEffectSettingDdxUvDdyUvRegisterIndex effect = new ShaderEffectSettingDdxUvDdyUvRegisterIndex();
            Rectangle rectangle = ContentRoot.Content as Rectangle;

            rectangle.Effect = effect;

            ExceptionHelper.ExpectException<InvalidOperationException>(
              delegate()
              {
                  effect.UpdateDdxUvDdyUvRegisterIndex(2);
              },
             new InvalidOperationException(),
             "Effect_ShaderDdxUvDdyUvRegisterIndex",
             Microsoft.Test.Globalization.WpfBinaries.PresentationCore);

            return TestResult.Pass;
        }

        #endregion

        /// <summary>
        /// A ShaderEffect with PixelShader, and API to update DdxUvDdyUvRegisterIndex property. 
        /// </summary>
        public class ShaderEffectSettingDdxUvDdyUvRegisterIndex : ShaderEffect
        {
            public ShaderEffectSettingDdxUvDdyUvRegisterIndex()
            {
                PixelShader shader = new PixelShader();
                shader.UriSource = new Uri("pack://siteoforigin:,,,/VerifyDdxUvDdyUv.ps");
                this.PixelShader = shader;
            }

            public void UpdateDdxUvDdyUvRegisterIndex(int index)
            {
                this.DdxUvDdyUvRegisterIndex = index;
            }
        }

        /// <summary>
        /// Inherit ShaderEffectSettingDdxUvDdyUvRegisterIndex, and set DdxUvDdyUvRegisterIndex = 1 in constructor
        /// </summary>
        public class ShaderEffectDdxUvDdyUvRegister1 : ShaderEffectSettingDdxUvDdyUvRegisterIndex
        {
            public ShaderEffectDdxUvDdyUvRegister1()
            {
                this.DdxUvDdyUvRegisterIndex = 1;
            }
        }
    }
}
