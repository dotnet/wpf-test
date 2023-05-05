// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types.XamlTemplate
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xaml;
    using Microsoft.Test.Xaml.Common;
    using System.Reflection;

    public class XamlTemplateTypeConverter : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            var iProvideValueTarget = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;
            PropertyInfo pi = iProvideValueTarget.TargetProperty as PropertyInfo;
            Type type = pi.PropertyType;

            var loaderFactory = (IXamlObjectWriterFactory)serviceProvider.GetService(typeof(IXamlObjectWriterFactory));

            Type evaluationType = type.GetGenericArguments()[0];
            Type factoryType = typeof (XamlTemplateFactory<>).MakeGenericType(evaluationType);

            object factory = Activator.CreateInstance(factoryType, loaderFactory, xamlReader);

            if (typeof (Delegate).IsAssignableFrom(type))
            {
                if (type.GetGenericArguments().Count() == 1)
                {
                    return Delegate.CreateDelegate(type, factory, factoryType.GetMethod("Evaluate", new Type[]
                                                                                                        {
                                                                                                        }));
                }
                else if (type.GetGenericArguments().Count() == 2)
                {
                    return Delegate.CreateDelegate(type, factory, factoryType.GetMethod("Evaluate", new Type[]
                                                                                                        {
                                                                                                            type.GetGenericArguments()[1]
                                                                                                        }));
                }
                else
                {
                    throw new DataTestException("Unexpected return type from template.");
                }
            }
            else
            {
                return factory;
            }
        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            IXamlNodeContainer nodeContainer;
            if (value is Delegate)
            {
                nodeContainer = (IXamlNodeContainer) ((Delegate) value).Target;
            }
            else
            {
                nodeContainer = (IXamlNodeContainer) value;
            }

            return nodeContainer.Nodes.GetReader();
        }
    }
}
