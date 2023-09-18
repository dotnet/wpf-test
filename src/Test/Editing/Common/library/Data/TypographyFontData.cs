// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Test.Uis.Data
{
    using System;    
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;    // for XmlLanguage
    using System.Windows.Media;
    using System.Windows.Media.TextFormatting;
    using System.Xml;
 
    /// <summary>
    /// Provides interesting test data for Advanced Typographical Features.
    /// </summary>
    public sealed class TypographyFontData
    {
        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private TypographyFontData() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Creates a new Paragraph with all the required properties applied to test the typography 
        /// feature associated with this TypographyFontData instance.
        /// </summary>
        /// <returns>Sample paragraph object with typography property applied.</returns>
        public Paragraph GetSampleParagraph()
        {
            Paragraph para = new Paragraph(new Run(TextContent));

            ApplyToObject(para);

            return para;            
        }

        /// <summary>
        /// Applies the typographical and its supporting properties to given dependencyObject.
        /// </summary>
        /// <param name="dependencyObject">DependencyObject to which properties have to be applied.</param>
        public void ApplyToObject(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            dependencyObject.SetValue(this.DependencyProperty, this.PropertyValue);
            dependencyObject.SetValue(TextElement.FontFamilyProperty, new FontFamily(this.FontFamilyName));
            dependencyObject.SetValue(TextElement.LanguageProperty, XmlLanguage.GetLanguage(this.CultureInfo.IetfLanguageTag));

            if ((AdditionalProperties != null) && (AdditionalValues != null))
            {
                for (int i = 0; i < AdditionalProperties.Length; i++)
                {
                    dependencyObject.SetValue(AdditionalProperties[i], AdditionalValues[i]);
                }
            }
        }

        /// <summary>
        /// Gets all TypographyFontData instances which are associated with the given 
        /// (Typography) DependencyProperty.
        /// </summary>
        /// <param name="property">Typography DependencyProperty.</param>
        /// <returns>TypographyFontData values for property.</returns>
        public static TypographyFontData[] GetValuesForDependencyProperty(DependencyProperty property)
        {
            System.Collections.Generic.List<TypographyFontData> values;            

            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            values = new System.Collections.Generic.List<TypographyFontData>(Values.Length);
            foreach (TypographyFontData data in Values)
            {
                if (data.DependencyProperty == property)
                {
                    values.Add(data);
                }
            }

            return values.ToArray();
        }

        /// <summary>
        /// Gets all TypographyFontData instances which are associated with the given fontFamilyName.
        /// </summary>
        /// <param name="fontFamilyName">Font family name.</param>
        /// <returns>TypographyFontData values for fontFamilyName.</returns>
        public static TypographyFontData[] GetValuesForFontFamily(string fontFamilyName)
        {
            System.Collections.Generic.List<TypographyFontData> values;

            if (fontFamilyName == null)
            {
                throw new ArgumentNullException("fontFamilyName");
            }

            values = new System.Collections.Generic.List<TypographyFontData>(Values.Length);
            foreach (TypographyFontData data in Values)
            {
                if (data.FontFamilyName == fontFamilyName)
                {
                    values.Add(data);
                }
            }

            return values.ToArray();
        }
         
        /// <summary>
        /// Gets all TypographyFontData instances which are associated with the given 
        /// (Typography dependencyProperty, Value) pair.
        /// </summary>
        /// <param name="property">DependencyProperty.</param>
        /// <param name="propertyValue">DependencyProperty value.</param>
        /// <returns>TypographyFontData values for the given pair.</returns>
        public static TypographyFontData[] GetValuesFor(DependencyProperty property, object propertyValue)
        {
            System.Collections.Generic.List<TypographyFontData> values;

            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException("propertyValue");
            }

            values = new System.Collections.Generic.List<TypographyFontData>(Values.Length);
            foreach (TypographyFontData data in Values)
            {
                if ((data.DependencyProperty == property) && (data.PropertyValue == propertyValue) )
                {
                    values.Add(data);
                }
            }

            return values.ToArray();
        }

        /// <summary>String representation of this object.</summary>
        /// <returns>Returns a string containing the Typography DendencyProperty and its value.</returns>
        public override string ToString()
        {
            return this.DependencyProperty.ToString() + " - " + this.PropertyValue.ToString();
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Additional DependencyProperties supporting the Typography property.</summary>
        public DependencyProperty[] AdditionalProperties
        {
            get { return _additionalProperties; }
        }

        /// <summary>Values for all AdditionalProperties.</summary>
        public object[] AdditionalValues
        {
            get { return _additionalValues; }
        }

        /// <summary>CultureInfo associated with this TypographyFontData instance.</summary>
        public CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
        }

        /// <summary>Typography DependencyProperty associated with this TypographyFontData instance.</summary>
        public DependencyProperty DependencyProperty
        {
            get { return _dependencyProperty; }
        }

        /// <summary>FontFamily name associated with this TypographyFontData instance.</summary>
        public string FontFamilyName
        {
            get { return _fontFamilyName; }
        }

        /// <summary>True if it is a simulated typographic effect, False otherwise.</summary>
        public bool IsSimulated
        {
            get { return _isSimulated; }
        }

        /// <summary>
        /// Name of the FontFamily which doesnt support the Typographical feature associated 
        /// with this TypographyFontData instance.
        /// </summary>
        public string NotSupportedFontFamilyName
        {
            get
            { 
                //"Impact" is another font which doesnt have opentype table
                return "Comin Sans MS"; 
            }
        }

        /// <summary>Xaml of a Paragraph element with typography and its supporting properties applied.</summary>
        public string SampleParagraphXaml
        {
            get
            {
                return System.Windows.Markup.XamlWriter.Save(GetSampleParagraph());                
            }
        }

        /// <summary>Property value for the Typography DependencyProperty.</summary>
        public object PropertyValue
        {
            get { return _propertyValue; }
        }

        /// <summary>Sample Text content.</summary>
        public string TextContent
        {
            get { return _textContent; }

            set
            {
                if (value == null)
                {
                    _textContent = String.Empty;
                }
                else
                {
                    _textContent = value;
                }
            }
        }
        
        /// <summary>Interesting test data values for Advanced Typography Features.</summary>
        public static TypographyFontData[] Values = new TypographyFontData[] {
            
            //CapitalsProperty
            ForValue(Typography.CapitalsProperty, FontCapitals.AllSmallCaps, "Palatino Linotype", 
            _capitalsString, false),
            ForValue(Typography.CapitalsProperty, FontCapitals.SmallCaps, "Palatino Linotype", 
            _capitalsString, false),
            ForValue(Typography.CapitalsProperty, FontCapitals.AllPetiteCaps, "Palatino Linotype", 
            _capitalsString, false),
            ForValue(Typography.CapitalsProperty, FontCapitals.PetiteCaps, "Palatino Linotype", 
            _capitalsString, false),
            ForValue(Typography.CapitalsProperty, FontCapitals.Unicase, "Palatino Linotype", 
            _capitalsString, false),
            ForValue(Typography.CapitalsProperty, FontCapitals.Titling, "Palatino Linotype", 
            _capitalsString, false),     
       
            //VariantsProperty
            ForValue(Typography.VariantsProperty, FontVariants.Superscript, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.VariantsProperty, FontVariants.Subscript, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.VariantsProperty, FontVariants.Ordinal, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.VariantsProperty, FontVariants.Inferior, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.VariantsProperty, FontVariants.Ruby, "Palatino Linotype", 
            _allCharNumString, false),               
    
            //ContextualAlternates
            ForValue(Typography.ContextualAlternatesProperty, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualAlternatesProperty, false, "Palatino Linotype", 
            _allCharNumString, false),

            //CapitalSpacing
            ForValue(Typography.CapitalSpacingProperty, true, "Palatino Linotype", 
            "Capital Spacing CAPITAL SPACING", false),
            ForValue(Typography.CapitalSpacingProperty, false, "Palatino Linotype", 
            "Capital Spacing CAPITAL SPACING", false),

            //ContextualLigatures
            ForValue(Typography.ContextualLigaturesProperty, true, "Palatino Linotype", 
            _contextualLigatureString, false),
            ForValue(Typography.ContextualLigaturesProperty, false, "Palatino Linotype", 
            _contextualLigatureString, false),

            //DiscretionaryLigatures
            ForValue(Typography.DiscretionaryLigaturesProperty, true, "Palatino Linotype", 
            _discretionaryLigatureString, false),
            ForValue(Typography.DiscretionaryLigaturesProperty, false, "Palatino Linotype", 
            _discretionaryLigatureString, false),

            //StandardLigatures
            ForValue(Typography.StandardLigaturesProperty, true, "Palatino Linotype", 
            _standardLigatureString, false),
            ForValue(Typography.StandardLigaturesProperty, false, "Palatino Linotype", 
            _standardLigatureString, false),

            //HistoricalLigatures
            ForValue(Typography.HistoricalLigaturesProperty, true, "Palatino Linotype", 
            _historicalLigatureString, false),
            ForValue(Typography.HistoricalLigaturesProperty, false, "Palatino Linotype", 
            _historicalLigatureString, false),

            //Kerning
            ForValue(Typography.KerningProperty, true, "Palatino Linotype", 
            _kerningString, false),
            ForValue(Typography.KerningProperty, false, "Palatino Linotype", 
            _kerningString, false),
            
            //MathematicalGreek
            ForValue(Typography.MathematicalGreekProperty, true, "Palatino Linotype", 
            _mathematicalGreekString, false),
            ForValue(Typography.MathematicalGreekProperty, false, "Palatino Linotype", 
            _mathematicalGreekString, false),

            //Fraction
            ForValue(Typography.FractionProperty, FontFraction.Slashed, "Palatino Linotype", 
            _fractionString, false),
            ForValue(Typography.FractionProperty, FontFraction.Stacked, "Palatino Linotype", 
            _fractionString, false),

            //NumeralAlignment
            ForValue(Typography.NumeralAlignmentProperty, FontNumeralAlignment.Normal, "Palatino Linotype", 
            _numeralString, false),
            ForValue(Typography.NumeralAlignmentProperty, FontNumeralAlignment.Proportional, "Palatino Linotype", 
            _numeralString, false),
            ForValue(Typography.NumeralAlignmentProperty, FontNumeralAlignment.Tabular, "Palatino Linotype", 
            _numeralString, false),

            //NumeralStyle
            ForValue(Typography.NumeralStyleProperty, FontNumeralStyle.Lining, "Palatino Linotype", 
            _numeralString, false),
            ForValue(Typography.NumeralStyleProperty, FontNumeralStyle.Normal, "Palatino Linotype", 
            _numeralString, false),
            ForValue(Typography.NumeralStyleProperty, FontNumeralStyle.OldStyle, "Palatino Linotype", 
            _numeralString, false),

            //SlashedZero
            ForValue(Typography.SlashedZeroProperty, true, "Palatino Linotype", 
            "Zero12344567890", false),
            ForValue(Typography.SlashedZeroProperty, false, "Palatino Linotype", 
            "Zero12344567890", false),            

            //StylisticSet
            ForValue(Typography.StylisticSet1Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet2Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet3Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet4Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet5Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet6Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet7Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet8Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet9Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet10Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet11Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet12Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet13Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet14Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet15Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet16Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet17Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet18Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet19Property, true, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StylisticSet20Property, true, "Palatino Linotype", 
            _allCharNumString, false),

            //StandardSwashes
            ForValue(Typography.StandardSwashesProperty, 1, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 2, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 3, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 4, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 5, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 6, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 7, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 8, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 9, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.StandardSwashesProperty, 10, "Palatino Linotype", 
            _allCharNumString, false),

            //ContextualSwashes
            ForValue(Typography.ContextualSwashesProperty, 1, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 2, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 3, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 4, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 5, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 6, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 7, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 8, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 9, "Palatino Linotype", 
            _allCharNumString, false),
            ForValue(Typography.ContextualSwashesProperty, 10, "Palatino Linotype", 
            _allCharNumString, false),

            //NumberSubstitution
            ForValue(NumberSubstitution.CultureOverrideProperty, CultureInfo.GetCultureInfoByIetfLanguageTag("ar-SA"), "Microsoft Sans Serif", 
            "123", false, "en-US",
            new DependencyProperty[] {
                NumberSubstitution.CultureSourceProperty, 
                NumberSubstitution.SubstitutionProperty},
            new object[] {
                NumberCultureSource.Override,
                NumberSubstitutionMethod.NativeNational}),

            //Example for using the constructor which takes the CultureInfoString as argument.
            //ForValue(Typography.ContextualLigaturesProperty, true, "Palatino Linotype", 
            //"fi fl ffi ffl fj ffj fb ffb ff ffh fk ffk", false, "es-AR"),
        };        
        #endregion Public properties.


        #region Private methods.        

        private static TypographyFontData ForValue(DependencyProperty dependencyProperty, object propertyValue,
            string fontFamilyName, string textContent, bool isSimulated)
        {
            return ForValue(dependencyProperty, propertyValue, fontFamilyName, textContent, isSimulated, "en-US", null, null);
        }

        private static TypographyFontData ForValue(DependencyProperty dependencyProperty, object propertyValue, 
            string fontFamilyName, string textContent, bool isSimulated, string cultureName)
        {
            return ForValue(dependencyProperty, propertyValue, fontFamilyName, textContent, isSimulated, cultureName, null, null);
        }

        private static TypographyFontData ForValue(DependencyProperty dependencyProperty, object propertyValue, 
            string fontFamilyName, string textContent, bool isSimulated, string cultureName,
            DependencyProperty[] additionalProperties, object[] additionalValues)
        {
            TypographyFontData result;

            result = new TypographyFontData();
            result._dependencyProperty = dependencyProperty;
            result._propertyValue = propertyValue;
            result._fontFamilyName = fontFamilyName;            
            result._textContent = textContent;
            result._isSimulated = isSimulated;

            result._cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(cultureName);

            if ((additionalProperties != null) && (additionalValues != null))
            {
                if (additionalProperties.Length == additionalValues.Length)
                {
                    result._additionalProperties = additionalProperties;
                    result._additionalValues = additionalValues;
                }
                else
                {
                    throw new ArgumentException("The length of additionalProperties and additionalValues should be equal");
                }
            }

            return result;
        } 

        #endregion Private methods.


        #region Private fields.

        private DependencyProperty _dependencyProperty;
        private string _fontFamilyName;        
        private object _propertyValue;
        private string _textContent;
        private bool _isSimulated;
        private CultureInfo _cultureInfo;
        private DependencyProperty[] _additionalProperties;
        private object[] _additionalValues;

        //contains unicode characters
        private const string _capitalsString = "\x201cOn Golden Pond\x201d opens to rave reviews ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz " +
            "\x0391\x0392\x0393\x0394\x0395\x0396\x0398\x0399\x039a\x039b\x039c\x039d\x039e\x039f\x03a0\x03a1\x03a3\x03a4\x03a5\x03a6\x03a7\x03a8\x03a9 " +
            "\x03b1\x03b2\x03b3\x03b4\x03b5\x03b6\x03b7\x03b8\x03b9\x03ba\x03bb\x03bd\x03be\x03bf\x03c0\x03c1\x03c2\x03c3\x03c4\x03c5\x03c6\x03c7\x03c8\x03c9 " +
            "\x0405\x0406\x0408\x0409\x040a\x040b\x040c\x040f\x0410\x0411\x0412\x0413\x0414\x0415\x0416\x0417\x0418\x0419\x041a\x041b\x041c\x041d\x041e\x041f" +
            "\x0420\x0421\x0422\x0423\x0424\x0425\x0426\x0427\x0428\x0429\x042a\x042b\x042c\x042d\x042e\x042f " +
            "\x0430\x0431\x0432\x0433\x0434\x0435\x0436\x0437\x0438\x0439\x043a\x043b\x043c\x043d\x043e\x043f" +
            "\x0440\x0441\x0442\x0443\x0444\x0445\x0446\x0447\x0448\x0449\x044a\x044b\x044c\x044d\x044e\x044f\x0455";

        private const string _allCharNumString = "abcdefghijklmnopqrstuvwxyz 1234567890 ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _contextualLigatureString = "ContextualLigatures fi fl ffi ffl fj ffj fb ffb ff ffh fk ffk";
        private const string _discretionaryLigatureString = "DiscretionaryLigatures ct st tz Qu ch ck Th ss";
        private const string _standardLigatureString = "StandardLigatures fi fl ffi ffl fj ffj fb ffb ff ffh fk ffk effluent efficient fight flour fjord";
        private const string _historicalLigatureString = "HistoricalLigatures ct st sp active antispam";
        private const string _kerningString = "Kerning To Ta Tc Te Tu AV AW VA WA Vo Va Ve Wa Wo We ow ov vo wo AVENUE avenue Toby Type avenue adventure OVATION";

        //contains unicode characters
        private const string _mathematicalGreekString = "MathematicalGreek \x03b2\x03b3\x03b5\x0393\x0394\x0398\x03a3\x03b1\x03a9 " +
            "\x03a4\x03b1 \x03c0\x03bb\x03bb\x03bd\x03c0\x03c1\x03c9 \x03c0\x03c5\x03b3\x03ba\x03c4\x03b9 \x03c7\x03c1\x03b7\x03c3\x03bc";

        private const string _fractionString = "Fractions 1/2 1/3 1/4 1/5 1/8 1/16 1/32 1/64 3/64 2/5 3/4 5/8 6/9 32/56 25/200";
        private const string _numeralString = "12344567890";

        #endregion Private fields.

        #region Unit Tests.

        #if false
        
        private static int currentIndex;
        private static RichTextBox rtb;
        private static Window win;

        [STAThread]
        private static void Main()
        {
            Application app;            
            
            app = new Application();
            currentIndex = -1;
            app.Startup += new StartupEventHandler(app_Startup);
            app.Run();
        }

        static void app_Startup(object sender, StartupEventArgs e)
        {
            Button next, prev;
            StackPanel panel;                       
                        
            win = new Window();
            panel = new StackPanel();
            rtb = new RichTextBox();
            next = new Button();
            prev = new Button();

            next.Click += new RoutedEventHandler(next_Click);
            prev.Click += new RoutedEventHandler(prev_Click);
            
            rtb.FontSize = 24;
            rtb.Height = 300;
            win.Text = "Click on the Next button to view each Typographical Feature";
            next.Content = "Next";
            prev.Content = "Prev";
                                                          
            win.Content = panel;
            panel.Children.Add(rtb);
            panel.Children.Add(next);
            panel.Children.Add(prev);
            win.Show();
        }

        static void prev_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < 1)
            {
                return;
            }

            currentIndex--;

            rtb.ContentStart.DeleteContentToPosition(rtb.ContentEnd);            
            rtb.ContentEnd.InsertContentElement(Values[currentIndex].GetSampleParagraph());
            win.Text = "Index[" + currentIndex.ToString() + "/" + (Values.Length - 1) + "] " +
                Values[currentIndex].DependencyProperty.ToString() + " - " +
                Values[currentIndex].PropertyValue.ToString();              
        }

        static void next_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex < Values.Length - 1)
            {
                currentIndex++;
            }
            else
            {                
                return;
            }

            rtb.ContentStart.DeleteContentToPosition(rtb.ContentEnd);
            rtb.ContentEnd.InsertContentElement(Values[currentIndex].GetSampleParagraph());
            win.Text = "Index[" + currentIndex.ToString() + "/" + (Values.Length - 1) + "] " +
                Values[currentIndex].DependencyProperty.ToString() + " - " +
                Values[currentIndex].PropertyValue.ToString();                      
        }

        #endif

        #endregion Unit Tests.
    }
}