// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
* Description: Test ValueSerializer Inheritance. 
* Owner: Microsoft
*
 
  
* Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    #region 
    /// <summary>
    /// 
    /// </summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TestValueSerializerInheritance
    {
        /// <summary>
        /// The main entry for the harness to call for a xaml file 
        /// to be used to test the serialization.
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCaseArea(@"Serialization\ValueSerializer")]
        [TestCasePriority("1")]
        public void RunTestCase()
        {
            object root = ConstructTree1();
            string serialized = SerializationHelper.SerializeObjectTree(root);
            VerifySerialized1(serialized);
            root = ConstructTree2();
            serialized = SerializationHelper.SerializeObjectTree(root);
            VerifySerialized2(serialized);
            root = ConstructTree3();
            serialized = SerializationHelper.SerializeObjectTree(root);
            VerifySerialized3(serialized);
        }
        object ConstructTree1()
        {
            CustomElementWithPropertiesOfTypeWithValueSerializer control = new CustomElementWithPropertiesOfTypeWithValueSerializer();
            control.CustomElementWithNewValueSerializerClr = new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerClr");
            control.InheritTypeNoValueSerializerClr = new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerClr");
            control.SetValue(CustomElementWithPropertiesOfTypeWithValueSerializer.CustomElementWithNewValueSerializerDPProperty,
                new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerDP"));
            control.SetValue(CustomElementWithPropertiesOfTypeWithValueSerializer.InheritTypeNoValueSerializerDPProperty,
                new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerDP"));
            return control;
        }
        void VerifySerialized1(string serialized)
        {
            if(-1 == serialized.IndexOf("NewCustomElementWithNewValueSerializerClr")
                ||-1 ==  serialized.IndexOf("BaseInheritTypeNoValueSerializerClr")
                ||-1 ==  serialized.IndexOf("NewCustomElementWithNewValueSerializerDP")
                ||-1 == serialized.IndexOf("BaseInheritTypeNoValueSerializerDP"))
                throw new Microsoft.Test.TestValidationException("Wong Value serializer used for CustomElementWithPropertiesOfTypeWithValueSerializer.");
            return;
        }


        object ConstructTree2()
        {
            InheritedCustomElementWithPropertiesOfTypeWithValueSerializer1 control = new InheritedCustomElementWithPropertiesOfTypeWithValueSerializer1();
            control.CustomElementWithNewValueSerializerClr = new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerClr");
            control.InheritTypeNoValueSerializerClr = new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerClr");
            control.SetValue(InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2.CustomElementWithNewValueSerializerDPProperty,
                new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerDP"));
            control.SetValue(InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2.InheritTypeNoValueSerializerDPProperty,
                new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerDP"));
            return control;
        }
        void VerifySerialized2(string serialized)
        {
            if (-1 == serialized.IndexOf("New1CustomElementWithNewValueSerializerClr")
                || -1 == serialized.IndexOf("BaseInheritTypeNoValueSerializerClr")
                || -1 == serialized.IndexOf("NewCustomElementWithNewValueSerializerDP")
                || -1 == serialized.IndexOf("BaseInheritTypeNoValueSerializerDP"))
                throw new Microsoft.Test.TestValidationException("Wong Value serializer used for CustomElementWithPropertiesOfTypeWithValueSerializer1.");
            return;
        }

        object ConstructTree3()
        {
            InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2 control = new InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2();
            control.CustomElementWithNewValueSerializerClr = new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerClr");
            control.InheritTypeNoValueSerializerClr = new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerClr");
            control.SetValue(InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2.CustomElementWithNewValueSerializerDPProperty,
                new CustomElementWithNewValueSerializer("CustomElementWithNewValueSerializerDP"));
            control.SetValue(InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2.InheritTypeNoValueSerializerDPProperty,
                new InheritTypeNoValueSerializer("InheritTypeNoValueSerializerDP"));
            return control;
        }
        void VerifySerialized3(string serialized)
        {
            if (-1 == serialized.IndexOf("NewCustomElementWithNewValueSerializerClr")
                || -1 == serialized.IndexOf("BaseInheritTypeNoValueSerializerClr")
                || -1 == serialized.IndexOf("NewCustomElementWithNewValueSerializerDP")
                || -1 == serialized.IndexOf("BaseInheritTypeNoValueSerializerDP"))
                throw new Microsoft.Test.TestValidationException("Wong Value serializer used for InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2.");
            return;
        }
    }       
    /// <summary>
    /// 
    /// </summary>
    public class InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2 : CustomElementWithPropertiesOfTypeWithValueSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InheritedCustomElementWithPropertiesOfTypeWithValueSerializer2()
            : base()
        {
        }

        /// <summary>
        ///  Clr property of type CustomElementWithNewValueSerializer.
        /// </summary>
        public override CustomElementWithNewValueSerializer CustomElementWithNewValueSerializerClr
        {
            get
            {
                return _CustomElementWithNewValueSerializerClr2;
            }
            set
            {
                _CustomElementWithNewValueSerializerClr2 = value;
            }
        }
        private CustomElementWithNewValueSerializer _CustomElementWithNewValueSerializerClr2;
    }
    /// <summary>
    /// 
    /// </summary>
    public class InheritedCustomElementWithPropertiesOfTypeWithValueSerializer1 : CustomElementWithPropertiesOfTypeWithValueSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InheritedCustomElementWithPropertiesOfTypeWithValueSerializer1()
            : base()
        {
        }

        
        /// <summary>
        ///  Clr property of type CustomElementWithNewValueSerializer.
        /// </summary>
        [ValueSerializer(typeof(New1ValueSerializer))]
        public override CustomElementWithNewValueSerializer CustomElementWithNewValueSerializerClr
        {
            get
            {
                return _CustomElementWithNewValueSerializerClr1;
            }
            set
            {
                _CustomElementWithNewValueSerializerClr1 = value;
            }
        }
        private CustomElementWithNewValueSerializer _CustomElementWithNewValueSerializerClr1;
    }
    /// <summary>
    /// 
    /// </summary>
    public class CustomElementWithPropertiesOfTypeWithValueSerializer : ContentControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomElementWithPropertiesOfTypeWithValueSerializer()
            : base()
        {
        }
        #region CustomElementWithNewValueSerializerDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementWithNewValueSerializer GetCustomElementWithNewValueSerializerDP(DependencyObject e)
        {
            return e.GetValue(CustomElementWithNewValueSerializerDPProperty) as CustomElementWithNewValueSerializer;
        }

        /// <summary>
        /// Gettor for CustomElementWithNewValueSerializerDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementWithNewValueSerializerDP(DependencyObject e, CustomElementWithNewValueSerializer myProperty)
        {
            e.SetValue(CustomElementWithNewValueSerializerDPProperty, myProperty);
        }
        /// <summary>
        ///  Depedency Property of type CustomElementNoneNoneInvalidType.
        /// </summary>
        public static DependencyProperty CustomElementWithNewValueSerializerDPProperty =
            DependencyProperty.Register("CustomElementWithNewValueSerializerDP", typeof(CustomElementWithNewValueSerializer), typeof(CustomElementWithPropertiesOfTypeWithValueSerializer));
        #endregion CustomElementWithNewValueSerializerDP
        

        #region CustomElementWithNewValueSerializerClr
        /// <summary>
        ///  Clr property of type CustomElementWithNewValueSerializer.
        /// </summary>
        public virtual CustomElementWithNewValueSerializer CustomElementWithNewValueSerializerClr
        {
            get
            {
                return _CustomElementWithNewValueSerializerClr;
            }
            set
            {
                _CustomElementWithNewValueSerializerClr = value;
            }
        }
        private CustomElementWithNewValueSerializer _CustomElementWithNewValueSerializerClr;
        #endregion
        #region InheritTypeNoValueSerializerDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static InheritTypeNoValueSerializer GetInheritTypeNoValueSerializerDP(DependencyObject e)
        {
            return e.GetValue(InheritTypeNoValueSerializerDPProperty) as InheritTypeNoValueSerializer;
        }

        /// <summary>
        /// Gettor for InheritTypeNoValueSerializerDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetInheritTypeNoValueSerializerDP(DependencyObject e, InheritTypeNoValueSerializer myProperty)
        {
            e.SetValue(InheritTypeNoValueSerializerDPProperty, myProperty);
        }
        /// <summary>
        ///  Depedency Property of type CustomElementNoneNoneInvalidType.
        /// </summary>
        public static DependencyProperty InheritTypeNoValueSerializerDPProperty =
            DependencyProperty.Register("InheritTypeNoValueSerializerDP", typeof(InheritTypeNoValueSerializer), typeof(CustomElementWithPropertiesOfTypeWithValueSerializer));
       #endregion InheritTypeNoValueSerializerDP


        #region InheritTypeNoValueSerializerClr
        /// <summary>
        ///  Clr property of type InheritTypeNoValueSerializer.
        /// </summary>
        public InheritTypeNoValueSerializer InheritTypeNoValueSerializerClr
        {
            get
            {
                return _InheritTypeNoValueSerializerClr;
            }
            set
            {
                _InheritTypeNoValueSerializerClr = value;
            }
        }
        private InheritTypeNoValueSerializer _InheritTypeNoValueSerializerClr;
        #endregion
    }
    /// <summary>
    /// CustomElementWithNewValueSerializer
    /// </summary>
    [ValueSerializer(typeof(NewValueSerializer))]
    public class CustomElementWithNewValueSerializer : CustomElementWithBaseValueSerializer
    {
        /// <summary>
        /// Default constructor for CustomElementWithNewValueSerializer.
        /// </summary>
        public CustomElementWithNewValueSerializer()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidValueSerializer.
        /// </summary>
        public CustomElementWithNewValueSerializer(string value)
            : base(value)
        {
            
        }
        
    }
    #endregion
    #region InheritTypeNoValueSerializer
    /// <summary>
    /// InheritTypeNoValueSerializer
    /// </summary>
    public class InheritTypeNoValueSerializer : CustomElementWithBaseValueSerializer
    {
        /// <summary>
        /// Default constructor for CustomElementWithInvalidValueSerializer.
        /// </summary>
        public InheritTypeNoValueSerializer()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidValueSerializer.
        /// </summary>
        public InheritTypeNoValueSerializer(string value) : base(value)
        {
            
        }
    }
    #endregion

    #region CustomElementWithBaseValueSerializer
    /// <summary>
    /// CustomElementWithBaseValueSerializer.
    /// </summary>
    [ValueSerializer(typeof(BaseValueSerializer))]
    public class CustomElementWithBaseValueSerializer : DependencyObject
    {
        /// <summary>
        /// Default constructor for CustomElementWithBaseValueSerializer.
        /// </summary>
        public CustomElementWithBaseValueSerializer()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidValueSerializer.
        /// </summary>
        public CustomElementWithBaseValueSerializer(string value)
        {
            Value = value;
        }
        /// <summary>
        /// Clr Accesser for ValueProperty
        /// </summary>
        public virtual String Value
        {
            get
            {
                return GetValue(ValueProperty) as string;
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        /// <summary>
        ///  A string property used to reflect the change caused by Triggers.
        /// </summary>
        public static DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(String), typeof(CustomElementWithBaseValueSerializer));
    }
    #endregion
    #region BaseValueSerializer
    /// <summary>
    /// A ValueSerializer that add Base before its Value. 
    /// </summary>
    public class BaseValueSerializer : ValueSerializer
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return "Base" + ((CustomElementWithBaseValueSerializer)value).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            string valueString = ((string)value).Substring(4);
            return new InheritTypeNoValueSerializer(valueString);
        }
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
        /// Whether the value can be converted from a string.
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="context">Context</param>
        /// <returns>true, it can convert, false otherwise.</returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }
    }
    #endregion
    #region New1ValueSerializer
    /// <summary>
    /// A ValueSerializer that add New1 before its Value. 
    /// </summary>
    public class New1ValueSerializer : BaseValueSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return "New1" + ((CustomElementWithBaseValueSerializer)value).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            string valueString = ((string)value).Substring(4);
            return new InheritTypeNoValueSerializer(valueString);
        } 
    }
    #endregion
    #region NewValueSerializer
    /// <summary>
    /// A ValueSerializer that add New before its Value. 
    /// </summary>
    public class NewValueSerializer : BaseValueSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            return "New" + ((CustomElementWithBaseValueSerializer)value).Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            string valueString = ((string)value).Substring(3);
            return new InheritTypeNoValueSerializer(valueString);
        }
    }
    #endregion

}
