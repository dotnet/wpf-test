// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *   Description:   Test Rect3D API
 *
 * Bad Parameter testing notes:
 *  ( location == -inf ) with ( size != inf ) intersects/contains nothing
 *      * try union with anything and see what happens
 *  ( location == x ) with ( size != inf ) can only intersect/contain [x,MaxValue+x]
 *      i.e. if ( x == MinValue ), it can only intersect/contain [MinValue,0]
 *
 * There's not much we can do about precision problems.
 *  Consider ( location == MinValue ) and ( size == relatively small value ):
 *      The size will be lost during the intersection test because a small value
 *      cannot be stored due to the limited precision of doubles.
 *
 ************************************************************/

using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class Rect3DTest : CoreGraphicsTest
    {
        private char _sep = Const.valueSeparator;
        private double _max = Const.max;
        private double _min = Const.min;
        private double _inf = Const.inf;
        private double _nan = Const.nan;
        private double _eps = Const.eps;
        private Rect3D _empty = Rect3D.Empty;

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestCtor();
                TestIsEmpty();
                TestLocationAndSize();
                TestProperties();
                TestEquality();
                TestContains();
                TestIntersect();
                TestOffset();
                TestUnion();
                TestParse();
                TestToString();
                TestGetHashCode();
            }
            else // priority > 0
            {
                RunTheTest2();
            }
        }

        private void TestCtor()
        {
            Log("Testing Ctors...");

            Rect3D rect = new Rect3D();

            if (MathEx.NotEquals(rect.X, 0) || MathEx.NotEquals(rect.Y, 0) ||
                 MathEx.NotEquals(rect.Z, 0) || MathEx.NotEquals(rect.SizeX, 0) ||
                 MathEx.NotEquals(rect.SizeY, 0) || MathEx.NotEquals(rect.SizeZ, 0) ||
                 failOnPurpose)
            {
                AddFailure("Rect3D.ctor() failed");
                Log("*** Expected: 0,0,0, 0,0,0");
                Log("***   Actual: {0}", rect);
            }

            TestCtorWith(Const.p0, Const.s0);
            TestCtorWith(Const.pEps, Const.sMax);
            TestCtorWith(Const.p0, Const.sEmpty);
            TestCtorWith(0, 0, 0, 0, 0, 0);
            TestCtorWith(3, 4, 5, 3, 3, 3);
        }

        private void TestCtorWith(Point3D p, Size3D size)
        {
            Rect3D r = new Rect3D(p, size);

            if (size.IsEmpty)
            {
                if (!r.IsEmpty || failOnPurpose)
                {
                    AddFailure("2-param ctor to make empty Rect3D failed");
                    Log("*** Expected: Size = {0}", size);
                    Log("*** Actual:   Size = {0}", r.Size);
                }
            }
            else if (MathEx.NotEquals(r.Location, p) || MathEx.NotEquals(r.Size, size) ||
                      failOnPurpose)
            {
                AddFailure("2-param ctor for Rect3D failed");
                Log("*** Expected: Location = {0}, Size = {1}", p, size);
                Log("*** Actual:   Location = {0}, Size = {1}", r.Location, r.Size);
            }
        }

        private void TestCtorWith(double x, double y, double z,
                                              double xOffset, double yOffset, double zOffset)
        {
            Rect3D r = new Rect3D(x, y, z, xOffset, yOffset, zOffset);

            if (MathEx.NotEquals(r.X, x) || MathEx.NotEquals(r.Y, y) ||
                 MathEx.NotEquals(r.Z, z) || MathEx.NotEquals(r.SizeX, xOffset) ||
                 MathEx.NotEquals(r.SizeY, yOffset) || MathEx.NotEquals(r.SizeZ, zOffset) ||
                 failOnPurpose)
            {
                AddFailure("6-param ctor for Rect3D failed");
                Log("*** Expected: ( {0}, {1}, {2}, {3}, {4}, {5} )", x, y, z, xOffset, yOffset, zOffset);
                Log("*** Actual:   ( {0}, {1}, {2}, {3}, {4}, {5} )", r.X, r.Y, r.Z, r.SizeX, r.SizeY, r.SizeZ);
            }
        }

        private void TestIsEmpty()
        {
            Log("Testing IsEmpty...");

            TestIsEmptyWith(Const.containsAll);
            TestIsEmptyWith(Const.rOrigin0);
            TestIsEmptyWith(Rect3D.Empty);
        }

        private void TestIsEmptyWith(Rect3D r)
        {
            bool theirAnswer = r.IsEmpty;
            bool myAnswer = double.IsPositiveInfinity(r.X) &&
                            double.IsPositiveInfinity(r.Y) &&
                            double.IsPositiveInfinity(r.Z) &&
                            double.IsNegativeInfinity(r.SizeX) &&
                            double.IsNegativeInfinity(r.SizeY) &&
                            double.IsNegativeInfinity(r.SizeZ);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("IsEmpty failed");
                Log("*** Rect     = " + r);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer);
            }
        }

        private void TestLocationAndSize()
        {
            Log("Testing Location and Size...");

            TestLocationAndSizeWith(Const.p0, Const.s0);
            TestLocationAndSizeWith(Const.p10, Const.s10);
            TestLocationAndSizeWith(Const.pMax, Const.s1);
            TestLocationAndSizeWith(Const.p0, Const.sEmpty);
        }

        private void TestLocationAndSizeWith(Point3D p, Size3D s)
        {
            Rect3D r = new Rect3D();

            r.Location = p;
            if (MathEx.NotEquals(r.Location, p) || failOnPurpose)
            {
                AddFailure("Location get/set failed");
                Log("*** Expected = " + p);
                Log("*** Actual   = " + r.Location);
            }

            r.Size = s;
            if (s.IsEmpty)
            {
                if (MathEx.NotEquals(r, Rect3D.Empty) || failOnPurpose)
                {
                    AddFailure("Setting size to Empty created a non-empty Rect3D");
                    Log("*** Expected = " + Rect3D.Empty);
                    Log("*** Actual   = " + r);
                }

                // I can't do anything else with this test case.  Exit.
                return;
            }
            else if (MathEx.NotEquals(r.Size, s) || failOnPurpose)
            {
                AddFailure("Size get/set failed");
                Log("*** Expected = " + s);
                Log("*** Actual   = " + r.Size);
            }
        }

        private void TestProperties()
        {
            Log("Testing get/set [Size]XYZ...");

            TestPropertiesWith(0, 0, 0, 0, 0, 0);
            TestPropertiesWith(10, 10, 10, 10, 10, 10);
            TestPropertiesWith(9120.1234, 12839401234, 128394.123489, 123, 1234.1234, 1234.1234);
            TestPropertiesWith(_max, _eps, _max, _eps, _max, _eps);
        }

        private void TestPropertiesWith(double x, double y, double z,
                                                    double sizeX, double sizeY, double sizeZ)
        {
            Rect3D r = new Rect3D();

            r.X = x;
            r.Y = y;
            r.Z = z;
            r.SizeX = sizeX;
            r.SizeY = sizeY;
            r.SizeZ = sizeZ;

            if (MathEx.NotEquals(r.X, x) || MathEx.NotEquals(r.Y, y) ||
                 MathEx.NotEquals(r.Z, z) || MathEx.NotEquals(r.SizeX, sizeX) ||
                 MathEx.NotEquals(r.SizeY, sizeY) || MathEx.NotEquals(r.SizeZ, sizeZ) ||
                 failOnPurpose)
            {
                AddFailure("get/set [Size]XYZ failed");
                Log("*** Expected = {0},{1},{2},{3},{4},{5}", x, y, z, sizeX, sizeY, sizeZ);
                Log("*** Actual   = {0},{1},{2},{3},{4},{5}", r.X, r.Y, r.Z, r.SizeX, r.SizeY, r.SizeZ);
            }
        }

        private void TestEquality()
        {
            Log("Testing Equality operators");

            TestEqualityWith(Const.rOrigin0, Const.rOrigin0);
            TestEqualityWith(Const.rNegative, Const.rPositive);
            TestEqualityWith(Rect3D.Empty, Rect3D.Empty);
            TestEqualityWith(Rect3D.Empty, Const.rOrigin0);
            TestEqualityWith(Const.rOrigin1, Const.rOrigin0);
        }

        private void TestEqualityWith(Rect3D r1, Rect3D r2)
        {
            bool theirAnswer1 = r1.Equals(r2);
            bool theirAnswer2 = Rect3D.Equals(r1, r2);
            bool theirAnswer3 = r1 == r2;
            bool theirNotAnswer3 = r1 != r2;
            bool myAnswer12 = !MathEx.NotEquals(r1, r2);
            bool myAnswer3 = !MathEx.ClrOperatorNotEquals(r1, r2);

            if (theirAnswer1 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Equals( Rect3D ) failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer12);
                Log("*** Actual:   " + theirAnswer1);
            }
            if (theirAnswer2 != myAnswer12 || failOnPurpose)
            {
                AddFailure("Static Equals( Rect3D,Rect3D ) failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer12);
                Log("*** Actual:   " + theirAnswer2);
            }
            if (theirAnswer3 != myAnswer3 || failOnPurpose)
            {
                AddFailure("operator == failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer3);
                Log("*** Actual:   " + theirAnswer3);
            }
            if (theirNotAnswer3 == myAnswer3 || failOnPurpose)
            {
                AddFailure("operator != failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + !myAnswer3);
                Log("*** Actual:   " + theirNotAnswer3);
            }

            if (priority > 0)
            {
                bool theirAnswer4 = r1.Equals(null);
                bool theirAnswer5 = r1.Equals(true);

                if (theirAnswer4 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Rect   = ( {0} )", r1);
                    Log("*** object = null");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer4);
                }

                if (theirAnswer5 != false || failOnPurpose)
                {
                    AddFailure("Equals( object ) failed.");
                    Log("*** Rect   = ( {0} )", r1);
                    Log("*** object = ( true )");
                    Log("*** Expected = false");
                    Log("*** Actual   = {0}", theirAnswer5);
                }
            }
        }

        private void TestContains()
        {
            Log("Testing Contains...");

            TestContainsWith(Const.containsAll, Const.rOrigin0);
            TestContainsWith(Const.containsAll, Const.rNegative);
            TestContainsWith(Const.containsAll, Const.containsAll);
            TestContainsWith(Const.rOrigin0, Const.rOrigin0);
            TestContainsWith(Const.disjoint1, Const.disjoint2);
            TestContainsWith(Const.cornersTouch1, Const.cornersTouch2);
            TestContainsWith(Const.overlap1, Const.overlap2);
            TestContainsWith(Rect3D.Empty, Const.containsAll);
            TestContainsWith(Const.containsAll, Rect3D.Empty);

            TestContainsWith(Const.rOrigin1, Const.p0 - Const.vEps);
            TestContainsWith(Const.containsAll, Const.p0);
            TestContainsWith(Const.rOrigin1, Const.pEps);
            TestContainsWith(Rect3D.Empty, Const.p0);
        }

        private void TestContainsWith(Rect3D r1, Rect3D r2)
        {
            bool theirAnswer = r1.Contains(r2);

            //    +-------+     Rect3D has LeftBottomBack (LBB-hidden in picture)
            //   /  r1   /|            and RightTopFront  (RTF-the 'O' vertex)
            //  +-------O |
            //  |  +--+ | |     r1 contains r2 if: ( r1.LBB <= r2.LBB && r2.RTF <= r1.RTF )
            //  | +--O| | |
            //  | |r2|+ | +
            //  | +--+  |/
            //  +-------+

            Point3D leftBottomBack1 = MathEx.LeftBottomBack(r1);
            Point3D rightTopFront1 = MathEx.RightTopFront(r1);
            Point3D leftBottomBack2 = MathEx.LeftBottomBack(r2);
            Point3D rightTopFront2 = MathEx.RightTopFront(r2);

            bool myAnswer = (r1.IsEmpty || r2.IsEmpty)
                                ? false
                                : (MathEx.LessThanOrEquals(leftBottomBack1, leftBottomBack2) &&
                                    MathEx.LessThanOrEquals(rightTopFront2, rightTopFront1));

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Contains with Rect3D failed");
                Log("*** Test:     Rect1 = {0}, Rect2 = {1}", r1, r2);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + theirAnswer);
            }
        }

        private void TestContainsWith(Rect3D r, Point3D p)
        {
            bool b1 = r.Contains(p);
            bool b2 = r.Contains(p.X, p.Y, p.Z);

            //    +-------+     Rect3D has LeftBottomBack (LBB-hidden in picture)
            //   /   r   /|            and RightTopFront  (RTF-the 'O' vertex)
            //  +-------O |
            //  |       | |     r contains p if: ( r.LBB <= p && p <= r.RTF )
            //  |   p   | |
            //  |   .   | +
            //  |       |/
            //  +-------+

            Point3D leftBottomBack = MathEx.LeftBottomBack(r);
            Point3D rightTopFront = MathEx.RightTopFront(r);

            bool myAnswer = (r.IsEmpty)
                                ? false
                                : (MathEx.LessThanOrEquals(leftBottomBack, p) &&
                                    MathEx.LessThanOrEquals(p, rightTopFront));

            if (b1 != myAnswer || failOnPurpose)
            {
                AddFailure("Contains with Point3D failed");
                Log("*** Test:     Rect = {0}, Point3D = {1}", r, p);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + b1);
            }

            if (b2 != myAnswer || failOnPurpose)
            {
                AddFailure("Contains with x,y,z failed");
                Log("*** Test:     Rect = {0}, x,y,z = {1},{2},{3}", r, p.X, p.Y, p.Z);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + b2);
            }
        }

        private void TestIntersect()
        {
            Log("Testing Intersect...");

            TestIntersectWith(Const.containsAll, Const.rOrigin1);
            TestIntersectWith(Const.overlap1, Const.overlap2);
            TestIntersectWith(Const.cornersTouch1, Const.cornersTouch2);
            TestIntersectWith(Const.disjoint1, Const.disjoint2);
            TestIntersectWith(Rect3D.Empty, Const.containsAll);
            TestIntersectWith(Const.containsAll, Rect3D.Empty);
        }

        private void TestIntersectWith(Rect3D r1, Rect3D r2)
        {
            Rect3D theirAnswer1 = r1;
            Rect3D theirAnswer2 = r2;
            theirAnswer1.Intersect(r2);
            theirAnswer2.Intersect(r1);

            Rect3D theirAnswer3 = Rect3D.Intersect(r1, r2);
            Rect3D myAnswer;
            bool intersectsWith = false;

            if (r1.IsEmpty || r2.IsEmpty)
            {
                myAnswer = Rect3D.Empty;
            }
            else
            {
                Point3D leftBottomBack1 = MathEx.LeftBottomBack(r1);
                Point3D leftBottomBack2 = MathEx.LeftBottomBack(r2);
                Point3D rightTopFront1 = MathEx.RightTopFront(r1);
                Point3D rightTopFront2 = MathEx.RightTopFront(r2);

                double x0 = Math.Max(leftBottomBack1.X, leftBottomBack2.X);
                double y0 = Math.Max(leftBottomBack1.Y, leftBottomBack2.Y);
                double z0 = Math.Max(leftBottomBack1.Z, leftBottomBack2.Z);
                double x1 = Math.Min(rightTopFront1.X, rightTopFront2.X);
                double y1 = Math.Min(rightTopFront1.Y, rightTopFront2.Y);
                double z1 = Math.Min(rightTopFront1.Z, rightTopFront2.Z);

                double sizeX = x1 - x0;
                double sizeY = y1 - y0;
                double sizeZ = z1 - z0;

                if (sizeX >= 0 && sizeY >= 0 && sizeZ >= 0)
                {
                    myAnswer = new Rect3D(x0, y0, z0, sizeX, sizeY, sizeZ);
                    intersectsWith = true;
                }
                else
                {
                    myAnswer = Rect3D.Empty;
                }
            }

            if (MathEx.NotEquals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Intersect (r1 ^ r2) failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + theirAnswer1);
            }
            if (MathEx.NotEquals(theirAnswer2, theirAnswer1) || failOnPurpose)
            {
                AddFailure("Intersect failed - r1.Intersect(r2) should equal r2.Intersect(r1)");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** r1 ^ r2 = " + theirAnswer1);
                Log("*** r2 ^ r1 = " + theirAnswer2);
            }
            if (MathEx.NotEquals(theirAnswer3, theirAnswer1) || failOnPurpose)
            {
                AddFailure("Static Intersect should give the same result as r1.Intersect(r2)");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + theirAnswer3);
            }
            if (r1.IntersectsWith(r2) != intersectsWith || failOnPurpose)
            {
                AddFailure("IntersectsWith( Rect3D ) failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected = " + intersectsWith);
                Log("*** Actual   = " + !intersectsWith);
            }
        }

        private void TestOffset()
        {
            Log("Testing Offset...");

            TestOffsetWith(Const.rOrigin1, Const.v0);
            TestOffsetWith(Const.rOrigin0, Const.v10);
            TestOffsetWith(Const.rPositive, Const.vNeg1);
        }

        private void TestOffsetWith(Rect3D r, Vector3D v)
        {
            Rect3D theirAnswer1 = r;
            Rect3D theirAnswer2 = r;
            Rect3D theirAnswer3 = Rect3D.Offset(r, v);
            Rect3D theirAnswer4 = Rect3D.Offset(r, v.X, v.Y, v.Z);
            theirAnswer1.Offset(v);
            theirAnswer2.Offset(v.X, v.Y, v.Z);

            Rect3D myAnswer = new Rect3D(r.X + v.X, r.Y + v.Y, r.Z + v.Z, r.SizeX, r.SizeY, r.SizeZ);

            if (MathEx.NotEquals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Offset( Vector3D ) failed");
                Log("*** Rect     = " + r);
                Log("*** Vector3D = " + v);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer1);
            }
            if (MathEx.NotEquals(theirAnswer2, myAnswer) || failOnPurpose)
            {
                AddFailure("Offset( double,double,double ) failed");
                Log("*** Rect     = " + r);
                Log("*** dx,dy,dz = {0},{1},{2}", v.X, v.Y, v.Z);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer2);
            }
            if (MathEx.NotEquals(theirAnswer3, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect3D.Offset( Rect3D,Vector3D ) failed");
                Log("*** Rect     = " + r);
                Log("*** dx,dy,dz = {0},{1},{2}", v.X, v.Y, v.Z);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer3);
            }
            if (MathEx.NotEquals(theirAnswer4, myAnswer) || failOnPurpose)
            {
                AddFailure("Rect3D.Offset( Rect3D,double,double,double ) failed");
                Log("*** Rect     = " + r);
                Log("*** dx,dy,dz = {0},{1},{2}", v.X, v.Y, v.Z);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer4);
            }
        }

        private void TestUnion()
        {
            Log("Testing Union...");

            TestUnionWith(Const.containsAll, Const.rOrigin1);
            TestUnionWith(Const.cornersTouch1, Const.cornersTouch2);
            TestUnionWith(Const.disjoint1, Const.disjoint2);
            TestUnionWith(Const.overlap1, Const.overlap2);
            TestUnionWith(Const.rPositive, Rect3D.Empty);
            TestUnionWith(Rect3D.Empty, Const.rNegative);
            TestUnionWith(Rect3D.Empty, Rect3D.Empty);

            TestUnionWith(Const.containsAll, Const.p0);
            TestUnionWith(Const.rOrigin1, Const.p0 - Const.vEps);
            TestUnionWith(Const.rOrigin1, Const.p10);
            TestUnionWith(Const.rPositive, Const.pNeg1);

        }

        private void TestUnionWith(Rect3D r1, Rect3D r2)
        {
            Rect3D theirAnswer1 = r1;
            Rect3D theirAnswer2 = r2;
            theirAnswer1.Union(r2);
            theirAnswer2.Union(r1);

            Rect3D theirAnswer3 = Rect3D.Union(r1, r2);

            Rect3D myAnswer;

            if (r1.IsEmpty)
            {
                myAnswer = r2;
            }
            else if (r2.IsEmpty)
            {
                myAnswer = r1;
            }
            else
            {
                Point3D leftBottomBack1 = MathEx.LeftBottomBack(r1);
                Point3D leftBottomBack2 = MathEx.LeftBottomBack(r2);
                Point3D rightTopFront1 = MathEx.RightTopFront(r1);
                Point3D rightTopFront2 = MathEx.RightTopFront(r2);

                double x0 = Math.Min(leftBottomBack1.X, leftBottomBack2.X);
                double y0 = Math.Min(leftBottomBack1.Y, leftBottomBack2.Y);
                double z0 = Math.Min(leftBottomBack1.Z, leftBottomBack2.Z);
                double x1 = Math.Max(rightTopFront1.X, rightTopFront2.X);
                double y1 = Math.Max(rightTopFront1.Y, rightTopFront2.Y);
                double z1 = Math.Max(rightTopFront1.Z, rightTopFront2.Z);

                double sizeX = MathEx.Equals(x1, _inf) ? _inf : x1 - x0;
                double sizeY = MathEx.Equals(y1, _inf) ? _inf : y1 - y0;
                double sizeZ = MathEx.Equals(z1, _inf) ? _inf : z1 - z0;

                myAnswer = new Rect3D(x0, y0, z0, sizeX, sizeY, sizeZ);
            }

            if (MathEx.NotEquals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Union (r1 U r2) failed");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + theirAnswer1);
            }
            if (MathEx.NotEquals(theirAnswer2, theirAnswer1) || failOnPurpose)
            {
                AddFailure("Union wasn't commutative - r1.Union(r2) should equal r2.Union(r1)");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** r1 U r2 = " + theirAnswer1);
                Log("*** r2 U r1 = " + theirAnswer2);
            }
            if (MathEx.NotEquals(theirAnswer3, theirAnswer1) || failOnPurpose)
            {
                AddFailure("Static Union should give the same result as r1.Union(r2)");
                Log("*** Rect1   = " + r1);
                Log("*** Rect2   = " + r2);
                Log("*** Expected: " + myAnswer);
                Log("*** Actual:   " + theirAnswer3);
            }
        }

        private void TestUnionWith(Rect3D r, Point3D p)
        {
            Rect3D theirAnswer1 = r;
            theirAnswer1.Union(p);
            Rect3D theirAnswer2 = Rect3D.Union(r, p);

            Point3D leftBottomBack = MathEx.LeftBottomBack(r);
            Point3D rightTopFront = MathEx.RightTopFront(r);

            Rect3D myAnswer;

            if (r.IsEmpty)
            {
                myAnswer = new Rect3D(p, Const.s0);
            }
            else
            {
                double x0 = Math.Min(leftBottomBack.X, p.X);
                double y0 = Math.Min(leftBottomBack.Y, p.Y);
                double z0 = Math.Min(leftBottomBack.Z, p.Z);
                double x1 = Math.Max(rightTopFront.X, p.X);
                double y1 = Math.Max(rightTopFront.Y, p.Y);
                double z1 = Math.Max(rightTopFront.Z, p.Z);

                double sizeX = MathEx.Equals(x1, _inf) ? _inf : x1 - x0;
                double sizeY = MathEx.Equals(y1, _inf) ? _inf : y1 - y0;
                double sizeZ = MathEx.Equals(z1, _inf) ? _inf : z1 - z0;

                myAnswer = new Rect3D(x0, y0, z0, sizeX, sizeY, sizeZ);
            }

            if (MathEx.NotEquals(theirAnswer1, myAnswer) || failOnPurpose)
            {
                AddFailure("Union( Point3D ) failed");
                Log("*** Rect     = " + r);
                Log("*** Point3D  = " + p);
                Log("*** Expected = " + myAnswer);
                Log("*** Actual   = " + theirAnswer1);
            }
            if (MathEx.NotEquals(theirAnswer1, theirAnswer2) || failOnPurpose)
            {
                AddFailure("Static Union should give the same result as r.Union( p )");
                Log("*** Rect     = " + r);
                Log("*** Point3D  = " + p);
                Log("*** Expected = " + theirAnswer1);
                Log("*** Actual   = " + theirAnswer2);
            }
        }

        private void TestParse()
        {
            Log("Testing Parse...");

            TestParseWith("Empty");
            TestParseWith("0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0" + _sep + "0");
            TestParseWith("0.0 " + _sep + " -1.0	" + _sep + " 90" + _sep + "	.234" + _sep + " 45." + _sep + " .0");
        }

        private void TestParseWith(string s)
        {
            string global = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect3D r1 = Rect3D.Parse(global);

            string invariant = MathEx.ToLocale(s, CultureInfo.InvariantCulture);
            Rect3D r2 = StringConverter.ToRect3D(invariant);

            if (MathEx.NotEquals(r1, r2) || failOnPurpose)
            {
                AddFailure("Rect3D.Parse( string ) failed");
                Log("*** Original String = {0}", global);
                Log("*** Expected = {0}", r2);
                Log("*** Actual   = {0}", r1);
            }
        }

        private void TestToString()
        {
            Log("Testing ToString...");

            TestToStringWith(Const.rNegative);
            TestToStringWith(Const.rOrigin0);
            TestToStringWith(Const.rOrigin1);
            TestToStringWith(Const.rPositive);
            TestToStringWith(Rect3D.Empty);
        }

        private void TestToStringWith(Rect3D r)
        {
            string theirAnswer = r.ToString();

            // Don't want these to be affected by locale yet
            string myX = r.X.ToString(CultureInfo.InvariantCulture);
            string myY = r.Y.ToString(CultureInfo.InvariantCulture);
            string myZ = r.Z.ToString(CultureInfo.InvariantCulture);
            string mySizeX = r.SizeX.ToString(CultureInfo.InvariantCulture);
            string mySizeY = r.SizeY.ToString(CultureInfo.InvariantCulture);
            string mySizeZ = r.SizeZ.ToString(CultureInfo.InvariantCulture);

            // ... Because of this
            string myAnswer = myX + _sep + myY + _sep + myZ + _sep + mySizeX + _sep + mySizeY + _sep + mySizeZ;
            if (r.IsEmpty)
            {
                myAnswer = "Empty";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Rect3D.ToString() failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = r.ToString(CultureInfo.CurrentCulture);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Rect3D.ToString( IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)r).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            // Don't want these to be affected by locale yet
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            myX = r.X.ToString("N", numberFormat);
            myY = r.Y.ToString("N", numberFormat);
            myZ = r.Z.ToString("N", numberFormat);
            mySizeX = r.SizeX.ToString("N", numberFormat);
            mySizeY = r.SizeY.ToString("N", numberFormat);
            mySizeZ = r.SizeZ.ToString("N", numberFormat);

            // ... Because of this
            myAnswer = myX + _sep + myY + _sep + myZ + _sep + mySizeX + _sep + mySizeY + _sep + mySizeZ;
            if (r.IsEmpty)
            {
                myAnswer = "Empty";
            }
            else
            {
                myAnswer = MathEx.ToLocale(myAnswer, CultureInfo.CurrentCulture);
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("Rect3D.ToString( string,IFormatProvider ) failed");
                Log("*** Expected = {0}", myAnswer);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestGetHashCode()
        {
            Log("Testing GetHashCode...");

            int hash1 = Const.rOrigin0.GetHashCode();
            int hash2 = Rect3D.Empty.GetHashCode();
            int hash3 = Const.rOriginMax.GetHashCode();
            int hash4 = Const.rMinInf.GetHashCode();

            if ((hash1 == hash2) && (hash2 == hash3) && (hash3 == hash4) || failOnPurpose)
            {
                AddFailure("GetHashCode failed");
                Log("*** Expected hash function to generate unique hashes.");
            }
        }

        private void RunTheTest2()
        {
            TestCtor2();
            TestEquality2();
            TestContains2();
            TestUnion2();
            TestIntersect2();
            TestLocationAndSize2();
            TestProperties2();
            TestOffset2();
            TestParse2();
            TestToString2();
        }

        private void TestCtor2()
        {
            Log("Testing Constructors with Bad Params...");

            Try(ConstructorNegativeX, typeof(ArgumentException));
            Try(ConstructorNegativeY, typeof(ArgumentException));
            Try(ConstructorNegativeZ, typeof(ArgumentException));
            Try(ConstructorNegativeInf, typeof(ArgumentException));
            TestCtorWith(0, 0, 0, _nan, _nan, _nan);
            TestCtorWith(0, 0, 0, _inf, _inf, _inf);
            TestCtorWith(_nan, _nan, _nan, _nan, _nan, _nan);
            TestCtorWith(_inf, _inf, _inf, _inf, _inf, _inf);
            TestCtorWith(Const.p0, Const.sNaN);
            TestCtorWith(Const.p0, Const.sInf);
            TestCtorWith(Const.pNaN, Const.sNaN);
            TestCtorWith(Const.pNaN, Const.sInf);
        }

        #region ExceptionThrowers for Rect3D constructor

        private void ConstructorNegativeX() { Rect3D r = new Rect3D(0, 0, 0, -1, 0, 0); }
        private void ConstructorNegativeY() { Rect3D r = new Rect3D(0, 0, 0, 0, -1, 0); }
        private void ConstructorNegativeZ() { Rect3D r = new Rect3D(0, 0, 0, 0, 0, -1); }
        private void ConstructorNegativeInf() { Rect3D r = new Rect3D(0, 0, 0, -_inf, -_inf, -_inf); }

        #endregion

        private void TestEquality2()
        {
            Log("Testing Equality with Bad Params...");

            //TestEqualityWith( Const.rNaN0, Const.rNaN0 );
            TestEqualityWith(Const.r0NaN, Const.r1NaN);
            TestEqualityWith(Const.r0Inf, Const.r0Inf);
            TestEqualityWith(Const.r0Inf, Const.r1Inf);
            TestEqualityWith(Rect3D.Empty, new Rect3D(Const.pNegInf, Const.sEmpty));
        }

        private void TestContains2()
        {
            Log("Testing Contains with Bad Params...");

            TestContainsWith(Const.r0Inf, Const.r1Inf);
            TestContainsWith(Const.r0NaN, Const.r1NaN);
            TestContainsWith(Const.r0NaN, Const.p0);
            TestContainsWith(new Rect3D(_min, _min, _min, _inf, _inf, _inf), Const.p0);
        }

        private void TestUnion2()
        {
            Log("Testing Union with Bad Params...");

            TestUnionWith(Rect3D.Empty, Const.p0);
            TestUnionWith(Rect3D.Empty, Const.p10);
            TestUnionWith(Const.rMinMax, Const.rOriginMax);
        }

        private void TestIntersect2()
        {
            Log("Testing Intersect with Bad Params...");
        }

        private void TestOffset2()
        {
            Log("Testing Offset with Bad Params...");

            Try(OffsetEmptyRectWithDoubles, typeof(InvalidOperationException));
            Try(OffsetEmptyRectWithVector, typeof(InvalidOperationException));
            Try(StaticOffsetEmptyRectWithDoubles, typeof(InvalidOperationException));
            Try(StaticOffsetEmptyRectWithVector, typeof(InvalidOperationException));
            TestOffsetWith(Const.r0Inf, Const.vInf);
            TestOffsetWith(Const.r0Inf, Const.vNegInf);
            TestOffsetWith(Const.r0Inf, Const.vNaN);
            TestOffsetWith(Const.r0NaN, Const.vNaN);
            TestOffsetWith(Const.r0NaN, Const.vInf);
            TestOffsetWith(Const.r0NaN, Const.vNegInf);
        }

        #region ExceptionThrowers for Rect3D constructor

        private void OffsetEmptyRectWithVector() { _empty.Offset(Const.v1); }
        private void OffsetEmptyRectWithDoubles() { _empty.Offset(0, 0, 1); }
        private void StaticOffsetEmptyRectWithVector() { Rect3D.Offset(_empty, Const.v1); }
        private void StaticOffsetEmptyRectWithDoubles() { Rect3D.Offset(_empty, 0, 0, 1); }

        #endregion

        private void TestLocationAndSize2()
        {
            Log("Testing Location and Size Properties with Bad Params...");

            Try(SetLocationOnEmptyRect, typeof(InvalidOperationException));
            Try(SetSizeOnEmptyRect, typeof(InvalidOperationException));
            Try(SetEmptySizeThenChangeLocation, typeof(InvalidOperationException));
            SafeExecute(GetLocationOfEmpty);
        }

        #region ExceptionThrowers for Rect3D Location and Size properties

        private void SetLocationOnEmptyRect() { _empty.Location = Const.p0; }
        private void SetSizeOnEmptyRect() { _empty.Size = Const.s0; }

        private void SetEmptySizeThenChangeLocation()
        {
            Rect3D r = Const.rOrigin0;
            r.Size = Const.sEmpty;
            r.Location = Const.p0;
        }

        #endregion

        #region SafeExecutionBlocks for Rect3D Location and Size properties

        private void GetLocationOfEmpty()
        {
            Rect3D r = new Rect3D(1, 2, 3, 0, 0, 0);
            r.Size = Const.sEmpty;
            if (MathEx.NotEquals(r.Location, _empty.Location) || failOnPurpose)
            {
                AddFailure("Changing rect to empty should also nullify its position");
                Log("*** Expected: " + _empty.Location);
                Log("***   Actual: " + r.Location);
            }
        }

        #endregion

        private void TestProperties2()
        {
            Log("Testing Properties with Bad Params...");

            TestPropertiesWith(_nan, _nan, _nan, _nan, _nan, _nan);
            TestPropertiesWith(_max, _min, _eps, _inf, _inf, _inf);
            Try(SetXOnEmptyRect, typeof(InvalidOperationException));
            Try(SetYOnEmptyRect, typeof(InvalidOperationException));
            Try(SetZOnEmptyRect, typeof(InvalidOperationException));
            Try(SetSizeXOnEmptyRect, typeof(InvalidOperationException));
            Try(SetSizeYOnEmptyRect, typeof(InvalidOperationException));
            Try(SetSizeZOnEmptyRect, typeof(InvalidOperationException));
            Try(SetNegativeSizeX, typeof(ArgumentException));
            Try(SetNegativeSizeY, typeof(ArgumentException));
            Try(SetNegativeSizeZ, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Rect3D Location and Size properties

        private void SetXOnEmptyRect() { _empty.X = 0; }
        private void SetYOnEmptyRect() { _empty.Y = 0; }
        private void SetZOnEmptyRect() { _empty.Z = 0; }
        private void SetSizeXOnEmptyRect() { _empty.SizeX = 0; }
        private void SetSizeYOnEmptyRect() { _empty.SizeY = 0; }
        private void SetSizeZOnEmptyRect() { _empty.SizeZ = 0; }

        private void SetNegativeSizeX()
        {
            Rect3D r = Const.rOrigin0;
            r.SizeX = -1;
        }

        private void SetNegativeSizeY()
        {
            Rect3D r = Const.rOrigin0;
            r.SizeY = -1;
        }

        private void SetNegativeSizeZ()
        {
            Rect3D r = Const.rOrigin0;
            r.SizeZ = -1;
        }

        #endregion

        private void TestParse2()
        {
            Log("Testing Parse with Bad Params...");

            TestParseWith("NaN" + _sep + "Infinity" + _sep + "-Infinity" + _sep + "NaN" + _sep + "Infinity" + _sep + "NaN");
        }

        private void TestToString2()
        {
            Log("Testing ToString with Bad Params...");

            TestToStringWith(Const.r0NaN);
            TestToStringWith(Const.r1Inf);
        }
    }
}