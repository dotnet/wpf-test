// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IIUriContext
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_ME.xaml
    /// </summary>    
    public static class IUriContext_ME_Verify
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
                GlobalLog.LogStatus("No Root Element in IUriContext_ME.xaml is Found");
                return result = false;
            }

            Page page = obj as Page;
            Button button = (Button) LogicalTreeHelper.FindLogicalNode(page, "bttn");
            Uri uri = button.Content as Uri;
            string path = Environment.CurrentDirectory + @"/IUriContext_ME.xaml";
            //// Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            if (uri == null)
            {
                GlobalLog.LogStatus("The Base Uri of IUriContext_ME is Null");
                return result = false;
            }
            else
            {
                if (path != uri.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_ME is Invalid");
                    return result = false;
                }
            }

            GlobalLog.LogStatus("The Base Uri of IUriContext_ME is Valid");
            return result;
        }
    }
}
