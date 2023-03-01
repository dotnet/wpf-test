// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;

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
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Windows.Controls;
using System.IO;
using System.Windows;
using System.Collections;
using System.Collections.Generic;					// TestSuite.
using System.Threading;
using System.Windows.Threading;
using System.Xml;
using System.Globalization;
using Proxies.MS.Internal.Annotations;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Annotations.Test;


/// <summary>
/// FixedTextSelectionProcessorSuite provides the implementation for the API tests for 
/// FixedTextSelectionProcessor's public methods.
/// </summary>
namespace Avalon.Test.Annotations.Suites
{
	public class FixedTextSelectionProcessorSuite : AFixedTextSelectionProcessorSuite
    {
        #region BVT TESTS

        #region Constructors

        [Priority(0)]
        private void fixedtext_ctor1()
        {
            try
            {
                FixedTextSelectionProcessor ft = new FixedTextSelectionProcessor();
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  \n" + exp.ToString());
            }
            passTest("No exceptions expected");
        }

        #endregion Constructors

        #region MergeSelections Tests

        /// <summary>
        /// Parameters: endOfPage1 = valid, non-null TextAnchor on page N
        ///				startOfPage2 = valid, non-null TextAnchor on page N+1
        ///				*endOfPage1, startOfPage2 are adjacent
        /// Verify: newAnchor = TextAnchor with endOfPage1 and startOfPage2 concatenated
        /// </summary>
        [Priority(0)]
        private void fixedtext_mergeselections6()
        {
            object endOfPage1 = MakeTextAnchor(1, 3980, 29); // Last 29 characters of page 1.	
            object startOfPage2 = MakeTextAnchor(2, 0, 10);	// First 10 characters of page 2.

            processorTester.VerifyMergeSelections(endOfPage1, startOfPage2, true, AnnotationTestHelper.GetText(endOfPage1) + AnnotationTestHelper.GetText(startOfPage2));
            passTest("Merged adjacent selections on page boundry.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on page N
        ///				anchor2 = valid, non-null TextAnchor on page N + 1
        ///				*anchor1, selection2 are non-adjacent	
        /// Verify: newAnchor = TextAnchor containing anchor1, all intermediate characters and anchor2 
        /// </summary>
        [Priority(0)]
        private void fixedtext_mergeselections7()
        {
            // Create NON-ADJACENT selections on consecutive pages.
            object anchor1 = MakeTextAnchor(1, 3980, 4);
            object anchor2 = MakeTextAnchor(2, 10, 7);

            string expectedText = GetText(new SimpleSelectionData(1, 3980, 4)) + GetText(new SimpleSelectionData(2, 10, 7));

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, expectedText);
            passTest("Merged non-adjacent selections across page boundry.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on page N
        ///				anchor2 = valid, non-null TextAnchor on page N
        ///				* anchor1, anchor2 overlap	
        /// Verify: newAnchor = TextAnchor containing anchor1, anchor2
        /// </summary>
        [Priority(0)]
        private void fixedtext_mergeselections9()
        {
            object anchor1 = MakeTextAnchor(0, 388, 11);
            object anchor2 = MakeTextAnchor(0, 397, 23);

            string expectedText = MakeTextRange(0, 388, 11).Text + MakeTextRange(0, 397, 23).Text;

            processorTester.VerifyMergeSelectionsFailed(anchor1, anchor2, typeof(InvalidOperationException));
            passTest("Merged overlapping selections on one page.");
        }

        /// <summary>
        /// Parameters: superAnchor = valid, non-null TextAnchor on page N 
        ///				subsetAnchor = valid, non-null TextAnchor on page N to N+1
        ///				* subsetAnchor is fully contained in superAnchor	
        /// Verify: newAnchor = TextAnchor containing subsetAnchor
        /// </summary>
        [Priority(0)]
        private void fixedtext_mergeselections10()
        {
            object superAnchor = MakeTextAnchor(3, 490, 66); // "right side (rather than at thebottom of the page) and is large eno"
            object subsetAnchor = MakeTextAnchor(3, 505, 38); // "her than at thebottom of the page) and"

            processorTester.VerifyMergeSelectionsFailed(superAnchor, subsetAnchor, typeof(InvalidOperationException));
            passTest("Merged overlapping selections on one page.");
        }

        #endregion MergeSelections Tests

        #region GetSelectedNodes Tests

        /// <summary>
        /// Bugs: 1067269 and 1067275
        /// 
        /// Parameters: selection = starts on page N, ends on page N+3	
        /// Verify: List of FixedPageProxy objects for pages N, N+1, N+2, N+3
        /// </summary>
        [Priority(0)]
        private void fixedtext_getselectednodes5()
        {
            TextRange selection = SetSelection(1, 3900, 6100); // Selection starts at end of Page1 and ends in middle of Page4.
            processorTester.VerifyGetSelectedNodes(selection, new object[] { 1, 2, 3, 4 });
            passTest("SelectedNodes verified for 4 page selection.");
        }

        #endregion GetSelectedNodes Tests

        #region GenerateLocatorParts Tests

        /// <summary>
        /// Parameters: selection = valid text selection (fully contained on one page)
        ///				startNode = FixedPageProxy for that page	
        /// Verify: List with single FixedTextRange for that selection
        /// </summary>
        [Priority(0)]
        private void fixedtext_generatelocatorparts4()
        {
            TextRange selection = SetSelection(0, 50, 19);
            FixedPageProxy page0 = new FixedPageProxy(ViewerBase, 0);
            PointSegment ps = new PointSegment(new Point(359.1272, 54.48841796875), new Point(713.9144, 999.80666015625));
            page0.AddSegment(ps);

            IList<ContentLocatorPart> locatorParts = fixedTextSelectionProcessor.GenerateLocatorParts(selection, page0);

            AssertNotNull("Verify locator parts are not null.", locatorParts);
            AssertEquals("Verify number of locator parts.", 1, locatorParts.Count);

            string[] expectedKeys = new string[] { "Segment0", "Count" };
            string[] expectedValues = new string[] { "359.1272,54.48841796875,713.9144,999.80666015625", "1" };
            ContentLocatorPart lp = locatorParts[0];
            AssertEquals("Verify number of Values in ContentLocatorPart.", expectedKeys.Length, lp.NameValuePairs.Count);
            for (int i = 0; i < expectedKeys.Length; i++)
                AssertEquals("Verify value for " + expectedKeys[i] + ".", expectedValues[i], lp.NameValuePairs[expectedKeys[i]]);

            passTest("ContentLocatorParts generated correctly for selection on single page.");
        }

        #endregion GenerateLocatorParts Tests

        #region ResolveLocatorPart Tests

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart for selection contained fully on one page
        ///				startNode = FixedPage for selection described by ContentLocatorPart, visible in DocumentViewer	
        /// Verify: Portion of selection on startNode�s FixedPage 
        /// </summary>
        [Priority(0)]
        private void fixedtext_resolvelocatorpart4()
        {
            String expectedText = "care Industries\r\nRes";
            TextRange selection = SetSelection(0, 50, 19);
            printStatus("Actual selection = '" + selection.Text + "'.");
            FixedPage page0 = GetFixedPage(0);

            ContentLocatorPart lp = new ContentLocatorPart(new XmlQualifiedName("FixedTextRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            lp.NameValuePairs.Add("Segment0", "365.9368,54.48841796875,125.8656,124.33759765625");
            lp.NameValuePairs.Add("Count", "1");

            AttachmentLevel attachmentLevel;
            object anchor = fixedTextSelectionProcessor.ResolveLocatorPart(lp, page0, out attachmentLevel);

            AssertNotNull("Verify ContentLocatorPart resolved.", anchor);
            AssertEquals("Verify attachmentLevel.", AttachmentLevel.Full, attachmentLevel);
            AssertEquals("Verify resolved range.", SelectionModule.PrintFriendlySelection(expectedText), SelectionModule.PrintFriendlySelection(AnnotationTestHelper.GetText(anchor)));

            passTest("ContentLocatorPart resolved.");
        }


        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart for multi-page selection portion on inner page
        ///				startNode = FixedPage for inner page of selection	
        /// Verify: TextAnchor for portion of anchor on the inner page is returned
        /// </summary>
        [Priority(0)]
        private void fixedtext_resolvelocatorpart9()
        {
            DocViewerWrapper.SetZoom(10.0);
            VerifyPageIsVisible(1);
            VerifyPageIsVisible(3);

            TextRange selection = SetSelection(1, 3900, 3600); // Selection from page 1 to page 3.

            IList<DependencyObject> nodes = fixedTextSelectionProcessor.GetSelectedNodes(selection);
            AssertEquals("Verify number of pages selection spans.", 3, nodes.Count);
            IList<ContentLocatorPart> lps = fixedTextSelectionProcessor.GenerateLocatorParts(selection, nodes[1]);
            AssertEquals("Verify num ContentLocatorParts.", 1, lps.Count);

            AttachmentLevel attachmentLevel;
            object anchor = fixedTextSelectionProcessor.ResolveLocatorPart(lps[0], GetFixedPage(2), out attachmentLevel);
            AssertNotNull("Verify ContentLocatorPart resolved.", anchor);
            AssertEquals("Verify AttachmentLevel.", AttachmentLevel.Full, attachmentLevel);

            // Text is long so just check the length and start and end segments.
            string anchorText = AnnotationTestHelper.GetText(anchor);
            printStatus("Resolved text = " + SelectionModule.PrintFriendlySelection(anchorText));
            Assert("Verify start of Text.", anchorText.StartsWith("Microsoft Office for the Pharmaceutical and Healthcare Industries\r\nThe 2003 release of Microsoft Office is a system"));
            Assert("Verify end of Text.", anchorText.EndsWith("threaded conversation, assign follow-up flags with a single click, glance at\r\n-3-"));
            AssertEquals("Verify length.", 3447, anchorText.Length);

            passTest("ContentLocatorPart for inner page resolved.");
        }

        #endregion ResolveLocatorPart Tests

        #region GetLocatorPartTypes Tests

        [Priority(0)]
        private void fixedtext_getlocatorparttypes1()
        {
            XmlQualifiedName[] result = fixedTextSelectionProcessor.GetLocatorPartTypes();
            AssertEquals("Verify number of types.", 1, result.Length);
            AssertEquals("Verify type.", new XmlQualifiedName("FixedTextRange", "http://schemas.microsoft.com/windows/annotations/2003/11/base"), result[0]);
            passTest("Verified GetLocatorPartTypes");
        }

        #endregion GetLocatorPartTypes Tests

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Constructor Tests

        /// <summary>
        /// Parameters: none
        /// Verify: no exceptions are thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_ctor2()
        {
            try
            {
                FixedTextSelectionProcessor ft = new FixedTextSelectionProcessor();
            }
            catch (Exception exp)
            {
                failTest("Unexpected exception:  \n" + exp.ToString());
            }
            passTest("No exceptions expected");
        }

        #endregion Constructor Tests

        #region MergeSelections Tests

        /// <summary>
        /// Parameters: anchor1 = null
        ///				anchor2 = valid, non-null TextAnchor
        /// Verify: newAnchor = anchor2
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections1()
        {
            object anchor1 = null;
            object anchor2 = MakeTextAnchor(0, 0, 10);

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, AnnotationTestHelper.GetText(anchor2));
            passTest("Merged null and valid selection.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor
        ///				anchor2 = null
        /// Verify: newAnchor = anchor1
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections2()
        {
            object anchor1 = MakeTextAnchor(1, 3894, 115);
            object anchor2 = null;

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, AnnotationTestHelper.GetText(anchor1));
            passTest("Merged valid selection and null.");
        }

        /// <summary>
        /// Parameters: anchor1 = null
        ///				anchor2 = null	
        /// Verify: newAnchor = null 
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections3()
        {
            object anchor1 = null;
            object anchor2 = null;

            processorTester.VerifyMergeSelections(anchor1, anchor2, false, null);
            passTest("Merged null selections.");
        }

        /// <summary>
        /// Parameters: anchor1 = not an TextAnchor
        ///				anchor2 = valid, non-null TextAnchor
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections4()
        {
            object anchor1 = new Canvas();
            object anchor2 = MakeTextAnchor(1, 14, 500);

            processorTester.VerifyMergeSelectionsFailed(anchor1, anchor2, typeof(ArgumentException));
            passTest("Merged non-TextAnchor and TextAnchor failed.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor
        ///				anchor2 = not an TextAnchor	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections5()
        {
            object anchor1 = MakeTextAnchor(4, 1150, 70);
            object anchor2 = ViewerBase;

            processorTester.VerifyMergeSelectionsFailed(anchor1, anchor2, typeof(ArgumentException));
            passTest("Merged TextAnchor and non-TextAnchor failed.");
        }

        /// <summary>
        /// Parameters: anchor1 = anchor2 = valid, non-null TextAnchor on page N	
        /// Verify: newAnchor = anchor1 = anchor2
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections8()
        {
            object anchor1 = MakeTextAnchor(0, 0, 10);
            object anchor2 = MakeTextAnchor(0, 15, 20);

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, MakeTextRange(0, 0, 10).Text + MakeTextRange(0, 15, 20).Text);
            passTest("Merged selections on same page.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on page N to page N+1
        ///				anchor2 = valid, non-null TextAnchor on page N
        ///				*anchor2 is fully contained in anchor1	
        /// Verify: newAnchor = TextAnchor containing anchor1
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections11()
        {
            object anchor1 = MakeTextAnchor(3, 1600, 100);
            object anchor2 = MakeTextAnchor(3, 1650, 25);

            processorTester.VerifyMergeSelectionsFailed(anchor1, anchor2, typeof(InvalidOperationException));
            passTest("Merged selection2 subset of selection1.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on page N
        ///				anchor2 = valid, non-null TextAnchor on page N+1
        ///				* anchor1, anchor2 are non-adjacent with other objects in between them (i.e. Image, Hyperlink, etc.)	
        /// Verify: newAnchor = TextAnchor containing anchor1 and anchor2 (as well as the characters and objects they span)
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections12()
        {
            //string imageDef = "<Image";

            object result;
            fixedTextSelectionProcessor.MergeSelections(MakeTextAnchor(0, 500, 10), MakeTextAnchor(1, 10, 19), out result);

            /* need to change this to get the start from something else
                        TextRange newSelection = result as TextRange;
                        AssertNotNull("Verify result is initialized.", result);
            
                        TextPointer pointer = newSelection.Start;
                        bool pass = false;
                        while (pointer.CompareTo(newSelection.End) != 0)
                        {
                            InlineUIContainer container = pointer.GetAdjacentElement(LogicalDirection.Forward) as InlineUIContainer;
                            if (container != null && container.Child is System.Windows.Controls.Image)
                            {
                                pass = true;
                                break;
                            }
                            pointer.GetNextContextPosition(LogicalDirection.Forward);
                        }
                        Assert("Verify resulting selection contains expected image.", pass);
            */
            passTest("Merged selections with image between them.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on page N
        ///				anchor2 = valid, non-null TextAnchor on page N+3
        ///				*anchor1, anchor2 have at least 1 pagebreak in between them	
        /// Verify: newAnchor = TextAnchor spanning the start of anchor1 to the end of anchor2
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections13()
        {
            processorTester.VerifyMergeSelections(
                MakeTextAnchor(0, 0, 100),
                MakeTextAnchor(4, 10, 25),
                true,
                GetText(new SimpleSelectionData(0, 0, 100)) + GetText(new SimpleSelectionData(4, 10, 25)));

            passTest("Merged selections multiple pages apart.");
        }

        /// <summary>
        /// Parameters: anchor1 = valid, non-null TextAnchor on first page
        ///				anchor2 = valid, non-null TextAnchor on last page
        ///				*anchor1 and anchor2 combined cover the entire document	
        /// Verify: newAnchor = TextAnchor spanning the entire document
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections14()
        {
            processorTester.VerifyMergeSelections(
                MakeTextAnchor(0, 0, 100),
                MakeTextAnchor(4, 1150, 70),
                true,
                MakeTextRange(0, 0, 100).Text + MakeTextRange(4, 1150, 70).Text);

            passTest("Merged selections at document start and end.");
        }

        /// <summary>
        /// Parameters: anchor1 = null
        ///				anchor2 = Zero length TextAnchor
        /// Verify: zero length text range.
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections15()
        {
            object anchor1 = null;
            object anchor2 = MakeTextAnchor(3, 928, 0);

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, "");
            passTest("Merged null with empty selection.");
        }

        /// <summary>
        /// Parameters: anchor1 = Zero length TextAnchor
        ///				anchor2 = null
        /// Verify: zero length text range.
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections15_1()
        {
            object anchor1 = MakeTextAnchor(4, 0, 0);
            object anchor2 = null;

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, "");
            passTest("Merged empty selection with null.");
        }

        /// <summary>
        /// Parameters: anchor1=non-null TextAnchor
        ///				anchor2=TextAnchor with length 0	
        /// Verify: TextAnchor from start of Selection1 to Selection2.
        /// </summary>
        [Priority(1)]
        private void fixedtext_mergeselections16()
        {
            object anchor1 = MakeTextAnchor(1, 14, 3);
            object anchor2 = MakeTextAnchor(1, 30, 0); // Empty Selection.

            processorTester.VerifyMergeSelections(anchor1, anchor2, true, MakeTextRange(1, 14, 3).Text);
            passTest("Merged non-empty selection with empty selection.");
        }


        #endregion MergeSelections Tests

        #region GetSelectedNodes Tests

        /// <summary>
        /// Parameters: selection = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes1()
        {
            processorTester.VerifyGetSelectedNodesFails(null, typeof(ArgumentNullException));
            passTest("Exception for null selection.");
        }

        /// <summary>
        /// Parameters: selection = not an TextRange	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes2()
        {
            processorTester.VerifyGetSelectedNodesFails(new TextBlock(), typeof(ArgumentException));
            passTest("Exception for non-ITextRange selection.");
        }

        /// <summary>
        /// Parameters: selection = fully contained on one page	List of single 
        /// Verify: FixedPageProxy object for page N
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes3()
        {
            processorTester.VerifyGetSelectedNodes(SetSelection(2, 15, 200), new object[] { 2 });
            passTest("Selection on page 2.");
        }

        /// <summary>
        /// Parameters: selection = starts on page N, ends on page N+1	
        /// Verify: List of FixedPageProxy objects for pages N, N+1 
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes4()
        {
            processorTester.VerifyGetSelectedNodes(SetSelection(2, 3300, 500), new object[] { 2, 3 });
            passTest("Selection on page 2 and 3.");
        }

        /// <summary>
        /// Parameters: selection = starts on page N+1, ends on page N	
        /// Verify: List of FixedPageProxy objects for pages N, N+1
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes6()
        {
            processorTester.VerifyGetSelectedNodes(SetSelection(3, 10, -50), new object[] { 2, 3 });
            passTest("Selection on page 3 and 2.");
        }

        /// <summary>
        /// 
        /// Parameters: selection = ITextRange not directly parented by DocumentViewer (i.e. TextRange in a TextPanel located next to the DocumentViewer) 	
        /// Verify: ArgumentException
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes7()
        {
            TextBox tb = new TextBox();
            tb.Text = "hello world";
            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
            {
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            }
            TextRange TextRange = TextSelectionInfo.GetValue(tb, null) as TextRange;
            TextRange selection = new TextRange(TextRange.Start, TextRange.End);
            processorTester.VerifyGetSelectedNodesFails(selection, typeof(ArgumentException));
            passTest("ArgumentException for unparented TextRange.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange that contains some non-text content (i.e. Image)	
        /// Verify: List of FixedPageProxy objects for pages spanned by the selection
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes8()
        {
            processorTester.VerifyGetSelectedNodes(DocViewerWrapper.SetSelection(0, 100, 2, 500), new object[] { 0, 1, 2 });
            passTest("Selection with image in it.");
        }

        /// <summary>
        /// 
        /// Parameters: selection = ITextRange that also covers portions of non-visible pages 
        /// Verify: FixedPageProxy for non-visible page.
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes9()
        {
            DocViewerWrapper.SetZoom(100.0);
            queueTask(new DispatcherOperationCallback(fixedtext_getselectednodes9_stage1), null);
        }

        [Priority(1)]
        private object fixedtext_getselectednodes9_stage1(object arg)
        {
            Assert("Verify page 4 is not visible.", DocViewerWrapper.LastVisiblePage < 4);
            processorTester.VerifyGetSelectedNodes(SetSelection(4, 90, 15), new object[] { 4 });
            passTest("Selection on non-visible page.");
            return null;
        }

        /// <summary>
        /// Parameters: selection = ITextRange that covers text for the entire document	
        /// Verify: List of FixedPageProxy objects for every page in the document
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes10()
        {
            TextRange selection = DocViewerWrapper.SetSelection(0, PagePosition.Beginning, 4, PagePosition.End);
            processorTester.VerifyGetSelectedNodes(selection, new object[] { 0, 1, 2, 3, 4 });
            passTest("Selection for entire document.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange at the start of the document	
        /// Verify: List of FixedPageProxy objects for the selection (must have page 1)
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes11()
        {
            TextRange selection = DocViewerWrapper.SetSelection(0, 0, 10);
            processorTester.VerifyGetSelectedNodes(selection, new object[] { 0 });
            passTest("Selection for start of document.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange at the end of the document	
        /// Verify: List of FixedPageProxy objects for the selection (must have last page)
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes12()
        {
            TextRange selection = DocViewerWrapper.SetSelection(4, PagePosition.End, -10);
            processorTester.VerifyGetSelectedNodes(selection, new object[] { 4 });
            passTest("Selection for end of document.");
        }

        /// <summary>
        /// Parameters: selection = empty ITextRange with length = 0	
        /// Verify: FixePageProxy for page that ITextRange exists on.
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes13()
        {
            TextRange selection = DocViewerWrapper.SetSelection(2, 692, 0);
            processorTester.VerifyGetSelectedNodes(selection, new object[] { 2 });
            passTest("Zero length Selection.");
        }

        /// <summary>
        /// 
        /// Parameters: selection that starts on page N and ends on N+1, with page N non-visible	
        /// Verify: List of FixedPageProxy object for N and N+1
        /// </summary>
        [Priority(1)]
        private void fixedtext_getselectednodes14()
        {
            TextRange selection = DocViewerWrapper.SetSelection(1, 400, 2, 529);

            AsyncTestScript script = new AsyncTestScript();
            script.Add(DocViewerWrapper, "SetZoom", new object[] { 100.0 });
            script.Add(DocViewerWrapper, "WholePageLayout", null);
            script.Add(DocViewerWrapper, "PageDown", null);
            script.Add(DocViewerWrapper, "ScrollDown", new object[] { 6 });
            script.Add("VerifyVisiblePages", new object[] { 1, 2 });
            script.Add(processorTester, "VerifyGetSelectedNodes", new object[] { selection, new object[] { 1, 2 } });
            new AsyncTestScriptRunner(this).Run(script);
        }

        #endregion GetSelectedNodes Tests

        #region GetParent Tests

        /// <summary>
        /// Parameters: selection = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent1()
        {
            processorTester.VerifyGetParentFails(null, typeof(ArgumentNullException));
            passTest("Exception for null.");
        }

        /// <summary>
        /// Parameters: selection = not an ITextRange	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent2()
        {
            processorTester.VerifyGetParentFails("foobar", typeof(ArgumentException));
            passTest("Exception for non-ITextRange.");
        }

        /// <summary>
        /// Parameters: selection = fully contained on one page	
        /// Verify: FixedPage for selection is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent3()
        {
            object anchor = MakeTextAnchor(0, 500, 250);
            processorTester.VerifyGetParent(anchor, GetDocumentPageHost(0));
            passTest("Selection on 0 page.");
        }

        /// <summary>
        /// Parameters: selection = spanning > 1 page	
        /// Verify: FixedPage for first page (start) of selection is returned 
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent4()
        {
            object selection = DocViewerWrapper.SetSelection(2, PagePosition.Beginning, 3, PagePosition.End);
            object anchor = MakeTextAnchor((TextRange)selection);
            DocViewerWrapper.SetZoom(10);
            VerifyPageIsVisible(2);
            VerifyPageIsVisible(3);
            processorTester.VerifyGetParent(anchor, GetDocumentPageHost(2));
            passTest("selection spanning > 1 page.");
        }

        /// <summary>
        /// Parameters: selection = spanning > 1 page, with end < start	
        /// Verify: FixedPage for first page (start) of selection is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent5()
        {
            object selection = DocViewerWrapper.SetSelection(4, PagePosition.End, 3, PagePosition.Beginning);
            object anchor = MakeTextAnchor((TextRange)selection);
            DocViewerWrapper.PageLayout(5);
            VerifyPageIsVisible(3);
            VerifyPageIsVisible(4);
            processorTester.VerifyGetParent(anchor, GetDocumentPageHost(3));
            passTest("Backwards multipage.");
        }

        /// <summary>
        /// 
        /// Parameters: selection = ITextRange is not directly parented by DocumentViewer�s PageGridElement	
        /// Verify: ArgumentException
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent6()
        {
            TextBox box = new TextBox();
            box.Text = "I am text.";

            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            TextRange TextRange = TextSelectionInfo.GetValue(box, null) as TextRange;
            object selection = new TextRange(TextRange.Start, TextRange.End);
            processorTester.VerifyGetParentFails(selection, typeof(ArgumentException));

            passTest("Exception for TextRange outside of DocumentViewer.");
        }

        /// <summary>
        /// 
        /// Parameters: selection=ITextRange on non-visible page.
        /// Verify: FixedPage for the start of the selection is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent7()
        {
            object anchor = MakeTextAnchor(1, 100, 20);
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown(2);
            VerifyPageIsNotVisible(1);
            processorTester.VerifyGetParent(anchor, null);
            passTest("GetParent for non-visible page.");
        }

        /// <summary>
        /// 
        /// Parameters: Selection=ITextRange on > 1 page with start on non-visible page.
        /// Verify: null returned.
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent8()
        {
            object selection = DocViewerWrapper.SetSelection(1, 3000, 2, 100);
            object anchor = MakeTextAnchor((TextRange)selection);
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown(2);
            VerifyPageIsNotVisible(1);
            VerifyPageIsVisible(2);
            processorTester.VerifyGetParent(anchor, null);
            passTest("GetParent for non-visible start page.");
        }

        /// <summary>
        /// Parameters: Selection=ITextRange on > 1 page with start on visible page.
        /// Verify: FixedPage for the start of the selection is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getparent9()
        {
            object selection = DocViewerWrapper.SetSelection(3, 200, 4, 5);
            object anchor = MakeTextAnchor((TextRange)selection);
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.GoToPageRange(3, 4);
            processorTester.VerifyGetParent(anchor, GetDocumentPageHost(3));
            passTest("GetParent for multipage (visible) selection.");
        }

        #endregion GetParent Tests

        #region GetAnchorPoint

        /// <summary>
        /// Parameters: selection = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint1()
        {
            processorTester.VerifyGetAnchorPointFails(null, typeof(ArgumentNullException));
            passTest("Exception for null selection.");
        }

        /// <summary>
        /// Parameters: selection = not an ITextRange	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint2()
        {
            processorTester.VerifyGetAnchorPointFails(true, typeof(ArgumentException));
            passTest("Exception for Non-TextRange selection.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange located at the start of the page	
        /// Verify: Start point (non-null) is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint3()
        {
            processorTester.VerifyGetAnchorPoint(MakeTextAnchor(1, 0, 500), new Point(96, 60.953984375));
            passTest("Selection starting at beginning of page.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange located in the middle of the page	
        /// Verify: Startpoint in the middle of the page is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint4()
        {
            processorTester.VerifyGetAnchorPoint(MakeTextAnchor(1, 1698, 200), new Point(382.30479999999994, 436.245078125));
            passTest("Selection starting in middle of page.");
        }

        /// <summary>
        /// Parameters: selection = ITextRange located at the end of the page
        /// Verify: Start point at the end of the page is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint5()
        {
            processorTester.VerifyGetAnchorPoint(MakeTextAnchor(1, PagePosition.End, -1), new Point(713.9144, 1007.925078125));
            passTest("Selection starting in end of page.");
        }

        /// <summary>
        /// Parameters: Selection=non-visible page
        /// Verify: Start point on non-visible page.
        /// </summary>
        [Priority(1)]
        private void fixedtext_getanchorpoint6()
        {
            DocViewerWrapper.SetZoom(100);
            DocViewerWrapper.WholePageLayout();
            DocViewerWrapper.PageDown(2);
            VerifyPageIsNotVisible(0);
            processorTester.VerifyGetAnchorPoint(MakeTextAnchor(0, 199, 25), new Point(290.78880000000004, 185.045078125));
            passTest("Start point on non-visible page.");
        }

        #endregion GetAnchorPoint

        #region GenerateLocatorParts Tests

        /// <summary>
        /// Parameters: selection = null
        ///				startNode = valid FixedPageProxy for page�s selection	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts1()
        {
            processorTester.VerifyGenerateLocatorPartsFails(null, new FixedPageProxy(ViewerBase, 0), typeof(ArgumentNullException));
            passTest("Exception for null selection.");
        }

        /// <summary>
        /// Parameters: selection = valid text selection on DocumentViewer contents
        ///				startNode = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts2()
        {
            processorTester.VerifyGenerateLocatorPartsFails(SetSelection(0, 10, 10), null, typeof(ArgumentNullException));
            passTest("Exception for null StartNode.");
        }

        /// <summary>
        /// Parameters: selection = valid text selection (spanning a single pagebreak)
        ///				startNode = FixedPageProxy for the first page	
        /// Verify: List with single ContentLocatorPart for the first page�s selection
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts5()
        {
            TextRange selection = DocViewerWrapper.SetSelection(1, PagePosition.End, -10, 2, PagePosition.Beginning, 200); // Page1 to Page2
            FixedPageProxy proxy = fixedTextSelectionProcessor.GetSelectedNodes(selection)[0] as FixedPageProxy;
            processorTester.VerifyGenerateLocatorParts(selection, proxy);
            passTest("Selection first of 2 pages.");
        }

        /// <summary>
        /// Parameters: selection = valid text selection (spanning a single pagebreak)
        ///				startNode = FixedPageProxy for the last page	
        /// Verify: List with single ContentLocatorPart for the last page�s selection
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts6()
        {
            TextRange selection = DocViewerWrapper.SetSelection(3, PagePosition.End, -300, 4, PagePosition.Beginning, 10); // Page3 to Page4
            FixedPageProxy proxy = fixedTextSelectionProcessor.GetSelectedNodes(selection)[1] as FixedPageProxy;
            processorTester.VerifyGenerateLocatorParts(selection, proxy);
            passTest("Selection last of 2 pages.");
        }

        /// <summary>
        /// Parameters: selection = valid text selection (multiple pages)
        ///				startNode = FixedPageProxy for an inner page	
        /// Verify: List with single ContentLocatorPart for the page�s portion of the selection
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts7()
        {
            TextRange selection = DocViewerWrapper.SetSelection(1, PagePosition.End, -10, 3, PagePosition.Beginning, 19); // Page1 to Page3
            FixedPageProxy proxy = fixedTextSelectionProcessor.GetSelectedNodes(selection)[1] as FixedPageProxy;
            processorTester.VerifyGenerateLocatorParts(selection, proxy);
            passTest("Selection middle of 3 pages.");
        }

        /// <summary>
        /// Parameters: selection = not an ITextRange
        ///				startNode = FixedPageProxy for any page in the selection	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts8()
        {
            processorTester.VerifyGenerateLocatorPartsFails(ViewerBase, new FixedPageProxy(ViewerBase, 1), typeof(ArgumentException));
            passTest("Exception for non-TextRange selection.");
        }

        /// <summary>
        /// Parameters: Selection = ITextRange not inside DocumentViewer
        ///				StartNode = Falid FixedPageProxy	
        /// Verify: ArgumentException
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts9()
        {
            TextBox box = new TextBox();
            box.Text = "foobar";

            PropertyInfo TextSelectionInfo = typeof(TextBoxBase).GetProperty("TextSelectionInternal", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextSelectionInfo == null)
            {
                throw new Exception("TextBoxBase.TextSelectionInternal property is not accessible");
            }
            TextRange TextRange = TextSelectionInfo.GetValue(box, null) as TextRange;
            processorTester.VerifyGenerateLocatorPartsFails(new TextRange(TextRange.Start, TextRange.End), new FixedPageProxy(ViewerBase, 1), typeof(ArgumentException));
            passTest("Exception for TextRange outside DocumentViewer.");
        }

        /// <summary>
        /// Parameters: Selection = selection on page N
        ///				StartNode = FixedPageProxy for selection on Page N+1	
        /// Verify: ContentLocatorPart for StartNode
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts10()
        {
            TextRange selection = SetSelection(0, 0, 10);
            FixedPageProxy proxy = fixedTextSelectionProcessor.GetSelectedNodes(selection)[0] as FixedPageProxy;
            selection = SetSelection(4, 10, 42);	// Selection no longer matches FixedPageProxy.
            processorTester.VerifyGenerateLocatorParts(selection, proxy);
            passTest("Selection parameter is un-used.");
        }

        /// <summary>
        /// Parameters: Selection = selection on page N+1
        ///				StartNode=new FixedPageProxy(N)	
        /// Verify: ContentLocatorPart with no Start/End attributes.
        /// </summary>
        [Priority(1)]
        private void fixedtext_generatelocatorparts11()
        {
            TextRange selection = SetSelection(0, 0, 10);
            FixedPageProxy proxy = new FixedPageProxy(ViewerBase, 4);
            processorTester.VerifyGenerateLocatorParts(selection, proxy);
            passTest("FixedPageProxy manually created.");
        }

        #endregion GenerateLocatorParts Tests

        #region ResolveLocatorPart

        /// <summary>
        /// Parameters: locatorPart = null
        ///				startNode = valid FixedPage object	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart1()
        {
            processorTester.VerifyResolveLocatorPartFails(null, GetFixedPage(0), typeof(ArgumentNullException));
            passTest("Exception for null ContentLocatorPart.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = null	
        /// Verify: ArgumentNullException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart2()
        {
            ContentLocatorPart lp = CreateLocatorPart(new Point(0, 0), new Point(0, 0));
            processorTester.VerifyResolveLocatorPartFails(lp, null, typeof(ArgumentNullException));
            passTest("Exception for null StartNode.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = not a FixedPage object	
        /// Verify: ArgumentException thrown
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart3()
        {
            ContentLocatorPart lp = CreateLocatorPart(new Point(0, 0), new Point(0, 0));
            processorTester.VerifyResolveLocatorPartFails(lp, ViewerBase, typeof(ArgumentException));
            passTest("Exception for non-FixedPage StartNode.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = FixedPage, not located in DocumentViewer	
        /// Verify: null
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart5()
        {
            FixedPage page = new FixedPage();
            ContentLocatorPart lp = CreateLocatorPart(new Point(0, 0), new Point(0, 0));
            processorTester.VerifyResolveLocatorPartFails(lp, page, typeof(ArgumentException));
            passTest("Exception for FixedPage StartNode not inside DocumentViewer.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart for multi-page selection portion on last page
        ///				startNode = FixedPage for last page of selection	
        /// Verify: TextRange for portion of selection on the last page is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart7()
        {
            DocViewerWrapper.SetZoom(10);
            VerifyPageIsVisible(2);
            VerifyPageIsVisible(3);

            ContentLocatorPart lp = CreateLocatorPart(DocViewerWrapper.SetSelection(2, PagePosition.End, -42, 3, PagePosition.Beginning, 100), 1);
            FixedPage page = GetFixedPage(3);
            processorTester.VerifyResolveLocatorPart(lp, page, GetText(new SimpleSelectionData(3, PagePosition.Beginning, 100)), AttachmentLevel.Full);
            passTest("Resolved last page of multi-page selection.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart for multi-page selection portion on first page
        ///				startNode = FixedPage for first page of selection	
        /// Verify: TextRange for portion of selection on the first page is returned
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart8()
        {
            DocViewerWrapper.SetZoom(10);
            VerifyPageIsVisible(2);
            VerifyPageIsVisible(3);

            ContentLocatorPart lp = CreateLocatorPart(DocViewerWrapper.SetSelection(2, PagePosition.End, -19, 3, PagePosition.Beginning, 100), 0);
            FixedPage page = GetFixedPage(2);
            processorTester.VerifyResolveLocatorPart(lp, page, GetText(new SimpleSelectionData(2, PagePosition.End, -19)), AttachmentLevel.Full);
            passTest("Resolved first page of multi-page selection.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = FixedPage not corresponding to the ContentLocatorPart	
        /// Verify: Non-null TextAnchor.
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart10()
        {
            DocViewerWrapper.SetZoom(10);
            VerifyPageIsVisible(0);
            VerifyPageIsVisible(3);

            ContentLocatorPart lp = CreateLocatorPart(DocViewerWrapper.SetSelection(0, 150, 42), 0);
            FixedPage page = GetFixedPage(3);
            AttachmentLevel attachementLevel;
            object result = fixedTextSelectionProcessor.ResolveLocatorPart(lp, page, out attachementLevel);
            AssertNotNull("Verify that result is not null.", AnnotationTestHelper.IsTextAnchor(result));
            AssertEquals("Verify AttachmentLevel.", AttachmentLevel.Full, attachementLevel);
            passTest("Resolved ContentLocatorPart for wrong FixedPage.");
        }

        /// <summary>
        /// Parameters: locatorPart = valid ContentLocatorPart
        ///				startNode = FixedPage for visible page of selection
        /// * selection contains non-text object inline, i.e. Image	
        /// Verify: TextAnchor�s xml contains image.
        /// </summary>
        [Priority(1)]
        private void fixedtext_resolvelocatorpart11()
        {
            //string imageDef = "<Image";

            ContentLocatorPart lp = CreateLocatorPart(DocViewerWrapper.SetSelection(0, PagePosition.Beginning, 0, 1, PagePosition.Beginning, 10), 0);
            FixedPage page = GetFixedPage(0);

            AttachmentLevel attachementLevel;
            object anchor = fixedTextSelectionProcessor.ResolveLocatorPart(lp, page, out attachementLevel);
            AssertNotNull("Anchor should not be null.", AnnotationTestHelper.IsTextAnchor(anchor));
            AssertEquals("Verify AttachmentLevel.", AttachmentLevel.Full, attachementLevel);

            /* Need to change this to use text anchor
                        TextPointer pointer = range.Start;
                        bool pass = false;
                        while (pointer.CompareTo(range.End) != 0)
                        {
                            InlineUIContainer container = pointer.GetAdjacentElement(LogicalDirection.Forward) as InlineUIContainer;
                            if (container != null && container.Child is System.Windows.Controls.Image)
                            {
                                pass = true;
                                break;
                            }
                            pointer.GetNextContextPosition(LogicalDirection.Forward);
                        }

                        Assert("Verify that Image control is in the range.", pass);
            */
            passTest("Resolved locator part containing image.");
        }

        /// <summary>
        /// Parameters: ContentLocatorPart has no Start/End Attributes	
        /// Verify: Returns whole page.
        /// </summary>
        [DisabledTestCase]
        [Priority(1)]
        private void fixedtext_resolvelocatorpart12()
        {
            ContentLocatorPart lp = new ContentLocatorPart(new XmlQualifiedName("FixedTextRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            FixedPage page = GetFixedPage(0);
            processorTester.VerifyResolveLocatorPart(lp, page, "Microsoft Office for the Pharmaceutical and Healthcare Industries -6-Research Pane The Research Pane is an addition to Microsoft Office that healthcare and pharmaceutical professionals will find particularly helpful. Here is where you will begin to see how the Microsoft Office System is a platform for providing professionals with the tools necessary to do their jobs more efficiently. With the Research Pane, research can be conducted from within a Word 2003 document by simply holding the Alt key and clicking on a word in the document. It is no longer necessary to open a browser or new window to search for information. If a scientist is writing a paper on a topic such ascardiomyopathy, she can conduct literature searches on the Web from within her m----cript, as well as access a medical dictionary. Since translation of foreign language text can be accomplished with a single click in the Research TaskPane, the scientist also can utilize scientific findings published in languages besides English. In the image below a clinician is utilizing the Research Pane to acquire information about Zocor. You can see that she is accessing information from the CP Medicine Information database (a third-party add-on) about Zocor. ", AttachmentLevel.Full);
            passTest("Resolved un-initialized ContentLocatorPart.");
        }


        /// <summary>
        /// Parameters: ContentLocatorPart has Start/End Attributes that don�t resolve to the window.	
        /// Verify: Returns empty TextAnchor.
        /// </summary>
        [DisabledTestCase]
        [Priority(1)]
        private void fixedtext_resolvelocatorpart13()
        {
            ContentLocatorPart lp = CreateLocatorPart(new Point(9999999, 999999), new Point(-42, 44449959));
            FixedPage page = GetFixedPage(0);
            processorTester.VerifyResolveLocatorPart(lp, page, "", AttachmentLevel.Full);
            passTest("Resolved ContentLocatorPart with outragous Start/End points.");
        }

        /// <summary>
        /// Parameters: ContentLocatorPart with no Start Attribute.	
        /// Verify: Return TextAnchor from beginning of page to End.
        /// </summary>
        [DisabledTestCase]
        [Priority(1)]
        private void fixedtext_resolvelocatorpart14()
        {
            IList<DependencyObject> selectedNodes = fixedTextSelectionProcessor.GetSelectedNodes(SetSelection(1, 150, 0));
            FixedPageProxy fpp = selectedNodes[0] as FixedPageProxy;
            ContentLocatorPart lp = CreateLocatorPart(new Point(double.NaN, double.NaN), fpp.Segments[0].Start);
            FixedPage page = GetFixedPage(1);
            processorTester.VerifyResolveLocatorPart(lp, page, "Microsoft Office for the Pharmaceutical and Healthcare Industries -2-It is likely that you and your colleagues are currently using a version of Micro", AttachmentLevel.Full);
            passTest("Resolved ContentLocatorPart no Start Point.");
        }

        /// <summary>
        /// Parameters: ContentLocatorPart with no End Attribute	
        /// Verify: Return TextAnchor from Start to end of page.
        /// </summary>
        [DisabledTestCase]
        [Priority(1)]
        private void fixedtext_resolvelocatorpart15()
        {
            IList<DependencyObject> selectedNodes = fixedTextSelectionProcessor.GetSelectedNodes(DocViewerWrapper.SetSelection(3, 0, -150));
            FixedPageProxy fpp = selectedNodes[0] as FixedPageProxy;
            ContentLocatorPart lp = CreateLocatorPart(fpp.Segments[0].Start, new Point(double.NaN, double.NaN));
            FixedPage page = GetFixedPage(fpp.Page);
            processorTester.VerifyResolveLocatorPart(lp, page, "ling professionals to view all e-mail accounts in one view, sort mail by threaded conversation, assign follow-up flags with a single click, glance at", AttachmentLevel.Full);
            passTest("Resolved ContentLocatorPart with no End Point.");
        }

        /// <summary>
        /// Parameters: DocumentViewer has no content and ContentLocatorPart has no Start/End Attributes	
        /// Verify: Return null
        /// </summary>
        [DisabledTestCase]
        [Priority(1)]
        private void fixedtext_resolvelocatorpart16()
        {
            SetSelection(0, 0, 0);
            DocViewerWrapper.SetZoom(100.0);
            queueTask(new DispatcherOperationCallback(fixedtext_resolvelocatorpart16_stage0), null);
        }

        private object fixedtext_resolvelocatorpart16_stage0(object arg)
        {
            ViewerBase.Document = LoadContent("fixed_empty.xaml");
            queueTask(new DispatcherOperationCallback(fixedtext_resolvelocatorpart16_stage1), null);
            return null;
        }

        private object fixedtext_resolvelocatorpart16_stage1(object arg)
        {
            ContentLocatorPart lp = new ContentLocatorPart(new XmlQualifiedName("FixedTextRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            FixedPage page = GetFixedPage(0);
            processorTester.VerifyResolveLocatorPart(lp, page, null, AttachmentLevel.Unresolved);
            passTest("Resolved ContentLocatorPart with no Start/End attributes on empty document.");
            return null;
        }

        private ContentLocatorPart CreateLocatorPart(TextRange selection, int pageNumber)
        {
            IList<DependencyObject> selectedNodes = fixedTextSelectionProcessor.GetSelectedNodes(selection);
            Assert("Requested page is out of bounds.", selectedNodes.Count > pageNumber);
            return fixedTextSelectionProcessor.GenerateLocatorParts(selection, selectedNodes[pageNumber])[0];
        }

        private ContentLocatorPart CreateLocatorPart(Point start, Point end)
        {
            ContentLocatorPart lp = new ContentLocatorPart(new XmlQualifiedName("FixedTextRange", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace));
            lp.NameValuePairs.Add("StartX", start.X.ToString());
            lp.NameValuePairs.Add("StartY", start.Y.ToString());
            lp.NameValuePairs.Add("EndX", end.X.ToString());
            lp.NameValuePairs.Add("EndY", end.Y.ToString());
            return lp;
        }

        #endregion ResolveLocatorPart

        #endregion PRIORITY TESTS
    }
}

