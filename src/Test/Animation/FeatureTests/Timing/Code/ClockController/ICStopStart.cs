// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description: Verify that a Clock can be started again after being stopped
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with begin=0 and duration=20 and attach to Manager
//  3. Start the TimeManager at timeline = 0
//  4. Stop the Clock
//  5. Begin the Clock
//
//Pass Verification:
//   The output of this test should match the expected output in ICStopStartExpect.txt

//Warnings:
//  Any changes made in the output should be reflected ICStopStartExpect.txt file

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
    class ICStopStart :ITimeBVT
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
            _tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            _tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.FillBehavior  = FillBehavior.Stop;
            _tNode.Name          = "Timeline";           
            
            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentStateInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, _tClock, tManager, 0, 20, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                _tClock.Controller.Stop();
            }
            if ( i == 8 )
            {
                _tClock.Controller.Begin();
            }
        }
    }
}
