// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphOperations.Builders
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore;

    /// <summary>
    /// Xaml object graph builder
    /// </summary>
    public class XamlObjectGraphBuilder : IXamlObjectGraphBuilder
    {
        /// <summary>
        /// Initializes a new instance of the XamlObjectGraphBuilder class
        /// </summary>
        public XamlObjectGraphBuilder()
        {
        }

        /// <summary>
        /// Build a single graph node
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="data">data to be heldin the node</param>
        /// <param name="type">type of the data</param>
        /// <param name="parent">parent of this nod</param>
        /// <param name="propertyName">name of this property</param>
        /// <param name="isReadOnly">is this readonly</param>
        /// <returns>Built IGraphNode instance</returns>
        public IGraphNode BuildNode(string name, object data, Type type, IGraphNode parent, string propertyName, bool isReadOnly)
        {
            return new ObjectGraph(name, data, type, parent);
        }

        /// <summary>
        /// Build visited node
        /// </summary>
        /// <param name="builtNode">the built node</param>
        /// <param name="parent">parent to this node</param>
        /// <returns>list of graph nodes</returns>
        public IList<IGraphNode> BuildVisitedNode(IGraphNode builtNode, IGraphNode parent)
        {
            return new List<IGraphNode>
                       {
                           builtNode
                       };
        }

        /// <summary>
        /// Build a wrapper for collections
        /// </summary>
        /// <param name="parent">collection parent</param>
        /// <returns>graph node wrapping collection</returns>
        public IGraphNode BuildCollectionWrapperNode(IGraphNode parent)
        {
            //// no collection wrapper
            return null;
        }
    }
}
