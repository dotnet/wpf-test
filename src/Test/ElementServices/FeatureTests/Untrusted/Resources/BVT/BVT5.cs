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
    /// This test case is designed to test Resource reference on a FrameworkContentElement like Paragraph
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT5" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT5" 
    /// </summary>
    /// <remarks>
    /// Create Tree
    /// StackPanel
    /// -Border
    /// --TextPanel
    /// ---Para
    /// <ol>
    /// <li>Set TextPanel's Resources Property to a Resource Dictionary</li>
    /// <li>Set the FontWeight property of the Paragraph to a value in the Resource</li>
    /// <li>Verify the FontWight Property of the Paragraph is set correctly</li>
    /// <li>Now Directly change the FontWeight property of Paragraph</li>
    /// <li>Verify the FontWight Property</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT5")]
    public class BVT5 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT5 Constructor
        ******************************************************************************/
        public BVT5()
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

            GlobalLog.LogStatus("Setting FlowDocumentScrollViewer.Resources to new resources dictionary");
            textPanel.Resources = resourceDictionayHelper.CreateBrushesFontWeights();

            border.BorderThickness = new Thickness(2);

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            GlobalLog.LogStatus("Setting Paragraph.FontWeightProperty to value in the resource dictionary");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontBold");

            GlobalLog.LogStatus("Building the tree");

            border.Child = fdsv;
            stackPanel.Children.Add(border);

            //para.Append ("hello world or hello universe?").

            string fontWeight = para.FontWeight.ToString();
            ////            CheckResults(fontWeight == "Bold","Checking FontWight Property of Paragraph", "Bold", fontWeight).

            GlobalLog.LogStatus("Setting FontWeight property directly now and see if it works");
            para.FontWeight = FontWeights.ExtraBold;

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "ExtraBold", "Checking FontWight Property of Paragraph", "ExtraBold", fontWeight);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
