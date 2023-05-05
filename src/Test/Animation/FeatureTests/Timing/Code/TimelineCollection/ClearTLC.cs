// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify applying the Clear() method to a TimelineCollection
 */

//Instructions:
//   1. Create a Timeline
//   2. Attach Begun and Ended handlers to the Container
//   3. Create a Clock, associated with the Container
//   4. Start the TimeManager

//Warnings:
//   Any changes made in the output should be reflected in ClearTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class ClearTLC :ITimeBVT
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
               tContainer.BeginTime     = TimeSpan.FromMilliseconds(10);
               tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(40));
               tContainer.Name          = "Container";

               // Create TimeNode 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime         = TimeSpan.FromMilliseconds(10);
               tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(40));
               tNode1.FillBehavior      = FillBehavior.HoldEnd;
               tNode1.Name              = "Timeline1";

               // Create TimeNode 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime     = TimeSpan.FromMilliseconds(10);
               tNode2.Duration      = new Duration(TimeSpan.FromMilliseconds(40));
               tNode2.FillBehavior  = FillBehavior.HoldEnd;
               tNode2.Name          = "Timeline2";
               
               //Attach TimeNodes to the TimeContainer
               tContainer.Children.Add(tNode1);
               tContainer.Children.Add(tNode2);
               DEBUG.LOGSTATUS(" Attached Timelines to the TimeContainer ");

               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;
               outString += "----------Count: " + _TLC.Count + "\n";

               _TLC.Clear();
              
               outString += "----------Count: " + _TLC.Count + "\n";
               
               // Create a Clock, connected to the container.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
          
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 50, 10, ref outString);
               
               return outString;
          }
     }
}
