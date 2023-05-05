// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking the Seek method on the TimeManager using TimeSeekOrigin.BeginTime

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//  All events should fire appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug131Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug131Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug131 :ITimeBVT
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
               tNode.BeginTime          = TimeSpan.FromMilliseconds(0);
               tNode.Duration           = new Duration(TimeSpan.FromMilliseconds(100));
               tNode.Name                 = "Timeline";

               // Create a Clock, connected to the container Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
               //Attach Event Handler to tClock
               AttachCurrentStateInvalidatedTC( _tClock );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 150, 10, ref outString);
               
               return outString;
          }

          public override void PreTick( int i )
          {
               if ( i == 30 )
               {
                    _tManager.Seek(0, TimeSeekOrigin.BeginTime);  //Seek the TimeManager.
               }
          }          
     }
}
