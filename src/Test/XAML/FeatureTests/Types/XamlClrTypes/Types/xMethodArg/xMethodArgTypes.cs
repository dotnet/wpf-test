// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Microsoft.Test.Xaml.Types.MethodArguments
{
    public class ConstructorArgTypes1
    {
        [ConstructorArgument("x")]
        public int x { get; set; }
        [ConstructorArgument("y")]
        public string y { get; set; }

        public ConstructorArgTypes1(int x, string y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class ConstructorArgTypes2
    {
        [ConstructorArgument("x")]
        public ComplexType ComplexType { get; set; }

        public ConstructorArgTypes2(ComplexType x)
        {
            this.ComplexType = x;
        }
    }

    public class ComplexType
    {
        public int x { get; set; }
        public string y { get; set; }
    }

    public class ConstructorArgTypes3
    {
        [ConstructorArgument("dictionary")]
        public IDictionary<int, int> dictionary { get; set; }

        [ConstructorArgument("list")]
        public IList<int> list { get; set; }

        [ConstructorArgument("array")]
        public int[] array { get; set; }

        [ConstructorArgument("enumerable")]
        public IEnumerable enumerable { get; set; }

        public ConstructorArgTypes3(IDictionary<int, int> dictionary, IList<int> list, int[] array, IEnumerable enumerable)
        {
            this.dictionary = dictionary;
            this.list = list;
            this.array = array;
            this.enumerable = enumerable;
        }
    }

    public class ConstructorArgTypes4
    {
        [ConstructorArgument("x")]
        public List<int> GenericList { get; set; }

        public ConstructorArgTypes4(List<int> x)
        {
            this.GenericList = x;
        }
    }

    public class ConstructorArgTypes5
    {
        [ConstructorArgument("x")]
        public int? nullableInt { get; set; }

        public ConstructorArgTypes5(int? x)
        {
            this.nullableInt = x;
        }
    }

    [TypeConverter(typeof (NoArgumentMethodClassTypeConverter))]
    public class NoArgumentMethodClass
    {
        internal NoArgumentMethodClass() { }
        public int x { get; set; }
        public string y { get; set; }

        public static NoArgumentMethodClass MyMethod()
        {
            return new NoArgumentMethodClass()
                       {
                           x = 10, y = "Hello World"
                       };
        }

        public static NoArgumentMethodClass Create()
        {
            return new NoArgumentMethodClass();
        }
    }

    public class NoArgumentMethodClassTypeConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is NoArgumentMethodClass)
            {
                NoArgumentMethodClass pt = (NoArgumentMethodClass) value;

                MemberInfo mi = typeof (NoArgumentMethodClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (DifferentArgumentMethodClassTypeConverter))]
    public class DifferentArgumentMethodClass
    {
        public int x { get; set; }

        public IDictionary<int, int> dictionary { get; set; }

        public IList<int> list { get; set; }

        public int[] array { get; set; }

        public IEnumerable enumerable { get; set; }

        public int? nullableInt { get; set; }

        public static DifferentArgumentMethodClass MyMethod(int x,
                                                            IDictionary<int, int> dict,
                                                            IList<int> list,
                                                            int[] array,
                                                            IEnumerable enumerable,
                                                            int? nullable)
        {
            return new DifferentArgumentMethodClass()
                       {
                           x = x,
                           dictionary = dict,
                           array = array,
                           enumerable = enumerable,
                           list = list,
                           nullableInt = nullable
                       };
        }
    }

    public class DifferentArgumentMethodClassTypeConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is DifferentArgumentMethodClass)
            {
                DifferentArgumentMethodClass pt = (DifferentArgumentMethodClass) value;

                MemberInfo mi = typeof (DifferentArgumentMethodClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              pt.x, pt.dictionary, pt.list, pt.array, pt.enumerable, pt.nullableInt
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class OverloadedConstructors
    {
        [ConstructorArgument("x")]
        public int x { get; set; }
        [ConstructorArgument("y")]
        public string y { get; set; }

        public OverloadedConstructors(int x, string y)
        {
            this.x = x;
            this.y = y;
        }

        public OverloadedConstructors(int x)
        {
            this.x = x;
            this.y = "boho";
        }
    }

    public class OverloadedConstructorsDerived
    {
        [ConstructorArgument("x")]
        public int x { get; set; }

        [ConstructorArgument("y")]
        public Base y { get; set; }

        public OverloadedConstructorsDerived(int x, Base y)
        {
            this.x = x;
            this.y = y;
        }

        public OverloadedConstructorsDerived(int x, Derived y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Base
    {
        public int x { get; set; }
    }

    public class Derived : Base
    {
        public string y { get; set; }
    }

    [TypeConverter(typeof (OverloadedArgumentMethodClassTypeConverter))]
    public class OverloadedArgumentMethodClass
    {
        internal OverloadedArgumentMethodClass() { }
        public int x { get; set; }
        public string y { get; set; }

        public static OverloadedArgumentMethodClass MyMethod()
        {
            return new OverloadedArgumentMethodClass()
                       {
                           x = 10, y = "Hello World"
                       };
        }

        public static OverloadedArgumentMethodClass MyMethod(int x, string y)
        {
            return new OverloadedArgumentMethodClass()
                       {
                           x = x, y = y
                       };
        }

        public static OverloadedArgumentMethodClass Create()
        {
            return new OverloadedArgumentMethodClass();
        }
    }

    public class OverloadedArgumentMethodClassTypeConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is OverloadedArgumentMethodClass)
            {
                OverloadedArgumentMethodClass pt = (OverloadedArgumentMethodClass) value;

                MemberInfo mi = typeof (OverloadedArgumentMethodClass).GetMember("MyMethod")[1];
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

    [TypeConverter(typeof (MethodOnDifferentTypeTypeConverter))]
    public class MethodOnDifferentType
    {
        internal MethodOnDifferentType() { }
        public int x { get; set; }
        public string y { get; set; }

        public static MethodOnDifferentType Create()
        {
            return new MethodOnDifferentType();
        }
    }

    public class DifferentType
    {
        public static MethodOnDifferentType MyMethod(int x, string y)
        {
            return new MethodOnDifferentType()
                       {
                           x = x, y = y
                       };
        }
    }

    public class MethodOnDifferentTypeTypeConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is MethodOnDifferentType)
            {
                MethodOnDifferentType pt = (MethodOnDifferentType) value;

                MemberInfo mi = typeof (DifferentType).GetMember("MyMethod")[0];
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

    public class GenericTypeConstructor<T>
    {
        public T x { get; set; }
        public string y { get; set; }

        public static GenericTypeConstructor<T> MyMethod(T x, string y)
        {
            return new GenericTypeConstructor<T>()
                       {
                           x = x, y = y
                       };
        }
    }

    [TypeConverter(typeof (GenericTypexMethodTypeConverter))]
    public class GenericTypexMethod<T>
    {
        public T x { get; set; }
        public string y { get; set; }

        public static GenericTypexMethod<T> MyMethod(T x, string y)
        {
            return new GenericTypexMethod<T>()
                       {
                           x = x, y = y
                       };
        }
    }

    public class GenericTypexMethodTypeConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is GenericTypexMethod<int>)
            {
                GenericTypexMethod<int> pt = (GenericTypexMethod<int>) value;

                MemberInfo mi = typeof (GenericTypexMethod<int>).GetMember("MyMethod")[0];
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

    [TypeConverter(typeof (GenericArgTypeMethodConverter))]
    public class GenericArgTypeMethod
    {
        public List<int> x { get; set; }
        public string y { get; set; }

        public static GenericArgTypeMethod MyMethod(List<int> x, string y)
        {
            return new GenericArgTypeMethod()
                       {
                           x = x, y = y
                       };
        }
    }

    public class GenericArgTypeMethodConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is GenericArgTypeMethod)
            {
                GenericArgTypeMethod pt = (GenericArgTypeMethod) value;

                MemberInfo mi = typeof (GenericArgTypeMethod).GetMember("MyMethod")[0];
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

    [TypeConverter(typeof (MethodWithParameterListConverter))]
    public class MethodWithParameterListClass
    {
        public List<int> x { get; set; }
        public string y { get; set; }

        public static MethodWithParameterListClass MyMethod(params object[] foo)
        {
            return new MethodWithParameterListClass()
                       {
                           x = (List<int>) foo[0], y = (string) foo[1]
                       };
        }
    }

    public class MethodWithParameterListConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is MethodWithParameterListClass)
            {
                MethodWithParameterListClass pt = (MethodWithParameterListClass) value;

                MemberInfo mi = typeof (MethodWithParameterListClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              new object[]
                                                                  {
                                                                      pt.x, pt.y
                                                                  }
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ConstructorWithParameterListClass
    {
        [ConstructorArgument("foo")]
        public int[] x { get; set; }

        public ConstructorWithParameterListClass(params int[] foo)
        {
            this.x = foo;
        }
    }

    [TypeConverter(typeof (MethodWithParameterListOverloadConverter))]
    public class MethodWithParameterListOverloadClass
    {
        public List<int> x { get; set; }
        public string y { get; set; }

        public static MethodWithParameterListOverloadClass MyMethod(params object[] foo)
        {
            return new MethodWithParameterListOverloadClass()
                       {
                           x = (List<int>) foo[0], y = (string) foo[1]
                       };
        }

        public static MethodWithParameterListOverloadClass MyMethod(List<int> x, string y)
        {
            return new MethodWithParameterListOverloadClass()
                       {
                           x = x, y = y
                       };
        }
    }

    public class MethodWithParameterListOverloadConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is MethodWithParameterListOverloadClass)
            {
                MethodWithParameterListOverloadClass pt = (MethodWithParameterListOverloadClass) value;

                MemberInfo mi = typeof (MethodWithParameterListOverloadClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              new object[]
                                                                  {
                                                                      pt.x, pt.y
                                                                  }
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (MismatchedParametersConverter))]
    public class MismatchedParametersClass
    {
        internal MismatchedParametersClass() { }
        public List<int> x { get; set; }
        public string y { get; set; }

        public static MismatchedParametersClass MyMethod(List<int> x, string y)
        {
            return new MismatchedParametersClass()
                       {
                           x = x, y = y
                       };
        }

        public static MismatchedParametersClass Create()
        {
            return new MismatchedParametersClass();
        }
    }

    public class MismatchedParametersConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is MismatchedParametersClass)
            {
                MismatchedParametersClass pt = (MismatchedParametersClass) value;

                MemberInfo mi = typeof (MismatchedParametersClass).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              pt.y, pt.x
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (NonStaticMethodConverter))]
    public class NonStaticMethod
    {
        internal NonStaticMethod() { }
        public List<int> x { get; set; }
        public string y { get; set; }

        public NonStaticMethod MyMethod(List<int> x, string y)
        {
            return new NonStaticMethod()
                       {
                           x = x, y = y
                       };
        }

        public static NonStaticMethod Create()
        {
            return new NonStaticMethod();
        }
    }

    public class NonStaticMethodConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is NonStaticMethod)
            {
                NonStaticMethod pt = (NonStaticMethod) value;

                MemberInfo mi = typeof (NonStaticMethod).GetMember("MyMethod")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              pt.y, pt.x
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (MethodPointsPropertyConverter))]
    public class MethodPointsProperty
    {
        internal MethodPointsProperty() { }
        public List<int> x { get; set; }
        public string y { get; set; }

        public MethodPointsProperty MyMethod(List<int> x, string y)
        {
            return new MethodPointsProperty()
                       {
                           x = x, y = y
                       };
        }

        public static MethodPointsProperty Create()
        {
            return new MethodPointsProperty();
        }
    }

    public class MethodPointsPropertyConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is MethodPointsProperty)
            {
                MethodPointsProperty pt = (MethodPointsProperty) value;

                MemberInfo mi = typeof (MethodPointsProperty).GetMember("x")[0];
                if (mi != null)
                {
                    return new InstanceDescriptor(mi, new object[]
                                                          {
                                                              pt.y, pt.x
                                                          }, true);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [TypeConverter(typeof (VoidMethodConverter))]
    public class VoidMethod
    {
        public List<int> x { get; set; }
        public string y { get; set; }

        public static void MyMethod(List<int> x, string y)
        {
            //return new VoidMethod() { x = x, y = y };
        }
    }

    public class VoidMethodConverter : TypeConverter
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
            if (destinationType == typeof (InstanceDescriptor) && value is VoidMethod)
            {
                VoidMethod pt = (VoidMethod) value;

                MemberInfo mi = typeof (VoidMethod).GetMember("MyMethod")[0];
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

    public class Nested
    {
        [ConstructorArgument("x")]
        public DifferentArgumentMethodClass foo { get; set; }

        public Nested(DifferentArgumentMethodClass x)
        {
            this.foo = x;
        }
    }

    public class ConstructorArgTypesDefault
    {
        [ConstructorArgument("x")]
        public int x { get; set; }
        [ConstructorArgument("y")]
        public string y { get; set; }

        public ConstructorArgTypesDefault()
        {
        }

        public ConstructorArgTypesDefault(int x, string y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class NoDefaultCtor
    {
        public string StringProperty { get; set; }

        public NoDefaultCtor(string str)
        {
            this.StringProperty = str;
        }
    }

    public class Base1
    {
        public string BaseString { get; set; }
    }

    public class Derived1 : Base1
    {
        public string DerivedString { get; set; }
    }

    public class ConstructorArgTypesDerived
    {
        [ConstructorArgument("x")]
        public Base1 x { get; set; }

        public ConstructorArgTypesDerived(Base1 x)
        {
            this.x = x;
        }
    }

    public class CtorDirectiveClass
    {
        public string Data { get; set; }

        public CtorDirectiveClass() { }
        public CtorDirectiveClass(string data) { Data = data; }

        public static CtorDirectiveClass Factory() { return new CtorDirectiveClass(); }
        public static CtorDirectiveClass Factory(string data) { return new CtorDirectiveClass(data); }
    }
}
