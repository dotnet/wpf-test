// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;

    /// <summary>
    /// Interface to build an object graph
    /// </summary>
    public interface IXamlObjectGraphBuilder
    {
        /// <summary>
        /// Build a node
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="data">data content of the node</param>
        /// <param name="type">type of the data</param>
        /// <param name="parent">parent node info</param>
        /// <param name="propertyName">name of the property</param>
        /// <param name="isReadOnly">is the node read only</param>
        /// <returns>The built node</returns>
        IGraphNode BuildNode(string name, object data, Type type, IGraphNode parent, string propertyName, bool isReadOnly);

        /// <summary>
        /// Build a visited node
        /// </summary>
        /// <param name="builtNode">the built node</param>
        /// <param name="parent">parent node info</param>
        /// <returns>List of nodes</returns>
        IList<IGraphNode> BuildVisitedNode(IGraphNode builtNode, IGraphNode parent);

        /// <summary>
        /// Wrap collection node
        /// </summary>
        /// <param name="parent">parent node info</param>
        /// <returns>wrapped graph node</returns>
        IGraphNode BuildCollectionWrapperNode(IGraphNode parent);
    }
}
