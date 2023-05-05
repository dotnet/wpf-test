// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Insert() method for the TimelineCollection
 */

//Instructions:
//     1. Create three Timelines
//     2. Attach the second Timeline to the first, which is now the container
//     3. Obtain a TimelineCollection
//     4. Insert the third timeline into the TimelineCollection

//Warnings:
//     Any changes made in the output should be reflected in InsertTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class InsertTLC :ITimeBVT
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
               tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(50));
               tContainer.Name         = "Container";

               // Create TimeNode 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime   = TimeSpan.FromMilliseconds(10);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(20));
               tNode1.Name          = "Timeline1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime   = TimeSpan.FromMilliseconds(20);
               tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(20));
               tNode2.Name          = "Timeline2";
               
               //Attach TimeNode to the TimeContainer
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");
               
               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;

               outString += "Count Before:    " + _TLC.Count + "\n";
               outString += "Child #1 Before: " + _TLC[0].Name + "\n";
               
               //Insert TimeNode 2 in front of TimeNode 1
               _TLC.Insert(0, tNode2);

               outString += "Count After:     " + _TLC.Count + "\n";
               outString += "Child #1 After:  " + _TLC[0].Name + "\n";
               outString += "Child #2 After:  " + _TLC[1].Name + "\n";
 
               return outString;
          }
     }
}
