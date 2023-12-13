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
    public partial class Anim_SpecularPower
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Color = Colors.White;
            light.Direction = new Vector3D( 0, 0, -1 );

            PerspectiveCamera camera = CameraFactory.PerspectiveDefault;
            camera.Position = new Point3D( 0, 0, 3 );
            VIEWPORT.Camera = camera;

            MeshGeometry3D mesh1 = MeshFactory.CreateFlatGridUV( 40, 40, 0, new Point( 0, 0 ), new Point( 1, 1 ) );
            Material mat1 = MaterialFactory.MakeMaterial( "255,255,128,0", "Specular", 0 );
            DoubleAnimation da = new DoubleAnimation( 1.0, 100.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
            da.AutoReverse = true;
            da.RepeatBehavior = RepeatBehavior.Forever;
            mat1.BeginAnimation( SpecularMaterial.SpecularPowerProperty, da );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }
    }
}
