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
    public partial class VideoTest
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            Application application = Application.Current as Application;
            if ( application.Properties[ "video" ] != null )
            {
                _videoUri = application.Properties[ "video" ] as string;
            }

            application.Properties[ "WindowWidth" ] = "200";
            application.Properties[ "WindowHeight" ] = "200";

            BuildScene();
        }

        public void BuildScene()
        {
            Light light = new AmbientLight();

            MeshGeometry3D mesh1 = MeshFactory.FullScreenMesh;
            Material mat1 = MakeVideoMaterial( new Uri( _videoUri, UriKind.RelativeOrAbsolute ) );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive1 );

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            OrthographicCamera camera = CameraFactory.OrthographicDefault;
            camera.Width = 0.5;
            VIEWPORT.Camera = camera;
        }

        public Material MakeVideoMaterial( Uri videoUri )
        {
            VideoDrawing vd = new VideoDrawing();

            MediaPlayer mp = new MediaPlayer();

            mp.Open( videoUri );

            mp.Play();

            vd.Player = mp;
            vd.Rect = new Rect( 0, 0, 256, 256 );

            DrawingBrush db = new DrawingBrush();
            db.Drawing = vd;

            return new DiffuseMaterial( db );
        }

        string _videoUri = "30SecondsOfRed.wmv";
    }
}
