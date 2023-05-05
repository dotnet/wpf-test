// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify adding a Timeline to a Container having no Duration
*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events, for both Timelines and Clocks, should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug38Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug38Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug38 :ITimeBVT
     {
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
               tContainer.Name           = "Container";
               //Attach Event Handler to tContainer
               AttachCurrentStateInvalidated( tContainer );

               // Create a child Timeline
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );
               tNode1.BeginTime    = TimeSpan.FromMilliseconds(0);
               tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(50));
               tNode1.Name           = "TimeNode1";
               //Attach Event Handler to tNode1
               AttachCurrentStateInvalidated( tNode1 );
               
               //Attach TimeNodes to the Container
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

               // Create a Clock, connected to the Timeline.
               ClockGroup tClock1 = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock1 != null, "Cannot create Clock 1" , " Created Clock 1 " );
               
               Clock tClock2 = tClock1.Children[0];
               
               //Attach Event Handlers to the Clocks
               AttachCurrentStateInvalidatedTC( tClock1 );
               AttachCurrentStateInvalidatedTC( tClock2 );
               DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeliness ");
               
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock1, tManager, 0, 100, 10, ref outString);
               
               return outString;
          }
          public override void OnCurrentStateInvalidated(object subject, EventArgs args)
          {
               outString += "----------CurrentStateInvalidated fired "  + "\n";               
          }
     }
}
