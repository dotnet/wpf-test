// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Comparers
{
    /// <summary>
    /// Results of graph compare operation
    /// </summary>
    [Serializable]
    public class GraphCompareResults
    {
        /// <summary>
        /// Errors backing field
        /// </summary>
        private List<CompareError> _errors = new List<CompareError>();

        /// <summary>
        /// Initializes a new instance of the GraphCompareResults class
        /// </summary>
        public GraphCompareResults()
        {
            this.ResultGraph = null;
            this.Passed = false;
        }

        /// <summary>
        /// Gets the Errors property
        /// </summary>
        public List<CompareError> Errors
        {
            get
            {
                return this._errors;
            }
        }

        /// <summary>
        /// Gets or sets the ResultGraph property
        /// </summary>
        public ObjectGraph ResultGraph { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the compare passed
        /// </summary>
        public bool Passed { get; set; }
    }
}
