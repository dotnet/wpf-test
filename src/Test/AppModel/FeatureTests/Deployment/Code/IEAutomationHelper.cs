// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MTI = Microsoft.Test.Input;
using Microsoft.Test.Loaders;
using Microsoft.Test.Diagnostics;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Win32;
using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Input;
using System.Security.Permissions;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Helper methods for Internet Explorer and UIAutomation operations
    /// </summary>
    [System.CLSCompliant(false)]
    public class IEAutomationHelper
    {
        #region DLL Imports
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        static extern IntPtr LoadMenu(IntPtr hInstance, IntPtr resourceIDHex);

        [DllImport("user32.dll")]
        static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        static extern int GetMenuStringW(IntPtr hMenu, uint uIDItem,
           [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpString, int nMaxCount, uint uFlag);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool altTab);

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process(
             [In] IntPtr hProcess,
             [Out] out bool wow64Process
             );

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int LoadString(IntPtr hInstance, int uID, StringBuilder lpBuffer, int nBufferMax);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr LoadAccelerators(IntPtr hInstance, IntPtr lpTableName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int CopyAcceleratorTable(IntPtr hAccelSrc, [Out] ACCEL[] lpAccelDst, int cAccelEntries);

        private const uint MF_BYPOSITION = 0x00000400;

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        #endregion

        #region Win32 Accelerator Helpers

        /// <summary>
        /// This method behaves just like MAKEINTRESOURCE().
        /// </summary>
        /// <param name="resourceId">
        /// The resource identifier.
        /// </param>
        /// <returns>
        /// The converted resource identifier.
        /// </returns>
        internal static IntPtr MakeIntResource(Int32 resourceId)
        {
            IntPtr debuggable = new IntPtr((UInt16)resourceId);
            return debuggable;
        }

        // The version in Microsoft.Test.Win32 is not usable.  So, here's a very simple one that works.
        // I dont want to contribute to redundancy but if I make this internal I get a warning that it will NEVER be used
        // So, suppress that warning to keep this guy internal.      
#pragma warning disable 649
        internal struct ACCEL
        {
            public byte fVirt;
            public short key;
            public ushort cmd;
        }
#pragma warning restore 649

        public static string GetIEResourceDLLPath()
        {
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    return getBrowseLCPath();
                case 7:
                    return getIEFramePath();
                default: goto case 7;
            }
        }

        public static KeyGesture GetAcceleratorFromWin32Table(string resourceDLLPath, int acceleratorTableId, int acceleratorId)
        {
            IntPtr hMod = LoadLibraryEx(resourceDLLPath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
            IntPtr acceleratorTableHandle;
            unchecked
            {
                acceleratorTableHandle = LoadAccelerators(hMod, MakeIntResource(acceleratorTableId));
            }
            if (acceleratorTableHandle == IntPtr.Zero)
            {
                // Return an empty pair... nothing to be found here
                return new KeyGesture(Key.None, ModifierKeys.None);
            }
            else
            {
                int size = CopyAcceleratorTable(acceleratorTableHandle, null, 0);
                ACCEL[] acceleratorTable = new ACCEL[size];
                CopyAcceleratorTable(acceleratorTableHandle, acceleratorTable, acceleratorTable.Length);

                for (int i = 0; i < acceleratorTable.Length; i++)
                {
                    // Looks like we found the matching resource!
                    if (acceleratorTable[i].cmd == acceleratorId)
                    {
                        Key pressedKey = Key.None;
                        ModifierKeys modifierToUse = ModifierKeys.None;
                        string displayName = "";

                        if ((acceleratorTable[i].fVirt & NativeConstants.FALT) == NativeConstants.FALT)
                        {
                            modifierToUse |= ModifierKeys.Alt;
                            displayName += "Alt + ";
                        }

                        if ((acceleratorTable[i].fVirt & NativeConstants.FCONTROL) == NativeConstants.FCONTROL)
                        {
                            modifierToUse |= ModifierKeys.Control;
                            displayName += "Ctrl + ";
                        }
                        if ((acceleratorTable[i].fVirt & NativeConstants.FSHIFT) == NativeConstants.FSHIFT)
                        {
                            modifierToUse |= ModifierKeys.Shift;
                            displayName += "Shift + ";
                        }
                        displayName += (char)acceleratorTable[i].key;
                        pressedKey = (Key)Enum.Parse(typeof(Key), ((char)acceleratorTable[i].key).ToString());
                        return new KeyGesture(pressedKey, modifierToUse, displayName);
                    }
                }
            }
            return new KeyGesture(Key.None, ModifierKeys.None);
        }

        #endregion

        #region IE Menu Helpers

        /// <summary>
        /// Loads the string from a submenu (menu below top level) given the values from TokenMapper
        /// These do not always line up! Use TokenMapper as a guide and experiment til you get the right value
        /// </summary>
        /// <param name="dllPath">path to unmanaged DLL to load menus from</param>
        /// <param name="ResourceId">Value from TokenMapper</param>
        /// <param name="MenuID">Value from TokenMapper</param>
        /// <param name="SubId">Value from TokenMapper</param>
        /// <returns>string</returns>
        public static string GetUnmanagedSubMenuResourceString(string dllPath, int ResourceId, int MenuID, uint SubId)
        {
            if (Directory.Exists(Environment.GetEnvironmentVariable("SystemRoot") + "\\SysWow64"))
            {
                dllPath = dllPath.ToLowerInvariant().Replace("system32", "syswow64");
                GlobalLog.LogDebug("Updated DLL load path to " + dllPath);
            }

            IntPtr hExe = LoadLibrary(dllPath);
            if (hExe.ToInt32() == 0)
            {
                return null;
            }

            IntPtr theMenu = LoadMenu(hExe, new IntPtr(ResourceId));

            IntPtr subMenu = GetSubMenu(theMenu, MenuID);

            StringBuilder loadedStringBuilder = new StringBuilder(260);


            if (GetMenuStringW(subMenu, SubId, loadedStringBuilder, loadedStringBuilder.Capacity, MF_BYPOSITION) > 0x00000000)
            {
                return loadedStringBuilder.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Loads the string from a top-level menu given the values from TokenMapper
        /// These do not always line up! Use TokenMapper as a guide and experiment til you get the right value
        /// </summary>
        /// <param name="dllPath">path to unmanaged DLL to load menus from</param>
        /// <param name="ResourceId">Value from TokenMapper</param>
        /// <param name="MenuID">Value from TokenMapper</param>
        /// <param name="SubId">Value from TokenMapper</param>
        /// <returns>string</returns>
        public static string GetUnmanagedTopMenuResourceString(string dllPath, int ResourceId, int MenuID, uint SubId)
        {
            if (Directory.Exists(Environment.GetEnvironmentVariable("SystemRoot") + "\\SysWow64"))
            {
                dllPath = dllPath.ToLowerInvariant().Replace("system32", "syswow64");
                GlobalLog.LogDebug("Updated load path to " + dllPath);
            }

            IntPtr hExe = LoadLibrary(dllPath);

            if (((IntPtr)hExe) == IntPtr.Zero)
            {
                // This happens when the file does not exist.  Not exceptional.
                // Return null so the caller can try a different path.
                return null;
            }

            IntPtr theMenu = LoadMenu(hExe, new IntPtr(ResourceId));

            StringBuilder loadedStringBuilder = new StringBuilder(260);


            if (GetMenuStringW(theMenu, SubId, loadedStringBuilder, loadedStringBuilder.Capacity, MF_BYPOSITION) > 0x00000000)
            {
                return loadedStringBuilder.ToString();
            }
            else
            {
                return null;
            }
        }

        private static AutomationElement ShowIE6orLaterMenu(AutomationElement IEWindow, params AndCondition[] conditions)
        {
            int conditionToUse = ApplicationDeploymentHelper.GetIEVersion() - 6;
            if (conditionToUse >= conditions.Length)
            {
                // Optimistic approach:  Try the last version of IE in the list, hoping things will be the same...
                conditionToUse = conditions.Length - 1;
            }

            AndCondition isIEMenu = conditions[conditionToUse];
            IEWindow.SetFocus();

            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            Thread.Sleep(250);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            Thread.Sleep(250);

            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    break;
                case 7:
                    {
                        bool alwaysShowMenusValue = false;

                        try
                        {
                            int value = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main", "AlwaysShowMenus", 0);
                            if (value == 1)
                            {
                                alwaysShowMenusValue = true;
                            }

                        }
                        catch // do nothing 
                        { }

                        if (!alwaysShowMenusValue)
                        {
                            AutomationElement menuBar = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "MenuBar"));
                            int countDown = 10;

                            while ((menuBar == null) && (countDown-- > 0))
                            {
                                MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
                                Thread.Sleep(250);
                                MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
                                Thread.Sleep(250);
                                menuBar = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "MenuBar"));
                            }
                        }
                    }
                    break;
                default:
                    // IE 8 and 9 work the same as 7 for now...
                    goto case 7;
            }
            AutomationElement theMenu = IEWindow.FindFirst(TreeScope.Descendants, isIEMenu);

            // WORKAROUND: Invoke() does not behave well for IE on all platforms
            // FIX: Click it instead.  May eventually not be needed, then should switch back as Invoke is more reliable when working.
            MTI.Input.MoveToAndClick(theMenu);
            //InvokePattern ip = theMenu.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            //ip.Invoke();

            Thread.Sleep(500);

            // Now we've invoked it, find the actual menu and return it, which is actually a direct child of the root element.
            AutomationElement ActualMenu = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));

            return ActualMenu;
        }

        private static string s_showIEViewMenu_String;
        /// <summary>
        /// Opens the IE View Menu and returns an AutomationElement that references it
        /// </summary>
        /// <param name="IEWindow">IE Window to start UI Automation search from</param>
        /// <returns>AutomationElement</returns>
        public static AutomationElement ShowIEViewMenu(AutomationElement IEWindow)
        {
            // Load this from WPF Assemblies.  A good idea since this test should only ever run with Avalon assemblies around!
            if (s_showIEViewMenu_String == null)
            {
                string pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + CultureInfo.CurrentUICulture.Name + ApplicationDeploymentHelper.PresentationHostDllMui;
                if (!File.Exists(pHostDllPath))
                {
                    pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + @"\en-us" + ApplicationDeploymentHelper.PresentationHostDllMui;
                }
                s_showIEViewMenu_String = GetUnmanagedTopMenuResourceString(pHostDllPath, 2000, 1, 1);
            }
            if (s_showIEViewMenu_String != null)
            {
                s_showIEViewMenu_String = s_showIEViewMenu_String.Replace("&", "");
            }
            return ShowIE6orLaterMenu(IEWindow,
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE6
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEViewMenu_String),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)),
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE7+
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEViewMenu_String),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)));
        }

        /// <summary>
        /// Opens the IE GoTo Menu and returns an AutomationElement that references it
        /// </summary>
        /// <param name="IEWindow">IE Window to start UI Automation search from</param>
        /// <returns>AutomationElement</returns>
        public static AutomationElement ShowIEGoToMenu(AutomationElement IEWindow)
        {
            return ShowIE6orLaterMenu(IEWindow,
                new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 33104"), new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 33104")),
                new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 33104"), new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 33104")));
        }

        private static string s_showIEEditMenuString;
        /// <summary>
        /// Opens the IE Edit Menu and returns an AutomationElement that references it
        /// Assumes WPF browser content running, and loads strings from WPF paths.
        /// </summary>
        /// <param name="IEWindow">IE Window to start UI Automation search from</param>
        /// <returns>AutomationElement</returns>
        public static AutomationElement ShowIEEditMenu(AutomationElement IEWindow)
        {
            // Load the next 2 from PresentationHostDll.Dll (since these are the "merged" menus)
            string pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + CultureInfo.CurrentUICulture.Name + ApplicationDeploymentHelper.PresentationHostDllMui;
            if (!File.Exists(pHostDllPath))
            {
                pHostDllPath = Microsoft.Test.Diagnostics.SystemInformation.Current.FrameworkWpfPath + @"\en-us" + ApplicationDeploymentHelper.PresentationHostDllMui;
            }
            s_showIEEditMenuString = IEAutomationHelper.GetUnmanagedTopMenuResourceString(pHostDllPath, 2000, 0, 0).Replace("&", "");

            return ShowIE6orLaterMenu(IEWindow,
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE6
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEEditMenuString),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)),
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE7+
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEEditMenuString),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)));
        }

        private static string s_showIEFileMenu_IE6String;
        private static string s_showIEFileMenu_IE7String;
        private static string s_showIEFileMenu_IE8String;
        /// <summary>
        /// Opens the IE File Menu (6 or 7) and returns an AutomationElement that references it
        /// IE8 is treated as a special case of IE7 since it continues storing resources primarily in the same assemblies.
        /// </summary>
        /// <param name="IEWindow">IE Window to start UI Automation search from</param>
        /// <returns>AutomationElement</returns>
        public static AutomationElement ShowIEFileMenu(AutomationElement IEWindow)
        {
            // IE6 String loading part...
            int currentUILCID = System.Globalization.CultureInfo.CurrentUICulture.LCID;
            string currentUILCIDHex = String.Format("{0:x4}", currentUILCID).ToUpperInvariant();
            string browseLCPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\browselc.dll";

            if ((s_showIEFileMenu_IE6String == null) && (ApplicationDeploymentHelper.GetIEVersion() == 6))
            {
                s_showIEFileMenu_IE6String = GetUnmanagedTopMenuResourceString(browseLCPath, 266, 0, 0);
            }
            if (s_showIEFileMenu_IE6String != null)
            {
                s_showIEFileMenu_IE6String = s_showIEFileMenu_IE6String.Replace("&", "");
                GlobalLog.LogDebug("Using IE File Menu string:" + s_showIEFileMenu_IE6String + " (Should match visual on browser)");
            }

            // IE7 String loading part... 
            if ((s_showIEFileMenu_IE7String == null) && (ApplicationDeploymentHelper.GetIEVersion() == 7))
            {
                s_showIEFileMenu_IE7String = IEAutomationHelper.GetUnmanagedTopMenuResourceString(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\" + System.Globalization.CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui", 33026, 0, 0);
            }
            // Second chance... try it again w/ english locale (for non-Loc IE installs)
            if ((s_showIEFileMenu_IE7String == null) && (ApplicationDeploymentHelper.GetIEVersion() == 7))
            {
                s_showIEFileMenu_IE7String = IEAutomationHelper.GetUnmanagedTopMenuResourceString(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\en-us\ieframe.dll.mui", 33026, 0, 0);
            }
            if (s_showIEFileMenu_IE7String != null)
            {
                s_showIEFileMenu_IE7String = s_showIEFileMenu_IE7String.Replace("&", "");
            }

            // use >= since IE9 uses the same values currently.
            // IE8 String loading part... 
            if ((s_showIEFileMenu_IE8String == null) && (ApplicationDeploymentHelper.GetIEVersion() >= 8))
            {
                s_showIEFileMenu_IE8String = IEAutomationHelper.GetUnmanagedTopMenuResourceString(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\" + System.Globalization.CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui", 333, 0, 0);
            }
            // Second chance... try it again w/ english locale (for non-Loc IE installs)
            if ((s_showIEFileMenu_IE8String == null) && (ApplicationDeploymentHelper.GetIEVersion() >= 8))
            {
                s_showIEFileMenu_IE8String = IEAutomationHelper.GetUnmanagedTopMenuResourceString(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\en-us\ieframe.dll.mui", 333, 0, 0);
            }
            if (s_showIEFileMenu_IE8String != null)
            {
                s_showIEFileMenu_IE8String = s_showIEFileMenu_IE8String.Replace("&", "");
            }

            return ShowIE6orLaterMenu(IEWindow,
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE6
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEFileMenu_IE6String),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)),
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE7
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEFileMenu_IE7String),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem)),
                new AndCondition( // AndCondition that will find the menu inside an IE window for IE8
                        new PropertyCondition(AutomationElement.NameProperty, s_showIEFileMenu_IE8String),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.MenuItem))
                     );
        }

        /// <summary>
        /// Click IE address bar twice to ensure the IE menu can be invoked by pressing Alt. 
        /// </summary>
        /// <param name="IEWindow"></param>
        public static void ClickIEAddrBarTwice(AutomationElement IEWindow)
        {
            GlobalLog.LogDebug("Try to click IE address bar for workaround bug 491048.");
            try
            {
                AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
                AutomationElement addressBar = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);
                IEAutomationHelper.ClickCenterOfAutomationElement(addressBar);
                IEAutomationHelper.ClickCenterOfAutomationElement(addressBar);
            }
            catch (Exception e)
            {
                GlobalLog.LogDebug("Failed to click IE address bar, this may cause IE menu can't be invoke.");
                GlobalLog.LogDebug("Exception:" + e.ToString());
            }
        }

        #endregion

        #region IE Navigation Helpers

        /// <summary>
        /// Find a Hyperlink that is a child element of parentElement by name
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        public static AutomationElement FindHTMLHyperlinkByName(AutomationElement parentElement, string name)
        {
            AndCondition isMyLink = new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, name),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink));
            return parentElement.FindFirst(TreeScope.Descendants, isMyLink);
        }


        /// <summary>
        /// Wait for a Hyperlink that is a child element of parentElement by name for secondsTimeout seconds.
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <param name="secondsTimeout"></param>
        public static AutomationElement WaitForHTMLHyperlinkByName(AutomationElement parentElement, string name, int secondsTimeout)
        {
            AutomationElement whatIFound = FindHTMLHyperlinkByName(parentElement, name);
            while ((whatIFound == null) && (secondsTimeout >= 0))
            {
                secondsTimeout--;
                Thread.Sleep(1000);
                whatIFound = FindHTMLHyperlinkByName(parentElement, name);
            }
            return whatIFound;
        }

        /// <summary>
        /// Find a Hyperlink and send mouse input to click on it
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static void MouseClickHTMLHyperlinkByName(AutomationElement parentElement, string name)
        {
            AutomationElement hyperlink = FindHTMLHyperlinkByName(parentElement, name);

            if (hyperlink != null)
            {
                ClickCenterOfAutomationElement(hyperlink);
            }
            else
            {
                throw new ArgumentNullException("Hyperlink not found");
            }
        }

        /// <summary>
        /// Clicks an HTML Hyperlink that is a child element of parentElement by name
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        public static void ClickHTMLHyperlinkByName(AutomationElement parentElement, string name)
        {
            AndCondition isMyLink = new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, name),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink));
            AutomationElement myLink = parentElement.FindFirst(TreeScope.Descendants, isMyLink);

            InvokePattern ip = myLink.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ip.Invoke();
        }

        // Keep these guys static so that the heavy lifting of figuring out their names
        // is kept out of looped actions.
        private static AndCondition s_isBackButton;
        private static AndCondition s_isFwdButton;
        /// <summary>
        /// Clicks IE back button regardless of IE version or locale
        /// </summary>
        /// <param name="ieWindow"></param>
        public static void ClickIEBackButton(AutomationElement ieWindow)
        {
            if (s_isBackButton == null)
            {
                switch (ApplicationDeploymentHelper.GetIEVersion())
                {
                    case 6:
                        if (CultureInfo.CurrentUICulture.LCID == 1025)
                        {
                            GlobalLog.LogDebug("Special Arabic Back Button Handling...");
                            AutomationElementCollection aec = ieWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                            IEnumerator num = aec.GetEnumerator();
                            num.MoveNext();
                            AutomationElement arBackBtn = (num.Current) as AutomationElement;
                            InvokePattern invP = arBackBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                            invP.Invoke();
                            return;
                        }
                        else
                        {
                            string resourceLoadPath = getBrowseLCPath();
                            IntPtr hMod = LoadLibraryEx(resourceLoadPath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                            StringBuilder sb = new StringBuilder();
                            // 58689 = IE6's string resource for back button
                            int ln = LoadString(hMod, 58689, sb, 255);
                            s_isBackButton = new AndCondition(
                                new PropertyCondition(AutomationElement.NameProperty, sb.ToString()),
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                        }
                        break;
                    case 7:  // In RTM timeframe, IE7 automation IDs appear to be relatively stable, so just use AutoIds there
                        {
                            s_isBackButton = new AndCondition(
                                new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 256"),
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                            break;
                        }
                    // IE8 and 9 are the same as 7 for now... 
                    default:
                        goto case 7;
                }
            }

            AutomationElement backBtn = WaitForElementByAndCondition(ieWindow, s_isBackButton, 30);
            try
            {
                InvokePattern ip = backBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                ip.Invoke();
            }
            catch (Exception invokeException)
            {
                GlobalLog.LogDebug("Hit Exception using Invoke (" + invokeException.ToString() + "), trying click instead");
                ClickCenterOfAutomationElement(backBtn);
            }
        }

        /// <summary>
        /// Clicks IE Forward button regardless of IE version or locale
        /// </summary>
        /// <param name="ieWindow"></param>
        public static void ClickIEFwdButton(AutomationElement ieWindow)
        {
            if (s_isFwdButton == null)
            {
                switch (ApplicationDeploymentHelper.GetIEVersion())
                {
                    case 6:
                        if (CultureInfo.CurrentUICulture.LCID == 1025)
                        {
                            GlobalLog.LogDebug("Special Arabic Back Button Handling...");
                            AutomationElementCollection aec = ieWindow.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                            IEnumerator num = aec.GetEnumerator();
                            num.MoveNext();
                            num.MoveNext();
                            AutomationElement arFwdBtn = (num.Current) as AutomationElement;
                            InvokePattern invP = arFwdBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                            invP.Invoke();
                            return;
                        }
                        else
                        {
                            string resourceLoadPath = getBrowseLCPath();
                            IntPtr hMod = LoadLibraryEx(resourceLoadPath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                            StringBuilder sb = new StringBuilder();
                            // 58690 = IE6's string resource for forward button
                            int ln = LoadString(hMod, 58690, sb, 255);
                            s_isFwdButton = new AndCondition(
                                new PropertyCondition(AutomationElement.NameProperty, sb.ToString()),
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.SplitButton));
                        }
                        break;
                    case 7:  // In RTM timeframe, IE7 automation IDs appear to be relatively stable, so just use AutoIds there
                        {
                            s_isFwdButton = new AndCondition(
                                new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 257"),
                                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
                            break;
                        }
                    default:
                        // IE8 + 9 are the same as 7 for now... 
                        goto case 7;
                }
            }
            AutomationElement fwdBtn = WaitForElementByAndCondition(ieWindow, s_isFwdButton, 30);
            try
            {
                InvokePattern ip = fwdBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                ip.Invoke();
            }
            catch (Exception invokeException)
            {
                GlobalLog.LogDebug("Hit Exception using Invoke (" + invokeException.ToString() + "), trying click instead");
                ClickCenterOfAutomationElement(fwdBtn);
            }
        }

        private static string[] s_ie6MenuList = null;
        public static string GetIE6MenuStringByIndex(int index)
        {
            try
            {
                if (s_ie6MenuList == null)
                {
                    int currentUILCID = System.Globalization.CultureInfo.CurrentUICulture.LCID;
                    string currentUILCIDHex = String.Format("{0:x4}", currentUILCID).ToUpperInvariant();
                    string resourceLoadPath = @"@%SystemRoot%\system32\browselc.dll,-";

                    // Default to English case, assume IE resources are in System32\
                    // But, if we find files in MUI folder under %ProgramFiles% for current culture, use them
                    if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\MUI\" + currentUILCIDHex + "\browselc.dll"))
                    {
                        resourceLoadPath = @"@%ProgramFiles%\Internet Explorer\MUI\" + currentUILCIDHex + @"\browselc.dll,-";
                    }
                    GlobalLog.LogDebug("Using resource path " + resourceLoadPath);
                    // 12647 = IE6 resource containing every menu's correct name separated by '|'
                    string allIE6Menus = UnmanagedStringHelper.LoadUnmanagedResourceString(resourceLoadPath + "12647");
                    GlobalLog.LogDebug("String for all IE6 Menus = " + allIE6Menus);
                    s_ie6MenuList = allIE6Menus.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                GlobalLog.LogDebug("Returning Menu String: " + s_ie6MenuList[index]);
                return s_ie6MenuList[index];

            }
            catch (Exception except)
            {
                GlobalLog.LogDebug("Hit exception loading string: " + except.ToString() + "\n");
                return null;
            }
        }

        /// <summary>
        /// Workaround: Can't get the AutomationElement of "Run" button via UIAutomation from the SecurityDialog 
        ///             that is poped up after starting an xbap with -debug/-debugsecurityzoneurl mode.
        /// Solution: Hitting the accsee key "Alt + R" of the "Run" button instead.
        /// </summary>
        /// <param name="dialogHandle">The handler of the SecurityDialog window we want to operate</param>
        public static void HandleSecDialogByAccessKey(IntPtr dialogHandle)
        {
            try
            {
                GlobalLog.LogEvidence("Swith to the window \n");

                // call SwitchToThisWindow to focus the window and bring the window to forground, 
                // make sure the access key effect on this window. 
                SwitchToThisWindow(dialogHandle, true);

                // Send the Access Key 
                MTI.Input.SendKeyboardInput(Key.Left, true);
                MTI.Input.SendKeyboardInput(Key.R, true);
                MTI.Input.SendKeyboardInput(Key.R, false);
                MTI.Input.SendKeyboardInput(Key.Left, false);
                GlobalLog.LogEvidence("Access Key Alt + R send successful \n");
            }
            catch (Exception except)
            {
                GlobalLog.LogDebug("Failed to send the access key via MTI" + except.ToString() + "\n");
            }
        }

        #endregion

        #region UIAutomation Timing Methods

        public static void TrySetFocus(AutomationElement element)
        {
            try
            {
                element.SetFocus();
            }
            catch (Exception exc)
            {
                GlobalLog.LogDebug("TrySetFocus: Failed due to " + exc.GetType());
            }
        }

        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy AndCondition ac
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ac"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByAndCondition(IntPtr hwnd, AndCondition ac, int timeout)
        {
            AutomationElement window = AutomationElement.FromHandle(hwnd);
            return WaitForElementByAndCondition(window, ac, timeout);
        }

        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy AndCondition ac
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ac"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByAndCondition(AutomationElement parentElement, AndCondition ac, int timeout)
        {
            AutomationElement element = parentElement.FindFirst(TreeScope.Descendants, ac);
            while ((element == null) && (timeout > 0))
            {
                element = parentElement.FindFirst(TreeScope.Descendants, ac);
                Thread.Sleep(1000);
                timeout--;
            }
            return element;
        }

        /// <summary>
        ///  Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="name"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithName(IntPtr hwnd, string name, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.NameProperty, name);
            return WaitForElementByPropertyCondition(hwnd, isElement, timeout);
        }
        /// <summary>
        /// Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithName(AutomationElement parentElement, string name, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.NameProperty, name);
            return WaitForElementByPropertyCondition(parentElement, isElement, timeout);
        }

        /// <summary>
        /// Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="AutoID"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithAutomationId(IntPtr hwnd, string AutoID, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.AutomationIdProperty, AutoID);
            return WaitForElementByPropertyCondition(hwnd, isElement, timeout);
        }

        /// <summary>
        /// Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="AutoID"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithAutomationId(AutomationElement parentElement, string AutoID, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.AutomationIdProperty, AutoID);
            return WaitForElementByPropertyCondition(parentElement, isElement, timeout);
        }

        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy PropertyCondition pc
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pc"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByPropertyCondition(IntPtr hwnd, PropertyCondition pc, int timeout)
        {
            AutomationElement window = AutomationElement.FromHandle(hwnd);
            return WaitForElementByPropertyCondition(window, pc, timeout);
        }


        /// <summary>
        /// Waits up to timeout seconds for a child of the given AutomationElement to satisfy PropertyCondition pc
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="pc"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByPropertyCondition(AutomationElement parentElement, PropertyCondition pc, int timeout)
        {
            if (parentElement == null)
            {
                return null;
            }

            AutomationElement element = null;

            while ((element == null) && (timeout >= 0))
            {
                try
                {
                    element = parentElement.FindFirst(TreeScope.Descendants, pc);
                }
                catch (System.InvalidOperationException)
                {
                    // Ignore this... it happens if we're navigating away and the app is being disposed.
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                    // And this too ... it happens randomly in test logs.
                }
                Thread.Sleep(1000);
                timeout--;
            }
            return element;
        }

        /// <summary>
        /// Waits up to timeout seconds to find and invoke element with automationId that is a child of parentElement
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="AutomationId"></param>
        /// <param name="timeout"></param>
        public static void InvokeElementViaAutomationId(AutomationElement parentElement, string AutomationId, int timeout)
        {
            AutomationElement el = WaitForElementWithAutomationId(parentElement, AutomationId, timeout);
            InvokePattern ip = el.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if ((ip == null) && (el != null))
            {
                throw new System.InvalidOperationException("Element with automation id \"" + AutomationId + "\" did not return an invoke pattern!");
            }
            ip.Invoke();
        }

        /// <summary>
        /// Hacky workaround for clicking Hyperlinks, which cannot be seen as control elements to UI automation.
        /// This method finds a control element with the specified Automation ID and clicks the center of its 
        /// bounding rectangle. 
        /// </summary>
        /// <param name="automationId">Automation ID of the element to click</param>
        public static void ClickCenterOfElementById(string automationId)
        {
            ClickCenterOfElementById(AutomationElement.RootElement, automationId);
        }

        /// <summary>
        /// Hacky workaround for clicking UI Elements which cannot be seen as control elements to UI automation.
        /// This method finds a control element with the specified Automation ID and clicks the center of its 
        /// bounding rectangle.  
        /// </summary>
        /// <param name="referenceElement"></param>
        /// <param name="automationId"></param>
        public static void ClickCenterOfElementById(AutomationElement referenceElement, string automationId)
        {
            AutomationElement toClick = referenceElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
            ClickCenterOfAutomationElement(toClick);
        }

        /// <summary>
        /// Hacky workaround for clicking UI Elements which cannot be seen as control elements to UI automation.
        /// This method finds a control element with the specified Automation ID and clicks the center of its 
        /// bounding rectangle.  
        /// </summary>
        /// <param name="referenceElement">Element to start search from</param>
        /// <param name="name">Automation Name property</param>
        public static void ClickCenterOfElementByName(AutomationElement referenceElement, string name, int timeout)
        {
            AutomationElement toClick = IEAutomationHelper.WaitForElementWithName(referenceElement, name, timeout);
            ClickCenterOfAutomationElement(toClick);
        }

        public static void ClickCenterOfAutomationElement(AutomationElement toClick)
        {
            if (toClick != null)
            {
                System.Windows.Rect r = (System.Windows.Rect)toClick.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                double horizMidPt = r.Left + ((r.Right - r.Left) / 2.0);
                double vertMidPt = r.Top + ((r.Bottom - r.Top) / 2.0);
                MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftDown | MTI.SendMouseInputFlags.Move);
                MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftUp | MTI.SendMouseInputFlags.Move);
            }
        }

        public static AndCondition GetIEAddressBarAndCondition()
        {
            AndCondition isAddrBar;
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 7:
                    isAddrBar = new AndCondition(
                                          new PropertyCondition(AutomationElement.IsTextPatternAvailableProperty, true),
                                          new PropertyCondition(AutomationElement.ClassNameProperty, "Edit"),
                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "41477")
                                         );
                    break;
                case 8:
                    isAddrBar = new AndCondition(
                                          new PropertyCondition(AutomationElement.IsTextPatternAvailableProperty, true),
                                          new PropertyCondition(AutomationElement.ClassNameProperty, "Edit")
                                         );
                    break;
                default:
                    goto case 8;
            }
            return isAddrBar;
        }

        #endregion

        #region Miscellaneous Helper methods
        [PermissionSet(SecurityAction.Assert, Name = "FullTrust")]
        public static string GetIEFrameStringResource(int resourceId)
        {
            string resourceLoadPath = getIEFramePath();
            GlobalLog.LogDebug("Using IEFrame path : " + resourceLoadPath);
            IntPtr hMod = LoadLibraryEx(resourceLoadPath, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
            StringBuilder sb = new StringBuilder();
            int ln = LoadString(hMod, resourceId, sb, 512);
            GlobalLog.LogDebug("Loaded String from IEFrame.dll: " + ((string)sb.ToString()));
            return sb.ToString();
        }

        private static string getIEFramePath()
        {
            if (File.Exists(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\" + System.Globalization.CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui"))
            {
                return Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\" + System.Globalization.CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui";
            }
            else
            {
                return Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\en-us\ieframe.dll.mui";
            }
        }

        private static string getBrowseLCPath()
        {
            int currentUILCID = System.Globalization.CultureInfo.CurrentUICulture.LCID;
            string currentUILCIDHex = String.Format("{0:x4}", currentUILCID).ToUpperInvariant();
            string resourceLoadPath = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\browselc.dll");

            // Default to English case, assume IE resources are in System32\
            // But, if we find files in MUI folder under %ProgramFiles% for current culture, use them
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\MUI\" + currentUILCIDHex + "\browselc.dll"))
            {
                resourceLoadPath = Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Internet Explorer\MUI\" + currentUILCIDHex + @"\browselc.dll");
            }
            GlobalLog.LogDebug("Using resource path " + resourceLoadPath);
            return resourceLoadPath;
        }

        public static bool Is64BitProcess(int processId)
        {
            bool is64Bit = false;
            Process proc = Process.GetProcessById(processId);

            if (System.Environment.OSVersion.Version.Major >= 5)
            {
                IntPtr processHandle = proc.Handle;
                bool retVal;
                if (!IsWow64Process(processHandle, out retVal))
                {
                    throw new Exception("Uncontinue-able Failure attempting to call IsWow64Process");
                }
                is64Bit = !retVal;
            }
            return is64Bit;
        }

        public static bool Is64BitExecutable(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Could not find file:" + file);
            }

            using (FileStream fs = File.OpenRead(file))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int PE_OFFSET = 0x3C;

                    if (fs.Length < PE_OFFSET + 4 || reader.ReadByte() != 'M' || reader.ReadByte() != 'Z')
                    {
                        throw new InvalidOperationException(file + " is not a valid executable.");
                    }

                    fs.Position = PE_OFFSET;
                    fs.Position = reader.ReadInt32();
                    if (reader.ReadByte() != 'P' || reader.ReadByte() != 'E' || reader.ReadByte() != 0 || reader.ReadByte() != 0)
                    {
                        throw new InvalidOperationException("No PE header found.");
                    }
                    const UInt16 IMAGE_FILE_MACHINE_I386 = 0x14C;
                    const UInt16 IMAGE_FILE_MACHINE_AMD64 = 0x8664;

                    string arch = null;
                    switch (reader.ReadUInt16())
                    {
                        case IMAGE_FILE_MACHINE_I386:
                            arch = "x86";
                            break;
                        case IMAGE_FILE_MACHINE_AMD64:
                            arch = "x64";
                            break;
                    }
                    GlobalLog.LogDebug("Detected architecture of " + file + ": " + (arch ?? "unknown"));
                    if (arch == "x64")
                    {
                        return true;
                    }
                    if (arch == "x86")
                    {
                        return false;
                    }
                    throw new InvalidOperationException("No idea what architecture type " + file + "is!");
                }
            }
        }

        public static string PresentationHostExePath
        {
            get
            {
                // Since the test build is multi-targeted, use the registry to find which is currently registered 
                string presHostLocation = Registry.GetValue(@"HKEY_CLASSES_ROOT\CLSID\{ADBE6DEC-9B04-4A3D-A09C-4BB38EF1351C}\ServerExecutable", null, "fail").ToString();

                // But fall back to the framework independent version if we fail (Correct for 4.0+)
                if (presHostLocation == "fail")
                {
                    presHostLocation = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\WPF\presentationhost.exe";
                }
                presHostLocation = presHostLocation.Replace("\"", "");
                return presHostLocation;
            }
        }

        #endregion
    }

    /// <summary>
    /// Temporarily puts HTTP internet URL for FileHost-hosted content in the IE Trusted Sites zone.
    /// </summary>
    public class AddToTrustedSites : LoaderStep
    {
        // Can easily modify this later to add other paths to trusted sites
        // but the Internet makes most sense because it is the most restricted zone to elevate
        // and I dont feel like implementing a mapping between FileHost scheme and the zone enumeration.       

        public override bool DoStep()
        {
            // Have to remove Internet zone mapping of this URL to move it.  This will get restored next time AppMonitor runs.
            ApplicationDeploymentHelper.RemoveUrlFromZone(IEUrlZone.URLZONE_INTERNET, FileHost.HttpInternetBaseUrl);
            ApplicationDeploymentHelper.AddUrlToZone(IEUrlZone.URLZONE_TRUSTED, FileHost.HttpInternetBaseUrl);
            //ApplicationDeploymentHelper.AddUrlToZone(IEUrlZone.URLZONE_TRUSTED, fileHostUri);
            GlobalLog.LogEvidence("Succeeded...");
            return true;
        }
    }

    /// <summary>
    /// Undoes HTTP internet URL being mapped to IE Trusted Sites zone.
    /// </summary>
    public class RemoveFromTrustedSites : LoaderStep
    {
        public override bool DoStep()
        {
            GlobalLog.LogEvidence("Removing " + FileHost.HttpInternetBaseUrl + " from Trusted Sites zone.");
            ApplicationDeploymentHelper.RemoveUrlFromZone(IEUrlZone.URLZONE_TRUSTED, FileHost.HttpInternetBaseUrl);
            // This shouldnt matter because this happens when every ApplicationMonitor instance starts, but try to put things back to normal
            ApplicationDeploymentHelper.AddUrlToZone(IEUrlZone.URLZONE_INTERNET, FileHost.HttpInternetBaseUrl);
            GlobalLog.LogEvidence("Succeeded...");
            return true;
        }

    }

}
