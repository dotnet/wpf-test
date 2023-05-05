// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify using Acceleration on a Timeline container 

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//  Progress should correctly reflect the Acceleration.

//Pass Verification:
//  The output of this test should match the expected output in bug72Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug72Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug72 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          Clock                 _tClock;
          
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
          public override string Test()
          {
               _tManager = EstablishTimeManager(this);
               DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

               // Create a container Timeline
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
               tContainer.BeginTime         = TimeSpan.FromMilliseconds(0);
               tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(50));
               tContainer.AccelerationRatio = 0.4f;
               tContainer.Name                = "Container";

               // Create a child Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode" );
               tNode.BeginTime              = TimeSpan.FromMilliseconds(10);
               tNode.Duration               = new Duration(TimeSpan.FromMilliseconds(30));
               tNode.Name                     = "TimeNode";
              
               //Attach TimeNode to the Container
               tContainer.Children.Add(tNode);
               DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

               // Create a Clock, connected to the container Timeline.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );


               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 51, 1, ref outString);
               
               return outString;
          }
     }
}
