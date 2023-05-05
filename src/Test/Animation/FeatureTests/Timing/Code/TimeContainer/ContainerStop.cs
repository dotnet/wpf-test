// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify CurrentStateInvalidated when a parent timeline stops before its children 
 */

//Instructions:
//  1. Create a timeline and attach a child to it, and a grandchild to the child
//  2. Check event firing on all three timelines.

//Warnings:
//  Any changes made in the output should be reflected in ContainerStopExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ContainerStop : ITimeBVT
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
            
            // Create a Child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create TimeNode" , " Created TimeNode " );
            child.BeginTime        = TimeSpan.FromMilliseconds(4);
            child.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            child.Name             = "Child";
            
            // Create a GrandChild Timeline
            ParallelTimeline grandchild = new ParallelTimeline();
            DEBUG.ASSERT(grandchild != null, "Cannot create TimeNode" , " Created TimeNode " );
            grandchild.BeginTime        = TimeSpan.FromMilliseconds(4);
            grandchild.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            grandchild.Name             = "GrandChild";
            
            //Attach TimeNode to the TimeContainer
            parent.Children.Add(child);
            child.Children.Add(grandchild);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            //Attach Handlers to the Timelines
            AttachCurrentStateInvalidated( parent );
            AttachCurrentStateInvalidated( child );
            AttachCurrentStateInvalidated( grandchild );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the TimeNode ");

            // Create a Clock, connected to the Timeline.
            Clock clock = parent.CreateClock();      
            DEBUG.ASSERT(clock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, clock, tMan, 0, 15, 1, ref outString);

            WriteAllEvents();
            
            return outString;   
        }
    }
}
