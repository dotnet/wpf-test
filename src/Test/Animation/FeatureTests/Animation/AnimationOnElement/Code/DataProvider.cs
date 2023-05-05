// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// $Id:$ $Change:$
using System;
using System.Collections;
using System.IO;
//using System.Printing.Configuration;  how to ref ReachFramework.dll?
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml;


namespace Microsoft.Test.Animation
{
    /* Factory Interface */

    /// <summary>
    /// Factory class to create Data and Associations
    /// </summary>
    public class DataFactory
    {

        /// <summary>
        /// Selects a provider based on element Name
        /// </summary>
        public static DataProvider GetProvider( string id )
        {
            DataProvider provider = null;

            // select provider based on id
            switch( id )
            {
                case "":
                    break;

                // create defaults
                default:
                    provider = new DataProvider();
                    break;
            }

            return provider;
        }

        /// <summary>
        /// Selects a translator based on element Name
        /// </summary>
        public static DataTranslator GetTranslator( string id )
        {
            DataTranslator translator = null;

            // select translator based on id
            switch( id )
            {
                case "":
                    break;

                // create defaults
                default:
                    translator = new MarkUpDataTranslator();
                    break;
            }

            return translator;
        }

        /// <summary>
        /// Selects an association based on element Name
        /// </summary>
        public static DataAssociation GetAssociation( string id )
        {
            DataAssociation association = null;
            DataProvider provider = null;
            DataTranslator translator = null;

            // select association
            switch( id )
            {
                case "":
                    break;

                // create defaults
                default:
                    provider = GetProvider( id );
                    translator = GetTranslator( id );
                    association = new MarkUpDataAssociation(provider,translator);
                    break;
            }

            return association;
        }
    }

    /* Data Providers */


    /// <summary>
    /// Gets an object instance for the given type
    /// </summary>
    public class DataProvider
    {
        /// <summary>
        /// Gets an object instance for the given type
        /// </summary>
        /// <param name="typeFullName">Actual Type name as given by "x.GetType().FullName".</param>
        /// <returns>An object array of the requested type.  </returns>
        virtual public object[] GetDataForType( string typeFullName)
        {
            return GetDataForType(typeFullName, String.Empty);
        }


