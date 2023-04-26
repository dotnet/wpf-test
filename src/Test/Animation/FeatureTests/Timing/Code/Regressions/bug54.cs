// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify "edge" event firing when the Tick is repeated
*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline
//  3. Create a Clock, associated with the parent Timeline; attach event handlers
//  4. Start the TimeManager and Tick at specified intervals


//Pass Condition:
//  This test passes if events fire as expected.

//Pass Verification:
//  The output of this test should match the expected output in bug54Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug54Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
     class bug54 :ITimeBVT
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

            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create parent Timeline" , " Created parent Timeline " );
            parent.BeginTime            = TimeSpan.FromMilliseconds(0);
            parent.Duration             = new Duration(TimeSpan.FromMilliseconds(50));
            parent.Name                   = "Parent";

            ParallelTimeline child1 = new ParallelTimeline();
            DEBUG.ASSERT(child1 != null, "Cannot create child1 Timeline", " Created child1 Timeline ");
            child1.BeginTime             = TimeSpan.FromMilliseconds(0);
            child1.Duration              = new Duration(TimeSpan.FromMilliseconds(20));
            child1.AutoReverse           = true;
            child1.Name                    = "Child1";

            ParallelTimeline child2 = new ParallelTimeline();
            DEBUG.ASSERT(child2 != null, "Cannot create child2 Timeline", " Created child2 Timeline ");
            child2.BeginTime             = TimeSpan.FromMilliseconds(0);
            child2.Duration              = new Duration(TimeSpan.FromMilliseconds(20));
            child2.RepeatBehavior        = new RepeatBehavior(2);
            child2.Name                    = "Child2";

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            // Create a Clock, connected to the container Timeline.
            ClockGroup clockParent = parent.CreateClock();          
            DEBUG.ASSERT(clockParent != null, "Cannot create parent Clock" , " Created paren Clock " );
            
            //Attach Event Handlers to clockParent
            AttachCurrentGlobalSpeedInvalidatedTC( clockParent );

            Clock clockChild1 = clockParent.Children[0];            
            DEBUG.ASSERT(clockChild1 != null, "Cannot create child1 Clock" , " Created child1 Clock " );
            
            //Attach Event Handlers to clockChild1
            AttachCurrentGlobalSpeedInvalidatedTC( clockChild1 );

            Clock clockChild2 = clockParent.Children[1];            
            DEBUG.ASSERT(clockChild2 != null, "Cannot create child2 Clock" , " Created child2 Clock " );
           
           //Attach Event Handlers to clockChild2
            AttachCurrentGlobalSpeedInvalidatedTC( clockChild2 );

            //Run the Timer
            tManager.Start();

            int[] a = new int[]{0,10,20,20,20,30,40,50,60};
            foreach(int i in a )
            {
                outString += "----Processing time: " + (int) i + " ms\n";
                outString += "CurrentState Parent:\t"  + clockParent.CurrentState.ToString()  + "\n";
                outString += "CurrentState Child1:\t"  + clockChild1.CurrentState.ToString()  + "\n";
                outString += "CurrentState Child2:\t"  + clockChild2.CurrentState.ToString()  + "\n";

                CurrentTime = TimeSpan.FromMilliseconds(i);
                tManager.Tick();    
            }

            return outString;
        }

          public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
          {
               outString += "----------CurrentGlobalSpeedInvalidated----------" + "\n";
          }
    }
}
