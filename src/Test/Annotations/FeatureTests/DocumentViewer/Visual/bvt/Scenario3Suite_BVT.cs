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
	/// See Scenario3 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario3Suite_BVT : AScenarioSuite
	{
		protected override AScenario SelectScenario(string testname)
		{
			if (testname.Contains("3a_")) 
				return new Scenario3a(this);
			if (testname.Contains("3b_"))
				return new Scenario3b(this);
			throw new ArgumentException("Unknown Scenario type: '" + testname + "'.");
		}

        protected new Scenario3 Scenario
        {
            get
            {
                return (Scenario3) base.Scenario;
            }
        }

		#region Test Cases

		/// <summary>
		/// Annotation on middle of non-edge page.
		/// </summary>
        [TestCase_Helper()]
		private void scenario3_5()
		{
			if (ContentMode == TestMode.Flow)
			{
				ViewAsTwoPages();
				Scenario.ViewAsTwoPages = true;
			}
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 3;
            Scenario.Selections = new ISelectionData[] { new SimpleSelectionData(3, 250, 300) };
            RunScenario(Script, Scenario);
		}
        private void scenario3a_5() { scenario3_5(); }
        private void scenario3b_5() { scenario3_5(); }

		/// <summary>
		/// Annotation across 1 page break.
		/// </summary>
        [TestCase_Helper()]
        private void scenario3_6()
		{
			if (ContentMode == TestMode.Flow)
			{
				ViewAsTwoPages();
				Scenario.ViewAsTwoPages = true;
			}

            Scenario.FirstVisiblePage = 3;
            Scenario.LastVisiblePage = 4;
            Scenario.Selections = new ISelectionData[] { new MultiPageSelectionData(3, PagePosition.End, -33, 4, PagePosition.Beginning, 70) };			           
			RunScenario(Script, Scenario);
		}
        private void scenario3a_6() { scenario3_6(); }
        private void scenario3b_6() { scenario3_6(); }

		#endregion Test Cases
	}
}	

