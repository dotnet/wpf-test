// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the SlipBehavior property
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create Timelines with different SlipBehavior settings
//  3. Attach the node to the manager
//  4. At every clock tick check properties

//Pass Verification:
//   The output of this test should match the expected output in SBSlipBehaviorExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected SBSlipBehaviorExpect.txt file

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
    class SBSlipBehavior : ITimeBVT
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
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create TimeNode" , " Created Timeline " );
            parent.BeginTime     = TimeSpan.FromMilliseconds(0);
            parent.Duration      = new Duration(TimeSpan.FromMilliseconds(1000));
            parent.SlipBehavior  = SlipBehavior.Slip;
            parent.Name          = "Parent";

            // Create a Child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create a Child TimeNode" , " Created a Child Timeline " );
            child.BeginTime     = TimeSpan.FromMilliseconds(0);
            child.Duration      = new Duration(TimeSpan.FromMilliseconds(500));
            child.SlipBehavior  = SlipBehavior.Grow;
            child.Name          = "Child";

            // Create a GrandChild Timeline
            ParallelTimeline grandchild = new ParallelTimeline();
            DEBUG.ASSERT(grandchild != null, "Cannot create a GrandChild TimeNode" , " Created a GrandChild Timeline " );
            grandchild.BeginTime     = TimeSpan.FromMilliseconds(0);
            grandchild.Duration      = new Duration(TimeSpan.FromMilliseconds(500));
            grandchild.SlipBehavior  = SlipBehavior.Slip;
            grandchild.Name          = "GrandChild";

            //Attach the Children
            parent.Children.Add(child);
            child.Children.Add(grandchild);
            
            // Create a Clock, connected to the Timeline.
            Clock clock = parent.CreateClock();       
            DEBUG.ASSERT(clock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(clock);

            //Read the value of parent.Fill
            outString  = parent.SlipBehavior.ToString() + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, clock, tManager, 0, 1100, 100, ref outString);
            
            //Read the value of parent.SlipBehavior again.
            outString += "SlipBehavior [Parent]     returns: " + parent.SlipBehavior + "\n";
            outString += "SlipBehavior [Child ]     returns: " + child.SlipBehavior + "\n";
            outString += "SlipBehavior [GrandChild] returns: " + grandchild.SlipBehavior + "\n";
            
            return outString;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "--CurrentProgress    [" + ((Clock)subject).Timeline.Name + "]: " + subject.CurrentProgress + "\n";
            outString += "--CurrentGlobalSpeed [" + ((Clock)subject).Timeline.Name + "]: " + subject.CurrentGlobalSpeed + "\n";
            outString += "--CurrentState       [" + ((Clock)subject).Timeline.Name + "]: " + subject.CurrentState + "\n";
            outString += "--CurrentTime        [" + ((Clock)subject).Timeline.Name + "]: " + subject.CurrentTime + "\n";
            outString += "--CurrentIteration   [" + ((Clock)subject).Timeline.Name + "]: " + subject.CurrentIteration + "\n";
            outString += "--IsPaused           [" + ((Clock)subject).Timeline.Name + "]: " + subject.IsPaused + "\n";
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
