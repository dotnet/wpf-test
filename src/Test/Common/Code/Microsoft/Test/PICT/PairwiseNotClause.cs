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

    sealed class PairwiseNotClause : PairwiseCondition
    {
        readonly PairwiseCondition cond;

        public PairwiseCondition Condition
        {
            get { return this.cond; }
            // set { this.cond = value;}
        }

        public PairwiseNotClause(PairwiseCondition condition)
        {
            this.cond = condition;
        }

        public override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
