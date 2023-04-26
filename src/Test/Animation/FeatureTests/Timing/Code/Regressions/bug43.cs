// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify event firing with a negative BeginTime and Duration = Forever

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with a negative BeginTime
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager

//Pass Condition:
//  CurrentProgress, CurrentState, and CurrentTime should take into account the negative BeginTime.

//Pass Verification:
//  The output of this test should match the expected output in bug43Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug43Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug43 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ParallelTimeline      _tNode;
        Clock                 _tClock;

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

            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tNode.BeginTime      = TimeSpan.FromMilliseconds(-2);
            _tNode.Duration       = Duration.Forever;
            _tNode.Name           = "Timeline";

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Attach Event Handler to the clock.
            AttachAllHandlersTC( _tClock );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 6, 1, ref outString);

            WriteAllEvents();

            return outString;
        }
    }
}
