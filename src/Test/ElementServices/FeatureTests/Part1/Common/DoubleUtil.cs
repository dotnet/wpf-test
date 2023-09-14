// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// General double utilities 
    /// </summary>
    public static class DoubleUtil
    {
        public const double epsilon = (double)0.49f; // half pixel, in various verifiers

        public const double DBL_EPSILON = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */

        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or
        /// not they are within epsilon of each other.  
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            // computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.  
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
        /// the other number.  
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
        /// epsilon of the other number.  
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
        /// epsilon of the other number.  
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
        /// Verifies if the given double is a finite number.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsDoubleFinite(double d)
        {
            return !(double.IsInfinity(d) || double.IsNaN(d));
        }
        
        /// <summary>
        /// Verifies if the given double is a finite number or 0.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsDoubleFiniteNonZero(double d)
        {
            return IsDoubleFinite(d) && !IsZero(d);
        }

        /// <summary>
        /// Verifies if the given value is close to 0.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsZero(double d)
        {
            // IsZero(d) check can be used to make sure that dividing by 'd' will never produce Infinity.
            // use DBL_EPSILON instead of double.Epsilon because double.Epsilon is too small and doesn't guarantee that.
            return Math.Abs(d) <= DBL_EPSILON;
        }

        /// <summary>
        /// Limits a double value to the given interval.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Limit(double d, double min, double max)
        {
            if (!double.IsNaN(max) && d > max)
            {
                return max;
            }

            if (!double.IsNaN(min) && d < min)
            {
                return min;
            }

            return d;
        }

        /// <summary>
        /// Converts a double to a float
        /// </summary>
        /// <param name="d">the double value to convert</param>
        /// <returns></returns>
        public static float ConvertToFloat(double d)
        {
            float f = (float)d;
            if (!double.IsInfinity(d) && float.IsInfinity(f))
            {
                // The conversion exceeded the size of a float and the value became infinity.
                // Instead, set the value to the min/max for float.
                if (d > float.MaxValue)
                {
                    f = float.MaxValue;
                }
                else if (d < float.MinValue)
                {
                    f = float.MinValue;
                }
            }

            return f;
        }

        /// <summary>
        /// If a nullable double has a value, returns that value. Otherwise, returns 0.0.
        /// </summary>
        public static double ValueOrDefault(double? nullable)
        {
            return ValueOrDefault(nullable, 0.0);
        }

        /// <summary>
        /// If a nullable double has a value, returns that value. Otherwise, returns defaultValue.
        /// </summary>
        public static double ValueOrDefault(double? nullable, double defaultValue)
        {
            return nullable.HasValue ? nullable.Value : defaultValue;
        }

    }
}
