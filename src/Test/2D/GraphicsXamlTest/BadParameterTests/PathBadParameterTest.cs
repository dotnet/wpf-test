// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
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
        public void CreatePathGeometry()
        {
            XamlTestHelper.AddStep(TestPathGeometryWithExtremeValues);
            XamlTestHelper.AddStep(XamlTestHelper.TakeSnapshot);
            XamlTestHelper.AddStep(CleanUpCanvas);
            XamlTestHelper.AddStep(VerifyBlank);
        }

        public object TestPathGeometryWithExtremeValues(object arg)
        {
            //Empty PathFigure, but closed
            PathFigure pf1 = new PathFigure();
            pf1.IsClosed = true;

            //The StartPoint in this PathFigure contains some extreme values
            PathFigure pf2 = new PathFigure();
            pf2.StartPoint = new Point(Double.PositiveInfinity, Double.NegativeInfinity);
            pf2.Segments.Add(new LineSegment(new Point(100, 32), true));

            //This PathFigure contains a LineSegment which has a extreme endpoint
            PathFigure pf3 = new PathFigure();
            pf3.Segments.Add(new LineSegment(new Point(Double.MaxValue, Double.MinValue), true));
            pf3.IsClosed = false;

            //PathGeometry contains int.MaxValue of PathFigure            
            PathFigure[] pfTO = new PathFigure[int.MaxValue];
            PathGeometry pg1 = new PathGeometry();

            //PathGeometry contains a malformed RectangleGeometry
            PathGeometry pg2 = new PathGeometry();
            pg2.AddGeometry(new RectangleGeometry(Rect.Empty));

            //PathGeometry contains a malformed EllipseGeometry
            PathGeometry pg3 = new PathGeometry();
            pg3.AddGeometry(new EllipseGeometry(new Point(Double.MaxValue, Double.NaN), 100, 23));

            PathGeometry pg4 = new PathGeometry();
            pg4.Figures.Add(pf1);

            PathGeometry pg5 = new PathGeometry();
            pg5.Figures.Add(pf2);

            PathGeometry pg6 = new PathGeometry();
            pg6.Figures.Add(pf3);

            PathGeometry[] pgArray = new PathGeometry[]{
                pg1, pg2, pg3, pg4, /* pg5, */ pg6};

            AddGeometryToPath(pgArray);
            return null;
        }


    }
}