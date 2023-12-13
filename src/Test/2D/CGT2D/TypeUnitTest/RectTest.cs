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
    public class RectTest : CoreGraphicsTest
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
                TestConstructor();
                TestBottom();
                TestBottomLeft();
                TestBottomRight();
                TestContains();
                TestEmpty();
                TestEquals();
                TestGetHashCode();
                TestInflate();
                TestIntersect();
                TestIntersectsWith();
                TestIsEmpty();
                TestLeft();
                TestOffset();
                TestOpEquality();
                TestOpInequality();
                TestParse();
                TestProperties();
                TestRight();
                TestTop();
                TestTopLeft();
                TestTopRight();
                TestToString();
                TestTransform();
                TestUnion();
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor...");

            TestConstructorWith();  // default

            TestConstructorWith(0, 0, 0, 0);
            TestConstructorWith(11.11, 22.22, 0, 0); // zero width, height
            TestConstructorWith(-11.11, -22.22, 0, 0);   // zero width, height
            TestConstructorWith(11.11, -22.22, 0, 0);    // zero width, height

            TestConstructorWith(0, 0, 0, 11.11); // zero width
            TestConstructorWith(11.11, 22.22, 0, 11.11); // zero width
            TestConstructorWith(-11.11, -22.22, 0, 11.11);   //zero width
            TestConstructorWith(11.11, -22.22, 0, 11.11);    // zero width

            TestConstructorWith(0, 0, 11.11, 0); // zero height
            TestConstructorWith(11.11, 22.22, 11.11, 0); // zero height
            TestConstructorWith(-11.11, -22.22, 11.11, 0);   // zero height
            TestConstructorWith(11.11, -22.22, 11.11, 0);    // zero height

            TestConstructorWith(0, 0, 11.11, 22.22);
            TestConstructorWith(11.11, 22.22, 11.11, 22.22);
            TestConstructorWith(-11.11, -22.22, 11.11, 22.22);
            TestConstructorWith(11.11, -22.22, 11.11, 22.22);

            TestConstructorWith(new Point(), new Size()); // zero width, height
            TestConstructorWith(new Point(11.11, 22.22), new Size());   // zero width, height
            TestConstructorWith(new Point(-11.11, -22.22), new Size()); // zero width, height
            TestConstructorWith(new Point(11.11, -22.22), new Size());  // zero width, height

            TestConstructorWith(new Point(), new Size(0, 11.11));   // zero width
            TestConstructorWith(new Point(11.11, 22.22), new Size(0, 11.11)); // zero width
            TestConstructorWith(new Point(-11.11, -22.22), new Size(0, 11.11));   // zero width
            TestConstructorWith(new Point(11.11, -22.22), new Size(0, 11.11));    // zero width

            TestConstructorWith(new Point(), new Size(11.11, 0));   // zero height
            TestConstructorWith(new Point(11.11, 22.22), new Size(11.11, 0)); // zero height
            TestConstructorWith(new Point(-11.11, -22.22), new Size(11.11, 0));   // zero height
            TestConstructorWith(new Point(11.11, -22.22), new Size(11.11, 0));    // zero height

            TestConstructorWith(new Point(), new Size(11.11, 22.22));
            TestConstructorWith(new Point(11.11, 22.22), new Size(11.11, 22.22));
            TestConstructorWith(new Point(-11.11, -22.22), new Size(11.11, 22.22));
            TestConstructorWith(new Point(11.11, -22.22), new Size(11.11, 22.22));

            TestConstructorWith(new Point(), Size.Empty);
            TestConstructorWith(new Point(11.11, 22.22), Size.Empty);
            TestConstructorWith(new Point(-11.11, -22.22), Size.Empty);
            TestConstructorWith(new Point(11.11, -22.22), Size.Empty);

            TestConstructorWith(new Point(), new Point());    // default
            TestConstructorWith(new Point(11.11, 22.22), new Point(11.11, 22.22));    // zero width, height
            TestConstructorWith(new Point(11.11, 22.22), new Point(11.11, 33.33));   // zero width
            TestConstructorWith(new Point(11.11, 22.22), new Point(33.33, 22.22));   // zero height
            TestConstructorWith(new Point(11.11, 22.22), new Point(33.33, 44.44));   // first point is upper-left corner
            TestConstructorWith(new Point(33.33, 44.44), new Point(-11.11, -22.22)); // second point is upper-left corner

            TestConstructorWith(new Point(), new Vector());   // dafault
            TestConstructorWith(new Point(11.11, 22.22), new Vector(11.11, 22.22));   // zero width, height
            TestConstructorWith(new Point(11.11, 22.22), new Vector(11.11, 33.33));  // zero width
            TestConstructorWith(new Point(11.11, 22.22), new Vector(33.33, 22.22));  // zero height
            TestConstructorWith(new Point(11.11, 22.22), new Vector(33.33, 44.44));  // first point is upper-left corner
            TestConstructorWith(new Point(33.33, 44.44), new Vector(-11.11, -22.22));    // second point is upper-left corner

            TestConstructorWith(new Size());  // zero width, height
            TestConstructorWith(new Size(0, 11.11));    // zero width
            TestConstructorWith(new Size(11.11, 0));    // zero height
            TestConstructorWith(new Size(11.11, 22.22));
            TestConstructorWith(Size.Empty);
        }

        private void TestConstructorWith()
        {
            Rect theirAnswer = new Rect();

            if (!MathEx.Equals(theirAnswer.X, 0) || !MathEx.Equals(theirAnswer.Y, 0) ||
                !MathEx.Equals(theirAnswer.Width, 0) || !MathEx.Equals(theirAnswer.Height, 0) || failOnPurpose)
            {
                AddFailure("Constructor Rect() failed");
                Log("*** Expected: X = {0}, Y = {1}, Width = {2}, Height = {3}", 0, 0, 0, 0);
                Log("*** Actual:   X = {0}, Y = {1}, Width = {2}, Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        // width and height here must be non-negatives, this is not bad param testings
        private void TestConstructorWith(double x, double y, double width, double height)
        {
            Rect theirAnswer = new Rect(x, y, width, height);

            if (!MathEx.Equals(theirAnswer.X, x) || !MathEx.Equals(theirAnswer.Y, y) ||
                !MathEx.Equals(theirAnswer.Width, width) || !MathEx.Equals(theirAnswer.Height, height) || failOnPurpose)
            {
                AddFailure("Constructor Rect( double, double, double, double ) failed");
                Log("*** Expected: X = {0}, Y = {1}, Width = {2}, Height = {3}", x, y, width, height);
                Log("*** Actual:   X = {0}, Y = {1}, Width = {2}, Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestConstructorWith(Point location, Size size)
        {
            Rect theirAnswer = new Rect(location, size);

            Rect myAnswer = Rect.Empty;
            if (!size.IsEmpty)
            {
                myAnswer = new Rect(location.X, location.Y, size.Width, size.Height);
            }

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Consrtructor Rect( Point, Size ) failed");
                Log("*** Expected: Rect = {0}", myAnswer);
                Log("*** Actual:   Rect = {0}", theirAnswer);
            }
        }

        private void TestConstructorWith(Point point, Vector vector)
        {
            Rect theirAnswer = new Rect(point, vector);

            Point tmpPoint = point + vector;
            double myX = Math.Min(point.X, tmpPoint.X);
            double myY = Math.Min(point.Y, tmpPoint.Y);
            double myWidth = Math.Max(point.X, tmpPoint.X) - myX;
            double myHeight = Math.Max(point.Y, tmpPoint.Y) - myY;

            Rect myAnswer = new Rect(myX, myY, myWidth, myHeight);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Constructor Rect( Point, Vector ) failed");
                Log("*** Expected: Rect = {0}", myAnswer);
                Log("*** Actual:   Rect = {0}", theirAnswer);
            }
        }

        private void TestConstructorWith(Point point1, Point point2)
        {
            Rect theirAnswer = new Rect(point1, point2);

            double myX = Math.Min(point1.X, point2.X);
            double myY = Math.Min(point1.Y, point2.Y);
            double myWidth = Math.Max(point1.X, point2.X) - myX;
            double myHeight = Math.Max(point1.Y, point2.Y) - myY;

            Rect myAnswer = new Rect(myX, myY, myWidth, myHeight);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Constructor Rect( Point, Point ) failed");
                Log("*** Expected: Rect = {0}", myAnswer);
                Log("*** Actual:   Rect = {0}", theirAnswer);
            }
        }

        private void TestConstructorWith(Size size)
        {
            Rect theirAnswer = new Rect(size);

            Rect myAnswer = Rect.Empty;
            if (!size.IsEmpty)
            {
                myAnswer = new Rect(0, 0, size.Width, size.Height);
            }

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Constructor Rect( Size ) failed");
                Log("*** Expected: Rect = {0}", myAnswer);
                Log("*** Actual:   Rect = {0}", theirAnswer);
            }
        }

        private void TestBottom()
        {
            Log("Testing Bottom...");

            TestBottomWith(Rect.Empty);

            TestBottomWith(new Rect(0, 0, 0, 0));   // zero width, height
            TestBottomWith(new Rect(11.11, 22.22, 0, 0));   // zero width, height
            TestBottomWith(new Rect(-11.11, -22.22, 0, 0)); // zero width, height
            TestBottomWith(new Rect(11.11, -22.22, 0, 0));  // zero width, height

            TestBottomWith(new Rect(0, 0, 0, 11.11));   // zero width
            TestBottomWith(new Rect(11.11, 22.22, 0, 11.11));   // zero width
            TestBottomWith(new Rect(-11.11, -22.22, 0, 11.11)); // zero width
            TestBottomWith(new Rect(11.11, -22.22, 0, 11.11));  // zero width

            TestBottomWith(new Rect(0, 0, 11.11, 0));   // zero height
            TestBottomWith(new Rect(11.11, 22.22, 11.11, 0));   // zero height
            TestBottomWith(new Rect(-11.11, -22.22, 11.11, 0)); // zero height
            TestBottomWith(new Rect(11.11, -22.22, 11.11, 0));  // zero height

            TestBottomWith(new Rect(0, 0, 11.11, 22.22));
            TestBottomWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestBottomWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestBottomWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestBottomWith(Rect rect)
        {
            double theirAnswer = rect.Bottom;

            double myAnswer;
            if (rect.IsEmpty)
            {
                myAnswer = Const2D.negInf;
            }
            else
            {
                myAnswer = rect.Y + rect.Height;
            }

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Bottom failed");
                Log("***Expected: Bottom = {0}", myAnswer);
                Log("***Actual: Bottom = {0}", theirAnswer);
            }
        }

        private void TestBottomLeft()
        {
            Log("Testing BottomLeft...");

            TestBottomLeftWith(Rect.Empty);

            TestBottomLeftWith(new Rect(0, 0, 0, 0));   // zero width, height
            TestBottomLeftWith(new Rect(11.11, 22.22, 0, 0));   // zero width, height
            TestBottomLeftWith(new Rect(-11.11, -22.22, 0, 0)); // zero width, height
            TestBottomLeftWith(new Rect(11.11, -22.22, 0, 0));  // zero width, height

            TestBottomLeftWith(new Rect(0, 0, 0, 11.11));   // zero width
            TestBottomLeftWith(new Rect(11.11, 22.22, 0, 11.11));   // zero width
            TestBottomLeftWith(new Rect(-11.11, -22.22, 0, 11.11)); // zero width
            TestBottomLeftWith(new Rect(11.11, -22.22, 0, 11.11));  // zero width

            TestBottomLeftWith(new Rect(0, 0, 11.11, 0));   // zero height
            TestBottomLeftWith(new Rect(11.11, 22.22, 11.11, 0));   // zero height
            TestBottomLeftWith(new Rect(-11.11, -22.22, 11.11, 0)); // zero height
            TestBottomLeftWith(new Rect(11.11, -22.22, 11.11, 0));  // zero height

            TestBottomLeftWith(new Rect(0, 0, 11.11, 22.22));
            TestBottomLeftWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestBottomLeftWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestBottomLeftWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestBottomLeftWith(Rect rect)
        {
            Point theirAnswer = rect.BottomLeft;

            Point myAnswer = new Point(rect.X, rect.Bottom);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("BottomLeft failed");
                Log("***Expected: BottomLeft = {0}", myAnswer);
                Log("***Actual: BottomLeft = {0}", theirAnswer);
            }
        }

        private void TestBottomRight()
        {
            Log("Testing BottomRight...");

            TestBottomRightWith(Rect.Empty);

            TestBottomRightWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestBottomRightWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestBottomRightWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestBottomRightWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestBottomRightWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestBottomRightWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestBottomRightWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestBottomRightWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestBottomRightWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestBottomRightWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestBottomRightWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestBottomRightWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestBottomRightWith(new Rect(0, 0, 11.11, 22.22));
            TestBottomRightWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestBottomRightWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestBottomRightWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestBottomRightWith(Rect rect)
        {
            Point theirAnswer = rect.BottomRight;

            Point myAnswer = new Point(rect.Right, rect.Bottom);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("BottomRight failed");
                Log("***Expected: BottomRight = {0}", myAnswer);
                Log("***Actual: BottomRight = {0}", theirAnswer);
            }
        }

        private void TestEmpty()
        {
            Log("Testing Rect.Empty...");

            Rect theirAnswer = Rect.Empty;

            if (!theirAnswer.IsEmpty || failOnPurpose)
            {
                AddFailure("Rect.Empty failed");
                Log("***Expected: Rect.X = {0}, Rect.Y ={1}, Rect.Width = {2}, Rect.Height = {3}", Const2D.posInf, Const2D.posInf, Const2D.negInf, Const2D.negInf);
                Log("***Actual: Rect.X = {0}, Rect.Y = {1}, Rect.Width = {2}, Rect.Height = {3}", theirAnswer.X, theirAnswer.Y, theirAnswer.Width, theirAnswer.Height);
            }
        }

        private void TestIsEmpty()
        {
            Log("Testing IsEmpty...");

            TestIsEmptyWith(Rect.Empty);
            TestIsEmptyWith(new Rect());
            TestIsEmptyWith(new Rect(0, 0, 11.11, 0));
            TestIsEmptyWith(new Rect(0, 0, 0, 11.11));
            TestIsEmptyWith(new Rect(0, 0, 11.11, 22.22));
            TestIsEmptyWith(new Rect(new Point(1, 1), Size.Empty));   //regression Regression_Bug73
            TestIsEmptyWith(new Rect(new Point(System.Double.NaN, System.Double.NaN), new Point(System.Double.NaN, System.Double.NaN))); //regression Regression_Bug74
        }

        private void TestIsEmptyWith(Rect rect)
        {
            bool myAnswer = true;
            if (!MathEx.Equals(rect.X, Const2D.posInf) || !MathEx.Equals(rect.Y, Const2D.posInf) ||
                !MathEx.Equals(rect.Width, Const2D.negInf) || !MathEx.Equals(rect.Height, Const2D.negInf))
            {
                myAnswer = false;
            }

            bool theirAnswer = rect.IsEmpty;

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("IsEmpty failed");
                Log("***Expected: IsEmpty = {0}", myAnswer);
                Log("***Actual: IsEmpty = {0}", theirAnswer);
            }
        }

        private void TestLeft()
        {
            Log("Testing Left...");

            TestLeftWith(Rect.Empty);

            TestLeftWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestLeftWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestLeftWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestLeftWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestLeftWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestLeftWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestLeftWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestLeftWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestLeftWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestLeftWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestLeftWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestLeftWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestLeftWith(new Rect(0, 0, 11.11, 22.22));
            TestLeftWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestLeftWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestLeftWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestLeftWith(Rect rect)
        {
            double theirAnswer = rect.Left;
            double myAnswer = rect.X;

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Left failed");
                Log("***Expected: Left = {0}", myAnswer);
                Log("***Actual: Left = {0}", theirAnswer);
            }
        }

        private void TestRight()
        {
            Log("Testing Right...");

            TestRightWith(Rect.Empty);

            TestRightWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestRightWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestRightWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestRightWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestRightWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestRightWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestRightWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestRightWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestRightWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestRightWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestRightWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestRightWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestRightWith(new Rect(0, 0, 11.11, 22.22));
            TestRightWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestRightWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestRightWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestRightWith(Rect rect)
        {
            double theirAnswer = rect.Right;

            double myAnswer;
            if (rect.IsEmpty)
            {
                myAnswer = Const2D.negInf;
            }
            else
            {
                myAnswer = rect.X + rect.Width;
            }
            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Right failed");
                Log("***Expected: Right = {0}", myAnswer);
                Log("***Actual: Right = {0}", theirAnswer);
            }
        }

        private void TestTop()
        {
            Log("Testing Top...");

            TestTopWith(Rect.Empty);

            TestTopWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestTopWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestTopWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestTopWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestTopWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestTopWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestTopWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestTopWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestTopWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestTopWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestTopWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestTopWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestTopWith(new Rect(0, 0, 11.11, 22.22));
            TestTopWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestTopWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestTopWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestTopWith(Rect rect)
        {
            double theirAnswer = rect.Top;
            double myAnswer = rect.Y;

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Top failed");
                Log("***Expected: Top = {0}", myAnswer);
                Log("***Actual: Top = {0}", theirAnswer);
            }
        }

        private void TestTopLeft()
        {
            Log("Testing TopLeft...");

            TestTopLeftWith(Rect.Empty);

            TestTopLeftWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestTopLeftWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestTopLeftWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestTopLeftWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestTopLeftWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestTopLeftWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestTopLeftWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestTopLeftWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestTopLeftWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestTopLeftWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestTopLeftWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestTopLeftWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestTopLeftWith(new Rect(0, 0, 11.11, 22.22));
            TestTopLeftWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestTopLeftWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestTopLeftWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestTopLeftWith(Rect rect)
        {
            Point theirAnswer = rect.TopLeft;

            Point myAnswer = new Point(rect.X, rect.Y);

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("TopLeft failed");
                Log("***Expected: TopLeft = {0}", myAnswer);
                Log("***Actual: TopLeft = {0}", theirAnswer);
            }
        }

        private void TestTopRight()
        {
            Log("Testing TopRight...");

            TestTopRightWith(Rect.Empty);

            TestTopRightWith(new Rect(0, 0, 0, 0));  // zero width, height
            TestTopRightWith(new Rect(11.11, 22.22, 0, 0));  // zero width, height
            TestTopRightWith(new Rect(-11.11, -22.22, 0, 0));    // zero width, height
            TestTopRightWith(new Rect(11.11, -22.22, 0, 0)); // zero width, height

            TestTopRightWith(new Rect(0, 0, 0, 11.11));  // zero width
            TestTopRightWith(new Rect(11.11, 22.22, 0, 11.11));  // zero width
            TestTopRightWith(new Rect(-11.11, -22.22, 0, 11.11));    // zero width
            TestTopRightWith(new Rect(11.11, -22.22, 0, 11.11)); // zero width

            TestTopRightWith(new Rect(0, 0, 11.11, 0));  // zero height
            TestTopRightWith(new Rect(11.11, 22.22, 11.11, 0));  // zero height
            TestTopRightWith(new Rect(-11.11, -22.22, 11.11, 0));    // zero height
            TestTopRightWith(new Rect(11.11, -22.22, 11.11, 0)); // zero height

            TestTopRightWith(new Rect(0, 0, 11.11, 22.22));
            TestTopRightWith(new Rect(11.11, 22.22, 11.11, 22.22));
            TestTopRightWith(new Rect(-11.11, -22.22, 11.11, 22.22));
            TestTopRightWith(new Rect(11.11, -22.22, 11.11, 22.22));
        }

        private void TestTopRightWith(Rect rect)
        {
            Point theirAnswer = rect.TopRight;

            Point myAnswer;
            if (rect.IsEmpty)
            {
                myAnswer = new Point(Const2D.negInf, Const2D.posInf);
            }
            else
            {
                myAnswer = new Point(rect.X + rect.Width, rect.Y);
            }
            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("TopRight failed");
                Log("***Expected: TopRight = {0}", myAnswer);
                Log("***Actual: TopRight = {0}", theirAnswer);
            }
        }

        private void TestContains()
        {
            Log("Testing Contains...");

            TestContainsWith(Rect.Empty, new Point());
            TestContainsWith(Rect.Empty, new Point(11.11, -22.22));

            TestContainsWith(new Rect(), new Point());    // zero width, height
            TestContainsWith(new Rect(10, 10, 0, 0), new Point(10, 10));    // zero width, height
            TestContainsWith(new Rect(), new Point(11.11, -22.22)); // zero width, height

            TestContainsWith(new Rect(10, 10, 0, 10), new Point(10, 10));  // zero width on end point
            TestContainsWith(new Rect(10, 10, 0, 10), new Point(10, 20));  // zero width on end point
            TestContainsWith(new Rect(10, 10, 0, 10), new Point(10, 15));  // zero width in
            TestContainsWith(new Rect(10, 10, 0, 10), new Point(30, 30));  // zero width out

            TestContainsWith(new Rect(10, 10, 10, 0), new Point(10, 10));  // zero height on end point
            TestContainsWith(new Rect(10, 10, 10, 0), new Point(20, 10));  // zero height on end point
            TestContainsWith(new Rect(10, 10, 10, 0), new Point(15, 10));  // zero height in
            TestContainsWith(new Rect(10, 10, 10, 0), new Point(30, 30));  // zero height out

            TestContainsWith(new Rect(10, 10, 10, 10), new Point(15, 10));    // on top edge
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(20, 15));    // on right edge
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(10, 15));    // on left edge
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(15, 20));    // on bottom edge
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(10, 10));    // on upper left corner
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(20, 10));    // on upper right corner
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(10, 20));    // on lower left corner
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(20, 20));    // on lower right corner
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(15, 15));    // in
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(30, 30));    // out

            TestContainsWith(new Rect(10, 10, 10, 10), new Point(9.5, 15));   // fail on left
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(20.5, 15));  // fail on right
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(10, 9.5));   // fail on top
            TestContainsWith(new Rect(10, 10, 10, 10), new Point(10, 20.5));  // fail on bottom


            TestContainsWith(Rect.Empty, 0, 0);
            TestContainsWith(Rect.Empty, 11.11, -22.22);

            TestContainsWith(new Rect(), 0, 0); // zero width, height
            TestContainsWith(new Rect(10, 10, 0, 0), 10, 10);    // zero width, height
            TestContainsWith(new Rect(), 11.11, -22.22); // zero width, height

            TestContainsWith(new Rect(10, 10, 0, 10), 10, 10);  // zero width on end point
            TestContainsWith(new Rect(10, 10, 0, 10), 10, 20);  // zero width on end point
            TestContainsWith(new Rect(10, 10, 0, 10), 10, 15);  // zero width in
            TestContainsWith(new Rect(10, 10, 0, 10), 30, 30);  // zero width out

            TestContainsWith(new Rect(10, 10, 10, 0), 10, 10);  // zero height on end point
            TestContainsWith(new Rect(10, 10, 10, 0), 20, 10);  // zero height on end point
            TestContainsWith(new Rect(10, 10, 10, 0), 15, 10);  // zero height in
            TestContainsWith(new Rect(10, 10, 10, 0), 30, 30);  // zero height out

            TestContainsWith(new Rect(10, 10, 10, 10), 15, 10);    // on top edge
            TestContainsWith(new Rect(10, 10, 10, 10), 20, 15);    // on right edge
            TestContainsWith(new Rect(10, 10, 10, 10), 10, 15);    // on left edge
            TestContainsWith(new Rect(10, 10, 10, 10), 15, 20);    // on bottom edge
            TestContainsWith(new Rect(10, 10, 10, 10), 10, 10);    // on upper left corner
            TestContainsWith(new Rect(10, 10, 10, 10), 20, 10);    // on upper right corner
            TestContainsWith(new Rect(10, 10, 10, 10), 10, 20);    // on lower left corner
            TestContainsWith(new Rect(10, 10, 10, 10), 20, 20);    // on lower right corner
            TestContainsWith(new Rect(10, 10, 10, 10), 15, 15);    // in
            TestContainsWith(new Rect(10, 10, 10, 10), 30, 30);    // out

            TestContainsWith(new Rect(10, 10, 10, 10), 9.5, 15);   // fail on left
            TestContainsWith(new Rect(10, 10, 10, 10), 20.5, 15);  // fail on right
            TestContainsWith(new Rect(10, 10, 10, 10), 10, 9.5);   // fail on top
            TestContainsWith(new Rect(10, 10, 10, 10), 10, 20.5);  // fail on bottom

            TestContainsWith(Rect.Empty, Rect.Empty);
            TestContainsWith(Rect.Empty, new Rect(11.11, 22.22, 11.11, 22.22));
            TestContainsWith(new Rect(11.11, 22.22, 11.11, 22.22), Rect.Empty);
            TestContainsWith(new Rect(), new Rect(10, 10, 10, 10));

            TestContainsWith(new Rect(10, 10, 10, 0), new Rect(10, 10, 10, 0));  // same
            TestContainsWith(new Rect(10, 10, 10, 0), new Rect(15, 10, 2, 0));  // completely inside
            TestContainsWith(new Rect(10, 10, 10, 0), new Rect(30, 30, 30, 30));  // completely outside
            TestContainsWith(new Rect(10, 10, 10, 0), new Rect(15, 10, 10, 0));  // intersect

            TestContainsWith(new Rect(10, 10, 0, 10), new Rect(10, 10, 0, 10));  // same
            TestContainsWith(new Rect(10, 10, 0, 10), new Rect(10, 15, 0, 2));  // completely inside
            TestContainsWith(new Rect(10, 10, 0, 10), new Rect(30, 30, 30, 30));  // completely outside
            TestContainsWith(new Rect(10, 10, 0, 10), new Rect(10, 15, 0, 10));  // intersect

            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10));  // same
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(15, 15, 2, 2));  // completely inside
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(30, 30, 30, 30));  // completely outside
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(10, 15, 20, 20));  // intersect

            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(9.5, 10, 10, 10));    // fail on left
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(10, 9.5, 10, 10));    // fail on top
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10.5, 10));    // fail on right
            TestContainsWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10.5));    // fail on bottom
        }

        private void TestContainsWith(Rect rect, Point point)
        {
            bool theirAnswer = rect.Contains(point);

            bool myAnswer = true;
            if (rect.IsEmpty || point.X < rect.X || point.X > rect.Right || point.Y < rect.Y || point.Y > rect.Bottom)
            {
                myAnswer = false;
            }

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Contains( Point ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestContainsWith(Rect rect, double x, double y)
        {
            bool theirAnswer = rect.Contains(x, y);

            bool myAnswer = true;
            if (rect.IsEmpty || x < rect.X || x > rect.Right || y < rect.Y || y > rect.Bottom)
            {
                myAnswer = false;
            }

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Contains( double, double ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestContainsWith(Rect rect1, Rect rect2)
        {
            bool theirAnswer = rect1.Contains(rect2);

            bool myAnswer = true;
            if (rect1.IsEmpty.Equals(true) || rect2.IsEmpty.Equals(true) ||
                rect2.X < rect1.X || rect2.Right > rect1.Right ||
                rect2.Y < rect1.Y || rect2.Bottom > rect1.Bottom)
            {
                myAnswer = false;
            }
            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Contains( Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestEquals()
        {
            Log("Testing Equals...");

            TestEqualsWith(Rect.Empty, Rect.Empty);
            TestEqualsWith(new Rect(), Rect.Empty);
            TestEqualsWith(Rect.Empty, new Rect());
            TestEqualsWith(new Rect(), new Rect());
            TestEqualsWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 11.11, 22.22));
            TestEqualsWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(-11.11, -22.22, 11.11, 22.22));    // fail on X
            TestEqualsWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, 22.22, 11.11, 22.22));  // fail on Y
            TestEqualsWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 22.22, 22.22)); // fail on Width
            TestEqualsWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 11.11, 11.11)); // fail on Height

            TestEqualsWith(new Rect(), false);
            TestEqualsWith(new Rect(), null);

            object obj1 = new Rect(0, 0, 10, 10);
            TestEqualsWith(new Rect(0, 0, 10, 10), obj1);

        }

        private void TestEqualsWith(Rect rect1, Rect rect2)
        {
            bool myAnswer = true;
            if (!MathEx.Equals(rect1, rect2))
            {
                myAnswer = false;
            }

            bool theirAnswer1 = rect1.Equals(rect2);
            if (!theirAnswer1.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Equals( Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer1);
            }

            bool theirAnswer2 = Rect.Equals(rect1, rect2);
            if (!theirAnswer2.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Equals( Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer2);
            }
        }

        private void TestEqualsWith(Rect rect1, object o)
        {
            bool theirAnswer = rect1.Equals(o);

            bool myAnswer = true;
            if (o == null || !(o is Rect))
            {
                myAnswer = false;
            }
            else
            {
                Rect rect2 = (Rect)o;
                myAnswer = MathEx.Equals(rect1, rect2);
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
            Log("Testing GetHashCode...");

            TestGetHashCodeWith(Rect.Empty);
            TestGetHashCodeWith(new Rect());
            TestGetHashCodeWith(new Rect(10, -10, 0, 10));  // zero width
            TestGetHashCodeWith(new Rect(10, -10, 10, 0));  // zero height
            TestGetHashCodeWith(new Rect(10, -10, 10, 10));
        }

        private void TestGetHashCodeWith(Rect rect1)
        {
            double theirAnswer = rect1.GetHashCode();

            double myAnswer = rect1.X.GetHashCode() ^ rect1.Y.GetHashCode() ^ rect1.Width.GetHashCode() ^ rect1.Height.GetHashCode();

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("GetHashCode() failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestInflate()
        {
            Log("Testing Inflate...");

            TestInflateWith(new Rect(), new Size());
            TestInflateWith(new Rect(10, 10, 10, 10), Size.Empty);
            TestInflateWith(new Rect(10, 10, 0, 0), new Size(0, 0));  // results in zero width and height
            TestInflateWith(new Rect(10, 10, 0, 10), new Size(0, 0)); // results in zero width
            TestInflateWith(new Rect(10, 10, 10, 0), new Size(0, 0)); // results in zero height
            TestInflateWith(new Rect(10, 10, 0, 10), new Size(5, 5));
            TestInflateWith(new Rect(10, 10, 10, 0), new Size(5, 5));
            TestInflateWith(new Rect(10, 10, 10, 10), new Size(5, 5));

            TestInflateWith(new Rect(), 0, 0);
            TestInflateWith(new Rect(10, 10, 10, 10), 0, 0);
            TestInflateWith(new Rect(10, 10, 10, 10), 5, 5);
            TestInflateWith(new Rect(10, 10, 10, 10), -3, -3);
            TestInflateWith(new Rect(10, 10, 10, 10), -5, -5);  // results in zero width and height
            TestInflateWith(new Rect(10, 10, 10, 10), -5, -3);  // results in zero width
            TestInflateWith(new Rect(10, 10, 10, 10), -3, -5);  // results in zero height
            TestInflateWith(new Rect(10, 10, 10, 10), -6, -3);  // results in negative width
            TestInflateWith(new Rect(10, 10, 10, 10), -3, -6);  // results in negative height
            TestInflateWith(new Rect(10, 10, 10, 10), -6, -6);  // results in negative width, height
        }

        private void TestInflateWith(Rect rect, Size size)
        {
            Rect myAnswer;
            // to ensure that we never allow Width, Height < 0
            if (size.Width * -2 > rect.Width || size.Height * -2 > rect.Height)
            {
                myAnswer = Rect.Empty;
            }
            else
            {
                double newX = rect.X - size.Width;
                double newY = rect.Y - size.Height;
                double newWidth = rect.Width + 2 * size.Width;
                double newHeight = rect.Height + 2 * size.Height;

                if (!(newWidth >= 0 && newHeight >= 0))
                {
                    myAnswer = Rect.Empty;
                }
                else
                {
                    myAnswer = new Rect(newX, newY, newWidth, newHeight);
                }
            }

            Rect theirAnswer1 = rect;

            theirAnswer1.Inflate(size);
            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Inflate( Size ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Inflate(rect, size);
            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Inflate( Size ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestInflateWith(Rect rect, double width, double height)
        {
            Rect myAnswer;
            // to ensure that we never allow Width, Height < 0
            if (width * -2 > rect.Width || height * -2 > rect.Height)
            {
                myAnswer = Rect.Empty;
            }
            else
            {
                double newX = rect.X - width;
                double newY = rect.Y - height;
                double newWidth = (rect.Width + width) + width;
                double newHeight = (rect.Height + height) + height;

                if (!(newWidth >= 0 && newHeight >= 0))
                {
                    myAnswer = Rect.Empty;
                }
                else
                {
                    myAnswer = new Rect(newX, newY, newWidth, newHeight);
                }
            }

            Rect theirAnswer1 = rect;

            theirAnswer1.Inflate(width, height);
            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Inflate( double, double ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Inflate(rect, width, height);
            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Inflate( double, double ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestIntersectsWith()
        {
            Log("Testing IntersectsWith...");

            TestIntersectsWithWith(Rect.Empty, Rect.Empty);
            TestIntersectsWithWith(Rect.Empty, new Rect(1, 1, 1, 1));
            TestIntersectsWithWith(new Rect(1, 1, 1, 1), Rect.Empty);
            TestIntersectsWithWith(new Rect(), new Rect());

            TestIntersectsWithWith(new Rect(10, 10, 0, 0), new Rect(10, 10, 0, 0));  // same
            TestIntersectsWithWith(new Rect(10, 10, 0, 0), new Rect(15, 15, 0, 0));

            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(10, 10, 10, 0));  // same
            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(15, 10, 2, 0));  // edge
            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(5, 10, 50, 0));
            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(30, 30, 30, 30));  // not intersected
            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(20, 10, 10, 0));  // point
            TestIntersectsWithWith(new Rect(10, 10, 10, 0), new Rect(0, 10, 10, 0));  // point

            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(10, 10, 0, 10));  // same
            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(10, 15, 0, 2));  // edge
            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(10, 5, 0, 50));
            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(30, 30, 30, 30));  // not intersected
            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(10, 20, 0, 10));  // point
            TestIntersectsWithWith(new Rect(10, 10, 0, 10), new Rect(10, 0, 0, 10));  // point

            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10));  // same
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(15, 15, 2, 2));  // completely inside
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 50, 50));
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(30, 30, 30, 30));  // completely outside
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, 15, 20, 20));  // intersected
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(0, 10, 10, 10));  // left edge
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(20, 10, 10, 10));  // right edge
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, 0, 10, 10));  // top edge
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, 20, 10, 10));  // bottom edge
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 10, 10));  // upper-left corner
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(20, 0, 10, 10));  // upper-right corner
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(0, 20, 10, 10));  // lower-left corner
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(20, 20, 10, 10));  // lower-right corner

            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(20.5, 10, 10, 10));  // fail
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(-0.5, 10, 10, 10));  // fail
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, 20.5, 10, 10));  // fail
            TestIntersectsWithWith(new Rect(10, 10, 10, 10), new Rect(10, -0.5, 10, 10));  // fail
        }

        private void TestIntersectsWithWith(Rect rect1, Rect rect2)
        {
            bool theirAnswer = rect1.IntersectsWith(rect2);

            bool myAnswer = true;
            if (rect1.IsEmpty || rect2.IsEmpty || rect1.Left > rect2.Right || rect1.Right < rect2.Left || rect1.Top > rect2.Bottom || rect1.Bottom < rect2.Top)
            {
                myAnswer = false;
            }

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("IntersectsWith( Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestIntersect()
        {
            Log("Testing Intersect...");

            TestIntersectWith(Rect.Empty, Rect.Empty);
            TestIntersectWith(Rect.Empty, new Rect(1, 1, 1, 1));
            TestIntersectWith(new Rect(1, 1, 1, 1), Rect.Empty);
            TestIntersectWith(new Rect(), new Rect());

            TestIntersectWith(new Rect(10, 10, 0, 0), new Rect(10, 10, 0, 0));  // same
            TestIntersectWith(new Rect(10, 10, 0, 0), new Rect(15, 15, 0, 0));

            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(10, 10, 10, 0));  // same
            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(15, 10, 2, 0));  // edge
            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(5, 10, 50, 0));
            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(30, 30, 30, 30));  // not intersected
            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(20, 10, 10, 0));  // point
            TestIntersectWith(new Rect(10, 10, 10, 0), new Rect(0, 10, 10, 0));  // point

            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(10, 10, 0, 10));  // same
            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(10, 15, 0, 2));  // edge
            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(10, 5, 0, 50));
            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(30, 30, 30, 30));  // not intersected
            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(10, 20, 0, 10));  // point
            TestIntersectWith(new Rect(10, 10, 0, 10), new Rect(10, 0, 0, 10));  // point

            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10));  // same
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(15, 15, 2, 2));  // completely inside
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 50, 50));
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(30, 30, 30, 30));  // completely outside
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, 15, 20, 20));  // intersected
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(0, 10, 10, 10));  // left edge
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(20, 10, 10, 10));  // right edge
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, 0, 10, 10));  // top edge
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, 20, 10, 10));  // bottom edge
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 10, 10));  // upper-left corner
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(20, 0, 10, 10));  // upper-right corner
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(0, 20, 10, 10));  // lower-left corner
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(20, 20, 10, 10));  // lower-right corner

            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(20.5, 10, 10, 10));  // not intersected
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(-0.5, 10, 10, 10));  // not intersected
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, 20.5, 10, 10));  // not intersected
            TestIntersectWith(new Rect(10, 10, 10, 10), new Rect(10, -0.5, 10, 10));  // not intersected
        }

        private void TestIntersectWith(Rect rect1, Rect rect2)
        {
            Rect myAnswer;
            if (!rect1.IntersectsWith(rect2))
            {
                myAnswer = Rect.Empty;
            }
            else
            {
                double newX = Math.Max(rect1.Left, rect2.Left);
                double newY = Math.Max(rect1.Top, rect2.Top);
                double newWidth = Math.Min(rect1.Right, rect2.Right) - newX;
                double newHeight = Math.Min(rect1.Bottom, rect2.Bottom) - newY;
                myAnswer = new Rect(newX, newY, newWidth, newHeight);
            }

            Rect theirAnswer1 = rect1;
            theirAnswer1.Intersect(rect2);
            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Intersect( Rect ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Intersect(rect1, rect2);
            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Intersect( Rect ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestOffset()
        {
            Log("Testing Offset...");

            TestOffsetWith(new Rect(), 0, 0);
            TestOffsetWith(new Rect(), 11.11, 22.22);
            TestOffsetWith(new Rect(), -11.11, -22.22);

            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), 0, 0);
            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), 11.11, 22.22);
            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), -11.11, -22.22);

            TestOffsetWith(new Rect(-11.11, -22.22, 11.11, 22.22), 0, 0);
            TestOffsetWith(new Rect(-11.11, -22.22, 11.11, 22.22), 11.11, 22.22);
            TestOffsetWith(new Rect(-11.11, -22.22, 11.11, 22.22), -11.11, -22.22);

            TestOffsetWith(new Rect(), new Vector(0, 0));
            TestOffsetWith(new Rect(), new Vector(11.11, 22.22));
            TestOffsetWith(new Rect(), new Vector(-11.11, -22.22));

            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), new Vector(0, 0));
            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), new Vector(11.11, 22.22));
            TestOffsetWith(new Rect(11.11, 22.22, 11.11, 22.22), new Vector(-11.11, -22.22));

            TestOffsetWith(new Rect(-11.11, -22.22, 11.11, 22.22), new Vector(0, 0));
            TestOffsetWith(new Rect(-11.11, -22.22, 11.11, 22.22), new Vector(11.11, 22.22));
            TestOffsetWith(new Rect(-11.11, 22.22, 11.11, 22.22), new Vector(-11.11, -22.22));
        }

        private void TestOffsetWith(Rect rect, Vector vector)
        {
            Rect theirAnswer1 = rect;
            theirAnswer1.Offset(vector);

            Rect myAnswer = new Rect(rect.X + vector.X, rect.Y + vector.Y, rect.Width, rect.Height);

            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Offset( Vector ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Offset(rect, vector);

            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Offset( Rect, Vector ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestOffsetWith(Rect rect, double x, double y)
        {
            Rect theirAnswer1 = rect;
            theirAnswer1.Offset(x, y);

            Rect myAnswer = new Rect(rect.X + x, rect.Y + y, rect.Width, rect.Height);

            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Offset( double, double ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Offset(rect, x, y);

            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Offset( Rect, double, double ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestOpEquality()
        {
            Log("Testing == Operator( Rect, Rect )...");

            TestOpEqualityWith(Rect.Empty, Rect.Empty);
            TestOpEqualityWith(new Rect(), Rect.Empty);
            TestOpEqualityWith(Rect.Empty, new Rect());
            TestOpEqualityWith(new Rect(), new Rect());
            TestOpEqualityWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 11.11, 22.22));
            TestOpEqualityWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(-11.11, -22.22, 11.11, 22.22));    // fail on X
            TestOpEqualityWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, 22.22, 11.11, 22.22));  // fail on Y
            TestOpEqualityWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 22.22, 22.22)); // fail on Width
            TestOpEqualityWith(new Rect(11.11, -22.22, 11.11, 22.22), new Rect(11.11, -22.22, 11.11, 11.11)); // fail on Height
        }

        private void TestOpEqualityWith(Rect rect1, Rect rect2)
        {
            bool theirAnswer = rect1 == rect2;

            bool myAnswer = MathEx.ClrOperatorEquals(rect1, rect2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("== Operator( Rect, Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestOpInequality()
        {
            Log("Testing != Operator( Rect, Rect )...");

            TestOpInequalityWith(Rect.Empty, Rect.Empty);
            TestOpInequalityWith(Rect.Empty, new Rect());
            TestOpInequalityWith(new Rect(), Rect.Empty);
            TestOpInequalityWith(new Rect(), new Rect());
            TestOpInequalityWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10));
            TestOpInequalityWith(new Rect(1, 1, 1, 1), new Rect(1, 1, 1, 10));
            TestOpInequalityWith(new Rect(1, 1, 1, 1), new Rect(1, 1, 10, 1));
            TestOpInequalityWith(new Rect(1, 1, 1, 1), new Rect(1, 10, 1, 1));
            TestOpInequalityWith(new Rect(1, 1, 1, 1), new Rect(10, 1, 1, 1));
        }

        private void TestOpInequalityWith(Rect rect1, Rect rect2)
        {
            bool theirAnswer = rect1 != rect2;

            bool myAnswer = MathEx.ClrOperatorNotEquals(rect1, rect2);

            if (!theirAnswer.Equals(myAnswer) || failOnPurpose)
            {
                AddFailure("!= Operator( Rect, Rect ) failed");
                Log("***Expected: {0}", myAnswer);
                Log("***Actual: {0}", theirAnswer);
            }
        }

        private void TestParse()
        {
            Log("Testing Rect.Parse...");

            TestParseWith("Empty");
            TestParseWith("0" + _sep + "0" + _sep + "0" + _sep + "0");
            TestParseWith("1 " + _sep + "   1  " + _sep + "0" + _sep + "1");
            TestParseWith("1 " + _sep + "   1  " + _sep + "0" + _sep + "0");
            TestParseWith(" -0.0 " + _sep + " -2.78" + _sep + "1.45 " + _sep + "2.001");
            TestParseWith(" 0.0 " + _sep + " 2.78" + _sep + "1.45 " + _sep + "2.001");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect theirAnswer = Rect.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect myAnswer = StringConverter.ToRect(invariant);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Rect.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Properties X, Y, Width, Height, Location, Size...");

            TestRWith(0, "X");
            TestRWith(1.1, "X");
            TestRWith(-1.1, "X");

            TestRWith(0, "Y");
            TestRWith(1.1, "Y");
            TestRWith(-1.1, "Y");

            TestRWith(0, "Width");
            TestRWith(1.1, "Width");

            TestRWith(0, "Height");
            TestRWith(1.1, "Height");

            TestRWith(new Point(), "Location");
            TestRWith(new Point(11.11, 22.22), "Location");
            TestRWith(new Point(-11.11, -22.22), "Location");
            TestRWith(new Point(-11.11, 22.22), "Location");

            TestRWith(Size.Empty, "Size");
            TestRWith(new Size(), "Size");
            TestRWith(new Size(11.11, 22.22), "Size");
        }

        private void TestRWith(double value, string property)
        {
            Rect rect = new Rect();
            double actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            rect = new Rect(11.11, 22.22, 11.11, 22.22);
            actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private double SetRWith(ref Rect rect, double value, string property)
        {
            switch (property)
            {
                case "X": rect.X = value; return rect.X;
                case "Y": rect.Y = value; return rect.Y;
                case "Width": rect.Width = value; return rect.Width;
                case "Height": rect.Height = value; return rect.Height;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Rect");
        }

        private void TestRWith(Point value, string property)
        {
            Rect rect = new Rect();
            Point actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            rect = new Rect(11.11, 22.22, 11.11, 22.22);
            actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }
        }

        private Point SetRWith(ref Rect rect, Point value, string property)
        {
            switch (property)
            {
                case "Location": rect.Location = value; return rect.Location;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Rect");
        }

        private void TestRWith(Size value, string property)
        {
            Rect rect;
            Size actual;
            if (value.IsEmpty) // Only an empty Size is allowed to be assigned on an empty Rect
            {
                rect = Rect.Empty;
                actual = SetRWith(ref rect, value, property);
                if (MathEx.NotEquals(value, actual) || failOnPurpose)
                {
                    AddFailure("set_" + property + " failed");
                    Log("*** Expected: {0}", value);
                    Log("*** Actual:   {0}", actual);
                }
                return;
            }

            rect = new Rect();
            actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

            rect = new Rect(11.11, 22.22, 11.11, 22.22);
            actual = SetRWith(ref rect, value, property);
            if (MathEx.NotEquals(value, actual) || failOnPurpose)
            {
                AddFailure("set_" + property + " failed");
                Log("*** Expected: {0}", value);
                Log("*** Actual:   {0}", actual);
            }

        }

        private Size SetRWith(ref Rect rect, Size value, string property)
        {
            switch (property)
            {
                case "Size": rect.Size = value; return rect.Size;
            }
            throw new ApplicationException("Invalid property: " + property + " cannot be set on Rect");
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Rect.Empty);
            TestToStringWith(new Rect());
            TestToStringWith(new Rect(11.11, 22.22, 0, 0));
            TestToStringWith(new Rect(11.11, 22.22, 0, 22.22));
            TestToStringWith(new Rect(11.11, 22.22, 11.11, 0));
            TestToStringWith(new Rect(0, 0, 11.11, 22.22));
            TestToStringWith(new Rect(-11.11, -22.22, 11.11, 22.22));
        }

        private void TestToStringWith(Rect rect)
        {
            string theirAnswer = rect.ToString();

            // Don't want these to be affected by locale yet
            string myX = rect.X.ToString(CultureInfo.InvariantCulture);
            string myY = rect.Y.ToString(CultureInfo.InvariantCulture);
            string myWidth = rect.Width.ToString(CultureInfo.InvariantCulture);
            string myHeight = rect.Height.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myX + _sep + myY + _sep + myWidth + _sep + myHeight;
            if (rect.IsEmpty)
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

            theirAnswer = rect.ToString(CultureInfo.CurrentCulture);

            if (!MathEx.EqualsIgnoringSeparators(theirAnswer,myAnswer) || failOnPurpose)
            {
                AddFailure("ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestTransform()
        {
            Log("Testing Transform...");

            TestTransformWith(Rect.Empty, Const2D.typeIdentity);  // Identity
            TestTransformWith(Rect.Empty, new Matrix(1, 0, 0, 1, 11.11, -22.22));   // translate
            TestTransformWith(Rect.Empty, new Matrix(11.11, 0, 0, -0.22, 0, 0));    // scale
            TestTransformWith(Rect.Empty, new Matrix(11.11, 0, 0, -0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(Rect.Empty, new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66)); // unknown

            TestTransformWith(new Rect(), Const2D.typeIdentity);  // Identity
            TestTransformWith(new Rect(), new Matrix(1, 0, 0, 1, 11.11, -22.22));   // translate
            TestTransformWith(new Rect(), new Matrix(-11.11, 0, 0, -0.22, 0, 0));    // scale
            TestTransformWith(new Rect(), new Matrix(11.11, 0, 0, 0.22, 0, 0));    // scale
            TestTransformWith(new Rect(), new Matrix(-11.11, 0, 0, -0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(), new Matrix(11.11, 0, 0, 0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66)); // unknown

            TestTransformWith(new Rect(11.11, 22.22, 0, 0), Const2D.typeIdentity);  // Identity
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(1, 0, 0, 1, 11.11, -22.22));   // translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(-11.11, 0, 0, -0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(11.11, 0, 0, 0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(-11.11, 0, 0, -0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(11.11, 0, 0, 0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66)); // unknown

            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), Const2D.typeIdentity);  // Identity
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(1, 0, 0, 1, 11.11, -22.22));   // translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(-11.11, 0, 0, -0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(11.11, 0, 0, 0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(-11.11, 0, 0, -0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(11.11, 0, 0, 0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 0, 22.22), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66)); // unknown

            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), Const2D.typeIdentity);  // Identity
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(1, 0, 0, 1, 11.11, -22.22));   // translate
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(-11.11, 0, 0, -0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(11.11, 0, 0, 0.22, 0, 0));    // scale
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(-11.11, 0, 0, -0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(11.11, 0, 0, 0.22, 11.11, -0.22));    // scale | translate
            TestTransformWith(new Rect(11.11, 22.22, 11.11, 0), new Matrix(11.11, -22.22, 33.33, -44.44, 55.55, -66.66)); // unknown
        }

        private void TestTransformWith(Rect rect, Matrix matrix)
        {
            Rect myAnswer;
            if (rect.IsEmpty)
            {
                myAnswer = Rect.Empty;
            }
            else
            {
                Point tmpPoint1 = matrix.Transform(rect.TopLeft);
                Point tmpPoint2 = matrix.Transform(rect.TopRight);
                Point tmpPoint3 = matrix.Transform(rect.BottomLeft);
                Point tmpPoint4 = matrix.Transform(rect.BottomRight);

                double newX = MathEx.Min(tmpPoint1.X, tmpPoint2.X, tmpPoint3.X, tmpPoint4.X);
                double newY = MathEx.Min(tmpPoint1.Y, tmpPoint2.Y, tmpPoint3.Y, tmpPoint4.Y);
                double newWidth = MathEx.Max(tmpPoint1.X, tmpPoint2.X, tmpPoint3.X, tmpPoint4.X) - newX;
                double newHeight = MathEx.Max(tmpPoint1.Y, tmpPoint2.Y, tmpPoint3.Y, tmpPoint4.Y) - newY;

                myAnswer = new Rect(newX, newY, newWidth, newHeight);
            }

            Rect theirAnswer1 = rect;
            theirAnswer1.Transform(matrix);

            if (MathEx.NotCloseEnough(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Transform( Matrix ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Transform(rect, matrix);

            if (MathEx.NotCloseEnough(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Transform( Rect, Matrix ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestUnion()
        {
            Log("Testing Union...");

            TestUnionWith(Rect.Empty, new Point(2, 2));
            TestUnionWith(new Rect(), new Point());
            TestUnionWith(new Rect(10, 10, 0, 0), new Point());

            TestUnionWith(new Rect(10, 10, 0, 0), new Point(10, 10)); // same
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(15, 15));   //  inside
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(30, 30));   // outside
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(10, 10));   // upper-left corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(20, 10));   // upper-right corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(10, 20));   // lower-left corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Point(20, 20));   // lower-right corner

            TestUnionWith(Rect.Empty, Rect.Empty);
            TestUnionWith(Rect.Empty, new Rect(11.11, 22.22, 11.11, 22.22));
            TestUnionWith(new Rect(11.11, 22.22, 11.11, 22.22), Rect.Empty);
            TestUnionWith(new Rect(), new Rect());

            TestUnionWith(new Rect(10, 10, 0, 0), new Rect(10, 10, 0, 0));    // same
            TestUnionWith(new Rect(10, 10, 0, 0), new Rect(20, 20, 0, 0));

            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(10, 10, 10, 0));  // same
            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(15, 10, 2, 0));  // edge
            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(5, 10, 50, 0));
            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(30, 30, 30, 0));  // not intersected
            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(20, 10, 10, 0));  // point
            TestUnionWith(new Rect(10, 10, 10, 0), new Rect(0, 10, 10, 0));  // point

            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(10, 10, 0, 10));  // same
            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(10, 15, 0, 2));  // edge
            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(10, 5, 0, 50));
            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(30, 30, 0, 30));  // not intersected
            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(10, 20, 0, 10));  // point
            TestUnionWith(new Rect(10, 10, 0, 10), new Rect(10, 0, 0, 10));  // point

            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(10, 10, 10, 10));  // same
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(15, 15, 2, 2));  // completely inside
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 50, 50)); // completely include
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(30, 30, 30, 30));
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(10, 15, 20, 20));  // intersected
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(0, 10, 10, 10));  // share left edge
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(20, 10, 10, 10));  // share right edge
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(10, 0, 10, 10));  // share top edge
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(10, 20, 10, 10));  // share bottom edge
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(0, 0, 10, 10));  // share upper-left corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(20, 0, 10, 10));  // share upper-right corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(0, 20, 10, 10));  // share lower-left corner
            TestUnionWith(new Rect(10, 10, 10, 10), new Rect(20, 20, 10, 10));  // share lower-right corner
        }

        private void TestUnionWith(Rect rect, Point point)
        {
            double newX = Math.Min(rect.Left, point.X);
            double newY = Math.Min(rect.Top, point.Y);
            double newWidth = Math.Max(rect.Right, point.X) - newX;
            double newHeight = Math.Max(rect.Bottom, point.Y) - newY;
            Rect myAnswer = new Rect(newX, newY, newWidth, newHeight);

            Rect theirAnswer1 = rect;
            theirAnswer1.Union(point);

            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Union( Point ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Union(rect, point);

            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Union( Rect, Point ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void TestUnionWith(Rect rect1, Rect rect2)
        {
            Rect myAnswer;
            if (rect1.IsEmpty)
            {
                myAnswer = rect2;
            }
            else if (rect2.IsEmpty)
            {
                myAnswer = rect1;
            }
            else
            {
                double newX = Math.Min(rect1.Left, rect2.Left);
                double newY = Math.Min(rect1.Top, rect2.Top);
                double newWidth = Math.Max(rect1.Right, rect2.Right) - newX;
                double newHeight = Math.Max(rect1.Bottom, rect2.Bottom) - newY;
                myAnswer = new Rect(newX, newY, newWidth, newHeight);
            }

            Rect theirAnswer1 = rect1;
            theirAnswer1.Union(rect2);

            if (!MathEx.Equals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Union( Rect ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer1);
            }

            Rect theirAnswer2 = Rect.Union(rect1, rect2);

            if (!MathEx.Equals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect.Union( Rect, Rect ) failed");
                Log("***Expected: Rect = {0}", myAnswer);
                Log("***Actual: Rect = {0}", theirAnswer2);
            }
        }

        private void RunTest2()
        {
            TestConstructor2();
            TestBottom2();
            TestBottomLeft2();
            TestBottomRight2();
            TestContains2();
            TestGetHashCode2();
            TestInflate2();
            TestIntersect2();
            TestIntersectsWith2();
            TestIsEmpty2();
            TestLeft2();
            TestOffset2();
            TestOpEquality2();
            TestOpInequality2();
            TestParse2();
            TestProperties2();
            TestRight2();
            TestTop2();
            TestTopLeft2();
            TestTopRight2();
            TestToString2();
            TestUnion2();
        }

        private void TestConstructor2()
        {
            Log("P2 Testing Construtor...");

            Try(ConstructNegativeWidth, typeof(ArgumentException));
            Try(ConstructNegativeHeight, typeof(ArgumentException));
            Try(ConstructNegativeWidthHeight, typeof(ArgumentException));
            TestConstructorWith(Const2D.min, Const2D.min, Const2D.max, Const2D.max);
            TestConstructorWith(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf);
            TestConstructorWith(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan);
            TestConstructorWith(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf);  // infinite

            TestConstructorWith(new Point(Const2D.min, Const2D.min), new Size(Const2D.max, Const2D.max));
            TestConstructorWith(new Point(Const2D.posInf, Const2D.posInf), new Size(Const2D.posInf, Const2D.posInf)); // infinite
            TestConstructorWith(new Point(Const2D.nan, Const2D.nan), new Size(Const2D.nan, Const2D.nan));
            TestConstructorWith(new Point(Const2D.negInf, Const2D.negInf), new Size(Const2D.posInf, Const2D.posInf)); // infinite

            TestConstructorWith(new Point(Const2D.min, Const2D.min), new Point(Const2D.max, Const2D.max));
            TestConstructorWith(new Point(Const2D.posInf, Const2D.posInf), new Point(Const2D.posInf, Const2D.posInf));
            TestConstructorWith(new Point(Const2D.nan, Const2D.nan), new Point(Const2D.nan, Const2D.nan));
            TestConstructorWith(new Point(Const2D.negInf, Const2D.negInf), new Point(Const2D.posInf, Const2D.posInf));    // infinite

            TestConstructorWith(new Point(Const2D.min, Const2D.min), new Vector(Const2D.max, Const2D.max));
            TestConstructorWith(new Point(Const2D.posInf, Const2D.posInf), new Vector(Const2D.posInf, Const2D.posInf));
            TestConstructorWith(new Point(Const2D.nan, Const2D.nan), new Vector(Const2D.nan, Const2D.nan));
            TestConstructorWith(new Point(Const2D.negInf, Const2D.negInf), new Vector(Const2D.posInf, Const2D.posInf));    // infinite

            TestConstructorWith(new Size(Const2D.max, Const2D.max));
            TestConstructorWith(new Size(Const2D.posInf, Const2D.posInf));
            TestConstructorWith(new Size(Const2D.nan, Const2D.nan));
        }

        #region ExceptionThrowers for Constructor

        private void ConstructNegativeWidth()
        {
            Rect rect = new Rect(0, 0, -1, 0);
        }

        private void ConstructNegativeHeight()
        {
            Rect rect = new Rect(0, 0, 0, -1);
        }

        private void ConstructNegativeWidthHeight()
        {
            Rect rect = new Rect(0, 0, -1, -1);
        }

        #endregion

        private void TestBottom2()
        {
            Log("P2 Testing Bottom...");

            TestBottomWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestBottomWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestBottomWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            //TestBottomWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // infinite
        }

        private void TestBottomLeft2()
        {
            Log("P2 Testing BottomLeft...");

            TestBottomLeftWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestBottomLeftWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestBottomLeftWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            //TestBottomLeftWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // infinite
        }

        private void TestBottomRight2()
        {
            Log("P2 Testing BottomRight...");

            TestBottomRightWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestBottomRightWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestBottomRightWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            //TestBottomRightWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // infinite
        }

        private void TestContains2()
        {
            Log("P2 Testing Contains...");

            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( 10, 10 ) );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( Const2D.negInf, Const2D.negInf ) );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( Const2D.posInf, Const2D.posInf ) );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( Const2D.nan, Const2D.nan ) );
            //TestContainsWith( new Rect( Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf ), new Point( Const2D.posInf, Const2D.posInf ) );

            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), 10, 10 );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), Const2D.negInf, Const2D.negInf );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), Const2D.posInf, Const2D.posInf );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), Const2D.nan, Const2D.nan );
            //TestContainsWith( new Rect( Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf ), Const2D.posInf, Const2D.posInf );

            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( 0, 0, 10, 10 ) );
            //TestContainsWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( Const2D.negInf,Const2D.negInf, Const2D.posInf, Const2D.posInf ) );
        }

        private void TestGetHashCode2()
        {
            Log("P2 Testing GetHashCode...");

            TestGetHashCodeWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestGetHashCodeWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestGetHashCodeWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestGetHashCodeWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
        }

        private void TestInflate2()
        {
            Log("P2 Testing Inflate...");

            Try(InflateEmptyRect1, typeof(InvalidOperationException));
            Try(InflateEmptyRect2, typeof(InvalidOperationException));
            Try(InflateEmptyRect3, typeof(InvalidOperationException));

            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), 0, 0);
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), 10, 10);
            //TestInflateWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), Const2D.negInf, Const2D.negInf );
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), Const2D.posInf, Const2D.posInf);
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), Size.Empty);
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Size());
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Size(10, 10));
            //TestInflateWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Size( Const2D.negInf, Const2D.negInf ) );
            TestInflateWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Size(Const2D.posInf, Const2D.posInf));
        }

        #region ExceptionThrowers for Inflate

        private void InflateEmptyRect1()
        {
            Rect rect = Rect.Empty;
            rect.Inflate(10, 10);
        }

        private void InflateEmptyRect2()
        {
            Rect.Inflate(Rect.Empty, 10, 10);
        }

        private void InflateEmptyRect3()
        {
            Rect.Inflate(Rect.Empty, new Size(10, 10));
        }

        #endregion

        private void TestIntersect2()
        {
            Log("P2 Testing Intersect...");

            //TestIntersectWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // same infinite
            //TestIntersectWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( 10, 10, 10, 10 ) ); // infinite with finite
            //TestIntersectWith( new Rect( Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan ), new Rect( 10, 10, 10, 10 ) );
            //TestIntersectWith( new Rect( Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf ), new Rect( 10, 10, 10, 10 ) );
        }

        private void TestIntersectsWith2()
        {
            Log("P2 Testing IntersectsWith...");

            //TestIntersectsWithWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // same infinite
            //TestIntersectsWithWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( 10, 10, 10, 10 ) ); // infinite with finite
            //TestIntersectsWithWith( new Rect( Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf ), new Rect( 10, 10, 10, 10 ) );
        }

        private void TestIsEmpty2()
        {
            Log("P2 Testing IsEmpty...");

            TestIsEmptyWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestIsEmptyWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestIsEmptyWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestIsEmptyWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
        }

        private void TestLeft2()
        {
            Log("P2 Testing Left...");

            TestLeftWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestLeftWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestLeftWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestLeftWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));  // infinite
        }

        private void TestOffset2()
        {
            Log("P2 Testing Offset...");

            Try(OffsetOnEmptyRect1, typeof(InvalidOperationException));
            Try(OffsetOnEmptyRect2, typeof(InvalidOperationException));
            Try(OffsetOnEmptyRect3, typeof(InvalidOperationException));
            Try(OffsetOnEmptyRect4, typeof(InvalidOperationException));

            TestOffsetWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), Const2D.negInf, Const2D.negInf);
            //TestOffsetWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), Const2D.posInf, Const2D.posInf );
            TestOffsetWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Vector(Const2D.negInf, Const2D.negInf));
            //TestOffsetWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Vector( Const2D.posInf, Const2D.posInf ) );
        }

        #region ExceptionThrowers for Location

        private void OffsetOnEmptyRect1()
        {
            Rect rect = Rect.Empty;
            rect.Offset(10, 10);
        }

        private void OffsetOnEmptyRect2()
        {
            Rect.Offset(Rect.Empty, 10, 10);
        }

        private void OffsetOnEmptyRect3()
        {
            Rect.Offset(Rect.Empty, new Vector(10, 10));
        }

        private void OffsetOnEmptyRect4()
        {
            Rect rect = Rect.Empty;
            rect.Offset(new Vector(10, 10));
        }

        #endregion

        private void TestOpEquality2()
        {
            Log("P2 Testing == Operator( Rect, Rect )...");

            TestOpEqualityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpEqualityWith(new Rect(Const2D.max, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpEqualityWith(new Rect(Const2D.negInf, Const2D.max, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpEqualityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.max, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpEqualityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.max), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpEqualityWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan), new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
        }

        private void TestOpInequality2()
        {
            Log("P2 Testing != Operator( Rect, Rect )...");

            TestOpInequalityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpInequalityWith(new Rect(Const2D.max, Const2D.negInf, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpInequalityWith(new Rect(Const2D.negInf, Const2D.max, Const2D.posInf, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpInequalityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.max, Const2D.posInf), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpInequalityWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.max), new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));
            TestOpInequalityWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan), new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
        }

        private void TestParse2()
        {
            Log("P2 Testing Rect.Parse...");

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
            Rect rect = Rect.Parse(global);
        }

        private void ParseTooFewParams()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "33.33";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect rect = Rect.Parse(global);
        }

        private void ParseTooManyParams1()
        {
            string s = "11.11" + _sep + "-22.22" + _sep + "33.33" + _sep + "44.44" + _sep + "55.55";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect rect = Rect.Parse(global);
        }

        private void ParseTooManyParams2()
        {
            string s = "Empty" + _sep + "-22.22" + _sep + "33.33" + _sep + "44.44";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect rect = Rect.Parse(global);
        }

        private void ParseWrongFormat1()
        {
            string s = "11.11" + _sep + "Empty" + _sep + "33.33" + _sep + "44.44";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect rect = Rect.Parse(global);
        }

        private void ParseWrongFormat2()
        {
            string s = "11.11" + _sep + "a" + _sep + "33.33" + _sep + "44.44";
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect rect = Rect.Parse(global);
        }

        #endregion

        private void TestProperties2()
        {
            Log("P2 Testing Properties X, Y, Width, Height, Location, Size...");

            TestRWith(Const2D.min, "X");
            TestRWith(Const2D.max, "X");
            TestRWith(Const2D.negInf, "X");
            TestRWith(Const2D.posInf, "X");
            TestRWith(Const2D.nan, "X");
            Try(SetXOnEmptyRect, typeof(InvalidOperationException));

            TestRWith(Const2D.min, "Y");
            TestRWith(Const2D.max, "Y");
            TestRWith(Const2D.negInf, "Y");
            TestRWith(Const2D.posInf, "Y");
            TestRWith(Const2D.nan, "Y");
            Try(SetYOnEmptyRect, typeof(InvalidOperationException));

            TestRWith(Const2D.max, "Width");
            TestRWith(Const2D.posInf, "Width");
            TestRWith(Const2D.nan, "Width");
            Try(SetWidthOnEmptyRect, typeof(InvalidOperationException));
            Try(SetNegativeWidth, typeof(InvalidOperationException));


            TestRWith(Const2D.max, "Height");
            TestRWith(Const2D.posInf, "Height");
            TestRWith(Const2D.nan, "Height");
            Try(SetHeightOnEmptyRect, typeof(InvalidOperationException));
            Try(SetNegativeHeight, typeof(InvalidOperationException));

            TestRWith(new Point(Const2D.min, Const2D.min), "Location");
            TestRWith(new Point(Const2D.max, Const2D.max), "Location");
            TestRWith(new Point(Const2D.negInf, Const2D.negInf), "Location");
            TestRWith(new Point(Const2D.posInf, Const2D.posInf), "Location");
            TestRWith(new Point(Const2D.nan, Const2D.nan), "Location");
            Try(SetLocationOnEmptyRect, typeof(InvalidOperationException));

            TestRWith(new Size(Const2D.max, Const2D.max), "Size");
            TestRWith(new Size(Const2D.posInf, Const2D.posInf), "Size");
            TestRWith(new Size(Const2D.nan, Const2D.nan), "Size");
            Try(SetNonEmptySizeOnEmptyRect, typeof(InvalidOperationException));
        }

        #region ExceptionThrowers for Properties

        private void SetXOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.X = 10;
        }

        private void SetYOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.Y = 10;
        }

        private void SetWidthOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.Width = 10;
        }

        private void SetHeightOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.Height = 10;
        }

        private void SetLocationOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.Location = new Point(10, 10);
        }

        private void SetNegativeWidth()
        {
            Rect rect = Rect.Empty;
            rect.Width = -10;
        }

        private void SetNegativeHeight()
        {
            Rect rect = Rect.Empty;
            rect.Height = -10;
        }

        private void SetNonEmptySizeOnEmptyRect()
        {
            Rect rect = Rect.Empty;
            rect.Size = new Size();
        }

        #endregion

        private void TestRight2()
        {
            Log("P2 Testing Right...");

            TestRightWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestRightWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestRightWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            //TestRightWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // infinite
        }

        private void TestTop2()
        {
            Log("P2 Testing Top...");

            TestTopWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestTopWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestTopWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestTopWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));  // infinite
        }

        private void TestTopLeft2()
        {
            Log("P2 Testing TopLeft...");

            TestTopLeftWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestTopLeftWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestTopLeftWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestTopLeftWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));  // infinite
        }

        private void TestTopRight2()
        {
            Log("P2 Testing TopRight...");

            TestTopRightWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestTopRightWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestTopRightWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            //TestTopRightWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );  // infinite
        }

        private void TestToString2()
        {
            Log("P2 Testing ToString...");

            TestToStringWith(new Rect(Const2D.min, Const2D.min, Const2D.max, Const2D.max));
            TestToStringWith(new Rect(Const2D.posInf, Const2D.posInf, Const2D.posInf, Const2D.posInf));
            TestToStringWith(new Rect(Const2D.nan, Const2D.nan, Const2D.nan, Const2D.nan));
            TestToStringWith(new Rect(Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf));  // infinite
        }

        private void TestUnion2()
        {
            Log("P2 Testing Union...");

            //TestUnionWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( 10, 10 ) );
            //TestUnionWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Point( Const2D.posInf, Const2D.posInf ) );
            //TestUnionWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( 10, 10, 10, 10 ) );
            //TestUnionWith( new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ), new Rect( Const2D.negInf, Const2D.negInf, Const2D.posInf, Const2D.posInf ) );
        }
    }
}