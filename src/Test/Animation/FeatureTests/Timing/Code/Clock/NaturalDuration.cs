// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a Clock's NaturalDuration property
 */

//Instructions:
//  1. Create a Timeline
//  3. Create a Clock and associate it to the Timeline
//  4. Start the TimeManager

//Pass Condition:
//   This test passes if the actual value of the NaturalDuration property is correct. 

//Pass Verification:
//   The output of this test should match the expected output in NaturalDurationExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected NaturalDurationExpect.txt file

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
    class NaturalDuration :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            timeline.BeginTime       = TimeSpan.FromMilliseconds(2);
            timeline.Duration        = new Duration(TimeSpan.FromMilliseconds(4));
            timeline.Name              = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 12, 1, ref outString);

            return outString;
        }

        public override void OnProgress( Clock subject )
        {
            outString += "------------NaturalDuration: " + subject.NaturalDuration + "\n";
        }
    }
}
