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

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{
    ///<summary>
    /// Create an HwndHost that hosts an HwndSource on a second thread.
    ///</summary>
    ///<remarks>
    ///     <filename>HwndHostThreaded.cs</filename>
    ///</remarks>
    [TestDefaults]
    public class HwndHostThreadedSimple: TestCase
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndHostThreadedSimple()
            : base(TestCaseType.HwndSourceSupport)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        [TestAttribute(0, @"Hosting\Threaded\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostThreadedSimple", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            try
            {
                StackPanel sp = new StackPanel();
                
                Button b = new Button();
                b.Content = "Local Button";
                sp.Children.Add(b);

                _host = new HwndHostThreaded();
                sp.Children.Add(_host);

                Source.RootVisual = sp;

                Microsoft.Test.Threading.ThreadingHelper.DispatcherTimerHelper(TimeSpan.FromSeconds(3), 
                    delegate(object o, EventArgs args)
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
            CoreLogger.EndVariation();
        }

        HwndHostThreaded _host = null;
    }
        
 }










