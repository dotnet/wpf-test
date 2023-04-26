// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify CurrentTime for a child Timeline when its Duration is set to Time.Unspecified

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tContainer) and children Timelines (tNode1 and tNode2)
//  3. Create a Clock and associate it with the parent Timeline (tContainer)

//Pass Condition:
//   This test passes if the CurrentTime property returns 0 for the Children.

//Pass Verification:
//   The output of this test should match the expected output in b.117Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug117Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug117 : ITimeBVT
    {
        ParallelTimeline    _tContainer;
        ParallelTimeline    _tNode1;
        ParallelTimeline    _tNode2;
        ClockGroup          _tClock;
        Clock               _tClock2;
        
        TimeManagerInternal _tManager;
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";     

            // Create a TimeManager
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create Container", " Created Container ");
            _tContainer.BeginTime            = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration             = new Duration(TimeSpan.FromMilliseconds(10));
            _tContainer.Name                   = "Container";

            // Create child Timeline 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            _tNode1.BeginTime    = TimeSpan.FromMilliseconds(4);
            _tNode1.Duration     = new Duration(TimeSpan.FromMilliseconds(2));
            _tNode1.Name           = "TimeNode1";

            // Create child Timeline 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            _tNode2.BeginTime    = TimeSpan.FromMilliseconds(8);
            _tNode2.Duration     = Duration.Automatic;
            _tNode2.FillBehavior = FillBehavior.HoldEnd;
            _tNode2.Name           = "TimeNode2";

            //Attach the TimeNodes to the Container
            _tContainer.Children.Add(_tNode1);
            _tContainer.Children.Add(_tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

            // Create a Clock, connected to the Container Timeline.
            _tClock = _tContainer.CreateClock();
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            _tClock2 = _tClock.Children[1];           
            DEBUG.ASSERT(_tClock2 != null, "Cannot create Clock2" , " Created Clock2 " );

            //Attach Event Handlers to tClock
            AttachCurrentStateInvalidatedTC( _tClock );

            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }
          
        public override void PreTick( int i )
        {
            outString += "----" + _tClock2.Timeline.Name + ": CurrentTime = " + _tClock2.CurrentTime + "\n";
        }       

        public override void OnProgress( Clock subject )
        {
        }

        public override void OnCurrentStateInvalidated(object subject, EventArgs args)
        {
            outString += "---" + _tClock2.Timeline.Name + ": CurrentTime = " + _tClock2.CurrentTime + "\n";
        }
    }
}
