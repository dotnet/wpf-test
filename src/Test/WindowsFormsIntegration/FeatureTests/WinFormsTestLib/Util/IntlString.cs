// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Configuration.Assemblies;

using ArrayList = System.Collections.ArrayList;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Contains routines to support generation of international strings.
    // </desc>
    // </doc>
    public class IntlString
    {
        // Some handy Code Page constants

        // <doc>
        // <desc>
        //   Simplified Chinese CodePage
        // <desc>
        // </doc>
        public const int CP_SIMPLIFIED_CHINESE  = 936;

        // <doc>
        // <desc>
        //   Traditional Chinese CodePage
        // </desc>
        // </doc>
        public const int CP_TRADITIONAL_CHINESE = 950;

        // <doc>
        // <desc>
        //   Japanese CodePage
        // </desc>
        // </doc>
        public const int CP_JAPANESE            = 932;

        // <doc>
        // <desc>
        //   Korean CodePage
        // </desc>
        // </doc>
        public const int CP_KOREAN_WANSUNG      = 949;

        // <doc>
        // <desc>
        //   United States of America CodePage
        // </desc>
        // </doc>
        public const int CP_USA                 = 1252;

        // <doc>
        // <desc>
        //  The sections in the .ini file that I can't find anywhere in the tree
        // </desc>
        // <seealso class="PresetStringType"/>
        // </doc>
        internal string[] Sections = new string[] {"boundary", "directory"};

        // <doc>
        // <desc>
        //  The prefix to the codepage string storing file
        // </desc>
        // </doc>
        internal String PREFIX    = "WFC\\AutoPME\\cp";

        // <doc>
        // <desc>
        //  The extension of the codepage string storing file
        // </desc>
        // </doc>
        internal String EXTENSION = ".UNI";

        // <doc>
        // <desc>
        //  The name of the preset file containing boundary strings
        // </desc>
        // </doc>
        internal String sPresetFile;

        // <doc>
        // <desc>
        //  The random number generator
        //  KevinTao 4/12/00: Made it static so all instances share the same
        //                    seed value.
        // </desc>
        // </doc>
        internal static Random rand;

        // <doc>
        // <desc>
        //  The codepage we are testing for.
        // </desc>
        // </doc>
        private int    _codePage;

        // <doc>
        // <desc>
        //  A list of unicode character ranges
        // </desc>
        // </doc>
        private System.Collections.ArrayList   _lUnicodeCharRanges;

        // <doc>
        // <desc>
        //  Constructs a new IntlString object
        // </desc>
        // <param term="codePage">
        //  The codepage to muck-about in. If it is one that is unknown
        //  to this library, you'll end up using the USA codepage.
        // </param>
        // <param term="rand">
        //  The random number generator to use
        // </param>
        // <doc>
        public IntlString(int codePage, Random rand)
        {
            IntlString.rand = rand;
            this._codePage = codePage;

            MakeByteRanges();
        }

        // <doc>
        // <desc>
        //  Calculates a random number in between nMin and nMax inclusive.
        // </desc>
        // <param term="nMin">
        //  The minimuim value.
        // </param>
        // <param term="nMax">
        //  The maximum value.
        // </param>
        // <retvalue>
        //  A random number between the specified minimum and maximum values
        // </retvalue>
        // <seealso class="IntlString" member="GetRange"/>
        // </doc>
        public int GetRange(int nMin, int nMax)
        {
            int nRandom;
            double dRand;
            long lDiff;

            if (nMin > nMax)
            {
                int nTemp = nMin;
                nMin = nMax;
                nMax = nTemp;
            }

            if ((nMin == Int32.MinValue) || (nMin == Int32.MaxValue))
                nMin /= 2;
            if ((nMax == Int32.MinValue) || (nMax == Int32.MaxValue))
                nMax /= 2;

            lDiff = Math.Abs(nMax - nMin);
            dRand = rand.NextDouble();
            nRandom = (int)((long)Math.Round(dRand * lDiff) + nMin);

            return nRandom;
        }


        // <doc>
        // <desc>
        //  Get a random UNICode char in the range.  If the char doesn't appear,
        //  you might need a different font (or it might not be printable).
        // </desc>
        // <retvalue>
        //  A random Unicode character in the range for this codepage
        // </retvalue>
        // </doc>
        public virtual char GetChar()
        {
            return this.GetChar(false);
        }

        // <doc>
        // <desc>
        //  Determines if the supplied unicode char is printable.  Right now
        //  it only checks to see that the char is > ' '.  Might need some more
        //  fancy criteria for internation non-printable chars if there is such
        //  a thing.
        // <desc>
        // <param term="c">
        //  The character to determine the printibility for.
        // </param>
        // <retvalue>
        //  True if the character is printable, false otherwise
        // </retvalue>
        // </doc>
        public virtual bool GetPrintable(char c)
        {
            return (c >= ' ');
        }

        // <doc>
        // <desc>
        //  Get a random UNICode char in the range that is also printable.
        // </desc>
        // <param term="bPrintableOnly">
        //  Specifies whether the character generated must be printable.
        // </param>
        // <retvalue>
        //  A unicode character in the range for the required codepage that
        //  is optionally printable.
        // </retvalue>
        // </doc>
        public virtual char GetChar(bool bPrintableOnly)
        {
            int nRange       = this.GetRange(0, _lUnicodeCharRanges.Count - 1);
            ByteRange range  = (ByteRange)(_lUnicodeCharRanges[nRange]);

            //Vinay added this code to make it work with Win95 & Win98
            OperatingSystem os = System.Environment.OSVersion ;

            char c;
            if ( os.Platform == PlatformID.Win32Windows )
                c = (char)(GetRange(65,91));
            else
                c = (char)(GetRange(range.start, range.end));

            while (bPrintableOnly && !GetPrintable(c))
                c = (char)(GetRange(range.start, range.end));

            return c;
        }

        // <doc>
        // <desc>
        //  Creates a random string of max length n.  Using leadByte + trailByte
        //  combinations according to the ranges for the code page in use.
        // </desc>
        // <param term="nMaxLength">
        //  max length of string to generate
        // </param>
        // <retvalue>
        //  new string with random chars
        // </retvalue>
        // </doc>
        public virtual String GetString(int nMaxLength)
        {
            return this.GetString(nMaxLength, false);
        }

        // <doc>
        // <desc>
        //  Creates a random string of max length n.  Using leadByte + trailByte
        //  combinations according to the ranges for the code page in use.
        // </desc>
        // <param term="nMaxLength">
        //  max length of string to generate
        // </param>
        // <param term="bPrintableOnly">
        //  true if you only want printable chars
        // </param>
        // <retvalue>
        //  new string with random chars
        // </retvalue>
        // </doc>
        public virtual String GetString(int nMaxLength, bool bPrintableOnly)
        {
            int nLength = this.GetRange(1, nMaxLength);
            char[] cArray = new char [nLength];

            for (int n = 0; n < nLength; n++)
                cArray [n] = GetChar(bPrintableOnly);

            String s = new String(cArray);

            return s;
        }

        // <doc>
        // <desc>
        //  Add common ranges for all FE languages
        // </desc>
        // </doc>
        internal virtual void AddRangesForAllFE()
        {
            _lUnicodeCharRanges.Add(new ByteRange(0x0000, 0x007F));
            _lUnicodeCharRanges.Add(new ByteRange(0x3000, 0x303F));
            _lUnicodeCharRanges.Add(new ByteRange(0x4E00, 0x9FFF));
        }

        // <doc>
        // <desc>
        //  Build our code page data structure based on the specified code page.
        //  Only data for the specified code page is loaded.
        // </desc>
        // </doc>
        internal virtual void MakeByteRanges()
        {
            sPresetFile = Directory.GetCurrentDirectory() + "\\" + PREFIX + _codePage.ToString() + EXTENSION;

            // if (!File.Exists(sPresetFile))
            //     Text.Error.WriteLine("IntlString: can't find preset string file: " + sPresetFile);

            _lUnicodeCharRanges = new System.Collections.ArrayList();

            switch (_codePage)
            {
                case CP_SIMPLIFIED_CHINESE:
                case CP_TRADITIONAL_CHINESE:
                    AddRangesForAllFE();
                    _lUnicodeCharRanges.Add(new ByteRange(0x3100, 0x312f));
                    break;

                case CP_JAPANESE:
                    AddRangesForAllFE();
                    _lUnicodeCharRanges.Add(new ByteRange(0x3041, 0x309E));
                    _lUnicodeCharRanges.Add(new ByteRange(0x30A0, 0x30FF));
                    break;

                case CP_KOREAN_WANSUNG:
                    AddRangesForAllFE();
                    _lUnicodeCharRanges.Add(new ByteRange(0x1100, 0x11FF));
                    _lUnicodeCharRanges.Add(new ByteRange(0x3130, 0x318F));
                    _lUnicodeCharRanges.Add(new ByteRange(0xAC00, 0xD7A3));
                    break;

                // if this isn't one of the other code pages, default to USA.
                default:
                if (this._codePage != CP_USA)
                {
#if Debug
                    Console.Error.WriteLine("IntlString: using default USA code page");
#endif
                    this._codePage = CP_USA;
                }
                _lUnicodeCharRanges.Add(new ByteRange(0x0000, 0x00FF));
                break;
            }
       }

        // <doc>
        // <desc>
        //  Helper class for holding start/end points in unicode char ranges.
        // </desc>
        // </doc>
        public class ByteRange
        {
            // <doc>
            // <desc>
            //  The beginning index of the byte range
            // </desc>
            // </doc>
            public int start;

            // <doc>
            // <desc>
            //  The ending index of the byte range
            // </desc>
            // </doc>
            public int end;

            // <doc>
            // <desc>
            //  Constructs a new ByteRange. Ensures start < end.
            // </desc>
            // <param term="start">
            //  The beginning of the ByteRange
            // </param>
            // <param term="end">
            //  The ending of the ByteRange
            // </param>
            // </doc>
            public ByteRange(int start, int end)
            {
                this.start = Math.Min(start, end);
                this.end   = Math.Max(start, end);
            }
        }
    }
}
