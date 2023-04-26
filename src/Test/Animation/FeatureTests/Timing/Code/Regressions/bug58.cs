// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify events fire for a Child with Duration=0 in the Reverse time of the Parent

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug58Expect.txt.
//  NOTE: with the new Eventing model, if a Timeline begins and ends in the same Tick
//        (in this case, because Duration=0), only one CurrentStateInvalidated event is fired.

//Warnings:
//  Any changes made in the output should be reflected bug58Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug58 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime         = TimeSpan.FromMilliseconds(20);
               tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(60));
               tContainer.AutoReverse       = true;
               tContainer.RepeatBehavior    = new RepeatBehavior(2);
               tContainer.Name                = "Container";
               //Attach Event Handlers to tContainer
               AttachCurrentStateInvalidated( tContainer );

               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime             = TimeSpan.FromMilliseconds(50);
               tNode1.Duration              = new Duration(TimeSpan.FromMilliseconds(0));
               tNode1.Name                    = "TimeNode1";
               //Attach Event Handlers to tNode1
               AttachCurrentStateInvalidated( tNode1 );

               //Attach Timelines to the Container
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached Timelines to the Container ");

               // Create a Clock, connected to the Timeline.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );
               
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 250, 10, ref outString);
               
               return outString;
          }
          public override void OnProgress( Clock subject )
          {
          }
     }
}
