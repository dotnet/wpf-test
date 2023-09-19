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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    public abstract class ScenarioTestVerifier
    {
        public ScenarioTestVerifier(Viewport3D vp, Color bg)
        {
            _logger = Logger.Create();
            LogStatus("Verifier Created... ");

            viewport = vp;
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

            int numAllowableMismatches = -1;   // Number of pixel mismatches that may be ignored
            string VScanToleranceFile  = null; // Path to VScan tolerance file (if any)

            Int32.TryParse((string)Application.Current.Properties["NumAllowableMismatches"], out numAllowableMismatches);
            VScanToleranceFile = (string)Application.Current.Properties["VScanToleranceFile"];

            int currentFailures = RenderVerifier.VerifyRender(capture, result, numAllowableMismatches, VScanToleranceFile);
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
            PhotoConverter.SaveImageAs(diff.FrameBuffer, filePrefix + "_Diff_fb.png", false);
            PhotoConverter.SaveImageAs(capture, filePrefix + "_Rendered.png");
            PhotoConverter.SaveImageAs(PhotoConverter.ToColorArray(result.ZBuffer),
                                        filePrefix + "_DepthBuffer.png");

            bool saveXamlRepro = false;
            Application application = Application.Current as Application;
            if (application.Properties["SaveXamlRepro"] != null)
            {
                saveXamlRepro = bool.Parse(application.Properties["SaveXamlRepro"] as string);
            }
            if (saveXamlRepro)
            {
                Point[] failPoints = RenderVerifier.GetPointsWithFailures(capture, result);

                SceneRenderer renderer = GetSceneRendererForScene();

                renderer.SaveSelectedSubSceneAsXaml(failPoints, filePrefix + "_XamlRepro.xaml");
                _logger.LogStatus("Failing triangles repro saved as: " + filePrefix + "_XamlRepro.xaml");
            }
        }

        protected object CleanUpCallback(object o)
        {
            LogStatus("Cleaning up ...");
            timer.Stop();
            if (captureTime)
            {
                clock.ReleaseTime();
            }
            _logger.Close();
            Logger.LogFinalResults(failures, passes + failures);
            Application.Current.Shutdown();
            return null;
        }

        protected object RenderFrameCallback(object o)
        {
            LogStatus("Invoking SceneRenderer ... ");
            DateTime before = DateTime.Now;

            SceneRenderer renderer = GetSceneRendererForScene();

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

        protected SceneRenderer GetSceneRendererForScene()
        {
            Window w = Application.Current.MainWindow;
            FrameworkElement fe = w.Content as FrameworkElement;
            double windowWidth;
            double windowHeight;
            if (fe != null)
            {
                // Get the window's Width/Height
                windowWidth = MathEx.FallbackIfNaN(fe.Width, fe.ActualWidth);
                windowHeight = MathEx.FallbackIfNaN(fe.Height, fe.ActualHeight);
            }
            else
            {
                // The viewport's Width/Height are the same as the window's Width/Height
                windowWidth = MathEx.FallbackIfNaN(viewport.Width, viewport.ActualWidth);
                windowHeight = MathEx.FallbackIfNaN(viewport.Height, viewport.ActualHeight);
            }
            Size windowSize = new Size(windowWidth, windowHeight);

            return new SceneRenderer(windowSize, viewport, backgroundColor);
        }

        protected AnimationClock clock;
        protected DispatcherTimer timer;
        protected Viewport3D viewport;
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