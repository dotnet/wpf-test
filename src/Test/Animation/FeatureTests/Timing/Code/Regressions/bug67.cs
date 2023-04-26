// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify a container's Ended event fires when synced to a child and ending via SkipToFill() 
 *               NOTE: originally, this test invoked BeginIn(1) and EndIn(2).
 *               NOTE: originally, this test invoked TimeEndSync.FirstChild.
 *               NOTE: originally, this test involved setting EndIn() on a child.  This is no
 *                     longer supported, so now SkipToFill() is invoked on the parent.


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager; at the 3rd tick, invoke Begin() on the child; on the 8th tick,
//     invoke SkipToFill()

//Pass Condition:
//  The Ended event on the container should fire when its child ends, which is immediately,
//  because the child has Duration=Forever.

//Pass Verification:
//  The output of this test should match the expected output in bug67Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug67Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug67 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          ClockGroup            _tClock;
          
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               //Verify events by listing them at the end.
               eventsVerify = true;

               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime    = null;
               tContainer.Name           = "Container";

               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime    = TimeSpan.FromMilliseconds(0);
               tNode1.Duration     = new System.Windows.Duration(new TimeSpan(0,0,0,0,10));
               tNode1.Name           = "TimeNode1";
              
               //Attach TimeNode to the Container
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

               // Create a Clock, connected to the container Timeline.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Attach Event Handlers to tContainer
               AttachCurrentStateInvalidatedTC( _tClock );
               AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
               
               //Run the Timer               
               TimeGenericWrappers.TESTSTATE( this, _tClock, _tManager, 0, 12, 1, ref outString);

               WriteAllEvents();
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 3 )
               {
                    _tClock.Controller.Begin();
               }
               if ( i == 7 )
               {
                    _tClock.Controller.SkipToFill();
               }
          }          
     }
}
