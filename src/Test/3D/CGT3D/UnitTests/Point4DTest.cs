// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class Point4DTest : CoreGraphicsTest
    {
        private double _eps = Const.eps;
        private double _min = Const.min;
        private double _max = Const.max;
        private double _inf = Const.inf;
        private double _nan = Const.nan;
        private char _sep = Const.valueSeparator;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTheTest2();
            }
            else
            {
                TestCtor();
                TestProperties();
                TestOffset();
                TestAdd();
                TestSubtract();
                TestMultiply();
                TestEquality();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
        }

        private void TestCtor()
        {
            Log("Testing constructors...");

            Point4D p = new Point4D();

            if (p.X != 0 || p.Y != 0 || p.Z != 0 || p.W != 0 || failOnPurpose)
            {
                AddFailure("Default ctor for Point4D failed.");
                Log("*** Expected = ( 0, 0, 0, 0 )");
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
            }

            TestCtorWith(0, 0, 0, 0);
            TestCtorWith(-25, 100, 2, 15);
            TestCtorWith(0, 0, Const.min, -25);
        }

        private void TestCtorWith(double x, double y, double z, double w)
        {
            Point4D p = new Point4D(x, y, z, w);

            if (MathEx.NotEquals(p.X, x) || MathEx.NotEquals(p.Y, y) ||
                 MathEx.NotEquals(p.Z, z) || MathEx.NotEquals(p.W, w) || failOnPurpose)
            {
                AddFailure("ctor for Point4D with x, y, z, w failed.");
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", x, y, z, w);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties...");

            TestPropsWith(10.23457, -10.83549, 100.2348590, 1109);
            TestPropsWith(_max, _min, _eps, -_eps);
        }

        private void TestPropsWith(double x, double y, double z, double w)
        {
            Point4D p = new Point4D();
            p.X = x;
            p.Y = y;
            p.Z = z;
            p.W = w;

            if (MathEx.NotEquals(p, new Point4D(x, y, z, w)) || failOnPurpose)
            {
                AddFailure("At least one Property for Point4D failed to set.");
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", x, y, z, w);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
            }
        }

        private void TestOffset()
        {
            Log("Testing Offset...");

            TestOffsetWith(Const.p4_0, 0, 0, 0, 0);
            TestOffsetWith(Const.p4_0, 100.45890, -100.234234, 129034.1234234, 23458923.23485902345);
            TestOffsetWith(Const.p4_0, _min, _max, _eps, -_eps);
            TestOffsetWith(new Point4D(_max, _max, _max, _max), 1.0, _max, _min, _eps);
            TestOffsetWith(new Point4D(_min, _min, _min, _min), -1.0, -_max, -_min, -_eps);
        }

        private void TestOffsetWith(Point4D p, double dx, double dy, double dz, double dw)
        {
            Point4D pCopy = p;
            pCopy.X += dx;
            pCopy.Y += dy;
            pCopy.Z += dz;
            pCopy.W += dw;
            p.Offset(dx, dy, dz, dw);

            if (MathEx.NotEquals(p, pCopy) || failOnPurpose)
            {
                Log("Offset failed.");
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", pCopy.X, pCopy.Y, pCopy.Z, pCopy.W);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
            }
        }

        private void TestAdd()
        {
            Log("Testing Addition...");

            TestAddWith(Const.p4_0, Const.p4_0);
            TestAddWith(Const.p4_1, Const.p4_0);
            TestAddWith(Const.p4_10, Const.p4_10);
            TestAddWith(Const.p4_Eps, Const.p4_Neg1);
        }

        private void TestAddWith(Point4D p1, Point4D p2)
        {
            Point4D p3 = p1 + p2;
            Point4D p4 = new Point4D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z, p1.W + p2.W);

            if (MathEx.NotEquals(p3, p4) || failOnPurpose)
            {
                AddFailure("operator + failed");
                Log("*** Expected = {0}, {1}, {2}, {3}", p4.X, p4.Y, p4.Z, p4.W);
                Log("*** Actual   = {0}, {1}, {2}, {3}", p3.X, p3.Y, p3.Z, p3.W);
            }

            p3 = Point4D.Add(p1, p2);

            if (MathEx.NotEquals(p3, p4) || failOnPurpose)
            {
                AddFailure("Point4D.Add failed");
                Log("*** Expected = {0}, {1}, {2}, {3}", p4.X, p4.Y, p4.Z, p4.W);
                Log("*** Actual   = {0}, {1}, {2}, {3}", p3.X, p3.Y, p3.Z, p3.W);
            }
        }

        private void TestSubtract()
        {
            Log("Testing Subtraction...");

            TestSubtractWith(Const.p4_0, Const.p4_0);
            TestSubtractWith(Const.p4_1, Const.p4_0);
            TestSubtractWith(Const.p4_10, Const.p4_10);
            TestSubtractWith(Const.p4_Eps, Const.p4_Neg1);
        }

        private void TestSubtractWith(Point4D p1, Point4D p2)
        {
            Point4D p3 = p1 - p2;
            Point4D p4 = new Point4D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z, p1.W - p2.W);

            if (MathEx.NotEquals(p3, p4) || failOnPurpose)
            {
                AddFailure("operator - failed");
                Log("*** Expected = {0}, {1}, {2}, {3}", p4.X, p4.Y, p4.Z, p4.W);
                Log("*** Actual   = {0}, {1}, {2}, {3}", p3.X, p3.Y, p3.Z, p3.W);
            }

            p3 = Point4D.Subtract(p1, p2);

            if (MathEx.NotEquals(p3, p4) || failOnPurpose)
            {
                AddFailure("Point4D.Subtract failed");
                Log("*** Expected = {0}, {1}, {2}, {3}", p4.X, p4.Y, p4.Z, p4.W);
                Log("*** Actual   = {0}, {1}, {2}, {3}", p3.X, p3.Y, p3.Z, p3.W);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Multiplication...");

            TestMultiplyWith(Const.p4_0, Const.tt10.Value);
            TestMultiplyWith(Const.p4_1, Const.tt10.Value);
            TestMultiplyWith(Const.p4_10, Const.rtZ135.Value);
            TestMultiplyWith(Const.p4_Neg1, Const.stNeg1.Value);
            TestMultiplyWith(Const.p4_1, Const.mAffine);
            TestMultiplyWith(new Point4D(1, 1, 1, 1), Const.mNAffine);
            TestMultiplyWith(new Point4D(1, 1, 1, .5), Const.mNAffine);
            TestMultiplyWith(new Point4D(1, 1, 1, -23.4), Const.mNAffine);
        }

        private void TestMultiplyWith(Point4D p, Matrix3D m)
        {
            Point4D pRes = p * m;
            Point4D pRes2 = Point4D.Multiply(p, m);
            double x = p.X * m.M11 + p.Y * m.M21 + p.Z * m.M31 + p.W * m.OffsetX;
            double y = p.X * m.M12 + p.Y * m.M22 + p.Z * m.M32 + p.W * m.OffsetY;
            double z = p.X * m.M13 + p.Y * m.M23 + p.Z * m.M33 + p.W * m.OffsetZ;
            double w = p.X * m.M14 + p.Y * m.M24 + p.Z * m.M34 + p.W * m.M44;

            if (MathEx.NotEquals(pRes, new Point4D(x, y, z, w)) || failOnPurpose)
            {
                AddFailure("operator * failed.");
                Log("*** Point  = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
                Log("*** Matrix =\r\n{0}", MatrixUtils.ToStr(m));
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", x, y, z, w);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", pRes.X, pRes.Y, pRes.Z, pRes.W);
            }

            if (MathEx.NotEquals(pRes2, new Point4D(x, y, z, w)) || failOnPurpose)
            {
                AddFailure("Point4D.Multiply failed.");
                Log("*** Point  = ( {0}, {1}, {2}, {3} )", p.X, p.Y, p.Z, p.W);
                Log("*** Matrix =\r\n{0}", MatrixUtils.ToStr(m));
                Log("*** Expected = ( {0}, {1}, {2}, {3} )", x, y, z, w);
                Log("*** Actual   = ( {0}, {1}, {2}, {3} )", pRes2.X, pRes2.Y, pRes2.Z, pRes2.W);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equals...");

            TestEqualityWith(Const.p4_0, Const.p4_0);
            TestEqualityWith(Const.p4_1, Const.p4_1);
            TestEqualityWith(new Point4D(1, 2, 3, 4), new Point4D(2, 2, 3, 4));
            TestEqualityWith(new Point4D(1, 2, 3, 4), new Point4D(1, 3, 3, 4));
            TestEqualityWith(new Point4D(1, 2, 3, 4), new Point4D(1, 2, 4, 4));
            TestEqualityWith(new Point4D(1, 2, 3, 4), new Point4D(1, 2, 3, 5));
        }

        private void TestEqualityWith(Point4D p1, Point4D p2)
        {
            bool theirAnswer1 = Point4D.Equals(p1, p2);
            bool theirAnswer2 = p1.Equals(p2);
            bool theirAnswer3 = p1 == p2;
            bool theirNotAnswer3 = p1 != p2;
            bool myAnswer12 = !MathEx.NotEquals(p1, p2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(p1, p2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Point4D.Equals( Point4D, Point4D ) failed.");
                Log("*** Point 1  = ( {0}, {1}, {2}, {3} )", p1.X, p1.Y, p1.Z, p1.W);
                Log("*** Point 2  = ( {0}, {1}, {2}, {3} )", p2.X, p2.Y, p2.Z, p2.W);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer1);
            }
            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( Point4D ) failed.");
                Log("*** Point 1  = ( {0}, {1}, {2}, {3} )", p1.X, p1.Y, p1.Z, p1.W);
                Log("*** Point 2  = ( {0}, {1}, {2}, {3} )", p2.X, p2.Y, p2.Z, p2.W);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer2);
            }
            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("operator == failed.");
                Log("*** Point 1  = ( {0}, {1}, {2}, {3} )", p1.X, p1.Y, p1.Z, p1.W);
                Log("*** Point 2  = ( {0}, {1}, {2}, {3} )", p2.X, p2.Y, p2.Z, p2.W);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirAnswer3);
            }
            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("operator != failed.");
                Log("*** Point 1  = ( {0}, {1}, {2}, {3} )", p1.X, p1.Y, p1.Z, p1.W);
                Log("*** Point 2  = ( {0}, {1}, {2}, {3} )", p2.X, p2.Y, p2.Z, p2.W);
                Log("*** Expected = {0}", !myAnswer3);
                Log("*** Actual   = {0}", theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = p1.Equals(null);
                bool theirAnswer5 = p1.Equals(true);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Point  = ( {0} )", p1);
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Point  = ( {0} )", p1);
                    Log("*** object = ( true )");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("0" + _sep + "0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2");
            TestParseWith("1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89e+33");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point4D p1 = Point4D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point4D p2 = StringConverter.ToPoint4D(invariant);

            if (MathEx.NotEquals(p1, p2) || failOnPurpose)
            {
                AddFailure("Point4D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", p2);
                Log("*** Actual   = {0}", p1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.p4_0);
            TestToStringWith(Const.p4_Eps);
            TestToStringWith(Const.p4_Neg1);
        }

        private void TestToStringWith(Point4D p)
        {
            string theirAnswer = p.ToString();

            // Don't want these to be affected by locale yet
            string myX = p.X.ToString(CultureInfo.InvariantCulture);
            string myY = p.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = p.Z.ToString(CultureInfo.InvariantCulture);
            string myW = p.W.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ + _sep + myW, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point4D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = p.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point4D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)p).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = p.X.ToString("N", numberFormat);
            myY = p.Y.ToString("N", numberFormat);
            myZ = p.Z.ToString("N", numberFormat);
            myW = p.W.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ + _sep + myW, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point4D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.p4_0.GetHashCode();
            int hash2 = new Point4D(1, 2, 3, 4).GetHashCode();
            int hash3 = new Point4D(-_inf, _inf, _nan, _max).GetHashCode();
            int hash4 = Const.p4_10.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }

        private void RunTheTest2()
        {
            TestCtor2();
            TestAdd2();
            TestSubtract2();
            TestMultiply2();
            TestOffset2();
            TestEquality2();
            TestProperties2();
            TestParse2();
            TestToString2();
        }

        private void TestCtor2()
        {
            Log("Testing constructor with Bad Params...");

            TestCtorWith(_nan, _nan, _nan, _nan);
            TestCtorWith(_inf, _inf, _inf, _inf);
            TestCtorWith(-_inf, -_inf, -_inf, -_inf);
        }

        private void TestAdd2()
        {
            Log("Testing Add with Bad Params...");

            TestAddWith(Const.p4_NaN, Const.p4_NaN);
            TestAddWith(Const.p4_NaN, Const.p4_1);
            TestAddWith(Const.p4_Inf, Const.p4_Inf);
            TestAddWith(Const.p4_Inf, Const.p4_NaN);
            TestAddWith(Const.p4_NegInf, Const.p4_Inf);
            TestAddWith(Const.p4_NegInf, Const.p4_10);
        }

        private void TestSubtract2()
        {
            Log("Testing Subtract with Bad Params...");

            TestSubtractWith(Const.p4_NaN, Const.p4_NaN);
            TestSubtractWith(Const.p4_NaN, Const.p4_1);
            TestSubtractWith(Const.p4_Inf, Const.p4_Inf);
            TestSubtractWith(Const.p4_Inf, Const.p4_NaN);
            TestSubtractWith(Const.p4_NegInf, Const.p4_Inf);
            TestSubtractWith(Const.p4_NegInf, Const.p4_10);
        }

        private void TestMultiply2()
        {
            Log("Testing Multiply with Bad Params...");

            TestMultiplyWith(Const.p4_NaN, Const.mAffine);
            TestMultiplyWith(Const.p4_Inf, Const.mAffine);
            TestMultiplyWith(Const.p4_NegInf, Const.mAffine);
            TestMultiplyWith(Const.p4_1, Const.mNaN);
            TestMultiplyWith(Const.p4_10, Const.mInf);
            TestMultiplyWith(Const.p4_Neg1, Const.mNegInf);
        }

        private void TestOffset2()
        {
            Log("Testing Offset with Bad Params...");

            TestOffsetWith(Const.p4_1, _nan, _inf, -_inf, _nan);
            TestOffsetWith(Const.p4_NaN, _nan, _inf, -_inf, _inf);
            TestOffsetWith(Const.p4_Inf, _nan, _inf, -_inf, -_inf);
            TestOffsetWith(Const.p4_NegInf, _nan, _inf, -_inf, _nan);
        }

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.p4_NaN, Const.p4_NaN );
            TestEqualityWith(Const.p4_NaN, Const.p4_Inf);
            TestEqualityWith(Const.p4_Inf, Const.p4_Inf);
            TestEqualityWith(Const.p4_Inf, Const.p4_NegInf);
            TestEqualityWith(Const.p4_NegInf, Const.p4_NegInf);
        }

        private void TestProperties2()
        {
            Log("Testing Properties with Bad Params...");

            TestPropsWith(_nan, _inf, -_inf, _nan);
            TestPropsWith(_inf, -_inf, _nan, -_inf);
            TestPropsWith(-_inf, _nan, _inf, _inf);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity" + _sep + "NaN");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.p4_NaN);
            TestToStringWith(Const.p4_Inf);
            TestToStringWith(Const.p4_NegInf);
        }
    }
}