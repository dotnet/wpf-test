// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides helper methods for using IME in automated tests

using System;
using System.Windows;
using System.IO;
using System.Xml;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Wrappers;
using UisWin32 = Test.Uis.Wrappers.Win32;

namespace Test.Uis.Utils
{
    public static class IMEHelper
    {
        /// <summary>
        /// Does the necessary steps to setup the IME keyboard specified in the input arguments on a 
        /// control in a active window. 
        /// It requires 
        /// a. Clean TextBox/RichTextBox (no existing content) to set up the appropriate IME mode properly.
        /// This TextBoxBase contents will be cleared while setting up the IME so dont use this TextBoxBase 
        /// for the main test.
        /// b. Locale of the IME to be setup.
        /// c. Active window on which TextBoxBase exists.        
        /// </summary>
        /// <param name="testLocale">Locale for which IME should be loaded</param>
        /// <param name="textBoxBase">Instance of TextBoxBase control to be used for IME setup</param>
        /// <param name="window">Active window on which the instance of TextBoxBase control exists</param>
        public static void SetUpIMEKeyboardLayout(IMELocales testLocale, TextBoxBase textBoxBase, Window window)
        {
            UIElementWrapper wrapper = new UIElementWrapper(textBoxBase);
            wrapper.Clear();
            wrapper.Element.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Logger.Current.Log("Loading keyboard layout for " + testLocale.ToString());

            //get OperatingSystem
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version; 

            switch (testLocale)
            {
                case IMELocales.Korean:
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Korean);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Korean.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    DoFocusChangeOnWindow(window);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    //Check and toggle to Hangul mode
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
                    if (wrapper.Text[0] != '\u3141') //Unicode code point for korean character 'a'
                    {
                        Logger.Current.Log("Typing the Kana key to change the mode to Hangul in KoreanIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANA);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANA);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
                    break;
                case IMELocales.Japanese:
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.Japanese);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.JapaneseMsIme2002.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    DoFocusChangeOnWindow(window);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    //Check and toggle to Hiragana mode
                    KeyboardInput.TypeString("a");
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
                    if (wrapper.Text[0] != '\u3042') //Unicode code point for japanese character 'a'
                    {
                        Logger.Current.Log("Typing the Kanji key to change the mode to Hiragana in JapaneseIme");
                        KeyboardInput.PressVirtualKey(UisWin32.VK_KANJI);
                        KeyboardInput.ReleaseVirtualKey(UisWin32.VK_KANJI);
                    }
                    KeyboardInput.TypeString("{BACKSPACE}"); //clean up the content "a"
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
                    break;
                case IMELocales.ChinesePinyin:
                    //There is "Chinese simplefast" on Win8,
                    //and there are "IME Pinyin" and "IME QuanPin" between win7 and vista,
                    //and others on vista below version    
                    if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1)) 
                    {
                        EnsureCleanIMEState(IMELocales.ChinesePinyinSimpleFast);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);                        
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);
                    }
                    else
                    {
                        EnsureCleanIMEState(IMELocales.ChinesePinyin);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyin);                        
                    }
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Chinese.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    DoFocusChangeOnWindow(window);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseQuanPin:
                    //There is "Chinese simplefast" on Win8,
                    //and there are "IME Pinyin" and "IME QuanPin" between win7 and vista,
                    //and others on vista below version  
                    if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1)) 
                    {
                        EnsureCleanIMEState(IMELocales.ChinesePinyinSimpleFast);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);
                    }
                    else
                    {
                        EnsureCleanIMEState(IMELocales.ChineseQuanPin);
                        Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                        KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChineseQuanPin);
                    }
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.Chinese.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    DoFocusChangeOnWindow(window);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseNewPhonetic:
                    //Install the Ime KeyboardLayout to test                    
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChineseNewPhonetic);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.ChineseTraditional.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    DoFocusChangeOnWindow(window);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseNewChangJie:
                    //Install the Ime KeyboardLayout to test                    
                    KeyboardLayoutHelper.TryInstallLayout(KeyboardLayouts.ChineseNewChangJie);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.ChineseTraditional.Identifier);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    KeyboardInput.EnableIME((UIElement)window.Content);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);

                    DoFocusChangeOnWindow(window);
                    break;
            }
        }

        /// <summary>
        /// Minimizes and restores the window. This gives some time for IME to set up properly.
        /// Without this in Vista, IME mode doesnt properly setup
        /// </summary>
        /// <param name="window">Window to perform the focus change</param>
        public static void DoFocusChangeOnWindow(Window window)
        {            
            window.WindowState = WindowState.Minimized;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
            window.WindowState = WindowState.Normal;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(CiceroWaitTimeMs);
        }

        /// <summary>
        /// Remove  Ime KeyboardLayouts which are loaded already to ensure proper clean state.
        /// </summary>
        /// </summary>        
        public static void EnsureCleanIMEState(IMELocales testLocale)
        {
            switch (testLocale)
            {
                case IMELocales.ChinesePinyin:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChinesePinyin);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseQuanPin:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChineseQuanPin);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChinesePinyinSimpleFast:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChinesePinyinSimpleFast);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseNewChangJie:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChineseNewChangJie);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.Korean:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.Korean);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.ChineseNewPhonetic:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.ChineseNewPhonetic);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
                case IMELocales.Japanese:
                    KeyboardLayoutHelper.TryUninstallLayout(KeyboardLayouts.Japanese);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(IMEWaitTimeMs);
                    break;
            }
        }

        /// <summary>Loads test case data from an xml file.</summary>
        /// <param name='fileName'>Name of the file.</param>
        /// <returns>ArrayList containing the test case data.</returns>
        public static ArrayList LoadTestCaseDataFromXml(string fileName)
        {
            string tempFileName;
            ArrayList arrayList = new ArrayList();
            string[] testCaseData;
            int count;

            new System.Security.Permissions.FileIOPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();

            tempFileName = fileName;

            if (!File.Exists(fileName))
            {
                tempFileName = Path.Combine(Path.GetPathRoot(System.Environment.SystemDirectory), fileName);
            }

            Logger.Current.Log("Loading XML Content from file : " + tempFileName);
            XmlDocument doc = new XmlDocument();
            doc.Load(tempFileName);

            XmlNode root = doc.DocumentElement;
            XmlNodeList TestCasesList = root.SelectNodes("//TestCase");

            foreach (XmlNode testCaseList in TestCasesList)
            {
                XmlNodeList testCaseDataList = testCaseList.ChildNodes;
                count = 0;
                testCaseData = new string[2];
                foreach (XmlNode dataNode in testCaseDataList)
                {
                    testCaseData[count++] = dataNode.InnerText;
                }
                arrayList.Add(testCaseData);
            }
            return arrayList;
        }
        

        /// <summary>
        /// Time to wait for Cicero updates, in milliseconds.
        /// </summary>
        public static int CiceroWaitTimeMs = 1000;        
        
        /// <summary>
        /// Time to wait for IME to come up properly, in milliseconds.
        /// </summary>
        public static int IMEWaitTimeMs = 4000;
    }
}