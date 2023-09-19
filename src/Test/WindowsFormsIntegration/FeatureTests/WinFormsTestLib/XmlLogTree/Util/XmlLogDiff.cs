// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define DEBUG

using System;
using System.Diagnostics;
using System.IO;

namespace WFCTestLib.XmlLogTree.Util {
    public class XmlLogDiff : ILogDiff {
        bool _allowMoreFailuresInBase = false;
        bool _matchSameFailComments = true;
        bool _matchSameFailedScenarioText = false;

        ScenarioDiffEventHandler _onScenarioDiff;

        public XmlLogDiff() { }

        /// <summary>
        /// If true, considers failures in the base log that don't occur in the
        /// comparison log to most likely be fixed bugs, and thus does not fail
        /// the compare. Default is false (in case a base result was analyzed
        /// with multiple bugs, some of which have been fixed).
		/// </summary>
        public bool AllowMoreFailuresInBase {
            get { return _allowMoreFailuresInBase; }
            set { _allowMoreFailuresInBase = value; }
        }

        /// <summary>
        /// If true, requires failed scenarios to have the same comment text.
        /// Default is true.
		/// </summary>
        public bool MatchSameFailComments {
            get { return _matchSameFailComments; }
            set { _matchSameFailComments = value; }
        }

        /// <summary>
		/// If true, requires failed scenarios to have the same scenario text
		/// (i.e. log output).  Default is false.
        /// </summary>
        public bool MatchSameFailedScenarioText {
            get { return _matchSameFailedScenarioText; }
            set { _matchSameFailedScenarioText = value; }
        }

        /// <summary>
		/// Raises an event whenever a scenario fails the diff.
        /// </summary>
        public event ScenarioDiffEventHandler ScenarioDiff {
            add    { _onScenarioDiff += value; }
            remove { _onScenarioDiff -= value; }
        }

        protected virtual void OnScenarioDiff(ScenarioDiffEventArgs e) {
            if ( _onScenarioDiff != null )
                _onScenarioDiff(this, e);
        }

        public bool LogsMatch(TextReader baseLog, TextReader compareLog) {
            return LogsMatch(LogParser.ParseLogFile(baseLog), LogParser.ParseLogFile(compareLog));
        }

        public bool LogsMatch(Stream baseLog, Stream compareLog) {
            return LogsMatch(LogParser.ParseLogFile(baseLog), LogParser.ParseLogFile(compareLog));
        }

		// I gave this a different name in case we want a method that takes 2 string filenames to logfiles.
		public bool LogsMatchFromStrings(string baseLog, string compareLog) {
			return LogsMatch(LogParser.ParseLogFileFromString(baseLog), LogParser.ParseLogFileFromString(compareLog));
		}

        public bool LogsMatch(TestResult baseResult, TestResult compareResult) {
            int numGroups = baseResult.ScenarioGroups.Count;

            if ( numGroups != compareResult.ScenarioGroups.Count ) {
                Debug.WriteLine("ScenarioGroup counts differed");
                return false;
            }

            for ( int i = 0; i < numGroups; ++i ) {
                ScenarioGroup baseGroup = baseResult.ScenarioGroups[i];
                ScenarioGroup compareGroup = compareResult.ScenarioGroups[i];
                int numScenarios = baseGroup.Scenarios.Count;

                if ( numScenarios != compareGroup.Scenarios.Count ) {
                    Debug.WriteLine("Scenario counts differed");
                    return false;
                }

                for ( int j = 0; j < numScenarios; j++ ) {
                    Scenario baseScenario = baseGroup.Scenarios[j];
                    Scenario compareScenario = compareGroup.Scenarios[j];

					if ( !ScenariosMatch(baseScenario, compareScenario) )
						return false;
                }
            }

            return true;
        }

