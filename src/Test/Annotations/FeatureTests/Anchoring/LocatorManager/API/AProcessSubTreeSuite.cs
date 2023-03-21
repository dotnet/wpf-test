// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the ProcessSubTreeSuite test suite.  This provides
//  some common functions and builds the trees used by the ProcessSubTree tests.
//  
//  The ProcessSubTree test suite consists of testing the following API and 
//  functionality:
//
//  - ProcessSubTree
//		These tests create annotations on nodes in the tree then call ProcessSubTree
//		and checks the list of attached annotations returned to see if we received
//		back the annotations originally set (and no extra ones)
//  - ProcessAnnotations
//		ProcessAnnotations is called on a subtree that needs to have annotations processor for it.
//		Usually called by processors so these tests just check for valid/invalid input
//

using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;

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
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using Proxies.MS.Internal.Annotations;					// TestSuite.


namespace Avalon.Test.Annotations
{
	public abstract class AProcessSubTreeSuite : AAnchoringAPITests
	{
		#region globals
		// Tree elements
		protected Canvas a = null;
		protected Canvas b = null;
		protected Canvas c = null;
		protected Canvas d = null;
		protected Canvas e = null;
		protected Canvas f = null;
		protected Canvas g = null;
		protected Canvas h = null;
		protected Canvas i = null;
		protected TextBox txt1 = null;
		protected TextBox txt2 = null;

		protected IList processResult = null;
		protected ArrayList toFind = new ArrayList();
		protected ArrayList doNotFind = new ArrayList();
		#endregion

		// ---------------------------------------------------------------------------------------------
		//									BUILD VARIATIONS OF TREE
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Creates a single-node tree with DataId and DataIdProcessor set on the root
		/// </summary>
		protected void BuildRoot()
		{
			a = new Canvas();
			EnableService(a, annotationStore);

			DataIdProcessor.SetDataId(a, "a");

			
			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
		}

		/// <summary>
		/// Creates a simple tree with root and 2 children, with DataIds set for all nodes
		/// and DataIdProcessor set on the root
		/// </summary>
		protected void BuildSingleLevelTree()
		{
			// Setup a
			BuildRoot();

			b = new Canvas();
			c = new Canvas();

			a.Children.Add(b);
			a.Children.Add(c);

			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
		}

		/// <summary>
		/// Builds the tree with depth 3, but does not set any IDs or SubTreeProcessors
		/// on the nodes
		/// </summary>
		private void BuildDoubleLevelTreeStructure()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			txt1 = new TextBox();
			txt2 = new TextBox();

			txt1.Text = "This string really serves no purpose except to act as some text to generate a hash code on for TextFingerprintProcessing";
			txt2.Text = "Some different text, so that the same TextFingerprint will not be generated for both Text elements";

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(txt1);
			b.Children.Add(txt2);
		}

		/// <summary>
		/// Builds the tree with depth 5, but does not set any IDs or SubTreeProcessors
		/// on the nodes
		/// </summary>
		private void BuildMultiLevelTreeStructure()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new Canvas();
			g = new Canvas();
			h = new Canvas();
			i = new Canvas();

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
			e.Children.Add(f);
			e.Children.Add(g);
			g.Children.Add(h);
			g.Children.Add(i);
		}

		/// <summary>
		/// Builds the tree with depth 5, with Text elements, but does not set any IDs
		/// or SubTreeProcessors on the nodes
		/// </summary>
		private void BuildMultiLevelTreeStructure_WithText()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			g = new Canvas();
			h = new Canvas();
			txt1 = new TextBox();
			txt2 = new TextBox();

			txt1.Text = "This string really serves no purpose except to act as some text to generate a hash code on for TextFingerprintProcessing";
			txt2.Text = "Some different text, so that the same TextFingerprint will not be generated for both Text elements";

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
			e.Children.Add(txt1);
			e.Children.Add(g);
			g.Children.Add(h);
			g.Children.Add(txt2);
		}

		/// <summary>
		/// Builds the tree with depth 4, with Text elements, but does not set any IDs
		/// or SubTreeProcessors on the nodes
		/// </summary>
		private void BuildMultiLevelTreeStructure_MultiSubTreeProcessors()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new Canvas();
			txt1 = new TextBox();

			txt1.Text = "This string really serves no purpose except to act as some text to generate a hash code on for TextFingerprintProcessing";

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			c.Children.Add(d);
			c.Children.Add(e);
			e.Children.Add(f);
			e.Children.Add(txt1);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 3
		/// </summary>
		protected void BuildTree_ProcessSubTree3()
		{
			BuildDoubleLevelTreeStructure();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(txt1, "txt1");
			DataIdProcessor.SetDataId(txt2, "txt2");

			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
			LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);
