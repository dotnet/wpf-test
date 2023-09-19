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
    public partial class Anim_Transforms : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /TestScaleVector         test Scale animations (scale)." +
                    "\n   /TestScaleCenter         test Center animations (scale)." +
                    "\n   /TestOffset              test Offset animations (translate)." +
                    "\n   /TestCenter              test Center animations (rotate)." +
                    "\n   /TestAxisRotation        test Axis animations (rotate)." +
                    "\n   /TestAngleRotation       test Angle animations (rotate)." +
                    "\n   /TestQuaternionRotation  test Quaternion animations (rotate)." +
                    "\n       /NoShortcuts             set UseShortestPath = false on /TestQuaternionRotation" +
                    "\n   /TestRotation3D=(AxisAngle|Quaternion|Mixed)   [default = AxisAngle]" +
                    "\n                            test Rotation3D animations using different Rotation3Ds." +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            _application = Application.Current as Application;
            BuildScene();
        }

        private Application _application;

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Color = Colors.White;
            light.Direction = new Vector3D( 0, -1, 0 );

            PerspectiveCamera camera = CameraFactory.PerspectiveDefault;
            camera.Position = new Point3D( 8, 2.5, 5 );
            camera.LookDirection = new Vector3D( -8, -2.5, -5 );
            VIEWPORT.Camera = camera;

            MeshGeometry3D mesh = MeshFactory.SimpleCubeMesh;
            Material mat = MaterialFactory.Default;
            GeometryModel3D primitive1 = new GeometryModel3D( mesh, mat );
            Transform3DGroup tc = new Transform3DGroup();
            tc.Children = new Transform3DCollection( new Transform3D[] { Transform3D.Identity } );

            // Test for Scale
            ScaleTransform3D scale = CreateScaleTransform3D();
            tc.Children.Add( scale );

            // Test for Translation
            TranslateTransform3D translate = CreateTranslateTransform3D();
            tc.Children.Add( translate );

            // Test for Rotation
            RotateTransform3D rotation = CreateRotateTransform3D();
            tc.Children.Add( rotation );

            primitive1.Transform = tc;

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }

        private ScaleTransform3D CreateScaleTransform3D()
        {
            ScaleTransform3D scale = new ScaleTransform3D( new Vector3D( 1.08, 1.08, 1.08 ) );

            if ( _application.Properties[ "TestScaleVector" ] != null )
            {
                DoubleAnimation daX = new DoubleAnimation( .5, 2, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daY = new DoubleAnimation( 1, 1.5, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daZ = new DoubleAnimation( -2, 3, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                daX.AutoReverse = true;
                daY.AutoReverse = true;
                daZ.AutoReverse = true;
                daX.RepeatBehavior = RepeatBehavior.Forever;
                daY.RepeatBehavior = RepeatBehavior.Forever;
                daZ.RepeatBehavior = RepeatBehavior.Forever;

                scale.BeginAnimation( ScaleTransform3D.ScaleXProperty, daX );
                scale.BeginAnimation( ScaleTransform3D.ScaleYProperty, daY );
                scale.BeginAnimation( ScaleTransform3D.ScaleZProperty, daZ );
            }
            if ( _application.Properties[ "TestScaleCenter" ] != null )
            {
                DoubleAnimation daX = new DoubleAnimation( -1, 2, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daY = new DoubleAnimation( -1, 1, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daZ = new DoubleAnimation( -1, 3, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                daX.AutoReverse = true;
                daY.AutoReverse = true;
                daZ.AutoReverse = true;
                daX.RepeatBehavior = RepeatBehavior.Forever;
                daY.RepeatBehavior = RepeatBehavior.Forever;
                daZ.RepeatBehavior = RepeatBehavior.Forever;

                scale.BeginAnimation( ScaleTransform3D.CenterXProperty, daX );
                scale.BeginAnimation( ScaleTransform3D.CenterYProperty, daY );
                scale.BeginAnimation( ScaleTransform3D.CenterZProperty, daZ );
            }
            return scale;
        }

        private TranslateTransform3D CreateTranslateTransform3D()
        {
            TranslateTransform3D translate = new TranslateTransform3D( new Vector3D( 0.1, 0.1, 0.1 ) );

            if ( _application.Properties[ "TestOffset" ] != null )
            {
                DoubleAnimation daX = new DoubleAnimation( -1, 2, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daY = new DoubleAnimation( -1, 1.5, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daZ = new DoubleAnimation( -1, .5, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                daX.AutoReverse = true;
                daY.AutoReverse = true;
                daZ.AutoReverse = true;
                daX.RepeatBehavior = RepeatBehavior.Forever;
                daY.RepeatBehavior = RepeatBehavior.Forever;
                daZ.RepeatBehavior = RepeatBehavior.Forever;

                translate.BeginAnimation( TranslateTransform3D.OffsetXProperty, daX );
                translate.BeginAnimation( TranslateTransform3D.OffsetYProperty, daY );
                translate.BeginAnimation( TranslateTransform3D.OffsetZProperty, daZ );

            }
            return translate;
        }

        private RotateTransform3D CreateRotateTransform3D()
        {
            RotateTransform3D rotation = new RotateTransform3D();

            if ( _application.Properties[ "TestAxisRotation" ] != null )
            {
                Vector3DAnimation va = new Vector3DAnimation( new Vector3D( 0, 1, 0 ), new Vector3D( 1, 0, 0 ), TimeSpan.FromMilliseconds( 1500 ) );
                va.AutoReverse = true;
                va.RepeatBehavior = RepeatBehavior.Forever;

                if ( rotation.Rotation == Rotation3D.Identity )
                {
                    rotation.Rotation = new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 45 );
                }
                rotation.Rotation.BeginAnimation( AxisAngleRotation3D.AxisProperty, va );
            }
            if ( _application.Properties[ "TestAngleRotation" ] != null )
            {
                DoubleAnimation da = new DoubleAnimation( 0, 75, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;

                if ( rotation.Rotation == Rotation3D.Identity )
                {
                    rotation.Rotation = new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 10 );
                }
                rotation.Rotation.BeginAnimation( AxisAngleRotation3D.AngleProperty, da );
            }
            if ( _application.Properties[ "TestQuaternionRotation" ] != null )
            {
                Quaternion dest = new Quaternion( new Vector3D( 0, 1, 0 ), 270 );
                QuaternionAnimation qa = new QuaternionAnimation( Quaternion.Identity, dest, TimeSpan.FromMilliseconds( 1500 ) );
                qa.AutoReverse = true;
                qa.RepeatBehavior = RepeatBehavior.Forever;
                if ( _application.Properties[ "NoShortcuts" ] != null )
                {
                    qa.UseShortestPath = false;
                }

                if ( rotation.Rotation == Rotation3D.Identity )
                {
                    rotation.Rotation = new QuaternionRotation3D( Const.qZ135 );
                }
                rotation.Rotation.BeginAnimation( QuaternionRotation3D.QuaternionProperty, qa );
            }
            if ( _application.Properties[ "TestRotation3D" ] != null )
            {
                QuaternionRotation3D qr1 = new QuaternionRotation3D( new Quaternion( new Vector3D( 1, 0, 0 ), 0 ) );
                QuaternionRotation3D qr2 = new QuaternionRotation3D( new Quaternion( new Vector3D( 1, 0, 0 ), 135 ) );
                AxisAngleRotation3D aar1 = new AxisAngleRotation3D( new Vector3D( 1, 0, 0 ), 0 );
                AxisAngleRotation3D aar2 = new AxisAngleRotation3D( new Vector3D( 1, 0, 0 ), 135 );
                Rotation3DAnimation ra;

                switch ( (string)_application.Properties[ "TestRotation3D" ] )
                {
                case "AxisAngle":
                    ra = new Rotation3DAnimation( aar1, aar2, TimeSpan.FromMilliseconds( 1500 ) );
                    break;

                case "Quaternion":
                    ra = new Rotation3DAnimation( qr1, qr2, TimeSpan.FromMilliseconds( 1500 ) );
                    break;

                case "Mixed":
                    ra = new Rotation3DAnimation( qr1, aar2, TimeSpan.FromMilliseconds( 1500 ) );
                    break;

                default:
                    throw new ArgumentException( "Invalid Rotation3D parameter: "+_application.Properties[ "TestRotation3D" ] );
                }
                ra.AutoReverse = true;
                ra.RepeatBehavior = RepeatBehavior.Forever;

                if ( rotation.Rotation == Rotation3D.Identity )
                {
                    rotation.Rotation = new QuaternionRotation3D( Const.qZ135 );
                }
                rotation.BeginAnimation( RotateTransform3D.RotationProperty, ra );
            }
            if ( _application.Properties[ "TestCenter" ] != null )
            {
                DoubleAnimation daX = new DoubleAnimation( -1, 2, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daY = new DoubleAnimation( -1, 1, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                DoubleAnimation daZ = new DoubleAnimation( -1, 3, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                daX.AutoReverse = true;
                daY.AutoReverse = true;
                daZ.AutoReverse = true;
                daX.RepeatBehavior = RepeatBehavior.Forever;
                daY.RepeatBehavior = RepeatBehavior.Forever;
                daZ.RepeatBehavior = RepeatBehavior.Forever;

                if ( rotation.Rotation == Rotation3D.Identity )
                {
                    rotation.Rotation = new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 10 );
                }
                rotation.BeginAnimation( RotateTransform3D.CenterXProperty, daX );
                rotation.BeginAnimation( RotateTransform3D.CenterYProperty, daY );
                rotation.BeginAnimation( RotateTransform3D.CenterZProperty, daZ );
            }

            return rotation;
        }
    }
}
