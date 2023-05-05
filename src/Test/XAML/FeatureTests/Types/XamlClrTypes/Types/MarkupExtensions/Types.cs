// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Windows.Markup;
using Microsoft.Test.Xaml.Common;
using Microsoft.Test.Xaml.Types.MarkupExtensions;
using Microsoft.Test.Xaml.Types.MarkupExtensions2;
using Microsoft.Test.Xaml.Types.MarkupExtensions.Ns1;
using Microsoft.Test.Xaml.Driver;

namespace Microsoft.Test.Xaml.Types.MarkupExtensions
{
    #region SimpleMarkupExtensionConverter

    public class SimpleMEClassContainer
    {
        public SimpleMEClass SimpleMEClass { get; set; }

        #region Test Implementation
        public static List<TestCaseInfo> GetTestCases()
        {
            SimpleMEClassContainer target = new SimpleMEClassContainer()
            {
                SimpleMEClass = new SimpleMEClass()
                {
                    Address = "city1",
                    AptNo = 210
                }
            };

            List<TestCaseInfo> testCases = new List<TestCaseInfo>
            {
                new TestCaseInfo
                {
                    Target = target,
                    XPathExpresions = 
                    {
                         @"/xx:SimpleMEClassContainer[@SimpleMEClass=""{SimpleME Address=city1, AptNo=210}""]"
                    },
                    TestID = "SimpleMarkupExtension" + 0,
                   
                },
            };

            return testCases;
        }
        #endregion
    }

    [TypeConverter(typeof(SimpleMEConverter))]
    public class SimpleMEClass
    {
        private string _streetAddress;
        private int _aptNo;

        public SimpleMEClass()
        {
        }

        public SimpleMEClass(string address)
        {
            this._streetAddress = address;
        }

        [ConstructorArgument("address")]
        public string Address
        {
            get
            {
                return this._streetAddress;
            }
            set
            {
                this._streetAddress = value;
            }
        }

        public int AptNo
        {
            get
            {
                return this._aptNo;
            }
            set
            {
                this._aptNo = value;
            }
        }
    }

    public class SimpleMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleMEClass simpleMEClass = value as SimpleMEClass;

            if (null == simpleMEClass)
            {
                throw new ArgumentException();
            }

            SimpleME extension = new SimpleME();
            extension.Address = simpleMEClass.Address;
            extension.AptNo = simpleMEClass.AptNo;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class SimpleME : MarkupExtension
    {
        private string _address;
        private int _apt;

        private SimpleMEClass _simpleMEClass;

        public SimpleME()
        {
        }

        public SimpleME(string foo)
        {
            this._address = foo;
        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }
        public int AptNo
        {
            get
            {
                return this._apt;
            }
            set
            {
                this._apt = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEClass == null)
            {
                _simpleMEClass = new SimpleMEClass();
                _simpleMEClass.Address = this._address;
                _simpleMEClass.AptNo = this._apt;
            }

            return _simpleMEClass;
        }
    }



    #endregion

    #region SimpleME in different namespace

    public class SimpleMEClassContainerDiffNS
    {
        public SimpleMEClassDiffNS SimpleMEClass { get; set; }
    }

    public class SimpleMEConverterDiffNS : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleMEClassDiffNS simpleMEClass = value as SimpleMEClassDiffNS;

            if (null == simpleMEClass)
            {
                throw new ArgumentException();
            }

            SimpleMEDiffNS extension = new SimpleMEDiffNS();
            extension.Address = simpleMEClass.Address;
            extension.AptNo = simpleMEClass.AptNo;

