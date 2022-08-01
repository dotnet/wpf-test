// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using DRT;
using System;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DRTAnimation
{
    public partial class NonVisualTestSuite : DrtTestSuite
    {
        void CompositionTargetTest()
        {
            //add a per frame callback
            CompositionTarget.Rendering += RenderingCallback;
            DRT.Suspend();

            _timeoutTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 3), 
                DispatcherPriority.Render,
                new EventHandler(TimerElapsed),
                Dispatcher.CurrentDispatcher);
        }

        private void RenderingCallback(object sender, EventArgs e)
        {
            TimeSpan renderingTime;
            _renderCount++;

            RenderingEventArgs args = e as RenderingEventArgs;

            DRT.Assert(args != null,
                "The Rendering event must pass in RenderingEventArgs");

            renderingTime = args.RenderingTime;

            DRT.Assert(renderingTime >= _lastRenderingTime,
                "The rendering time should always be monotonically increasing");

            _lastRenderingTime = renderingTime;

            // We should be able to get 20 callbacks before our timer expires.
            if (_renderCount == 20)
            {
                _timeoutTimer.Stop();

                CompositionTarget.Rendering -= RenderingCallback;

                DRT.Resume();
            }
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            throw new Exception("Rendering Callback failed to be called enough times");
        }

        private DispatcherTimer _timeoutTimer;
        private int _renderCount;
        private TimeSpan _lastRenderingTime;
    }
}
