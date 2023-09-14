// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Threading; 
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Trusted;

namespace Avalon.Test.CoreUI.Trusted.Security
{
    ///<summary>
    ///</summary>
    ///<remarks>
    ///     <filename>BeginInvokeSecurityTest.cs</filename>
    ///</remarks>
    public class TestBeginInvokeSecurity 
    {
      
        ///<summary>
        ///</summary>
        public TestBeginInvokeSecurity()
        {
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        public void PostingFromUnTrustedDLL()
        {
            DDispatcher = Dispatcher.CurrentDispatcher;
            
            DDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(foo));

            Dispatcher.Run();

            if (s_exp == null)
                throw new Microsoft.Test.TestValidationException("SecurityException was not thrown");
        }


        /// <summary>
        /// 
        /// </summary>
        object foo (object o)
        {
            Assembly a = Assembly.LoadFrom("CoreTestsUntrusted.dll");
            Type t = a.GetType("Avalon.Test.CoreUI.Threading.Context.Queues.BeginInvoke.SecurityInsecureHelper");
            t.InvokeMember("PostingBadCode", BindingFlags.Public |BindingFlags.Static |BindingFlags.InvokeMethod,null,null,null);

            DDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (QuitItem), null);
            return null;
        }

        object QuitItem(object o)
        {            
            Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static object ReadDirectory (object o)
        {
            try
            {
                Console.WriteLine("Hacked " + System.IO.Directory.GetCurrentDirectory());
            }
            catch(SecurityException e)
            {
                s_exp = e;
            }
            
            return null;
        }
            
        static Exception s_exp = null;

        /// <summary>
        /// 
        /// </summary>
        static public Dispatcher DDispatcher;


    }
    
}



