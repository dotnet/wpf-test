// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

using System.Xaml;
using System.Text;
using System.Reflection;

namespace XamlTools
{
    public class DiagnosticWriter : XamlWriter, IXamlLineInfoConsumer
    {
        const string _nullString = "*null*";
        const string _nullObjectFromMember = "*objectFromMember*";

        string _indentUnit;
        string _fromMemberText;
        string _unknownText;
        string _implicitText;

        TextWriter _out;
        int _depth = 0;
        XamlSchemaContext _schemaContext;
        SimpleWriterStack _stack;
        XamlWriter _wrappedWriter = null;
        IXamlLineInfoConsumer _wrappedWriterLineInfoConsumer = null;
        int _lineNumber;
        int _linePosition;
        bool _lineInfoIsNew;

        public DiagnosticWriter(XamlSchemaContext xamlSchemaContext)
        {
            Initialize();
            _out = Console.Out;
            _schemaContext = xamlSchemaContext;
        }

        public DiagnosticWriter(XamlWriter wrappedWriter, XamlSchemaContext xamlSchemaContext)
            : this(xamlSchemaContext)
        {
            _wrappedWriter = wrappedWriter;
            Initialize();
        }

        public DiagnosticWriter(TextWriter outputStream, XamlWriter wrappedWriter, XamlSchemaContext xamlSchemaContext)
            : this(outputStream, xamlSchemaContext)
        {
            _wrappedWriter = wrappedWriter;
            Initialize();
        }

        public DiagnosticWriter(TextWriter outputStream, XamlSchemaContext xamlSchemaContext)
        {
            Initialize();
            _out = outputStream;
            _schemaContext =  xamlSchemaContext;
        }

        private void Initialize()
        {
            _indentUnit = " ";
            _fromMemberText = "Retrieved";
            _unknownText = "Unknown";
            _implicitText = "Implicit";
            _stack = new SimpleWriterStack();
            _wrappedWriterLineInfoConsumer = _wrappedWriter as IXamlLineInfoConsumer;
        }

