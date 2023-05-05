// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Progress when a Timeline and its child are both reversing

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug50Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug50Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug50 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               //Intialize output String
               outString = "";

               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
               tContainer.AutoReverse  = true;
               tContainer.Name           = "Container";

               // Create a child Timeline
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(40));
               tContainer.AutoReverse  = true;
               tNode1.Name               = "TimeNode1";
               //Attach Event Handlers to tNode1
               AttachCurrentGlobalSpeedInvalidated( tNode1 );

               //Attach TimeNodes to the Container
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

               // Create a Clock, connected to the Timeline.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );

               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 100, 10, ref outString);
               
               return outString;
          }
     }
}
