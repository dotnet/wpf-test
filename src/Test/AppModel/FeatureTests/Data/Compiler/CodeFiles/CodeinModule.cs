// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Avalon.Test
{
    using System;
    
    using System.Windows.Navigation;
    using System.Windows;
    using System.Windows.Controls;
    
    using Microsoft.Test.Logging;


    public partial class MyApp : Application
    {
        
        TestLog _log = null;
         bool _browserhostedapp = false;

         protected override void OnStartup(System.Windows.StartupEventArgs e)
         {
             _log = Microsoft.Test.Logging.TestLog.Current; 
            // log.Stage = TestStage.Initialize;        

            if ( AppDomain.CurrentDomain.FriendlyName.ToString().Contains(".xbap") )        
            {
                _browserhostedapp = true;
            }

            base.OnStartup(e);
         }

        protected override void OnExit(ExitEventArgs e)
        {
            if ( _browserhostedapp == false )
            {
                _log.LogStatus("Shutting down application");
                // log.Close();
            }
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
