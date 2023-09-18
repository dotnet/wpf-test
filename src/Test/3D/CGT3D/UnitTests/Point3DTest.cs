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
    public class Point3DTest : CoreGraphicsTest
    {      
        private double _eps = Const.eps;
        private double _min = Const.min;
        private double _max = Const.max;
        private double _inf = Const.inf;
        private double _nan = double.NaN;
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
                TestCast();
                TestEquality();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
        }

        private void TestCtor()
        {
            Log("Testing constructors...");

            Point3D p = new Point3D();

            if (p.X != 0 || p.Y != 0 || p.Z != 0 || failOnPurpose)
            {
                AddFailure("Default ctor for Point3D failed.");
                Log("*** Expected = ( 0, 0, 0 )");
                Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
            }

            TestCtorWith(0, 0, 0);
            TestCtorWith(_max, _min, _eps);
        }

        private void TestCtorWith(double x, double y, double z)
        {
            Point3D p = new Point3D(x, y, z);

            if (MathEx.NotEquals(p.X, x) || MathEx.NotEquals(p.Y, y) ||
                 MathEx.NotEquals(p.Z, z) || failOnPurpose)
            {
                AddFailure("3-param ctor for Point3D failed.");
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties...");

            TestPropsWith(10.23457, -10.83549, 100.2348590);
            TestPropsWith(_max, _min, _eps);
        }

        private void TestPropsWith(double x, double y, double z)
        {
            Point3D p = new Point3D();
            p.X = x;
            p.Y = y;
            p.Z = z;

            if (MathEx.NotEquals(p.X, x) || MathEx.NotEquals(p.Y, y) ||
                 MathEx.NotEquals(p.Z, z) || failOnPurpose)
            {
                AddFailure("At least one Property for Point3D failed to set.");
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
            }
        }

        private void TestOffset()
        {
            Log("Testing Offset...");

            TestOffsetWith(Const.p0, 0, 0, 0);
            TestOffsetWith(Const.p0, 100.45890, -100.234234, 129034.1234234);
            TestOffsetWith(Const.p0, _min, _max, _eps);
            TestOffsetWith(Const.pMax, 1.0, _max, _min);
            TestOffsetWith(Const.pMin, -1.0, -_max, -_min);
        }

        private void TestOffsetWith(Point3D p, double dx, double dy, double dz)
        {
            Point3D pCopy = p;
            pCopy.X += dx;
            pCopy.Y += dy;
            pCopy.Z += dz;
            p.Offset(dx, dy, dz);

            if (MathEx.NotCloseEnough(p, pCopy) || failOnPurpose)
            {
                Log("Offset failed.");
                Log("*** Expected = ( {0}, {1}, {2} )", pCopy.X, pCopy.Y, pCopy.Z);
                Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
            }
        }

        private void TestAdd()
        {
            Log("Testing Addition...");

            TestAddWith(Const.p0, new Vector3D(10, -10, 10));
            TestAddWith(Const.pMin, Const.vMax);
            TestAddWith(Const.pMax, Const.vMax);
            TestAddWith(Const.pEps, Const.vEps);
        }

        private void TestAddWith(Point3D p, Vector3D v)
        {
            double x = p.X + v.X;
            double y = p.Y + v.Y;
            double z = p.Z + v.Z;

            Point3D pRes = p + v;
            Point3D pRes2 = Point3D.Add(p, v);

            if (MathEx.NotEquals(pRes, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator + failed.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes.X, pRes.Y, pRes.Z);
            }

            if (MathEx.NotEquals(pRes2, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Point3D.Add failed.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes2.X, pRes2.Y, pRes2.Z);
            }
        }

        private void TestSubtract()
        {
            Log("Testing Subtraction...");

            TestSubtractWith(Const.p0, new Vector3D(10, -10, 10));
            TestSubtractWith(Const.pMin, Const.vMax);
            TestSubtractWith(Const.pMax, Const.vMax);
            TestSubtractWith(Const.pEps, Const.vEps);
            TestSubtractWith(Const.p0, new Point3D(10, -10, 10));
            TestSubtractWith(Const.pMin, Const.pMax);
            TestSubtractWith(Const.pMax, Const.pMax);
            TestSubtractWith(Const.pEps, Const.pEps);
        }

        private void TestSubtractWith(Point3D p, Vector3D v)
        {
            double x = p.X - v.X;
            double y = p.Y - v.Y;
            double z = p.Z - v.Z;

            Point3D pRes = p - v;
            Point3D pRes2 = Point3D.Subtract(p, v);

            if (MathEx.NotEquals(pRes, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator - failed.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes.X, pRes.Y, pRes.Z);
            }

            if (MathEx.NotEquals(pRes2, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Point3D.Subtract failed.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes2.X, pRes2.Y, pRes2.Z);
            }
        }

        private void TestSubtractWith(Point3D p1, Point3D p2)
        {
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            double z = p1.Z - p2.Z;

            Vector3D v = p1 - p2;
            Vector3D v2 = Point3D.Subtract(p1, p2);

            if (MathEx.NotEquals(v, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator - failed.");
                Log("*** Point1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
            }

            if (MathEx.NotEquals(v2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Point3D.Subtract failed.");
                Log("*** Point1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Multiply...");

            TestMultWith(Const.p0, Const.tt1);
            TestMultWith(Const.p10, Const.tt10);
            TestMultWith(Const.p10, Const.ttMax);
            TestMultWith(Const.pMax, Const.ttMin);
            TestMultWith(Const.pNeg1, Const.ttEps);
            TestMultWith(Const.pNeg1, Const.ttNeg1);
            TestMultWith(Const.p0, Const.st1);
            TestMultWith(Const.p10, Const.st10);
            TestMultWith(Const.p10, Const.stMax);
            TestMultWith(Const.pMax, Const.stMin);
            TestMultWith(Const.pNeg1, Const.stEps);
            TestMultWith(Const.pNeg1, Const.stNeg1);
        }

        private void TestMultWith(Point3D p, Transform3D t)
        {
            TestMultWith(p, t.Value);
        }

        private void TestMultWith(Point3D p, Matrix3D m)
        {
            if (m.M14 != 0 || m.M24 != 0 || m.M34 != 0 || m.M44 != 1 || failOnPurpose)
            {
                AddFailure("Non affine transform on point.  Is this legal?");
                Log("Transform:\r\n" + MatrixUtils.ToStr(m));
            }
            double x = p.X * m.M11 + p.Y * m.M21 + p.Z * m.M31 + m.OffsetX;
            double y = p.X * m.M12 + p.Y * m.M22 + p.Z * m.M32 + m.OffsetY;
            double z = p.X * m.M13 + p.Y * m.M23 + p.Z * m.M33 + m.OffsetZ;

            Point3D pRes = p * m;
            Point3D pRes2 = Point3D.Multiply(p, m);

            if (MathEx.NotEquals(pRes, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator * failed with Matrix3D.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes.X, pRes.Y, pRes.Z);
            }

            if (MathEx.NotEquals(pRes2, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Point3D.Multiply failed with Matrix3D.");
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes2.X, pRes2.Y, pRes2.Z);
            }
        }

        private void TestCast()
        {
            Log("Testing explicit casting...");

            TestCastWith(Const.p0);
            TestCastWith(Const.p1);
            TestCastWith(Const.p10);
            TestCastWith(Const.pNeg1);
            TestCastWith(Const.pEps);
            TestCastWith(Const.pMax);
            TestCastWith(Const.pMin);
        }

        private void TestCastWith(Point3D p)
        {
            try
            {
                Vector3D v = (Vector3D)p;
                Point4D p4 = (Point4D)p;

                if (MathEx.NotEquals(v.X, p.X) || MathEx.NotEquals(v.Y, p.Y) ||
                     MathEx.NotEquals(v.Z, p.Z) || failOnPurpose)
                {
                    AddFailure("Cast to Vector3D failed.");
                    Log("*** Expected = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                    Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                }

                if (MathEx.NotEquals(p4.X, p.X) || MathEx.NotEquals(p4.Y, p.Y) ||
                     MathEx.NotEquals(p4.Z, p.Z) || MathEx.NotEquals(p4.W, 1) || failOnPurpose)
                {
                    AddFailure("Cast to Point4D failed.");
                    Log("*** Expected = ( {0}, {1}, {2}, 1 )", p.X, p.Y, p.Z);
                    Log("*** Actual   = ( {0}, {1}, {2}, {3} )", p4.X, p4.Y, p4.Z, p4.W);
                }

                if (failOnPurpose)
                {
                    throw new InvalidCastException("Invalid cast (not!)");
                }
            }
            catch (InvalidCastException ex)
            {
                AddFailure("Cast exception thrown\r\n" + ex);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equality...");

            TestEqualityWith(Const.p0, Const.p0);
            TestEqualityWith(Const.p10, Const.p10);
            TestEqualityWith(new Point3D(1, 2, 3), new Point3D(2, 2, 3));
            TestEqualityWith(new Point3D(1, 2, 3), new Point3D(1, 3, 3));
            TestEqualityWith(new Point3D(1, 2, 3), new Point3D(1, 2, 4));
        }

        private void TestEqualityWith(Point3D p1, Point3D p2)
        {
            bool theirAnswer1 = Point3D.Equals(p1, p2);
            bool theirAnswer2 = p1.Equals(p2);
            bool theirAnswer3 = p1 == p2;
            bool theirNotAnswer3 = p1 != p2;
            bool myAnswer12 = !MathEx.NotEquals(p1, p2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(p1, p2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Point3D.Equals failed.");
                Log("*** Point 1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point 2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer1);
            }

            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( object ) failed.");
                Log("*** Point 1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point 2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer2);
            }

            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("op_Equality( Point3D,Point3D ) failed.");
                Log("*** Point 1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point 2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirAnswer3);
            }

            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("op_Inequality failed.");
                Log("*** Point 1 = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                Log("*** Point 2 = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                Log("*** Expected = {0}", !myAnswer3);
                Log("*** Actual   = {0}", theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = p1.Equals(null);
                bool theirAnswer5 = p1.Equals((Vector3D)p2);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Point  = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Point  = ( {0}, {1}, {2} )", p1.X, p1.Y, p1.Z);
                    Log("*** Vector = ( {0}, {1}, {2} )", p2.X, p2.Y, p2.Z);
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + "	-1.0  " + _sep + " 90");
            TestParseWith("1. " + _sep + " .0 " + _sep + "	90.");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point3D p1 = Point3D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point3D p2 = StringConverter.ToPoint3D(invariant);

            if (MathEx.NotEquals(p1, p2) || failOnPurpose)
            {
                AddFailure("Point3D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", p2);
                Log("*** Actual   = {0}", p1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.p0);
            TestToStringWith(Const.p10);
            TestToStringWith(Const.pEps);
            TestToStringWith(Const.pMin);
        }

        private void TestToStringWith(Point3D p)
        {
            string theirAnswer = p.ToString();

            // Don't want these to be affected by locale yet
            string myX = p.X.ToString(CultureInfo.InvariantCulture);
            string myY = p.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = p.Z.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point3D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = p.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point3D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)p).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = p.X.ToString("N", numberFormat);
            myY = p.Y.ToString("N", numberFormat);
            myZ = p.Z.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Point3D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.p0.GetHashCode();
            int hash2 = Const.p10.GetHashCode();
            int hash3 = Const.pEps.GetHashCode();
            int hash4 = Const.pNaN.GetHashCode();

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
            TestCast2();
            TestParse2();
            TestToString2();
        }

        private void TestCtor2()
        {
            Log("Testing constructor with Bad Params...");

            TestCtorWith(_nan, _nan, _nan);
            TestCtorWith(_inf, _inf, _inf);
            TestCtorWith(-_inf, -_inf, -_inf);
        }

        private void TestAdd2()
        {
            Log("Testing Add with Bad Params...");

            TestAddWith(Const.pNaN, Const.vNaN);
            TestAddWith(Const.pNaN, Const.v1);
            TestAddWith(Const.pInf, Const.vInf);
            TestAddWith(Const.pInf, Const.vNaN);
            TestAddWith(Const.pNegInf, Const.vInf);
            TestAddWith(Const.pNegInf, Const.v10);
        }

        private void TestSubtract2()
        {
            Log("Testing Subtract with Bad Params...");

            TestSubtractWith(Const.pNaN, Const.vNaN);
            TestSubtractWith(Const.pNaN, Const.v1);
            TestSubtractWith(Const.pInf, Const.vInf);
            TestSubtractWith(Const.pInf, Const.vNaN);
            TestSubtractWith(Const.pNegInf, Const.vInf);
            TestSubtractWith(Const.pNegInf, Const.v10);
            TestSubtractWith(Const.pNaN, Const.pNaN);
            TestSubtractWith(Const.pNaN, Const.p1);
            TestSubtractWith(Const.pInf, Const.pInf);
            TestSubtractWith(Const.pInf, Const.pNaN);
            TestSubtractWith(Const.pNegInf, Const.pInf);
            TestSubtractWith(Const.pNegInf, Const.p10);
        }

        private void TestMultiply2()
        {
            Log("Testing Multiply with Bad Params...");

            TestMultWith(Const.pNaN, Const.mAffine);
            TestMultWith(Const.pInf, Const.mAffine);
            TestMultWith(Const.pNegInf, Const.mAffine);
            TestMultWith(Const.p1, Const.mNaN);
            TestMultWith(Const.p10, Const.mInf);
            TestMultWith(Const.pNeg1, Const.mNegInf);
        }

        private void TestOffset2()
        {
            Log("Testing Offset with Bad Params...");

            TestOffsetWith(Const.p1, _nan, _inf, -_inf);
            TestOffsetWith(Const.pNaN, _nan, _inf, -_inf);
            TestOffsetWith(Const.pInf, _nan, _inf, -_inf);
            TestOffsetWith(Const.pNegInf, _nan, _inf, -_inf);
        }

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.pNaN, Const.pNaN );
            TestEqualityWith(Const.pNaN, Const.pInf);
            TestEqualityWith(Const.pInf, Const.pInf);
            TestEqualityWith(Const.pInf, Const.pInf2);
            TestEqualityWith(Const.pInf, Const.pNegInf);
            TestEqualityWith(Const.pNegInf, Const.pNegInf);
        }

        private void TestProperties2()
        {
            Log("Testing Properties with Bad Params...");

            TestPropsWith(_nan, _inf, -_inf);
            TestPropsWith(_inf, -_inf, _nan);
            TestPropsWith(-_inf, _nan, _inf);
        }

        private void TestCast2()
        {
            Log("Testing explicit casting with Bad Params...");

            TestCastWith(Const.pNaN);
            TestCastWith(Const.pInf);
            TestCastWith(Const.pInf2);
            TestCastWith(Const.pNegInf);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.pNaN);
            TestToStringWith(Const.pInf);
            TestToStringWith(Const.pInf2);
            TestToStringWith(Const.pNegInf);
        }
    }
}