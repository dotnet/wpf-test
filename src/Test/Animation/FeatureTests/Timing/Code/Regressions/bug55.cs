// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Progress in the EndEvent when Duration is 0
*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager


//Pass Condition:
//  The Progress should return 0.

//Pass Verification:
//  The output of this test should match the expected output in bug55Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug55Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug55 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
               tNode.BeginTime      = TimeSpan.FromMilliseconds(0);
               tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(0));
               tNode.Name             = "TimeNode";
               //Attach Event Handlers to tNode
               AttachCurrentStateInvalidated( tNode );

               // Create a Clock, connected to the Timeline.
               Clock tClock = tNode.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock 1" , " Created Clock 1 " );
               
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 5, 1, ref outString);
               
               return outString;
          }
          public override void OnProgress( Clock subject )
          {
               outString += "  " + ((Clock)subject).Timeline.Name + ": Progress = " + ((Clock)subject).CurrentProgress + "\n";
          }

          public override void OnCurrentStateInvalidated(object subject, EventArgs args)
          {
               outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentStateInvalidated = " + ((Clock)subject).CurrentProgress + "\n";
          }
     }
}
