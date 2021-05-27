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

    abstract class PairwiseCondition : IPairwiseVisitable, IPairwiseComment
    {

        string comment;

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
            }
        }
        internal PairwiseCondition()
        {
        }

        public abstract object Accept(PairwiseVisitor visitor);

        public virtual PairwiseAndClause And(PairwiseCondition c)
        {
            return new PairwiseAndClause(this, c);
        }

        public virtual PairwiseOrClause Or(PairwiseCondition c)
        {
            return new PairwiseOrClause(this, c);
        }

        public PairwiseIfThenConstraint Then(PairwiseCondition thenPart)
        {
            return new PairwiseIfThenConstraint(this, thenPart);
        }
    }
}
