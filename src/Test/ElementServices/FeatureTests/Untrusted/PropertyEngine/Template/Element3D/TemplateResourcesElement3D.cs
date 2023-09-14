// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
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
    /// This test case is designed to test Templates containing a Resource, for templated elements inside a Viewport2DVisual3D 
    /// <relatedBugs>
    /// <





    [Test(0, "PropertyEngine.Template.TemplateInElement3D", TestCaseSecurityLevel.FullTrust, "TemplateResourcesElement3D", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\TemplateResourceTransforms.xml")]
    public class TemplateResourcesElement3D : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper   s_signalHelper;
        private static NavigationWindow         s_navWin;
        private static Page                     s_page;
        private static string                   s_testName                = "";

        //These names are specified in the .xml file associated with these tests.
        private static string                   s_rootName                = "TestRoot";
        private static string                   s_parentElementName       = "mainViewport3D";
        private static string                   s_templateElementName     = "border1";
        private static string                   s_templatedElementName    = "vp2d_second_child";
        private static DependencyProperty       s_dependencyProp;
        private static double                   s_expectedValue;
        #endregion
        
        #region Constructor

        [Variation("InViewport3D_Static_ContainingStyle",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InViewport3D_Dynamic_ContainingStyle",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_ResourcesInElement",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_StyleDynamicToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_StyleStaticToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_StyleDynamicToSibling",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_StyleDynamic",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_StyleStatic",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InViewport3D_Dynamic_StyleDynamicToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InViewport3D_Dynamic_StyleStatic",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InViewport3D_Static_StyleDynamicToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InViewport3D_Static_StyleStaticToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InParent_Dynamic_StyleStaticToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InParent_Dynamic_StyleDynamicToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InParent_Static_StyleStaticToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InParent_Static_StyleDynamicToParent",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Static_ValueConflictingWithParentStyle",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Dynamic_ValueConflictingWithParentStyle",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Static_ValueConflictingWithParentStyle2",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Dynamic_ValueConflictingWithParentStyle2",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Static_StaticStyle_ConflictingWithParentStyle",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("InElement_Static_DynamicStyle_ConflictingWithParentStyle",Versions="3.0SP1,3.0SP2,AH")]

        /******************************************************************************
        * Function:          TemplateResourcesElement3D Constructor
        ******************************************************************************/
        public TemplateResourcesElement3D(string arg)
        {
            s_testName = arg;
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

            switch(s_testName)
            {
                case "InViewport3D_Dynamic_ContainingStyle" :
                    s_dependencyProp = FrameworkElement.OpacityProperty;
                    s_expectedValue = 0.5d;
                    break;
                case "InElement_Static_DynamicStyle_ConflictingWithParentStyle" :
                    s_dependencyProp = FrameworkElement.HeightProperty;
                    s_expectedValue = 1d;
                    break;
                default:
                    s_dependencyProp = FrameworkElement.WidthProperty;
                    s_expectedValue = 450d;
                    break;
            }

            XamlTransformer transformer = new XamlTransformer("TemplateResourceTransforms.xml");

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            XmlDocument output = transformer.ApplyTransform(testDoc, s_testName);

            s_page = (Page)XamlReader.Load(new XmlNodeReader(output));

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Reads the content of a stock markup file and inserts per-test requested content from an xml file,
        /// then creates a Page object with that content.
        /// </summary>
        TestResult StartTest()
        {
            // The test name is the name of the sub-test, which is also the location of the section of xml to be inserted into the xaml.
            GlobalLog.LogStatus("Starting test: " + s_testName);

            ShowWindow(s_page);

            TestResult result = s_signalHelper.WaitForSignal("Finished");

            return result;
        }
        #endregion


        #region Public and Protected Members
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
            TestResult result = TestResult.Fail;
            
            GlobalLog.LogStatus("----OnContentRendered----");

            //Find the ControlTemplate, using the names of elements in the loaded Markup.
            ControlTemplate template = TemplateElement3DHelper.FindControlTemplate(s_navWin, s_rootName, s_parentElementName, s_templatedElementName);

            if (template == null)
            {
                GlobalLog.LogEvidence("OnContentRendered:  The ControlTemplate was not found.");
                result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogStatus("OnContentRendered:  The ControlTemplate was found.");

                result = VerifyResource(template);
            }

            s_signalHelper.Signal("Finished", result);

            s_navWin.Close();
        }

        /******************************************************************************
        * Function:          VerifyResource
        ******************************************************************************/
        /// <summary>
        /// Verifies the results of the test case.  What is checked depends on the nature of the test case.
        /// </summary>
        /// <param name="template">The ControlTemplate that is involved in the test result verification.</param>
        /// <returns>A TestResult, Pass or Fail</returns>
        private static TestResult VerifyResource(ControlTemplate template)
        {
            TestResult finalResult  = TestResult.Fail;
            bool result1             = false;
            bool result2             = false;

            switch (s_testName)
            {
                case "InViewport3D_Static_ContainingStyle":
                    result1 = TemplateElement3DHelper.VerifyResourceDictionary(template, true);
                    break;
                case "InViewport3D_Dynamic_ContainingStyle":
                    result1 = TemplateElement3DHelper.VerifyResourceDictionary(template, true);
                    break;
                case "InElement_ResourcesInElement":
                    result1 = TemplateElement3DHelper.VerifyResourceDictionary(template, true);
                    break;
                default:
                    result1 = TemplateElement3DHelper.VerifyResourceDictionary(template, false);
                    break;
            }

            //Always verify the property value.
            result2 = TemplateElement3DHelper.VerifyPropertyValue(s_dependencyProp, s_expectedValue, template, s_navWin, s_rootName, s_parentElementName, s_templatedElementName, s_templateElementName);

            if (result1 && result2)
            {
                finalResult = TestResult.Fail;

            }
            else
            {
                finalResult = TestResult.Pass;
            }

            return finalResult;
        }
        #endregion
    }
}

