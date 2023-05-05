// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the instance and static method Equals in RepeatBehavior with different parameters

 */

//Instructions:
//  1. Create a RepeatBehavior with an Object parameter
//  2. Verify the return value of Equal method (instance)
//  3. Create two RepeatBehavior with both Forever type
//  4. Verify the return value of Equal method (instance)
//  5. Create two RepeatBehavior with the same TimeSpan values
//  6. Verify the return value of Equal method (static)


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
    class RepeatBehaviorEqual :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            ParallelTimeline parallelTimeline = new ParallelTimeline();          
            
            RepeatBehavior rb1 = new RepeatBehavior(25);
            RepeatBehavior rb2 = RepeatBehavior.Forever;
            RepeatBehavior rb3 = RepeatBehavior.Forever;
        
            
            //To instance method Equals with Object parameter
            DEBUG.LOGSTATUS("Checking a Non-RepeatBehavior object as a parameter");
            outString += "The non-RepeatBehavior parameter's type : " + parallelTimeline.ToString() + "\n";
            outString += "Non-RepeatBehavior parameter returns : " + rb1.Equals(parallelTimeline).ToString()+ "\n";

            //To instance method Equals with RepeatBehavior parameter
            DEBUG.LOGSTATUS("Checking a RepeatBehavior with Forever type ");
            outString += "Two RepeatBehaviors with both Forever type return : " + rb2.Equals(rb3).ToString()+ "\n";
            
            //To static method Equals
            DEBUG.LOGSTATUS("Checking static method Equals in RepeatBehavior");
            Check(rb1, rb2, RepeatBehavior.Equals(rb2, rb3), "Static method Equals with two RepeatBehavior parameters returns");
        
            return outString;
        } 
        private void Check( RepeatBehavior r1, RepeatBehavior r2, bool condition, string message )
        {
            outString += message + " : "  + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
            outString += "The 1st RepeatBehavior's Type = " + r1.ToString() + "\n";
            outString += "The 2nd RepeatBehavior's Type = " + r2.ToString() + "\n";
        }
    }
}
