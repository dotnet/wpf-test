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
    public class TranslateTransformTest : CoreGraphicsTest
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

                // We only test non-animated TranslateTransform, animated TranslateTransform will be tested in Animation tests
                TestCloneCurrentValue();

                TestProperties();
                TestValue();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor TranslateTransform()...");

            TestConstructorWith();

            Log("Testing Constructor TranslateTransform( double, double )...");

            TestConstructorWith(0, 0);
            TestConstructorWith(11.11, 11.11);
            TestConstructorWith(-11.11, -11.11);
        }

        private void TestConstructorWith()
        {
            TranslateTransform theirAnswer = new TranslateTransform();

            if (!MathEx.Equals(theirAnswer.X, 0) ||
                !MathEx.Equals(theirAnswer.Y, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor TranslateTransform() failed");
                Log("***Expected: X = {0}, Y = {1}", 0, 0);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestConstructorWith(double x, double y)
        {
            TranslateTransform theirAnswer = new TranslateTransform(x, y);

            if (!MathEx.Equals(theirAnswer.X, x) ||
                !MathEx.Equals(theirAnswer.Y, y) ||
                failOnPurpose)
            {
                AddFailure("Constructor TranslateTransform( double, double ) failed");
                Log("***Expected: X = {0}, Y = {1}", x, y);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new TranslateTransform());
            TestCloneCurrentValueWith(new TranslateTransform(11.11, 11.11));
            TestCloneCurrentValueWith(new TranslateTransform(-11.11, -11.11));
        }

        private void TestCloneCurrentValueWith(TranslateTransform translateTransform1)
        {
            TranslateTransform theirAnswer = translateTransform1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, translateTransform1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("***Expected: {0}", translateTransform1);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties X, Y...");

            TestTWith(0.0, "X");
            TestTWith(11.11, "X");
            TestTWith(-11.11, "X");

            TestTWith(0.0, "Y");
            TestTWith(11.11, "Y");
            TestTWith(-11.11, "Y");
        }

        private void TestTWith(double value, string property)
        {
            TranslateTransform translateTransform = new TranslateTransform();
            double actual = SetTWith(ref translateTransform, value, property);
            if (!MathEx.Equals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("***Expected: {0}", value);
                Log("***Actual:   {0}", actual);
            }
        }

        private double SetTWith(ref TranslateTransform translateTransform, double value, string property)
        {
            switch (property)
            {
                case "X": translateTransform.X = value; return translateTransform.X;
                case "Y": translateTransform.Y = value; return translateTransform.Y;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on TranslateTransform");
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(new TranslateTransform());
            TestValueWith(new TranslateTransform(11.11, 11.11));
            TestValueWith(new TranslateTransform(-11.11, -11.11));
        }

        private void TestValueWith(TranslateTransform translateTransform1)
        {
            Matrix theirAnswer = translateTransform1.Value;

            Matrix myAnswer = MatrixUtils.Translate(translateTransform1.X, translateTransform1.Y);

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
            Log("P2 Testing Constructor TranslateTransform( double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.epsilon, Const2D.epsilon);
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new TranslateTransform(Const2D.min, Const2D.min));
            TestCloneCurrentValueWith(new TranslateTransform(Const2D.max, Const2D.max));
            TestCloneCurrentValueWith(new TranslateTransform(Const2D.negInf, Const2D.negInf));
            TestCloneCurrentValueWith(new TranslateTransform(Const2D.posInf, Const2D.posInf));
            TestCloneCurrentValueWith(new TranslateTransform(Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new TranslateTransform(Const2D.epsilon, Const2D.epsilon));
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties X, Y...");

            TestTWith(Const2D.min, "X");
            TestTWith(Const2D.max, "X");
            TestTWith(Const2D.negInf, "X");
            TestTWith(Const2D.posInf, "X");
            TestTWith(Const2D.nan, "X");
            TestTWith(Const2D.epsilon, "X");

            TestTWith(Const2D.min, "Y");
            TestTWith(Const2D.max, "Y");
            TestTWith(Const2D.negInf, "Y");
            TestTWith(Const2D.posInf, "Y");
            TestTWith(Const2D.nan, "Y");
            TestTWith(Const2D.epsilon, "Y");
        }

        private void TestValue2()
        {
            Log("P2 Testing Value...");

            TestValueWith(new TranslateTransform(Const2D.min, Const2D.min));
            TestValueWith(new TranslateTransform(Const2D.max, Const2D.max));
            TestValueWith(new TranslateTransform(Const2D.negInf, Const2D.negInf));
            TestValueWith(new TranslateTransform(Const2D.posInf, Const2D.posInf));
            TestValueWith(new TranslateTransform(Const2D.nan, Const2D.nan));
            TestValueWith(new TranslateTransform(Const2D.epsilon, Const2D.epsilon));
        }
    }
}