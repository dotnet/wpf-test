// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 

/* 
 * Description: Verify Pause(), SeekAlignedToLastTick, and Resume() methods
 */

//Instructions:
//  1. Create a Timeline.
//  2. Create a Clock and connect it to the timeline.
//  3. Attach event handler to the Clock.
//  4. Create a TimeManager with Start=0 and Step = 1.
//  5. Pause, SeekAlignedToLastTick, Resume the Clock at tick = 5.

//Warnings:
//  Any changes made in the output should be reflected in ICPauseSeekAlignedResumeExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ICPauseSeekAlignedResume :ITimeBVT
    {
        Clock _clock;
        
        /*
         *  Function:  Test
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
            DEBUG.ASSERT(timeline != null, "Cannot create Timetimeline", " Created Timetimeline ");
            // Set Properties to the Timetimeline
            timeline.BeginTime    = TimeSpan.FromMilliseconds(0);
            timeline.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            timeline.Name           = "timeline";
            DEBUG.LOGSTATUS(" Set Timetimeline Properties ");

            // Create a Clock, connected to the Timeline.
            _clock = timeline.CreateClock();        
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handler to the Clock
            AttachAllHandlersTC(_clock);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _clock, tMan, 0, 12, 1, ref outString);

            WriteAllEvents();
            
            return outString;   
        }

        public override void PreTick( int i )
        {
            if ( i == 5 )
            {
                _clock.Controller.Pause();
                _clock.Controller.SeekAlignedToLastTick( TimeSpan.FromMilliseconds(8), TimeSeekOrigin.BeginTime );
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
        }
    }
}
