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
	/// See Scenario5 in ScenarioDefinitions.cs.
	/// </summary>
    [OverrideClassTestDimensions]
    [TestDimension("stickynote,highlight")]
    public class Scenario5Suite : AScenarioSuite
	{
        protected new Scenario5 Scenario
        {
            get
            {
                return (Scenario5)base.Scenario;
            }
        }
		protected override AScenario SelectScenario(string testname)
		{
			return new Scenario5();
		}
        protected override TestMode DetermineTestMode(string[] args)
        {
            TestMode mode = base.DetermineTestMode(args);
            if (mode == TestMode.Flow)
                throw new Exception("Scenario5 tests are for FixedContent only.");
            return mode;
        }

		#region Test Cases

		/// <summary>
		/// Create annotation at start of document.
		/// </summary>
		private void scenario5_1()
		{
			Scenario.SelectionData = SelectionData(SelectionType.StartOfDocument);
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.StartOfDocument);
			RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Create annotation at end of document.
		/// </summary>
		private void scenario5_2()
		{
            Scenario.FirstVisiblePage = 4;
            Scenario.LastVisiblePage = 4;
			Scenario.SelectionData = SelectionData(SelectionType.EndOfDocDocument);
			Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.EndOfDocDocument);
			RunScenario(Script, Scenario);
		}

        /// <summary>
        /// Create annotation at start of non-edge page.
        /// </summary>
        private void scenario5_3()
        {
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 2;
            Scenario.SelectionData = SelectionData(SelectionType.Page2_Start);
            Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_Start);
            RunScenario(Script, Scenario);
        }

        /// <summary>
        ///  Create annotation at end of non-edge page.
        /// </summary>
        private void scenario5_4()
        {
            Scenario.FirstVisiblePage = 1;
            Scenario.LastVisiblePage = 1;
            Scenario.SelectionData = SelectionData(SelectionType.Page1_End);
            Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page1_End);
            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Create annotation at middle of non-edge page.
        /// </summary>
        private void scenario5_5()
        {
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 2;
            Scenario.SelectionData = SelectionData(SelectionType.Page2_1067_to_1261);
            Scenario.ExpectedAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_1067_to_1261);
            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Anchor spanning 1 page break.
        /// </summary>
        private void scenario5_6()
        {
            Scenario.FirstVisiblePage = 1;
            Scenario.LastVisiblePage = 2;
            Scenario.SelectionData = SelectionData(SelectionType.Page1_to_Page2);
            Scenario.ExpectedAttachedAnchor = GetText(new MultiPageSelectionData(1, PagePosition.End, -50, 2, PagePosition.Beginning, 50));
            RunScenario(Script, Scenario);
        }

        #endregion Test Cases
	}
}	

