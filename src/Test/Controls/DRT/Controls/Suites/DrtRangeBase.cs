// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class DrtRangeBaseSuite : DrtTestSuite
    {
        public DrtRangeBaseSuite() : base("RangeBase")
        {
            Contact = "Microsoft";
        }

        private bool _valuechanged = false;

        public override DrtTest[] PrepareTests()
        {
            RangeBaseSubclass rb = new RangeBaseSubclass();

            // Verify default prop values
            DRT.Assert(rb.Value == 0.0,  "RangeBase Value should be 0.0 by default.");

            // Verify ValueChanged event
            rb.ValueChanged += new RoutedPropertyChangedEventHandler<double>(OnValueChanged);
            _valuechanged = false;
            rb.Value = 50.0;
            DRT.Assert(_valuechanged, "RangeBase ValueChanged event did not fire.");

            // Verify validation
            rb.Minimum = 0.0;
            rb.Maximum = 100.0;
            rb.Value = 1000.0;
            VerifyValues(rb, 100.0);

            rb.Maximum = 500;
            VerifyValues(rb, 500.0);

            rb.Minimum = 750;
            VerifyValues(rb, 750.0);

            rb.Maximum = 0;
            VerifyValues(rb, 750.0);

            rb.Maximum = 1000;
            VerifyValues(rb, 1000.0);

            rb.Maximum = 2000;
            VerifyValues(rb, 1000.0);
            
            Console.WriteLine("Reset RangeBase state (Value:0, Min:0, Max:10)");
            rb.Minimum = 0;
            rb.Maximum = 10;
            rb.Value = 0;
            VerifyValues(rb, 0,0,10);
            
            Console.WriteLine("Set Value=110");
            rb.Value = 110;
            VerifyValues(rb, 10,0,10);
            
            Console.WriteLine("Set Max=100");
            rb.Maximum = 100;
            VerifyValues(rb, 100,0,100);
            
            Console.WriteLine("Set Min=150");
            rb.Minimum = 150;
            VerifyValues(rb, 150,150,150);
            
            Console.WriteLine("Set Max=200");
            rb.Maximum = 200;
            VerifyValues(rb, 150, 150, 200);
            
            Console.WriteLine("Set Min=100");
            rb.Minimum = 100;
            VerifyValues(rb, 110,100,200);
            
            Console.WriteLine("Set Max=90");
            rb.Maximum = 90;
            VerifyValues(rb, 100,100,100);
            
            Console.WriteLine("Set Value=50");
            rb.Value = 50;
            VerifyValues(rb, 100,100,100);
            
            Console.WriteLine("Set Min=20");
            rb.Minimum = 20;
            VerifyValues(rb, 50,20,90);
            
            return new DrtTest[] {};
        }

        private void VerifyValues(RangeBase rb, double val, double min, double max)
        {
            DRT.Assert((rb.Value == val) && (rb.Minimum == min) && (rb.Maximum == max),
                String.Format("RangeBase values are not correct: Value:{0} Minimum:{1} Maximum:{2} (Expected:{3}, {4}, {5})",
                rb.Value, rb.Minimum, rb.Maximum, val,min,max));
        }
        
        private void VerifyValues(RangeBase rb, double expectedValue)
        {
            DRT.Assert((rb.Value == expectedValue 
                &&  rb.Value <= rb.Maximum
                &&  rb.Value >= rb.Minimum
                &&  rb.Minimum <= rb.Maximum),
                "RangeBase values are not correct: Value:"+rb.Value+" Minimum:"+rb.Minimum+" Maximum:"+rb.Maximum);
        }

        public void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Console.WriteLine("Got ValueChanged event. OldValue:"+e.OldValue+" NewValue:"+e.NewValue);
            _valuechanged = true;
        }


    }

    public class RangeBaseSubclass : RangeBase
    {
        public RangeBaseSubclass() : base() {}
    }
}

