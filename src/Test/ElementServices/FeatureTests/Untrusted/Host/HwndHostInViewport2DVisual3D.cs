// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// todo: Remove unused namespaces.
using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;

using Microsoft.Test.Modeling;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    /// Verify HwndHosts do not render in a Viewport2DVisual3D.
    ///</summary>
    /// <remarks>
    /// Regression coverage for devdiv 

    [TestDefaults(SupportFiles = @"FeatureTests\ElementServices\CommonUIElement3DScenario.xaml", DefaultMethodName = "RunTest")]
    public class HwndHostInViewport2DVisual3D
    {
        #region Private Data

        private static DispatcherSignalHelper s_signalHelper;
        private static TestLog s_log;

        private static NavigationWindow s_navWin;
        private static Win32ButtonElement s_host;

        // These names are specified in the .xml file associated with these tests.
        private static string s_immediateParentName = "vp2d_parent1";

        #endregion


        #region Public and Protected Members

        /// <summary>
        /// Verify HwndHosts do not render in a Viewport2DVisual3D.
        /// </summary>
        ///<returns>The final result of the tests</returns>    
        [TestAttribute(0, @"Hosting\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostInViewport2DVisual3D", Area = "AppModel")]
        public static void RunTest()
        {
            CoreLogger.BeginVariation();
            s_signalHelper = new DispatcherSignalHelper();

            s_log = TestLog.Current;

            s_log.LogStatus("Starting test.");

            XmlDocument testDoc = new XmlDocument();
            testDoc.Load("CommonUIElement3DScenario.xaml");

            Page page = (Page)XamlReader.Load(new XmlNodeReader(testDoc));

            // Create extra host in the root 2D tree that should appear - protection against devdiv 
            Win32ButtonElement extraHost = new Win32ButtonElement();
            extraHost.Width = 100;
            extraHost.Height = 100;
            Grid.SetRow(extraHost, 1);

            Panel testGrid = (Panel)page.FindName("testGrid");
            testGrid.Children.Add(extraHost);

            // Create host in Viewport2DVisual3D - should not be built.
            s_host = new Win32ButtonElement();
            s_host.Height = 30;

            Panel viewportPanel = (Panel)page.FindName(s_immediateParentName);
            viewportPanel.Children.Insert(1, s_host);

            ShowWindow(page);

            // Push frame here and wait for Finished signal.
            s_log.Result = s_signalHelper.WaitForSignal("Finished");

            s_navWin.Close();
            CoreLogger.EndVariation();
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
        /// An event handler that is invoked when the page loads. 
        /// Verify BuildWindowCore was called on the HwndHost but that OnRender was not called.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private static void OnContentRendered(object sender, EventArgs e)
        {
            s_log.LogStatus("----OnContentRendered - Verifying ----");
            
            TestResult result = TestResult.Fail;

            s_log.LogStatus("Verifying...");

            // Verify BuildWindowCore was not called.
            if ((s_host.ContainerWindowHandle == IntPtr.Zero) && (s_host.Win32Handle == IntPtr.Zero))
            {
                s_log.LogEvidence("HwndHost BuildWindowCore was not called.");

                result = TestResult.Pass;
            }

            s_signalHelper.Signal("Finished", result);
        }

        #endregion
    }

}










