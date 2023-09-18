// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Color class
//
using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ColorClass : ApiTest
    {
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
                profileUri = new Uri("file://" + WindowsColorProfiles + filename);
            }
            else
            {
                throw new ArgumentNullException("fileStream " + WindowsColorProfiles + filename);
            }

            return profileUri;
        }

        [EnvironmentPermission(SecurityAction.Assert, Read = "windir")]
        private static string GetSystemColorProfilesDirectory()
        {
            string s = Environment.GetEnvironmentVariable("windir") + @"\system32\spool\drivers\color\";

            return s.ToLower();
        }

        public WCP_ColorClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(Color);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            // Shared Variables for Color
            SolidColorBrush brush = new SolidColorBrush();
            bool comp1, comp2, comp3, comp4, comp5;

            // float arrays 
            float[] floats_in;
            float[] floats_in_diff;
            float[] bad_floats_in;

            // Layout Variables for rendering to surface
            Point P1 = new Point(10, 10);
            Point P2 = new Point(40, 40);
            Vector Vx = new Vector(40, 0);
            Vector Vx2 = new Vector(10, 0);
            Vector Vx3 = new Vector(20, 0);
            Vector Vxy = new Vector(-120, 40);
            Vector Vxy2 = new Vector(-141, 40);
            Vector Vxy3 = new Vector(-100, 40);

            // Exception strings
            string expected_exception = "Fail to get stream pointer for ICC file.";
            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(Color).Assembly);

            CommonLib.LogStatus(_objectType.ToString());

            #region SECTION I - PROPERTIES
            CommonLib.LogStatus("***** SECTION I - PROPERTIES *****");

            #region Test #1 - The A property
            // Usage: byte = Color.A (read/write)
            // Notes: Alpha color channel.
            CommonLib.LogStatus("Test #1 - The A property");

            // Create a White Color with Alpha = 0.
            Color C11 = Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF);

            // Set the A property 
            C11.A = 0xFF;

            // Get the A property
            byte alpha = C11.A;

            SolidColorBrush brush11 = new SolidColorBrush();
            brush11.Color = C11;
            DC.DrawRectangle(brush11, null, new Rect(P1, P2));

            // Check the A value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("A", alpha, 0xFF);

            // The Alpha property, both Set and Get, should work with an
            // AValues color.
            floats_in = new float[] { 1.0f, 0.5f, 0.5f };
            Color C11b = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the A property 
            C11b.A = 0xFF;

            // Get the A property
            byte alpha_context = C11b.A;

            // Check the A value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("A context", alpha_context, 0xFF);
            #endregion

            #region Test #2 - The R property
            // Usage: byte = Color.R (read/write)
            // Notes: Red Color channel.
            CommonLib.LogStatus("Test #2 - The R property");

            // Create a Black Color.
           Color C12 = Color.FromRgb(0x00, 0x00, 0x00);

            // Set the R property 
            C12.R = 0xFF;

            // Get the R property
            byte red = C12.R;

            SolidColorBrush brush12 = new SolidColorBrush();
            brush12.Color = C12;
            DC.DrawRectangle(brush12, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the R value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("R", red, 0xFF);

            // The R property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 0.0f, 1.0f, 1.0f };
           Color C12b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the R property
            try
            {
                C12b.R = 0xFF;
            }
            catch (System.Exception e)
            {
                //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
                //to the trouble of prepare all of those localized exception strings running on different locales.
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting R to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Setting R to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the R property
            try
            {
                byte red_context = C12b.R;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting R context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Getting R context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #3 - The G property
            // Usage: byte = Color.G (read/write)
            // Notes: Green color channel.
            CommonLib.LogStatus("Test #3 - The G property");

            // Create a Black Color.
           Color C13 = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);

            // Set the G property 
            C13.G = 0xFF;

            // Get the G property
            byte green = C13.G;

            SolidColorBrush brush13 = new SolidColorBrush();
            brush13.Color = C13;
            DC.DrawRectangle(brush13, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the G value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("G", green, 0xFF);

            // The G property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 1.0f, 0.5f, 1.0f };
           Color C13b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the G property
            try
            {
                C13b.G = 0xFF;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting G to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Setting G to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the G property
            try
            {
                byte green_context = C13b.G;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting G context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Getting G context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #4 - The B property
            // Usage: byte = Color.B (read/write)
            // Notes: Blue color channel.
            CommonLib.LogStatus("Test #4 - The B property");

            // Create a Black Color.
           Color C14 = Color.FromRgb(0x00, 0x00, 0x00);

            // Set the B property 
            C14.B = 0xFF;

            // Get the B property
            byte blue = C14.B;

            SolidColorBrush brush14 = new SolidColorBrush();
            brush14.Color = C14;
            DC.DrawRectangle(brush14, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the B value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("B", blue, 0xFF);

            // The B property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 1.0f, 1.0f, 0.1f };
           Color C14b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the B property
            try
            {
                C14b.B = 0xFF;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting B to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Setting B to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the B property
            try
            {
                byte blue_context = C14b.B;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting B context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Getting B context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #5 - The ScA property
            // Usage: float32 = Color.ScA (read/write)
            // Notes: (ScAGB) Red color channel.
            CommonLib.LogStatus("Test #5 - The ScA property");

            // Create a White Color with Alpha = 0.
           Color C15 = Color.FromScRgb(0.0f, 1.0f, 1.0f, 1.0f);

            // Set the ScA property 
            C15.ScA = 1.0f;

            // Get the ScA property
            float sca = C15.ScA;

            SolidColorBrush brush15 = new SolidColorBrush();
            brush15.Color = C15;
            DC.DrawRectangle(brush15, null, new Rect(P1 += Vxy, P2 += Vxy));

            // Check the ScA value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ScA", sca, 1.0f);

            // The ScA property, both Set and Get, should work with an
            // AValues color.
            floats_in = new float[] { 1.0f, 0.5f, 0.5f };
           Color C15b = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the A property 
            C15b.ScA = 1.0f;

            // Get the A property
            float sca_context = C15b.ScA;

            // Check the A value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ScA context", sca_context, 1.0f);
            #endregion

            #region Test #6 - The ScR property
            // Usage: float32 = Color.ScR (read/write)
            // Notes: (ScRGB) Red color channel.
            CommonLib.LogStatus("Test #6 - The ScR property");

            // Create a Black Color.
           Color C16 = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f);

            // Set the ScR property 
            C16.ScR = 1.0f;

            // Get the ScR property
            float scr = C16.ScR;

            SolidColorBrush brush16 = new SolidColorBrush();
            brush16.Color = C16;
            DC.DrawRectangle(brush16, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the ScR value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ScR", scr, 1.0f);

            // The ScR property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 0.0f, 1.0f, 1.0f };
           Color C16b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the ScR property
            try
            {
                C16b.ScR = 1.0f;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting ScR to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Setting ScR to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the ScR property
            try
            {
                float scr_context = C16b.ScR;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting ScR context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Getting ScR context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #7 - The ScG property
            // Usage: float32 = Color.ScG (read/write)
            // Notes: (ScRGB) Green color channel.
            CommonLib.LogStatus("Test #7 - The ScG property");

            // Create a Black Color.
           Color C17 = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f);

            // Set the ScG property 
            C17.ScG = 1.0f;

            // Get the ScG property
            float scg = C17.ScG;

            SolidColorBrush brush17 = new SolidColorBrush();
            brush17.Color = C17;
            DC.DrawRectangle(brush17, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the ScG value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ScG", scg, 1.0f);

            // The ScG property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 1.0f, 0.5f, 1.0f };
           Color C17b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the ScG property
            try
            {
                C17b.ScG = 0xFF;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting ScG to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Setting ScG to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the ScG property
            try
            {
                float scg_context = C17b.ScG;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting ScG context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Getting ScG context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #8 - The ScB property
            // Usage: float32 = Color.ScB (read/write)
            // Notes: ScRGB) Blue Color Channel.
            CommonLib.LogStatus("Test #8 - The ScB property");

            // Create a Black Color.
           Color C18 = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f);

            // Set the ScB property 
            C18.ScB = 1.0f;

            // Get the ScB property
            float scb = C18.ScB;

            SolidColorBrush brush18 = new SolidColorBrush();
            brush18.Color = C18;
            DC.DrawRectangle(brush18, null, new Rect(P1 += Vx, P2 += Vx));

            // Check the ScG value to assure that it was the one that was set.
            _class_testresult &= _helper.CompareProp("ScB", scb, 1.0f);

            // The ScB property, both Set and Get, should generate an exception
            // when called on a color that has a non RGB/ScRGB context.
            // AValues color.
            floats_in = new float[] { 1.0f, 1.0f, 0.1f };
           Color C18b = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Set the ScB property
            try
            {
                C18b.ScB = 0xFF;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Setting ScB to an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Setting ScB to an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // Get the ScB property
            try
            {
                float scb_context = C18b.ScB;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextNotsRGB_or_scRGB");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Getting ScB context from an invalid value returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Getting ScB context from an invalid value returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #9 - The ColorContext property
            CommonLib.LogStatus("Test #9 - The ColorContext property");

            // Create a Black Color.
           Color C19 = Color.FromScRgb(1.0f, 0.0f, 0.0f, 0.0f);

            if (C19.ColorContext == null)
            {
                CommonLib.LogStatus("Pass: ColorContext properly set");
            }
            else
            {
                CommonLib.LogStatus("Fail: ColorContext not set properly, it should be null");
                _class_testresult &= false;
            }

            #endregion
            #endregion End Of SECTION I

            #region SECTION II - METHODS
            CommonLib.LogStatus("***** SECTION II - METHODS *****");

            #region Test #1 - The Clamp Method
            // Usage: void = Color.Clamp()
            // Notes: Changes any A,R,G,B values that are >1 to be 1 and values < 0 to be 0.
            CommonLib.LogStatus("Test #1 - The Clamp Method");

            // Create a Color with all values > 1.
           Color C21a = Color.FromScRgb(1.5f, 2.0f, 1.01f, 10.0f);

            // Create a Color with all values < 0.
           Color C21b = Color.FromScRgb(-1.5f, -2.0f, -.001f, -10.0f);

            // Create a Color with a Context and all values > 1.
            floats_in = new float[] { 1.5f, 10.5f, 1.1f };
           Color C21c = Color.FromAValues(2.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Create a Color with a Context and all values < 0.
            floats_in = new float[] { -1.5f, -10.5f, -1.1f };
           Color C21d = Color.FromAValues(-2.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the Clamp method to create a White Color.
            C21a.Clamp();

            // Use the Clamp method to create a Transparent Color.
            C21b.Clamp();

            // Use the Clamp method to create a White Color.
            C21c.Clamp();

            // Use the Clamp method to create a Transparent Color.
            C21d.Clamp();

            // Check the Color values > 1 to assure that they were Clamped.
            _class_testresult &= _helper.CompareColorPropFloats("Clamp>1", C21a, 1.0f, 1.0f, 1.0f, 1.0f);

            // Check the Color values < 0 to assure that they were Clamped.
            _class_testresult &= _helper.CompareColorPropFloats("Clamp<0", C21b, 0.0f, 0.0f, 0.0f, 0.0f);

            // ******* Regression_Bug236 - ToString() doesn't work with AValues colors *******
            // Check the Color values > 1 to assure that they were Clamped.
            //class_testresult &= helper.CompareProp("Clamp>1 (context)", C21c.ToString(), "sc#FFFFFFFFFFFFFFFF"); 

            // Check the Color values < 0 to assure that they were Clamped.
            //class_testresult &= helper.CompareProp("Clamp<0 (context)", C21d.ToString(), "sc#0000000000000000");
            #endregion

            #region Test #2 - The Equals Method
            // Usage: bool = Color.Equals(object)
            // Notes: Indicates whether the Color is equal in type and value to the object in the argument.
            CommonLib.LogStatus("Test #2 - The Equals Method");

            // Create three ScRGB Colors, two with identical values. And one other type of object.
           Color C22a_ScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.0f);
           Color C22b_ScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.0f);
           Color C22c_ScRGB = Color.FromScRgb(1.0f, 0.99f, 0.4f, 0.0f);
            Point P22 = new Point(0.0, 1.0);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors and the other object.
            comp1 = C22a_ScRGB.Equals(C22b_ScRGB);
            comp2 = C22a_ScRGB.Equals(C22c_ScRGB);
            comp3 = C22a_ScRGB.Equals(P22);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals ScRGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals ScRGB 2", comp2, false);
            _class_testresult &= _helper.CompareProp("Equals ScRGB 3", comp3, false);

            // Create three ARGB Colors, two with identical values. 
           Color C22a_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x00);
           Color C22b_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x00);
           Color C22c_ARGB = Color.FromArgb(0xFF, 0xFE, 0x66, 0x00);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors and the other object.
            comp1 = C22a_ARGB.Equals(C22b_ARGB);
            comp2 = C22a_ARGB.Equals(C22c_ARGB);
            comp3 = C22a_ARGB.Equals(P22);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals ARGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals ARGB 2", comp2, false);
            _class_testresult &= _helper.CompareProp("Equals ARGB 3", comp3, false);

            // Create three RGB Colors, two with identical values. 
           Color C22a_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C22b_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C22c_RGB = Color.FromRgb(0xFF, 0x65, 0x22);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors and the other object.
            comp1 = C22a_RGB.Equals(C22b_RGB);
            comp2 = C22a_RGB.Equals(C22c_RGB);
            comp3 = C22a_RGB.Equals(P22);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals RGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals RGB 2", comp2, false);
            _class_testresult &= _helper.CompareProp("Equals RGB 3", comp3, false);

            // Use the Equals Method when the object argument is null. Should return false.
            comp1 = C22a_ScRGB.Equals(null);
            comp2 = C22a_ARGB.Equals(null);
            comp3 = C22a_RGB.Equals(null);

            _class_testresult &= _helper.CompareProp("Equals ScRGB Null", comp1, false);
            _class_testresult &= _helper.CompareProp("Equals ARGB Null", comp2, false);
            _class_testresult &= _helper.CompareProp("Equals RGB Null", comp3, false);
            #endregion

            #region Test #3 - The GetHashCode Method
            // Usage: int32 = GetHashCode(void)
            // Notes: Returns a hash code for the Color object. Two references to the same Color
            // have the same hash code.
            CommonLib.LogStatus("Test #3 - The GetHashCode Method");

            // Create three Colors, two with identical values.
           Color C23a = Color.FromScRgb(1.0f, 0.3f, 0.4f, 1.0f);
           Color C23b = Color.FromScRgb(1.0f, 0.3f, 0.4f, 1.0f);
           Color C23c = Color.FromScRgb(1.0f, 0.299f, 0.4f, 1.0f);

            // Retrieve the hash codes for the three colors by using the GetHashCode
            // Method.  The hash codes for the first two colors should be the same.
            int hash1 = C23a.GetHashCode();
            int hash2 = C23b.GetHashCode();
            int hash3 = C23c.GetHashCode();
            comp1 = (hash1 == hash2);
            comp2 = (hash1 == hash3);

            // The first two HashCodes should be the same and the third different.
            _class_testresult &= _helper.CompareProp("HashCode Part1", comp1, true);
            _class_testresult &= _helper.CompareProp("HashCode Part2", comp2, false);
            #endregion

            #region Test #4 - The AreClose Method
            // Usage: bool = Color.AreClose(Color1, Color2)
            // Notes: Indicates whether a Color1 is close to Color2.
            // Note: 1.000001f is "Close" to 1.000000f.
            CommonLib.LogStatus("Test #4 - The AreClose Method");

            // Create two Colors that are similar in value and "Close".
           Color C34a = Color.FromScRgb(1.0f, 1.000001f, 0.0f, 0.0f);
           Color C34b = Color.FromScRgb(1.0f, 1.000000f, 0.0f, 0.0f);

            // Compare the two Colors to determine if they are "Close".
            comp1 = Color.AreClose(C34a, C34b);

            // The comparison should be true.
            _class_testresult &= _helper.CompareProp("AreClose", comp1, true);

            // Create two Colors with Context that are similar in value and "Close".
            floats_in = new float[] { 1.0f, 1.000001f, 0.0f };
           Color C34c = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Create a Color with a Context and all values < 0.
            floats_in = new float[] { 1.0f, 1.000000f, 0.0f };
           Color C34d = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Compare the two Colors to determine if they are "Close".
            comp2 = Color.AreClose(C34c, C34d);

            // The comparison should be true.
            _class_testresult &= _helper.CompareProp("AreClose (context)", comp2, true);
            #endregion

            #region Test #5 - The GetColorContextColor Method - obsolete method
            #endregion

            #region Test #6 - The ToString Method
            // Usage: string = ToString(void)
            // Notes: Returns a string representation of the Color values.
            CommonLib.LogStatus("Test #6 - The ToString Method");

            // Create three Colors, each using a different method.
           Color C36a = Color.FromRgb(0x30, 0xAA, 0xCC);
           Color C36b = Color.FromArgb(0x05, 0x80, 0x40, 0xFF);
           Color C36c = Color.FromScRgb(1.0f, 1.0f, 1.0f, 0.0f);

            // Get the string representations of the Colors
            string ToString1 = C36a.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string ToString2 = C36b.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string ToString3 = C36c.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Check the values of ToString against the expected values.
            _class_testresult &= _helper.CompareProp("ToString FromRgb", ToString1, "#FF30AACC");
            _class_testresult &= _helper.CompareProp("ToString FromArgb", ToString2, "#058040FF");

            string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string myColorLocalized = "sc#1"+sep+" 1"+sep+" 1"+sep+" 0";

            // String will be in a different format if the color has been constructed with ScRgb data
            // (usually if running under HW)
            // The FF.. test value comes from antiqiuty, and needs to be verified against allowable behavior.
            if (ToString3 =="#FFFFFF00")
            {
                _class_testresult &= _helper.CompareProp("ToString FromScRgb", ToString3, "#FFFFFF00");
            }
            if (ToString3 == myColorLocalized) 
            {
                _class_testresult &= _helper.CompareProp("ToString FromScRgb", ToString3, myColorLocalized);
            }   
            else
            {     
	            _class_testresult &= _helper.CompareProp("ToString FromScRgb", ToString3, "#FFFFFF00, or "+myColorLocalized);
            }    
	
            #endregion

            #region Test #7 - The ToString Method (IFormatProvider)
            // Usage: string = ToString(IFormatProvider)
            // Notes: Returns a string representation of the Color values that is formatted
            // acording to the format provider.
            CommonLib.LogStatus("Test #7 - The ToString Method (IFormatProvider)");

            // Create three Colors, each using a different method.
           Color C37a = Color.FromRgb(0x30, 0xAA, 0xCC);
           Color C37b = Color.FromArgb(0x05, 0x80, 0x40, 0xFF);
           Color C37c = Color.FromScRgb(1.0f, 1.0f, 1.0f, 0.0f);

            // Get the string representations of the Colors
            string ToStringfp1 = C37a.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string ToStringfp2 = C37b.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string ToStringfp3 = C37c.ToString(System.Globalization.CultureInfo.InvariantCulture); 

            // ******* Regression_Bug236 - ToString() doesn't work with AValues colors *******
            // Create an AValues color to provide a context.
            //floats_in = new float[] { 1.0f, 1.0f, 0.0f };
            //Color C37d = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            //string ToStringfp4 = C37d.ToString(provider);
            //class_testresult &= helper.CompareProp("ToString (FP) FromAValues", ToStringfp4, "sc#FFFFFFFFFFFF0000");

            // Check the values of ToString against the expected values.
            _class_testresult &= _helper.CompareProp("ToString (FP) FromRgb", ToStringfp1, "#FF30AACC");
            _class_testresult &= _helper.CompareProp("ToString (FP) FromArgb", ToStringfp2, "#058040FF");

            // String will be in a different format if the color has been constructed with ScRgb data
            // (usually if running under HW).
            // The FF.. test value comes from antiqiuty, and needs to be verified against allowable behavior.
            if (ToStringfp3 =="#FFFFFF00")
            {
                _class_testresult &= _helper.CompareProp("ToString (FP) FromScRgb", ToStringfp3, "#FFFFFF00");
            }            
            if (ToStringfp3 == myColorLocalized) 
            {
                _class_testresult &= _helper.CompareProp("ToString (FP) FromScRgb", ToStringfp3, myColorLocalized);
            } 
            else
            {     
                _class_testresult &= _helper.CompareProp("ToString FromScRgb", ToStringfp3, "#FFFFFF00, or "+myColorLocalized);
            }   
			
            #endregion

            #region Test #8 - The GetNativeColorValues Method
            CommonLib.LogStatus("Test #8 - The GetNativeColorValues Method");
            TestGetNativeColorValues();

            #endregion

            #endregion End Of SECTION II

            #region SECTION III - STATIC METHODS
            CommonLib.LogStatus("***** SECTION III - STATIC METHODS *****");

            #region Test #1 - The Add Method
            // Usage: Color = Color.Add(Color, Color)
            // Notes: Adds the values of two Color objects to create a new Color object. Uses function call syntax.
            CommonLib.LogStatus("Test #1 - The Add Method");

            // Create two ScRGB Colors
           Color C31a_FromScRGB = Color.FromScRgb(0.5f, 1.0f, 0.4f, 0.0f);
           Color C31b_FromScRGB = Color.FromScRgb(0.5f, 0.0f, 0.6f, 1.0f);

            // Create two ARGB Colors
           Color C31a_FromARGB = Color.FromArgb(0x77, 0xFF, 0x66, 0x00);
           Color C31b_FromARGB = Color.FromArgb(0x88, 0x00, 0x99, 0xFF);

            // Create two RGB Colors
           Color C31a_FromRGB = Color.FromRgb(0xFF, 0x66, 0x00);
           Color C31b_FromRGB = Color.FromRgb(0x00, 0x99, 0xFF);

            // Use the Add method to combine the two ScRGB Colors and create a third.
           Color C31c_FromScRGB = Color.Add(C31a_FromScRGB, C31b_FromScRGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush311 = new SolidColorBrush();
            brush311.Color = C31c_FromScRGB;
            DC.DrawRectangle(brush311, null, new Rect(P1 += Vxy, P2 += Vxy2));

            // Use the Add method to combine the two ARGB Colors and create a third.
           Color C31c_FromARGB = Color.Add(C31a_FromARGB, C31b_FromARGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush312 = new SolidColorBrush();
            brush312.Color = C31c_FromARGB;
            DC.DrawRectangle(brush312, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the Add method to combine the two RGB Colors and create a third.
           Color C31c_FromRGB = Color.Add(C31a_FromRGB, C31b_FromRGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush313 = new SolidColorBrush();
            brush313.Color = C31c_FromRGB;
            DC.DrawRectangle(brush313, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Add ScRGB", C31c_FromScRGB, (0.5f + 0.5f), (1.0f + 0.0f), (0.4f + 0.6f), (0.0f + 1.0f));
            _class_testresult &= _helper.CompareColorPropInts("Add ARGB", C31c_FromARGB, (0x77 + 0x88), _helper.AddRGBColorChannel(0xFF, 0x00), _helper.AddRGBColorChannel(0x66, 0x99), _helper.AddRGBColorChannel(0x00, 0xFF));
            _class_testresult &= _helper.CompareColorPropInts("Add RGB", C31c_FromRGB, 0xFF, _helper.AddRGBColorChannel(0xFF, 0x00), _helper.AddRGBColorChannel(0x66, 0x99), _helper.AddRGBColorChannel(0x00, 0xFF));
            #endregion

            #region Test #2 - The Multiply Method
            // Usage: Color = Color.Multiply(Color, float32)
            // Notes: Multiplies each Color channel value by a float32 to create a new Color object.
            // Uses function call syntax.
            CommonLib.LogStatus("Test #2 - The Multiply Method");

            // Create an ScRGB Color, ARGB Color, RGB Color and two multipliers
           Color C32a_FromScRGB = Color.FromScRgb(0.5f, 0.5f, 0.1f, 0.1f);
            float multiplier_float = 2.0f;
           Color C32a_FromARGB = Color.FromArgb(0x7F, 0x7F, 0x19, 0x19);
            byte multiplier_byte = 0x02;
           Color C32a_FromRGB = Color.FromRgb(0x7F, 0x19, 0x19);

            // Use the Multiply method to combine each Color with the multiplier
            // and create a new Color.
           Color C32b_FromScRGB = Color.Multiply(C32a_FromScRGB, multiplier_float);
           Color C32b_FromARGB = Color.Multiply(C32a_FromARGB, multiplier_byte);
           Color C32b_FromRGB = Color.Multiply(C32a_FromRGB, multiplier_byte);

            // Fill a Rectangle with each Color
            SolidColorBrush brush321 = new SolidColorBrush();
            brush321.Color = C32b_FromScRGB;
            DC.DrawRectangle(brush321, null, new Rect(P1 += Vx3, P2 += Vx3));

            SolidColorBrush brush322 = new SolidColorBrush();
            brush322.Color = C32b_FromARGB;
            DC.DrawRectangle(brush322, null, new Rect(P1 += Vx2, P2 += Vx2));

            SolidColorBrush brush323 = new SolidColorBrush();
            brush323.Color = C32b_FromRGB;
            DC.DrawRectangle(brush323, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Multiply ScRGB", C32b_FromScRGB, (0.5f * multiplier_float), (0.5f * multiplier_float), (0.1f * multiplier_float), (0.1f * multiplier_float));
            _class_testresult &= _helper.CompareColorPropInts("Multiply ARGB", C32b_FromARGB, (0x7F * multiplier_byte), _helper.MultiplyRGBColorChannel(0x7F, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte));
            _class_testresult &= _helper.CompareColorPropInts("Multiply RGB", C32b_FromRGB, 0xFF, _helper.MultiplyRGBColorChannel(0x7F, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte));
            #endregion

            #region Test #3 - The Subtract Method
            // Usage: Color = Color.Subtract(Color, Color)
            // Notes: Subtracts the values of one Color object from another to create a new Color object.
            // Uses function call syntax.
            CommonLib.LogStatus("Test #3 - The Subtract Method");

            // Create two ScRGB Colors
           Color C33a_FromScRGB = Color.FromScRgb(1.0f, 0.7f, 0.6f, 0.8f);
           Color C33b_FromScRGB = Color.FromScRgb(0.5f, 1.0f, 0.6f, 0.1f);

            // Create two ARGB Colors
           Color C33a_FromARGB = Color.FromArgb(0xFF, 0xB2, 0x99, 0xCC);
           Color C33b_FromARGB = Color.FromArgb(0x80, 0xFF, 0x99, 0x19);

            // Create two RGB Colors
           Color C33a_FromRGB = Color.FromRgb(0xB2, 0x99, 0xCC);
           Color C33b_FromRGB = Color.FromRgb(0xFF, 0x99, 0x19);

            // Use the Subtract method to combine the two ScRGB Colors and create a third.
           Color C33c_FromScRGB = Color.Subtract(C33a_FromScRGB, C33b_FromScRGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush331 = new SolidColorBrush();
            brush331.Color = C33c_FromScRGB;
            DC.DrawRectangle(brush331, null, new Rect(P1 += Vx3, P2 += Vx3));

            // Use the Subtract method to combine the two ARGB Colors and create a third.
           Color C33c_FromARGB = Color.Subtract(C33a_FromARGB, C33b_FromARGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush332 = new SolidColorBrush();
            brush332.Color = C33c_FromARGB;
            DC.DrawRectangle(brush332, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the Subtract method to combine the two RGB Colors and create a third.
           Color C33c_FromRGB = Color.Subtract(C33a_FromRGB, C33b_FromRGB);

            // Fill a Rectangle with the Color
            SolidColorBrush brush333 = new SolidColorBrush();
            brush333.Color = C33c_FromRGB;
            DC.DrawRectangle(brush333, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Subtract ScRGB", C33c_FromScRGB, (1.0f - 0.5f), (0.7f - 1.0f), (0.6f - 0.6f), (0.8f - 0.1f));
            _class_testresult &= _helper.CompareColorPropInts("Subtract ARGB", C33c_FromARGB, (0xFF - 0x80), _helper.SubtractRGBColorChannel(0xB2, 0xFF), _helper.SubtractRGBColorChannel(0x99, 0x99), _helper.SubtractRGBColorChannel(0xCC, 0x19));
            _class_testresult &= _helper.CompareColorPropInts("Subtract RGB", C33c_FromRGB, 0x00, _helper.SubtractRGBColorChannel(0xB2, 0xFF), _helper.SubtractRGBColorChannel(0x99, 0x99), _helper.SubtractRGBColorChannel(0xCC, 0x19));
            #endregion

            #region Test #4 - The Equals Method
            // Usage: bool = Color.Equals(Color, Color)
            // Notes: Indicates whether the Color channel values of one Color is equal to those in another.
            // Uses  function call syntax.
            CommonLib.LogStatus("Test #4 - The Equals Method");

            // Create three Colors, two with identical values. 
           Color C34a_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C34b_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C34c_FromScRGB = Color.FromScRgb(0.99f, 1.0f, 0.4f, 0.2f);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors.
            comp1 = Color.Equals(C34a_FromScRGB, C34b_FromScRGB);
            comp2 = Color.Equals(C34a_FromScRGB, C34c_FromScRGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals ScRGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals ScRGB 2", comp2, false);

            // Create three ARGB Colors, two with identical values. 
           Color C34a_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x22);
           Color C34b_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x22);
           Color C34c_ARGB = Color.FromArgb(0xFF, 0xFF, 0x65, 0x22);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors.
            comp1 = Color.Equals(C34a_ARGB, C34b_ARGB);
            comp2 = Color.Equals(C34a_ARGB, C34c_ARGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals ARGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals ARGB 2", comp2, false);

            // Create three RGB Colors, two with identical values. 
           Color C34a_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C34b_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C34c_RGB = Color.FromRgb(0xFF, 0x66, 0x21);

            // Use the Equals Method to compare the equality of the first color to
            // the other two colors.
            comp1 = Color.Equals(C34a_RGB, C34b_RGB);
            comp2 = Color.Equals(C34a_RGB, C34c_RGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equals RGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equals RGB 2", comp2, false);
            #endregion

            #region Test #5 - The AreClose Method
            // Usage: bool = Color.AreClose(Color, Color)
            // Notes: Indicates whether two Color objects are "Close" in value. 
            // 1.000001f  and 1.000000f are "Close".
            CommonLib.LogStatus("Test #5 - The AreClose Method");

            // Create two Colors that are similar in value and "Close".
           Color C35a = Color.FromScRgb(1.000001f, 1.0f, 0.0f, 0.0f);
           Color C35b = Color.FromScRgb(1.000000f, 1.0f, 0.0f, 0.0f);

            // Compare the two Colors to determine if they are "Close".
            comp1 = Color.AreClose(C35a, C35b);

            // The comparison should be true.
            _class_testresult &= _helper.CompareProp("AreClose", comp1, true);
            #endregion

            #region Test #6 - The FromArgb Method
            // Usage: Color = Color.FromArgb(byte, byte, byte, byte)
            // Notes: Returns a new Color object based on byte argument values for (Alpha, Red, Green, Blue)
            CommonLib.LogStatus("Test #6 - The FromArgb Method");

            // Create a Color from ARGB arguments.
           Color C36 = Color.FromArgb(0xFF, 0xFF, 0xAA, 0x01);

            // Check the Color values against the expected values.
            _class_testresult &= _helper.CompareColorPropInts("FromArgb", C36, 0xFF, 0xFF, 0xAA, 0x01);
            #endregion

            #region Test #7 - The FromRgb Method
            // Usage: Color = Color.FromRgb(byte, byte, byte)
            // Notes: Returns a new Color object with opaque Alpha based on byte argument values for (Red, Green, Blue)
            CommonLib.LogStatus("Test #7 - The FromRgb Method");

            // Create a Color from ARGB arguments.
           Color C37 = Color.FromRgb(0x80, 0xDD, 0x21);

            // Check the Color values against the expected values. (Assume Alpha is 0xFF)
            _class_testresult &= _helper.CompareColorPropInts("FromRgb", C37, 0xFF, 0x80, 0xDD, 0x21);
            #endregion

            #region Test #8 - The FromScRgb Method
            // Usage: Color = Color.FromScRgb(float32, float32, float32, float32)
            // Notes: Returns a Color (scrgb) object based on argument values for (Alpha, Red, Green, Blue)
            CommonLib.LogStatus("Test #8 - The FromScRgb Method");

            // Create a Color from ScRGB arguments.
           Color C38a = Color.FromScRgb(0.8f, 0.5f, 0.7f, 0.2f);

            // Check the Color values against the expected values. (Assume Alpha is 0xFF)
            _class_testresult &= _helper.CompareColorPropFloats("FromScRgb", C38a, 0.8f, 0.5f, 0.7f, 0.2f);

            // Create a Color from ScRGB arguments.
           Color C38b = Color.FromScRgb(1.8f, 0.5f, 0.7f, 0.2f);

            // Check the Color values against the expected values. (Assume Alpha is 0xFF)
            _class_testresult &= _helper.CompareColorPropFloats("FromScRgb", C38b, 1.8f, 0.5f, 0.7f, 0.2f);

            // Create a Color from ScRGB arguments.
           Color C38c = Color.FromScRgb(-0.8f, 0.5f, 0.7f, 0.2f);

            // Check the Color values against the expected values. (Assume Alpha is 0xFF)
            _class_testresult &= _helper.CompareColorPropFloats("FromScRgb", C38c, -0.8f, 0.5f, 0.7f, 0.2f);

            #endregion

            #region Test #9 - The FromAValues Method
            // Usage: Color = Color.FromAValues(float32, float[], Uri)
            // Notes: Returns a Color object based on an Alpha value, an array of values
            // and a color profile file name.
            CommonLib.LogStatus("Test #9 - The FromAValues Method");

            // Create two float arrays          
            floats_in = new float[] { 1.0f, 0.5f, 0.5f };
            bad_floats_in = new float[] { 1.0f, 0.7f };

            // Basic Test
            // If the ICC profile is not found on the machine, then expect
            // a specific exception message.
            try
            {
                // Create a Color with one float array and a profile name
               Color C39 = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C39, _objectType);
            }
            catch (System.Exception e)
            {
                if (e.Message == expected_exception)
                {
                    CommonLib.LogStatus("Pass: FromAValues returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: FromAValues returns " + e.Message + " should be " + expected_exception);
                    _class_testresult &= false;
                }
            }

            // Test with Alpha out of gamut
            // If the ICC profile is not found on the machine, then expect
            // a specific exception message.
            try
            {
                // Create a Color with one float array and a profile name
               Color C39aoog = Color.FromAValues(1.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C39aoog, _objectType);

                // Create a Color with one float array and a profile name
               Color C39boog = Color.FromAValues(-0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C39boog, _objectType);
            }
            catch (System.Exception e)
            {
                if (e.Message == expected_exception)
                {
                    CommonLib.LogStatus("Pass: FromAValues returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: FromAValues returns " + e.Message + " should be " + expected_exception);
                    _class_testresult &= false;
                }
            }

            // Test with wrong number of values for the profile.
            // If AValues is given the wrong number of dimensions for the ICC profile
            // then expect a specific exception message.
            try
            {
                // Create a Color with one float array and a profile name
               Color C39bf = Color.FromAValues(1.0f, bad_floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C39bf, _objectType);
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_DimensionMismatch");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: FromAValues returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: FromAValues returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            try
            {
                // Create a Color with one float array and a profile name
               Color C39null = Color.FromAValues(1.0f, null, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C39null, _objectType);
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_DimensionMismatch");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: FromAValues returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: FromAValues returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #10 - The FromValues Method
            // Usage: Color = Color.FromValues(float[], Uri)
            // Notes: Returns a Color object based on an array of values and a color profile
            // file name.
            CommonLib.LogStatus("Test #10 - The FromValues Method");

            // Create a float array
            floats_in = new float[] { 1.0f, 0.0f, 1.0f };

            // If the ICC profile is not found on the machine, then expect
            // a specific exception message.
            try
            {
                // Create a Color with one float array and a profile name
               Color C310 = Color.FromValues(floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

                // Confirm that a Color was successfully created
                _class_testresult &= _helper.CheckType(C310, _objectType);
            }
            catch (System.Exception e)
            {
                if (e.Message == expected_exception)
                {
                    CommonLib.LogStatus("Pass: FromValues returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: FromValues returns " + e.Message + " should be " + expected_exception);
                    _class_testresult &= false;
                }
            }
            #endregion
            #endregion End Of SECTION III

            #region SECTION IV - STATIC OVERLOADED OPERATORS
            CommonLib.LogStatus("***** SECTION IV - STATIC OVERLOADED OPERATORS *****");

            #region Test #1 - The Addition (+) Operator
            // Usage: Color = Color + Color
            // Notes: Adds the values of two Color objects to create a new Color object. 
            // Uses operator syntax.
            CommonLib.LogStatus("Test #1 - The Addition (+) Operator");

            // Create two ScRGB Colors
           Color C41a_FromScRGB = Color.FromScRgb(0.5f, 1.0f, 0.4f, 0.0f);
           Color C41b_FromScRGB = Color.FromScRgb(0.5f, 0.0f, 0.6f, 1.0f);

            // Create two ARGB Colors
           Color C41a_FromARGB = Color.FromArgb(0x80, 0xFF, 0x66, 0x00);
           Color C41b_FromARGB = Color.FromArgb(0x7F, 0x00, 0x99, 0xFF);

            // Create two RGB Colors
           Color C41a_FromRGB = Color.FromRgb(0xFF, 0x66, 0x00);
           Color C41b_FromRGB = Color.FromRgb(0x00, 0x99, 0xFF);

            // Create two AValues Colors.
            floats_in = new float[] { 1.0f, 0.5f, 0.0f };
           Color C41a_FromAValues = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            floats_in = new float[] { 0.0f, 0.5f, 1.0f };
           Color C41b_FromAValues = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the + Operator to combine the two ScRGB Colors and create a third.
           Color C41c_FromScRGB = C41a_FromScRGB + C41b_FromScRGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush411 = new SolidColorBrush();
            brush411.Color = C41c_FromScRGB;
            DC.DrawRectangle(brush411, null, new Rect(P1 += Vxy3, P2 += Vxy3));

            // Use the + Operator method to combine the two ARGB Colors and create a third.
           Color C41c_FromARGB = C41a_FromARGB + C41b_FromARGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush412 = new SolidColorBrush();
            brush412.Color = C41c_FromARGB;
            DC.DrawRectangle(brush412, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the + Operator method to combine the two RGB Colors and create a third.
           Color C41c_FromRGB = C41a_FromRGB + C41b_FromRGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush413 = new SolidColorBrush();
            brush413.Color = C41c_FromRGB;
            DC.DrawRectangle(brush413, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the + Operator method to combine the two AValues Colors and create a
            // third.  The Color can only be retrieved via ToString().
           Color C41c_FromAValues = C41a_FromAValues + C41b_FromAValues;

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Addition (+) ScRGB", C41c_FromScRGB, (0.5f + 0.5f), (1.0f + 0.0f), (0.4f + 0.6f), (0.0f + 1.0f));
            _class_testresult &= _helper.CompareColorPropInts("Addition (+) ARGB", C41c_FromARGB, (0x80 + 0x7F), _helper.AddRGBColorChannel(0xFF, 0x00), _helper.AddRGBColorChannel(0x66, 0x99), _helper.AddRGBColorChannel(0x00, 0xFF));
            _class_testresult &= _helper.CompareColorPropInts("Addition (+) RGB", C41c_FromRGB, 0xFF, _helper.AddRGBColorChannel(0xFF, 0x00), _helper.AddRGBColorChannel(0x66, 0x99), _helper.AddRGBColorChannel(0x00, 0xFF));

            try
            {
               Color C41d_FromAValues = C41a_FromAValues + C41b_FromRGB;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextTypeMismatch");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Adding colors with mistmatched context returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Adding colors with mistmatched context returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // ******* Regression_Bug236 - ToString() doesn't work with AValues colors *******
            //class_testresult &= helper.CompareProp ("Addition (+) AValues", C41c_FromAValues.ToString(), "sc#FFFFFFFF85E0FFFF");
            #endregion

            #region Test #2 - The Multiplication (*) Operator
            // Usage: Color = Color * float42
            // Notes: Multiplies each Color channel value by a float42 to create a new Color object. 
            // Uses operator syntax.
            CommonLib.LogStatus("Test #2 - The Multiplication (*) Operator");

            // Create an ScRGB Color, ARGB Color, RGB Color, AValues Color and
            // two multipliers
           Color C42a_FromScRGB = Color.FromScRgb(0.5f, 0.5f, 0.1f, 0.2f);
            multiplier_float = 2.0f;
           Color C42a_FromARGB = Color.FromArgb(0x7F, 0x7F, 0x19, 0x33);
            multiplier_byte = 0x02;
           Color C42a_FromRGB = Color.FromRgb(0x7F, 0x19, 0x33);
            floats_in = new float[] { 0.5f, 0.5f, 0.5f };
           Color C42a_FromAValues = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the * Operator to combine each Color with the multiplier
            // and create a new Color.
           Color C42b_FromScRGB = C42a_FromScRGB * multiplier_float;
           Color C42b_FromARGB = C42a_FromARGB * multiplier_byte;
           Color C42b_FromRGB = C42a_FromRGB * multiplier_byte;
           Color C42b_FromAValues = C42a_FromAValues * multiplier_float;

            // Fill a Rectangle with each Color (except for the AValues Color)
            SolidColorBrush brush421 = new SolidColorBrush();
            brush421.Color = C42b_FromScRGB;
            DC.DrawRectangle(brush421, null, new Rect(P1 += Vx3, P2 += Vx3));

            SolidColorBrush brush422 = new SolidColorBrush();
            brush422.Color = C42b_FromARGB;
            DC.DrawRectangle(brush422, null, new Rect(P1 += Vx2, P2 += Vx2));

            SolidColorBrush brush423 = new SolidColorBrush();
            brush423.Color = C42b_FromRGB;
            DC.DrawRectangle(brush423, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Multiplication (*) ScRGB", C42b_FromScRGB, (0.5f * multiplier_float), (0.5f * multiplier_float), (0.1f * multiplier_float), (0.2f * multiplier_float));
            _class_testresult &= _helper.CompareColorPropInts("Multiplication (*) ARGB", C42b_FromARGB, (0x7F * multiplier_byte), _helper.MultiplyRGBColorChannel(0x7F, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte), _helper.MultiplyRGBColorChannel(0x33, multiplier_byte));
            _class_testresult &= _helper.CompareColorPropInts("Multiplication (*) RGB", C42b_FromRGB, 0xFF, _helper.MultiplyRGBColorChannel(0x7F, multiplier_byte), _helper.MultiplyRGBColorChannel(0x19, multiplier_byte), _helper.MultiplyRGBColorChannel(0x33, multiplier_byte));

            // ******* Regression_Bug236 - ToString() doesn't work with AValues colors *******
            //class_testresult &= helper.CompareProp("Multiplication (*) AValues", C42b_FromAValues.ToString(), "sc#FFFF85E085E085E0");
            #endregion

            #region Test #3 - The Subtraction (-) Operator
            // Usage: Color = Color - Color
            // Notes: Subtracts the values of one Color object from another to create a new Color object. 
            // Uses operator syntax.
            CommonLib.LogStatus("Test #3 - The Subtraction (-) Operator");

            // Create two ScRGB Colors
           Color C43a_FromScRGB = Color.FromScRgb(1.0f, 0.7f, 0.6f, 0.8f);
           Color C43b_FromScRGB = Color.FromScRgb(0.5f, 1.0f, 0.6f, 0.1f);

            // Create two ARGB Colors
           Color C43a_FromARGB = Color.FromArgb(0xFF, 0xB2, 0x99, 0xCC);
           Color C43b_FromARGB = Color.FromArgb(0x80, 0xFF, 0x99, 0x19);

            // Create two RGB Colors
           Color C43a_FromRGB = Color.FromRgb(0xB2, 0x99, 0xCC);
           Color C43b_FromRGB = Color.FromRgb(0xFF, 0x99, 0x19);

            // Create two AValues Colors.
            floats_in = new float[] { 1.0f, 1.0f, 1.0f };
           Color C43a_FromAValues = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
            floats_in = new float[] { 0.5f, 0.5f, 0.5f };
           Color C43b_FromAValues = Color.FromAValues(0.5f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the - Operator to combine the two ScRGB Colors and create a third.
           Color C43c_FromScRGB = C43a_FromScRGB - C43b_FromScRGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush431 = new SolidColorBrush();
            brush431.Color = C43c_FromScRGB;
            DC.DrawRectangle(brush431, null, new Rect(P1 += Vx3, P2 += Vx3));

            // Use the - Operator to combine the two ARGB Colors and create a third.
           Color C43c_FromARGB = C43a_FromARGB - C43b_FromARGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush432 = new SolidColorBrush();
            brush432.Color = C43c_FromARGB;
            DC.DrawRectangle(brush432, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the - Operator to combine the two RGB Colors and create a third.
           Color C43c_FromRGB = C43a_FromRGB - C43b_FromRGB;

            // Fill a Rectangle with the Color
            SolidColorBrush brush433 = new SolidColorBrush();
            brush433.Color = C43c_FromRGB;
            DC.DrawRectangle(brush433, null, new Rect(P1 += Vx2, P2 += Vx2));

            // Use the - Operator method to combine the two AValues Colors and create a
            // third.  The Color can only be retrieved using ToString().
           Color C43c_FromAValues = C43a_FromAValues - C43b_FromAValues;

            // Color math is always done in 1.0 gamma space, so a helper function is needed
            // to verify the result using the math in color.cs.  Alpha is always in 1.0 space
            // so it does not need to use the helper function.
            _class_testresult &= _helper.CompareColorPropFloats("Subtraction (-) ScRGB", C43c_FromScRGB, (1.0f - 0.5f), (0.7f - 1.0f), (0.6f - 0.6f), (0.8f - 0.1f));
            _class_testresult &= _helper.CompareColorPropInts("Subtraction (-) ARGB", C43c_FromARGB, (0xFF - 0x80), _helper.SubtractRGBColorChannel(0xB2, 0xFF), _helper.SubtractRGBColorChannel(0x99, 0x99), _helper.SubtractRGBColorChannel(0xCC, 0x19));
            _class_testresult &= _helper.CompareColorPropInts("Subtraction (-) RGB", C43c_FromRGB, 0x00, _helper.SubtractRGBColorChannel(0xB2, 0xFF), _helper.SubtractRGBColorChannel(0x99, 0x99), _helper.SubtractRGBColorChannel(0xCC, 0x19));

            try
            {
               Color C43d_FromAValues = C43a_FromAValues - C43b_FromRGB;
            }
            catch (System.Exception e)
            {
                string message = rm.GetString("Color_ColorContextTypeMismatch");
                if (String.Compare(e.Message, message, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Pass: Substracting colors with mistmatched context returns the expected exception");
                }
                else
                {
                    CommonLib.LogStatus("Fail: Substracting colors with mistmatched context returns " + e.Message + " should be " + message);
                    _class_testresult &= false;
                }
            }

            // ******* Regression_Bug236 - ToString() doesn't work with AValues colors *******
            //class_testresult &= helper.CompareProp("Subtraction (-) AValues", C43c_FromAValues.ToString(), "sc#7FFFBD0EBD0EBD0E");
            #endregion

            #region Test #4 - The Equality (==) Operator
            // Usage: bool = (Color == Color)
            // Notes: Indicates whether the Color channel values of one Color are equal to those in another. 
            // Uses operator syntax.
            CommonLib.LogStatus("Test #4 - The Equality (==) Operator");

            // Create three Colors, two with identical values. 
           Color C44a_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C44b_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C44c_FromScRGB = Color.FromScRgb(0.99f, 1.0f, 0.4f, 0.2f);
           Color C44d_FromScRGB = Color.FromScRgb(1.0f, 0.0f, 0.4f, 0.2f);
           Color C44e_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.8f, 0.2f);
           Color C44f_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.3f);

            // Use the == Operator to compare the equality of the first color to
            // the other two colors.
            comp1 = (C44a_FromScRGB == C44b_FromScRGB);
            comp2 = (C44a_FromScRGB == C44c_FromScRGB);
            comp3 = (C44a_FromScRGB == C44d_FromScRGB);
            comp4 = (C44a_FromScRGB == C44e_FromScRGB);
            comp5 = (C44a_FromScRGB == C44f_FromScRGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equality (==) ScRGB 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equality (==) ScRGB 2", comp2, false);
            _class_testresult &= _helper.CompareProp("Equality (==) ScRGB 3", comp3, false);
            _class_testresult &= _helper.CompareProp("Equality (==) ScRGB 4", comp4, false);
            _class_testresult &= _helper.CompareProp("Equality (==) ScRGB 5", comp5, false);

            // FromArgb and FromRGB are already tested in Equals

            // Create four AValues Colors, two with the same values, 
            // one with different color values and one with a different alpha value. 
            floats_in = new float[] { 1.0f, 0.5f, 0.5f };
            floats_in_diff = new float[] { 0.9f, 0.6f, 0.4f };

           Color C44a_AValues = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C44b_AValues = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C44c_AValues = Color.FromAValues(1.0f, floats_in_diff, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C44d_AValues = Color.FromAValues(0.9f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the == Operator to compare the equality of the first color to
            // the other three colors.
            comp1 = (C44a_AValues == C44b_AValues);
            comp2 = (C44a_AValues == C44c_AValues);
            comp3 = (C44a_AValues == C44d_AValues);
            comp4 = (C44a_FromScRGB == C44a_AValues);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Equality (==) AValues 1", comp1, true);
            _class_testresult &= _helper.CompareProp("Equality (==) AValues 2", comp2, false);
            _class_testresult &= _helper.CompareProp("Equality (==) AValues 3", comp3, false);
            _class_testresult &= _helper.CompareProp("Equality ScRGBValue (==) AValues ", comp4, false);
            #endregion

            #region Test #5 - The Inequality (!=) Operator
            // Usage: bool = (Color != Color)
            // Notes: Indicates whether the Color channel values of one Color is NOT equal to those in another.
            // Uses operator syntax.
            CommonLib.LogStatus("Test #5 - The Inequality (!=) Operator");

            // Create three Colors, two with identical values. 
           Color C45a_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C45b_FromScRGB = Color.FromScRgb(1.0f, 1.0f, 0.4f, 0.2f);
           Color C45c_FromScRGB = Color.FromScRgb(0.99f, 1.0f, 0.4f, 0.2f);

            // Use the != Operator to compare the inequality of the first color to
            // the other two colors.
            comp1 = (C45a_FromScRGB != C45b_FromScRGB);
            comp2 = (C45a_FromScRGB != C45c_FromScRGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Inequality (!=) ScRGB 1", comp1, false);
            _class_testresult &= _helper.CompareProp("Inequality (!=) ScRGB 2", comp2, true);

            // Create three ARGB Colors, two with identical values. 
           Color C45a_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x22);
           Color C45b_ARGB = Color.FromArgb(0xFF, 0xFF, 0x66, 0x22);
           Color C45c_ARGB = Color.FromArgb(0xFF, 0xFF, 0x65, 0x22);

            // Use the != Operator to compare the inequality of the first color to
            // the other two colors.
            comp1 = (C45a_ARGB != C45b_ARGB);
            comp2 = (C45a_ARGB != C45c_ARGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Inequality (!=) ARGB 1", comp1, false);
            _class_testresult &= _helper.CompareProp("Inequality (!=) ARGB 2", comp2, true);

            // Create three RGB Colors, two with identical values. 
           Color C45a_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C45b_RGB = Color.FromRgb(0xFF, 0x66, 0x22);
           Color C45c_RGB = Color.FromRgb(0xFF, 0x66, 0x21);

            // Use the != Operator to compare the inequality of the first color to
            // the other two colors.
            comp1 = (C45a_RGB != C45b_RGB);
            comp2 = (C45a_RGB != C22c_RGB);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Inequality (!=) RGB 1", comp1, false);
            _class_testresult &= _helper.CompareProp("Inequality (!=) RGB 2", comp2, true);

            // Create four AValues Colors, two with the same values, 
            // one with different color values and one with a different alpha value. 
            floats_in = new float[] { 1.0f, 0.5f, 0.5f };
            floats_in_diff = new float[] { 0.9f, 0.6f, 0.4f };

           Color C45a_AValues = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C45b_AValues = Color.FromAValues(1.0f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C45c_AValues = Color.FromAValues(1.0f, floats_in_diff, GetUriFromProfileName("sRGB Color Space Profile.icm"));
           Color C45d_AValues = Color.FromAValues(0.9f, floats_in, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            // Use the != Operator to compare the inequality of the first color to
            // the other three colors.
            comp1 = (C45a_AValues != C45b_AValues);
            comp2 = (C45a_AValues != C45c_AValues);
            comp3 = (C45a_AValues != C45d_AValues);

            // Check the comparisons to assure that they are correct.
            _class_testresult &= _helper.CompareProp("Inequality (!=) AValues 1", comp1, false);
            _class_testresult &= _helper.CompareProp("Inequality (!=) AValues 2", comp2, true);
            _class_testresult &= _helper.CompareProp("Inequality (!=) AValues 3", comp3, true);
            #endregion
            #endregion End Of SECTION IV

            #region SECTION V - ROUND TRIP MATH TEST
            // Test the conversion of sRGB to ScRGB and back again using math from Color.cs
            for (byte bval = 0x00; bval < 0xFF; bval += 0x01)
            {
                byte original_val = bval;

                // Convert sRGB color channel value to ScRGB
                float ScRGB_Value = _helper.ConvertsRGBToScRGB(bval);

                // Convert ScRGB color channel value to sRGB
                byte sRGB_Value = _helper.ConvertScRGBTosRGB(ScRGB_Value);

                // Compare the roundtripped sRGB with the original sRGB value
                if (original_val != sRGB_Value)
                {
                    CommonLib.LogStatus("Fail: Round Trip Color " + original_val.ToString() + " = " + sRGB_Value.ToString());
                    _class_testresult &= false;
                }
            }
            #endregion End Of SECTION V

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogStatus("Logging the Test Result");
            if (!_class_testresult)
            {
                CommonLib.ErrorFlag = true;
            }
            CommonLib.LogResult();
            #endregion End of TEST LOGGING
        }

        private void TestGetNativeColorValues()
        {
           Color noContext = Color.FromArgb(0, 12, 34, 56);
           Color noContext2 = Color.FromRgb(90, 78, 56);
           Color noContext3 = Color.FromScRgb(1f, .5f, .5f, 1f);
            float[] nativeColorValues = new float[] { .1f, .4f, .7f };
            float[] nativeColorValues2 = new float[] { 1f, 1f, 1f };
           
           //Broken case due to missing file
           //Color withContext = Color.FromAValues(.5f, nativeColorValues, GetUriFromProfileName("kodak_dc.icm"));
           //Color withContext2 = Color.FromValues(nativeColorValues2, GetUriFromProfileName("sRGB Color Space Profile.icm"));

            float[] result = null;

            try
            {
                result = noContext.GetNativeColorValues();
                _class_testresult = false;
                CommonLib.LogStatus("FAIL - Exception should have been thrown for Color without a ColorContext");
            }
            catch (InvalidOperationException)
            {
                CommonLib.LogStatus("Correct exception thrown for Argb Color without Native Color Values");
            }
            try
            {
                result = noContext2.GetNativeColorValues();
                _class_testresult = false;
                CommonLib.LogStatus("FAIL - Exception should have been thrown for Color without a ColorContext");
            }
            catch (InvalidOperationException)
            {
                CommonLib.LogStatus("Correct exception thrown for Rgb Color without Native Color Values");
            }
            try
            {
                result = noContext3.GetNativeColorValues();
                _class_testresult = false;
                CommonLib.LogStatus("FAIL - Exception should have been thrown for Color without a ColorContext");
            }
            catch (InvalidOperationException)
            {
                CommonLib.LogStatus("Correct exception thrown for ScRgb Color without Native Color Values");
            }
            /* Broken color context refs
            result = withContext.GetNativeColorValues();
            if (!AreFloatArraysEqual(result, nativeColorValues))
            {
                class_testresult = false;
                CommonLib.LogStatus("FAIL - native color values are not correct");
                CommonLib.LogStatus("*** Expected: " + FloatArrayToString(nativeColorValues));
                CommonLib.LogStatus("*** Actual:   " + FloatArrayToString(result));
            }
            else
            {
                CommonLib.LogStatus("Color.GetNativeColorValues returned the correct answer");
            }
            result = withContext2.GetNativeColorValues();
            if (!AreFloatArraysEqual(result, nativeColorValues2))
            {
                class_testresult = false;
                CommonLib.LogStatus("FAIL - native color values are not correct");
                CommonLib.LogStatus("*** Expected: " + FloatArrayToString(nativeColorValues2));
                CommonLib.LogStatus("*** Actual:   " + FloatArrayToString(result));
            }
            else
            {
                CommonLib.LogStatus("Color.GetNativeColorValues returned the correct answer");
            }*/
        }

        private bool AreFloatArraysEqual(float[] array1, float[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int n = 0; n < array1.Length; n++)
            {
                if (array1[n] != array2[n])
                {
                    return false;
                }
            }
            return true;
        }

        private string FloatArrayToString(float[] array)
        {
            string result = string.Empty;
            foreach (float f in array)
            {
                result += f.ToString() + " ";
            }
            return result;
        }

        //--------------------------------------------------------------------

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
