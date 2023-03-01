// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations
{
    public class AStickyNoteAccessibilitySuite : AStickyNoteControlSuite
    {
        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            // Set defaults.
            InitiallyExpanded = true;
            TabHelper.Direction = TabHelper.TabDirection.Forward;

            foreach (string arg in args)
            {
                if (arg.Equals("/expanded=false"))
                    InitiallyExpanded = false;
                else if (arg.Equals("/tabdirection=backward"))
                    TabHelper.Direction = TabHelper.TabDirection.Backward;
            }

            printStatus("TabDirection = " + TabHelper.Direction);
            printStatus("InitiallyExpanded = " + InitiallyExpanded);
        }        

        protected override IList<string> UsageParameters()
        {
            IList<string> parameters = base.UsageParameters();
            parameters.Add("/expanded=[true|false] - default expanded state of all notes.");
            parameters.Add("/tabdirection=[forward|backward] - direction that tabbing should occur.");
            return parameters;
        }

        protected override IList<string> UsageExamples()
        {
            IList<string> examples = UsageExamples();
            examples.Add("XXX.exe /expanded=false /tabdirection=backward - notes are iconified and tab order is backward.");
            return examples;
        }

        protected override void DoExtendedSetup()
        {
            TabHelper.Root = MainWindow;
            if (ContentMode == TestMode.Fixed)
                SetZoom(75);
        }

        protected override void CreateDefaultNote()
        {
            base.CreateDefaultNote();
            if (!InitiallyExpanded)
                CurrentlyAttachedStickyNote.Expanded = false;
        }

        protected override void CleanupVariation()
        {
            TabHelper.Root = null;
            base.CleanupVariation();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Create stickynote with given author with default type and expanded state.
        /// </summary>
        [TestCase_Helper()]
        protected void CreateStickyNote(ISelectionData selection, string author)
        {
            new StickyNoteDefinition(selection, AnnotationType, InitiallyExpanded, author).Create(DocViewerWrapper);
        }

        /// <summary>
        /// Drag note with the given name to the specified position in the text.
        /// </summary>
        /// <param name="noteAuthor">Author of note to drag.</param>
        /// <param name="page">Page number</param>
        /// <param name="offset">Character offset onto page to drag note to.</param>
        [TestCase_Helper()]
        protected void DragNoteToTextPosition(string noteAuthor, int page, int offset)
        {
            StickyNoteWrapper wrapper = GetStickyNoteWithAuthor(noteAuthor);
            AssertNotNull("Verify note exists for author.", wrapper);
            ADocumentViewerBaseWrapper.HorizontalJustification justification = ADocumentViewerBaseWrapper.HorizontalJustification.Bottom;
            Point position = DocViewerWrapper.PointerToScreenCoordinates(page, offset, LogicalDirection.Forward, justification);
            wrapper.Drag(new Point(Monitor.ConvertScreenToLogical(Dimension.Width, position.X), Monitor.ConvertScreenToLogical(Dimension.Height, position.Y)));
        }

        /// <summary>
        /// Tab to the tools menu of the currently attached note.
        /// </summary>
        [TestCase_Helper()]
        protected void TabToToolsMenu()
        {
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            TabHelper.Tab(1);
            TabHelper.LogFocusedElement();
            Assert("Verify Tools menu is focused.", CurrentlyAttachedStickyNote.Menu.Target == TabHelper.CurrentlyFocusedElement);
            printStatus("Tools menu is current TabStop.");
        }

        /// <summary>
        /// Tab to the close button of the currently attached note.
        /// </summary>
        [TestCase_Helper()]
        protected void TabToCloseButton()
        {
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            TabHelper.Tab(2);
            TabHelper.LogFocusedElement();
            Assert("Verify Close button is focused.", CurrentlyAttachedStickyNote.CloseButton == TabHelper.CurrentlyFocusedElement);
            printStatus("Close button is current TabStop.");
        }

        /// <summary>
        /// PASS if from the current Focus state we can Tab into an Annotation.
        /// </summary>
        [TestCase_Helper()]
        protected void TestCanTabToAnnotationGroup()
        {
            TabHelper.TabToAnnotationGroup();
            Assert("Verify Note is focused.", TabHelper.IsCurrentElementAStickyNote);
            passTest("Verified can tab to Note..");
        }

        [TestCase_Helper()]
        protected void VerifyCurrentTabStop(string expectedAuthorName)
        {
            StickyNoteWrapper currentTabStop = TabHelper.CurrentlyFocusedStickyNote;
            AssertNotNull("Verify note is focused.", currentTabStop);
            AssertEquals("Verify correct note is focused.", expectedAuthorName, currentTabStop.Author);
        }

        /// <summary>
        /// Verify that if there are 2 notes in the Annotation TabGroup that we can tab back and forth between them.
        /// </summary>
        [TestCase_Helper()]
        protected void TestCanTabBetweenNotes(string[] authors)
        {
            if (authors.Length != 2 || GetStickyNoteWrappers().Length != 2)
                throw new NotImplementedException("Method not implemented for more than 2 notes.");

            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            VerifyCurrentTabStop(authors[0]);
            TabHelper.MoveToNextAnnotation();
            VerifyCurrentTabStop(authors[1]);
            TabHelper.MoveToPreviousAnnotation();
            VerifyCurrentTabStop(authors[0]);
            TabHelper.MoveToNextAnnotation();
            VerifyCurrentTabStop(authors[1]);
            passTest("Verified tabbing between 2 notes.");
        }

        /// <summary>
        /// Verify that tab order matches the expected order.
        /// Note: the given expected order will automatically get reversed if the Tab direction is backwards.
        /// </summary>
        /// <param name="expectedTabOrder"></param>
        [TestCase_Helper()]
        protected void TestTabOrder(string[] expectedTabOrder)
        {
            // Move mouse to uppper corner so that it isn't overlapping any anchors.
            MoveMouseToCharacterOffset(DocViewerWrapper.FirstVisiblePage, 0);

            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();

            VisualTreeWalker<StickyNoteControl> visualTreeWalker = new VisualTreeWalker<StickyNoteControl>();
            int tabIdx = (TabHelper.Direction == TabHelper.TabDirection.Forward) ? 0 : expectedTabOrder.Length - 1;
            while (tabIdx >= 0 && tabIdx < expectedTabOrder.Length)
            {
                Assert("Verify current element is a StickyNoteControl.", TabHelper.IsCurrentElementAStickyNote);
                StickyNoteWrapper currentNote = new StickyNoteWrapper(visualTreeWalker.FindParent(TabHelper.CurrentlyFocusedElement as Visual), "Note" + tabIdx.ToString());
                AssertEquals("Verify tab order.", expectedTabOrder[tabIdx], currentNote.Author);
                printStatus("Tab Stop: " + currentNote.Author);

                // Ensure that icon size transitions properly during tabbing.
                VerifyIconSizesOfAllNotes(currentNote);           

                TabHelper.MoveToNextAnnotation();
                if (TabHelper.Direction == TabHelper.TabDirection.Forward)
                    tabIdx++;
                else
                    tabIdx--;
            }
            passTest("Verified tab order.");
        }

        /// <summary>
        /// Verify the icon sizes of all iconified notes.
        /// </summary>
        /// <param name="focusedNote">Only note whose icon should appear in Hover state.<param>
        [TestCase_Helper()]
        private void VerifyIconSizesOfAllNotes(StickyNoteWrapper focusedNote)
        {
            printStatus("Verifying icon sizes...");
            StickyNoteWrapper[] wrapppers = GetStickyNoteWrappers();
            foreach (StickyNoteWrapper wrapper in wrapppers)
            {
                if (!wrapper.Expanded)
                {
                    printStatus("Checking Note '" + wrapper.Author + "'.");
                    if (wrapper.Target == focusedNote.Target)
                        VerifyIconSize(wrapper.Target, StickyNoteWrapper.IconHoverSize);
                    else
                        VerifyIconSize(wrapper.Target, StickyNoteWrapper.IconDefaultSize);
                }
            }
        }

        #endregion

        protected bool InitiallyExpanded = true;
        protected TabHelper TabHelper = new TabHelper();
    }
}	

