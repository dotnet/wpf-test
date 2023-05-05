// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CDF.Test.TestCases.Xaml.Driver.XamlReaderWriter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xaml;
    using System.Xaml.Schema;
    using System.Xml;
    using CDF.Test.Common;
    using CDF.Test.Common.TestObjects.Xaml.GraphCore;
    using CDF.Test.TestCases.Xaml.Common;
    using CDF.Test.TestCases.Xaml.Common.XamlOM;

    public class WrappedXamlWriter : IXamlWriter, IDisposable
    {
        private class XamlTypeInfo
        {
            public XName TypeName { get; set; }
            public XamlType XamlType { get; set; }
        } ;

        private System.Xaml.XamlWriter _innerWriter;
        private readonly Stack<XamlTypeInfo> _currentTypes = new Stack<XamlTypeInfo>();
        private readonly XamlSchemaContext _schemaContext = new XamlSchemaContext();
        public const string AlternateTypeProperty = "AlternateTypeTag";
        public const string SkipProperty = "SkipTag";

        public WrappedXamlWriter(Stream output)
        {
            Init(output);
        }

        public WrappedXamlWriter(TextWriter output)
        {
            _innerWriter = new XamlXmlWriter(output, this._schemaContext);
        }

        public WrappedXamlWriter()
        {
            _innerWriter = null;
        }

        #region IXamlWriter Members

        public void Init(Stream output)
        {
            if (Settings == null)
            {
                Settings = new XamlXmlWriterSettings();
            }

            _innerWriter = new XamlXmlWriter(output, this._schemaContext, Settings);
        }

        public XamlXmlWriterSettings Settings { get; set; }

        public void WriteRaw(string data, ITestDependencyObject props)
        {
        }

        public void WriteAtom(object value, ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            if (value is XmlQualifiedName)
            {
                value = ((XmlQualifiedName) value).Name;
            }

            // Currently value can only be a string //
            _innerWriter.WriteValue(value.ToString());
        }

        private int _prefix = 0;

        public void WriteStartRecord(XName typeName, ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            XamlType xamlType = null;
            if (GetAlternateType(props) != null)
            {
                xamlType = this._schemaContext.GetXamlType(GetAlternateType(props));
            }
            else
            {
                xamlType = GetXamlType(typeName);
            }
            _currentTypes.Push(new XamlTypeInfo()
                                  {
                                      TypeName = typeName, XamlType = xamlType
                                  });

            GraphNodeRecord graphRecord = props as GraphNodeRecord;
            foreach (XNamespace expecteNs in graphRecord.ExpectedNamespaces)
            {
                WriteNamespace(expecteNs.NamespaceName, "prefix" + _prefix++);
            }
            if (graphRecord.IsObjectFromMember)
            {
                _innerWriter.WriteGetObject();
            }
            else
            {
                _innerWriter.WriteStartObject(xamlType);
            }
        }

        public void WriteEndRecord(ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            _currentTypes.Pop();
            _innerWriter.WriteEndObject();
        }

        public void WriteStartMember(string memberName, ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            GraphNodeMember member = props as GraphNodeMember;

            XamlMember xamlProperty = GetXamlProperty(_currentTypes.Peek().XamlType, memberName);
            _innerWriter.WriteStartMember(xamlProperty);
        }

        public void WriteStartMember(string memberName, XName typeName, ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            GraphNodeMember member = props as GraphNodeMember;

            XamlMember xamlProperty = GetXamlProperty(typeName, memberName, member.MemberType);
            _innerWriter.WriteStartMember(xamlProperty);
        }

        public void WriteEndMember(ITestDependencyObject props)
        {
            if (GetSkipProperty(props))
            {
                return;
            }

            _innerWriter.WriteEndMember();
        }

        public void Close()
        {
            _innerWriter.Close();
        }

        public XName GetCurrentType()
        {
            return XName.Get(_currentTypes.Peek().XamlType.Name, _currentTypes.Peek().XamlType.GetXamlNamespaces()[0]);
        }

        public void WriteNamespace(string xamlNamespace, string prefix)
        {
            _innerWriter.WriteNamespace(new NamespaceDeclaration(xamlNamespace, prefix));
        }

        #endregion

        #region IDisposable Members

        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    _innerWriter.Close();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }

        ~WrappedXamlWriter()
        {
            Dispose(false);
        }

        #endregion

        private XamlMember GetXamlProperty(XamlType xamlType, string memberName)
        {
            return xamlType.GetMember(memberName);
        }

        private XamlMember GetXamlProperty(XName typeName, string memberName, MemberType memberType)
        {
            // Normal, Attachable, Directive,Implicit //
            XamlMember property = null;
            switch (memberType)
            {
                case MemberType.Normal:
                    XamlType xamlType = GetXamlType(typeName);
                    property = GetXamlProperty(xamlType, memberName);
                    if (property == null)
                    {
                        property = xamlType.GetAttachableMember(memberName);
                    }
                    break;

                case MemberType.Directive:
                    // Directives //
                    Tracer.LogTrace("Dir: " + typeName);
                    if (typeName == Constants.Directive2006Type)
                    {
                        property = this._schemaContext.GetXamlDirective(Constants.Namespace2006, memberName);
                    }
                    else if (typeName == Constants.DirectiveV2Type)
                    {
                        throw new NotImplementedException("2008 Namespace is not supported yet");
                        //XamlNamespace xaml2008Namespace = this.schemaContext.GetXamlNamespace(System.Runtime.Xaml.Constants.DirectiveV2Type.NamespaceName);
                        //property = xaml2008Namespace.GetDirectiveProperty(memberName);
                    }
                    break;

                case MemberType.Implicit:
                    property = this._schemaContext.GetXamlDirective(Constants.Namespace2006, memberName);
                    break;

                default:
                    throw new Exception("Member type not valid");
            }

            if (property == null)
            {
                throw new Exception(String.Format(CultureInfo.InvariantCulture, "Unable to get member {0} from type {1}", memberName, typeName));
            }

            return property;
        }

        private XamlType GetXamlType(XName typeName)
        {
            XamlTypeName xamlTypeName = new XamlTypeName(typeName.NamespaceName, typeName.LocalName);
            XamlType xamlType = this._schemaContext.GetXamlType(xamlTypeName);

            if (xamlType == null)
            {
                throw new Exception("Unable to get XamlType for : " + typeName);
            }

            return xamlType;
        }

        private static Type GetAlternateType(ITestDependencyObject props)
        {
            object value = props.GetValue(AlternateTypeProperty);
            return value == null ? null : (Type) value;
        }

        private static bool GetSkipProperty(ITestDependencyObject props)
        {
            object value = props.GetValue(SkipProperty);
            return value == null ? false : (bool) value;
        }
    }
}
