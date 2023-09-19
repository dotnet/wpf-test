// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal struct functionData
    {
        public DispatcherOperationCallback functionPointer;
        public object args;
        public int timeout;

        public functionData(DispatcherOperationCallback fp)
        {
            functionPointer = fp;
            args = null;
            timeout = 1000;
        }

        public functionData(DispatcherOperationCallback fp, object a)
        {
            functionPointer = fp;
            args = a;
            timeout = 1000;
        }

        public functionData(DispatcherOperationCallback fp, object a, int t)
        {
            functionPointer = fp;
            args = a;
            timeout = t;
        }
    }

    public class XamlTestHelper
    {
        static XamlTestHelper()
        {
            s_log = new TestLog("Graphics Xaml Test");
            s_log.Result = TestResult.Pass;
        }

        private static TestLog s_log;
        public static string[] args = null;
        public static Window window = null;
        public static Dispatcher currentDispatcher = null;
        public static ArrayList methodArray = new ArrayList();
        public static bool errorFlag = false;
        private static double s_masterDpiX = 96.0;
        private static double s_masterDpiY = 96.0;
        private static bool s_ignoreDPI = false;

        private static DateTime s_startTime = DateTime.Now;

        public static void LogFail(string failMessage)
        {
            s_log.LogStatus(failMessage);
            s_log.Result = TestResult.Fail;
        }

        public static void LogStatus(string message)
        {
            s_log.LogStatus(message);
        }

        //this variable holding the capture make by TakeSnapshot method
        //user can get the snapshot in memory by querying this variable after
        //the TakeSnapshot is called.
        public static System.Drawing.Bitmap captureBMP = null;

        public static void AddStep(DispatcherOperationCallback fp)
        {
            methodArray.Add(new functionData(fp, null));
        }

        public static void AddStep(DispatcherOperationCallback fp, object args)
        {
            methodArray.Add(new functionData(fp, args));
        }

        public static void AddStep(DispatcherOperationCallback fp, object args, int timeout)
        {
            methodArray.Add(new functionData(fp, args, timeout));
        }

        /// <summary>
        /// Start running the scheduled task(s);
        /// </summary>
        public static void Run()
        {
            if (methodArray.Count == 0)
            {
                s_log.LogStatus("Error: No method in the Queue");
                s_log.LogStatus("Please use AddStep to add a method");
                Quit(null);
            }

            int currentTimeOut = 0;

            foreach (functionData f in methodArray)
            {
                if (f.args == null)
                {
                    PostNextStep(f.functionPointer, currentTimeOut);
                }
                else
                {
                    PostNextStep(f.functionPointer, f.args, currentTimeOut);
                }
                currentTimeOut += f.timeout;

            }

        }

        #region VisualUIScan helper methods
        public static object TakeSnapshot(object arg)
        {
            s_log.LogStatus("Taking snap shot");
            WindowInteropHelper wih = new WindowInteropHelper(window);
            captureBMP = ImageUtility.CaptureScreen(wih.Handle, true);

            if (null != arg && arg is string)
            {
                //Save the snapshot to image
                s_log.LogStatus("Save the snapshot to image, " + (string)arg);
                captureBMP.Save((string)arg);
            }
            return null;
        }

        /// <summary>
        /// Compare the captured bitmap vs. master
        /// </summary>
        /// <param name="master">file path to master bitmap</param>
        /// <returns>true if they are the same, false otherwise</returns>
        public static bool Compare(string master)
        {
            return Compare(master, captureBMP, false);
        }

        /// <summary>
        /// Compare the captured bitmap vs. master
        /// </summary>
        /// <param name="master">file path to master bitmap</param>
        /// <returns>true if they are the same, false otherwise</returns>
        public static bool CompareWithoutDPI(string master)
        {
            bool currentIgnoreDPI = s_ignoreDPI;//save the old setting
            s_ignoreDPI=true; //rather than send this through API and change everything...
            bool result = Compare(master, captureBMP, false);
            s_ignoreDPI=currentIgnoreDPI;//restore the old setting
            return result;
        }
        
        /// <summary>
        /// Compare the captured bitmap vs. master bitmap
        /// </summary>
        /// <param name="master">file path to master bitmap</param>
        /// <param name="expectFailure">Expecting the comparison failure or pass</param>
        /// <returns>true if the comparison result matchs to the expectation (true/false), false otherwise</returns>
        public static bool Compare(string master, bool expectFailure)
        {
            return Compare(master, captureBMP, expectFailure);
        }

        /// <summary>
        /// Compare the captured bitmap vs. master 
        /// </summary>
        /// <param name="master">File path to master bitmap</param>
        /// <param name="capture">captured bitmap</param>
        /// <param name="expectFailure">Expectation on the comparison</param>
        /// <returns>true if the comparison result matchs to the expectation (true/false), false otherwise</returns>
        public static bool Compare(string master, System.Drawing.Bitmap capture, bool expectFailure)
        {
            ImageAdapter imgCapture = new ImageAdapter(capture);

            if (!File.Exists(master))
            {
                Pack(null, imgCapture);
                return false;
            }

            ImageAdapter imgMaster = new ImageAdapter(master);

            return Compare(imgMaster, imgCapture, expectFailure);
        }

        /// <summary>
        /// Compare the captured bitmap vs. master 
        /// </summary>
        /// <param name="master">ImageAdapter of the master bitmap</param>
        /// <param name="capture">ImageAdapter of the captured bitmap</param>
        /// <param name="expectFailure">Expectation on the comparison</param>
        /// <returns>true if the comparison result matchs to the expectation (true/false), false otherwise</returns>
        public static bool Compare(ImageAdapter imgMaster, ImageAdapter imgCapture, bool expectFailure)
        {
            ImageComparator hcic = new ImageComparator();
            imgMaster.DpiX = s_masterDpiX;
            imgMaster.DpiY = s_masterDpiY;

            if (null == hcic)
            {
                throw new System.ApplicationException("Failed to instantiate VScan");
            }

            SetupToleranceControlPointsGood(hcic);

            System.Drawing.Point masterDPI = new System.Drawing.Point((int)Math.Round(imgMaster.DpiX), (int)Math.Round(imgMaster.DpiY));
            System.Drawing.Point captureDPI = new System.Drawing.Point((int)Math.Round(imgCapture.DpiX), (int)Math.Round(imgCapture.DpiY));

            bool match = false;
            //ignore the DPI differences if requested
            if(s_ignoreDPI)
            {
                match = hcic.Compare(imgMaster,imgCapture,false);
            }
            else
            {
                match = hcic.Compare(imgMaster,
                                      masterDPI,  // master image dpi info
                                      imgCapture,
                                      captureDPI,  // test image dpi info
                                      false);
            }
            
            if (expectFailure)
            {
                if (match)
                {
                    s_log.LogStatus("Error:  The capture & Master are matching while the expectation is not!");
                    Pack(imgMaster, imgCapture);
                    return false;
                }
            }
            else
            {
                if (!match)
                {
                    s_log.LogStatus("Error: The capture & Master don't match!");
                    Pack(imgMaster, imgCapture);
                    return false;
                }
            }
            return true;
        }

        private static void SetupToleranceControlPointsGood(ImageComparator hcic)
        {
            // setup tolerance control points 
            // This set of tolerance control points is equivalent to the "Good" level in GTO
            hcic.Curve.CurveTolerance.Entries.Clear();
            hcic.Curve.CurveTolerance.Entries.Add(0, 1.0);
            hcic.Curve.CurveTolerance.Entries.Add(2, 0.0005);
            hcic.Curve.CurveTolerance.Entries.Add(10, 0.0004);
            hcic.Curve.CurveTolerance.Entries.Add(15, 0.0003);
            hcic.Curve.CurveTolerance.Entries.Add(25, 0.0002);
            hcic.Curve.CurveTolerance.Entries.Add(35, 0.0001);
            hcic.Curve.CurveTolerance.Entries.Add(45, 0.00001);
            hcic.FilterLevel = 0;
            hcic.ChannelsInUse = ChannelCompareMode.ARGB;
        }
        /// <summary>
        /// Package the master and captured bitmap into a vscan file for failure dignostic
        /// </summary>
        /// <param name="imgMaster">master bitmap</param>
        /// <param name="imgCapture">captured bitmap</param>
        private static void Pack(ImageAdapter imgMaster, ImageAdapter imgCapture)
        {
            string packageName = Path.Combine(Path.GetTempPath(), XamlTestHelper.GetArgument("Test", args) + ".vscan");
            
            System.Drawing.Bitmap captureBMP = ImageUtility.ToBitmap(imgCapture);
            Package package = null;

            if (imgMaster == null)
            {
                s_log.LogStatus("Master is not found");
                s_log.LogStatus("Only packing Capture into the vscan file");
                package = Package.Create(packageName, null, captureBMP);
            }
            else
            {
                System.Drawing.Bitmap masterBMP = ImageUtility.ToBitmap(imgMaster);
                s_log.LogStatus("Master and Capture bitmaps are save in a VSCan file, " + packageName);
                package = Package.Create(packageName, masterBMP, captureBMP);
            }

            s_log.LogStatus("Generating VScan file: :  " + packageName);
            package.Save();
            s_log.LogFile(packageName);
        }
        #endregion

        #region Window action methods
        /// <summary>
        /// Shutdown the window
        /// It is the end of the test
        /// </summary>
        public static object Quit(object arg)
        {
            s_log.LogStatus("Closing down");
            s_log.LogStatus("End of the test");
            s_log.Close();
            window.Close();
            return null;
        }
        #endregion

        #region GetArgument method
        public static string GetArgument(string name, string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("/"))
                {
                    int pos = arg.IndexOf(":");

                    if (pos > 0)
                    {
                        string argName = arg.Substring(1, pos - 1);

                        if (argName.ToLower() == name.ToLower())
                            return arg.Substring(pos + 1).Trim();
                    }
                    else if ("/" + name == arg)
                        return "true";
                }
                else if (name == null)
                    return arg;
            }

            return null;
        }
        #endregion

        #region PostMethods
        /// <summary>
        /// Used to post a method to the context of your ApplicationWindow after a certain amount of time has passed
        /// </summary>
        /// <param name="postMethod">Method to post</param>
        /// <param name="TimerInterval">int time in milliseconds</param>
        public static void PostNextStep(DispatcherOperationCallback postMethod, int timerInterval)
        {
            System.Object args = new System.Object();
            PostNextStep(postMethod, args, timerInterval);
        }

        public static void PostNextStep(DispatcherOperationCallback postMethod, object args, int timerInterval)
        {
            DispatcherTimer dTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            dTimer.Interval = new System.TimeSpan(0, 0, 0, 0, timerInterval);
            dTimer.Tick += new EventHandler(timerHelper);
            dTimer.Tag = new object[] { postMethod, args };
            dTimer.Start();
            s_startTime = DateTime.Now;
        }

        static void timerHelper(object o, EventArgs args)
        {
            DispatcherTimer dTimer = (DispatcherTimer)o;
            if (((TimeSpan)dTimer.Interval).TotalMilliseconds - 100 > (((TimeSpan)(DateTime.Now - s_startTime)).TotalMilliseconds))
            {
                return;
            }
            else
            {
                dTimer.Stop();
            }

            object[] objs = (object[])dTimer.Tag;
            DispatcherOperationCallback callback = (DispatcherOperationCallback)objs[0];
            callback(objs[1]);
        }

        #endregion

        #region BuildColorStringARGB - Color
        // BuildColorString(Color color)
        // A protected helper function that returns a string representation of the 
        // ARGB values in a Color object.
        // Parameters: 
        //      Color color - The color to parse.
        //
        // Returns:
        //      string
        //  
        public static string BuildColorStringARGB(Color color)
        {
            return (("(alpha=" + color.A + " red=" + color.R + " green=" + color.G + " blue=" + color.B + ")"));
        }
        #endregion End BuildColorStringARGB - Color

        #region BuildColorStringScArgb - Color
        // BuildColorString(Color color)
        // A protected helper function that returns a string representation of the 
        // ScArgb values in a Color object.
        // Parameters: 
        //      Color color - The color to parse.
        //
        // Returns:
        //      string
        //  
        public static string BuildColorStringScArgb(Color color)
        {
            return (("(alpha=" + color.ScA + " red=" + color.ScR + " green=" + color.ScG + " blue=" + color.ScB + ")"));
        }
        #endregion End BuildColorStringScArgb - Color

        #region BuildColorStringFloats
        // BuildColorStringFloats(float a, float r, float g, float b)
        // A protected helper function that returns a string representation of the 
        // values in a Color object.
        // Parameters: 
        //      float a, float r, float g, float b - The values of the four color
        //      channels.
        //
        // Returns:
        //      string
        //  
        public static string BuildColorStringFloats(float a, float r, float g, float b)
        {
            return (("(alpha=" + a + " red=" + r + " green=" + g + " blue=" + b + ")"));
        }
        #endregion

        #region BuildColorStringInts
        // BuildColorStringInts(int a, int r, int g, int b)
        // A protected helper function that returns a string representation of the 
        // values in a Color object.
        // Parameters: 
        //      int a, int r, int g, int b - The values of the four color
        //      channels.
        //
        // Returns:
        //      string
        //  
        public static string BuildColorStringInts(int a, int r, int g, int b)
        {
            return (("(alpha=" + a + " red=" + r + " green=" + g + " blue=" + b + ")"));
        }
        #endregion BuildColorStringInts

        #region ConvertsRGBToScRGB
        // ConvertsRGBToScRGB(byte bval)
        // A protected helper function that converts a byte Color value (sRGB) to it's 
        // equivalent float scRGB value using the math in Color.cs. 
        // Parameters: 
        //      byte bval - The sRGB Color value to convert
        //
        // Returns:
        //      float
        //  
        public static float ConvertsRGBToScRGB(byte bval)
        {
            float fval = ((float)bval / 255.0f);

            if (fval <= 0.0)
                return (0.0f);
            else if (fval < 0.081)
                return (fval / 4.5f);
            else if (fval < 1.0f)
                return ((float)Math.Pow(((double)fval + 0.099) / 1.099, 1.0 / 0.45));
            else
                return (1.0f);
        }
        #endregion ConvertsRGBToScRGB

        #region ConvertScRGBTosRGB
        // ConvertScRGBTosRGB(float fval)
        // A protected helper function that converts a float Color value (ScRGB) to it's 
        // equivalent byte sRGB value using the math in Color.cs. 
        // Parameters: 
        //      float fval - The ScRGB Color value to convert/
        //
        // Returns:
        //      byte
        //  
        public static byte ConvertScRGBTosRGB(float fval)
        {
            if (fval <= 0.0)
                return (0);
            else if (fval < 0.018)
                return ((byte)((255.0f * fval * 4.5f) + 0.5f));
            else if (fval < 1.0)
                return ((byte)((255.0f * ((1.099f * (float)Math.Pow((double)fval, 0.45)) - 0.099f)) + 0.5f));
            else
                return (255);
        }
        #endregion ConvertScRGBTosRGB

        #region MultiplyRGBColorChannel
        // MultiplyRGBColorChannel(byte color_channel_value, float multiplier)
        // A helper function that multiplies an RGB color channel value by
        // a float multiplier as it is done in Color.cs.  This is used for verirfying
        // Color math operations.
        // Parameters: 
        //      byte color_channel_value - The color channel value to multiply
        //      float multiplier         - The value to multiply with.
        //
        // Returns:
        //      byte 
        //  
        public static byte MultiplyRGBColorChannel(byte color_channel_value, float multiplier)
        {
            // Convert sRGB color channel value to ScRGB
            float ScRGB_Value = ConvertsRGBToScRGB(color_channel_value);

            // Do Multiplication
            ScRGB_Value *= multiplier;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion MultiplyRGBColorChannel

        #region AddRGBColorChannel
        // AddRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        // A helper function that adds two RGB color channel values together as it
        // is done in Color.cs.  This is used for verirfying Color math operations.
        // Parameters: 
        //      byte color_channel_value1 - One color channel value to add
        //      byte color_channel_value2 - The other color channel value to add
        //
        // Returns:
        //      byte 
        //  
        public static byte AddRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        {
            // Convert both sRGB color channel values to ScRGB
            float ScRGB_Value1 = ConvertsRGBToScRGB(color_channel_value1);
            float ScRGB_Value2 = ConvertsRGBToScRGB(color_channel_value2);

            // Do Addition
            float ScRGB_Value = ScRGB_Value1 + ScRGB_Value2;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion AddRGBColorChannel

        #region SubtractRGBColorChannel
        // SubtractRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        // A helper function that subtracts one RGB color channel value from another as it
        // is done in Color.cs.  This is used for verirfying Color math operations.
        // Parameters: 
        //      byte color_channel_value1 - One color channel value
        //      byte color_channel_value2 - The other color channel value to subtract
        //
        // Returns:
        //      byte 
        //  
        public static byte SubtractRGBColorChannel(byte color_channel_value1, byte color_channel_value2)
        {
            // Convert both sRGB color channel values to ScRGB
            float ScRGB_Value1 = ConvertsRGBToScRGB(color_channel_value1);
            float ScRGB_Value2 = ConvertsRGBToScRGB(color_channel_value2);

            // Do Subtracton
            float ScRGB_Value = ScRGB_Value1 - ScRGB_Value2;

            // Convert ScRGB color channel value to sRGB
            byte sRGB_Value = ConvertScRGBTosRGB(ScRGB_Value);

            // Return the new value.
            return (sRGB_Value);
        }
        #endregion SubtractRGBColorChannel

        #region BuildMatrixString - Six doubles
        // BuildMatrixString(double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        // A protected helper function that returns a string representation of the 
        // values in a Matrix.
        // Parameters: 
        //      double M11, double M12, double M21, double M22, double MOfX, double MOfY
        //      - The values of the Matrix
        //
        // Returns:
        //      string
        //  
        public static string BuildMatrixString(double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            return ("(" + M11 + ", " + M12 + ", " + M21 + ", " + M22 + ", " + MOfX + ", " + MOfY + ")");
        }
        #endregion BuildMatrixString - Six doubles

        #region CheckType
        // bool CheckType(object obj, string object_type)
        // A protected helper function to display a status message based on the
        // expected type of an Object and return a pass/fail result.
        // Parameters: 
        //      object obj - The object to check.
        //      string object_type - The type of object that is expected.
        //
        // Returns:
        //      bool (Pass = true, Fail = false)
        //
        public static bool CheckType(object obj, string object_type)
        {
            // Confirm that the expected object type was created successfully.
            if (obj.GetType().ToString() == object_type)
            {
                s_log.LogStatus("Pass: " + object_type + " was created successfully");
            }
            else
            {
                s_log.LogStatus("Fail: " + object_type + " was NOT created a " + obj.GetType().ToString() + " was created.");
                return false;
            }

            return true;
        }
        #endregion End CheckType

        #region CompareProp - Two strings
        // bool CompareProp(string prop, string propvalue, string expected)
        // A protected helper function to compare two string values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      string propvalue - The value of the property.
        //      string expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, string propvalue, string expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue);
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue + " should be " + expected);
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles

        #region CompareProp - Two doubles
        // bool CompareProp(string prop, double propvalue, double expected)
        // A protected helper function to compare two double values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      double propvalue - The value of the property.
        //      double expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, double propvalue, double expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture) + " should be " + expected.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles

        #region CompareProp - Two doubles with tolerance
        // bool CompareProp(string prop, double propvalue, double expected, double tolerance)
        // A protected helper function to compare two double values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      double propvalue - The value of the property.
        //      double expected - The expected value.
        //      double tolerance
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, double propvalue, double expected, double tolerance)
        {
            if (Math.Abs(propvalue - expected) < tolerance)
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " expected=" + expected.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " tolerance=" + tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " expected=" + expected.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + " tolerance=" + tolerance.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two doubles with tolerance

        #region CompareProp - Two ints
        // bool CompareProp(string prop, int propvalue, int expected)
        // A protected helper function to compare two int values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      int propvalue - The value of the property.
        //      int expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, int propvalue, int expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two ints

        #region CompareProp - Two bools
        // bool CompareProp(string prop, bool propvalue, bool expected)
        // A protected helper function to compare two bool values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      bool propvalue - The value of the property.
        //      bool expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, bool propvalue, bool expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two bools

        #region CompareProp - Two Rects
        // bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        // A protected helper function to compare two Rect values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Rect propvalue - The value of the property.
        //      Rect expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        {
            if (System.Windows.Rect.Equals(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Rects

        #region CompareProp - Two Sizes
        // bool CompareProp(string prop, System.Windows.Rect propvalue, System.Windows.Rect expected)
        // A protected helper function to compare two Rect values, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Rect propvalue - The value of the property.
        //      Rect expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, System.Windows.Size propvalue, System.Windows.Size expected)
        {
            if (System.Windows.Size.Equals(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + "=" + propvalue.ToString());
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + propvalue.ToString() + " should be " + expected.ToString());
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Sizes

        #region CompareProp - Two GradientStops
        // bool CompareProp(string prop, GradientStop propvalue, GradientStop expected)
        // A protected helper function to compare two GradientStops, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      GradientStop propvalue - The value of the property.
        //      GradientStop expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, GradientStop propvalue, GradientStop expected)
        {
            if ((propvalue.Color == expected.Color) && (propvalue.Offset == expected.Offset))
            {
                s_log.LogStatus("Pass: " + prop + " = GradientStop has the expected values");
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " = GradientStop does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two GradientStops

        #region CompareProp - Two Float Arrays
        // bool CompareProp(string prop, float[] propvalue, float[] expected)
        // A protected helper function to compare two Float Arrays, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      float[] propvalue - The value of the property.
        //      float[] expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, float[] propvalue, float[] expected)
        {
            if (propvalue.Length != expected.Length)
            {
                s_log.LogStatus("Fail: " + prop + " = Float Array contains " + propvalue.Length + " values should have " + expected.Length + " values");
                return false;
            }

            if (CompareFloatArrays(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + " = Float Array has the expected values");
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " = Float Array does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Float Arrays

        #region CompareFloatArrays
        // bool CompareFloatArrays(float[] F1, float[] F2)
        // A protected helper function to compare two DoubleCollections, and return a pass/fail result. 
        // Parameters: 
        //      float[] F1 - The value to compare.
        //      float[] F2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareFloatArrays(float[] F1, float[] F2)
        {
            if (F1.Length != F2.Length)
            {
                return false;
            }

            for (int i = 0; i < F1.Length; i++)
            {
                if (F1[i] != F2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two DoubleCollections
        // bool CompareProp(string prop, DoubleCollection propvalue, DoubleCollection expected)
        // A protected helper function to compare two DoubleCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      DoubleCollection propvalue - The value of the property.
        //      DoubleCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, DoubleCollection propvalue, DoubleCollection expected)
        {
            if (propvalue.Count != expected.Count)
            {
                s_log.LogStatus("Fail: " + prop + " = DoubleCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareDoubleCollections(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + " = DoubleCollection has the expected values");
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " = DoubleCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two DoubleCollections

        #region CompareDoubleCollections
        // bool CompareDoubleCollections(DoubleCollection D1, DoubleCollection D2)
        // A protected helper function to compare two DoubleCollections, and return a pass/fail result. 
        // Parameters: 
        //      DoubleCollection D1 - The value to compare.
        //      DoubleCollection D2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareDoubleCollections(DoubleCollection D1, DoubleCollection D2)
        {
            if (D1.Count != D2.Count)
            {
                return false;
            }

            for (int i = 0; i < D1.Count; i++)
            {
                if (D1[i] != D2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two ColorCollections
        // bool CompareProp(string prop, IList<Color> propvalue, IList<Color> expected)
        // A protected helper function to compare two ColorCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      ColorCollection propvalue - The value of the property.
        //      ColorCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, IList<Color> propvalue, IList<Color> expected)
        {
            if (propvalue.Count != expected.Count)
            {
                s_log.LogStatus("Fail: " + prop + " = ColorCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareColorCollections(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + " = ColorCollection has the expected values");
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " = ColorCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two ColorCollections

        #region CompareColorCollections
        // bool CompareColorCollections(IList<Color> C1, IList<Color> C2)
        // A protected helper function to compare two ColorCollections, and return a pass/fail result. 
        // Parameters: 
        //      ColorCollection C1 - The value to compare.
        //      ColorCollection C2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareColorCollections(IList<Color> C1, IList<Color> C2)
        {
            if (C1.Count != C2.Count)
            {
                return false;
            }

            for (int i = 0; i < C1.Count; i++)
            {
                if (C1[i] != C2[i])
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections

        #region CompareProp - Two GradientStopCollections
        // bool CompareProp(string prop, GradientStopCollection propvalue, GradientStopCollection expected)
        // A protected helper function to compare two GradientStopCollections, output the result
        // to the Automation log and return a pass/fail result. 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      GradientStopCollection propvalue - The value of the property.
        //      GradientStopCollection expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, GradientStopCollection propvalue, GradientStopCollection expected)
        {
            if (propvalue.Count != expected.Count)
            {
                s_log.LogStatus("Fail: " + prop + " = GradientStopCollection contains " + propvalue.Count + " values should have " + expected.Count + " values");
                return false;
            }

            if (CompareGradientStopCollections(propvalue, expected))
            {
                s_log.LogStatus("Pass: " + prop + " = GradientStopCollection has the expected values");
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " = GradientStopCollection does not have the expected values");
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two GradientStopCollections

        #region CompareProp - Two Colors
        // bool CompareProp(string prop, Color propvalue, Color expected)
        // A protected helper function to compare two Colors, using the Equals
        // operator in the Avalon Color object. Returns a pass/fail result and
        // logs a message.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color propvalue - The value of the property.
        //      Color expected - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareProp(string prop, Color propvalue, Color expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildColorStringScArgb(propvalue));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildColorStringScArgb(propvalue) + " should be " + BuildColorStringScArgb(expected));
                return false;
            }

            return true;
        }
        #endregion CompareProp - Two Colors

        #region CompareGradientStopCollections
        // bool CompareGradientStopCollections(GradientStopCollection gs1, GradientStopCollection gs2)
        // A protected helper function to compare two GradientStopCollections, and return a pass/fail result. 
        // Parameters: 
        //      GradientStopCollection gs1 - The value to compare.
        //      GradientStopCollection gs2 - The expected value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareGradientStopCollections(GradientStopCollection gs1, GradientStopCollection gs2)
        {
            if (gs1.Count != gs2.Count)
            {
                return false;
            }

            for (int i = 0; i < gs1.Count; i++)
            {
                if ((gs1[i].Color != gs2[i].Color) || (gs1[i].Offset != gs2[i].Offset))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion CompareGradientStopCollections


        #region CompareColorPropFloats
        // bool CompareColorPropFloats(string prop, Color color, float expected_ScA, float expected_ScR,
        //                  float expected_ScG, float expected_ScB)
        // A protected helper function to compare a Color Property against four float
        // values, output the result to the Automation log using the ShowColorStatus
        // Method and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color color - Color to compare.
        //      float expected_ScA - The expected Alpha value.
        //      float expected_ScR - The expected Red value.
        //      float expected_ScG - The expected Green value.
        //      float expected_ScB - The expected Blue value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareColorPropFloats(string prop, Color color, float expected_ScA, float expected_ScR, float expected_ScG, float expected_ScB)
        {
            if ((color.ScA == expected_ScA) && (color.ScR == expected_ScR) && (color.ScG == expected_ScG) && (color.ScB == expected_ScB))
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildColorStringScArgb(color));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildColorStringScArgb(color) + " should be " + BuildColorStringFloats(expected_ScA, expected_ScR, expected_ScG, expected_ScB));
                return false;
            }

            return true;
        }
        #endregion CompareColorPropFloats

        #region CompareColorPropInts
        // bool CompareColorPropInts(string prop, Color color, int expected_A, int expected_R,
        //                  int expected_G, int expected_B)
        // A protected helper function to compare a Color Property against four int
        // values, output the result to the Automation log using the ShowColorStatus
        // Method and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color color - Color to compare.
        //      int expected_A - The expected Alpha value.
        //      int expected_R - The expected Red value.
        //      int expected_G - The expected Green value.
        //      int expected_B - The expected Blue value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareColorPropInts(string prop, Color color, int expected_A, int expected_R, int expected_G, int expected_B)
        {
            if ((color.A == expected_A) && (color.R == expected_R) && (color.G == expected_G) && (color.B == expected_B))
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildColorStringARGB(color));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildColorStringARGB(color) + " should be " + BuildColorStringInts(expected_A, expected_R, expected_G, expected_B));
                return false;
            }

            return true;
        }
        #endregion CompareColorPropInts

        #region CompareColorPropUint
        // bool CompareColorPropUint(string prop, Color color, uint expected)
        // A helper function to compare a Color Property against one uint value,
        // output the result to the Automation log and return a pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color color - Color to compare.
        //      Int64 expected - The expected color value.
        // Returns:
        //      bool (Pass = true, Fail = false)
        // 
        public static bool CompareColorPropUint(string prop, Color color, Int64 expected)
        {
            bool pass = true;
            if ((color.A == (byte)((expected & 0xff000000) >> 24)) && (color.R == (byte)((expected & 0x00ff0000) >> 16)) && (color.G == (byte)((expected & 0x0000ff00) >> 8)) && (color.B == (byte)(expected & 0x000000ff)))
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildColorStringARGB(color));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildColorStringARGB(color) + " should be " + expected.ToString());
                pass = false;
            }
            return pass;
        }
        #endregion CompareColorPropUint

        #region CompareMatrix
        // bool CompareMatrix(string prop, Transform trans, double M11, double M12,
        //               double M21, double M22, double MOfX, double MOfY)
        // A protected helper function to compare the Matrix value in a Transform with
        // Six Matrix values, output the result to the Automation logand return a
        // pass/fail result.
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color color - Color to compare.
        //      Transform trans - The Transform to compare.
        //      double M11, M12, M21, M22, MOfX, MOfY - The expected Matrix values.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareMatrix(string prop, Transform trans, double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            if ((trans.Value.M11 == M11) && (trans.Value.M12 == M12) && (trans.Value.M21 == M21) && (trans.Value.M22 == M22) && (trans.Value.OffsetX == MOfX) && (trans.Value.OffsetY == MOfY))
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildMatrixString(trans.Value.M11, trans.Value.M12, trans.Value.M21, trans.Value.M22, trans.Value.OffsetX, trans.Value.OffsetY));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildMatrixString(trans.Value.M11, trans.Value.M12, trans.Value.M21, trans.Value.M22, trans.Value.OffsetX, trans.Value.OffsetY) + " should be " + BuildMatrixString(M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }

            return true;
        }
        // bool CompareMatrix(Matrix matrix, double M11, double M12,
        //               double M21, double M22, double MOfX, double MOfY)
        // A protected helper function to compare the Matrix value in a Transform with
        // Six Matrix values, output the result to the Automation logand return a
        // pass/fail result.
        // This function uses a tolerance since there could be slightly differences 
        // Parameters: 
        //      string prop - Name of the Property to compare. 
        //      Color color - Color to compare.
        //      Matrix - The Matrix to compare.
        //      double M11, M12, M21, M22, MOfX, MOfY - The expected Matrix values.
        // Returns:
        //      bool (Pass = true, Fail = false)
        //  
        public static bool CompareMatrix(string prop, Matrix matrix, double M11, double M12, double M21, double M22, double MOfX, double MOfY)
        {
            double tolerance = 0.00001;
            if ((Math.Abs(matrix.M11 - M11) < tolerance) && (Math.Abs(matrix.M12 - M12) < tolerance) && (Math.Abs(matrix.M21 - M21) < tolerance) && (Math.Abs(matrix.M22 - M22) < tolerance) && (Math.Abs(matrix.OffsetX - MOfX) < tolerance) && (Math.Abs(matrix.OffsetY - MOfY) < tolerance))
            {
                s_log.LogStatus("Pass: " + prop + "=" + BuildMatrixString(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY));
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + "=" + BuildMatrixString(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY) + " should be " + BuildMatrixString(M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }
            /*
            if ((matrix.M11 == M11) && (matrix.M12 == M12) && (matrix.M21 == M21) && (matrix.M22 == M22) && (matrix.OffsetX == MOfX) && (matrix.OffsetY == MOfY))
            {
                log.LogStatus ("Pass: " + prop + "=" + BuildMatrixString (matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY));
            }
            else
            {
                log.LogStatus ("Fail: " + prop + "=" + BuildMatrixString (matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY) + " should be " + BuildMatrixString (M11, M12, M21, M22, MOfX, MOfY) + ")");
                return false;
            }
            */

            return true;
        }
        #endregion CompareMatrix

        #region ComparePoint
        // ComparePoint(string prop, Point point1, Point point2)
        public static bool ComparePoint(string prop, Point point1, Point point2)
        {
            if ((point1.X == point2.X) && (point1.Y == point2.Y))
            {
                s_log.LogStatus("Pass: " + prop + "=(" + point1.X + "," + point1.Y + ")");
                return true;
            }
            else
            {
                s_log.LogStatus("Fail: " + prop + " should equal (" + point2.X + "," + point2.Y + ") not (" + point1.X + "," + point1.Y + ")");
                return false;
            }
        }
        #endregion

        #region CompareResults
        public static bool CompareResults(string testName, bool propvalue, bool expected)
        {
            if (propvalue == expected)
            {
                s_log.LogStatus("Pass: " + testName + " returned " + propvalue.ToString());
                return true;
            }
            else
            {
                s_log.LogStatus("Fail: " + testName + " returned " + propvalue.ToString() + " instead of " + expected.ToString());
                return false;
            }
        }
        public static bool CompareResults(string testName, string propvalue, string expected)
        {
            if (string.Compare(propvalue, expected, false, System.Globalization.CultureInfo.CurrentCulture) == 0)
            {
                s_log.LogStatus("Pass: " + testName + " returned " + propvalue);
                return true;
            }
            else
            {
                s_log.LogStatus("Fail: " + testName + " returned " + propvalue + " instead of " + expected);
                return false;
            }
        }
        #endregion

        #region CompareVector

        #region CompareVector with Tolerance explicitly specified
        public static bool CompareVector(string testName, Vector value, Vector expected, double tolerance)
        {
            if ((Math.Abs(value.X - expected.X) < tolerance) && (Math.Abs(value.Y - expected.Y) < tolerance))
            {
                s_log.LogStatus("Pass: " + testName + " vector.X=" + value.X + " vector.Y=" + value.Y);
                return true;
            }
            else
            {
                s_log.LogStatus("Fail: " + testName + " vector.X=" + value.X + " should=" + expected.X + ",vector.Y=" + value.Y + " should=" + expected.Y);
                return false;
            }
        }
        #endregion


        #region CompareVector without Tolerance explicitly specified, default 0.000001
        public static bool CompareVector(string testName, Vector value, Vector expected)
        {
            // default tolerance 0.000001
            return CompareVector(testName, value, expected, 0.000001);
        }
        #endregion

        #endregion
    }

}
