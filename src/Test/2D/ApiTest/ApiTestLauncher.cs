// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.IO;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class ApiTestLauncher
    {
        /// <summary/>
        public static void Launch()
        {
            char[] tokens ={ ' ' };
            Main(DriverState.DriverParameters["Args"].Split(tokens));
        }

        #region Main

        /// <summary/>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                s_dispatcher = Dispatcher.CurrentDispatcher;
                if (ReadCommandLine(args))
                {
                    if (s_isXamlTest)
                    {
                        LoadAndRunXamlTest();
                    }
                    else
                    {
                        // Create the Visual which is the MIL API Test
                        s_dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(CreateVisuals), null);

                        // If VisualVerification is requested then invoke the PerformVisualVerification delegate.
                        if (s_useVisualVerification)
                        {
                            s_dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(PerformVisualVerification), null);
                        }

                        // A Manual Test (isAutoTest == False) does not invoke Quit, and must have the window closed
                        // manually.
                        if (s_isAutoTest)
                        {
                            s_dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(Quit), null);
                        }

                        Dispatcher.Run();
                    }
                }
                else
                {
                    CommonLib.Log.LogStatus("Invalid or incomplete command line arguments");
                    CommonLib.Log.Result = TestResult.Fail;
                }
            }
            catch (Exception e)
            {
                string error = "An unhandled exception occurred while running APITestLoader:\n\n" + e.ToString();

                if (CommonLib.Log != null)
                {
                    CommonLib.Log.LogStatus(error);
                    CommonLib.Log.Result = TestResult.Fail;
                }
                else
                {
                    Console.WriteLine(error);
                }
            }
            finally
            {
                CommonLib.CloseLog();
            }
        }
        #endregion Main

        //------------------------------------------------------
        #region Private Methods

        private static void LoadAndRunXamlTest()
        {           
            object TestObject = Application.LoadComponent(new Uri(s_testName, UriKind.RelativeOrAbsolute));
            if (TestObject == null)
            {
                CommonLib.Log.LogEvidence("Test could not be loaded");
                LogTestResult(false);
                
                return;
            }

            if (TestObject is Application)
            {
                TestObject.GetType().GetEvent("DispatcherUnhandledException").AddEventHandler(TestObject, new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException));
                TestObject.GetType().GetMethod("InitializeComponent").Invoke(TestObject, null);
                TestObject.GetType().GetMethod("Run", new Type[0]).Invoke(TestObject, null);
            }
            else
            {
                Application app = new Application();
                app.StartupUri = new Uri(s_testName, UriKind.RelativeOrAbsolute);
                app.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
                app.Run();
            }

        }

        static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CommonLib.Log.LogEvidence("DispatcherUnhandledException Caught!\n" + e.Exception.ToString());
            LogTestResult(false);
        }

        private static object CreateVisuals(object o)
        {
            System.Windows.Interop.HwndSourceParameters parameters;

            s_dpiRatio = Microsoft.Test.Display.Monitor.Dpi.x / (double)s_defaultDpi;
            GlobalLog.LogStatus("DpiRatio=" + s_dpiRatio.ToString());
            s_formwidth *= s_dpiRatio;
            s_formheight *= s_dpiRatio;

            if (s_isAutoTest)
            {
                // Frameless Window:  0x90000000
                parameters = new HwndSourceParameters(("MIL API Test:  " + s_testName), (int)s_formwidth, (int)s_formheight);
                parameters.WindowStyle = unchecked((int)0x90000000);
                parameters.SetPosition(s_formX, s_formY);
                s_source = new HwndSource(parameters);
            }
            else
            {
                // Framed Window with an "X" (Close) button:  0x10CF0000
                // Add to the formwidth and formheight so that the client area of the window
                // is the same as a frameless window.
                parameters = new HwndSourceParameters(("MIL API Test:  " + s_testName), ((int)s_formwidth + 8), ((int)s_formheight + 28));
                parameters.WindowStyle = 0x10CF0000;
                parameters.SetPosition(s_formX, s_formY);
                s_source = new HwndSource(parameters);
                s_source.AddHook(new HwndSourceHook(CloseWindowManually));
            }

            object v = null;

            try
            {
                bool valid_testname = false;

                Assembly a = Assembly.GetExecutingAssembly();
                Type[] types = a.GetTypes();

                foreach (Type t in types)
                {
                    if (t.Name == s_testName)
                    {
                        v = Activator.CreateInstance(t, new object[] {  s_formX, s_formY, s_formwidth, s_formheight });
                        valid_testname = true;
                        break;
                    }
                }

                if (!valid_testname)
                {
                    throw new Exception("Unable to find the test " + s_testName);
                }
            }
            catch (Exception e)
            {
                CommonLib.Log.LogEvidence(e.ToString());
                CommonLib.Log.LogStatus("Failed to launch the test in reflection");
                CommonLib.Log.Result = TestResult.Fail;
                return null;
            }
            s_source.RootVisual = (Visual)v;
            return null;
        }

        //------------------------------------------------------
        private static object PerformVisualVerification(object o)
        {
            bool bResult = false;
            System.Drawing.Bitmap bmp = null;

            ImageComparator hcic;

            CurveTolerance tolerance = null;
            //Adding ability to load custom tolerance curve
            if (File.Exists(s_toleranceFile))
            {
                tolerance = new CurveTolerance();
                tolerance.LoadTolerance(s_toleranceFile);
                hcic = new ImageComparator(tolerance);
                GlobalLog.LogStatus("Using Tolerance File: " + s_toleranceFile);
            }
            else
            {
                hcic = new ImageComparator();
                GlobalLog.LogStatus("Using Default Tolerance");
            }


            string useMaster = "Master_" + s_testName + (s_isHardware ? "_HW.bmp" : "_SW.bmp");

            // Calls HwndTarget.Render() to force the batch to be committed before screen capture
            Assembly mcasm = Assembly.GetAssembly(typeof(System.Windows.Interop.HwndTarget));
            Type mcType = mcasm.GetType("System.Windows.Media.MediaContext");
            object mc = mcType.InvokeMember("From", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { System.Windows.Threading.Dispatcher.CurrentDispatcher });
            mcType.InvokeMember("CompleteRender", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod, null, mc, new object[] { });

            if (null == hcic)
            {
                throw new System.ApplicationException("Failed to instantiate VScan");
            }

            CommonLib.Log.LogStatus("Setting up VScan");

            // This set of tolerance control points is equivalent to the "Good" level in GTO
            //       Tolerance curve entries should not be added after ImageComparator is created since by default
            //       these gets added to dpi=2.0 profile, therefore, is not being used by the comparator. 
            //       Leaving as is for now. The real fix should happen in VScan side.
            if (tolerance == null)
            {
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

            CommonLib.Log.LogStatus("Capturing BMP of the AvalonApp Window");
            bmp = ImageUtility.CaptureScreen(s_source.Handle, true);

            if (s_isMaster)
            {
                // Save the image so that it can be used as a new master image.
                CommonLib.Log.LogStatus("Saving the Rendered Image as " + useMaster);
                bmp.Save(useMaster);
                CommonLib.Log.LogStatus("Exiting without results for Visual Verification");
            }
            else
            {
                if (File.Exists(useMaster) == false)
                {
                    throw new Exception("Unable to find " + useMaster);
                }

                CommonLib.Log.LogStatus("Using Master : " + useMaster);
                ImageAdapter imgMaster = new ImageAdapter(useMaster);
                imgMaster.DpiX = (double)s_defaultDpi;
                imgMaster.DpiY = (double)s_defaultDpi;

                CommonLib.Log.LogStatus("Capturing Rendered Image");
                ImageAdapter imgCapture = new ImageAdapter(bmp);

                CommonLib.Log.LogStatus("Comparing to Master");
                if (s_dpiRatio == 1)
                {
                    bResult = hcic.Compare(imgMaster, imgCapture, true);
                }
                else
                {
                    bResult = hcic.Compare(imgMaster,
                                         new System.Drawing.Point(s_defaultDpi, s_defaultDpi),
                                         imgCapture,
                                         new System.Drawing.Point((int)(s_defaultDpi*s_dpiRatio), (int)(s_defaultDpi*s_dpiRatio)),
                                         true);
                }


                // If VisualVerification fails, then save the VScan failure analysis package
                if (!bResult)
                {
                    string packageName = s_testName + (s_isHardware ? "_HW" : "_SW") + ".vscan";
                    CommonLib.Log.LogStatus("Failure - Saving failure analysis package - " + packageName);
                    Package package = Package.Create(packageName,
                                                     ImageUtility.ToBitmap(imgMaster),
                                                     ImageUtility.ToBitmap(imgCapture),
                                                     hcic.Curve.CurveTolerance.WriteToleranceToNode());
                    package.Save();
                }

                // Log the Visual Verification Result.  If it is False it will override a True result from the
                // analytical verification.
                LogTestResult(bResult);
            }

            bmp.Dispose();

            return o;
        }

        //------------------------------------------------------
        private static void LogTestResult(bool result)
        {
            CommonLib.Log.LogStatus("Logging the Final Test Result");
            CommonLib.LogTest(result, s_testName + ": " + (result ? "Pass" : "Fail"));
        }

        //------------------------------------------------------
        private static object Quit(object arg)
        {
            s_dispatcher.InvokeShutdown();
            return null;
        }
        #endregion Private Methods

        //------------------------------------------------------
        #region Public Methods
        internal static IntPtr CloseWindowManually(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the Window is closed manually
            if (msg == s_WM_CLOSE)
            {
                s_dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(Quit), null);
                handled = true;
            }

            return IntPtr.Zero;
        }
        #endregion Public Methods

        //------------------------------------------------------
        #region CommandLine Parser
        internal static bool ReadCommandLine(string[] args)
        {
            // If no args are supplied or "?" is entered, then display the command line usage instructions.
            if (args.Length < 1 || GetArgument("?", args) != null)
            {
                ShowUsage();
                return false;
            }

            // Parse the TestName from the command line args
            s_testName = args[0];

            // Parse the FormWidth from the command line args
            // Default is 200.
            if (GetArgument("FormWidth", args) != null)
            {
                s_formwidth = int.Parse(GetArgument("FormWidth", args));
            }

            // Parse the FormHeight from the command line args
            // Default is 200.
            if (GetArgument("FormHeight", args) != null)
            {
                s_formheight = int.Parse(GetArgument("FormHeight", args));
            }

            // Parse the FormX from the command line args
            // X position of the TopLeft corner of the Window
            // Default is 0.
            if (GetArgument("FormX", args) != null)
            {
                s_formX = int.Parse(GetArgument("FormX", args));
            }

            // Parse the FormY from the command line args
            // Y position of the TopLeft corner of the Window
            // Default is 0.
            if (GetArgument("FormY", args) != null)
            {
                s_formY = int.Parse(GetArgument("FormY", args));
            }

            // Parse the Automated/Manual Window switch from the command line args
            // Default is True.
            if (GetArgument("Auto", args) != null)
            {
                // Determines whether or not the Window is frameless and closes
                // automatically or whether the Window has a frame and must be
                // closed Automatically.
                // Default is a frameless Window that closes Automatically
                s_isAutoTest = bool.Parse(GetArgument("Auto", args));
            }

            // Parse the VisualVerification switch from the command line args
            // Default is False.
            if (GetArgument("VV", args) != null)
            {
                // Determines whether or not to use VisualVerification or to just log a
                // pass or fail based on programmatic results.
                // Default is to NOT use VisualVerification
                s_useVisualVerification = bool.Parse(GetArgument("VV", args));
            }

            // Parse the VisualVerification tolerance file from the command line args
            if (GetArgument("Tolerance", args) != null)
            {
                s_toleranceFile = GetArgument("Tolerance", args);
            }

            // Parse the Hardware switch from the command line args
            // Default is False.
            if (GetArgument("HW", args) != null)
            {
                // Determines whether or not to add an HW to the name of the Master or Rendered Image.
                // This is neccessary to have seperate Master images for SW and HW.
                // Default is to add SW to name and NOT HW.
                s_isHardware = bool.Parse(GetArgument("HW", args));
            }

            // Parse the Master switch from the command line args
            // Default is False.
            if (GetArgument("Master", args) != null)
            {
                // Determines whether or not to capture a Master image for Visual Verification
                // or to just compare the rendered image to an existing master image.
                // Default is to NOT capture a Master image.
                s_isMaster = bool.Parse(GetArgument("Master", args));
            }

            if (GetArgument("XamlTest", args) != null)
            {
                // Determins whether this is a standalone XmlTest which loads and instantiates 
                // the object specified in [TestName].xaml to start the test
                s_isXamlTest = true;
            }

            if (GetArgument("XamlTestVar", args) != null)
            {
                // Specifies the test variation name to be passed onto XamlTest.
                // Ignored for non-XamlTest.

                throw new NotImplementedException("Harness isn't available - use DriverState or Cross Process Dictionary");
                //Harness.Current["XamlTestVar"] = GetArgument("XamlTestVar", args);
            }



            // Valid number of command line arguments have been entered
            return true;
        }

        //------------------------------------------------------
        private static string GetArgument(string name, string[] args)
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

        //------------------------------------------------------
        private static void ShowUsage()
        {
            MessageBox.Show("APITestLoader.exe TestName /XamlTest /XamlTestVar: /FormWidth: /FormHeight: /FormX: /FormY: /Auto: " +
            "/VV: /HW: /Master:\n\n" +
            "DLL Name\t\t\tName of the Test DLL that contains a WCP_MILAPITest.\n" +
            "XamlTest (optional)\tLoads a standalone XAML test. \n" +
            "XamlTestVar (optional) \tName of the test variation to be passed onto XamlTest. (Ignored for non-XamlTests)\n" +
            "Test Name\t\tName of the individual WCP_MILAPITest or XamlTest to run.\n" +
            "FormWidth (optional)\tWidth of the Window. (Default=200)\n" +
            "FormHeight (optional)\tHeight of the Window. (Default=200)\n" +
            "FormX (optional)\t\tX Position of the Top Left corner of the Window. (Default=0)\n" +
            "FormY (optional)\t\tY Position of the Top Left corner of the Window. (Default=0)\n" +
            "Auto (optional)\t\tFalse=Framed Window that closes Manually\n" +
            "\t\t\tTrue=Frameless Window that closes Automatically. (Default=True)\n" +
            "VV (optional)\t\tFalse=Don't use VisualVerification  True=Programmatic Verification only (Default=False)\n" +
            "HW (optional)\t\tTrue=Test uses Hardware Acceleration  False=Test uses Software Rendering (Default=False)\n" +
            "Master (optional)\t\tTrue=Store rendered bitmap as Master  False=Don't store rendered bitmap (Default=False)\n", "WCP_MILAPITestLoader - Command Line Usage");
        }
        #endregion CommandLine Parser


        //------------------------------------------------------
        #region Data Members

        private static Dispatcher s_dispatcher;
        private static int s_WM_CLOSE = 0x10;
        private static HwndSource s_source;
        private static string s_testName;
        private static double s_formwidth = 200;
        private static double s_formheight = 200;
        private static int s_formX = 0;
        private static int s_formY = 0;
        private static bool s_isAutoTest = true;
        private static bool s_useVisualVerification = false;
        private static bool s_isHardware = false;
        private static bool s_isMaster = false;
        private static bool s_isXamlTest = false;
        private static double s_dpiRatio = 1.0;
        private static string s_toleranceFile = "2DApiTestTolerance.xml"; // Vscan Tolerance filename
        private static readonly int s_defaultDpi = 96;
        #endregion Data Members
    }
}