//			LocatorManager.SetSubTreeProcessorId(b, TextFingerprintProcessor.Id);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test cases 4, 5
		/// </summary>
		protected void BuildTree_ProcessSubTree4And5()
		{
			// The same as ProcessSubTree6, except with DataIdProcessor set on the root
			BuildTree_ProcessSubTree6();
			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 6
		/// </summary>
		protected void BuildTree_ProcessSubTree6()
		{
			BuildDoubleLevelTreeStructure();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(txt1, "txt1");
			DataIdProcessor.SetDataId(txt2, "txt2");

			LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);
//			LocatorManager.SetSubTreeProcessorId(b, TextFingerprintProcessor.Id);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 7
		/// </summary>
		protected void BuildTree_ProcessSubTree7()
		{
			// Exactly like ProcessSubtree8's tree, except with IdProcessor set on the root
			BuildTree_ProcessSubTree8();
			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 8
		/// </summary>
		protected void BuildTree_ProcessSubTree8()
		{
			BuildMultiLevelTreeStructure();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(f, "f");
			DataIdProcessor.SetDataId(i, "i");
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 9
		/// </summary>
		protected void BuildTree_ProcessSubTree9()
		{
			BuildMultiLevelTreeStructure_WithText();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(g, "g");
			DataIdProcessor.SetDataId(h, "h");
			DataIdProcessor.SetDataId(txt1, "txt1");
			DataIdProcessor.SetDataId(txt2, "txt2");

			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
			LocatorManager.SetSubTreeProcessorId(b, DataIdProcessor.Id);
//			LocatorManager.SetSubTreeProcessorId(b, TextFingerprintProcessor.Id);
		}

		/// <summary>
		/// Sets DataIds and SubTreeProcessors for the tree used for test case 10
		/// </summary>
		protected void BuildTree_ProcessSubTree10()
		{
			BuildMultiLevelTreeStructure_MultiSubTreeProcessors();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(d, "d");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(f, "f");
			DataIdProcessor.SetDataId(txt1, "txt1");

			LocatorManager.SetSubTreeProcessorId(a, DataIdProcessor.Id);
//			LocatorManager.SetSubTreeProcessorId(a, TextFingerprintProcessor.Id);
			LocatorManager.SetSubTreeProcessorId(c, DataIdProcessor.Id);
			LocatorManager.SetSubTreeProcessorId(e, DataIdProcessor.Id);
//			LocatorManager.SetSubTreeProcessorId(e, TextFingerprintProcessor.Id);
		}


		// ---------------------------------------------------------------------------------------------
		//									HELPER METHODS
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Calls the LocatorManager.ProcessSubTree internal function
		/// </summary>
		/// <param name="subTreeRoot">root of the subtree to process</param>
		/// <returns>list of attached annotations in subtree</returns>
		protected IList ProcessSubTree(DependencyObject subTreeRoot)
		{
			return (IList)AnchoringAPIHelpers.CallInternalMethod("ProcessSubTree", manager, new object[] {subTreeRoot});
		}

		/// <summary>
		/// Searches the list of attached annotations returned to find the ones set in the
		/// subtree.  Also checks that annotations set in the subtree that should not be
		/// found do not appear in the list returned
		/// </summary>
		/// <param name="processedAnnots">list returned by ProcessSubTree</param>
		/// <param name="toBeFound">annotations that should be found by ProcessSubTree</param>
		/// <param name="shouldNotBeFound">annotations that shouldn't be found by ProcessSubTree</param>
		protected void SearchProcessedAnnotations(IList processedAnnots, ArrayList toBeFound, 
			ArrayList shouldNotBeFound)
		{
			int annotationsFound = 0;

			if (toBeFound == null || processedAnnots == null)
				failTest("Cannot search and compare annotations in null lists");

			foreach (IAttachedAnnotation ann in processedAnnots)
			{
				Annotation currAnn = ann.Annotation;
				foreach (Annotation findMe in toBeFound)
				{
					if (AnchoringAPIHelpers.AreAnnotationsEqual(currAnn, findMe))
						annotationsFound++;
				}

				if (shouldNotBeFound != null)
				{
					foreach (Annotation dontFindMe in shouldNotBeFound)
					{
						if (AnchoringAPIHelpers.AreAnnotationsEqual(currAnn, dontFindMe))
						{
							failTest("Extra annotation should not have been returned");
							return;
						}
					}
				}
			}

			if (annotationsFound == toBeFound.Count)
				passTest("ProcessSubTree returned all expected annotations");
			else
				failTest("ProcessSubtree did not return all expected annotations created on tree");
		}


	}		// end of AProcessSubTreeSuite class

}			// end of namespace

