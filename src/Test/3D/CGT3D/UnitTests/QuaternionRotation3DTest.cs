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
    public class QuaternionRotation3DTest : CoreGraphicsTest
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

            QuaternionRotation3D r = new QuaternionRotation3D();

            Quaternion q = r.Quaternion;

            if (MathEx.NotEquals(q, Quaternion.Identity) || failOnPurpose)
            {
                AddFailure("Default ctor for QuaternionRotation3D failed.");
                Log("*** Expected = 0,0,0,1");
                Log("*** Actual   = {0}", q);
            }

            TestCtorWith(Const.q0);
            TestCtorWith(Const.q1);
            TestCtorWith(Const.qX135);
            TestCtorWith(Const.qY90);
            TestCtorWith(Quaternion.Identity);
        }

        private void TestCtorWith(Quaternion q)
        {
            QuaternionRotation3D r = new QuaternionRotation3D(q);

            Quaternion theirAnswer = r.Quaternion;

            if (MathEx.NotCloseEnough(theirAnswer, q) || failOnPurpose)
            {
                AddFailure("ctor for QuaternionRotation3D with Quaternion failed.");
                Log("*** Expected = {0}", q);
                Log("*** Actual   = {0}", theirAnswer);
            }
        }

        private void TestProperties()
        {
            Log("Testing Quaternion get/set...");

            TestQuaternionWith(Const.q0);
            TestQuaternionWith(Const.qX180);
            TestQuaternionWith(Const.qX540);
            TestQuaternionWith(Const.qY90);
            TestQuaternionWith(Const.qZ135);
            TestQuaternionWith(Const.q1);
            TestQuaternionWith(Const.qNeg1);
            TestQuaternionWith(Quaternion.Identity);
        }

        private void TestQuaternionWith(Quaternion q)
        {
            QuaternionRotation3D r = new QuaternionRotation3D();
            r.Quaternion = q;

            Quaternion theirAnswer = r.Quaternion;

            if (MathEx.NotCloseEnough(theirAnswer, q) || failOnPurpose)
            {
                AddFailure("get/set_Quaternion failed");
                Log("*** Expected: {0}", q);
                Log("***   Actual: {0}", theirAnswer);
            }
        }
    }
}