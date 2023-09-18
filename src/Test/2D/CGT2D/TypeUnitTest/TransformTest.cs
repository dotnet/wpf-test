// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class TransformTest : CoreGraphicsTest
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
                TestIdentity();
                TestInverse();
            }
        }

        private void TestIdentity()
        {
            Log("Testing Transform.Identity...");

            TestIdentityWith();
        }

        private void TestIdentityWith()
        {
            Transform theirAnswer = Transform.Identity;

            if (!MathEx.Equals(theirAnswer.Value, Matrix.Identity) || failOnPurpose)
            {
                AddFailure("Transform.Identity failed");
                Log("***Expected: Value = {0}", Matrix.Identity);
                Log("***Expected: Value = {0}", theirAnswer.Value);
            }
        }

        private void TestInverse()
        {
            Log("Testing Transform.Inverse...");

            //Actual test for Inverse is covered in Matrix API test
            TestInverseWith(new Matrix(1, 0, 0, 1, 0, 0));
            TestInverseWith(new Matrix(1, 2, 0, 1, 3, 0));
            TestInverseWith(new Matrix(0, 0, 0, 0, 0, 0));
        }

        private void TestInverseWith(Matrix matrix1)
        {
            MatrixTransform theirAnswer = new MatrixTransform(matrix1.M11, matrix1.M12, matrix1.M21, matrix1.M22, matrix1.OffsetX, matrix1.OffsetY);

            MatrixTransform inverse = (MatrixTransform)theirAnswer.Inverse;

            if (MatrixUtils.Determinant(matrix1) == 0)
            {
                if (theirAnswer.Inverse != null || failOnPurpose)
                {
                    AddFailure("Inverse failed: Expecting a null transform as inverse for {0}", matrix1);
                }
                return;
            }

            Matrix identity = MatrixUtils.Multiply(matrix1, inverse.Matrix);

            if (MathEx.NotCloseEnough(identity, Matrix.Identity) || failOnPurpose)
            {
                AddFailure("Inverse failed for {0}", matrix1);
            }
        }

        private void RunTest2()
        {
            throw new ApplicationException("Transform does not have any Pri 2 tests anymore - BC removed them");
        }
    }
}