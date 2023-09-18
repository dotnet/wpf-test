// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression Tests.

namespace Test.Uis.Regressions
{
    #region Namespaces.
    using System;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Shapes;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Imaging;
    using Microsoft.Test;
using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the regression bugs doesnt repro.
    /// Test1: Check that you can set KeyboardNavigation.AcceptsReturn to true from Xaml    
    /// Test2: Check that you can set KeyboardNavigation.AcceptsReturn to false from Xaml    
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("469"), TestBugs("454, 455")]
    [Test(2, "TextBox", "XamlRegressions", MethodParameters = "/TestCaseType:XamlRegressions")]
    public class XamlRegressions : CustomTestCase
    {
        const string inputXaml1 = "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            "<RichTextBox Name='TestTP' Focusable='True' Width='200' Height='400' " +
            "KeyboardNavigation.AcceptsReturn='True' " +
            ">" +
            "<FlowDocument Background='LightYellow'>" + 
            "<Paragraph>This is an editable textPanel</Paragraph>" +
            "</FlowDocument></RichTextBox></DockPanel>";
        const string inputXaml2 = "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            "<RichTextBox Name='TestTP' Focusable='True' Width='200' Height='400' " +
            "KeyboardNavigation.AcceptsReturn='False' " +
            "><FlowDocument Background='LightYellow'>" +
            "<Paragraph>This is an editable textPanel</Paragraph>" +
            "</FlowDocument></RichTextBox></DockPanel>";

        UIElement _testUIElement;
        UIElementWrapper _testWrapper;
        string _expectedString1 = "This is an editable \r\n";
        string _expectedString2 = "textPanel\r\n";
        const string expectedString1_1 = "This is an editable \r\n";
        const string expectedString1_2 = "This is an editable ";

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Test1();
        }

        private void Test1()
        {
            ActionItemWrapper.SetMainXaml(inputXaml1);
            _expectedString1 = expectedString1_1;                       
            StartTest();
        }

        private void StartTest()
        {
            _testUIElement = ElementUtils.FindElement(MainWindow, "TestTP") as UIElement;
            _testWrapper = new UIElementWrapper(_testUIElement);
            QueueDelegate(DoInputAction);
        }

        private void DoInputAction()
        {
            MouseInput.MouseClick(_testUIElement);
            KeyboardInput.TypeString("{END}{LEFT 9}{ENTER}");            
            QueueDelegate(VerifyTextPanelProperties);
        }

        private void VerifyTextPanelProperties()
        {
            Log("Text to the left: [" + _testWrapper.GetTextOutsideSelection(LogicalDirection.Backward) + "]");
            Log("Text to the right: [" + _testWrapper.GetTextOutsideSelection(LogicalDirection.Forward) + "]");
            Log("Exp Text to the left: [" + _expectedString1 + "]");
            Log("Exp Text to the right: [" + _expectedString2 + "]");
            
            Verifier.Verify(_testWrapper.GetTextOutsideSelection(LogicalDirection.Backward) == _expectedString1,
                "Verifying Backward string after hitting error", true);
            Verifier.Verify(_testWrapper.GetTextOutsideSelection(LogicalDirection.Forward).Contains(_expectedString2),
                "Verifying Forward string after hitting error. Actual[" + _testWrapper.GetTextOutsideSelection(LogicalDirection.Forward) + "]", true);

            if (_expectedString1 == expectedString1_1)
            {
                Log("Test1 ran fine");
                Test2();
            }
            else if (_expectedString1 == expectedString1_2)
            {
                Log("Test2 ran fine");
                Logger.Current.ReportSuccess();
            }
        }

        private void Test2()
        {
            ActionItemWrapper.SetMainXaml(inputXaml2);
            _expectedString1 = expectedString1_2;            
            StartTest();
        }
    }
}
