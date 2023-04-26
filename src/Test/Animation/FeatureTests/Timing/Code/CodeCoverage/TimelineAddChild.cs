// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the internal method IAddChild.AddChild within class Timeline

 */

//Instructions:
//  1. Create a Time Container
//  2. Create new time node with properties
//  3. Add the time node as child to the Time Container using IAddChild.AddChild
//  4. Verify the property of the added child Timeline

//Pass Condition:
//  Test passes when added child time node exists.

//Pass Verification:
//  The output of this test should match the expected output in TimelineAddChildExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TimelineAddChildExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class TimelineAddChild : ITimeBVT
    {
        TimelineGroup _tContainer;
        ParallelTimeline   _tNode;
        Clock      _tClock;

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

            // Create a tContainer
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create Time Container", " Created Time Container ");
            _tContainer.BeginTime     = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(8));
            _tContainer.Name            = "Container";

            // Create a Clock, connected to the container.
            _tClock = _tContainer.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);

            // Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 10, 1, ref outString);

            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 5 )
            {
                // Create a TimeNode
                _tNode = new ParallelTimeline();
                DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created Timeline " );          
                // Set Properties to the TimeNode
                _tNode.BeginTime    = TimeSpan.FromMilliseconds(0);
                _tNode.Duration = new Duration(TimeSpan.FromMilliseconds(50));
                _tNode.Name           = "TimeNode";
                DEBUG.LOGSTATUS(" Set Timeline Properties ");

                //Add TimeNode to the TimeContainer         
                ((IAddChild)_tContainer).AddChild(_tNode);                

                DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

                outString += " This timeline :  " + _tContainer.Name + ". The number of its children : " + _tContainer.Children.Count + "\n";
                outString += " The name of contained timeline (child) : " + _tContainer.Children[0].Name + "\n";
            }           
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
