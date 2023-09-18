// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;
using Microsoft.Test.Graphics.ReferenceRender;

// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    // http://www.playgo.to/gs/about.html

    public partial class GoBoard : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /UseCube           use a cube instead of a simple plane as a board." +
                    "\n   /SpinCamera        move the camera around the board." +
                    "\n   /Moves             specify the moves played on the board. See http://www.playgo.to/gs/about.html for a compatible format." +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            RenderTolerance.TextureLookUpTolerance = .5;
            RenderTolerance.PixelToEdgeTolerance = .2;
            BuildScene();
        }

        public void BuildScene()
        {
            Application application = Application.Current as Application;
            bool useCube = false;
            if ( application.Properties[ "UseCube" ] != null )
            {
                useCube = true;
            }

            _engine = new GoEngine();
            _renderer = new GoRenderer3D( _engine, useCube );

            if ( application.Properties[ "Moves" ] != null )
            {
                _engine.Parse( application.Properties[ "Moves" ] as string );
            }

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = _renderer.Render();
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            PerspectiveCamera camera = CameraFactory.PerspectiveDefault;
            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 100;
            camera.FieldOfView = 60;
            camera.Position = new Point3D( 0, 18, 12 );
            camera.LookDirection = new Vector3D( 0, -18, -12 );
            camera.UpDirection = new Vector3D( 0, 1, 0 );

            if ( application.Properties[ "SpinCamera" ] != null )
            {
                Point3DAnimationUsingKeyFrames pa = new Point3DAnimationUsingKeyFrames();
                pa.AutoReverse = false;
                pa.Duration = new TimeSpan( 0, 0, 0, 0, 2000 );

                pa.RepeatBehavior = RepeatBehavior.Forever;
                LinearPoint3DKeyFrame kf;

                kf = new LinearPoint3DKeyFrame( new Point3D( 0, 18, 12 ), TimeSpan.FromMilliseconds( 0 ) );
                pa.KeyFrames.Add( kf );

                kf = new LinearPoint3DKeyFrame( new Point3D( 12, 18, 0 ), TimeSpan.FromMilliseconds( 500 ) );
                pa.KeyFrames.Add( kf );

                kf = new LinearPoint3DKeyFrame( new Point3D( 0, 18, -12 ), TimeSpan.FromMilliseconds( 1000 ) );
                pa.KeyFrames.Add( kf );

                kf = new LinearPoint3DKeyFrame( new Point3D( -12, 18, 0 ), TimeSpan.FromMilliseconds( 1500 ) );
                pa.KeyFrames.Add( kf );

                kf = new LinearPoint3DKeyFrame( new Point3D( 0, 18, 12 ), TimeSpan.FromMilliseconds( 2000 ) );
                pa.KeyFrames.Add( kf );

                pa.SpeedRatio = 0.1;
                camera.BeginAnimation( ProjectionCamera.PositionProperty, pa );
            }

            VIEWPORT.Camera = camera;
        }

        GoEngine _engine;
        GoRenderer3D _renderer;
    }
}
