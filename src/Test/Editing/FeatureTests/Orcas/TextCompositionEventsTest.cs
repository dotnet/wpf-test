// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Verified TextComposition events fire properly and
//  args parameter have the right values.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

using Microsoft.Test;
using Microsoft.Test.Discovery;

using Test.Uis.Data;
using Test.Uis.IO;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;
using UisWin32 = Test.Uis.Wrappers.Win32;

namespace Test.Uis.TextEditing
{
    /// <summary>Tests TextComposition events and their args parameters</summary>
    [Test(0, "IME", "TextCompositionEventsTest_Japanese", MethodParameters = "/TestCaseType=TextCompositionEventsTest /TestLocale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "TextCompositionEventsTest_Korean", MethodParameters = "/TestCaseType=TextCompositionEventsTest /TestLocale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    public class TextCompositionEventsTest : CustomTestCase
    {
        #region Private fields

        private TextBox _textBox;
        private RichTextBox _richTextBox;
        private UIElementWrapper _wrapper;        
        private KeyboardLayout _installedKeyboardLayout;

        private bool _enableCompositionEventHandlers = true;
        private int _startCompEventCount,_updateCompEventCount,_completeCompEventCount;
        
        private string _expectedCompositedString;
        private int[] _expectedEventCount;

        //These expected values are calculated for the strings in GlobalStringData. 
        //If those strings change, the expected values have to be changed.
        //Changes in the Ime code or WPF Editing code can also regress these values.
        private readonly int[] _expectedEventCountForJapaneseInXP = new int[] { 6, 56, 6 };
        private readonly int[] _expectedEventCountForJapaneseInVista = new int[] { 4, 56, 4 };
        private readonly int[] _expectedEventCountForKorean = new int[] { 14, 14, 14 };
        private readonly int[] _expectedEventCountForChinesePinyin = new int[] { 0, 0, 0 };
        private readonly int[] _expectedEventCountForChineseQuanPin = new int[] { 0, 0, 0 };

        private Dictionary<string, int[]> _expectedEventCountForIme = new Dictionary<string, int[]>();

        //Time to wait for Cicero updates, in milliseconds.
        private const int ciceroWaitTimeMs = 450;

        //Time to wait for IME to come up properly, in milliseconds.
        private const int imeWaitTimeMs = 1000;

        #endregion

        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            StackPanel panel = new StackPanel();

            _textBox = new TextBox();
            _textBox.AcceptsReturn = true;
            _textBox.FontSize = 24;
            _textBox.Height = 200;
            TextCompositionManager.AddPreviewTextInputStartHandler(_textBox, PreviewTextInputStartHandler_TextBox);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(_textBox, PreviewTextInputUpdateHandler_TextBox);
            TextCompositionManager.AddPreviewTextInputHandler(_textBox, PreviewTextInputHandler_TextBox);                

            _richTextBox = new RichTextBox();
            _richTextBox.FontSize = 24;
            _richTextBox.Height = 200;
            TextCompositionManager.AddPreviewTextInputStartHandler(_richTextBox, PreviewTextInputStartHandler_RichTextBox);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(_richTextBox, PreviewTextInputUpdateHandler_RichTextBox);
            TextCompositionManager.AddPreviewTextInputHandler(_richTextBox, PreviewTextInputHandler_RichTextBox);                

            panel.Children.Add(_textBox);
            panel.Children.Add(_richTextBox);

            MainWindow.Content = panel;
            
            _expectedEventCountForIme.Add("ExpectedEventCountForKorean", _expectedEventCountForKorean);
            _expectedEventCountForIme.Add("ExpectedEventCountForJapaneseInVista", _expectedEventCountForJapaneseInVista);
            _expectedEventCountForIme.Add("ExpectedEventCountForJapaneseInXP", _expectedEventCountForJapaneseInXP);            
            _expectedEventCountForIme.Add("ExpectedEventCountForChinesePinyin", _expectedEventCountForChinesePinyin);
            _expectedEventCountForIme.Add("ExpectedEventCountForChineseQuanPin", _expectedEventCountForChineseQuanPin);

            QueueDelegate(PerformTestAction);
        }        

        private void PerformTestAction()
        {
            string actualCompositedString;
            IMELocales testLocale = (IMELocales)Enum.Parse(typeof(IMELocales), ConfigurationSettings.Current.GetArgument("TestLocale", true));            

            PerformInputAction(_textBox, testLocale);

            //Verification            
            Log("TextBox (Actual): StartCompositionEventCount [" + _startCompEventCount + "] UpdateCompositionEventCount [" + 
                _updateCompEventCount + "] CompleteCompositionEventCount [" + _completeCompEventCount + "]");
            Log("TextBox (Expected): StartCompositionEventCount [" + _expectedEventCount[0] + "] UpdateCompositionEventCount [" +
                _expectedEventCount[1] + "] CompleteCompositionEventCount [" + _expectedEventCount[2] + "]");
            actualCompositedString = _wrapper.Text;
            Log("TextBox: ExpectedCompositedString [" + _expectedCompositedString + "]");
            Log("TextBox: ActualCompositedString [" + actualCompositedString + "]");
            Verifier.Verify(String.Compare(actualCompositedString, _expectedCompositedString, StringComparison.InvariantCulture) == 0, 
                "TextBox: Verifying the composited string typed", true);
            Verifier.Verify(((_startCompEventCount == _expectedEventCount[0]) &&
                (_updateCompEventCount == _expectedEventCount[1]) &&
                (_completeCompEventCount == _expectedEventCount[2])), "Verifying composition event count", true);

            PerformInputAction(_richTextBox, testLocale);            

            //Verification
            Log("RichTextBox (Actual): StartCompositionEventCount [" + _startCompEventCount + "]\r\nUpdateCount [" +
                _updateCompEventCount + "]\r\nCompleteCount [" + _completeCompEventCount + "]");
            Log("RichTextBox (Expected): StartCompositionEventCount [" + _expectedEventCount[0] + "]\r\nUpdateCount [" +
                _expectedEventCount[1] + "]\r\nCompleteCount [" + _expectedEventCount[2] + "]");
            actualCompositedString = _wrapper.Text;
            //Trim the new line which happens in RichTextBox when accessing through wrapper.Text
            if (actualCompositedString.EndsWith("\r\n"))
            {
                actualCompositedString = actualCompositedString.Substring(0, actualCompositedString.Length - 2);
            }
            Log("TextBox: ExpectedCompositedString [" + _expectedCompositedString + "]");
            Log("TextBox: ActualCompositedString [" + actualCompositedString + "]");

            //Regression_Bug105: Due to this bug skip verification in Korean IME            
            if (testLocale != IMELocales.Korean)
            {
                Verifier.Verify(String.Compare(actualCompositedString, _expectedCompositedString, StringComparison.InvariantCulture) == 0,
                    "RichTextBox: Verifying the composited string typed", true);
            }            
            Verifier.Verify(((_startCompEventCount == _expectedEventCount[0]) &&
                (_updateCompEventCount == _expectedEventCount[1]) &&
                (_completeCompEventCount == _expectedEventCount[2])), "Verifying composition event count", true);

            Logger.Current.ReportSuccess();
        }

        private void PerformInputAction(TextBoxBase textBoxBase, IMELocales testLocale)
        {
            Log("Testing in " + textBoxBase.GetType().Name + "...");
            textBoxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            _wrapper = new UIElementWrapper(textBoxBase);
            _startCompEventCount = _updateCompEventCount = _completeCompEventCount = 0;
            DoImeInput(testLocale);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
        }        

        private void DoImeInput(IMELocales testLocale)
        {            
            Log("Loading keyboard layout for " + testLocale.ToString());
            switch (testLocale)
            {
                case IMELocales.Korean:                    
                    InstallImeKeyboardLayout(KeyboardLayouts.Korean, InputLocaleData.Korean.Identifier);

                    //Check and toggle to Hangul mode
                    _enableCompositionEventHandlers = false;
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    if (_wrapper.Text[0] != '\u3141') //Unicode code point for korean character 'a'
                    {
                        Log("Typing the Kana key to change the mode to Hangul in KoreanIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANA);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANA);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    _enableCompositionEventHandlers = true;

                    //Type content     
                    GlobalStringData koreanStringData = new GlobalStringData("Korean");
                    KeyboardInput.TypeString(koreanStringData.StringTypeSequence.Substring(0, koreanStringData.StringTypeSequence.Length - 10 /*remove the numbers*/));

                    _expectedCompositedString = koreanStringData.CompositedString.Substring(0, koreanStringData.CompositedString.Length - 10 /*remove the numbers*/);                    
                    _expectedEventCount = _expectedEventCountForIme["ExpectedEventCountForKorean"];
                    break;
                case IMELocales.Japanese:                    
                    InstallImeKeyboardLayout(KeyboardLayouts.Japanese, InputLocaleData.JapaneseMsIme2002.Identifier);                    

                    //Check and toggle to Hiragana mode
                    _enableCompositionEventHandlers = false;
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    if (_wrapper.Text[0] != '\u3042') //Unicode code point for japanese character 'a'
                    {
                        Log("Typing the Kanji key to change the mode to Hiragana in JapaneseIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANJI);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANJI);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    _enableCompositionEventHandlers = true;

                    //Type content      
                    GlobalStringData japaneseStringData = new GlobalStringData("Japanese");              
                    KeyboardInput.TypeString(japaneseStringData.StringTypeSequence);

                    _expectedCompositedString = japaneseStringData.CompositedString;                                     
                    if (System.Environment.OSVersion.Version.Major >= 6)
                    {
                        _expectedEventCount = _expectedEventCountForIme["ExpectedEventCountForJapaneseInVista"]; ;
                    }
                    else
                    {
                        _expectedEventCount = _expectedEventCountForIme["ExpectedEventCountForJapaneseInXP"];
                    }
                    break;
                case IMELocales.ChinesePinyin:                    
                    InstallImeKeyboardLayout(KeyboardLayouts.ChinesePinyin, InputLocaleData.Chinese.Identifier);                    

                    //Type content         
                    GlobalStringData chinesePinyinStringData = new GlobalStringData("ChinesePinyin");
                    KeyboardInput.TypeString(chinesePinyinStringData.StringTypeSequence);

                    _expectedCompositedString = chinesePinyinStringData.CompositedString;                    
                    _expectedEventCount = _expectedEventCountForIme["ExpectedEventCountForChinesePinyin"];
                    break;
                case IMELocales.ChineseQuanPin:                    
                    InstallImeKeyboardLayout(KeyboardLayouts.ChineseQuanPin, InputLocaleData.Chinese.Identifier);                    

                    //Type content         
                    GlobalStringData chineseQuanPinStringData = new GlobalStringData("ChineseQuanPin");
                    KeyboardInput.TypeString(chineseQuanPinStringData.StringTypeSequence);

                    _expectedCompositedString = chineseQuanPinStringData.CompositedString;                    
                    _expectedEventCount = _expectedEventCountForIme["ExpectedEventCountForChineseQuanPin"];
                    break;
            }
        }

        #endregion

        #region Event handlers

        private void PreviewTextInputStartHandler_TextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForStartOrUpdateEvent(sender, e);

                _startCompEventCount++;
            }
        }

        private void PreviewTextInputUpdateHandler_TextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForStartOrUpdateEvent(sender, e);

                _updateCompEventCount++;
            }
        }

        private void PreviewTextInputHandler_TextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForCompleteEvent(sender, e);

                _completeCompEventCount++;
            }
        }

