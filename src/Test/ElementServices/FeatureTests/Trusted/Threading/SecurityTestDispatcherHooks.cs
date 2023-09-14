// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Security;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Trusted.Threading
{
    ///<summary>
    /// Test cases trying to execute code with higher privilages.
    ///</summary>
    public class SecurityTestDispatcherHooks 
    {

        /// <summary>
        /// The idea behind this test case is to test that a PT app that it is running a dispatcher on 
        /// a trusted dll, a untrusted dll add something on the hooks to try to execute code with a 
        /// higher privileges.
        /// </summary>
        public void TestOperationCompleted()
        {
            // Because CoreTrusted could not be built agains Base
            // I have to use reflection to execute a call (This reflection thing had nothing
            // to do with the test)
            // We call AddToDispatcherHook, this simulate a untrusted dll adding 
            // to the DispatcherHooks.OperationComplete event a handler to a trusted code, 
            // the reason is that the Untrusted code won't be on the stack when this event is raised.

            Dispatcher.CurrentDispatcher.UnhandledException += delegate(object o, DispatcherUnhandledExceptionEventArgs args)
            {
                if (args.Exception is SecurityException)
                {
                    args.Handled = true;
                    s_exceptionThrown = true;
                }
                DispatcherHelper.ShutDown();
            };

            Assembly assembly = Assembly.LoadFrom("CoreTestsUntrustedBase.dll");
            Type type = assembly.GetType("Avalon.Test.CoreUI.SecurityBaseUnTrustedHelper");
            type.InvokeMember("AddToDispatcherHook", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,null, null, null, System.Globalization.CultureInfo.InvariantCulture);


            // Enqueue the item that it will trigger the OperationComplete once
            // that the op is dispatched.
            DispatcherHelper.EnqueueBackgroundCallback( new DispatcherOperationCallback(ExecuteHandler),null);           

            DispatcherHelper.ShutDownBackground();                            
            Dispatcher.Run();
                
            if (!s_exceptionThrown)
            {
                CoreLogger.LogTestResult(false,"Security Exception was expected.");
            }
        }


        static bool s_exceptionThrown = false;

        private object ExecuteHandler(object o)
        {
            return null;
        }

        ///<summary>
        ///</summary>
        public static void GetCurrentDirectory(object sender, DispatcherHookEventArgs e)
        {
            System.IO.Directory.GetCurrentDirectory();
        }
        
    }
}

