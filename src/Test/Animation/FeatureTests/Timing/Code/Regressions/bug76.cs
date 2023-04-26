// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify setting Acceleration and Deceleration on both parent and children

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline and add children to it
//  3. Create a Clock and associate it with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//   This test passes if all events fire appropriately

//Pass Verification:
//   The output of this test should match the expected output in bug76Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug76Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug76 : ITimeBVT
    {
         Clock            _clockParent;
         ParallelTimeline _parent;
         ParallelTimeline _timelineChild1;
         ParallelTimeline _timelineChild2;
         
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Timeline Parent
            _parent = new ParallelTimeline();
            DEBUG.ASSERT(_parent != null, "Cannot create Timeline", " Created Timeline ");
            _parent.BeginTime          = TimeSpan.FromMilliseconds(3);
            _parent.AccelerationRatio  = 0;
            _parent.DecelerationRatio  = 0.7493133f;
            _parent.AutoReverse        = true;
            _parent.Name                 = "Parent";
            _parent.FillBehavior       = FillBehavior.Stop;

            // Create a Timeline Child 1
            _timelineChild1 = new ParallelTimeline();
            DEBUG.ASSERT(_timelineChild1 != null, "Cannot create timelineChild1 Timeline", " Created timelineChild1 Timeline ");
            _timelineChild1.BeginTime         = TimeSpan.FromMilliseconds(5);
            _timelineChild1.Duration          = new Duration(TimeSpan.FromMilliseconds(8));
            _timelineChild1.RepeatBehavior    = new RepeatBehavior(3.786242f);
            _timelineChild1.AccelerationRatio = 0.3544517f;
            _timelineChild1.DecelerationRatio = 0.4286028f;
            _timelineChild1.Name                = "Child1";
            _timelineChild1.FillBehavior      = FillBehavior.Stop;

            // Create a Timeline Child 2
            _timelineChild2 = new ParallelTimeline();
            DEBUG.ASSERT(_timelineChild2 != null, "Cannot create timelineChild2 Timeline", " Created timelineChild2 Timeline ");
            _timelineChild2.BeginTime         = TimeSpan.FromMilliseconds(1);
            _timelineChild2.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            _timelineChild2.AutoReverse       = true;
            _timelineChild2.AccelerationRatio = 0.009689149f;
            _timelineChild2.DecelerationRatio = 0;
            _timelineChild2.Name                = "Child2";
            _timelineChild2.FillBehavior      = FillBehavior.Stop;

            _parent.Children.Add( _timelineChild1 );
            _parent.Children.Add( _timelineChild2 );

            // Create a Clocks, connected to the Timelines.
            _clockParent = _parent.CreateClock();       
            DEBUG.ASSERT(_clockParent != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _clockParent, tManager, 0, 100, 1, ref outString);

            WriteAllEvents();

            return outString;
        }
    }
}
