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


namespace Avalon.Test.CoreUI.Parser
{

    /// <summary>Custom Control to parse literal content</summary>
    /// Need a custom serializer, like that for NodeWithLiteralContent

    public class NodeForLiteralwithResources: UIElement
    {
        /// <summary>
        /// Property LiteralString
        /// </summary>
        public TextBlock MyText
        {
            get { return _text; }
            set { _text = value; }
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
            MyText=new TextBlock();
            System.Windows.ResourceDictionary rd = new System.Windows.ResourceDictionary();
            rd.Add("foreground", System.Windows.Media.Brushes.Red);
            MyText.Resources = rd;
            MyText.SetResourceReference(TextBlock.ForegroundProperty, "foreground");

        }
        /// <summary>
        /// literalstring
        /// </summary>
        public TextBlock _text = null;
    }
}
