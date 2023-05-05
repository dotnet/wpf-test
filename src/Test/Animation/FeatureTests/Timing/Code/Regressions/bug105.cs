// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentTime when Seeking from a Paused state


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//  The Timeline Clock's CurrentTime should change appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug105Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug105Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug105 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        Clock                 _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.Name              = "Timeline";

            // Create a Clock, connected to the container Timeline.
            _tClock = tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 10, 1, ref outString);

            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 2 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 5 )
            {
                _tClock.Controller.Seek(TimeSpan.FromMilliseconds(7),TimeSeekOrigin.BeginTime);
            }
        }
        
        public override void OnProgress(Clock subject)
        {
            outString += "CurrentTime: " + _tClock.CurrentTime.ToString() + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
