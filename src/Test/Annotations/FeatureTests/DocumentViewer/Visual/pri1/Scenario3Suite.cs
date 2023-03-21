// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: 

using System;
using System.Windows;
using Annotations.Test;
using Annotations.Test.Framework;

namespace Avalon.Test.Annotations.Pri1s
{
	/// <summary>
	/// See Scenario3 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario3Suite : AScenarioSuite
	{
        protected new Scenario3 Scenario
        {
            get
            {
                return (Scenario3)base.Scenario;
            }
        }
		protected override AScenario SelectScenario(string testname)
		{
			if (testname.Contains("3a_")) 
				return new Scenario3a(this);
			if (testname.Contains("3b_"))
				return new Scenario3b(this);
			throw new ArgumentException("Unknown Scenario type: '" + testname + "'.");
		}

		#region Test Cases

		/// <summary>
		/// Annotation on start of non-edge page.
		/// </summary>
        [TestCase_Helper()]
        private void scenario3_3()
		{
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 2;
            Scenario.Selections = new ISelectionData[] { new SimpleSelectionData(2, PagePosition.Beginning, 50) };
            RunScenario(Script, Scenario);
		}
        private void scenario3a_3() { scenario3_3(); }
        private void scenario3b_3() { scenario3_3(); }

		/// <summary>
		/// Annotation on end of non-edge page.
		/// </summary>
        [TestCase_Helper()]
        private void scenario3_4()
		{
            Scenario.FirstVisiblePage = 1;
            Scenario.LastVisiblePage = 1;
            Scenario.Selections = new ISelectionData[] { new SimpleSelectionData(1, PagePosition.End, -50) };
            RunScenario(Script, Scenario);
		}
        private void scenario3a_4() { scenario3_4(); }
        private void scenario3b_4() { scenario3_4(); }

		#endregion Test Cases
	}
}	

