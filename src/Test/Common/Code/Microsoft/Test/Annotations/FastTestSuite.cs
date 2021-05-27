// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------
//
//  Description: 
//  Creator: Derek Mehlhorn (derekme)
//  Date Created: 8/10/2004
//---------------------------------------------------------------------

using System;
using System.Threading;
using System.Windows;
using System.Collections;
using System.Reflection;

namespace Annotations.Test.Framework
{
	/// <summary>
	/// Each method that begins with the word "Test" will be automatically run via reflection
	/// when this TestSuite is executed.
	/// 
	/// Named "Fast" because all tests are excuted in the same application, thereby mitgating
	/// the startup and shutdown.  Also you don't need to keep a manifest of each test case
	/// to be run, just name your test method with "Test" at the beginning and it will automatically
	/// be run.
	/// </summary>
	public abstract class FastTestSuite : TestSuite
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public override void ProcessArgs(string[] args)
        {
            base.ProcessArgs(args);
            verbose = (args.Length > 0 && args[0].Equals("-verbose"));
        }

		/// <summary>
		/// Reflectively invoke all methods that begin with the word "Test" and report pass or failed state
		/// for each of them.
		/// </summary>
		protected override void RunTestCase()
		{
			int totalRun = 0;
			int passCount = 0;
			int failCount = 0;

			Type thisType = Type.GetType(this.ToString());
			MethodInfo[] methods = thisType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			for (int i = 0; i < methods.Length; i++)
			{
				if (methods[i].Name.ToLower().StartsWith("test"))
				{
					ParameterInfo[] parameters = methods[i].GetParameters();
					if (parameters.Length == 0)
					{
						totalRun++;

						printStatus("-------------------------------------------------");
						printStatus("Running '" + methods[i].ToString() + "':");

						try
						{
							methods[i].Invoke(this, null);
						}
						catch (TargetInvocationException e)
						{
							if (e.InnerException.ToString().Contains("TestPassedException"))
							{
								printStatus("passed - " + e.InnerException.Message);
								passCount++;
							}
							else
							{
								if (verbose)
									printStatus("FAILED - " + e.InnerException.ToString());
								else
									printStatus("FAILED - " + e.InnerException.Message);
								failCount++;
							}
						}

						printStatus("-------------------------------------------------\n");
					}
				}
			}

			printStatus("====================================================");
			printStatus("Ran " + totalRun + " test cases.");
			printStatus(passCount + " Passed, " + failCount + " Failed.");
			printStatus("====================================================");
			if (failCount == 0)
				passTest("All test cases passed.");
			else
				failTest(failCount + " test cases failed!");
		}

        bool verbose = false;
	}
}
