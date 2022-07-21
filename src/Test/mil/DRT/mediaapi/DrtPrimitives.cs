// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a set of DRTs designed to test the MediaAPIs 
//              and public classes.
//
//

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using MS.Internal;

/// <summary>
/// Test for primitive opertations
/// </summary>
public class DrtPrimitives : DrtPrimitiveBase
{
    /// <summary>
    /// Run DRT.
    /// </summary>
    public override bool Run(out string results)
    {
        bool succeeded = true;
        
        string floatUtil, doubleUtil, vectors, points, matrix, rects, primitiveStrings, empty;

        // Note that because of the order of the operands to &&, all tests will run
        // even if one failed.  This is intentional, and ensures that all regressions 
        // are noted, not just the initial failure.

        succeeded = TestFloatUtil(out floatUtil) && succeeded;
        succeeded = TestDoubleUtil(out doubleUtil) && succeeded;
        succeeded = TestVectors(out vectors) && succeeded;
        succeeded = TestPoints(out points) && succeeded;
        succeeded = TestMatrix(out matrix) && succeeded;
        succeeded = TestRects(out rects) && succeeded;
        succeeded = TestPrimitiveStringOperations(out primitiveStrings) && succeeded;
        succeeded = TestEmptyStrings(out empty) && succeeded;
        succeeded = TestEquality(out empty) && succeeded;

        // If failure occurred, then we care about the output
        if (!succeeded)
        {
            results = floatUtil + 
                      doubleUtil + 
                      vectors + 
                      points + 
                      matrix +
                      rects +
                      primitiveStrings +
                      empty;
        }
        else
        {
            results = "SUCCEEDED";
        }

        return succeeded;
    }

