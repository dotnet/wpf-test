// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 0618
using Avalon.Test.ComponentModel;
using Microsoft.Test.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using Microsoft.Test.Security.Wrappers;
using Microsoft.Test.Loaders;
using System.ComponentModel;
using Microsoft.Test.UIAutomaion;
using Microsoft.Test.Hosting;
using Microsoft.Test.CrossProcess;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// The UIAutoLoader class is responsible for running a set of component model tests
    /// as defined by an xtc file, or a single test from the command line.
    /// </summary>
    public class UIAutoLoader
    {
        #region Bootstrap

        public UIAutoLoader()
        {
        }

        public void RunUiaTest(String xtcFile)
        {
            DictionaryStore.StartServer();
            LogManager.LogMessageDangerously("*** begin RunUiaTest ***");

            // Wrap everything in a top-level universal try/catch block
            // If an exception gets to this level, it's fatal to the loader.

            try
            {
                DictionaryStore.Current["ApplicationMonitorStartNewProcessForXbap"] = "true";
                this._xtcFile = xtcFile;

                //set ApplicationMonitorStartNewProcessForXbap to be true.                
                this.RunXTC();
            }
            catch (Exception ex)
            {
                LogManager.LogMessageDangerously("*** Exception Thrown ***");
                LogManager.LogMessageDangerously(ex.Message);
                LogManager.LogMessageDangerously("************************");
                LogManager.LogMessageDangerously(ex.ToString());
                LogManager.LogMessageDangerously("************************");
            }
            finally
            {
                DictionaryStore.Close();
            }
        }

        #endregion

        /// <summary>
        /// Entry point for loader execution of an XTC file.
        /// </summary>
        private void RunXTC()
        {
            // Load XmlDocument and find the nodes we require
            XmlDocumentSW xmlDoc = new XmlDocumentSW();

            try
            {
                xmlDoc.Load(_xtcFile);
            }
            catch (FileNotFoundException)
            {
                LogManager.LogMessageDangerously(String.Format("Unable to load xtc file \"{0}\".", _xtcFile));
                throw;
            }

            XmlElement testNode = xmlDoc["XTC"]["TEST"]["SCENARIO"] as XmlElement;

            string testType = testNode["INIT"].GetAttribute("TestType");

            // This block of code is a shim to adapt the xtc format for IUIAutomationTest to the 
            // format used by UIAutomation testcases.
            Assembly asm = Assembly.LoadWithPartialName("UIAutoFTUtils");
            if (asm != null)
            {
                // IUIAutomationTest xtc format uses caps for class name.
                Type t = asm.GetType("UIAutomation." + testNode["INIT"].GetAttribute("CLASS"), false);
                if (t != null)
                {
                    if (t.GetInterface("UIAutomation.IUIAutomationTest") != null)
                    {
                        string realClass = testNode["INIT"].GetAttribute("CLASS");

                        // Alter the INIT node so it looks like a UIAutomation testcase.
                        testType = "UIAutomation";
                        testNode["INIT"].SetAttribute("TestType", "UIAutomation");
                        testNode["INIT"].SetAttribute("Assembly", "UiAutoFTUtils");
                        testNode["INIT"].SetAttribute("Class", "Avalon.Test.ComponentModel.UiaShim");

                        XmlNodeList variationNodes = testNode.SelectNodes("VARIATION");
                        foreach (XmlElement variation in variationNodes)
                        {
                            // We set the Class attribute on the INIT node to our shim, and we put
                            // the name of the IUIAutomationTest class inside the xmlelement shim
                            // so UiaShim knows what test to run.
                            variation.SetAttribute("RealClass", realClass);

                            string originalText = variation.OuterXml;

                            // Create a UIAPatternTest element with the formatting expected.
                            // Since UIAPatternTest expected a UIELEMENTID, it is important
                            // that the document element in the Xaml file be named.
                            // If either the XamlFile or UIElementID are missing, we can't run
                            // this variation. In that case, we remove this partical variation node.
                            XmlElement xe = xmlDoc.CreateElement("UIAPatternTest");
                            if (variation["XAMLFILE"] != null && variation["UIELEMENTID"] != null)
                            {
                                xe.SetAttribute("XamlFile", variation["XAMLFILE"].InnerText);
                                xe.SetAttribute("ControlName", variation["UIELEMENTID"].InnerText);

                                // If there is a <FLOW>false</FLOW> node, we don't want to run this variation.
                                if (variation["FLOW"] != null && variation["FLOW"].InnerText == "false")
                                {
                                    testNode.RemoveChild(variation);
                                }
                            }
                            else
                            {
                                testNode.RemoveChild(variation);
                            }

                            // This code is very important - it contains the variation information
                            // in the IUIAutomationTest format, which our shim will need to pull
                            // out when calling the real testcase. To get it into our shim, we
                            // use the UIAutomation property setting functionality to save the
                            // variation text as a string.
                            XmlElement propxe = xmlDoc.CreateElement("Property");
                            propxe.SetAttribute("Name", "XmlElementShim");
                            propxe.SetAttribute("Value", originalText);

                            xe.AppendChild(propxe);

                            variation.AppendChild(xe);
                        }
                    }
                }
            }

            if (string.Compare(testType, "uiautomation", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                LogManager.LogMessageDangerously("*** Testing UIAutomation ***");

                using (XamlBrowserHost xamlHost = new XamlBrowserHost())
                {
                    
                    //run the app
                    try
                    {
                        xamlHost.StartHost();
                    }
                    catch (TimeoutException timeoutException)
                    {
                        TestLog log = new TestLog("Failed Test");
                        log.LogEvidence("Unable to launch the xbap.");
                        log.LogEvidence(timeoutException.ToString());
                        log.Result = TestResult.Fail;
                        log.Close();
                    }

                    //run the test on the target application
                    UiaVariationLoader.RunXtcVariations(xamlHost, Path.GetFileNameWithoutExtension(_xtcFile), xmlDoc["XTC"]["TEST"]["SCENARIO"]);
                }
            }
        }

        #region Variables

        protected string _xtcFile = null;

        public string[] args;

        static public UIAutoLoader _hostApp = null;

        #endregion
    }
}
