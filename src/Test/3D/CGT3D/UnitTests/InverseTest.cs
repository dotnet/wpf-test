// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class InverseTest : CoreGraphicsTest
    {
        private const int numMatrices = 25;

        /// <summary/>
        public override void RunTheTest()
        {
            for (int n = 0; n < numMatrices; n++)
            {
                TestMatrix(GetMatrix(n));
            }
        }

        private void TestMatrix(Matrix3D m)
        {
            Matrix3D inverse;

            if (GetInverse(m, out inverse))
            {
                VerifyInverse(m, inverse);
            }
        }

        private bool GetInverse(Matrix3D m, out Matrix3D inverse)
        {
            bool hasInverse = true;

            inverse = m;
            try
            {
                inverse.Invert();
                if (!m.HasInverse || failOnPurpose)
                {
                    AddFailure("Invert failed to throw exception for non-invertable matrix");
                }
            }
            catch (InvalidOperationException ex)
            {
                hasInverse = false;
                inverse = default(Matrix3D);

                if (m.HasInverse)
                {
                    AddFailure("Matrix does not have an inverse, but property HasInverse is true.\r\n" +
                                "Original:\r\n" +
                                MatrixUtils.ToStr(m) +
                                ex.ToString()
                                );
                }
                else if (m.Determinant == 0)
                {
                    Log("The correct exception was thrown for matrix without an Inverse");
                }
                else
                {
                    AddFailure("Determinant is not 0; this matrix should have an inverse:\r\n" +
                                MatrixUtils.ToStr(m) +
                                ex.ToString());
                }
            }

            return hasInverse;
        }

        private void VerifyInverse(Matrix3D m, Matrix3D m_inv)
        {
            Matrix3D result;

            // We might have bogus info in either matrix by now
            try
            {
                result = m * m_inv;
            }
            catch (InvalidOperationException ex)
            {
                AddFailure("Could not multiply:\r\n" +
                            "Original * Inverse = Identity\r\n" +
                            "Original Matrix:\r\n" +
                            MatrixUtils.ToStr(m) +
                            "\r\nInverse:\r\n" +
                            MatrixUtils.ToStr(m_inv) +
                            ex.ToString()
                            );

                return;
            }

            if (MatrixUtils.IsIdentity(result))
            {
                Log("The inverse is correct");
            }
            else
            {
                AddFailure("Original * Inverse should be the Identity matrix.\r\n" +
                            "Original Matrix:\r\n" +
                            MatrixUtils.ToStr(m) +
                            "\r\nInverse:\r\n" +
                            MatrixUtils.ToStr(m_inv) +
                            "\r\nResult:\r\n" +
                            MatrixUtils.ToStr(result)
                            );
            }
        }

        private Matrix3D GetMatrix(int n)
        {
            switch (n)
            {
                case 0: return new Matrix3D();      // should be identity
                case 1: return TranslateMatrix(new Vector3D(1, 1, 1));
                case 2: return TranslateMatrix(new Vector3D(-1, -1, -1));
                case 3: return ScaleMatrix(new Vector3D(2, 2, 2));
                case 4: return ScaleMatrix(new Vector3D(-2, -2, -2));
                case 5: return RotateMatrix(new Vector3D(0, 0, 1), 90);
                case 6: return RotateMatrix(new Vector3D(0, 1, 0), 90);
                case 7: return RotateMatrix(new Vector3D(1, 0, 0), 90);
                case 8: return RotateMatrix(new Vector3D(0, 0, 1), 45);
                case 9: return RotateMatrix(new Vector3D(0, 0, -1), 90);
                case 10: return RotateMatrix(new Vector3D(0, 0, 1), 135);
                case 11: return RotateMatrix(new Vector3D(0, 0, 1), 180);
                case 12: return RotateMatrix(new Vector3D(0, 0, 1), 270);
                case 13: return TranslateMatrix(new Vector3D(double.MaxValue, 0, 0));
                case 14: return TranslateMatrix(new Vector3D(0, -double.MaxValue, 0));
                case 15: return ScaleMatrix(new Vector3D(double.MinValue, 0, 0));
                case 16: return new Matrix3D(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
                case 17: return new Matrix3D(1, -1, 1, -1, -1, 1, -1, 1, 1, -1, 1, -1, -1, 1, -1, 1);
                case 18: return new Matrix3D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                case 19: return new Matrix3D(double.MaxValue, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                case 20: return new Matrix3D(1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1);
                case 21: return new Matrix3D(11, 1, 101, 0, 23, -9, 1, 0, -390, 1, 1, 0, 1, 1, 1, 1);
                case 22: return new Matrix3D(1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1);
                case 23: return new Matrix3D(1, 2, 3, 0, 4, 5, 6, 0, 7, 8, 9, 0, 10, 11, 12, 1);
                case 24: return new Matrix3D(1, 1, 1, 0, 1, 1, 1, 0, double.MinValue, -double.MinValue, 1, 0, 1, double.MinValue, 1, 1);

                /* PS #919596: Need framework stance on how to handle NaN, double.PositiveInfinity, ...

                        Remove or Enable these cases once this decision has been made

                            case 25: return new Matrix3D( double.MinValue, -double.MinValue, 1, 0, 1, double.MinValue, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1 );
                            case 26: return new Matrix3D( double.MinValue, double.MinValue, 1, 0, 1, -double.MinValue, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1 );
                            case 27: return new Matrix3D( double.MaxValue, -double.MinValue, 1, 0, 1, double.MinValue, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1 );
                            case 28: return new Matrix3D( 1, 1, double.MaxValue, 0, -double.MinValue, double.MinValue, 1, 0, 1, 1, 1, 0, 1, 1, 1, 1 );
                            case 29: return new Matrix3D( 40, -99999, double.MinValue, 0, -double.MinValue, 1, double.MinValue, 0, 1, 1, 1, 0, 1, 1, 1, 1 );
                */

                default: throw new ArgumentException("I don't have that many test cases");
            }
        }

        private Matrix3D TranslateMatrix(Vector3D offset)
        {
            return new TranslateTransform3D(offset).Value;
        }

        private Matrix3D RotateMatrix(Vector3D axis, double angle)
        {
            return new RotateTransform3D(new AxisAngleRotation3D(axis, angle)).Value;
        }

        private Matrix3D ScaleMatrix(Vector3D scale)
        {
            return new ScaleTransform3D(scale).Value;
        }
    }
}