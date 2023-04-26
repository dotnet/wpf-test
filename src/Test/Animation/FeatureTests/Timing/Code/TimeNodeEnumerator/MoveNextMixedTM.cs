// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the MoveNext method for the TimeManager's TimeNodeEnumerator for 
 *              both Time Nodes and Time Containers.
 */
 
//Instructions:
//   1. Create a TimeManager
//   2. Create four TimeNodes and four TimeContainers
//   4. Add the nodes to the TimeManager
//   5. Obtain an Enumerator object

//Pass Condition:
//   This test passes if MoveNext succeeds in iterating through the Time Manager's child nodes.
//   Only the Time Mananger's immediate children should be accessed.

//Pass Verification:
//   The output of this test should match the expected output in MoveNextMixedTMExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected MoveNextMixedTMExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class MoveNextMixedTM :ITimeBVT
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
            ParallelTimeline tRoot = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tRoot != null, "Cannot create TimeContainer tRoot", " Created TimeContainer tRoot");
            tRoot.Name = "Root";

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode1", " Created TimeNode1 ");
            tNode1.Name = "TimeNode1";

            // Create TimeContainer 1
            ParallelTimeline tContainer1 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tContainer1 != null, "Cannot create TimeContainer1", " Created ParallelTimeContainer1 ");
            tContainer1.Name = "TimeContainer1";

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode2", " Created TimeNode2 ");
            tNode2.Name ="TimeNode2";

            // Create TimeContainer 2
            ParallelTimeline tContainer2 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tContainer2 != null, "Cannot create TimeContainer2", " Created ParallelTimeContainer2 ");
            tContainer2.Name = "TimeContainer2";

            // Create TimeNode 3
            ParallelTimeline tNode3 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode3", " Created TimeNode3 ");
            tNode3.Name ="TimeNode3";

            // Create TimeContainer 3
            ParallelTimeline tContainer3 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tContainer3 != null, "Cannot create TimeContainer3", " Created ParallelTimeContainer3 ");
            tContainer3.Name = "TimeContainer3";

            // Create TimeNode 4
            ParallelTimeline tNode4 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );
            tNode4.Name ="TimeNode4";
            
            // Create TimeContainer 4
            ParallelTimeline tContainer4 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tContainer4 != null, "Cannot create TimeContainer4", " Created ParallelTimeContainer4 ");
            tContainer4.Name = "TimeContainer4";

            //Attach TimeNodes to the TimeContainer
            tRoot.Children.Add(tContainer1);
            tRoot.Children.Add(tContainer2);
            tRoot.Children.Add(tContainer3);
            tRoot.Children.Add(tNode1);
            tRoot.Children.Add(tNode2);
            tContainer3.Children.Add(tNode3); //Should not be picked up by the Enumerator.
            tRoot.Children.Add(tNode4);
            tRoot.Children.Add(tContainer4);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tRoot.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Obtain an Enumerator object.
            IEnumerator E = ((IEnumerable)tRoot.Children).GetEnumerator();
            DEBUG.LOGSTATUS(" Called GetEnumerator ");
            
            string outString = "";
            
            //Iterate through the TimeManager's children, invoking MoveNext() each time.
            //Must MoveNext() once initially to get to the first child.
            
            Timeline timeLine;          
            while (E.MoveNext())
            {
                timeLine = (Timeline)E.Current;
                outString += timeLine.Name + "\n";
            }
            return outString;
        }
    }
}
