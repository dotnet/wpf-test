// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Various tests that focus on different kinds of flow content.

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Test.Annotations;
using System.Windows.Annotations;
using System.Windows.Markup;

namespace Avalon.Test.Annotations.Pri1s
{
    [TestDimension("highlight,stickynote")]
	public class FlowContentSuite : AFlowSuite
	{
        public override IDocumentPaginatorSource FlowContent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Annotation that spans from end of a page to a page with only whitespace.
        /// -Create while viewing last page.
        /// </summary>
        protected void flowcontent3_1()
        {
            Test_flowcontent3(true);
        }

        /// <summary>
        /// Annotation that spans from end of a page to a page with only whitespace.
        /// -Create while viewing 2nd to last page.
        /// </summary>
        protected void flowcontent3_2()
        {
            Test_flowcontent3(false);
        }

        /// <summary>
        /// Annotation that spans from end of a page to a page with only whitespace.
        /// </summary>
        [TestCase_Helper()]
        protected void Test_flowcontent3(bool viewLast)
        {
            SetDocumentViewerContent(ViewerTestConstants.DrtFlowContent);
            SetWindowWidth(669);
            SetWindowHeight(543);

            if (viewLast)
                GoToLastPage();
            else
                GoToPage(DocViewerWrapper.PageCount - 2);

            CreateAnnotation(new MultiPageSelectionData(DocViewerWrapper.PageCount - 2, PagePosition.End, 0, DocViewerWrapper.PageCount - 1, PagePosition.End, 0));
            VerifyNumberOfAttachedAnnotations(1);
            printStatus("Verified annotation on 1st page.");

            if (viewLast)
                PageUp();
            else
                PageDown();

            VerifyNumberOfAttachedAnnotations(1);
            printStatus("Verified annotation on 2nd page.");
            passTest("Bug#1304405");
        }

    }

    public class NestedViewerSuite : AFlowSuite
    {
        Button TestButton;

        public override IDocumentPaginatorSource FlowContent
        {
            get
            {
                return null;
            }
        }        

        protected override object CreateWindowContents()
        {
            DockPanel mainPanel = new DockPanel();

            // Put our button inside a Toolbar because the CommandTarget will
            // then get automatically set to the currently focused item.
            ToolBar tb = new ToolBar();
            TestButton = new Button();
            ((IAddChild)tb).AddChild(TestButton);
            DockPanel.SetDock(tb, Dock.Top);
            mainPanel.Children.Add(tb);

            mainPanel.Children.Add(TextControl);
            return mainPanel;
        }

        [TestCase_Setup("nestedviewer.*")]
        protected void NestedViewerSetup()
        {
            base.DoSetup();
            SetDocumentViewerContent(ViewerTestConstants.NestedViewer_Simple);
            SetWindowWidth(669);
            SetWindowHeight(543);
        }

        /// <summary>
        /// Annotate a doc with nested viewers.
        /// Page down, ensure annotation unloaded.
        /// Page up, ensure annotation loaded.
        /// </summary>
        protected void nestedviewer1()
        {
            ISelectionData selection = new SimpleSelectionData(0, 1, 5);
            string anchor = GetText(selection);
            CreateAnnotation(selection);
            VerifyAnnotation(anchor);
            PageDown();
            VerifyNumberOfAttachedAnnotations(0);
            PageUp();
            VerifyAnnotation(anchor);
            passTest("Verified loading/unloading annotations in document with nested viewers.");
        }

        /// <summary>
        /// Annotate across nested viewer.
        /// Page down, ensure annotation unloaded.
        /// Page up, ensure annotation loaded.
        /// </summary>
        protected void nestedviewer2_1() { TestAcrossViewer(0); }

        /// <summary>
        /// Annotate across nested viewer inside a figure.
        /// Page down, ensure annotation unloaded.
        /// Page up, ensure annotation loaded.
        /// </summary>
        protected void nestedviewer2_2() { TestAcrossViewer(1); }

