// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
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
    * CLASS:          UIElement3DPopupServices
    ******************************************************************************/
    ///<summary>
    /// Verify popup services work on UIElement3D. 
    ///</summary>
    ///<remarks>
    /// Regression coverage for devdiv 

    [Test(0, "Input.Element3D", TestCaseSecurityLevel.FullTrust, "UIElement3DPopupServices", SupportFiles=@"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml,FeatureTests\ElementServices\PopupServicesTransforms.xml")]
    public class UIElement3DPopupServices : AvalonTest
    {
        #region Private Data
        private static DispatcherSignalHelper s_signalHelper;
        private static NavigationWindow s_navWin = null;
        private static UIElement3D s_targetModel = null;
        private static bool s_popupOpened = false;
        // This name is specified in the .xml file associated with these tests.
        private static string s_targetModelElementName = "simpleModelUIElement3D";
        private string _transformName = "";
        #endregion


        #region Constructor

        [Variation("ToolTip_ModelUIElement3D", Versions="3.0SP1,3.0SP2,AH")]
        [Variation("ContextMenu_ModelUIElement3D", Versions="3.0SP1,3.0SP2,AH")]

        /******************************************************************************
        * Function:          UIElement3DPopupServices Constructor
        ******************************************************************************/
        public UIElement3DPopupServices(string arg)
        {
            _transformName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            s_signalHelper = new DispatcherSignalHelper();

            GlobalLog.LogStatus("Starting test: " + _transformName);

            // Load template xaml, transform xml, and apply transform.
            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            XamlTransformer transformer = new XamlTransformer("PopupServicesTransforms.xml");

            XmlDocument output = transformer.ApplyTransform(testDoc, _transformName);

            Page page = (Page)XamlReader.Load(new XmlNodeReader(output));

            // Attach event handler to target element's tooltip to verify tooltip was opened.
            s_targetModel = (UIElement3D)page.FindName(s_targetModelElementName);
            switch (_transformName)
            {
                case "ToolTip_ModelUIElement3D":
                    ToolTip tt = (ToolTip)ToolTipService.GetToolTip(s_targetModel);
                    tt.Opened += HandlePopupOpened;
                    break;

                case "ContextMenu_ModelUIElement3D":
                    ContextMenu cm = ContextMenuService.GetContextMenu(s_targetModel);
                    cm.Opened += HandlePopupOpened;
                    break;
            }

            // Create window and start test. 
            ShowWindow(page);

            // Push frame here and wait for Finished signal.
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
            GlobalLog.LogStatus("----OnContentRendered - Verifying ----");

            TestResult result = TestResult.Fail;

            GlobalLog.LogStatus("Verifying...");

            MouseHelper.Move(s_targetModel);

            // Wait for ToolTip to appear.
            DispatcherHelper.DoEvents(1000);

            // Open context menu.
            MouseHelper.Click(MouseButton.Right);

            // Wait for context menu to appear.
            DispatcherHelper.DoEvents(1000);

            if (s_popupOpened)
            {
                result = TestResult.Pass;
            }

            // Report result and continue execution in entry method.
            s_signalHelper.Signal("Finished", result);
        }

        /******************************************************************************
        * Function:          HandlePopupOpened
        ******************************************************************************/
        /// <summary>
        /// ToolTip opened event handler used for verification. If handler
        /// is invoked, test passes.
        /// </summary>
        /// <param name="s">Sender</param>
        /// <param name="r">EventArgs</param>
        private static void HandlePopupOpened(object s, RoutedEventArgs r)
        {
            s_popupOpened = true;
        }
        #endregion
    }

}










