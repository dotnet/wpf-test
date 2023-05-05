// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify calling Pause() then Resume on a SyncTimeline - Grow

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree, containing a parent Timeline that is a Custom "Sync" timeline
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager


//Pass Verification:
//  The output of this test should match the expected output in SBPauseResume5Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected SBPauseResume5Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class SBPauseResume5 :ITimeBVT
    {
        private Clock        _clock;
        private SyncTimeline _syncTimeline;
        
        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            // Intialize output String
            outString = "";
            
            // Define the Slip period.
            slipBegin    = 0d;
            slipDuration = 0d;

            // Verify events by listing them at the end.
            eventsVerify = true;

            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            _syncTimeline = EstablishSyncTimeline(this);
            _syncTimeline.Name       = "SlippingTimeline";
            _syncTimeline.BeginTime  = TimeSpan.FromMilliseconds(0);
            _syncTimeline.Duration   = TimeSpan.FromMilliseconds(10);

            AttachAllHandlers( _syncTimeline );

            // Create a Clock, connected to the Timeline.
            _clock = _syncTimeline.CreateClock();          
            DEBUG.ASSERT(_clock != null, "Cannot create Clock " , " Created Clock " );
            
            // Run the Timer
            TimeGenericWrappers.EXECUTE( this, _clock, tManager, 0, 16, 1, ref outString);

            WriteAllEvents();

            return outString;
        }
        
        public override void PreTick( int i )
        {
            if ( i == 4 )
            {
                _clock.Controller.Pause();
            }
            if ( i == 7 )
            {
                _clock.Controller.Resume();
            }
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {    
        }
    }
}
