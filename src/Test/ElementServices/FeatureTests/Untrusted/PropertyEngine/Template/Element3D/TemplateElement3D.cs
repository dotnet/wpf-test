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

using Microsoft.Test.Logging;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Modeling;  //XamlTransformer.
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.PropertyEngine.Template.Element3D
{
    /// <summary>
    /// This test case is designed to test Templates for templated elements inside a Viewport2DVisual3D 
    /// </summary>
    /// <remarks>
    /// Each specific test inserts a ControlTemplate into a stock Markup containing Viewport2DVisual3D elements
    /// </remarks>            
    [Test(0, "PropertyEngine.Template.TemplateInElement3D", TestCaseSecurityLevel.FullTrust, "TemplateElement3D", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\TemplateTransforms.xml")]
    public class TemplateElement3D : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper   s_signalHelper;
        private static NavigationWindow         s_navWin;
        private static Page                     s_page;
        private static string                   s_testName                = "";

        //These names are specified in the .xml file associated with these tests.
        private static string                   s_rootName                = "TestRoot";
        private static string                   s_parentElementName       = "mainViewport3D";
        private static string                   s_templatedButtonName     = "vp2d_nested_child";
        #endregion


        #region Constructor

        [Variation("Template_InRootStyle_Static",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_InRootStyle_Dynamic",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_InRoot_Static",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_InRoot_Dynamic",Versions="3.0SP1,3.0SP2,AH")]
        [Variation("Template_InElement",Versions="3.0SP1,3.0SP2,AH")]

        /******************************************************************************
        * Function:          TemplateElement3D Constructor
        ******************************************************************************/
        public TemplateElement3D(string arg)
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

            XamlTransformer transformer = new XamlTransformer("TemplateTransforms.xml");

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
            GlobalLog.LogStatus("Starting test: " + s_testName);

            ShowWindow(s_page);
            
            TestResult result = s_signalHelper.WaitForSignal("Finished");

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
            TestResult result = TestResult.Fail;
            
            GlobalLog.LogStatus("----OnContentRendered----");
            
            //Find the ControlTemplate, using the names of elements in the loaded Markup.
            ControlTemplate template = TemplateElement3DHelper.FindControlTemplate(s_navWin, s_rootName, s_parentElementName, s_templatedButtonName);

            if (template == null)
            {
                GlobalLog.LogEvidence("OnContentRendered: ----FAIL---- A Template was not found for: " + s_templatedButtonName + ".");
                result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("OnContentRendered: ----PASS---- A Template was found for: " + s_templatedButtonName + ".");
                result = TestResult.Pass;
            }

            s_signalHelper.Signal("Finished", result);
        }
        #endregion
    }
}

