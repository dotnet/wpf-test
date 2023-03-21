// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Reflection;
using System.Windows.Threading;

using Annotations.Test;
using Annotations.Test.Framework;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Annotations;
using System.Xml;					// TestSuite.

namespace Avalon.Test.Annotations.Pri1s
{
	/// <summary>
	/// Misc visual test cases that do not fall into a specific test bin.
	/// </summary>
    [TestDimension("fixed,fixed /fds=false,flow")]    
    public class MiscSuite : AVisualSuite
	{
		#region Tests

		/// <summary>
		/// Test that if we call CreateXXXForSelection without setting any content we throw the correct exception.
		/// </summary>
        [TestDimension("stickynote")]
        private void nocontent1()
		{
			ViewerBase.Document = null;
			DispatcherHelper.DoEvents();
			AssertNull("Verify DocumentViewer has no content.", ViewerBase.Document);
            try
            {
				CreateAnnotation();               
            }
            catch (InvalidOperationException e)
            {
                passTest("Expected exception occurred.");
            }
            failTest("Expected exception but none occurred.");
		}

		/// <summary>
		/// Verify that calling create annotation without making a selection doesn't throw.
		/// </summary>
        [TestDimension("stickynote")]
        private void noselection1()
		{
			SetDocumentViewerContent();
			DispatcherHelper.DoEvents();
			VerifyCreateAnnotationFails(null, typeof(InvalidOperationException));
			passTest("Verified exception for default selection.");
		}

        /// <summary>
        /// Create an annotation that spans > 2 page breaks.  Make sure that it is properly attached
        /// on each page.
        /// </summary>
        [TestDimension("stickynote,highlight")]
        private void longanchor1()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Beginning, 200, TextControlWrapper.PageCount-1, PagePosition.End, -375));
            for (int i=0; i < TextControlWrapper.PageCount; i++) {
                VerifyNumberOfAttachedAnnotations(1);
                PageDown();
            }
            passTest("Verified annotation attached on all pages of very long anchor.");
        }

        /// <summary>
        /// Create an annotation that spans the entire document.  Make sure that it is properly attached
        /// on each page.
        /// </summary>
        [TestDimension("stickynote,highlight")]
        private void longanchor2()
        {
            CreateAnnotation(new MultiPageSelectionData(0, PagePosition.Beginning, 0, TextControlWrapper.PageCount-1, PagePosition.End, 0));
            for (int i=0; i < TextControlWrapper.PageCount; i++) {
                VerifyNumberOfAttachedAnnotations(1);
                PageDown();
            }
            passTest("Verified annotation attached on all pages of very long anchor.");
        }

        #endregion Tests
    }
}	

