// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshWithStyleDynamicResourceReferenceTest
{
    /******************************************************************************
    * CLASS:          WithStyleDynamicResourceReference
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "WithStyleDynamicResourceReference")]
    public class WithStyleDynamicResourceReference : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("NegativeTestStyleSetResourceReference")]
        [Variation("PositiveTestStyleSetResourceReference")]
        [Variation("NegativeTestFEFSetResourceReference")]
        [Variation("PositiveTestFEFSetResourceReference")]

        /******************************************************************************
        * Function:          WithStyleDynamicResourceReference Constructor
        ******************************************************************************/
        public WithStyleDynamicResourceReference(string arg)
        {
            _testName = arg;
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
            TestWithStyleDynamicResourceReference test = new TestWithStyleDynamicResourceReference();

            Utilities.StartRunAllTests("WithStyleDynamicResourceReference");

            switch (_testName)
            {
                case "NegativeTestStyleSetResourceReference":
                    test.NegativeTestStyleSetResourceReference();
                    break;
                case "PositiveTestStyleSetResourceReference":
                    test.PositiveTestStyleSetResourceReference();
                    break;
                case "NegativeTestFEFSetResourceReference":
                    test.NegativeTestFEFSetResourceReference();
                    break;
                case "PositiveTestFEFSetResourceReference":
                    test.PositiveTestFEFSetResourceReference();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            Utilities.StopRunAllTests();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }



    /******************************************************************************
    * CLASS:          TestWithStyleDynamicResourceReference
    ******************************************************************************/
    ///<summary>
    ///Allow resource references on DPs in styles to be dynamic 
    ///(currently they're calculated by the parser).  
    ///I.e. put something liike SetResourceReference on FEF. 
    ///Important API: Style.SetResourceReference And FrmeworkElementFactory.SetResourceReference
    ///</summary>
    public class TestWithStyleDynamicResourceReference
    {
        /******************************************************************************
        * Function:          NegativeTestStyleSetResourceReference
        ******************************************************************************/
        /// <summary>
        /// Negative test case using Style.SetResourceReference
        /// </summary>
        public void NegativeTestStyleSetResourceReference()
        {
            Utilities.PrintTitle("Negative test case using Style.SetResourceReference");
            Utilities.PrintStatus("When DependencyProperty is null");

            Style testStyle = new Style();

            try
            {
                testStyle.Setters.Add(new Setter(null, new DynamicResourceExtension("ABC")));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("When Style is already sealed");

            Button testButton = new Button();

            testButton.Style = testStyle;  //testStyle is sealed as the result of this call
            try
            {
                testStyle.Setters.Add(new Setter(Button.ContentProperty, new DynamicResourceExtension("ABC")));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /******************************************************************************
        * Function:          PositiveTestStyleSetResourceReference
        ******************************************************************************/
        /// <summary>
        /// Positive Test Case Using Style.SetResourceReference
        /// </summary>
        public void PositiveTestStyleSetResourceReference()
        {
            Utilities.PrintTitle("Positive Test Case Using Style.SetResourceReference");

            Style testStyle = new Style();

            testStyle.Setters.Add(new Setter(Button.ContentProperty, new DynamicResourceExtension("ABC")));
            Button testButton = new Button();

            Utilities.Assert(testButton.Content == null, "Without Resource, content is null");

            ResourceDictionary rd = new ResourceDictionary();

            rd.BeginInit();
            rd.Add("ABC", "Hello World");
            rd.EndInit();
            testButton.Resources = rd;
            
            testButton.Content = "TEST";
            Utilities.Assert((string)testButton.Content == "TEST", "After Assignment, Content has a different value");
            testButton.Resources = rd;
            Utilities.Assert((string)testButton.Content == "TEST", "Now Resource Reference Lookup is no longer invoked.");

        }

        /******************************************************************************
        * Function:          NegativeTestFEFSetResourceReference
        ******************************************************************************/
        /// <summary>
        /// Negative test cases with FrameworkElementFactory.SetResourceReference
        /// </summary>
        public void NegativeTestFEFSetResourceReference()
        {
            Utilities.PrintTitle("Negative test cases with FrameworkElementFactory.SetResourceReference");

            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(DockPanel));


            try
            {
                chrome.SetResourceReference(null, "AAA");
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Style testStyle = new Style();
            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = chrome;
            testStyle.Setters.Add(new Setter(Button.TemplateProperty, template));


            Button testButton = new Button();

            testButton.Style = testStyle;

            try
            {
                chrome.SetResourceReference(DockPanel.BackgroundProperty, "AAA");
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        /******************************************************************************
        * Function:          PositiveTestFEFSetResourceReference
        ******************************************************************************/
        /// <summary>
        /// Positive test cases with FrameworkElementFactory.SetResourceReference
        /// Change Note: Text Property in TextBox cannot be Styled (

        public void PositiveTestFEFSetResourceReference()
        {
            Utilities.PrintTitle("Positive test cases with FrameworkElementFactory.SetResourceReference");


            Style testStyle = new Style();


            FrameworkElementFactory chrome = new FrameworkElementFactory(typeof(DockPanel));
            FrameworkElementFactory content = new FrameworkElementFactory(typeof(TextBox));

            chrome.SetResourceReference(DockPanel.BackgroundProperty, "AAA");
            content.SetResourceReference(TextBox.BackgroundProperty, "BBB");
            chrome.AppendChild(content);

            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = chrome;
            testStyle.Setters.Add(new Setter(Button.TemplateProperty, template));

            Button testButton = new Button();

            testButton.Style = testStyle;
            testButton.ApplyTemplate(); //Fault in VisualTree

            DockPanel dp = (DockPanel)VisualTreeHelper.GetChild(testButton,0);

            TextBox tb = (TextBox)VisualTreeHelper.GetChild(dp,0);

            Utilities.PrintStatus("Now Add Resources");

            ResourceDictionary rd = new ResourceDictionary();

            rd.Add("AAA", Brushes.Blue);
            rd.Add("BBB", Brushes.Green);
            testButton.Resources = rd;
            Utilities.Assert(dp.Background == Brushes.Blue, "DockPanel.Backfround is Brushes.Blue");
            Utilities.Assert(tb.Background == Brushes.Green, "TextBox.Background is Brushes.Green");
        }
    }
}

