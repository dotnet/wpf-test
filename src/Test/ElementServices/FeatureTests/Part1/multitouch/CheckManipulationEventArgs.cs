// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input.MultiTouch;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Utils for the values of various properties on manipulation event args classes
    /// 
    /// 

    public static class CheckManipulationEventArgs
    {
        #region Manipulation events

        /// <summary>
        /// Checks ManipulationStartingEventArgs for validity
        /// </summary>
        /// <param name="e"></param>
        public static void Starting(ManipulationStartingEventArgs e)
        {
            Utils.Assert(e.Mode == ManipulationModes.All, "In Starting - the default mode is not correct");
            Utils.Assert(e.ManipulationContainer is UIElement, "In Starting - the default container is not correct");
            Utils.Assert(e.Pivot == null, "In Starting - the default pivot is not null");
            Utils.Assert(e.IsSingleTouchEnabled == true, "In Starting - the default IsSingleTouchEnabled is not correct");
////            Utils.Assert(e.Cancel == false, "In Starting - the default Cancel is not correct");
            Utils.Assert(e.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble, "In Starting - the Starting routing strategy is incorrect");
        }

        /// <summary>
        /// Checks ManipulationStartedEventArgs for validity
        /// </summary>
        /// <param name="e"></param>
        public static void Started(ManipulationStartedEventArgs e)
        {
            Utils.Assert(IsFinite(e.ManipulationOrigin.X), string.Format("In Started - e.ManipulationOrigin.X IsFinite check failed"));
            Utils.Assert(IsFinite(e.ManipulationOrigin.Y), string.Format("In Started - e.ManipulationOrigin.Y IsFinite check failed"));
            Utils.Assert(e.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble, "In Started - the Started routing strategy is incorrect");
        }

        /// <summary>
        /// Checks ManipulationDeltaEventArgs for validity
        /// </summary>
        /// <param name="e">Event args.</param>
        /// <param name="currentSupportedManipulations">The set of currently supported manipulations.</param>
        /// <param name="cumulativeSupportedManipulations">The set of all manipulations that
        /// have been supported at any time since the operation began.</param>
        public static void Delta(ManipulationDeltaEventArgs e,
            ManipulationModes currentSupportedManipulations, ManipulationModes cumulativeSupportedManipulations)
        {
            Utils.Assert(IsFinite(e.ManipulationOrigin.X), string.Format("In Delta - e.ManipulationOrigin.X IsFinite check failed"));
            Utils.Assert(IsFinite(e.ManipulationOrigin.Y), string.Format("In Delta - e.ManipulationOrigin.Y IsFinite check failed"));
            Utils.Assert(e.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble, "In Delta - the Delta routing strategy is incorrect");

            CheckValidDelta(e.DeltaManipulation);
            CheckValidDelta(e.CumulativeManipulation);

            CheckAllowedDelta(e.DeltaManipulation, currentSupportedManipulations);
            CheckAllowedDelta(e.CumulativeManipulation, cumulativeSupportedManipulations);

            CheckVelocities(e.Velocities, cumulativeSupportedManipulations);
        }

        /// <summary>
        /// Checks ManipulationInertiaStartingEventArgs for validity
        /// </summary>
        /// <param name="e"></param>
        public static void InertiaStarting(ManipulationInertiaStartingEventArgs e, 
            ManipulationModes cumulativeSupportedManipulations)
        {            
            Utils.Assert(IsFinite(e.ManipulationOrigin.X), "In InertiaStarting - e.ManipulationOrigin.X IsFinite check failed");
            Utils.Assert(IsFinite(e.ManipulationOrigin.Y), "In InertiaStarting - e.ManipulationOrigin.Y IsFinite check failed");
            Utils.Assert(e.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble, "In InertiaStarting - the InertiaStarting routing strategy is incorrect");

            CheckVelocities(e.InitialVelocities, cumulativeSupportedManipulations);
        }

        /// <summary>
        /// Checks ManipulationCompletedEventArgs for validity.
        /// </summary>
        /// <param name="e">Event args.</param>
        /// <param name="cumulativeSupportedManipulations">The set of all manipulations that
        /// have been supported at any time since the operation began.</param>
        public static void Completed(ManipulationCompletedEventArgs e, ManipulationModes cumulativeSupportedManipulations)
        {
            Utils.Assert(IsFinite(e.ManipulationOrigin.X), "In Completed - e.ManipulationOrigin.X IsFinite check failed");
            Utils.Assert(IsFinite(e.ManipulationOrigin.Y), "In Completed - e.ManipulationOrigin.y IsFinite check failed");

            CheckValidDelta(e.TotalManipulation);
            CheckAllowedDelta(e.TotalManipulation, cumulativeSupportedManipulations);

            CheckVelocities(e.FinalVelocities, cumulativeSupportedManipulations);
        }

        /// <summary>
        /// Check a delta for valid values.
        /// </summary>
        /// <param name="delta"></param>
        public static void CheckValidDelta(ManipulationDelta delta)
        {
            Utils.Assert(delta != null, "In CheckValidDelta - delta param should not be null");

            Utils.Assert(IsFinite(delta.Translation.X), "In CheckValidDelta - IsFinite for delta.Translation.X failed");
            Utils.Assert(IsFinite(delta.Translation.Y), "In CheckValidDelta - IsFinite for delta.Translation.y failed");
            Utils.Assert(IsFinite(delta.Rotation), "In CheckValidDelta - IsFinite for delta.Rotation failed");
            Utils.Assert(IsFinite(delta.Expansion.X), "In CheckValidDelta - IsFinite for delta.Expansion.X failed");
            Utils.Assert(IsFinite(delta.Expansion.Y), "In CheckValidDelta - IsFinite for delta.Expansion.y failed");
            Utils.Assert(IsFinite(delta.Scale.X), "In CheckValidDelta - IsFinite for delta.Scale.X failed");
            Utils.Assert(IsFinite(delta.Scale.Y), "In CheckValidDelta - IsFinite for delta.Scale.X failed");

            //
            Utils.AssertEqual(delta.Expansion.X, delta.Expansion.Y, "In CheckValidDelta - non-proportional scaling detected");
            Utils.AssertEqual(delta.Scale.X, delta.Scale.Y, "In CheckValidDelta - non-proportional scaling detected");

            Utils.Assert(delta.Scale.X > 0, "In CheckValidDelta - delta.Scale.X should be > 0");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check a manipulation delta to see whether it is consistent with
        /// the specified set of supported manipulations
        /// </summary>
        /// <param name="delta"></param>
        private static void CheckAllowedDelta(ManipulationDelta delta, ManipulationModes supportedManipulations)
        {
            CheckDeltaComponent(supportedManipulations, ManipulationModes.TranslateX, delta.Translation.X != 0);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.TranslateY, delta.Translation.Y != 0);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.Rotate, delta.Rotation != 0);

            bool hasExpansion = (delta.Expansion.X != 0)
                || (delta.Expansion.Y != 0)
                || (delta.Scale.X != 1)
                || (delta.Scale.Y != 1);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.Scale, hasExpansion);
        }

        /// <summary>
        /// Verifies that a double value is finite.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        /// <summary>
        /// check allowed manipulation given mode and isActive
        /// </summary>
        /// <param name="allowed"></param>
        /// <param name="component"></param>
        /// <param name="isActive"></param>
        private static void CheckDeltaComponent(ManipulationModes allowed, ManipulationModes component, bool isActive)
        {
            bool isEnabled = (allowed & component) != ManipulationModes.None;
            if (!isEnabled)
            {
                Utils.Assert(isActive == false, "In CheckDeltaComponent - {0} occurred when not allowed", component);
            }
        }
        /// <summary>
        /// Checks velocities to see if they're valid
        /// </summary>
        /// <param name="velocities"></param>
        /// <param name="supportedManipulations"></param>
        private static void CheckVelocities(ManipulationVelocities velocities, ManipulationModes supportedManipulations)
        {
            Utils.Assert(velocities != null, "In CheckVelocities - the param velocities should not be null");

            Utils.Assert(IsFinite(velocities.LinearVelocity.X), "In CheckVelocities - velocities.LinearVelocity.X should be finite");
            Utils.Assert(IsFinite(velocities.LinearVelocity.Y), "In CheckVelocities - velocities.LinearVelocity.Y should be finite");
            Utils.Assert(IsFinite(velocities.AngularVelocity), "In CheckVelocities - velocities.AngularVelocity should be finite");
            Utils.Assert(IsFinite(velocities.ExpansionVelocity.X), "In CheckVelocities - velocities.ExpansionVelocity.X should be finite");
            Utils.Assert(IsFinite(velocities.ExpansionVelocity.Y), "In CheckVelocities - velocities.ExpansionVelocity.Y should be finite");
            Utils.AssertEqual(velocities.ExpansionVelocity.X, velocities.ExpansionVelocity.Y, "In CheckVelocities - non-proportional scaling detected");

            CheckDeltaComponent(supportedManipulations, ManipulationModes.TranslateX, velocities.LinearVelocity.X != 0);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.TranslateY, velocities.LinearVelocity.Y != 0);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.Rotate, velocities.AngularVelocity != 0);
            CheckDeltaComponent(supportedManipulations, ManipulationModes.Scale, velocities.ExpansionVelocity.X != 0);
        }

        #endregion
    }
}
