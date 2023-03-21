// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.IO;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
using System.Windows.Annotations.Storage;
using Annotations.Test.Framework;						// TestSuite.

using Proxies.MS.Internal.Annotations;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Abstract base class for all AnchoringAPI tests.
	/// Contains common helper methods specific to Anchoring test cases
	/// </summary>
	public abstract class AAnchoringAPITests : TestSuite
	{
		#region globals
		// LocatorManager to test in this class or its subclasses
		protected LocatorManager manager;

		// Store to use in each test case
		protected AnnotationStore annotationStore;
		private const string STORENAME = ".\\Anchoring_store.xml";
		#endregion

		/// <summary>
		/// Cleans up and initializes the AnnotationStore used by the test cases.  If the
		/// test being run is not the constructor test, the global LocatorManager is
		/// initialized in here as well.
		/// </summary>
		[TestCase_Setup()]
        protected void  DoSetup()
		{
			// Initialize the AnnotationStore
			String storeName = STORENAME;

			// Cleanup from previous run of this test
			FileInfo fi = new FileInfo(storeName);
			fi.Delete();
			annotationStore = AnnotationTestHelper.CreateStore(storeName);
			annotationStore.AutoFlush = true;

			// Initialize the global LocatorManager here, if we are not running the constructor test case
			if (!(CaseNumber.StartsWith("constructor")))
			{
				try
				{
					manager = new LocatorManager();

					manager.RegisterSubTreeProcessor(new DataIdProcessor(manager), DataIdProcessor.Id);
					manager.RegisterSubTreeProcessor(new FixedPageProcessor(manager), FixedPageProcessor.Id);
					//_manager.RegisterSubTreeProcessor(new TextFingerprintProcessor(_manager), TextFingerprintProcessor.Id);
					manager.RegisterSelectionProcessor(new TreeNodeSelectionProcessor(), typeof(FrameworkElement));
					manager.RegisterSelectionProcessor(new TreeNodeSelectionProcessor(), typeof(FrameworkContentElement));
					manager.RegisterSelectionProcessor(new TextSelectionProcessor(), typeof(TextRange));
				}
				catch (Exception E)
				{
					failTest("Exception thrown constructing LocatorManager:  " + E.ToString());
				}
			}
		}

        //protected override void CleanupVariation()
        //{
        //    //annotationStore.Flush();
        //    //annotationStore.Dispose();
        //    annotationStore = null;
        //    base.CleanupVariation();
        //}

		protected void EnableService(DependencyObject target, AnnotationStore store)
		{
			new AnnotationService(target).Enable(store);
		}

		/// <summary>
		/// Generates Locators for a text selection that spans multiple adjacent text elements
		/// specifically Paragraphs (framework only works with Paragraphs for now).  Resolves the 
		/// ContentLocatorBase and checks based on the expected AttachmentLevel whether the resolved
		/// anchor is appropriate.  At each stage, ContentLocatorBase sanity checks are performed.
		/// </summary>
		/// <param name="start">TextPointer to mark start of text selection</param>
		/// <param name="end">TextPointer to mark end of text selection</param>
		/// <param name="moveStart"># chars to move start point by</param>
		/// <param name="moveEnd"># chars to move end point by</param>
		/// <param name="numLocators"># LocatorPartLists generated for the selection</param>
		/// <param name="numLocatorParts"># ContentLocatorParts composing each generated ContentLocatorBase</param>
		/// <param name="treeNode">DependencyObject to start resolving Locators from</param>
		/// <param name="anchorType">AttachmentLevel expected from resolving the Locators</param>
		/// <returns>true if the checks executed successfully, false if any check failed</returns>
		protected bool GenerateAndResolveLocatorSets(
			TextPointer selectionStartPointer,
			TextPointer selectionEndPointer,
			int startOffset,
			int endOffset,
			int expectedNumLocators,
			int expectedNumLocatorParts,
			DependencyObject startNode,
			AttachmentLevel expectedAttachmentLevel)
		{
			// Create TextRange for the selection and generate locators for it
			TextPointer adjustedStart = selectionStartPointer;
			adjustedStart = adjustedStart.GetPositionAtOffset(startOffset);
			TextPointer adjustedEnd = selectionEndPointer;
			adjustedEnd = adjustedEnd.GetPositionAtOffset(endOffset);
			TextRange txtSelection = new TextRange(adjustedStart, adjustedEnd);

			IList<ContentLocatorBase> LocatorSets = manager.GenerateLocators(txtSelection);
			AssertNotNull("Verify Locators not null.", LocatorSets);
			AssertEquals("Verify number of Locators generated.", 1, LocatorSets.Count);

			// Verify the number of Locators in the group.
			ContentLocatorGroup locatorGroup = LocatorSets[0] as ContentLocatorGroup;
			AssertEquals("Verify number of Locators.", expectedNumLocators, locatorGroup.Locators.Count);

			// Check the number of ContentLocatorParts in each individual ContentLocator
			IEnumerator<ContentLocator> locators = locatorGroup.Locators.GetEnumerator();
			while (locators.MoveNext())
			{
				AssertEquals("Verify num ContentLocatorParts for each ContentLocator.", expectedNumLocatorParts, locators.Current.Parts.Count);
			}

			// Resolve
			ResolveAndCheckLocators(txtSelection, locatorGroup, 0, startNode, expectedAttachmentLevel);
			return true;
		}

		/// <summary>
		/// Generates Locators for a given anchor, and checks ContentLocatorBase and ContentLocatorPart count.
		/// </summary>
		/// <param name="selection">TextRange/FrameworkElement anchor for annotation</param>
		/// <param name="numLocators"># Locators expected to be generated</param>
		/// <param name="numLocatorParts"># ContentLocatorParts expected per ContentLocatorBase generated</param>
		/// <returns>true if the ContentLocatorBase generated has the expected number of locators and parts.
		/// Returns the ContentLocator as an out parameter.</returns>
		protected bool GenerateAndCheckLocators(object selection, int numLocators, int numLocatorParts, out ContentLocator locator)
		{
			IList<ContentLocatorBase> locators = null;

			locators = manager.GenerateLocators(selection);
			locator = locators[0] as ContentLocator;

			if (locators.Count != numLocators)
			{
				failTest("Invalid number of locators");
				return false;
			}

			if (locator.Parts.Count != numLocatorParts)
			{
				failTest("Invalid number of locator parts");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Resolves the generated Locators for a given anchor and based on the expected
		/// AttachmentLevel, verifies if the Locators resolve back to the appropriate anchor
		/// or tree node with the proper anchor type.
		/// </summary>
		/// <param name="anchor">Original anchor that Locators were generated for</param>
		/// <param name="loc">ContentLocatorBase generated by the GenerateLocators method</param>
		/// <param name="offset">index of ContentLocatorPart that we want to start looking for</param>
		/// <param name="startNode"></param>
		/// <param name="anchorType">AttachmentLevel indicating the degree of resolution</param>
		/// <returns>true if resolving ContentLocatorBase produced the appropriate anchor and attachment level,
		/// otherwise false</returns>
		protected bool ResolveAndCheckLocators(object anchor, ContentLocatorBase loc, int offset, DependencyObject startNode, AttachmentLevel anchorType)
		{
			object ignored;
			return ResolveAndCheckLocators(anchor, loc, offset, startNode, anchorType, out ignored);
		}

		/// <summary>
		/// Resolves the generated Locators for a given anchor and based on the expected
		/// AttachmentLevel, verifies if the Locators resolve back to the appropriate anchor
		/// or tree node with the proper anchor type.
		/// </summary>
		/// <param name="anchor">Original anchor that locators were generated for</param>
		/// <param name="loc">ContentLocatorBase generated by the GenerateLocators method</param>
		/// <param name="offset">index of ContentLocatorPart that we want to start looking for</param>
		/// <param name="startNode"></param>
		/// <param name="anchorType">AttachmentLevel indicating the degree of resolution</param>
		/// <param name="anchorReturned">Anchor produced by resolving the given ContentLocatorBase</param>
		/// <returns>true if resolving ContentLocatorBase produced the appropriate anchor and attachment level,
		/// otherwise false</returns>
		protected bool ResolveAndCheckLocators(
			object expectedAnchor,
			ContentLocatorBase loc,
			int offset,
			DependencyObject startNode,
			AttachmentLevel expectedAttachmentLevel,
			out object anchorReturned)
		{
			AttachmentLevel actualAttachementLevel;
			anchorReturned = manager.ResolveLocator(loc, offset, startNode, out actualAttachementLevel);

            if (AnnotationTestHelper.IsTextAnchor(expectedAnchor))
			{
				Assert("Verify anchors are equal.", expectedAnchor.Equals(anchorReturned));
			}
			else
			{
				Assert("Verify anchor identity.", anchorReturned == expectedAnchor);
			}
			AssertEquals("Verify AttachmentLevel.", expectedAttachmentLevel, actualAttachementLevel);

			return true;
		}

		/// <summary>
		/// Checks that the anchor returned by ResolveLocator is null and that the 
		/// AttachmentLevel returned equals Unresolved
		/// </summary>
		/// <param name="anchor">object returned by ResolveLocator</param>
		/// <param name="anchorType">AttachmentLevel returned by ResolveLocator</param>
		protected void VerifyAnchorIsUnresolved(object anchor, AttachmentLevel anchorType)
		{
			if (anchor == null && anchorType == AttachmentLevel.Unresolved)
				passTest("Unresolved anchor (anchor == null) correctly returned");
			else
				failTest("Unresolved anchor (anchor == null) expected but not returned");
		}

		/// <summary>
		/// Checks that the anchor returned by ResolveLocator is the original selection
		/// and that the AttachmentLevel returned equals Full
		/// </summary>
		/// <param name="anchor">object returned by ResolveLocator</param>
		/// <param name="selection">original selection</param>
		/// <param name="anchorType">AttachmentLevel returned by ResolveLocator</param>
		protected void VerifyAnchorIsFullyResolved(object anchor, object selection, AttachmentLevel anchorType)
		{
			TextRange trAnchor = anchor as TextRange;
			Assert("verify anchor is what was expected.", anchor.Equals(selection));
            AssertEquals("Verify attachment level.", AttachmentLevel.Full, anchorType);
            passTest("Verified anchor.");
		}
	}
}

