// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************************
 *
 * Description: FontItems.cs implement the immediate window with the following
 * features:
 *      1. FontItems.
 *
 *******************************************************************************/

#region Using directives

using System;
using System.Diagnostics;
using System.Windows;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;    // for XmlLanguage


#endregion

/// <summary>
///
///     FontItem.cs - encapsulates entries in font chooser combo boxes
///
///     Provides appropriate behavior for the listbox and editbox components
///     of font chooser comboboxes.
///
///     Font size     Displays with 2 decimal points
///
///     Font family   Sorted alphabetically
///
///     Typeface      Sorted by stretch, then weight, then style
///                   Name constructed from stretch then weight then style
///                   Ability to determine nominal GlyphTypeface
///                   Ability to determin cultures for which (some) descriptive
///                      text is available.
///
///     DescriptionLanguage
///                   Displays and sorts using culture DisplayName
///
/// </summary>




namespace EditingExaminer
{

    /// <summary>
    /// IFontChooserComboBoxItem provides methods to control the relationship between the
    /// textbox and listbox components of a font chooser combobox.
    /// </summary>
    public interface IFontChooserComboBoxItem
    {
        int CompareWithString(string value);
    }
    
    /// <summary>
    /// class of FontSizeComboBoxItem
    /// </summary>
    public class FontSizeComboBoxItem : IFontChooserComboBoxItem
    {
        private double _size;

        /// <summary>
        /// FontSizeComboBoxItem
        /// </summary>
        /// <param name="size"></param>
        public FontSizeComboBoxItem(double size)
        {
            _size = size;
        }

