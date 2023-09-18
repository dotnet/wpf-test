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
    public partial class Perf_FillRate : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /AnimateCamera             (Flag) Set to move camera." +
                    "\n   /MeshCount                 (Int)  Number of meshes." +
                    "\n   /NegativeScale             (Flag) Set to make Z-scale negative." +
                    "\n   /SmallScale                (Flag) Make the Mesh really small." +
                    "";
            }
        }


        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            int meshCount = 100;

            Application application = Application.Current as Application;

            MeshGeometry3D mesh = MeshFactory.FullScreenMesh;

            // common default parameters    
            PointLight light = new PointLight();
            light.Position = new Point3D( 0, 0, 30 );
            light.ConstantAttenuation = 1;
            light.LinearAttenuation = 0;
            light.QuadraticAttenuation = 0;
            light.Range = 200;

            PerspectiveCamera camera = (PerspectiveCamera)CameraFactory.PerspectiveDefault;
            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 200;
            camera.Position = new Point3D( .5, .5, 4 );
            camera.LookDirection = new Vector3D( 0, 0, -1 );

            // Test for Position
            if ( application.Properties[ "AnimateCamera" ] != null )
            {
                Point3DAnimation pa = new Point3DAnimation(
                        new Point3D( 0, 0, 4 ),
                        new Point3D( 1, 1, 4 ),
                        new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                pa.AutoReverse = true;
                pa.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.PositionProperty, pa );
            }
            VIEWPORT.Camera = camera;

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );


            if ( application.Properties[ "MeshCount" ] != null )
            {
                meshCount = int.Parse( (string)application.Properties[ "MeshCount" ] );
            }
            double scale = 1.0 / meshCount;
            double ZScale = scale;
            if ( application.Properties[ "NegativeScale" ] != null )
            {
                ZScale *= -1;
            }

            for ( int i=0; i<meshCount; i++ )
            {
                Material mat = new DiffuseMaterial( BrushFactory.GetRandomSolidColorBrush( 1234+i, true ) );
                GeometryModel3D primitive = new GeometryModel3D( mesh, mat );

                Transform3DCollection tc = new Transform3DCollection();
                TranslateTransform3D translate = new TranslateTransform3D( new Vector3D( i*scale, i*scale, i*ZScale ) );
                if ( application.Properties[ "SmallScale" ] != null )
                {
                    ScaleTransform3D scaleTR = new ScaleTransform3D( new Vector3D( 0.01, 0.01, 1 ) );
                    tc.Add( scaleTR );
                }
                tc.Add( translate );
                Transform3DGroup tg = new Transform3DGroup();
                tg.Children = tc;

                primitive.Transform = tg;
                mg.Children.Add( primitive );
            }
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }

    }
}
