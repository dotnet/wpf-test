// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Begin() before starting the TimeManager

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers; set Begin = TimeFill.Indefinite
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 5th tick, invoke Begin()

//Pass Condition:
//  

//Pass Verification:
//  The output of this test should match the expected output in bug74Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug74Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug74 :ITimeBVT
     {
          TimeManagerInternal       _tManager;
          ParallelTimeline          _tNode;
          Clock                     _tClock;
          
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
               _tNode.Duration = new Duration(TimeSpan.FromMilliseconds(50));
               _tNode.Name       = "Timeline";
               
               //Attach Handler to the Timeline
               AttachCurrentStateInvalidated( _tNode );
               DEBUG.LOGSTATUS(" Attached EventHandler to the Timeliness ");

               // Create a Clock, connected to the Timeline.
               _tClock = _tNode.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               _tClock.Controller.Begin();

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 100, 10, ref outString);
               
               return outString;
          }
     }
}
