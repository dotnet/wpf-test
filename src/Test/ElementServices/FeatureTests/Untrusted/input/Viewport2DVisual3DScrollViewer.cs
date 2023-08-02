// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

using Avalon.Test.CoreUI.Trusted;


namespace Avalon.Test.CoreUI.Hosting
{
    /******************************************************************************
    * CLASS:          Viewport2DVisual3DScrollViewer
    ******************************************************************************/
    // Verify hit testing on elements in a ScrollViewer in a Viewport2DVisual3D.
    // Regression coverage for devdiv 
    [Test(0, "Input.Element3D", TestCaseSecurityLevel.FullTrust, "Viewport2DVisual3DScrollViewer", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\ScrollViewerTransforms.xml", Disabled=true)]
    public class Viewport2DVisual3DScrollViewer : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper s_signalHelper;
        private static bool s_gotClick = false;
        private static NavigationWindow s_navWin;
        private static Button s_targetButton = null;
        private static StackPanel s_targetPanel = null;
        private static Page s_page;

        // These names are specified in the .xml file associated with these tests.
        private static string s_targetButtonName = "scrollViewerButton";
        private static string s_targetPanelName = "vp2d_parent2";
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          Viewport2DVisual3DScrollViewer Constructor
        ******************************************************************************/
        public Viewport2DVisual3DScrollViewer()
        {
            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Reads the content of a stock markup file and inserts per-test requested content from an xml file,
        /// then creates a Page object with that content.
        /// </summary>
        TestResult Initialize()
        {
            string transformName = "ScrollViewerWithButton";

            s_signalHelper = new DispatcherSignalHelper();

            GlobalLog.LogStatus("Starting test: " + transformName);

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            XamlTransformer transformer = new XamlTransformer("ScrollViewerTransforms.xml");
            XmlDocument output = transformer.ApplyTransform(testDoc, transformName);

            s_page = (Page)XamlReader.Load(new XmlNodeReader(output));

            // Set up target button.
            s_targetButton = (Button)s_page.FindName(s_targetButtonName);
            s_targetButton.Click += ButtonClick;

            s_targetPanel = (StackPanel)s_page.FindName(s_targetPanelName);

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            ShowWindow(s_page);

            // Push frame here and wait for Finished signal.
            TestResult finalResult = s_signalHelper.WaitForSignal("Finished");

            s_navWin.Close();

            return finalResult;
        }
        #endregion


        #region Private Members
        /// <summary>
        /// Navigates to the Window defined by the Page content.
        /// </summary>
        /// <param name="pageContent">The content of a Page, created from xml</param>
        private static void ShowWindow(Page pageContent)
        {
            s_navWin = new NavigationWindow();
            s_navWin.Left = 50;
            s_navWin.Top = 50;
            s_navWin.Height = 400;
            s_navWin.Width = 600;
            s_navWin.WindowStyle = WindowStyle.None;
            s_navWin.ContentRendered += new EventHandler(OnContentRendered);

            NameScope.SetNameScope(s_navWin, new NameScope());

            s_navWin.Navigate(pageContent);
            s_navWin.Show();
        }

        /// <summary>
        /// An event handler that is invoked when the page loads. Verification occurs here.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void OnContentRendered(object sender, EventArgs e)
        {
            GlobalLog.LogStatus("----OnContentRendered - Verifying ----");

            TestResult result = TestResult.Fail;

            MouseHelper.Click(MouseButton.Left, s_targetButton, MouseLocation.BottomLeft);

            if (s_gotClick)
            {
                result = TestResult.Pass;
            }
            
            // Report result and continue execution in entry method.
            s_signalHelper.Signal("Finished", result);
        }

        /// <summary>
        /// Mouse click event handler used for verification. If handler
        /// is invoked, test passes.
        /// </summary>
        /// <param name="s">Sender</param>
        /// <param name="r">EventArgs</param>
        private static void ButtonClick(object s, RoutedEventArgs r)
        {
            GlobalLog.LogStatus("Got button click");
            s_gotClick = true;
        }
        #endregion
    }
}










