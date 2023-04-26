// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify invoking Begin() on a child when the container is inactive


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug53Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug53Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug53 :ITimeBVT
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
               tContainer.BeginTime     = TimeSpan.FromMilliseconds(0);
               tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(20));
               tContainer.Name            = "Container";

               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime         = TimeSpan.FromMilliseconds(0);
               tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
               tNode1.Name                = "TimeNode1";

               // Create child Timeline 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
               tNode2.BeginTime         = TimeSpan.FromMilliseconds(0);
               tNode2.Duration          = new Duration(TimeSpan.FromMilliseconds(40));
               tNode2.Name                = "TimeNode1";

               //Attach Timelines to the Container
               tContainer.Children.Add(tNode1);
               tContainer.Children.Add(tNode2);
               DEBUG.LOGSTATUS(" Attached Timelines to the Container ");

               // Create a Clock, connected to the Timeline.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );

               //Attach Handler to the Clock
               AttachCurrentStateInvalidatedTC( tClock );
               DEBUG.LOGSTATUS(" Attached EventHandler to the Clock ");
               
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 42, 1, ref outString);
               
               return outString;
          }
          
          public override void OnCurrentStateInvalidated(object subject, EventArgs args)
          {
               if (((Clock)subject).CurrentState == ClockState.Active)
               {
                   outString += "----------" +((Clock)subject).Timeline.Name + " OnCurrentStateInvalidated (Begun)"  + "\n";
               }
               else
               {
                   outString += "----------" + ((Clock)subject).Timeline.Name + " OnCurrentStateInvalidated (Ended)" + "\n";

                   try
                   {
                       ((ClockGroup)subject).Children[0].Controller.Begin();
                   }
                   catch (System.NullReferenceException)
                   {
                        //If no exception occurs, the output will be blank and the test will fail.
                        outString += "---PASS.  The expected exception occurred.\n";
                   }
               }
          }
     }
}
