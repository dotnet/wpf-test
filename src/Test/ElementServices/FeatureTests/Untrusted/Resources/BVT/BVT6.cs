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
    /// This test case is designed to test FindResources on a FrameworkContenElement(Paragraph) and FrameworkElement(StackPanel, TextPanel)
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT6" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT6" 
    /// </summary>
    /// <remarks>
    /// Create Tree
    /// StackPanel
    /// -Border
    /// --TextPanel
    /// ---Para
    /// <ol>
    /// <li>Set StackPanel's Resources Property to a Resource Dictionary</li>
    /// <li>Set TextPanel's Resources Property to a Resource Dictionary</li>
    /// <li>Set Paragraph's Resource Property to a Resource Dictionary</li>
    /// <li>Verify values obtained by Paragraph.FindResource </li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT6")]
    public class BVT6 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT6 Constructor
        ******************************************************************************/
        public BVT6()
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

            GlobalLog.LogStatus("Creating StackPanel, Border, TextPanel, Paragraph");
            StackPanel stackPanel = new StackPanel();
            Border border = new Border();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();

            GlobalLog.LogStatus("Creating ResourceDictionaryHelper");
            ResourceDictionaryHelper resourceDictionayHelper = new ResourceDictionaryHelper();

            border.BorderThickness = new Thickness(2);

            GlobalLog.LogStatus("Building the tree");

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            border.Child = fdsv;
            stackPanel.Children.Add(border);

            ////            para.Append ("hello world or hello universe?").

            GlobalLog.LogStatus("Setting StackPanel.Resources to new resources dictionary");
            stackPanel.Resources = resourceDictionayHelper.CreateGreetings();

            GlobalLog.LogStatus("Setting FlowDocumentScrollViewer.Resources to new resources dictionary");
            textPanel.Resources = resourceDictionayHelper.CreateBrushesFontWeights();

            GlobalLog.LogStatus("Setting Paragraph.Resources to new resources dictionary");
            para.Resources = resourceDictionayHelper.CreateNumbers();


            string findResourcesValue = para.FindResource("1").ToString();
            CheckResults(findResourcesValue == "1", "Checking para.FindResource(\"1\")", "1", findResourcesValue);

            ////            findResourcesValue = para.FindResource("FontBold").ToString().
            ////            CheckResults( findResourcesValue == "Bold","Checking para.FindResource(\"FontBold\")", "Bold", findResourcesValue).

            ////            findResourcesValue = para.FindResource("Hey").ToString().
            ////            CheckResults( findResourcesValue == "Hey","Checking para.FindResource(\"Hey\")", "Hey", findResourcesValue).

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

