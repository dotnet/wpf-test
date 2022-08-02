// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Static class that holds on to a variety of slightly
//              strange meshes that don't have the exact right number
//              of texture coordinates or have bad indices or other
//              weirdness that definitely shouldn't cause crashes.
//

using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

static class WeirdMeshes
{
    static WeirdMeshes()
    {
        Torus torus = new Torus();
        torus.SpineSegments = 3;
        torus.FleshSegments = 3;
        _mesh = torus.MeshGeometry3D;
        _meshes = new List<MeshGeometry3D>();

        // A couple extra, valid triangle indices.
        MeshGeometry3D m = _mesh.Clone();
        m.TriangleIndices.Add(0);
        m.TriangleIndices.Add(1);
        _meshes.Add(m);

        // Three extra invalid triangle indices.
        m = _mesh.Clone();
        m.TriangleIndices.Add(2342340);
        m.TriangleIndices.Add(23423440);
        m.TriangleIndices.Add(23423340);
        _meshes.Add(m);

        // Extra texture coordinates
        m = _mesh.Clone();
        m.TextureCoordinates.Add(new Point(1,2));
        m.TextureCoordinates.Add(new Point(1,2));
        m.TextureCoordinates.Add(new Point(1,2));
        m.TextureCoordinates.Add(new Point(1,2));
        _meshes.Add(m);

        // No texture coordinates
        m = _mesh.Clone();
        m.TextureCoordinates.Clear();
        _meshes.Add(m);

        // Too few texture coordinates
        m = _mesh.Clone();
        m.TextureCoordinates.Clear();
        m.TextureCoordinates.Add(new Point(1,2));
        m.TextureCoordinates.Add(new Point(1,2));
        _meshes.Add(m);

        // Extra normals
        m = _mesh.Clone();
        m.Normals.Add(new Vector3D(1,2,3));
        m.Normals.Add(new Vector3D(1,2,3));
        m.Normals.Add(new Vector3D(1,2,3));
        m.Normals.Add(new Vector3D(1,2,3));
        _meshes.Add(m);

        // No normals
        m = _mesh.Clone();
        m.Normals.Clear();
        _meshes.Add(m);

        // Too few normals
        m = _mesh.Clone();
        m.Normals.Clear();
        m.Normals.Add(new Vector3D(1,1,1));
        m.Normals.Add(new Vector3D(1,2,3));
        _meshes.Add(m);

        // Only invalid triangle indices.
        m = _mesh.Clone();
        m.TriangleIndices.Clear();
        m.TriangleIndices.Add(2342340);
        m.TriangleIndices.Add(23423440);
        m.TriangleIndices.Add(23423340);
        m.TriangleIndices.Add(2342340);
        m.TriangleIndices.Add(23423440);
        m.TriangleIndices.Add(23423340);
        m.TriangleIndices.Add(2342340);
        m.TriangleIndices.Add(23423440);
        m.TriangleIndices.Add(23423340);
        _meshes.Add(m);

        // An extra triangle with one negative index.
        m = _mesh.Clone();
        m.TriangleIndices.Add(0);
        m.TriangleIndices.Add(-2);
        m.TriangleIndices.Add(1);
        _meshes.Add(m);

        // Six negative indices
        m = _mesh.Clone();
        m.TriangleIndices.Clear();
        m.TriangleIndices.Add(-1);
        m.TriangleIndices.Add(-2);
        m.TriangleIndices.Add(-3);
        m.TriangleIndices.Add(-4);
        m.TriangleIndices.Add(-5);
        m.TriangleIndices.Add(-6);
        _meshes.Add(m);
    }

    static public List<MeshGeometry3D> Meshes
    {
        get
        {
            return _meshes;
        }
    }

    static private MeshGeometry3D _mesh;
    static private List<MeshGeometry3D> _meshes;
}
