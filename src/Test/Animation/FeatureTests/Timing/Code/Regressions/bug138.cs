// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Begin values from two different trees are comparable

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create two Timeline trees
//  3. Create Clocks and associate them with the parent Timelines

//Pass Condition:
//   This test passes if the Begin comparison returns true

//Pass Verification:
//   The output of this test should match the expected output in b.138Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug138Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug138 : ITimeBVT
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

            // Create parent Timeline 1
            ParallelTimeline parent1 = new ParallelTimeline();
            DEBUG.ASSERT(parent1 != null, "Cannot create Parent 1", " Created Parent 1");
            parent1.BeginTime   = TimeSpan.FromMilliseconds(0);
            parent1.Duration    = new Duration(TimeSpan.FromMilliseconds(20));
            parent1.Name          = "Parent1";

            // Create child Timeline 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
            child1.BeginTime    = TimeSpan.FromMilliseconds(2);
            child1.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            child1.Name           = "Child1";

            // Create parent Timeline 2 
            ParallelTimeline parent2 = new ParallelTimeline();
            DEBUG.ASSERT(parent2 != null, "Cannot create Parent 2", " Created Parent 2");
            parent2.BeginTime   = TimeSpan.FromMilliseconds(0);
            parent2.Duration    = new Duration(TimeSpan.FromMilliseconds(20));
            parent2.Name          = "Parent2";

            // Create child Timeline 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
            child2.BeginTime    = TimeSpan.FromMilliseconds(2);
            child2.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            child2.Name           = "Child2";

            outString += " Child1.BeginTime == Child2.BeginTime : " + (child1.BeginTime == child2.BeginTime).ToString() + "\n\n";

            //Attach the children to their parents
            parent1.Children.Add(child1);
            parent2.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached Children to Parents ");

            outString += " Child1.BeginTime == Child2.BeginTime : " + (child1.BeginTime == child2.BeginTime).ToString() + "\n\n";

            // Create Clocks, connected to the Parent Timelines.
            Clock tClock1 = parent1.CreateClock();
            DEBUG.ASSERT(tClock1 != null, "Cannot create Clock 1" , " Created Clock 1" );
            Clock tClock2 = parent2.CreateClock();
            DEBUG.ASSERT(tClock2 != null, "Cannot create Clock 2" , " Created Clock 2" );

            outString += " Child1.BeginTime == Child2.BeginTime : " + (child1.BeginTime == child2.BeginTime).ToString() + "\n\n";

            return outString;
        }
    }
}
