// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Parser
{
    /// <summary>
    /// Interface for implementing a Xaml Parser wrapper
    /// </summary>
    public interface IXamlTestParser
    {
        /// <summary>
        /// Returns the root element of the tree created by the xaml parser.
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="xamlParseParam">object value.</param>
        /// <returns>object value</returns>
        object LoadXaml(string xamlFileName, object xamlParseParam);

        /// <summary>
        /// Compares the tree loaded by LoadXaml with a tree loaded with a different mode
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="rootElement">object value.</param>
        /// <param name="xamlParseMode">object value xamlParseMode</param>
        /// <returns>object value</returns>
        bool CompareXamlTrees(string xamlFileName, object rootElement, object xamlParseMode);

        /// <summary>
        /// Returns each mode of the Xaml parser once
        /// </summary>
        /// <param name="parseMode">Decides whether to run all the parse modes</param>
        /// <returns>IEnumerable value</returns>
        IEnumerable<object> ParseModes(string parseMode);

        /// <summary>
        /// Given and SRID name this will extract the exception string 
        /// </summary>
        /// <param name="sridName">string value</param>
        /// <returns>string value.</returns>
        string ExtractExceptionMessage(string sridName);
    }
}
