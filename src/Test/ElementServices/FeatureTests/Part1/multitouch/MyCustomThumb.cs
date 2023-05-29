// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// A custom thumb control inherited from UIElement3D
    /// </summary>
    public class MyCustomThumb : UIElement3D
    {
        /// <summary>
        /// add the model to the control when OnUpdateModel once to improve perf
        /// </summary>
        protected override void OnUpdateModel()
        {
            if (this.Visual3DModel == null)
            {
                this.Visual3DModel = Thumb3DModel(); 
            }
        }

        /// <summary>
        /// generate the GeometryModel3D for the control to use
        /// </summary>
        /// <returns></returns>
        protected GeometryModel3D Thumb3DModel()
        {     
            MeshGeometry3D geo3D = new MeshGeometry3D();

            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(-2, -1, 0));
            myPositionCollection.Add(new Point3D(-1, -0.25, 4));
            myPositionCollection.Add(new Point3D(-2, 1, 0));
            myPositionCollection.Add(new Point3D(-1, 0.25, 4));
            myPositionCollection.Add(new Point3D(2, -1, 0));
            myPositionCollection.Add(new Point3D(1, -0.25, 4));
            myPositionCollection.Add(new Point3D(2, 1, 0));
            myPositionCollection.Add(new Point3D(1, 0.25, 4));
            geo3D.Positions = myPositionCollection;

            PointCollection myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new Point(0, 1));
            myTextureCoordinatesCollection.Add(new Point(0.2, 0.6));
            myTextureCoordinatesCollection.Add(new Point(0, 0));
            myTextureCoordinatesCollection.Add(new Point(0.2, 0.4));
            myTextureCoordinatesCollection.Add(new Point(1, 1));
            myTextureCoordinatesCollection.Add(new Point(0.8, 0.6));
            myTextureCoordinatesCollection.Add(new Point(1, 0));
            myTextureCoordinatesCollection.Add(new Point(0.8, 0.4));
            geo3D.TextureCoordinates = myTextureCoordinatesCollection;

            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);  // 0 1 2,
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(1);  // 1 3 2, 
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(0); // 0 2 4, 
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(2);  // 2 6 4, 
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(0);  // 0 4 1, 
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(1);  // 1 4 5, 
            myTriangleIndicesCollection.Add(4);
            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(1);  // 1 5 7, 
            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(7);
            myTriangleIndicesCollection.Add(1); // 1 7 3, 
            myTriangleIndicesCollection.Add(7);
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(4);  //4 6 5, 
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(7);  // 7 5 6, 
            myTriangleIndicesCollection.Add(5);
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(2);  // 2 3 6,  
            myTriangleIndicesCollection.Add(3);
            myTriangleIndicesCollection.Add(6);
            myTriangleIndicesCollection.Add(3);  // 3 7 6 
            myTriangleIndicesCollection.Add(7);
            myTriangleIndicesCollection.Add(6);            
            geo3D.TriangleIndices = myTriangleIndicesCollection;

            Material material = new DiffuseMaterial(new SolidColorBrush(Colors.DarkKhaki));

            GeometryModel3D model3D = new GeometryModel3D();
            model3D.Geometry = geo3D;
            model3D.Material = material;

            return model3D; 
        }
    }
}
