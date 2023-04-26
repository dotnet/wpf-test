// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description: Verify the ClockController.SkipToFill() method when FillBehavior = Stop
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 10 
//  2. Create a TimeManager with Start=0 and Step = 1 and add the Timeline.
//  3. At tick=4 call the SkipToFill() method

//Warnings:
//  Any changes made in the output should be reflected ICSkipToFillStopExpect.txt file

//Pass Condition:
//  Verify at tick=15 the CurrentState of the clock.

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
    class ICSkipToFillStop :ITimeBVT
    {
        ParallelTimeline    _tNode;
        Clock               _tClock;
        
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *               Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            _tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.Name              = "Timeline";
            _tNode.FillBehavior      = FillBehavior.Stop;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a click.
            AttachCurrentTimeInvalidatedTC(_tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 11 , 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }
        
        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                DEBUG.LOGSTATUS(" Calling SkipToFill() on " + _tNode.Name );
                _tClock.Controller.SkipToFill(); 
            }
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "     CurrentProgress    = " + _tClock.CurrentProgress + "\n";
            outString += "     CurrentGlobalSpeed = " + _tClock.CurrentGlobalSpeed + "\n";
            outString += "     CurrentState       = " + _tClock.CurrentState + "\n";
            outString += "     CurrentTime        = " + _tClock.CurrentTime + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
