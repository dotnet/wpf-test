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
    [TestCaseArea(@"Serialization\Expression\Deep")]
    [TestCaseMethod("RunTestCase")]
    [TestCaseDisabled("0")]
    [TestCaseSupportFile("SerializationExpression.xaml")]
    public class SerializationExpressionDeep : SerializationBaseCase
    {
        private void _SetCustomExpression(object treeRoot)
        {
            DockPanel panel = (DockPanel)treeRoot;
            ExpressionElement item = (ExpressionElement)(panel.Children[0]);

            //create an empty expression object
            Assembly assembly =  typeof(DispatcherObject).Assembly;
            Type type = assembly.GetType("System.Windows.Expression");
            Object expressionObj = type.InvokeMember(null, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, null, null);

            item.SetValue(ExpressionElement.CustomExprDPProperty, expressionObj);
        }

        private void _OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            VerifyString(args.Xaml, "CustomExprDP", true);

            SerializationBaseCase.OnXamlSerialized(sender, args);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName">xaml file name</param>
        override protected void DoTheTest(String fileName)
        {
            fileName = "SerializationExpression.xaml";

            SerializationHelper helper = new SerializationHelper();
            helper.AlwaysTestExpressionMode = false;
            helper.PreFirstDisplay += new SerializationCustomerEventHandler(_SetCustomExpression);
            helper.PreSecondDisplay += new SerializationCustomerEventHandler(_SetCustomExpression);
            helper.XamlSerialized += new XamlSerializedEventHandler(_OnXamlSerialized);

            helper.RoundTripTestFile(fileName, XamlWriterMode.Value, true);
        }

        private static void VerifyString(string s, string ToVerify, bool shouldBeThere)
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
    }
    
}
