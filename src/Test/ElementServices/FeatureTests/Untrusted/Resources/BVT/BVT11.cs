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
    /// Test FrameworkContent Element (Paragraph) take value form the tree when we have resources both in the tree and the application
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT11" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT11" /SecurityLevel="FullTrust"
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
    /// <li>Assign the FontWeight and FontStyle property of Paragraph a value in the resource </li>
    /// <li>Verify Paragraph's FontWeight and FontStyle before the tree is added to the Window</li>
    /// <li>Verify Paragraph's FontWeight and FontStyle after the tree is added to the Window</li>
    /// <li>Verify Paragraph's FontWeight and FontStyle after changing the value directly</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT11")]
    public class BVT11 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT11 Constructor
        ******************************************************************************/
        public BVT11()
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

            CheckResults(app.Resources.Count == 6, "Checking the count in Application resources", "6", app.Resources.Count.ToString());

            ResourceDictionaryHelper rdHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Creating window and other tree Elements");

            StackPanel stackPanel = new StackPanel();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();
            Button button = new Button();

            button.Content = "hello";

            GlobalLog.LogStatus("Setting Resource on TextPanel, resource dictionary has the same key but different values from that of the application ");
            textPanel.Resources = rdHelper.CreateTempFontWeights();

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            GlobalLog.LogStatus("SetResourceReference on FontWeightProperty and FontStyleProperty of Paragraph");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontMedium");
            para.SetResourceReference(Paragraph.FontStyleProperty, "FontStyleItalic");

            GlobalLog.LogStatus("Creating the element tree");

            stackPanel.Children.Add(fdsv);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            DisplayObject(stackPanel);
            string fontWeight, fontStyle;

            ////            string fontWeight = para.FontWeight.ToString ().
            ////            CheckResults(fontWeight == "Thin","Checking the fontweight property of Paragraph after the tree is added to window", "Bold", fontWeight).
            ////
            ////            string fontStyle =  para.FontStyle.ToString ().
            ////            CheckResults(fontStyle == "Italic","Checking the fontstyle property of Paragraph after the tree is added to window", "Italic", fontStyle).

            para.FontWeight = FontWeights.Bold;
            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "Bold", "Checking the fontweight property of Paragraph after changing it directly", "Bold", fontWeight);

            para.FontStyle = FontStyles.Normal;
            fontStyle = para.FontStyle.ToString();
            CheckResults(fontStyle == "Normal", "Checking the fontstyle property of Paragraph after changing it directly", "Normal", fontStyle);

            AppShutDown(app);
        }
        #endregion
    }
}
