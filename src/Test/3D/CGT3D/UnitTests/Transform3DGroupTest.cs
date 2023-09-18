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
    public class Transform3DGroupTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            TestCtor();
            TestCopy();
            TestValue();
            TestIsAffine();
        }

        private void TestCtor()
        {
            Log("Testing Constructors...");

            Transform3DGroup txg = new Transform3DGroup();

            if (txg.Children.Count != 0 || !txg.Value.IsIdentity ||
                 !MatrixUtils.IsIdentity(txg.Value) || failOnPurpose)
            {
                AddFailure("Default constructor for Transform3DGroup failed.");
                Log("*** Expected: Count = 0, IsIdentity = True");
                Log("*** Actual:   Count = {0}, IsIdentity = {1}, Value =\r\n{2}",
                     txg.Children.Count,
                     txg.Value.IsIdentity,
                     MatrixUtils.ToStr(txg.Value)
                     );
            }

            TestCtorWith(Transform3D.Identity);
            TestCtorWith(Const.tt10, Const.rtY90, Const.st1);
        }

        private void TestCtorWith(params Transform3D[] txs)
        {
            Transform3DGroup txg = new Transform3DGroup();
            txg.Children = new Transform3DCollection(txs);

            for (int i = 0; i < txs.Length; i++)
            {
                if (txg.Children[i].Value != txs[i].Value)
                {
                    AddFailure("Transform3D[]-param constructor for Transform3DGroup failed");
                    Log("*** Expected: Child {0} =\r\n{1}", i, MatrixUtils.ToStr(txs[i].Value));
                    Log("*** Actual:   Child {0} =\r\n{1}", i, MatrixUtils.ToStr(txg.Children[i].Value));
                    return;
                }
            }
        }

        private void TestCopy()
        {
            Log("Testing Copy...");

            TestCopyWith(Const.tg0);
            TestCopyWith(Const.tg1);
            TestCopyWith(Const.tg3);
        }

        private void TestCopyWith(Transform3DGroup txg)
        {
            Transform3DGroup copy = txg.Clone();

            if (copy == txg || failOnPurpose)
            {
                AddFailure("Copy failed");
                Log("The copy is actually just a reference to the original (shallow)");
            }

            for (int i = 0; i < copy.Children.Count; i++)
            {
                if (copy.Children[i].Value != txg.Children[i].Value)
                {
                    AddFailure("Copy failed");
                    Log("*** Expected: Child {0} =\r\n{1}", i, MatrixUtils.ToStr(copy.Children[i].Value));
                    Log("*** Actual:   Child {0} =\r\n{1}", i, MatrixUtils.ToStr(txg.Children[i].Value));
                    return;
                }
            }
        }

        private void TestValue()
        {
            Log("Testing Value...");

            TestValueWith(Transform3D.Identity);
            TestValueWith(Const.tt10, Const.rtY90, Const.st1);
            TestValueWith(Const.ttEps, Const.ttNeg1, Const.rtX45);
        }

        private void TestValueWith(params Transform3D[] txs)
        {
            Transform3DGroup txg = new Transform3DGroup();
            txg.Children = new Transform3DCollection(txs);
            Matrix3D theirValue = txg.Value;
            Matrix3D myValue = MatrixUtils.Multiply(txg);

            if (MathEx.NotCloseEnough(theirValue, myValue) || failOnPurpose)
            {
                AddFailure("Value failed.");
                Log("*** Expected:\r\n{0}", MatrixUtils.ToStr(myValue));
                Log("*** Actual:\r\n{0}", MatrixUtils.ToStr(theirValue));
            }
        }

        private void TestIsAffine()
        {
            Log("Testing IsAffine...");

            TestIsAffineWith(Transform3D.Identity);
            TestIsAffineWith(Const.tt10, Const.ttNeg1, Const.rtZ135, Const.st10, Const.mtAffine);
            TestIsAffineWith(Const.mtNAffine);
            TestIsAffineWith(Const.mtAffine);
        }

        private void TestIsAffineWith(params Transform3D[] txs)
        {
            Transform3DGroup txg = new Transform3DGroup();
            txg.Children = new Transform3DCollection(txs);
            bool theirValue = txg.IsAffine;
            bool myValue = MatrixUtils.IsAffine(txg.Value);

            if (theirValue != myValue || failOnPurpose)
            {
                AddFailure("IsAffine failed.");
                Log("*** Expected: {0}", myValue);
                Log("*** Actual:   {0}\r\n{1}", theirValue, MatrixUtils.ToStr(txg.Value));
            }
        }
    }
}