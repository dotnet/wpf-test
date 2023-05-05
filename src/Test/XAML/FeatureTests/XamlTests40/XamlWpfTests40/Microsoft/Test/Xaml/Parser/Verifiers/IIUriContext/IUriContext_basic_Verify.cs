// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IIUriContext
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_basic.xaml
    /// </summary> 
    public static class IUriContext_basic_Verify
    {
        /// <summary>
        /// This method verifies that the BaseUri of the xaml with the object is Correct
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns> bool value </returns>
        public static bool Verify(object obj)
        {
            bool result = true;
            if (obj == null)
            {
                GlobalLog.LogStatus("No Root Element in IUriContext_Basic.xaml is Found");
                return result = false;
            }

            Page page = obj as Page;
            Custom_IUriContext uriContext = (Custom_IUriContext) page.Content;
            string path = Environment.CurrentDirectory + @"/IUriContext_basic.xaml";
            ////Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            if (uriContext.BaseUri == null)
            {
                GlobalLog.LogStatus("The Base Uri of IUriContext_basic is Null");
                return result = false;
            }
            else
            {
                if (path != uriContext.BaseUri.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_ME is Invalid");
                    return result = false;
                }
            }

            GlobalLog.LogStatus("The Base Uri of IUriContext_basic is Valid");
            return result;
        }
    }
}
