// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Validates IME input in TextBox and RichTextBox

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
    /// <summary>Tests composition events through FilteredTextBox</summary>
    [Test(0, "IME", "ImeFilterTest_Japanese", MethodParameters = "/TestCaseType=ImeFilterTest /TestLocale=Japanese", Timeout = 120, Keywords = "JapaneseIME")]
    [Test(0, "IME", "ImeFilterTest_Korean", MethodParameters = "/TestCaseType=ImeFilterTest /TestLocale=Korean", Timeout = 120, Keywords = "KoreanIME")]
    [Test(2, "IME", "ImeFilterTest_ChinesePinyin", MethodParameters = "/TestCaseType=ImeFilterTest /TestLocale=ChinesePinyin", Timeout = 120, Keywords = "ChinesePinyinIME")]
    [Test(2, "IME", "ImeFilterTest_ChineseQuanPin", MethodParameters = "/TestCaseType=ImeFilterTest /TestLocale=ChineseQuanPin", Timeout = 120, Keywords = "ChineseQuanPinIME")]
    public class ImeFilterTest : CustomTestCase
    {
        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {            
            StackPanel panel = new StackPanel();

            _textBox = new FilteredTextBox(s_filter);
            _textBox.AcceptsReturn = true;
            _textBox.FontSize = 24;
            _textBox.Height = 100;

            _richTextBox = new FilteredRichTextBox(s_filter);
            _richTextBox.Background = System.Windows.Media.Brushes.LightBlue;
            _richTextBox.FontSize = 24;
            _richTextBox.Height = 100;

            //used to set proper Ime setting before testing on FilteredTextBox and FilteredRichTextBox
            _plainTextBox = new TextBox();
            _plainTextBox.AcceptsReturn = true;
            _plainTextBox.Background = System.Windows.Media.Brushes.LightGray;
            _plainTextBox.Height = 50;

            panel.Children.Add(_textBox); 
            panel.Children.Add(_richTextBox);
            panel.Children.Add(_plainTextBox); 
                        
            MainWindow.Content = panel;

            QueueDelegate(PerformTestAction);
        }        

        private void PerformTestAction()
        {
            _testLocale = (IMELocales)Enum.Parse(typeof(IMELocales), ConfigurationSettings.Current.GetArgument("TestLocale", true));

            SetUpImeKeyboardLayout(_plainTextBox);
            
            DoInput(_textBox);

            DoInput(_richTextBox);            

            Logger.Current.ReportSuccess();
        }

        private void SetUpImeKeyboardLayout(TextBoxBase textBoxBase)
        {
            UIElementWrapper wrapper = new UIElementWrapper(textBoxBase);
            wrapper.Element.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            //get operation system
            _os = Environment.OSVersion;
            _ver = _os.Version; 

            Log("Loading keyboard layout for " + _testLocale.ToString());
            switch (_testLocale)
            {
                case IMELocales.Korean:
                    _installedKeyboardLayout = KeyboardLayouts.Korean;
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Korean);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Korean.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)MainWindow.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    DoFocusChangeOnWindow();

                    //Check and toggle to Hangul mode
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    if (wrapper.Text[0] != '\u3141') //Unicode code point for korean character 'a'
                    {
                        Log("Typing the Kana key to change the mode to Hangul in KoreanIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANA);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANA);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);

                    //Type content
                    _contentToType = (new GlobalStringData("Korean").StringTypeSequence);
                    break;

                case IMELocales.Japanese:
                    _installedKeyboardLayout = KeyboardLayouts.Japanese;
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);                  
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.JapaneseMsIme2002.Identifier); 
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)MainWindow.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    DoFocusChangeOnWindow();                  

                    //Check and toggle to Hiragana mode
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
                    if (wrapper.Text[0] != '\u3042') //Unicode code point for japanese character 'a'
                    {
                        Log("Typing the Kanji key to change the mode to Hiragana in JapaneseIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANJI);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANJI);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);

                    //Type content   
                    _contentToType = (new GlobalStringData("Japanese").StringTypeSequence);
                    break;

                case IMELocales.ChinesePinyin:              
                    EnsureCleanChineseImeState();

                    //Install the Ime KeyboardLayout to test
                    //There is "Chinese simplefast" on Win8,
                    //and there are "IME Pinyin" and "IME QuanPin" between win7 and vista,
                    //and others on vista below version    
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1)) 
                    {
                        _installedKeyboardLayout = KeyboardLayouts.ChinesePinyinSimpleFast;
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);                        
                    }
                    else
                    {
                        _installedKeyboardLayout = KeyboardLayouts.ChinesePinyin;
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyin);
                    }
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Chinese.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)MainWindow.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    DoFocusChangeOnWindow();

                    //Type content
                    _contentToType = (new GlobalStringData("ChinesePinyin").StringTypeSequence);
                    break;

                case IMELocales.ChineseQuanPin:
                    EnsureCleanChineseImeState();                    

                    //Install the Ime KeyboardLayout to test
                    //There is "Chinese simplefast" on Win8,
                    //and there are "IME Pinyin" and "IME QuanPin" between win7 and vista,
                    //and others on vista below version                
                    if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1)) 
                    {
                        _installedKeyboardLayout = KeyboardLayouts.ChinesePinyinSimpleFast;
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);
                    }
                    else
                    {
                        _installedKeyboardLayout = KeyboardLayouts.ChineseQuanPin;
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChineseQuanPin);
                    }
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);
                    
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Chinese.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)MainWindow.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);

                    DoFocusChangeOnWindow();

                    //Type content
                    _contentToType = (new GlobalStringData("ChineseQuanPin").StringTypeSequence);
                    break;
            }
        }

        private void DoInput(TextBoxBase textBoxBase)
        {
            Log("Testing on " + textBoxBase.GetType().Name);
            textBoxBase.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            KeyboardInput.TypeString(_contentToType);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(ciceroWaitTimeMs);
            VerifyContents(textBoxBase);            
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

        private void VerifyContents(TextBoxBase textBoxBase)
        {
            UIElementWrapper wrapper = new UIElementWrapper(textBoxBase);   
            string contents = wrapper.Text;
            if (textBoxBase is FilteredRichTextBox || textBoxBase is FilteredTextBox)
            {
                contents = contents.Replace("\r\n", "");
            }
            Log("Contents of " + textBoxBase.GetType().Name + " : [" + contents + "]");

            try
            {
                ICollection<Char> filterCollection = new ReadOnlyCollection<char>(s_filter);
                for (int i = 0; i < contents.Length; i++)
                {
                    if ((!filterCollection.Contains(contents[i])) && (contents[i] != ' '))
                    {
                        Verifier.Verify(false, "Char : [" + contents[i] + "] is not filtered", true);
                    }
                }
            }
            catch (Exception e)
            {
                //Uninstall the keyboardlayout installed in this combination if a tests fails 
                //in the verification for some reason.
                KeyboardLayoutHelper.TryUninstallLayout(_installedKeyboardLayout);

                throw e;
            }
        }

        private void EnsureCleanChineseImeState()
        {
            //Remove any Chinese Ime KeyboardLayouts which are loaded already to ensure proper clean state
            //There is "Chinese simplefast" on Win8,
            //and there are "IME Pinyin" and "IME QuanPin" between win7 and vista,
            //and others on vista below version    
            if (_ver.Major > 6 || ((6 == _ver.Major) && _ver.Minor > 1)) 
            {
                KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);
            }
            else
            {
                KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChinesePinyin);
                KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChineseQuanPin);
            }
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(imeWaitTimeMs);
        }

        #endregion

        #region Private fields

        private static readonly char[] s_filter = new char[]
        {
            // ascii digits
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            // full width digits
            '\uff10', '\uff11', '\uff12', '\uff13', '\uff14', '\uff15', '\uff16', '\uff17', '\uff18', '\uff19',
        };
        
        private IMELocales _testLocale;        
        private TextBox _textBox,_plainTextBox;
        private RichTextBox _richTextBox;        
        private KeyboardLayout _installedKeyboardLayout;
        private string _contentToType;

        //Time to wait for Cicero updates, in milliseconds.
        private const int ciceroWaitTimeMs = 450;

        //Time to wait for IME to come up properly, in milliseconds.
        private const int imeWaitTimeMs = 1000;

        //get OperatingSystem
        OperatingSystem _os;
        Version _ver;
        #endregion
    }
}