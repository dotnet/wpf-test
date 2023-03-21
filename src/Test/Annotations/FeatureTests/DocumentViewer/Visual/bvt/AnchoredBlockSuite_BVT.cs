// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: BVTs for annotating elements of type AnchoredBlock (e.g. Figures and Floaters).

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;

namespace Avalon.Test.Annotations.BVTs
{
	public class AnchoredBlockSuite_BVT : AAnchoredBlockSuite
	{
        #region Figure Tests

        /// <summary>
        ///  Annotate across figure.
        /// </summary>
        protected void figure1()
        {
            ISelectionData selection = AnchoredBlockSelection(PagePosition.Beginning, -50, PagePosition.End, 50);
            CreateAnnotation(selection);
            VerifyAnnotation(GetText(selection));
            passTest("Verified annotating across figure.");
        }

        /// <summary>
        ///  Annotate within figure.
        /// </summary>
        protected void figure2()
        {
            ISelectionData selection = AnchoredBlockSelection(PagePosition.Beginning, 10, PagePosition.End, -25);
            CreateAnnotation(selection);
            VerifyAnnotation(GetText(selection));
            passTest("Verified annotating inside figure.");
        }

        #endregion

        #region Floater

        /// <summary>
        ///  Annotate across floater.
        /// </summary>
        [DisabledTestCase()]
        
        protected void floater1()
        {
            ISelectionData selection = AnchoredBlockSelection(PagePosition.Beginning, -50, PagePosition.End, 50);
            TestFloaterAnnotation(selection);
        }

        /// <summary>
        ///  Annotate within floater.
        /// </summary>
        protected void floater2()
        {
            ISelectionData selection = AnchoredBlockSelection(PagePosition.Beginning, 10, PagePosition.End, -25);
            TestFloaterAnnotation(selection);
        }

        protected void TestFloaterAnnotation(ISelectionData selection)
        {
            CreateAnnotation(selection);
            VerifyAnnotation(GetText(selection));

            // Reflow so that floater changes pages.
            SetZoom(112);
            for (int i = 0; i < 10; i++)
                DocViewerWrapper.ZoomIn();

            PageDown();
            passTest("Verified annotating floater.");
        }

        #endregion
    }
}	

