// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify  SpeedChanged event for a child when its parent is resumed after a pause
 
*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline parent (tContainer) and a child (parent)
//  3. Create a Clock and associate it with the parent (tContainer)

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug136Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug136Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug136 : ITimeBVT
    {
         ClockGroup _clockParent;
         Clock      _clockChild;
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

            // Create a Parent Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create parent Timeline", " Created parent Timeline ");
            parent.BeginTime             = TimeSpan.FromMilliseconds(10);
            parent.Duration              = new Duration(TimeSpan.FromMilliseconds(50));
            parent.Name                    = "Parent";

            // Create a Child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create child Timeline", " Created child Timeline ");
            child.BeginTime              = TimeSpan.FromMilliseconds(0);
            child.Duration               = new Duration(TimeSpan.FromMilliseconds(50));
            child.Name                     = "Child";

            parent.Children.Add(child);

            // Create a Clocks, connected to the Timelines.
            _clockParent = parent.CreateClock();       
            DEBUG.ASSERT(_clockParent != null, "Cannot create parent Clock" , " Created parent Clock " );
            _clockChild = _clockParent.Children[0];;      
            DEBUG.ASSERT(_clockChild != null, "Cannot create child Clock" , " Created child Clock " );
            
            // Attach Event Handler to the child.
            AttachCurrentGlobalSpeedInvalidatedTC( _clockChild );

            // Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _clockParent, tManager, 0, 100, 10, ref outString);
               
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 20 )
            {
                _clockParent.Controller.Pause();
            }
            if ( i == 40 )
            {
                _clockParent.Controller.Resume();
            }
        }
    }
}
