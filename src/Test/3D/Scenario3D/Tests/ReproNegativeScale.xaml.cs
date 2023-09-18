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
    public partial class ReproNegativeScale
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();

            // Make sure the window has no borders            
            Application application = Application.Current;
            Window w = application.Windows[ 0 ];
            w.WindowStyle = WindowStyle.None;
            w.ResizeMode = ResizeMode.NoResize;

            // resize the VP3D to whatever windowsize we are given, 
            //   and move it by its width to counteract the negative scale.
            Viewport3D vp = VIEWPORT as Viewport3D;
            vp.Width = w.Width;
            vp.Height = w.Height;
            Canvas.SetLeft( vp, vp.Width );
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
            VIEWPORT.Camera = CameraFactory.OrthographicDefault;
        }
    }
}
