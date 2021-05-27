// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;

    /// <summary>
    /// Something which can be visited; returns visitor.Visit(this)
    /// </summary>
    interface IPairwiseVisitable
    {
        /// <summary>
        /// Something which can be visited; returns visitor.Visit(this)
        /// </summary>
        /// <param name="visitor">the visitor</param>
        /// <returns>the results of visitor.Visit(this)</returns>
        object Accept(PairwiseVisitor visitor);
    }
}
