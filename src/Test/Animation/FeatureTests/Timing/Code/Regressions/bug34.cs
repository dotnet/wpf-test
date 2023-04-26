// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify changing Timeline properties via the CurrentStateInvalidated event

*/

//Pass Verification:
//  The output of this test should match the expected output in bug34Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug34Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug34 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _clock;
        ParallelTimeline      _timeline;
        int                   _fillings = 0;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a timeline
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create Timeline", " Created Timeline ");
            _timeline.BeginTime          = TimeSpan.FromMilliseconds(0);
            _timeline.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            _timeline.Name               = "Timeline";

            // Create a Clock, connected to the Timeline.
            _clock = _timeline.CreateClock();
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the Clock
            AttachCurrentStateInvalidatedTC( _clock );

            //Intialize output String
            outString = "";
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _clock, _tManager, 0, 41, 1, ref outString);

            return outString;
        }

        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "----------CurrentStateInvalidated fired --- CurrentState: " + ((Clock)subject).CurrentState  + "\n";
            if ( ((Clock)subject).CurrentState == ClockState.Filling && _fillings == 0)
            {
                _fillings++;
                _clock.Controller.SkipToFill();
                _timeline.Duration       = new Duration(TimeSpan.FromMilliseconds(5));
                _timeline.Name           = "NEWNAME";
                _timeline.AutoReverse    = true;
                _timeline.RepeatBehavior = new RepeatBehavior(3d);
                _clock.Controller.Begin();
            }
        }
    }
}
