// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 


/* 
 * Description: Verify invoking Begin() via the CurrentStateInvalidated event
 */

//Instructions:
//  1. Create a TimeContainer with BeginTime = null
//  2. Create a Timeline with BeginTime = 0 and Duration = 4
//  4. Create a Clock associcated with the Timline
//  5. Attach eventHandlers to the clock

//Warnings:
//  Any changes made in the output should be reflected in ICBeginInStateExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using ArrayList = System.Collections.ArrayList;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class ICBeginInState :ITimeBVT
    {
        private Clock         _clock;
        private int           _stateFired        = 0;
        private ArrayList     _timeEvents        = new ArrayList();
        private ArrayList     _stateEvents       = new ArrayList();
        private ArrayList     _speedEvents       = new ArrayList();
        private ArrayList     _completedEvents   = new ArrayList();

        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the Timeline
            timeline.BeginTime     = TimeSpan.FromMilliseconds(0);
            timeline.Duration      = new Duration(TimeSpan.FromMilliseconds(4));
            timeline.Name            = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the container.
            _clock = timeline.CreateClock();        
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers
            AttachAllHandlersTC( _clock );
            DEBUG.LOGSTATUS(" Attached EventHandlers");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _clock, tMan, 0, 10, 1, ref outString);

            WriteTheseEvents();            
            
            return outString;
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
            _timeEvents.Add(tickNumber);
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            _stateEvents.Add(tickNumber);
            
            if (_stateFired == 0)
            {
                outString += "-----CurrentStateInvalidated-----Invoking Begin()" + "\n";
                _clock.Controller.Begin();
            }
            _stateFired++;
        }
        
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
            _speedEvents.Add(tickNumber);
        }
        
        public override void OnCompleted(object subject, EventArgs args)
        {
            _completedEvents.Add(tickNumber);
        }
        
        public virtual void WriteTheseEvents()
        {
            if ( _timeEvents.Count > 0 )
            {
                _timeEvents.Sort();
                outString += "--- CurrentTimeInvalidated fired---" + "\n";
                foreach ( int s1 in _timeEvents )
                {
                    outString += "---At Tick #" + s1 + "\n";
                }
            }
            if ( _stateEvents.Count > 0 )
            {
                _stateEvents.Sort();
                outString += "--- CurrentStateInvalidated fired---" + "\n";
                foreach ( int s2 in _stateEvents )
                {
                    outString += "---At Tick #" + s2 + "\n";
                }
            }
            if ( _speedEvents.Count > 0 )
            {
                _speedEvents.Sort();
                outString += "--- CurrentGlobalSpeedInvalidated fired---" + "\n";
                foreach ( int s3 in _speedEvents )
                {
                    outString += "---At Tick #" + s3 + "\n";
                }
            }
            if ( _completedEvents.Count > 0 )
            {
                _completedEvents.Sort();
                outString += "--- Completed fired---" + "\n";
                foreach ( int s4 in _completedEvents )
                {
                    outString += "---At Tick #" + s4 + "\n";
                }
            }
        }
    }
}
