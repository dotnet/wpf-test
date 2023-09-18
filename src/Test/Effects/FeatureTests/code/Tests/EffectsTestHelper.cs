// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Helper for Effects testing
 * Owner: Microsoft 
 ********************************************************************/
using System;
using System.Windows;
using System.Xml;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Controls;
using System.Reflection;
using Microsoft.Test.Logging;
using Microsoft.Test.Graphics;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using Microsoft.Test.VisualVerification;
using System.Drawing.Imaging;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Helper methods for Effects test. 
    /// </summary>`
    public class EffectsTestHelper
    {
        #region Internal Data

        internal static readonly string LocalResourceDictionaryPath = "InvariantTheme.xaml";
        internal static readonly double defaultSystemDpi = 96.0;
        internal static double TopWindowOffset = 20;
        internal static double LeftWindowOffset = 20;
        public static readonly TimeSpan IntervalForRender = TimeSpan.FromMilliseconds(1600);

        #endregion 

        #region Methods
        /// <summary>
        /// Test Clone and CloneCurrentValue method in Effect types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="effect"></param>
        /// <returns></returns>
        internal static bool TestClone<T>(Effect effect) where T : Effect
        {
            T clone = effect.Clone() as T;

            if (clone == null)
            {
                TestLog.Current.LogStatus("Value from Clone is null.");
                return false;
            }

            if (!ObjectUtils.DeepEquals(effect, clone))
            {
                TestLog.Current.LogStatus("Cloned effect has different value.");
                return false;
            }
            //Cloned effect is the same. 
            return true;
        }

        /// <summary>
        ///  Test CloneCurrentValue method in Effect types
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        internal static bool TestCloneCurrentValue<T>(Effect effect) where T: Effect
        {
            T clone = effect.CloneCurrentValue() as T;
            if (clone == null)
            {
                TestLog.Current.LogStatus("Value from Clone is null.");
                return false;
            }

            if (!ObjectUtils.DeepEquals(effect, clone))
            {
                TestLog.Current.LogStatus("Cloned effect has different value.");
                return false;
            }
            //Cloned effect is the same. 
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="masterFileName"></param>
        /// <returns></returns>
        public static bool VerifyWindowAgainstMasterImage(Window window, string masterFileName, string tolerenceFilePath)
        {
            Snapshot capture = SnapshotHelper.SnapshotFromWindow(window, WindowSnapshotMode.IncludeWindowBorder);
            Snapshot master = null;
               
            bool passed = false;
            if (File.Exists(masterFileName))
            {
                master = Snapshot.FromFile(masterFileName);
            }

            if (master != null)
            {
                SnapshotVerifier verifier = new SnapshotHistogramVerifier(Histogram.FromFile(tolerenceFilePath));
                Snapshot diff = master.CompareTo(capture);
                passed = (verifier.Verify(diff) == VerificationResult.Pass);
            }

            if (!passed)
            {
                string renderedFileName = "rendered_" + masterFileName;
                LogPngSnapshot(capture, renderedFileName);
                LogPngSnapshot(master, masterFileName);
            }
            return passed;
        }

        internal static Window CreateWindow(double windowWidth, double windowHeight, bool allowsTransparency, FrameworkElement content)
        {
            Window window = new Window();
            window.AllowsTransparency = allowsTransparency;
            SetupEffectsTestWindow(window, windowWidth, windowHeight, content);
            
            return window;
        }

        internal static void SetupEffectsTestWindow(Window window, double windowWidth, double windowHeight, FrameworkElement content)
        {
            window.Topmost = true;
            window.WindowStyle = WindowStyle.None;
            window.BorderThickness = new Thickness(0);
            window.ResizeMode = ResizeMode.NoResize;

            //Not moving to 0,0 to avoid mouseover. 
            window.Left = LeftWindowOffset;
            window.Top = TopWindowOffset;
            window.Height = windowHeight;
            window.Width = windowWidth;

            //Set Content
            window.Content = content;

            //scale down to default system dpi to use same master for different dpi. 
            DpiScalingHelper.ScaleWindowToFixedDpi(window, defaultSystemDpi, defaultSystemDpi);

            AddLocalResourceDictionary(window);
        }

        /// <summary>
        /// Add a locally available resource dictionary to the element. 
        /// The purpose is to prevent the element from rendering differently 
        /// with different theme. 
        /// </summary>
        /// <param name="root">element to add the resource dictionary</param>
        /// <param name="ResourceFileName">file name of the resource</param>
        internal static void AddLocalResourceDictionary(System.Windows.FrameworkElement root)
        {
            System.Windows.ResourceDictionary resourceDictionary;
            try
            {
                resourceDictionary = Microsoft.Test.Serialization.SerializationHelper.ParseXamlFile(LocalResourceDictionaryPath) as System.Windows.ResourceDictionary;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Cannot load ResourceDictionary from file: " + LocalResourceDictionaryPath, e);
            }

            root.Resources.MergedDictionaries.Add(resourceDictionary);
        }


        /// <summary>
        /// Log a Snapshot into a file in Png format. 
        /// </summary>
        /// <param name="snapshot">The Snapshot to be logged.</param>
        /// <param name="name">File name.</param>

        public static void LogPngSnapshot(Snapshot snapshot, string name)
        {
            if (snapshot != null)
            {
                snapshot.ToFile(name, ImageFormat.Png);
                GlobalLog.LogFile(name);
            }
            else
            {
                GlobalLog.LogStatus(string.Format("snapshot : {0} is null.", name));
            }
        }

        /// <summary>
        /// Sets the window containing the Visual to render in certain RenderMode.
        /// </summary>
        internal static void SetRenderMode(Visual visual, RenderMode renderMode)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(visual) as HwndSource;
            HwndTarget hwndTarget = hwndSource.CompositionTarget;
            hwndTarget.RenderMode = renderMode;
        }

        #endregion
    }
}