// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//$comment: allot of this was ripped from Anton's wcltest app.

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Common
{
    class KCharConst // Some character constants
    {
        /// <summary>
        /// FIsCharSpaceEolnTab
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool FIsCharSpaceEolnTab(char ch)
        { return (ch == ' ' || ch == eoln1 || ch == eoln2 || ch == tab); }

        /// <summary>
        /// eoln1
        /// </summary>
        public static char eoln1
        {
            get
            {
                return '\x0D';
            }
        }

        /// <summary>
        /// eoln2
        /// </summary>
        public static char eoln2
        {
            get
            {
                return '\x0A'; 
            }
        }

        /// <summary>
        /// tab
        /// </summary>
        public static char tab
        {
            get
            {
                return '\x09'; 
            }
        }

        /// <summary>
        /// zero
        /// </summary>
        public static char zero
        {
            get
            {
                return '\x00';
            }
        }
    };	

    // Static class with service functions
    class Common 
    {
        public static Random random = new Random();

        public static void Assert(bool f, string str)
        {
            if (!f)
            {

                Debug.Assert(f, str);

            };
        }

        public static void Assert(bool f)
        {
            if (!f) Assert(f, "");
        }

        // FCompareStr
        // Compares strings without case
        public static int CompareStr(string s1, string s2)
        {
            return string.Compare(s1, s2, true);
        }

        // PackMultiLineString 
        // Compares strings without case
        public static string PackMultiLineString(string strM)
        {
            string strComp = ""; ;
            int i;

            for (i = 0; i < strM.Length; i++)
            {
                if (strM[i] == KCharConst.eoln1) strComp += "^D";
                else if (strM[i] == KCharConst.eoln2) strComp += "^A";
                else strComp += strM[i];
            };

            return strComp;
        }


        // PackMultiLineString 
        // Compares strings without case
        public static string UnpackMultiLineString(string strComp)
        {
            string strM = ""; ;
            int i;

            for (i = 0; i < strComp.Length; i++)
            {
                if (strComp[i] == '^' && i + 1 < strComp.Length && strComp[i + 1] == 'D')
                    strM += KCharConst.eoln1;
                else if (strComp[i] == '^' && i + 1 < strComp.Length && strComp[i + 1] == 'A')
                    strM += KCharConst.eoln2;
                else
                    strM += strComp[i];
            };

            return strM;
        }

        // Reads names of subfolders
        public static string[] ReadDirectotyNames(string spath, string sfnwildcard)
        {
            long i;

            string[] rgspathSub;

            if (!Directory.Exists(spath))
            {
                return new string[0];
            };

            rgspathSub = Directory.GetDirectories(spath, sfnwildcard);

            for (i = 0; i < rgspathSub.Length; i++)
            {
                rgspathSub[i] = Path.GetFileName(rgspathSub[i]);
            }

            return rgspathSub;
        }

        // Reads names of files
        public static string[] ReadFileNames(string spath, string sfnwildcard)
        {
            long i;

            string[] rgsfn;

            if (!Directory.Exists(spath))
            {
                return new string[0];
            };

            rgsfn = Directory.GetFiles(spath, sfnwildcard);

            for (i = 0; i < rgsfn.Length; i++)
            {
                rgsfn[i] = Path.GetFileName(rgsfn[i]);
            }

            return rgsfn;
        }

        // Delete folder, handle error
        static public void DeleteFolder(string spath)
        {
            try
            {
                if (Directory.Exists(spath))
                {
                    Directory.Delete(spath, true);
                };
            }
            catch (Exception)
            {
/*                W11Messages.RaiseError("Unable to delete folder " + spath +
                    "\nProbably it is used by another process or has read-only files" +
                    "\n\n" + ex.ToString());*/
            }
        }

        // Delete folder, handle error
        static public void RenameFolder(string pathold, string pathnew)
        {
            try
            {
                Directory.Move(pathold, pathnew);
            }
            catch (Exception)
            {
               /* W11Messages.RaiseError("Unable to rename folder " + pathold + " => " + pathnew +
                    "\nProbably the folder does not exist or it is used by another process" +
                    "\n\n" + ex.ToString());*/
            }
        }

        // Reads given number of lines from stream        
        public static string[] ReadLinesFromStream(StreamReader stream, long nlines)
        {
            int i = 0;
            string[] arrlines = new string[nlines];

            while (i < nlines && stream.Peek() != -1)
            {
                arrlines[i] = stream.ReadLine();
                i++;
            }

            if (i == nlines) return arrlines;
            else
            {
                string[] arrlinesT = new string[i];
                int j;
                for (j = 0; j < i; j++) arrlinesT[j] = arrlines[j];

                return arrlinesT;
            };
        }

        // Split string by given character (private)
        public static bool FSplitString(string s, char chr, out string s1, out string s2)
        {
            int i = 0;
            while (i < s.Length && s[i] != chr) i++;
            s1 = null;
            s2 = null;
            if (i == s.Length) return false;
            else
            {
                s1 = s.Substring(0, i);
                s2 = s.Substring(i + 1, s.Length - i - 1);
                return true;
            }
        }

        // GetTemporaryDocumentName
        public static string GetTemporaryDocumentName(string spathDoc)
        {
            string strDocnameTemp;
            int index = random.Next();

            do
            {
                strDocnameTemp = "~$-DBC-" + index.ToString() + ".doc";
                index++;
            } while (File.Exists(Path.Combine(spathDoc, strDocnameTemp)));

            return strDocnameTemp;
        }


        // FTemporaryDocument
        public static bool FTemporaryDocument(string sfnDoc)
        {
            string strFileName = Path.GetFileName(sfnDoc);

            return (strFileName.Length >= 2 && strFileName[0] == '~' && strFileName[1] == '$');
        }

        // StrToInt with error check, but no check
        // for overflow
        public static bool ParseStrToInt(string s, out int lout)
        {
            int ich = 0;
            int len = s.Length;
            int lresult;
            bool fNegative = false;

            lout = System.Int32.MaxValue;

            /* Skip tabs and white spaces */
            while (ich < len && (s[ich] == ' ' || s[ich] == 9)) ich++;

            /* Must have content */
            if (ich == len) return false;

            if (s[ich] == '-')
            {
                fNegative = true;
                ich++;
            };

            /* Number must follow */
            if (ich == len || s[ich] < '0' || s[ich] > '9') return false;

            lresult = 0;

            while (ich < len && (s[ich] >= '0' && s[ich] <= '9'))
            {
                lresult = lresult * 10 + (s[ich] - '0');
                ich++;
            };

            /* Skip tabs and white spaces */
            while (ich < len && (s[ich] == ' ' || s[ich] == 9)) ich++;

            /* Must be in the end */
            if (ich != len) return false;

            if (fNegative) lout = -lresult;
            else lout = lresult;

            return true;
        }

        // SortStringArray
        public static void SortStringArray(string[] rgstr)
        {
            int n = rgstr.Length;
            int step = n / 2;

            while (step > 0)
            {
                bool fContinue = true;

                while (fContinue)
                {
                    int i = 0;
                    fContinue = false;
                    while (i + step < n)
                    {
                        if (Common.CompareStr(rgstr[i], rgstr[i + step]) > 0)
                        {
                            string help = rgstr[i];
                            rgstr[i] = rgstr[i + step];
                            rgstr[i + step] = help;

                            fContinue = true;
                        }

                        i = i + step;
                    };
                }
                step = step / 2;
            }
        }

        // Find if string contains special uncode character 
        static public bool FContainsUnicodeCharacter(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] > 0xFF) return true;
            }

            return false;
        }


        // Finds string in given array (not sorted); 
        // returns -1 if such string is not found
        public static int FindStringInArray(string[] rgstrArray, string strKey)
        {
            int i = 0;
            while (i < rgstrArray.Length && Common.CompareStr(strKey, rgstrArray[i]) != 0)
            {
                i++;
            };

            if (i == rgstrArray.Length) return -1;
            else return i;
        }

        //Canceling dialog returns path passed in.
        public static string GetFolder(string szPath)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = szPath;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }
            else
                return szPath;
        }
    }

    class Converters
    {
        public static double TwipToPx(double twip)
        {
            return (twip / 1440f) * 96f;
        }

        public static double TwipToPositivePx(double twip)
        {
            double px = (twip / 1440f) * 96f;
            if (px < 0) px = 0;
            return px;
        }

        public static double TwipToPositiveVisiblePx(double twip)
        {
            double px = (twip / 1440f) * 96f;
            if (px < 0) px = 0;
            if (twip > 0.0 && px < 1.0) px = 1.0;
            return px;
        }

        public static double PtToPx(double pt)
        {
            return Math.Round(((pt / 72f) * 96f), 2);
        }

        public static double PxToPt(double px)
        {
            return (px / 96) * 72f;
        }

        public static double PxToTwip(double px)
        {
            return (px / 96f) * 1440f;
        }

        public static long PxToTwipRounded(double px)
        {
            double twip = (px / 96f) * 1440f;
            if (twip < 0)
                return (long)(twip - 0.5);
            else
                return (long)(twip + 0.5);
        }

        // Convert FG, BG and shading to produce color to fill with.
        // Shading is in 100'ths of a percent (ie, 10000 is 100%)
        // A value of zero for shading means use all CB.
        // A value of 10000 for shading means use all CF.
        // Intermediate values mean some combination of
        //internal static bool ColorToUse(ConverterState converterState, long cb, long cf, long shade, ref Color c)
        //{
        //    ColorTableEntry entryCB = cb >= 0 ? converterState.ColorTable.EntryAt((int)cb) : null;
        //    ColorTableEntry entryCF = cf >= 0 ? converterState.ColorTable.EntryAt((int)cf) : null;

        //    // No shading
        //    if (shade < 0)
        //    {
        //        if (entryCB == null)
        //            return false;
        //        else
        //        {
        //            c = entryCB.Color;
        //            return true;
        //        }
        //    }

        //    // Shading
        //    else
        //    {
        //        Color cCB = entryCB != null ? entryCB.Color : Color.FromArgb(0xFF, 0, 0, 0);
        //        Color cCF = entryCF != null ? entryCF.Color : Color.FromArgb(0xFF, 255, 255, 255);

        //        // No color specifies means shading is treated as a grey intensity.
        //        if (entryCF == null && entryCB == null)
        //        {
        //            c = Color.FromArgb(0xff,
        //                              (byte)(255 - (255 * shade / 10000)),
        //                              (byte)(255 - (255 * shade / 10000)),
        //                              (byte)(255 - (255 * shade / 10000)));
        //            return true;
        //        }

        //        // Only CF means CF fades as shading goes from 10,000 to 0
        //        else if (entryCB == null)
        //        {
        //            c = Color.FromArgb(0xff,
        //                                (byte)(cCF.R + ((255 - cCF.R) * (10000 - shade) / 10000)),
        //                                (byte)(cCF.G + ((255 - cCF.G) * (10000 - shade) / 10000)),
        //                                (byte)(cCF.B + ((255 - cCF.B) * (10000 - shade) / 10000)));
        //            return true;
        //        }

        //        // Only CB means CB gets larger impact (from black ) as shading goes from 10000 to 0
        //        else if (entryCF == null)
        //        {
        //            c = Color.FromArgb(0xff,
        //                                (byte)(cCB.R - (cCB.R * shade / 10000)),
        //                                (byte)(cCB.G - (cCB.G * shade / 10000)),
        //                                (byte)(cCB.B - (cCB.B * shade / 10000)));
        //            return true;
        //        }

        //        // Both - need to mix colors
        //        else
        //        {
        //            c = Color.FromArgb(0xff,
        //                              (byte)((cCB.R * (10000 - shade) / 10000) +
        //                                      (cCF.R * shade / 10000)),
        //                              (byte)((cCB.G * (10000 - shade) / 10000) +
        //                                      (cCF.G * shade / 10000)),
        //                              (byte)((cCB.B * (10000 - shade) / 10000) +
        //                                      (cCF.B * shade / 10000)));
        //            return true;
        //        }
        //    }
        //}

    };
}