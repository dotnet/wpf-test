using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    public class ActionTest : IUnitTest   //Adapter
    {
        public TestResult Perform(object testElement, XmlElement variation)
        {
            // Parse the scene, use ActionExecutor to execute each action until one return false.
            // Init State: single item is selected.
            TestResult testResult = TestResult.Pass;
            FrameworkElement actionTarget = (FrameworkElement)testElement;

            XmlNodeList actionElements = variation.SelectNodes("ACTIONS/*");
            foreach (XmlElement actionElement in actionElements)
            {
                GlobalLog.LogStatus("Performing action - " + actionElement.LocalName);

                if (!ActionExecutor.Execute(actionTarget, actionElement))
                {
                    testResult = TestResult.Fail;
                    break;
                }
                
            }
            return testResult;
        }
    }
}


