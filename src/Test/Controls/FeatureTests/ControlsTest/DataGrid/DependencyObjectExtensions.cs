using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Extension methods for DependencyObject
    /// </summary>
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<DependencyProperty> GetAttachedProperties(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }
            return from pd in TypeDescriptor.GetProperties(dependencyObject,
                       new Attribute[] { 
                           new PropertyFilterAttribute(PropertyFilterOptions.SetValues | 
                           PropertyFilterOptions.UnsetValues | 
                           PropertyFilterOptions.Valid) 
                       }).OfType<PropertyDescriptor>()
                   let dpd = DependencyPropertyDescriptor.FromProperty(pd)
                   where dpd != null && dpd.IsAttached
                   select dpd.DependencyProperty;
        }

        public static void CopyAttachedPropertiesTo(this DependencyObject srcObject, DependencyObject destObject)
        {
            if (srcObject == null) { throw new ArgumentNullException("srcObject"); }
            if (destObject == null) { throw new ArgumentNullException("destObject"); }
            foreach (DependencyProperty property in srcObject.GetAttachedProperties())
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(property, srcObject.GetType());
                if (dpd != null && dpd.IsAttached)
                {
                    destObject.SetValue(dpd.DependencyProperty, dpd.GetValue(srcObject));
                }
            }
        }

        public static IEnumerable<DependencyProperty> GetDependencyProperties(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }
            return from pd in TypeDescriptor.GetProperties(dependencyObject,
                       new Attribute[] { 
                           new PropertyFilterAttribute(PropertyFilterOptions.SetValues | 
                               PropertyFilterOptions.UnsetValues | 
                               PropertyFilterOptions.Valid) 
                       }).OfType<PropertyDescriptor>()
                   let dpd = DependencyPropertyDescriptor.FromProperty(pd)
                   where dpd != null
                   select dpd.DependencyProperty;
        }

        public static IEnumerable<Binding> GetAllBindings(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            LocalValueEnumerator lve = dependencyObject.GetLocalValueEnumerator();
            while (lve.MoveNext())
            {
                LocalValueEntry entry = lve.Current;
                if (BindingOperations.IsDataBound(dependencyObject, entry.Property))
                {
                    Binding binding = (entry.Value as BindingExpression).ParentBinding;
                    yield return binding;
                }
            }
        }
    }
}
