// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: See spec http://team/sites/ag/Test%20Specs/TDS%20-%20Accessibility.doc

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace Avalon.Test.Annotations.Suites
{
    public class FlowSuite : AStickyNoteAccessibilitySuite
    {
        protected override TestMode DetermineTestMode(string[] args)
        {
            return TestMode.Flow;
        }

        #region BVT TESTS

        /// <summary>
        /// Variations	            Initial State	                Action.
        ///                         1 Note and a highlight.	        Can only tab to Note.
        /// </summary>
        [DisabledTestCase()]
        protected void flow1()
        {
            SetDocumentViewerContent(ViewerTestConstants.ComplexFlowContent);
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        #region Overrides

        public override IDocumentPaginatorSource FlowContent
        {
            get
            {
                return null;
            }
        }

        [TestCase_Helper()]
        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            AnchoredBlockType = typeof(Figure); //default.
            foreach (string arg in args)
            {
                if (arg.ToLower().Equals("floater"))
                {
                    AnchoredBlockType = typeof(Floater);
                    break;
                }
            }
        }

        [TestCase_Helper()]
        protected override void DoExtendedSetup()
        {            
            if (CaseNumber.Contains("table"))
                SetDocumentViewerContent(ViewerTestConstants.TableTests.Filename);
            else if (CaseNumber.Contains("anchoredblock"))
            {
                SetDocumentViewerContent(ViewerTestConstants.AnchoredBlockTests.Filename);
                if (AnchoredBlockType.Equals(typeof(Figure)))
                {
                    AnchoredBlockName = Figure1;
                    AnchoredBlockPage = 0;
                }
                else 
                {
                    AnchoredBlockName = Floater1;
                    AnchoredBlockPage = 1;
                    SetWindowWidth(856);
                    SetWindowHeight(603);
                    GoToPage(1);
                }
            }
            else
                SetDocumentViewerContent(ViewerTestConstants.SimpleFlowContent);
            base.DoExtendedSetup();
        }

        #endregion

        /// <summary>
        /// Variations	            Initial State	                        Action.
        /// RightToLeft             Two page style where notes have         Forward tab order: A, B, C, D
        ///                         been dragged across pages.              Backward tab order: D, C, B, A
        /// </summary>
        [Priority(1)]
        protected void flow2()
        {
            string[] authors = new string[] { "A", "B", "C", "D" };
            ViewAsTwoPages();
            SetZoom(60);
            SetWindowWidth(1036);
            SetWindowHeight(808);
            CreateStickyNote(new SimpleSelectionData(0, 10, 100), authors[0]);
            CreateStickyNote(new SimpleSelectionData(1, PagePosition.Middle, 84), authors[3]);
            CreateStickyNote(new SimpleSelectionData(0, PagePosition.End, -90), authors[1]);
            CreateStickyNote(new SimpleSelectionData(1, 71, 134), authors[2]);            
            DragNoteToTextPosition(authors[2], 1, 200);
            DragNoteToTextPosition(authors[0], 1, 10);
            DragNoteToTextPosition(authors[1], 0, 800);            
            DragNoteToTextPosition(authors[3], 0, 900);
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations	            Initial State	                Action.
        /// Iconified               A in cell before B              Forward tab order: A, B
        ///                                                         Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void flow_table1()
        {
            string[] authors = new string[] { "A", "B" };            
            CreateStickyNote(new TableSelectionData(SimpleTable, 1, 3), authors[1]);
            CreateStickyNote(new TableSelectionData(SimpleTable, 1, 1), authors[0]);
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations	            Initial State	                Action.
        /// Iconified               B in row below A                Forward tab order: A, B
        ///                                                         Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void flow_table2()
        {
            string[] authors = new string[] { "A", "B" };
            CreateStickyNote(new TableSelectionData(SimpleTable, 1, 1), authors[0]);
            CreateStickyNote(new TableSelectionData(SimpleTable, 2, 1), authors[1]);
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations	            Initial State	                    Action.
        /// Iconified               A on cells 1 and 2, B on cell 1.    Forward tab order: A, B
        ///                                                             Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void flow_table3()
        {
            string[] authors = new string[] { "A", "B" };
            CreateStickyNote(new TableSelectionData(SimpleTable, 1, 1, 1, 4), authors[1]);
            CreateStickyNote(new TableSelectionData(SimpleTable, 1, 1, 1, 2), authors[0]);            
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations	            Initial State	                        Action.
        /// Iconified               One note within figure, one after.      Forward tab order: A, B
        ///                                                                 Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void flow_anchoredblock1()
        {
            string[] authors = new string[] { "A", "B" };
            CreateStickyNote(new AnchoredBlockSelectionData(AnchoredBlockType, AnchoredBlockName, PagePosition.Beginning, 1, PagePosition.End, -1), authors[0]);
            CreateStickyNote(new SimpleSelectionData(AnchoredBlockPage, 900, 25), authors[1]);
            TestTabOrder(authors);
        }

        /// <summary>
        /// Variations	            Initial State	                        Action.
        /// Iconified               Both notes inside the figure.           Forward tab order: A, B
        ///                                                                 Backward tab order: B, A
        /// </summary>
        [Priority(1)]
        protected void flow_anchoredblock2()
        {
            string[] authors = new string[] { "A", "B" };
            CreateStickyNote(new AnchoredBlockSelectionData(AnchoredBlockType, AnchoredBlockName, PagePosition.End, -10, PagePosition.End, -5), authors[1]);
            CreateStickyNote(new AnchoredBlockSelectionData(AnchoredBlockType, AnchoredBlockName, PagePosition.Middle, -2, PagePosition.Middle, 0), authors[0]);
            TestTabOrder(authors);
        }

        #region Fields

        static string SimpleTable = "SimpleTable";
        static string Figure1 = "Figure1";
        static string Floater1 = "Floater1";


        Type AnchoredBlockType;
        string AnchoredBlockName;
        int AnchoredBlockPage;

        #endregion

        #endregion PRIORITY TESTS
    }    
}	

