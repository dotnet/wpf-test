// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Text;
using GenStrings;
using ReflectTools;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Provides utils for random chars, numbers, Enums, images, & misc
    //  objects, array comparisons, etc.
    //
    //  This class is basically the dumping grounds for utility type
    //  methods that one or more classes may find useful.
    // </desc>
    // </doc>
    public class RandomUtil
    {
        //
        // Various permissions asserted in RandomUtil.
        //
        private static readonly SecurityPermission s_unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);

        // <doc>
        // <desc>
        //  The maximum possible Red, Green or Blue value in an RGB
        // </desc>
        // <seealso member="GetColor"/>
        // </doc>
        internal const int MaxRGBInt = 255;

        // <doc>
        // <desc>
        //  The maximum number of tries to get a Font before giving it up as a bad job
        // </desc>
        // <seealso member="GetFont"/>
        // </doc>
        private const int maxAttemptsToGetFont = 100;

        // <doc>
        // <desc>
        //  The Random number generator
        //  Made it static so all instances share the same
        //                    seed value.
        // </desc>
        // </doc>
        private static Random s_rand = null;

        // <doc>
        // <desc>
        //  The object that builds international aware strings for us
        // </desc>
        // <seealso class="IntlString"/>
        // </doc>
        private IntlStrings _intlStr = null;

        // <doc>
        // <desc>
        //  The list of image names that this library knows about.
        //  Note: This list must be kept in sync with the values in
        //        The ImageStyle enum class.
        // </desc>
        // <seealso class="ImageStyle"/>
        // <seealso member="GetImage"/>
        // </doc>
        private String[] _imageNames = new string[]
        {
            "BEANY.BMP",   /* 0 */
            "MS.EMF",      /* 1 */
            "1bit87a.gif", /* 2 */
            "rock.JPG",    /* 3 */
            "FIRE.ICO",    /* 4 */
            "FLOWER.WMF",   /* 5 */
            "Animated.gif" /* 6 */
        };

        // <doc>
        // <desc>
        //  The list of icon names that this library knows about.
        // </desc>
        // <seealso member="GetIcon"/>
        // </doc>
        private String[] _iconNames = new string[]
        {
            "applet.ico",
            "button.ico",
            "class.ico",
            "combo.ico",
            "fire.ico",
            "horzsplt.ico",
            "list.ico"
        };

        //
        // For use with GetValidDateTime() method.  Represents the minimum valid date for
        // the current culture.
        //
        private DateTime _minValidDate;

        // <doc>
        // <desc>
        //  The list of culture codes that the CultureInfo class supports.
        // </desc>
        // <seealso member="GetIFormatProvider"/>
        // </doc>
        // changed the name of culture: "az-AZ-Latn" to "az-Latn-AZ", "az-AZ-Cyrl" to "az-Cyrl-AZ", "uz-UZ-Cyrl" to "uz-Cyrl-UZ", "uz-UZ-Latn" to "uz-Latn-UZ"
        // changed the above names back again
        private string[] _cultureFormats = new string[]
		{
			"",				// invariant culture
			"af",			// Afrikaans
			"af-ZA",		// Afrikaans - South Africa
			"sq",			// Albanian
			"sq-AL",		// Albanian - Albania
			"ar",			// Arabic
			"ar-DZ",		// Arabic - Algeria
			"ar-BH",		// Arabic - Bahrain
			"ar-EG",		// Arabic - Egypt
			"ar-IQ",		// Arabic - Iraq
			"ar-JO",		// Arabic - Jordan
			"ar-KW",		// Arabic - Kuwait
			"ar-LB",		// Arabic - Lebanon
			"ar-LY",		// Arabic - Libya
			"ar-MA",		// Arabic - Morocco
			"ar-OM",		// Arabic - Oman
			"ar-QA",		// Arabic - Qatar
			"ar-SA",		// Arabic - Saudi Arabia
			"ar-SY",		// Arabic - Syria
			"ar-TN",		// Arabic - Tunisia
			"ar-AE",		// Arabic - United Arab Emirates
			"ar-YE",		// Arabic - Yemen
			"hy",			// Armenian
			"hy-AM",		// Armenian - Armenia
			"az",			// Azeri
           	"az-AZ-Cyrl",	// Azeri (Cyrillic) - Azerbaijan
          	"az-AZ-Latn",	// Azeri (Latin) - Azerbaijan
			"eu",			// Basque
			"eu-ES",		// Basque - Basque
			"be",			// Belarusian
			"be-BY",		// Belarusian - Belarus
			"bg",			// Bulgarian
			"bg-BG",		// Bulgarian - Bulgaria
			"ca",			// Catalan
			"ca-ES",		// Catalan - Catalan
			"zh-HK",		// Chinese - Hong Kong SAR
			"zh-MO",		// Chinese - Macau SAR
			"zh-CN",		// Chinese - China
			"zh-CHS",		// Chinese (Simplified)
			"zh-SG",		// Chinese - Singapore
			"zh-TW",		// Chinese - Taiwan
			"zh-CHT",		// Chinese (Traditional)
			"hr",			// Croatian
			"hr-HR",		// Croatian - Croatia
			"cs",			// Czech
			"cs-CZ",		// Czech - Czech Republic
			"da",			// Danish
			"da-DK",		// Danish - Denmark
			"div",			// Dhivehi
			"div-MV",		// Dhivehi - Maldives
			"nl",			// Dutch
			"nl-BE",		// Dutch - Belgium
			"nl-NL",		// Dutch - The Netherlands
			"en",			// English
			"en-AU",		// English - Australia
			"en-BZ",		// English - Belize
			"en-CA",		// English - Canada
			"en-CB",		// English - Caribbean
			"en-IE",		// English - Ireland
			"en-JM",		// English - Jamaica
			"en-NZ",		// English - New Zealand
			"en-PH",		// English - Philippines
			"en-ZA",		// English - South Africa
			"en-TT",		// English - Trinidad and Tobago
			"en-GB",		// English - United Kingdom
			"en-US",		// English - United States
			"en-ZW",		// English - Zimbabwe
			"et",			// Estonian
			"et-EE",		// Estonian - Estonia
			"fo",			// Faroese
			"fo-FO",		// Faroese - Faroe Islands
			"fa",			// Farsi
			"fa-IR",		// Farsi - Iran
			"fi",			// Finnish
			"fi-FI",		// Finnish - Finland
			"fr",			// French
			"fr-BE",		// French - Belgium
			"fr-CA",		// French - Canada
			"fr-FR",		// French - France
			"fr-LU",		// French - Luxembourg
			"fr-MC",		// French - Monaco
			"fr-CH",		// French - Switzerland
			"gl",			// Galician
			"gl-ES",		// Galician - Galician
			"ka",			// Georgian
			"ka-GE",		// Georgian - Georgia
			"de",			// German
			"de-AT",		// German - Austria
			"de-DE",		// German - Germany
			"de-LI",		// German - Liechtenstein
			"de-LU",		// German - Luxembourg
			"de-CH",		// German - Switzerland
			"el",			// Greek
			"el-GR",		// Greek - Greece
			"gu",			// Gujarati
			"gu-IN",		// Gujarati - India
			"he",			// Hebrew
			"he-IL",		// Hebrew - Israel
			"hi",			// Hindi
			"hi-IN",		// Hindi - India
			"hu",			// Hungarian
			"hu-HU",		// Hungarian - Hungary
			"is",			// Icelandic
			"is-IS",		// Icelandic - Iceland
			"id",			// Indonesian
			"id-ID",		// Indonesian - Indonesia
			"it",			// Italian
			"it-IT",		// Italian - Italy
			"it-CH",		// Italian - Switzerland
			"ja",			// Japanese
			"ja-JP",		// Japanese - Japan
			"kn",			// Kannada
			"kn-IN",		// Kannada - India
			"kk",			// Kazakh
			"kk-KZ",		// Kazakh - Kazakhstan
			"kok",			// Konkani
			"kok-IN",		// Konkani - India
			"ko",			// Korean
			"ko-KR",		// Korean - Korea
			"ky",			// Kyrgyz
			//"ky-KZ",		// Kyrgyz - Kazakhstan -- This is not supported
			"lv",			// Latvian
			"lv-LV",		// Latvian - Latvia
			"lt",			// Lithuanian
			"lt-LT",		// Lithuanian - Lithuania
			"mk",			// Macedonian
			"mk-MK",		// Macedonian - FYROM
			"ms",			// Malay
			"ms-BN",		// Malay - Brunei
			"ms-MY",		// Malay - Malaysia
			"mr",			// Marathi
			"mr-IN",		// Marathi - India
			"mn",			// Mongolian
			"mn-MN",		// Mongolian - Mongolia
			"no",			// Norwegian
			"nb-NO",		// Norwegian (Bokml) - Norway
			"nn-NO",		// Norwegian (Nynorsk) - Norway
			"pl",			// Polish
			"pl-PL",		// Polish - Poland
			"pt",			// Portuguese
			"pt-BR",		// Portuguese - Brazil
			"pt-PT",		// Portuguese - Portugal
			"pa",			// Punjabi
			"pa-IN",		// Punjabi - India
			"ro",			// Romanian
			"ro-RO",		// Romanian - Romania
			"ru",			// Russian
			"ru-RU",		// Russian - Russia
			"sa",			// Sanskrit
			"sa-IN",		// Sanskrit - India
         	"sr-SP-Cyrl",	// Serbian (Cyrillic) - Serbia
         	"sr-SP-Latn",	// Serbian (Latin) - Serbia
			"sk",			// Slovak
			"sk-SK",		// Slovak - Slovakia
			"sl",			// Slovenian
			"sl-SI",		// Slovenian - Slovenia
			"es",			// Spanish
			"es-AR",		// Spanish - Argentina
			"es-BO",		// Spanish - Bolivia
			"es-CL",		// Spanish - Chile
			"es-CO",		// Spanish - Colombia
			"es-CR",		// Spanish - Costa Rica
			"es-DO",		// Spanish - Dominican Republic
			"es-EC",		// Spanish - Ecuador
			"es-SV",		// Spanish - El Salvador
			"es-GT",		// Spanish - Guatemala
			"es-HN",		// Spanish - Honduras
			"es-MX",		// Spanish - Mexico
			"es-NI",		// Spanish - Nicaragua
			"es-PA",		// Spanish - Panama
			"es-PY",		// Spanish - Paraguay
			"es-PE",		// Spanish - Peru
			"es-PR",		// Spanish - Puerto Rico
			"es-ES",		// Spanish - Spain
			"es-UY",		// Spanish - Uruguay
			"es-VE",		// Spanish - Venezuela
			"sw",			// Swahili
			"sw-KE",		// Swahili - Kenya
			"sv",			// Swedish
			"sv-FI",		// Swedish - Finland
			"sv-SE",		// Swedish - Sweden
			"syr",			// Syriac
			"syr-SY",		// Syriac - Syria
			"ta",			// Tamil
			"ta-IN",		// Tamil - India
			"tt",			// Tatar
			"tt-RU",		// Tatar - Russia
			"te",			// Telugu
			"te-IN",		// Telugu - India
			"th",			// Thai
			"th-TH",		// Thai - Thailand
			"tr",			// Turkish
			"tr-TR",		// Turkish - Turkey
			"uk",			// Ukrainian
			"uk-UA",		// Ukrainian - Ukraine
			"ur",			// Urdu
			"ur-PK",		// Urdu - Pakistan
			"uz",			// Uzbek
          	"uz-UZ-Cyrl",	// Uzbek (Cyrillic) - Uzbekistan
          	"uz-UZ-Latn",	// Uzbek (Latin) - Uzbekistan
			"vi",			// Vietnamese
			"vi-VN",		// Vietnamese - Vietnam
		};

        // <doc>
        // <desc>
        //  The list of culture codes that the CultureInfo class supports for Vista.
        // Due to culture code changes as part of Vista and .NET 3.0 we now have to have a seperate array for Vista
        // </desc>
        // <seealso member="GetIFormatProvider"/>
        // </doc>
        private string[] _cultureFormatsVista = new string[]
		{
			"",				// invariant culture
			"af",			// Afrikaans
			"af-ZA",		// Afrikaans - South Africa
			"sq",			// Albanian
			"sq-AL",		// Albanian - Albania
			"ar",			// Arabic
			"ar-DZ",		// Arabic - Algeria
			"ar-BH",		// Arabic - Bahrain
			"ar-EG",		// Arabic - Egypt
			"ar-IQ",		// Arabic - Iraq
			"ar-JO",		// Arabic - Jordan
			"ar-KW",		// Arabic - Kuwait
			"ar-LB",		// Arabic - Lebanon
			"ar-LY",		// Arabic - Libya
			"ar-MA",		// Arabic - Morocco
			"ar-OM",		// Arabic - Oman
			"ar-QA",		// Arabic - Qatar
			"ar-SA",		// Arabic - Saudi Arabia
			"ar-SY",		// Arabic - Syria
			"ar-TN",		// Arabic - Tunisia
			"ar-AE",		// Arabic - United Arab Emirates
			"ar-YE",		// Arabic - Yemen
			"hy",			// Armenian
			"hy-AM",		// Armenian - Armenia
			"az",			// Azeri
            "az-Cyrl-AZ",   // Azeri (Cyrillic) - Azerbaijan
		    "az-Latn-AZ",  // Azeri (Latin) - Azerbaijan
			"eu",			// Basque
			"eu-ES",		// Basque - Basque
			"be",			// Belarusian
			"be-BY",		// Belarusian - Belarus
			"bg",			// Bulgarian
			"bg-BG",		// Bulgarian - Bulgaria
			"ca",			// Catalan
			"ca-ES",		// Catalan - Catalan
			"zh-HK",		// Chinese - Hong Kong SAR
			"zh-MO",		// Chinese - Macau SAR
			"zh-CN",		// Chinese - China
			"zh-Hans",		// Chinese (Simplified)
			"zh-SG",		// Chinese - Singapore
			"zh-TW",		// Chinese - Taiwan
			"zh-Hant",		// Chinese (Traditional)
			"hr",			// Croatian
			"hr-HR",		// Croatian - Croatia
			"cs",			// Czech
			"cs-CZ",		// Czech - Czech Republic
			"da",			// Danish
			"da-DK",		// Danish - Denmark
			"div",			// Dhivehi
			"div-MV",		// Dhivehi - Maldives
			"nl",			// Dutch
			"nl-BE",		// Dutch - Belgium
			"nl-NL",		// Dutch - The Netherlands
			"en",			// English
			"en-AU",		// English - Australia
			"en-BZ",		// English - Belize
			"en-CA",		// English - Canada
			"en-029",		// English - Caribbean
			"en-IE",		// English - Ireland
			"en-JM",		// English - Jamaica
			"en-NZ",		// English - New Zealand
			"en-PH",		// English - Philippines
			"en-ZA",		// English - South Africa
			"en-TT",		// English - Trinidad and Tobago
			"en-GB",		// English - United Kingdom
			"en-US",		// English - United States
			"en-ZW",		// English - Zimbabwe
			"et",			// Estonian
			"et-EE",		// Estonian - Estonia
			"fo",			// Faroese
			"fo-FO",		// Faroese - Faroe Islands
			"fa",			// Farsi
			"fa-IR",		// Farsi - Iran
			"fi",			// Finnish
			"fi-FI",		// Finnish - Finland
			"fr",			// French
			"fr-BE",		// French - Belgium
			"fr-CA",		// French - Canada
			"fr-FR",		// French - France
			"fr-LU",		// French - Luxembourg
			"fr-MC",		// French - Monaco
			"fr-CH",		// French - Switzerland
			"gl",			// Galician
			"gl-ES",		// Galician - Galician
			"ka",			// Georgian
			"ka-GE",		// Georgian - Georgia
			"de",			// German
			"de-AT",		// German - Austria
			"de-DE",		// German - Germany
			"de-LI",		// German - Liechtenstein
			"de-LU",		// German - Luxembourg
			"de-CH",		// German - Switzerland
			"el",			// Greek
			"el-GR",		// Greek - Greece
			"gu",			// Gujarati
			"gu-IN",		// Gujarati - India
			"he",			// Hebrew
			"he-IL",		// Hebrew - Israel
			"hi",			// Hindi
			"hi-IN",		// Hindi - India
			"hu",			// Hungarian
			"hu-HU",		// Hungarian - Hungary
			"is",			// Icelandic
			"is-IS",		// Icelandic - Iceland
			"id",			// Indonesian
			"id-ID",		// Indonesian - Indonesia
			"it",			// Italian
			"it-IT",		// Italian - Italy
			"it-CH",		// Italian - Switzerland
			"ja",			// Japanese
			"ja-JP",		// Japanese - Japan
			"kn",			// Kannada
			"kn-IN",		// Kannada - India
			"kk",			// Kazakh
			"kk-KZ",		// Kazakh - Kazakhstan
			"kok",			// Konkani
			"kok-IN",		// Konkani - India
			"ko",			// Korean
			"ko-KR",		// Korean - Korea
			"ky",			// Kyrgyz
			//"ky-KZ",		// Kyrgyz - Kazakhstan -- This is not supported
			"lv",			// Latvian
			"lv-LV",		// Latvian - Latvia
			"lt",			// Lithuanian
			"lt-LT",		// Lithuanian - Lithuania
			"mk",			// Macedonian
			"mk-MK",		// Macedonian - FYROM
			"ms",			// Malay
			"ms-BN",		// Malay - Brunei
			"ms-MY",		// Malay - Malaysia
			"mr",			// Marathi
			"mr-IN",		// Marathi - India
			"mn",			// Mongolian
			"mn-MN",		// Mongolian - Mongolia
			"no",			// Norwegian
			"nb-NO",		// Norwegian (Bokml) - Norway
			"nn-NO",		// Norwegian (Nynorsk) - Norway
			"pl",			// Polish
			"pl-PL",		// Polish - Poland
			"pt",			// Portuguese
			"pt-BR",		// Portuguese - Brazil
			"pt-PT",		// Portuguese - Portugal
			"pa",			// Punjabi
			"pa-IN",		// Punjabi - India
			"ro",			// Romanian
			"ro-RO",		// Romanian - Romania
			"ru",			// Russian
			"ru-RU",		// Russian - Russia
			"sa",			// Sanskrit
			"sa-IN",		// Sanskrit - India
           // "sr-Cyrl-SP",   // Serbian (Cyrillic) - Serbia)(Does not look to be supported on Vista)
		  //  "sr-Latn-SP",   // Serbian (Latin) - Serbia (Does not look to be supported on Vista)
		    "sk",			// Slovak
			"sk-SK",		// Slovak - Slovakia
			"sl",			// Slovenian
			"sl-SI",		// Slovenian - Slovenia
			"es",			// Spanish
			"es-AR",		// Spanish - Argentina
			"es-BO",		// Spanish - Bolivia
			"es-CL",		// Spanish - Chile
			"es-CO",		// Spanish - Colombia
			"es-CR",		// Spanish - Costa Rica
			"es-DO",		// Spanish - Dominican Republic
			"es-EC",		// Spanish - Ecuador
			"es-SV",		// Spanish - El Salvador
			"es-GT",		// Spanish - Guatemala
			"es-HN",		// Spanish - Honduras
			"es-MX",		// Spanish - Mexico
			"es-NI",		// Spanish - Nicaragua
			"es-PA",		// Spanish - Panama
			"es-PY",		// Spanish - Paraguay
			"es-PE",		// Spanish - Peru
			"es-PR",		// Spanish - Puerto Rico
			"es-ES",		// Spanish - Spain
			"es-UY",		// Spanish - Uruguay
			"es-VE",		// Spanish - Venezuela
			"sw",			// Swahili
			"sw-KE",		// Swahili - Kenya
			"sv",			// Swedish
			"sv-FI",		// Swedish - Finland
			"sv-SE",		// Swedish - Sweden
			"syr",			// Syriac
			"syr-SY",		// Syriac - Syria
			"ta",			// Tamil
			"ta-IN",		// Tamil - India
			"tt",			// Tatar
			"tt-RU",		// Tatar - Russia
			"te",			// Telugu
			"te-IN",		// Telugu - India
			"th",			// Thai
			"th-TH",		// Thai - Thailand
			"tr",			// Turkish
			"tr-TR",		// Turkish - Turkey
			"uk",			// Ukrainian
			"uk-UA",		// Ukrainian - Ukraine
			"ur",			// Urdu
			"ur-PK",		// Urdu - Pakistan
			"uz",			// Uzbek
         //   "uz-Cyrl-UZ",   // Uzbek (Cyrillic) - Uzbekistan (Does not look to be supported on Vista)
		 //   "uz-Latn-UZ",   // Uzbek (Latin) - Uzbekistan (Does not look to be supported on Vista)
		    "vi",			// Vietnamese
			"vi-VN",		// Vietnamese - Vietnam
		};





        // <doc>
        // <desc>
        //  List of valid Japanese ImeMode values.
        // </desc>
        // <see also member="GetValidJapaneseImeValue()"/>
        // </doc>
        private ImeMode[] _validJapaneseImeModes = new ImeMode[]
		{
            ImeMode.Alpha,
            ImeMode.AlphaFull,
            ImeMode.Disable,
            ImeMode.Hiragana,
            ImeMode.Inherit,
            ImeMode.Katakana,
            ImeMode.KatakanaHalf,
            ImeMode.NoControl
        };

        // <doc>
        // <desc>
        //  List of valid Korean ImeMode values.
        // </desc>
        // <see also member="GetValidKoreanImeValue"/>
        // </doc>
        private ImeMode[] _validKoreanImeModes = new ImeMode[]
		{
            ImeMode.Alpha,
            ImeMode.AlphaFull,
            ImeMode.Disable,
            ImeMode.Hangul,
            ImeMode.HangulFull,
            ImeMode.Inherit,
            ImeMode.NoControl
        };

        // <doc>
        // <desc>
        //  Constructs a new RandomUtil object using the system's default locale.
        // </desc>
        // </doc>
        public RandomUtil()
        {
            // Only create a new one if it doesn't already exist.  That way, new
            // instances of RandomUtils won't overwrite rand, which is static.
            //
            if (s_rand == null)
                InitializeRandoms(new Random().Next());

            // This ensures we have a valid min DateTime for the current culture.
            Calendar cal = CultureInfo.CurrentCulture.Calendar;

            // Argh.  There seems to be no global way of finding the minimum date of a calendar.
            // The line below works for ThaiBuddhistCalendar but throws for Hijri.  The alternate
            // works for Hijri but not ThaiBuddhist.  Don't even know about other calendars.
            try
            {
                DateTime min = DateTime.MinValue;
                _minValidDate = new DateTime(cal.GetYear(min), cal.GetMonth(min), cal.GetDayOfMonth(min), CultureInfo.CurrentCulture.Calendar);
            }
            catch
            {
                // If this throws, we'll need to find yet another way to do this.
                //minValidDate = new DateTime(1, 1, 1, CultureInfo.CurrentCulture.Calendar);
            	//Yes, this throws in Vista Arabic x64. Commenting this out temporarily to unblock.

                try
                {
                    _minValidDate = DateTime.Now;
                }
                catch
                {

                }
			}
        }

        public RandomUtil(Log.Log log)
            : this()
        {
            this.Log = log;
        }

        private Log.Log _log = null;
        public Log.Log Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        private bool _logRandomValues = false;
        public bool LogRandomValues
        {
            get
            {
                return _logRandomValues;
            }
            set
            {
                _logRandomValues = value;
            }
        }

        // Args that get passed in here:
        //  0 - name of calling method
        //  1 - string with the method argument types & names
        //  2 - return value
        // Example: if you wanted to show "**** 5 ****" when calling GetInteger, you'd set this to "**** {2} ****"
        private string _logRandomValueFormat = "RandomUtil: {0}{1} returned {2}";
        public string LogRandomValueFormat
        {
            get
            {
                return _logRandomValueFormat;
            }
            set
            {
                _logRandomValueFormat = value;
            }
        }

        //The object that's being logged is passed here in case we decide to do any special stuff for
        //particular types, like write out Height & Width for Controls or Bitmaps (or whatever)
        //Examples: "Picked a number from 1 to 5 and got", 5
        //"Random integer: ", 7
        private void LogValueIfLogging(object obj)
        {
            if (LogRandomValues && Log != null)
            {
                //If we're calling one random function from another, only log the outermost one
                StackTrace st = new StackTrace();
                if (st.GetFrame(2).GetMethod().ReflectedType.Assembly != this.GetType().Assembly)
                {
                    MethodBase mi = st.GetFrame(1).GetMethod();
                    string methodName = mi.Name;
                    string methodString = ReflectBase.GetParameterList(mi);
                    string returnValue = obj.ToString();
                    Log.WriteLine(string.Format(_logRandomValueFormat, methodName, methodString, returnValue));
                }
            }
        }


        //
        // HACK: rand and intlStr MUST be recreated at the same time or else their random
        //       generators will be out of sync.  As long as this is the only place where
        //       rand is assigned to, we're good, but a cleaner solution would be nice.
        //
        private void InitializeRandoms(int seed)
        {
            s_rand = new Random(seed);

            s_unmanagedCodePermission.Assert();
            _intlStr = new IntlStrings((long)seed);
            CodeAccessPermission.RevertAssert();
        }

        // <doc>
        // <desc>
        //  Compares length and equivalence of all elements of two int arrays
        // </desc>
        // <param term="iArray1">
        //  The first array to compare.
        // </param>
        // <param term="iArray2">
        //  The second array to compare
        // </param>
        // <retvalue>
        //  True if both arrays are identical in size and contents; false otherwise.
        // </retvalue>
        // </doc>
        public virtual bool CompareIntArrays(int[] iArray1, int[] iArray2)
        {
            if (iArray1.Length != iArray2.Length)
                return false;
            for (int i = 0; i < iArray1.Length; i++)
            {
                if (iArray1[i] != iArray2[i])
                    return false;
            }
            return true;
        }

        // <doc>
        // <desc>
        //  Compares length and equivalence of all elements of two Object arrays
        // </desc>
        // <param term="oArray1">
        //  The first array to compare.
        // </param>
        // <param term="oArray2">
        //  The second array to compare
        // </param>
        // <retvalue>
        //  True if both arrays are identical in size and contents; false otherwise.
        // </retvalue>
        // </doc>
        public virtual bool CompareObjectArrays(Object[] oArray1, Object[] oArray2)
        {
            if (oArray1.Length != oArray2.Length)
                return false;
            for (int i = 0; i < oArray1.Length; i++)
            {
                if (oArray1[i] != oArray2[i])
                    return false;
            }
            return true;
        }

        // <doc>
        // <desc>
        //  Compares length and equivalence of all elements of two Object arrays.
        //  Objects are compared as Strings using the result of the ToString() method.
        // </desc>
        // <param term="oArray1">
        //  The first array to compare.
        // </param>
        // <param term="oArray2">
        //  The second array to compare
        // </param>
        // <retvalue>
        //  True if both arrays are identical in size and contents; false otherwise.
        // </retvalue>
        // </doc>
        public virtual bool CompareStringArrays(Object[] oArray1, Object[] oArray2)
        {
            if (oArray1.Length != oArray2.Length)
                return false;
            for (int i = 0; i < oArray1.Length; i++)
            {
                if (!oArray1[i].ToString().Equals(oArray2[i].ToString()))
                    return false;
            }
            return true;
        }

        // <doc>
        // <desc>
        //  Returns a random element of the given one-dimensional array.
        //
        //  TODO: handling multidimensional arrays wouldn't be too difficult, but there
        //        is no current need for them.
        //
        // </desc>
        // <param term="array">
        //  The array from which to select an element
        // </param>
        // <retvalue>
        //  A random element from the array, or null if array == null or its length is 0.
        // </retvalue>
        // </doc>
        public virtual object GetArrayElement(Array array)
        {
            if (array == null || array.Length == 0)
                return null;

            // TODO: if array is multidimensional, GetValue(int) will throw an exception.
            //       To handle a multidimensional array, we should use GetValue(int[]).
            //
            int val = GetRange(0, array.Length - 1);
            LogValueIfLogging(array.GetValue(val));
            return array.GetValue(val);
        }

        // Generic version
        public virtual T GetArrayElement<T>(T[] array)
        {
            return (T)GetArrayElement((Array)array);
        }

        // Generic version.  Will return a random element in the array that is not
        // "notMe".
        public virtual T GetArrayElement<T>(T[] array, T notMe)
        {
            return GetArrayElement<T>(array, new T[] { notMe });
        }

        // Generic version.  Will return a random element in the array that is not
        // one of the elements in the "notMe" array.
        public virtual T GetArrayElement<T>(T[] array, T[] notMe)
        {
            T retVal;
            bool foundMatch;
            int numReps = 0;

            do
            {
                retVal = (T)GetArrayElement(array);
                foundMatch = false;

                // Prevent infinite loop
                if (numReps == 10)
                    throw new WinFormsTestLibException("Couldn't find random element not matching 'notMe' within 10 tries");

                foreach (T t in notMe)
                {
                    if (object.Equals(retVal, t))
                    {
                        foundMatch = true;
                        break;
                    }
                }
            } while (foundMatch);  // Just keep going until we get a new element

            return retVal;
        }

        //
        // Same as GetArrayElement() but for the more generic IList.
        //
        public virtual object GetIListElement(IList list)
        {
            if (list == null || list.Count == 0)
                return null;

            object retVal = list[GetRange(0, list.Count - 1)];
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a value from an enum that is not equal to the
        //  specified value.
        // </desc>
        // <param term="enumType">
        //  The enum from which to retrieve the value.
        // </param>
        // <param term="nNotThisValue">
        //  The value to ensure is not returned.
        // </param>
        // <retvalue>
        //  A integral value from the specified enum class that is not
        //  equal to the nNotThisValue parameter.
        // </retvalue>
        // <seealso member="GetEnumValue"/>
        // </doc>
        public virtual int GetDifferentEnumValue(Type enumType, int nNotThisValue)
        {
            Array oValues = Enum.GetValues(enumType);

            // If there is only one field in the enum, return that field
            // regardless of whether it is equal to nNotThisValue.
            if (oValues.Length == 1)
            {
                LogValueIfLogging((int)oValues.GetValue(0));
                return (int)oValues.GetValue(0);
            }

            int nValue;

            do
            {
                int nIndex = GetRange(0, oValues.Length - 1);
                nValue = (int)oValues.GetValue(nIndex);
            }
            while (nValue == nNotThisValue);

            LogValueIfLogging(nValue);
            return nValue;
        }

        public virtual T GetDifferentEnumValue<T>(T notThisValue)
        {
            T val = (T)(object)GetDifferentEnumValue(typeof(T), (int)(object)notThisValue);
            LogValueIfLogging(val);
            return val;
        }

        public virtual T GetDifferentEnumValue<T>(TParams p, params T[] notTheseValues)
        {

            List<T> possibleValues = new List<T>((T[])Enum.GetValues(typeof(T)));
            foreach (T excluded in notTheseValues)
            { possibleValues.Remove(excluded); }
            T ret = possibleValues[p.ru.GetRange(0, possibleValues.Count - 1)];
            LogValueIfLogging(ret);
            return ret;
        }


        // <doc>
        // <desc>
        //  Retrieves a random element from the specified enum.  If the enum is a bit mask,
        //  calls GetEnumValue(enumType, true), i.e. it returns either a random element, or
        //  a combination of several elements from the enum.
        // </desc>
        // <param term="enumType">
        //  The type of the enum to be retrieved
        // </param>
        // <retvalue>
        //  A random integral value from the specified enum class
        // </retvalue>
        // <seealso member="GetDifferentEnumValue"/>
        // </doc>
        public virtual int GetEnumValue(Type enumType)
        {
            int val = GetEnumValue(enumType, enumType.IsDefined(typeof(System.FlagsAttribute), true));
            LogValueIfLogging(val);
            return val;
        }

        public virtual T GetEnumValue<T>()
        {
            T val = (T)(object)GetEnumValue(typeof(T));
            LogValueIfLogging(val);
            return val;
        }

        // <doc>
        // <desc>
        //  Retrieves a random element from the specified enum.  If the enum is a bit mask,
        //  calls GetEnumValue(enumType, true), i.e. it returns either a random element, or
        //  a combination of several elements from the enum.
        // </desc>
        // <param term="enumType">
        //  The type of the enum to be retrieved
        // </param>
        // <param term="combineValues">
        //  If true, may return a combination of enum values, or a single value.  If false,
        //  only returns a single random value from this enum.
        // </param>
        // <retvalue>
        //  A random integral value from the specified enum class
        // </retvalue>
        // <seealso member="GetDifferentEnumValue"/>
        // </doc>
        public virtual int GetEnumValue(Type enumType, bool combineValues)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException(enumType.Name + " is not an enum");

            Array oValues = Enum.GetValues(enumType);
            int numVals = oValues.Length;

            // Only combine values 50% of the time if combineValues is true
            //
            int retVal;
            if (!(combineValues && GetBoolean()))
                retVal = (int)oValues.GetValue(GetRange(0, numVals - 1));
            else
            {
                // Loop through each value.  If it's a power of two, add it
                // to the bit mask 50% the time.
                //
                retVal = 0;

                foreach (int val in oValues)
                {
                    if ((Math.Pow(2, (int)Math.Log(val, 2)) == val) && GetBoolean())
                        retVal |= val;
                }

                // ugh, we made it through without adding any enums; just choose one
                if (retVal == 0)
                    retVal = (int)oValues.GetValue(GetRange(0, numVals - 1));

            }
            LogValueIfLogging(retVal);
            return retVal;
        }

        public virtual T GetEnumValue<T>(bool combineValues)
        {
            T retVal = (T)(object)GetEnumValue(typeof(T), combineValues);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a valid Japanese ImeMode value.
        // </desc>
        // <retvalue>
        //  A random value from the ImeMode enum that is valid for Japanese IME.
        // </retvalue>
        // <see also member="GetEnumValue"/>
        // </doc>
        public virtual int GetValidJapaneseImeValue()
        {
            int nValue;
            int nIndex = GetRange(0, _validJapaneseImeModes.Length - 1);
            nValue = (int)_validJapaneseImeModes[nIndex];
            LogValueIfLogging(nValue);
            return nValue;
        }

        // <doc>
        // <desc>
        //  Retrieves a valid Korean ImeMode value.
        // </desc>
        // <retvalue>
        //  A random value from the ImeMode enum that is valid for Korean IME.
        // </retvalue>
        // <see also member="GetEnumValue"/>
        // </doc>
        public virtual int GetValidKoreanImeValue()
        {
            int nValue;
            int nIndex = GetRange(0, _validKoreanImeModes.Length - 1);
            nValue = (int)_validKoreanImeModes[nIndex];
            LogValueIfLogging(nValue);
            return nValue;
        }

        // <doc>
        // <desc>
        //  Retrieves a random byte value.
        // </desc>
        // <retvalue>
        //  A random byte value.
        // </retvalue>
        // </doc>
        public virtual Byte GetByte()
        {
            Byte retVal = (Byte)GetRange(Byte.MinValue, Byte.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random int16 value.
        // </desc>
        // <retvalue>
        //  A random int16 value.
        // </retvalue>
        // </doc>
        public virtual Int16 GetInt16()
        {
            Int16 retVal = (Int16)GetRange(Int16.MinValue, Int16.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random integer value.
        // </desc>
        // <retvalue>
        //  A random integer value.
        // </retvalue>
        // </doc>
        public virtual int GetInt()
        {
            int retVal = s_rand.Next();
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random Double such that 0 <= n < 1.0.
        // </desc>
        // <retvalue>
        //  A random Double value.
        // </retvalue>
        // </doc>
        public virtual double GetDouble()
        {
            double retVal = s_rand.NextDouble();
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Retrieves a random float between 0.0 and 1.0.
        // </desc>
        // <retvalue>
        //  A random float value between 0.0 and 1.0.
        // </retvalue>
        // </doc>
        public virtual float GetFloat()
        {
            float retVal = (float)GetDouble();
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Retrieves a random float between min and max.
        // </desc>
        // <param term="min">
        //  The minimum bound for the return value. min must be less than max.
        // </param>
        // <param term="max">
        //  The maximum bound for the return value.
        // <retvalue>
        //  A random float value between min and max.
        // </retvalue>
        // </doc>
        public virtual float GetFloat(float min, float max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException("max must be greater than min");
            double retVal = GetFloat() * ((double)max - (double)min) + (double)min;
            LogValueIfLogging(retVal);
            return (float)retVal;
        }
        // TAK
        // <doc>
        // <desc>
        //  Returns a random PointF.
        // </desc>
        // <retvalue>
        //  A PointF object with random X and Y values.
        // </retvalue>
        // </doc>
        public virtual PointF GetPointF()
        {
            PointF retVal = new PointF(GetFloat(float.MinValue, float.MaxValue), GetFloat(float.MinValue, float.MaxValue));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Retrieves a PointF object whose X and Y coordinates are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="xMax">
        //  The maximum value for the X coordinate.
        // </param>
        // <param term="yMax">
        //  The maximum value for the Y coordinate.
        // </param>
        // <retvalue>
        //  A PointF object whose X and Y coordinates are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual PointF GetPointF(float xMax, float yMax)
        {
            PointF retVal = new PointF(GetFloat(0, xMax), GetFloat(0, yMax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Gets a PointF within the given range.
        // </desc>
        // <param term="xmin">
        //  The minimum value for the x field in the Point
        // </param>
        // <param term="xmax">
        //  The maximum value for the x field in the Point
        // </param>
        // <param term="ymin">
        //  The minimum value for the y field in the Point
        // </param>
        // <param term="ymax">
        //  The maximum value for the y field in the Point
        // </param>
        // <retvalue>
        //  A PointF within the given range.
        // </retvalue>
        // </doc>
        public virtual PointF GetPointF(float xmin, float xmax, float ymin, float ymax)
        {
            PointF retVal = new PointF(GetFloat(xmin, xmax), GetFloat(ymin, ymax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Returns a random SizeF.
        // </desc>
        // <retvalue>
        //  A SizeF object with random Width and Height values.
        // </retvalue>
        // </doc>
        public virtual SizeF GetSizeF()
        {
            SizeF retVal = new SizeF(GetFloat(), GetFloat());
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Retrieves a SizeF object whose Width and Height coordinates
        //  are between 0 and the specified maximum values.
        // </desc>
        // <param term="wMax">
        //  The maximum value for the Width coordinate.
        // </param>
        // <param term="hMax">
        //  The maximum value for the Height coordinate.
        // </param>
        // <retvalue>
        //  A SizeF object whose Width and Height coordinates are between
        //  0 and the specified maximum values.
        // </retvalue>
        // </doc>
        public virtual SizeF GetSizeF(float wMax, float hMax)
        {
            SizeF retVal = GetSizeF(0, wMax, 0, hMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Gets a SizeF within the given range.
        // </desc>
        // <param term="wMin">
        //  The minimum value for the Width field in the SizeF
        // </param>
        // <param term="wMax">
        //  The maximum value for the Width field in the SizeF
        // </param>
        // <param term="hMin">
        //  The minimum value for the Height field in the SizeF
        // </param>
        // <param term="hMax">
        //  The maximum value for the Height field in the SizeF
        // </param>
        // <retvalue>
        //  A SizeF within the given range.
        // </retvalue>
        // </doc>
        public virtual SizeF GetSizeF(float wMin, float wMax, float hMin, float hMax)
        {
            SizeF retVal = new SizeF(GetFloat(wMin, wMax), GetFloat(hMin, hMax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Creates a RectangleF with random bounds.
        // </desc>
        // <retvalue>
        //  A RectangleF whose X and Y coordinates and Width and Height are
        //  random floats.
        // </retvalue>
        // </doc>
        public virtual RectangleF GetRectangleF()
        {
            float fX = GetFloat(float.MinValue, float.MaxValue);
            float fY = GetFloat(float.MinValue, float.MaxValue);
            float fHeight = GetFloat(0, (Single)Math.Min(float.MaxValue, float.MaxValue - fX));
            float fWidth = GetFloat(0, (Single)Math.Min(float.MaxValue, float.MaxValue - fY));

            RectangleF retVal = new RectangleF(fX, fY, fHeight, fWidth);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a RectangleF with random bounds the specified limits
        // </desc>
        // <param term="xMax">
        //  The maximum value for the X coordinate.
        // </param>
        // <param term="yMax">
        //  The maximum value for the Y coordinate.
        // </param>
        // <param term="hMax">
        //  The maximum value for the rectangle's Height
        // </param>
        // <param term="wMax">
        //  The maximum value for the rectangle's Width
        // </param>
        // <retvalue>
        //  A RectangleF whose X and Y coordinates and Width and Height are
        //  random floats between 0.0 and the specified limits.
        // </retvalue>
        // </doc>
        public virtual RectangleF GetRectangleF(float xMax, float yMax, float wMax, float hMax)
        {
            float fX = GetFloat(float.MinValue, xMax);
            float fY = GetFloat(float.MinValue, yMax);
            float fWidth = GetFloat(0, (Single)Math.Min(Math.Min(float.MaxValue, float.MaxValue - fX), wMax));
            float fHeight = GetFloat(0, (Single)Math.Min(Math.Min(float.MaxValue, float.MaxValue - fY), hMax));

            RectangleF retVal = new RectangleF(fX, fY, fWidth, fHeight);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a RectangleF with random bounds the specified limits
        // </desc>
        // <param term="Max">
        //  The maximum value for the all coordinates.
        // </param>
        // <param term="Min">
        //  The minimum value for the X, Y coordinates.
        // </param>
        // <retvalue>
        //  A RectangleF whose X and Y are bounded by Min and Max
        // and Width and Height are bounded by 0 and Max.
        // </retvalue>
        // </doc>
        public virtual RectangleF GetRectangleF(float Min, float Max)
        {
            float fX = GetFloat(Min, Max);
            float fY = GetFloat(Min, Max);
            float fWidth = GetFloat(0, (Single)Math.Min(Math.Min(float.MaxValue, float.MaxValue - fX), Max));
            float fHeight = GetFloat(0, (Single)Math.Min(Math.Min(float.MaxValue, float.MaxValue - fY), Max));

            RectangleF retVal = new RectangleF(fX, fY, fWidth, fHeight);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // TAK
        // <doc>
        // <desc>
        //  Returns a SolidBrush object using a random KnownColor.
        // </desc>
        // <retvalue>
        //  A SolidBrush using a random KnownColor.
        // </retvalue>
        // </doc>
        //
        public virtual Brush GetBrush()
        {
            return new SolidBrush(Color.FromKnownColor((KnownColor)GetEnumValue(typeof(KnownColor))));
        }

        // TAK
        // <doc>
        // <desc>
        //  Returns a Pen object using a random KnownColor.
        // </desc>
        // <retvalue>
        //  A Pen using a random KnownColor.
        // </retvalue>
        // </doc>
        //
        public virtual Pen GetPen()
        {
            return new Pen(Color.FromKnownColor((KnownColor)GetDifferentEnumValue(typeof(KnownColor), (int)KnownColor.Transparent)));
        }


        // <doc>
        // <desc>
        //  Retrieves a random integer value that is optionally unsigned.
        // </desc>
        // <param term="bUnsigned">
        //  If true the absolute value of the randomly selected integer is returned.
        // </param>
        // <retvalue>
        //  A random integer value. The value is made positive if the bUnsigned
        //  parameter is true.
        // </retvalue>
        // </doc>
        public virtual int GetInt(bool bUnsigned)
        {
            int i = s_rand.Next();
            if (bUnsigned && (i < 0))
                i = -i;

            LogValueIfLogging(i);
            return i;
        }

        // <doc>
        // <desc>
        //  Retrieves a random long value.
        // </desc>
        // <retvalue>
        //  A random value of type System.Int64.
        // </retvalue>
        // </doc>
        public virtual long GetInt64()
        {
            Int64 retVal = GetInt64(Int64.MinValue, Int64.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random long value within a given range.
        // </desc>
        // <param term="min">
        //  The minimuim value.
        // </param>
        // <param term="max">
        //  The maximum value.
        // </param>
        // <retvalue>
        //  A random value of type System.Int64 between the specified minimum
        //  and maximum values.
        // </retvalue>
        // </doc>
        public virtual long GetInt64(long min, long max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException("max must be greater than min");

            // There is no way to method on System.Random to get a long so we
            // need to calculate one ourselves.
            //
            // Stuff everything into a Decimal to maintain precision.
            //
            Int64 retVal = (long)((decimal)GetDouble() * ((decimal)max - (decimal)min) + (decimal)min);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random boolean value
        // </desc>
        // <retvalue>
        //  Randomly true or false
        // </retvalue>
        // </doc>
        public virtual bool GetBoolean()
        {
            bool retVal = (this.GetInt() % 2 == 0);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random DateTime value.
        // </desc>
        // <retvalue>
        //  A random value of type System.DateTime.
        // </retvalue>
        // </doc>
        public virtual DateTime GetDateTime()
        {
            DateTime retVal = GetDateTime(DateTime.MinValue, DateTime.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random DateTime value within a given range.
        // </desc>
        // <param term="min">
        //  The minimuim value.
        // </param>
        // <param term="max">
        //  The maximum value.
        // </param>
        // <retvalue>
        //  A random value of type System.DateTime between the specified minimum
        //  and maximum values.
        // </retvalue>
        // </doc>
        public virtual DateTime GetDateTime(DateTime min, DateTime max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("max must be greater than min");

            DateTime retVal = new DateTime(GetInt64(min.Ticks, max.Ticks));
            LogValueIfLogging(retVal);
            return retVal;
        }

        //
        // In some locales which use different calendars (e.g. Arabic, which uses the Hijiri calendar),
        // the range of valid DateTimes is smaller than the standard Gregorian calendar.  This method
        // will return a DateTime that should be valid for the current culture.
        //
        // HACK: In v1, there is no direct way to query a calendar for its min and max dates.  So we
        //       determine the min date by creating a DateTime for 1/1/0001 in the current culture's
        //       calendar.  The max date we assume to still be MaxValue.  If properties are added in
        //       a future version, we should use them instead of this hack.
        //
        public virtual DateTime GetValidDateTime()
        {
            DateTime retVal = GetDateTime(_minValidDate, DateTime.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random TimeSpan value.
        // </desc>
        // <retvalue>
        //  A random value of type System.TimeSpan.
        // </retvalue>
        // </doc>
        public virtual TimeSpan GetTimeSpan()
        {
            TimeSpan retVal = GetTimeSpan(TimeSpan.MinValue, TimeSpan.MaxValue);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a random TimeSpan value within a given range.
        // </desc>
        // <param term="min">
        //  The minimuim value.
        // </param>
        // <param term="max">
        //  The maximum value.
        // </param>
        // <retvalue>
        //  A random value of type System.TimeSpan between the specified minimum
        //  and maximum values.
        // </retvalue>
        // </doc>
        public virtual TimeSpan GetTimeSpan(TimeSpan min, TimeSpan max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("max must be greater than min");

            TimeSpan retVal = new TimeSpan(GetInt64(min.Ticks, max.Ticks));
            LogValueIfLogging(retVal);
            return retVal;
        }


        // <doc>
        // <desc>
        //  Returns a bmp, emf, gif, jpg, ico, or wmf object based on
        //  the value provided in the ImageStyle enum. If ImageStyle.Random
        //  is selected, then a random Image type is returned.
        // </desc>
        // <param term="type">
        //  The type of image to return
        // </param>
        // <retvalue>
        //  An image from the images this library knows about.
        // </retvalue>
        // <seealso class="ImageStyle"/>
        // <seealso member="ImageNames"/>
        // </doc>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public virtual Image GetImage(ImageStyle type)
        {
            // If the type specfied is ImageStyle.Random
            // change type to be a random element from ImageStyle
            if (type == ImageStyle.Random)
                type = (ImageStyle)(GetInt(true) % (int)type);

            // BUGBUG: once the resource manager stuff works, get rid of the hard-coded
            // filepath in the #else clause below...

            String fileName = _imageNames[(int)type];

            // Arrrrrrrgh!!  Got OutOfMemoryExceptions in XCheckBox with this fix.
            // We'll have to hold off for now.
            /*
                       // We go through all this hocus pocus so we don't lock up the file
                       // for the lifetime of the image.
                       FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                       int len = (int)f.Length;
                       byte[] b = new byte[len];
                       f.Read(b, 0, len);
                       f.Close();

                       MemoryStream m = new MemoryStream(b);
                       return Image.FromStream(m);
            */
            // This would lock up the file and occasionally cause a failure if 2 tests tried
            // to access the same file at the same time.
            return Image.FromFile(fileName);
#if false
            Console.WriteLine("RandomUtil.GetImage(): trying to create " + ImageNames[(int)type]);
            Stream imageStream = ResourceManager.GetResource(GetType(),
                                                             ImageNames[(int)type]);
            Image i = Image.LoadImage(imageStream);
            Console.WriteLine("RandomUtil.GetImage(): returning " + i);
            return i;
//#else
            String fileName = String.Concat("\\\\radt\\vbssdb\\vbtests\\shadow\\wfcclientruntime\\libs\\WFCTestLib\\Util\\Images\\", ImageNames[(int)type]);
            Console.WriteLine("RandomUtil.GetImage(): loading " + fileName);
            //FileStream fs = File.OpenRead(fileName);
            File f = new File(fileName);
            Stream fs = f.Open(FileMode.Open , FileAccess.Read );
            Image i =  Image.FromStream(fs);
            fs.Close();
            return i ;
#endif
        }

        // <doc>
        // <desc>
        //  Returns a ImageCodecInfo taken from the merger of ImageCodecInfo.GetImageDecoders and
        //  ImageCodecInfo.GetImageEncoders
        // </desc>
        // <retvalue>
        //  a ImageCodecInfo
        // </retvalue>
        // </doc>
        public virtual ImageCodecInfo GetImageCodecInfo()
        {
            ImageCodecInfo[] decoders = ImageCodecInfo.GetImageDecoders();
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo[] allCodecs = new ImageCodecInfo[decoders.Length + encoders.Length];

            decoders.CopyTo(allCodecs, 0);
            encoders.CopyTo(allCodecs, decoders.Length);

            return allCodecs[GetRange(0, allCodecs.Length - 1)];
        }

        public virtual Image GetImage()
        {
            return GetImage(ImageStyle.Random);
        }

        // TAK
        // <doc>
        // <desc>
        //  Returns a random Icon object from the IconNames list of
        //  icons that the library knows about.
        // </desc>
        // </param>
        // <retvalue>
        //  An icon from the icons this library knows about.
        // </retvalue>
        // <seealso member="IconNames"/>
        // </doc>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        public virtual Icon GetIcon()
        {
            int index = GetRange(0, _iconNames.Length - 1);
            String fileName = _iconNames[index];
            Console.WriteLine("RandomUtil.GetIcon(): loading " + fileName);
            //FileStream fs = File.OpenRead(fileName);
            Stream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Icon ico = new Icon(fs);
            fs.Close();
            return ico;
        }

        // <doc>
        // <desc>
        //  Returns a random Point.
        // </desc>
        // <retvalue>
        //  A Point object with random X and Y values.
        // </retvalue>
        // </doc>
        public virtual Point GetPoint()
        {
            int iX = GetRange(Int16.MinValue, Int16.MaxValue);
            int iY = GetRange(Int16.MinValue, Int16.MaxValue);
            Point retVal = new Point(iX, iY);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Point object whose X and Y coordinates are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="xMax">
        //  The maximum value for the X coordinate.
        // </param>
        // <param term="yMax">
        //  The maximum value for the Y coordinate.
        // </param>
        // <retvalue>
        //  A Point object whose X and Y coordinates are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Point GetPoint(int xMax, int yMax)
        {
            Point retVal = GetPoint(0, xMax, 0, yMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Point object whose X and Y coordinates are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="p">
        //  The Point to use to determine the maximum values for the X and Y
        //  of the new point.
        // </param>
        // <retvalue>
        //  A Point object whose X and Y coordinates are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Point GetPoint(Point p)
        {
            Point retVal = GetPoint(p.X, p.Y);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Gets a Point within the given range.
        // </desc>
        // <param term="xmin">
        //  The minimum value for the x field in the Point
        // </param>
        // <param term="xmax">
        //  The maximum value for the x field in the Point
        // </param>
        // <param term="ymin">
        //  The minimum value for the y field in the Point
        // </param>
        // <param term="ymax">
        //  The maximum value for the y field in the Point
        // </param>
        // <retvalue>
        //  A Point within the given range.
        // </retvalue>
        // </doc>
        public virtual Point GetPoint(int xmin, int xmax, int ymin, int ymax)
        {
            Point retVal = new Point(GetRange(xmin, xmax), GetRange(ymin, ymax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        //
        // Gets a point within the given rectangle (borders included).
        //
        public virtual Point GetPoint(Rectangle r)
        {
            Point retVal = GetPoint(r.X, r.X + r.Width - 1, r.Y, r.Y + r.Height - 1);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Returns a random Size.
        // </desc>
        // <retvalue>
        //  A Size object with random X and Y values.
        // </retvalue>
        // </doc>
        public virtual Size GetSize()
        {
            Size retVal = new Size(s_rand.Next(), s_rand.Next());
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Size object whose X and Y coordinates are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="xMax">
        //  The maximum value for the X coordinate.
        // </param>
        // <param term="yMax">
        //  The maximum value for the Y coordinate.
        // </param>
        // <retvalue>
        //  A Size object whose X and Y coordinates are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Size GetSize(int xMax, int yMax)
        {
            Size retVal = GetSize(0, xMax, 0, yMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Size object whose X and Y coordinates are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="p">
        //  The Size to use to determine the maximum values for the X and Y
        //  of the new point.
        // </param>
        // <retvalue>
        //  A Size object whose X and Y coordinates are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Size GetSize(Size s)
        {
            Size retVal = GetSize(s.Width, s.Height);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Gets a Size within the given range.
        // </desc>
        // <param term="xmin">
        //  The minimum value for the x field in the Size
        // </param>
        // <param term="xmax">
        //  The maximum value for the x field in the Size
        // </param>
        // <param term="ymin">
        //  The minimum value for the y field in the Size
        // </param>
        // <param term="ymax">
        //  The maximum value for the y field in the Size
        // </param>
        // <retvalue>
        //  A Size within the given range.
        // </retvalue>
        // </doc>
        public virtual Size GetSize(int xmin, int xmax, int ymin, int ymax)
        {
            Size retVal = new Size(GetRange(xmin, xmax), GetRange(ymin, ymax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Returns a random Padding.
        // </desc>
        // <retvalue>
        //  A Padding object with random Left, Top, Right, and Bottom values.
        // </retvalue>
        // </doc>
        public virtual Padding GetPadding()
        {
            Padding retVal = new Padding(s_rand.Next(), s_rand.Next(), s_rand.Next(), s_rand.Next());
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Padding object whose Left, Top, Right, and Bottom field are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="leftMax">
        //  The maximum value for the Left field.
        // </param>
        // <param term="topMax">
        //  The maximum value for the Top field.
        // </param>
        // <param term="rightMax">
        //  The maximum value for the Right field.
        // </param>
        // <param term="bottomMax">
        //  The maximum value for the Bottom field.
        // </param>
        // <retvalue>
        //  A Padding object whose Left, Top, Right, and Bottom fields are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Padding GetPadding(int leftMax, int topMax, int rightMax, int bottomMax)
        {
            Padding retVal = GetPadding(0, leftMax, 0, topMax, 0, rightMax, 0, bottomMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Padding object whose Left, Top, Right, and Bottom fields are between
        //  0 and the specified maximum values.
        // </desc>
        // <param term="p">
        //  The Padding to use to determine the maximum values for the Left, Top, Right, and Bottom
        //  of the new Padding.
        // </param>
        // <retvalue>
        //  A Padding object whose Left, Top, Right, and Bottom fields are between 0 and the
        //  specified maximum values.
        // </retvalue>
        // </doc>
        public virtual Padding GetPadding(Padding pd)
        {
            Padding retVal = GetPadding(pd.Left, pd.Top, pd.Right, pd.Bottom);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Gets a Padding within the given range.
        // </desc>
        // <param term="leftMin">
        //  The minimum value for the Left field in the Padding
        // </param>
        // <param term="leftMax">
        //  The maximum value for the Left field in the Padding
        // </param>
        // <param term="topMin">
        //  The minimum value for the Top field in the Padding
        // </param>
        // <param term="topMax">
        //  The maximum value for the Top field in the Padding
        // </param>
        // <param term="rightMin">
        //  The minimum value for the Right field in the Padding
        // </param>
        // <param term="rightMax">
        //  The maximum value for the Right field in the Padding
        // </param>
        // <param term="bottomMin">
        //  The minimum value for the Bottom field in the Padding
        // </param>
        // <param term="bottomMax">
        //  The maximum value for the Bottom field in the Padding
        // </param>
        // <retvalue>
        //  A Padding within the given range.
        // </retvalue>
        // </doc>
        public virtual Padding GetPadding(int leftMin, int leftMax, int topMin, int topMax, int rightMin, int rightMax, int bottomMin, int bottomMax)
        {
            Padding retVal = new Padding(GetRange(leftMin, leftMax), GetRange(topMin, topMax), GetRange(rightMin, rightMax), GetRange(bottomMin, bottomMax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Returns a random Padding with four equal dimensions.
        // </desc>
        // <retvalue>
        //  A Padding object with random value for all dimensions.
        // </retvalue>
        // </doc>
        public virtual Padding GetPaddingAll()
        {
            Padding retVal = new Padding(s_rand.Next());
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Padding object whose values for all four dimensions are equal and
        //  between 0 and the specified maximum value.
        // </desc>
        // <param term="allMax">
        //  The maximum value for all four dimensions.
        // </param>
        // <retvalue>
        //  A Padding object whose four dimensions are equal and between
        //  0 and the specified maximum value.
        // </retvalue>
        // </doc>
        public virtual Padding GetPaddingAll(int allMax)
        {
            Padding retVal = GetPaddingAll(0, allMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Gets a Padding.All within the given range.
        // </desc>
        // <param term="allMin">
        //  The minimum value for all four fields in the Padding
        // </param>
        // <param term="allMax">
        //  The maximum value for all four fields in the Padding
        // </param>
        // <retvalue>
        //  A Padding within the given range.
        // </retvalue>
        // </doc>
        public virtual Padding GetPaddingAll(int allMin, int allMax)
        {
            Padding retVal = new Padding(GetRange(allMin, allMax));
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a rectangle with random bounds.
        // </desc>
        // <retvalue>
        //  A rectangle whose X and Y coordinates and Width and Height are
        //  random integers.
        // </retvalue>
        // </doc>
        public virtual Rectangle GetRectangle()
        {
            int iX = GetRange(Int16.MinValue, Int16.MaxValue);
            int iY = GetRange(Int16.MinValue, Int16.MaxValue);
            int iHeight = GetRange(0, Int16.MaxValue - iX);
            int iWidth = GetRange(0, Int16.MaxValue - iY);

            Rectangle retVal = new Rectangle(iX, iY, iHeight, iWidth);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a rectangle with random bounds the specified limits
        // </desc>
        // <param term="xMax">
        //  The maximum value for the X coordinate.
        // </param>
        // <param term="yMax">
        //  The maximum value for the Y coordinate.
        // </param>
        // <param term="iHeightMax">
        //  The maximum value for the rectangle's Height
        // </param>
        // <param term="iWidthMax">
        //  The maximum value for the rectangle's Width
        // </param>
        // <retvalue>
        //  A rectangle whose X and Y coordinates and Width and Height are
        //  random integers between 0 and the specified limits.
        // </retvalue>
        // </doc>
        public virtual Rectangle GetRectangle(int xMax, int yMax, int iHeightMax, int iWidthMax)
        {
            int iX = GetRange(0, xMax);
            int iY = GetRange(0, yMax);
            int iHeight = GetRange(0, iHeightMax);
            int iWidth = GetRange(0, iWidthMax);

            Rectangle retVal = new Rectangle(iX, iY, iHeight, iWidth);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves a Rectangle that is guaranteed to intersect with the Rectangle provided.
        // </desc>
        // <param term="intersectsWith">
        //  The rectangle to intersect with
        // </param>
        // <retvalue>
        //  A rectangle that intersects with the rectangle provided.
        // </retvalue>
        // </doc>
        public virtual Rectangle GetIntersectingRectangle(Rectangle intersectsWith)
        {
            Rectangle newRect;

            // This will get stuck in an infinite loop if Width or Height is 0.  Workaround is
            // to change width.  This shouldn't affect the correctness of our result.
            //
            if (intersectsWith.Width == 0)
                intersectsWith.Width = GetRange(0, Int16.MaxValue / 5);

            if (intersectsWith.Height == 0)
                intersectsWith.Height = GetRange(0, Int16.MaxValue / 5);

            do
            {
                newRect = new Rectangle(GetRange(intersectsWith.X - intersectsWith.Width, intersectsWith.X + intersectsWith.Width),
                                        GetRange(intersectsWith.Y - intersectsWith.Height, intersectsWith.Y + intersectsWith.Height),
                                        GetRange(0, intersectsWith.Width),
                                        GetRange(0, intersectsWith.Height));
            } while (!newRect.IntersectsWith(intersectsWith));

            LogValueIfLogging(newRect);
            return newRect;
        }

        public virtual Rectangle GetInternalRectangle(Rectangle outer)
        {
            Rectangle newRect = new Rectangle();

            // This will get stuck in an infinite loop if Width or Height is 0.  Workaround is
            // to change width.  This shouldn't affect the correctness of our result.
            //
            newRect.X = GetRange(outer.Left, outer.Right);
            newRect.Width = GetRange(0, outer.Right - newRect.X);

            newRect.Y = GetRange(outer.Top, outer.Bottom);
            newRect.Height = GetRange(0, outer.Bottom - newRect.Y);


            LogValueIfLogging(newRect);
            return newRect;
        }


        // <doc>
        // <desc>
        //  Retrieves a Rectangle that is guaranteed to be contained within the Rectangle provided.
        // </desc>
        // <param term="containingRect">
        //  The rectangle to contain the one returned
        // </param>
        // <retvalue>
        //  A rectangle that is contained within the rectangle provided.
        // </retvalue>
        // </doc>
        public virtual Rectangle GetContainedRectangle(Rectangle containingRect)
        {
            int newX, newY, newWidth, newHeight;
            newX = GetRange(containingRect.X, containingRect.Right);
            newY = GetRange(containingRect.Y, containingRect.Bottom);
            newWidth = GetRange(0, containingRect.Right - newX);
            newHeight = GetRange(0, containingRect.Bottom - newY);
            Rectangle retVal = new Rectangle(newX, newY, newWidth, newHeight);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a new Color object with random RGB values.
        // </desc>
        // <retval>
        //  A Color object with random values for Red, Green
        //  and Blue.
        // </retval>
        // </doc>
        public virtual Color GetColor()
        {
            byte rval, gval, bval;

            rval = (byte)GetRange(0, MaxRGBInt);
            gval = (byte)GetRange(0, MaxRGBInt);
            bval = (byte)GetRange(0, MaxRGBInt);

            Color c = Color.FromArgb(rval, gval, bval);
            LogValueIfLogging(c);
            return c;
        }

        // TAK
        // <doc>
        // <desc>
        //  Creates a new Color object with random ARGB values.
        // </desc>
        // <retval>
        //  A Color object with random values for Alpha, Red, Green
        //  and Blue.
        // </retval>
        // </doc>
        public virtual Color GetARGBColor()
        {
            byte aval, rval, gval, bval;

            aval = (byte)GetRange(0, MaxRGBInt);
            rval = (byte)GetRange(0, MaxRGBInt);
            gval = (byte)GetRange(0, MaxRGBInt);
            bval = (byte)GetRange(0, MaxRGBInt);

            Color c = Color.FromArgb(aval, rval, gval, bval);
            LogValueIfLogging(c);
            return c;
        }

        // <doc>
        // <desc>
        //  Generates a string composed of consecutive ascending ansi chars values thus
        //  assurring every character is hit
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <retvalue>
        //  A string of Consecutive ascending characters.
        // </retvalue>
        // </doc>
        public string GetAnsiString(short nMaxLength, bool bAbsolute, bool bValidOnly)
        {
            string retVal = _intlStr.GetString(nMaxLength, bAbsolute, bValidOnly, false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Generates a string that is composed of at least one character that can not be
        //  represeted in ASCII.  First it trys GetString() and searches for non-ASCII characters.
        //  If it can't any it'll generate a string using the Japanese culture info.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <retvalue>
        //  A string of characters.
        // </retvalue>
        // </doc>
        public string GetNonAsciiString(short nMaxLength)
        {
            string retVal = GetString(nMaxLength);
            bool found = false;

            foreach (char c in retVal.ToCharArray())
            {
                if ((int)c >= 128)
                {
                    found = true;
                    break;
                }
            }

            if (!found) { retVal = _intlStr.GetStrLcid(nMaxLength, true, true, enuLCIDList.Japanese); }
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Generates a string that is composed of problem characters in various positions.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <retvalue>
        //  returns one of the top 20 problematic strings.
        // </retvalue>
        // </doc>
        public string GetProbCharString(short nMaxLength, bool bAbsolute, bool bValidOnly)
        {
            string retVal = _intlStr.GetProbCharString(nMaxLength, bAbsolute, bValidOnly);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Retrieves all the problematic strings for the current Locale.
        // </desc>
        // <retvalue>
        //  All the problem strings for the current Locale.
        // </retvalue>
        // </doc>
        public string[] GetProblematicStrings()
        {
            return _intlStr.GetProblematicStrings();
        }

        // <doc>
        // <desc>
        //  This function returns a string of problem Unicode Round Trip Conversion
        //  characters if there are found problem characters for the tested code page.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <retvalue>
        //  A problem URT string
        // </retvalue>
        // </doc>
        public string GetProbURTCString(short nMaxLength, bool bAbsolute, bool bValidOnly)
        {
            string retVal = _intlStr.GetProbURTCString(nMaxLength, bAbsolute, bValidOnly);
            LogValueIfLogging(retVal);
            return retVal;
        }


        // <doc>
        // <desc>
        //  This function returns random ansi characters.
        //  Random character generation is based on the input lcid.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <param term="enuLCIDType">
        //  Valid lcid number to generate the random string
        // </param>
        // <retvalue>
        //  A random generated string
        // </retvalue>
        // </doc>
        public string GetRandStrLCID(short nMaxLength, bool bAbsolute, bool bValidOnly, enuLCIDList LCIDType)
        {
            //TODO: Verify the LCID value.
            string retVal = _intlStr.GetRandStrLCID(nMaxLength, bAbsolute, bValidOnly, LCIDType, false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  To generate a string composed of consecutive ascending ansi characters values
        //  thus assurring every character is hit.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <param term="enuLCIDType">
        //  Valid lcid number to generate the random string
        // </param>
        // <retvalue>
        //  A consecutive ascending string
        // </retvalue>
        // </doc>
        public string GetStrLcid(short nMaxLength, bool bAbsolute, bool bValidOnly, enuLCIDList enuLCIDType)
        {
            //TODO: Verify the LCID value.
            string retVal = _intlStr.GetStrLcid(nMaxLength, bAbsolute, bValidOnly, enuLCIDType, false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  To generate a string that is composed of problem characters in various positions.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <param term="enuLCIDType">
        //  Valid lcid number to generate the random string
        // </param>
        // <retvalue>
        //  A consecutive ascending string
        // </retvalue>
        // </doc>
        public string GetProbCharStrLCID(short nMaxLength, bool bAbsolute, bool bValidOnly, enuLCIDList enuLCIDType)
        {
            //TODO: Verify the LCID value.
            string retVal = _intlStr.GetProbCharStrLCID(nMaxLength, bAbsolute, bValidOnly, enuLCIDType);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  To generate a string that is composed of Unicode Round Trip Conversion
        //  problem characters in various positions.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="enuLCIDType">
        //  Valid lcid number to generate the random string
        // </param>
        // <retvalue>
        //  A consecutive ascending string
        // </retvalue>
        // </doc>
        public string GetProbURTCStrLCID(short nMaxLength, bool bAbsolute, bool bValidOnly, enuLCIDList enuLCIDType)
        {
            //TODO: Verify the LCID value.
            string retVal = _intlStr.GetProbURTCStrLCID(nMaxLength, bAbsolute, true, enuLCIDType);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  To generate a string that is composed of Unicode Round Trip Conversion
        //  problem characters in various positions.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bAbsolute">
        //  if set true, exact number (nMaxLength) will be generated, if set falise, number of generated chars will be random.
        // </param>
        // <param term="bValidOnly">
        //  boolean, if true, verify generated characters are valid, if false, does not verify generated characters
        // </param>
        // <param term="intCodeType">
        //  Type of conversion to make. CODE_UNI, CODE_UTF7, CODE_UTF8.
        // </param>
        // <param term="enuLCIDType">
        //  Valid lcid number to generate the random string
        // </param>
        // <retvalue>
        //  A consecutive ascending string
        // </retvalue>
        // </doc>
        [Obsolete("This method is obsolete in GenStrings.")]
        public string GetUniStrRandAnsi(short nMaxLength, bool bAbsolute, bool bValidOnly, enuCodeType CodeType, enuLCIDList enuLCIDType)
        {
            // TODO: Verify the LCID value.
            //return intlStr.GetUniStrRandAnsi(nMaxLength, bAbsolute, bValidOnly, CodeType, enuLCIDType, false);
            return _intlStr.GetRandStrLCID(nMaxLength, bAbsolute, bValidOnly, enuLCIDType, false);
        }

        // <doc>
        // <desc>
        //  Determines if the supplied unicode char is Valid.
        // </desc>
        // <param term="c">
        //  The character evaluate.
        // </param>
        // <retvalue>
        //  True if the character is printable; false otherwise.
        // </retvalue>
        // </doc>
        public virtual bool GetPrintable(char c)
        {
            String chrToCheck = c.ToString();
            return _intlStr.IsValidChar(chrToCheck);
        }

        // <doc>
        // <desc>
        //  Get a random UNICode char.  If the char doesn't appear you might
        //  need a different font (or it might not be printable).
        // </desc>
        // <retvalue>
        //  A random Unicode character.
        // </retvalue>
        // </doc>
        public virtual char GetChar()
        {
            char retVal = GetChar(false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Get a random UNICode char in the range that is optionally guaranteed
        //  to be printable.
        // </desc>
        // <param term="bValidOnly">
        //  If true the character returned is always valid character.
        // </param>
        // <retvalue>
        //  A random Unicode character that is optionally printable.
        // </retvalue>
        // </doc>
        public virtual char GetChar(bool bValidOnly)
        {
            short nMaxChars = 4;  //GetUniStrRandAnsi method returns nothing if the nMaxChars < 4. It doesn't make sense but we don't have any other choice right now.
            string strChar = _intlStr.GetString(nMaxChars, true, bValidOnly, false);
            LogValueIfLogging(strChar[0]);
            return strChar[0];
        }

        // <doc>
        // <desc>
        //  Get a random UNICode char that's not in the excluded string.  If the char
        //  doesn't appear you might need a different font (or it might not be printable).
        // </desc>
        // <retvalue>
        //  A random Unicode character that's not in excludedString.
        // </retvalue>
        // </doc>
        public virtual char GetNonexcludedChar(string excludedString)
        {
            const int MAXTRIES = 100;
            char randomChar;

            for (int i = 0; i < MAXTRIES; i++)
            {
                randomChar = this.GetChar(false);
                if (excludedString.IndexOf(randomChar) == -1)
                {
                    LogValueIfLogging(randomChar);
                    return randomChar;
                }
            }
            throw new ApplicationException("Exceeded " + MAXTRIES.ToString() + " tries to get character that's not in excluded string");
        }

        // <doc>
        // <desc>
        //  Get a random UNICode char that's not in the excluded string.  If the char
        //  doesn't appear you might need a different font (or it might not be printable).
        //  If bValidOnly is true, you should get a valid character for your character set.
        // </desc>
        // <retvalue>
        //  A random Unicode character that's not in excludedString.
        // </retvalue>
        // </doc>
        public virtual char GetNonexcludedChar(string excludedString, bool bValidOnly)
        {
            const int MAXTRIES = 100;
            string randomString;
            short nMaxChars = 4;  //GetUniStrRandAnsi method returns nothing if the nMaxChars < 4. It doesn't make sense but we don't have any other choice right now.

            for (int i = 0; i < MAXTRIES; i++)
            {
                randomString = _intlStr.GetString(nMaxChars, true, bValidOnly, false);
                if (excludedString.IndexOf(randomString[0]) == -1)
                {
                    LogValueIfLogging(randomString[0]);
                    return randomString[0];
                }
            }
            throw new ApplicationException("Exceeded " + MAXTRIES.ToString() + " tries to get character that's not in excluded string");
        }


        // <doc>
        // <desc>
        //  Creates a random string of a specified maximum length random ansi characters. Problematic and
        //  interesting characters are also included in the return string.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length the generated string can be.
        // </param>
        // <retvalue>
        //  A random string of characters.
        // </retvalue>
        // </doc>
        public virtual String GetString(int nMaxLength)
        {
            string retVal = this.GetString(1, nMaxLength, false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a random string of a specified maximum length random ansi characters. Problematic and
        //  interesting characters are also included in the return string.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length the generated string can be.
        // </param>
        // <param term="bValidOnly">
        //  This argument is no longer used and will eventually be ripped.
        // </param>
        // <retvalue>
        //  A random string of characters.
        // </retvalue>
        // </doc>
        public virtual String GetString(int nMaxLength, bool bValidOnly)
        {
            string retVal = this.GetString(1, nMaxLength, bValidOnly);
            LogValueIfLogging(retVal);
            return retVal;
        }


        // <doc>
        // <desc>
        //  Creates a random string of a specified maximum length random ansi characters. Problematic and
        //  interesting characters are also included in the return string.
        // </desc>
        // <param term="nMinLength">
        //  The minimum length the generated string can be.
        // </param>
        // <param term="nMaxLength">
        //  The maximum length the generated string can be.
        // </param>
        // <retvalue>
        //  A random string of characters.
        // </retvalue>
        // </doc>
        public virtual String GetString(int nMinLength, int nMaxLength)
        {
            string retVal = this.GetString(nMinLength, nMaxLength, false);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a random string of a specified maximum length random ansi characters. The string is like
        //  that returned by GetString (int nMaxLength, true), but with characters unacceptable to SendKeys
        //  stripped out.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length the generated string can be.
        // </param>
        // <retvalue>
        //  A random string of characters, excluding those with special meaning to SendKeys.
        // </retvalue>
        // </doc>
        public virtual String GetSendableString(int nMaxLength)
        {
            String s = this.GetString(1, nMaxLength, true);
            s = System.Text.RegularExpressions.Regex.Replace(s, "[\\+\\^\\(\\)\\[\\]\\{\\}%~]", "");
            LogValueIfLogging(s);
            return s;
        }

        //
        // Returns a random string of characters guaranteed to be valid on the current OS.  Basically,
        // the logic is "If we're on WinNT 4 or greater, call GetString().  Else (we're on Win9x), use
        // a drastically restricted character set (no intl chars) to generate a random string".
        //
        // NOTE: This is NOT a good method to use for getting random strings if you're concerned with
        //       testing international strings.  This is sort of a hack to get around the fact that
        //       Win9x sucks and doesn't properly round-trip some valid Unicode characters.
        //
        public virtual String GetValidString(int maxLen)
        {
            string retVal;
            if (!Utilities.IsWin9x)
            {
                retVal = GetString(maxLen);
                LogValueIfLogging(retVal);
                return retVal;
            }
            else
            {
                // Code copied from WFCTestLib.Util.IntlString (the old IntlStrings class)
                int len = GetRange(1, maxLen);
                char[] chars = new char[len];

                for (int i = 0; i < len; i++)
                    chars[i] = (char)GetRange(65, 91);  // Restricted char range for Win9x

                retVal = new string(chars);
                LogValueIfLogging(retVal);
                return retVal;
            }
        }

        // <doc>
        // <desc>
        //  Creates a random string of a specified maximum length random ansi characters. Problematic and
        //  interesting characters are also included in the return string. Optionally
        //  all characters in the string can be guaranteed to be valid once.
        // </desc>
        // <param term="nMaxLength">
        //  The maximum length for the generated string.
        // </param>
        // <param term="bValidOnly">
        //  This argument is no longer used and will eventually be ripped.
        // </param>
        // <retvalue>
        //  A string of random characters.
        // </retvalue>
        // </doc>
        public virtual String GetString(int iMinLength, int iMaxLength, bool bValidOnly)
        {
            int length = 0;
            String strText = "";

            // if the string length is <= zero, return an empty string
            if (iMaxLength <= 0)
            {
                LogValueIfLogging(string.Empty);
                return String.Empty;
            }

            if (iMinLength > iMaxLength)
                throw new ArgumentException("GetString: iMinLength > iMaxLength");

            // iMinLength cannot be less than zero
            if (iMinLength < 0)
                iMinLength = 0;

            length = this.GetRange(iMinLength, iMaxLength);

            // REVIEW: what is going on here?
            if (length > Int16.MaxValue)
                length = Int16.MaxValue;

            while (strText.Length < length)
                strText += _intlStr.GetString(length, true, true);

            // get all the interesting characters and include into the string.
            String strInterestingChars = _intlStr.GetTop20String(10, true, true);
            string retVal;

            // If the random length is smaller than the set of interesting chars, return a substring
            // of the interesting char set.
            if (length <= strInterestingChars.Length)
            {
                retVal = strInterestingChars.Substring(0, length);
                LogValueIfLogging(retVal);
                return retVal;
            }

            // insert interesting characters in various positions.
            foreach (Char c in strInterestingChars)
            {
                int i = this.GetRange(0, strText.Length);
                strText = strText.Insert(i, Convert.ToString(c));
            }

            retVal = strText.Substring(0, length);
            LogValueIfLogging(retVal);
            return retVal;
        }

        public virtual String GetStringWithProbChars(int iMaxLength, bool bValidOnly)
        {
            int iRandNum = 0;
            String strText;

            if (iMaxLength <= 0)  // If the string length is <= zero, return an empty string
                return String.Empty;

            iRandNum = this.GetRange(1, iMaxLength);

            if (iMaxLength > Int16.MaxValue)
                iMaxLength = Int16.MaxValue;
            else
                iMaxLength = iRandNum;

            strText = GetString(1, iMaxLength, true);

            //Get all the interesting chracters and inlcude into the string.

            String strProblematicChars = _intlStr.GetProbCharString(10, true, true);
            if (iMaxLength <= strProblematicChars.Length)
                return strProblematicChars.Substring(0, iMaxLength);

            //Insert Interesting characters in various positions.
            foreach (Char c in strProblematicChars)
            {
                iRandNum = this.GetRange(0, strText.Length);
                strText = strText.Insert(iRandNum, Convert.ToString(c));
            }

            if (strProblematicChars.Length > iMaxLength)
                return strText.Substring(0, iMaxLength);
            else
                return strText;
        }

        // <doc>
        // <desc>
        //  Calls RandomUtil.GetString()
        // </desc>
        // <seealso member="GetString"/>
        // <deprecated/>
        // </doc>
        public virtual String GetText(int nMaxLength)
        {
            return this.GetString(1, nMaxLength, true);
        }

        //
        // overload that doesn't require minStrLen
        //
        public string[] GetUniqueStrings(int numStrings, int maxStrLen)
        {
            return GetUniqueStrings(numStrings, 1, maxStrLen);
        }

        //
        // The new IntlStrings makes it such that GetString() has a fairly high chance of
        // returning duplicate strings given a large enough sample size.  This method will
        // return an array of random strings that guarantees that each is unique in the
        // array.
        //
        public string[] GetUniqueStrings(int numStrings, int minStrLen, int maxStrLen)
        {
            StringTable table = new StringTable(numStrings);

            for (int i = 0; i < numStrings; i++)
            {
                string s = GetString(minStrLen, maxStrLen);
                int numLoops = 0;

                while (table.Contains(s))
                {
                    if (numLoops == 100)
                        throw new ApplicationException("Got 100 consecutive duplicate strings in GetUniqueStrings()--something wrong here, please investigate");

                    s = GetString(minStrLen, maxStrLen);
                    ++numLoops;
                }

                table.Add(s);
            }

            return table.ToArray();
        }

        // <doc>
        // <desc>
        //  Calculates a random number in between nMin and nMax inclusive.
        // </desc>
        // <param term="nMin">
        //  The minimuim value.
        // </param>
        // <param term="nMax">
        //  The maximum value.
        // </param>
        // <retvalue>
        //  A random number between the specified minimum and maximum values
        // </retvalue>
        // <seealso class="IntlString" member="GetRange"/>
        // </doc>
        public int GetRange(int nMin, int nMax)
        {
            // Swap max and min if min > max
            if (nMin > nMax)
            {
                int nTemp = nMin;
                nMin = nMax;
                nMax = nTemp;
            }

            // Add 1 to max because rand.Next() returns min <= value < max...
            // Uh, don't do this if Max is Int32.MaxValue. :P
            if (nMax != Int32.MaxValue)
                ++nMax;

            int retVal = s_rand.Next(nMin, nMax);
            LogValueIfLogging(retVal);
            return retVal;
        }

        public T Choice<T>(params T[] args)
        {
            if (null == args || args.Length < 2)
            { throw new ArgumentException("Must be more than one parameter to choose from", "args"); }
            return args[GetRange(0, args.Length - 1)];
        }

        // <doc>
        // <desc>
        //  Creates a random point on the primary monitor
        // </desc>
        // <retvalue>
        //  A point in the primary monitor
        // </retvalue>
        // </doc>
        public virtual Point GetScreenPoint()
        {
            //TODO: make this thing work for multi monitors!

            int nX = GetRange(0, Screen.PrimaryScreen.Bounds.Width);
            int nY = GetRange(0, Screen.PrimaryScreen.Bounds.Height);

            Point pRet = new Point(nX, nY);
            LogValueIfLogging(pRet);
            return pRet;
        }

        // <doc>
        // <desc>
        //  Creates a random font with random attributes.
        // </desc>
        // <retvalue>
        //  A random font.
        // </retvalue>
        // </doc>
        public virtual Font GetFont()
        {
            // Families is a static array of FontFamilies
            // installed on the current system
            FontFamily[] families = FontFamily.Families;
            Font font = null;

            for (int trial = 0; trial < maxAttemptsToGetFont && font == null; trial++)
            {
                try
                {
                    // Randomly choose an entry in the array
                    FontFamily family = (FontFamily)GetArrayElement(families);

                    float emSize = GetFloat(0, 200);
                    FontStyle style = (FontStyle)GetEnumValue(typeof(FontStyle));

                    // Workaround for bug #27605 (by design).  Some fonts don't support a regular style
                    if (family.Name == "Monotype Corsiva")
                        style |= FontStyle.Italic;
                    else if (family.Name == "Aharoni")
                        style |= FontStyle.Bold;
                    else if (family.Name == "Segoe UI")
                        style |= FontStyle.Bold;

                    // Display isn't a valid unit for Fonts.
                    GraphicsUnit unit = (GraphicsUnit)GetDifferentEnumValue(typeof(GraphicsUnit), (int)GraphicsUnit.Display);
                    font = new Font(family, emSize, style, unit);
                }
                catch (ArgumentException)
                {
                    // This is to cover other special case fonts that don't support all styles, like Harlow Solid Italic
                    // not supporting underline
                }
            }
            if (font == null)
                throw new InvalidOperationException("Unable to obtain a valid Font!");
            LogValueIfLogging(font);
            return font;
        }

        public virtual Decimal GetDecimal()
        {
            //Get a random double and convert to decimal type
            Decimal retVal = new Decimal(GetDouble());
            LogValueIfLogging(retVal);
            return retVal;
        }

        public virtual Decimal GetDecimal(Decimal minimum, Decimal maximum)
        {
            // Get a random double, remap to between provided minimum and maximum
            Decimal retVal = (((Decimal)s_rand.NextDouble()) * (maximum - minimum) + minimum);
            LogValueIfLogging(retVal);
            return retVal;
        }

        // <doc>
        // <desc>
        //  Creates a new random number generator based on the supplied seed.
        // </desc>
        // </doc>
        public virtual void SeedRandomGenerator(Int32 iSeed)
        {
            InitializeRandoms(iSeed);
        }

        // <doc>
        // <desc>
        //  Creates a polygonal region of the type and size specified. This region
        //  is in the shape of a five-pointed star.
        // </desc>
        // <param term="fillMode">
        //  One of the values from PolyFillModeEnum.
        // </param>
        // <param term="size">
        //  The width and height of the desired region
        // </param>
        // </doc>
        private static Region polygonRegion(FillMode fillMode, Size size)
        {
            Point[] pa = new Point[5];
            pa[0] = new Point(size.Width / 6, size.Height);
            pa[1] = new Point(size.Width / 2, 0);
            pa[2] = new Point(5 * size.Width / 6, size.Height);
            pa[3] = new Point(0, size.Height / 3);
            pa[4] = new Point(size.Width, size.Height / 3);
            GraphicsPath gp = new GraphicsPath(fillMode);
            gp.AddLines(pa);
            return new Region(gp);
        }

        [
        DllImport("gdi32", EntryPoint = "CreateFont", CharSet = System.Runtime.InteropServices.CharSet.Auto),
        SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)
        ]
        private static extern int IntCreateFont(int nHeight, int nWidth,
                                                int nEscapement, int nOrientation, int fnWeight, int fdwItalic,
                                                int fdwUnderline, int fdwStrikeOut, int fdwCharSet,
                                                int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality,
                                                int fdwPitchAndFamily, String lpszFace);

        // <doc>
        // <desc>
        //  Creates a region that is in the shape of an uppercase "A".
        // </desc>
        // <param term="size">
        //  The width and height of the region to be created.
        // </param>
        // </doc>
        [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        private Region pathRegion(Size size)
        {
            Font fnt2 = this.GetFont();
            Font fnt = new Font(fnt2.FontFamily, size.Height, fnt2.Style, GraphicsUnit.Pixel);
            fnt2.Dispose();


            /*
            * Create a Graphics object based on our window and select our font into it
            */
            GraphicsPath gp = new GraphicsPath();

            //gp.AddString("A", fnt, 0, 0);
            gp.AddString("A", new FontFamily(fnt.Name), (int)FontStyle.Regular,
                         1.5f, new PointF(0, 0), new StringFormat(0, 0));
            fnt.Dispose();

            return new Region(gp);
        }

        // <doc>
        // <desc>
        //  Gets a random region of the size specified.
        // </desc>
        // <param term="size">
        //  The width and height of the region to be created.
        // </param>
        // </doc>
        public Region GetRegion(Size size)
        {
            return GetRegion((RegionType)GetEnumValue(typeof(RegionType)), size);
        }

        // <doc>
        // <desc>
        //  Gets a region of the type and size specified.
        // </desc>
        // <param term="rt">
        //  The type of the region to be created
        // </param>
        // <param term="size">
        //  The width and height of the region to be created.
        // </param>
        // </doc>
        public Region GetRegion(RegionType rt, Size size)
        {
            switch (rt)
            {
                case RegionType.None:
                    return null;

                case RegionType.Empty:
                    return new Region();

                case RegionType.Rect:
                    return new Region(new Rectangle(size.Width / 4, size.Height / 4, size.Width / 2, size.Height / 2));

                case RegionType.RoundRect:
                    // TODO: no support for rounded rectangle region yet. Use code from Rect instead.
#if false
                    return Region.CreateRoundedRectangular(size.Width / 4, size.Height / 4, size.Width / 2, size.Height / 2, size.Width/12);
#else
                    return new Region(new Rectangle(size.Width / 4, size.Height / 4, size.Width / 2, size.Height / 2));
#endif

                case RegionType.Ellipse:
                    GraphicsPath gpe = new GraphicsPath();
                    gpe.AddEllipse(size.Width / 4, size.Height / 4, size.Width / 2, size.Height / 2);
                    return new Region(gpe);

                case RegionType.PolygonWinding:
                    return polygonRegion(FillMode.Winding, size);

                case RegionType.PolygonAlternate:
                    return polygonRegion(FillMode.Alternate, size);

                case RegionType.Path:
                    return pathRegion(size);
            }

            return null;
        }

        // <doc>
        // <desc>
        //  Retrieves a randomly selected cursor
        // </desc>
        // <retvalue>
        //  A WFC Cursor object
        // </retvalue>
        // </doc>
        public Cursor GetCursor()
        {
            Cursor[] ca = new Cursor[]
                           {Cursors.AppStarting, Cursors.Arrow, Cursors.Cross, Cursors.Default,
                           Cursors.IBeam, Cursors.No, Cursors.SizeAll, Cursors.SizeNESW,
                           Cursors.SizeNS, Cursors.SizeNWSE, Cursors.SizeWE, Cursors.UpArrow,
                           Cursors.WaitCursor, Cursors.Help, Cursors.HSplit, Cursors.VSplit,
                           Cursors.NoMove2D, Cursors.NoMoveHoriz, Cursors.NoMoveVert,
                           Cursors.PanEast, Cursors.PanNE, Cursors.PanNorth, Cursors.PanNW,
                           Cursors.PanWest, Cursors.PanSW, Cursors.PanSouth, Cursors.PanSE,
                           Cursors.Hand};

            return ca[GetRange(0, ca.Length - 1)];
        }

        // <doc>
        // <desc>
        //  Creates a random matrix.
        // </desc>
        // </doc>
        public Matrix GetMatrix()
        {
            return (new Matrix(
                GetFloat(int.MinValue, int.MaxValue),
                GetFloat(int.MinValue, int.MaxValue),
                GetFloat(int.MinValue, int.MaxValue),
                GetFloat(int.MinValue, int.MaxValue),
                GetFloat(int.MinValue, int.MaxValue),
                GetFloat(int.MinValue, int.MaxValue)));
        }

        // <doc>
        // <desc>
        //  Returns a randomly-selected IComponent, which optionally may be a Control (50/50 chance).  Not a complete list of IComponents.
        // </desc>
        // </doc>
        public IComponent GetIComponent(bool mayBeControl)
        {
            if (mayBeControl && GetBoolean())
            {
                Control c = GetControl();
                LogValueIfLogging(c.GetType().Name);
                return c;
            }
            IComponent ic = GetIComponent();
            LogValueIfLogging(ic.GetType().Name);
            return ic;
        }

        // <doc>
        // <desc>
        //  Returns a randomly-selected IComponent, which is never a control.  Not a complete list of IComponents.
        // </desc>
        // </doc>
        public IComponent GetIComponent()
        {
            Type[] ic = new Type[]
            {
                typeof(PageSetupDialog), typeof(PrintDialog), typeof(PrintPreviewDialog), typeof(SaveFileDialog),
                typeof(ToolTip),
                typeof(System.Data.DataSet), typeof(System.Data.DataView),
                typeof(System.Data.Odbc.OdbcCommand), typeof(System.Data.OleDb.OleDbConnection),
                typeof(System.Data.SqlClient.SqlDataAdapter),
                typeof(System.Diagnostics.Process), typeof(System.DirectoryServices.DirectorySearcher),
                typeof(System.Drawing.Printing.PrintDocument),
                typeof(System.IO.FileSystemWatcher),
                typeof(System.Timers.Timer)
            };
            Type t = (Type)GetArrayElement(ic);
            if (t == typeof(ThreadExceptionDialog))
            {
                LogValueIfLogging("ThreadExceptionDialog");
                return new ThreadExceptionDialog(new Exception());
            }
            LogValueIfLogging(t.Name);
            return (IComponent)Activator.CreateInstance(t);
        }

        // <doc>
        // <desc>
        //  Returns a randomly-selected control
        // </desc>
        // </doc>
        public Control GetControl()
        {
            Type[] types = {
				typeof(Button), typeof(CheckBox), typeof(CheckedListBox), typeof(ComboBox),
				typeof(DateTimePicker), typeof(GroupBox), typeof(Label), typeof(Label), typeof(LinkLabel),
				typeof(ListBox), typeof(ListView), typeof(MonthCalendar), typeof(Panel), typeof(PictureBox),
				typeof(ProgressBar), typeof(RadioButton), typeof(HScrollBar), typeof(VScrollBar), typeof(Splitter),
				typeof(TabControl), typeof(TrackBar), typeof(TreeView),
				typeof(NumericUpDown), typeof(DomainUpDown), typeof(PrintPreviewControl), typeof(PropertyGrid),
				typeof(DataGridView), typeof(SplitContainer)
			};
            Type t = (Type)GetArrayElement(types);
            Control c = (Control)Activator.CreateInstance(t);
            LogValueIfLogging(c.GetType().Name);
            return c;
        }

        // <doc>
        // <desc>
        //  Retrieves a random CultureInfo.
        // </desc>
        // <retvalue>
        //  A random CultureInfo.
        // </retvalue>
        // </doc>
        public virtual CultureInfo GetCultureInfo() { return GetCultureInfo(true); }
        public virtual CultureInfo GetCultureInfo(bool allowNeutral)
        {
            CultureInfo culture;
            if (Utilities.IsVista)
            {

                culture = new CultureInfo(GetArrayElement(_cultureFormatsVista).ToString());
            }
            else
            {
                culture = new CultureInfo(GetArrayElement(_cultureFormats).ToString());
            }

            // Neutral cultures can't be used
            if (!allowNeutral)
            {
                while (culture.IsNeutralCulture)
                {
                    if (Utilities.IsVista)
                    {

                        culture = new CultureInfo(GetArrayElement(_cultureFormatsVista).ToString());
                    }
                    else
                    {
                        culture = new CultureInfo(GetArrayElement(_cultureFormats).ToString());
                    }
                }
            }

            return culture;
        }

        // <doc>
        // <desc>
        //  Retrieves a random IFormatProvider.
        // </desc>
        // <retvalue>
        //  A random IFormatProvider.
        // </retvalue>
        // </doc>
        public virtual IFormatProvider GetIFormatProvider()
        {
            IFormatProvider ret;
            CultureInfo culture = GetCultureInfo(false);

            // Select a type of IFormatProvider
            int type = s_rand.Next() % 3;
            switch (type)
            {
                case 1:
                    // DateTimeInfo
                    ret = culture.DateTimeFormat;
                    break;
                case 2:
                    // NumberInfo
                    ret = culture.NumberFormat;
                    break;
                default:
                    // CultureInfo
                    ret = culture;
                    break;
            }
            return ret;
        }

        // <doc>
        // <desc>
        //  Retrieves a random GraphicsPath using a combination of random shapes
        // </desc>
        // <retvalue>
        //  A random GraphicsPath.
        // </retvalue>
        // </doc>
        public virtual GraphicsPath GetGraphicsPath()
        {
            GraphicsPath result = new GraphicsPath();
            int numShapes = this.GetRange(0, 255);

            result.FillMode = this.GetEnumValue<FillMode>();

            for (int i = 0; i < numShapes; i++)
            {
                switch (this.GetRange(0, 4))
                {
                    case 0: result.AddEllipse(this.GetRectangle()); break;
                    case 1: result.AddEllipse(this.GetRectangleF()); break;
                    case 2:
                        Rectangle r = this.GetRectangle();
                        result.AddLine(r.Left, r.Top, r.Right, r.Bottom);
                        break;
                    case 3: result.AddRectangle(this.GetRectangle()); break;
                    case 4: result.AddRectangle(this.GetRectangleF()); break;
                }
            }

            return result;
        }

        // <doc>
        // <desc>
        //  Retrieves a random GraphicsPath suitable for use as a CustomLineCap.
        //  Due to limitations of CustomLineCap, it has to be on the negative side.
        //  The people in GDI+ can't code for shit.
        // </desc>
        // <retvalue>
        //  A random GraphicsPath.
        // </retvalue>
        // </doc>
        public virtual GraphicsPath GetLineCapGraphicsPath()
        {
            GraphicsPath result = new GraphicsPath();
            result.AddRectangle(new Rectangle(this.GetRange(-255, 0), this.GetRange(-255, 0), this.GetRange(0, 255), this.GetRange(0, 255)));
            return result;
        }

        // <doc>
        // <desc>
        //  Returns a randomly-selected VisualStyleElement
        // </desc>
        // </doc>
        public VisualStyleElement GetVisualStyleElement() { return GetVisualStyleElement(true); }
        public VisualStyleElement GetVisualStyleElement(bool validOnly)
        {
            GenerateVisualStyleElementTable(typeof(VisualStyleElement));
            if (s_elementTable == null || s_elementTable.Count == 0)
            { return VisualStyleElement.Button.PushButton.Default; } // give them something

            int index = this.GetRange(0, s_elementTable.Count - 1);
            VisualStyleElement element = (VisualStyleElement)s_elementTable[index].Invoke(null, null);

            if (validOnly)
            {
                while (!VisualStyleRenderer.IsElementDefined(element))
                {
                    index = this.GetRange(0, s_elementTable.Count - 1);
                    element = (VisualStyleElement)s_elementTable[index].Invoke(null, null);
                }
            }

            return element;
        }

        // <doc>
        // <desc>
        //  Returns a randomly-selected VisualStyleElement thats located under the type provided
        //
        //	Ex. GetVisualStyleElement(typeof(VisualStyleElement.Button.PushButton)) would return
        //		either Button.PushButton.Normal, Button.PushButton.Hot, Button.PushButton.Pressed,
        //		Button.PushButton.Disabled or Button.PushButton.Default
        // </desc>
        // </doc>
        public VisualStyleElement GetVisualStyleElement(Type root) { return GetVisualStyleElement(root, true); }
        public VisualStyleElement GetVisualStyleElement(Type root, bool validOnly)
        {
            GenerateVisualStyleElementTable(root);
            if (s_elementTable == null || s_elementTable.Count == 0) { return null; }

            int index = this.GetRange(0, s_elementTable.Count - 1);
            VisualStyleElement element = (VisualStyleElement)s_elementTable[index].Invoke(null, null);

            if (validOnly)
            {
                while (!VisualStyleRenderer.IsElementDefined(element))
                {
                    index = this.GetRange(0, s_elementTable.Count - 1);
                    element = (VisualStyleElement)s_elementTable[index].Invoke(null, null);
                }
            }

            return element;
        }

        private static List<MethodInfo> s_elementTable = null;
        private static Type s_rootOfTable = null;

        private void GenerateVisualStyleElementTable(Type root)
        {
            if (s_elementTable == null || s_rootOfTable != root)
            {
                s_rootOfTable = root;
                s_elementTable = new List<MethodInfo>();
                TypeWalker walker = new TypeWalker(s_rootOfTable);
                walker.NestedTypes += new TypeWalker.NestedTypeCallback(GetVisualStyleElement_OnNestedType);
                walker.Walk();
            }
        }

        private static void GetVisualStyleElement_OnNestedType(Type info)
        {
            TypeWalker walker = new TypeWalker(info);
            walker.NestedTypes += new TypeWalker.NestedTypeCallback(GetVisualStyleElement_OnNestedType);
            walker.Walk();

            walker.NestedTypes = null;
            walker.Methods += new TypeWalker.MethodCallback(GetVisualStyleElement_OnMethod);
            walker.IgnorePropertyMethods = false;
            walker.Walk();
        }

        private static void GetVisualStyleElement_OnMethod(MethodInfo info)
        {
            if (info.ReturnType != typeof(VisualStyleElement)) { return; }
            s_elementTable.Add(info);
        }
    }
}
