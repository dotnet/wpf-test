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
    /// Disabled result from: Expressions removed from public use, task 27253
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  SerializationEvent.cs
    /// </remarks>
    ///
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("0")]
    [TestCaseArea(@"Serialization\Expression")]
    [TestCaseMethod("RunTestCase")]
    [TestCaseDisabled("1")]
    [TestCaseSupportFile("SerializationExpression.xaml")]
    public class SerializationExpressionShallow : SerializationBaseCase
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">xaml file name</param>
        override protected void DoTheTest(String filename)
        {
            filename = "SerializationExpression.xaml";

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

            using (CoreLogger.AutoStatus("Set expression... "))
            {
                DockPanel panel = _firstTreeRoot as DockPanel;
                ExpressionElement item = (ExpressionElement)(panel.Children[0]);

                
                //create an empty expression object
                Assembly assembly =  typeof(DispatcherObject).Assembly;
                Type type = assembly.GetType("System.Windows.Expression");
                Object expressionObj = type.InvokeMember(null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);

                item.SetValue(ExpressionElement.CustomExprDPProperty, expressionObj);
            }


            using (CoreLogger.AutoStatus("Serialize"))
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);

                // Create XmlTextWriter
                XmlTextWriter xmlWriter = new XmlTextWriter(writer);

                // Create Serialization Manager
                _manager = new XamlDesignerSerializationManager(xmlWriter);
                _manager.XamlWriterMode = XamlWriterMode.Expression;
                Window win = new Window();
                win.Content = _firstTreeRoot;
                SerializationHelper.SerializeObjectTree((object)_firstTreeRoot, _manager);
                xmlWriter.Close();
                writer.Close();
                s_outStr = sb.ToString();
            }

            using (CoreLogger.AutoStatus("Verifying and deleting Expression ..."))
            {
                //RemoveStrings();
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

            using (CoreLogger.AutoStatus("Set expression... "))
            {
                DockPanel panel = _firstTreeRoot as DockPanel;
                ExpressionElement item = (ExpressionElement)(panel.Children[0]);

                
                //create an empty expression object
                Assembly assembly =  typeof(DispatcherObject).Assembly;
                Type type = assembly.GetType("System.Windows.Expression");
                Object expressionObj = type.InvokeMember(null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);
                
                item.SetValue(ExpressionElement.CustomExprDPProperty, expressionObj);
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
                _manager.XamlWriterMode = XamlWriterMode.Expression;
                Window win = new Window();
                win.Content = _secondTreeRoot;
                SerializationHelper.SerializeObjectTree((object)_secondTreeRoot, _manager);
                xmlWriter.Close();
                writer.Close();
                s_outStr = sb.ToString();

            }

            using (CoreLogger.AutoStatus("Verifying and deleting Expression ..."))
            {
                RemoveStrings();
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


        private static void RemoveStrings()
        {

            VerifyandDeleteString(ref s_outStr, "CustomExprDP=\"[identity]\"", true);
        }
        private static void VerifyandDeleteString(ref string s, string ToVerify, bool shouldBeThere)
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

            if (shouldBeThere)
            {
                int startIndex = s.LastIndexOf(" ", index);
                int endIndex = s.IndexOf(" ", index);
                s = s.Remove(startIndex, (endIndex-startIndex));
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
