// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace DRT
{
    //Helper class for extracting list of all applicable OpenType features from a GlyphRun.
    internal class D
    {
        private static bool s_traceOn = false;

        internal static void WriteLine(string message)
        {
            if (s_traceOn)
            {
                Console.WriteLine(message);
            }
        }

        internal static void WriteLine(string message, params object[] arg)
        {
            if (s_traceOn)
            {
                Console.WriteLine(message, arg);
            }
        }
    }


    internal class FontFile
    {
        #region Constructors

        internal FontFile(string filename)
        {
			_reader = new BinaryReader(new FileStream(filename, FileMode.Open));
		}

		
		internal FontFile(Uri uri)
		{
			_reader = new BinaryReader(new FileStream(uri.LocalPath, FileMode.Open));
        }

        #endregion Constructors

        #region Private Methods

        internal void Close()
        {
            _reader.BaseStream.Close();
            _reader.Close();
        }

        private static string FeatureDescription(string feature)
        {
            switch (feature)
            {
                case "aalt":  return "Access All Alternates";
                case "abvf":  return "Above-base Forms";
                case "abvm":  return "Above-base Mark Positioning";
                case "abvs":  return "Above-base Substitutions";
                case "afrc":  return "Alternative Fractions";
                case "akhn":  return "Akhands";
                case "blwf":  return "Below-base Forms";
                case "blwm":  return "Below-base Mark Positioning";
                case "blws":  return "Below-base Substitutions";
                case "calt":  return "Contextual Alternates";
                case "case":  return "Case-Sensitive Forms";
                case "ccmp":  return "Glyph Composition / Decomposition";
                case "clig":  return "Contextual Ligatures";
                case "cpsp":  return "Capital Spacing";
                case "cswh":  return "Contextual Swash";
                case "curs":  return "Cursive Positioning";
                case "c2sc":  return "Small Capitals From Capitals";
                case "c2pc":  return "Petite Capitals From Capitals";
                case "dist":  return "Distances";
                case "dlig":  return "Discretionary Ligatures";
                case "dnom":  return "Denominators";
                case "expt":  return "Expert Forms";
                case "falt":  return "Final Glyph on Line Alternates";
                case "fin2":  return "Terminal Forms #2";
                case "fin3":  return "Terminal Forms #3";
                case "fina":  return "Terminal Forms";
                case "frac":  return "Fractions";
                case "fwid":  return "Full Widths";
                case "half":  return "Half Forms";
                case "haln":  return "Halant Forms";
                case "halt":  return "Alternate Half Widths";
                case "hist":  return "Historical Forms";
                case "hkna":  return "Horizontal Kana Alternates";
                case "hlig":  return "Historical Ligatures";
                case "hngl":  return "Hangul";
                case "hojo":  return "Hojo Kanji Forms (JIS X 0212-1990 Kanji Forms)";
                case "hwid":  return "Half Widths";
                case "init":  return "Initial Forms";
                case "isol":  return "Isolated Forms";
                case "ital":  return "Italics";
                case "jalt":  return "Justification Alternates";
                case "jp78":  return "JIS78 Forms";
                case "jp83":  return "JIS83 Forms";
                case "jp90":  return "JIS90 Forms";
                case "jp04":  return "JIS2004 Forms";
                case "kern":  return "Kerning";
                case "lfbd":  return "Left Bounds";
                case "liga":  return "Standard Ligatures";
                case "ljmo":  return "Leading Jamo Forms";
                case "lnum":  return "Lining Figures";
                case "locl":  return "Localized Forms";
                case "mark":  return "Mark Positioning";
                case "med2":  return "Medial Forms #2";
                case "medi":  return "Medial Forms";
                case "mgrk":  return "Mathematical Greek";
                case "mkmk":  return "Mark to Mark Positioning";
                case "mset":  return "Mark Positioning via Substitution";
                case "nalt":  return "Alternate Annotation Forms";
                case "nlck":  return "NLC Kanji Forms";
                case "nukt":  return "Nukta Forms";
                case "numr":  return "Numerators";
                case "onum":  return "Oldstyle Figures";
                case "opbd":  return "Optical Bounds";
                case "ordn":  return "Ordinals";
                case "ornm":  return "Ornaments";
                case "palt":  return "Proportional Alternate Widths";
                case "pcap":  return "Petite Capitals";
                case "pnum":  return "Proportional Figures";
                case "pref":  return "Pre-Base Forms";
                case "pres":  return "Pre-base Substitutions";
                case "pstf":  return "Post-base Forms";
                case "psts":  return "Post-base Substitutions";
                case "pwid":  return "Proportional Widths";
                case "qwid":  return "Quarter Widths";
                case "rand":  return "Randomize";
                case "rlig":  return "Required Ligatures";
                case "rphf":  return "Reph Forms";
                case "rtbd":  return "Right Bounds";
                case "rtla":  return "Right-to-left alternates";
                case "ruby":  return "Ruby Notation Forms";
                case "salt":  return "Stylistic Alternates";
                case "sinf":  return "Scientific Inferiors";
                case "size":  return "Optical size";
                case "smcp":  return "Small Capitals";
                case "smpl":  return "Simplified Forms";
                case "ss01":  return "Stylistic Set 1";
                case "ss02":  return "Stylistic Set 2";
                case "ss03":  return "Stylistic Set 3";
                case "ss04":  return "Stylistic Set 4";
                case "ss05":  return "Stylistic Set 5";
                case "ss06":  return "Stylistic Set 6";
                case "ss07":  return "Stylistic Set 7";
                case "ss08":  return "Stylistic Set 8";
                case "ss09":  return "Stylistic Set 9";
                case "ss10":  return "Stylistic Set 10";
                case "ss11":  return "Stylistic Set 11";
                case "ss12":  return "Stylistic Set 12";
                case "ss13":  return "Stylistic Set 13";
                case "ss14":  return "Stylistic Set 14";
                case "ss15":  return "Stylistic Set 15";
                case "ss16":  return "Stylistic Set 16";
                case "ss17":  return "Stylistic Set 17";
                case "ss18":  return "Stylistic Set 18";
                case "ss19":  return "Stylistic Set 19";
                case "ss20":  return "Stylistic Set 20";
                case "subs":  return "Subscript";
                case "sups":  return "Superscript";
                case "swsh":  return "Swash";
                case "titl":  return "Titling";
                case "tjmo":  return "Trailing Jamo Forms";
                case "tnam":  return "Traditional Name Forms";
                case "tnum":  return "Tabular Figures";
                case "trad":  return "Traditional Forms";
                case "twid":  return "Third Widths";
                case "unic":  return "Unicase";
                case "valt":  return "Alternate Vertical Metrics";
                case "vatu":  return "Vattu Variants";
                case "vert":  return "Vertical Writing";
                case "vhal":  return "Alternate Vertical Half Metrics";
                case "vjmo":  return "Vowel Jamo Forms";
                case "vkna":  return "Vertical Kana Alternates";
                case "vkrn":  return "Vertical Kerning";
                case "vpal":  return "Proportional Alternate Vertical Metrics";
                case "vrt2":  return "Vertical Alternates and Rotation";
                case "zero":  return "Slashed Zero";
                default: return "";
            }
        }

        private static string LookupTypeDescription(int i)
        {
            switch (i)
            {
                case 1:  return "Single substitution";
                case 2:  return "Multiple substitution";
                case 3:  return "Alternate substitution";
                case 4:  return "Ligature substitution";
                case 5:  return "Contextual substitution";
                case 6:  return "Chaining contextual substitution";
                case 7:  return "Extension substitution";
                case 8:  return "Reverse chaining context single substitution";
                default: return "";
            }
        }

        private int CheckLength(string s1, int l1, string s2, int l2)
        {
            int length = l1;
            if (l2 < l1)
            {
                length = l2;
            }
            if (l1 != l2)
            {
                D.WriteLine("Warning: {0} length {1} != {2} length {3}", s1, l1, s2, l2);
            }
            return length;
        }

        UInt16 ReadUInt16()
        {
            byte b0 = _reader.ReadByte();
            byte b1 = _reader.ReadByte();
            return (UInt16)((b0 << 8) + b1);
        }

        Int16 ReadInt16()
        {
            byte b0 = _reader.ReadByte();
            byte b1 = _reader.ReadByte();
            return (Int16)(((UInt16)b0 << 8) + b1);
        }

        UInt32 ReadUInt32()
        {
            byte b0 = _reader.ReadByte();
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            byte b3 = _reader.ReadByte();
            return (UInt32)((b0 << 24) + (b1 << 16) + (b2 << 8) + b3);
        }

        internal string ReadTag()
        {
            char[] c = _reader.ReadChars(4);
            for (int i=0; i<4; i++)
            {
                if (c[i] == (char)0)
                {
                    c[i] = ' ';
                }
            }
            return new string(c);
        }

        internal struct SfntTable
        {
            internal string tag;
            internal UInt32 checkSum;
            internal UInt32 offset;
            internal UInt32 length;
        };

        private SfntTable ReadSfntTable()
        {
            SfntTable t = new SfntTable();

            t.tag      = ReadTag();
            t.checkSum = ReadUInt32();
            t.offset   = ReadUInt32();
            t.length   = ReadUInt32();
            return t;
        }

        internal struct Sfnt
        {
            internal UInt32 version;
            internal UInt16 numTables;
            internal UInt16 searchRange;
            internal UInt16 entrySelector;
            internal UInt16 rangeShift;
            internal SfntTable[] tableArray;
            internal Hashtable tables;
        };

        internal Sfnt ReadSfnt()
        {
            Sfnt t = new Sfnt();

            t.version       = ReadUInt32();
            t.numTables     = ReadUInt16();
            t.searchRange   = ReadUInt16();
            t.entrySelector = ReadUInt16();
            t.rangeShift    = ReadUInt16();

            t.tableArray = new SfntTable[t.numTables];
            t.tables     = new Hashtable(t.numTables);
            for (int i=0; i<t.numTables; i++)
            {
                t.tableArray[i] = ReadSfntTable();
                t.tables.Add(t.tableArray[i].tag, t.tableArray[i]);
            }

            return t;
        }

        internal struct FeatureEntry
        {
            internal string tag;
            internal UInt16 offset;
            internal UInt16[] lookupList;
        }

        internal struct LookupTable
        {
            internal int absoluteOffset;
            internal UInt16 lookupType;
            internal UInt16 lookupFlag;
            internal UInt16 subtableCount;
            internal UInt16 subtableOffset;
        }

        internal struct GSUB
        {
            internal int gsubOffset;
            internal UInt32 version;
            internal UInt16 scriptListOffset;
            internal UInt16 featureListOffset;
            internal UInt16 lookupListOffset;
            internal UInt16 featureCount;
            internal FeatureEntry[] features;
            internal UInt16[] lookupOffsets;
            internal LookupTable[] lookupTables;
        };

        private UInt16[] ReadUInt16Array()
        {
            UInt16 length = ReadUInt16();
            UInt16[] array = new UInt16[length];
            for (int i=0; i<length; i++)
            {
                array[i] = ReadUInt16();
            }
            return array;
        }

        private UInt16[] ReadCoverage()
        {
            UInt16 format = ReadUInt16();
            UInt16[] coverage = null;
            switch (format)
            {
                case 1:
                    return ReadUInt16Array();

                case 2:
                    UInt16 rangeCount   = ReadUInt16();
                    UInt16[] rangeStart = new UInt16[rangeCount];
                    UInt16[] rangeEnd   = new UInt16[rangeCount];
                    int coverageOffset  = 0;

                    for (int i=0; i<rangeCount; i++)
                    {
                        rangeStart[i] = ReadUInt16();
                        rangeEnd[i] = ReadUInt16();
                        UInt16 startCoverageIndex = ReadUInt16();

                        if (startCoverageIndex != coverageOffset)
                        {
                            D.WriteLine("startCoverageIndex {0} != coverageOffset {1} in range {2}",
                                startCoverageIndex,
                                coverageOffset,
                                i);
                        }
                        coverageOffset += rangeEnd[i] - rangeStart[i] + 1;
                    }

                    coverage = new UInt16[coverageOffset];
                    coverageOffset = 0;
                    for (int i=0; i<rangeCount; i++)
                    {
                        for (UInt16 j=rangeStart[i]; j<=rangeEnd[i]; j++)
                        {
                            coverage[coverageOffset++] = j;
                        }
                    }
                    break;

                default:
                    break;
            }

            return coverage;
        }

        internal GSUB ReadGSUB(int offset)
        {
            _reader.BaseStream.Seek(offset, SeekOrigin.Begin);

            GSUB t = new GSUB();
            t.gsubOffset = offset;

            t.version           = ReadUInt32();
            t.scriptListOffset  = ReadUInt16();
            t.featureListOffset = ReadUInt16();
            t.lookupListOffset  = ReadUInt16();

            // Load feature list

            _reader.BaseStream.Seek(offset + t.featureListOffset, SeekOrigin.Begin);

            t.featureCount = ReadUInt16();
            t.features = new FeatureEntry[t.featureCount];
            for (int i=0; i<t.featureCount; i++)
            {
                t.features[i].tag     = ReadTag();
                t.features[i].offset  = ReadUInt16();
            }

            // Load features
            for (int i=0; i<t.featureCount; i++)
            {
                int featureTableOffset = t.gsubOffset + t.featureListOffset + t.features[i].offset;
                _reader.BaseStream.Seek(featureTableOffset, SeekOrigin.Begin);

                UInt16 featureParams = ReadUInt16();

                t.features[i].lookupList = ReadUInt16Array();

                StringBuilder featureTable = new StringBuilder();
                featureTable.AppendFormat("FeatureTable at {0,8:x8}: featureParams {1,4:x4}, lookup count {2}, feature {3,2} {4,-4}, lookups: ",
                    featureTableOffset,
                    featureParams,
                    t.features[i].lookupList.Length,
                    i,
                    t.features[i].tag);
                if (t.features[i].lookupList.Length > 0)
                {
                    featureTable.AppendFormat("{0,2}", t.features[i].lookupList[0]);
                }
                for (int j=1; j<t.features[i].lookupList.Length; j++)
                {
                    featureTable.AppendFormat(", {0,2}", t.features[i].lookupList[j]);
                }
                D.WriteLine(featureTable.ToString());

                if (featureParams != 0)
                {
                    D.WriteLine("Unexpected featureParams offset {0} in feature table at {1,8:x8}",
                        featureParams,
                        featureTableOffset);
                }
            }

            // Load lookup list

            _reader.BaseStream.Seek(offset + t.lookupListOffset, SeekOrigin.Begin);
            t.lookupOffsets = ReadUInt16Array();

            // Load lookup tables

            t.lookupTables = new LookupTable[t.lookupOffsets.Length];
            for (int i=0; i<t.lookupOffsets.Length; i++)
            {
                _reader.BaseStream.Seek(offset + t.lookupListOffset + t.lookupOffsets[i], SeekOrigin.Begin);

                t.lookupTables[i].absoluteOffset = offset + t.lookupListOffset + t.lookupOffsets[i];
                t.lookupTables[i].lookupType     = ReadUInt16();
                t.lookupTables[i].lookupFlag     = ReadUInt16();
                t.lookupTables[i].subtableCount  = ReadUInt16();
                t.lookupTables[i].subtableOffset = ReadUInt16();
            }
            return t;
        }

        private struct GPOS
        {
            internal UInt32 version;
            internal UInt16 scriptListOffset;
            internal UInt16 featureListOffset;
            internal UInt16 LookupListOffset;
            internal UInt16 featureCount;
            internal FeatureEntry[] features;
        }

        private GPOS ReadGPOS(int Offset)
        {
            _reader.BaseStream.Seek(Offset, SeekOrigin.Begin);

            GPOS t = new GPOS();

            t.version           = ReadUInt32();
            t.scriptListOffset  = ReadUInt16();
            t.featureListOffset = ReadUInt16();
            t.LookupListOffset  = ReadUInt16();

            _reader.BaseStream.Seek(Offset + t.featureListOffset, SeekOrigin.Begin);

            t.featureCount = ReadUInt16();
            t.features = new FeatureEntry[t.featureCount];
            for (int i=0; i<t.featureCount; i++)
            {
                t.features[i].tag    = ReadTag();
                t.features[i].offset = ReadUInt16();
            }

            return t;
        }

        internal class GlyphString
        {
            private UInt16[] _glyphs;

            internal GlyphString(UInt16[] glyphs)
            {
                _glyphs = new UInt16[glyphs.Length];
                glyphs.CopyTo(_glyphs, 0);
            }

            public override int GetHashCode()
            {
                int hashcode = 0;
                for (int i=0; i<_glyphs.Length; i++)
                {
                    hashcode = hashcode * 17 + _glyphs[i];
                }
                return hashcode;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is GlyphString))
                    return false;

                if (((GlyphString)(obj))._glyphs.Length != _glyphs.Length)
                    return false;

                for (int i=0; i<_glyphs.Length; i++)
                {
                    if (((GlyphString)(obj))._glyphs[i] != _glyphs[i])
                        return false;
                }

                return true;
            }

            internal bool Matches(IList<ushort>glyphs, int index)
            {
                for (int i=0; i<_glyphs.Length; i++)
                {
                    if (    index >= glyphs.Count
                        ||  _glyphs[i] != glyphs[index])
                    {
                        return false;
                    }
                    index++;
                }
                return true;
            }
        }

        ////    Lookups are stored in format optimized for matching against incoming glyph strings
        ///
        ///     The root of the structure is a Hashtable indexed by glyph index - the first glyph of the substition pattern
        ///     Hashtable entries are ArrayLists of substitutions
        ///     Each substitution in the ArrayList contains the pattern (UInt16[]) and replacement (ArrayList of UInt16[]'s)
        internal class Substitution
        {
            internal GlyphString Pattern;
            internal ArrayList Replacement;

            internal Substitution(UInt16[] pattern, ArrayList replacement)
            {
                Pattern     = new GlyphString(pattern);
                Replacement = replacement;
            }
        }

        private static void AddLookup(Hashtable lookupRoot, UInt16[] pattern, ArrayList replacement)
        {
            ArrayList substitutions;

            if (lookupRoot.ContainsKey(pattern[0]))
            {
                substitutions = (ArrayList)(lookupRoot[pattern[0]]);
            }
            else
            {
                substitutions = new ArrayList();
                lookupRoot.Add(pattern[0], substitutions);
            }
            substitutions.Add(new Substitution(pattern, replacement));
        }

        private static UInt16[] newUInt16Array(UInt16 val)
        {
            UInt16[] array = new UInt16[1];
            array[0] = val;
            return array;
        }

        private static ArrayList newArrayOfUInt16Array(UInt16[] array)
        {
            ArrayList arrayList = new ArrayList(1);
            arrayList.Add(array);
            return arrayList;
        }

        private void ReadSingleSubstitutionCalculatedGlyphIndices(int subtableOffset, ref Hashtable lookupRoot)
        {
            UInt16 coverageOffset = ReadUInt16();
            Int16  deltaGlyphId   = ReadInt16();

            _reader.BaseStream.Seek(subtableOffset + coverageOffset, SeekOrigin.Begin);

            UInt16[] coverage = ReadCoverage();

            for (int i=0; i<coverage.Length; i++)
            {
                D.WriteLine("--- Map {0,4:x4} to {1,4:x4}", coverage[i], coverage[i]+deltaGlyphId);
                if (deltaGlyphId != 0)
                {
                    AddLookup(lookupRoot, newUInt16Array(coverage[i]), newArrayOfUInt16Array(newUInt16Array((UInt16)(coverage[i]+deltaGlyphId))));
                }
            }
        }

        private void ReadSingleSubstitutionSpecifiedGlyphIndices(int subtableOffset, ref Hashtable lookupRoot)
        {
            UInt16   coverageOffset = ReadUInt16();
            UInt16[] substitutions  = ReadUInt16Array();

            _reader.BaseStream.Seek(subtableOffset + coverageOffset, SeekOrigin.Begin);
            UInt16[] coverage = ReadCoverage();

            int length = CheckLength("coverage", coverage.Length, "substitutions", substitutions.Length);
            for (int i=0; i<length; i++)
            {
                D.WriteLine("--- Map {0,4:x4} to {1,4:x4}", coverage[i], substitutions[i]);

                if (coverage[i] != substitutions[i])
                {
                    AddLookup(lookupRoot, newUInt16Array(coverage[i]), newArrayOfUInt16Array(newUInt16Array((UInt16)(substitutions[i]))));
                }
            }
        }

        private void ReadMultipleSubstitution(int subtableOffset, ref Hashtable lookupRoot)
        {
            UInt16 coverageOffset = ReadUInt16();

            UInt16[] sequenceOffsets = ReadUInt16Array();

            // Load the coverage
            _reader.BaseStream.Seek(subtableOffset + coverageOffset, SeekOrigin.Begin);
            UInt16[] coverage = ReadCoverage();

            // Walk through all the sequences
            int length = CheckLength("coverage", coverage.Length, "sequenceOffsets", sequenceOffsets.Length);
            for (int i=0; i<length; i++)
            {
                _reader.BaseStream.Seek(subtableOffset + sequenceOffsets[i], SeekOrigin.Begin);

                StringBuilder sequence = new StringBuilder();
                sequence.AppendFormat("--- Map {0,4:x4} to (", coverage[i]);
                UInt16[] newGlyphs = ReadUInt16Array();
                if (newGlyphs.Length > 0)
                {
                    sequence.AppendFormat("{0,4:x4}", newGlyphs[0]);
                }
                for (int j=1; j<newGlyphs.Length; j++)
                {
                    sequence.AppendFormat(", {0,4:x4}", newGlyphs[j]);
                }
                sequence.Append(")");
                D.WriteLine(sequence.ToString());

                AddLookup(lookupRoot, newUInt16Array(coverage[i]), newArrayOfUInt16Array(newGlyphs));
            }
        }

        private void ReadAlternateSubstitution(int subtableOffset, ref Hashtable lookupRoot)
        {
            UInt16   coverageOffset      = ReadUInt16();
            UInt16[] alternateSetOffsets = ReadUInt16Array();

            // Load the coverage
            _reader.BaseStream.Seek(subtableOffset + coverageOffset, SeekOrigin.Begin);
            UInt16[] coverage = ReadCoverage();

            // Walk through all the alternates
            int length = CheckLength("coverage", coverage.Length, "alternateSetOffsets", alternateSetOffsets.Length);
            for (int i=0; i<length; i++)
            {
                _reader.BaseStream.Seek(subtableOffset + alternateSetOffsets[i], SeekOrigin.Begin);

                StringBuilder sequence = new StringBuilder();
                sequence.AppendFormat("--- Alternatives for {0,4:x4} are (", coverage[i]);
                UInt16[] alternates = ReadUInt16Array();
                ArrayList alternateGlyphStrings = new ArrayList(alternates.Length);

                for (int j=0; j<alternates.Length; j++)
                {
                    UInt16[] alternateGlyphString = new UInt16[1];
                    alternateGlyphString[0] = alternates[j];
                    alternateGlyphStrings.Add(alternateGlyphString);
                }
                AddLookup(lookupRoot, newUInt16Array(coverage[i]), alternateGlyphStrings);

                if (alternates.Length > 0)
                {
                    sequence.AppendFormat("{0,4:x4}", alternates[0]);
                }
                for (int j=1; j<alternates.Length; j++)
                {
                    sequence.AppendFormat(", {0,4:x4}", alternates[j]);
                }
                sequence.Append(")");
                D.WriteLine(sequence.ToString());
            }
        }

        private void ReadLigatureSubstitution(int subtableOffset, ref Hashtable lookupRoot)
        {
            UInt16   coverageOffset     = ReadUInt16();
            UInt16[] ligatureSetOffsets = ReadUInt16Array();

            // Load the coverage
            _reader.BaseStream.Seek(subtableOffset + coverageOffset, SeekOrigin.Begin);
            UInt16[] coverage = ReadCoverage();

            // Walk through all the ligature sets
            int length = CheckLength("coverage", coverage.Length, "ligatureSetOffsets", ligatureSetOffsets.Length);
            for (int i=0; i<length; i++)
            {
                _reader.BaseStream.Seek(subtableOffset + ligatureSetOffsets[i], SeekOrigin.Begin);

                // Load tables of per-ligature details

                UInt16[] ligatureOffsets = ReadUInt16Array();

                // Process each ligature

                for (int j=0; j<ligatureOffsets.Length; j++)
                {
                    _reader.BaseStream.Seek(subtableOffset + ligatureSetOffsets[i] + ligatureOffsets[j], SeekOrigin.Begin);

                    UInt16 ligatureGlyph  = ReadUInt16();
                    UInt16 componentCount = ReadUInt16();
                    UInt16[] oldGlyphs = new UInt16[componentCount];

                    StringBuilder mapping = new StringBuilder();
                    mapping.AppendFormat("--- Map ({0,4:x4}", coverage[i]);
                    oldGlyphs[0] = coverage[i];

                    for (int k=1; k<componentCount; k++)
                    {
                        oldGlyphs[k] = ReadUInt16();
                        mapping.AppendFormat(", {0,4:x4}", oldGlyphs[k]);
                    }
                    mapping.AppendFormat(") to {0,4:x4}", ligatureGlyph);
                    D.WriteLine(mapping.ToString());

                    AddLookup(lookupRoot, oldGlyphs, newArrayOfUInt16Array(newUInt16Array(ligatureGlyph)));
                }
            }
        }

        internal Hashtable LoadGsubLookup(GSUB gsub, UInt16 lookupIndex)
        {
            D.WriteLine("Lookup {0,2}, type {1,4:x4} {2,-32} flag {3,4:x4}, subtable count: {4,2}, offset: {5,4:x4}",
                lookupIndex,
                gsub.lookupTables[lookupIndex].lookupType,
                LookupTypeDescription(gsub.lookupTables[lookupIndex].lookupType),
                gsub.lookupTables[lookupIndex].lookupFlag,
                gsub.lookupTables[lookupIndex].subtableCount,
                gsub.lookupTables[lookupIndex].subtableOffset);

            int subtableOffset = gsub.lookupTables[lookupIndex].absoluteOffset + gsub.lookupTables[lookupIndex].subtableOffset;

            _reader.BaseStream.Seek(subtableOffset, SeekOrigin.Begin);

            UInt16 substFormat;

            Hashtable glyphSubstitutions = new Hashtable(10);

            switch (gsub.lookupTables[lookupIndex].lookupType)
            {
                case 1:
                    // Single substitution
                    substFormat = ReadUInt16();
                    D.WriteLine("Dump single substitution, subst format {0}", substFormat);

                    switch (substFormat)
                    {
                        case 1:   ReadSingleSubstitutionCalculatedGlyphIndices(subtableOffset, ref glyphSubstitutions);  break;
                        case 2:   ReadSingleSubstitutionSpecifiedGlyphIndices(subtableOffset, ref glyphSubstitutions);   break;
                        default:  D.WriteLine("** Unrecognised single substitution subtable format {0}", substFormat);   break;
                    }
                    break;

                case 2:
                    // Multiple substitution
                    substFormat = ReadUInt16();
                    D.WriteLine("Dump multiple substitution, subst format {0}", substFormat);

                    switch (substFormat)
                    {
                        case 1:   ReadMultipleSubstitution(subtableOffset, ref glyphSubstitutions);                      break;
                        default:  D.WriteLine("** Unrecognised multiple substitution subtable format {0}", substFormat); break;
                    }
                    break;

                case 3:
                    // Alternate substitution
                    substFormat = ReadUInt16();
                    D.WriteLine("Dump alternate substitution, subst format {0}", substFormat);

                    switch (substFormat)
                    {
                        case 1:   ReadAlternateSubstitution(subtableOffset, ref glyphSubstitutions);                       break;
                        default:  D.WriteLine("** Unrecognised alternate substitution subtable format {0}", substFormat);  break;
                    }
                    break;

                case 4:
                    // Ligature substitution
                    substFormat = ReadUInt16();
                    D.WriteLine("Dump ligature substitution, subst format {0}", substFormat);

                    switch (substFormat)
                    {
                        case 1:   ReadLigatureSubstitution(subtableOffset, ref glyphSubstitutions);                       break;
                        default:  D.WriteLine("** Unrecognised ligature substitution subtable format {0}", substFormat);  break;
                    }
                    break;

                default:
                    D.WriteLine("** Unimplemented gsub lookup type {0}", gsub.lookupTables[lookupIndex].lookupType);
                    break;
            }

            return glyphSubstitutions;
        }

        private void DumpSubstitutions(GSUB gsub, string feature)
        {
            D.WriteLine("Dumping lookups for feature {0} - {1}", feature, FeatureDescription(feature));
            SortedList usedIndices = new SortedList(10);

            for (int i=0; i<gsub.featureCount; i++)
            {
                if (gsub.features[i].tag == feature)
                {
                    int featureTableOffset = gsub.gsubOffset + gsub.featureListOffset + gsub.features[i].offset;
                    _reader.BaseStream.Seek(featureTableOffset, SeekOrigin.Begin);

                    for (int j=0; j<gsub.features[i].lookupList.Length; j++)
                    {
                        UInt16 index = ReadUInt16();
                        if (!usedIndices.ContainsKey(gsub.features[i].lookupList[j]))
                        {
                            usedIndices.Add(gsub.features[i].lookupList[j],0);
                        }
                    }
                }
            }
            for (int j=0; j<usedIndices.Count; j++)
            {
                LoadGsubLookup(gsub, (UInt16)(usedIndices.GetKey(j)));
            }
        }

        #endregion Private Methods

        #region Private Fields

        private BinaryReader _reader;

        #endregion Private Fields
    }

    internal class TypographyFeature
    {
        #region Internal Methods

        internal static ArrayList GetTypographyFeatures(GlyphRun glyphRun)
        {
            return GetTypographyFeatures(
                glyphRun.GlyphTypeface.FontUri,
                glyphRun.GlyphIndices,
                glyphRun.ClusterMap);
        }

        #endregion Internal Methods

        #region Private Methods

        public override bool Equals(object obj)
        {
            TypographyFeature otherFeature = (TypographyFeature)obj;

            return  FeatureName    == otherFeature.FeatureName
                &&  StartCharacter == otherFeature.StartCharacter
                &&  CharacterCount == otherFeature.CharacterCount
                &&  AlternateCount == otherFeature.AlternateCount;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode ();
        }

        internal static void GlyphClusterToCharacterCluster(
            int       gs,
            int       gl,
            out int   cs,
            out int   cl,
            IList<ushort>  characterMap)
        {
            if (characterMap == null)
            {
                // Easy case: Glyphs and characters map 1:1
                cs = gs;
                cl = gl;
                return;
            }
            
            // There is a cluster map. Walk it to find the character positions corresponding 
            // to the input glyph positions.
            
            int c=0;

            while (    c < characterMap.Count
                   &&  characterMap[c] <= gs)
            {
                c++;
            }

            // Last character of target cluster is at c-1

            if (c == 0)
            {
                // This shouldn't happen (or I don't understand the characterMap contract correctly)
                D.WriteLine("First entry {0} in character map is non-zero", characterMap[0]);
                c = 1;
            }


            // Find first character of target cluster

            cs = c-1;
            while (    cs > 0
                   &&  characterMap[cs-1] == characterMap[cs])
            {
                cs--;
            }

            // cs points at first character of cluster containing g.

            cl = c-cs;
        }

        internal static ArrayList GetTypographyFeatures(Uri fontUri, IList<ushort> glyphs, IList<ushort> characterMap)
        {
            ArrayList features = new ArrayList();
            FontFile font = new FontFile(fontUri);
            FontFile.Sfnt sfnt = font.ReadSfnt();
            if (sfnt.tables.ContainsKey("GSUB"))
            {
                FontFile.GSUB gsub = new FontFile.GSUB();
                int GsubOffset = (int)((FontFile.SfntTable)(sfnt.tables["GSUB"])).offset;
                gsub = font.ReadGSUB(GsubOffset);

                // Load the various kinds of lookup
                Hashtable[] Lookups = new Hashtable[gsub.lookupTables.Length];

                for (UInt16 i=0; i<gsub.lookupTables.Length; i++)
                {
                    Lookups[i] = font.LoadGsubLookup(gsub, i);
                }

                // Go through lookup by lookup looking for lookups that will trigger on this glyph string

                for (UInt16 i=0; i<gsub.lookupTables.Length; i++)
                {
                    for (int g=0; g<glyphs.Count; g++)
                    {
                        // Need to handle multi-glyph substitutions. Needs significant reorg.
                        if (Lookups[i].ContainsKey(glyphs[g]))
                        {
                            // There is at least one lookup starting with this glyph

                            ArrayList substitutions = (ArrayList)(Lookups[i][glyphs[g]]);

                            for (int s=0; s<substitutions.Count; s++)
                            {
                                // Search substitutions for one that matches glyps starting at g

                                FontFile.Substitution substitution = (FontFile.Substitution)(substitutions[s]);

                                if (substitution.Pattern.Matches(glyphs, g))
                                {
                                    // Add any features using this lookup to the switchableFeature dictionary
                                    for (int f=0; f<gsub.features.Length; f++)
                                    {
                                        for (int l=0; l<gsub.features[f].lookupList.Length; l++)
                                        {
                                            if (gsub.features[f].lookupList[l] == i)
                                            {
                                                D.WriteLine("match: lookupindex {0} glyph offset {1} glyph {2} feature {3} feature lookup {4}",
                                                    i,
                                                    g,
                                                    glyphs[g],
                                                    f,
                                                    l);

                                                // We found a feature that applies to this glyph
                                                TypographyFeature feature = new TypographyFeature();

                                                feature.FeatureName = gsub.features[f].tag;

                                                GlyphClusterToCharacterCluster(
                                                    g, 1,
                                                    out feature.StartCharacter, out feature.CharacterCount,
                                                    characterMap);

                                                feature.AlternateCount = ((ArrayList)(substitution.Replacement)).Count;

                                                if (!features.Contains(feature))
                                                {
                                                    features.Add(feature);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            font.Close();
            return features;
        }

        #endregion Private Methods

        #region Internal Fields

        internal string FeatureName;     // four character abbreviation
        internal int StartCharacter;  // offset of a character combination in source GlyphRun
        internal int CharacterCount;  // number of unicode charatcres representing a glyph
        internal int AlternateCount;  // number of typographic alternates available for this glyph with this feature
        // AlternateCount=1 means that the feature may be a switch or a (single) alternative
        // AlternateCount>1 means that the feature must be multiple alternative

        #endregion Internal Fields
    }
}