// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// 
//
// Description: Class that builds and contains a torus mesh.
//
//
//

using System;
using System.Windows;
using System.Windows.Media.Media3D;

class Torus
{
    private const double _defaultInnerRadius = 0.20;
    private const double _defaultOuterRadius = 0.05;
//     private const int _defaultSpineSegments = 25;
//     private const int _defaultFleshSegments = 15;
    // For testing purposes use small.
    private const int _defaultSpineSegments = 15;
    private const int _defaultFleshSegments = 15;
    
    /// <summary>
    /// Class that builds a torus shape.
    /// </summary>
    /// <param name="innerRadius">Radius of circle defining the skeleton of the torus</param>
    /// <param name="outerRadius">Radius of circle around the skeleton (<i>i.e.,</i> the profile)</param>
    public Torus(double innerRadius, double outerRadius)
    {
        _innerRadius = innerRadius;
        _outerRadius = outerRadius;
        _spineSegments = _defaultSpineSegments;
        _fleshSegments = _defaultFleshSegments;
    }

    public Torus() 
        : this( _defaultInnerRadius, _defaultOuterRadius )
    {
    }

    public MeshGeometry3D MeshGeometry3D
    {
        get
        {
            if(_mesh == null)
            {
                _mesh = new MeshGeometry3D();

                // The spine is the circle around the hole in the
                // torus, the flesh is a set of circles around the
                // spine.

                int cp = 0;	// Index of	last added point.

                for (int i = 0; i < _spineSegments; ++i)
                {
                    double spineParam = ((double)i) / ((double)_spineSegments);
                    double spineAngle = Math.PI * 2 * spineParam;
                    Vector3D spineVector = new Vector3D(Math.Cos(spineAngle), Math.Sin(spineAngle), 0);

                    for (int j = 0; j < _fleshSegments; ++j)
                    {
                        double fleshParam = ((double)j) / ((double)_fleshSegments);
                        double fleshAngle = Math.PI * 2 * fleshParam;
                        Vector3D fleshVector = spineVector * Math.Cos(fleshAngle) + new Vector3D(0, 0, Math.Sin(fleshAngle));
                        Point3D p = new Point3D(0, 0, 0) + _innerRadius * spineVector + _outerRadius * fleshVector;

                        _mesh.Positions.Add(p);
                        _mesh.Normals.Add(fleshVector);
                        _mesh.TextureCoordinates.Add(new Point(spineParam, fleshParam));

                        // Now add a quad that has it's	upper-right	corner at the point	we just	added.
                        // i.e.	cp . cp-1 . cp-1-_fleshSegments . cp-_fleshSegments
                        int a = cp - _fleshSegments;
                        int b = cp - (int) 1 - _fleshSegments;
                        int c = cp - 1;
                        int d = cp;

                        // The next two if statements handle the wrapping around of the torus.  For either i = 0 or j = 0
                        // the created quad references vertices that haven't been created yet.
                        if(j == 0)
                        {
                            b += _fleshSegments;
                            c += _fleshSegments;
                        }

                        if(i == 0)
                        {
                            a += _fleshSegments * _spineSegments;
                            b += _fleshSegments * _spineSegments;
                        }

                        _mesh.TriangleIndices.Add((ushort)a); _mesh.TriangleIndices.Add((ushort)b); _mesh.TriangleIndices.Add((ushort)c);
                        _mesh.TriangleIndices.Add((ushort)a); _mesh.TriangleIndices.Add((ushort)c); _mesh.TriangleIndices.Add((ushort)d);
                        ++cp;
                    }
                }
            }

            return _mesh;
        }
    }

    public double InnerRadius
    {
        get { return _innerRadius; }
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Torus radius must be non-negative");
            _innerRadius = value;
            _mesh = null;
        }
    }

    public double OuterRadius
    {
        get { return _outerRadius; }
        set {
            if (value < 0)
                throw new ArgumentOutOfRangeException("Torus radius must be non-negative");
            _outerRadius = value;
            _mesh = null;
        }
    }

    public int FleshSegments
    {
        get {
            return _fleshSegments;
        }
        set {
            // One segment won't produce a valid triangulation.
            if (value < 2)
                throw new ArgumentOutOfRangeException("Torus segments must be at least 2.");
            _fleshSegments = value;
            _mesh = null;
        }
    }

    public int SpineSegments
    {
        get {
            return _spineSegments;
        }
        set {
            // One segment won't produce a valid triangulation.
            if (value < 2)
                throw new ArgumentOutOfRangeException("Torus segments must be at least 2.");
            _spineSegments = value;
            _mesh = null;
        }
    }

    private MeshGeometry3D _mesh;
    private double _innerRadius, _outerRadius;
    private int _spineSegments,_fleshSegments;
}
