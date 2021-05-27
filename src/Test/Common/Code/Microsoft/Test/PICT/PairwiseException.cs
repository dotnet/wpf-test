// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Microsoft.Test.Pict
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown for (most) pairwise issues
    /// </summary>
    [Serializable]
    sealed class PairwiseException: ApplicationException
    {
        /// <summary>
        /// Pairwise exception
        /// </summary>
        public PairwiseException(): base()
        {
        }
        /// <summary>
        /// Pairwise exception
        /// </summary>
        /// <param name="message">reason</param>
        public PairwiseException(string message): base(message)
        {
        }
        /// <summary>
        /// Pairwise exception
        /// </summary>
        /// <param name="message">reason</param>
        /// <param name="inner">inner</param>
        public PairwiseException(string message, Exception inner): base(message, inner)
        {
        }
        PairwiseException(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }
    }
}
