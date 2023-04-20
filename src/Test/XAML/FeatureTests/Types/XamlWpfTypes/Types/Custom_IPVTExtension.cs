// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    /// <summary>
    /// Custom IPVTExtension
    /// </summary>
    public class Custom_IPVTExtension : MarkupExtension
    {
        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object value </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget ipvt = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));
            Custom_IPVTObject retObj = new Custom_IPVTObject();
            retObj.TargetObject = ipvt.TargetObject;
            retObj.TargetProperty = ipvt.TargetProperty;

            if (ipvt.TargetProperty is DependencyProperty)
            {
                retObj.TargetPropertyType = "DependencyProperty";
            }
            else if (ipvt.TargetProperty is MethodInfo) // For attached propeties, this code will not be hit until attached properties have been implemented outside of WPF
            {
                retObj.TargetPropertyType = "MethodInfo";
            }
            else if (ipvt.TargetProperty is PropertyInfo)
            {
                retObj.TargetPropertyType = "PropertyInfo";
            }
            else
            {
                retObj.TargetPropertyType = ipvt.TargetProperty.ToString();
            }

            return retObj;
        }
    }
}
