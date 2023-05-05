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
    
    public class XamlHostBase
    {
        public XamlHostBase()
        {
            IntData = 42;
            ClassData = new ClassType2
                            {
                                Category = new ClassType1
                                               {
                                                   Category = "Some category"
                                               }
                            };
        }

        public int IntData { get; set; }
        public ClassType2 ClassData { get; set; }

        public static object GetTemplateValue(object target)
        {
            PropertyInfo templateProp = target.GetType().GetProperty("Template");
            object template = templateProp.GetValue(target, new object[]
                                                                {
                                                                });

            if (typeof (Delegate).IsAssignableFrom(template.GetType()))
            {
                return ((Delegate) template).DynamicInvoke();
            }
            else
            {
                MethodInfo evaluateMethod = template.GetType().GetMethod("Evaluate", new Type[]
                                                                                         {
                                                                                         });
                return evaluateMethod.Invoke(template, new object[]
                                                           {
                                                           });
            }
        }
    }

    public class FuncHost<T> : XamlHostBase
    {
        [System.Windows.Markup.XamlDeferLoad(typeof (XamlTemplateTypeConverter), typeof (object))]
        public Func<T> Template { get; set; }
    }

    public class FactoryHost<T> : XamlHostBase
    {
        [System.Windows.Markup.XamlDeferLoad(typeof (XamlTemplateTypeConverter), typeof (object))]
        public XamlTemplateFactory<T> Template { get; set; }
    }

    // should work exactly the same as FactoryHost<>, content type is only used for tooling
    public class FactoryHostWContentType<T> : XamlHostBase
    {
        [System.Windows.Markup.XamlDeferLoad(typeof (XamlTemplateTypeConverter), typeof (ClassType3))]
        public XamlTemplateFactory<T> Template { get; set; }
    }

    public class ClassHost<T> : XamlHostBase
    {
        public XamlTemplateClass<T> Template { get; set; }
    }

    public class OverridenClassHost<T> : XamlHostBase
    {
        [System.Windows.Markup.XamlDeferLoad(typeof (InteractionConverterClass), typeof (object))]
        public XamlTemplateClass<T> Template { get; set; }
    }

    public class ListClassHost<T> : XamlHostBase
    {
        public ListClassHost()
        {
            Templates = new List<XamlTemplateClass<T>>();
        }

        public List<XamlTemplateClass<T>> Templates { get; private set; }
    }

    public class DerivedClassHost<T> : XamlHostBase
    {
        public XamlTemplateDerivedClass<T> Template { get; set; }
    }

    public class StructHost<T> : XamlHostBase
    {
        public XamlTemplateStruct<T> Template { get; set; }
    }

    public class InterfaceHost<T> : XamlHostBase
    {
        public IXamlTemplateInterface<T> Template { get; set; }
    }

    public delegate T XamlDelegateTemplate<T>();

    public delegate T XamlDelegateTemplateWContext<T>(object context);

    public class DelegateHost<T> : XamlHostBase
    {
        public XamlDelegateTemplate<T> Template { get; set; }
    }

    public class PointInstanceDescriptorWrapper : XamlHostBase
    {
        [System.Windows.Markup.XamlDeferLoad(typeof (PointXamlTemplateTypeConverterBad), typeof (object))]
        public PointWithInstanceDescriptor Point { get; set; }
    }

    public class PointXamlTemplateWrapper : XamlHostBase
    {
        [TypeConverter(typeof (PointInstanceDescriptorTypeConverterBad))]
        public PointWithXamlTemplate Point { get; set; }
    }
}
