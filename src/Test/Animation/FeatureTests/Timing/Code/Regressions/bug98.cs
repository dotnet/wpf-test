// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Begin() on a parent in a Paused state

*/

//Pass Verification:
//  The output of this test should match the expected output in bug98Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug98Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug98 :ITimeBVT
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

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime        = TimeSpan.FromMilliseconds(2);
            parent.Duration         = new Duration(TimeSpan.FromMilliseconds(25));
            parent.Name             = "Parent";
            AttachCurrentStateInvalidated( parent );

            // Create child Timeline
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child1" , " Created Child1" );
            child1.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            child1.Name              = "Child1";
            AttachCurrentStateInvalidated( child1 );

            // Create child Timeline
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child1" , " Created Child1" );
            child2.BeginTime         = TimeSpan.FromMilliseconds(21);
            child2.Duration          = new Duration(TimeSpan.FromMilliseconds(2));
            child2.Name              = "Child2";
            AttachCurrentStateInvalidated( child2 );

            //Attach TimeNodes to the Container
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 51, 1, ref outString);

            return outString;
        }
        
        public override void PreTick( int i )
        {
            if ( i == 22 )
            {
                _tClock.Controller.Resume();
                _tClock.Controller.Begin();
            }
        }

        public override void PostTick( int i )
        {
            if ( i == 18 )
            {
                _tClock.Controller.Pause();
            }
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
