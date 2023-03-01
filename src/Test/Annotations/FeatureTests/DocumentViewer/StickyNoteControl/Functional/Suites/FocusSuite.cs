// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Controls;

namespace Avalon.Test.Annotations.Suites
{
    public class FocusSuite : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        /// <summary>
        /// Iconfiy SN.	
        /// DocumentViewer receives focus
        /// </summary>
        //[Priority(0)]
        //protected void focus1()
        //{
        //    CreateDefaultNote();
        //    CurrentlyAttachedStickyNote.MinimizeWithMouse();
        //    Assert("Verify DocumentViewer has focus.", ViewerBase.IsFocused);
        //    passTest("Viewer focused after minimize.");
        //}

        /// <summary>
        /// Delete focused SN with command.	
        /// DocumentViewer receives focus
        /// </summary>
        [DisabledTestCase()]
        [Priority(0)]
        protected void focus2()
        {
            CreateDefaultNote();
            StickyNoteControl.DeleteNoteCommand.Execute(null, Note);
            DispatcherHelper.DoEvents();
            Assert("Verify DocumentViewer has focus.", ViewerBase.IsFocused);
            passTest("Viewer focused after delete.");
        }

        /// <summary>
        /// Restore note.	
        /// Note has focus.
        /// </summary>
        [Priority(0)]
        protected void focus3()
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.MinimizeWithMouse();
            CurrentlyAttachedStickyNote.ClickIn();
            Assert("Verify Note.InnerControl has focus.", CurrentlyAttachedStickyNote.InnerControl.IsFocused);
            passTest("Note focused after maximized.");
        }

        #endregion BVT TESTS

        #region PRIORITY TESTS

        /// <summary>
        /// 2 Notes, A and B.
        /// Focus A.
        /// Focus B.
        /// Delete B.
        /// Verify: DV has focus.
        /// </summary>
        [DisabledTestCase()]
        [Priority(1)]
        protected void focus4()
        {
            CreateAnnotation(new SimpleSelectionData(0, 1, 10));
            CreateAnnotation(new SimpleSelectionData(0, 500, 10));
            StickyNoteWrapper[] wrappers = GetStickyNoteWrappers();
            wrappers[0].ClickIn();
            wrappers[1].ClickIn();
            wrappers[1].Menu.Delete.Execute();
            Assert("Verify focus.", ViewerBase.IsFocused);
            passTest("Viewer focused.");
        }

        #endregion PRIORITY TESTS
    }
}	

