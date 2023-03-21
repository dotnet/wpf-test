// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1 test cases for the PathNode test suite.


using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    [ExecutionGroupCompatible(false)]
    public class PathNodeSuite_Pri1 : APathNodeSuite
    {
        /// <summary>
        /// PathNode.Node should return a (DependencyObject passed into the constructor)
        /// </summary>
        private void pathnode_node1()
        {
            if (a.Equals(pn_a.Node))
                passTest("PathNode.Node:  Logical tree node correctly returned");
            else
                failTest("PathNode.Node:  Logical tree node not returned");
        }

        /// <summary>
        /// PathNode.Children should return a list of 0 children of c
        /// </summary>
        [Priority(1)]
        private void pathnode_children1()
        {
            CompareChildren(pn_c, 0);
        }

        /// <summary>
        /// PathNode.Children should return a list of 5 children of b
        /// </summary>
        [Priority(1)]
        private void pathnode_children2()
        {
            CompareChildren(pn_b, 5);
        }

        /// <summary>
        /// PathNode.Equals called on a null object should return false
        /// </summary>
        [Priority(1)]
        private void pathnode_equals1()
        {
            if (pn_a.Equals(null))
                failTest("PathNode.Equals(null) should return false");
            else
                passTest("PathNode.Equals(null) correctly returned false");
        }

        /// <summary>
        /// PathNode.Equals called on a non-PathNode element should return false
        /// </summary>
        [Priority(1)]
        private void pathnode_equals2()
        {
            if (pn_a.Equals(h))
                failTest("PathNode.Equals(non-PathNode object) should return false");
            else
                passTest("PathNode.Equals(non-PathNode object) correctly returned false");
        }

        /// <summary>
        /// PathNode.Equals called on 2 PathNode objects based on the same DependencyObject returns true
        /// </summary>
        [Priority(1)]
        private void pathnode_equals3()
        {
            if (pn_b.Equals(pn_b1))
                passTest("PathNode.Equals on PathNodes with the same DependencyObject correctly returned true");
            else
                failTest("PathNode.Equals on PathNodes with the same DependencyObject should return true");
        }

        /// <summary>
        /// PathNode.Equals called on 2 PathNode objects based on diff DependencyObjects returns false
        /// </summary>
        [Priority(1)]
        private void pathnode_equals4()
        {
            if (pn_b.Equals(pn_c))
                failTest("PathNode.Equals on PathNodes with different DependencyObjects should return false");
            else
                passTest("PathNode.Equals on PathNodes with different DependencyObjects correctly returned false");
        }

        /// <summary>
        /// PathNode.GetHashCode called multiple times on the same node should always return the same #
        /// </summary>
        [Priority(1)]
        private void pathnode_gethashcode1()
        {
            int result = pn_a.GetHashCode();

            for (int i = 0; i < 5; i++)
            {
                if (pn_a.GetHashCode() != result)
                    failTest("Multiple calls to PathNode.GetHashCode for the same node returned different hashcodes");
            }

            passTest("Multiple calls to PathNode.GetHashCode for the same node returned the same hashcode each time");
        }

        /// <summary>
        /// PathNode.GetHashCode called on PathNodes based on different DependencyObjects should return non-equal values
        /// </summary>
        [Priority(1)]
        private void pathnode_gethashcode2()
        {
            CompareHashCodes(pn_b, pn_c);
        }

        /// <summary>
        /// PathNode.GetHashCode called on PathNodes based on same DependencyObjects should return same value
        /// </summary>
        [Priority(1)]
        private void pathnode_gethashcode3()
        {
            CompareHashCodes(pn_b, pn_b1);
        }
    }
}

