// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Test the Timeline's FillBehavior.HoldEnd Property
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with FillBehavior.HoldEnd
//  3. Attach the node to the manager
//  4. At every clock tick check the properties CurrentProgress, CurrentState

//Pass Verification:
//   The output of this test should match the expected output in FillHoldExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected FillHoldExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class FillHold : ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline with AutoReverse = true
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            tNode.Duration      = new System.Windows.Duration(TimeSpan.FromMilliseconds(1000));
            tNode.FillBehavior  = FillBehavior.HoldEnd;
            tNode.Name            = "TimeNode";

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Read the value of tNode.Fill
            outString  = tNode.FillBehavior.ToString() + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, tClock, tManager, 0, 1200, 100, ref outString);
            
            //Read the value of tNode.Fill again.
            outString += "TimeFill returns: " + tNode.FillBehavior;
            
            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
