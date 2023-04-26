// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Clock property of a ClockController
*/

//Instructions:
//   1. Create a Timeline 
//   2. Create a Clock, connecting it to the Timeline.
//   3. Start the TimeManager.

//Warnings:
//     Any changes made in the output should be reflected in ICClockExpect.txt file

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
     class ICClock :ITimeBVT
     {
          Clock  _timelineClock;
          /*
           *  Function:  Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               // Create a TimeManager
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a Timeline
               ParallelTimeline timeline = new ParallelTimeline();
               DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
               timeline.BeginTime   = TimeSpan.FromMilliseconds(0);
               timeline.Duration    = new Duration(TimeSpan.FromMilliseconds(8));
               timeline.Name          = "Timeline";
               DEBUG.LOGSTATUS(" Set Timeline Properties ");

               // Create a Clock, connected to the Timeline.
               _timelineClock = timeline.CreateClock();          
               DEBUG.ASSERT(_timelineClock != null, "Cannot create Clock" , " Created Clock " );
               
               outString += _timelineClock.Controller.Clock + "\n";
               outString += _timelineClock.Controller.Clock.CurrentTime + "\n";
               outString += _timelineClock.Controller.Clock.Timeline.Name;

               return outString;
          }
     }
}
