//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

#region Using Statements

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

using Avalon.Test.ComponentModel.Validations;

#endregion

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// interface for xml parser
    /// </summary>
    public interface IXmlParser
    {
        /// <summary>
        /// parse xmlElement for target
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmlElement"></param>
        void Parse(object target, XmlElement xmlElement);
    }

    /// <summary>
    /// parser for the following format
    /// <code>
    /// [PropertyParser]
    /// public class AClass
    /// {
    ///     public string AProperty { get; set; }
    ///     public DateTime BProperty { get; set; }
    /// }
    /// </code>
    /// xtc code
    /// <code>
    ///     <AClass AProperty="Hello" BProperty="2005-5-1"/>
    /// </code>
    /// It will set AClass.AProperty to "Hello", and set a DateTime(2005,5,1) as its BProperty
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PropertyParserAttribute : Attribute, IXmlParser
    {
        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            XmlParserHelper.ParseProperty(target, xmlElement);
        }

        #endregion
    }

    /// <summary>
    /// parser for the following format
    /// <code>
    /// [XamlContentParser("BProperty")]
    /// public class AClass
    /// {
    ///     public string AProperty { get; set; }
    ///     public object BProperty { get; set; }
    /// }
    /// </code>
    /// xtc code
    /// <code>
    ///     <AClass AProperty="Hello">
    ///         <ListView xmlns="http://schemas.microsoft.com/winfx/avalon/2005"/>
    ///     </AClass>
    /// </code>
    /// It will set AClass.AProperty to "Hello", and set the ListView as its BProperty
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XamlContentParserAttribute : Attribute, IXmlParser
    {
        public XamlContentParserAttribute(string contentProperty)
        {
            if (string.IsNullOrEmpty(contentProperty))
                throw new ArgumentException("content property is empty or null");

            _contentProperty = contentProperty;
        }

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");
            XmlParserHelper.ParseProperty(target, xmlElement);
            if (string.IsNullOrEmpty(xmlElement.InnerXml))
                return;
            XmlParserHelper.ParseContent(target, xmlElement, ContentProperty);
        }

        #endregion

        #region ContentProperty

        public string ContentProperty
        {
            get { return _contentProperty; }
        }

        private string _contentProperty;

        #endregion
    }

    /// <summary>
    /// xml parser for the legacy IAction or IValidation format
    /// the format is the following
    /// <code>
    ///     <WaitAction>
    ///         <TimeToWait Value="0:0:1"/>
    ///     </WaitAction>
    /// </code>.
    /// the parser will parse each value string in each xml element against the corresponding property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LegacyParserAttribute : Attribute, IXmlParser
    {
        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            // verify parameters are correct
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            // get the value string array
            string[] values = PMEUtils.Current.ValuesFromXml(xmlElement);
            if (values.Length > Properties.Length)
                throw new ArgumentException("Too many values. Max value number is " + Properties.Length);

            // parse each string against the corresponding property name
            Type type = target.GetType();
            for (int i = 0; i < values.Length; ++i)
            {
                string property = Properties[i];
                PropertyInfo pi = type.GetProperty(property);
                if (pi == null)
                    throw new ArgumentException("Property " + property + " not exist in " + type);

                object value = XmlHelper.ConvertToType(pi.PropertyType, values[i]);
                pi.SetValue(target, value, new object[0]);
            }
        }

        #endregion

        #region Properties

        public string[] Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        private string[] _properties;

        #endregion
    }

    /// <summary>
    /// wrapper class for IAction
    /// </summary>
    internal sealed class ActionWrapper : ITestStep
    {
        #region Constructor

        public ActionWrapper(IAction action, XmlElement element)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            if (element == null)
                throw new ArgumentNullException("element");
            _action = action;
            _element = element;
        }

        IAction _action;
        XmlElement _element;

        #endregion

        #region ITestStep Members

        public void Do(object testObject)
        {
            if (testObject is FrameworkElement)
                XtcTestHelper.DoAction(_action, (FrameworkElement)testObject, _element);
            else
                throw new ArgumentException("testObject should be a FrameworkElement");
        }

        #endregion
    }

    /// <summary>
    /// wrapper class for IAction
    /// </summary>
    internal sealed class ValidationWrapper : ITestStep
    {
        #region Constructor

        public ValidationWrapper(IValidation valid, XmlElement element)
        {
            if (valid == null)
                throw new ArgumentNullException("valid");
            if (element == null)
                throw new ArgumentNullException("element");
            _valid = valid;
            _element = element;
        }

        IValidation _valid;
        XmlElement _element;

        #endregion

        #region ITestStep Members

        public void Do(object testObject)
        {
            Assert.AssertTrue("Validation failed: " + _valid + " xml: " + _element.LocalName,
                XtcTestHelper.DoValidation(_valid, testObject, _element));
        }

        #endregion
    }

    /// <summary>
    /// It is the parser for the inner property parsing
    /// It can also parse an extra property without explictly specified in xml
    /// It support the following format
    /// <code>
    ///     <DispatcherValidation>
    ///         <!--this if for extra property-->
    ///         <LogicalNameFinder NameToFind="testObject"/>
    ///         <!--this is for the inner property-->
    ///         <Validations>
    ///             <!--every entry should be an ITestStep-->
    ///             <ObjectPropertyValidation>
    ///                 <TextBlock Text="Hello" FontSize="24"/>
    ///             </ObjectPropertyValidation>
    ///         </Validations>
    ///     </DispatcherValidation>
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InnerPropertyParserAttribute : Attribute, IXmlParser
    {
        #region Constructor

        public InnerPropertyParserAttribute(string stepsProperty)
        {
            _stepsProperty = stepsProperty;
        }

        #endregion

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            // verify parameters are correct
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            XmlElement stepsXml = xmlElement[StepsProperty];
            if (stepsXml != null)
            {
                // verify finder and steps properties exist in target's type and are the correct type
                PropertyInfo stepsProperty = target.GetType().GetProperty(StepsProperty);
                if (stepsProperty == null)
                    throw new ArgumentException(StepsProperty + " not exist in target's type " + target.GetType());
                object stepsObject = stepsProperty.GetValue(target, new object[0]);
                if (!(stepsObject is IList<ITestStep>))
                    throw new ArgumentException(StepsProperty + " is not an instance of IList<ITestStep>");

                ParseSteps((IList<ITestStep>)stepsObject, stepsXml);
            }

            if (string.IsNullOrEmpty(ExtraProperty))
                return;

            // find the finder xml element
            XmlElement finderXml = null;
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                XmlElement element = node as XmlElement;
                if (node == null)
                    continue;

                if (StepsProperty != element.Name)
                {
                    finderXml = element;
                    break;
                }
            }

            if (finderXml != null)
            {
                PropertyInfo extraProperty = target.GetType().GetProperty(ExtraProperty);
                if (extraProperty == null)
                    throw new ArgumentException(ExtraProperty + " not exist in target's type " + target.GetType());

                // parse the finder
                object obj = XtcTestHelper.GetObjectFromXml(finderXml);
                if (!(obj is IFinder))
                    throw new ArgumentException("" + obj + " is not an instance of IFinder");
                XmlParserHelper.Parse(obj, finderXml);
                IFinder finder = (IFinder)obj;
                extraProperty.SetValue(target, finder, new object[0]);
            }
        }

        private void ParseSteps(IList<ITestStep> steps, XmlElement stepsElement)
        {
            foreach (XmlNode node in stepsElement.ChildNodes)
            {
                XmlElement element = node as XmlElement;
                if (node == null)
                    continue;

                // parse the element
                object obj = XtcTestHelper.GetObjectFromXml(element);

                // create wrapper object for each type
                if (obj is IAction)
                {
                    ITestStep step = new ActionWrapper((IAction)obj, element);
                    steps.Add(step);
                }
                else if (obj is IValidation)
                {
                    ITestStep step = new ValidationWrapper((IValidation)obj, element);
                    steps.Add(step);
                }
                else
                    throw new ArgumentException("Incorrect object type in the inner xml: " + obj);
            }
        }

        #endregion

        #region ExtraProperty

        /// <summary>
        /// the finder property name
        /// It is defaulted to "Finder"
        /// the parser will parse the first xml element and then set the result to the finder property of the target
        /// </summary>
        public string ExtraProperty
        {
            get { return _extraProperty; }
            set { _extraProperty = value; }
        }

        private string _extraProperty;

        #endregion

        #region StepsProperty

        /// <summary>
        /// the steps property name
        /// It is defaulted to "Steps"
        /// the parser will parse the xml element with name=StepsTag and then add the results into the steps property of the target
        /// the property type should be IList&gt;ITestStep&lt;
        /// </summary>
        public string StepsProperty
        {
            get { return _stepsProperty; }
            set { _stepsProperty = value; }
        }

        private string _stepsProperty;

        #endregion
    }

    /// <summary>
    /// parser for ObjectPropertyValidation
    /// it supports the following format
    /// <code>
    ///     <ObjectPropertyValidation>
    ///         <TextBlock Foreground='Red'/>
    ///     </ObjectPropertyValidation>
    /// </code>
    /// it will parse the TextBlock as the TypeName, and its attributes, Foreground for example to its properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ObjectPropertyParserAttribute : Attribute, IXmlParser
    {
        #region Constructor

        public ObjectPropertyParserAttribute(string tagName, string tagAttributes)
        {
            _tagName = tagName;
            _tagAttributes = tagAttributes;
        }

        private string _tagName;
        private string _tagAttributes;

        #endregion

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            PropertyInfo nameProperty = target.GetType().GetProperty(_tagName);
            if (nameProperty == null)
                throw new ArgumentException(_tagName + " not exist in target's type " + target.GetType());
            PropertyInfo attrsProperty = target.GetType().GetProperty(_tagAttributes);
            if (attrsProperty == null)
                throw new ArgumentException(_tagAttributes + " not exist in target's type " + target.GetType());
            object attrsObject = attrsProperty.GetValue(target, new object[0]);
            if (!(attrsObject is ICollection<Checker>))
                throw new ArgumentException(_tagAttributes + " is not an instance of ICollection<Checker>");

            XmlElement inner = null;
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                XmlElement element = node as XmlElement;
                if (node != null)
                {
                    inner = (XmlElement)node;
                    break;
                }
            }
            if (inner == null)
                throw new ArgumentException("inner xml element is null");

            nameProperty.SetValue(target, inner.Name, new object[0]);

            ICollection<Checker> properties = (ICollection<Checker>)attrsObject;
            foreach (XmlAttribute attr in inner.Attributes)
            {
                properties.Add(new Checker(attr.Name, attr.Value));
            }
        }

        #endregion
    }

    /// <summary>
    /// the parser supports the following format
    /// <code>
    ///     <MyClass MyProperty="MyValue">
    ///         <OtherClass OtherProperty='OtherValue'/>
    ///     </MyClass>
    /// </code>
    /// it will parse the attributes to its properties. MyClass.MyProperty="MyValue"
    /// and then parse the inner xml element as its content. MyClass.Content="OtherClass instance"
    /// the content property should be specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlContentParserAttribute : Attribute, IXmlParser
    {
        #region Constructor

        public XmlContentParserAttribute(string contentProperty)
        {
            _contentProperty = contentProperty;
        }

        private string _contentProperty;

        #endregion

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            // veriy parameters are correct
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            // verify the content property really exists in target's type
            PropertyInfo contentProperty = target.GetType().GetProperty(_contentProperty);
            if (contentProperty == null)
                throw new ArgumentException(_contentProperty + " not exist in target's type " + target.GetType());

            // parse the attributes
            XmlParserHelper.ParseProperty(target, xmlElement);

            // get the content xml element, only the first valid xml element will be used
            XmlElement inner = null;
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                XmlElement element = node as XmlElement;
                if (node != null)
                {
                    inner = (XmlElement)node;
                    break;
                }
            }
            if (inner == null)
                return;

            // parse the element and set it to the content of the target
            object obj = XtcTestHelper.GetObjectFromXml(inner);
            XmlParserHelper.Parse(obj, inner);
            contentProperty.SetValue(target, obj, new object[0]);
        }

        #endregion
    }

    /// <summary>
    /// the parser support the following format
    /// <code>
    ///     <MyClass MyProperty='value'>
    ///         <FirstObjectClass MyProperty1='value'/>
    ///         <SecondObjectClass MyProperty2='value'/>
    ///     </MyClass>
    /// </code>
    /// it will parse the first xml element into the FirstProperty of the target,
    /// and then parse the second xml element into the SecondProperty of the target.
    /// it also parse attributes into properties of the target.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DoubleXmlContentParserAttribute : Attribute, IXmlParser
    {
        #region Constructor

        public DoubleXmlContentParserAttribute(string first, string second)
        {
            _first = first;
            _second = second;
        }

        #endregion

        #region FirstProperty

        /// <summary>
        /// the first property name
        /// the parser will parse the first xml element and then put it into the first property of the target
        /// </summary>
        public string FirstProperty
        {
            get { return _first; }
            set { _first = value; }
        }

        private string _first;

        #endregion

        #region SecondProperty

        /// <summary>
        /// the second property name
        /// the parser will parse the second xml element and then put it into the second property of the target
        /// </summary>
        public string SecondProperty
        {
            get { return _second; }
            set { _second = value; }
        }

        private string _second;

        #endregion

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            PropertyInfo firstProperty = target.GetType().GetProperty(_first);
            if (firstProperty == null)
                throw new ArgumentException("Property " + _first + " not exist in " + target.GetType());

            PropertyInfo secondProperty = target.GetType().GetProperty(_second);
            if (secondProperty == null)
                throw new ArgumentException("Property " + _second + " not exist in " + target.GetType());

            XmlParserHelper.ParseProperty(target, xmlElement);

            const uint OBJECT_COUNT = 2;
            object[] objs = new object[OBJECT_COUNT];

            int index = 0;
            foreach (XmlNode node in xmlElement.ChildNodes)
            {
                if (!(node is XmlElement))
                    continue;

                XmlElement element = (XmlElement)node;
                object obj = XtcTestHelper.GetObjectFromXml(element);
                XmlParserHelper.Parse(obj, element);
                objs[index++] = obj;
                if (index == OBJECT_COUNT)
                    break;
            }

            Assert.AssertTrue("objects is not legal: [" + objs[0] + "] [" + objs[1] + "]", objs[0] != null && objs[1] != null);
            firstProperty.SetValue(target, objs[0], new object[0]);
            secondProperty.SetValue(target, objs[1], new object[0]);
        }

        #endregion
    }

    public static class XmlParserHelper
    {
        /// <summary>
        /// parse the target object using the attached xml parser attributes
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmlElement"></param>
        public static void Parse(object target, XmlElement xmlElement)
        {
            // verify the parameters are correct
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            Type type = target.GetType();
            object[] attrs = type.GetCustomAttributes(typeof(IXmlParser), true);
            foreach (object attrObj in attrs)
            {
                IXmlParser parser = (IXmlParser)attrObj;
                parser.Parse(target, xmlElement);
            }
        }

        /// <summary>
        /// parse the attributes in the xml element as target's properties
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmlElement"></param>
        public static void ParseProperty(object target, XmlElement xmlElement)
        {
            Type type = target.GetType();
            foreach (XmlAttribute attr in xmlElement.Attributes)
            {
                PropertyInfo pi = type.GetProperty(attr.Name);
                if (pi == null)
                    throw new ArgumentException("Property " + attr.Name + " not exist in " + type);
                pi.SetValue(target, XmlHelper.ConvertToType(pi.PropertyType, attr.Value), new object[0]);
            }
        }

        /// <summary>
        /// parse the inner xml as target's content
        /// </summary>
        /// <param name="target"></param>
        /// <param name="xmlElement"></param>
        /// <param name="contentProperty">the property name of content</param>
        public static void ParseContent(object target, XmlElement xmlElement, string contentProperty)
        {
            Type type = target.GetType();
            PropertyInfo pi = type.GetProperty(contentProperty);
            if (pi == null)
                throw new ArgumentException("Property " + contentProperty + " not exist in " + type);

            object value = XtcTestHelper.LoadXml(xmlElement.InnerXml);
            pi.SetValue(target, value, new object[0]);
        }
    }

    /// <summary>
    /// xaml parser for XamlStepsUnitTest
    /// It will parse the inner xml of "XAML" element of variation and put the result into its content property
    /// </summary>
    public class XamlParserAttribute : Attribute, IXmlParser
    {
        #region Constructor

        public XamlParserAttribute(string contentProperty)
        {
            _contentProperty = contentProperty;
        }

        private string _contentProperty;

        #endregion

        #region IXmlParser members

        public void Parse(object target, XmlElement xmlElement)
        {
            // veriy parameters are correct
            if (target == null)
                throw new ArgumentNullException("target");
            if (xmlElement == null)
                throw new ArgumentNullException("xmlElement");

            // verify the content property really exists in target's type
            PropertyInfo contentProperty = target.GetType().GetProperty(_contentProperty);
            if (contentProperty == null)
                throw new ArgumentException(_contentProperty + " not exist in target's type " + target.GetType());

            // get the content xml element
            XmlElement inner = xmlElement["XAML"];
            if (inner == null)
                return;

            // parse the element and set it to the content of the target
            object obj = XtcTestHelper.LoadXml(inner.InnerXml);
            contentProperty.SetValue(target, obj, new object[0]);
        }

        #endregion
    }
}
