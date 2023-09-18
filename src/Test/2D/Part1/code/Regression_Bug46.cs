// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug46 
*/
using System;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Converters;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug46 
    /// </summary>
    [Test(1, "Regression", "Regression_Bug46",
        Area = "2D",
        Description = @"Regression test for Regression_Bug46 : crash in cider when bitmap cache is applied")]
    public class Regression_Bug46 : WindowTest
    {

        /// <summary>
        /// Constructor
        /// </summary>
        [Variation()]
        public Regression_Bug46()
            : base(false)
        {
            RunSteps += new TestStep(CreateWindowContent);
            RunSteps += new TestStep(Verify);
        }

        #region Test Methods
        /// <summary>
        /// Create window content
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CreateWindowContent()
        {
            Window.Content = ConstructPanel();
            Window.BeginInit();
            Window.Height = 100;
            Window.Width = 100;
            Window.EndInit();
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify we don't crash when repro steps applied
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            TestResult result = TestResult.Pass;
            ContainerVisual myVisual = (ContainerVisual)(_myButton.Parent);

            //if we don't overflow our stack here then this is a pass
            try
            {
                myVisual.TransformToAncestor(_myVp3D);
            }
            catch (StackOverflowException)
            {
                result = TestResult.Fail;
            }
            catch (Exception)
            {
            }

            return result;

        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// In this test, we need a vp3dv2d with a button as the v2d,
        /// wrapped in a stackpanel
        /// </summary>
        /// <returns>the constructed stackpanel</returns>
        private UIElement ConstructPanel()
        {
            Viewport3D vp3D = new Viewport3D();

            // Defining the Camera 
            vp3D.Camera = SetupCamera();

            // Defining the light(s) 
            ModelVisual3D mv3D = new ModelVisual3D();
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            mv3D.Content = light;
            vp3D.Children.Add(mv3D);

            // Defining a button on a 3D plane 
            _vp2Dv3D = new Viewport2DVisual3D();
            AxisAngleRotation3D axisTransform = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 20);
            RotateTransform3D transform = new RotateTransform3D(axisTransform);
            _vp2Dv3D.Transform = transform;

            _vp2Dv3D.Geometry = ConstructGeometry();

            _myMaterial = new DiffuseMaterial(System.Windows.Media.Brushes.White);
            _myMaterial.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            _vp2Dv3D.Material = _myMaterial;

            _myButton = new Button();
            _myButton.BeginInit();
            _myButton.Height = 100;
            _myButton.Width = 100;
            _myButton.EndInit();
            _myButton.Background = new SolidColorBrush(Colors.Red);
            _vp2Dv3D.Visual = _myButton;

            //now add the cache.
            BitmapCache cache = new BitmapCache(1.0);
            _myButton.CacheMode = cache;

            //layout because we're embedding in a vb/bcb and button
            vp3D.BeginInit();
            vp3D.Width = 100;
            vp3D.Height = 100;
            vp3D.EndInit();

            vp3D.Children.Add(_vp2Dv3D);
            vp3D.Measure(new System.Windows.Size(100, 100));
            vp3D.Arrange(new System.Windows.Rect(new System.Windows.Size(100, 100)));
            vp3D.UpdateLayout();

            _myPanel = new StackPanel();
            _myVp3D = vp3D;
            _myPanel.Children.Add(vp3D);

            return _myPanel;
        }

        private static PerspectiveCamera SetupCamera()
        {
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 4);
            return camera;
        }

        private static MeshGeometry3D ConstructGeometry()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions = new Point3DCollection();
            mesh.Positions.Add(new Point3D(-1, 1, 0));
            mesh.Positions.Add(new Point3D(-1, -1, 0));
            mesh.Positions.Add(new Point3D(1, -1, 0));
            mesh.Positions.Add(new Point3D(1, 1, 0));
            mesh.TextureCoordinates = new PointCollection();
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mesh.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mesh.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mesh.TriangleIndices = new Int32Collection();
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            return mesh;
        }
        #endregion

        #region members
        private DiffuseMaterial _myMaterial;
        private Viewport2DVisual3D _vp2Dv3D;
        private Viewport3D _myVp3D;
        private StackPanel _myPanel;
        private Button _myButton;
        #endregion
    }
}