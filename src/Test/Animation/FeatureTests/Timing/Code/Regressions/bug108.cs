// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Seek on a Timeline before its child repeats 

*/

//Pass Verification:
//  The output of this test should match the expected output in bug108Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug108Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug108 :ITimeBVT
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
            parent.Name             = "Parent";

            // Create a TimeContainer
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child", " Created Child ");
            child.BeginTime        = TimeSpan.FromMilliseconds(0);
            child.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            child.RepeatBehavior   = new RepeatBehavior(2);
            child.Name             = "Child";
            AttachCurrentStateInvalidated( child );

            parent.Children.Add( child );
            
            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 21, 1, ref outString );

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 5 )
            {
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(8), TimeSeekOrigin.BeginTime );
            }
        } 
    }
}
