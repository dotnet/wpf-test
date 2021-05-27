// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

﻿
namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Provides a base class for the functionality that all parameters must implement.
    /// </summary>
    public abstract class ParameterBase
    {
        /// <summary>
        /// Initializes a new instance of the base class using the specified name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        public ParameterBase(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The number of values in this parameter.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Returns the value at the given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The item.</returns>
        public abstract object GetAt(int index);
    }
}
