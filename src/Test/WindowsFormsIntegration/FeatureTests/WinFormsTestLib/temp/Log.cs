// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Xml;
using WFCTestLib.Util;
using System.Collections;
using System.Security.Permissions;
using Microsoft.Test.Logging; 

namespace WFCTestLib.Log
{
    // <doc>
    // <desc>
    //  Writes XML logs and uses the ScenarioResults object.  Uses
    //  XmlLogWriter to write general XML statements.
    // </desc>
    // </doc>
    public class Log
    {
        // The tags we know how to write
        // <doc>
        // <desc>
        //  The <Scenario> tag
        // </desc>
        // </doc>
        private static readonly string Scenario = "Scenario";

        // <doc>
        // <desc>
        //  The <Testcase> tag
        // </desc>
        // </doc>
        private static readonly string Testcase = "Testcase";

        // <doc>
        // <desc>
        //  The <Result> tag
        // </desc>
        // </doc>
        private static readonly string Result = "Result";

        // <doc>
        // <desc>
        //  The <FinalResults> tag
        // </desc>
        // </doc>
        private static readonly string FinalResults = "FinalResults";

        // <doc>
        // <desc>
        //  The <ResultComments> tag
        // </desc>
        // </doc>
        private static readonly string ResultComments = "ResultComments";

		// <doc>
		// <desc>
		//  The <Expected> tag
		// </desc>
		// </doc>
		private static readonly string Expected = "Expected";

		// <doc>
		// <desc>
		//  The <Actual> tag
		// </desc>
		// </doc>
		private static readonly string Actual = "Actual";

		// <doc>
		// <desc>
		//  The <ExpectedActual> tag
		// </desc>
		// </doc>
		private static readonly string ExpectedActual = "ExpectedActual";

		// <doc>
		// <desc>
		//  The <Exception> tag
		// </desc>
		// </doc>
		private static readonly string Exception = "Exception";


		// <doc>
		// <desc>
		//  The <InnerException> tag
		// </desc>
		// </doc>
		private static readonly string InnerException = "InnerException";

		// <doc>
		// <desc>
		//  The <Type> tag
		// </desc>
		// </doc>
		private static readonly string Type = "type";

		// <doc>
		// <desc>
		//  The <ActualType> tag
		// </desc>
		// </doc>
		private static readonly string Message = "Message";

		// <doc>
		// <desc>
		//  The <ActualType> tag
		// </desc>
		// </doc>
		private static readonly string StackTrace = "StackTrace";

		// <doc>
		// <desc>
		//  The <ActualType> tag
		// </desc>
		// </doc>
		private static readonly string Frame = "Frame";

        // <doc>
        // <desc>
        //  The XML stream we want to log to
        // </desc>
        // </doc>
        private XmlLogWriter writer;

        // <doc>
        // <desc>
        //  The final results for this testcase
        // </desc>
        // </doc>
        private ScenarioResult tcr = new ScenarioResult();

        // <doc>
        // <desc>
        //  Global variable holding the name of the current scenario being
        //  logged.
        // </desc>
        // </doc>
        private string currentScenario;

        //
        // Filename to write the log to.
        //
        private string fileName;

        // <doc>
        // <desc>
        //   Constructs a new Log object to the file name specified.
        // </desc>
        // <param term="fileName">
        //   The name of the file to log into
        // </param>
        // <seealso class="XMLWriter">
        // </doc>
        public Log(string fileName) {
            this.fileName = fileName;
            Console.WriteLine("*********** Logging to " + fileName);
            writer = new XmlLogWriter(fileName);
        }

        //
        // Filename to write the log to.
        //
        public string Filename {
            get { return fileName; }
        }

        // <doc>
        // <desc>
        //  The uber ScenarioResult for this testcase
        // </desc>
        // </doc>
        public virtual ScenarioResult TestResults {
            get { return tcr; }
        }
		public void WriteRaw(string s)
		{ writer.WriteRaw(s); }

