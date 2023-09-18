// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class TerrainDemo
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();

            // override scene defaults if missing
            Application application = Application.Current as Application;
            if ( application.Properties[ "WindowWidth" ] == null )
            {
                application.Properties[ "WindowWidth" ] = "500";
            }
            if ( application.Properties[ "WindowHeight" ] == null )
            {
                application.Properties[ "WindowHeight" ] = "300";
            }
        }

        public void BuildScene()
        {
            PerspectiveCamera camera = (PerspectiveCamera)CameraFactory.PerspectiveDefault;
            camera.FarPlaneDistance = 200;
            camera.NearPlaneDistance = 1;
            camera.Position = new Point3D( 0, 35, -50 );
            camera.LookDirection = new Vector3D( 0, -13, 50 );
            VIEWPORT.Camera = camera;

            // light vector is in the same direction as the camera
            DirectionalLight light = new DirectionalLight();
            light.Direction = camera.LookDirection;

            // load the heightfield from file, which is 100x100
            MeshGeometry3D mesh1 = MeshFactory.CreateGridFromImage(
                System.IO.Directory.GetCurrentDirectory() + "\\heightfield.bmp", 15.5 );

            Material mat1 = new DiffuseMaterial( new LinearGradientBrush( Colors.Green, Colors.Brown, 120 ) );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );
            primitive1.Transform = new TranslateTransform3D( new Vector3D( -50, 0, -50 ) );

            // create sun by setting up a flat disc ... or a radial gradient on a quad when that's done :)
            MeshGeometry3D mesh3 = MeshFactory.CreateFlatDisc( 20 );
            Material mat3 = new EmissiveMaterial( new LinearGradientBrush( Colors.Yellow, Colors.Orange, 0 ) );
            GeometryModel3D primitive3 = new GeometryModel3D( mesh3, mat3 );
            primitive3.Transform = new TranslateTransform3D( new Vector3D( 0, 32, -10 ) );

            // add everything to the scene
            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive3 );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }
    }
}
