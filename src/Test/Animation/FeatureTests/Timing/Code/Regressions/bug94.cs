// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify seeking during the repeat time of a parent timeline

*/

//Pass Verification:
//  The output of this test should match the expected output in bug94Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug94Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug94 :ITimeBVT
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

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime        = TimeSpan.FromMilliseconds(0);
            parent.RepeatBehavior   = new RepeatBehavior(2);
            parent.Name             = "Parent";

            // Create child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            child.BeginTime         = TimeSpan.FromMilliseconds(0);
            child.Duration          = new Duration(TimeSpan.FromMilliseconds(4));
            child.Name              = "Child";
            child.RepeatBehavior    = new RepeatBehavior(TimeSpan.FromMilliseconds(8));

            //Attach TimeNodes to the Container
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 10, 1, ref outString);

            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 7 )
            {
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(16), TimeSeekOrigin.BeginTime );
            }
        }
        public override void OnProgress( Clock subject )
        {
            outString += "------" + ((ClockGroup)subject).Timeline.Name + ": CurrentProgress  = " + ((ClockGroup)subject).CurrentProgress + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
