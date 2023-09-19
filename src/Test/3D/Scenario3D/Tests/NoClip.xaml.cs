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
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class NoClip
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            MeshGeometry3D mesh1 = MeshFactory.FullScreenMesh;
            Material mat1 = new DiffuseMaterial( Brushes.Green );
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( new AmbientLight() );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            OrthographicCamera oc = CameraFactory.OrthographicDefault;
            oc.Width = 0.2;
            VIEWPORT.Camera = oc;
        }
    }
}
