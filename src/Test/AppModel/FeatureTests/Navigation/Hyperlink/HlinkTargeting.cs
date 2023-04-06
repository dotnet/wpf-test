// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  HyperlinkTargeting tests that programmatically navigating
//  using Hyperlinks with special _* targets results in a SecurityException and SystemException
//
//  Step 1 - Verify we can navigate to the xaml file in the hyperlink without TargetName
//           This will guarantee that the xaml file exists at the site of origin
//  Step 2 - Go back to the initial page
//  Step 3 - Programmatically click on Hyperlink using DoClick
//           (try this for _parent, _self, _top, _blank, unrecognized target)
//  Step 4 - Programmatically click on Hyperlink by raising RequestNavigate event
//           (try this for _parent, _self, _top, _blank, unrecognized target)
//
//  Note : Some exceptions are not caught in the DispatcherUnhandledException event handler
//         
//

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Globalization;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // HlinkTargeting
    public class HlinkTargeting
    {
        private enum Test
        {
            UnInit,
            DoClickNoTargetName, // hyperlink doesn't have a TargetName, should navigate
            GoBack,              // go back to initial page
            DoClickBlank,
            DoClickSelf,
            DoClickTop,
            DoClickParent,
            DoClickUnrecognized,
            ReqNavNoTargetName,  // navigating to an html file at site of origin wihtout TargetName
            ReqNavBlank,
            ReqNavSelf,
            ReqNavTop,
            ReqNavParent,
            ReqNavUnrecognized,
            End
        }

        #region private methods
        private NavigationWindow _currNavWin = null;
        private Test _currentTest = Test.UnInit;
        private String _currHyperlink = String.Empty;
        private bool _testPassed = true;
        private String _navigationExceptionMessage = String.Empty; // exception message for hyperlink navigation
        #endregion

        /// <summary>
        /// StartUp eventhandler initializes the TestLog and registers the eventhandler
        /// for application load completion.
        /// </summary>
        public void Startup(object sender, StartupEventArgs e)
        {
            // Initialize TestLog
            NavigationHelper.CreateLog("HyperlinkTargeting");

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);

            Application.Current.StartupUri = new Uri("HlinkTargeting_Content.xaml", UriKind.RelativeOrAbsolute);

            // test expects WebPage1_Loose.html at site of origin but none of tests is supposed to navigate to it
            // therefore make sure this file exists before running tests
            try
            {
                Stream stream = Application.GetRemoteStream(new Uri("WebPage1_Loose.html", UriKind.RelativeOrAbsolute)).Stream;
                if (stream == null)
                {
                    NavigationHelper.Fail("Couldn't locate WebPage1_Loose.html at site of origin");
                }
            }
            catch (Exception ex)
            {
                NavigationHelper.Fail("Application.GetRemoteStream failed with exception. " + ex.Message);
            }

            // retrieve the localized exception message
            Assembly presentationFramework = null;
            try
            {
                presentationFramework = Assembly.Load("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
                _navigationExceptionMessage = Extract.GetExceptionString("FailToNavigateUsingHyperlinkTarget", presentationFramework);
                NavigationHelper.Output("Retrieved navigationExceptionMessage from PresentationFramework.dll - " + _navigationExceptionMessage);
            }
            catch (Exception ex)
            {
                NavigationHelper.Output("Couldn't get resource string for FailToNavigateUsingHyperlinkTarget: " + ex.ToString());
                _navigationExceptionMessage = "Failed to navigate using the Hyperlink target."; // if we get an exception reset to english and move forward
            }
        }

        /// <summary>
        /// LoadCompleted eventhandler grabs a reference to the current NavigationWindow
        /// and starts the test suite.
        /// </summary>
        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("e.Navigator is " + e.Navigator.ToString());
            if (_currentTest == Test.UnInit)
            {
                NavigationHelper.Output("Initializing test.");
                if (e.Navigator is NavigationWindow)
                {
                    NavigationHelper.Output("Grabbing reference to the current NavigationWindow.");
                    _currNavWin = Application.Current.MainWindow as NavigationWindow;
                    if (_currNavWin == null)
                    {
                        NavigationHelper.Fail("Couldn't retrieve the NavigationWindow");
                    }
                }

                // Start running the test suite
                NavigationHelper.Output("Starting the test suite.");
                RouteSubTests();
            }
            else if (_currentTest == Test.DoClickNoTargetName || _currentTest == Test.GoBack)
            {
                RouteSubTests();
            }
        }

        // Navigated event handler
        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired. Test - " + _currentTest);

            if (_currentTest != Test.UnInit && _currentTest != Test.DoClickNoTargetName && _currentTest != Test.GoBack)
            {
                // we don't expect to be in Navigated event handler during other tests
                NavigationHelper.Fail("Unexpected Navigated event fired in test " + _currentTest);
            }
        }

        /// <summary>
        /// Writes a short description of the test to the log and executes Hyperlink.DoClick or
        /// raises the RequestNavigate event, depending on the value of the current test.
        /// </summary>
        private void RouteSubTests()
        {
            MoveToNextTest();

            if (_testPassed)
            {
                switch (_currentTest)
                {
                    case Test.DoClickNoTargetName:
                        NavigationHelper.Output("[1] Programmatic click via Hyperlink.DoClick() on hlink without TargetName");
                        NavigateHyperlinkViaDoClick();
                        break;

                    case Test.GoBack:
                        NavigationHelper.Output("[2] Calling GoBack() on the NavigationWindow");
                        _currNavWin.GoBack();
                        break;

                    case Test.DoClickBlank:
                        NavigationHelper.Output("[3] Programmatic click via Hyperlink.DoClick() on hlink with TargetName = _blank");
                        NavigateHyperlinkViaDoClick();
                        break;

                    case Test.DoClickSelf:
                        NavigationHelper.Output("[4] Programmatic click via Hyperlink.DoClick() on hlink with TargetName = _self");
                        NavigateHyperlinkViaDoClick();
                        break;

                    case Test.DoClickTop:
                        NavigationHelper.Output("[5] Programmatic click via Hyperlink.DoClick() on hlink with TargetName = _top");
                        NavigateHyperlinkViaDoClick();
                        break;

                    case Test.DoClickParent:
                        NavigationHelper.Output("[6] Programmatic click via Hyperlink.DoClick() on hlink with TargetName = _parent");
                        NavigateHyperlinkViaDoClick();
                        break;

                    case Test.DoClickUnrecognized:
                        NavigationHelper.Output("[7] Programmatic click via Hyperlink.DoClick() on hlink with unrecognized TargetName");
                        // exception is not routed to DispatcherUnhandledException
                        try
                        {
                            NavigateHyperlinkViaDoClick();
                        }
                        catch (Exception e)
                        {
                            // process the exception
                            ProcessException(e);
                        }
                        break;

                    case Test.ReqNavNoTargetName:
                        NavigationHelper.Output("[8] Programmatic click via raising RequestNavigate event on hlink with no TargetName");
                        NavigateHyperlinkViaRequestNavigate();
                        break;

                    case Test.ReqNavBlank:
                        NavigationHelper.Output("[9] Programmatic click via raising RequestNavigate event on hlink with TargetName = _blank");
                        NavigateHyperlinkViaRequestNavigate();
                        break;

                    case Test.ReqNavSelf:
                        NavigationHelper.Output("[10] Programmatic click via raising RequestNavigate event on hlink with TargetName = _self");
                        // exception is not routed to DispatcherUnhandledException
                        try
                        {
                            NavigateHyperlinkViaRequestNavigate();
                        }
                        catch (Exception e)
                        {
                            // process the exception
                            ProcessException(e);
                        }
                        break;

                    case Test.ReqNavTop:
                        NavigationHelper.Output("[11] Programmatic click via raising RequestNavigate event on hlink with TargetName = _top");
                        // exception is not routed to DispatcherUnhandledException
                        try
                        {
                            NavigateHyperlinkViaRequestNavigate();
                        }
                        catch (Exception e)
                        {
                            // process the exception
                            ProcessException(e);
                        }
                        break;

                    case Test.ReqNavParent:
                        NavigationHelper.Output("[12] Programmatic click via raising RequestNavigate event on hlink with TargetName = _parent");
                        try
                        {
                            NavigateHyperlinkViaRequestNavigate();
                        }
                        catch (Exception e)
                        {
                            // process the exception
                            ProcessException(e);
                        }
                        break;

                    case Test.ReqNavUnrecognized:
                        NavigationHelper.Output("[13] Programmatic click via raising RequestNavigate event on hlink with unrecognized TargetName");
                        try
                        {
                            NavigateHyperlinkViaRequestNavigate();
                        }
                        catch (Exception e)
                        {
                            // process the exception
                            ProcessException(e);
                        }
                        break;

                    case Test.End:
                        NavigationHelper.Pass("Success. All subtests passed.");
                        break;

                    default:
                        NavigationHelper.Output("Test " + _currentTest.ToString() + " is not one of the pre-defined tests. Exiting.");
                        _testPassed = false;
                        _currentTest = Test.End;
                        break;
                }
            }
            else
            {
                NavigationHelper.Fail("An earlier subtest failed.");
            }
        }


        /// <summary>
        /// Changes the current test value to be the next one in the Test enum list.
        /// </summary>
        private void MoveToNextTest()
        {
            NavigationHelper.Output("Moving forward to next subtest.");

            if (_currentTest == Test.End)
            {
                NavigationHelper.Fail("Unexpected test Test.End found in MoveToNextTest()");
            }
            else
            {
                _currentTest = (Test)((int)_currentTest + 1);
                NavigationHelper.Output("Switching to " + _currentTest.ToString());
            }
        }

        /// <summary>
        /// NavigateHyperlinkViaRequestNavigate finds the Hyperlink on the current NavigationWindow and creates a
        /// RequestNavigateEventArgs object with the Hyperlink's information.  This is then passed to RaiseEvent
        /// to initiate a navigation.  The navigation is expected to throw a System.Exception
        /// (with the Exception message: "Failed to navigate using the Hyperlink target")
        /// </summary>
        private void NavigateHyperlinkViaRequestNavigate()
        {
            _currHyperlink = _currentTest.ToString();

            NavigationHelper.Output("Locating " + _currHyperlink + " in current NavigationWindow");
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode(_currNavWin.Content as DependencyObject, _currHyperlink) as Hyperlink;

            if (hlink == null)
            {
                NavigationHelper.Fail("Could not find control: " + _currHyperlink + " in " + _currNavWin.Content.ToString());
            }
            else
            {
                NavigationHelper.Output("Creating RequestNavigateEventArgs.");
                RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(hlink.NavigateUri, hlink.TargetName);
                requestNavigateEventArgs.Source = hlink;

                NavigationHelper.Output("Raising RequestNavigate event");
                hlink.RaiseEvent(requestNavigateEventArgs);
            }
        }

        /// <summary>
        /// NavigateHyperlinkViaDoClick finds the Hyperlink on the current NavigationWindow and invokes its
        /// DoClick method, which causes the navigation.  The navigation is expected to throw a System.Exception
        /// (with the Exception message: "Failed to navigate using the Hyperlink target")
        /// </summary>
        private void NavigateHyperlinkViaDoClick()
        {
            _currHyperlink = _currentTest.ToString();

            NavigationHelper.Output("Locating " + _currHyperlink + " in current NavigationWindow");
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode(_currNavWin.Content as DependencyObject, _currHyperlink) as Hyperlink;

            if (hlink == null)
            {
                NavigationHelper.Fail("Could not find control: " + _currHyperlink + " in " + _currNavWin.Content.ToString());
            }
            else
            {
                NavigationHelper.Output("Invoking the Hyperlink " + _currHyperlink + "'s DoClick method");
                hlink.DoClick();
            }
        }

        // verify System.Exception and System.Security.SecurityException are caught
        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception type = " + e.Exception.GetType());
            NavigationHelper.Output("Exception message = " + e.Exception.Message);
            NavigationHelper.Output("Test = " + _currentTest);
            e.Handled = true;

            if (e.Exception is System.Security.SecurityException)
            {
                if (_currentTest == Test.ReqNavNoTargetName)
                {
                    // Navigating to html file at site of origin without TargetName should throw SecurityException
                    // because we use RequestNavigateEventArgs
                    NavigationHelper.Output("Expected System.Security.SecurityException is correctly thrown");
                    // Move to next test
                    RouteSubTests();
                }
            }
            else if (e.Exception is System.Exception)
            {
                if (_currentTest == Test.DoClickBlank || _currentTest == Test.DoClickSelf ||
                    _currentTest == Test.DoClickTop || _currentTest == Test.DoClickParent ||
                    _currentTest == Test.ReqNavBlank)
                {
                    if (String.Compare(e.Exception.Message.ToString(), _navigationExceptionMessage) == 0)
                    {
                        NavigationHelper.Output("Expected System.Exception is correctly thrown");
                        // Move to next test
                        RouteSubTests();
                    }
                    else
                    {
                        NavigationHelper.Fail("Unexpected Exception Message, expected: " + _navigationExceptionMessage);
                    }
                }
                else
                {
                    NavigationHelper.Fail("Unexpected System.Exception caught in DispatcherUnhandledException");
                }
            }
            else
            {
                NavigationHelper.Fail("Unexpected exception caught in DispatcherUnhandledException");
            }
        }

        // process the exceptions not caught in DispatcherUnhandledException
        // verify System.Exception with correct message is caught
        private void ProcessException(Exception e)
        {
            NavigationHelper.Output("Exception type = " + e.GetType());
            NavigationHelper.Output("Exception message = " + e.Message);
            if (e is System.Exception)
            {
                if (_currentTest == Test.DoClickUnrecognized || _currentTest == Test.ReqNavSelf ||
                    _currentTest == Test.ReqNavTop || _currentTest == Test.ReqNavParent ||
                    _currentTest == Test.ReqNavUnrecognized)
                {
                    if (String.Compare(e.Message.ToString(), _navigationExceptionMessage) == 0)
                    {
                        NavigationHelper.Output("Expected System.Exception is correctly thrown");
                        // Move to next test
                        RouteSubTests();
                    }
                    else
                    {
                        NavigationHelper.Fail("Unexpected Exception Message, expected: " + _navigationExceptionMessage);
                    }
                }
                else
                {
                    NavigationHelper.Fail("Unexpected System.Exception caught in ProcessException. Test = " + _currentTest);
                }
            }
            else
            {
                NavigationHelper.Fail("Unexpected exception caught in ProcessException");
            }
        }
    }
}
