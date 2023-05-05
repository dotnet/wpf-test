// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///  Exception thrown for data test failures
    /// </summary>
    public class DataTestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the DataTestException class
        /// </summary>
        /// <param name="message">The exception message</param>
        public DataTestException(string message)
            : base(message)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the DataTestException class
        /// </summary>
        public DataTestException() : base() 
        { 
        }

        /// <summary>
        /// Initializes a new instance of the DataTestException class
        /// </summary>
        /// <param name="message">the exception message</param>
        /// <param name="innerEx">the inner exception</param>
        public DataTestException(string message, Exception innerEx) : base(message, innerEx) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the DataTestException class
        /// </summary>
        /// <param name="info">Serialization infromation</param>
        /// <param name="context">streaming context</param>
        protected DataTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
