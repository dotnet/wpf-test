// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace Microsoft.Test.Xaml.Utilities
{
    /// <summary>
    /// Utility class for system language detection
    /// </summary>
    public static class LangUtils
    {
        /// <summary>
        /// Check if OS Language is of the requested Culture
        /// </summary>
        /// <param name="requestedCulture">CultureInfo requestedCulture</param>
        /// <returns>true if OS Language is of the requested Culture</returns>
        public static bool IsSystemLanguage(CultureInfo requestedCulture)
        {
            CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
            if (currentUICulture.Name == requestedCulture.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if OS Language is of the requested Culture
        /// </summary>
        /// <param name="cultureInfos">CultureInfo requestedCultures</param>
        /// <returns>true if OS Language is of the requested Culture</returns>
        public static bool IsSystemLanguage(CultureInfo[] cultureInfos)
        {
            foreach (CultureInfo requestedCulture in cultureInfos)
            {
                if (IsSystemLanguage(requestedCulture))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
