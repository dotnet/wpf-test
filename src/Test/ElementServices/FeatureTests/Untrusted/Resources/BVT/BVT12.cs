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
    /// Test FrameworkContent Element's (Here Paragraph) and Application's FindResouce method, when there are resources in the tree and the application
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT12" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT12" /SecurityLevel="FullTrust"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create Application</li>
    /// <li>Set resources on the Application</li>
    /// <li>Handle the Startup event</li>
    /// <li>Run the Application </li>
    /// <li>In the Startup event handler. </li>
    /// <li>Create an element tree</li>
    /// <li>Set a resource on the stackPanel, Paragraph</li>
    /// <li>Verify values obtained by Para.FindResource </li>
    /// </ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT12")]
    public class BVT12 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT12 Constructor
        ******************************************************************************/
        public BVT12()
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

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            GlobalLog.LogStatus("Setting Resource on Paragraph");
            para.Resources = rdHelper.CreateTempFontWeights();

            GlobalLog.LogStatus("Creating the element tree");

            stackPanel.Children.Add(fdsv);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            DisplayObject(stackPanel);

            string fontWeight; ////, fontStyle, brushColor;
            GlobalLog.LogStatus("Testing FindResources method of Paragraph");

            fontWeight = para.FindResource("FontMedium").ToString();
            GlobalLog.LogStatus("Get fontWeight as " + fontWeight);

            CheckResults(fontWeight == "Thin", "Checking the value obtained by FindResource method of Paragraph", "Thin", fontWeight);

            ////            brushColor = ((SolidColorBrush)para.FindResource("BrushRed")).Color.ToString ().
            ////            CheckResults(brushColor == "#FFFF0000","Checking the value obtained by FindResource method of Paragraph", "#FFFF0000", brushColor).

            ////            fontStyle =  para.FindResource("FontStyleItalic").ToString().
            ////            CheckResults(fontStyle == "Italic","Checking the value obtained by FindResource method of Paragraph", "Italic", fontStyle).

            fontWeight = app.FindResource("FontBold").ToString();
            CheckResults(fontWeight == "Bold", "Checking the value obtained by FindResource method of Application", "Bold", fontWeight);

            AppShutDown(app);
        }
        #endregion
    }
}
