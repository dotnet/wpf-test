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
    public partial class Anim_DirectionalLight
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();

            ColorAnimation ca = new ColorAnimation( Colors.Cyan, Colors.Yellow, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
            ca.AutoReverse = true;
            ca.RepeatBehavior = RepeatBehavior.Forever;
            light.BeginAnimation( Light.ColorProperty, ca );

            Vector3DAnimation va = new Vector3DAnimation(
                new Vector3D( 0, 1, 0 ), new Vector3D( -.5, 0, -1 ), new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
            va.AutoReverse = true;
            va.RepeatBehavior = RepeatBehavior.Forever;
            light.BeginAnimation( DirectionalLight.DirectionProperty, va );

            MeshGeometry3D mesh1 = MeshFactory.Sphere( 12, 12, 1.2 );
            Material mat1 = new DiffuseMaterial( Brushes.Gray );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            VIEWPORT.Camera = CameraFactory.PerspectiveDefault;
        }
    }
}
