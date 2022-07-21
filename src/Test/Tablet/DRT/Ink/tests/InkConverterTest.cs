// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Runtime.Serialization.Formatters;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Threading;

namespace DRT
{
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class InkConverterTest : DrtInkTestcase
	{   
        /// <summary>
        /// Our test method
        /// </summary>
        public override void Run()
        {
            StylusPointCollection stylusPoints1 = new StylusPointCollection();
            StylusPointCollection stylusPoints2 = new StylusPointCollection();

            for (int x = 0; x < 100; x++)
            {
                StylusPoint p1 = new StylusPoint((double)x + 200, (double)x + 400);
                StylusPoint p2 = new StylusPoint((double)x + 400, (double)x + 200);

                stylusPoints1.Add(p1);
                stylusPoints2.Add(p2);
            }
            StrokeCollection ink = new StrokeCollection();
            ink.Add(new Stroke(stylusPoints1));
            ink.Add(new Stroke(stylusPoints2));

            if (2 != ink.Count)
            {
                throw new InvalidOperationException("Failed to create two strokes in an ink object");
            }
            
            //convert to a string
            string base64 = (string)TypeDescriptor.GetConverter(typeof(StrokeCollection)).ConvertToString(ink);
            
            //and convert back to a new ink object
            StrokeCollection ink2 = (StrokeCollection)TypeDescriptor.GetConverter(typeof(StrokeCollection)).ConvertFromString(base64);
            
            if (2 != ink2.Count)
            {
                throw new InvalidOperationException("Failed to convert a new ink object with two ink strokes");
            }

            //now convert to an instance descriptor
            InstanceDescriptor descriptor = 
                (InstanceDescriptor)TypeDescriptor.GetConverter(typeof(StrokeCollection)).ConvertTo(ink2, typeof(InstanceDescriptor));

            StrokeCollection ink6 = (StrokeCollection)descriptor.Invoke();
            if (null == ink6)
            {
                throw new InvalidOperationException("Failed to reconstitue and ink object from an InstanceDescriptor");
            }

            if (2 != ink6.Count)
            {
                throw new InvalidOperationException("Failed to reconstitue a new ink object with two ink strokes from an InstanceDescriptor");
            }

            //test to make sure you can create an Ink object from an empty string
            StrokeCollection ink3 = (StrokeCollection)TypeDescriptor.GetConverter(typeof(StrokeCollection)).ConvertFromString("");
            if (null == ink3)
            {
                throw new InvalidOperationException("Failed to convert a new ink object from a '' string");
            }

            StrokeCollection ink4 = (StrokeCollection)TypeDescriptor.GetConverter(typeof(StrokeCollection)).ConvertFromString(String.Empty);
            if (null == ink4)
            {
                throw new InvalidOperationException("Failed to convert a new ink object from a String.Empty string");
            }

            Success = true;
        }
	}
}
