// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify thrown exception for the method This[index]{get;set} in TimelineCollection class         

 */

//Instructions:
//  1. Create a Time Container
//  2. Create time node with properties
//  3. Add time node as child to the Time Container
//  4. Verify exception wil be thrown when index value (as parameter of RemoveAt method 
//      is out of range of the internal count

//Pass Condition:
//  Test passes when removed child time node no longer exists in the Time Container.

//Pass Verification:
//  The output of this test should match the expected output in TLCException.txt.

//Warnings:
//  Any changes made in the output should be reflected TLCException.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class Thrown_TLCThis : ITimeBVT
    {
        TimelineCollection _tc;
        ParallelTimeline _tContainer,_tNode1,_tNode2;

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
            _tContainer = new ParallelTimeline();
            DEBUG.ASSERT(_tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            _tContainer.BeginTime  = TimeSpan.FromMilliseconds(0);
            _tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(8));
            _tContainer.Name         = "Container";

            // Create TimeNode 1
            _tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            _tNode1.BeginTime   = TimeSpan.FromMilliseconds(1);
            _tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            _tNode1.Name          = "Timeline1";

            // Create TimeNode 2
            _tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(_tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            _tNode2.BeginTime   = TimeSpan.FromMilliseconds(2);
            _tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            _tNode2.Name          = "Timeline2";

            //Obtain a TimelineCollection for the Container
            _tc = _tContainer.Children;
            DEBUG.ASSERT(_tc != null, "Cannot create TimelineCollection" , " Create TimelineCollection" );
            outString += "Count Before: " + _tc.Count + "\n";

            //Add a new time node to the TimelineCollection
            DEBUG.LOGSTATUS("Add child time nodes." );
            _tc.Add(_tNode1);
            _tc.Add(_tNode2);

            // Create a Clock, connected to the container.
            Clock tClock = _tContainer.CreateClock();          
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 10, 1, ref outString);

            return outString;
        }
        
        public override void PreTick( int i )
        {
            if ( i == 2 )
            {
                //Call RemoveAt method to throw exception
                try
                {
                    //Assign timeline to the TimelineCollection
                    DEBUG.LOGSTATUS("Test \'{set}\' method." );
                    _tc[-1] = _tNode2;  
                }
                catch (Exception e1)
                {
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentOutOfRangeException), e1.GetType(), ref outString );
                }
                DEBUG.LOGSTATUS( "Out of Range Index: Negative index" );
            }
            if ( i == 5 )
            {
                try
                {
                    //Assign timeline to the TimelineCollection
                    DEBUG.LOGSTATUS("Test \'{set}\' method." );
                    _tc[2] = _tNode1;  
                }
                catch (Exception e2)
                {
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentOutOfRangeException), e2.GetType(), ref outString );
                }
                DEBUG.LOGSTATUS( "Out of Range Index: index exceeding the existing count" );
            }
        }

        public override void PostTick( int i )
        {
            if ( i == 6 )
            {
                outString += "Count child Timeline:  " + _tc.Count + "\n";                           
                try
                {
                    //Reading from the TimelineCollection
                    DEBUG.LOGSTATUS("Test \'{get}\' method." );
                    ParallelTimeline t = (ParallelTimeline)_tc[-1];  
                }
                catch (Exception e3)
                {
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentOutOfRangeException), e3.GetType(), ref outString );
                }
                DEBUG.LOGSTATUS( "Out of Range Index: Negative index" );
            } 
            if ( i == 8 )
            {
                try
                {
                    //Reading from the TimelineCollection
                    DEBUG.LOGSTATUS("Test \'{get}\' method." );
                    ParallelTimeline t = (ParallelTimeline)_tc[2];  
                }
                catch (Exception e4)
                {
                    TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentOutOfRangeException), e4.GetType(), ref outString );
                }
                DEBUG.LOGSTATUS( "Out of Range Index: index exceeding the existing count" );
            }
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}
