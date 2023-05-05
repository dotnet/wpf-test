// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xaml;

    public class XamlTemplateTypeConverterStruct : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var iProvideValueTarget = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;
            PropertyInfo pi = iProvideValueTarget.TargetProperty as PropertyInfo;
            Type type = pi.PropertyType;

            var loaderFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));

            Type evaluationType = type.GetGenericArguments()[0];
            Type factoryType = typeof (XamlTemplateStruct<>).MakeGenericType(evaluationType);

            return Activator.CreateInstance(factoryType, loaderFactory, xamlReader);
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            PropertyInfo getNodes = value.GetType().GetProperty("Nodes");
            return ((XamlNodeList) getNodes.GetValue(value, new object[]
                                                                {
                                                                })).GetReader();
        }
    }
}
