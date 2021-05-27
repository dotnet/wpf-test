// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;

    // LATER: remove this??
    [Obsolete("Use 'normal' terms for this", false)]
    sealed class PairwiseParameterConstraint: PairwiseTerm, IPairwiseVisitable
    {
        readonly PairwiseComparison op;
        readonly PairwiseParameter second;

        public PairwiseComparison Comparison
        {
            get { return this.op; }
            // set { this.op = value;}
        }

        public PairwiseParameter First
        {
            get { return this.Parameter; }
            // set { this.first = value;}
        }

        public PairwiseParameter Second
        {
            get { return this.second; }
            // set { this.second = value;}
        }

        public PairwiseParameterConstraint(PairwiseParameter first, PairwiseComparison op, PairwiseParameter second): base(first)
        {
            this.op = op;
            this.second = second;

            // only allow between the same strong-ish-types
            if (! this.First.IsCompatibleWith(second))
            {
                throw new ArgumentException(string.Format("{0} != {1}", this.First.PairwiseValueType, this.second.PairwiseValueType));
            }

            bool isrelative = ! (op == PairwiseComparison.Equal || op == PairwiseComparison.NotEqual);
            PairwiseValueType t = this.First.PairwiseValueType;
            if (isrelative)
            {
                if (t == PairwiseValueType.Enum || t == PairwiseValueType.ComplexObject)
                {
                    throw new NotSupportedException("Not supported: " + op + " on " + t + "; might be possible, though!");
                }
            }
        }

        public override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }

    }
}
