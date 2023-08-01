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
    /// Test Framework Element (Button) take value form the tree when we have resources both in the tree and the application
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT10" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT10" /SecurityLevel="FullTrust"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create Application</li>
    /// <li>Set resources on the Application</li>
    /// <li>Handle the Startup event</li>
    /// <li>Run the Application </li>
    /// <li>In the Startup event handler. </li>
    /// <li>Create an element tree</li>
    /// <li>Set a resource on the stackPanel</li>
    /// <li>Assign the FontWeight and FontStyle property of Button a value in the resource </li>
    /// <li>Verify Button's FontWeight and FontStyle before the tree is added to the Window</li>
    /// <li>Verify Button's FontWeight and FontStyle after the tree is added to the Window</li>
    /// <li>Verify Button's FontWeight and FontStyle after changing the value directly</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT10")]
    public class BVT10 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT10 Constructor
        ******************************************************************************/
        public BVT10()
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

            GlobalLog.LogStatus("Setting Resource on the Application");
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

            CheckResults(app.Resources.Count == 6, "Checking the count in Application resources", "6", app.Resources.Count.ToString());

            ResourceDictionaryHelper rdHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Creating window and other tree Elements");

            StackPanel stackPanel = new StackPanel();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();
            Button button = new Button();

            button.Content = "hello";

            GlobalLog.LogStatus("Setting Resource on the StackPanel, this resource has the same key but different values from the Application Resource");
            stackPanel.Resources = rdHelper.CreateTempFontWeights();

            GlobalLog.LogStatus("SetResourceReference on FontWeightProperty(StackPanel Resource) and FontStyleProperty(App Resource) of Button");
            button.SetResourceReference(Button.FontWeightProperty, "FontMedium");
            button.SetResourceReference(Button.FontStyleProperty, "FontStyleItalic");

            GlobalLog.LogStatus("Creating the element tree");
            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = textPanel.ContentEnd;
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            stackPanel.Children.Add(fdsv);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            DisplayObject(stackPanel);

            string fontWeight = button.FontWeight.ToString();
            CheckResults(fontWeight == "Thin", "Checking the fontweight property of Button after the tree is added to window, should come from the stackPanel resource", "Thin", fontWeight);

            string fontStyle = button.FontStyle.ToString();
            CheckResults(fontStyle == "Italic", "Checking the fontstyle property of Button before the tree is added to window", "Italic", fontStyle);

            button.FontWeight = FontWeights.Bold;
            fontWeight = button.FontWeight.ToString();
            CheckResults(fontWeight == "Bold", "Checking the fontweight property of button after changing it directly", "Bold", fontWeight);

            button.FontStyle = FontStyles.Normal;
            fontStyle = button.FontStyle.ToString();
            CheckResults(fontStyle == "Normal", "Checking the fontstyle property of Button before the tree is added to window", "Normal", fontStyle);

            AppShutDown(app);
        }
        #endregion
    }
}