        private static Type FindType(string name)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (t.Name == name)
                        return t;
                }
            }
            return null;
        }

        public bool ShowCloseComments { get; set; }

        public string IndentUnit
        {
            get { return _indentUnit; }
            set { _indentUnit = value; }
        }

        public string FromMemberText
        {
            get { return _fromMemberText; }
            set { _fromMemberText = value; }
        }

        public bool ShowLinebreaks { get; set; }

        public string UnknownText
        {
            get { return _unknownText; }
            set { _unknownText = value; }
        }

        public string ImplicitText
        {
            get { return _implicitText; }
            set { _implicitText = value; }
        }

        #region XamlWriter

        public override void WriteGetObject()
        {
            WriteObject(null, true);
        }
        
        public override void WriteStartObject(XamlType xamlType)
        {
            WriteObject(xamlType, false);
        }
        void WriteObject(XamlType xamlType, bool isFromMember)

        {
            Indent();

            if (isFromMember)
            {
                _out.Write("GO");
            }
            else
            {
                _out.Write("SO ");
            }

            SimpleWriterFrame frame;
            if (_stack.CurrentIndex == 0 || 
               _stack.Peek().NodeType == XamlNodeType.StartObject || 
               _stack.Peek().NodeType == XamlNodeType.GetObject)
            {
                frame = new SimpleWriterFrame();
                _stack.Push(frame);
            }
            else
            {
                frame = _stack.Peek();
            }
            frame.Type = xamlType;

            if (xamlType == null)
            {
                if (isFromMember)
                {
                    frame.NodeType = XamlNodeType.GetObject;
                }
                else
                {
                    _out.Write(_nullString);
                }
            }
            else
            {
                //figure out prefix
                frame.NodeType = XamlNodeType.StartObject;
                IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
                string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);
                _stack.Peek().TypePrefix = prefix;

                switch (prefix)
                {
                case null:
                    string nullStr = isFromMember ? _nullObjectFromMember : _nullString;
                    _out.Write(nullStr + ":");
                    break;
                case "":
                    break;
                default:
                    _out.Write("{0}:", prefix);
                    break;
                }

                if (xamlType.TypeArguments != null)
                {
                    _out.Write("{0}({1})", xamlType.Name, Join<XamlType>(xamlType.TypeArguments, PrintType, ", "));
                }
                else
                {
                    _out.Write("{0}", xamlType.Name);
                }
            }

            if (!isFromMember && xamlType.IsUnknown)
            {
                _out.Write("     [{0}]", _unknownText);
            }

            _out.WriteLine("     {0}", LineInfoString);
            ++_depth;

            if (_wrappedWriter != null)
            {
                if(isFromMember)
                {
                    _wrappedWriter.WriteGetObject();
                }
                else
                {
                    _wrappedWriter.WriteStartObject(xamlType);
                }
            }
        }
        
        private string UnknownLabel(XamlType type)
        {
            return type.IsUnknown ? String.Format("   [{0}]", _unknownText)
                : String.Empty;
        }

        private string PrintType(XamlType type)
        {
            return (type.Name +
                    (type.TypeArguments == null ? String.Empty
                    : "(" + Join(type.TypeArguments, PrintType, ", ") + ")"))
                    + UnknownLabel(type);
        }

        private delegate S Func<R, S>(R src);
        private static string Join<R>(IList<R> src, Func<R, string> ToString, string delim)
        {
            StringBuilder sb = new StringBuilder();
            int remaining = src.Count;
            foreach (R r in src)
            {
                sb.Append(ToString(r));
                if (--remaining > 0)
                    sb.Append(delim);
            }
            return sb.ToString();
        }

        public override void WriteEndObject()
        {
            --_depth;
            Indent();

            _out.Write("EO");

            XamlType xamlType = _stack.Peek().Type;

            if (ShowCloseComments)
            {
                _out.Write("        // ");

                if (xamlType == null)
                {
                    _out.Write(_nullString);
                }
                else
                {
                    IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
                    string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);

                    if (!String.IsNullOrEmpty(prefix))
                    {
                        _out.Write("{0}:", prefix);
                    }
                    _out.Write("{0}", xamlType.Name);
                }
            }
            _out.WriteLine("     {0}", LineInfoString);

            _stack.Pop();

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteEndObject();
            }
        }

        public override void WriteStartMember(XamlMember property)
        {
            Indent();

            _out.Write("SM ");

            _stack.Peek().Member = property;

            if (property == null)
            {
                _out.Write(_nullString);
            }
            else
            {
                IList<string> xmlns = property.GetXamlNamespaces();
                string prefix = _stack.FindPrefixFromXmlnsList(xmlns);
                if (prefix != string.Empty && prefix != _stack.Peek().TypePrefix)
                {
                    _out.Write("{0}:", prefix);
                }

                if (property.IsAttachable)
                {
                    _out.Write("{0}.", property.DeclaringType.Name);
                }
                _out.Write("{0}", property.Name);

                if (property.IsUnknown)
                {
                    _out.Write("     [{0}]", _unknownText);
                }
            }

            _out.WriteLine("     {0}", LineInfoString);
            ++_depth;

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteStartMember(property);
            }
        }

        public override void WriteEndMember()
        {
            --_depth;
            Indent();

            _out.Write("EM");

            XamlMember property = _stack.Peek().Member;

            if (ShowCloseComments)
            {
                _out.Write("        // ");
                if (property == null)
                {
                    _out.Write(_nullString);
                }
                else
                {
                    IList<string> xmlNamespaces = _stack.Peek().Type.GetXamlNamespaces();
                    string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);

                    if (prefix != string.Empty && prefix != _stack.Peek().TypePrefix)
                    {
                        _out.Write("{0}:", prefix);
                    }
                    if (property.IsAttachable)
                    {
                        _out.Write("{0}.", property.DeclaringType.Name);
                    }
                    _out.Write("{0}", property.Name);
                }
            }

            _stack.Peek().Member = null;

            _out.WriteLine("     {0}", LineInfoString);

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteEndMember();
            }
        }

        public override void WriteValue(object value)
        {
            Indent();

            string valueString = value as string;
            if (value == null)
            {
                _out.Write("V ", _nullString);
            }
            else if (valueString != null)
            {
                if (ShowLinebreaks)
                {
                    valueString = valueString.Replace("\n", "\\n");
                }
                _out.Write("V \"{0}\"", valueString);
            }
            else
            {
                _out.Write("V ({0})  [{1}]", value.ToString(), value.GetType().Name);
            }
            _out.WriteLine("     {0}", LineInfoString);

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteValue(value);
            }
        }

        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            string xamlNs = namespaceDeclaration.Namespace;
            string prefix = namespaceDeclaration.Prefix;
            Indent();
            string prefixString = (prefix == string.Empty || prefix == null) ? string.Empty : ":" + prefix;
            if (xamlNs == null)
                _out.Write("NS  xmlns{0}={1}", prefixString, _nullString);
            else
            {
                _out.Write("NS  xmlns{0}=\"{1}\"", prefixString, xamlNs);
            }
            _out.WriteLine("     {0}", LineInfoString);

            SimpleWriterFrame frame;
            if (_stack.CurrentIndex == 0)
            {
                frame = new SimpleWriterFrame();
                _stack.Push(frame);
            }
            else
            {
                frame = _stack.Peek();
                if (frame.NodeType == XamlNodeType.GetObject || 
                    frame.NodeType == XamlNodeType.StartObject)
                {
                    frame = new SimpleWriterFrame();
                    _stack.Push(frame);
                }
            }
            frame.NodeType = XamlNodeType.StartMember;
            frame.AddNamespaceTableEntry(xamlNs, prefix);

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteNamespace(new NamespaceDeclaration(xamlNs, prefix));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !IsDisposed)
                {
                    Indent();
                    _out.WriteLine("Closed.");

                    if (_wrappedWriter != null)
                    {
                        _wrappedWriter.Close();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        public override XamlSchemaContext SchemaContext
        {
            get { return _schemaContext; }
        }

        public object Result
        {
            get
            {
                XamlObjectWriter objectWriter = _wrappedWriter as XamlObjectWriter;
                if (objectWriter != null)
                {
                    return objectWriter.Result;
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion


        // =========== private ===============

        private void Indent()
        {
            for (int i = 0; i < _depth; i++)
            {
                _out.Write(_indentUnit);
            }
        }

        private string LineInfoString
        {
            get
            {
                if (_lineInfoIsNew)
                {
                    _lineInfoIsNew = false;
                    return String.Format("({0},{1})", _lineNumber, _linePosition);
                }
                return String.Empty;
            }
        }

        #region IXamlLineInfoConsumer Members

        public void SetLineInfo(int lineNumber, int linePosition)
        {
            {
                _lineNumber = lineNumber;
                _linePosition = linePosition;
                _lineInfoIsNew = true;

                if (_wrappedWriterLineInfoConsumer != null && _wrappedWriterLineInfoConsumer.ShouldProvideLineInfo)
                {
                    _wrappedWriterLineInfoConsumer.SetLineInfo(lineNumber, linePosition);
                }
            }
        }

        public bool ShouldProvideLineInfo
        {
            get { return true; }
        }

        #endregion
    }

    public class SimpleWriterFrame
    {
        public XamlNodeType NodeType { get; set; }
        public XamlType Type { get; set; }
        public string TypePrefix { get; set; }
        public XamlMember Member { get; set; }
        public Dictionary<string, string> NamespaceTable;
        public void AddNamespaceTableEntry(string namespaceUri, string prefix)
        {
            if (NamespaceTable == null)
            {
                NamespaceTable = new Dictionary<string, string>();
            }
            // right now, we only store the first prefix in the NamespaceTable.  Ideally, we'd store them all and give back the best one (shortest?).
            if (!NamespaceTable.ContainsKey(namespaceUri))
            {
                NamespaceTable.Add(namespaceUri, prefix);
            }
        }
        public SimpleWriterFrame(XamlType type)
        {
            Type = type;
        }
        public SimpleWriterFrame()
        {
        }
    }

    public class SimpleWriterStack
    {
        const int _initialStackSize = 100;

        SimpleWriterFrame[] _stackData = new SimpleWriterFrame[_initialStackSize];
        public void Push(SimpleWriterFrame frame)
        {
            _stackData[CurrentIndex++] = frame;
            if (CurrentIndex == _stackData.Length)
            {
                //double the size of the stack
                SimpleWriterFrame[] newStackData = new SimpleWriterFrame[_stackData.Length * 2];
                _stackData.CopyTo(newStackData, 0);
                _stackData = newStackData;
            }
        }
        public SimpleWriterFrame Peek()
        {
            return _stackData[CurrentIndex - 1];
        }
        public SimpleWriterFrame Pop()
        {
            return _stackData[--CurrentIndex];
        }

        public int CurrentIndex { get; set; }

        public string FindPrefixFromXmlnsList(IList<string> xmlnsList)
        {
            // this case should only have one uri in the list.
            if (xmlnsList[0] == "http://www.w3.org/XML/1998/namespace")
            {
                return "xml";
            }

            int searchIndex = CurrentIndex;
            string prefix = null;
            for(int i=CurrentIndex-1; i>=0; i--)
            {
                var searchFrame = _stackData[i];
                if (searchFrame.NamespaceTable != null)
                {
                    foreach (string xmlns in xmlnsList)
                    {
                        if(searchFrame.NamespaceTable.TryGetValue(xmlns, out prefix))
                        {
                            return prefix;
                        }
                    }
                }
            }

            // this case should only have one uri in the list.
            foreach (string xmlns in xmlnsList)
            {
                if (xmlns == "http://schemas.microsoft.com/winfx/2006/xaml"
                    // || xmlns == "http://schemas.microsoft.com/winfx/2009/xaml"  (eventually)
                    )
                {
                    return "xamlNamespace";  //in case nobody used the xamlnamespace
                }
            }
            return null;
        }
    }
}
