// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: This file contains a set of DRTs designed to test the MediaAPIs 
//              and public classes.
//
//

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Markup;
using MS.Internal;

/// <summary>
/// Test for primitive opertations
/// </summary>
public class DrtPrimitives : ColorAPIDRTs
{
    /// <summary>
    /// Run DRT.
    /// </summary>
    public override bool Run(out string results)
    {
        bool succeeded = true;
        
        string colors;

        // Note that because of the order of the operands to &&, all tests will run
        // even if one failed.  This is intentional, and ensures that all regressions 
        // are noted, not just the initial failure.

        succeeded = TestColors(out colors) && succeeded;

        // If failure occurred, then we care about the output
        if (!succeeded)
        {
            results = colors;
        }
        else
        {
            results = "SUCCEEDED";
        }

        return succeeded;
    }

    internal static readonly string WindowsColorProfiles = GetSystemColorProfilesDirectory();

    ///<summary>
    /// returns stream when trying to open profiles from a string
    ///</summary>
    internal static Uri GetUriFromProfileName(string filename)
    {
        Uri profileUri = null;

        if (File.Exists(filename))
        {
            profileUri = new Uri("file://" + Directory.GetCurrentDirectory() + "/" + filename);
        }
        else if (File.Exists(WindowsColorProfiles + filename))
        {
            profileUri = new Uri("file://"+WindowsColorProfiles+filename);
        }
        else
        {
            throw new ArgumentNullException("fileStream");
        }

        return profileUri;
    }

    [EnvironmentPermission(SecurityAction.Assert, Read = "windir")]
    private static string GetSystemColorProfilesDirectory()
    {
        string s = Environment.GetEnvironmentVariable("windir") + @"\system32\spool\drivers\color\";

        return s.ToLower(CultureInfo.InvariantCulture);
    }

