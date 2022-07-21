// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;

namespace DRT
{
    [TestedSecurityLevelAttribute(SecurityLevel.PartialTrust)]
    public class BezierTest : DrtInkTestcase
    {
        private string inkFileName = "Bezier.isf";

        private void PerformCuspTest(Stroke s, int [] cusps, bool bezierCusp)
        {
            int [] res = bezierCusp ? s.GetBezierCusps() : s.GetPolylineCusps();
            int hitCount = (null == cusps) ? 0 : cusps.Length;
            int resCount = (null == res) ? 0 : res.Length;
            if(hitCount != resCount)
            {
                throw new InvalidOperationException(
                    String.Format("Unexpected number of cusps returned by {0} : Expected {1} Actual {2}",
                    bezierCusp ? "Stroke.BezierCusps" : "Stroke.PolylineCusps",
                    hitCount, res.Length));
            }
            int i;
            for(i = 0; i < hitCount; ++i)
            {
                if(cusps[i] != res[i])
                    break;
            }
            if(i < hitCount)
            {
                throw new InvalidOperationException(
                    String.Format("Indices returned by {0} do not match with ground truth",
                    bezierCusp ? "Stroke.BezierCusps" : "Stroke.PolylineCusps"));
            }
        }

        private void TestPolyCusps()
        {
            int [] cusps1 = {0, 7, 17, 24, 44, 49, 57, 62, 70, 75, 83, 88, 94, 102, };
            int [] cusps2 = {0, 1, 29, 65, 98, 130, 159, 190, 223, 253, 291, 324, 359, };
            int [] cusps3 = {0, 101, };
            int [] cusps4 = {0, 32, 81, 133, 199, };
            int [] cusps5 = {0, 1, 57, 139, };
            int [] cusps6 = {0, 197, };
            int [] cusps7 = {0, 24, 64, 105, 140, 192, };
            int [] cusps8 = {0, 104, };

            object [] cusps = { cusps1, cusps2, cusps3, cusps4, cusps5, cusps6, cusps7, cusps8};

            for(int i = 0; i < TestStrokes.Count; ++i)
                this.PerformCuspTest(TestStrokes[i], (int [])cusps[i], false);
        }

        private void TestBezierCusps()
        {
            int [] cusps1 = {0, 6, 12, 18, 30, 36, 42, 48, 54, 60, 66, 72, 78, 84, };
            int [] cusps2 = {0, 3, 18, 39, 66, 87, 105, 123, 144, 168, 189, 210, 234, };
            int [] cusps3 = {0, 57, };
            int [] cusps4 = {0, 15, 45, 54, 87, };
            int [] cusps5 = {0, 3, 30, 72, };
            int [] cusps6 = {0, 105, };
            int [] cusps7 = {0, 18, 45, 72, 96, 135, };
            int [] cusps8 = {0, 75, };

            object [] cusps = { cusps1, cusps2, cusps3, cusps4, cusps5, cusps6, cusps7, cusps8};

            for(int i = 0; i < TestStrokes.Count; ++i)
                this.PerformCuspTest(TestStrokes[i], (int [])cusps[i], true);
        }

        public override void Run()
        {
            // Load reference ink file first
            TestStrokes = DrtHelpers.LoadInk( inkFileName);
            // Perform polyline cusp detection test
            TestPolyCusps();
            // Perform bezier cusp detection test
            TestBezierCusps();
            // done
            Success = true;
        }
    }
}
