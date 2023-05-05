// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;
using System.Xaml;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace1;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace2;
using Microsoft.Test.Xaml.Types.ObjectReader.Namespace3;
using System.Collections;

namespace Microsoft.Test.Xaml.Types.ObjectReader
{
    public class ROCollectionContainer
    {
        private readonly List<string> _roCollection = new List<string>();
        public List<string> ROCollection
        {
            get
            {
                return _roCollection;
            }
        }

        private readonly Dictionary<string, string> _roDictionary = new Dictionary<string, string>();
        public Dictionary<string, string> RODictionary
        {
            get
            {
                return _roDictionary;
            }
        }

        private string[] _roArray = new string[2];
        public string[] ROArray
        {
            get { return _roArray; }
        }
    }

    public class RWCollectionContainer
    {
        public List<string> RWCollection { get; set; }
        public Dictionary<string, string> RWDictionary { get; set; }
        public string[] RWArray { get; set; }
    }

    public class Container
    {
        public object child1 { get; set; }
        public object child2 { get; set; }
    }

    public class Element { }

    public class HoldsOneElement
    {
        public Element Element { get; set; }
    }

    public class NameReferencedHoldsTwoElements
    {
        public Element One
        {
            get;
            set;
        }

        [TypeConverter(typeof(NameReferenceConverter))]
        public Element Two
        {
            get;
            set;
        }
    }

    public class NameElement : Element
    {
        [TypeConverter(typeof(HoldsOneElementConverter))]
        public HoldsOneElement Container { get; set; }
    }

    [RuntimeNameProperty("Name")]
    public class NameElementWithRuntimeName : Element
    {
        public string Name { get; set; }

        [TypeConverter(typeof(HoldsOneElementConverter))]
        public HoldsOneElement Container { get; set; }
    }

    public class HoldsOneElementConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var container = value as HoldsOneElement;
            var service = context.GetService(typeof(IXamlNameProvider)) as IXamlNameProvider;
            string name = service.GetName(container.Element);
            return name;
        }
    }

    [ContentProperty("Content")]
    public class NameScope : Element, INameScope
    {
        Dictionary<string, object> _names = new Dictionary<string, object>();

        public object Content
        { get; set; }

        [DefaultValue(0)]
        public int RandomProp
        { get; set; }

        public object FindName(string name)
        {
            object value;
            if (_names.TryGetValue(name, out value))
            {
                return value;
            }
            return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            _names[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            _names.Remove(name);
        }
    }

    public class DictionaryContainer
    {
        private Dictionary<string, int> _dict;
        public Dictionary<string, int> Dict 
        {
            get
            {
                if (this._dict == null)
                {
                    this._dict = new Dictionary<string, int>();
                }
                return this._dict;
            }

            set
            {
                this._dict = value;
            }
        }
    }

    public class BadGetter
    {
        public string Foo
        {
            get { throw new ArgumentException(); }
            set { throw new ArgumentException(); }
        }
    }

    public class DerivedDictionaryContainer
    {
        public DictionaryAsContent ContentProperty { get; set; }
        public DictionaryAsProperty NormalProperty { get; set; }
    }

    public class DoubleDerivedDictionaryContainer
    {
        public DictionaryAsProperty NormalProperty1 { get; set; }
        public DictionaryAsProperty NormalProperty2 { get; set; }
    }

    [ContentProperty("TheContent")]
    public class DictionaryAsContent
    {
        private DerivedDictionary _content;
        public DerivedDictionary TheContent
        {
            get
            {
                if (_content == null) _content = new DerivedDictionary();
                return _content;
            }
            set
            {
                _content = value;
            }
        }

    }

    public class DictionaryAsProperty
    {
        private DerivedDictionary _property;
        public DerivedDictionary TheProperty
        {
            get
            {
                if (_property == null) _property = new DerivedDictionary();
                return _property;
            }
            set
            {
                _property = value;
            }
        }
    }

    [RuntimeNameProperty("Name")]
    public class DerivedDictionary : Dictionary<string, string>
    {
        public string Name { get; set; }
        public DerivedDictionary()
        {
        }
    }

    public class NameScopeArrayList : ArrayList, INameScope
    {
        NameScope _ns = new NameScope();

        void INameScope.RegisterName(string s, object o)
        {
            _ns.RegisterName(s, o);
        }

        object INameScope.FindName(string s)
        {
            return _ns.FindName(s);
        }

        void INameScope.UnregisterName(string s)
        {
            _ns.UnregisterName(s);
        }
    }

    public class NameScopeArrayListHolder : INameScope
    {
        NameScope _ns = new NameScope();

        NameScopeArrayList _list = new NameScopeArrayList();

        /// <summary>
        /// The value returned is an instance of NameScopeArrayList which
        /// implements INameScope.
        /// </summary>
        public ArrayList ArrayList { get { return _list; } }

        public NameScopeArrayList NameScopeArrayList { get { return _list; } }

        void INameScope.RegisterName(string s, object o)
        {
            _ns.RegisterName(s, o);
        }

        object INameScope.FindName(string s)
        {
            return _ns.FindName(s);
        }

        void INameScope.UnregisterName(string s)
        {
            _ns.UnregisterName(s);
        }
    }

}

