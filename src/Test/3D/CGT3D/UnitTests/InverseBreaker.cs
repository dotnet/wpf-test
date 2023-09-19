// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;

#if !STANDALONE_BUILD
using TrustedStreamWriter = Microsoft.Test.Security.Wrappers.StreamWriterSW;
#else
using TrustedStreamWriter = System.IO.StreamWriter;
#endif

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Find matrices that break Matrix3D.Inverse
    /// </summary>
    public class InverseBreaker : CoreGraphicsTest
    {
        private const double epsilon = 0.0000001;

        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            _random = new Random((int)DateTime.Now.Ticks);
            string filename = v["log"];

            if (filename == null)
            {
                throw new ArgumentException("You must specify a file to log to: /log=<filename>");
            }

            _logFile = new TrustedStreamWriter(filename);
        }

        /// <summary/>
        public override void RunTheTest()
        {
            while (Failures < 10)
            {
                TestMatrix(MatrixUtils.GenerateMatrix());
            }

            _logFile.Close();
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
            }
            catch (InvalidOperationException ex)
            {
                hasInverse = false;
                inverse = default(Matrix3D);

                if (m.HasInverse)
                {
                    ToLogs("Matrix does not have an inverse, but property HasInverse is true.\r\n" +
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
                    ToLogs("Determinant is not 0; this matrix should have an inverse:\r\n" +
                            MatrixUtils.ToStr(m) +
                            ex.ToString()
                            );
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
                ToLogs("Could not multiply:\r\n" +
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
                ToLogs("Original * Inverse should be the Identity matrix.\r\n" +
                        "Original Matrix:\r\n" +
                        MatrixUtils.ToStr(m) +
                        "\r\nInverse:\r\n" +
                        MatrixUtils.ToStr(m_inv) +
                        "\r\nResult:\r\n" +
                        MatrixUtils.ToStr(result)
                        );
            }
        }

        private void ToLogs(string s)
        {
            AddFailure(s);
            _logFile.Write("--------------------------------------------------------------------------------------------------\r\n" +
                           s
                           );
        }

        private Random _random;
        private TrustedStreamWriter _logFile;
    }
}