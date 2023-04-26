// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  


//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline
//  3. Create a Clock, associated with the parent Timeline; attach event handlers
//  4. Start the TimeManager; invoke Timing methods at various intervals


//Pass Condition:
//  This test passes if the CurrentTime is correct after each Timing method is invoked.

//Pass Verification:
//  The output of this test should match the expected output in bug99Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug99Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug99 :ITimeBVT
    {
        Clock _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a simple TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            tNode.BeginTime          = TimeSpan.FromMilliseconds(50);
            tNode.Duration           = new Duration(TimeSpan.FromMilliseconds(35));
            tNode.RepeatBehavior     = new RepeatBehavior(3.837653f);
            tNode.Name               = "Timeline";

            // Create a Clock, connected to the container Timeline.
            _tClock = tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Event Handler to the Clock
            AttachCurrentStateInvalidatedTC( _tClock );
            AttachCurrentGlobalSpeedInvalidatedTC( _tClock );
            AttachRepeatTC( _tClock );
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clock to force a Tick.
            AttachCurrentTimeInvalidatedTC(_tClock);

            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 235, 1, ref outString);

            WriteAllEvents();
               
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 116 )
            {
                _tClock.Controller.Resume();
            }

            if ( i == 208 )
            {
                _tClock.Controller.Pause();
            }
        }

        public override void PostTick( int i )
        {
            if ( i == 101 )
            {
                _tClock.Controller.Pause();
            }

            if ( i == 229 )
            {
                _tClock.Controller.Resume();
            }
        }
        
        public override void OnProgress( Clock subject )
        {    
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
