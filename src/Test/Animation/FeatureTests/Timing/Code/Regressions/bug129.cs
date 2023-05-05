// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Progress when the Speed/Duration ratio is a fraction

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Condition:
//  The Progress property should be rounded appropriately.

//Pass Verification:
//  The output of this test should match the expected output in bug129Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug129Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug129 :ITimeBVT
    {
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

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
            tNode.BeginTime    = TimeSpan.FromMilliseconds(2);
            tNode.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.SpeedRatio   = 3;
            tNode.Name           = "Timeline";

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();          
            DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );

            // Attach an event, to force CurrentProgress to update.
            AttachCurrentTimeInvalidatedTC(tClock);

            //Run the Timer
            TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 11, 1, ref outString);

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
