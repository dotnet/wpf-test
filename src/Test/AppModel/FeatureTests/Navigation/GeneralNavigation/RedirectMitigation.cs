// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using Microsoft.Test;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Redirection to URI outside of SiteOfOrigin should fail
    /// Step 1: Navigate to RedirectMitigation_Page1.xaml that contains an html page inside a frame
    /// Step 2: When page is loaded verify the URI (pack://siteoforigin:,,,/test.html)
    /// Step 3: Navigate to an html page outside of SiteOfOrigin that browser alone should be able to open
    /// Step 4: Verify we get the security exception when navigating to this page from xbap
    /// </summary>

    public partial class NavigationTests : Application
    {
        String _redirectMitigation_startupUri = @"pack://siteoforigin:,,,/test.html";
        String _redirectMitigation_redirectExternalUri = "";
        Frame _redirectMitigation_navFrame = null;


        enum RedirectMitigation_State
        {
            Init,
            RedirectToSiteOfOriginHtml,
            RedirectToExternal
        }

        RedirectMitigation_State _redirectMitigation_currentState = RedirectMitigation_State.Init;
        void RedirectMitigation_Startup(object sender, StartupEventArgs e)
        {
            this.StartupUri = new Uri("RedirectMitigation_Page1.xaml", UriKind.RelativeOrAbsolute);
            _redirectMitigation_currentState = RedirectMitigation_State.RedirectToSiteOfOriginHtml;
            Log.Current.CurrentVariation.LogMessage("StartupUri is asp page that redirects to html page at site of origin - should pass");

            // set the external URI
            _redirectMitigation_redirectExternalUri = Microsoft.Test.Loaders.FileHost.HttpInternetBaseUrl + @"AppModel_NavigationTest/test.html";
            Log.Current.CurrentVariation.LogMessage("ExternalUri is " + _redirectMitigation_redirectExternalUri);
        }

        void RedirectMitigation_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Navigator is Frame)
            {
                if (_redirectMitigation_navFrame == null)
                {
                    _redirectMitigation_navFrame = e.Navigator as Frame;
                    _redirectMitigation_navFrame.ContentRendered += new EventHandler(OnContentRendered_RedirectMitigation_Frame);
                }

                switch (_redirectMitigation_currentState)
                {
                    case RedirectMitigation_State.RedirectToSiteOfOriginHtml:
                        if (e.Uri.ToString().Equals(_redirectMitigation_startupUri))
                        {
                            Log.Current.CurrentVariation.LogMessage("Got expected Startup Uri = " + e.Uri);
                        }
                        else
                        {
                            Log.Current.CurrentVariation.LogMessage("Did not get expected Startup Uri = " + e.Uri);
                            Log.Current.CurrentVariation.LogResult(Result.Fail);
                        }
                        break;
                }
            }
        }

        void OnContentRendered_RedirectMitigation_Frame(object sender, EventArgs e)
        {
            switch (_redirectMitigation_currentState)
            {
                case RedirectMitigation_State.RedirectToSiteOfOriginHtml:
                    Log.Current.CurrentVariation.LogMessage("Navigating to RedirectExternalUri = " + _redirectMitigation_redirectExternalUri);
                    _redirectMitigation_currentState = RedirectMitigation_State.RedirectToExternal;
                    _redirectMitigation_navFrame.Navigate(new Uri(_redirectMitigation_redirectExternalUri, UriKind.Absolute));
                    break;
   
                default:
                    break;
            }
        }

        void RedirectMitigation_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("Exception = " + e.Exception);
            if (_redirectMitigation_currentState == RedirectMitigation_State.RedirectToExternal &&
                e.Exception is System.Security.SecurityException)
            {
                // test pass  - got a securityexception on navigating
                // to a uri that does a server side redirect outside of site of origin
                Log.Current.CurrentVariation.LogMessage("Got expected SecurityException on redirection outside of Site of Origin");
                Log.Current.CurrentVariation.LogResult(Result.Pass);
                e.Handled = true;
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Got unexpected exception. Test fail");
                Log.Current.CurrentVariation.LogResult(Result.Fail);
            }
            Log.Current.CurrentVariation.Close();
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }

    }
}
