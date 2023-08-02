using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    public class ControlWithAttachedProperty : Control
    {
        public static readonly DependencyProperty CustomAttachedProperty = DependencyProperty.RegisterAttached(
            "CustomAttached", typeof(string), typeof(ControlWithAttachedProperty), null);

        public static void SetCustomAttached(Control target, string value)
        {
            target.SetValue(CustomAttachedProperty, value);
        }
        public static string GetCustomAttached(Control target)
        {
            return target.GetValue(CustomAttachedProperty) as string;
        }
    }

    public class CustomControl : Control
    {
        public static readonly DependencyProperty WorkingTagProperty = DependencyProperty.Register(
            "WorkingTag", typeof(object), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomStringProperty = DependencyProperty.Register(
            "CustomString", typeof(string), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomIntProperty = DependencyProperty.Register(
            "CustomInt", typeof(int), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomSizeProperty = DependencyProperty.Register(
            "CustomSize", typeof(Size), typeof(CustomControl), null);

        public static readonly DependencyProperty CustomThicknessProperty = DependencyProperty.Register(
            "CustomThickness", typeof(Thickness), typeof(CustomControl), null);

        public object WorkingTag
        {
            get { return GetValue(WorkingTagProperty); }
            set { SetValue(WorkingTagProperty, value); }
        }

        public string CustomString
        {
            get { return (string)GetValue(CustomStringProperty); }
            set { SetValue(CustomStringProperty, value); }
        }

        public int CustomInt
        {
            get { return (int)GetValue(CustomIntProperty); }
            set { SetValue(CustomIntProperty, value); }
        }

        public Size CustomSize
        {
            get { return (Size)GetValue(CustomSizeProperty); }
            set { SetValue(CustomSizeProperty, value); }
        }

        public Thickness CustomThickness
        {
            get { return (Thickness)GetValue(CustomThicknessProperty); }
            set { SetValue(CustomThicknessProperty, value); }
        }

        public double NotDp { get; set; }

        public DependencyObject PublicGetTemplateChild(string name)
        {
            return GetTemplateChild(name);
        }
    }
}
