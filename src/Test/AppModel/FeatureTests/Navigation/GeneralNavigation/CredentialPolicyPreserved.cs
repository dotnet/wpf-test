// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Interop;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class is a regression test 
    /// AuthenticationManager.CredentialPolicy set by the user gets lost in NavigationService.cctor()
    /// This runs in full trust only.
    /// </summary>
    public class CredentialPolicyPreserved
    {
        #region Private Data
        Frame _frame = null;
        StackPanel _stackPanel = new StackPanel();
        
        // Disabling for .NET Core no IntranetZoneCredentialPolicy
        //IntranetZoneCredentialPolicy policy = null;

        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop Subframes tests");
            }

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri("CredentialPolicyPreserved_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_frame != null)
            {
                return;
            }
            else
            {
                _frame = new Frame();
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

                _stackPanel.Children.Add(_frame);

	        _frame.LoadCompleted += OnFrameLoadCompleted;
                _frame.ContentRendered += OnFrameContentRendered;

                //Set our own credential policy which WPF needs to respect instead of stomping on.
                // Disabling for .NET Core no IntranetZoneCredentialPolicy
                // policy = new IntranetZoneCredentialPolicy();
                // AuthenticationManager.CredentialPolicy = policy;

                //Uri doesn't need to exist, the credential policy code is called before we actually make the web request.
                //This keeps us from having to deploy any files to the test server.
                _frame.Source = new Uri("http://wpfapps/testscratch/Microsoft/bogus.htm");
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            
            // Disabling for .NET Core no IntranetZoneCredentialPolicy
            // if ((e.Exception is WebException) && (AuthenticationManager.CredentialPolicy == policy))
            // {
                // NavigationHelper.Pass("Expected WebException caught, and credential policy was preserved. Test passes.");
            // }
            // else
            // {
                NavigationHelper.Fail("Unexpected exception caught.  Expected WebException. Test fails.");
            //}
            e.Handled = true;
        }
        #endregion

        #region Private Members
        private void OnFrameLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("In frame LoadCompleted");

            NavigationHelper.Fail("We expected a dispatcher exception, not LoadCompleted.  Test fails.");
        }

        private void OnFrameContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("In frame ContentRendered");

            NavigationHelper.Fail("We expected a dispatcher exception, not ContentRendered.  Test fails.");
        }
        #endregion
    }
}
