// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   We need our own TestColor type to make the test machinery independent of dev's.
 *   TestColor is a struct (instead of a class) because currently the CLR does not 
 *   consolidate small objects.  Because test cases make so many Color objects
 *   we got random Out Of Memory exceptions before converting to struct.
 *
 ************************************************************/

using System;

namespace Microsoft.Test.Graphics
{
    internal struct TestColor
    {

        public enum Curve { Gamma_1_0, Gamma_2_2 }

        // normal 32bit ARGB color representation

        public TestColor(uint c)
            : this((byte)((c >> 24) & 0xFF), (byte)((c >> 16) & 0xFF), (byte)((c >> 8) & 0xFF), (byte)((c >> 0) & 0xFF))
        {
        }

        public TestColor(double a, double r, double g, double b, Curve m)
        {
            _alpha = a;
            _red = r;
            _green = g;
            _blue = b;
            _gamma = m;
        }

        public TestColor(byte a, byte r, byte g, byte b)
        {
            _alpha = a / 255.0;
            _red = r / 255.0;
            _green = g / 255.0;
            _blue = b / 255.0;
            _gamma = Curve.Gamma_2_2;
        }

        public TestColor Convert(Curve m)
        {
            if (m == _gamma)
            {
                return this;
            }
            else if (m == Curve.Gamma_1_0)
            {
                return new TestColor(_alpha, To_Gamma_1_0(_red), To_Gamma_1_0(_green), To_Gamma_1_0(_blue), m);
            }
            else
            {
                return new TestColor(_alpha, To_Gamma_2_2(_red), To_Gamma_2_2(_green), To_Gamma_2_2(_blue), m);
            }
        }

        public System.Windows.Media.Color CreateWCPColor()
        {
            System.Windows.Media.Color retval = new System.Windows.Media.Color();
            if (_gamma == Curve.Gamma_2_2)
            {
                // Use A, R, G, B properties for Gamma 2.2
                retval.A = (byte)ToByte(_alpha);
                retval.R = (byte)ToByte(_red);
                retval.G = (byte)ToByte(_green);
                retval.B = (byte)ToByte(_blue);
            }
            else
            {
                // Use ScA, ScR, ScG, ScB properties for Gamma 1.0
                retval.ScA = (float)_alpha;
                retval.ScR = (float)_red;
                retval.ScG = (float)_green;
                retval.ScB = (float)_blue;
            }
            return retval;
        }

        public static TestColor CreateBlack()
        {
            TestColor retval = new TestColor(0xFF000000);
            return retval;
        }

        public static TestColor CreateBlack(Curve gamma)
        {
            return CreateBlack().Convert(gamma);
        }

        public static TestColor CreateWhite()
        {
            TestColor retval = new TestColor(0xFFFFFFFF);
            return retval;
        }

        public static TestColor CreateWhite(Curve gamma)
        {
            return CreateWhite().Convert(gamma);
        }

        public static TestColor CreateGreen()
        {
            TestColor retval = new TestColor(0xFF00FF00);
            return retval;
        }

