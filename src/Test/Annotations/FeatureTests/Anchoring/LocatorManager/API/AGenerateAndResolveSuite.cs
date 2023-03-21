// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Superclass for the GenerateAndResolveSuite test suite.  
//  This provides common functions and builds the trees used by the 
//  GenerateAndResolveSuite tests.
//  
//  The GenerateAndResolveSuite test suite consists of testing the following
//  API and functionality:
//
//  - ResolveLocator
//		ResolveLocator takes in a ContentLocatorBase and an offset and tries to find the node in the 
//		tree starting from a specified node that matches the ContentLocatorPart at that offset.
//  - GenerateLocators
//		GenerateLocators takes an object (tree node or text selection) and creates
//		a sequence of ContentLocatorParts that define in a data-centric way where the
//		selection is anchored
//  - BasicGenerateLocators
//		BasicGenerateLocators tests whether Locators are generated for selections that
//		either have no Name set (when DataIdProcessor is in effect) or is not a chunkable
//		element (when TextFingerprintProcessor is in effect)
//  - SinglePassGenerateLocators
//		SinglePassGenerateLocators generates a set of Locators based off a selection in
//		the tree, then resolves each ContentLocatorBase generated and checks that the resolved  
//		object equals the original anchor 
//  - MultiPassGenerateLocators
//		MultiPassGenerateLocators generates Locators for a selection, resolves each
//		generated ContentLocatorBase and checks if the resolved object equals the original anchor.
//		Then it generates Locators for the resolved object and checks if the second set of 
//		Locators equals the first set

using System;
using System.Windows;
using System.Windows.Controls;

