// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace DRT
{
    public class DrtThemesSuite : DrtTestSuite
    {
        public DrtThemesSuite() : base("Themes")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            Border border = new Border();
            border.SetResourceReference(Border.BackgroundProperty, SystemColors.WindowBrushKey);
            DRT.Show(border);

            DockPanel dockPanel = new DockPanel();
            border.Child = dockPanel;

            s_title = new TextBlock();
            s_title.Text = "Testing themes...";
            DockPanel.SetDock(s_title, Dock.Top);
            dockPanel.Children.Add(s_title);

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            DockPanel.SetDock(stackPanel, Dock.Top);
            dockPanel.Children.Add(stackPanel);
            s_test = stackPanel;

            Button button = new Button();
            button.Content = "Button";
            stackPanel.Children.Add(button);

            CheckBox checkBox = new CheckBox();
            checkBox.Content = "CheckBox";
            stackPanel.Children.Add(checkBox);

            RadioButton radioButton = new RadioButton();
            radioButton.Content = "RadioButton";
            stackPanel.Children.Add(radioButton);

            ListBox listBox = new ListBox();
            listBox.Items.Add("ListBoxItem 1");
            listBox.Items.Add("ListBoxItem 2");
            stackPanel.Children.Add(listBox);

            ComboBox comboBox = new ComboBox();
            comboBox.Items.Add("ComboBoxItem 1");
            comboBox.Items.Add("ComboBoxItem 2");
            stackPanel.Children.Add(comboBox);

            ScrollBar scrollBar = new ScrollBar();
            scrollBar.Orientation = Orientation.Horizontal;
            scrollBar.Width = 100.0;
            scrollBar.Height = 16.0;
            stackPanel.Children.Add(scrollBar);


            // Tests

            if (!DRT.KeepAlive)
            {
                return new DrtTest[] {
                    new DrtTest(Start),
                    new DrtTest(TestDictionary),
                    new DrtTest(ThemedTest),
                    new DrtTest(ChangeThemeTest),
                    new DrtTest(Cleanup),
                };
            }
            else
            {
                return new DrtTest[] {};
            }
        }


        private void Start()
        {
#if WCP_SYSTEM_THEMES_ENABLED
            DRT.Assert(NativeMethods.IsThemeActive() != 0, "Themes are not active. You must enable the default theme in Longhorn.");
#endif // WCP_SYSTEM_THEMES_ENABLED
        }

        public void Cleanup()
        {
        }

        private void TestDictionary()
        {
            ResourceDictionary d = new ResourceDictionary();

            d.Add("key1", "value1");
            d.Add("key2", "value2");
            d.Add("key3", "value3");

            DictionaryEntry[] a = new DictionaryEntry[d.Count];
            d.CopyTo(a, 0);

            foreach (DictionaryEntry e in a)
            {
                TestEntry(e);
            }

            foreach (DictionaryEntry e in d)
            {
                TestEntry(e);
            }

            // Ensure enums work separately from int values
            d.Add(TestEnum.IndexZero, "IndexZero");
            d.Add(TestEnum.IndexOne, "IndexOne");
            d.Add(TestEnum.IndexTwo, "IndexTwo");
            DRT.Assert(d[(object)0] == null, "Enum value recognized as an int (incorrect behavior)");
        }

        private enum TestEnum
        {
            IndexZero = 0,
            IndexOne = 1,
            IndexTwo = 2
        }

        private void TestEntry(DictionaryEntry e)
        {
            string key = e.Key as string;
            DRT.Assert(key != null, "Non-string key found");

            switch (key)
            {
                case "key1":
                    DRT.Assert((string)e.Value == "value1", "Mismatched key/value pair 1");
                    break;

                case "key2":
                    DRT.Assert((string)e.Value == "value2", "Mismatched key/value pair 2");
                    break;

                case "key3":
                    DRT.Assert((string)e.Value == "value3", "Mismatched key/value pair 3");
                    break;

                default:
                    DRT.Assert(false, "Unknown key found");
                    break;
            }
        }

        private void ThemedTest()
        {
            // System values

            string fontFamily = SystemFonts.MenuFontFamily.Source;
            s_valueCache = fontFamily;
            object testCompare = SystemFonts.MenuFontFamily.Source;
            DRT.Assert((fontFamily != null) && (fontFamily != String.Empty), "Querying MenuFontFamily failed");
            DRT.Assert(Object.ReferenceEquals(testCompare, fontFamily), "System parameters were not cached");

            double width = SystemParameters.HorizontalScrollBarButtonWidth;
            double compareWidth = (double)s_test.FindResource(SystemParameters.HorizontalScrollBarButtonWidthKey);
            DRT.Assert(width != 0.0, "Querying HorizontalScrollBarButtonWidth failed");
            DRT.Assert(width == compareWidth, "HorizontalScrollBarButtonWidth and resource lookup did not match");

            object o = s_test.FindResource(SystemColors.WindowBrushKey);
            DRT.Assert((o != null) && (o is Brush), "Window color brush resource not found");
            DRT.Assert(SystemColors.WindowBrush == (Brush)o, "Window color brush resource incorrect");

            o = s_test.FindResource(SystemFonts.MessageFontFamilyKey);
            DRT.Assert((o != null) && (o is FontFamily) && (SystemFonts.MessageFontFamily.Source == ((FontFamily)o).Source), "Message font family resource not found");


#if WCP_SYSTEM_THEMES_ENABLED
            // UxTheme API

            o = _test.FindResource(new UxThemeResourceKey("BUTTON", 1, 1, 3001));
            DRT.Assert((o != null) && (o is BitmapImage), "UxTheme resource reference BitmapImage not found");
            DRT.Assert(((BitmapImage)o).IsFrozen, "UxTheme resource reference BitmapImage is mutable, it may have context affinity, not allowed for system resources");

            object refCompare = _test.FindResource(new UxThemeResourceKey("BUTTON", 1, 1, 3001));
            DRT.Assert((o != null) && (o is BitmapImage) && (refCompare != null) && (refCompare is BitmapImage), "UxTheme resource reference BitmapImage not found (2nd time)");
            DRT.Assert(Object.ReferenceEquals(o, refCompare), "UxTheme resource reference caching not working");
#endif
        }

        private void ChangeThemeTest()
        {
            IntPtr hwnd = NativeMethods.FindWindow(null, "SystemResourceNotifyWindow");
            DRT.Assert(hwnd != IntPtr.Zero, "Could not find SystemResourceNotifyWindow");
            NativeMethods.PostMessage(hwnd, NativeMethods.WM_THEMECHANGED, IntPtr.Zero, IntPtr.Zero);
            DRT.ResumeAt(new DrtTest(VerifyThemeChangeTest));
        }

        private void VerifyThemeChangeTest()
        {
            string fontFamily = SystemFonts.MenuFontFamily.Source;
            DRT.Assert((fontFamily != null) && (fontFamily != String.Empty), "Querying MenuFontFamily failed (after theme change)");
            DRT.Assert(!Object.ReferenceEquals(s_valueCache, fontFamily), "System parameters were not re-evaluated after theme change");
        }

        [SuppressUnmanagedCodeSecurity]
        private sealed class NativeMethods
        {
            internal const int WM_THEMECHANGED = 0x031A;

            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
            internal static extern int IsThemeActive();

            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            internal static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImport("user32.dll", CharSet=CharSet.Auto)]
            internal static extern IntPtr FindWindow(string className, string windowName);
        }

