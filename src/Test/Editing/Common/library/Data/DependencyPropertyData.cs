// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides data to be used for testing DependencyProperties of TextBox

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Data/DependencyPropertyData.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections;    
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;    // for XmlLanguage
    using System.Windows.Media;
    using System.Windows.Threading;

    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Determines where a property value is coming from.</summary>
    public enum PropertyValueSource
    {
        /// <summary>The value is set locally on the element.</summary>
        LocalValueSet,
        /// <summary>The value is inherited from the parent of the element.</summary>
        ParentInherited,
        /// <summary>The value is set from the element style.</summary>
        StyleSet,
        /// <summary>The value is set from the property default.</summary>
        PropertyDefault
    }

    /// <summary>
    /// Provides information about interesting DependencyProperties of
    /// TextBox and RichTextBox.
    /// </summary>
    public sealed class DependencyPropertyData
    {

        #region Constructors.

        /// <summary>Hide the constructor.</summary>
        private DependencyPropertyData() { }

        #endregion Constructors.


        #region Public methods.

        /// <summary>
        /// Verifies that targetVisual or one of its children has the DependencyProperty
        /// with expectedValue.
        /// </summary>
        /// <param name="targetVisual">Visual to start search from</param>
        /// <param name="expectedValue">Expected value for the DependencyProperty</param>
        /// <returns>
        /// Returns the instance of Visual which has the expectedValue
        /// for the dependencyProperty. Returns null if it cannot find.
        /// </returns>
        public object FindElementForProperty(DependencyObject targetVisual, object expectedValue)
        {
            object propertyFoundIn = null;

            object checkValue = targetVisual.GetValue(Property);
            if (checkValue != null)
            {
                if (checkValue.ToString() == expectedValue.ToString())
                {
                    return targetVisual;
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(targetVisual);

            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject childVisual = VisualTreeHelper.GetChild(targetVisual, i);

                propertyFoundIn = FindElementForProperty(childVisual, expectedValue);
                if (propertyFoundIn != null)
                {
                    return propertyFoundIn;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the list of all (Character and Block) formatting properties.
        /// </summary>
        /// <returns>Array of DependencyPropertyData which are formatting properties</returns>
        public static DependencyPropertyData[] GetFormattingProperties()
        {
            ArrayList list;
  
            list = new ArrayList(CombinePropertyDataArrays(ParagraphPropertyData, InlinePropertyData));
            
            for (int i = 0; i < list.Count; i++)
            {
                //remove some properties from paragraph && inline that are not formating properties.
                if (((DependencyPropertyData)list[i]).Property == Block.ClearFloatersProperty ||
                    ((DependencyPropertyData)list[i]).Property == Block.IsHyphenationEnabledProperty ||
                    ((DependencyPropertyData)list[i]).Property == Block.LineStackingStrategyProperty )
                {
                    list.RemoveAt(i);
                    i--;
                }
            }

            return (DependencyPropertyData[])list.ToArray(typeof(DependencyPropertyData));
        }

       
        /// <summary>
        /// Gets the list of all character formatting properties. Taken from TextSchema.cs
        /// </summary>
        /// <returns>Array of DependencyPropertyData which are character formatting properties</returns>
        public static DependencyPropertyData[] GetCharacterFormattingProperties()
        {
            return InlinePropertyData;
        }

        /// <summary>
        /// Get all DependencyPropertyData values that apply to text serialization.
        /// </summary>
        /// <param name='defaultValues'>Object to get default values from.</param>
        /// <returns>DependencyPropertyData array</returns>
        public static DependencyPropertyData[] GetForTextSerialization(DependencyObject defaultValues)
        {
            DependencyPropertyData[] result;

            if (defaultValues == null)
            {
                throw new ArgumentNullException("defaultValues");
            }

            result = new DependencyPropertyData[s_textSerializationProperties.Length];
            for (int i = 0; i < s_textSerializationProperties.Length; i++)
            {
                object defaultValue;

                defaultValue = defaultValues.GetValue(s_textSerializationProperties[i]);
                result[i] = CreateDPData(s_textSerializationProperties[i],
                    defaultValue, GetDifferentValue(s_textSerializationProperties[i], defaultValue));
            }

            return result;
        }

        /// <summary>
        /// Determines what the source is for the given property on the
        /// specified element.
        /// </summary>
        /// <param name="element">Element for which to determine value source.</param>
        /// <param name="property">Property being evaluated.</param>
        /// <returns>The source for the value of the given property on the element.</returns>
        /// <remarks>
        /// This method is not an exact clone of the property engine value
        /// resolution, and will return incorrect results for the following
        /// cases:
        /// - values inherited from the visual (not logical) tree
        /// </remarks>
        public static PropertyValueSource GetPropertyValueSource(
            DependencyObject element, DependencyProperty property)
        {
            DependencyObject parent;
            Style elementStyle=null;
            Style parentStyle=null;            

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (IsLocalValueSet(element, property))
            {
                return PropertyValueSource.LocalValueSet;
            }

            //Get the style if it is set on the element.
            elementStyle = ((FrameworkElement)element).Style;
            //Get the default theme style. Using reflection to get the default theme style as
            //there is not public API to get that value (
            if (elementStyle == null)
            {
                elementStyle = (Style)ReflectionUtils.GetProperty((FrameworkElement)element, "ThemeStyle");
            }

            // Check whether the value is set on the element's style.
            if (element is FrameworkElement &&
                elementStyle != null &&
                IsStyleValueSet(elementStyle, property))
            {
                return PropertyValueSource.StyleSet;
            }
            
            if (GetIsInheritable(property))
            {
                // NOTE: adapt to traverse the visual tree is required.
                parent = element;
                while (parent != null && parent is FrameworkElement)
                {
                    parent = ((FrameworkElement)parent).Parent;
                    if (parent != null && IsLocalValueSet(parent, property))
                    {
                        return PropertyValueSource.ParentInherited;
                    }
                }

                // Check whether the value is set on one of the parent's style.
                // NOTE: adapt to traverse the visual tree is required.
                parent = element;
                while (parent != null && parent is FrameworkElement)
                {
                    parent = ((FrameworkElement)parent).Parent;

                    if (parent != null)
                    {
                        //Get the style if it is set on the parent.
                        parentStyle = ((FrameworkElement)parent).Style;
                        //Get the default theme style. Using reflection to get the default theme style as
                        //there is not public API to get that value (
                        if (parentStyle == null)
                        {
                            parentStyle = (Style)ReflectionUtils.GetProperty((FrameworkElement)parent, "ThemeStyle");
                        }
                    }

                    if (parent != null &&
                        parent is FrameworkElement &&
                        parentStyle != null &&
                        IsStyleValueSet(parentStyle, property))
                    {
                        return PropertyValueSource.ParentInherited;
                    }
                }
            }

            // If the value is not set anywhere, the default applies.
            return PropertyValueSource.PropertyDefault;
        }

        /// <summary>
        /// Get all DependencyProperties from TextBoxBase, TextBox, and RichTextBox
        /// </summary>
        /// <param name="elementType">Type of the element for which DependencyProperties are needed(TextBox,RichTextBox)</param>
        /// <returns>DependencyPropertyData array</returns>
        public static DependencyPropertyData[] GetDPDataForControl(Type elementType)
        {
             if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            // All of the TextBox property
            if (typeof(TextBox).IsAssignableFrom(elementType))
            {
                return TextBoxPropertyData;
            }

            // All the RichTextBox property
            else if (typeof(RichTextBox).IsAssignableFrom(elementType))
            {
                return RichTextBoxPropertyData;
            }
            // All the PasswordBox property
            else if (typeof(PasswordBox).IsAssignableFrom(elementType))
            {
                return PasswordBoxPropertyData;
            }
            else
            {
                throw new ApplicationException("Use TextBox, RichTextBox, or PasswordBox only.");
            }
        }

        /// <summary>
        /// Determines whether the given property is set locally on
        /// the specified element.
        /// </summary>
        /// <param name="element">Element to evaluate for local property set.</param>
        /// <param name="property">Property to evaluate for local property set.</param>
        /// <returns>
        /// true if the value for property is set locally on element;
        /// false otherwise.
        /// </returns>
        public static bool IsLocalValueSet(DependencyObject element, DependencyProperty property)
        {
            LocalValueEnumerator enumerator;    // Local value store enumerator.

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            enumerator = element.GetLocalValueEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Property == property)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the given property is set on the specified style.
        /// </summary>
        /// <param name="style">Style to evaluate for property application.</param>
        /// <param name="property">Property to evaluate for property application.</param>
        /// <returns>
        /// true if the value for property is set on the style setter;
        /// false otherwise.
        /// </returns>
        /// <remarks>
        /// This method does not take into account animation setters or property
        /// set on template elements.
        /// </remarks>
        public static bool IsStyleValueSet(Style style, DependencyProperty property)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            do
            {
                foreach (SetterBase setter in style.Setters)
                {
                    if (setter is Setter && ((Setter)setter).Property == property)
                    {
                        return true;
                    }
                }
                style = style.BasedOn;
            } while (style != null);

            return false;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns>Returns the string representation of the object</returns>
        public override string ToString()
        {
            if (DefaultValue == null)
            {
                return Property.ToString() + " - DefaultValue: " + "null"
                + " TestValue: " + TestValue.ToString();
            }
            else
            {
                return Property.ToString() + " - DefaultValue: " + DefaultValue.ToString()
                + " TestValue: " + TestValue.ToString();
            }
            
        }

        #endregion


        #region Public properties.

        /// <summary>Default value for the DependencyProperty.</summary>
        public object DefaultValue
        {
            get
            {
                //if (_defaultValue != null && _defaultValue is System.Windows.GetValueOverride)
                //{
                //    return ((System.Windows.GetValueOverride)_defaultValue)(null);
                //}
                //else
                //{
                    return _defaultValue;
                //}
            }
        }

        /// <summary>Whether the dependency property is inheritable.</summary>
        public bool IsInheritable
        {
            get
            {
                return GetIsInheritable(_property);
            }
        }

        /// <summary>Whether the dependency property is Inline property.</summary>
        public bool IsInlineFormattingProperty
        {
            get
            {
                DependencyPropertyData[] inlineFormattingProperties = GetCharacterFormattingProperties();
                for (int i = 0; i < inlineFormattingProperties.Length; i++)
                {
                    if (inlineFormattingProperties[i].Property == this.Property)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>DependencyProperty for test.</summary>
        public DependencyProperty Property
        {
            get { return _property; }
        }

        /// <summary>A sample custom value for the DependencyProperty.</summary>
        public object TestValue
        {
            get { return _testValue; }
        }

        /// <summary>Multiple custom values for the DependencyProperty.</summary>
        public object[] TestValues
        {
            get { return _testValues; }
        }

        #endregion


        #region Private methods.

        /// <summary>Creates a TextEffectCollection with the specified effect.</summary>
        private static TextEffectCollection CreateCollectionForEffect(TextEffect effect)
        {
            TextEffectCollection result;

            result = new TextEffectCollection();
            result.Add(effect);

            return result;
        }

        /// <summary>Initializes a new DependencyPropertyData object with a sample value.</summary>
        public static DependencyPropertyData CreateDPData(DependencyProperty property,
            object defaultValue, object testValue)
        {
            DependencyPropertyData dpData = new DependencyPropertyData();

            dpData._property = property;
            dpData._defaultValue = defaultValue;
            dpData._testValue = testValue;
            dpData._testValues = new object[] { testValue };

            return dpData;
        }

        /// <summary>Initializes a new DependencyPropertyData object with multiple sample values.</summary>
        private static DependencyPropertyData FromValues(DependencyProperty property,
            object defaultValue, object[] testValues)
        {
            DependencyPropertyData dpData = new DependencyPropertyData();

            dpData._property = property;
            dpData._defaultValue = defaultValue;
            dpData._testValue = testValues[0];
            dpData._testValues = testValues;

            return dpData;
        }

        
        /// <summary>
        /// Initializes a new DependencyPropertyData object with
        /// multiple sample values and a callback-based default.
        /// 
        /// Note: After Property Engine Tightening Work Item, there is no longer GetValueOverride 
        /// </summary>
        private static DependencyPropertyData FromCallback(DependencyProperty property,
            System.Windows.CoerceValueCallback callback, object[] testValues)
        {
            DependencyPropertyData dpData = new DependencyPropertyData();

            dpData._property = property;
            dpData._defaultValue = callback;
            dpData._testValue = testValues[0];
            dpData._testValues = testValues;

            return dpData;
        }
         

        /// <summary>Gets the default border brush for the current theme.</summary>
        /// <returns>The default border brush for the current theme.</returns>
        private static object GetDefaultBorderBrush(DependencyObject target, object baseValue)
        {
            // Get the brush through reflection from the theme-specific library.

            if (Win32.IsThemeActive())
            {
                // Get the brush through reflection from the theme-specific library.
                const string ThemeBorderKey = "PFThemeListBorder";
                return (Brush)System.Windows.Application.Current.FindResource(ThemeBorderKey);
            }
            else
            {
                //
                // This will be a dummy brush named ClassicBorderBrush on
                // Microsoft.Windows.Themes.ClassicBorderDecorator, in
                // PresentationFramework.Classic.dll.
                //
                Type classicBorderDecoratorType;

                classicBorderDecoratorType = ReflectionUtils.FindType("Microsoft.Windows.Themes.ClassicBorderDecorator");
                return (Brush) ReflectionUtils.GetStaticProperty(classicBorderDecoratorType, "ClassicBorderBrush");
            }
        }

        /// <summary>Gets the default border thickness for the current theme.</summary>
        private static object GetDefaultBorderThickness(DependencyObject target, object baseValue)
        {
            if (Win32.IsThemeActive())
            {
                return new Thickness(1, 1, 1, 1);
            }
            else
            {
                // This is Windows Classic.
                return new Thickness(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the default MinHeight of TextBoxBase for the current theme.
        /// Look at the theme file for values
        /// </summary>
        private static double GetDefaultMinHeightForTBB()
        {
            return 0.0;

            /* ********************************************************
             * MinHeight triggers are removed with a breaking change. Keeping the below commented code
             * as an example for how to determine current theme.
             * ********************************************************
             */
            //if (string.Compare(Win32.SafeGetCurrentThemeName(), "Aero", true, CultureInfo.InvariantCulture)==0)
            //{
            //    return (double)23.0;
            //}
            //else
            //{
            //    return (double)20.0;
            //}            
        }

        /// <summary>
        /// Gets the default MinWidth of TextBoxBase for the current theme.
        /// Look at the theme file for values
        /// </summary>
        private static double GetDefaultMinWidthForTBB()
        {
            return 0.0;

            /* ********************************************************
             * MinHeight triggers are removed with a breaking change. Keeping the below commented code
             * as an example for how to determine current theme.
             * ********************************************************
             */

            //if (string.Compare(Win32.SafeGetCurrentThemeName(), "Aero", true, CultureInfo.InvariantCulture) == 0)
            //{
            //    return (double)189.0;
            //}
            //else
            //{
            //    return (double)100.0;
            //}
        }

        /// <summary>Gets the default padding for the current theme.</summary>
        private static object GetDefaultPadding(DependencyObject target, object baseValue)
        {
            if (Win32.IsThemeActive())
            {
                return new Thickness(4, 5, 4, 5);
            }
            else
            {
                return new Thickness(4, 1, 4, 1);
            }
        }

        /// <summary>Gets a value for the specified property, different from the one supplied.</summary>
        private static object GetDifferentValue(DependencyProperty property, object value)
        {
            SampleValue sampleValue;

            sampleValue = SampleValue.GetForProperty(property);

            return (sampleValue.Sample == value)? sampleValue.OtherSample : sampleValue.Sample;
        }

        /// <summary>Gets a TextEffectCollection to produce displaced text.</summary>
        private static TextEffectCollection GetDisplacedEffectCollection()
        {
            return CreateCollectionForEffect(
                new TextEffect(new TranslateTransform(2, 0), Brushes.Blue, null, 0, 2));
        }

        /// <summary>Checks whether the specified property is inheritable.</summary>
        private static bool GetIsInheritable(DependencyProperty property)
        {
            return property.DefaultMetadata is FrameworkPropertyMetadata &&
                ((FrameworkPropertyMetadata)property.DefaultMetadata).Inherits;
        }

        /// <summary>Gets a TextEffectCollection to produce tilted text.</summary>
        private static TextEffectCollection GetTiltedEffectCollection()
        {
            return CreateCollectionForEffect(
                new TextEffect(new SkewTransform(45, 0), Brushes.Green, null, 0, 2));
        }

        #endregion
        #region public method

        /// <summary>
        /// Retrun an array of DP in a propertyData array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DependencyProperty[] GetPropertiesFromPropertyData(DependencyPropertyData[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", "Data Array should not be null!");
            }
            if (data.Length == 0)
            {
                throw new ArgumentException("Data array should not be empty!", "data");
            }
            
            DependencyProperty[] result = new DependencyProperty[data.Length];
            
            for(int i = 0; i<data.Length; i++)
            {
                result[i] = data[i].Property;
            }
            return result; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static DependencyPropertyData[] CombinePropertyDataArrays(DependencyPropertyData[] array1, DependencyPropertyData[] array2)
        {
            DependencyPropertyData[] result = new DependencyPropertyData[array1.Length + array2.Length];

            Array.Copy(array1, 0, result, 0, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }
        #endregion

        #region default property data for editing purpose.

        /// <summary>Return property data for TextElement</summary>
        public static DependencyPropertyData[] TextElementPropertyData = new DependencyPropertyData[]
        {
            /*** TextElement inheritable properties ***/
            CreateDPData(TextElement.BackgroundProperty, Brushes.White, Brushes.LightBlue),
            //CreateDPData(TextElement.TextEffectsProperty, null, null),
            CreateDPData(FrameworkElement.LanguageProperty, XmlLanguage.GetLanguage("en-US"), XmlLanguage.GetLanguage("ar-SA")),
            CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
            CreateDPData(NumberSubstitution.CultureSourceProperty, NumberCultureSource.User, NumberCultureSource.Text),
            CreateDPData(NumberSubstitution.SubstitutionProperty, NumberSubstitutionMethod.AsCulture, NumberSubstitutionMethod.Context),
            CreateDPData(NumberSubstitution.CultureOverrideProperty, null, CultureInfo.CreateSpecificCulture("ar-SA")),
            CreateDPData(TextElement.FontFamilyProperty, System.Windows.SystemFonts.MessageFontFamily, new System.Windows.Media.FontFamily("Arial")),
            CreateDPData(TextElement.FontStyleProperty, FontStyles.Normal, FontStyles.Italic),
            CreateDPData(TextElement.FontWeightProperty, FontWeights.Normal, FontWeights.Bold),
            CreateDPData(TextElement.FontStretchProperty, FontStretch.FromOpenTypeStretch(5), FontStretch.FromOpenTypeStretch(8)), //5=normal, 8=ExtraExpanded
            CreateDPData(TextElement.FontSizeProperty, (double)11, (double)24),
            CreateDPData(TextElement.ForegroundProperty, Brushes.Black, Brushes.Blue),       
        };

        /// <summary>Return property data for Inline</summary>
        public static DependencyPropertyData[] InlinePropertyData
        {
            get
            {
                //Properties defined on Inline
                DependencyPropertyData[] InlineDefinedPropertyData;
                InlineDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(Inline.BaselineAlignmentProperty, BaselineAlignment.Baseline, BaselineAlignment.Superscript),
                    CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                    CreateDPData(Inline.TextDecorationsProperty, new TextDecorationCollection(), TextDecorations.Underline),

                };
                return CombinePropertyDataArrays(InlineDefinedPropertyData, TextElementPropertyData);
            }
        }

        /// <summary>return all propertyData for Block.</summary>
        public static DependencyPropertyData[] BlockPropertyData
        {
            get
            {
                //Properties defined on Block
                DependencyPropertyData[] BlockDefinedPropertyData;
                BlockDefinedPropertyData = new DependencyPropertyData[]
                {
                    FromCallback(Block.BorderBrushProperty, GetDefaultBorderBrush, new object[] { Brushes.HotPink }),
                    FromCallback(Block.BorderThicknessProperty, GetDefaultBorderThickness, new object[] { new Thickness(3,3,3,3) }),
                    CreateDPData(Block.ClearFloatersProperty, WrapDirection.Left, WrapDirection.Right),
                    CreateDPData(Block.LineHeightProperty, double.NaN, 15d), 
                    CreateDPData(Block.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight, LineStackingStrategy.MaxHeight),
                    CreateDPData(Block.IsHyphenationEnabledProperty, false, true),
                    CreateDPData(Block.MarginProperty, new Thickness(0d), new Thickness(5d)),
                    FromCallback(Block.PaddingProperty, GetDefaultPadding, SampleValue.GetValuesForProperty(TextBox.PaddingProperty)),
                    CreateDPData(Block.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                };
                return CombinePropertyDataArrays(BlockDefinedPropertyData, TextElementPropertyData);
            }
        }

        /// <summary>Return property data for Run</summary>
        public static DependencyPropertyData[] RunPropertyData
        {
            get
            {
                //Run:Inline
                //No local defined properties.
                return InlinePropertyData;
            }
        }

        /// <summary>Return property data for InlineUIContainer</summary>
        public static DependencyPropertyData[] InlineUIContainerPropertyData
        {
            get
            {
                //InlineUIContainer:Inline
                //No Local defined properties.
                return InlinePropertyData;
            }
        }
        /// <summary>Return property data for BlockIContainer</summary>
        public static DependencyPropertyData[] BlockUIContainerPropertyData
        {
            get
            {
                //BlockUIContainer:Block
                //No local defined properties.
                return BlockPropertyData;
            }
        }
        /// <summary>return  All hyperlink properties.</summary>
        public static DependencyPropertyData[] HyperlinkPropertyData
        {
            get
            {
                DependencyPropertyData[] HyperlinkDefinedPropertyData;
                HyperlinkDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(Hyperlink.CommandProperty, /*ICommand*/ null, /*ICommand*/null),
                    CreateDPData(Hyperlink.CommandParameterProperty,/*object*/null, /*object*/null),
                    CreateDPData(Hyperlink.CommandTargetProperty, /*IinputElement*/null, /*IInputElement*/ null),
                    CreateDPData(Hyperlink.NavigateUriProperty, /*Uri*/ null, new Uri("http://www.Msn.com")),
                    CreateDPData(Hyperlink.TargetNameProperty, /*string*/string.Empty, "TestTarget"),
                };

                //Hyperlink:Span:Inline
                //Hyperlink defined PropertyData + Inine propertyData (note: Span does not define any)
                return CombinePropertyDataArrays(HyperlinkDefinedPropertyData, InlinePropertyData);
            }
        }

        /// <summary>
        /// Return all propertyData for a Paragraph.
        /// </summary>
        public static DependencyPropertyData[] ParagraphPropertyData
        {
            get
            {
                //Properties defined on Paragraph.
                DependencyPropertyData[] ParagrapDefinedhPropertyData;

                ParagrapDefinedhPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(Paragraph.MinOrphanLinesProperty, 0, 2),
                    CreateDPData(Paragraph.MinWidowLinesProperty, 0, 2),
                    CreateDPData(Paragraph.TextIndentProperty, 0d, 2d),
                    CreateDPData(Paragraph.KeepTogetherProperty, false, true),
                    CreateDPData(Paragraph.KeepWithNextProperty, false, true),
                    CreateDPData(Paragraph.TextDecorationsProperty, new TextDecorationCollection(), TextDecorations.Underline),
                };

                //Paragraph:Block
                //Block properties + Paragraph defined Properties
                return CombinePropertyDataArrays(BlockPropertyData, ParagrapDefinedhPropertyData);
            }
        }

        /// <summary> Return all propertyData for a List.</summary>
        public static DependencyPropertyData[] ListPropertyData
        {
            get
            {
                //PropertyData defined on List
                DependencyPropertyData[] ListDefinedPropertyData;
                ListDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(List.MarkerStyleProperty,TextMarkerStyle.Disc, TextMarkerStyle.Box),
                    CreateDPData(List.MarkerOffsetProperty, Double.NaN, 10.0), 
                    CreateDPData(List.StartIndexProperty, 1, 2),
                };

                //List:Block
                //Block Properties + List defined properties.
                return CombinePropertyDataArrays (BlockPropertyData, ListDefinedPropertyData);
            }
        }

        /// <summary>Return all properities for a ListItem.</summary>
        public static DependencyPropertyData[] ListItemPropertyData
        {
            get
            {
                //Properties Defined on ListItem.
                DependencyPropertyData[] ListItemDefinedPropertyData;
                ListItemDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(ListItem.MarginProperty, new Thickness(0d), new Thickness(5d)),
                    CreateDPData(ListItem.PaddingProperty, new Thickness(0,0,0,0), new Thickness(1,1,1,1)), 
                    CreateDPData(ListItem.BorderThicknessProperty, new Thickness(0,0,0,0),new Thickness(3,3,3,3)),
                    FromCallback(ListItem.BorderBrushProperty, GetDefaultBorderBrush, new object[] { Brushes.HotPink }),
                    CreateDPData(Block.LineHeightProperty, double.NaN, 15d), 
                    CreateDPData(Block.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight, LineStackingStrategy.MaxHeight),
                    CreateDPData(Block.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),               
                };

                //ListItem:TextElement
                //TextElement properties + ListItem Defined properties
                return CombinePropertyDataArrays(TextElementPropertyData, ListItemDefinedPropertyData);
            }
        }

        /// <summary>Return property data for FlowDocument </summary>
        public static DependencyPropertyData[] FlowDocumentPropertyData
        {
            get{
                DependencyPropertyData[] FlowDocumentPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(Block.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    CreateDPData(Block.LineHeightProperty, double.NaN, 15d),
                    
                    //TextElement inheritable properties
                    CreateDPData(FrameworkElement.LanguageProperty, XmlLanguage.GetLanguage("en-US"), XmlLanguage.GetLanguage("ar-SA")),
                    CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                    CreateDPData(NumberSubstitution.CultureSourceProperty, NumberCultureSource.User, NumberCultureSource.Text),
                    CreateDPData(NumberSubstitution.SubstitutionProperty, NumberSubstitutionMethod.AsCulture, NumberSubstitutionMethod.Context),
                    CreateDPData(NumberSubstitution.CultureOverrideProperty, null, CultureInfo.CreateSpecificCulture("ar-SA")),
                    
                    //Font related
                    CreateDPData(TextElement.FontFamilyProperty, System.Windows.SystemFonts.MessageFontFamily, new System.Windows.Media.FontFamily("Arial")),
                    CreateDPData(TextElement.FontStyleProperty, FontStyles.Normal, FontStyles.Italic),
                    CreateDPData(TextElement.FontWeightProperty, FontWeights.Normal, FontWeights.Bold),
                    CreateDPData(TextElement.FontStretchProperty, FontStretch.FromOpenTypeStretch(5), FontStretch.FromOpenTypeStretch(8)), //5=normal, 8=ExtraExpanded
                    CreateDPData(TextElement.FontSizeProperty, (double)11, (double)24),
                                        
                    CreateDPData(FlowDocument.IsOptimalParagraphEnabledProperty, false, true),
                    CreateDPData(FlowDocument.PagePaddingProperty, Double.NaN, new Thickness(5, 5, 5, 5)),
                    CreateDPData(Block.IsHyphenationEnabledProperty, false, true),
                   
                    //Background, foreground.
                    CreateDPData(TextElement.ForegroundProperty, Brushes.Black, Brushes.DarkBlue),
                    CreateDPData(TextElement.BackgroundProperty, Brushes.Transparent, Brushes.LightBlue),
                };
                return FlowDocumentPropertyData; 
            }
        }

        /// <summary>
        /// return all properityData for Table.
        /// </summary>
        /// <returns></returns>
        public static DependencyPropertyData[] TablePropertyData
        {
            get
            {
                //Properties defined on Table.
                DependencyPropertyData[] TableDefinedPropertyData;
                TableDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(Table.CellSpacingProperty, 0 , 5.0),
                };
                //Table:Block
                //Table defined Properties + Block Properties
                return CombinePropertyDataArrays(BlockPropertyData, TableDefinedPropertyData);
            }
        }

        /// <summary>Return all properityData for TableGroup.</summary>

        public static DependencyPropertyData[] TableRowGroupPropertyData
        {
            get
            {
                //TableRowGroup does not define any property.
                //TableGroupRow:TextElement
                return TextElementPropertyData;
            }
        }

        /// <summary>Return all properitydata for TableRow. </summary>
        public static DependencyPropertyData[] TableRowPropertyData
        {
            get
            {
                //TableRow does not defined any DP.
                //TableRow:TextElement
                return TextElementPropertyData;
            }
        }

        /// <summary>Return all properities for TableCell.</summary>
        public static DependencyPropertyData[] TableCellPropertyData
        {
            get
            {
                //Properties defined on TableCell
                DependencyPropertyData[] TableCellDefinedPropertyData;
                TableCellDefinedPropertyData = new DependencyPropertyData[]
                {
                    FromCallback(Block.BorderBrushProperty, GetDefaultBorderBrush, new object[] { Brushes.HotPink }),
                    FromCallback(Block.BorderThicknessProperty, GetDefaultBorderThickness, new object[] { new Thickness(3,3,3,3) }),
                    CreateDPData(Block.LineHeightProperty, double.NaN, 15d), 
                    CreateDPData(Block.LineStackingStrategyProperty, LineStackingStrategy.BlockLineHeight, LineStackingStrategy.MaxHeight),
                    CreateDPData(Block.MarginProperty, new Thickness(0d), new Thickness(5d)),
                    FromCallback(Block.PaddingProperty, GetDefaultPadding, SampleValue.GetValuesForProperty(TextBox.PaddingProperty)),
                    CreateDPData(Block.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    CreateDPData(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                    
                    CreateDPData(TableCell.ColumnSpanProperty,0, 5),
                    CreateDPData(TableCell.RowSpanProperty, 0, 5),
                };

                //TableCell:TextElement
                //TableCell defined properties + TextElement Properites.
                return CombinePropertyDataArrays(TextElementPropertyData, TableCellDefinedPropertyData);
            }
        }

        /// <summary>
        /// return all properities for TableColumn.
        /// </summary>
        /// <returns></returns>
        public static DependencyPropertyData[] TableColumnPropertyData
        {
            get
            {
                //Properties defined on TableColumn
                DependencyPropertyData[] TableColumnDefinedPropertyData;
                TableColumnDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(TableColumn.WidthProperty, new GridLength(0.0), new GridLength(50.0)),
                    CreateDPData(TableColumn.BackgroundProperty, Brushes.White, Brushes.Red),
                };

                //TableColumn:FrameworkContentElement
                return TableColumnDefinedPropertyData;
            }
        }

        /// <summary>
        /// return all properities for TextBox.
        /// </summary>
        /// <returns></returns>
        public static DependencyPropertyData[] TextBoxPropertyData
        {
            get
            {
                //Properties defined on TextBox
                DependencyPropertyData[] TextBoxDefinedPropertyData;
                TextBoxDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(TextBox.MaxLengthProperty, 0, 10), 
                    CreateDPData(TextBox.MaxLinesProperty, Int32.MaxValue, 2),
                    CreateDPData(TextBox.MinHeightProperty, GetDefaultMinHeightForTBB(), 100.0),                    
                    CreateDPData(TextBox.MinLinesProperty, 1, 2), 
                    CreateDPData(TextBox.MinWidthProperty, GetDefaultMinWidthForTBB(), 200.0),
                    CreateDPData(TextBox.TextProperty, "", "This is a test"),
                    CreateDPData(TextBox.TextWrappingProperty, TextWrapping.NoWrap, TextWrapping.Wrap), 
                    CreateDPData(Block.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    
                    //Defined in TextBoxBase by the default value is different from RichTextBox.
                    CreateDPData(TextBox.AcceptsReturnProperty, false, true), 
                    CreateDPData(TextBoxBase.AutoWordSelectionProperty, false, true),       
                    CreateDPData(TextBox.CharacterCasingProperty, CharacterCasing.Normal, CharacterCasing.Lower), 
                };

                //TextBox:TextBoxBase
                return CombinePropertyDataArrays(TextBoxDefinedPropertyData, TextBoxBasePropertyData);
            }
        }

        /// <summary>return all properities for RichTextBox.</summary>
        public static DependencyPropertyData[] RichTextBoxPropertyData
        {
            get
            {
                //Properties defined on TextBox
                DependencyPropertyData[] RichTextBoxDefinedPropertyData;
                RichTextBoxDefinedPropertyData = new DependencyPropertyData[]
                {
                    //Those properties have different default value than TextBoxbase event though they are inherited from TextBoxBase.
                    CreateDPData(TextBox.AcceptsReturnProperty, true, false), 
                    CreateDPData(TextBoxBase.AutoWordSelectionProperty, true, false),       
                };

                //TextBox:TextBoxBase
                return CombinePropertyDataArrays(RichTextBoxDefinedPropertyData, TextBoxBasePropertyData);
            }
        }

        /// <summary>Return all Properties for PasswordBox</summary>
        public static DependencyPropertyData[] PasswordBoxPropertyData
        {
            get
            {
                DependencyPropertyData[] PasswordBoxDefinedPropertyData;
                PasswordBoxDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(PasswordBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Left, HorizontalAlignment.Right),
                    CreateDPData(PasswordBox.MarginProperty, new Thickness(0,0,0,0), new Thickness(50,50,50,50)),
                    CreateDPData(PasswordBox.MinHeightProperty, 0.0, 100.0),
                    CreateDPData(PasswordBox.MaxLengthProperty, 0, 200),
                    CreateDPData(PasswordBox.MinWidthProperty, 0.0, 200.0),
                    CreateDPData(PasswordBox.PasswordCharProperty, (char)0x97, 'A'),
                    
                    //defined in UIElement, and tested for password.
                    CreateDPData(PasswordBox.AllowDropProperty, false, true),
                 };
                //Password:Control
                return CombinePropertyDataArrays(PasswordBoxDefinedPropertyData, ControlPropertyData);
            }
        }
        /// <summary>Return all Properties for PasswordBox</summary>
        public static DependencyPropertyData[] ControlPropertyData
        {
            get
            {
                DependencyPropertyData[] ControlDefinedPropertyData;
                ControlDefinedPropertyData = new DependencyPropertyData[]
                {
                      //inherited from base classes.
                    CreateDPData(Control.BackgroundProperty, System.Windows.SystemColors.WindowBrush, Brushes.Lavender),
                    CreateDPData(Control.ClipToBoundsProperty, false, true),
                    CreateDPData(Control.LanguageProperty, XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag), XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)),
                    CreateDPData(Control.CursorProperty, null, Cursors.Hand),
                    CreateDPData(Control.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                    CreateDPData(Control.FocusableProperty, true, false),
                    CreateDPData(Control.FontFamilyProperty, System.Windows.SystemFonts.MessageFontFamily, new FontFamily("Microsoft Sans Serif")),
                    CreateDPData(Control.FontSizeProperty, System.Windows.SystemFonts.MessageFontSize, 40.0),
                    CreateDPData(Control.FontStretchProperty, FontStretches.Normal, FontStretches.UltraExpanded),
                    CreateDPData(Control.FontStyleProperty, FontStyles.Normal, FontStyles.Italic),
                    CreateDPData(Control.FontWeightProperty, FontWeights.Normal, FontWeights.Thin),
                    CreateDPData(Control.ForceCursorProperty, false, true),
                    FromValues  (Control.ForegroundProperty, SystemColors.WindowTextBrush, BrushData.BrushValues),
                    CreateDPData(Control.HeightProperty, double.NaN, 100.0),
                    CreateDPData(Control.HorizontalAlignmentProperty, HorizontalAlignment.Stretch, HorizontalAlignment.Center),
                    CreateDPData(Control.IsEnabledProperty, true, false),
                    CreateDPData(Control.IsTabStopProperty, true, false),
                    CreateDPData(Control.MaxHeightProperty, double.PositiveInfinity, 100.0),
                    CreateDPData(Control.MaxWidthProperty, double.PositiveInfinity, 120.0),
                    CreateDPData(TextBox.MarginProperty, new Thickness(0,0,0,0), new Thickness(50,50,50,50)),
                    CreateDPData(Control.OpacityMaskProperty, null, Brushes.Green),
                    CreateDPData(Control.OpacityProperty, 1.0, 0.5),
                    CreateDPData(Control.TabIndexProperty, int.MaxValue, 10),
                    CreateDPData(TextBlock.TextAlignmentProperty, TextAlignment.Left, TextAlignment.Right),
                    CreateDPData(Control.ToolTipProperty, null, "This is tooltip"),
                    CreateDPData(Control.VerticalAlignmentProperty, VerticalAlignment.Stretch, VerticalAlignment.Center),
                    CreateDPData(Control.VerticalContentAlignmentProperty, VerticalAlignment.Top, VerticalAlignment.Bottom),
                    CreateDPData(Control.VisibilityProperty, Visibility.Visible, Visibility.Hidden),
                    CreateDPData(Control.WidthProperty, double.NaN, 200.0),
                 };

                return ControlDefinedPropertyData;
            }
        }
        /// <summary>
        /// return all properities for TableColumn.
        /// </summary>
        /// <returns></returns>
        public static DependencyPropertyData[] TextBoxBasePropertyData
        {
            get
            {
                //Properties defined on TableColumn
                DependencyPropertyData[] TextBoxBaseDefinedPropertyData;
                TextBoxBaseDefinedPropertyData = new DependencyPropertyData[]
                {
                    CreateDPData(TextBox.AcceptsTabProperty, false, true), 
                    CreateDPData(TextBox.AllowDropProperty, true, false),  
                    CreateDPData(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Left, HorizontalAlignment.Right),
                    CreateDPData(TextBox.IsReadOnlyProperty, false, true), 
                    CreateDPData(SpellCheck.IsEnabledProperty, false, true),
                    CreateDPData(TextBoxBase.IsUndoEnabledProperty, true, false),                                    
                    CreateDPData(TextBox.MarginProperty, new Thickness(0,0,0,0), new Thickness(50,50,50,50)),
                    CreateDPData(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible), 
                    CreateDPData(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Disabled), 
                };

                //TextBoxBase:Control
                return CombinePropertyDataArrays(TextBoxBaseDefinedPropertyData, ControlPropertyData);
            }
        }

        /// <summary>Return all Properties for Typography</summary>
        public static DependencyPropertyData[] TypographyPropertyData = new DependencyPropertyData[]
        {
            CreateDPData(Typography.StandardLigaturesProperty, true, false),
            CreateDPData(Typography.ContextualLigaturesProperty, true, false),
            CreateDPData(Typography.DiscretionaryLigaturesProperty, false, true),
            CreateDPData(Typography.HistoricalLigaturesProperty, false, true),

            CreateDPData(Typography.AnnotationAlternatesProperty, 0, 1),
            CreateDPData(Typography.ContextualAlternatesProperty, true, false),
            CreateDPData(Typography.HistoricalFormsProperty, false, true),

            CreateDPData(Typography.KerningProperty, true, false),
            CreateDPData(Typography.CapitalSpacingProperty, false, true),

            CreateDPData(Typography.StylisticSet1Property, false, true),
            CreateDPData(Typography.StylisticSet2Property, false, true),
            CreateDPData(Typography.StylisticSet3Property, false, true),
            CreateDPData(Typography.StylisticSet4Property, false, true),
            CreateDPData(Typography.StylisticSet5Property, false, true),
            CreateDPData(Typography.StylisticSet6Property, false, true),
            CreateDPData(Typography.StylisticSet7Property, false, true),
            CreateDPData(Typography.StylisticSet8Property, false, true),
            CreateDPData(Typography.StylisticSet9Property, false, true),
            CreateDPData(Typography.StylisticSet10Property, false, true),
            CreateDPData(Typography.StylisticSet11Property, false, true),
            CreateDPData(Typography.StylisticSet12Property, false, true),
            CreateDPData(Typography.StylisticSet13Property, false, true),
            CreateDPData(Typography.StylisticSet14Property, false, true),
            CreateDPData(Typography.StylisticSet15Property, false, true),
            CreateDPData(Typography.StylisticSet16Property, false, true),
            CreateDPData(Typography.StylisticSet17Property, false, true),
            CreateDPData(Typography.StylisticSet18Property, false, true),
            CreateDPData(Typography.StylisticSet19Property, false, true),


            CreateDPData(Typography.FractionProperty, "Normal", false),

            CreateDPData(Typography.SlashedZeroProperty, false, true),
            CreateDPData(Typography.MathematicalGreekProperty, false, true),
            CreateDPData(Typography.EastAsianExpertFormsProperty, false, true),

            CreateDPData(Typography.VariantsProperty, FontVariants.Normal, FontVariants.Ordinal),
            CreateDPData(Typography.CapitalsProperty, FontCapitals.Normal, FontCapitals.AllSmallCaps),
            CreateDPData(Typography.NumeralStyleProperty, FontNumeralStyle.Normal, FontNumeralStyle.OldStyle),
            CreateDPData(Typography.NumeralAlignmentProperty, FontNumeralAlignment.Normal, FontNumeralAlignment.Proportional),
            CreateDPData(Typography.EastAsianWidthsProperty, FontEastAsianWidths.Normal, FontEastAsianWidths.Full),
            CreateDPData(Typography.EastAsianLanguageProperty, FontEastAsianLanguage.Normal, FontEastAsianLanguage.Traditional),

            CreateDPData(Typography.StandardSwashesProperty, 0, 1),
            CreateDPData(Typography.ContextualSwashesProperty, 0, 1),
            CreateDPData(Typography.StylisticAlternatesProperty, 0, 1),
        };
        #endregion 


        #region Private fields.

        /// <summary>Dependency Property under test.</summary>
        private DependencyProperty _property;

        /// <summary>Default value for property.</summary>
        private object _defaultValue;

        /// <summary>Sample test value for property.</summary>
        private object _testValue;

        /// <summary>Sample test values for property.</summary>
        private object[] _testValues;

        private static DependencyProperty[] s_textSerializationProperties = new DependencyProperty[] {
            FrameworkElement.LanguageProperty,
            TextElement.FontFamilyProperty,
            TextElement.FontStyleProperty,
            TextElement.FontWeightProperty,
            TextElement.FontStretchProperty,
            TextElement.FontSizeProperty,
            TextElement.ForegroundProperty,
        };

        #endregion


        #region Inner types.

        class SampleValue
        {
            internal static SampleValue GetForProperty(DependencyProperty property)
            {
                if (property == null)
                {
                    throw new ArgumentNullException("property");
                }

                foreach(SampleValue result in s_sampleValues)
                {
                    if (result.Property == property)
                    {
                        return result;
                    }
                }
                throw new ArgumentException("SampleValue does not support property " + property);
            }

            internal static object[] GetValuesForProperty(DependencyProperty property)
            {
                SampleValue sample;

                sample = GetForProperty(property);

                return new object[] { sample.Sample, sample.OtherSample };
            }

            internal SampleValue(DependencyProperty property, object sample, object otherSample)
            {
                this.Property = property;
                this.Sample = sample;
                this.OtherSample = otherSample;
            }

            internal DependencyProperty Property;
            internal object Sample;
            internal object OtherSample;

            private static SampleValue[] s_sampleValues = new SampleValue[] {
                // Control dependency properties.
                new SampleValue(Control.PaddingProperty, new Thickness(1, 2, 3, 4), new Thickness(0, 0, 0, 0)),

                // FrameworkElement dependency properties.
                // new SampleValue(FrameworkElement.StyleProperty, null, bleargh),
                // new SampleValue(FrameworkElement.ThemeStyleKeyProperty, null, bleargh),
                // new SampleValue(FrameworkElement.DataContextProperty, null, bleargh),
                // new SampleValue(FrameworkElement.InputScopeProperty, null, bleargh),
                // new SampleValue(FrameworkElement.LayoutTransformProperty, null, bleargh),
                // new SampleValue(FrameworkElement.FocusVisualStyleProperty, null, bleargh),
                // new SampleValue(FrameworkElement.ContextMenuProperty, null, bleargh),

                new SampleValue(FrameworkElement.LanguageProperty, XmlLanguage.GetLanguage("en-US"), XmlLanguage.GetLanguage("fr-FR")),
                new SampleValue(FrameworkElement.NameProperty, "sample-name", "other-name"),
                new SampleValue(FrameworkElement.TagProperty, "sample-tag", "other-tag"),
                new SampleValue(FrameworkElement.WidthProperty, (double)60, (double)100),
                new SampleValue(FrameworkElement.HeightProperty, (double)60, (double)100),
                new SampleValue(FrameworkElement.MinWidthProperty, (double)40, (double)80),
                new SampleValue(FrameworkElement.MaxWidthProperty, (double)80, (double)120),
                new SampleValue(FrameworkElement.HeightProperty, (double)50, (double)70),
                new SampleValue(FrameworkElement.MinHeightProperty, (double)45, (double)65),
                new SampleValue(FrameworkElement.MaxHeightProperty, (double)55, (double)75),
                new SampleValue(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight, FlowDirection.RightToLeft),
                new SampleValue(FrameworkElement.MarginProperty, new Thickness(0,0,0,0), new Thickness(8,8,8,8)),
                new SampleValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left, HorizontalAlignment.Center),
                new SampleValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top, VerticalAlignment.Bottom),
                new SampleValue(FrameworkElement.CursorProperty, Cursors.Cross, Cursors.SizeAll),
                new SampleValue(FrameworkElement.ForceCursorProperty, true, false),
                new SampleValue(FrameworkElement.FocusableProperty, true, false),
                new SampleValue(FrameworkElement.ToolTipProperty, "sample-tip", "other-tip"),

                // TextElement dependency properties.
                new SampleValue(TextElement.BackgroundProperty, Brushes.Gray, Brushes.LightBlue),
                new SampleValue(TextElement.FontFamilyProperty, new FontFamily("Arial"), new FontFamily("Times New Roman")),
                new SampleValue(TextElement.FontSizeProperty, (double)10, (double)32),
                new SampleValue(TextElement.FontStretchProperty, FontStretches.Normal, FontStretches.Expanded),
                new SampleValue(TextElement.FontStyleProperty, FontStyles.Italic, FontStyles.Normal),
                new SampleValue(TextElement.FontWeightProperty, FontWeights.Bold, FontWeights.Normal),
                new SampleValue(TextElement.ForegroundProperty, Brushes.Green, Brushes.Blue),
                new SampleValue(TextElement.TextEffectsProperty, GetTiltedEffectCollection(), GetDisplacedEffectCollection()),

                // TextBox dependency properties.
                new SampleValue(TextBox.MinLinesProperty, 0, 2),
                new SampleValue(TextBox.MaxLinesProperty, 2, 3),
                new SampleValue(TextBox.TextProperty, "sample", "value"),
                new SampleValue(TextBox.CharacterCasingProperty, CharacterCasing.Upper, CharacterCasing.Lower),
                new SampleValue(TextBox.MaxLengthProperty, 12, 0),

                // TextBoxBase dependency properties.
                new SampleValue(TextBoxBase.AcceptsReturnProperty, true, false),
                new SampleValue(TextBoxBase.AcceptsTabProperty, true, false),
//                new SampleValue(TextBoxBase.HorizontalOffsetProperty, (double)0, (double)10),
                new SampleValue(TextBoxBase.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible),
                new SampleValue(TextBoxBase.IsReadOnlyProperty, true, false),
                new SampleValue(SpellCheck.IsEnabledProperty, true, false),
//              new SampleValue(TextBoxBase.VerticalOffsetProperty, (double)0, (double)10),
                new SampleValue(TextBoxBase.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Visible),
                new SampleValue(TextBox.TextWrappingProperty, TextWrapping.Wrap, TextWrapping.NoWrap),
            };
        }

        #endregion Inner types.
    }

}
