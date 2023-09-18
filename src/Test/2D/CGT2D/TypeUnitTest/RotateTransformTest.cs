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
    public class RotateTransformTest : CoreGraphicsTest
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


                // We only test non-animated RotateTransform, animated RotateTransform will be tested in Animation tests
                TestCloneCurrentValue();

                TestProperties();
                TestValue();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor RotateTransform()...");

            TestConstructorWith();

            Log("Testing Constructor RotateTransform( double )...");

            TestConstructorWith(0);
            TestConstructorWith(444.44);
            TestConstructorWith(-444.44);

            Log("Testing Constructor RotateTransform( double, double, double )...");

            TestConstructorWith(0, 0, 0);
            TestConstructorWith(444.44, 0, 0);
            TestConstructorWith(-444.44, 0, 0);

            TestConstructorWith(0, 11.11, -11.11);
            TestConstructorWith(444.44, 11.11, -11.11);
            TestConstructorWith(-444.44, 11.11, -11.11);
        }

        private void TestConstructorWith()
        {
            RotateTransform theirAnswer = new RotateTransform();

            if (!MathEx.Equals(theirAnswer.Angle, 0) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor RotateTransform() failed");
                Log("***Expected: Angle = {0}, CenterX = {1}, CenterY = {2}", 0, 0, 0);
                Log("***Actual: Angle = {0}, CenterX = {1}, CenterY = {2}", theirAnswer.Angle, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double angle)
        {
            RotateTransform theirAnswer = new RotateTransform(angle);

            if (!MathEx.Equals(theirAnswer.Angle, angle) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor RotateTransform( double ) failed");
                Log("***Expected: Angle = {0}, CenterX = {1}, CenterY = {2}", angle, 0, 0);
                Log("***Actual: Angle = {0}, CenterX = {1}, CenterX = {2}", theirAnswer.Angle, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double angle, double centerX, double centerY)
        {
            RotateTransform theirAnswer = new RotateTransform(angle, centerX, centerY);

            if (!MathEx.Equals(theirAnswer.Angle, angle) ||
                !MathEx.Equals(theirAnswer.CenterX, centerX) ||
                !MathEx.Equals(theirAnswer.CenterY, centerY) ||
                failOnPurpose)
            {
                AddFailure("Constructor RotateTransform( double, double, double ) failed");
                Log("***Expected: Angle = {0}, CenterX = {1}, CenterY = {2}", angle, centerX, centerY);
                Log("***Actual: Angle = {0}, CenterX = {1}, CenterY = {2}", theirAnswer.Angle, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new RotateTransform(0, 0, 0));
            TestCloneCurrentValueWith(new RotateTransform(444.44, 0, 0));
            TestCloneCurrentValueWith(new RotateTransform(-444.44, 0, 0));

            TestCloneCurrentValueWith(new RotateTransform(0, 11.11, -11.11));
            TestCloneCurrentValueWith(new RotateTransform(444.44, 11.11, -11.11));
            TestCloneCurrentValueWith(new RotateTransform(-444.44, 11.11, -11.11));
        }

        private void TestCloneCurrentValueWith(RotateTransform rotateTransform1)
        {
            RotateTransform theirAnswer = rotateTransform1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, rotateTransform1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("***Expected: {0}", rotateTransform1);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties Angle, CenterX, CenterY...");

            TestTWith(0.0, "Angle");
            TestTWith(444.44, "Angle");
            TestTWith(-444.44, "Angle");

            TestTWith(0, "CenterX");
            TestTWith(11.11, "CenterX");
            TestTWith(-11.11, "CenterX");

            TestTWith(0, "CenterY");
            TestTWith(11.11, "CenterY");
            TestTWith(-11.11, "CenterY");
        }

        private void TestTWith(double value, string property)
        {
            RotateTransform rotateTransform = new RotateTransform();
            double actual = SetTWith(ref rotateTransform, value, property);
            if (!MathEx.Equals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("***Expected: {0}", value);
                Log("***Actual:   {0}", actual);
            }
        }

        private double SetTWith(ref RotateTransform rotateTransform, double value, string property)
        {
            switch (property)
            {
                case "Angle": rotateTransform.Angle = value; return rotateTransform.Angle;
                case "CenterX": rotateTransform.CenterX = value; return rotateTransform.CenterX;
                case "CenterY": rotateTransform.CenterY = value; return rotateTransform.CenterY;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on RotateTransform");
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(new RotateTransform(0, 0, 0));
            TestValueWith(new RotateTransform(45, 0, 0));
            TestValueWith(new RotateTransform(90, 0, 0));
            TestValueWith(new RotateTransform(135, 0, 0));
            TestValueWith(new RotateTransform(180, 0, 0));
            TestValueWith(new RotateTransform(225, 0, 0));
            TestValueWith(new RotateTransform(270, 0, 0));
            TestValueWith(new RotateTransform(315, 0, 0));
            TestValueWith(new RotateTransform(360, 0, 0));
            TestValueWith(new RotateTransform(-45, 0, 0));
            TestValueWith(new RotateTransform(-90, 0, 0));
            TestValueWith(new RotateTransform(-135, 0, 0));
            TestValueWith(new RotateTransform(-180, 0, 0));
            TestValueWith(new RotateTransform(-225, 0, 0));
            TestValueWith(new RotateTransform(-270, 0, 0));
            TestValueWith(new RotateTransform(-315, 0, 0));
            TestValueWith(new RotateTransform(-360, 0, 0));
            TestValueWith(new RotateTransform(444.44, 0, 0));
            TestValueWith(new RotateTransform(-444.44, 0, 0));

            TestValueWith(new RotateTransform(0, 11.11, -11.11));
            TestValueWith(new RotateTransform(45, 11.11, -11.11));
            TestValueWith(new RotateTransform(90, 11.11, -11.11));
            TestValueWith(new RotateTransform(135, 11.11, -11.11));
            TestValueWith(new RotateTransform(180, 11.11, -11.11));
            TestValueWith(new RotateTransform(225, 11.11, -11.11));
            TestValueWith(new RotateTransform(270, 11.11, -11.11));
            TestValueWith(new RotateTransform(315, 11.11, -11.11));
            TestValueWith(new RotateTransform(360, 11.11, -11.11));
            TestValueWith(new RotateTransform(-45, 11.11, -11.11));
            TestValueWith(new RotateTransform(-90, 11.11, -11.11));
            TestValueWith(new RotateTransform(-135, 11.11, -11.11));
            TestValueWith(new RotateTransform(-180, 11.11, -11.11));
            TestValueWith(new RotateTransform(-225, 11.11, -11.11));
            TestValueWith(new RotateTransform(-270, 11.11, -11.11));
            TestValueWith(new RotateTransform(-315, 11.11, -11.11));
            TestValueWith(new RotateTransform(-360, 11.11, -11.11));
            TestValueWith(new RotateTransform(444.44, 11.11, -11.11));
            TestValueWith(new RotateTransform(-444.44, 11.11, -11.11));

            TestValueWith(new RotateTransform(0, -100.55, 10.55));
            TestValueWith(new RotateTransform(45, -100.55, 10.55));
            TestValueWith(new RotateTransform(90, -100.55, 10.55));
            TestValueWith(new RotateTransform(135, -100.55, 10.55));
            TestValueWith(new RotateTransform(180, -100.55, 10.55));
            TestValueWith(new RotateTransform(225, -100.55, 10.55));
            TestValueWith(new RotateTransform(270, -100.55, 10.55));
            TestValueWith(new RotateTransform(315, -100.55, 10.55));
            TestValueWith(new RotateTransform(360, -100.55, 10.55));
            TestValueWith(new RotateTransform(-45, -100.55, 10.55));
            TestValueWith(new RotateTransform(-90, -100.55, 10.55));
            TestValueWith(new RotateTransform(-135, -100.55, 10.55));
            TestValueWith(new RotateTransform(-180, -100.55, 10.55));
            TestValueWith(new RotateTransform(-225, -100.55, 10.55));
            TestValueWith(new RotateTransform(-270, -100.55, 10.55));
            TestValueWith(new RotateTransform(-315, -100.55, 10.55));
            TestValueWith(new RotateTransform(-360, -100.55, 10.55));
            TestValueWith(new RotateTransform(444.44, -100.55, 10.55));
            TestValueWith(new RotateTransform(-444.44, -100.55, 10.55));
        }

        private void TestValueWith(RotateTransform rotateTransform1)
        {
            Matrix theirAnswer = rotateTransform1.Value;

            Matrix myAnswer = MatrixUtils.Rotate(rotateTransform1.Angle, new Point(rotateTransform1.CenterX, rotateTransform1.CenterY));

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
            Log("P2 Testing Constructor RotateTransform( double )...");

            TestConstructorWith(Const2D.min);
            TestConstructorWith(Const2D.max);
            TestConstructorWith(Const2D.negInf);
            TestConstructorWith(Const2D.posInf);
            TestConstructorWith(Const2D.nan);
            TestConstructorWith(Const2D.epsilon);

            Log("P2 Testing Constructor RotateTransform( double, double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min, Const2D.max);
            TestConstructorWith(Const2D.min, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.min, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.negInf);
            TestConstructorWith(Const2D.max, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.negInf, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon);
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new RotateTransform(Const2D.min, Const2D.min, Const2D.max));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.min, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.min, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.max, Const2D.max, Const2D.negInf));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.max, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.negInf, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.nan, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new RotateTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties Angle, Center...");

            TestTWith(Const2D.min, "Angle");
            TestTWith(Const2D.max, "Angle");
            TestTWith(Const2D.negInf, "Angle");
            TestTWith(Const2D.posInf, "Angle");
            TestTWith(Const2D.nan, "Angle");
            TestTWith(Const2D.epsilon, "Angle");

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

            TestValueWith(new RotateTransform(Const2D.min, Const2D.min, Const2D.max));
            TestValueWith(new RotateTransform(Const2D.min, Const2D.negInf, Const2D.posInf));
            TestValueWith(new RotateTransform(Const2D.min, Const2D.nan, Const2D.nan));
            TestValueWith(new RotateTransform(Const2D.max, Const2D.max, Const2D.negInf));
            TestValueWith(new RotateTransform(Const2D.max, Const2D.posInf, Const2D.nan));
            TestValueWith(new RotateTransform(Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestValueWith(new RotateTransform(Const2D.negInf, Const2D.nan, Const2D.nan));
            TestValueWith(new RotateTransform(Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestValueWith(new RotateTransform(Const2D.nan, Const2D.nan, Const2D.nan));
            TestValueWith(new RotateTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }
    }
}