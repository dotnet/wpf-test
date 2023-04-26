// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 

/* 
 * Description: Verify calling Pause(), Begin, then Resume() on the ClockController
 */

//Instructions:
//  1. Create a Timeline.
//  2. Create a Clock and connect it to the timeline.
//  3. Attach event handler to the Clock.
//  4. Create a TimeManager with Start=0 and Step = 1.
//  5. Pause, Begin, Resume the Clock.

//Warnings:
//  Any changes made in the output should be reflected in ICPauseBeginResumeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class ICPauseBeginResume :ITimeBVT
    {
        Clock _clock;
        
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

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            // Set Properties to the Timeline
            timeline.BeginTime    = TimeSpan.FromMilliseconds(0);
            timeline.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _clock = timeline.CreateClock();        
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handler to the Clock
            AttachAllHandlersTC(_clock);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _clock, tMan, 0, 20, 1, ref outString);

            WriteAllEvents();
            
            return outString;   
        }

        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                _clock.Controller.Pause();
            }
            if ( i == 7 )
            {
                _clock.Controller.Begin();
                _clock.Controller.Resume();
            }
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "---CurrentTime        = " + subject.CurrentTime + "\n";
            outString += "---CurrentProgress    = " + subject.CurrentProgress + "\n";
            outString += "---CurrentState       = " + subject.CurrentState + "\n";
            outString += "---CurrentGlobalSpeed = " + subject.CurrentGlobalSpeed + "\n";
            outString += "---CurrentIteration   = " + subject.CurrentIteration + "\n";
            outString += "---IsPaused           = " + subject.IsPaused + "\n";
        }
    }
}
