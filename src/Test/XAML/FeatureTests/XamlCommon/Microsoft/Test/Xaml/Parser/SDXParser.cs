// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xaml;

namespace Microsoft.Test.Xaml.Parser
{
    /// <summary>
    /// SDXParser class
    /// </summary>
    public class SDXParser : IXamlTestParser
    {
        /// <summary>
        /// enum XamlParseMode
        /// </summary>
        public enum XamlParseMode
        {
            /// <summary>
            /// Load = 0 value
            /// </summary>
            Load
        }

        #region IXamlTestParser Members

        /// <summary>
        /// Returns the root element of the tree created by the xaml parser.
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="xamlParseParam">object value.</param>
        /// <returns>object value</returns>
        public object LoadXaml(string xamlFileName, object xamlParseParam)
        {
            return XamlServices.Load(xamlFileName);
        }

        /// <summary>
        /// Compares the tree loaded by LoadXaml with a tree loaded with a different mode
        /// </summary>
        /// <param name="xamlFileName">string value</param>
        /// <param name="rootElement">object value.</param>
        /// <param name="xamlParseMode">object value xamlParseMode</param>
        /// <returns>object value</returns>
        public bool CompareXamlTrees(string xamlFileName, object rootElement, object xamlParseMode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns each mode of the Xaml parser once
        /// </summary>
        /// <param name="selectedParseMode">The user selected parse mode</param>
        /// <returns>IEnumerable value</returns>
        public IEnumerable<object> ParseModes(string selectedParseMode)
        {
            yield return XamlParseMode.Load;
        }

        /// <summary>
        /// Given an SRID name this will extract the exception string
        /// </summary>
        /// <param name="sridName">string value</param>
        /// <returns>string value.</returns>
        public string ExtractExceptionMessage(string sridName)
        {
            // Get the assembly of the parser
            Assembly assem = Assembly.GetAssembly(typeof(System.Xaml.XamlReader));
            if (assem == null)
            {
                throw new Exception("Assembly containing System.Xaml.XamlReader is not found");
            }

            Type sridType = assem.GetType("System.Xaml.SRID");
            if (sridType == null)
            {
                throw new Exception("System.Xaml.SRID type not found in assembly " + assem.FullName);
            }

            // Get the field of type SRID sridName on the type SRID
            FieldInfo sridFI = sridType.GetField(sridName);
            if (sridFI == null)
            {
                throw new Exception("Field named " + sridName + " is not found in SRID type");
            }

            // Get the actual SRID object
            object sridObject = sridFI.GetValue(sridType);
            if (sridObject == null)
            {
                throw new Exception("Cannot get value of " + sridFI.Name + " field in SRID type");
            }

            Type sysSrType = assem.GetType("System.Xaml.SR");
            if (sysSrType == null)
            {
                throw new Exception("System.Xaml.SR type not found in assembly " + assem.FullName);
            }

            // Get the method: internal static string SR.Get(SRID id, object[] args)
            MethodInfo sridGetMethod = sysSrType.GetMethod("Get", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (sridGetMethod == null)
            {
                throw new Exception("Get method not found in SR type");
            }

            // Invoke the Get method, this will return a string formatted in the CurrentUICulture
            return sridGetMethod.Invoke(sridGetMethod, new object[] { sridObject }) as string;
        }

        #endregion
    }
}
