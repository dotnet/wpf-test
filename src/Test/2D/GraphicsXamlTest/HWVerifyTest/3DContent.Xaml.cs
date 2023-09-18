// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Data;
using System.Xml;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Configuration;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Interaction logic for Application.xaml
    /// </summary>

    public partial class ThreeDContent : Window
    {
        public ThreeDContent()
        {
            InitializeComponent();
        }

        public void RunTest(object sender, System.EventArgs e)
        {
            XamlTestHelper.AddStep(Create3DContent);
            XamlTestHelper.AddStep(XamlTestHelper.Quit);
            XamlTestHelper.Run();
        }

        private object Create3DContent(object arg)
        {
            Viewport3D vp = new Viewport3D();

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = GetModelGroup();
            vp.Children.Add( visual );
            vp.Camera = GetCamera();
            XamlTestHelper.LogStatus("Creating 3D content");
            this.Content = vp;
            return null;
        }

        public Camera GetCamera()
        {
            ProjectionCamera camera = new PerspectiveCamera(
            new Point3D( 0, 0, 5 ),      // Position
                new Vector3D( 0, 0, -1 ),  // LookDirection
                new Vector3D( 0, 1, 0 ), // Up
                45 );                    // FOV

            camera.NearPlaneDistance = .25;
            camera.FarPlaneDistance = 10;
            XamlTestHelper.LogStatus("Creating Camera object");
            return camera;
        }

 

        public Model3DGroup GetModelGroup()
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add( new Point3D( -1, -1, 0 ) );
            mesh.Positions.Add( new Point3D( -1, 1, 0 ) );
            mesh.Positions.Add( new Point3D( 1, -1, 0 ) );
            mesh.Positions.Add( new Point3D( 1, 1, 0 ) );
            mesh.TextureCoordinates.Add( new Point( 0, 1 ) );
            mesh.TextureCoordinates.Add( new Point( 0, 0 ) );
            mesh.TextureCoordinates.Add( new Point( 1, 1 ) );
            mesh.TextureCoordinates.Add( new Point( 1, 0 ) );
            mesh.TriangleIndices.Add( 2 );
            mesh.TriangleIndices.Add( 1 );
            mesh.TriangleIndices.Add( 0 );
            mesh.TriangleIndices.Add( 3 );
            mesh.TriangleIndices.Add( 1 );
            mesh.TriangleIndices.Add( 2 ); 

            GeometryModel3D model = new GeometryModel3D( mesh, new DiffuseMaterial( Brushes.Red ) );
            Light light = new AmbientLight( Colors.White );
 
            Model3DGroup parent = new Model3DGroup();
            parent.Children = new Model3DCollection( new Model3D[] { light, model } );
            XamlTestHelper.LogStatus("Creating Model3DGroup object");
            return parent;

        }

    }
}