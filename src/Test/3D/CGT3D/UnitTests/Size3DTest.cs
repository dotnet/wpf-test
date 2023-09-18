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
    public class Size3DTest : CoreGraphicsTest
    {
        private double _eps = Const.eps;
        private double _min = Const.min;
        private double _max = Const.max;
        private double _inf = Const.inf;
        private char _sep = Const.valueSeparator;
        private Size3D _empty = Const.sEmpty;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestCtor();
                TestProperties();
                TestEquality();
                TestCast();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
            else // priority > 0
            {
                RunTheTest2();
            }
        }

        private void TestCtor()
        {
            Log("Testing Ctors...");

            Size3D s = new Size3D();
            if (s.X != 0 || s.Y != 0 || s.Z != 0 || failOnPurpose)
            {
                AddFailure("Size3D.ctor() failed");
                Log("*** Expected: 0,0,0");
                Log("***   Actual: {0}", s);
            }

            TestCtorWith(0, 0, 0);
            TestCtorWith(3, 3, 3);
            TestCtorWith(_max, _max, _eps);
        }

        private void TestCtorWith(double x, double y, double z)
        {
            Size3D s = new Size3D(x, y, z);

            if (MathEx.NotEquals(s.X, x) || MathEx.NotEquals(s.Y, y) ||
                 MathEx.NotEquals(s.Z, z) || failOnPurpose)
            {
                AddFailure("3-param ctor for Size3D failed");
                Log("*** Expected: ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual:   ( {0}, {1}, {2} )", s.X, s.Y, s.Z);
            }
        }

        private void TestProperties()
        {
            TestIsEmpty();
            TestGetSet();
        }

        private void TestIsEmpty()
        {
            Log("Testing IsEmpty...");

            TestIsEmptyWith(Const.sEmpty);
            TestIsEmptyWith(Const.s0);
            TestIsEmptyWith(Const.s10);
        }

        private void TestIsEmptyWith(Size3D s)
        {
            bool theirAnswer = s.IsEmpty;
            bool myAnswer = double.IsNegativeInfinity(s.X) &&
                            double.IsNegativeInfinity(s.Y) &&
                            double.IsNegativeInfinity(s.Z);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsEmpty failed");
                Log("*** Size     = " + s);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer);
            }
        }

        private void TestGetSet()
        {
            Log("Testing get/set...");

            TestGetSetWith(0, 0, 0);
            TestGetSetWith(9120.1234, 12839401234, 128394.123489);
            TestGetSetWith(_max, _eps, _max);
        }

        private void TestGetSetWith(double x, double y, double z)
        {
            Size3D s = new Size3D();

            s.X = x;
            s.Y = y;
            s.Z = z;

            if (MathEx.NotEquals(s.X, x) || MathEx.NotEquals(s.Y, y) ||
                 MathEx.NotEquals(s.Z, z) || failOnPurpose)
            {
                AddFailure("get/set failed");
                Log("*** Expected = {0},{1},{2}", x, y, z);
                Log("*** Actual   = {0},{1},{2}", s.X, s.Y, s.Z);
            }
        }

        private void TestCast()
        {
            Log("Testing explicit casting...");

            TestCastWith(Const.s0);
            TestCastWith(Const.s10);
            TestCastWith(Const.sMax);
            TestCastWith(Const.sEmpty);
            TestCastWith(Const.sEps);
        }

        private void TestCastWith(Size3D s)
        {
            try
            {
                Point3D p = (Point3D)s;

                if (MathEx.NotEquals(s.X, p.X) || MathEx.NotEquals(s.Y, p.Y) ||
                     MathEx.NotEquals(s.Z, p.Z) || failOnPurpose)
                {
                    AddFailure("Cast to Point3D failed.");
                    Log("*** Expected = ( {0}, {1}, {2} )", s.X, s.Y, s.Z);
                    Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                }
            }
            catch (Exception ex)
            {
                AddFailure("Cast exception thrown\r\n" + ex);
            }

            try
            {
                Vector3D v = (Vector3D)s;

                if (MathEx.NotEquals(s.X, v.X) || MathEx.NotEquals(s.Y, v.Y) ||
                     MathEx.NotEquals(s.Z, v.Z) || failOnPurpose)
                {
                    AddFailure("Cast to Vector3D failed.");
                    Log("*** Expected = ( {0}, {1}, {2} )", s.X, s.Y, s.Z);
                    Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                }
            }
            catch (Exception ex)
            {
                AddFailure("Cast exception thrown\r\n" + ex);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equality operators");

            TestEqualityWith(Const.s0, Const.s0);
            TestEqualityWith(Size3D.Empty, Size3D.Empty);
            TestEqualityWith(Size3D.Empty, Const.s10);
            TestEqualityWith(new Size3D(1, 2, 3), new Size3D(2, 2, 3));
            TestEqualityWith(new Size3D(1, 2, 3), new Size3D(1, 3, 3));
            TestEqualityWith(new Size3D(1, 2, 3), new Size3D(1, 2, 4));
        }

        private void TestEqualityWith(Size3D s1, Size3D s2)
        {
            bool theirAnswer1 = s1.Equals(s2);
            bool theirAnswer2 = Size3D.Equals(s1, s2);
            bool theirAnswer3 = s1 == s2;
            bool theirNotAnswer3 = s1 != s2;
            bool myAnswer12 = !MathEx.NotEquals(s1, s2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(s1, s2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( Size3D ) failed");
                Log("*** Size 1   = " + s1);
                Log("*** Size 2   = " + s2);
                Log("*** Expected: " + myAnswer12);
                Log("*** Actual:   " + theirAnswer1);
            }
            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Static Equals( Size3D,Size3D ) failed");
                Log("*** Size 1   = " + s1);
                Log("*** Size 2   = " + s2);
                Log("*** Expected: " + myAnswer12);
                Log("*** Actual:   " + theirAnswer2);
            }
            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("operator == failed");
                Log("*** Size 1   = " + s1);
                Log("*** Size 2   = " + s2);
                Log("*** Expected: " + myAnswer3);
                Log("*** Actual:   " + theirAnswer3);
            }
            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("operator != failed");
                Log("*** Size 1   = " + s1);
                Log("*** Size 2   = " + s2);
                Log("*** Expected: " + !myAnswer3);
                Log("*** Actual:   " + theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = s1.Equals(null);
                bool theirAnswer5 = s1.Equals(true);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Size   = ( {0} )", s1);
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Size   = ( {0} )", s1);
                    Log("*** object = ( true )");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("Empty");
            TestParseWith("0" + _sep + "0" + _sep + "0");
            TestParseWith("1.0	" + _sep + " 90" + _sep + "	.234");
            TestParseWith("89." + _sep + " 45e+34" + _sep + " 2");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size3D s1 = Size3D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size3D s2 = StringConverter.ToSize3D(invariant);

            if (MathEx.NotEquals(s1, s2) || failOnPurpose)
            {
                AddFailure("Size3D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", s2);
                Log("*** Actual   = {0}", s1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.s0);
            TestToStringWith(Const.sEmpty);
            TestToStringWith(Const.s10);
            TestToStringWith(new Size3D(123.345, 340e+48, .20340230402));
        }

        private void TestToStringWith(Size3D s)
        {
            string theirAnswer = s.ToString();

            // Don't want these to be affected by locale yet
            string myX = s.X.ToString(CultureInfo.InvariantCulture);
            string myY = s.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = s.Z.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myX + _sep + myY + _sep + myZ;
            if (s.IsEmpty)
            {
                myAnswer = "Empty";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Size3D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = s.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Size3D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)s).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = s.X.ToString("N", numberFormat);
            myY = s.Y.ToString("N", numberFormat);
            myZ = s.Z.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = myX + _sep + myY + _sep + myZ;
            if (s.IsEmpty)
            {
                myAnswer = "Empty";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Size3D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.s0.GetHashCode();
            int hash2 = Const.sEmpty.GetHashCode();
            int hash3 = Const.sNaN.GetHashCode();
            int hash4 = Const.sEps.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }

        private void RunTheTest2()
        {
            TestCtor2();
            TestGetSet2();
            TestEquality2();
            TestCast2();
            TestParse2();
            TestToString2();
        }

        private void TestCtor2()
        {
            Log("Testing Ctor with bad parameters...");

            Try(ConstructorNegativeX, typeof(ArgumentException));
            Try(ConstructorNegativeY, typeof(ArgumentException));
            Try(ConstructorNegativeZ, typeof(ArgumentException));
            Try(ConstructorNegativeInf, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Size3D constructor

        private void ConstructorNegativeX() { Size3D s = new Size3D(-1, 0, 0); }
        private void ConstructorNegativeY() { Size3D s = new Size3D(0, -1, 0); }
        private void ConstructorNegativeZ() { Size3D s = new Size3D(0, 0, -1); }
        private void ConstructorNegativeInf() { Size3D s = new Size3D(-_inf, -_inf, -_inf); }

        #endregion

        private void TestGetSet2()
        {
            Log("Testing get/set with bad parameters...");

            Try(SetEmptySizeX, typeof(InvalidOperationException));
            Try(SetEmptySizeY, typeof(InvalidOperationException));
            Try(SetEmptySizeZ, typeof(InvalidOperationException));
            Try(SetNegativeSizeX, typeof(ArgumentException));
            Try(SetNegativeSizeY, typeof(ArgumentException));
            Try(SetNegativeSizeZ, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Size3D Location and Size properties

        private void SetEmptySizeX() { _empty.X = 0; }
        private void SetEmptySizeY() { _empty.Y = 0; }
        private void SetEmptySizeZ() { _empty.Z = 0; }

        private void SetNegativeSizeX()
        {
            Size3D s = Const.s0;
            s.X = -1;
        }
        private void SetNegativeSizeY()
        {
            Size3D s = Const.s0;
            s.Y = -1;
        }
        private void SetNegativeSizeZ()
        {
            Size3D s = Const.s0;
            s.Z = -1;
        }

        #endregion

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.sNaN, Const.sNaN );
            TestEqualityWith(Const.sNaN, Const.sInf);
            TestEqualityWith(Const.sInf, Const.sInf);
        }

        private void TestCast2()
        {
            Log("Testing explicit casting with Bad Params...");

            TestCastWith(Const.sNaN);
            TestCastWith(Const.sInf);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "NaN");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.sNaN);
            TestToStringWith(Const.sInf);
        }
    }
}