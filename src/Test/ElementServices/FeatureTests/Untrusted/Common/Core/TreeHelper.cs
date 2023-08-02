// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts framework-level tree operations.  The intention
 *          of abstracting the operations is to reduce the effect
 *          of future API changes in the product's core level.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Media.Animation;

using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Abstraction of framework-level tree operations.
    /// </summary>
    public class TreeHelper
    {
        /// <summary>
        /// Waits for the next tick of the TimeManager.
        /// </summary>
        /// <remarks>
        /// Storyboards that are triggered by events do not begin until 
        /// the first following tick of the TimeManager.
        /// </remarks>
        public static void WaitForTimeManager()
        {
            lock (s_syncObject)
            {
                ParallelTimeline timeline = new ParallelTimeline();
                timeline.BeginTime = TimeSpan.FromMilliseconds(0);
                timeline.Duration = new Duration(TimeSpan.FromMilliseconds(0));
                timeline.CurrentStateInvalidated += new EventHandler(_OnCurrentStateInvalidated);
                Clock tlc = timeline.CreateClock();

                s_timelineStateInvalidated = false;

                while (!s_timelineStateInvalidated)
                {
                    DispatcherHelper.DoEvents(1);
                }
            }
        }

        // Resets the timeline state flag.
        private static void _OnCurrentStateInvalidated(object sender, EventArgs e)
        {
            s_timelineStateInvalidated = true;
        }

        /// <summary>
        /// Gets the Name of a node.
        /// </summary>
        public static string GetNodeId(DependencyObject node)
        {
            string name = null;

            // Get the name from the node's Name property if possible
            // or from the list of registered names.
            if (node is IFrameworkInputElement)
            {
                name = ((IFrameworkInputElement)node).Name;
            }
            else if (s_registeredNames.ContainsKey(node))
            {
                name = s_registeredNames[node];
            }

            // If there was a Name property, verify it was a registered.
            if (!String.IsNullOrEmpty(name) && (node is FrameworkElement || node is FrameworkContentElement))
            {
                object registeredNode = null;
                if (node is FrameworkElement)
                {
                    registeredNode = ((FrameworkElement)node).FindName(name);
                }
                else // assume node is FrameworkContentElement
                {
                    registeredNode = ((FrameworkContentElement)node).FindName(name);
                }

                if (registeredNode != node)
                {
                    throw new InvalidOperationException("The node's Name '" + name + "' is registered for a different object.");
                }
            }

            return name;
        }

        /// <summary>
        /// Finds a tree node by its runtime identifier value.
        /// </summary>
        /// <param name="subtreeRoot">Another node in the same tree.</param>
        /// <param name="id">A runtime identifier value.</param>
        public static object FindNodeById(DependencyObject subtreeRoot, string id)
        {
            if (subtreeRoot == null)
            {
                throw new ArgumentNullException("subtreeRoot");
            }

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            object foundNode = null;

            List<INameScope> scopes = _GetNameScopes(subtreeRoot, true);

            foreach (INameScope parentNode in scopes)
            {
                foundNode = ((INameScope)parentNode).FindName(id);

                // Break loop if the named node is found.
                if (foundNode != null)
                    break;
            }

            return foundNode;
        }

        /// <summary>
        /// Set the Name property of the node.  Also, registers it
        /// with the current name scope.
        /// </summary>
        public static void SetName(string name, DependencyObject node)
        {
            // Verify the name and node are not registered already.
            if (s_registeredNames.ContainsValue(name))
            {
                throw new ArgumentException("name", "Name '" + name + "' is already registered in the current name scope.");
            }

            if (s_registeredNames.ContainsKey(node))
            {
                throw new ArgumentException("node", "The node is already registered in the current name scope.");
            }

            //
            // Register the name and store it in our list of registered names.
            //
            INameScope nameScope = TreeHelper.EnsureNameScope(node);
            nameScope.RegisterName(name, node);

            s_registeredNames[node] = name;

            // Set the Name property if possible.
            if (node is IFrameworkInputElement)
            {
                ((IFrameworkInputElement)node).Name = name;
            }
        }

        /// <summary>
        /// Finds the current Name scope and registers the given object with it.
        /// </summary>
        public static void RegisterName(string id, DependencyObject node)
        {
            INameScope nameScope = TreeHelper.EnsureNameScope(node);

            nameScope.RegisterName(id, node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public static INameScope EnsureNameScope(DependencyObject node)
        {
            DependencyObject walkerNode = null;
            DependencyObject parentNode = node;

            // Just return the current name scope if it exists.
            INameScope nameScope = TreeHelper.GetNameScope(node);
            if (nameScope != null)
            {
                return nameScope;
            }

            // Walk up ancestor chain until we reach the root.
            while (parentNode != null)
            {
                walkerNode = parentNode;
                parentNode = LogicalTreeHelper.GetParent(parentNode);
            }

            // Set new name scope on the root.
            NameScope newNameScope = new NameScope();
            NameScope.SetNameScope(walkerNode, newNameScope);

            return nameScope;
        }

        /// <summary>
        /// Returns the current Name scope - the closest in the ancestor chain.
        /// </summary>
        public static INameScope GetNameScope(DependencyObject subtreeRoot)
        {
            DependencyObject parentNode = subtreeRoot;
            INameScope nameScope = null;

            // Walk up ancestor chain until we find an INameScope.
            // The current node could be an INameScope itself or have an attached INameScope.
            while (parentNode != null)
            {
                if (parentNode is INameScope)
                {
                    nameScope = (INameScope)parentNode;
                }
                else
                {
                    nameScope = NameScope.GetNameScope(parentNode);
                }

                if (nameScope != null)
                    break;

                parentNode = LogicalTreeHelper.GetParent(parentNode);
            }

            return nameScope;
        }

        // Returns all name scopes, either the closest in the ancestor chain or all in the tree.
        private static List<INameScope> _GetNameScopes(DependencyObject subtreeRoot, bool getAllScopesInTree)
        {
            DependencyObject parentNode = subtreeRoot;
            List<INameScope> scopes = null;

            if (!getAllScopesInTree)
            {
                // Walk up ancestor chain until we find a INameScope
                while (parentNode != null && !(parentNode is INameScope))
                {
                    parentNode = LogicalTreeHelper.GetParent(parentNode);
                }

                scopes = new List<INameScope>();

                if (parentNode != null)
                {
                    scopes.Add((INameScope)parentNode);
                }
            }
            else
            {
                DependencyObject walkerNode = null;

                // Walk up ancestor chain until we reach the root.
                while (parentNode != null)
                {
                    walkerNode = parentNode;
                    parentNode = LogicalTreeHelper.GetParent(parentNode);
                }

                scopes = _GetNestedNameScopes(walkerNode);
            }

            return scopes;
        }

        // Returns all namescopes in the given subtree. 
        // Ancestors, siblings, etc. of the given subtree root are ignored.
        private static List<INameScope> _GetNestedNameScopes(DependencyObject subtreeRoot)
        {
            List<INameScope> scopes = new List<INameScope>();

            // Return immediately if the root is null.
            if (subtreeRoot == null)
            {
                return scopes;

            }

            // Check parent.
            INameScope nameScope = null;
            if (subtreeRoot is INameScope)
            {
                nameScope = (INameScope)subtreeRoot;
            }
            else
            {
                nameScope = NameScope.GetNameScope(subtreeRoot);
            }

            if (nameScope != null)
            {
                scopes.Add((INameScope)nameScope);
            }

            // Check children.
            List<DependencyObject> children = GetChildren(subtreeRoot);

            for (int i = 0; i < children.Count; i++)
            {
                List<INameScope> subScopes = _GetNestedNameScopes(children[i]);
                scopes.AddRange(subScopes);
            }

            return scopes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subtreeRoot"></param>
        /// <returns></returns>
        public static List<DependencyObject> GetChildren(DependencyObject subtreeRoot)
        {
            if (subtreeRoot == null)
            {
                throw new ArgumentNullException("subtreeRoot");
            }

            DependencyObject childNode = null;
            List<DependencyObject> children = new List<DependencyObject>();

            // Check children.
            IEnumerator childEnumerator = null;
            try
            {
                childEnumerator = LogicalTreeHelper.GetChildren(subtreeRoot).GetEnumerator();
            }
            catch (NotImplementedException)
            {
                // Can't reach them. Skip silently.
            }

            // If we can enumerate, check the children.
            if (childEnumerator != null)
            {
                childEnumerator.Reset();
                while (childEnumerator.MoveNext() == true)
                {
                    childNode = childEnumerator.Current as DependencyObject;
                    if (childNode != null)
                    {
                        children.Add(childNode);
                    }
                }
            }

            return children;
        }

        /// <summary>
        ///	Given a DependencyObject tree node, searches all its descendent nodes in 
        /// the logical tree for nodes having a non-empty Id. Puts all those nodes in 
        /// the provided hashtable with the respective Ids as keys.
        ///	</summary>
        ///	<remarks>
        ///	We're searching	in a depth-first manner. 
        ///	</remarks>
        /// <param name="subtreeRoot">Root of the tree to be searched.</param>
        /// <param name="nodesWithIds">Hashtable for storing nodes with non-empty Ids.</param>
        public static void FindNodesWithIds(DependencyObject subtreeRoot, Hashtable nodesWithIds)
        {
            if (subtreeRoot == null)
            {
                throw new ArgumentNullException("subtreeRoot");
            }

            if (nodesWithIds == null)
            {
                throw new ArgumentNullException("nodesWithIds");
            }

            // Check given node.
            string id = TreeHelper.GetNodeId(subtreeRoot);
            if (!(String.IsNullOrEmpty(id)) && !nodesWithIds.ContainsKey(id))
            {
                nodesWithIds.Add(id, subtreeRoot);
            }

            // Check children.
            List<DependencyObject> children = GetChildren(subtreeRoot);

            for (int i = 0; i < children.Count; i++)
            {
                FindNodesWithIds(children[i], nodesWithIds);
            }
        }

        /// <summary>
        /// Given a DependencyObject tree node, walks up the parent
        /// chain to find the root of the logical tree.
        /// </summary>
        /// <param name="logicalTreeNode">Given tree node.</param>
        public static DependencyObject FindLogicalRoot(DependencyObject logicalTreeNode)
        {
            if (logicalTreeNode == null)
            {
                throw new ArgumentNullException("logicalTreeNode");
            }

            DependencyObject walkerNode = logicalTreeNode;
            DependencyObject parentNode = LogicalTreeHelper.GetParent(walkerNode);

            // Call "Parent" repeatedly until we hit a null.
            while (parentNode != null)
            {
                walkerNode = parentNode;
                parentNode = LogicalTreeHelper.GetParent(parentNode);
            }

            // Return the last non-null node we saw
            return walkerNode;
        }

        private static Dictionary<object, string> s_registeredNames = new Dictionary<object, string>();

        // Flag that indicates if CurrentStateInvalidated has been raised.
        private static bool s_timelineStateInvalidated = false;

        // Object for sync'ing WaitForTimeManager().
        private static object s_syncObject = new object();
    }
}
