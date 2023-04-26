// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Pause() and Resume() via CurrentStateInvalidated fired by a Seek()

*/

//Pass Verification:
//  The output of this test should match the expected output in bug116Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug116Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug116 :ITimeBVT
    {
        TimeManagerInternal _tManager;
        ClockGroup          _tClock;
        bool                _seeked = false;

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
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Parent", " Created Parent ");
            timeline.BeginTime      = TimeSpan.FromMilliseconds(0);
            timeline.Duration       = new Duration(TimeSpan.FromMilliseconds(10));

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the Clock
            AttachCurrentStateInvalidatedTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 12, 1, ref outString );

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 4 )
            {
                _seeked = true;
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(8), TimeSeekOrigin.BeginTime );
            }
        }

        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "----------CurrentStateInvalidated fired --- CurrentState: " + ((Clock)subject).CurrentState  + "\n";
            if (_seeked)
            {
                _tClock.Controller.Pause();
                _tClock.Controller.Resume();
            }
        }
    }
}