        /// <summary>
        /// Gets an object instance for the given type
        /// </summary>
        /// <param name="typeFullName">Actual Type name as given by "x.GetType().FullName".</param>
        /// <returns>An object array of the requested type.  </returns>
        virtual public object[] GetDataForType( string typeFullName, string propertyName)
        {
            object[] dataObject = null;

            switch( typeFullName )
            {

                // ### Enums ...
                //case "System.Windows.Navigation.JournalMode":
                //    dataObject = new object[] {  System.Windows.Navigation.JournalMode.Auto, System.Windows.Navigation.JournalMode.Auto };
                //    break;
                case "System.Windows.FontFraction":
                    dataObject = new object[] {  System.Windows.FontFraction.Normal, System.Windows.FontFraction.Slashed, System.Windows.FontFraction.Stacked };
                    break;
                case "System.Windows.FontStyle":
                    dataObject = new object[] {  System.Windows.FontStyles.Normal, System.Windows.FontStyles.Oblique, System.Windows.FontStyles.Italic };
                    break;
                case "System.Windows.Media.PenLineCap":
                    dataObject = new object[] {  System.Windows.Media.PenLineCap.Flat, System.Windows.Media.PenLineCap.Round, System.Windows.Media.PenLineCap.Square, System.Windows.Media.PenLineCap.Triangle };
                    break;
                case "System.Windows.TextMarkerStyle":
                    dataObject = new object[] {  System.Windows.TextMarkerStyle.None, System.Windows.TextMarkerStyle.Disc, System.Windows.TextMarkerStyle.Circle, System.Windows.TextMarkerStyle.Square, System.Windows.TextMarkerStyle.Box, System.Windows.TextMarkerStyle.LowerRoman, System.Windows.TextMarkerStyle.UpperRoman, System.Windows.TextMarkerStyle.LowerLatin, System.Windows.TextMarkerStyle.UpperLatin, System.Windows.TextMarkerStyle.Decimal };
                    break;
                case "System.Windows.Media.StyleSimulations":
                    dataObject = new object[] {  System.Windows.Media.StyleSimulations.None, System.Windows.Media.StyleSimulations.BoldSimulation, System.Windows.Media.StyleSimulations.ItalicSimulation, System.Windows.Media.StyleSimulations.BoldItalicSimulation };
                    break;
                case "System.Windows.Media.TileMode":
                    dataObject = new object[] {  System.Windows.Media.TileMode.None, System.Windows.Media.TileMode.Tile };
                    break;
                case "System.Windows.TextDecorationCollection":
                    dataObject = new object[] {  new TextDecorationCollection(), System.Windows.TextDecorations.Underline, System.Windows.TextDecorations.OverLine, System.Windows.TextDecorations.Strikethrough };
                    break;
                case "System.Windows.WrapDirection":
                    dataObject = new object[] {  System.Windows.WrapDirection.Left, System.Windows.WrapDirection.Right, System.Windows.WrapDirection.Both };
                    break;
                case "System.Windows.Media.PenLineJoin":
                    dataObject = new object[] {  System.Windows.Media.PenLineJoin.Miter, System.Windows.Media.PenLineJoin.Round, System.Windows.Media.PenLineJoin.Bevel };
                    break;
                case "System.Windows.Controls.Primitives.CharacterCasing":
                    dataObject = new object[] {  System.Windows.Controls.CharacterCasing.Normal, System.Windows.Controls.CharacterCasing.Lower, System.Windows.Controls.CharacterCasing.Upper };
                    break;
                case "System.Windows.ColumnSpaceDistribution":
                    dataObject = new object[] {  System.Windows.ColumnSpaceDistribution.Left, System.Windows.ColumnSpaceDistribution.Right, System.Windows.ColumnSpaceDistribution.Between };
                    break;
                case "System.Windows.FontCapitals":
                    dataObject = new object[] {  System.Windows.FontCapitals.Normal, System.Windows.FontCapitals.AllSmallCaps, System.Windows.FontCapitals.SmallCaps, System.Windows.FontCapitals.AllPetiteCaps, System.Windows.FontCapitals.PetiteCaps, System.Windows.FontCapitals.Unicase, System.Windows.FontCapitals.Titling };
                    break;
                case "System.Windows.Controls.MenuItemRole":
                    dataObject = new object[] {  System.Windows.Controls.MenuItemRole.TopLevelItem, System.Windows.Controls.MenuItemRole.TopLevelHeader, System.Windows.Controls.MenuItemRole.SubmenuItem, System.Windows.Controls.MenuItemRole.SubmenuHeader };
                    break;
//                case "System.Windows.Documents.Emphasis":
//                    dataObject = new object[] {  System.Windows.Documents.Emphasis.Low, System.Windows.Documents.Emphasis.Medium, System.Windows.Documents.Emphasis.High };
//                    break;
                case "System.Windows.Documents.FlowDocument.TextEffects":
                    dataObject = new object[] { new TextEffect(), new TextEffect(), new TextEffect(), new TextEffect() };
                    break;
                case "System.Windows.Controls.Dock":
                    dataObject = new object[] {  System.Windows.Controls.Dock.Left, System.Windows.Controls.Dock.Top, System.Windows.Controls.Dock.Right, System.Windows.Controls.Dock.Bottom };
                    break;
                case "System.Windows.FontWeight":
                    dataObject = new object[] {  System.Windows.FontWeights.Normal, System.Windows.FontWeights.Thin, System.Windows.FontWeights.ExtraLight, System.Windows.FontWeights.Light, System.Windows.FontWeights.Medium, System.Windows.FontWeights.SemiBold, System.Windows.FontWeights.Bold, System.Windows.FontWeights.ExtraBold, System.Windows.FontWeights.Black };
                    break;
                case "System.Windows.FontEastAsianWidths":
                    dataObject = new object[] {  System.Windows.FontEastAsianWidths.Normal, System.Windows.FontEastAsianWidths.Proportional, System.Windows.FontEastAsianWidths.Full, System.Windows.FontEastAsianWidths.Half, System.Windows.FontEastAsianWidths.Third, System.Windows.FontEastAsianWidths.Quarter };
                    break;
                case "System.Windows.TextWrapping":
                    dataObject = new object[] {  System.Windows.TextWrapping.WrapWithOverflow, System.Windows.TextWrapping.NoWrap, System.Windows.TextWrapping.Wrap };
                    break;
                case "System.Windows.FontEastAsianLanguage":
                    dataObject = new object[] {  System.Windows.FontEastAsianLanguage.Normal, System.Windows.FontEastAsianLanguage.Jis78, System.Windows.FontEastAsianLanguage.Jis83, System.Windows.FontEastAsianLanguage.Jis90, System.Windows.FontEastAsianLanguage.Jis04, System.Windows.FontEastAsianLanguage.NlcKanji, System.Windows.FontEastAsianLanguage.Simplified, System.Windows.FontEastAsianLanguage.Traditional, System.Windows.FontEastAsianLanguage.TraditionalNames };
                    break;
                case "System.Windows.Controls.SelectionMode":
                    dataObject = new object[] {  System.Windows.Controls.SelectionMode.Single, System.Windows.Controls.SelectionMode.Multiple, System.Windows.Controls.SelectionMode.Extended };
                    break;
                case "System.Windows.Media.FillRule":
                    dataObject = new object[] {  System.Windows.Media.FillRule.EvenOdd, System.Windows.Media.FillRule.Nonzero };
                    break;
                case "System.Windows.Controls.ClickMode":
                    dataObject = new object[] {  System.Windows.Controls.ClickMode.Release, System.Windows.Controls.ClickMode.Press };
                    break;
                case "System.Windows.FlowDirection":
                    dataObject = new object[] {  System.Windows.FlowDirection.LeftToRight, System.Windows.FlowDirection.RightToLeft };
                    break;
                case "System.Windows.Input.ImeConversionMode":
                    dataObject = new object[] {  System.Windows.Input.ImeConversionModeValues.Alphanumeric, System.Windows.Input.ImeConversionModeValues.Native, System.Windows.Input.ImeConversionModeValues.Katakana, System.Windows.Input.ImeConversionModeValues.FullShape, System.Windows.Input.ImeConversionModeValues.Roman, System.Windows.Input.ImeConversionModeValues.CharCode, System.Windows.Input.ImeConversionModeValues.NoConversion, System.Windows.Input.ImeConversionModeValues.Eudc, System.Windows.Input.ImeConversionModeValues.Symbol, System.Windows.Input.ImeConversionModeValues.Fixed };
                    break;
                case "System.Windows.Media.Stretch":
                    dataObject = new object[] {  System.Windows.Media.Stretch.None, System.Windows.Media.Stretch.Fill, System.Windows.Media.Stretch.Uniform, System.Windows.Media.Stretch.UniformToFill };
                    break;
                case "System.Windows.FigureVerticalAnchor":
                    dataObject = new object[] {  System.Windows.FigureVerticalAnchor.ContentTop, System.Windows.FigureVerticalAnchor.ContentCenter, System.Windows.FigureVerticalAnchor.ContentBottom, System.Windows.FigureVerticalAnchor.ParagraphTop};
                    break;
                case "System.Windows.FontNumeralStyle":
                    dataObject = new object[] {  System.Windows.FontNumeralStyle.Normal, System.Windows.FontNumeralStyle.Lining, System.Windows.FontNumeralStyle.OldStyle };
                    break;
                case "System.Windows.Controls.Primitives.PlacementMode":
                    dataObject = new object[] {  System.Windows.Controls.Primitives.PlacementMode.Absolute, System.Windows.Controls.Primitives.PlacementMode.Relative, System.Windows.Controls.Primitives.PlacementMode.Bottom, System.Windows.Controls.Primitives.PlacementMode.Center, System.Windows.Controls.Primitives.PlacementMode.Right, System.Windows.Controls.Primitives.PlacementMode.AbsolutePoint, System.Windows.Controls.Primitives.PlacementMode.RelativePoint, System.Windows.Controls.Primitives.PlacementMode.Mouse, System.Windows.Controls.Primitives.PlacementMode.MousePoint };
                    break;
                case "System.Windows.LineStackingStrategy":
                    dataObject = new object[] {  System.Windows.LineStackingStrategy.BlockLineHeight, System.Windows.LineStackingStrategy.MaxHeight};
                    break;
                case "System.Windows.FontStretch":
                    dataObject = new object[] {  System.Windows.FontStretches.Normal, System.Windows.FontStretches.UltraCondensed, System.Windows.FontStretches.ExtraCondensed, System.Windows.FontStretches.Condensed, System.Windows.FontStretches.SemiCondensed, System.Windows.FontStretches.SemiExpanded, System.Windows.FontStretches.Expanded, System.Windows.FontStretches.ExtraExpanded, System.Windows.FontStretches.UltraExpanded };
                    break;
                case "System.Windows.Controls.Primitives.ToggleButton.IsChecked":
                    dataObject = new object[] {  false, true, null };
                    break;
                case "System.Windows.TextTrimming":
                    dataObject = new object[] {  System.Windows.TextTrimming.None, System.Windows.TextTrimming.CharacterEllipsis, System.Windows.TextTrimming.WordEllipsis };
                    break;
                case "System.Windows.Visibility":
                    dataObject = new object[] {  System.Windows.Visibility.Visible, System.Windows.Visibility.Hidden, System.Windows.Visibility.Collapsed };
                    break;
                case "System.Windows.Input.ImeSentenceMode":
                    dataObject = new object[] {  System.Windows.Input.ImeSentenceModeValues.None, System.Windows.Input.ImeSentenceModeValues.PluralClause, System.Windows.Input.ImeSentenceModeValues.SingleConversion, System.Windows.Input.ImeSentenceModeValues.Automatic, System.Windows.Input.ImeSentenceModeValues.PhrasePrediction, System.Windows.Input.ImeSentenceModeValues.Conversation };
                    break;
                case "System.Windows.Controls.ScrollBarVisibility":
                    dataObject = new object[] {  System.Windows.Controls.ScrollBarVisibility.Auto, System.Windows.Controls.ScrollBarVisibility.Hidden, System.Windows.Controls.ScrollBarVisibility.Visible };
                    break;
                case "System.Windows.Input.KeyboardNavigationMode":
                    dataObject = new object[] {  System.Windows.Input.KeyboardNavigationMode.Continue, System.Windows.Input.KeyboardNavigationMode.Once, System.Windows.Input.KeyboardNavigationMode.Cycle, System.Windows.Input.KeyboardNavigationMode.None, System.Windows.Input.KeyboardNavigationMode.Contained };
                    break;
                case "System.Windows.FigureHorizontalAnchor":
                    dataObject = new object[] { System.Windows.FigureHorizontalAnchor.ContentLeft, System.Windows.FigureHorizontalAnchor.ContentCenter, System.Windows.FigureHorizontalAnchor.ContentRight, System.Windows.FigureHorizontalAnchor.ColumnLeft, System.Windows.FigureHorizontalAnchor.ColumnCenter, System.Windows.FigureHorizontalAnchor.ColumnRight };
                    break;
                case "System.Windows.FontNumeralAlignment":
                    dataObject = new object[] {  System.Windows.FontNumeralAlignment.Normal, System.Windows.FontNumeralAlignment.Proportional, System.Windows.FontNumeralAlignment.Tabular };
                    break;
                case "System.Windows.BaselineAlignment":
                    dataObject = new object[] {  System.Windows.BaselineAlignment.Top, System.Windows.BaselineAlignment.Center, System.Windows.BaselineAlignment.Bottom, System.Windows.BaselineAlignment.Baseline, System.Windows.BaselineAlignment.TextTop, System.Windows.BaselineAlignment.TextBottom, System.Windows.BaselineAlignment.Subscript, System.Windows.BaselineAlignment.Superscript };
                    break;
                case "System.Windows.Input.InputMethodState":
                    dataObject = new object[] {  System.Windows.Input.InputMethodState.Off, System.Windows.Input.InputMethodState.On };
                    break;
//                case "System.Windows.FlowBehavior":
//                    dataObject = new object[] {  System.Windows.FlowBehavior.Inline, System.Windows.FlowBehavior.Block, System.Windows.FlowBehavior.Figure, System.Windows.FlowBehavior.Floating };
//                    break;
                case "System.Windows.FontVariants":
                    dataObject = new object[] {  System.Windows.FontVariants.Normal, System.Windows.FontVariants.Superscript, System.Windows.FontVariants.Subscript, System.Windows.FontVariants.Ordinal, System.Windows.FontVariants.Inferior, System.Windows.FontVariants.Ruby };
                    break;
//                case "System.Windows.Documents.ColumnPreference":
//                    dataObject = new object[] {  System.Windows.Documents.ColumnPreference.Single, System.Windows.Documents.ColumnPreference.Low, System.Windows.Documents.ColumnPreference.Medium, System.Windows.Documents.ColumnPreference.High, System.Windows.Documents.ColumnPreference.Maximum };
//                    break;
                //case "System.Windows.Navigation.Journal+JournalMode":
                //    dataObject = new object[] {  System.Windows.Navigation.JournalMode.Auto, System.Windows.Navigation.JournalMode.Source, System.Windows.Navigation.JournalMode.Serialize, System.Windows.Navigation.JournalMode.KeepAlive };
                //    break;
                case "System.Windows.ResizeMode":
                    dataObject = new object[] {
                    System.Windows.ResizeMode.NoResize, System.Windows.ResizeMode.CanResize, System.Windows.ResizeMode.CanResizeWithGrip };
                    break;
                case "System.Windows.Input.InputScopeNameValue":
                    dataObject = new object[]  {
                                  System.Windows.Input.InputScopeNameValue.Default,
                                  System.Windows.Input.InputScopeNameValue.Url,
                                  System.Windows.Input.InputScopeNameValue.FullFilePath,
                                  System.Windows.Input.InputScopeNameValue.FileName,
                                  System.Windows.Input.InputScopeNameValue.EmailUserName,
                                  System.Windows.Input.InputScopeNameValue.EmailSmtpAddress,
                                  System.Windows.Input.InputScopeNameValue.LogOnName,
                                  System.Windows.Input.InputScopeNameValue.PersonalFullName,
                                  System.Windows.Input.InputScopeNameValue.PersonalNamePrefix,
                                  System.Windows.Input.InputScopeNameValue.PersonalGivenName,
                                  System.Windows.Input.InputScopeNameValue.PersonalMiddleName,
                                  System.Windows.Input.InputScopeNameValue.PersonalSurname,
                                  System.Windows.Input.InputScopeNameValue.PersonalNameSuffix,
                                  System.Windows.Input.InputScopeNameValue.PostalAddress,
                                  System.Windows.Input.InputScopeNameValue.PostalCode,
                                  System.Windows.Input.InputScopeNameValue.AddressStreet,
                                  System.Windows.Input.InputScopeNameValue.AddressStateOrProvince,
                                  System.Windows.Input.InputScopeNameValue.AddressCity,
                                  System.Windows.Input.InputScopeNameValue.AddressCountryName,
                                  System.Windows.Input.InputScopeNameValue.AddressCountryShortName,
                                  System.Windows.Input.InputScopeNameValue.CurrencyAmountAndSymbol,
                                  System.Windows.Input.InputScopeNameValue.CurrencyAmount,
                                  System.Windows.Input.InputScopeNameValue.Date,
                                  System.Windows.Input.InputScopeNameValue.DateMonth,
                                  System.Windows.Input.InputScopeNameValue.DateDay,
                                  System.Windows.Input.InputScopeNameValue.DateYear,
                                  System.Windows.Input.InputScopeNameValue.DateMonthName,
                                  System.Windows.Input.InputScopeNameValue.DateDayName,
                                  System.Windows.Input.InputScopeNameValue.Digits,
                                  System.Windows.Input.InputScopeNameValue.Number,
                                  System.Windows.Input.InputScopeNameValue.OneChar,
                                  System.Windows.Input.InputScopeNameValue.Password,
                                  System.Windows.Input.InputScopeNameValue.TelephoneNumber,
                                  System.Windows.Input.InputScopeNameValue.TelephoneCountryCode,
                                  System.Windows.Input.InputScopeNameValue.TelephoneAreaCode,
                                  System.Windows.Input.InputScopeNameValue.TelephoneLocalNumber,
                                  System.Windows.Input.InputScopeNameValue.Time,
                                  System.Windows.Input.InputScopeNameValue.TimeHour,
                                  System.Windows.Input.InputScopeNameValue.TimeMinorSec,
                                  System.Windows.Input.InputScopeNameValue.NumberFullWidth,
                                  System.Windows.Input.InputScopeNameValue.AlphanumericHalfWidth,
                                  System.Windows.Input.InputScopeNameValue.AlphanumericFullWidth,
                                  System.Windows.Input.InputScopeNameValue.CurrencyChinese,
                                  System.Windows.Input.InputScopeNameValue.Bopomofo,
                                  System.Windows.Input.InputScopeNameValue.Hiragana,
                                  System.Windows.Input.InputScopeNameValue.KatakanaHalfWidth,
                                  System.Windows.Input.InputScopeNameValue.KatakanaFullWidth,
                                  System.Windows.Input.InputScopeNameValue.Hanja,
                                  System.Windows.Input.InputScopeNameValue.PhraseList,
                                  System.Windows.Input.InputScopeNameValue.RegularExpression,
                                  System.Windows.Input.InputScopeNameValue.Srgs,
                                  System.Windows.Input.InputScopeNameValue.Xml };
                    break;
                case "System.Windows.Controls.Orientation":
                    dataObject = new object[] {  System.Windows.Controls.Orientation.Horizontal, System.Windows.Controls.Orientation.Vertical };
                    break;
                case "System.Windows.WindowStyle":
                    dataObject = new object[] {  System.Windows.WindowStyle.None, System.Windows.WindowStyle.SingleBorderWindow, System.Windows.WindowStyle.ThreeDBorderWindow, System.Windows.WindowStyle.ToolWindow };
                    break;
                case "System.Windows.WindowState":
                    dataObject = new object[] {  System.Windows.WindowState.Normal, System.Windows.WindowState.Minimized, System.Windows.WindowState.Maximized };
                    break;
                case "System.Windows.Media.BrushMappingMode":
                    dataObject = new object[] {  System.Windows.Media.BrushMappingMode.Absolute, System.Windows.Media.BrushMappingMode.RelativeToBoundingBox,  };
                    break;
                case "System.Windows.SizeToContent":
                    dataObject = new object[] {  System.Windows.SizeToContent.Manual  };
                    break;
                case "System.Windows.Controls.StretchDirection":
                    dataObject = new object[] { System.Windows.Controls.StretchDirection.Both };
                    break;
                case "System.Windows.Media.PixelFormat":
                    dataObject = new object[] { System.Windows.Media.PixelFormats.Cmyk32 };
                    break;


                // ### Value Types ...
                case "System.Windows.Thickness":
                    dataObject = new object[] { new System.Windows.Thickness() };
                    break;
                case "System.Int32":
                    dataObject = new object[] { new System.Int32() };
                    break;
                case "System.Boolean":
                    dataObject = new object[] { false, true };
                    break;
                case "System.Double":
                    dataObject = new object[] { new System.Double() };
                    break;
                case "System.Windows.FontSize":
                    dataObject = new object[] { 8.0, 24.0, 16.0 };
                    break;
                case "System.Windows.Rect":
                    dataObject = new object[] {
                        new System.Windows.Rect(0.9,0.5,100.2,200.1) ,
                        new System.Windows.Rect(new System.Windows.Point(110.9,170.2), new System.Windows.Vector(12.2,20.1)) ,
                        new System.Windows.Rect(new System.Windows.Point(210.9,90.8), new System.Windows.Size(30.2,22.0)) ,
                        };
                    break;
                case "System.Windows.GridLength":
                    dataObject = new object[] { new System.Windows.GridLength(120), new System.Windows.GridLength(96), new System.Windows.GridLength(210),  };
                    break;
                case "System.Windows.Point":
                    dataObject = new object[]
                    {
                        new System.Windows.Point(0, 0),
                        new System.Windows.Point(0, 100),
                        new System.Windows.Point(100, 100),
                        new System.Windows.Point(100, 0)
                    };
                    break;
                case "System.Windows.Size":
                    dataObject = new object[]
                    {
                        new System.Windows.Size(100, 200),
                        new System.Windows.Size(200, 400),
                        new System.Windows.Size(400, 800),
                        new System.Windows.Size(1, 2)
                    };
                    break;
                case "System.Windows.Media.Color":
                    dataObject = new object[] {
                        Colors.Cyan,
                        Colors.Red,
                        Colors.Azure,
                        Colors.MistyRose
                        };
                    break;
                case "System.Char":
                    dataObject = new object[] { 'a', 'b', 'c', 'd' };
                    break;

                // ### Interfaces ...
                case "System.Collections.IEnumerable":
                    dataObject = new object[] {
                        new System.Collections.ArrayList()};
                    break;
                case "System.Windows.Controls.Primitives.IScrollInfo":
                    break;

                // ### Abstract Classes ...
                case "System.Windows.Media.Geometry":
                    dataObject = new object[] {
                        // simple ellipse
                        new EllipseGeometry(new Rect(0,0,100,120)) ,
                        new EllipseGeometry(new Rect(0,0,120,120)) ,
                        // compound stuff ...
                        };
                    break;
                case "System.Windows.Media.Brush":

                    /*
                    //ImageBrush
                    ImageBrush imageBrush = new ImageBrush(BitmapFrame.Create(new System.IO.FileStream(@"Black.png", System.IO.FileMode.Open)));

                    //NineGridBrush
                    BitmapSource imageData = BitmapFrame.Create(new System.IO.FileStream(@"Red.png", System.IO.FileMode.Open));
                    NineGridBrush nineGridBrush = new NineGridBrush(imageData, 22,18,52,12);
                    */

                    //DrawingBrush
                    System.Windows.Media.DrawingGroup drawing = new System.Windows.Media.DrawingGroup();
                    DrawingContext ctx = drawing.Open();
                    ctx.DrawRectangle(Brushes.Orange, null, new Rect(1,1,100,100));
                    ctx.Close();
                    DrawingBrush drawingBrush = new DrawingBrush(drawing);
                    drawingBrush.Viewbox = new Rect(0,0,11,11);
                    drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    drawingBrush.Viewport = new Rect(0,0,0.1,0.1);
                    drawingBrush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                    drawingBrush.TileMode = TileMode.None;

                    dataObject = new object[] {
                        new LinearGradientBrush(Colors.Red, Colors.Gold, 0),
                        // Solid Color Brushes
                        System.Windows.Media.Brushes.Blue,
                        System.Windows.Media.Brushes.Gold,
                        System.Windows.Media.Brushes.Red,
                        System.Windows.Media.Brushes.Silver,
                        // Gradient Brushes
                        new LinearGradientBrush(Colors.Black, Colors.White, 0),
                        // new LinearGradientBrush(Colors.Red, Colors.Gold, 0),
                        new RadialGradientBrush(Colors.White, Colors.Black),
                        new RadialGradientBrush(Colors.Silver, Colors.Blue),
                        // Drawing Brushes
                        drawingBrush,
                        /*
                        // Image Brushes
                        imageBrush,
                        // Nine Grid
                        nineGridBrush,
                        */
                        };
                    break;
                case "System.Windows.Media.Transform":
                    dataObject = new object[] {
                        // rotation
                        new System.Windows.Media.RotateTransform( 21.5 ),
                        // scale
                        new System.Windows.Media.ScaleTransform( 1.5, 0.7 ),
                        // translation
                        new System.Windows.Media.TranslateTransform( 200.5, 166.7 ),
                        // skew x
                        new System.Windows.Media.SkewTransform( 370.0, 0.0 ),
                        // skew y
                        new System.Windows.Media.SkewTransform( 0.0, 200.0 ),
                        // matrix, mirror x
                        new System.Windows.Media.MatrixTransform(
                                -1.0,    0.0,
                                0.0,    1.0,
                                0.0,    0.0
                            ),
                        // matrix, mirror y
                        new System.Windows.Media.MatrixTransform(
                                1.0,    0.0,
                                0.0,    -1.0,
                                0.0,    0.0
                            ),
                        };
                    break;

                // ###  Classes ...
                case "System.Windows.Controls.ContextMenu":
                    dataObject = new object[] {   new System.Windows.Controls.ContextMenu(),  new System.Windows.Controls.ContextMenu() };
                    break;

             /*// PageSource does not exist anymore. Please replace it with what you think is appropriate.
                case "System.Windows.Controls.Primitives.PageSource":
                    dataObject = new object[] {   new System.Windows.Controls.Primitives.PageSource(),  new System.Windows.Controls.Primitives.PageSource() };
                    break; */

                case "System.Windows.Media.Visual":
                case "System.Windows.FrameworkElement":
                    dataObject = new object[] {   new System.Windows.FrameworkElement(),  new System.Windows.Controls.Button(),  new System.Windows.Controls.Canvas() };
                    break;

                case "System.Windows.Input.Cursor":
                    dataObject = new object[] {   System.Windows.Input.Cursors.Hand, System.Windows.Input.Cursors.Arrow };
                    break;
                case "System.Windows.Media.Pen":
                    dataObject = new object[] {
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Gold,   3.141592 ),
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Red,    2.731201 ),
                        new System.Windows.Media.Pen(),
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Silver, 0.673902 ),
                        };
                    break;
                case "System.Globalization.CultureInfo":
                    dataObject = new object[] {new System.Globalization.CultureInfo("es-AR"),  new System.Globalization.CultureInfo("en-US"),  new System.Globalization.CultureInfo("tr-TR"), };
                    break;
                case "System.Windows.Media.SolidColorBrush":
                    dataObject = new object[] {
                        System.Windows.Media.Brushes.Green,
                        System.Windows.Media.Brushes.Gold,
                        System.Windows.Media.Brushes.Red,
                        System.Windows.Media.Brushes.Silver,
                        // new System.Windows.Media.SolidColorBrush(),  new System.Windows.Media.SolidColorBrush(),  new System.Windows.Media.SolidColorBrush()
                        };
                    break;
                case "System.Windows.Media.PointCollection":
                    dataObject = new object[] {   new System.Windows.Media.PointCollection(),  new System.Windows.Media.PointCollection(),  new System.Windows.Media.PointCollection(),  new System.Windows.Media.PointCollection() };
                    break;

