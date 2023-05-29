// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Common;

using System.Windows.Markup;
using System.Runtime.InteropServices;
using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Source.Hwnd
{
    /// <summary>
    ///  Testing the HwndSource Hooks
    ///         
    /// </summary>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]
    [TestDefaults]
    public class SimpleTestHooks : TestCase
    {
        
        /// <summary>
        /// 
        /// </summary>
        public SimpleTestHooks() :base(TestCaseType.None){}
        
        /// <summary>
        /// Writing test case for 


        [Test(0, @"Source\Hooks", TestCaseSecurityLevel.FullTrust, "SimpleTestHooks", Description="Listen for WM_SETFOCUS and WM_KILLFOCUS on HwndSource hooks", Area = "AppModel")]
        override public void Run()
        {
            CoreLogger.BeginVariation();
            _vTrack = new ValidationTracking(4);
            _vTrack.AddValidation(0,"Hook didn't get WM_SETFOCUS on Source 1");
            _vTrack.AddValidation(1,"Hook didn't get WM_KILLFOCUS on Source 1");
            _vTrack.AddValidation(2,"Hook didn't get WM_SETFOCUS on Source 2");
            _vTrack.AddValidation(3,"Hook didn't get WM_KILLFOCUS on Source 2");
            

            MainDispatcher = Dispatcher.CurrentDispatcher;
                

            // Creating Two HwndSOurce on a Context
            testOne(null);

            // Setting Focus to HwndSource
            MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(startTest), null);

            // Quit the dispatcher          
            MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);

            Dispatcher.Run();

            // Destroy the HwndSource
            testOneEnd(null);

            _vTrack.ValidateAll();

            CoreLogger.EndVariation();
        }


        object testOne(object o)
        {
            Source = SourceHelper.CreateHwndSource(500, 500,0,0);

            _sourceTwo = SourceHelper.CreateHwndSource( 500, 500,0,0);

            Source.AddHook(new HwndSourceHook(listener));

            return null;
        }

        object testOneEnd(object o)
        {
            _sourceTwo.Dispose();
            Source.Dispose();
            return null;
        }


        object startTest(object o)
        {

            _startTest = true;

            NativeMethods.SetFocus(new HandleRef(null, Source.Handle));
            NativeMethods.SetFocus(new HandleRef(null, _sourceTwo.Handle));

            return null;
        }

        IntPtr listener(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (_startTest)
            {
                if (Source.Handle == hwnd)
                {
                    if (msg == NativeConstants.WM_SETFOCUS)
                    {
                        _vTrack.SetValue(0, true);
                    }

                    if (msg == NativeConstants.WM_KILLFOCUS)
                    {
                        _vTrack.SetValue(1, true);
                    }
                }

                if (_sourceTwo.Handle == _sourceTwo.Handle)
                {

                    if (msg == NativeConstants.WM_SETFOCUS)
                    {
                        _vTrack.SetValue(2, true);
                    }

                    if (msg == NativeConstants.WM_KILLFOCUS)
                    {
                        _vTrack.SetValue(3, true);
                    }
                }
            }

            handled = false;
            return IntPtr.Zero;

        }







        bool _startTest = false;
        HwndSource _sourceTwo;
 
        ValidationTracking _vTrack = null;
        
    }
}




