// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Progress when the SpeedRatio property is set

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  All appropriate events should fire.

//Pass Verification:
//  The output of this test should match the expected output in bug59Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug59Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug59 :ITimeBVT
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
               tContainer.BeginTime         = TimeSpan.FromMilliseconds(2);
               tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
               tContainer.SpeedRatio        = 2;
               tContainer.Name                = "Container";

               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
               tNode1.BeginTime        = TimeSpan.FromMilliseconds(4);
               tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(16));
               tNode1.SpeedRatio        = 4;
               tNode1.Name                = "TimeNode1";

               //Attach Timelines to the Container
               tContainer.Children.Add(tNode1);
               DEBUG.LOGSTATUS(" Attached Timelines to the Container ");

               // Create a Clock, connected to the Timeline.
               Clock tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );
               
               //Intialize output String
               outString = "";
                             
               //Run the Timer
               TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 20, 1, ref outString);
               
               return outString;
          }
     }
}
