// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Provide a 3d context for the cache.
    /// </summary>
    class VP2DV3DContent : ChangeableContent
    {

        #region Public methods

        public override FrameworkElement Construct(Requirements req)
        {
            Viewport3D vp3D = new Viewport3D();

            /* Defining the Camera */
            vp3D.Camera = SetupCamera();

            /* Defining the light(s) */
            ModelVisual3D mv3D = new ModelVisual3D();
            DirectionalLight light = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            mv3D.Content = light;
            vp3D.Children.Add(mv3D);

            /* Defining a button on a 3D plane */
            _vp2Dv3D = new Viewport2DVisual3D();
            AxisAngleRotation3D axisTransform = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 20);
            RotateTransform3D transform = new RotateTransform3D(axisTransform);
            _vp2Dv3D.Transform = transform;

            _vp2Dv3D.Geometry = ConstructGeometry();

            _myMaterial = new DiffuseMaterial(Brushes.White);
            _myMaterial.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            _vp2Dv3D.Material = _myMaterial;

            DisplayButton = new Button();
            DisplayButton.Height = 100;
            DisplayButton.Width = 100;

            B = new SolidColorBrush(Colors.Red);
            DisplayButton.Background = B;
            _vp2Dv3D.Visual = DisplayButton;

            //create the animation for us to use later
            CreateAnimation();

            ////now add the cache.
            cache = CreateCache(renderAtScale);
            if (!req.cacheDisabled) { DisplayButton.CacheMode = cache; }            

            //layout because we're embedding in a vb/bcb and button
            vp3D.BeginInit();
            vp3D.Width = 100;
            vp3D.Height = 100;
            vp3D.EndInit();

            vp3D.Children.Add(_vp2Dv3D);
            vp3D.Measure(new Size(100, 100));
            vp3D.Arrange(new Rect(new Size(100, 100)));
            vp3D.UpdateLayout();
            
            Button OuterButton;
            if (req.bitmapCacheBrush)
            {
                OuterButton = WrapUIElementInBCB(vp3D, req.cacheOnBitmapCacheBrush);
            }
            else
            {
                VisualBrush vb = new VisualBrush(vp3D);
                OuterButton = new Button();
                OuterButton.Background = vb;
                OuterButton.BeginInit();
                OuterButton.Height = 100;
                OuterButton.Width = 100;
                OuterButton.EndInit();
            }

            return OuterButton;
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
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TriangleIndices = new Int32Collection();
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            return mesh;
        }

        public override TestResult Display() { return TestResult.Pass; }

        public override TestResult ChangeColor()
        {
            B = new SolidColorBrush(Colors.Green);
            DisplayButton.Background = B;
            //vp2Dv3D.Visual = DisplayButton;
            return TestResult.Pass;
        }

        #endregion

        #region members

        //GeometryModel3D myGeometryModel;
        DiffuseMaterial _myMaterial;
        Viewport2DVisual3D _vp2Dv3D;

        #endregion
    }
}
