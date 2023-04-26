// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify RepeatBehaviorConverter methods
 */

//Instructions:
//  1. Create a Timeline, without setting any properties
//  2. Invoke RepeatBehaviorConverter methods.

//Pass Condition:
//  This test passes if the methods return appropriate values.

//Pass Verification:
//  The output of this test should match the expected output in RBConverterExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RBConverterExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class RBConverter :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results.
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline timeline = new ParallelTimeline();
            DEBUG.ASSERT(timeline != null, "Cannot create Timeline" , " Created Timeline " );
            
            //Must explicitly set RepeatBehavior with a TimeSpan to be able to read RepeatDuration.
            timeline.RepeatBehavior  = new RepeatBehavior(TimeSpan.FromMilliseconds(5));


            RepeatBehaviorConverter converter = new RepeatBehaviorConverter();
            outString += "RepeatBehaviorConverter   = " +  converter + "\n";

            // CanConvertFrom
            bool b = false;
            b = converter.CanConvertFrom(null, typeof(string));
            outString += "CanConvertFrom - string    = " +  b + "\n";
            b = converter.CanConvertFrom(null, typeof(double));
            outString += "CanConvertFrom - double    = " +  b + "\n";
            b = converter.CanConvertFrom(null, typeof(int));
            outString += "CanConvertFrom - int       = " +  b + "\n";

            // CanConvertTo
            b = converter.CanConvertTo(null, typeof(string));
            outString += "CanConvertTo - string    = " +  b + "\n";
            b = converter.CanConvertTo(null, typeof(double));
            outString += "CanConvertTo - double    = " +  b + "\n";
            b = converter.CanConvertTo(null, typeof(int));
            outString += "CanConvertTo - int       = " +  b + "\n";

            // ConvertFrom
            timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "0");
            outString += "ConvertFrom result = " +  timeline.RepeatBehavior + "\n";

            timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "2");
            outString += "ConvertFrom result = " +  timeline.RepeatBehavior + "\n";

            timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "Forever");
            outString += "ConvertFrom result = " +  timeline.RepeatBehavior + "\n";

            timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "0:0:0");
            outString += "ConvertFrom result = " +  timeline.RepeatBehavior + "\n";

            try
            {
                timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "-2");
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.ArgumentOutOfRangeException), e.GetType(), ref outString );
            }

            try
            {
                timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "25:25:25");
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.OverflowException), e.GetType(), ref outString );
            }

            try
            {
                timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, null);
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.NotSupportedException), e.GetType(), ref outString );
            }

            try
            {
                timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "abc");
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.FormatException), e.GetType(), ref outString );
            }

            try
            {
                timeline.RepeatBehavior = (RepeatBehavior)converter.ConvertFrom(null, CultureInfo.InvariantCulture, "");
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.FormatException), e.GetType(), ref outString );
            }

            // ConvertTo
            RepeatBehavior rb1 = new RepeatBehavior(1);
            RepeatBehavior rb2 = new RepeatBehavior(TimeSpan.FromMilliseconds(0d));
            InstanceDescriptor id;
            string s = "";
            
            s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, "0", typeof(string));
            outString += "ConvertTo result: " +  s + "\n";

            s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, "0:0:1", typeof(string));
            outString += "ConvertTo result: " +  s + "\n";

            s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, rb1, typeof(string));
            outString += "ConvertTo result: " +  s + "\n";

            s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, rb2, typeof(string));
            outString += "ConvertTo result: " +  s + "\n";

            s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, RepeatBehavior.Forever, typeof(string));
            outString += "ConvertTo result: " +  s + "\n";

            id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, rb1, typeof(InstanceDescriptor));
            outString += "ConvertTo result: " +  id.MemberInfo.ReflectedType + "\n";

            id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, rb2, typeof(InstanceDescriptor));
            outString += "ConvertTo result: " +  id.MemberInfo.ReflectedType + "\n";

            id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, RepeatBehavior.Forever, typeof(InstanceDescriptor));
            outString += "ConvertTo result: " +  id.MemberInfo.ReflectedType + "\n";

            try
            {
                s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, "abc", typeof(string));
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.FormatException), e.GetType(), ref outString );
            }

            try
            {
                s = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, 2, typeof(string));
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.FormatException), e.GetType(), ref outString );
            }

            try
            {
                id = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, "0:0:1", typeof(InstanceDescriptor));
            }
            catch (Exception e)
            {
                TimeGenericWrappers.CHECKEXCEPTION( typeof(System.NotSupportedException), e.GetType(), ref outString );
            }

            return outString;
        }
    }
}
