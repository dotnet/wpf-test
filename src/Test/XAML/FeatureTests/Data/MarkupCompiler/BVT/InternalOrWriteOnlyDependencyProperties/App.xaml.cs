// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace InternalOrWriteOnlyDependencyProperties
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Window1 window1 = new Window1();
                window1.Show();


            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Exception from Window1..ctor: " + exception.ToString());
                Application.Current.Shutdown(-1);
            }

            Application.Current.Shutdown(0);
        }
    }

    public class InternalAndWriteOnlyAttachedProps : DependencyObject
    {
        internal static readonly DependencyProperty FooProperty =
            DependencyProperty.Register("Foo", typeof(int), typeof(InternalAndWriteOnlyAttachedProps));

        internal int Foo
        {
            get { return (int)GetValue(FooProperty); }
            set { SetValue(FooProperty, value); }
        }

        public static readonly DependencyProperty BarProperty =
            DependencyProperty.RegisterAttached("Bar", typeof(int), typeof(InternalAndWriteOnlyAttachedProps));

        public static void SetBar(UIElement element, int value)
        {
            element.SetValue(BarProperty, value);
        }
    }

    public class IntProvider : MarkupExtension
    {
        public int Value { get; set; }
        public string ObjectName { get; set; }
        public string PropertyName { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            bool fail = false;
            IProvideValueTarget ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));


            string actualObjectValue = ipvt.TargetObject.ToString();
            string actualPropertyValue = ipvt.TargetProperty == null ? "null" : ipvt.TargetProperty.ToString();

            if (actualObjectValue != ObjectName)
            {
                GlobalLog.LogEvidence("Expected: " + ObjectName + " Actual: " + actualObjectValue);
                fail = true;
            }

            if (actualPropertyValue != PropertyName)
            {
                GlobalLog.LogEvidence("Expected: " + PropertyName + " Actual: " + actualPropertyValue);
                fail = true;
            }

            if (fail)
            {
                throw new Exception("Unexpected value returned from IProvideValueTarget");
            }

            return Value;
        }
    }
}
