// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
* Description:  Test TypeConverter Inheritance. 
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
    public class TestTypeConverterInheritance
    {
        /// <summary>
        /// The main entry for the harness to call for a xaml file 
        /// to be used to test the serialization.
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCaseArea(@"Serialization\TypeConverter")]
        [TestCasePriority("1")]
        [TestCaseSupportFile("TestTypeConverterInheritance.xaml")]
        public void RunTestCase()
        {
            InitializeCase();
            DoTheTest("TestTypeConverterInheritance.xaml");
        }

        /// <summary>
        /// To initialize Logger, and to add _OnXamlSerilaized event handler.
        /// </summary>
        protected void InitializeCase()
        {
            _serhelper = new SerializationHelper();
            _serhelper.XamlSerialized += new XamlSerializedEventHandler(OnXamlSerialized);

            if (File.Exists(_tempXamlFile))
            {
                File.Delete(_tempXamlFile);
            }

            if (File.Exists(_tempXamlFile2))
            {
                File.Delete(_tempXamlFile2);
            }
        }


        /// <summary>
        /// Calls RoundTripTest() on SerializationHelper.
        /// </summary> 
        /// <param name="fileName">xaml file name</param>
        virtual protected void DoTheTest(String fileName)
        {
            _serhelper.RoundTripTestFile(fileName, XamlWriterMode.Expression, true);
        }

        /// <summary>
        /// Logs round trip status messages to CoreLogger.
        /// </summary>
        protected void OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            string xamlFile = _tempXamlFile;

            // Save xaml to file for potential debugging.
            if (File.Exists(_tempXamlFile))
            {
                xamlFile = _tempXamlFile2;
            }

            IOHelper.SaveTextToFile(args.Xaml, xamlFile);
            VerifySerialized(args.Xaml);
        }

        private void VerifySerialized(string serialized)
        {
            //Test NewCustomElementWithNewTypeConverterClr count, the number of 
            // times NewTypeConverter is used. 
            VerifyAppearance(serialized, "NewCustomElementWithNewTypeConverterClr", 2);
            
            //Verify BaseInheritTypeNoTypeConverterClr count
            VerifyAppearance(serialized, "BaseInheritTypeNoTypeConverterClr", 3);
            
            //Verify BaseInheritTypeNoTypeConverterClr count
            VerifyAppearance(serialized, "NewCustomElementWithNewTypeConverterDP", 3);
            //Verify BaseInheritTypeNoTypeConverterClr count
            VerifyAppearance(serialized, "BaseInheritTypeNoTypeConverterDP", 3);
            //Verify BaseInheritTypeNoTypeConverterClr count
            VerifyAppearance(serialized, "New1ustomElementWithNewTypeConverterClr", 1);            
        }
        void VerifyAppearance(string original, string pattern, int count)
        {
                        
            string temp =original;
            int appearance = 0;
            while (temp.IndexOf(pattern) != -1)
            {
                int index = temp.IndexOf(pattern);
                temp = temp.Substring(index + 5);
                appearance++;
            }
            if (appearance != count)
            {
                throw new Microsoft.Test.TestValidationException("Should have serialized " + pattern + " times.");
            }            
        }

        #region Variables
        /// <summary>
        /// Provides common serialization test functions.
        /// </summary>
        SerializationHelper _serhelper;

        /// <summary>
        /// File name for the xaml file serialized from the first tree.
        /// </summary>
        string _tempXamlFile = "___SerializationTempFile.xaml";
        /// <summary>
        /// File name for the xaml file serialized from the second tree.
        /// </summary>
        string _tempXamlFile2 = "___SerializationTempFile2.xaml";
        #endregion Variables
    }       
    /// <summary>
    /// 
    /// </summary>
    public class InheritedCustomElementWithPropertiesOfTypeWithTypeConverter2 : CustomElementWithPropertiesOfTypeWithTypeConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InheritedCustomElementWithPropertiesOfTypeWithTypeConverter2()
            : base()
        {
        }

        /// <summary>
        ///  Clr property of type CustomElementWithNewTypeConverter.
        /// </summary>
        public override CustomElementWithNewTypeConverter CustomElementWithNewTypeConverterClr
        {
            get
            {
                return _CustomElementWithNewTypeConverterClr2;
            }
            set
            {
                _CustomElementWithNewTypeConverterClr2 = value;
            }
        }
        private CustomElementWithNewTypeConverter _CustomElementWithNewTypeConverterClr2;
    }
    /// <summary>
    /// 
    /// </summary>
    public class InheritedCustomElementWithPropertiesOfTypeWithTypeConverter1 : CustomElementWithPropertiesOfTypeWithTypeConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InheritedCustomElementWithPropertiesOfTypeWithTypeConverter1()
            : base()
        {
        }

        
        /// <summary>
        ///  Clr property of type CustomElementWithNewTypeConverter.
        /// </summary>
        [TypeConverter(typeof(New1TypeConverter))]
        public override CustomElementWithNewTypeConverter CustomElementWithNewTypeConverterClr
        {
            get
            {
                return _CustomElementWithNewTypeConverterClr1;
            }
            set
            {
                _CustomElementWithNewTypeConverterClr1 = value;
            }
        }
        private CustomElementWithNewTypeConverter _CustomElementWithNewTypeConverterClr1;
    }
    /// <summary>
    /// 
    /// </summary>
    public class CustomElementWithPropertiesOfTypeWithTypeConverter : ContentControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomElementWithPropertiesOfTypeWithTypeConverter()
            : base()
        {
        }
        #region CustomElementWithNewTypeConverterDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static CustomElementWithNewTypeConverter GetCustomElementWithNewTypeConverterDP(DependencyObject e)
        {
            return e.GetValue(CustomElementWithNewTypeConverterDPProperty) as CustomElementWithNewTypeConverter;
        }

        /// <summary>
        /// Gettor for CustomElementWithNewTypeConverterDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetCustomElementWithNewTypeConverterDP(DependencyObject e, CustomElementWithNewTypeConverter myProperty)
        {
            e.SetValue(CustomElementWithNewTypeConverterDPProperty, myProperty);
        }
        /// <summary>
        ///  Depedency Property of type CustomElementNoneNoneInvalidType.
        /// </summary>
        public static DependencyProperty CustomElementWithNewTypeConverterDPProperty =
            DependencyProperty.Register("CustomElementWithNewTypeConverterDP", typeof(CustomElementWithNewTypeConverter), typeof(CustomElementWithPropertiesOfTypeWithTypeConverter));
        #endregion CustomElementWithNewTypeConverterDP
        

        #region CustomElementWithNewTypeConverterClr
        /// <summary>
        ///  Clr property of type CustomElementWithNewTypeConverter.
        /// </summary>
        public virtual CustomElementWithNewTypeConverter CustomElementWithNewTypeConverterClr
        {
            get
            {
                return _CustomElementWithNewTypeConverterClr;
            }
            set
            {
                _CustomElementWithNewTypeConverterClr = value;
            }
        }
        private CustomElementWithNewTypeConverter _CustomElementWithNewTypeConverterClr;
        #endregion
        #region InheritTypeNoTypeConverterDP
        /// <summary>
        /// Settor for CustomElementNoneNoneInvalidTypeDP.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static InheritTypeNoTypeConverter GetInheritTypeNoTypeConverterDP(DependencyObject e)
        {
            return e.GetValue(InheritTypeNoTypeConverterDPProperty) as InheritTypeNoTypeConverter;
        }

        /// <summary>
        /// Gettor for InheritTypeNoTypeConverterDP.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="myProperty"></param>
        public static void SetInheritTypeNoTypeConverterDP(DependencyObject e, InheritTypeNoTypeConverter myProperty)
        {
            e.SetValue(InheritTypeNoTypeConverterDPProperty, myProperty);
        }
        /// <summary>
        ///  Depedency Property of type CustomElementNoneNoneInvalidType.
        /// </summary>
        public static DependencyProperty InheritTypeNoTypeConverterDPProperty =
            DependencyProperty.Register("InheritTypeNoTypeConverterDP", typeof(InheritTypeNoTypeConverter), typeof(CustomElementWithPropertiesOfTypeWithTypeConverter));
       #endregion InheritTypeNoTypeConverterDP


        #region InheritTypeNoTypeConverterClr
        /// <summary>
        ///  Clr property of type InheritTypeNoTypeConverter.
        /// </summary>
        public InheritTypeNoTypeConverter InheritTypeNoTypeConverterClr
        {
            get
            {
                return _InheritTypeNoTypeConverterClr;
            }
            set
            {
                _InheritTypeNoTypeConverterClr = value;
            }
        }
        private InheritTypeNoTypeConverter _InheritTypeNoTypeConverterClr;
        #endregion
    }
    /// <summary>
    /// CustomElementWithNewTypeConverter
    /// </summary>
    [TypeConverter(typeof(NewTypeConverter))]
    public class CustomElementWithNewTypeConverter : CustomElementWithBaseTypeConverter
    {
        /// <summary>
        /// Default constructor for CustomElementWithNewTypeConverter.
        /// </summary>
        public CustomElementWithNewTypeConverter()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidTypeConverter.
        /// </summary>
        public CustomElementWithNewTypeConverter(string value)
            : base(value)
        {
            
        }
        
    }
    #endregion
    #region InheritTypeNoTypeConverter
    /// <summary>
    /// InheritTypeNoTypeConverter
    /// </summary>
    public class InheritTypeNoTypeConverter : CustomElementWithBaseTypeConverter
    {
        /// <summary>
        /// Default constructor for CustomElementWithInvalidTypeConverter.
        /// </summary>
        public InheritTypeNoTypeConverter()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidTypeConverter.
        /// </summary>
        public InheritTypeNoTypeConverter(string value) : base(value)
        {
            
        }
    }
    #endregion

    #region CustomElementWithBaseTypeConverter
    /// <summary>
    /// CustomElementWithBaseTypeConverter.
    /// </summary>
    [TypeConverter(typeof(BaseTypeConverter))]
    public class CustomElementWithBaseTypeConverter : DependencyObject
    {
        /// <summary>
        /// Default constructor for CustomElementWithBaseTypeConverter.
        /// </summary>
        public CustomElementWithBaseTypeConverter()
        {
        }
        /// <summary>
        /// Constructor for CustomElementWithInvalidTypeConverter.
        /// </summary>
        public CustomElementWithBaseTypeConverter(string value)
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
            DependencyProperty.Register("Value", typeof(String), typeof(CustomElementWithBaseTypeConverter));
    }
    #endregion
    #region BaseTypeConverter
    /// <summary>
    /// A typeconverter that add Base before its Value. 
    /// </summary>
    public class BaseTypeConverter : TypeConverter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(String))
                throw new Exception("Can only convert from string");
            return "Base" + ((CustomElementWithBaseTypeConverter)value).Value;
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            string valueString = ((string)value).Substring(4);
            return new InheritTypeNoTypeConverter(valueString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(
           ITypeDescriptorContext context,
           Type sourceType
        )
        {
            if (sourceType.Equals(typeof(String)))
                return true;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
                return true;
            return false;
        }
    }
    #endregion
    #region New1TypeConverter
    /// <summary>
    /// A typeconverter that add New1 before its Value. 
    /// </summary>
    public class New1TypeConverter : TypeConverter
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(String))
                throw new Exception("Can only convert from string");
            return "New1" + ((CustomElementWithNewTypeConverter)value).Value;
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            string valueString = ((string)value).Substring(4);
            return new CustomElementWithNewTypeConverter(valueString);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(
           ITypeDescriptorContext context,
           Type sourceType
        )
        {
            if (sourceType.Equals(typeof(String)))
                return true;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
                return true;
            return false;
        }
    }
    #endregion
    #region NewTypeConverter
    /// <summary>
    /// A typeconverter that add New before its Value. 
    /// </summary>
    public class NewTypeConverter : TypeConverter
    {

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(String))
                throw new Exception("Can only convert from string");
            return "New" + ((CustomElementWithNewTypeConverter)value).Value;
        }

        /// <summary>
        /// ConvertFrom
        /// </summary>
        /// <param name="typeDescriptorContext"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext typeDescriptorContext,
                                           CultureInfo cultureInfo,
                                           object value)
        {
            string valueString = ((string)value).Substring(3);
            return new CustomElementWithNewTypeConverter(valueString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(
           ITypeDescriptorContext context,
           Type sourceType
        )
        {
            if (sourceType.Equals(typeof(String)))
                return true;
            return false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType.Equals(typeof(String)))
                return true;
            return false;
        }
    }
    #endregion

}
