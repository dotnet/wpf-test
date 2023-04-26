// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Timeline container's RepeatCount & AutoReverse properties
 */

//Instructions:
//  1. Create a TimeNode with BeginTime=0, Duration=20, RepeatCount=2, and AutoReverse=true
//  2. Create a TimeContainer with BeginTime=0 , RepeatCount=2 and AutoReverse=true and attach the TimeNode
//  3. Attach event handlers to the TimeNode
//  4. Create a TimeManager with Start=0 and Step=5 and add the TimeNodes
//  5. Check the CurrentState Property on each fired Event

//Warnings:
//  Any changes made in the output should be reflected in ReverseRepeatExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ContainerReverseRepeat :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.BeginTime                    = TimeSpan.FromMilliseconds(0);
            tContainer.Name                           = "Parent";
            //NOTE: no Duration has been set on the Container.

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode.BeginTime                   = TimeSpan.FromMilliseconds(0);
            tNode.Duration                    = new Duration(TimeSpan.FromMilliseconds(20));
            tNode.RepeatBehavior              = new RepeatBehavior(2);
            tNode.AutoReverse                 = true;
            tNode.Name                        = "Child";

            // Attach Handlers to the Child Timeline
            AttachCurrentGlobalSpeedInvalidated( tNode );
            DEBUG.LOGSTATUS(" Attached EventHandlers ");

            // Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 210, 5, ref outString);
            
            return outString;
        }
        public override void OnProgress(Clock subject)
        {
            outString += "--------" + subject.Timeline.Name + ": CurrentProgress = " + subject.CurrentProgress;
            outString += " -- CurrentState = " + subject.CurrentState + "\n" ;          
        }
    }
}
