// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Wpf.AppModel.SecurityTests
{
    /// <summary>
    /// This test verifies that link option /CLRUNMANAGEDCODECHECK was specified during compile of System.Printing.
    /// The effect of the flag is to not add SuppressUnmanagedCodeSecurity (SUCS) to all PInvokes.
    /// We use reflection to verify a PInvoke method in the assembly is not marked SUCS.
    /// </summary>
    public partial class ClrUnmanagedCodeCheck : Window
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            try
            {
                Logger.LogStatus("Finding the DllImport method for 'TextOutW' or 'free' in System.Printing.dll");

                Assembly systemPrinting = typeof(System.Printing.PrintQueue).Assembly;
                Module module = systemPrinting.GetModule("System.Printing.dll");
                MethodInfo[] methodInfos = module.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                MethodInfo method = null;

                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if ((methodInfo.Name == "TextOutW") || (methodInfo.Name == "free"))
                    {
                        Logger.LogStatus("Found method " + methodInfo.Name);
                        method = methodInfo;
                        break;
                    }
                }
                if(null == method)
                {
                    Logger.LogFail("Couldn't find 'TextOutW' or 'free' - fail.");
                }

                if (HasSucs(method))
                {
                    Logger.LogFail("Unexpectedly found SUCS attribute, ClrUnmanagedCodeCheck must not be on for System.Printing.");
                }
                else
                {
                    Logger.LogPass("As expected, SUCS attribute was not found, so ClrUnmanagedCodeCheck is on.");
                }

            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
            }

            TestHelper.Current.TestCleanup();
        }

        /// <summary>
        /// If the supplied method has the SuppressUnmanagedCodeSecurityAttribute, return true, else false
        /// </summary>
        bool HasSucs(MethodInfo method)
        {
            foreach(CustomAttributeData attrib in CustomAttributeData.GetCustomAttributes(method))
            {
                if (attrib.Constructor.DeclaringType.Equals(typeof(SuppressUnmanagedCodeSecurityAttribute)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
