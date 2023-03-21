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
using Annotations.Test.Reflection;
using System.Collections.Generic;
using System.Windows.Media;					// TestSuite.

namespace Avalon.Test.Annotations
{
    [TestDimension("fixed,fixed /fds=false,flow")]
    [TestDimension("stickynote,highlight")]
    public abstract class AScenarioSuite : AVisualSuite
	{
		/// <summary>
		/// Initialize and return scenario to run.
		/// </summary>
		abstract protected AScenario SelectScenario(string testname);

		#region Private Methods

		/// <summary>
		/// Set the zoom and layout so that exactly 1 page is visible.*
		/// 
		/// *Even though viewport only shows 1 page, upon scrolling 2 pages will always be "visible".
		/// </summary>
		/// <returns>Script containing setup steps for all tests.</returns>
		private AsyncTestScript CreateSetupScript()
		{
			SetZoom(100);
			WholePageLayout();			
			AsyncTestScript setupScript = new AsyncTestScript();	
			return setupScript;
		}

		#endregion Private Methods

		#region Protected Methods

		public override void ProcessArgs(string[] args)
		{
			base.ProcessArgs(args);

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "vscan")
					_doVscan = true;
				else if (args[i] == "/slow")
					_slow = true;
			}
		}

        /// <summary>
        /// Create a setup script that is common to all Scenario tests, select a Scenario, and call subclass
        /// implementation to select and run specific test case.
        /// </summary>
        [TestCase_Setup()]
        protected override void DoSetup()
        {
            base.DoSetup();

            Script = CreateSetupScript();
            _scenario = SelectScenario(CaseNumber);
        }

        [TestCase_Cleanup()]
        protected override void CleanupVariation()
        {
            base.CleanupVariation();

            Script = null;
            _scenario = null;
        }

		protected void MakeWholeDocumentVisible()
        {
            // "Magic" values that, for flow, will keep the same pagination while displaying the entire document.			
			PageLayout(6);
			SetZoom(40);
        }

		protected void RunScenario(AsyncTestScript script, AScenario scenario)
		{
			scenario.AppendScript(ref script);
			AsyncTestScriptRunner runner = new AsyncTestScriptRunner(this);
            if (_slow)
                runner.ActionDelay = new TimeSpan(0,0,2);
            runner.Run(script, true);
		}

		protected void VerifyAnnotations(SelectionType[] selections)
		{
			string[] anchors = new string[selections.Length];
			for (int i = 0; i < selections.Length; i++)
				anchors[i] = ExpectedSelectedText(selections[i]);
			VerifyAnnotations(anchors);
		}

		#endregion Protected Methods

        #region Public Methods

        public bool VerifyVisuals
        {
            get { return _doVscan; }
        }

        #endregion Public Methods

        #region Protected Fields

        protected AsyncTestScript Script;
        protected virtual AScenario Scenario
        {
            get
            {
                return _scenario;
            }
        }

        #endregion

        #region Private Fields

        private bool _doVscan = false;
        private bool _slow = false;
        private AScenario _scenario;

		#endregion Private Fields
	}
}	