        public void WritePreFormatted(string s)
        {
            const string CDATA_FORMAT = @"<pre><![CDATA[
{0}
]]></pre>
";
            WriteRaw(string.Format(CDATA_FORMAT, s));
        }
		//
        // Writes the specified tag to the log file.
        //
        // tagName      -- Name of the tag
        // closeToo     -- If true, closes the tag after writing the data
        // elementText  -- Text to write between the open and close tags
        // data         -- Attributes for this tag (e.g. "name" in <foo name="bar">)
        //
        public void WriteTag(string tagName) {
            WriteTag(tagName, false, null, null);
        }

        public void WriteTag(string tagName, bool closeToo) {
            WriteTag(tagName, closeToo, null, null);
        }

        public void WriteTag(string tagName, bool closeToo, LogAttribute data) {
            if ( data == null )
                WriteTag(tagName, closeToo, null, null);
            else
                WriteTag(tagName, closeToo, null, new LogAttribute[] { data });
        }

        public void WriteTag(string tagName, bool closeToo, /*params*/ LogAttribute[] data) {
            WriteTag(tagName, closeToo, null, data);
        }

        public void WriteTag(string tagName, bool closeToo, string elementText, /*params*/ LogAttribute[] data) {
            writer.WriteTag(tagName, closeToo, elementText, data);
        }

        // <doc>
        // <desc>
        //  Closes the specified tag.
        // </desc>
        // <param term="tagName">
        //  The name of the tag to close; e.g. </Scenario>
        //
        //  NOTE: This parameter is no longer used.  Kept here for backward compatibility.
        // </param>
        // </doc>
        public void CloseTag(string tagName) {      // tagName no longer needed
            CloseTag();
        }

        public void CloseTag() {
            writer.CloseTag();
        }

        // <doc>
        // <desc>
        //   Logs the beginning of a testcase by writing the
        //   <Testcase name="testName"> tag
        // </desc>
        // <param term="testName">
        //   The name of the testcase being logged.
        // </param>
        // <seealso member="EndTest">
        // </doc>
        public virtual void StartTest(string testName) {
            WriteTag(Testcase, false, new LogAttribute("name", testName));
        }

        // <doc>
        // <desc>
        //   Logs the beginning of a new scenario by writing the tag
        //   <Scenario name="scenarioName">comments
        // </desc>
        // <param term="scenarioName">
        //   The name of the scenario about to be executed.
        // </param>
        // <param term="comments">
        //   The comments to be written
        // </param>
        // <seealso member="EndScenario">
        // </doc>
        public virtual void StartScenario(string method, string scenarioName) {
            WriteTag(Scenario, false, null, new LogAttribute[] { new LogAttribute("method", method), new LogAttribute("name", scenarioName) });
            currentScenario = scenarioName;
        }

        // <doc>
        // <desc>
        //   Logs the beginning of a new scenario by writing the tag
        //   Scenario name="scenarioName">comments
        // </desc>
        // <param term="scenarioName">
        //   The name of the scenario about to be executed.
        // </param>
        // <seealso member="EndScenario">
        // </doc>
        public virtual void StartScenario(string method) {
            StartScenario(method, method);
        }

        // <doc>
        // <desc>
        //  Ends the current scenario by writing the <Result.../> and
        //  </Scenario> tags.
        // </desc>
        // <param term="result">
        //  The results of the scenario. True if the scenario passed; false
        //  otherwise. If false, a generic string for the <ResultComments>
        //  tag is created.
        // </param>
        // </doc>
        public virtual void EndScenario(bool result) {
            string failString = "scenario \"" + currentScenario + "\" failed.";
            EndScenario(result, result ? null : failString);
        }

        // <doc>
        // <desc>
        //  Ends the current scenario by writing the <Result.../> and
        //  </Scenario> tags.
        // </desc>
        // <param term="result">
        //  The results of the scenario. True if the scenario passed; false
        //  otherwise.
        // </param>
        // <param term="comments">
        //  If the scenario failed, this string is used in the
        //  <ResultComments> tag.
        // </param>
        // </doc>
        public virtual void EndScenario(bool result, string comments) {
            EndScenario(new ScenarioResult(result, result ? null : comments));
        }

        // <doc>
        // <desc>
        //  Ends the current scenario by writing the <Result.../> and
        //  </Scenario> tags.
        // </desc>
        // <param term="result">
        //  The results of the scenario.
        // </param>
        // </doc>
        public virtual void EndScenario(ScenarioResult result) {
            ResultsBlock(Result, result);

            TestLog testLog = new TestLog(currentScenario);

            if (result == ScenarioResult.Pass)
            {
                tcr.PassCount++;
	    	testLog.Result = TestResult.Pass;
            }
            else
            {
                tcr.FailCount++;
                if (result.Comments != null)
                    tcr.Comments = result.Comments;

		testLog.LogEvidence(result.Comments);
                testLog.Result = TestResult.Fail;
            }
            testLog.Close();
            CloseTag();
        }

        // <doc>
        // <desc>
        //  Ends the testcase by writing the </Testcase> tag and closing
        //  the log file.
        // </desc>
        // </doc>
        public virtual void EndTest() {
            ResultsBlock(FinalResults, tcr);
            CloseTag();
            writer.Close();
        }

        // <doc>
        // <desc>
        //  Writes a results block.
        // </desc>
        // <param term="tagName">
        //  The name of the tag to use for this results block.
        // </param>
        // <param term="sr">
        //  The ScenarioResult from which the "type", "total" and "fail"
        //  properties are retrieved.
        // </param>
        // </doc>
        private void ResultsBlock(string tagName, ScenarioResult sr) {
            if ( sr.Comments == null )
                WriteTag(tagName, true, null, sr.GetResultLogAttributes());
            else {
                WriteTag(tagName, false, null, sr.GetResultLogAttributes());
                    WriteTag(ResultComments, true, sr.Comments, null);
                CloseTag();
            }
        }

        //
        // Writes information about a known bug to the log file.  You specify the
        // database and bug ID, and an optional comment for the bug (e.g. "this bug
        // causes an ArgumentException")
        //
        // This information is used for auto-analysis purposes.  To avoid erroneous
        // auto-analysis, you should only write this information when you're sure a
        // failure has occurred because of the known bug (e.g. catch the exception the
        // bug causes and only then, print the bug information).
        //
        // Alternatively, if the failure can't be handled, you can log the known bug,
        // then return ScenarioResult.Fail before the failing code is executed.
        //
        // Additionally, specifying a comment can help distinguish between multiple
        // bugs, or help determine when a failure was caused by a known bug as opposed
        // to a new one.
        //
        public virtual void LogKnownBug(BugDb database, int id) {
            LogKnownBug(database, id, null);
        }

        public virtual void LogKnownBug(BugDb database, int id, string comment) {
			string url = GetBugCheckUrl(database, id);

			if (url != null) {
				WriteTag("KnownBug", true, comment, new LogAttribute[] {
					new LogAttribute("db", database.ToString()),
					new LogAttribute("id", id.ToString()),
					new LogAttribute("url", url)
				});
			}
			else {
				WriteTag("KnownBug", true, comment, new LogAttribute[] {
					new LogAttribute("db", database.ToString()),
					new LogAttribute("id", id.ToString())
				});
			}
        }

		// There are bug DBs that aren't covered by BugCheck, so if that's ever the case
		// here, we return null.
		private string GetBugCheckUrl(BugDb db, int id) {
			string dbName;

			switch (db) {
				case BugDb.ASURT :		dbName = "URT"; break;
				case BugDb.VS7 :		dbName = "VisualStudio7"; break;
				case BugDb.VSWhidbey :	dbName = "VSWhidbey"; break;
                case BugDb.WindowsOSBugs: dbName = "Windows OS Bugs"; break;
				default :				return null;
			}

			return "http://bugcheck/default.asp?URL=/bugs/" + dbName + "/" + id + ".asp";
		}

		/// <summary>
		/// Writes expected and actual values along with their types
		/// </summary>
		/// <param name="expected">expected value</param>
		/// <param name="actual">actual value</param>
		public virtual void LogExpectedActual(object expected, object actual)
		{
			writer.WriteTag(ExpectedActual, false);
			if (expected == null)
				writer.WriteTag(Expected, true, null, new LogAttribute[] { new LogAttribute(Type, null) });
			else
				writer.WriteTag(Expected, true, expected.ToString(), new LogAttribute[] { new LogAttribute(Type, expected.GetType().ToString()) });

			if (actual == null)
				writer.WriteTag(Actual, true, null, new LogAttribute[] { new LogAttribute(Type, null) });
			else
				writer.WriteTag(Actual, true, actual.ToString(), new LogAttribute[] { new LogAttribute(Type, actual.GetType().ToString()) });
			writer.CloseTag();
		}

		public virtual void LogException(Exception ex)
		{
			if (ex == null)
				return;

			ArrayList list = new ArrayList();
			list.Add(ex);
			while (ex.InnerException != null)
			{
				list.Add(ex.InnerException);
				ex = ex.InnerException;
			}

			for (int i = list.Count-1; i >=0; i--)
			{
				if (i > 0)
					LogException((System.Exception)list[i], true);
				else
					LogException((System.Exception)list[i], false);
			}
		}

		[SecurityPermission(SecurityAction.Assert, Unrestricted =true)]
		[FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
		private void LogException(Exception ex, bool isInnerException)
		{
			if (ex == null)
				return;

			if(isInnerException)
				writer.WriteTag(InnerException, false, new LogAttribute(Type, ex.GetType().ToString()));
			else
				writer.WriteTag(Exception, false, new LogAttribute(Type, ex.GetType().ToString()));
			writer.WriteTag(Message, true, ex.Message, null);

			writer.WriteTag(StackTrace, false);
			if (ex.StackTrace != null)
			{
				System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(ex, true);
				foreach (System.Diagnostics.StackFrame stackFrame in stackTrace.GetFrames())
				{
					writer.WriteTag(Frame, true, new LogAttribute[]{
						new LogAttribute("function", stackFrame.GetMethod().Name),
						new LogAttribute("file", stackFrame.GetFileName()),
						new LogAttribute("line", stackFrame.GetFileLineNumber().ToString())
						});
				}
			}
			// StackTrace tag
			writer.CloseTag();

			if(!isInnerException)
				writer.WriteTag("Text", true, ex.ToString(), null);
			// Exception tag
			writer.CloseTag();
		}

        // <doc>
        // <desc>
        //  Writes a string of data to the log file without appending a CRLF pair.
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // </doc>
        public virtual void Write(string data) {
            writer.Write(data);
	    if(TestLog.Current != null) TestLog.Current.LogEvidence(data);
	    else { GlobalLog.LogEvidence(data); }
        }

        // <doc>
        // <desc>
        //  Appends a CRLF pair to the log file.
        // </desc>
        // </doc>
        public virtual void WriteLine() {
            writer.WriteLine();
        }

        // <doc>
        // <desc>
        //  Writes a string of data followed by a CRLF pair to the log file..
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // </doc>
        public virtual void WriteLine(string data) {
            writer.WriteLine(data);
	    if(TestLog.Current != null) TestLog.Current.LogEvidence(data);
	    else { GlobalLog.LogEvidence(data); }
        }

        // <doc>
        // <desc>
        //  Writes a string of data followed by a CRLF pair to the log file.  Allows
        //  string formatting.
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // <param term="arg0">
        //  Object to format.
        // </param>
        // </doc>
        public virtual void WriteLine(string data, object arg0) {
            WriteLine(String.Format(data, arg0));
        }

        // <doc>
        // <desc>
        //  Writes a string of data followed by a CRLF pair to the log file.  Allows
        //  string formatting.
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // <param term="arg0">
        //  First object to format.
        // </param>
        // <param term="arg1">
        //  Second object to format.
        // </param>
        // </doc>
        public virtual void WriteLine(string data, object arg0, object arg1) {
            WriteLine(String.Format(data, arg0, arg1));
        }

        // <doc>
        // <desc>
        //  Writes a string of data followed by a CRLF pair to the log file.  Allows
        //  string formatting.
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // <param term="arg0">
        //  First object to format.
        // </param>
        // <param term="arg1">
        //  Second object to format.
        // </param>
        // <param term="arg2">
        //  Third object to format.
        // </param>
        // </doc>
        public virtual void WriteLine(string data, object arg0, object arg1, object arg2) {
            WriteLine(String.Format(data, arg0, arg1, arg2));
        }

        // <doc>
        // <desc>
        //  Writes a string of data followed by a CRLF pair to the log file.  Allows
        //  string formatting.
        // </desc>
        // <param term="data">
        //  The data to write to the log file.
        // </param>
        // <param term="args">
        //  Array of objects to format.
        // </param>
        // </doc>
        public virtual void WriteLine(string data, params object[] args) {
            WriteLine(String.Format(data, args));
        }
    }

    //
    // Simple data structure to represent a name-value XML element attribute
    //
    public class LogAttribute {
        public string Name;
        public string Value;

        public LogAttribute(string name, string value) {
            Name = name;
            Value = value;
        }
    }

    //
    // Enum of Product Studio databases for use with the LogKnownBug() method
    //
    public enum BugDb { ASURT, VS7, VSWhidbey, WindowsOSBugs };
}
