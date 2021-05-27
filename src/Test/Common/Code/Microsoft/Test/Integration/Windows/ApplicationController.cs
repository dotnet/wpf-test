// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// System
using System;

// Avalon
using System.Windows.Threading;

// Test
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.Integration.Windows
{
    /// <summary>
    /// Runs on a pre-existing Dispatcher thread, and performs variations 
    /// on a separate thread as they are sent from the ControllerProxy. 
    /// </summary>
    public sealed class ApplicationController : Controller
    {
        /// <summary>
        /// Runs a loop to handle variations. Assumes the current thread is already
        /// running a Dispatcher.  Works in a .Xbap or .Application process.
        /// </summary>
        public override void RunVariationLoop()
        {
            // Start the variation loop.
            this.StartLoop();
        }

        /// <summary>
        /// Ends test by notifying ApplicationMonitor.
        /// </summary>
        public override void EndTest()
        {
            ApplicationMonitor.NotifyStopMonitoring();
        }
    }

}

