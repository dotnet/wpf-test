// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting a Timeline's RepeatBehavior to a TimeSpan value (count)
 */

//Instructions:
//  1. Create a Timeline with Begin = 500 , Duration = 500 and RepeatCount = 3
//  2. Attach event handlers to the Timeline
//  3. Create a TimeManager with Start=0 and Step = 100 and add the Timelines
//  4. Check the CurrentState Property on each fired Event

//Warnings:
//  Any changes made in the output should be reflected in TimelineRepeatedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class TimeNodeRepeat :ITimeBVT
    {
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the Timeline
            tNode.BeginTime       = TimeSpan.FromMilliseconds(500);
            tNode.Duration        = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));
            tNode.RepeatBehavior  = new RepeatBehavior(3);
            tNode.Name            = "SimpleTimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handlers to the Timeline
            AttachCurrentGlobalSpeedInvalidated( tNode );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach handler to the clock, to detect the repeat.
            AttachRepeatTC( tClock );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 3000, 100, ref outString);

            WriteAllEvents();
            
            return outString;
        }
    }
}
