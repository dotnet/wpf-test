// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description: Verify Seek when a Timeline parent has no Duration set
 *              and its child has Duration=Forever

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Parent Timeline with begin=0 and no Duration
//  3. Create a Child Timeline with BeginTime=0 and Duration=Forever
//  4. Start the TimeManager at timeline = 0
//  5. Seek the Timeline with offset = 4 at Tick #7
//  6. Check the CurrentProgress and CurrentState  properties after each Tick


//Pass Verification:
//   The output of this test should match the expected output in ICSeekNoDuration2Expect.txt

//Warnings:
//  Any changes made in the output should be reflected ICSeekNoDuration2Expect.txt file

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
    class ICSeekNoDuration2 :ITimeBVT
    {
        Clock               _clock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a Parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent Timeline" , " Created Parent Timeline " );
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Name          = "Parent";           
            
            // Create a Child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child Timeline" , " Created Child Timeline " );
            child.BeginTime     = TimeSpan.FromMilliseconds(0);
            child.Duration      = Duration.Forever;
            child.Name          = "Child";           
            
            //Add the Child to the Parent.
            parent.Children.Add(child);
            
            // Create a Clock, connected to the Timeline.
            _clock = parent.CreateClock();     
            DEBUG.ASSERT(_clock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, _clock, tManager, 0, 11, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 4 )
            {
                _clock.Controller.Seek( TimeSpan.FromMilliseconds(7), TimeSeekOrigin.BeginTime );
            }
        }
    }
}
