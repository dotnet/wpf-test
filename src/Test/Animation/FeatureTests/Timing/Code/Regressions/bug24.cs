// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify calling CreateClock() when no TimeManager exists

*/

//Pass Condition:
//  The Timeline should end.

//Pass Verification:
//  The output of this test should match the expected output in bug24Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug24Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug24 :ITimeBVT
     {
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

               ParallelTimeline timeline = new ParallelTimeline();
               DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
               timeline.Name          = "timeline";
               
               // Create a Clock, connected to the Timeline.
               _tClock = timeline.CreateClock();          
               
               outString += "Children: " + _tClock.Children.Count;
               
               return outString;
          }
     }
}
