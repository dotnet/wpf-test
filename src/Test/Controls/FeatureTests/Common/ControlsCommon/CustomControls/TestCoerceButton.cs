using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Avalon.Test.ComponentModel
{
    public class TestCoerceButton : Button
    {
        public static readonly DependencyProperty TestCoerceProperty
            = DependencyProperty.RegisterAttached("TestCoerce", typeof(int), typeof(TestCoerceButton),
            new FrameworkPropertyMetadata(0,
                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits,
                new PropertyChangedCallback(OnTestCoerceChanged),
                new CoerceValueCallback(CoerceTestCoerce)),
                new ValidateValueCallback(IsValidIntValue));

        public int TestCoerce
        {
            get { return (int)GetValue(TestCoerceProperty); }
            set { SetValue(TestCoerceProperty, value); }
        }

        private static void OnTestCoerceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceTestCoerce(DependencyObject d, object value)
        {
            if ((int)value > 50)
            {
                return 50;
            }
            else
            {
                return value;
            }
        }

        private static bool IsValidIntValue(object value)
        {
            return true;
        }

    }
}
