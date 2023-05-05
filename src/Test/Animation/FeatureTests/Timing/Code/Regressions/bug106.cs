// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify event firing when Duration is set to Automatic

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager; at the 4th tick, invoke EndIn(0)

//Pass Condition:
//  All events should fire appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug106Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug106Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug106 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
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

               // Create a container Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
               tNode.BeginTime              = TimeSpan.FromMilliseconds(500);
               tNode.Duration               = Duration.Automatic;
               tNode.Name                     = "Timeline";

               //Attach Event Handler to tNode
               AttachAllHandlers( tNode );
              
               // Create a Clock, connected to the container Timeline.
               _tClock = tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 1000, 100, ref outString);

               WriteAllEvents();
               
               return outString;
          }
     }
}
