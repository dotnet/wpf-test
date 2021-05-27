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

    sealed class PairwiseLikeTerm : PairwiseTerm
    {
        string wildcardPattern;

        public string WildcardPattern
        {
            get { return this.wildcardPattern; }
        }

        public PairwiseLikeTerm(PairwiseParameter parameter, string wildcardPattern): base(parameter)
        {
            if (wildcardPattern == null)
            {
                throw new ArgumentNullException("wildcardPattern");
            }

            // NOTE: theoretically, you could use "Like" with enums, but that fails when flags are involved
            if (parameter.PairwiseValueType != PairwiseValueType.String)
            {
                throw new ArgumentOutOfRangeException("parameter", parameter.PairwiseValueType, "Non-string value type given to Like operator");
            }

            this.wildcardPattern = wildcardPattern;
        }

        public override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
