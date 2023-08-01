// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Core's main Xaml generator driver. Creates xamls and 
 *          verifies they pass round-trip parsing and serialization
 *          as well as displaying.
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using System.Threading; 
using System.Collections;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Xml;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Creates xamls and verifies they pass round-trip parsing and serialization.
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("4")]
    [TestCaseArea(@"Serialization\TestXamlsCreatedWithXamlCreator")]
    [TestCaseMethod("TestXamls")]
    [TestCaseDisabled("1")]
    [TestCaseTimeout("1000000")]       
    public class TestXamlsCreatedWithXamlCreator
    {
        int _maxDepth = 5;
        int _maxAttributes = 2;
        int _maxChildren = 4;
        int _pauseMilliseconds = 250;
        bool _showXaml = false;
        string _failuresDirectory = "";
        int _seed = -1;

        /// <summary>
        /// Initialzes the generation constraints if they have been
        /// passed from the main test driver.
        /// </summary>
        private void _InitParams()
        {
            // Get current TestCaseInfo.
            TestCaseInfo testCaseInfo = testCaseInfo = TestCaseInfo.GetCurrentInfo();

            // If TestCaseInfo was available on the AppDomain,
            // read args and set generation constraints.
            if (testCaseInfo != null && testCaseInfo.Params != null && 0 < testCaseInfo.Params.Length)
            {
                SortedList args = Utility.ParseArgs(testCaseInfo.Params.Split(' '), false, true, "/");

                if(args["maxdepth"] != null)
                    _maxDepth = Convert.ToInt32(args["maxdepth"]);

                if (args["maxattributes"] != null)
                    _maxAttributes = Convert.ToInt32(args["maxattributes"]);

                if (args["maxchildren"] != null)
                    _maxChildren = Convert.ToInt32(args["maxchildren"]);

                if (args["pause"] != null)
                    _pauseMilliseconds = Convert.ToInt32(args["pause"]);

                if (args["showxaml"] != null)
                    _showXaml = true;

                if (args["seed"] != null)
                    _seed = Convert.ToInt32(args["seed"]);
            }

            if (_seed == -1)
            {
                _seed = (int)DateTime.Now.Ticks;
            }
        }
        
        // Writes text content for mixed content nodes.
        private static HandledLevel _GenerateText(XmlNode parentNode)
        {
            XmlText textNode = parentNode.OwnerDocument.CreateTextNode("abc def");
            parentNode.AppendChild(textNode);
            
            return HandledLevel.Complete;
        }

        // Writes values for xml:lang attributes.  Values written here override what the generator would create.
        private static HandledLevel _GenerateXmlLangValue(XmlNode parentNode, XmlAttribute attribute)
        {
            // We could put other value here.
            attribute.Value = "en-US";
            parentNode.Attributes.Append(attribute);

            return HandledLevel.Complete;
        }

        // Writes values for brush type attributes.  Values written here override what the generator would create.
        private static HandledLevel _GenerateBrushValue(XmlNode parentNode, XmlAttribute attribute)
        {
            string prefix = attribute.Prefix;

            if (prefix != String.Empty)
            {
                prefix += ":";
            }

            attribute.Value = "*" + prefix + "SolidColorBrush(Color=#44AAAAAA)";
            parentNode.Attributes.Append(attribute);

            return HandledLevel.Complete;
        }

        private static Hashtable s_styleKeys = new Hashtable();

        private static void _SetStyleName(XmlNode styleNode, XmlNode newNode)
        {
            XmlNode resourcesNode = styleNode.ParentNode;

            if (resourcesNode == null)
                return;

            XmlNode elementNode = resourcesNode.ParentNode;

            if (elementNode == null)
            {
                throw new NotImplementedException("Don't know how to generate name for current Style.");
            }

            // Get collection of Style names for the parent node and target type.
            if (!s_styleKeys.Contains(elementNode))
            {
                s_styleKeys[elementNode] = new Hashtable();
            }

            Hashtable styleNames = (Hashtable)s_styleKeys[elementNode];

            // Create new Style name.  If it's the first one for this
            // target under this parent, user *typeof() syntax.  Otherwise,
            // use count to create unique name.
            string newStyleName = String.Empty;
            int cnt = 0;

            if (styleNames[newNode.Name] == null)
            {
                newStyleName = "*typeof(" + newNode.Name + ")";
            }
            else
            {
                cnt = (int)styleNames[newNode.Name];
                newStyleName = newNode.Name + "Style" + cnt.ToString();
            }

            styleNames[newNode.Name] = cnt + 1;

            //
            // Add attribute
            //
            XmlAttribute newAttrib = styleNode.OwnerDocument.CreateAttribute("x", "Name", "http://schemas.microsoft.com/winfx/2006/xaml");

            newAttrib.Value = newStyleName;
            styleNode.Attributes.Append(newAttrib);
        }

        // Provides values for XamlGenerator to write for attributes.
        private static HandledLevel _GenerateElement(XmlNode parentNode, XmlNode newNode, bool isStart)
        {
            // Write x:Key atribute if we know now what the Style's 
            // target type is.
            if (isStart && parentNode.Name == "Style" && !newNode.Name.Contains("VisualTree"))
            {
                _SetStyleName(parentNode, newNode);
                return HandledLevel.Partial;
            }
            // return false to mark this element as 'not handled'.
            // The node will be added to the parent.
            else
            {
                return HandledLevel.None;
            }
        }

        static XamlGenerator s_xamlgen = null;

        /// <summary>
        /// Create xamls and test
        /// </summary>
        public void TestXamls()
        {
            _InitParams();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(_StopProcess);

            // Load avalon and core schemas from assembly resources.
            Stream avalonXsdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("testxamls_avalon.xsd");
            Stream coreXsdStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("testxamls_core.xsd");

            // Create generator.
            s_xamlgen = new XamlGenerator(avalonXsdStream, new Stream[] { coreXsdStream });

            avalonXsdStream.Close();
            coreXsdStream.Close();

            // Set seed for random number generator.
            s_xamlgen.Reset(_seed);

            // Initialize failures directory.
            _failuresDirectory = "XamlFailures_" + _seed.ToString();

            if (Directory.Exists(_failuresDirectory))
            {
                Directory.Delete(_failuresDirectory, true);
            }

            Directory.CreateDirectory(_failuresDirectory);

            // Add Mapping PI reference for core types.
            s_xamlgen.AddMappingPI("cmn", "Microsoft.Test.Serialization.CustomElements", "TestRuntime");

            // Register helper for generating text content.
            s_xamlgen.RegisterTextHelper(new TextContentHelper(_GenerateText));

            // Register helper for generating attribute values.
            s_xamlgen.RegisterAttributeHelper(new AttributeHelper(_GenerateBrushValue), "Background", "Foreground");

            // Register helper for generating attribute values.
            s_xamlgen.RegisterAttributeHelper(new AttributeHelper(_GenerateXmlLangValue), "lang");

            // Register helper for generating elements.
            s_xamlgen.RegisterElementHelper(new ElementHelper(_GenerateElement));

            while (!_stopTheProcess)
            {
                TestLog testLog = new TestLog((++_sequenceNum).ToString());

                CoreLogger.LogStatus("\r\n");
                CoreLogger.LogStatus("Creating a new xaml...", ConsoleColor.Yellow);

                // Reset collection of style names.
                s_styleKeys.Clear();

                Stream myStream = s_xamlgen.CreateStream(_maxDepth, _maxAttributes, _maxChildren);

                // Save xaml to file.
                IOHelper.SaveTextToFile(myStream, _createdXaml);

                myStream.Close();
                
                // Optionally show xaml file in notepad.
                if (_showXaml)
                {
                    CoreTestsSingleRunServices.RunTestCaseProcess("notepad.exe", 360000, _createdXaml, false);
                }

                // Load the xaml in a new process.
                CoreTestsSingleRunServices.RunTestCaseProcess("coretests.exe", 360000, "/xamlfile=\"" + _createdXaml + "\" /actionforxaml=Serialization", true);
                
                if (TestLog.Current.Result != TestResult.Pass)
                {
                    if (TestLog.Current.Result == TestResult.Unknown)
                    {
                       CoreLogger.LogTestResult(false, "coretests.exe didn't log a pass when loading the xaml.");
                    }

                    _failedCount++;

                    string failedFileName = _failuresDirectory + @"\XamlFailure_" + _sequenceNum.ToString() + ".xaml";

                    // save failed xaml
                    File.Copy(_createdXaml, failedFileName, true);
                }

                CoreLogger.LogStatus("xaml #" + _sequenceNum.ToString() + " finished.");

                testLog.Close();

                // Log the progress of xamls loaded.
                CoreLogger.LogStatus("Xamls Generated: " + _sequenceNum + "  Fail: " + _failedCount + "\r\n");

                // Pausing allows a human to interact more easily (view results, CTRL-C to cancel run).
                if (_pauseMilliseconds > 0)
                    Thread.Sleep(_pauseMilliseconds);
            }

            return;
        }

        private void _StopProcess(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Canceled ...");
            _stopTheProcess = true;
        }

        string _createdXaml = "XamlFileCreatedWithXamlCreator.xaml";
        bool _stopTheProcess = false;
        int _sequenceNum = 0;
        int _failedCount = 0;
        SerializationHelper _serhelper = new SerializationHelper();
    }
}
