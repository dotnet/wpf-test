// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the equality operation for the Timeline's TimeNodeEnumerator.
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeContainer and four Timelines added as Children.
//  3. Obtain two Enumerator objects and compare this with the earlier one using ==

//Pass Condition:
//   This test passes if only the comparison returns false.

//Pass Verification:
//   The output of this test should match the expected output in EqualityTMExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected EqualityTMExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class EqualityTM :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.Name = "TimeContainer";

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode1" , " Created TimeNode1 " );

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode2" , " Created TimeNode3 " );

            // Create TimeNode 3
            ParallelTimeline tNode3 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode3" , " Created TimeNode3 " );

            // Create TimeNode 4
            ParallelTimeline tNode4 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );
            
            //Attach TimeNodes to each other.
            tContainer.Children.Add(tNode1);
            tNode1.Children.Add(tNode2);
            
            tContainer.Children.Add(tNode3);
            tNode3.Children.Add(tNode4);
            DEBUG.LOGSTATUS(" Attached TimeNodes to each other ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            IEnumerator E1 = ((IEnumerable)tNode1.Children).GetEnumerator();
            IEnumerator E2 = ((IEnumerable)tNode3.Children).GetEnumerator();
                        
            return Convert.ToString( E1 == E2 ) ;
        }
    }
}
