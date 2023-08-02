// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: Library of utilities used by TextPattern for UIVerify
//*          and other TextPattern related tests
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
#define CODE_ANALYSIS  // Required for FxCop suppression attributes

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;  // Required for FxCop suppression attributes
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Automation.Text;

using ATGTestInput;
using Microsoft.Test.WindowsUIAutomation.Core;
using Microsoft.Test.WindowsUIAutomation.Logging;
using Microsoft.Test.WindowsUIAutomation.TestManager;
using MS.Win32;

namespace InternalHelper
{

    /// ------------------------------------------------------------------------------------------
    /// <summary>Utilities used by TextPattern testing both within UIVerify and without</summary>
    /// ------------------------------------------------------------------------------------------
    public static class TextLibrary 
    {
        #region MemberVariables

        internal static string typeOfProvider;                  // Default provider

        /// <summary></summary>
        public const string RichEditClassName = "RICHEDIT";     // ClassName for Win32 RichEdit Class Names

        /// <summary></summary>
        public const string EditClassName = "EDIT";             // ClassName for Win32 Edit Class Names

        /// <summary></summary>
        public const string OctetName = "OCTET";                // Used for IPControl's LocalizedControlType property

        /// <summary></summary>
        public const int MAXTIME = 20000;                       // Used for starting process

        /// <summary></summary>
        public const int TIMEWAIT = 100;                        // Used for starting process

        #endregion

        #region AutomationElementFromCustomId
        // -------------------------------------------------------------------
        // Finds an automation element from a given Automation ID
        // -------------------------------------------------------------------
        static public AutomationElement AutomationElementFromCustomId(AutomationElement element, object identifier, bool verbose)
        {
            Library.ValidateArgumentNonNull(element, "Automation Element");
            Library.ValidateArgumentNonNull(identifier, "Automation ID");

            if (verbose == true)
                Logger.LogComment("Looking for control (" + identifier + ")");

            AutomationElement ae;
            ae = element.FindFirst(TreeScope.Children | TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, identifier));
            if (ae == null)
            {
                throw new ArgumentException("Could not identify the element based on the AutomationIdProperty");
            }

            return ae;
        }
        #endregion AutomationElementFromCustomId

