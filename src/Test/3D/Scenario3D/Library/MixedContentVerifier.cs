// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Utility to test 2D/3D content visually
    /// </summary>
    public class MixedContentVerifier : ScenarioTestVerifier
    {
        public MixedContentVerifier(Viewport3D vp, Color bg)
            : base(vp, bg)
        {
        }

        public void Verify(int verificationTime)
        {
            times = new int[] { verificationTime };
            if (verificationTime > 0)
            {
                captureTime = true;
            }

            // Recent product changes required test DLLs for this functionality.  While this is needed in the lab automated runs,
            //    it is not essential for repro scenarios.
#if USES_TEST_BINARIES   
            // Force the mouse out of the way, to avoid issues with mouseover states
            Microsoft.Test.Input.Input.SendMouseInput( 800, 600, 0,
                    Microsoft.Test.Input.SendMouseInputFlags.Absolute
                    | Microsoft.Test.Input.SendMouseInputFlags.Move );
#endif

            EnterVerificationLoop();
        }

        protected override void OnTimerEvent(object sender, System.EventArgs args)
        {
            switch (stageNumber++)
            {

                case 0:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(SetTimeCallback),
                            times[0]);
                    break;

                case 1:
                    // We want to override the background color so that we can blend correctly
                    // with the other capture later.
                    backgroundColor = Color.FromArgb(0x00, 0x00, 0x00, 0x00);
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(RenderFrameCallback),
                            null);
                    break;

                case 2:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(CaptureScreenOriginalCallback),
                            null);
                    break;

                case 3:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(Remove3DCallback),
                            null);
                    break;

                case 4:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(SetTimeCallback),
                            times[0]);
                    break;

                case 5:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(CaptureScreen2DCallback),
                            null);
                    break;

                case 6:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(VerifyTestCallback),
                            null);
                    break;

                case 7:
                    Dispatcher.CurrentDispatcher.Invoke(
                            DispatcherPriority.Normal,
                            new DispatcherOperationCallback(CleanUpCallback),
                            null);
                    break;

                default:
                    break;
            }
        }

        protected object CaptureScreen2DCallback(object o)
        {
            CaptureScreenCallback(o);
            _non3DCapture = capture;
            capture = null;
            return null;
        }

        protected object CaptureScreenOriginalCallback(object o)
        {
            CaptureScreenCallback(o);
            _originalCapture = capture;
            capture = null;
            return null;
        }

        protected object VerifyTestCallback(object o)
        {
            // Blt our 3D rendering to the 2D scene and compare
            RenderBuffer updatedResult = new RenderBuffer(_non3DCapture, backgroundColor);

            // We need to add tolerances to the background image.  This is so our background tolerance 
            //   level still works on non-rendered pixels.
            updatedResult.AddDefaultTolerances();

            // Blit the rendered scene onto the capture minus the viewport3D
            updatedResult.BitBlockTransfer(result, 0, 0);

            // Update internal expected result
            result = updatedResult;

            // Use default verification
            VerifyAgainstCapture("Mixed_Content", _originalCapture);
            return null;
        }

        protected object Remove3DCallback(object o)
        {
            // remember Viewport3D position
            _origin = viewport.TranslatePoint(new Point(0, 0), Application.Current.MainWindow);

            // Replace Viewport3D with empty color rectangle
            DependencyObject parent = this.viewport.Parent;
            if (parent is Page)
            {
                // destroy it
                ((Page)parent).Content = null;
            }
            if (parent is Panel)
            {
                Panel dp = parent as Panel;
                for (int i = 0; i < dp.Children.Count; i++)
                {
                    if (dp.Children[i] == viewport)
                    {
                        // destroy it
                        dp.Children.RemoveAt(i);
                        dp.Children.Add(new System.Windows.Controls.Border());
                    }
                }
            }
            return null;
        }

        Point _origin;
        Color[,] _non3DCapture = null;
        Color[,] _originalCapture = null;
    }
}