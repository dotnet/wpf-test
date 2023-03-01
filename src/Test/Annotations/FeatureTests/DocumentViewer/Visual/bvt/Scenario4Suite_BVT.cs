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
	/// See Scenario4 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario4Suite_BVT : AScenarioSuite
	{
		protected override AScenario SelectScenario(string testname)
		{
			return new Scenario4(DocViewerWrapper);
		}

        protected new Scenario4 Scenario
        {
            get
            {
                return (Scenario4) base.Scenario;
            }
        }

		#region Test Cases

		/// <summary>
		/// Create annotation at middle of non-edge page.
		/// </summary>
		private void scenario4_5()
		{
            Scenario.FirstVisiblePage = 2;
            Scenario.LastVisiblePage = 2;
            Scenario.SelectionData = new SimpleSelectionData(2, 1067, 194); //SelectionData(SelectionType.Page2_1067_to_1261);                        
			Scenario.ResizedWindowSize = new Size(180, 350);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);// ExpectedSelectedText(SelectionType.Page2_1067_to_1261);
            RunScenario(Script, Scenario);
		}

		/// <summary>
		/// Anchor spanning 1 page break.
		/// </summary>
		private void scenario4_6()
		{
			if (ContentMode == TestMode.Flow) ViewAsTwoPages();
			else Scenario.ViewPreviousPage = true;

            Scenario.FirstVisiblePage = 3;
            Scenario.LastVisiblePage = 4;
            Scenario.SelectionData = new MultiPageSelectionData(3, PagePosition.End, -33, 4, PagePosition.Beginning, 70);// SelectionData(SelectionType.Page3_To_Page4);
            Scenario.ResizedWindowSize = new Size(201, 303);
            Scenario.ExpectedAttachedAnchor = GetText(Scenario.SelectionData);// ExpectedSelectedText(SelectionType.Page3_To_Page4);
			RunScenario(Script, Scenario);
		}
		
		#endregion Test Cases
	}
}	

