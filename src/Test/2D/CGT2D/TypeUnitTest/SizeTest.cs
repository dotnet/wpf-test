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
    public class SizeTest : CoreGraphicsTest
    {
        private char _sep = Const.valueSeparator;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestConstructor();
                TestCastToPoint();
                TestCastToVector();
                TestEmpty();
                TestEquals();
                TestGetHashCode();
                TestHeight();
                TestIsEmpty();
                TestOpInequality();
                TestParse();
                TestToString();
                TestWidth();
            }
            else // priority > 0
            {
                TestConstructor2();
                TestHeight2();
                TestWidth2();
                TestParse2();
            }
        }

        private void TestConstructor()
        {
            TestConstructorWith();
            TestConstructorWith(0, 0);
            TestConstructorWith(1.1, 0.2);
        }

        private void TestConstructorWith()
        {
            Log("Testing Constructor Size()...");

            Size theirAnswer = new Size();

            if (theirAnswer.Width != 0 || theirAnswer.Height != 0 || failOnPurpose)
            {
                AddFailure("Constructor Size() failed");
                Log("*** Expected: Width = {0}, Height = {1}", 0, 0);
                Log("*** Actual: Width = {0}, Height = {1}", theirAnswer.Width, theirAnswer.Height);
            }
        }

        // width and height here must be non-negatives, this is not bad param testings
        private void TestConstructorWith(double width, double height)
        {
            Log("Testing Construtor Size( double, double )...");

            Size theirAnswer = new Size(width, height);

            if (theirAnswer.Width != width || theirAnswer.Height != height || failOnPurpose)
            {
                AddFailure("Constructor Size( double, double ) failed");
                Log("*** Expected: Width = {0}, Height = {1}", width, height);
                Log("*** Actual: Width = {0}, Height = {1}", theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestConstructor2()
        {
            Log("Testing Constructor Size( double, double ) with bad Parameters...");
            Try(CtorNegWidth, typeof(ArgumentException));
            Try(CtorNegHeight, typeof(ArgumentException));
            Try(CtorNegWidthHeight, typeof(ArgumentException));
        }


        #region ExceptionThrowers for Constrictor Size( double, double )

        private void CtorNegWidth()
        {
            Size size = new Size(-1.5, 1);
        }

        private void CtorNegHeight()
        {
            Size size = new Size(1.5, -1.1);
        }

        private void CtorNegWidthHeight()
        {
            Size size = new Size(-1.5, -0.1);
        }

        #endregion

        private void TestCastToPoint()
        {
            TestCastToPointWith(Size.Empty);
            TestCastToPointWith(new Size(0, 0));
            TestCastToPointWith(new Size(1.1, 0.2));
        }

        private void TestCastToPointWith(Size size)
        {
            Log("Testing explicit operator (Point)size...");

            Point myAnswer = new Point(size.Width, size.Height);

            Point theirAnswer = (Point)size;

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("explicit operator (Point)size failed");
                Log("*** Expected: Point.X = {0}, Point.Y = {1}", myAnswer.X, myAnswer.Y);
                Log("*** Actual: Point.X = {0}, Point.Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestCastToVector()
        {
            TestCastToVectorWith(Size.Empty);
            TestCastToVectorWith(new Size(0, 0));
            TestCastToVectorWith(new Size(1.1, 0.2));
        }

        private void TestCastToVectorWith(Size size)
        {
            Log("Testing explicit operator (Vector)size...");

            Vector myAnswer = new Vector(size.Width, size.Height);

            Vector theirAnswer = (Vector)size;

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("explicit operator (Vector)size failed");
                Log("*** Expected: Vector.X = {0}, Vector.Y = {1}", myAnswer.X, myAnswer.Y);
                Log("*** Actual: Vector.X = {0}, Vector.Y = {1}", theirAnswer.X, theirAnswer.Y);
            }
        }

        private void TestEmpty()
        {
            Log("Testing Empty property...");

            Size theirAnswer = Size.Empty;

            if (theirAnswer.Width != Const2D.negInf || theirAnswer.Height != Const2D.negInf || failOnPurpose)
            {
                AddFailure("Empty failed");
                Log("***Expected: Size.Width = {0}, Size.Height = {1}", Const2D.negInf, Const2D.negInf);
                Log("***Actual: Size.Width = {0}, Size.Height = {1}", theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestIsEmpty()
        {
            TestIsEmptyWith(Size.Empty);
            TestIsEmptyWith(Const2D.size1);
        }

        private void TestIsEmptyWith(Size size)
        {
            Log("Testing IsEmpty()...");

            bool myAnswer = false;
            if (size.Width == Const2D.negInf && size.Height == Const2D.negInf || failOnPurpose)
            {
                myAnswer = true;
            }

            bool theirAnswer = size.IsEmpty;

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsEmpty() failed");
                Log("***Expected: IsEmpty = {0}", myAnswer);
                Log("***Actual: IsEmpty = {0}", theirAnswer);
            }
        }

        private void TestWidth()
        {
            Log("Testing Width property...");

            Size size = new Size();
            size.Width = Const2D.size1.Width;
            double theirAnswer = size.Width;

            if (theirAnswer != Const2D.size1.Width || failOnPurpose)
            {
                AddFailure("get/set Width failed");
                Log("***Expected: Width = {0}", Const2D.size1.Width);
                Log("***Actual: Width = {0}", theirAnswer);
            }
        }

        private void TestWidth2()
        {
            Log("Testing Width property with bad parameters...");

            Try(SetWidthOnEmptySize, typeof(InvalidOperationException));
            Try(SetNegWidth, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Width

        private void SetWidthOnEmptySize()
        {
            Size size = Size.Empty;
            size.Width = 1.1;
        }

        private void SetNegWidth()
        {
            Size size = new Size();
            size.Width = -1.1;
        }

        #endregion

        private void TestHeight()
        {
            Log("Testing Height property...");

            Size size = new Size();
            size.Height = Const2D.size1.Height;
            double theirAnswer = size.Height;

            if (theirAnswer != Const2D.size1.Height || failOnPurpose)
            {
                AddFailure("get/set Height failed");
                Log("***Expected: Height = {0}", Const2D.size1.Height);
                Log("***Actual: Height = {0}", theirAnswer);
            }
        }

        private void TestHeight2()
        {
            Log("Testing Height property with bad parameters...");

            Try(SetHeightOnEmptySize, typeof(InvalidOperationException));
            Try(SetNegHeight, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Height

        private void SetHeightOnEmptySize()
        {
            Size size = Size.Empty;
            size.Height = 1.1;
        }

        private void SetNegHeight()
        {
            Size size = new Size();
            size.Height = -1.1;
        }

        #endregion

        private void TestEquals()
        {
            TestEqualsWith(Size.Empty, Size.Empty);
            TestEqualsWith(new Size(1.1, 0.2), Size.Empty);
            TestEqualsWith(new Size(1.1, 0.2), new Size(1.1, 0.2));
            TestEqualsWith(new Size(1.1, 0.2), new Size(1.1, 0.22));
            TestEqualsWith(new Size(1.1, 0.2), null);
            TestEqualsWith(new Size(1.1, 0.2), new Point(1.1, 0.2));
        }

        private void TestEqualsWith(Size size1, object obj)
        {
            Log("Testing Equals( object )...");

            bool theirAnswer1 = size1.Equals(obj);

            bool myAnswer = true;
            if (obj == null || !(obj is Size))
            {
                myAnswer = false;
                if (theirAnswer1 != myAnswer || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed");
                    Log("***Expected: {0}", myAnswer);
                    Log("***Actual: {0}", theirAnswer1);
                }
            }
            else // obj is Size
            {
                Size size2 = (Size)obj;

                if (size1.Width != size2.Width || size1.Height != size2.Height)
                {
                    myAnswer = false;
                }

                if (theirAnswer1 != myAnswer || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed");
                    Log("***Expected: {0}", myAnswer);
                    Log("***Actual: {0}", theirAnswer1);
                }

                Log("Testing Size.Equals( Size, Size )...");

                bool theirAnswer2 = Size.Equals(size1, size2);
                if (theirAnswer2 != myAnswer || failOnPurpose)
                {
                    AddFailure("Size.Equals( Size ) failed");
                    Log("***Expected: {0}", myAnswer);
                    Log("***Actual: {0}", theirAnswer2);
                }

                Log("Testing ==...");

                bool theirAnswer3 = (size1 == size2);
                if (theirAnswer3 != myAnswer || failOnPurpose)
                {
                    AddFailure("== failed");
                    Log("***Expected: {0}", myAnswer);
                    Log("***Actual: {0}", theirAnswer3);
                }
            }
        }

        private void TestGetHashCode()
        {
            TestGetHashCodeWith(Size.Empty);
            TestGetHashCodeWith(Const2D.size1);
            TestGetHashCodeWith(new Size(1.1, 0.2));
        }

        private void TestGetHashCodeWith(Size size)
        {
            Log("Testing GetHashCode()...");

            int theirAnswer = size.GetHashCode();

            int myAnswer;
            if (size.IsEmpty == true)
            {
                myAnswer = 0;
            }
            else
            {
                myAnswer = size.Width.GetHashCode() ^ size.Height.GetHashCode();
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("GetHashCode() failed");
                Log("***Expected: HashCode of {0} = {1}", size, myAnswer);
                Log("***Actual: HashCode of {0} = {1}", size, theirAnswer);
            }
        }

        private void TestOpInequality()
        {
            TestOpInequalityWith(Size.Empty, Size.Empty);
            TestOpInequalityWith(new Size(1.1, 0.2), Size.Empty);
            TestOpInequalityWith(new Size(1.1, 0.2), new Size(1.1, 0.2));
            TestOpInequalityWith(new Size(1.2, 0.1), new Size(1.1, 0.2));
        }

        private void TestOpInequalityWith(Size size1, Size size2)
        {
            Log("Testing != Operator...");

            bool myAnswer = false;
            if (size1.Width != size2.Width || size1.Height != size2.Height)
            {
                myAnswer = true;
            }

            bool theirAnswer = size1 != size2;

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("!= Operator failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestParse()
        {
            TestParseWith("Empty");
            TestParseWith("0" + _sep + "0");
            TestParseWith(" 1.1 " + _sep + "0.2");
        }

        private void TestParseWith(string s)
        {
            Log("Testing Size.Parse( string )...");

            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size theirAnswer = Size.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size myAnswer = StringConverter.ToSize(invariant);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure(" Size.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestParse2()
        {
            Log("Testing Size.Parse( string ) with bad parameters...");

            Try(ParseTooFewParams, typeof(InvalidOperationException));
            Try(ParseTooManyParams, typeof(InvalidOperationException));
            Try(ParseNegWidthHeight, typeof(ArgumentException));
            Try(ParseEmptyString, typeof(InvalidOperationException));
            Try(ParseWrongFormat, typeof(FormatException));
        }

        #region ExceptionThrowers for Parse

        private void ParseTooFewParams()
        {
            string s = "1.1";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size size = Size.Parse(global);
        }

        private void ParseTooManyParams()
        {
            string s = "1.1" + _sep + "0.2" + _sep + "2.3";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size size = Size.Parse(global);
        }

        private void ParseNegWidthHeight()
        {
            string s = "-1" + _sep + "-2";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size size = Size.Parse(global);
        }

        private void ParseEmptyString()
        {
            string s = "";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size size = Size.Parse(global);
        }

        private void ParseWrongFormat()
        {
            string s = "a" + _sep + "0.1";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Size size = Size.Parse(global);
        }

        #endregion

        private void TestToString()
        {
            TestToStringWith(Size.Empty);
            TestToStringWith(new Size(0, 0));
            TestToStringWith(new Size(1.1, 0.2));
        }

        private void TestToStringWith(Size size)
        {
            Log("Testing ToString...");

            string theirAnswer = size.ToString();

            // Don't want these to be affected by locale yet
            string myWidth = size.Width.ToString(CultureInfo.InvariantCulture);
            string myHeight = size.Height.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myWidth + _sep + myHeight;
            if (size.IsEmpty)
            {
                myAnswer = "Empty";
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

            theirAnswer = size.ToString(CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)size).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myWidth = size.Width.ToString("N", numberFormat);
            myHeight = size.Height.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = myWidth + _sep + myHeight;
            if (size.IsEmpty)
            {
                myAnswer = "Empty";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( string, IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }
    }
}