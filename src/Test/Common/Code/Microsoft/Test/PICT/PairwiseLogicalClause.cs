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

    abstract class PairwiseLogicalClause : PairwiseCondition
    {

        PairwiseCondition first, second;
        PairwiseLogicalRelationship logical;

        public PairwiseCondition First
        {
            get { return this.first; }
        }

        public PairwiseLogicalRelationship LogicalRelationship
        {
            get { return this.logical; }
        }

        public PairwiseCondition Second
        {
            get { return this.second; }
        }

        internal PairwiseLogicalClause(PairwiseLogicalRelationship comparison, PairwiseCondition first, PairwiseCondition second): base()
        {
            if (!Enum.IsDefined(typeof(PairwiseLogicalRelationship), comparison))
            {
                throw new ArgumentOutOfRangeException("comparison", comparison, "Not defined");
            }

            this.first = first;
            this.second = second;
            this.logical = comparison;
        }

        public sealed override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
