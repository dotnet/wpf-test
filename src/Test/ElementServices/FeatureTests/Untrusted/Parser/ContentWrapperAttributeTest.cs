// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Avalon.Test.CoreUI.Common;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.Test.Serialization;
using System.Windows.Media;
using Avalon.Test.CoreUI.Trusted.Utilities;
using Microsoft.Test;
using Microsoft.Test.Discovery;


namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Verifies basic behavior of ContentWrapperAttribute.
    /// </summary>
    [TestDefaults]
    public class ContentWrapperAttributeTest
    {
        /// <summary>
        /// Verifies basic behavior of ContentWrapperAttribute.
        /// </summary>
        //[Test(2, @"Parser\ContentWrapperAttribute", TestCaseSecurityLevel.FullTrust, @"Verify basic parser handling of ContentWrapperAttribute.", Area = "XAML")]
        public void VerifyBasicBehavior()
        {
            _TestContentWrapperAttribute("PresentationBuildTasks");
            _TestContentWrapperAttribute("WindowsBase");
        }

        private void _TestContentWrapperAttribute(string assemblyName)
        {
            CoreLogger.LogStatus("Verifying ContentWrapperAttribute from " + assemblyName + "...");

            // Create the list.
            InternalObject attrib1 = InternalObject.CreateInstance(assemblyName, "System.Windows.Markup.ContentWrapperAttribute", new object[] { typeof(Run) });
            InternalObject attrib2 = InternalObject.CreateInstance(assemblyName, "System.Windows.Markup.ContentWrapperAttribute", new object[] { typeof(Run) });
            InternalObject attrib3 = InternalObject.CreateInstance(assemblyName, "System.Windows.Markup.ContentWrapperAttribute", new object[] { typeof(Panel) });

            // Verify Equals()
            CoreLogger.LogStatus("Verifying Equals()...");
            if (!_Equals(attrib1, attrib1) || !_Equals(attrib1, attrib2) || _Equals(attrib1, attrib3)) throw new Microsoft.Test.TestValidationException("FAILED");

            // Verify GetHashCode()
            CoreLogger.LogStatus("Verifying GetHashCode()...");
            if (_GetHashCode(attrib1) != _GetHashCode(attrib1) || _GetHashCode(attrib1) == _GetHashCode(attrib3)) throw new Microsoft.Test.TestValidationException("FAILED");

            // Verify ContentWrapper
            CoreLogger.LogStatus("Verifying ContentWrapper...");
            if (_ContentWrapper(attrib1) != _ContentWrapper(attrib2) || _ContentWrapper(attrib1) != _ContentWrapper(attrib2) || _ContentWrapper(attrib1) == _ContentWrapper(attrib3)) throw new Microsoft.Test.TestValidationException("FAILED");

            // Verify TypeId
            CoreLogger.LogStatus("Verifying TypeId...");
            if (_TypeId(attrib1) != _TypeId(attrib1) || _TypeId(attrib1) == _TypeId(attrib2)) throw new Microsoft.Test.TestValidationException("FAILED");
        }

        private bool _Equals(InternalObject obj1, InternalObject obj2)
        {
            return (bool)obj1.InvokeMethod("Equals", obj2.Target);
        }

        private int _GetHashCode(InternalObject obj)
        {
            return (int)obj.InvokeMethod("GetHashCode");
        }

        private Type _ContentWrapper(InternalObject obj)
        {
            return (Type)obj.GetProperty("ContentWrapper");
        }

        private object _TypeId(InternalObject obj)
        {
            return obj.GetProperty("TypeId");
        }
    }
}
