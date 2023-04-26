// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify applying the Contains() method to a TimelineCollection
 */

//Instructions:
//     1. Create a Timeline
//     2. Attach Begun and Ended handlers to the Container
//     3. Create a Clock, associated with the Container
//     4. Start the TimeManager

//Note:
//     Because StatusOfNextUse is -not- set to UseStatus.ChangeableReference, Contains will
//     return true only when the timeline is obtained from Clock, because otherwise it
//     is a copy, not the original.
//     [2-16-05:  a recent Freezables breaking change affects this scenario:
//     call by reference is now the default 
//Warnings:
//     Any changes made in the output should be reflected in ContainsTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class ContainsTLC :ITimeBVT
     {
          TimelineCollection _TLC;
          ParallelTimeline   _tNode1;
          ParallelTimeline   _tNode2;
          ClockGroup         _tClock;
          
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
               tContainer.BeginTime     = TimeSpan.FromMilliseconds(0);
               tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
               tContainer.Name          = "Container";

               // Create TimeNode 1
               _tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               _tNode1.BeginTime     = TimeSpan.FromMilliseconds(2);
               _tNode1.Duration      = new Duration(TimeSpan.FromMilliseconds(2));
               _tNode1.FillBehavior  = FillBehavior.HoldEnd;
               _tNode1.Name          = "Timeline1";

               // Create TimeNode 2
               _tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               _tNode2.BeginTime     = TimeSpan.FromMilliseconds(4);
               _tNode2.Duration      = new Duration(TimeSpan.FromMilliseconds(2));
               _tNode2.FillBehavior  = FillBehavior.HoldEnd;
               _tNode2.Name          = "Timeline2";

               // Attach TimeNode 1 only to the TimeContainer
               tContainer.Children.Add(_tNode1);
               DEBUG.LOGSTATUS(" Attached Timelines to the TimeContainer ");

               // Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;

               outString += "-a---------Contains 1: " + _TLC.Contains(_tNode1).ToString() + "\n";
               outString += "-b---------Contains 2: " + _TLC.Contains(_tNode2).ToString() + "\n";
               
               // Create a Clock, connected to the container.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               outString += "-a---------Contains 1: " + _TLC.Contains(_tNode1).ToString() + "\n";
               outString += "-b---------Contains 2: " + _TLC.Contains(_tNode2).ToString() + "\n";
          
               // Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 9, 1, ref outString);
              
               outString += "-d---------Contains 1: " + _TLC.Contains(_tNode1).ToString() + "\n";
               outString += "-e---------Contains 2: " + _TLC.Contains(_tNode2).ToString() + "\n";
              
               outString += "-f---------Contains 1: " + _TLC.Contains(_tClock.Children[0].Timeline).ToString() + "\n";
               
               return outString;
          }
          public override void OnProgress( Clock subject )
          {
               if ( ((Clock)subject).Timeline.Name == "Timeline 1")
               {
                outString += "-c---------Contains 1 : " + _TLC.Contains(((Clock)subject).Timeline).ToString() + "\n";
               }
          }
     }
}
