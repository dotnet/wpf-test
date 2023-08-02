// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//******************************************************************* 
//* Purpose: Library of utilities used by TextPattern for UIVerify
//*          and other TextPattern related tests
//*          Note that this library can be used by other tests other
//*          than UIVerify
//* Owner: Microsoft
//* Contributors:
//******************************************************************* 
#define CODE_ANALYSIS  // Required for FxCop suppression attributes

// This suppresses warnings #'s not recognized by the compiler.
#pragma warning disable 1634, 1691

using System;
using System.Diagnostics.CodeAnalysis;  // Required for FxCop suppression attributes
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Automation.Text;

using MS.Win32;

namespace InternalHelper
{

    /// ------------------------------------------------------------------------------------------
    /// <summary>Utilities used by TextPattern testing both within UIVerify and without</summary>
    /// ------------------------------------------------------------------------------------------
    public static class TextLibraryCount
    {

        #region MemberVariables

        internal static string typeOfProvider = "";             // Default provider

        /// <summary></summary>
        public const string RichEditClassName = "RICHEDIT";     // ClassName for Win32 RichEdit Class Names

        /// <summary></summary>
        public const string EditClassName = "EDIT";             // ClassName for Win32 Edit Class Names

        /// <summary></summary>
        public const string OctetName = "OCTET";                // Used for IPControl's LocalizedControlType property

        #endregion MemberVariables