        /// <summary>
        /// CompareWithString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareWithString(string value)
        {
            return _size.CompareTo(double.Parse(value));
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _size.ToString("0.##", CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// class of FontFamilyComboBoxItem
    /// </summary>
    public class FontFamilyComboBoxItem : IFontChooserComboBoxItem
    {
        private string _familyName;

        /// <summary>
        /// FontFamilyComboBoxItem
        /// </summary>
        /// <param name="familyName"></param>
        public FontFamilyComboBoxItem(string familyName)
        {
            _familyName = familyName;
        }

        /// <summary>
        /// CompareWithString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareWithString(string value)
        {
            return String.Compare(_familyName, value, true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _familyName;
        }
    }
    
    /// <summary>
    /// DescriptionLanguageComboBoxItem
    /// </summary>
    public class DescriptionLanguageComboBoxItem : IFontChooserComboBoxItem
    {
        private CultureInfo _descriptionCulture;

        /// <summary>
        /// DescriptionLanguageComboBoxItem
        /// </summary>
        /// <param name="culture"></param>
        public DescriptionLanguageComboBoxItem(CultureInfo culture)
        {
            _descriptionCulture = culture;
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _descriptionCulture.DisplayName;
        }

        /// <summary>
        /// CompareWithString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareWithString(string value)
        {
            return String.Compare(ToString(), value, true, CultureInfo.InvariantCulture);
        }

        public CultureInfo Culture { get { return _descriptionCulture; } }
    }

    /// <summary>
    /// define a class that contains a font face for use in UI listboxes.
    /// </summary>
    public class TypefaceComboBoxItem : ListBoxItem, IComparer, IFontChooserComboBoxItem
    {
        private Typeface _face;
        private List<CultureInfo>  _descriptiveTextCultures;

        private void InitializeTypeface(Typeface face)
        {
            _face = face;
            _descriptiveTextCultures = null;
            Content = ToString();
            if (_face.IsBoldSimulated || _face.IsObliqueSimulated)
                Foreground = Brushes.Gray;
            else
                Foreground = Brushes.Black;
        }

        /// <summary>
        /// TypefaceComboBoxItem
        /// </summary>
        public TypefaceComboBoxItem()
        {
            InitializeTypeface(new Typeface("Arial"));
        }

        /// <summary>
        /// TypefaceComboBoxItem
        /// </summary>
        /// <param name="face"></param>
        public TypefaceComboBoxItem(Typeface face)
        {
            InitializeTypeface(face);
        }

        /// <summary>
        /// NominalGlyphTypeface
        /// </summary>
        public GlyphTypeface NominalGlyphTypeface
        {
            get
            {
                GlyphTypeface glyphTypeface;
                _face.TryGetGlyphTypeface(out glyphTypeface);
                return glyphTypeface;
            }
        }

        /// <summary>
        /// DescriptiveTextCultures
        /// </summary>
        public List<CultureInfo> DescriptiveTextCultures
        {
            get
            {
                if (_descriptiveTextCultures == null)
                {
                    List<CultureInfo> descriptiveTextCultures = new List<CultureInfo>();
                    foreach (CultureInfo culture in s_allCultures)
                    {
                        if (  NominalGlyphTypeface.Copyrights[culture]        != null
                           |  NominalGlyphTypeface.Descriptions[culture]      != null
                           |  NominalGlyphTypeface.DesignerNames[culture]     != null
                           |  NominalGlyphTypeface.DesignerUrls[culture]      != null
                           |  NominalGlyphTypeface.FaceNames[culture]         != null
                           |  NominalGlyphTypeface.FamilyNames[culture]       != null
                           |  NominalGlyphTypeface.ManufacturerNames[culture] != null
                           |  NominalGlyphTypeface.SampleTexts[culture]       != null
                           |  NominalGlyphTypeface.Trademarks[culture]        != null
                           |  NominalGlyphTypeface.VendorUrls[culture]        != null
                           |  NominalGlyphTypeface.VersionStrings[culture]    != null
                           |  NominalGlyphTypeface.Win32FaceNames[culture]    != null
                           |  NominalGlyphTypeface.Win32FamilyNames[culture]  != null
                           )
                        {
                            descriptiveTextCultures.Add(culture);
                        }
                    }
                    _descriptiveTextCultures = descriptiveTextCultures;
                }
                return _descriptiveTextCultures;
            }
        }

        /// <summary>
        /// FamilyName
        /// </summary>
        public string FamilyName    { get { return _face.FontFamily.Source; } }
        
        /// <summary>
        /// Style
        /// </summary>
        public new FontStyle Style      { get { return _face.Style; } }
        
        /// <summary>
        /// Weight
        /// </summary>
        public FontWeight Weight    { get { return _face.Weight; } }
        
        /// <summary>
        /// Stretch
        /// </summary>
        public FontStretch Stretch  { get { return _face.Stretch; } }

        /// <summary>
        /// Build and return a corresponding Typeface
        /// </summary>
        public Typeface Typeface
        {
            get
            {
                return new Typeface(_face.FontFamily, Style, Weight, Stretch, null); // Note: no fallback family
            }
        }

        /// <summary>
        /// the string returned is used to create the list item.
        /// </summary>
        /// <returns>font face name</returns>
        public override string ToString()
        {
            // Note: When new APIs are available to obtain the language-specific face name
            // from the font this code will need to be replaced with calls to those new APIs.
            IDictionary<XmlLanguage, string> faceNames = _face.FaceNames;
            string faceName;

            // If there is no entry for the current culture or English US culture,
            // just pick the first face name.
            if (!faceNames.TryGetValue(XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag), out faceName) &&
                !faceNames.TryGetValue(XmlLanguage.GetLanguage("en-us"), out faceName))
            {
                foreach (KeyValuePair<XmlLanguage, string> pair in faceNames)
                {
                    faceName = pair.Value;
                    break;
                }
            }
            Debug.Assert(faceName != null);
            return faceName;
        }

        /// <summary>
        /// CompareWithString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int CompareWithString(string value)
        {
            return String.Compare(ToString(), value, true, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Compares the family, style, weight and stretch of two TypefacesItems.
        /// </summary>
        public int Compare(object face1Object, object face2Object)
        {
            TypefaceComboBoxItem face1 = face1Object as TypefaceComboBoxItem;
            TypefaceComboBoxItem face2 = face2Object as TypefaceComboBoxItem;

            if (face1.FamilyName != face2.FamilyName)
            {
                return string.Compare(face1.FamilyName, face2.FamilyName, true, CultureInfo.InvariantCulture);
            }
            else if ((face1._face.IsBoldSimulated || face1._face.IsObliqueSimulated) != (face2._face.IsBoldSimulated || face2._face.IsObliqueSimulated))
            {
                // Put non-simulated faces first.
                return (face1._face.IsBoldSimulated || face1._face.IsObliqueSimulated) ? 1 : -1;
            }
            else if (face1.Stretch != face2.Stretch)
            {
                return FontStretch.Compare(face1.Stretch, face2.Stretch);
            }
            else if (face1.Weight != face2.Weight)
            {
                return FontWeight.Compare(face1.Weight, face2.Weight);
            }
            else
            {
                if (face1.Style == face2.Style)
                {
                    return 0;
                }
                else
                {
                    if (face1.Style == FontStyles.Normal)
                    {
                        return -1;
                    }
                    else if (face1.Style == FontStyles.Oblique)
                    {
                        return 1;
                    }
                    else
                    {
                        return (face2.Style == FontStyles.Normal) ? 1 : -1;
                    }
                }
            }
        }

        /// <summary>
        /// impement the equal method
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <returns></returns>
        public static bool Equals(TypefaceComboBoxItem face1, TypefaceComboBoxItem face2)
        {
            return face1.Stretch == face2.Stretch
                && face1.Weight == face2.Weight
                && face1.Style == face2.Style;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <param name="face"></param>
        /// <returns></returns>
        public int GetHashCode(TypefaceComboBoxItem face)
        {
            return _face.GetHashCode();
        }

        /// <summary>
        /// SortedTypefaces
        /// </summary>
        /// <returns></returns>
        public ICollection SortedTypefaces()
        {
            ArrayList faces = new ArrayList();
            foreach (Typeface face in _face.FontFamily.GetTypefaces())
            {
                faces.Add(new TypefaceComboBoxItem(face));
            }
            faces.Sort(new TypefaceComboBoxItem());
            return faces;
        }

        private static CultureInfo[] s_allCultures = {
            new CultureInfo(0x0036), // Afrikaans
            new CultureInfo(0x0436), // Afrikaans - South Africa
            new CultureInfo(0x001C), // Albanian
            new CultureInfo(0x041C), // Albanian - Albania
            new CultureInfo(0x0001), // Arabic
            new CultureInfo(0x1401), // Arabic - Algeria
            new CultureInfo(0x3C01), // Arabic - Bahrain
            new CultureInfo(0x0C01), // Arabic - Egypt
            new CultureInfo(0x0801), // Arabic - Iraq
            new CultureInfo(0x2C01), // Arabic - Jordan
            new CultureInfo(0x3401), // Arabic - Kuwait
            new CultureInfo(0x3001), // Arabic - Lebanon
            new CultureInfo(0x1001), // Arabic - Libya
            new CultureInfo(0x1801), // Arabic - Morocco
            new CultureInfo(0x2001), // Arabic - Oman
            new CultureInfo(0x4001), // Arabic - Qatar
            new CultureInfo(0x0401), // Arabic - Saudi Arabia
            new CultureInfo(0x2801), // Arabic - Syria
            new CultureInfo(0x1C01), // Arabic - Tunisia
            new CultureInfo(0x3801), // Arabic - United Arab Emirates
            new CultureInfo(0x2401), // Arabic - Yemen
            new CultureInfo(0x002B), // Armenian
            new CultureInfo(0x042B), // Armenian - Armenia
            new CultureInfo(0x002C), // Azeri
            new CultureInfo(0x082C), // Azeri (Cyrillic) - Azerbaijan
            new CultureInfo(0x042C), // Azeri (Latin) - Azerbaijan
            new CultureInfo(0x002D), // Basque
            new CultureInfo(0x042D), // Basque - Basque
            new CultureInfo(0x0023), // Belarusian
            new CultureInfo(0x0423), // Belarusian - Belarus
            new CultureInfo(0x0002), // Bulgarian
            new CultureInfo(0x0402), // Bulgarian - Bulgaria
            new CultureInfo(0x0003), // Catalan
            new CultureInfo(0x0403), // Catalan - Catalan
            new CultureInfo(0x0C04), // Chinese - Hong Kong SAR
            new CultureInfo(0x1404), // Chinese - Macao SAR
            new CultureInfo(0x0804), // Chinese - China
            new CultureInfo(0x0004), // Chinese (Simplified)
            new CultureInfo(0x1004), // Chinese - Singapore
            new CultureInfo(0x0404), // Chinese - Taiwan
            new CultureInfo(0x7C04), // Chinese (Traditional)
            new CultureInfo(0x001A), // Croatian
            new CultureInfo(0x041A), // Croatian - Croatia
            new CultureInfo(0x0005), // Czech
            new CultureInfo(0x0405), // Czech - Czech Republic
            new CultureInfo(0x0006), // Danish
            new CultureInfo(0x0406), // Danish - Denmark
            new CultureInfo(0x0065), // Dhivehi
            new CultureInfo(0x0465), // Dhivehi - Maldives
            new CultureInfo(0x0013), // Dutch
            new CultureInfo(0x0813), // Dutch - Belgium
            new CultureInfo(0x0413), // Dutch - The Netherlands
            new CultureInfo(0x0009), // English
            new CultureInfo(0x0C09), // English - Australia
            new CultureInfo(0x2809), // English - Belize
            new CultureInfo(0x1009), // English - Canada
            new CultureInfo(0x2409), // English - Caribbean
            new CultureInfo(0x1809), // English - Ireland
            new CultureInfo(0x2009), // English - Jamaica
            new CultureInfo(0x1409), // English - New Zealand
            new CultureInfo(0x3409), // English - Philippines
            new CultureInfo(0x1C09), // English - South Africa
            new CultureInfo(0x2C09), // English - Trinidad and Tobago
            new CultureInfo(0x0809), // English - United Kingdom
            new CultureInfo(0x0409), // English - United States
            new CultureInfo(0x3009), // English - Zimbabwe
            new CultureInfo(0x0025), // Estonian
            new CultureInfo(0x0425), // Estonian - Estonia
            new CultureInfo(0x0038), // Faroese
            new CultureInfo(0x0438), // Faroese - Faroe Islands
            new CultureInfo(0x0029), // Farsi
            new CultureInfo(0x0429), // Farsi - Iran
            new CultureInfo(0x000B), // Finnish
            new CultureInfo(0x040B), // Finnish - Finland
            new CultureInfo(0x000C), // French
            new CultureInfo(0x080C), // French - Belgium
            new CultureInfo(0x0C0C), // French - Canada
            new CultureInfo(0x040C), // French - France
            new CultureInfo(0x140C), // French - Luxembourg
            new CultureInfo(0x180C), // French - Monaco
            new CultureInfo(0x100C), // French - Switzerland
            new CultureInfo(0x0056), // Galician
            new CultureInfo(0x0456), // Galician - Galician
            new CultureInfo(0x0037), // Georgian
            new CultureInfo(0x0437), // Georgian - Georgia
            new CultureInfo(0x0007), // German
            new CultureInfo(0x0C07), // German - Austria
            new CultureInfo(0x0407), // German - Germany
            new CultureInfo(0x1407), // German - Liechtenstein
            new CultureInfo(0x1007), // German - Luxembourg
            new CultureInfo(0x0807), // German - Switzerland
            new CultureInfo(0x0008), // Greek
            new CultureInfo(0x0408), // Greek - Greece
            new CultureInfo(0x0047), // Gujarati
            new CultureInfo(0x0447), // Gujarati - India
            new CultureInfo(0x000D), // Hebrew
            new CultureInfo(0x040D), // Hebrew - Israel
            new CultureInfo(0x0039), // Hindi
            new CultureInfo(0x0439), // Hindi - India
            new CultureInfo(0x000E), // Hungarian
            new CultureInfo(0x040E), // Hungarian - Hungary
            new CultureInfo(0x000F), // Icelandic
            new CultureInfo(0x040F), // Icelandic - Iceland
            new CultureInfo(0x0021), // Indonesian
            new CultureInfo(0x0421), // Indonesian - Indonesia
            new CultureInfo(0x0010), // Italian
            new CultureInfo(0x0410), // Italian - Italy
            new CultureInfo(0x0810), // Italian - Switzerland
            new CultureInfo(0x0011), // Japanese
            new CultureInfo(0x0411), // Japanese - Japan
            new CultureInfo(0x004B), // Kannada
            new CultureInfo(0x044B), // Kannada - India
            new CultureInfo(0x003F), // Kazakh
            new CultureInfo(0x043F), // Kazakh - Kazakhstan
            new CultureInfo(0x0057), // Konkani
            new CultureInfo(0x0457), // Konkani - India
            new CultureInfo(0x0012), // Korean
            new CultureInfo(0x0412), // Korean - Korea
            new CultureInfo(0x0040), // Kyrgyz
            new CultureInfo(0x0440), // Kyrgyz - Kyrgyzstan
            new CultureInfo(0x0026), // Latvian
            new CultureInfo(0x0426), // Latvian - Latvia
            new CultureInfo(0x0027), // Lithuanian
            new CultureInfo(0x0427), // Lithuanian - Lithuania
            new CultureInfo(0x002F), // Macedonian
            new CultureInfo(0x042F), // Macedonian - Former Yugoslav Republic of Macedonia
            new CultureInfo(0x003E), // Malay
            new CultureInfo(0x083E), // Malay - Brunei
            new CultureInfo(0x043E), // Malay - Malaysia
            new CultureInfo(0x004E), // Marathi
            new CultureInfo(0x044E), // Marathi - India
            new CultureInfo(0x0050), // Mongolian
            new CultureInfo(0x0450), // Mongolian - Mongolia
            new CultureInfo(0x0014), // Norwegian
            new CultureInfo(0x0414), // Norwegian (Bokmål) - Norway
            new CultureInfo(0x0814), // Norwegian (Nynorsk) - Norway
            new CultureInfo(0x0015), // Polish
            new CultureInfo(0x0415), // Polish - Poland
            new CultureInfo(0x0016), // Portuguese
            new CultureInfo(0x0416), // Portuguese - Brazil
            new CultureInfo(0x0816), // Portuguese - Portugal
            new CultureInfo(0x0046), // Punjabi
            new CultureInfo(0x0446), // Punjabi - India
            new CultureInfo(0x0018), // Romanian
            new CultureInfo(0x0418), // Romanian - Romania
            new CultureInfo(0x0019), // Russian
            new CultureInfo(0x0419), // Russian - Russia
            new CultureInfo(0x004F), // Sanskrit
            new CultureInfo(0x044F), // Sanskrit - India
            new CultureInfo(0x0C1A), // Serbian (Cyrillic) - Serbia
            new CultureInfo(0x081A), // Serbian (Latin) - Serbia
            new CultureInfo(0x001B), // Slovak
            new CultureInfo(0x041B), // Slovak - Slovakia
            new CultureInfo(0x0024), // Slovenian
            new CultureInfo(0x0424), // Slovenian - Slovenia
            new CultureInfo(0x000A), // Spanish
            new CultureInfo(0x2C0A), // Spanish - Argentina
            new CultureInfo(0x400A), // Spanish - Bolivia
            new CultureInfo(0x340A), // Spanish - Chile
            new CultureInfo(0x240A), // Spanish - Colombia
            new CultureInfo(0x140A), // Spanish - Costa Rica
            new CultureInfo(0x1C0A), // Spanish - Dominican Republic
            new CultureInfo(0x300A), // Spanish - Ecuador
            new CultureInfo(0x440A), // Spanish - El Salvador
            new CultureInfo(0x100A), // Spanish - Guatemala
            new CultureInfo(0x480A), // Spanish - Honduras
            new CultureInfo(0x080A), // Spanish - Mexico
            new CultureInfo(0x4C0A), // Spanish - Nicaragua
            new CultureInfo(0x180A), // Spanish - Panama
            new CultureInfo(0x3C0A), // Spanish - Paraguay
            new CultureInfo(0x280A), // Spanish - Peru
            new CultureInfo(0x500A), // Spanish - Puerto Rico
            new CultureInfo(0x0C0A), // Spanish - Spain
            new CultureInfo(0x380A), // Spanish - Uruguay
            new CultureInfo(0x200A), // Spanish - Venezuela
            new CultureInfo(0x0041), // Swahili
            new CultureInfo(0x0441), // Swahili - Kenya
            new CultureInfo(0x001D), // Swedish
            new CultureInfo(0x081D), // Swedish - Finland
            new CultureInfo(0x041D), // Swedish - Sweden
            new CultureInfo(0x005A), // Syriac
            new CultureInfo(0x045A), // Syriac - Syria
            new CultureInfo(0x0049), // Tamil
            new CultureInfo(0x0449), // Tamil - India
            new CultureInfo(0x0044), // Tatar
            new CultureInfo(0x0444), // Tatar - Russia
            new CultureInfo(0x004A), // Telugu
            new CultureInfo(0x044A), // Telugu - India
            new CultureInfo(0x001E), // Thai
            new CultureInfo(0x041E), // Thai - Thailand
            new CultureInfo(0x001F), // Turkish
            new CultureInfo(0x041F), // Turkish - Turkey
            new CultureInfo(0x0022), // Ukrainian
            new CultureInfo(0x0422), // Ukrainian - Ukraine
            new CultureInfo(0x0020), // Urdu
            new CultureInfo(0x0420), // Urdu - Pakistan
            new CultureInfo(0x0043), // Uzbek
            new CultureInfo(0x0843), // Uzbek (Cyrillic) - Uzbekistan
            new CultureInfo(0x0443), // Uzbek (Latin) - Uzbekistan
            new CultureInfo(0x002A), // Vietnamese
            new CultureInfo(0x042A) // Vietnamese - Vietnam
        };
    }
}
