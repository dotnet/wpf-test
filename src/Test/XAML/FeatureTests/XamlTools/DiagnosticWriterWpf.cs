// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

using System.Xaml;
using System.Windows.Documents;
using System.Text;
using System.Windows.Media;

namespace XamlTools
{
    public class DiagnosticWriterWpf : XamlWriter
    {
        FlowDocument _flowDoc;
        int _depth = 0;
        string _indentUnit;
        XamlSchemaContext _schemaContext;
        SimpleWriterStack _stack;
        Paragraph _paragraph;

        public DiagnosticWriterWpf(FlowDocument flowDocument)
        {
            _indentUnit = " ";
            _stack = new SimpleWriterStack();

            ImplicitColor = Colors.Green;
            UnknownColor = Colors.Red;
            _flowDoc = flowDocument;
            _flowDoc.FontFamily = new FontFamily("Consolas");
            _flowDoc.FontSize = 10.5;
            _paragraph = new Paragraph();
            _flowDoc.Blocks.Add(_paragraph);
        }

        public bool ShowCloseComments { get; set; }
        public Color ImplicitColor { get; set; }
        public Color UnknownColor { get; set; }

        public string IndentUnit
        {
            get { return _indentUnit; }
            set { _indentUnit = value; }
        }

        #region XamlWriter

        override public void WriteObject(XamlType xamlType, bool isRetrieved)
        {
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            sb.Append("SO ");

            SimpleWriterFrame frame;
            if (_stack.Count == 0 || _stack.Peek().Type != null)
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
                sb.Append("*NULL*");
            }
            else
            {
                sb.AppendFormat("{0}", xamlType.Name);
                if (isRetrieved)
                {
                    color = ImplicitColor;
                }

                if (xamlType.IsUnknown)
                {
                    color = UnknownColor;
                }
            }

            if(isRetrieved)
            {
                color = ImplicitColor;
            }

            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
            ++_depth;
        }

        override public void WriteEndObject()
        {
            --_depth;
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            sb.Append("EO");

            XamlType xamlType = _stack.Peek().Type;
            _stack.Pop();

            if (ShowCloseComments)
            {
                sb.Append("        // ");
                if (xamlType == null)
                {
                    sb.Append("*NULL*");
                }
                else
                {
                    sb.AppendFormat("{0}", xamlType.Name);
                    // Disabling the coloring of close tag for now.
                    // would need to do via stack, since this is no longer on XamlType.IsImplicit,
                    // but now XR.IsRetrievedObject.
                    //if (xamlType.IsImplicit)
                    //{
                    //    color = ImplicitColor;
                    //}

                    if (xamlType.IsUnknown)
                    {
                        color = UnknownColor;
                    }
                }
            }

            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }

        override public void WriteMember(XamlProperty property)
        {
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            sb.Append("SM ");

            _stack.Peek().Member = property;

            if (property == null)
            {
                sb.Append("*NULL*");
            }
            else
            {
                if (property.IsAttachable)
                {
                    XamlType ownerType = property.DeclaringType;
                    sb.AppendFormat("{0}.", ownerType.Name);
                }
                sb.AppendFormat("{0}", property.Name);

                if (property.IsImplicit)
                {
                    color = ImplicitColor;
                }

                if (property.IsUnknown)
                {
                    color = UnknownColor;
                }
            }

            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
            ++_depth;
        }

        override public void WriteEndMember()
        {
            --_depth;
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            sb.Append("EM");

            XamlProperty property = _stack.Peek().Member;
            _stack.Peek().Member = null;

            if (ShowCloseComments)
            {
                sb.Append("        // ");
                if (property == null)
                {
                    sb.Append("*NULL*");
                }
                else
                {
                    sb.AppendFormat("{0}", property.Name);
                    if (property.IsImplicit)
                    {
                        color = ImplicitColor;
                    }

                    if (property.IsUnknown)
                    {
                        color = UnknownColor;
                    }
                }
            }

            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }

        override public void WriteValue(object value)
        {
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            string valueString = value as string;
            if (value == null)
            {
                sb.AppendFormat("V null");
            }
            else if (valueString != null)
            {
                sb.AppendFormat("V \"{0}\"", valueString);
            }
            else
            {
                sb.AppendFormat("V ({0})  [{1}]", value.ToString(), value.GetType().Name);
            }

            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }

        override public void WriteNamespace(XamlNamespace xamlNs, string prefix)
        {
            _paragraph.Inlines.Add(Indent());
            StringBuilder sb = new StringBuilder();
            Color color = Colors.Black;

            SimpleWriterFrame frame;
            if (_stack.Count == 0)
            {
                frame = new SimpleWriterFrame();
                _stack.Push(frame);
            }
            else
            {
                frame = _stack.Peek();
                if (frame.Type != null)
                {
                    frame = new SimpleWriterFrame();
                    _stack.Push(frame);
                }
            }
            frame.AddNamespaceTableEntry(xamlNs.TargetNamespace, prefix);

            if (xamlNs == null || prefix == null)
            {
                string prefixString = (prefix == null ? "*NULL*" : prefix);
                string nsString = (xamlNs == null ? "*NULL*" : xamlNs.TargetNamespace);
                sb.AppendFormat("NS  {0}={1}", prefixString, nsString);
            }
            else
            {
                sb.AppendFormat("NS  prefix='{0}'   {1}", prefix, xamlNs.TargetNamespace);
            }
            Run run = new Run(sb.ToString());
            run.Foreground = new SolidColorBrush(color);
            _paragraph.Inlines.Add(run);
            _paragraph.Inlines.Add(new LineBreak());
        }

        override public void Close()
        {
#if NO
            Indent();
            _flowDoc.WriteLine("Closed.");
#endif
        }

        override public object Result { get { return null; } }

        public override XamlSchemaContext SchemaContext
        {
            get { return _schemaContext; }
            set
            {
                CheckSettingSchemaContext(_schemaContext, value);
                _schemaContext = value;
            }
        }
        
        #endregion

        // =========== private ===============

        private Run Indent()
        {
            string indent = String.Empty;
            for(int i=0; i<_depth; i++)
            {
                indent = indent + _indentUnit;
            }
            return new Run(indent);
        }
    }
}
