// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Abstracts core-level tree operations.  The intention
 *          of abstracting the operations is to reduce the effect
 *          of future API changes in the product's core level.
 *
 
  
 * Revision:         $Revision: 1 $
 
 * Filename:         $Source: //depot/devdiv/Orcas/feature/element3d/wpf/Test/ElementServices/FeatureTests/Untrusted/Common/VisualTreeUtils.cs $
********************************************************************/
using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace Avalon.Test.CoreUI
{
    /// <summary>
    /// Abstraction of core-level tree operations.
    /// </summary>
    public partial class VisualTreeUtils
    {
        /// <summary>
        /// Walks the visual tree (using VisualTreeHelper) to find a requested element.
        /// </summary>
        /// <param name="id">the name assigned to the to-be-found element</param>
        /// <param name="root">the root element</param>
        /// <returns>
        /// The element, when found, otherwise null.
        /// </returns>
        static public FrameworkElement FindElement(string id, DependencyObject root)
        {
            if (root == null) return null;
            FrameworkElement fe = root as FrameworkElement;
            
            if (fe != null && fe.Name == id)
            {
                return fe;
            }

            int count = VisualTreeHelper.GetChildrenCount(root);
            Console.WriteLine("Count: " + count);

            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(root,i);
                Console.WriteLine("child: " + child.GetType());

                DependencyObject feRet = FindElement(id, child);
                if (feRet != null)
                {
                    return (FrameworkElement)feRet;
                }
            }
            return null;
        }
    }
}



