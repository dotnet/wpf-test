// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using System.Windows.Markup;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;
using Avalon.Test.CoreUI.Serialization;
using System.ComponentModel;

namespace Avalon.Test.CoreUI.Parser
{

    /// <summary>Custom Control to parse literal content</summary>
    /// Todo: need to override methods control the parsing of literal content
    /// and enable this case later
	public class NodeWithLiteralContent: UIElement
    {
        /// <summary>
        /// Property LiteralString
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public string LiteralString
        {
            get { return _literalString; }
            set { _literalString = value; }
        }
        /// <summary>
        /// For the ultimate in control over markup, a component can 
        /// implement IParseLiteralContent.  This interface gives the component 
        /// complete control over parsing of children.
        /// The component itself, and attributes on the components tag, 
        /// are not part of the literal content and are parsed by the usual 
        /// Avalon parsing logic.
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="parserContext"></param>
        public void Parse(XmlTextReader textReader, ParserContext parserContext)
        {
			textReader.WhitespaceHandling = WhitespaceHandling.None;
			textReader.Read();
            _literalString = textReader.Value;
        }
        /// <summary>
        /// literalstring
        /// </summary>
        public string _literalString = "";
    }
}
