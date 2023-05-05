// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a RepeatBehavior's default properties
 */

//Instructions:
//  1. Create a Timeline, without setting any properties
//  2. Read RepeatBehavior properties.

//Pass Condition:
//  This test passes if the actual value of the RBDefaults property matches the value
//  passed to the RepeatBehavior constructor.

//Pass Verification:
//  The output of this test should match the expected output in RBDefaultsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBDefaultsExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBDefaults :ITimeBVT
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
            
            //Must explicitly set RepeatBehavior with a TimeSpan to be able to read RepeatDuration.
            timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(5));

            outString += "RepeatBehavior   = " +  timeline.RepeatBehavior + "\n";
            outString += "Duration         = " +  timeline.RepeatBehavior.Duration + "\n";
            outString += "HasCount         = " +  timeline.RepeatBehavior.HasCount + "\n";
            outString += "HasDuration      = " +  timeline.RepeatBehavior.HasDuration + "\n";
            
            //Must explicitly set RepeatBehavior with a double to be able to read Count.
            timeline.RepeatBehavior  = new RepeatBehavior(5);

            outString += "RepeatBehavior    = " +  timeline.RepeatBehavior + "\n";
            outString += "Count             = " +  timeline.RepeatBehavior.Count + "\n";
            outString += "HasCount          = " +  timeline.RepeatBehavior.HasCount + "\n";
            outString += "HasDuration       = " +  timeline.RepeatBehavior.HasDuration + "\n";

            return outString;
        }
    }
}
