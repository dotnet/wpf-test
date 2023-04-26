// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Seeking when Duration = Forever (TimeSeekOrigin.Duration)
 */


//Pass Verification:
//   The output of this test should match the expected output in ICSeekPastDuration15Expect.txt

//Warnings:
//  Any changes made in the output should be reflected ICSeekPastDuration15Expect.txt file

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
    class ICSeekPastDuration15 :ITimeBVT
    {
        ParallelTimeline    _tNode;
        Clock               _tClock;
        
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
            
            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create Timeline" , " Created Timeline " );
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(1);
            _tNode.Duration          = Duration.Forever;
            
            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            AttachCurrentGlobalSpeedInvalidatedTC(_tClock);
            AttachCurrentTimeInvalidatedTC(_tClock);
            AttachCompletedTC(_tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 7, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 2 )
            {
                try
                {
                    _tClock.Controller.Seek( TimeSpan.FromMilliseconds(0), TimeSeekOrigin.Duration );
                }
                catch (Exception e)
                {
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.InvalidOperationException), e.GetType(), ref outString );
                }
            }
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
