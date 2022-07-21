// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace Tests
{
    public class SphereUIElement3D : UIElement3D
    {
        protected override void OnUpdateModel()
        {
            MeshGeometry3D myGeom = Sphere.Tessellate(10, 10, 0.5);
            _diffMat = new DiffuseMaterial();
            _diffMat.Brush = Brushes.White;

            GeometryModel3D gm3D = new GeometryModel3D(myGeom, _diffMat);

            Visual3DModel = gm3D;
        }

        private DiffuseMaterial _diffMat;
    }
}
