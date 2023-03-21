// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the PathNode test suite.  This provides
//  some common functions that verify the return values of PathNode methods
//  and builds the tree used by all PathNode tests.


using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;					// TestSuite.


namespace Avalon.Test.Annotations
{
	public abstract class APathNodeSuite : AAnchoringAPITests
	{
		#region globals
		private String testCaseNumber = String.Empty;

		// Tree elements (DependencyObjects)
		protected Canvas a = null;
		protected Canvas b = null;
		protected Canvas c = null;
		protected Canvas d = null;
		protected Canvas e = null;
		protected Canvas f = null;
		protected Canvas g = null;
		protected Canvas h = null;

		// PathNodes
		protected PathNode pn_a = null;
		protected PathNode pn_b = null;
		protected PathNode pn_b1 = null;
		protected PathNode pn_c = null;
		#endregion

		/// <summary>
		/// Make the tree to be used by all PathNode tests.  Create the PathNodes
		/// used for testing the API
		/// </summary>
		[TestCase_Setup()]
        protected void BuildPathNodeTree()
		{
            DoSetup();

			#region build tree
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new Canvas();
			g = new Canvas();
			h = new Canvas();

			// Build the app tree
			b.Children.Add(d);
			b.Children.Add(e);
			b.Children.Add(f);
			b.Children.Add(g);
			b.Children.Add(h);
			#endregion

			#region create PathNodes
			ArrayList buildPathForElementsParams_A = new ArrayList();
			ArrayList buildPathForElementsParams_B = new ArrayList();
			ArrayList buildPathForElementsParams_C = new ArrayList();

			buildPathForElementsParams_A.Add(a);
			buildPathForElementsParams_C.Add(c);
			buildPathForElementsParams_B.Add(d);
			buildPathForElementsParams_B.Add(e);
			buildPathForElementsParams_B.Add(f);
			buildPathForElementsParams_B.Add(g);
			buildPathForElementsParams_B.Add(h);

			pn_a = AnchoringAPIHelpers.CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_A }) as PathNode;
			pn_b = AnchoringAPIHelpers.CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_B }) as PathNode;
			pn_b1 = AnchoringAPIHelpers.CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_B }) as PathNode;
			pn_c = AnchoringAPIHelpers.CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_C }) as PathNode;
			#endregion
		}


		#region PathNode tests

