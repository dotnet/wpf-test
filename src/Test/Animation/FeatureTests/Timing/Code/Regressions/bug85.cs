// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify a Timeline pauses when Pause() is invoked immediately after a Begin()

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager; at the 7th tick, invoke Begin(), then a Pause()

//Pass Condition:
//  The Timeline should pause appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug85Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug85Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug85 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          Clock                 _tClock;
          
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
               tNode.BeginTime              = null;
               tNode.Duration               = new Duration(TimeSpan.FromMilliseconds(10));
               tNode.Name                     = "Timeline";
               //Attach Event Handler to tNode
               AttachCurrentGlobalSpeedInvalidated( tNode );
              
               // Create a Clock, connected to the container Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );


               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 7 )
               {
                    _tClock.Controller.Begin();
                    _tClock.Controller.Pause();
               }
          }          
     }
}
