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

namespace Microsoft.Test.Graphics
{
    public partial class SphereGroup : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /sphereU               Number of segments in the U direction (default=30)." +
                    "\n   /sphereV               Number of segments in the V direction (default=30)." +
                    "\n   /sphereRadius          Radius (default=1.5)." +
                    "\n   /sphereUseGroup        True for modelgroup with a mesh per face, False for single mesh (default=true)." +
                    "\n   /sphereUseTransfrom    True for adding a transform to each face (requires sphereUseGroup=true)(default=false)." +
                    "\n   /sphereUseAnimation    True for adding an animated transform to each face (requires sphereUseGroup=true)(default=false)." +
                    "\n   /sphereAngle           Angle of roation transform applied by either of the above (default=45.0)." +
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
            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D( 0, -1, -1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );

            Material bm = MaterialFactory.Default;

            // customization ... :)
            Application application = Application.Current as Application;

            // set defaults
            int sphereU = 30;
            int sphereV = 30;
            double sphereRadius = 1.5;
            bool sphereUseGroup = true;
            bool sphereUseTransfrom = false;
            bool sphereUseAnimation = false;
            double sphereAngle = 45.0;

            // customize as needed
            if ( application.Properties[ "sphereU" ] != null )
            {
                sphereU = int.Parse( (string)application.Properties[ "sphereU" ] );
            }
            if ( application.Properties[ "sphereV" ] != null )
            {
                sphereV = int.Parse( (string)application.Properties[ "sphereV" ] );
            }
            if ( application.Properties[ "sphereRadius" ] != null )
            {
                sphereRadius = double.Parse( (string)application.Properties[ "sphereRadius" ] );
            }
            if ( application.Properties[ "sphereUseGroup" ] != null )
            {
                sphereUseGroup = bool.Parse( (string)application.Properties[ "sphereUseGroup" ] );
            }
            if ( application.Properties[ "sphereUseTransfrom" ] != null )
            {
                sphereUseTransfrom = bool.Parse( (string)application.Properties[ "sphereUseTransfrom" ] );
            }
            if ( application.Properties[ "sphereUseAnimation" ] != null )
            {
                sphereUseAnimation = bool.Parse( (string)application.Properties[ "sphereUseAnimation" ] );
            }
            if ( application.Properties[ "sphereAngle" ] != null )
            {
                sphereAngle = double.Parse( (string)application.Properties[ "sphereAngle" ] );
            }


            // solid or group ?
            if ( sphereUseGroup )
            {
                // use a group of faces 
                Model3DGroup gsphere = SceneFactory.GroupSphere( sphereU, sphereV, sphereRadius, bm );
                // with or without a transform
                if ( sphereUseTransfrom )
                {
                    AddTransform( gsphere, sphereAngle );
                }
                // with or without animation, which overrides the transform
                if ( sphereUseAnimation )
                {
                    AddAnimation( gsphere, sphereAngle );
                }
                mg.Children.Add( gsphere );
            }
            else
            {
                // use a single mesh
                GeometryModel3D model = new GeometryModel3D( MeshFactory.Sphere( sphereV, sphereU, sphereRadius ), bm );
                mg.Children.Add( model );
            }

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            VIEWPORT.Camera = CameraFactory.PerspectiveDefault;
        }

        protected DoubleAnimation GenerateSimpleAnimation( int durationInMs, double angleTo )
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration( TimeSpan.FromMilliseconds( durationInMs ) );
            animation.From = 0;
            animation.To = angleTo;
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            return animation;
        }

        protected DoubleAnimation GenerateSimpleDoubleAnimation( int durationInMs, double position )
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration( TimeSpan.FromMilliseconds( durationInMs ) );
            animation.From = 0;
            animation.To = position * 0.2;
            animation.AutoReverse = true;
            animation.RepeatBehavior = RepeatBehavior.Forever;
            return animation;
        }

        private void AddAnimation( Model3DGroup g, double angleInDegrees )
        {
            Vector3D yAxis = new Vector3D( 0, 1, 0 );

            foreach ( GeometryModel3D gm in g.Children )
            {
                DoubleAnimation xAnim = GenerateSimpleDoubleAnimation( 1000, ( (MeshGeometry3D)gm.Geometry ).Positions[ 0 ].X );
                DoubleAnimation yAnim = GenerateSimpleDoubleAnimation( 1000, ( (MeshGeometry3D)gm.Geometry ).Positions[ 0 ].Y );
                DoubleAnimation zAnim = GenerateSimpleDoubleAnimation( 1000, ( (MeshGeometry3D)gm.Geometry ).Positions[ 0 ].Z );
                TranslateTransform3D translate = new TranslateTransform3D( new Vector3D( 0, 0, 0 ) );
                translate.BeginAnimation( TranslateTransform3D.OffsetXProperty, xAnim );
                translate.BeginAnimation( TranslateTransform3D.OffsetYProperty, yAnim );
                translate.BeginAnimation( TranslateTransform3D.OffsetZProperty, zAnim );
                gm.Transform = translate;
            }
        }

        private void AddTransform( Model3DGroup g, double angleInDegrees )
        {
            Vector3D yAxis = new Vector3D( 0, 1, 0 );

            foreach ( GeometryModel3D gm in g.Children )
            {
                RotateTransform3D rot = new RotateTransform3D( new AxisAngleRotation3D( yAxis, angleInDegrees ), ( (MeshGeometry3D)gm.Geometry ).Positions[ 0 ] );
                gm.Transform = rot;
            }
        }

    }
}
