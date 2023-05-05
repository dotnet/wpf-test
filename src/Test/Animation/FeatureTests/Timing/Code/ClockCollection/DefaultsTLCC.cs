// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a ClockCollection's default properties
 */

//Instructions:
//  1. Create a set of Timelines, without setting any properties.
//  2. Read ClockCollection properties.

//Pass Condition:
//  This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//  The output of this test should match the expected output in DefaultsTLCCExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DefaultsTLCCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class DefaultsTLCC :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and checks default values.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent" , " Created Parent " );

            // Create Child 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1 " );

            // Create Child 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2 " );

            // Add the children to the parent
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            // Create a ClockCollection, connected to the Timeline.
            ClockGroup tClock = parent.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            // Establish a ClockCollection
            ClockCollection TLCC = (ClockCollection)tClock.Children;

            outString += "ClockCollection = " +  TLCC + "\n";
            outString += "Count                   = " +  TLCC.Count + "\n";
            outString += "IsReadOnly              = " +  TLCC.IsReadOnly + "\n";
            outString += "Item                    = " +  TLCC[0];

            return outString;
        }
    }
}
