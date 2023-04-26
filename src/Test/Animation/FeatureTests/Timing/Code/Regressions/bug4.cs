// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a TimelineClock starts when Duration is Forever
 */

//Instructions:
//    1. Create a Timeline
//    2. Create a Clock, associated with the Timeline.
//    3. Read the Clock's CurrentState property

//Pass Condition:
//   This test passes if TimlineClock properties are correct.

//Pass Verification:
//   The output of this test should match the expected output in bug4Expect.txt.

//Warnings:
//     Any changes made in the output should be reflected bug4Expect.txt file

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
    class bug4 :ITimeBVT
    {
        Clock _tClock;

        /*
        *  Function:    Test
        *  Arguments:   Framework
        */
        public override string Test()
        {
            // Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            tNode.Duration = Duration.Forever;
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");

            // Create a Clock, connected to the Timeline.
            _tClock = tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            outString += "---CurrentState:         " + _tClock.CurrentState + "\n";
            outString += "---CurrentGlobalSpeed:   " + _tClock.CurrentGlobalSpeed + "\n";
            outString += "---CurrentTime:          " + _tClock.CurrentTime + "\n";

            tMan.Start();

            outString += "---CurrentState:         " + _tClock.CurrentState + "\n";
            outString += "---CurrentGlobalSpeed:   " + _tClock.CurrentGlobalSpeed + "\n";
            outString += "---CurrentTime:          " + _tClock.CurrentTime + "\n";

            _tClock.Controller.Begin();

            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 3, 1, ref outString );

            return outString;
        }

        public override void OnProgress(Clock subject)
        {
            outString += "---CurrentState:         " + _tClock.CurrentState + "\n";
            outString += "---CurrentGlobalSpeed:   " + _tClock.CurrentGlobalSpeed + "\n";
            outString += "---CurrentTime:          " + _tClock.CurrentTime + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