namespace Microsoft.Test.Xaml.Types.ObjectReader.Namespace1
{
    public class Ns1Type
    {
        public object child { get; set; }

        [TypeConverter(typeof (MyXNameTypeConverter))]
        public object XName { get; set; }

        [TypeConverter(typeof (MyXNameTypeConverter))]
        [ValueSerializer(typeof (MyXNameValueSerializer))]
        public object XNameVS { get; set; }
    }

    public class Ns1GenericType<T>
    {
        public T child { get; set; }
    }

    [TypeConverter(typeof (SimpleMEConverter))]
    public class SimpleMEClass
    {
        public SimpleMEClass()
        {
        }

        public object Data { get; set; }
    }

    public class SimpleMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof (MarkupExtension))
            {
                throw new ArgumentException("destinationType");
            }

            SimpleMEClass simpleMEClass = value as SimpleMEClass;

            if (null == simpleMEClass)
            {
                throw new ArgumentException("simpleMEClass");
            }

            SimpleME extension = new SimpleME();
            extension.Data = simpleMEClass.Data;

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    [TypeConverter(typeof (SomeTypeTypeConverter))]
    public class SomeType
    {
        public SomeType() { }
        public object Data { get; set; }
    }

    public class MyXNameTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            XName name = value as XName;
            if (destinationType == typeof (string))
            {
                INamespacePrefixLookup prefixLookupService = (INamespacePrefixLookup) context.GetService(typeof (INamespacePrefixLookup));
                string prefix = prefixLookupService.LookupPrefix(name.Namespace.NamespaceName);
                if (prefix == string.Empty)
                {
                    return name.LocalName;
                }
                else
                {
                    return prefix + ":" + name.LocalName;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class MyXNameValueSerializer : ValueSerializer
    {
        public override Object ConvertFromString(string value, IValueSerializerContext context)
        {
            throw new NotImplementedException();
        }

        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            XName name = value as XName;
            INamespacePrefixLookup prefixLookupService = (INamespacePrefixLookup) context.GetService(typeof (INamespacePrefixLookup));
            string prefix = prefixLookupService.LookupPrefix(name.Namespace.NamespaceName);
            if (prefix == string.Empty)
            {
                return name.LocalName;
            }
            else
            {
                return prefix + ":" + name.LocalName;
            }
        }

        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            if (value is XName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }
}

namespace Microsoft.Test.Xaml.Types.ObjectReader.Namespace2
{
    public class Ns2Type
    {
        public object child { get; set; }
    }

    public class SimpleME : MarkupExtension
    {
        public SimpleME()
        {
        }

        public SimpleME(string data)
        {
            this.Data = data;
        }

        public object Data { get; set; }
        private SimpleMEClass _simpleMEClass;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEClass == null)
            {
                _simpleMEClass = new SimpleMEClass();
                _simpleMEClass.Data = this.Data;
            }
            return _simpleMEClass;
        }
    }

    public class SomeTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof (InstanceDescriptor) && value is SomeType)
            {
                SomeType someType = (SomeType) value;

                MemberInfo mi = typeof (Ns3Type).GetMember("GetSomeType")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              someType.Data
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

namespace Microsoft.Test.Xaml.Types.ObjectReader.Namespace3
{
    public class Ns3Type
    {
        public object child { get; set; }

        public static SomeType GetSomeType(object data)
        {
            return new SomeType()
                       {
                           Data = data
                       };
        }
    }
}
