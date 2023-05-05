// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test Timeline DecelerationRatio Property
 *                                            Also, Deceleration is now called DecelerationRatio.
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 and Duration = 100
//  2. Give an Deceleration value of 0.6
//  3. Create a Clock, associated with the Timeline
//  4. Create a TimeManager with Start=0 and Step = 1
//  5. Check the progress after each tick

//Warnings:
//  Any changes made in the output should be reflected DecelerationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Deceleration :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and sets and gets the Deceleration property.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime             = TimeSpan.FromMilliseconds(0);
            tNode.Duration              = new System.Windows.Duration(TimeSpan.FromMilliseconds(100));
            tNode.Name                  = "SimpleTimeNode";
            tNode.DecelerationRatio     = 0.6f;
            DEBUG.ASSERT( tNode.DecelerationRatio == 0.6f , "Deceleration is 0.6" , "Deceleration is not 0.6");
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach to a TimeManager           
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock,  tMan, 0, 100, 1, ref outString);
            
            //Place 'Progress' in outString in order to force a call to LocalizeProgressValue().
            outString += "(Progress) Acceleration returns: " + ((Math.Round(tNode.AccelerationRatio * 10))/10).ToString() + "\n";
            outString += "(Progress) Deceleration returns: " + ((Math.Round(tNode.DecelerationRatio * 10))/10).ToString();
            return outString;
        }
    }
}
