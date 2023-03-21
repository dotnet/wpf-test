// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Annotations;
using System.Windows.Controls;
using Annotations.Test.Reflection;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Ink;
using System.Windows.Media;
using System.Xml;
using Proxies.MS.Internal.Annotations;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class APISuite_BVT : AStickyNoteControlAPISuite
    {
        #region BVT TESTS

        #region AuthorProperty

        /// <summary>
        /// Set author.	Verify author value.
        /// </summary>
        [Priority(0)]
        protected void author1()
        {
            CreateAnnotation(new SimpleSelectionData(0, PagePosition.End, -50), "TestA");
            AssertEquals("Verif author.", "TestA", Note.GetValue(StickyNoteControl.AuthorProperty));
            passTest("Verify author was set properly.");
        }

        /// <summary>
        /// Verify author is String.Empty by default.
        /// </summary>
        [Priority(0)]
        protected void author2()
        {
            CreateDefaultNote();
            AssertEquals("Verif author.", String.Empty, Note.GetValue(StickyNoteControl.AuthorProperty));
            passTest("Verify author default");
        }

        #endregion

        #region IsExpanded

        /// <summary>
        /// Create SN via CAF	IsExpanded == true.
        /// </summary>
        [Priority(0)]
        protected void isexpanded1()
        {
            CreateDefaultNote();
            Assert("Verify note is expanded.", Note.IsExpanded);
            passTest("Note is expanded by default.");
        }

        ///// <summary>
        ///// Iconify SN.	IsExpanded == false.
        ///// </summary>
        //[Priority(0)]
        //protected void isexpanded2()
        //{
        //    CreateDefaultNote();
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    Assert("Verify note is not expanded.", !Note.IsExpanded);
        //    passTest("Note is not expanded after mouse event.");
        //}

        #endregion

        #region IsActive

        ///// <summary>
        ///// Iconify.	IsActive == false.
        ///// </summary>
        //[Priority(0)]
        //protected void isactive2()
        //{
        //    CreateDefaultNote();
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    Assert("Verify inactive.", !Note.IsActive);
        //    passTest("Note is inactive after being iconified.");
        //}

        /// <summary>
        /// Iconify.
        /// Restore note	
        /// Verify: IsActive == true.
        /// </summary>
        [OverrideClassTestDimensions]
        [TestDimension("fixed, flow")]
        [TestDimension("stickynote, inkstickynote")]
        [Priority(0)]
        protected void isactive3()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify active.", Note.IsActive);
            passTest("Note is active after being iconified and restored.");
        }

        /// <summary>
        /// Create SN via CAF.
        /// Focus Viewer.
        /// Click in SN	
        /// Verify: IsActive == true.
        /// </summary>
        [Priority(0)]
        protected void isactive5()
        {
            CreateDefaultNote();
            ViewerBase.Focus();
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify active.", Note.IsActive);
            passTest("Note is active after being clicked in.");
        }

        #endregion

        #region IsMouseOverAnchor

        [TestCase_Setup("ismouseoveranchor.*")]
        private void DefaultSetup()
        {
            DoSetup();
            CreateAnnotation(new SimpleSelectionData(0, 100, 500));
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
        }

        /// <summary>
        /// Position mouse over start of anchor.	
        /// Verify: True.
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void ismouseoveranchor1_1()
        {
            TestMouseOverAnchor(100, true);
            passTest("Verified mouse over anchor.");
        }
        /// <summary>
        /// Position mouse over end of anchor.	
        /// Verify: True.
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor1_2()
        {
            TestMouseOverAnchor(600, true);
            passTest("Verified mouse over anchor.");
        }
        /// <summary>
        /// Position mouse over middle of anchor.	
        /// Verify: True.
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor1_3()
        {
            TestMouseOverAnchor(350, true);
            passTest("Verified mouse over anchor.");
        }
        /// <summary>
        /// Position mouse 1 char before start of anchor.	
        /// Verify: False
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor1_4()
        {
            if (DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                TestMouseOverAnchor(99, false);
            }
            passTest("Verified mouse over anchor.");
        }
        /// <summary>
        /// Position mouse 1 char after end of anchor.	
        /// Verify: False
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor1_5()
        {
            if (DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                TestMouseOverAnchor(602, false);
            }
            passTest("Verified mouse over anchor.");
        }

        [TestCase_Helper()]
        protected void TestMouseOverAnchor(int position, bool isMouseOverAnchor)
        {
            MoveMouseToCharacterOffset(0, position);
            AssertEquals("Verify value of IsMouseOverAnchor property.", isMouseOverAnchor, Note.IsMouseOverAnchor);
        }

        /// <summary>
        /// Position mouse over icon.	
        /// Verify: True.
        /// </summary>
        [OverrideClassTestDimensions()]
        [TestDimension("fixed")]
        [TestDimension("stickynote, inkstickynote")]
        [Priority(0)]
        protected void ismouseoveranchor3()
        {
            CurrentlyAttachedStickyNote.MoveTo();
            DispatcherHelper.DoEvents();
            Assert("Verify IsMouseOverAnchor.", Note.IsMouseOverAnchor);
            passTest("Verified for mouse over icon.");
        }

        /// <summary>
        /// Two overlapping anchors.  Mouse over both.	
        /// Verify: True for both.
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor5()
        {
            StickyNoteWrapper[] notes = this.GetStickyNoteWrappers();
            notes[0].MinimizeWithMouse();
            notes = this.GetStickyNoteWrappers();
            notes[1].MinimizeWithMouse();

            UIAutomationModule.MoveTo(DocViewerWrapper.PointerToScreenCoordinates(0, 350, LogicalDirection.Forward, ADocumentViewerBaseWrapper.HorizontalJustification.Bottom));
            DispatcherHelper.DoEvents(); // Wait for property to get set.

            notes = this.GetStickyNoteWrappers();
            Assert("Verify IsMouseOverAnchor for note A.", notes[0].Target.IsMouseOverAnchor);
            Assert("Verify IsMouseOverAnchor for note B.", notes[1].Target.IsMouseOverAnchor);
            passTest("Verified mouse over anchors.");
        }
        [TestCase_Setup("ismouseoveranchor5")]
        private void SetupIsMouseOverAnchor5()
        {
            DoSetup();
            ISelectionData selection = new SimpleSelectionData(0, 100, 500);
            CreateAnnotation(selection);
            CreateAnnotation(selection);
        }

        /// <summary>
        /// 1. Mouse over anchor.       1. True.
        /// 2. Mouse not over anchor.	2. False.
        /// </summary>
        [Priority(0)]
        protected void ismouseoveranchor6()
        {
            if (DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                TestMouseOverAnchor(303, true);
                TestMouseOverAnchor(1000, false);
            }
            passTest("Verified transition of IsMouseOverAnchor state.");
        }

        #endregion

        #region Font Defaults

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void captionfontfamily1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", new FontFamily("Segoe UI,Arial"), Note.CaptionFontFamily);
            passTest("Verified font default.");
        }

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void captionfontsize1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", 9.0, Note.CaptionFontSize);
            passTest("Verified font default.");
        }

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void captionfontstretch1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", FontStretches.Normal, Note.CaptionFontStretch);
            passTest("Verified font default.");
        }

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void captionfontstyle1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", SystemFonts.MessageFontStyle, Note.CaptionFontStyle);
            passTest("Verified font default.");
        }

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void captionfontweight1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", SystemFonts.MessageFontWeight, Note.CaptionFontWeight);
            passTest("Verified font default.");
        }

        /// <summary>
        /// Verify default.
        /// </summary>
        [Priority(0)]
        protected void penwidth1()
        {
            CreateDefaultNote();
            AssertEquals("Verify default.", new DrawingAttributes().Width, Note.PenWidth);
            passTest("Verified font default.");
        }

        #endregion

        #region DeleteNoteCommand

        [Priority(0)]
        protected void deletenotecommand1()
        {
            CreateDefaultNote();
            StickyNoteControl.DeleteNoteCommand.Execute(null, Note);
            DispatcherHelper.DoEvents();
            VerifyNumberOfAttachedAnnotations(0);
            passTest("Note deleted.");
        }

        #endregion

        #region InkCommand

        [Priority(0)]
        protected void inkcommand1()
        {
            TestInkCommand(InkCanvasEditingMode.Ink, InkCanvasEditingMode.Ink);
        }
        [Priority(0)]
        protected void inkcommand2()
        {
            TestInkCommand(InkCanvasEditingMode.Select, InkCanvasEditingMode.Select);
        }
        [Priority(0)]
        protected void inkcommand3()
        {
            TestInkCommand(InkCanvasEditingMode.EraseByStroke, InkCanvasEditingMode.EraseByStroke);
        }

        [Priority(0)]
        protected void TestInkCommand(InkCanvasEditingMode snInkMode, InkCanvasEditingMode canvasInkMode)
        {
            CreateDefaultNote();
            if (AnnotationType == AnnotationMode.InkStickyNote)
            {
                Assert("Verify command is enabled.", StickyNoteControl.InkCommand.CanExecute(null, Note));
                StickyNoteControl.InkCommand.Execute(snInkMode, Note);
                DispatcherHelper.DoEvents();
                VerifyInkCanvasEditMode(canvasInkMode);
            }
            else
                Assert("Verify command is disabled.", !StickyNoteControl.InkCommand.CanExecute(null, Note));

            passTest("Verified InkCommand.");
        }

        #endregion

        #region Public Fields

        //[Priority(0)]protected void schemanamespace1()
        //{
        //    AssertEquals("Verify namespace.", "http://schemas.microsoft.com/stickynote", StickyNoteControl.SchemaNamespace);
        //    passTest("Verified static field");
        //}

        [Priority(0)]
        protected void textschemanamespace1()
        {
            AssertEquals("Verify namespace.", new XmlQualifiedName("TextStickyNote", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace), StickyNoteControl.TextSchemaName);
            passTest("Verified static field");
        }

        [Priority(0)]
        protected void inkschemanamespace1()
        {
            AssertEquals("Verify namespace.", new XmlQualifiedName("InkStickyNote", AnnotationXmlConstants.Namespaces.BaseSchemaNamespace), StickyNoteControl.InkSchemaName);
            passTest("Verified static field");
        }

        #endregion

        #endregion BVT TESTS
    }
}

