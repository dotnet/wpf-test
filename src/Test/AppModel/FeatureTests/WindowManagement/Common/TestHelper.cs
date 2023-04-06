// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using System.Xml;
    using Microsoft.Test.Logging;
    using Microsoft.Test.RenderingVerification; 
    using Microsoft.Test.RenderingVerification.Model.Analytical;
    using Microsoft.Test;
    
    public partial class TestHelper
    {
        // Private properties
        private string[] _appArgs = null;
        private static bool s_testCleanupCalled = false;


        // Static properties
        static private TestHelper s_currentInstance = null;

        public TestHelper()
        {
            Initialize(null);
        }
        
        public TestHelper(string[] appArgs)
        {
            Initialize(appArgs);
        }

        private void Initialize(string[] args)
        {            
            s_currentInstance = this;
            if (args != null && args.Length != 0)
            {
               _appArgs = args;
            }
            else
            {
                _appArgs = new String[] { DriverState.DriverParameters["TestName"], DriverState.DriverParameters["Param1"], DriverState.DriverParameters["Param2"], DriverState.DriverParameters["Param3"] };
            }
        }

        public static void CloseAndSetVariationResults()
        {
            if (!s_testCleanupCalled)
            {
                s_testCleanupCalled = true; 
                Logger.Status("Shutting down application");
                Log.Current.CurrentVariation.LogResult(Logger.CachedResult);
                // Must close variation to register a pass.
                Log.Current.CurrentVariation.Close();
            }
        }

        public void TestCleanup()
        {
            // Note: This Cleanup always set pass before.
            // Confusing, but seems to work.
            Logger.CachedResult = Result.Pass;
            CloseAndSetVariationResults();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback) delegate (object o)
            {
                try
                {
                    Application.Current.Shutdown();
                }
                catch(System.Security.SecurityException ex)
                {
                    Logger.Status("SecurityException caught. " + ex.ToString());
                }
                catch(System.Exception ex)
                {
                    Logger.LogFail("Application Exit threw exception. " + ex.ToString());
                }
                return null;
            },
            null);
        }

        static public TestHelper Current
        {
            get
            {
                return s_currentInstance;
            }
        }

        public string[] AppArgs
        {
            set
            {
                _appArgs = value;
            }
            get
            {
                return _appArgs;
            }
        }
    }

    public static class TestUtil
    {
        public static bool IsEqual(double a, double b)
        {
            double delta = a-b;

            if (1.0d >= delta && -1.0d <= delta)
            {
                return true;
            }
            return false;
        }
        
        public static System.Windows.Point DeviceToLogicalUnits(System.Windows.Point ptDeviceUnits, Window w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }

            HwndSource hs = PresentationSource.FromVisual(w) as HwndSource;

            if (hs == null)
            {
                throw new ApplicationException(String.Format("Could not get a valid HwndSource from Window object, hs = {0}", hs));
            }

            if (hs.CompositionTarget == null)
            {
                throw new ApplicationException(String.Format("CompositionTarget is null, hs = {0}", hs.CompositionTarget));
            }
            
            System.Windows.Point ptLogicalUnits = hs.CompositionTarget.TransformFromDevice.Transform(ptDeviceUnits);
            return ptLogicalUnits;
        }

        public static System.Windows.Point LogicalToDeviceUnits(System.Windows.Point ptLogicalUnits, Window w)
        {
            if (w == null)
            {
                throw new ArgumentNullException("w");
            }

            HwndSource hs = PresentationSource.FromVisual(w) as HwndSource;

            if (hs == null)
            {
                throw new ApplicationException(String.Format("Could not get a valid HwndSource from Window object, hs = {0}", hs));
            }

            if (hs.CompositionTarget == null)
            {
                throw new ApplicationException(String.Format("CompositionTarget is null, hs = {0}", hs.CompositionTarget));
            }
            
            System.Windows.Point ptDeviceUnits = hs.CompositionTarget.TransformToDevice.Transform(ptLogicalUnits);
            return ptDeviceUnits;
        }

        public static double DPIXRatio
        {
            get
            {
                return System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiX / 96;
            }
        }

        public static double DPIYRatio
        {
            get
            {
                return System.Drawing.Graphics.FromHwnd(IntPtr.Zero).DpiY / 96;
            }
        }

        public static bool IsThemeClassic
        {
            get
            {
                return Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme().ToLower().Contains("classic");
            }
        }

        public static bool IsThemeLuna
        {
            get
            {
                return Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme().ToLower().Contains("luna");
            }
        }

        public static bool IsThemeRoyale
        {
            get
            {
                return Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme().ToLower().Contains("royale");
            }
        }

        public static bool IsThemeAero
        {
            get
            {
                return Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme().ToLower().Contains("aero");
            }
        }

        public static string CurrentTheme
        {
            get
            {
                return Microsoft.Test.RenderingVerification.DisplayConfiguration.GetTheme();
            }
        }

        public static bool IsLanguageJapanese
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower().Contains("japanese");
            }
        }
        public static bool IsLanguageChinese
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower().Contains("chinese");
            }
        }
        public static bool IsLanguageEnglish
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower().Contains("english");
            }
        }

        public static string CurrentLanguage
        {
            get
            {
                return System.Globalization.CultureInfo.CurrentCulture.EnglishName;
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
    }

    public class TypeConverter
    {
        public static bool StringToDouble(string s, out Double result)
        {
            Logger.Status("  [TypeConverter.StringToDouble] Try to parse string \"" + s + "\" to type Double");
            if (Double.TryParse(s, out result))
            {          
                Logger.Status("  [TypeConverter.StringToDouble] Parsing Successful: NewValue=" + result.ToString());
                return true;
            }               
            else
            {
                Logger.LogFail( "  [TypeConverter.StringToDouble] Parsing Failed: string \"" + s + "\" could not be converted to type Double.");
                return false;
            }
        }

        public static bool StringToWindowState(string s, out WindowState result)
        {
            Logger.Status("  [TypeConverter.StringToWindowState] Try to parse string \"" + s + "\" to WindowState");
            switch(s)
            {
                case "Normal":
                    result = WindowState.Normal;
                    break;
                case "Maximized":
                    result = WindowState.Maximized;
                    break;
                case "Minimized":
                    result = WindowState.Minimized;
                    break;
                default:                    
                    Logger.LogFail( "  [TypeConverter.StringToWindowState] Parsing Failed: string \"" + s + "\" is not a valid WindowState value.");
                    result = WindowState.Normal;
                    return false;
            }
            
            Logger.Status("  [TypeConverter.StringToWindowState] Parsing Successful: WindowState=" + result.ToString());
            return true;
        }        
    }
}
