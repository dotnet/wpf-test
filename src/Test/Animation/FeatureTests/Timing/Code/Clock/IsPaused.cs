// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Timeline's IsPaused Property
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 and Duration = 500
//  2. Create a TimeManager with Start=0 and Step = 100 and add the TimeNode
//  3. Pause the Timeline, then resume it
//  4. Check the IsPaused property after each Tick

//Warnings:
//  Any changes made in the output should be reflected in IsPausedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class IsPaused :ITimeBVT
    {
        ParallelTimeline        _timeline;
        Clock           _tClock;
        
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            _timeline = new ParallelTimeline();
            DEBUG.ASSERT(_timeline != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _timeline.BeginTime    = TimeSpan.FromMilliseconds(0);
            _timeline.Duration     = new Duration(TimeSpan.FromMilliseconds(12));
            _timeline.Name           = "Timeline";            
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = _timeline.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";     
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 12, 1, ref outString);
            
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 5 )
                _tClock.Controller.Pause();

            if ( i == 10 )
                _tClock.Controller.Resume();
        }

        public override void OnProgress( Clock subject )
        {
            outString += "------------IsPaused: " + subject.IsPaused + "\n";
        }
    }
}