                case "System.Windows.Media.BitmapSource":
                    dataObject = new object[] {
                        new System.Windows.Media.Imaging.RenderTargetBitmap( 10, 12, 96.0, 96.0, System.Windows.Media.PixelFormats.Default ),
                        new System.Windows.Media.Imaging.RenderTargetBitmap( 5, 5, 96.0, 96.0, System.Windows.Media.PixelFormats.Pbgra32 ),
                        new System.Windows.Media.Imaging.RenderTargetBitmap( 100, 140, 96.0, 96.0, System.Windows.Media.PixelFormats.Pbgra32 ),
                        // ### 
                        };
                    break;

                case "System.Object":
                    // we have a whole class of things that say they are System.Object, but we can't just assign
                    // any random object to them. So we use the propertyName parameter to qualify which property
                    // we're talking about & return the right set of objects.
                    switch(propertyName)
                    {
                      case "XmlNamespaceManager":
                        dataObject = new object[] { new XmlNamespaceManager(new NameTable()), new XmlNamespaceManager(new NameTable()), new XmlNamespaceManager(new NameTable())};
                        break;
                      case "DataContext":
                        dataObject = new object[] { new ObjectDataProvider(), new ObjectDataProvider(), new ObjectDataProvider()};
                        break;
                      //case "PrintTicket":
                        //dataObject = new object[] { new PrintTicket()};
                        //break;
                      case "ContextMenu":
                        dataObject = new object[] { new ContextMenu(), new ContextMenu(), new ContextMenu()};
                        break;
                      case "ToolTip":
                        dataObject = new object[] {new ToolTip(), new ToolTip(), new ToolTip() };
                        break;
                      default:
                        dataObject = new object[] {   new System.Object(), String.Empty, new System.Windows.UIElement(),  new System.Windows.FrameworkElement(),  new System.Windows.Controls.Button()};
                        break;
                    }
                    break;
                case "System.Windows.Controls.ItemsControl":
                    dataObject = new object[] {   new System.Windows.Controls.ItemsControl(),  new System.Windows.Controls.ItemsControl() };
                    break;
//                case "System.Windows.Documents.ReadingMetricsCache":
//                    dataObject = new object[] {   new System.Windows.Documents.ReadingMetricsCache(),  new System.Windows.Documents.ReadingMetricsCache() };
//                    break;
                case "System.Windows.Controls.StyleSelector":
                    dataObject = new object[] {
                        new System.Windows.Controls.StyleSelector(),
                        new HashStyleSelector(),
                        new System.Windows.Controls.StyleSelector(),
                        };
                    break;
                case "System.Uri":
                    dataObject = new object[] {   new System.Uri( @"Integration.xaml" , UriKind.RelativeOrAbsolute), new System.Uri( @"http://www.msn.com" , UriKind.RelativeOrAbsolute), new System.Uri( @"IntegrationProxy.xaml", UriKind.RelativeOrAbsolute), new System.Uri( @"http://www.microsoft.com", UriKind.RelativeOrAbsolute), new System.Uri( @"PageViewerContent1.xaml", UriKind.RelativeOrAbsolute) };
                    break;
                case "System.String":
                    dataObject = new object[] {   "Hello_World", String.Empty, "Some rather long string is placed here.  Maybe a paragraph or some other random writing.  This is only a test.  ", "This one has...\n ...more than one line" };
                    break;
                case "System.Windows.Markup.XmlnsDictionary":
                    dataObject = new object[] {   new System.Windows.Markup.XmlnsDictionary(),  new System.Windows.Markup.XmlnsDictionary() };
                    break;
                case "System.Collections.Hashtable":
                    dataObject = new object[] {   new System.Collections.Hashtable(), new System.Collections.Hashtable(), new System.Collections.Hashtable() };
                    break;
                case "System.Windows.Style":
                    dataObject = new object[] { new System.Windows.Style() };
                    break;
                case "System.Windows.Media.Animation.ParallelTimeline":
                    dataObject = new object[] {   new System.Windows.Media.Animation.ParallelTimeline() };
                    break;
                case "System.Windows.UIElement":
                    dataObject = new object[] {   new System.Windows.UIElement(),  new System.Windows.FrameworkElement(),  new System.Windows.Controls.Button() };
                    break;
                case "System.Windows.Media.DoubleCollection":
                    dataObject = new object[] {
                        new System.Windows.Media.DoubleCollection()
                        };
                    break;
                case "System.Windows.Input.RoutedCommand":
                    dataObject = new object[] {
                        System.Windows.Input.ApplicationCommands.New,
                        System.Windows.Input.ApplicationCommands.Open,
                        System.Windows.Input.ApplicationCommands.Save,
                        System.Windows.Input.ApplicationCommands.SaveAs,
                        System.Windows.Input.ApplicationCommands.Print,
                        System.Windows.Input.ApplicationCommands.PrintPreview,
                        System.Windows.Input.ApplicationCommands.Properties,
                        System.Windows.Input.NavigationCommands.Refresh,
                        System.Windows.Input.ApplicationCommands.Stop,
                        System.Windows.Input.ApplicationCommands.Cut,
                        System.Windows.Input.ApplicationCommands.Copy,
                        System.Windows.Input.ApplicationCommands.Paste,
                        System.Windows.Input.ApplicationCommands.Delete,
                        System.Windows.Input.ApplicationCommands.Undo,
                        System.Windows.Input.ApplicationCommands.Redo,
                        System.Windows.Input.ApplicationCommands.SelectAll,
                        System.Windows.Documents.EditingCommands.ToggleInsert,
                        System.Windows.Documents.EditingCommands.Delete,
                        System.Windows.Documents.EditingCommands.Backspace,
                        System.Windows.Documents.EditingCommands.DeleteNextWord,
                        System.Windows.Documents.EditingCommands.DeletePreviousWord,
                        System.Windows.Documents.EditingCommands.EnterParagraphBreak,
                        System.Windows.Documents.EditingCommands.EnterLineBreak,
                        System.Windows.Documents.EditingCommands.TabForward,
                        System.Windows.Documents.EditingCommands.TabBackward,
                        System.Windows.Documents.EditingCommands.MoveRightByCharacter,
                        System.Windows.Documents.EditingCommands.MoveLeftByCharacter,
                        System.Windows.Documents.EditingCommands.MoveRightByWord,
                        System.Windows.Documents.EditingCommands.MoveLeftByWord,
                        System.Windows.Documents.EditingCommands.MoveDownByLine,
                        System.Windows.Documents.EditingCommands.MoveUpByLine,
                        System.Windows.Documents.EditingCommands.MoveDownByParagraph,
                        System.Windows.Documents.EditingCommands.MoveUpByParagraph,
                        System.Windows.Documents.EditingCommands.MoveDownByPage,
                        System.Windows.Documents.EditingCommands.MoveUpByPage,
                        System.Windows.Documents.EditingCommands.MoveToLineStart,
                        System.Windows.Documents.EditingCommands.MoveToLineEnd,
                        System.Windows.Documents.EditingCommands.MoveToDocumentStart,
                        System.Windows.Documents.EditingCommands.MoveToDocumentEnd,
                        System.Windows.Documents.EditingCommands.SelectRightByCharacter,
                        System.Windows.Documents.EditingCommands.SelectLeftByCharacter,
                        System.Windows.Documents.EditingCommands.SelectRightByWord,
                        System.Windows.Documents.EditingCommands.SelectLeftByWord,
                        System.Windows.Documents.EditingCommands.SelectDownByLine,
                        System.Windows.Documents.EditingCommands.SelectUpByLine,
                        System.Windows.Documents.EditingCommands.SelectDownByParagraph,
                        System.Windows.Documents.EditingCommands.SelectUpByParagraph,
                        System.Windows.Documents.EditingCommands.SelectDownByPage,
                        System.Windows.Documents.EditingCommands.SelectUpByPage,
                        System.Windows.Documents.EditingCommands.SelectToLineStart,
                        System.Windows.Documents.EditingCommands.SelectToLineEnd,
                        System.Windows.Documents.EditingCommands.SelectToDocumentStart,
                        System.Windows.Documents.EditingCommands.SelectToDocumentEnd,
                        System.Windows.Documents.EditingCommands.ToggleBold,
                        System.Windows.Documents.EditingCommands.ToggleItalic,
                        System.Windows.Documents.EditingCommands.ToggleUnderline,
                        System.Windows.Documents.EditingCommands.ToggleSubscript,
                        System.Windows.Documents.EditingCommands.ToggleSuperscript,
                        System.Windows.Documents.EditingCommands.IncreaseFontSize,
                        System.Windows.Documents.EditingCommands.DecreaseFontSize,
                        //System.Windows.Documents.EditingCommands.ApplyFontSize,
                        //System.Windows.Documents.EditingCommands.ApplyFontFamily,
                        //System.Windows.Documents.EditingCommands.ApplyForeground,
                        //System.Windows.Documents.EditingCommands.ApplyBackground,
                        System.Windows.Documents.EditingCommands.AlignLeft,
                        System.Windows.Documents.EditingCommands.AlignCenter,
                        System.Windows.Documents.EditingCommands.AlignRight,
                        System.Windows.Documents.EditingCommands.AlignJustify,
                        System.Windows.Documents.EditingCommands.ToggleBullets,
                        System.Windows.Documents.EditingCommands.ToggleNumbering,
                        System.Windows.Documents.EditingCommands.IncreaseIndentation,
                        System.Windows.Documents.EditingCommands.DecreaseIndentation,
                        };
                    break;
                case "System.Windows.Media.ColorInterpolationMode":
                    dataObject = new object[] { new System.Windows.Media.ColorInterpolationMode() };
                    break;
                case "System.Windows.Media.GeometryCombineMode":
                    dataObject = new object[] { new System.Windows.Media.GeometryCombineMode() };
                    break;
                case "System.Windows.Media.Matrix":
                    dataObject = new object[] { new System.Windows.Media.Matrix() };
                    break;
                case "System.Windows.Media.GradientSpreadMethod":
                    dataObject = new object[] { new System.Windows.Media.GradientSpreadMethod() };
                    break;
                case "System.Windows.FrameworkElementFactory":
                    dataObject = new object[] {   new System.Windows.FrameworkElementFactory(typeof(DockPanel)) };
                    break;
                case "System.Windows.Media.Media3D.Material":
                    // abstract class for DiffuseMaterial / SpecularMaterial / Emissive Material / MaterialGroup
                    dataObject = new object[]
                    {
                        new System.Windows.Media.Media3D.DiffuseMaterial(new SolidColorBrush(Colors.Cyan)),
                        new System.Windows.Media.Media3D.SpecularMaterial(new SolidColorBrush(Colors.Green), 20),
                        new System.Windows.Media.Media3D.EmissiveMaterial(new SolidColorBrush(Colors.Purple))
                    };
                    break;
                case "System.Windows.Media.Media3D.Transform3D":
                    // abstract base class for AffineTransform3D and MatrixTransform3D
                    dataObject = new object[]
                    {
                        new System.Windows.Media.Media3D.MatrixTransform3D(new System.Windows.Media.Media3D.Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1)),
                        new System.Windows.Media.Media3D.RotateTransform3D(new System.Windows.Media.Media3D.AxisAngleRotation3D(new System.Windows.Media.Media3D.Vector3D(1, 2, 1), 65)),
                        new System.Windows.Media.Media3D.TranslateTransform3D(new System.Windows.Media.Media3D.Vector3D(0, -0.2, 0))
                    };
                    break;
                case "System.Windows.Media.Media3D.MatrixTransform3D":
                    dataObject = new object[]
                    {
                        new System.Windows.Media.Media3D.MatrixTransform3D(new System.Windows.Media.Media3D.Matrix3D(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)),
                        new System.Windows.Media.Media3D.MatrixTransform3D(new System.Windows.Media.Media3D.Matrix3D(16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1)),
                        new System.Windows.Media.Media3D.MatrixTransform3D(new System.Windows.Media.Media3D.Matrix3D(1.5, 1.5, 1.5, 1.5, 1.0, 1.0, 1.0, 1.0, 0.5, 0.5, 0.5, 0.5, 1, 1, 1, 1)),
                    };
                    break;
                case "System.Windows.Media.Media3D.Matrix3D":
                    dataObject = new object[] {   new System.Windows.Media.Media3D.Matrix3D() };
                    break;
                case "System.Windows.Media.Media3D.Point3D":
                    dataObject = new object[] {   new System.Windows.Media.Media3D.Point3D() };
                    break;
                case "System.Windows.Media.Media3D.Vector3D":
                    dataObject = new object[] {   new System.Windows.Media.Media3D.Vector3D() };
                    break;
                case "System.Windows.Media.Media3D.Quaternion":
                    dataObject = new object[] {   new System.Windows.Media.Media3D.Quaternion() };
                    break;
                //case "System.Windows.Text.TypographyProperties":
                //    //### - internal class

                case "System.Windows.Data.XmlNamespaceMappingCollection":
                    dataObject = new object[] { new System.Windows.Data.XmlNamespaceMappingCollection() };
                    break;
                case "System.Windows.Media.FontFamily":
                    dataObject = new object[] { new FontFamily("Bookman Old Style"), new FontFamily("Beesknees"), new FontFamily(""), new FontFamily("Gigi")};
                    break;
                case "System.Windows.TextAlignment":
                    dataObject = new object[] {TextAlignment.Left, TextAlignment.Right, TextAlignment.Center, TextAlignment.Justify};
                    break;
                case "System.Windows.IFrameworkInputElement":   // any FrameworkElement or FrameworkContentElement
                    dataObject = new object[] { new Canvas(), new Button(), new DockPanel(), new Window()};
                    break;
                case "System.Windows.Controls.CharacterCasing":
                    dataObject = new object[] { CharacterCasing.Normal, CharacterCasing.Upper, CharacterCasing.Lower };
                    break;
                case "System.Windows.Controls.OverflowMode":
                    dataObject = new object[] {OverflowMode.AsNeeded, OverflowMode.Always, OverflowMode.Never};
                    break;

                default:
                    dataObject = null;
                    break;
            }

            return dataObject;
        }

    }

    /* Data Translators */

    /// <summary>
    /// Gets a string representation for the given type
    /// </summary>
    public class DataTranslator
    {
        /// <summary>
        /// Implements ToString()
        /// </summary>
        /// <param name="typeFullName">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theObject">Object instance of the given type.</param>
        /// <returns>A string that can be used inside markup tags to represent the object</returns>
        virtual public string GetMarkupRepresentation( string typeFullName, object theObject )
        {
            // use type converter here
            return System.ComponentModel.TypeDescriptor.GetConverter( theObject )
                .ConvertToInvariantString( theObject ) ;
        }

        /// <summary>
        /// Checks whether it can implement ToString()
        /// </summary>
        /// <param name="typeFullName">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theObject">Object instance of the given type.</param>
        /// <returns>A string that can be used inside markup tags to represent the object</returns>
        virtual public bool CanGetMarkupRepresentation( string typeFullName, object theObject )
        {
            // use type converter here
            return System.ComponentModel.TypeDescriptor.GetConverter( theObject)
                .CanConvertTo( String.Empty.GetType() );
        }
    }

    /// <summary>
    /// Gets a string representation for the given type
    /// </summary>
    public class MarkUpDataTranslator : DataTranslator
    {
        /// <summary>
        /// Implements ToString() for those type that don't
        /// </summary>
        /// <param name="typeFullName">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theObject">Object instance of the given type.</param>
        /// <returns>A string that can be used inside markup tags to represent the object</returns>
        public override string GetMarkupRepresentation( string typeFullName, object theObject )
        {
            string markup = null;

            switch( typeFullName )
            {

                case "System.Windows.Media.NineGridBrush" :
                    markup = "";
                    break;

                // all others are safe ...
                default:
                    // try base
                    markup = base.GetMarkupRepresentation( typeFullName, theObject );
                    break;
            }
             return markup;
        }
    }

    /* Data Associations */

    /// <summary>
    /// Associates a property with a value
    /// </summary>
    public class DataAssociation
    {

        /// <summary>
        /// Internal provider
        /// </summary>
        protected DataProvider provider;
        /// <summary>
        /// Internal translator
        /// </summary>
        protected DataTranslator translator;

        /// <summary>
        /// Internal provider refernce
        /// </summary>
        public DataProvider Provider
        {
            get { return provider; }
        }

        /// <summary>
        /// Internal translator refernce
        /// </summary>
        public DataTranslator Translator
        {
            get { return translator; }
        }


        /// <summary>
        /// Constructor requires a data provider and translator
        /// </summary>
        public DataAssociation( DataProvider prv, DataTranslator trn )
        {
            provider = prv;
            translator = trn;
        }
        /// <summary>
        /// Empty constructor uses default data provider and translator
        /// </summary>
        public DataAssociation()
        {
            provider = new DataProvider() ;
            translator = new DataTranslator();
        }

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theType">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used inside markup tags to represent the property value</returns>
        virtual public string GetInlineStringForProperty( string theType, string theProperty )
        {
            string markup = null;
            // delegate to provider
            object[] options = provider.GetDataForType(theType);
            if ( options != null )
            {
                // always choose first option and delegate to translator
                markup = translator.GetMarkupRepresentation( theType, options[0] ) ;
            }
            return markup;
        }

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theType">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used inside markup tags to represent the property value</returns>
        virtual public bool CanGetInlineStringForProperty( string theType, string theProperty )
        {
            bool canGetMarkup = false;
            // delegate to provider
            object[] options = provider.GetDataForType(theType);
            if ( options != null )
            {
                // always choose first option and delegate to translator
                canGetMarkup = translator.CanGetMarkupRepresentation( theType, options[0] ) ;
            }
            return canGetMarkup;
        }

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theType">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used between markup complex property tags to represent the property value.</returns>
        virtual public string GetXMLStringForProperty( string theType, string theProperty )
        {
            string markup = GetInlineStringForProperty( theType, theProperty );
            if ( markup != null )
            {
                markup = String.Format( "<{1}>{0}</{1}>" , markup, theProperty );
            }
            return markup;
        }

        /// <summary>
        /// Gets a value for a given property, of its appropriate type
        /// </summary>
        /// <param name="theType">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>An object that can be used on the given propery.</returns>
        virtual public object GetValueForProperty( string theType, string theProperty )
        {
            object theValue = null;
            // get avalable options for this type
            object[] options = provider.GetDataForType(theType);
            // if we have an option
            if ( options != null )
            {
                switch( theProperty )
                {
                    // empty case, do nothing
                    case "":
                        break;

                    // delegate to provider
                    default:
                        // default is get the first option
                        theValue = options[0];
                        break;
                }
            }
            return theValue;
        }

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used inside markup tags to represent the property value</returns>
        virtual public string GetInlineStringForProperty( DependencyProperty theProperty )
        {
            return GetInlineStringForProperty(
                theProperty.PropertyType.FullName,
                String.Format("{0}.{1}", theProperty.PropertyType.Name, theProperty.Name )
                );
        }

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used between markup complex property tags to represent the property value.</returns>
        virtual public string GetXMLStringForProperty( DependencyProperty theProperty )
        {
            return GetXMLStringForProperty(
                theProperty.PropertyType.FullName,
                String.Format("{0}.{1}", theProperty.PropertyType.Name, theProperty.Name )
                );
        }

        /// <summary>
        /// Gets a value for a given property, of its appropriate type
        /// </summary>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>An object that can be used on the given propery.</returns>
        virtual public object GetValueForProperty( DependencyProperty theProperty )
        {
            return GetValueForProperty(
                theProperty.PropertyType.FullName,
                String.Format("{0}.{1}", theProperty.PropertyType.Name, theProperty.Name )
                );
        }


    }

    /// <summary>
    /// Associates a property with a default value
    /// </summary>
    public class MarkUpDataAssociation : DataAssociation
    {

        /// <summary>
        /// Constructor requires a data provider and translator
        /// </summary>
        public MarkUpDataAssociation( DataProvider prv, DataTranslator trn ) : base( prv, trn ) {}
        /// <summary>
        /// Empty constructor uses default data provider and translator
        /// </summary>
        public MarkUpDataAssociation() : base() {}

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theType">Actual Type name as given by "x.GetType().FullName".</param>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used inside markup tags to represent the property value</returns>
        public override string GetInlineStringForProperty( string theType, string theProperty )
        {
            string markup = null;

            switch( theProperty )
            {
                // empty case, do nothing
                case "":
                    break;

                // delegate to parent
                default:
                    markup = base.GetInlineStringForProperty( theType, theProperty );
                    break;
            }
            return markup;
        }
    }


    /// <summary>
    /// Associates a property with a default animated value
    /// </summary>
    public class AnimationDataAssociation : DataAssociation
    {


        /// <summary>
        /// Constructor requires a data provider and translator
        /// </summary>
        internal AnimationDataAssociation( DataProvider prv, DataTranslator trn ) : base( prv, trn ) {}
        /// <summary>
        /// Empty constructor uses default data provider and translator
        /// </summary>
        internal AnimationDataAssociation() : base() {}
        /// <summary>
        /// constructor uses default data provider and translator
        /// </summary>
        public AnimationDataAssociation(Nullable<TimeSpan> begin, Duration duration, DependencyObject source ) : base()
        {
            // set key local values
            _begin = begin;
            _duration = duration;
            _source = source;

            // get components from factory
            this.provider = DataFactory.GetProvider( this.GetType().Name );
            this.translator = DataFactory.GetTranslator( this.GetType().Name );
        }


        protected Nullable<TimeSpan> _begin;
        protected Duration _duration;
        protected DependencyObject _source;

        // 
        private double _lowMultiply = 0.3;
        private double _highMultiply = 1.5;
        private double _distinctValue = 100.5;
        private int _discreteDefaultSize = 50;

        /// <summary>
        /// Gets markup for a given property
        /// </summary>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>A string that can be used inside markup tags to represent the property value</returns>
        public override string GetInlineStringForProperty( DependencyProperty theProperty )
        {
            string markup = null;
            return markup;
        }

        /// <summary>
        /// Gets a value for a given property, of its appropriate type
        /// </summary>
        /// <param name="theProperty">Property of the given type, in the form "Owner.Name".</param>
        /// <returns>An object that can be used on the given propery as a parameter to x.SetAnimations().</returns>
        public override object GetValueForProperty( DependencyProperty theProperty )
        {
            object theValue = null;
            object currentValue = null;
            string theType = theProperty.PropertyType.FullName;
            string thePropertyName = String.Format("{0}.{1}", theProperty.PropertyType.Name, theProperty.Name );

            // get original value
            if (_source != null)
            {
                // get current value from source element, and add a few variants based on numerics
                currentValue = _source.GetValue(theProperty);

                // get available options for this type. We pass in the original property name as well
                // because the property might say that it's a System.Object, but that doesn't necessarily
                // mean that we can just create an array of random objects and try to assign them.
                object[] options = provider.GetDataForType(theType, theProperty.Name);

                // for a few numeric types, calculate numerically
        
                switch (theType)
                {
                    case "System.Windows.Point":
                        {


                            Point startValue = new Point(0, 0);
                            Point endValue = new Point(100, 200);

                            // create animation
                            PointAnimation animation = new PointAnimation();
                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.From = startValue;
                            animation.To = endValue;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Windows.Rect":
                        {


                            Rect startValue = new Rect(new Point(0, 0), new Point(10, 10));
                            Rect endValue = new Rect(new Point(100, 100), new Point(200, 200));

                            // create animation
                            RectAnimation animation = new RectAnimation();
                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.From = startValue;
                            animation.To = endValue;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Windows.Media.Color":
                        {
                            Color startValue = Colors.Red;
                            Color endValue = Colors.Blue;

                            // create animation
                            ColorAnimation animation = new ColorAnimation();
                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.From = startValue;
                            animation.To = endValue;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Windows.Size":
                        {
                            Size startValue = new Size(10, 20);
                            Size endValue = new Size(20, 40);

                            // create animation
                            SizeAnimation animation = new SizeAnimation();
                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.From = startValue;
                            animation.To = endValue;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Double":
                        {
                            if (currentValue == null || Double.IsNaN((double)currentValue))
                            {
                                currentValue = 1.0;
                            }

                            double startValue = (double)currentValue * _lowMultiply;
                            double endValue = (double)currentValue * _highMultiply;
                            // when both values are equal (zero), bake the end at 100
                            if (startValue == endValue)
                            {
                                endValue = _distinctValue;
                            }
                            // create animation
                            DoubleAnimation animation = new DoubleAnimation();
                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.From = startValue;
                            animation.To = endValue;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Windows.Thickness":
                        {
                            if (currentValue == null) currentValue = new Thickness(1.0);

                            Thickness startValue = new Thickness(
                                ((Thickness)currentValue).Top * _lowMultiply,
                                ((Thickness)currentValue).Bottom * _lowMultiply,
                                ((Thickness)currentValue).Left * _lowMultiply,
                                ((Thickness)currentValue).Right * _lowMultiply
                                );
                            Thickness endValue = new Thickness(
                                ((Thickness)currentValue).Top * _highMultiply,
                                ((Thickness)currentValue).Bottom * _highMultiply,
                                ((Thickness)currentValue).Left * _highMultiply,
                                ((Thickness)currentValue).Right * _highMultiply
                                );
                            if (startValue == endValue)
                            {
                                endValue = new Thickness(_distinctValue);
                            }
                            // fake thickness using discrete
                            options = new object[_discreteDefaultSize];
                            double dTop = (endValue.Top - startValue.Top) / (double)_discreteDefaultSize;
                            double dBottom = (endValue.Bottom - startValue.Bottom) / (double)_discreteDefaultSize;
                            double dLeft = (endValue.Left - startValue.Left) / (double)_discreteDefaultSize;
                            double dRight = (endValue.Right - startValue.Right) / (double)_discreteDefaultSize;
                            for (int i = 0; i < _discreteDefaultSize; i++)
                            {
                                options[i] = new Thickness(
                                    startValue.Top + (dTop * i),
                                    startValue.Bottom + (dBottom * i),
                                    startValue.Left + (dLeft * i),
                                    startValue.Right + (dRight * i)
                                );
                            }
                            // create animation
                            DiscreteAnimation animation = new DiscreteAnimation(options);

                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;
                    case "System.Int32":
                        {
                            int currentValueInt;
                            // filter out missing values
                            if (currentValue == null)
                                currentValueInt = (int)Math.Round(_distinctValue);
                            else
                                currentValueInt = (int)currentValue;
                            // filter out negative values and 1
                            if (currentValueInt <= 1)
                                currentValueInt = (int)Math.Round(_distinctValue);
                            // filter out huge values
                            if (currentValueInt > 1000)
                                currentValueInt = (int)Math.Round(_distinctValue);

                            // create min-max values
                            int startValue = (int)((double)currentValueInt * _lowMultiply);
                            int endValue = (int)((double)currentValueInt * _highMultiply);

                            options = new object[endValue - startValue];
                            for (int i = 0; i < options.Length; i++)
                            {
                                options[i] = (startValue + i);
                            }
                            // create animation
                            DiscreteAnimation animation = new DiscreteAnimation(options);

                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            // set return value
                            theValue = animation;
                        }
                        break;

                    default:
                        // default is to get value from provider ...
                        // if we have an option
                        if (options != null)
                        {
                            // create discrete animation, which will cycle through the
                            // options collection for values.
                            DiscreteAnimation animation = new DiscreteAnimation(options);

                            // complete animation
                            animation.BeginTime = _begin;
                            animation.Duration = _duration;
                            animation.RepeatBehavior = RepeatBehavior.Forever;
                            animation.AutoReverse = true;

                            theValue = animation;
                        }
                        break;
                }
            }

            // return the animation or null

            return theValue;
        }

    }


    /* Extra elements */

    /// <summary>
    /// Sample syle selector class
    /// </summary>
    public class HashStyleSelector : StyleSelector
    {
        /// <summary>
        /// Override this method to return an app specific <seealso cref="Style"/>.
        /// </summary>
        /// <param name="item">The data content</param>
        /// <returns>a app specific style to apply.</returns>
        public override System.Windows.Style SelectStyle(object item, DependencyObject container)
        {
            if ( item == null ) return null;
            return s_dataObject[ Math.Abs(item.GetHashCode()) % s_dataObject.Length ];
        }

        /// <summary>
        /// Override this method to return an app specific <seealso cref="Style"/>.
        /// </summary>
        static HashStyleSelector()
        {
            // create some styles
            s_dataObject = new System.Windows.Style[] {
                new System.Windows.Style( typeof(System.Windows.Controls.Button) ) ,
                new System.Windows.Style( typeof(System.Windows.Documents.Hyperlink)) ,
                new System.Windows.Style() ,
                };
        }
        static System.Windows.Style[] s_dataObject;

    }

}

