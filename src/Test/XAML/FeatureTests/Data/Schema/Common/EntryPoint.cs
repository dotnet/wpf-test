// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Schema
{
    public class EntryPoint
    {
        /// <summary>
        /// Exit codes
        /// 0 - Pass
        /// 1 - Fail
        /// 2 - Unexpected exception or unable to find any type marked with SchemaTest attribute
        /// </summary>
        public static void Main()
        {
            try
            {
                Type testType = FindTestType();
                if (testType == null)
                {
                    GlobalLog.LogDebug("Found NO type with SchemaTest attribute");
                    Environment.Exit(2);
                }

                object testObject = Activator.CreateInstance(testType);
                testType.InvokeMember("Run", BindingFlags.InvokeMethod, null, testObject, null);
            }
            catch (SchemaTestFailedException stfe)
            {
                GlobalLog.LogDebug("Test case failed with SchemaTestFailed exception: " + stfe.ToString());
                Environment.Exit(1);
            }
            catch (TargetInvocationException tie)
            {
                if (tie.InnerException.GetType() == typeof(SchemaTestFailedException))
                {
                    GlobalLog.LogDebug("Test case failed with SchemaTestFailed exception: " + tie.InnerException.ToString());
                    Environment.Exit(1);
                }
                else
                {
                    GlobalLog.LogDebug("Test case failed with unknown exception: " + tie.ToString());
                    Environment.Exit(2);
                }
            }
            catch (Exception e)
            {
                GlobalLog.LogDebug("Test case failed with unknown exception: " + e.ToString());
                Environment.Exit(2);
            }
            GlobalLog.LogDebug("Test case passed");
        }

        private static Type FindTestType()
        {
            Type[] types = typeof(EntryPoint).Assembly.GetTypes();
            foreach (Type type in types)
            {
                object[] attributes = type.GetCustomAttributes(false);
                for(int i=0; i<attributes.Length; i++)
                {
                    Attribute attrib = (Attribute)attributes[i];
                    if (attrib.GetType() == typeof(SchemaTestAttribute))
                    {
                        return type;
                    }
                }
            }
            return null;
        }
    }
}
