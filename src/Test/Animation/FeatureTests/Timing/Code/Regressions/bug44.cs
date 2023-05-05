// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify a child Timeline with a negative BeginTime

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with a negative BeginTime
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager

//Pass Condition:
//  CurrentProgress  should take into account the negative BeginTime.

//Pass Verification:
//  The output of this test should match the expected output in bug44Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug44Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug44 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClockParent,_tClockChild;

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
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime      = TimeSpan.FromMilliseconds(2);
            parent.Duration       = new Duration(TimeSpan.FromMilliseconds(5));
            parent.Name           = "Parent";

            // Create a TimeContainer
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Child", " Created Child ");
            child.BeginTime      = TimeSpan.FromMilliseconds(-6);
            child.Duration       = new Duration(TimeSpan.FromMilliseconds(10));
            child.Name           = "Child";

            // Attach the child to the Parent.
            parent.Children.Add( child );

            // Create Clocks, connected to the Timelines.
            _tClockParent = parent.CreateClock();
            DEBUG.ASSERT(_tClockParent != null, "Cannot create Parent Clock" , " Created Parent Clock " );
            _tClockChild = (ClockGroup)_tClockParent.Children[0];
            DEBUG.ASSERT(_tClockChild != null, "Cannot create Child Clock" , " Created Child Clock " );

            AttachAllHandlersTC( _tClockParent );
            AttachAllHandlersTC( _tClockChild );

            // Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClockParent, _tManager, 0, 9, 1, ref outString);

            WriteAllEvents();

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            string name = ((Clock)subject).Timeline.Name;
            
            outString += "-------CurrentProgress    [" + name + "] = " + subject.CurrentProgress + "\n";
            outString += "-------CurrentState       [" + name + "] = " + subject.CurrentState + "\n";
            outString += "-------CurrentGlobalSpeed [" + name + "] = " + subject.CurrentGlobalSpeed + "\n";
            outString += "-------CurrentTime        [" + name + "] = " + subject.CurrentTime + "\n";
            outString += "-------CurrentIteration   [" + name + "] = " + subject.CurrentIteration + "\n";
            outString += "-------IsPaused           [" + name + "] = " + subject.IsPaused + "\n";
            outString += "-------NaturalDuration    [" + name + "] = " + subject.NaturalDuration + "\n";
            outString += "-------Parent             [" + name + "] = " + subject.Parent + "\n";
            outString += "-------------------------------------------------------------\n";
        }
    }
}
