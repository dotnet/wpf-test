// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Trusted;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Threading
{

    ///<summary>
    /// Main Class that holds the BVTS for Dispatcher Class
    ///</summary>
    [TestDefaults]
    public class DispatcherBVTs : TestCaseBase
    {
        ///<summary>
        ///</summary>
        [Test(0, @"Threading\Dispatcher", TestCaseSecurityLevel.PartialTrust, "In a single thread application, the current thread needs" +
        " to grab the dispatcher, calling Dispatcher.CurrentDispatcher API", Area="ElementServices")]
        public void CurrentDispatcher_ScenarioOne()
        {
            
            this.MainDispatcher = Dispatcher.CurrentDispatcher;

            if (this.MainDispatcher != Dispatcher.CurrentDispatcher)
                throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match with the cached dispatcher");

            DispatcherHelper.EnqueueBackgroundCallback(
                this.MainDispatcher,
                new DispatcherOperationCallback(currentDispatcher_scenarioOne_Handler),
                null);                            
        }

        object currentDispatcher_scenarioOne_Handler(object o)
        {
            if (this.MainDispatcher != Dispatcher.CurrentDispatcher)
                throw new Microsoft.Test.TestValidationException("The dispatcher doesn't match with the cached dispatcher");            

            DispatcherHelper.ShutDown();
            
            return null;
        }
    }
}




