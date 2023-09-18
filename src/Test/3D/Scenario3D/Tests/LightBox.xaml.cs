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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class LightBox : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nParameters:" +
                    "\n   /SphereU=           (Int) Number of sphere segments in the U dimension. Defult is 16." +
                    "\n   /SphereV=           (Int) Number of sphere segments in the V dimension. Defult is 8." +
                    "\n   /Seed=              (Int) Random number seed value. Defult is 12345." +
                    "\n   /CellSize=          (Int) Number of vertices to include in the lowest level quad tree. Defult is 24." +
                    "\n   /RandomizeMaterials (Bool) Toggle the use of multiple random Materials. Default is true." +
                    "\n   /UseHeightField     (Flag) Set to use a image based heightfield. Default is not set." +
                    "\n   /AllowNulls         (Flag) Set to use null brush/material random values. Default is not set." +
                    "\n   /AnimateCamera      (Flag) Set to add an animation to the Camera. Default is not set." +
                    "";
            }
        }

        int _sphereU = 16;
        int _sphereV = 8;
        double _sphereRadius = 1.2;
        bool _randomizeMaterials = true;
        bool _animateCamera = false;
        bool _allowNulls = false;
        int _seed = 12345;
        int _cellSize = 24;
        bool _useHeightField = false;
        Random _rand;

        public void OnLoaded( object sender, EventArgs args )
        {
            RenderTolerance.TextureLookUpTolerance = 0.5;

            ParseParameters();
            _rand = new Random( _seed );
            BuildScene();
        }

        public void ParseParameters()
        {
            // customize as needed
            Application application = Application.Current as Application;
            if ( application.Properties[ "SphereU" ] != null )
            {
                _sphereU = int.Parse( (string)application.Properties[ "SphereU" ] );
            }
            if ( application.Properties[ "SphereV" ] != null )
            {
                _sphereV = int.Parse( (string)application.Properties[ "SphereV" ] );
            }
            if ( application.Properties[ "Seed" ] != null )
            {
                _seed = int.Parse( (string)application.Properties[ "Seed" ] );
            }
            if ( application.Properties[ "RandomizeMaterials" ] != null )
            {
                _randomizeMaterials = bool.Parse( (string)application.Properties[ "RandomizeMaterials" ] );
            }
            if ( application.Properties[ "UseHeightField" ] != null )
            {
                _useHeightField = bool.Parse( (string)application.Properties[ "UseHeightField" ] );
            }
            if ( application.Properties[ "CellSize" ] != null )
            {
                _cellSize = int.Parse( (string)application.Properties[ "CellSize" ] );
            }
            if ( application.Properties[ "AllowNulls" ] != null )
            {
                _allowNulls = bool.Parse( (string)application.Properties[ "AllowNulls" ] );
            }
            if ( application.Properties[ "AnimateCamera" ] != null )
            {
                _animateCamera = bool.Parse( (string)application.Properties[ "AnimateCamera" ] );
            }

        }

        public void BuildScene()
        {
            // Latitude x Longitude (v x u)
            Model3DGroup gsphere = SceneFactory.GroupSphere(
                    _sphereV, _sphereU, _sphereRadius, MaterialFactory.Default );

            if ( _useHeightField )
            {
                // Use the heightfield mesh, with a quad tree
                BitmapImage bmp = new BitmapImage( new Uri( "heightField.bmp", UriKind.RelativeOrAbsolute ) );
                gsphere = SceneFactory.QuadTreeHeightField( bmp, _cellSize, _cellSize, MaterialFactory.Default );
            }

            if ( _randomizeMaterials )
            {
                SetRandomMaterials( gsphere );
            }

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( gsphere );
            mg.Children.Add( new AmbientLight( Colors.DarkBlue ) );
            mg.Children.Add( new DirectionalLight( Colors.Red, new Vector3D( 1, 0, 0 ) ) );
            mg.Children.Add( new DirectionalLight( Colors.Blue, new Vector3D( -1, 0, 0 ) ) );
            mg.Children.Add( new DirectionalLight( Colors.Green, new Vector3D( 0, 0, -1 ) ) );
            mg.Children.Add( new DirectionalLight( Colors.Green, new Vector3D( 0, 0, 1 ) ) );
            mg.Children.Add( new PointLight( Colors.DarkGreen, new Point3D( 0, 1.5, 2 ) ) );
            mg.Children.Add( new SpotLight( Colors.DarkOrchid, new Point3D( 1, 1.5, 1 ), new Vector3D( -1, -1.5, -1 ), 45, 30 ) );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );

            ProjectionCamera cam = CameraFactory.PerspectiveDefault;
            if ( _animateCamera )
            {
                Point3DAnimation pa = new Point3DAnimation(
                        new Point3D( 0, -0.25, 5 ),
                        new Point3D( 0, 0.75, 4 ),
                        new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                pa.AutoReverse = true;
                pa.RepeatBehavior = RepeatBehavior.Forever;
                cam.BeginAnimation( ProjectionCamera.PositionProperty, pa );
            }
            VIEWPORT.Camera = cam;
        }

        public void SetRandomMaterials( Model3DGroup mg )
        {
            foreach ( Model3D model in mg.Children )
            {
                if ( model is GeometryModel3D )
                {
                    GeometryModel3D gm = model as GeometryModel3D;
                    gm.Material = MaterialFactory.GetRandomMaterial( _rand.Next(), false, _allowNulls );
                }
#if SSL
                else if ( model is ScreenSpaceLines3D )
                {
                    ScreenSpaceLines3D ssl = model as ScreenSpaceLines3D;
                    ssl.Color = MaterialFactory.GetRandomColor( rand.Next(), false );
                }
#endif
                else if ( model is Model3DGroup )
                {
                    SetRandomMaterials( model as Model3DGroup );
                }
            }
        }

    }
}
