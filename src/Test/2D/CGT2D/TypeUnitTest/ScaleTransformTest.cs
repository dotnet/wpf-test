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
    public class ScaleTransformTest : CoreGraphicsTest
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
              
                // We only test non-animated ScaleTransform, animated ScaleTransform will be tested in Animation tests
                TestCloneCurrentValue();

                TestProperties();
                TestValue();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor ScaleTransform()...");

            TestConstructorWith();

            Log("Testing Constructor ScaleTransform( double, double )...");

            TestConstructorWith(0, 0);
            TestConstructorWith(1, 1);
            TestConstructorWith(11.11, 0.11);
            TestConstructorWith(-0.11, -11.11);

            Log("Testing Constructor ScaleTransform( double, double, double, double )...");

            TestConstructorWith(0, 0, 0, 0);
            TestConstructorWith(1, 1, 0, 0);
            TestConstructorWith(11.11, 0.11, 0, 0);
            TestConstructorWith(-0.11, -11.11, 0, 0);

            TestConstructorWith(0, 0, 11.11, -11.11);
            TestConstructorWith(1, 1, 11.11, -11.11);
            TestConstructorWith(11.11, 0.11, 11.11, -11.11);
            TestConstructorWith(-0.11, -11.11, 11.11, -11.11);
        }

        private void TestConstructorWith()
        {
            ScaleTransform theirAnswer = new ScaleTransform();

            if (!MathEx.Equals(theirAnswer.ScaleX, 1) ||
                !MathEx.Equals(theirAnswer.ScaleY, 1) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor ScaleTransform() failed");
                Log("***Expected: ScaleX = {0}, ScaleY = {1}, CenterX = {2}, CenterY = {3}", 1, 1, 0, 0);
                Log("***Actual: ScaleX = {0}, ScaleY = {1}, CenterX  = {2}, CenterY = {3}", theirAnswer.ScaleX, theirAnswer.ScaleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double scaleX, double scaleY)
        {
            ScaleTransform theirAnswer = new ScaleTransform(scaleX, scaleY);

            if (!MathEx.Equals(theirAnswer.ScaleX, scaleX) ||
                !MathEx.Equals(theirAnswer.ScaleY, scaleY) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor ScaleTransform( double, double ) failed");
                Log("***Expected: ScaleX = {0}, ScaleY = {1}, CenterX = {2}, CenterY = {3}", scaleX, scaleY, 0, 0);
                Log("***Actual: ScaleX = {0}, ScaleY = {1}, CenterX = {2}, CenterY = {3}", theirAnswer.ScaleX, theirAnswer.ScaleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double scaleX, double scaleY, double centerX, double centerY)
        {
            ScaleTransform theirAnswer = new ScaleTransform(scaleX, scaleY, centerX, centerY);

            if (!MathEx.Equals(theirAnswer.ScaleX, scaleX) ||
                !MathEx.Equals(theirAnswer.ScaleY, scaleY) ||
                !MathEx.Equals(theirAnswer.CenterX, centerX) ||
                !MathEx.Equals(theirAnswer.CenterY, centerY) ||
                failOnPurpose)
            {
                AddFailure("Constructor ScaleTransform( double, double, double, double ) failed");
                Log("***Expected: ScaleX = {0}, ScaleY = {1}, CenterX = {2}, CenterY = {3}", scaleX, scaleY, centerX, centerY);
                Log("***Actual: ScaleX = {0}, ScaleY = {1}, CenterX = {2}, CenterY = {3}", theirAnswer.ScaleX, theirAnswer.ScaleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new ScaleTransform(0, 0, 0, 0));
            TestCloneCurrentValueWith(new ScaleTransform(1, 1, 0, 0));
            TestCloneCurrentValueWith(new ScaleTransform(11.11, 0.11, 0, 0));
            TestCloneCurrentValueWith(new ScaleTransform(-0.11, -11.11, 0, 0));

            TestCloneCurrentValueWith(new ScaleTransform(0, 0, 11.11, -11.11));
            TestCloneCurrentValueWith(new ScaleTransform(1, 1, 11.11, -11.11));
            TestCloneCurrentValueWith(new ScaleTransform(11.11, 0.11, 11.11, -11.11));
            TestCloneCurrentValueWith(new ScaleTransform(-0.11, -11.11, 11.11, -11.11));
        }

        private void TestCloneCurrentValueWith(ScaleTransform scaleTransform1)
        {
            ScaleTransform theirAnswer = scaleTransform1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, scaleTransform1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("***Expected: {0}", scaleTransform1);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties ScaleX, ScaleY, Center...");

            TestTWith(0.0, "ScaleX");
            TestTWith(1, "ScaleX");
            TestTWith(11.11, "ScaleX");
            TestTWith(-0.11, "ScaleX");

            TestTWith(0.0, "ScaleY");
            TestTWith(1, "ScaleY");
            TestTWith(11.11, "ScaleY");
            TestTWith(-0.11, "ScaleY");

            TestTWith(0.0, "CenterX");
            TestTWith(1, "CenterX");
            TestTWith(11.11, "CenterX");
            TestTWith(-0.11, "CenterX");

            TestTWith(0.0, "CenterY");
            TestTWith(1, "CenterY");
            TestTWith(11.11, "CenterY");
            TestTWith(-0.11, "CenterY");
        }

        private void TestTWith(double value, string property)
        {
            ScaleTransform scaleTransform = new ScaleTransform();
            double actual = SetTWith(ref scaleTransform, value, property);
            if (!MathEx.Equals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("***Expected: {0}", value);
                Log("***Actual:   {0}", actual);
            }
        }

        private double SetTWith(ref ScaleTransform scaleTransform, double value, string property)
        {
            switch (property)
            {
                case "ScaleX": scaleTransform.ScaleX = value; return scaleTransform.ScaleX;
                case "ScaleY": scaleTransform.ScaleY = value; return scaleTransform.ScaleY;
                case "CenterX": scaleTransform.CenterX = value; return scaleTransform.CenterX;
                case "CenterY": scaleTransform.CenterY = value; return scaleTransform.CenterY;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on ScaleTransform");
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(new ScaleTransform(0, 0, 0, 0));
            TestValueWith(new ScaleTransform(1, 1, 0, 0));
            TestValueWith(new ScaleTransform(1.5, 1.5, 0, 0));
            TestValueWith(new ScaleTransform(2.0, 2.0, 0, 0));
            TestValueWith(new ScaleTransform(-1.5, -1.5, 0, 0));
            TestValueWith(new ScaleTransform(-2.0, -2.0, 0, 0));
            TestValueWith(new ScaleTransform(11.11, 0.11, 0, 0));
            TestValueWith(new ScaleTransform(-0.11, -11.11, 0, 0));

            TestValueWith(new ScaleTransform(0, 0, 11.11, -11.11));
            TestValueWith(new ScaleTransform(1, 1, 11.11, -11.11));
            TestValueWith(new ScaleTransform(1.5, 1.5, 11.11, -11.11));
            TestValueWith(new ScaleTransform(2.0, 2.0, 11.11, -11.11));
            TestValueWith(new ScaleTransform(-1.5, -1.5, 11.11, -11.11));
            TestValueWith(new ScaleTransform(-2.0, -2.0, 11.11, -11.11));
            TestValueWith(new ScaleTransform(11.11, 0.11, 11.11, -11.11));
            TestValueWith(new ScaleTransform(-0.11, -11.11, 11.11, -11.11));

            TestValueWith(new ScaleTransform(0, 0, -100.55, 10.55));
            TestValueWith(new ScaleTransform(1, 1, -100.55, 10.55));
            TestValueWith(new ScaleTransform(1.5, 1.5, -100.55, 10.55));
            TestValueWith(new ScaleTransform(2.0, 2.0, -100.55, 10.55));
            TestValueWith(new ScaleTransform(-1.5, -1.5, -100.55, 10.55));
            TestValueWith(new ScaleTransform(-2.0, -2.0, -100.55, 10.55));
            TestValueWith(new ScaleTransform(11.11, 0.11, -100.55, 10.55));
            TestValueWith(new ScaleTransform(-0.11, -11.11, -100.55, 10.55));
        }

        private void TestValueWith(ScaleTransform scaleTransform1)
        {
            Matrix theirAnswer = scaleTransform1.Value;

            Matrix myAnswer = MatrixUtils.Scale(scaleTransform1.ScaleX, scaleTransform1.ScaleY, new Point(scaleTransform1.CenterX, scaleTransform1.CenterY));

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
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

            // Disabled, need better verification code, then enable again
            //TestValue2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor ScaleTransform( double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan);
            TestConstructorWith(1 + Const2D.epsilon, 1 + Const2D.epsilon);
            TestConstructorWith(1 - Const2D.epsilon, 1 - Const2D.epsilon);

            Log("P2 Testing Constructor ScaleTransform( double, double, double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min, Const2D.min, Const2D.max);
            TestConstructorWith(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan);
            TestConstructorWith(1 + Const2D.epsilon, 1 + Const2D.epsilon, Const2D.epsilon, Const2D.epsilon);
            TestConstructorWith(1 - Const2D.epsilon, 1 - Const2D.epsilon, Const2D.epsilon, Const2D.epsilon);
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.max));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new ScaleTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new ScaleTransform(1 + Const2D.epsilon, 1 + Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
            TestCloneCurrentValueWith(new ScaleTransform(1 - Const2D.epsilon, 1 - Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties Angle, Center...");

            TestTWith(Const2D.min, "ScaleX");
            TestTWith(Const2D.max, "ScaleX");
            TestTWith(Const2D.negInf, "ScaleX");
            TestTWith(Const2D.posInf, "ScaleX");
            TestTWith(Const2D.nan, "ScaleX");
            TestTWith(1 + Const2D.epsilon, "ScaleX");
            TestTWith(1 - Const2D.epsilon, "ScaleX");

            TestTWith(Const2D.min, "ScaleY");
            TestTWith(Const2D.max, "ScaleY");
            TestTWith(Const2D.negInf, "ScaleY");
            TestTWith(Const2D.posInf, "ScaleY");
            TestTWith(Const2D.nan, "ScaleY");
            TestTWith(1 + Const2D.epsilon, "ScaleY");
            TestTWith(1 - Const2D.epsilon, "ScaleY");

            TestTWith(Const2D.min, "CenterX");
            TestTWith(Const2D.max, "CenterX");
            TestTWith(Const2D.negInf, "CenterX");
            TestTWith(Const2D.posInf, "CenterX");
            TestTWith(Const2D.nan, "CenterX");
            TestTWith(Const2D.epsilon, "CenterX");

            TestTWith(Const2D.min, "CenterY");
            TestTWith(Const2D.max, "CenterY");
            TestTWith(Const2D.negInf, "CenterY");
            TestTWith(Const2D.posInf, "CenterY");
            TestTWith(Const2D.nan, "CenterY");
            TestTWith(Const2D.epsilon, "CenterY");
        }

        private void TestValue2()
        {
            Log("P2 Testing Value...");

            TestValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.max));
            TestValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf));
            TestValueWith(new ScaleTransform(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan));
            TestValueWith(new ScaleTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf));
            TestValueWith(new ScaleTransform(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan));
            TestValueWith(new ScaleTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestValueWith(new ScaleTransform(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan));
            TestValueWith(new ScaleTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestValueWith(new ScaleTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestValueWith(new ScaleTransform(1 + Const2D.epsilon, 1 + Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
            TestValueWith(new ScaleTransform(1 - Const2D.epsilon, 1 - Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }
    }
}