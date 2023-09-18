// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public enum RayOrigin
    {
        /// <summary/>
        Visual2D,
        /// <summary/>
        Visual3D
    }

    /// <summary>
    /// Class to facilitate Ray hit testing
    /// </summary>
    public class Ray
    {
        /// <summary/>
        public Ray(Point3D origin, Vector3D direction, RayOrigin rayOrigin)
        {
            this._origin = origin;
            this._direction = MathEx.Normalize(direction);
            this._rayOrigin = rayOrigin;
        }

        /// <summary/>
        public bool DoesIntersect(Rect3D bounds)
        {
            // Trivial reject
            if (bounds.IsEmpty)
            {
                return false;
            }
            // Trivial accept
            if (bounds.Contains(_origin))
            {
                return true;
            }

            // Implementation of an algorithm in Real-Time Rendering p.574
            double tmin = double.NegativeInfinity;
            double tmax = double.PositiveInfinity;
            Point3D center = new Point3D(bounds.X + bounds.SizeX / 2.0, bounds.Y + bounds.SizeY / 2.0, bounds.Z + bounds.SizeZ);
            Vector3D p = center - _origin;
            double[] e = new double[] { p.X, p.Y, p.Z };
            double[] f = new double[] { _direction.X, _direction.Y, _direction.Z };
            double[] h = new double[] { bounds.SizeX / 2.0, bounds.SizeY / 2.0, bounds.SizeZ / 2.0 };
            for (int i = 0; i < 3; i++)
            {
                if (Math.Abs(f[i]) > Const.eps)
                {
                    double t1 = (e[i] + h[i]) / f[i];
                    double t2 = (e[i] - h[i]) / f[i];
                    if (t1 > t2)
                    {
                        double temp = t1;
                        t1 = t2;
                        t2 = temp;
                    }
                    if (t1 > tmin)
                    {
                        tmin = t1;
                    }
                    if (t2 < tmax)
                    {
                        tmax = t2;
                    }
                    if (tmin > tmax || tmax < 0)
                    {
                        return false;
                    }
                }
                else if (-e[i] - h[i] > 0 || -e[i] + h[i] < 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determine if the ray intersects the triangle defined by the input points.
        /// Store the results of the test in "hitInfo."
        /// Note that hitInfo.intersection will not be set because it is assumed that the triangle
        ///  being tested is in world space, not model space.
        /// </summary>
        public bool DoesIntersect(Point3D p1, Point3D p2, Point3D p3, VisibleFaces visibleFaces, RayMeshHitInfo hitInfo)
        {
            Vector3D edge1 = p2 - p1;
            Vector3D edge2 = p3 - p1;
            Vector3D normal = MathEx.CrossProduct(_direction, edge2);
            double det = MathEx.DotProduct(edge1, normal);

            if (!CanHit(det, visibleFaces))
            {
                return false;
            }

            double detInv = 1 / det;
            Vector3D tVector = _origin - p1;
            double u = MathEx.DotProduct(tVector, normal);
            u *= detInv;
            if (u < 0.0 || u > 1.0)
            {
                return false;
            }
            Vector3D qVector = MathEx.CrossProduct(tVector, edge1);
            double v = MathEx.DotProduct(_direction, qVector);
            v *= detInv;
            if (v < 0.0 || u + v > 1.0)
            {
                return false;
            }
            double t = MathEx.DotProduct(edge2, qVector);
            t *= detInv;

            if (t < 0)
            {
                // We cannot hit things behind us
                return false;
            }

            hitInfo.distanceToRayOrigin = t;
            hitInfo.vertexWeight1 = 1 - u - v;
            hitInfo.vertexWeight2 = u;
            hitInfo.vertexWeight3 = v;

            return true;
        }

        private bool CanHit(double determinant, VisibleFaces visibleFaces)
        {
            if (-MathEx.Epsilon < determinant && determinant < MathEx.Epsilon)
            {
                // The ray is parallel to the triangle face
                return false;
            }

            switch (visibleFaces)
            {
                case VisibleFaces.None: return false;
                case VisibleFaces.Front: return (0 < determinant);
                case VisibleFaces.Back: return (determinant < 0);
                case VisibleFaces.Both: return true;
            }

            throw new NotSupportedException("Unrecognized VisibleFaces value used for hit testing: " + visibleFaces);
        }

        /// <summary/>
        public Point3D Origin { get { return _origin; } }
        /// <summary/>
        public Vector3D Direction { get { return _direction; } }
        /// <summary/>
        public RayOrigin RayOrigin { get { return _rayOrigin; } }

        private Point3D _origin;
        private Vector3D _direction;
        private RayOrigin _rayOrigin;
    }
}