// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Exception MessageHelper
    /// </summary>
    public static class ExceptionMessageHelper
    {
        /// <summary>
        /// Matches the specified exception message.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <param name="stringResourceIDName">Name of the string resource ID.</param>
        /// <returns>bool value</returns>
        public static bool Match(string exceptionMessage, string stringResourceIDName)
        {
            if (exceptionMessage == null)
            {
                GlobalLog.LogDebug("exceptionMessage is null");
                return false;
            }

            if (stringResourceIDName == null)
            {
                GlobalLog.LogDebug("stringResourceIDName is null");
                return false;
            }

            string expectedExceptionMessage = Extract(stringResourceIDName);
            if (expectedExceptionMessage == null)
            {
                GlobalLog.LogDebug(String.Format("A matching exception message for SRID: {0} was not found.", stringResourceIDName));
                return false;
            }

            Regex exceptionRegex = new Regex(Escape(expectedExceptionMessage));

            GlobalLog.LogDebug("Exception caught, attempting to match with: [" + expectedExceptionMessage + "]");
            if (exceptionRegex.IsMatch(exceptionMessage))
            {
                GlobalLog.LogDebug("Exception matched");
                return true;
            }
            else
            {
                GlobalLog.LogDebug("Exception did not match.  Exception message:");
                GlobalLog.LogDebug("[" + exceptionMessage + "]");
                return false;
            }
        }

        /// <summary>
        /// Escapes the specified exception message.
        /// </summary>
        /// <param name="exceptionMessage">The exception message.</param>
        /// <returns>string value</returns>
        public static string Escape(string exceptionMessage)
        {
            if (exceptionMessage == null)
            {
                return null;
            }

            string excapedExceptionMessage = Regex.Escape(exceptionMessage);
            excapedExceptionMessage = String.Format(excapedExceptionMessage, new object[] { ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*" });
            
            // When escaping the exceptionMessage, the placeholder {n} also gets escaped as \{n}
            // and the \ remains after replacing {n} by .*
            excapedExceptionMessage = excapedExceptionMessage.Replace(@"\.*", ".*");
            return excapedExceptionMessage;
        }

        /// <summary>
        /// Extracts the specified string resource ID name.
        /// </summary>
        /// <param name="stringResourceIDName">Name of the string resource ID.</param>
        /// <returns>string value</returns>
        public static string Extract(string stringResourceIDName)
        {
            if (stringResourceIDName == null)
            {
                GlobalLog.LogDebug("stringResourceIDName is null");
                return null;
            }

            // Get the assembly of the parser
            Assembly assem = Assembly.GetAssembly(typeof(System.Xaml.XamlReader));
            if (assem == null)
            {
                GlobalLog.LogDebug("Assembly containing System.Xaml.XamlReader is not found");
                return null;
            }

            Type sridType = assem.GetType("System.Xaml.SRID");
            if (sridType == null)
            {
                GlobalLog.LogDebug("System.Xaml.SRID type not found in assembly " + assem.FullName);
                return null;
            }

            // Get the field of type SRID stringResourceIDName on the type SRID
            FieldInfo sridFI = sridType.GetField(stringResourceIDName);
            if (sridFI == null)
            {
                GlobalLog.LogDebug("Field named " + stringResourceIDName + " is not found in SRID type");
                return null;
            }

            // Get the actual SRID object
            object sridObject = sridFI.GetValue(sridType);
            if (sridObject == null)
            {
                GlobalLog.LogDebug("Cannot get value of " + sridFI.Name + " field in SRID type");
                return null;
            }

            Type sysSrType = assem.GetType("System.Xaml.SR");
            if (sysSrType == null)
            {
                GlobalLog.LogDebug("System.Xaml.SR type not found in assembly " + assem.FullName);
                return null;
            }

            // Get the method: internal static string SR.Get(SRID id, object[] args)
            MethodInfo sridGetMethod = sysSrType.GetMethod("Get", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            if (sridGetMethod == null)
            {
                GlobalLog.LogDebug("Get method not found in SR type");
                return null;
            }

            // Invoke the Get method, this will return a string formatted in the CurrentUICulture
            string exceptionMessage = null;
            try
            {
                exceptionMessage = sridGetMethod.Invoke(sridGetMethod, new object[] { sridObject }) as string;
            }
            catch (Exception) // Catch all, since this function will be called from a catch block where we dont want to throw and overwrite the stack
            {
                return null;
            }

            return exceptionMessage;
        }

        /// <summary>
        /// Verify if two exception strings match
        /// - could have missing arguments
        /// </summary>
        /// <param name="expected">expected message</param>
        /// <param name="actual">actual message</param>
        /// <returns>true if matched</returns>
        public static bool IsExceptionStringMatch(string expected, string actual)
        {
            if (expected.Equals(actual))
            {
                return true;
            }

            string exceptionString = String.Format(expected, new object[] { ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*" });
            Regex exceptionRegex = new Regex(exceptionString);

            return exceptionRegex.IsMatch(actual);
        }
    }
}
