// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define TRACE

#region Using directives
using System;
using System.Collections;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

#endregion

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Contains common Visual and Logical Tree helper utility functions
    /// Will add more functionality to this class as reqd
    /// </summary>
    public class TreeUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        public TreeUtilities()
        {

        }

        // Lifted the following from the DrtBase.cs file under wcp Drts
        // Useful functionality that we should make use of and probably
        // expose such (and other) functionality through a base class BVT that all
        // our BVTs should derive from

        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            if (node == null)
                return null;

            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            DependencyObject result;
            DependencyObject child;

            // if not, recursively look at the logical children
            foreach (object currentChild in LogicalTreeHelper.GetChildren(node))
            {
                child = currentChild as DependencyObject;
                result = FindElementByPropertyValue(dp, value, child, true);
                if (result != null)
                    return result;
            }

            // then the visual children
            Visual vNode = node as Visual;
            if (vNode != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(vNode);
                for (int i = 0; i < count; i++)
                {
                    child = VisualTreeHelper.GetChild(vNode,i) as DependencyObject;
                    result = FindElementByPropertyValue(dp, value, child, true);
                    if (result != null)
                        return result;
                }
            }

            // not found
            return null;
        }

        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public DependencyObject FindElementByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindElementByPropertyValue(dp, value, node, true);
        }

        /// <summary>
        /// Search the visual and logical trees looking for a node with
        /// a given Name.
        /// </summary>
        /// <param name="id">id of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindElementByID(string id, DependencyObject node)
        {
            return FindElementByPropertyValue(FrameworkElement.NameProperty, id, node);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right type
            if (includeNode)
            {
                if (type == node.GetType())
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByType(type, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given type.
        /// </summary>
        /// <param name="type">type of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindVisualByType(Type type, DependencyObject node)
        {
            return FindVisualByType(type, node, true);
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given Name.
        /// </summary>
        /// <param name="id">id of desired node</param>
        /// <param name="node">starting node for the search</param>
        public DependencyObject FindVisualByID(string id, DependencyObject node)
        {
            return FindVisualByPropertyValue(FrameworkElement.NameProperty, id, node);
        }
        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <param name="includeNode">if false, do not test the node itself</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node, bool includeNode)
        {
            // see if the node itself has the right value
            if (includeNode)
            {
                object nodeValue = node.GetValue(dp);
                if (Object.Equals(value, nodeValue))
                    return node;
            }

            // if not, recursively look at the visual children
            int count = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < count; i++)
            {
                DependencyObject result = FindVisualByPropertyValue(dp, value, VisualTreeHelper.GetChild(node,i), true);
                if (result != null)
                    return result;
            }

            // not found
            return null;
        }

        /// <summary>
        /// Do a depth-first search of the visual tree looking for a node with
        /// a given property value.
        /// </summary>
        /// <param name="dp">property to query</param>
        /// <param name="value">desired value</param>
        /// <param name="node">starting node for the search</param>
        /// <returns></returns>
        public DependencyObject FindVisualByPropertyValue(DependencyProperty dp, object value, DependencyObject node)
        {
            return FindVisualByPropertyValue(dp, value, node, true);
        }

        // Recursively searches the visual tree for an element with the given Name...
        public FrameworkElement FindVisualTreeElementByID(string elementID, object searchBase)
        {
            // DependencyObject is the common base class for Visual and Visual3D
            DependencyObject v = (DependencyObject)searchBase;
            int count = VisualTreeHelper.GetChildrenCount(v);

            for (int i = 0; i < count; i++)
            {
                bool lookingAtFrameworkElement = true;
                DependencyObject child = VisualTreeHelper.GetChild(v, i);
                try
                {
                    FrameworkElement currentElement = (FrameworkElement)child;
                }
                catch (System.InvalidCastException)
                {
                    lookingAtFrameworkElement = false;
                }

                if (lookingAtFrameworkElement && ((FrameworkElement)child).Name == elementID)
                {
                    // We found it! yay!
                    return (FrameworkElement)child;
                }
                else
                {
                    // Nope, so start the depth first search, discarding if it isnt below this node
                    FrameworkElement leafResult = FindVisualTreeElementByID(elementID, child);

                    if (leafResult != null)
                    {
                        return leafResult;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Goes through the logical tree to search for a FrameworkElement or
        /// FrameworkContentElement with the given Name
        /// </summary>
        public DependencyObject TraverseLogicalTree(string idToFind, DependencyObject node)
        {
            IEnumerator logicalChildren = LogicalTreeHelper.GetChildren(node).GetEnumerator();
            DependencyObject nodeWithId = null;

            logicalChildren.Reset();

            while (logicalChildren.MoveNext())
            {
                if (logicalChildren.Current is FrameworkElement)
                {
                    FrameworkElement feCurrent = logicalChildren.Current as FrameworkElement;
                    if (feCurrent.Name.Equals(idToFind))
                    {
                        nodeWithId = feCurrent;
                    }
                    else
                    {
                        nodeWithId = TraverseLogicalTree(idToFind, feCurrent);
                    }
                }

                if (logicalChildren.Current is FrameworkContentElement)
                {
                    FrameworkContentElement fceCurrent = logicalChildren.Current as FrameworkContentElement;
                    if (fceCurrent.Name.Equals(idToFind))
                    {
                        nodeWithId = fceCurrent;
                    }
                    else
                    {
                        nodeWithId = TraverseLogicalTree(idToFind, fceCurrent);
                    }
                }
            }
            return nodeWithId;
        }
    }
}
