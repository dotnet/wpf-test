// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary>
    /// Verify correct functional behavior of GeneralTransform3DGroup methods:
    ///     TryTransform
    ///     TestTransformBounds
    /// </summary>
    public class GeneralTransform3DGroupTest : CoreGraphicsTest
    {      
        public override void RunTheTest()
        {
            TestTryTransform();
            TestTransformBounds();
        }

        private void TestTryTransform()
        {
           bool result = true;

            GeneralTransform3DGroup transformGroup = new GeneralTransform3DGroup();
            transformGroup.Children.Add(new ScaleTransform3D(2, 2, 2)); // scale by a factor of 2
            transformGroup.Children.Add(new TranslateTransform3D(0.5, 0.5, -0.5)); // translate
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90), 0, 0, 0)); // rotate 90 degrees around the x-axis

            Point3D[] inputPoints = { new Point3D(0.25, 0, 0), new Point3D(0, 0.25, 0), new Point3D(0, 0, 0.25),
                                      new Point3D(0.25, 0.25, 0), new Point3D(0.25, 0, 0.25), new Point3D(0.25, 0.25, 0.25),
                                      new Point3D(-0.25, 0, 0), new Point3D(0, -0.25, 0), new Point3D(0, 0, -0.25),
                                      new Point3D(-0.25, -0.25, 0), new Point3D(-0.25, 0, -0.25), new Point3D(-0.25, -0.25, -0.25),
                                      new Point3D(-0.25, -0.25, 0.25), new Point3D(Double.NaN, Double.NaN, Double.NaN) };

            Point3D[] expectedOutput = { new Point3D(1, 0.5, 0.5), new Point3D(0.5, 0.5, 1), new Point3D(0.5, 0, 0.5),
                                         new Point3D(1, 0.5, 1), new Point3D(1, 0, 0.5), new Point3D(1, 0, 1),
                                         new Point3D(0, 0.5, 0.5), new Point3D(0.5, 0.5, 0), new Point3D(0.5, 1, 0.5),
                                         new Point3D(0, 0.5, 0), new Point3D(0, 1, 0.5), new Point3D(0, 1, 0),
                                         new Point3D(0, 0, 0), new Point3D(Double.NaN, Double.NaN, Double.NaN) };

            for (int i = 0; i < inputPoints.Length; i++)
            {
                Point3D p;
                bool transformed = transformGroup.TryTransform(inputPoints[i], out p);

                if (transformed == false || MathEx.AreCloseEnough(p, expectedOutput[i]) == false)
                {
                    result = false;
                    Log("*** Error: Input = " + inputPoints[i] + "   expected output = " + expectedOutput[i] + "   actual output = " + p);
                }
            }

            if (result == false)
            {
                AddFailure("TryTransform of GeneralTransform3DGroup failed.");
            }
        }

        private void TestTransformBounds()
        {
            bool result = true;

            GeneralTransform3DGroup transformGroup = new GeneralTransform3DGroup();
            transformGroup.Children.Add(new ScaleTransform3D(2, 2, 2)); // scale by a factor of 2
            transformGroup.Children.Add(new TranslateTransform3D(0.5, 0.5, -0.5)); // translate
            transformGroup.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90), 0, 0, 0)); // rotate 90 degrees around the x-axis

            Rect3D[] inputRects = { new Rect3D(0.5, 0, 0, 1, 1, 1), new Rect3D(0, 0.5, 0, 1, 1, 1), new Rect3D(0, 0, 0.5, 1, 1, 1),
                                    new Rect3D(0.5, 0.5, 0, 1, 1, 1), new Rect3D(0.5, 0, 0.5, 1, 1, 1), new Rect3D(0.5, 0.5, 0.5, 1, 1, 1),
                                    new Rect3D(-0.5, 0, 0, 1, 1, 1), new Rect3D(0, -0.5, 0, 1, 1, 1), new Rect3D(0, 0, -0.5, 1, 1, 1),
                                    new Rect3D(-0.5, -0.5, 0, 1, 1, 1), new Rect3D(-0.5, 0, -0.5, 1, 1, 1), new Rect3D(-0.5, -0.5, -0.5, 1, 1, 1),
                                    new Rect3D(-0.25, -0.25, 0.25, 1, 1, 1), new Rect3D(Double.NaN, Double.NaN, Double.NaN, 1, 1, 1),
                                    new Rect3D(0, 0, 0, Double.NaN, Double.NaN, Double.NaN), new Rect3D(0, 0, 0, 0, 0, 0), 
                                    new Rect3D(0, 0, 0, Double.MaxValue, Double.MaxValue, Double.MaxValue),
                                    new Rect3D(Double.MaxValue, Double.MaxValue, Double.MaxValue, 0, 0, 0),
                                    new Rect3D(Double.MinValue, Double.MinValue, Double.MinValue, 0, 0, 0) };

            Rect3D[] expectedOutput = { new Rect3D(1.5, -1.5, 0.5, 2, 2, 2), new Rect3D(0.5, -1.5, 1.5, 2, 2, 2), new Rect3D(0.5, -2.5, 0.5, 2, 2, 2),
                                        new Rect3D(1.5, -1.5, 1.5, 2, 2, 2), new Rect3D(1.5, -2.5, 0.5, 2, 2, 2), new Rect3D(1.5, -2.5, 1.5, 2, 2, 2),
                                        new Rect3D(-0.5, -1.5, 0.5, 2, 2, 2), new Rect3D(0.5, -1.5, -0.5, 2, 2, 2), new Rect3D(0.5, -0.5, 0.5, 2, 2, 2),
                                        new Rect3D(-0.5, -1.5, -0.5, 2, 2, 2), new Rect3D(-0.5, -0.5, 0.5, 2, 2, 2), new Rect3D(-0.5, -0.5, -0.5, 2, 2, 2),
                                        new Rect3D(0, -2, 0, 2, 2, 2), new Rect3D(Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN),
                                        new Rect3D(Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN),
                                        new Rect3D(0.5, 0.5, 0.5, 0, 0, 0), new Rect3D(Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN),
                                        new Rect3D(Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN),
                                        new Rect3D(Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN) };

            for (int i = 0; i < inputRects.Length; i++)
            {
                Rect3D outputRect = transformGroup.TransformBounds(inputRects[i]);

                if (MathEx.AreCloseEnough(outputRect, expectedOutput[i]) == false)
                {
                    result = false;
                    Log("*** Error: Input = " + inputRects[i] + "   expected output = " + expectedOutput[i] + "   actual output = " + outputRect);
                }
            }

            if (result == false)
            {
                AddFailure("TryTransform of GeneralTransform3DGroup failed.");
            }
        }
    }
}