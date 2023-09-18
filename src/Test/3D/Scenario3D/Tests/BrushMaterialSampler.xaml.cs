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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;
using Microsoft.Test.Graphics.ReferenceRender;

// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    public partial class DiffuseMaterialSampler
    {
        public void OnLoaded(object sender, EventArgs args)
        {
            RenderTolerance.TextureLookUpTolerance = .5;
            BuildScene();
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D(0, -1, 0);

            MeshGeometry3D mesh1 = MeshFactory.SmoothEdgeCube;

            Material mat1 = new DiffuseMaterial(BrushFactory.BrushSolidColor);
            GeometryModel3D primitive1 = new GeometryModel3D(mesh1, mat1);
            primitive1.Transform = new TranslateTransform3D(new Vector3D(-3.5, 0, 0));

            Material mat2 = new DiffuseMaterial(BrushFactory.BrushImage);
            GeometryModel3D primitive2 = new GeometryModel3D(mesh1, mat2);
            primitive2.Transform = new TranslateTransform3D(new Vector3D(-3.5, 1.5, -3.5));

            Material mat3 = new DiffuseMaterial(BrushFactory.MakeBrush("moo.bmp"));
            GeometryModel3D primitive3 = new GeometryModel3D(mesh1, mat3);
            primitive3.Transform = new TranslateTransform3D(new Vector3D(0, 0, -3.5));

            Material mat4 = new DiffuseMaterial(BrushFactory.BrushLinearGradient);
            GeometryModel3D primitive4 = new GeometryModel3D(mesh1, mat4);
            primitive4.Transform = new TranslateTransform3D(new Vector3D(0, 0, 0));

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add(light);
            mg.Children.Add(primitive1);
            mg.Children.Add(primitive2);
            mg.Children.Add(primitive3);
            mg.Children.Add(primitive4);
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add(visual);
        }
    }
}
