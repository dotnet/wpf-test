// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Xaml
{
    class WriterInfo
    {
        public XamlType Type { get; set; }
        public PropertyTechnique PropertyTechnique { get; set; }
        public XamlMember Property { get; set; }
        public bool PropertyElementOpened { get; set; }

        internal void EndProperty()
        {
            Property = null;
            PropertyElementOpened = false;
        }
    }

    //
    public enum PropertyTechnique
    {
        Attribute = 0, Content, PropertyElement,
    }

    public class SimpleXamlTextWriter : XamlWriter
    {
        Stack<WriterInfo> _writerStack = new Stack<WriterInfo>();

        TextWriter _out;
        int _indentLevel = 0;
        string _indentUnit = "  ";
        string _attrIndent = "        ";
        bool _closeStreamAtEnd;
        XamlSchemaContext _xamlSchemaContext;

        public SimpleXamlTextWriter(XamlSchemaContext xamlSchemaContext)
        {
            _out = Console.Out;
            _xamlSchemaContext = xamlSchemaContext;
        }

        public SimpleXamlTextWriter(TextWriter outputStream, XamlSchemaContext xamlSchemaContext)
        {
            _out = outputStream;
            _xamlSchemaContext = xamlSchemaContext;
        }

        public SimpleXamlTextWriter(string fileName, XamlSchemaContext xamlSchemaContext)
        {
            _out = new StreamWriter(fileName);
            _closeStreamAtEnd = true;
            _xamlSchemaContext = xamlSchemaContext;
        }

        public string IndentUnit
        {
            get { return _indentUnit; }
            set { _indentUnit = value; }
        }

        public string AttributeIndentUnit
        {
            get { return _attrIndent; }
            set { _attrIndent = value; }
        }

        #region xamlWriter

        public override void WriteStartObject(XamlType xamlType)
        {
            WriterInfo frame = CurrentFrame();
            if (frame != null)
            {
                switch (frame.PropertyTechnique)
                {
                case PropertyTechnique.Attribute:
                    _out.WriteLine(">");
                    WriteOpenPropertyElement();
                    frame.PropertyTechnique = PropertyTechnique.PropertyElement;
                    break;
                case PropertyTechnique.Content:
                    break;
                case PropertyTechnique.PropertyElement:
                    if (!frame.PropertyElementOpened)
                        WriteOpenPropertyElement();
                    break;
                }
            }
            
            //
            string prefix = string.Empty;

            Indent();
            _out.Write("<{0}{1}", GetPrefixString(prefix), xamlType.Name);
            _indentLevel++;
            PushFrame(xamlType);
        }

        public override void WriteGetObject()
        {
            WriterInfo frame = CurrentFrame();
            XamlType xamlType = frame.Property.Type;
            if (frame != null)
            {
                switch (frame.PropertyTechnique)
                {
                case PropertyTechnique.Attribute:
                    _out.WriteLine(">");
                    WriteOpenPropertyElement();
                    frame.PropertyTechnique = PropertyTechnique.PropertyElement;
                    break;
                case PropertyTechnique.Content:
                    break;
                case PropertyTechnique.PropertyElement:
                    if (!frame.PropertyElementOpened)
                        WriteOpenPropertyElement();
                    break;
                }
            }
            
            //
            string prefix = string.Empty;

            Indent();
            _out.Write("<{0}{1}", GetPrefixString(prefix), xamlType.Name);
            _indentLevel++;
            PushFrame(xamlType);
        }

        public override void WriteEndObject()
        {
            // 
            WriterInfo frame = PopFrame();
            _indentLevel--;
            if (frame.PropertyTechnique == PropertyTechnique.Attribute)
                _out.WriteLine("/>");
            else
            {
                Indent();
                //
                _out.WriteLine("</{0}{1}>", "", frame.Type.Name);
            }
        }

        public override void WriteStartMember(XamlMember property)
        {
            XamlType ownerType = property.DeclaringType;
            WriterInfo frame = CurrentFrame();
            frame.Property = property;
            if (ownerType != null)
            {
                if (ownerType.ContentProperty == property || property.Name == "_CONTENT")
                {
                    if (frame.PropertyTechnique == PropertyTechnique.Attribute)
                    {
                        _out.WriteLine(">");
                    }
                    frame.PropertyTechnique = PropertyTechnique.Content;
                }
                else
                {
                    if (frame.PropertyTechnique == PropertyTechnique.Content)
                        frame.PropertyTechnique = PropertyTechnique.PropertyElement;
                }
            }
        }

        public override void WriteEndMember()
        {
            WriterInfo frame = CurrentFrame();
            if (frame.PropertyTechnique == PropertyTechnique.PropertyElement)
                WriteClosePropertyElement();
            if (frame.Property != null)
            {
                frame.EndProperty();
            }
            else
                throw new Exception("EndProperty with out matching StartMember");
        }

        public override void WriteValue(object value)
        {
            WriterInfo frame = CurrentFrame();
            XamlMember currentProperty = frame.Property;
            if (currentProperty == null)
            {
                if (frame.PropertyTechnique == PropertyTechnique.Attribute)
                {
                    _out.WriteLine(">");
                    Indent();
                    _out.WriteLine(value);
                    frame.PropertyTechnique = PropertyTechnique.PropertyElement;
                }
            }
            else
            {
                switch (frame.PropertyTechnique)
                {
                case PropertyTechnique.Attribute:
                    //

                    XamlType propertyOwnerType = frame.Property.DeclaringType;
                    _out.WriteLine();
                    AttributeIndent();
                    string propPrefix = ""; //
                    if (frame.Type != null &&
                        !frame.Type.CanAssignTo(propertyOwnerType))
                    {
                        _out.Write("{0}{1}.{2}=\"{3}\"", GetPrefixString(propPrefix),
                                                          frame.Property.DeclaringType.Name,
                                                          frame.Property.Name, value);
                    }
                    else
                    {
                        _out.Write("{0}{1}=\"{2}\"", GetPrefixString(propPrefix),
                                                      frame.Property.Name, value);
                    }
                    break;
                case PropertyTechnique.Content:
                    Indent();
                    _out.WriteLine(value);
                    break;
                case PropertyTechnique.PropertyElement:
                    WriteOpenPropertyElement();
                    Indent();
                    _out.WriteLine(value);
                    break;
                }
            }
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            _out.WriteLine();
            AttributeIndent();
            if (string.IsNullOrEmpty(namespaceDeclaration.Prefix))
            {
                _out.Write("xmlns=\"{0}\"", namespaceDeclaration.Namespace);
            }
            else
            {
                _out.Write("xmlns:{0}=\"{1}\"", namespaceDeclaration.Prefix, namespaceDeclaration.Namespace);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !IsDisposed)
                {
                    //don't let any other write after an EOF
                    _out.Flush();
                    if (_closeStreamAtEnd)
                    {
                        _out.Close();
                    }
                    _out = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override XamlSchemaContext SchemaContext 
        {
            get
            {
                return this._xamlSchemaContext;
            }
        }

        #endregion

        // ---------- private ----------------------

        private void WriteClosePropertyElement()
        {
            _indentLevel--;
            Indent();
            WriterInfo frame = CurrentFrame();
            _out.WriteLine("</{0}.{1}>", frame.Type.Name, frame.Property.Name);
        }

        private void WriteOpenPropertyElement()
        {
            Indent();
            WriterInfo frame = CurrentFrame();
            _out.WriteLine("<{0}.{1}>", frame.Type.Name, frame.Property.Name);
            frame.PropertyElementOpened = true;
            _indentLevel++;

        }

        private static string GetPrefixString(string prefix)
        {
            if (prefix == null || prefix == string.Empty)
                return string.Empty;
            else
                return prefix + ":";
        }

        private void PushFrame(XamlType xamlType)
        {
            WriterInfo newFrame = new WriterInfo();
            newFrame.Type = xamlType;
            _writerStack.Push(newFrame);
        }

        private WriterInfo CurrentFrame()
        {
            if (_writerStack.Count > 0)
            {
                return _writerStack.Peek();
            }
            return null;
        }

        private WriterInfo PopFrame()
        {
            return _writerStack.Pop();
        }

        private void Indent()
        {
            for (int i = 0; i < _indentLevel; i++)
                _out.Write(_indentUnit);
        }

        private void AttributeIndent()
        {
            Indent();
            _out.Write(_attrIndent);
        }
    }
}
