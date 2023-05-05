// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test Timeline RepeatBehavior & AutoReverse properties together

*/

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 19 , RepeatedCount = 2 and AutoReverse = true
//  2. Attach event handlers to the Timeline
//  3. Create a TimeManager with Start=0 and Step = 5 and add the Timelines
//  4. Check each fired Event

//Warnings:
//  Any changes made in the output should be reflected in ReversedRepeatedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class ReverseRepeat :ITimeBVT
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
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new System.Windows.Duration(TimeSpan.FromMilliseconds(25));
            tNode.RepeatBehavior  = new RepeatBehavior(2);
            tNode.AutoReverse     = true;
            tNode.Name            = "Timeline";            
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            AttachAllHandlers(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");
        
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 110, 5, ref outString);

            WriteAllEvents();

            return outString;
        }
    }
}
