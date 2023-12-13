// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Assembly-level test case metadata for EditingTestPart1

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.TextEditing;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

namespace Microsoft.Test.Editing
{
    /// <summary>Test case data for test cases in this assembly.</summary>
    [TestCaseDataTableClass]
    public static class Part1AssemblyTestCaseData
    {
        /// <summary>Test case data for test cases in this assembly.</summary>
        [TestCaseDataTable]
        public static TestCaseData[] Data = new TestCaseData[] {                      
            new TestCaseData(typeof(IMEMaxLengthTest), "",
                new Dimension("maxLength", new object[]{0, 1, 4}),
                new Dimension("locale", GetEnumValues(typeof(IMELocales))),                
                new Dimension("existingContentLength", new object[]{0,1,4,5}),                
                new Dimension("isUndoEnabled", BooleanValues)),         
       
            new TestCaseData(typeof(IMETypingOverSelection), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEConversionModeTest), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEMaxLengthUndoTest), "",
                new Dimension("maxLength", new object[]{2}),
                new Dimension("locale", GetEnumValues(typeof(IMELocales))),                
                new Dimension("isUndoEnabled", BooleanValues)),
                
            new TestCaseData(typeof(IMEMaxLengthEventsTest), "",
                new Dimension("maxLength", new object[]{2}),
                new Dimension("locale", GetEnumValues(typeof(IMELocales))),                
                new Dimension("isUndoEnabled", BooleanValues)),
            new TestCaseData(typeof(IMETextCompositionVerification), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMETableEditing), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEInsertDeleteTest), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMETextSelectionTest), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEVerifyAutoFinalization), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEUndoRedoTest), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(IMEInputOverMixedContent), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(VerifyBackSpaceInKoreanIME), "",                
                new Dimension("locale", GetEnumValues(typeof(IMELocales)))),

            new TestCaseData(typeof(SetPreferredImeConversionModeTest), "",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")})),                

            new TestCaseData(typeof(SelectionRenderingTest), "Pri=0",               
                new Dimension("testControlType", new object[]{typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox)}),                         
                new Dimension("useDependencyPropertyGetSetMethods", BooleanValues),
                new Dimension("testSelectionBrushColorValue", new object[]{"Default", "Null", "Red", "AlphaRed"}),
                new Dimension("testSelectionOpacityValue", new object[]{"Default", "0", "0.6", "1"}),                
                new Dimension("testContent", new object[]{"     \r\n     ", "abcdef\r\n123456"})),

            new TestCaseData(typeof(SelectionRenderingTest), "Pri=1",               
                new Dimension("testControlType", new object[]{typeof(FlowDocumentReader), typeof(FlowDocumentPageViewer), typeof(FlowDocumentScrollViewer)}),                         
                new Dimension("useDependencyPropertyGetSetMethods", BooleanValues),
                new Dimension("testSelectionBrushColorValue", new object[]{"Default", "Null", "Red", "AlphaRed"}),
                new Dimension("testSelectionOpacityValue", new object[]{"Default", "0", "0.6", "1"}),                
                new Dimension("testContent", new object[]{"     \r\n     ", "abcdef\r\n123456"})),

            new TestCaseData(typeof(NonAdornerSelectionRenderingTest), "Pri=0",
                new Dimension("testControlType", new object[]{typeof(TextBox), typeof(RichTextBox), typeof(PasswordBox)}),
                new Dimension("useDependencyPropertyGetSetMethods", BooleanValues),
                new Dimension("testSelectionBrushColorValue", new object[]{"Default", "Null", "Red", "AlphaRed"}),
                new Dimension("testSelectionTextBrushColorValue", new object[]{"Default", "Null", "Yellow", "Green"}),
                new Dimension("testSelectionOpacityValue", new object[]{"Default", "0", "0.6", "1"}),
                new Dimension("testContent", new object[]{"     \r\n     ", "abcdef\r\n123456"})),

            new TestCaseData(typeof(NonAdornerSelectionRenderingTest), "Pri=1",
                new Dimension("testControlType", new object[]{typeof(FlowDocumentReader), typeof(FlowDocumentPageViewer), typeof(FlowDocumentScrollViewer)}),
                new Dimension("useDependencyPropertyGetSetMethods", BooleanValues),
                new Dimension("testSelectionBrushColorValue", new object[]{"Default", "Null", "Red", "AlphaRed"}),
                new Dimension("testSelectionOpacityValue", new object[]{"Default", "0", "0.6", "1"}),
                new Dimension("testContent", new object[]{"     \r\n     ", "abcdef\r\n123456"})),

            new TestCaseData(typeof(SelectionRenderingStyleAndTriggerTest), "",               
                new Dimension("testControlType", new object[]{typeof(RichTextBox), typeof(TextBox), typeof(PasswordBox), typeof(FlowDocumentPageViewer), typeof(FlowDocumentReader), typeof(FlowDocumentScrollViewer)}),                         
                new Dimension("testSelectionBrushColorValue", new object[]{"Red", "AlphaRed"})),                

            new TestCaseData(typeof(CaretBrushRenderingTest), "",
                new Dimension("testControlType", TextEditableType.PlatformTypes),
                new Dimension("useDependencyPropertyGetSetMethods", BooleanValues),
                new Dimension("testCaretBrushColorValue", new object[]{"Default", "Null", "Red", "AlphaRed"}),
                // Using space as the content so that render verification becomes easy without text overlap with caret
                new Dimension("testContent", new object[]{"     \r\n     "})),

            new TestCaseData(typeof(CaretBrushRenderingStyleAndTriggerTest), "",
                new Dimension("testControlType", TextEditableType.PlatformTypes),                                
                new Dimension("testCaretBrushColorValue", new object[]{"Red", "AlphaRed"})),  

            new TestCaseData(typeof(BindingSelectionAndCaretBrushTest), "",
                new Dimension("testControlType", new object[]{typeof(RichTextBox), typeof(TextBox), typeof(PasswordBox), typeof(FlowDocumentPageViewer), typeof(FlowDocumentReader), typeof(FlowDocumentScrollViewer)})), 

            new TestCaseData(typeof(IsReadOnlyCaretVisibleTest), "",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("isReadOnlyCaretVisible", BooleanValues)),
                
            new TestCaseData(typeof(CustomSpellerDictionaryTest), "Pri=0",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("testContent", new object[]{"no error string", "xmadeupstringx", "madeupstringx", "Ymadeupstringy", "Ymadeupstringtyq", "nebocnor", "flowe", "lexicona"}),
                new Dimension("testCustomDictionaryUris", new object[]                                           
                {
                    new Uri[] {
                        new Uri(Environment.CurrentDirectory + "\\CustomSpellerDictionary1.lex", UriKind.RelativeOrAbsolute)                       
                    }                   
                }),                              
                new Dimension("testClearCustomDictionary", BooleanValues),                
                new Dimension("testIgnoreSpellingError", BooleanValues)),  
                            
            new TestCaseData(typeof(CustomSpellerDictionaryTest), "Pri=1",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("testContent", new object[]{"xmadeupstringx", "Ymadeupstringy"}),
                new Dimension("testInvalidDictionaryLoad", BooleanValues),
                new Dimension("testCustomDictionaryUris", new object[]
                {                    
                    new Uri[] {
                        new Uri(Environment.CurrentDirectory + "\\CustomSpellerDictionary2.lex", UriKind.Absolute),
                        new Uri(Environment.CurrentDirectory + "\\CustomSpellerDictionary3.lex", UriKind.Absolute)
                    },
                    new Uri[] {
                        new Uri("CustomSpellerDictionary1.lex", UriKind.Relative)                       
                    }, 
                    new Uri[] {
                        new Uri("pack://application:,,,/EditingTestPart1;component/CustomSpellerDictionaryResourceLocal.lex")                       
                    },
                    new Uri[] {
                        new Uri("pack://application:,,,/EditingTestLib;component/CustomSpellerDictionaryResourceReferenced.lex")                       
                    },
                    new Uri[] {
                        //the file "\\scratch2\scratch\tanujak\Editing\CustomSpellerDictionaryUNC.lex" moved to "\\wpf\TestScratch\WPFTest\Editing\"
                        new Uri(@"\\wpf\TestScratch\WPFTest\Editing\CustomSpellerDictionaryUNC.lex", UriKind.Absolute)
                    }                                       
                })),                              
 
            new TestCaseData(typeof(CustomSpellerDictionaryTest), "Pri=2",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("testContent", new object[]{"xmadeupstringx", "Ymadeupstringy"}),
                new Dimension("testCustomDictionaryUris", new object[]
                {                    
                    new Uri[] {
                        new Uri(Environment.CurrentDirectory + "\\CustomSpellerDictionary1.lex", UriKind.RelativeOrAbsolute)                       
                    },
                    new Uri[] {
                        new Uri("pack://application:,,,/EditingTestPart1;component/CustomSpellerDictionaryResourceLocal.lex")                       
                    }
                }),  
                new Dimension("testDisableEnableSpellCheck", BooleanValues),
                new Dimension("testEnableSpellCheckBeforeDictionaryCreation", BooleanValues),
                new Dimension("testContentLoadedBeforeDictionaryCreation", BooleanValues)),  
              
            new TestCaseData(typeof(CustomSpellerDictionaryTest), "Pri=3",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),                
                new Dimension("testContent", new object[]{"xmadeupstringx"}),
                new Dimension("testXamlUriType", new object[]{"local"}),
                new Dimension("testDisableEnableSpellCheck", BooleanValues),
                new Dimension("testInvalidDictionaryLoad", BooleanValues),
                new Dimension("testContentLoadedBeforeDictionaryCreation", BooleanValues),
                new Dimension("testClearCustomDictionary", BooleanValues),   
                new Dimension("testEnableSpellCheckBeforeDictionaryCreation", BooleanValues),
                new Dimension("testXaml", new object[] {true})),  

            new TestCaseData(typeof(CustomSpellerDictionaryTest), "Pri=4",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),                
                new Dimension("testContent", new object[]{"xmadeupstringx"}),
                new Dimension("testXamlUriType", new object[]{"ResourceLocal", "ResourceReferenced", "UNC", "Multiple"}),                                               
                new Dimension("testXaml", new object[] {true})), 

            new TestCaseData(typeof(CustomSpellerDictionaryLocaleTest), "",
                new Dimension("testControlType", new object[]{TextEditableType.GetByName("TextBox"), TextEditableType.GetByName("RichTextBox")}),
                new Dimension("testContent", new object[]
                {
                    "EnglishLangxx FrenchLangxx GermanLangxx SpanishLangxx JapaneseLangxx", 
                    "EnglishLangxxw FrenchLangxxw GermanLangxxw SpanishLangxxw JapaneseLangxxw"
                }),
                new Dimension("testStartLocale", new object[] {"english", "french", "german", "spanish", "japanese"}),   
                new Dimension("testEndLocale", new object[] {"english", "french", "german", "spanish", "japanese"})),               

            new TestCaseData(typeof(CustomSpellerDictionaryRTBMixedRunsTest), "",                
                new Dimension("testRTBXmlLanguage", new object[] {"english", "french", "german", "spanish", "japanese"})),  
	    
	        new TestCaseData(typeof(Regression_Bug71), "",
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("FontSize",   new object[] {1,2,3,4,5,6,7,8,9,10,11,12,16,17,18,26,30}),
                new Dimension("FontFamily",   new object [] {"Times New Roman","Tahoma","Verdana","Arial","MS Gothic"})),

            new TestCaseData(typeof(TextBoxLineUpLineDownTests), "",
                new Dimension("EditableType", new object[]{TextEditableType.GetByName("TextBox")}),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("InputSwitch",    GetEnumValues(typeof(InputTrigger))),
                new Dimension("FontSize",   new object [] {20, 40, 80}),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("FontFamily",   new object [] {"Times New Roman","Comic", "Tahoma","Verdana"})),        

            new TestCaseData(typeof(TextBoxGetLineLengthAndLineText), "",
                new Dimension("EditableType",  TextEditableType.Values),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("InputStringDataSwitch",    GetEnumValues(typeof(InputStringData)))),

            new TestCaseData(typeof(TextBoxGetFirstLastVisibleIndexTests), "",
                new Dimension("EditableType",  TextEditableType.Values),
                new Dimension("TextWrap",  BooleanValues),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("MultiLine",  BooleanValues)),

            new TestCaseData(typeof(ScrollingPageupDownLineUpDownTests), "",
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxGetLineCharacterIndex), "",
                new Dimension("EditableType",           TextEditableType.Values),
                new Dimension("FlowDirectionProperty",   BooleanValues),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("InputStringSwitch",      GetEnumValues(typeof(InputStringDataChoices))),
                new Dimension("FunctionNameSwitch",      GetEnumValues(typeof(FunctionChoices)))),

            new TestCaseData(typeof(TextBoxScrollFunctionTests), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("LargeMultiLineContent",  new object[] { true }),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("WrapText",  BooleanValues),
                new Dimension("TextBoxScrollSwitch",  GetEnumValues(typeof(TextBoxScrollFunction)))),

            new TestCaseData(typeof(ScrollingPageupDownLineUpDownTests), "",
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("EditableType", TextEditableType.Values)),

            new TestCaseData(typeof(TextBoxBaseScrollFunctionsLineTests), "",
                new Dimension("EditableType", TextEditableType.Values),
                new Dimension("textFormattingMode", new object[] {"Ideal","Display"}),
                new Dimension("LargeMultiLineContent",  BooleanValues),
                new Dimension("WrapText",  BooleanValues),
                new Dimension ("FunctionSwitch", GetEnumValues(typeof(FunctionName)))),
        };
       

        #region Public properties.

        /// <summary>Array with static boolean values.</summary>
        public static object[] BooleanValues
        {
            get
            {
                if (s_booleanValues == null)
                {
                    s_booleanValues = new object[] { true, false };
                }
                return s_booleanValues;
            }
        }

        #endregion


        #region Private methods

        // Returns an object array with all values defined in thespecified enumeration.        
        private static object[] GetEnumValues(Type enumType)
        {
            object[] result;
            Array values;

            if (enumType == null)
            {
                throw new ArgumentNullException("enumType");
            }

            // Simple typecasting from System.Array to System.Object[] fails.
            values = System.Enum.GetValues(enumType);
            result = new object[values.Length];
            values.CopyTo(result, 0);
            return result;
        }

        #endregion


        #region Private fields.

        /// <summary>Array with static boolean values.</summary>
        private static object[] s_booleanValues;

        #endregion
    }
}
