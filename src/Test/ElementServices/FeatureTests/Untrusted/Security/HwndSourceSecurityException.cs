// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Verify SecurityExceptions are thrown by HwndSource when protected
 * members are used in partial trust.
 * 
 * Contributors: 
 *
 
  
 * Revision:         $Revision:  $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/host/Security/SecurityExceptionTest.cs $
********************************************************************/

using System;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Xml;

using System.Collections;

using Microsoft.Test.Security;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI.Trusted.Utilities; // internalobject

using System.Windows.Interop;

namespace Avalon.Test.CoreUI.Hosting.Security
{
    /// <summary>
    /// Parse an xml file describing assembly class members to test for SecurityExceptions in partial trust.
    /// </summary>
    /// <description>
    /// Test case helper for verifying SecurityExceptions
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class HwndSourceSecurityExceptionTest
    {
        /// <summary>
        /// Test case entry point.
        /// The TestCaseAttribute param is used to select which assembly in the support file will be tested. The case
        /// parses this element in the file to determine which classes and members to test.
        /// </summary>
        [TestCase("0", @"Security", TestCaseSecurityLevel.PartialTrust, "HwndSource security exception tests")]
        public void Run()
        {
            //
            // Setup test case.
            //

            // Type to be tested
            string targetTypeName = "System.Windows.Interop.HwndSource";
            string targetAssemblyName = "PresentationCore";

            // List of members to test.
            string[] targetMemberNames = { 
                "AddHook",
                "RemoveHook",
            };

            // Create instance of target object.
            HwndSource targetInstance = new HwndSource(new HwndSourceParameters("Test HwndSource Security"));


            if (SecurityExceptionTest.TestSecurityExceptions(targetTypeName, targetAssemblyName, targetMemberNames, targetInstance) == false)
            {
                CoreLogger.LogTestResult(false, "HwndSource security critical methods failed test.");
            }
            else
            {
                CoreLogger.LogTestResult(true, "HwndSource sucurity critical methods threw correct exceptions.");
            }
            
        }

       
    }
}

