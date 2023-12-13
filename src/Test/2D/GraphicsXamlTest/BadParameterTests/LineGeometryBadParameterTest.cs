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
        public void CreateLineGeometry()
        {
            XamlTestHelper.AddStep(TestLineGeometryWithExtremeValues);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(CleanUpCanvas);
            XamlTestHelper.AddStep(VerifyBlank);
        }

        public object TestLineGeometryWithExtremeValues(object arg)
        {
            //Creating an array of malformed LineGeometries.
            LineGeometry[] lg = new LineGeometry[] {
                new LineGeometry(new Point(Double.Epsilon, Double.Epsilon), new Point(Double.Epsilon, Double.Epsilon)),
                new LineGeometry(new Point(Double.MaxValue, Double.MinValue), new Point(Double.MaxValue, Double.MinValue)),
                new LineGeometry(new Point(Double.MinValue, Double.MaxValue), new Point(Double.MaxValue, Double.MinValue)),
                new LineGeometry(new Point(Double.NaN, Double.NegativeInfinity), new Point(Double.NaN, Double.NegativeInfinity)),
                new LineGeometry(new Point(Double.NegativeInfinity, Double.NaN), new Point(Double.NegativeInfinity, Double.NaN)),
                new LineGeometry(new Point(Double.PositiveInfinity, Double.NegativeInfinity), new Point(Double.PositiveInfinity, Double.NegativeInfinity)),
                new LineGeometry(new Point(Double.NegativeInfinity, Double.NegativeInfinity), new Point(Double.NegativeInfinity, Double.NegativeInfinity)),
                new LineGeometry(new Point(Double.PositiveInfinity, Double.PositiveInfinity), new Point(Double.PositiveInfinity, Double.PositiveInfinity)),
                new LineGeometry(new Point(Math.Pow(2.0, 32) - 1, 100), new Point(20, 100))
            };

            AddGeometryToPath(lg);
            return null;
        }


    }
}