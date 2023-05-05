// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Seek() to the end as specified via a RepeatBehavior

*/

//Pass Verification:
//  The output of this test should match the expected output in bug102Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug102Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug102 :ITimeBVT
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
            timeline.BeginTime      = TimeSpan.FromMilliseconds(0);
            timeline.Duration       = new Duration(TimeSpan.FromMilliseconds(20));
            timeline.RepeatBehavior = new RepeatBehavior(TimeSpan.FromMilliseconds(10));
            AttachCurrentStateInvalidated( timeline );

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 21, 1, ref outString);

            return outString;
        }
        
        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                _tClock.Controller.Seek(TimeSpan.FromMilliseconds(11),TimeSeekOrigin.BeginTime);
            }
        }
    }
}
