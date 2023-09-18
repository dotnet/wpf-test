// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System;

namespace Test.Uis.Utils
{
    /// <summary>
    /// Static class which exposes string helper functions
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Function which converts a Securestring to a string
        /// </summary>
        /// <param name="secureString">securestring to be converted</param>
        /// <returns></returns>
        public static string GetStringFromSecureString(SecureString secureString)
        {
            IntPtr stringPointer = IntPtr.Zero;
            string resultStr = string.Empty;
            try
            {
                stringPointer = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
                resultStr = stringPointer.ToString();
                unsafe
                {
                    resultStr = new string((char*)stringPointer);
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(stringPointer);
            }

            return resultStr;
        }

        /// <summary>
        /// Function which converts a string to SecureString
        /// </summary>
        /// <param name="sampleString">string to be converted</param>
        /// <returns></returns>
        public static SecureString GetSecureStringFromString(string sampleString)
        {
            SecureString secureString = new SecureString();
            for (int i = 0; i < sampleString.Length; i++)
            {
                secureString.AppendChar(sampleString[i]);
            }
            return secureString;
        }
    }
}
