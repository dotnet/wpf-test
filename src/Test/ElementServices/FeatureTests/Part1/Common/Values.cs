// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// A set of collections of valid/invalid values
    /// </summary>
    /// <remarks>
    /// Most methods take a bool and return an IEnumerable of double values.
    /// The values are valid for the method's criterion if the parameter is true,
    /// and invalid for that criterion if the parameter is false.
    /// </remarks>
    public static class Values
    {
        /// <summary>
        /// Enumerates a set of integer values to use as IDs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> Ids(bool getValidValues)
        {
            if (getValidValues)
            {
                for (int n = -3; n <= 3; ++n)
                {
                    yield return n;
                }
                yield return int.MinValue;
                yield return int.MaxValue;
                yield return int.MinValue + 1;
                yield return int.MaxValue - 1;
            }
            // no "else"; all values are valid
        }

        /// <summary>
        /// Enumerate values that pass (or fail) a "finite" test
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<double> Finite(bool getValidValues)
        {
            if (getValidValues)
            {
                // valid values
                yield return double.MinValue;
                yield return double.MaxValue;
                yield return 0.0;
                yield return 307.298;
                yield return 0.3983;
                yield return -49.29892;
            }
            else
            {
                // invalid values
                yield return double.PositiveInfinity;
                yield return double.NaN;
                yield return double.NegativeInfinity;
            }
        }

        /// <summary>
        /// Enumerate values that pass (or fail) a "finite or NaN" test.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<double> FiniteOrNaN(bool getValidValues)
        {
            if (getValidValues)
            {
                // valid values
                foreach (double validValue in Finite(true))
                {
                    yield return validValue;
                }
                yield return double.NaN;
            }
            else
            {
                // invalid values
                yield return double.PositiveInfinity;
                yield return double.NegativeInfinity;
            }
        }

        /// <summary>
        /// Enumerate values that pass (or fail) a "finite non-negative" test.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<double> FiniteNonNegative(bool getValidValues)
        {
            if (getValidValues)
            {
                yield return double.MaxValue;
                yield return 0.0;
                yield return double.Epsilon;
                yield return 387.298;
                yield return 0.39837;
            }
            else
            {
                yield return double.PositiveInfinity;
                yield return double.NaN;
                yield return double.NegativeInfinity;
                yield return -float.Epsilon;
                yield return -4923.2989;
            }
        }

        /// <summary>
        /// Enumerate values that pass (or fail) a "finite and >= 1" test.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<double> FiniteNotLessThanOne(bool getValidValues)
        {
            if (getValidValues)
            {
                // valid values
                foreach (double finite in Finite(true))
                {
                    if (finite >= 1.0)
                    {
                        yield return finite;
                    }
                    yield return 1.0;
                }
            }
            else
            {
                // invalid values
                yield return double.NaN;
                yield return double.PositiveInfinity;
                yield return double.NegativeInfinity;
                yield return 1.0 - double.Epsilon;
                foreach (double finite in Finite(true))
                {
                    if (finite < 1.0)
                    {
                        yield return finite;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerate values that pass (or fail) a "NaN, or finite and >= 1" test.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<double> NaNOrFiniteNotLessThanOne(bool getValidValues)
        {
            foreach (double value in FiniteNotLessThanOne(getValidValues))
            {
                if (getValidValues || !double.IsNaN(value))
                {
                    yield return value;
                }
            }
            if (getValidValues)
            {
                yield return double.NaN;
            }
        }

        /// <summary>
        /// Enumerates valid or invalid values for Vector.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<Vector> FiniteORNonNegative(bool getValidValues)
        {
            if (getValidValues)
            {
                yield return (new Vector(0, 0));
                yield return (new Vector(0.11, 0.11));
                yield return (new Vector(11.11, 11.11));
                yield return (new Vector(-0.11, -0.11));
                yield return (new Vector(-11.11, -11.11));
                yield return (new Vector(double.MaxValue, double.MaxValue));
                yield return (new Vector(double.MinValue, double.MinValue));
                yield return (new Vector(double.Epsilon, double.Epsilon));
            }
            else
            {
                yield return (new Vector(double.NaN, double.Epsilon));
                yield return (new Vector(double.NaN, double.PositiveInfinity)); 
                yield return (new Vector(double.NaN, double.NegativeInfinity)); 
            }
        }

        /// <summary>
        /// Enumerates valid or invalid values for Point.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<Point> FiniteValid(bool getValidValues)
        {
            if (getValidValues)
            {
                yield return (new Point(0, 0));
                yield return (new Point(16.8, 16.8));
                yield return (new Point(-16.8, -16.8));
                yield return (new Point(double.MaxValue, double.MinValue));
                yield return (new Point(double.MinValue, double.MaxValue));
                yield return (new Point(double.MinValue, double.MaxValue));
            }
            else
            {
                yield return (new Point(double.PositiveInfinity, double.NaN));
                yield return (new Point(double.NegativeInfinity, double.PositiveInfinity));
                yield return (new Point(double.NaN, double.NegativeInfinity));              
            }      
        }

        /// <summary>
        /// Enumerate reference-type values where null is allowed but not required.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<T> NullOrNot<T>(bool getValidValues)
            where T : class, new()
        {
            if (getValidValues)
            {
                yield return null;
                yield return new T();
            }
            // no "else" clause, because no values are invalid
        }

        /// <summary>
        /// Enumerates valid or invalid values for ManipulationModes.
        /// </summary>
        /// <param name="getValidValues"></param>
        /// <returns></returns>
        public static IEnumerable<ManipulationModes> Manipulations(bool getValidValues)
        {
            if (getValidValues)
            {
                for (int n = (int)ManipulationModes.None; n < (int)ManipulationModes.All; ++n)
                {
                    yield return (ManipulationModes)n;
                }
            }
            else
            {
                yield return (ManipulationModes)int.MaxValue;
                yield return (ManipulationModes)int.MinValue;
                yield return (ManipulationModes)((int)ManipulationModes.None - 1);
                yield return (ManipulationModes)((int)ManipulationModes.All + 1);
            }
        }

        /// <summary>
        /// Enumerate pivots with various parameter values
        /// </summary>
        /// <param name="getLegalValues"></param>
        /// <returns></returns>
        public static IEnumerable<ManipulationPivot> GetPivots(bool getValidValues)
        {
            if (getValidValues)
            {
                yield return null;
                foreach (double validX in Values.FiniteOrNaN(true))
                {
                    foreach (double validY in Values.FiniteOrNaN(true))
                    {
                        foreach (double validRadius in Values.NaNOrFiniteNotLessThanOne(true))
                        {
                            ManipulationPivot pivot = new ManipulationPivot();
                            pivot.Center = new Point(validX, validY);
                            pivot.Radius = validRadius;
                            yield return pivot;
                        }
                    }
                }
            }
            // no "else" clause; no values are invalid
        }
    }
}
