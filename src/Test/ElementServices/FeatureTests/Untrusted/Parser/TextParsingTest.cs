// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.Source;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// 
    /// </summary>
    public class TextParsingTest
    {
        /// <summary>
        /// This method tests that Text under a CLR object implementing IList (and not IAddChild)
        /// throws an exception while loading BAML.
        /// We ignore the exception thrown by XamlTokenReader (using a custom deserializer), so that
        /// it reaches the BAML loading stage.
        /// </summary>
                public void Text_under_IList_Clr_Object()
        {
            try
            {
                ParserUtil.ParseXamlFile("Text_under_IList_Clr_Object.xaml", 
                    new ParserContext());
                throw new Microsoft.Test.TestValidationException("Exception was expected because text is not allowed under IList, but not thrown");
            }
            catch (XamlParseException)
            { }
        }

        /// <summary>
        /// This method tests that Text under a CLR object not implementing IAddChild/IList
        /// but under an IEnumerable property of an IAddChild, is accepted.
        /// We ignore the exception thrown by XamlTokenReader (using a custom deserializer), so that
        /// it reaches the BAML loading stage.
        /// </summary>
                public void Text_under_Clr_under_IEnum_prop()
        {
            try
            {
                ParserUtil.ParseXamlFile("Text_under_Clr_under_IEnum_prop.xaml",
                    new ParserContext());
            }
            catch (XamlParseException e)
            { throw e;}
        }
    }
}
