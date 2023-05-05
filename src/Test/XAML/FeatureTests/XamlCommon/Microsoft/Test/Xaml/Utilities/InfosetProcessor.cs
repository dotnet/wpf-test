// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Write Object Delegate
    /// </summary>
    /// <param name="xamlType">XamlType xamlType</param>
    /// <param name="context">XamlSchemaContext context</param>
    public delegate void WriteObjectDelegate(XamlType xamlType, XamlSchemaContext context);

    /// <summary>
    /// WriteMember Delegate
    /// </summary>
    /// <param name="property">XamlMember property</param>
    /// <param name="context">XamlSchemaContext context</param>
    public delegate void WriteMemberDelegate(XamlMember property, XamlSchemaContext context);

    /// <summary>
    /// WriteValue Delegate
    /// </summary>
    /// <param name="value">object value</param>
    public delegate void WriteValueDelegate(object value);

    /// <summary>
    /// Infoset Processor
    /// </summary>
    public class InfosetProcessor : XamlWriter, IXamlLineInfoConsumer
    {
        /// <summary> Xaml Writer</summary>
        private XamlWriter _writer;

        /// <summary>WriteObject Delegate </summary>
        private WriteObjectDelegate _doWriteObject = null;

        /// <summary> WriteMember Delegate</summary>
        private WriteMemberDelegate _doWriteMember = null;

        /// <summary> WriteValue Delegate</summary>
        private WriteValueDelegate _doWriteValue = null;

        /// <summary>schema Context </summary>
        private XamlSchemaContext _schemaContext = null;

        /// <summary> current LineNumber</summary>
        private int _currentLineNumber;

        /// <summary>current LinePosition </summary>
        private int _currentLinePosition;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="InfosetProcessor"/> class.
        /// </summary>
        /// <param name="schemaContext">The schema context.</param>
        public InfosetProcessor(XamlSchemaContext schemaContext)
        {
            _writer = null;
            _currentLineNumber = 0;
            _currentLinePosition = 0;
            this._schemaContext = schemaContext;
        }

        /// <summary>
        /// Gets or sets the writer.
        /// </summary>
        /// <value>The writer.</value>
        public XamlWriter Writer
        {
            set
            {
                if (_writer != null)
                {
                    throw new Exception("can't set Writer twice");
                }
                else
                {
                    _writer = value;
                }
            }

            get
            {
                return _writer;
            }
        }

        /// <summary>
        /// Gets the schema context
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
        /// Gets or sets the write object delegate.
        /// Method which will be called when we encounter a StartObject 
        /// </summary>
        /// <value>The write object delegate.</value>
        public WriteObjectDelegate WriteObjectDelegate
        {
            get
            {
                return _doWriteObject;
            }

            set
            {
                _doWriteObject = new WriteObjectDelegate(value);
            }
        }

        /// <summary>
        /// Gets or sets the write member delegate.
        /// Method which will be called when we encounter a StartObject 
        /// </summary>
        /// <value>The write member delegate.</value>
        public WriteMemberDelegate WriteMemberDelegate
        {
            get
            {
                return _doWriteMember;
            }

            set
            {
                _doWriteMember = new WriteMemberDelegate(value);
            }
        }

        /// <summary>
        /// Gets or sets the write value delegate.
        /// </summary>
        /// <value>The write value delegate.</value>
        public WriteValueDelegate WriteValueDelegate
        {
            get
            {
                return _doWriteValue;
            }

            set
            {
                _doWriteValue = new WriteValueDelegate(value);
            }
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        /// <value>The current line number.</value>
        public int CurrentLineNumber
        {
            get
            {
                return _currentLineNumber;
            }
        }

        /// <summary>
        /// Gets the current line position.
        /// </summary>
        /// <value>The current line position.</value>
        public int CurrentLinePosition
        {
            get
            {
                return _currentLinePosition;
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
                XamlObjectWriter objectWriter = _writer as XamlObjectWriter;
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

        #region XamlWriter Members

        /// <summary>
        /// Writes the namespace.
        /// </summary>
        /// <param name="namespaceDeclaration">The namespace declaration.</param>
        public override void WriteNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            if (_writer != null)
            {
                _writer.WriteNamespace(namespaceDeclaration);
            }
        }

        /// <summary>
        /// Writes the end object.
        /// </summary>
        public override void WriteEndObject()
        {
            if (_writer != null)
            {
                _writer.WriteEndObject();
            }
        }

        /// <summary>
        /// Writes the end member.
        /// </summary>
        public override void WriteEndMember()
        {
            if (_writer != null)
            {
                _writer.WriteEndMember();
            }
        }

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
        /// Writes the object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="retrieved">if set to <c>true</c> [retrieved].</param>
        public void WriteObject(XamlType xamlType, bool retrieved)
        {
            if (_doWriteObject != null && !retrieved)
            {
                _doWriteObject(xamlType, _schemaContext);
            }

            if (_writer != null)
            {
                if (retrieved)
                {
                    _writer.WriteGetObject();
                }
                else
                {
                    _writer.WriteStartObject(xamlType);
                }
            }
        }

        /// <summary>
        /// Writes the start member.
        /// </summary>
        /// <param name="property">The property.</param>
        public override void WriteStartMember(XamlMember property)
        {
            if (_doWriteMember != null)
            {
                _doWriteMember(property, _schemaContext);
            }

            if (_writer != null)
            {
                _writer.WriteStartMember(property);
            }
        }

        /// <summary>
        /// Writes the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void WriteValue(object value)
        {
            if (_doWriteValue != null)
            {
                _doWriteValue(value);
            }

            if (_writer != null)
            {
                _writer.WriteValue(value);
            }
        }

        #endregion

        #region IConsumeXamlLineInfo Members

        /// <summary>
        /// Sets the line info.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="linePosition">The line position.</param>
        public void SetLineInfo(int lineNumber, int linePosition)
        {
            _currentLineNumber = lineNumber;
            _currentLinePosition = linePosition;
        }

        #endregion

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
                    if (_writer != null)
                    {
                        _writer.Close();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
