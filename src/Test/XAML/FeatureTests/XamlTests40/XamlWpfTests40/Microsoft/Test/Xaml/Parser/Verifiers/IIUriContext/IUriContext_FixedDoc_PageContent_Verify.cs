// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.Verifiers.IIUriContext
{
    /// <summary>
    /// Verification class for Xaml File
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_FixedDocument.xaml
    /// </summary>    
    public static class IUriContext_FixedDoc_PageContent_Verify
    {
        /// <summary>
        /// This method verifies that the BaseUri of the FixedDocument is set correctly
        /// This method verifies that the BaseUri of the PageContent is set correctly
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns> bool value </returns>
        public static bool Verify(object obj)
        {
            bool result = true;
            if (obj == null)
            {
                GlobalLog.LogStatus("No Root Element in IUriContext_FixedDoc_PageContent is Found");
                return result = false;
            }

            string path = Environment.CurrentDirectory + @"/IUriContext_FixedDoc_PageContent.xaml";
            //// Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            FixedDocument fixedDoc = obj as FixedDocument;
            IUriContext iUriCntxt = fixedDoc;
            //// Compares BaseUri of the FixedDocument and the given path
            if (iUriCntxt == null)
            {
                GlobalLog.LogStatus("The IUriContext is Null");
                return result = false;
            }
            else if (iUriCntxt.BaseUri == null)
            {
                GlobalLog.LogStatus("The Base Uri is Null");
                result = false;
            }
            else
            {
                if (path != iUriCntxt.BaseUri.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_FixedDoc_PageContent is Invalid");
                    result = false;
                }
            }

            //// Compares BaseUri of the PageContent and the given path
            PageContentCollection pageContColl = fixedDoc.Pages;
            foreach (PageContent pageCont in pageContColl)
            {
                IUriContext iUriC = pageCont;
                if (iUriC == null)
                {
                    GlobalLog.LogStatus("The IUriContext is Null");
                    return result = false;
                }
                else if (iUriC.BaseUri == null)
                {
                    GlobalLog.LogStatus("The Base Uri is Null");
                    result = false;
                }
                else
                {
                    if (path != iUriC.BaseUri.ToString())
                    {
                        GlobalLog.LogStatus("The Base Uri of PageContent in IUriContext_FixedDocument is Invalid");
                        result = false;
                    }
                }
            }

            return result;
        }
    }
}
