// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Pict.MatrixOutput
{
    #region using;
    using System;
    using System.Collections;
    #endregion

    public sealed class PairwiseHelper
    {
        /// <summary>
        /// Genetate PairwiseTestCase array from a pict model file and optional arguments for pict. 
        /// </summary>
        /// <param name="modelFile">model file</param>
        /// <param name="pictArgs">arguments for pict</param>
        /// <returns></returns>
        public static PairwiseTestCase[] GenerateTestsFromFile(string modelFile, string pictArgs)
        {
            PairwiseTestMatrix matrix = PairwiseTestMatrix.GenerateMatrixFromPictFile(modelFile, pictArgs);
            return matrix.PairwiseTestCases;
        }
    }
}