		public bool ScenariosMatch(Scenario baseScenario, Scenario compareScenario) {
			bool failed = false;
			FailureReason reason = FailureReason.None;

			// Pass / Pass
			if (baseScenario.Passed && compareScenario.Passed)
				return true;
			else if (!baseScenario.Passed && !compareScenario.Passed) {
				// Fail / Fail
				if (MatchSameFailComments && (baseScenario.Comment != compareScenario.Comment)) {
					Debug.WriteLine("Both failed, but comments were different");
					failed = true;
					reason = FailureReason.CommentDiffered;
				}

				if (MatchSameFailedScenarioText && (baseScenario.Text != compareScenario.Text)) {
					Debug.WriteLine("Both failed, but Text was different");
					failed = true;
					reason = FailureReason.TextDiffered;
				}
			}
			else if (baseScenario.Passed && !compareScenario.Passed) {
				// Pass / Fail
				Debug.WriteLine("Base passed, compare failed");
				failed = true;
				reason = FailureReason.NoMatch;
			}
			else if (!baseScenario.Passed && compareScenario.Passed) {
				// Fail / Pass
				if (!AllowMoreFailuresInBase) {
					Debug.WriteLine("AllowMoreFailuresInBase == false and base failed, but compare passed");
					failed = true;
					reason = FailureReason.FailedInBase;
				}
			}
			else
				Debug.Assert(false, "We should never reach this else clause.");

			if (failed) {
				ScenarioDiffEventArgs e = new ScenarioDiffEventArgs(baseScenario, compareScenario, reason);
				OnScenarioDiff(e);

				if (!e.OverrideFailure) {
					Debug.WriteLine("Failure was not overridden");
					return false;
				}
			}

			return true;
		}

#if TEST_DIFF
        //
        // Test methods
        //
        private static void Main(string[] args) {
            XmlLogDiff diff = new XmlLogDiff();
            string f1 = "results.Log", f2 = "results2.log";

            Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));

            if ( args.Length == 2 ) {
                f1 = args[0];
                f2 = args[1];
            }

            diff.ScenarioDiff += new ScenarioDiffEventHandler(ScenarioDiffFired);
            Console.WriteLine(diff.LogsMatch(File.OpenRead(f1), File.OpenRead(f2)));
        }

        private static void ScenarioDiffFired(object o, ScenarioDiffEventArgs e) {
            Console.WriteLine("*********************************************************************************");
            Console.WriteLine("Scenario: " + e.BaseScenario.Name);
            Console.WriteLine();
            Console.WriteLine("========BASE========");
            Console.WriteLine(e.BaseScenario.XmlNode.OuterXml);
            Console.WriteLine("======COMPARE=======");
            Console.WriteLine(e.CompareScenario.XmlNode.OuterXml);
            Console.WriteLine();
        }
#endif
    }

	public enum FailureReason {
		/// <summary>
		/// Didn't fail, or no reason given.
		/// </summary>
		None,

		/// <summary>
		/// Scenario failed, but it didn't in the base test.
		/// </summary>
		NoMatch,

		/// <summary>
		/// Scenario passed, but it failed in the base test.  This really only matters if the base
		/// test was analyzed with multiple bugs and some have been fixed.
		/// </summary>
		FailedInBase,

		/// <summary>
		/// Comment text differed from the base test failure.
		/// </summary>
		CommentDiffered,

		/// <summary>
		/// Scenario text differed from the base test failure.
		/// </summary>
		TextDiffered
	}

    public delegate void ScenarioDiffEventHandler(object sender, ScenarioDiffEventArgs e);

    public class ScenarioDiffEventArgs : EventArgs {
        Scenario _baseScenario;
        Scenario _compareScenario;
		FailureReason _reason;
        bool _overrideFailure = false;

		public ScenarioDiffEventArgs(Scenario baseScenario, Scenario compareScenario) : this(baseScenario, compareScenario, FailureReason.None)
        { }

		public ScenarioDiffEventArgs(Scenario baseScenario, Scenario compareScenario, FailureReason reason) {
            this._baseScenario = baseScenario;
            this._compareScenario = compareScenario;
			this._reason = reason;
        }

        public Scenario BaseScenario {
            get { return _baseScenario; }
        }

        public Scenario CompareScenario {
            get { return _compareScenario; }
        }

		public FailureReason FailureReason {
			get { return _reason; }
		}

        public bool OverrideFailure {
            get { return _overrideFailure; }
            set { _overrideFailure = value; }
        }
    }
}
