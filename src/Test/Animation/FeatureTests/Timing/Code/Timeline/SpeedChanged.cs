// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CurrentGlobalSpeedInvalidated when Pause and Resume are invoked
  */

//Instructions:
//    1. Create a Timeline
//    2. Attach a SpeedChanged event handler
//    3. Create a Clock, connected to the Timeline
//    4. Start the TimeManager with Start=0 and Step = 10
//    5. Invoke Pause() and Resume() on the Clock

//Pass Condition:
//   This test passes if the SpeedChanged event fired.

//Pass Verification:
//   The output of this test should match the expected output in SpeedChangedExpect.txt.

//Warnings:
//     Any changes made in the output should be reflected SpeedChangedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
     class SpeedChanged :ITimeBVT
     {
          Clock _tClock;
          
          /*
           *  Function:    Test
           *  Arguments:   Framework
           *  Description: Constructs a Timeline and checks whether events are properly caught.
           *                    Logs the results.
           */
          public override string Test()
          {
               // Intialize output String
               outString = "";
               
               // Create a TimeManager
               TimeManagerInternal tMan = EstablishTimeManager(this);
               DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a parent Timeline
               ParallelTimeline timeline = new ParallelTimeline();
               DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
               // Set properties directly on the Timeline.
               timeline.BeginTime    = TimeSpan.FromMilliseconds(0);
               timeline.Duration     = new System.Windows.Duration(TimeSpan.FromMilliseconds(8));
               timeline.Name           = "Timeline";
               DEBUG.LOGSTATUS(" Set Timeline Properties ");
               
               // Attach Handlers to the Timeline
               AttachCurrentGlobalSpeedInvalidated( timeline );
               DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");

               // Create a Clock, connected to the Timeline.
               _tClock = timeline.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
               //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a click.
               AttachCurrentTimeInvalidatedTC(_tClock);
               
               // Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 15, 1, ref outString);

               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 2 )
                    _tClock.Controller.Pause();

               if ( i == 5 )
                    _tClock.Controller.Resume();
          }

          public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
          {
          }
     }
}
