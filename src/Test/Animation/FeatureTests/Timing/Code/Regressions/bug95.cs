// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify Seeking a Timeline past the end of the Timeline


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 11th tick, Seek

//Pass Condition:
//  The Timeline should end.

//Pass Verification:
//  The output of this test should match the expected output in bug95Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug95Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug95 :ITimeBVT
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

               // Create a TimeContainer
               ParallelTimeline tContainer = new ParallelTimeline();
               DEBUG.ASSERT(tContainer != null, "Cannot create Parent", " Created Parent ");
               tContainer.BeginTime     = TimeSpan.FromMilliseconds(0);
               tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
               tContainer.FillBehavior  = FillBehavior.HoldEnd;
               tContainer.Name          = "Parent";
               
               // Create child Timeline 1
               ParallelTimeline tNode1 = new ParallelTimeline();
               DEBUG.ASSERT(tNode1 != null, "Cannot create Child1" , " Created Child1" );
               tNode1.BeginTime     = TimeSpan.FromMilliseconds(0);
               tNode1.Duration      = new Duration(TimeSpan.FromMilliseconds(4));
               tNode1.FillBehavior  = FillBehavior.HoldEnd;
               tNode1.Name          = "Child1";
               
               // Create child Timeline 2
               ParallelTimeline tNode2 = new ParallelTimeline();
               DEBUG.ASSERT(tNode2 != null, "Cannot create Child2" , " Created Child2" );
               tNode2.BeginTime     = TimeSpan.FromMilliseconds(2);
               tNode2.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
               tNode2.FillBehavior  = FillBehavior.HoldEnd;
               tNode2.Name          = "Child2";
              
               //Attach TimeNodes to the Container
               tContainer.Children.Add(tNode1);
               tContainer.Children.Add(tNode2);
               DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");
               
               // Create a Clock, connected to the Timeline.
               _tClock = tContainer.CreateClock();          
               DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
               //Attach events to the Clock
               AttachCurrentStateInvalidatedTC( _tClock );

               //Intialize output String
               outString = "";
               
               //Run the Timer               
               TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 10, 1, ref outString);
               
               return outString;
          }

          public override void PostTick( int i )
          {
               if ( i == 7 )
               {
                   _tClock.Controller.Seek( TimeSpan.FromMilliseconds(11), TimeSeekOrigin.BeginTime );
               }
          } 
          public override void OnProgress( Clock subject )
          {
               outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentProgress      = " + ((Clock)subject).CurrentProgress + "\n";
               outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentTime          = " + ((Clock)subject).CurrentTime + "\n";
               outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentState         = " + ((Clock)subject).CurrentState + "\n";
          }
     }
}
