// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify invoking a Pause after an initial Seek

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline 
//  3. Create a Clock and associate it with the Timeline
//  4. Invoke interactive methods on the Timeline

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug135Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug135Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug135 : ITimeBVT
    {
         Clock _tClock;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            timeline.BeginTime              = null;
            timeline.Duration               = new Duration(TimeSpan.FromMilliseconds(100));
            timeline.Name                     = "Timeline";

            // Create a Clocks, connected to the Timelines.
            _tClock = timeline.CreateClock();      
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Event Handlers to tClock
            AttachCurrentStateInvalidatedTC( _tClock );
            AttachCurrentGlobalSpeedInvalidatedTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 140, 10, ref outString);

            WriteAllEvents();

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 0 )
            {
                _tClock.Controller.Begin();
                _tClock.Controller.Seek(TimeSpan.FromMilliseconds(20), TimeSeekOrigin.BeginTime);
            }
            if ( i == 40 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 60 )
            {
                _tClock.Controller.Resume();
            }
        }
    }
}
