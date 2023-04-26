// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/*
 * Description:  Verify Seeking when Duration=Automatic

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (timeline)
//  3. Create a Clock and associate it with the parent Timeline (timeline)

//Pass Condition:
//   . The test passes if CurrentState remains Filling when the Seek invoked.

//Pass Verification:
//   The output of this test should match the expected output in ICSeekWhenAutomaticExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in ICSeekWhenAutomaticExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ICSeekWhenAutomatic : ITimeBVT
    {
        ParallelTimeline    _timeline;
        ClockGroup          _tClock;
        
        TimeManagerInternal _tManager;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     

            // Create a TimeManager
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create Timeline", " Created Timeline ");
            _timeline.BeginTime          = TimeSpan.FromMilliseconds(0);
            _timeline.Duration           = Duration.Automatic;
            _timeline.Name               = "Timeline";

            // Create a Clock, connected to the Timeline Timeline.
            _tClock = _timeline.CreateClock();
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Event Handlers to tClock
            AttachCurrentStateInvalidatedTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }
          
        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime );
            }
        }

        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "---CurrentStateInvalidated -- CurrentState: " + _tClock.CurrentState + "\n";
        }
    }
}
