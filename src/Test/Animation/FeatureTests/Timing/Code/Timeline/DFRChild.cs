// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify SetDesiredFrameRate() on parent and child Timelines
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with Begin = 0, Duration = 25, and DFR = 100
//  3. Create additional Timelines and add them as chidren.
//  4. Set DFR on the children -- it should be ignored.

//Pass Condition:
//   This test passes if the methods affect the CurrentTimeInvalidated event appropriately.
//   In this case, it should not be affected, because DFR is only supported on the root Timeline.

//Pass Verification:
//   The output of this test should match the expected output in DFRChildExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in DFRChildExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class DFRChild : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Timeline" , " Created Timeline " );
            parent.BeginTime        = TimeSpan.FromMilliseconds(0);
            parent.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(25));
            parent.Name             = "Parent";
            
            // Create a Child Timeline 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
            child1.BeginTime        = TimeSpan.FromMilliseconds(0);
            child1.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(15));
            child1.Name             = "Child1";
            
            // Create a Child Timeline 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
            child2.BeginTime        = TimeSpan.FromMilliseconds(10);
            child2.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            child2.Name             = "Child2";
            
            //Attach the Children to the Parent Timeline
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached Children to the Parent ");
            
            // Set a desired frame rate.
            Timeline.SetDesiredFrameRate(parent, 200);
            Timeline.SetDesiredFrameRate(child1, 500);
            Timeline.SetDesiredFrameRate(child2, 300);

            // Create a Clock, connected to the tContainer.
            ClockGroup tClock = parent.CreateClock();     
            DEBUG.ASSERT(tClock != null, "Cannot create Clock for Timeline" , " Created Clock for Timeline " );
            
            AttachCurrentTimeInvalidatedTC(tClock);
            AttachCurrentTimeInvalidatedTC(tClock.Children[0]);
            AttachCurrentTimeInvalidatedTC(tClock.Children[1]);

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 52, 1, ref outString);
        
            return outString;
        }

        public override void OnProgress(Clock subject)
        {
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
            outString += "----CurrentTimeInvalidated fired -- " + ((Clock)subject).Timeline.Name + "\n";
        }
    }
}
