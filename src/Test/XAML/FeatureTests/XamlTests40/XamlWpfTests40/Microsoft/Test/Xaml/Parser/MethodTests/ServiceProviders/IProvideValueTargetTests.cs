// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;

namespace Microsoft.Test.Xaml.Parser.MethodTests.ServiceProviders
{
    /// <summary>
    /// Tests for IProvideValueTarget service provider
    /// </summary>
    public class IProvideValueTargetTests
    {
        /// <summary>
        /// IProvideValueTarget.TargetProperty should return the property if it is a regular property
        /// </summary>
        public void TargetProperty_RegularProperty()
        {
            string xaml = @"
<IPVTContainer 
    xmlns=""clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.ServiceProviders;assembly=XamlWpfTests40"">
    <IPVTContainer.Setter>
        <TargetPropertyExtension />
    </IPVTContainer.Setter>
</IPVTContainer>";

            IPVTContainer container = XamlServices.Load(XmlReader.Create(new StringReader(xaml))) as IPVTContainer;

            if (container.Setter != "Setter")
            {
                throw new Exception("IProvideValueTarget.TargetProperty did not return the Setter method");
            }
        }

        /// <summary>
        /// IProvideValueTarget.TargetProperty should return the Set method if it is an attached property
        /// </summary>
        public void TargetProperty_AttachedProperty()
        {
            string xaml = @"
<IPVTContainer 
    xmlns=""clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.ServiceProviders;assembly=XamlWpfTests40"">
    <TargetPropertyHolder.Setter>
        <TargetPropertyExtension />
    </TargetPropertyHolder.Setter>
</IPVTContainer>";

            IPVTContainer container = XamlServices.Load(XmlReader.Create(new StringReader(xaml))) as IPVTContainer;

            if (TargetPropertyHolder.GetSetter(container) != "SetSetter")
            {
                throw new Exception("IProvideValueTarget.TargetProperty did not return the Setter method for an attached property");
            }
        }

        /// <summary>
        /// IProvideValueTarget.TargetProperty should return null if it is a collection
        /// </summary>
        public void TargetProperty_CollectionItem()
        {
            string xaml = @"
<IPVTContainer 
    xmlns:sc=""clr-namespace:System.Collections;assembly=mscorlib"" 
    xmlns=""clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.ServiceProviders;assembly=XamlWpfTests40"">
    <IPVTContainer.List>
        <sc:ArrayList>
            <TargetPropertyExtension />
        </sc:ArrayList>
    </IPVTContainer.List>
</IPVTContainer>";

            IPVTContainer container = XamlServices.Load(XmlReader.Create(new StringReader(xaml))) as IPVTContainer;

            if (container.List.Count != 1 || container.List[0].ToString() != "null")
            {
                throw new Exception("IProvideValueTarget.TargetProperty did not return null for an item in a collection");
            }
        }

        /// <summary>
        /// IProvideValueTarget.TargetProperty should return null if it is a dictionary
        /// </summary>
        public void TargetProperty_DictionaryItem()
        {
            string xaml = @"
<sc:Hashtable 
    xmlns:sc=""clr-namespace:System.Collections;assembly=mscorlib"" 
    xmlns=""clr-namespace:Microsoft.Test.Xaml.Parser.MethodTests.ServiceProviders;assembly=XamlWpfTests40"" 
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <TargetPropertyExtension x:Key=""foo"" />
</sc:Hashtable>";

            Hashtable table = XamlServices.Load(XmlReader.Create(new StringReader(xaml))) as Hashtable as Hashtable;

            if (table.Count != 1 || table["foo"].ToString() != "null")
            {
                throw new Exception("IProvideValueTarget.TargetProperty did not return null for an item in a dictionary");
            }
        }
    }

    /// <summary>
    /// IProvideValueTarget container
    /// </summary>
    public class IPVTContainer
    {
        /// <summary>
        /// Gets or sets Setter property
        /// </summary>
        public string Setter { get; set; }

        /// <summary>
        /// Gets or sets List property
        /// </summary>
        public ArrayList List { get; set; }
    }

    /// <summary>
    /// Type with attachable property
    /// </summary>
    public class TargetPropertyHolder
    {
        /// <summary>
        /// Dictionary object to hold the attached properties
        /// </summary>
        private static Dictionary<object, string> s_slots = new Dictionary<object, string>();

        /// <summary>
        /// Synchronization object
        /// </summary>
        private static object s_syncObj = new object();

        /// <summary>
        /// Attachable property setter
        /// </summary>
        /// <param name="target">Attachable property target</param>
        /// <param name="value">Attachable property value</param>
        public static void SetSetter(object target, string value)
        {
            lock (s_syncObj)
            {
                string storedValue;

                if (s_slots.TryGetValue(target, out storedValue))
                {
                    s_slots[target] = value;
                }
                else
                {
                    s_slots.Add(target, value);
                }
            }
        }

        /// <summary>
        /// Attachable property getter
        /// </summary>
        /// <param name="target">Attachable property target</param>
        /// <returns>Attachable property value</returns>
        public static string GetSetter(object target)
        {
            string storedValue;

            lock (s_syncObj)
            {
                if (s_slots.TryGetValue(target, out storedValue))
                {
                    return storedValue;
                }
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Markup extension that uses IProvideValueTarget
    /// </summary>
    public class TargetPropertyExtension : MarkupExtension
    {
        /// <summary>
        /// ProvideValue method of markup extension
        /// </summary>
        /// <param name="serviceProvider">Service provider object</param>
        /// <returns>Return value</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget ipvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (ipvt != null)
            {
                object targetProperty = ipvt.TargetProperty;

                if (targetProperty is MethodInfo)
                {
                    return ((MethodInfo)targetProperty).Name;
                }
                else if (targetProperty is PropertyInfo)
                {
                    return ((PropertyInfo)targetProperty).Name;
                }
            }

            return "null";
        }
    }
}
