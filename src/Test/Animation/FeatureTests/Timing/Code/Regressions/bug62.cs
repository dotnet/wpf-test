// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify a Clock's CurrentTime property after a Begin()

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager; at the 5th tick, invoke Begin()

//Pass Condition:
//  The CurrentTime should return the current time.

//Pass Verification:
//  The output of this test should match the expected output in bug62Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug62Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug62 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ParallelTimeline      _tNode;
        Clock                 _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tNode.BeginTime      = null;
            _tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(7));
            _tNode.Name             = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 15, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 5 )
            {
                _tClock.Controller.Begin();
            }
        }          
        public override void OnProgress( Clock subject )
        {
            outString += "-----------------------CurrentTime: " + ((Clock)subject).CurrentTime + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
