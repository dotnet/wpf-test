// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug43 
*/
using System.ComponentModel.Design.Serialization;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Converters;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.RenderingVerification;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug43 
    /// </summary>
    // commented ignored tests
    // [Test(1, "Regression", "Regression_Bug43",
    //     Area = "2D",
    //     Description = @"Regression test for Regression_Bug43 : CleartypeHint should be disabled by Clip / Opacity when using surfaces with alpha")]
    public class Regression_Bug43 : WindowTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Variation()]
        public Regression_Bug43()
            : base(true)
        {
            RunSteps += new TestStep(CreateWindowContent);
            RunSteps += new TestStep(Capture);
            RunSteps += new TestStep(Verify);
        }

        #region Test Methods
        /// <summary>
        /// Create window content.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult CreateWindowContent()
        {
            //make a layered window

            Window.WindowStyle = System.Windows.WindowStyle.None;
            Window.Background = System.Windows.Media.Brushes.Transparent;

            Window.SetValue(RenderOptions.ClearTypeHintProperty, ClearTypeHint.Enabled);

            _box = new TextBox();
            _box.Text = "Clear";
            _box.BeginInit();
            _box.Height = 100;
            _box.Width = 100;
            _box.EndInit();

            Window.Content = _box;
            Window.BeginInit();
            Window.Height = 100;
            Window.Width = 100;
            Window.EndInit();

            return TestResult.Pass;
        }

        private TestResult Capture()
        {
            RenderSynchronization.WaitForRenderedFrames(this,1,1000);
            _capture = ImageUtility.CaptureElement(_box);
            return TestResult.Pass;
        }

        /// <summary>
        /// Attempt our repro steps.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            _rowToScan = 8;
            if (!ClearTypeDetector.CheckForRedBlueShift(_capture, this._rowToScan, 4, 4))//ignore window borders
            {
                return TestResult.Pass;
            }
            else
            {
                string saveFile = "captured.bmp";
                Status("Saving captured image to " + saveFile);
                ImageAdapter img = new ImageAdapter(_capture);
                ImageUtility.ToImageFile(img, saveFile);
                Log.LogFile(saveFile);
                return TestResult.Fail;
            }
        }
        #endregion

        #region Members
        private TextBox _box;
        private Bitmap _capture;
        private int _rowToScan;
        #endregion
    }
}