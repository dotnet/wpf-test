// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;
using Avalon.Test.Framework.Dispatchers;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.Framework.Dispatchers
{
    /******************************************************************************
    * CLASS:          HostingAvalonWithaDispatcher
    ******************************************************************************/
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
    ///</remarks>
    [Test(0, "Dispatcher", TestCaseSecurityLevel.FullTrust, "HostingAvalonWithaDispatcher")]
    public class HostingAvalonWithaDispatcher : TestCase
    {
        #region Data
        public static Window w;
        private Button _b;
        private AvalonHostedControl _ahc;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          HostingAvalonWithaDispatcher Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HostingAvalonWithaDispatcher() :base(TestCaseType.None)
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Testing Hosting a HwndHost that contains avalon with another HwndDispatcher and Context
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///      <li>Create a Win32Dispatcher</li>
        ///  </ol>
        ///     <filename>HwndDispatcherMultipleThreads.cs</filename>
        /// </remarks>
        TestResult StartTest()
        {
             AppTestHostingAvalon app = new AppTestHostingAvalon();
             app.Startup += new StartupEventHandler(OnStartup);
             app.Run();

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }     
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          OnStartup
        ******************************************************************************/
        // <summary>
        // </summary>        
        private void OnStartup(object sender, StartupEventArgs e) 
        {
            w = new Window();

            StackPanel panel = new StackPanel();

            w.Content = panel;

            _b = new Button();
            _b.Click+= new RoutedEventHandler(Click);

            _b.Content = "HOLA";
            _b.Width = 100;
            _b.Height = 100;
            panel.Children.Add(_b);
            _ahc = new AvalonHostedControl();

            panel.Children.Add(_ahc);

            w.Show();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(ClickButton), null);
        }

        /******************************************************************************
        * Function:          OnExit
        ******************************************************************************/
        private void OnExit(Object sender, ExitEventArgs e)
        {
          
        }

        /******************************************************************************
        * Function:          ClickButton
        ******************************************************************************/
        /// <summary>Clicks on the Button.</summary>
        private object ClickButton(object o)
        {
            MouseHelper.Click(_b);

            w.Close();
            return null;
        }
        
        /******************************************************************************
        * Function:          Click
        ******************************************************************************/
        /// <summary>Clicks on the Button.</summary>
        protected void Click(object o, RoutedEventArgs args)
        {
            MouseHelper.Click(_ahc.RootUIElement);            
        }
        #endregion
    }

    
    /******************************************************************************
    * CLASS:          AppTestHostingAvalon
    ******************************************************************************/
    /// <summary>
    /// </summary>
    public class AppTestHostingAvalon : Application
    {
        /// <summary>
        /// </summary>
        public AppTestHostingAvalon():base(){}      
    }
 }