    private bool TestFloatUtil(out string results)
    {
        bool succeed = true;
        int test = 0;

        //Note that float epsilon is actually 1.19209289*6*e-7, so this is "within epsilon".
        //Of course, we test against 10 epsilon in FloatUtil, so this should definately
        //be close enough
        float ep = 1.192092895e-07F;

        results = "";

        test++; if (!FloatUtil.IsZero(0)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsOne(1.0f)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.AreClose(0,0)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.AreClose(1,1)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.AreClose(1.0f, 1.0f + ep)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.AreClose(10.0f * 1.0f, 10.0f * (1.0f + ep))) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsZero(ep)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsZero(1e-4f * 1e-4f)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsZero(-ep)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsOne(1.0f - ep)) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.AreClose((4.0f + ep) * 10.1e11f, (4.0f * 10.1e11f) + (ep * 10.1e11f))) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }
        test++; if (!FloatUtil.IsOne(12.3492e4f * (1.0f / 12.3492e4f))) { succeed = false; results += "FloatUtil Test " + test + " FAILED\n"; }

        return succeed;
    }

    private bool TestDoubleUtil(out string results)
    {
        bool succeed = true;
        int test = 0;

        //Note that double epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        //Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        //be close enough
        double ep = 2.2204460492503131e-017;

        results = "";

        test++; if (!DoubleUtil.IsZero(0)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsOne(1.0)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(0,0)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(1,1)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(1.0, 1.0 + ep)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(10.0 * 1.0, 10.0 * (1.0 + ep))) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsZero(ep)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsZero(1e-9 * 1e-9)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsZero(-ep)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsOne(1.0 - ep)) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose((4.0 + ep) * 10.1e11, (4.0 * 10.1e11) + (ep * 10.1e11))) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.IsOne(12.3492e4 * (1.0 / 12.3492e4))) { succeed = false; results += "DoubleUtil Test " + test + " FAILED\n"; }

        return succeed;
    }

    private bool TestVectors(out string results)
    {
        bool succeed = true;
        int test = 0;
        //Note that epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        //Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        //be close enough
        double ep = 2.2204460492503131e-017;

        results = "";

        Vector vec1 = new Vector(12.3, -19.332);

        test++; if (!(vec1.X == 12.3)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!(vec1.Y == -19.332)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(vec1.Length, 22.913232508749175)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(vec1, new Vector(12.3 - ep, -19.332 + ep))) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }

        Vector vec2 = vec1;

        test++; if (!DoubleUtil.AreClose(vec1, vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!(vec1.GetHashCode() == vec2.GetHashCode())) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(vec1 * 2, vec2 + vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!(vec1 == vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1 != vec2) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        
        vec1.X = 24.6;
        vec1.Y = -38.664;
        test++; if (!DoubleUtil.AreClose(vec1, (vec2 * 2))) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(vec1 * 4.5, vec2 * 10 - vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose((vec1 / 3.0), vec2 / 2.0 + (vec2 / 6.0))) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(null)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(1.0)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (vec1.Equals(vec2)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }
        test++; if (!vec1.Equals(vec1)) { succeed = false; results += "Vector Test " + test + " FAILED\n"; }

        return succeed;
    }

    private bool TestPoints(out string results)
    {
        bool succeed = true;
        int test = 0;
        //Note that epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        //Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        //be close enough
        double ep = 2.2204460492503131e-017;

        results = "";

        Point pt1 = new Point(12.3, -19.332);

        test++; if (!(pt1.X == 12.3)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!(pt1.Y == -19.332)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(pt1, new Point(12.3 - ep, -19.332 + ep))) { succeed = false; results += "Point Test " + test + " FAILED\n"; }

        Point pt2 = pt1;

        test++; if (!DoubleUtil.AreClose(pt1, pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!(pt1.GetHashCode() == pt2.GetHashCode())) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!(pt1 == pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (pt1 != pt2) { succeed = false; results += "Point Test " + test + " FAILED\n"; }

        pt1.X = 24.6;
        pt1.Y = -38.664;
        Vector vec = pt2 - new Point(0,0);
        test++; if (!DoubleUtil.AreClose(pt1, pt2 + vec)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(pt1 + ((pt1 - new Point(0,0)) * 3.5), pt2 + vec * 8)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (pt1.Equals(null)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (pt1.Equals(1.0)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (pt1.Equals(pt2)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }
        test++; if (!pt1.Equals(pt1)) { succeed = false; results += "Point Test " + test + " FAILED\n"; }

        return succeed;
    }

    /// <summary>
    /// CompareMatrix - compare the values of the matrix against the values 
    /// passed in.  If exact is true, uses ==, else uses AreClose.
    /// </summary>
    /// <returns>
    /// bool - the result of the comparison.
    /// </returns>
    private bool CompareMatrix(Matrix matrix, 
                               double m11, 
                               double m12,
                               double m21, 
                               double m22,
                               double offsetX,
                               double offsetY,
                               bool exact)
    {
        if (exact)
        {
            return (m11 == matrix.M11) &&
                   (m12 == matrix.M12) &&
                   (m21 == matrix.M21) &&
                   (m22 == matrix.M22) &&
                   (offsetX == matrix.OffsetX) &&
                   (offsetY == matrix.OffsetY);
        }
        else
        {
            return DoubleUtil.AreClose(m11,matrix.M11) &&
                   DoubleUtil.AreClose(m12,matrix.M12) &&
                   DoubleUtil.AreClose(m21,matrix.M21) &&
                   DoubleUtil.AreClose(m22,matrix.M22) &&
                   DoubleUtil.AreClose(offsetX,matrix.OffsetX) &&
                   DoubleUtil.AreClose(offsetY,matrix.OffsetY);
        }
    }

    private bool TestMatrix(out string results)
    {
        bool succeed = true;
        int test = 0;

        results = "";

        test++; if (!CompareMatrix(Matrix.Identity, 1,0,0,1,0,0, true)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        Matrix matrix1 = new Matrix(12.3, -19.332, 12.33, 11222, -00.233, 0);

        matrix1.SetIdentity();

        test++; if (!CompareMatrix(matrix1, 1,0,0,1,0,0, true)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == Matrix.Identity)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(Matrix.Equals(matrix1, Matrix.Identity))) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (matrix1 != Matrix.Identity) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!matrix1.Equals(Matrix.Identity)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        matrix1 = new Matrix(1, 0, 0, 1, 0, 0);

        test++; if (!CompareMatrix(matrix1, 1,0,0,1,0,0, true)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == Matrix.Identity)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(Matrix.Equals(matrix1, Matrix.Identity))) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (matrix1 != Matrix.Identity) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!matrix1.Equals(Matrix.Identity)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        matrix1 = new Matrix(12.3, -19.332, 12.33, 11222, -00.233, 0);
        
        Matrix matrix2 = matrix1;

        test++; if (!CompareMatrix(matrix1, matrix2.M11, matrix2.M12, matrix2.M21, matrix2.M22, matrix2.OffsetX, matrix2.OffsetY, true)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(Matrix.Equals(matrix1, matrix2))) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (matrix1 != matrix2) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!matrix1.Equals(matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        matrix1.SetIdentity();
        matrix2.SetIdentity();

        // Matrix multiplication fun

        // There are 5 general types (ident, translate, scale, translate and scale, everything else)
        // which leads to 25 different cases.  Fun.

        // Identity * Identity
        matrix1 *= matrix2;

        test++; if (!CompareMatrix(matrix1, 1,0,0,1,0,0, true)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        // Identity * Translate
        matrix1.SetIdentity();
        matrix2.SetIdentity();
        matrix2.Translate(12.3, -0.002);
        matrix1 *= matrix2;

        test++; if (!CompareMatrix(matrix1, 1,0,0,1,12.3,-0.002, false)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        // Identity * Scale
        matrix1.SetIdentity();
        matrix2.SetIdentity();
        matrix2.Scale(12.3, -0.002);
        matrix1 *= matrix2;

        test++; if (!CompareMatrix(matrix1, 12.3, 0, 0, -0.002, 0, 0, false)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        // Identity * (Scale and Translate)
        matrix1.SetIdentity();
        matrix2.SetIdentity();
        matrix2.Scale(12.3, -0.002);
        matrix2.Translate(12.3, -0.002);
        matrix1 *= matrix2;
        
        test++; if (!CompareMatrix(matrix1, 12.3, 0, 0, -0.002, 12.3, -0.002, false)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        // Identity * other
        matrix1.SetIdentity();
        matrix2 = new Matrix(12.3, 3333, 12.332, -23.1, 0, -2);
        matrix1 *= matrix2;

        test++; if (!CompareMatrix(matrix1, 12.3, 3333, 12.332, -23.1, 0, -2, false)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }
        test++; if (!(matrix1 == matrix2)) { succeed = false; results += "Matrix Test " + test + " FAILED\n"; }

        // 
        
        return succeed;
    }

    private bool TestRects(out string results)
    {
        bool succeed = true;
        int test = 0;

        //Note that epsilon is actually 2.2204460492503131e-016, so this is "within epsilon".
        //Of course, we test against 10 epsilon in FloatUtil, so this should definitely
        //be close enough
        double ep = 2.2204460492503131e-017;

        results = "";

        Rect rect1 = new Rect(12.3, -19.332, 12.3, 19.332);

        test++; if (!(rect1.X == 12.3)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Y == -19.332)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Width == 12.3)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Height == 19.332)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Right == 24.6)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.Bottom == 0)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.Location).Length, 22.913232508749175)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(rect1, new Rect(12.3 - ep, -19.332 + ep, 12.3 - ep, 19.332 + ep))) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.Size).Length, 22.913232508749175)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.TopLeft).Length, 22.913232508749175)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.TopRight).Length, 31.287157493131268)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.BottomLeft).Length, 12.3)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!DoubleUtil.AreClose(((Vector)rect1.BottomRight).Length, 24.6)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }

        Rect rect2 = rect1;

        test++; if (!DoubleUtil.AreClose(rect1, rect2)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1.GetHashCode() == rect2.GetHashCode())) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (!(rect1 == rect2)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        test++; if (rect1 != rect2) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        
        rect2 = new Rect(27.0, 0, 33, 50);
        rect2.Union(rect1);
        test++; if (!DoubleUtil.AreClose(new Rect(12.3, -19.332, 47.7, 69.332), rect2)) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }

        // Regression test for 
        rect2 = new Rect(3, 4, 0, 0);
        rect2.Inflate(0, 1);
        // The rect still should not be empty:
        test++; if (rect2.IsEmpty) { succeed = false; results += "Rect Test " + test + " FAILED\n"; }
        
        // 

        return succeed;
    }

    public class CultureTest
    {
        public CultureTest(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public void AddTest(object obj, string str)
        {
            _objects.Add(obj);
            _strings.Add(str);
        }

        public CultureInfo _cultureInfo;
        public ArrayList _objects = new ArrayList();
        public ArrayList _strings = new ArrayList();
    }

    private bool TestPrimitiveStringOperations(out string results)
    {
        bool succeed = true;
        int test = 0;
        
        int cultureTest = 0;

        // We use these so that intermediate results are stored somewhere.  If they aren't,
        // this is pretty hard to debug.
        string stringOutput = String.Empty;
        object objOuput = null;

        results = "";

        Point testPoint = new Point(12.34, 0);
        Size testSize = new Size(1.0, 56.78);
        Vector testVector = new Vector(910.11, -12.13);
        Rect testRect = new Rect(14.15, 16.17, 18.19, 20);
        Matrix testMatrix = new Matrix(1.2, 11.3, -1293, 0, 0, 0.1122);

        CultureTest[] cultureTests = new CultureTest[6];

        // This tests invariant culture, the markup scenario
        cultureTests[cultureTest] = new CultureTest(CultureInfo.InvariantCulture);
        cultureTests[cultureTest].AddTest(testPoint, "12.34,0");
        cultureTests[cultureTest].AddTest(testSize,"1,56.78");
        cultureTests[cultureTest].AddTest(testVector,"910.11,-12.13");
        cultureTests[cultureTest].AddTest(testRect,"14.15,16.17,18.19,20");
        cultureTests[cultureTest].AddTest(testMatrix,"1.2,11.3,-1293,0,0,0.1122");
                   
        // This tests null vs. current culture
        cultureTest++;
        cultureTests[cultureTest] = new CultureTest(null);
        cultureTests[cultureTest].AddTest(testPoint,(testPoint).ToString(CultureInfo.CurrentCulture));
        cultureTests[cultureTest].AddTest(testSize,(testSize).ToString(CultureInfo.CurrentCulture));
        cultureTests[cultureTest].AddTest(testVector,(testVector).ToString(CultureInfo.CurrentCulture));
        cultureTests[cultureTest].AddTest(testRect,(testRect).ToString(CultureInfo.CurrentCulture));
        cultureTests[cultureTest].AddTest(testMatrix,(testMatrix).ToString(CultureInfo.CurrentCulture));
                
        // This tests current culture vs. ToString()
        cultureTest++;
        cultureTests[cultureTest] = new CultureTest(CultureInfo.CurrentCulture);
        cultureTests[cultureTest].AddTest(testPoint,(testPoint).ToString());
        cultureTests[cultureTest].AddTest(testSize,(testSize).ToString());
        cultureTests[cultureTest].AddTest(testVector,(testVector).ToString());
        cultureTests[cultureTest].AddTest(testRect,(testRect).ToString());
        cultureTests[cultureTest].AddTest(testMatrix,(testMatrix).ToString());

        // This tests null vs. ToString()
        cultureTest++;
        cultureTests[cultureTest] = new CultureTest(null);
        cultureTests[cultureTest].AddTest(testPoint,(testPoint).ToString());
        cultureTests[cultureTest].AddTest(testSize,(testSize).ToString());
        cultureTests[cultureTest].AddTest(testVector,(testVector).ToString());
        cultureTests[cultureTest].AddTest(testRect,(testRect).ToString());
        cultureTests[cultureTest].AddTest(testMatrix,(testMatrix).ToString());

        // This tests null vs. ToString(null)
        cultureTest++;
        cultureTests[cultureTest] = new CultureTest(null);
        cultureTests[cultureTest].AddTest(testPoint,(testPoint).ToString(null));
        cultureTests[cultureTest].AddTest(testSize,(testSize).ToString(null));
        cultureTests[cultureTest].AddTest(testVector,(testVector).ToString(null));
        cultureTests[cultureTest].AddTest(testRect,(testRect).ToString(null));
        cultureTests[cultureTest].AddTest(testMatrix,(testMatrix).ToString(null));
                
        // This tests fr-FR culture, which uses "," as a decimal place separator
        cultureTest++;
        cultureTests[cultureTest] = new CultureTest(new CultureInfo("fr-FR", false));
        cultureTests[cultureTest].AddTest(testPoint,"12,34;0");
        cultureTests[cultureTest].AddTest(testSize,"1;56,78");
        cultureTests[cultureTest].AddTest(testVector,"910,11;-12,13");
        cultureTests[cultureTest].AddTest(testRect,"14,15;16,17;18,19;20");
        cultureTests[cultureTest].AddTest(testMatrix,"1,2;11,3;-1293;0;0;0,1122");

        // Type arrays for use later to find ToString(IFormatProvider) and Equals
        Type[] toStringTypeArray = new Type[1];
        toStringTypeArray[0] = typeof(IFormatProvider);

        Type[] equalsTypeArray = new Type[1];
        equalsTypeArray[0] = typeof(object);

        for (int i = 0; i < cultureTests.Length; i++)
        {
            // Sanity check to make sure we have the same number of objects as strings.
            Debug.Assert(cultureTests[i]._objects.Count == cultureTests[i]._strings.Count);

            for (int j = 0; j < cultureTests[i]._objects.Count; j++)
            {
                // Let's reflect a bit a grab the ToString(IFormatProvider) method and the Equals method
                MethodInfo toString = cultureTests[i]._objects[j].GetType().GetMethod("ToString", toStringTypeArray);
                Debug.Assert(null != toString);

                MethodInfo equals = cultureTests[i]._objects[j].GetType().GetMethod("Equals", equalsTypeArray);
                Debug.Assert(null != equals);

                object[] temp = new object[] { cultureTests[i]._cultureInfo };
                string objectToStringOutput = (string)toString.Invoke(cultureTests[i]._objects[j], temp );

                // Test object using ToString(IFormattable)
                test++; 
                if (0 != String.Compare(objectToStringOutput,
                                        (string)cultureTests[i]._strings[j],
                                        false, /* false == case sensitive */
                                        CultureInfo.InvariantCulture))
                {
                    succeed = false; 
                    results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
                }


                // Test object using the type converter to convert to a string
                test++; 

                stringOutput = (string)TypeDescriptor.GetConverter(cultureTests[i]._objects[j]).ConvertTo(
                        null, /* Type descriptor */
                        cultureTests[i]._cultureInfo,
                        cultureTests[i]._objects[j],
                        typeof(string));

                if (0 != String.Compare(stringOutput,
                                        objectToStringOutput,
                                        false, /* false == case sensitive */
                                        CultureInfo.InvariantCulture))
                {
                    succeed = false; 
                    results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
                }

                // This tests that the object correctly implements IFormattable
                test++;
                
                stringOutput = String.Format(cultureTests[i]._cultureInfo,
                                             "{0}",
                                             cultureTests[i]._objects[j]);

                if (0 != String.Compare(stringOutput,
                                        (string)cultureTests[i]._strings[j],
                                        false, /* false == case sensitive */
                                        CultureInfo.InvariantCulture))
                {
                    succeed = false; 
                    results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
                }
            }

            // Test Rect Empty using ToString(IFormattable)
            test++; 

            stringOutput = Rect.Empty.ToString(cultureTests[i]._cultureInfo);

            if (0 != String.Compare(stringOutput,
                                    "Empty",
                                    false, /* false == case sensitive */
                                    CultureInfo.InvariantCulture))
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }

            // Test Rect.Empty via type converter from string
            test++; 

            objOuput = TypeDescriptor.GetConverter(Rect.Empty).ConvertFrom(
                null, /* Type descriptor */
                cultureTests[i]._cultureInfo,
                Rect.Empty.ToString(cultureTests[i]._cultureInfo));

            if ((Rect)objOuput != Rect.Empty)
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }

            // Test Rect.Empty via type converter to string
            test++; 
            
            stringOutput = (string)TypeDescriptor.GetConverter(Rect.Empty).ConvertTo(
                null, /* Type descriptor */
                cultureTests[i]._cultureInfo,
                Rect.Empty,
                typeof(string));

            if (0 != String.Compare(
                stringOutput,
                Rect.Empty.ToString(cultureTests[i]._cultureInfo),
                false, /* false == case sensitive */
                CultureInfo.InvariantCulture))
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }

            // Test Matrix Identity using ToString(IFormattable)
            test++; 

            stringOutput = Matrix.Identity.ToString(cultureTests[i]._cultureInfo);

            if (0 != String.Compare(stringOutput,
                                    "Identity",
                                    false, /* false == case sensitive */
                                    CultureInfo.InvariantCulture))
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }

            // Test Matrix.Identity via type converter from string
            test++; 

            objOuput = TypeDescriptor.GetConverter(Matrix.Identity).ConvertFrom(
                null, /* Type descriptor */
                cultureTests[i]._cultureInfo,
                Matrix.Identity.ToString(cultureTests[i]._cultureInfo));

            if ((Matrix)objOuput != Matrix.Identity)
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }

            // Test Matrix.Identity via type converter to string
            test++; 
            
            stringOutput = (string)TypeDescriptor.GetConverter(Matrix.Identity).ConvertTo(
                null, /* Type descriptor */
                cultureTests[i]._cultureInfo,
                Matrix.Identity,
                typeof(string));

            if (0 != String.Compare(
                stringOutput,
                Matrix.Identity.ToString(cultureTests[i]._cultureInfo),
                false, /* false == case sensitive */
                CultureInfo.InvariantCulture))
            {
                succeed = false; 
                results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
            }
        }

        // Test the "format" of IFormattable
        CultureTest[] formatTests = new CultureTest[2];
        
        cultureTest = 0;
        formatTests[cultureTest] = new CultureTest(CultureInfo.InvariantCulture);
        formatTests[cultureTest].AddTest(testPoint,"12,0");
        formatTests[cultureTest].AddTest(testSize,"1,57");
        formatTests[cultureTest].AddTest(testVector,"910,-12");
        formatTests[cultureTest].AddTest(testRect,"14,16,18,20");
        formatTests[cultureTest].AddTest(testMatrix,"1,11,-1293,0,0,0");

        cultureTest++;
        formatTests[cultureTest] = new CultureTest(new CultureInfo("fr-FR", false));
        formatTests[cultureTest].AddTest(testPoint,"12;0");
        formatTests[cultureTest].AddTest(testSize,"1;57");
        formatTests[cultureTest].AddTest(testVector,"910;-12");
        formatTests[cultureTest].AddTest(testRect,"14;16;18;20");
        formatTests[cultureTest].AddTest(testMatrix,"1;11;-1293;0;0;0");

        for (int i = 0; i < formatTests.Length; i++)
        {
            for (int j = 0; j < formatTests[i]._objects.Count; j++)
            {
                // This tests that the object correctly implements IFormattable and the format string.
                test++;
                
                stringOutput = String.Format(formatTests[i]._cultureInfo,
                                             "{0:####0}",
                                             formatTests[i]._objects[j]);

                if (0 != String.Compare(stringOutput,
                                        (string)formatTests[i]._strings[j],
                                        false, /* false == case sensitive */
                                        CultureInfo.InvariantCulture))
                {
                    succeed = false; 
                    results += "PrimitiveStringOperations Test " + test + " FAILED\n"; 
                }            
            }
        }
        
        return succeed;
    }


    private bool TestEmptyStrings(out string results)
    {
        bool success = true;
        results = String.Empty;

        success &= EmptyTestHelper(typeof(Rect), "Empty", ref results);
        success &= EmptyTestHelper(typeof(Size), "Empty", ref results);
        success &= EmptyTestHelper(typeof(Matrix), "Identity", ref results);
        return success;
    }

    private bool TestEquality(out string results)
    {
        bool success = true;
        results = String.Empty;

        // Here's the pattern set for us by the clr.  The symbol == is
        // the strange non-reflexive IEEE fp equality.  The method
        // Equals(object) is "value" equality and must be reflexive.
#if false
    // Compiler warning about unreachable code.  Cool.
        if (Double.NaN == Double.NaN)
        {
            success = false;
            results += "FAILED if (Double.NaN == Double.NaN)"
                       + "\n";
        }
#endif
        if (!Double.NaN.Equals(Double.NaN))
        {
            success = false;
            results += "FAILED if (!Double.NaN.Equals(Double.NaN))"
                       + "\n";
        }

        double poszero = 0;
        double negzero = poszero * -1;
        
        if (poszero != negzero)
        {
            success = false;
            results += "if (poszero != negzero)"
                       + "\n";
        }
        // This shouldn't pass but it does.  For what it's worth
        // they have different hash codes.  This is a bummer.
        if (!poszero.Equals(negzero))
        {
            success = false;
            results += "if (!poszero.Equals(negzero))"
                       + "\n";
        }

#if false // unsafe code
        // FWIW, I confirmed that different nans also compare as
        // Equal, despite having different hash codes.
        double nan = Double.NaN;
        double nanan = Double.NaN;
        unsafe
        {
            Int64 *pNanan = (Int64*)&nanan;
            *pNanan = *pNanan ^ 101;
        }

        if (nan.Equals(nanan))
        {
            if (nan.GetHashCode() != nanan.GetHashCode())
            {
                Console.WriteLine("Equals should imply hash equality.");
            }
        }
#endif
    
        Vector3D vecNan = new Vector3D(Double.NaN,Double.NaN,Double.NaN);
        Vector3D vecNan2 = new Vector3D(Double.NaN,Double.NaN,Double.NaN);
        Vector3D vecPosZero = new Vector3D(0,0,0);
        Vector3D vecNegZero = -1 * vecPosZero;

        if (vecNan == vecNan2)
        {
            success = false;
            results += "FAILED if (vecNan == vecNan)"
                       + "\n";
        }

        if (!vecNan.Equals(vecNan))
        {
            success = false;
            results += "FAILED if (!vecNan.Equals(vecNan))"
                       + "\n";
        }
        if (vecPosZero != vecNegZero)
        {
            success = false;
            results += "if (vecPosZero != vecNegZero)"
                       + "\n";
        }
        if (!vecPosZero.Equals(vecNegZero))
        {
            success = false;
            results += "if (!vecPosZero.Equals(vecNegZero))"
                       + "\n";
        }
        
        return success;
    }
}

