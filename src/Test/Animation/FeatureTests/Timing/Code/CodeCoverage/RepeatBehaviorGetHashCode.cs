// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the return value of GetHashCode method when RepeatBehavior's types are both IterationCount and Forever
 
 */

//Instructions:
//  1. Create RepeatBehavior with different types of IterationCount and Forever 
//  2. Verify the return value of GetHashCode method.

//Pass Condition:
//  All checks of the operators should pass.

//Pass Verification:
//  The output of this test should match the expected output in DurationOperatorsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DurationOperatorsExpect.txt file

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
    class RepeatBehaviorGetHashCode :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            RepeatBehavior rb1 = new RepeatBehavior(78);
            RepeatBehavior rb2 = RepeatBehavior.Forever;
           
            //RepeatBehavior with a IterationCount parameter (double type)
            DEBUG.LOGSTATUS("Checking HashCode of a RepeatBehavior with IterationCount type");
            outString += "HasCount method returns : " + rb1.HasCount.ToString() + "\n";
            outString += "The RepeatBehavior with IterationCount type returns the HashCode : " + rb1.GetHashCode().ToString() + "\n";
 
            //RepeatBehavior with Forever type
            DEBUG.LOGSTATUS("Checking HashCode of a RepeatBehavior with Forever type");
            outString += "RepeatBehavior with type of : " + rb2.ToString() + "\n";            
            outString += "HasCount method returns : " + rb2.HasCount.ToString() + "\n";
            outString += "HasDuration method returns : " + rb2.HasDuration.ToString() + "\n";            
            outString += "The RepeatBehavior with Forever type returns the HashCode : " + rb2.GetHashCode().ToString() + "\n";
 
            return outString;
        }
    }
}