        #region ClassName and LocalizedControlType properties

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets ClassName and LocalizedControlType properties
        /// </summary>
        ///---------------------------------------------------------------------------
        public static void GetClassName(AutomationElement autoElement, out string className, out string localizedControlType)
        {
            TextLibraryCount.GetClassName( autoElement, out className, out localizedControlType );
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets ClassName property
        /// </summary>
        ///---------------------------------------------------------------------------
        public static string GetClassName(AutomationElement autoElement)
        {
            return TextLibraryCount.GetClassName(autoElement);
        }

        #endregion ClassName and LocalizedControlType properties

        #region CountTextUnits

        /// -------------------------------------------------------------------
        /// Known reliable way to count # of text units for Format/Word units
        /// This is independent of the provider being used
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static int CountTextUnit(TextUnit tu, TextPatternRange rangeToCount)
        {
            return TextLibraryCount.CountTextUnit(tu, rangeToCount);
        }

        /// -------------------------------------------------------------------
        /// Count # of lines within a TextPatternRange (win32 specific)
        /// -------------------------------------------------------------------
        public static int Win32CountLines(AutomationElement element)
        {
            return TextLibraryCount.Win32CountLines(element);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Count # of LF within a TextPatternRange (provide agnostic... for now)
        /// </summary>
        /// -------------------------------------------------------------------
        public static int CountParagraphs(string actualText)
        {
            return TextLibraryCount.CountParagraphs( actualText );
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Count a single TextUnit
        /// </summary>
        /// -------------------------------------------------------------------
        internal static int Win32CountTextUnit(TextUnit textUnit, TextPatternRange rangeToCount)
        {
            return Win32CountTextUnit(textUnit, rangeToCount);
        }

        #endregion CountTextUnits

        #region IdentifySupportedTextUnits

        /// -------------------------------------------------------------------
        /// <summary>
        /// Identifies supported text units. Some units will map upwards
        /// TextUnit.Page, for example, maps up to a Document for all 
        /// controls/providers except Word/Office
        /// </summary>
        /// -------------------------------------------------------------------
        public static void IdentifySupportedTextUnits(AutomationElement element, ref TextUnit[] supportedTextUnits)
        {
            TextLibraryCount.IdentifySupportedTextUnits(element, ref supportedTextUnits);
        }

        //-------------------------------------------------------------------
        // Identify supported TextUnits in Win32
        //-------------------------------------------------------------------
        internal static TextUnit win32_IdentifySupportedTextUnits(AutomationElement element, TextUnit targetUnit)
        {
            return TextLibraryCount.win32_IdentifySupportedTextUnits(element, targetUnit);
        }

        #endregion IdentifySupportedTextUnits

        #region IsMultiLine

        /// -------------------------------------------------------------------
        /// <summary>
        /// Determine if control supports multiple lines of text
        ///  Win32: Use GetWindowLong to get the style
        ///  Avalon:  TBD
        ///  Trident: TBD
        ///  Office:  TBD
        ///  DUI:     TBD
        ///  MSAA:    TBD
        /// 
        /// WHY RETURN Resutls instead of Bool? Because depending on the
        /// provider, our results may be indeterminate or not applicable
        /// 
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        public static bool IsMultiLine(AutomationElement element)
        {
            switch (typeOfProvider)
            {
                case "win32":
                case "winform":
                    return Win32IsStyle(element, SafeNativeMethods.ES_MULTILINE);
                case "wpf": // 
                    InternalHelper.Tests.TestObject.Comment("****** IsMultiLine() is not supported for WPF/Avalon/Windows Presentation Foundation. Assuming false");
                    return false;       
                default:
                    throw new ArgumentException("IsMultiLine() has no support for " + typeOfProvider);
            }
        }

        ///-------------------------------------------------------------------
        /// <summary>
        /// Determine if WIN32 control supports multiple lines of text
        /// </summary>
        ///-------------------------------------------------------------------
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        internal static bool Win32IsStyle(AutomationElement element, int style)
        {
            int _style;
            IntPtr hWndPtr;
            NativeMethods.HWND hWnd;

            // Determine _style
            hWndPtr = Helpers.CastNativeWindowHandleToIntPtr(element);
            hWnd = NativeMethods.HWND.Cast(hWndPtr);

            _style = SafeNativeMethods.GetWindowLong(hWnd, SafeNativeMethods.GWL_STYLE);

            // OACR requirement
            if (_style == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            // Validate against expected results, then set comment text
            if ((_style & style) > 0)
                return true;
            else
                return false;
        }

        #endregion IsMultiLine

        #region Keybopard Support

        /// -------------------------------------------------------------------
        /// <summary>
        /// Sends a single key to application that currently has focus
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static void SendOneKey(System.Windows.Input.Key key, string msg, int sleepTime)
        {
            Thread.Sleep(sleepTime);

            Library.ValidateArgumentNonNull(msg, "msg argument cannot be null");

            if (msg.Length > 0)
                msg = msg + " by ";
            Logger.LogComment(msg + "Sending key " + key.ToString());
            ATGTestInput.Input.SendKeyboardInput(key, true);
            ATGTestInput.Input.SendKeyboardInput(key, false);
        }


        /// -------------------------------------------------------------------
        /// <summary>
        /// Sends a two key sequence to application that currently has focus
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static void SendTwoKeys(System.Windows.Input.Key key1, System.Windows.Input.Key key2, string msg, int sleepTime)
        {
            Thread.Sleep(sleepTime);

            Library.ValidateArgumentNonNull(msg, "msg argument cannot be null");

            if (msg.Length > 0)
                msg = msg + " by ";
            Logger.LogComment(msg + "Sending keys " + key1.ToString() + ", " + key2.ToString());

            ATGTestInput.Input.SendKeyboardInput(key1, true);
            Thread.Sleep(300);
            ATGTestInput.Input.SendKeyboardInput(key2, true);
            Thread.Sleep(sleepTime);
            ATGTestInput.Input.SendKeyboardInput(key2, false);
            ATGTestInput.Input.SendKeyboardInput(key1, false);
            Thread.Sleep(sleepTime);
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Sends a three key sequence to application that currently has focus
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static void SendThreeKeys(System.Windows.Input.Key key1, System.Windows.Input.Key key2, System.Windows.Input.Key key3, string msg, int sleepTime)
        {
            Thread.Sleep(sleepTime);

            Library.ValidateArgumentNonNull(msg, "SendSingleKey() msg argument is null");

            if (msg.Length > 0)
                msg = msg + " by ";
            Logger.LogComment(msg + "Sending keys " + key1.ToString() + ", " + key2.ToString() + " + " + key3.ToString());

            ATGTestInput.Input.SendKeyboardInput(key1, true);
            Thread.Sleep(300);
            ATGTestInput.Input.SendKeyboardInput(key2, true);
            Thread.Sleep(300);
            ATGTestInput.Input.SendKeyboardInput(key3, true);
            Thread.Sleep(sleepTime);
            ATGTestInput.Input.SendKeyboardInput(key3, false);
            ATGTestInput.Input.SendKeyboardInput(key1, false);
            ATGTestInput.Input.SendKeyboardInput(key2, false);
            Thread.Sleep(sleepTime);
        }

        #endregion Keyboard Support

        #region RichEdit Madness

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// Identify if we're on RichEdit control or not
        /// </summary>
        /// ---------------------------------------------------------------------------
        public static bool IsRichEdit(AutomationElement autoElement)
        {
            return TextLibraryCount.IsRichEdit(autoElement);
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Trims trailing CRLF from a range for a richedit control
        /// </summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static int CountTrailingCRLF(AutomationElement autoElement, TextPatternRange callingRange)
        {
            // 
            int offset = 0;

            Library.ValidateArgumentNonNull(autoElement, "AutomationElement cannot be null");
            Library.ValidateArgumentNonNull(callingRange, "callingRange cannot be null");

            if (IsRichEdit(autoElement) == true)
            {
                string text = callingRange.GetText(-1);

                offset = GetTrailingCRLFOffset(text);
            }
            return offset;
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Trims trailing CRLF from a range for a richedit control
        /// </summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String)")]
        public static void TrimRangeCRLF(AutomationElement autoElement, TextPatternRange callingRange)
        {
            Library.ValidateArgumentNonNull(autoElement, "AutomationElement cannot be null");
            Library.ValidateArgumentNonNull(autoElement, "callingRange cannot be null");

            if (IsRichEdit(autoElement) == true)
            {
                int actualOffset = 0; // Actual   offset of \r, \n or \r\n
                int expectedOffset = 0; // Expected offset of \r, \n or \r\n
                string text = callingRange.GetText(-1);

                expectedOffset = GetTrailingCRLFOffset(text);

                // Now... move the endpoint
                actualOffset = callingRange.MoveEndpointByUnit(TextPatternRangeEndpoint.End, TextUnit.Character, expectedOffset);

                if (actualOffset != expectedOffset)
                    throw new InvalidOperationException("Unable to move endpoint back by " + expectedOffset + " characters. Actual = " + actualOffset);
                else
                    Logger.LogComment("Adjusted size of range for RichEdit control by omitting last " + expectedOffset + " characters");
            }
        }

        // ---------------------------------------------------------------------------
        // Determines the size of trialing \r, \n or \r\n
        // ---------------------------------------------------------------------------
        static private int GetTrailingCRLFOffset(string text)
        {
            int idx = 0;
            int offset = 0;
            int textLength = 0;

            if ((text == null) || (text.Length == 0))
                return 0;

            textLength = text.Length;

            //---------------------------------------------------
            // Case 1: Trailing \r\n, newer versions of RichEdit
            //---------------------------------------------------
            idx = text.LastIndexOf("\r\n");
            // What if idx = -1? Don't want to trim anyway so idx must be >= 0
            if ((idx >= 0) && (idx == (textLength - 2)))
            {
                offset = -2;
            }
            else
            {
                // ---------------------------------------------------
                // Case 2: Trailing \r, older versions of RichEdit
                // ---------------------------------------------------
                idx = text.LastIndexOf('\r');
                if ((idx >= 0) && (idx == (text.Length - 1)))
                {
                    offset = -1;
                }
                else
                {
                    // ---------------------------------------------------
                    // Case 3: Trailing \n, older versions of RichEdit
                    // ---------------------------------------------------
                    idx = text.LastIndexOf('\n');
                    if ((idx >= 0) && (idx == (text.Length - 1)))
                    {
                        offset = -1;
                    }
                }
            }

            return offset;
        }

        #endregion RichEdit Madness

        #region SetProvider

        //---------------------------------------------------------------------------
        // Set the type of provider to be used
        //---------------------------------------------------------------------------
        internal static void SetProvider(string _typeOfProvider)
        {
            typeOfProvider = _typeOfProvider.ToLower(CultureInfo.InvariantCulture);
        }

        #endregion SetProvider

        #region SupportedTextSelection

        /// -------------------------------------------------------------------
        /// <summary>
        /// Does underlying application / provider support text selection ?
        ///  - Win32: Yes. Always
        ///  - Avalon: ??? (likely yes, always)
        ///  - Trident: Yes, no, maybe. See TextPattern spec for variatances.
        ///  - Office: TBD
        /// </summary>
        /// -------------------------------------------------------------------
        public static SupportedTextSelection ProviderSupportedTextSelection()
        {
            switch (typeOfProvider)
            {
                case "win32":
                case "winform":
                case "wpf":
                    return SupportedTextSelection.Single;
                default:
                    throw new ArgumentException("SupportsTextSelection() has no support for " + typeOfProvider);
            }
        }

        #endregion SupportedTextSelection

    }
}
