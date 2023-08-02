// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Various utilities for multitouch testing 
    /// </summary>
    public static class Utils
    {
        #region Constants

        private static readonly Random s_rand = new Random();

        #endregion

        #region Point to PointF

        public static System.Drawing.PointF ToPointF(this Point p)
        {
            return new System.Drawing.PointF((float)p.X, (float)p.Y);
        }

        public static System.Drawing.PointF[] ToPointFArray(this Point[] ps)
        {
            return (from p in ps select new System.Drawing.PointF((float)p.X, (float)p.Y)).ToArray();
        }

        #endregion
        
        #region Lengths and Rects

        public static RectangleR FindBox(ArrayList points)
        {
            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;

            foreach (PointR p in points)
            {
                if (p.X < minX)
                { 
                    minX = p.X; 
                }

                if (p.X > maxX)
                {
                    maxX = p.X;
                }

                if (p.Y < minY)
                {
                    minY = p.Y;
                }

                if (p.Y > maxY)
                {
                    maxY = p.Y;
                }
            }

            return new RectangleR(minX, minY, maxX - minX, maxY - minY);
        }

        public static double Distance(PointR p1, PointR p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// compute the centroid of the given points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static PointR Centroid(ArrayList points)
        {
            double xsum = 0.0;
            double ysum = 0.0;

            foreach (PointR p in points)
            {
                xsum += p.X;
                ysum += p.Y;
            }

            return new PointR(xsum / points.Count, ysum / points.Count);
        }

        public static double PathLength(ArrayList points)
        {
            double length = 0;
            for (int i = 1; i < points.Count; i++)
            {
                length += Distance((PointR)points[i - 1], (PointR)points[i]);
            }

            return length;
        }

        #endregion

        #region Angles and Rotations

        /// <summary>
        /// Determines the angle, in degrees, between two points. the angle is defined
        /// by the circle centered on the start point with a radius to the end point,
        /// where 0 degrees is straight right from start (+x-axis) and 90 degrees is
        /// straight down (+y-axis).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="positiveOnly"></param>
        /// <returns></returns>
        public static double AngleInDegrees(PointR start, PointR end, bool positiveOnly)
        {
            double radians = AngleInRadians(start, end, positiveOnly);
            return Rad2Deg(radians);
        }

        /// <summary>
        /// Determines the angle, in radians, between two points. the angle is defined 
        /// by the circle centered on the start point with a radius to the end point, 
        /// where 0 radians is straight right from start (+x-axis) and PI/2 radians is
        /// straight down (+y-axis).
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="positiveOnly"></param>
        /// <returns></returns>
        public static double AngleInRadians(PointR start, PointR end, bool positiveOnly)
        {
            double radians = 0.0;

            if (start.X != end.X)
            {
                radians = Math.Atan2(end.Y - start.Y, end.X - start.X);
            }
            else // pure vertical movement
            {
                if (end.Y < start.Y)
                {
                    radians = -Math.PI / 2.0; // -90 degrees for straight up
                }
                else if (end.Y > start.Y)
                {
                    radians = Math.PI / 2.0; // 90 degrees for straight down
                }
            }

            if (positiveOnly && radians < 0.0)
            {
                radians += Math.PI * 2.0;
            }

            return radians;
        }

        public static double Rad2Deg(double rad)
        {
            return (rad * 180d / Math.PI);
        }

        public static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180d);
        }

        /// <summary>
        /// Rotate the points by the given degrees about their centroid
        /// </summary>
        /// <param name="points"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static ArrayList RotateByDegrees(ArrayList points, double degrees)
        {
            double radians = Deg2Rad(degrees);
            return RotateByRadians(points, radians);
        }

        /// <summary>
        /// Rotate the points by the given radians about their centroid
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static ArrayList RotateByRadians(ArrayList points, double radians)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            PointR c = Centroid(points);

            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double cx = c.X;
            double cy = c.Y;

            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];

                double dx = p.X - cx;
                double dy = p.Y - cy;

                PointR q = PointR.Empty;
                q.X = dx * cos - dy * sin + cx;
                q.Y = dx * sin + dy * cos + cy;

                newPoints.Add(q);
            }
            return newPoints;
        }

        /// <summary>
        /// Rotate a point 'p' around a point 'c' by the given radians.
        /// Rotation (around the origin) amounts to a 2x2 matrix of the form:
        ///		[ cos A		-sin A	] [ p.x ]
        ///		[ sin A		cos A	] [ p.y ]        
        /// Note that the C# Math coordinate system has +x-axis stright right and
        /// +y-axis straight down. Rotation is clockwise such that from +x-axis to
        /// +y-axis is +90 degrees, from +x-axis to -x-axis is +180 degrees, and 
        /// from +x-axis to -y-axis is -90 degrees.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="c"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static PointR RotatePoint(PointR p, PointR c, double radians)
        {
            PointR q = PointR.Empty;
            q.X = (p.X - c.X) * Math.Cos(radians) - (p.Y - c.Y) * Math.Sin(radians) + c.X;
            q.Y = (p.X - c.X) * Math.Sin(radians) + (p.Y - c.Y) * Math.Cos(radians) + c.Y;
            return q;
        }

        #endregion

        #region Translations

        /// <summary>
        /// Translates the points so that the upper-left corner of their bounding box lies at 'toPt'
        /// </summary>
        /// <param name="points"></param>
        /// <param name="toPt"></param>
        /// <returns></returns>
        public static ArrayList TranslateBBoxTo(ArrayList points, PointR toPt)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            RectangleR r = Utils.FindBox(points);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X += (toPt.X - r.X);
                p.Y += (toPt.Y - r.Y);
                newPoints.Add(p);
            }
            return newPoints;
        }

        /// <summary>
        /// Translates the points so that their centroid lies at 'toPt'
        /// </summary>
        /// <param name="points"></param>
        /// <param name="toPt"></param>
        /// <returns></returns>
        public static ArrayList TranslateCentroidTo(ArrayList points, PointR toPt)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            PointR centroid = Centroid(points);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X += (toPt.X - centroid.X);
                p.Y += (toPt.Y - centroid.Y);
                newPoints.Add(p);
            }
            return newPoints;
        }

        /// <summary>
        /// Translates the points by the given delta amounts
        /// </summary>
        /// <param name="points"></param>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static ArrayList TranslateBy(ArrayList points, SizeR sz)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X += sz.Width;
                p.Y += sz.Height;
                newPoints.Add(p);
            }
            return newPoints;
        }

        #endregion

        #region Scaling

        /// <summary>
        /// Scales the points so that they form the size given. Does not restore the origin of the box.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static ArrayList ScaleTo(ArrayList points, SizeR sz)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            RectangleR r = Utils.FindBox(points);

            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];

                if (r.Width != 0d)
                {
                    p.X *= (sz.Width / r.Width);
                }

                if (r.Height != 0d)
                {
                    p.Y *= (sz.Height / r.Height);
                }

                newPoints.Add(p);
            }
            return newPoints;
        }

        /// <summary>
        /// Scales by percentages contained in 'sz' parameter. values of 1.0 would result in the identity scale w/o change.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="sz"></param>
        /// <returns></returns>
        public static ArrayList ScaleBy(ArrayList points, SizeR sz)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            RectangleR r = FindBox(points);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X *= sz.Width;
                p.Y *= sz.Height;
                newPoints.Add(p);
            }
            return newPoints;
        }

        /// <summary>
        /// Scales the points so that the length of their longer side matches the length of the longer side of the given rect.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static ArrayList ScaleToMax(ArrayList points, RectangleR rect)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            RectangleR r = FindBox(points);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X *= (rect.MaxSide / r.MaxSide);
                p.Y *= (rect.MaxSide / r.MaxSide);
                newPoints.Add(p);
            }
            return newPoints;
        }

        /// <summary>
        /// Scales the points so that the length of their shorter side matches the length of the shorter side of the given rect.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static ArrayList ScaleToMin(ArrayList points, RectangleR rect)
        {
            ArrayList newPoints = new ArrayList(points.Count);
            RectangleR r = FindBox(points);
            for (int i = 0; i < points.Count; i++)
            {
                PointR p = (PointR)points[i];
                p.X *= (rect.MinSide / r.MinSide);
                p.Y *= (rect.MinSide / r.MinSide);
                newPoints.Add(p);
            }
            return newPoints;
        }

        #endregion

        #region Path Sampling and Distance

        public static ArrayList Resample(ArrayList points, int n)
        {
            double I = PathLength(points) / (n - 1); // interval length
            double D = 0.0;

            ArrayList srcPts = new ArrayList(points);
            ArrayList dstPts = new ArrayList(n);
            dstPts.Add(srcPts[0]);

            for (int i = 1; i < srcPts.Count; i++)
            {
                PointR pt1 = (PointR)srcPts[i - 1];
                PointR pt2 = (PointR)srcPts[i];

                double d = Distance(pt1, pt2);
                if ((D + d) >= I)
                {
                    double qx = pt1.X + ((I - D) / d) * (pt2.X - pt1.X);
                    double qy = pt1.Y + ((I - D) / d) * (pt2.Y - pt1.Y);
                    PointR q = new PointR(qx, qy);
                    dstPts.Add(q);       // append new point 'q'
                    srcPts.Insert(i, q); // insert 'q' at position i in points s.t. 'q' will be the next i
                    D = 0.0;
                }
                else
                {
                    D += d;
                }
            }

            // at times due to rounding-error it falls short of adding the last point, so add it
            if (dstPts.Count == n - 1)
            {
                dstPts.Add(srcPts[srcPts.Count - 1]);
            }

            return dstPts;
        }

        /// <summary>
        /// Computes the 'distance' between two point paths by summing their corresponding point distances.
        /// assumes that each path has been resampled to the same number of points at the same distance apart.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns></returns>
        public static double PathDistance(ArrayList path1, ArrayList path2)
        {
            double distance = 0;
            for (int i = 0; i < path1.Count; i++)
            {
                distance += Distance((PointR)path1[i], (PointR)path2[i]);
            }
            return distance / path1.Count;
        }

        #endregion

        #region Random Numbers

        /// <summary>
        /// Gets a random number between low and high, inclusive.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <returns></returns>
        public static int Random(int low, int high)
        {
            return s_rand.Next(low, high + 1);
        }

        /// <summary>
        /// Gets multiple random numbers between low and high, inclusive. The numbers are guaranteed to be distinct.
        /// </summary>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int[] Random(int low, int high, int num)
        {
            int[] array = new int[num];

            for (int i = 0; i < num; i++)
            {
                array[i] = s_rand.Next(low, high + 1);
                for (int j = 0; j < i; j++)
                {
                    if (array[i] == array[j])
                    {
                        i--; // redo i
                        break;
                    }
                }
            }

            return array;
        }

        public static object GetRandomEnumValue(Enum enumType)
        {
            Random rnd;
            int seed;
            seed = DateTime.Now.Millisecond;
            rnd = new Random(seed);

            object retVal = null;
            try
            {
                System.Reflection.FieldInfo[] fi = enumType.GetType().GetFields();
                if (fi.Length > 0)
                {
                    int index = rnd.Next(1, fi.Length);
                    retVal = fi[index].GetValue(null);
                }
            }
            catch { }

            Utils.Assert(retVal != null);
            return retVal;
        }

        public static double GetRandomDouble()
        {
            double d = 0.0;

            int seed = DateTime.Now.Millisecond;
            Random rnd = new Random(seed);

            d = rnd.NextDouble();

            return d;
        }

        /// <summary>
        /// Gets a Random Point on strokecollection or outside
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="rnd"></param>
        /// <returns></returns>
        public static Point GetRandomPoint(StrokeCollection sc, Random rnd)
        {
            Point p = new Point(0, 0);

            if (sc == null || rnd == null || sc.Count == 0)
            {
                return new Point(rnd.Next(), rnd.Next());
            }

            //get a list of all points on the stroke
            StylusPointCollection spc = new StylusPointCollection();
            foreach (Stroke s in sc)
            {
                spc.Add(s.StylusPoints.Reformat(spc.Description));
            }

            if (spc.Count == 0)
            {
                p = new Point(rnd.Next(), rnd.Next());
            }
            else
            {
                int randomIndex = 0;

                if (rnd.NextDouble() > 0.5)  // Pick either a point on the stroke or outside
                {
                    randomIndex = rnd.Next(0, spc.Count);
                    p = spc[randomIndex].ToPoint();
                }
                else
                {
                    p = new Point(rnd.Next(), rnd.Next());
                }
            }

            return p;
        }

        public static Rect GetRandomRect(StrokeCollection sc, Random rnd)
        {
            Rect rc = Rect.Empty;
            //error condition - not expected
            if (sc == null || rnd == null)
            {
                return rc;
            }

            Rect boundRect = sc.GetBounds();

            if (boundRect == Rect.Empty || sc.Count == 0)
            {
                return rc = new Rect(GetRandomPoint(sc, rnd), new Size(rnd.Next(1, 1000), rnd.Next(1, 1000)));
            }

            int option = rnd.Next() % 3;

            switch (option)
            {
                case 0:  //whole bounding rect
                    rc = boundRect;
                    break;

                case 1:  //totally random
                    rc = new Rect(GetRandomPoint(sc, rnd), new Size(rnd.Next(1, 1000), rnd.Next(1, 1000)));
                    break;

                case 2:
                    // with left,top inside the bounds but random size & location
                    double width1 = rnd.Next((int)Math.Min(1d, boundRect.Width), (int)Math.Max(100d, boundRect.Width)) + rnd.NextDouble();
                    double height1 = rnd.Next((int)Math.Min(1d, boundRect.Height), (int)Math.Max(100d, boundRect.Height)) + rnd.NextDouble();

                    //just to protect ourselves again.
                    if (Double.IsNaN(width1) || width1 <= Double.Epsilon || Double.IsInfinity(width1))
                    {
                        width1 = 1;
                    }

                    if (Double.IsNaN(height1) || height1 <= Double.Epsilon || Double.IsInfinity(height1))
                    {
                        height1 = 1;
                    }

                    double x1 = rnd.Next((int)boundRect.X, (int)(boundRect.X + boundRect.Width)) + rnd.NextDouble();
                    double y1 = rnd.Next((int)boundRect.Y, (int)(boundRect.Y + boundRect.Height)) + rnd.NextDouble();
                    rc = new Rect(x1, y1, width1, height1);

                    break;
            }

            return rc;
        }

        #endregion

        #region Stroke and Points

        /// <summary>
        /// Compare two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool ComparePoints(Point a, Point b)
        {
            const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
            // Mean squared magnitude of point coefficients
            double mag = 0.25 * (a.X * a.X + a.Y * a.Y + b.X * b.X + b.Y * b.Y);
            if (mag == 0)
            { 
                return true; 
            }

            // Allowing magnitude * DBL_EPSILON * 10 error, but squared
            double maxError = mag * DBL_EPSILON * DBL_EPSILON * 100;
            Vector delta = a - b;

            return delta.LengthSquared < maxError;
        }

        /// <summary>
        /// Compare points from two Stylus point collections
        /// </summary>
        /// <param name="sSrc"></param>
        /// <param name="sDst"></param>
        /// <param name="bComparePoints"></param>
        /// <returns></returns>
        public static bool CompareStrokes(Stroke sSrc, Stroke sDst, bool bComparePoints)
        {
            int nPoints = sSrc.StylusPoints.Count;
            if (nPoints != sDst.StylusPoints.Count)
            { 
                return false; 
            }

            if (sSrc.DrawingAttributes != sDst.DrawingAttributes)
            {
                return false;
            }

            bool bRetCode = true;

            if (bComparePoints)
            {
                StylusPointCollection ptSrc = sSrc.StylusPoints;
                StylusPointCollection ptDst = sDst.StylusPoints;
                int i = 0;

                nPoints = ptSrc.Count;
                for (; i < nPoints; ++i)
                {
                    if (!StylusPoint.Equals(ptSrc[i], ptDst[i]))
                    {
                        break;
                    }
                }

                bRetCode = (i == nPoints);
            }

            return bRetCode;
        }

        /// <summary>
        /// Compare two strokecollections
        /// </summary>
        /// <param name="strokesSource"></param>
        /// <param name="strokesDestination"></param>
        /// <param name="bComparePoints"></param>
        /// <returns></returns>
        public static bool CompareStrokeCollections(StrokeCollection strokesSource, StrokeCollection strokesDestination, bool bComparePoints)
        {
            int nStrokes = strokesSource.Count;
            if (nStrokes != strokesDestination.Count) return false;

            int i = 0;

            for (; i < nStrokes; ++i)
            {
                Stroke sSrc = strokesSource[i];
                Stroke sDst = strokesDestination[i];

                if (!Utils.CompareStrokes(sSrc, sDst, bComparePoints))
                {
                    break;
                }
            }

            return (i == nStrokes);
        }

        /// <summary>
        /// Generic comparison of two arrays of type ItemType that impements Equals
        /// </summary>
        /// <typeparam name="ItemType"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void CompareArrays<ItemType>(ItemType[] first, ItemType[] second)
        {
            if (first.Length != second.Length)
            {
                throw new InvalidOperationException("Arrays do not have equal lengths");
            }
            else
            {
                for (int i = 0; i < first.Length; i++)
                {
                    if (!first[i].Equals(second[i]))
                    {
                        throw new InvalidOperationException(string.Format("Array item does not match at i = {0}", i));
                    }
                }
            }
        }

        /// <summary>
        /// Compare two points arrays
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static void ComparePointArrays(Point[] first, Point[] second)
        {
            if (first.Length != second.Length)
            {
                throw new InvalidOperationException("Arrays do not have equal lengths");
            }
            else
            {
                Utils.CompareArrays<Point>(first, second);
            }
        }

        #endregion

        #region Asserts

        /// <summary>
        /// Assert that condition is true.
        /// </summary>
        /// <param name="cond">condition to test</param>
        public static void Assert(bool cond)
        {
            Utils.Assert(cond, String.Empty, null, null);
        }

        /// <summary>
        /// Do assert
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="message"></param>
        /// <param name="arg"></param>
        public static void Assert(bool cond, string message, params object[] arg)
        {
            if (!cond)
            {
                throw new TestValidationException(string.Format(message, arg));
            }
        }

        /// <summary>
        /// Assert that objects are equal. The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public static void AssertEqual(object expected, object actual, string message, params object[] arg)
        {
            if (!Object.Equals(expected, actual))
            {
                if (expected == null)
                { 
                    expected = "NULL"; 
                }

                if (actual == null)
                { 
                    actual = "NULL"; 
                }
                
                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                Utils.Assert(false, message, arg);
            }
        }

        /// <summary>
        /// Assert that objects are equal. The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public static void AssertEqual(int expected, int actual, string message, params object[] arg)
        {
            if (expected != actual)
            {
                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                Utils.Assert(false, message, arg);
            }
        }

        /// <summary>
        /// Assert that doubles are equal (up to relative error of epsion).
        /// The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="epsilon">tolerance for relative error</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public static void AssertAreClose(double expected, double actual, double epsilon, string message, params object[] arg)
        {
            double tolerance = epsilon * Math.Max(Math.Abs(expected), Math.Abs(actual));
            if (Math.Abs(expected - actual) > tolerance)
            {
                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                Utils.Assert(false, message, arg);
            }
        }

        /// <summary>
        /// Assert that doubles are equal (up to relative error of epsion).
        /// The phrase "Expected: x  Got: y" is automatically added to the message.
        /// </summary>
        /// <param name="expected">expected value</param>
        /// <param name="actual">actual value</param>
        /// <param name="epsilon">tolerance for relative error</param>
        /// <param name="message">message to display if assert fails</param>
        /// <param name="arg">args for format tags in message</param>
        public static void AssertAreCloseNullable(double? expected, double? actual, double epsilon, string message, params object[] arg)
        {
            if (expected.HasValue && actual.HasValue)
            {
                double tolerance = epsilon * Math.Max(Math.Abs(expected.Value), Math.Abs(actual.Value));
                if (Math.Abs(expected.Value - actual.Value) > tolerance)
                {
                    message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                    Utils.Assert(false, message, arg);
                }
            }
            else if (!expected.HasValue && !actual.HasValue)
            {
                Utils.Assert(true, message, arg);
            }
            else
            {
                Utils.Assert(false, message, arg);
            }
        }

        /// <summary>
        /// Verifies that objects are equal, determined by their comparison methods.
        /// </summary>
        /// <typeparam name="T">Comparable type</typeparam>
        /// <param name="a">The first value to compare - Expected</param>
        /// <param name="b">The second value to compare</param>
        /// <param name="description">Description of the comparison</param>
        public static void AssertEqualGeneric<T>(T expected, T actual, string message) where T : IComparable
        {
            if (expected.CompareTo(actual) == 0)
            {
                message += String.Format(" Expected: {0}  Got: {1}", expected, actual);
                Utils.Assert(false, message, null);
            }
        }

        #endregion

        #region Param Verifiers

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is not finite.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public static void CheckFinite(double value, string property, string paramName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckFinite for property [{0}]", property));
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is not finite or NaN.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public static void CheckFiniteOrNaN(double value, string property, string paramName)
        {
            if (double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckFiniteOrNaN for property [{0}]", property));
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is not positive and finite.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public static void CheckFinitePositive(double value, string property, string paramName)
        {
            if (value <= 0 || double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckFinitePositive for property [{0}]", property));
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is infinite or less than zero.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public static void CheckFiniteNonNegativeOrNaN(double value, string property, string paramName)
        {
            if (value < 0 || double.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckFiniteNonNegativeOrNaN for property [{0}]", property));
            }
        }

        /// <summary>
        /// Throws ArgumentOutOfRangeException if double value is not finite and non-negative.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <param name="paramName"></param>
        public static void CheckFiniteNonNegative(double value, string property, string paramName)
        {
            if (value < 0 || double.IsInfinity(value) || double.IsNaN(value))
            {
                throw new ArgumentOutOfRangeException(paramName, value, string.Format(CultureInfo.InvariantCulture, "CheckFiniteNonNegative for property [{0}]", property));
            }
        }

        #endregion

        #region Misc.

        /// <summary>
        /// Returns the screen resolution (in pixels per inches)
        /// </summary>
        /// <returns></returns>
        public static Size GetScreenResolution()
        {
            Size retVal = Size.Empty;
            IntPtr screenHDC = IntPtr.Zero;
            try
            {
                screenHDC = MultiTouchNativeMethods.GetDC(IntPtr.Zero);
                if (screenHDC == IntPtr.Zero)
                {
                    throw new ExternalException("Native call to 'GetDC' failed w/ (error # " + Marshal.GetLastWin32Error() + ")");
                }

                int x = MultiTouchNativeMethods.GetDeviceCaps(screenHDC, (int)MultiTouchNativeMethods.LOGPIXELSX);
                if (x == 0)
                {
                    throw new ExternalException("Native call to 'GetDeviceCaps(x)' failed w/ (error # " + Marshal.GetLastWin32Error() + ")");
                }
                
                int y = MultiTouchNativeMethods.GetDeviceCaps(screenHDC, (int)MultiTouchNativeMethods.LOGPIXELSY);
                if (y == 0)
                {
                    throw new ExternalException("Native call to 'GetDeviceCaps(y)' failed w/ (error # " + Marshal.GetLastWin32Error() + ")");
                }
                
                retVal = new Size(x, y);
            }
            finally
            {
                if (screenHDC != IntPtr.Zero)
                {
                    MultiTouchNativeMethods.ReleaseDC(IntPtr.Zero, screenHDC); screenHDC = IntPtr.Zero;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Find the vertical and horizontal scale factors for different DPI setting.
        /// </summary>
        /// <param name="xFactor">float that returns the horizontal factor</param>
        /// <param name="yFactor">float that returns the vertical factor</param>
        /// <returns></returns>
        public static void HighDpiScaleFactors(out float xFactor, out float yFactor)
        {
            using (System.Drawing.Graphics gs = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
            {
                xFactor = gs.DpiX / 96;
                yFactor = gs.DpiY / 96;
            }
        }

        /// <summary>
        /// Gets the top-most visual for the specified visual element.
        /// </summary>
        public static Visual GetTopMostVisual(Visual element)
        {
            PresentationSource source;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            source = PresentationSource.FromVisual(element);
            if (source == null)
            {
                throw new InvalidOperationException(string.Format("The specified UiElement is not connected to a rendering Visual Tree: {0}", element));
            }

            return source.RootVisual;
        }
                
        public static string TypeWithoutNamespace(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            string[] astr = obj.GetType().ToString().Split('.');
            return astr[astr.Length - 1];
        }

        #endregion
    }
}
