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
    public class Matrix3DTest : CoreGraphicsTest
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
                TestConstructors();
                TestIsIdentity();
                TestIsAffine();
                TestSetIdentity();
                TestProperties();
                TestDeterminant();
                TestEquality();
                TestMultiply();
                TestTransforms();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
        }

        private void TestConstructors()
        {
            Log("Testing Constructors...");

            Matrix3D m = new Matrix3D();

            if (!MatrixUtils.IsIdentity(m) || failOnPurpose)
            {
                AddFailure("Matrix3D.ctor() failed");
                Log("*** Expected: Identity Matrix");
                Log("***   Actual:\r\n" + MatrixUtils.ToStr(m));
            }

            TestConstructorWith(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            TestConstructorWith(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5);
            TestConstructorWith(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private void TestConstructorWith(double m11, double m12, double m13, double m14,
                                                     double m21, double m22, double m23, double m24,
                                                     double m31, double m32, double m33, double m34,
                                                     double m41, double m42, double m43, double m44)
        {
            Matrix3D theirAnswer = new Matrix3D(m11, m12, m13, m14, m21, m22, m23, m24, m31, m32, m33, m34, m41, m42, m43, m44);

            if (MathEx.NotEquals(theirAnswer.M11, m11) ||
                 MathEx.NotEquals(theirAnswer.M12, m12) ||
                 MathEx.NotEquals(theirAnswer.M13, m13) ||
                 MathEx.NotEquals(theirAnswer.M14, m14) ||
                 MathEx.NotEquals(theirAnswer.M21, m21) ||
                 MathEx.NotEquals(theirAnswer.M22, m22) ||
                 MathEx.NotEquals(theirAnswer.M23, m23) ||
                 MathEx.NotEquals(theirAnswer.M24, m24) ||
                 MathEx.NotEquals(theirAnswer.M31, m31) ||
                 MathEx.NotEquals(theirAnswer.M32, m32) ||
                 MathEx.NotEquals(theirAnswer.M33, m33) ||
                 MathEx.NotEquals(theirAnswer.M34, m34) ||
                 MathEx.NotEquals(theirAnswer.OffsetX, m41) ||
                 MathEx.NotEquals(theirAnswer.OffsetY, m42) ||
                 MathEx.NotEquals(theirAnswer.OffsetZ, m43) ||
                 MathEx.NotEquals(theirAnswer.M44, m44) ||
                 failOnPurpose)
            {
                AddFailure("Matrix3D.ctor( double x16 ) failed");
                string expected = string.Format(
                                        "[ {0,22}, {1,22}, {2,22}, {3,22} ]\r\n" +
                                        "[ {4,22}, {5,22}, {6,22}, {7,22} ]\r\n" +
                                        "[ {8,22}, {9,22}, {10,22}, {11,22} ]\r\n" +
                                        "[ {12,22}, {13,22}, {14,22}, {15,22} ]\r\n",
                                        m11, m12, m13, m14,
                                        m21, m22, m23, m24,
                                        m31, m32, m33, m34,
                                        m41, m42, m43, m44
                                        );
                Log("*** Expected:\r\n" + expected);
                Log("***   Actual:\r\n" + MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestIsIdentity()
        {
            Log("Testing IsIdentity...");

            TestIsIdentityWith(Matrix3D.Identity);
            TestIsIdentityWith(Const.mAffine);
            TestIsIdentityWith(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0));
        }

        private void TestIsIdentityWith(Matrix3D m)
        {
            bool theirAnswer = m.IsIdentity;
            bool myAnswer = MatrixUtils.IsIdentity(m);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsIdentity failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestIsAffine()
        {
            Log("Testing IsAffine...");

            TestIsAffineWith(Matrix3D.Identity);
            TestIsAffineWith(Const.mAffine);
            TestIsAffineWith(Const.mNAffine);
            TestIsAffineWith(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0));
        }

        private void TestIsAffineWith(Matrix3D m)
        {
            bool theirAnswer = m.IsAffine;
            bool myAnswer = MatrixUtils.IsAffine(m);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsAffine failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestSetIdentity()
        {
            Log("Testing SetIdentity...");

            Matrix3D m = Const.mNAffine;
            m.SetIdentity();

            if (!MatrixUtils.IsIdentity(m) || failOnPurpose)
            {
                AddFailure("SetIdentity failed");
                Log("*** Resulting Matrix =\r\n" + MatrixUtils.ToStr(m));
            }
        }

        private void TestProperties()
        {
            Log("Testing Identity and M11 - M44...");

            TestIdentity();
            TestMWith(1.0, "M11");
            TestMWith(4.5, "M11");
            TestMWith(0.0, "M12");
            TestMWith(-4.5, "M12");
            TestMWith(0.0, "M13");
            TestMWith(33.2, "M13");
            TestMWith(0.0, "M14");
            TestMWith(_max, "M14");
            TestMWith(0.0, "M21");
            TestMWith(_min, "M21");
            TestMWith(1.0, "M22");
            TestMWith(4.5, "M22");
            TestMWith(0.0, "M23");
            TestMWith(4.5, "M23");
            TestMWith(0.0, "M24");
            TestMWith(4.5, "M24");
            TestMWith(0.0, "M31");
            TestMWith(4.5, "M31");
            TestMWith(0.0, "M32");
            TestMWith(4.5, "M32");
            TestMWith(1.0, "M33");
            TestMWith(4.5, "M33");
            TestMWith(0.0, "M34");
            TestMWith(4.5, "M34");
            TestMWith(0.0, "OffsetX");
            TestMWith(4.5, "OffsetX");
            TestMWith(0.0, "OffsetY");
            TestMWith(4.5, "OffsetY");
            TestMWith(0.0, "OffsetZ");
            TestMWith(4.5, "OffsetZ");
            TestMWith(1.0, "M44");
            TestMWith(4.5, "M44");
        }

        private void TestIdentity()
        {
            Matrix3D m = Matrix3D.Identity;

            if (!MatrixUtils.IsIdentity(m) || failOnPurpose)
            {
                AddFailure("get_Identity failed");
                Log("*** Expected: Identity matrix");
                Log("***   Actual:\r\n" + MatrixUtils.ToStr(m));
            }
        }

        private void TestMWith(double value, string property)
        {
            Matrix3D m = Matrix3D.Identity;
            double actual = SetMWith(ref m, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private double SetMWith(ref Matrix3D m, double value, string property)
        {
            switch (property)
            {
                case "M11": m.M11 = value; return m.M11;
                case "M12": m.M12 = value; return m.M12;
                case "M13": m.M13 = value; return m.M13;
                case "M14": m.M14 = value; return m.M14;
                case "M21": m.M21 = value; return m.M21;
                case "M22": m.M22 = value; return m.M22;
                case "M23": m.M23 = value; return m.M23;
                case "M24": m.M24 = value; return m.M24;
                case "M31": m.M31 = value; return m.M31;
                case "M32": m.M32 = value; return m.M32;
                case "M33": m.M33 = value; return m.M33;
                case "M34": m.M34 = value; return m.M34;
                case "OffsetX": m.OffsetX = value; return m.OffsetX;
                case "OffsetY": m.OffsetY = value; return m.OffsetY;
                case "OffsetZ": m.OffsetZ = value; return m.OffsetZ;
                case "M44": m.M44 = value; return m.M44;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Matrix3D");
        }

        private void TestDeterminant()
        {
            Log("Testing Determinant...");

            TestDeterminantWith(Const.mAffine);
            TestDeterminantWith(Const.mIdent);
            TestDeterminantWith(Const.mNAffine);
        }

        private void TestDeterminantWith(Matrix3D m)
        {
            double theirAnswer = m.Determinant;
            double myAnswer = MatrixUtils.Determinant(m);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Matrix3D.Determinant failed");
                Log("*** Matrix:\r\n" + MatrixUtils.ToStr(m));
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equality...");

            TestEqualityWith(new Matrix3D(), Matrix3D.Identity);
            TestEqualityWith(Const.mIdent, Const.mIdent);
            TestEqualityWith(Const.mIdent, Const.mAffine);
            TestEqualityWith(Const.mAffine, Const.mNAffine);
            TestEqualityWith(new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2), Const.mIdent);
        }

        private void TestEqualityWith(Matrix3D m1, Matrix3D m2)
        {
            bool theirAnswer1 = Matrix3D.Equals(m1, m2);
            bool theirAnswer2 = m1.Equals(m2);
            bool theirAnswer3 = m1 == m2;
            bool theirNotAnswer3 = m1 != m2;
            bool myAnswer12 = MathEx.Equals(m1, m2);
            bool myAnswer3 = MathEx.ClrOperatorEquals(m1, m2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Matrix3D.Equals( Matrix3D, Matrix3D ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected = " + myAnswer12);
                Log("*** Actual   = " + theirAnswer1);
            }

            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( object ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected = " + myAnswer12);
                Log("*** Actual   = " + theirAnswer2);
            }

            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("Matrix3D.op_Equality( Matrix3D, Matrix3D ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected = " + myAnswer3);
                Log("*** Actual   = " + theirAnswer3);
            }

            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("Matrix3D.op_Inequality( Matrix3D, Matrix3D ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected = " + !myAnswer3);
                Log("*** Actual   = " + theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = m1.Equals(null);
                bool theirAnswer5 = m1.Equals(true);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m1));
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m1));
                    Log("*** object = ( true )");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestMultiply()
        {
            Log("Testing Multiply, Append, and Prepend, ...");

            TestMultiplyWith(Const.mNAffine, Const.mIdent);
            TestMultiplyWith(Const.mNAffine, new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1));
            TestMultiplyWith(new Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16),
                              new Matrix3D(21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36));
        }

        private void TestMultiplyWith(Matrix3D m1, Matrix3D m2)
        {
            Matrix3D theirAnswer1 = m1 * m2;
            Matrix3D theirAnswer2 = Matrix3D.Multiply(m1, m2);
            Matrix3D theirAnswer3 = m2;
            theirAnswer3.Prepend(m1);
            Matrix3D theirAnswer4 = m1;
            theirAnswer4.Append(m2);
            Matrix3D myAnswer = MatrixUtils.Multiply(m1, m2);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("operator * failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1));
            }

            if (MathEx.NotCloseEnough(theirAnswer1, theirAnswer2) || failOnPurpose)
            {
                AddFailure("Matrix3D.Multiply and operator * do not generate the same result");
                Log("*** operator * result        =\r\n" + MatrixUtils.ToStr(theirAnswer1));
                Log("*** Matrix3D.Multiply result =\r\n" + MatrixUtils.ToStr(theirAnswer2));
            }

            if (MathEx.NotCloseEnough(theirAnswer3, myAnswer) || failOnPurpose)
            {
                AddFailure("Prepend( Matrix3D ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer3));
            }

            if (MathEx.NotCloseEnough(theirAnswer4, myAnswer) || failOnPurpose)
            {
                AddFailure("Append( Matrix3D ) failed");
                Log("*** Matrix 1 =\r\n" + MatrixUtils.ToStr(m1));
                Log("*** Matrix 2 =\r\n" + MatrixUtils.ToStr(m2));
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer4));
            }
        }

        private void TestTransforms()
        {
            Log("Testing Rotate*...");

            TestRotateWith(Const.mIdent, Const.xAxis, 45, Const.p0);
            TestRotateWith(Const.mIdent, Const.xAxis, 180, Const.p0);
            TestRotateWith(Const.mIdent, Const.yAxis, 90, Const.p0);
            TestRotateWith(Const.mIdent, Const.zAxis, 135, Const.p0);
            TestRotateWith(Const.mIdent, Const.v1, 90, Const.p0);
            TestRotateWith(Const.mIdent, new Vector3D(3, 4, 5), 36, new Point3D(3, 4, 5));
            TestRotateWith(Const.mIdent, new Vector3D(-3, 4, 5), -54, new Point3D(2, -6, -3));
            TestRotateWith(Const.mIdent, new Vector3D(3, 4, -5), 200, new Point3D(-2, -6, 3));
            TestRotateWith(Const.mAffine, Const.xAxis, 45, Const.p0);
            TestRotateWith(Const.mAffine, Const.xAxis, 180, Const.p0);
            TestRotateWith(Const.mAffine, Const.yAxis, 90, Const.p0);
            TestRotateWith(Const.mAffine, Const.zAxis, 135, Const.p0);
            TestRotateWith(Const.mNAffine, Const.v1, 90, Const.p0);
            TestRotateWith(Const.mAffine, new Vector3D(3, 4, 5), 36, new Point3D(3, 4, 5));
            TestRotateWith(Const.mAffine, new Vector3D(-3, 4, 5), -54, new Point3D(2, -6, -3));
            TestRotateWith(Const.mNAffine, new Vector3D(3, 4, -5), 200, new Point3D(-2, -6, 3));

            Log("Testing Scale*...");

            TestScaleWith(Const.mIdent, Const.v0, Const.p0);
            TestScaleWith(Const.mIdent, Const.v10, Const.p0);
            TestScaleWith(Const.mIdent, Const.vNeg1, Const.p0);
            TestScaleWith(Const.mIdent, Const.v10, new Point3D(4, 5, -6));
            TestScaleWith(Const.mIdent, new Vector3D(89, -23, 100), new Point3D(-3, -4, 20));
            TestScaleWith(Const.mAffine, Const.v0, Const.p0);
            TestScaleWith(Const.mAffine, Const.v10, Const.p0);
            TestScaleWith(Const.mAffine, Const.vNeg1, Const.p0);
            TestScaleWith(Const.mAffine, Const.v10, new Point3D(4, 5, -6));
            TestScaleWith(Const.mNAffine, new Vector3D(89, -23, 100), new Point3D(-3, -4, 20));

            Log("Testing Translate*...");

            TestTranslateWith(Const.mIdent, Const.v0);
            TestTranslateWith(Const.mIdent, Const.v10);
            TestTranslateWith(Const.mIdent, Const.vNeg1);
            TestTranslateWith(Const.mIdent, new Vector3D(89, -23, 100));
            TestTranslateWith(Const.mAffine, Const.v0);
            TestTranslateWith(Const.mAffine, Const.v10);
            TestTranslateWith(Const.mAffine, Const.vNeg1);
            TestTranslateWith(Const.mNAffine, new Vector3D(89, -23, 100));
        }

        private void TestRotateWith(Matrix3D m, Vector3D axis, double angle, Point3D center)
        {
            Matrix3D theirAnswer1 = m;
            Matrix3D theirAnswer2 = m;
            Matrix3D theirAnswer3 = m;
            Matrix3D theirAnswer4 = m;
            Quaternion q = new Quaternion(axis, angle);

            theirAnswer1.Rotate(q);
            theirAnswer2.RotatePrepend(q);
            theirAnswer3.RotateAt(q, center);
            theirAnswer4.RotateAtPrepend(q, center);

            Matrix3D myRotate = MatrixUtils.Rotate(axis, angle);
            Matrix3D myRotateAt = MatrixUtils.Rotate(axis, angle, center);
            Matrix3D myAnswer1 = MatrixUtils.Multiply(m, myRotate);
            Matrix3D myAnswer2 = MatrixUtils.Multiply(myRotate, m);
            Matrix3D myAnswer3 = MatrixUtils.Multiply(m, myRotateAt);
            Matrix3D myAnswer4 = MatrixUtils.Multiply(myRotateAt, m);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer1) || failOnPurpose)
            {
                AddFailure("Rotate( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Axis   = ( {0}, {1}, {2} )", axis.X, axis.Y, axis.Z);
                Log("*** Angle  = " + angle);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1));
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer2) || failOnPurpose)
            {
                AddFailure("RotatePrepend( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Axis   = ( {0}, {1}, {2} )", axis.X, axis.Y, axis.Z);
                Log("*** Angle  = " + angle);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer2));
            }
            if (MathEx.NotCloseEnough(theirAnswer3, myAnswer3) || failOnPurpose)
            {
                AddFailure("RotateAt( Vector3D,Point3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Axis   = ( {0}, {1}, {2} )", axis.X, axis.Y, axis.Z);
                Log("*** Angle  = " + angle);
                Log("*** Center = ( {0}, {1}, {2} )", center.X, center.Y, center.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer3));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer3));
            }
            if (MathEx.NotCloseEnough(theirAnswer4, myAnswer4) || failOnPurpose)
            {
                AddFailure("RotateAtPrepend( Vector3D,Point3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Axis   = ( {0}, {1}, {2} )", axis.X, axis.Y, axis.Z);
                Log("*** Angle  = " + angle);
                Log("*** Center = ( {0}, {1}, {2} )", center.X, center.Y, center.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer4));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer4));
            }
        }

        private void TestScaleWith(Matrix3D m, Vector3D scale, Point3D center)
        {
            Matrix3D theirAnswer1 = m;
            Matrix3D theirAnswer2 = m;
            Matrix3D theirAnswer3 = m;
            Matrix3D theirAnswer4 = m;

            theirAnswer1.Scale(scale);
            theirAnswer2.ScalePrepend(scale);
            theirAnswer3.ScaleAt(scale, center);
            theirAnswer4.ScaleAtPrepend(scale, center);

            Matrix3D myScale = MatrixUtils.Scale(scale);
            Matrix3D myScaleAt = MatrixUtils.Scale(scale, center);
            Matrix3D myAnswer1 = MatrixUtils.Multiply(m, myScale);
            Matrix3D myAnswer2 = MatrixUtils.Multiply(myScale, m);
            Matrix3D myAnswer3 = MatrixUtils.Multiply(m, myScaleAt);
            Matrix3D myAnswer4 = MatrixUtils.Multiply(myScaleAt, m);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer1) || failOnPurpose)
            {
                AddFailure("Scale( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", scale.X, scale.Y, scale.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1));
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer2) || failOnPurpose)
            {
                AddFailure("ScalePrepend( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", scale.X, scale.Y, scale.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer2));
            }
            if (MathEx.NotCloseEnough(theirAnswer3, myAnswer3) || failOnPurpose)
            {
                AddFailure("ScaleAt( Vector3D,Point3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", scale.X, scale.Y, scale.Z);
                Log("*** Center = ( {0}, {1}, {2} )", center.X, center.Y, center.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer3));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer3));
            }
            if (MathEx.NotCloseEnough(theirAnswer4, myAnswer4) || failOnPurpose)
            {
                AddFailure("ScaleAtPrepend( Vector3D,Point3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", scale.X, scale.Y, scale.Z);
                Log("*** Center = ( {0}, {1}, {2} )", center.X, center.Y, center.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer4));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer4));
            }
        }

        private void TestTranslateWith(Matrix3D m, Vector3D translation)
        {
            Matrix3D theirAnswer1 = m;
            Matrix3D theirAnswer2 = m;

            theirAnswer1.Translate(translation);
            theirAnswer2.TranslatePrepend(translation);

            Matrix3D myTranslate = MatrixUtils.Translate(translation);
            Matrix3D myAnswer1 = MatrixUtils.Multiply(m, myTranslate);
            Matrix3D myAnswer2 = MatrixUtils.Multiply(myTranslate, m);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer1) || failOnPurpose)
            {
                AddFailure("Translate( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", translation.X, translation.Y, translation.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer1));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer1));
            }
            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer2) || failOnPurpose)
            {
                AddFailure("TranslatePrepend( Vector3D ) failed");
                Log("*** Matrix =\r\n" + MatrixUtils.ToStr(m));
                Log("*** Vector = ( {0}, {1}, {2} )", translation.X, translation.Y, translation.Z);
                Log("*** Expected =\r\n" + MatrixUtils.ToStr(myAnswer2));
                Log("*** Actual   =\r\n" + MatrixUtils.ToStr(theirAnswer2));
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("Identity");
            TestParseWith("0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2" + _sep + "0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2" + _sep + "0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2" + _sep + "0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + " 2");
            TestParseWith("1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89e+33" + _sep + "1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89e+33" + _sep + "1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89e+33" + _sep + "1. " + _sep + "	.0 " + _sep + " 90." + _sep + " 89e+33");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix3D m1 = Matrix3D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix3D m2 = StringConverter.ToMatrix3D(invariant);

            if (MathEx.NotEquals(m1, m2) || failOnPurpose)
            {
                AddFailure("Matrix3D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", m2);
                Log("*** Actual   = {0}", m1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Matrix3D.Identity);
            TestToStringWith(new Matrix3D());
            TestToStringWith(Const.mNAffine);
            TestToStringWith(Const.mAffine);
        }

        private void TestToStringWith(Matrix3D m)
        {
            string theirAnswer = m.ToString();

            // Don't want these to be affected by locale yet
            string myM11 = m.M11.ToString(CultureInfo.InvariantCulture);
            string myM12 = m.M12.ToString(CultureInfo.InvariantCulture);
            string myM13 = m.M13.ToString(CultureInfo.InvariantCulture);
            string myM14 = m.M14.ToString(CultureInfo.InvariantCulture);
            string myM21 = m.M21.ToString(CultureInfo.InvariantCulture);
            string myM22 = m.M22.ToString(CultureInfo.InvariantCulture);
            string myM23 = m.M23.ToString(CultureInfo.InvariantCulture);
            string myM24 = m.M24.ToString(CultureInfo.InvariantCulture);
            string myM31 = m.M31.ToString(CultureInfo.InvariantCulture);
            string myM32 = m.M32.ToString(CultureInfo.InvariantCulture);
            string myM33 = m.M33.ToString(CultureInfo.InvariantCulture);
            string myM34 = m.M34.ToString(CultureInfo.InvariantCulture);
            string myOffX = m.OffsetX.ToString(CultureInfo.InvariantCulture);
            string myOffY = m.OffsetY.ToString(CultureInfo.InvariantCulture);
            string myOffZ = m.OffsetZ.ToString(CultureInfo.InvariantCulture);
            string myM44 = m.M44.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myM11 + _sep + myM12 + _sep + myM13 + _sep + myM14 + _sep +
                                myM21 + _sep + myM22 + _sep + myM23 + _sep + myM24 + _sep +
                                myM31 + _sep + myM32 + _sep + myM33 + _sep + myM34 + _sep +
                                myOffX + _sep + myOffY + _sep + myOffZ + _sep + myM44;
            if (MatrixUtils.IsIdentity(m))
            {
                myAnswer = "Identity";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Matrix3D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = m.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Matrix3D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)m).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            if (MatrixUtils.IsIdentity(m))
            {
                myAnswer = "Identity";
            }
            else
            {
                // Don't want these to be affected by locale yet
                NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                myM11 = m.M11.ToString("N", numberFormat);
                myM12 = m.M12.ToString("N", numberFormat);
                myM13 = m.M13.ToString("N", numberFormat);
                myM14 = m.M14.ToString("N", numberFormat);
                myM21 = m.M21.ToString("N", numberFormat);
                myM22 = m.M22.ToString("N", numberFormat);
                myM23 = m.M23.ToString("N", numberFormat);
                myM24 = m.M24.ToString("N", numberFormat);
                myM31 = m.M31.ToString("N", numberFormat);
                myM32 = m.M32.ToString("N", numberFormat);
                myM33 = m.M33.ToString("N", numberFormat);
                myM34 = m.M34.ToString("N", numberFormat);
                myOffX = m.OffsetX.ToString("N", numberFormat);
                myOffY = m.OffsetY.ToString("N", numberFormat);
                myOffZ = m.OffsetZ.ToString("N", numberFormat);
                myM44 = m.M44.ToString("N", numberFormat);

                // ... Because of this
                myAnswer = myM11 + _sep + myM12 + _sep + myM13 + _sep + myM14 + _sep +
                            myM21 + _sep + myM22 + _sep + myM23 + _sep + myM24 + _sep +
                            myM31 + _sep + myM32 + _sep + myM33 + _sep + myM34 + _sep +
                            myOffX + _sep + myOffY + _sep + myOffZ + _sep + myM44;
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Matrix3D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.mAffine.GetHashCode();
            int hash2 = Const.mIdent.GetHashCode();
            int hash3 = Const.mNAffine.GetHashCode();
            int hash4 = Const.mNaN.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }

        private void RunTheTest2()
        {
            TestConstructors2();
            TestEquality2();
            TestIsIdentity2();
            TestProperties2();
            TestMultiply2();

            // This is broken and blocked by bugs.  Enable it later
            //TestTransforms2();

            TestParse2();
            TestToString2();
        }

        private void TestConstructors2()
        {
            Log("Testing Constructors with Bad Params...");

            if (MathEx.NotEquals(new Matrix3D(), Matrix3D.Identity) || failOnPurpose)
            {
                AddFailure("Default matrix is not identity");
            }

            TestConstructorWith(_nan, _inf, _min, _max, -_inf, _max, _eps, _min, _nan, _inf, _nan, _max, _min, _eps, _eps, 0.0);
        }

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.mNaN, Const.mNaN );
            TestEqualityWith(Const.mInf, Const.mNaN);
            TestEqualityWith(Const.mInf, Const.mInf);
            TestEqualityWith(Const.mNegInf, Const.mInf);
            TestEqualityWith(Const.mNegInf, Const.mNegInf);
        }

        private void TestIsIdentity2()
        {
            Log("Testing IsIdentity with Bad Params...");

            TestIdentityAfterChangingM("M11");
            TestIdentityAfterChangingM("M12");
            TestIdentityAfterChangingM("M13");
            TestIdentityAfterChangingM("M14");
            TestIdentityAfterChangingM("M21");
            TestIdentityAfterChangingM("M22");
            TestIdentityAfterChangingM("M23");
            TestIdentityAfterChangingM("M24");
            TestIdentityAfterChangingM("M31");
            TestIdentityAfterChangingM("M32");
            TestIdentityAfterChangingM("M33");
            TestIdentityAfterChangingM("M34");
            TestIdentityAfterChangingM("OffsetX");
            TestIdentityAfterChangingM("OffsetY");
            TestIdentityAfterChangingM("OffsetZ");
            TestIdentityAfterChangingM("M44");
        }

        private void TestIdentityAfterChangingM(string property)
        {
            Matrix3D m1 = new Matrix3D();
            Matrix3D m2 = Matrix3D.Identity;
            double value = (property == "M11" || property == "M22" || property == "M33" || property == "M44") ? 1.0 : 0.0;
            SetMWith(ref m1, value, property);
            SetMWith(ref m2, value, property);

            if (!m1.IsIdentity || failOnPurpose)
            {
                AddFailure("Setting property: " + property + " on new Matrix3D() breaks IsIdentity");
            }
            if (!m2.IsIdentity || failOnPurpose)
            {
                AddFailure("Setting property: " + property + " on Matrix3D.Identity breaks IsIdentity");
            }
        }

        private void TestProperties2()
        {
            Log("Testing M11-M44 with Bad Params...");

            TestMWith(_nan, "M11");
            TestMWith(_nan, "M12");
            TestMWith(_nan, "M13");
            TestMWith(_nan, "M14");
            TestMWith(_nan, "M21");
            TestMWith(_nan, "M22");
            TestMWith(_nan, "M23");
            TestMWith(_nan, "M24");
            TestMWith(_nan, "M31");
            TestMWith(_nan, "M32");
            TestMWith(_nan, "M33");
            TestMWith(_nan, "M34");
            TestMWith(_nan, "OffsetX");
            TestMWith(_nan, "OffsetY");
            TestMWith(_nan, "OffsetZ");
            TestMWith(_nan, "M44");
            TestMWith(_inf, "M11");
            TestMWith(_inf, "M12");
            TestMWith(_inf, "M13");
            TestMWith(_inf, "M14");
            TestMWith(_inf, "M21");
            TestMWith(_inf, "M22");
            TestMWith(_inf, "M23");
            TestMWith(_inf, "M24");
            TestMWith(_inf, "M31");
            TestMWith(_inf, "M32");
            TestMWith(_inf, "M33");
            TestMWith(_inf, "M34");
            TestMWith(_inf, "OffsetX");
            TestMWith(_inf, "OffsetY");
            TestMWith(_inf, "OffsetZ");
            TestMWith(_inf, "M44");
            TestMWith(-_inf, "M11");
            TestMWith(-_inf, "M12");
            TestMWith(-_inf, "M13");
            TestMWith(-_inf, "M14");
            TestMWith(-_inf, "M21");
            TestMWith(-_inf, "M22");
            TestMWith(-_inf, "M23");
            TestMWith(-_inf, "M24");
            TestMWith(-_inf, "M31");
            TestMWith(-_inf, "M32");
            TestMWith(-_inf, "M33");
            TestMWith(-_inf, "M34");
            TestMWith(-_inf, "OffsetX");
            TestMWith(-_inf, "OffsetY");
            TestMWith(-_inf, "OffsetZ");
            TestMWith(-_inf, "M44");
        }

        private void TestMultiply2()
        {
            Log("Testing Multiply,Append,Prepend with Bad Params...");

            TestMultiplyWith(Const.mNaN, Const.mNaN);
            TestMultiplyWith(Const.mInf, Const.mNaN);
            TestMultiplyWith(Const.mInf, Const.mInf);
            TestMultiplyWith(Const.mInf, Const.mNegInf);
            TestMultiplyWith(Const.mNegInf, Const.mNegInf);
        }

        private void TestTransforms2()
        {
            Log("Testing Rotate* with Bad Params...");

            TestRotateWith(Const.mIdent, Const.vNaN, 45, Const.p0);
            TestRotateWith(Const.mIdent, Const.vInf, 180, Const.p0);
            TestRotateWith(Const.mIdent, Const.vInf2, 90, Const.p0);
            TestRotateWith(Const.mIdent, Const.vNegInf, 135, Const.p0);
            TestRotateWith(Const.mIdent, Const.vMax, 90, Const.p0);
            TestRotateWith(Const.mIdent, Const.vMin, -90, Const.p0);
            TestRotateWith(Const.mIdent, Const.vEps, -135, Const.p0);

            TestRotateWith(Const.mIdent, Const.xAxis, _nan, Const.p0);
            TestRotateWith(Const.mIdent, Const.xAxis, _inf, Const.p0);
            TestRotateWith(Const.mIdent, Const.yAxis, -_inf, Const.p0);
            TestRotateWith(Const.mIdent, Const.zAxis, _max, Const.p0);
            TestRotateWith(Const.mIdent, Const.v1, _min, Const.p0);
            TestRotateWith(Const.mIdent, new Vector3D(3, 4, 5), _eps, new Point3D(3, 4, 5));

            TestRotateWith(Const.mIdent, Const.xAxis, 45, Const.pNaN);
            TestRotateWith(Const.mIdent, Const.xAxis, 180, Const.pInf);
            TestRotateWith(Const.mIdent, Const.yAxis, 90, Const.pInf2);
            TestRotateWith(Const.mIdent, Const.zAxis, 135, Const.pNegInf);
            TestRotateWith(Const.mIdent, Const.v1, 90, Const.pMax);
            TestRotateWith(Const.mIdent, new Vector3D(3, 4, 5), 36, Const.pMin);
            TestRotateWith(Const.mIdent, new Vector3D(-3, 4, 5), -54, Const.pEps);

            Log("Testing Scale* with Bad Params...");

            TestScaleWith(Const.mIdent, Const.vNaN, Const.p0);
            TestScaleWith(Const.mIdent, Const.vInf, Const.p0);
            TestScaleWith(Const.mIdent, Const.vInf2, Const.p0);
            TestScaleWith(Const.mIdent, Const.vNegInf, new Point3D(4, 5, -6));
            TestScaleWith(Const.mIdent, Const.vMax, new Point3D(-3, -4, 20));
            TestScaleWith(Const.mIdent, Const.vMin, new Point3D(-.99, .99, .49));
            TestScaleWith(Const.mIdent, Const.vEps, new Point3D(0, 25, -25));

            TestScaleWith(Const.mIdent, Const.v0, Const.pNaN);
            TestScaleWith(Const.mIdent, Const.v10, Const.pInf);
            TestScaleWith(Const.mIdent, Const.vNeg1, Const.pInf2);
            TestScaleWith(Const.mIdent, Const.v10, Const.pNegInf);
            TestScaleWith(Const.mIdent, new Vector3D(89, -23, 100), Const.pMax);
            TestScaleWith(Const.mIdent, new Vector3D(-.99, .99, .49), Const.pMin);
            TestScaleWith(Const.mIdent, new Vector3D(0, 25, -25), Const.pEps);

            Log("Testing Translate* with Bad Params...");

            TestTranslateWith(Const.mIdent, Const.vNaN);
            TestTranslateWith(Const.mIdent, Const.vInf);
            TestTranslateWith(Const.mIdent, Const.vInf2);
            TestTranslateWith(Const.mIdent, Const.vNegInf);
            TestTranslateWith(Const.mIdent, Const.vMax);
            TestTranslateWith(Const.mIdent, Const.vMin);
            TestTranslateWith(Const.mIdent, Const.vEps);
        }

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity" + _sep + "NaN" + _sep +
                           "-Infinity" + _sep + "NaN" + _sep + "NaN" + _sep + "Infinity" + _sep +
                           "Infinity" + _sep + "-Infinity" + _sep + "NaN" + _sep + "NaN" + _sep +
                           "NaN" + _sep + "NaN" + _sep + "Infinity" + _sep + "-Infinity");
        }

        private void TestToString2()
        {
            TestToStringWith(Const.mNaN);
            TestToStringWith(Const.mInf);
            TestToStringWith(Const.mNegInf);
        }
    }
}