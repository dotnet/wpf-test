// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Current method for the Timeline's TimeNodeEnumerator for children.
 */

//Instructions:
//   1. Create a TimeManager
//   2. Create TimeContainer1
//   3. Create TimeContainer2
//   4. Attach the TimeContainers to the TimeManager
//   5. Obtain an Enumerator object

//Pass Condition:
//   This test passes if the Timeline object returned by Current matches the Timelines
//   in the tree.


//Pass Verification:
//   The output of this test should match the expected output in CurrentContainerTMExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected CurrentContainerTMExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class CurrentContainerTM :ITimeBVT
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

            // Create Time Node 1 [ Setting StatusOfNextUse ]
            ParallelTimeline tNode1 = new ParallelTimeline( null , Duration.Automatic);    
            DEBUG.ASSERT(tNode1 != null, "Cannot create tNode1", " Created tNode1 ");
            tNode1.Name = "TimeNode1";

            // Create Time Node 2 [ NOT Setting StatusOfNextUse ]
            ParallelTimeline tNode2 = new ParallelTimeline( null , Duration.Automatic);    
            DEBUG.ASSERT(tNode2 != null, "Cannot create tNode2", " Created tNode2 ");
            //tNode2.StatusOfNextUse = System.Windows.UseStatus.ChangeableReference;
            tNode2.Name = "tNode2";

            // Create Time Node 3 [ Setting StatusOfNextUse ]
            ParallelTimeline tNode3 = new ParallelTimeline( null , Duration.Automatic);    
            DEBUG.ASSERT(tNode3 != null, "Cannot create tNode2", " Created tNode2 ");
            tNode3.Name = "tNode3";
            
            //Attach TimeNodes to the TimeContainer
            tContainer.Children.Add(tNode1);
            tNode1.Children.Add(tNode2);
            tNode2.Children.Add(tNode3);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the container.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            IEnumerator E = ((IEnumerable)tContainer.Children).GetEnumerator();
            
            //Must MoveNext() once initially to get to the first child.
            E.MoveNext();           
            
            bool Result1 = Object.Equals(E.Current, tNode1);
            
            IEnumerator E1 = ((IEnumerable)tNode1.Children).GetEnumerator();
            
            E1.MoveNext();          
            
            bool Result2 = Object.Equals(E1.Current, tNode2);
            
            IEnumerator E2 = ((IEnumerable)tNode2.Children).GetEnumerator();
            
            E2.MoveNext();          
            
            bool Result3 = Object.Equals(E2.Current, tNode3);
            
            return Convert.ToString(Result1) + "\n" + Convert.ToString(Result2) + "\n" + Convert.ToString(Result3);
        }
    }
}
