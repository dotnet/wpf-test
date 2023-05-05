// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using Microsoft.Test.RenderingVerification;
using System.Reflection;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Verification;

namespace Microsoft.Test.Verifiers
{
    /// <summary>
    /// Verifies that the FrameworkElement passed into the 
    /// constructor renders correctly. This is deteremined
    /// by the expected object passed into the Verify method
    /// </summary>
    public class VScanVerifier : IVerifier
    {

        #region Private Members

        FrameworkElement _target;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="target">Test Framework element to verify</param>
        public VScanVerifier(FrameworkElement target)
        {
            _target = target;
        }

        #endregion

        #region IVerifier Members

        /// <summary>
        /// Verifies the target against the expected
        /// </summary>
        /// <param name="expectedState">expected Bitmap or Framework element</param>
        /// <returns>VScanVerifierResult containing result of the verify</returns>
        public IVerifyResult Verify(params object[] expectedState)
        {
            if (expectedState[0] is Bitmap)
            {
                return this.Verify(expectedState[0] as Bitmap);
            }
            else if (expectedState[0] is FrameworkElement)
            {
                return this.Verify(expectedState[0] as FrameworkElement);
            }
            else
            {
                throw new ArgumentException(expectedState[0].ToString() + " is not a valid argument to " + this.ToString());
            }
        }

        /// <summary>
        /// Verifies the target against the expected
        /// </summary>
        /// <param name="expectedState">expected Framework element</param>
        /// <returns>VScanVerifierResult containing result of the verify</returns>
        public IVerifyResult Verify(FrameworkElement expectedState)
        {
            Window win = Microsoft.Test.Data.WindowHelper.GetAncestorWindow(expectedState);
            Microsoft.Test.Data.WindowHelper.WindowOnTopAndIdle(win);
            Bitmap expectedBMP = ImageUtility.CaptureElement(expectedState);
            return (this.Verify(expectedBMP));
        }

        /// <summary>
        /// Verifies the target against the expected
        /// </summary>
        /// <param name="expectedState">expected Bitmap</param>
        /// <returns>VScanVerifierResult containing result of the verify</returns>
        public IVerifyResult Verify(Bitmap expectedState)
        {
            VScanVerifierResult result = new VScanVerifierResult();

            Window win = Microsoft.Test.Data.WindowHelper.GetAncestorWindow(_target);
            Microsoft.Test.Data.WindowHelper.WindowOnTopAndIdle(win);
            Bitmap renderBMP = ImageUtility.CaptureElement(_target);
            ImageAdapter expectAdapt = new ImageAdapter(expectedState);
            ImageAdapter renderAdapt = new ImageAdapter(renderBMP);
            ImageComparator comparator = new ImageComparator(ChannelCompareMode.ARGB);
            comparator.Curve.CurveTolerance.Entries.Clear();
            comparator.Curve.CurveTolerance.Entries.Add(0, 1.0);
            comparator.Curve.CurveTolerance.Entries.Add(2, 0.003);
            comparator.Curve.CurveTolerance.Entries.Add(3, 0.002);
            comparator.Curve.CurveTolerance.Entries.Add(6, 0.001);
            comparator.Curve.CurveTolerance.Entries.Add(8, 0.00055);
            comparator.Curve.CurveTolerance.Entries.Add(10, 0.00045);
            comparator.Curve.CurveTolerance.Entries.Add(15, 0.0004);
            comparator.Curve.CurveTolerance.Entries.Add(25, 0.0003);
            comparator.Curve.CurveTolerance.Entries.Add(35, 0.002);
            comparator.Curve.CurveTolerance.Entries.Add(45, 0.000001);
            bool exactMatch = comparator.Compare(expectAdapt, renderAdapt, false);
            if (exactMatch)
            {
                result.Message = "The images compared the same.";
                result.Result = TestResult.Pass;
            }
            else
            {
                result.DifferenceFileName = "VScanDiff.png";
                result.RenderedImageName = "Rendered.bmp";
                result.ExpectedImageName = "Expected.bmp";

                ImageUtility.ToImageFile(comparator.GetErrorDifference(ErrorDifferenceType.IgnoreAlpha), result.DifferenceFileName, ImageFormat.Png);
                expectedState.Save(result.ExpectedImageName);
                renderBMP.Save(result.RenderedImageName);
                result.Message = "There were rendering differences. Check image files: " + result.ExpectedImageName + ", " + result.RenderedImageName + ", " + result.DifferenceFileName;
                result.Result = TestResult.Pass;
            }

            return result;
        }

        #endregion

    }

    /// <summary>
    /// Verify Result for the VScanVerifier
    /// </summary>
    public class VScanVerifierResult : IVerifyResult
    {

        #region Private Members

        private TestResult _result = TestResult.Unknown;
        private string _differenceFileName = "";
        private string _expectedImageName = "";
        private string _renderedImageName = "";
        private string _message = "";

        #endregion

        #region IVerifyResult Members

        public TestResult Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string DifferenceFileName
        {
            get { return _differenceFileName; }
            set { _differenceFileName = value; }
        }

        public string ExpectedImageName
        {
            get { return _expectedImageName; }
            set { _expectedImageName = value; }
        }

        public string RenderedImageName
        {
            get { return _renderedImageName; }
            set { _renderedImageName = value; }
        }

        #endregion

    }
}
