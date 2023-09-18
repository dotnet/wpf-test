// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Microsoft.Test.Graphics;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    public abstract class Verifier
    {
        public Verifier(Panel p, Color bg)
        {
            _logger = Logger.Create();
            LogStatus("Verifier Created... ");

            panel = p;
            backgroundColor = bg;

            captureTime = false;
        }

        virtual protected void EnterVerificationLoop()
        {
            // reset data
            frameNumber = 0;
            stageNumber = 0;
            errorCount = 0;
            passes = 0;
            failures = 0;

            if (captureTime)
            {
                clock = new AnimationClock();
                clock.CaptureTime();
            }

            // Use a timer to give Avalon time to refresh and adjust to the new 
            //      frame time.  1000ms is a reasonable default.
            timer = new DispatcherTimer();
            timer.Interval = new System.TimeSpan(0, 0, 0, 0, delayInMilliseconds);
            timer.Tick += new System.EventHandler(OnTimerEvent);
            timer.Start();

        }

        abstract protected void OnTimerEvent(object sender, System.EventArgs args);

        protected void VerifyAgainstCapture(string name, Color[,] capture)
        {
            LogStatus(String.Format("Verifying Scenario {0} ...", name));

            int currentFailures = RenderVerifier.VerifyRender(capture, result);
            errorCount += currentFailures;
            LogStatus(RenderVerifier.GetErrorStatistics(result));

            if (currentFailures > 0)
            {
                AddFailure(String.Format("{1} FAILED {0} time(s).", currentFailures, name));
                failures++;
            }
            else
            {
                LogStatus(String.Format("{0} PASSED.", name));
                passes++;
            }
            LogStatus("");
            LogStatus("==============================");
            LogStatus("==============================");

            RenderBuffer diff = RenderVerifier.ComputeDifference(capture, result);
            string filePrefix = ScenarioUtility.CurrentTestPrefix + "_" + name;
            PhotoConverter.SaveImageAs(result.FrameBuffer, filePrefix + "_Expected_fb.png");
            PhotoConverter.SaveImageAs(result.ToleranceBuffer, filePrefix + "_Expected_tb.png", false);
            PhotoConverter.SaveImageAs(diff.ToleranceBuffer, filePrefix + "_Diff_tb.png", false);
            PhotoConverter.SaveImageAs(capture, filePrefix + "_Rendered.png");

        }

        protected object CleanUpCallback(object o)
        {
            LogStatus("Cleaning up ...");
            _logger.Close();
            timer.Stop();
            if (captureTime)
            {
                clock.ReleaseTime();
            }
            Logger.LogFinalResults(failures, passes + failures);
            Application.Current.Shutdown();
            return null;
        }

        protected object RenderFrameCallback(object o)
        {
            LogStatus("Invoking SceneRenderer ... ");
            DateTime before = DateTime.Now;

            SceneRenderer2D renderer = GetSceneRendererForScene();

            result = renderer.Render();
            DateTime after = DateTime.Now;
            LogStatus("DONE. Render time= " + (after - before).ToString());
            return null;
        }

        protected object SetTimeCallback(object o)
        {
            if (captureTime)
            {
                LogStatus("Setting time=" + o.ToString());
                clock.CurrentTime = TimeSpan.FromMilliseconds((double)(int)o);
            }
            return null;
        }

        protected object CaptureScreenCallback(object o)
        {
            LogStatus("Taking screen capture ...");
            WindowInteropHelper helper = new WindowInteropHelper(Application.Current.MainWindow);
            Photographer photographer = new Photographer();
            System.Drawing.Bitmap bitmap = photographer.TakeScreenCapture(helper.Handle);
            capture = PhotoConverter.ToColorArray(bitmap);
            return null;
        }

        protected void LogStatus(string s)
        {
            _logger.LogStatus(s);
        }

        protected void AddFailure(string s)
        {
            _logger.AddFailure(s);
        }

        protected SceneRenderer2D GetSceneRendererForScene()
        {

            Window w = Application.Current.MainWindow;
            FrameworkElement fe = w.Content as FrameworkElement;
            double windowWidth;
            double windowHeight;

            // Get the window's Width/Height
            windowWidth = MathEx.FallbackIfNaN(fe.Width, fe.ActualWidth);
            windowHeight = MathEx.FallbackIfNaN(fe.Height, fe.ActualHeight);

            Size windowSize = new Size(windowWidth, windowHeight);

            return new SceneRenderer2D(windowSize, panel, backgroundColor);
        }


        protected AnimationClock clock;
        protected DispatcherTimer timer;
        protected Panel panel;
        protected Color backgroundColor;
        protected int frameNumber;
        protected int stageNumber;
        protected int[] times;

        protected Color[,] capture;
        protected RenderBuffer result;

        protected int passes;
        protected int failures;
        protected int errorCount;
        protected bool captureTime;

        private Logger _logger;

        protected int delayInMilliseconds = 1000;

    }
}