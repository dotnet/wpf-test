// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the CurrentState property for an inactive TimelineClock
 */

//Instructions:
//    1. Create a Timeline
//    2. Create a TimelineClock, associated with the Timeline.
//    3. Read the TimelineClock's CurrentState property

//Pass Condition:
//   This test passes if CurrentState returns the correct enumeration

//Pass Verification:
//   The output of this test should match the expected output in CurrentStateExpect.txt.

//Warnings:
//     Any changes made in the output should be reflected CurrentStateExpect.txt file

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
     class CurrentState :ITimeBVT
     {
          Clock _tClock;
          
          /*
           *  Function:    Test
           *  Arguments:   Framework
           */
          public override string Test()
          {
               // Intialize output String
               outString = "";

               // Create a TimeManager
               TimeManagerInternal tMan = EstablishTimeManager(this);
               DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");

               // Create a TimelineClock, connected to the Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create TimelineClock" , " Created TimelineClock " );
               
               outString = "---CurrentState:         " + _tClock.CurrentState + "\n";
               outString += "---CurrentState.Active:  " + (_tClock.CurrentState == ClockState.Active) + "\n";
               outString += "---CurrentState.Filling: " + (_tClock.CurrentState == ClockState.Filling) + "\n";
               outString += "---CurrentState.Stopped: " + (_tClock.CurrentState == ClockState.Stopped);

               return outString;
          }
     }
}
