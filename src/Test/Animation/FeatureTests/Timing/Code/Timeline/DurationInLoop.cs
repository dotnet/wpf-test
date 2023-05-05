// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Duration property inside the Tick loop
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 500, AutoReverse = true, RepeatCount = 2
//  2. Create a TimeManager with Start = 0 and Step = 100 and add the TimeNode

//Pass Condition:
//   This test passes if Duration returns the assigned Duration each time it is read.

//Pass Verification:
//   The output of this test should match the expected output in DurationInLoopExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DurationInLoopExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DurationInLoop :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));
            tNode.RepeatBehavior  = new RepeatBehavior(2);
            tNode.AutoReverse     = true;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer
            tMan.Start();
            for(int i = 0; i <= 2200; i += 100 )
            {
                outString += i + " ms\tDuration " + (int) tNode.Duration.TimeSpan.TotalMilliseconds + " ms\n";
                DEBUG.LOGSTATUS("Processing time " + i);
                
                CurrentTime = TimeSpan.FromMilliseconds(i);
                tMan.Tick();        
            }           
            return outString;
        }
    }
}
