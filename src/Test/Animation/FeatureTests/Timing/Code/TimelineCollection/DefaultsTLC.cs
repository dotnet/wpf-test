// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a TimelineCollection's default properties
 */

//Instructions:
//  1. Create a Timelines, without setting any properties
//  2. Read TimelineCollection properties.

//Pass Condition:
//   This test passes if the actual values of the properties match the expected values.

//Pass Verification:
//   The output of this test should match the expected output in DefaultsTLCExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DefaultsTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class DefaultsTLC :ITimeBVT
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

            // Create a Parent timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent" , " Created Parent " );

            // Create Child 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1 " );

            // Create Child 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2 " );

            // Create Child 3
            ParallelTimeline child3 = new ParallelTimeline();
            DEBUG.ASSERT(child3 != null, "Cannot create Child 3" , " Created Child 3 " );

            // Create Child 4
            ParallelTimeline child4 = new ParallelTimeline();
            DEBUG.ASSERT(child4 != null, "Cannot create Child 4" , " Created Child 4 " );

            // Create a Timeline heirarchy
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            child1.Children.Add(child3);
            child2.Children.Add(child4);

            // Obtain a TimelineCollection for the Container
            TimelineCollection TLC = parent.Children;

            outString += "TimelineCollection  = " +  TLC + "\n";
            outString += "IsFixedSize         = " +  ((IList)TLC).IsFixedSize + "\n";
            outString += "IsReadOnly          = " +  ((IList)TLC).IsReadOnly + "\n";
            outString += "IsSynchronized      = " +  ((IList)TLC).IsSynchronized + "\n";
            outString += "Item                = " +  TLC[1] + "\n";
            outString += "SyncRoot            = " +  ((IList)TLC).SyncRoot;

            return outString;
        }
    }
}
