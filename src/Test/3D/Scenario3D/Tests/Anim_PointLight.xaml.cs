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
    public partial class Anim_PointLight
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            PointLight light = new PointLight();
            light.ConstantAttenuation = 1;
            light.LinearAttenuation = 0;
            light.QuadraticAttenuation = 0;
            light.Range = 100;

            ColorAnimation ca = new ColorAnimation( Colors.Cyan, Colors.Yellow, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
            ca.AutoReverse = true;
            ca.RepeatBehavior = RepeatBehavior.Forever;
            light.BeginAnimation( Light.ColorProperty, ca );

            Point3DAnimation pa = new Point3DAnimation(
                new Point3D( 0, 5, 0 ), new Point3D( 2.5, 0, 5 ), new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
            pa.AutoReverse = true;
            pa.RepeatBehavior = RepeatBehavior.Forever;
            light.BeginAnimation( PointLight.PositionProperty, pa );

            MeshGeometry3D mesh1 = MeshFactory.Sphere( 20, 20, 1.2 );
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
