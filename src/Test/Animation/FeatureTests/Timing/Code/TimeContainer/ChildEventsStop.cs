// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify event firing for a Timeline's children (FillBehavior.Stop)
 */

//Instructions:
//  1. Create a timeline and attach children to it.
//  2. Check event firing on the child timelines.

//Warnings:
//  Any changes made in the output should be reflected in ChildEventsStopExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ChildEventsStop : ITimeBVT
    {
        /*
         *  Function: Test
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

            // Create a Parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create TimeContainer", " Created TimeContainer ");
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            parent.FillBehavior  = FillBehavior.Stop;
            parent.Name          = "Parent";
            
            // Create a Child Timeline 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
            child1.BeginTime        = TimeSpan.FromMilliseconds(2);
            child1.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            child1.Name             = "Child1";
            
            // Create a Child Timeline 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
            child2.BeginTime        = TimeSpan.FromMilliseconds(4);
            child2.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            child2.Name             = "Child2";

            //Attach Handlers to the Timelines
            AttachAllHandlers( child1 );
            AttachAllHandlers( child2 );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Child Timelines ");
            
            //Attach the Children to the Parent Timeline
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached Children to the Parent ");

            // Create a Clock, connected to the Timeline.
            Clock clock = parent.CreateClock();      
            DEBUG.ASSERT(clock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, clock, tMan, 0, 11, 1, ref outString);

            WriteAllEvents();
            
            return outString;   
        }
    }
}
