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
    /// \src\wpf\Test\XAML\FeatureTests\Data\Parser\IUriContext\IUriContext_FixedDocumentSequence.xaml
    /// </summary>   
    public static class IUriContext_FixedDocumentSequence_Verify
    {
        /// <summary>
        /// This method verifies that the BaseUri of the DocumentReference is set correctly
        /// </summary>
        /// <param name="obj"> The object. </param>
        /// <returns> bool value </returns>
        public static bool Verify(object obj)
        {
            bool result = true;
            if (obj == null)
            {
                GlobalLog.LogStatus("No Root Element in IUriContext_FixedDocumentSequence is Found");
                return result = false;
            }

            string path = Environment.CurrentDirectory + @"/IUriContext_FixedDocumentSequence.xaml";
            //// Adding file and replacing the \\ with / in path to compare the BaseUri
            path = @"file:///" + path.Replace("\\", "/");
            FixedDocumentSequence fixedDocSeq = obj as FixedDocumentSequence;
            IUriContext iUriCtxt = fixedDocSeq;
            if (iUriCtxt == null)
            {
                GlobalLog.LogStatus("The IUriContext is Null");
                return result = false;
            }
            else if (iUriCtxt.BaseUri == null)
            {
                GlobalLog.LogStatus("The Base Uri is Null");
                return result = false;
            }
            else
            {
                if (path != iUriCtxt.BaseUri.ToString())
                {
                    GlobalLog.LogStatus("The Base Uri of IUriContext_FixedDocumentSequence is Invalid");
                    return result = false;
                }
            }

            GlobalLog.LogStatus("The Base Uri of IUriContext_FixedDocumentSequence is Valid");
            return result;
        }
    }
}
