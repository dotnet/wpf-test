// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Types
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Windows.Markup;
    using System.Xaml;
    using System.Xaml.Schema;
    using Microsoft.Test.Xaml.Common;
    using Microsoft.Test.Globalization;
    using Microsoft.Test;
    using Microsoft.Test.Xaml.Driver;

    // RW property of collection
    public class Zoo1
    {
        private List<Animal> _animals;

        public Zoo1()
        {
            this._animals = new List<Animal>();
        }

        [TypeConverter(typeof(AnimalCollectionConverter))]
        public List<Animal> Animals
        {
            get
            {
                return _animals;
            }
            set
            {
                this._animals = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Zoo1 instance1 = new Zoo1();
            Animal tiger = new Animal();
            tiger.Name = "Tiger";
            tiger.Number = 2;
            Animal monkey = new Animal();
            monkey.Name = "Monkey";
            monkey.Number = 3;
            instance1.Animals.Add(tiger);
            instance1.Animals.Add(monkey);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            Zoo1 instance2 = new Zoo1();
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 2
                              });

            return testCases;
        }

        #endregion
    }

    //RO property of collection with constructor arg
    public class Zoo2
    {
        private readonly List<Animal> _animals;

        public Zoo2(List<Animal> animals)
        {
            if (animals == null)
            {
                this._animals = new List<Animal>();
            }
            else
            {
                this._animals = animals;
            }
        }

        [TypeConverter(typeof(AnimalCollectionConverter))]
        [ConstructorArgument("animals")]
        public List<Animal> Animals
        {
            get
            {
                return _animals;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Zoo2 instance1 = new Zoo2(null);
            Animal tiger = new Animal();
            tiger.Name = "Tiger";
            tiger.Number = 2;
            Animal monkey = new Animal();
            monkey.Name = "Monkey";
            monkey.Number = 3;
            instance1.Animals.Add(tiger);
            instance1.Animals.Add(monkey);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });

            Zoo2 instance2 = new Zoo2(null);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 2
                              });

            return testCases;
        }

        #endregion
    }

    public class Zoo3
    {
        private readonly AnimalList _animals;

        public Zoo3(AnimalList animals)
        {
            if (animals == null)
            {
                this._animals = new AnimalList();
            }
            else
            {
                this._animals = animals;
            }
        }

        [ConstructorArgument("animals")]
        public AnimalList Animals
        {
            get
            {
                return _animals;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();

            Zoo3 instance1 = new Zoo3(null);
            Animal tiger = new Animal();
            tiger.Name = "Tiger";
            tiger.Number = 2;
            Animal monkey = new Animal();
            monkey.Name = "Monkey";
            monkey.Number = 3;
            instance1.Animals.Add(tiger);
            instance1.Animals.Add(monkey);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1,
                                  TestID = instanceIDPrefix + 1
                              });
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance1.Animals,
                                  TestID = instanceIDPrefix + 2
                              });

            Zoo3 instance2 = new Zoo3(null);
            testCases.Add(new TestCaseInfo
                              {
                                  Target = instance2,
                                  TestID = instanceIDPrefix + 3
                              });

            return testCases;
        }

        #endregion
    }

    public class Animal
    {
        private string _name;
        private int _number;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
            }
        }
    }

    public class AnimalCollectionConverter : TypeConverter
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
            if (value is string)
            {
                List<Animal> animals = new List<Animal>();
                string[] animalStrings = (value as string).Split(new char[]
                                                                     {
                                                                         '#', '$'
                                                                     }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string animalString in animalStrings)
                {
                    string[] animalInfo = animalString.Split(':');
                    Animal animal = new Animal();
                    animal.Name = animalInfo[0];
                    animal.Number = Int32.Parse(animalInfo[1]);
                    animals.Add(animal);
                }

                return animals;
            }
            else
            {
                throw new ArgumentException("In ConvertFrom: can not convert to List<Animal>.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is List<Animal>)
            {
                List<Animal> animals = value as List<Animal>;
                StringBuilder text = new StringBuilder();

                for (int i = 0; i < animals.Count; i++)
                {
                    Animal animal = animals[i];
                    text.Append(animal.Name + ":" + animal.Number);

                    if (i != animals.Count - 1)
                    {
                        text.Append("#");
                    }
                }
                return text.ToString();
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "In ConvertTo: can not convert type '{0}' to string.", value.GetType().AssemblyQualifiedName));
            }
        }
    }

    [TypeConverter(typeof(AnimalListConverter))]
    public class AnimalList : List<Animal>
    {
    }

    public class AnimalListConverter : TypeConverter
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
            if (value is string)
            {
                AnimalList animals = new AnimalList();
                string[] animalStrings = (value as string).Split(new char[]
                                                                     {
                                                                         '#', '$'
                                                                     }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string animalString in animalStrings)
                {
                    string[] animalInfo = animalString.Split(':');
                    Animal animal = new Animal();
                    animal.Name = animalInfo[0];
                    animal.Number = Int32.Parse(animalInfo[1]);
                    animals.Add(animal);
                }

                return animals;
            }
            else
            {
                throw new ArgumentException("In ConvertFrom: can not convert to List<Animal>.");
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is AnimalList)
            {
                AnimalList animals = value as AnimalList;
                StringBuilder text = new StringBuilder();

                for (int i = 0; i < animals.Count; i++)
                {
                    Animal animal = animals[i];
                    text.Append(animal.Name + ":" + animal.Number);

                    if (i != animals.Count - 1)
                    {
                        text.Append("#");
                    }
                }
                return text.ToString();
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "In ConvertTo: can not convert type '{0}' to string.", value.GetType().AssemblyQualifiedName));
            }
        }
    }

    [TypeConverter(typeof(AlwaysThrowStringTypeConverter))]
    public enum EnumWithTypeConverter
    {
        One,
        Two,
        Three
    }

    public enum EnumWithoutTypeConverter
    {
        Four,
        Five,
        Six
    }

    public class ClassWithEnumWithTypeConverter
    {
        public EnumWithTypeConverter Value { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            string message = Exceptions.GetMessage("TypeConverterFailed2", WpfBinaries.SystemXaml);
            message = string.Format(message, "Three", "System.String");
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new ClassWithEnumWithTypeConverter
                                                {
                                                    Value = EnumWithTypeConverter.Three
                                                },
                                   ExpectedResult = false,
                                   ExpectedMessage = message,
                               }
                       };
        }

        #endregion
    }

    public class ClassWithEnumWithoutTypeConverter
    {
        [TypeConverter(typeof(AlwaysThrowStringTypeConverter))]
        public EnumWithoutTypeConverter Value { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            string message = Exceptions.GetMessage("TypeConverterFailed2", WpfBinaries.SystemXaml);
            message = string.Format(message, "Five", "System.String");
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new ClassWithEnumWithoutTypeConverter
                                                {
                                                    Value = EnumWithoutTypeConverter.Five
                                                },
                                   ExpectedResult = false,
                                   ExpectedMessage = message,
                               }
                       };
        }

        #endregion
    }

    public class AlwaysThrowStringTypeConverter : TypeConverter
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
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new InvalidOperationException("This TypeConverter should never be called.");
        }
    }

    [TypeConverter(typeof(BadTypeConverter))]
    public class ClassWithBadTypeConverter
    {
        public string Data { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            List<TestCaseInfo> testCases = new List<TestCaseInfo>();
            testCases.Add(new TestCaseInfo
                              {
                                  TestID = instanceIDPrefix + 1,
                                  Target = new ClassWithBadTypeConverterWrapper
                                               {
                                                   Data = new ClassWithBadTypeConverter
                                                              {
                                                                  Data = "some data"
                                                              }
                                               },
                                  ExpectedResult = false,
                                  ExpectedMessage = Exceptions.GetMessage("SetValue", WpfBinaries.SystemXaml),
                              });

            return testCases;
        }

        #endregion
    }

    public class ClassWithBadTypeConverterWrapper
    {
        public ClassWithBadTypeConverter Data { get; set; }
    }

    public class BadTypeConverter : TypeConverter
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
            return DateTime.Now;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var actualClass = (ClassWithBadTypeConverter)value;
            return actualClass.Data;
        }
    }

    public class ImaginaryWrapper
    {
        public ImaginaryNumber Number { get; set; }
    }

    [TypeConverter(typeof(ImaginaryNumberConverter))]
    public class ImaginaryNumber
    {
        public int Real { get; set; }
        public int Imaginary { get; set; }
    }

    public class ImaginaryNumberConverter : TypeConverter
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
            string number = (string)value;
            ImaginaryNumber i = new ImaginaryNumber();
            string[] parts = number.Split('+');
            i.Real = ((IConvertible)parts[0]).ToInt32(null);
            string imaginary = parts[1].Remove(parts[1].Length - 1);
            i.Imaginary = ((IConvertible)imaginary).ToInt32(null);
            return i;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            //ImaginaryNumber i = (ImaginaryNumber)value;
            //return i.Real + "+" + i.Imaginary + "i";
            throw new DataTestException("Type converter is used in serialization.");
        }
    }

    public class UsesNested
    {
        public UsesNested()
        {
            MyNestedType = new Nested()
                               {
                                   MyString = "string on nested type"
                               };
        }

        [TypeConverter(typeof(NestedConverter))]
        public NestedBase MyNestedType { get; set; }

        public static NestedBase CreateNested(string nestedString)
        {
            return new Nested()
                       {
                           MyString = nestedString
                       };
        }

        private class Nested : NestedBase
        {
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new UsesNested()
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 2,
                                   Target = new UsesNestedInvalid(),
                                   ExpectedResult = false,
                                   ExpectedMessage = Exceptions.GetMessage("ObjectReader_TypeNotVisible", WpfBinaries.SystemXaml),
                               }
                       };
        }

        #endregion
    }

    public class UsesNestedInvalid
    {
        public UsesNestedInvalid()
        {
            MyNestedType = new Nested()
                               {
                                   MyString = "string on nested type"
                               };
        }

        public NestedBase MyNestedType { get; set; }

        public static NestedBase CreateNested(string nestedString)
        {
            return new Nested()
                       {
                           MyString = nestedString
                       };
        }

        [TypeConverter(typeof(NestedConverter))]
        private class Nested : NestedBase
        {
        }
    }

    public class NestedBase
    {
        public string MyString { get; set; }
    }

    public class NestedConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() != typeof(string))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return UsesNested.CreateNested((string)value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType().IsAssignableFrom(typeof(NestedBase)))
            {
                return base.ConvertFrom(context, culture, value);
            }

            return ((NestedBase)value).MyString;
        }
    }

    [TypeConverter(typeof(CPAAndTypeConverter))]
    [ContentProperty("NameCPA")]
    public class CPAAndTypeConverterContainer
    {
        public string NameCPA { get; set; }
    }

    // only roundtrips with one item in the list
    [TypeConverter(typeof(CPAAndTypeConverter))]
    [ContentProperty("NameCPA")]
    public class CPAonCollectionTypeConverterContainer
    {
        List<string> _content = new List<string>();
        public List<string> NameCPA { get { return _content; } }
    }


    public class CPAAndTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext typeDescriptorContext, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override bool CanConvertTo(ITypeDescriptorContext typeDescriptorContext, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value)
        {
            String str = value as string;
            return str + "ContertedThroughTypeConverter";
        }

        public override object ConvertTo(ITypeDescriptorContext typeDescriptorContext, CultureInfo cultureInfo, object value, Type destinationType)
        {
            if (value is CPAAndTypeConverterContainer)
            {
                CPAAndTypeConverterContainer cPAAndTypeConverterContainer = value as CPAAndTypeConverterContainer;
                return cPAAndTypeConverterContainer.NameCPA;
            }
            else
            {
                var container = value as CPAonCollectionTypeConverterContainer;
                return container.NameCPA[0];
            }
        }
    }

    #region XamlNamespaceResolverConverter
    public class XamlNamespaceResolverContainer
    {
        public XamlNamespaceResolverTypeList Types { get; set; }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new XamlNamespaceResolverContainer(),
                               },
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 2,
                                   Target = new XamlNamespaceResolverContainer()
                                   {
                                       Types = new XamlNamespaceResolverTypeList
                                       {
                                           typeof(string),
                                           typeof(int),
                                           typeof(XamlNamespaceResolverContainer)
                                       }
                                   }
                               }
                       };
        }

        #endregion
    }

    [TypeConverter(typeof(XamlNamespaceResolverTypeListConverter))]
    public class XamlNamespaceResolverTypeList : IEnumerable<Type>
    {
        private HashSet<Type> _types = new HashSet<Type>();

        public XamlNamespaceResolverTypeList()
        {
        }

        public void Add(Type type)
        {
            _types.Add(type);
        }

        #region IEnumerable<Type> Members

        public IEnumerator<Type> GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        #endregion
    }

    // reads and writes names of types as a space delimitted string
    // x:String x:Int32 etc
    public class XamlNamespaceResolverTypeListConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            XamlNamespaceResolverTypeList list = new XamlNamespaceResolverTypeList();
            string[] types = ((string)value).Trim().Split(' ');
            var namespaceResolver = context.GetService(typeof(IXamlNamespaceResolver)) as IXamlNamespaceResolver;
            var schemaContextProvider = context.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;

            if (namespaceResolver == null)
            {
                throw new Exception("IXamlNamespaceResolver service not available.");
            }

            if (schemaContextProvider == null)
            {
                throw new Exception("IXamlSchemaContextProvider service not available.");
            }

            var schemaContext = schemaContextProvider.SchemaContext;

            foreach (string type in types)
            {
                string[] parts = type.Split(':');
                string ns = namespaceResolver.GetNamespace(parts[0]);
                XamlType xamlType = schemaContext.GetXamlType(new XamlTypeName(ns, parts[1]));
                list.Add(xamlType.UnderlyingType);
            }

            return list;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            StringBuilder temp = new StringBuilder();

            var schemaContextProvider = context.GetService(typeof(IXamlSchemaContextProvider)) as IXamlSchemaContextProvider;
            var namespacePrefixLookup = context.GetService(typeof(INamespacePrefixLookup)) as INamespacePrefixLookup;

            if (schemaContextProvider == null)
            {
                throw new Exception("IXamlSchemaContextProvider service not available.");
            }

            if (namespacePrefixLookup == null)
            {
                throw new Exception("INamespacePrefixLookup service not available.");
            }

            XamlSchemaContext schemaContext = schemaContextProvider.SchemaContext;
            foreach (Type t in ((XamlNamespaceResolverTypeList)value))
            {
                XamlType xamlType = schemaContext.GetXamlType(t);
                string prefix = namespacePrefixLookup.LookupPrefix(xamlType.GetXamlNamespaces()[0]);
                temp.Append(prefix + ":" + t.Name + " ");
            }

            return temp.ToString();
        }
    }
    #endregion

    [TypeConverter(typeof(AbstractBaseClassTC))]
    public abstract class AbstractBaseClasswithTC
    {
    }

    internal class InternalDerivedClass : AbstractBaseClasswithTC
    {
    }

    public class AbstractBaseClassTC : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string v = value as string;
            if (v == "InternalDerivedClassName")
            {
                return new InternalDerivedClass();
            }
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return "InternalDerivedClassName";
            }
            return null;
        }
    }

    public class ContainerClassForInternalTypeTest
    {
        AbstractBaseClasswithTC _prop = new InternalDerivedClass();
        public AbstractBaseClasswithTC Prop
        {
            get
            {
                return _prop;
            }
            set
            {
                _prop = value;
            }
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new ContainerClassForInternalTypeTest(),
                               },
                       };
        }

        #endregion
    }

    [ContentProperty("Content")]
    public class ContentContainer
    {
        [TypeConverter(typeof(EmptyTypeConverter))]
        public object Content { get; set; }

        public ContentContainer()
        {
            this.Content = String.Empty;
        }

        #region Test Implementation

        public static List<TestCaseInfo> GetTestCases()
        {
            string instanceIDPrefix = XamlTestDriver.GetInstanceIDPrefix(MethodInfo.GetCurrentMethod().DeclaringType);
            return new List<TestCaseInfo>
                       {
                           new TestCaseInfo
                               {
                                   TestID = instanceIDPrefix + 1,
                                   Target = new ContentContainer(){ },
                                   XPathExpresions = 
                                   {
                                        "/t:ContentContainer[@Content=\"\"]"    
                                   },
                                   XPathNamespacePrefixMap = 
                                   {
                                       { "t", "http://XamlTestTypes"}
                                   }
                               },
                       };
        }

        #endregion
    }

    public class EmptyTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return String.Empty;
        }
    }

    public class Dictionaries<K, V>
    {
        public IDictionary IDictionary { get; set; }

        public IDictionary<K, V> GenericIDictionary { get; set; }

        public Dictionary<K, V> Dictionary { get; set; }

        public MyDictionary<K, V> MyDictionary { get; set; }

        public Dictionaries()
        {
            IDictionary = new Dictionary<K, V>();
            GenericIDictionary = new Dictionary<K, V>();
            Dictionary = new Dictionary<K, V>();
            MyDictionary = new MyDictionary<K, V>();
        }
    }

    public class MyDictionary<K, V> : Dictionary<K, V>
    {
    }

    [XamlSetTypeConverter("BaseHandler")]
    public class BaseWithSetTypeConverter
    {
        [DefaultValue(null)]
        public string BaseProperty { get; set; }

        public static void BaseHandler(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
        {
            ((BaseWithSetTypeConverter)targetObject).BaseProperty = BasePropertyValue;
        }

        public const string BasePropertyValue = "Base prop set.";
    }

    [XamlSetTypeConverter("DerivedHandler")]
    public class DerivedWithSetTypeConverter : BaseWithSetTypeConverter
    {
        public Tiger DerivedProperty { get; set; }
        public static void DerivedHandler(object targetObject, XamlSetTypeConverterEventArgs eventArgs)
        {
            eventArgs.CallBase();
        }
    }
}
