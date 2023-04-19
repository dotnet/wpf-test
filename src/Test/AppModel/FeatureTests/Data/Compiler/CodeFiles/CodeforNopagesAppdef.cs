// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TestDll
{
    public class OnStartupBVT : System.Windows.Application
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

              NavigationWindow win = new NavigationWindow();              
//              FlowPanel fp = new FlowPanel(win.Context);
//              fp.Background = System.Windows.Media.Brushes.OrangeRed;

              // log.Stage = Microsoft.Test.Logging.TestStage.Run;

              _log.LogStatus("Adding Textbox to window");
              TextBox textbox = new TextBox();
              textbox.Text = "Test started";
              textbox.Name = "textbox";
              textbox.Background = System.Windows.Media.Brushes.BlanchedAlmond;
              textbox.Loaded += new EventHandler(OnRender);
              
              win.Content = (textbox);
              _log.LogStatus("Show window");
              win.Show();
              
              base.OnStartup(e);
         }

         void OnRender ( object sender, EventArgs e )
         {
            _log.LogStatus("Loaded event fired - Pass");
            System.Threading.Thread.Sleep(2000);
            
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


namespace TestDll {
    
    
    public class Executor {
        
        [System.STAThread()]
        public static int Main(string[] args) {

            System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;
            System.Windows.Application app = new OnStartupBVT();
            return app.Run(args);
        }
    }
}
