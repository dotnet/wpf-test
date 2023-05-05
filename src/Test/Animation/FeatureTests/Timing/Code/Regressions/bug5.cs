// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify CurrentTime when an accelerated Clock is Paused, Resumed, then Paused again

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager
//  5. Pause, resume, then pause the clock again

//Pass Condition:
//  The Timeline Clock's CurrentTime should change appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug5Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug5Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug5 :ITimeBVT
    {
        TimeManagerInternal     _tManager;
        Clock                   _tClock;
        ParallelTimeline        _tNode;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create Timeline", " Created Timeline ");
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            _tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            _tNode.AccelerationRatio = .5d;
            _tNode.Name              = "Timeline";

            // Create a Clock, connected to the container Timeline.
            _tClock = _tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 2 )
            {
                _tClock.Controller.Pause();
            }
            if ( i == 5 )
            {
                _tClock.Controller.Resume();
            }
            if ( i == 8 )
            {
                _tClock.Controller.Pause();
            }
        }
          public override void OnProgress( Clock subject )
          {
               outString += "  " + ((Clock)subject).Timeline.Name + ": Progress    = " + ((Clock)subject).CurrentProgress + "\n";
               outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentTime = " + ((Clock)subject).CurrentTime + "\n";
          }
    }
}
