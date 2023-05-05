// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify Begin() on a Clock with a child
 *              NOTE: replacing BeginIn(1) at i=5 with Begin() at i=6
 */

//Instructions:
//  1. Create a TimeContainer with BeginTime = null
//  2. Create a TimeNode with BeginTime = 0 and Duration = 5
//  4. Attach Begun and Ended eventHandlers to the timelines
//  5. Attach TimeNode to Container
//  6. Create a Clock associcated with the Container
//  7. At Step = 5 call Begin() on the Container's Clock

//Warnings:
//  Any changes made in the output should be reflected in BeginInExpect.txt file

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
    class BeginIn :ITimeBVT
    {

        ParallelTimeline        _tNode;
        ClockGroup              _tClock;
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            tContainer.BeginTime   = null;
            tContainer.Duration    = new Duration(TimeSpan.FromMilliseconds(10));
            tContainer.Name        = "TimeContainer";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create a TimeNode
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            
            // Set Properties to the TimeNode
            _tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            _tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(5));
            _tNode.Name          = "TimeNode";
            DEBUG.LOGSTATUS(" Set TimeNode Properties ");

            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(_tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            // Create a Clock, connected to the container.
            _tClock = tContainer.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers
            AttachCurrentStateInvalidatedTC( _tClock );
            AttachCurrentStateInvalidatedTC( _tClock.Children[0] );
            DEBUG.LOGSTATUS(" Attached EventHandlers");

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 20, 1, ref outString);
            
            return outString;
        }

        public override void PostTick(int i)
        {
            if ( i == 6 )
            {
                _tClock.Controller.Begin();
            }
        }
    }
}
