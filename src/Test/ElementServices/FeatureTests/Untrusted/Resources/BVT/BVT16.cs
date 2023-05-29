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
    /// Testing Invalidation on the Application when Application Resource is set to null
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT16" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT16" /SecurityLevel="FullTrust"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create Application</li>
    /// <li>Set resources on the Application</li>
    /// <li>Handle the Startup event</li>
    /// <li>Run the Application </li>
    /// <li>In the Startup event handler. </li>
    /// <li>Create an element tree</li>
    /// <li>Set a resource on the stackPanel, button, TextPanel</li>
    /// <li>Verify values </li>
    /// <li>Change the Resource on the Application </li>
    /// <li>Verify the new values</li>
    /// </ol>
    /// </remarks>        
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT16")]
    public class BVT16 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT16 Constructor
        ******************************************************************************/
        public BVT16()
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
            FlowDocumentScrollViewer textPanel = new FlowDocumentScrollViewer();
            textPanel.Document = new FlowDocument();
            Button button = new Button();

            button.Content = "hello";

            GlobalLog.LogStatus("Setting Resource on StackPanel");
            stackPanel.Resources = rdHelper.CreateSolidColorBrushes();

            GlobalLog.LogStatus("SetResourceReference on Button");
            button.SetResourceReference(Button.FontWeightProperty, "FontBold");

            GlobalLog.LogStatus("SetResourceReference on TextPanel");
            textPanel.SetResourceReference(TextElement.FontWeightProperty, "FontThin");

            Paragraph paragraph = new Paragraph();
            textPanel.Document.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.Document.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            stackPanel.Children.Add(textPanel);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            DisplayObject(stackPanel);

            GlobalLog.LogStatus("Changing the Resource on the Application,this resource has the same key but different values from that of the previous one");
            button.ClearValue(Button.FontWeightProperty);
            paragraph.ClearValue(Button.FontWeightProperty);
            textPanel.ClearValue(TextElement.FontWeightProperty);
            app.Resources = null;

            string fontWeight = button.FontWeight.ToString();

            CheckResults(fontWeight == "Normal", "Checking the fontweight property of Button after App Resource has changed", "Normal", fontWeight);
            //fontWeight = paragraph.FontWeight.ToString();
            //CheckResults(fontWeight == "Normal", "Checking the fontweight property of FlowDocumentScrollViewer after App Resource has changed", "Normal", fontWeight);

            AppShutDown(app);
        }
        #endregion
    }
}