        private void PreviewTextInputStartHandler_RichTextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForStartOrUpdateEvent(sender, e);

                _startCompEventCount++;
            }
        }

        private void PreviewTextInputUpdateHandler_RichTextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForStartOrUpdateEvent(sender, e);

                _updateCompEventCount++;
            }
        }

        private void PreviewTextInputHandler_RichTextBox(object sender, TextCompositionEventArgs e)
        {
            if (_enableCompositionEventHandlers)
            {
                VerifyForCompleteEvent(sender, e);

                _completeCompEventCount++;
            }
        }

        #endregion

        #region Verification & Helpers

        private void VerifyForStartOrUpdateEvent(object sender, TextCompositionEventArgs e)
        {            
            if (sender is TextBox)
            {
                //For TextBox, TextComposition should be of type FrameworkTextComposition
                if (!(e.TextComposition is FrameworkTextComposition))
                {
                    throw new ApplicationException("TextBox: e.TextComposition is not FrameworkTextComposition");
                }

                FrameworkTextComposition frameworkTextComposition = (FrameworkTextComposition)e.TextComposition;
                //ResultOffset & ResultLength should be -1 for Start/Update Composition Events
                if ((frameworkTextComposition.ResultOffset != -1) || (frameworkTextComposition.ResultLength != -1))
                {
                    throw new ApplicationException("TextBox: ResultOffset/ResultLength is != -1 during Start/Update Composition Events");
                }
            }
            else
            {
                //For RichTextBox, TextComposition should be of type FrameworkRichTextComposition
                if (!(e.TextComposition is FrameworkRichTextComposition))
                {
                    throw new System.Exception("RichTextBox: e.TextComposition is not FrameworkRichTextComposition");
                }

                FrameworkRichTextComposition frameworkRichTextComposition = (FrameworkRichTextComposition)e.TextComposition;
                //ResultOffset & ResultLength should be -1 for Start/Update Composition Events
                if ((frameworkRichTextComposition.ResultOffset != -1) || (frameworkRichTextComposition.ResultLength != -1))
                {
                    throw new ApplicationException("RichTextBox: ResultOffset/ResultLength is != -1 during Start/Update Composition Events");
                }
                //ResultStart & ResultEnd should be null for Start/Update Composition Events
                if ((frameworkRichTextComposition.ResultStart != null) || (frameworkRichTextComposition.ResultEnd != null))
                {
                    throw new ApplicationException("RichTextBox: ResultStart/ResultEnd is != null during Start/Update Composition Events");
                }
            }            
        }

        private void VerifyForCompleteEvent(object sender, TextCompositionEventArgs e)
        {            
            if (sender is TextBox)
            {
                //For TextBox, TextComposition should be of type FrameworkTextComposition
                if (!(e.TextComposition is FrameworkTextComposition))
                {
                    throw new ApplicationException("TextBox: e.TextComposition is not FrameworkTextComposition");
                }

                FrameworkTextComposition frameworkTextComposition = (FrameworkTextComposition)e.TextComposition;
                //CompositionOffset & CompositionLength should be -1 for Complete Composition Event
                if ((frameworkTextComposition.CompositionOffset != -1) || (frameworkTextComposition.CompositionLength != -1))
                {
                    throw new ApplicationException("TextBox: CompositionOffset/CompositionLength is != -1 during Complete Composition Event");
                }
            }
            else
            {
                //For RichTextBox, TextComposition should be of type FrameworkRichTextComposition
                if (!(e.TextComposition is FrameworkRichTextComposition))
                {
                    throw new System.Exception("RichTextBox: e.TextComposition is not FrameworkRichTextComposition");
                }

                FrameworkRichTextComposition frameworkRichTextComposition = (FrameworkRichTextComposition)e.TextComposition;
                //CompositionOffset & CompositionLength should be -1 for Complete Composition Event
                if ((frameworkRichTextComposition.CompositionOffset != -1) || (frameworkRichTextComposition.CompositionLength != -1))
                {
                    throw new ApplicationException("RichTextBox: CompositionOffset/CompositionLength is != -1 during Complete Composition Event");
                }
                //CompositionStart & CompositionEnd should be null for Complete Composition Event
                if ((frameworkRichTextComposition.CompositionStart != null) || (frameworkRichTextComposition.CompositionEnd != null))
                {
                    throw new ApplicationException("RichTextBox: CompositionStart/CompositionEnd is != null during Complete Composition Event");
                }
            }            
        }

        private void InstallImeKeyboardLayout(KeyboardLayout keyboardLayout, string inputLocaleIdentifier)
        {
            _installedKeyboardLayout = keyboardLayout;
            KeyboardLayoutHelper.TryInstallLayout(keyboardLayout);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);
            KeyboardInput.SetActiveInputLocale(inputLocaleIdentifier);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

            KeyboardInput.EnableIME((UIElement)MainWindow.Content);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

            DoFocusChangeOnWindow();
        }

        private void DoFocusChangeOnWindow()
        {
            //Change the focus of the main window to trigger and give time for IME to come up properly
            //Without this in Vista, IME mode is not properly set up
            MainWindow.WindowState = WindowState.Minimized;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
            MainWindow.WindowState = WindowState.Normal;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
        }          

        #endregion                
    }
}