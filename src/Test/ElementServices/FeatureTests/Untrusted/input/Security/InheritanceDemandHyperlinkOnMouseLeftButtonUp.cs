// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives
using System;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Helper;
using Avalon.Test.CoreUI.PropertyEngine;
using System.Windows.Documents;
using System.Security;
#endregion

namespace Avalon.Test.CoreUI.CoreInput.Security.TestInheritanceDemandHyperlinkOnMouseLeftButtonUp
{
    /// <summary>
    /// Use the command below to run this test with CoreTests.exe:
    /// coretests.exe /Class=Avalon.Test.CoreUI.CoreInput.Security.TestInheritanceDemandHyperlinkOnMouseLeftButtonUp.PETestCase  /Method=LabRunAll 
    /// coretests.exe /Class=Avalon.Test.CoreUI.CoreInput.Security.TestInheritanceDemandHyperlinkOnMouseLeftButtonUp.PETestCase  /Method=LabRunAllUntrusted 
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class PETestCase : TestCase
    {
        /// <summary>
        /// Used by the Test Engine
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\InheritanceDemandHyperlinkOnMouseLeftButtonUp")]
        [TestCaseMethod("LabRunAll")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        [TestCaseTimeout("180")]
        [TestCaseDisabled("0")]
        public void LabRunAll()
        {
            Utilities.PrintTitle("Inherit OnMouseLeftButtonUp. (Note: Partial trust code also calls this.)");
            TestInheritanceDemandHyperlinkOnMouseLeftButtonUp test = new TestInheritanceDemandHyperlinkOnMouseLeftButtonUp();

            Utilities.StartRunAllTests("InheritanceDemandHyperlinkOnMouseLeftButtonUp");
            test.RunTest();
            Utilities.StopRunAllTests();
        }

        /// <summary>
        /// Used by the Test Engine
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"PropertyEngine\InheritanceDemandHyperlinkOnMouseLeftButtonUp")]
        [TestCaseMethod("LabRunAllUntrusted")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.PartialTrust)]
        [TestCaseTimeout("180")]
        [TestCaseDisabled("0")]
        public void LabRunAllUntrusted()
        {
            Utilities.PrintTitle("Inherit OnMouseLeftButtonUp in Partial Trust. Get SecurityException.");
            try
            {
                LabRunAll();
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (SecurityException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }
    }

    /// <summary></summary>
    public class TestInheritanceDemandHyperlinkOnMouseLeftButtonUp
    {
        /// <summary>
        /// Create an instance of TestHyperlink and call its OnMouseLeftButtonUp
        /// </summary>
        public void RunTest()
        {
            TestHyperlink testHyperlink = new TestHyperlink();
            testHyperlink.TestIt();
        }
    }

    /// <summary>
    /// This class derives from Hyperlink and overrides OnMouseLeftButtonUp.
    /// Because of InheritanceDemand from Hyperlink.OnMouseLeftButtonUp on Unrestricted UIPermission,
    /// test will get SecurityException in partial trust
    /// </summary>
    public class TestHyperlink : Hyperlink
    {
        /// <summary>
        /// Override OnMouseLeftButtonUp()
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            Utilities.PrintStatus("TestHyperlink.OnMouseLeftButtonUp is called.");
        }

        /// <summary>
        /// Just call the override
        /// </summary>
        public void TestIt()
        {
            this.OnMouseLeftButtonUp(null);
        }
    }
}

