// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify events for a child (with no Duration) beginning when a parent timeline ends


*/

//Pass Condition:
//  The Timeline should end.

//Pass Verification:
//  The output of this test should match the expected output in bug48Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug48Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug48 :ITimeBVT
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

            // Create a TimeContainer
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Duration      = new Duration(TimeSpan.FromMilliseconds(5));
            parent.Name          = "Parent";

            // Create child Timeline 1
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            child.BeginTime      = TimeSpan.FromMilliseconds(5);
            child.Name           = "Child";

            //Attach TimeNodes to the Container
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach events to the Clock
            AttachAllHandlersTC( _tClock.Children[0] );

            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 6, 1, ref outString);

            WriteAllEvents();

            return outString;
        }

        public override void OnProgress( Clock subject )
        {    
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress     = " + subject.CurrentProgress + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentState = " + subject.CurrentState    + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentTime  = " + subject.CurrentTime    + "\n";
        }
    }
}
