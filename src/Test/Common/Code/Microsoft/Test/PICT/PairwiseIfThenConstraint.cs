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

    sealed class PairwiseIfThenConstraint: IPairwiseVisitable, IPairwiseComment
    {
        string comment;
        PairwiseCondition ifp;
        PairwiseCondition thenp;
        PairwiseCondition elsep;

        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value;}
        }

        public PairwiseCondition IfPart
        {
            get { return ifp; } 
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                ifp = value;
            }
        }
        public PairwiseCondition ThenPart
        {
            get { return thenp; }
            set 
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                
                thenp = value;
            }
        }

        public PairwiseCondition ElsePart
        {
            get { return elsep;}
            set
            {
                if (value == null) 
                {
                    throw new ArgumentNullException("value");
                }
                elsep = value;
            }
        }

        #region IPairwiseVisitable Members

#endregion

        public PairwiseIfThenConstraint()
        {
        }

        public PairwiseIfThenConstraint(PairwiseCondition ifPart, PairwiseCondition thenPart)
        {
            this.ifp = ifPart;
            this.thenp = thenPart;
            this.elsep = null;
        }

        public PairwiseIfThenConstraint(PairwiseCondition ifPart, PairwiseCondition thenPart, PairwiseCondition elsePart)
        {
            this.ifp = ifPart;
            this.thenp = thenPart;
            this.elsep = elsePart;
        }


#region IPairwiseVisitable Members

        public object Accept(PairwiseVisitor visitor)
        {
            return visitor.Visit(this);
        }

        #endregion
    }
}
