// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Math methods

using System;
using System.Windows;

namespace Test.Uis.Utils
{
    /// <summary>
    /// This class contains all methods for math
    /// </summary>
    public class MathUtils
    {
        /// <summary>
        /// This method tries to get the smallest integer value in the left half
        /// of the rect
        /// e.g. a rect {21.3433333333333,36,8.92333333333334,19.3133333333333}
        /// will return 22.
        /// It is possible that it can't find an integer value, if the width
        /// of the rect is just too small (most of the character rects won't have
        /// this problem) In this case, rect.Left will be returned and the caller
        /// will have to decide what to do with that.
        /// </summary>
        /// <param name="rect">Avalon rect</param>
        /// <returns>possible integer in double</returns>
        public static double GetSmallestPossibleIntegerValueWithinTheRect(Rect rect)
        {
            if (rect == Rect.Empty)
            {
                return rect.Left;
            }

            double limit = rect.Left + rect.Width / 2;
            double suspectedFirstIntegralValue = Math.Ceiling(rect.Left);

            return suspectedFirstIntegralValue < limit ? suspectedFirstIntegralValue : rect.Left;
        }
        /// <summary>
        /// This method tries to get the largest integer value in the right half
        /// of the rect
        /// e.g. a rect {21.3433333333333,36,8.92333333333334,19.3133333333333}
        /// will return 30. (floor(right))
        /// It is possible that it can't find an integer value, if the width
        /// of the rect is just too small (most of the character rects won't have
        /// this problem) In this case, rect.Right will be returned and the caller
        /// will have to decide what to do with that.
        /// </summary>
        /// <param name="rect">Avalon rect</param>
        /// <returns>possible integer in double</returns>
        public static double GetLargestPossibleIntegerValueWithinTheRect(Rect rect)
        {
            if (rect == Rect.Empty)
            {
                return rect.Right;
            }

            double start = rect.Left + rect.Width / 2;
            double suspectedLastIntegralValue = Math.Floor(rect.Right);

            return suspectedLastIntegralValue > start ? suspectedLastIntegralValue : rect.Right;
        }

    }

}
