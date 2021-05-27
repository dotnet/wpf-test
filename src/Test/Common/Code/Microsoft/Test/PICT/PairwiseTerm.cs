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

    abstract class PairwiseTerm : PairwiseCondition
    {
        readonly PairwiseParameter parameter;

        public PairwiseParameter Parameter
        {
            get { return this.parameter; }
        }

        internal PairwiseTerm(PairwiseParameter parameter)
        {
            this.parameter = parameter;
        }
    }
}
