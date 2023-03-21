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
	/// See Scenario2 in ScenarioDefinitions.cs.
	/// </summary>
	public class Scenario2Suite : AScenarioSuite
	{
        protected new Scenario2 Scenario
        {
            get
            {
                return (Scenario2) base.Scenario;
            }
        }

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

		#region Tests


        /// <summary>
        /// Create annotation at start of document.
        /// </summary>
        private void scenario2b_1()
        {
            Script.Add("CreateAnnotation", SelectionData(SelectionType.StartOfDocument));

            Scenario.VisiblePage = 0;
            Scenario.InitialAttachedAnchor = ExpectedSelectedText(SelectionType.StartOfDocument);
            Scenario.ExpectedNumAttachedAnnotations = 0;

            RunScenario(Script, Scenario);
        }

        /// <summary>
        /// Create annotation at start of non-edge page.
        /// </summary>
        [TestCase_Helper()]
        private void scenario2_3()
        {
            Script.Add("GoToPage", new object[] { 2 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_Start));

            Scenario.VisiblePage = 2;
            Scenario.InitialAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_Start);
            Scenario.ExpectedNumAttachedAnnotations = 0;

            RunScenario(Script, Scenario);
        }
        private void scenario2a_3() { scenario2_3(); }
        private void scenario2b_3() { scenario2_3(); }


        /// <summary>
        /// Create annotation at end of non-edge page.
        /// </summary>
        [TestCase_Helper()]
        private void scenario2_4()
        {
            Script.Add("GoToPage", new object[] { 2 });
            Script.Add("CreateAnnotation", SelectionData(SelectionType.Page2_End));

            Scenario.VisiblePage = 2;
            Scenario.InitialAttachedAnchor = ExpectedSelectedText(SelectionType.Page2_End);
            Scenario.ExpectedNumAttachedAnnotations = 0;

            RunScenario(Script, Scenario);
        }
        private void scenario2a_4() { scenario2_4(); }
        private void scenario2b_4() { scenario2_4(); }


		#endregion Tests
	}
}	