//		public void RunTestCase(string caseNumber)
//		{
//			testCaseNumber = caseNumber;
//
//			#region build tree
//			a = new Canvas();
//			b = new Canvas();
//			c = new Canvas();
//			d = new Canvas();
//			e = new Canvas();
//			f = new Canvas();
//			g = new Canvas();
//			h = new Canvas();
//
//			// Build the app tree
//			b.Children.Add(d);
//			b.Children.Add(e);
//			b.Children.Add(f);
//			b.Children.Add(g);
//			b.Children.Add(h);
//			#endregion
//
//			#region create PathNodes
//			ArrayList buildPathForElementsParams_A = new ArrayList();
//			ArrayList buildPathForElementsParams_B = new ArrayList();
//			ArrayList buildPathForElementsParams_C = new ArrayList();
//
//			buildPathForElementsParams_A.Add(a);
//			buildPathForElementsParams_C.Add(c);
//			buildPathForElementsParams_B.Add(d);
//			buildPathForElementsParams_B.Add(e);
//			buildPathForElementsParams_B.Add(f);
//			buildPathForElementsParams_B.Add(g);
//			buildPathForElementsParams_B.Add(h);
//
//			pn_a = CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_A }) as PathNode;
//			pn_b = CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_B }) as PathNode;
//			pn_b1 = CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_B }) as PathNode;
//			pn_c = CallInternalMethod("BuildPathForElements", typeof(PathNode), new Object[] { buildPathForElementsParams_C }) as PathNode;
//			#endregion
//
//			switch (caseNumber)
//			{
//				#region PathNode.Node tests
//				// PathNode.Node should return a (DependencyObject passed into the constructor)
//				case "pathnode_node1":
//					try
//					{
//						object result = pn_a.Node;
//
//						if (result.Equals(a))
//							PassTest(caseNumber, "PathNode.Node:  Logical tree node correctly returned");
//						else
//							FailTest(caseNumber, "PathNode.Node:  Logical tree node not returned");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//				#endregion
//
//				#region PathNode.Children tests
//				// PathNode.Children should return a list of 0 children of c
//				case "pathnode_children1":
//					CompareChildren(pn_c, 0);
//					break;
//
//				// PathNode.Children should return a list of 5 children of b
//				case "pathnode_children2":
//					CompareChildren(pn_b, 5);
//					break;
//				#endregion
//
//				#region PathNode.Equals tests
//				// PathNode.Equals called on a null object should return false
//				case "pathnode_equals1":
//					try
//					{
//						if (pn_a.Equals(null))
//							FailTest(caseNumber, "PathNode.Equals(null) should return false");
//						else
//							PassTest(caseNumber, "PathNode.Equals(null) correctly returned false");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//
//				// PathNode.Equals called on a non-PathNode element should return false
//				case "pathnode_equals2":
//					try
//					{
//						if (pn_a.Equals(h))
//							FailTest(caseNumber, "PathNode.Equals(non-PathNode object) should return false");
//						else
//							PassTest(caseNumber, "PathNode.Equals(non-PathNode object) correctly returned false");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//
//				// PathNode.Equals called on 2 PathNode objects based on the same DependencyObject returns true
//				case "pathnode_equals3":
//					try
//					{
//						if (pn_b.Equals(pn_b1))
//							PassTest(caseNumber, "PathNode.Equals on PathNodes with the same DependencyObject correctly returned true");
//						else
//							FailTest(caseNumber, "PathNode.Equals on PathNodes with the same DependencyObject should return true");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//
//				// PathNode.Equals called on 2 PathNode objects based on diff DependencyObjects returns false
//				case "pathnode_equals4":
//					try
//					{
//						if (pn_b.Equals(pn_c))
//							FailTest(caseNumber, "PathNode.Equals on PathNodes with different DependencyObjects should return false");
//						else
//							PassTest(caseNumber, "PathNode.Equals on PathNodes with different DependencyObjects correctly returned false");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//				#endregion
//
//				#region PathNode.GetHashCode tests
//				// PathNode.GetHashCode called multiple times on the same node should always return the same #
//				case "pathnode_gethashcode1":
//					try
//					{
//						int result = pn_a.GetHashCode();
//
//						for (int i = 0; i < 5; i++)
//						{
//							if (pn_a.GetHashCode() != result)
//								FailTest(caseNumber, "Multiple calls to PathNode.GetHashCode for the same node returned different hashcodes");
//						}
//
//						PassTest(caseNumber, "Multiple calls to PathNode.GetHashCode for the same node returned the same hashcode each time");
//					}
//					catch (Exception exp)
//					{
//						FailTest(caseNumber, "Unexpected exception: " + exp.ToString());
//					}
//					break;
//
//				// PathNode.GetHashCode called on PathNodes based on different DependencyObjects should return non-equal values
//				case "pathnode_gethashcode2":
//					CompareHashCodes(pn_b, pn_c);
//					break;
//
//				// PathNode.GetHashCode called on PathNodes based on same DependencyObjects should return same value
//				case "pathnode_gethashcode3":
//					CompareHashCodes(pn_b, pn_b1);
//					break;
//				#endregion
//
//				default:
//					FailTest("TestPathNodes", "Unexpected test case number");
//					break;
//			}
//		}
		#endregion


		// -------------------------------------------------------------------------------
		//                           COMMON PATHNODE METHODS
		// -------------------------------------------------------------------------------


		/// <summary>
		/// Checks whether the PathNodes are derived from the same DependencyObject
		/// Then, if they are, checks that the hash codes generated for both PathNodes give
		/// the same value.  If they are not, checks that the hash code generated for each
		/// PathNode gives a different value from the other
		/// </summary>
		/// <param name="p1">first PathNode to generate a hash code for</param>
		/// <param name="p2">second PathNode to generate a hash code for</param>
		protected void CompareHashCodes(PathNode p1, PathNode p2)
		{
			// Check if the PathNodes are based on the same DependencyObject
			if (p1.Node.Equals(p2.Node))
			{
				// if they are, they should return the same hash code
				if (p1.GetHashCode() == p2.GetHashCode())
					passTest("PathNode.GetHashCode on PathNodes (same DependencyObject) returns equal values");
				else
					failTest("PathNode.GetHashCode on PathNodes (same DependencyObject) should return equal values");
			}
			else
			{
				// if they aren't, then they should return different values
				if (p1.GetHashCode() != p2.GetHashCode())
					passTest("PathNode.GetHashCode on PathNodes (diff DependencyObjects) returns different values");
				else
					failTest("PathNode.GetHashCode on PathNodes (diff DependencyObjects) should return different values");
			}
		}


		/// <summary>
		/// Checks the value returned by the PathNode.Children property against the
		/// expected number of children for that PathNode
		/// </summary>
		/// <param name="p">PathNode to find children for</param>
		/// <param name="expectedChildren">number of children expected for this node</param>
		protected void CompareChildren(PathNode p, int expectedChildren)
		{
			IList pChildren = p.Children;

			if (pChildren == null)
				failTest(expectedChildren + " children expected.  Null list returned");
			else if (pChildren.Count == expectedChildren)
				passTest(expectedChildren + " children correctly returned");
			else
				failTest(expectedChildren + " children expected, " + pChildren.Count + " children returned");
		}

	}		// end of APathNodeSuite class

}			// end of namespace

