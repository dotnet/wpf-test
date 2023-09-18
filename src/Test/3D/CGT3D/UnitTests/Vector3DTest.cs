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
    public class Vector3DTest : CoreGraphicsTest
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
                TestNormalize();
                TestNegate();
                TestAdd();
                TestSubtract();
                TestMultiply();
                TestDivide();
                TestDotProduct();
                TestCrossProductandAngleBetween();
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

            Vector3D v = new Vector3D();

            if (v.X != 0 || v.Y != 0 || v.Z != 0 || failOnPurpose)
            {
                AddFailure("Default ctor for Vector3D failed.");
                Log("*** Expected = ( 0, 0, 0 )");
                Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
            }

            TestCtorWith(0, 0, 0);
            TestCtorWith(_max, _min, _eps);
        }

        private void TestCtorWith(double x, double y, double z)
        {
            Vector3D v = new Vector3D(x, y, z);

            if (MathEx.NotEquals(v.X, x) || MathEx.NotEquals(v.Y, y) ||
                 MathEx.NotEquals(v.Z, z) || failOnPurpose)
            {
                AddFailure("3-param ctor for Vector3D failed.");
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
            }

        }

        private void TestProperties()
        {
            Log("Testing Properties...");

            TestXYZWith(10.23457, -10.83549, 100.2348590);
            TestXYZWith(_max, _min, _eps);

            Vector3DCollection c = Const.vCollection;

            foreach (Vector3D v in c)
            {
                TestLengthWith(v);
                TestLengthSquaredWith(v);
            }
        }

        private void TestXYZWith(double x, double y, double z)
        {
            Vector3D v = new Vector3D();
            v.X = x;
            v.Y = y;
            v.Z = z;

            if (MathEx.NotEquals(v, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("At least one XYZ Property for Vector3D failed to set.");
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
            }
        }

        private void TestLengthWith(Vector3D v)
        {
            double length = MathEx.Length(v);
            double vLength = v.Length;

            if (MathEx.NotCloseEnough(length, vLength))
            {
                if (double.IsInfinity(vLength) && !double.IsInfinity(length))
                {
                    length = Math.Sqrt(MathEx.LengthSquared(v));
                }
            }

            if (MathEx.NotCloseEnough(length, vLength) || failOnPurpose)
            {
                AddFailure("Vector3D.Length property returned an unexpected value");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = {0}", length);
                Log("*** Actual   = {0}", vLength);
            }
        }

        private void TestLengthSquaredWith(Vector3D v)
        {
            double lengthSq = MathEx.LengthSquared(v);
            double vLengthSq = v.LengthSquared;

            if (MathEx.NotCloseEnough(lengthSq, vLengthSq) || failOnPurpose)
            {
                AddFailure("Vector3D.LengthSquared property returned an unexpected value");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = {0}", lengthSq);
                Log("*** Actual   = {0}", vLengthSq);
            }
        }

        private void TestNormalize()
        {
            Log("Testing Normalize...");

            Vector3DCollection c = Const.vCollection;

            foreach (Vector3D v in c)
            {
                TestNormalizeWith(v);
            }
        }

        private void TestNormalizeWith(Vector3D v)
        {
            Vector3D theirAnswer = v;
            theirAnswer.Normalize();
            Vector3D myAnswer = MathEx.Normalize(v);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Normalize() failed.");
                Log("*** Vector = ( {0} )", v);
                Log("*** Expected = ( {0} )", myAnswer);
                Log("*** Actual   = ( {0} )", theirAnswer);
            }
        }

        private void TestNegate()
        {
            Log("Testing Negation...");

            Vector3DCollection c = Const.vCollection;

            foreach (Vector3D v in c)
            {
                TestNegateWith(v);
            }
        }

        private void TestNegateWith(Vector3D v)
        {
            double x = -(v.X);
            double y = -(v.Y);
            double z = -(v.Z);
            Vector3D theirAnswer1 = -v;
            Vector3D theirAnswer2 = v;
            theirAnswer2.Negate();

            if (MathEx.NotEquals(theirAnswer1, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator - (negate) failed");
                Log("*** Expected = ( {0},{1},{2} )", x, y, z);
                Log("*** Actual   = ( {0} )", theirAnswer1);
            }

            if (MathEx.NotEquals(theirAnswer2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Negate() failed");
                Log("*** Expected = ( {0},{1},{2} )", x, y, z);
                Log("*** Actual   = ( {0} )", theirAnswer2);
            }
        }

        private void TestAdd()
        {
            Log("Testing Add...");

            TestAddWith(Const.v0, new Vector3D(10, -10, 10));
            TestAddWith(Const.vMin, Const.vMax);
            TestAddWith(Const.vMax, Const.vMax);
            TestAddWith(Const.vEps, Const.vEps);

            TestAddWith(Const.v0, new Point3D(10, -10, 10));
            TestAddWith(Const.vMin, Const.pMax);
            TestAddWith(Const.vMax, Const.pMax);
            TestAddWith(Const.vEps, Const.pEps);
        }

        private void TestAddWith(Vector3D v1, Vector3D v2)
        {
            double x = v1.X + v2.X;
            double y = v1.Y + v2.Y;
            double z = v1.Z + v2.Z;

            Vector3D vRes = v1 + v2;
            Vector3D vRes2 = Vector3D.Add(v1, v2);

            if (MathEx.NotEquals(vRes, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator + failed.");
                Log("*** Vector1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes.X, vRes.Y, vRes.Z);
            }

            if (MathEx.NotEquals(vRes2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Add failed.");
                Log("*** Vector1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes2.X, vRes2.Y, vRes2.Z);
            }
        }

        private void TestAddWith(Vector3D v, Point3D p)
        {
            double x = v.X + p.X;
            double y = v.Y + p.Y;
            double z = v.Z + p.Z;

            Point3D pRes = v + p;
            Point3D pRes2 = Vector3D.Add(v, p);

            if (MathEx.NotEquals(pRes, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator + failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes.X, pRes.Y, pRes.Z);
            }

            if (MathEx.NotEquals(pRes2, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Add failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes2.X, pRes2.Y, pRes2.Z);
            }
        }

        private void TestSubtract()
        {
            Log("Testing Subtract...");

            TestSubtractWith(Const.v0, new Vector3D(10, -10, 10));
            TestSubtractWith(Const.vMin, Const.vMax);
            TestSubtractWith(Const.vMax, Const.vMax);
            TestSubtractWith(Const.vEps, Const.vEps);

            TestSubtractWith(Const.v0, new Point3D(10, -10, 10));
            TestSubtractWith(Const.vMin, Const.pMax);
            TestSubtractWith(Const.vMax, Const.pMax);
            TestSubtractWith(Const.vEps, Const.pEps);
        }

        private void TestSubtractWith(Vector3D v1, Vector3D v2)
        {
            double x = v1.X - v2.X;
            double y = v1.Y - v2.Y;
            double z = v1.Z - v2.Z;

            Vector3D vRes = v1 - v2;
            Vector3D vRes2 = Vector3D.Subtract(v1, v2);

            if (MathEx.NotEquals(vRes, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator - (subtract) failed.");
                Log("*** Vector1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes.X, vRes.Y, vRes.Z);
            }

            if (MathEx.NotEquals(vRes2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Subtract failed.");
                Log("*** Vector1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes2.X, vRes2.Y, vRes2.Z);
            }
        }

        private void TestSubtractWith(Vector3D v, Point3D p)
        {
            double x = v.X - p.X;
            double y = v.Y - p.Y;
            double z = v.Z - p.Z;

            Point3D pRes = v - p;
            Point3D pRes2 = Vector3D.Subtract(v, p);

            if (MathEx.NotEquals(pRes, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator - (subtract) failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes.X, pRes.Y, pRes.Z);
            }

            if (MathEx.NotEquals(pRes2, new Point3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Subtract failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Point  = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", pRes2.X, pRes2.Y, pRes2.Z);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Multiply...");

            TestMultWith(Const.v0, 0);
            TestMultWith(Const.v1, 0);
            TestMultWith(Const.vNeg1, 100);
            TestMultWith(Const.vMax, 2);
            TestMultWith(Const.vMin, -0.5);
            TestMultWith(Const.vEps, _max);
            TestMultWith(Const.vEps, _eps);
            TestMultWith(Const.v0, Const.tt1);
            TestMultWith(Const.v10, Const.tt10);
            TestMultWith(Const.v10, Const.ttMax);
            TestMultWith(Const.vMax, Const.ttMin);
            TestMultWith(Const.vNeg1, Const.ttEps);
            TestMultWith(Const.vNeg1, Const.ttNeg1);
            TestMultWith(Const.v0, Const.st1);
            TestMultWith(Const.v10, Const.st10);
            TestMultWith(Const.v10, Const.stMax);
            TestMultWith(Const.vMax, Const.stMin);
            TestMultWith(Const.vNeg1, Const.stEps);
            TestMultWith(Const.vNeg1, Const.stNeg1);
        }

        private void TestMultWith(Vector3D v, double d)
        {
            double x = v.X * d;
            double y = v.Y * d;
            double z = v.Z * d;

            Vector3D vRes = v * d;
            Vector3D vRes2 = d * v;
            Vector3D vRes3 = Vector3D.Multiply(v, d);
            Vector3D vRes4 = Vector3D.Multiply(d, v);

            if (MathEx.NotEquals(vRes, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator * (v * scalar) failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes.X, vRes.Y, vRes.Z);
            }

            if (MathEx.NotEquals(vRes2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator * (scalar * v) failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes2.X, vRes2.Y, vRes2.Z);
            }

            if (MathEx.NotEquals(vRes3, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Multiply (v * scalar) failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes3.X, vRes3.Y, vRes3.Z);
            }

            if (MathEx.NotEquals(vRes4, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Multiply (scalar * v) failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes4.X, vRes4.Y, vRes4.Z);
            }
        }

        private void TestMultWith(Vector3D v, Transform3D t)
        {
            TestMultWith(v, t.Value);
        }

        private bool TestMultWith(Vector3D v, Matrix3D m)
        {
            bool retval = true;

            if (m.M14 != 0 || m.M24 != 0 || m.M34 != 0 || m.M44 != 1 || failOnPurpose)
            {
                AddFailure("Non affine transform on vector.  Is this legal?");
                Log("Transform:\r\n" + MatrixUtils.ToStr(m));
                retval = false;
            }
            double x = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            double y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            double z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;

            Vector3D vRes = v * m;
            Vector3D vRes2 = Vector3D.Multiply(v, m);

            if (MathEx.NotEquals(vRes, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator * failed with Matrix3D.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes.X, vRes.Y, vRes.Z);
                retval = false;
            }

            if (MathEx.NotEquals(vRes2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Multiply failed with Matrix3D.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", vRes2.X, vRes2.Y, vRes2.Z);
                retval = false;
            }

            return retval;
        }

        private void TestDivide()
        {
            Log("Testing Divide...");

            TestDivideWith(Const.v0, 0);
            TestDivideWith(Const.v1, 0);
            TestDivideWith(Const.vNeg1, 100);
            TestDivideWith(Const.vMax, 2);
            TestDivideWith(Const.vMin, -0.5);
            TestDivideWith(Const.vEps, _max);
            TestDivideWith(Const.vEps, _eps);
        }

        private void TestDivideWith(Vector3D v, double d)
        {
            Vector3D theirAnswer1 = v / d;
            Vector3D theirAnswer2 = Vector3D.Divide(v, d);

            double x = v.X / d;
            double y = v.Y / d;
            double z = v.Z / d;

            if (MathEx.NotEquals(theirAnswer1, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("operator / failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", theirAnswer1.X, theirAnswer1.Y, theirAnswer1.Z);
            }

            if (MathEx.NotEquals(theirAnswer2, new Vector3D(x, y, z)) || failOnPurpose)
            {
                AddFailure("Vector3D.Divide failed.");
                Log("*** Vector = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                Log("*** Scalar = {0}", d);
                Log("*** Expected = ( {0}, {1}, {2} )", x, y, z);
                Log("*** Actual   = ( {0}, {1}, {2} )", theirAnswer2.X, theirAnswer2.Y, theirAnswer2.Z);
            }
        }

        private void TestDotProduct()
        {
            Log("Testing Dot Product...");

            TestDotProductWith(Const.v0, Const.v1);
            TestDotProductWith(Const.vMax, Const.v1);
            TestDotProductWith(Const.vMin, Const.vNeg1);
            TestDotProductWith(Const.vEps, Const.v10);
            TestDotProductWith(Const.vEps, Const.vMax);
        }

        private void TestDotProductWith(Vector3D v1, Vector3D v2)
        {
            double dotProd = MathEx.DotProduct(v1, v2);
            double vDotProd = Vector3D.DotProduct(v1, v2);

            if (MathEx.NotCloseEnough(dotProd, vDotProd) || failOnPurpose)
            {
                AddFailure("Vector3D.DotProduct failed");
                Log("*** Vector1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = {0}", dotProd);
                Log("*** Actual   = {0}", vDotProd);
            }
        }

        private void TestCrossProductandAngleBetween()
        {
            Log("Testing CrossProduct and AngleBetween...");

            TestCPABWith(Const.v0, Const.v0);
            TestCPABWith(Const.v1, Const.v1);
            TestCPABWith(new Vector3D(-1, 0, 2), Const.v1);
            TestCPABWith(new Vector3D(0, 0, 1), new Vector3D(0, 1, 0));
            TestCPABWith(new Vector3D(1, 0, 0), new Vector3D(0, 0, -1));
            TestCPABWith(new Vector3D(Math.Sin(Math.PI / 4), Math.Sin(Math.PI / 4), 0), new Vector3D(0, 0, 1));
        }

        private void TestCPABWith(Vector3D v1, Vector3D v2)
        {
            Vector3D vCProd = Vector3D.CrossProduct(v1, v2);
            Vector3D myCProd = MathEx.CrossProduct(v1, v2);

            if (MathEx.NotCloseEnough(vCProd, myCProd) || failOnPurpose)
            {
                AddFailure("Vector3D.CrossProduct failed.");
                Log("*** Vector1 = ( {0} )", v1);
                Log("*** Vector2 = ( {0} )", v2);
                Log("*** Expected = ( {0} )", myCProd);
                Log("*** Actual   = ( {0} )", vCProd);
            }

            double theirAngle1 = Vector3D.AngleBetween(v1, v2);
            double theirAngle2 = Vector3D.AngleBetween(v2, v1);
            double myAngle1 = MathEx.AngleBetween(v1, v2);
            double myAngle2 = MathEx.AngleBetween(v2, v1);

            if (MathEx.NotCloseEnough(theirAngle1, myAngle1) || MathEx.NotCloseEnough(theirAngle2, myAngle2) || failOnPurpose)
            {
                AddFailure("Vector3D.AngleBetween failed");
                Log("*** Vector1 = ( {0} )", v1);
                Log("*** Vector2 = ( {0} )", v2);
                Log("*** Expected v1->v2 = {0}", myAngle1);
                Log("*** Actual   v1->v2 = {0}", theirAngle1);
                Log("*** Expected v2->v1 = {0}", myAngle2);
                Log("*** Actual   v2->v1 = {0}", theirAngle2);
            }
        }

        private void TestCast()
        {
            Log("Testing explicit casting...");

            Vector3DCollection c = Const.vCollection;

            foreach (Vector3D v in c)
            {
                TestCastWith(v);
            }
        }

        private void TestCastWith(Vector3D v)
        {
            try
            {
                Point3D p = (Point3D)v;

                if (MathEx.NotEquals(v.X, p.X) || MathEx.NotEquals(v.Y, p.Y) ||
                     MathEx.NotEquals(v.Z, p.Z) || failOnPurpose)
                {
                    AddFailure("Cast to Point3D failed.");
                    Log("*** Expected = ( {0}, {1}, {2} )", v.X, v.Y, v.Z);
                    Log("*** Actual   = ( {0}, {1}, {2} )", p.X, p.Y, p.Z);
                }

                Size3D s = (Size3D)v;

                if (MathEx.NotEquals(s.X, Math.Abs(v.X)) || MathEx.NotEquals(s.Y, Math.Abs(v.Y)) ||
                     MathEx.NotEquals(s.Z, Math.Abs(v.Z)) || failOnPurpose)
                {
                    AddFailure("Cast to Size3D failed.");
                    Log("*** Expected = ( {0}, {1}, {2} )", Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
                    Log("*** Actual   = ( {0}, {1}, {2} )", s.X, s.Y, s.Z);
                }
            }
            catch (Exception ex)
            {
                AddFailure("Cast exception thrown\r\n" + ex);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equals...");

            TestEqualityWith(Const.v0, Const.v0);
            TestEqualityWith(Const.v10, Const.v10);
            TestEqualityWith(new Vector3D(1, 2, 3), new Vector3D(2, 2, 3));
            TestEqualityWith(new Vector3D(1, 2, 3), new Vector3D(1, 3, 3));
            TestEqualityWith(new Vector3D(1, 2, 3), new Vector3D(1, 2, 4));
        }

        private void TestEqualityWith(Vector3D v1, Vector3D v2)
        {
            bool theirAnswer1 = Vector3D.Equals(v1, v2);
            bool theirAnswer2 = v1.Equals(v2);
            bool theirAnswer3 = v1 == v2;
            bool theirNotAnswer3 = v1 != v2;
            bool myAnswer12 = !MathEx.NotEquals(v1, v2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(v1, v2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Vector3D.Equals( Vector3D,Vector3D ) failed.");
                Log("*** Vector 1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector 2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer1);
            }

            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( object ) failed.");
                Log("*** Vector 1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector 2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = {0}", myAnswer12);
                Log("*** Actual   = {0}", theirAnswer2);
            }

            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("op_Equality failed.");
                Log("*** Vector 1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector 2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirAnswer3);
            }

            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("op_Inequality failed.");
                Log("*** Vector 1 = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                Log("*** Vector 2 = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                Log("*** Expected = {0}", myAnswer3);
                Log("*** Actual   = {0}", theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = v1.Equals(null);
                bool theirAnswer5 = v1.Equals((Point3D)v2);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Vector = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Vector = ( {0}, {1}, {2} )", v1.X, v1.Y, v1.Z);
                    Log("*** Point  = ( {0}, {1}, {2} )", v2.X, v2.Y, v2.Z);
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + " -1.0	" + _sep + "	90");
            TestParseWith("1. " + _sep + " .0 " + _sep + " 90.");
        }      

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector3D v1 = Vector3D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector3D v2 = StringConverter.ToVector3D(invariant);

            if (MathEx.NotEquals(v1, v2) || failOnPurpose)
            {
                AddFailure("Vector3D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", v2);
                Log("*** Actual   = {0}", v1);
            }
        }      

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.v0);
            TestToStringWith(Const.v10);
            TestToStringWith(Const.vEps);
            TestToStringWith(Const.vMin);
        }

        private void TestToStringWith(Vector3D v)
        {
            string theirAnswer = v.ToString();

            // Don't want these to be affected by locale yet
            string myX = v.X.ToString(CultureInfo.InvariantCulture);
            string myY = v.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = v.Z.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Vector3D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = v.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Vector3D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)v).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = v.X.ToString("N", numberFormat);
            myY = v.Y.ToString("N", numberFormat);
            myZ = v.Z.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = MathEx.ToLocale(myX + _sep + myY + _sep + myZ, CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Vector3D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }      

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.v0.GetHashCode();
            int hash2 = Const.vMax.GetHashCode();
            int hash3 = Const.vNegInf.GetHashCode();
            int hash4 = Const.vNeg1.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }
      
        private void RunTheTest2()
        {
            TestCtor2();
            TestNegate2();
            TestAdd2();
            TestSubtract2();
            TestMultiply2();
            TestDivide2();
            TestEquality2();
            TestProperties2();
            TestNormalize2();
            TestDotProduct2();
            TestCrossProductandAngleBetween2();
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

        private void TestNegate2()
        {
            Log("Testing Negate with Bad Params...");

            TestNegateWith(Const.vNaN);
            TestNegateWith(Const.vInf);
            TestNegateWith(Const.vInf2);
            TestNegateWith(Const.vNegInf);
        }

        private void TestAdd2()
        {
            Log("Testing Add with Bad Params...");

            TestAddWith(Const.vNaN, Const.vNaN);
            TestAddWith(Const.vNaN, Const.v1);
            TestAddWith(Const.vInf, Const.vInf);
            TestAddWith(Const.vInf, Const.vNaN);
            TestAddWith(Const.vNegInf, Const.vInf);
            TestAddWith(Const.vNegInf, Const.v10);

            TestAddWith(Const.vNaN, Const.pNaN);
            TestAddWith(Const.vNaN, Const.p1);
            TestAddWith(Const.vInf, Const.pInf);
            TestAddWith(Const.vInf, Const.pNaN);
            TestAddWith(Const.vNegInf, Const.pInf);
            TestAddWith(Const.vNegInf, Const.p10);
        }

        private void TestSubtract2()
        {
            Log("Testing Subtract with Bad Params...");

            TestSubtractWith(Const.vNaN, Const.vNaN);
            TestSubtractWith(Const.vNaN, Const.v1);
            TestSubtractWith(Const.vInf, Const.vInf);
            TestSubtractWith(Const.vInf, Const.vNaN);
            TestSubtractWith(Const.vNegInf, Const.vInf);
            TestSubtractWith(Const.vNegInf, Const.v10);

            TestSubtractWith(Const.vNaN, Const.pNaN);
            TestSubtractWith(Const.vNaN, Const.p1);
            TestSubtractWith(Const.vInf, Const.pInf);
            TestSubtractWith(Const.vInf, Const.pNaN);
            TestSubtractWith(Const.vNegInf, Const.pInf);
            TestSubtractWith(Const.vNegInf, Const.p10);
        }

        private void TestMultiply2()
        {
            Log("Testing Multiply with Bad Params...");

            TestMultWith(Const.vNaN, 0);
            TestMultWith(Const.vInf, 0);
            TestMultWith(Const.vNegInf, 0);
            TestMultWith(Const.vNaN, 10);
            TestMultWith(Const.vInf, -1);
            TestMultWith(Const.vNegInf, 2000);
            TestMultWith(Const.vNaN, _inf);
            TestMultWith(Const.vInf, -_inf);
            TestMultWith(Const.vNegInf, _nan);

            TestMultWith(Const.vNaN, Const.mAffine);
            TestMultWith(Const.vInf, Const.mAffine);
            TestMultWith(Const.vNegInf, Const.mAffine);
            TestMultWith(Const.v1, Const.mNaN);
            TestMultWith(Const.v10, Const.mInf);
            TestMultWith(Const.vNeg1, Const.mNegInf);
        }

        private void TestDivide2()
        {
            Log("Testing Divide with Bad Params...");

            TestDivideWith(Const.vNaN, 0);
            TestDivideWith(Const.vInf, 0);
            TestDivideWith(Const.vNegInf, 0);
            TestDivideWith(Const.vNaN, 10);
            TestDivideWith(Const.vInf, -1);
            TestDivideWith(Const.vNegInf, 2000);
            TestDivideWith(Const.vNaN, _inf);
            TestDivideWith(Const.vInf, -_inf);
            TestDivideWith(Const.vNegInf, _nan);
        }

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.vNaN, Const.vNaN );
            TestEqualityWith(Const.vNaN, Const.vInf);
            TestEqualityWith(Const.vInf, Const.vInf);
            TestEqualityWith(Const.vInf, Const.vInf2);
            TestEqualityWith(Const.vInf, Const.vNegInf);
            TestEqualityWith(Const.vNegInf, Const.vNegInf);
        }

        private void TestProperties2()
        {
            Log("Testing Properties with Bad Params...");

            TestXYZWith(_nan, _inf, -_inf);
            TestXYZWith(_inf, -_inf, _nan);
            TestXYZWith(-_inf, _nan, _inf);

            TestLengthWith(Const.vNaN);
            TestLengthWith(Const.vInf);
            TestLengthWith(Const.vNegInf);
            TestLengthWith(new Vector3D(_inf, 0, 0));
            TestLengthWith(new Vector3D(0, -_inf, 0));
            TestLengthWith(new Vector3D(0, 0, _nan));

            // overflow! but length is obviously Sqrt( max )*2
            TestLengthWith(new Vector3D(Math.Sqrt(_max) * 2, 0, 0));

            TestLengthSquaredWith(Const.vNaN);
            TestLengthSquaredWith(Const.vInf);
            TestLengthSquaredWith(Const.vNegInf);
            TestLengthSquaredWith(new Vector3D(_inf, 0, 0));
            TestLengthSquaredWith(new Vector3D(0, -_inf, 0));
            TestLengthSquaredWith(new Vector3D(0, 0, _nan));
        }

        private void TestCast2()
        {
            Log("Testing explicit casting with Bad Params...");

            TestCastWith(Const.vNaN);
            TestCastWith(Const.vInf);
            TestCastWith(Const.vInf2);
            TestCastWith(Const.vNegInf);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.vNaN);
            TestToStringWith(Const.vInf);
            TestToStringWith(Const.vInf2);
            TestToStringWith(Const.vNegInf);
        }

        private void TestNormalize2()
        {
            Log("Testing Normalize with Bad Params...");

            TestNormalizeWith(Const.vNaN);
        }

        private void TestDotProduct2()
        {
            Log("Testing DotProduct with Bad Params...");

            TestDotProductWith(Const.vNaN, Const.vNaN);
            TestDotProductWith(Const.vNaN, Const.v0);
            TestDotProductWith(Const.vInf, Const.xAxis);
            TestDotProductWith(Const.vNegInf, Const.yAxis);
            TestDotProductWith(Const.vNegInf, Const.v0);
        }

        private void TestCrossProductandAngleBetween2()
        {
            Log("Testing CrossProduct and AngleBetween with Bad Params...");

            TestCPABWith(Const.vNaN, Const.xAxis);

            // The new MathEx.AngleBetween is currently better than Avalon's.
            // Commenting out these tests until we reach a consensus with them.
            //TestCPABWith( Const.vInf, Const.yAxis );
            //TestCPABWith( Const.vNegInf, Const.zAxis );
            //TestCPABWith( new Vector3D( inf,0,0 ), new Vector3D( 0,0,inf ) );
        }
    }
}