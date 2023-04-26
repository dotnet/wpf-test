// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentStateInvalidated for a child when the parent specifies AccelerationRatio

*/

//Pass Verification:
//  The output of this test should match the expected output in bug7Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug7Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug7 :ITimeBVT
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
            parent.BeginTime            = TimeSpan.FromMilliseconds(0);
            parent.Duration             = new Duration(TimeSpan.FromMilliseconds(20));
            parent.AccelerationRatio    = 0.5;
            parent.Name                 = "Parent";

            // Create a Parent parent
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child", " Created Child ");
            child.BeginTime          = TimeSpan.FromMilliseconds(5);
            child.Duration           = new Duration(TimeSpan.FromMilliseconds(5));
            child.Name               = "Child";
            child.CurrentStateInvalidated   += new EventHandler(OnCurrentStateInvalidated);

            parent.Children.Add( child );
            
            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 22, 1, ref outString);

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
