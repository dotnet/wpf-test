// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify event firing of a Child during a Parent's reveral

*/

//Pass Verification:
//  The output of this test should match the expected output in bug78Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug78Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug78 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime            = TimeSpan.FromMilliseconds(12);
            parent.AutoReverse          = true;
            parent.DecelerationRatio    = 0.001843564;
            parent.Name                 = "Parent";

            // Create Child 1
            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create Child1" , " Created Child1" );
            child1.BeginTime     = TimeSpan.FromMilliseconds(4);
            child1.Duration             = new Duration(TimeSpan.FromMilliseconds(13));
            child1.AutoReverse          = true;
            child1.RepeatBehavior       = new RepeatBehavior(0.1773438);
            child1.AccelerationRatio    = 0.007962223;
            child1.DecelerationRatio    = 0.003846226;
            child1.Name                 = "Child1";

            // Create Child 2
            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create Child2" , " Created Child2" );
            child2.BeginTime            = TimeSpan.FromMilliseconds(4);
            child2.Duration             = new Duration(TimeSpan.FromMilliseconds(13));
            child2.AutoReverse          = true;
            child2.AccelerationRatio    = 0.002948401f;
            child2.Name                 = "Child2";

            //Attach TimeNodes to the Container
            parent.Children.Add(child1);
            parent.Children.Add(child2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach events to the child's Clock
            AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
            AttachCurrentGlobalSpeedInvalidatedTC( _tClock.Children[0] );

            //Intialize output String
            outString = "";
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 80, 1, ref outString);

            return outString;
        }
    }
}