using System.Collections;
using System.Collections.Generic;

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
	public abstract class AGenerateAndResolveSuite : AAnchoringAPITests
	{
		#region globals
		// Used for API testing GenerateLocators and ResovleLocator
		protected IList<ContentLocatorBase> locList = null;
		protected AttachmentLevel type;
		protected object resolvedLocator = null;
		protected ContentLocator locator = null;

		// Tree elements
		protected Canvas a = null;
		protected Canvas b = null;
		protected Canvas c = null;
		protected Canvas d = null;
		protected Canvas e = null;
		protected Canvas f = null;
		protected Canvas g = null;
		protected Canvas i = null;
		protected TextBlock txt1 = null;
		protected TextBlock txt2 = null;

		protected AttachmentLevel anchorType;
		protected object anchor = null;
		protected int numExpectedLocators = 0;
		protected int numExpectedLocatorParts = 0;

		protected ContentLocator locator1 = null;
		protected ContentLocator locator2 = null;
		protected object selection2 = null;
		#endregion		

		// ---------------------------------------------------------------------------------------------
		//									BUILD VARIATIONS OF TREE
		// ---------------------------------------------------------------------------------------------

		/// <summary>
		/// Builds tree with root + 2 children.  Sets service and store on root
		/// </summary>
		protected void BuildSimpleTree_BasicGenerateLocators()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);

			numExpectedLocators = 1;
		}

		/// <summary>
		/// Builds a multi-level tree with depth of 5.  Sets service and store on the root
		/// </summary>
		protected void BuildComplexTree_BasicGenerateLocators()
		{
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			f = new Canvas();
			g = new Canvas();
			txt1 = new TextBlock();
			i = new Canvas();

			txt1.Text = "The quick brown fox jumped over the lazy dog";

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
			e.Children.Add(f);
			e.Children.Add(g);
			g.Children.Add(txt1);
			g.Children.Add(i);

			numExpectedLocators = 1;
		}

		/// <summary>
		/// Builds a simple tree, similar to that of BasicGenerateLocators, but with
		/// DataIds set on all nodes in the tree
		/// </summary>
		protected void BuildSimpleTree_SinglePassGenerateLocators()
		{
			BuildSimpleTree_BasicGenerateLocators();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");

			numExpectedLocators = 1;
			numExpectedLocatorParts = 2;
		}

		/// <summary>
		/// Builds a tree of depth 3 (incl 2 TextBlock leaves) and sets DataIds on root and 
		/// FrameworkElement leaf
		/// </summary>
		protected void BuildDualLevelTree_SinglePassGenerateLocators()
		{
			BuildSimpleTree_BasicGenerateLocators();
			txt1 = new TextBlock();
			txt2 = new TextBlock();

			b.Children.Add(txt1);
			b.Children.Add(txt2);
			//LocatorManager.SetSubTreeProcessorId(b, TextFingerprintProcessor.Id);

			txt1.Text = "The quick brown fox jumped over the lazy dog";

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(c, "c");

			numExpectedLocators = 1;
			numExpectedLocatorParts = 3;
		}

		/// <summary>
		/// 
		/// </summary>
		protected void BuildMultiLevelTree_SinglePassGenerateLocators()
		{
			BuildSimpleTree_BasicGenerateLocators();
			d = new Canvas();
			txt1 = new TextBlock();
			txt2 = new TextBlock();

			b.Children.Add(d);
			d.Children.Add(txt1);
			d.Children.Add(txt2);
			//LocatorManager.SetSubTreeProcessorId(d, TextFingerprintProcessor.Id);

			txt1.Text = "This content is not really meaningful, except to generate TextFingerprints for testing purposes";

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(txt1, "e");
			DataIdProcessor.SetDataId(txt2, "f");

			numExpectedLocators = 1;
			numExpectedLocatorParts = 4;
		}


		protected void BuildMultiLevelTree_MultiPassGenerateLocators()
		{
			// Build the basic tree
			BuildComplexTree_BasicGenerateLocators();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(f, "f");
			DataIdProcessor.SetDataId(txt1, "g");

			numExpectedLocators = 1;
			numExpectedLocatorParts = 5;
		}

		protected void BuildUnnamedMultiLevelTree_MultiPassGenerateLocators()
		{
			// Build the basic tree
			a = new Canvas();
			b = new Canvas();
			c = new Canvas();
			d = new Canvas();
			e = new Canvas();
			txt1 = new TextBlock();
			f = new Canvas();

			txt1.Text = "The quick brown fox jumped over the lazy dog";

			EnableService(a, annotationStore);

			a.Children.Add(b);
			a.Children.Add(c);
			b.Children.Add(d);
			b.Children.Add(e);
			e.Children.Add(f);
			e.Children.Add(txt1);

			numExpectedLocators = 1;
			numExpectedLocatorParts = 5;
		}

		protected void BuildNamedMultiLevelTree_MultiPassGenerateLocators()
		{
			// Build the basic tree
			BuildUnnamedMultiLevelTree_MultiPassGenerateLocators();

			DataIdProcessor.SetDataId(a, "a");
			DataIdProcessor.SetDataId(b, "b");
			DataIdProcessor.SetDataId(c, "c");
			DataIdProcessor.SetDataId(d, "d");
			DataIdProcessor.SetDataId(e, "e");
			DataIdProcessor.SetDataId(f, "f");
			DataIdProcessor.SetDataId(txt1, "g");

			numExpectedLocators = 1;
		}


		// ---------------------------------------------------------------------------------------------
		//									HELPER METHODS
		// ---------------------------------------------------------------------------------------------

		protected void VerifyLocatorsAreEqual(ContentLocatorBase l1, ContentLocatorBase l2)
		{
			if (AnchoringAPIHelpers.AreLocatorPartListsEqual(locator1, locator2))
				passTest("Valid number of locators & locator parts, selection1 = selection2, locators1 = locators2");
			else
				failTest("Locators are not equal");
		}

		protected void VerifyLocatorList(IList<ContentLocatorBase> locators, int expectedLocatorCount)
		{
			if (locators != null && locators.Count == expectedLocatorCount)
				passTest(expectedLocatorCount + " locators expected and generated");
			else if (locators == null)
				failTest(expectedLocatorCount + " locators expected.  Null list returned");
			else
				failTest(expectedLocatorCount + " locators expected.  " + locators.Count + " locators generated");
		}

	}		// end of AGenerateAndResolveSuite class

}			// end of namespace

