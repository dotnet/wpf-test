// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentProgress during reversal when Speed and RepeatBehavior are specified

*/

//Pass Verification:
//  The output of this test should match the expected output in bug90Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug90Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug90 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Parent", " Created Parent ");
            timeline.BeginTime        = TimeSpan.FromMilliseconds(6);
            timeline.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            timeline.RepeatBehavior   = new RepeatBehavior(0.596923);
            timeline.FillBehavior     = FillBehavior.Stop;
            timeline.SpeedRatio       = 1.26215;
            AttachCurrentStateInvalidated( timeline );

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 35, 1, ref outString);

            return outString;
        }
    }
}
