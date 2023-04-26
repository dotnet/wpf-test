// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a RepeatBehavior's Duration when it is smaller than Duration
  */

//Instructions:
//  1. Create a Timeline, setting new RepeatBehavior(TimeSpan.FromMilliseconds(0))
//  2. Read RepeatBehavior.Duration

//Pass Condition:
//  This test passes if the actual value of the Duration property matches the value
//  passed to the RepeatBehavior constructor, and the Timeline ends appropriately.

//Pass Verification:
//  The output of this test should match the expected output in RepeatDurationSmallerExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RepeatDurationSmallerExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RepeatDurationSmaller :ITimeBVT
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
            timeline.BeginTime       = TimeSpan.FromMilliseconds(0);
            timeline.Name              = "Timeline";
            timeline.Duration        = new Duration(TimeSpan.FromMilliseconds(4));
            timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(2));
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = timeline.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentGlobalSpeedInvalidatedTC(tClock);
            AttachCurrentStateInvalidatedTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            outString += "RepeatBehavior = " +  timeline.RepeatBehavior + "\n";
            outString += "Duration = " +  timeline.RepeatBehavior.Duration + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 10, 1, ref outString);

            return outString;
        }
          
          public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
          {
               //In this scenario, CurrentIteration should not increase.
               if ((((Clock)subject).CurrentIteration > iterationCount))
               {
                   if (iterationCount > 0)
                   {
                       outString += "----CurrentGlobalSpeedInvalidated fired (Repeated)\n";
                   }
                   iterationCount++;
               }
          }
          public override void OnProgress( Clock subject )
          {
                outString += "--Progress         = " + ((Clock)subject).CurrentProgress  + "\n";
                outString += "--CurrentState     = " + ((Clock)subject).CurrentState     + "\n";
                outString += "--CurrentIteration = " + ((Clock)subject).CurrentIteration + "\n";
          }
    }
}
