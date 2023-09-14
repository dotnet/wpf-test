// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using Avalon.Test.CoreUI.Parser;
using System.Windows.Markup;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Windows;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>


    public class AliasPropertyRepro : SerializationBaseCase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">xaml file name</param>
        override protected void DoTheTest(String filename)
        {
            CoreLogger.BeginVariation();
            using (CoreLogger.AutoStatus("Constructing Object Tree from code ... " ))
            {
                CustomCanvas myCanvas = new CustomCanvas();
                CustomItem1 item1 = new CustomItem1();
                item1.SetValue(CustomItem1.AliasDPProperty, "property value");
                myCanvas.Children.Add(item1);
                _firstTreeRoot = myCanvas;
            }


            using (CoreLogger.AutoStatus("Test Serialized File"))
            {
                TestSerializedFile1(_firstTreeRoot);
            }
            using (CoreLogger.AutoStatus("Serializing the first tree root..."))
            {
                try
                {
                    SerializationHelper.SerializeObjectTree(_firstTreeRoot, _tempXamlFileName);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Error occur while serializing tree.", e);
                }
            }

            using (CoreLogger.AutoStatus("Parsing the serialized xaml..."))
            {
                try
                {
                    _secondTreeRoot = SerializationHelper.ParseXamlFile(_tempXamlFileName);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Error occurred while parsing serialized xaml.", e);
                }
            }

            using (CoreLogger.AutoStatus("Test Serialized File"))
            {
                TestSerializedFile1(_secondTreeRoot);
            }

            using (CoreLogger.AutoStatus("Serializing the second tree root..."))
            {
                try
                {
                    SerializationHelper.SerializeObjectTree(_secondTreeRoot, _tempXamlFileName2);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Error occur while serializing tree.", e);
                }
            }

            using (CoreLogger.AutoStatus("Comparing xaml files ..."))
            {
                try
                {
                    XamlComparer.CompareFiles(_tempXamlFileName, _tempXamlFileName2);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Two Xaml files are different.", e);
                }
            }
            CoreLogger.EndVariation();
        }
        private void TestSerializedFile1(object root)
        {
            _outXmlStr = SerializationHelper.SerializeObjectTree(root);
            CoreLogger.LogStatus(_outXmlStr);
            CheckProperty(_outXmlStr, "AliasDP", true);
            CheckProperty(_outXmlStr, "CustomItem2.AliasDP", false);
        }

        private void CheckProperty(string sourcestr, string propertystr, bool serialized)
        {
            if (sourcestr.Contains(propertystr) != serialized)
            {
                throw new Microsoft.Test.TestValidationException("Should " + (serialized?"" :"not ") + " serialize property: " + propertystr);
            }
        }

        private object _firstTreeRoot;
        private object _secondTreeRoot;
        String _outXmlStr;
        string _tempXamlFileName2 = "____tempXamlFile2____.xaml";
        String _tempXamlFileName = "___tempXamlFile____.xaml";
    }
}
