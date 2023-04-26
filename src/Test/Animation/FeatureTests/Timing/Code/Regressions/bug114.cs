// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking a Seek() after a Pause()

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 11th tick, Seek to 0

//Pass Condition:
//  The events should fire appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug114Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug114Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug114 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          ParallelTimeline      _tNode;
          Clock                 _tClock;
          
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

               // Create a TimeContainer
               _tNode = new ParallelTimeline();
               DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
               _tNode.BeginTime      = TimeSpan.FromMilliseconds(0);
               _tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(10));
               _tNode.Name             = "Timeline";
               
               //Attach Handlers to the Timeline
               AttachCurrentStateInvalidated( _tNode );
               AttachCurrentGlobalSpeedInvalidated( _tNode );
               DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeliness ");

               // Create a Clock, connected to the Timeline.
               _tClock = _tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.TESTSTATE( this, _tClock, _tManager, 0, 15, 1, ref outString);

               WriteAllEvents();
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 8 )
               {
                    _tClock.Controller.Pause();
                    _tClock.Controller.Seek( TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime );
                    _tClock.Controller.Resume();
               }
          } 
     }
}
