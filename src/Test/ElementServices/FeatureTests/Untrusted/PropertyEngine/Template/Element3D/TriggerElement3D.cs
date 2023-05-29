// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Modeling;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.PropertyEngine.Template.Element3D
{
    /// <summary>
    /// This test case is designed to test Triggers with Templates for templated elements inside a Viewport2DVisual3D 
    /// Verification consists of checking the property value of the property changed by the Trigger.
    /// </summary>
    /// <remarks>
    /// Each specific test inserts a ControlTemplate into a stock Markup containing Viewport2DVisual3D elements
    /// </remarks>            
    [Test(0, "PropertyEngine.Template.TemplateInElement3D", TestCaseSecurityLevel.FullTrust, "TriggerElement3D", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\TriggerTransforms.xml")]
    public class TriggerElement3D : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper   s_signalHelper;
        private static NavigationWindow         s_navWin;
        private static Page                     s_page;
        private static bool                     s_triggerElementFound     = true;
        private static DependencyProperty       s_dependencyProp;
        private static double                   s_expectedValue           = 450d;
        private static string                   s_testName                = "";

        //These names are specified in the .xml file associated with these tests.
        private static string                   s_rootName                = "TestRoot";
        private static string                   s_parentElementName       = "mainViewport3D";
        private static string                   s_immediateParentName     = "vp2d_parent1";
        private static string                   s_templateElementName     = "";
        private static string                   s_templatedElementName    = "vp2d_second_child";
        #endregion
        
        #region Constructor

        [Variation("Template_Style_NoResource_EventTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_StaticStyle_EventTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_DynamicStyle_EventTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_StaticStyle_Trigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_DynamicStyle_Trigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_Parent_DynamicStyle_DataTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_Parent_StaticStyle_DataTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_Style_DataTrigger",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_DataTrigger_InElement",Versions="3.0SP1,3.0SP2,AH")]

        /******************************************************************************
        * Function:          TriggerElement3D Constructor
        ******************************************************************************/
        public TriggerElement3D(string arg)
        {
            s_testName = arg;
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(ExecuteTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Reads the content of a stock markup file and inserts per-test requested content from an xml file,
        /// then later creates a Page object with that content.
        /// </summary>
        TestResult Initialize()
        {
            s_signalHelper = new DispatcherSignalHelper();

            switch(s_testName)
            {
                case "Template_Style_NoResource_EventTrigger" :
                    s_templateElementName     = "ellipse1";
                    s_dependencyProp          = FrameworkElement.WidthProperty;
                    s_expectedValue           = 450d;
                    break;
                case "Template_StaticStyle_EventTrigger" :
                    s_templateElementName     = "ellipse1";
                    s_dependencyProp          = FrameworkElement.WidthProperty;
                    s_expectedValue           = 450d;
                    break;
                case "Template_DynamicStyle_EventTrigger" :
                    s_templateElementName     = "ellipse1";
                    s_dependencyProp          = FrameworkElement.HeightProperty;
                    s_expectedValue           = 0d;
                    break;
                case "Template_StaticStyle_Trigger" :
                    s_templateElementName     = "border1";
                    s_dependencyProp          = UIElement.OpacityProperty;
                    s_expectedValue           = 0d;
                    break;
                case "Template_DynamicStyle_Trigger" :
                    s_templateElementName     = "border1";
                    s_dependencyProp          = UIElement.OpacityProperty;
                    s_expectedValue           = 0.4d;
                    break;
                case "Template_Parent_DynamicStyle_DataTrigger" :
                    s_templateElementName     = "border1";
                    s_dependencyProp          = FrameworkElement.HeightProperty;
                    s_expectedValue           = 1000d;
                    break;
                case "Template_Parent_StaticStyle_DataTrigger" :
                    s_templateElementName     = "border1";
                    s_dependencyProp          = FrameworkElement.HeightProperty;
                    s_expectedValue           = 1000d;
                    break;
                case "Template_Style_DataTrigger" :
                    s_templateElementName     = "rectangle1";
                    s_dependencyProp          = UIElement.OpacityProperty;
                    s_expectedValue           = 0.5d;
                    break;
                case "Template_DataTrigger_InElement" :
                    s_templateElementName     = "textbox1";
                    s_dependencyProp          = UIElement.OpacityProperty;
                    s_expectedValue           = 0.25d;
                    break;
            }

            GlobalLog.LogStatus("Starting test: " + s_testName);

            XamlTransformer transformer = new XamlTransformer("TriggerTransforms.xml");

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            XmlDocument output = transformer.ApplyTransform(testDoc, s_testName);

            s_page = (Page)XamlReader.Load(new XmlNodeReader(output));

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          ExecuteTest
        ******************************************************************************/
        TestResult ExecuteTest()
        {
            // The test name is the name of the sub-test, which is also the location of the section of xml to be inserted into the xaml.
            GlobalLog.LogStatus("Starting test: " + s_testName);

            ShowWindow(s_page);

            TestResult result = s_signalHelper.WaitForSignal("Finished");

            //"Restore" mouse position to 0,0, to prevent a current mouse move affecting later cases.
            MouseHelper.MoveOnPrimaryMonitor();

            s_navWin.Close();

            return result;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          ShowWindow
        ******************************************************************************/
        /// <summary>
        /// Navigates to the Window defined by the Page content.
        /// </summary>
        /// <param name="pageContent">The content of a Page, created from xml</param>
        private static void ShowWindow(Page pageContent)
        {
            s_navWin = new NavigationWindow();
            s_navWin.Left                 = 50;
            s_navWin.Top                  = 50;
            s_navWin.Height               = 400;
            s_navWin.Width                = 600;
            s_navWin.WindowStyle          = WindowStyle.None;
            s_navWin.ContentRendered     += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(s_navWin, new NameScope());

            s_navWin.Navigate(pageContent);
            s_navWin.Show();
        }                

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
            GlobalLog.LogStatus("----OnContentRendered----");
            InvokeTrigger();

            DispatcherHelper.DoEvents(1500);

            VerifyTest();
        }
        
        /******************************************************************************
        * Function:          VerifyTest
        ******************************************************************************/
        /// <summary>
        /// Verify the effect of the Trigger and pass/fail the test case.
        /// </summary>
        /// <returns></returns>              
        private static void VerifyTest()          
        {
            bool result = false;
            TestResult testPassed = TestResult.Fail;

            if (s_triggerElementFound)
            {
                ControlTemplate template = TemplateElement3DHelper.FindControlTemplate(s_navWin, s_rootName, s_parentElementName, s_templatedElementName);

                if (template == null)
                {
                    GlobalLog.LogEvidence("OnDispatcherTick: The ControlTemplate was not found for: " + s_templatedElementName + ".");

                    result = false;
                }
                else
                {
                    GlobalLog.LogEvidence("OnDispatcherTick: The ControlTemplate was found for: " + s_templatedElementName + ".");

                    result = TemplateElement3DHelper.VerifyPropertyValue(s_dependencyProp, s_expectedValue, template, s_navWin, s_rootName, s_parentElementName, s_templatedElementName, s_templateElementName);
                }

                if (result)
                {
                    testPassed = TestResult.Pass;
                }
                else
                {
                    testPassed = TestResult.Fail;
                }
            }

            s_signalHelper.Signal("Finished", testPassed);
        }

        /******************************************************************************
        * Function:          InvokeTrigger
        ******************************************************************************/
        /// <summary>
        /// Employ UIAutomation to move the mouse over the templated Element.
        /// </summary>
        private static void InvokeTrigger()
        {
            DependencyObject parentElement = (DependencyObject)LogicalTreeHelper.FindLogicalNode((DependencyObject)s_navWin.Content, s_parentElementName);
            if (parentElement == null)
            {
                GlobalLog.LogEvidence("InvokeTrigger:  A Parent (" + s_parentElementName + ") was not found.");
                s_triggerElementFound = false;
            }
            else
            {
                FrameworkElement immediateParent = (FrameworkElement)VisualTreeUtils.FindElement(s_immediateParentName, parentElement);

                if (immediateParent == null)
                {
                    GlobalLog.LogEvidence("InvokeTrigger:  The immediate Parent (" + s_immediateParentName + ") was not found.");
                    s_triggerElementFound = false;
                }
                else
                {
                    MouseHelper.Move(immediateParent);
                }
            }
        }
        #endregion
    }
}

