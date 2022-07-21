// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows.Ink;
using System.Windows;
using System.Windows.Input;

namespace DRT
{
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class CreateStrokeTest : DrtInkTestcase
    {
        private void CreateAndVerifyStroke(int pointCount)
        {
            double[] data;
            Stroke stroke;
            CreateRandomStroke(pointCount, out stroke, out data);

            //
            // validate our values
            //
            if (stroke.StylusPoints.Count != pointCount)
            {
                throw new InvalidOperationException("Instanced stroke does not have the correct number of StylusPoints");
            }
            for (int i = 0, x = 0; i < data.Length; i += 3, x++)
            {
                StylusPoint stylusPoint = stroke.StylusPoints[x];
                if (stylusPoint.X != data[i])
                {
                    throw new InvalidOperationException("Unexpected X value");
                }
                if (stylusPoint.Y != data[i + 1])
                {
                    throw new InvalidOperationException("Unexpected X value");
                }
                if (stylusPoint.PressureFactor != (float)data[i + 2])
                {
                    throw new InvalidOperationException("Unexpected PressureFactor value");
                }
            }

        }

        public void VerifyBezier()
        {
            double[] data;
            Stroke stroke;
            CreateRandomStroke(100, out stroke, out data);

            StylusPointCollection bez1 = stroke.GetBezierStylusPoints();
            StylusPointCollection bez2 = stroke.GetBezierStylusPoints();

            //
            // verify
            //
            if (bez1.Count != bez2.Count)
            {
                throw new InvalidOperationException("stroke.GetBezierStylusPoints() is returning different data for two calls");
            }
            for (int x = 0; x < bez1.Count; x++)
            {
                if (bez1[x] != bez2[x])
                {
                    throw new InvalidOperationException("stroke.GetBezierStylusPoints() is returning different data for two calls");
                }
            }
        }

        public void CreateRandomStroke(int pointCount, out Stroke stroke, out double[] data)
        {
            Random rnd = new Random();
            // packetCount packets containing {X, Y}
            data = new double[pointCount * 3];
            for (int i = 0; i < data.Length; i += 3)
            {
                data[i] = (double)rnd.Next((int)StylusPoint.MinXY, (int)StylusPoint.MaxXY);
                data[i + 1] = (double)rnd.Next((int)StylusPoint.MinXY, (int)StylusPoint.MaxXY);
                data[i + 2] = (double)rnd.Next(0, 1);
            }

            //
            // create the stroke
            //
            StylusPointCollection stylusPoints = new StylusPointCollection(pointCount);
            for (int i = 0; i < data.Length; i += 3)
            {
                StylusPoint stylusPoint = new StylusPoint(data[i], data[i + 1], (float)data[i + 2]);
                stylusPoints.Add(stylusPoint);
            }

            stroke = new Stroke(stylusPoints);
        }

        public override void Run()
        {
            // Do the testing
            CreateAndVerifyStroke(100);
            CreateAndVerifyStroke(50);
            VerifyBezier();
            VerifyBezier();
            // done
            Success = true;
        }
    }
}
