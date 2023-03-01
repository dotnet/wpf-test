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

namespace Avalon.Test.Annotations.Pri1s
{
	/// <summary>
	/// See Scenario1 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario1Suite : AScenarioSuite
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

		/// <summary>
		/// Create annotation at start of document.
		/// </summary>
		private void scenario1_1()
		{
			Script.Add("CreateAnnotation", SelectionData(SelectionType.StartOfDocument));
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.StartOfDocument);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Create annotation at end of document.
		/// </summary>
		private void scenario1_2()
		{
			Script.Add("GoToLastPage");
			Script.Add("VerifyPageIsVisible", new object[] { 4 });
			Script.Add("CreateAnnotation", SelectionData(SelectionType.EndOfDocDocument));
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.EndOfDocDocument);
            RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Anchor spanning whole document.
		/// </summary>
		private void scenario1_8()
		{
            MakeWholeDocumentVisible();
            Script.Add("VerifyPageIsVisible", new object[] { 0 });
            Script.Add("VerifyPageIsVisible", new object[] { 4 });
            ISelectionData selection = new MultiPageSelectionData(0, PagePosition.Beginning, 0, 4, PagePosition.End, 0);
            Script.Add("CreateAnnotation", new object[] { selection });
            Scenario.ExpectedAttachedAnchor = GetText(selection);
            RunScenario(Script, Scenario);
        }

		/// <summary>
		/// Zero length anchor at start of document.
		/// </summary>
		private void scenario1_9()
		{
			VerifyCreateAnnotationFails(new SimpleSelectionData(0, 0, 0), typeof(InvalidOperationException));
			passTest("Exception for zero length anchor at start of doc.");
		}

		/// <summary>
		/// Zero length anchor at end of document.
		/// </summary>
		private void scenario1_10()
		{
			GoToPage(4);
			VerifyCreateAnnotationFails(new SimpleSelectionData(4, PagePosition.End, 0), typeof(InvalidOperationException));
			passTest("Exception for zero length anchor at end of doc.");
		}


        /// <summary>
        /// Two annotations with same anchor.
        /// </summary>
        private void scenario1_14()
        {
            Script.Add("GoToPage", new object[] { 1 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_2215_to_2305));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_2215_to_2305));
            if (this.AnnotationType == AnnotationMode.Highlight)
                Scenario.ExpectedAttachedAnchors = new string[] { ExpectedSelectedText(SelectionType.Page1_2215_to_2305) };
            else
                Scenario.ExpectedAttachedAnchors = new string[] { ExpectedSelectedText(SelectionType.Page1_2215_to_2305), ExpectedSelectedText(SelectionType.Page1_2215_to_2305) };
            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Two annotations, where Annotation A’s anchor contains B’s anchor.
        /// </summary>
        private void scenario1_15()
        {
            ISelectionData selection1 = new SimpleSelectionData(1, 100, 50);
            ISelectionData selection2 = new SimpleSelectionData(1, 110, 10);
            Script.Add("GoToPage", new object[] { 1 });
            Script.Add("CreateAnnotation", new object[] { selection1 });
            Script.Add("CreateAnnotation", new object[] { selection2 });
            if (this.AnnotationType == AnnotationMode.Highlight)
                Scenario.ExpectedAttachedAnchors = new string[] { GetText(new SimpleSelectionData(1, 100, 10)) + GetText(new SimpleSelectionData(1, 120, 30)), GetText(new SimpleSelectionData(1, 110, 10)) };
            else
                Scenario.ExpectedAttachedAnchors = new string[] { GetText(selection1), GetText(selection2) };
            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Two annotations, overlap between anchors.
        /// </summary>
        private void scenario1_16()
        {
            Script.Add("GoToPage", new object[] { 1 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_20_to_30));
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page1_25_to_40));
            if (this.AnnotationType == AnnotationMode.Highlight)
                Scenario.ExpectedAttachedAnchors = new string[] { ExpectedSelectedText(SelectionType.Page1_20_to_25), ExpectedSelectedText(SelectionType.Page1_25_to_40) };
            else
                Scenario.ExpectedAttachedAnchors = new string[] { ExpectedSelectedText(SelectionType.Page1_20_to_30), ExpectedSelectedText(SelectionType.Page1_25_to_40) };
            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Annotation from A-B: page B is visible.
        /// </summary>
        private void scenario1_18b()
        {
            ISelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -256, 3, PagePosition.Beginning, 401);
            string expectedAnchor = GetText(selection);
            GoToPage(3);
            VerifyPageIsNotVisible(2);
            CreateAnnotation(selection);
            GoToPageRange(2, 3);
            VerifyAnnotation(expectedAnchor);
            passTest("Verified creating annotation on non-visible page-break B.");
        }
	}
}	

