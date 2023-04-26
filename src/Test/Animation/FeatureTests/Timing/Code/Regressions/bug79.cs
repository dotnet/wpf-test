// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify a Timeline child's CurrentProgress when its Parent is paused


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree, with associated event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 10th tick, invoke Pause() on the container

//Pass Condition:
//  The child's Progress should return a non-zero value.

//Pass Verification:
//  The output of this test should match the expected output in bug79Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug79Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug79 :ITimeBVT
     {
          Clock _tClock;
          
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
               tContainer.BeginTime         = TimeSpan.FromMilliseconds(0);
               tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(1000));
               tContainer.Name              = "PARENT";
               //Attach Event Handlers to the Container
               AttachCurrentStateInvalidated( tContainer );

               // Create a child Timeline
               ParallelTimeline tNode = new ParallelTimeline();
               DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode" );
               tNode.BeginTime          = TimeSpan.FromMilliseconds(100);
               tNode.Duration           = new Duration(TimeSpan.FromMilliseconds(500));
               tNode.Name               = "CHILD";
               //Attach Event Handlers to tNode
               AttachCurrentStateInvalidated( tNode );

               //Attach the child Timeline to the Container
               tContainer.Children.Add(tNode);
               DEBUG.LOGSTATUS(" Attached Timelines to the Container ");

               // Create a Clock, connected to the Timeline.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 500, 100, ref outString);
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 300 )
               {
                    _tClock.Controller.Pause();
               }
          }
     }
}
