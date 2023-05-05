// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// All schema tests should throw this exception on failure.
    /// </summary>
    [Serializable]
    public class SchemaTestFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SchemaTestFailedException class.
        /// </summary>
        public SchemaTestFailedException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaTestFailedException class.
        /// </summary>
        /// <param name="message">Input message.</param>
        public SchemaTestFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaTestFailedException class.
        /// </summary>
        /// <param name="message">Input message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SchemaTestFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SchemaTestFailedException class.
        /// </summary>
        /// <param name="serializationInfo">SerializationInfo object to be passed to base class constructor.</param>
        /// <param name="streamingContext">StreamingContext object to be passed to base class constructor.</param>
        protected SchemaTestFailedException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}
