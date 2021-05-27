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

    sealed class PairwiseLessThanOrEqualTerm : PairwiseComparisonTerm
    {
        public PairwiseLessThanOrEqualTerm(PairwiseParameter parameter, PairwiseValue value): base(PairwiseComparison.LessThanOrEqual, parameter, null, value)
        {
        }

        public PairwiseLessThanOrEqualTerm(PairwiseParameter parameter, PairwiseParameter right): base(PairwiseComparison.LessThanOrEqual, parameter, right, null)
        {
        }
    }
}
