// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Clock's events fire
 *
 */

//Instructions:
//  1. Create a Timeline with Begin = 0, Duration = 20, AutoReverse = True, RepeatedCount = 2
//  2. Create a Clock and connect it to the Timeline
//  3. Attach event handlers to the Clock
//  4. Detach event handlers to the Clock
//  5. Create a TimeManager with Start=0 and Step = 100

//Pass Condition:
//   This test passes if events fired correctly.

//Pass Verification:
//   The output of this test should match the expected output in ClockEventsExpectDetachBefore.txt.

//Warnings:
//  Any changes made in the output should be reflected ClockEventsExpectDetachBefore.txt file

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
    class ClockEventsDetachBefore :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Clock and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(20));
            tNode.AutoReverse       = true;
            tNode.RepeatBehavior    = new RepeatBehavior(2);
            tNode.Name              = "TimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            AttachAllHandlersTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Detach Handlers to the TimeNode
            DetachAllHandlersTC(tClock);
            DEBUG.LOGSTATUS(" Detached EventHandlers to the Clock ");

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 100, 10, ref outString);

               
            return outString;
        }
    }
}
