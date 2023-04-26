// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify event firing during a parent timeline's reversal period given a fractional repeat count

*/

//Pass Verification:
//  The output of this test should match the expected output in bug86Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug86Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug86 :ITimeBVT
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
            parent.AutoReverse      = true;
            parent.RepeatBehavior   = new RepeatBehavior(1.9);
            parent.Name             = "Parent";
            AttachCurrentStateInvalidated( parent );

            // Create child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            child.BeginTime         = TimeSpan.FromMilliseconds(2);
            child.Name              = "Child";
            //AttachCurrentStateInvalidated( child );

            // Create grandchild Timeline
            ParallelTimeline grandchild = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            grandchild.BeginTime        = TimeSpan.FromMilliseconds(1);
            grandchild.Duration         = new Duration(TimeSpan.FromMilliseconds(5));
            grandchild.AutoReverse      = true;
            grandchild.Name             = "Grandchild";
            AttachCurrentStateInvalidated( grandchild );

            //Attach TimeNodes to the Container
            child.Children.Add(grandchild);
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 52, 1, ref outString);

            return outString;
        }
          
        public override void OnProgress( Clock subject )
        {
        }
    }
}
