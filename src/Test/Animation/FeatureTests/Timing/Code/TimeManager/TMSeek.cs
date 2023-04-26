// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Seek method of TimeManager
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeNode with begin=0 and duration=20 and attach to Manager
//  3. Start the TimeManager at timeline = 0
//  4. Seek the TimeManager with offset = 3 and origin = TimeSeekOrigin.Begin at Time = 13
//  5. Check CurrentProgress and CurrentState properties after each Tick

//Pass Verification:
//   The output of this test should match the expected output in TMSeekExpect.txt

//Warnings:
//  Any changes made in the output should be reflected TMSeekExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TMSeek :ITimeBVT
    {
        TimeManagerInternal    _tManager;
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
            
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode.BeginTime             = TimeSpan.FromMilliseconds(0);
            tNode.Duration              = new Duration(TimeSpan.FromMilliseconds(20));
            tNode.Name                  = "TimeNode";
            
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, tClock, _tManager, 0, 25, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 13 )
            {
                _tManager.Seek( 16, TimeSeekOrigin.BeginTime );             
            }
        }       

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
