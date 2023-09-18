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
    /// Verify correct functional behavior of GeneralTransform2DTo3D methods:
    ///     TryTransform
    /// </summary>
    public class GeneralTransform2DTo3DTest : CoreGraphicsTest
    {      
        public override void RunTheTest()
        {
            TestTryTransform();
        }

        private void TestTryTransform()
        {
           bool result = true;

            // Create a simple 3D rectangle
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(new Point3D(-1, 1, -1));
            mesh.Positions.Add(new Point3D(-1, -1, -1));
            mesh.Positions.Add(new Point3D(1, -1, -1));
            mesh.Positions.Add(new Point3D(1, 1, -1));
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);

            // Map a button over the 3D rectangle using a Viewport2DVisual3D
            Button button = new Button();
            Viewport2DVisual3D vp2dv3d = new Viewport2DVisual3D();
            vp2dv3d.Geometry = mesh;
            vp2dv3d.Visual = button;
            Size size = new Size(100, 100);
            button.Arrange(new Rect(size));

            // Now use GeneralTransform2DTo3D to get 3D coords of known 2D points and see if they are correct
            GeneralTransform2DTo3D transform = button.TransformToAncestor(vp2dv3d);
            if (transform != null)
            {
                Point[] inputPoints = { new Point(0, 0), // top-left --> should return top-left corner of 3D rectangle
                                        new Point(0, size.Height), // bottom-left --> should return bottom-left corner of 3D rectangle
                                        new Point(size.Width, 0),  // top-right --> should return top-right corner of 3D rectangle
                                        new Point(size.Width, size.Height),  // bottom-right --> should return bottom-right corner of 3D rectangle
                                        new Point(size.Width * 0.5, size.Height * 0.5),  // cetner --> should return center of 3D rectangle
                                        new Point(double.NaN, double.NaN), // should return NaN
                                        new Point(size.Width * 2, size.Width * 2) // outside the bounds of the button --> should not be able to transform it
                                      };

                Point3D[] expectedPoints3D = { new Point3D(-1, 1, -1),  // top-left corner of 3D rectangle
                                               new Point3D(-1, -1, -1), // bottom-left corner of 3D rectangle
                                               new Point3D(1, 1, -1),   // top-right corner of 3D rectangle
                                               new Point3D(1, -1, -1),  // bottom-right corner of 3D rectangle
                                               new Point3D(0, 0, -1),   // center of 3D rectangle
                                               new Point3D(double.NaN, double.NaN, double.NaN),
                                               new Point3D(0, 0, 0) };

                bool[] canTransform = { true, true, true, true, true, true, false };

                for (int i = 0; i < inputPoints.Length; i++)
                {
                    Point3D p;
                    bool transformed = transform.TryTransform(inputPoints[i], out p);

                    if (transformed != canTransform[i] || MathEx.AreCloseEnough(p, expectedPoints3D[i]) == false)
                    {
                        result = false;
                        Log("*** Error: Input = " + inputPoints[i] + "   expected output = " + expectedPoints3D[i] + "   actual output = " + p);
                    }
                }
            }
            else
            {
                result = false;
                Log("*** Error: transform is NULL");
            }

            if (result == false)
            {
                AddFailure("TryTransform of GeneralTransform2DTo3D failed.");
            }
        }
    }
}