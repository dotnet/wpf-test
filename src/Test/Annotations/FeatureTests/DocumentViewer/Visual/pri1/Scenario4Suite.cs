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
	/// See Scenario4 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario4Suite : AScenarioSuite
	{
        protected new Scenario4 Scenario
        {
            get
            {
                return (Scenario4)base.Scenario;
            }
        }
		protected override AScenario SelectScenario(string testname)
		{
			return new Scenario4(DocViewerWrapper);
		}

		#region Test Cases

		/// <summary>
		/// Create annotation at start of document.
		/// </summary>
		private void scenario4_1()
		{
            Scenario.SelectionData = new SimpleSelectionData(0, PagePosition.Beginning, 20);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Create annotation at end of document.
		/// </summary>
        private void scenario4_2()
		{
            ViewAsTwoPages();
            Scenario.FirstVisiblePage = 4;
            Scenario.LastVisiblePage = 4;
            Scenario.SelectionData = new SimpleSelectionData(4, PagePosition.End, -50);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
			RunScenario(Script, Scenario);
		}


        /// <summary>
        /// Create annotation at start of non-edge page.
        /// </summary>
        private void scenario4_3()
        {
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 2;
            Scenario.SelectionData = new SimpleSelectionData(2, PagePosition.Beginning, 50);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
            Scenario.ResizedWindowSize = new Size(200, 300);
            RunScenario(Script, Scenario);
        }

        /// <summary>
        ///  Create annotation at end of non-edge page.
        /// </summary>
        private void scenario4_4()
        {
            Scenario.FirstVisiblePage = 1;
            Scenario.LastVisiblePage = 1;
            Scenario.SelectionData = new SimpleSelectionData(1, PagePosition.End, -50);
            Scenario.ResizedWindowSize = new Size(208, 296);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
            RunScenario(Script, Scenario);
        }

		/// <summary>
		/// Anchor spanning N page breaks.
		/// </summary>
        [OverrideClassTestDimensions]
        private void scenario4_7()
		{
			Script.Add("PageLayout", new object[] { 4 });
            Script.Add("VerifyPageIsVisible", new object[] { 1 });
            Script.Add("VerifyPageIsVisible", new object[] { 3 });
            Scenario.SelectionData = new MultiPageSelectionData(1, PagePosition.End, -50, 3, PagePosition.Beginning, 100);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
            Scenario.ResizedWindowSize = new Size(900, 500);
            Scenario.ZoomOutPercentage = 75;
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Anchor spanning whole document.
		/// </summary>
        [OverrideClassTestDimensions]
        private void scenario4_8()
		{
			Script.Add("PageLayout", new object[] { 5 });
            Script.Add("VerifyPageIsVisible", new object[] { 0 });
            Script.Add("VerifyPageIsVisible", new object[] { 4 });
            Scenario.SelectionData = new MultiPageSelectionData(0, PagePosition.Beginning, 0, 4, PagePosition.End, 0);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);
			RunScenario(Script, Scenario);
		}
		
		#endregion Test Cases
	}
}	

