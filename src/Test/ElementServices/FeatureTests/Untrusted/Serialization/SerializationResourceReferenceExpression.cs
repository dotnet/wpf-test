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
using Avalon.Test.CoreUI.Source;
using System.ComponentModel;
using System.Windows.Data;
using System.Text;
using Microsoft.Test.Win32;
using Microsoft.Test.Xml;
using Microsoft.Test.Serialization;
using Microsoft.Test.Windows;
namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to verify serialization of Events
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  SerializationEvent.cs
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    ///
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("0")]
    [TestCaseArea(@"Serialization\Expression\Reference")]
    [TestCaseMethod("RunTestCase")]
    [TestCaseDisabled("0")]
    [TestCaseTimeout("300")]
    [TestCaseSupportFile("SerializationResourceReferenceExpression.xaml")]
    public class SerializationResourceReferenceExpression : SerializationBaseCase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">xaml file name</param>
        override protected void DoTheTest(String filename)
        {
            filename = "SerializationResourceReferenceExpression.xaml";

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

            using (CoreLogger.AutoStatus("Serialize"))
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);

                // Create XmlTextWriter
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);

                // Create Serialization Manager
                _manager = new XamlDesignerSerializationManager(xmlWriter);
                SerializationHelper.SerializeObjectTree((object)_firstTreeRoot, _manager);
                xmlWriter.Close();
                writer.Close();
                s_outStr = sb.ToString();
            }

            using (CoreLogger.AutoStatus("Verifying and deleting Expression ..."))
            {
                VerifyStrings();
            }
            using (CoreLogger.AutoStatus("save file ..."))
            {
                StreamWriter sr = File.CreateText(_tempXamlFileName);

                sr.WriteLine(s_outStr);
                sr.Close();
            }
            
            using (CoreLogger.AutoStatus("Parsing the serialized xaml..."))
            {
                try
                {
                    _secondTreeRoot = SerializationHelper.ParseXamlFile(_tempXamlFileName);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Error occurred while parsing serialized xaml."+e.Message);
                }
            }

            using (CoreLogger.AutoStatus("Comparing Object Trees..."))
            {
                TreeCompareResult result = TreeComparer.CompareLogical(_firstTreeRoot, _secondTreeRoot);

                if (CompareResult.Different == result.Result)
                {
                    throw new Microsoft.Test.TestValidationException("Fail while comparing object tree.");
                }
            }

            using (CoreLogger.AutoStatus("Serializing the second tree root..."))
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);

                // Create XmlTextWriter
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);

                // Create Serialization Manager
                _manager = new XamlDesignerSerializationManager(xmlWriter);
                SerializationHelper.SerializeObjectTree((object)_secondTreeRoot, _manager);
                xmlWriter.Close();
                writer.Close();
                s_outStr = sb.ToString();

            }

            using (CoreLogger.AutoStatus("Verifying Expression ..."))
            {
                VerifyStrings();
            }

            using (CoreLogger.AutoStatus("save file ..."))
            {
                StreamWriter sr = File.CreateText(_tempXamlFileName2);

                sr.WriteLine(s_outStr);
                sr.Close();
            }

            using (CoreLogger.AutoStatus("Comparing xaml files ..."))
            {
                try
                {
                    XamlComparer.CompareFiles(_tempXamlFileName2, _tempXamlFileName2);
                }
                catch (Exception e)
                {
                    throw new Microsoft.Test.TestValidationException("Two Xaml files are different.", e);
                }
            }
        }


        private static void VerifyStrings()
        {

            VerifyString(ref s_outStr, "ResourceRefDP1", true);
            VerifyString(ref s_outStr, "ResourceRefDP2", true);
        }

        private static void VerifyString(ref string s, string ToVerify, bool shouldBeThere)
        {
            int index = s.LastIndexOf(ToVerify);

            if (shouldBeThere && index == -1)
            {
                throw new Microsoft.Test.TestValidationException("Not found: " + ToVerify);
            }
            else if (!shouldBeThere && index != -1)
            {
                throw new Exception("Should not found " + ToVerify);
            }
        }   
        Object _firstTreeRoot = null;
        Object _secondTreeRoot = null;
        String _tempXamlFileName2 = "___tempXamlFile2____.xaml";
        String _tempXamlFileName = "___tempXamlFile____.xaml";

        XamlDesignerSerializationManager _manager = null;

        static string s_outStr = null;
    }
    
}
