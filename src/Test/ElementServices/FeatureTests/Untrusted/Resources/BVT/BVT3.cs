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
    /// This Testcase is designed to test FindResource and TryFindResource on a FrameworkElement and FrameworkContentElement.
    /// </summary>
    [Test(0, "Resources.BVT", TestCaseSecurityLevel.FullTrust, "BVT3")]
    public class BVT3 : TestHelper
    {
        #region Constructor
        /******************************************************************************
        * Function:          BVT3 Constructor
        ******************************************************************************/
        public BVT3()
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
            GlobalLog.LogStatus("Creating a StackPanel and set Height and width");
            StackPanel stackPanel = new StackPanel();
            stackPanel.Width = 100.00;
            stackPanel.Height = 100.00;

            GlobalLog.LogStatus("Creating ResourceDictionaryHelper");
            ResourceDictionaryHelper resourceDictionayHelper = new ResourceDictionaryHelper();

            GlobalLog.LogStatus("Adding Border as a child of StackPanel");
            Border border = new Border();
            stackPanel.Children.Add(border);

            GlobalLog.LogStatus("Setting StackPanel.Resources to new ResourceDictionay");
            stackPanel.Resources = resourceDictionayHelper.CreateSolidColorBrushes();

            GlobalLog.LogStatus("Setting Border.Resources to new ResourceDictionay");
            border.Resources = resourceDictionayHelper.CreateNumbers();

            string brushColor = ((SolidColorBrush)stackPanel.FindResource("BrushRed")).Color.ToString();
            CheckResults(brushColor == "#FFFF0000", "Checking stackPanel.FindResource(\"BrushRed\")", "#FFFF0000", brushColor);

            string number = border.FindResource("1").ToString();
            CheckResults(number == "1", "Checking border.FindResource (\"1\")", "1", number);

            brushColor = ((SolidColorBrush)border.FindResource("BrushRed")).Color.ToString();
            CheckResults(brushColor == "#FFFF0000", "Checking border.FindResource(\"BrushRed\")", "#FFFF0000", brushColor);

            //
            // Test FindResource and TryFindResource on a FrameworkElement.
            //
            GlobalLog.LogStatus("Setting a DockPanel element to Border.Child");
            DockPanel dockPanel = new DockPanel();
            border.Child = dockPanel;

            VerifyResourceLookupOnChild(dockPanel);

            //
            // Test FindResource and TryFindResource on a FrameworkContentElement.
            //
            GlobalLog.LogStatus("Setting a Bold in a Button element to Border.Child");
            Button button = new Button();
            Bold bold = new Bold();
            button.Content = bold;
            border.Child = null;
            border.Child = button;

            VerifyResourceLookupOnChild(bold);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          VerifyResourceLookupOnChild
        ******************************************************************************/
        private void VerifyResourceLookupOnChild(object obj)
        {
            //object res = null;

            // FindResource
            string brushColor = ((SolidColorBrush)FindResource(obj, "BrushRed", false, false)).Color.ToString();
            CheckResults(brushColor == "#FFFF0000", "Checking " + obj.GetType().Name + ".FindResource(\"BrushRed\")", "#FFFF0000", brushColor);

            string number = FindResource(obj, "1", false, false).ToString();
            CheckResults(number == "1", "Checking " + obj.GetType().Name + ".FindResource (\"1\")", "1", number);

            // TryFindResource
            brushColor = ((SolidColorBrush)FindResource(obj, "BrushRed", true, false)).Color.ToString();
            CheckResults(brushColor == "#FFFF0000", "Checking " + obj.GetType().Name + ".FindResource(\"BrushRed\")", "#FFFF0000", brushColor);

            number = FindResource(obj, "1", true, false).ToString();
            CheckResults(number == "1", "Checking " + obj.GetType().Name + ".FindResource (\"1\")", "1", number);

            // Invalid key to FindResource
            FindResource(obj, "INVALIDRESOURCEKEY", false, true);

            // Invalid key to TryFindResource
            FindResource(obj, "INVALIDRESOURCEKEY", true, false);
        }

        /******************************************************************************
        * Function:          FindResource
        ******************************************************************************/
        private object FindResource(object obj, string key, bool tryFind, bool expectException)
        {
            FrameworkElement fe = obj as FrameworkElement;
            FrameworkContentElement fce = obj as FrameworkContentElement;
            object res = null;
            Exception caughtException = null;

            try
            {
                if (fe != null)
                {
                    if (tryFind) res = fe.TryFindResource(key);
                    else res = fe.FindResource(key);
                }
                else
                {
                    if (tryFind) res = fce.TryFindResource(key);
                    else res = fce.FindResource(key);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            CheckResults(expectException == (caughtException != null), "Checking if exception occurred, and if it was expected", "Expected:" + expectException.ToString(), "Actual:" + (caughtException != null).ToString());

            return res;
        }
        #endregion
    }
}
