// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;


namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class MatrixTest : CoreGraphicsTest
    {
        private string _sep = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTest2();
            }
            else
            {
                TestAppend();
                TestConstructor();
                TestDeterminant();
                TestEquals();
                TestGetHashCode();
                TestHasInverse();
                TestIdentity();
                TestInvert();
                TestIsIdentity();
                TestMultiply();
                TestOpEqual();
                TestOpInequality();
                TestOpMultiply();
                TestParse();
                TestPrepend();
                TestProperties();
                TestRotate();
                TestRotateAt();
                TestRotateAtPrepend();
                TestRotatePrepend();
                TestScale();
                TestScaleAt();
                TestScaleAtPrepend();
                TestScalePrepend();
                TestSetIdentity();
                TestSkew();
                TestSkewPrepend();
                TestToString();
                TestTransformPoint();
                TestTransformPoints();
                TestTransformVector();
                TestTransformVectors();
                TestTranslate();
                TestTranslatePrepend();
            }
        }

        private void TestAppend()
        {
            Log("Testing Append( Matrix )...");

            TestAppendWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestAppendWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.typeIdentity);
            TestAppendWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestAppendWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestAppendWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestAppendWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestAppendWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestAppendWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(-33.33, 0, 0, -0.44, 0, 0));
            TestAppendWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestAppendWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestAppendWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestAppendWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestAppendWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestAppendWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestAppendWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestAppendWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1.11, -2.22, 3.33, -4.44, 5.55, -6.66));
        }

        private void TestAppendWith(Matrix matrix1, Matrix matrix2)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Append(matrix2);

            Matrix myAnswer = MatrixUtils.Multiply(matrix1, matrix2);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Append( Matrix ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor Matrix()...");

            TestConstructorWith();

            Log("Testing Constructor Matrix( double, double, double, double, double, double)...");

            TestConstructorWith(0, 0, 0, 0, 0, 0);
            TestConstructorWith(1, 0, 0, 1, 0, 0);
            TestConstructorWith(11.11, 11.11, 11.11, 11.11, 11.11, 11.11);
            TestConstructorWith(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11);
        }

        private void TestConstructorWith()
        {
            Matrix theirAnswer = new Matrix();

            if (!MathEx.Equals(theirAnswer.M11, 1) || !MathEx.Equals(theirAnswer.M12, 0) ||
                 !MathEx.Equals(theirAnswer.M21, 0) || !MathEx.Equals(theirAnswer.M22, 1) ||
                 !MathEx.Equals(theirAnswer.OffsetX, 0) || !MathEx.Equals(theirAnswer.OffsetY, 0) || failOnPurpose)
            {
                AddFailure("Constructor Matrix() failed");
                Log("*** Expected: M11 = {0}, M12 = {1}, M21 = {2}, M22 = {3}, OffsetX = {4}, OffsetY = {5}", 1, 0, 0, 1, 0, 0);
                Log("*** Actual:   M11 = {0}, M12 = {1}, M21 = {2}, M22 = {3}, OffsetX = {4}, OffsetY = {5}", theirAnswer.M11, theirAnswer.M12, theirAnswer.M21, theirAnswer.M22, theirAnswer.OffsetX, theirAnswer.OffsetY);
            }
        }

        private void TestConstructorWith(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            Matrix theirAnswer = new Matrix(m11, m12, m21, m22, offsetX, offsetY);

            if (!MathEx.Equals(theirAnswer.M11, m11) || !MathEx.Equals(theirAnswer.M12, m12) ||
                 !MathEx.Equals(theirAnswer.M21, m21) || !MathEx.Equals(theirAnswer.M22, m22) ||
                 !MathEx.Equals(theirAnswer.OffsetX, offsetX) || !MathEx.Equals(theirAnswer.OffsetY, offsetY) || failOnPurpose)
            {
                AddFailure("Constructor Matrix( double, double, double, double, double, double ) failed");
                Log("*** Expected: M11 = {0}, M12 = {1}, M21 = {2}, M22 = {3}, OffsetX = {4}, OffsetY = {5}", m11, m12, m21, m22, offsetX, offsetY);
                Log("*** Actual:   M11 = {0}, M12 = {1}, M21 = {2}, M22 = {3}, OffsetX = {4}, OffsetY = {5}", theirAnswer.M11, theirAnswer.M12, theirAnswer.M21, theirAnswer.M22, theirAnswer.OffsetX, theirAnswer.OffsetY);
            }
        }

        private void TestDeterminant()
        {
            Log("Testing Determinant...");

            TestDeterminantWith(Const2D.typeIdentity);
            TestDeterminantWith(new Matrix(0, 0, 0, 0, 0, 0));
            TestDeterminantWith(new Matrix(1, 0, 0, 1, 11.11, -0.22));
            TestDeterminantWith(new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestDeterminantWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestDeterminantWith(new Matrix(11.11, -11.11, 0.22, -0.22, 11.11, 0.22));
        }

        private void TestDeterminantWith(Matrix matrix1)
        {
            double theirAnswer = matrix1.Determinant;

            double myAnswer = MatrixUtils.Determinant(matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Determinant failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual:   {0}", theirAnswer);
            }
        }

        private void TestEquals()
        {
            Log("Testing Matrix.Equals( Matrix, Matrix)...");

            TestEqualsWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestEqualsWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestEqualsWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestEqualsWith(new Matrix(-11.11, 22.22, -33.33, 44.44, -55.55, 66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));


            Log("Testing Equals( object )...");

            TestEqualsWith(new Matrix(), false);
            TestEqualsWith(new Matrix(), null);

            object obj1 = new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66);
            TestEqualsWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), obj1);
        }

        private void TestEqualsWith(Matrix matrix1, Matrix matrix2)
        {
            bool theirAnswer = Matrix.Equals(matrix1, matrix2);

            bool myAnswer = MathEx.Equals(matrix1, matrix2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Matrix.Equals( Matrix, Matrix ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestEqualsWith(Matrix matrix1, object o)
        {
            bool theirAnswer = matrix1.Equals(o);

            bool myAnswer = true;
            if (o == null || !(o is Matrix))
            {
                myAnswer = false;
            }
            else
            {
                Matrix matrix2 = (Matrix)o;
                myAnswer = MathEx.Equals(matrix1, matrix2);
            }
            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Equals( object ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode()...");

            TestGetHashCodeWith(Const2D.typeIdentity);
            TestGetHashCodeWith(new Matrix(0, 0, 0, 0, 0, 0));
            TestGetHashCodeWith(new Matrix(1.1, -0.2, 3.3, -0.4, 5.5, -0.6));
        }

        private void TestGetHashCodeWith(Matrix matrix1)
        {
            int theirAnswer = matrix1.GetHashCode();

            int myAnswer = MatrixUtils.GetHashCode(matrix1);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("GetHashCode() failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestHasInverse()
        {
            Log("Testing HasInverse...");

            TestHasInverseWith(Const2D.typeIdentity);
            TestHasInverseWith(new Matrix(0, 0, 0, 0, 0, 0));
            TestHasInverseWith(new Matrix(-11.11, -0.22, 0.22, 11.11, 0, 0));
            TestHasInverseWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestHasInverseWith(Matrix matrix1)
        {
            bool theirAnswer = matrix1.HasInverse;

            bool myAnswer = (MatrixUtils.Determinant(matrix1) == 0) ? false : true;

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("HasInverse failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestIdentity()
        {
            Log("Testing Matrix.Identity...");

            Matrix theirAnswer = Matrix.Identity;
            Matrix myAnswer = new Matrix(1, 0, 0, 1, 0, 0);

            if (!MatrixUtils.IsIdentity(theirAnswer) || failOnPurpose)
            {
                AddFailure("Matrix.Identity failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(new Matrix(1, 0, 0, 1, 0, 0)));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestInvert()
        {
            Log("Testing Invert()...");

            TestInvertWith(Const2D.typeIdentity);
            TestInvertWith(new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestInvertWith(new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestInvertWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestInvertWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestInvertWith(Matrix matrix1)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Invert();

            Matrix identity = MatrixUtils.Multiply(matrix1, theirAnswer);

            if (MathEx.NotCloseEnough(identity, Matrix.Identity) || failOnPurpose)
            {
                AddFailure("Invert failed");
            }
        }

        private void TestIsIdentity()
        {
            Log("Testing IsIdentity...");

            TestIsIdentityWith(new Matrix());
            TestIsIdentityWith(Matrix.Identity);
            TestIsIdentityWith(Const2D.typeIdentity);
            TestIsIdentityWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));

            // The values are identity but the type is not
            TestIsIdentityWith(MatrixUtils.Multiply(new Matrix(2, 0, 0, 2, 0, 0), new Matrix(0.5, 0, 0, 0.5, 0, 0)));
        }

        private void TestIsIdentityWith(Matrix matrix1)
        {
            bool theirAnswer = matrix1.IsIdentity;

            bool myAnswer = MatrixUtils.IsIdentity(matrix1);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("IsIdentity failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Matrix.Multiply( Matrix, Matrix )...");

            TestMultiplyWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.typeIdentity);
            TestMultiplyWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestMultiplyWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestMultiplyWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(-33.33, 0, 0, -0.44, 0, 0));
            TestMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestMultiplyWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1.11, -2.22, 3.33, -4.44, 5.55, -6.66));
        }

        private void TestMultiplyWith(Matrix matrix1, Matrix matrix2)
        {
            Matrix theirAnswer = Matrix.Multiply(matrix1, matrix2);

            Matrix myAnswer = MatrixUtils.Multiply(matrix1, matrix2);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Matrix.Multiply( Matrix, Matrix ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestOpEqual()
        {
            Log("Testing == Operator( Matrix, Matrix )...");

            TestOpEqualWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestOpEqualWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpEqualWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpEqualWith(new Matrix(-11.11, 22.22, -33.33, 44.44, -55.55, 66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestOpEqualWith(Matrix matrix1, Matrix matrix2)
        {
            bool theirAnswer = (matrix1 == matrix2);

            bool myAnswer = MathEx.Equals(matrix1, matrix2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("== Operator( Matrix, Matrix ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpInequality()
        {
            Log("Testing != Operator( Matrix, Matrix )...");

            TestOpInequalityWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestOpInequalityWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpInequalityWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpInequalityWith(new Matrix(-11.11, 22.22, -33.33, 44.44, -55.55, 66.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestOpInequalityWith(Matrix matrix1, Matrix matrix2)
        {
            bool theirAnswer = (matrix1 != matrix2);

            bool myAnswer = !MathEx.Equals(matrix1, matrix2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("!= Operator( Matrix, Matrix ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiply()
        {
            Log("Testing * Operator( Matrix, Matrix )...");

            TestOpMultiplyWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestOpMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.typeIdentity);
            TestOpMultiplyWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestOpMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestOpMultiplyWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestOpMultiplyWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(-33.33, 0, 0, -0.44, 0, 0));
            TestOpMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestOpMultiplyWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestOpMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1.11, -2.22, 3.33, -4.44, 5.55, -6.66));
        }

        private void TestOpMultiplyWith(Matrix matrix1, Matrix matrix2)
        {
            Matrix theirAnswer = matrix1 * matrix2;

            Matrix myAnswer = MatrixUtils.Multiply(matrix1, matrix2);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( Matrix, Matrix ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestParse()
        {
            Log("Testing Matrix.Parse...");

            TestParseWith("Identity");
            TestParseWith("1 " + _sep + " 0	" + _sep + " 0" + _sep + " 1" + _sep + "0.0 " + _sep + " 0.0	");
            TestParseWith("0 " + _sep + " 0	" + _sep + " 0" + _sep + " 0" + _sep + "0.0 " + _sep + " 0.0	");
            TestParseWith("11.11 " + _sep + " 22.22	" + _sep + " 33.33" + _sep + " 44.44" + _sep + "55.55 " + _sep + " 66.66	");
            TestParseWith("-11.11 " + _sep + " -22.22	" + _sep + " -33.33" + _sep + " -44.44" + _sep + "-55.55 " + _sep + " -66.66	");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix m1 = Matrix.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix m2 = StringConverter.ToMatrix(invariant);

            if (!MathEx.Equals(m1, m2) || failOnPurpose)
            {
                AddFailure("Matrix.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", m2);
                Log("*** Actual   = {0}", m1);
            }
        }

        private void TestPrepend()
        {
            Log("Testing Prepend( Matrix )...");

            TestPrependWith(Const2D.typeIdentity, Const2D.typeIdentity);
            TestPrependWith(Const2D.typeIdentity, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.typeIdentity);
            TestPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestPrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(1, 0, 0, 1, -11.11, 22.22));
            TestPrependWith(new Matrix(-33.33, 0, 0, -0.44, 0, 0), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestPrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66));
            TestPrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -0.66), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestPrependWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestPrependWith(new Matrix(1.11, -2.22, 3.33, -4.44, 5.55, -6.66), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestPrependWith(Matrix matrix1, Matrix matrix2)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Prepend(matrix2);

            Matrix myAnswer = MatrixUtils.Multiply(matrix2, matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Prepend( Matrix ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties M11, M12, M21, M22, OffsetX, OffsetY...");

            TestMWith(0.0, "M11");
            TestMWith(1.1, "M11");
            TestMWith(-1.1, "M11");
            TestMWith(0.0, "M12");
            TestMWith(1.1, "M12");
            TestMWith(-1.1, "M12");
            TestMWith(0.0, "M21");
            TestMWith(1.1, "M21");
            TestMWith(-1.1, "M21");
            TestMWith(0.0, "M22");
            TestMWith(1.1, "M22");
            TestMWith(-1.1, "M22");
            TestMWith(0.0, "OffsetX");
            TestMWith(1.1, "OffsetX");
            TestMWith(-1.1, "OffsetX");
            TestMWith(0.0, "OffsetY");
            TestMWith(1.1, "OffsetY");
            TestMWith(-1.1, "OffsetY");
        }

        private void TestMWith(double value, string property)
        {
            // get and set on Identity Matrix
            Matrix m = Const2D.typeIdentity;
            double actual = SetMWith(ref m, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            // get and set on unknown type Matrix
            m = new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66);
            actual = SetMWith(ref m, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            // get and set on Scale | Translate  type Matrix
            m = new Matrix(11.11, 0, 0, -0.44, 55.55, -66.66);
            actual = SetMWith(ref m, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private double SetMWith(ref Matrix m, double value, string property)
        {
            switch (property)
            {
                case "M11": m.M11 = value; return m.M11;
                case "M12": m.M12 = value; return m.M12;
                case "M21": m.M21 = value; return m.M21;
                case "M22": m.M22 = value; return m.M22;
                case "OffsetX": m.OffsetX = value; return m.OffsetX;
                case "OffsetY": m.OffsetY = value; return m.OffsetY;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Matrix");
        }

        private void TestRotate()
        {
            Log("Testing Rotate( double )...");

            TestRotateWith(Const2D.typeIdentity, 0);
            TestRotateWith(Const2D.typeIdentity, 444.44);
            TestRotateWith(Const2D.typeIdentity, -444.44);
            TestRotateWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0);
            TestRotateWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 444.44);
            TestRotateWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -444.44);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 444.44);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -444.44);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 444.44);
            TestRotateWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -444.44);
            TestRotateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0);
            TestRotateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44);
            TestRotateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -444.44);
        }

        private void TestRotateWith(Matrix matrix1, double angle)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Rotate(angle);

            Matrix tmp = MatrixUtils.Rotate(angle);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Rotate( double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestRotateAt()
        {
            Log("Testing RotateAt( double, Point )...");

            TestRotateAtWith(Const2D.typeIdentity, 0, new Point(33.33, -44.44));
            TestRotateAtWith(Const2D.typeIdentity, 444.44, new Point(33.33, -44.44));
            TestRotateAtWith(Const2D.typeIdentity, -444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -444.44, new Point(33.33, -44.44));
            TestRotateAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44, new Point());
        }

        private void TestRotateAtWith(Matrix matrix1, double angle, Point center)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.RotateAt(angle, center.X, center.Y);

            Matrix tmp = MatrixUtils.Rotate(angle, center);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("RotateAt( double, Point ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestRotateAtPrepend()
        {
            Log("Testing RotateAtPrepend( double, Point )...");

            TestRotateAtPrependWith(Const2D.typeIdentity, 0, new Point(33.33, -44.44));
            TestRotateAtPrependWith(Const2D.typeIdentity, 444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(Const2D.typeIdentity, -444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -444.44, new Point(33.33, -44.44));
            TestRotateAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44, new Point());
        }

        private void TestRotateAtPrependWith(Matrix matrix1, double angle, Point center)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.RotateAtPrepend(angle, center.X, center.Y);

            Matrix tmp = MatrixUtils.Rotate(angle, center);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("RotateAtPrePend( double, Point ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestRotatePrepend()
        {
            Log("Testing RotatePrepend( double )...");

            TestRotatePrependWith(Const2D.typeIdentity, 0);
            TestRotatePrependWith(Const2D.typeIdentity, 444.44);
            TestRotatePrependWith(Const2D.typeIdentity, -444.44);
            TestRotatePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0);
            TestRotatePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 444.44);
            TestRotatePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -444.44);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 444.44);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -444.44);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 444.44);
            TestRotatePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -444.44);
            TestRotatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0);
            TestRotatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 444.44);
            TestRotatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -444.44);
        }

        private void TestRotatePrependWith(Matrix matrix1, double angle)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.RotatePrepend(angle);

            Matrix tmp = MatrixUtils.Rotate(angle);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("RotatePrepend( double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestScale()
        {
            Log("Testing Scale( double, double )...");

            TestScaleWith(Const2D.typeIdentity, 0, 0);
            TestScaleWith(Const2D.typeIdentity, 1, 1);
            TestScaleWith(Const2D.typeIdentity, 11.11, 22.22);
            TestScaleWith(Const2D.typeIdentity, -0.11, -0.22);
            TestScaleWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0);
            TestScaleWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 1, 1);
            TestScaleWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22);
            TestScaleWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 1, 1);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 1, 1);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22);
            TestScaleWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22);
            TestScaleWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestScaleWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 1, 1);
            TestScaleWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestScaleWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22);
        }

        private void TestScaleWith(Matrix matrix1, double scaleX, double scaleY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Scale(scaleX, scaleY);

            Matrix tmp = MatrixUtils.Scale(scaleX, scaleY);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Scale( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestScaleAt()
        {
            Log("Testing ScaleAt( double, double, Point )...");

            TestScaleAtWith(Const2D.typeIdentity, 0, 0, new Point(33.33, -44.44));
            TestScaleAtWith(Const2D.typeIdentity, 1, 1, new Point(33.33, -44.44));
            TestScaleAtWith(Const2D.typeIdentity, 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtWith(Const2D.typeIdentity, -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 1, 1, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 1, 1, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 1, 1, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 1, 1, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22, new Point());
        }

        private void TestScaleAtWith(Matrix matrix1, double scaleX, double scaleY, Point center)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.ScaleAt(scaleX, scaleY, center.X, center.Y);

            Matrix tmp = MatrixUtils.Scale(scaleX, scaleY, center);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("ScaleAt( double, double, Point ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestScaleAtPrepend()
        {
            Log("Testing ScaleAtPrepend( double, double, Point )...");

            TestScaleAtPrependWith(Const2D.typeIdentity, 0, 0, new Point(33.33, -44.44));
            TestScaleAtPrependWith(Const2D.typeIdentity, 1, 1, new Point(33.33, -44.44));
            TestScaleAtPrependWith(Const2D.typeIdentity, 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(Const2D.typeIdentity, -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 1, 1, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 1, 1, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 1, 1, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 1, 1, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22, new Point(33.33, -44.44));
            TestScaleAtPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22, new Point());
        }

        private void TestScaleAtPrependWith(Matrix matrix1, double scaleX, double scaleY, Point center)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.ScaleAtPrepend(scaleX, scaleY, center.X, center.Y);

            Matrix tmp = MatrixUtils.Scale(scaleX, scaleY, center);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("ScaleAtPrepend( double, double, Point ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestScalePrepend()
        {
            Log("Testing ScalePrepend( double, double )...");

            TestScalePrependWith(Const2D.typeIdentity, 0, 0);
            TestScalePrependWith(Const2D.typeIdentity, 1, 1);
            TestScalePrependWith(Const2D.typeIdentity, 11.11, 22.22);
            TestScalePrependWith(Const2D.typeIdentity, -0.11, -0.22);
            TestScalePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0);
            TestScalePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 1, 1);
            TestScalePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22);
            TestScalePrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 1, 1);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 1, 1);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22);
            TestScalePrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22);
            TestScalePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestScalePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 1, 1);
            TestScalePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestScalePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22);
        }

        private void TestScalePrependWith(Matrix matrix1, double scaleX, double scaleY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.ScalePrepend(scaleX, scaleY);

            Matrix tmp = MatrixUtils.Scale(scaleX, scaleY);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("ScalePrepend( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestSetIdentity()
        {
            Log("Testing SetIdentity()...");

            TestSetIdentityWith(Matrix.Identity);
            TestSetIdentityWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestSetIdentityWith(Matrix matrix1)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.SetIdentity();

            if (!MatrixUtils.IsIdentity(theirAnswer) || failOnPurpose)
            {
                AddFailure("SetIdentity() failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(new Matrix()));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestSkew()
        {
            Log("Testing Skew( double, double )...");

            TestSkewWith(Const2D.typeIdentity, 0, 0);
            TestSkewWith(Const2D.typeIdentity, 11.11, 22.22);
            TestSkewWith(Const2D.typeIdentity, -0.11, -0.22);
            TestSkewWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0);
            TestSkewWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22);
            TestSkewWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22);
            TestSkewWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22);
            TestSkewWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestSkewWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestSkewWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22);
            TestSkewWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 90, 90);
        }

        private void TestSkewWith(Matrix matrix1, double skewX, double skewY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Skew(skewX, skewY);

            Matrix tmp = MatrixUtils.Skew(skewX, skewY);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Skew( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestSkewPrepend()
        {
            Log("Testing SkewPrepend( double, double )...");

            TestSkewPrependWith(Const2D.typeIdentity, 0, 0);
            TestSkewPrependWith(Const2D.typeIdentity, 11.11, 22.22);
            TestSkewPrependWith(Const2D.typeIdentity, -0.11, -0.22);
            TestSkewPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 0, 0);
            TestSkewPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), 11.11, 22.22);
            TestSkewPrependWith(new Matrix(1, 0, 0, 1, -11.11, 22.22), -0.11, -0.22);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 0, 0);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), 11.11, 22.22);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), -0.11, -0.22);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 0, 0);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), 11.11, 22.22);
            TestSkewPrependWith(new Matrix(11.11, 0, 0, -0.22, -11.11, 22.22), -0.11, -0.22);
            TestSkewPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestSkewPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestSkewPrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -0.11, -0.22);
        }

        private void TestSkewPrependWith(Matrix matrix1, double skewX, double skewY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.SkewPrepend(skewX, skewY);

            Matrix tmp = MatrixUtils.Skew(skewX, skewY);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("SkewPrepend( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const2D.typeIdentity);
            TestToStringWith(new Matrix(0, 0, 0, 0, 0, 0));
            TestToStringWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestToStringWith(Matrix matrix1)
        {
            string theirAnswer = matrix1.ToString();

            // Don't want these to be affected by locale yet
            string myM11 = matrix1.M11.ToString(CultureInfo.InvariantCulture);
            string myM12 = matrix1.M12.ToString(CultureInfo.InvariantCulture);
            string myM21 = matrix1.M21.ToString(CultureInfo.InvariantCulture);
            string myM22 = matrix1.M22.ToString(CultureInfo.InvariantCulture);
            string myOffX = matrix1.OffsetX.ToString(CultureInfo.InvariantCulture);
            string myOffY = matrix1.OffsetY.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myM11 + _sep + myM12 + _sep +
                                myM21 + _sep + myM22 + _sep +
                                myOffX + _sep + myOffY;
            if (MatrixUtils.IsIdentity(matrix1))
            {
                myAnswer = "Identity";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = matrix1.ToString(CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)matrix1).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            if (MatrixUtils.IsIdentity(matrix1))
            {
                myAnswer = "Identity";
            }
            else
            {
                // Don't want these to be affected by locale yet
                NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
                myM11 = matrix1.M11.ToString("N", numberFormat);
                myM12 = matrix1.M12.ToString("N", numberFormat);
                myM21 = matrix1.M21.ToString("N", numberFormat);
                myM22 = matrix1.M22.ToString("N", numberFormat);
                myOffX = matrix1.OffsetX.ToString("N", numberFormat);
                myOffY = matrix1.OffsetY.ToString("N", numberFormat);

                // ... Because of this
                myAnswer = myM11 + _sep + myM12 + _sep +
                            myM21 + _sep + myM22 + _sep +
                            myOffX + _sep + myOffY;
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestTransformPoint()
        {
            Log("Testing Transform( Point )...");

            TestTransformPointWith(Const2D.typeIdentity, new Point());
            TestTransformPointWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), new Point());
            TestTransformPointWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Point());
            TestTransformPointWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Point());
            TestTransformPointWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Point());
            TestTransformPointWith(Const2D.typeIdentity, new Point(-1.1, 1.1));
            TestTransformPointWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), new Point(-1.1, 1.1));
            TestTransformPointWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Point(-1.1, 1.1));
            TestTransformPointWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Point(-1.1, 1.1));
            TestTransformPointWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Point(-1.1, 1.1));
        }

        private void TestTransformPointWith(Matrix matrix1, Point point1)
        {
            Point theirAnswer = matrix1.Transform(point1);

            Point myAnswer = MatrixUtils.Transform(point1, matrix1);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Transform( Point ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestTransformPoints()
        {
            Log("Testing Transform( Point[] )...");

            TestTransformPointsWith(Const2D.typeIdentity, Const2D.points);
            TestTransformPointsWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), Const2D.points);
            TestTransformPointsWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), Const2D.points);
            TestTransformPointsWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), Const2D.points);
            TestTransformPointsWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.points);
        }

        private void TestTransformPointsWith(Matrix matrix1, Point[] point1)
        {
            Point[] myAnswer;
            Point[] theirAnswer;
            if (point1 == null)
            {
                theirAnswer = point1;
            }
            else
            {
                theirAnswer = new Point[point1.Length];
                for (int i = 0; i < point1.Length; i++)
                {
                    theirAnswer[i] = point1[i];
                }
            }

            myAnswer = MatrixUtils.Transform(point1, matrix1);

            matrix1.Transform(theirAnswer);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Transform( Point[] ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestTransformVector()
        {
            Log("Testing Transform( Vector )...");

            TestTransformVectorWith(Const2D.typeIdentity, new Vector());
            TestTransformVectorWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), new Vector());
            TestTransformVectorWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Vector());
            TestTransformVectorWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Vector());
            TestTransformVectorWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Vector());
            TestTransformVectorWith(Const2D.typeIdentity, new Vector(-1.1, 1.1));
            TestTransformVectorWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), new Vector(-1.1, 1.1));
            TestTransformVectorWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), new Vector(-1.1, 1.1));
            TestTransformVectorWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), new Vector(-1.1, 1.1));
            TestTransformVectorWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), new Vector(-1.1, 1.1));
        }

        private void TestTransformVectorWith(Matrix matrix1, Vector vector1)
        {
            Vector theirAnswer = matrix1.Transform(vector1);

            Vector myAnswer = MatrixUtils.Transform(vector1, matrix1);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Transform( Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestTransformVectors()
        {
            Log("Testing Transform( Vector[] )...");

            TestTransformVectorsWith(Const2D.typeIdentity, Const2D.vectors);
            TestTransformVectorsWith(new Matrix(1, 0, 0, 1, 11.11, -22.22), Const2D.vectors);
            TestTransformVectorsWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), Const2D.vectors);
            TestTransformVectorsWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), Const2D.vectors);
            TestTransformVectorsWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), Const2D.vectors);
        }

        private void TestTransformVectorsWith(Matrix matrix1, Vector[] vector1)
        {
            Vector[] myAnswer;
            Vector[] theirAnswer;
            if (vector1 == null)
            {
                theirAnswer = vector1;
            }
            else
            {
                theirAnswer = new Vector[vector1.Length];
                for (int i = 0; i < vector1.Length; i++)
                {
                    theirAnswer[i] = vector1[i];
                }
            }

            myAnswer = MatrixUtils.Transform(vector1, matrix1);

            matrix1.Transform(theirAnswer);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Transform( Vector[] ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestTranslate()
        {
            Log("Testing Translate( double, double )...");

            TestTranslateWith(Const2D.typeIdentity, 0, 0);
            TestTranslateWith(Const2D.typeIdentity, 11.11, 22.22);
            TestTranslateWith(Const2D.typeIdentity, -11.11, -22.22);
            TestTranslateWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), 0, 0);
            TestTranslateWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), 11.11, 22.22);
            TestTranslateWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), -11.11, -22.22);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), 0, 0);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), 11.11, 22.22);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), -11.11, -22.22);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), 0, 0);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), 11.11, 22.22);
            TestTranslateWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), -11.11, -22.22);
            TestTranslateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestTranslateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestTranslateWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -11.11, -22.22);
        }

        private void TestTranslateWith(Matrix matrix1, double offsetX, double offsetY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.Translate(offsetX, offsetY);

            Matrix tmp = MatrixUtils.Translate(offsetX, offsetY);
            Matrix myAnswer = MatrixUtils.Multiply(matrix1, tmp);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Translate( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void TestTranslatePrepend()
        {
            Log("Testing TranslatePrepend( double, double )...");

            TestTranslatePrependWith(Const2D.typeIdentity, 0, 0);
            TestTranslatePrependWith(Const2D.typeIdentity, 11.11, 22.22);
            TestTranslatePrependWith(Const2D.typeIdentity, -11.11, -22.22);
            TestTranslatePrependWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), 0, 0);
            TestTranslatePrependWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), 11.11, 22.22);
            TestTranslatePrependWith(new Matrix(1, 0, 0, 1, 33.33, -44.44), -11.11, -22.22);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), 0, 0);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), 11.11, 22.22);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 0, 0), -11.11, -22.22);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), 0, 0);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), 11.11, 22.22);
            TestTranslatePrependWith(new Matrix(33.33, 0, 0, -0.44, 55.55, -66.66), -11.11, -22.22);
            TestTranslatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 0, 0);
            TestTranslatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), 11.11, 22.22);
            TestTranslatePrependWith(new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66), -11.11, -22.22);
        }

        private void TestTranslatePrependWith(Matrix matrix1, double offsetX, double offsetY)
        {
            Matrix theirAnswer = matrix1;
            theirAnswer.TranslatePrepend(offsetX, offsetY);

            Matrix tmp = MatrixUtils.Translate(offsetX, offsetY);
            Matrix myAnswer = MatrixUtils.Multiply(tmp, matrix1);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("TranslatePrepend( double, double ) failed");
                Log("*** Expected: \n{0}", MatrixUtils.ToStr(myAnswer));
                Log("*** Actual:   \n{0}", MatrixUtils.ToStr(theirAnswer));
            }
        }

        private void RunTest2()
        {
            // these are P2 tests, overflow, bignumber, exception
            TestConstructor2();
            TestInvert2();
            TestParse2();
            TestProperties2();
            TestToString2();
            TestTransformPoints2();
            TestTransformVectors2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor( double, double, double, double, double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan);
        }

        private void TestInvert2()
        {
            Log("P2 Testing Invert()...");

            Try(InvertZeroDeterminantMatrix, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for Invert

        private void InvertZeroDeterminantMatrix()
        {
            Matrix matrix = new Matrix(11.11, 22.22, -11.11, -22.22, 11.11, -22.22);
            matrix.Invert();
        }

        #endregion

        private void TestParse2()
        {
            Log("P2 Testing Matrix.Parse...");

            Try(ParseEmptyString, typeof(InvalidOperationException));
            Try(ParseTooFewParams, typeof(InvalidOperationException));
            Try(ParseTooManyParams1, typeof(InvalidOperationException));
            Try(ParseTooManyParams2, typeof(InvalidOperationException));
            Try(ParseWrongFormat1, typeof(FormatException));
            Try(ParseWrongFormat2, typeof(FormatException));
        }

        #region ExceptionThrowers for Parse

        private void ParseEmptyString()
        {
            string s = "";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        private void ParseTooFewParams()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "33.33" + _sep + "-44.44" + _sep + "55.55";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        private void ParseTooManyParams1()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "33.33" + _sep + "-44.44" + _sep + "55.55" + _sep + "-66.66" + _sep + "77.77";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        private void ParseTooManyParams2()
        {
            string s = "Identity" + _sep + "-22.22" + _sep + "33.33" + _sep + "-44.44" + _sep + "55.55" + _sep + "-66.66";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        private void ParseWrongFormat1()
        {
            string s = "11.11" + _sep + "Identity" + _sep + "33.33" + _sep + "-44.44" + _sep + "55.55" + _sep + "-66.66";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        private void ParseWrongFormat2()
        {
            string s = "11.11" + _sep + "a" + _sep + "33.33" + _sep + "-44.44" + _sep + "55.55" + _sep + "-66.66";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Matrix matrix = Matrix.Parse(global);
        }

        #endregion

        private void TestProperties2()
        {
            Log("P2 Testing Properties M11, M12, M21, M22, OffsetX, OffsetY...");

            TestMWith(Const2D.min, "M11");
            TestMWith(Const2D.max, "M11");
            TestMWith(Const2D.posInf, "M11");
            TestMWith(Const2D.negInf, "M11");
            TestMWith(Const2D.nan, "M11");
            TestMWith(Const2D.min, "M12");
            TestMWith(Const2D.max, "M12");
            TestMWith(Const2D.posInf, "M12");
            TestMWith(Const2D.negInf, "M12");
            TestMWith(Const2D.nan, "M12");
            TestMWith(Const2D.min, "M21");
            TestMWith(Const2D.max, "M21");
            TestMWith(Const2D.posInf, "M21");
            TestMWith(Const2D.negInf, "M21");
            TestMWith(Const2D.nan, "M21");
            TestMWith(Const2D.min, "M22");
            TestMWith(Const2D.max, "M22");
            TestMWith(Const2D.posInf, "M22");
            TestMWith(Const2D.negInf, "M22");
            TestMWith(Const2D.nan, "M22");
            TestMWith(Const2D.min, "OffsetX");
            TestMWith(Const2D.max, "OffsetX");
            TestMWith(Const2D.posInf, "OffsetX");
            TestMWith(Const2D.negInf, "OffsetX");
            TestMWith(Const2D.nan, "OffsetX");
            TestMWith(Const2D.min, "OffsetY");
            TestMWith(Const2D.max, "OffsetY");
            TestMWith(Const2D.posInf, "OffsetY");
            TestMWith(Const2D.negInf, "OffsetY");
            TestMWith(Const2D.nan, "OffsetY");
        }

        private void TestToString2()
        {
            Log("P2 Testing ToString...");

            TestToStringWith(new Matrix(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min));
            TestToStringWith(new Matrix(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max));
            TestToStringWith(new Matrix(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf));
            TestToStringWith(new Matrix(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestToStringWith(new Matrix(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
        }

        private void TestTransformPoints2()
        {
            Log("P2 Testing Transform( Point[] )...");

            TestTransformPointsWith(new Matrix(1, 1, 1, 1, 1, 1), null);

            Point[] points1 = new Point[0];
            TestTransformPointsWith(new Matrix(1, 1, 1, 1, 1, 1), points1);
        }

        private void TestTransformVectors2()
        {
            Log("P2 Testing Transform( Vector[] )...");

            TestTransformVectorsWith(new Matrix(1, 1, 1, 1, 1, 1), null);

            Vector[] vectors1 = new Vector[0];
            TestTransformVectorsWith(new Matrix(1, 1, 1, 1, 1, 1), vectors1);
        }
    }
}