// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify calling Remove() before a Timeline begins

*/

//Pass Verification:
//  The output of this test should match the expected output in bug27Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug27Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug27 :ITimeBVT
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
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            timeline.BeginTime          = null;
            timeline.Duration           = new Duration(TimeSpan.FromMilliseconds(4));
            timeline.Name               = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the Clock
            AttachAllHandlersTC( _tClock );

            //Intialize output String
            outString = "";
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 5, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 0 )
            {
                _tClock.Controller.Remove();
            }
        }
    }
}
