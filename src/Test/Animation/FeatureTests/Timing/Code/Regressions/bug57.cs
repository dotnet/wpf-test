// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a child's active state when the parent has FillBehvior=FillBehvior.HoldEnd

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  The Children's CurrentState property should return ClockState.Filling when the Parent is filling.

//Pass Verification:
//  The output of this test should match the expected output in bug57Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug57Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug57 :ITimeBVT
     {
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

               TimeManagerInternal tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime         = TimeSpan.FromMilliseconds(0);
               tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(50));
               tContainer.FillBehavior      = FillBehavior.HoldEnd;
               tContainer.Name                = "Container";
               //Attach Event Handler to tContainer
               AttachCurrentStateInvalidated( tContainer );

               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime         = TimeSpan.FromMilliseconds(0);
               tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(100));
               tNode1.FillBehavior      = FillBehavior.HoldEnd;
               tNode1.Name                = "TimeNode1";
               //Attach Event Handler to tNode1
               AttachCurrentStateInvalidated( tNode1 );

               // Create child Timeline 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime         = TimeSpan.FromMilliseconds(0);
               tNode2.Duration          = new Duration(TimeSpan.FromMilliseconds(100));
               tNode2.FillBehavior      = FillBehavior.Stop;
               tNode2.Name                = "TimeNode2";
               //Attach Event Handler to tNode2
               AttachCurrentStateInvalidated( tNode2 );
               
               //Attach TimeNodes to the Container
               tContainer.Children.Add(tNode1);
               tContainer.Children.Add(tNode2);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

               // Create a Clock, connected to the Timeline.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );
                             
               //Run the Timer
               TimeGenericWrappers.TESTSTATE( this, tClock, tManager, 0, 120, 10, ref outString);

               WriteAllEvents();
               
               return outString;
          }
     }
}
