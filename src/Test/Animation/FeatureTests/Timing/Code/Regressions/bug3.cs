// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentStateInvalidated for a child when the parent resumes

*/

//Pass Verification:
//  The output of this test should match the expected output in bug3Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug3Expect.txt file

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
    class bug3 :ITimeBVT
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

            // Create a Parent parent
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime          = TimeSpan.FromMilliseconds(1);
            parent.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            parent.FillBehavior       = FillBehavior.Stop;
            parent.Name               = "Parent";

            // Create a Parent parent
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child", " Created Child ");
            child.BeginTime          = TimeSpan.FromMilliseconds(0);
            child.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            child.FillBehavior       = FillBehavior.Stop;
            child.Name               = "Child";

            parent.Children.Add( child );
            
            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
               
            //Attach events to the child's Clock
            AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 2 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 4 )
            {
                _tClock.Controller.Resume();
            }
        } 
    }
}
