// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common.TestObjects.Xaml.GraphCore
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Graph operation deleage
    /// </summary>
    /// <param name="node">node to operate on</param>
    /// <param name="state">custom state</param>
    public delegate void GraphNodeOperation(IGraphNode node, object state);

    /// <summary>
    /// Transform operation delegate
    /// </summary>
    /// <param name="node">root node to transform</param>
    /// <param name="state">custom state</param>
    /// <returns>transformed node</returns>
    public delegate IGraphNode TransformOperation(IGraphNode node, object state);

    /// <summary>
    /// Graph node class
    /// </summary>
    [Serializable]
    public abstract class GraphNode : TestDependencyObject, IGraphNode
    {
        /// <summary>
        /// Child nodes of this graph node
        /// </summary>
        private readonly IList<IGraphNode> _children;

        /// <summary>
        /// Initializes a new instance of the GraphNode class
        /// </summary>
        protected GraphNode()
            : this("NoName", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphNode class
        /// </summary>
        /// <param name="name">name of the node</param>
        protected GraphNode(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the GraphNode class
        /// </summary>
        /// <param name="name">name of the node</param>
        /// <param name="parent">parent node</param>
        protected GraphNode(string name, IGraphNode parent)
        {
            this.Name = name;
            this.Parent = parent;
            _children = new List<IGraphNode>();
        }

        /// <summary>
        /// Gets the Children property
        /// </summary>
        public IList<IGraphNode> Children
        {
            get
            {
                return _children;
            }
        }

        /// <summary>
        /// Gets or sets the parent property
        /// </summary>
        public IGraphNode Parent { get; set; }
        
        /// <summary>
        /// Gets or sets the Name property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the depth of this node from the root
        /// </summary>
        public int Depth
        {
            get
            {
                // Walk until root and give the depth //
                IGraphNode node = this;
                int depth = 0;
                while (node.Parent != null)
                {
                    depth++;
                    node = node.Parent;
                }

                return depth;
            }
        }

        /// <summary>
        /// Gets the QualifiedName property
        /// </summary>
        public string QualifiedName
        {
            get
            {
                // Trace until root and generate a qualified name //
                IGraphNode node = this;
                string name = this.Name;
                while (node.Parent != null)
                {
                    name = node.Parent.Name + "_" + name;
                    node = node.Parent;
                }

                return name;
            }
        }

        /// <summary>
        ///  Gets the list of all nodes that are reachable through 
        ///  the children links - inclides this node as well 
        /// </summary>
        public List<IGraphNode> Descendants
        {
            get
            {
                List<IGraphNode> visited = new List<IGraphNode>();
                this.BreadthFirstSearchOperation(new GraphNodeOperation(VisitNode), visited);

                return visited;
            }
        }

        #region Graph Operations

        /// <summary>
        /// Walk the graph like it was a tree from the root - if you encounter a cycle, stop further walking
        /// and continue with the next node
        /// Perform an operation on each node, the final result being a new tree
        /// </summary>
        /// <param name="operation">
        /// operation should return a new reference (don't just return the node passed in) with an empty Children collection
        /// </param>
        /// <param name="state">custom state</param>
        /// <returns>transormed graph node</returns>
        public IGraphNode Transform(TransformOperation operation, object state)
        {
            Stack<TransformWalkInfo> frames = new Stack<TransformWalkInfo>();
            TransformWalkInfo rootInfo = new TransformWalkInfo
                                             {
                                                 CurrentNode = this,
                                                 TransformedNode = operation(this, state)
                                             };
            frames.Push(rootInfo);
            IGraphNode root = rootInfo.TransformedNode;
            Dictionary<IGraphNode, IGraphNode> visitedNodes = new Dictionary<IGraphNode, IGraphNode>();

            while (frames.Count > 0)
            {
                TransformWalkInfo frame = frames.Pop();

                if (visitedNodes.ContainsKey(frame.CurrentNode))
                {
                    continue;
                }
                else
                {
                    visitedNodes[frame.CurrentNode] = frame.TransformedNode;
                }

                for (int i = 0; i < frame.CurrentNode.Children.Count; i++)
                {
                    IGraphNode current = frame.CurrentNode.Children[i];
                    IGraphNode transformed = operation(current, state);
                    frame.TransformedNode.Children.Add(transformed);
                    frames.Push(new TransformWalkInfo
                                    {
                                        CurrentNode = current,
                                        TransformedNode = transformed
                                    });
                }
            }

            return root;
        }

        /// <summary>
        /// Walk the graph like it was a tree from the root - if you encounter a cycle, stop further walking
        /// and continue with the next node
        /// </summary>
        /// <param name="operation">operation to perform</param>
        /// <param name="state">custom state</param>
        public void DepthFirstSearchOperation(GraphNodeOperation operation, object state)
        {
            DepthFirstSearchOperation(operation, null, state);
        }

        /// <summary>
        /// Perform a depth first search operation
        /// </summary>
        /// <param name="preVisit">operation to preform before each node visit</param>
        /// <param name="postVisit">operation to perform after each node visit</param>
        /// <param name="state">custom state</param>
        public void DepthFirstSearchOperation(GraphNodeOperation preVisit, GraphNodeOperation postVisit, object state)
        {
            // nothing happens unless one operation is provided
            if (preVisit == null && postVisit == null)
            {
                return;
            }

            // we must be able to distinguish these for the algorithm below to work
            if (preVisit == postVisit)
            {
                throw new InvalidOperationException("Operations must be different.");
            }

            List<IGraphNode> visitedNodes = new List<IGraphNode>();
            Stack<DepthFirstWalkInfo> frames = new Stack<DepthFirstWalkInfo>();
            frames.Push(new DepthFirstWalkInfo
                            {
                                CurrentNode = this,
                                Operation = preVisit
                            });

            while (frames.Count > 0)
            {
                DepthFirstWalkInfo current = frames.Pop();

                if (visitedNodes.Contains(current.CurrentNode))
                {
                    if (current.Operation == preVisit)
                    {
                        continue;
                    }
                }
                else
                {
                    visitedNodes.Add(current.CurrentNode);
                }

                if (current.Operation != null)
                {
                    current.Operation(current.CurrentNode, state);
                }

                if (current.Operation != postVisit)
                {
                    frames.Push(new DepthFirstWalkInfo
                                    {
                                        CurrentNode = current.CurrentNode,
                                        Operation = postVisit
                                    });

                    // this goes into the stack in reverse order of what we need, 
                    // put onto one stack then another to reverse it
                    Stack<DepthFirstWalkInfo> tempStack = new Stack<DepthFirstWalkInfo>();
                    foreach (IGraphNode child in current.CurrentNode.Children)
                    {
                        tempStack.Push(new DepthFirstWalkInfo
                                           {
                                               CurrentNode = child,
                                               Operation = preVisit
                                           });
                    }

                    while (tempStack.Count > 0)
                    {
                        frames.Push(tempStack.Pop());
                    }
                }
            }
        }

        /// <summary>
        /// Walk the graph like it was a tree from the root - if you encounter a cycle, stop further walking
        /// and continue with the next node
        /// </summary>
        /// <param name="operation">opeartion to perform</param>
        /// <param name="state">custom state</param>
        public void BreadthFirstSearchOperation(GraphNodeOperation operation, object state)
        {
            Queue<IGraphNode> pendingNodes = new Queue<IGraphNode>();
            pendingNodes.Enqueue(this);
            List<IGraphNode> visitedNodes = new List<IGraphNode>();

            while (pendingNodes.Count != 0)
            {
                IGraphNode node = pendingNodes.Dequeue();
                if (visitedNodes.Contains(node))
                {
                    // cycle - already seen this node //
                    continue;
                }
                else
                {
                    visitedNodes.Add(node);
                }

                operation(node, state);

                foreach (IGraphNode child in node.Children)
                {
                    pendingNodes.Enqueue(child);
                }
            }
        }

        /// <summary>
        /// Visit the given node
        /// </summary>
        /// <param name="node">node to visit</param>
        /// <param name="state">custom state</param>
        private static void VisitNode(IGraphNode node, object state)
        {
            List<IGraphNode> visited = state as List<IGraphNode>;
            if (visited.Contains(node))
            {
                return;
            }
            else
            {
                visited.Add(node);
            }
        }

        /// <summary>
        /// Content holder during depth first walk
        /// </summary>
        private class DepthFirstWalkInfo
        {
            /// <summary>
            /// Gets or sets the CurrentNode property
            /// </summary>
            public IGraphNode CurrentNode { get; set; }

            /// <summary>
            /// Gets or sets the Operation property 
            /// </summary>
            public GraphNodeOperation Operation { get; set; }
        }

        /// <summary>
        /// Transofmr walk information
        /// </summary>
        private class TransformWalkInfo
        {
            /// <summary>
            /// Gets or sets the current node
            /// </summary>
            public IGraphNode CurrentNode { get; set; }

            /// <summary>
            /// Gets or sets the transformed node
            /// </summary>
            public IGraphNode TransformedNode { get; set; }
        }

        #endregion
    }
}
