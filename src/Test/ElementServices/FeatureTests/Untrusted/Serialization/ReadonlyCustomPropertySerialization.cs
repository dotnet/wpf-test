// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;
using System.Windows.Markup;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.InteropServices;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to handle serialization and its verification for Read Only propertyies
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  ReadonlyCustomPropertySerialization.cs
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    public class ReadonlyCustomPropertySerialization : SerializationBaseCase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">xaml file name</param>
        override protected void DoTheTest(String filename)
        {
            CoreLogger.BeginVariation();
            filename = "ReadonlyCustomPropertySerialization.xaml";

            using (CoreLogger.AutoStatus("Constructing Object Tree ... from xaml file: " + filename))
            {
                try
                {
                    _firstTreeRoot = SerializationHelper.ParseXamlFile(filename);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Error occur while constructing tree" + e.Message);
                }
                if (null == _firstTreeRoot)
                    throw new Microsoft.Test.TestValidationException("Tree root from ConstructObjectTree is null. Forgot to override ConstructObjectTree?");
            }

            using (CoreLogger.AutoStatus("Displaying the first tree root"))
            {
                _serhelper.DisplayTree(_firstTreeRoot as UIElement);
            }
            using (CoreLogger.AutoStatus("Serialize out ..."))
            {
                SerializationHelper.SerializeObjectTree(_firstTreeRoot, _tempXamlFileName);
            }



            using (CoreLogger.AutoStatus("Test Serialized File"))
            {
                TestSerializedFile();
            }
            CoreLogger.EndVariation();
        }
        private void TestSerializedFile()
        {
            _outXmlStr = SerializationHelper.SerializeObjectTree(_firstTreeRoot);
            Console.WriteLine(_outXmlStr);
            CheckProperty(_outXmlStr, "MyReadOnlyDP", false);
            CheckProperty(_outXmlStr, "MyReadOnlyProperty", false);
            CheckProperty(_outXmlStr, "MyContentDP", true);
            CheckProperty(_outXmlStr, "MyContentClrProperty", true);
            CheckProperty(_outXmlStr, "MyShouldDP", false);
            CheckProperty(_outXmlStr, "should serialized ReadOnly DP", false);
            CheckProperty(_outXmlStr, "MyShouldNotDP", false);
            CheckProperty(_outXmlStr, "Red", true);
            CheckProperty(_outXmlStr, "MyShouldNotSerialized", false);
            CheckProperty(_outXmlStr, "DP with clr accessor", true);
        }

        private void CheckProperty(string sourcestr, string propertystr, bool serialized)
        {
            if (sourcestr.Contains(propertystr) != serialized)
            {
                throw new Microsoft.Test.TestValidationException("Should " + (serialized ? "" : "not ") + " serialize property: " + propertystr);
            }
            else
            {
                CoreLogger.LogStatus(propertystr +" OK");
            }
        }


        private object _firstTreeRoot;
        String _outXmlStr;
        String _tempXamlFileName = "___tempXamlFile____.xaml";
    }
    


}
