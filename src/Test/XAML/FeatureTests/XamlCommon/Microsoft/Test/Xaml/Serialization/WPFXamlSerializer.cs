// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Windows;

namespace Microsoft.Test.Xaml.Serialization
{
    /// <summary>
    /// Wrapper class for WPF's XamlWriter
    /// </summary>
    public class WPFXamlSerializer : IXamlTestSerializer
    {
        #region IXamlTestSerializer Members

        /// <summary>
        /// Serialized the given object tree with XamlWriter
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="treeRoot">The tree root.</param>
        public void SerializeObjectTree(string fileName, object treeRoot)
        {
            if (File.Exists(fileName))
            {
                GlobalLog.LogStatus("Deleting pre-existing file: " + fileName);
                File.Delete(fileName);
            }

            SerializationHelper.SerializeObjectTree(treeRoot, XamlWriterMode.Expression, fileName);
        }

        #endregion
    }
}
