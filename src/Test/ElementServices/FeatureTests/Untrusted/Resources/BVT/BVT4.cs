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
    /// This test case is designed to test setResource reference on a FrameworkContentElement like Paragraph
    /// 
    /// To run this test:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT4" /Method="RunTest"
    /// Because TestCaseInfo.Method has default value of "RunTest", command can be simplified as:
    /// coretests.exe /Class="Avalon.Test.Resources.BVT4" 
    /// </summary>
    /// <remarks>
    /// Create Tree
    /// StackPanel
    /// -Border
    /// --TextPanel
    /// ---Para
    /// <ol>
    /// <li>Set StackPanel's Resources Property to a Resource Dictionary</li>
    /// <li>Set the FontWeight property of the Paragraph to a value in the Resource</li>
    /// <li>Verify the FontWight Property of the Paragraph is set correctly</li>
    /// <li>Now Directly change the FontWeight property of Paragraph</li>
    /// <li>Verify the FontWight Property</li>
    ///</ol>
    /// </remarks>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT4")]
    public class BVT4 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT4 Constructor
        ******************************************************************************/
        public BVT4()
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

            GlobalLog.LogStatus("Setting stackPanel.Resources to new resources dictionary");
            stackPanel.Resources = resourceDictionayHelper.CreateBrushesFontWeights();

            GlobalLog.LogStatus("Setting Border.BorderBrushProperty to value in the resource dictionary");
            border.BorderThickness = new Thickness(2);
            border.SetResourceReference(Border.BorderBrushProperty, "BrushGreen");

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

            ////para.Append ("hello world or hello universe?").

            string fontWeight = para.FontWeight.ToString();
            ////            CheckResults(fontWeight == "Bold","Checking FontWight Property of Paragraph", "Bold", fontWeight).

            GlobalLog.LogStatus("Setting FontWeight property directly now and see if it works");
            para.FontWeight = FontWeights.ExtraBold;

            fontWeight = para.FontWeight.ToString();
            CheckResults(fontWeight == "ExtraBold", "Checking FontWight Property of Paragraph", "ExtraBold", fontWeight);

            // MergedDictionaries Verification

            // Create.
            ResourceDictionary rd = new ResourceDictionary();
            ResourceDictionary rd1 = new ResourceDictionary();
            ResourceDictionary rd2 = new ResourceDictionary();
            ResourceDictionary rd3 = new ResourceDictionary();
            ResourceDictionary rd4 = new ResourceDictionary();
            

            rd1.Add("rd1Resource", Brushes.Green);
            rd2.Add("rd2Resource", Brushes.Green);
            rd3.Add("rd3Resource", Brushes.Green);
            rd4.Add("rd4Resource", Brushes.Green);
            

            // Add.
            rd.MergedDictionaries.Add(rd1);
            rd.MergedDictionaries.Add(rd2);
            rd.MergedDictionaries.Add(rd3);
            rd.MergedDictionaries.Add(rd4);
            

            // Verify Count.
            if (rd.MergedDictionaries.Count != 4)            
            {
                throw new Microsoft.Test.TestValidationException("MergedDictionary Count did not match - Expected: 4 GOT :" + rd.MergedDictionaries.Count);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary Count verified");
            }

            // Clear.
            rd.MergedDictionaries.Clear();

            // Verify Clear Count.
            if (rd.MergedDictionaries.Count != 0)
            {
                throw new Microsoft.Test.TestValidationException("MergedDictionary CLEAR Count did not match - Expected: 0 GOT :" + rd.MergedDictionaries.Count);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary CLEAR Count verified");
            }

            // Iterate.
            rd.MergedDictionaries.Add(rd1);
            rd.MergedDictionaries.Add(rd2);
            rd.MergedDictionaries.Add(rd3);
            rd.MergedDictionaries.Add(rd4);

            IEnumerator myEnumerator = rd.MergedDictionaries.GetEnumerator();
            int counter = 0;
            while (myEnumerator.MoveNext())
            {
                if (myEnumerator.Current != null)
                {
                    counter++;
                }
            }
            if (counter != 4)
            {
                throw new Microsoft.Test.TestValidationException("MergedDictionary Count did not match - Expected: 4 GOT :" + counter);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary Iteration verified");
            }


            // Verify Lookup.
            GlobalLog.LogStatus("Validating MergedDictionary lookup");

            Button testTarget = new Button();
            testTarget.Resources = rd;

            testTarget.SetResourceReference(Button.BackgroundProperty, "rd4Resource");

            string actual = testTarget.Background.ToString();
            string expected = Brushes.Green.ToString();

            if (actual != expected)
            {
                throw new Microsoft.Test.TestValidationException("Backgrounds did not match - Expected: " + expected + " GOT :" + actual);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary lookup verified");
            }

            //
            // Verifying MergedDictionaries.Remove().
            //
            GlobalLog.LogStatus("Validating MergedDictionary Remove");

            // Remove by index.
            rd.MergedDictionaries.Remove(rd.MergedDictionaries[3]);

            if (rd.MergedDictionaries.Count != 3)
            {
                throw new Microsoft.Test.TestValidationException("Index Remove did not work as expected.");
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary index remove verified");
            }

            // Verify reinserting at removed index
            ResourceDictionary oldIndex = new ResourceDictionary();
            oldIndex.Add("oldIndexResource", Brushes.Red);
            rd.MergedDictionaries.Add(oldIndex);

            testTarget.SetResourceReference(Button.BackgroundProperty, "oldIndexResource");

            actual = testTarget.Background.ToString();
            expected = Brushes.Red.ToString();

            if (actual != expected)
            {
                throw new Microsoft.Test.TestValidationException("Backgrounds did not match after adding removed by index resource - Expected: " + expected + " GOT :" + actual);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary add after remove verified");
            }

            // Remove by resource.
            rd.MergedDictionaries.Remove(rd1);

            if (rd.MergedDictionaries.Count != 3)
            {
                throw new Microsoft.Test.TestValidationException("Object Remove did not work as expected.");
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary object remove verified");
            }

            // Verify inserting resource with removed resource name.
            rd.MergedDictionaries.Add(rd1);


            testTarget.SetResourceReference(Button.BackgroundProperty, "rd1Resource");

            actual = testTarget.Background.ToString();
            expected = Brushes.Green.ToString();

            if (actual != expected)
            {
                throw new Microsoft.Test.TestValidationException("Backgrounds did not match after adding removed by index resource - Expected: " + expected + " GOT :" + actual);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary add after remove verified");
            }

            // Remove multi-nested merged dictionaries
            ResourceDictionary rdNested = new ResourceDictionary();

            // Create.            
            ResourceDictionary rdN1 = new ResourceDictionary();
            ResourceDictionary rdN2 = new ResourceDictionary();

            rdN1.Add("rdN1Resource", Brushes.Blue);
            rdN2.Add("rdN2Resource", Brushes.Blue);
            

            // Add.
            rdNested.MergedDictionaries.Add(rdN1);
            rdNested.MergedDictionaries.Add(rdN2);

            // Verify inserting nested merged dictionaries
            rd.MergedDictionaries.Add(rdNested);
            rd.MergedDictionaries[4].MergedDictionaries.Remove(rdN1);

            Button bNested = new Button();
            bNested.Resources = rd;

            bNested.SetResourceReference(Button.BackgroundProperty, "rdN1Resource");
            string exceptionMessage = "";

            try
            {
                actual = bNested.Background.ToString();
            }
            catch (Exception e)
            {
                exceptionMessage = e.Message;
            }

            if (exceptionMessage == "")
            {
                throw new Microsoft.Test.TestValidationException("Nested Resource not removed");
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary nested mergeddictionary remove verified");
            }

            // Add the resource again and verify behaviour.
            rd.MergedDictionaries[4].MergedDictionaries.Add(rdN1);


            testTarget.SetResourceReference(Button.BackgroundProperty, "rdN1Resource");

            actual = testTarget.Background.ToString();
            expected = Brushes.Blue.ToString();

            if (actual != expected)
            {
                throw new Microsoft.Test.TestValidationException("Backgrounds did not match after adding removed by nested resource - Expected: " + expected + " GOT :" + actual);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary add after remove nested verified");
            }

            // Remove from application resources.
            // Create new Application            
            Application app = new Application();

            // Launch the Application.            
            app.Startup += new StartupEventHandler(app_Startup);
            app.Run();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          app_Startup
        ******************************************************************************/
        private void app_Startup(object sender, StartupEventArgs e)
        {
            Application app;
            app = (Application)sender;

            ResourceDictionary rdApp = new ResourceDictionary();

            ResourceDictionary rdA1 = new ResourceDictionary();
            ResourceDictionary rdA2 = new ResourceDictionary();

            rdA1.Add("rdA1Resource", Brushes.Yellow);
            rdA2.Add("rdA1Resource", Brushes.Orange);

            rdApp.MergedDictionaries.Add(rdA1);
            

            app.Resources = rdApp;

            // Remove app resource & verify.
            app.Resources.MergedDictionaries.Remove(app.Resources.MergedDictionaries[0]);
            

            Button bApp = new Button();
            string actual = "";
            string exceptionMessage = "";

            bApp.SetResourceReference(Button.BackgroundProperty, "rdA1Resource");           

            try
            {
                actual = bApp.Background.ToString();
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            if (exceptionMessage == "")
            {
                throw new Microsoft.Test.TestValidationException("App Merged Resource not removed. GOT: " + actual.ToString() + " EXPECTED: " + null);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary nested mergeddictionary remove verified");
            }

            // Add app resource & verify.
            app.Resources.MergedDictionaries.Add(rdA1);
            bApp.SetResourceReference(Button.BackgroundProperty, "rdA1Resource");


            string actual1 = bApp.Background.ToString();
            string expected1 = Brushes.Yellow.ToString();

            if (actual1 != expected1)
            {
                throw new Microsoft.Test.TestValidationException("Backgrounds did not match after adding removed by nested resource - Expected: " + expected1 + " GOT :" + actual1);
            }
            else
            {
                GlobalLog.LogStatus("MergedDictionary add after remove app verified");
            }
            
            // Shutdown App.
            AppShutDown(app);
        }
        #endregion
    }
}