            return extension;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class SimpleMEDiffNS : MarkupExtension
    {
        private SimpleMEClassDiffNS _simpleMEClass;

        public SimpleMEDiffNS()
        {
        }

        public string Address { get; set; }

        public int AptNo { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEClass == null)
            {
                _simpleMEClass = new SimpleMEClassDiffNS();
                _simpleMEClass.Address = this.Address;
                _simpleMEClass.AptNo = this.AptNo;
            }

            return _simpleMEClass;
        }
    }

    #endregion

    #region SimpleMarkupExtensionConverter Non Attributable

    public class SimpleNonAttribMEClassContainer
    {
        public SimpleNonAttribMEClass SimpleNonAttribMEClass { get; set; }
    }

    [TypeConverter(typeof(SimpleNonAttribMEConverter))]
    public class SimpleNonAttribMEClass
    {
        private List<string> _strings;

        public SimpleNonAttribMEClass()
        {
        }

        public List<string> Strings
        {
            get
            {
                return this._strings;
            }
            set
            {
                this._strings = value;
            }
        }
    }

    public class SimpleNonAttribMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleNonAttribMEClass simpleNonAttribMEClass = value as SimpleNonAttribMEClass;

            if (null == simpleNonAttribMEClass)
            {
                throw new ArgumentException();
            }

            SimpleNonAttribME extension = new SimpleNonAttribME();
            extension.Strings = simpleNonAttribMEClass.Strings;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class SimpleNonAttribME : MarkupExtension
    {
        private List<string> _strings;

        private SimpleNonAttribMEClass _simpleNonAttribMEClass;

        public SimpleNonAttribME()
        {
        }

        public List<string> Strings
        {
            get
            {
                return this._strings;
            }
            set
            {
                this._strings = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleNonAttribMEClass == null)
            {
                _simpleNonAttribMEClass = new SimpleNonAttribMEClass();
                _simpleNonAttribMEClass.Strings = this._strings;
            }

            return _simpleNonAttribMEClass;
        }
    }



    #endregion

    #region SimpleMESingleProperty

    public class SimpleMEClassSPContainer
    {
        public SimpleMESPClass SimpleMESPClass { get; set; }
    }

    [TypeConverter(typeof(SimpleMESPConverter))]
    public class SimpleMESPClass
    {
        private float _prop;

        public SimpleMESPClass()
        {
        }

        public float FloatProperty
        {
            get
            {
                return this._prop;
            }
            set
            {
                this._prop = value;
            }
        }
    }

    public class SimpleMESPConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleMESPClass simpleMESPClass = value as SimpleMESPClass;

            if (null == simpleMESPClass)
            {
                throw new ArgumentException();
            }

            SimpleSPME extension = new SimpleSPME();
            extension.FloatProperty = simpleMESPClass.FloatProperty;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class SimpleSPME : MarkupExtension
    {
        private float _prop;

        private SimpleMESPClass _simpleMESPClass;

        public SimpleSPME()
        {
        }

        public float FloatProperty
        {
            get
            {
                return this._prop;
            }
            set
            {
                this._prop = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMESPClass == null)
            {
                _simpleMESPClass = new SimpleMESPClass();
                _simpleMESPClass.FloatProperty = this._prop;
            }

            return _simpleMESPClass;
        }
    }

    #endregion

    #region MultipleProperties

    public class MultiplePropetiesMEContainer
    {
        public MultiplePropertiesMEClass MultiplePropertiesMEClass { get; set; }
    }

    public class MultiplePropetiesMEContainer2
    {
        [TypeConverter(typeof(MultiplePropertiesMEConverter<MultiplePropertyMEWMultiCtors>))]
        public MultiplePropertiesMEClass MultiplePropertiesMEClass { get; set; }
    }

    [TypeConverter(typeof(MultiplePropertiesMEConverter))]
    public class MultiplePropertiesMEClass
    {
        public bool BooleanProperty { get; set; }
        public Byte ByteProperty { get; set; }
        public Char CharProperty { get; set; }
        public Double DoubleProperty { get; set; }
        public int IntProperty { get; set; }
        public long LongProperty { get; set; }
        public short ShortProperty { get; set; }
        public string StringProperty { get; set; }

        public SimpleMEClass SimpleMEClass { get; set; }

        public MethodPointsProperty XMethodProperty { get; set; }

        public DifferentNamespaceType DifferentNamespaceTypeProperty { get; set; }

        public MultiplePropertiesMEClass()
        {
        }
    }

    public class MultiplePropertiesMEConverter<T> : TypeConverter where T : MultiplePropertyMEBase, new()
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            MultiplePropertiesMEClass toConvert = value as MultiplePropertiesMEClass;

            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            T extension = new T();
            // initialize extension here //
            extension.BooleanProperty = toConvert.BooleanProperty;
            extension.ByteProperty = toConvert.ByteProperty;
            extension.CharProperty = toConvert.CharProperty;
            extension.DoubleProperty = toConvert.DoubleProperty;
            extension.IntProperty = toConvert.IntProperty;
            extension.LongProperty = toConvert.LongProperty;
            extension.ShortProperty = toConvert.ShortProperty;
            extension.StringProperty = toConvert.StringProperty;
            extension.SimpleMEClass = toConvert.SimpleMEClass;
            extension.XMethodProperty = toConvert.XMethodProperty;
            extension.DifferentNamespaceTypeProperty = toConvert.DifferentNamespaceTypeProperty;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class MultiplePropertiesMEConverter : MultiplePropertiesMEConverter<MultiplePropertyME>
    {
    }

    public abstract class MultiplePropertyMEBase : MarkupExtension
    {

        MultiplePropertiesMEClass _meClass;

        public bool BooleanProperty { get; set; }
        public Byte ByteProperty { get; set; }
        public Char CharProperty { get; set; }
        public Double DoubleProperty { get; set; }
        public int IntProperty { get; set; }
        public long LongProperty { get; set; }
        public short ShortProperty { get; set; }
        public string StringProperty { get; set; }
        public SimpleMEClass SimpleMEClass { get; set; }
        public MethodPointsProperty XMethodProperty { get; set; }
        public DifferentNamespaceType DifferentNamespaceTypeProperty { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_meClass == null)
            {
                _meClass = new MultiplePropertiesMEClass();
                // initialize object here //
                _meClass.BooleanProperty = this.BooleanProperty;
                _meClass.ByteProperty = this.ByteProperty;
                _meClass.CharProperty = this.CharProperty;
                _meClass.DoubleProperty = this.DoubleProperty;
                _meClass.IntProperty = this.IntProperty;
                _meClass.LongProperty = this.LongProperty;
                _meClass.ShortProperty = this.ShortProperty;
                _meClass.StringProperty = this.StringProperty;
                _meClass.SimpleMEClass = this.SimpleMEClass;
                _meClass.XMethodProperty = this.XMethodProperty;
                _meClass.DifferentNamespaceTypeProperty = this.DifferentNamespaceTypeProperty;
            }

            return _meClass;
        }
    }

    public class MultiplePropertyME : MultiplePropertyMEBase
    {
        public MultiplePropertyME()
        {
        }


        public MultiplePropertyME(string str, bool b, Char ch, long l, short s)
        {
            this.StringProperty = str;
            this.BooleanProperty = b;
            this.CharProperty = ch;
            this.LongProperty = l;
            this.ShortProperty = s;
        }


        public MultiplePropertyME(SimpleMEClass input)
        {
            this.SimpleMEClass = input;
        }
    }

    public class MultiplePropertyME2 : MultiplePropertyMEBase
    {
        public MultiplePropertyME2()
        {
        }

        public MultiplePropertyME2(string str)
        {
            this.StringProperty = str;
        }

        public MultiplePropertyME2(string str, bool b, Char ch, long l, short s)
        {
            this.StringProperty = str;
            this.BooleanProperty = b;
            this.CharProperty = ch;
            this.LongProperty = l;
            this.ShortProperty = s;
        }
    }

    public class MultiplePropertyMEWMultiCtors : MultiplePropertyMEBase
    {
        public MultiplePropertyMEWMultiCtors()
        {
        }

        public MultiplePropertyMEWMultiCtors(string str, bool b, Char ch, long l, short s)
        {
            this.StringProperty = str;
            this.BooleanProperty = b;
            this.CharProperty = ch;
            this.LongProperty = l;
            this.ShortProperty = s;
        }

        public MultiplePropertyMEWMultiCtors(string input)
        {
            this.StringProperty = input;
        }

        public MultiplePropertyMEWMultiCtors(SimpleMEClass input)
        {
            this.SimpleMEClass = input;
        }
    }

    [TypeConverter(typeof(MethodPointsPropertyConverter))]
    public class MethodPointsProperty
    {
        internal MethodPointsProperty() { }
        public int x { get; set; }
        public string y { get; set; }

        public static MethodPointsProperty MyMethod(int x, string y)
        {
            return new MethodPointsProperty()
                       {
                           x = x,
                           y = y
                       };
        }
    }

    public class MethodPointsPropertyConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {

            if (destinationType == typeof(InstanceDescriptor) && value is MethodPointsProperty)
            {
                MethodPointsProperty pt = (MethodPointsProperty)value;

                MemberInfo mi = typeof(MethodPointsProperty).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              pt.x, pt.y
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }


    #endregion

    #region MarkupExtension for generic type

    public class GenericMEContainer
    {
        public GenericMEClass<string> GenericMEClass { get; set; }
    }

    [TypeConverter(typeof(GenericMEConverter))]
    public class GenericMEClass<T>
    {
        public T GenericProperty { get; set; }

        public GenericMEClass()
        {
        }
    }

    public class GenericMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            GenericMEClass<string> toConvert = value as GenericMEClass<string>;
            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            GenericME extension = new GenericME();
            // initialize extension here //
            extension.StringProperty = toConvert.GenericProperty;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class GenericME : MarkupExtension
    {

        private GenericMEClass<string> _meClass;

        public string StringProperty { get; set; }


        public GenericME()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_meClass == null)
            {
                _meClass = new GenericMEClass<string>();
                // initialize object here //
                _meClass.GenericProperty = this.StringProperty;
            }

            return _meClass;
        }
    }


    #endregion

    #region Collection ME
    public class CollectionMEContainer
    {
        public CollectionMEClass CollectionMEClass { get; set; }
    }

    [TypeConverter(typeof(CollectionMEConverter))]
    public class CollectionMEClass : List<string>
    {
        public CollectionMEClass()
        {
        }
    }

    public class CollectionMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            CollectionMEClass toConvert = value as CollectionMEClass;
            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            CollectionME extension = new CollectionME();
            // initialize extension here //
            foreach (string str in toConvert)
            {
                extension.Strings += str + "-";
            }

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class CollectionME : MarkupExtension
    {
        public String Strings { get; set; }

        private CollectionMEClass _meClass;

        public CollectionME()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_meClass == null)
            {
                _meClass = new CollectionMEClass();
                // initialize object here //
                string[] strings = Strings.Split('-');
                for (int i = 0; i < strings.Length - 1; i++)
                {
                    _meClass.Add(strings[i]);
                }
            }

            return _meClass;
        }
    }


    #endregion

    #region ME on primitive type

    public class StringMEContainer
    {
        [TypeConverter(typeof(StringMEConverter))]
        public string StringProperty { get; set; }
    }

    public class StringMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            string toConvert = value as string;
            if (null == toConvert)
            {
                throw new ArgumentException();
            }

            StringME extension = new StringME();
            // initialize extension here //
            extension.StringProperty = toConvert;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }

    public class StringME : MarkupExtension
    {
        private string _stringP;

        public String StringProperty
        {
            get
            {
                return this._stringP;
            }
            set
            {
                this._stringP = "!" + value + "!";
            }
        }

        public StringME()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _stringP.Replace("!", "");
        }
    }



    #endregion

    #region Property without setter

    public class SimpleMEWoSetterContainer
    {
        public SimpleMEWoSetterClass SimpleMEWoSetterClass { get; set; }
    }

    [TypeConverter(typeof(SimpleMEWoSetterConverter))]
    public class SimpleMEWoSetterClass
    {
        private readonly string _streetAddress;

        public SimpleMEWoSetterClass()
        {
        }

        public SimpleMEWoSetterClass(string ad)
        {
            this._streetAddress = ad;
        }

        public string Address
        {
            get
            {
                return this._streetAddress;
            }
        }
    }

    public class SimpleMEWoSetterConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleMEWoSetterClass simpleMEWoSetterClass = value as SimpleMEWoSetterClass;

            if (null == simpleMEWoSetterClass)
            {
                throw new ArgumentException();
            }

            SimpleMEWoSetter extension = new SimpleMEWoSetter();
            extension.Address = simpleMEWoSetterClass.Address;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class SimpleMEWoSetter : MarkupExtension
    {
        private string _address;
        private SimpleMEWoSetterClass _simpleMEWoSetterClass;

        public SimpleMEWoSetter()
        {
        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEWoSetterClass == null)
            {
                _simpleMEWoSetterClass = new SimpleMEWoSetterClass(this._address);
            }

            return _simpleMEWoSetterClass;
        }
    }



    #endregion

    #region Collection and Dictionary without setters

    public class SimpleMECodWoSetterContainer
    {
        public SimpleMECodWoSetterClass SimpleMECodWoSetterClass { get; set; }
    }

    [TypeConverter(typeof(SimpleMECodWoSetterConverter))]
    public class SimpleMECodWoSetterClass
    {
        private readonly ICollection _collection;
        private Dictionary<string, string> _dictionary;

        public SimpleMECodWoSetterClass()
        {
        }

        public SimpleMECodWoSetterClass(ICollection col, Dictionary<string, string> dic)
        {
            this._collection = col;
            this._dictionary = dic;
        }

        public ICollection Collection
        {
            get
            {
                return this._collection;
            }
        }

        public Dictionary<string, string> Dictionary
        {
            get
            {
                if (this._dictionary == null)
                {
                    this._dictionary = new Dictionary<string, string>();
                }
                return this._dictionary;
            }
        }
    }

    public class SimpleMECodWoSetterConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            SimpleMECodWoSetterClass simpleMECodWoSetterClass = value as SimpleMECodWoSetterClass;

            if (null == simpleMECodWoSetterClass)
            {
                throw new ArgumentNullException("simpleMECodWoSetterClass is null", "simpleMECodWoSetterClass");
            }

            SimpleMECodWoSetter extension = new SimpleMECodWoSetter();
            extension.Collection = simpleMECodWoSetterClass.Collection;
            extension.Dictionary = simpleMECodWoSetterClass.Dictionary;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class SimpleMECodWoSetter : MarkupExtension
    {
        private SimpleMECodWoSetterClass _simpleMECodWoSetterClass;

        public ICollection Collection { get; set; }
        private Dictionary<string, string> _dictionary;
        public Dictionary<string, string> Dictionary
        {
            get
            {
                if (this._dictionary == null)
                {
                    this._dictionary = new Dictionary<string, string>();
                }
                return this._dictionary;
            }

            set
            {
                this._dictionary = value;
            }
        }

        public SimpleMECodWoSetter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMECodWoSetterClass == null)
            {
                _simpleMECodWoSetterClass = new SimpleMECodWoSetterClass(this.Collection, this.Dictionary);
            }

            return _simpleMECodWoSetterClass;
        }
    }


    #endregion

    #region Nested Converters simple

    public class NestedMeContainer
    {
        public NestedMEClass NestedMEClass { get; set; }
    }

    [TypeConverter(typeof(NestedMEConverter))]
    public class NestedMEClass
    {
        public SimpleMEClass SimpleMEClass { get; set; }

        public string Address { get; set; }
    }

    public class NestedMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            NestedMEClass toConvert = value as NestedMEClass;

            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            NestedME extension = new NestedME();
            extension.Address = toConvert.Address;
            extension.SimpleMEClass = toConvert.SimpleMEClass;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class NestedME : MarkupExtension
    {
        public NestedME()
        {
        }


        public string Address { get; set; }
        public SimpleMEClass SimpleMEClass { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            NestedMEClass nestedMe = new NestedMEClass();
            nestedMe.Address = this.Address;
            nestedMe.SimpleMEClass = this.SimpleMEClass;


            return nestedMe;
        }
    }

    #endregion

    #region ME with typeconverter

    public class MeWithTCContainer
    {
        public MeWithTCClass MeWithTCClass { get; set; }
    }

    [TypeConverter(typeof(MeWithTCConverter))]
    public class MeWithTCClass
    {
        public string Address { get; set; }
    }

    public class MeWithTCConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            MeWithTCClass toConvert = value as MeWithTCClass;

            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            MeWithTC extension = new MeWithTC();
            extension.Address = toConvert.Address;

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class MeWithTC : MarkupExtension
    {
        public MeWithTC()
        {
        }

        [TypeConverter(typeof(StringTypeConverter))]
        public string Address { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            MeWithTCClass MeWithTCMe = new MeWithTCClass();
            MeWithTCMe.Address = this.Address;

            return MeWithTCMe;
        }
    }

    public class StringTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value;
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new DataTestException("ConvertTo should not be called for WPFv3 compatibility reasons.");
        }
    }


    #endregion

    #region typeconverter goes string and ME

    public class ComplexMEClassContainer
    {
        public ComplexMEClass ComplexMEClass { get; set; }
    }

    [TypeConverter(typeof(ComplexMEConverter))]
    public class ComplexMEClass
    {
        private string _streetAddress;

        public ComplexMEClass()
        {
        }

        public string Address
        {
            get
            {
                return this._streetAddress;
            }
            set
            {
                this._streetAddress = value;
            }
        }
    }

    public class ComplexMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(MarkupExtension) ||
                destinationType == typeof(string));
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension))
            {

                ComplexMEClass simpleMEClass = value as ComplexMEClass;

                if (null == simpleMEClass)
                {
                    throw new ArgumentNullException("simpleMEClass");
                }

                ComplexME extension = new ComplexME();
                extension.Address = simpleMEClass.Address;

                return extension;
            }

            if (destinationType == typeof(string))
            {
                return "!" + (value as ComplexMEClass).Address + "!";
            }

            throw new ArgumentException("desitnationType is not ME or string", "destinationType");
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value as string).Replace("!", "");
        }
    }
    public class ComplexME : MarkupExtension
    {
        private string _address;
        private ComplexMEClass _complexMEClass;

        public ComplexME()
        {
        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_complexMEClass == null)
            {
                _complexMEClass = new ComplexMEClass();
                _complexMEClass.Address = this._address;
            }

            return _complexMEClass;
        }
    }

    #endregion

    #region ME on both type and property

    public class TypePropertyClassContainer
    {
        [TypeConverter(typeof(PropertyMEConverter))]
        public TypePropertyClass TypePropertyClass { get; set; }
    }

    [TypeConverter(typeof(TypeMEConverter))]
    public class TypePropertyClass
    {
        private string _streetAddress;

        public TypePropertyClass()
        {
        }

        public string Address
        {
            get
            {
                return this._streetAddress;
            }
            set
            {
                this._streetAddress = value;
            }
        }
    }

    public class TypeMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            TypePropertyClass toConvert = value as TypePropertyClass;

            if (null == toConvert)
            {
                throw new ArgumentException();
            }

            TypeME extension = new TypeME();
            extension.TypeAddress = toConvert.Address;

            return extension;

        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class PropertyMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            TypePropertyClass toConvert = value as TypePropertyClass;

            if (null == toConvert)
            {
                throw new ArgumentNullException("toConvert");
            }

            PropertyME extension = new PropertyME();
            extension.PropertyAddress = toConvert.Address;

            return extension;

        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeME : MarkupExtension
    {
        private string _address;
        private TypePropertyClass _toConvert;

        public TypeME()
        {
        }

        public string TypeAddress
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_toConvert == null)
            {
                _toConvert = new TypePropertyClass();
                _toConvert.Address = this._address;
            }

            return _toConvert;
        }
    }

    public class PropertyME : MarkupExtension
    {
        private string _address;
        private TypePropertyClass _toConvert;

        public PropertyME()
        {
        }

        public string PropertyAddress
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_toConvert == null)
            {
                _toConvert = new TypePropertyClass();
                _toConvert.Address = this._address;
            }

            return _toConvert;
        }
    }

    #endregion

    #region ME with param arguments

    public class ParamsMEClassContainer
    {
        public ParamsMEClass ParamsMEClass { get; set; }
    }

    [TypeConverter(typeof(ParamsMEConverter))]
    public class ParamsMEClass
    {
        private string _streetAddress;
        private int _aptNo;

        public ParamsMEClass()
        {
        }

        public string Address
        {
            get
            {
                return this._streetAddress;
            }
            set
            {
                this._streetAddress = value;
            }
        }

        public int AptNo
        {
            get
            {
                return this._aptNo;
            }
            set
            {
                this._aptNo = value;
            }
        }
    }

    public class ParamsMEConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(MarkupExtension);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(MarkupExtension))
            {
                throw new ArgumentException("destinationType is not a MarkupExtension", "destinationType");
            }

            ParamsMEClass simpleMEClass = value as ParamsMEClass;

            if (null == simpleMEClass)
            {
                throw new ArgumentNullException("simpleMEClass is null", "simpleMEClass");
            }

            ParamsME extension = new ParamsME(simpleMEClass.Address, simpleMEClass.AptNo);

            return extension;
        }


        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            throw new NotSupportedException();
        }
    }
    public class ParamsME : MarkupExtension
    {
        private ParamsMEClass _simpleMEClass;

        public ParamsME()
        {
        }

        public ParamsME(params object[] parameters)
        {
            this.Parameters = parameters;
        }

        [ConstructorArgument("parameters")]
        public object[] Parameters { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_simpleMEClass == null)
            {
                _simpleMEClass = new ParamsMEClass();
                _simpleMEClass.Address = (string)this.Parameters[0];
                _simpleMEClass.AptNo = (int)this.Parameters[1];
            }

            return _simpleMEClass;
        }
    }


    #endregion

    #region IXNameResolver extension

    //public class Wrapper
    //{
    //    public object Value { get; set; }
    //}

    //public class ContextExtension : MarkupExtension
    //{
    //    public override object ProvideValue(IServiceProvider serviceProvider)
    //    {
    //        IXNameResolver xnameResolver = (IXNameResolver)serviceProvider.GetService(typeof(IXNameResolver));
    //        return xnameResolver;
    //    }
    //}
    #endregion

    #region PreferExtensionOverNoneTest
    public class PreferExtensionWrapper
    {
        [TypeConverter(typeof(PreferExtensionTypeTypeConverter))]
        public PreferExtensionType Data { get; set; }
    }

    public class PreferExtensionType : MarkupExtension
    {
        public PreferExtensionType()
        {
            throw new Exception("This constructor shouldn't be called.");
        }

        public PreferExtensionType(int i) { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            throw new Exception("This should never be called.");
        }
    }

    public class PreferExtensionTypeExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PreferExtensionType(3);
        }
    }

    public class PreferExtensionTypeTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(MarkupExtension)) return true;
            else return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return new PreferExtensionType(3);
        }
    }
    #endregion

    #region VerySimpleMarkupExtension
    [ContentProperty("Value")]
    public class VerySimpleMarkupExtension : MarkupExtension
    {
        public object Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
    #endregion
}

namespace Microsoft.Test.Xaml.Types.MarkupExtensions2
{
    [TypeConverter(typeof(SimpleMEConverterDiffNS))]
    public class SimpleMEClassDiffNS
    {
        public SimpleMEClassDiffNS()
        {
        }

        public SimpleMEClassDiffNS(string address)
        {
            this.Address = address;
        }

        public string Address { get; set; }
        public int AptNo { get; set; }
    }
}
namespace Microsoft.Test.Xaml.Types.MarkupExtensions.Ns1
{
    public class DifferentNamespaceType
    {
        public int IntegerProperty { get; set; }
    }
}
