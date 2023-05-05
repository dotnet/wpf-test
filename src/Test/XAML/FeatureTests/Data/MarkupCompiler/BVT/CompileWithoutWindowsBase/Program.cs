// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test;

namespace CompileWithoutWindowsBase
{
    public static class Program 
    {
        /// <summary>
        /// This test makes sure that you can compile using types that 
        /// are forwarded from windows base without having a reference to 
        /// windows base.
        /// </summary>
        public static void Main()
        {
	   
            Console.WriteLine(new CustomValueSerializer());
            Console.WriteLine(new ArrayExtension());
            Console.WriteLine(new DateTimeValueSerializer());
            Console.WriteLine(new CustomComponentConnector());
            Console.WriteLine(new CustomNameScope());
            Console.WriteLine(new CustomProvideValueTarget());
            Console.WriteLine(new CustomUriContext());
            Console.WriteLine(new CustomValueSerializerContext());
            Console.WriteLine(new CustomXamlTypeResolver());
            Console.WriteLine(new CustomMarkupExtension());
            Console.WriteLine(new NullExtension());
            Console.WriteLine(new StaticExtension());
            Console.WriteLine(new TypeExtension());
            Console.WriteLine(new AmbientAttribute());
            Console.WriteLine(new UsableDuringInitializationAttribute(true));
            Console.WriteLine(new ConstructorArgumentAttribute("FooBar"));
            Console.WriteLine(new ContentPropertyAttribute("Content"));
            Console.WriteLine(new ContentWrapperAttribute(typeof(int)));
            Console.WriteLine(new DependsOnAttribute("depend"));
            Console.WriteLine(new DictionaryKeyPropertyAttribute("key"));
            Console.WriteLine(new MarkupExtensionReturnTypeAttribute());
            Console.WriteLine(new NameScopePropertyAttribute("foo", typeof(int)));
            Console.WriteLine(new RootNamespaceAttribute("foons"));
            Console.WriteLine(new TrimSurroundingWhitespaceAttribute());
            Console.WriteLine(new UidPropertyAttribute("name"));
            Console.WriteLine(new ValueSerializerAttribute("foobar"));
            Console.WriteLine(new WhitespaceSignificantCollectionAttribute());
            Console.WriteLine(new XmlLangPropertyAttribute("name"));
            Console.WriteLine(new XmlnsCompatibleWithAttribute("old","new"));
            Console.WriteLine(new XmlnsDefinitionAttribute("xmlnns", "clrns"));
            Console.WriteLine(new XmlnsPrefixAttribute("xmlns", "prefix"));
            Console.WriteLine(new RuntimeNamePropertyAttribute("name"));   
                     
        }
    }

    public class CustomValueSerializer : ValueSerializer { }
    
    public class CustomComponentConnector : IComponentConnector
    {
        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }

        public void InitializeComponent()
        {
            throw new NotImplementedException();
        }
    }

    public class CustomNameScope : INameScope
    {

        public object FindName(string name)
        {
            throw new NotImplementedException();
        }

        public void RegisterName(string name, object scopedElement)
        {
            throw new NotImplementedException();
        }

        public void UnregisterName(string name)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomProvideValueTarget : IProvideValueTarget
    {
        public object TargetObject
        {
            get { throw new NotImplementedException(); }
        }

        public object TargetProperty
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class CustomUriContext : IUriContext
    {
        public Uri BaseUri
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public class CustomValueSerializerContext : IValueSerializerContext
    {
        public ValueSerializer GetValueSerializerFor(System.ComponentModel.PropertyDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        public ValueSerializer GetValueSerializerFor(Type type)
        {
            throw new NotImplementedException();
        }

        public System.ComponentModel.IContainer Container
        {
            get { throw new NotImplementedException(); }
        }

        public object Instance
        {
            get { throw new NotImplementedException(); }
        }

        public void OnComponentChanged()
        {
            throw new NotImplementedException();
        }

        public bool OnComponentChanging()
        {
            throw new NotImplementedException();
        }

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor
        {
            get { throw new NotImplementedException(); }
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomXamlTypeResolver : IXamlTypeResolver
    {
        public Type Resolve(string qualifiedTypeName)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomMarkupExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
    }




}
