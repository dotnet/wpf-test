// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: See spec http://team/sites/ag/Test%20Specs/TDS%20-%20Accessibility.doc

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Documents;

namespace Avalon.Test.Annotations.Suites
{
    public class MultiNoteSuite : AStickyNoteAccessibilitySuite
    {
        #region BVT TESTS

        [TestCase_Helper()]
        protected void Setup_multinote1()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 42), AnnotationType, InitiallyExpanded, Author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 521, 61), AnnotationType, InitiallyExpanded, Author2).Create(DocViewerWrapper);
        }

        /// <summary>
        /// Variations                  Initial State               Action
        /// Backwards, RightToLeft      Two Notes, A and B.         Forward tab order: A, B
        ///                                                         Backward tab order: B, A
        /// </summary>
        [TestDimension(",/tabdirection=backward")]
        [TestDimension(",/property=(FlowDirection=RightToLeft)")]
        [Priority(0)]
        protected void multinote1_1()
        {
            Setup_multinote1();
            TestTabOrder(new string[] { Author1, Author2 });
        }

        /// <summary>
        /// Variations                  Initial State               Action
        ///                             Two Notes, A and B.         Can tab between Notes.        
        /// </summary>
        [Priority(0)]
        protected void multinote1_2()
        {
            Setup_multinote1();
            TestCanTabBetweenNotes(new string[] { Author1, Author2 });
        }

        /// <summary>
        /// Variations                  Initial State               Action
        ///                             Two Notes, A and B.         Iconify First.
        ///                                                         Can tab between Notes.      
        /// </summary>
        [Priority(0)]
        protected void multinote1_3()
        {
            Setup_multinote1();
            GetStickyNotesByAuthor(new string[] { Author1 })[0].Expanded = false;
            TestCanTabBetweenNotes(new string[] { Author1, Author2 });
        }


        /// <summary>
        /// Variations                  Initial State               Action
        ///                             Two Notes, A and B.         Iconify both.
        ///                                                         Can tab between Notes.      
        /// </summary>
        [Priority(0)]
        protected void multinote1_4()
        {
            InitiallyExpanded = false;
            Setup_multinote1();
            TestCanTabBetweenNotes(new string[] { Author1, Author2 });
        }

        /// <summary>
        /// Variations                  Initial State               Action
        ///                             Two Notes, A and B.         Iconify both.
        ///                                                         Restore both.
        /// </summary>
        //[Priority(0)]
        //protected void multinote1_5()
        //{
        //    InitiallyExpanded = false;
        //    Setup_multinote1();
        //    ViewerBase.Focus();
        //    TabHelper.TabToAnnotationGroup();
        //    AssertEquals("Verify Note A is initially iconified.", false, GetStickyNoteWithAuthor(Author1).Expanded);
        //    UIAutomationModule.PressKey(Key.Enter);
        //    AssertEquals("Verify Note A is expanded after enter.", true, GetStickyNoteWithAuthor(Author1).Expanded);
        //    TabHelper.MoveToNextAnnotation();
        //    AssertEquals("Verify Note B is initially iconified.", false, GetStickyNoteWithAuthor(Author2).Expanded);
        //    UIAutomationModule.PressKey(Key.Enter);
        //    AssertEquals("Verify Note B is expanded after enter.", true, GetStickyNoteWithAuthor(Author2).Expanded);
        //    passTest("Verified expanding multiple notes with keyboard.");
        //}
        [TestCase_Helper()]
        private void Setup_multinote2()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 42), AnnotationType, InitiallyExpanded, Author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 521, 61), AnnotationType, InitiallyExpanded, Author2).Create(DocViewerWrapper);
            Point dest = new Point(300, 500);
            GetStickyNoteWithAuthor(Author1).Drag(dest);
            GetStickyNoteWithAuthor(Author2).Drag(dest);
        }

        /// <summary>
        /// Variations      Initial State                           Action
        /// Backward        Different anchor overlapping notes.     Forward tab order: A, B
        ///                                                         Backward tab order: B, A
        /// </summary>
        [TestDimension(",/tabdirection=backward")]
        protected void multinote2_1()
        {
            Setup_multinote2();
            TestTabOrder(new string[] { Author1, Author2 });
        }

        /// <summary>
        /// Variations      Initial State                           Action
        ///                 Different anchor overlapping notes.     Tab between Note A and B, Z-order changes.
        /// </summary>
        [Priority(0)]
        protected void multinote2_2()
        {
            Setup_multinote2();
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();

            StickyNoteWrapper[] zOrder1 = GetStickyNotesByAuthor(new string[] { Author1, Author2 });
            StickyNoteWrapper[] zOrder2 = GetStickyNotesByAuthor(new string[] { Author2, Author1 });

            VerifyZOrders(zOrder1);
            TabHelper.MoveToNextAnnotation();
            VerifyZOrders(zOrder2);
            TabHelper.MoveToPreviousAnnotation();
            VerifyZOrders(zOrder1);

            passTest("Verified Z-order changes.");
        }

        [TestCase_Helper()]
        private void Setup_multinote3()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 50), AnnotationType, true, Author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 50), AnnotationType, true, Author2).Create(DocViewerWrapper);
            GetStickyNoteWithAuthor(Author2).Drag(new Vector(200, 200));
            if (!InitiallyExpanded)
            {
                GetStickyNoteWithAuthor(Author1).Expanded = false;
                GetStickyNoteWithAuthor(Author2).Expanded = false;
            }
        }

        /// <summary>
        /// Variations              Initial State                           Action
        /// Iconified only          Same anchor, different note position.   Tab to Note A.
        ///                                                                 ‘Enter’ Restores note.
        /// </summary>
        [Priority(0)]
        protected void multinote3_4()
        {
            InitiallyExpanded = false;
            Setup_multinote3();
            TabHelper.TabToAnnotationGroup();
            UIAutomationModule.PressKey(Key.Enter);
            AssertEquals("Verify expanded state of note A.", true, GetStickyNoteWithAuthor(Author1).Expanded);
            AssertEquals("Verify expanded state of note B.", false, GetStickyNoteWithAuthor(Author2).Expanded);
            passTest("Verified expanding overlapping icons with keyboard.");
        }

        /// <summary>
        /// Variations              Initial State                           Action
        /// Iconified only          Same anchor, different note position.   Tab to Note B.
        ///                                                                 ‘Enter’ Restores note.
        /// </summary>
        [Priority(0)]
        [Keywords("MicroSuite")]
        protected void multinote3_5()
        {
            InitiallyExpanded = false;
            Setup_multinote3();
            TabHelper.TabToAnnotationGroup();
            TabHelper.MoveToNextAnnotation();
            UIAutomationModule.PressKey(Key.Enter);
            AssertEquals("Verify expanded state of note A.", false, GetStickyNoteWithAuthor(Author1).Expanded);
            AssertEquals("Verify expanded state of note B.", true, GetStickyNoteWithAuthor(Author2).Expanded);
            passTest("Verified expanding overlapping icons with keyboard.");
        }

        /// <summary>
        /// Variations              Initial State                               Action
        ///                         Note A is iconified, Note B is expanded.    Tab between Note A and B.
        /// </summary>
        [Priority(0)]
        protected void multinote4_2()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 76, 61), AnnotationType, true, Author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 491, 19), AnnotationType, false, Author2).Create(DocViewerWrapper);
            TestCanTabBetweenNotes(new string[] { Author1, Author2 });
        }

        /// <summary>
        /// Variations              Initial State                               Action
        ///                         Note A is iconified, note B is expanded     Tab to Note A.
        ///                                                                     ‘Enter’ Expands note and Note A is above note B.
        /// </summary>
        [Priority(0)]
        protected void multinote5()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 76, 61), AnnotationType, false, Author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 491, 19), AnnotationType, true, Author2).Create(DocViewerWrapper);

            ADocumentViewerBaseWrapper.HorizontalJustification justification = ADocumentViewerBaseWrapper.HorizontalJustification.Bottom;
            Point iconPosition = DocViewerWrapper.PointerToScreenCoordinates(0, 76, LogicalDirection.Forward, justification);

            GetStickyNoteWithAuthor(Author2).Drag(Point.Add(iconPosition, new Vector(-50, -50)));

            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            UIAutomationModule.PressKey(Key.Enter);
            AssertEquals("Verify expanded state of note A.", true, GetStickyNoteWithAuthor(Author1).Expanded);
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { Author1, Author2 }));
            passTest("Verified z-order of expanded note.");
        }

        /// <summary>
        /// Variations              Initial State                           Action
        /// Iconified, backward,    Anchors w/ same start point.            Forward tab order: Green, Red
        /// RightToLeft	                                                    Backward tab order: Red, Green
        /// </summary>
        [TestDimension(",/tabdirection=backward")]
        [TestDimension("/expanded=false,/property=(FlowDirection=RightToLeft)")]
        [Priority(0)]
        protected void multinote7()
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 50, 100), AnnotationType, InitiallyExpanded, Author2).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 50, 50), AnnotationType, InitiallyExpanded, Author1).Create(DocViewerWrapper);
            TestTabOrder(new string[] { Author1, Author2 });
        }


        /// <summary>
        /// Variations              Initial State                           Action
        /// Backwards                                               Forward tab order: A, B, C, D
        ///                                                         Backward tab order: D, C, B, A
        ///                                                         * Ensure icon size changes as expected.
        /// </summary>
        [TestDimension(",/tabdirection=backward")]
        [Priority(0)]
        protected void multinote14()
        {
            string[] authors = new string[] { "A", "B", "C", "D" };
            CreateStickyNote(new SimpleSelectionData(0, 732, 13), authors[3]);
            CreateStickyNote(new SimpleSelectionData(0, 100, 10), authors[0]);
            CreateStickyNote(new SimpleSelectionData(0, 419, -61), authors[2]);
            CreateStickyNote(new SimpleSelectionData(0, 169, 500), authors[1]);
            GetStickyNoteWithAuthor(authors[1]).Expanded = false;
            GetStickyNoteWithAuthor(authors[3]).Expanded = false;
            TestTabOrder(authors);
        }

        #region Fields

        string Author1 = "A";
        string Author2 = "B";

        #endregion

        #endregion BVT TESTS

        #region PRIORITY TESTS

        [TestCase_Helper()]
        private void Setup_multinote3(string author1, string author2)
        {
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 50), AnnotationType, InitiallyExpanded, author1).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 100, 50), AnnotationType, InitiallyExpanded, author2).Create(DocViewerWrapper);
            GetStickyNoteWithAuthor(author2).Drag(new Vector(200, 200));
        }

        /// <summary>
        /// Variations      Initial State                               Action
        ///                 Same Anchor. Note 1 created before 2.       Can Tab between notes causing Z-order to change.
        /// </summary>
        [Priority(1)]
        protected void multinote3_2()
        {
            Setup_multinote3("foo", "bar");
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            // Don't depend upon the tab order of these notes.
            string firstAuthor = TabHelper.CurrentlyFocusedStickyNote.Author;
            string secondAuthor = (firstAuthor.Equals("foo")) ? "bar" : "foo";
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { firstAuthor, secondAuthor }));
            printStatus("Verified z-order 1.");
            TabHelper.MoveToNextAnnotation();
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { secondAuthor, firstAuthor }));
            printStatus("Verified z-order 2.");
            TabHelper.MoveToPreviousAnnotation();
            VerifyZOrders(GetStickyNotesByAuthor(new string[] { firstAuthor, secondAuthor }));
            printStatus("Verified z-order 3.");
            passTest("Verified z order changes for notes with same anchor.");
        }

        /// <summary>
        /// Variations              Initial State                               Action
        /// Backward, RightToLeft   Note A is iconified, Note B is expanded.    Forward tab order: A, B
        ///                                                                     Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void multinote4_1()
        {
            string [] authors = new string [] { "Darth", "Vader" };            
            new StickyNoteDefinition(new SimpleSelectionData(0, 25, 281), AnnotationType, false, authors[0]).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, PagePosition.Middle, 92), AnnotationType, true, authors[1]).Create(DocViewerWrapper);
            MoveMouseToCharacterOffset(0, 5);
            ViewerBase.Focus();
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations              Initial State                                       Action
        /// Iconified, backward,    Note position is opposite from anchor position      Forward Tab order: B, A
        /// RightToLeft	                                                                Backward Tab order: A, B
        /// </summary>
        [Priority(1)]
        protected void multinote6()
        {
            string[] authors = new string[] { "A", "B" };
            new StickyNoteDefinition(new SimpleSelectionData(0, 25, 281), AnnotationType, true, authors[0]).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 500, 92), AnnotationType, true, authors[1]).Create(DocViewerWrapper);
            DragNoteToTextPosition(authors[0], 0, 600);
            DragNoteToTextPosition(authors[1], 0, 25);
            MoveMouseToCharacterOffset(0, 5);
            ViewerBase.Focus();
            if (!InitiallyExpanded) {
                GetStickyNoteWithAuthor(authors[0]).Expanded = false;
                GetStickyNoteWithAuthor(authors[1]).Expanded = false;
            }
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations              Initial State                                       Action
        /// Iconified, backward,    Note B and A have same anchor start point.          A created before B.
        /// RightToLeft	            Both anchors are larger than visible text.          Forward tab order: blue, red
        ///                                                                             Backward tab order: red, blue
        /// </summary>
        [Priority(1)]
        protected void multinote8_1()
        {
            Test_multinote8(true);
        }

        /// <summary>
        /// Variations              Initial State                                       Action
        /// Iconified, backward,    Note B and A have same anchor start point.          B created before A.
        /// RightToLeft	            Both anchors are larger than visible text.          Forward tab order: blue, red
        ///                                                                             Backward tab order: red, blue
        /// </summary>
        [Priority(1)]
        protected void multinote8_2()
        {
            Test_multinote8(false);
        }

        /// <summary>
        /// Variations              Initial State                                       Action
        /// Iconified, backward,    Note B and A have same anchor start point.          Forward tab order: blue, red
        /// RightToLeft	            Both anchors are larger than visible text.          Backward tab order: red, blue
        /// </summary>
        [TestCase_Helper()]
        protected void Test_multinote8(bool aBeforeB)
        {
            string[] authors = new string[] { "A", "B" };
            StickyNoteDefinition [] definitions = new StickyNoteDefinition [] {
                new StickyNoteDefinition(new MultiPageSelectionData(0, PagePosition.End, -200, 1, PagePosition.Beginning, 50), AnnotationType, InitiallyExpanded, authors[0]),
                new StickyNoteDefinition(new MultiPageSelectionData(0, PagePosition.End, -200, 1, PagePosition.Beginning, 100), AnnotationType, InitiallyExpanded, authors[1])
            };

            int first = (aBeforeB) ? 0 : 1;
            int second = (aBeforeB) ? 1 : 0;
            definitions[first].Create(DocViewerWrapper);
            definitions[second].Create(DocViewerWrapper);

            MoveMouseToCharacterOffset(0, 2);
            VerifyPageIsNotVisible(1);
            ViewerBase.Focus();
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations              Initial State                           Action
        /// RightToLeft	            Two pages visible, 4 notes.             Forward tab order: A, B, C, D
        ///                                                                 Backward tab order: D, C, B, A
        /// </summary>
        [Priority(1)]
        protected void multinote9()
        {
            ViewAsTwoPages();

            string[] authors = new string[] { "A", "B", "C", "D" };            
            new StickyNoteDefinition(new SimpleSelectionData(1, PagePosition.Beginning, 42), AnnotationType, InitiallyExpanded, authors[2]).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, PagePosition.Middle, 640), AnnotationType, InitiallyExpanded, authors[1]).Create(DocViewerWrapper);            
            new StickyNoteDefinition(new SimpleSelectionData(1, PagePosition.Middle, -11), AnnotationType, InitiallyExpanded, authors[3]).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 91, 450), AnnotationType, InitiallyExpanded, authors[0]).Create(DocViewerWrapper);
            
            VerifyNumberOfAttachedAnnotations(4);
            ViewerBase.Focus();
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations              Initial State                   Action
        /// Iconified               1 text and 1 ink note.          Can tab between text and ink notes.
        /// </summary>
        [Priority(1)]
        protected void multinote10()
        {
            string[] authors = new string[] { "A", "B" };
            new StickyNoteDefinition(new SimpleSelectionData(0, 303, -45), AnnotationMode.StickyNote, InitiallyExpanded, authors[0]).Create(DocViewerWrapper);
            new StickyNoteDefinition(new SimpleSelectionData(0, 900, 21), AnnotationMode.InkStickyNote, InitiallyExpanded, authors[1]).Create(DocViewerWrapper);

            ViewerBase.Focus();
            TestCanTabBetweenNotes(authors);
        }

        /// <summary>
        /// Variations              Initial State                   Action
        ///                         Create 2 notes.                 Verify tab order is as expected.
        ///                         Page Down.
        ///                         Page Up.	
        /// </summary>
        [Priority(1)]
        protected void multinote11()
        {
            string[] authors = new string[] { "A", "B" };
            CreateStickyNote(new SimpleSelectionData(0, 500, 62), authors[1]);
            CreateStickyNote(new SimpleSelectionData(0, 25, -12), authors[0]);
            ViewerBase.Focus();
            PageDown();
            PageUp();
            TestCanTabBetweenNotes(authors);
        }

        /// <summary>
        /// Variations              Initial State                       Action
        ///                         Notes on Page1, Page2, Page3.       Can only tab to notes that start on Page2.
        ///                         Page 2 is visible.	                        	
        /// </summary>
        [Priority(1)]
        protected void multinote12_1()
        {
            Setup_multinote12();
            GoToPage(1);
            TestTabOrder(new string [] { "1-B", "1-A" } );
        }

        /// <summary>
        /// Variations              Initial State                   Action
        ///                         Notes on Page1, Page2, Page3.   Can only tab to notes that start on either Page 2 or 3.
        ///                         Pages 2 and 3 are visible.	
        /// </summary>
        [Priority(1)]
        protected void multinote12_2()
        {
            Setup_multinote12();
            GoToPageRange(1, 2);
            TestTabOrder(new string [] { "1-B", "1-A",  "2-B", "2-A" });
        }

        [TestCase_Helper()]
        protected void Setup_multinote12()
        {
            WholePageLayout();
            CreateStickyNote(new SimpleSelectionData(0, 100, 20), "0-A");
            CreateStickyNote(new SimpleSelectionData(0, PagePosition.Middle, 49), "0-B");
            GoToPage(1);
            CreateStickyNote(new SimpleSelectionData(1, PagePosition.End, -61), "1-A");
            CreateStickyNote(new SimpleSelectionData(1, PagePosition.Beginning, 74), "1-B");
            GoToPage(2);
            CreateStickyNote(new SimpleSelectionData(2, 421, 25), "2-A");
            CreateStickyNote(new SimpleSelectionData(2, 350, 500), "2-B");
            ViewerBase.Focus();
        }

        /// <summary>
        /// Variations              Initial State               Action
        ///                         Note A is iconfied.         Tab from A to B causes A’s icon to return to small size.
        ///                         Note B is expanded.	        Tab from B to A causes A’s icon to change to hover state.
        /// </summary>
        [Priority(1)]
        protected void multinote13()
        {            
            CreateStickyNote(new SimpleSelectionData(0, 10, 19), "A");
            CurrentlyAttachedStickyNote.Expanded = false;
            CreateStickyNote(new SimpleSelectionData(0, 900, 10), "B");
            StickyNoteWrapper iconifiedNote = GetStickyNoteWithAuthor("A");
            ViewerBase.Focus();
            TabHelper.TabToAnnotationGroup();
            printStatus("Verify icon initial size.");
            VerifyIconSize(iconifiedNote.Target,StickyNoteWrapper.IconHoverSize);
            TabHelper.MoveToNextAnnotation();
            printStatus("Verfy icon changed to small size.");
            VerifyIconSize(iconifiedNote.Target, StickyNoteWrapper.IconDefaultSize);
            TabHelper.MoveToPreviousAnnotation();
            printStatus("Verfy icon changed back to hover size.");
            VerifyIconSize(iconifiedNote.Target, StickyNoteWrapper.IconHoverSize);
            passTest("Verified icon size changed with tabbed element.");
        }

        #endregion PRIORITY TESTS
    }
}	

