using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Avalon.Test.ComponentModel.Utilities
{
    public static class ValueGetter
    {
        private class BindingHelper : DependencyObject
        {
            public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(BindingHelper),
               new PropertyMetadata((object)null));

            public object Target
            {
                get
                {
                    return GetValue(TargetProperty);
                }
                set
                {
                    SetValue(TargetProperty, value);
                }
            }
        }

        public static object GetValue(object item, PropertyPath path)
        {
            Binding b = new Binding();
            b.Path = path;
            b.Source = item;
            BindingHelper bh = new BindingHelper();
            BindingOperations.SetBinding(bh, BindingHelper.TargetProperty, b);
            try
            {
                return bh.Target;
            }
            finally
            {
                BindingOperations.ClearBinding(bh, BindingHelper.TargetProperty);
            }
        }
    }
}
