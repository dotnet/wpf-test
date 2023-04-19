// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;

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

            // log.Stage = Microsoft.Test.Logging.TestStage.Run;

            System.Reflection.Assembly exeAsm = System.Reflection.Assembly.GetExecutingAssembly();

            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("NoXaml.g", exeAsm);

            if (rm == null)
            {
                _log.LogEvidence("ResourceManager could not find NoXaml.g.resources");
                goto Exit;
            }

            rm.IgnoreCase = true;
            System.IO.UnmanagedMemoryStream stream = (System.IO.UnmanagedMemoryStream)rm.GetStream("picture1.jpg");            
            if (stream == null)
            {
                _log.LogEvidence("Could not get Picture1.jpg from ResourceManager");
                goto Exit;
            }

            _log.LogStatus("Creating Image and loading it to current window");
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = System.Windows.Media.Imaging.BitmapFrame.Create(stream);
            img.Loaded += new RoutedEventHandler(img_Loaded);

            //System.Windows.Application.Current.Windows[0].Content = img;
            Window win = new Window();
            win.Content = img;
            win.Show();

            return;

        Exit:
            _log.Result = Microsoft.Test.Logging.TestResult.Fail;

            ShutdownApp();
            
        }

         void OnLoad ( object sender, NavigationEventArgs e )
         {
              //log.LogStatus("LoadCompleted event fired - Pass");

//              // log.Stage = Microsoft.Test.Logging.TestStage.Cleanup;
//              ShutdownApp();
              
         }

        private void img_Loaded(object sender, RoutedEventArgs e)
        {
            _log.LogStatus("Image loaded event fired");

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

namespace TestDll {
        
  public class Executor 
  {

      [System.STAThread()]
      public static int Main(string[] args) 
      {
          System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);

          System.Windows.Application app = new OnStartupBVT();
          return app.Run();
      }
  }
}
