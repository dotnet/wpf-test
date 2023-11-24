using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace WFCTestLib.XmlLogTree {

    /// <summary>
    /// Contains static methods for generating an XML log file parse tree.
    /// </summary>
	
    public class LogParser 
	{
		public static TestResult ParseLogFile(string filename)
		{
			StreamReader sr = new StreamReader(filename);
			string logFile = sr.ReadToEnd();
			sr.Close();
			return ParseLogFileFromString(logFile);
		}

        public static TestResult ParseLogFile(Stream stream) {
			StreamReader sr = new StreamReader(stream);
			string logFile = sr.ReadToEnd();
			sr.Close();
			return ParseLogFileFromString(logFile);
		}

        public static TestResult ParseLogFile(TextReader reader) {
			string logFile = reader.ReadToEnd();
			return ParseLogFileFromString(logFile);
		}

        public static TestResult ParseLogFile(XmlReader reader) {
			XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
			doc.Load(reader);
			return TestResult.BuildTestResult(doc.DocumentElement);
        }

		public static TestResult ParseLogFileFromString(string logFile) {
			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = true;
			try
			{
				doc.LoadXml(logFile);
				return TestResult.BuildTestResult(doc.DocumentElement);
			}
			catch
			{
				if (IsTimedOutInScenario(logFile))
					return GetDummyTimedOutTestResult();
				else
					throw;
			}
		}

		private static bool IsTimedOutInScenario(string logFile)
		{
			if (logFile == null)
				return false;

			if (logFile.Contains("!!! TIMED OUT:") && logFile.Contains("<Scenario"))
				return true;
			else
				return false;
		}

		private static TestResult GetDummyTimedOutTestResult()
		{
			string logFile = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
			logFile += "<Testcase name=\"Dummy\">\n";
			logFile += "<ScenarioGroup name=\"DummyScenarioGroup\">\n";
			logFile += "<Scenario method=\"DummyScenario\" name=\"DummyScenario\">\n";
			logFile += "<Result type=\"Fail\" total=\"1\" fail=\"1\">\n";
			logFile += "<ResultComments>\n";
			logFile += "Timed Out\n";
			logFile += " </ResultComments>\n";
			logFile += "</Result>\n";
			logFile += "</Scenario>\n";
			logFile += "<ClassResults type=\"Fail\" total=\"1\" fail=\"1\" />\n";
			logFile += "</ScenarioGroup>\n";
			logFile += "<FinalResults type=\"Fail\" total=\"1\" fail=\"1\">\n";
			logFile += "<ResultComments>\n";
			logFile += "Timed Out\n";
			logFile += " </ResultComments>\n";
			logFile += "</FinalResults>\n";
			logFile += "</Testcase>\n";

			return LogParser.ParseLogFileFromString(logFile); 
		}

        /// <summary>
        /// Print the contents of the log tree.
        /// </summary>
        /// <param name="result">TestResult of which to print the contents.</param>
        public static void PrintResultTree(TestResult result) {
            Console.WriteLine("Testcase:              " + result.Name);
            Console.WriteLine("    Passed?            " + result.Passed);
            Console.WriteLine("    TotalCount:        " + result.TotalCount);
            Console.WriteLine("    FailCount:         " + result.FailCount);
            Console.WriteLine("    Comment:           " + result.Comment);
            Console.WriteLine("    CommandLineParams: ");

            foreach ( string key in result.CommandLineParameters.Keys )
                Console.WriteLine("        {0} = {1}", key, result.CommandLineParameters[key]);
                Console.WriteLine();
            
            foreach ( ScenarioGroup group in result.ScenarioGroups ) {
                Console.WriteLine("    ScenarioGroup:         " + group.Name);
                Console.WriteLine("        Passed?            " + group.Passed);
                Console.WriteLine("        TotalCount:        " + group.TotalCount);
                Console.WriteLine("        FailCount:         " + group.FailCount);
                Console.WriteLine("        Missing Scenarios: ");

                foreach ( string s in group.MissingScenarios )
                    Console.WriteLine("            " + s);

                Console.WriteLine("        Extra Scenarios:   ");

                foreach ( string s in group.ExtraScenarios )
                    Console.WriteLine("            " + s);

                Console.WriteLine();

                foreach ( Scenario scenario in group.Scenarios ) {
                    Console.WriteLine("        Scenario:       " + scenario.Name);
                    Console.WriteLine("            Passed?     " + scenario.Passed);
                    Console.WriteLine("            Comment:    " + (scenario.Comment == null ? "<none>" : scenario.Comment));
                    Console.WriteLine("            TotalCount: " + scenario.TotalCount);
                    Console.WriteLine("            FailCount:  " + scenario.FailCount);
                    Console.WriteLine();

                    foreach ( BugInfo bug in scenario.KnownBugs ) {
                        Console.WriteLine("            Bug:");
                        Console.WriteLine("                DB:      " + bug.Db);
                        Console.WriteLine("                ID:      " + bug.Id);
                        Console.WriteLine("                Comment: " + bug.Comment);
                        Console.WriteLine();
                    }
                }
            }
        }

    }

    /// <summary>
    /// Abstract parent class for all nodes in the XML log parse tree.
    /// </summary>
    public abstract class LogNode {
        private string name;
        private XmlNode xml;

        public LogNode(XmlNode xml) {
            this.xml = xml;

            XmlAttribute nameAttr = xml.Attributes["name"];

            if ( nameAttr == null )
                throw new Exception("No name on xml node");

            name = nameAttr.Value;
        }

        public string Name {
            get { return name; }
        }

        public virtual XmlNode XmlNode {
            get { return xml; }
        }

        //
        // Utility methods
        //
        internal static int FindElement(XmlNode parent, string localName) {
            return FindElement(parent, localName, 0);
        }

        internal static int FindElement(XmlNode parent, string localName, int startIndex) {
            int numChildren = parent.ChildNodes.Count;

            for ( int i = startIndex; i < numChildren; i++ ) {
                XmlNode node = parent.ChildNodes[i];
                if ( node.NodeType == XmlNodeType.Element && node.LocalName == localName )
                    return i;
            }

            return -1;
        }

        internal static ArrayList FindAllElements(XmlNode parent, string localName) {
            ArrayList elements = new ArrayList();
            int index = FindElement(parent, localName);

            while ( index != -1 ) {
                elements.Add(parent.ChildNodes[index]);
                index = FindElement(parent, localName, index + 1);
            }

            return elements;
        }
    }

    /// <summary>
    /// Top-level node representing the entire result log.
    /// </summary>
    public class TestResult : LogNode {
        private ScenarioGroupCollection groups;
        private bool passed;
        private int totalCount;
        private int failCount;
        private string comment;
        private NameValueCollection commandLineParams;
        private string initTestText;

        internal TestResult(XmlNode xml) : base(xml) {
            groups = new ScenarioGroupCollection(this);

            // Init FinalResults values
            XmlNode finalResults = xml.ChildNodes[FindElement(xml, "FinalResults")];

			if (finalResults != null)
			{
				if (finalResults.Attributes["type"] != null)
					passed = finalResults.Attributes["type"].Value == "Pass";

				if (finalResults.Attributes["total"] != null)
					totalCount = Int32.Parse(finalResults.Attributes["total"].Value);
				if (finalResults.Attributes["fail"] != null)
					failCount = Int32.Parse(finalResults.Attributes["fail"].Value);
			}

            if ( finalResults.ChildNodes.Count > 0 ) {
                XmlNode commentElement = finalResults.ChildNodes[FindElement(finalResults, "ResultComments")];
                comment = commentElement.ChildNodes[0].Value.Trim();
            }

            // Init remaining values
            ArrayList elements = FindAllElements(xml, "CommandLineParameter");
            commandLineParams = new NameValueCollection(elements.Count);

            foreach ( XmlNode param in elements )
                commandLineParams.Add(param.Attributes["name"].Value, param.Attributes["value"].Value);

            XmlNode initTest = xml.ChildNodes[FindElement(xml, "TestInitialize")];

			if (initTest != null)
			{
				foreach (XmlNode child in initTest.ChildNodes)
				{
					if (child.NodeType == XmlNodeType.Text)
						initTestText = child.Value;
				}
			}
        }

        public ScenarioGroupCollection ScenarioGroups {
            get { return groups; }
        }

        public bool Passed {
            get { return passed; }
        }

        public string Comment {
            get { return comment; }
        }

        public int TotalCount {
            get { return totalCount; }
        }

        public int FailCount {
            get { return failCount; }
        }

        public NameValueCollection CommandLineParameters {
            get { return commandLineParams; }
        }

        public string InitTestText {
            get { return initTestText; }
        }

        public static TestResult BuildTestResult(XmlNode node) {
            TestResult result = new TestResult(node);

            foreach ( XmlNode child in FindAllElements(node, "ScenarioGroup") )
                result.ScenarioGroups.Add(ScenarioGroup.BuildScenarioGroup(child));

            return result;
        }
    }

    /// <summary>
    /// Node representing a ScenarioGroup.
    /// </summary>
    public class ScenarioGroup : LogNode {
        private TestResult parent;
        private ScenarioCollection scenarios;
        private bool passed;
        private int totalCount;
        private int failCount;
        private StringCollection missingScenarios = new StringCollection();
        private StringCollection extraScenarios = new StringCollection();

        // Use BuildScenarioGroup() externally
        private ScenarioGroup(XmlNode xml) : base(xml) {
            scenarios = new ScenarioCollection(this);
            
            XmlNode classResults = xml.ChildNodes[FindElement(xml, "ClassResults")];

            passed = classResults.Attributes["type"].Value == "Pass";
            totalCount = Int32.Parse(classResults.Attributes["total"].Value);
            failCount = Int32.Parse(classResults.Attributes["fail"].Value);

            foreach ( XmlNode missing in FindAllElements(classResults, "MissingScenario") )
                missingScenarios.Add(missing.Attributes["name"].Value);

            foreach ( XmlNode extra in FindAllElements(classResults, "ExtraScenario") )
                extraScenarios.Add(extra.Attributes["name"].Value);
        }
        
        public TestResult Parent {
            get { return parent; }
        }

        internal void SetParent(TestResult r) {
            parent = r;
        }

        public ScenarioCollection Scenarios {
            get { return scenarios; }
        }

        public bool Passed {
            get { return passed; }
        }

        public int TotalCount {
            get { return totalCount; }
        }

        public int FailCount {
            get { return failCount; }
        }

        public StringCollection MissingScenarios {
            get { return missingScenarios; }
        }

        public StringCollection ExtraScenarios {
            get { return extraScenarios; }
        }

        internal static ScenarioGroup BuildScenarioGroup(XmlNode node) {
            ScenarioGroup group = new ScenarioGroup(node);

            foreach ( XmlNode child in FindAllElements(node, "Scenario") )
                group.Scenarios.Add(Scenario.BuildScenario(child));

            return group;
        }
    }

    /// <summary>
    /// Node representing an individual Scenario.
    /// </summary>
    public class Scenario : LogNode {
        private ScenarioGroup parent;
        private bool passed;
        private string comment;
        private string text;
        private int totalCount;
        private int failCount;
        private BugInfoCollection bugs = new BugInfoCollection();
		private ExpectedActualInfoCollection expectedActualCol = new ExpectedActualInfoCollection();
		private ExceptionInfoCollection exceptions = new ExceptionInfoCollection();

        // Use BuildScenario() externally
        private Scenario(XmlNode xml) : base(xml) {
            bool gotPassed = false;

            foreach ( XmlNode node in xml.ChildNodes ) {
                if ( node.NodeType == XmlNodeType.Text )
                    text += node.Value;
                else if ( node.LocalName == "Result" ) {
                    // Child will be the optional comment
                    passed = node.Attributes["type"].Value == "Pass";
                    gotPassed = true;

                    if ( node.Attributes.Count == 3 ) {
                        totalCount = Int32.Parse(node.Attributes["total"].Value);
                        failCount = Int32.Parse(node.Attributes["fail"].Value);
                    }

                    if ( node.ChildNodes.Count > 0 ) {
                        int index = FindElement(node, "ResultComments");
                        XmlNode commentElement = node.ChildNodes[index];

                        // CONSIDER: This should never happen.  ChildNodes[] would throw first.
                        if ( commentElement != null )
                            comment = commentElement.ChildNodes[0].Value.Trim();
                    }
                }
                else if ( node.LocalName == "KnownBug" ) {
                    string db = node.Attributes["db"].Value;
                    int id = Int32.Parse(node.Attributes["id"].Value);
                    string comment = null;

                    // Optional comment
                    if ( node.ChildNodes.Count == 1 )
                        comment = node.ChildNodes[0].Value.Trim();

                    bugs.Add(new BugInfo(db, id, comment));
                }
				else if (node.LocalName == "ExpectedActual")
				{
					try
					{
						if (node.ChildNodes == null)
							continue;

						string expectedType, expectedValue, actualType, actualValue;
						expectedType = expectedValue = actualType = actualValue = String.Empty;

						XmlNode expectedNode = node.ChildNodes[FindElement(node, "Expected")];
						expectedType = expectedNode.Attributes["type"].Value;
						if (expectedNode.ChildNodes != null && expectedNode.ChildNodes.Count > 0)
							expectedValue = expectedNode.ChildNodes[0].Value.Trim();

						XmlNode actualNode = node.ChildNodes[FindElement(node, "Actual")];
						actualType = actualNode.Attributes["type"].Value;
						if (actualNode.ChildNodes != null && actualNode.ChildNodes.Count > 0)
							actualValue = actualNode.ChildNodes[0].Value.Trim();
						this.expectedActualCol.Add(new ExpectedActualInfo(expectedValue, expectedType, actualValue, actualType));
					}
					catch (Exception ex)
					{ Trace.WriteLine(ex.ToString()); }
				}
				else if (node.LocalName == "Exception" || node.LocalName == "InnerException")
				{
					try 
					{
						bool isInner = (node.LocalName == "Exception") ? false : true;
						string type = node.Attributes["type"].Value;
						ExceptionInfo ei = new ExceptionInfo(type, null, isInner);
						if (node.ChildNodes == null || node.ChildNodes.Count <= 0)
						{
							this.exceptions.Add(ei);
							continue;
						}

						ei.Message = node.ChildNodes[FindElement(node, "Message")].ChildNodes[0].Value.Trim();

						int index = FindElement(node, "StackTrace");
						if (index < 0)
						{
							this.exceptions.Add(ei);
							continue;
						}

						XmlNode stackNode = node.ChildNodes[index];
						foreach (XmlNode frameNode in stackNode.ChildNodes)
						{
							if (frameNode == null || frameNode.LocalName!="Frame")
								continue;
							string function = frameNode.Attributes["function"].Value;
							string file = frameNode.Attributes["file"].Value;
							int line = Int32.Parse(frameNode.Attributes["line"].Value);
							ei.StackTrace.AddFrame(function, file, line);
						}
						this.exceptions.Add(ei);
					}
					catch (Exception ex)
					{ Trace.WriteLine(ex.ToString()); }
				}
            }

            // Text, comment, total, and fail counts are optional
            Debug.Assert(gotPassed);
        }

        public ScenarioGroup Parent {
            get { return parent; }
        }

        internal void SetParent(ScenarioGroup r) {
            parent = r;
        }

        public bool Passed {
            get { return passed; }
        }

        public string Comment {
            get { return comment; }
        }

        public string Text {
            get { return text; }
        }

        public int TotalCount {
            get { return totalCount; }
        }

        public int FailCount {
            get { return failCount; }
        }

        public BugInfoCollection KnownBugs {
            get { return bugs; }
        }

		public ExpectedActualInfoCollection ExpectedActualValues
		{
			get { return expectedActualCol; }
		}

		public ExceptionInfoCollection Exceptions
		{
			get { return exceptions; }
		}

        internal static Scenario BuildScenario(XmlNode node) {
            return new Scenario(node);
        }
    }

    /// <summary>
    /// Simple data structure to hold information about a bug.
    /// </summary>
    public class BugInfo {
        string db;
        int id;
        string comment;

        public BugInfo(string db, int id) : this(db, id, null) { }

        public BugInfo(string db, int id, string comment) {
            this.db = db;
            this.id = id;
            this.comment = comment;
        }

        public string Db {
            get { return db; }
            set { db = value; }
        }

        public int Id {
            get { return id; }
            set { id = value; }
        }

        public string Comment {
            get { return comment == null ? "" : comment; }
            set { comment = value; }
        }
    }

	public class ExpectedActualInfo
	{
		string expected;
		string expectedType;

		string actual;
		string actualType;

		public ExpectedActualInfo(string expectedValue, string expectedType, string actualValue, string actualType)
		{
			this.expected = expectedValue;
			this.expectedType = expectedType;
			this.actual = actualValue;
			this.actualType = actualType;
		}

		public string ExpectedValue
		{
			get
			{
				return expected;
			}
			set
			{
				expected = value;
			}
		}

		public string ExpectedType
		{
			get
			{
				return expectedType;
			}
			set
			{
				expectedType = value;
			}
		}

		public string ActualValue
		{
			get
			{
				return actual;
			}
			set
			{
				actual = value;
			}
		}

		public string ActualType
		{
			get
			{
				return actualType;
			}
			set
			{
				actualType = value;
			}
		}
	}

	public class ExceptionInfo
	{
		string type;
		string message;
		bool isInner = false;
		StackTraceInfo stackTraceInfo = new StackTraceInfo();

		public ExceptionInfo(string type, string message, bool isInner)
		{
			this.type = type;
			this.message = message;
			this.isInner = isInner;
		}

		public ExceptionInfo(string type, string message, bool isInner, StackTraceInfo stackTraceInfo)
		{
			this.type = type;
			this.message = message;
			this.isInner = isInner;
			this.stackTraceInfo = stackTraceInfo;
		}

		public string Type
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public string Message
		{
			get
			{
				return message;
			}
			set
			{
				message = value;
			}
		}

		public bool IsInner
		{
			get
			{
				return isInner;
			}
			set
			{
				isInner = value;
			}
		}

		public StackTraceInfo StackTrace
		{
			get
			{
				return stackTraceInfo;
			}
			set
			{
				stackTraceInfo = value;
			}
		}

	}

	public class StackTraceInfo
	{
		FrameInfoCollection frameInfoCollection = new FrameInfoCollection();

		public StackTraceInfo()
		{
		}

		public FrameInfoCollection Frames
		{
			get
			{
				return frameInfoCollection;
			}
			set
			{
				frameInfoCollection = value;
			}
		}

		public void AddFrame(string function, string fileName, int lineNumber)
		{
			frameInfoCollection.Add(new FrameInfo(function, fileName, lineNumber));
		}
	}

	public class FrameInfo
	{
		string function;
		string fileName;
		int lineNumber;

		public FrameInfo(string function, string fileName, int lineNumber)
		{
			this.function = function;
			this.fileName = fileName;
			this.lineNumber = lineNumber;
		}

		public string Function
		{
			get
			{
				return function;
			}
			set
			{
				function = value;
			}
		}

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
			set
			{
				lineNumber = value;
			}
		}

	}


}
