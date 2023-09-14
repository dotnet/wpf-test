// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Covering ShouldSerialize Threat.
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Microsoft.Test.Serialization;
using System.Security;
namespace Avalon.Test.CoreUI.Serialization.Security
{
    /// <summary>
    /// A class to verify that serialization of a control with private ShouldSerialize
    /// throws a security Exception 
    /// </summary>
    public class ShouldSerializeThreatTests
    {     
        /// <summary>
        /// Entry Point
        /// </summary>
        public void Run()
        {
            bool isSecurityException = false;
            CoreLogger.LogStatus("Testing private ShouldSerialize function for clr property ...");
            try
            {
                FrameworkElementWithShouldSerializeCLR clrObj = new FrameworkElementWithShouldSerializeCLR();
                System.Windows.Markup.XamlWriter.Save(clrObj);
            }
            catch (SecurityException)
            {
                isSecurityException = true;
            }
            catch (Exception e)
            {
                CoreLogger.LogStatus("Wrong Exception Type: " + e.GetType().Name);
            }
            if (!isSecurityException)
            {
                throw new Microsoft.Test.TestValidationException("Should have thrown a security exception while serializing a object with private ShouldSerialize<PropertyName> clr property.");
            }

            CoreLogger.LogStatus("Testing private ShouldSerialize function for dependency property...");

            isSecurityException = false;
            try
            {
                FrameworkElementWithShouldSerializeDP dpObj = new FrameworkElementWithShouldSerializeDP();
                System.Windows.Markup.XamlWriter.Save(dpObj);
            }
            catch (SecurityException)
            {
                isSecurityException = true;
            }
            catch (Exception e)
            {
                CoreLogger.LogStatus("Wrong Exception Type: " + e.GetType().Name);
            }
            if (!isSecurityException)
            {
                throw new Microsoft.Test.TestValidationException("Should have thrown a security exception while serializing a object with private ShouldSerialize<PropertyName> dependency property.");
            }
        }
    }
}
