// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Tests for MarkupExtensionBracketCharacterAttribute
    /// </summary>
    public static class BracketCharactersAttributeTests
    {
        /// <summary>Xaml Reader </summary>
        private static System.Xaml.XamlReader s_reader;

        /// <summary>
        /// Entry point for MarkupExtensionBracketCharacterAttribute tests
        /// </summary>
        public static void RunTest()
        {
            // The below two instantiations are done so as to get the containing assemblies to load.
            FrameworkElement fe = new FrameworkElement();
            CustomAttached att = new CustomAttached();
            XmlReader xmlReader = XmlReader.Create("BracketCharacters.xaml");
            
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);
            s_reader = xtr;
            fe = null;
            att = null;
            Grid grid = System.Windows.Markup.XamlReader.Load(xtr) as Grid;
            VerifyResults(grid);
        }

        /// <summary>
        /// Verifies whether the markup extension was parsed correctly
        /// </summary>
        /// <param name="grid">Type of the xaml.</param>
        public static void VerifyResults(Grid grid)
        {
            foreach (UIElement element in grid.Children)
            {
                TextBlock textBlock = element as TextBlock;
                if (textBlock != null)
                {
                    string actual = textBlock.Text;
                    string expected = textBlock.Tag as string;
                    GlobalLog.LogEvidence(string.Format("Actual : {0} , Expected : {1}", actual, expected));
                    if (actual != expected)
                    {
                        TestLog.Current.Result = TestResult.Fail;
                        return;
                    }
                }
            }
        }
    }
}
