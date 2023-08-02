// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify intermediate points with an element.
    /// </summary>
    public abstract class GetIntermediatePointsBaseApp: TestApp
    {
        /// <summary>
        /// Execute stuff.
        /// </summary>
        protected override object DoExecute(object arg)
        {
            Point lastPoint = new Point();

            // Track the point of the latest MouseMove.
            InputElement.MouseMove +=
                (MouseEventHandler)delegate(object sender, MouseEventArgs args)
                {
                    lastPoint = args.GetPosition(null);
                };

            //
            // Move over several points to the center of the element.
            // Verify GetIntermediatePoints returns multiple points, the last one
            // equal to the mouse position when MouseMove is raised.
            //

            // -> Send move messages async, wait for some time, then wait for all input events.
            CoreLogger.LogStatus("Moving mouse to center of element...");
            MouseHelper.IsSynchronous = false;
            MouseHelper.Move(_rootElement, MouseLocation.Center, false);
            DispatcherHelper.DoEvents(1000);
            InputHelper.WaitForAllInputEvents();

            _VerifyPoints(5, lastPoint);

            //
            // Move to the right side of the element.
            // Verify GetIntermediatePoints returns two points equal to the 
            // start position and the position when MouseMove is raised.
            //
            CoreLogger.LogStatus("Moving mouse immediately to right of element...");
            MouseHelper.IsSynchronous = true;
            MouseHelper.Move(_rootElement, MouseLocation.CenterRight, true);

            _VerifyPoints(2, lastPoint);

            //
            // Move to the outer right side of the element.
            // Verify GetIntermediatePoints returns 0 points.
            //
            CoreLogger.LogStatus("Moving mouse immediately to outer right of element...");
            MouseHelper.MoveOutside(_rootElement, MouseLocation.CenterRight, true);

            _VerifyPoints(-1, new Point(0, 0));

            base.DoExecute(arg);

            //
            //
            //

            return null;
        }

        private void _VerifyPoints(int expectedCount, Point lastPoint)
        {
            CoreLogger.LogStatus("Getting intermediate points....");
            Point[] pts = new Point[5];

            int countPoints = Mouse.GetIntermediatePoints(InputElement, pts);
            foreach (Point pt in pts)
            {
                CoreLogger.LogStatus("  pt: " + pt.ToString());
            }

            if (countPoints != expectedCount)
            {
                throw new Microsoft.Test.TestValidationException("The number of intermediate points is not the expected value. Expected: " + expectedCount + ", Actual:" + countPoints);
            }

            if (countPoints > 0 && lastPoint != pts[0])
            {
                throw new Microsoft.Test.TestValidationException("The last point in the array is unexpected. Expected: " + lastPoint + ", Actual:" + pts[0]);
            }

            countPoints = Mouse.GetIntermediatePoints(null, pts);
            if (countPoints != -1)
            {
                throw new Microsoft.Test.TestValidationException("Mouse.GetIntermediatePoints(null) returned an unexpected count. Expected: -1, Actual:" + countPoints);
            }
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg) 
        {
            this.TestPassed = true;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Main element to use with GetIntermediatePoints().
        /// </summary>
        protected IInputElement InputElement = null;
    }
}

