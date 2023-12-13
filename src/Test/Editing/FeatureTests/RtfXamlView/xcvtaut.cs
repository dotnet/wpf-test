// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using Common;

//this file is still under heavy developement. 

namespace RtfXamlView
{
    #region Conveter error helpers
    /// <summary>
    /// 
    /// </summary>
    public enum ConverterErrorType
    {
        /// <summary></summary>
        ErrorNone,
        /// <summary></summary>
        ErrorFile,
        /// <summary></summary>
        ErrorException,
        /// <summary></summary>
        ErrorRtfConversion,
        /// <summary></summary>
        ErrorXamlConversion
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConverterError
    {
        /// <summary></summary>
        public ConverterErrorType errortype;
        /// <summary></summary>
        public string ExceptionText;
    }
    #endregion

    #region Content State helpers
    #region old state helpers
    
    //Assuming the new compare code produces equiv results this should 
    //go away as it's designed arround a semi-different architecture and
    //no longer used. But ENSURE that the results are the same Before axing this code!!!
    
    /// <summary>
    /// 
    /// </summary>
    public enum FontAttribute
    {
        /// <summary></summary>
        none = 0,
        /// <summary></summary>
        bold = 1,
        /// <summary></summary>
        italic = 2,
        /// <summary></summary>
        underline = 4
    }

    class FontInfoEntry
    {
        public FontInfoEntry()
        {
            cpMin = 0;
            cpMax = 0;
            FontSize = 0;
            facename = "";
            fa = FontAttribute.none;
            fIsCR = false;
        }

        public static bool operator ==(FontInfoEntry fi1, FontInfoEntry fi2)
        {
            return fi1.Equals(fi2);
        }

        public static bool operator !=(FontInfoEntry fi1, FontInfoEntry fi2)
        {
            return !fi1.Equals(fi2);
        }

        public override bool Equals(object obj)
        {
            if (null == obj) return false;
            if (obj.GetType() != this.GetType()) return false;

            FontInfoEntry fie = obj as FontInfoEntry;
            // Don't compare cpMin and cpMax.
            // FontInfoEntry is an idealized font class, abstracting the actual implemenation
            // in Avalon, RichEdit, Word, etc.
            // Therefore, cpMin and cpMax could be different between different implementations of a "font". 
            // The cpMin and cpMax are primarily intended for help when debugging.
            if (facename == fie.facename &&
                fa == fie.fa &&
                FontSize == fie.FontSize)
            {
                return true;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public int cpMin;
        public int cpMax;
        public float FontSize;
        public string facename;
        public FontAttribute fa;
        public bool fIsCR;
    }

    internal class FontInfo : ArrayList
    {
        #region Constructors
        public FontInfo()
            : base(20)
        {
        }

        #endregion Constructors

        #region Public Methods
        public FontInfoEntry DefineEntry()
        {
            FontInfoEntry entry = new FontInfoEntry();
            Add(entry);
            return entry;
        }

        public FontInfoEntry EntryAt(int index)
        {
            return (FontInfoEntry)this[index];
        }

        // Merge adjacent font entries that are the same
        public void CompactList()
        {
            if (Count < 2)
                return; // nothing to do
            int index = 0;
            while (index < (Count - 1))
            {
                bool fItemsLeft = true;
                FontInfoEntry fie1 = this.EntryAt(index);
                FontInfoEntry fie2 = this.EntryAt(index + 1);
                while (fie1 == fie2 && fItemsLeft)
                {
                    fie1.cpMax = fie2.cpMax;
                    this.RemoveAt(index + 1);
                    if (Count > (index + 1))
                        fie2 = this.EntryAt(index + 1);
                    else
                        fItemsLeft = false;
                }
                index++;
            }
        }
        #endregion
    }

    class CompareState
    {
        public CompareState()
        {
            fi = new FontInfo();
        }

        public string text;
        public FontInfo fi;
    }
    #endregion

    #region Basic Typographic Element classes

    /// <summary>
    /// TypoAttrib: used to identify the type of TypoElement
    /// Note:the order for these top three is important as I sort based on it.
    /// </summary>
    public enum TypoAttrib
    {
        /// <summary></summary>
        paragraph = 0,
        /// <summary></summary>
        list = 1,
        /// <summary></summary>
        table = 2,
        /// <summary></summary>
        hyperlink = 3,
        /// <summary></summary>
        none = 4,
        /// <summary></summary>
        span = 5,
        /// <summary></summary>
        run = 6,
        /// <summary></summary>
        listitem = 7,
        /// <summary></summary>
        tablerow = 8,
        /// <summary></summary>
        tablecell = 9
    }

    /// <summary>
    /// FormatType: Identifies the type of 
    /// </summary>
    public enum FormatType
    {
        /// <summary></summary>
        FontName = 0,
        /// <summary></summary>
        ForeGround = 1,
        /// <summary></summary>
        FontSize = 2,
        /// <summary></summary>
        Underline = 3,
        /// <summary></summary>
        Bold = 4,
        /// <summary></summary>
        Italic = 5,
        /// <summary></summary>
        SubScript = 6,
        /// <summary></summary>
        SuperScript = 7,
        /// <summary></summary>
        Language = 8,
        /// <summary></summary>
        Kerning = 9,
        /// <summary></summary>
        None = 10
    }

    /// <summary>
    /// 
    /// </summary>
    public enum XamlProp
    {
        /// <summary></summary>
        None = 0,
        /// <summary></summary>
        FlowDir = 1,
        /// <summary></summary>
        Margin = 2,
        /// <summary></summary>
        Border = 3,
        /// <summary></summary>
        Padding = 4,
        /// <summary></summary>
        BorderColor = 5,
        /// <summary></summary>
        Foreground = 6,
        /// <summary></summary>
        Background = 7
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CompareMsg
    {
        /// <summary></summary>
        CompareFail = 0,
        /// <summary></summary>
        ComparePass = 1,
        /// <summary>abandon Para Compares</summary>
        CompareParaSyncAbandon = 2,
        /// <summary>attempt to resync the two state arrays</summary>
        CompareParaSyncFix = 3,
        /// <summary>This part of compare succeded, continue comparing</summary>
        CompareContinue = 4,
        /// <summary>Needed to continue to next iteration without failing</summary>
        CompareMsgSkip = 5,
        /// <summary>Avalon RichTextBoc was not populated as expected</summary>
        CompareFailAvalonNotLoaded = 6,
        ///<summary>Possible problem, look deeper, used mainly for para compare</summary>
        CompareDetailed = 7,
        ///<summary>Don't compare format runs</summary>
        CompareSkipFormat = 8

    }

    /// <summary>
    /// return value for cp node adjustments
    /// </summary>
    public enum AdjustResult
    {
        /// <summary></summary>
        AdjustDone = 0,
        /// <summary>delete this node</summary>
        AdjustDelete = 1,
        /// <summary>ignore this node, not in range</summary>
        AdjustIgnore = 2
    }

    static class TomHelpers
    {
        public static TomAlignment ConvertXamlAlignment(TextAlignment ta)
        {
            switch (ta)
            {
                case TextAlignment.Left:// = 0,
                    {
                        return TomAlignment.tomAlignLeft;
                    }
                case TextAlignment.Right://= 1,
                    {
                        return TomAlignment.tomAlignRight;
                    }
                case TextAlignment.Center:// = 2,
                    {
                        return TomAlignment.tomAlignCenter;
                    }
                case TextAlignment.Justify:// = 3,
                    {
                        return TomAlignment.tomAlignJustify;
                    }
                default:
                    {
                        return TomAlignment.tomUndefined;
                    }
            }
        }
        public static TomBool ConvertXamlWidowOrphan(Paragraph para)
        {
            if ((para.MinWidowLines == 0) || para.MinOrphanLines == 0)
            {
                return TomBool.tomTrue;
            }
            else
                return TomBool.tomFalse;
        }
        public static TomBool ConvertXamlBool(bool bVal)
        {
            return bVal ? TomBool.tomTrue : TomBool.tomFalse;
        }
        public static TomListType ConvertXamlListType(TextMarkerStyle ts)
        {
            string sz = ts.ToString();
            switch (ts.ToString())
            {
                case "Decimal":
                    {
                        return TomListType.tomListNumberAsArabic;
                    }
                case "LowerLatin":
                    {
                        return TomListType.tomListNumberAsLCLetter;
                    }
                case "UpperLatin":
                    {
                        return TomListType.tomListNumberAsUCLetter;
                    }
                case "LowerRoman":
                    {
                        return TomListType.tomListNumberAsLCRoman;
                    }
                case "UpperRoman":
                    {
                        return TomListType.tomListNumberAsUCRoman;
                    }
                case "Disc":
                    {
                        return TomListType.tomListBullet;
                    }
                default:
                    {
                        return TomListType.tomListNone;
                    }
            }
        }
        public static string TypoAttribToString(TypoAttrib ta)
        {
            switch (ta)
            {
                case TypoAttrib.table:
                    {
                        return "table";
                    }
                case TypoAttrib.list:
                    {
                        return "list";
                    }
                case TypoAttrib.paragraph:
                    {
                        return "paragraph";
                    }
                case TypoAttrib.hyperlink:
                    {
                        return "hyperlink";
                    }
                default:
                    {
                        return "none";
                    }
            }
        }
        public static string TomAlignToString(TomAlignment ta)
        {
            switch (ta)
            {
                case TomAlignment.tomAlignJustify:
                    {
                        return "Justify";
                    }
                case TomAlignment.tomAlignLeft:
                    {
                        return "Left";
                    }
                case TomAlignment.tomAlignRight:
                    {
                        return "Right";
                    }
                case TomAlignment.tomAlignCenter:
                    {
                        return "Center";
                    }
                default:
                    return "Unknown";
            }
        }
        public static string TypeToString(TomListType tl)
        {
            switch (tl)
            {
                case TomListType.tomListBullet:
                    {
                        return "Bullet";
                    }
                case TomListType.tomListNumberAsArabic:
                    {
                        return "Arabic";
                    }
                case TomListType.tomListNumberAsLCLetter:
                    {
                        return "LowerCase Letter";
                    }
                case TomListType.tomListNumberAsLCRoman:
                    {
                        return "LowerCase Roman";
                    }
                case TomListType.tomListNumberAsUCLetter:
                    {
                        return "UpperCase Letter";
                    }
                case TomListType.tomListNumberAsUCRoman:
                    {
                        return "UpperCase Roman";
                    }
                default:
                    {
                        return "None";
                    }
            }
        }
    }

    class TypoElement : IComparable
    {
        public TypoElement(bool fXAMLState, TypoAttrib ta)
        {
            _ta = ta;
            _fXAMLState = fXAMLState;
            _rcMargin = new Thickness(0, 0, 0, 0);
            _rcBorderThickness = new Thickness(0, 0, 0, 0);
            _rcPadding = new Thickness(0, 0, 0, 0);
            _cBackground = new Color();
            _cForeground = new Color();
            _cBorderColor = new Color();
            
            //_fd is used but for some reason the pre-compiler doesn't 
            //see it. Adding #pragma for neatness.            
#pragma warning disable 0219
            System.Windows.FlowDirection _fd = System.Windows.FlowDirection.LeftToRight;
#pragma warning restore 0219
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public static bool operator ==(TypoElement te1, TypoElement te2)
        {
            return te1.Equals(te2);
        }

        public static bool operator !=(TypoElement te1, TypoElement te2)
        {
            return !te1.Equals(te2);
        }

        public override bool Equals(object obj)
        {
            if (SameType(obj))
            {
                //if they are both xaml state elements, compare 
                //common xaml properties.
                if ((_fXAMLState == true) && (((TypoElement)obj)._fXAMLState == true))
                {
                    TypoElement te = obj as TypoElement;
                    if (te._fd.Equals(_fd) &&
                        te._cBackground.Equals(_cBackground) &&
                        te._cBorderColor.Equals(_cBorderColor) &&
                        te._cForeground.Equals(_cForeground) &&
                        te._rcBorderThickness.Equals(_rcBorderThickness) &&
                        te._rcMargin.Equals(_rcMargin) &&
                        te._rcPadding.Equals(_rcPadding))
                    {
                        return true;
                    }
                    else return false;
                }
                else return true;
            }
            else return false;
        }

        bool SameType(object obj)
        {
            if (null == obj) return false;
            if (obj.GetType() != this.GetType()) return false;
            TypoElement te = obj as TypoElement;
            if ((_ta == te._ta))
            {
                return true;
            }
            else
                return false;
        }


        // This is strictly for sorting it just helps to order items 
        // in the ArrayList.
        // broke a rule here, this method knows about the various
        // derived typo elements, so if there are new ones
        // added, it needs to be updated.
        public int CompareTo(object obj)
        {
            if (obj is TypoElement)
            {
                TypoElement target = (TypoElement)obj;

                if (_ta == target._ta)
                {
                    switch (_ta)
                    {
                        case TypoAttrib.table:
                            {
                                return ((TableElement)this)._iKey - ((TableElement)target)._iKey;
                            }
                        case TypoAttrib.list:
                            {
                                return ((ListElement)this)._iKey - ((ListElement)target)._iKey;
                            }
                        case TypoAttrib.paragraph:
                            {
                                return ((ParagraphElement)this)._iKey - ((ParagraphElement)target)._iKey;
                            }
                        case TypoAttrib.hyperlink:
                            {
                                //not implemented yet.
                                return 0;
                            }
                    }
                }
                else
                {
                    return _ta - target._ta;
                }
            }
            throw new ArgumentException("Object is not a TypoElement.");
        }

        #region TypoClass fields
        public TypoAttrib _ta;
        public bool _fXAMLState;
        
        //xaml specific
        public System.Windows.FlowDirection _fd;
        public Thickness _rcMargin;
        public Thickness _rcBorderThickness;
        public Thickness _rcPadding;
        public Color _cForeground;
        public Color _cBackground;
        public Color _cBorderColor;
        #endregion
    }

    #region ParaElements

    //only XAML state can use HyperlinkElement currently.
    //the reason is it is not straitforward or simple to gather 
    //hyperlink information from RTF, through TOM
    class HyperLinkElement : TypoElement
    {
        public HyperLinkElement(bool fXAMLState)
            : base(fXAMLState, TypoAttrib.hyperlink)
        {
        }

        #region Hyperlink specific fields
        public string szNavURI;
        public string szDispText;//in xaml this is the text inbetween that <Hyperlink>text</Hyperlink>tags,
        //for richedit, it is the non hidden text that the user see's.
        public string szTarget;

        #endregion
    }

    class ParagraphElement : TypoElement
    {
        public ParagraphElement(bool fXAMLState, int key)
            : base(fXAMLState, TypoAttrib.paragraph)
        {
            _dFirstLineIndent = 0;
            _dLineSpacing = 0;
            _taTextAlignment = TomAlignment.tomUndefined;
            _lstFontSize = new ArrayList();
            _lstFontName = new ArrayList();
            _lstForeGround = new ArrayList();
            _lstItalic = new ArrayList();
            _lstBold = new ArrayList();
            _lstKerning = new ArrayList();
            _lstLang = new ArrayList();
            _lstSubScript = new ArrayList();
            _lstSuperScript = new ArrayList();
            _lstUnderline = new ArrayList();
            _iKey = key;
            _fListPara = false;
            //_tltMarkerStyle = TomListType.tomListNone;

            _sbText = new StringBuilder();
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public static bool operator ==(ParagraphElement pe1, ParagraphElement pe2)
        {
            return pe1.Equals(pe2);
        }

        public static bool operator !=(ParagraphElement pe1, ParagraphElement pe2)
        {
            return !pe1.Equals(pe2);
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ParagraphElement pe = obj as ParagraphElement;

                if (_szText.Equals(pe._szText) &&
                      (pe._taTextAlignment == _taTextAlignment) &&
                      (pe._dFirstLineIndent == _dFirstLineIndent))
                {
                    return true;
                }
            }
            return false;
        }

        //start and end values are relative to the paragraph
        public SpanElement DefineElement(FormatType tp, int start, int end)
        {
            return new SpanElement(tp, start, end, base._fXAMLState, _iKey);
        }
        
        public void CompactSpanList()
        {
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            CompactRun(ref _lstFontName);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            CompactRun(ref _lstFontSize);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            CompactRun(ref _lstForeGround);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            CompactRun(ref _lstItalic);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            CompactRun(ref _lstBold);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            CompactRun(ref _lstKerning);
                            break;
                        }
                    case FormatType.Language:
                        {
                            CompactRun(ref _lstLang);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            CompactRun(ref _lstSubScript);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            CompactRun(ref _lstSuperScript);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            CompactRun(ref _lstUnderline);
                            break;
                        }
                }
            }
        }

        public string _szText
        {
            get
            {
                return _sbText.ToString();
            }
            set
            {
                _sbText = new StringBuilder((string)value);
            }
        }

        void CompactRun(ref ArrayList lst)
        {
            //if (lst.Count < 2)
            //    return; // nothing to do
            int index = 0;
            while (index < (lst.Count - 1))
            {
                bool fItemsLeft = true;
                SpanElement se1 = (SpanElement)lst[index];
                SpanElement se2 = (SpanElement)lst[index + 1];
                while (se1.FormatEquals(se2) && fItemsLeft)
                {
                    se1._iCPEnd = se2._iCPEnd;
                    lst.RemoveAt(index + 1);
                    if ((lst.Count-1) >= (index + 1))
                        se2 = (SpanElement)lst[index + 1];
                    else
                        fItemsLeft = false;
                }
                index++;
            }
        }

        #region ParagraphElement specific fields
        
        //if this paragraph is part of a list, 
        //these members below are set appropriatly.        
        public bool _fListPara;
        //public TomListType _tltMarkerStyle;
        public int _iList;//the list this item belongs to

        public StringBuilder _sbText;

        //para specific properties
        public double _dFirstLineIndent;
        public double _dLineSpacing;
        public TomAlignment _taTextAlignment;

        //Various types of format runs possible.
        public ArrayList _lstFontSize;
        public ArrayList _lstFontName;
        public ArrayList _lstForeGround;
        public ArrayList _lstItalic;
        public ArrayList _lstBold;
        public ArrayList _lstKerning;
        public ArrayList _lstLang;
        public ArrayList _lstSubScript;
        public ArrayList _lstSuperScript;
        public ArrayList _lstUnderline;

        public int _iKey;
        #endregion //ParagraphElement specific fields
    }
    
    //Spans are lumped together both runs and spans in an attempt
    //to "flatten" xaml a bit more towards rtf.    
    class SpanElement : TypoElement
    {
        public SpanElement(FormatType tp, int start, int end, bool fXAMLState, int iPara)
            : base(fXAMLState, TypoAttrib.span)
        {
            _type = tp;
            _iCPStart = start;
            _iCPEnd = end;
            //_lBackgroundColor = 0;
            _lForegroundColor = 0;
            _tbBold = TomBool.tomFalse;
            _tbItalic = TomBool.tomFalse;
            _fKerning = false;
            _lLanguageID = 0;
            //_fCharOffsetPosition = 0;
            _dFontSize = 0;
            //_fInterCharSpacing = 0;
            //_tbStrikeThrough = TomBool.tomFalse;
            _tbSuperScript = TomBool.tomFalse;
            _tbSubScript = TomBool.tomFalse;
            _tbUnderline = TomBool.tomFalse;
            //_lFontWieght = 0;
            _iPara = iPara;
        }

        //the only reason this is here is to get rid 
        //of the warning about it not being here.        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SpanElement se1, SpanElement se2)
        {
            return se1.Equals(se2);
        }

        public static bool operator !=(SpanElement se1, SpanElement se2)
        {
            return !se1.Equals(se2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;
            bool bRet = false;
            SpanElement se = obj as SpanElement;
            if (_type == se._type)
            {
                #region set bRet true if type == type
                switch (_type)
                {
                    case FormatType.FontName:
                        {
                            bRet = se._szFontName.Equals(_szFontName);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            bRet = se._dFontSize.Equals(_dFontSize);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            bRet = se._lForegroundColor.Equals(_lForegroundColor);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            bRet = se._tbItalic.Equals(_tbItalic);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            bRet = se._tbBold.Equals(_tbBold);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            bRet = se._fKerning.Equals(_fKerning);
                            break;
                        }
                    case FormatType.Language:
                        {
                            bRet = se._lLanguageID.Equals(_lLanguageID);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            bRet = se._tbSubScript.Equals(_tbSubScript);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            bRet = se._tbSuperScript.Equals(_tbSuperScript);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            bRet = se._tbUnderline.Equals(_tbUnderline);
                            break;
                        }
                }
                #endregion
            }
            else
                return false;
            if (bRet && (_iCPStart == se._iCPStart) && (_iCPEnd == se._iCPEnd))
            {
                return true;
            }
            else return false;
        }

        //This only validates that se is the same format type
        //and that the two have the same format value for that type.        
        public bool FormatEquals(SpanElement se)
        {
            bool bRet = false;
            if (_type == se._type)
            {
                #region set bRet true if type == type
                switch (_type)
                {
                    case FormatType.FontName:
                        {
                            bRet = se._szFontName.Equals(_szFontName);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            bRet = se._dFontSize.Equals(_dFontSize);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            bRet = se._lForegroundColor.Equals(_lForegroundColor);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            bRet = se._tbItalic.Equals(_tbItalic);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            bRet = se._tbBold.Equals(_tbBold);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            bRet = se._fKerning.Equals(_fKerning);
                            break;
                        }
                    case FormatType.Language:
                        {
                            bRet = se._lLanguageID.Equals(_lLanguageID);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            bRet = se._tbSubScript.Equals(_tbSubScript);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            bRet = se._tbSuperScript.Equals(_tbSuperScript);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            bRet = se._tbUnderline.Equals(_tbUnderline);
                            break;
                        }
                }
                #endregion
            }

            return bRet;
        }

        #region Span specific fields
        public FormatType _type;
        public int _iCPStart;
        public int _iCPEnd;
        
        //font information        
        public string _szFontName;
        //public long _lBackgroundColor;
        public long _lForegroundColor;
        //public long _lFontWieght;//this may change, see remarks of tom docs
        public long _lLanguageID;
        public TomBool _tbBold;
        public TomBool _tbItalic;
        //public TomBool _tbStrikeThrough;
        public TomBool _tbSuperScript;
        public TomBool _tbSubScript;
        public TomBool _tbUnderline;
        public bool _fKerning;
        //public float _fCharOffsetPosition;
        public double _dFontSize;
        //public float _fInterCharSpacing;
        int _iPara;//the para# this span belongs to.
        public string _szText;//see coments in xaml's handle run
        #endregion
    }
    #endregion//ParaElements

