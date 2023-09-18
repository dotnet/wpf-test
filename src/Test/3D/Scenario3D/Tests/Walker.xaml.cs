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
    public partial class Walker
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D( 0, -1, -5 );

            Model3DGroup walker = SceneFactory.BipedWalker(
                    new DiffuseMaterial( new SolidColorBrush( Color.FromArgb( 255, 128, 0, 0 ) ) ),
                    new DiffuseMaterial( new SolidColorBrush( Color.FromArgb( 255, 128, 128, 13 ) ) ),
                    new DiffuseMaterial( new SolidColorBrush( Color.FromArgb( 255, 64, 0, 32 ) ) ),
                    new DiffuseMaterial( new SolidColorBrush( Color.FromArgb( 255, 27, 128, 64 ) ) ),
                    true );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( walker );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );

            PerspectiveCamera camera = CameraFactory.PerspectiveDefault;
            camera.Position = new Point3D( 2, .67, 4 );
            camera.LookDirection = new Vector3D( -3, -1, -5 );
            VIEWPORT.Camera = camera;
        }
    }
}
