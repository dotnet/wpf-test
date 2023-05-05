// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Progress when a parent Timeline and its children set RepeatCount >= 2

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (parent) and children Timelines
//  3. Create a Clock and associate it with the parent Timeline (parent)

//Pass Condition:
//   This test passes if the CurrentTime property returns 0 for the Children.

//Pass Verification:
//   The output of this test should match the expected output in b.35Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug35Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug35 : ITimeBVT
    {
        TimeManagerInternal _tManager;
        
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     

            // Create a TimeManager
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime         = TimeSpan.FromMilliseconds(0);
            parent.Duration          = new Duration(TimeSpan.FromMilliseconds(50));
            parent.Name                = "Parent";

            // Create child Timeline 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
            child1.BeginTime         = TimeSpan.FromMilliseconds(2);
            child1.Duration          = new Duration(TimeSpan.FromMilliseconds(20));
            child1.RepeatBehavior    = new RepeatBehavior(2);
            child1.Name                = "Child1";

            // Create child Timeline 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
            child2.BeginTime         = TimeSpan.FromMilliseconds(4);
            child2.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            child2.RepeatBehavior    = new RepeatBehavior(2);
            child2.Name                = "Child2";

            // Create child Timeline 3
            ParallelTimeline child3 = new ParallelTimeline();
            DEBUG.ASSERT(child3 != null, "Cannot create Child 3" , " Created Child 3" );
            child3.BeginTime         = TimeSpan.FromMilliseconds(0);
            child3.Duration          = new Duration(TimeSpan.FromMilliseconds(4));
            child3.RepeatBehavior    = new RepeatBehavior(3);
            child3.Name                = "Child3";

            //Attach the Child1/Child2 to the Parent and Child3 to Child1
            child1.Children.Add(child3);
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached Children to Parents ");

            // Create a Clock, connected to the Parent Timeline.
            Clock tClock = parent.CreateClock();
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Event Handlers to tClock
            AttachCurrentGlobalSpeedInvalidatedTC( tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, tClock, _tManager, 0, 52, 1, ref outString);

            return outString;
        }
    }
}
