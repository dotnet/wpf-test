// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a frozen child Timeline is pointed to by the corresponding clock child Timeline
 */

//Instructions:
//  1. Create a timeline and attach children to it, and a grandchild to one of the children
//  2. Check equivalency of clock Timelines to the original Timelines.

//Warnings:
//  Any changes made in the output should be reflected in ChildFrozenExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ChildFrozen : ITimeBVT
    {
        /*
         *  Function: Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            parent.FillBehavior  = FillBehavior.HoldEnd;
            parent.Name          = "Parent";
            
            // Create Child Timeline 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
            
            // Create Child Timeline 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
            
            // Create a GrandChild 1 Timeline (a child of Child1)
            ParallelTimeline grandchild1 = new ParallelTimeline();
            DEBUG.ASSERT(grandchild1 != null, "Cannot create GrandChild 1" , " Created GrandChild 1" );
            
            // Create a GrandChild 2 Timeline (a child of Child2)
            ParallelTimeline grandchild2 = new ParallelTimeline();
            DEBUG.ASSERT(grandchild2 != null, "Cannot create GrandChild 2" , " Created GrandChild 2" );
            
            //Attach TimeNode to the Parent
            parent.Children.Add(child1);
            DEBUG.LOGSTATUS(" Attached Child 1 to the Parent ");
            child1.Children.Add(grandchild1);
            DEBUG.LOGSTATUS(" Attached Grandchild 1 to the Child 1 ");
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached Child 2 to the Parent ");
            child2.Children.Add(grandchild2);
            DEBUG.LOGSTATUS(" Attached Grandchild 2 to the Child 2 ");

            // Freeze the child Timeline
            child1.Freeze();
            DEBUG.LOGSTATUS(" Froze Child 1 ");

            // Create a Clock, connected to the Timeline.
            ClockGroup clock = parent.CreateClock();      
            DEBUG.ASSERT(clock != null, "Cannot create Clock" , " Created Clock " );

            outString += (clock.Children[0].Timeline == child1) + "\n";
            outString += (clock.Children[1].Timeline == child2) + "\n";
            outString += (((ClockGroup)clock.Children[0]).Children[0].Timeline == grandchild1) + "\n";
            outString += (((ClockGroup)clock.Children[1]).Children[0].Timeline == grandchild2);

            return outString;   
        }
    }
}
