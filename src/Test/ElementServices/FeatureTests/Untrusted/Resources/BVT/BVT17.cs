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
    /// Test whether the default property is evaluated when the Resource reference on a Framework elemet is made null
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT17" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT17" 
    /// </summary>
    /// <remarks>
    /// Create Tree
    /// StackPanel
    /// -Border
    /// --TextPanel
    /// ---Para
    /// <ol>
    /// <li>Set Resources on the Paragraph</li>
    /// <li>Set the FontWeight property to a value in the Resource</li>
    /// <li>Verify the FontWeight Property of the StackPanel is set correctly</li>
    /// <li>Make the Paragraph Resource null</li>
    /// <li>Verify the FontWeight Property and see that we get the default value</li>
    ///</ol>
    /// </remarks>    
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT17")]
    public class BVT17 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT17 Constructor
        ******************************************************************************/
        public BVT17()
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

            GlobalLog.LogStatus("Creating StackPanel, Border, TextPanel, Paragraph");
            StackPanel stackPanel = new StackPanel();
            Border border = new Border();
            FlowDocumentScrollViewer fdsv = new FlowDocumentScrollViewer();
            FlowDocument textPanel = fdsv.Document = new FlowDocument();

            GlobalLog.LogStatus("Creating ResourceDictionaryHelper");
            ResourceDictionaryHelper resourceDictionayHelper = new ResourceDictionaryHelper();

            border.BorderThickness = new Thickness(2);

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            border.Child = fdsv;
            stackPanel.Children.Add(border);

            GlobalLog.LogStatus("Setting Resource on the Paragraph");
            para.Resources = resourceDictionayHelper.CreateBrushesFontWeights();

            GlobalLog.LogStatus("Setting Paragraph.FontWeightProperty to value in the resource dictionary");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontBold");

            //para.Append ("hello world or hello universe?").

            string fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "Bold", "Checking FontWight Property of Paragraph", "Bold", fontWeight);

            GlobalLog.LogStatus("Setting Paragraph.Resources to null");
            para.Resources = null;

            para.ClearValue(Paragraph.FontWeightProperty);

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "Normal", "Checking FontWight Property of paragraph, should be default", "Normal", fontWeight);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

