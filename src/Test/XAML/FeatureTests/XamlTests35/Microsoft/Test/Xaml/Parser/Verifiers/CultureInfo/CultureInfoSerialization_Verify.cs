// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Xaml.Utilities;
using Microsoft.Test.Logging;
using System.Windows;

namespace Microsoft.Test.Xaml.Parser.Verifiers.CultureInfo
{
    /// <summary/>
    public static class CultureInfoSerialization_Verify
    {
        private static readonly string[] s_allCultures = {
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

        /// <summary>
        /// Verifies routine for CultureInfoSerialization.xaml
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool   result   = true;
            Button defaultb = (Button) LogicalTreeHelper.FindLogicalNode(rootElement, "Default");

            VerifyElement.VerifyBool(null == defaultb, false, ref result);
            VerifyElement.VerifyString(defaultb.Language.IetfLanguageTag.ToLower(), "en-us", ref result);
            foreach (string cultureStr in s_allCultures)
            {
                GlobalLog.LogStatus("Testing culture : " + cultureStr);
                string           id = cultureStr.Replace('-', '_');
                FrameworkElement fe = (FrameworkElement) LogicalTreeHelper.FindLogicalNode(rootElement, id);
                VerifyElement.VerifyString(fe.Language.IetfLanguageTag.ToLowerInvariant(), cultureStr.ToLowerInvariant(), ref result);
            }
            return result;
        }
    }
}
