// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Remove() method for the TimelineCollection
 */

//Instructions:
//     1. Create three Timelines
//     2. Attach the second and third Timelines to the first, which is now the container
//     3. Obtain a TimelineCollection
//     4. Remove the first Timeline from the TimelineCollection

//Warnings:
//     Any changes made in the output should be reflected in RemoveTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class RemoveTLC :ITimeBVT
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
               tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(4));
               tContainer.Name         = "Container";

               // Create TimeNode 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime  = TimeSpan.FromMilliseconds(1);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(2));
               tNode1.Name                = "Timeline1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime  = TimeSpan.FromMilliseconds(0);
               tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(3));
               tNode2.Name          = "Timeline2";
               
               //Attach TimeNode to the TimeContainer
               tContainer.Children.Add(tNode1);
               tContainer.Children.Add(tNode2);
               DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");
               
               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;

               outString += "Count Before: " + _TLC.Count + "\n";
               
               //Remove TimeNode 1 from the TimelineCollection
               _TLC.Remove(tNode1);

               outString += "Count After:  " + _TLC.Count + "\n";
               outString += "Child After:  " + _TLC[0].Name + "\n";

               // Create a Clock, connected to the container.
               ClockGroup tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
               
               outString += "Clock Children In Loop:  " + tClock.Children.Count + "\n";
          
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 5, 1, ref outString);
 
               outString += "Count After Loop:  " + _TLC.Count + "\n";
               outString += "Child After Loop:  " + _TLC[0].Name + "\n";

               return outString;
          }
          
          public override void OnProgress( Clock subject )
          {
               if ( ((Clock)subject).Timeline.Name == "Container")
               {
                outString += "Clock Children In Loop:  " + ((ClockGroup)subject).Children.Count + "\n";
               }
          }
     }
}
