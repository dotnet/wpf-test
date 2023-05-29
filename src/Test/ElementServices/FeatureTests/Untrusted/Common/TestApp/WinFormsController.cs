// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// System
using System;
using System.Threading;
using System.Security.Permissions;

// Avalon
using System.Windows.Threading;

// Test
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Runs a message pump on the current thread, and performs variations 
    /// on a separate thread as they are sent from the ControllerProxy. 
    /// </summary>
    public sealed class WinFormsController : Controller
    {
        /// <summary>
        /// Runs a loop to handle variations. Also, runs a Dispatcher.
        /// </summary>
        public override void RunVariationLoop()
        {
            // Start the variation loop.
            this.StartLoop();

            // Run the message pump.
            Microsoft.Test.WindowsForms.Application.Run();
        }

        /// <summary>
        /// 
        /// </summary>
        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        public override void EndTest()
        {
            Microsoft.Test.WindowsForms.Application.Exit();
        }
    }
}

