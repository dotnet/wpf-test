// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;

    sealed class PairwiseSubModel: IPairwiseVisitable, IPairwiseComment
    {
        string comment;
        int order;
        readonly PairwiseParameterCollection parameters = new PairwiseParameterCollection();

        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                this.comment = value;
            }
        }

        public int SubModelOrder
        {
            get 
            {
                return order;
            }
            
            set
            {
                if (value < 2 || value > parameters.Count)
                {
                    throw new ArgumentOutOfRangeException("value", value, "Too big or too small");
                }
                order = value;
            }
        }

        public PairwiseParameterCollection SubModelParameters
        {
            get
            {
                return this.parameters;
            }
        }
        public PairwiseSubModel()
        {
        }

        public PairwiseSubModel(int order, params PairwiseParameter[] parameters)
        {
            this.order = order;
            this.parameters = new PairwiseParameterCollection(parameters);
        }

        public object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}
