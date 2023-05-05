// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Stop() when a Timeline reverses

*/

//Pass Verification:
//  The output of this test should match the expected output in bug119Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug119Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug119 :ITimeBVT
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
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Parent", " Created Parent ");
            timeline.BeginTime        = TimeSpan.FromMilliseconds(5);
            timeline.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            timeline.AutoReverse      = true;
            timeline.Name             = "Timeline";
            AttachCurrentGlobalSpeedInvalidated( timeline );
            
            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 16, 1, ref outString );

            return outString;
        }

        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
            outString += "---CurrentGlobalSpeedInvalidated fired --- CurrentGlobalSpeed: " + ((Clock)subject).CurrentGlobalSpeed  + "\n";
            if ( ((Clock)subject).CurrentGlobalSpeed < 0 )
            {
                _tClock.Controller.Stop();
            }
        }
    }
}
