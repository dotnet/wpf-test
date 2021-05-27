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

    sealed class PairwiseInTerm : PairwiseTerm
    {
        readonly PairwiseValueCollection values;

        public PairwiseValueCollection Values
        {
            get { return this.values; }
            // set { this.values = Check(value);}
        }

        public PairwiseInTerm(PairwiseParameter parameter, PairwiseValueCollection values): base(parameter)
        {
            this.values = new PairwiseValueCollection(Check(values));
        }

        public override object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }

        PairwiseValueCollection Check(PairwiseValueCollection coll)
        {
            if (coll == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (coll.Count == 0)
            {
                throw new ArgumentException("No items in collection");
            }

            if (!this.Parameter.IsCompatibleWith(coll[0]))
            {
                throw new PairwiseException("Inconsistent value types: " + coll[0].ValueType + " != " + this.Parameter.PairwiseValueType + " (expected for " + this.Parameter.Name + ")");
            }

            return coll;
        }
    }
}
