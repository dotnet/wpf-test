// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Threading; 
using System.Windows.Threading;
using System.Windows;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Threading;

namespace Avalon.Test.CoreUI
{
    ///<summary>
    /// Helper code. This needs to live on an untrusted dll.
    ///</summary>
    public static class SecurityBaseUnTrustedHelper
    {
        ///<summary>
        /// This code is called from a test case that it is located on the trusted dll.
        ///</summary>
        public static void AddToDispatcherHook()
        {
            DispatcherHooks hook = DispatcherHelper.GetHooks();
            hook.OperationCompleted += new DispatcherHookEventHandler(SecurityTestDispatcherHooks.GetCurrentDirectory);
        }

    }
}




