// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        private string _name;
        private XmlNode _xml;

        public LogNode(XmlNode xml) {
            this._xml = xml;

            XmlAttribute nameAttr = xml.Attributes["name"];

            if ( nameAttr == null )
                throw new Exception("No name on xml node");

            _name = nameAttr.Value;
        }

        public string Name {
            get { return _name; }
        }

        public virtual XmlNode XmlNode {
            get { return _xml; }
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
        private ScenarioGroupCollection _groups;
        private bool _passed;
        private int _totalCount;
        private int _failCount;
        private string _comment;
        private NameValueCollection _commandLineParams;
        private string _initTestText;

        internal TestResult(XmlNode xml) : base(xml) {
            _groups = new ScenarioGroupCollection(this);

            // Init FinalResults values
            XmlNode finalResults = xml.ChildNodes[FindElement(xml, "FinalResults")];

			if (finalResults != null)
			{
				if (finalResults.Attributes["type"] != null)
					_passed = finalResults.Attributes["type"].Value == "Pass";

				if (finalResults.Attributes["total"] != null)
					_totalCount = Int32.Parse(finalResults.Attributes["total"].Value);
				if (finalResults.Attributes["fail"] != null)
					_failCount = Int32.Parse(finalResults.Attributes["fail"].Value);
			}

            if ( finalResults.ChildNodes.Count > 0 ) {
                XmlNode commentElement = finalResults.ChildNodes[FindElement(finalResults, "ResultComments")];
                _comment = commentElement.ChildNodes[0].Value.Trim();
            }

            // Init remaining values
            ArrayList elements = FindAllElements(xml, "CommandLineParameter");
            _commandLineParams = new NameValueCollection(elements.Count);

            foreach ( XmlNode param in elements )
                _commandLineParams.Add(param.Attributes["name"].Value, param.Attributes["value"].Value);

            XmlNode initTest = xml.ChildNodes[FindElement(xml, "TestInitialize")];

			if (initTest != null)
			{
				foreach (XmlNode child in initTest.ChildNodes)
				{
					if (child.NodeType == XmlNodeType.Text)
						_initTestText = child.Value;
				}
			}
        }

        public ScenarioGroupCollection ScenarioGroups {
            get { return _groups; }
        }

        public bool Passed {
            get { return _passed; }
        }

        public string Comment {
            get { return _comment; }
        }

        public int TotalCount {
            get { return _totalCount; }
        }

        public int FailCount {
            get { return _failCount; }
        }

        public NameValueCollection CommandLineParameters {
            get { return _commandLineParams; }
        }

        public string InitTestText {
            get { return _initTestText; }
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
        private TestResult _parent;
        private ScenarioCollection _scenarios;
        private bool _passed;
        private int _totalCount;
        private int _failCount;
        private StringCollection _missingScenarios = new StringCollection();
        private StringCollection _extraScenarios = new StringCollection();

        // Use BuildScenarioGroup() externally
        private ScenarioGroup(XmlNode xml) : base(xml) {
            _scenarios = new ScenarioCollection(this);
            
            XmlNode classResults = xml.ChildNodes[FindElement(xml, "ClassResults")];

            _passed = classResults.Attributes["type"].Value == "Pass";
            _totalCount = Int32.Parse(classResults.Attributes["total"].Value);
            _failCount = Int32.Parse(classResults.Attributes["fail"].Value);

            foreach ( XmlNode missing in FindAllElements(classResults, "MissingScenario") )
                _missingScenarios.Add(missing.Attributes["name"].Value);

            foreach ( XmlNode extra in FindAllElements(classResults, "ExtraScenario") )
                _extraScenarios.Add(extra.Attributes["name"].Value);
        }
        
        public TestResult Parent {
            get { return _parent; }
        }

        internal void SetParent(TestResult r) {
            _parent = r;
        }

        public ScenarioCollection Scenarios {
            get { return _scenarios; }
        }

        public bool Passed {
            get { return _passed; }
        }

        public int TotalCount {
            get { return _totalCount; }
        }

        public int FailCount {
            get { return _failCount; }
        }

        public StringCollection MissingScenarios {
            get { return _missingScenarios; }
        }

        public StringCollection ExtraScenarios {
            get { return _extraScenarios; }
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
        private ScenarioGroup _parent;
        private bool _passed;
        private string _comment;
        private string _text;
        private int _totalCount;
        private int _failCount;
        private BugInfoCollection _bugs = new BugInfoCollection();
		private ExpectedActualInfoCollection _expectedActualCol = new ExpectedActualInfoCollection();
		private ExceptionInfoCollection _exceptions = new ExceptionInfoCollection();

        // Use BuildScenario() externally
        private Scenario(XmlNode xml) : base(xml) {
            bool gotPassed = false;

            foreach ( XmlNode node in xml.ChildNodes ) {
                if ( node.NodeType == XmlNodeType.Text )
                    _text += node.Value;
                else if ( node.LocalName == "Result" ) {
                    // Child will be the optional comment
                    _passed = node.Attributes["type"].Value == "Pass";
                    gotPassed = true;

                    if ( node.Attributes.Count == 3 ) {
                        _totalCount = Int32.Parse(node.Attributes["total"].Value);
                        _failCount = Int32.Parse(node.Attributes["fail"].Value);
                    }

                    if ( node.ChildNodes.Count > 0 ) {
                        int index = FindElement(node, "ResultComments");
                        XmlNode commentElement = node.ChildNodes[index];

                        // CONSIDER: This should never happen.  ChildNodes[] would throw first. 
                        if ( commentElement != null )
                            _comment = commentElement.ChildNodes[0].Value.Trim();
                    }
                }
                else if ( node.LocalName == "KnownBug" ) {
                    string db = node.Attributes["db"].Value;
                    int id = Int32.Parse(node.Attributes["id"].Value);
                    string comment = null;

                    // Optional comment
                    if ( node.ChildNodes.Count == 1 )
                        comment = node.ChildNodes[0].Value.Trim();

                    _bugs.Add(new BugInfo(db, id, comment));
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
						this._expectedActualCol.Add(new ExpectedActualInfo(expectedValue, expectedType, actualValue, actualType));
					}
					catch (Exception ex)
					{ 
						Trace.WriteLine(ex.ToString()); 
					}
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
							this._exceptions.Add(ei);
							continue;
						}

						ei.Message = node.ChildNodes[FindElement(node, "Message")].ChildNodes[0].Value.Trim();

						int index = FindElement(node, "StackTrace");
						if (index < 0)
						{
							this._exceptions.Add(ei);
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
						this._exceptions.Add(ei);
					}
					catch (Exception ex)
					{ Trace.WriteLine(ex.ToString()); }
				}
            }

            // Text, comment, total, and fail counts are optional
            Debug.Assert(gotPassed);
        }

        public ScenarioGroup Parent {
            get { return _parent; }
        }

        internal void SetParent(ScenarioGroup r) {
            _parent = r;
        }

        public bool Passed {
            get { return _passed; }
        }

        public string Comment {
            get { return _comment; }
        }

        public string Text {
            get { return _text; }
        }

        public int TotalCount {
            get { return _totalCount; }
        }

        public int FailCount {
            get { return _failCount; }
        }

        public BugInfoCollection KnownBugs {
            get { return _bugs; }
        }

		public ExpectedActualInfoCollection ExpectedActualValues
		{
			get { return _expectedActualCol; }
		}

		public ExceptionInfoCollection Exceptions
		{
			get { return _exceptions; }
		}

        internal static Scenario BuildScenario(XmlNode node) {
            return new Scenario(node);
        }
    }

    /// <summary>
    /// Simple data structure to hold information about a bug.
    /// </summary>
    public class BugInfo {
        string _db;
        int _id;
        string _comment;

        public BugInfo(string db, int id) : this(db, id, null) { }

        public BugInfo(string db, int id, string comment) {
            this._db = db;
            this._id = id;
            this._comment = comment;
        }

        public string Db {
            get { return _db; }
            set { _db = value; }
        }

        public int Id {
            get { return _id; }
            set { _id = value; }
        }

        public string Comment {
            get { return _comment == null ? "" : _comment; }
            set { _comment = value; }
        }
    }

	public class ExpectedActualInfo
	{
		string _expected;
		string _expectedType;

		string _actual;
		string _actualType;

		public ExpectedActualInfo(string expectedValue, string expectedType, string actualValue, string actualType)
		{
			this._expected = expectedValue;
			this._expectedType = expectedType;
			this._actual = actualValue;
			this._actualType = actualType;
		}

		public string ExpectedValue
		{
			get
			{
				return _expected;
			}
			set
			{
				_expected = value;
			}
		}

		public string ExpectedType
		{
			get
			{
				return _expectedType;
			}
			set
			{
				_expectedType = value;
			}
		}

		public string ActualValue
		{
			get
			{
				return _actual;
			}
			set
			{
				_actual = value;
			}
		}

		public string ActualType
		{
			get
			{
				return _actualType;
			}
			set
			{
				_actualType = value;
			}
		}
	}

	public class ExceptionInfo
	{
		string _type;
		string _message;
		bool _isInner = false;
		StackTraceInfo _stackTraceInfo = new StackTraceInfo();

		public ExceptionInfo(string type, string message, bool isInner)
		{
			this._type = type;
			this._message = message;
			this._isInner = isInner;
		}

		public ExceptionInfo(string type, string message, bool isInner, StackTraceInfo stackTraceInfo)
		{
			this._type = type;
			this._message = message;
			this._isInner = isInner;
			this._stackTraceInfo = stackTraceInfo;
		}

		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		public bool IsInner
		{
			get
			{
				return _isInner;
			}
			set
			{
				_isInner = value;
			}
		}

		public StackTraceInfo StackTrace
		{
			get
			{
				return _stackTraceInfo;
			}
			set
			{
				_stackTraceInfo = value;
			}
		}
	}

	public class StackTraceInfo
	{
		FrameInfoCollection _frameInfoCollection = new FrameInfoCollection();

		public StackTraceInfo()
		{
		}

		public FrameInfoCollection Frames
		{
			get
			{
				return _frameInfoCollection;
			}
			set
			{
				_frameInfoCollection = value;
			}
		}

		public void AddFrame(string function, string fileName, int lineNumber)
		{
			_frameInfoCollection.Add(new FrameInfo(function, fileName, lineNumber));
		}
	}

	public class FrameInfo
	{
		string _function;
		string _fileName;
		int _lineNumber;

		public FrameInfo(string function, string fileName, int lineNumber)
		{
			this._function = function;
			this._fileName = fileName;
			this._lineNumber = lineNumber;
		}

		public string Function
		{
			get
			{
				return _function;
			}
			set
			{
				_function = value;
			}
		}

		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				_fileName = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return _lineNumber;
			}
			set
			{
				_lineNumber = value;
			}
		}
	}
}
