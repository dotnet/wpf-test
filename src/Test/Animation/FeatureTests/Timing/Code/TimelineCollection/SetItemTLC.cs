// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting the Item property of the TimelineCollection
 */

//Instructions:
//     1. Create three Timelines
//     2. Attach the second Timeline to the first, which is now the container
//     3. Obtain a TimelineCollection
//     4. Insert the third timeline into the TimelineCollection

//Warnings:
//     Any changes made in the output should be reflected in SetItemTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class SetItemTLC :ITimeBVT
     {
          TimelineCollection _TLC;
          
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
               tContainer.BeginTime  = TimeSpan.FromMilliseconds(0);
               tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(1));
               tContainer.Name         = "Container";

               // Create Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime   = TimeSpan.FromMilliseconds(0);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(1));
               tNode1.Name          = "Timeline1";

               // Create Timeline 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime   = TimeSpan.FromMilliseconds(0);
               tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(1));
               tNode2.Name          = "Timeline2";
               
               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;
               
               //Attach TimeNode to the TimeContainer
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

               //Set item, thereby replacing Timeline 1 with Timeline 2.
               _TLC[0] = tNode2;
               
               outString += "Count:    " + _TLC.Count + "\n";
               outString += "Get item returns Child #1: " + (_TLC[0].Equals(tNode2)).ToString() + "\n";
               outString += "Child #1 Name: " + _TLC[0].Name + "\n";

               return outString;
          }
     }
}
