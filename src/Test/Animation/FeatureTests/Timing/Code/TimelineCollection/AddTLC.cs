// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Add() method for the TimelineCollection
 */

//Instructions:
//     1. Create three Timelines
//     2. Attach the second Timeline to the first, which is now the container
//     3. Obtain a TimelineCollection
//     4. Add the third timeline to the TimelineCollection

//Warnings:
//     Any changes made in the output should be reflected in AddTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class AddTLC :ITimeBVT
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
               tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(500));
               tContainer.Name         = "Container";

               // Create TimeNode 1
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create TimeNode " , " Created TimeNode " );
               tNode.BeginTime   = TimeSpan.FromMilliseconds(100);
               tNode.Duration = new Duration(TimeSpan.FromMilliseconds(200));
               tNode.Name          = "Timeline1";
               
               //Attach TimeNode to the TimeContainer
               //tContainer.Children.Add(tNode);
               //DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");
               
               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;

               outString += "Count Before:    " + _TLC.Count + "\n";
               
               //Add TimeNode 2 in front of TimeNode 1
               _TLC.Add(tNode);

               outString += "Count After:     " + _TLC.Count + "\n";
               outString += "Child After:  " + _TLC[0].Name + "\n";
 
               return outString;
          }
     }
}
