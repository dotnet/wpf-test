// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the TimelineClock's CurrentIteration property
 */

//Instructions:
//  1. Create a Container Timeline.
//  2. Create another Timeline and attach it to the Container.
//  3. Create a TimelineClock and connect it to the Container.
//  4. Create a TimeManager with Start=0 and Step = 1.
//  5. Pause the TimelineClock at tick = 3.
//  6. Resume the TimelineClock at tick = 8.

//Warnings:
//  Any changes made in the output should be reflected in CurrentIterationExpect.txt file

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
    class CurrentIteration :ITimeBVT
    {
        private ParallelTimeline    _tNode;
        private Clock               _tClock;

        /*
         *  Function:  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create TimeNode 1
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created TimeNode " );         
            // Set Properties to the TimeNode
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            _tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            _tNode.AutoReverse       = true;
            _tNode.RepeatBehavior    = new RepeatBehavior(2);
            _tNode.Name                = "TimeNode1";
            DEBUG.LOGSTATUS(" Set TimeNode 1 Properties ");         

            // Create a TimelineClock, connected to the Timeline.
            _tClock = _tNode.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create TimelineClock" , " Created TimelineClock " );

            //Attach Handlers to the Clock
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a click.
            AttachCurrentTimeInvalidatedTC(_tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 32, 1, ref outString);
            
            outString += "FINAL CurrentIteration: " + _tClock.CurrentIteration + "\n";

            return outString;   
        }

        public override void PreTick( int i )
        {
            if ( i == 7 )
                _tClock.Controller.Pause();

            if ( i == 11 )
                _tClock.Controller.Resume();
        }
        
          
        public override void OnProgress(Clock subject)
        {
            outString += "---------CurrentProgress  = " + ((Clock)subject).CurrentProgress + "\n";
            outString += "---------CurrentIteration = " + ((Clock)subject).CurrentIteration + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
