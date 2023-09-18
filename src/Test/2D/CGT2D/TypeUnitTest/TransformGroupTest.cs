// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class TransformGroupTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if (priority > 0)
            {
                RunTest2();
            }
            else
            {
                TestChildren();
                TestConstructor();

                // We only test non-animated TransformGroup, animated TransformGroup will be tested in Animation tests
                TestCloneCurrentValue();

                TestValue();
            }
        }

        private void TestChildren()
        {
            Log("Testing Children...");

            TestChildrenWith(new TransformCollection());
            TestChildrenWith(Const2D.transformCollection1);
        }

        private void TestChildrenWith(TransformCollection transformCollection1)
        {
            // set and get Children on TransformGroup with empty Collection
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children = transformCollection1;
            if (!ObjectUtils.Equals(transformGroup.Children, transformCollection1) || failOnPurpose)
            {
                AddFailure("Chidren failed");
                Log("Expect: {0}", transformCollection1);
                Log("Actual: {0}", transformGroup.Children);
            }

            // set and get on Chidren on TransformGroup with non empty Collection
            transformGroup = Const2D.transformGroup1;
            transformGroup.Children = transformCollection1;
            if (!ObjectUtils.Equals(transformGroup.Children, transformCollection1) || failOnPurpose)
            {
                AddFailure("Chidren failed");
                Log("Expect: {0}", transformCollection1);
                Log("Actual: {0}", transformGroup.Children);
            }
        }

        private void TestConstructor()
        {
            Log("Testing Constructor TransformGroup()...");

            TestConstructorWith();
        }

        private void TestConstructorWith()
        {
            TransformGroup theirAnswer = new TransformGroup();

            if (ObjectUtils.Equals(theirAnswer.Children, null) || !theirAnswer.Children.Count.Equals(0) || failOnPurpose)
            {
                AddFailure("Constructor TransformGroup() failed");
                Log("Expect: TransformGroup with empty Collection");
            }
        }

        private void TestCloneCurrentValue()
        {
            Log("Testing CloneCurrentValue()...");

            // TransformGroup with empty Collection
            TestCloneCurrentValueWith(new TransformGroup());

            // General TransformGroup
            TestCloneCurrentValueWith(Const2D.transformGroup1);
        }

        private void TestCloneCurrentValueWith(TransformGroup transformGroup1)
        {
            TransformGroup theirAnswer = transformGroup1.CloneCurrentValue();

            if (!ObjectUtils.DeepEquals(theirAnswer, transformGroup1) || failOnPurpose)
            {
                AddFailure("CloneCurrentValue() failed");
                Log("Expected: {0}", transformGroup1);
                Log("Actual: {0}", theirAnswer);
            }
        }

        private void TestValue()
        {
            Log("Testing Value...");

            // TransformGroup with empty Collection
            TestValueWith(new TransformGroup());

            // Group with only one Transform in Collection
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(Const2D.translate10);
            TestValueWith(transformGroup);

            // General TransformGroup
            TestValueWith(Const2D.transformGroup1);
        }

        private void TestValueWith(TransformGroup transformGroup1)
        {
            Matrix theirAnswer = transformGroup1.Value;
            Matrix myAnswer = Const2D.typeIdentity;

            if (!ObjectUtils.Equals(transformGroup1.Children, null) && !transformGroup1.Children.Count.Equals(0))
            {
                for (int i = 0; i < transformGroup1.Children.Count; i++)
                {
                    myAnswer = myAnswer * transformGroup1.Children[i].Value;
                }
            }

            if (!MathEx.Equals(theirAnswer, myAnswer) || failOnPurpose)
            {
                AddFailure("Value failed");
                Log("*** Expected: Value = {0}", myAnswer);
                Log("*** Actual: Value = {0}", theirAnswer);
            }
        }

        private void RunTest2()
        {
            // these are P2 tests, overflow, bignumber, exception, null
            TestChildren2();
            TestCloneCurrentValue2();
            TestValue2();
        }

        private void TestChildren2()
        {
            Log("P2 Testing Children...");

            TestChildrenWith(null);
        }

        private void TestCloneCurrentValue2()
        {
            Log("P2 Testing CloneCurrentValue()...");

            // TransformGroup with Collection explicitly set to null
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children = null;
            TestCloneCurrentValueWith(transformGroup);

        }

        private void TestValue2()
        {
            Log("P2 Testing Value...");

            // TransformGroup with Collection explicitly set to null
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children = null;
            TestValueWith(transformGroup);
        }
    }
}
