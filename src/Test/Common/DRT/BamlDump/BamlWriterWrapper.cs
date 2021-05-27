// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***************************************************************************\
*
* File: BamlWriterWrapper.cs
*
*  Wrapper for reflection based use of BamlWriter
*
*
\***************************************************************************/

using System;
using System.Text;
using System.Threading;

using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Markup;


namespace DRT
{
        
    // Wrap an internal BamlWriter by giving it a public api that mirrors the internal one.
    // Use reflection to invoke the internal properties and methods.
    public class BamlWriterWrapper
    {
        public BamlWriterWrapper(Stream stream)
        {
            _writerType = WrapperUtil.AssemblyPF.GetType("System.Windows.Markup.BamlWriter");
            _writer = Activator.CreateInstance(_writerType, new object[1] {stream});
            _bamlAttributeUsage = WrapperUtil.AssemblyPF.GetType("System.Windows.Markup.BamlAttributeUsage");
        }

        public void WriteStartDocument()
        {
            _writerType.InvokeMember("WriteStartDocument", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }

        public void WriteEndDocument()
        {
            _writerType.InvokeMember("WriteEndDocument", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }

        public void WriteConnectionId(Int32 connectionId)
        {
            _writerType.InvokeMember("WriteConnectionId", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { connectionId });
        }

        public void WriteRootElement(string assemblyName, string typeFullName)
        {
            _writerType.InvokeMember("WriteRootElement", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, typeFullName });
        }
 
        public void WriteStartElement(
            string assemblyName,
            string typeFullName,
            bool isInjected,
            bool useTypeConverter)
        {
            _writerType.InvokeMember("WriteStartElement", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, typeFullName, isInjected, useTypeConverter });
        }

        public void WriteEndElement()
        {
            _writerType.InvokeMember("WriteEndElement", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }
        
        public void WriteStartConstructor()
        {
            _writerType.InvokeMember("WriteStartConstructor", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {  });
        }
        
        public void WriteEndConstructor()
        {
            _writerType.InvokeMember("WriteEndConstructor", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }
        
        public void WriteProperty(
            string  assemblyName,
            string  ownerTypeFullName,
            string  propName,
            string  value,
            object attributeUsage)
        {
            _writerType.InvokeMember("WriteProperty", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, ownerTypeFullName, propName, value, attributeUsage});
        }

        public void WriteProperty(
            string  assemblyName,
            string  ownerTypeFullName,
            string  propName,
            string  value,
            string  attributeUsage)
        {
            _writerType.InvokeMember("WriteProperty", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, ownerTypeFullName, propName, value,
                                      Enum.Parse(_bamlAttributeUsage, attributeUsage) });
        }

        public void WriteXmlnsProperty(
            string  localName,
            string  xmlNamespace)
        {
            _writerType.InvokeMember("WriteXmlnsProperty", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { localName, xmlNamespace });
        }

        public void WriteDefAttribute(
            string name,
            string value)
        {
            _writerType.InvokeMember("WriteDefAttribute", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { name, value });
        }

        public void WritePresentationOptionsAttribute(
            string name,
            string value)
        {
            _writerType.InvokeMember("WritePresentationOptionsAttribute", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { name, value });
        }        

        public void WriteContentProperty(
            string assemblyName,
            string ownerTypeFullName,
            string propName)
        {
            _writerType.InvokeMember("WriteContentProperty",
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, ownerTypeFullName, propName } );
        }

        public void WriteStartComplexProperty(
            string assemblyName,
            string ownerTypeFullName,
            string propName)
        {
            _writerType.InvokeMember("WriteStartComplexProperty", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { assemblyName, ownerTypeFullName, propName });
        }

        public void WriteEndComplexProperty()
        {
            _writerType.InvokeMember("WriteEndComplexProperty", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }

        public void WriteLiteralContent(
            string contents)
        {
            _writerType.InvokeMember("WriteLiteralContent", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {contents});
        }

        public void WritePIMapping(
            string    xmlNamespace,
            string    clrNamespace,
            string    assemblyName)
        {
            _writerType.InvokeMember("WritePIMapping", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {xmlNamespace, clrNamespace, assemblyName});
        }

        public void WriteText(
            string textContent,
            string typeConverterAssemblyName,
            string typeConverterName)
        {
            _writerType.InvokeMember("WriteText", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {textContent, typeConverterAssemblyName, typeConverterName});
        }

        public void WriteRoutedEvent(
            string     assemblyName,
            string     ownerTypeFullName,
            string     eventIdName,
            string     handlerName)
        {
            _writerType.InvokeMember("WriteRoutedEvent", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {assemblyName, ownerTypeFullName, eventIdName, handlerName});
        }

        public void WriteEvent(
            string    eventName,
            string    handlerName)
        {
            _writerType.InvokeMember("WriteEvent", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {eventName, handlerName});
        }

        public void WriteStartArray(
            string assemblyName,
            string typeFullName)
        {
            _writerType.InvokeMember("WriteStartArray", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {assemblyName, typeFullName});
        }
        
        public void WriteEndArray()
        {
            _writerType.InvokeMember("WriteEndArray", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] {});
        }
        
        public void Close()
        {
            _writerType.InvokeMember("Close", 
                                      WrapperUtil.MethodBindFlags, null, _writer, 
                                      new object[] { });
        }
        Type   _writerType;
        object _writer;
        Type _bamlAttributeUsage;
    }
    
}











