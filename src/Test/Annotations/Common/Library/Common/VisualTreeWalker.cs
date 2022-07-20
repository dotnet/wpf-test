// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Visual tree parser for recursively parsing a visual tree
//   for elements of a specific type.

using System;
using System.Collections;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Media;


namespace Avalon.Test.Annotations
{
    public class VisualTreeWalker<T>
    {
        /// <summary>
        /// Recursively walk tree starting at given node.
        /// </summary>
        /// <returns>All elements of type T that it finds.</returns>
        public IList<T> FindChildren(Visual startNode)
        {
            _results = new List<T>();
            DoWalkTree(startNode);
            return _results;
        }

        /// <summary>
        /// Walk up the visual tree starting at given node.
        /// </summary>
        /// <returns>Return first parent of type T, or null if none.</returns>
        public T FindParent(Visual startNode)
        {
            if (startNode == null)
                return default(T);
            object parent;
            do
            {
                parent = VisualTreeHelper.GetParent(startNode);
                startNode = parent as Visual;
            } while (parent != null && !IsTargetType(parent.GetType()));
            return (T)parent;
        }

        private void DoWalkTree(DependencyObject startNode)
        {
            int count = VisualTreeHelper.GetChildrenCount(startNode);
            for(int i = 0; i < count; i++)
            {
                object current = VisualTreeHelper.GetChild(startNode, i);
                if (IsTargetType(current.GetType()))
                {
                    T asType = (T)current;
                    _results.Add(asType);
                }
                DoWalkTree(VisualTreeHelper.GetChild(startNode, i));
            }
        }

        private bool IsTargetType(Type t)
        {
            return t.Equals(typeof(T)) || t.IsSubclassOf(typeof(T));
        }

        IList<T> _results;
    }
}	

