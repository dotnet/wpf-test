// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the OnCurrentTimeInvalidated on a Clock
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with begin=10 and duration=100
//  3. Attach 2 eventhandlers to the Timeline
//  4. Create a Clock, associated with the Timeline
//  5. Start the TimeManager at timeline = 0
//  6. At timeline = 50 remove 1 CurrentTimeInvalidated eventhandler from the Timeline
//  7. Check OnCurrentTimeInvalidated event fired after every tick between tick = 10 and tick = 50


//Pass Verification:
//   The output of this test should match the expected output in ChangedEventExpect.txt

//Warnings:
//  Any changes made in the output should be reflected ChangedEventExpect.txt file

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
    class ChangedEvent :ITimeBVT
    {
        ParallelTimeline    _tNode;
        Clock       _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create Timeline" , " Created Timeline" );
            _tNode.BeginTime     = TimeSpan.FromMilliseconds(10);
            _tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(100));
            _tNode.Name            = "TimeNode";          

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
                      
            //Attach Event handlers to the Clock
            _tClock.CurrentTimeInvalidated += new EventHandler(OnCurrentTimeInvalidated);
            _tClock.CurrentTimeInvalidated += new EventHandler(TestCurrentTimeInvalidated);

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 120 , 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 50 )
            {
                _tClock.CurrentTimeInvalidated -= new EventHandler(OnCurrentTimeInvalidated);
            }
        }

        public void TestCurrentTimeInvalidated( object subject, EventArgs args )
        {
                outString += "  ********** Second Handler : " + _tNode.Name + " Progress: " + ((Clock)subject).CurrentProgress + "\n";
        }

        public override void OnProgress( Clock subject )
        {
        }
    }
}
