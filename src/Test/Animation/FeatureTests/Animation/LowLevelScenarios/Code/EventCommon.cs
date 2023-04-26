// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/**********************************************************************************************
* Description:          Common routines for handling Animation Event testing
* File dependencies:    TestRuntime.dll
***********************************************************************************************/
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
     
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    public class CommonEvents : WindowTest
    {
        public static int                   begunCount       = 0;
        public static int                   endedCount       = 0;
        public static int                   changedCount     = 0;
        public static int                   pausedCount      = 0;
        public static int                   repeatedCount    = 0;
        public static int                   resumedCount     = 0;
        public static int                   reversedCount    = 0;
        public static int                   iterationCount   = 0;
        public static double                lastSpeed        = 0;
        public static bool                  lastPaused       = false;
        public static bool                  seekInvoked      = false;


        /*****************************************************************************
            * Function:     CurrentStateInvalidatedHandler
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public static void CurrentStateInvalidatedHandler(object sender)
        {
            //GlobalLog.LogEvidence("---OnCurrentStateInvalidated---");

            if (seekInvoked)
            {
                //A seek will fire this event; so using a flag to avoid signalling Begun/Ended.
                seekInvoked = false;
            }
            else
            {
                if (((Clock)sender).CurrentState == ClockState.Active)
                {
                    begunCount += 1;
                    GlobalLog.LogEvidence("---Begun---");
                }
                else
                {
                    endedCount += 1;          
                    GlobalLog.LogEvidence("---Ended---");
                }
            }
        }

        /*****************************************************************************
            * Function:     CurrentGlobalSpeedInvalidatedHandler
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public static void CurrentGlobalSpeedInvalidatedHandler(object sender)
        {
            //GlobalLog.LogEvidence("---OnCurrentGlobalSpeedInvalidated---");

            double globalSpeed  = GetCurrentGlobalSpeed(((Clock)sender));

            //Using the Clock's CurrentGlobalSpeed property to determine if the Timeline reverses.
            //CurrentGlobalSpeed changes to -1 when the clock reverses.
            if (((Clock)sender).CurrentGlobalSpeed != null)
            {
                if (globalSpeed < 0)
                {
                    //Avoid signalling repeated "Reversed" during Acceleration/Deceleration, by
                    //counting 'Reversed' only when the sign of the CurrentGlobalSpeed property changes.
                    if (Math.Sign(globalSpeed) != Math.Sign(lastSpeed))
                    {
                        reversedCount += 1;          
                        GlobalLog.LogEvidence("---Reversed---");
                    }
                }
                lastSpeed = globalSpeed;
            }
               
            //Check if the animation is paused and/or resumed.
                if (((Clock)sender).IsPaused)
                {
                    pausedCount += 1;
                    GlobalLog.LogEvidence("---Paused---");
                }
                else
                {
                    if (lastPaused)
                    {
                        resumedCount += 1;
                        GlobalLog.LogEvidence("---Resumed---");
                    }
                }
                lastPaused = ((Clock)sender).IsPaused;
        }

        /*****************************************************************************
            * Function:     CurrentTimeInvalidatedHandler
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public static void CurrentTimeInvalidatedHandler(object sender)
        {
           //GlobalLog.LogEvidence("---OnCurrentTimeInvalidated---");
           changedCount += 1;
        }

        /*****************************************************************************
            * Function:     CurrentTimeInvalidatedRepeatHandler
            *****************************************************************************/
        /// <summary>
        /// </summary>
        /// <param>sender: the TimelineClock associated with the event</param>
        /// <returns></returns>
        public static void CurrentTimeInvalidatedRepeatHandler(object sender)
        {
            //Using the Clock's CurrentIteration property to determine if the Timeline repeats.
            //CurrentIteration changes to 1 when the Timeline begins, and -1 when it ends.
            //It also increments when the Timeline repeats.

            if ((((Clock)sender).CurrentIteration > iterationCount))
            {
                if (iterationCount > 0)
                {
                    repeatedCount += 1;          
                    GlobalLog.LogEvidence("---Repeated---");
                }
                iterationCount++;
            }
        }

        public static double GetCurrentGlobalSpeed(Clock clock)
        {
            if (clock.CurrentGlobalSpeed == null)
            {
                //Handle a null CurrentGlobalSpeed.
                return 99999;
            }
            else
            {
                return (double)clock.CurrentGlobalSpeed;
            }
        }
    }
}
