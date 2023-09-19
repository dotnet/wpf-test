// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Automation;
using Microsoft.Test.Logging;
using Microsoft.Test.VisualVerification;

namespace Microsoft.Test.Graphics.CachedComposition
{
    /// <summary>
    /// Detect changes by checking for visual difference.
    /// </summary>
    class VisualChangeDetector : ChangeDetector
    {

        #region Public methods

        internal static System.Drawing.Rectangle GetElementSize(AutomationElement element)
        {
            Rect rr = (Rect)element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            return new System.Drawing.Rectangle((int)rr.Left, (int)rr.Top, (int)rr.Width, (int)rr.Height);
        }

        /// <summary>
        /// Grab the start snapshot so that we can determine if things have changed.
        /// </summary>
        /// <returns></returns>
        public override TestResult DetectBefore(Window w)
        {
            _rect = new System.Drawing.Rectangle((int)w.Left,
                                                (int)w.Top,
                                                (int)w.RestoreBounds.Width,
                                                (int)w.RestoreBounds.Height);
            _before = Snapshot.FromRectangle(_rect);
            return TestResult.Pass;
        }

        /// <summary>
        /// Grab our end snapshot to compare to the stored initial condition.
        /// </summary>
        /// <returns></returns>
        public override TestResult DetectAfter()
        {
            _after = Snapshot.FromRectangle(_rect);
            return TestResult.Pass;
        }

        /// <summary>
        /// Compare our start and end conditions (in this case our snapshots) and determine if we changed, and compare THAT 
        /// to whether we expected to change or not.
        /// </summary>
        /// <returns>Whether the changes are what is expected.</returns>
        public override bool VerifyChanges(Requirements r, TestLog Log)
        {
            // Compare the actual image with the master image
            Snapshot difference = _before.CompareTo(_after);

            SnapshotColorVerifier colorVerifier = new SnapshotColorVerifier(
                System.Drawing.Color.Black, new ColorDifference(255, 0, 0, 0));

            // Evaluate the difference image and retun the result
            VerificationResult changed = colorVerifier.Verify(difference);

            //if the images are different, and we expected this, then this is a success
            bool result = false;

            if (changed == VerificationResult.Fail && r.successExpected)//fail means that they are different
            {
                result = true;
            }
            else if (changed == VerificationResult.Pass && !r.successExpected) // this is an unexpected success - log it
            {
                result = false;
                // save out the images so that we can see what went wrong
                _before.ToFile("before.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                _after.ToFile("after.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                difference.ToFile("diff.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
            return result;
        }

        #endregion

        #region members

        private System.Drawing.Rectangle _rect;
        private Snapshot _before,_after;

        #endregion
    }
}
