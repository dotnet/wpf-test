// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

//  Provides wrappers for the Uniscribe library

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Wrappers/UniscribeWrappers.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    #endregion Namespaces.

    /// <summary>
    /// Provides support for Uniscribe, one of several ways to process 
    /// complex scripts.
    /// </summary>
    /// <remarks>
    /// Currently all Uniscribe APIs are private, and .NET-friendly
    /// entry points are exposed publicly.
    /// </remarks>
    [SuppressUnmanagedCodeSecurity]
    public static class Uniscribe
    {

        #region Public methods.

        /// <summary>
        /// Creates a list of valid caret positions in the specified string.
        /// </summary>
        /// <param name="text">String of text to analyze.</param>
        /// <param name="info">Culture info for the string.</param>
        /// <param name="direction">FlowDirection for the string.</param>
        /// <returns>A list of characters before which there are valid caret positions.</returns>
        public static List<int> ListValidCaretPositions(string text, 
            System.Globalization.CultureInfo info, System.Windows.FlowDirection direction)
        {
            bool isRightToLeft; // Whether the text is flowing right-to-left.
            UInt16 langId;      // LANGID for the ambiguous values.

            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // Get values from FlowDirection and CultureInfo.
            isRightToLeft = (direction == System.Windows.FlowDirection.RightToLeft);
            langId = (ushort)info.LCID;

            return ListValidCaretPositions(text, langId, isRightToLeft);
        }

        /// <summary>
        /// Creates a list of valid caret positions in the specified string.
        /// </summary>
        /// <param name="text">String of text to analyze.</param>
        /// <param name="langId">Language ID for the string.</param>
        /// <param name="isRightToLeft">Whether the language ID is right-to-left.</param>
        /// <returns>A list of characters before which there are valid caret positions.</returns>
        public static List<int> ListValidCaretPositions(string text, UInt16 langId, bool isRightToLeft)
        {
            List<int> result;               // List of valid caret positions.
            SCRIPT_CONTROL scriptControl;   // Script control flags (how to itemize).
            SCRIPT_STATE scriptState;       // Script state flag (environment information).

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            // Create the list in which we will store valid caret positions.
            result = new List<int>(text.Length);

            // Consider empty text as a special case.
            if (text.Length == 0)
            {
                result.Add(0);
                return result;
            }

            // Initialize control and state flags.
            scriptControl = new SCRIPT_CONTROL();
            scriptControl.uDefaultLanguage = langId;

            scriptState = new SCRIPT_STATE();
            if (isRightToLeft)
            {
                scriptState.uBidiLevel = 1;
            }

            // Process text in runs no longer than 200 characters.
            // This works around Uniscribe Regression_Bug1.
            int nextRunIndex;

            nextRunIndex = 0;
            while (nextRunIndex < text.Length)
            {
                nextRunIndex = ListRunCaretPositions(text, nextRunIndex,
                    scriptControl, scriptState, result);
            }

            // Uniscribe will return \r\n pairs as having a caret position
            // within. Let's whack those out.
            RemoveCrLfCaretPositions(text, result);

            // The last position in a string of text is also a valid caret position.
            result.Add(text.Length);

            return result;
        }

        #endregion Public methods.


        #region Private methods.

        #region Helper methods.

        /// <summary>
        /// Lists caret positions for a short run within text.
        /// </summary>
        private static int ListRunCaretPositions(string text, int runIndex,
            SCRIPT_CONTROL scriptControl, SCRIPT_STATE scriptState, List<int> result)
        {
            SCRIPT_ITEM[] items;    // Shapeable items.
            int itemCount;          // Number of shapeable items.
            int hr;                 // HRESULT from calls.
            int runLength;          // Length of run.
            int processLength;      // Length of processed run.
            string runText;         // Text of run.

            System.Diagnostics.Trace.Assert(text != null);
            System.Diagnostics.Trace.Assert(result != null);

            // We will pass in no more than 200 characters, and even then,
            // we will process no more than 190 (to avoid problems with
            // combining characters around the edges).
            runLength = text.Length - runIndex;
            if (runLength > 200)
            {
                runLength = 200;
                processLength = 190;
            }
            else
            {
                processLength = runLength;
            }

            runText = text.Substring(runIndex, runLength);

            // Create a list of individual shapeable items. Initialize
            // the properties to describe how text starts out.
            items = new SCRIPT_ITEM[runText.Length + 1];
            hr = ScriptItemize(runText, runText.Length,
                items.Length, ref scriptControl, ref scriptState, items,
                out itemCount);
            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            // For every item, find out where the stops are in its characters.
            for (int i = 0; i < itemCount; i++)
            {
                int itemLength;                 // Length of item in characters.
                SCRIPT_LOGATTR[] attributes;    // Attributes for each character.

                itemLength = items[i + 1].iCharPos - items[i].iCharPos;
                attributes = new SCRIPT_LOGATTR[itemLength];
                hr = ScriptBreak(runText.Substring(items[i].iCharPos, itemLength),
                    itemLength, ref items[i].a, attributes);
                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }

                // Add the characters marked with fCharStop into the results.
                for (int j = 0; j < itemLength; j++)
                {
                    if (attributes[j].fCharStop)
                    {
                        int textIndex;

                        textIndex = runIndex + items[i].iCharPos + j;

                        // Don't process the items that 'overflow' the number
                        // of characters we intend to process.
                        if (textIndex >= runIndex + processLength)
                        {
                            return runIndex + processLength;
                        }
                        result.Add(textIndex);
                    }
                }
            }

            return runIndex + processLength;
        }

        /// <summary>
        /// Removes \r\n pairs found in <paramref name="text"/> 
        /// from <paramref name="positions"/>.
        /// </summary>
        private static void RemoveCrLfCaretPositions(string text, List<int> positions)
        {
            int findIndex;

            findIndex = 0;
            do
            {
                findIndex = text.IndexOf("\r\n", findIndex);
                if (findIndex != -1)
                {
                    positions.Remove(findIndex + 1);
                    findIndex += 2;
                }
            } while (findIndex != -1);
        }

        #endregion Helper methods.

        #region Uniscribe entry points.

        /// <summary>
        /// The ScriptBreak function returns information for determining line breaks.
        /// </summary>
        /// <param name="pwcChars">Unicode characters to be processed.</param>
        /// <param name="cChars">Number of Unicode characters to be processed.</param>
        /// <param name="psa">Pointer to the SCRIPT_ANALYSIS structure obtained from an earlier call to the ScriptItemize function.</param>
        /// <param name="psla">Pointer to a buffer that receives the character attributes as a SCRIPT_LOGATTR structure.</param>
        /// <returns>If the function succeeds, the return value is zero; an HRESULT otherwise.</returns>
        /// <remarks>
        /// The ScriptBreak function returns cursor movement and formatting
        /// break positions for an item in an array of SCRIPT_LOGATTR
        /// structures. To support mixed formatting within a single word
        /// correctly, ScriptBreak should be passed whole items as returned
        /// by ScriptItemize and not the finer formatting runs.
        ///
        /// ScriptBreak does not require an hdc and does not perform shaping.
        ///
        /// The SCRIPT_LOGATTR structure, pointed to by psla, identifies
        /// valid caret positions and line breaks. The SCRIPT_LOGATTR.fCharStop
        /// flag marks cluster boundaries for those scripts where it is
        /// conventional to restrict from moving inside clusters. The
        /// same boundaries could also be inferred by inspecting the
        /// pwLogCLust array returned by ScriptShape, however ScriptBreak is
        /// considerably faster in implementation and does not require an hdc
        /// to be prepared. The fWordStop, fSoftBreak, and fWhiteSpace flags
        /// in SCRIPT_LOGATTR are only available through ScriptBreak.
        ///
        /// Most shaping engines that identify invalid sequences do so
        /// by setting the fInvalid flag in ScriptBreak. The fInvalidLogAttr
        /// flag in SCRIPT_PROPERTIES identifies which scripts do this.
        /// </remarks>
        [DllImport("usp10.dll")]
        private static extern int ScriptBreak(string pwcChars, int cChars,
            ref SCRIPT_ANALYSIS psa, [Out]SCRIPT_LOGATTR[] psla);

        /// <summary>
        /// The ScriptFreeCache function frees a SCRIPT_CACHE item.
        /// </summary>
        /// <param name="psc">Pointer to a SCRIPT_CACHE item to be freed.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        /// <remarks>
        /// The client may free a SCRIPT_CACHE at any time. Uniscribe maintains
        /// reference counts in its font and shaper caches and frees font data
        /// only when all sizes of the font are free, and shaper data only when
        /// all fonts it supports are freed.
        ///
        /// The client should free the SCRIPT_CACHE for a style when it discards
        /// that style.
        ///
        /// ScriptFreeCache always sets its parameter to NULL to help avoid
        /// misreferencing.
        /// </remarks>
        [DllImport("usp10.dll")]
        private static extern int ScriptFreeCache(IntPtr psc);

        /// <summary>
        /// The ScriptGetProperties function returns information about the 
        /// current scripts.
        /// </summary>
        /// <param name="ppSp">Receives a pointer to an array of pointers to SCRIPT_PROPERTIES structures indexed by script.</param>
        /// <param name="piNumScripts">Receives the number of scripts. The valid range for this value is zero through NumScripts-1.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("usp10.dll")]
        private static unsafe extern int ScriptGetProperties(out SCRIPT_PROPERTIES** ppSp,
            out int piNumScripts);

        /// <summary>
        /// The ScriptItemize function breaks a Unicode string into individually shapeable items.
        /// </summary>
        /// <param name="pwcInChars">Pointer to a Unicode string to be itemized.</param>
        /// <param name="cInChars">Number of characters in pwcInChars to be itemized.</param>
        /// <param name="cMaxItems">Maximum number of SCRIPT_ITEM structures to process.</param>
        /// <param name="psControl">
        /// Pointer to a SCRIPT_CONTROL structure containing flags indicating
        /// the type of itemization to be performed. Use NULL if this is not
        /// needed.
        /// </param>
        /// <param name="psState">
        /// Pointer to a SCRIPT_STATE structure indicating the initial
        /// bidirectional algorithm state. Use NULL if this is not needed.
        /// </param>
        /// <param name="pItems">
        /// Pointer to a buffer to receive the SCRIPT_ITEM structures
        /// processed. The buffer pointed to by pItems should be
        /// cMaxItems * sizeof(SCRIPT_ITEM) bytes in length.
        /// </param>
        /// <param name="pcItems">
        /// Pointer to a variable to receive the number of SCRIPT_ITEM
        /// structures processed.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is zero.
        ///
        /// If the function fails, it returns a nonzero value. The function
        /// returns E_INVALIDARG if pwcInChars is NULL or cInChars is 0 or
        /// pItems is NULL or cMaxItems less than 2.
        ///
        /// The function returns E_OUTOFMEMORY if the output buffer length
        /// (cMaxItems) is insufficient. Note that in this case, as in all
        /// error cases, no items have been fully processeds no part of the
        /// output array contains defined values.
        ///
        /// If any other unrecoverable error is encountered, it is returned
        /// as an HRESULT.
        /// </returns>
        [DllImport("usp10.dll")]
        private static extern int ScriptItemize(string pwcInChars,
            int cInChars, int cMaxItems, ref SCRIPT_CONTROL psControl,
            ref SCRIPT_STATE psState, [In, Out] SCRIPT_ITEM[] pItems, out int pcItems);

        #endregion Uniscribe entry points.

        #endregion Private methods.


        #region Private constants.

        /// <summary>
        /// This is the only public script ordinal. May be
        /// forced into the eScript field of a SCRIPT_ANALYSIS to disable shaping.
        /// SCRIPT_UNDEFINED is supported by all fonts - ScriptShape will display
        /// whatever glyph is defined in the font CMAP table, or, if none, the
        /// missing glyph.
        /// </summary>
        private const UInt16 SCRIPT_UNDEFINED = 0;

        #endregion Private constants.


        #region Inner types.

        /// <summary>
        /// The SCRIPT_ANALYSIS structure describes an item, that is, a portion
        /// of a Unicode string. This structure is filled by ScriptItemize,
        /// which breaks a Unicode string into individually shapeable items.
        /// The structure also includes a copy of the Unicode algorithm
        /// state (SCRIPT_STATE).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_ANALYSIS
        {
            /// <summary>Bitmap flags on script analysis item.</summary>
            public UInt16 flags;

            /// <summary>A SCRIPT_STATE structure.</summary>
            public SCRIPT_STATE s;

            /// <summary>Shaping engine.</summary>
            /// <remarks>
            /// Opaque value identifying which engine Uniscribe will use
            /// when calling the ScriptShape, ScriptPlace, and ScriptTextOut
            /// functions for this item. The value of eScript is undefined
            /// and will change in future releases, but attributes of eScript
            /// may be obtained by calling ScriptGetProperties.
            /// To disable shaping, set this parameter to SCRIPT_UNDEFINED.
            ///
            /// This is stored in the first 10 bits of flags.
            /// </remarks>
            public UInt16 eFlags { get { return (ushort)(flags & 0x3FF); } }

            /// <summary>
            /// Rendering direction.
            /// </summary>
            /// <remarks>
            /// Normally identical to the parity of the Unicode embedding level,
            /// but it may differ if overridden by GetCharacterPlacement
            /// legacy support.
            ///
            /// This is stored in the 11th bit of flags.
            /// </remarks>
            public bool fRTL { get { return (flags & 0x400) != 0; } }

            /// <summary>
            /// Set for GCP classes ARABIC/HEBREW and LOCALNUMBER.
            /// </summary>
            /// <remarks>
            /// Logical direction, whether left-to-right or right-to-left.
            /// Although this is usually the same as fRTL, for a number in a
            /// RTL run, fRTL is False (because digits are always displayed LTR),
            /// but fLayoutRTL is True (because the number is read as part
            /// of the RTL sequence).
            ///
            /// This is stored in the 12th bit of flags.
            /// </remarks>
            public bool fLayoutRTL { get { return (flags & 0x800) != 0; } }

            /// <summary>
            /// Implies there was a ZWJ before this item.
            /// </summary>
            /// <remarks>
            /// If set, the shaping engine will shape the first character of
            /// this item as if it were joining with a previous character.
            /// Set by ScriptItemize, it may be overridden before calling
            /// ScriptShape.
            ///
            /// This is stored in the 13th bit of flags.
            /// </remarks>
            public bool fLinkBefore { get { return (flags & 0x1000) != 0; } }

            /// <summary>
            /// Implies there is a ZWJ following this item.
            /// </summary>
            /// <remarks>
            /// If set, the shaping engine will shape the last character of this
            /// item as if it were joining with a subsequent character. Set by
            /// ScriptItemize, it may be overridden before calling ScriptShape.
            ///
            /// This is stored in the 14th bit of flags.
            /// </remarks>
            public bool fLinkAfter { get { return (flags & 0x2000) != 0; } }

            /// <summary>
            /// Implies there was a ZWJ before this item.
            /// </summary>
            /// <remarks>
            /// If set, the shaping engine will generate all glyph-related arrays
            /// in logical order. By default, glyph-related arrays are in visual
            /// order, the first array entry corresponding to the leftmost glyph.
            /// Set to FALSE by ScriptItemize, it may be overridden before
            /// calling ScriptShape.
            ///
            /// This is stored in the 15th bit of flags.
            /// </remarks>
            public bool fLogicalOrder { get { return (flags & 0x4000) != 0; } }

            /// <summary>
            /// Implies there was a ZWJ before this item.
            /// </summary>
            /// <remarks>
            /// Typically FALSE. Set to TRUE for bitmap, vector, and device fonts;
            /// and for Asian scripts. It may be set to TRUE on input to
            /// ScriptShape to disable use of glyphs for this item.
            /// Additionally, ScriptShape will set it to TRUE for an hdc
            /// containing symbolic, unrecognized, and device fonts.
            ///
            /// Disabling glyphing disables complex script shaping. When set,
            /// shaping and placing for this item is implemented directly by
            /// calls to GetTextExtentExPoint and ExtTextOut.
            ///
            /// This is stored in the 16th bit of flags.
            /// </remarks>
            public bool fNoGlyphIndex { get { return (flags & 0x8000) != 0; } }
        }

        /// <summary>
        /// The SCRIPT_ITEM structure includes a SCRIPT_ANALYSIS with the
        /// string offset of the first character of the item.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_ITEM
        {
            /// <summary>
            /// Specifies the offset from the beginning of the itemized
            /// string to the first character of this item, counted in
            /// Unicode code points.
            /// </summary>
            public int iCharPos;

            /// <summary>
            /// Specifies a SCRIPT_ANALYSIS structure containing analysis
            /// specific to this item.
            /// </summary>
            public SCRIPT_ANALYSIS a;
        }

        /// <summary>
        /// The SCRIPT_LOGATTR structure describes attributes of logical
        /// characters that are useful when editing and formatting text.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_LOGATTR
        {
            /// <summary>Bitmap flags on item.</summary>
            public Byte flags;

            /// <summary>Potential linebreak point.</summary>
            /// <remarks>
            /// It is valid to break the line in front of this character.
            /// This member is set on the first character of Southeast Asian words.
            /// </remarks>
            public bool fSoftBreak { get { return (flags & 0x1) != 0; } }

            /// <summary>A unicode whitespace character, except NBSP, ZWNBSP.</summary>
            /// <remarks>
            /// This character is one of the many Unicode characters that are
            /// classified as breakable white space. Breakable white space can
            /// break wordsthat is, it is all white space except NBSP (nonbreaking
            /// space) and ZWNBSP (zero-width nonbreaking space).
            /// </remarks>
            public bool fWhiteSpace { get { return (flags & 0x2) != 0; } }

            /// <summary>
            /// Valid cursor position (for left/right arrow).
            /// </summary>
            /// <remarks>
            /// Valid caret position. Set on most characters, but not on code
            /// points inside Indian and Southeast Asian character clusters. May
            /// be used to implement LEFT ARROW and RIGHT ARROW operations in editors.
            /// </remarks>
            public bool fCharStop { get { return (flags & 0x4) != 0; } }

            /// <summary>
            /// Valid cursor position (for ctrl + left/right arrow).
            /// </summary>
            /// <remarks>
            /// Valid caret position. It is the correct place to show the caret
            /// when you use a word movement keyboard action such as CTRL+LEFT
            /// ARROW and CTRL+RIGHT ARROW. May be used to implement the
            /// CTRL+LEFT ARROW and CTRL+RIGHT ARROW operations in editors.
            /// </remarks>
            public bool fWordStop { get { return (flags & 0x8) != 0; } }

            /// <summary>Invalid character sequence.</summary>
            /// <remarks>
            /// Marks characters which form an invalid or undisplayable
            /// combination. Scripts which can set this flag have the flag
            /// fInvalidLogAttr set in their SCRIPT_PROPERTIES structure.
            /// </remarks>
            public bool fInvalid { get { return (flags & 0x10) != 0; } }
        }

        /// <summary>
        /// The SCRIPT_STATE structure is used both to initialize the Unicode
        /// algorithm state as an input parameter to ScriptItemize, and is
        /// also a component of the analysis returned by ScriptItemize.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_STATE
        {
            /// <summary>Bitmap flags on item.</summary>
            public UInt16 flags;

            /// <summary>Unicode Bidi algorithm embedding level (0-16).</summary>
            /// <remarks>
            /// The embedding level associated with all characters in this run
            /// according to the Unicode bidirectional algorithm. When passed to
            /// ScriptItemize, should be initialized to zero for an LTR base
            /// embedding level, or 1 for RTL.
            ///
            /// This is stored in the first 5 bits of flags.
            /// </remarks>
            public Byte uBidiLevel
            {
                get { return (Byte)(flags & 0x1F); }
                set { flags |= (Byte)(value & 0x1F); }
            }

            /// <summary>Set when in LRO/RLO embedding.</summary>
            /// <remarks>
            /// TRUE if this level is an override level (LRO/RLO). In
            /// an override level, characters are laid out in one direction
            /// only, either left-to-right or right-to-left. No reordering
            /// of digits or strong characters of opposing direction takes
            /// place. Note that this initial value is reset by LRE, RLE,
            /// LRO or RLO codes in the string.
            ///
            /// This is stored in the 6th bit of flags.
            /// </remarks>
            public bool fOverrideDirection { get { return (flags & 0x20) != 0; } }

            /// <summary>Set by U+206A (ISS), cleared by U+206B (ASS).</summary>
            /// <remarks>
            /// TRUE if the shaping engine is to bypass mirroring of Unicode
            /// mirrored glyphs such as brackets. Set by Unicode character
            /// ISS, cleared by ASS.
            ///
            /// This is stored in the 7th bit of flags.
            /// </remarks>
            public bool fInhibitSymSwap { get { return (flags & 0x40) != 0; } }

            /// <summary>Set by U+206D (AAFS), cleared by U+206C (IAFS).</summary>
            /// <remarks>
            /// TRUE if character codes in the Arabic Presentation Forms
            /// areas of Unicode should be shaped. (Not implemented).
            ///
            /// This is stored in the 8th bit of flags.
            /// </remarks>
            public bool fCharShape { get { return (flags & 0x80) != 0; } }

            /// <summary>Set by U+206E (NADS), cleared by U+206F (NODS).</summary>
            /// <remarks>
            /// TRUE if character codes U+0030 through U+0039 (European digits)
            /// are to be substituted by national digits. Set by Unicode NADS,
            /// cleared by NODS.
            ///
            /// This is stored in the 9th bit of flags.
            /// </remarks>
            public bool fDigitSubstitute { get { return (flags & 0x100) != 0; } }

            /// <summary>Equiv !GCP_Ligate, no Unicode control chars yet.</summary>
            /// <remarks>
            /// TRUE if ligatures are not to be used in the shaping of Arabic or
            /// Hebrew characters.
            ///
            /// This is stored in the 10th bit of flags.
            /// </remarks>
            public bool fInhibitLigate { get { return (flags & 0x200) != 0; } }

            /// <summary>Equiv GCP_DisplayZWG, no Unicode control characters yet.</summary>
            /// <remarks>
            /// TRUE if control characters are to be shaped as representational glyphs.
            /// (Typically, control characters are shaped to the blank glyph and
            /// given a width of zero).
            ///
            /// This is stored in the 11th bit of flags.
            /// </remarks>
            public bool fDisplayZWG { get { return (flags & 0x400) != 0; } }

            /// <summary>For EN->AN Unicode rule.</summary>
            /// <remarks>
            /// TRUE indicates prior strong characters were Arabic for the
            /// purposes of rule P0 as discussed in The Unicode Standard,
            /// version 2.0. This should normally be set to TRUE before
            /// itemizing an RTL paragraph in an Arabic language, and
            /// FALSE otherwise.
            ///
            /// This is stored in the 12th bit of flags.
            /// </remarks>
            public bool fArabicNumContext { get { return (flags & 0x800) != 0; } }

            /// <summary>Set when in LRO/RLO embedding.</summary>
            /// <remarks>
            /// For GetCharacterPlacement legacy support only. Initialize to TRUE
            /// to request ScriptShape to generate the pwLogClust array the same
            /// way as GetCharacterPlacement does in Arabic and Hebrew Windows.
            /// Affects only Arabic and Hebrew items.
            ///
            /// This is stored in the 13th bit of flags.
            /// </remarks>
            public bool fGcpClusters { get { return (flags & 0x1000) != 0; } }

            // NOTE: the rest of the bit fields are reserved.
        }

        /// <summary>
        /// The SCRIPT_CONTROL structure provides itemization control flags
        /// to the ScriptItemize function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_CONTROL
        {
            /// <summary>Bitmap flags on item.</summary>
            public UInt32 flags;

            /// <summary>For NADS, also default for context.</summary>
            /// <remarks>
            /// Specifies the language to use when Unicode values are ambiguous.
            /// This is a LANGID, including both a primary language and a
            /// sublanguage. Used by numeric processing to select digit shape
            /// when fDigitSubstitute (see SCRIPT_STATE) is set.
            ///
            /// This is stored in the first 16 bits of flags.
            /// </remarks>
            public UInt16 uDefaultLanguage
            {
                get { return (UInt16)(flags & 0xFFFF); }
                set { flags |= (UInt16) value; }
            }

            /// <summary>Means use previous script instead of uDefaultLanguage.</summary>
            /// <remarks>
            /// Specifies that national digits are chosen according to the
            /// nearest previous strong text, rather than using uDefaultLanguage.
            ///
            /// This is stored in the 17th bit of flags.
            /// </remarks>
            public bool fContextDigits { get { return (flags & 0x10000) != 0; } }

            // NOTE: there are more flags defined, that are here for legacy
            // support and have not been translated for P/Invoke support.
        }

        /// <summary>
        /// The SCRIPT_PROPERTIES structure has information about 
        /// special processing for each script.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        struct SCRIPT_PROPERTIES
        {
            /// <summary>Bitmap flags on item.</summary>
            public UInt32 flags;
            /// <summary>Overflow bitmap flags on item.</summary>
            public UInt32 flags2;

            public UInt16 langid { get { return (UInt16)(flags & 0xFFFF); } }

            /// <summary>
            /// If set, the script contains only digits and the other 
            /// characters used in writing numbers by the rules of 
            /// the Unicode bidirectional algorithm.
            /// </summary>
            /// <remarks>
            /// For example, currency symbols, the thousands separator, and 
            /// the decimal point are classified as numeric when adjacent to 
            /// or between digits.
            /// 
            /// This is stored in the 17th bit.
            /// </remarks>
            public bool fNumeric { get { return (flags & 0x10000) != 0; } }

            /// <summary>
            /// Script requires special shaping or layout.
            /// </summary>
            /// <remarks>
            /// If set, this is a language whose script requires special 
            /// shaping or layout. If fComplex is false, the script 
            /// contains no combining characters and requires no 
            /// contextual shaping or reordering. 
            /// 
            /// This is stored in the 18th bit.
            /// </remarks>
            public bool fComplex { get { return (flags & 0x20000) != 0; } }

            /// <summary>
            /// Requires ScriptBreak for word breaking information.
            /// </summary>
            /// <remarks>
            /// If set, this is a language whose word break placement 
            /// requires that the application call ScriptBreak and that 
            /// word break placement include character positions marked 
            /// as fWordStop in SCRIPT_LOGATTR. 
            /// 
            /// If not set, word break placement is identified by scanning 
            /// for characters marked as fWhiteSpace in SCRIPT_LOGATTR, 
            /// or for glyphs marked as uJustify == SCRIPT_JUSTIFY_BLANK 
            /// or SCRIPT_JUSTIFY_ARABIC_BLANK in SCRIPT_VISATTR. 
            /// 
            /// This is stored in the 19th bit.
            /// </remarks>
            public bool fNeedsWordBreaking { get { return (flags & 0x40000) != 0; } }

            /// <summary>
            /// Requires caret restriction to cluster boundaries.
            /// </summary>
            /// <remarks>
            /// If set, this is a language that restricts caret placement 
            /// to cluster boundaries, for example, Thai and Indian. To 
            /// determine valid caret positions, inspect the fCharStop flag 
            /// in the logical attributes returned by ScriptBreak, or compare 
            /// adjacent values in the pwLogClust array returned by 
            /// ScriptShape.
            /// 
            /// Note that ScriptXtoCP and ScriptCPtoX automatically apply caret placement restictions. 
            /// 
            /// This is stored in the 20th bit.
            /// </remarks>
            public bool fNeedsCaretInfo { get { return (flags & 0x80000) != 0; } }

            /// <summary>
            /// Charset to use when creating font.
            /// </summary>
            /// <remarks>
            /// The nominal charset associated with the script. This 
            /// charset may be used in a log font when creating a font 
            /// suitable for displaying this script. Note that for a 
            /// new script where no charset is defined, bCharSet may 
            /// be inappropriate. In this case, DEFAULT_CHARSET should be 
            /// used instead. See the description of fAmbiguousCharSet. 
            /// 
            /// This is stored in the 21st-28th bits.
            /// </remarks>
            public Byte bCharSet { get { return (Byte)((flags & 0xFF00000) >> 20); } }

            /// <summary>
            /// Contains only control characters.
            /// </summary>
            /// <remarks>
            /// If set, the script contains only control characters. Note, 
            /// the converse is not necessarily true - not every control 
            /// character ends up in a SCRIPT_CONTROL structure. 
            /// 
            /// This is stored in the 29th bit.
            /// </remarks>
            public bool fControl { get { return (flags & 0x10000000) != 0; } }

            /// <summary>
            /// This item is from the Unicode range U+E000 through U+F8FF.
            /// </summary>
            /// <remarks>
            /// If set, the script uses a special set of characters that is 
            /// privately defined for the Unicode range U+E000 through U+F8FF. 
            /// 
            /// This is stored in the 30th bit.
            /// </remarks>
            public bool fPrivateUseArea { get { return (flags & 0x20000000) != 0; } }

            /// <summary>
            /// Requires inter-character justification.
            /// </summary>
            /// <remarks>
            /// If set, justification for the script is achieved by 
            /// increasing the space between all letters, not just between 
            /// words. When performing inter-character justification, insert 
            /// extra space only after glyphs marked with 
            /// SCRIPT_VISATTR.uJustify == SCRIPT_JUSTIFY_CHARACTER. 
            /// 
            /// This is stored in the 31st bit.
            /// </remarks>
            public bool fNeedsCharacterJustify { get { return (flags & 0x40000000) != 0; } }

            /// <summary>
            /// Invalid combinations generate glyph wgInvalid in the glyph buffer.
            /// </summary>
            /// <remarks>
            /// If set, this is a script for which ScriptShape generates an 
            /// invalid glyph to represent invalid sequences. That is, it 
            /// generates wgInvalid in the glyph buffer. The glyph index of 
            /// the invalid glyph for a particular font may be obtained by 
            /// calling ScriptGetFontProperties. 
            /// 
            /// This is stored in the 32nd bit.
            /// </remarks>
            public bool fInvalidGlyph { get { return (flags & 0x80000000) != 0; } }

            /// <summary>
            /// Invalid combinations are marked by fInvalid in the logical attributes.
            /// </summary>
            /// <remarks>
            /// If set, this is a script for which ScriptBreak marks invalid 
            /// combinations by setting fInvalid in the logical attributes buffer. 
            /// 
            /// This is stored in the 33rd bit.
            /// </remarks>
            public bool fInvalidLogAttr { get { return (flags2 & 0x1) != 0; } }

            /// <summary>
            /// Contains Combining Diacritical Marks.
            /// </summary>
            /// <remarks>
            /// If set, the script contains an item that was analyzed by 
            /// ScriptItemize as including Combining Diacritical Marks
            /// (U+0300 through U+36F). 
            /// 
            /// This is stored in the 34th bit.
            /// </remarks>
            public bool fCDM { get { return (flags2 & 0x2) != 0; } }

            /// <summary>
            /// Script does not correspond 1:1 with a charset.
            /// </summary>
            /// <remarks>
            /// If set, the script contains characters that are supported by 
            /// more than one charset. See the Remarks section for more 
            /// information. The bCharSet member should be set to 
            /// DEFAULT_CHARSET.
            /// 
            /// This is stored in the 35th bit.
            /// </remarks>
            public bool fAmbiguousCharSet { get { return (flags2 & 0x4) != 0; } }

            /// <summary>
            /// Measured cluster width depends on adjacent clusters.
            /// </summary>
            /// <remarks>
            /// If set, this is a script, such as Arabic, in which 
            /// contextual shaping may cause a string to increase in size 
            /// when removing characters. An example of this is Arabic. 
            /// 
            /// This is stored in the 36th bit.
            /// </remarks>
            public bool fClusterSizeVaries { get { return (flags2 & 0x8) != 0; } }

            /// <summary>
            /// Invalid combinations should be rejected.
            /// </summary>
            /// <remarks>
            /// If set, this is a script, such as Thai, where invalid 
            /// sequences conventionally cause an editor program such as 
            /// Notepad to beep and ignore keystrokes. 
            /// 
            /// This is stored in the 37th bit.
            /// </remarks>
            public bool fRejectInvalid { get { return (flags2 & 0x10) != 0; } }
        }

        #endregion Inner types.
    }
}
