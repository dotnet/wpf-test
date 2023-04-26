// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify assigning an negative BeginTime to a Timeline with a RepeatDuration TimeSpan

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with a negative BeginTime
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager

//Pass Condition:
//  CurrentProgress, CurrentState, and CurrentTime should take into account the negative BeginTime.

//Pass Verification:
//  The output of this test should match the expected output in bug46Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug46Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug46 :ITimeBVT
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
            // Intialize output String
            outString = "";

            // Verify events by listing them at the end.
            eventsVerify = true;

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(-2);
            _tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(4));
            _tNode.Name              = "Timeline";
            _tNode.RepeatBehavior    = new RepeatBehavior(TimeSpan.FromMilliseconds(10));

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            // Attach Event Handlers to parent and child.
            AttachAllHandlersTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 12, 1, ref outString);

            WriteAllEvents();

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "-----------------CurrentProgress    = " + subject.CurrentProgress + "\n";
            outString += "-----------------CurrentState       = " + subject.CurrentState + "\n";
            outString += "-----------------CurrentGlobalSpeed = " + subject.CurrentGlobalSpeed + "\n";
            outString += "-----------------CurrentTime        = " + subject.CurrentTime + "\n";
            outString += "-----------------CurrentIteration   = " + subject.CurrentIteration + "\n";
            outString += "-----------------Duration           = " + subject.Timeline.RepeatBehavior.Duration + "\n";
            outString += "-------------------------------------------------------------\n";
        }
    }
}
