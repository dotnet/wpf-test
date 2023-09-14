// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using System.IO;
using System.Security;

namespace Avalon.Test.CoreUI
{
    ///<summary>
    /// Test cases to verify that the harness correctly sets security permissions.
    ///</summary>
    [TestDefaults]
    public class DriverSecurityTests
    {
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
		public DriverSecurityTests() { }

		/// <summary>
        /// Validates that the restricted (partial trust) permissions were set 
		/// correctly by harness.
        /// </summary>
		[Test(0, @"Driver\Security", TestCaseSecurityLevel.PartialTrust, "Verify partial trust permissions are set.")]
        public void VerifyPartialTrust()
        {
            bool wasThrown = false;
            try
            {
                Directory.GetCurrentDirectory();
            }
            catch(SecurityException)
            {
				wasThrown = true;
			}

			if (!wasThrown)
			{
				throw new Microsoft.Test.TestValidationException("No security exception was thrown when calling Directory.GetCurrentDirectory().  The caller apparently has full trust.  It should have partial trust.");
			}
        }

		/// <summary>
		/// Validates that the unrestricted (full trust) permissions were set 
		/// correctly by harness.
		/// </summary>
		[Test(0, @"Driver\Security", TestCaseSecurityLevel.FullTrust, "Verify full trust permissions are set.")]
		public void VerifyFullTrust()
		{
			bool wasThrown = false;
			try
			{
				Directory.GetCurrentDirectory();
			}
			catch (SecurityException)
			{
				wasThrown = true;
			}

			if (wasThrown)
			{
				throw new Microsoft.Test.TestValidationException("A security exception was thrown when calling Directory.GetCurrentDirectory().  The caller apparently has partial trust.  It should have full trust.");
			}
		}
	}
}

