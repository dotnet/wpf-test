// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTs verifying functionality of StickyNote ContextMenu.

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

using Microsoft.Test.Input;
using System.Windows.Input;
using Annotations.Test.Reflection;
using System.Windows.Media;
using System.Windows.Ink;
using System.Text.RegularExpressions;

namespace Avalon.Test.Annotations.Suites
{
    /// <summary>
    /// NOTE: context menu is only applicable to TextNotes.
    /// </summary>
    public class ContextMenuSuite_BVT : AStickyNoteControlFunctionalSuite
    {
        #region BVT TESTS

        #region Overrides

        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);

            foreach (string arg in args)
            {
                if (arg.Equals("/accesskeys=false"))
                {
                    _usingAccessKeys = false;
                    break;
                }
            }
        }

        protected override AnnotationMode DetermineAnnotationMode(string[] args)
        {
            return AnnotationMode.StickyNote;
        }

        #endregion

        /// <summary>
        /// Copy/Paste.
        /// </summary>
        [Priority(0)]
        protected void contextmenu1()
        {
            TestCutCopyPaste(false);
        }

        /// <summary>
        /// Cut/Paste.
        /// </summary>
        [Priority(0)]
        protected void contextmenu2()
        {
            TestCutCopyPaste(true);
        }

        [TestCase_Helper()]
        private void TestCutCopyPaste(bool doCut) 
        {
            CreateDefaultNote();
            CurrentlyAttachedStickyNote.ContextMenu.UseAccessKeys = _usingAccessKeys;

            SetContent(ContentKind.Standard_Small);
            Clipboard.Clear();
            CurrentlyAttachedStickyNote.SelectAll();
            
            if (doCut)
            {
                CurrentlyAttachedStickyNote.ContextMenu.Cut(StickyNoteWrapper.ContextMenuWrapper.Position.UpperLeft);
                Assert("Verify no content.", !CurrentlyAttachedStickyNote.HasContent);
            }
            else
            {
                CurrentlyAttachedStickyNote.ContextMenu.Copy(StickyNoteWrapper.ContextMenuWrapper.Position.UpperLeft);
                VerifyContent(ContentKind.Standard_Small);
            }
            
            printStatus("Verified: Menu->" + ((doCut) ? "Cut" : "Copy"));

            DeleteDefaultNote();
            VerifyNumberOfAttachedAnnotations(0);
            CreateDefaultNote();            
            CurrentlyAttachedStickyNote.ContextMenu.Paste(StickyNoteWrapper.ContextMenuWrapper.Position.UpperLeft);            
            VerifyContent(ContentKind.Standard_Small);
            printStatus("Verified: Menu->Paste");

            passTest("Verified ContextMenu usage.");
        }        

        #region Fields

        bool _usingAccessKeys = true;

        #endregion

        #endregion BVT TESTS
    }
}	

