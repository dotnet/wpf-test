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
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class Integrate2D
    {
        public void OnLoaded( object sender, EventArgs args )
        {
            RenderTolerance.TextureLookUpTolerance = .5;
            BuildScene();
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D( 0, -1, 0 );

            MeshGeometry3D mesh1 = MeshFactory.SimpleCubeMesh;
            Material mat1 = MaterialFactory.Default;
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );
            primitive1.Transform = new RotateTransform3D( new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), 30 ) );

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
