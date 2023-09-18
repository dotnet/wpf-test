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
    public class MatrixTransformTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTest2();
            }
            else
            {
                TestConstructor();


                // We only test non-animated MatrixTransform, animated MatrixTransform will be tested in Animation tests
                TestCloneCurrentValue();

                TestProperties();
                TestValue();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor MatrixTransform()...");

            TestConstructorWith();

            Log("Testing Constructor MatrixTransform( double, double, double, double, double, double )...");

            TestConstructorWith(0, 0, 0, 0, 0, 0);
            TestConstructorWith(1, 0, 0, 1, 0, 0);
            TestConstructorWith(11.11, 11.11, 11.11, 11.11, 11.11, 11.11);
            TestConstructorWith(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11);
            TestConstructorWith(1, 0, 0, 1, 11.11, -0.22);
            TestConstructorWith(11.11, 0, 0, -0.22, 0, 0);
            TestConstructorWith(11.11, 0, 0, -0.22, 11.11, -0.22);
            TestConstructorWith(11.11, -11.11, 0.22, -0.22, 11.11, 0.22);

            Log("Testing Constructor MatrixTransform( Matrix )...");

            TestConstructorWith(new Matrix(0, 0, 0, 0, 0, 0));
            TestConstructorWith(new Matrix(1, 0, 0, 1, 0, 0));
            TestConstructorWith(new Matrix(11.11, 11.11, 11.11, 11.11, 11.11, 11.11));
            TestConstructorWith(new Matrix(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11));
            TestConstructorWith(new Matrix(1, 0, 0, 1, 11.11, -0.22));
            TestConstructorWith(new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestConstructorWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestConstructorWith(new Matrix(11.11, -11.11, 0.22, -0.22, 11.11, 0.22));
        }

        private void TestConstructorWith()
        {
            MatrixTransform theirAnswer = new MatrixTransform();

            if (!MathEx.Equals(theirAnswer.Matrix, Matrix.Identity) || failOnPurpose)
            {
                AddFailure("Constructor MatrixTransform() failed");
                Log("***Expected: Matrix = {0}", Matrix.Identity);
                Log("***Actual: Matrix = {0}", theirAnswer.Matrix);
            }
        }

        private void TestConstructorWith(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            MatrixTransform theirAnswer = new MatrixTransform(m11, m12, m21, m22, offsetX, offsetY);

            if (!MathEx.Equals(theirAnswer.Matrix, new Matrix(m11, m12, m21, m22, offsetX, offsetY)) || failOnPurpose)
            {
                AddFailure("Constructor MatrixTransform( double, double, double, double, double, double ) failed");
                Log("***Expected: Matrix = {0}", new Matrix(m11, m12, m21, m22, offsetX, offsetY));
                Log("***Actual: Matrix = {0}", theirAnswer.Matrix);
            }
        }

        private void TestConstructorWith(Matrix matrix1)
        {
            MatrixTransform theirAnswer = new MatrixTransform(matrix1);

            if (!MathEx.Equals(theirAnswer.Matrix, matrix1) || failOnPurpose)
            {
                AddFailure("Constructor MatrixTransform( Matrix ) failed");
                Log("***Expected: Matrix = {0}", matrix1);
                Log("***Actual: Matrix = {0}", theirAnswer.Matrix);
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new MatrixTransform(0, 0, 0, 0, 0, 0));
            TestCloneCurrentValueWith(new MatrixTransform(1, 0, 0, 1, 0, 0));
            TestCloneCurrentValueWith(new MatrixTransform(11.11, 11.11, 11.11, 11.11, 11.11, 11.11));
            TestCloneCurrentValueWith(new MatrixTransform(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11));
            TestCloneCurrentValueWith(new MatrixTransform(1, 0, 0, 1, 11.11, -0.22));
            TestCloneCurrentValueWith(new MatrixTransform(11.11, 0, 0, -0.22, 0, 0));
            TestCloneCurrentValueWith(new MatrixTransform(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestCloneCurrentValueWith(new MatrixTransform(11.11, -11.11, 0.22, -0.22, 11.11, 0.22));
        }

        private void TestCloneCurrentValueWith(MatrixTransform matrixTransform1)
        {
            MatrixTransform theirAnswer = matrixTransform1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, matrixTransform1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("***Expected: {0}", matrixTransform1);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties Matrix...");

            TestTWith(new Matrix(0, 0, 0, 0, 0, 0), "Matrix");
            TestTWith(new Matrix(1, 0, 0, 1, 0, 0), "Matrix");
            TestTWith(new Matrix(11.11, 11.11, 11.11, 11.11, 11.11, 11.11), "Matrix");
            TestTWith(new Matrix(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11), "Matrix");
            TestTWith(new Matrix(1, 0, 0, 1, 11.11, -0.22), "Matrix");
            TestTWith(new Matrix(11.11, 0, 0, -0.22, 0, 0), "Matrix");
            TestTWith(new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22), "Matrix");
            TestTWith(new Matrix(11.11, -11.11, 0.22, -0.22, 11.11, 0.22), "Matrix");
        }

        private void TestTWith(Matrix value, string property)
        {
            MatrixTransform matrixTransform = new MatrixTransform();
            Matrix actual = SetTWith(ref matrixTransform, value, property);
            if (!MathEx.Equals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("***Expected: {0}", value);
                Log("***Actual:   {0}", actual);
            }
        }

        private Matrix SetTWith(ref MatrixTransform matrixTransform, Matrix value, string property)
        {
            switch (property)
            {
                case "Matrix": matrixTransform.Matrix = value; return matrixTransform.Matrix;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on MatrixTransform");
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(new MatrixTransform(0, 0, 0, 0, 0, 0));
            TestValueWith(new MatrixTransform(1, 0, 0, 1, 0, 0));
            TestValueWith(new MatrixTransform(11.11, 11.11, 11.11, 11.11, 11.11, 11.11));
            TestValueWith(new MatrixTransform(-11.11, -11.11, -11.11, -11.11, -11.11, -11.11));
            TestValueWith(new MatrixTransform(1, 0, 0, 1, 11.11, -0.22));
            TestValueWith(new MatrixTransform(11.11, 0, 0, -0.22, 0, 0));
            TestValueWith(new MatrixTransform(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestValueWith(new MatrixTransform(11.11, -11.11, 0.22, -0.22, 11.11, 0.22));
        }

        private void TestValueWith(MatrixTransform matrixTransform1)
        {
            Matrix theirAnswer = matrixTransform1.Value;

            Matrix myAnswer = matrixTransform1.Matrix;

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Value failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void RunTest2()
        {
            // these are P2 tests, overflow, bignumber, exception
            TestConstructor2();
            TestCloneCurrentValue2();
            TestProperties2();
            TestValue2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor MatrixTransform( double, double, double, double, double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon);

            Log("P2 Testing Constructor MatrixTransform( Matrix )...");

            TestConstructorWith(new Matrix(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min));
            TestConstructorWith(new Matrix(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max));
            TestConstructorWith(new Matrix(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf));
            TestConstructorWith(new Matrix(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestConstructorWith(new Matrix(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestConstructorWith(new Matrix(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new MatrixTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min));
            TestCloneCurrentValueWith(new MatrixTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max));
            TestCloneCurrentValueWith(new MatrixTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf));
            TestCloneCurrentValueWith(new MatrixTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestCloneCurrentValueWith(new MatrixTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new MatrixTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties Matrix...");

            TestTWith(new Matrix(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min), "Matrix");
            TestTWith(new Matrix(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max), "Matrix");
            TestTWith(new Matrix(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf), "Matrix");
            TestTWith(new Matrix(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf), "Matrix");
            TestTWith(new Matrix(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan), "Matrix");
            TestTWith(new Matrix(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon), "Matrix");
        }

        private void TestValue2()
        {
            Log("P2 Testing Value...");

            TestValueWith(new MatrixTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min, Const2D.min));
            TestValueWith(new MatrixTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max, Const2D.max));
            TestValueWith(new MatrixTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.negInf));
            TestValueWith(new MatrixTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestValueWith(new MatrixTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestValueWith(new MatrixTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }
    }
}