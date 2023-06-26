// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Hooks up the RtfXamlView tool to Editing Test framework 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Text;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    
    using Microsoft.Test;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;        
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    using RtfXamlView;

    #endregion Namespaces.

    /// <summary>TestCase# 491: Provides an interactive test case for TextBox.</summary> 
    [Test(1, "XamlRtf", "RtfXamlViewBVT", MethodName = "DriverEntryPoint", MethodParameters = @"/TestCaseType:RtfXamlViewBVT /BVTDir ./rtfCIT /LogFile ./RtfXamlViewBVT.log /LogPri0Only 1 /UseXCVT 0 /RTRtfXaml", TestParameters = "Class=EntryPointType", SupportFiles = @"FeatureTests\Editing\rtf", Timeout = 200, Keywords = "Setup_SanitySuite")]    
    public class RtfXamlViewBVT
    {        
        #region Private fields

        RtfXamlViewApp _viewerApp;

        /// <summary>
        /// Right now the main goal is to find regression in Rtf->Xaml converstion.
        /// While we work through the known failures, this number may have to be changed.
        /// </summary>
        int _expectedFailCount = 8;

        #endregion Private fields

        /// <summary>Runs the test case.</summary>        
        [TestEntryPoint]
        public void RunTestCase()
        {
            _viewerApp = new RtfXamlViewApp(ConfigurationSettings.Current.CommandLineArguments);
            _viewerApp.Run();
            
            ProcessLogFile();
        }

        private void ProcessLogFile()
        {
            int failCount = 0;
            string input = "";
            StreamReader re = File.OpenText(_viewerApp._szLogFile);

            if (Logger.Current.TestLog == null)
            {
                TestLog logger = new TestLog("EditingLogger");
                Logger.Current.TestLog = logger;
            }            
            Logger.Current.Log("---------------------------------FAILED CASES ----------------------------------");
            while ((input = re.ReadLine()) != null)
            {
                if ((input.Contains("Pass")) || (input.Contains("Windows OS bug")))
                {
                    continue;
                }
                else
                {
                    failCount++;
                    Logger.Current.Log(input + "\r\n");
                }
            }
            Logger.Current.Log("---------------------------------END OF FAILED CASES ----------------------------");
            re.Close();
            //_rtfPanel.GetWindow().Close();
            //_xamlPanel.GetWindow().Close();

            Logger.Current.Log("Total failed count: " + failCount);
            Logger.Current.Log("Expected failed count: " + _expectedFailCount);

            //if (pass == true)
            if(failCount <= _expectedFailCount)
            {
                Logger.Current.ReportResult(true, "Test passed successfully.", true);                
            }
            else
            {
                Logger.Current.ReportResult(false, "\r\n*********************TEST HAS FAILED***************************\r\n", true);
            }

            if (Logger.Current.TestLog != null)
            {                
                Logger.Current.TestLog.Close();
                Logger.Current.TestLog = null;
            }            
        }        
    }
}
