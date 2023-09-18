// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Regression test for Regression_Bug45 
*/

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.VisualVerification;
using System.ComponentModel.Design.Serialization;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using SWM=System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

using Microsoft.Test.Threading;


namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Regression test for Regression_Bug45 
    /// </summary>
    // commented ignored tests
    // [Test(1, "Regression", "Regression_Bug45",
    //     Area = "2D",
    //     Description = @"Regression test for Regression_Bug45 :  Cached composition leaves dirty bits on the screen when the content of the cache shrinks",
    //     SupportFiles = @"TestApiCore.dll,FeatureTests\2D\Dev10\Masters\Regression_Bug45_KnownGood.png"
    //     )
    // ]
    public class Regression_Bug45 : WindowTest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        [Variation("Regression_Bug45_KnownGood.png")]
        public Regression_Bug45(string knownGoodFile)
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
            Window.WindowStyle = WindowStyle.None;
            Window.ResizeMode = ResizeMode.NoResize;
            s_c = new Chart(Window);
            Window.Content = s_c;
            Window.Height = 200;
            Window.Width = 200;
            return TestResult.Pass;
        }

        private TestResult Capture()
        {
            WaitFor(6000);
            _captured = GrabWindowSnapshot(Window);
			_captured.ToFile("captured.png", ImageFormat.Png);
            return TestResult.Pass;
        }

        /// <summary>
        /// Attempt our repro steps.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Verify()
        {
            SnapshotColorVerifier verifier = new SnapshotColorVerifier();
            Snapshot diff = _knownGood.CompareTo(_captured);
            // crop out the outer pixels - we get background from windows' rounded corners.
            Rectangle crop = new System.Drawing.Rectangle(
                                                2,
                                                2,
                                                (int)diff.Width - 4,
                                                (int)diff.Height - 4);
            
            _captured.ToFile("captured.png", ImageFormat.Png);



            VerificationResult result = verifier.Verify(diff.Crop(crop));
            if (VerificationResult.Pass == result)
            {
                return TestResult.Pass;
            }
            else
            {
                // save out our data
                _captured.ToFile("captured.png", ImageFormat.Png);
                diff.ToFile("diff.png", ImageFormat.Png);
                return TestResult.Fail;
            }
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
        private static Chart s_c;
        #endregion
    }

    internal class ChartCore : SWM.DrawingVisual
    {
        public ChartCore()
        {
            SWM.DrawingContext dc = RenderOpen();
            dc.PushTransform(new SWM.TranslateTransform(30, 30));
            SWM.ScaleTransform st = new SWM.ScaleTransform(20, 20, 0.5, 0.5);
            DoubleAnimation da = new DoubleAnimation(0.1, new Duration(new TimeSpan(0, 0, 0, 0, 2000)));
            st.BeginAnimation(SWM.ScaleTransform.ScaleXProperty, da);
            st.BeginAnimation(SWM.ScaleTransform.ScaleYProperty, da);
            dc.PushTransform(st);
            dc.DrawRoundedRectangle(SWM.Brushes.LightCyan, new SWM.Pen(SWM.Brushes.Gray, 2), new Rect(new System.Windows.Size(96, 96)), 100, 10);
            dc.Close();
        }
    }

    internal class Chart : UIElement
    {
        SWM.ContainerVisual _container;
        ChartCore _core;

        public Chart(Window w)
        {
            CacheMode = new SWM.BitmapCache(1);
            _container = new SWM.ContainerVisual();
            _core = new ChartCore();
            _container.Children.Add(_core);
            AddVisualChild(_container);
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override SWM.Visual GetVisualChild(int index)
        {
            return _container;
        }
    }
}