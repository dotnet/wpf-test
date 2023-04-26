// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify ClockController.Begin()
 *   
 */

//Instructions:
//   1. Create a Timeline 
//   2. Create a Clock, connecting it to the Timeline.
//   3. Start the TimeManager.
//   4. At 2ms, invoke Begin() on the Clock.

//Warnings:
//   Any changes made in the output should be reflected in BeginExpect.txt file

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
     class Begin :ITimeBVT
     {
          Clock  _timelineClock;
          /*
           *  Function:  Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               // Create a TimeManager
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a Timeline
               ParallelTimeline timeline = new ParallelTimeline();
               DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
               timeline.BeginTime   = null; //Can only begin the Timeline interactively.
               timeline.Duration    = new Duration(TimeSpan.FromMilliseconds(8));
               timeline.Name          = "Timeline";
               DEBUG.LOGSTATUS(" Set Timeline Properties ");

               // Create a Clock, connected to the Timeline.
               _timelineClock = timeline.CreateClock();          
               DEBUG.ASSERT(_timelineClock != null, "Cannot create Clock" , " Created Clock " );

               //Attach Handlers
               AttachCurrentStateInvalidatedTC( _timelineClock );
               DEBUG.LOGSTATUS(" Attached EventHandlers");

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _timelineClock, tManager, 0, 15, 1, ref outString);
               
               return outString;
          }

          public override void PreTick(int i)
          {
               if ( i == 4 )
               {
                    _timelineClock.Controller.Begin();
               }
          }
     }
}
