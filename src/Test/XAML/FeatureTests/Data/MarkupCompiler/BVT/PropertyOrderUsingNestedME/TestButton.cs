// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace PropertyOrderUsingNestedME
{
    public class TestButton : Button
    {

        public TestButton()
        {
            Order = String.Empty;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.Property.Name)
            {
                case "NonSharableProp":
                    Order += "NonSharableProp" + ", ";
                    AllOrder += "NonSharableProp" + ", ";
                    break;
                case "ShareableProp":
                    Order += "SharableProp" + ", ";
                    AllOrder += "SharableProp" + ", ";
                    break;
                case "PropInQuestion":
                    Order += "PropInQuestion" + ", ";
                    AllOrder += "PropInQuestion" + ", ";
                    break;
            }
        }

        /// <summary>
        /// Order in which properties are set on the instance.  This is useful for debugging
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// Order in which properties are set for all instances of this class.
        /// This is used for test case verification.
        /// </summary>
        public static string AllOrder { get; set; }

        public object NonSharableProp
        {
            get { return (object)GetValue(NonSharablePropProperty); }
            set { SetValue(NonSharablePropProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonSharableProp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonSharablePropProperty =
            DependencyProperty.Register("NonSharableProp", typeof(object), typeof(TestButton));

        public int ShareableProp
        {
            get { return (int)GetValue(ShareablePropProperty); }
            set { SetValue(ShareablePropProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShareableProp.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShareablePropProperty =
            DependencyProperty.Register("ShareableProp", typeof(int), typeof(TestButton), new UIPropertyMetadata(0));

        public Object PropInQuestion
        {
            get { return (Object)GetValue(PropInQuestionProperty); }
            set { SetValue(PropInQuestionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropInQuestion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropInQuestionProperty =
            DependencyProperty.Register("PropInQuestion", typeof(Object), typeof(TestButton));
    }
}