        #region ClassName and LocalizedControlType properties

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets ClassName and LocalizedControlType properties
        /// </summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#")]
        public static void GetClassName(AutomationElement autoElement, out string className, out string localizedControlType)
        {
            ValidateArgumentNonNull(autoElement, "AutomationElement argument cannot be null");  // Sanity check

            className = ""; // Initialize
            localizedControlType = "";

            // Get ClassName (edit, richedit and various flavors of each)
            className = GetClassName(autoElement);

            // get LocalizedControlType (needed for IPControl, etc.)
            localizedControlType = (string)autoElement.GetCurrentPropertyValue(AutomationElement.LocalizedControlTypeProperty);
            if (localizedControlType != null)
                localizedControlType = localizedControlType.ToUpperInvariant(); // Culture Invariant value for property
            else
                throw new InvalidOperationException("LocalizedControlType Property cannot return null");
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets ClassName property
        /// </summary>
        ///---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static string GetClassName(AutomationElement autoElement)
        {
            ValidateArgumentNonNull(autoElement, "AutomationElement argument cannot be null");  // Sanity check

            string className = ""; // Initialize

            // Get ClassName (edit, richedit and various flavors of each)
            className = (string)autoElement.GetCurrentPropertyValue(AutomationElement.ClassNameProperty);
            if (className != null)
                className = className.ToUpperInvariant(); // Culture Invariant value for property
            else
                throw new InvalidOperationException("ClassNameProperty cannot return null");

            return className;
        }

        #endregion ClassName and LocalizedControlType properties

        #region CountTextUnits

        //---------------------------------------------------------------------------
        // Count text units in a given range
        //---------------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        public static void CountTextUnitsInRange(TextPatternRange rangeToCount, TextUnit[] supportedTextUnits, ref int[] numberOfTextUnits)
        {
            // Validate arrays
            if (supportedTextUnits.Length != (((int)TextUnit.Document) + 1))
                throw new ArgumentException("supportedTextUnits array is of incorrect length");

            if (numberOfTextUnits.Length != (((int)TextUnit.Document) + 1))
                throw new ArgumentException("numberOfTextUnits array is of incorrect length");

            // Determine supported text units
            numberOfTextUnits[(int)TextUnit.Character] = TextLibraryCount.CountTextUnit(TextUnit.Character, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Format]    = TextLibraryCount.CountTextUnit(TextUnit.Format, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Word]      = TextLibraryCount.CountTextUnit(TextUnit.Word, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Line]      = TextLibraryCount.CountTextUnit(TextUnit.Line, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Paragraph] = TextLibraryCount.CountTextUnit(TextUnit.Paragraph, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Page]      = TextLibraryCount.CountTextUnit(TextUnit.Page, rangeToCount);
            numberOfTextUnits[(int)TextUnit.Document]  = TextLibraryCount.CountTextUnit(TextUnit.Document, rangeToCount);
        }


        /// -------------------------------------------------------------------
        /// Known reliable way to count # of text units for Format/Word units
        /// This is independent of the provider being used
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static int CountTextUnit(TextUnit tu, TextPatternRange rangeToCount)
        {
            ValidateArgumentNonNull(rangeToCount, "rangeToCount argument cannot be null");  // Sanity check

            TextPatternRange clone = rangeToCount.Clone();

            int count = clone.MoveEndpointByUnit(TextPatternRangeEndpoint.Start, tu, Int32.MaxValue);
        
            // Provider / Text Unit specific offsets    
            switch(typeOfProvider)
            {
            case "win32":
            case "winform":
                {
                    // This is to correct the fact that our line count will skip the last line if it 
                    // DOES NOT have a trailing \r, \n or \r\n
                    if( tu == TextUnit.Line ) 
                    {
                        string text = rangeToCount.GetText(-1);
                        AutomationElement element = rangeToCount.GetEnclosingElement();
                        if( IsTrailingCRLF(element, text) == 0 )
                            count++;
                    }
                }
                break;
            case "wpf":
                break;
            } 
            
            return count;
        }

        /// -------------------------------------------------------------------
        /// Count # of lines within a TextPatternRange (win32 specific)
        /// -------------------------------------------------------------------
        public static int Win32CountLines(AutomationElement element)
        {
            IntPtr hWndPtr = CastNativeWindowHandleToIntPtr(element);
            NativeMethods.HWND hWnd = NativeMethods.HWND.Cast(hWndPtr);

            IntPtr lineCount1 = UnsafeNativeMethods.SendMessage(hWnd, UnsafeNativeMethods.EM_GETLINECOUNT, IntPtr.Zero, IntPtr.Zero);

            return (int)lineCount1.ToInt32();
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Count # of LF within a TextPatternRange (provide agnostic... for now)
        /// 
        /// This gets tricky. Per UIA, a paragraph is text followed by N instances
        /// of \r, \n, or \r\n until we run out of text, or we run into the first
        /// character that isn't \r\n.  If we start our text range with a 
        /// sequence of \r, \n, or \r\n, then this sequence counts as a paragraph
        /// as well even though it has no text preceeding it.
        /// 
        /// Where:
        /// [*.]    equals 1 or more instances of any character except \r or \n
        /// [\r\n]+ equals 1 or more instnaces of either \r or \n
        ///
        /// Return Value  If the input string is (using faux regular expressions):
        /// ------------  ------------------------------------------------------
        ///       0       Null or empty string
        ///       1       [\r\n]+                or [*.][\r\n]+
        ///       2       [\r\n]+[*.]            or [\r\n]+[*.][\r\n]+            or [*.][\r\n]+[*.]            or [*.][\r\n]+[*.][\r\n]+
        ///       3       [\r\n]+[*.][\r\n]+[*.] or [\r\n]+[*.][\r\n]+[*.][\r\n]+ or [*.][\r\n]+[*.][\r\n]+[*.] or [*.][\r\n]+[*.][\r\n]+[*.][\r\n]+
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String)")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        public static int CountParagraphs(string actualText)
        {
            bool lastIsPara = false; // State: Was last char either \r or \n
            int count = 1;     // Count of paragraphs

            ValidateArgumentNonNull(actualText, "actualText cannot be null");

            // NOTE: If we change the state, i.e. value of lastIsPara, then
            //       we need to jump to the next character immediatly, hence
            //       the "continues" even in places it may not be strictly
            //       necessary (it does convey intent, tho! :-)

            // we got nothing? return nothing!
            if ((actualText == null) || (string.Compare(actualText, "") == 0))
                return 0;

            for (int idx = 0; idx < actualText.Length; idx++)
            {
                char c = actualText[idx];

                // Is current character a CR or LF???
                if ((c == '\r') || (c == '\n'))
                {
                    // Is the previous character a CR or LF?
                    if (lastIsPara == true) // a sequence of CR/LF ???
                    {
                        continue;  // "State" is unchanged, jump to next char anyway
                    }
                    else
                    {
                        lastIsPara = true;  // Flip state, but don't incr. count
                        continue;  // state is reset, jump to next char...
                    }

                }
                else
                {
                    if (lastIsPara == true) // New paragraph of text!
                    {
                        count++;    // increment counter, start of new paragraph
                        lastIsPara = false; // Flip state
                        continue;  // state is reset, jump to next char...
                    }
                }
            }

            return count;
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Count a single TextUnit
        /// </summary>
        /// -------------------------------------------------------------------
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String,System.Exception)")]
        internal static int Win32CountTextUnit(TextUnit textUnit, TextPatternRange rangeToCount)
        {
            AutomationElement element = null;
            string actualText = "";

            ValidateArgumentNonNull(rangeToCount, "rangeToCount argument cannot be null");  // Sanity check

            // Get text
            try
            {
                actualText = rangeToCount.GetText(-1);
                element = rangeToCount.GetEnclosingElement();
                if (element == null)
                    throw new InvalidOperationException("Null automation element incorrectly returned by TextPatternRange.GetEnclosingElement() in Win32CountTextUnit()");
            }
            catch (Exception exception)
            {
                if (IsCriticalException(exception))
                    throw;

                throw new InvalidOperationException("Unable to call TextPatternRange.GetText() in Win32CountTextUnit()", exception);
            }

            // Count text units and assign to array element
            switch (textUnit)
            {
                case TextUnit.Character:
                    return actualText.Length;
                case TextUnit.Format:
                    return CountTextUnit(TextUnit.Format, rangeToCount);
                case TextUnit.Word:
                    return CountTextUnit(TextUnit.Word, rangeToCount);
                case TextUnit.Line:
                    return Win32CountLines(element);
                case TextUnit.Paragraph:
                    return CountParagraphs(actualText);
                case TextUnit.Page:
                    return 1;
                case TextUnit.Document:
                    return 1;
                default:
                    throw new ArgumentException("CountTextUnits() does not support " + textUnit.ToString());
            }
        }

        #endregion CountTextUnits

        #region IdentifySupportedTextUnits

        /// -------------------------------------------------------------------
        /// <summary>
        /// Identifies supported text units. Some units will map upwards
        /// TextUnit.Page, for example, maps up to a Document for all 
        /// controls/providers except Word/Office
        ///  Win32: By control
        ///  Avalon: TBD
        ///  Trident: TBD
        ///  Office: TBD
        /// </summary>
        /// -------------------------------------------------------------------
        public static void IdentifySupportedTextUnits(AutomationElement element, ref TextUnit[] supportedTextUnits)
        {
            // Validate input argument
            if (supportedTextUnits.Length != (((int)TextUnit.Document) + 1))
                throw new ArgumentException("supportedTextUnits array is of incorrect length");

            if (element == null)
                throw new ArgumentException("element cannot be null");

            switch (typeOfProvider)
            {
                case "win32":
                case "winform":
                    supportedTextUnits[(int)TextUnit.Character] = win32_IdentifySupportedTextUnits(element, TextUnit.Character);
                    supportedTextUnits[(int)TextUnit.Format]    = win32_IdentifySupportedTextUnits(element, TextUnit.Format);
                    supportedTextUnits[(int)TextUnit.Word]      = win32_IdentifySupportedTextUnits(element, TextUnit.Word);
                    supportedTextUnits[(int)TextUnit.Line]      = win32_IdentifySupportedTextUnits(element, TextUnit.Line);
                    supportedTextUnits[(int)TextUnit.Paragraph] = win32_IdentifySupportedTextUnits(element, TextUnit.Paragraph);
                    supportedTextUnits[(int)TextUnit.Page]      = win32_IdentifySupportedTextUnits(element, TextUnit.Page);
                    supportedTextUnits[(int)TextUnit.Document]  = win32_IdentifySupportedTextUnits(element, TextUnit.Document);
                    break;
                case "wpf":
                    supportedTextUnits[(int)TextUnit.Character] = wpf_IdentifySupportedTextUnits(element, TextUnit.Character);
                    supportedTextUnits[(int)TextUnit.Format]    = wpf_IdentifySupportedTextUnits(element, TextUnit.Format);
                    supportedTextUnits[(int)TextUnit.Word]      = wpf_IdentifySupportedTextUnits(element, TextUnit.Word);
                    supportedTextUnits[(int)TextUnit.Line]      = wpf_IdentifySupportedTextUnits(element, TextUnit.Line);
                    supportedTextUnits[(int)TextUnit.Paragraph] = wpf_IdentifySupportedTextUnits(element, TextUnit.Paragraph);
                    supportedTextUnits[(int)TextUnit.Page]      = wpf_IdentifySupportedTextUnits(element, TextUnit.Page);
                    supportedTextUnits[(int)TextUnit.Document]  = wpf_IdentifySupportedTextUnits(element, TextUnit.Document);
                    break;
                default:
                    throw new ArgumentException("IdentifySupportedTextUnits() has no support for " + typeOfProvider);
            }
        }

        //-------------------------------------------------------------------
        // Identify supported TextUnits in Win32
        //-------------------------------------------------------------------
        internal static TextUnit win32_IdentifySupportedTextUnits(AutomationElement element, TextUnit targetUnit)
        {
            string className = GetClassName(element);

            if (className.IndexOf(RichEditClassName) > -1)
            {
                switch (targetUnit)
                {
                    case TextUnit.Character: return TextUnit.Character;
                    case TextUnit.Format: return TextUnit.Format;
                    case TextUnit.Word: return TextUnit.Word;
                    case TextUnit.Line: return TextUnit.Line;
                    case TextUnit.Paragraph: return TextUnit.Paragraph;
                    case TextUnit.Page:                                 // Maps to Document for everything but MS Word
                    case TextUnit.Document: return TextUnit.Document;
                    default:
                        throw new NotImplementedException("IdentifySupportedTextUnits() does not support " + targetUnit.ToString());
                }
            }
            else if (className.IndexOf(EditClassName) > -1)
            {
                switch (targetUnit)
                {
                    case TextUnit.Character: return TextUnit.Character;
                    case TextUnit.Word: return TextUnit.Word;
                    case TextUnit.Line: return TextUnit.Line;
                    case TextUnit.Paragraph: return TextUnit.Paragraph;
                    case TextUnit.Format:                               // No support for Format in Win32 Edit
                    case TextUnit.Page:                                 // Maps to Document for everything but MS Word
                    case TextUnit.Document: return TextUnit.Document;
                    default:
                        throw new NotImplementedException("IdentifySupportedTextUnits() does not support " + targetUnit.ToString());
                }
            }

            throw new NotImplementedException("IdentifySupportedTextUnits() cannot determine supported text units for class " + className);
        }


        //-------------------------------------------------------------------
        // Identify supported TextUnits in WPF
        //-------------------------------------------------------------------
        internal static TextUnit wpf_IdentifySupportedTextUnits(AutomationElement element, TextUnit targetUnit)
        {
            switch (targetUnit)
            {
                case TextUnit.Character: return TextUnit.Character;
                case TextUnit.Format: return TextUnit.Format;
                case TextUnit.Word: return TextUnit.Word;
                case TextUnit.Line: return TextUnit.Line;
                case TextUnit.Paragraph: return TextUnit.Paragraph;
                case TextUnit.Page:                                 // Maps to Document for everything but MS Word
                case TextUnit.Document: return TextUnit.Document;
                default:
                    throw new NotImplementedException("IdentifySupportedTextUnits() does not support " + targetUnit.ToString());
            }
        }

        #endregion IdentifySupportedTextUnits

        #region SetProvider

        //---------------------------------------------------------------------------
        // Set the type of provider to be used
        //---------------------------------------------------------------------------
        internal static void SetProvider(string _typeOfProvider)
        {
            typeOfProvider = _typeOfProvider.ToLower(CultureInfo.InvariantCulture);
        }

        #endregion SetProvider
        
        #region Helpers

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Trims trailing CRLF from a string
        /// </summary>


        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#")]
        public static uint IsTrailingCRLF(AutomationElement autoElement, string text)
        {
            if (string.IsNullOrEmpty(text) == true)
                return 0;

            int idx = 0;

            //---------------------------------------------------
            // Case 1: Trailing \r\n, newer versions of RichEdit
            //---------------------------------------------------
            idx = text.LastIndexOf("\r\n");
            // What if idx = -1? Don't want to trim anyway so idx must be >= 0
            if ((idx >= 0) && (idx == (text.Length - 2)))
            {
                return 2;
            }

            //---------------------------------------------------
            // Case 2: Trailing \r, older versions of RichEdit
            //---------------------------------------------------
            idx = text.LastIndexOf('\r');
            // What if idx = -1? Don't want to trim anyway so idx must be >= 0
            if ((idx >= 0) && (idx == (text.Length - 1)))
            {
                return 1;
            }

            //---------------------------------------------------
            // Case 3: Trailing \r, older versions of RichEdit
            //---------------------------------------------------
            idx = text.LastIndexOf('\n');
            // What if idx = -1? Don't want to trim anyway so idx must be >= 0
            if ((idx >= 0) && (idx == (text.Length - 1)))
            {
                return 1;
            }
            
            return 0;
        }

        /// ---------------------------------------------------------------------------
        /// <summary>
        /// Identify if we're on RichEdit control or not
        /// </summary>
        /// ---------------------------------------------------------------------------
        public static bool IsRichEdit(AutomationElement autoElement)
        {
            string className = "";

            ValidateArgumentNonNull(autoElement, "AutomationElement cannot be null");

            className = GetClassName(autoElement);

            if (className.LastIndexOf(RichEditClassName) > -1)
                return true;
            else
                return false;
        }
        
        /// -------------------------------------------------------------------
        /// Check that specified argument is non-null, if so, throw exception
        /// -------------------------------------------------------------------
        static public void ValidateArgumentNonNull(object obj, string argName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argName + "== null");
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>
        /// Checks for critical nonrecoverable errors
        /// </summary>
        /// -------------------------------------------------------------------
        static public bool IsCriticalException(Exception exception)
        {
            return exception is SEHException || exception is NullReferenceException || exception is StackOverflowException || exception is OutOfMemoryException || exception is System.Threading.ThreadAbortException;
        }

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets WindowHandle from an AutomationElement
        /// </summary>
        ///---------------------------------------------------------------------------
        public static IntPtr CastNativeWindowHandleToIntPtr(AutomationElement element)
        {
            ValidateArgumentNonNull(element, "Automation Element cannot be null");

            object objHwnd = element.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
            IntPtr ptr = IntPtr.Zero;

            if (objHwnd is IntPtr)
                ptr = (IntPtr)objHwnd;
            else
                ptr = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException("Could not get the handle to the element(window)");

            return ptr;
        }

        #endregion
    }
}
