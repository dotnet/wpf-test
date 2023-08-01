// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Utilities for float
    /// </summary>
    public static class FloatUtil
    {
        #region constants

        public static float FLT_EPSILON = 1.192092896e-07F;
        public static float FLT_MAX_PRECISION = 0xffffff;
        public static float INVERSE_FLT_MAX_PRECISION = 1.0F / FLT_MAX_PRECISION;

        #endregion 

        /// <summary>
        /// AreClose
        /// </summary>
        public static bool AreClose(float a, float b)
        {
            if (a == b)
            { 
                return true; 
            }
            
            // computes (|a-b| / (|a| + |b| + 10.0f)) < FLT_EPSILON
            float eps = ((float)Math.Abs(a) + (float)Math.Abs(b) + 10.0f) * FLT_EPSILON;
            float delta = a - b;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// IsOne
        /// </summary>
        public static bool IsOne(float a)
        {
            return AreClose(a, 1.0f); 
        }

        /// <summary>
        /// IsZero
        /// </summary>
        public static bool IsZero(float a)
        {
            return AreClose(a, 0.0f);
        }

        /// <summary>
        /// IsCloseToDivideByZero
        /// </summary>
        public static bool IsCloseToDivideByZero(float numerator, float denominator)
        {
            return Math.Abs(denominator) <= Math.Abs(numerator) * INVERSE_FLT_MAX_PRECISION;
        }       

    }
}