    private bool TestColors(out string results)
    {
        bool succeed = true;
        int test = 0;

        results = "";
        
        do
        {
            // Color FromAValues(float a, float[], filename)
            // Color FromValues(float[], filename)
            // AreClose(Color, Color)
            // IsClose(Color)
            // Equals(object)
            // float[] GetColorContextColor()
            // The == and != operators are not working correctly for AValues Colors
            float[] floats_in = new float[] { 1.0f, 1.0f, 0.0f };
            Color C1 = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            Color C2 = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            bool result = (C1 == C2);

            // Calling ToString() on an AValues Color generates the exception "Input String was not in the correct format"
            Color newcolor = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            newcolor.ToString(CultureInfo.InvariantCulture); //  or newcolor.ToString(System.IFormatProvider.default); generates the exception:  "Input String was not in the correct format"

            // Changeables: Calling BitmapEffectColorTransform.Clone() causes System.NullReferenceException
            /*
            try
            {
                ColorContext colorCtx = new ColorContext(System.Windows.Media.PixelFormats.Bgra32);
                BitmapEffectColorTransform img = new BitmapEffectColorTransform(colorCtx, colorCtx, PixelFormats.Pbgra32);
                BitmapEffectColorTransform img2 = img.Clone();
                Console.WriteLine("OK");

            }
            catch (System.ArgumentNullException e)
            {
                Console.WriteLine("{0}", e.ToString());
            }
            */
            // scRGB test
            // constructor Color(float, float, float, float)
            Color col1 = Color.FromScRgb(0.2f, 0.0f, 0.5f, 1.0f);
            // operator ==
            test++; if (!(col1 == Color.FromScRgb(0.2f, 0.0f, 0.5f, 1.0f))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator Equals(Color, Color)
            test++; if (!col1.Equals(Color.FromScRgb(0.2f, 0.0f, 0.5f, 1.0f))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator !=
            test++; if (col1 != Color.FromScRgb(0.2f, 0.0f, 0.5f, 1.0f)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // method GetHashCode

            test++; if (!(col1.GetHashCode() == (Color.FromScRgb(0.2f, 0.0f, 0.5f, 1.0f)).GetHashCode())) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property. ScA
            test++; if (0.2f != col1.ScA) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .ScR
            test++; if (0.0f != col1.ScR) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .ScG
            test++; if (0.5f != col1.ScG) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .ScB
            test++; if (1.0f != col1.ScB) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            Color col2 = Color.FromScRgb(0.3f, 1.0f, 0.5f, 0.0f);
            // operator +
            test++; if (Color.FromScRgb(0.5f, 1.0f, 1.0f, 1.0f) != col1+col2) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator Add
            test++; if (Color.FromScRgb(0.5f, 1.0f, 1.0f, 1.0f) != Color.Add(col1, col2)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator -
            Color col3 = col1+col2;
            test++; if (!Color.AreClose(col1, col3-col2)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator Subtract
            test++; if (!Color.AreClose(col1, Color.Subtract(col3, col2))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator *
            test++; if (Color.FromScRgb(0.25f, 0.5f, 0.5f, 0.5f) != col3*0.5f) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // operator Multiply
            test++; if (Color.FromScRgb(0.25f, 0.5f, 0.5f, 0.5f) != Color.Multiply(col3, 0.5f)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromScRgb(2.0f, 1.5f, 0.5f, -1.0f);
            // method clamp
            col1.Clamp();
            test++; if (!(col1 == Color.FromScRgb(1.0f, 1.0f, 0.5f, 0.0f))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            // Color.FromProfile(filename)
            float[] cmykValue = new float[4];
            cmykValue[0] = 0.0f; cmykValue[1] = 0.5f; cmykValue[2] = 0.5f; cmykValue[3] = 0.5f;
            col1 = Color.FromValues(cmykValue, GetUriFromProfileName(@"DrtFiles\DrtColorapi\testCMYK1.icc"));
            cmykValue[0] = 0.5f; cmykValue[1] = 0.0f; cmykValue[2] = 0.0f; cmykValue[3] = 0.5f;
            col2 = Color.FromValues(cmykValue, GetUriFromProfileName(@"DrtFiles\DrtColorapi\testCMYK1.icc"));
            col1 = col1 + col2;

            col3 = Color.FromAValues(0.4f, cmykValue, GetUriFromProfileName(@"DrtFiles\DrtColorapi\testCMYK1.icc"));

            // Testing type conversion of context colors to string ....
            string scol1 = col1.ToString(CultureInfo.InvariantCulture);
            string scol2 = col2.ToString(CultureInfo.InvariantCulture);
            string scol3 = col3.ToString(CultureInfo.InvariantCulture);

            // .... and back from string
            ColorConverter colorConverter = new ColorConverter();
            Color parsecol1 = (Color)colorConverter.ConvertFromInvariantString(scol1);
            Color parsecol2 = (Color)colorConverter.ConvertFromInvariantString(scol2);
            Color parsecol3 = (Color)colorConverter.ConvertFromInvariantString(scol3);

            // .... and comparing for equality.
            test++; if (!(parsecol1 == col1)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            test++; if (!(parsecol2 == col2)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            test++; if (!(parsecol3 == col3)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            // constructor Color(byte, byte, byte, byte)
            col1 = Color.FromRgb(0x00, 0x7f, 0xff);
            // ToString()
            test++; if (!("#FF007FFF" == col1.ToString(CultureInfo.InvariantCulture))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // constructor Color(byte, byte, byte) and ==
            test++; if (!(col1 == Color.FromRgb(0x00, 0x7f, 0xff))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .A
            test++; if (0xff != col1.A) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .R
            test++; if (0x00 != col1.R) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .G
            test++; if (0x7f != col1.G) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .B
            test++; if (0xff != col1.B) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            // constructor Color(byte, byte, byte, byte)
            col1 = Color.FromRgb(0xff, 0x00, 0x7f);
            // ToString()
            test++; if (!("#FFFF007F" == col1.ToString(CultureInfo.InvariantCulture))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // constructor Color(byte, byte, byte) and ==
            test++; if (!(col1 == Color.FromRgb(0xff, 0x00, 0x7f))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .A
            test++; if (0xff != col1.A) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .R
            test++; if (0xff != col1.R) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .G
            test++; if (0x00 != col1.G) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .B
            test++; if (0x7f != col1.B) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            // constructor Color(byte, byte, byte, byte)
            col1 = Color.FromRgb(0x7f, 0xff, 0x00);
            // ToString()
            test++; if (!("#FF7FFF00" == col1.ToString(CultureInfo.InvariantCulture))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // constructor Color(byte, byte, byte) and ==
            test++; if (!(col1 == Color.FromRgb(0x7f, 0xff, 0x00))) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .A
            test++; if (0xff != col1.A) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .R
            test++; if (0x7f != col1.R) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .G
            test++; if (0xff != col1.G) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property .B
            test++; if (0x00 != col1.B) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            // property KnownColor
            DRTKnownColors kc = new DRTKnownColors();
            for (int i = 0; i < 141; i++)
            {
                test++; if (!kc.Test(i)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }
            }         
            // color parsing
            test++; if ((Color)(TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString("#7FFF7F00")) != Color.FromArgb(0x7F, 0xFF, 0x7F, 0x00)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            col1 = Color.FromScRgb(0.5f, 0.5f, 0.5f, 0.5f);
            test++; if (Color.Multiply(col1, 2.0f) != Color.FromScRgb(1.0f, 1.0f, 1.0f, 1.0f)) { succeed = false; results += "Color Test " + test + " FAILED\n"; }

            /*
            // Round-trip of sRGB values is only guaranteed when the internal floating-point sRGB channel values are in [0,1] and equal to n/255 or n/100, n an integer.
            // This is only useful for standard input sRGB values  not after performing math operations on them.

            col1 = Color.FromArgb(0x7F, 0x7F, 0x7F, 0x7F);
            test++; if (!ByteCompareColors (Color.Multiply (col1, 0x2), Color.FromArgb (254, 183, 183, 183))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromArgb(0xFF, 0xB2, 0x99, 0xCC);
            col2 = Color.FromArgb(0x80, 0x33, 0x1A, 0x4D);
            test++; if (!ByteCompareColors ((col1 - col2), Color.FromArgb (127, 167, 148, 186))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromArgb(0x7F, 0x7F, 0x7F, 0x7F);
            test++; if (!ByteCompareColors (Color.Multiply (col1, 0x2), Color.FromArgb (254, 183, 183, 183))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromRgb(0xFF, 0x00, 0x00);
            col2 = Color.FromRgb(0x00, 0x00, 0xFF);
            test++; if (!ByteCompareColors (Color.Add (col1, col2), Color.FromArgb (255, 255, 0, 255))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromRgb(0xB2, 0x99, 0xCC);
            col2 = Color.FromRgb(0x33, 0x1A, 0x4D);
            test++; if (!ByteCompareColors ((col1 - col2), Color.FromArgb (0, 167, 148, 186))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            col1 = Color.FromRgb(0x7F, 0x7F, 0x7F);
            test++; if (!ByteCompareColors (Color.Multiply (col1, 0x2), Color.FromArgb (255, 183, 183, 183))) { succeed = false;results += "Color Test " + test + " FAILED\n"; }
            */
        }
        while (false);

        return succeed;
    }

    private bool ByteCompareColors (Color c1, Color c2)
    {
        bool result=true;

        if (c1.R != c2.R)
            result = false;
        if (c1.G != c2.G)
            result = false;
        if (c1.B != c2.B)
            result = false;
        if (c1.A != c2.A)
            result = false;
        return result;
    }

    private class DRTKnownColors
    {
        static Color[]         myColor = new Color[144];
        static string[]                             myColorString = new string[144];
        static byte[]                               ExpectedRed = new byte[144];
        static byte[]                               ExpectedGreen = new byte[144];
        static byte[]                               ExpectedBlue = new byte[144];

        public DRTKnownColors()
        {
            #region set colors
            myColor[0] = System.Windows.Media.Colors.AliceBlue;
            myColor[1] = System.Windows.Media.Colors.AntiqueWhite;
            myColor[2] = System.Windows.Media.Colors.Aqua;
            myColor[3] = System.Windows.Media.Colors.Aquamarine;
            myColor[4] = System.Windows.Media.Colors.Azure;
            myColor[5] = System.Windows.Media.Colors.Beige;
            myColor[6] = System.Windows.Media.Colors.Bisque;
            myColor[7] = System.Windows.Media.Colors.Black;
            myColor[8] = System.Windows.Media.Colors.BlanchedAlmond;
            myColor[9] = System.Windows.Media.Colors.Blue;
            myColor[10] = System.Windows.Media.Colors.BlueViolet;
            myColor[11] = System.Windows.Media.Colors.Brown;
            myColor[12] = System.Windows.Media.Colors.BurlyWood;
            myColor[13] = System.Windows.Media.Colors.CadetBlue;
            myColor[14] = System.Windows.Media.Colors.Chartreuse;
            myColor[15] = System.Windows.Media.Colors.Chocolate;
            myColor[16] = System.Windows.Media.Colors.Coral;
            myColor[17] = System.Windows.Media.Colors.CornflowerBlue;
            myColor[18] = System.Windows.Media.Colors.Cornsilk;
            myColor[19] = System.Windows.Media.Colors.Crimson;
            myColor[20] = System.Windows.Media.Colors.Cyan;
            myColor[21] = System.Windows.Media.Colors.DarkBlue;
            myColor[22] = System.Windows.Media.Colors.DarkCyan;
            myColor[23] = System.Windows.Media.Colors.DarkGoldenrod;
            myColor[24] = System.Windows.Media.Colors.DarkGray;
            myColor[25] = System.Windows.Media.Colors.DarkGreen;
            myColor[26] = System.Windows.Media.Colors.DarkKhaki;
            myColor[27] = System.Windows.Media.Colors.DarkMagenta;
            myColor[28] = System.Windows.Media.Colors.DarkOliveGreen;
            myColor[29] = System.Windows.Media.Colors.DarkOrange;
            myColor[30] = System.Windows.Media.Colors.DarkOrchid;
            myColor[31] = System.Windows.Media.Colors.DarkRed;
            myColor[32] = System.Windows.Media.Colors.DarkSalmon;
            myColor[33] = System.Windows.Media.Colors.DarkSeaGreen;
            myColor[34] = System.Windows.Media.Colors.DarkSlateBlue;
            myColor[35] = System.Windows.Media.Colors.DarkSlateGray;
            myColor[36] = System.Windows.Media.Colors.DarkTurquoise;
            myColor[37] = System.Windows.Media.Colors.DarkViolet;
            myColor[38] = System.Windows.Media.Colors.DeepPink;
            myColor[39] = System.Windows.Media.Colors.DeepSkyBlue;
            myColor[40] = System.Windows.Media.Colors.DimGray;
            myColor[41] = System.Windows.Media.Colors.DodgerBlue;
            myColor[42] = System.Windows.Media.Colors.Firebrick;
            myColor[43] = System.Windows.Media.Colors.FloralWhite;
            myColor[44] = System.Windows.Media.Colors.ForestGreen;
            myColor[45] = System.Windows.Media.Colors.Fuchsia;
            myColor[46] = System.Windows.Media.Colors.Gainsboro;
            myColor[47] = System.Windows.Media.Colors.GhostWhite;
            myColor[48] = System.Windows.Media.Colors.Gold;
            myColor[49] = System.Windows.Media.Colors.Goldenrod;
            myColor[50] = System.Windows.Media.Colors.Gray;
            myColor[51] = System.Windows.Media.Colors.Green;
            myColor[52] = System.Windows.Media.Colors.GreenYellow;
            myColor[53] = System.Windows.Media.Colors.Honeydew;
            myColor[54] = System.Windows.Media.Colors.HotPink;
            myColor[55] = System.Windows.Media.Colors.IndianRed;
            myColor[56] = System.Windows.Media.Colors.Indigo;
            myColor[57] = System.Windows.Media.Colors.Ivory;
            myColor[58] = System.Windows.Media.Colors.Khaki;
            myColor[59] = System.Windows.Media.Colors.Lavender;
            myColor[60] = System.Windows.Media.Colors.LavenderBlush;
            myColor[61] = System.Windows.Media.Colors.LawnGreen;
            myColor[62] = System.Windows.Media.Colors.LemonChiffon;
            myColor[63] = System.Windows.Media.Colors.LightBlue;
            myColor[64] = System.Windows.Media.Colors.LightCoral;
            myColor[65] = System.Windows.Media.Colors.LightCyan;
            myColor[66] = System.Windows.Media.Colors.LightGoldenrodYellow;
            myColor[67] = System.Windows.Media.Colors.LightGreen;
            myColor[68] = System.Windows.Media.Colors.LightGray;
            myColor[69] = System.Windows.Media.Colors.LightPink;
            myColor[70] = System.Windows.Media.Colors.LightSalmon;
            myColor[71] = System.Windows.Media.Colors.LightSeaGreen;
            myColor[72] = System.Windows.Media.Colors.LightSkyBlue;
            myColor[73] = System.Windows.Media.Colors.LightSlateGray;
            myColor[74] = System.Windows.Media.Colors.LightSteelBlue;
            myColor[75] = System.Windows.Media.Colors.LightYellow;
            myColor[76] = System.Windows.Media.Colors.Lime;
            myColor[77] = System.Windows.Media.Colors.LimeGreen;
            myColor[78] = System.Windows.Media.Colors.Linen;
            myColor[79] = System.Windows.Media.Colors.Magenta;
            myColor[80] = System.Windows.Media.Colors.Maroon;
            myColor[81] = System.Windows.Media.Colors.MediumAquamarine;
            myColor[82] = System.Windows.Media.Colors.MediumBlue;
            myColor[83] = System.Windows.Media.Colors.MediumOrchid;
            myColor[84] = System.Windows.Media.Colors.MediumPurple;
            myColor[85] = System.Windows.Media.Colors.MediumSeaGreen;
            myColor[86] = System.Windows.Media.Colors.MediumSlateBlue;
            myColor[87] = System.Windows.Media.Colors.MediumSpringGreen;
            myColor[88] = System.Windows.Media.Colors.MediumTurquoise;
            myColor[89] = System.Windows.Media.Colors.MediumVioletRed;
            myColor[90] = System.Windows.Media.Colors.MidnightBlue;
            myColor[91] = System.Windows.Media.Colors.MintCream;
            myColor[92] = System.Windows.Media.Colors.MistyRose;
            myColor[93] = System.Windows.Media.Colors.Moccasin;
            myColor[94] = System.Windows.Media.Colors.NavajoWhite;
            myColor[95] = System.Windows.Media.Colors.Navy;
            myColor[96] = System.Windows.Media.Colors.OldLace;
            myColor[97] = System.Windows.Media.Colors.Olive;
            myColor[98] = System.Windows.Media.Colors.OliveDrab;
            myColor[99] = System.Windows.Media.Colors.Orange;
            myColor[100] = System.Windows.Media.Colors.OrangeRed;
            myColor[101] = System.Windows.Media.Colors.Orchid;
            myColor[102] = System.Windows.Media.Colors.PaleGoldenrod;
            myColor[103] = System.Windows.Media.Colors.PaleGreen;
            myColor[104] = System.Windows.Media.Colors.PaleTurquoise;
            myColor[105] = System.Windows.Media.Colors.PaleVioletRed;
            myColor[106] = System.Windows.Media.Colors.PapayaWhip;
            myColor[107] = System.Windows.Media.Colors.PeachPuff;
            myColor[108] = System.Windows.Media.Colors.Peru;
            myColor[109] = System.Windows.Media.Colors.Pink;
            myColor[110] = System.Windows.Media.Colors.Plum;
            myColor[111] = System.Windows.Media.Colors.PowderBlue;
            myColor[112] = System.Windows.Media.Colors.Purple;
            myColor[113] = System.Windows.Media.Colors.Red;
            myColor[114] = System.Windows.Media.Colors.RosyBrown;
            myColor[115] = System.Windows.Media.Colors.RoyalBlue;
            myColor[116] = System.Windows.Media.Colors.SaddleBrown;
            myColor[117] = System.Windows.Media.Colors.Salmon;
            myColor[118] = System.Windows.Media.Colors.SandyBrown;
            myColor[119] = System.Windows.Media.Colors.SeaGreen;
            myColor[120] = System.Windows.Media.Colors.SeaShell;
            myColor[121] = System.Windows.Media.Colors.Sienna;
            myColor[122] = System.Windows.Media.Colors.Silver;
            myColor[123] = System.Windows.Media.Colors.SkyBlue;
            myColor[124] = System.Windows.Media.Colors.SlateBlue;
            myColor[125] = System.Windows.Media.Colors.SlateGray;
            myColor[126] = System.Windows.Media.Colors.Snow;
            myColor[127] = System.Windows.Media.Colors.SpringGreen;
            myColor[128] = System.Windows.Media.Colors.SteelBlue;
            myColor[129] = System.Windows.Media.Colors.Tan;
            myColor[130] = System.Windows.Media.Colors.Teal;
            myColor[131] = System.Windows.Media.Colors.Thistle;
            myColor[132] = System.Windows.Media.Colors.Tomato;
            myColor[133] = System.Windows.Media.Colors.Transparent;
            myColor[134] = System.Windows.Media.Colors.Turquoise;
            myColor[135] = System.Windows.Media.Colors.Violet;
            myColor[136] = System.Windows.Media.Colors.Wheat;
            myColor[137] = System.Windows.Media.Colors.White;
            myColor[138] = System.Windows.Media.Colors.WhiteSmoke;
            myColor[139] = System.Windows.Media.Colors.Yellow;
            myColor[140] = System.Windows.Media.Colors.YellowGreen;
            //            myColor[141] = new Color(0.5f,0.5f,0.5f);
            //            myColor[142] = new Color(0xff8080ff);
            //            myColor[143] = new Color(1f,0f,0.5f,0.5f);

            myColorString[0] = "AliceBlue";
            myColorString[1] = "AntiqueWhite";
            myColorString[2] = "Aqua";
            myColorString[3] = "Aquamarine";
            myColorString[4] = "Azure";
            myColorString[5] = "Beige";
            myColorString[6] = "Bisque";
            myColorString[7] = "Black";
            myColorString[8] = "BlanchedAlmond";
            myColorString[9] = "Blue";
            myColorString[10] = "BlueViolet";
            myColorString[11] = "Brown";
            myColorString[12] = "BurlyWood";
            myColorString[13] = "CadetBlue";
            myColorString[14] = "Chartreuse";
            myColorString[15] = "Chocolate";
            myColorString[16] = "Coral";
            myColorString[17] = "CornflowerBlue";
            myColorString[18] = "Cornsilk";
            myColorString[19] = "Crimson";
            myColorString[20] = "Cyan";
            myColorString[21] = "DarkBlue";
            myColorString[22] = "DarkCyan";
            myColorString[23] = "DarkGoldenrod";
            myColorString[24] = "DarkGray";
            myColorString[25] = "DarkGreen";
            myColorString[26] = "DarkKhaki";
            myColorString[27] = "DarkMagenta";
            myColorString[28] = "DarkOliveGreen";
            myColorString[29] = "DarkOrange";
            myColorString[30] = "DarkOrchid";
            myColorString[31] = "DarkRed";
            myColorString[32] = "DarkSalmon";
            myColorString[33] = "DarkSeaGreen";
            myColorString[34] = "DarkSlateBlue";
            myColorString[35] = "DarkSlateGray";
            myColorString[36] = "DarkTurquoise";
            myColorString[37] = "DarkViolet";
            myColorString[38] = "DeepPink";
            myColorString[39] = "DeepSkyBlue";
            myColorString[40] = "DimGray";
            myColorString[41] = "DodgerBlue";
            myColorString[42] = "Firebrick";
            myColorString[43] = "FloralWhite";
            myColorString[44] = "ForestGreen";
            myColorString[45] = "Fuchsia";
            myColorString[46] = "Gainsboro";
            myColorString[47] = "GhostWhite";
            myColorString[48] = "Gold";
            myColorString[49] = "Goldenrod";
            myColorString[50] = "Gray";
            myColorString[51] = "Green";
            myColorString[52] = "GreenYellow";
            myColorString[53] = "Honeydew";
            myColorString[54] = "HotPink";
            myColorString[55] = "IndianRed";
            myColorString[56] = "Indigo";
            myColorString[57] = "Ivory";
            myColorString[58] = "Khaki";
            myColorString[59] = "Lavender";
            myColorString[60] = "LavenderBlush";
            myColorString[61] = "LawnGreen";
            myColorString[62] = "LemonChiffon";
            myColorString[63] = "LightBlue";
            myColorString[64] = "LightCoral";
            myColorString[65] = "LightCyan";
            myColorString[66] = "LightGoldenrodYellow";
            myColorString[67] = "LightGreen";
            myColorString[68] = "LightGray";
            myColorString[69] = "LightPink";
            myColorString[70] = "LightSalmon";
            myColorString[71] = "LightSeaGreen";
            myColorString[72] = "LightSkyBlue";
            myColorString[73] = "LightSlateGray";
            myColorString[74] = "LightSteelBlue";
            myColorString[75] = "LightYellow";
            myColorString[76] = "Lime";
            myColorString[77] = "LimeGreen";
            myColorString[78] = "Linen";
            myColorString[79] = "Magenta";
            myColorString[80] = "Maroon";
            myColorString[81] = "MediumAquamarine";
            myColorString[82] = "MediumBlue";
            myColorString[83] = "MediumOrchid";
            myColorString[84] = "MediumPurple";
            myColorString[85] = "MediumSeaGreen";
            myColorString[86] = "MediumSlateBlue";
            myColorString[87] = "MediumSpringGreen";
            myColorString[88] = "MediumTurquoise";
            myColorString[89] = "MediumVioletRed";
            myColorString[90] = "MidnightBlue";
            myColorString[91] = "MintCream";
            myColorString[92] = "MistyRose";
            myColorString[93] = "Moccasin";
            myColorString[94] = "NavajoWhite";
            myColorString[95] = "Navy";
            myColorString[96] = "OldLace";
            myColorString[97] = "Olive";
            myColorString[98] = "OliveDrab";
            myColorString[99] = "Orange";
            myColorString[100] = "OrangeRed";
            myColorString[101] = "Orchid";
            myColorString[102] = "PaleGoldenrod";
            myColorString[103] = "PaleGreen";
            myColorString[104] = "PaleTurquoise";
            myColorString[105] = "PaleVioletRed";
            myColorString[106] = "PapayaWhip";
            myColorString[107] = "PeachPuff";
            myColorString[108] = "Peru";
            myColorString[109] = "Pink";
            myColorString[110] = "Plum";
            myColorString[111] = "PowderBlue";
            myColorString[112] = "Purple";
            myColorString[113] = "Red";
            myColorString[114] = "RosyBrown";
            myColorString[115] = "RoyalBlue";
            myColorString[116] = "SaddleBrown";
            myColorString[117] = "Salmon";
            myColorString[118] = "SandyBrown";
            myColorString[119] = "SeaGreen";
            myColorString[120] = "SeaShell";
            myColorString[121] = "Sienna";
            myColorString[122] = "Silver";
            myColorString[123] = "SkyBlue";
            myColorString[124] = "SlateBlue";
            myColorString[125] = "SlateGray";
            myColorString[126] = "Snow";
            myColorString[127] = "SpringGreen";
            myColorString[128] = "SteelBlue";
            myColorString[129] = "Tan";
            myColorString[130] = "Teal";
            myColorString[131] = "Thistle";
            myColorString[132] = "Tomato";
            myColorString[133] = "Transparent";
            myColorString[134] = "Turquoise";
            myColorString[135] = "Violet";
            myColorString[136] = "Wheat";
            myColorString[137] = "White";
            myColorString[138] = "WhiteSmoke";
            myColorString[139] = "Yellow";
            myColorString[140] = "YellowGreen";
            myColorString[141] = "Constructor using 3 floats";
            myColorString[142] = "Constructor using uint";
            myColorString[143] = "Constructor using 4 floats";

            ExpectedRed[0] = 240;   ExpectedGreen[0] = 248;     ExpectedBlue[0] = 255;
            ExpectedRed[1] = 250;   ExpectedGreen[1] = 235;     ExpectedBlue[1] = 215;
            ExpectedRed[2] = 0;     ExpectedGreen[2] = 255;     ExpectedBlue[2] = 255;
            ExpectedRed[3] = 127;   ExpectedGreen[3] = 255;     ExpectedBlue[3] = 212;
            ExpectedRed[4] = 240;   ExpectedGreen[4] = 255;     ExpectedBlue[4] = 255;
            ExpectedRed[5] = 245;   ExpectedGreen[5] = 245;     ExpectedBlue[5] = 220;
            ExpectedRed[6] = 255;   ExpectedGreen[6] = 228;     ExpectedBlue[6] = 196;
            ExpectedRed[7] = 0;     ExpectedGreen[7] = 0;       ExpectedBlue[7] = 0;
            ExpectedRed[8] = 255;   ExpectedGreen[8] = 235;     ExpectedBlue[8] = 205;
            ExpectedRed[9] = 0;     ExpectedGreen[9] = 0;       ExpectedBlue[9] = 255;
            ExpectedRed[10] = 138;  ExpectedGreen[10] = 43;     ExpectedBlue[10] = 226;
            ExpectedRed[11] = 165;  ExpectedGreen[11] = 42;     ExpectedBlue[11] = 42;
            ExpectedRed[12] = 222;  ExpectedGreen[12] = 184;    ExpectedBlue[12] = 135;
            ExpectedRed[13] = 95;   ExpectedGreen[13] = 158;    ExpectedBlue[13] = 160;
            ExpectedRed[14] = 127;  ExpectedGreen[14] = 255;    ExpectedBlue[14] = 0;
            ExpectedRed[15] = 210;  ExpectedGreen[15] = 105;    ExpectedBlue[15] = 30;
            ExpectedRed[16] = 255;  ExpectedGreen[16] = 127;    ExpectedBlue[16] = 80;
            ExpectedRed[17] = 100;  ExpectedGreen[17] = 149;    ExpectedBlue[17] = 237;
            ExpectedRed[18] = 255;  ExpectedGreen[18] = 248;    ExpectedBlue[18] = 220;
            ExpectedRed[19] = 220;  ExpectedGreen[19] = 20;     ExpectedBlue[19] = 60;
            ExpectedRed[20] = 0;    ExpectedGreen[20] = 255;    ExpectedBlue[20] = 255;
            ExpectedRed[21] = 0;    ExpectedGreen[21] = 0;      ExpectedBlue[21] = 139;
            ExpectedRed[22] = 0;    ExpectedGreen[22] = 139;    ExpectedBlue[22] = 139;
            ExpectedRed[23] = 184;  ExpectedGreen[23] = 134;    ExpectedBlue[23] = 11;
            ExpectedRed[24] = 169;  ExpectedGreen[24] = 169;    ExpectedBlue[24] = 169;
            ExpectedRed[25] = 0;    ExpectedGreen[25] = 100;    ExpectedBlue[25] = 0;
            ExpectedRed[26] = 189;  ExpectedGreen[26] = 183;    ExpectedBlue[26] = 107;
            ExpectedRed[27] = 139;  ExpectedGreen[27] = 0;      ExpectedBlue[27] = 139;
            ExpectedRed[28] = 85;   ExpectedGreen[28] = 107;    ExpectedBlue[28] = 47;
            ExpectedRed[29] = 255;  ExpectedGreen[29] = 140;    ExpectedBlue[29] = 0;
            ExpectedRed[30] = 153;  ExpectedGreen[30] = 50;     ExpectedBlue[30] = 204;
            ExpectedRed[31] = 139;  ExpectedGreen[31] = 0;      ExpectedBlue[31] = 0;
            ExpectedRed[32] = 233;  ExpectedGreen[32] = 150;    ExpectedBlue[32] = 122;
            ExpectedRed[33] = 143;  ExpectedGreen[33] = 188;    ExpectedBlue[33] = 139;
            ExpectedRed[34] = 72;   ExpectedGreen[34] = 61;     ExpectedBlue[34] = 139;
            ExpectedRed[35] = 47;   ExpectedGreen[35] = 79;     ExpectedBlue[35] = 79;
            ExpectedRed[36] = 0;    ExpectedGreen[36] = 206;    ExpectedBlue[36] = 209;
            ExpectedRed[37] = 148;  ExpectedGreen[37] = 0;      ExpectedBlue[37] = 211;
            ExpectedRed[38] = 255;  ExpectedGreen[38] = 20;     ExpectedBlue[38] = 147;
            ExpectedRed[39] = 0;    ExpectedGreen[39] = 191;    ExpectedBlue[39] = 255;
            ExpectedRed[40] = 105;  ExpectedGreen[40] = 105;    ExpectedBlue[40] = 105;
            ExpectedRed[41] = 30;   ExpectedGreen[41] = 144;    ExpectedBlue[41] = 255;
            ExpectedRed[42] = 178;  ExpectedGreen[42] = 34;     ExpectedBlue[42] = 34;
            ExpectedRed[43] = 255;  ExpectedGreen[43] = 250;    ExpectedBlue[43] = 240;
            ExpectedRed[44] = 34;   ExpectedGreen[44] = 139;    ExpectedBlue[44] = 34;
            ExpectedRed[45] = 255;  ExpectedGreen[45] = 0;      ExpectedBlue[45] = 255;
            ExpectedRed[46] = 220;  ExpectedGreen[46] = 220;    ExpectedBlue[46] = 220;
            ExpectedRed[47] = 248;  ExpectedGreen[47] = 248;    ExpectedBlue[47] = 255;
            ExpectedRed[48] = 255;  ExpectedGreen[48] = 215;    ExpectedBlue[48] = 0;
            ExpectedRed[49] = 218;  ExpectedGreen[49] = 165;    ExpectedBlue[49] = 32;
            ExpectedRed[50] = 128;  ExpectedGreen[50] = 128;    ExpectedBlue[50] = 128;
            ExpectedRed[51] = 0;    ExpectedGreen[51] = 128;    ExpectedBlue[51] = 0;
            ExpectedRed[52] = 173;  ExpectedGreen[52] = 255;    ExpectedBlue[52] = 47;
            ExpectedRed[53] = 240;  ExpectedGreen[53] = 255;    ExpectedBlue[53] = 240;
            ExpectedRed[54] = 255;  ExpectedGreen[54] = 105;    ExpectedBlue[54] = 180;
            ExpectedRed[55] = 205;  ExpectedGreen[55] = 92;     ExpectedBlue[55] = 92;
            ExpectedRed[56] = 75;   ExpectedGreen[56] = 0;      ExpectedBlue[56] = 130;
            ExpectedRed[57] = 255;  ExpectedGreen[57] = 255;    ExpectedBlue[57] = 240;
            ExpectedRed[58] = 240;  ExpectedGreen[58] = 230;    ExpectedBlue[58] = 140;
            ExpectedRed[59] = 230;  ExpectedGreen[59] = 230;    ExpectedBlue[59] = 250;
            ExpectedRed[60] = 255;  ExpectedGreen[60] = 240;    ExpectedBlue[60] = 245;
            ExpectedRed[61] = 124;  ExpectedGreen[61] = 252;    ExpectedBlue[61] = 0;
            ExpectedRed[62] = 255;  ExpectedGreen[62] = 250;    ExpectedBlue[62] = 205;
            ExpectedRed[63] = 173;  ExpectedGreen[63] = 216;    ExpectedBlue[63] = 230;
            ExpectedRed[64] = 240;  ExpectedGreen[64] = 128;    ExpectedBlue[64] = 128;
            ExpectedRed[65] = 224;  ExpectedGreen[65] = 255;    ExpectedBlue[65] = 255;
            ExpectedRed[66] = 250;  ExpectedGreen[66] = 250;    ExpectedBlue[66] = 210;
            ExpectedRed[67] = 144;  ExpectedGreen[67] = 238;    ExpectedBlue[67] = 144;
            ExpectedRed[68] = 211;  ExpectedGreen[68] = 211;    ExpectedBlue[68] = 211;
            ExpectedRed[69] = 255;  ExpectedGreen[69] = 182;    ExpectedBlue[69] = 193;
            ExpectedRed[70] = 255;  ExpectedGreen[70] = 160;    ExpectedBlue[70] = 122;
            ExpectedRed[71] = 32;   ExpectedGreen[71] = 178;    ExpectedBlue[71] = 170;
            ExpectedRed[72] = 135;  ExpectedGreen[72] = 206;    ExpectedBlue[72] = 250;
            ExpectedRed[73] = 119;  ExpectedGreen[73] = 136;    ExpectedBlue[73] = 153;
            ExpectedRed[74] = 176;  ExpectedGreen[74] = 196;    ExpectedBlue[74] = 222;
            ExpectedRed[75] = 255;  ExpectedGreen[75] = 255;    ExpectedBlue[75] = 224;
            ExpectedRed[76] = 0;    ExpectedGreen[76] = 255;    ExpectedBlue[76] = 0;
            ExpectedRed[77] = 50;   ExpectedGreen[77] = 205;    ExpectedBlue[77] = 50;
            ExpectedRed[78] = 250;  ExpectedGreen[78] = 240;    ExpectedBlue[78] = 230;
            ExpectedRed[79] = 255;  ExpectedGreen[79] = 0;      ExpectedBlue[79] = 255;
            ExpectedRed[80] = 128;  ExpectedGreen[80] = 0;      ExpectedBlue[80] = 0;
            ExpectedRed[81] = 102;  ExpectedGreen[81] = 205;    ExpectedBlue[81] = 170;
            ExpectedRed[82] = 0;    ExpectedGreen[82] = 0;      ExpectedBlue[82] = 205;
            ExpectedRed[83] = 186;  ExpectedGreen[83] = 85;     ExpectedBlue[83] = 211;
            ExpectedRed[84] = 147;  ExpectedGreen[84] = 112;    ExpectedBlue[84] = 219;
            ExpectedRed[85] = 60;   ExpectedGreen[85] = 179;    ExpectedBlue[85] = 113;
            ExpectedRed[86] = 123;  ExpectedGreen[86] = 104;    ExpectedBlue[86] = 238;
            ExpectedRed[87] = 0;    ExpectedGreen[87] = 250;    ExpectedBlue[87] = 154;
            ExpectedRed[88] = 72;   ExpectedGreen[88] = 209;    ExpectedBlue[88] = 204;
            ExpectedRed[89] = 199;  ExpectedGreen[89] = 21;     ExpectedBlue[89] = 133;
            ExpectedRed[90] = 25;   ExpectedGreen[90] = 25;     ExpectedBlue[90] = 112;
            ExpectedRed[91] = 245;  ExpectedGreen[91] = 255;    ExpectedBlue[91] = 250;
            ExpectedRed[92] = 255;  ExpectedGreen[92] = 228;    ExpectedBlue[92] = 225;
            ExpectedRed[93] = 255;  ExpectedGreen[93] = 228;    ExpectedBlue[93] = 181;
            ExpectedRed[94] = 255;  ExpectedGreen[94] = 222;    ExpectedBlue[94] = 173;
            ExpectedRed[95] = 0;    ExpectedGreen[95] = 0;      ExpectedBlue[95] = 128;
            ExpectedRed[96] = 253;  ExpectedGreen[96] = 245;    ExpectedBlue[96] = 230;
            ExpectedRed[97] = 128;  ExpectedGreen[97] = 128;    ExpectedBlue[97] = 0;
            ExpectedRed[98] = 107;  ExpectedGreen[98] = 142;    ExpectedBlue[98] = 35;
            ExpectedRed[99] = 255;  ExpectedGreen[99] = 165;    ExpectedBlue[99] = 0;
            ExpectedRed[100] = 255; ExpectedGreen[100] = 69;    ExpectedBlue[100] = 0;
            ExpectedRed[101] = 218; ExpectedGreen[101] = 112;   ExpectedBlue[101] = 214;
            ExpectedRed[102] = 238; ExpectedGreen[102] = 232;   ExpectedBlue[102] = 170;
            ExpectedRed[103] = 152; ExpectedGreen[103] = 251;   ExpectedBlue[103] = 152;
            ExpectedRed[104] = 175; ExpectedGreen[104] = 238;   ExpectedBlue[104] = 238;
            ExpectedRed[105] = 219; ExpectedGreen[105] = 112;   ExpectedBlue[105] = 147;
            ExpectedRed[106] = 255; ExpectedGreen[106] = 239;   ExpectedBlue[106] = 213;
            ExpectedRed[107] = 255; ExpectedGreen[107] = 218;   ExpectedBlue[107] = 185;
            ExpectedRed[108] = 205; ExpectedGreen[108] = 133;   ExpectedBlue[108] = 63;
            ExpectedRed[109] = 255; ExpectedGreen[109] = 192;   ExpectedBlue[109] = 203;
            ExpectedRed[110] = 221; ExpectedGreen[110] = 160;   ExpectedBlue[110] = 221;
            ExpectedRed[111] = 176; ExpectedGreen[111] = 224;   ExpectedBlue[111] = 230;
            ExpectedRed[112] = 128; ExpectedGreen[112] = 0;     ExpectedBlue[112] = 128;
            ExpectedRed[113] = 255; ExpectedGreen[113] = 0;     ExpectedBlue[113] = 0;
            ExpectedRed[114] = 188; ExpectedGreen[114] = 143;   ExpectedBlue[114] = 143;
            ExpectedRed[115] = 65;  ExpectedGreen[115] = 105;   ExpectedBlue[115] = 225;
            ExpectedRed[116] = 139; ExpectedGreen[116] = 69;    ExpectedBlue[116] = 19;
            ExpectedRed[117] = 250; ExpectedGreen[117] = 128;   ExpectedBlue[117] = 114;
            ExpectedRed[118] = 244; ExpectedGreen[118] = 164;   ExpectedBlue[118] = 96;
            ExpectedRed[119] = 46;  ExpectedGreen[119] = 139;   ExpectedBlue[119] = 87;
            ExpectedRed[120] = 255; ExpectedGreen[120] = 245;   ExpectedBlue[120] = 238;
            ExpectedRed[121] = 160; ExpectedGreen[121] = 82;    ExpectedBlue[121] = 45;
            ExpectedRed[122] = 192; ExpectedGreen[122] = 192;   ExpectedBlue[122] = 192;
            ExpectedRed[123] = 135; ExpectedGreen[123] = 206;   ExpectedBlue[123] = 235;
            ExpectedRed[124] = 106; ExpectedGreen[124] = 90;    ExpectedBlue[124] = 205;
            ExpectedRed[125] = 112; ExpectedGreen[125] = 128;   ExpectedBlue[125] = 144;
            ExpectedRed[126] = 255; ExpectedGreen[126] = 250;   ExpectedBlue[126] = 250;
            ExpectedRed[127] = 0;   ExpectedGreen[127] = 255;   ExpectedBlue[127] = 127;
            ExpectedRed[128] = 70;  ExpectedGreen[128] = 130;   ExpectedBlue[128] = 180;
            ExpectedRed[129] = 210; ExpectedGreen[129] = 180;   ExpectedBlue[129] = 140;
            ExpectedRed[130] = 0;   ExpectedGreen[130] = 128;   ExpectedBlue[130] = 128;
            ExpectedRed[131] = 216; ExpectedGreen[131] = 191;   ExpectedBlue[131] = 216;
            ExpectedRed[132] = 255; ExpectedGreen[132] = 99;    ExpectedBlue[132] = 71;
            ExpectedRed[133] = 255; ExpectedGreen[133] = 255;   ExpectedBlue[133] = 255;
            ExpectedRed[134] = 64;  ExpectedGreen[134] = 224;   ExpectedBlue[134] = 208;
            ExpectedRed[135] = 238; ExpectedGreen[135] = 130;   ExpectedBlue[135] = 238;
            ExpectedRed[136] = 245; ExpectedGreen[136] = 222;   ExpectedBlue[136] = 179;
            ExpectedRed[137] = 255; ExpectedGreen[137] = 255;   ExpectedBlue[137] = 255;
            ExpectedRed[138] = 245; ExpectedGreen[138] = 245;   ExpectedBlue[138] = 245;
            ExpectedRed[139] = 255; ExpectedGreen[139] = 255;   ExpectedBlue[139] = 0;
            ExpectedRed[140] = 154; ExpectedGreen[140] = 205;   ExpectedBlue[140] = 50;
            ExpectedRed[141] = 128; ExpectedGreen[141] = 128;   ExpectedBlue[141] = 128;
            ExpectedRed[142] = 128; ExpectedGreen[142] = 128;   ExpectedBlue[142] = 255;
            ExpectedRed[143] = 0;   ExpectedGreen[143] = 128;   ExpectedBlue[143] = 128;
            #endregion
        }
        public bool Test(int i)
        {
            if( myColor[i].R != ExpectedRed[i] || myColor[i].R != ExpectedRed[i] || myColor[i].R != ExpectedRed[i] )
            {
                return false;
            }
            else
                return true;
        }
    }
}


