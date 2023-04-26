// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CurrentGlobalSpeedInvalidated event fires during Deceleration.
 */

//Instructions:
//    1. Create a Timelines, with Deceleration = .7
//    2. Attach a CurrentGlobalSpeedInvalidated event handler to the Timeline
//    4. Start the TimeManager with Start=0 and Step = 1


//Pass Condition:
//   This test passes if the CurrentGlobalSpeedInvalidated event fired when appropriate.

//Pass Verification:
//   The output of this test should match the expected output in SpeedChangedDecelExpect.txt.

//Warnings:
//     Any changes made in the output should be reflected SpeedChangedDecelExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
     class SpeedChangedDecel :ITimeBVT
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
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
              
              // Set properties directly on the Timeline.
               tNode.BeginTime          = TimeSpan.FromMilliseconds(0);
               tNode.Duration           = new System.Windows.Duration(TimeSpan.FromMilliseconds(50));
               tNode.DecelerationRatio  = .2;
               tNode.Name                 = "Timeline";
               DEBUG.LOGSTATUS(" Set Timeline Properties ");
               
               // Attach Handler to the Timeline
               AttachCurrentGlobalSpeedInvalidated(tNode);
               DEBUG.LOGSTATUS(" Attached EventHandler to the Timeline ");

               // Create a Clock, connected to the Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               // Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 52, 1, ref outString);

               return outString;
          }
     }
}
