// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Serialization;
using System.Windows.Automation;
using System.Windows.Markup;

namespace Avalon.Test.CoreUI.UtilityHelper
{
    /// <summary>
    /// Helper class contains internal static helper methods
    /// Pet stands for property engine testing.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Used to check that the condition is true. If not, NewTestValidationException
        /// is thrown and this test case would Fail
        /// </summary>
        /// <param name="condition">true to prevents test from failing. False otherwise.</param>
        /// <param name="message">Used to notify what error has caused test to fail. Also passed in as exception message</param>
        internal static void Assert(bool condition, string message)
        {
            if (condition)
            {
                PrintStatus("[Vevified OK] " + message, System.ConsoleColor.Blue);
            }
            else
            {
                string failMessage = "[Verification Failed] " + message;

                PrintStatus(failMessage, System.ConsoleColor.Red);
                throw NewTestValidationException(failMessage);
            }
        }

        /// <summary>
        /// Exception builder method for Microsoft.Test.TestValidationException
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal static Microsoft.Test.TestValidationException NewTestValidationException(string message)
        {
            return new Microsoft.Test.TestValidationException(message);
        }

        /// <summary>
        /// Helper method to throw ExpectedExceptionNotReceived error condition
        /// </summary>
        /// <returns></returns>
        internal static Microsoft.Test.TestValidationException ExpectedExceptionNotReceived()
        {
            throw new Microsoft.Test.TestValidationException("Expected Exception is not received. Test validation failed.");
        }

        /// <summary>
        /// Helper funtion to show the expected exception message
        /// </summary>
        /// <param name="ex"></param>
        internal static void ExpectedExceptionReceived(Exception ex)
        {
            Utilities.PrintStatus("Get " + ex.GetType().ToString() + ": " + ex.Message, System.ConsoleColor.Blue);
        }

        /// <summary>
        /// Print out status message
        /// </summary>
        /// <param name="message">message to be printed</param>
        internal static void PrintStatus(string message)
        {
            PrintStatus(message, System.ConsoleColor.White);
        }

        /// <summary>
        /// Print out status with color
        /// </summary>
        /// <param name="message">message to be printed</param>
        /// <param name="foreColor">color to be printed in</param>
        internal static void PrintStatus(string message, System.ConsoleColor foreColor)
        {
            CoreLogger.LogStatus(message, foreColor);
        }

        /// <summary>
        /// Add format to make title more visible
        /// </summary>
        /// <param name="title">Title to be printed out</param>
        internal static void PrintTitle(string title)
        {
            PrintStatus("");
            if (s_runAllEnabled)
            {
                s_totalTestFunction++;
                PrintStatus("[" + s_runAllGroupName + "] No." + s_totalTestFunction.ToString() + " Test", System.ConsoleColor.Green);
            }
            PrintStatus("Title: " + title, System.ConsoleColor.Green);
        }

        /// <summary>
        /// Given a property engine, displays its GlobalIndex, name, uri, type and default value
        /// </summary>
        /// <param name="dp"></param>
        internal static void PrintDependencyProperty(System.Windows.DependencyProperty dp)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(100);

            sb1.Append("[PrintDP] Name:");
            sb1.Append(dp.Name);
            sb1.Append(System.Environment.NewLine);
            sb1.Append("GlobalIndex:");
            sb1.Append(dp.GlobalIndex);
            sb1.Append(System.Environment.NewLine);
            sb1.Append("OwnerType:");
            sb1.Append(dp.OwnerType);
            sb1.Append(System.Environment.NewLine);
            sb1.Append("PropertyType:");
            sb1.Append(dp.PropertyType);
            sb1.Append(System.Environment.NewLine);
            sb1.Append("DefaultMetadata: (see next line)");

            //DefaultMetadata can never be null, throw exception if it is
            if (dp.DefaultMetadata == null)
            {
                throw Utilities.NewTestValidationException("DependencyProperty.DefaultMetadata should Never be NULL!");
            }

