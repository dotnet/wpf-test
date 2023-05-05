// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting Controller methods on Clock children
 */

//Instructions:
//  1. Create parent and child Timelines with BeginTime = null
//  2. Attach the Timeline to a Clock
//  3. Start the TimeManager and attempt to apply interactive methods on Clock children

//Warnings:
//  Any changes made in the output should be reflected in ICOnChildExpect.txt file

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
    class ICOnChild :ITimeBVT
    {
        ClockGroup _tClockP;
        ClockGroup _tClock1;
        ParallelTimeline _parent;
        ParallelTimeline _child1;
        ParallelTimeline _child2;
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            _parent = new ParallelTimeline();
            DEBUG.ASSERT(_parent != null, "Cannot create Parent", " Created Parent ");
            _parent.BeginTime        = TimeSpan.FromMilliseconds(0);
            _parent.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(20));
            _parent.Name             = "Parent";

            // Create Child 1
            _child1 = new ParallelTimeline();
            DEBUG.ASSERT(_child1 != null, "Cannot create Child 1" , " Created Child 1 " );
            _child1.BeginTime        = null;
            _child1.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(2));
            _child1.Name             = "Child1";

            // Create Child 2
            _child2 = new ParallelTimeline();
            DEBUG.ASSERT(_child1 != null, "Cannot create Child 2" , " Created Child 2 " );
            _child2.BeginTime        = null;
            _child2.Duration         = new System.Windows.Duration(TimeSpan.FromMilliseconds(2));
            _child2.Name             = "Child2";

            // Create a hierarchy of Timelines
            _child1.Children.Add(_child2);
            _parent.Children.Add(_child1);
            DEBUG.LOGSTATUS(" Attached children to parents ");

            // Create a Clock, connected to the container.
            _tClockP = _parent.CreateClock();        
            DEBUG.ASSERT(_tClockP != null, "Cannot create ClockP" , " Created ClockP " );
            
            // Create a Clock for the Child.
            _tClock1 = (ClockGroup)_tClockP.Children[0];
            DEBUG.ASSERT(_tClockP != null, "Cannot create Clock1" , " Created Clock1 " );
            
            AttachAllHandlersTC(_tClockP);

            outString += "-P-Before--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-Before--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-Before--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClockP, tMan, 0, 2, 1, ref outString);

            outString += "-P-After--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-After--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-After--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
            
            return outString;
        }

        public override void PostTick(int i)
        {
            outString += "-P-PostTick--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-PostTick--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-PostTick--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
        }
        
        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
            outString += "-P-CurrentTime--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-CurrentTime--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-CurrentTime--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
        }
        
        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "-P-CurrentState--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-CurrentState--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-CurrentState--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
        }
        
        public override void OnCurrentGlobalSpeedInvalidated(object subject, EventArgs args)
        {
            outString += "-P-CurrentGlobalSpeed--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-CurrentGlobalSpeed--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-CurrentGlobalSpeed--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
        }
        
        public override void OnCompleted(object subject, EventArgs args)
        {
            outString += "-P-Completed--Controller is null: " + (_tClockP.Controller == null).ToString() + "\n";
            outString += "-1-Completed--Controller is null: " + (_tClockP.Children[0].Controller == null).ToString() + "\n";
            outString += "-2-Completed--Controller is null: " + (_tClock1.Children[0].Controller == null).ToString() + "\n";
        }
    }
}