#if false
        private static void SwitchTheme(string themeFile, string scheme, string fontSize)
        {
            Thread.Sleep(1000);

            Process process = new Process();
            process.StartInfo.FileName = "regsvr32.exe";
            process.StartInfo.Arguments = String.Format("/s /n /i:/InstallVS:\"'{0}','{1}','{2}'\" themeui.dll", themeFile, scheme, fontSize);
            process.Start();

            process.WaitForExit();
        }

        private static string MakeThemeFilePath(string themeName)
        {
            return String.Format("{0}\\Resources\\Themes\\{1}\\{1}.msstyles", Environment.GetEnvironmentVariable("SystemRoot"), themeName);
        }

        private const string ThemeManagerRegKey = "Software\\Microsoft\\Windows\\CurrentVersion\\ThemeManager";
        private const string ThemePropertyDllName = "DllName";
        private const string ThemePropertyFontSize = "SizeName";
        private const string ThemePropertyColor = "ColorName";

        [RegistryPermissionAttribute(System.Security.Permissions.SecurityAction.Assert, Read = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\ThemeManager")]
        [EnvironmentPermissionAttribute(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        private static void StoreThemeInformation()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(ThemeManagerRegKey);

            if (key != null)
            {
                _oldTheme = key.GetValue(ThemePropertyDllName) as string;
                _oldFont = key.GetValue(ThemePropertyFontSize) as string;
                _oldColor = key.GetValue(ThemePropertyColor) as string;
            }

            if (_oldTheme == null)
            {
                _oldTheme = String.Empty;
            }

            if (_oldFont == null)
            {
                _oldFont = String.Empty;
            }

            if (_oldColor == null)
            {
                _oldColor = String.Empty;
            }
        }


        private const string AppearanceRegKey = "Control Panel\\Appearance";
        private const string AppearanceName = "Current";

        [RegistryPermissionAttribute(System.Security.Permissions.SecurityAction.Assert, Read = "HKEY_CURRENT_USER\\Control Panel\\Appearance")]
        [EnvironmentPermissionAttribute(System.Security.Permissions.SecurityAction.Assert, Unrestricted = true)]
        private static void StoreColorName()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(AppearanceRegKey);

            if (key != null)
            {
                _oldColor = key.GetValue(AppearanceName) as string;
            }

            if (_oldColor == null)
            {
                _oldColor = String.Empty;
            }
        }

        private static void SaveTheme()
        {
            Console.WriteLine("Saving current theme...");

            StoreThemeInformation();

            if (_oldTheme == String.Empty)
            {
                StoreColorName();

                if (_oldColor.StartsWith("High Contrast"))
                {
                    int paren1 = _oldColor.IndexOf('(');
                    int paren2 = _oldColor.IndexOf(')');
                    if ((paren1 > 0) && (paren2 > paren1))
                    {
                        _oldFont = _oldColor.Substring(paren1 + 1, paren2 - paren1 - 1);
                        _oldColor = _oldColor.Substring(0, paren1).Trim();
                    }
                }

                if (_oldFont == null)
                {
                    _oldFont = "Normal";
                }
            }
        }

        private static void RestoreTheme(UIContextOperationCallback returnPoint)
        {
            Console.WriteLine("Restoring original theme...");

            // Do this twice to overcome switching 
            _desiredThemeFile = _oldTheme;
            _desiredScheme = _oldColor;
            _desiredFontSize = _oldFont;
            GoTheme(returnPoint);
        }

        private static void GoClassic(UIContextOperationCallback returnPoint)
        {
            Console.WriteLine("Switching Theme to Classic...");

            _desiredScheme = "Windows Standard";
            _desiredFontSize = "Normal";
            GoTheme(returnPoint);
        }

        private static void GoLuna(UIContextOperationCallback returnPoint)
        {
            Console.WriteLine("Switching Theme to Luna...");

            _desiredThemeFile = MakeThemeFilePath("Luna");
            GoTheme(returnPoint);
        }

        private static void GoSlate(UIContextOperationCallback returnPoint)
        {
            Console.WriteLine("Switching Theme to Slate...");

            _desiredThemeFile = MakeThemeFilePath("Slate");
            GoTheme(returnPoint);
        }

        private static void GoJade(UIContextOperationCallback returnPoint)
        {
            Console.WriteLine("Switching Theme to Jade...");

            _desiredThemeFile = MakeThemeFilePath("Jade");
            GoTheme(returnPoint);
        }

        private static void GoTheme(UIContextOperationCallback returnPoint)
        {
            _returnPoint = returnPoint;
            Thread thread = new Thread(new ThreadStart(PerformThemeChange));
            thread.Start();
        }

        private static void PerformThemeChange()
        {
            // Do twice to overcome 
            SwitchTheme(_desiredThemeFile, _desiredScheme, _desiredFontSize);
            SwitchTheme(_desiredThemeFile, _desiredScheme, _desiredFontSize);

            _desiredThemeFile = String.Empty;
            _desiredScheme = String.Empty;
            _desiredFontSize = String.Empty;

            _context.BeginInvoke(_returnPoint, null, UIContextPriority.Background);
            _returnPoint = null;
        }

        private static string _oldTheme = String.Empty;
        private static string _oldColor = String.Empty;
        private static string _oldFont = String.Empty;
        private static UIContextOperationCallback _returnPoint = null;
        private static string _desiredThemeFile = String.Empty;
        private static string _desiredScheme = String.Empty;
        private static string _desiredFontSize = String.Empty;
#endif

        private static string s_valueCache;
        private static TextBlock s_title;
        private static FrameworkElement s_test;
    }
}


