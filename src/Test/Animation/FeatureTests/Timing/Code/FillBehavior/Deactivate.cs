// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify FillBehavior.Stop
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with FillBehavior = FillBehavior.Stop
//  3. Attach the node to the manager
//  4. At every clock tick check the properties: Progress and CurrentState

//Pass Verification:
//   The output of this test should match the expected output in DeactivateExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DeactivateExpect.txt file

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
    class Deactivate : ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent TimeNode" , " Created Parent Timeline " );
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
            parent.Name            = "Parent";

            // Create a Child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Child TimeNode" , " Created Child Timeline " );
            child.BeginTime      = TimeSpan.FromMilliseconds(0);
            child.Duration       = new Duration(TimeSpan.FromMilliseconds(4));
            child.FillBehavior   = FillBehavior.Stop;
            child.Name             = "Child";

            //Attach Handlers to the parent and child Timelinds
            AttachCurrentStateInvalidated(child);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Attach TimeNode to the TimeContainer
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached Child to the Parent ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = parent.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tManager, 0, 10, 1, ref outString);
            
            return outString;
        }

        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "-------- CurrentStateInvalidated: CurrentState = " + ((Clock)subject).CurrentState + "\n";        
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
