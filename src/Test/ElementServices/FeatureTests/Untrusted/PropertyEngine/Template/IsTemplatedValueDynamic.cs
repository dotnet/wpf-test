// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Common;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Modeling;  //XamlTransformer.
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Win32;
using Microsoft.Test.Utilities;
using System.Xml;

namespace Avalon.Test.CoreUI.Properties
{
    /// <summary>
    /// Tests for DependencyPropertyHelper.IsTemplatedValueDynamic
    ///
    /// In .NET 4.7.1, we introduced the IsTemplatedValueDynamic method on
    /// DependencyPropertyHelper to enable Visual Studio, or similar diagnostic tools,
    /// to distinguish how properties were set. This builds on the information that
    /// DependencyPropertyHelper.GetValueSource() returns.
    /// There are several cases that cause GetValueSource to return ParentTemplate.
    /// </summary>
    [Test(1, "Properties", TestCaseSecurityLevel.FullTrust, "IsTemplatedValueDynamicTests", Versions="4.7.1", SupportFiles = @"FeatureTests\ElementServices\IsTemplatedValueDynamic.xaml")]
    public class IsTemplatedValueDynamicTests : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper s_signalHelper;
        private static Window s_window;
        private static string s_testName = "";
        #endregion


        #region Constructor

        /******************************************************************************
        * Function:          IsTemplatedValueDynamicTests Constructor
        ******************************************************************************/
        public IsTemplatedValueDynamicTests()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        TestResult Initialize()
        {
            s_signalHelper = new DispatcherSignalHelper();

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("IsTemplatedValueDynamic.xaml");

            s_window = (Window)System.Windows.Markup.XamlReader.Load(new XmlNodeReader(testDoc));

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Tests DependencyPropertyHelper.IsTemplatedValueDynamic
        /// </summary>
        TestResult StartTest()
        {
            GlobalLog.LogStatus("Starting test: " + s_testName);

            s_window.ContentRendered += OnContentRendered;
            s_window.Show();

            TestResult result = s_signalHelper.WaitForSignal("Finished");

            s_window.Close();

            return result;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          OnContentRendered
        ******************************************************************************/
        /// <summary>
        /// An event handler that is invoked when the page loads. Verification occurs here.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void OnContentRendered(object sender, EventArgs e)
        {
            TestResult result = TestResult.Fail;

            GlobalLog.LogStatus("----OnContentRendered----");

            DependencyProperty tbBackgroundProperty = TextBlock.BackgroundProperty;
            Control ct1 = (Control)s_window.FindName("ct1");
            ContentPresenter dt1 = (ContentPresenter)s_window.FindName("dt1");
            TextBlock tbOuter = (TextBlock)s_window.FindName("tbOuter");

            TestResult tr1 = VerifyValuesForElementsInControlTemplates(ct1, tbBackgroundProperty);
            TestResult tr2 = VerifyValuesForElementsInDataTemplates(dt1, tbBackgroundProperty);
            TestResult tr3 = VerifyArgumentExceptionForElementsNotInATemplate(tbOuter, tbBackgroundProperty);

            if (tr1 == TestResult.Pass && tr2 == TestResult.Pass && tr3 == TestResult.Pass)
            {
                result = TestResult.Pass;
            }

            s_signalHelper.Signal("Finished", result);
        }

        private static TestResult VerifyValuesForElementsInDataTemplates(ContentPresenter dt1, DependencyProperty tbBackgroundProperty)
        {
            TestResult result = TestResult.Fail;

            TestResult tr1 = VerifyValueForElementInDataTemplate(dt1, "tbLocalValue", tbBackgroundProperty, false);
            TestResult tr2 = VerifyValueForElementInDataTemplate(dt1, "tbStaticResource", tbBackgroundProperty, false);
            TestResult tr3 = VerifyValueForElementInDataTemplate(dt1, "tbDynamicResource", tbBackgroundProperty, true);
            TestResult tr4 = VerifyValueForElementInDataTemplate(dt1, "tbBinding", tbBackgroundProperty, true);
            TestResult tr5 = VerifyValueForElementInDataTemplate(dt1, "tbDynamicResourceTriggerSimple", tbBackgroundProperty, true);
            TestResult tr6 = VerifyValueForElementInDataTemplate(dt1, "tbSimpleTriggerDynamicResource", tbBackgroundProperty, false);
            TestResult tr7 = VerifyValueForElementInDataTemplate(dt1, "tbStyleViaBinding", tbBackgroundProperty, false);
            TestResult tr8 = VerifyValueForElementInDataTemplate(dt1, "tbStyleViaDynamicResource", tbBackgroundProperty, false);

            if (tr1 == TestResult.Pass && tr2 == TestResult.Pass && tr3 == TestResult.Pass &&
                tr4 == TestResult.Pass && tr5 == TestResult.Pass && tr6 == TestResult.Pass &&
                tr7 == TestResult.Pass && tr8 == TestResult.Pass)
            {
                result = TestResult.Pass;
            }

            return result;
        }

        private static TestResult VerifyValuesForElementsInControlTemplates(Control ct1, DependencyProperty tbBackgroundProperty)
        {
            TestResult result = TestResult.Fail;

            TestResult tr1 = VerifyValueForElementInControlTemplate(ct1, "tbLocalValue", tbBackgroundProperty, false);
            TestResult tr2 = VerifyValueForElementInControlTemplate(ct1, "tbStaticResource", tbBackgroundProperty, false);
            TestResult tr3 = VerifyValueForElementInControlTemplate(ct1, "tbDynamicResource", tbBackgroundProperty, true);
            TestResult tr4 = VerifyValueForElementInControlTemplate(ct1, "tbTemplateBinding", tbBackgroundProperty, true);
            TestResult tr5 = VerifyValueForElementInControlTemplate(ct1, "tbBinding", tbBackgroundProperty, true);
            TestResult tr6 = VerifyValueForElementInControlTemplate(ct1, "tbDynamicResourceTriggerSimple", tbBackgroundProperty, true);
            TestResult tr7 = VerifyValueForElementInControlTemplate(ct1, "tbSimpleTriggerDynamicResource", tbBackgroundProperty, false);
            TestResult tr8 = VerifyValueForElementInControlTemplate(ct1, "tbStyleViaBinding", tbBackgroundProperty, false);
            TestResult tr9 = VerifyValueForElementInControlTemplate(ct1, "tbStyleViaDynamicResource", tbBackgroundProperty, false);

            if (tr1 == TestResult.Pass && tr2 == TestResult.Pass && tr3 == TestResult.Pass &&
               tr4 == TestResult.Pass && tr5 == TestResult.Pass && tr6 == TestResult.Pass &&
               tr7 == TestResult.Pass && tr8 == TestResult.Pass && tr9 == TestResult.Pass)
            {
                result = TestResult.Pass;
            }

            return result;
        }

        private static TestResult VerifyArgumentExceptionForElementsNotInATemplate(DependencyObject dObj, DependencyProperty dp)
        {
            bool argumentExceptionRaised = false;
            try
            {
                bool isTemplatedValueDynamic = DependencyPropertyHelper.IsTemplatedValueDynamic(dObj, TextBlock.BackgroundProperty);
            }
            catch (ArgumentException)
            {
                argumentExceptionRaised = true;
            }

            if (!argumentExceptionRaised)
            {
                GlobalLog.LogEvidence(String.Format("----FAIL {0}.{1}:  ArgumentException was not raised, but should have been.",
                    dObj.GetValue(FrameworkElement.NameProperty), dp.Name));
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence(String.Format("----PASS {0}.{1}:  ArgumentException was raised, but should have been.",
                    dObj.GetValue(FrameworkElement.NameProperty), dp.Name));
                return TestResult.Pass;
            }
        }

        private static TestResult VerifyValueForElementInControlTemplate(Control templatedElement, string childName, DependencyProperty dp, bool expectedValue)
        {
            var template = templatedElement.Template;
            DependencyObject dObj = (DependencyObject)template.FindName(childName, templatedElement);
            return VerifyIsTemplatedValueDynamic(dObj, dp, expectedValue);
        }

        private static TestResult VerifyValueForElementInDataTemplate(ContentPresenter templatedElement, string childName, DependencyProperty dp, bool expectedValue)
        {
            var template = templatedElement.ContentTemplate;
            DependencyObject dObj = (DependencyObject)template.FindName(childName, templatedElement);
            return VerifyIsTemplatedValueDynamic(dObj, dp, expectedValue);
        }

        private static TestResult VerifyIsTemplatedValueDynamic(DependencyObject dObj, DependencyProperty dp, bool expectedValue)
        {
            BaseValueSource baseValueSource = DependencyPropertyHelper.GetValueSource(dObj, dp).BaseValueSource;
            bool isTemplatedValueDynamic = DependencyPropertyHelper.IsTemplatedValueDynamic(dObj, dp);

            if (isTemplatedValueDynamic != expectedValue)
            {
                GlobalLog.LogEvidence(String.Format("----FAIL {0}.{1}:  IsTemplatedValueDynamic was {2}, expected {3} (BaseValueSource={4})",
                    dObj.GetValue(FrameworkElement.NameProperty), dp.Name, isTemplatedValueDynamic, expectedValue, baseValueSource));
                return TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence(String.Format("----PASS {0}.{1}:  IsTemplatedValueDynamic was {2}, expected {3} (BaseValueSource={4})",
                    dObj.GetValue(FrameworkElement.NameProperty), dp.Name, isTemplatedValueDynamic, expectedValue, baseValueSource));
                return TestResult.Pass;
            }
        }
        #endregion
    }
}

