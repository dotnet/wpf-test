// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Pause and Resume methods on a Timeline container and its children
 *                                        --- Also replacing InteractiveController with ClockController
 */

//Instructions:
//  1. Create a TimeNode with Begin = 10 and Duration = 200. Attach event handler.
//  2. Create another node with Begin = 20 and Duration = 150. Attach event handler.
//  3. Create a TimeContainer with Begin = 0 and Duration 250. Attach event handler.
//  4. Attach both TimeNodes to Container
//  5. Create a TimeManager with Start=0 and Step = 1 and add the Container
//  6. Pause the TimeContainer at tick = 30.
//  7. Resume the TimeContainer at tick = 40.
//  8. Pause the TimeContainer at tick = 70.
//  9. Resume the TimeContainer at tick = 80.
//  10. Check Pause and Resume events.

//Warnings:
//  Any changes made in the output should be reflected in PauseResumeTCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class PauseResumeTC :ITimeBVT
    {
        private Timeline _tNode1,_tNode2;
        private TimelineGroup _tContainer;
        private Clock _tClock;

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

            // Create a TimeContainer
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            // Set Properties to the TimeContainer
            _tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration     = new Duration(TimeSpan.FromMilliseconds(100));
            _tContainer.Name           = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create TimeNode 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode" , " Created TimeNode " );         
            // Set Properties to the TimeNode
            _tNode1.BeginTime        = TimeSpan.FromMilliseconds(10);
            _tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(50));
            _tNode1.Name               = "TimeNode1";
            DEBUG.LOGSTATUS(" Set TimeNode 1 Properties ");         
            //Attach Handlers to TimeNode 1
            AttachCurrentGlobalSpeedInvalidated( _tNode1 );
            DEBUG.LOGSTATUS(" Attached EventHandlers to TimeNode 1");

            // Create TimeNode 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode" , " Created TimeNode " );     
            // Set Properties to the TimeNode
            _tNode2.BeginTime        = TimeSpan.FromMilliseconds(20);
            _tNode2.Duration         = new Duration(TimeSpan.FromMilliseconds(20));
            _tNode2.Name               = "TimeNode2";
            DEBUG.LOGSTATUS(" Set TimeNode 2 Properties ");
            //Attach Handlers to TimeNode 2
            AttachCurrentGlobalSpeedInvalidated( _tNode2 );
            DEBUG.LOGSTATUS(" Attached EventHandlers to TimeNode 2");

            //Attach TimeNodes to the TimeContainer
            _tContainer.Children.Add(_tNode1);
            _tContainer.Children.Add(_tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 120, 1, ref outString);
            
            return outString;   
        }

        public override void PreTick( int i )
        {
            if ( i == 10 )
                _tClock.Controller.Pause();

            if ( i == 15 )
                _tClock.Controller.Resume();
        }

        public override void PostTick( int i )
        {
            if ( i == 30 )
                _tClock.Controller.Pause();

            if ( i == 40 )
                _tClock.Controller.Resume();
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
