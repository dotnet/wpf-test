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
    /// This test case is designed to test Application resources, FrameworkContent element(Paragraph here) is tested for fallback to Application Resources
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT8" /Method="RunTest" /SecurityLevel="FullTrust"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT8" /SecurityLevel="FullTrust"
    /// </summary>
    /// <remarks>
    /// <ol>
    /// <li>Create Application</li>
    /// <li>Set resources on the Application</li>
    /// <li>Handle the Startup event</li>
    /// <li>Run the Application </li>
    /// <li>In the Startup event handler. </li>
    /// <li>Create an element tree</li>
    /// <li>Assign the FontWeight property of Paragraph a value in the resource </li>
    /// <li>Verify Paragraph's FontWeight before the tree is added to the Window</li>
    /// <li>Verify Paragraph's FontWeight after the tree is added to the Window</li>
    /// <li>Verify Paragraph's FontWeight after changing the value directly</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT8")]
    public class BVT8 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT8 Constructor
        ******************************************************************************/
        public BVT8()
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
            app.Resources = resourceDictionayHelper.CreateBrushesFontWeights();
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

            CheckResults(app.Resources.Count == 8, "Checking the count in Application resources", "8", app.Resources.Count.ToString());

            GlobalLog.LogStatus("Creating window and other tree Elements");

            StackPanel stackPanel = new StackPanel();
            Border border = new Border();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();
            Button button = new Button();

            button.Content = "hello";

            border.BorderThickness = new Thickness(2);
            border.SetResourceReference(Border.BorderBrushProperty, "BrushGreen");

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            GlobalLog.LogStatus("SetResourceReference on FontWeightProperty of textPanel");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontBold");

            GlobalLog.LogStatus("Creating the element tree");

            border.Child = fdsv;
            stackPanel.Children.Add(border);
            stackPanel.Children.Add(button);

            //para.Append ("hello world or hello universe?").

            DisplayObject(stackPanel);

            string fontWeight;
            ////            string fontWeight = para.FontWeight.ToString ().
            ////            CheckResults(fontWeight == "Bold","Checking the fontweight property of Paragraph after the tree is added to window", "Bold", fontWeight).

            para.FontWeight = FontWeights.Thin;

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "Thin", "Checking the fontweight property of Paragraph after changing it directly", "Thin", fontWeight);

            AppShutDown(app);
        }
        #endregion
    }
}
