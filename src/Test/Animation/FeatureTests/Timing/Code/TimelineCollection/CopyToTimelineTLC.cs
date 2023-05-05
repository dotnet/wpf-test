// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CopyTo(Timeline[],int32) method for the TimelineCollection
 */

//Instructions:
//     1. Create five Timelines
//     2. Attach Timelines 3-4 to the first, which is now the container
//     3. Create an array of Timelines and add Timelines 1-2 to it
//     4. Obtain a TimelineCollection
//     5. Use CopyTo() to copy Timelines 3-4 to the array of Timelines

//Warnings:
//     Any changes made in the output should be reflected in CopyToTimelineTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class CopyToTimelineTLC :ITimeBVT
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
               tNode1.BeginTime         = TimeSpan.FromMilliseconds(1);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode1.Name                = "Timeline1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime         = TimeSpan.FromMilliseconds(2);
               tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode2.Name                = "Timeline2";

               // Create TimeNode 3
               ParallelTimeline tNode3 = new ParallelTimeline();
               DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode 3" , " Created TimeNode 3" );
               tNode3.BeginTime         = TimeSpan.FromMilliseconds(3);
               tNode3.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode3.Name                = "Timeline3";

               // Create TimeNode 4
               ParallelTimeline tNode4 = new ParallelTimeline();
               DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode 4" , " Created TimeNode 4" );
               tNode4.BeginTime         = TimeSpan.FromMilliseconds(4);
               tNode4.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode4.Name                = "Timeline4";
               
               // Attach TimeNodes to the TimeContainer
               tContainer.Children.Add(tNode3);
               tContainer.Children.Add(tNode4);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

               // Create an array of Timelines and assign to Timelines to it
               Timeline[] timelineArray = new Timeline[4];
               timelineArray[0] = tNode1;
               timelineArray[1] = tNode2;
               DEBUG.LOGSTATUS(" Created array of Timelines ");

               outString += "Timeline[0].Name Before:    " + timelineArray[0].Name + "\n";
               outString += "Timeline[1].Name Before:    " + timelineArray[1].Name + "\n";

               // Obtain a TimelineCollection for the Container
               TimelineCollection TLC1 = tContainer.Children;

               //Copy the TimelineCollection to the Timeline array using CopyTo()
               TLC1.CopyTo(timelineArray,2);
               DEBUG.LOGSTATUS(" Applied CopyTo ");

               outString += "Timeline[0].Name After:     " + timelineArray[0].Name + "\n";
               outString += "Timeline[1].Name After:     " + timelineArray[1].Name + "\n";
               outString += "Timeline[2].Name After:     " + timelineArray[2].Name + "\n";
               outString += "Timeline[3].Name After:     " + timelineArray[3].Name + "\n";

               return outString;
          }
     }
}
