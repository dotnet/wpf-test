// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.ComponentModel;
    using System.Xaml;
    using System.Reflection;

    public class XamlTemplateTypeConverterInterface : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var iProvideValueTarget = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;
            PropertyInfo pi = iProvideValueTarget.TargetProperty as PropertyInfo;
            Type type = pi.PropertyType;

            var loaderFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));

            Type evaluationType = type.GetGenericArguments()[0];
            Type factoryType = typeof (XamlTemplateInterfaceImpl<>).MakeGenericType(evaluationType);

            return Activator.CreateInstance(factoryType, loaderFactory, xamlReader);
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            return ((value as IXamlNodeContainer).Nodes).GetReader();
        }
    }
}
