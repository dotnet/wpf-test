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
    public class SkewTransformTest : CoreGraphicsTest
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


                // We only test non-animated SkewTransform, animated SkewTransform will be tested in Animation tests
                TestCloneCurrentValue();

                TestProperties();
                TestValue();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor SkewTransform()...");

            TestConstructorWith();

            Log("Testing Constructor SkewTransform( double, double )...");

            TestConstructorWith(0, 0);
            TestConstructorWith(444.44, 444.44);
            TestConstructorWith(-444.44, -444.44);
            TestConstructorWith(45, 45);
            TestConstructorWith(90, 90);

            Log("Testing Constructor SkewTransform( double, double, double, double )...");

            TestConstructorWith(0, 0, 0, 0);
            TestConstructorWith(444.44, 444.44, 0, 0);
            TestConstructorWith(-444.44, -444.44, 0, 0);
            TestConstructorWith(45, 45, 0, 0);
            TestConstructorWith(90, 90, 0, 0);

            TestConstructorWith(0, 0, 11.11, -11.11);
            TestConstructorWith(444.44, 444.44, 11.11, -11.11);
            TestConstructorWith(-444.44, -444.44, 11.11, -11.11);
            TestConstructorWith(45, 45, 11.11, -11.11);
            TestConstructorWith(90, 90, 11.11, -11.11);
        }

        private void TestConstructorWith()
        {
            SkewTransform theirAnswer = new SkewTransform();

            if (!MathEx.Equals(theirAnswer.AngleX, 0) ||
                !MathEx.Equals(theirAnswer.AngleY, 0) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor SkewTransform() failed");
                Log("***Expected: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", 0, 0, 0, 0);
                Log("***Actual: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", theirAnswer.AngleX, theirAnswer.AngleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double angleX, double angleY)
        {
            SkewTransform theirAnswer = new SkewTransform(angleX, angleY);

            if (!MathEx.Equals(theirAnswer.AngleX, angleX) ||
                !MathEx.Equals(theirAnswer.AngleY, angleY) ||
                !MathEx.Equals(theirAnswer.CenterX, 0) ||
                !MathEx.Equals(theirAnswer.CenterY, 0) ||
                failOnPurpose)
            {
                AddFailure("Constructor SkewTransform( double, double ) failed");
                Log("***Expected: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", angleX, angleY, 0, 0);
                Log("***Actual: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", theirAnswer.AngleX, theirAnswer.AngleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestConstructorWith(double angleX, double angleY, double centerX, double centerY)
        {
            SkewTransform theirAnswer = new SkewTransform(angleX, angleY, centerX, centerY);

            if (!MathEx.Equals(theirAnswer.AngleX, angleX) ||
                !MathEx.Equals(theirAnswer.AngleY, angleY) ||
                !MathEx.Equals(theirAnswer.CenterX, centerX) ||
                !MathEx.Equals(theirAnswer.CenterY, centerY) ||
                failOnPurpose)
            {
                AddFailure("Constructor SkewTransform( double, double, double, double ) failed");
                Log("***Expected: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", angleX, angleY, centerX, centerY);
                Log("***Actual: AngleX = {0}, AngleY = {1}, CenterX = {2}, CenterY = {3}", theirAnswer.AngleX, theirAnswer.AngleY, theirAnswer.CenterX, theirAnswer.CenterY);
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new SkewTransform(0, 0, 0, 0));
            TestCloneCurrentValueWith(new SkewTransform(444.44, 444.44, 0, 0));
            TestCloneCurrentValueWith(new SkewTransform(-444.44, -444.44, 0, 0));
            TestCloneCurrentValueWith(new SkewTransform(45, 45, 0, 0));
            TestCloneCurrentValueWith(new SkewTransform(90, 90, 0, 0));

            TestCloneCurrentValueWith(new SkewTransform(0, 0, 11.11, -11.11));
            TestCloneCurrentValueWith(new SkewTransform(444.44, 444.44, 11.11, -11.11));
            TestCloneCurrentValueWith(new SkewTransform(-444.44, -444.44, 11.11, -11.11));
            TestCloneCurrentValueWith(new SkewTransform(45, 45, 11.11, -11.11));
            TestCloneCurrentValueWith(new SkewTransform(90, 90, 11.11, -11.11));
        }

        private void TestCloneCurrentValueWith(SkewTransform skewTransform1)
        {
            SkewTransform theirAnswer = skewTransform1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, skewTransform1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("***Expected: {0}", skewTransform1);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties AngleX, AngleY, CenterX, CenterY...");

            TestTWith(0.0, "AngleX");
            TestTWith(444.44, "AngleX");
            TestTWith(-444.44, "AngleX");
            TestTWith(90, "AngleX");

            TestTWith(0.0, "AngleY");
            TestTWith(444.44, "AngleY");
            TestTWith(-444.44, "AngleY");
            TestTWith(90, "AngleY");

            TestTWith(0, "CenterX");
            TestTWith(11.11, "CenterX");
            TestTWith(-11.11, "CenterX");

            TestTWith(0, "CenterY");
            TestTWith(11.11, "CenterY");
            TestTWith(-11.11, "CenterY");
        }

        private void TestTWith(double value, string property)
        {
            SkewTransform skewTransform = new SkewTransform();
            double actual = SetTWith(ref skewTransform, value, property);
            if (!MathEx.Equals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("***Expected: {0}", value);
                Log("***Actual:   {0}", actual);
            }
        }

        private double SetTWith(ref SkewTransform skewTransform, double value, string property)
        {
            switch (property)
            {
                case "AngleX": skewTransform.AngleX = value; return skewTransform.AngleX;
                case "AngleY": skewTransform.AngleY = value; return skewTransform.AngleY;
                case "CenterX": skewTransform.CenterX = value; return skewTransform.CenterX;
                case "CenterY": skewTransform.CenterX = value; return skewTransform.CenterX;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on SkewTransform");
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(new SkewTransform(0, 0, 0, 0));
            TestValueWith(new SkewTransform(45, 45, 0, 0));
            TestValueWith(new SkewTransform(90, 90, 0, 0));
            TestValueWith(new SkewTransform(135, -135, 0, 0));
            TestValueWith(new SkewTransform(180, -180, 0, 0));
            TestValueWith(new SkewTransform(225, -225, 0, 0));
            TestValueWith(new SkewTransform(270, -270, 0, 0));
            TestValueWith(new SkewTransform(315, -315, 0, 0));
            TestValueWith(new SkewTransform(360, -360, 0, 0));
            TestValueWith(new SkewTransform(-45, 45, 0, 0));
            TestValueWith(new SkewTransform(-90, 90, 0, 0));
            TestValueWith(new SkewTransform(-135, 135, 0, 0));
            TestValueWith(new SkewTransform(-180, 180, 0, 0));
            TestValueWith(new SkewTransform(-225, 225, 0, 0));
            TestValueWith(new SkewTransform(-270, 270, 0, 0));
            TestValueWith(new SkewTransform(-315, 315, 0, 0));
            TestValueWith(new SkewTransform(-360, 360, 0, 0));
            TestValueWith(new SkewTransform(444.44, 444.44, 0, 0));
            TestValueWith(new SkewTransform(-444.44, -444.44, 0, 0));

            TestValueWith(new SkewTransform(0, 0, 11.11, -11.11));
            TestValueWith(new SkewTransform(45, 45, 11.11, -11.11));
            TestValueWith(new SkewTransform(90, 90, 11.11, -11.11));
            TestValueWith(new SkewTransform(135, -135, 11.11, -11.11));
            TestValueWith(new SkewTransform(180, -180, 11.11, -11.11));
            TestValueWith(new SkewTransform(225, -225, 11.11, -11.11));
            TestValueWith(new SkewTransform(270, -270, 11.11, -11.11));
            TestValueWith(new SkewTransform(315, -315, 11.11, -11.11));
            TestValueWith(new SkewTransform(360, -360, 11.11, -11.11));
            TestValueWith(new SkewTransform(-45, 45, 11.11, -11.11));
            TestValueWith(new SkewTransform(-90, 90, 11.11, -11.11));
            TestValueWith(new SkewTransform(-135, 135, 11.11, -11.11));
            TestValueWith(new SkewTransform(-180, 180, 11.11, -11.11));
            TestValueWith(new SkewTransform(-225, 225, 11.11, -11.11));
            TestValueWith(new SkewTransform(-270, 270, 11.11, -11.11));
            TestValueWith(new SkewTransform(-315, 315, 11.11, -11.11));
            TestValueWith(new SkewTransform(-360, 360, 11.11, -11.11));
            TestValueWith(new SkewTransform(444.44, 444.44, 11.11, -11.11));
            TestValueWith(new SkewTransform(-444.44, -444.44, 11.11, -11.11));

            TestValueWith(new SkewTransform(0, 0, -100.55, 10.55));
            TestValueWith(new SkewTransform(45, 45, -100.55, 10.55));
            TestValueWith(new SkewTransform(90, 90, -100.55, 10.55));
            TestValueWith(new SkewTransform(135, -135, -100.55, 10.55));
            TestValueWith(new SkewTransform(180, -180, -100.55, 10.55));
            TestValueWith(new SkewTransform(225, -225, -100.55, 10.55));
            TestValueWith(new SkewTransform(270, -270, -100.55, 10.55));
            TestValueWith(new SkewTransform(315, -315, -100.55, 10.55));
            TestValueWith(new SkewTransform(360, -360, -100.55, 10.55));
            TestValueWith(new SkewTransform(-45, 45, -100.55, 10.55));
            TestValueWith(new SkewTransform(-90, 90, -100.55, 10.55));
            TestValueWith(new SkewTransform(-135, 135, -100.55, 10.55));
            TestValueWith(new SkewTransform(-180, 180, -100.55, 10.55));
            TestValueWith(new SkewTransform(-225, 225, -100.55, 10.55));
            TestValueWith(new SkewTransform(-270, 270, -100.55, 10.55));
            TestValueWith(new SkewTransform(-315, 315, -100.55, 10.55));
            TestValueWith(new SkewTransform(-360, 360, -100.55, 10.55));
            TestValueWith(new SkewTransform(444.44, 444.44, -100.55, 10.55));
            TestValueWith(new SkewTransform(-444.44, -444.44, -100.55, 10.55));
        }

        private void TestValueWith(SkewTransform skewTransform1)
        {
            Matrix theirAnswer = skewTransform1.Value;

            Matrix myAnswer = MatrixUtils.Skew(skewTransform1.AngleX, skewTransform1.AngleY, new Point(skewTransform1.CenterX, skewTransform1.CenterY));

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
            Log("P2 Testing Constructor SkewTransform( double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.epsilon, Const2D.epsilon);

            Log("P2 Testing Constructor SkewTransform( double, double, double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min, Const2D.min, Const2D.max);
            TestConstructorWith(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf);
            TestConstructorWith(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon);
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            TestCloneCurrentValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.max));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestCloneCurrentValueWith(new SkewTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties AngleX, AngleY, CenterX, CenterY...");

            TestTWith(Const2D.min, "AngleX");
            TestTWith(Const2D.max, "AngleX");
            TestTWith(Const2D.negInf, "AngleX");
            TestTWith(Const2D.posInf, "AngleX");
            TestTWith(Const2D.nan, "AngleX");
            TestTWith(Const2D.epsilon, "AngleX");

            TestTWith(Const2D.min, "AngleY");
            TestTWith(Const2D.max, "AngleY");
            TestTWith(Const2D.negInf, "AngleY");
            TestTWith(Const2D.posInf, "AngleY");
            TestTWith(Const2D.nan, "AngleY");
            TestTWith(Const2D.epsilon, "AngleY");

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

            TestValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.min, Const2D.max));
            TestValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.negInf, Const2D.posInf));
            TestValueWith(new SkewTransform(Const2D.min, Const2D.min, Const2D.nan, Const2D.nan));
            TestValueWith(new SkewTransform(Const2D.max, Const2D.max, Const2D.max, Const2D.negInf));
            TestValueWith(new SkewTransform(Const2D.max, Const2D.max, Const2D.posInf, Const2D.nan));
            TestValueWith(new SkewTransform(Const2D.negInf, Const2D.negInf, Const2D.negInf, Const2D.posInf));
            TestValueWith(new SkewTransform(Const2D.negInf, Const2D.negInf, Const2D.nan, Const2D.nan));
            TestValueWith(new SkewTransform(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.nan));
            TestValueWith(new SkewTransform(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestValueWith(new SkewTransform(Const2D.epsilon, Const2D.epsilon, Const2D.epsilon, Const2D.epsilon));
        }
    }
}