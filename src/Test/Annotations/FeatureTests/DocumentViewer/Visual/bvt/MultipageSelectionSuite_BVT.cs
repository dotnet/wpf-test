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
using Proxies.System.Windows.Annotations;
using System.Windows.Annotations.Storage;
using Proxies.MS.Internal.Annotations.Anchoring;

using Annotations.Test;
using Annotations.Test.Framework;
using System.Collections.Generic;					// TestSuite.

namespace Avalon.Test.Annotations.BVTs
{
	/// <summary>
	/// Visual test cases specifically for testing the behavior of multipage selections.
	/// </summary>
	public class MultipageSelectionSuite_BVT : AScenarioSuite
	{
		protected override AScenario SelectScenario(string testname)
		{
			return null;
        }

        /// <summary>
        /// 1. Annotation across from page N to N+1.
        /// 2. Page down.
        /// 3. Verify annotation is attached to N+1.
        /// </summary>
        private void multipage0()
        {
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();

			MultiPageSelectionData selection = new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50);
			ISelectionData lastPage = new SimpleSelectionData(2, PagePosition.Beginning, 50);

			GoToPageRange(1, 2);
			selection.SetSelection(DocViewerWrapper.SelectionModule);
			CreateAnnotation(true);
			VerifyAnnotation(GetText(selection));
			PageDown();
			VerifyAnnotation(GetText(lastPage));
			passTest("Verified page N+1.");
        }

        /// <summary>
        /// 1. Annotation across from page N to N+1.
        /// 2. Page up.
        /// 3. Verify annotation is attached to N.
        /// </summary>
        private void multipage1()
        {
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();

			MultiPageSelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -50, 3, PagePosition.Beginning, 50);
			ISelectionData firstPage = new SimpleSelectionData(2, PagePosition.End, -50);

			GoToPageRange(2, 3);
			selection.SetSelection(DocViewerWrapper.SelectionModule);
			CreateAnnotation(true);
			VerifyAnnotation(GetText(selection));

			if (ContentMode == TestMode.Fixed)
			{
				while (PageIsVisible(3))
					DocViewerWrapper.ScrollUp(1);
			}
			else
			{
				PageUp();
			}
            VerifyAnnotation(GetText(firstPage));
			passTest("Verified page N.");
        }

        /// <summary>
        /// Create two overlapping annotations that span page boundary.
        /// Page Down.
        /// </summary>
        private void multipage2()
        {
            MultiPageSelectionData selection1 = new MultiPageSelectionData(0, PagePosition.End, -651, 1, PagePosition.Beginning, 984);
            MultiPageSelectionData selection2 = new MultiPageSelectionData(0, PagePosition.End, -215, 1, PagePosition.Beginning, 671);
            CreateAnnotation(selection1);
            CreateAnnotation(selection2);
            VerifyNumberOfAttachedAnnotations(2);
            PageDown();
            VerifyNumberOfAttachedAnnotations(2);
            passTest("Verified overlapping annotations across page boundary.");
        }
	}
}	

