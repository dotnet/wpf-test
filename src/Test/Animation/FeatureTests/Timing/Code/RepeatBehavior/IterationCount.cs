// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a RepeatBehavior's Count property
 */

//Instructions:
//  1. Create a Timeline, setting new RepeatBehavior(2d)
//  2. Read RepeatBehavior.Count

//Pass Condition:
//  This test passes if the actual value of the Count property matches the value
//  passed to the RepeatBehavior constructor.

//Pass Verification:
//  The output of this test should match the expected output in CountExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CountExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class IterationCount :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            timeline.BeginTime          = TimeSpan.FromMilliseconds(2);
            timeline.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            timeline.RepeatBehavior     = new RepeatBehavior(2d);
            DEBUG.LOGSTATUS(" Set Timeline Properties ");
            
            outString += "Count = " +  timeline.RepeatBehavior.Count + "\n";

            // Create a Clock, connected to the Timeline.
            Clock tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            outString += "Count = " +  timeline.RepeatBehavior.Count + "\n";
            outString += "Count = " +  tClock.Timeline.RepeatBehavior.Count + "\n";

            return outString;
        }
    }
}
