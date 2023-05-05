// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the SpeedRatio property of a ClockController
 */

//Instructions:
//   1. Create a Timeline 
//   2. Create a Clock, connecting it to the Timeline.
//   3. Start the TimeManager.

//Warnings:
//     Any changes made in the output should be reflected in ICSpeedRatioExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class ICSpeedRatio :ITimeBVT
    {
        Clock  _tClock;
        /*
        *  Function:  Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            timeline.BeginTime   = TimeSpan.FromMilliseconds(0);
            timeline.Duration    = new Duration(TimeSpan.FromMilliseconds(15));
            timeline.Name          = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);

            outString += "---SpeedRatio: " + _tClock.Controller.SpeedRatio + "\n";

            _tClock.Controller.SpeedRatio = 3;

            outString += "---SpeedRatio: " + _tClock.Controller.SpeedRatio + "\n";

            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, _tClock, tManager, 0, 16, 1, ref outString);

            outString += "---SpeedRatio: " + _tClock.Controller.SpeedRatio + "\n";

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
