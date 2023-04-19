// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace TestDll
{
    using System;
    
    using System.Windows.Navigation;
    using System.Windows;
    using System.Windows.Controls;
    
    public partial class MyApp : Application
    {
        
     Microsoft.Test.Logging.TestLog _log = null;
     bool _browserhostedapp = false;

         protected override void OnStartup(System.Windows.StartupEventArgs e)
         {
            _log = Microsoft.Test.Logging.TestLog.Current; 
            // log.Stage = Microsoft.Test.Logging.TestStage.Initialize;

            if ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") )        
            {
                _browserhostedapp = true;
            }

            this.StartupUri = new Uri(@"Simple.xaml", UriKind.RelativeOrAbsolute);

            this.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(OnLoad);

            // log.Stage = Microsoft.Test.Logging.TestStage.Run;
            
            base.OnStartup(e);

         }

         void OnLoad ( object sender, NavigationEventArgs e )
         {
            _log.LogStatus("LoadCompleted event fired - Pass");
            _log.Result = Microsoft.Test.Logging.TestResult.Pass;

            ShutdownApp();                        
         }

        protected override void OnExit(ExitEventArgs e)
        {
            if ( _browserhostedapp == false )
            {
                _log.LogStatus("Shutting down application");
                // log.Close();
            }
        }

        private void ShutdownApp()
        {

            // log.Stage = Microsoft.Test.Logging.TestStage.Cleanup;

            if ( _browserhostedapp == false )        
            {
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                _log.LogStatus("Shutting down application");
                // log.Close();                    
            }            

            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();

        }

        public bool BrowserHostedApp
        {
            get
            {
                return _browserhostedapp;
            }
        }

    }
}