    #region List Elements
    class ListElement : TypoElement
    {
        public ListElement(bool fXAMLState, int iKey)
            : base(fXAMLState, TypoAttrib.list)
        {
            _iKey = iKey;
            _ListType = TomListType.tomListNone;
            _lListLevel = 0;
            _lListStart = 0;
            _fListTab = 0;

            _lstListItems = new ArrayList();
        }

        public ListItemElement DefineElement()
        {
            return new ListItemElement(base._fXAMLState, _iKey);
        }

        //the only reason this is here is to get rid 
        //of the warning about it not being here.        
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ListElement le1, ListElement le2)
        {
            return le1.Equals(le2);
        }

        public static bool operator !=(ListElement le1, ListElement le2)
        {
            return !le1.Equals(le2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;

            ListElement le = obj as ListElement;
            
            //This does not consider list items,
            //it is only concerned with basic top level list attribs            
            if (le._fListTab.Equals(_fListTab) &&
               le._iKey.Equals(_iKey) &&
               le._ListType.Equals(_ListType) &&
               le._lListLevel.Equals(_lListLevel) &&
               le._lListStart.Equals(_lListStart))
            {
                return true;
            }
            else return false;
        }

        #region ListElement specific fields
        public int _iKey;
        public TomListType _ListType;
        public long _lListLevel;
        public long _lListStart;
        public float _fListTab;
        public ArrayList _lstListItems;
        public string _szText;//text of first list item in list.
        #endregion
    }

    class ListItemElement : TypoElement
    {
        public ListItemElement(bool fXAMLState, int iParentKey)
            : base(fXAMLState, TypoAttrib.listitem)
        {
            _iSublistKey = 0;
            _iParentKey = iParentKey;
            _lstSubList = new ArrayList();
            _sbListText = new StringBuilder();
            //_fMatched = false;
        }

        public ListElement DefineElement()
        {
            _iSublistKey++;
            return new ListElement(base._fXAMLState, _iSublistKey - 1);

        }

        public string _szText
        {
            get
            {
                return _sbListText.ToString();
            }
            set
            {
                _sbListText = new StringBuilder((string)value);
            }
        }

        #region ListItemElement specific fields
        public ArrayList _lstSubList;//any sub lists just under this list item
        public int _iSublistKey;
        public int _iParentKey;
        StringBuilder _sbListText;//the list text for this item so I can compare later and ensure a good match.
        //bool _fMatched;
        #endregion
    }
    #endregion//List Elements

    #region table elements

    class TableCellElement : TypoElement
    {
        public TableCellElement(bool fXAMLState, int iTableKey, int iRowKey, int iCellKey)
            : base(fXAMLState, TypoAttrib.tablecell)
        {
            //currently these aren't used
            //int _iTableKey = iTableKey;
            //int _iRowKey = iRowKey;
            //int _iKey = iCellKey;
        }

        #region tablecell fields
        //currently these aren't used
        //int _iTableKey;
        //int _iRowKey;
        //int _iKey;
        #endregion
    }

    class TableRowElement : TypoElement
    {
        public TableRowElement(bool fXAMLState, int iParent, int iKey)
            : base(fXAMLState, TypoAttrib.tablerow)
        {
            _iParentKey = iParent;
            _iKey = iKey;
            _iCellKey = 0;
            _lstTableCells = new ArrayList();
        }

        public TableCellElement DefineElement()
        {
            _iCellKey++;
            return new TableCellElement(base._fXAMLState, _iParentKey, _iKey, (_iCellKey - 1));
        }

        //the only reason this is here is to get rid 
        //of the warning about it not being here.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TableRowElement tre1, TableRowElement tre2)
        {
            return tre1.Equals(tre2);
        }

        public static bool operator !=(TableRowElement tre1, TableRowElement tre2)
        {
            return !tre1.Equals(tre2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;

            TableRowElement tre = obj as TableRowElement;

            //This only ensures that, based on how we gather state info,
            //that these two items appear to be the same.
            //The base method can grab border info, but thats about it.            
            if (tre._iParentKey.Equals(_iParentKey) &&
               tre._iKey.Equals(_iKey) &&
               tre._lstTableCells.Count.Equals(_lstTableCells.Count))
            {
                return true;
            }
            else return false;
        }

        #region table row fields

        public ArrayList _lstTableCells;
        public int _iKey;
        public int _iParentKey;
        int _iCellKey;
        #endregion
    }

    class TableElement : TypoElement
    {
        public TableElement(bool fXAMLState, int iKey)
            : base(fXAMLState, TypoAttrib.table)
        {
            _iKey = iKey;
            _iRowKey = 0;
            _lstTableRows = new ArrayList();
        }

        //the only reason this is here is to get rid 
        //of the warning about it not being here.
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TableElement te1, TableElement te2)
        {
            return te1.Equals(te2);
        }

        public static bool operator !=(TableElement te1, TableElement te2)
        {
            return !te1.Equals(te2);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj)) return false;

            TableElement te = obj as TableElement;

            //This only ensures that, based on how we gather state info,
            //that these two items appear to be the same.
            //The base method can grab border info, but thats about it.
            if (te._iKey.Equals(_iKey) &&
               te._lstTableRows.Count.Equals(_lstTableRows.Count))
            {
                return true;
            }
            else return false;
        }

        public TableRowElement DefineElement()
        {
            _iRowKey++;
            return new TableRowElement(base._fXAMLState, _iKey, (_iRowKey - 1));

        }

        #region tableElement fields
        public ArrayList _lstTableRows;
        public int _iKey;
        int _iRowKey;
        #endregion
    }

    #endregion//table elements
    #endregion//Basic Typographic Element classes


    //holds all of the document's state information from 
    //either a richedit or xaml perspective.    
    class TypoInfo : ArrayList
    {
        public TypoInfo(bool fXaml)
            : base(20)
        {
            _iParagraphKey = 0;
            _iTableKey = 0;
            _iListKey = 0;
            _fIsXaml = fXaml;
        }
        public TypoElement DefineEntry(TypoAttrib ta)
        {
            TypoElement entry = CreateElement(ta);
            Add(entry);
            return entry;
        }
        
        //Need to be able to remove last para added if it has no text.        
        public void RemoveLast(ParagraphElement para)
        {
            Remove(para);
            _iParagraphKey--;
        }

        TypoElement CreateElement(TypoAttrib ta)
        {
            switch (ta)
            {
                case TypoAttrib.hyperlink:
                    {
                        return new HyperLinkElement(_fIsXaml);
                    }
                case TypoAttrib.list:
                    {
                        _iListKey++;
                        return new ListElement(_fIsXaml, _iListKey - 1);
                    }
                case TypoAttrib.paragraph:
                    {
                        _iParagraphKey++;
                        return new ParagraphElement(_fIsXaml, _iParagraphKey - 1);
                    }
                case TypoAttrib.table:
                    {
                        _iTableKey++;
                        return new TableElement(_fIsXaml, _iTableKey - 1);
                    }
                default:
                    return null;
            }//switch
        }

        public TypoElement EntryAt(int index)
        {
            return (TypoElement)this[index];
        }
        #region TypoInfo fields
        int _iParagraphKey;
        int _iTableKey;
        int _iListKey;
        public bool _fIsXaml;
        #endregion
    }

    #endregion//Content State helpers

    #region state comparison helpers

    #region Old State comparison help
    class RichEditStateCompare
    {
        public RichEditStateCompare()
        {
            _preState = new CompareState();
            _postState = new CompareState();
            _fIgnoreLastCR = false;
            _rtb = null;
        }

        public void SetRichTextBox(System.Windows.Forms.RichTextBox rtb)
        {
            _rtb = rtb;
        }

        #region Public State and Compare methods
        public void SetPreState()
        {
            // Text
            //PreState.text = _rtb.Text;
            ITextRange range = GetITextRange(0, 0);
            GetTextInfoFromRange(ref _preState, ref range);

            // Fonts
            GetFontInfoFromRange(ref _preState, ref range);
            _document = null;
        }

        public void SetPostState()
        {
            // Text
            //PostState.text = _rtb.Text;
            ITextRange range = GetITextRange(0, 0);
            GetTextInfoFromRange(ref _postState, ref range);

            // Fonts
            GetFontInfoFromRange(ref _postState, ref range);
            _document = null;
        }

        public bool Compare(ref string errstring)
        {
            _fIgnoreLastCR = false; // initialize state (workaround for copy / paste adding a CR)
            bool fPass = CompareText(ref errstring);
            if (fPass == true)
                fPass = CompareFonts(ref errstring);
            _fIgnoreLastCR = false; // reset
            return fPass;
        }
        #endregion

        #region Private Comparison helpers
        // Get the text from in RichEdit using TOM 
        // Note: WinForms does translations on things like CR's, so we cant do _rtb.Text
        // Additionally, TOM will return raw text, which lets us do more precise comparisons
        private void GetTextInfoFromRange(ref CompareState cs, ref ITextRange range)
        {
            cs.text = ""; // clear text
            range.Start = 0;
            range.End = 0;
            range.Expand(TomUnit.tomCharFormat);
            bool fFormatsLeft = true;
            int cpMinLast = 0;
            int cpMaxLast = 0;
            while (fFormatsLeft)
            {
                // walk the control by font, and only retrieve non-hidden text
                // This is to work around URL roundtrip problems.
                // Note that TOM does not expose the link property, hence why I have to use
                // the 'hint' of hidden text to determine if it is link text.
                // This is temporary, as I currently cannot count on always skipping hidden text, 
                // see Regression_Bug182
                if (range.Font.Hidden != TomBool.tomTrue)
                    cs.text += range.Text;

                range.Move(TomUnit.tomCharFormat, 1);
                range.Expand(TomUnit.tomCharFormat);
                if (range.Start == cpMinLast || range.End == cpMaxLast)
                {
                    fFormatsLeft = false;
                }
                cpMinLast = range.Start;
                cpMaxLast = range.End;
            }
        }

        // Get the font information from first range to end and store in passed CompareState
        private void GetFontInfoFromRange(ref CompareState cs, ref ITextRange range)
        {
            range.Move(TomUnit.tomParagraph, -1);
            range.Collapse(TomStartEnd.tomEnd);
            int cpMaxContent = range.End;
            range.Start = 0;
            range.End = 0;
            range.Expand(TomUnit.tomCharFormat);
            bool fFormatsLeft = true;
            while (fFormatsLeft)
            {
                if ((range.End - range.Start) == 1 && range.Text == "\r")
                {
                    bool fAtEnd = (range.End >= cpMaxContent);
                    // Many font compares fail on the CR.
                    // Some of these are interesting, but many are not.
                    // For now, I am ignoring these. But I am leaving in
                    // code that allows us to get the failures, but still
                    // track them as CR diffs (see fIsCR usage in compare fonts)
                    // fie.fIsCR = true;
                    range.Move(TomUnit.tomCharFormat, 1);
                    range.Expand(TomUnit.tomCharFormat);
                    if (fAtEnd && (range.Start >= cpMaxContent || range.End >= cpMaxContent))
                        fFormatsLeft = false;
                }
                else
                {
                    FontInfoEntry fie = cs.fi.DefineEntry();
                    fie.cpMin = range.Start;
                    fie.cpMax = range.End;
                    fie.facename = range.Font.Name;
                    fie.FontSize = range.Font.Size;
                    fie.fa |= (range.Font.Italic == TomBool.tomTrue) ? FontAttribute.italic : FontAttribute.none;
                    fie.fa |= (range.Font.Underline != TomUnderline.tomNone) ? FontAttribute.underline : FontAttribute.none;
                    fie.fa |= (range.Font.Bold == TomBool.tomTrue) ? FontAttribute.bold : FontAttribute.none;
                    range.Move(TomUnit.tomCharFormat, 1);
                    range.Expand(TomUnit.tomCharFormat);
                    if (range.Start == fie.cpMin || range.End == fie.cpMax)
                    {
                        fFormatsLeft = false;
                    }
                }
            }
        }

        // check for issues that have been resolved WF or BD.
        // Important: This function can change the text arrays and index!
        // It has to do this to be able to "fix up" issues that
        // the converter causes.
        private bool IsKnownTextIssue(ref int index)
        {
            if (_preState.text[index] == _postState.text[index])
                return true; // if the indices are equal, there is no issue

            switch ((int)_preState.text[index])
            {
                case 0x0C: // See Regression_Bug183. FF translates as CR.
                    {
                        // The tricky part here is that if the previous character in the prestate
                        // was a CR, then the 0x0C is completely eaten in the post state, not turned into a CR. 
                        // It is also completely eaten when the 0x0C is at the beginning of the control.
                        if (index == 0 ||
                            (index > 0 &&
                            _preState.text[index - 1] == 0x0D && _postState.text[index - 1] == 0x0D &&
                            _postState.text[index] != 0x0C))
                        {
                            // check to see if the next character matches
                            // If it does, we know we hit the eaten FF case, so eat
                            // eat the 0xC from the PreState array and return.
                            if (_preState.text.Length > index + 1 &&
                                _postState.text[index] == _preState.text[index + 1])
                            {
                                _preState.text = _preState.text.Remove(index, 1);
                                index--;
                                return true;
                            }
                        }
                        else if (_postState.text[index] == 0x0D)
                            return true; // FF converted to CR
                    }
                    break;

                case 0xFFFC:
                    {
                        // The converter does not currently support conversion of OLE or other images.
                        _preState.text = _preState.text.Remove(index, 1);
                        index--;
                        return true;
                    }

                case 0xFFFF:
                    {
                        // The converter does not currently support horizontally merged table cells.
                        // See Regression_Bug184.
                        // In RE, A horizontally or vertically merged cell has two characters: 
                        // NOTACHAR (0xFFFF) followed by CELL (0x7). Any text that appears 
                        // in a merged cell is stored in the first cell of the set of merged cells.
                        // A row with three cells has the plain text 0xFFF9 0xD 7 7 7 0xFFFB 0xD.
                        // The 0xFFFF should be replaced with a 7 until horz merge cells are supported.
                        if (_postState.text[index] == 0x7)
                        {
                            _preState.text = _preState.text.Remove(index, 1);
                            index--;
                            return true;
                        }
                    }
                    break;

                case 0xAD:
                    {
                        // Optional hyphen is dropped.
                        // 
                        _preState.text = _preState.text.Remove(index, 1);
                        index--;
                        return true;
                    }

                // Note for case "HYPERLINK "link text" display text:
                // HYPERLINK and other hidden text is stripped inside of GetTextInfoFromRange
                // If the converter starts to support URLs, we will keep link text in a seperate store.
            }

            // issues that are easier to identify from the post state
            switch ((int)_postState.text[index])
            {
                case 0x0020:
                    {
                        //as of 9/28/05 there is a problem with the converter adding a space
                        //on bulleted and or nubmered lines of text. This if clause will hopefully let us 
                        //work arround that until it is fixed.
                        _postState.text = _postState.text.Remove(index, 1);
                        index--;
                        return true;
                    }

                case 0xFFF9:
                    {
                        if (_preState.text[index] == 0x0D &&
                            _preState.text.Length > index + 1 &&
                            _preState.text[index + 1] == 0xFFF9)
                        {
                            _preState.text = _preState.text.Remove(index, 1);
                            index--;
                            return true;
                        }
                    }
                    break;
            }

            if ((int)_postState.text[index] >= 0x2550 &&
                (int)_postState.text[index] <= 0x2566)
            {
                // richedit interprets some old RTF
                // as one code page, and Word interprets it with another.
                // Terry is following Word's behavior.
                // It is in the box drawing area of Unicode.
                if (_preState.text[index] >= 0xCB ||
                    _preState.text[index] <= 0xCD)
                {
                    return true;
                }
            }

            return false;
        }

        // Compare the text from the PreState and PostState
        private bool CompareText(ref string errstring)
        {
            bool fPass = true;
            // Note: IsKnownTextIssue can change the text array lengths and the index value!
            for (int index = 0; (index < _preState.text.Length && index < _postState.text.Length) && fPass; index++)
            {
                if (_preState.text[index] != _postState.text[index] &&
                    false == IsKnownTextIssue(ref index)) // dont log known issues for why the conversion failed
                {
                    errstring = "Text Compare failed at index " + index.ToString() +
                        ", pre U+" + ((uint)_preState.text[index]).ToString("x") +
                        ", post U+" + ((uint)_postState.text[index]).ToString("x") +
                        ", pre=\"" +
                        _preState.text.Substring(index, Math.Min(5, _preState.text.Length - index - 1)) + "\"" +
                        ", post=\"" +
                        _postState.text.Substring(index, Math.Min(5, _postState.text.Length - index - 1)) + "\"";
                    fPass = false;
                }
            }
            int prelength = _preState.text.Length;
            int postlength = _postState.text.Length;
            if (fPass && prelength != postlength)
            {
                fPass = false;
                int diff = prelength - postlength;
                if (diff > 0)
                {
                    errstring = "Text Compare failed - PRE string longer by " + diff.ToString() +
                        " characters " + _preState.text.Substring(Math.Max(0, postlength - 1), diff);
                }
                else if ((diff == -1) && (_postState.text[postlength - 1] == 0x000d))
                {
                    //$Comment: When we moved to the integrated converter
                    //we had to use SelectAll on the Xaml RichTextBox which puts
                    // and extra return at the end. This corrects that. Note: as of 9/23/05
                    //Trying to use TextPointers to avoid selecting this last para doesn't work 
                    //(or couldn't get it to work).
                    fPass = true;
                    _fIgnoreLastCR = true;
                }
                else
                {
                    errstring = "Text Comare failed - POST string longer by " + ((int)Math.Abs(diff)).ToString() +
                          " characters " + _postState.text.Substring(Math.Max(0, prelength - 1), Math.Abs(diff));
                }
            }
            return fPass;
        }

        private bool CompareFonts(ref string errstring)
        {
            // Compact the font lists to merge adjacent entries that are the same
            _preState.fi.CompactList();
            _postState.fi.CompactList();
            bool fPass = true;
            for (int index = 0; (index < _preState.fi.Count && index < _postState.fi.Count) && fPass; index++)
            {
                FontInfoEntry fiePre = _preState.fi.EntryAt(index);
                FontInfoEntry fiePost = _postState.fi.EntryAt(index);
                //for some reason font sizes are showing up as .5 less than the original.
                //the converted text doesn't seem to show this nor the round tripped, anywhere 
                //but in the comparison. Fixing up so bvt can look deeper.
                if (fiePre.facename.Equals(fiePost.facename) && (fiePre.fa == fiePost.fa) && (fiePre.FontSize - .5) == fiePost.FontSize)
                {
                    fiePost.FontSize = fiePost.FontSize + (float)0.5;
                }
                if (fiePre != fiePost)
                {
                    errstring = "Font Compare failed at run index " + index.ToString() +
                        ", PRE: cpMin=" + fiePre.cpMin.ToString() +
                        ", cpMax=" + fiePre.cpMax.ToString() +
                        ", face=" + fiePre.facename +
                        ", Size=" + fiePre.FontSize +
                        ", attributes=" + fiePre.fa.ToString() +
                        ", POST: cpMin=" + fiePost.cpMin.ToString() +
                        ", cpMax=" + fiePost.cpMax.ToString() +
                        ", face=" + fiePost.facename +
                        ", Size=" + fiePost.FontSize +
                        ", attributes=" + fiePost.fa.ToString();
                    if (fiePost.fIsCR || fiePre.fIsCR)
                        errstring += ", Run is CR";
                    fPass = false;
                }
            }
            if (fPass && _preState.fi.Count != _postState.fi.Count)
            {
                errstring = "Extra font entry";
                int diff = _preState.fi.Count - _postState.fi.Count;
                // PreState.fi.Count > PostState.fi.Count
                if (diff == -1 && _fIgnoreLastCR)
                {
                    //$Comment: When we moved to the integrated converter
                    //we had to use SelectAll on the Xaml RichTextBox which puts
                    // and extra return at the end. This corrects that. Note: as of 9/23/05
                    //Trying to use TextPointers to avoid selecting this last para doesn't work 
                    //(or couldn't get it to work).
                    // this change mirrors that made in CompareText.
                    fPass = true;
                }
                else if (diff > 0)
                {
                    FontInfoEntry fiePre = _preState.fi.EntryAt(_preState.fi.Count - 1);
                    errstring += ", PRE: cpMin=" + fiePre.cpMin.ToString() +
                    ", cpMax=" + fiePre.cpMax.ToString() +
                    ", face=" + fiePre.facename +
                    ", attributes=" + fiePre.fa.ToString();
                    fPass = false;
                }
                else
                {
                    FontInfoEntry fiePost = _postState.fi.EntryAt(_postState.fi.Count - 1);
                    errstring += ", POST: cpMin=" + fiePost.cpMin.ToString() +
                    ", cpMax=" + fiePost.ToString() +
                    ", face=" + fiePost.facename +
                    ", attributes=" + fiePost.fa.ToString();
                    fPass = false;
                }

            }
            return fPass;
        }

