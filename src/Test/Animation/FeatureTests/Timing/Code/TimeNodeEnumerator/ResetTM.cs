// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Reset method for a Timeline's TimeNodeEnumerator.
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create four TimeNodes
//  3. Obtain an Enumerator object

//Pass Condition:
//   This test passes if Reset sets the enumerator to its initial position, which is before the 
//   first child of the root Timeline.

//Pass Verification:
//   The output of this test should match the expected output in ResetTMExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected ResetExpectTM.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ResetTM :ITimeBVT
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
            tNode1.Name ="TimeNode1";

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode2", " Created TimeNode2 ");
            tNode2.Name ="TimeNode2";

            // Create TimeNode 3
            ParallelTimeline tNode3 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode3", " Created TimeNode3 ");
            tNode3.Name ="TimeNode3";

            // Create TimeNode 4
            ParallelTimeline tNode4 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );
            tNode4.Name = "TimeNode4";

            // Create TimeNode 5
            ParallelTimeline tNode5 = new ParallelTimeline( null , Duration.Automatic);
            DEBUG.ASSERT(tNode5 != null, "Cannot create TimeNode4" , " Created TimeNode4 " );
            tNode4.Name = "TimeNode4";
        
            //Attach TimeNodes to the TimeContainer
            tRoot.Children.Add(tNode1);
            tRoot.Children.Add(tNode2);
            tRoot.Children.Add(tNode3);
            tRoot.Children.Add(tNode4);
            tNode4.Children.Add(tNode5);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer or each other ");

            // Create a Clock, connected to the container.
            Clock tClock = tRoot.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Obtain an Enumerator object.
            IEnumerator E = ((IEnumerable)tRoot.Children).GetEnumerator();
            DEBUG.LOGSTATUS(" Called GetEnumerator ");
            
            //Iterate.
            Timeline timeLine;
            
            while (E.MoveNext())
            {
                timeLine = (Timeline)E.Current;
                outString += timeLine.Name + "\n";
            }
            
            E.Reset();
                    
            while (E.MoveNext())
            {
                timeLine = (Timeline)E.Current;
                outString += timeLine.Name + "\n";
            }
                        
            return outString;
        }
    }
}
