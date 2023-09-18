// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug44 
*/

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.VisualVerification;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Converters;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug44 
    /// </summary>
    [Test(1, "Regression", "Regression_Bug44",
        Area = "2D",
        Description = @"Regression test for Regression_Bug44 :  Hardware text not blending correctly in intermediate render targets and layered windows",
        SupportFiles = @"infra\TestApiCore.dll,FeatureTests\2D\Dev10\Masters\Regression_Bug44_KnownGood.png",
        Disabled=true
        )
    ]
    public class Regression_Bug44 : WindowTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Variation("Regression_Bug44_KnownGood.png")]
        public Regression_Bug44(string knownGoodFile)
            : base(true)
        {
            this._knownGood = Snapshot.FromFile(knownGoodFile);
            RunSteps += new TestStep(CreateWindowContent);
            RunSteps += new TestStep(Capture);
            RunSteps += new TestStep(Verify);
        }

        #region Test Methods
        /// <summary>
        /// Create window content.
        /// </summary>
        private TestResult CreateWindowContent()
        {
            // make a layered window
            Window.WindowStyle = System.Windows.WindowStyle.None;
            Window.Background = System.Windows.Media.Brushes.Transparent;

            TextBox box = new TextBox();
            box.Text = "XYZZY"; // standard debugging string from early 80's text adventures

            SolidColorBrush b = new SolidColorBrush(Colors.CornflowerBlue);
            b.Opacity = 0.5f;
            box.Foreground = b;

            box.BeginInit();
            box.Height = 100;
            box.Width = 100;
            box.EndInit();

            Window.Content = box;
            Window.BeginInit();
            Window.Height = 100;
            Window.Width = 100;
            Window.EndInit();

            return TestResult.Pass;
        }

        private TestResult Capture()
        {
            RenderSynchronization.WaitForRenderedFrames(this,1,1000);
            _captured = GrabWindowSnapshot(Window);
            return TestResult.Pass;
        }

        /// <summary>
        /// Attempt our repro steps.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            SnapshotColorVerifier verifier = new SnapshotColorVerifier();
            // crop out the outer pixels, now 4 instead of 2, because I am paranoid about window edges
            Rectangle crop = new System.Drawing.Rectangle(
                                                4,
                                                4,
                                                (int)_captured.Width - 8,
                                                (int)_captured.Height - 8);
            Snapshot croppedCaptured = _captured.Crop(crop);
            croppedCaptured.ToFile("captured.png", ImageFormat.Png); 
            Snapshot diff = _knownGood.CompareTo(croppedCaptured);    
            diff.ToFile("diff.png", ImageFormat.Png);

            VerificationResult result = verifier.Verify(diff);
            if (VerificationResult.Pass == result)
            {
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }
        #endregion


        #region Helpers
        /// <summary>
        /// Grab the start snapshot so that we can determine if things have changed.
        /// </summary>
        /// <returns>A snapshot of the window.</returns>
        private Snapshot GrabWindowSnapshot(Window w)
        {
            Rectangle rect = new System.Drawing.Rectangle(
                                                (int)w.Left,
                                                (int)w.Top,
                                                (int)w.Width,
                                                (int)w.Height);
            return Snapshot.FromRectangle(rect);
        }
        
        #endregion
        #region Members
        private Snapshot _captured;
        private Snapshot _knownGood;
        #endregion
    }
}