// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify invoking Begin() via the Ended event



*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with attached event handlers
//  3. Create a Clock, associated with the Timeline
//  4. Start the TimeManager


//Pass Condition:
//  The Begun event should fire as a result of invoking Begin().

//Pass Verification:
//  The output of this test should match the expected output in bug61Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug61Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug61 :ITimeBVT
     {
        private int _beginCount = 0;
          /*
           *  Function:   Test
           *  Arguments:  Framework
           */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline", " Created Timeline ");
            tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.Name                = "TimeNode";
            //Attach Event Handlers to tNode
            AttachCurrentStateInvalidated( tNode );

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();          
            DEBUG.ASSERT(tClock != null, "Cannot create Clock 1" , " Created Clock 1 " );

            //Intialize output String
            outString = "";

            //Run the Timer
            TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 22, 1, ref outString);

            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress = " + ((Clock)subject).CurrentProgress + "\n";
        }
          
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {               

           if (((Clock)subject).CurrentState == ClockState.Active)
           {
               outString += "  " + ((Clock)subject).Timeline.Name + ": Timeline Begun = " + ((Clock)subject).CurrentProgress + "\n";
           }
           else
           {
                outString += "  " + ((Clock)subject).Timeline.Name + ": Timeline Ended = " + ((Clock)subject).CurrentProgress + "\n";

                if (_beginCount == 0)
                {
                    ((Clock)subject).Controller.Begin();
                }
                _beginCount++;
           }

        }
    }
}
