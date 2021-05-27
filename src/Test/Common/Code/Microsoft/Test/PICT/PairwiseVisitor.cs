// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;

    abstract class PairwiseVisitor
    {
        protected PairwiseVisitor()
        {
        }
        public abstract object Visit(PairwiseLogicalClause clause);
        public abstract object Visit(PairwiseComparisonTerm term);
        public abstract object Visit(PairwiseInTerm term);
        public abstract object Visit(PairwiseNotInTerm term);
        public abstract object Visit(PairwiseLikeTerm term);
        public abstract object Visit(PairwiseNotClause clause);
        [Obsolete]
        public abstract object Visit(PairwiseParameterConstraint constraint);
        public abstract object Visit(PairwiseIfThenConstraint constraint);
        public abstract object Visit(PairwiseSubModel childModel);
        public abstract object Visit(PairwiseModel model);
    }
}
