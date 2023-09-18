// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: TextHintingMode tests
 ********************************************************************/
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.VisualVerification;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Each variation will have settings for the members of this struct,
    /// these are the test vectors for this test engine.
    /// </summary>
    internal struct TestSettings
    {
        public TextHintingMode DesiredTextHintingMode; // which text hinting mode we should use
        public bool SuccessExpected; // whether this case is expected to match or not
        public string ExpectedFilename; // the known good image to compare this render to
    }

    /// <summary>
    /// Animation function tests for the TextHintingMode DWrite text feature
    /// </summary>
    [Test(1, "TextHinting",
     SupportFiles = @"FeatureTests\2D\Dev10\Masters\TextHinting-animated.png,FeatureTests\2D\Dev10\Masters\TextHinting-fixed.png,FeatureTests\2D\Dev10\Masters\TextHinting-auto.png")]
    public class TextHintingTests : WindowTest
    {
        private TestSettings _settings; // test vector values
        private Button _button; // display surface for the text
        private Bitmap _captured; // the actual rendered image
        private Bitmap _expected; // the expected render of the text

        #region Variations
        // - using a string instead of seperate values, so that we can change the values 
        // in the lab environment and run different tests. 
        // inputs - 
        // 1: value of textHint to apply to text
        // 2: is the rendered image expected to be the same as the reference?
        // 3: the name of the reference image

        ////////////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName: Microsoft.Test.Graphics.TextHintingTests(Animated\\\,true\\\,TextHinting-animated.png)
        // Area: 2D   SubArea: TextHinting
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        ////////////////////////////////////////////////////////////////////////////////////////////
        //[Variation("Animated,true,TextHinting-animated.png")]

        ////////////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName: Microsoft.Test.Graphics.TextHintingTests(Auto\\\,true\\\,TextHinting-auto.png)
        // Area: 2D   SubArea: TextHinting
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        ////////////////////////////////////////////////////////////////////////////////////////////
        //[Variation("Auto,true,TextHinting-auto.png")]

        [Variation("Fixed,true,TextHinting-fixed.png")]

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TextHintingTests(string inputs)
        {
            ParseInputs(inputs, ref _settings);
            InitializeSteps += new TestStep(CreateWindowContent);
            InitializeSteps += new TestStep(LoadReferenceContent);
            RunSteps += new TestStep(Capture);
            RunSteps += new TestStep(Verification);
        }

        /// <summary>
        /// Load reference image
        /// </summary>
        /// <returns>success of image loading</returns>
        private TestResult LoadReferenceContent()
        {
            LogComment("Loading reference image [" + _settings.ExpectedFilename + "]");
            _expected = null;
            _expected = new Bitmap(_settings.ExpectedFilename);
            if (null == _expected)
            {
                LogComment("reference image failed to load!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// Grab test vector values from the per-variation test settings string.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="s"></param>
        private void ParseInputs(string inputs, ref TestSettings s)
        {
            string[] tokens = inputs.Split(',');

            // first token
            s.DesiredTextHintingMode = (TextHintingMode)Enum.Parse(typeof(TextHintingMode), tokens[0], true);

            // second token
            s.SuccessExpected = bool.Parse(tokens[1]);

            // third token
            s.ExpectedFilename = tokens[2];
        }

        /// <summary>
        /// Put a button with text on it in the window for us to animate.
        /// </summary>
        private TestResult CreateWindowContent()
        {
            _button = new Button();
            _button.Width = 50;
            _button.Height = 20;
            _button.Content = "Text";

            TextOptions.SetTextHintingMode(_button, _settings.DesiredTextHintingMode);

            Window.Height = 100;
            Window.Width = 100;

            Window.Content = _button;

            return TestResult.Pass;
        }

        /// <summary>
        /// Capture text image at start
        /// </summary>
        private TestResult Capture()
        {
            // capture the image
            WaitFor(1000);
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);

            LogComment("Capturing image");
            _captured = ImageUtility.CaptureElement(_button);

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify Renderings of text before and after are the same (if success expected)
        /// </summary>
        private TestResult Verification()
        {
            Snapshot capturedSnapshot = Snapshot.FromBitmap(_captured);
            Snapshot expectedSnapshot = Snapshot.FromBitmap(_expected);
            byte d = 24;
            SnapshotColorVerifier verifier = new SnapshotColorVerifier(System.Drawing.Color.Black , new ColorDifference(d,d,d,d));
            Snapshot diff = expectedSnapshot.CompareTo(capturedSnapshot);

            VerificationResult result = verifier.Verify(diff);
            bool imagesMatch = (result == VerificationResult.Pass);

            // if we fail, write out the before and after shots.            
            if (imagesMatch)
            {
                LogComment("Images match");
            }
            else
            {
                LogComment("Images do not match");
            }

            if (_settings.SuccessExpected)
            {
                LogComment("Images were expected to match");
            }
            else
            {
                LogComment("Images were not expected to match");
            }

            // unexpected results? log everything and fail.
            if (imagesMatch != _settings.SuccessExpected)
            {
                string loggedCaptureFile = _settings.ExpectedFilename + "-captured.png";

                capturedSnapshot.ToFile(loggedCaptureFile, ImageFormat.Png);

                Log.LogFile(loggedCaptureFile);
                Log.LogFile(_settings.ExpectedFilename);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }
}

