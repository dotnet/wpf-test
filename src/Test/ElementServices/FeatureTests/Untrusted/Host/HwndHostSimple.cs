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
    ///</summary>
    ///<remarks>
    ///     <filename>RegisterTwiceSameContext.cs</filename>
    ///</remarks>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]    
    [TestDefaults]
    public class HwndHostSimple : TestCase
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public HwndHostSimple() :base(TestCaseType.HwndSourceSupport)
        {

        }

        IntPtr MessageCallback(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            
            handled = false;

            throw new Microsoft.Test.TestValidationException("This message should never be received.");

        }

        /// <summary>
        /// Creating 4 Threads: each thread creates it own dispatcher and context, later creates a canvas and button. Validate everything run fine.
        /// </summary>
        [TestAttribute(0, @"Hosting\Simple", TestCaseSecurityLevel.FullTrust, "HwndHostSimple", Area = "AppModel")]
        public override void Run()
        {
            CoreLogger.BeginVariation();
            TestCaseFailed = true;

            Win32ButtonElement host = null;



            Canvas canvas = new Canvas();               

            host = new Win32ButtonElement();
            host.MessageHook += new HwndSourceHook(MessageCallback);
            host.MessageHook -= new HwndSourceHook(MessageCallback);
            canvas.Children.Add(host);

            host.Painted += new EventHandler(Fired);



            Source.RootVisual = canvas;

            ExitDispatcheronTimeout(TimeSpan.FromSeconds(120), true);

            Dispatcher.Run();

            FinalReportFailure();
            CoreLogger.EndVariation();
        }

        void Fired(object t, EventArgs e)
        {

        
            if (!s_b)
            {
                TestCaseFailed = false;
                s_b = true;

                Win32ButtonElement button = t as Win32ButtonElement;
                button.ValidateHandle();


                ExitDispatcheronTimeout(TimeSpan.FromSeconds(2), false);

            }
        }

        static bool s_b  = false;

    }
        
 }










