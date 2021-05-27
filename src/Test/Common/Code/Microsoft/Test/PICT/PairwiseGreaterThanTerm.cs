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

    sealed class PairwiseGreaterThanTerm : PairwiseComparisonTerm
    {
        public PairwiseGreaterThanTerm(PairwiseParameter parameter, PairwiseValue value): base(PairwiseComparison.GreaterThan, parameter, null, value)
        {
        }

        public PairwiseGreaterThanTerm(PairwiseParameter parameter, PairwiseParameter right): base(PairwiseComparison.GreaterThan, parameter, right, null)
        {
        }
    }
}
