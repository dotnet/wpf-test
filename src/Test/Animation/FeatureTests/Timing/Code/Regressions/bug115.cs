// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify Clock properties with a Begin value of 0

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager

//Pass Condition:
//  The Timeline properties should return appropriate values.

//Pass Verification:
//  The output of this test should match the expected output in bug115Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug115Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug115 :ITimeBVT
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

               // Create a TimeContainer
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
               tNode.BeginTime      = TimeSpan.FromMilliseconds(0);
               tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(2));
               tNode.Name             = "TimeContainer";
               
               // Create a Clock, connected to the Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
               //Attach events to the Clock
               AttachCurrentStateInvalidatedTC( _tClock );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 2, 1, ref outString);
               
               return outString;
          }

          public override void OnProgress( Clock subject )
          {
               outString += "---------------" + ((Clock)subject).Timeline.Name + ": TimeNode CurrentProgress       = " + ((Clock)subject).CurrentProgress + "\n";
               outString += "---------------" + ((Clock)subject).Timeline.Name + ": TimeNode CurrentTime           = " + ((Clock)subject).CurrentTime + "\n";
               outString += "---------------" + ((Clock)subject).Timeline.Name + ": TimeNode CurrentState          = " + ((Clock)subject).CurrentState + "\n";
          }
     }
}
