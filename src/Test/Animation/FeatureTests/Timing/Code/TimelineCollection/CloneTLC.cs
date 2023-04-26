// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Clone() method for a TimelineCollection
 */

//Instructions:
//     1. Create three Timelines, a parent and two children
//     2. Obtain a TimelineCollection
//     5. Use Clone() to copy the TimelineCollection

//Warnings:
//     Any changes made in the output should be reflected in CloneTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class CloneTLC :ITimeBVT
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
               ParallelTimeline parent = new ParallelTimeline();
               DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
               parent.BeginTime     = TimeSpan.FromMilliseconds(4);
               parent.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
               parent.Name          = "Parent";

               // Create TimeNode 1
               ParallelTimeline child1 = new ParallelTimeline();
               DEBUG.ASSERT(child1 != null, "Cannot create Child 1" , " Created Child 1" );
               child1.BeginTime     = TimeSpan.FromMilliseconds(1);
               child1.Duration      = new Duration(TimeSpan.FromMilliseconds(2));
               child1.Name          = "Child1";

               // Create TimeNode 2
               ParallelTimeline child2 = new ParallelTimeline();
               DEBUG.ASSERT(child2 != null, "Cannot create Child 2" , " Created Child 2" );
               child2.BeginTime     = TimeSpan.FromMilliseconds(3);
               child2.Duration      = new Duration(TimeSpan.FromMilliseconds(4));
               child2.Name          = "Child2";
               
               // Attach the Children to the Parent
               parent.Children.Add(child1);
               parent.Children.Add(child2);
               DEBUG.LOGSTATUS(" Attached Children to the Parent ");

               // Obtain a TimelineCollection for the Parent
               TimelineCollection TLC1 = parent.Children;

               //Create a new TimelineCollection using Clone()
               TimelineCollection TLC2 = TLC1.Clone();
               DEBUG.LOGSTATUS(" Applied Clone ");

               outString += "TLC1.Count:       " + TLC1.Count + "\n";
               outString += "TLC2.Count:       " + TLC2.Count + "\n\n";

               outString += "TLC1[0].Name:     " + TLC1[0].Name + "\n";
               outString += "TLC1[1].Name:     " + TLC1[1].Name + "\n";
               outString += "TLC2[0].Name:     " + TLC2[0].Name + "\n";
               outString += "TLC2[1].Name:     " + TLC2[1].Name + "\n\n";

               outString += "TLC1[0].BeginTime:    " + TLC1[0].BeginTime + "\n";
               outString += "TLC1[1].BeginTime:    " + TLC1[1].BeginTime + "\n";
               outString += "TLC2[0].BeginTime:    " + TLC2[0].BeginTime + "\n";
               outString += "TLC2[1].BeginTime:    " + TLC2[1].BeginTime + "\n\n";

               outString += "TLC1[0].Duration:     " + TLC1[0].Duration + "\n";
               outString += "TLC1[1].Duration:     " + TLC1[1].Duration + "\n";
               outString += "TLC2[0].Duration:     " + TLC2[0].Duration + "\n";
               outString += "TLC2[1].Duration:     " + TLC2[1].Duration + "\n";

               return outString;
          }
     }
}
