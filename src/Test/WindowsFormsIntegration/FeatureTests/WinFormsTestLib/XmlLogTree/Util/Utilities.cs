// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace WFCTestLib.XmlLogTree.Util {
	/// <summary>
	/// Static utility methods for working with XML logs.
	/// </summary>
	public class XmlLogUtilities {
		// Can't be instantiated.
		private XmlLogUtilities() { }

		/// <summary>
		/// Determines if the only failures in this test result were caused by known bugs.
		/// </summary>
		/// <param name="result"></param>
		/// <returns>
		/// If the only failures in this test result were caused by known bugs, returns a
		/// collection of them.  If one or more failures was not caused by a known bug, or
		/// the testcase passed, returns null.
		/// </returns>
		public static BugInfoCollection AnalyzeBugs(TestResult result) {
			if ( result.Passed )
				return null;
			
			BugInfoCollection retVal = new BugInfoCollection();

			foreach ( ScenarioGroup group in result.ScenarioGroups ) {
				// No use looping through a ScenarioGroup that passed.
				if ( group.Passed )
					continue;

				foreach ( Scenario scenario in group.Scenarios ) {
					if ( !scenario.Passed ) {
						if ( scenario.KnownBugs.Count == 0 )
							return null;	// Found failure that wasn't caused by a known bug

						retVal.AddRange(scenario.KnownBugs);
					}
				}
			}

			return retVal;
		}

#if TEST_UTILITIES
		//
		// Test method
		//
		private static void Main(string[] args) {
			string filename;

			if ( args.Length == 0 )
				filename = "results.log";
			else
				filename = args[0];

			TestResult result = LogParser.ParseLogFile(filename);
			BugInfoCollection knownBugs = Utilities.AnalyzeBugs(result);

			if ( knownBugs == null )
				Console.WriteLine("Testcase \"{0}\" had failures that were not caused by known bugs.", result.Name);
			else {
				Console.WriteLine("Testcase \"{0}\" failed due to the following known bugs:", result.Name);

				foreach ( BugInfo bug in knownBugs )
                    Console.WriteLine("    {0} #{1}: {2}", bug.Db, bug.Id, bug.Comment);
			}
		}
#endif
	}
}
