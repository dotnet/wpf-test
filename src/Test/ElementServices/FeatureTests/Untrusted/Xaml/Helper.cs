// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Avalon.Test.Xaml.Markup
{
    /// <summary>
    /// 
    /// </summary>
    static class Helper
    {
        /// <summary>
        /// Removes the *\ for the support file
        /// </summary>
        public static string PreprocessSupportFile(string supportFile)
        {
            if (!String.IsNullOrEmpty(supportFile) && supportFile.StartsWith(@"*\"))
            {
                int index = supportFile.LastIndexOf(@"\");
                index++;
                supportFile = supportFile.Substring(index);
            }

            return supportFile;
        }
    }
}
