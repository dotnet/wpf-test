// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Resources
{
    /// <summary>
    /// Testing locally set values after invalidation of Resource on the Application
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT15" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT15" /SecurityLevel="FullTrust"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create Application</li>
    /// <li>Set resources on the Application</li>
    /// <li>Handle the Startup event</li>
    /// <li>Run the Application </li>
    /// <li>In the Startup event handler. </li>
    /// <li>Create an element tree</li>
    /// <li>Set a resource on the stackPanel, button, Paragraph</li>
    /// <li>Locally set new values </li>
    /// <li>Change the Resource on the Application </li>
    /// <li>Verify the new values</li>
    /// </ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT15")]
    public class BVT15 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT15 Constructor
        ******************************************************************************/
        public BVT15()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            this.Init();

            GlobalLog.LogStatus("Creating a new Application");
            Application app = new Application();

            ResourceDictionaryHelper resourceDictionayHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Setting Reosurce on the Application");
            app.Resources = resourceDictionayHelper.CreateFontWeightsStyles();
            app.Startup += new StartupEventHandler(app_Startup);

            GlobalLog.LogStatus("Running the Application");

            app.Run();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          app_Startup
        ******************************************************************************/
        private void app_Startup(object sender, StartupEventArgs e)
        {
            GlobalLog.LogStatus("Entered app_Startup");
            Application app;
            app = (Application)sender;

            ResourceDictionaryHelper rdHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Creating window and other tree Elements");

            StackPanel stackPanel = new StackPanel();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();
            Button button = new Button();

            button.Content = "hello";

            GlobalLog.LogStatus("Setting Resource on StackPanel");
            stackPanel.Resources = rdHelper.CreateSolidColorBrushes();

            GlobalLog.LogStatus("SetResourceReference on Button");
            button.SetResourceReference(Button.FontWeightProperty, "FontMedium");

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            GlobalLog.LogStatus("SetResourceReference on Paragraph");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontMedium");

            stackPanel.Children.Add(fdsv);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            GlobalLog.LogStatus("Adding the Tree to the window");
            DisplayObject(stackPanel);

            GlobalLog.LogStatus("set the values locally for Button and TextPanel");
            button.FontWeight = FontWeights.Bold;
            para.FontWeight = FontWeights.Bold;

            GlobalLog.LogStatus("Changing the Resource on the Application, this resource has the same key but different values from that of the previous one");
            app.Resources = rdHelper.CreateTempFontWeights();

            string fontWeight = "";

            fontWeight = button.FontWeight.ToString();
            CheckResults(fontWeight == "Bold", "Checking the fontweight property of Button after App Resource has changed, we should get locally set values ", "Bold", fontWeight);

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "Bold", "Checking the fontweight property of Paragraph after App Resource has changed,we should get locally set values ", "Bold", fontWeight);

            AppShutDown(app);
        }
        #endregion
    }
}
