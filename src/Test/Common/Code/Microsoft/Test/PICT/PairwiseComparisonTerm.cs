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

    abstract class PairwiseComparisonTerm : PairwiseTerm
    {
        readonly PairwiseComparison rel;

        readonly PairwiseParameter rhsParameter;

        readonly PairwiseValue rhsValue;

        public PairwiseComparison Comparison
        {
            get { return this.rel; }
        }

        [Obsolete("Call GetRightValue() after checking against HasRightValue()")]
        public PairwiseValue Value
        {
            get { return GetRightValue(); }
        }

        internal PairwiseComparisonTerm(PairwiseComparison comparison, PairwiseParameter left, PairwiseParameter rightParam, PairwiseValue rightVal): base(left)
        {
            if (rightVal == null && rightParam == null)
            {
                throw new ArgumentException("Can't have both val and param == null!");
            }

            if (rightVal != null && rightParam != null)
            {
                throw new ArgumentException("Can't have both val and param non-null!");
            }

            if (left == rightParam)
            {
                throw new ArgumentException("Can't have the same two parameters in the same comparison");
            }

            this.rel = comparison;
            this.rhsValue = rightVal;
            this.rhsParameter = rightParam;
            if (rightVal != null && !this.Parameter.IsCompatibleWith(rightVal))
            {
                throw new ArgumentException(string.Format("Mismatched types: {0} != {1}", left.PairwiseValueType, rightVal.PairwiseValueType));
            }

            if (rightParam != null && !this.Parameter.IsCompatibleWith(rightParam))
            {
                throw new ArgumentException(string.Format("Mismatched types: {0} != {1}", left.PairwiseValueType, rightParam.PairwiseValueType));
            }

            if (!this.Parameter.IsCompatibleWith(rel))
            {
                throw new NotSupportedException("Not supported: " + rel + " on " + this.Parameter + "; might be possible, though!");
            }
        }

        public sealed override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }

        public PairwiseParameter GetRightParameter()
        {
            if (HasRightValue())
            {
                throw new InvalidOperationException("Uses value");
            }

            return this.rhsParameter;
        }

        public PairwiseValue GetRightValue()
        {
            if (!HasRightValue())
            {
                throw new InvalidOperationException("Uses parameter");
            }

            return this.rhsValue;
        }

        public bool HasRightValue()
        {
            return this.rhsValue != null;
        }
    }
}
