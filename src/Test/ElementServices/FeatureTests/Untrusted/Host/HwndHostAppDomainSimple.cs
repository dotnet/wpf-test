// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Controls;

//using Avalon.Test.Framework.Dispatchers;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Win32;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class HwndHostAppDomainSimple : TestCase
    {
        

        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndHostAppDomainSimple() :base(TestCaseType.HwndSourceSupport)
        {

        }

        /// <summary>
        /// Creating 4 Threads: each thread creates it own dispatcher and context, later creates a canvas and button. Validate everything run fine.
        /// </summary>
        /// <remarks>
        ///  <ol>Description Steps:
        ///     <li>Spwan 4 Threads and wait for all the thread finished their work.</li>
        ///  </ol>
	    ///     <filename>MultipleContextMultipleDispatcherMultiple.cs</filename>
        /// </remarks>
        [Test(0, @"Hosting\AppDomain\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostAppDomainSimple", Area = "AppModel")]
        public override void Run()
        {
            StackPanel sp = new StackPanel();               
            _host = new AvalonHostIsolated();

            try
            {
                Button b = new Button();
                b.Content = "Local Button";
                
                sp.Children.Add(b);
                sp.Children.Add(_host);
                   
                Source.RootVisual = sp;

                MouseHelper.Click(_host);

                Microsoft.Test.Threading.ThreadingHelper.DispatcherTimerHelper(TimeSpan.FromSeconds(3), delegate(object o, EventArgs args)
                    {
                        Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Background);
                    }, null);
                Dispatcher.Run();
            }
            finally
            {
                _host.StopControl();
            }

            if (_host.InternalException != null)
            {
                CoreLogger.LogTestResult(false,"Exception was caught");
                CoreLogger.LogStatus(_host.InternalException.Message);
                CoreLogger.LogStatus(_host.InternalException.StackTrace.ToString());
            }
            
        }

        AvalonHostIsolated _host = null;


        
    }
        
 }










