// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/* This file is copied from src\wpf\Tools\SystemXaml\XamlTools. Both the files should be kept in sync except the namespace*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Diagnostic Writer
    /// </summary>
    public class DiagnosticWriter : XamlWriter, IXamlLineInfoConsumer
    {
        #region PrivateData

        /// <summary> Null String </summary>
        private const string NullString = "*null*";

        /// <summary> nullObjectFrom Member </summary>
        private const string NullObjectFromMember = "*objectFromMember*";

        /// <summary> Text Writer </summary>
        private readonly TextWriter _textWriterOutput;

        /// <summary> wrapped Writer </summary>
        private readonly XamlWriter _wrappedWriter = null;

        /// <summary> indent Unit </summary>
        private string _indentUnit;

        /// <summary> fromMember Text </summary>
        private string _fromMemberText;

        /// <summary> unknown Text </summary>
        private string _unknownText;

        /// <summary> implicit Text </summary>
        private string _implicitText;

        /// <summary> depth value </summary>
        private int _depth = 0;

        /// <summary> schema Context </summary>
        private XamlSchemaContext _schemaContext;

        /// <summary> Simple WriterStack </summary>
        private SimpleWriterStack _stack;

        /// <summary> wrappedWriter LineInfo Consumer </summary>
        private IXamlLineInfoConsumer _wrappedWriterLineInfoConsumer = null;

        /// <summary> line Number </summary>
        private int _lineNumber;

        /// <summary> line Position </summary>
        private int _linePosition;

        /// <summary> lineInfo IsNew </summary>
        private bool _lineInfoIsNew;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticWriter"/> class.
        /// </summary>
        /// <param name="schemaContext">The schema context.</param>
        public DiagnosticWriter(XamlSchemaContext schemaContext)
        {
            Initialize();
            _textWriterOutput = Console.Out;
            this._schemaContext = schemaContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticWriter"/> class.
        /// </summary>
        /// <param name="wrappedWriter">The wrapped writer.</param>
        /// <param name="schemaContext">The schema context.</param>
        public DiagnosticWriter(XamlWriter wrappedWriter, XamlSchemaContext schemaContext)
            : this(schemaContext)
        {
            this._wrappedWriter = wrappedWriter;
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticWriter"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="wrappedWriter">The wrapped writer.</param>
        /// <param name="schemaContext">The schema context.</param>
        public DiagnosticWriter(TextWriter outputStream, XamlWriter wrappedWriter, XamlSchemaContext schemaContext)
            : this(outputStream, schemaContext)
        {
            this._wrappedWriter = wrappedWriter;
            Initialize();
            if (wrappedWriter != null)
            {
                SetRuntimeTextWriter(wrappedWriter, _textWriterOutput);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticWriter"/> class.
        /// </summary>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="schemaContext">The schema context.</param>
        public DiagnosticWriter(TextWriter outputStream, XamlSchemaContext schemaContext)
        {
            this._schemaContext = schemaContext;
            Initialize();
            _textWriterOutput = outputStream;
        }

        #endregion

        /// <summary>
        /// Simple delegate 
        /// </summary>
        /// <typeparam name="R">param of Type R</typeparam>
        /// <typeparam name="S">param of Type S</typeparam>
        /// <param name="src">param of Type R.</param>
        /// <returns>Oject of Type S</returns>
        private delegate S Func<R, S>(R src);

        #region Properties
        
        /// <summary>
        /// Gets or sets a value indicating whether [show close comments].
        /// </summary>
        /// <value><c>true</c> if [show close comments]; otherwise, <c>false</c>.</value>
        public bool ShowCloseComments { get; set; }

        /// <summary>
        /// Gets or sets the indent unit.
        /// </summary>
        /// <value>The indent unit.</value>
        public string IndentUnit
        {
            get
            {
                return _indentUnit;
            }

            set
            {
                _indentUnit = value;
            }
        }

        /// <summary>
        /// Gets or sets from member text.
        /// </summary>
        /// <value>From member text.</value>
        public string FromMemberText
        {
            get
            {
                return _fromMemberText;
            }

            set
            {
                _fromMemberText = value;
            }
        }

        /// <summary>
        /// Gets or sets the unknown text.
        /// </summary>
        /// <value>The unknown text.</value>
        public string UnknownText
        {
            get
            {
                return _unknownText;
            }

            set
            {
                _unknownText = value;
            }
        }

        /// <summary>
        /// Gets or sets the implicit text.
        /// </summary>
        /// <value>The implicit text.</value>
        public string ImplicitText
        {
            get
            {
                return _implicitText;
            }

            set
            {
                _implicitText = value;
            }
        }

        /// <summary>
        /// Gets the schema context.
        /// </summary>
        /// <value>The schema context.</value>
        public override XamlSchemaContext SchemaContext
        {
            get
            {
                return _schemaContext;
            }
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
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

        /// <summary>
        /// Gets a value indicating whether [should provide line info].
        /// </summary>
        /// <value>
        /// <c>true</c> if [should provide line info]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldProvideLineInfo
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the line info string.
        /// </summary>
        /// <value>The line info string.</value>
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the get object.
        /// </summary>
        public override void WriteGetObject()
        {
            WriteObject(null, true);
        }

        /// <summary>
        /// Writes the start object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        public override void WriteStartObject(XamlType xamlType)
        {
            WriteObject(xamlType, false);
        }

        /// <summary>
        /// Writes the end object.
        /// </summary>
        public override void WriteEndObject()
        {
            --_depth;
            Indent();

            _textWriterOutput.Write("EO");

            XamlType xamlType = _stack.Peek().Type;

            if (ShowCloseComments)
            {
                _textWriterOutput.Write("        // ");

                if (xamlType == null)
                {
                    _textWriterOutput.Write(NullString);
                }
                else
                {
                    IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
                    string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);

                    if (!String.IsNullOrEmpty(prefix))
                    {
                        _textWriterOutput.Write("{0}:", prefix);
                    }

                    _textWriterOutput.Write("{0}", xamlType.Name);
                }
            }

            _textWriterOutput.WriteLine("     {0}", LineInfoString);

            _stack.Pop();

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes the start member.
        /// </summary>
        /// <param name="property">The property.</param>
        public override void WriteStartMember(XamlMember property)
        {
            Indent();

            _textWriterOutput.Write("SM ");

            _stack.Peek().Member = property;

            if (property == null)
            {
                _textWriterOutput.Write(NullString);
            }
            else
            {
                IList<string> xmlns = property.GetXamlNamespaces();
                string prefix = _stack.FindPrefixFromXmlnsList(xmlns);
                if (prefix != string.Empty && prefix != _stack.Peek().TypePrefix)
                {
                    _textWriterOutput.Write("{0}:", prefix);
                }

                if (property.IsAttachable)
                {
                    _textWriterOutput.Write("{0}.", property.DeclaringType.Name);
                }

                _textWriterOutput.Write("{0}", property.Name);

                // 
                if (IsImplicit(property))
                {
                    _textWriterOutput.Write("     [{0}]", _implicitText);
                }

                if (property.IsUnknown)
                {
                    _textWriterOutput.Write("     [{0}]", _unknownText);
                }
            }

            _textWriterOutput.WriteLine("     {0}", LineInfoString);
            ++_depth;

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteStartMember(property);
            }
        }

        /// <summary>
        /// Writes the end member.
        /// </summary>
        public override void WriteEndMember()
        {
            --_depth;
            Indent();

            _textWriterOutput.Write("EM");

            XamlMember property = _stack.Peek().Member;

            if (ShowCloseComments)
            {
                _textWriterOutput.Write("        // ");
                if (property == null)
                {
                    _textWriterOutput.Write(NullString);
                }
                else
                {
                    IList<string> xmlNamespaces = _stack.Peek().Type.GetXamlNamespaces();
                    string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);

                    if (prefix != string.Empty && prefix != _stack.Peek().TypePrefix)
                    {
                        _textWriterOutput.Write("{0}:", prefix);
                    }

                    if (property.IsAttachable)
                    {
                        _textWriterOutput.Write("{0}.", property.DeclaringType.Name);
                    }

                    _textWriterOutput.Write("{0}", property.Name);
                }
            }

            _stack.Peek().Member = null;

            _textWriterOutput.WriteLine("     {0}", LineInfoString);

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteEndMember();
            }
        }

        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteValue(object value)
        {
            Indent();

            string valueString = value as string;
            if (value == null)
            {
                _textWriterOutput.Write("V ", NullString);
            }
            else if (valueString != null)
            {
                _textWriterOutput.Write("V \"{0}\"", valueString);
            }
            else
            {
                _textWriterOutput.Write("V ({0})  [{1}]", value.ToString(), value.GetType().Name);
            }

            _textWriterOutput.WriteLine("     {0}", LineInfoString);

            if (_wrappedWriter != null)
            {
                _wrappedWriter.WriteValue(value);
            }
        }

        /// <summary>
        /// Writes the namespace.
        /// </summary>
        /// <param name="namespaceDeclaration">The namespace declaration.</param>
        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            string xamlNs = namespaceDeclaration.Namespace;
            string prefix = namespaceDeclaration.Prefix;
            Indent();
            string prefixString = (prefix == string.Empty || prefix == null) ? string.Empty : ":" + prefix;
            if (xamlNs == null)
            {
                _textWriterOutput.Write("NS  xmlns{0}={1}", prefixString, NullString);
            }
            else
            {
                _textWriterOutput.Write("NS  xmlns{0}=\"{1}\"", prefixString, xamlNs);
            }

            _textWriterOutput.WriteLine("     {0}", LineInfoString);

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
                _wrappedWriter.WriteNamespace(namespaceDeclaration);
            }
        }

        /// <summary>
        /// Sets the line info.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        public void SetLineInfo(int lineNumber, int linePosition)
        {
            {
                this._lineNumber = lineNumber;
                this._linePosition = linePosition;
                _lineInfoIsNew = true;

                if (_wrappedWriterLineInfoConsumer != null && _wrappedWriterLineInfoConsumer.ShouldProvideLineInfo)
                {
                    _wrappedWriterLineInfoConsumer.SetLineInfo(lineNumber, linePosition);
                }
            }
        }

        #endregion

        /// <summary>
        /// Determines whether the specified property is implicit.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <c>true</c> if the specified property is implicit; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsImplicit(XamlMember property)
        {
            return
                property.IsDirective &&
                (property == XamlLanguage.Items ||
                 property == XamlLanguage.Initialization ||
                 property == XamlLanguage.PositionalParameters);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !IsDisposed)
                {
                    Indent();
                    _textWriterOutput.WriteLine("Closed.");

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

        #region Private Methods

        /// <summary>
        /// Sets the runtime text writer.
        /// </summary>
        /// <param name="wrappedwriter">The wrappedwriter.</param>
        /// <param name="tw">The TextWriter.</param>
        private static void SetRuntimeTextWriter(XamlWriter wrappedwriter, TextWriter tw)
        {
            Type writerType = typeof(XamlObjectWriter);
            PropertyInfo diagnoticWriterProperty = writerType.GetProperty("DiagnosticWriter", BindingFlags.NonPublic | BindingFlags.Instance);
            diagnoticWriterProperty.SetValue(wrappedwriter, tw, null);
        }

        /// <summary>
        /// Finds the type.
        /// </summary>
        /// <param name="name">The name value.</param>
        /// <returns>Type value </returns>
        private static Type FindType(string name)
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in asm.GetTypes())
                {
                    if (t.Name == name)
                    {
                        return t;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Joins the specified SRC.
        /// </summary>
        /// <typeparam name="R">Object of Type R</typeparam>
        /// <param name="src">The SRC IList.</param>
        /// <param name="toString">To string.</param>
        /// <param name="delim">The delimiter string.</param>
        /// <returns>string value</returns>
        private static string Join<R>(IList<R> src, Func<R, string> toString, string delim)
        {
            StringBuilder sb = new StringBuilder();
            int remaining = src.Count;
            foreach (R r in src)
            {
                sb.Append(toString(r));
                if (--remaining > 0)
                {
                    sb.Append(delim);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            _indentUnit = " ";
            _fromMemberText = "Retrieved";
            _unknownText = "Unknown";
            _implicitText = "Implicit";
            _stack = new SimpleWriterStack();
            _wrappedWriterLineInfoConsumer = _wrappedWriter as IXamlLineInfoConsumer;
        }

        /// <summary>
        /// Writes the object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="isFromMember">if set to <c>true</c> [is from member].</param>
        private void WriteObject(XamlType xamlType, bool isFromMember)
        {
            Indent();

            if (isFromMember)
            {
                _textWriterOutput.Write("GO");
            }
            else
            {
                _textWriterOutput.Write("SO ");
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
                    _textWriterOutput.Write(NullString);
                }
            }
            else
            {
                //// figure out prefix
                frame.NodeType = XamlNodeType.StartObject;
                IList<string> xmlNamespaces = xamlType.GetXamlNamespaces();
                string prefix = _stack.FindPrefixFromXmlnsList(xmlNamespaces);
                _stack.Peek().TypePrefix = prefix;

                switch (prefix)
                {
                    case null:
                        string nullStr = isFromMember ? NullObjectFromMember : NullString;
                        _textWriterOutput.Write(nullStr + ":");
                        break;
                    case "":
                        break;
                    default:
                        _textWriterOutput.Write("{0}:", prefix);
                        break;
                }

                if (xamlType.TypeArguments != null)
                {
                    _textWriterOutput.Write("{0}({1})", xamlType.Name, Join<XamlType>(xamlType.TypeArguments, PrintType, ", "));
                }
                else
                {
                    _textWriterOutput.Write("{0}", xamlType.Name);
                }
            }

            if (!isFromMember && xamlType.IsUnknown)
            {
                _textWriterOutput.Write("     [{0}]", _unknownText);
            }

            _textWriterOutput.WriteLine("     {0}", LineInfoString);
            ++_depth;

            if (_wrappedWriter != null)
            {
                if (isFromMember)
                {
                    _wrappedWriter.WriteGetObject();
                }
                else
                {
                    _wrappedWriter.WriteStartObject(xamlType);
                }
            }
        }

        /// <summary>
        /// Unknowns the label.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>string value</returns>
        private string UnknownLabel(XamlType type)
        {
            return type.IsUnknown ? String.Format("   [{0}]", _unknownText)
                       : String.Empty;
        }

        /// <summary>
        /// Prints the type.
        /// </summary>
        /// <param name="type">The type value.</param>
        /// <returns>string value</returns>
        private string PrintType(XamlType type)
        {
            return (type.Name +
                    (type.TypeArguments == null ? String.Empty
                         : "(" + Join(type.TypeArguments, PrintType, ", ") + ")"))
                   + UnknownLabel(type);
        }

        /// <summary>
        /// Indents this instance.
        /// </summary>
        private void Indent()
        {
            for (int i = 0; i < _depth; i++)
            {
                _textWriterOutput.Write(_indentUnit);
            }
        }

        #endregion
    }

    /// <summary>
    /// Simple WriterFrame
    /// </summary>
    public class SimpleWriterFrame
    {
        /// <summary>
        /// Dictionary NamespaceTable
        /// </summary>
        private Dictionary<string, string> _namespaceTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWriterFrame"/> class.
        /// </summary>
        /// <param name="type">The type value.</param>
        public SimpleWriterFrame(XamlType type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWriterFrame"/> class.
        /// </summary>
        public SimpleWriterFrame()
        {
        }

        /// <summary>
        /// Gets or sets the namespace table.
        /// </summary>
        /// <value>The namespace table.</value>
        public Dictionary<string, string> NamespaceTable
        {
            get
            {
                return _namespaceTable;
            }

            set
            {
                _namespaceTable = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public XamlNodeType NodeType { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type value.</value>
        public XamlType Type { get; set; }

        /// <summary>
        /// Gets or sets the type prefix.
        /// </summary>
        /// <value>The type prefix.</value>
        public string TypePrefix { get; set; }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        /// <value>The member.</value>
        public XamlMember Member { get; set; }

        /// <summary>
        /// Adds the namespace table entry.
        /// </summary>
        /// <param name="namespaceUri">The namespace URI.</param>
        /// <param name="prefix">The prefix.</param>
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
    }

    /// <summary>
    /// Simple WriterStack
    /// </summary>
    public class SimpleWriterStack
    {
        /// <summary>
        /// Initial StackSize
        /// </summary>
        private const int InitialStackSize = 100;

        /// <summary>
        /// stack Data
        /// </summary>
        private SimpleWriterFrame[] _stackData = new SimpleWriterFrame[InitialStackSize];

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>The index of the current.</value>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Pushes the specified frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        public void Push(SimpleWriterFrame frame)
        {
            _stackData[CurrentIndex++] = frame;
            if (CurrentIndex == _stackData.Length)
            {
                //// double the size of the stack
                SimpleWriterFrame[] newStackData = new SimpleWriterFrame[_stackData.Length * 2];
                _stackData.CopyTo(newStackData, 0);
                _stackData = newStackData;
            }
        }

        /// <summary>
        /// Peeks this instance.
        /// </summary>
        /// <returns>Simple Writer Frame</returns>
        public SimpleWriterFrame Peek()
        {
            return _stackData[CurrentIndex - 1];
        }

        /// <summary>
        /// Pops this instance.
        /// </summary>
        /// <returns>Simple WriterFrame</returns>
        public SimpleWriterFrame Pop()
        {
            return _stackData[--CurrentIndex];
        }

        /// <summary>
        /// Finds the prefix from XMLNS list.
        /// </summary>
        /// <param name="xmlnsList">The XMLNS list.</param>
        /// <returns>string value</returns>
        public string FindPrefixFromXmlnsList(IList<string> xmlnsList)
        {
            // this case should only have one uri in the list.
            if (xmlnsList[0] == "http://www.w3.org/XML/1998/namespace")
            {
                return "xml";
            }

            int searchIndex = CurrentIndex;
            string prefix = null;
            for (int i = CurrentIndex - 1; i >= 0; i--)
            {
                var searchFrame = _stackData[i];
                if (searchFrame.NamespaceTable != null)
                {
                    foreach (string xmlns in xmlnsList)
                    {
                        if (searchFrame.NamespaceTable.TryGetValue(xmlns, out prefix))
                        {
                            return prefix;
                        }
                    }
                }
            }

            // this case should only have one uri in the list.
            foreach (string xmlns in xmlnsList)
            {
                if (xmlns == "http://schemas.microsoft.com/winfx/2006/xaml") // || xmlns == "http://schemas.microsoft.com/winfx/2009/xaml"  (eventually)
                {
                    return "xamlNamespace"; // in case nobody used the xamlnamespace
                }
            }

            return null;
        }
    }
}
