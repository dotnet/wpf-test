// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class RotateTransform3DAPITest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTheTest2();
            }
            else
            {
                TestConstructors();
                TestProperties();
            }
        }

        private void TestConstructors()
        {
            Log("Testing Constructors...");

            TestConstructorWith(Const.rX135, Const.p10);
            TestConstructorWith(Const.rY90, Const.pNeg1);
            TestConstructorWith(null, Const.p1);
        }

        private void TestConstructorWith(Rotation3D rotation, Point3D center)
        {
            RotateTransform3D theirAnswer1 = new RotateTransform3D(rotation);
            RotateTransform3D theirAnswer2 = new RotateTransform3D(rotation, center);
            Point3D theirCenter = new Point3D(theirAnswer2.CenterX, theirAnswer2.CenterY, theirAnswer2.CenterZ);

            if (rotation != null)
            {
                if (MathEx.NotEquals(MathEx.Axis(theirAnswer1.Rotation), MathEx.Axis(rotation)) ||
                     MathEx.NotEquals(MathEx.Angle(theirAnswer1.Rotation), MathEx.Angle(rotation)) ||
                     failOnPurpose)
                {
                    AddFailure("RotateTransform3D.ctor( Rotation3D ) failed");
                    Log("*** Expected: Rotation = {0}", rotation);
                    Log("*** Actual:   Rotation = {0}", theirAnswer1.Rotation);
                }

                if (MathEx.NotEquals(MathEx.Axis(theirAnswer2.Rotation), MathEx.Axis(rotation)) ||
                     MathEx.NotEquals(MathEx.Angle(theirAnswer2.Rotation), MathEx.Angle(rotation)) ||
                     MathEx.NotEquals(theirCenter, center) ||
                     failOnPurpose)
                {
                    AddFailure("RotateTransform3D.ctor( Rotation3D, Point3D ) failed");
                    Log("*** Expected: Rotation = {0}, Center = {1}", rotation, center);
                    Log("*** Actual:   Rotation = {0}, Center = {1}", theirAnswer2.Rotation, theirCenter);
                }
            }
            else
            {
                if (theirAnswer1.Rotation != rotation || failOnPurpose)
                {
                    AddFailure("RotateTransform3D.ctor( null ) failed");
                    Log("*** Actual: Rotation = {0}", theirAnswer1.Rotation);
                }

                if (theirAnswer2.Rotation != rotation ||
                     MathEx.NotEquals(theirCenter, center) ||
                     failOnPurpose)
                {
                    AddFailure("RotateTransform3D.ctor( null ) failed");
                    Log("*** Actual: Rotation = {0}", theirAnswer2.Rotation);
                }
            }
        }

        private void TestProperties()
        {
            Log("Testing RotateTransform3D.Rotation.Angle/Axis ...");

            TestPropertiesWith(130, Const.yAxis, Const.p10);
            TestPropertiesWith(270.345, Const.v10, Const.pNeg1);
            TestPropertiesWith(200, Const.vNeg1, Const.p1);

            TestPropertiesWith(0, Const.v10, Const.p0);
            TestPropertiesWith(360, Const.v10, Const.p0);
            TestPropertiesWith(1280, Const.vNeg1, Const.p0);
            TestPropertiesWith(-12839.45, Const.zAxis, Const.p0);
        }

        private void TestPropertiesWith(double angle, Vector3D axis, Point3D center)
        {
            TestPropertiesWith(new AxisAngleRotation3D(axis, angle), center);
        }

        private void TestPropertiesWith(AxisAngleRotation3D rotation, Point3D center)
        {
            RotateTransform3D tx = Const.rtZ135;
            tx.Rotation = rotation;

            if (MathEx.NotEquals(MathEx.Axis(tx.Rotation), rotation.Axis) ||
                 MathEx.NotEquals(MathEx.Angle(tx.Rotation), rotation.Angle) ||
                 failOnPurpose)
            {
                AddFailure("get/set_Rotation property failed");
                Log("*** Expected: Rotation = " + rotation);
                Log("*** Actual:   Rotation = " + tx.Rotation);
            }

            tx.CenterX = center.X;
            tx.CenterY = center.Y;
            tx.CenterZ = center.Z;

            if (MathEx.NotEquals(MathEx.Axis(tx.Rotation), rotation.Axis) ||
                 MathEx.NotEquals(MathEx.Angle(tx.Rotation), rotation.Angle) ||
                 MathEx.NotEquals(tx.CenterX, center.X) ||
                 MathEx.NotEquals(tx.CenterY, center.Y) ||
                 MathEx.NotEquals(tx.CenterZ, center.Z) ||
                 failOnPurpose)
            {
                AddFailure("get/set_Rotation property failed");
                Log("*** Expected: Rotation = {0}, Center = {1}", rotation, center);
                Log("*** Actual:   Rotation = {0}, Center = {1},{2},{3}", tx.Rotation, tx.CenterX, tx.CenterY, tx.CenterZ);
            }
        }

        private void RunTheTest2()
        {
            throw new NotImplementedException("No pri 2 tests yet");
        }
    }
}