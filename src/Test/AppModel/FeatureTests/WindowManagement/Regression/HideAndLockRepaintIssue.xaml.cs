// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
#if TESTBUILD_CLR40
using Microsoft.Test.VisualVerification;
#endif
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    // Description: 
    // Verify that if a window is hidden and the desktop is locked/unlocked, the window repaints.
    public partial class HideAndLockRepaintIssue
    {

        DispatcherTimer _timer = new DispatcherTimer();
#if TESTBUILD_CLR40
        Snapshot beforeShot;
#endif

        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            Logger.Status("Hiding window and causing the DX device to be lost to simulate locking the desktop");

            beforeShot = SnapshotHelper.SnapshotFromWindow(this, WindowSnapshotMode.IncludeWindowBorder);

            Hide();
            InitializeFullScreen();           
            ShutdownFullScreen();

            DependencyObject canvasObject = LogicalTreeHelper.FindLogicalNode(this, "MyOuterCanvas");
            Canvas myCanvas = (Canvas)canvasObject;
            myCanvas.Background = Brushes.Violet;

            canvasObject = LogicalTreeHelper.FindLogicalNode(this, "MyCanvas");
            myCanvas = (Canvas)canvasObject;
            myCanvas.Background = Brushes.Violet;

            timer.Interval = new TimeSpan(0,0,3);
            timer.Tick += FinishTest;
            timer.Start();

            Logger.Status("Re-showing window and verifying repaint happens");
            Show();
        }

        void FinishTest(object sender, EventArgs e)
        {
            timer.Stop();

            Snapshot afterShot = SnapshotHelper.SnapshotFromWindow(this, WindowSnapshotMode.IncludeWindowBorder);
            Snapshot diff = afterShot.CompareTo(beforeShot);

            //we expect the after to NOT match before, since we changed the canvas background color.
            SnapshotVerifier verifier = new SnapshotColorVerifier(System.Drawing.Color.Black, new ColorDifference(0, 0, 0, 0));
            if (verifier.Verify(diff) == VerificationResult.Pass)
            {
                Logger.LogFail("The window unexpectedly didn't repaint.");
            }
            else
            {
                Logger.LogPass("As expected, the window repainted properly after simulated desktop lock/unlock.");
            }
#endif

            Close();
        }

        #region Full Screen API

        /// <summary>
        /// Initializes a full screen application
        /// </summary>
        /// <returns></returns>
        [DllImport("NativeD3DApp.dll")]
        public static extern bool InitializeFullScreen();

        /// <summary>
        /// Shuts down a previously created full screen application
        /// </summary>
        [DllImport("NativeD3DApp.dll")]
        public static extern void ShutdownFullScreen();

        #endregion

    }
}
