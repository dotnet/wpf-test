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
//  4. Start the TimeManager; at the 5th tick, invoke Pause(), then Seek(), then Resume()

//Pass Condition:
//  The events should fire appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug63Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug63Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug63 :ITimeBVT
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
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a TimeContainer
               _tNode = new ParallelTimeline();
               DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
               _tNode.BeginTime      = TimeSpan.FromMilliseconds(0);
               _tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(10));
               _tNode.Name             = "Timeline";

               // Create a Clock, connected to the Timeline.
               _tClock = _tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Attach Event Handler to tClock
               AttachCurrentGlobalSpeedInvalidatedTC( _tClock );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 15, 1, ref outString);
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 5 )
               {
                    _tClock.Controller.Pause();
                    _tClock.Controller.Seek( TimeSpan.FromMilliseconds(8), TimeSeekOrigin.BeginTime );
                    _tClock.Controller.Resume();
               }
          } 
     }
}
