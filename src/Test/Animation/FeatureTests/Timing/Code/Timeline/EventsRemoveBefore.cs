// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify removing timing events before creating a Clock
 */

//Instructions:
//   1. Create a TimeManager with Start=0 and Step = 10
//   2. Create a Timeline with Begin = 0, Duration = 20, AutoReverse = True, RepeatCount = 2
//   3. Attach the event handlers to the TimeNode
//   4. Detach the event handlers
//   5. Add the Timeline to the TimeManager's LiveNodes collection
//   6. Execute the Tick loop
//   7. Detach the Timeline from the TimeManager's LiveNodes collection

//Pass Condition:
//   This test passes if no events fire.

//Pass Verification:
//   The output of this test should match the expected output in EventsRemoveBeforeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected EventsRemoveBeforeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class EventsRemoveBefore :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
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
            tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            tNode.RepeatBehavior    = new RepeatBehavior(2);
            tNode.AutoReverse       = true;
            tNode.Name              = "SimpleTimeNode";         
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handlers to the TimeNode
            AttachAllHandlers(tNode);                  
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            //Detach Handlers to the TimeNode
            DetachAllHandlers(tNode);          
            DEBUG.LOGSTATUS(" Detached EventHandlers from the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 110, 10, ref outString);
            
            return outString;
        }
    }
}