        private void TestAcrossViewer(int initialPage)
        {
            GoToPage(initialPage);
            ISelectionData selection = new MultiPageSelectionData(initialPage, PagePosition.Beginning, 0, initialPage, PagePosition.End, 0);
            string anchor = GetText(selection);
            CreateAnnotation(selection);
            VerifyAnnotation(anchor);
            if (initialPage == 0) PageDown(); else PageUp();
            VerifyNumberOfAttachedAnnotations(0);
            if (initialPage == 0) PageUp(); else PageDown();
            VerifyAnnotation(anchor);
            passTest("Verified annotation across nested viewer.");
        }

        /// <summary>
        /// Annotate entire document containing multiple nested viewers.
        /// Verify annotation loaded on each page.
        /// </summary>
        protected void nestedviewer3()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Beginning, 0, 1, PagePosition.End, 0));
            VerifyNumberOfAttachedAnnotations(1);
            PageDown();
            VerifyNumberOfAttachedAnnotations(1);
            PageUp();
            VerifyNumberOfAttachedAnnotations(1);
            passTest("Verified annotating across multiple nested viewers.");
        }

        /// <summary>
        /// Directly nested viewer:
        /// Selection in nested viewer, verify annotation commands are not enabled.
        /// </summary>
        [OverrideClassTestDimensions]
        protected void nestedviewer4_1() { TestAllCommandsDisabled("NestedViewer1"); }

        /// <summary>
        /// Nested viewer inside Figure:
        /// Selection in nested viewer, verify annotation commands are not enabled.
        /// </summary>
        [OverrideClassTestDimensions ]
        protected void nestedviewer4_2() { TestAllCommandsDisabled("NestedViewer2"); }

        /// <summary>
        /// Selection in nested viewer but focus is on "main" viewer.
        /// All commands are enabled.
        /// </summary>
        [OverrideClassTestDimensions]
        protected void nestedviewer4_3()
        {
            FlowDocumentScrollViewer nestedViewer = (FlowDocumentScrollViewer)TextControlWrapper.SelectionModule.AnchoredBlocks.FindElement(typeof(FlowDocumentScrollViewer), "NestedViewer1");
            AssertNotNull("Verify nested viewer was found.", nestedViewer);

            // Set selection in "main" viewer.
            SetSelection(new SimpleSelectionData(0, 1, 5));
            // Set selection in nested viewer.
            new FlowDocumentScrollViewerSelector(nestedViewer).SetSelection(2, 5);
            // Focus main viewer.
            TextControl.Focus();
            VerifyCommandEnablement(true);
            passTest("All commands enabled if selection in main viewer and focus.");
        }

        private void TestAllCommandsDisabled(string viewerName) 
        {
            FlowDocumentScrollViewer nestedViewer = (FlowDocumentScrollViewer)TextControlWrapper.SelectionModule.AnchoredBlocks.FindElement(typeof(FlowDocumentScrollViewer), viewerName);
            AssertNotNull("Verify nested viewer was found.", nestedViewer);

            // Set selection in "main" viewer.
            SetSelection(new SimpleSelectionData(0, 1, 5));
            // Set selection in nested viewer, and focus it.
            new FlowDocumentScrollViewerSelector(nestedViewer).SetSelection(2, 5);
            nestedViewer.Focus();

            VerifyCommandEnablement(false);
            passTest("All commands disabled for selection in nested viewer.");
        }

        private void VerifyCommandEnablement(bool areEnabled)
        {
            VerifyCommandEnablement(AnnotationService.CreateHighlightCommand, areEnabled);
            VerifyCommandEnablement(AnnotationService.CreateTextStickyNoteCommand, areEnabled);
            VerifyCommandEnablement(AnnotationService.CreateInkStickyNoteCommand, areEnabled);
            VerifyCommandEnablement(AnnotationService.ClearHighlightsCommand, areEnabled);
            VerifyCommandEnablement(AnnotationService.DeleteStickyNotesCommand, areEnabled);
            VerifyCommandEnablement(AnnotationService.DeleteAnnotationsCommand, areEnabled);
        }

        private void VerifyCommandEnablement(RoutedUICommand command, bool expectedEnablement)
        {
            TestButton.Command = command;
            TestButton.Content = command.Name;
            DispatcherHelper.DoEvents();
            AssertEquals("Command '" + command.Name + "' enablement.", expectedEnablement, TestButton.IsEnabled);
        }
    }
}	