            PrintStatus(sb1.ToString());
        }

        /// <summary>
        /// Return a string that consists of Name, name and SystemType of the given
        /// DependencyObjectType
        /// </summary>
        /// <param name="dType">DependencyObjectType of interest</param>
        /// <returns>a string that consists of Name, name and SystemType </returns>
        private static string DependencyObjectTypeTag(System.Windows.DependencyObjectType dType)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(100);

            sb1.Append("(");
            sb1.Append("Name:");
            sb1.Append(dType.Id);
            sb1.Append("; Name:");
            sb1.Append(dType.Name);
            sb1.Append("; SystemType:");
            sb1.Append(dType.SystemType);
            sb1.Append(")");
            return sb1.ToString();
        }

        /// <summary>
        /// Print out information about the given DependencyObjectType. Also
        /// show BaseType chain.
        /// </summary>
        /// <param name="dType">DependencyObjectType of interest</param>
        internal static void PrintDependencyObjectType(System.Windows.DependencyObjectType dType)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(100);

            sb1.Append("Now Print DependencyObjectType (");
            sb1.Append(dType.Name);
            sb1.Append(")");
            sb1.Append(System.Environment.NewLine);
            sb1.Append("Current Info: ");
            sb1.Append(DependencyObjectTypeTag(dType));

            int nSpace = 0;
            System.Windows.DependencyObjectType dTypeBase = dType.BaseType;

            while (dTypeBase != null)
            {
                nSpace += 2;
                sb1.Append(System.Environment.NewLine);
                sb1.Append(' ', nSpace);
                sb1.Append("BaseType: ");
                sb1.Append(DependencyObjectTypeTag(dTypeBase));
                dTypeBase = dTypeBase.BaseType;
            }

            sb1.Append(System.Environment.NewLine);
            PrintStatus(sb1.ToString());
        }

        private static string NullOrNot(object obj) { return obj == null ? "[Null]" : "[Not Null]"; }

        private static string strNullEmptyOrNot(string str)
        {
            if (str == null) { return "(null)"; }
            else if (str == string.Empty)
            {
                return "(emptyString)";
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Print out information on FrameworkElementFactory
        /// </summary>
        /// <param name="factory">The FrameworkElementFactory to display</param>
        internal static void PrintFrameworkElementFactory(System.Windows.FrameworkElementFactory factory)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(100);

            sb1.Append("FrameworkElementFactory ");
            sb1.Append(NullOrNot(factory));
            if (factory != null)
            {
                sb1.Append(System.Environment.NewLine);
                sb1.Append("Name: ");
                sb1.Append(factory.Name);
                sb1.Append(System.Environment.NewLine);
                sb1.Append("IsSealed: ");
                sb1.Append(factory.IsSealed.ToString());
                sb1.Append(System.Environment.NewLine);
                sb1.Append("FirstChild: ");
                sb1.Append(NullOrNot(factory.FirstChild));
                sb1.Append(System.Environment.NewLine);
                sb1.Append("NextSibling: ");
                sb1.Append(NullOrNot(factory.NextSibling));
                sb1.Append(System.Environment.NewLine);
                sb1.Append("Parent: ");
                sb1.Append(NullOrNot(factory.Parent));
            }

            PrintStatus(sb1.ToString());
        }

        private static bool s_runAllEnabled = false;
        private static DateTime s_dtStartTime = DateTime.MinValue;
        private static int s_totalTestFunction = 0;
        private static string s_runAllGroupName = null;
        /// <summary>
        /// Provde more support for refreshed test cases
        /// </summary>
        /// <param name="testGroupName">name of the test group</param>
        internal static void StartRunAllTests(string testGroupName)
        {
            s_runAllEnabled = true;
            s_dtStartTime = DateTime.Now;
            s_totalTestFunction = 0;
            s_runAllGroupName = testGroupName;

            PrintStatus("===== [" + s_runAllGroupName + "] test group =====", System.ConsoleColor.Red);
        }

        internal static void StopRunAllTests()
        {
            //PrintStatus("[" + RunAllGroupName + "] Test Group Completed.", System.ConsoleColor.Red).
            TimeSpan ts = System.DateTime.Now - s_dtStartTime;
            PrintStatus("Test Duration: " + ts.ToString() + " for " + s_runAllGroupName, System.ConsoleColor.Red);
            PrintStatus("");
            PrintStatus("");
            s_runAllEnabled = false;
        }

        /// <summary>
        /// Print out string array information
        /// </summary>
        /// <param name="array">the array to print out</param>
        internal static void PrintStringArray(string[] array)
        {
            int i = 0; foreach (string str in array)
            {
                i++;
                PrintStatus("[" + i.ToString() + "] " + str);
            }
        }

        /// <summary>
        /// Print out NameSpaceMapEntry instance information
        /// </summary>
        /// <param name="map"></param>
        internal static void PrintNamespaceMap(System.Windows.Markup.NamespaceMapEntry map)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder(100);

            sb1.Append("NamespaceUri:");
            sb1.Append(strNullEmptyOrNot(map.XmlNamespace));
            sb1.Append("; AssemblyName:");
            sb1.Append(strNullEmptyOrNot(map.AssemblyName));
            sb1.Append("; ClrNamespace");
            sb1.Append(strNullEmptyOrNot(map.ClrNamespace));
            PrintStatus(sb1.ToString());
        }

        /// <summary>
        /// From Xmal string to UIElement
        /// </summary>
        /// <param name="xmal">complete xaml file as string</param>
        /// <returns>DependencyObject as the result of Serialization.XamlReader.Load</returns>
        internal static DependencyObject FromXamlToElement(string xmal)
        {
            byte[] characters = System.Text.ASCIIEncoding.ASCII.GetBytes(xmal);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            ms.Write(characters, 0, characters.Length);
            ms.Position = 0;
            ParserContext pc = new ParserContext();
            pc.BaseUri = System.IO.Packaging.PackUriHelper.Create(new Uri("siteoforigin://"));
            object o = System.Windows.Markup.XamlReader.Load(ms, pc);
            System.Diagnostics.Debug.Assert(o is DependencyObject, "FromXamlToUIElement Check Assert Condition");
            return (DependencyObject)o;
        }

        /// <summary>
        /// From Element to Xaml string
        /// </summary>
        /// <param name="element">Element that is to be serialized</param>
        /// <returns>Xaml string as the result of Serialization.Parser.GetOuterXml</returns>
        internal static string FromElementToXaml(DependencyObject element)
        {
            return SerializationHelper.SerializeObjectTree(element);
        }
    }
}
