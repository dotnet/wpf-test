// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts core-level tree operations.  The intention
 *          of abstracting the operations is to reduce the effect
 *          of future API changes in the product's core level.
********************************************************************/
using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Abstraction of core-level tree operations.
    /// </summary>
    public partial class VisualTreeUtils
    {
        /// <summary>
        /// Returns a specific visual child of a given parent node.
        /// </summary>
        static public object GetChild(object parent, int index)
        {
            if (!(parent is Visual))
            {
                throw new ArgumentException("The parent node is not the correct type.", "parent");
            }
            if(index<0)
            {
                throw new ArgumentException("The index has to be positive.", "index");
            }

            return VisualTreeHelper.GetChild((Visual)parent, index);
        }
        /// <summary>
        /// Checks if a given node has any visual children.
        /// </summary>
        static public bool HasAnyChildren(object parent)
        {
            return HasChild(parent, 1);
        }
        /// <summary>
        /// Checks if a given node has at least as has many visual children as the given number.
        /// </summary>
        static public bool HasChild(object parent, int count)
        {
            if (!(parent is Visual))
            {
                throw new ArgumentException("The parent node is not the correct type.", "parent");
            }

            if (!(count > 0))
            {
                throw new ArgumentOutOfRangeException("The number must be greater than 0.", "count");
            }

            int myCount = VisualTreeHelper.GetChildrenCount((Visual)parent);
            return myCount>=count;
        }

        /// <summary>
        /// Find ContentPresenter in visual tree.
        /// Adapted from FindDataVisuals in ConnectedData\Common\Util.cs
        /// TO-DO:  a copy of this exists in TriggerActionRunner.cs.  Shouldn't be there too.
        /// </summary>
        static public FrameworkElement getFirstContentPresenter(FrameworkElement element)
        {
            if ((element is ContentPresenter) && !(element is ScrollContentPresenter)) return element;

            FrameworkElement cp = null;

            int count = VisualTreeHelper.GetChildrenCount(element);
            
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject

                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                if (child is FrameworkElement)
                    cp = getFirstContentPresenter((FrameworkElement)child);

                if (cp != null) return cp;
            }

            return null;
        }
    }
}



