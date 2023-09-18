// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data about input locale data inovled in testing.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 5 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Common/Library/Data/StringData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Text;

    #endregion Namespaces.

    /// <summary>Indicates the language that is used.</summary>
    /// <remarks>
    /// To identify the language that is used in a country or
    /// region you must combine the primary language with a sublanguage
    /// identifier to form language identifiers.
    /// </remarks>
    public sealed class PrimaryLanguageIdentifierData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private PrimaryLanguageIdentifierData() { }

        #endregion Constructors.


        #region Public properties.

        /// <summary>Numeric identifier for the language.</summary>
        public int Identifier
        {
            get { return _identifier; }
        }

        /// <summary>System symbol for the language.</summary>
        public string Symbol
        {
            get { return _symbol; }
        }

        /// <summary>Name of the language.</summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// System-defined primary language identifiers
        /// </summary>
        public static PrimaryLanguageIdentifierData[] Values = new PrimaryLanguageIdentifierData[] {
            ForLanguage(0x00, "LANG_NEUTRAL", "Neutral"),
            ForLanguage(0x01, "LANG_ARABIC", "Arabic"),
            ForLanguage(0x02, "LANG_BULGARIAN", "Bulgarian"),
            ForLanguage(0x03, "LANG_CATALAN", "Catalan "),
            ForLanguage(0x04, "LANG_CHINESE", "Chinese"),
            ForLanguage(0x05, "LANG_CZECH", "Czech"),
            ForLanguage(0x06, "LANG_DANISH", "Danish"),
            ForLanguage(0x07, "LANG_GERMAN", "German"),
            ForLanguage(0x08, "LANG_GREEK", "Greek"),
            ForLanguage(0x09, "LANG_ENGLISH", "English "),
            ForLanguage(0x0a, "LANG_SPANISH", "Spanish "),
            ForLanguage(0x0b, "LANG_FINNISH", "Finnish"),
            ForLanguage(0x0c, "LANG_FRENCH", "French"),
            ForLanguage(0x0d, "LANG_HEBREW", "Hebrew"),
            ForLanguage(0x0e, "LANG_HUNGARIAN", "Hungarian"),
            ForLanguage(0x0f, "LANG_ICELANDIC", "Icelandic"),
            ForLanguage(0x10, "LANG_ITALIAN", "Italian "),
            ForLanguage(0x11, "LANG_JAPANESE", "Japanese"),
            ForLanguage(0x12, "LANG_KOREAN", "Korean"),
            ForLanguage(0x13, "LANG_DUTCH", "Dutch"),
            ForLanguage(0x14, "LANG_NORWEGIAN", "Norwegian"),
            ForLanguage(0x15, "LANG_POLISH", "Polish"),
            ForLanguage(0x16, "LANG_PORTUGUESE", "Portuguese"),
            ForLanguage(0x18, "LANG_ROMANIAN", "Romanian"),
            ForLanguage(0x19, "LANG_RUSSIAN", "Russian"),
            ForLanguage(0x1a, "LANG_CROATIAN", "Croatian"),
            ForLanguage(0x1a, "LANG_SERBIAN", "Serbian"),
            ForLanguage(0x1b, "LANG_SLOVAK", "Slovak"),
            ForLanguage(0x1c, "LANG_ALBANIAN", "Albanian"),
            ForLanguage(0x1d, "LANG_SWEDISH", "Swedish "),
            ForLanguage(0x1e, "LANG_THAI", "Thai"),
            ForLanguage(0x1f, "LANG_TURKISH", "Turkish "),
            ForLanguage(0x20, "LANG_URDU", "Urdu"),
            ForLanguage(0x21, "LANG_INDONESIAN", "Indonesian"),
            ForLanguage(0x22, "LANG_UKRAINIAN", "Ukrainian"),
            ForLanguage(0x23, "LANG_BELARUSIAN", "Belarusian"),
            ForLanguage(0x24, "LANG_SLOVENIAN", "Slovenian"),
            ForLanguage(0x25, "LANG_ESTONIAN", "Estonian"),
            ForLanguage(0x26, "LANG_LATVIAN", "Latvian"),
            ForLanguage(0x27, "LANG_LITHUANIAN", "Lithuanian"),
            ForLanguage(0x29, "LANG_FARSI", "Farsi"),
            ForLanguage(0x2a, "LANG_VIETNAMESE", "Vietnamese"),
            ForLanguage(0x2b, "LANG_ARMENIAN", "Armenian"),
            ForLanguage(0x2c, "LANG_AZERI", "Azeri"),
            ForLanguage(0x2d, "LANG_BASQUE", "Basque"),
            ForLanguage(0x2f, "LANG_MACEDONIAN", "FYRO Macedonian"),
            ForLanguage(0x36, "LANG_AFRIKAANS", "Afrikaans"),
            ForLanguage(0x37, "LANG_GEORGIAN", "Georgian"),
            ForLanguage(0x38, "LANG_FAEROESE", "Faeroese"),
            ForLanguage(0x39, "LANG_HINDI", "Hindi"),
            ForLanguage(0x3e, "LANG_MALAY", "Malay"),
            ForLanguage(0x3f, "LANG_KAZAK", "Kazak"),
            ForLanguage(0x40, "LANG_KYRGYZ", "Kyrgyz"),
            ForLanguage(0x41, "LANG_SWAHILI", "Swahili"),
            ForLanguage(0x43, "LANG_UZBEK", "Uzbek"),
            ForLanguage(0x44, "LANG_TATAR", "Tatar"),
            ForLanguage(0x45, "LANG_BENGALI", "Not supported."),
            ForLanguage(0x46, "LANG_PUNJABI", "Punjabi"),
            ForLanguage(0x47, "LANG_GUJARATI", "Gujarati"),
            ForLanguage(0x48, "LANG_ORIYA", "Not supported."),
            ForLanguage(0x49, "LANG_TAMIL", "Tamil"),
            ForLanguage(0x4a, "LANG_TELUGU", "Telugu"),
            ForLanguage(0x4b, "LANG_KANNADA", "Kannada"),
            ForLanguage(0x4c, "LANG_MALAYALAM", "Not supported."),
            ForLanguage(0x4d, "LANG_ASSAMESE", "Not supported."),
            ForLanguage(0x4e, "LANG_MARATHI", "Marathi"),
            ForLanguage(0x4f, "LANG_SANSKRIT", "Sanskrit"),
            ForLanguage(0x50, "LANG_MONGOLIAN", "Mongolian"),
            ForLanguage(0x56, "LANG_GALICIAN", "Galician"),
            ForLanguage(0x57, "LANG_KONKANI", "Konkani"),
            ForLanguage(0x58, "LANG_MANIPURI", "Not supported."),
            ForLanguage(0x59, "LANG_SINDHI", "Not supported."),
            ForLanguage(0x5a, "LANG_SYRIAC", "Syriac"),
            ForLanguage(0x60, "LANG_KASHMIRI", "Not supported."),
            ForLanguage(0x61, "LANG_NEPALI", "Not supported."),
            ForLanguage(0x65, "LANG_DIVEHI", "Divehi"),
            ForLanguage(0x7f, "LANG_INVARIANT", "Invariant"),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new PrimaryLanguageIdentifierData instance for
        /// the given values.
        /// </summary>
        private static PrimaryLanguageIdentifierData ForLanguage(int identifier,
            string symbol, string language)
        {
            PrimaryLanguageIdentifierData result;

            result = new PrimaryLanguageIdentifierData();
            result._identifier = identifier;
            result._symbol = symbol;
            result._language = language;

            return result;
        }

        #endregion Private methods.


        #region Private fields.

        private int _identifier;
        private string _symbol;
        private string _language;

        #endregion Private fields.
    }


    /// <summary>Indicates a regional sublanguage.</summary>
    /// <remarks>
    /// The name immediately following SUBLANG_ indicates the primary
    /// language ID that is used with the sublanguage ID to form a
    /// valid language ID.
    /// </remarks>
    public class SubLanguageIdentifierData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private SubLanguageIdentifierData() { }

        #endregion Constructors.

        #region Public properties.

        /// <summary>Numeric identifier for the language.</summary>
        public int Identifier
        {
            get { return _identifier; }
        }

        /// <summary>System symbol for the language.</summary>
        public string Symbol
        {
            get { return _symbol; }
        }

        /// <summary>Name of the language.</summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// System-defined sublanguage identifiers
        /// </summary>
        public static SubLanguageIdentifierData[] Values = new SubLanguageIdentifierData[] {
            ForLanguage(0x00, "SUBLANG_NEUTRAL", "Language neutral"),
            ForLanguage(0x01, "SUBLANG_DEFAULT", "User Default"),
            ForLanguage(0x02, "SUBLANG_SYS_DEFAULT", "System Default"),
            ForLanguage(0x01, "SUBLANG_ARABIC_SAUDI_ARABIA", "Arabic (Saudi Arabia)"),
            ForLanguage(0x02, "SUBLANG_ARABIC_IRAQ", "Arabic (Iraq)"),
            ForLanguage(0x03, "SUBLANG_ARABIC_EGYPT", "Arabic (Egypt)"),
            ForLanguage(0x04, "SUBLANG_ARABIC_LIBYA", "Arabic (Libya)"),
            ForLanguage(0x05, "SUBLANG_ARABIC_ALGERIA", "Arabic (Algeria)"),
            ForLanguage(0x06, "SUBLANG_ARABIC_MOROCCO", "Arabic (Morocco)"),
            ForLanguage(0x07, "SUBLANG_ARABIC_TUNISIA", "Arabic (Tunisia)"),
            ForLanguage(0x08, "SUBLANG_ARABIC_OMAN", "Arabic (Oman)"),
            ForLanguage(0x09, "SUBLANG_ARABIC_YEMEN", "Arabic (Yemen)"),
            ForLanguage(0x0a, "SUBLANG_ARABIC_SYRIA", "Arabic (Syria)"),
            ForLanguage(0x0b, "SUBLANG_ARABIC_JORDAN", "Arabic (Jordan)"),
            ForLanguage(0x0c, "SUBLANG_ARABIC_LEBANON", "Arabic (Lebanon)"),
            ForLanguage(0x0d, "SUBLANG_ARABIC_KUWAIT", "Arabic (Kuwait)"),
            ForLanguage(0x0e, "SUBLANG_ARABIC_UAE", "Arabic (U.A.E.)"),
            ForLanguage(0x0f, "SUBLANG_ARABIC_BAHRAIN", "Arabic (Bahrain)"),
            ForLanguage(0x10, "SUBLANG_ARABIC_QATAR", "Arabic (Qatar)"),
            ForLanguage(0x01, "SUBLANG_AZERI_LATIN", "Azeri (Latin)"),
            ForLanguage(0x02, "SUBLANG_AZERI_CYRILLIC", "Azeri (Cyrillic)"),
            ForLanguage(0x01, "SUBLANG_CHINESE_TRADITIONAL", "Chinese (Traditional)"),
            ForLanguage(0x02, "SUBLANG_CHINESE_SIMPLIFIED", "Chinese (Simplified)"),
            ForLanguage(0x03, "SUBLANG_CHINESE_HONGKONG", "Chinese (Hong Kong SAR, PRC)"),
            ForLanguage(0x04, "SUBLANG_CHINESE_SINGAPORE", "Chinese (Singapore)"),
            ForLanguage(0x05, "SUBLANG_CHINESE_MACAU", "Chinese (Macao SAR)"),
            ForLanguage(0x01, "SUBLANG_DUTCH", "Dutch"),
            ForLanguage(0x02, "SUBLANG_DUTCH_BELGIAN", "Dutch (Belgian) "),
            ForLanguage(0x01, "SUBLANG_ENGLISH_US", "English (US)"),
            ForLanguage(0x02, "SUBLANG_ENGLISH_UK", "English (UK)"),
            ForLanguage(0x03, "SUBLANG_ENGLISH_AUS", "English (Australian)"),
            ForLanguage(0x04, "SUBLANG_ENGLISH_CAN", "English (Canadian)"),
            ForLanguage(0x05, "SUBLANG_ENGLISH_NZ", "English (New Zealand)"),
            ForLanguage(0x06, "SUBLANG_ENGLISH_EIRE", "English (Ireland)"),
            ForLanguage(0x07, "SUBLANG_ENGLISH_SOUTH_AFRICA", "English (South Africa)"),
            ForLanguage(0x08, "SUBLANG_ENGLISH_JAMAICA", "English (Jamaica)"),
            ForLanguage(0x09, "SUBLANG_ENGLISH_CARIBBEAN", "English (Caribbean)"),
            ForLanguage(0x0a, "SUBLANG_ENGLISH_BELIZE", "English (Belize)"),
            ForLanguage(0x0b, "SUBLANG_ENGLISH_TRINIDAD", "English (Trinidad)"),
            ForLanguage(0x0c, "SUBLANG_ENGLISH_ZIMBABWE", "English (Zimbabwe)"),
            ForLanguage(0x0d, "SUBLANG_ENGLISH_PHILIPPINES", "English (Philippines)"),
            ForLanguage(0x01, "SUBLANG_FRENCH", "French"),
            ForLanguage(0x02, "SUBLANG_FRENCH_BELGIAN", "French (Belgian)"),
            ForLanguage(0x03, "SUBLANG_FRENCH_CANADIAN", "French (Canadian)"),
            ForLanguage(0x04, "SUBLANG_FRENCH_SWISS", "French (Swiss)"),
            ForLanguage(0x05, "SUBLANG_FRENCH_LUXEMBOURG", "French (Luxembourg)"),
            ForLanguage(0x06, "SUBLANG_FRENCH_MONACO", "French (Monaco)"),
            ForLanguage(0x01, "SUBLANG_GERMAN", "German"),
            ForLanguage(0x02, "SUBLANG_GERMAN_SWISS", "German (Swiss)"),
            ForLanguage(0x03, "SUBLANG_GERMAN_AUSTRIAN", "German (Austrian)"),
            ForLanguage(0x04, "SUBLANG_GERMAN_LUXEMBOURG", "German (Luxembourg)"),
            ForLanguage(0x05, "SUBLANG_GERMAN_LIECHTENSTEIN", "German (Liechtenstein)"),
            ForLanguage(0x01, "SUBLANG_ITALIAN", "Italian"),
            ForLanguage(0x02, "SUBLANG_ITALIAN_SWISS", "Italian (Swiss)"),
            ForLanguage(0x01, "SUBLANG_KOREAN", "Korean"),
            ForLanguage(0x01, "SUBLANG_LITHUANIAN", "Lithuanian"),
            ForLanguage(0x01, "SUBLANG_MALAY_MALAYSIA", "Malay (Malaysia)"),
            ForLanguage(0x02, "SUBLANG_MALAY_BRUNEI_DARUSSALAM", "Malay (Brunei Darassalam)"),
            ForLanguage(0x01, "SUBLANG_NORWEGIAN_BOKMAL", "Norwegian (Bokmal)"),
            ForLanguage(0x02, "SUBLANG_NORWEGIAN_NYNORSK", "Norwegian (Nynorsk)"),
            ForLanguage(0x01, "SUBLANG_PORTUGUESE_BRAZILIAN", "Portuguese (Brazil)"),
            ForLanguage(0x02, "SUBLANG_PORTUGUESE", "Portuguese (Portugal)"),
            ForLanguage(0x02, "SUBLANG_SERBIAN_LATIN", "Serbian (Latin)"),
            ForLanguage(0x03, "SUBLANG_SERBIAN_CYRILLIC", "Serbian (Cyrillic)"),
            ForLanguage(0x01, "SUBLANG_SPANISH", "Spanish (Castilian)"),
            ForLanguage(0x02, "SUBLANG_SPANISH_MEXICAN", "Spanish (Mexican)"),
            ForLanguage(0x03, "SUBLANG_SPANISH_MODERN", "Spanish (Spain)"),
            ForLanguage(0x04, "SUBLANG_SPANISH_GUATEMALA", "Spanish (Guatemala)"),
            ForLanguage(0x05, "SUBLANG_SPANISH_COSTA_RICA", "Spanish (Costa Rica)"),
            ForLanguage(0x06, "SUBLANG_SPANISH_PANAMA", "Spanish (Panama)"),
            ForLanguage(0x07, "SUBLANG_SPANISH_DOMINICAN_REPUBLIC", "Spanish (Dominican Republic)"),
            ForLanguage(0x08, "SUBLANG_SPANISH_VENEZUELA", "Spanish (Venezuela)"),
            ForLanguage(0x09, "SUBLANG_SPANISH_COLOMBIA", "Spanish (Colombia)"),
            ForLanguage(0x0a, "SUBLANG_SPANISH_PERU", "Spanish (Peru)"),
            ForLanguage(0x0b, "SUBLANG_SPANISH_ARGENTINA", "Spanish (Argentina)"),
            ForLanguage(0x0c, "SUBLANG_SPANISH_ECUADOR", "Spanish (Ecuador)"),
            ForLanguage(0x0d, "SUBLANG_SPANISH_CHILE", "Spanish (Chile)"),
            ForLanguage(0x0e, "SUBLANG_SPANISH_URUGUAY", "Spanish (Uruguay)"),
            ForLanguage(0x0f, "SUBLANG_SPANISH_PARAGUAY", "Spanish (Paraguay)"),
            ForLanguage(0x10, "SUBLANG_SPANISH_BOLIVIA", "Spanish (Bolivia)"),
            ForLanguage(0x11, "SUBLANG_SPANISH_EL_SALVADOR", "Spanish (El Salvador)"),
            ForLanguage(0x12, "SUBLANG_SPANISH_HONDURAS", "Spanish (Honduras)"),
            ForLanguage(0x13, "SUBLANG_SPANISH_NICARAGUA", "Spanish (Nicaragua)"),
            ForLanguage(0x14, "SUBLANG_SPANISH_PUERTO_RICO", "Spanish (Puerto Rico)"),
            ForLanguage(0x01, "SUBLANG_SWEDISH", "Swedish"),
            ForLanguage(0x02, "SUBLANG_SWEDISH_FINLAND", "Swedish (Finland)"),
            ForLanguage(0x01, "SUBLANG_URDU_PAKISTAN", "Urdu (Pakistan)"),
            ForLanguage(0x02, "SUBLANG_URDU_INDIA", "Urdu (India)"),
            ForLanguage(0x01, "SUBLANG_UZBEK_LATIN", "Uzbek (Latin)"),
            ForLanguage(0x02, "SUBLANG_UZBEK_CYRILLIC", "Uzbek (Cyrillic)"),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new SubLanguageIdentifierData instance for
        /// the given values.
        /// </summary>
        private static SubLanguageIdentifierData ForLanguage(int identifier,
            string symbol, string language)
        {
            SubLanguageIdentifierData result;

            result = new SubLanguageIdentifierData();
            result._identifier = identifier;
            result._symbol = symbol;
            result._language = language;

            return result;
        }

        #endregion Private methods.

        #region Private fields.

        private int _identifier;
        private string _symbol;
        private string _language;

        #endregion Private fields.
    }

    /// <summary>Indicates the language that is used.</summary>
    public sealed class LanguageIdentifierData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private LanguageIdentifierData() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Returns the language identifier for the specified identifier value.
        /// </summary>
        /// <param name='identifier'>Identifier to look for.</param>
        /// <returns>
        /// The LanguageIdentifierData with the given value, null if not found.
        /// </returns>
        public static LanguageIdentifierData FindByIdentifier(int identifier)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Identifier == identifier)
                {
                    return Values[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the language identifier for the specified identifier value.
        /// </summary>
        /// <param name='identifier'>Identifier to look for.</param>
        /// <returns>
        /// The LanguageIdentifierData with the given value, null if not found.
        /// </returns>
        public static LanguageIdentifierData FindByIdentifier(string identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }
            if (identifier.Length != 4)
            {
                throw new ArgumentException(
                    "Language identifier needs to be 4 characters long.",
                    "identifier");
            }
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].IdentifierString.Equals(identifier, StringComparison.InvariantCultureIgnoreCase))
                {
                    return Values[i];
                }
            }
            return null;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Numeric identifier for the language.</summary>
        public int Identifier
        {
            get { return _identifier; }
        }

        /// <summary>
        /// Numeric identifier for the language in 4 character
        /// hex format.
        /// </summary>
        public string IdentifierString
        {
            get
            {
                return Identifier.ToString("X4", CultureInfo.InvariantCulture);
            }
        }

        /// <summary>Name of the language.</summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// System-defined language identifiers
        /// </summary>
        public static LanguageIdentifierData[] Values = new LanguageIdentifierData[] {
            ForValues(0x0000, "Language Neutral"),
            ForValues(0x007f, "The language for the invariant locale (LOCALE_INVARIANT). See MAKELCID."),
            ForValues(0x0400, "Process or User Default Language"),
            ForValues(0x0800, "System Default Language"),
            ForValues(0x0436, "Afrikaans"),
            ForValues(0x041c, "Albanian"),
            ForValues(0x0401, "Arabic (Saudi Arabia)"),
            ForValues(0x0801, "Arabic (Iraq)"),
            ForValues(0x0c01, "Arabic (Egypt)"),
            ForValues(0x1001, "Arabic (Libya)"),
            ForValues(0x1401, "Arabic (Algeria)"),
            ForValues(0x1801, "Arabic (Morocco)"),
            ForValues(0x1c01, "Arabic (Tunisia)"),
            ForValues(0x2001, "Arabic (Oman)"),
            ForValues(0x2401, "Arabic (Yemen)"),
            ForValues(0x2801, "Arabic (Syria)"),
            ForValues(0x2c01, "Arabic (Jordan)"),
            ForValues(0x3001, "Arabic (Lebanon)"),
            ForValues(0x3401, "Arabic (Kuwait)"),
            ForValues(0x3801, "Arabic (U.A.E.)"),
            ForValues(0x3c01, "Arabic (Bahrain)"),
            ForValues(0x4001, "Arabic (Qatar)"),
            ForValues(0x042b, "Windows 2000/XP: Armenian. This is Unicode only."),
            ForValues(0x042c, "Azeri (Latin)"),
            ForValues(0x082c, "Azeri (Cyrillic)"),
            ForValues(0x042d, "Basque"),
            ForValues(0x0423, "Belarusian"),
            ForValues(0x0445, "Bengali (India)"),
            ForValues(0x141a, "Bosnian (Bosnia and Herzegovina)"),
            ForValues(0x0402, "Bulgarian"),
            ForValues(0x0455, "Burmese"),
            ForValues(0x0403, "Catalan"),
            ForValues(0x0404, "Chinese (Taiwan)"),
            ForValues(0x0804, "Chinese (PRC)"),
            ForValues(0x0c04, "Chinese (Hong Kong SAR, PRC)"),
            ForValues(0x1004, "Chinese (Singapore)"),
            ForValues(0x1404, "Windows 98/Me, Windows 2000/XP: Chinese (Macao SAR)"),
            ForValues(0x041a, "Croatian"),
            ForValues(0x101a, "Croatian (Bosnia and Herzegovina)"),
            ForValues(0x0405, "Czech"),
            ForValues(0x0406, "Danish"),
            ForValues(0x0465, "Windows XP: Divehi. This is Unicode only."),
            ForValues(0x0413, "Dutch (Netherlands)"),
            ForValues(0x0813, "Dutch (Belgium)"),
            ForValues(0x0409, "English (United States)"),
            ForValues(0x0809, "English (United Kingdom)"),
            ForValues(0x0c09, "English (Australian)"),
            ForValues(0x1009, "English (Canadian)"),
            ForValues(0x1409, "English (New Zealand)"),
            ForValues(0x1809, "English (Ireland)"),
            ForValues(0x1c09, "English (South Africa)"),
            ForValues(0x2009, "English (Jamaica)"),
            ForValues(0x2409, "English (Caribbean)"),
            ForValues(0x2809, "English (Belize)"),
            ForValues(0x2c09, "English (Trinidad)"),
            ForValues(0x3009, "Windows 98/Me, Windows 2000/XP: English (Zimbabwe)"),
            ForValues(0x3409, "Windows 98/Me, Windows 2000/XP: English (Philippines)"),
            ForValues(0x0425, "Estonian"),
            ForValues(0x0438, "Faeroese"),
            ForValues(0x0429, "Farsi"),
            ForValues(0x040b, "Finnish"),
            ForValues(0x040c, "French (Standard)"),
            ForValues(0x080c, "French (Belgian)"),
            ForValues(0x0c0c, "French (Canadian)"),
            ForValues(0x100c, "French (Switzerland)"),
            ForValues(0x140c, "French (Luxembourg)"),
            ForValues(0x180c, "Windows 98/Me, Windows 2000/XP: French (Monaco)"),
            ForValues(0x0456, "Windows XP: Galician"),
            ForValues(0x0437, "Windows 2000/XP: Georgian. This is Unicode only."),
            ForValues(0x0407, "German (Standard)"),
            ForValues(0x0807, "German (Switzerland)"),
            ForValues(0x0c07, "German (Austria)"),
            ForValues(0x1007, "German (Luxembourg)"),
            ForValues(0x1407, "German (Liechtenstein)"),
            ForValues(0x0408, "Greek"),
            ForValues(0x0447, "Windows XP: Gujarati. This is Unicode only."),
            ForValues(0x040d, "Hebrew"),
            ForValues(0x0439, "Windows 2000/XP: Hindi. This is Unicode only."),
            ForValues(0x040e, "Hungarian"),
            ForValues(0x040f, "Icelandic"),
            ForValues(0x0421, "Indonesian"),
            ForValues(0x0434, "isiXhosa/Xhosa (South Africa)"),
            ForValues(0x0435, "isiZulu/Zulu (South Africa)"),
            ForValues(0x0410, "Italian (Standard)"),
            ForValues(0x0810, "Italian (Switzerland)"),
            ForValues(0x0411, "Japanese"),
            ForValues(0x044b, "Windows XP: Kannada. This is Unicode only."),
            ForValues(0x0457, "Windows 2000/XP: Konkani. This is Unicode only."),
            ForValues(0x0412, "Korean"),
            ForValues(0x0812, "Windows 95, Windows NT 4.0 only: Korean (Johab)"),
            ForValues(0x0440, "Windows XP: Kyrgyz."),
            ForValues(0x0426, "Latvian"),
            ForValues(0x0427, "Lithuanian"),
            ForValues(0x0827, "Windows 98 only: Lithuanian (Classic)"),
            ForValues(0x042f, "FYRO Macedonian"),
            ForValues(0x043e, "Malay (Malaysian)"),
            ForValues(0x083e, "Malay (Brunei Darussalam)"),
            ForValues(0x044c, "Malayalam (India)"),
            ForValues(0x0481, "Maori (New Zealand)"),
            ForValues(0x043a, "Maltese (Malta)"),
            ForValues(0x044e, "Windows 2000/XP: Marathi. This is Unicode only."),
            ForValues(0x0450, "Windows XP: Mongolian"),
            ForValues(0x0414, "Norwegian (Bokmal)"),
            ForValues(0x0814, "Norwegian (Nynorsk)"),
            ForValues(0x0415, "Polish"),
            ForValues(0x0416, "Portuguese (Brazil)"),
            ForValues(0x0816, "Portuguese (Portugal)"),
            ForValues(0x0446, "Windows XP: Punjabi. This is Unicode only."),
            ForValues(0x046b, "Quechua (Bolivia)"),
            ForValues(0x086b, "Quechua (Ecuador)"),
            ForValues(0x0c6b, "Quechua (Peru)"),
            ForValues(0x0418, "Romanian"),
            ForValues(0x0419, "Russian"),
            ForValues(0x044f, "Windows 2000/XP: Sanskrit. This is Unicode only."),
            ForValues(0x043b, "Sami, Northern (Norway)"),
            ForValues(0x083b, "Sami, Northern (Sweden)"),
            ForValues(0x0c3b, "Sami, Northern (Finland)"),
            ForValues(0x103b, "Sami, Lule (Norway)"),
            ForValues(0x143b, "Sami, Lule (Sweden)"),
            ForValues(0x183b, "Sami, Southern (Norway)"),
            ForValues(0x1c3b, "Sami, Southern (Sweden)"),
            ForValues(0x203b, "Sami, Skolt (Finland)"),
            ForValues(0x243b, "Sami, Inari (Finland)"),
            ForValues(0x0c1a, "Serbian (Cyrillic)"),
            ForValues(0x1c1a, "Serbian (Cyrillic, Bosnia, and Herzegovina)"),
            ForValues(0x081a, "Serbian (Latin)"),
            ForValues(0x181a, "Serbian (Latin, Bosnia, and Herzegovina)"),
            ForValues(0x046c, "Sesotho sa Leboa/Northern Sotho (South Africa)"),
            ForValues(0x0432, "Setswana/Tswana (South Africa)"),
            ForValues(0x041b, "Slovak"),
            ForValues(0x0424, "Slovenian"),
            ForValues(0x040a, "Spanish (Spain, Traditional Sort)"),
            ForValues(0x080a, "Spanish (Mexican)"),
            ForValues(0x0c0a, "Spanish (Spain, Modern Sort)"),
            ForValues(0x100a, "Spanish (Guatemala)"),
            ForValues(0x140a, "Spanish (Costa Rica)"),
            ForValues(0x180a, "Spanish (Panama)"),
            ForValues(0x1c0a, "Spanish (Dominican Republic)"),
            ForValues(0x200a, "Spanish (Venezuela)"),
            ForValues(0x240a, "Spanish (Colombia)"),
            ForValues(0x280a, "Spanish (Peru)"),
            ForValues(0x2c0a, "Spanish (Argentina)"),
            ForValues(0x300a, "Spanish (Ecuador)"),
            ForValues(0x340a, "Spanish (Chile)"),
            ForValues(0x380a, "Spanish (Uruguay)"),
            ForValues(0x3c0a, "Spanish (Paraguay)"),
            ForValues(0x400a, "Spanish (Bolivia)"),
            ForValues(0x440a, "Spanish (El Salvador)"),
            ForValues(0x480a, "Spanish (Honduras)"),
            ForValues(0x4c0a, "Spanish (Nicaragua)"),
            ForValues(0x500a, "Spanish (Puerto Rico)"),
            ForValues(0x0430, "Sutu"),
            ForValues(0x0441, "Swahili (Kenya)"),
            ForValues(0x041d, "Swedish"),
            ForValues(0x081d, "Swedish (Finland)"),
            ForValues(0x045a, "Windows XP: Syriac. This is Unicode only."),
            ForValues(0x0449, "Windows 2000/XP: Tamil. This is Unicode only."),
            ForValues(0x0444, "Tatar (Tatarstan)"),
            ForValues(0x044a, "Windows XP: Telugu. This is Unicode only."),
            ForValues(0x041e, "Thai"),
            ForValues(0x041f, "Turkish"),
            ForValues(0x0422, "Ukrainian"),
            ForValues(0x0420, "Windows 98/Me, Windows 2000/XP: Urdu (Pakistan)"),
            ForValues(0x0820, "Urdu (India)"),
            ForValues(0x0443, "Uzbek (Latin)"),
            ForValues(0x0843, "Uzbek (Cyrillic)"),
            ForValues(0x042a, "Windows 98/Me, Windows NT 4.0 and later: Vietnamese"),
            ForValues(0x0452, "Welsh (United Kingdom)"),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new LanguageIdentifier instance for
        /// the given values.
        /// </summary>
        private static LanguageIdentifierData ForValues(int identifier, string language)
        {
            LanguageIdentifierData result;

            result = new LanguageIdentifierData();
            result._identifier = identifier;
            result._language = language;

            return result;
        }

        #endregion Private methods.

   
        #region Private fields.

        private int _identifier;
        private string _language;

        #endregion Private fields.
    }

    /// <summary>
    /// Provides information about available keyboard layouts.
    /// </summary>
    /// <remarks>
    /// Keyboard Layouts are found in the registry, at
    /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layouts.
    /// </remarks>
    public sealed class KeyboardLayoutData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private KeyboardLayoutData() { }

        #endregion Constructors.

        
        #region Public methods.

        /// <summary>
        /// Returns the keyboard layout for the specified identifier name.
        /// </summary>
        /// <param name='name'>Name to look for.</param>
        /// <returns>
        /// The KeyboardLayoutData with the given name, null if not found.
        /// </returns>
        public static KeyboardLayoutData FindByName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            foreach(KeyboardLayoutData layout in Values)
            {
                if (layout.Name == name)
                {
                    return layout;
                }
            }
            return null;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Identifier for keyboard layout.</summary>
        public uint KeyboardId
        {
            get { return _keyboardId; }
        }

        /// <summary>8-character identifier for keyboard ID.</summary>
        public string KeyboardIdString
        {
            get { return _keyboardId.ToString("X8", CultureInfo.InvariantCulture); }
        }

        /// <summary>Identifier for keyboard layout for HKL composing.</summary>
        public uint LayoutId
        {
            get { return _layoutId; }
        }

        /// <summary>4-character identifier for layout ID.</summary>
        public string LayoutIdString
        {
            get { return _layoutId.ToString("X4", CultureInfo.InvariantCulture); }
        }

        /// <summary>Name of keyboard layout.</summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Whether this keyboard layout cannot be packed into 16
        /// bits and required composition with language to
        /// create a valid HKL.
        /// </summary>
        public bool RequiresHklComposition
        {
            get { return (_layoutId & 0xFFFF0000) != 0; }
        }

        /// <summary>
        /// Name of IME file, an empty string if no IME file is required.
        /// </summary>
        public string ImeFile
        {
            get { return (_imeFile != null)? _imeFile : ""; }
        }

        /// <summary>
        /// KeyboardLayoutData for Latin American keyboard.
        /// </summary>
        public static KeyboardLayoutData LatinAmerican
        {
            get
            {
                return FindByName("Latin American");
            }
        }

        /// <summary>KeyboardLayoutData for US keyboard.</summary>
        public static KeyboardLayoutData US
        {
            get
            {
                return FindByName("US");
            }
        }

        /// <summary>
        /// Available KeyboardLayoutData values.
        /// </summary>
        public static KeyboardLayoutData[] Values = new KeyboardLayoutData[] {
            ForValues(0x00000401, "Arabic (101)"),
            ForValues(0x00000402, "Bulgarian"),
            ForValues(0x00000404, "Chinese (Traditional) - US Keyboard"),
            ForValues(0x00000405, "Czech"),
            ForValues(0x00000406, "Danish"),
            ForValues(0x00000407, "German"),
            ForValues(0x00000408, "Greek"),
            ForValues(0x00000409, "US"),
            ForValues(0x0000040a, "Spanish"),
            ForValues(0x0000040b, "Finnish"),
            ForValues(0x0000040c, "French"),
            ForValues(0x0000040d, "Hebrew"),
            ForValues(0x0000040e, "Hungarian"),
            ForValues(0x0000040f, "Icelandic"),
            ForValues(0x00000410, "Italian"),
            ForValues(0x00000411, "Japanese"),
            ForValues(0x00000412, "Korean"),
            ForValues(0x00000413, "Dutch"),
            ForValues(0x00000414, "Norwegian"),
            ForValues(0x00000415, "Polish (Programmers)"),
            ForValues(0x00000416, "Portuguese (Brazilian ABNT)"),
            ForValues(0x00000418, "Romanian"),
            ForValues(0x00000419, "Russian"),
            ForValues(0x0000041A, "Croatian"),
            ForValues(0x0000041b, "Slovak"),
            ForValues(0x0000041c, "Albanian"),
            ForValues(0x0000041d, "Swedish"),
            ForValues(0x0000041e, "Thai Kedmanee"),
            ForValues(0x0000041f, "Turkish Q"),
            ForValues(0x00000420, "Urdu"),
            ForValues(0x00000422, "Ukrainian"),
            ForValues(0x00000423, "Belarusian"),
            ForValues(0x00000424, "Slovenian"),
            ForValues(0x00000425, "Estonian"),
            ForValues(0x00000426, "Latvian"),
            ForValues(0x00000427, "Lithuanian IBM"),
            ForValues(0x00000429, "Farsi"),
            ForValues(0x0000042a, "Vietnamese"),
            ForValues(0x0000042b, "Armenian Eastern"),
            ForValues(0x0000042c, "Azeri Latin"),
            ForValues(0x0000042f, "FYRO Macedonian"),
            ForValues(0x00000437, "Georgian"),
            ForValues(0x00000438, "Faeroese"),
            ForValues(0x00000439, "Devanagari - INSCRIPT"),
            ForValues(0x0000043a, "Maltese 47-key"),
            ForValues(0x0000043b, "Norwegian with Sami"),
            ForValues(0x0000043f, "Kazakh"),
            ForValues(0x00000440, "Kyrgyz Cyrillic"),
            ForValues(0x00000444, "Tatar"),
            ForValues(0x00000445, "Bengali"),
            ForValues(0x00000446, "Punjabi"),
            ForValues(0x00000447, "Gujarati"),
            ForValues(0x00000449, "Tamil"),
            ForValues(0x0000044a, "Telugu"),
            ForValues(0x0000044b, "Kannada"),
            ForValues(0x0000044c, "Malayalam"),
            ForValues(0x0000044e, "Marathi"),
            ForValues(0x00000450, "Mongolian Cyrillic"),
            ForValues(0x00000452, "United Kingdom Extended"),
            ForValues(0x0000045a, "Syriac"),
            ForValues(0x00000465, "Divehi Phonetic"),
            ForValues(0x00000481, "Maori"),
            ForValues(0x00000804, "Chinese (Simplified) - US Keyboard"),
            ForValues(0x00000807, "Swiss German"),
            ForValues(0x00000809, "United Kingdom"),
            ForValues(0x0000080a, "Latin American"),
            ForValues(0x0000080c, "Belgian French"),
            ForValues(0x00000813, "Belgian (Period)"),
            ForValues(0x00000816, "Portuguese"),
            ForValues(0x0000081a, "Serbian (Latin)"),
            ForValues(0x0000082c, "Azeri Cyrillic"),
            ForValues(0x0000083b, "Swedish with Sami"),
            ForValues(0x00000843, "Uzbek Cyrillic"),
            ForValues(0x00000c0c, "Canadian French (Legacy)"),
            ForValues(0x00000c1a, "Serbian (Cyrillic)"),
            ForValues(0x00001009, "Canadian French"),
            ForValues(0x0000100c, "Swiss French"),
            ForValues(0x0000141a, "Bosnian"),
            ForValues(0x00001809, "Irish"),
            ForValues(0x00010401, "Arabic (102)", 0x0028),
            ForValues(0x00010402, "Bulgarian (Latin)", 0x0004),
            ForValues(0x00010405, "Czech (QWERTY)", 0x0005),
            ForValues(0x00010407, "German (IBM)", 0x0012),
            ForValues(0x00010408, "Greek (220)", 0x0016),
            ForValues(0x00010409, "United States-Dvorak", 0x0002),
            ForValues(0x0001040a, "Spanish Variation", 0x0086),
            ForValues(0x0001040e, "Hungarian 101-key", 0x0006),
            ForValues(0x00010410, "Italian (142)", 0x0003),
            ForValues(0x00010415, "Polish (214)", 0x0007),
            ForValues(0x00010416, "Portuguese (Brazilian ABNT2)", 0x001D),
            ForValues(0x00010419, "Russian (Typewriter)", 0x0008),
            ForValues(0x0001041b, "Slovak (QWERTY)", 0x0013),
            ForValues(0x0001041e, "Thai Pattachote", 0x0021),
            ForValues(0x0001041f, "Turkish F", 0x0014),
            ForValues(0x00010426, "Latvian (QWERTY)", 0x0015),
            ForValues(0x00010427, "Lithuanian", 0x0027),
            ForValues(0x0001042b, "Armenian Western", 0x0025),
            ForValues(0x00010439, "Hindi Traditional", 0x000c),
            ForValues(0x0001043a, "Maltese 48-key", 0x002b),
            ForValues(0x0001043b, "Sami Extended Norway", 0x002c),
            ForValues(0x00010445, "Bengali (Inscript)", 0x002a),
            ForValues(0x0001045a, "Syriac Phonetic", 0x000E),
            ForValues(0x00010465, "Divehi Typewriter", 0x000D),
            ForValues(0x0001080c, "Belgian (Comma)", 0x001E),
            ForValues(0x0001083b, "Finnish with Sami", 0x002d),
            ForValues(0x00011009, "Canadian Multilingual Standard", 0x0020),
            ForValues(0x00011809, "Gaelic", 0x0026),
            ForValues(0x00020401, "Arabic (102) AZERTY", 0x0029),
            ForValues(0x00020405, "Czech Programmers", 0x000A),
            ForValues(0x00020408, "Greek (319)", 0x0018),
            ForValues(0x00020409, "United States-International", 0x0001),
            ForValues(0x0002041e, "Thai Kedmanee (non-ShiftLock)", 0x0022),
            ForValues(0x0002083b, "Sami Extended Finland-Sweden", 0x002e),
            ForValues(0x00030408, "Greek (220) Latin", 0x0017),
            ForValues(0x00030409, "United States-Dvorak for left hand", 0x001A),
            ForValues(0x0003041e, "Thai Pattachote (non-ShiftLock)", 0x0023),
            ForValues(0x00040408, "Greek (319) Latin", 0x0011),
            ForValues(0x00040409, "United States-Dvorak for right hand", 0x001B),
            ForValues(0x00050408, "Greek Latin", 0x0019),
            ForValues(0x00050409, "US English Table for IBM Arabic 238_L", 0x000B),
            ForValues(0x00060408, "Greek Polytonic", 0x001F),
            ForIme(0xE0010404, "Chinese (Traditional) - Phonetic", "phon.ime"),
            ForIme(0xE0010411, "Japanese Input System (MS-IME2002)", "imjp81.ime"),
            ForIme(0xE0010412, "Korean Input System (IME 2000)", "imekr61.ime"),
            ForIme(0xE0010804, "Chinese (Simplified) - QuanPin", "winpy.ime"),
            ForIme(0xE0020404, "Chinese (Traditional) - ChangJie", "chajei.ime"),
            ForIme(0xE0020804, "Chinese (Simplified) - ShuangPin", "winsp.ime"),
            ForIme(0xE0030404, "Chinese (Traditional) - Quick", "quick.ime"),
            ForIme(0xE0030804, "Chinese (Simplified) - ZhengMa", "winzm.ime"),
            ForIme(0xE0040404, "Chinese (Traditional) - Big5 Code", "winime.ime"),
            ForIme(0xE0050404, "Chinese (Traditional) - Array", "winar30.ime"),
            ForIme(0xE0050804, "Chinese (Simplified) - NeiMa", "wingb.ime"),
            ForIme(0xE0060404, "Chinese (Traditional) - DaYi", "dayi.ime"),
            ForIme(0xE0070404, "Chinese (Traditional) - Unicode", "unicdime.ime"),
            ForIme(0xE0080404, "Chinese (Traditional) - New Phonetic", "TINTLGNT.IME"),
            ForIme(0xE0090404, "Chinese (Traditional) - New ChangJie", "CINTLGNT.IME"),
            ForIme(0xE00E0804, "Chinese (Simplified) - Microsoft Pinyin IME 3.0", "pintlgnt.ime"),
            ForIme(0xE01F0404, "Chinese (Traditional) - Alphanumeric", "romanime.ime"),
        };

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Creates a new KeyboardLayoutData instance for the given values.
        /// </summary>
        private static KeyboardLayoutData ForValues(uint keyboardId,
            string name)
        {
            return ForValues(keyboardId, name, 0);
        }

        /// <summary>
        /// Creates a new KeyboardLayoutData instance for the given values.
        /// </summary>
        private static KeyboardLayoutData ForValues(uint keyboardId,
            string name, uint layoutId)
        {
            KeyboardLayoutData result;

            result = new KeyboardLayoutData();
            result._keyboardId = keyboardId;
            result._name = name;
            result._layoutId = layoutId;

            return result;
        }

        /// <summary>
        /// Creates a new KeyboardLayoutData instance for the given values.
        /// </summary>
        private static KeyboardLayoutData ForIme(uint keyboardId,
            string name, string imeFile)
        {
            KeyboardLayoutData result;

            result = new KeyboardLayoutData();
            result._keyboardId = keyboardId;
            result._name = name;
            result._imeFile = imeFile;

            return result;
        }

        #endregion Private methods.


        #region Private fields.

        private uint _keyboardId;
        private uint _layoutId;
        private string _name;
        private string _imeFile;

        #endregion Private fields.
    }

    /// <summary>
    /// Provides information about interesting input locale values.
    /// </summary>
    /// <remarks>
    /// An input locale is a combination of a language (with
    /// primary and sublanguage identifiers) and a keyboard
    /// device.
    /// </remarks>
    public sealed class InputLocaleData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private InputLocaleData() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Finds the InputLocaleInstance with the specified identifier.
        /// </summary>
        /// <param name='identifier'>Identifier to look for.</param>
        /// <returns>
        /// The InputLocaleInstance with the specified identifier, null if
        /// not found.
        /// </returns>
        public static InputLocaleData FindByIdentifier(string identifier)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i].Identifier == identifier)
                {
                    return Values[i];
                }
            }
            return null;
        }

        /// <summary>Returns a String representation of the object.</summary>
        /// <returns>String representation of the object.</returns>
        public override string ToString()
        {
            return LanguageName;
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>User-friendly name of input device.</summary>
        public string DeviceName
        {
            get { return _deviceName; }
        }

        /// <summary>Hex identifier for input locale.</summary>
        public string Identifier
        {
            get { return _identifier; }
        }

        /// <summary>User-friendly name of language.</summary>
        public string LanguageName
        {
            get { return _languageName; }
        }

        /// <summary>Default input locale for Arabic (Saudi Arabia).</summary>
        public static InputLocaleData ArabicSaudiArabia
        {
            get
            {
                return FindByIdentifier("00000401");
            }
        }

        /// <summary>Default input locale for German.</summary>
        public static InputLocaleData German
        {
            get
            {
                return FindByIdentifier("00000407");
            }
        }

        /// <summary>Default input locale for Hebrew.</summary>
        public static InputLocaleData Hebrew
        {
            get
            {
                return FindByIdentifier("0000040d");
            }
        }

        /// <summary>Default input locale for Chinese.</summary>
        public static InputLocaleData Chinese
        {
            get
            {
                return FindByIdentifier("00000804");
            }
        }

        /// <summary>Default input locale for Chinese.</summary>
        public static InputLocaleData ChineseTraditional
        {
            get
            {
                return FindByIdentifier("00000404");
            }
        }

        /// <summary>Default input locale for Korean.</summary>
        public static InputLocaleData Korean
        {
            get
            {
                return FindByIdentifier("00000412");
            }
        }

        /// <summary>Default input locale for French.</summary>
        public static InputLocaleData French
        {
            get
            {
                return FindByIdentifier("0000040c");
            }
        }

        /// <summary>Default input locale for Spanish.</summary>
        public static InputLocaleData Spanish
        {
            get
            {
                return FindByIdentifier("0000040a");
            }
        }

        /// <summary>Default input locale for Japanese - Japanese Input System (MS-IME2002).</summary>
        public static InputLocaleData JapaneseMsIme2002
        {
            get
            {
                return FindByIdentifier("00000411");
            }
        }

        /// <summary>Default input locale for English (United States).</summary>
        public static InputLocaleData EnglishUS
        {
            get
            {
                return FindByIdentifier("00000409");
            }
        }

        /// <summary>Default input locale for Spanish (Argentina).</summary>
        public static InputLocaleData SpanishArgentina
        {
            get
            {
                return FindByIdentifier("0000040a");
            }
        }

        /// <summary>Default input locale for Thai.</summary>
        public static InputLocaleData Thai
        {
            get
            {
                return FindByIdentifier("0000041e");
            }
        }

        /// <summary>
        /// Interesting InputLocaleData values for testing.
        /// </summary>
        public static InputLocaleData[] Values = new InputLocaleData[] {
            ForValues("00000401", "Arabic(SaudiArabia)", "Arabic (101)"),
            ForValues("00000804", "Chinese", "Chinese (Simplified)"),
            ForValues("00000404", "ChineseTraditional ", "Chinese (Traditional)"),
            ForValues("00000409", "English(UnitedStates)", "US"),
            ForValues("00000407", "German", "German"),
            ForValues("0000040d", "Hebrew", "Hebrew"),
            ForValues("00000412", "Korean", "Korean Input System (IME 2000)"),
            ForValues("0000040c", "French", "French"),
            ForValues("00000411", "Japanese", "Japanese Input System (MS-IME2002)"),
            ForValues("0000040a", "Spanish(Argentina)", "Latin American"),
            ForValues("0000041e", "ThaiKedmanee", "Thai Kedmanee"),
        };

        #endregion Public properties.


        #region Private methods.

        /// <summary>
        /// Creates a new InputLocaleData instance for the given script.
        /// </summary>
        private static InputLocaleData ForValues(string identifier,
            string languageName, string deviceName)
        {
            InputLocaleData result;

            result = new InputLocaleData();
            result._identifier = identifier;
            result._languageName = languageName;
            result._deviceName = deviceName;

            return result;
        }

        #endregion Private methods.


        #region Private fields.

        private string _identifier;
        private string _languageName;
        private string _deviceName;

        #endregion Private fields.
    }
}