        // Get an ITextRange pointer from RichEdit via MSAA (WM_GETOBJECT)
        ITextRange GetITextRange(int cpMin, int cpMax)
        {
            if (_document == null)
            {
                IntPtr hwndRE = _rtb.Handle;
                Guid IID_IDispatch = new Guid("00020400-0000-0000-C000-000000000046");
                Object obj = IntPtr.Zero;
                const int OBJID_NATIVEOM = -16; //0xFFFFFFF0
                int hResult = AccessibleObjectFromWindow(hwndRE, OBJID_NATIVEOM, ref IID_IDispatch, ref obj);
                _document = obj as ITextDocument;
            }
            ITextRange range = _document.Range(cpMin, cpMax);
            return range;
        }
        #endregion

        #region Private RichEdit and interop members
        // Get TOM via RichEdit MSAA proxy
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, int id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);
        private ITextDocument _document; // TOM ITextDocument
        System.Windows.Forms.RichTextBox _rtb; // RichTextBox
        #endregion

        #region Private state information
        CompareState _preState;
        CompareState _postState;
        bool _fIgnoreLastCR; // default false. Set to true if we need to ignore a trailing CR.
        #endregion

    }
    #endregion

    //Handles XAML specific state gathering    
    class XamlStateCompare
    {
        public XamlStateCompare(System.Windows.Controls.RichTextBox xtb)
        {
            _xamlParas = new TypoInfo(true);
            _xamlLists = new TypoInfo(true);
            _xamlTables = new TypoInfo(true);
            _xamlHyperlinks = new TypoInfo(true);
            _xtb = xtb;
        }

        public CompareMsg SetState()
        {
            //the BlockCollection contains all of the xaml elements for this doc
            BlockCollection docBlocks = _xtb.Document.Blocks;
            _defaultFlowDir = _xtb.Document.FlowDirection;
            
            //If the conversion failed but did not assert or throw
            //an exception, there will only be one default block in 
            //doc blocks and it will have no text.
            //Additionally, there are some rtf documents that would 
            //naturally produce no xaml so some of these fails will be 
            //false fails.            
            if (docBlocks.Count <= 1)
            {
                switch (docBlocks.FirstBlock.ToString())
                {
                    case "System.Windows.Documents.Paragraph":
                        {                            
                            //Some documents only have 1 paragraph, check to see if
                            //this one para has text or not. if not, bail.
                            if (GetText((Paragraph)docBlocks.FirstBlock).Length < 1)
                                return CompareMsg.CompareFailAvalonNotLoaded;
                            break;
                        }
                    case "System.Windows.Documents.Table":
                        {
                            break;
                        }
                    case "System.Windows.Documents.List":
                        {
                            break;
                        }
                    default:
                        {                            
                            //it indicates control not populated.                            
                            return CompareMsg.CompareFailAvalonNotLoaded;
                        }
                }
            }
            HandleBlocks(docBlocks);
            _xamlParas.Sort();
            _xamlLists.Sort();
            _xamlTables.Sort();
            _xamlHyperlinks.Sort();
            return CompareMsg.CompareContinue;
        }

        #region State Gathering methods

        string GetText(Paragraph para)
        {
            TextRange range = new TextRange(para.ContentStart, para.ContentEnd);

            return range.Text;
        }

        string GetText(Span sp)
        {
            TextRange range = new TextRange(sp.ContentStart, sp.ContentEnd);
            return range.Text;
        }
        string GetText(Hyperlink link)
        {
            TextRange range = new TextRange(link.ContentStart, link.ContentEnd);
            return range.Text;
        }

        #region Block level handlers

