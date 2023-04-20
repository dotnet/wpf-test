// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IIUriContext
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_DocumentReference.xaml
    /// </summary>    
    public static class IUriContext_DocumentReference_Verify
    {
        /// <summary>
        /// This method verifies that the BaseUri of the DocumentReference is set correctly
        /// </summary>
        /// <param name="obj"> The object.  </param>
        /// <returns> bool value </returns>
        public static bool Verify(object obj)
        {
            bool result = true;
            if (obj == null)
            {
                GlobalLog.LogStatus("No Root Element in IUriContext_DocumentReference is Found");
                return result = false;
            }

            Page page = obj as Page;
            DocumentReference docRef = (DocumentReference) page.Content;
            IUriContext iUriCntxt = docRef;
            string path = Environment.CurrentDirectory + @"/IUriContext_DocumentReference.xaml";
            //// Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            if (iUriCntxt == null)
            {
                GlobalLog.LogStatus("The IUriContext is Null");
                return result = false;
            }
            else if (iUriCntxt.BaseUri == null)
            {
                GlobalLog.LogStatus("The Base Uri of IUriContext_DocumentReference is Null");
                return result = false;
            }
            else
            {
                if (path != iUriCntxt.BaseUri.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_DocumentReference is Invalid");
                    return result = false;
                }
            }

            GlobalLog.LogStatus("The Base Uri of IUriContext_DocumentReference is Valid");
            return result;
        }
    }
}
