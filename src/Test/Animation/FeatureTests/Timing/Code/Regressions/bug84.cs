// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Seek after Begin and Pause are applied

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tNode)
//  3. Create a Clock and associate it with the Timeline (tNode)
//  4. Start the TimeManager and invoke the Seek() and Begin() methods at intervals.

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug84Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug84Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug84 : ITimeBVT
    {
         Clock _tClock;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
            tNode.BeginTime             = null;
            tNode.Duration              = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.Name                    = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Event Handler to tClock
            AttachCurrentStateInvalidatedTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 12, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 0 )
            {
                _tClock.Controller.Begin();
            }
            if ( i == 2 )
            {
                _tClock.Controller.Pause();
                _tClock.Controller.Seek(TimeSpan.FromMilliseconds(4), TimeSeekOrigin.BeginTime);
            }
            if ( i == 3 )
            {
                _tClock.Controller.Resume();
            }
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "  " + ((Clock)subject).Timeline.Name + ": CurrentState = " + ((Clock)subject).CurrentState + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress     = " + ((Clock)subject).CurrentProgress + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": IsPaused     = " + ((Clock)subject).IsPaused + "\n";
        }
    }
}
