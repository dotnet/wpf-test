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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_Converter.xaml
    /// </summary>    
    public static class IUriContext_Converter_Verify
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
                GlobalLog.LogStatus("No Root Element in IUriContext_Converter.xaml is Found");
                return result = false;
            }

            Page page = obj as Page;
            Custom_Tag customTag = page.Content as Custom_Tag;
            string path = Environment.CurrentDirectory + @"/IUriContext_Converter.xaml";
            //// Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            Custom_IUriContext contextIUri = customTag.ContextProperty;
            if (contextIUri.UriProperty == null)
            {
                GlobalLog.LogStatus("The Base Uri of IUriContext_Converter is Null");
                return result = false;
            }
            else
            {
                if (path != contextIUri.UriProperty.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_Converter is Invalid");
                    return result = false;
                }
            }

            GlobalLog.LogStatus("The Base Uri of IUriContext_Converter is Valid");
            return result;
        }
    }
}
