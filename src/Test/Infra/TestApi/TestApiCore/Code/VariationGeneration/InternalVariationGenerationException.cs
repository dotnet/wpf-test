// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
using System;
using System.Runtime.Serialization;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Indicates an error in the generated internal variation table prevents completion of generation.
    /// This indicates a 

    [Serializable]
    internal class InternalVariationGenerationException : Exception
    {
        public InternalVariationGenerationException()
            : base()
        {
        }

        public InternalVariationGenerationException(string message)
            : base(message)
        {
        }

        protected InternalVariationGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
