// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Holds verification routines for xaml-based Enum syntax support tests.
 *
 
  
 * Revision:         $Revision: $
 
 * Filename:         $Source: $
********************************************************************/

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using System.Windows.Media;
using System.Windows.Documents;
using Avalon.Test.CoreUI.Parser;
using System.Collections;

namespace Avalon.Test.CoreUI.Serialization
{
    /// <summary>
    /// Holds verification routines for serialization of CultureInfoes
    /// </summary>
    public class CultureInfoSerializationVerifier
    {
        /// <summary>
        /// Verifies routine for CultureInfoSerialization.xaml
        /// </summary>
        public static void VerifyFE(UIElement root)
        {

            Button defaultb = (Button)LogicalTreeHelper.FindLogicalNode(root, "Default");

            VerifyElement.VerifyBool(null == defaultb, false);
            VerifyElement.VerifyString(defaultb.Language.IetfLanguageTag.ToLower(), "en-us");
            foreach (string cultureStr in s_allCultures)
            {
                CoreLogger.LogStatus("Testing culture : " + cultureStr);
                string id = cultureStr.Replace('-', '_');
                FrameworkElement fe = (FrameworkElement)LogicalTreeHelper.FindLogicalNode(root, id);
                VerifyElement.VerifyString(fe.Language.IetfLanguageTag.ToLowerInvariant(), cultureStr.ToLowerInvariant());
            }
        }
        /// <summary>
        /// Verifies routine for CultureInfoFCESerialization.xaml
        /// </summary>
        public static void VerifyFCE(UIElement root)
        {

            CoreLogger.LogStatus("Inside CultureInfoSerializationVerifier.VerifyFCE()...");

            Bold defaultb = (Bold)LogicalTreeHelper.FindLogicalNode(root, "Default");

            VerifyElement.VerifyBool(null == defaultb, false);
            VerifyElement.VerifyString(defaultb.Language.IetfLanguageTag.ToLower(), "en-us");
            foreach (string cultureStr in s_allCultures)
            {
                CoreLogger.LogStatus("Testing culture : " + cultureStr);
                string id = cultureStr.Replace('-', '_');
                FrameworkContentElement fce = (FrameworkContentElement)LogicalTreeHelper.FindLogicalNode(root, id);
                VerifyElement.VerifyString(fce.Language.IetfLanguageTag.ToLowerInvariant(), cultureStr.ToLowerInvariant());
            }
        }
        static string[] s_allCultures = {
                "ar-SA",
                "ar-IQ",
                "ar-EG",
                "ar-LY",
                "ar-DZ",
                "ar-MA",
                "ar-TN",
                "ar-OM",
                "ar-YE",
                "ar-SY",
                "ar-JO",
                "ar-LB",
                "ar-KW",
                "ar-AE",
                "ar-BH",
                "ar-QA",
                "bg-BG",
                "ca-ES",
                //"zh-TW",
                "zh-CN",
                "zh-HK",
                "zh-SG",
                "zh-MO",
                //"zh-CZ",
                "da-DK",
                "de-DE",
                "de-CH",
                "de-AT",
                "de-LU",
                "de-LI",
                "el-GR",
                "en-US",
                "en-GB",
                "en-AU",
                "en-CA",
                "en-NZ",
                "en-IE",
                "en-ZA",
                "en-JM",
                //"en-029",
                "en-BZ",
                "en-TT",
                "en-ZW",
                "en-PH",
                "es-MX",
                "es-ES",
                "es-GT",
                "es-CR",
                "es-PA",
                "es-DO",
                "es-VE",
                "es-CO",
                "es-PE",
                "es-AR",
                "es-EC",
                "es-CL",
                "es-UY",
                "es-PY",
                "es-BO",
                "es-SV",
                "es-HN",
                "es-NI",
                "es-PR",
                "fi-FI",
                "fr-FR",
                "fr-BE",
                "fr-CA",
                "fr-CH",
                "fr-LU",
                "fr-MC",
                "he-IL",
                "hu-HU",
                "is-IS",
                "it-IT",
                "it-CH",
                "ja-JP",
                "ko-KR",
                "nl-NL",
                "nl-BE",
                "nb-NO",
                "nn-NO",
                "pl-PL",
                "pt-BR",
                "pt-PT",
                "ro-RO",
                "ru-RU",
                "hr-HR",
                //"sr-Latn-SP",
                //"sr-Cyrl-SP",
                "sk-SK",
                "sq-AL",
                "sv-SE",
                "sv-FI",
                "th-TH",
                "tr-TR",
                "ur-PK",
                "id-ID",
                "uk-UA",
                "be-BY",
                "sl-SI",
                "et-EE",
                "lv-LV",
                "lt-LT",
                "fa-IR",
                "vi-VN",
                "hy-AM",
                //"az-Latn-AZ",
                //"az-Cyrl-AZ",
                "eu-ES",
                "mk-MK",
                "af-ZA",
                "ka-GE",
                "fo-FO",
                "hi-IN",
                "ms-MY",
                "ms-BN",
                "kk-KZ",
                "ky-KG",
                "sw-KE",
                //"uz-Latn-UZ",
                //"uz-Cyrl-UZ",
                "tt-RU",
                "pa-IN",
                "gu-IN",
                "ta-IN",
                "te-IN",
                "kn-IN",
                "mr-IN",
                "sa-IN",
                "mn-MN",
                "gl-ES",
                "kok-IN",
                "syr-SY",
                "div-MV"
                };

    }
}
