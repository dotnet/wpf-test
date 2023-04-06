// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Deployment;
using System.Windows.Automation;
using Microsoft.Test.Loaders.Steps;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// Loader Step that can be used to activate an Avalon application type
    /// </summary>

	public class XamlNavigationHandler1 : Microsoft.Test.Loaders.UIHandler
	{
        /// <summary>
        /// Navigate between two XAML pages across a wide variety of schemes.  Fail if any error UI is shown.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Starting Xaml -> Xaml navigation test ... ");
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");
            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);

            GlobalLog.LogEvidence("Local navigation worked ...");

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToUNC");

            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToUNC");

            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToUNC");


            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");

            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");

            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            GlobalLog.LogEvidence("UNC navigation worked ...");

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPIntra");

            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPIntra");
            
            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");

            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");

            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);
            
            GlobalLog.LogEvidence("HTTP Intranet navigation worked ...");
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPSIntra");
            
            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPSIntra");
            
            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");
            
            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");
            
            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);
            
            GlobalLog.LogEvidence("HTTPS Intranet navigation worked ...");
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPInter");
            
            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPInter");
            
            // Wait til we get to Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);
            
            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToRelative");
            
            // Wait til we get to Page 2
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 30);
            
            GlobalLog.LogEvidence("HTTP Internet navigation worked ...");
            
            GlobalLog.LogEvidence("Xaml -> Xaml navigation test passed ... no exceptions thrown and error page not seen. ");

            if (TestLog.Current != null)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            return UIHandlerAction.Abort;
        }
    }

    public class XamlNavigationHandler2 : Microsoft.Test.Loaders.UIHandler
    {
        /// <summary>
        /// Navigate between two XAML pages across a wide variety of schemes.  Fail if any error UI is shown.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Starting HTML -> Xaml navigation test ... ");
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();

            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(handleWindowNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start((object) IEWindow);

            thread.Join();

            return UIHandlerAction.Abort;
        }

        private static void handleWindowNewThread(object theWindow)
        {
            AutomationElement IEWindow = (AutomationElement)theWindow;

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            try
            {
                IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "Relative XAML");
            }
            catch (System.NullReferenceException)
            {
                GlobalLog.LogEvidence("Whoops, hit UI Automation bug with downlevel platforms + IE (WOSB 1883785 or 1985683, both not likely to ever be fixed) .  Setting result to Ignore and returning, since failure occurred before WPF even loaded.");
                TestLog.Current.Result = TestResult.Ignore;
                return;
            }

            // Wait til we get to XAML Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickIEBackButton(IEWindow);

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "UNC XAML");

            // Wait til we get to XAML Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickIEBackButton(IEWindow);

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "HTTPIntra XAML");

            // Wait til we get to XAML Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPIntraHTML");

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "HTTPSIntra XAML");

            // Wait til we get to XAML Page 1
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPSIntraHTML");

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "HTTPInter XAML");

            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page1", 30);

            IEAutomationHelper.ClickCenterOfElementById(IEWindow, "ToHTTPInterHTML");

            IEAutomationHelper.WaitForElementWithName(IEWindow, "TARGET", 30);

            if (TestLog.Current != null)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }

}
