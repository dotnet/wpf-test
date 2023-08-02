// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model ValueSerializer.
 *          Construct trees, serialize them and verify.
 
  
 * Revision:         $Revision: 1 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Reflection;
using System.Xml;
using Microsoft.Test.Windows;
using Avalon.Test.CoreUI.Trusted.Utilities;
using System.Windows.Controls.Primitives;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    /// <summary>
    /// Test all Avalone ValueSerializer. Read from AvalonValueSerializers.xml, for each type, 
    /// get the serializer and verify whether the serializer is the same as defined in 
    /// AvalonValueSerializers.xml. So if there is any change, we can inform feature teams.
    /// </summary>
    /// 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class TestAvalonPublicValueSerializer 
    {
        /// <summary>
        /// 
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("0")]
        [TestCaseArea(@"Serialization\ValueSerializer\AvalonValueSerializers")]
        [TestCaseMethod("RunTest")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void RunTest()
        {
            XmlDocument doc = new XmlDocument();
            Stream xmlFileStream = null;
            try
            {
                xmlFileStream = File.OpenRead(_valueSerializersXml);
                StreamReader reader = new StreamReader(xmlFileStream);
                doc.LoadXml(reader.ReadToEnd());
            }
            finally
            {
                if (null != xmlFileStream)
                    xmlFileStream.Close();
            }
            XmlNodeList serializers = doc.GetElementsByTagName("ValueSerializer");

            foreach (XmlNode serializer in serializers)
            {
                string typeName = GetAttributeValue(serializer, "TypeConsumingMe");
                CoreLogger.LogStatus("Test type: " + typeName);
                string assembly = GetAttributeValue(serializer, "Assembly");
                if (null == typeName) continue;
                
                string serializerName = GetAttributeValue(serializer, "Name");

                Type type = InternalObject.GetType(assembly, typeName);

                if(null == type)
                {
                    throw new Microsoft.Test.TestValidationException("Type : " + typeName + " not found.");
                }
                ValueSerializer serializerFromValueSerializer = ValueSerializer.GetSerializerFor(type);
                if (!string.Equals(serializerName, serializerFromValueSerializer.GetType().Name))
                {
                    throw new Microsoft.Test.TestValidationException("ValueSerializer has been changed from : "
                        + serializerName + " To " + serializerFromValueSerializer.GetType().Name + " for type"
                        + typeName);
                }
            }
        }

        /// <summary>
        /// For each Avalon type check the its ValueSerializer and throw if a new ValueSerializer 
        /// if found.
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("1")]
        [TestCaseArea(@"Serialization\ValueSerializer\RoutedEvent")]
        [TestCaseMethod("TestRoutedEventValueserializer")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void TestRoutedEventValueserializer()
        {
            Button button = new Button();
            button.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnClick));
            string serialized = SerializationHelper.SerializeObjectTree(button);
            if (serialized.Contains("Click"))
            {
                throw new Microsoft.Test.TestValidationException("Event not serialized.");
            }
        }
        private void OnClick(object sender, RoutedEventArgs args)
        {
            Console.WriteLine("Do something.");
        }
        /// <summary>
        /// For each Avalon type check the its ValueSerializer and throw if a new ValueSerializer 
        /// if found.
        /// </summary>
        [CoreTestsLoader(CoreTestsTestType.MethodBase)]
        [TestCasePriority("1")]
        [TestCaseArea(@"Serialization\ValueSerializer\AvalonValueSerializers")]
        [TestCaseMethod("CheckValueserializerForAllType")]
        [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
        public void CheckValueserializerForAllType()
        {
            string[] assemblyNames = { "PresentationCore", "WindowsBase", "PresentationFramework"};
            ReadValueSerializersTable();
            foreach (string assemblyName in assemblyNames)
            {
                CheckTypeInAssembly(assemblyName);
            }
        }
        void CheckTypeInAssembly(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                ValueSerializer serializerFromValueSerializer = null;
                try
                {
                    serializerFromValueSerializer = ValueSerializer.GetSerializerFor(type);
                }
                catch (Exception)
                {
                }

                if(null == serializerFromValueSerializer)
                {
                    continue;
                }
                if (!_valueSerializers.ContainsKey(serializerFromValueSerializer.GetType().Name))
                {
                    throw new Microsoft.Test.TestValidationException("New ValueSerializer: "
                        + serializerFromValueSerializer.GetType().AssemblyQualifiedName
                        + " found.");
                }
            }
        }
        void ReadValueSerializersTable()
        {
            XmlDocument doc = new XmlDocument();
            Stream xmlFileStream = null;
            try
            {
                xmlFileStream = File.OpenRead(_valueSerializersXml);
                StreamReader reader = new StreamReader(xmlFileStream);
                doc.LoadXml(reader.ReadToEnd());
            }
            finally
            {
                if (null != xmlFileStream)
                    xmlFileStream.Close();
            }
            XmlNodeList serializers = doc.GetElementsByTagName("ValueSerializer");

            foreach (XmlNode serializer in serializers)
            {
                string serializerName = GetAttributeValue(serializer, "Name");
                if (null == serializerName)
                {
                    throw new Microsoft.Test.TestValidationException("not found Name property in : "
                        + serializer.ToString());
                }
                _valueSerializers.Add(serializerName, null);
            }
        }
        private string GetAttributeValue(
           XmlNode node,
           string attributeName)
        {
            XmlAttributeCollection attributes = node.Attributes;
            XmlAttribute attribute = attributes[attributeName];

            if (null == attribute)
            {
                return null;
            }
            return attribute.Value;
        }
        Hashtable _valueSerializers = new Hashtable();
        string _valueSerializersXml = "AvalonValueSerializers.xml";
    }
}
