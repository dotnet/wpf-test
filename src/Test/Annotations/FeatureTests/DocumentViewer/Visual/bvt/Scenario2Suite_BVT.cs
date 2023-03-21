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
	/// See Scenario2 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario2Suite_BVT : AScenarioSuite
	{
		protected override AScenario SelectScenario(string testname)
		{
			Scenario2 Scenario = null;
			if (testname.Contains("a_"))
				Scenario = new Scenario2a();
			else if (testname.Contains("b_"))
				Scenario = new Scenario2b();
			else
				failTest("Unknown Scenario type '" + testname + "'.");
			return Scenario;
		}

        protected new Scenario2 Scenario
        {
            get
            {
                return (Scenario2) base.Scenario;
            }
        }

		#region Tests

		/// <summary>
		/// Create annotation at middle of non-edge page.
		/// </summary>
        [TestCase_Helper()]
		private void scenario2_5()
		{
            Script.Add("GoToPage", new object[] { 2 });
			Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_1067_to_1261));

			Scenario.VisiblePage = 2;
            Scenario.InitialAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_1067_to_1261);
            Scenario.ExpectedNumAttachedAnnotations = 0;

			RunScenario(Script, Scenario);
		}
        private void scenario2a_5() { scenario2_5(); }
        private void scenario2b_5() { scenario2_5(); }

		/// <summary>
		/// Anchor spans 1 page break.
		/// </summary>
        [TestCase_Helper()]
        private void scenario2_6()
		{
            ISelectionData selection = new MultiPageSelectionData(2, PagePosition.End, -52, 3, PagePosition.Beginning, 100);
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();

			Script.Add("GoToPageRange", new object[] { 2, 3 });
            Script.Add("CreateAnnotation", new object[] { selection });

			Scenario.VisiblePages = new int[] { 2, 3 };
			Scenario.NumPageChanges = 2;

			Scenario.InitialAttachedAnchor = GetText(selection);
			Scenario.ExpectedNumAttachedAnnotations = 0;

			RunScenario(Script, Scenario);
		}
        private void scenario2a_6() { scenario2_6(); }
        private void scenario2b_6() { scenario2_6(); }

		#endregion Tests
	}
}	

