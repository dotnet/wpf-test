// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations
{
    [TestDimension("fixed,fixed /fds=false,flow")]
    [TestDimension("stickynote,highlight")]
    public abstract class AScenario1_5Suite : AVisualSuite
    {
        protected void EnsureStickyNote()
        {
            if (AnnotationType == AnnotationMode.Highlight)
                throw new Exception("Given test cases is not compatibile with Highlight annotations.");
        }

        protected void RunScenario(ISelectionData selection)
        {
            CreateAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(1);
            DeleteAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(0);
        }

        protected void RunScenario(ISelectionData selection1, ISelectionData selection2, ISelectionData selectionForDelete)
        {
            CreateAnnotation(selection1);
            CreateAnnotation(selection2);
            VerifyNumberOfAttachedAnnotations(2);
            DeleteAnnotation(selectionForDelete);
            VerifyNumberOfAttachedAnnotations(0);
        }

        protected void RunScenario(MultiPageSelectionData selection, int[] initialPageRange, int[] pageRangeForDelete, int[] finalPageRange)
        {
            GoToPageRange(initialPageRange[0], initialPageRange[1]);
            CreateAnnotation(selection);
            VerifyNumberOfAttachedAnnotations(1);
            GoToPageRange(pageRangeForDelete[0], pageRangeForDelete[1]);
            DeleteAnnotation(selection);
            GoToPageRange(finalPageRange[0], finalPageRange[1]);
            VerifyNumberOfAttachedAnnotations(0);
        }
    }
}

