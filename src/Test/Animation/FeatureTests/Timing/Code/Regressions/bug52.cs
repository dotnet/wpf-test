// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify the default value of a Timeline's Name property


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (parent)
//  3. Create a Clock and associate it with the Timeline (parent)

//Pass Condition:
//   This test passes if the expected Name is returned.

//Pass Verification:
//   The output of this test should match the expected output in b.52Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug52Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug52 : ITimeBVT
    {   
        TimeManagerInternal     _tManager;
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

            // Create a Timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create parenparent", " Created parent Timeline ");
            parent.BeginTime        = TimeSpan.FromMilliseconds(0);
            parent.Duration         = new Duration(TimeSpan.FromMilliseconds(100));

            // Create child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child Timeline" , " Created Child Timeline" );
            child.BeginTime         = TimeSpan.FromMilliseconds(10);
            child.Duration          = new Duration(TimeSpan.FromMilliseconds(80));

            //Attach Child Timeline to Parent
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Timeline ");
             
            outString += "----Default parent Name:  " + parent.Name  + "\n";               
            outString += "----Default child Name:   " + child.Name  + "\n";               
               
            return outString;
        }
    }
}
