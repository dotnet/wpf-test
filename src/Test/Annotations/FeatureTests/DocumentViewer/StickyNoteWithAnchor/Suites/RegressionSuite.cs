// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Suites
{
    public class RegressionSuite : AStickyNoteWithAnchorSuite
    {
        #region PRIORITY TESTS

        protected override void SetDocumentViewerContent()
        {
            if (CaseNumber.Contains("drt"))
            {
                SetContent(AnnotationTestHelper.LoadContent("Flow_DRT.xaml"));
            }
            else
            {
                base.SetDocumentViewerContent();
            }
        }

        [OverrideClassTestDimensions]
        [TestDimension("flow")]
        protected void regression_drt1()
        {
            SetWindowWidth(612);
            SetWindowHeight(400);
            GoToLastPage();
            int pageCount = ViewerBase.PageCount;
            CreateAnnotation(new MultiPageSelectionData(pageCount - 2, PagePosition.End, -1, pageCount - 1, PagePosition.Beginning, 1));
            PageUp();
            printStatus("Verifing page n-1.");
            VerifyNumberOfAttachedAnnotations(1);
            PageDown();
            printStatus("Verifing page n.");
            VerifyNumberOfAttachedAnnotations(1);
            passTest("Verified anchor appears on both pages.");
        }

        #endregion PRIORITY TESTS
    }
}

