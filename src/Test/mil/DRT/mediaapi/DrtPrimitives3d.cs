// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a set of DRTs designed to test 3D 
//              primitives.
//
//

using System;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;
using MS.Internal;

/// <summary>
/// Test for 3D Primitive opertations
/// </summary>
public class DrtPrimitives3D : DrtPrimitiveBase
{
    /// <summary>
    /// Run DRT.
    /// </summary>
    public override bool Run(out string results)
    {
        bool succeeded = true;
        
        string vector, point, quaternion, matrix, rect, empty;

        succeeded = TestVector3D(out vector) && succeeded;
        succeeded = TestPoint3D(out point) && succeeded;
        succeeded = TestQuaternion(out quaternion) && succeeded;
        succeeded = TestMatrix3D(out matrix) && succeeded;
        succeeded = TestRect3D(out rect) && succeeded;
        succeeded = TestEmptyStrings(out empty) && succeeded;

        // If failure occurred, then we care about the output
        if (!succeeded)
        {
            results = vector + point + quaternion + matrix + rect;
        }
        else
        {
            results = "SUCCEEDED";
        }

        return succeeded;
    }

    private bool TestPoint3D(out string results)
    {
        bool succeed = true;
        int test = 0;
        // Note that epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        // Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        // be close enough
        double ep = 2.2204460492503131e-017;

        results = "";

        Point3D pt1 = new Point3D(12.3, -19.332, 4.23);

        test++; if (!(pt1.X == 12.3)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!(pt1.Y == -19.332)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!(pt1.Z == 4.23)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!AreClose(pt1, new Point3D(12.3 - ep, -19.332 + ep, 4.23 + ep))) { succeed = false; results += "Point Test " + test + " FAILED\n";; }

        Point3D pt2 = pt1;

        test++; if (!AreClose(pt1, pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!(pt1.GetHashCode() == pt2.GetHashCode())) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!(pt1 == pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (pt1 != pt2) { succeed = false; results += "Point Test " + test + " FAILED\n";; }

        Vector3D vec = pt2 - new Point3D(0,0,0);
        test++; if (!AreClose(pt2, (Point3D)vec)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        pt1.X = 1.0;
        pt1.Y = 2.0;
        pt1.Z = 3.0;
        vec.X = 3.0;
        vec.Y = 6.0;
        vec.Z = 9.0;
        test++; if (!AreClose(pt2 + ((pt1 - new Point3D(0,0,0)) * 3.0), pt2 + vec)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (pt1.Equals(null)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (pt1.Equals(1.0)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (pt1.Equals(pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }
        test++; if (!pt1.Equals(pt1)) { succeed = false; results += "Point Test " + test + " FAILED\n";; }

        return succeed;
    }


    
    private bool TestVector3D(out string results)
    {
        bool succeed = true;
        int test = 0;
        double ep = 2.2204460492503131e-017;

        results = "";

        Vector3D vec1 = new Vector3D(12.3, -19.332, 23.123);

        test++; if (!(vec1.X == 12.3)) { succeed = false; results += "Vector3D Test " + test + " FAILED\n"; }
        test++; if (!(vec1.Y == -19.332)) { succeed = false; results += "Vector3D Test " + test + " FAILED\n"; }
        test++; if (!(vec1.Z == 23.123)) { succeed = false; results += "Vector3D Test " + test + " FAILED\n"; }
        test++; if (!AreClose(vec1, new Vector3D(12.3 - ep, -19.332 + ep, 23.123 + ep))) { succeed = false; results += "Vector Test " + test + " FAILED\n";; }

        Vector3D vec2 = vec1;

        test++; if (!AreClose(vec1, vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!(vec1.GetHashCode() == vec2.GetHashCode())) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!AreClose(vec1 * 2, vec2 + vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!(vec1 == vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1 != vec2) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        
        vec1.X = 24.6;
        vec1.Y = -38.664;
        vec1.Z = 46.246;
        test++; if (!AreClose(vec1, (vec2 * 2))) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!AreClose(vec1 * 4.5, vec2 * 10 - vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!AreClose((vec1 / 3.0), vec2 / 2.0 + (vec2 / 6.0))) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(null)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(1.0)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!vec1.Equals(vec1)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }

        Vector3D vec3 = new Vector3D(1.0, 2.0, 3.0);
        Vector3D vec4 = new Vector3D(5.0, 6.0, 7.0);

        test++; if (Vector3D.DotProduct(vec3,vec4) != (vec3.X*vec4.X+vec3.Y*vec4.Y+vec3.Z*vec4.Z)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (Vector3D.CrossProduct(vec3, vec4).Length != Vector3D.CrossProduct(vec4, vec3).Length) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }

        vec3.Normalize();

        test++; if (!DoubleUtil.AreClose(vec3.Length, 1)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(vec3.LengthSquared, 1)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }

        return succeed;
    }

    /// <summary>
    /// Old implementation of slerp that only supported normalized
    /// quaternions but extended to normalize the inputs.  Used to test our new implementation
    /// that supports non-normalized quaternions.
    /// </summary>
    /// <param name="left">First quaternion for interpolation.</param>
    /// <param name="right">Second quaternion for interpolation.</param>
    /// <param name="t">Interpolation coefficient.</param>
    /// <returns>SLERP-interpolated quaternion between the two given quaternions.</returns>
    private static Quaternion OldSlerp(Quaternion left, Quaternion right, double t)
    {
        const double delta = 1e-6;
        double cosOmega;
        double scaleLeft, scaleRight;

        left.Normalize();
        right.Normalize();
        
        // Calculate cos of omega, the angle between the two
        // quaternions viewed as vectors in 4-space.  This is just
        // their dot product.
        cosOmega = left.X*right.X + left.Y*right.Y + left.Z*right.Z + left.W*right.W;

        // Adjust signs if necessary.
        if (cosOmega < 0.0)
        {
            cosOmega = -cosOmega;
            right = new Quaternion(-right.X,-right.Y,-right.Z,-right.W);
        }

        // Calculate scaling coefficients.
        if ((1.0 - cosOmega) > delta) 
        {
            // Standard case - use SLERP interpolation.
            // Clip cosOmega to [-1,1] to avoid an exception from Acos below.
            // Note that, because of the "adjust sign" test above, cosOmega is non-negative
            if (cosOmega > 1.0)
            {
                cosOmega = 1.0;
            }
            double omega = Math.Acos(cosOmega);
            double sinOmega = Math.Sqrt(1.0 - cosOmega*cosOmega);
            scaleLeft = Math.Sin((1.0 - t) * omega) / sinOmega;
            scaleRight = Math.Sin(t * omega) / sinOmega;
        } 
        else 
        {        
            // Quaternions are too close - use linear interpolation.
            scaleLeft = 1.0 - t;
            scaleRight = t;
        }

        return new Quaternion(scaleLeft*left.X + scaleRight*right.X,
                              scaleLeft*left.Y + scaleRight*right.Y,
                              scaleLeft*left.Z + scaleRight*right.Z,
                              scaleLeft*left.W + scaleRight*right.W);
    }

    private static void ScaleQuaternion(ref Quaternion q, double s)
    {
        q = new Quaternion(q.X * s, q.Y * s, q.Z * s, q.W * s);
    }
    
    private bool TestQuaternion(out string results)
    {
        bool succeed = true;
        int test = 0;
  
        results = "";
        double a = Math.Sqrt(0.25);
        Quaternion q1 = new Quaternion(a,a,a,a);

        test++; if (!AreClose(q1.X, a)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (!AreClose(q1.Y, a)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (!AreClose(q1.Z, a)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (!AreClose(q1.W, a)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        Quaternion q2 = q1;

        test++; if (!AreClose(q1, q2)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (!(q1.GetHashCode() == q2.GetHashCode())) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (!(q1 == q2)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if (q1 != q2) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        Quaternion qq1 = new Quaternion(1.0, 2.0, 3.0, 4.345);
        Quaternion qq2 = new Quaternion(3.0, 88.0, 6.45, 34.5);
        Quaternion plus = qq1 + qq2;
        Quaternion minus = qq1 - qq2;

        test++; if (plus != (qq2 + qq1)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if ((qq1-qq1) != new Quaternion(0,0,0,0)) { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        // Non-normalized tests
        Vector3D axis1 = new Vector3D(1,2,3);
        Vector3D axis2 = new Vector3D(3400,4500,300);
        double angle1 = 34;
        double angle2 = 45;

        Quaternion quat1 = new Quaternion(axis1,angle1);
        Quaternion quat2 = new Quaternion(axis2,angle2);

        test++; if(!AreEquivalent(axis1,angle1,quat1.Axis,quat1.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(axis2,angle2,quat2.Axis,quat2.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        ScaleQuaternion( ref quat1, 34 );
        ScaleQuaternion( ref quat2, 0.00034 );
        
        test++; if(!AreEquivalent(axis1,angle1,quat1.Axis,quat1.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(axis2,angle2,quat2.Axis,quat2.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        ScaleQuaternion( ref quat1, 133400034324 );
        ScaleQuaternion( ref quat2, 23e-5 );
        
        test++; if(!AreEquivalent(axis1,angle1,quat1.Axis,quat1.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(axis2,angle2,quat2.Axis,quat2.Angle))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        Quaternion slerp1a = Quaternion.Slerp(quat1,quat2,0.3);
        Quaternion slerp2a = Quaternion.Slerp(quat1,quat2,0.9);
        Quaternion slerp3a = Quaternion.Slerp(quat1,quat2,0);
        Quaternion slerp4a = Quaternion.Slerp(quat1,quat2,1);

        Quaternion slerp1b = OldSlerp(quat1,quat2,0.3);
        Quaternion slerp2b = OldSlerp(quat1,quat2,0.9);
        Quaternion slerp3b = OldSlerp(quat1,quat2,0);
        Quaternion slerp4b = OldSlerp(quat1,quat2,1);

        test++; if(!AreEquivalent(slerp1a,slerp1b))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(slerp2a,slerp2b))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(slerp3a,slerp3b))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(slerp4a,slerp4b))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        Quaternion bigboy = new Quaternion(Double.MaxValue/1000,
                                           Double.MaxValue/1000,
                                           Double.MaxValue/1000,
                                           Double.MaxValue/1000);

        test++; if(!AreEquivalent(bigboy,new Quaternion(1,1,1,1)))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }


        // Tests of addition & distinguished identity.
        Quaternion distinguished = new Quaternion();
        Quaternion distinguished2 = Quaternion.Identity;
        Quaternion identity = new Quaternion(0,0,0,1); // Not distinguished
        Quaternion other = new Quaternion(1,2,3,4);

        test++; if(!AreEquivalent(distinguished+identity,identity+identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished2+identity,identity+identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished+distinguished2,identity+identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished+other,identity+other))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished2+other,identity+other))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        test++; if(!AreEquivalent(distinguished-identity,identity-identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished2-identity,identity-identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished-distinguished2,identity-identity))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished-other,identity-other))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }
        test++; if(!AreEquivalent(distinguished2-other,identity-other))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        // Test of normalizing huge quaternion.  
        Quaternion small = new Quaternion(1,1,1,1);
        Quaternion big = new Quaternion(Double.MaxValue,Double.MaxValue,Double.MaxValue,Double.MaxValue);
        small.Normalize();
        big.Normalize();
        test++; if(!AreEquivalent(big,small))  { succeed = false; results += "Quaternion Test " + test + " FAILED\n"; }

        return succeed;
    }


    private bool TestMatrix3D(out string results)
    {
        bool succeed = true;
        int test = 0;
  
        results = "";

        Matrix3D m1 = new Matrix3D();

        // Identity
        test++; if (!m1.IsIdentity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; }
        m1.M22 = 5.0;
        test++; if (m1.IsIdentity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; }
        m1.SetIdentity();
        test++; if (!m1.IsIdentity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; }
        test++; if (m1 != Matrix3D.Identity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; }

        // Prepend, append
        m1.SetIdentity();
        Matrix3D m2 = new Matrix3D(1,2,3,4,1,2,3,4,1,2,3,4,1,2,3,4);
        m1.Append(m2);
        test++; if (!AreClose(m1, new Matrix3D(1,2,3,4,1,2,3,4,1,2,3,4,1,2,3,4))) 
                { 
                    succeed = false; 
                    results += m1.M11 + " " + m1.M12 + " " + m1.M13 + " " + m1.M14 + " \n";
                    results += m1.M21 + " " + m1.M22 + " " + m1.M23 + " " + m1.M24 + " \n";
                    results += m1.M31 + " " + m1.M32 + " " + m1.M33 + " " + m1.M34 + " \n";
                    results += m1.OffsetX + " " + m1.OffsetY + " " + m1.OffsetZ + " " + m1.M44 + " \n";
                    results += "Matrix3D Test " + test + " FAILED\n"; 
                }
        m1.SetIdentity();
        m2.Prepend(m1);
        test++; if (!AreClose(m2, new Matrix3D(1,2,3,4,1,2,3,4,1,2,3,4,1,2,3,4))) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; }

        // Rotate
        m1.SetIdentity();
        m1.Rotate(new Quaternion(new Vector3D(1,2,3), 45));
        m1.Rotate(new Quaternion(new Vector3D(1,2,3), -45));
        test++; if (!AreClose(m1, Matrix3D.Identity)) { succeed = false; results += "Matrix3D Test " + test + " (rotation) FAILED\n"; }
        m1.SetIdentity();
        m1.RotatePrepend(new Quaternion(new Vector3D(1,2,3), -45));
        m1.RotatePrepend(new Quaternion(new Vector3D(1,2,3), 45));
        test++; if (!AreClose(m1, Matrix3D.Identity)) { succeed = false; results += "Matrix3D Test " + test + " (rotation) FAILED\n"; }

        m1.SetIdentity();
        m1.RotateAt(new Quaternion(new Vector3D(1,2,3), 45), new Point3D(0,0,0));
        m1.RotateAt(new Quaternion(new Vector3D(1,2,3), -45), new Point3D(0,0,0));
        test++; if (!AreClose(m1, Matrix3D.Identity)) { succeed = false; results += "Matrix3D Test " + test + " (rotation) FAILED\n"; }
        m1.SetIdentity();
        m1.RotateAtPrepend(new Quaternion(new Vector3D(1,2,3), -45), new Point3D(0,0,0));
        m1.RotateAtPrepend(new Quaternion(new Vector3D(1,2,3), 45), new Point3D(0,0,0));
        test++; if (!AreClose(m1, Matrix3D.Identity)) { succeed = false; results += "Matrix3D Test " + test + " (rotation) FAILED\n"; }


        // Scale
        m1.SetIdentity();
        m1.Scale(new Vector3D(1,2,3));
        test++; if (m1.M11 != 1 || m1.M22 != 2 || m1.M33 != 3 || m1.M44 != 1) { succeed = false; results += "Matrix3D Test " + test + " (scale) FAILED\n"; }
        m1.SetIdentity();
        m1.ScalePrepend(new Vector3D(1,2,3));
        test++; if (m1.M11 != 1 || m1.M22 != 2 || m1.M33 != 3 || m1.M44 != 1) { succeed = false; results += "Matrix3D Test " + test + " (scale) FAILED\n"; }

        m1.SetIdentity();
        m1.ScaleAt(new Vector3D(1,2,3), new Point3D(0,0,0));
        test++; if (m1.M11 != 1 || m1.M22 != 2 || m1.M33 != 3 || m1.M44 != 1) { succeed = false; results += "Matrix3D Test " + test + " (scale) FAILED\n"; }
        m1.SetIdentity();
        m1.ScaleAtPrepend(new Vector3D(1,2,3), new Point3D(0,0,0));
        test++; if (m1.M11 != 1 || m1.M22 != 2 || m1.M33 != 3 || m1.M44 != 1) { succeed = false; results += "Matrix3D Test " + test + " (scale) FAILED\n"; }

        // Translate
        m1.SetIdentity();
        m1.Translate(new Vector3D(1,2,3));
        test++; if (m1.OffsetX != 1 || m1.OffsetY != 2 || m1.OffsetZ != 3) { succeed = false; results += "Matrix3D Test " + test + " (translate) FAILED\n"; }
        m1.SetIdentity();
        m1.TranslatePrepend(new Vector3D(1,2,3));
        test++; if (m1.OffsetX != 1 || m1.OffsetY != 2 || m1.OffsetZ != 3) { succeed = false; results += "Matrix3D Test " + test + " (translate) FAILED\n"; }

        // Multiplication
        m1.SetIdentity();
        m2.SetIdentity();
        test++; if (m1 * m1 != Matrix3D.Identity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; } 
        test++; if (m1 * m2 != Matrix3D.Identity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; } 
        test++; if (m2 * m1 != Matrix3D.Identity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; } 
        test++; if (m2 * m2 != Matrix3D.Identity) { succeed = false; results += "Matrix3D Test " + test + " FAILED\n"; } 

        // Inverse
        Matrix3D m3 = new Matrix3D(1,2,3,4,1,2,3,4,1,2,3,4,1,2,3,4);
        test++; if (m3.IsAffine) { succeed = false; results += "Matrix3D Test " + test + " (inverse) FAILED\n"; } 
        m1.SetIdentity();
        Matrix3D m1Inverse = m1;
        m1Inverse.Invert();
        test++; if (!m1.IsAffine) { succeed = false; results += "Matrix3D Test " + test + " (inverse) FAILED\n"; } 
        test++; if (!DoubleUtil.AreClose(m1.Determinant,1)) { succeed = false; results += "Matrix3D Test " + test + " (inverse) FAILED\n"; } 
        test++; if (!m1.HasInverse) { succeed = false; results += "Matrix3D Test " + test + " (inverse) FAILED\n"; } 
        test++; if (m1Inverse != m1) { succeed = false; results += "Matrix3D Test " + test + " (inverse) FAILED\n"; } 

        // Operations
        Matrix3D m4 = new Matrix3D();
        test++; if (m4.M11 != 1 || m4.M12 != 0 || m4.M13 != 0 || m4.M14 != 0) { succeed = false; results += "Matrix3D Test " + test + " (init) FAILED\n"; } 
        test++; if (m4.M21 != 0 || m4.M22 != 1 || m4.M23 != 0 || m4.M24 != 0) { succeed = false; results += "Matrix3D Test " + test + " (init) FAILED\n"; } 
        test++; if (m4.M31 != 0 || m4.M32 != 0 || m4.M33 != 1 || m4.M34 != 0) { succeed = false; results += "Matrix3D Test " + test + " (init)FAILED\n"; } 
        test++; if (m4.OffsetX != 0 || m4.OffsetY != 0 || m4.OffsetZ != 0 || m4.M44 != 1) { succeed = false; results += "Matrix3D Test " + test + " (init) FAILED\n"; } 

        // Transform services
        m1.SetIdentity();
        Point3D p = new Point3D(5,6,7);
        m1.Translate(new Vector3D(1,2,3));
        test++; if (!AreClose(m1.Transform(p), new Point3D(6,8,10))) { succeed = false; results += "Matrix3D Test " + test + " (transform) FAILED\n"; } 
        test++; if (!AreClose(p, new Point3D(5,6,7))) { succeed = false; results += "Matrix3D Test " + test + " (transform) FAILED\n"; } 
        // Vectors should not be transformed
        test++; if (!AreClose(m1.Transform(new Vector3D(1,2,3)), new Vector3D(1,2,3))) { succeed = false; results += "Matrix3D Test " + test + " (transform) FAILED\n"; }


        // Test determinant and inverse optimizations for affine matrices.
        Vector3D vector = new Vector3D( 1,2,3 );
        List<Matrix3D> affines = new List<Matrix3D>();
        affines.Add(new Matrix3D(1,2,3,0,
                                  44,5,6,0,
                                  7,8,9,0,
                                  10,11,12,1));
        affines.Add(new Matrix3D(2341,3422,12343,0,
                                  344,435,34.346,0,
                                  74.34,0.348,0.349,0,
                                  10.43,11.434,12.434,1));
        affines.Add(m1);
        foreach (Matrix3D mm in affines)
        {
            // Note that det(km) = k^4 det(m) and
            //           inv(km) = 1/k inv(m)
            Matrix3D m = mm;
            test++; if (!m.IsAffine) { succeed = false; results += "Matrix3D Test " + test + " (affineness1) FAILED\n"; }
            double d1 = m.Determinant;
            m.M11 *= 2; m.M12 *= 2; m.M13 *= 2; m.M14 *= 2;
            m.M21 *= 2; m.M22 *= 2; m.M23 *= 2; m.M24 *= 2;
            m.M31 *= 2; m.M32 *= 2; m.M33 *= 2; m.M34 *= 2;
            m.OffsetX *= 2; m.OffsetY *= 2; m.OffsetZ *= 2; m.M44 *= 2;
            test++; if (m.IsAffine) { succeed = false; results += "Matrix3D Test " + test + " (affineness2) FAILED\n"; }
            double d2 = m.Determinant/16;
            test++; if (!DoubleUtil.AreClose(d1,d2)) { succeed = false; results += "Matrix3D Test " + test + " (affineness3) FAILED "+ d1 + " != " + d2 + "\n"; }

            m.Invert();         // Non-affine inversion of 2 * mm
            m.M11 *= 2; m.M12 *= 2; m.M13 *= 2; m.M14 *= 2;
            m.M21 *= 2; m.M22 *= 2; m.M23 *= 2; m.M24 *= 2;
            m.M31 *= 2; m.M32 *= 2; m.M33 *= 2; m.M34 *= 2;
            m.OffsetX *= 2; m.OffsetY *= 2; m.OffsetZ *= 2; m.M44 *= 2;
            // Now m should be equal to mm^-1 because we first multiplied by 2, inverted then multiplied by 2 again.
            Matrix3D mmm = mm;
            mmm.Invert();
            test++; if (!AreClose(m, mmm)) { succeed = false; results += "Matrix3D Test " + test + " (affineness6) FAILED\n"; }
        }

        return succeed;
    }

    private bool TestRect3D(out string results)
    {
        // Note that epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        // Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        // be close enough
        double ep = 2.2204460492503131e-017;
        
        bool succeed = true;
        int test = 0;

        results = "";

        Rect3D rect1 = new Rect3D(12.3, -19.332, -12.3, 19.332, 12.3, 19.332);

        test++; if (!(rect1.X == 12.3)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Y == -19.332)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Z == -12.3)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.SizeX == 19.332)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.SizeY == 12.3)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.SizeZ == 19.332)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }

        Rect3D rect2 = rect1;

        test++; if (!AreClose(rect1, rect2)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1.GetHashCode() == rect2.GetHashCode())) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!(rect1 == rect2)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (rect1 != rect2) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        
        rect2 = new Rect3D(27.0, 0, -33, 33, 50, 65);
        rect2.Union(rect1);
        test++; if (!AreClose(new Rect3D(12.3, -19.332, -33, 47.7, 69.332, 65),rect2)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }

        Rect3D rect3 = new Rect3D( 1,-2,-2,3,3,3 );
        Rect3D rect4 = new Rect3D( -5.1,-1,-1,3,3,3 );
        Rect3D rect5 = new Rect3D( 0,0,0,5,5,5 );
        Rect3D rect6 = new Rect3D( -5,-5,-5,5,5,5 );
        Rect3D rect7 = new Rect3D( -10,-10,-10,20,20,20 );
        Rect3D rect8 = new Rect3D( 0,0,0,1,1,1 );

        Rect3D ans1 = Rect3D.Intersect( rect5, rect6 );      // Should be 0,0,0,0,0,0
        Rect3D ans2 = Rect3D.Intersect( rect7, rect8 );      // Should be 0,0,0,1,1,1

        Rect3D offset1 = rect3;
        offset1.Offset( new Vector3D(1.1,2.2,3.3) );

        test++; if (!rect7.IntersectsWith(rect8)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!rect8.IntersectsWith(rect7)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (rect3.IntersectsWith(rect4)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!AreClose(ans1, new Rect3D(0,0,0,0,0,0))) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!AreClose(ans2, new Rect3D(0,0,0,1,1,1))) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }

        test++; if (!ans2.Contains(0,0,0)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!ans2.Contains(1-ep,1-ep,1-ep)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (!rect7.Contains(rect6)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }
        test++; if (rect6.Contains(rect5)) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }

        test++; if (!AreClose(offset1, new Rect3D(2.1,0.2,1.3,3,3,3))) { succeed = false; results += "Rect3D Test " + test + " FAILED\n"; }

        return succeed;
    }

    private bool TestEmptyStrings(out string results)
    {
        bool success = true;
        results = String.Empty;
        success &= EmptyTestHelper(typeof(Rect3D), "Empty", ref results);
        success &= EmptyTestHelper(typeof(Size3D), "Empty", ref results);
        success &= EmptyTestHelper(typeof(Matrix3D), "Identity", ref results);
        success &= EmptyTestHelper(typeof(Quaternion), "Identity", ref results);
        return success;
    }    

    private static bool AreClose(double d1, double d2)
    {
        return ((d1-d2) < 0.001) && ((d2-d1) < 0.001);
    }

    public static bool AreClose(Point3D point1, Point3D point2)
    {
        return DoubleUtil.AreClose(point1.X, point2.X) && 
               DoubleUtil.AreClose(point1.Y, point2.Y) &&
               DoubleUtil.AreClose(point1.Z, point2.Z);
    }

    public static bool AreClose(Vector3D vector1, Vector3D vector2)
    {
        return DoubleUtil.AreClose(vector1.X, vector2.X) && 
               DoubleUtil.AreClose(vector1.Y, vector2.Y) &&
               DoubleUtil.AreClose(vector1.Z, vector2.Z);
    }

    /// <summary>Returns the equivalent angle in the range [0,360)</summary>
    private static double SimpleAngle(double angle)
    {
        angle = Math.IEEERemainder(angle,360);
        // Angle could be between -360 & 360
        if (angle < 0)
            angle += 360;
        Debug.Assert(angle>=0 && angle<360);
        return angle;
    }
    
    /// <summary>Are the axis/angle pairs close or close to opposite
    /// (and thus equivalent) after normalizing the axis.
    public static bool AreEquivalent(Vector3D axis1, double angle1, Vector3D axis2, double angle2)
    {
        axis1.Normalize();
        axis2.Normalize();

        if ( AreClose(axis1,axis2) )
        {
            double error = Math.Abs(SimpleAngle(angle1)-SimpleAngle(angle2));
            return error < 0.001;
        }
        if ( AreClose(axis1,-axis2) )
        {
            double error = Math.Abs(SimpleAngle(angle1)-SimpleAngle(-angle2));
            return error < 0.001;
        }
        return false;
    }

    /// <summary>Are the quaternions equivalent</summary>
    public static bool AreEquivalent(Quaternion q1, Quaternion q2)
    {
        return AreEquivalent(q1.Axis,q1.Angle,q2.Axis,q2.Angle);
    }

    public static bool AreClose(Rect3D rect1, Rect3D rect2)
    {
        return DoubleUtil.AreClose(rect1.X, rect2.X) && 
               DoubleUtil.AreClose(rect1.Y, rect2.Y) &&
               DoubleUtil.AreClose(rect1.Z, rect2.Z) &&
               DoubleUtil.AreClose(rect1.SizeX, rect2.SizeX) && 
               DoubleUtil.AreClose(rect1.SizeY, rect2.SizeY) &&
               DoubleUtil.AreClose(rect1.SizeZ, rect2.SizeZ);
    }

    public static bool AreClose(Quaternion quaternion1, Quaternion quaternion2)
    {
        return DoubleUtil.AreClose(quaternion1.X, quaternion2.X) && 
               DoubleUtil.AreClose(quaternion1.Y, quaternion2.Y) &&
               DoubleUtil.AreClose(quaternion1.Z, quaternion2.Z) &&
               DoubleUtil.AreClose(quaternion1.W, quaternion2.W);
    }

    public static bool AreClose(Matrix3D matrix1, Matrix3D matrix2)
    {
        return DoubleUtil.AreClose(matrix1.M11, matrix2.M11) && 
               DoubleUtil.AreClose(matrix1.M12, matrix2.M12) && 
               DoubleUtil.AreClose(matrix1.M13, matrix2.M13) && 
               DoubleUtil.AreClose(matrix1.M14, matrix2.M14) && 

               DoubleUtil.AreClose(matrix1.M21, matrix2.M21) && 
               DoubleUtil.AreClose(matrix1.M22, matrix2.M22) && 
               DoubleUtil.AreClose(matrix1.M23, matrix2.M23) && 
               DoubleUtil.AreClose(matrix1.M24, matrix2.M24) && 

               DoubleUtil.AreClose(matrix1.M31, matrix2.M31) && 
               DoubleUtil.AreClose(matrix1.M32, matrix2.M32) && 
               DoubleUtil.AreClose(matrix1.M33, matrix2.M33) && 
               DoubleUtil.AreClose(matrix1.M34, matrix2.M34) && 

               DoubleUtil.AreClose(matrix1.OffsetX, matrix2.OffsetX) && 
               DoubleUtil.AreClose(matrix1.OffsetY, matrix2.OffsetY) && 
               DoubleUtil.AreClose(matrix1.OffsetZ, matrix2.OffsetZ) && 
               DoubleUtil.AreClose(matrix1.M44, matrix2.M44);
    }

    public static bool AreCloseEnough(double value1, double value2)
    {
        // Less stringent closeness after AreClose from DoubleUtil.
        // Remember that AreClose is always just a heuristic
        const double epsilon  = 4.0e-10;
        //in case they are Infinities (then epsilon check does not work)
        if(value1 == value2) return true;
        // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < epsilon
        double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
        double delta = value1 - value2;
        return(-eps < delta) && (eps > delta);
    }

    public static bool AreCloseEnough(Matrix3D matrix1, Matrix3D matrix2)
    {
        return AreCloseEnough(matrix1.M11, matrix2.M11) && 
               AreCloseEnough(matrix1.M12, matrix2.M12) && 
               AreCloseEnough(matrix1.M13, matrix2.M13) && 
               AreCloseEnough(matrix1.M14, matrix2.M14) && 

               AreCloseEnough(matrix1.M21, matrix2.M21) && 
               AreCloseEnough(matrix1.M22, matrix2.M22) && 
               AreCloseEnough(matrix1.M23, matrix2.M23) && 
               AreCloseEnough(matrix1.M24, matrix2.M24) && 

               AreCloseEnough(matrix1.M31, matrix2.M31) && 
               AreCloseEnough(matrix1.M32, matrix2.M32) && 
               AreCloseEnough(matrix1.M33, matrix2.M33) && 
               AreCloseEnough(matrix1.M34, matrix2.M34) && 

               AreCloseEnough(matrix1.OffsetX, matrix2.OffsetX) && 
               AreCloseEnough(matrix1.OffsetY, matrix2.OffsetY) && 
               AreCloseEnough(matrix1.OffsetZ, matrix2.OffsetZ) && 
               AreCloseEnough(matrix1.M44, matrix2.M44);
    }
}

