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
    public class PointTest : CoreGraphicsTest
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
                TestAdd();
                TestCastToSize();
                TestCastToVector();
                TestConstructor();
                TestEquals();
                TestGetHashCode();
                TestMultiply();
                TestOffset();
                TestOpAdd();
                TestOpEqual();
                TestOpInequality();
                TestOpMultiply();
                TestOpSubtract();
                TestParse();
                TestProperties();
                TestSubtract();
                TestToString();
            }
        }

        private void TestAdd()
        {
            Log("Testing Point.Add( Point, Vector )...");

            TestAddWith(new Point(), new Vector());
            TestAddWith(new Point(11.11, -22.22), new Vector());
            TestAddWith(new Point(), new Vector(11.11, -22.22));
            TestAddWith(new Point(11.11, -22.22), new Vector(11.11, -22.22));
            TestAddWith(new Point(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestAddWith(Point point1, Vector vector1)
        {
            Point theirAnswer = Point.Add(point1, vector1);

            Point myAnswer = new Point(point1.X + vector1.X, point1.Y + vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Point.Add( Point, Vector ) failed");
                Log("***Expected: Point = {0}", myAnswer);
                Log("***Actual: Point = {0}", theirAnswer);
            }
        }

        private void TestCastToSize()
        {
            Log("Testing explicit operator (Size)point...");

            TestCastToSizeWith(new Point());
            TestCastToSizeWith(new Point(11.11, 22.22));
            TestCastToSizeWith(new Point(11.11, -22.22));
            TestCastToSizeWith(new Point(-11.11, 22.22));
            TestCastToSizeWith(new Point(-11.11, -22.22));
        }

        private void TestCastToSizeWith(Point point1)
        {
            Size theirAnswer = (Size)point1;

            Size myAnswer = new Size(Math.Abs(point1.X), Math.Abs(point1.Y));

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("explicit operator (Size)point failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestCastToVector()
        {
            Log("Testing explicit operator (Vector)point...");

            TestCastToVectorWith(new Point());
            TestCastToVectorWith(new Point(11.11, 22.22));
            TestCastToVectorWith(new Point(-11.11, -22.22));
        }

        private void TestCastToVectorWith(Point point1)
        {
            Vector theirAnswer = (Vector)point1;

            Vector myAnswer = new Vector(point1.X, point1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("explicit operator (Vector)point failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor Point()...");

            TestConstructorWith();

            Log("Testing Constructor Point( double, double )...");

            TestConstructorWith(0, 0);
            TestConstructorWith(11.11, -22.22);
            TestConstructorWith(-11.11, 22.22);
        }

        private void TestConstructorWith()
        {
            Point theirAnswer = new Point();

            if (!MathEx.Equals(theirAnswer.X, 0) || !MathEx.Equals(theirAnswer.Y, 0) || failOnPurpose)
            {
                AddFailure("Constructor Point() failed");
                Log("***Expected: X = {0}, Y = {1}", 0, 0);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestConstructorWith(double x, double y)
        {
            Point theirAnswer = new Point(x, y);

            if (!MathEx.Equals(theirAnswer.X, x) || !MathEx.Equals(theirAnswer.Y, y) || failOnPurpose)
            {
                AddFailure("Constrctor Point( double, double ) failed");
                Log("***Expected: X = {0}, Y = {1}", x, y);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestEquals()
        {
            Log("Testing Point.Equals( Point, Point )...");

            TestEqualsWith(new Point(), new Point());
            TestEqualsWith(new Point(), new Point(11.11, -22.22));
            TestEqualsWith(new Point(11.11, -22.22), new Point());
            TestEqualsWith(new Point(11.11, -22.22), new Point(11.11, -22.22));
            TestEqualsWith(new Point(11.11, -22.22), new Point(-11.11, 22.22));

            Log("Testing Equals( object )...");

            TestEqualsWith(new Point(), null);
            TestEqualsWith(new Point(), true);
            TestEqualsWith(new Point(), new Vector());

            object obj1 = new Point(11.11, -22.22);
            TestEqualsWith(new Point(11.11, -22.22), obj1);
        }

        private void TestEqualsWith(Point point1, Point point2)
        {
            bool theirAnswer = Point.Equals(point1, point2);

            bool myAnswer = MathEx.Equals(point1, point2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Point.Equals( Point, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestEqualsWith(Point point1, object obj1)
        {
            bool theirAnswer = point1.Equals(obj1);

            bool myAnswer = true;
            if (obj1 == null || !(obj1 is Point))
            {
                myAnswer = false;
            }
            else
            {
                Point point2 = (Point)obj1;
                myAnswer = MathEx.Equals(point1, point2);
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

            TestGetHashCodeWith(new Point());
            TestGetHashCodeWith(new Point(11.11, -22.22));
            TestGetHashCodeWith(new Point(-11.11, 22.22));
        }

        private void TestGetHashCodeWith(Point point1)
        {
            int theirAnswer = point1.GetHashCode();

            int myAnswer = point1.X.GetHashCode() ^ point1.Y.GetHashCode();

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("GetHashCode() failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Point.Multiply( Point, Matrix )...");

            TestMultiplyWith(new Point(), Const2D.typeIdentity);
            TestMultiplyWith(new Point(), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestMultiplyWith(new Point(), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Point(), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Point(), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Point(11.11, -22.22), Const2D.typeIdentity);
            TestMultiplyWith(new Point(11.11, -22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Point(-11.11, 22.22), Const2D.typeIdentity);
            TestMultiplyWith(new Point(-11.11, 22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestMultiplyWith(Point point1, Matrix matrix1)
        {
            Point theirAnswer = Point.Multiply(point1, matrix1);

            Point myAnswer = MatrixUtils.Transform(point1, matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Point.Multiply( Point, Matrix ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOffset()
        {
            Log("Testing Offset( double, double )...");

            TestOffsetWith(new Point(), 0, 0);
            TestOffsetWith(new Point(), 11.11, 0);
            TestOffsetWith(new Point(), 0, -22.22);
            TestOffsetWith(new Point(), 11.11, -22.22);
            TestOffsetWith(new Point(11.11, -22.22), 0, 0);
            TestOffsetWith(new Point(11.11, -22.22), 11.11, 0);
            TestOffsetWith(new Point(11.11, -22.22), 0, -22.22);
            TestOffsetWith(new Point(11.11, -22.22), 11.11, -22.22);
            TestOffsetWith(new Point(11.11, -22.22), -11.11, 22.22);
        }

        private void TestOffsetWith(Point point1, double offsetX, double offsetY)
        {
            Point theirAnswer = point1;
            theirAnswer.Offset(offsetX, offsetY);

            Point myAnswer = new Point(point1.X + offsetX, point1.Y + offsetY);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Offset( double, double ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpAdd()
        {
            Log("Testing + Operator( Point, Vector )...");

            TestOpAddWith(new Point(), new Vector());
            TestOpAddWith(new Point(11.11, -22.22), new Vector());
            TestOpAddWith(new Point(), new Vector(11.11, -22.22));
            TestOpAddWith(new Point(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpAddWith(new Point(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpAddWith(Point point1, Vector vector1)
        {
            Point theirAnswer = point1 + vector1;

            Point myAnswer = new Point(point1.X + vector1.X, point1.Y + vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("+ Operator( Point, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpEqual()
        {
            Log("Testing == Operator( Point, Point )...");

            TestOpEqualWith(new Point(), new Point());
            TestOpEqualWith(new Point(), new Point(11.11, -22.22));
            TestOpEqualWith(new Point(11.11, -22.22), new Point());
            TestOpEqualWith(new Point(11.11, -22.22), new Point(11.11, -22.22));
            TestOpEqualWith(new Point(11.11, -22.22), new Point(-11.11, 22.22));
        }

        private void TestOpEqualWith(Point point1, Point point2)
        {
            bool theirAnswer = (point1 == point2);

            bool myAnswer = MathEx.Equals(point1, point2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("== Operator( Point, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpInequality()
        {
            Log("Testing != Operator( Point, Point )...");

            TestOpInequalityWith(new Point(), new Point());
            TestOpInequalityWith(new Point(), new Point(11.11, -22.22));
            TestOpInequalityWith(new Point(11.11, -22.22), new Point());
            TestOpInequalityWith(new Point(11.11, -22.22), new Point(11.11, -22.22));
            TestOpInequalityWith(new Point(11.11, -22.22), new Point(-11.11, 22.22));
        }

        private void TestOpInequalityWith(Point point1, Point point2)
        {
            bool theirAnswer = (point1 != point2);

            bool myAnswer = !MathEx.Equals(point1, point2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("!= Operator( Point, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiply()
        {
            Log("Testing * Operator( Point, Matrix )...");

            TestOpMultiplyWith(new Point(), Const2D.typeIdentity);
            TestOpMultiplyWith(new Point(), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestOpMultiplyWith(new Point(), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Point(), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Point(), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Point(11.11, -22.22), Const2D.typeIdentity);
            TestOpMultiplyWith(new Point(11.11, -22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestOpMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Point(11.11, -22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Point(-11.11, 22.22), Const2D.typeIdentity);
            TestOpMultiplyWith(new Point(-11.11, 22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestOpMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Point(-11.11, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
        }

        private void TestOpMultiplyWith(Point point1, Matrix matrix1)
        {
            Point theirAnswer = point1 * matrix1;

            Point myAnswer = MatrixUtils.Transform(point1, matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( Point, Matrix ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpSubtract()
        {
            Log("Testing - Operator( Point, Point )...");

            TestOpSubtractWith(new Point(), new Point());
            TestOpSubtractWith(new Point(11.11, -22.22), new Point());
            TestOpSubtractWith(new Point(), new Point(11.11, -22.22));
            TestOpSubtractWith(new Point(11.11, -22.22), new Point(11.11, -22.22));
            TestOpSubtractWith(new Point(11.11, -22.22), new Point(-11.11, 22.22));
        }

        private void TestOpSubtractWith(Point point1, Point point2)
        {
            Vector theirAnswer = point1 - point2;

            Vector myAnswer = new Vector(point1.X - point2.X, point1.Y - point2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("- Operator( Point, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestParse()
        {
            Log("Testing Point.Parse...");

            TestParseWith("0 " + _sep + " 0	");
            TestParseWith(" 11.11" + _sep + " 22.22	");
            TestParseWith(" -11.11" + _sep + "     -22.22");
            TestParseWith(" -11.11E+1" + _sep + "     -22.22E5");
            TestParseWith(" -11.11E-1" + _sep + "     -22.22e-5");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point p1 = Point.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point p2 = StringConverter.ToPoint(invariant);

            if (!MathEx.Equals(p1, p2) || failOnPurpose)
            {
                AddFailure("Point.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", p2);
                Log("*** Actual   = {0}", p1);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties X, Y...");

            TestPWith(0.0, "X");
            TestPWith(1.1, "X");
            TestPWith(-1.1, "X");
            TestPWith(0.0, "Y");
            TestPWith(1.1, "Y");
            TestPWith(-1.1, "Y");
        }

        private void TestPWith(double value, string property)
        {
            Point p = new Point();
            double actual = SetPWith(ref p, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            p = new Point(11.11, -22.22);
            actual = SetPWith(ref p, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private double SetPWith(ref Point p, double value, string property)
        {
            switch (property)
            {
                case "X": p.X = value; return p.X;
                case "Y": p.Y = value; return p.Y;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Point");
        }

        private void TestSubtract()
        {
            Log("Testing Point.Subtract( Point, Point )...");

            TestSubtractWith(new Point(), new Point());
            TestSubtractWith(new Point(11.11, -22.22), new Point());
            TestSubtractWith(new Point(), new Point(11.11, -22.22));
            TestSubtractWith(new Point(11.11, -22.22), new Point(11.11, -22.22));
            TestSubtractWith(new Point(11.11, -22.22), new Point(-11.11, 22.22));

            Log("Testing Point.Subtract( Point, Vector )...");

            TestSubtractWith(new Point(), new Vector());
            TestSubtractWith(new Point(11.11, -22.22), new Vector());
            TestSubtractWith(new Point(), new Vector(11.11, -22.22));
            TestSubtractWith(new Point(11.11, -22.22), new Vector(11.11, -22.22));
            TestSubtractWith(new Point(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestSubtractWith(Point point1, Point point2)
        {
            Vector theirAnswer = Point.Subtract(point1, point2);

            Vector myAnswer = new Vector(point1.X - point2.X, point1.Y - point2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Point.Subtract( Point, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestSubtractWith(Point point1, Vector vector1)
        {
            Point theirAnswer = Point.Subtract(point1, vector1);

            Point myAnswer = new Point(point1.X - vector1.X, point1.Y - vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Point.Subtract( Point, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(new Point());
            TestToStringWith(new Point(11.11, -22.22));
            TestToStringWith(new Point(-11.11, 22.22));
        }

        private void TestToStringWith(Point point1)
        {
            string theirAnswer = point1.ToString();			

            // Don't want these to be affected by locale yet
            string myX = point1.X.ToString(CultureInfo.InvariantCulture);
            string myY = point1.Y.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myX + _sep + myY;

            myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = point1.ToString(CultureInfo.CurrentCulture);
            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)point1).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet

            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;

            myX = point1.X.ToString("N", numberFormat);
            myY = point1.Y.ToString("N", numberFormat);

            // ... Because of this

            myAnswer = myX + _sep + myY;
            myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void RunTest2()
        {
            // these are P2 tests, overflow, bignumber, exception
            TestAdd2();
            TestCastToSize2();
            TestCastToVector2();
            TestConstructor2();
            TestEquals2();
            TestGetHashCode2();
            TestOffset2();
            TestOpAdd2();
            TestOpEqual2();
            TestOpInequality2();
            TestOpSubtract2();
            TestParse2();
            TestProperties2();
            TestSubtract2();
            TestToString2();
        }

        private void TestAdd2()
        {
            Log("P2 Testing Point.Add( Point, Vector )...");

            TestAddWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestAddWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Point(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestAddWith(new Point(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Point(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestAddWith(new Point(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestAddWith(new Point(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestAddWith(new Point(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestCastToSize2()
        {
            Log("P2 Testing explicit operator (Size)point...");

            TestCastToSizeWith(new Point(Const2D.min, Const2D.min));
            TestCastToSizeWith(new Point(Const2D.max, Const2D.max));
            TestCastToSizeWith(new Point(Const2D.negInf, Const2D.negInf));
            TestCastToSizeWith(new Point(Const2D.posInf, Const2D.posInf));
            TestCastToSizeWith(new Point(Const2D.nan, Const2D.nan));
        }

        private void TestCastToVector2()
        {
            Log("P2 Testing explicit operator (Vector)point...");

            TestCastToVectorWith(new Point(Const2D.min, Const2D.min));
            TestCastToVectorWith(new Point(Const2D.max, Const2D.max));
            TestCastToVectorWith(new Point(Const2D.negInf, Const2D.negInf));
            TestCastToVectorWith(new Point(Const2D.posInf, Const2D.posInf));
            TestCastToVectorWith(new Point(Const2D.nan, Const2D.nan));
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Constructor( double, double )...");

            TestConstructorWith(Const2D.min, Const2D.min);
            TestConstructorWith(Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.negInf, Const2D.negInf);
            TestConstructorWith(Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan);
        }

        private void TestEquals2()
        {
            Log("P2 Testing Point.Equals( Point, Point )...");

            TestEqualsWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestEqualsWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestEqualsWith(new Point(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestEqualsWith(new Point(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestEqualsWith(new Point(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestEqualsWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestEqualsWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
            TestEqualsWith(new Point(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));
        }

        private void TestGetHashCode2()
        {
            Log("P2 Testing GetHashCode()...");

            TestGetHashCodeWith(new Point(Const2D.min, Const2D.max));
            TestGetHashCodeWith(new Point(Const2D.negInf, Const2D.posInf));
            TestGetHashCodeWith(new Point(Const2D.nan, Const2D.nan));
        }

        private void TestOffset2()
        {
            Log("P2 Testing Offset( double, double )...");

            TestOffsetWith(new Point(Const2D.min, Const2D.min), Const2D.min, Const2D.max);
            TestOffsetWith(new Point(Const2D.min, Const2D.min), Const2D.negInf, Const2D.posInf);
            TestOffsetWith(new Point(Const2D.min, Const2D.max), Const2D.nan, Const2D.max);
            TestOffsetWith(new Point(Const2D.max, Const2D.max), Const2D.negInf, Const2D.posInf);
            TestOffsetWith(new Point(Const2D.max, Const2D.negInf), Const2D.nan, Const2D.negInf);
            TestOffsetWith(new Point(Const2D.negInf, Const2D.negInf), Const2D.posInf, Const2D.nan);
            TestOffsetWith(new Point(Const2D.posInf, Const2D.posInf), Const2D.posInf, Const2D.nan);
            TestOffsetWith(new Point(Const2D.nan, Const2D.nan), Const2D.nan, Const2D.nan);
        }

        private void TestOpAdd2()
        {
            Log("P2 Testing + Operator( Point, Vector )...");

            TestOpAddWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestOpAddWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Point(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestOpAddWith(new Point(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Point(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestOpAddWith(new Point(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Point(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Point(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestOpEqual2()
        {
            Log("P2 Testing == Operator( Point, Point )...");

            TestOpEqualWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestOpEqualWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestOpEqualWith(new Point(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestOpEqualWith(new Point(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestOpEqualWith(new Point(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestOpEqualWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpEqualWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
        }

        private void TestOpInequality2()
        {
            Log("P2 Testing != Operator( Point, Point )...");

            TestOpInequalityWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestOpInequalityWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestOpInequalityWith(new Point(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestOpInequalityWith(new Point(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestOpInequalityWith(new Point(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestOpInequalityWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpInequalityWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
        }

        private void TestOpSubtract2()
        {
            Log("P2 Testing - Operator( Point, Point )...");

            TestOpSubtractWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestOpSubtractWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestOpSubtractWith(new Point(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestOpSubtractWith(new Point(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestOpSubtractWith(new Point(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestOpSubtractWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpSubtractWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpSubtractWith(new Point(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));
        }

        private void TestParse2()
        {
            Log("P2 Testing Point.Parse...");

            Try(ParseEmptyString, typeof(InvalidOperationException));
            Try(ParseTooFewParams, typeof(InvalidOperationException));
            Try(ParseTooManyParams, typeof(InvalidOperationException));
            Try(ParseWrongFormat, typeof(FormatException));
        }

        #region ExceptionThrowers for Parse

        private void ParseEmptyString()
        {
            string s = "";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point point = Point.Parse(global);
        }

        private void ParseTooFewParams()
        {
            string s = "11.11";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point point = Point.Parse(global);
        }

        private void ParseTooManyParams()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "1";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point point = Point.Parse(global);
        }

        private void ParseWrongFormat()
        {
            string s = "11.11" + _sep + "a";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Point point = Point.Parse(global);
        }

        #endregion

        private void TestProperties2()
        {
            Log("P2 Testing Properties X, Y...");

            TestPWith(Const2D.min, "X");
            TestPWith(Const2D.max, "X");
            TestPWith(Const2D.negInf, "X");
            TestPWith(Const2D.posInf, "X");
            TestPWith(Const2D.nan, "X");
            TestPWith(Const2D.min, "Y");
            TestPWith(Const2D.max, "Y");
            TestPWith(Const2D.negInf, "Y");
            TestPWith(Const2D.posInf, "Y");
            TestPWith(Const2D.nan, "Y");
        }

        private void TestSubtract2()
        {
            Log("P2 Testing Point.Subtract( Point, Point )...");

            TestSubtractWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestSubtractWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Point(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestSubtractWith(new Point(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Point(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestSubtractWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Point(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));

            Log("P2 Testing Point.Subtract( Point, Vector )...");

            TestSubtractWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestSubtractWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Point(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestSubtractWith(new Point(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Point(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestSubtractWith(new Point(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Point(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Point(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestToString2()
        {
            Log("P2 Testing ToString...");

            TestToStringWith(new Point(Const2D.min, Const2D.min));
            TestToStringWith(new Point(Const2D.max, Const2D.max));
            TestToStringWith(new Point(Const2D.negInf, Const2D.negInf));
            TestToStringWith(new Point(Const2D.posInf, Const2D.posInf));
            TestToStringWith(new Point(Const2D.nan, Const2D.nan));
        }
    }
}