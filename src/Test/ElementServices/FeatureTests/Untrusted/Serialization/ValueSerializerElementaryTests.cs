// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Parameter Validation and Negitive test for class ValueSerializer.
 
  
 * Revision:         $Revision: 1 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
namespace Avalon.Test.CoreUI.Serialization.Converter
{
    /// <summary>
    /// Parameter Validation and Nagitive Tests for ValueSerializer. 
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ValueSerializerElementaryTests
    {
        /// <summary>
        /// Basic Verification tests for ValueSerializer. Verify that correct
        /// ValueSerializer returned with various paramater(s).
        /// </summary>
        [TestCase("0", @"Serialization\ValueSerializer\Function", "BVTs fro ValueSerializer.")]
        public void ValueSerializerBasicTest()
        {
            GetSerializerForType();
            GetSerializerForTypeAndContext();
            GetSerializerForTypeAndDescriptor();
        }
        /// <summary>
        /// Parameter validation for ValueSerialier.
        /// </summary>
        [TestCase("1", @"Serialization\ValueSerializer\Validation", "Parameter Validation and some basic function tests")]
        public void ValueSerializerParameterValidation()
        {
            ParameterValidationForGetSerializerFor();
            ParameterValidationValueSerializerInstanceMethods();
        }

        /// <summary>
        /// Parameter validation for ValueSerialier.GetSerializerFor().
        /// </summary>
        void ParameterValidationForGetSerializerFor()
        {
            bool gotCorrectException = false;
            try
            {
                ValueSerializer.GetSerializerFor((Type)null);
            }
            catch (ArgumentNullException)
            {
                gotCorrectException = true;
            }
            if (!gotCorrectException)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(null as Type) should have thrown a ArgumentNullException");
            }

            gotCorrectException = false;
            try
            {
                ValueSerializer.GetSerializerFor((PropertyDescriptor)null);
            }
            catch (ArgumentNullException)
            {
                gotCorrectException = true;
            }
            if (!gotCorrectException)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(null as PropertyDescriptor) should have thrown a ArgumentNullException");
            }
        }
        /// <summary>
        /// Test ValueSerializer.GetSerializerFor(Type).
        /// </summary>
        void GetSerializerForType()
        {
            //Custom ValueSerializer
            ValueSerializer serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementTypeBothNoNone));
            if (!(serializer is ValueSerializer2))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementTypeBothNoNone)) should return ValueSerializer2.");
            }

            Type sertype = typeof(ValueSerializer2);
            ValueSerializerAttribute attrib = TypeDescriptor.GetAttributes(typeof(CustomElementTypeBothNoNone))[typeof(ValueSerializerAttribute)] as ValueSerializerAttribute;
            if (attrib.ValueSerializerType != sertype || attrib.ValueSerializerTypeName != sertype.AssemblyQualifiedName)
            {
                throw new Microsoft.Test.TestValidationException("ValueSerializerAttribute API returns unexpected values.");
            }

            //with TypeConverter only, TypeConverterValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementNoneNoneValidType));
            if (serializer != null && !(serializer is ValueSerializer2))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementNoneNoneValidType)) should return TypeConverterValueSerializer.");
            }
            //No ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementNoneNoneNoNone));
            if (null != serializer)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementNoneNoneNoNone)) should not return a ValueSerializer" + serializer.ToString());
            }
        }

        /// <summary>
        /// Test ValueSerializer.GetSerializerFor(Type, Context).
        /// </summary>
        void GetSerializerForTypeAndContext()
        {
            IValueSerializerContext context = new MyIValueSerializerContext();

            //Context is not null, and there is a ValueSerializer
            ValueSerializer serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementTypeBothNoNone), context);
            if (!(serializer is ValueSerializer2))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementTypeBothNoNone), context) should return ValueSerializer2.");
            }

            //Context is null, and there is a ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementTypeBothNoNone), null);
            if (!(serializer is ValueSerializer2))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementTypeBothNoNone), null) should return ValueSerializer2.");
            }

            //Context is not null, and there is no ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementNoneNoneNoNone), context);
            if (serializer != null)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementNoneNoneNoNone), context) should return null.");
            }

            //Context is null, and there is no ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementNoneNoneNoNone), null);
            if (serializer != null)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(typeof(CustomElementNoneNoneNoNone), null) should return null.");
            }
        }

        /// <summary>
        /// Test ValueSerializer.GetSerializerFor(Type, Context).
        /// </summary>
        void GetSerializerForTypeAndDescriptor()
        {
            IValueSerializerContext context = new MyIValueSerializerContext();

            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(CustomType).TypeHandle);
            CustomType element = new CustomType();

            PropertyDescriptorCollection descriptions = TypeDescriptor.GetProperties(element);
            PropertyDescriptor descriptor = descriptions["CustomElementTypeBothNoNoneDP"];

            //Context is not null, and there is a ValueSerializer
            ValueSerializer serializer = ValueSerializer.GetSerializerFor(descriptor, context);
            if (!(serializer.GetType().Equals(typeof(ValueSerializer2))))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(CustomElementTypeBothNoNoneDP, context) should return ValueSerializer1.");
            }

            //Context is null, and there is a ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(descriptor, null);
            if (!(serializer.GetType().Equals(typeof(ValueSerializer2))))
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(CustomElementTypeBothNoNoneDP, null) should return ValueSerializer1.");
            }

            //Context is not null, and there is no ValueSerializer
            descriptor = TypeDescriptor.GetProperties(element)["CustomElementNoneNoneNoNoneClr"];

            serializer = ValueSerializer.GetSerializerFor(descriptor, context);
            if (serializer != null)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(descriptor, context) should return null.");
            }

            //Context is null, and there is no ValueSerializer
            serializer = ValueSerializer.GetSerializerFor(typeof(CustomElementNoneNoneNoNone), null);
            if (serializer != null)
            {
                throw new Microsoft.Test.TestValidationException("GetSerializerFor(descriptor, null) should return null.");
            }
        }

        /// <summary>
        /// Parameter validation for ValueSerialier instance methods.
        /// </summary>
        void ParameterValidationValueSerializerInstanceMethods()
        {
            BasicValueSerializer serializer = new BasicValueSerializer();
            IValueSerializerContext context = new MyIValueSerializerContext();
            CustomType element = new CustomType();
            if (serializer.CanConvertFromString((string)null, context)
                || serializer.CanConvertToString(element, context))
            {
                throw new Microsoft.Test.TestValidationException("Base ValueSerializer cannot convert.");
            }

            bool expected = false;
            try
            {
                serializer.ConvertToString(null, context);
            }
            catch (NotSupportedException)
            {
                expected = true;
            }
            if (!expected)
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException expected.");
            }

            expected = false;
            try
            {
                serializer.ConvertFromString(null, context);
            }
            catch (NotSupportedException)
            {
                expected = true;
            }
            if (!expected)
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException expected.");
            }

            expected = false;
            try
            {
                serializer.ConvertToString(element, context);
            }
            catch (NotSupportedException)
            {
                expected = true;
            }
            if (!expected)
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException expected.");
            }

            expected = false;
            try
            {
                serializer.ConvertFromString("something", context);
            }
            catch (NotSupportedException)
            {
                expected = true;
            }
            if (!expected)
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException expected.");
            }
        }
        /// <summary>
        /// Nagitive test for ValueSerializer.
        /// </summary>
        [TestCase("1", @"Serialization\ValueSerializer\Nagative", "")]
        public void TestInValidValueSerializer()
        {
            bool expected = true;
            object element = null;
            // ValueSerializer with ConvertToString return null should not cause NullReferenceException, blocked on 
            ExceptionSerializingAType(typeof(CustomTypeWithInValidValueSerializer1), true, typeof(NullReferenceException), false);

            //Exception thrown in ConvertToString should be handled, blocked on 
            ExceptionSerializingAType(typeof(CustomTypeWithInValidValueSerializer2), true, typeof(NotImplementedException), false);

            //Verify that if the CanConvertToString return false, 
            //ValueSerializer won't be used.
            ExceptionSerializingAType(typeof(CustomTypeWithInValidValueSerializer3), false, null, true);

            // Verify that If the ValueSerializer specified with ValueSerializerAttribute is not of 
            // expected type, this attribute is ignored.  
            //Blocked on 
            ExceptionSerializingAType(typeof(CustomElementWithNonValueSerializerTypeAsValueSerializer), false, null, true);

            //Verify that exception thrown in GetReferences be handled.            
            ExceptionSerializingAType(typeof(CustomTypeWithInValidValueSerializer4), true, typeof(NotImplementedException), false);
        }
        /// <summary>
        /// Verify that serializing an object of type elementType should get an 
        /// exception of(or not of) type exceptionType. 
        /// </summary>
        /// <param name="elementType">Element type</param>
        /// <param name="IsExpectingException">Should there be an exception thrown?</param>
        /// <param name="exceptionType">Expected Exception type</param>
        /// <param name="IsExpected">Is the type of exception expected</param>
        void ExceptionSerializingAType(Type elementType, bool IsExpectingException, Type exceptionType, bool IsExpected)
        {
            bool expected = true;
            object element = null;
            bool gotException = false;

            element = Activator.CreateInstance(elementType);
            try
            {
                SerializationHelper.SerializeObjectTree(element);
            }
            catch (Exception e)
            {
                gotException = true;
                if (!IsExpectingException)
                {
                    expected = false;
                }
                else if (e.GetType().Equals(exceptionType) != IsExpected)
                {
                        expected = false;
                }
                CoreLogger.LogStatus("Got an exception: " + e.GetType().Name + ".");
            }
            
            if(IsExpectingException != gotException)
            {
                throw new Microsoft.Test.TestValidationException("Expecting an exception: " + exceptionType.Name + ", but not exception thrown.");
            }

            if (!expected)
            {
                if (!IsExpectingException)
                {
                    throw new Microsoft.Test.TestValidationException("Should not get an exception.");
                }
                else
                {
                    throw new Microsoft.Test.TestValidationException(IsExpected? "E" : "Not e" + "xpecting exception: " + exceptionType.Name + ".");
                }
            }
        }
    }
    #region Custom Type and custom ValueSerializer
    #region Custom with ValueSerializer whose GetReferences throws.
    /// <summary>
    /// Custom with ValueSerializer with TypeReferences() throws an exception
    /// </summary>
    [ValueSerializer(typeof(InValidValueSerializer4))]
    public class CustomTypeWithInValidValueSerializer4 : FrameworkElement
    {
    }
    /// <summary>
    /// ValueSerializer with TypeReferences() throws an exception
    /// </summary>
    public class InValidValueSerializer4 : ValueSerializer
    {
        /// <summary>
        /// Whether this value can be converted to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return "a string";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerable<Type> TypeReferences(object value, IValueSerializerContext context)
        {
            throw new NotImplementedException("not implement TypeReferences.");
        }
    }
    #endregion 

    #region Custom with ValueSerializer whose ConvertToString return null
    /// <summary>
    /// Custom with ValueSerializer with ConvertToString return null
    /// </summary>
    [ValueSerializer(typeof(InValidValueSerializer1))]
    public class CustomTypeWithInValidValueSerializer1 : FrameworkElement
    {
    }
    /// <summary>
    /// ValueSerializer the ConvertToString return null.
    /// </summary>
    public class InValidValueSerializer1 : ValueSerializer
    {
        /// <summary>
        /// Whether this value can be converted to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            //throw new NotImplementedException("Not Implemented.");
            return null;
        }
    }
    #endregion 

    #region Custom ValueSerializer with ConvertToString return null
    /// <summary>
    /// Custom with ValueSerializer with ConvertToString throw an exception.
    /// </summary>
    [ValueSerializer(typeof(InValidValueSerializer2))]
    public class CustomTypeWithInValidValueSerializer2 : FrameworkElement
    {
    }
    /// <summary>
    /// ValueSerializer the ConvertToString throw a NotImplementedException.
    /// </summary>
    public class InValidValueSerializer2 : InValidValueSerializer1
    {

        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>converted string</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            throw new NotImplementedException("Not Implemented.");
        }
    }
    #endregion 

    #region Custom ValueSerializer with Can ConvertToString return false;
    /// <summary>
    /// Custom with ValueSerializer with ConvertToString return null
    /// </summary>
    [ValueSerializer(typeof(InValidValueSerializer3))]
    public class CustomTypeWithInValidValueSerializer3 : FrameworkElement
    {
    }
    /// <summary>
    /// ValueSerializer the ConvertToString return null.
    /// </summary>
    public class InValidValueSerializer3 : InValidValueSerializer2
    {
        /// <summary>
        /// Whether this value can be converted to a string.
        /// </summary>
        /// <param name="value">The value being serialized</param>
        /// <param name="context">Context information</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return false;
        }
    }
    #endregion 

    #region CustomElementWithNonValueSerializerTypeAsValueSerializer;
    /// <summary>
    /// Custom with ValueSerializer with ConvertToString return null
    /// </summary>
    [ValueSerializer(typeof(CustomType))]
    public class CustomElementWithNonValueSerializerTypeAsValueSerializer : FrameworkElement
    {
    }
    #endregion 

    class CustomType : DependencyObject
    {
        public static readonly DependencyProperty CustomElementTypeBothNoNoneDPProperty =
            DependencyProperty.RegisterAttached("Attached", typeof(CustomElementTypeBothNoNone),
            typeof(CustomType),
            new PropertyMetadata());
        [ValueSerializer(typeof(ValueSerializer2))]
        public CustomElementTypeBothNoNone CustomElementTypeBothNoNoneDP
        {
            get
            {
                return GetValue(CustomElementTypeBothNoNoneDPProperty) as CustomElementTypeBothNoNone;
            }
            set
            {
                SetValue(CustomElementTypeBothNoNoneDPProperty, value);
            }
        }

        public CustomElementNoneNoneNoNone CustomElementNoneNoneNoNoneClr
        {
            get
            {
                return _customElementNoneNoneNoNoneClr;
            }
            set
            {
                _customElementNoneNoneNoNoneClr = value;
            }
        }
        CustomElementNoneNoneNoNone _customElementNoneNoneNoNoneClr = null;
    }
    /// <summary>
    /// Custom IValueSerializerContext just use ValueSerializer to return Serializers.
    /// </summary>
    public class MyIValueSerializerContext : IValueSerializerContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public ValueSerializer GetValueSerializerFor(PropertyDescriptor descriptor)
        {
                return ValueSerializer.GetSerializerFor(descriptor);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ValueSerializer GetValueSerializerFor(Type type)
        {
                return ValueSerializer.GetSerializerFor(type);
        }

        /// <summary>
        /// 
        /// </summary>
        public IContainer Container
        {
            get { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public object Instance
        {
            get { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnComponentChanged()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool OnComponentChanging()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyDescriptor PropertyDescriptor
        {
            get { return null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
    #endregion 
}
