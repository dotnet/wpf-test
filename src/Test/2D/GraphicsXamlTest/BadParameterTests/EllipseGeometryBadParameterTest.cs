// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace Microsoft.Test.Graphics
{
    public partial class BadParameterTests : Window
    {
        public void CreateEllipseGeometry()
        {
            XamlTestHelper.AddStep(TestEllipseGeometryWithExtremeValues);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(CleanUpCanvas);
            XamlTestHelper.AddStep(VerifyBlank);
        }

        public object TestEllipseGeometryWithExtremeValues(object arg)
        {
            //Creating an array of malformed EllipseGeometries.
            EllipseGeometry[] eg = new EllipseGeometry[] {
                //new EllipseGeometry(new Rect(new Point(Double.Epsilon, Double.Epsilon), new Point(Double.Epsilon, Double.Epsilon))),
                new EllipseGeometry(new Rect(new Point(Double.MaxValue, Double.MinValue), new Point(Double.MaxValue, Double.MinValue))),
                new EllipseGeometry(new Rect(new Point(Double.MinValue, Double.MaxValue), new Point(Double.MaxValue, Double.MinValue))),
                new EllipseGeometry(new Rect(new Point(Double.NaN, Double.NegativeInfinity), new Point(Double.NaN, Double.NegativeInfinity))),
                new EllipseGeometry(new Rect(new Point(Double.NegativeInfinity, Double.NaN), new Point(Double.NegativeInfinity, Double.NaN))),
                new EllipseGeometry(new Rect(new Point(Double.PositiveInfinity, Double.NegativeInfinity), new Point(Double.PositiveInfinity, Double.NegativeInfinity))),
                new EllipseGeometry(new Rect(new Point(Double.NegativeInfinity, Double.NegativeInfinity), new Point(Double.NegativeInfinity, Double.NegativeInfinity))),
                new EllipseGeometry(new Rect(new Point(Double.PositiveInfinity, Double.PositiveInfinity), new Point(Double.PositiveInfinity, Double.PositiveInfinity))),
                new EllipseGeometry(new Rect(new Point(Math.Pow(2.0, 32) - 1, 100), new Point(20, 100))),
                new EllipseGeometry(new Rect(23, 2, Double.PositiveInfinity, Double.PositiveInfinity)),
                new EllipseGeometry(new Rect(Double.NegativeInfinity, Double.NegativeInfinity, 100, 100)),
                new EllipseGeometry(new Rect(200, 200, Double.MaxValue, Double.MaxValue)),
                new EllipseGeometry(new Rect(300, 300, Double.PositiveInfinity, Double.PositiveInfinity)),
                new EllipseGeometry(new Rect(300, 300, Double.NaN, Double.NaN))
            };

            AddGeometryToPath(eg);
            return null;
        }

    }
}