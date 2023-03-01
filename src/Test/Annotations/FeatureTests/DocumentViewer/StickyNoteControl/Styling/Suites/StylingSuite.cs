// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Microsoft.Test.Display;

namespace Avalon.Test.Annotations.Suites
{
    public class StylingSuite : AStickyNoteStylingSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Set the background of note using the style.
        /// </summary>
        [Priority(0)]
        protected void style1()
        {
            LoadStyle();
            AssertEquals("Verify background color.", Brushes.Red, Note.Background);
            passTest("Background set without error.");
        }

        /// <summary>
        /// Change behavior on IsMouseOverAnchor
        /// </summary>
        [Priority(0)]
        [DisabledTestCase()]
        protected void style2()
        {
            if (DisplayConfiguration.GetAppearance() != DesktopAppearance.WindowsClassic)
            {
                LoadStyle();
                AssertEquals("Verify initial background color.", Brushes.Pink, Note.Background);
                MoveMouseToCharacterOffset(0, 200);
                AssertEquals("Verify background color with mouse over.", Brushes.Orange, Note.Background);
                MoveMouseToCharacterOffset(0, 500);
                AssertEquals("Verify background color without mouse over.", Brushes.Pink, Note.Background);
            }
            passTest("Can override IsMouseOverAnchor behavior.");
        }

        /// <summary>
        /// Change behavior of IsActive
        /// </summary>
        [Priority(0)]
        protected void style3()
        {
            LoadStyle();
            AssertEquals("Verify Inactive ScaleX.", .5, ((ScaleTransform)Note.RenderTransform).ScaleX);
            CurrentlyAttachedStickyNote.ClickIn();
            AssertEquals("Verify Active ScaleX.", 1, ((ScaleTransform)Note.RenderTransform).ScaleX, 1e-10);
            passTest("Can override IsActive behavior.");
        }


        /// <summary>
        /// Test minimum style requirements:
        /// Override the default style so that we only have a ScrollViewer.
        /// </summary>
        [Priority(0)]
        protected void style4()
        {
            LoadStyle(CaseNumber + "." + AnnotationType);
            if (IsTextNote)
                Assert("Verify RTB exists.", new VisualTreeWalker<RichTextBox>().FindChildren(Note).Count == 1);
            else
                Assert("Verify InkCanvas exists.", new VisualTreeWalker<InkCanvas>().FindChildren(Note).Count == 1);
            passTest("Verified minimum style applied correctly.");
        }

        /// <summary>
        /// Test overriding only the expanded template of a note.
        /// </summary>
        [Priority(0)]
        protected void style5()
        {
            Brush background;
            LoadStyle(CaseNumber + "." + AnnotationType);
            if (IsTextNote)
            {
                IList<RichTextBox> rtbs = new VisualTreeWalker<RichTextBox>().FindChildren(Note);
                Assert("Verify RTB exists.", rtbs.Count == 1);
                background = rtbs[0].Background;
            }
            else
            {
                IList<InkCanvas> canvases = new VisualTreeWalker<InkCanvas>().FindChildren(Note);
                Assert("Verify InkCanvas exists.", canvases.Count == 1);
                background = canvases[0].Background;
            }
            AssertEquals("Verify background color.", Brushes.Yellow, background);
            passTest("Verified Expanded template applied successfully.");
        }

        /// <summary>
        /// Test overriding only the icon template of a note.
        /// </summary>
        //[Priority(0)]
        //protected void style6()
        //{
        //    LoadStyle();
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    IList<DockPanel> panels = new VisualTreeWalker<DockPanel>().FindChildren(Note);
        //    AssertEquals("Verify num DockPanels.", 1, panels.Count);
        //    AssertEquals("Verify background color.", Brushes.Green, panels[0].Background);
        //    passTest("Verified icon template applied successfully.");
        //}

        /// <summary>
        /// Verify that data is preserved for custom control.
        /// </summary>
        [Priority(0)]
        [Keywords("MicroSuite")]
        protected void style8()
        {
            LoadStyle("style4." + AnnotationType);
            SetContent(ContentKind.Standard_Small);
            DisableAnnotationService();
            SetupAnnotationService();
            VerifyContent(ContentKind.Standard_Small);
            printStatus("Verified content preserved when service is toggled.");

            SetResourceDictionary(null); // Reset style to default.
            VerifyContent(ContentKind.Standard_Small);
            printStatus("Verified content is preserved during re-style.");

            passTest("Content preserved.");
        }

        /// <summary>
        /// Can change default size of note.
        /// </summary>
        [Priority(0)]
        protected void style14()
        {
            Point initialLocation = UIAutomationModule.BoundingRectangle(Note).TopLeft;
            LoadStyle();
            StickyNoteControl sn = Note;
            AssertEquals("Verify expanded width.", 100, sn.Width, 1e-6);
            AssertEquals("Verify expanded height.", 50, sn.Height, 1e-6);
            AssertEquals("Verify expanded location.", initialLocation, UIAutomationModule.BoundingRectangle(sn).TopLeft);

            CurrentlyAttachedStickyNote.Expanded = false;
            sn = Note;
            AssertEquals("Verify icon width.", 25, sn.Width, 1e-6);
            AssertEquals("Verify icon height.", 45, sn.Height, 1e-6);

            CurrentlyAttachedStickyNote.Expanded = true;
            sn = Note;
            AssertEquals("Verify re-expanded width.", 100, sn.Width, 1e-6);
            AssertEquals("Verify re-expanded height.", 50, sn.Height, 1e-6);

            passTest("Verified styling size.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// Verify controlling IsExpanded state with databinding.
        /// </summary>
        [Priority(1)]
        protected void style7()
        {
            LoadStyle(CaseNumber+"."+AnnotationType);
            AssertEquals("Verify initial Expanded state.", true, Note.IsExpanded);
            UIAutomationModule.MoveToAndClickElement(new VisualTreeWalker<CheckBox>().FindChildren(Note)[0]);
            AssertEquals("Verify note was iconified.", false, Note.IsExpanded);
            UIAutomationModule.MoveToAndClickElement(new VisualTreeWalker<CheckBox>().FindChildren(Note)[0]);
            AssertEquals("Verify note was restored.", true, Note.IsExpanded);
            passTest("Used databinding to control expanded state.");
        }

        /// <summary>
        /// Verify applying MIL Effects doesn't break us.
        /// </summary>
        //[Priority(1)]
        //protected void style15()
        //{
        //    LoadStyle();
        //    CurrentlyAttachedStickyNote.ClickIn();
        //    printStatus("Verify we can still insert content...");
        //    InsertContent(ContentKind.Standard_Small);
        //    VerifyContent(ContentKind.Standard_Small);
        //    printStatus("Verify we can still iconify/restor...");
        //    AssertEquals("Verify initial Expanded state.", true, Note.IsExpanded);
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    AssertEquals("Verify Expanded state after iconified.", false, Note.IsExpanded);
        //    CurrentlyAttachedStickyNote.ClickIn();
        //    AssertEquals("Verify final Expanded state.", true, Note.IsExpanded);
        //    passTest("Verified MIL Effects.");
        //}

        #endregion PRIORITY TESTS
    }
}	

