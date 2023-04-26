// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify applying Begin() a second time


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (tContainer)
//  3. Create a Clock and associate it with the Timeline (tContainer)

//Pass Condition:
//   This test passes if the events fire appropriately.

//Pass Verification:
//   The output of this test should match the expected output in b.111Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug111Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug111 : ITimeBVT
    {
        ParallelTimeline    _tContainer;
        ParallelTimeline    _tNode1;
        ParallelTimeline    _tNode2;
        Clock       _tClock;
        
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
            _tContainer.BeginTime              = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration               = new Duration(TimeSpan.FromMilliseconds(20));
            _tContainer.Name                     = "Container";
            //Attach Event Handler to tNode
            AttachCurrentStateInvalidated( _tContainer );

            // Create child Timeline 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            _tNode1.BeginTime    = null;
            _tNode1.Duration     = new Duration(TimeSpan.FromMilliseconds(2));
            _tNode1.FillBehavior = FillBehavior.HoldEnd;
            _tNode1.Name           = "TimeNode1";
            //Attach Event Handler to tNode1
            AttachCurrentStateInvalidated( _tNode1 );

            // Create child Timeline 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            _tNode2.BeginTime    = null;
            _tNode2.Duration     = new Duration(TimeSpan.FromMilliseconds(2));
            _tNode2.FillBehavior = FillBehavior.HoldEnd;
            _tNode2.Name           = "TimeNode2";
            //Attach Event Handler to tNode2
            AttachCurrentStateInvalidated( _tNode2 );

            //Attach the TimeNodes to the Container
            _tContainer.Children.Add(_tNode1);
            _tContainer.Children.Add(_tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 21, 1, ref outString);

            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 1 )
            {
                _tClock.Controller.SkipToFill();
                _tClock.Controller.Begin();
            }
            if ( i == 6 )
            {
                _tClock.Controller.SkipToFill();
                _tClock.Controller.Begin();
            }
            if ( i == 10 )
            {
                _tClock.Controller.SkipToFill();
                _tClock.Controller.Begin();
            }
            if ( i == 14 )
            {
                _tClock.Controller.SkipToFill();
                _tClock.Controller.SkipToFill();
            }
        }
    }
}
