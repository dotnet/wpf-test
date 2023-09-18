// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Negative tests for Effect 
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Loaders;
using Microsoft.Test.Serialization;
using Microsoft.Test.Globalization;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Negative tests. 
    /// </summary>`
   //commenting ignored cases
    //  [Test(1, "Arrowhead\\NegativeTest", "NegativeTest", SupportFiles =
    //  @"Common\InvariantTheme.xaml,
    //  Common\Effects\Shaders\reddish.ps,
    //  Common\Effects\Shaders\MultiParameterShader.ps,
    //  Common\Effects\Shaders\DoubleTextureShader.ps,
    //  FeatureTests\Effects\Masters\MultiParameterShaderOnRectangleWithFill_100_100_100.png,
    //  FeatureTests\Effects\Masters\NullShaderOnRectangleWithFill_100_100_100.png,
    //  FeatureTests\Effects\Masters\ReddishShaderOnRectangleWithFill_100_100_100.png,
    //  FeatureTests\Effects\Model\testprofilebad.xml"
    //  )]
    public class NegativeTest : EffectsWindowTest
    {
        #region Private Data

        private static readonly string s_tolerenceFileName = "testprofilebad.xml";

        #endregion

        #region Methods

        /// <summary>
        /// Conatrutor
        /// </summary>
        /// <param name="shaderFile">Compiled shader</param>
        /// <param name="renderMode">ShaderRenderMode</param>
        /// <param name="exceptedDispatcherException">Expecting a Exception in Dispatcher?</param>
        public NegativeTest()
        {
            RunSteps += new TestStep(RunTest);
        }
        

        /// <summary>
        /// RunTest apply effect with negative shader on a button, verify the exception. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            TestSettingPixelShader();
            CombineEffects();
            TestShaderConstant();
            TestPadding();
            TestNullBrushes();
            TestNotSupportedBrushes();
            TestSamplerRegister();
            TestZeroLengthShader();
            return TestResult.Pass;
        }

        //test there is no crash with 0 length shader, with both sw and hw rendering.
        private void TestZeroLengthShader()
        {
            Status("Testing Zero lenghth stream.");

            PixelShader shader = new PixelShader();
            MemoryStream memStream = new MemoryStream(0);
            shader.SetStreamSource(memStream);
            BasicShaderEffect effect = new BasicShaderEffect();
            effect.PixelShader = shader;
            Rectangle rectangle = CreateRectangle();
            rectangle.Effect = effect;
            ContentRoot.Content = rectangle;

            Window.Show();
            WaitFor(200);

            //software rendering
            RenderingModeHelper.SetRenderingMode(Window, RenderingMode.Software);
            WaitFor(200);
            
        }
        /// <summary>
        /// Test passing sampler through invalid register. 
        /// </summary>
        private void TestSamplerRegister()
        {
            EffectSendingInputThroughIllegalRegister effect = new EffectSendingInputThroughIllegalRegister();
            Status("Testing sampler on register -2.");
            ExceptionHelper.ExpectException<ArgumentException>(
                delegate()
                {
                    effect.UpdateInputProperty(EffectSendingInputThroughIllegalRegister.Input_2Property);
                },
                new ArgumentException());

            Status("Testing sampler on register 16.");
            ExceptionHelper.ExpectException<ArgumentException>(
                delegate()
                {
                    effect.UpdateInputProperty(EffectSendingInputThroughIllegalRegister.Input16Property);
                },
                new ArgumentException());
        }

        /// <summary>
        /// Test scenario using null as texture input. 
        /// </summary>
        private void TestNullBrushes()
        {
            Status("Testing null brush...");

            DoubleInputEffect effect = new DoubleInputEffect();
            effect.Input0 = null;
            Rectangle rectangle = CreateRectangle();
            rectangle.Effect = effect;
            ContentRoot.Content = rectangle;
            WaitFor(200);
        }

        /// <summary>
        /// Create a rectangle with Fill color (255, 100, 100, 100)
        /// </summary>
        /// <returns></returns>
        private Rectangle CreateRectangle()
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 200;
            rectangle.Height = 200;
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 100, 100, 100);
            rectangle.Fill = mySolidColorBrush;
            return rectangle;
        }

        /// <summary>
        /// Test types not supported so far as Input texture. 
        /// </summary>
        private void TestNotSupportedBrushes()
        {
            Status("Testing input not supported yet ...");

            DoubleInputEffect effect = new DoubleInputEffect();

            ExceptionHelper.ExpectException<ArgumentException>(
             delegate()
             {
                 effect.Input0 = new SolidColorBrush();
             },
             new ArgumentException(string.Empty, "dp"));
        }

        /// <summary>
        /// Test invalid type for shader constant. 
        /// </summary>
        private void TestShaderConstant()
        {
            Status("Testing illegal constant type...");
            Button button = new Button();
            ExceptionHelper.ExpectException<InvalidOperationException>(
             delegate()
             {
                 ShaderEffectWithIllegalConstantType effect = new ShaderEffectWithIllegalConstantType();
             },
             new InvalidOperationException(),
             "Effect_ShaderConstantType",
             Microsoft.Test.Globalization.WpfBinaries.PresentationCore);

            Status("Testing Effect with a constant set to register over limit...");
            ExceptionHelper.ExpectException<ArgumentException>(
              delegate()
              {
                  button.Effect = new ShaderEffectOverRegisterLimit();
              },
              new ArgumentException(string.Empty, "dp"));
        }

        /// <summary>
        /// Combine new Effect with legacy BitmapEffect. 
        /// </summary>
        private void CombineEffects()
        {
            Status("Testing element with Effect and BitmapEffect...");
            Button button = new Button();
#pragma warning disable 0618
            button.BitmapEffect = new BlurBitmapEffect();
#pragma warning restore 0618

            ExceptionHelper.ExpectException<Exception>(
                delegate()
                {
                    button.Effect = new BlurEffect();
                },
                new Exception(),
                "Effect_CombinedLegacyAndNew",
                Microsoft.Test.Globalization.WpfBinaries.PresentationCore);
            ContentRoot.Content = button;
            WaitFor(200);
        }

        /// <summary>
        /// Test negative padding property. 
        /// </summary>
        private void TestPadding()
        {
            Status("Test Negative PaddingTop.");

            ShaderEffectWithPadding effect = new ShaderEffectWithPadding();

            ExceptionHelper.ExpectException<ArgumentOutOfRangeException>(
                delegate()
                {
                    effect.PaddingTop = -1.0;
                },
                new ArgumentOutOfRangeException(GetParameterName("PaddingTop"), -1.0, Exceptions.GetMessage("Effect_ShaderEffectPadding", WpfBinaries.PresentationCore)));

            Status("Test Negative PaddingBottom.");

            ExceptionHelper.ExpectException<ArgumentOutOfRangeException>(
              delegate()
              {
                  effect.PaddingBottom = -1.0;
              },
              new ArgumentOutOfRangeException(GetParameterName("PaddingBottom"), -1.0, Exceptions.GetMessage("Effect_ShaderEffectPadding", WpfBinaries.PresentationCore)));

            Status("Test Negative PaddingLeft.");

            ExceptionHelper.ExpectException<ArgumentOutOfRangeException>(
              delegate()
              {
                  effect.PaddingLeft = -1.0;
              },
              new ArgumentOutOfRangeException(GetParameterName("PaddingLeft"), -1.0, Exceptions.GetMessage("Effect_ShaderEffectPadding", WpfBinaries.PresentationCore)));

            Status("Test Negative PaddingRight.");

            ExceptionHelper.ExpectException<ArgumentOutOfRangeException>(
              delegate()
              {
                  effect.PaddingRight = -1.0;
              },
              new ArgumentOutOfRangeException(GetParameterName("PaddingRight"), -1.0, Exceptions.GetMessage("Effect_ShaderEffectPadding", WpfBinaries.PresentationCore)));

        }

        /// <summary>
        /// In Clr4.0 the parameter name used is "value" to workaround Part1 Regression_Bug4 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        private string GetParameterName(string parameterName)
        {
#if TESTBUILD_CLR40   
            parameterName = "value";
#endif
            return parameterName;
        }



        /// <summary>
        /// Test setting pixel shader. 
        /// </summary>
        void TestSettingPixelShader()
        {
            Status("Test Set null as shader stream source.");

            PixelShader shader = new PixelShader();
            shader.SetStreamSource(null);

            BasicShaderEffect effect = new BasicShaderEffect();
            effect.PixelShader = shader;

            Rectangle rectangle = CreateRectangle();

            rectangle.Effect = effect;
            ContentRoot.Content = rectangle;

            WaitFor(200);
            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "NullShaderOnRectangleWithFill_100_100_100.png", s_tolerenceFileName))
            {
                throw new TestValidationException("Test failed in the first visual validation.");
            }

            Status("Test set StreamSource on a shader already has UriSource.");
            shader.UriSource = new Uri("pack://siteoforigin:,,,/reddish.ps");
            effect.PixelShader = shader;
            rectangle.Effect = effect;
            WaitFor(200);
            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "ReddishShaderOnRectangleWithFill_100_100_100.png", s_tolerenceFileName))
            {
                throw new TestValidationException("Test failed in visual validation after setting UriSource to reddish.");
            }

            shader.SetStreamSource(File.OpenRead("MultiParameterShader.ps"));
            effect.PixelShader = shader;
            rectangle.Effect = effect;
            WaitFor(200);

            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "MultiParameterShaderOnRectangleWithFill_100_100_100.png", s_tolerenceFileName))
            {
                throw new TestValidationException("Test failed in visual validation after switching shader to MultiParameterShader.");
            }

            //Set to null again and verify. 
            shader.UriSource = null;
            effect.PixelShader = shader;
            rectangle.Effect = effect;
            WaitFor(2000);
            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, "NullShaderOnRectangleWithFill_100_100_100.png", s_tolerenceFileName))
            {
                throw new TestValidationException("Test failed in visual validation after setting UriSource to null.");
            }
        }
        #endregion
    }
}

