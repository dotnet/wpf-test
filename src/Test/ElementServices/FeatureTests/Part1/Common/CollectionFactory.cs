// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Input.MultiTouch
{
    public static class CollectionFactory
    {
        /// <summary>
        /// A set of valid and invalid Points
        /// </summary>
        public static PointCollection Points
        {
            get
            {
                PointCollection c = new PointCollection();

                c.Add(new Point(0, 0));
                c.Add(new Point(16.8, 16.8));
                c.Add(new Point(-16.8, -16.8));
                c.Add(new Point(double.MaxValue, double.MinValue));
                c.Add(new Point(double.MinValue, double.MaxValue));
                c.Add(new Point(double.MinValue, double.MaxValue));
                c.Add(new Point(double.NaN, double.Epsilon));

                return c;
            }
        }

        /// <summary>
        /// A set of valid and invalid Vectors
        /// </summary>
        public static VectorCollection Vectors
        {
            get
            {
                VectorCollection c = new VectorCollection();

                c.Add(new Vector(0, 0));
                c.Add(new Vector(11.11, 0.11));
                c.Add(new Vector(-11.11, -0.11));
                c.Add(new Vector(double.MaxValue, double.MinValue));
                c.Add(new Vector(double.MinValue, double.MaxValue));
                c.Add(new Vector(double.MinValue, double.MaxValue));
                c.Add(new Vector(double.NaN, double.Epsilon));

                return c;
            }
        }

        /// <summary>
        /// A set of valid and invalid doubles
        /// </summary>
        public static DoubleCollection Doubles
        {
            get
            {
                DoubleCollection c = new DoubleCollection();

                c.Add(0);
                c.Add(-20.16);
                c.Add(20.16);
                c.Add(double.MaxValue);
                c.Add(double.MinValue);
                c.Add(double.PositiveInfinity);
                c.Add(double.NegativeInfinity);
                c.Add(double.Epsilon);
                c.Add(double.NaN);

                return c;
            }
        }

    }
}
