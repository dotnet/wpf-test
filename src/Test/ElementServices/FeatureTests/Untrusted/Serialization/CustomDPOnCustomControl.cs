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
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// A class to handle serialization and its verification for compact syntax and databinding
    /// <para />
    /// </summary>
    /// <remarks>
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  SerializationDataBindingCase.cs
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    public class CustomDPOnCustomControl : SerializationBaseCase
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="filename">xaml file name</param>
        override protected void DoTheTest(String filename)
        {
            CoreLogger.BeginVariation();
            CheckAXaml("CustomDPOnCustomControl.xaml");
            CheckAXaml("CustomDPOnCustomControl2.xaml");
            CoreLogger.EndVariation();
        }
        private void CheckAXaml(string filename)
        {
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

            using (CoreLogger.AutoStatus("Test property of first tree root"))
            {
                TestFirstNodeProperties(_firstTreeRoot);
            }

            using (CoreLogger.AutoStatus("Serialize out ..."))
            {
                SerializationHelper.SerializeObjectTree(_firstTreeRoot, _tempXamlFileName);
            }

            using (CoreLogger.AutoStatus("Test Serialized File"))
            {
                TestSerializedFile1();
            }

        }

        private void TestFirstNodeProperties(object root)
        {
            DockPanel myPanel = ((Page)root).Content as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Should has 2 children");
            }
        }
        private void TestSerializedFile1()
        {
            _outXmlStr = SerializationHelper.SerializeObjectTree(_firstTreeRoot);
            CheckProperty(_outXmlStr, "MyHiddenDP", false);
            CheckProperty(_outXmlStr, "MyContentDP", true);
            CheckProperty(_outXmlStr, "MyShouldDP", true);
            CheckProperty(_outXmlStr, "MyShouldNotDP", false);
        }

        private void CheckProperty(string sourcestr, string propertystr, bool serialized)
        {
            if (sourcestr.Contains(propertystr) != serialized)
            {
                throw new Microsoft.Test.TestValidationException("Should " + (serialized?"" :"not ") + " serialize property: " + propertystr);
            }
        }


        private object _firstTreeRoot;
        String _outXmlStr;
        String _tempXamlFileName = "___tempXamlFile____.xaml";
    }
    /// <summary>
    /// 
    /// </summary>
    public class MyCustomControl : UIElement
    {
        /// <summary>
        /// 
        /// </summary>
        public MyCustomControl():base() { }
    }
    



}
