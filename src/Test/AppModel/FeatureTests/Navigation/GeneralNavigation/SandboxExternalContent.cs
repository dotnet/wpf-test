// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: SandboxExternalContent tests that:
//  [1] Setting SandboxExternalContent = true in partial trust throws SecurityException
//  [2] Navigating Frame to non-built in XAML when we specify SandboxExternalContent = true (FT app)
//  [3] Navigating NavigationWindow non-built in XAML when we specify SandboxExternalContent = true (FT app) 

using System;
using System.IO;
using System.Security;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Setup:
    /// 1.  Detect if we are running a partial-trust xbap or a full-trust application
    /// 2.  Navigate to Uri that starts with pack://siteoforigin:,, (two commas)
    /// 3.  Navigate to a Uri that starts with pack://siteoforigin:,,, (three commas)
    ///
    /// If we are in a partially-trusted xbap:
    /// 4.  Try SandboxExternalContent = true on both Frame and NavigationWindow.
    ///     This should throw a SecurityException.
    /// 
    /// OR...if we are in a fully-trusted application:
    /// 4.  Set SandboxExternalContent = true on both Frame and NavigationWindow, then
    ///     navigate each of these to a file binplaced in their output directory (but not
    ///     compiled with the app)
    /// </summary>

    public partial class NavigationTests : Application
    {
        internal enum SandboxExternalContent_CurrentTest
        {
            UnInit,
            NavigateSiteOfOriginDoubleComma,
            NavigateSiteOfOriginTripleComma,
            SetSandboxFromPartialTrust,
            NavigateFrameToSandboxContent,
            NavigateNavigationWindowToSandboxContent,
            End
        }

        internal enum SandboxExternalContent_NavigationContainer
        {
            Frame,
            NavigationWindow
        }

        private NavigationWindow _sandboxExternalContent_currNavWin = null;
        private Frame _sandboxExternalContent_currFrame = null;
        private SandboxExternalContent_CurrentTest _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.UnInit;
        private SandboxExternalContent_NavigationContainer _sandboxExternalContent_testContainer = SandboxExternalContent_NavigationContainer.Frame;

        private const String SandboxExternalContent_HOMEPAGE = "SandboxExternalContent_HomePage.xaml";
        private const String SandboxExternalContent_FIRSTPAGE = FLOWDOCPAGE;
        private const String SandboxExternalContent_SECONDPAGE = IMAGEPAGE;
        private const String SandboxExternalContent_THIRDPAGE = ANCHOREDPAGE;
        private const String SandboxExternalContent_SOO_DOUBLECOMMA = "pack://siteoforigin:,,/";
        private const String SandboxExternalContent_SOO_TRIPLECOMMA = "pack://siteoforigin:,,,/";
        private String _sandboxExternalContent_binplaceDir = String.Empty;
        private bool _sandboxExternalContent_isPartialTrust = false;

        void SandboxExternalContent_Startup(object sender, StartupEventArgs e)
        {
            // Initialize TestLog
            if (Log.Current == null)
                new TestLog("SandboxExternalContent");
            Application.Current.StartupUri = new Uri(SandboxExternalContent_HOMEPAGE, UriKind.RelativeOrAbsolute);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
        }

        private void SandboxExternalContent_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_sandboxExternalContent_test == SandboxExternalContent_CurrentTest.UnInit)
            {
                if (e.Navigator is NavigationWindow)
                {
                    NavigationHelper.Output("Grabbing reference to the NavigationWindow.");
                    _sandboxExternalContent_currNavWin = Application.Current.MainWindow as NavigationWindow;
                    _sandboxExternalContent_currNavWin.ContentRendered += new EventHandler(OnContentRendered_SandboxExternalContent_NavWin);

                    // Register eventhandlers on Frame
                    NavigationHelper.Output("Registering Frame level eventhandlers.");
                    _sandboxExternalContent_currFrame = LogicalTreeHelper.FindLogicalNode(_sandboxExternalContent_currNavWin.Content as DependencyObject, "frame1") as Frame;
                    _sandboxExternalContent_currFrame.ContentRendered += new EventHandler(OnContentRendered_SandboxExternalContent_Frame);

                    // Check if we are in an XBAP (partial trust) or not
                    if (BrowserInteropHelper.IsBrowserHosted)
                        _sandboxExternalContent_isPartialTrust = true;

                    _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.NavigateSiteOfOriginDoubleComma;
                }
            }
        }

        private void OnContentRendered_SandboxExternalContent_Frame(object sender, EventArgs e)
        {
            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
            {
                switch (_sandboxExternalContent_test)
                {
                    case SandboxExternalContent_CurrentTest.NavigateSiteOfOriginDoubleComma:
                        NavigationHelper.Output("Testing siteoforigin variants");
                        SandboxExternalContent_TestSiteOfOriginVariants();
                        break;

                    case SandboxExternalContent_CurrentTest.NavigateSiteOfOriginTripleComma:
                        if (SandboxExternalContent_IsAtSource(SandboxExternalContent_SOO_TRIPLECOMMA + SandboxExternalContent_FIRSTPAGE))
                        {
                            if (_sandboxExternalContent_isPartialTrust && SandboxExternalContent_TestSandboxInPartialTrust())
                            {
                                NavigationHelper.Output("SUCCESS!!! Partial trust app threw SecurityException when setting SandboxExternalContent = true in frame");
                                NavigationHelper.Pass("SandboxExternalContent partial trust test passes.");
                            }
                            else
                                SandboxExternalContent_TestSandboxInFullTrust();
                        }
                        else
                        {
                            NavigationHelper.Fail("NavWin should be at " + SandboxExternalContent_FIRSTPAGE,
                                "NavWin is at " + _sandboxExternalContent_currNavWin.Source);
                        }
                        break;

                    case SandboxExternalContent_CurrentTest.NavigateFrameToSandboxContent:
                        SandboxExternalContent_VerifySandboxInFullTrust();
                        break;

                    default:
                        NavigationHelper.Fail(_sandboxExternalContent_test.ToString() + " is not one of the predefined subtests.  Exiting test case.");
                        break;
                }
            }
        }


        private void OnContentRendered_SandboxExternalContent_NavWin(object sender, EventArgs e)
        {
            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
            {
                switch (_sandboxExternalContent_test)
                {
                    case SandboxExternalContent_CurrentTest.NavigateSiteOfOriginDoubleComma:
                        NavigationHelper.Output("Testing siteoforigin variants");
                        SandboxExternalContent_TestSiteOfOriginVariants();
                        break;

                    case SandboxExternalContent_CurrentTest.NavigateSiteOfOriginTripleComma:
                        if (SandboxExternalContent_IsAtSource(SandboxExternalContent_SOO_TRIPLECOMMA + SandboxExternalContent_FIRSTPAGE))
                        {
                            if (_sandboxExternalContent_isPartialTrust && SandboxExternalContent_TestSandboxInPartialTrust())
                                NavigationHelper.Output("SUCCESS!!! Partial trust app threw SecurityException when setting SandboxExternalContent = true in NavigationWindow");
                            else
                                SandboxExternalContent_TestSandboxInFullTrust();
                        }
                        else
                        {
                            NavigationHelper.Fail("NavWin should be at " + SandboxExternalContent_FIRSTPAGE,
                                "NavWin is at " + _sandboxExternalContent_currNavWin.Source);
                        }
                        break;

                    case SandboxExternalContent_CurrentTest.NavigateNavigationWindowToSandboxContent:
                        SandboxExternalContent_VerifySandboxInFullTrust();
                        break;

                    default:
                        NavigationHelper.ExitWithError(_sandboxExternalContent_test.ToString() + " is not one of the predefined subtests.  Exiting test case.");
                        break;
                }
            }
        }

        private bool SandboxExternalContent_IsAtSource(String sourceUri)
        {
            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
                return _sandboxExternalContent_currNavWin.Source.ToString().Equals(sourceUri);
            else if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
                return _sandboxExternalContent_currFrame.Source.ToString().Equals(sourceUri);
            else
            {
                NavigationHelper.Output("Not using Frame/NavigationWindow.  Cannot check Source property.");
                return false;
            }
        }

        private void SandboxExternalContent_NavigateToUri(Uri destination)
        {
            NavigationHelper.Output("Navigating " + _sandboxExternalContent_testContainer + " to " + destination.ToString());
            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
                _sandboxExternalContent_currNavWin.Navigate(destination);
            else if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
                _sandboxExternalContent_currFrame.Navigate(destination);
            else
                NavigationHelper.Fail("Not using Frame/NavigationWindow.  Cannot navigate");

            return;
        }

        private void SandboxExternalContent_TestSiteOfOriginVariants()
        {
            if ((_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame && SandboxExternalContent_IsAtSource(SandboxExternalContent_FIRSTPAGE)) ||
                (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow && SandboxExternalContent_IsAtSource(SandboxExternalContent_HOMEPAGE)))
            {
                NavigationHelper.Output("Navigating to " + SandboxExternalContent_SOO_DOUBLECOMMA + SandboxExternalContent_SECONDPAGE);
                SandboxExternalContent_NavigateToUri(new Uri(SandboxExternalContent_SOO_DOUBLECOMMA + SandboxExternalContent_SECONDPAGE, UriKind.RelativeOrAbsolute));
            }
            else if (SandboxExternalContent_IsAtSource(SandboxExternalContent_SOO_DOUBLECOMMA + SandboxExternalContent_SECONDPAGE))
            {
                _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.NavigateSiteOfOriginTripleComma;
                NavigationHelper.Output("Navigating to " + SandboxExternalContent_SOO_TRIPLECOMMA + SandboxExternalContent_FIRSTPAGE);
                SandboxExternalContent_NavigateToUri(new Uri(SandboxExternalContent_SOO_TRIPLECOMMA + SandboxExternalContent_FIRSTPAGE, UriKind.RelativeOrAbsolute));
            }
        }

        private bool SandboxExternalContent_TestSandboxInPartialTrust()
        {
            if (_sandboxExternalContent_isPartialTrust)
            {
                _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.SetSandboxFromPartialTrust;
                try
                {
                    NavigationHelper.Output("Setting SandboxExternalContent = true in PT " + _sandboxExternalContent_testContainer);
                    if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
                        _sandboxExternalContent_currNavWin.SandboxExternalContent = true;
                    else if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
                        _sandboxExternalContent_currFrame.SandboxExternalContent = true;
                    else
                        NavigationHelper.Output("Not using Frame/NavigationWindow.  Cannot set SandboxExternalContent.");

                    if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame ||
                        _sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
                    {
                        NavigationHelper.Output("Should have thrown a SecurityException at this point but didn't");
                        NavigationHelper.Fail("SecurityException", "Expected exception not thrown.");
                    }
                }
                catch (SecurityException se)
                {
                    NavigationHelper.Output("SUCCESS!!! Setting SandboxExternalContent = true in a partial trust app threw a SecurityException\n" + se.ToString());
                    return true;
                }
                catch (Exception exp)
                {
                    NavigationHelper.Fail("SecurityException",
                        "Unexpected exception" + exp.ToString());
                }
            }
            else
                NavigationHelper.Fail("This method can only be called from partially trusted apps");

            return false;
        }

        private void SandboxExternalContent_TestSandboxInFullTrust()
        {
            _sandboxExternalContent_binplaceDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            String page3 = Path.Combine(_sandboxExternalContent_binplaceDir, SandboxExternalContent_THIRDPAGE);

            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
            {
                _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.NavigateNavigationWindowToSandboxContent;
                _sandboxExternalContent_currNavWin.SandboxExternalContent = true;
            }
            else if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
            {
                _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.NavigateFrameToSandboxContent;
                _sandboxExternalContent_currFrame.SandboxExternalContent = true;
            }

            try
            {
                SandboxExternalContent_NavigateToUri(new Uri(page3, UriKind.RelativeOrAbsolute));
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("Unexpected exception thrown attempting to navigate to " + page3 + " with SandboxExternalContent = true\n" + exp.ToString());
            }

            return;
        }

        private void SandboxExternalContent_VerifySandboxInFullTrust()
        {
            if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.NavigationWindow)
            {
                // Verify that SandboxExternalContent = true
                NavigationHelper.Output("Checking SandboxExternalContent value");
                if (!_sandboxExternalContent_currNavWin.SandboxExternalContent)
                {
                    NavigationHelper.Fail("NavWin.SandboxExternalContent == true",
                        "NavWin.SandboxExternalContent == " + _sandboxExternalContent_currNavWin.SandboxExternalContent);
                }
                else
                {
                    // Verify that we are at the proper Source uri
                    NavigationHelper.Output("Checking NavWin's Source uri");
                    String page3 = SandboxExternalContent_ConvertSandboxedUri(Path.Combine(_sandboxExternalContent_binplaceDir, SandboxExternalContent_THIRDPAGE));
                    if (SandboxExternalContent_IsAtSource(page3))
                        NavigationHelper.Pass("NavWin.Source is at " + page3);
                    else
                        NavigationHelper.Fail("NavWin.Source = " + page3,
                            "NavWin.Source = " + _sandboxExternalContent_currNavWin.Source);
                }
            }
            else if (_sandboxExternalContent_testContainer == SandboxExternalContent_NavigationContainer.Frame)
            {
                // Verify that SandboxExternalContent = true
                NavigationHelper.Output("Checking SandboxExternalContent value");
                if (!_sandboxExternalContent_currFrame.SandboxExternalContent)
                {
                    NavigationHelper.Fail("Frame.SandboxExternalContent == true",
                        "Frame.SandboxExternalContent == " + _sandboxExternalContent_currFrame.SandboxExternalContent);
                }
                else
                {
                    // Verify that we are at the proper Source uri
                    NavigationHelper.Output("Checking Frame's Source uri");
                    String page3 = SandboxExternalContent_ConvertSandboxedUri(Path.Combine(_sandboxExternalContent_binplaceDir, SandboxExternalContent_THIRDPAGE));
                    if (SandboxExternalContent_IsAtSource(page3))
                    {
                        NavigationHelper.Output("SUCCESS!!! Frame.Source is at " + page3);
                        // Switch from Frame tests to NavigationWindow tests
                        _sandboxExternalContent_testContainer = SandboxExternalContent_NavigationContainer.NavigationWindow;
                        _sandboxExternalContent_test = SandboxExternalContent_CurrentTest.NavigateSiteOfOriginDoubleComma;
                        SandboxExternalContent_TestSiteOfOriginVariants();
                    }
                    else
                        NavigationHelper.Fail("Frame should be at " + page3,
                            "Frame is at " + _sandboxExternalContent_currFrame.Source);

                }
            }
        }

        private String SandboxExternalContent_ConvertSandboxedUri(String sourceUri)
        {
            sourceUri = sourceUri.Replace("file:\\", "file:///");
            sourceUri = sourceUri.Replace('\\', '/');
            return sourceUri;
        }
    }
}
