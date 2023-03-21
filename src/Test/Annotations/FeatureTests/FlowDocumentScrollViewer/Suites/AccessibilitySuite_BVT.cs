// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;
using System.Windows.Media;
using System.Windows.Input;

namespace Avalon.Test.Annotations.Bvts
{
    public class AccessibilitySuite_BVT : AFlowDocumentScrollViewerSuite
    {
        #region Tests

        /// <summary>
        /// Anchor not visible. Tab to annotation, verify it is scrolled into view.
        /// </summary>
        protected void accessibility1()
        {
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.End, -200, PagePosition.End, -100), AnnotationType, true, "A"));
            GoToStart();
            VerifyNoteViewportVisibility("A", false);
            TestCanTabTo("A");
        }

        /// <summary>
        /// Whole document annotate, start of anchor in view, note not in view.
        /// Tab brings note into view.
        /// </summary>
        protected void accessibility5()
        {
            CreateAnnotation(new StickyNoteDefinition(new AbsoluteSelectionData(PagePosition.Beginning, 0, PagePosition.End, 0), AnnotationType, true, "A"));            
            CurrentlyAttachedStickyNote.Drag(new Vector(0, 2000));
            SetZoom(125);
            GoToStart();
            VerifyNoteViewportVisibility("A", false);
            TestCanTabTo("A");
        }

        #endregion
    }
}	

