// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers
{
    /// <summary>
    /// Stores the error information from a Compare operation
    /// </summary>
    [Serializable]
    public class CompareError
    {
        /// <summary>
        /// Initializes a new instance of the CompareError class
        /// </summary>
        /// <param name="node1">first node that is compared</param>
        /// <param name="node2">second node that is compared</param>
        /// <param name="exception">the comparison exception</param>
        public CompareError(ObjectGraph node1, ObjectGraph node2, Exception exception)
        {
            this.Node1 = node1;
            this.Node2 = node2;
            this.Error = exception;
        }

        /// <summary>
        /// Gets or sets the Node1 property
        /// </summary>
        public ObjectGraph Node1 { get; set; }

        /// <summary>
        /// Gets or sets the Node2 property
        /// </summary>
        public ObjectGraph Node2 { get; set; }
        
        /// <summary>
        /// Gets or sets the Error property
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Get the compare error property
        /// </summary>
        /// <param name="dependencyObject">The dependency object instance</param>
        /// <returns>the compare error property</returns>
        public static CompareError GetCompareError(TestDependencyObject dependencyObject)
        {
            return (CompareError)dependencyObject.GetValue(ObjectGraphComparer.CompareErrorProperty);
        }

        /// <summary>
        /// Get the string rep
        /// </summary>
        /// <returns>string rep</returns>
        public override string ToString()
        {
            string mesg = string.Format(
                                        CultureInfo.InvariantCulture, 
                                        "Compare Error: {0}; Node1:{1}; Node2:{2}", 
                                        Error.Message,
                                        Node1,
                                        Node2);
            return mesg;
        }      
    }
}
