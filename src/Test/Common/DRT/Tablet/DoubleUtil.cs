// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains the implementation of DoubleUtil, which 

// provides "fuzzy" comparison functionality for doubles and 

// double-based classes and structs in our code.




using System;
using System.Windows;

namespace System.Windows.Ink.Internal
{
    internal sealed class DoubleUtil
    {
        // NOTE that the static members of the Double,
        // e.g. Double.MinValue & Double.Epsilon are not named in
        // accordance with traditional standards.  Here's a key
        //
        // Standard C++      C#            Meaning
        // DBL_EPSILON       -            Difference between 1 & next larger double
        // DBL_MIN           -            Smallest positive normalized double
        // -             Double.Epsilon   Smallest positive double (denormalized!)
        // DBL_MAX       Double.MaxValue  Largest positive non-infinite double
        // -             Double.MinValue  -Double.MaxValue
        //
        // We'll use the C++ terminology & values 'cause the C# ones
        // aren't useful.
        
        // This value comes from sdk\inc\crt\float.h
        internal const double Epsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        internal const double SmallestNormal = 2.2250738585072014e-308; /* smallest normalized double */
        internal const double Neighborhood = 50; /* Number of distinct floating point values that we will consider equal to a give value. */
        
        
        // Idea of fuzzy equality of a & b as exposed in this file is
        // that a number a should be considered equal to the
        // *Neighborhood* closest floating point values.
        //
        // To ensure symmetry we use the average of |a| & |b| to
        // determine the allowed distance between them, this makes
        // only a small difference if the numbers are actually close.
        //
        // Given the average value AVG = (|a|+|b|)/2 the distance
        // between adjacent floats around AVG is approximately
        //
        //     Max(AVG,SmallestNormal) * Epsilon
        //
        // The max is just because numbers between -SmallestNormal and
        // SmallestNormal are spaced equally with numbers around
        // SmallestNormal.
        

        public static int AddSortedDouble(System.Collections.Generic.List<double> doubleList, double r)
        {
            return AddSortedDouble(doubleList, r, false);
        }
        public static int AddSortedDouble(System.Collections.Generic.List<double> doubleList, double r, bool allowDuplicates)
        {
            if (doubleList.Count == 0 || r < doubleList[0])
            {
                doubleList.Insert(0, r);
                return 0;
            }

            int index = doubleList.Count;
            int nFloor = 0;
            int nMid = (doubleList.Count - 1);

            do
            {
                if (r >= doubleList[nMid])
                    nFloor = nMid;
                else
                    index = nMid;

                nMid = (nFloor + index) / 2;
            } while (nMid > nFloor);

            if (allowDuplicates)
                doubleList.Insert(index, r);
            else
            {
                int testIndex = (0 < index) ? index - 1 : index;

                if (r < Math.Abs(doubleList[testIndex]) && DoubleUtil.AreClose(r,Math.Abs(doubleList[testIndex])) )
                    doubleList.Insert(index, r);
            }

            return index;
        }

        public static double Clamp(double input)
        {
            return Clamp(input, 0.0d, 1.0d);
        }

        public static double Clamp(double input, double minimum, double maximum)
        {
            if (input <= minimum)
                return minimum;
            else if (input >= maximum)
                return maximum;
            else
                return input;
        }

        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool AreClose(double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if(value1 == value2) return true;
            // This computes (|value1-value2| / Max((|value1| + |value2|)/2,SmallestNormal) < Neighborhood/2 * Epsilon
            double avg = (Math.Abs(value1)+Math.Abs(value2))/2;
            if (avg < SmallestNormal)
            {
                avg = SmallestNormal;
            }
            double eps = avg * ((Neighborhood/2) * Epsilon);
            double delta = value1 - value2;
            return(-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThan(double value1, double value2)
        {
            return (value1 < value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThan - Returns whether or not the first double is greater than the second double.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThan(double value1, double value2)
        {
            return (value1 > value2) && !AreClose(value1, value2);
        }

        /// <summary>
        /// LessThanOrClose - Returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool LessThanOrClose(double value1, double value2)
        {
            return (value1 < value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
        /// the second double.  That is, whether or not the first is strictly greater than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThanOrClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return (value1 > value2) || AreClose(value1, value2);
        }

        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param>
        public static bool IsOne(double value)
        {
            if (value < 0)
            {
                return false;
            }
            
            double delta = value-1.0;
            double avg = 1+0.5*delta;
            return delta < avg * ((Neighborhood/2) * Epsilon);
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        public static bool IsZero(double value)
        {
            // Largest value that "is zero", note that all "is zero"
            // values are denormalized.  Don't re-order this
            // expression! Epsilon * SmallestNormal is the smallest
            // denorm number and the multiplication could produce 0.

            const double largestZero = ((Neighborhood/2) * Epsilon) * SmallestNormal;
            return Math.Abs(value) < largestZero;
        }

        // The Point, Size, Rect and Matrix class have moved to WinCorLib.  However, we provide
        // internal AreClose methods for our own use here.

        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='point1'>The first point to compare</param>
        /// <param name='point2'>The second point to compare</param>
        /// <returns>Whether or not the two points are equal</returns>
        public static bool AreClose(Point point1, Point point2)
        {
            return DoubleUtil.AreClose(point1.X, point2.X) && 
            DoubleUtil.AreClose(point1.Y, point2.Y);
        }

        /// <summary>
        /// Compares two Size instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='size1'>The first size to compare</param>
        /// <param name='size2'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns>
        public static bool AreClose(Size size1, Size size2)
        {
            return DoubleUtil.AreClose(size1.Width, size2.Width) && 
                   DoubleUtil.AreClose(size1.Height, size2.Height);
        }
        
        /// <summary>
        /// Compares two Vector instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        /// <returns>Whether or not the two Vector instances are equal</returns>
        public static bool AreClose(System.Windows.Vector vector1, System.Windows.Vector vector2)
        { 
            return DoubleUtil.AreClose(vector1.X, vector2.X) && 
                   DoubleUtil.AreClose(vector1.Y, vector2.Y);
        }

        /// <summary>
        /// Compares two rectangles for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='rect1'>The first rectangle to compare</param>
        /// <param name='rect2'>The second rectangle to compare</param>
        /// <returns>Whether or not the two rectangles are equal</returns>
        public static bool AreClose(Rect rect1, Rect rect2)
        {
            return DoubleUtil.AreClose(rect1.X, rect2.X) &&
                DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
                DoubleUtil.AreClose(rect1.Height, rect2.Height) &&
                DoubleUtil.AreClose(rect1.Width, rect2.Width);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsBetweenZeroAndOne(double val)
        {
            return (GreaterThanOrClose(val, 0) && LessThanOrClose(val, 1));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int DoubleToInt(double val)
        {
            return (0 < val) ? (int)(val + 0.5) : (int)(val - 0.5);
        }
    }
}
