#if false
using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using MDLog;
using MDLog.MDLogItems;

namespace WFCTestLib.XmlLogTree.Util {
	/// <summary>
	/// Provides static methods to convert ReflectBase-style XML logs into the
	/// MDLog standard log format.
	/// </summary>
	public class MDLogConverter {
		private MDLogConverter() { }

		public static void ConvertLog(XmlLogTree.TestResult root, string filename) {
/* Currently not supported
			using ( StreamWriter writer = new StreamWriter(filename) ) {
				ConvertLog(root, writer);
				writer.Flush();
			}
*/
			MDLog.Log log = MDLog.Log.OpenLog(filename, null);
			ConvertLogToMDLog(root, log);
		}

        public static void ConvertLog(string xmlLogSourceFilename, string mdLogDestFilename) {
            ConvertLog(LogParser.ParseLogFile(xmlLogSourceFilename), mdLogDestFilename);
        }

        public static void ConvertLog(Stream xmlLogSource, string mdLogDestFilename) {
            ConvertLog(LogParser.ParseLogFile(xmlLogSource), mdLogDestFilename);
        }

/* MDLog doesn't support outputting to stream.
		public void ConvertLog(XmlLogTree.TestResult root, TextWriter writer) {
			Log log = ConvertLogToMDLog(root);
			log.Dispose();
		}
*/

		// Should return Log but MDLog currently requires us to pass a filename to the log
		// constructor, so the log has to be created first.
		public static void ConvertLogToMDLog(XmlLogTree.TestResult root, MDLog.Log mdLog) {
			MDLog.Testcase testcase = mdLog.AddTestcase(root.Name);

			testcase.ScenariosPassed = root.TotalCount - root.FailCount;
			testcase.ScenariosFailed = root.FailCount;

			foreach ( XmlLogTree.ScenarioGroup group in root.ScenarioGroups ) {
				foreach ( XmlLogTree.Scenario scenario in group.Scenarios ) {
					MDLog.Scenario mdScenario = testcase.AddScenario(scenario.Name);
					mdScenario.Passed = scenario.Passed;

/*
					if (scenario.TotalCount > 1)
					{
						mdScenario.AddLogItem("TotalCount=" + scenario.TotalCount, InfoType.
					}
*/

					foreach ( BugInfo bug in scenario.KnownBugs )
						mdScenario.AddLogItem(bug.Db + " #" + bug.Id, InfoType.BUG);

					if ( scenario.Text != null ) {
                        mdScenario.AddLogItem(scenario.Text, InfoType.INFO);
/*						StringReader text = new StringReader(scenario.Text);
						string line;

						while ( (line = text.ReadLine()) != null ) {
							if ( line != "" )
								mdScenario.AddLogItem(line.Trim(), InfoType.INFO);
						}
*/
					}

					if ( scenario.Comment != null )
						mdScenario.AddLogItem(scenario.Comment, InfoType.COMMENT);
				}
			}

            new FileIOPermission(PermissionState.Unrestricted).Assert();
			mdLog.Dispose();
            CodeAccessPermission.RevertAssert();
		}
	}
}
#endif
