// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Collections;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Test.Loaders;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Steps to add test case:
    /// (1) Create delegates for new tests (Test1_Navigating, Test1_Navigated etc...)
    /// (2) Populate DelegateTable with new tests' event delegates
    /// (3) Implement test logic inside delegates
    /// </summary>

    public partial class NavigationTests : System.Windows.Application
    {
        #region Local Variables
        Hashtable _delegateTable = new Hashtable();
        string _currentTest = "";
        string[] _appArgs;

        private FrameTestClass _frameTest = null;
        private NavigationWindow    _navWin = null;
        private RootBrowserWindowTestClass _rbwTest = null;
        #endregion

        #region Navigation page destinations
        private String _RESOURCEPREFIX = String.Empty;
        private const String FLOWDOCPAGE = @"FlowDocument_Loose.xaml";
        private const String CONTROLSPAGE = @"ContentControls_Page.xaml";
        private const String BUTTONNAME = @"navigateButton";
        private const String ANCHOREDPAGE = @"AnchoredPage_Loose.xaml";
        private const String IMAGEPAGE = @"ImagePage_Loose.xaml";
        private const String FRAMEPAGE = @"FramePage_Loose.xaml";
        private const String HLINKPAGE = @"HyperlinkPage_Loose.xaml";
        private const String BOTTOMFRAGMENT = @"#bottom";
        private const String TOPFRAGMENT = @"#top";
        private const String SITEOFORIGINPREFIX = @"pack://siteoforigin:,,,/";

        // PageFunction destinations
        private const String MARKUPPF1 = @"MarkupPF1.xaml";
        private const String MARKUPSTRPF1 = @"MarkupStrPF1.xaml";
        private const String MARKUPBOOLPF1 = @"MarkupBoolPF1.xaml";
        private const String LAUNCHPF = @"LaunchPage.xaml";
        #endregion

        #region Delegate type declarations
        // Declarations for each delegate types (events)
        delegate void Startup_Delegate(object sender, StartupEventArgs e);
        delegate void Navigating_Delegate(object sender, NavigatingCancelEventArgs e);
        delegate void Navigated_Delegate(object sender, NavigationEventArgs e);
        delegate void NavigationFailed_Delegate(object sender, NavigationFailedEventArgs e);
        delegate void NavigationProgress_Delegate(object sender, NavigationProgressEventArgs e);
        delegate void NavigationStopped_Delegate(object sender, NavigationEventArgs e);
        delegate void FragmentNavigation_Delegate(object sender, FragmentNavigationEventArgs e);
        delegate void FragmentNavigationToObject_Delegate(object sender, FragmentNavigationEventArgs e);
        delegate void LoadCompleted_Delegate(object sender, NavigationEventArgs e);
        delegate void Exit_Delegate(object sender, ExitEventArgs e);
        delegate void DispatcherUnhandledException_Delegate(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e);
        delegate void ContentRendered_Delegate(object sender, EventArgs e);
        #endregion
        
        #region Application OnStartup (Populating DelegateTable)
        /***********************************************
         * Default Application OnStartup override
         * Test event delegates should be populated here
         ***********************************************/
        protected override void OnStartup(StartupEventArgs e)
        {
            Microsoft.Test.CrossProcess.DictionaryStore.StartClient();
            string preLogging = "ENTERING OnStartup";

            _appArgs = ApplicationMonitor.GetArguments();
            if (_appArgs.Length == 0)
            {
                _appArgs = e.Args;
            }

            preLogging += "\nNumber of Args received: " + _appArgs.Length.ToString();
            if (_appArgs.Length < 1)
            {
                NavigationHelper.Fail("*** Testname is needed ***");
            }
            else
            {
                _currentTest = _appArgs[0];
                preLogging += ("\nCurrent Test: " + _currentTest.ToString());
            }
            if (Log.Current == null)
            {
                // HACK: The old navigation area was allowed to have many tests expect this as a window title
                // And many tests set window title to the current test log
                // Thus we must name the current test log RedirectMitigation.  SaMadan to eventually fix this.
                LogManager.BeginTest("RedirectMitigation");
            }
            Log.Current.CreateVariation("RedirectMitigation");
            // MORE HACK:  All the actual redirect mitigation tests also need CurrentTest to be "RedirectMitigation".
            // Thankfully they all start with Redirect or have empty names.  This is needed for the delegate table below to work.
            if (_currentTest.ToLowerInvariant().StartsWith("redirect") || String.IsNullOrEmpty(_currentTest))
            {
                _currentTest = "RedirectMitigation";
                Log.Current.CurrentVariation.LogMessage("Renamed CurrentTest to RedirectMitigation to work around test bug");
            }
            
            Log.Current.CurrentVariation.LogMessage(preLogging);
            PopulateDelegateTable();

            // Initialize the RESOURCEPREFIX
            _RESOURCEPREFIX += "NavigationTests_";
            if (BrowserInteropHelper.IsBrowserHosted)
                _RESOURCEPREFIX += "Browser;component/";
            else
                _RESOURCEPREFIX += "Standalone;component/";

            // Setup logging mechanism
            Startup_Delegate startupHandler = _delegateTable[_currentTest + "_Startup"] as Startup_Delegate;
            if (startupHandler != null)
            {
                startupHandler(this, e);
            }
            else
            {
                NavigationHelper.Fail("*** Startup handler is needed ***");
            }
            base.OnStartup(e);
        }

        private void PopulateDelegateTable()
        {
            #region Frame test cases

            // FrameLoadCompleted delegates
            _delegateTable["FrameLoadCompleted_Startup"] = new Startup_Delegate(FrameLoadCompleted_Startup);
            _delegateTable["FrameLoadCompleted_LoadCompleted"] = new LoadCompleted_Delegate(FrameLoadCompleted_LoadCompleted);

            // FrameNavigated delegates
            _delegateTable["FrameNavigated_Startup"] = new Startup_Delegate(FrameNavigated_Startup);
            _delegateTable["FrameNavigated_Navigated"] = new Navigated_Delegate(FrameNavigated_Navigated);
            _delegateTable["FrameNavigated_LoadCompleted"] = new LoadCompleted_Delegate(FrameNavigated_LoadCompleted);

            // FrameRelativeUri delegates
            _delegateTable["FrameRelativeUri_Startup"] = new Startup_Delegate(FrameRelativeUri_Startup);
            _delegateTable["FrameRelativeUri_LoadCompleted"] = new LoadCompleted_Delegate(FrameRelativeUri_LoadCompleted);

            // FrameStatusBar delegates -- NOT LAB READY

            // MultipleFrames delegates
            _delegateTable["MultipleFrames_Startup"] = new Startup_Delegate(MultipleFrames_Startup);
            _delegateTable["MultipleFrames_Navigated"] = new Navigated_Delegate(MultipleFrames_Navigated);
            _delegateTable["MultipleFrames_ContentRendered"] = new ContentRendered_Delegate(MultipleFrames_ContentRendered);

            // FrameLocalFileAccess delegates
            _delegateTable["FrameLocalFileAccess_Startup"] = new Startup_Delegate(FrameLocalFileAccess_Startup);
            _delegateTable["FrameLocalFileAccess_Navigated"] = new Navigated_Delegate(FrameLocalFileAccess_Navigated);

            #endregion

            #region General Navigation test cases
            //GeneralNavigation
            _delegateTable["AddBackEntryOnEmptyNS_Startup"] = new Startup_Delegate(AddBackEntryOnEmptyNS_Startup);
            _delegateTable["CrossDomainBlocked_Startup"] = new Startup_Delegate(CrossDomainBlocked_Startup);
            _delegateTable["CrossDomainBlocked_LoadCompleted"] = new LoadCompleted_Delegate(CrossDomainBlocked_LoadCompleted);
            _delegateTable["FilePermission_Startup"] = new Startup_Delegate(FilePermission_Startup);
            _delegateTable["FilePermission_Navigated"] = new Navigated_Delegate(FilePermission_Navigated);
            _delegateTable["FragmentNavigation_Startup"] = new Startup_Delegate(FragmentNavigation_Startup);
            _delegateTable["FragmentNavigation_LoadCompleted"] = new LoadCompleted_Delegate(FragmentNavigation_LoadCompleted);
            _delegateTable["FragmentNavigation_Navigated"] = new Navigated_Delegate(FragmentNavigation_Navigated);
            _delegateTable["FragmentNavigation_FragmentNavigation"] = new FragmentNavigation_Delegate(FragmentNavigation_FragmentNavigation);
            _delegateTable["GetNavigationService_Startup"] = new Startup_Delegate(GetNavigationService_Startup);
            _delegateTable["GetNavigationService_LoadCompleted"] = new LoadCompleted_Delegate(GetNavigationService_LoadCompleted);
            _delegateTable["LoadCompleted_Startup"] = new Startup_Delegate(LoadCompleted_Startup);
            _delegateTable["LoadCompleted_LoadCompleted"] = new LoadCompleted_Delegate(LoadCompleted_LoadCompleted);
            _delegateTable["LooseImageResource_Startup"] = new Startup_Delegate(LooseImageResource_Startup);
            _delegateTable["LooseImageResource_Exit"] = new Exit_Delegate(LooseImageResource_Exit);
            _delegateTable["LooseImageResource_Navigated"] = new Navigated_Delegate(LooseImageResource_Navigated);
            _delegateTable["LooseImageResource_LoadCompleted"] = new LoadCompleted_Delegate(LooseImageResource_LoadCompleted);
            _delegateTable["LooseXaml_Startup"] = new Startup_Delegate(LooseXaml_Startup);
            _delegateTable["LooseXaml_LoadCompleted"] = new LoadCompleted_Delegate(LooseXaml_LoadCompleted);
            _delegateTable["MultithreadedApp_Startup"] = new Startup_Delegate(MultithreadedApp_Startup);
            _delegateTable["MultithreadedApp_Exit"] = new Exit_Delegate(MultithreadedApp_Exit);
            _delegateTable["NavigateArgs_Startup"] = new Startup_Delegate(NavigateArgs_Startup);
            _delegateTable["NavigateArgs_Navigating"] = new Navigating_Delegate(NavigateArgs_Navigating);
            _delegateTable["NavigateArgs_Navigated"] = new Navigated_Delegate(NavigateArgs_Navigated);
            _delegateTable["NavigateArgs_NavigationProgress"] = new NavigationProgress_Delegate(NavigateArgs_NavigationProgress);
            _delegateTable["NavigateArgs_FragmentNavigation"] = new FragmentNavigation_Delegate(NavigateArgs_FragmentNavigation);
            _delegateTable["NavigateArgs_Exit"] = new Exit_Delegate(NavigateArgs_Exit);
            _delegateTable["NavigateArgs_LoadCompleted"] = new LoadCompleted_Delegate(NavigateArgs_LoadCompleted);
            _delegateTable["NavigateBrowserWinToPage_Startup"] = new Startup_Delegate(NavigateBrowserWinToPage_Startup);
            _delegateTable["NavigateBrowserWinToPage_Navigating"] = new Navigating_Delegate(NavigateBrowserWinToPage_Navigating);
            _delegateTable["NavigateBrowserWinToPage_Navigated"] = new Navigated_Delegate(NavigateBrowserWinToPage_Navigated);
            _delegateTable["NavigateBrowserWinToPage_LoadCompleted"] = new LoadCompleted_Delegate(NavigateBrowserWinToPage_LoadCompleted);
            _delegateTable["NavigateUri_Startup"] = new Startup_Delegate(NavigateUri_Startup);
            _delegateTable["NavigateUri_Navigated"] = new Navigated_Delegate(NavigateUri_Navigated);
            _delegateTable["NavigateUriWithFragment_Startup"] = new Startup_Delegate(NavigateUriWithFragment_Startup);
            _delegateTable["NavigateUriWithFragment_LoadCompleted"] = new LoadCompleted_Delegate(NavigateUriWithFragment_LoadCompleted);
            _delegateTable["Navigating_Startup"] = new Startup_Delegate(Navigating_Startup);
            _delegateTable["Navigating_Navigating"] = new Navigating_Delegate(Navigating_Navigating);
            _delegateTable["Navigating_LoadCompleted"] = new LoadCompleted_Delegate(Navigating_LoadCompleted);

            switch (_currentTest)
            {
                //GeneralNavigation
                case "NavigatingEvents":
                    NavigatingEvents navigatingEvents = new NavigatingEvents();
                    _delegateTable["NavigatingEvents_Startup"] = new Startup_Delegate(navigatingEvents.Startup);
                    _delegateTable["NavigatingEvents_LoadCompleted"] = new LoadCompleted_Delegate(navigatingEvents.LoadCompleted);
                    break;

                case "NavigateToObject":
                    NavigateToObject navigateToObject = new NavigateToObject();
                    _delegateTable["NavigateToObject_Startup"] = new Startup_Delegate(navigateToObject.Startup);
                    _delegateTable["NavigateToObject_LoadCompleted"] = new LoadCompleted_Delegate(navigateToObject.LoadCompleted);
                    _delegateTable["NavigateToObject_Navigated"] = new Navigated_Delegate(navigateToObject.Navigated);
                    break;

                case "FragmentNavigationToObject":
                    FragmentNavigationToObject fragmentNavigationToObject = new FragmentNavigationToObject();
                    _delegateTable["FragmentNavigationToObject_Startup"] = new Startup_Delegate(fragmentNavigationToObject.Startup);
                    _delegateTable["FragmentNavigationToObject_LoadCompleted"] = new LoadCompleted_Delegate(fragmentNavigationToObject.LoadCompleted);
                    _delegateTable["FragmentNavigationToObject_Navigated"] = new Navigated_Delegate(fragmentNavigationToObject.Navigated);
                    _delegateTable["FragmentNavigationToObject_FragmentNavigation"] = new FragmentNavigation_Delegate(fragmentNavigationToObject.FragmentNavigation);
                    break;

                case "StopNavigation":
                    StopNavigation stopNavigation = new StopNavigation();
                    _delegateTable["StopNavigation_Startup"] = new Startup_Delegate(stopNavigation.Startup);
                    _delegateTable["StopNavigation_Navigating"] = new Navigating_Delegate(stopNavigation.Navigating);
                    _delegateTable["StopNavigation_NavigationStopped"] = new NavigationStopped_Delegate(stopNavigation.NavigationStopped);
                    _delegateTable["StopNavigation_Navigated"] = new Navigated_Delegate(stopNavigation.Navigated);
                    _delegateTable["StopNavigation_LoadCompleted"] = new LoadCompleted_Delegate(stopNavigation.LoadCompleted);
                    _delegateTable["StopNavigation_ContentRendered"] = new ContentRendered_Delegate(stopNavigation.ContentRendered);
                    break;

                //Frame Test Cases
                case "FrameFocus":
                    FrameFocus frameFocus = new FrameFocus();
                    _delegateTable["FrameFocus_Startup"] = new Startup_Delegate(frameFocus.Startup);
                    _delegateTable["FrameFocus_LoadCompleted"] = new LoadCompleted_Delegate(frameFocus.LoadCompleted);
                    _delegateTable["FrameFocus_ContentRendered"] = new ContentRendered_Delegate(frameFocus.ContentRendered);
                    _delegateTable["FrameFocus_Navigated"] = new Navigated_Delegate(frameFocus.Navigated);
                    break;

                case "FrameNavigationEvents":
                    FrameNavigationEvents frameNavigationEvents = new FrameNavigationEvents();
                    _delegateTable["FrameNavigationEvents_Startup"] = new Startup_Delegate(frameNavigationEvents.Startup);
                    _delegateTable["FrameNavigationEvents_LoadCompleted"] = new LoadCompleted_Delegate(frameNavigationEvents.LoadCompleted);
                    _delegateTable["FrameNavigationEvents_ContentRendered"] = new ContentRendered_Delegate(frameNavigationEvents.ContentRendered);
                    _delegateTable["FrameNavigationEvents_NavigationProgress"] = new NavigationProgress_Delegate(frameNavigationEvents.NavigationProgress);
                    _delegateTable["FrameNavigationEvents_Navigating"] = new Navigating_Delegate(frameNavigationEvents.Navigating);
                    _delegateTable["FrameNavigationEvents_NavigationStopped"] = new NavigationStopped_Delegate(frameNavigationEvents.NavigationStopped);
                    _delegateTable["FrameNavigationEvents_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(frameNavigationEvents.DispatcherUnhandledException);
                    break;

                case "FrameSourceNavigation":
                    FrameSourceNavigation frameSourceNavigation = new FrameSourceNavigation();
                    _delegateTable["FrameSourceNavigation_Startup"] = new Startup_Delegate(frameSourceNavigation.Startup);
                    _delegateTable["FrameSourceNavigation_LoadCompleted"] = new LoadCompleted_Delegate(frameSourceNavigation.LoadCompleted);
                    _delegateTable["FrameSourceNavigation_Navigating"] = new Navigating_Delegate(frameSourceNavigation.Navigating);
                    _delegateTable["FrameSourceNavigation_Navigated"] = new Navigated_Delegate(frameSourceNavigation.Navigated);
                    _delegateTable["FrameSourceNavigation_ContentRendered"] = new ContentRendered_Delegate(frameSourceNavigation.ContentRendered);
                    _delegateTable["FrameSourceNavigation_FragmentNavigation"] = new FragmentNavigation_Delegate(frameSourceNavigation.FragmentNavigation);
                    break;

                case "NavigateFrameInPF":
                    NavigateFrameInPF navigateFrameInPF = new NavigateFrameInPF();
                    _delegateTable["NavigateFrameInPF_Startup"] = new Startup_Delegate(navigateFrameInPF.Startup);
                    _delegateTable["NavigateFrameInPF_LoadCompleted"] = new LoadCompleted_Delegate(navigateFrameInPF.LoadCompleted);
                    _delegateTable["NavigateFrameInPF_ContentRendered"] = new ContentRendered_Delegate(navigateFrameInPF.ContentRendered);
                    break;

                case "NavigateFrameToObject":
                    NavigateFrameToObject navigateFrameToObject = new NavigateFrameToObject();
                    _delegateTable["NavigateFrameToObject_Startup"] = new Startup_Delegate(navigateFrameToObject.Startup);
                    _delegateTable["NavigateFrameToObject_LoadCompleted"] = new LoadCompleted_Delegate(navigateFrameToObject.LoadCompleted);
                    _delegateTable["NavigateFrameToObject_ContentRendered"] = new ContentRendered_Delegate(navigateFrameToObject.ContentRendered);
                    break;

                case "NestedFrameNavigation":
                    NestedFrameNavigation nestedFrameNavigation = new NestedFrameNavigation();
                    _delegateTable["NestedFrameNavigation_Startup"] = new Startup_Delegate(nestedFrameNavigation.Startup);
                    _delegateTable["NestedFrameNavigation_LoadCompleted"] = new LoadCompleted_Delegate(nestedFrameNavigation.LoadCompleted);
                    _delegateTable["NestedFrameNavigation_ContentRendered"] = new ContentRendered_Delegate(nestedFrameNavigation.ContentRenderedOuter);
                    break;

                case "FrameJournaling":
                    FrameJournaling frameJournaling = new FrameJournaling();
                    _delegateTable["FrameJournaling_Startup"] = new Startup_Delegate(frameJournaling.Startup);
                    _delegateTable["FrameJournaling_LoadCompleted"] = new LoadCompleted_Delegate(frameJournaling.LoadCompleted);
                    break;

                case "JournalOwnershipTest":
                    JournalOwnershipTest journalOwnershipTest = new JournalOwnershipTest();
                    _delegateTable["JournalOwnershipTest_Startup"] = new Startup_Delegate(journalOwnershipTest.Startup);
                    _delegateTable["JournalOwnershipTest_Navigated"] = new Navigated_Delegate(journalOwnershipTest.Navigated);
                    _delegateTable["JournalOwnershipTest_ContentRendered"] = new ContentRendered_Delegate(journalOwnershipTest.ContentRendered);
                    _delegateTable["JournalOwnershipTest_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(journalOwnershipTest.DispatcherUnhandledException);
                    break;

                case "FrameNavigateToNull":
                    FrameNavigateToNull frameNavigateToNull = new FrameNavigateToNull();
                    _delegateTable["FrameNavigateToNull_Startup"] = new Startup_Delegate(frameNavigateToNull.Startup);
                    _delegateTable["FrameNavigateToNull_LoadCompleted"] = new LoadCompleted_Delegate(frameNavigateToNull.LoadCompleted);
                    _delegateTable["FrameNavigateToNull_Navigated"] = new Navigated_Delegate(frameNavigateToNull.Navigated);
                    _delegateTable["FrameNavigateToNull_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(frameNavigateToNull.DispatcherUnhandledException);
                    break;

                // Navigation Window Test Cases
                case "NavigatePageToPage":
                    NavigatePageToPage navigatePageToPage = new NavigatePageToPage();
                    _delegateTable["NavigatePageToPage_LoadCompleted"] = new LoadCompleted_Delegate(navigatePageToPage.LoadCompleted);
                    _delegateTable["NavigatePageToPage_Startup"] = new Startup_Delegate(navigatePageToPage.Startup);
                    _delegateTable["NavigatePageToPage_Navigating"] = new Navigating_Delegate(navigatePageToPage.Navigating);
                    _delegateTable["NavigatePageToPage_Navigated"] = new Navigated_Delegate(navigatePageToPage.Navigated);
                    break;

                case "RefreshNavWin":
                    RefreshNavWin refreshNavWin = new RefreshNavWin();
                    _delegateTable["RefreshNavWin_Startup"] = new Startup_Delegate(refreshNavWin.Startup);
                    _delegateTable["RefreshNavWin_Navigating"] = new Navigating_Delegate(refreshNavWin.Navigating);
                    _delegateTable["RefreshNavWin_LoadCompleted"] = new LoadCompleted_Delegate(refreshNavWin.LoadCompleted);
                    _delegateTable["RefreshNavWin_ContentRendered"] = new ContentRendered_Delegate(refreshNavWin.ContentRendered);
                    break;

                // WebOC Test Cases
                case "WebOCGeneral":
                    WebOCGeneral webOCGeneral = new WebOCGeneral();
                    _delegateTable["WebOCGeneral_Startup"] = new Startup_Delegate(webOCGeneral.Startup);
                    _delegateTable["WebOCGeneral_Navigated"] = new Navigated_Delegate(webOCGeneral.Navigated);
                    _delegateTable["WebOCGeneral_LoadCompleted"] = new LoadCompleted_Delegate(webOCGeneral.LoadCompleted);
                    _delegateTable["WebOCGeneral_Navigating"] = new Navigating_Delegate(webOCGeneral.Navigating);
                    _delegateTable["WebOCGeneral_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(webOCGeneral.DispatcherUnhandledException);
                    break;

                case "OverlapWebOCFrames":
                    OverlapWebOCFrames overlapWebOCFrames = new OverlapWebOCFrames();
                    _delegateTable["OverlapWebOCFrames_Startup"] = new Startup_Delegate(overlapWebOCFrames.Startup);
                    _delegateTable["OverlapWebOCFrames_LoadCompleted"] = new LoadCompleted_Delegate(overlapWebOCFrames.LoadCompleted);
                    _delegateTable["OverlapWebOCFrames_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(overlapWebOCFrames.DispatcherUnhandledException);
                    break;

                // Journal Test Cases
                case "GoBackNW":
                    GoBackNW goBackNW = new GoBackNW();
                    _delegateTable["GoBackNW_Startup"] = new Startup_Delegate(goBackNW.Startup);
                    _delegateTable["GoBackNW_Navigating"] = new Navigating_Delegate(goBackNW.Navigating);
                    _delegateTable["GoBackNW_LoadCompleted"] = new LoadCompleted_Delegate(goBackNW.LoadCompleted);
                    _delegateTable["GoBackNW_Navigated"] = new Navigated_Delegate(goBackNW.Navigated);
                    break;

                case "GoForwardNW":
                    GoForwardNW goForwardNW = new GoForwardNW();
                    _delegateTable["GoForwardNW_Startup"] = new Startup_Delegate(goForwardNW.Startup);
                    _delegateTable["GoForwardNW_Navigating"] = new Navigating_Delegate(goForwardNW.Navigating);
                    _delegateTable["GoForwardNW_LoadCompleted"] = new LoadCompleted_Delegate(goForwardNW.LoadCompleted);
                    _delegateTable["GoForwardNW_Navigated"] = new Navigated_Delegate(goForwardNW.Navigated);
                    break;

                case "VerifyIETravelog":
                    VerifyIETravelog verifyIETravelog = new VerifyIETravelog();
                    _delegateTable["VerifyIETravelog_Startup"] = new Startup_Delegate(verifyIETravelog.Startup);
                    _delegateTable["VerifyIETravelog_Navigated"] = new Navigated_Delegate(verifyIETravelog.Navigated);
                    _delegateTable["VerifyIETravelog_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(verifyIETravelog.DispatcherUnhandledException);
                    break;

                case "NavigateToNull":
                    NavigateToNull navigateToNull = new NavigateToNull();
                    _delegateTable["NavigateToNull_Startup"] = new Startup_Delegate(navigateToNull.Startup);
                    _delegateTable["NavigateToNull_LoadCompleted"] = new LoadCompleted_Delegate(navigateToNull.LoadCompleted);
                    _delegateTable["NavigateToNull_Navigated"] = new Navigated_Delegate(navigateToNull.Navigated);
                    _delegateTable["NavigateToNull_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(navigateToNull.DispatcherUnhandledException);
                    break;

                // Hyperlink Test Cases
                case "HlinkNavigation":
                    HlinkNavigation hlinkNavigation = new HlinkNavigation();
                    _delegateTable["HlinkNavigation_Startup"] = new Startup_Delegate(hlinkNavigation.Startup);
                    _delegateTable["HlinkNavigation_Navigated"] = new Navigated_Delegate(hlinkNavigation.Navigated);
                    _delegateTable["HlinkNavigation_ContentRendered"] = new ContentRendered_Delegate(hlinkNavigation.ContentRendered);
                    _delegateTable["HlinkNavigation_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(hlinkNavigation.DispatcherUnhandledException);
                    break;

                case "HlinkTargeting":
                    HlinkTargeting hlinkTargeting = new HlinkTargeting();
                    _delegateTable["HlinkTargeting_Startup"] = new Startup_Delegate(hlinkTargeting.Startup);
                    _delegateTable["HlinkTargeting_LoadCompleted"] = new LoadCompleted_Delegate(hlinkTargeting.LoadCompleted);
                    _delegateTable["HlinkTargeting_Navigated"] = new Navigated_Delegate(hlinkTargeting.Navigated);
                    _delegateTable["HlinkTargeting_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(hlinkTargeting.DispatcherUnhandledException);
                    break;

                //HTML Interop cases
                case "HtmlInteropEvents":
                    HtmlInteropEvents htmlInteropEvents = new HtmlInteropEvents();
                    _delegateTable["HtmlInteropEvents_Startup"] = new Startup_Delegate(htmlInteropEvents.Startup);
                    _delegateTable["HtmlInteropEvents_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropEvents.LoadCompleted);
                    _delegateTable["HtmlInteropEvents_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropEvents.DispatcherUnhandledException);
                    break;

                case "HtmlInteropInvokeScript":
                    HtmlInteropInvokeScript htmlInteropInvokeScript = new HtmlInteropInvokeScript();
                    _delegateTable["HtmlInteropInvokeScript_Startup"] = new Startup_Delegate(htmlInteropInvokeScript.Startup);
                    _delegateTable["HtmlInteropInvokeScript_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropInvokeScript.LoadCompleted);
                    _delegateTable["HtmlInteropInvokeScript_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropInvokeScript.DispatcherUnhandledException);
                    break;

                case "HtmlInteropObjectForScripting":
                    HtmlInteropObjectForScripting htmlInteropObjectForScripting = new HtmlInteropObjectForScripting();
                    _delegateTable["HtmlInteropObjectForScripting_Startup"] = new Startup_Delegate(htmlInteropObjectForScripting.Startup);
                    _delegateTable["HtmlInteropObjectForScripting_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropObjectForScripting.LoadCompleted);
                    _delegateTable["HtmlInteropObjectForScripting_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropObjectForScripting.DispatcherUnhandledException);
                    break;

                case "HtmlInteropForwardBack":
                    HtmlInteropForwardBack htmlInteropForwardBack = new HtmlInteropForwardBack();
                    _delegateTable["HtmlInteropForwardBack_Startup"] = new Startup_Delegate(htmlInteropForwardBack.Startup);
                    _delegateTable["HtmlInteropForwardBack_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropForwardBack.LoadCompleted);
                    _delegateTable["HtmlInteropForwardBack_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropForwardBack.DispatcherUnhandledException);
                    break;

                case "HtmlInteropNavigate":
                    HtmlInteropNavigate htmlInteropNavigate = new HtmlInteropNavigate();
                    _delegateTable["HtmlInteropNavigate_Startup"] = new Startup_Delegate(htmlInteropNavigate.Startup);
                    _delegateTable["HtmlInteropNavigate_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropNavigate.LoadCompleted);
                    _delegateTable["HtmlInteropNavigate_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropNavigate.DispatcherUnhandledException);
                    break;

                case "HtmlInteropLoad":
                    HtmlInteropLoad htmlInteropLoad = new HtmlInteropLoad();
                    _delegateTable["HtmlInteropLoad_Startup"] = new Startup_Delegate(htmlInteropLoad.Startup);
                    _delegateTable["HtmlInteropLoad_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropLoad.LoadCompleted);
                    _delegateTable["HtmlInteropLoad_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropLoad.DispatcherUnhandledException);
                    break;

                case "HtmlInteropSecurity":
                    HtmlInteropSecurity htmlInteropSecurity = new HtmlInteropSecurity();
                    _delegateTable["HtmlInteropSecurity_Startup"] = new Startup_Delegate(htmlInteropSecurity.Startup);
                    _delegateTable["HtmlInteropSecurity_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropSecurity.LoadCompleted);
                    _delegateTable["HtmlInteropSecurity_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropSecurity.DispatcherUnhandledException);
                    break;

                case "HtmlInteropSubframes":
                    HtmlInteropSubframes htmlInteropSubframes = new HtmlInteropSubframes();
                    _delegateTable["HtmlInteropSubframes_Startup"] = new Startup_Delegate(htmlInteropSubframes.Startup);
                    _delegateTable["HtmlInteropSubframes_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropSubframes.LoadCompleted);
                    _delegateTable["HtmlInteropSubframes_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropSubframes.DispatcherUnhandledException);
                    break;

                case "HtmlInteropRefresh":
                    HtmlInteropRefresh htmlInteropRefresh = new HtmlInteropRefresh();
                    _delegateTable["HtmlInteropRefresh_Startup"] = new Startup_Delegate(htmlInteropRefresh.Startup);
                    _delegateTable["HtmlInteropRefresh_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropRefresh.LoadCompleted);
                    _delegateTable["HtmlInteropRefresh_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropRefresh.DispatcherUnhandledException);
                    break;

                case "HtmlInteropDocument":
                    HtmlInteropDocument htmlInteropDocument = new HtmlInteropDocument();
                    _delegateTable["HtmlInteropDocument_Startup"] = new Startup_Delegate(htmlInteropDocument.Startup);
                    _delegateTable["HtmlInteropDocument_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropDocument.LoadCompleted);
                    _delegateTable["HtmlInteropDocument_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropDocument.DispatcherUnhandledException);
                    break;


                //Regression test cases
                case "CredentialPolicyPreserved":
                    CredentialPolicyPreserved credentialPolicyPreserved = new CredentialPolicyPreserved();
                    _delegateTable["CredentialPolicyPreserved_Startup"] = new Startup_Delegate(credentialPolicyPreserved.Startup);
                    _delegateTable["CredentialPolicyPreserved_LoadCompleted"] = new LoadCompleted_Delegate(credentialPolicyPreserved.LoadCompleted);
                    _delegateTable["CredentialPolicyPreserved_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(credentialPolicyPreserved.DispatcherUnhandledException);
                    break;

                case "htmlinterop1":
                    htmlinterop1 htmlinterop1 = new htmlinterop1();
                    _delegateTable["htmlinterop1_Startup"] = new Startup_Delegate(htmlinterop1.Startup);
                    _delegateTable["htmlinterop1_LoadCompleted"] = new LoadCompleted_Delegate(htmlinterop1.LoadCompleted);
                    _delegateTable["htmlinterop1_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlinterop1.DispatcherUnhandledException);
                    break;

                case "HtmlInteropXXProc":
                    HtmlInteropXXProc htmlInteropXXProc = new HtmlInteropXXProc();
                    _delegateTable["HtmlInteropXXProc_Startup"] = new Startup_Delegate(htmlInteropXXProc.Startup);
                    _delegateTable["HtmlInteropXXProc_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropXXProc.LoadCompleted);
                    _delegateTable["HtmlInteropXXProc_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropXXProc.DispatcherUnhandledException);
                    break;

                case "Htmlinterop2":
                    Htmlinterop2 Htmlinterop2 = new Htmlinterop2();
                    _delegateTable["Htmlinterop2_Startup"] = new Startup_Delegate(Htmlinterop2.Startup);
                    _delegateTable["Htmlinterop2_LoadCompleted"] = new LoadCompleted_Delegate(Htmlinterop2.LoadCompleted);
                    _delegateTable["Htmlinterop2_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(Htmlinterop2.DispatcherUnhandledException);
                    break;

                case "Htmlinterop3":
                    Htmlinterop3 Htmlinterop3 = new Htmlinterop3();
                    _delegateTable["Htmlinterop3_Startup"] = new Startup_Delegate(Htmlinterop3.Startup);
                    _delegateTable["Htmlinterop3_LoadCompleted"] = new LoadCompleted_Delegate(Htmlinterop3.LoadCompleted);
                    _delegateTable["Htmlinterop3_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(Htmlinterop3.DispatcherUnhandledException);
                    break;

                case "Htmlinterop4":
                    Htmlinterop4 Htmlinterop4 = new Htmlinterop4();
                    _delegateTable["Htmlinterop4_Startup"] = new Startup_Delegate(Htmlinterop4.Startup);
                    _delegateTable["Htmlinterop4_LoadCompleted"] = new LoadCompleted_Delegate(Htmlinterop4.LoadCompleted);
                    _delegateTable["Htmlinterop4_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(Htmlinterop4.DispatcherUnhandledException);
                    break;

                case "Htmlinterop5":
                    Htmlinterop5 Htmlinterop5 = new Htmlinterop5();
                    _delegateTable["Htmlinterop5_Startup"] = new Startup_Delegate(Htmlinterop5.Startup);
                    _delegateTable["Htmlinterop5_LoadCompleted"] = new LoadCompleted_Delegate(Htmlinterop5.LoadCompleted);
                    _delegateTable["Htmlinterop5_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(Htmlinterop5.DispatcherUnhandledException);
                    break;

                case "HtmlInteropLMZLockdown":
                    HtmlInteropLMZLockdown htmlInteropLMZLockdown = new HtmlInteropLMZLockdown();
                    _delegateTable["HtmlInteropLMZLockdown_Startup"] = new Startup_Delegate(htmlInteropLMZLockdown.Startup);
                    _delegateTable["HtmlInteropLMZLockdown_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropLMZLockdown.LoadCompleted);
                    _delegateTable["HtmlInteropLMZLockdown_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropLMZLockdown.DispatcherUnhandledException);
                    break;

                case "HtmlInteropXXProcAlert":
                    HtmlInteropXXProcAlert htmlInteropXXProcAlert = new HtmlInteropXXProcAlert();
                    _delegateTable["HtmlInteropXXProcAlert_Startup"] = new Startup_Delegate(htmlInteropXXProcAlert.Startup);
                    _delegateTable["HtmlInteropXXProcAlert_LoadCompleted"] = new LoadCompleted_Delegate(htmlInteropXXProcAlert.LoadCompleted);
                    _delegateTable["HtmlInteropXXProcAlert_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(htmlInteropXXProcAlert.DispatcherUnhandledException);
                    break;

            }

            _delegateTable["NavigationEvents_Startup"] = new Startup_Delegate(NavigationEvents_Startup);
            _delegateTable["NavigationEvents_LoadCompleted"] = new LoadCompleted_Delegate(NavigationEvents_LoadCompleted);
            _delegateTable["NoStartupUri_Startup"] = new Startup_Delegate(NoStartupUri_Startup);
            _delegateTable["RedirectMitigation_Startup"] = new Startup_Delegate(RedirectMitigation_Startup);
            _delegateTable["RedirectMitigation_LoadCompleted"] = new LoadCompleted_Delegate(RedirectMitigation_LoadCompleted);
            _delegateTable["RedirectMitigation_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(RedirectMitigation_DispatcherUnhandledException);
            _delegateTable["SandboxExternalContent_Startup"] = new Startup_Delegate(SandboxExternalContent_Startup);
            _delegateTable["SandboxExternalContent_LoadCompleted"] = new LoadCompleted_Delegate(SandboxExternalContent_LoadCompleted);
            _delegateTable["SubclassNavContainer_Startup"] = new Startup_Delegate(SubclassNavContainer_Startup);
            _delegateTable["SubclassNavContainer_LoadCompleted"] = new LoadCompleted_Delegate(SubclassNavContainer_LoadCompleted);
            _delegateTable["ThreadedNavigateInvalid_Startup"] = new Startup_Delegate(ThreadedNavigateInvalid_Startup);
            _delegateTable["ThreadedNavigateInvalid_Exit"] = new Exit_Delegate(ThreadedNavigateInvalid_Exit);
            _delegateTable["ThreadedNavigateInvalid_Navigated"] = new Navigated_Delegate(ThreadedNavigateInvalid_Navigated);
            _delegateTable["ThreadedNavigateInvalid_LoadCompleted"] = new LoadCompleted_Delegate(ThreadedNavigateInvalid_LoadCompleted);
            _delegateTable["TimeDelayedNavigation_Startup"] = new Startup_Delegate(TimeDelayedNavigation_Startup);
            _delegateTable["TimeDelayedNavigation_LoadCompleted"] = new LoadCompleted_Delegate(TimeDelayedNavigation_LoadCompleted);
            _delegateTable["TimeDelayedNavigation_Navigating"] = new Navigating_Delegate(TimeDelayedNavigation_Navigating);
            _delegateTable["TimeDelayedNavigation_Navigated"] = new Navigated_Delegate(TimeDelayedNavigation_Navigated);
            #endregion

            #region NavigationWindow test cases
            // NavigationWindow
            _delegateTable["EvenMorePonies_Startup"] = new Startup_Delegate(EvenMorePonies_Startup);
            _delegateTable["EvenMorePonies_Navigating"] = new Navigating_Delegate(EvenMorePonies_Navigating);
            _delegateTable["EvenMorePonies_Navigated"] = new Navigated_Delegate(EvenMorePonies_Navigated);
            _delegateTable["EvenMorePonies_LoadCompleted"] = new LoadCompleted_Delegate(EvenMorePonies_LoadCompleted);
            _delegateTable["NavigatePageToWindow_Startup"] = new Startup_Delegate(NavigatePageToWindow_Startup);
            _delegateTable["NavigatePageToWindow_Navigating"] = new Navigating_Delegate(NavigatePageToWindow_Navigating);
            _delegateTable["NavigatePageToWindow_Navigated"] = new Navigated_Delegate(NavigatePageToWindow_Navigated);
            _delegateTable["NavigatePageToWindow_LoadCompleted"] = new LoadCompleted_Delegate(NavigatePageToWindow_LoadCompleted);
            _delegateTable["NavigateWinToPage_Startup"] = new Startup_Delegate(NavigateWinToPage_Startup);
            _delegateTable["NavigateWinToPage_Navigating"] = new Navigating_Delegate(NavigateWinToPage_Navigating);
            _delegateTable["NavigateWinToPage_Navigated"] = new Navigated_Delegate(NavigateWinToPage_Navigated);
            _delegateTable["NavigateWinToPage_LoadCompleted"] = new LoadCompleted_Delegate(NavigateWinToPage_LoadCompleted);
            _delegateTable["NavWinLoadCompleted_Startup"] = new Startup_Delegate(NavWinLoadCompleted_Startup);
            _delegateTable["NavWinLoadCompleted_LoadCompleted"] = new LoadCompleted_Delegate(NavWinLoadCompleted_LoadCompleted);
            _delegateTable["NavWinLoadCompleted_Navigated"] = new Navigated_Delegate(NavWinLoadCompleted_Navigated);
            _delegateTable["Ponies_Startup"] = new Startup_Delegate(Ponies_Startup);
            _delegateTable["Ponies_LoadCompleted"] = new LoadCompleted_Delegate(Ponies_LoadCompleted);
            _delegateTable["TapPage_Startup"] = new Startup_Delegate(TapPage_Startup);
            _delegateTable["TapPage_LoadCompleted"] = new LoadCompleted_Delegate(TapPage_LoadCompleted);
            _delegateTable["VerifyChrome_Startup"] = new Startup_Delegate(VerifyChrome_Startup);
            _delegateTable["VerifyChrome_Navigated"] = new Navigated_Delegate(VerifyChrome_Navigated);
            _delegateTable["NavigationFailed_Startup"] = new Startup_Delegate(NavigationFailed_Startup);
            _delegateTable["NavigationFailed_Navigating"] = new Navigating_Delegate(NavigationFailed_Navigating);
            _delegateTable["NavigationFailed_Navigated"] = new Navigated_Delegate(NavigationFailed_Navigated);
            _delegateTable["NavigationFailed_LoadCompleted"] = new LoadCompleted_Delegate(NavigationFailed_LoadCompleted);
            _delegateTable["NavigationFailed_NavigationFailed"] = new NavigationFailed_Delegate(NavigationFailed_NavigationFailed);
            _delegateTable["NavigationFailed_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(NavigationFailed_DispatcherUnhandledException);
            #endregion

            #region RootBrowserWindow test cases
            // ApplicationShutdown delegates
            _delegateTable["RBWApplicationShutdown_Startup"] = new Startup_Delegate(RBWApplicationShutdown_Startup);
            _delegateTable["RBWApplicationShutdown_LoadCompleted"] = new LoadCompleted_Delegate(RBWApplicationShutdown_LoadCompleted);

            // WindowActivate delegates
            _delegateTable["RBWWindowActivate_Startup"] = new Startup_Delegate(RBWWindowActivate_Startup);
            _delegateTable["RBWWindowActivate_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowActivate_LoadCompleted);

            // WindowClose delegates
            _delegateTable["RBWWindowClose_Startup"] = new Startup_Delegate(RBWWindowClose_Startup);
            _delegateTable["RBWWindowClose_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowClose_LoadCompleted);

            // WindowConstructor delegates
            _delegateTable["RBWWindowConstructor_Startup"] = new Startup_Delegate(RBWWindowConstructor_Startup);
            _delegateTable["RBWWindowConstructor_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowConstructor_LoadCompleted);

            // WindowDragMove delegates
            _delegateTable["RBWWindowDragMove_Startup"] = new Startup_Delegate(RBWWindowDragMove_Startup);
            _delegateTable["RBWWindowDragMove_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowDragMove_LoadCompleted);

            // WindowHide delegates
            _delegateTable["RBWWindowHide_Startup"] = new Startup_Delegate(RBWWindowHide_Startup);
            _delegateTable["RBWWindowHide_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowHide_LoadCompleted);

            // WindowShowActivated delegates
            _delegateTable["RBWWindowShowActivated_Startup"] = new Startup_Delegate(RBWWindowShowActivated_Startup);
            _delegateTable["RBWWindowShowActivated_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowShowActivated_LoadCompleted);

            // WindowShowDialog delegates
            _delegateTable["RBWWindowShowDialog_Startup"] = new Startup_Delegate(RBWWindowShowDialog_Startup);
            _delegateTable["RBWWindowShowDialog_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowShowDialog_LoadCompleted);

            // Dimensions delegates
            _delegateTable["RBWWindowDimensions_Startup"] = new Startup_Delegate(RBWWindowDimensions_Startup);

            // IsActive delegates (Don't need LoadCompleted since IsActive is no longer true in LoadCompleted.  Now registers GotKeyboardFocus in the Startup handler.
            _delegateTable["RBWWindowIsActive_Startup"] = new Startup_Delegate(RBWWindowIsActive_Startup);

            // Location delegates
            _delegateTable["RBWWindowLocation_Startup"] = new Startup_Delegate(RBWWindowLocation_Startup);
            _delegateTable["RBWWindowLocation_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowLocation_LoadCompleted);

            // MaxSize delegates
            _delegateTable["RBWWindowMaxSize_Startup"] = new Startup_Delegate(RBWWindowMaxSize_Startup);
            _delegateTable["RBWWindowMaxSize_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowMaxSize_LoadCompleted);

            // MinSize delegates
            _delegateTable["RBWWindowMinSize_Startup"] = new Startup_Delegate(RBWWindowMinSize_Startup);
            _delegateTable["RBWWindowMinSize_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowMinSize_LoadCompleted);

            // NavWinDimensions delegates
            _delegateTable["RBWWindowNavWinDimensions_Startup"] = new Startup_Delegate(RBWWindowNavWinDimensions_Startup);

            // Owner delegates
            _delegateTable["RBWWindowOwner_Startup"] = new Startup_Delegate(RBWWindowOwner_Startup);
            _delegateTable["RBWWindowOwner_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowOwner_LoadCompleted);

            // RestoreBounds delegates
            _delegateTable["RBWWindowRestoreBounds_Startup"] = new Startup_Delegate(RBWWindowRestoreBounds_Startup);
            _delegateTable["RBWWindowRestoreBounds_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowRestoreBounds_LoadCompleted);

            // Topmost delegates
            _delegateTable["RBWWindowTopmost_Startup"] = new Startup_Delegate(RBWWindowTopmost_Startup);
            _delegateTable["RBWWindowTopmost_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowTopmost_LoadCompleted);

            // Visibility delegates
            _delegateTable["RBWWindowVisibility_Startup"] = new Startup_Delegate(RBWWindowVisibility_Startup);
            _delegateTable["RBWWindowVisibility_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowVisibility_LoadCompleted);

            // WindowStyle delegates
            _delegateTable["RBWWindowStyle_Startup"] = new Startup_Delegate(RBWWindowStyle_Startup);
            _delegateTable["RBWWindowStyle_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowStyle_LoadCompleted);

            // WindowTitle delegates
            _delegateTable["RBWWindowTitle_Startup"] = new Startup_Delegate(RBWWindowTitle_Startup);
            _delegateTable["RBWWindowTitle_LoadCompleted"] = new LoadCompleted_Delegate(RBWWindowTitle_LoadCompleted);
            #endregion

            #region Hyperlink test cases
            // HlinkLocalFileAccess delegates
            _delegateTable["HlinkLocalFileAccess_Startup"] = new Startup_Delegate(HlinkLocalFileAccess_Startup);
            _delegateTable["HlinkLocalFileAccess_Navigated"] = new Navigated_Delegate(HlinkLocalFileAccess_Navigated);
            _delegateTable["HlinkLocalFileAccess_ContentRendered"] = new ContentRendered_Delegate(HlinkLocalFileAccess_ContentRendered);
            #endregion

            #region PageFunction test cases
            // BasicMarkupPF delegates
            _delegateTable["BasicMarkupPF_Startup"] = new Startup_Delegate(BasicMarkupPF_Startup);
            _delegateTable["BasicMarkupPF_LoadCompleted"] = new LoadCompleted_Delegate(BasicMarkupPF_LoadCompleted);

            // BasicPF delegates
            _delegateTable["BasicPF_Startup"] = new Startup_Delegate(BasicPF_Startup);

            // BasicUILessPF delegates
            _delegateTable["BasicUILessPF_Startup"] = new Startup_Delegate(BasicUILessPF_Startup);

            // ChildPropertyMultiplePF delegates
            _delegateTable["ChildPropertyMultiplePF_Startup"] = new Startup_Delegate(ChildPropertyMultiplePF_Startup);

            // ChildPropertyPF delegates
            _delegateTable["ChildPropertyPF_Startup"] = new Startup_Delegate(ChildPropertyPF_Startup);

            // DifferentTypesPF delegates
            _delegateTable["DifferentTypesPF_Startup"] = new Startup_Delegate(DifferentTypesPF_Startup);

            // FinishChildMarkupPF delegates
            _delegateTable["FinishChildMarkupPF_Startup"] = new Startup_Delegate(FinishChildMarkupPF_Startup);
            _delegateTable["FinishChildMarkupPF_LoadCompleted"] = new LoadCompleted_Delegate(FinishChildMarkupPF_LoadCompleted);

            // FinishPFWithNonEmptyFwdJournal delegates
            _delegateTable["FinishPFWithNonEmptyFwdJournal_Startup"] = new Startup_Delegate(FinishPFWithNonEmptyFwdJournal_Startup);
            _delegateTable["FinishPFWithNonEmptyFwdJournal_LoadCompleted"] = new LoadCompleted_Delegate(FinishPFWithNonEmptyFwdJournal_LoadCompleted);

            // GoingBackPF delegates
            _delegateTable["GoingBackPF_Startup"] = new Startup_Delegate(GoingBackPF_Startup);

            // GoingForwardPF delegates
            _delegateTable["GoingForwardPF_Startup"] = new Startup_Delegate(GoingForwardPF_Startup);

            // JournalingUILessPF delegates
            _delegateTable["JournalingUILessPF_Startup"] = new Startup_Delegate(JournalingUILessPF_Startup);

            // KeepAlivePF delegates
            _delegateTable["KeepAlivePF_Startup"] = new Startup_Delegate(KeepAlivePF_Startup);

            // MarkupPFNavigateToChild delegates
            _delegateTable["MarkupPFNavigateToChild_Startup"] = new Startup_Delegate(MarkupPFNavigateToChild_Startup);
            _delegateTable["MarkupPFNavigateToChild_LoadCompleted"] = new LoadCompleted_Delegate(MarkupPFNavigateToChild_LoadCompleted);

            // NavBackFwdFinishedChildPF delegates
            _delegateTable["NavBackFwdFinishedChildPF_Startup"] = new Startup_Delegate(NavBackFwdFinishedChildPF_Startup);

            // NavBackMultiplePF delegates
            _delegateTable["NavBackMultiplePF_Startup"] = new Startup_Delegate(NavBackMultiplePF_Startup);

            // NavCodeToMarkupPF delegates
            _delegateTable["NavCodeToMarkupPF_Startup"] = new Startup_Delegate(NavCodeToMarkupPF_Startup);

            // NavFwdMultiplePF delegates
            _delegateTable["NavFwdMultiplePF_Startup"] = new Startup_Delegate(NavFwdMultiplePF_Startup);

            // NavigateToChildPF delegates
            _delegateTable["NavigateToChildPF_Startup"] = new Startup_Delegate(NavigateToChildPF_Startup);

            // NoReturnEventHandlerPF delegates
            _delegateTable["NoReturnEventHandlerPF_Startup"] = new Startup_Delegate(NoReturnEventHandlerPF_Startup);
            _delegateTable["NoReturnEventHandlerPF_LoadCompleted"] = new LoadCompleted_Delegate(NoReturnEventHandlerPF_LoadCompleted);

            // PageToMarkupPF delegates
            _delegateTable["PageToMarkupPF_Startup"] = new Startup_Delegate(PageToMarkupPF_Startup);
            _delegateTable["PageToMarkupPF_LoadCompleted"] = new LoadCompleted_Delegate(PageToMarkupPF_LoadCompleted);

            // ParentPFStateOnNavigate delegates
            _delegateTable["ParentPFStateOnNavigate_Startup"] = new Startup_Delegate(ParentPFStateOnNavigate_Startup);

            // RemoveFromJournalFalsePF delegates
            _delegateTable["RemoveFromJournalFalsePF_Startup"] = new Startup_Delegate(RemoveFromJournalFalsePF_Startup);

            // RemoveFromJournalFinishChildMarkupPF delegates
            _delegateTable["RemoveFromJournalFinishChildMarkupPF_Startup"] = new Startup_Delegate(RemoveFromJournalFinishChildMarkupPF_Startup);
            _delegateTable["RemoveFromJournalFinishChildMarkupPF_LoadCompleted"] = new LoadCompleted_Delegate(RemoveFromJournalFinishChildMarkupPF_LoadCompleted);

            // RemoveFromJournalMarkupPF delegates
            _delegateTable["RemoveFromJournalMarkupPF_Startup"] = new Startup_Delegate(RemoveFromJournalMarkupPF_Startup);
            _delegateTable["RemoveFromJournalMarkupPF_LoadCompleted"] = new LoadCompleted_Delegate(RemoveFromJournalMarkupPF_LoadCompleted);

            // RemoveFromJournalTruePF delegates
            _delegateTable["RemoveFromJournalTruePF_Startup"] = new Startup_Delegate(RemoveFromJournalTruePF_Startup);

            // ReuseReturnedPF delegates
            _delegateTable["ReuseReturnedPF_Startup"] = new Startup_Delegate(ReuseReturnedPF_Startup);
            _delegateTable["ReuseReturnedPF_LoadCompleted"] = new LoadCompleted_Delegate(ReuseReturnedPF_LoadCompleted);

            // StartMethodFinishChildPF delegates
            _delegateTable["StartMethodFinishChildPF_Startup"] = new Startup_Delegate(StartMethodFinishChildPF_Startup);

            // StartMethodNavBackPF delegates
            _delegateTable["StartMethodNavBackPF_Startup"] = new Startup_Delegate(StartMethodNavBackPF_Startup);

            // StartMethodNavFwdPF delegates
            _delegateTable["StartMethodNavFwdPF_Startup"] = new Startup_Delegate(StartMethodNavFwdPF_Startup);

            // StartMethodPF delegates
            _delegateTable["StartMethodPF_Startup"] = new Startup_Delegate(StartMethodPF_Startup);

            // StockPageFunction delegates
            _delegateTable["StockPageFunction_Startup"] = new Startup_Delegate(StockPageFunction_Startup);

            // Regression case  
            _delegateTable["Regression10_Startup"] = new Startup_Delegate(Regression10.Regression10_Startup);
            _delegateTable["Regression10_DispatcherUnhandledException"] = new DispatcherUnhandledException_Delegate(Regression10.Regression10_DispatcherUnhandledException);

            // Regression case 
            _delegateTable["Regression9_Startup"] = new Startup_Delegate(Regression9_Startup);

            #endregion

            #region Journal test cases
            // CustomJournaling delegates
            _delegateTable["CustomJournaling_Startup"] = new Startup_Delegate(CustomJournaling_Startup);
            _delegateTable["CustomJournaling_Navigating"] = new Navigating_Delegate(CustomJournaling_Navigating);
            _delegateTable["CustomJournaling_LoadCompleted"] = new LoadCompleted_Delegate(CustomJournaling_LoadCompleted);
            _delegateTable["CustomJournaling_ContentRendered"] = new ContentRendered_Delegate(CustomJournaling_ContentRendered);

            // AddBackEntryOnEmptyNS delegates
            _delegateTable["AddBackEntryOnEmptyNS_Startup"] = new Startup_Delegate(AddBackEntryOnEmptyNS_Startup);

            // BackForwardApp delegates
            _delegateTable["BackForwardApp_Startup"] = new Startup_Delegate(BackForwardApp_Startup);
            _delegateTable["BackForwardApp_Navigating"] = new Navigating_Delegate(BackForwardApp_Navigating);
            _delegateTable["BackForwardApp_Navigated"] = new Navigated_Delegate(BackForwardApp_Navigated);
            _delegateTable["BackForwardApp_LoadCompleted"] = new LoadCompleted_Delegate(BackForwardApp_LoadCompleted);

            // ClearForwardStack delegates
            _delegateTable["ClearForwardStack_Startup"] = new Startup_Delegate(ClearForwardStack_Startup);
            _delegateTable["ClearForwardStack_LoadCompleted"] = new LoadCompleted_Delegate(ClearForwardStack_LoadCompleted);

            // DropdownMenusNavigate delegates -- 


            // ForwardBackStacks delegates
            _delegateTable["ForwardBackStacks_Startup"] = new Startup_Delegate(ForwardBackStacks_Startup);
            _delegateTable["ForwardBackStacks_LoadCompleted"] = new LoadCompleted_Delegate(ForwardBackStacks_LoadCompleted);

            // GoBackUriToPage delegates
            _delegateTable["GoBackUriToPage_Startup"] = new Startup_Delegate(GoBackUriToPage_Startup);
            _delegateTable["GoBackUriToPage_Navigating"] = new Navigating_Delegate(GoBackUriToPage_Navigating);
            _delegateTable["GoBackUriToPage_LoadCompleted"] = new LoadCompleted_Delegate(GoBackUriToPage_LoadCompleted);

            // JournalGoBack delegates
            _delegateTable["JournalGoBack_Startup"] = new Startup_Delegate(JournalGoBack_Startup);
            _delegateTable["JournalGoBack_Navigating"] = new Navigating_Delegate(JournalGoBack_Navigating);
            _delegateTable["JournalGoBack_LoadCompleted"] = new LoadCompleted_Delegate(JournalGoBack_LoadCompleted);

            // JournalGoForward delegates
            _delegateTable["JournalGoForward_Startup"] = new Startup_Delegate(JournalGoForward_Startup);
            _delegateTable["JournalGoForward_Navigating"] = new Navigating_Delegate(JournalGoForward_Navigating);
            _delegateTable["JournalGoForward_LoadCompleted"] = new LoadCompleted_Delegate(JournalGoForward_LoadCompleted);

            // MenuEntries delegates
            _delegateTable["MenuEntries_Startup"] = new Startup_Delegate(MenuEntries_Startup);
            _delegateTable["MenuEntries_ContentRendered"] = new ContentRendered_Delegate(MenuEntries_ContentRendered); 
            
            // MultipleFwd3 delegates
            _delegateTable["MultipleFwd3_Startup"] = new Startup_Delegate(MultipleFwd3_Startup);
            _delegateTable["MultipleFwd3_Navigating"] = new Navigating_Delegate(MultipleFwd3_Navigating);
            _delegateTable["MultipleFwd3_LoadCompleted"] = new LoadCompleted_Delegate(MultipleFwd3_LoadCompleted);

            // MultipleFwd4 delegates
            _delegateTable["MultipleFwd4_Startup"] = new Startup_Delegate(MultipleFwd4_Startup);
            _delegateTable["MultipleFwd4_Navigating"] = new Navigating_Delegate(MultipleFwd4_Navigating);
            _delegateTable["MultipleFwd4_LoadCompleted"] = new LoadCompleted_Delegate(MultipleFwd4_LoadCompleted);

            // ProgTreeJournal delegates
            _delegateTable["ProgTreeJournal_Startup"] = new Startup_Delegate(ProgTreeJournal_Startup);
            _delegateTable["ProgTreeJournal_Navigating"] = new Navigating_Delegate(ProgTreeJournal_Navigating);
            _delegateTable["ProgTreeJournal_Navigated"] = new Navigated_Delegate(ProgTreeJournal_Navigated);
            _delegateTable["ProgTreeJournal_LoadCompleted"] = new LoadCompleted_Delegate(ProgTreeJournal_LoadCompleted);

            // RemoveBackEntry delegates
            _delegateTable["RemoveBackEntry_Startup"] = new Startup_Delegate(RemoveBackEntry_Startup);
            _delegateTable["RemoveBackEntry_LoadCompleted"] = new LoadCompleted_Delegate(RemoveBackEntry_LoadCompleted);

            // XamlJournalEntry delegates
            _delegateTable["XamlJournalEntry_Startup"] = new Startup_Delegate(XamlJournalEntry_Startup);

            // Regression test  - Navigate from CustomContentState.Replay()
            _delegateTable["NavigateFromCCSReplay_Startup"] = new Startup_Delegate(NavigateFromCCSReplay_Startup);
            #endregion
        }
        #endregion

        #region Application default event handles (Test delegate routers)
        /************************************************************************
         * Default Application.Navigating event
         * To route to Navigating event delegate for the current specified test
         ************************************************************************/
        
        void OnAppNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (_delegateTable[_currentTest + "_Navigating"] != null)
            {
                ((Navigating_Delegate)_delegateTable[_currentTest + "_Navigating"])(sender, e);
            }
        }

        /************************************************************************
         * Default Application.Navigated event
         * To route to Navigated event delegate for the current specified test
         ************************************************************************/
        void OnAppNavigated(object sender, NavigationEventArgs e)
        {
            if (_delegateTable[_currentTest + "_Navigated"] != null)
            {
                ((Navigated_Delegate)_delegateTable[_currentTest + "_Navigated"])(sender, e);
            }
        }

        /************************************************************************
         * Default Application.NavigationFailed event
         * To route to NavigationFailed event delegate for the current specified test
         ************************************************************************/
        void OnAppNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (_delegateTable[_currentTest + "_NavigationFailed"] != null)
            {
                ((NavigationFailed_Delegate)_delegateTable[_currentTest + "_NavigationFailed"])(sender, e);
            }
        }

        /******************************************************************************
         * Default Application.NavigationProgress event
         * To route to NavigationProgress event delegate for the current specified test
         ******************************************************************************/
        void OnAppNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            if (_delegateTable[_currentTest + "_NavigationProgress"] != null)
            {
                ((NavigationProgress_Delegate)_delegateTable[_currentTest + "_NavigationProgress"])(sender, e);
            }
        }

        /*****************************************************************************
         * Default Application.NavigationStopped event
         * To route to NavigationStopped event delegate for the current specified test
         *****************************************************************************/
        void OnAppNavigationStopped(object sender, NavigationEventArgs e)
        {
            if (_delegateTable[_currentTest + "_NavigationStopped"] != null)
            {
                ((NavigationStopped_Delegate)_delegateTable[_currentTest + "_NavigationStopped"])(sender, e);
            }
        }

        /******************************************************************************
         * Default Application.FragmentNavigation event
         * To route to FragmentNavigation event delegate for the current specified test
         ******************************************************************************/
        void OnAppFragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            if (_delegateTable[_currentTest + "_FragmentNavigation"] != null)
            {
                ((FragmentNavigation_Delegate)_delegateTable[_currentTest + "_FragmentNavigation"])(sender, e);
            }
            if (_delegateTable[_currentTest + "_FragmentNavigationToObject"] != null)
            {
                ((FragmentNavigationToObject_Delegate)_delegateTable[_currentTest + "_FragmentNavigationToObject"])(sender, e);
            }
        }

      /******************************************************************************
       * Default Application.LoadCompleted event
       * To route to LoadCompleted event delegate for the current specified test
       ******************************************************************************/
        void OnAppLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_delegateTable[_currentTest + "_LoadCompleted"] != null)
            {
                ((LoadCompleted_Delegate)_delegateTable[_currentTest + "_LoadCompleted"])(sender, e);
            }

            // Register NavigationWindow's ContentRendered eventhandler
            if (_navWin == null)
            {
                _navWin = Application.Current.MainWindow as NavigationWindow;
                if (_navWin != null)
                    _navWin.ContentRendered += new EventHandler(OnNavWinContentRendered);
                else
                    NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        void OnAppExit(object sender, ExitEventArgs e)
        {
            if (_delegateTable[_currentTest + "_Exit"] != null)
            {
                ((Exit_Delegate)_delegateTable[_currentTest + "_Exit"])(sender, e);
            }

        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_delegateTable[_currentTest + "_DispatcherUnhandledException"] != null)
            {
                ((DispatcherUnhandledException_Delegate)_delegateTable[_currentTest + "_DispatcherUnhandledException"])(sender, e);
            }
        }

        /******************************************************************************
         * Default NavigationWindow.ContentRendered event
         * To route to ContentRendered event delegate for the current specified test
         ******************************************************************************/
        void OnNavWinContentRendered(object sender, EventArgs e)
        {
            if (_delegateTable[_currentTest + "_ContentRendered"] != null)
            {
                ((ContentRendered_Delegate)_delegateTable[_currentTest + "_ContentRendered"])(sender, e);
            }
        }       
        
        #endregion


    }
}
