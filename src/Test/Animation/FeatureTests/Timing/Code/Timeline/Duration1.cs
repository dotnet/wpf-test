// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading Duration via the Begun, Repeated, Reversed, and Ended events.
 */

//Instructions:
//  1. Create a Timeline with Begin = 10, Duration = 50, AutoReverse = True, RepeatCount = 2
//  2. Create a TimeContainer with none of the attributes set and add the Timenode
//  3. Create a TimeManager with Start=0 and Step = 50 and add the container

//Pass Condition:
//   This test passes if the actual value of the Duration property matches the value initially
//   assigned.

//Pass Verification:
//   The output of this test should match the expected output in Duration1Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected Duration1Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Duration1 :ITimeBVT
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
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime         = TimeSpan.FromMilliseconds(10);
            tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(50));
            tNode.RepeatBehavior    = new RepeatBehavior(2);
            tNode.AutoReverse       = true;
            tNode.Name              = "SimpleTimeNode";          
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            // Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 220, 10, ref outString);

            return outString;
        }

        public override void OnProgress(Clock subject)
        {
            Clock tlc = (Clock)subject;
            Timeline timeLine = (Timeline)tlc.Timeline;
            outString += "    Duration = " + timeLine.Duration + "\n";
        }
    }
}
