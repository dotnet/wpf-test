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
    public class VectorTest : CoreGraphicsTest
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
                //TestAngleBetween();
                TestCastToPoint();
                TestCastToSize();
                TestConstructor();
                TestCrossProduct();
                TestDeterminant();
                TestDivide();
                TestEquals();
                TestGetHashCode();
                TestLength();
                TestLengthSquared();
                TestMultiply();
                TestNegate();
                TestNormalize();
                TestOpAdd();
                TestOpDivide();
                TestOpEqual();
                TestOpInequality();
                TestOpMultiply();
                TestOpNegate();
                TestOpSubtract();
                TestParse();
                TestProperties();
                TestSubtract();
                TestToString();
            }
        }

        private void TestAdd()
        {
            Log("Testing Vector.Add( Vector, Point )...");

            TestAddWith(new Vector(), new Point());
            TestAddWith(new Vector(11.11, -22.22), new Point());
            TestAddWith(new Vector(), new Point(11.11, -22.22));
            TestAddWith(new Vector(11.11, -22.22), new Point(11.11, -22.22));
            TestAddWith(new Vector(11.11, -22.22), new Point(-11.11, 22.22));


            Log("Testing Vector.Add( Vector, Vector )...");

            TestAddWith(new Vector(), new Vector());
            TestAddWith(new Vector(11.11, -22.22), new Vector());
            TestAddWith(new Vector(), new Vector(11.11, -22.22));
            TestAddWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestAddWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestAddWith(Vector vector1, Point point1)
        {
            Point theirAnswer = Vector.Add(vector1, point1);

            Point myAnswer = new Point(vector1.X + point1.X, vector1.Y + point1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Add( Vector, Point ) failed");
                Log("***Expected: Point = {0}", myAnswer);
                Log("***Actual: Point = {0}", theirAnswer);
            }
        }

        private void TestAddWith(Vector vector1, Vector vector2)
        {
            Vector theirAnswer = Vector.Add(vector1, vector2);

            Vector myAnswer = new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Add( Vector, Vector ) failed");
                Log("***Expected: Vector = {0}", myAnswer);
                Log("***Actual: Vector = {0}", theirAnswer);
            }
        }

        private void TestAngleBetween()
        {
            Log("Testing Vector.AngleBetween( Vector, Vector )...");

            TestAngleBetweenWith(new Vector(), new Vector());
            TestAngleBetweenWith(new Vector(), new Vector(11.11, -22.22));
            TestAngleBetweenWith(new Vector(11.11, -22.22), new Vector());
            TestAngleBetweenWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestAngleBetweenWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
            TestAngleBetweenWith(new Vector(0, 22.22), new Vector(0, -22.22));
        }

        private void TestAngleBetweenWith(Vector vector1, Vector vector2)
        {
            double theirAnswer = Vector.AngleBetween(vector1, vector2);

            double myAnswer = MathEx.AngleBetween(vector1, vector2);

            if (MathEx.NotCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.AngleBetween( Vector, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestCastToPoint()
        {
            Log("Testing explicit operator (Point)vector...");

            TestCastToPointWith(new Vector());
            TestCastToPointWith(new Vector(11.11, -22.22));
            TestCastToPointWith(new Vector(-11.11, 22.22));
        }

        private void TestCastToPointWith(Vector vector1)
        {
            Point theirAnswer = (Point)vector1;

            Point myAnswer = new Point(vector1.X, vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("explicit operator (Point)vector failed");
                Log("***Expected: Point = {0}", myAnswer);
                Log("***Actual: Point = {0}", theirAnswer);
            }
        }

        private void TestCastToSize()
        {
            Log("Testing explicit operator (Size)vector...");

            TestCastToSizeWith(new Vector());
            TestCastToSizeWith(new Vector(11.11, 22.22));
            TestCastToSizeWith(new Vector(11.11, -22.22));
            TestCastToSizeWith(new Vector(-11.11, 22.22));
            TestCastToSizeWith(new Vector(-11.11, -22.22));
        }

        private void TestCastToSizeWith(Vector vector1)
        {
            Size theirAnswer = (Size)vector1;

            Size myAnswer = new Size(Math.Abs(vector1.X), Math.Abs(vector1.Y));

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("explicit operator (Size)vector failed");
                Log("***Expected: Size = {0}", myAnswer);
                Log("***Actual: Size = {0}", theirAnswer);
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor Vector()...");

            TestConstructorWith();

            Log("Testing Constructor Vector( double, double )...");

            TestConstructorWith(0, 0);
            TestConstructorWith(11.11, -22.22);
            TestConstructorWith(-11.11, 22.22);
        }

        private void TestConstructorWith()
        {
            Vector theirAnswer = new Vector();

            if (!MathEx.Equals(theirAnswer.X, 0) || !MathEx.Equals(theirAnswer.Y, 0) || failOnPurpose)
            {
                AddFailure("Constructor Vector() failed");
                Log("***Expected: X = {0}, Y = {1}", 0, 0);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestConstructorWith(double x, double y)
        {
            Vector theirAnswer = new Vector(x, y);

            if (!MathEx.Equals(theirAnswer.X, x) || !MathEx.Equals(theirAnswer.Y, y) || failOnPurpose)
            {
                AddFailure("Constrctor Vector( double, double ) failed");
                Log("***Expected: X = {0}, Y = {1}", x, y);
                Log("***Actual: X = {0}, Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestCrossProduct()
        {
            Log("Testing Vector.CrossProduct( Vector, Vector )...");

            TestCrossProductWith(new Vector(), new Vector());
            TestCrossProductWith(new Vector(11.11, -22.22), new Vector());
            TestCrossProductWith(new Vector(), new Vector(11.11, -22.22));
            TestCrossProductWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestCrossProductWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestCrossProductWith(Vector vector1, Vector vector2)
        {
            double theirAnswer = Vector.CrossProduct(vector1, vector2);

            double myAnswer = vector1.X * vector2.Y - vector1.Y * vector2.X;

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.CrossProduct( Vector, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestDeterminant()
        {
            Log("Testing Vector.Determinant( Vector, Vector )...");

            TestDeterminantWith(new Vector(), new Vector());
            TestDeterminantWith(new Vector(11.11, -22.22), new Vector());
            TestDeterminantWith(new Vector(), new Vector(11.11, -22.22));
            TestDeterminantWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestDeterminantWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestDeterminantWith(Vector vector1, Vector vector2)
        {
            double theirAnswer = Vector.Determinant(vector1, vector2);

            double myAnswer = vector1.X * vector2.Y - vector1.Y * vector2.X;

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Determinant( Vector, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestDivide()
        {
            Log("Testing Vector.Divide( Vector, double )...");

            TestDivideWith(new Vector(), 0);
            TestDivideWith(new Vector(), 11.11);
            TestDivideWith(new Vector(), -22.22);
            TestDivideWith(new Vector(11.11, -22.22), 0);
            TestDivideWith(new Vector(11.11, -22.22), 11.11);
            TestDivideWith(new Vector(11.11, -22.22), -22.22);
            TestDivideWith(new Vector(-11.11, 22.22), 0);
            TestDivideWith(new Vector(-11.11, 22.22), 11.11);
            TestDivideWith(new Vector(-11.11, 22.22), -22.22);
        }

        private void TestDivideWith(Vector vector1, double scalar)
        {
            Vector theirAnswer = Vector.Divide(vector1, scalar);

            Vector myAnswer = new Vector(vector1.X / scalar, vector1.Y / scalar);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Divide( Vector, double ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Expected: {0}", theirAnswer);
            }
        }

        private void TestEquals()
        {
            Log("Testing Vector.Equals( Vector, Vector )...");

            TestEqualsWith(new Vector(), new Vector());
            TestEqualsWith(new Vector(), new Vector(11.11, -22.22));
            TestEqualsWith(new Vector(11.11, -22.22), new Vector());
            TestEqualsWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestEqualsWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));

            Log("Testing Equals( object )...");

            TestEqualsWith(new Vector(), null);
            TestEqualsWith(new Vector(), true);
            TestEqualsWith(new Vector(), new Point());

            object obj1 = new Vector(11.11, -22.22);
            TestEqualsWith(new Vector(11.11, -22.22), obj1);
        }

        private void TestEqualsWith(Vector vector1, Vector vector2)
        {
            bool theirAnswer = Vector.Equals(vector1, vector2);

            bool myAnswer = MathEx.Equals(vector1, vector2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Equals( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestEqualsWith(Vector vector1, object obj1)
        {
            bool theirAnswer = vector1.Equals(obj1);

            bool myAnswer = true;
            if (obj1 == null || !(obj1 is Vector))
            {
                myAnswer = false;
            }
            else
            {
                Vector vector2 = (Vector)obj1;
                myAnswer = MathEx.Equals(vector1, vector2);
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

            TestGetHashCodeWith(new Vector());
            TestGetHashCodeWith(new Vector(11.11, -22.22));
            TestGetHashCodeWith(new Vector(-11.11, 22.22));
        }

        private void TestGetHashCodeWith(Vector vector1)
        {
            int theirAnswer = vector1.GetHashCode();

            int myAnswer = vector1.X.GetHashCode() ^ vector1.Y.GetHashCode();

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("GetHashCode() failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestLength()
        {
            Log("Testing Length...");

            TestLengthWith(new Vector());
            TestLengthWith(new Vector(Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            TestLengthWith(new Vector(11.11, -22.22));
            TestLengthWith(new Vector(-11.11, 22.22));
        }

        private void TestLengthWith(Vector vector1)
        {
            double theirAnswer = vector1.Length;

            double myAnswer = MathEx.Length(vector1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Length failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestLengthSquared()
        {
            Log("Testing LengthSquared...");

            TestLengthSquaredWith(new Vector());
            TestLengthSquaredWith(new Vector(Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            TestLengthSquaredWith(new Vector(11.11, -22.22));
            TestLengthSquaredWith(new Vector(-11.11, 22.22));
        }

        private void TestLengthSquaredWith(Vector vector1)
        {
            double theirAnswer = vector1.LengthSquared;

            double myAnswer = MathEx.LengthSquared(vector1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Length failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiply()
        {
            Log("Testing Vector.Multiply( double, Vector )...");

            TestMultiplyWith(0, new Vector());
            TestMultiplyWith(11.11, new Vector());
            TestMultiplyWith(-22.22, new Vector());
            TestMultiplyWith(0, new Vector(11.11, -22.22));
            TestMultiplyWith(11.11, new Vector(11.11, -22.22));
            TestMultiplyWith(-22.22, new Vector(11.11, -22.22));
            TestMultiplyWith(0, new Vector(-11.11, 22.22));
            TestMultiplyWith(11.11, new Vector(-11.11, 22.22));
            TestMultiplyWith(-22.22, new Vector(-11.11, 22.22));

            Log("Testing Vector.Multiply( Vector, double )...");

            TestMultiplyWith(new Vector(), 0);
            TestMultiplyWith(new Vector(), 11.11);
            TestMultiplyWith(new Vector(), -22.22);
            TestMultiplyWith(new Vector(11.11, -22.22), 0);
            TestMultiplyWith(new Vector(11.11, -22.22), 11.11);
            TestMultiplyWith(new Vector(11.11, -22.22), -22.22);
            TestMultiplyWith(new Vector(-11.111, 22.22), 0);
            TestMultiplyWith(new Vector(-11.111, 22.22), 11.11);
            TestMultiplyWith(new Vector(-11.111, 22.22), -22.22);

            Log("Testing Vector.Multiply( Vector, Matrix )...");

            TestMultiplyWith(new Vector(), Const2D.typeIdentity);
            TestMultiplyWith(new Vector(), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestMultiplyWith(new Vector(), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Vector(), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Vector(), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestMultiplyWith(new Vector(11.11, -22.22), Const2D.typeIdentity);
            TestMultiplyWith(new Vector(11.11, -22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));

            Log("Testing Vector.Multiply( Vector, Vector )...");

            TestMultiplyWith(new Vector(), new Vector());
            TestMultiplyWith(new Vector(), new Vector(11.11, -22.22));
            TestMultiplyWith(new Vector(11.11, -22.22), new Vector());
            TestMultiplyWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestMultiplyWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestMultiplyWith(double scalar, Vector vector1)
        {
            Vector theirAnswer = Vector.Multiply(scalar, vector1);

            Vector myAnswer = new Vector(scalar * vector1.X, scalar * vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Multiply( double, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiplyWith(Vector vector1, double scalar)
        {
            Vector theirAnswer = Vector.Multiply(vector1, scalar);

            Vector myAnswer = new Vector(vector1.X * scalar, vector1.Y * scalar);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Multiply( Vector, double ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiplyWith(Vector vector1, Matrix matrix1)
        {
            Vector theirAnswer = Vector.Multiply(vector1, matrix1);

            Vector myAnswer = MatrixUtils.Transform(vector1, matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Multiply( Vector, Matrix ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestMultiplyWith(Vector vector1, Vector vector2)
        {
            double theirAnswer = Vector.Multiply(vector1, vector2);

            double myAnswer = MathEx.DotProduct(vector1, vector2);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Multiply( Vector, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestNegate()
        {
            Log("Testing Negate()...");

            TestNegateWith(new Vector());
            TestNegateWith(new Vector(11.11, -22.22));
            TestNegateWith(new Vector(-11.11, 22.22));
        }

        private void TestNegateWith(Vector vector1)
        {
            Vector theirAnswer = vector1;
            theirAnswer.Negate();

            Vector myAnswer = new Vector(-vector1.X, -vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Negate() failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestNormalize()
        {
            Log("Testing Normalize()...");

            TestNormalizeWith(new Vector());
            TestNormalizeWith(new Vector(1, 1));
            TestNormalizeWith(new Vector(Math.Sqrt(2) / 2, Math.Sqrt(2) / 2));
            TestNormalizeWith(new Vector(11.11, -22.22));
            TestNormalizeWith(new Vector(-11.11, 22.22));
        }


        private void TestNormalizeWith(Vector vector1)
        {
            Vector theirAnswer = vector1;
            theirAnswer.Normalize();

            Vector myAnswer = MathEx.Normalize(vector1);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Normalize() failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpAdd()
        {
            Log("Testing + Operator( Vector, Point )...");

            TestOpAddWith(new Vector(), new Point());
            TestOpAddWith(new Vector(11.11, -22.22), new Point());
            TestOpAddWith(new Vector(), new Point(11.11, -22.22));
            TestOpAddWith(new Vector(11.11, -22.22), new Point(11.11, -22.22));
            TestOpAddWith(new Vector(11.11, -22.22), new Point(-11.11, 22.22));

            Log("Testing + Operator( Vector, Vector )...");

            TestOpAddWith(new Vector(), new Vector());
            TestOpAddWith(new Vector(11.11, -22.22), new Vector());
            TestOpAddWith(new Vector(), new Vector(11.11, -22.22));
            TestOpAddWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpAddWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpAddWith(Vector vector1, Point point1)
        {
            Point theirAnswer = vector1 + point1;

            Point myAnswer = new Point(vector1.X + point1.X, vector1.Y + point1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("+ Operator( Vector, Point ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpAddWith(Vector vector1, Vector vector2)
        {
            Vector theirAnswer = vector1 + vector2;

            Vector myAnswer = new Vector(vector1.X + vector2.X, vector1.Y + vector2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("+ Operator( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpDivide()
        {
            Log("Testing / Operator( Veoctor, double )...");

            TestOpDivideWith(new Vector(), 0);
            TestOpDivideWith(new Vector(), 11.11);
            TestOpDivideWith(new Vector(), -22.22);
            TestOpDivideWith(new Vector(11.11, -22.22), 0);
            TestOpDivideWith(new Vector(11.11, -22.22), 11.11);
            TestOpDivideWith(new Vector(11.11, -22.22), -22.22);
            TestOpDivideWith(new Vector(-11.11, 22.22), 0);
            TestOpDivideWith(new Vector(-11.11, 22.22), 11.11);
            TestOpDivideWith(new Vector(-11.11, 22.22), -22.22);
        }

        private void TestOpDivideWith(Vector vector1, double scalar)
        {
            Vector theirAnswer = vector1 / scalar;

            Vector myAnswer = new Vector(vector1.X / scalar, vector1.Y / scalar);

            if (!MathEx.AreCloseEnough(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("/ Operator( Vector, double ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpEqual()
        {
            Log("Testing == Operator( Vector, Vector )...");

            TestOpEqualWith(new Vector(), new Vector());
            TestOpEqualWith(new Vector(), new Vector(11.11, -22.22));
            TestOpEqualWith(new Vector(11.11, -22.22), new Vector());
            TestOpEqualWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpEqualWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpEqualWith(Vector vector1, Vector vector2)
        {
            bool theirAnswer = (vector1 == vector2);

            bool myAnswer = MathEx.Equals(vector1, vector2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("== Operator( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpInequality()
        {
            Log("Testing != Operator( Vector, Vector )...");

            TestOpInequalityWith(new Vector(), new Vector());
            TestOpInequalityWith(new Vector(), new Vector(11.11, -22.22));
            TestOpInequalityWith(new Vector(11.11, -22.22), new Vector());
            TestOpInequalityWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpInequalityWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpInequalityWith(Vector vector1, Vector vector2)
        {
            bool theirAnswer = (vector1 != vector2);

            bool myAnswer = !MathEx.Equals(vector1, vector2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("!= Operator( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiply()
        {
            Log("Testing * Operator( double, Vector )...");

            TestOpMultiplyWith(0, new Vector());
            TestOpMultiplyWith(11.11, new Vector());
            TestOpMultiplyWith(-22.22, new Vector());
            TestOpMultiplyWith(0, new Vector(11.11, -22.22));
            TestOpMultiplyWith(11.11, new Vector(11.11, -22.22));
            TestOpMultiplyWith(-22.22, new Vector(11.11, -22.22));
            TestOpMultiplyWith(0, new Vector(-11.11, 22.22));
            TestOpMultiplyWith(11.11, new Vector(-11.11, 22.22));
            TestOpMultiplyWith(-22.22, new Vector(-11.11, 22.22));

            Log("Testing * Operator( Vector, double )...");

            TestOpMultiplyWith(new Vector(), 0);
            TestOpMultiplyWith(new Vector(), 11.11);
            TestOpMultiplyWith(new Vector(), -22.22);
            TestOpMultiplyWith(new Vector(11.11, -22.22), 0);
            TestOpMultiplyWith(new Vector(11.11, -22.22), 11.11);
            TestOpMultiplyWith(new Vector(11.11, -22.22), -22.22);
            TestOpMultiplyWith(new Vector(-11.111, 22.22), 0);
            TestOpMultiplyWith(new Vector(-11.111, 22.22), 11.11);
            TestOpMultiplyWith(new Vector(-11.111, 22.22), -22.22);

            Log("Testing * Operator( Vector, Matrix )...");

            TestOpMultiplyWith(new Vector(), Const2D.typeIdentity);
            TestOpMultiplyWith(new Vector(), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestOpMultiplyWith(new Vector(), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Vector(), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Vector(), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));
            TestOpMultiplyWith(new Vector(11.11, -22.22), Const2D.typeIdentity);
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 0, 0));
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66));

            Log("Testing * Operator( Vector, Vector )...");

            TestOpMultiplyWith(new Vector(), new Vector());
            TestOpMultiplyWith(new Vector(), new Vector(11.11, -22.22));
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Vector());
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpMultiplyWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpMultiplyWith(double scalar, Vector vector1)
        {
            Vector theirAnswer = scalar * vector1;

            Vector myAnswer = new Vector(scalar * vector1.X, scalar * vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( double, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiplyWith(Vector vector1, double scalar)
        {
            Vector theirAnswer = vector1 * scalar;

            Vector myAnswer = new Vector(vector1.X * scalar, vector1.Y * scalar);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( Vector, double ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiplyWith(Vector vector1, Matrix matrix1)
        {
            Vector theirAnswer = vector1 * matrix1;

            Vector myAnswer = MatrixUtils.Transform(vector1, matrix1);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( Vector, Matrix ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpMultiplyWith(Vector vector1, Vector vector2)
        {
            double theirAnswer = vector1 * vector2;

            double myAnswer = MathEx.DotProduct(vector1, vector2);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("* Operator( Vector, Vector ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpNegate()
        {
            Log("Testing - Operator( Vector )...");

            TestOpNegateWith(new Vector());
            TestOpNegateWith(new Vector(11.11, -22.22));
            TestOpNegateWith(new Vector(-11.11, 22.22));
        }

        private void TestOpNegateWith(Vector vector1)
        {
            Vector theirAnswer = -vector1;

            Vector myAnswer = new Vector(-vector1.X, -vector1.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("- Operator( Vector ) failed");
                Log("***Expect: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpSubtract()
        {
            Log("Testing - Operator( Vector, Vector )...");

            TestOpSubtractWith(new Vector(), new Vector());
            TestOpSubtractWith(new Vector(), new Vector(11.11, -22.22));
            TestOpSubtractWith(new Vector(11.11, -22.22), new Vector());
            TestOpSubtractWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestOpSubtractWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestOpSubtractWith(Vector vector1, Vector vector2)
        {
            Vector theirAnswer = vector1 - vector2;

            Vector myAnswer = new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("- Operator( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestParse()
        {
            Log("Testing Vector.Parse...");

            TestParseWith("0 " + _sep + " 0	");
            TestParseWith(" 11.11" + _sep + " 22.22	");
            TestParseWith(" -11.11" + _sep + "     -22.22");
            TestParseWith(" -11.11E+1" + _sep + "     -22.22E5");
            TestParseWith(" -11.11E-1" + _sep + "     -22.22e-5");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector v1 = Vector.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector v2 = StringConverter.ToVector(invariant);

            if (!MathEx.Equals(v1, v2) || failOnPurpose)
            {
                AddFailure("Vector.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", v2);
                Log("*** Actual   = {0}", v1);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties X, Y...");

            TestVWith(0.0, "X");
            TestVWith(1.1, "X");
            TestVWith(-1.1, "X");
            TestVWith(0.0, "Y");
            TestVWith(1.1, "Y");
            TestVWith(-1.1, "Y");
        }

        private void TestVWith(double value, string property)
        {
            Vector v = new Vector();
            double actual = SetVWith(ref v, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            v = new Vector(1.1, -1.1);
            actual = SetVWith(ref v, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private double SetVWith(ref Vector v, double value, string property)
        {
            switch (property)
            {
                case "X": v.X = value; return v.X;
                case "Y": v.Y = value; return v.Y;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Vector");
        }

        private void TestSubtract()
        {
            Log("Testing Vector.Subtract( Vector, Vector )...");

            TestSubtractWith(new Vector(), new Vector());
            TestSubtractWith(new Vector(), new Vector(11.11, -22.22));
            TestSubtractWith(new Vector(11.11, -22.22), new Vector());
            TestSubtractWith(new Vector(11.11, -22.22), new Vector(11.11, -22.22));
            TestSubtractWith(new Vector(11.11, -22.22), new Vector(-11.11, 22.22));
        }

        private void TestSubtractWith(Vector vector1, Vector vector2)
        {
            Vector theirAnswer = Vector.Subtract(vector1, vector2);

            Vector myAnswer = new Vector(vector1.X - vector2.X, vector1.Y - vector2.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Vector.Subtract( Vector, Vector ) failed");
                Log("*** Expected: {0}", myAnswer);
                Log("*** Actual: {0}", theirAnswer);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(new Vector());
            TestToStringWith(new Vector(11.11, -22.22));
            TestToStringWith(new Vector(-11.11, 22.22));
        }

        private void TestToStringWith(Vector vector1)
        {
            string theirAnswer = vector1.ToString();

            // Don't want these to be affected by locale yet
            string myX = vector1.X.ToString(CultureInfo.InvariantCulture);
            string myY = vector1.Y.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myX + _sep + myY;

            myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = vector1.ToString(CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)vector1).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet

            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;

            myX = vector1.X.ToString("N", numberFormat);
            myY = vector1.Y.ToString("N", numberFormat);

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
            //TestAngleBetween2();
            TestCastToPoint2();
            TestCastToSize2();
            TestConstructor2();
            TestCrossProduct2();
            TestDeterminant2();
            TestDivide2();
            TestEquals2();
            TestGetHashCode2();
            TestLength2();
            TestLengthSquared2();
            TestNegate2();
            TestNormalize2();
            TestOpAdd2();
            TestOpDivide2();
            TestOpEqual2();
            TestOpInequality2();
            TestOpNegate2();
            TestOpSubtract2();
            TestParse2();
            TestProperties2();
            TestSubtract2();
            TestToString2();
        }

        private void TestAdd2()
        {
            Log("P2 Testing Vector.Add( Vector, Point )...");

            TestAddWith(new Vector(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestAddWith(new Vector(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Vector(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestAddWith(new Vector(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Vector(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestAddWith(new Vector(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestAddWith(new Vector(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
            TestAddWith(new Vector(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));


            Log("P2 Testing Vector.Add( Vector, Vector )...");

            TestAddWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestAddWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestAddWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestAddWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestAddWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestAddWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestAddWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestAngleBetween2()
        {
            Log("P2 Testing Vector.AngleBetween( Vector, Vector )...");

            TestAngleBetweenWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.min));
            TestAngleBetweenWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.max, Const2D.max));
            TestAngleBetweenWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.negInf, Const2D.negInf));
            TestAngleBetweenWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.posInf));
            TestAngleBetweenWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestCastToPoint2()
        {
            Log("P2 Testing explicit operator (Point)vector...");

            TestCastToPointWith(new Vector(Const2D.min, Const2D.min));
            TestCastToPointWith(new Vector(Const2D.max, Const2D.max));
            TestCastToPointWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestCastToPointWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestCastToPointWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestCastToSize2()
        {
            Log("P2 Testing explicit operator (Size)vector...");

            TestCastToSizeWith(new Vector(Const2D.min, Const2D.min));
            TestCastToSizeWith(new Vector(Const2D.max, Const2D.max));
            TestCastToSizeWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestCastToSizeWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestCastToSizeWith(new Vector(Const2D.nan, Const2D.nan));
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

        private void TestCrossProduct2()
        {
            Log("P2 Testing Vector.CrossProduct( Vector, Vector )...");

            TestCrossProductWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.max, Const2D.min));
            TestCrossProductWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.posInf, Const2D.negInf));
            TestCrossProductWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.max, Const2D.nan));
            TestCrossProductWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.posInf, Const2D.negInf));
            TestCrossProductWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.negInf, Const2D.nan));
            TestCrossProductWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.nan, Const2D.posInf));
            TestCrossProductWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.nan, Const2D.posInf));
            TestCrossProductWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestDeterminant2()
        {
            Log("P2 Testing Vector.Determinant( Vector, Vector )...");

            TestDeterminantWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.max, Const2D.min));
            TestDeterminantWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.posInf, Const2D.negInf));
            TestDeterminantWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.max, Const2D.nan));
            TestDeterminantWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.posInf, Const2D.negInf));
            TestDeterminantWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.negInf, Const2D.nan));
            TestDeterminantWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.nan, Const2D.posInf));
            TestDeterminantWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.nan, Const2D.posInf));
            TestDeterminantWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestDivide2()
        {
            Log("P2 Testing Vector.Divide( Vector, double )...");

            TestDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.min);
            TestDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.min);
            TestDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.min);
            TestDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.max);
            TestDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.max);
            TestDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.max);
            TestDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.negInf);
            TestDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.negInf);
            TestDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.negInf);
            TestDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.posInf);
            TestDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.posInf);
            TestDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.posInf);
            TestDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.nan);
            TestDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.nan);
            TestDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.nan);
        }

        private void TestEquals2()
        {
            Log("P2 Testing Vector.Equals( Vector, Vector )...");

            TestEqualsWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestEqualsWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestEqualsWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestEqualsWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestEqualsWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestEqualsWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestEqualsWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestEqualsWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestGetHashCode2()
        {
            Log("P2 Testing GetHashCode()...");

            TestGetHashCodeWith(new Vector(Const2D.min, Const2D.max));
            TestGetHashCodeWith(new Vector(Const2D.negInf, Const2D.posInf));
            TestGetHashCodeWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestLength2()
        {
            Log("P2 Testing Length...");

            TestLengthWith(new Vector(Const2D.min, Const2D.min));
            TestLengthWith(new Vector(Const2D.max, Const2D.max));
            TestLengthWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestLengthWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestLengthWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestLengthSquared2()
        {
            Log("P2 Testing LengthSquared...");

            TestLengthSquaredWith(new Vector(Const2D.min, Const2D.min));
            TestLengthSquaredWith(new Vector(Const2D.max, Const2D.max));
            TestLengthSquaredWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestLengthSquaredWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestLengthSquaredWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestNegate2()
        {
            Log("P2 Testing Negate()...");

            TestNegateWith(new Vector(Const2D.min, Const2D.min));
            TestNegateWith(new Vector(Const2D.max, Const2D.max));
            TestNegateWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestNegateWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestNegateWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestNormalize2()
        {
            Log("Testing Normalize()...");

            TestNormalizeWith(new Vector(Const2D.min, Const2D.min));
            TestNormalizeWith(new Vector(Const2D.max, Const2D.max));
            TestNormalizeWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestNormalizeWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestNormalizeWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestOpAdd2()
        {
            Log("P2 Testing + Operator( Vector, Point )...");

            TestOpAddWith(new Vector(Const2D.min, Const2D.min), new Point(Const2D.min, Const2D.max));
            TestOpAddWith(new Vector(Const2D.min, Const2D.min), new Point(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Vector(Const2D.min, Const2D.max), new Point(Const2D.nan, Const2D.max));
            TestOpAddWith(new Vector(Const2D.max, Const2D.max), new Point(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Vector(Const2D.max, Const2D.negInf), new Point(Const2D.nan, Const2D.negInf));
            TestOpAddWith(new Vector(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Vector(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Vector(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));

            Log("P2 Testing + Operator( Vector, Vector )...");

            TestOpAddWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestOpAddWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestOpAddWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpAddWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestOpAddWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpAddWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestOpDivide2()
        {
            Log("P2 Testing / Operator( Veoctor, double )...");

            TestOpDivideWith(new Vector(), 0);
            TestOpDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.min);
            TestOpDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.min);
            TestOpDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.min);
            TestOpDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.max);
            TestOpDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.max);
            TestOpDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.max);
            TestOpDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.negInf);
            TestOpDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.negInf);
            TestOpDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.negInf);
            TestOpDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.posInf);
            TestOpDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.posInf);
            TestOpDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.posInf);
            TestOpDivideWith(new Vector(Const2D.min, Const2D.max), Const2D.nan);
            TestOpDivideWith(new Vector(Const2D.negInf, Const2D.posInf), Const2D.nan);
            TestOpDivideWith(new Vector(Const2D.nan, Const2D.nan), Const2D.nan);
        }

        private void TestOpEqual2()
        {
            Log("P2 Testing == Operator( Vector, Vector )...");

            TestOpEqualWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestOpEqualWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpEqualWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestOpEqualWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpEqualWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestOpEqualWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpEqualWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
        }

        private void TestOpInequality2()
        {
            Log("P2 Testing != Operator( Vector, Vector )...");

            TestOpInequalityWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestOpInequalityWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpInequalityWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestOpInequalityWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpInequalityWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestOpInequalityWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpInequalityWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
        }

        private void TestOpNegate2()
        {
            Log("P2 Testing - Operator( Vector )...");

            TestOpNegateWith(new Vector(Const2D.min, Const2D.min));
            TestOpNegateWith(new Vector(Const2D.max, Const2D.max));
            TestOpNegateWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestOpNegateWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestOpNegateWith(new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestOpSubtract2()
        {
            Log("P2 Testing - Operator( Vector, Vector )...");

            TestOpSubtractWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestOpSubtractWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpSubtractWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestOpSubtractWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestOpSubtractWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestOpSubtractWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpSubtractWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestOpSubtractWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestParse2()
        {
            Log("P2 Testing Vector.Parse...");

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
            Vector vector = Vector.Parse(global);
        }

        private void ParseTooFewParams()
        {
            string s = "11.11";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector vector = Vector.Parse(global);
        }

        private void ParseTooManyParams()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "1";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector vector = Vector.Parse(global);
        }

        private void ParseWrongFormat()
        {
            string s = "11.11" + _sep + "a";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Vector vector = Vector.Parse(global);
        }

        #endregion

        private void TestProperties2()
        {
            Log("P2 Testing Properties X, Y...");

            TestVWith(Const2D.min, "X");
            TestVWith(Const2D.max, "X");
            TestVWith(Const2D.negInf, "X");
            TestVWith(Const2D.posInf, "X");
            TestVWith(Const2D.nan, "X");
            TestVWith(Const2D.min, "Y");
            TestVWith(Const2D.max, "Y");
            TestVWith(Const2D.negInf, "Y");
            TestVWith(Const2D.posInf, "Y");
            TestVWith(Const2D.nan, "Y");
        }

        private void TestSubtract2()
        {
            Log("P2 Testing Vector.Subtract( Vector, Vector )...");

            TestSubtractWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.min, Const2D.max));
            TestSubtractWith(new Vector(Const2D.min, Const2D.min), new Vector(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Vector(Const2D.min, Const2D.max), new Vector(Const2D.nan, Const2D.max));
            TestSubtractWith(new Vector(Const2D.max, Const2D.max), new Vector(Const2D.negInf, Const2D.posInf));
            TestSubtractWith(new Vector(Const2D.max, Const2D.negInf), new Vector(Const2D.nan, Const2D.negInf));
            TestSubtractWith(new Vector(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Vector(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.nan));
            TestSubtractWith(new Vector(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
        }

        private void TestToString2()
        {
            Log("P2 Testing ToString...");

            TestToStringWith(new Vector(Const2D.min, Const2D.min));
            TestToStringWith(new Vector(Const2D.max, Const2D.max));
            TestToStringWith(new Vector(Const2D.negInf, Const2D.negInf));
            TestToStringWith(new Vector(Const2D.posInf, Const2D.posInf));
            TestToStringWith(new Vector(Const2D.nan, Const2D.nan));
        }
    }
}