        void HandleBlocks(BlockCollection Blocks)
        {
            foreach (Block bk in Blocks)
            {
                switch (bk.ToString())
                {
                    case "System.Windows.Documents.List":
                        {
                            HandleList((List)bk);
                            break;
                        }
                    case "System.Windows.Documents.Paragraph":
                        {
                            HandlePara((Paragraph)bk, null);
                            break;
                        }
                    case "System.Windows.Documents.Table":
                        {
                            HandleTable((Table)bk);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        
        //this will allow to ensure sub lists wind up 
        //in the parent list's _lstsublist array.        
        void HandleListItemBlocks(ref ListItemElement ParentLE, BlockCollection Blocks)
        {
            foreach (Block bk in Blocks)
            {
                switch (bk.ToString())
                {
                    case "System.Windows.Documents.List":
                        {
                            HandleChildList(ref ParentLE, (List)bk);
                            break;
                        }
                    case "System.Windows.Documents.Paragraph":
                        {
                            ParentLE._szText = HandlePara((Paragraph)bk, ParentLE)._szText;
                            break;
                        }
                    case "System.Windows.Documents.Table":
                        {
                            HandleTable((Table)bk);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        void HandleChildList(ref ListItemElement ParentLE, List lst)
        {
            ListItemElement CurrentItem = null;

            ListElement le = ParentLE.DefineElement();
            HandleBlockAttribs((Block)lst, (TypoElement)le);

            le._lListStart = lst.StartIndex;
            le._fListTab = (float)lst.MarkerOffset;
            le._ListType = TomHelpers.ConvertXamlListType(lst.MarkerStyle);
            ParentLE._lstSubList.Add(le);

            foreach (ListItem item in lst.ListItems)
            {
                CurrentItem = le.DefineElement();
                HandleListItemBlocks(ref CurrentItem, item.Blocks);
                le._lstListItems.Add(CurrentItem);
            }
        }

        void HandleList(List lst)
        {
            ListItemElement CurrentItem = null;

            ListElement le = (ListElement)_xamlLists.DefineEntry(TypoAttrib.list);
            HandleBlockAttribs((Block)lst, (TypoElement)le);
            le._lListStart = lst.StartIndex;
            le._fListTab = (float)lst.MarkerOffset;
            le._ListType = TomHelpers.ConvertXamlListType(lst.MarkerStyle);

            foreach (ListItem item in lst.ListItems)
            {
                CurrentItem = le.DefineElement();
                HandleListItemBlocks(ref CurrentItem, item.Blocks);
                le._lstListItems.Add(CurrentItem);
                
                //ListItem also contains the ListItem.List member which is a collection of some sort.                
            }
        }

        ParagraphElement HandlePara(Paragraph para, ListItemElement lie)
        {
            ParagraphElement pe = (ParagraphElement)_xamlParas.DefineEntry(TypoAttrib.paragraph);

            //record xaml specific attribs
            HandleBlockAttribs((Block)para, (TypoElement)pe);

            //record general para props.
                        #region 
            pe._taTextAlignment = TomHelpers.ConvertXamlAlignment(para.TextAlignment);
            pe._dFirstLineIndent = Common.Converters.PxToPt(para.TextIndent);
            pe._dLineSpacing = Common.Converters.PxToPt(para.LineHeight);
            
            #endregion

            //deal with the paragraph contents
            #region 
            if (para.Inlines.Count > 0)
            {
                bool fSpanUnderline = false;
                if (para.TextDecorations != null)
                {
                    foreach (TextDecoration td in para.TextDecorations)
                    {
                        if (td.Location == TextDecorationLocation.Underline)
                        {
                            fSpanUnderline = true;
                        }
                    }
                }
                HandleInlines(ref pe, ref para, para.Inlines, fSpanUnderline);
            } 
            #endregion

            //Note: that this is a paragraph in a list.
            if (((object)lie) != null)
            {
                pe._fListPara = true;
                pe._iList = lie._iParentKey;//does it matter which list it is? Or just that it's a paragraph in a list?
            }

            //fixup cp's in span's so that they are relative to the para
            //not the document.
            FixupXamlSpanCPs(ref pe);
            pe.CompactSpanList();
            return pe;
        }

        void FixupXamlSpanCPs(ref ParagraphElement pe)
        {
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            FixupArrayCPs(ref pe._lstFontName);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            FixupArrayCPs(ref pe._lstFontSize);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            FixupArrayCPs(ref pe._lstForeGround);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            FixupArrayCPs(ref pe._lstItalic);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            FixupArrayCPs(ref pe._lstBold);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            FixupArrayCPs(ref pe._lstKerning);
                            break;
                        }
                    case FormatType.Language:
                        {
                            FixupArrayCPs(ref pe._lstLang);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            FixupArrayCPs(ref pe._lstSubScript);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            FixupArrayCPs(ref pe._lstSuperScript);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            FixupArrayCPs(ref pe._lstUnderline);
                            break;
                        }
                }
            }
        }

        void FixupArrayCPs(ref ArrayList lst)
        {
            int cp = 0;
            foreach (SpanElement se in lst)
            {
                se._iCPStart = cp;
                cp = se._iCPEnd = cp + se._szText.Length;
            }
        }

        void HandleTable(Table tbl)
        {
            TableRowElement CurrentRow = null;
            TableCellElement CurrentCell = null;
            TableElement te = (TableElement)_xamlTables.DefineEntry(TypoAttrib.table);
            HandleBlockAttribs((Block)tbl, (TypoElement)te);

            foreach (TableRowGroup trg in tbl.RowGroups)
            {
                foreach (TableRow tr in trg.Rows)
                {
                    CurrentRow = te.DefineElement();
                    //Not ready yet
                    //HandleBlockAttribs((Block)tr, (TypoElement)CurrentRow);
                    foreach (TableCell tc in tr.Cells)
                    {
                        CurrentCell = CurrentRow.DefineElement();
                        CurrentRow._lstTableCells.Add(CurrentCell);
                        HandleBlocks(tc.Blocks);
                    }
                    te._lstTableRows.Add(CurrentRow);
                }
            }
        }

        //Collects Xaml specific attribs
        void HandleBlockAttribs(Block bk, TypoElement te)
        {
            te._rcPadding.Bottom = bk.Padding.Bottom;
            te._rcPadding.Top = bk.Padding.Top;
            te._rcPadding.Left = bk.Padding.Left;
            te._rcPadding.Right = bk.Padding.Right;

            te._rcMargin.Bottom = bk.Margin.Bottom;
            te._rcMargin.Top = bk.Margin.Top;
            te._rcMargin.Right = bk.Margin.Right;
            te._rcMargin.Left = bk.Margin.Left;

            te._rcBorderThickness.Bottom = bk.BorderThickness.Bottom;
            te._rcBorderThickness.Top = bk.BorderThickness.Top;
            te._rcBorderThickness.Left = bk.BorderThickness.Left;
            te._rcBorderThickness.Right = bk.BorderThickness.Right;
            if (bk.BorderBrush != null)
            {
                SolidColorBrush br = (SolidColorBrush)bk.BorderBrush;
                te._cBorderColor.B = br.Color.B;
                te._cBorderColor.G = br.Color.G;
                te._cBorderColor.R = br.Color.R;
            }
            if (bk.Background != null)
            {
                SolidColorBrush br = (SolidColorBrush)bk.Background;
                te._cBackground.B = br.Color.B;
                te._cBackground.G = br.Color.G;
                te._cBackground.R = br.Color.R;
            }
            if (bk.Foreground != null)
            {
                SolidColorBrush br = (SolidColorBrush)bk.Foreground;
                te._cForeground.B = br.Color.B;
                te._cForeground.G = br.Color.G;
                te._cForeground.R = br.Color.R;
            }
            te._fd = bk.FlowDirection;
        }

        #endregion//Block level handlers

        #region Inline/TextElement handlers

        void HandleInlines(ref ParagraphElement pe, ref Paragraph para, InlineCollection ic, bool fUnderline)
        {
            foreach (Inline element in ic)
            {
                switch (element.ToString())
                {
                    case "System.Windows.Documents.Span":
                        {
                            HandleSpan(ref pe, ref para, (Span)element, fUnderline);
                            break;
                        }
                    case "System.Windows.Documents.Run":
                        {
                            HandleRun(ref pe, ref para, (Run)element, fUnderline);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        void HandleSpan(ref ParagraphElement pe, ref Paragraph para, Span sp, bool fUnderlined)
        {
            #region Old code not used but left for reference
            
            //Currently the converter uses spans to break out various kinds
            //of formatting. I check to see if there is more than one, inline
            //for this span. If not, I'm assumming that the run tag itself
            //just contains the text for this span and no formating info.
            //when we run through XAML test cases, this may change.
            
            //if (sp.Inlines.Count <= 1)
            //{
            //    
            //    // I store the text, then when compacting, do a count of chars.
            //    //store the run's cp's relative to para start.
            //    
            //    for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            //    {
            //        #region //Deal with each type of format

            //        SpanElement se = pe.DefineElement(tp, 0, 0);
            //        HandleInlineAttribs((TypoElement)se, (Inline)sp);//record xaml specific properties
            //        se._szText = GetText(sp);
            //        switch (tp)
            //        {
            //            case FormatType.FontName:
            //                {
            //                    se._szFontName = sp.FontFamily.Source;
            //                    pe._lstFontName.Add(se);
            //                    break;
            //                }
            //            case FormatType.FontSize:
            //                {
            //                    se._dFontSize = Common.Converters.PxToPt(sp.FontSize);
            //                    pe._lstFontSize.Add(se);
            //                    break;
            //                }
            //            case FormatType.ForeGround:
            //                {
            //                    if (sp.Foreground != null)
            //                    {
            //                        SolidColorBrush br = (SolidColorBrush)sp.Foreground;
            //                        se._lForegroundColor = br.Color.B;
            //                        se._lForegroundColor = se._lForegroundColor << 8;
            //                        se._lForegroundColor += br.Color.G;
            //                        se._lForegroundColor = se._lForegroundColor << 8;
            //                        se._lForegroundColor += br.Color.R;

            //                    }
            //                    else
            //                    {
            //                        se._lForegroundColor = 0;
            //                    }
            //                    pe._lstForeGround.Add(se);
            //                    break;
            //                }
            //            case FormatType.Italic:
            //                {
            //                    if (sp.FontStyle.ToString().Equals("Italic"))
            //                    {
            //                        se._tbItalic = TomBool.tomTrue;
            //                    }
            //                    pe._lstItalic.Add(se);
            //                    break;
            //                }
            //            case FormatType.Bold:
            //                {
            //                    if (sp.FontWeight.ToString().Equals("Bold"))
            //                    {
            //                        se._tbBold = TomBool.tomTrue;
            //                    }
            //                    pe._lstBold.Add(se);
            //                    break;
            //                }
            //            case FormatType.Kerning:
            //                {
            //                    se._fKerning = sp.Typography.Kerning;
            //                    pe._lstKerning.Add(se);
            //                    break;
            //                }
            //            case FormatType.Language:
            //                {
            //                    try
            //                    {
            //                        System.Globalization.CultureInfo ci;
            //                        ci = sp.Language.GetEquivalentCulture();
            //                        se._lLanguageID = ci.LCID;
            //                    }
            //                    catch (Exception x)
            //                    {
            //                        // if no equivelent culture, use undefined or neutral value.
            //                        if (x.GetType() == typeof(InvalidOperationException))
            //                            se._lLanguageID = 0x00;
            //                        else
            //                            throw (x);
            //                    }
            //                    pe._lstLang.Add(se);
            //                    break;
            //                }
            //            case FormatType.SubScript:
            //                {

            //                    pe._lstSubScript.Add(se);
            //                    break;
            //                }
            //            case FormatType.SuperScript:
            //                {

            //                    pe._lstSuperScript.Add(se);
            //                    break;
            //                }
            //            case FormatType.Underline:
            //                {
            //                    if (sp.TextDecorations != null)
            //                    {
            //                        foreach (TextDecoration td in sp.TextDecorations)
            //                        {
            //                            if (td.Location == TextDecorationLocation.Underline)
            //                            {
            //                                se._tbUnderline = TomBool.tomTrue;
            //                            }
            //                        }
            //                    }
            //                    pe._lstUnderline.Add(se);
            //                    break;
            //                }
            //            //not doing this one yet
            //            //case StrikeThrough
            //        }
            //        #endregion
            //    }
            //}
            //else if (sp.Inlines.Count > 1)
            //{
            #endregion

            if (!fUnderlined)
            {
                if (sp.TextDecorations != null)
                {
                    foreach (TextDecoration td in sp.TextDecorations)
                    {
                        if (td.Location == TextDecorationLocation.Underline)
                        {
                            fUnderlined = true;
                        }
                    }
                }
            }
            HandleInlines(ref pe, ref para, sp.Inlines, fUnderlined);
        }

        // only detect hyperlinks for xaml so they only get compared on xaml to xaml test.
        
        void HandleHyperlink(ref ParagraphElement pe, ref Paragraph para, Hyperlink link)
        {
            HyperLinkElement theLink = (HyperLinkElement)_xamlHyperlinks.DefineEntry(TypoAttrib.hyperlink);
            HandleInlineAttribs((TypoElement)theLink, (Inline)link);
            theLink.szNavURI = link.NavigateUri.OriginalString;
            theLink.szTarget = link.TargetName;
            theLink.szDispText = GetText(link);

            if (link.Inlines.Count > 0)
            {
                HandleInlines(ref pe, ref para, link.Inlines, false);//stubbed to not be underlined for now
            }
        }

        void HandleRun(ref ParagraphElement pe, ref Paragraph para, Run run, bool fUnderline)
        {
            //store the text, then when compacting, do a count of chars
            //to store the run's cp's relative to para start.
            pe._szText += run.Text;
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                SpanElement se = pe.DefineElement(tp, 0, 0);
                //HandleInlineAttribs((TypoElement)se, (Inline)sp);//record xaml specific properties
                se._szText = run.Text;
                switch (tp)
                {
                    case FormatType.FontName:
                            #region 
                        {
		                    se._szFontName = run.FontFamily.Source;
                            pe._lstFontName.Add(se);
                            break;
                        }
                            #endregion
                    case FormatType.FontSize:
                        #region 
                        {
                            se._dFontSize = Common.Converters.PxToPt(run.FontSize);
                            pe._lstFontSize.Add(se);
                            break;
                        }
                        #endregion 
                    case FormatType.ForeGround:
                        #region 
                        {
                            if (run.Foreground != null)
                            {
                                SolidColorBrush br = (SolidColorBrush)run.Foreground;
                                se._lForegroundColor = br.Color.B;
                                se._lForegroundColor = se._lForegroundColor << 8;
                                se._lForegroundColor += br.Color.G;
                                se._lForegroundColor = se._lForegroundColor << 8;
                                se._lForegroundColor += br.Color.R;
                            }
                            else
                            {
                                se._lForegroundColor = 0;
                            }
                            pe._lstForeGround.Add(se);
                            break;
                        } 
                        #endregion
                    case FormatType.Italic:
                        #region 
                        {
                            if (run.FontStyle.ToString().Equals("Italic"))
                            {
                                se._tbItalic = TomBool.tomTrue;
                            }
                            pe._lstItalic.Add(se);
                            break;
                        } 
                        #endregion
                    case FormatType.Bold:
                        #region 
                        {
                            if (run.FontWeight.ToString().Equals("Bold"))
                            {
                                se._tbBold = TomBool.tomTrue;
                            }
                            pe._lstBold.Add(se);
                            break;
                        } 
                        #endregion
                    case FormatType.Kerning:
                        #region 
                        {
                            se._fKerning = run.Typography.Kerning;
                            pe._lstKerning.Add(se);
                            break;
                        } 
                        #endregion
                    case FormatType.Language:
                        #region 
                        {
                            try
                            {
                                System.Globalization.CultureInfo ci;
                                ci = run.Language.GetEquivalentCulture();
                                se._lLanguageID = ci.LCID;
                            }
                            catch (Exception x)
                            {
                                // if no equivelent culture, use undefined or nuetral value.
                                if (x.GetType() == typeof(InvalidOperationException))
                                    se._lLanguageID = 0x00;
                                else
                                    throw (x);
                            }
                            pe._lstLang.Add(se);
                            break;
                        } 
                        #endregion
                    case FormatType.SubScript:
                        {
                            //add in sub and super script gathering code!!!
                            pe._lstSubScript.Add(se);
                            break;
                        }
                    case FormatType.SuperScript:
                        {

                            pe._lstSuperScript.Add(se);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            if (fUnderline)
                            {
                                se._tbUnderline = TomBool.tomTrue;
                            }
                            else if (run.TextDecorations != null)
                            {
                                foreach (TextDecoration td in run.TextDecorations)
                                {
                                    if (td.Location == TextDecorationLocation.Underline)
                                    {
                                        se._tbUnderline = TomBool.tomTrue;
                                    }
                                }
                            }
                            pe._lstUnderline.Add(se);
                            break;
                        }
                }
            }
        }

        //Collects Xaml specific attribs, for xaml to xaml compare
        void HandleInlineAttribs(TypoElement te, Inline item)
        {
            if (item.Background != null)
            {
                SolidColorBrush br = (SolidColorBrush)item.Background;
                te._cBackground.B = br.Color.B;
                te._cBackground.G = br.Color.G;
                te._cBackground.R = br.Color.R;
            }
            if (item.Foreground != null)
            {
                SolidColorBrush br = (SolidColorBrush)item.Foreground;
                te._cForeground.B = br.Color.B;
                te._cForeground.G = br.Color.G;
                te._cForeground.R = br.Color.R;
            }
            te._fd = item.FlowDirection;
        }

        #endregion //Inline/TextElement handlers

        #endregion //State Gathering methods

        #region class fields
        public TypoInfo _xamlParas;
        public TypoInfo _xamlLists;
        public TypoInfo _xamlTables;
        public TypoInfo _xamlHyperlinks;
        System.Windows.Controls.RichTextBox _xtb;
        FlowDirection _defaultFlowDir;
        #endregion
    }

    //Handles richedit specific state gathering
    class NewREStateCompare
    {
        public NewREStateCompare()
        {
            _rtfParas = new TypoInfo(false);
            _rtfLists = new TypoInfo(false);
            _rtfTables = new TypoInfo(false);
            _rtb = null;
        }

        public void SetRichTextBox(System.Windows.Forms.RichTextBox rtb)
        {
            _rtb = rtb;
        }

        public bool SetState()
        {
            //first determine if there are any tables in the document
            //this is one of the items that requires scanning through 
            //the raw text of the document.
            ITextRange range = null;
            range = GetITextRange(0, 0);

            if (range != null)//it can happen
            {
                range.End = range.StoryLength;
                FindTables(range.Text);
                FindLists(range);
                GetParagraphs(range);
            }
            _rtfParas.Sort();
            _rtfLists.Sort();
            _rtfTables.Sort();
            return true;
        }

        #region State gathering methods

        #region List handling routines

        void FindLists(ITextRange tr)
        {
            int i = 0;
            ListElement CurrentList = null;
            
            //move the range to the start of the document
            tr.Start = 0;
            tr.End = 0;

            tr.Expand(TomUnit.tomParagraph);
            do
            {
                if (tr.Para.ListType != TomListType.tomListNone)
                {
                    tr.Expand(TomUnit.tomParagraph);
                    CurrentList = (ListElement)_rtfLists.DefineEntry(TypoAttrib.list);
                    i = HandleList(tr, CurrentList, tr.Para.ListLevelIndex);
                }
                else
                    i = tr.Move(TomUnit.tomParagraph, 1);
            }
            while (i > 0);
        }

        /************************************************
        * Function: HandleList
        *
        * Comments: 
        *  Assumption: Range unit is TomParagraph. 
        *  Current index is first para of list to be looked at.
        *  ie:
        *  1. Line One. <- Range will be here, List element will be created and ready to recieve
        *  2. Line Two.     ListItems. HandleList Assigns CP start and end values.
        *  3. Line Three.      
        **************************************************/
        int HandleList(ITextRange tr, ListElement CurrentList, long CurrentListLevel)
        {
            ListItemElement CurrentListItem = null;
            int i = 0;
            bool fSkipMove = false;

            //based on first line of list
            //gather attribs of this list.
            CurrentList._fListTab = tr.Para.ListTab;
            CurrentList._ListType = tr.Para.ListType & TomListType.tomListTypeMask;
            CurrentList._lListLevel = tr.Para.ListLevelIndex;
            CurrentList._lListStart = tr.Para.ListStart;
            CurrentList._szText = tr.Text;

            //add an item for the para that spawned
            //this call to HandleList.
            CurrentListItem = CurrentList.DefineElement();
            CurrentListItem._szText = tr.Text;
            CurrentList._lstListItems.Add(CurrentListItem);

            //Collect listItem paragraphs
            do
            {
                if (!fSkipMove)
                {
                    //attempt to move to the next paragraph.
                    i = tr.Move(TomUnit.tomParagraph, 1);
                    tr.Expand(TomUnit.tomParagraph);
                }
                else
                {
                    fSkipMove = false;
                }

                //Determine if this new paragraph is part of the list, 
                //is a different list, or not a list at all.
                if (((tr.Para.ListType & TomListType.tomListTypeMask) == CurrentList._ListType) &&
                    (tr.Para.ListLevelIndex == CurrentList._lListLevel))
                {
                    //just another item, gather properties and move on.
                    CurrentListItem = CurrentList.DefineElement();
                    CurrentListItem._szText = tr.Text;
                    CurrentList._lstListItems.Add(CurrentListItem);
                }
                else if (tr.Para.ListLevelIndex > CurrentList._lListLevel)
                {
                    //The new paragraph is part of a sublist.
                    ListElement NewList = CurrentListItem.DefineElement();
                    i = HandleList(tr, NewList, tr.Para.ListLevelIndex);
                    CurrentListItem._lstSubList.Add(NewList);
                    fSkipMove = true;
                }
                else
                {
                    //Since it's not a sub list and it's not a list item, 
                    //we are done handling this list.
                    return i;
                }
            } while (i == 1);
            return i;
        }
        #endregion

        //[stubbed for now]Hyperlinks are going to be more dificult on the richedit side.
        //The new Tom interfaces have some methods which may be helpfull
        //but for now I'm going to leave hyperlinks off of this code pass.      
        void FindHyperLinks(ITextRange tr)
        {
        }

        #region Paragraph handling routines
        void GetParagraphs(ITextRange tr)
        {
            int i = 0;
            int ctrs = 0;// table row start counter
            bool bEndReached = false;
            ParagraphElement CurrentPara = null;

            tr.Start = 0;
            tr.End = 0;

            do
            {
                tr.Expand(TomUnit.tomParagraph);
                if (tr.End == tr.StoryLength)
                {
                    bEndReached = true;
                }
                if (!isTablePara(tr.Text, ref ctrs))
                {
                    CurrentPara = (ParagraphElement)_rtfParas.DefineEntry(TypoAttrib.paragraph);
                    
                    // RichEdit does this really annoying thing were it puts \v into a seperate para
                    // even though it really isnt a seperate para.
                    // To handle this we look to see if the last character in the current range is a \v,
                    // and if it is we move the range's end to get the whole "real" Paragraph.
                    string strPara = tr.Text;
                    while (strPara.EndsWith("\v") && tr.End < tr.StoryLength)
                    {
                        tr.MoveEnd(TomUnit.tomParagraph, 1);
                        strPara = tr.Text;
                    }

                    //gather paragraph specific properties
                    CurrentPara._taTextAlignment = tr.Para.Alignment;
                    CurrentPara._dFirstLineIndent = Math.Round(tr.Para.FirstLineIndent, 2);

                    //Not compared as yet, 
                    //but might be added in later.
                    //CurrentPara._dLineSpacing = Math.Round(tr.Para.LineSpacing);
                    //CurrentPara._dLeftIndent = Math.Round(tr.Para.LeftIndent);
                    //CurrentPara._dRightIndent = Math.Round(tr.Para.RightIndent);
                    //CurrentPara._dSpaceAfter = Math.Round(tr.Para.SpaceAfter);
                    //CurrentPara._dSpaceBefore = Math.Round(tr.Para.SpaceBefore);

                    CurrentPara._szText = CleanseParaTextAndGetFontInfo(CurrentPara, tr, ref ctrs);//remove hidden text, etcetra
                }
                i = tr.Move(TomUnit.tomParagraph, 1);//on to the next paragraph
            }
            while ((i > 0) && (!bEndReached));
        }

        bool isTablePara(string sz, ref int ctrs)
        {
            if (sz.Length > 2) return false;
            if ((uint)sz[0] == 0xFFF9)
            {
                ctrs++;//deleted a TableRow Start
                return true;
            }
            if ((uint)sz[0] == 0xfffb)
            {
                ctrs--;//deleted a TableRow End
                return true;
            }
            return false;
        }

        //remove hidden text and anything else that becomes bothersome\
        string CleanseParaTextAndGetFontInfo(ParagraphElement para, ITextRange tr, ref int ctrs)
        {
            StringBuilder szReturn = new StringBuilder();
            ITextRange tr2 = tr.GetDuplicate();
            tr2.End = tr.Start;
            ITextRange tr3 = null;
            int i = 0;
            int SpanCpS = 0;
            int SpanCpE = 0;
            int iHiddenCorrection = 0;
            int cpCurrentStart = tr.Start;
            int cpEnd = tr.End;
            bool fDone = false;

            while (!fDone)
            {
                tr2.Expand(TomUnit.tomCharFormat);
                #region Handle Expand Modifications
                
                // Expand can move the start closer to the begining, so i just 
                // track and automaticly assign it.
                
                tr2.Start = cpCurrentStart;
                
                //Expanding by tomCharFormat can se tthe start and end of the
                //range beyond the bounds of the paragraph whose properties we are after.
                //In order to log span properties relative to the current paragraph
                //the code below creates other ranges to ensure we don't record anything 
                //out of tr's bounds.
                
                if (tr2.End > cpEnd)
                {
                    tr2.End = cpEnd;
                    fDone = true;
                }
                else if (tr2.End == cpEnd)
                {
                    fDone = true;
                }
                else
                {
                    cpCurrentStart = tr2.End;
                }
                tr3 = tr2;
                #endregion

                if ((tr3.Font.Hidden == TomBool.tomFalse))
                {                    
                    //first setup the span cp's to be relative to the para
                    SpanCpS = tr3.Start - (tr.Start + iHiddenCorrection);
                    SpanCpE = tr3.End - (tr.Start + iHiddenCorrection);
                    szReturn.Append(tr3.Text);
                    #region Capture Span level formatting

                    for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
                    {
                        SpanElement se = para.DefineElement(tp, SpanCpS, SpanCpE);
                        switch (tp)
                        {
                            case FormatType.FontName:
                                {
                                    se._szFontName = tr3.Font.Name;
                                    para._lstFontName.Add(se);
                                    break;
                                }
                            case FormatType.FontSize:
                                {
                                    se._dFontSize = tr3.Font.Size;
                                    para._lstFontSize.Add(se);
                                    break;
                                }
                            case FormatType.ForeGround:
                                {
                                    se._lForegroundColor = tr3.Font.ForeColor;
                                    if(se._lForegroundColor == -9999997)
                                    {
                                        //-9999997 is tomAutoColor. 
                                        //As Far as I can tell, the XAML
                                        //equivalent is 0.
                                        se._lForegroundColor = 0;
                                    }
                                    para._lstForeGround.Add(se);
                                    break;
                                }
                            case FormatType.Italic:
                                {
                                    se._tbItalic = tr3.Font.Italic;
                                    para._lstItalic.Add(se);
                                    break;
                                }
                            case FormatType.Bold:
                                {
                                    se._tbBold = tr3.Font.Bold;
                                    para._lstBold.Add(se);
                                    break;
                                }
                            case FormatType.Kerning:
                                {
                                    se._fKerning = (tr3.Font.Kerning != 0) ? true : false;
                                    para._lstKerning.Add(se);
                                    break;
                                }
                            case FormatType.Language:
                                {                                    
                                    //This value, the Richedit gives for the langID is not
                                    //neccessarily going to be the same as what Avalon uses for 
                                    //langID. Avalon converter uses lang rtf tag, Richedit uses 
                                    //semi-sophisiticated heuristics based on charactor and 
                                    //charset andother info.
                                    
                                    se._lLanguageID = tr3.Font.LanguageID;
                                    para._lstLang.Add(se);
                                    break;
                                }
                            case FormatType.SubScript:
                                {
                                    se._tbSubScript = tr3.Font.Subscript;
                                    para._lstSubScript.Add(se);
                                    break;
                                }
                            case FormatType.SuperScript:
                                {
                                    se._tbSuperScript = tr3.Font.Superscript;
                                    para._lstSuperScript.Add(se);
                                    break;
                                }
                            case FormatType.Underline:
                                {
                                    if (tr3.Font.Underline != TomUnderline.tomNone)
                                    {
                                        se._tbUnderline = TomBool.tomTrue;
                                    }
                                    para._lstUnderline.Add(se);
                                    break;
                                }
                            //not doing this one yet
                            //case StrikeThrough
                        }
                    }
                    #endregion
                }
                else
                {
                    //add in hidden correction value                    
                    iHiddenCorrection += (tr3.End - tr3.Start);
                }
                i = tr3.Move(TomUnit.tomCharFormat, 1);
                tr2 = tr3;
            }
            para.CompactSpanList();
            return IsKnownTextIssue(ref szReturn, ref ctrs);
        }

        //check for issues that have been resolved WF or BD.
        //Important: This function can change the text arrays and index!
        //It has to do this to be able to "fix up" issues that
        //the converter causes.
        private string IsKnownTextIssue(ref StringBuilder sz, ref int ctrs)
        {
            int index = 0;
            while (index < sz.Length)
            {
                switch ((int)sz[index])
                {
                    //ole place holder char
                    case 0xFFFC:
                        {
                            sz.Remove(index, 1);
                            index--;
                            break;
                        }
                    //should never get to these two!
                    case 0xfffb:
                    case 0xFFF9:
                        {
                            sz.Remove(index, 1);
                            index--;
                            break;
                        }
                }
                index++;
            }
            return sz.ToString();
        }
        #endregion

        #region table handling routines
        //table compare between rtf and xaml is not practicle. Specificly,
        // xaml handles nested tables by simply nesting table tags. Rtf, on the other hand,
        // handles nested tables by considering the nested table as merely rows within rows.
        // so, in the case of rtf -> comparison, for now, I'm going to not compare tables.
        void FindTables(string szText)
        {
            char[] c = szText.ToCharArray();
            bool bInTable = false;
            TableElement CurrentTable = null;
            for (int i = 0; i < szText.Length; i++)
            {
                if (c[i] == 0xfff9)//tablerow start
                {
                    if (!bInTable)
                    {
                        CurrentTable = (TableElement)_rtfTables.DefineEntry(TypoAttrib.table);
                        bInTable = true;
                    }
                    
                    //move index to next char and scan for end of row/new rows.
                    
                    i++;
                    HandleRow(szText, ref i, ref CurrentTable);
                    
                    //Index pointer should now be pointing at 0xfffb
                    //check to see if this is the end of this table or just end of a row
                    
                    if (c[i + 2] != 0xfff9)
                    {
                        //then Table is done.
                        //close this table typo element and 
                        //continue hunting for other tables
                        bInTable = false;
                        CurrentTable = null;
                    }//if (c[i + 2] != 0xfff9)
                }//if (c[i] == 0xfff9)
            }//for (int i = 0; i < szText.Length; i++)
        }


        //makes scanning for tables more state like.
        //handles adding cells and potentially new rows
        void HandleRow(string szText, ref int Index, ref TableElement thisTable)
        {
            char[] c = szText.ToCharArray();
            TableRowElement CurrentRow = thisTable.DefineElement();
            thisTable._lstTableRows.Add(CurrentRow);
            for (; Index < szText.Length; Index++)
            {
                if (c[Index] == 0xfff9)//tablerow start
                {
                    //Move index one char past 0xfff9
                    Index++;
                    HandleRow(szText, ref Index, ref thisTable);
                    Index++;//move past 0xfffb
                }
                else if (c[Index] == 0x0007)
                {
                    //since, currently I am just using the TableCellElements for 
                    //counting cells, I don't need to store it, just add and 
                    //move on.
                    CurrentRow.DefineElement();
                }
                else if (c[Index] == 0xfffb)
                {
                    //this row is done, bail out of for-loop and return.
                    break;
                }
            }
            if (Index >= szText.Length)
            {
                Exception TableRowException = new Exception("Reached end of Text before end of table, while gathering RE state.");
                throw TableRowException;
            }
            return;
        }
        #endregion//table handling routines

        //Get an ITextRange pointer from RichEdit via MSAA (WM_GETOBJECT)
        ITextRange GetITextRange(int cpMin, int cpMax)
        {
            if (_document == null)
            {
                IntPtr hwndRE = _rtb.Handle;
                Guid IID_IDispatch = new Guid("00020400-0000-0000-C000-000000000046");
                Object obj = IntPtr.Zero;
                const int OBJID_NATIVEOM = -16; //0xFFFFFFF0
                int hResult = AccessibleObjectFromWindow(hwndRE, OBJID_NATIVEOM, ref IID_IDispatch, ref obj);
                _document = obj as ITextDocument;
            }
            ITextRange range = _document.Range(cpMin, cpMax);
            return range;
        }
        #endregion

        #region class fields
        
        // Get TOM via RichEdit MSAA proxy
        [DllImport("oleacc.dll")]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, int id, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);

        private ITextDocument _document; // TOM ITextDocument
        private System.Windows.Forms.RichTextBox _rtb;
        public TypoInfo _rtfParas;
        public TypoInfo _rtfLists;
        public TypoInfo _rtfTables;
        #endregion
    }

    #endregion //state comparison helpers

    #region Error Logging Helpers
    /// <summary>
    /// 
    /// </summary>
    public class ErrorLog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fLogOnlyPri"></param>
        /// <param name="szLogFilePath"></param>
        /// <param name="szSumary"></param>
        public ErrorLog(bool fLogOnlyPri, string szLogFilePath, string szSumary)
        {
            if (File.Exists(szLogFilePath))
            {
                File.Delete(szLogFilePath);
            }
            _lstErrors = new ArrayList();
            _szCurrentFile = null;
            _szLogFilePath = szLogFilePath;
            _fLogOnlyPri = fLogOnlyPri;
            if (szSumary != null)
                _szSumaryPath = szSumary;
            else
                _szSumaryPath = null;

        }

        //small internal class to hold individual error info
        //Implemnet IComparable to use ArrayList's sort method        
        class Error : IComparable
        {
            public Error(ConverterErrorType ErrorType, int priority, string szError)
            {
                _Type = ErrorType;
                _pri = priority;
                _szError = szError;
            }

            public override string ToString()
            {
                return "pri" + _pri.ToString() + " :" + _szError;
            }

            //IComparable's only method
            public int CompareTo(object obj)
            {
                if (obj is Error)
                {
                    Error target = (Error)obj;
                    if (_pri == target._pri)
                    {
                        return _szError.CompareTo(target._szError);
                    }
                    else
                        return _pri - target._pri;
                }

                throw new ArgumentException("Object is not an Error Object.");
            }

            public ConverterErrorType _Type;
            public int _pri;
            public string _szError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szFileName"></param>
        public void BeginCase(string szFileName)
        {
            _szCurrentFile = szFileName;
            _dtDocStart = DateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorType"></param>
        /// <param name="priority"></param>
        /// <param name="szError"></param>
        public void LogError(ConverterErrorType ErrorType, int priority, string szError)
        {
            if (_szCurrentFile != null)
            {
                Error er = new Error(ErrorType, priority, szError);
                _lstErrors.Add(er);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndCase()
        {
            string szFolder = Path.GetDirectoryName(_szCurrentFile);
            int i = 0;
            bool fLog = false;
            bool fLoggedAtLeastOne = false;
            uint cErrors = 0;
            _dtDocEnd = DateTime.Now;
            
            //the error class implements a compare based on the errors priority
            //and the array list's sort method uses that to sort.
            
            _lstErrors.Sort();
            
            //if selected, log only pri 0 errors
            
            while (i < _lstErrors.Count)
            {

                if (_fLogOnlyPri && (((Error)_lstErrors[i])._pri <= 1))
                {
                    fLog = true;
                }
                else if (!_fLogOnlyPri)
                {
                    fLog = true;
                }
                else
                {
                    fLog = false;
                }
                if (fLog)
                {

                    LogString(_szCurrentFile);
                    LogString(",");
                    Error er = (Error)_lstErrors[i];
                    LogLine(er.ToString());
                    Flush();
                    fLoggedAtLeastOne = true;
                    cErrors++;
                }
                i++;
            }
            //if no errors, log success
            if ((_lstErrors.Count == 0) || (!fLoggedAtLeastOne))
            {
                LogString(_szCurrentFile);
                LogString(",");
                LogLine("Pass");
                Flush();
            }
            _lstErrors.Clear();
            
            //now update our Sumary file if there is one
            
            if (_szSumaryPath != null)
            {
                TimeSpan dtDoc = _dtDocEnd - _dtDocStart;
                _szSumaryCache += _szCurrentFile + ", Fails: " + cErrors.ToString() +
                   ", Seconds: " + dtDoc.TotalSeconds.ToString() + (char)0xD + (char)0xA;
            }
            _szCurrentFile = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EndRun()
        {
            StreamWriter stream;
            if (_szSumaryPath != null)
            {
                lock (this)
                {
                    if (_szSumaryCache != "")
                    {
                        /* Open for "append" */
                        try { stream = new StreamWriter(_szSumaryPath, true); }
                        catch
                        {
                            return;
                        };

                        stream.Write(this._szSumaryCache);
                        stream.Close();
                    };

                    _strCache = "";
                }
            }
        }

        // Log new line
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMessage"></param>
        public void LogLine(string strMessage)
        {
            _strCache += strMessage + (char)0xD + (char)0xA;
        }

        // Log empty line
        /// <summary></summary>
        public void LogLine()
        {
            LogLine("");
        }

        /// <summary></summary>
        public void LogString(string strMessage)
        {
            _strCache += strMessage;
        }

        // Flush caches string
        void Flush()
        {
            StreamWriter stream;

            lock (this)
            {
                if (_strCache != "")
                {
                    /* Open for "append" */
                    try
                    {
                        stream = new StreamWriter(_szLogFilePath, true);
                        stream.Write(this._strCache);
                        stream.Close();
                    }
                    catch
                    {
                        //During Close there is a problem if invalide surrogate text
                        //is found, which causes an exception in mscorlib.dll. 
                        //Problem occurs in stream.Close, but other misc exceptions could occur here too.
                        stream = new StreamWriter(_szLogFilePath, true);
                        stream.Write(_szCurrentFile + ",Pri0: There was a problem writting out text of this files fails.\r\n");
                        stream.Close();
                    };
                };
                _strCache = "";
            }
        }

        ArrayList _lstErrors;
        string _szCurrentFile;
        string _strCache;
        string _szLogFilePath;
        string _szSumaryCache;
        string _szSumaryPath;
        DateTime _dtDocStart;
        DateTime _dtDocEnd;
        bool _fLogOnlyPri;
    }
    #endregion

    #region automation test helper class
    /// <summary>
    /// xcvtaut handles coordinating gathering state info for available controls, xaml or richedit for now
    /// and has the highlevel compare methods.
    /// </summary>    
    public class xcvtaut
    {
       /// <summary>
        /// 
        /// </summary>
        /// <param name="rxv"></param>
        /// <param name="elErrors"></param>
        public xcvtaut(RtfXamlViewApp rxv, ref ErrorLog elErrors)
        {
            _rxv = rxv;
            _elLog = elErrors;
        }

        #region Comparison Tests

        #region Old CompareRtfRoundtrip
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="swLog"></param>
        /// <returns></returns>
        public bool CompareRtfRoundtrip(string filename, ref StreamWriter swLog)
        {
            string errstring = "";
            bool fPass = true;
            ConverterError err = _rxv.LoadFile(filename);
            if (err.errortype == ConverterErrorType.ErrorNone)
            {
                RichEditStateCompare resc = new RichEditStateCompare();
                resc.SetRichTextBox(_rxv.GetRETextBox());
                resc.SetPreState();
                err = _rxv.ConvertXamlToRtf();
                resc.SetRichTextBox(_rxv.GetRETextBox());
                if (err.errortype == ConverterErrorType.ErrorNone)
                {
                    resc.SetPostState();
                    fPass = resc.Compare(ref errstring);
                }
            }

            if (err.errortype != ConverterErrorType.ErrorNone)
            {
                errstring = err.errortype.ToString();
                switch (err.errortype)
                {
                    case ConverterErrorType.ErrorException:
                        errstring += ", " + err.ExceptionText;
                        break;
                }
                fPass = false;
            }

            string bvtlog;
            if (!fPass)
                bvtlog = "FAIL:\t" + filename + ", " + errstring;
            else
                bvtlog = "PASS:\t" + filename;
            swLog.WriteLine(bvtlog);

            return fPass;
        }
        #endregion

        //This tests the converter through RichEdit only.
        //It should not be assumed that all RTF will round trip this way.
        //This test is primarily to check for large inconsistancies
        //and is not a true user scenereo.
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CompareXamlRoundtrip()
        {
            ConverterError err;
            int iDocErrorCount = 0;

            XamlStateCompare xaml1 = new XamlStateCompare(_rxv.GetXamlTextBox());
            xaml1.SetState();
            err = _rxv.SendXamlContentToRECtrl();
            err = _rxv.SendREContentToXamlCtrl();
            if (err.errortype == ConverterErrorType.ErrorNone)
            {
                XamlStateCompare xaml2 = new XamlStateCompare(_rxv.GetXamlTextBox());
                xaml2.SetState();
                
                //changed to compare test specific compares 
                //for better clarity when writing fixups and debugging.         
                iDocErrorCount = CompareXamlToXaml(xaml1, xaml2);
            }
            return iDocErrorCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int CompareRtfToXaml()
        {
            XamlStateCompare xaml = new XamlStateCompare(_rxv.GetXamlTextBox());
            
            //In case there was no comparision possible,
            //ie the avalon control had nothing in it, just log it 
            //and return.            
            if (CompareMsg.CompareContinue != xaml.SetState())
            {
                _elLog.LogError(ConverterErrorType.ErrorXamlConversion, 0,
                                "Avalon control not populated for unknown reason.");
                return 1;
            }
            NewREStateCompare RE = new NewREStateCompare();
            RE.SetRichTextBox(_rxv.GetRETextBox());
            if (RE.SetState() == true)
            {
                return CompareRtfToXaml(xaml, RE);
            }
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int NewCompareRtfRoundTrip()
        {
            ConverterError err;
            int iDocErrorCount = 0;

            NewREStateCompare RE = new NewREStateCompare();
            RE.SetRichTextBox(_rxv.GetRETextBox());
            if (RE.SetState() == true)
            {
                err = _rxv.SendXamlContentToRECtrl();
                if (err.errortype == ConverterErrorType.ErrorNone)
                {
                    NewREStateCompare RE2 = new NewREStateCompare();
                    RE2.SetRichTextBox(_rxv.GetRETextBox());
                    if (RE2.SetState() == true)
                    {                        
                        //changed to compare test specific compares 
                        //for better clarity when writing fixups and debugging.
                        //For now this test is NYI.                        
                        //iDocErrorCount = Compare(RE._RichedState, RE2._RichedState, ref _elLog);
                    }
                    else
                    {
                        _elLog.LogError(ConverterErrorType.ErrorException, 0, "Error setting state info after Conversion.");
                        iDocErrorCount++;
                    }
                }
            }
            else
            {
                _elLog.LogError(ConverterErrorType.ErrorException, 0, "Error setting state info before Conversion.");
            }

            return iDocErrorCount;
        }

        #endregion

        #region Comparison methods

        #region Xaml Specifc prop compares

        // Series of overloaded methods that compare Xaml specific props (xaml to xaml only).
        int CompareXamlProperties(string szElementType,
                                    FlowDirection Before, FlowDirection After, int iID)
        {
            if (Before != After)
            {
                LogXamlPropFail(iID, szElementType, "FlowDirection", Before.ToString(), After.ToString());
                return 1;
            }
            return 0;
        }

        int CompareXamlProperties(string szElementType, XamlProp tp,
                                    Thickness Before, Thickness After, int iID)
        {
            string szType;
            if (Before != After)
            {
                switch (tp)
                {
                    case XamlProp.Border:
                        {
                            szType = "BorderThickness";
                            break;
                        }
                    case XamlProp.Margin:
                        {
                            szType = "Margin";
                            break;
                        }
                    case XamlProp.Padding:
                        {
                            szType = "Padding";
                            break;
                        }
                    default:
                        return 0;
                }
                LogXamlPropFail(iID, szElementType, szType, Before.ToString(), After.ToString());
                return 1;
            }
            return 0;
        }

        int CompareXamlProperties(string szElementType, XamlProp tp,
                                    Color Before, Color After, int iID)
        {
            string szType;
            if (Before != After)
            {
                switch (tp)
                {
                    case XamlProp.BorderColor:
                        {
                            szType = "BorderColor";
                            break;
                        }
                    case XamlProp.Foreground:
                        {
                            szType = "Foreground";
                            break;
                        }
                    case XamlProp.Background:
                        {
                            szType = "Background";
                            break;
                        }
                    default:
                        return 0;
                }
                LogXamlPropFail(iID, szElementType, szType, Before.ToString(), After.ToString());
                return 1;
            }
            return 0;
        }

        //simply log the specific failure
        void LogXamlPropFail(int iID, string szElementType, string szType,
                             string szBefore, string szAfter)
        {
            string szError = szElementType + "#: " + iID.ToString() +
                               szType + " changed. Before: " + szBefore +
                               " After: " + szAfter;
            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
        }
        #endregion

        #region Table Comparision
        #region XamlToRtf
        int CompareXamlToRtfTableRow(TableRowElement Xaml, TableRowElement Rtf)
        {
            int iRet = 0;
            
            //about the only thing we can check is to ensure they 
            //have the same number of cells.
            if (Xaml._lstTableCells.Count != Rtf._lstTableCells.Count)
            {
                int diff = Xaml._lstTableCells.Count - Rtf._lstTableCells.Count;
                if (diff < 0)
                {
                    string szError = "Rtf table: " + Rtf._iParentKey.ToString() +
                       " Row: " + Rtf._iKey.ToString() + " Has " +
                       Math.Abs(diff).ToString() + " more cells than Xaml Table.";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                }
                else
                {
                    string szError = "Xaml table: " + Xaml._iParentKey.ToString() +
                       " Row: " + Xaml._iKey.ToString() + " Has " +
                       diff.ToString() + "more cells than the rtf table.";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                }
                iRet++;
            }
            return iRet;
        }

        // table state gathering has some bugs and I haven't the time to sort 
        // them out. I'm dropping the pri on this to 2.
        int CompareXamlToRtfTable(TypoInfo xamlTables, TypoInfo rtfTables)
        {
            int iXaml = 0;
            int iRtf = 0;
            int iRet = 0;

            while ((iXaml < xamlTables.Count) && (iRtf < rtfTables.Count))
            {
                //if there is a difference bewtween the tables,
                // then Examine things more closely.
                if (((TableElement)xamlTables[iXaml]) != ((TableElement)rtfTables[iRtf]))
                {
                    #region Row Comparision
                    for (int i = 0; ((i < ((TableElement)xamlTables[iXaml])._lstTableRows.Count) &&
                        (i < ((TableElement)rtfTables[iRtf])._lstTableRows.Count)); i++)
                    {
                        if (((TableRowElement)((TableElement)xamlTables[iXaml])._lstTableRows[i]) !=
                            ((TableRowElement)((TableElement)rtfTables[iRtf])._lstTableRows[i]))
                        {
                            iRet += CompareXamlToRtfTableRow((TableRowElement)((TableElement)xamlTables[iXaml])._lstTableRows[i],
                                     (TableRowElement)((TableElement)rtfTables[iRtf])._lstTableRows[i]);
                        }
                    }
                    #endregion

                    #region Compare Row Counts
                    if (((TableElement)xamlTables[iXaml])._lstTableRows.Count !=
                        ((TableElement)rtfTables[iRtf])._lstTableRows.Count)
                    {
                        int diff = ((TableElement)xamlTables[iXaml])._lstTableRows.Count -
                            ((TableElement)rtfTables[iRtf])._lstTableRows.Count;
                        if (diff < 0)
                        {
                            string szError = "Table: " + ((TableElement)rtfTables[iRtf])._iKey.ToString() +
                               " has " + Math.Abs(diff).ToString() + " more rows after conversion.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        }
                        else
                        {
                            string szError = "Table: " + ((TableElement)xamlTables[iXaml])._iKey.ToString() +
                               " has " + diff.ToString() + " less rows after conversion.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        }
                        iRet++;
                    }
                    #endregion
                }
                iXaml++;
                iRtf++;
            }
            return iRet;
        }
        #endregion
        #region XamlToXaml
        int CompareXamlToXamlTable(TypoInfo Before, TypoInfo After)
        {
            int iBefore = 0;
            int iAfter = 0;
            int iRet = 0;

            while ((iBefore < Before.Count) && (iAfter < After.Count))
            {
                //if there is a difference bewtween the tables,
                // then examine things more closely.
                #region
                if (((TableElement)Before[iBefore]) != ((TableElement)After[iAfter]))
                {
                    #region Table
                    CompareXamlProperties("Table", ((TypoElement)Before[iBefore])._fd,
                        ((TypoElement)After[iAfter])._fd, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.Border, ((TypoElement)Before[iBefore])._rcBorderThickness,
                        ((TypoElement)After[iAfter])._rcBorderThickness, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.Margin, ((TypoElement)Before[iBefore])._rcMargin,
                        ((TypoElement)After[iAfter])._rcMargin, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.Padding, ((TypoElement)Before[iBefore])._rcPadding,
                        ((TypoElement)After[iAfter])._rcPadding, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.BorderColor, ((TypoElement)Before[iBefore])._cBorderColor,
                        ((TypoElement)After[iAfter])._cBorderColor, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.Background, ((TypoElement)Before[iBefore])._cBackground,
                        ((TypoElement)After[iAfter])._cBackground, ((TableElement)Before[iBefore])._iKey);
                    CompareXamlProperties("Table", XamlProp.Foreground, ((TypoElement)Before[iBefore])._cForeground,
                        ((TypoElement)After[iAfter])._cForeground, ((TableElement)Before[iBefore])._iKey);
                    #endregion
                    #region TableRows
                    for (int i = 0; ((i < ((TableElement)Before[iBefore])._lstTableRows.Count) &&
                        (i < ((TableElement)After[iAfter])._lstTableRows.Count)); i++)
                    {
                        if (((TableRowElement)((TableElement)Before[iBefore])._lstTableRows[i]) !=
                            ((TableRowElement)((TableElement)After[iAfter])._lstTableRows[i]))
                        {
                            iRet += CompareXamlToXamlTableRow((TableRowElement)((TableElement)Before[iBefore])._lstTableRows[i],
                                     (TableRowElement)((TableElement)After[iAfter])._lstTableRows[i]);
                        }
                    }
                    #region Note differencs in row count
                    if (((TableElement)Before[iBefore])._lstTableRows.Count !=
                        ((TableElement)After[iAfter])._lstTableRows.Count)
                    {
                        int diff = ((TableElement)Before[iBefore])._lstTableRows.Count -
                            ((TableElement)After[iAfter])._lstTableRows.Count;
                        if (diff < 0)
                        {
                            string szError = "Table: " + ((TableElement)After[iAfter])._iKey.ToString() +
                               " has " + Math.Abs(diff).ToString() + " more rows after conversion.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                        }
                        else
                        {
                            string szError = "Table: " + ((TableElement)Before[iBefore])._iKey.ToString() +
                               " has " + diff.ToString() + " less rows after conversion.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                        }
                        iRet++;
                    }
                    #endregion
                    #endregion
                }
                iBefore++;
                iAfter++;
                #endregion
            }
            return iRet;
        }
        int CompareXamlToXamlTableRow(TableRowElement Before, TableRowElement After)
        {
            int iRet = 0;
            #region row properties
            CompareXamlProperties("TableRow", ((TypoElement)Before)._fd,
                ((TypoElement)After)._fd, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.Border, ((TypoElement)Before)._rcBorderThickness,
                ((TypoElement)After)._rcBorderThickness, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.Margin, ((TypoElement)Before)._rcMargin,
                ((TypoElement)After)._rcMargin, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.Padding, ((TypoElement)Before)._rcPadding,
                ((TypoElement)After)._rcPadding, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.BorderColor, ((TypoElement)Before)._cBorderColor,
                ((TypoElement)After)._cBorderColor, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.Background, ((TypoElement)Before)._cBackground,
                ((TypoElement)After)._cBackground, ((TableRowElement)Before)._iKey);
            CompareXamlProperties("TableRow", XamlProp.Foreground, ((TypoElement)Before)._cForeground,
                ((TypoElement)After)._cForeground, ((TableRowElement)Before)._iKey);
            #endregion
            #region Cell Count
            if (Before._lstTableCells.Count != After._lstTableCells.Count)
            {
                int diff = Before._lstTableCells.Count - After._lstTableCells.Count;
                if (diff < 0)
                {
                    string szError = "Converted table: " + After._iParentKey.ToString() +
                       " Row: " + After._iKey.ToString() + " Has " +
                       Math.Abs(diff).ToString() + " more cells than original.";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                }
                else
                {
                    string szError = "Original table: " + Before._iParentKey.ToString() +
                       " Row: " + Before._iKey.ToString() + " Has " +
                       diff.ToString() + "more cells than the converted table.";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                }
                iRet++;
            }
            #endregion
            return iRet;
        }
        
        int CompareXamlToXamlTableCell(TableRowElement Before, TableRowElement After)
        {
            return 0;
        }

        #endregion
        #endregion

        #region List Comparison
        #region Xaml To Rtf

        //Many false false do to differences in RE and Avalon.
        // Requires fixups in text compare.
        int XamlToRtfCompareList(TypoInfo xamlLists, TypoInfo rtfLists)
        {
            int iXaml = 0;
            int iRtf = 0;
            int iRet = 0;
            while ((iXaml < xamlLists.Count) && (iRtf < rtfLists.Count))
            {
                ListElement xamlList = (ListElement)xamlLists[iXaml];
                ListElement rtfList = (ListElement)rtfLists[iRtf];
                if (xamlList != rtfList)
                {
                    //First compare overall list properties
                    #region
                    if (xamlList._ListType != rtfList._ListType)
                    {
                        string szError = "List: " + xamlList._iKey.ToString() + " List type Before: " +
                           TomHelpers.TypeToString(xamlList._ListType) +
                           " After: " + TomHelpers.TypeToString(rtfList._ListType);
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        iRet++;
                    }

                    if (xamlList._lListLevel != rtfList._lListLevel)
                    {
                        string szError = "List: " + xamlList._iKey.ToString() + " List level before: " +
                           xamlList._lListLevel.ToString() + " After: " +
                           rtfList._lListLevel.ToString();
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        iRet++;
                    }

                    if (xamlList._lListStart != rtfList._lListStart)
                    {
                        if ((xamlList._fXAMLState && (xamlList._lListStart == 1 && rtfList._lListStart == 0)) ||
                            (rtfList._fXAMLState && (rtfList._lListStart == 1 && xamlList._lListStart == 0)))
                        {
                            string szError = "List: " + xamlList._iKey.ToString() + " List Start Before: " +
                               xamlList._lListStart.ToString() + " After: " +
                               rtfList._lListStart.ToString() +
                               " Regression_Bug350 Avalon doesn't let the start value of a list go less than 1";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                        }
                        else
                        {
                            string szError = "List: " + xamlList._iKey.ToString() + " List Start Before: " +
                               xamlList._lListStart.ToString() + " After: " +
                               rtfList._lListStart.ToString();
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        }
                        iRet++;
                    }

                    #endregion
                    
                    //Compare SubLists if any
                    #region
                    if (xamlList._lstListItems.Count != rtfList._lstListItems.Count)
                    {

                        for (int i = 0; ((i < xamlList._lstListItems.Count) && (i < rtfList._lstListItems.Count)); i++)
                        {
                            iRet += XamlToRtfCompareListItem((ListItemElement)xamlList._lstListItems[i], (ListItemElement)rtfList._lstListItems[i]);
                        }
                        int diff = xamlList._lstListItems.Count - rtfList._lstListItems.Count;
                        if (diff < 0)
                        {
                            string szError = "Xaml List: " + xamlList._iKey.ToString() +
                               " Has " + Math.Abs(diff).ToString() + " more list items than the rtf list.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        }
                        else
                        {
                            string szError = "Xaml List: " + xamlList._iKey.ToString() +
                               " Has " + diff.ToString() + " less list items than the rtf list.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                        }
                        iRet++;
                    }
                    #endregion
                } //if (xamlList != rtfList)
                iXaml++;
                iRtf++;
            }//while
            return iRet;
        }

        //This just coordinates Comparing any sublists the item may have.
        int XamlToRtfCompareListItem(ListItemElement xamlListItem, ListItemElement rtfListItem)
        {
            int iRet = 0;
            for (int i = 0; ((i < xamlListItem._lstSubList.Count) &&
                (i < rtfListItem._lstSubList.Count)); i++)
            {
                iRet += XamlToRtfCompareSubList((ListElement)xamlListItem._lstSubList[i],
                   (ListElement)rtfListItem._lstSubList[i]);
            }
            return iRet;
        }

        int XamlToRtfCompareSubList(ListElement xamlSubList, ListElement rtfSubList)
        {
            int iRet = 0;
            if (xamlSubList != rtfSubList)
            {
                //First compare overall list properties
                #region
                if (xamlSubList._ListType != rtfSubList._ListType)
                {
                    string szError = "List: " + xamlSubList._iKey.ToString() + " List type Before: " +
                       TomHelpers.TypeToString(xamlSubList._ListType) +
                       " After: " + TomHelpers.TypeToString(rtfSubList._ListType);
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                    iRet++;
                }

                if (xamlSubList._lListLevel != rtfSubList._lListLevel)
                {
                    string szError = "List: " + xamlSubList._iKey.ToString() + " List level before: " +
                       xamlSubList._lListLevel.ToString() + " After: " +
                       rtfSubList._lListLevel.ToString();
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                    iRet++;
                }

                if (xamlSubList._lListStart != rtfSubList._lListStart)
                {
                    if ((xamlSubList._fXAMLState && (xamlSubList._lListStart == 1 && rtfSubList._lListStart == 0)) ||
                        (rtfSubList._fXAMLState && (rtfSubList._lListStart == 1 && xamlSubList._lListStart == 0)))
                    {
                        string szError = "List: " + xamlSubList._iKey.ToString() + " List Start Before: " +
                           xamlSubList._lListStart.ToString() + " After: " +
                           rtfSubList._lListStart.ToString() +
                           " Regression_Bug350 Avalon doesn't let the start value of a list go less than 1";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                    }
                    else
                    {
                        string szError = "List: " + xamlSubList._iKey.ToString() + " List Start Before: " +
                           xamlSubList._lListStart.ToString() + " After: " +
                           rtfSubList._lListStart.ToString();
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                    }
                    iRet++;
                }

                #endregion
                //Compare SubLists if any
                #region
                if (xamlSubList._lstListItems.Count != rtfSubList._lstListItems.Count)
                {

                    for (int i = 0; ((i < xamlSubList._lstListItems.Count) && (i < rtfSubList._lstListItems.Count)); i++)
                    {
                        iRet += XamlToRtfCompareListItem((ListItemElement)xamlSubList._lstListItems[i],
                                        (ListItemElement)rtfSubList._lstListItems[i]);
                    }
                    int diff = xamlSubList._lstListItems.Count - rtfSubList._lstListItems.Count;
                    if (diff < 0)
                    {
                        string szError = "Xaml List: " + xamlSubList._iKey.ToString() +
                           " Has " + Math.Abs(diff).ToString() + " more list items than the rtf list.";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                    }
                    else
                    {
                        string szError = "Xaml List: " + xamlSubList._iKey.ToString() +
                           " Has " + diff.ToString() + " less list items than the rtf list.";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                    }
                    iRet++;
                }
                #endregion
            } //if (xamlSubList != rtfSubList)
            return iRet;
        }
        #endregion
        
        #region Xaml to Xaml
        
        int XamlToXamlCompareList(TypoInfo BeforeLists, TypoInfo AfterLists)
        {
            int iBefore = 0;
            int iAfter = 0;
            int iRet = 0;
            while ((iBefore < BeforeLists.Count) && (iAfter < AfterLists.Count))
            {
                ListElement Before = (ListElement)BeforeLists[iBefore];
                ListElement After = (ListElement)AfterLists[iAfter];
                if (Before != After)
                {
                    #region
                    //First compare overall list properties
                    #region
                    if (Before._ListType != After._ListType)
                    {
                        #region
                        string szError = "List: " + Before._iKey.ToString() + " List type Before: " +
                               TomHelpers.TypeToString(Before._ListType) +
                               " After: " + TomHelpers.TypeToString(After._ListType);
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                        iRet++;
                        #endregion
                    }

                    if (Before._lListLevel != After._lListLevel)
                    {
                        #region
                        string szError = "List: " + Before._iKey.ToString() + " List level before: " +
                           Before._lListLevel.ToString() + " After: " +
                           After._lListLevel.ToString();
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                        iRet++;

                        #endregion
                    }

                    if (Before._lListStart != After._lListStart)
                    {
                        #region
                        if ((Before._fXAMLState && (Before._lListStart == 1 && After._lListStart == 0)) ||
                            (After._fXAMLState && (After._lListStart == 1 && Before._lListStart == 0)))
                        {
                            string szError = "List: " + Before._iKey.ToString() + " List Start Before: " +
                               Before._lListStart.ToString() + " After: " +
                               After._lListStart.ToString() +
                               " Regression_Bug350 Avalon doesn't let the start value of a list go less than 1";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                        }
                        else
                        {
                            string szError = "List: " + Before._iKey.ToString() + " List Start Before: " +
                               Before._lListStart.ToString() + " After: " +
                               After._lListStart.ToString();
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                        }
                        iRet++;
                        #endregion
                    }

                    #endregion
                    //Compare SubLists if any
                    #region
                    if (Before._lstListItems.Count != After._lstListItems.Count)
                    {

                        for (int i = 0; ((i < Before._lstListItems.Count) &&
                            (i < After._lstListItems.Count)); i++)
                        {
                            iRet += XamlToXamlCompareListItem((ListItemElement)Before._lstListItems[i],
                                        (ListItemElement)After._lstListItems[i]);
                        }
                        int diff = Before._lstListItems.Count - After._lstListItems.Count;
                        if (diff < 0)
                        {
                            string szError = "Before List: " + Before._iKey.ToString() +
                               " Has " + Math.Abs(diff).ToString() + " more list items than After list.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                        }
                        else
                        {
                            string szError = "Before List: " + Before._iKey.ToString() +
                               " Has " + diff.ToString() + " less list items than After list.";
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                        }
                        iRet++;
                    }
                    #endregion
                    #endregion
                } //if (Before != After)
                iBefore++;
                iAfter++;
            }//while
            return iRet;
        }

        // This just coordinates comparing any sublists the item may have.
        int XamlToXamlCompareListItem(ListItemElement BeforeItem, ListItemElement AfterItem)
        {
            int iRet = 0;
            for (int i = 0; ((i < BeforeItem._lstSubList.Count) &&
                (i < AfterItem._lstSubList.Count)); i++)
            {
                iRet += XamlToXamlCompareSubList((ListElement)BeforeItem._lstSubList[i],
                                (ListElement)AfterItem._lstSubList[i]);
            }
            return iRet;
        }

        //Handles the details of comparing sublists, but is essentially the same 
        // as the main compare function
        int XamlToXamlCompareSubList(ListElement Before, ListElement After)
        {
            int iRet = 0;
            if (Before != After)
            {
                //First compare overall list properties
                #region
                if (Before._ListType != After._ListType)
                {
                    #region
                    string szError = "List: " + Before._iKey.ToString() + " List type Before: " +
                                   TomHelpers.TypeToString(Before._ListType) +
                                   " After: " + TomHelpers.TypeToString(After._ListType);
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                    iRet++;
                    #endregion
                }

                if (Before._lListLevel != After._lListLevel)
                {
                    #region
                    string szError = "List: " + Before._iKey.ToString() + " List level before: " +
                                   Before._lListLevel.ToString() + " After: " +
                                   After._lListLevel.ToString();
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                    iRet++;
                    #endregion
                }

                if (Before._lListStart != After._lListStart)
                {
                    #region
                    if ((Before._fXAMLState && (Before._lListStart == 1 && After._lListStart == 0)) ||
                                    (After._fXAMLState && (After._lListStart == 1 && Before._lListStart == 0)))
                    {
                        string szError = "List: " + Before._iKey.ToString() + " List Start Before: " +
                           Before._lListStart.ToString() + " After: " +
                           After._lListStart.ToString() +
                           " Regression_Bug350 Avalon doesn't let the start value of a list go less than 1";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                    }
                    else
                    {
                        string szError = "List: " + Before._iKey.ToString() + " List Start Before: " +
                           Before._lListStart.ToString() + " After: " +
                           After._lListStart.ToString();
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
                    }
                    iRet++;
                    #endregion
                }

                #endregion

                //Compare SubLists if any
                #region
                if (Before._lstListItems.Count != After._lstListItems.Count)
                {

                    #region
                    for (int i = 0; ((i < Before._lstListItems.Count) && (i < After._lstListItems.Count)); i++)
                    {
                        iRet += XamlToXamlCompareListItem((ListItemElement)Before._lstListItems[i],
                                            (ListItemElement)After._lstListItems[i]);
                    }
                    //Report exactly how the coutns differ
                    #region
                    int diff = Before._lstListItems.Count - After._lstListItems.Count;
                    if (diff < 0)
                    {
                        string szError = "Before List: " + Before._iKey.ToString() +
                           " Has " + Math.Abs(diff).ToString() + " more list items than After list.";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                    }
                    else
                    {
                        string szError = "Before List: " + Before._iKey.ToString() +
                           " Has " + diff.ToString() + " less list items than After list.";
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                    }
                    #endregion
                    iRet++;
                    #endregion
                }
                #endregion
            } //if (Before != After)
            return iRet;
        }
        #endregion
        #endregion

        #region paragraph comparison

        #region XamlToXaml Compares
        
        int CompareXamlToXamlParagraphs(TypoInfo BeforeParas, TypoInfo AfterParas)
        {
            int iBefore = 0;
            int iAfter = 0;
            int iRet = 0;
            CompareMsg result = CompareMsg.CompareContinue;

            while ((iBefore < BeforeParas.Count) && (iAfter < AfterParas.Count) &&
               ((result == CompareMsg.CompareContinue) || result == CompareMsg.CompareMsgSkip))
            {
                if ((((TypoElement)BeforeParas[iBefore])._ta == TypoAttrib.paragraph) &&
                   (((TypoElement)AfterParas[iAfter])._ta == TypoAttrib.paragraph))
                {
                    //first compare paragraph strings and log problems with that
                    
                    result = HandleKnownXamlToXamlTextIssues(BeforeParas, AfterParas, ref iBefore, ref iAfter);
                    if (result == CompareMsg.CompareContinue)
                    {
                        //The above function can sometimes modify the state info
                        //to get rid of bogus fails, now we do a carefull char by 
                        //char compare.
                        
                        #region
                        
                        string szTextError = CompareString(((ParagraphElement)BeforeParas[iBefore])._szText,
                                                            ((ParagraphElement)AfterParas[iAfter])._szText,
                                                            ((ParagraphElement)BeforeParas[iBefore])._iKey);
                        if (szTextError != null)
                        {
                            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szTextError);
                            iRet++;
                        }
                        #endregion
                        //Compare Xaml Specific properties
                        #region
                        
                        //Typo object will compare generic xaml props, iff there's a 
                        //difference, look more deeply and determine what difference 
                        //is.
                        if (((TypoElement)BeforeParas[iBefore]) != ((TypoElement)AfterParas[iAfter]))
                        {
                            iRet += CompareXamlProperties("Paragraph", ((TypoElement)BeforeParas[iBefore])._fd,
                                    ((TypoElement)AfterParas[iAfter])._fd,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.Border,
                                    ((TypoElement)BeforeParas[iBefore])._rcBorderThickness,
                                    ((TypoElement)AfterParas[iAfter])._rcBorderThickness,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.Margin,
                                    ((TypoElement)BeforeParas[iBefore])._rcMargin,
                                    ((TypoElement)AfterParas[iAfter])._rcMargin,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.Padding,
                                    ((TypoElement)BeforeParas[iBefore])._rcPadding,
                                    ((TypoElement)AfterParas[iAfter])._rcPadding,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.BorderColor,
                                    ((TypoElement)BeforeParas[iBefore])._cBorderColor,
                                    ((TypoElement)AfterParas[iAfter])._cBorderColor,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.Foreground,
                                    ((TypoElement)BeforeParas[iBefore])._cForeground,
                                    ((TypoElement)AfterParas[iAfter])._cForeground,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                            iRet += CompareXamlProperties("Paragraph", XamlProp.Background,
                                    ((TypoElement)BeforeParas[iBefore])._cBackground,
                                    ((TypoElement)AfterParas[iAfter])._cBackground,
                                    ((ParagraphElement)BeforeParas[iBefore])._iKey);
                        }
                        #endregion
                        
                        //now determine if these two paragraphs have the same 
                        //paragraph properties.
                        #region
                        if ((ParagraphElement)BeforeParas[iBefore] != (ParagraphElement)AfterParas[iAfter])
                        {
                            iRet += logXAMLToXamlParaPropFails((ParagraphElement)BeforeParas[iBefore],
                                     (ParagraphElement)AfterParas[iAfter]);
                        }
                        #endregion
                        //Next Compare format runs
                        #region
                        ParagraphElement paraBefore = (ParagraphElement)BeforeParas[iBefore];
                        ParagraphElement paraAfter = (ParagraphElement)AfterParas[iAfter];
                        iRet += XamlToXamlCompareSpans(ref paraBefore,
                                             ref paraAfter);
                        #endregion
                    }
                    else if (result == CompareMsg.CompareParaSyncAbandon)
                    {
                        _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, "Para elements out of sync, quitting compare.");
                        return iRet;//para's got out of sync, quiting compare.
                    }
                }
                else
                {
                    //If we hit anything but paragraphs then the compare is done
                    // In the future this will change.
                    return iRet;
                }

                iBefore++;
                iAfter++;
            }
            return iRet;
        }

        //returns true if caller should procede with a text compare
        //or false if it shouldn't.
        //Also can modify one or both state arraylists 
        CompareMsg HandleKnownXamlToXamlTextIssues(TypoInfo BeforeParas, TypoInfo AfterParas, ref int iBefore, ref int iAfter)
        {
            #region //Please leave the following
            
            //$Comment: Please leave the following commented if clauses 
            //in place for quick and easy debugging fixup work
            
            //if ((((ParagraphElement)BeforeParas[iBefore])._iKey == 187) || (((ParagraphElement)AfterParas[iAfter])._iKey == 187))
            //{
            //   iBefore = iBefore;//just something to hang a break point on.
            //}

            //if (iBefore == 1079)
            //{
            //   iAfter = iAfter;//Just something to hang a brek point on.
            //}
            #endregion

            return CompareMsg.CompareContinue;
        }

        void LogXamlToXamlFormatError(FormatType type, SpanElement Before, SpanElement After, int iPara)
        {
            string szType;
            string szBefore;
            string szAfter;
            switch (type)
            {
                case FormatType.FontName:
                    {
                        szType = " FontName";
                        szBefore = Before._szFontName;
                        szAfter = After._szFontName;
                        break;
                    }
                case FormatType.FontSize:
                    {
                        szType = " FontSize";
                        szBefore = Before._dFontSize.ToString();
                        szAfter = After._dFontSize.ToString();
                        break;
                    }
                case FormatType.ForeGround:
                    {
                        szType = " ForeGround";
                        szBefore = Before._lForegroundColor.ToString();
                        szAfter = After._lForegroundColor.ToString();
                        break;
                    }
                case FormatType.Italic:
                    {
                        szType = " Italic";
                        szBefore = (Before._tbItalic == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szAfter = (After._tbItalic == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.Bold:
                    {
                        szType = " Bold";
                        szBefore = (Before._tbBold == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szAfter = (After._tbBold == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.Kerning:
                    {
                        szType = " Kerning";
                        szBefore = (Before._fKerning == true) ? "True" : "False";
                        szAfter = (After._fKerning == true) ? "True" : "False";
                        break;
                    }
                case FormatType.Language:
                    {
                        szType = " Language";
                        szBefore = Before._lLanguageID.ToString();
                        szAfter = After._lLanguageID.ToString();
                        break;
                    }
                case FormatType.SubScript:
                    {
                        szType = " SubScript";
                        szBefore = (Before._tbSubScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szAfter = (After._tbSubScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.SuperScript:
                    {
                        szType = " SuperScript";
                        szBefore = (Before._tbSuperScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szAfter = (After._tbSuperScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.Underline:
                    {
                        szType = " Underline";
                        szBefore = (Before._tbUnderline == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szAfter = (After._tbUnderline == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                default:
                    szType = " none";
                    szBefore = "none";
                    szAfter = "none";
                    break;
            }
            string szError = "Paragraph#: " + iPara.ToString() +
               szType + " changed Before: " + szBefore + " cpStart: " +
               Before._iCPStart.ToString() +
               " cpEnd: " + Before._iCPEnd.ToString() +
               " After: " + szAfter + " cpStart: " + After._iCPStart.ToString() +
               " cpEnd: " + After._iCPEnd.ToString();
            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
        }

        //compare formating for a given paragraph
        int XamlToXamlCompareSpans(ref ParagraphElement Before, ref ParagraphElement After)
        {
            int iRet = 0;
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                #region
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstFontName, After._lstFontName, Before._iKey);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstFontSize, After._lstFontSize, Before._iKey);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstForeGround, After._lstForeGround, Before._iKey);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstItalic, After._lstItalic, Before._iKey);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstBold, After._lstBold, Before._iKey);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            
                            //WPF Kerning doesn't appear to be converting, not sure if it should.
                           //CompareSpanInfo(tp, Before._lstKerning, After._lstKerning, Before._iKey);
                            break;
                        }
                    case FormatType.Language:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstLang, After._lstLang, Before._iKey);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstSubScript, After._lstSubScript, Before._iKey);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstSuperScript, After._lstSuperScript, Before._iKey);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            XamlToXamlCompareSpanInfo(tp, Before._lstUnderline, After._lstUnderline, Before._iKey);
                            break;
                        }
                }
                #endregion
            }
            return iRet;
        }
        
        //Change to RTFToXaml specific compare.
        int XamlToXamlCompareSpanInfo(FormatType tp, ArrayList Before, ArrayList After, int iParaKey)
        {
            int i1 = 0;
            int i2 = 0;
            bool fFail = false;
            //add format count failure

            #region Handle Each type of format
            switch (tp)
            {
                case FormatType.FontName:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (!((SpanElement)Before[i1])._szFontName.Equals(((SpanElement)After[i2])._szFontName))
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.FontSize:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            // Avalon and RE font sizes can be off due to rounding when converted between pixels and points.
                            // Ensure they are valid within .5 of a point (closer precision is not necessary and has been WF'd)
                            if (((SpanElement)Before[i1])._dFontSize < (((SpanElement)After[i2])._dFontSize - .4) ||
                                ((SpanElement)Before[i1])._dFontSize > (((SpanElement)After[i2])._dFontSize + .4))
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.ForeGround:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._lForegroundColor != ((SpanElement)After[i2])._lForegroundColor)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Italic:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._tbItalic != ((SpanElement)After[i2])._tbItalic)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Bold:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._tbBold != ((SpanElement)After[i2])._tbBold)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Kerning:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._fKerning != ((SpanElement)After[i2])._fKerning)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Language:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._lLanguageID != ((SpanElement)After[i2])._lLanguageID)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.SubScript:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._tbSubScript != ((SpanElement)After[i2])._tbSubScript)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    #endregion
                    }
                case FormatType.SuperScript:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._tbSuperScript != ((SpanElement)After[i2])._tbSuperScript)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Underline:
                    #region
                    {
                        while ((i1 < Before.Count) && (i2 < After.Count))
                        {
                            if (((SpanElement)Before[i1])._tbUnderline != ((SpanElement)After[i2])._tbUnderline)
                            {
                                fFail = true;
                                break;
                            }
                            i1++;
                            i2++;
                        }
                        break;
                    }
                    #endregion
            }
            #endregion
            if (fFail)
            {
                LogXamlToXamlFormatError(tp, (SpanElement)Before[i1],
                   (SpanElement)After[i2], iParaKey);
                return 1;
            }
            else
                return 0;
        }

        #endregion

        #region XamlToRtf specific compares
        
        int CompareXamlToRtfParagraphs(TypoInfo xamlParas, TypoInfo rtfParas)
        {
            int iXaml = 0;
            int iRtf = 0;
            int iRet = 0;
            int iCFails = 0;
            CompareMsg result = CompareMsg.CompareContinue;
            
            //Reminder bool for Fixup function and to determin whether or 
            //not to do compare of xaml props.
            while ((iXaml < xamlParas.Count) && (iRtf < rtfParas.Count) &&
               ((result == CompareMsg.CompareContinue) || result == CompareMsg.CompareMsgSkip))
            {
                #region//first compare paragraph strings
                //HandleKnownRtfToXamlTextIssues can sometimes modify the 
                //state info to get rid of bogus fails. It can also cause a
                //skip to next iteration in the while loop, based on some
                //fixups.
                result = HandleKnownRtfToXamlTextIssues(xamlParas, rtfParas, ref iXaml, ref iRtf);

                //If after fixups have been applied the text
                //does not match, a char by char comparison is done
                //to determine if there is really text missing or 
                //mis-converted, and if the discrepancy is worth abandoning 
                //the comparison of the remaining paragraphs.
                if (result == CompareMsg.CompareDetailed)
                {
                    result = RtfToXamlTextCompare(xamlParas, rtfParas, ref iXaml, ref iRtf);
                    
                    //if more than 2 consecutive compare Fails occur, break out and abandon
                    //comparing paragraphs because either the paragraphs are out of sync or 
                    //something equally catastrofic happen during conversion or state gathering.
                    if (result == CompareMsg.CompareFail)
                    {
                        iCFails++;
                        
                        //since this is a fail, I use CompareMsgSkip to avoid
                        //wasting time comparing para props that will definately 
                        //fail.
                        
                        result = (iCFails < 2) ? CompareMsg.CompareSkipFormat : CompareMsg.CompareFail;
                    }
                    else
                    {
                        iCFails = iCFails > 0 ? (iCFails--) : 0;
                    }
                }
                #endregion
                if (result == CompareMsg.CompareContinue)
                {
                    #region Para and span prop Comparison code
                    
                    //now determine if these two paragraphs have the same 
                    //paragraph properties.
                    
                    #region
                    if ((ParagraphElement)xamlParas[iXaml] != (ParagraphElement)rtfParas[iRtf])
                    {
                        iRet += logRTFToXAMLParaPropFails((ParagraphElement)xamlParas[iXaml],
                                 (ParagraphElement)rtfParas[iRtf]);
                    }
                    #endregion
                    
                    //Next Compare format runs
                    
                    #region
                    
                    //charformats differ, then note in the error how they differ.
                    //see font compare for reference
                    //if maybe the next element or so matches or is close to this element
                    ParagraphElement pe1 = (ParagraphElement)xamlParas[iXaml];
                    ParagraphElement pe2 = (ParagraphElement)rtfParas[iRtf];
                    iRet += RtfToXamlCompareSpans(ref pe1, ref pe2);
                    #endregion
                    #endregion
                }
                else if (result == CompareMsg.CompareParaSyncAbandon)
                {
                    //changed the para out of sync error to pri one. From
                    // all of the instances investigated, this is almost never
                    // due to text not being converted and is much more indicative of
                    // mis-recognized lists or other problems that are either bugged
                    // or are in some other way not a relevent bug.
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, "Para elements out of sync, quitting compare.");
                    iRet++;
                    return iRet;
                }
                else if (result == CompareMsg.CompareFail)
                {
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, "More than 2 consecutive text mismatches, quitting Compare.");
                    iRet++;
                    return iRet;
                }
                else if (result == CompareMsg.CompareSkipFormat)
                {
                    //We skipped the format compare so 
                    //ensure further compares are attempted
                    result = CompareMsg.CompareContinue;
                }
                
                //If we are skipping, then indexes have been modified and 
                //need to be left alone.
                if (result != CompareMsg.CompareMsgSkip)
                {
                    iXaml++;
                    iRtf++;
                }
            }

            return iRet;
        }

        //returns true if caller should procede with a text compare
        //  or false if it shouldn't.
        //  Also can modify one or both state arraylists
        CompareMsg HandleKnownRtfToXamlTextIssues(TypoInfo xamlParas, TypoInfo rtfParas, ref int iXaml, ref int iRtf)
        {
            ParagraphElement peAV = ((ParagraphElement)xamlParas[iXaml]);
            ParagraphElement peRE = ((ParagraphElement)rtfParas[iRtf]);
            #region $Comment: Please leave these commented if clauses
            
            //in place for quick and easy debugging fixup work
            
            //if ((((ParagraphElement)xamlParas[iXaml])._iKey == 187) || (((ParagraphElement)rtfParas[iRtf])._iKey == 187))
            //{
            //   iXaml = iXaml;
            //}

            //if (iXaml == 10)
            //{
            //    iXaml = iXaml;//just something to hang a break point on.
            //}
            #endregion


            #region//VT fixup 1
            if (peRE._szText.Contains("\v"))
            {
                bool fDoFixUp = false;
                
                // do a sanity check to make sure we are replacing the correct things
                // we only want to do this replacement if every VT corresponds with a matching \r\n
                
                int i = 0;
                int j = 0;
                for (; i < peRE._szText.Length && j < peAV._szText.Length; i++, j++)
                {
                    if (peRE._szText[i] == '\v')
                    {
                        if (peAV._szText[j] == '\r' &&
                            j + 1 < peAV._szText.Length &&
                            peAV._szText[j + 1] == '\n')
                        {
                            fDoFixUp = true;
                            j++; // skip over the \n
                        }
                        else
                        {
                            fDoFixUp = false;
                        }
                    }
                }
                if (fDoFixUp)
                {
                    peAV._szText = peAV._szText.Replace("\r\n", "\v");
                    //May Cause format comparison problems by not adjusting
                }

            }
            #endregion

            
            //test: I don't think the converter/WPF will ever have a \v
            //in it's text stream. I'm going to try pulling all \v's from
            //peRE and add an assert if one is ever detected in peAV, then 
            //ensure running several text files.
            Debug.Assert(!peAV._szText.Contains("\v"), "A VT has been found in WPF text!");

            //Since it appears that WPF wont return text containing a \v
            //remove all \v's from peRE
            #region
            if (peRE._szText.Contains("\v"))
            {
                while (peRE._szText.Contains("\v"))
                {
                    int iVT = peRE._szText.IndexOf("\v");
                    peRE._szText = peRE._szText.Remove(iVT, 1);
                    
                    //I know that adjusting each time one is removed isn't as 
                    //efficient as storing locations and doing some kind of 
                    //mass adjustment but I'm short on time to complete the 
                    //automation.
                    AdjustParaCps(ref peRE, iVT, 1);
                }
            }
            #endregion

            #region
            if (peAV._szText.Length == 0 || // just para mark
                !(peAV._szText.EndsWith("\r"))) // para with text
            {
                peAV._szText += '\r';
                // if we add a para mark, we might need to add to the endCP for
                //format runs.
                AddCpToEnd(ref peAV);
            }
            #endregion

            
            // 0x7 in RE is end of table cell.
            // If we are comparing RE to AV, the 0x7 will become a \r.
            // This isn't a conversion bug, this is just a implementation difference.
            
            #region
            
            // vertically merged cell, we need to skip this RE para
            // not a conversion bug, just an implementation difference.
            //RE will have one or more para's with just "\xFFFF\a" while 
            //WPF will just return the text of the v merged cell.
            if (peRE._szText == "\xFFFF\a")
            {
                iRtf++;
                return CompareMsg.CompareMsgSkip;
            }

            if (peRE._szText.EndsWith("\a") && peAV._szText.EndsWith("\r"))
            {
                // Set the AV \r to \a to match richedit.
                if (peAV._szText.Length == 1)
                {
                    peAV._szText = "\a"; // only an \r
                    //format runs are going to be messed up 
                    //here. There wont be anything to extend.
                }
                else if (peAV._szText.Substring(0, peAV._szText.Length - 1) == peRE._szText.Substring(0, peRE._szText.Length - 1))
                {
                    peAV._szText = peAV._szText.Substring(0, peAV._szText.Length - 1);
                    peAV._szText += '\a';
                    AddCpToEnd(ref peAV);//adjust format runs
                }
            }
            #endregion

            
            //Formfeed (fixup Simple one), there is another below
            //the bnn fixup clause as well.            
            #region
            if ((peRE._szText == "\f"))
            {
                if (((ParagraphElement)rtfParas[iRtf + 1])._szText.Contains(peAV._szText))
                {
                    iRtf++;//skip the formfeed
                    return CompareMsg.CompareMsgSkip;
                }
            }
            #endregion

            #region
            if (peRE._szText.Contains("\xAD"))
            {
                if (peRE._szText.IndexOf("\xAD") != peAV._szText.IndexOf("\xAD"))
                {
                    int iRemoveCp = peRE._szText.IndexOf("\xAD");
                    peRE._szText = peRE._szText.Replace("\xAD", "");
                    
                    //if a charactor is removed, need to update format runs.
                    
                    AdjustParaCps(ref peRE, iRemoveCp, 1);
                }
            }
            #endregion

            //Richedit doesn't always recognize a list item, but will 
            //often add the text for the list marker instead. Xaml, by 
            //structure, doesn't include marker text.
            //6/7/06 WPF tends to recognize more lists and change them
            //to WPF style lists, which will not yield list text. So list 
            //fixups use WPF (peAV) to indicate whether to check for 
            //RE list text or not. 
            #region
            if (peAV._fListPara && (peAV._szText.Length >= 1) && (peRE._szText.Length >= 1))
            {
                int uiRemoveLength = 0;
                int iRemoveCp = 0;

                //determine if mismatch due to RE list handling
                
                #region BNN Check #1
                if (peRE._szText.Length >= 2)
                {
                    if (((peRE._szText[0] == 0xb7) ||//bullet char
                       ((0x30 > peRE._szText[0]) && (peRE._szText[0] < 0x3A)) ||//numeral
                       ((0x40 > peRE._szText[0]) && (peRE._szText[0] < 0x5B)) ||//upper case alpha
                       ((0x60 > peRE._szText[0]) && (peRE._szText[0] < 0x7B))) &&//lower case alpha
                       peRE._szText.Substring(0, Math.Min(5, peRE._szText.Length)).Contains("\t"))
                    {
                        //remove list marker text and tab from RE text.
                        
                        uiRemoveLength = peRE._szText.IndexOf("\t") + 1;//count must be inc'd because index is zero based.
                        peRE._szText = peRE._szText.Remove(0, uiRemoveLength);
                        iRemoveCp = 0;
                    }
                }

                #endregion
                
                //A regex to hopefully catch other types of lists, 
                //?even empty lists with just marker text?
                //It must have a TAB following the list text, or it will 
                //not match and catch it.
                //BNN Check #2
                
                #region BNN Check #2
                Regex myRegex = new Regex(@"^.{1,3}[\).-]?\t");
                if (myRegex.IsMatch(peRE._szText))
                {
                    //remove list marker text and tab from RE text.                    
                    uiRemoveLength = peRE._szText.IndexOf("\t") + 1;//count must be inc'd because index is zero based.
                    peRE._szText = peRE._szText.Remove(0, uiRemoveLength);
                    iRemoveCp = 0;
                }
                #endregion

                #region Unimplemented BNN Check ideas

                #endregion

                if (uiRemoveLength > 0)
                {
                    //We have removed text from the RE side, so we need 
                    //to adjust span cp values accordingly
                    AdjustParaCps(ref peRE, iRemoveCp, uiRemoveLength);
                }
            }
            #endregion

            //Sometimes form feed "\f" is at the end of the string
            #region
            if (peAV._szText.EndsWith("\r") && peRE._szText.EndsWith("\f"))
            {
                string szOld = ((ParagraphElement)rtfParas[iRtf])._szText;
                string szNew = szOld.Remove(szOld.Length - 1);//remove the trailing \f
                szNew = szNew + "\r";//add in the \r
                ((ParagraphElement)rtfParas[iRtf])._szText = szNew;
            }
            #endregion

            string szXaml = ((ParagraphElement)xamlParas[iXaml])._szText;
            string szRtf = ((ParagraphElement)rtfParas[iRtf])._szText;

            #region
            if ((szXaml.Length >= 1) && (szRtf.Length >= 1))
            {
                if ((szXaml[0] == 0xC || szRtf[0] == 0xC) && (szRtf[0] != szXaml[0]))
                {
                    // The tricky part here is that if the previous character in RE
                    // was a CR, then the 0x0C is completely eaten in the post state, not turned into a CR. 
                    // It is also completely eaten when the 0x0C is at the beginning of the control.
                                        
                    //Remove this assert before checkin, I'm verifying that xaml 
                    //doesn't output formfeed charactors.
                    
                    Debug.Assert((szXaml[0] != 0xC), "WPF had a form feed, change this routine!");

                    iRtf++;
                    return CompareMsg.CompareMsgSkip;
                }
            }
            #endregion

            //Check to see if the para strings match or not.             
            if (szXaml == szRtf)
            {
                return CompareMsg.CompareContinue;
            }
            else
            {
                //this will let the controller function know 
                //to do a char by char compare.
                return CompareMsg.CompareDetailed;
            }
        }


        // Char by char compare of two specific paragraphs.
        // Note: this function will log an error if it finds one.
        CompareMsg RtfToXamlTextCompare(TypoInfo xamlParas, TypoInfo rtfParas, ref int iXaml, ref int iRtf)
        {
            string szRtf = ((ParagraphElement)rtfParas[iRtf])._szText;
            string szXaml = ((ParagraphElement)xamlParas[iXaml])._szText;

            //Since this function was called, there is going to be 
            //a difference of some sort. The difference will occur at
            //iCp. Now I need to determine if it's minor or serious.
            int iCp = szCompare(szRtf, szXaml);

            //Single char differences are often misc tabs, etc that are
            //might not be considered bugs.
            if (Math.Abs(szRtf.Length - szXaml.Length) == 1)
            {
                LogStringCompareDiscrepancy(szRtf, szXaml, iRtf,
                                            iXaml, iCp, true);
                return CompareMsg.CompareContinue;
            }
            else
            {
                //For now, if there's more than one char difference
                //I'll consider that a fatal pri 0 error and report the 
                //difference. 
                
                //in order to avoid index out of bounds exceptions,
                //it's neccessary to calc and check substring lens
                //against actual string lengths first!
                
                int iRtfSub = Math.Min(5, szRtf.Length - iCp - 1);
                int iXamlSub = Math.Min(5, szXaml.Length - iCp - 1);
                string szErrstring = "Text Compare failed: Rtf para#";
                if (((iCp + iRtfSub) < szRtf.Length) && ((iCp + iXamlSub) < szXaml.Length) && iRtfSub > 0 && iXamlSub > 0)
                {
                    szErrstring += iRtf.ToString() + " U+" + ((uint)szRtf[iCp]).ToString("x") +
                    " Xaml para#: " + iXaml.ToString() + " U+" + ((uint)szXaml[iCp]).ToString("x") +
                    " Rtf=\"" + szRtf.Substring(iCp, iRtfSub) +
                    "\"" + " Xaml=\"" + szXaml.Substring(iCp, iXamlSub) +
                    "\"" + " at cp: " + iCp.ToString();
                }
                else
                {
                    szErrstring += iRtf.ToString() + " Xaml para#: " + iXaml.ToString() +
                        "Unable to show string difference at cp: " + iCp.ToString();
                }
                _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szErrstring);
                return CompareMsg.CompareFail;
            }
        }

        // handles determining exactly what properties 
        // were different and logging those specific
        // Discrepancies.
        int logRTFToXAMLParaPropFails(ParagraphElement Xaml, ParagraphElement Rtf)
        {
            int iRet = 0;

            if (Xaml._taTextAlignment != Rtf._taTextAlignment)
            {
                #region
                bool fFail = true;
                
                //Xaml has logical verses physical layout for text alignment
                //in rtl para's. So not a converter bug just a difference 
                //in display engines.
                //for more info see: Regression_Bug351
                if ((Xaml._fd == FlowDirection.RightToLeft) &&
                   (Xaml._taTextAlignment == TomAlignment.tomAlignRight) &&
                   (Rtf._taTextAlignment == TomAlignment.tomAlignLeft))
                {
                    fFail = false;
                }
                else if ((Xaml._fd == FlowDirection.RightToLeft) &&
                   (Xaml._taTextAlignment == TomAlignment.tomAlignLeft) &&
                   (Rtf._taTextAlignment == TomAlignment.tomAlignRight))
                {
                    fFail = false;
                }

                if (fFail)
                {
                    string szError = "Paragraph#: " + Xaml._iKey.ToString() +
                       " TextAlignment  Xaml: " + TomHelpers.TomAlignToString(Xaml._taTextAlignment) +
                       " Rtf: " + TomHelpers.TomAlignToString(Rtf._taTextAlignment);
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                    iRet++;
                }
                #endregion
            }
            
            //FirstLine indent is not a strait forward conversion that 
            //will result in an exact TextIndent value on the xaml side
            //or visa versa.
            //hence a pri 2 error.
            if (Math.Round(Xaml._dFirstLineIndent, 2) != Math.Round(Rtf._dFirstLineIndent, 2))
            {
                #region
                if (((Xaml._dFirstLineIndent == 0) && (Rtf._dFirstLineIndent < 0)))
                {
                    string szError = "Paragraph#: " + Xaml._iKey.ToString() +
                       " FirstLineIndent Xaml: " + Xaml._dFirstLineIndent.ToString() +
                       " Rtf: " + Rtf._dFirstLineIndent.ToString() +
                       " Regression_Bug353 Avalon doesnt allow negative margins";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                }
                else
                {
                    string szError = "Paragraph#: " + Xaml._iKey.ToString() +
                       " FirstLineIndent Xaml: " + Xaml._dFirstLineIndent.ToString() +
                       " Rtf: " + Rtf._dFirstLineIndent.ToString();
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                }
                iRet++;
                #endregion
            }
            return iRet;
        }

        void LogRtfToXamlFormatError(FormatType type, SpanElement seXaml, SpanElement seRtf, int iPara)
        {
            string szType;
            string szXaml;
            string szRtf;
            int iPri = 1;
            switch (type)
            {
                case FormatType.FontName:
                    {
                        szType = " FontName";
                        szXaml = seXaml._szFontName;
                        szRtf = seRtf._szFontName;
                        iPri = 2;
                        break;
                    }
                case FormatType.FontSize:
                    {
                        szType = " FontSize";
                        szXaml = seXaml._dFontSize.ToString();
                        szRtf = seRtf._dFontSize.ToString();
                        break;
                    }
                case FormatType.ForeGround:
                    {
                        szType = " ForeGround";
                        szXaml = seXaml._lForegroundColor.ToString();
                        szRtf = seRtf._lForegroundColor.ToString();
                        break;
                    }
                case FormatType.Italic:
                    {
                        szType = " Italic";
                        szXaml = (seXaml._tbItalic == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szRtf = (seRtf._tbItalic == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.Bold:
                    {
                        szType = " Bold";
                        szXaml = (seXaml._tbBold == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szRtf = (seRtf._tbBold == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                case FormatType.Kerning:
                    {
                        //reportable kerning values between RE and WPF are different 
                        //enough that this is a very low pri failure.
                        
                        szType = " Kerning";
                        szXaml = (seXaml._fKerning == true) ? "True" : "False";
                        szRtf = (seRtf._fKerning == true) ? "True" : "False";
                        iPri = 4;
                        break;
                    }
                case FormatType.Language:
                    {
                        //Lang compares not entirely valid. See comments in rtf 
                        //state gatehring section for language to understand why.
                        
                        szType = " Language";
                        szXaml = seXaml._lLanguageID.ToString();
                        szRtf = seRtf._lLanguageID.ToString();
                        iPri = 4;
                        break;
                    }
                case FormatType.SubScript:
                    {
                        szType = " SubScript";
                        szXaml = (seXaml._tbSubScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szRtf = (seRtf._tbSubScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        iPri = 2;//xaml side state not collected yet
                        break;
                    }
                case FormatType.SuperScript:
                    {
                        szType = " SuperScript";
                        szXaml = (seXaml._tbSuperScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szRtf = (seRtf._tbSuperScript == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        iPri = 2;//xaml side state not collected yet
                        break;
                    }
                case FormatType.Underline:
                    {
                        szType = " Underline";
                        szXaml = (seXaml._tbUnderline == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        szRtf = (seRtf._tbUnderline == TomBool.tomTrue) ? "tomTrue" : "tomFalse";
                        break;
                    }
                default:
                    szType = " none";
                    szXaml = "none";
                    szRtf = "none";
                    iPri = 4;
                    break;
            }
            string szError = "Paragraph#: " + iPara.ToString() +
               szType + " changed Xaml: " + szXaml + " cpStart: " +
               seXaml._iCPStart.ToString() +
               " cpEnd: " + seXaml._iCPEnd.ToString() +
               " Rtf: " + szRtf + " cpStart: " + seRtf._iCPStart.ToString() +
               " cpEnd: " + seRtf._iCPEnd.ToString();
            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, iPri, szError);
        }

        //compare formating for a given paragraph
        int RtfToXamlCompareSpans(ref ParagraphElement peXaml, ref ParagraphElement peRtf)
        {
            int iRet = 0;
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                #region
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstFontName, peRtf._lstFontName, peXaml._iKey);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstFontSize, peRtf._lstFontSize, peXaml._iKey);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstForeGround, peRtf._lstForeGround, peXaml._iKey);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstItalic, peRtf._lstItalic, peXaml._iKey);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstBold, peRtf._lstBold, peXaml._iKey);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            //WPF Kerning doesn't appear to be converting, not sure if it should.
                            //CompareSpanInfo(tp, Before._lstKerning, After._lstKerning, Before._iKey);
                            break;
                        }
                    case FormatType.Language:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstLang, peRtf._lstLang, peXaml._iKey);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstSubScript, peRtf._lstSubScript, peXaml._iKey);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstSuperScript, peRtf._lstSuperScript, peXaml._iKey);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            RtfToXamlCompareSpanInfo(tp, peXaml._lstUnderline, peRtf._lstUnderline, peXaml._iKey);
                            break;
                        }
                }
                #endregion
            }
            return iRet;
        }

        //RtfToXamlCompareSpanInfo:
        //Just helps to make the comparison of the format arrays 
        //a bit more modular.
        int RtfToXamlCompareSpanInfo(FormatType tp, ArrayList Xaml, ArrayList Rtf, int iParaKey)
        {
            int iXaml = 0;
            int iRtf = 0;
            bool fFail = false;
            //add format count failure

            #region Handle Each type of format
            switch (tp)
            {
                case FormatType.FontName:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (!((SpanElement)Xaml[iXaml])._szFontName.Equals(((SpanElement)Rtf[iRtf])._szFontName))
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.FontSize:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            // Avalon and RE font sizes can be off due to rounding when converted between pixels and points.
                            // Ensure they are valid within +-.5 of a point (closer precision is not necessary and has been WF'd)
                            
                            //if(Math.Ceiling(((SpanElement)Xaml[iXaml])._dFontSize) !=
                            //    Math.Ceiling(((SpanElement)Rtf[iRtf])._dFontSize))
                            //trying out a different rounding routine to get ride of so many fails.
                            if (((SpanElement)Xaml[iXaml])._dFontSize < (((SpanElement)Rtf[iRtf])._dFontSize - .5) ||
                                ((SpanElement)Xaml[iXaml])._dFontSize > (((SpanElement)Rtf[iRtf])._dFontSize + .5))                            
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.ForeGround:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._lForegroundColor != ((SpanElement)Rtf[iRtf])._lForegroundColor)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Italic:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._tbItalic != ((SpanElement)Rtf[iRtf])._tbItalic)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Bold:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._tbBold != ((SpanElement)Rtf[iRtf])._tbBold)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Kerning:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._fKerning != ((SpanElement)Rtf[iRtf])._fKerning)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Language:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._lLanguageID != ((SpanElement)Rtf[iRtf])._lLanguageID)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.SubScript:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._tbSubScript != ((SpanElement)Rtf[iRtf])._tbSubScript)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    #endregion
                    }
                case FormatType.SuperScript:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._tbSuperScript != ((SpanElement)Rtf[iRtf])._tbSuperScript)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
                case FormatType.Underline:
                    #region
                    {
                        while ((iXaml < Xaml.Count) && (iRtf < Rtf.Count))
                        {
                            if (((SpanElement)Xaml[iXaml])._tbUnderline != ((SpanElement)Rtf[iRtf])._tbUnderline)
                            {
                                fFail = true;
                                break;
                            }
                            iXaml++;
                            iRtf++;
                        }
                        break;
                    }
                    #endregion
            }
            #endregion
            if (fFail)
            {
                LogRtfToXamlFormatError(tp, (SpanElement)Xaml[iXaml],
                   (SpanElement)Rtf[iRtf], iParaKey);
                return 1;
            }
            else
                return 0;
        }

        #endregion

        //The following are generic Para compare helper methods
        //used by both XamlToRtf and XamlToXaml compares
        #region


        int logXAMLToXamlParaPropFails(ParagraphElement Before, ParagraphElement After)
        {
            int iRet = 0;

            if (Before._taTextAlignment != After._taTextAlignment)
            {
                #region
                bool fFail = true;
                
                //Xaml has logical verses physical layout for text alignment
                //in rtl para's. So not a converter bug just a difference 
                //in display engines.
                //for more info see: Regression_Bug351
                if (Before._fXAMLState)//rtf to xaml compare
                {
                    if ((Before._fd == FlowDirection.RightToLeft) &&
                       (Before._taTextAlignment == TomAlignment.tomAlignRight) &&
                       (After._taTextAlignment == TomAlignment.tomAlignLeft))
                    {
                        fFail = false;
                    }
                    else if ((Before._fd == FlowDirection.RightToLeft) &&
                       (Before._taTextAlignment == TomAlignment.tomAlignLeft) &&
                       (After._taTextAlignment == TomAlignment.tomAlignRight))
                    {
                        fFail = false;
                    }
                }
                if (fFail)
                {
                    string szError = "Paragraph#: " + Before._iKey.ToString() +
                       " TextAlignment  Before: " + TomHelpers.TomAlignToString(Before._taTextAlignment) +
                       " After: " + TomHelpers.TomAlignToString(After._taTextAlignment);
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 0, szError);
                    iRet++;
                }
                #endregion
            }
            
            if (Math.Round(Before._dFirstLineIndent, 2) != Math.Round(After._dFirstLineIndent, 2))
            {
                #region
                if (((Before._fXAMLState && Before._dFirstLineIndent == 0) && (After._dFirstLineIndent < 0)) ||
                    (After._fXAMLState && After._dFirstLineIndent == 0) && (Before._dFirstLineIndent < 0))
                {
                    string szError = "Paragraph#: " + Before._iKey.ToString() +
                       " FirstLineIndent Before: " + Before._dFirstLineIndent.ToString() +
                       " After: " + After._dFirstLineIndent.ToString() +
                       " Regression_Bug353 Avalon doesnt allow negative margins";
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 4, szError);
                }
                else
                {
                    string szError = "Paragraph#: " + Before._iKey.ToString() +
                       " FirstLineIndent Before: " + Before._dFirstLineIndent.ToString() +
                       " After: " + After._dFirstLineIndent.ToString();
                    _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 2, szError);
                }
                iRet++;
                #endregion
            }
            return iRet;
        }

        /************************************************
        * Function: LogStringCompareDiscrepancy
        *
        * Comments: Logs a difference in string compare that 
        *           does not halt compare process.
        *           I am considering this a Pri 1 failure. 
        *           Because this is now in it's own function,
        *           known bugs and inate handling dirrences 
        *           between RichEdit and WPF can be pointed out 
        *           here as well.
         * 
         *          when calling this with fXamlRtf = true
         *          szBefore is always RTF version, and 
         *          szAfter is always Xaml Version
        **************************************************/
        void LogStringCompareDiscrepancy(string szBefore, string szAfter, int iBeforePara,
                                        int iAfterPara, int iCp, bool fXamlRtf)
        {
            string szError;
            //Create initial error string
            if (fXamlRtf)
            {
                //XamlToRtf compare
                if (szBefore.Length > iCp && szAfter.Length > iCp)
                {
                    szError = "Discrepancy: Char at Rtf para# " +
                             iBeforePara.ToString() + " U+" + ((uint)szBefore[iCp]).ToString("x") +
                            ", Char at Corresponding Xaml para# " + iAfterPara.ToString() +
                            " U+" + ((uint)szAfter[iCp]).ToString("x") +
                            ". At cp: " + iCp.ToString() + ".";
                }
                else
                {
                    szError = "Discrepancy at Rtf para# " +
                             iBeforePara.ToString() +
                            ", Corresponding Xaml para# " + iAfterPara.ToString() +
                            ". At cp: " + iCp.ToString() + ". Unable to show difference.";
                }
            }
            else
            {
                if (szBefore.Length > iCp && szAfter.Length > iCp)
                {
                    szError = "Discrepancy: Char at Before para# " +
                             iBeforePara.ToString() + " U+" + ((uint)szBefore[iCp]).ToString("x") +
                            ", Char at Corresponding After para# " + iAfterPara.ToString() +
                            " U+" + ((uint)szAfter[iCp]).ToString("x") +
                            ". At cp: " + iCp.ToString() + ".";
                }
                else
                {
                    szError = "Discrepancy at Before para# " +
                             iBeforePara.ToString() +
                            ", Corresponding After para# " + iAfterPara.ToString() +
                            ". At cp: " + iCp.ToString() + ". Unable to show difference.";
                }
            }

            _elLog.LogError(ConverterErrorType.ErrorRtfConversion, 1, szError);
        }

        /************************************************
        * Function: string CompareString(string Before, string after, int iPara)
        *
        * Comments: Does a char by char comparison, and returns an
        *           error string that illustrates the first mismatch,
        *           with 5 chars to show difference.
        *           Note: compare is per paragraph.
         * 
        **************************************************/
        string CompareString(string Before, string After, int iParaKey)
        {
            for (int index = 0; (index < Before.Length && index < After.Length); index++)
            {
                if (Before[index] != After[index])
                {
                    string errstring = "Text Compare failed for paragraph# " +
                          iParaKey.ToString() + " at index " + index.ToString() +
                          " pre U+" + ((uint)Before[index]).ToString("x") +
                          " post U+" + ((uint)After[index]).ToString("x") +
                          " pre=\"" +
                          Before.Substring(index, Math.Min(5, Before.Length - index - 1)) + "\"" +
                        " post=\"" +
                        After.Substring(index, Math.Min(5, After.Length - index - 1)) + "\"";
                    return errstring;
                }
            }
            return null;
        }

        /************************************************
        * Function: szCompare
        *
        * Comments: This does a char by char compare that 
         *          let's caller know where compare failed
         *          
        *           iFail is the cp at which the compare failed.
         *          Returns False if strings don't match.
         *                  true if they do.
         *         6/8/06:
         *         Note: There doesn't appear to be a string method
         *         or String method that does a char by char 
         *         ordinal compare, then returns either the 
         *         index of adifference or -1 if no difference 
         *         exists, eitehr I missed something or I dunno.
         *
        **************************************************/
        int szCompare(string sz1, string sz2)
        {
            int iLen = Math.Min(sz1.Length, sz2.Length); //0;

            if ((iLen == 0) && (sz1.Length != sz2.Length))
            {
                return 0;
            }

            for (int index = 0; (index < iLen); index++)
            {
                if (sz1[index] != sz2[index])
                {
                    return index;
                }
            }
            return -1;
        }

        /************************************************
        * Function: AddCpToEnd - add's 1 to end cp value for 
         *                       span format runs that cover end.
        *
        * Comments: Just extends whatever format value is there.
         * assumes the text has already been modified.
        **************************************************/
        void AddCpToEnd(ref ParagraphElement Para)
        {
            int endCP = Para._szText.Length - 1;
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                #region
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            if ((Para._lstFontName.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstFontName[Para._lstFontName.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstFontName[Para._lstFontName.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            if ((Para._lstFontSize.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstFontSize[Para._lstFontSize.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstFontSize[Para._lstFontSize.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            if ((Para._lstForeGround.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstForeGround[Para._lstForeGround.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstForeGround[Para._lstForeGround.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.Italic:
                        {
                            if ((Para._lstItalic.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstItalic[Para._lstItalic.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstItalic[Para._lstItalic.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.Bold:
                        {
                            if ((Para._lstBold.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstBold[Para._lstBold.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstBold[Para._lstBold.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            //WPF Kerning doesn't appear to be converting, not sure if it should.
                            //CompareSpanInfo(tp, Before._lstKerning, After._lstKerning, Before._iKey);
                            break;
                        }
                    case FormatType.Language:
                        {
                            if ((Para._lstLang.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstLang[Para._lstLang.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstLang[Para._lstLang.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            if ((Para._lstSubScript.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstSubScript[Para._lstSubScript.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstSubScript[Para._lstSubScript.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            if ((Para._lstSuperScript.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstSuperScript[Para._lstSuperScript.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstSuperScript[Para._lstSuperScript.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                    case FormatType.Underline:
                        {
                            if ((Para._lstUnderline.Count - 1) >= 0)
                            {
                                if (((SpanElement)Para._lstUnderline[Para._lstUnderline.Count - 1])._iCPEnd < endCP)
                                {
                                    ((SpanElement)Para._lstUnderline[Para._lstUnderline.Count - 1])._iCPEnd++;
                                }
                            }
                            break;
                        }
                }
                #endregion
            }
        }

        /************************************************
        * Function: AdjustParaCps
        *
        * Comments: Adjusts the cp values of the various 
        *           format span area's from front to back, 
        *           or from the back.
        **************************************************/
        void AdjustParaCps(ref ParagraphElement pe, int iCp, int length)
        {
            for (FormatType tp = FormatType.FontName; tp != FormatType.None; tp++)
            {
                switch (tp)
                {
                    case FormatType.FontName:
                        {
                            AdjustListCps(ref pe._lstFontName, iCp, length);
                            break;
                        }
                    case FormatType.FontSize:
                        {
                            AdjustListCps(ref pe._lstFontSize, iCp, length);
                            break;
                        }
                    case FormatType.ForeGround:
                        {
                            AdjustListCps(ref pe._lstForeGround, iCp, length);
                            break;
                        }
                    case FormatType.Italic:
                        {
                            AdjustListCps(ref pe._lstItalic, iCp, length);
                            break;
                        }
                    case FormatType.Bold:
                        {
                            AdjustListCps(ref pe._lstBold, iCp, length);
                            break;
                        }
                    case FormatType.Kerning:
                        {
                            AdjustListCps(ref pe._lstKerning, iCp, length);
                            break;
                        }
                    case FormatType.Language:
                        {
                            AdjustListCps(ref pe._lstLang, iCp, length);
                            break;
                        }
                    case FormatType.SubScript:
                        {
                            AdjustListCps(ref pe._lstSubScript, iCp, length);
                            break;
                        }
                    case FormatType.SuperScript:
                        {
                            AdjustListCps(ref pe._lstSuperScript, iCp, length);
                            break;
                        }
                    case FormatType.Underline:
                        {
                            AdjustListCps(ref pe._lstUnderline, iCp, length);
                            break;
                        }
                }
            }
        }

        //AdjustListCps(ref ArrayList lst, int iCp, int length)
        // iCP = point to begin deletion
        // length = length of cells to remove.
        //the deletion will always include iCp
        void AdjustListCps(ref ArrayList lst, int iCp, int length)
        {
            int i = 0;//index of initial change node.
            int j = 0;//index of node whose iCPEnd <=iEndCp
            int iEndCp = iCp + length;
            //bool fBreak = false;
            //int diff = 0;//scratch var 1
            //int nodeLength = 0;//scratch var 2
            //int nodeEnd = 0; //scratch var 3

            //if this list has no elements then just return
            //this is valid because not all Arrays will have 
            //elements.
            if (lst.Count == 0)
            {
                return;
            }

            //Next obvious case, only one node in array
            if (lst.Count == 1)
            {
                SpanElement sp = ((SpanElement)lst[0]);
                if (AdjustNodeCps(ref sp, iCp, length) == AdjustResult.AdjustDelete)
                {
                    lst.RemoveAt(0);
                }
                else
                {
                    lst[0] = sp;
                }
            }
            else
            {
                //Multinode list to contend with
                //Find Node i containing iCp
                
                #region
                while ((i < lst.Count) && (((SpanElement)lst[i])._iCPStart < iCp))
                {
                    i++;
                }
                if (i > 0)
                {
                    i--;
                }
                
                //either none of the nodes contains the delete range or
                //the last one does.
                if (i == (lst.Count - 1))
                {
                    SpanElement sp = ((SpanElement)lst[i]);
                    if (AdjustNodeCps(ref sp, iCp, length) == AdjustResult.AdjustDelete)
                    {
                        lst.RemoveAt(i);
                    }
                    else
                    {
                        lst[i] = sp;
                    }
                    return;//done
                }
                #endregion

                //At this point i should be the index of the first cell
                //whose range contains iCp.
                if (((SpanElement)lst[i])._iCPEnd >= iEndCp)
                {
                    //Deleted range is entirely in this node
                    SpanElement sp = ((SpanElement)lst[i]);
                    if (AdjustNodeCps(ref sp, iCp, length) == AdjustResult.AdjustDelete)
                    {
                        lst.RemoveAt(i);
                    }
                    else
                    {
                        lst[i] = sp;
                    }
                }
                else
                {
                    //Find Node node containing iEndCp
                    #region
                    j = i;//start looking from iCp's node

                    while ((((SpanElement)lst[j])._iCPEnd <= iEndCp) && j < lst.Count)
                    {
                        j++;
                    }
                    j--;
                    
                    //Either iEndCP is greater than all node endCP's past i or
                    //the last node End CP is greater than or equal to iEndCP
                    
                    if (j == (lst.Count - 1))
                    {
                        #region
                        if (((SpanElement)lst[j])._iCPEnd <= iEndCp)
                        {
                            //iEndCp was larger than or equal to everything in the array
                            //after i.
                            //Remove everything after i.
                            lst.RemoveRange(i + 1, (lst.Count - (i + 1)));
                            
                            //Since we know that i contained iCp and the iEndCp was higher
                            //than any other node's endcp value, if could still require
                            //i to be deleted
                            SpanElement sp = ((SpanElement)lst[i]);
                            if (AdjustNodeCps(ref sp, iCp, length) == AdjustResult.AdjustDelete)
                            {
                                lst.RemoveAt(i);
                            }
                            else
                            {
                                lst[i] = sp;
                            }
                            return;//nothing left in this array to adjust
                        }
                        Debug.Assert(false, "Something is wrong!");
                        #endregion
                    }
                    #endregion

                    //at this point:
                    //i holds a range containing iCP
                    //j holds a range containing iEndCP, but is not the end of lst
                    
                    #region
                    AdjustResult result = AdjustResult.AdjustDone;
                    while ((result != AdjustResult.AdjustIgnore))
                    {
                        SpanElement sp = ((SpanElement)lst[i]);
                        result = AdjustNodeCps(ref sp, iCp, length);
                        if (result == AdjustResult.AdjustDelete)
                        {
                            lst.RemoveAt(i);
                        }
                        else if (result == AdjustResult.AdjustDone)
                        {
                            lst[i] = sp;
                            i++;
                        }
                        Debug.Assert(i < lst.Count, "AdjustListCps is now out of range!");
                    }
                    #endregion

                    //Adjust remaining cells, if any
                    //i should be the first cell after 
                    //what was j.
                    if (i > 0)
                        while (i < lst.Count)
                        {
                            int nodeLength = ((SpanElement)lst[i])._iCPEnd - ((SpanElement)lst[i])._iCPStart;
                            ((SpanElement)lst[i])._iCPStart = ((SpanElement)lst[i - 1])._iCPEnd + 1;
                            ((SpanElement)lst[i])._iCPEnd = ((SpanElement)lst[i])._iCPStart + nodeLength;
                            i++;
                        }
                }
            }
        }

        /************************************************
        * Function: AdjustNodeCps
        *
        * Comments: Adjusts a single node based on the 
         *          supplied iCp and length.
        **************************************************/
        AdjustResult AdjustNodeCps(ref SpanElement sp, int iCp, int length)
        {
            int iEndCp = iCp + length;
            int diff = sp._iCPStart - iCp;
            int nodeLength = sp._iCPEnd - sp._iCPStart;
            int nodeEnd = sp._iCPEnd;
            AdjustResult result = AdjustResult.AdjustIgnore;
            
            //delete starts at the same cp
            //this node starts at 
            if ((diff == 0))
            {
                #region //node start and iCp are equal
                if (nodeLength <= length)
                {
                    //Deletion extends beyond end of node.
                    result = AdjustResult.AdjustDelete;
                }
                else
                {
                    //Deletion doesn't extend beyond the end of node.
                    //so start stays the same, but end changes.
                    
                    //((SpanElement)lst[0])._iCPStart = iCp;
                    sp._iCPEnd = nodeEnd - length;
                    Debug.Assert((sp._iCPEnd - sp._iCPStart) >= 1, "invalid node length!");
                    result = AdjustResult.AdjustDone;
                }
                #endregion
            }
            else if (diff < 0)
            {
                
                //Node starts somewhere before iCp
                
                #region
                if (nodeEnd <= iEndCp)
                {
                    //Delete extends beyond end of node
                    sp._iCPEnd = iCp - 1;
                    //Debug.Assert((sp._iCPEnd - sp._iCPStart) >= 1, "invalid node length!");
                    result = AdjustResult.AdjustDone;
                }
                else
                {
                    //Delete ends before end of node,
                    //so we are simply removing a chunk
                    //from the middle.
                    sp._iCPEnd = nodeEnd - length;
                    result = AdjustResult.AdjustDone;
                }
                #endregion
            }
            else
            {
                //Node starts after iCp
                //if this occurs, node start never changes
                //there is always soemthing left of it's range.
                #region
                if (iEndCp >= sp._iCPEnd)
                {
                    //Deletion extends to or beyond end of node.
                    //Rear chunk is removed
                    sp._iCPEnd = iCp - 1;//delete includes iCp
                    result = AdjustResult.AdjustDone;
                }
                else if (sp._iCPStart <= iEndCp)
                {
                    //Delete clips from front end 
                    //of node's range.
                    sp._iCPStart = iEndCp + 1;
                    result = AdjustResult.AdjustDone;
                }
                #endregion
                //delete misses node all together
            }
            return result;
        }
        #endregion

        #endregion

        int CompareXamlToXaml(XamlStateCompare Before, XamlStateCompare After)
        {
            int iRet = 0;

            //Compare all of the paragraphs in a seperate routine 
            //for clarity, need to be able to reference the 
            //previous and next paragraph for fixup's.
            iRet += CompareXamlToXamlParagraphs(Before._xamlParas, After._xamlParas);

            //Now compare lists
            iRet += XamlToXamlCompareList(Before._xamlLists,
                After._xamlLists);

            //Now compare tables
            iRet += CompareXamlToXamlTable(Before._xamlTables, After._xamlTables);

            return iRet;
        }

        /************************************************
        * Function: CompareRtfToXaml
        *
        * Comments: Compare's the current RichEdit RichTextbox
        *           to the current Avalon RichTextBox with
        *           Fixup's to compensate for differences in
        *           file formats.
        *
        **************************************************/
        int CompareRtfToXaml(XamlStateCompare xaml, NewREStateCompare rtf)
        {
            int iRet = 0;
            
            //Compare paragraphs first,
            //this will offer clues as to whether richedit missed a list 
            //and such.
            //Compare all of the paragraphs in a seperate routine 
            //for clarity, need to be able to reference the 
            //previous and next paragraph for fixup's.
            iRet += CompareXamlToRtfParagraphs(xaml._xamlParas, rtf._rtfParas);

            //Now compare lists
            iRet += XamlToRtfCompareList(xaml._xamlLists, rtf._rtfLists);
            //Now compare tables
            iRet += CompareXamlToRtfTable(xaml._xamlTables, rtf._rtfTables);

            return iRet;
        }

        #endregion

        #region private members
        private RtfXamlViewApp _rxv;
        private ErrorLog _elLog;
        #endregion
    }
    #endregion
}