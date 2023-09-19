// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Graphics
{
    public class ShutdownVerifier : ScenarioTestVerifier
    {
        public ShutdownVerifier(Viewport3D vp, Color bg)
            : base(vp, bg)
        {
        }

        public void SetExitTime(int runningTimeInMs)
        {
            delayInMilliseconds = runningTimeInMs;
            EnterVerificationLoop();
        }

        protected override void OnTimerEvent(object sender, System.EventArgs args)
        {
            // log a single pass
            passes = 1;
            Dispatcher.CurrentDispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new DispatcherOperationCallback(CleanUpCallback),
                    null);
        }
    }
}