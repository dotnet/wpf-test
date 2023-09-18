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
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Utility to test video
    /// </summary>
    public class VideoVerifier : ScenarioTestVerifier
    {
        public VideoVerifier(Viewport3D vp, Color bg)
            : base(vp, bg)
        {
            captureTime = false;
        }

        public void EnterAnimationLoop(int[] verificationTimes, Color[] verificationColors)
        {
            times = verificationTimes;
            colors = verificationColors;
            EnterVerificationLoop();
        }

        protected override void EnterVerificationLoop()
        {
            // reset data
            frameNumber = 0;
            stageNumber = 0;
            errorCount = 0;
            passes = 0;
            failures = 0;

            // Use a timer to give Avalon time to refresh and adjust to the new 
            // frame time.  
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, times[0]);
            timer.Tick += new System.EventHandler(OnTimerEvent);
            timer.Start();
        }

        protected override void OnTimerEvent(object sender, System.EventArgs args)
        {
            // regenerate timer
            timer.Stop();

            DateTime before = DateTime.Now;

            // process things for this frame
            ProcessSingleFrame(times[frameNumber], colors[frameNumber]);

            // moeve to the next frame
            int nextFrame = ++frameNumber;

            // exit and cleanup
            if (nextFrame >= times.Length)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Normal, new DispatcherOperationCallback(CleanUpCallback), null);
                return;
            }

            DateTime after = DateTime.Now;

            int processingTime = (int)((before - after).Milliseconds);

            LogStatus(String.Format("Time spent verifying last frame: {0}ms", processingTime));

            // set up timer at next time stage
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, Math.Max(0, times[nextFrame] - processingTime));
            timer.Start();
        }

        virtual protected void ProcessSingleFrame(int time, Color color)
        {
            LogStatus("");
            LogStatus(String.Format(
                    "Verifying Frame #{0} at time delta of +{1}ms. Video expected color={2} CurrentTime={3}",
                    frameNumber,
                    time,
                    color,
                    DateTime.Now));

            // Capture the screen
            CaptureScreenCallback(null);

            int width = capture.GetLength(0);
            int height = capture.GetLength(1);
            // use renderbuffer for error difference estimation 
            result = new RenderBuffer(width, height, color);
            result.ClearToleranceBuffer(RenderTolerance.DefaultColorTolerance);

            // Use default verification
            VerifyAgainstCapture("VideoFrame_" + frameNumber.ToString(), capture);

        }

        protected Color[] colors;
    }
}