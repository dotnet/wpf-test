// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    #region using;
    using System;
    using System.Text;
    using System.Xml;
    using System.Collections;
    #endregion

    sealed class PairwiseEqualTerm : PairwiseComparisonTerm
    {
        public PairwiseEqualTerm(PairwiseParameter parameter, PairwiseValue value): base(PairwiseComparison.Equal, parameter, null, value)
        {
        }

        public PairwiseEqualTerm(PairwiseParameter parameter, PairwiseParameter right): base(PairwiseComparison.Equal, parameter, right, null)
        {
        }
    }
}
