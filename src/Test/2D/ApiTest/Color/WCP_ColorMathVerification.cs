// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Color class
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ColorMathVerification : ApiTest
    {
        public WCP_ColorMathVerification( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(Color);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            #region SECTION I - COMPARE MILTEST COLOR WITH AVALON COLOR
            CommonLib.LogStatus("***** SECTION I - COMPARE MILTEST COLOR WITH AVALON COLOR *****");

            #region Test #1 - MILTest Byte and Uint VS. FromArgb and FromRgb
            // Notes: Pass the same color value to all four methods and compare the results of ToString.  Convert the
            // MILTest string to uppercase and add a leading # so that it can be compared with the Avalon Color.ToString().
            CommonLib.LogStatus("Test #1 - MILTest Byte and Uint VS. FromArgb and FromRgb");

            TestColor MILColorUint = new TestColor(4288230335);
            TestColor MILColorBytes = new TestColor(0xFF, 0x99, 0x33, 0xBF);
            System.Windows.Media.Color AvalonColorARGB = System.Windows.Media.Color.FromArgb(0xFF, 0x99, 0x33, 0xBF);
            System.Windows.Media.Color AvalonColorRGB = System.Windows.Media.Color.FromRgb(0x99, 0x33, 0xBF);

            String S11 = ConvertToAvalonFormat(MILColorUint.ToString());
            String S12 = ConvertToAvalonFormat(MILColorBytes.ToString());
            String S13 = AvalonColorARGB.ToString();
            String S14 = AvalonColorRGB.ToString();

            if ((string.Compare(S11, S12, false, System.Globalization.CultureInfo.InvariantCulture) == 0) &&
            (string.Compare(S13, S14, false, System.Globalization.CultureInfo.InvariantCulture) == 0) &&
            (string.Compare(S11, S14, false, System.Globalization.CultureInfo.InvariantCulture) == 0) &&
            (string.Compare(S12, S13, false, System.Globalization.CultureInfo.InvariantCulture) == 0))
            {
                CommonLib.LogStatus("Pass: All four color values equal " + S11);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColorUint          = " + S11);
                CommonLib.LogStatus("MILColorBytes         = " + S12);
                CommonLib.LogStatus("AvalonColorARGB       = " + S13);
                CommonLib.LogStatus("AvalonColorRGB        = " + S14);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion

            #region Test #2 - MILTest 1.0 VS. MILTest.CreateColor and FromScRgb
            // Notes: Create a MILTest Gamma 1.0 Color and an Avalon.FromScRgb Color with the same values.  Compare these
            // with the AvalonColor returned from MILTest.CreateColor().  They should all have the same value.
            CommonLib.LogStatus("Test #2 - MILTest 1.0 VS. MILTest.CreateColor and FromScRgb");

            TestColor MILColor10 = new TestColor(0.5, 0.6, 0.02, 0.75, TestColor.Curve.Gamma_1_0);
            System.Windows.Media.Color AvalonColor10 = MILColor10.CreateWCPColor();
            System.Windows.Media.Color AvalonColorScRGB = System.Windows.Media.Color.FromScRgb(0.5f, 0.6f, 0.02f, 0.75f);

            String S21 = MILColor10.ToString();
            String S22 = BuildScRGBString(AvalonColor10);
            String S23 = BuildScRGBString(AvalonColorScRGB);

            if ((string.Compare(S21, S22, false, System.Globalization.CultureInfo.InvariantCulture) == 0) &&
            (string.Compare(S21, S23, false, System.Globalization.CultureInfo.InvariantCulture) == 0))
            {
                CommonLib.LogStatus("Pass: All three color values equal " + S21);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColor10            = " + S21);
                CommonLib.LogStatus("AvalonColor10         = " + S22);
                CommonLib.LogStatus("AvalonColorScRGB      = " + S23);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion

            #region Test #3 - MILTest 2.2 VS. MILTest.CreateColor
            // Notes: Create a MILTest Gamma 2.2 Color and use it to create an Avalon Color using the MILTest.CreateColor()
            // method.
            CommonLib.LogStatus("Test #3 - MILTest 2.2 VS. MILTest.CreateColor");

            TestColor MILColor22 = new TestColor(0.5, 0.6, 0.02, 0.75, TestColor.Curve.Gamma_2_2);
            System.Windows.Media.Color AvalonColor22 = MILColor22.CreateWCPColor();

            String S31 = ConvertToAvalonFormat(MILColor22.ToString());
            String S32 = AvalonColor22.ToString();

            if (string.Compare(S31, S32, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S31);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColor22            = " + S31);
                CommonLib.LogStatus("AvalonColor22         = " + S32);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion

            #region Test #4 - MILTest 2.2 -> 1.0 VS. MILTest.CreateColor
            // Notes: Create a MILTest Gamma 2.2 Color, convert it to Gamma 1.0 and create an AvalonColor from it, compare the
            // the Avalon Color value with the MILTest color value.
            CommonLib.LogStatus("Test #4 - MILTest 2.2 -> 1.0 VS. MILTest.CreateColor");

            TestColor MILColor2210 = MILColor22.Convert(TestColor.Curve.Gamma_1_0);
            System.Windows.Media.Color AvalonColor2210 = MILColor2210.CreateWCPColor();

            String S41 = BuildMILTestFloatString(MILColor2210);
            String S42 = BuildScRGBString(AvalonColor2210);

            if (string.Compare(S41, S42, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S41);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColor2210         = " + S41);
                CommonLib.LogStatus("AvalonColor2210      = " + S42);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion

            #region Test #5 - MILTest Blend Method - Two Colors
            // Notes: Test the MILTest Blend method by blending two MILTest Colors and comparing the result to the result of
            // two Avalon Colors that were blended in the same way. 
            CommonLib.LogStatus("Test #5 - MILTest Blend Method - Two Colors");

            TestColor MILColor51 = new TestColor(0.5, 1.0, 0.0, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColor52 = new TestColor(0.5, 0.0, 1.0, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColorBlend = TestColor.Blend(MILColor51, MILColor52);

            System.Windows.Media.Color AvalonColor51 = System.Windows.Media.Color.FromScRgb(0.5f, 1.0f, 0.0f, 0.5f);
            System.Windows.Media.Color AvalonColor52 = System.Windows.Media.Color.FromScRgb(0.5f, 0.0f, 1.0f, 0.5f);

            // Blend the Avalon Colors in the same way that the MILTest Colors are blended
            double f0 = (double)AvalonColor52.ScA;
            double f1 = 1.0 - f0;
            System.Windows.Media.Color AvalonColorBlend = System.Windows.Media.Color.FromScRgb
             (
                  (float)((double)AvalonColor51.ScA * f1 + (double)AvalonColor52.ScA),
                  (float)((double)AvalonColor51.ScR * f1 + (double)AvalonColor52.ScR * f0),
                  (float)((double)AvalonColor51.ScG * f1 + (double)AvalonColor52.ScG * f0),
                  (float)((double)AvalonColor51.ScB * f1 + (double)AvalonColor52.ScB * f0)
             );

            String S51 = BuildMILTestFloatString(MILColorBlend);
            String S52 = BuildScRGBString(AvalonColorBlend);

            if (string.Compare(S51, S52, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S51);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColorBlend         = " + S51);
                CommonLib.LogStatus("AvalonColorBlend      = " + S52);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion

            #region Test #6 - MILTest Blend Method - Two Colors With Opacity
            // Notes: Test the MILTest Blend method by blending two MILTest Colors with an additional opacity
            // and comparing the result to the result of two Avalon Colors that were blended
            // in the same way. 
            CommonLib.LogStatus("Test #6 - MILTest Blend Method - Two Colors With Opacity");

            double opacity = 0.2;
            TestColor MILColor61 = new TestColor(0.5, 1.0, 0.0, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColor62 = new TestColor(0.5, 0.0, 1.0, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColorBlendOp = TestColor.Blend(MILColor61, MILColor62, opacity);
            System.Windows.Media.Color AvalonColor61 = System.Windows.Media.Color.FromScRgb(0.5f, 1.0f, 0.0f, 0.5f);
            System.Windows.Media.Color AvalonColor62 = System.Windows.Media.Color.FromScRgb(0.5f, 0.0f, 1.0f, 0.5f);

            // Blend the Avalon Colors in the same way that the MILTest Colors are blended
            f0 = (double)(AvalonColor62.ScA * opacity);
            f1 = 1.0 - f0;
            System.Windows.Media.Color AvalonColorBlendOp = System.Windows.Media.Color.FromScRgb
             (
                 (float)((double)AvalonColor61.ScA * f1 + (double)(AvalonColor62.ScA * opacity)),
                 (float)((double)AvalonColor61.ScR * f1 + (double)AvalonColor62.ScR * f0),
                 (float)((double)AvalonColor61.ScG * f1 + (double)AvalonColor62.ScG * f0),
                 (float)((double)AvalonColor61.ScB * f1 + (double)AvalonColor62.ScB * f0)
             );
            String S61 = BuildMILTestFloatString(MILColorBlendOp);
            String S62 = BuildScRGBString(AvalonColorBlendOp);

            if (string.Compare(S61, S62, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S61);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColorBlend with Opacity         = " + S61);
                CommonLib.LogStatus("AvalonColorBlend with Opacity      = " + S62);
                _class_testresult &= false;
            }

            CommonLib.LogStatus("");
            #endregion

            #region Test #7 - MILTest Mix Method
            // Notes: Test the MILTest Mix method by mixing two MILTest Colors and comparing
            // the result to the result of two Avalon Colors that were mixed in the same way. 
            CommonLib.LogStatus("Test #7 - MILTest Mix Method");

            double mixer = 0.3;
            TestColor MILColor71 = new TestColor(0.5, 0.7, 0.0, 0.4, TestColor.Curve.Gamma_2_2);
            TestColor MILColor72 = new TestColor(0.5, 0.0, 0.8, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColorMix = TestColor.Mix(MILColor71, MILColor72, mixer);
            System.Windows.Media.Color AvalonColor71 = System.Windows.Media.Color.FromScRgb(0.5f, 0.7f, 0.0f, 0.4f);
            System.Windows.Media.Color AvalonColor72 = System.Windows.Media.Color.FromScRgb(0.5f, 0.0f, 0.8f, 0.5f);

            // Mix the Avalon Colors in the same way that the MILTest Colors are mixed
            f1 = 1.0 - mixer;

            System.Windows.Media.Color AvalonColorMix = System.Windows.Media.Color.FromScRgb
             (
                 (float)((double)AvalonColor71.ScA * f1 + (double)AvalonColor72.ScA * mixer),
                 (float)((double)AvalonColor71.ScR * f1 + (double)AvalonColor72.ScR * mixer),
                 (float)((double)AvalonColor71.ScG * f1 + (double)AvalonColor72.ScG * mixer),
                 (float)((double)AvalonColor71.ScB * f1 + (double)AvalonColor72.ScB * mixer)
             );
            String S71 = BuildMILTestFloatString(MILColorMix);
            String S72 = BuildScRGBString(AvalonColorMix);

            if (AreColorsEqual(MILColorMix,AvalonColorMix))
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S71);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColorMix         = " + S71);
                CommonLib.LogStatus("AvalonColorMix      = " + S72);
                _class_testresult &= false;
            }

            CommonLib.LogStatus("");
            #endregion

            #region Test #8 - MILTest MixPremult Method
            // Notes: Test the MILTest MixPremult method by mixing two MILTest Colors and comparing
            // the result to the result of two Avalon Colors that were mixed in the same way. 
            CommonLib.LogStatus("Test #8 - MILTest MixPremult Method");

            mixer = 0.4;
            TestColor MILColor81 = new TestColor(0.5, 0.7, 0.0, 0.4, TestColor.Curve.Gamma_2_2);
            TestColor MILColor82 = new TestColor(0.5, 0.0, 0.8, 0.5, TestColor.Curve.Gamma_2_2);
            TestColor MILColorMixPremult = TestColor.MixPremult(MILColor81, MILColor82, mixer);
            System.Windows.Media.Color AvalonColor81 = System.Windows.Media.Color.FromScRgb(0.5f, 0.7f, 0.0f, 0.4f);
            System.Windows.Media.Color AvalonColor82 = System.Windows.Media.Color.FromScRgb(0.5f, 0.0f, 0.8f, 0.5f);

            // Mix the Avalon Colors in the same way that the MILTest Colors are mixed
            f1 = 1.0 - mixer;
            double alpha = (double)AvalonColor81.ScA * f1 + (double)AvalonColor82.ScA * mixer;

            f1 *= (double)AvalonColor81.ScA;
            mixer *= (double)AvalonColor82.ScA;

            double red = (double)AvalonColor81.ScR * f1 + (double)AvalonColor82.ScR * mixer;
            double green = (double)AvalonColor81.ScG * f1 + (double)AvalonColor82.ScG * mixer;
            double blue = (double)AvalonColor81.ScB * f1 + (double)AvalonColor82.ScB * mixer;

            System.Windows.Media.Color AvalonColorMixPremult = System.Windows.Media.Color.FromScRgb
             (
                 (float)alpha,
                 (float)(red / alpha),
                 (float)(green / alpha),
                 (float)(blue / alpha)
             );
            String S81 = BuildMILTestFloatString(MILColorMixPremult);
            String S82 = BuildScRGBString(AvalonColorMixPremult);

            if (string.Compare(S81, S82, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                CommonLib.LogStatus("Pass: Both color values equal " + S81);
            }
            else
            {
                CommonLib.LogFail("Fail: Color values are not equal");
                CommonLib.LogStatus("MILColorMixPremult         = " + S81);
                CommonLib.LogStatus("AvalonColorMixPremult      = " + S82);
                _class_testresult &= false;
            }
            CommonLib.LogStatus("");
            #endregion
            #endregion

            CommonLib.LogTest("Result for: " + _objectType);

        }

        // Compares 2 colors and returns true if the colors are equal, uses MathEx for float comparisons.
        private bool AreColorsEqual(TestColor testColor, System.Windows.Media.Color avalonColor)
        {
            double oldNumericalPrecisionTolerance = MathEx.NumericalPrecisionTolerance;

            // Smallest positive float number x, such that x + 1.0f is not equal to 1.0f
            MathEx.NumericalPrecisionTolerance = 1.192092896e-07F;

            bool result = MathEx.AreCloseEnough((float)testColor.Alpha, avalonColor.ScA) &&
                          MathEx.AreCloseEnough((float)testColor.Red, avalonColor.ScR) &&
                          MathEx.AreCloseEnough((float)testColor.Blue, avalonColor.ScB) &&
                          MathEx.AreCloseEnough((float)testColor.Green, avalonColor.ScG);

            MathEx.NumericalPrecisionTolerance = oldNumericalPrecisionTolerance;

            return result;
        }

        // Convert the MIL Color String to the Avalon Color String format
        private String ConvertToAvalonFormat(String MILString)
        {
            return (("#" + MILString.ToUpper()));
        }

        // Build a String of ScRGB values from an Avalon Color
        private String BuildScRGBString(System.Windows.Media.Color color)
        {
            return ((color.ScA + "-" + color.ScR + "-" + color.ScG + "-" + color.ScB));
        }

        // Build a String of MILTest Color values expressed as floats
        private String BuildMILTestFloatString(TestColor color)
        {
            return (((float)color.Alpha + "-" + (float)color.Red + "-" + (float)color.Green + "-" + (float)color.Blue));
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
