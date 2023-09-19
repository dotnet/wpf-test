// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class IntegerRectTest : CoreGraphicsTest
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
                TestEmpty();
                TestProperties();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor Int32Rect()...");

            TestConstructorWith();

            Log("Testing Constructor Int32Rect( int, int, int, int )...");

            TestConstructorWith(0, 0, 0, 0);
            TestConstructorWith(5, 5, 5, 5);
            TestConstructorWith(-5, -5, -5, -5);
            TestConstructorWith(5, -5, 5, -5);
        }

        private void TestConstructorWith()
        {
            Int32Rect theirAnswer = new Int32Rect();

            if (!theirAnswer.X.Equals(0) || !theirAnswer.Y.Equals(0) || !theirAnswer.Width.Equals(0) || !theirAnswer.Height.Equals(0) || failOnPurpose)
            {
                AddFailure("Constructor Int32Rect() failed");
                Log("***Expected: X = {0}, Y = {1}, Width = {2}, Height = {3}", 0, 0, 0, 0);
                Log("***Actual: X = {0}, Y = {1}, Width = {2}, Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestConstructorWith(int x, int y, int width, int height)
        {
            Int32Rect theirAnswer = new Int32Rect(x, y, width, height);

            if (!theirAnswer.X.Equals(x) || !theirAnswer.Y.Equals(y) || !theirAnswer.Width.Equals(width) || !theirAnswer.Height.Equals(height) || failOnPurpose)
            {
                AddFailure("Constrctor Int32Rect( int, int, int, int ) failed");
                Log("***Expected: X = {0}, Y = {1}, Width = {2}, Height = {3}", x, y, width, height);
                Log("***Actual: X = {0}, Y = {1}, Width = {2}, Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestEmpty()
        {
            Log("Testing Int32Rect.Empty...");

            Int32Rect theirAnswer = Int32Rect.Empty;

            if (!theirAnswer.X.Equals(0) || !theirAnswer.Y.Equals(0) || !theirAnswer.Width.Equals(0) || !theirAnswer.Height.Equals(0) || failOnPurpose)
            {
                AddFailure("Int32Rect.Empty failed");
                Log("***Expected: X = {0}, Y = {1}, Width = {2}, Height = {3}", 0, 0, 0, 0);
                Log("***Actual: X = {0}, Y = {1}, Width = {2}, Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties X, Y, Width, Height...");

            TestIWith(0, "X");
            TestIWith(5, "X");
            TestIWith(-5, "X");
            TestIWith(0, "Y");
            TestIWith(5, "Y");
            TestIWith(-5, "Y");
            TestIWith(0, "Width");
            TestIWith(5, "Width");
            TestIWith(-5, "Width");
            TestIWith(0, "Height");
            TestIWith(5, "Height");
            TestIWith(-5, "Height");
        }

        private void TestIWith(int value, string property)
        {
            Int32Rect integerRect = new Int32Rect();
            int actual = SetIWith(ref integerRect, value, property);
            if (!value.Equals(actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            integerRect = new Int32Rect(5, -5, 5, -5);
            actual = SetIWith(ref integerRect, value, property);
            if (!value.Equals(actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private int SetIWith(ref Int32Rect integerRect, int value, string property)
        {
            switch (property)
            {
                case "X": integerRect.X = value; return integerRect.X;
                case "Y": integerRect.Y = value; return integerRect.Y;
                case "Width": integerRect.Width = value; return integerRect.Width;
                case "Height": integerRect.Height = value; return integerRect.Height;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Int32Rect");
        }

        private void RunTest2()
        {
            // these are P2 tests, overflow, bignumber, exception
            TestConstructor2();
            TestProperties2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor( int, int, int, int )...");

            TestConstructorWith(System.Int32.MinValue, System.Int32.MinValue, System.Int32.MinValue, System.Int32.MinValue);
            TestConstructorWith(System.Int32.MaxValue, System.Int32.MaxValue, System.Int32.MaxValue, System.Int32.MaxValue);
        }

        private void TestProperties2()
        {
            Log("P2 Testing Properties X, Y, Width, Height...");

            TestIWith(System.Int32.MinValue, "X");
            TestIWith(System.Int32.MaxValue, "X");
            TestIWith(System.Int32.MinValue, "Y");
            TestIWith(System.Int32.MaxValue, "Y");
            TestIWith(System.Int32.MinValue, "Width");
            TestIWith(System.Int32.MaxValue, "Width");
            TestIWith(System.Int32.MinValue, "Height");
            TestIWith(System.Int32.MaxValue, "Height");
        }
    }
}