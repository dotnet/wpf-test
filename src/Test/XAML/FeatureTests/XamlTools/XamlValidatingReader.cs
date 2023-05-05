// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.Xaml.Schema;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace XamlTools
{
    public class XamlValidatingReader : XamlReader
    {
        #region XamlValidatingReader constructors
        public XamlValidatingReader(XamlReader reader)
            : this(reader, null)
        {
        }
        public XamlValidatingReader(XamlReader reader, XamlWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }
        public XamlValidatingReader(string fileName)
            : this(fileName, null)
        {
        }
        public XamlValidatingReader(string fileName, XamlWriter writer)
        {
            XmlReader xmlReader = XmlReader.Create(fileName);
            XamlXmlReader xamlReader = new XamlXmlReader(xmlReader);
            _reader = xamlReader;
            _writer = writer;
        }
        #endregion XamlValidatingReader constructors

        #region errorType, Error, RaiseError(...)
        public enum ErrorType
        {
            UnknownType, UnknownMember, InvalidContent, TooMuchContent, InvalidValue,
            EndTagNotMatchingStart, EndOfFileBeforeAllObjectsAreClosed, InvalidSchema, XmlError
        }
        public class Error
        {

            public Error(ErrorType errorType)
            {
                _ErrorType = errorType;
            }
            private ErrorType _ErrorType;
            public ErrorType ErrorType { get { return _ErrorType; } }
            public XamlType XamlType { get; set; }
            public XamlType ContainingXamlType { get; set; }
            public XamlMember XamlProperty { get; set; }
            public String TextSyntax { get; set; }
            public override string ToString()
            {
                string errorString = ErrorType.ToString() + ": ";
                if (XamlProperty != null)
                    errorString += XamlType.Name + "." + XamlProperty.Name;
                else
                    if (XamlType != null)
                        errorString += XamlType.Name;
                return errorString;
            }
        }

        private void RaiseError(ErrorType errorType, XamlType xamlType)
        {
            Error error = new Error(errorType);
            error.XamlType = xamlType;
            RaiseError(error);
        }

        private void RaiseError(ErrorType errorType, XamlMember xamlMember, XamlType xamlType)
        {
            Error error = new Error(errorType);
            error.XamlProperty = xamlMember;
            error.XamlType = xamlType;
            RaiseError(error);
        }

        private void RaiseError(ErrorType errorType, string textSyntax)
        {
            Error error = new Error(errorType);
            error.TextSyntax = textSyntax;
            RaiseError(error);
        }
        private void RaiseError(ErrorType errorType)
        {
            Error error = new Error(errorType);
            RaiseError(error);
        }

        private void RaiseError(Error error)
        {
            Console.Write("[" + ErrorList.Count + "]");
            ErrorList.Add(error);
        }
        #endregion

        #region private fields
        XamlWriter _writer;
        XamlReader _reader;

        class ValidatingReaderInfo
        {
            public XamlType XamlType { get; set; }
            public XamlMember XamlProperty { get; set; }
        }
        Stack<ValidatingReaderInfo> _readerStack = new Stack<ValidatingReaderInfo>();
        #endregion private fields

        #region public object model
        public List<Error> ErrorList = new List<Error>();

        private List<string> _Schemas;
        public List<string> Schemas
        {
            get
            {
                if (_Schemas == null)
                    _Schemas = new List<string>();
                return _Schemas;
            }
        }
        #endregion public object model

        public override bool Read()
        {
            XamlType xamlType;
            XamlMember xamlMember;

            bool ret = _reader.Read();
            switch (_reader.NodeType)
            {
            case XamlNodeType.StartObject:
                xamlType = _reader.Type;

                //type exists?
                XamlType xamlTypeFromWhiteList = findXamlTypeInWhiteList(xamlType);
                if (xamlTypeFromWhiteList == null)
                    RaiseError(ErrorType.UnknownType, xamlType);
                else
                {
                    //type is valid in content model?
                    XamlMember containingProperty = null;
                    if (_readerStack.Count > 0)
                    {
                        containingProperty = _readerStack.Peek().XamlProperty;
                        Debug.Assert(containingProperty != null);
                        XamlType containingPropertyType = containingProperty.Type;
                        if (containingPropertyType == null)
                        {
                            //
                            RaiseError(ErrorType.InvalidSchema
                                , _readerStack.Peek().XamlProperty
                                , _readerStack.Peek().XamlType);
                        }
                        else if (!xamlType.CanAssignTo(containingPropertyType))
                            RaiseError(ErrorType.InvalidContent, containingProperty, xamlType);
                    }
                }

                //readerStack maintenance
                ValidatingReaderInfo vri = new ValidatingReaderInfo();
                vri.XamlType = xamlType;
                _readerStack.Push(vri);
                break;
            case XamlNodeType.EndObject:
                //readerStack maintenance
                _readerStack.Pop();
                xamlType = _readerStack.Count > 0 ? _readerStack.Peek().XamlType : null;
                break;
            case XamlNodeType.StartMember:
                //readerStack maintenance
                xamlMember = _reader.Member;
                _readerStack.Peek().XamlProperty = xamlMember;

                XamlType propertyOwnerType = xamlMember.DeclaringType;
                xamlTypeFromWhiteList = findXamlTypeInWhiteList(propertyOwnerType);
                if (xamlTypeFromWhiteList == null)
                    RaiseError(ErrorType.UnknownMember, xamlMember, propertyOwnerType);
                else
                {
                    XamlMember xamlMemberFromWhiteList = findXamlPropertyInWhiteList(xamlTypeFromWhiteList, xamlMember.Name);
                    if (xamlMemberFromWhiteList == null)
                        RaiseError(ErrorType.UnknownMember, xamlMember, propertyOwnerType);
                }
                break;
            case XamlNodeType.EndMember:
                //readerStack maintenance
                _readerStack.Peek().XamlProperty = null;
                break;
            case XamlNodeType.Value:
                //
                break;
            case XamlNodeType.None:
                if (_readerStack.Count != 0)
                    RaiseError(ErrorType.EndOfFileBeforeAllObjectsAreClosed);
                break;
            default:
                break;
            }
            return ret; //check that this is the right thing to return
        }

        private XamlMember findXamlPropertyInWhiteList(XamlType xamlTypeFromWhiteList, string propertyName)
        {
            try
            {
                return xamlTypeFromWhiteList.GetMember(propertyName);
            }
            catch
            {
                return null;
            }
        }

        private XamlType findXamlTypeInWhiteList(XamlType xamlTypeFromCurrentXamlNode)
        {
            foreach (string xn in Schemas)
            {
                //
                XamlTypeName typeName = new XamlTypeName(xn, xamlTypeFromCurrentXamlNode.Name);
                return SchemaContext.GetXamlType(typeName);
            }
            return null;
        }

        #region pass through XamlReader calls to _reader
        public override XamlNodeType NodeType
        {
            get { return _reader.NodeType; }
        }

        public override bool IsEof
        {
            get { return _reader.IsEof; }
        }

        public override NamespaceDeclaration Namespace
        {
            get { return _reader.Namespace; }
        }

        public override XamlType Type
        {
            get { return _reader.Type; }
        }

        public override object Value
        {
            get { return _reader.Value; }
        }

        public override XamlMember Member
        {
            get { return _reader.Member; }
        }

        public override XamlSchemaContext SchemaContext
        {
            get { return _reader.SchemaContext; }
        }

        #endregion pass through XamlReader calls to _reader
    }
}
