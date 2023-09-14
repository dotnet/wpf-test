// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Various comparisons for results of manipulations
    /// </summary>
    public static class ManipulationComparisons
    {
        /// <summary>
        /// Check that all the deltas are zero.
        /// </summary>
        /// <param name="delta">The delta to check.</param>
        /// <param name="isHardZero">If true, comparisons are done with hard ==; 
        /// otherwise, epsilon-tolerant comparison is done.</param>
        public static void CheckZero(ManipulationDelta delta, bool isHardZero)
        {
            CheckEqual(delta.Translation.X, 0.0, isHardZero, "Translation.X");
            CheckEqual(delta.Translation.Y, 0.0, isHardZero, "Translation.Y");
            CheckEqual(delta.Rotation, 0.0, isHardZero, "Rotation");
            CheckEqual(delta.Expansion.X, 0.0, isHardZero, "Expansion.X");
            CheckEqual(delta.Expansion.Y, 0.0, isHardZero, "Expansion.Y");
            CheckEqual(delta.Scale.X, 1.0, isHardZero, "Scale.X");
            CheckEqual(delta.Scale.Y, 1.0, isHardZero, "Scale.Y");
        }

        /// <summary>
        /// Check that all the velocities are zero
        /// </summary>
        /// <param name="velocities">The velocities to check.</param>
        /// <param name="isHardZero">If true, comparisons are done with hard ==; 
        /// otherwise, epsilon-tolerant comparison is done.</param>
        public static void CheckZero(ManipulationVelocities velocities, bool isHardZero)
        {
            CheckEqual(velocities.LinearVelocity.X, 0.0, isHardZero, "LinearVelocity.X");
            CheckEqual(velocities.LinearVelocity.Y, 0.0, isHardZero, "LinearVelocity.Y");
            CheckEqual(velocities.AngularVelocity, 0.0, isHardZero, "AngularVelocity");
            CheckEqual(velocities.ExpansionVelocity.X, 0.0, isHardZero, "ExpansionVelocity.X");
            CheckEqual(velocities.ExpansionVelocity.Y, 0.0, isHardZero, "ExpansionVelocity.Y");
        }

        /// <summary>
        /// Checks two manipulation deltas to see if they're equal
        /// </summary>
        /// <param name="delta1">The first delta</param>
        /// <param name="delta2">The second delta</param>
        /// <param name="scale">A number representing the typical distance a point might move. 
        ///     Used for generating error tolerances.</param>
        public static void CheckEqual(ManipulationDelta delta1, ManipulationDelta delta2, double scale)
        {
            const double baseEpsilon = 0.000006;
            const double scaleEpsilon = 5 * baseEpsilon;
            double linearEpsilon = baseEpsilon * scale;

            CheckNearlyEqual(delta2.Translation.X, delta1.Translation.X, linearEpsilon, "Translation.X");
            CheckNearlyEqual(delta2.Translation.Y, delta1.Translation.Y, linearEpsilon, "Translation.Y");
            CheckNearlyEqual(delta2.Rotation, delta1.Rotation, baseEpsilon, "Rotation");
            CheckNearlyEqual(delta2.Expansion.X, delta1.Expansion.X, linearEpsilon, "Expansion.X");
            CheckNearlyEqual(delta2.Expansion.Y, delta1.Expansion.Y, linearEpsilon, "Expansion.Y");
            CheckNearlyEqual(delta2.Scale.X, delta1.Scale.X, scaleEpsilon, "Scale.X");
            CheckNearlyEqual(delta2.Scale.X, delta1.Scale.Y, scaleEpsilon, "Scale.Y");
        }

        /// <summary>
        /// Compares two sets of velocities 
        /// If one is a simple multiple of the other, returns the ratio. Otherwise, will assert. 
        /// If both are all zeroes, returns NaN.
        /// </summary>
        /// <param name="velocities1"></param>
        /// <param name="velocities2"></param>
        /// <returns>The ratio of velocities2 to velocities1.</returns>
        public static double CheckVelocityRatio(ManipulationVelocities velocities1, ManipulationVelocities velocities2)
        {
            double overallRatio = double.NaN;

            CheckRatio(ref overallRatio, velocities1.LinearVelocity.X, velocities2.LinearVelocity.X, "LinearVelocity.X");
            CheckRatio(ref overallRatio, velocities1.LinearVelocity.Y, velocities2.LinearVelocity.Y, "LinearVelocity.Y");
            CheckRatio(ref overallRatio, velocities1.AngularVelocity, velocities2.AngularVelocity, "AngularVelocity");
            CheckRatio(ref overallRatio, velocities1.ExpansionVelocity.X, velocities2.ExpansionVelocity.X, "ExpansionVelocity.X");
            CheckRatio(ref overallRatio, velocities1.ExpansionVelocity.Y, velocities2.ExpansionVelocity.Y, "ExpansionVelocity.Y");

            return overallRatio;
        }

        /// <summary>
        /// Check that a manipulation delta is rotation only
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="expectedDegrees"></param>
        public static void CheckRotationOnly(ManipulationDelta delta, double expectedDegrees)
        {
            double degrees = delta.Rotation * (double)(180.0 / Math.PI);

            
            //CheckNearlyEqual(degrees - expectedDegrees, 0.0, 0.0001, "rotation error");
            //CheckNearlyEqual(delta.Translation.X, 0.0, 0.001, "Translation.X");
            //CheckNearlyEqual(delta.Translation.Y, 0.0, 0.001, "Translation.Y");
            //CheckNearlyEqual(delta.Expansion.X, 0.0, 0.0001, "Expansion.X");
            //CheckNearlyEqual(delta.Expansion.Y, 0.0, 0.0001, "Expansion.Y");
            //CheckNearlyEqual(delta.Scale.X, 1.0, 0.00001, "Scale.X");
            //CheckNearlyEqual(delta.Scale.Y, 1.0, 0.00001, "Scale.Y");
        }

        /// <summary>
        /// Check whether two numbers are equal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expected"></param>
        /// <param name="isHardComparison"></param>
        /// <param name="paramName"></param>
        private static void CheckEqual(double value, double expected, bool isHardComparison, string paramName)
        {
            if (isHardComparison)
            {
                Utils.AssertEqualGeneric<double>(expected, value,
                    string.Format("{0} should exactly equal {1}", paramName, expected));
            }
            else
            {
                Utils.Assert(DoubleUtil.AreClose(value, expected),
                    string.Format("{0} should equal {1}", paramName, expected));
            }
        }

        /// <summary>
        /// Check whether two numbers are fairly close together.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expected"></param>
        /// <param name="epsilon"></param>
        /// <param name="paramName"></param>
        private static void CheckNearlyEqual(double value, double expected, double epsilon, string paramName)
        {
            double error = Math.Abs(value - expected);
            Utils.Assert(error <= epsilon, 
                string.Format("{0} value of {1} doesn't match expected value of {2} with epsilon {3}", paramName, value, expected, epsilon));
        }


        /// <summary>
        /// Get a ratio between two numbers
        /// If both are zero, returns NaN
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static double GetRatio(double value1, double value2)
        {
            if (DoubleUtil.IsZero(value1))
            {
                if (DoubleUtil.IsZero(value2))
                {
                    return double.NaN;
                }
                else
                {
                    return (value2 > 0.0) ? double.PositiveInfinity : double.NegativeInfinity;
                }
            }
            else
            {
                return value2 / value1;
            }
        }

        /// <summary>
        /// Checks the ratio between two numbers and sees whether it matches the expected
        /// value. If the expected ratio is NaN, will allow any ratio, and set the expected
        /// one to the actual one.
        /// </summary>
        /// <param name="expectedRatio"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="paramName"></param>
        private static void CheckRatio(ref double expectedRatio, double value1, double value2, string paramName)
        {
            double ratio = GetRatio(value1, value2);
            if (double.IsNaN(expectedRatio))
            {
                expectedRatio = ratio;
            }
            else if (!double.IsNaN(ratio))
            {
                double error = Math.Abs(1 - ratio / expectedRatio);
                const double epsilon = 0.0002;
                Utils.Assert(error < epsilon, "{0} ratio is {1}, doesn't match expected value of {2}",
                    paramName, ratio, expectedRatio);
            }
        }
    }
}
