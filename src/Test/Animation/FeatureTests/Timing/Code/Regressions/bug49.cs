// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify events for a child with no Duration and a parent with no Duration


*/

//Pass Condition:
//  The Timeline should end.

//Pass Verification:
//  The output of this test should match the expected output in bug49Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug49Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug49 :ITimeBVT
     {
          TimeManagerInternal   _tManager;
          ClockGroup            _tClock;
          
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

               // Create a TimeContainer
               ParallelTimeline parent = new ParallelTimeline();
               DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
               parent.Name          = "Parent";
               
               // Create child Timeline 1
               ParallelTimeline child1 = new ParallelTimeline();
               DEBUG.ASSERT(child1 != null, "Cannot create Child1" , " Created Child1" );
               child1.BeginTime      = TimeSpan.FromMilliseconds(1);
               child1.Duration      = new Duration(TimeSpan.FromMilliseconds(5));
               child1.Name           = "Child1";
               
               // Create child Timeline 2
               ParallelTimeline child2 = new ParallelTimeline();
               DEBUG.ASSERT(child2 != null, "Cannot create Child2" , " Created Child2" );
               child2.BeginTime      = TimeSpan.FromMilliseconds(10);
               child2.Name           = "Child2";

               //Attach TimeNodes to the Container
               parent.Children.Add(child1);
               parent.Children.Add(child2);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");
               
               // Create a Clock, connected to the Timeline.
               _tClock = parent.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
               //Attach events to the Clock
               AttachCurrentStateInvalidatedTC( _tClock );
               AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
               AttachCurrentStateInvalidatedTC( _tClock.Children[1] );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 12, 1, ref outString);

               WriteAllEvents();
               
               return outString;
          }
     }
}
