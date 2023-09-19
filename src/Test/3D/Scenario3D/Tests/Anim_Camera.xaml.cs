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

// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    public partial class Anim_Camera : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /UseOrthographicCamera     use an OrthographicCamera instead of a PerspectiveCamera." +
                    "\n   /TestWidth                 test Width animations (OrthographicCamera only)." +
                    "\n   /TestFieldOfView           test FieldOfView animations (PerspectiveCamera only)." +
                    "\n   /TestNearPlaneDistance     test NearPlaneDistance animations." +
                    "\n   /TestFarPlaneDistance      test FarPlaneDistance animations." +
                    "\n   /TestPosition              test Position animations." +
                    "\n   /TestLookAtPoint           test LookAtPoint animations." +
                    "\n   /TestUp                    test Up animations." +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            Application application = Application.Current as Application;

            ProjectionCamera camera;
            if ( application.Properties[ "UseOrthographicCamera" ] != null )
            {
                // Orthographic
                camera = new OrthographicCamera();

                // defaults
                camera.Position = new Point3D( 0, 35, -50 );
                camera.LookDirection = new Vector3D( 0, -20, 50 );
                ( (OrthographicCamera)camera ).Width = 20;

                // Test for Width
                if ( application.Properties[ "TestWidth" ] != null )
                {
                    ( (OrthographicCamera)camera ).Width = 0;  // this will be the default, which should be overriden by the animation
                    DoubleAnimation da = new DoubleAnimation( 5.0, 35.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                    da.AutoReverse = true;
                    da.RepeatBehavior = RepeatBehavior.Forever;
                    camera.BeginAnimation( OrthographicCamera.WidthProperty, da );
                }
            }
            else
            {
                // default to perspective
                camera = new PerspectiveCamera();

                // defaults
                camera.Position = new Point3D( 0, 35, -50 );
                camera.LookDirection = new Vector3D( 0, -13, 50 );
                ( (PerspectiveCamera)camera ).FieldOfView = 45;

                // Test for FieldOfView
                if ( application.Properties[ "TestFieldOfView" ] != null )
                {
                    ( (PerspectiveCamera)camera ).FieldOfView = 0;  // this will be the default, which should be overriden by the animation
                    DoubleAnimation da = new DoubleAnimation( 5.0, 90.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                    da.AutoReverse = true;
                    da.RepeatBehavior = RepeatBehavior.Forever;
                    camera.BeginAnimation( PerspectiveCamera.FieldOfViewProperty, da );
                }
            }

            // defaults
            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 200;
            camera.UpDirection = new Vector3D( 0, 1, 0 );

            // Test for NearPlaneDistance
            if ( application.Properties[ "TestNearPlaneDistance" ] != null )
            {
                camera.NearPlaneDistance = 0;  // this will be the default, which should be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 40.0, 100.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.NearPlaneDistanceProperty, da );
            }
            // Test for FarPlaneDistance
            if ( application.Properties[ "TestFarPlaneDistance" ] != null )
            {
                camera.FarPlaneDistance = 0;  // this will be the default, which should be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 40.0, 100.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.FarPlaneDistanceProperty, da );
            }
            // Test for Position
            if ( application.Properties[ "TestPosition" ] != null )
            {
                camera.Position = new Point3D( 0, 0, 0 ); // this should be overriden by the animation
                Point3DAnimation pa = new Point3DAnimation(
                    new Point3D( -10, 35, -50 ), new Point3D( 10, 35, -50 ), new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                pa.AutoReverse = true;
                pa.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.PositionProperty, pa );
            }
            // Test for LookDirection
            if ( application.Properties[ "TestLookDirection" ] != null )
            {
                Vector3D from = new Vector3D( -10, -13, 50 );
                Vector3D to = new Vector3D( 10, -13, 50 );
                camera.LookDirection = new Vector3D( 0, 0, -1 ); // this should be overriden by the animation

                Vector3DAnimation va = new Vector3DAnimation( from, to, TimeSpan.FromMilliseconds( 1500 ) );
                va.AutoReverse = true;
                va.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.LookDirectionProperty, va );
            }
            // Test for UpDirection
            if ( application.Properties[ "TestUp" ] != null )
            {
                camera.UpDirection =new Vector3D( 0, 1, 0 ); // this should be overriden by the animation
                Vector3DAnimation va = new Vector3DAnimation(
                    new Vector3D( 1, 0, 0 ), new Vector3D( 0, 1, 0 ), new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                va.AutoReverse = true;
                va.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.UpDirectionProperty, va );
            }


            // set camera
            VIEWPORT.Camera = camera;

            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D( 0, -1, .7 );

            // load the heightfield from file, which is 100x100
            MeshGeometry3D mesh1 = MeshFactory.CreateGridFromImage(
                System.IO.Directory.GetCurrentDirectory() + "\\heightfield.bmp", 15.5 );
            Material mat1 = new DiffuseMaterial( new LinearGradientBrush( Colors.Green, Colors.Brown, 120 ) );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            // move the center of the grid to the origin.
            primitive1.Transform = new TranslateTransform3D( new Vector3D( -50, 0, -50 ) );

            // create sun by setting up a flat disc ... or a radial gradient on a quad when that's done :)
            MeshGeometry3D mesh3 = MeshFactory.CreateFlatDisc( 20 );
            Material mat3 = new EmissiveMaterial( new LinearGradientBrush( Colors.Yellow, Colors.Orange, 0 ) );
            GeometryModel3D primitive3 = new GeometryModel3D( mesh3, mat3 );
            primitive3.BackMaterial = mat3;
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
