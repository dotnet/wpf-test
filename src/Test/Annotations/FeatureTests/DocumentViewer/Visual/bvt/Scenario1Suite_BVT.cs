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
using System.Collections.Generic;

namespace Avalon.Test.Annotations.BVTs
{
	/// <summary>
	/// See Scenario1 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario1Suite_BVT : AScenarioSuite
	{
		protected override AScenario SelectScenario(string testname)
		{
			return new Scenario1();
		}

        protected new Scenario1 Scenario
        {
            get
            {
                return (Scenario1) base.Scenario;
            }
        }

		#region Script Based Cases

		/// <summary>
		/// Create annotation at start of non-edge page.
		/// </summary>
		private void scenario1_3()
		{
            Script.Add("GoToPage", new object[] { 2 });
            
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_Start));
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_Start);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		///  Create annotation at end of non-edge page.
		/// </summary>
		private void scenario1_4()
		{
            Script.Add("GoToPage", new object[] { 1 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_End));
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page1_End);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Create annotation at middle of non-edge page.
		/// </summary>
		private void scenario1_5()
		{
            Script.Add("GoToPage", new object[] { 3 });
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page3_250_to_300));
            Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page3_250_to_300);
            RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Anchor spanning 1 page break.
		/// </summary>
		private void scenario1_6()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
                        Script.Add("GoToPageRange", new object[] { 1, 2 });
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50);
            Script.Add("CreateAnnotation", new object[] { selection });
            Scenario.ExpectedAttachedAnchor = GetText(selection);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Anchor spanning N page breaks.
		/// </summary>
		private void scenario1_7()
		{
            MakeWholeDocumentVisible();
			Script.Add("VerifyPageIsVisible", new object[] { 1 });
			Script.Add("VerifyPageIsVisible", new object[] { 3 });
            ISelectionData selection = new MultiPageSelectionData(1, PagePosition.End, -50, 3, PagePosition.Beginning, 100);
            Script.Add("CreateAnnotation", new object[] { selection });
            Scenario.ExpectedAttachedAnchor = GetText(selection);
			RunScenario(Script, Scenario);
        }

		/// <summary>
		/// Zero length anchor at middle of document.
		/// </summary>
		private void scenario1_11()
		{
			GoToPage(1);
			VerifyCreateAnnotationFails(new SimpleSelectionData(1, 1500, 0), typeof(InvalidOperationException));
			passTest("Exception for zero length anchor at middle of doc.");
		}

		/// <summary>
		/// Multiple annotations on same page.
		/// </summary>
		private void scenario1_12()
		{
            Script.Add("GoToPage", new object[] { 1 });
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_90_to_140));
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_2215_to_2305)); 
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_3590_to_3602));
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_20_to_30));
			Scenario.ExpectedAttachedAnchors = new string[] {
													ExpectedSelectedText(SelectionType.Page1_90_to_140), 
													ExpectedSelectedText(SelectionType.Page1_2215_to_2305), 			
													ExpectedSelectedText(SelectionType.Page1_3590_to_3602),
													ExpectedSelectedText(SelectionType.Page1_20_to_30)};
			RunScenario(Script, Scenario);
		}

        #endregion

        /// <summary>
        /// Selection on non-visible page.
        /// </summary>
        private void scenario1_17()
        {
            ISelectionData selection = new SimpleSelectionData(2, 100, 200);
            CreateAnnotation(selection);
            GoToPage(2);
            VerifyAnnotation(GetText(selection));
            passTest("Verified creating annotation on non-visible page.");
        }

        /// <summary>
        /// Annotation from A-B: page A is visible.
        /// </summary>
        private void scenario1_18a()
        {
            ISelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -256, 3, PagePosition.Beginning, 401);
            string expectedAnchor = GetText(selection);
            GoToPage(2);
            VerifyPageIsNotVisible(3);
            CreateAnnotation(selection);
            GoToPageRange(2, 3);
            VerifyAnnotation(expectedAnchor);
            passTest("Verified creating annotation on non-visible page-break A.");
        }

        /// <summary>
        /// Annotation from A-B: neither page is visible..
        /// </summary>
        private void scenario1_18c()
        {
            ISelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -256, 3, PagePosition.Beginning, 401);
            string expectedAnchor = GetText(selection);
            CreateAnnotation(selection);
            GoToPageRange(2, 3);
            VerifyAnnotation(expectedAnchor);
            passTest("Verified creating annotation on non-visible page-break C.");
        }
	}
}	

