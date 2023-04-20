// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.Xaml.Serialization
{
    /// <summary>
    /// Interface for implementing a Xaml Serializer wrapper class
    /// </summary>
    public interface IXamlTestSerializer
    {
        /// <summary>
        /// Writes the object tree to the specified xaml file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="treeRoot">The tree root.</param>
        void SerializeObjectTree(string fileName, object treeRoot);
    }
}
