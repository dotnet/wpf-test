// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Reflection;

using Annotations.Test.Framework;


using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;

using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Media;
using System.Xml;
using Annotations.Test.Reflection;
using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Module that provides default support for testing SelectionProcessor apis.
	/// May be extended to provide implementation specific verification.
	/// </summary>
	public class SelectionProcessorTester
	{
		public SelectionProcessorTester(TestSuite testSuite, SelectionProcessor selectionProcessor)
		{
            suite = testSuite;
            processor = selectionProcessor;
		}

		#region Test MergeSelections

		/// <summary>
		/// Pass the given anchors to SelectionProcessor.MergeSelections and verify that 
		/// the expected TextAnchor is returned.
		/// </summary>
		public void VerifyMergeSelections(object anchor1, object anchor2, bool expectedResult, string expectedNewAnchor)
		{
            suite.printStatus("Anchor1 = '" + ((anchor1 == null) ? "null" : SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor1))) + "'");
            suite.printStatus("Anchor2 = '" + ((anchor2 == null) ? "null" : SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor2))) + "'");

            object newAnchor;
            suite.AssertEquals("Verify return value.", expectedResult, processor.MergeSelections(anchor1, anchor2, out newAnchor));

			if (expectedNewAnchor == null)
				suite.AssertNull("Expected merge result to be null.", newAnchor);
			else
			{
                suite.AssertEquals("Verify new anchor contents.", SelectionModule.PrintFriendlySelection(expectedNewAnchor), SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(newAnchor)));
			}
		}

		/// <summary>
		/// Verify MergeSelection for very long results.  Only check beginning text, end text, and total length.
		/// </summary>
        public void VerifyMergeSelections(object anchor1, object anchor2, string startText, string endText, int length)
		{
            suite.printStatus("Anchor1 = '" + ((anchor1 == null) ? "null" : SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor1))) + "'");
            suite.printStatus("Anchor2 = '" + ((anchor2 == null) ? "null" : SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor2))) + "'");

			object result;
            suite.Assert("Verify return value is true.", processor.MergeSelections(anchor1, anchor2, out result));

            suite.AssertNotNull("Verify anchor is initialized.", AnnotationTestHelper.IsTextAnchor(result));

            string newAnchorText = AnnotationTestHelper.GetText(result);
            suite.printStatus("Merged Anchor = '" + SelectionModule.PrintFriendlySelection(newAnchorText) + "'.");

            suite.Assert("Verify starting text.", newAnchorText.StartsWith(startText));
            suite.Assert("Verify ending text.", newAnchorText.EndsWith(endText));
            suite.AssertEquals("Verify length of anchor.", length, newAnchorText.Length);
		}

		/// <summary>
		/// Verify that SelectionProcessor.MergeSelections fails with a specific exception.
		/// </summary>
		/// <param name="selection1">Parameter 1 to MergeSelections.</param>
		/// <param name="selection2">Parameter 2 to MergeSelections.</param>
		/// <param name="expectedExceptionType">Expected type of exception.</param>
		public void VerifyMergeSelectionsFailed(object selection1, object selection2, Type expectedExceptionType)
		{
			bool exception = false;
			try
			{
				object result;
				processor.MergeSelections(selection1, selection2, out result);			
			}
			catch (Exception e)
			{
				exception = true;
                suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}

		#endregion Test MergeSelections

		#region Test GetSelectedNodes

		/// <summary>
		/// Default behavior: calls GetSelectedNodes for the given selection, verifies that the returned nodes
		/// are .Equal to the expectedResults.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="expectedResults"></param>
		public virtual void VerifyGetSelectedNodes(object selection, object[] expectedResults)
		{
			IList<DependencyObject> nodes = processor.GetSelectedNodes(selection);

			if (expectedResults == null)
				suite.AssertNull("Expected result is null.", nodes);
			else 
			{
				suite.AssertNotNull("Verify result is not null.", nodes);
				suite.AssertEquals("Verify number of selected nodes.", expectedResults.Length, nodes.Count);
				for (int i=0; i < expectedResults.Length; i++) 
				{
					suite.AssertEquals("Verify node " + i + ".", expectedResults[i], nodes[i]);
				}
			}
		}

		/// <summary>
		/// Verify that SelectionProcessor.GetSelectedNodes fails with a specific exception.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="expectedExceptionType"></param>
		public virtual void VerifyGetSelectedNodesFails(object selection, Type expectedExceptionType)
		{
			bool exception = false;
			try
			{
				processor.GetSelectedNodes(selection);				
			}
			catch (Exception e)
			{
				exception = true;
                suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}

		#endregion Test GetSelectedNodes

		#region GenerateLocatorParts

		/// <summary>
		/// Verify that SelectionProcessor.GenerateLocatorParts fails with a specific exception.
		/// </summary>
		/// <param name="selection"></param>
		/// <param name="expectedExceptionType"></param>
		public virtual void VerifyGenerateLocatorPartsFails(object selection, DependencyObject startNode, Type expectedExceptionType)
		{
			bool exception = false;
			try
			{
				processor.GenerateLocatorParts(selection, startNode);
			}
			catch (Exception e)
			{
				exception = true;
				suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}


		#endregion GenerateLocatorParts

		#region ResolveLocatorPart

		/// <summary>
		/// Verify that value returned by SelectionProcessor.ResolveLocatorPart is .Equal to the expectedResult.
		/// </summary>
		public virtual void VerifyResolveLocatorPart(ContentLocatorPart part, DependencyObject startNode, object expectedResult, AttachmentLevel expectedAttachmentLevel)
		{
			AttachmentLevel attachmentLevel;
			object result = processor.ResolveLocatorPart(part, startNode, out attachmentLevel);
			suite.AssertEquals("Verify result of ResolveLocatorPart.", expectedResult, result);
			suite.AssertEquals("Verify attachmentLevel.", expectedAttachmentLevel, attachmentLevel);
		}

		/// <summary>
		/// Verify that SelectionProcessor.ResolveLocatorPart fails with a specific exception.
		/// </summary>
		public virtual void VerifyResolveLocatorPartFails(ContentLocatorPart part, DependencyObject node, Type expectedExceptionType)
		{
			bool exception = false;
			try
			{
				AttachmentLevel attachmentLevel;
				processor.ResolveLocatorPart(part, node, out attachmentLevel);
			}
			catch (Exception e)
			{
				exception = true;
                suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}

		#endregion ResolveLocatorPart

		#region GetParent

		/// <summary>
		/// Verify the type returned by SelectionProcessor.GetParent and that it is .Equal to the expecteParent.
		/// </summary>
		public virtual void VerifyGetParent(object selection, UIElement expectedParent)
		{
			UIElement result = processor.GetParent(selection);

			if (expectedParent == null)
			{
				suite.AssertNull("Result should be null.", result);
			}
			else
			{
				suite.AssertNotNull("Verify result is non-null.", result);
				suite.AssertEquals("Verify type of parent.", expectedParent.GetType(), result.GetType());
				suite.AssertEquals("Verify parent.", expectedParent, result);
			}
		}

		/// <summary>
		/// Verify that SelectionProcessor.GetParent fails with a specific exception.
		/// </summary>
		public virtual void VerifyGetParentFails(object selection, Type expectedExceptionType)
		{	
			bool exception = false;
			try
			{
				processor.GetParent(selection);
			}
			catch (Exception e)
			{
				exception = true;
                suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}

		#endregion GetParent

		#region GetAnchorPoint

		private static double ANCHOR_POINT_TOLERANCE = 1e-11;

		/// <summary>
		/// Verify the SelectionProcessor.GetAnchorPoint returns a point with ANCHOR_POINT_TOLERANCE of 
		/// the expectedPoint.
		/// </summary>
		public virtual void VerifyGetAnchorPoint(object selection, Point expectedPoint)
		{
			Point result = processor.GetAnchorPoint(selection);
			suite.AssertNotNull("Result is never null.", result);

			suite.AssertEquals("Verify x coor.", expectedPoint.X, result.X, ANCHOR_POINT_TOLERANCE);
			suite.AssertEquals("Verify y coor.", expectedPoint.Y, result.Y, ANCHOR_POINT_TOLERANCE);
		}

		/// <summary>
		/// Verify that SelectionProcessor.GetAnchorPoint fails with a specific exception.
		/// </summary>
		public virtual void VerifyGetAnchorPointFails(object selection, Type expectedExceptionType)
		{
			bool exception = false;
			try
			{
				processor.GetAnchorPoint(selection);
			}
			catch (Exception e)
			{
				exception = true;
                suite.AssertEquals("Exception was: '" + e.Message + "'.", expectedExceptionType, e.GetType());
			}

			if (!exception)
				suite.failTest("Expected exception but none occurred.");
		}

		#endregion GetAnchorPoint

        
        protected SelectionProcessor processor;
        
        protected TestSuite suite;
	}
}
