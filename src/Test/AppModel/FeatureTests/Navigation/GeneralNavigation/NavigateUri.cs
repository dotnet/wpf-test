// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT to verify that absolute App uri doesn't show up in NavigationService Source, CurrentSource,
    /// and in the journal entries.
    /// We'll test the following uris (will remove this test cases once it is covered as part of modeling)
    /// a) absolute uri to compiled xaml included as part of app (pack://application,,,/Pages/Page2.xaml)
    /// b) relative uri to compiled xaml included as part of app (../Page1.xaml)
    /// c) absolute uri to xaml at site of origin (activation uri + "LoosePages/Page3.xaml" (example http://wpf..../LoosePages/Page3.xaml")
    /// d) relative uri xaml at site of origin (should fail) (..Page4.xaml - Page4.xaml is a loose file in the site of origin (activation uri))
    /// e) relative uri to xaml in component assembly included in app ("XamlAssembly;component/ComponentPages/PageA.xaml")
    /// f) relative (to current page) uri to xaml in component assembly included in app (should fail) (PageB.xaml - PageB is included in XamlAssembly;component\ComponentPages)
    /// g) absolute uri to xaml in component assembly in site of origin (later)
    /// Also checks BaseUri
    /// </summary>

    public partial class NavigationTests : Application
    {
        NavigationWindow _navigateUri_navWin = null;
        String _navigateUri_activationPath = null;

        enum NavigateUri_State
        {
            UnInit,
            Init,
            NavToCompiledXamlInDirectory,
            NavToCompiledXaml,
            NavToContentLooseXaml,
            NavToXamlAtSiteOfOriginInDir,
            NavToXamlInDirComponentAssembly,
            NavToXamlInComponentAssembly,
            NavToHTMLAtSiteOfOrigin,
            End
        };

        NavigateUri_State _navigateUri_currentState = NavigateUri_State.UnInit;

        String _expectedSource = String.Empty;

        String[] _expectedJournalEntries = {
            "NavigateUri (NavigateUri_Start.xaml)",
            "NavigateUri (Pages/NavigateUri_Page2.xaml)",
            "NavigateUri (NavigateUri_Page1.xaml)", 
            "NavigateUri (pack://siteoforigin:,,,/LoosePages/NavigateUri_Page3.xaml)",      // NOTE:  This is changed in the NavigateUri_Startup eventhandler
            "NavigateUri (NavigateUri_Page4.xaml)",
            @"NavigateUri (ComponentPages/PageA.xaml)",
            };

        String[] _expectedBaseUris = 
       {
            "pack://application:,,,/Pages/NavigateUri_Page2.xaml",
            "pack://application:,,,/NavigateUri_Page1.xaml",             
            "pack://siteoforigin:,,,/LoosePages/NavigateUri_Page3.xaml",            
            "pack://application:,,,/NavigateUri_Page4.xaml",
            @"pack://application:,,,/XamlComponent;component/ComponentPages/PageA.xaml"
       };
        void NavigateUri_Startup(object sender, StartupEventArgs e)
        {
            _navigateUri_currentState = NavigateUri_State.Init;
            NavigationHelper.SetStage(TestStage.Initialize);

            // Build proper URI activation path
            Uri activationUri;
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                activationUri = System.Windows.Interop.BrowserInteropHelper.Source;
            }
            else
            {
                activationUri = new Uri(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), UriKind.RelativeOrAbsolute);
            }
            NavigationHelper.Output("Detected activation URI = " + activationUri);

            String activationUriString = activationUri.ToString();
            int len = activationUriString.LastIndexOf('/');
            if (len == activationUriString.Length)
            {
                _navigateUri_activationPath = activationUriString.Substring(0, len + 1);
            }
            else
            {
                _navigateUri_activationPath = activationUriString;
            }

            // Replace the third entry in expectedJournalEntries with the proper translated site of origin
            _expectedJournalEntries[3] = "NavigateUri (" + _navigateUri_activationPath + "/LoosePages/NavigateUri_Page3.xaml)";


            NavigationHelper.SetStage(TestStage.Run);

            Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(NavigateUri_DispatcherException);
            StartupUri = new Uri("NavigateUri_Start.xaml", UriKind.RelativeOrAbsolute);
        }

        void NavigateUri_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_navigateUri_currentState)
            {
                case NavigateUri_State.Init:
                    _navigateUri_navWin = Application.Current.MainWindow as NavigationWindow;
                    _navigateUri_navWin.ContentRendered += new EventHandler(NavigateUri_ContentRendered);
                    break;
            }
        }

        void NavigateUri_ContentRendered(object sender, EventArgs e)
        {
            NavigateUriExecNextState();
        }

        void NavigateUriExecNextState()
        {
            switch (_navigateUri_currentState)
            {
                case NavigateUri_State.Init:

                    _navigateUri_currentState = NavigateUri_State.NavToCompiledXamlInDirectory;
                    _expectedSource = "Pages/NavigateUri_Page2.xaml";
                    _navigateUri_navWin.Navigate(new Uri(@"pack://application:,,,/Pages/NavigateUri_Page2.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case NavigateUri_State.NavToCompiledXamlInDirectory:
                    _navigateUri_currentState = NavigateUri_State.NavToCompiledXaml;
                    VerifySource();
                    _expectedSource = "NavigateUri_Page1.xaml"; 
                    _navigateUri_navWin.Navigate(new Uri(@"..\NavigateUri_Page1.xaml", UriKind.RelativeOrAbsolute));
                    break; 

                case NavigateUri_State.NavToCompiledXaml:
                    _navigateUri_currentState = NavigateUri_State.NavToXamlAtSiteOfOriginInDir;
                    VerifySource();
                    // Source property should remain as what we set it to.
                    _expectedSource = "pack://siteoforigin:,,,/LoosePages/NavigateUri_Page3.xaml";
                    _navigateUri_navWin.Source = new Uri("pack://siteoforigin:,,,/LoosePages/NavigateUri_Page3.xaml", UriKind.RelativeOrAbsolute);
                    break;

                case NavigateUri_State.NavToXamlAtSiteOfOriginInDir:
                    _navigateUri_currentState = NavigateUri_State.NavToContentLooseXaml;
                    VerifySource();
                    _expectedSource = "NavigateUri_Page4.xaml";
                    // ..\Page4.xaml works because we resolve against pack://application:,,,
                    // and since it is specified throught Content in project file this works
                    // 
                    _navigateUri_navWin.Navigate(new Uri(@"..\NavigateUri_Page4.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case NavigateUri_State.NavToContentLooseXaml:
                    _navigateUri_currentState = NavigateUri_State.NavToXamlInDirComponentAssembly;
                    VerifySource();
                    _expectedSource = @"XamlComponent;component/ComponentPages/PageA.xaml";
                    _navigateUri_navWin.Navigate(new Uri(@"XamlComponent;component/ComponentPages/PageA.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case NavigateUri_State.NavToXamlInDirComponentAssembly:
                    _navigateUri_currentState = NavigateUri_State.NavToXamlInComponentAssembly; 
                    VerifySource();
                    // should fail - relative uri to PageB.xaml (in component assembly)
                    _navigateUri_navWin.Navigate(new Uri(@"PageB.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case NavigateUri_State.NavToXamlInComponentAssembly:
                    _navigateUri_currentState = NavigateUri_State.NavToHTMLAtSiteOfOrigin;
                    // pack://siteoforigin:,,,/ doesn't work for htm - temporarily use activationpath		
                    // programmatic navigation of weboc should fail in partial trust
                    _expectedSource = Path.Combine(_navigateUri_activationPath, @"LoosePages\NavigateUri_Loose.html");
                    _navigateUri_navWin.Navigate(new Uri(Path.Combine(_navigateUri_activationPath, @"LoosePages\NavigateUri_Loose.html")));
                    break;

                case NavigateUri_State.NavToHTMLAtSiteOfOrigin:
                    _navigateUri_currentState = NavigateUri_State.End;
                    _navigateUri_navWin.Navigate("Verify Journal Entries");
                    break;

                case NavigateUri_State.End:
                    VerifyJournalEntries();

                    NavigationHelper.Pass("NavigateUri test passes");

                    NavigationHelper.SetStage(TestStage.Cleanup);

                    break;
            }
        }

        void VerifyJournalEntries()
        {
            // Special case for full-trust scenario (NavToHTMLAtSiteOfOrigin succeeds)
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                _navigateUri_navWin.RemoveBackEntry();
            }

            int i = 0;
            int index = _expectedJournalEntries.Length - 1;
            while (_navigateUri_navWin.CanGoBack)
            {
                IEnumerator backStackEnumerator = NavigationUtilities.GetBackStack(_navigateUri_navWin);
                backStackEnumerator.MoveNext();
                JournalEntry backTopEntry = backStackEnumerator.Current as JournalEntry;
                String actualName = backTopEntry.Name;
                NavigationHelper.Output("backstack entry = " + actualName);
                if (!_expectedJournalEntries[index - i].Equals(actualName))
                {
                    NavigationHelper.Output("Journal entry names don't match Actual = " + actualName
                            + " expected = " + _expectedJournalEntries[index - i]);
                    NavigationHelper.Fail("NavigateUri test fails");
                }
                _navigateUri_navWin.RemoveBackEntry();
                i++;
            }
        }

        int _baseUriIndex = 0;


        void VerifySource()
        {
            NavigationService ns = NavigationService.GetNavigationService(_navigateUri_navWin.Content as DependencyObject);
            NavigationHelper.Output("\nState = " + _navigateUri_currentState);
            String src = ns.Source.ToString();
            NavigationHelper.Output("\nnavService Source = " + src);
            if (!_expectedSource.Equals(src))
            {
                NavigationHelper.Output("Source mismatch. CurrentState = " + _navigateUri_currentState +
                    " expectedSource = " + _expectedSource);
                NavigationHelper.Fail("NavigateUri test fails");
            }
            Uri baseUri = BaseUriHelper.GetBaseUri(_navigateUri_navWin.Content as DependencyObject);
            NavigationHelper.Output("BaseUri = " + baseUri);
            if (!baseUri.ToString().Equals(_expectedBaseUris[_baseUriIndex]))
            {
                NavigationHelper.Output("BaseUri mismatch. CurrentState = " + _navigateUri_currentState +
                  " expected BaseUri = " + _expectedBaseUris[_baseUriIndex]);
                NavigationHelper.Fail("NavigateUri test fails");
            }
            ++_baseUriIndex;
        }

        void NavigateUri_DispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("NavigateUri_State = " + _navigateUri_currentState);
            switch (_navigateUri_currentState)
            {
                case NavigateUri_State.NavToHTMLAtSiteOfOrigin:
                    if (e.Exception is System.Security.SecurityException)
                    {
                        // test passed
                        e.Handled = true;
                        NavigateUriExecNextState();
                        return;
                    }
                    break;
                case NavigateUri_State.NavToXamlInComponentAssembly:
                    if (e.Exception is System.IO.IOException)
                    {
                        // test passed
                        e.Handled = true;
                        NavigateUriExecNextState();
                        return;
                    }
                    break;
            }
            // test failed						
            NavigationHelper.Fail("NavigateUri test fails");
        }
    }
}
