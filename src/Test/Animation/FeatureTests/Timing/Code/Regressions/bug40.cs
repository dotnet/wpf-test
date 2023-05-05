// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify Clock properties when BeginTime is given a negative value


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with a negative BeginTime
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager

//Pass Condition:
//  CurrentProgress, CurrentState, and CurrentTime should take into account the negative BeginTime.

//Pass Verification:
//  The output of this test should match the expected output in bug40Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug40Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug40 :ITimeBVT
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
            //Intialize output String
            outString = "";

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tNode.BeginTime      = TimeSpan.FromMilliseconds(-5);
            _tNode.Duration       = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.Name           = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 12, 1, ref outString);

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "-----------------CurrentProgress    = " + subject.CurrentProgress + "\n";
            outString += "-----------------CurrentState       = " + subject.CurrentState + "\n";
            outString += "-----------------CurrentGlobalSpeed = " + subject.CurrentGlobalSpeed + "\n";
            outString += "-----------------CurrentTime        = " + subject.CurrentTime + "\n";
            outString += "-----------------CurrentIteration   = " + subject.CurrentIteration + "\n";
            outString += "-----------------IsPaused           = " + subject.IsPaused + "\n";
            outString += "-----------------NaturalDuration    = " + subject.NaturalDuration + "\n";
            outString += "-----------------Parent             = " + subject.Parent + "\n";
            outString += "-------------------------------------------------------------\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
