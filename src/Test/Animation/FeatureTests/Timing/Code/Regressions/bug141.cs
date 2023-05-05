// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentProgress when Seeking to the end of a Duration


*/

//Pass Verification:
//  The output of this test should match the expected output in bug141Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug141Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug141 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Parent", " Created Parent ");
            timeline.BeginTime          = null;
            timeline.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            timeline.Name               = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the Clock
            AttachCurrentStateInvalidatedTC( _tClock );

            //Intialize output String
            outString = "";
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            outString += "----------CurrentProgress: " + _tClock.CurrentProgress + "\n";
            outString += "----------CurrentState   : " + _tClock.CurrentState + "\n";
            
            if ( i == 5 )
            {
                _tClock.Controller.Begin();
                _tClock.Controller.Pause();
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(5), TimeSeekOrigin.BeginTime );
            }
        } 
    }
}
