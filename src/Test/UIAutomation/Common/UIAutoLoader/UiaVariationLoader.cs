// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 618
using System;
using System.Reflection;
using System.Xml;
using System.Windows.Automation;
using Microsoft.Test.Logging;
using System.ComponentModel;
using Microsoft.Test.Hosting;

namespace Microsoft.Test.UIAutomaion
{
    /// <summary>
    /// UIautomation Variation Loader runs xtc
    /// </summary>
    public static class UiaVariationLoader
    {
        /// <summary>
        /// Run Xtc variations
        /// </summary>
        /// <param name="xamlBrowserHost"></param>
        /// <param name="testName"></param>
        /// <param name="scenarioNode"></param>
        public static void RunXtcVariations(XamlBrowserHost xamlBrowserHost, string testName, XmlElement scenarioNode)
        {
            //Ensure the window state is normal and the position is 0,0
            AutomationElement topWindowElement = AutomationElement.FromHandle(xamlBrowserHost.HostWindowHandle);
            WindowPattern window = topWindowElement.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            window.SetWindowVisualState(WindowVisualState.Normal);
            TransformPattern transform = topWindowElement.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;
            transform.Move(0, 0);
    
            string currentXamlFile = null;
            string className = scenarioNode["INIT"].GetAttribute("Class");
            string assemblyName = scenarioNode["INIT"].GetAttribute("Assembly");

            XmlNodeList variationNodes = scenarioNode.SelectNodes("VARIATION");
            foreach (XmlElement variation in variationNodes)
            {
                string id = variation.GetAttribute("ID");
                XmlElement uiaTestNode = variation["UIAPatternTest"];
                string xamlFile = uiaTestNode.GetAttribute("XamlFile");
                string controlName = uiaTestNode.GetAttribute("ControlName");

                TestLog log = new TestLog(id);

                //only navigate to the page if you are not already on that page
                //Navigate to the xaml file
                if (currentXamlFile != xamlFile)
                {
                    xamlBrowserHost.Navigate(xamlFile, true);
                    currentXamlFile = xamlFile;
                }

                //it is in assemblyName
                Assembly asm = Assembly.LoadWithPartialName(assemblyName);

                //Get the type and create the test
                Type testType = asm.GetType(className, true);
                UiaDistributedTestcase uiaTest = (UiaDistributedTestcase)Activator.CreateInstance(testType);

                //set the properties
                foreach (XmlElement parmNode in uiaTestNode.SelectNodes("Property"))
                {
                    string parmName = parmNode.GetAttribute("Name");
                    string parmValue = parmNode.GetAttribute("Value");
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(testType)[parmName];

                    if (prop == null)
                    {
                        log.LogEvidence("can't find the property");
                        log.Result = TestResult.Fail;
                    }

                    if (prop.PropertyType == typeof(string))
                        prop.SetValue(uiaTest, parmValue);
                    else if (prop.Converter.CanConvertFrom(typeof(string)))
                    {
                        object propVal = prop.Converter.ConvertFromInvariantString(parmValue);
                        prop.SetValue(uiaTest, propVal);
                    }
                    else
                    {
                        log.LogEvidence("cant convert to the correct type from a string");
                        log.Result = TestResult.Fail;
                    }
                }


                //perform the test
                try
                {
                    xamlBrowserHost.RunTestcase(uiaTest, controlName);
                }
                catch (Exception e)
                {
                    log.LogEvidence("An unexpected Exception has occured");
                    log.LogEvidence(e.ToString());
                    log.Result = TestResult.Fail;
                }

                log.Close();
            }
        }



    }




}
