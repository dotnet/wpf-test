// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//
//  Description: Test base class that provides implementors with apis for
//		automating visual tests (e.g. automated input and screenshot testing).
//	
//  Creator: Derek Mehlhorn (derekme)
//  Date Created: 10/15/04
//---------------------------------------------------------------------

using System;
using System.Windows;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Annotations.Test.Framework
{
	/// <summary>
	/// A testing framework that provides APIs for automating visual testst including
	/// verifying visual appearance and automating mouse input.
	/// </summary>
	abstract public class VisualAutomationTestSuite: TestSuite
	{

		#region Constructors

        /// <summary/>
		public VisualAutomationTestSuite()
		{
            _uiAutomationModule = new UIAutomationModule();
		}	

		#endregion Constructors
		
		#region Public Properties

        /// <summary>
        /// Module encapsulating vscan operations.
        /// </summary>
		public VScanModule VScan
		{
			get
			{
				return _vscan;
			}
		}

		#endregion

		#region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
		public override void ProcessArgs(string[] args)
		{
			base.ProcessArgs(args); // Inherit base class functionality.
			
			_vscan = new VScanModule(this);
			_vscan.ComparisionMode = DetermineRenderMode(args);
			printStatus("Render Mode = '" + _vscan.ComparisionMode + "'.");
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected override IList<string> UsageParameters()
		{
			IList<string> parameters = base.UsageParameters();
			parameters.Add("/" + parameter_rendermode + " - if 1 then render in SW, otherwise default to hardware acceleration.");
			return parameters;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		protected override IList<string> UsageExamples()
		{
			IList<string> examples = base.UsageExamples();
			examples.Add("'XXX.exe /" + parameter_rendermode + "=1' - render in w/o hardware acceleration mode.");
			examples.Add("'XXX.exe' - render in using default hardware acceleration.");
			return examples;
		}

		/// <summary>
		/// Parses a set of command line arguments to determine what RenderMode it is running under.
		/// </summary>
		/// <remarks>
		/// Looks for argument of form '/rendermode=XXX' where XXX==1 if render mode is Software and
		/// all other values indicate hardward.
		/// </remarks>
		/// <param name="args">Command line args passed to main.</param>
		protected virtual RenderMode DetermineRenderMode(string[] args)
		{
			RenderMode mode = RenderMode.Hardware;
			foreach (string arg in args)
			{
				Match match = new Regex("/" + parameter_rendermode + "=(.*)").Match(arg);
				if (match.Success)
				{
					switch (match.Groups[1].Value)
					{
						case "1":
						case "software":
							mode = RenderMode.Software;
							break;
						default:
							mode = RenderMode.Hardware;
							break;
					}
					break;
				}
			}
			return mode;
		}

		#endregion

		#region Private Variables

		private UIAutomationModule _uiAutomationModule;
		private VScanModule _vscan;

		private string parameter_rendermode = "rendermode";

		#endregion
	}
}