        public static TestColor CreateGreen(Curve gamma)
        {
            return CreateGreen().Convert(gamma);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override int GetHashCode()
        {
            return (int)ARGB;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override string ToString()
        {
            if (_gamma == Curve.Gamma_2_2)
            {
                return String.Format("{0:x8}", ARGB);
            }
            else
            {
                return String.Format("{0}-{1}-{2}-{3}", Alpha, Red, Green, Blue);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public bool Match(TestColor c, MatchLevel level)
        {
            if (_gamma != c._gamma)
            {
                throw new ApplicationException("Cannot compare TestColors with different gamma");
            }
            return IsNear(_alpha, c._alpha, level) &&
                    IsNear(_red, c._red, level) &&
                    IsNear(_green, c._green, level) &&
                    IsNear(_blue, c._blue, level);
        }

        /// <summary>
        /// Matches "this" TestColor with TestColor c, if the abs diff is less than mask
        /// </summary>
        /// <param name="c"> TestColor to be compared with</param>
        /// <param name="toleranceMask">Acceptable abs diff; null = 0 tolerance</param>
        /// <param name="includeAlpha">Whether we should inlcude alpha for the diffs</param>
        public bool Match(TestColor c, TestColor toleranceMask, bool includeAlpha)
        {
            if (_gamma != c._gamma)
            {
                throw new ApplicationException("Cannot compare TestColors with different gamma");
            }
            bool returnValue;
            returnValue =
                IsNear(_red, c._red, toleranceMask.Red) &&
                IsNear(_green, c._green, toleranceMask.Green) &&
                IsNear(_blue, c._blue, toleranceMask.Blue);

            if (includeAlpha == true)
            {
                returnValue = returnValue && IsNear(_alpha, c._alpha, toleranceMask.Alpha);
            }

            return returnValue;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public uint ARGB
        {
            get
            {
                TestColor c = this;
                if (_gamma != Curve.Gamma_2_2)
                {
                    c = Convert(Curve.Gamma_2_2);
                }
                return (ToByte(c.Alpha) << 24) | (ToByte(c.Red) << 16) | (ToByte(c.Green) << 8) | ToByte(c.Blue);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public double Alpha { get { return _alpha; } }
        public double Red { get { return _red; } }
        public double Green { get { return _green; } }
        public double Blue { get { return _blue; } }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Changing Gamma does not change the perception of the TestColor, it changes only representation.
        // That's why setter is also implemented.

        public Curve Gamma
        {
            get
            {
                return _gamma;
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Blending b over a

        public static TestColor Blend(TestColor a, TestColor b)
        {
            return Blend(a, b, 1.0);
        }


        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Blending b with additional opacity o over a

        public static TestColor Blend(TestColor a, TestColor b, double o)
        {
            if (a.Gamma != b.Gamma)
            {
                throw new ApplicationException("cannot blend TestColors with mismatching gamma");
            }

            double f0 = b.Alpha * o;
            double f1 = 1.0 - f0;
            return new TestColor(a.Alpha * f1 + f0,     // b.Alpha is already "premultiplied"
                               a.Red * f1 + b.Red * f0,
                               a.Green * f1 + b.Green * f0,
                               a.Blue * f1 + b.Blue * f0,
                               a.Gamma);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Blending b with additional opacity o over a, premultiplied version

        public static TestColor BlendPremult(TestColor a, TestColor b, double o)
        {
            if (a.Gamma != b.Gamma)
            {
                throw new ApplicationException("cannot blend TestColors with mismatching gamma");
            }

            double f0 = b.Alpha * o;            // effective b.Alpha
            double Alpha = a.Alpha * (1.0 - f0) + f0;
            return new TestColor(Alpha,
                               BlendChannel(a.Red, a.Alpha, b.Red, f0, Alpha),
                               BlendChannel(a.Green, a.Alpha, b.Green, f0, Alpha),
                               BlendChannel(a.Blue, a.Alpha, b.Blue, f0, Alpha),
                               a.Gamma);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private static double BlendChannel(double aC, double aA, double bC, double bA, double alpha)
        {
            if (alpha < 0.0000001)
            {
                return 0.0;
            }
            return (bC * bA + (1.0 - bA) * aC * aA) / alpha;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Linear interpolation.
        //  a(0) - f0 - b(1)

        public static TestColor Mix(TestColor a, TestColor b, double f0)
        {
            if (a.Gamma != b.Gamma)
            {
                throw new ApplicationException("cannot mix TestColors with mismatching gamma");
            }

            double f1 = 1.0 - f0;
            return new TestColor(a.Alpha * f1 + b.Alpha * f0,
                               a.Red * f1 + b.Red * f0,
                               a.Green * f1 + b.Green * f0,
                               a.Blue * f1 + b.Blue * f0,
                               a.Gamma);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Linear interpolation, premultiplied version.
        //  a(0) - f0 - b(1)

        public static TestColor MixPremult(TestColor a, TestColor b, double f0)
        {
            if (a.Gamma != b.Gamma)
            {
                throw new ApplicationException("cannot mix TestColors with mismatching gamma");
            }

            double f1 = 1.0 - f0;
            double alpha = a.Alpha * f1 + b.Alpha * f0;

            if (alpha < 0.0000001)
            {
                return new TestColor(0.0, 0.0, 0.0, 0.0, a.Gamma);
            }
            else
            {
                f1 *= a.Alpha;
                f0 *= b.Alpha;
                double red = a.Red * f1 + b.Red * f0;
                double green = a.Green * f1 + b.Green * f0;
                double blue = a.Blue * f1 + b.Blue * f0;
                return new TestColor(alpha, red / alpha, green / alpha, blue / alpha, a.Gamma);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Bilinear interpolation.
        //  c00(0,0) - f - c01(1,0)
        //   |              |
        //   g         +    g
        //   |              |
        //  c10(0,1) - f - c11(1,1)

        public static TestColor Mix(TestColor c00, TestColor c01, TestColor c10, TestColor c11, double f, double g)
        {
            // Not very effective, but in test code I value simplicity over effectiveness.
            TestColor c0x = Mix(c00, c01, f);
            TestColor c1x = Mix(c10, c11, f);
            return Mix(c0x, c1x, g);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Bilinear interpolation, premultiplied version.
        //  c00(0,0) - f - c01(1,0)
        //   |              |
        //   g         +    g
        //   |              |
        //  c10(0,1) - f - c11(1,1)

        public static TestColor MixPremult(TestColor c00, TestColor c01, TestColor c10, TestColor c11, double f, double g)
        {
            // Not very effective, but in test code I value simplicity over effectiveness.
            TestColor c0x = MixPremult(c00, c01, f);
            TestColor c1x = MixPremult(c10, c11, f);
            return MixPremult(c0x, c1x, g);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public static TestColor ClampedAdd(TestColor TestColor1, TestColor TestColor2)
        {
            if (TestColor1.Gamma != TestColor2.Gamma)
            {
                throw new ArgumentException("Cannot add TestColors with different gammas");
            }
            return new TestColor(
                    Math.Min(TestColor1.Alpha + TestColor2.Alpha, 1.0),
                    Math.Min(TestColor1.Red + TestColor2.Red, 1.0),
                    Math.Min(TestColor1.Green + TestColor2.Green, 1.0),
                    Math.Min(TestColor1.Blue + TestColor2.Blue, 1.0),
                    TestColor1.Gamma);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public static TestColor AbsoluteDifference(TestColor TestColor1, TestColor TestColor2)
        {
            if (TestColor1.Gamma != TestColor2.Gamma)
            {
                throw new ArgumentException("Cannot subtract TestColors with different gammas");
            }
            return new TestColor(
                            Math.Abs(TestColor1.Alpha - TestColor2.Alpha),
                            Math.Abs(TestColor1.Red - TestColor2.Red),
                            Math.Abs(TestColor1.Green - TestColor2.Green),
                            Math.Abs(TestColor1.Blue - TestColor2.Blue),
                            TestColor1.Gamma);
        }

        //----------------------------------------------------------
        // These are formulas from sRGB standard.

        private static double To_Gamma_1_0(double c)
        {
            if (c <= 0.04045)
            {
                c /= 12.92;
            }
            else
            {
                c = Math.Pow((c + 0.055) / 1.055, 2.4);
            }
            return c;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // These are formulas from sRGB standard.

        private static double To_Gamma_2_2(double c)
        {
            c = Math.Min(1.0, Math.Max(0.0, c));
            if (c <= 0.0031308)
            {
                c *= 12.92;
            }
            else
            {
                c = 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
            }
            return c;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private static uint ToByte(double c)
        {
            return (uint)(Math.Max(0.0, Math.Min(1.0, c)) * 255.0 + 0.5);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //As with the other IsNear, this code is OK for sRGB, but might not be for scRGB
        /// <summary>
        /// IsNear with a non-enum difference parameter
        /// </summary>
        private static bool IsNear(double a, double b, double acceptableDiff)
        {
            return Math.Abs(a - b) <= acceptableDiff;
        }




        private static bool IsNear(double a, double b, MatchLevel level)
        {
            switch (level)
            {
                case MatchLevel.Exact:
                    {
                        return Math.Abs(a - b) < 1.0 / 256.0;
                    }
                case MatchLevel.Excellent:
                    {
                        return Math.Abs(a - b) < 2.0 / 256.0;
                    }
                case MatchLevel.Good:
                    {
                        return Math.Abs(a - b) < 4.0 / 256.0;
                    }
                case MatchLevel.Poor:
                    {
                        return Math.Abs(a - b) < 16.0 / 256.0;
                    }
                default:
                    {
                        throw new ApplicationException("Unknown MatchLevel " + level);
                    }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private double _alpha;
        private double _red;
        private double _green;
        private double _blue;
        private Curve _gamma;
    }
}
