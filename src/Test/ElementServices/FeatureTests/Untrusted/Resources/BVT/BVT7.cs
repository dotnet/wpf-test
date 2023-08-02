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
    /// This test case is designed to test Invalidation of  ResourceReference
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT7" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT7" 
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
    /// <li>verify values in sub tree</li>
    /// <li>Change the Resource Property of the StackPanel to a new value </li>
    /// <li>Change the Resource Property of the Paragraph to a new value </li>
    ///<li>verify values</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT7")]
    public class BVT7 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT7 Constructor
        ******************************************************************************/
        public BVT7()
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

            GlobalLog.LogStatus("Setting StackPanel.Resources to new resources dictionary");
            stackPanel.Resources = resourceDictionayHelper.CreateBrushesFontWeights();
            border.BorderThickness = new Thickness(2);

            Paragraph paragraph = new Paragraph();
            textPanel.Blocks.Add(paragraph);

            TextPointer pointer;
            pointer = (textPanel.ContentEnd);
            pointer = pointer.GetNextContextPosition(LogicalDirection.Backward);
            Paragraph para = (Paragraph)pointer.Parent;

            border.Child = fdsv;
            stackPanel.Children.Add(border);

            GlobalLog.LogStatus("Setting Border.BorderBrushProperty to value in the resource dictionary");
            border.SetResourceReference(Border.BorderBrushProperty, "BrushGreen");

            GlobalLog.LogStatus("Setting Paragraph.FontWeightProperty to value in the resource dictionary");
            para.SetResourceReference(Paragraph.FontWeightProperty, "FontBold");

            //para.Append ("hello world or hello universe?").


            string fontWeight = para.FontWeight.ToString();
            ////            CheckResults(fontWeight == "Bold","Checking FontWight Property of Paragraph", "Bold", fontWeight).

            string brushColor = ((SolidColorBrush)border.BorderBrush).Color.ToString();
            CheckResults(brushColor == "#FF008000", "Checking BorderBrush Property of Border", "#FF008000", brushColor);


            GlobalLog.LogStatus("Creating new temp resources dictionary");
            ResourceDictionary tempResourceDictionary1 = new ResourceDictionary();
            tempResourceDictionary1.Add("BrushGreen", Brushes.LightGreen);
            tempResourceDictionary1.Add("FontBold", FontWeights.ExtraBold);

            ResourceDictionary tempResourceDictionary2 = new ResourceDictionary();
            tempResourceDictionary2.Add("BrushGreen", Brushes.LightGreen);
            tempResourceDictionary2.Add("FontBold", FontWeights.ExtraBold);

            GlobalLog.LogStatus("Setting StackPanel.Resources to new temp resources dictionary");
            stackPanel.Resources = tempResourceDictionary1;

            brushColor = ((SolidColorBrush)border.BorderBrush).Color.ToString();
            CheckResults(brushColor == "#FF90EE90", "Checking BorderBrush Property of Border", "#FF90EE90", brushColor);

            GlobalLog.LogStatus("Setting Paragraph.Resources to new resources dictionary");
            para.Resources = tempResourceDictionary2;

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "ExtraBold", "Checking FontWight Property of paragraph", "ExtraBold", fontWeight);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}
