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
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    public partial class AnimatedCubeSphere
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            // Set some defaults for scenario
            RenderTolerance.PixelToEdgeTolerance = 0.07;
            BuildScene();
        }

        public void BuildScene()
        {
            Light light = new DirectionalLight();
            ( (DirectionalLight)light ).Direction = new Vector3D( 0, -1, 0 );
            Vector3DAnimation val = new Vector3DAnimation();
            val.From = new Vector3D( 0, -1, 0 );
            val.To = new Vector3D( 0, 0, -1 );
            val.Duration = new Duration( TimeSpan.FromMilliseconds( 1600 ) );
            val.RepeatBehavior = RepeatBehavior.Forever;
            val.AutoReverse = true;
            light.BeginAnimation( DirectionalLight.DirectionProperty, val );

            MeshGeometry3D mesh = MeshFactory.Sphere( 20, 20, 1.25 );
            MaterialGroup mat = new MaterialGroup();
            mat.Children.Add( new DiffuseMaterial( BrushFactory.BrushSolidColorAnimated ) );
            SpecularMaterial sm = new SpecularMaterial( Brushes.White, 1 );
            DoubleAnimation da = new DoubleAnimation();
            da.From = 20.0;
            da.To = 100.0;
            da.Duration = new Duration( TimeSpan.FromMilliseconds( 800 ) );
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            sm.BeginAnimation( SpecularMaterial.SpecularPowerProperty, da );
            mat.Children.Add( sm );
            mat.Children.Add( new EmissiveMaterial( Brushes.Black ) );

            GeometryModel3D primitive = new GeometryModel3D( mesh, mat );
            TranslateTransform3D tx = new TranslateTransform3D( new Vector3D( 0, 1, 0 ) );
            da = new DoubleAnimation();
            da.From = 0;
            da.To = 4;
            da.Duration = new Duration( TimeSpan.FromMilliseconds( 1750 ) );
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;
            tx.BeginAnimation( TranslateTransform3D.OffsetYProperty, da );
            primitive.Transform = tx;

            MeshGeometry3D mesh2 = MeshFactory.SimpleCubeMesh;
            GeometryModel3D primitive2 = new GeometryModel3D( mesh2, mat );
            TranslateTransform3D tx2 = new TranslateTransform3D( new Vector3D( -3.5, 0, 0 ) );
            primitive2.Transform = tx2;

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );

            DirectionalLight light2 = new DirectionalLight();
            light2.Direction = new Vector3D( 0, 0, 1 );
            mg.Children.Add( light2 );

            mg.Children.Add( primitive2 );
            mg.Children.Add( primitive );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );

            // update camera to look at new primitive
            ScenarioUtility.UpdateCameraToLookAtPrimitive( primitive, VIEWPORT );
            ProjectionCamera cam = (ProjectionCamera)VIEWPORT.Camera.Clone();
            Point3DAnimation pa = new Point3DAnimation();
            pa.By = new Point3D( -4, 0, 0 );
            pa.Duration = new Duration( TimeSpan.FromMilliseconds( 2000 ) );
            pa.RepeatBehavior = RepeatBehavior.Forever;
            pa.AutoReverse = true;
            cam.BeginAnimation( ProjectionCamera.PositionProperty, pa );
            VIEWPORT.Camera = cam;
        }
    }
}
