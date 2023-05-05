// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify thrown exception of the public implicit method operator within class Duration

 */

//Instructions:
//  1. Create a negative TimeSpan
//  2. Casting the TimeSpan to Duration type
//  2. Verify an exception thrown for negative TimeSpan as parameter to a Duration.

//Pass Condition:
//  All checks of the operators should pass.

//Pass Verification:
//  The output of this test should match the expected output in Thrown_DurationExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected Thrown_DurationExpect.txt file

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
    class Thrown_Duration : ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(-10000);
            DEBUG.LOGSTATUS("Create negative TimeSpan : " + timeSpan.ToString());
            try
            {
                Duration duration = (Duration)timeSpan;
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentException), e.GetType(), ref outString );
            }
            DEBUG.LOGSTATUS( "Casting Negative TimeSpan to Duration and thrown exception" );
            
            return outString;
        }
     }
}
