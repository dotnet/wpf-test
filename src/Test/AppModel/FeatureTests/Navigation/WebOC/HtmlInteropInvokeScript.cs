// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Microsoft.Test.Logging;
using Microsoft.Test.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.InvokeScript.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropInvokeScript
    {

        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;
        const string resultFor32Bit =         "undefinednumbernumbernumberunknownbooleannumbernumbernumbernumbernumbernumbernumberunknownunknownstringnumber";
        const string resultFor64Bit =         "undefinednumbernumbernumberunknownbooleannumbernumbernumbernumbernumbernumbernumbernumbernumberstringnumber";
        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop InvokeScript tests");
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

                //try InvokeScript before there's an HTMLDocument
                bool caught = false;
                try
                {
                    _browser.InvokeScript("test");
                }
                catch (InvalidOperationException exception)
                {
                    NavigationHelper.Output("browser.InvokeScript with no HTMLDocument got expected exception: " + exception.ToString());
                    caught = true;
                }
                if (!caught)
                {
                    NavigationHelper.Output("browser.InvokeScript didn't throw as expected");
                    _oneOrMoreTestsFailed = true;
                }

                _browser.ObjectForScripting = _testObject;
                _browser.LoadCompleted += OnHtmlLoadCompleted;

                _browser.Source = new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute);
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
            //put local machine lockdown script setting back to disable
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

            //test calling w/no args
            object returnValue = _browser.InvokeScript("SimpleTest");
            NavigationHelper.Output("InvokeScript(no args test) result is: " + returnValue);
            if (returnValue.ToString() != "42")
            {
                NavigationHelper.Output("InvokeScript no args test failed");
                _oneOrMoreTestsFailed = true;
            }

            //test calling with multiple args
            returnValue = _browser.InvokeScript("ArgTest", new string[] { "test1", "test2" });
            NavigationHelper.Output("InvokeScript(argument test) result is: " + returnValue);
            if (returnValue.ToString() != "test1test2")
            {
                NavigationHelper.Output("InvokeScript arg test failed");
                _oneOrMoreTestsFailed = true;
            }


            //test calling with all possible argument types
            caught = false;
            object[] allArguments = new object[17];

            allArguments[0] = null;             //undefined
            allArguments[1] = (float)3.14159;   //number
            allArguments[2] = (double)3.14159;  //number
            allArguments[3] = (decimal)3.14159; //number
            allArguments[4] = _browser;          //unknown
            allArguments[5] = false;            //boolean
            allArguments[6] = (byte)42;         //number
            allArguments[7] = (sbyte)49;        //number
            allArguments[8] = 'a';              //number
            allArguments[9] = (int)43;          //number
            allArguments[10] = (uint)44;        //number
            allArguments[11] = (short)45;       //number
            allArguments[12] = (ushort)46;      //number
            allArguments[13] = (long)47;        //unknown on 32 bit, number on 64 bit
            allArguments[14] = (ulong)48;       //unknown on 32 bit, number on 64 bit
            allArguments[15] = "testing";       //string
            allArguments[16] = JournalOwnership.OwnsJournal;  //number             

            try
            {
                returnValue = _browser.InvokeScript("ArgTest2", allArguments);
            }
            catch (Exception exception)
            {
                caught = true;
                NavigationHelper.Output("InvokeScript argument types test got unexpected exception: " + exception.ToString());
                _oneOrMoreTestsFailed = true;
            }

            bool is32Bit = IntPtr.Size == 4;

            NavigationHelper.Output("InvokeScript(argument types test) result is: " + returnValue);

            string expectedResult = (is32Bit ? resultFor32Bit : resultFor64Bit);
            // IE9 now handles 64-bit long/ulong values even in pure 32-bit environments

            if (SystemInformation.Current.IEVersion.StartsWith("9.0"))
            {
                expectedResult = resultFor64Bit;
            }

            if (returnValue.ToString() != expectedResult)
            {
                NavigationHelper.Output("InvokeScript argument types test failed, expected: " + expectedResult);
                _oneOrMoreTestsFailed = true;
            }

            //test calling back into our test class
            _browser.InvokeScript("CallbackTest", new string[] { "more testing" });
            NavigationHelper.Output("InvokeScript(callback test) result is: " + _testObject.PageTitle);
            if (_testObject.PageTitle != "more testing")
            {
                NavigationHelper.Output("InvokeScript callback test failed");
                _oneOrMoreTestsFailed = true;
            }

            //test setting property from script
            _browser.InvokeScript("PropertyTest", new string[] { "prop test" });
            NavigationHelper.Output("InvokeScript(property test) result is: " + _testObject.PageTitle);
            if (_testObject.PageTitle != "prop test")
            {
                NavigationHelper.Output("InvokeScript property test failed");
                _oneOrMoreTestsFailed = true;
            }

            //test InvokeScript with bad argument
            caught = false;
            object[] badArgument = new object[1];
            badArgument[0] = new PageRange(0);
            try
            {
                returnValue = _browser.InvokeScript("ArgTest", badArgument);
                NavigationHelper.Output("InvokeScript with bogus argument returned: " + returnValue.ToString());
            }
            catch (ArgumentException exception)
            {
                NavigationHelper.Output("browser.InvokeScript with bogus argument got expected exception: " + exception.ToString());
                caught = true;
            }
            catch (COMException exception) //On Server 2008 SP2, we see a COMException instead of an ArgumentException.  Pass.
            {
                NavigationHelper.Output("browser.InvokeScript with bogus argument got expected exception: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("Unexpected exception calling browser.InvokeScript with bogus argument: " + exception.ToString());
            }
            if (!caught)
            {
                NavigationHelper.Output("browser.InvokeScript with bogus argument did not get expected exception");
                _oneOrMoreTestsFailed = true;
            }

            //test InvokeScript with bogus script name
            caught = false;
            try
            {
                _browser.InvokeScript("ThisNameIsBogus");
            }
            catch (COMException exception)
            {
                NavigationHelper.Output("browser.InvokeScript with bogus script name got expected exception: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("Unexpected exception calling browser.InvokeScript with bogus script name: " + exception.ToString());
            }
            if (!caught)
            {
                NavigationHelper.Output("browser.InvokeScript with bogus script name did not get expected exception");
                _oneOrMoreTestsFailed = true;
            }

            //test InvokeScript with empty script name
            caught = false;
            try
            {
                _browser.InvokeScript("");
            }
            catch (ArgumentNullException exception)
            {
                NavigationHelper.Output("browser.InvokeScript with empty script name got expected exception: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("Unexpected exception calling browser.InvokeScript with empty script name: " + exception.ToString());
            }
            if (!caught)
            {
                NavigationHelper.Output("browser.InvokeScript with empty script name should have thrown exception but did not");
                _oneOrMoreTestsFailed = true;
            }

            //test InvokeScript with null name
            caught = false;
            try
            {
                _browser.InvokeScript(null);
            }
            catch (ArgumentNullException exception)
            {
                NavigationHelper.Output("browser.InvokeScript with null script name got expected exception: " + exception.ToString());
                caught = true;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("Unexpected exception calling browser.InvokeScript with null name: " + exception.ToString());
            }
            if (!caught)
            {
                NavigationHelper.Output("browser.InvokeScript didn't throw as expected");
                _oneOrMoreTestsFailed = true;
            }

            //test InvokeScript with null argument
            caught = false;
            try
            {
                returnValue = _browser.InvokeScript("ArgTest", null);
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("browser.InvokeScript with null argument got unexpected exception: " + exception.ToString());
                caught = true;
                _oneOrMoreTestsFailed = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("As expected, browser.InvokeScript didn't throw when a null arg was used.  Result was: " + ((returnValue == null) ? "null" : returnValue.ToString()));
            }


            if (_oneOrMoreTestsFailed)
            {
                NavigationHelper.Fail("One or more HTML interop invoke script tests failed");
            }
            else
            {
                NavigationHelper.Pass("HTML interop invoke script tests passed");
            }
        }
        #endregion

    }
}
