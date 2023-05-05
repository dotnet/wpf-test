// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 

/* 
 * Description: Verify the Completed event fires appropriate with various methods
 */

//Instructions:
//  1. Create a Timeline.
//  2. Create a Clock and connect it to the timeline.
//  3. Attach event handler to the Clock.
//  4. Create a TimeManager with Start=0 and Step = 1.
//  5. Apply methods to the Clock.

//Warnings:
//  Any changes made in the output should be reflected in ICCompleteExpect.txt file

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
    class ICComplete :ITimeBVT
    {
        Clock _clock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline", " Created Timeline ");
            // Set Properties to the Timeline
            timeline.BeginTime    = null;
            timeline.Duration     = new Duration(TimeSpan.FromMilliseconds(50));
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Handler to the Timeline
            AttachCompleted(timeline);
            DEBUG.LOGSTATUS(" Attached EventHandler to the Timeline ");

            // Create a Clock, connected to the Timeline.
            _clock = timeline.CreateClock();        
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _clock, tMan, 0, 21, 1, ref outString);
            
            return outString;   
        }

        public override void PreTick( int i )
        {
            if ( i == 2 )
            {
                _clock.Controller.Begin();
            }
            
            if ( i == 4 )
            {
                _clock.Controller.Pause();
            }

            
            if ( i == 6 )
            {
                _clock.Controller.Resume();
            }

            if ( i == 8 )
            {
                _clock.Controller.Seek( TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime );
            }

            if ( i == 10 )
            {
                _clock.Controller.SkipToFill();
            }

            if ( i == 12 )
            {
                _clock.Controller.Stop();
            }

            if ( i == 14 )
            {
                _clock.Controller.Begin();
            }

            if ( i == 16 )
            {
                _clock.Controller.Stop();
            }

            if ( i == 18 )
            {
                _clock.Controller.Begin();
            }

            if ( i == 20 )
            {
                _clock.Controller.Remove();
            }
        }
        
    }
}
