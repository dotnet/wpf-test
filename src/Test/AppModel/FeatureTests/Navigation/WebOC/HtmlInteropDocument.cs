// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.Document
    /// This runs in full trust only.  There is a case in HtmlInteropSecurity that verifies it doesn't work in partial trust.
    /// </summary>
    public class HtmlInteropDocument
    {

        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;
        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop Document tests");
            }

            //Local machine lockdown disables scripting inside IE.  Re-enable it for testing purposes.
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 0, RegistryValueKind.DWord);

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri("HtmlInterop_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_browser != null)
            {
                return;
            }
            else
            {
                _browser = new WebBrowser();
                if (e.Navigator is Frame)
                {
                    Log.Current.CurrentVariation.LogMessage("Frame Navigation to " + e.Uri);
                    (e.Navigator as Frame).Content = _stackPanel;
                }
                else if (e.Navigator is NavigationWindow)
                {
                    Log.Current.CurrentVariation.LogMessage("NavWin Navigation to " + e.Uri);
                    (e.Navigator as NavigationWindow).Content = _stackPanel;
                }

                _stackPanel.Children.Add(_browser);

                _browser.LoadCompleted += OnHtmlLoadCompleted;

                //try Document before there's an HTMLDocument
                bool caught = false;
                object document = null;
                try
                {
                    document = _browser.Document;
                }
                catch (Exception exception)
                {
                    NavigationHelper.Output("browser.Document with no HTMLDocument got unexpected exception: " + exception.ToString());
                    _oneOrMoreTestsFailed = true;
                    caught = true;
                }
                if (!caught)
                {
                    if(document == null)
                    {
                        NavigationHelper.Output("Document return value was null as expected.");
                    }
                    else
                    {
                        NavigationHelper.Output("Document type didn't match.  Expected: null , Actual: " + document.GetType().ToString());
                    }
                }    

                _browser.ObjectForScripting = _testObject;

                NavigationHelper.Output("Navigating to test html page");
                _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute));
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Fail("Unexpected exception caught. Test fails");
            e.Handled = true;
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }
        #endregion

        #region Private Members
        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            bool caught = false;
            NavigationHelper.Output("LoadCompleted event fired");
            IHTMLDocument2 document = null;
            string returnValue = "";

            try
            {
                document = _browser.Document as IHTMLDocument2;
                returnValue = document.readyState;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("browser.Document got unexpected exception: " + exception.ToString());
                _oneOrMoreTestsFailed = true;
                caught = true;
            }
            if (!caught)
            {
                if (returnValue == "complete")
                {
                    NavigationHelper.Output("Getting browser.Document worked: " + returnValue);
                }
                else
                {
                    NavigationHelper.Output("Document.readyState didn't match.  Expected: complete , Actual: " + returnValue);
                }
            } 

            //put local machine lockdown script setting back to disable
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

            if (_oneOrMoreTestsFailed)
            {
                NavigationHelper.Fail("One or more HTML interop Document tests failed");
            }
            else
            {
                NavigationHelper.Pass("HTML interop Document tests passed");
            }
        }
        #endregion

        #region mshtml interfaces

        [ComVisible(true), ComImport(), Guid("25336920-03F9-11CF-8FD0-00AA00686F13")]
        internal class HTMLDocument
        {

        }
        
        [ComImport, Guid("626FC520-A41E-11CF-A731-00A0C9082637"), TypeLibType((short) 0x1040)]
        internal interface IHTMLDocument
        {
            [DispId(0x3e9)]
            object Script1 { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
        }

        [ComImport, TypeLibType((short) 0x1040), Guid("332C4425-26CB-11D0-B483-00C04FD90119")]
        internal interface IHTMLDocument2 : IHTMLDocument
        {
            //Most of these are just placeholders since order and position matter.
            //The code only calls readyState.
            [DispId(0x3e9)]
            object Script { get; }
            [DispId(0x3eb)]
            object all { get; }
            [DispId(0x3ec)]
            object body { get; }
            [DispId(0x3ed)]
            object activeElement { get; }
            [DispId(0x3f3)]
            object images { get; }
            [DispId(0x3f0)]
            object applets { get; }
            [DispId(0x3f1)]
            object links { get; }
            [DispId(0x3f2)]
            object forms { get; }
            [DispId(0x3ef)]
            object anchors {  get; }
            [DispId(0x3f4)]
            string title { get; set; }
            [DispId(0x3f5)]
            object scripts { get; }
            [DispId(0x3f6)]
            string designMode { get; set; }
            [DispId(0x3f9)]
            object selection { get; }
            [DispId(0x3fa)]
            string readyState { [return: MarshalAs(UnmanagedType.BStr)] get; }

            //...skipping the rest for brevity
        }

        #endregion

    }
}
