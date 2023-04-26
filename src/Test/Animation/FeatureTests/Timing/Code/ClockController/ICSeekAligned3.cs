// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the SeekAlignedToLastTick going forward
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with begin=0 and duration=20 and attach to Manager
//  3. Start the TimeManager at timeline = 0
//  4. SeekAlignedToLastTick the Timeline with offset = 7
//  5. Check the CurrentProgress and CurrentState  properties after each Tick
//  6. Check event firing

//Pass Verification:
//   The output of this test should match the expected output in ICSeekAligned3Expect.txt

//Warnings:
//  Any changes made in the output should be reflected ICSeekAligned3Expect.txt file

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
    class ICSeekAligned3 :ITimeBVT
    {
        ParallelTimeline    _tNode;
        Clock               _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create Timeline" , " Created Timeline " );
            _tNode.BeginTime   = TimeSpan.FromMilliseconds(0);
            _tNode.Duration    = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.Name        = "TimeNode";           
            
            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentTimeInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, _tClock, tManager, 0, 11, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 2 )
            {
                _tClock.Controller.SeekAlignedToLastTick( TimeSpan.FromMilliseconds(7), TimeSeekOrigin.BeginTime );
            }
        }
    }
}
