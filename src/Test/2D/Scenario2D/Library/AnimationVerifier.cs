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
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Graphics
{
    public class AnimationVerifier : Verifier
    {
        public AnimationVerifier(Panel panel, Color bg, bool failonidentical)
            : base(panel, bg)
        {
            captureTime = true;
            _lastCapture = null;
            _failOnIdentical = failonidentical;
        }

        public void EnterAnimationLoop(int[] verificationTimes)
        {
            times = verificationTimes;
            EnterVerificationLoop();
        }

        protected override void OnTimerEvent(object sender, System.EventArgs args)
        {
            // exit and cleanup
            if (frameNumber >= times.Length)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(CleanUpCallback), null);
                return;
            }

            switch (stageNumber++)
            {
                case 0:
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(SetTimeCallback), times[frameNumber]);
                    break;

                case 1:
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(RenderFrameCallback), frameNumber);
                    break;

                case 2:
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(CaptureScreenCallback), null);
                    break;

                case 3:
                    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(VerifyTestCallback), frameNumber);
                    break;

                default:
                    stageNumber = 0;
                    frameNumber++;
                    break;
            }

        }

        protected object VerifyTestCallback(object o)
        {
            int frameNumber = (int)o;
            LogStatus("");
            LogStatus(String.Format("Verifying Frame #{0} at time={1}ms.", frameNumber, times[frameNumber]));

            bool framesChanged = VerifyFramesChanged(capture);

            if (!_failOnIdentical ||
                (_failOnIdentical && framesChanged))
            {
                VerifyAgainstCapture("Frame_" + frameNumber.ToString(), capture);
            }

            return null;
        }

        private bool VerifyFramesChanged(Color[,] capture)
        {
            bool framesChanged = true;

            if (_lastCapture == null)
            {
                // This is the first frame.
                goto Cleanup;
            }

            LogStatus("Verifying current frame is not identical to previous frame...");

            int width = capture.GetLength(0);
            int height = capture.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (_lastCapture[x, y] != capture[x, y])
                    {
                        goto Cleanup;
                    }
                }
            }
            AddFailure("Frames are identical... are we sure we're animating?");
            if (_failOnIdentical)
            {
                AddFailure("FAIL: Animation does not expect to remain a static view");
                failures++;
                framesChanged = false;
            }
            return framesChanged;

        Cleanup:
            _lastCapture = capture;
            return framesChanged;
        }

        private Color[,] _lastCapture;
        private bool _failOnIdentical;
    }
}