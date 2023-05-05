// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: "Timing: events are fired synchronously"

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree with attached event handlers
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager; at the 10th tick, begin the Clock, then immediately
//     stop the TimeManager.


//Pass Condition:
//  Events should not fire after the TimeManager is stopped.

//Pass Verification:
//  The output of this test should match the expected output in bug36Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug36Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug36 :ITimeBVT
    {
        TimeManagerInternal     _tManager;
        ParallelTimeline        _tContainer;
        Clock                   _tClock;
        
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
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tContainer.BeginTime    = null;
            _tContainer.Duration     = new Duration(TimeSpan.FromMilliseconds(80));
            _tContainer.Name           = "Container";

            // Create a TimeNode with AutoReverse = true
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            tNode1.AutoReverse      = true ;            
            tNode1.Name               = "TimeNode1";
            
            // Create a TimeNode with RepeatCount=2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode2.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode2.Duration         = new Duration(TimeSpan.FromMilliseconds(30));
            tNode2.RepeatBehavior   = new RepeatBehavior(2);            
            tNode2.Name               = "TimeNode2";
            
            //Attach Handlers to the Timelines
            AttachCurrentStateInvalidated( _tContainer );
            AttachCurrentStateInvalidated( tNode1 );
            AttachCurrentStateInvalidated( tNode2 );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeliness ");

            //Attach TimeNodes to the Container
            _tContainer.Children.Add(tNode1);
            _tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 20, 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 10 )
            {
                _tClock.Controller.Begin();
                outString += "----------------Stopping the TimeManager now\n";
                _tManager.Stop();                
            }
        }       
          public override void OnProgress( Clock subject )
          {
          }
    }
}
