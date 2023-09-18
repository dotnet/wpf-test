// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class Rotation3DTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestCtor();
                TestProperties();
            }
            else // priority > 0
            {
            }
        }

        private void TestCtor()
        {
            Log("Testing constructors...");

            AxisAngleRotation3D r = new AxisAngleRotation3D();

            Vector3D axis = r.Axis;

            if (MathEx.NotEquals(axis, new Vector3D(0, 1, 0)) || r.Angle != 0 || failOnPurpose)
            {
                AddFailure("Default ctor for AxisAngleRotation3D failed.");
                Log("*** Expected = 0,1,0 0");
                Log("*** Actual   = {0} {1}", axis, r.Angle);
            }

            TestCtorWith(Const.v0, 0);
            TestCtorWith(Const.v10, 0);
            TestCtorWith(Const.vNeg1, 90);
            TestCtorWith(Const.xAxis, 520);
        }

        private void TestCtorWith(Vector3D v, double a)
        {
            AxisAngleRotation3D r = new AxisAngleRotation3D(v, a);

            Vector3D axis = r.Axis;

            if (MathEx.NotCloseEnough(axis.X, v.X) || MathEx.NotCloseEnough(axis.Y, v.Y) ||
                 MathEx.NotCloseEnough(axis.Z, v.Z) || MathEx.NotCloseEnough(r.Angle, a) ||
                 failOnPurpose)
            {
                AddFailure("ctor for AxisAngleRotation3D with axis, angle failed.");
                Log("*** Expected = {0} {1}", v, a);
                Log("*** Actual   = {0} {1}", axis, r.Angle);
            }
        }

        private void TestProperties()
        {
            Log("Testing Axis/Angle get/set...");

            TestAxisAngleWith(Const.v0, 0);
            TestAxisAngleWith(Const.xAxis, 0);
            TestAxisAngleWith(Const.xAxis, 90);
            TestAxisAngleWith(Const.yAxis, -135);
            TestAxisAngleWith(Const.zAxis, 1060);
            TestAxisAngleWith(Const.zAxis, -1060);
            TestAxisAngleWith(Const.v1, 90);
            TestAxisAngleWith(Const.vNeg1, 360);
        }

        private void TestAxisAngleWith(Vector3D axis, double angle)
        {
            AxisAngleRotation3D r = new AxisAngleRotation3D();
            r.Axis = axis;
            r.Angle = angle;

            Vector3D v = r.Axis;
            double a = r.Angle;

            if (MathEx.NotCloseEnough(v, axis) || MathEx.NotCloseEnough(a, angle) ||
                 failOnPurpose)
            {
                AddFailure("Axis/Angle get/set failed.");
                Log("*** Expected: Axis={0}, Angle={1}", axis, angle);
                Log("***   Actual: Axis={0}, Angle={1}", v, a);
            }
        }
    }
}