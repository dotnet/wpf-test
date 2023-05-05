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

//Note:
//     Setting the StatusOfNextUse property of the two child timelines to
//     UseStatus.ChangeableReference, so that when the timelines are passed to IndexOf,
//     they refer to the originals, rather than being copies (default).  Therefore, they will
//     be successfully found -before- a Clock is created.
//Warnings:
//     Any changes made in the output should be reflected in IndexOfTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class IndexOfTLC :ITimeBVT
     {
          TimelineCollection    _TLC;
          ParallelTimeline      _tNode1;
          ParallelTimeline      _tNode2;
          ClockGroup            _tClock;
          
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
               tContainer.BeginTime     = TimeSpan.FromMilliseconds(4);
               tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
               tContainer.Name            = "Container";

               // Create TimeNode 1
               _tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               _tNode1.BeginTime         = TimeSpan.FromMilliseconds(1);
               _tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(2));
               _tNode1.Name                = "Timeline1";

               // Create TimeNode 2
               _tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               _tNode2.BeginTime         = TimeSpan.FromMilliseconds(2);
               _tNode2.Duration          = new Duration(TimeSpan.FromMilliseconds(2));
               _tNode2.Name                = "Timeline2";
               
               //Attach TimeNode to the TimeContainer
               tContainer.Children.Add(_tNode1);
               tContainer.Children.Add(_tNode2);
               DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");
               
               //Obtain a TimelineCollection for the Container
               _TLC = tContainer.Children;

               outString += "IndexOf Child 1:  " + _TLC.IndexOf(_tNode1) + "\n";
               outString += "IndexOf Child 2:  " + _TLC.IndexOf(_tNode2) + "\n";

               // Create a Clock, connected to the container.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
          
            outString += "IndexOf Child 1:  " + _TLC.IndexOf(_tClock.Children[0].Timeline) + "\n";
            outString += "IndexOf Child 2:  " + _TLC.IndexOf(_tClock.Children[1].Timeline) + "\n";
               
               return outString;
          }
     }
}
