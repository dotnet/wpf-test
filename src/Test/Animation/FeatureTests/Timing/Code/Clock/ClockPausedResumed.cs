// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test the Paused and Resumed events for a Clock
 */

//Instructions:
//  1. Create a Container Timeline.
//  2. Create another Timeline and attach it to the Container.
//  3. Create a Clock and connect it to the Container.
//  4. Attach event handlers to the Clock.
//  5. Create a TimeManager with Start=0 and Step = 1.
//  6. Pause the Clock at tick = 3.
//  7. Resume the Clock at tick = 8.

//Warnings:
//  Any changes made in the output should be reflected in ClockPausedResumedExpect.txt file

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
    class ClockPausedResumed :ITimeBVT
    {
        private ParallelTimeline _tNode1,_tContainer;
        private Clock _tClock;

        /*
         *  Function:  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            // Set Properties to the TimeContainer
            _tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration     = new Duration(TimeSpan.FromMilliseconds(20));
            _tContainer.Name           = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create TimeNode 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );         
            // Set Properties to the TimeNode
            _tNode1.BeginTime        = TimeSpan.FromMilliseconds(5);
            _tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode1.Name               = "TimeNode1";
            DEBUG.LOGSTATUS(" Set TimeNode 1 Properties ");         

            //Attach TimeNode to the TimeContainer
            _tContainer.Children.Add(_tNode1);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            AttachCurrentStateInvalidatedTC(_tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 22, 1, ref outString);
            
            return outString;   
        }

        public override void PreTick( int i )
        {
            if ( i == 3 )
                _tClock.Controller.Pause();

            if ( i == 8 )
                _tClock.Controller.Resume();
        }

        public override void OnProgress(Clock subject)
        {
            outString += "---------CurrentState = " + ((Clock)subject).CurrentState + "\n";
        }
    }
}
