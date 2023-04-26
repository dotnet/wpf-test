// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentStateInvalidated when Pause/Resume are invoked during a Repeat

*/

//Pass Verification:
//  The output of this test should match the expected output in bug2Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug2Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class bug2 :ITimeBVT
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

            // Create a Parent timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Parent", " Created Parent ");
            timeline.BeginTime          = TimeSpan.FromMilliseconds(0);
            timeline.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            timeline.RepeatBehavior     = new RepeatBehavior(3);
            timeline.FillBehavior       = FillBehavior.Stop;
            timeline.Name               = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the Clock
            AttachCurrentStateInvalidatedTC( _tClock );
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 20, 1, ref outString);

            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 6 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 7 )
            {
                _tClock.Controller.Resume();
            }
            if ( i == 16 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 17 )
            {
                _tClock.Controller.Resume();
            }
        } 
    }
}
