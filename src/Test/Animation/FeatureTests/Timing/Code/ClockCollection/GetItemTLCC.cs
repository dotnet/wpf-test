// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Item property of the ClockCollection
 * 
 */

//Instructions:
//     1. Create a heirarchy of Timelines by creating 4 Timelines and attaching them to
//        each other
//     2. Create a Clock, connected to the container Timeline.
//     3. Establish a ClockCollection, based on the Children of the root Clock

//Warnings:
//     Any changes made in the output should be reflected in GetItemTLCCExpect.txt file

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
     class GetItemTLCC :ITimeBVT
     {
          /*
           *  Function:  Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               // Create a TimeManager
               TimeManagerInternal tMan = EstablishTimeManager(this);
               DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a TimeContainer
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime  = TimeSpan.FromMilliseconds(4);
               tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(8));
               tContainer.Name         = "Container";

               // Create TimeNode 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime   = TimeSpan.FromMilliseconds(1);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode1.Name          = "Timeline1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime   = TimeSpan.FromMilliseconds(2);
               tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode2.Name          = "Timeline2";

               // Create TimeNode 3
               ParallelTimeline tNode3 = new ParallelTimeline();
               DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode 3" , " Created TimeNode 3" );
               tNode3.BeginTime   = TimeSpan.FromMilliseconds(3);
               tNode3.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode3.Name          = "Timeline3";
               
               // Create a heirarchy of Timelines
               tContainer.Children.Add(tNode1);
               tNode1.Children.Add(tNode2);
               tNode2.Children.Add(tNode3);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the container Timeline.
            ClockGroup tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            // Establish a ClockCollection
            ClockGroup tTempClock = (ClockGroup) tClock.Children[0];
            tTempClock = (ClockGroup) tTempClock.Children[0];
            ClockCollection TLCC = (ClockCollection)tTempClock.Children;

               outString += "Count:        " + TLCC.Count + "\n";
               outString += "Name of Child:  " + TLCC[0].Timeline.Name + "\n";

               return outString;
          }
     }
}
