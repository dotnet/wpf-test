// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xaml;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// DiagXaml Parser
    /// </summary>
    public class DiagXamlParser
    {
        /// <summary>Xaml Writer </summary>
        private readonly XamlWriter _writer;

        /// <summary> diag XamlString</summary>
        private readonly string _diagXamlString;

        /// <summary>Reader Context </summary>
        private readonly ReaderContext<XamlBaseFrame> _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagXamlParser"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="path">The path value.</param>
        public DiagXamlParser(XamlWriter writer, string path)
        {
            this._writer = writer;
            _context = new ReaderContext<XamlBaseFrame>();
            StreamReader reader = new StreamReader(path);
            _diagXamlString = reader.ReadToEnd();
            reader.Close();
        }

        /// <summary>
        /// Gets the schema context.
        /// </summary>
        /// <value>The schema context.</value>
        public XamlSchemaContext SchemaContext
        {
            get
            {
                return _context.SchemaContext;
            }
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        public void Parse()
        {
            DiagXamlScanner scanner = new DiagXamlScanner(_diagXamlString);
            List<string[]> instructionlist = scanner.Read();

            foreach (string[] instruction in instructionlist)
            {
                switch (instruction[0])
                {
                    case "NS":
                        ParseNS(instruction);
                        break;
                    case "SO":
                        ParseSO(instruction);
                        break;
                    case "EO":
                        ParseEO(instruction);
                        break;
                    case "SM":
                        ParseSP(instruction);
                        break;
                    case "EM":
                        ParseEP(instruction);
                        break;
                    case "V":
                        ParseT(instruction);
                        break;
                    case "Close":
                        _writer.Close();
                        break;
                    default:
                        throw new Exception("Invalid tag: " + instruction[0]);
                }
            }
        }

        /// <summary>
        /// Parses the T.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseT(string[] instruction)
        {
            // CodeReview - do we need to change the argument at all?
            _writer.WriteValue(instruction[1]);
        }

        /// <summary>
        /// Parses the EP.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseEP(string[] instruction)
        {
            _context.PopFrame();
            _writer.WriteEndMember();
        }

        /// <summary>
        /// Parses the SP.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseSP(string[] instruction)
        {
            XamlMember property = _context.CurrentType.GetMember(instruction[2]);
            if (property == null)
            {
                if (instruction.Length > 3 && instruction[3] == "IMPLICIT")
                {
                    property = _context.GetXamlDirective(XamlLanguage.Xaml2006Namespace, instruction[2]);
                }
                else
                {
                    property = new XamlMember(instruction[2], _context.CurrentType, false /*isAttachable*/);
                }
            }

            _context.PushFrame(property);
            _writer.WriteStartMember(property);
        }

        /// <summary>
        /// Parses the EO.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseEO(string[] instruction)
        {
            _context.PopFrame();
            _writer.WriteEndObject();
        }

        /// <summary>
        /// Parses the SO.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseSO(string[] instruction)
        {
            bool retrieved = false;
            XamlType xamlType = _context.FindXamlType(instruction[1], instruction[2]);
            if (instruction.Length > 3 && instruction[3] == "RETRIEVED")
            {
                retrieved = true;
            }

            _context.PushFrame(xamlType);
            if (retrieved)
            {
                _writer.WriteGetObject();
            }
            else
            {
                _writer.WriteStartObject(xamlType);
            }
        }

        /// <summary>
        /// Parses the NS.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private void ParseNS(string[] instruction)
        {
            _context.PushFrame(instruction[2], instruction[1]);
            _writer.WriteNamespace(new NamespaceDeclaration(instruction[2], instruction[1]));
        }
    }
}
