using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// The ControlsLoader class is responsible for running a set of component model tests
    /// as defined by an xtc file, or a single test from the command line.
    /// </summary>
    public class ControlsLoader
    {
        public void RunTest()
        {
            XmlDocument xmlDoc = new XmlDocument() ;
            xmlDoc.Load(DriverState.DriverParameters["XtcFileName"]);
            XmlElement dataElement = (XmlElement)xmlDoc["XTC"]["TEST"]["DATA"];
            XmlElement initElement = dataElement["INIT"];
            Type testType = LoadType(initElement);
            controls = LoadControls(initElement);

            //Set the Window type
            string windowType = "NavigationWindow";
            if (initElement.HasAttribute("Window"))
            {
                windowType = "Window";
            }

            //using (VariationContext varContext = new VariationContext(xmlDoc["XTC"]["TEST"].GetAttribute("Name")))
            //{
                XmlElement pictElement = (XmlElement)xmlDoc["XTC"]["TEST"]["DATA"]["PICT"];
                XmlNodeList variationList = dataElement.GetElementsByTagName("VARIATION");

                // If pictElement is not null, we parser test data for pict tests
                if (pictElement != null)
                {
                    XmlElement xamlsElement = (XmlElement)xmlDoc["XTC"]["TEST"]["DATA"]["XAMLS"];

                    string pictFileName = pictElement.Attributes["Name"].Value;
                    string args = pictElement.Attributes["Args"].Value;
                    string excludes = pictElement.Attributes["Excludes"].Value;

                    List<Dictionary<string, string>> testVariations = VariationGenerator.CombineVariations(VariationGenerator.GeneratePictVariations(pictFileName, args, excludes), VariationGenerator.GenerateVariations(xamlsElement, variationList));

                    foreach (Dictionary<string, string> testVariation in testVariations)
                    {
                        StringBuilder testInfo = new StringBuilder();
                        foreach (KeyValuePair<string, string> keyValuePair in testVariation)
                        {
                            testInfo.Append(keyValuePair.Key);
                            testInfo.Append("=");
                            testInfo.Append(keyValuePair.Value);
                            testInfo.Append(" ");
                        }

                        try
                        {
                            StepsTest test = Activator.CreateInstance(testType, new object[] { testVariation, testInfo.ToString() }) as StepsTest;
                            test.Run();
                        }
                        catch (Exception e)
                        {
                            GlobalLog.LogEvidence("*FAIL: Create StepsTest failed " + e.Message);
                            GlobalLog.LogEvidence(testInfo.ToString());
                        }
                    }
                }
                else if (String.Compare(initElement.Attributes["Class"].Value, "ControlsIntegrationTest") == 0)
                {
                    XmlAttribute timeoutAttribute = xmlDoc["XTC"]["TEST"].Attributes["Timeout"];

                    foreach (XmlElement variation in variationList)
                    {
                        if (timeoutAttribute != null)
                        {
                            variation.Attributes.Append(timeoutAttribute);
                        }

                        try
                        {
                            StepsTest test = Activator.CreateInstance(testType, new object[] { variation }) as StepsTest;
                            test.Run();
                        }
                        catch (Exception e)
                        {
                            GlobalLog.LogEvidence("*FAIL: Create ControlsIntegrationTest failed " + e.Message);
                            StringBuilder testInfoBuilder = new StringBuilder();
                            testInfoBuilder.Append("Variation: ");
                            foreach (XmlAttribute attribute in variation.Attributes)
                            {
                                testInfoBuilder.Append(attribute.Name);
                                testInfoBuilder.Append("=");
                                testInfoBuilder.Append(attribute.Value);
                                testInfoBuilder.Append(" ");
                            }
                            GlobalLog.LogEvidence(testInfoBuilder.ToString());
                        }
                    }
                }
                else
                {
                    // Loop over each variation node, logging and executing each.
                    // If the user has specified a specific Variation ID, we'll only run that one.
                    foreach (XmlNode node in dataElement.ChildNodes)
                    {
                        XmlElement variationNode = node as XmlElement;

                        if (variationNode == null) continue;

                        if (variationNode.Name.Equals("VARIATION"))
                        {
                            using (TestLog log = new TestLog(variationNode.GetAttribute("ID")))
                            {
                                CreateWindow(variationNode, windowType);
                                log.Result = RunVariation(testType, variationNode);
                                // Close window for each variation to enable ExecutionGroup to run a group of tests
                                // under the same appdomain to reduce test cases run time.
                                w.Close();
                            }
                        }
                    }
                }
            //}
        }

        // build a collection of CONTROLS.
        private Dictionary<string, XmlElement> LoadControls(XmlElement initNode)
        {
            Dictionary<string, XmlElement> controls = new Dictionary<string, XmlElement>(StringComparer.InvariantCultureIgnoreCase);
            
            foreach (XmlElement node in initNode.SelectNodes("CONTROLS/CONTROL"))
            {
                string name = node.GetAttribute("Name");
                if (String.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Missing a Name for CONTROL in INIT/CONTROLS table.");
                }
                if (controls.ContainsKey(name))
                {
                    throw new ArgumentException("A control in the INIT/CONTROLS table already exists with the name: " + name);
                }
                controls.Add(name, node);
            }
            return controls;
        }

     
        /// <summary>
        /// Run a variation, based on the given test type and variation node.
        /// This function simply creates the test object and invokes it. 
        /// </summary>
        private TestResult RunVariation(Type testType, XmlElement variation)
        {
            TestResult tr = TestResult.Unknown;

            TestLog.Current.LogStatus("Executing Variation ID=" + variation.GetAttribute("ID"));

            object testObj = Activator.CreateInstance(testType, new object[] { });
            if (testObj == null)
                throw new ArgumentException("Can't create object '" + testType.Name + "' (do you have a ctor with no args?)");

            // Our policy for performing the test is that we first look for a control with the name "CONTROLTOTEST". If there is none, we
            // grab the first child of the stackpanel if it exists, or the stackpanel itself if there are no children.
            // 
            UIElement controlToTest = System.Windows.LogicalTreeHelper.FindLogicalNode(_hostApp.MainStackPanel, "CONTROLTOTEST") as UIElement;
            if (controlToTest == null)
            {
                if (_hostApp.MainStackPanel.Children.Count > 0)
                {
                    controlToTest = _hostApp.MainStackPanel.Children[0];
                }
                else
                {
                    controlToTest = _hostApp.MainStackPanel;
                }
            }

            // Right now we call IScenario differently because it doesn't return a TestResult, but when we remove IScenario we can just
            // call it in one way.
            // 

            IUnitTest unitTest = testObj as IUnitTest;
            if (unitTest == null)
            {
                IIntegrationTest integrationTest = testObj as IIntegrationTest;
                if (integrationTest == null)
                {
                    throw new ArgumentException("Can't cast object '" + testType.Name + "' to IScenario, IUnitTest, or IIntegrationTest.  Tests must implement one of these.");
                }
                else
                {
                    tr = integrationTest.Perform(controlToTest as object, variation);
                    TestLog.Current.Result = tr;
                }
            }
            else
            {
                tr = unitTest.Perform(controlToTest as object, variation);
                TestLog.Current.Result = tr;
            }

            return tr;
        }

        /// <summary>
        /// Load a type from an INIT node
        /// </summary>
        /// <param name="initNode">Xml INIT node from the xtc file</param>
        /// <returns>
        /// The type loaded from the assembly, matching the class attribute in the init node.
        /// Returns null if the class isn't found.
        /// </returns>
        private Type LoadType(XmlElement initNode)
        {
            string assemblyName = initNode.GetAttribute("Assembly");
            string className    = initNode.GetAttribute("Class");

            // If assemblyname attribute in empty, it should look in ControlsCommon and
            // ControlsTest.
            if (assemblyName.Equals(string.Empty))
            {
                Type loadedType = LoadType("ControlsCommon.dll", className);
                if (loadedType == null)
                {
                    loadedType = LoadType("ControlsTest.dll", className);
                }
                if (loadedType == null)
                {
                    loadedType = LoadType("ControlsOrcas.dll", className);
                }

                if (loadedType == null)
                    throw new ArgumentException("Can't create type object '" + className + "' (class or assembly missing?)");
                return loadedType;
            }
            else
            {
                return LoadType(assemblyName, className);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        private Type LoadType(string assemblyName, string className)
        {
            Assembly a = Assembly.LoadFile(Path.GetFullPath(assemblyName));
            foreach (Type t in a.GetTypes())
            {
                if (t.Name == className)
                {
                    return t;
                }
            }

            return null;
        }




        private object CreateWindow(XmlElement variation, string windowType)
        {
            if ( windowType.Equals("Window") )
                w = new Window();
            else
                w = new NavigationWindow();

            // Here we create the environment we want
            // 
            XmlElement control = null;
            string name;
            if (variation != null)
            {
                control = variation["CONTROL"];

                if (control != null)
                {
                    name = control.GetAttribute("Name");

                    // search the CONTROLS dictionary to get the control when the name matches.
                    if (!String.IsNullOrEmpty(name))
                    {
                        if (!controls.ContainsKey(name))
                        {
                            throw new ArgumentException("Could not find a control in the INIT/CONTROLS table that has the name: " + name);
                        }
                        control = controls[name];
                    }
                }
            }
            else
            {
                control = null;
            }

            StackPanel _mainFp = new StackPanel();

            w.Content = _mainFp;
            w.Show();
            w.Activate();

            _hostApp.MainStackPanel = _mainFp;

            if (control != null)
            {
                object newControl = ObjectFactory.CreateObjectFromXaml(control.InnerXml);
                _hostApp.MainStackPanel.Children.Add((FrameworkElement)newControl);
            }

            QueueHelper.WaitTillQueueItemsProcessed();
            return null;
        }

        #region Variables

        Dictionary<string, XmlElement> controls = null;
        StackPanel _mainSp;
        static Window w = null;
        static public ControlsLoader _hostApp = new ControlsLoader();

        #endregion

        /// <summary>
        /// Add framework elements to test to this flow panel
        /// </summary>
        /// <value>The main flow panel of our app</value>
        public StackPanel MainStackPanel
        {
            get
            {
                return _mainSp;
            }
            set { _mainSp = value; }
        }
    }
}
