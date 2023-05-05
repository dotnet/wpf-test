// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a graph node
    /// </summary>
    public interface IGraphNode : ITestDependencyObject
    {
        /// <summary>
        /// Gets the children nodes
        /// </summary>
        IList<IGraphNode> Children { get; }

        /// <summary>
        /// Gets or sets the Parent node
        /// </summary>
        IGraphNode Parent { get; set; }

        /// <summary>
        /// Gets or sets the Name of the node
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the qualified name of the node
        /// </summary>
        string QualifiedName { get; }

        /// <summary>
        /// Depth first search
        /// </summary>
        /// <param name="operation">operation to perform on node</param>
        /// <param name="state">custom state</param>
        void DepthFirstSearchOperation(GraphNodeOperation operation, object state);

        /// <summary>
        /// Depth first search
        /// </summary>
        /// <param name="preVisit">operation to perform before node visit</param>
        /// <param name="postVisit">operation to perform after node visit</param>
        /// <param name="state">custom state</param>
        void DepthFirstSearchOperation(GraphNodeOperation preVisit, GraphNodeOperation postVisit, object state);

        /// <summary>
        /// Breadth first search
        /// </summary>
        /// <param name="operation">operation to perform on node</param>
        /// <param name="state">custom state</param>
        void BreadthFirstSearchOperation(GraphNodeOperation operation, object state);

        /// <summary>
        /// peform a transform on the graph
        /// </summary>
        /// <param name="operation">opeation to perform on each node</param>
        /// <param name="state">custom state</param>
        /// <returns>the transformed graph</returns>
        IGraphNode Transform(TransformOperation operation, object state);
    }
}
