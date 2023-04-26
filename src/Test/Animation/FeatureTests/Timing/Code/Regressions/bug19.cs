// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description:  Verify CurrentGlobalSpeedInvalidated when BeginTime > 0

*/

//Pass Verification:
//  The output of this test should match the expected output in bug19Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug19Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug19 :ITimeBVT
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

            //Verify events by listing them at the end.
            eventsVerify = true;

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            timeline.BeginTime            = TimeSpan.FromMilliseconds(300);
            timeline.Duration             = new Duration(TimeSpan.FromMilliseconds(500));
               
            //Attach an event to the timeline
            AttachAllHandlers( timeline );
             
            // Create a Clock, connected to the Timeline.
            _tClock = timeline.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 820, 10, ref outString);

            WriteAllEvents();

            return outString;
        }
          
        public override void OnProgress( Clock subject )
        {    
        }
    }
}
